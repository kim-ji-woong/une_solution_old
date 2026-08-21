using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using TeamEditor.IDAL;
using TeamEditor.Model.Sop.Team;
using Newtonsoft.Json.Linq;

namespace SensorMaker.BLL
{
    using Models;
    using Models.Account;

    public class AccountManager
    {
        private class AES256Cipher
        {
            private static string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

            public static String AES_encrypt(String Input, String key)
            {
                RijndaelManaged aes = new RijndaelManaged();
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = System.Text.Encoding.UTF8.GetBytes(key);
                aes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
                byte[] xBuff = null;
                using (var ms = new System.IO.MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                    {
                        byte[] xXml = System.Text.Encoding.UTF8.GetBytes(Input);
                        cs.Write(xXml, 0, xXml.Length);
                    }

                    xBuff = ms.ToArray();
                }

                String Output = Convert.ToBase64String(xBuff);
                return Output;
            }

            public static byte[] AES_encrypt(byte[] input, string key)
            {
                RijndaelManaged aes = new RijndaelManaged();
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = System.Text.Encoding.UTF8.GetBytes(key);
                aes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
                byte[] xBuff = null;
                using (var ms = new System.IO.MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                    {
                        cs.Write(input, 0, input.Length);
                    }

                    xBuff = ms.ToArray();
                }

                return xBuff;
            }

            public static String AES_decrypt(String Input, String key)
            {
                // FormatException 유발
                if (Input.Length % 4 > 0)
                    return Input;

                byte[] base64Xml = null;

                try
                {
                    base64Xml = Convert.FromBase64String(Input);
                }
                catch (System.FormatException)
                {
                    return Input;
                }

                RijndaelManaged aes = new RijndaelManaged();
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = System.Text.Encoding.UTF8.GetBytes(key);
                aes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                var decrypt = aes.CreateDecryptor();
                byte[] xBuff = null;
                using (var ms = new System.IO.MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
                    {
                        byte[] xXml = base64Xml;
                        cs.Write(xXml, 0, xXml.Length);
                    }

                    xBuff = ms.ToArray();
                }

                String Output = System.Text.Encoding.UTF8.GetString(xBuff);
                return Output;
            }

            public static byte[] AES_decrypt(byte[] input, String key)
            {
                RijndaelManaged aes = new RijndaelManaged();
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = System.Text.Encoding.UTF8.GetBytes(key);
                aes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                var decrypt = aes.CreateDecryptor();
                byte[] xBuff = null;
                using (var ms = new System.IO.MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
                    {
                        cs.Write(input, 0, input.Length);
                    }

                    xBuff = ms.ToArray();
                }

                return xBuff;
            }

            public static string Encrypt(string input)
            {
                return AES_encrypt(input, key);
            }

            public static string Decrypt(string input)
            {
                return AES_decrypt(input, key);
            }
        }

        private const int MaxMemberIDLength = 50;

        private IDataManager m_dataManager = null;
        private ProcessManager m_processManager = null;

        public AccountManager(IDataManager dataManager, ProcessManager processManager)
        {
            m_dataManager = dataManager;
            m_processManager = processManager;
        }

        public RegisterResult SendRegistEmail(string strSystemMail, string strAdminMail, string strSystemCode, string strSiteURL, RegisterData data, string strURL, string strSolutionName)
        {
            string strErrorMessage;
            string strEmail = data.IsValidEmail(out strErrorMessage);

            if (strEmail == null)
                return new RegisterResult(false, strErrorMessage);

            string strPhoneNumber = data.IsValidPhoneNumber(out strErrorMessage);

            if (strPhoneNumber == null)
                return new RegisterResult(false, strErrorMessage);

            ISelect selectManager = m_dataManager.GetSelectManager();

            Dictionary<RegularMember.Fields, object> dicConditions = new Dictionary<RegularMember.Fields, object>();
            dicConditions[RegularMember.Fields.MemberName] = data.Name;
            dicConditions[RegularMember.Fields.Email] = data.Email;
            dicConditions[RegularMember.Fields.PhoneNumber] = data.PhoneNumber;

            List<RegularMember> members = selectManager.SelectRegularMembers(dicConditions, null, out strErrorMessage);

            if (members == null || strErrorMessage != null)
                return new RegisterResult(false, strErrorMessage);

            if (members.Count == 0)
            {
                if (data.RegistNewUser)
                    return ProcessNewUser(data.Name, data.Email, data.PhoneNumber, data.Password, strAdminMail, strSiteURL, strSystemMail, strSystemCode, strSolutionName);
                else
                    return new RegisterResult(false, "등록되지 않은 사용자입니다.");
            }

            if (data.RegistNewUser)
                return new RegisterResult(false, "이미 등록된 사용자입니다.");

            string strResultMessage = "";
            return new RegisterResult(false, strResultMessage);
        }

