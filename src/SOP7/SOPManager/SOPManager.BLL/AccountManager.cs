using System.Collections.Generic;
using dnsDBUtil;
using Newtonsoft.Json.Linq;
using System.IO;

namespace SOPManager.BLL
{
    using dnsEmail;
    using dnsSMS;
    using SOPManager.BLL.Models;
    using SOPManager.BLL.Models.Request;
    using SOPManager.BLL.Models.Response;
    using SOPManager.IDAL;
    using SOPManager.Model.Sop.Account;
    using System;
    using System.Text.RegularExpressions;
    using TeamEditor.Model.Sop.Team;

    public class AccountManager
    {
        private static string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

        private IDataManager m_dataManager = null;
        private TeamEditor.IDAL.IDataManager m_teamDataManager = null;
        private Common.IDAL.IDataManager m_commonDataManager = null;
        private SDMS.IDAL.IDataManager m_sdmsDataManager = null;
        private ProcessManager m_processManager = null;

        public AccountManager(IDataManager manager, TeamEditor.IDAL.IDataManager teamDataManager, Common.IDAL.IDataManager commonDataManager, SDMS.IDAL.IDataManager sdmsDataManager, ProcessManager processManager)
        {
            m_dataManager = manager;
            m_teamDataManager = teamDataManager;
            m_processManager = processManager;
            m_commonDataManager = commonDataManager;
            m_sdmsDataManager = sdmsDataManager;
        }

        public Models.LoginResult Login(string strUserID, string strPW, string strSessionKey, bool isFullVersion, string strExternalLoginURL, bool autoLogin)
        {
            LoginResult result = null;
            User user = null;
            Level level = null;
            string strErrorMessage = null;

            if (strExternalLoginURL != null && strExternalLoginURL.Length > 0)
            {
                result = ExternalLogin(strUserID, strPW, strExternalLoginURL, strSessionKey, autoLogin, isFullVersion, out user, out level);

                if (result.Success == false)
                    return result;
                else if (result.User == null)
                {
                    result.Success = false;
                    result.Message = "해당 ID를 가진 유저 정보를 찾을 수 없습니다.";
                }

                /*
                int nUserID = result.User.ID;

                if (UpdateSession(nUserID, strSessionKey, strAutoLogin, out strErrorMessage) == false)
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }

                if (Update3DVersion(nUserID, strVersion, out strErrorMessage) == false)
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }

                result.User.SessionKey = strSessionKey;
                */

                return result;
            }
            else
            {
                result = new LoginResult();

                // ID 값으로 유저를 검색
                Dictionary<User.Fields, object> dicConditions = new Dictionary<User.Fields, object>();
                dicConditions[Model.Sop.Account.User.Fields.UserID] = strUserID;

                List<User> users = m_dataManager.GetSelectManager().SelectUsers(dicConditions, out strErrorMessage);
                if (users == null)
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                } 
                else if (users.Count == 0)
                {
                    result.Success = false;
                    result.Message = "해당 ID를 가진 유저 정보를 찾을 수 없습니다.";
                    return result;
                }

                user = users[0];
                level = m_dataManager.GetSelectManager().SelectLevel(user.UserLevel, out strErrorMessage);

                if (level == null)
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }

                if (user.Password != strPW)
                {
                    strErrorMessage = "비밀번호가 일치하지 않습니다.";

                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (UpdateSession(user.ID, strSessionKey, autoLogin, out strErrorMessage) == false)
            {
                result.Success = false;
                result.Message = strErrorMessage;
                return result;
            }

            if (Update3DVersion(user.ID, isFullVersion.ToString().ToLower(), out strErrorMessage) == false)
            {
                result.Success = false;
                result.Message = strErrorMessage;
                return result;
            }

            result.User = Models.ApplicationUser.MakeUser(user, level, strSessionKey);
            result.Success = true;
            return result;
        }

        public Models.LoginResult AutoLogin(string strBeginCode, string strExternalLoginURL, string strKey)
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

                    return GetExternalLoginResult(strResult, strKey, true);
                }
                catch (System.Net.WebException ex)
                {
                    strErrorMessage = ex.Message;
                }

