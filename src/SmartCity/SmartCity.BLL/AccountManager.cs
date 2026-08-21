using SmartCity.BLL.Models.Response;
using SmartCity.IDAL;
using SmartCity.Model;
using System;
using System.Collections.Generic;

namespace SmartCity.BLL
{
    public class AccountManager
    {
        private IDataManager m_dataManager = null;

        public AccountManager(IDataManager dataManager)
        {
            m_dataManager = dataManager;
        }

        public ResponseLogin Login(string strUserID, string strPW, string strKey)
        {
            ResponseLogin result = new ResponseLogin();

            // ID 값으로 유저를 검색
            Dictionary<AccountUser.Fields, object> dicConditions = new Dictionary<AccountUser.Fields, object>();
            dicConditions[AccountUser.Fields.UserID] = strUserID;

            string strAdditionalConditions = "";

            string strErrorMessage = null;
            List<AccountUser> users = m_dataManager.GetSelectManager().SelectAccountUsers(dicConditions, strAdditionalConditions, out strErrorMessage);
            if (users == null || users.Count == 0)
            {
                result.Success = false;
                result.Message = strErrorMessage;
                return result;
            }

            AccountUser user = users[0];

            if (user.Password != strPW)
            {
                strErrorMessage = "비밀번호가 일치하지 않습니다.";

                result.Success = false;
                result.Message = strErrorMessage;
                return result;
            }
            else
            {
                // 계정 정보가 맞다면 해당 유저의 세션 키 값 저장
                bool bRet = SaveAccoutSession(user.ID, strKey, out strErrorMessage);
                if (bRet == false)
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }

                result.Success = true;
                result.Message = strErrorMessage;
                result.KEY = strKey;
                return result;
            }
        }

        public bool SaveAccoutSession(int nUserID, string strKey, out string strResultMessage)
        {
            bool bResult = false;
            strResultMessage = "";

            // ID 값으로 유저를 검색
            Dictionary<AccountSession.Fields, object> dicConditions = new Dictionary<AccountSession.Fields, object>();
            dicConditions[AccountSession.Fields.AccountUserID] = nUserID;

            string strAdditionalConditions = "";

            List<AccountSession> accountSessions = m_dataManager.GetSelectManager().SelectAccountSessions(dicConditions, strAdditionalConditions, out strResultMessage);
            if (accountSessions == null)
                return bResult;

            else if (accountSessions.Count == 0)
            {   // 해당 유저의 세션이 없음. 새로 생성
                DateTime date = DateTime.Now;

                AccountSession session = m_dataManager.GetCreateManager().CreateAccountSession(nUserID, strKey, date, null);
                if (session == null)
                {
                    strResultMessage = "CreateAccountSession Error가 발생하였습니다.";
                    return bResult;
                }

                bResult = true;
                strResultMessage = "로그인 성공하였습니다.";
            }
            else if (accountSessions.Count != 0)
            {   // 해당 유저의 세션 값이 있음. 기존의 해당 유저의 세션 지우고 새로 생성 (다중 로그인이 되어 있을 수도 있으므로)
                DateTime date = DateTime.Now;

                strAdditionalConditions = "";
                dicConditions = new Dictionary<AccountSession.Fields, object>();
                dicConditions[AccountSession.Fields.AccountUserID] = nUserID;

                // 기존의 해당 유저의 세션 지우기
                bResult = m_dataManager.GetDeleteManager().DeleteAccountSession(dicConditions, strAdditionalConditions, out strResultMessage);
                if (!bResult)
                    return bResult;

                AccountSession session = m_dataManager.GetCreateManager().CreateAccountSession(nUserID, strKey, date, null);
                if (session == null)
                {
                    strResultMessage = "CreateAccountSession Error가 발생하였습니다.";
                    return bResult;
                }

                strResultMessage = "기존 로그인을 해제 후 로그인을 하였습니다..";
                bResult = true;
            }

            return bResult;
        }

        public ResponseLogin SessionLogin(string strKey)
        {
            ResponseLogin result = new ResponseLogin();

            Dictionary<AccountSession.Fields, object> dicConditions = new Dictionary<AccountSession.Fields, object>();
            dicConditions[AccountSession.Fields.SessionKey] = strKey;

            string strAdditionalConditions = "";
            string strErrorMessage = null;

            List<AccountSession> accountSessions = m_dataManager.GetSelectManager().SelectAccountSessions(dicConditions, strAdditionalConditions, out strErrorMessage);
            if (accountSessions == null)
            {
                result.Success = false;
                result.Message = strErrorMessage;
                return result;
            }
            else if (accountSessions.Count == 0)
            {
                result.Success = false;
                result.Message = "세션이 로그아웃 되었습니다.";
                return result;
            }

            AccountSession accountSession = accountSessions[0];

            // ID 값으로 유저를 검색
            AccountUser user = m_dataManager.GetSelectManager().SelectAccountUser(accountSession.AccountUserID, out strErrorMessage);
            if (user == null)
            {
                result.Success = false;
                result.Message = strErrorMessage;
                return result;
            }

            AccountLevel level = m_dataManager.GetSelectManager().SelectAccountLevel(user.UserLevel, out strErrorMessage);
            if (level == null)
            {
                result.Success = false;
                result.Message = strErrorMessage;
                return result;
            }

            result.User = ApplicationUser.MakeUser(user, level);
            result.KEY = strKey;
            result.Success = true;

            return result;
        }