        private RegisterResult ProcessNewUser(string strUserName, string strEmail, string strPhoneNumber, string strPassword, string strAdminEmail, string strSiteURL, string strSystemMail, string strSystemCode, string strSolutionName)
        {
            string strErrorMessage;

            Dictionary<Regular.Fields, object> dicConditions = new Dictionary<Regular.Fields, object>();
            dicConditions[Regular.Fields.TeamName] = ApplicationUser.SystemTeamName;
            List<Regular> teams = m_dataManager.GetSelectManager().SelectRegulars(dicConditions, out strErrorMessage);

            if (teams == null)
                return new RegisterResult(false, strErrorMessage);

            Regular team = null;

            if (teams.Count == 0)
                team = CreateTeam(ApplicationUser.SystemTeamName, out strErrorMessage);
            else
                team = teams[0];

            if (team == null)
                return new RegisterResult(false, strErrorMessage);

            int nMemberID = m_dataManager.GetSelectManager().GetMaxID(RegularMember.GetTableName(), out strErrorMessage);

            if (nMemberID < 0)
                return new RegisterResult(false, strErrorMessage);

            string strMemberID, strOfficePhoneNumber;
            DivideEncryptedPassword(strPassword + GetCurrentTimeString(), out strMemberID, out strOfficePhoneNumber);

            RegularMember member = new RegularMember();
            member.ID = nMemberID;
            member.MemberName = strUserName;
            member.Email = strEmail;
            member.PhoneNumber = strPhoneNumber;
            member.OfficePhoneNumber = strOfficePhoneNumber;
            member.MemberID = strMemberID;
            member.RegularID = team.ID;

            RegisterResult result = new RegisterResult(true, "");

            if (strEmail == strAdminEmail)
            {
                ApplicationUser.UserType userType = ApplicationUser.UserType.Administrator;
                ApplicationUser.UserStatus status = ApplicationUser.UserStatus.Normal;
                member.StatusID = ApplicationUser.GetStatusID(status, userType);

                if (m_dataManager.GetCreateManager().AddRegularMember(member, out strErrorMessage) == false)
                    return new RegisterResult(false, strErrorMessage);
                else
                {
                    EmailManager.SendPermitEmail(strUserName, strEmail, strSiteURL, strSystemMail, strSystemCode, strSolutionName);
                    KakaoManager.SendMessage(KakaoManager.MessageType.Permit, strEmail, strPhoneNumber, strSiteURL, strSolutionName);

                    result.RegisterAdminUser = true;
                    result.Message = "관리자로 등록되었습니다.";
                }
            }
            else
            {
                RegularMember admin = GetAdminMember(strAdminEmail, out strErrorMessage);

                if (admin == null)
                    return new RegisterResult(false, strErrorMessage);

                ApplicationUser.UserType userType = ApplicationUser.UserType.Normal;
                ApplicationUser.UserStatus status = ApplicationUser.UserStatus.NotConfirmed;
                member.StatusID = ApplicationUser.GetStatusID(status, userType);

                if (m_dataManager.GetCreateManager().AddRegularMember(member, out strErrorMessage) == false)
                    return new RegisterResult(false, strErrorMessage);
                else
                {
                    KakaoManager.SendMessage(KakaoManager.MessageType.Request, strEmail, strPhoneNumber, strSiteURL, strSolutionName);
                    KakaoManager.SendMessage(KakaoManager.MessageType.Request, strEmail, admin.PhoneNumber, strSiteURL, strSolutionName);
                    result.IsNewUser = true;
                    result.Message = "관리자에게 계정 승인이 요청되었습니다.\r\n요청이 처리되면 카카오톡과 메일로 결과가 전달됩니다.";
                }
            }

            return result;
        }

