using System;
using System.Collections.Generic;
using System.Collections;
using System.Net;
using System.Net.Mail;
using System.Linq;

namespace Vacation.BLL
{
    using IDAL;
    using Model;

    public class AccountManager
    {
        private IDataManager m_dataManager = null;
        private ProcessManager m_processManager = null;

        public AccountManager(IDataManager dataManager, ProcessManager processManager)
        {
            m_dataManager = dataManager;
            m_processManager = processManager;
        }

        public Models.Account.LoginResult Login(string strUserID, string strPW)
        {
            return Login(strUserID, strPW, false);
        }

        private Models.Account.LoginResult Login(string strUserID, string strPW, bool ignorePassword)
        {
            Models.Account.LoginResult result = new Models.Account.LoginResult();

            Dictionary<CompanyMember.Fields, object> dicConditions = new Dictionary<CompanyMember.Fields, object>();
            dicConditions[CompanyMember.Fields.UserID] = strUserID;

            string strErrorMessage = null;
            ArrayList arrDatas = m_dataManager.GetSelectManager().SelectCompanyMemberJobLevelRegularTeam(dicConditions, null, null, out strErrorMessage);
            Models.Account.ApplicationUser user = null;

            if (arrDatas == null || strErrorMessage != null)
            {
                result.Success = false;
                result.Message = strErrorMessage;
                return result;
            }

            if (arrDatas.Count == 3 &&
                arrDatas[0] is CompanyMember &&
                arrDatas[1] is JobLevel &&
                arrDatas[2] is RegularTeam)
            {
                CompanyMember member = (CompanyMember)arrDatas[0];
                JobLevel level = (JobLevel)arrDatas[1];
                RegularTeam team = (RegularTeam)arrDatas[2];

                if (!ignorePassword && member.Password != strPW)
                {
                    strErrorMessage = "비밀번호가 일치하지 않습니다.";

                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
                else
                {
                    result.Options = ReadOptions();

                    result.User = Models.Account.ApplicationUser.MakeUser(member, level, team);
                    result.User.ReservationMonth = m_processManager.GetVacationManager().ReservationMonth;
                    result.Success = true;
                    return result;
                }
            }
            else
            {
                strErrorMessage = string.Format("{0}에 대한 직원정보를 찾을수 없습니다.", strUserID);

                result.Success = false;
                result.Message = strErrorMessage;
            }

            return result;
        }

        public Models.Account.ExternalLoginResult ExternalLogin(Models.Account.ExternalLoginData data)
        {
            return ExternalLogin(data, false, null);
        }

        private Models.Account.ExternalLoginResult ExternalLogin(Models.Account.ExternalLoginData data, bool ignorePassword, string strLoginKey)
        {
            if (data.UserID == null)
                return new Models.Account.ExternalLoginResult(false, "UserID가 null입니다.");

            if (!ignorePassword)
            {
                if (data.HashCode == null && data.Password == null)
                    return new Models.Account.ExternalLoginResult(false, "비밀번호가 null입니다.");
            }

            Dictionary<CompanyMember.Fields, object> dicConditions = new Dictionary<CompanyMember.Fields, object>();
            dicConditions[CompanyMember.Fields.UserID] = data.UserID;

            string strErrorMessage;
            List<CompanyMember> members = m_dataManager.GetSelectManager().SelectCompanyMembers(dicConditions, out strErrorMessage);

            if (strErrorMessage != null || members == null)
                return new Models.Account.ExternalLoginResult(false, strErrorMessage);

            if (members.Count == 0)
                return new Models.Account.ExternalLoginResult(false, string.Format("ID 또는 비밀번호를 잘못 입력하였습니다."));

            CompanyMember member = members[0];

            if (member.Password == null || member.Password.Length == 0)
                return new Models.Account.ExternalLoginResult(false, "아직 비밀번호가 설정되지 않은 계정입니다.");

            if (!ignorePassword)
            {
                if (data.HashCode != null)
                {
                    if (data.HashCode != member.Password)
                        return new Models.Account.ExternalLoginResult(false, string.Format("ID 또는 비밀번호를 잘못 입력하였습니다."));
                }
                else
                {
                    System.Security.Cryptography.SHA256 sha = System.Security.Cryptography.SHA256.Create();
                    byte[] bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(data.Password));
                    string strEnc = Enumerable.Aggregate(bytes, string.Empty, (current, x) => current + $"{x:x2}");

                    if (strEnc != member.Password)
                        return new Models.Account.ExternalLoginResult(false, string.Format("ID 또는 비밀번호를 잘못 입력하였습니다."));
                }
            }

            RegularTeam team = m_dataManager.GetSelectManager().SelectReqularTeam(member.TeamID, out strErrorMessage);

            Models.Account.ExternalLoginResult result = new Models.Account.ExternalLoginResult(true, "");

            result.UserID = data.UserID;
            result.Name = member.Name;
            result.TeamName = team == null ? "" : team.Name;

            long loginKey;

            if (strLoginKey == null)
            {
                loginKey = GetGuid();
                result.LoginKey = loginKey.ToString();
            }
            else
            {
                if (long.TryParse(strLoginKey, out loginKey))
                    result.LoginKey = strLoginKey;
                else
                {
                    loginKey = GetGuid();
                    result.LoginKey = loginKey.ToString();
                }
            }

            ExternalLogin login = m_processManager.GetDataManager().GetSelectManager().SelectExternalLogin(data.UserID, out strErrorMessage);

            if (login == null && strErrorMessage != null)
            {
                return new Models.Account.ExternalLoginResult(false, strErrorMessage);
            }
            else if (login == null)
            {
                login = m_processManager.GetDataManager().GetCreateManager().CreateExternalLogin(data.UserID, loginKey, DateTime.Now, false, out strErrorMessage);

                if (login == null || strErrorMessage != null)
                    return new Models.Account.ExternalLoginResult(false, strErrorMessage);
            }
            else
            {
                login.LoginKey = loginKey;
                login.LoginTime = DateTime.Now;

                if (m_processManager.GetDataManager().GetUpdateManager().UpdateExternalLogin(login, out strErrorMessage) == false)
                {
                    return new Models.Account.ExternalLoginResult(false, strErrorMessage);
                }
            }

            return result;
        }