                return new LoginResult(false, strErrorMessage);
            }

            return new LoginResult(false, "자동 로그인을 위한 url이 설정되지 않았습니다.");
        }

        private LoginResult ExternalLogin(string strUserID, string strPW, string strExternalLoginURL, string strSessionKey, bool autoLogin, bool isFullVersion, out User user, out Level level)
        {
            user = null;
            level = null;

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
            string strErrorMessage = null;
            LoginResult result = new LoginResult();

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

                return GetExternalLoginResult(strResult, strSessionKey, autoLogin, isFullVersion);
                /*string strUserName, strTeamName;
                bool success = GetJsonResult(JObject.Parse(strResult), out strUserID, out strUserName, out strTeamName, out strErrorMessage);

                List<Level> levels = m_dataManager.GetSelectManager().SelectLevels(null, out strErrorMessage);

                if (levels == null)
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
                else if (levels.Count == 0)
                {
                    result.Success = false;
                    result.Message = "사용자 계정이 존재하지 않습니다.";
                    return result;
                }

                level = levels[0];

                user = new User();
                user.ID = 1;
                user.UserLevel = level.ID;
                user.UserID = strUserID;
                user.NickName = strUserName;

                result.Success = true;
                result.Message = "";
                return result;*/
            }
            catch (System.Net.WebException ex)
            {
                strErrorMessage = ex.Message;
            }

            result.Success = false;
            result.Message = strErrorMessage;
            return result;
        }

        private LoginResult GetExternalLoginResult(string strResult, string strSessionKey, bool isFullVersion, bool autoLogin = false)
        {
            string strErrorMessage;
            string strUserID, strUserName, strTeamName;
            bool success = GetJsonResult(JObject.Parse(strResult), out strUserID, out strUserName, out strTeamName, out strErrorMessage);

            if (success == false)
                return new LoginResult(false, strErrorMessage);

            LoginResult result = new LoginResult();

            List<Level> levels = m_dataManager.GetSelectManager().SelectLevels(null, out strErrorMessage);

            if (levels == null)
            {
                result.Success = false;
                result.Message = strErrorMessage;
                return result;
            }
            else if (levels.Count == 0)
            {
                result.Success = false;
                result.Message = "Account Level이 존재하지 않습니다.";
                return result;
            }

            Level level = levels[0];

            // ID 조회 
            User user = null;

            Dictionary<User.Fields, object> dicConditions = new Dictionary<User.Fields, object>();
            dicConditions[User.Fields.UserID] = strUserID;

            List<User> users = m_dataManager.GetSelectManager().SelectUsers(dicConditions, out strErrorMessage);
            if (users == null)
            {
                result.Success = false;
                result.Message = strErrorMessage;
                return result;
            }
            else if (users.Count == 0)
            {   // 없으면 새로 생성
                user = m_dataManager.GetCreateManager().CreateUser(null, level.ID, strUserID, "", strUserName, m_dataManager.SiteID);
                
                if (user == null)
                {
                    result.Success = false;
                    result.Message = "External 계정 생성 실패";
                    return result;
                }
            } 
            else if (users.Count > 0)
            {
                user = users[0];

                level = m_dataManager.GetSelectManager().SelectLevel(user.UserLevel, out strErrorMessage);

                if (level == null)
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }
            }

            if (UpdateSession(user.ID, strSessionKey, autoLogin, out strErrorMessage) == false)
            {
                result.Success = false;
                result.Message = strErrorMessage;
                return result;
            }

            if (Update3DVersion(user.ID, isFullVersion.ToString().ToLower(), out strErrorMessage) == false)
            {
                result.Success = false;
                result.Message = strErrorMessage;
                return result;
            }

            ApplicationUser loginUser = new ApplicationUser();
            loginUser.ID = user.ID;
            loginUser.Level = level.LevelName;
            loginUser.LevelID = level.ID;
            loginUser.NickName = strUserName;
            loginUser.UserID = strUserID;
            loginUser.SessionKey = strSessionKey;

            result.Success = true;
            result.Message = "";
            result.User = loginUser;
            return result;
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

        private bool UpdateSession(int nUserID, string strSessionKey, bool autoLogin, out string strErrorMessage)
        {
            strErrorMessage = "";

            // 해당 유저 세션 유무 확인
            Dictionary<Session.Fields, object> dicConditions_sessions = new Dictionary<Session.Fields, object>();
            dicConditions_sessions[Session.Fields.AccountUserID] = nUserID;

            List<Session> sessions = m_dataManager.GetSelectManager().SelectSessions(dicConditions_sessions, out strErrorMessage);
            if (sessions == null)
            {
                return false;
            }

            // 있으면 삭제 후 생성, 없으면 생성
            if (sessions.Count > 0)
            {
                string strCondition = "AccountUserID = " + nUserID;
                if (!m_dataManager.GetDeleteManager().DeleteSession(strCondition))
                {
                    strErrorMessage = m_dataManager.GetDeleteManager().GetErrorMessage();
                    return false;
                }
            }

            DateTime dtNow = DateTime.Now;

            Session session = m_dataManager.GetCreateManager().CreateSession(nUserID, strSessionKey, dtNow, dtNow, autoLogin);
            if (session == null)
            {
                strErrorMessage = m_dataManager.GetCreateManager().GetErrorMessage();
                return false;
            }

            return true;
        }

        private bool Update3DVersion(int nUserID, string strVersion, out string strErrorMessage)
        {
            strErrorMessage = "";

            // 해당 유저 3D 버전 확인
            Dictionary<Option.Fields, object> dicConditions = new Dictionary<Option.Fields, object>();
            dicConditions[Option.Fields.UserID] = nUserID;
            dicConditions[Option.Fields.Category] = "SDMS";
            dicConditions[Option.Fields.SubCategory] = "3DHighVer";

            List<Option> options = m_dataManager.GetSelectManager().SelectOptions(dicConditions, out strErrorMessage);
            if (options == null)
            {
                return false;
            }

            // 있으면 버전 확인 후 업데이트, 없으면 생성
            if (options.Count > 2)
            {   // 2개 이상이면 삭제 후 생성
                string strCondition = "UserID = " + nUserID;

                // 삭제
                if (m_dataManager.GetDeleteManager().DeleteOption(strCondition) == false)
                {
                    strErrorMessage = m_dataManager.GetDeleteManager().GetErrorMessage();
                    return false;
                }

                // 생성
                Option option = m_dataManager.GetCreateManager().CreateOption(nUserID, "SDMS", "3DHighVer", strVersion, "", "", "");
                if (option == null)
                {
                    strErrorMessage = m_dataManager.GetCreateManager().GetErrorMessage();
                    return false;
                }
            } 
            else if (options.Count == 1)
            {   // 확인 후 다르면 업데이트
                Option option = options[0];

                if (option.PropertyValue1 != strVersion)
                {
                    option.PropertyValue1 = strVersion;
                    
                    if (m_dataManager.GetUpdateManager().UpdateOption(option) == false)
                    {
                        strErrorMessage = m_dataManager.GetUpdateManager().GetErrorMessage();
                        return false;
                    }
                }
            }
            else
            {   // 없으면 생성
                Option option = m_dataManager.GetCreateManager().CreateOption(nUserID, "SDMS", "3DHighVer", strVersion, "", "", "");
                if (option == null)
                {
                    strErrorMessage = m_dataManager.GetCreateManager().GetErrorMessage();
                    return false;
                }
            }

            return true;
        }

        public ResponseAccountLevels GetAccountLevels()
        {
            ResponseAccountLevels response = new ResponseAccountLevels();
            string strErrorMessage;

            Dictionary<Level.Fields, object> dicCondition = new Dictionary<Level.Fields, object>();

            List<Level> listLevels = m_dataManager.GetSelectManager().SelectLevels(dicCondition, out strErrorMessage);
            if (listLevels != null && listLevels.Count > 0)
            {
                response.AccountLevels = listLevels;

                response.Success = true;
                response.Message = strErrorMessage;
            }
            else
            {
                response.Success = false;
                response.Message = "Account Level 조회를 할 수 없습니다.";
            }

            return response;
        }

        public MessageResult RemoveAccountUsers(List<AccountUser> accountUsers)
        {
            MessageResult result = new MessageResult();
            string strErrorMessage = "";

            foreach (AccountUser accountUser in accountUsers)
            {
                // 해당 계정의 옵션 삭제
                Dictionary<Option.Fields, object> dicConditions_option = new Dictionary<Option.Fields, object>();
                dicConditions_option[Option.Fields.UserID] = accountUser.ID;

                List<Option> options = m_dataManager.GetSelectManager().SelectOptions(dicConditions_option, out strErrorMessage);
                if (options == null)
                {
                    result.Message = strErrorMessage;
                    result.Success = false;
                    return result;
                }

                foreach (Option option in options)
                {
                    if (!m_dataManager.GetDeleteManager().DeleteOption(option.ID))
                    {
                        result.Message = "RemoveAccountUsers 에러 (DeleteOption 실패)";
                        result.Success = false;
                        return result;
                    }
                }

                // 해당 계정에 세션 삭제
                Dictionary<Session.Fields, object> dicConditions_session = new Dictionary<Session.Fields, object>();
                dicConditions_session[Session.Fields.AccountUserID] = accountUser.ID;

                List<Session> sessions = m_dataManager.GetSelectManager().SelectSessions(dicConditions_session, out strErrorMessage);
                if (sessions == null)
                {
                    result.Message = strErrorMessage;
                    result.Success = false;
                    return result;
                }

                foreach (Session session in sessions)
                {
                    if (!m_dataManager.GetDeleteManager().DeleteSession(session.ID))
                    {
                        result.Message = "RemoveAccountUsers 에러 (DeleteSession 실패)";
                        result.Success = false;
                        return result;
                    }
                }
                
                if (m_dataManager.GetDeleteManager().DeleteUser(accountUser.AccountID) == false)
                {
                    result.Success = false;
                    result.Message = "DeleteUser 실패";
                    return result;
                }
            }

            result.Success = true;
            return result;
        }

        public MessageResult UpdateAccountUsers(RequestAccountUser requestData)
        {
            List<AccountUser> accountUsers = requestData.AccountUsers;
            int accessedUserID = requestData.AccessedUserID;
            MessageResult result = new MessageResult();

            Common.BLL.ProcessManager commonProcessManager =
                new Common.BLL.ProcessManager(m_processManager.CommonDataManager, m_processManager.SopDataManager, m_processManager.TeamDataManager, m_processManager.SDMSDataManager);

            Common.BLL.SaveManager commonSaveManager = commonProcessManager.GetSaveManager();

            foreach (AccountUser accountUser in accountUsers)
            {
                if (accountUser.AccountID == -1)
                {   // 계정이 없는 경우
                    string strUserID = "";
                    string strNickName = "";
                    string strPassword = "";
                    int nMemberID = -1;
                    int nUserLevel = -1;
                    int nSiteID = m_dataManager.SiteID;

                    // 아이디는 사번 또는 이름
                    if (accountUser.MemberID != null && accountUser.MemberID != "")
                        strUserID = accountUser.MemberID;
                    else if (accountUser.MemberName != null && accountUser.MemberName != "")
                        strUserID = accountUser.MemberName;
                    else
                    {
                        result.Success = false;
                        result.Message = "해당 인원의 이름 또는 사번을 입력해주세요.";
                        return result;
                    }

                    // 비밀번호는 휴대폰 뒷자리 7 or 8자리 >> 없을 경우 1234 부여
                    strPassword = accountUser.UserID;

                    // 닉네임은 이름
                    strNickName = accountUser.MemberName;

                    nMemberID = accountUser.ID;
                    nUserLevel = accountUser.AccountLevel.ID;

                    // 계정 생성
                    if (m_dataManager.GetCreateManager().CreateUser(nMemberID, nUserLevel, strUserID, strPassword, strNickName, nSiteID) == null)
                    {
                        result.Success = false;
                        result.Message = "CreateUser 실패";
                        return result;
                    }

                    commonSaveManager.SaveUserHistory_ModifyUserAuth(accessedUserID, accountUser.ID, -1);
                }
                else
                {   // 계정이 존재하는 경우
                    User user = new User();
                    user.ID = accountUser.AccountID;
                    user.MemberID = accountUser.ID;
                    user.NickName = accountUser.NickName;
                    user.Password = accountUser.Password;
                    user.SiteID = m_dataManager.SiteID;
                    user.UserID = accountUser.UserID;
                    user.UserLevel = accountUser.AccountLevel.ID;

                    string strErrorMessage = null;
                    User orgUser = m_dataManager.GetSelectManager().SelectUser(user.ID, out strErrorMessage);
                    if (orgUser == null)
                        continue;

                    if (m_dataManager.GetUpdateManager().UpdateUser(user) == false)
                    {
                        result.Success = false;
                        result.Message = "UpdateUser 실패";

                        return result;
                    }

                    commonSaveManager.SaveUserHistory_ModifyUserAuth(accessedUserID, accountUser.ID, orgUser.UserLevel);
                }
            }

            result.Success = true;
            return result;
        }

        public MessageResult ReRegisterAccountUsers(List<AccountUser> accountUsers)
        {
            MessageResult result = new MessageResult();

            foreach (AccountUser accountUser in accountUsers)
            {
                int nMemberID = accountUser.ID;
                string strNickName = accountUser.NickName;
                string strPassword = accountUser.Password;
                int nSiteID = m_dataManager.SiteID;
                string strUserID = accountUser.UserID;
                int nUserLevel = accountUser.AccountLevel.ID;

                // 계정 생성
                if (m_dataManager.GetCreateManager().CreateUser(nMemberID, nUserLevel, strUserID, strPassword, strNickName, nSiteID) == null)
                {
                    result.Success = false;
                    result.Message = "CreateUser 실패";
                    return result;
                }
            }

            result.Success = true;
            return result;
        }

        public MessageResult ChangePassword(string strName, string strData, string strPW, string strPwHash, int nMode)
        {
            MessageResult result = new MessageResult();

            // 해당 계정을 조회 및 멤버 조회
            string strErrorMessage = "";
            string strCondition = "";

            if (nMode == 0)
            {   // Email
                strCondition = "MemberName = '" + strName + "' AND Email = '" + strData + "'";
            }
            else if (nMode == 1)
            {   // SMS
                string strPhoneNumber = EncryptString(strData);
                strCondition = "MemberName = '" + strName + "' AND PhoneNumber = '" + strPhoneNumber + "'";
            }
            else
            {
                result.Success = false;
                result.Message = "데이터가 잘못 전달되었습니다.";
                return result;
            }

            List<RegularMember> members = m_teamDataManager.GetSelectManager().SelectRegularMembers(strCondition, out strErrorMessage);
            if (members == null)
            {
                result.Success = false;
                result.Message = strErrorMessage;
                return result;
            }
            else if (members.Count == 0)
            {
                result.Success = false;
                result.Message = "해당 계정이 없습니다.";
                return result;
            } 

            RegularMember member = members[0];

            Dictionary<User.Fields, object> dicConditions = new Dictionary<User.Fields, object>();
            dicConditions[User.Fields.MemberID] = member.ID;
            List<User> users = m_dataManager.GetSelectManager().SelectUsers(dicConditions, out strErrorMessage);
            if (users == null)
            {
                result.Success = false;
                result.Message = strErrorMessage;
                return result;
            }
            else if (users.Count == 0)
            {
                result.Success = false;
                result.Message = "해당 계정이 없습니다.";
                return result;
            }

            User user = users[0];
            string strResultMsg = "";

            if (nMode == 0)
            {   
                // 이메일 여부 확인
                if (member.Email == "" || member.Email == null ||
                !Regex.IsMatch(member.Email, @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?"))
                {
                    result.Success = false;
                    result.Message = "해당 계정에 대한 이메일 정보가 없거나 잘못 되었습니다.";
                    return result;
                }

                // 임시 비밀번호 업데이트
                user.Password = strPwHash;
                if (m_dataManager.GetUpdateManager().UpdateUser(user) == false)
                {
                    result.Success = false;
                    result.Message = "임시 비밀번호 업데이트 실패.";
                    return result;
                }

                // 이메일 발송
                string strEmailTitle = "", strMessage = "", strSubject = "";

                strSubject = "임시 비밀번호 입니다.";

                strMessage = "임시 비밀번호 입니다.\r\n";
                strMessage += "ID는 " + user.UserID + " 이며,\r\n";
                strMessage += "비밀번호는 " + strPW + " 입니다.\r\n";
                strMessage += "로그인하여 비밀번호 변경 부탁드리겠습니다.\r\n\r\n";
                strMessage = string.Format("안녕하세요. {0}님\r\n\r\n", member.MemberName) + strMessage;

                strEmailTitle = "비밀번호 변경안내";

                if (m_dataManager.SiteID == 12)
                {   // 녹십자 카카오웍스 방식
                    IMessageClient client = MessageClientFactory.CreateMessageClient(m_commonDataManager, m_sdmsDataManager);

                    List<string> strEmails = new List<string>();
                    strEmails.Add(member.Email);

                    MessageContent content = new MessageContent();
                    content.Caller = "";
                    content.EMails.AddRange(strEmails);
                    content.Message = strMessage;

                    if (client.SendSMS(content) == true)
                    {
                        strResultMsg = "카카오웍스가 전송되었습니다. 확인부탁드립니다.";
                    }
                    else
                    {
                        result.Success = false;
                        result.Message = "카카오웍스 전송이 실패하였습니다. (카카오웍스 실패) 관리자에게 문의바랍니다.";
                        return result;
                    }
                } 
                else
                {   // 기존 이메일 전송 방식
                    IEmailClient clientMail = EmailClientFactory.CreateMailClient();

                    if (clientMail != null)
                    {
                        Dictionary<string, string> dicMail = new Dictionary<string, string>();
                        dicMail[member.Email] = member.Email;

                        EmailContent contents = new EmailContent();
                        contents.EmailList.AddRange(dicMail.Values);
                        contents.Message = strMessage;

                        contents.Title = strEmailTitle;
                        contents.Subject = strSubject;
                        contents.TimeStamp = System.DateTime.Now;

                        // 수신자번호 가운데 빈문자열이 있으면 없앤다.
                        int nIndex = contents.EmailList.IndexOf("");

                        if (nIndex >= 0)
                            contents.EmailList.RemoveAt(nIndex);

                        if (clientMail.SendEmail(contents, ref strResultMsg) == false)
                        {
                            result.Success = false;
                            result.Message = "관리자에게 문의바람. " + strResultMsg;
                            return result;
                        }
                    }

                }

                
            }
            else if (nMode == 1)
            {
                // 핸드폰 여부 확인
                if (member.PhoneNumber == "" || member.PhoneNumber == null)
                {
                    result.Success = false;
                    result.Message = "해당 계정에 대한 핸드폰 정보가 없거나 잘못 되었습니다.";
                    return result;
                }

                // 임시 비밀번호 업데이트
                user.Password = strPwHash;
                if (m_dataManager.GetUpdateManager().UpdateUser(user) == false)
                {
                    result.Success = false;
                    result.Message = "임시 비밀번호 업데이트 실패.";
                    return result;
                }

                IMessageClient client = MessageClientFactory.CreateMessageClient(m_commonDataManager, m_sdmsDataManager);
                if (client != null)
                {
                    string strMessage = "";
                    strMessage = "임시 비밀번호 입니다.\r\n";
                    strMessage += "ID는 " + user.UserID + " 이며,\r\n";
                    strMessage += "비밀번호는 " + strPW + " 입니다.\r\n";
                    strMessage += "로그인하여 비밀번호 변경 부탁드리겠습니다.\r\n\r\n";

                    string strPhoneNumber = DecryptString(member.PhoneNumber);

                    List<string> strPhoneNumbers = new List<string>();
                    strPhoneNumbers.Add(strPhoneNumber);

                    MessageContent content = new MessageContent();
                    content.Caller = "";
                    content.PhoneNumbers.AddRange(strPhoneNumbers);
                    content.Message = strMessage;

                    if (client.SendSMS(content) == true)
                    {
                        strResultMsg = "SMS가 전송되었습니다. 확인부탁드립니다.";
                    } 
                    else
                    {
                        result.Success = false;
                        result.Message = "SMS 전송이 실패하였습니다. (SendSMS 실패) 관리자에게 문의바랍니다.";
                        return result;
                    }
                }
            }

            result.Success = true;
            result.Message = strResultMsg;
            return result;
        }

        public bool CheckParamsCode(string strCode, out int nID, out string strUserName, out string strUserID, out string strResultMessage)
        {
            strUserName = strUserID = strResultMessage = "";
            nID = -1;

            MessageResult result = new MessageResult();

            try
            {
                string strData = DecryptString(strCode);
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

                long time;

                if (int.TryParse(strID, out nID) == false ||
                    long.TryParse(strTime, out time) == false)
                {
                    strResultMessage = "유효하지 않은 Code입니다.";
                    return false;
                }

                System.DateTime timeStamp = System.DateTime.FromBinary(time);

                if (strCheckSum != (timeStamp.Millisecond + nID).ToString())
                {
                    strResultMessage = "유효하지 않은 Code입니다.";
                    return false;
                }

                User member = m_dataManager.GetSelectManager().SelectUser(nID, out strResultMessage);

                if (member == null || strResultMessage != null)
                    return false;

                if (member.UserID != strUserID ||
                    member.PasswordCode != strTime)
                {
                    strResultMessage = "유효하지 않은 Code입니다.";
                    return false;
                }

                strUserName = member.NickName;
                strResultMessage = "";
                return true;
            }
            catch (Exception e)
            {
                strResultMessage = e.Message;
            }

            return false;
        }

        public MessageResult SetPassword(int nID, string strPW, string strNewPW)
        {
            MessageResult result = new MessageResult();

            string strErrorMessage = null;
            User user = m_dataManager.GetSelectManager().SelectUser(nID, out strErrorMessage);
            if (user == null)
            {
                result.Success = false;
                result.Message = strErrorMessage;
                return result;
            } 
            else if (user.Password != strPW)
            {
                result.Success = false;
                result.Message = "기존 비밀번호가 맞지 않습니다. 확인바랍니다.";
                return result;
            }

            user.Password = strNewPW;
            user.PasswordCode = null;

            if (m_dataManager.GetUpdateManager().UpdateUser(user))
            {
                result.Success = true;
                return result;
            }
            else
            {
                result.Success = false;
                strErrorMessage = "비밀번호 업데이트를 실패하였습니다.";
                return result;
            }
        }

        public ResponseAccountUsers GetAccountUsers()
        {
            ResponseAccountUsers response = new ResponseAccountUsers();
            string strErrorMessage;

            // 팀 불러오기
            List<Regular> regulars = m_teamDataManager.GetSelectManager().SelectRegulars(out strErrorMessage);
            if (regulars == null)
            {
                response.Success = false;
                response.Message = strErrorMessage;
                return response;
            }

            // JobLevel 불러오기
            string strCondition = " PropertyName = 'JobLevel'";
            List<Options> options = m_teamDataManager.GetSelectManager().SelectOptions(strCondition, out strErrorMessage);
            if (options == null)
            {
                response.Success = false;
                response.Message = strErrorMessage;
                return response;
            }

            Dictionary<int, JobLevel> dicJobLevel = new Dictionary<int, JobLevel>();
            foreach(Options option in options)
            {
                JobLevel level = new JobLevel();
                level.ID = option.PropertyID;
                level.Name = option.PropertyValue;

                dicJobLevel[option.PropertyID] = level;
            }

            // JobPosition 불러오기
            strCondition = " PropertyName = 'JobPosition'";
            options = m_teamDataManager.GetSelectManager().SelectOptions(strCondition, out strErrorMessage);
            if (options == null)
            {
                response.Success = false;
                response.Message = strErrorMessage;
                return response;
            }

            Dictionary<int, JobPosition> dicJobPosition = new Dictionary<int, JobPosition>();
            foreach (Options option in options)
            {
                JobPosition position = new JobPosition();
                position.ID = option.PropertyID;
                position.Name = option.PropertyValue;

                dicJobPosition[option.PropertyID] = position;
            }

            // 계정 정보 불러오기
            Dictionary<User.Fields, object> dicConditions = new Dictionary<User.Fields, object>();
            List<User> users = m_dataManager.GetSelectManager().SelectUsers(dicConditions, out strErrorMessage);
            if (users == null)
            {
                response.Success = false;
                response.Message = strErrorMessage;
                return response;
            }

            // 계정 권한 불러오기
            Dictionary<Level.Fields, object> dicLevelConditions = new Dictionary<Level.Fields, object>();
            List<Level> levels = m_dataManager.GetSelectManager().SelectLevels(dicLevelConditions, out strErrorMessage);
            if (levels == null)
            {
                response.Success = false;
                response.Message = strErrorMessage;
                return response;
            }

            // 정규 멤버 불러오기
            strCondition = "MemberID is not null AND MemberID != '' AND Email != '' AND Email is not null";

            List<RegularMember> regularMembers = m_teamDataManager.GetSelectManager().SelectRegularMembers(strCondition, out strErrorMessage);
            if (regularMembers == null)
            {
                response.Success = false;
                response.Message = strErrorMessage;
                return response;
            }


            List<AccountUser> accountUsers = new List<AccountUser>();

            foreach (RegularMember regularMember in regularMembers)
            {
                AccountUser accountUser = new AccountUser();
                accountUser.ID = regularMember.ID;

                foreach (Regular team in regulars)
                {
                    if (team.ID == regularMember.RegularID)
                    {
                        accountUser.Regular = team;
                        break;
                    }
                }

                accountUser.MemberID = regularMember.MemberID;
                accountUser.MemberName = regularMember.MemberName;

                if (regularMember.OfficePhoneNumber != null)
                    accountUser.OfficePhoneNumber = regularMember.OfficePhoneNumber;

                if (regularMember.PhoneNumber != null)
                    accountUser.PhoneNumber = DecryptString(regularMember.PhoneNumber);

                if (regularMember.JobLevelID != null && dicJobLevel.ContainsKey((int)regularMember.JobLevelID))
                {
                    accountUser.JobLevel = dicJobLevel[(int)regularMember.JobLevelID];
                }

                if (regularMember.JobPositionID != null && dicJobPosition.ContainsKey((int)regularMember.JobPositionID))
                {
                    accountUser.JobPosition = dicJobPosition[(int)regularMember.JobPositionID];
                }

                accountUser.Email = regularMember.Email;

                foreach (User user in users)
                {
                    if (user.MemberID != null && regularMember.ID == user.MemberID)
                    {
                        accountUser.AccountID = user.ID;
                        
                        if (user.UserLevel != -1)
                        {
                            foreach (Level level in levels)
                            {
                                if (user.UserLevel == level.ID)
                                {
                                    accountUser.AccountLevel = level;
                                    break;
                                }
                            }
                        }

                        accountUser.UserID = user.UserID;
                        accountUser.NickName = user.NickName;
                        accountUser.Password = user.Password;
                        break;
                    }
                }

                accountUsers.Add(accountUser);
            }

            response.Success = true;
            response.AccountUsers = accountUsers;

            return response;
        }

        public bool CheckLoginSession(int nUserID, string strSessionKey, out string strResultMessage)
        {
            try
            {
                Dictionary<Session.Fields, object> dicConditions = new Dictionary<Session.Fields, object>();
                dicConditions[Session.Fields.AccountUserID] = nUserID;
                //dicConditions[Session.Fields.SessionKey] = strSessionKey;

                List<Session> sessions = m_dataManager.GetSelectManager().SelectSessions(dicConditions, out strResultMessage);

                if (sessions == null)
                    return false;
                else if (sessions.Count == 0)
                {
                    strResultMessage = "해당 유저 Session은 존재하지 않습니다.";
                    return false;
                } 
                else
                {
                    Session session = sessions[0];

                    if (session.SessionKey == strSessionKey)
                    {
                        // 자동 로그인 확인 여부
                        if (session.IsAutoLogin == false)
                        {   // 자동 로그인이 아니라면
                            // 마지막 Session 업데이트 시간 체크(현 시간으로부터 30초 이내인지)
                            DateTime dtSession = session.UpdateDate;
                            DateTime dtNow = DateTime.Now;

                            TimeSpan diffTime = dtNow - dtSession;
                            double dSecond = diffTime.TotalSeconds;

                            if (dSecond > 30)
                            {
                                strResultMessage = "로그아웃 되었습니다.";
                                return false;
                            }
                            else
                            {
                                session.UpdateDate = DateTime.Now;
                                m_dataManager.GetUpdateManager().UpdateSession(session);
                            }
                        }

                        strResultMessage = "해당 Session은 유효합니다.";
                        return true;
                    } 
                    else
                    {
                        strResultMessage = "다른 곳에서 로그인하였습니다.";
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                strResultMessage = e.Message;
            }

            return false;
        }

        public static string EncryptString(string str)
        {
            return AES256Cipher.AES_encrypt(str, key);
        }

        public static string DecryptString(string str)
        {
            if (str == null)
                return null;

            return AES256Cipher.AES_decrypt(str, key);
        }
    }
}