        private string GetCurrentTimeString()
        {
            DateTime dtNow = DateTime.Now;
            return string.Format("{0}{1:00}{2:00}{3:00}{4:00}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute);
        }

        // Password가 MemberID 길이보다 길 경우 OfficePhoneNumber에 나머지를 넘겨주도록 한다.
        private void DivideEncryptedPassword(string strPassword, out string strMemberID, out string strOfficePhoneNumber)
        {
            strMemberID = strOfficePhoneNumber = null;

            if (strPassword.Length <= MaxMemberIDLength)
                strMemberID = strPassword;
            else
            {
                strMemberID = strPassword.Substring(0, MaxMemberIDLength);
                strOfficePhoneNumber = strPassword.Substring(MaxMemberIDLength);
            }
        }

        private string GetPassword(RegularMember member, out DateTime dtCreate)
        {
            dtCreate = new DateTime();
            string strPassword = member.MemberID;

            if (strPassword == null)
                return strPassword;

            if (member.OfficePhoneNumber != null)
                strPassword += member.OfficePhoneNumber;

            // YYYYMMDDHHmm
            const int timeLength = 12;

            if (strPassword.Length < timeLength)
                return strPassword;

            string strTime = strPassword.Substring(strPassword.Length - timeLength);
            StringToDateTime(strTime, ref dtCreate);

            strPassword = strPassword.Substring(0, strPassword.Length - timeLength);

            return strPassword;
        }

        private void StringToDateTime(string strTime, ref DateTime timeStamp)
        {
            if (strTime.Length < 12)
                return;

            int year, month, day, hour, minute;

            if (int.TryParse(strTime.Substring(0, 4), out year) &&
                int.TryParse(strTime.Substring(4, 2), out month) &&
                int.TryParse(strTime.Substring(6, 2), out day) &&
                int.TryParse(strTime.Substring(8, 2), out hour) &&
                int.TryParse(strTime.Substring(10, 2), out minute))
            {
                timeStamp = new DateTime(year, month, day, hour, minute, 0);
            }
        }

        private RegularMember GetAdminMember(string strAdminEmail, out string strErrorMessage)
        {
            Dictionary<RegularMember.Fields, object> dicCondition = new Dictionary<RegularMember.Fields, object>();
            dicCondition[RegularMember.Fields.Email] = strAdminEmail;

            List<RegularMember> members = m_dataManager.GetSelectManager().SelectRegularMembers(dicCondition, null, out strErrorMessage);

            if (members == null)
                return null;

            if (members.Count == 0)
            {
                strErrorMessage = "시스템 관리자가 지정되지 않았습니다.";
                return null;
            }

            return members[0];
        }

        private Regular CreateTeam(string strTeamName, out string strErrorMessage)
        {
            int nID = m_dataManager.GetSelectManager().GetMaxID(Regular.GetTableName(), out strErrorMessage);

            Regular regular = new Regular();
            regular.ID = nID;
            regular.TeamName = strTeamName;

            if (m_dataManager.GetCreateManager().AddRegular(regular, out strErrorMessage))
                return regular;

            return null;
        }

        public LoginResult Login(string strUserID, string strPW, string strTempRootResource, string strRootResource, string strExternalLoginURL)
        {
            string strErrorMessage = null;
            ApplicationUser user = null;

            if (strExternalLoginURL != null && strExternalLoginURL.Length > 0)
            {
                LoginResult result2 = ExternalLogin(strUserID, strPW, strTempRootResource, strRootResource, strExternalLoginURL, out strErrorMessage);

                if (result2.Success == false)
                    return result2;

                user = result2.User;
            }
            else
            {
                Dictionary<RegularMember.Fields, object> dicConditions = new Dictionary<RegularMember.Fields, object>();
                dicConditions[RegularMember.Fields.Email] = strUserID;

                List<RegularMember> members = m_dataManager.GetSelectManager().SelectRegularMembers(dicConditions, null, out strErrorMessage);

                if (members == null || strErrorMessage != null)
                    return new LoginResult(false, strErrorMessage);

                if (members.Count == 0)
                    return new LoginResult(false, string.Format("존재하지 않는 계정이거나 비밀번호가 잘못되었습니다."));

                DateTime dtCreate;
                RegularMember member = members[0];
                string strPassword = GetPassword(member, out dtCreate);

                if (strPW != strPassword)
                    return new LoginResult(false, string.Format("존재하지 않는 계정이거나 비밀번호가 잘못되었습니다."));

                user = ApplicationUser.FromRegularMember(member, dtCreate);
            }

            if (user.GetStatus() == ApplicationUser.UserStatus.NotConfirmed)
                return new LoginResult(false, "아직 승인되지 않은 사용자입니다.");

            string strBackgroundImagePath;
            string strTargetBackgroundImageFolder = ModelFileManager.GetTextureBaseFolder(user.ID, user.Name, strRootResource, out strBackgroundImagePath);

            // 로그인에 성공하면 개별 계정별로 필요한 필수 Resource들을 각 계정별 폴더에 복사해 넣는다.
            CopyToFile(strTargetBackgroundImageFolder, strBackgroundImagePath);

            LoginResult result = new LoginResult(true, "");
            result.User = user;
            result.Options = ModelFileManager.GetGltfOption(user.ID, user.Name, strRootResource);

            if (user.IsAdmin)
            {
                // 계정생성후 승인 대기중인 리스트를 얻어온다.
                result.RequestUsers = GetRequestUsers();
            }

            // 이전 세션에서 작업하던 임시 파일들이 남아있으면 모두 삭제한다.
            ModelFileManager.ClearTempFiles(user.ID, user.Name, strTempRootResource);

            return result;
        }

        public LoginResult AutoLogin(string strBeginCode, string strTempRootResource, string strRootResource, string strExternalLoginURL)
        {
            if (strExternalLoginURL != null && strExternalLoginURL.Length > 0)
            {
                JObject jsonData = new JObject();

                jsonData.Add("beginCode", strBeginCode);

                JObject json = new JObject();
                json.Add("externalAutoLogin", jsonData);

                string strJson = json.ToString();

                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(strJson);
                int len = bytes.Length;

                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(new Uri(strExternalLoginURL));
                request.Method = "POST";
                request.ContentType = "application/json; charset=utf-8";
                request.ContentLength = len + 3;

                string strResult = "";
                string strErrorMessage = null;

                try
                {
                    StreamWriter writer = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.UTF8);
                    writer.Write(strJson);
                    writer.Close();

                    System.Net.HttpWebResponse wRes = (System.Net.HttpWebResponse)request.GetResponse();

                    Stream respPostStream = wRes.GetResponseStream();
                    StreamReader readerPost = new StreamReader(respPostStream, System.Text.Encoding.UTF8);

                    strResult = readerPost.ReadToEnd().Trim();
                    request.Abort();
                    readerPost.Close();
                    respPostStream.Close();

                    string strUserID, strUserName, strTeamName;
                    bool success = GetJsonResult(JObject.Parse(strResult), out strUserID, out strUserName, out strTeamName, out strErrorMessage);

                    LoginResult result = new LoginResult(true, "");

                    if (success == false)
                    {
                        result.Success = success;
                        result.Message = strErrorMessage;
                        return result;
                    }

                    result.User = new ApplicationUser();
                    result.User.ID = 1;
                    result.User.Name = strUserName;
                    result.User.SetUserType(ApplicationUser.UserType.Normal);
                    result.User.CreateTime = DateTime.Now;
                    result.User.SetStatus(ApplicationUser.UserStatus.Normal);

                    return result;
                }
                catch (System.Net.WebException ex)
                {
                    strErrorMessage = ex.Message;
                }

                return new LoginResult(false, strErrorMessage);
            }

            return new LoginResult(false, "자동 로그인을 위한 url이 설정되지 않았습니다.");
        }