        public MessageResult ChangePassword(int nUserID, string strOldPW, string strNewPW)
        {
            MessageResult result = new MessageResult();

            string strErrorMessage = null;
            AccountUser user = m_dataManager.GetSelectManager().SelectAccountUser(nUserID, out strErrorMessage);
            if (user == null)
            {
                result.Success = false;
                result.Message = strErrorMessage;
                return result;
            }

            //AccountUser user = users[0];

            if (user.Password != strOldPW)
            {
                strErrorMessage = "비밀번호가 일치하지 않습니다.";

                result.Success = false;
                result.Message = strErrorMessage;
                return result;
            }
            else
            {
                user.Password = strNewPW;

                bool bRet = m_dataManager.GetUpdateManager().UpdateAccountUser(user, out strErrorMessage);
                if (bRet == false)
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return result;
                }

                result.Success = true;
                result.Message = "비밀번호를 변경하였습니다.";
                return result;
            }
        }

        public MessageResult Logout(string strKey)
        {
            MessageResult result = new MessageResult();

            Dictionary<AccountSession.Fields, object> dicConditions = new Dictionary<AccountSession.Fields, object>();
            dicConditions[AccountSession.Fields.SessionKey] = strKey;

            string strAdditionalConditions = "";
            string strErrorMessage = null;

            List<AccountSession> accountSessions = m_dataManager.GetSelectManager().SelectAccountSessions(dicConditions, strAdditionalConditions, out strErrorMessage);
            if (accountSessions == null || accountSessions.Count == 0)
            {
                result.Success = false;
                result.Message = strErrorMessage;
                return result;
            }

            AccountSession accountSession = accountSessions[0];

            // 기존의 해당 유저의 세션 지우기
            bool bResult = m_dataManager.GetDeleteManager().DeleteAccountSession(accountSession.ID, out strErrorMessage);
            if (!bResult)
            {
                result.Success = false;
                result.Message = strErrorMessage;
                return result;
            }

            result.Success = true;
            result.Message = "로그아웃 되었습니다.";

            return result;
        }

        public MessageResult CheckUserID(string strUserID)
        {
            MessageResult result = new MessageResult();

            Dictionary<AccountUser.Fields, object> dicConditions = new Dictionary<AccountUser.Fields, object>();
            dicConditions[AccountUser.Fields.UserID] = strUserID;

            string strAdditionalConditions = "";
            string strErrorMessage = null;

            List<AccountUser> accountUsers = m_dataManager.GetSelectManager().SelectAccountUsers(dicConditions, strAdditionalConditions, out strErrorMessage);
            if (accountUsers == null)
            {
                result.Success = false;
                result.Message = strErrorMessage;
                return result;
            } else if (accountUsers.Count == 0)
            {
                result.Success = false;
                result.Message = "해당 유저ID가 존재하지 않습니다.";
                return result;
            }

            result.Success = true;
            result.Message = "해당 유저ID가 존재합니다.";

            return result;
        }

        public MessageResult CheckCode(string strCode)
        {
            MessageResult result = new MessageResult();

            Dictionary<Options.Fields, object> dicConditions = new Dictionary<Options.Fields, object>();
            dicConditions[Options.Fields.PropertyName] = "AccountCode";
            dicConditions[Options.Fields.PropertyValue] = strCode;

            string strAdditionalConditions = "";
            string strErrorMessage = null;

            List<Options> accountUsers = m_dataManager.GetSelectManager().SelectOptions(dicConditions, strAdditionalConditions, out strErrorMessage);

            if (accountUsers == null)
            {
                result.Success = false;
                result.Message = strErrorMessage;
                return result;
            }
            else if (accountUsers.Count == 0)
            {
                result.Success = false;
                result.Message = "인증코드가 맞질 않습니다. 관리자에게 문의해주세요.";
                return result;
            }

            result.Success = true;
            result.Message = "인증 성공하였습니다.";

            return result;
        }

        public MessageResult PWDFind(string strUserID, string strPW)
        {
            MessageResult result = new MessageResult();

            // ID 값으로 유저를 검색
            Dictionary<AccountUser.Fields, object> dicConditions = new Dictionary<AccountUser.Fields, object>();
            dicConditions[AccountUser.Fields.UserID] = strUserID;

            string strAdditionalConditions = "";

            string strErrorMessage = null;
            List<AccountUser> users = m_dataManager.GetSelectManager().SelectAccountUsers(dicConditions, strAdditionalConditions, out strErrorMessage);
            if (users == null || users.Count == 0)
            {
                result.Success = false;
                result.Message = strErrorMessage;
                return result;
            }

            AccountUser user = users[0];
            user.Password = strPW;

            bool bResult = m_dataManager.GetUpdateManager().UpdateAccountUser(user, out strErrorMessage);

            if (!bResult)
            {
                result.Success = false;
                result.Message = strErrorMessage;
                return result;
            }

            result.Success = true;
            result.Message = "비밀번호가 변경되었습니다.";
            return result;

        }
    }
}