        public Models.MessageResult ExternalLogout(string strUserID)
        {
            string strErrorMessage;

            if (m_processManager.GetDataManager().GetDeleteManager().DeleteExternalLogin(strUserID, out strErrorMessage) == false)
                return new Models.MessageResult(false, strErrorMessage);

            return new Models.MessageResult(true, "");
        }

        private string HexToString(string strHex)
        {
            int len = strHex.Length;

            if (len == 0)
                return "";

            byte[] bytes = new byte[len / 2];

            for (int i=0;i<strHex.Length-1;i+=2)
            {
                int data;

                if (int.TryParse(strHex.Substring(i, 2), System.Globalization.NumberStyles.HexNumber, null, out data) == false)
                    return "";

                byte b = (byte)data;
                bytes[i / 2] = b;
            }

            return System.Text.Encoding.UTF8.GetString(bytes);
        }

        private ExternalLogin ParseBeginCode(string strBeginCode, out string strErrorMessage)
        {
            strErrorMessage = null;

            if (strBeginCode == null)
            {
                strErrorMessage = "시작코드가 null입니다.";
                return null;
            }

            long beginCode;

            if (long.TryParse(strBeginCode, out beginCode) == false)
            {
                strErrorMessage = "유효하지 않은 시작코드입니다.";
                return null;
            }

            Dictionary<Vacation.Model.ExternalLogin.Fields, object> dicConditions = new Dictionary<Vacation.Model.ExternalLogin.Fields, object>();
            dicConditions[Vacation.Model.ExternalLogin.Fields.LoginKey] = beginCode;

            List<ExternalLogin> logins = m_dataManager.GetSelectManager().SelectExternalLogins(dicConditions, null, null, out strErrorMessage);

            if (logins == null || strErrorMessage != null)
                return null;

            if (logins.Count == 0)
            {
                strErrorMessage = "유효하지 않은 시작코드입니다.";
                return null;
            }

            return logins[0];
        }

        public Models.Account.ExternalLoginResult ExternalAutoLogin(string strBeginCode)
        {
            string strErrorMessage;

            ExternalLogin login = ParseBeginCode(strBeginCode, out strErrorMessage);

            if (login == null)
                return new Models.Account.ExternalLoginResult(false, strErrorMessage);

            if (login.Enabled == false)
                return new Models.Account.ExternalLoginResult(false, "유효하지 않은 시작코드입니다.");

            login.Enabled = false;

            if (m_dataManager.GetUpdateManager().UpdateExternalLogin(login, out strErrorMessage) == false)
                return new Models.Account.ExternalLoginResult(false, strErrorMessage);

            Models.Account.ExternalLoginData data = new Models.Account.ExternalLoginData();
            data.UserID = login.UserID;

            return ExternalLogin(data, true, login.LoginKey.ToString());
        }