        private LoginResult ExternalLogin(string strUserID, string strPW, string strTempRootResource, string strRootResource, string strExternalLoginURL, out string strErrorMessage)
        {
            JObject jsonData = new JObject();

            jsonData.Add("userID", strUserID);
            jsonData.Add("hashCode", strPW);

            JObject json = new JObject();
            json.Add("externalLogin", jsonData);

            string strJson = json.ToString();

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(strJson);
            int len = bytes.Length;

            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(new Uri(strExternalLoginURL));
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            request.ContentLength = len + 3;

            string strResult = "";

            try
            {
                StreamWriter writer = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.UTF8);
                writer.Write(strJson);
                writer.Close();

                System.Net.HttpWebResponse wRes = (System.Net.HttpWebResponse)request.GetResponse();

                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, System.Text.Encoding.UTF8);

                strResult = readerPost.ReadToEnd().Trim();
                request.Abort();
                readerPost.Close();
                respPostStream.Close();
                strErrorMessage = null;

                string strUserName, strTeamName;
                bool success = GetJsonResult(JObject.Parse(strResult), out strUserID, out strUserName, out strTeamName, out strErrorMessage);

                LoginResult result = new LoginResult(true, "");

                result.User = new ApplicationUser();
                result.User.ID = 1;
                result.User.Name = strUserName;
                result.User.SetUserType(ApplicationUser.UserType.Normal);
                result.User.CreateTime = DateTime.Now;
                result.User.SetStatus(ApplicationUser.UserStatus.Normal);

                return result;
            }
            catch (System.Net.WebException ex)
            {
                strErrorMessage = ex.Message;
            }

            return new LoginResult(false, strErrorMessage);
        }

        private bool GetJsonResult(JObject json, out string strUserID, out string strUserName, out string strTeamName, out string strErrorMessage)
        {
            strUserID = strUserName = strTeamName = null;
            strErrorMessage = null;

            if (json == null)
                return false;

            JToken tokenName = json.GetValue("name");
            JToken tokenUserID = json.GetValue("userID");
            JToken tokenTeamName = json.GetValue("teamName");
            JToken tokenMessage = json.GetValue("message");
            JToken tokenSuccess = json.GetValue("success");

            if (tokenMessage != null)
                strErrorMessage = tokenMessage.Value<string>();

            if (tokenName == null || tokenUserID == null)
                return false;

            strUserID = tokenUserID.Value<string>();
            strUserName = tokenName.Value<string>();

            if (tokenTeamName != null)
                strTeamName = tokenTeamName.Value<string>();

            if (tokenSuccess != null)
            {
                string strSuccess = tokenSuccess.Value<string>().ToLower();

                if (strSuccess == "true")
                    return true;
            }

            return false;
        }

        private void CopyToFile(string strTargetFolder, string strImagePath)
        {
            int nIndex = strImagePath.LastIndexOf('\\');
            string strFileName = nIndex < 0 ? strImagePath : strImagePath.Substring(nIndex + 1);

            string strTargetPath = strTargetFolder.EndsWith("\\") ? strTargetFolder + strFileName : strTargetFolder + "\\" + strFileName;

            try
            {
                if (Directory.Exists(strTargetFolder) == false)
                    Directory.CreateDirectory(strTargetFolder);

                if (File.Exists(strTargetPath) == false)
                    File.Copy(strImagePath, strTargetPath, false);
            }
            catch (Exception e)
            {
                // 파일복사 도중에 문제가 생겨도 신경쓰지 않는다.
                System.Diagnostics.Trace.WriteLine("CopyToFile 실패, " + e.Message);
            }
        }

        private List<ApplicationUser> GetRequestUsers()
        {
            bool isNullable;
            string strCondition = string.Format("({0} / 100) > 0", RegularMember.GetFieldName(RegularMember.Fields.StatusID, out isNullable));

            string strErrorMessage;
            List<RegularMember> members = m_dataManager.GetSelectManager().SelectRegularMembers(null, strCondition, out strErrorMessage);

            if (members == null || members.Count == 0)
                return null;

            List<ApplicationUser> users = new List<ApplicationUser>();

            foreach (RegularMember member in members)
            {
                DateTime dtCreate;
                GetPassword(member, out dtCreate);
                users.Add(ApplicationUser.FromRegularMember(member, dtCreate));
            }

            return users;
        }

        public LoginResult UpdateMemberRegist(RequestRegist data, string strSiteURL, string strSolutionName, string strSystemMail, string strSystemCode)
        {
            if (data.Users == null)
                return new LoginResult(false, "처리할 데이터를 찾을수 없습니다.");

            string strErrorMessage;

            foreach (RequestRegist.UserInfo user in data.Users)
            {
                ApplicationUser.UserType userType;

                if (user.IsAdmin)
                    userType = ApplicationUser.UserType.Administrator;
                else if (user.IsDeveloper)
                    userType = ApplicationUser.UserType.Developer;
                else if (user.IsNormalUser)
                    userType = ApplicationUser.UserType.Normal;
                else
                    continue;

                if (data.Permit)
                {
                    if (PermitMember(user.ID, userType, strSiteURL, strSolutionName, strSystemMail, strSystemCode, out strErrorMessage) == false)
                        return new LoginResult(false, strErrorMessage);
                }
                else
                {
                    if (DenyMember(user.ID, data.DenyDescription, userType, strSiteURL, strSolutionName, strSystemMail, strSystemCode, out strErrorMessage) == false)
                        return new LoginResult(false, strErrorMessage);
                }
            }

            LoginResult result = new LoginResult(true, "");
            result.RequestUsers = GetRequestUsers();
            return result;
        }

        // 계정생성 승인
        private bool PermitMember(int nUserID, ApplicationUser.UserType userType, string strSiteURL, string strSolutionName, string strSystemMail, string strSystemCode, out string strErrorMessage)
        {
            RegularMember member = m_dataManager.GetSelectManager().SelectRegularMember(nUserID, out strErrorMessage);

            if (member == null)
            {
                if (strErrorMessage != null)
                    return false;
                else
                    return true;
            }

            member.StatusID = ApplicationUser.GetStatusID(ApplicationUser.UserStatus.Normal, userType);

            if (m_dataManager.GetUpdateManager().UpdateRegularMember(member, out strErrorMessage) == false)
                return false;

            KakaoManager.SendMessage(KakaoManager.MessageType.Permit, member.Email, member.PhoneNumber, strSiteURL, strSolutionName);
            EmailManager.SendPermitEmail(member.MemberName, member.Email, strSiteURL, strSystemMail, strSystemCode, strSolutionName);
            return true;
        }

        // 계정생성 거절
        private bool DenyMember(int nUserID, string strDenyDescription, ApplicationUser.UserType userType, string strSiteURL, string strSolutionName, string strSystemMail, string strSystemCode, out string strErrorMessage)
        {
            RegularMember member = m_dataManager.GetSelectManager().SelectRegularMember(nUserID, out strErrorMessage);

            if (member == null)
            {
                if (strErrorMessage != null)
                    return false;
                else
                    return true;
            }

            if (m_dataManager.GetDeleteManager().DeleteRegularMember(member.ID, out strErrorMessage) == false)
                return false;

            KakaoManager.SendMessage(KakaoManager.MessageType.Deny, member.Email, member.PhoneNumber, strSiteURL, strSolutionName, strDenyDescription);
            EmailManager.SendDenyEmail(member.MemberName, member.Email, strDenyDescription, strSiteURL, strSystemMail, strSystemCode, strSolutionName);
            return true;
        }
    }
}