        public Models.Account.LoginResult AutoLogin(string strBeginCode)
        {
            string strErrorMessage;

            ExternalLogin login = ParseBeginCode(strBeginCode, out strErrorMessage);

            if (login == null)
                return new Models.Account.LoginResult(false, strErrorMessage);

            if (login.Enabled == false)
                return new Models.Account.LoginResult(false, "유효하지 않은 시작코드입니다.");

            login.Enabled = false;

            if (m_dataManager.GetUpdateManager().UpdateExternalLogin(login, out strErrorMessage) == false)
                return new Models.Account.LoginResult(false, strErrorMessage);

            return Login(login.UserID, null, true);
        }

        public Models.Account.ExternalLoginResult RequestNewLoginKey(string strBeginCode)
        {
            string strErrorMessage;

            ExternalLogin login = ParseBeginCode(strBeginCode, out strErrorMessage);

            if (login == null)
                return new Models.Account.ExternalLoginResult(false, strErrorMessage);

            login.LoginKey = GetGuid();
            login.Enabled = true;

            if (m_dataManager.GetUpdateManager().UpdateExternalLogin(login, out strErrorMessage) == false)
                return new Models.Account.ExternalLoginResult(false, strErrorMessage);

            Models.Account.ExternalLoginResult result = new Models.Account.ExternalLoginResult(true, "");
            result.LoginKey = login.LoginKey.ToString();
            return result;
        }

        private long GetGuid()
        {
            byte[] bytes = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt64(bytes, 0);
        }

        private Models.Vacation.Options ReadOptions()
        {
            string strErrorMessage;
            List<VacationOption> _options = m_dataManager.GetSelectManager().SelectOptions(null, out strErrorMessage);

            Models.Vacation.Options options = new Models.Vacation.Options();

            foreach (VacationOption option in _options)
            {
                if (string.Compare(option.PropertyName, "MinSpecialVacationDays", true) == 0)
                {
                    float fDays;

                    if (float.TryParse(option.PropertyValue, out fDays))
                    {
                        options.MinSpecialVacationDays = fDays;
                    }
                }
                else if (string.Compare(option.PropertyName, "MaxSpecialVacationDays", true) == 0)
                {
                    float fDays;

                    if (float.TryParse(option.PropertyValue, out fDays))
                    {
                        options.MaxSpecialVacationDays = fDays;
                    }
                }
            }

            options.UsingType = ScheduleManager.UsingType.ToString();
            return options;
        }

        public bool SendRegistEmail(string strSystemMail, string strSystemCode, string strUserName, string strUserID, string strEmail, string strURL, out string strResultMessage)
        {
            strResultMessage = null;
            ISelectManager selectManager = m_dataManager.GetSelectManager();

            Dictionary<CompanyMember.Fields, object> dicConditions = new Dictionary<CompanyMember.Fields, object>();
            dicConditions[CompanyMember.Fields.Name] = strUserName;
            dicConditions[CompanyMember.Fields.UserID] = strUserID;

            //string strCondition = string.Format("Name = '{0}' and UserID = '{1}'", strUserName, strUserID);
            List<CompanyMember> members = selectManager.SelectCompanyMembers(dicConditions, out strResultMessage);

            if (members == null || strResultMessage != null)
                return false;

            if (members.Count == 0)
            {
                strResultMessage = "해당 조건에 맞는 직원정보를 찾을수 없습니다.";
                return false;
            }

            CompanyMember member = members[0];
            string strEmailTitle = "", strMessage = "", strSubject = "";

            if (member.Password != null)
            {
                // 비밀번호 변경일 경우
                strSubject = "[U&E Internal] 비밀번호 변경 안내메일입니다.";

                strMessage = "시스템에 등록한 비밀번호는 암호화되어 안전하게 보관됩니다.\r\n";
                strMessage += "아래의 링크를 눌러 변경을 완료하시기 바랍니다.\r\n\r\n";
                strMessage += strURL;

                strEmailTitle = "비밀번호 변경안내";
            }
            else
            {
                // 사용자 등록일 경우
                strSubject = "[U&E Internal] 사용자 등록 안내메일입니다.";

                strMessage = "U&E Internal 사이트를 이용하기 위해서는 사용자 등록을 하여야 합니다.\r\n";
                strMessage += "아래의 링크를 눌러 사용자 등록을 완료하시기 바랍니다.\r\n\r\n";
                strMessage += strURL;

                strEmailTitle = "사용자 등록안내";
            }

            DateTime dtNow = DateTime.Now;
            string strNow = dtNow.ToBinary().ToString();
            string strCheckSum = (member.ID + dtNow.Millisecond).ToString();
            string strCode = members[0].ID + "_" + strUserID + "_" + strNow + "_" + strCheckSum;
            string strEncryptedCode = m_dataManager.Encrypt(strCode);

            JobLevel jobLevel = m_dataManager.GetSelectManager().SelectJobLevel(member.JobLevelID, out strResultMessage);

            if (jobLevel == null)
                return false;

            if (jobLevel.LevelName == "사원")
            {
                strMessage = string.Format("어서오세요 {0}님\r\n\r\n", strUserName) + strMessage;
            }
            else
            {
                strMessage = string.Format("어서오세요 {0} {1}님\r\n\r\n", strUserName, jobLevel.LevelName) + strMessage;
            }

            strMessage += "?code=" + System.Web.HttpUtility.UrlEncode(strEncryptedCode);

            Dictionary<CompanyMember.Fields, object> dicSets = new Dictionary<CompanyMember.Fields, object>();
            dicSets[CompanyMember.Fields.PasswordCode] = strNow;

            string strErrorMessage;
            if (m_dataManager.GetUpdateManager().UpdateCompanyMember(dicSets, dicConditions, out strErrorMessage) == false)
            {
                strResultMessage = strErrorMessage;
                return false;
            }

            return SendEmail(strSystemMail, strSystemCode, strEmail, strSubject, strMessage, strEmailTitle, ref strResultMessage);
        }

        private bool SendEmail(string strSystemMail, string strSystemCode, string strEmail, string strSubject, string strMessage, string strEmailTitle, ref string strResultMessage)
        {
            try
            {
                // Credentials
                var credentials = new NetworkCredential(strSystemMail, strSystemCode);

                // Mail message
                var mail = new MailMessage()
                {
                    From = new MailAddress(strSystemMail),
                    Subject = strSubject,
                    Body = strMessage
                };

                mail.To.Add(new MailAddress(strEmail));

                // Smtp client
                var client = new SmtpClient()
                {
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = "smtp.gmail.com",
                    EnableSsl = true,
                    Credentials = credentials
                };

                // Send it...         
                client.Send(mail);
            }
            catch (Exception ex)
            {
                strResultMessage = "Error in sending email: " + ex.Message;
                return false;
            }

            if (strEmailTitle != null && strEmailTitle.Length > 0)
                strResultMessage = strEmailTitle + " 메일이 발송되었습니다.\r\n메일을 확인해 주세요.";
            else
                strResultMessage = "메일이 발송되었습니다.\r\n메일을 확인해 주세요.";

            return true;
        }

        public bool CheckRegisterParam(string strCode, out string strUserName, out string strJobLevel, out string strUserID, out string strResultMessage)
        {
            strUserName = strJobLevel = strUserID = strResultMessage = "";

            try
            {
                string strData = m_dataManager.Decrypt(strCode);
                string[] tokens = strData.Split('_');

                if (tokens.Length != 4)
                {
                    strResultMessage = "유효하지 않은 Code입니다.";
                    return false;
                }

                string strID = tokens[0].Trim();
                strUserID = tokens[1].Trim();
                string strTime = tokens[2].Trim();
                string strCheckSum = tokens[3].Trim();

                int id;
                long time;

                if (int.TryParse(strID, out id) == false ||
                    long.TryParse(strTime, out time) == false)
                {
                    strResultMessage = "유효하지 않은 Code입니다.";
                    return false;
                }

                DateTime timeStamp = DateTime.FromBinary(time);

                if (strCheckSum != (timeStamp.Millisecond + id).ToString())
                {
                    strResultMessage = "유효하지 않은 Code입니다.";
                    return false;
                }

                CompanyMember member = m_dataManager.GetSelectManager().SelectCompanyMember(id, out strResultMessage);

                if (member == null || strResultMessage != null)
                    return false;

                if (member.UserID != strUserID ||
                    member.PasswordCode != strTime)
                {
                    strResultMessage = "유효하지 않은 Code입니다.";
                    return false;
                }

                JobLevel level = m_dataManager.GetSelectManager().SelectJobLevel(member.JobLevelID, out strResultMessage);

                if (level == null || strResultMessage != null)
                {
                    strResultMessage = "유효하지 않은 Code입니다.";
                    return false;
                }

                strUserName = member.Name;
                strJobLevel = level.LevelName;
                strResultMessage = "";
                return true;
            }
            catch (Exception e)
            {
                strResultMessage = e.Message;
            }

            return false;
        }

        public bool SetPassword(string strUserID, string strPassword, out string strErrorMessage)
        {
            Dictionary<CompanyMember.Fields, object> dicConditions = new Dictionary<CompanyMember.Fields, object>();
            dicConditions[CompanyMember.Fields.UserID] = strUserID;

            List<CompanyMember> members = m_dataManager.GetSelectManager().SelectCompanyMembers(dicConditions, out strErrorMessage);

            if (members == null || strErrorMessage != null)
                return false;

            if (members.Count == 0)
            {
                strErrorMessage = string.Format("{0}에 대한 직원정보를 찾을수 없습니다.", strUserID);
                return false;
            }

            CompanyMember member = members[0];

            dicConditions.Clear();
            dicConditions[CompanyMember.Fields.ID] = member.ID;

            Dictionary<CompanyMember.Fields, object> dicSets = new Dictionary<CompanyMember.Fields, object>();
            dicSets[CompanyMember.Fields.UserPW] = strPassword;
            dicSets[CompanyMember.Fields.PasswordCode] = null;

            return m_dataManager.GetUpdateManager().UpdateCompanyMember(dicSets, dicConditions, out strErrorMessage);
            //string strCondition = string.Format("set UserPW = '{0}' where ID = {1}", strPassword, member.ID);
            //return m_dataManager.GetUpdateManager().UpdateCompanyMember(strCondition, out strErrorMessage);
        }

        public CompanyMember AddMember(string name, int jobLevelID, DateTime startDate, int teamID, bool isTeamLeader, bool isAdmin, string strUserID, string strPhoneNumber, out string strErrorMessage)
        {
            Dictionary<CompanyMember.Fields, object> dicConditions = new Dictionary<CompanyMember.Fields, object>();
            dicConditions[CompanyMember.Fields.UserID] = strUserID;

            List<CompanyMember> members = m_dataManager.GetSelectManager().SelectCompanyMembers(dicConditions, out strErrorMessage);

            if (members == null || strErrorMessage != null)
                return null;

            if (members.Count > 0)
            {
                strErrorMessage = string.Format("{0}는 이미 존재하는 id입니다.", strUserID);
                return null;
            }

            if (strPhoneNumber != null && strPhoneNumber.Length > 0)
            {
                dicConditions.Clear();
                dicConditions[CompanyMember.Fields.PhoneNumber] = strPhoneNumber;

                members = m_dataManager.GetSelectManager().SelectCompanyMembers(dicConditions, out strErrorMessage);

                if (members == null || strErrorMessage != null)
                    return null;

                if (members.Count > 0)
                {
                    strErrorMessage = string.Format("{0}는 이미 존재하는 전화번호 입니다.", strPhoneNumber);
                    return null;
                }
            }

            CompanyMember member = m_dataManager.GetCreateManager().CreateCompanyMember(name, jobLevelID, startDate, teamID, isTeamLeader, isAdmin, strUserID, null, null, strPhoneNumber);

            if (member == null)
            {
                strErrorMessage = "직원 등록에 실패하였습니다.";
                return null;
            }

            DateTime dtNow = DateTime.Now;
            DateTime nextVacationDay;

            float fVacationDay = m_processManager.GetVacationManager().GetVacationDay(startDate, dtNow, out nextVacationDay);
            
            if (m_dataManager.GetCreateManager().CreateHistory(member.ID, dtNow.Year, fVacationDay, 0, 0, new List<int>(), nextVacationDay) == null)
            {
                strErrorMessage = "직원의 휴가이력을 등록할 수 없습니다.\r\n직원정보를 삭제합니다.";

                string str;
                m_dataManager.GetDeleteManager().DeleteCompanyMember(member.ID, out str);
                return null;
            }

            return member;
        }
    }
}
