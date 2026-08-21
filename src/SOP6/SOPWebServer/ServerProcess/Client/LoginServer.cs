using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgentFactory;
using DBUtility2;
using System.ServiceModel;
using System.Collections;
using System.Threading;

namespace ServerProcess.Client
{
    // 통합관리자를 위한 클래스
    public class LoginServer : BaseClient
    {
        private static LoginServer m_instance = null;

        public static LoginServer Instance
        {
            get { return m_instance; }
        }

        public class ClientData : ServerProcess.Client.ClientData
        {
            private int m_nSOPGenUserID = -1;
            private string m_strUserID = "";
            private string m_strUserName = "";
            private VariousData<DateTime> m_dtLogin = null;

            public int SOPGenUserID
            {
                get { return m_nSOPGenUserID; }
                set { m_nSOPGenUserID = value; }
            }

            public string UserID
            {
                get { return m_strUserID; }
                set { m_strUserID = value; }
            }

            public string UserName
            {
                get { return m_strUserName; }
                set { m_strUserName = value; }
            }

            public VariousData<DateTime> LoginTime
            {
                get { return m_dtLogin; }
                set { m_dtLogin = value; }
            }

            public ClientData()
                : base()
            {
            }

            public ClientData(string strSessionID, IPostMan postMan)
                : base(strSessionID, postMan)
            {
            }

            public ClientData(string strSessionID, IPostMan postMan, int nClientType, int nClientSubType)
                : base(strSessionID, postMan, nClientType, nClientSubType)
            {
            }
        }

        public enum CommanderErrorType
        {
            SUCCESS = 0,
            FAIL_DELETE_DAY = 1,
            FAIL_INSERT_DAY,
            FAIL_UPDATE_DAY,
            FAIL_DELETE_NIGHT,
            FAIL_INSERT_NIGHT,
            FAIL_UPDATE_NIGHT
        }

        public override int ClientType
        {
            get { return SOPWebServer.ClientType.LOGIN_SERVER; }
        }

        public LoginServer()
            : base()
        {
            m_instance = this;
        }

        public LoginServer(Factory factory, IPostOffice postOffice)
            : base(factory, postOffice)
        {
            m_agent = m_agentFactory.MakeAgent(Factory.AgentType.LogIn);
            m_instance = this;
        }

        protected override void OnLoadEvent()
        {

        }

        protected override ServerProcess.Client.ClientData MakeClientData(int nClientType, int nClientSubType, OperationContext ctx, string strIP, int nPort)
        {
            if (m_postOffice != null)
            {
                IPostMan postMan = m_postOffice.GetPostMan(ctx);
                LoginServer.ClientData data = new LoginServer.ClientData(ctx.SessionId, postMan, nClientType, nClientSubType);
                data.IP = strIP;
                data.Port = nPort;
                postMan.ClientData = data;

                // 이미 로그인된 사용자인지 확인한다.
                CheckLogin(data, ctx);
                return data;
            }

            return null;
        }

        protected override int OnReceiveEvent(Client.ClientData data, OperationContext ctx, int header, byte[] messages, ArrayList arrDatas)
        {
            // 로그인 요청
            if (header == SOPWebServer.Header.LOGIN_USER)
            {
                int nResult = ProcessLoginUser(arrDatas, data, ctx);

                if (nResult != SOPWebServer.ErrorMessageType.SUCCESS)
                    SendReject(nResult, data);

                return nResult;
            }
            // 이미 로그인된 사용자인지에 대한 회신
            else if (header == SOPWebServer.Header.CHECK_LOGIN)
            {
                int nResult = ProcessCheckLogin(arrDatas, data, ctx);

                // 로그인 상태로 인정할 수 없을 경우 강제로 로그아웃 시킨다.
                if (nResult != SOPWebServer.ErrorMessageType.SUCCESS)
                    SendLogout(data);

                return nResult;
            }
            else if (header == SOPWebServer.Header.LOGOUT_USER)
                return ProcessLogout(arrDatas, data, ctx);
            else if (header == SOPWebServer.Header.CHANGE_PASSWORD)
            {
                int nResult = ProcessChangePassword(arrDatas);
                SendChangePassword(data, ctx, nResult == SOPWebServer.ErrorMessageType.SUCCESS);
                return nResult;
            }
            else if (header == SOPWebServer.Header.SET_PASSWORD)
            {
                int nResult = ProcessSetPassword(arrDatas);
                SendSuccessMessage(SOPWebServer.Header.CHANGE_PASSWORD, data, nResult == SOPWebServer.ErrorMessageType.SUCCESS);
                return nResult;
            }
            else if (header == SOPWebServer.Header.CHANGE_NICKNAME)
            {
                string strNickName;
                int nResult = ProcessChangeNickName(arrDatas, out strNickName);
                SendChangeNickName(data, ctx, nResult == SOPWebServer.ErrorMessageType.SUCCESS, strNickName);
                return nResult;
            }
            else if (header == SOPWebServer.Header.JOIN_USER)
            {
                int nID;
                int nResult = ProcessJoinUser(arrDatas, out nID);
                SendJoinUser(data, ctx, nID);
                return nResult;
            }
            else if (header == SOPWebServer.Header.INTERNAL_MESSAGE)
            {
                // 통합관리자를 통한 Local Message를 SOP Server를 통해 전달하는 경우
                return ProcessInternalMessage(messages);
            }
            else if (header == SOPWebServer.Header.CHANGE_SOPGENUSER_COMMANDER)
                return ProcessChangeSOPGenCommander(arrDatas, data);

            return SOPWebServer.ErrorMessageType.UNKNOWN_HEADER;
        }

        private void SendJoinUser(Client.ClientData data, OperationContext ctx, int nUserID)
        {
            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(nUserID);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            SendClientData(SOPWebServer.Header.JOIN_USER, bytes, data);
            /*arrDatas.Clear();

            arrDatas.Add(ctx);
            arrDatas.Add(SOPWebServer.Header.JOIN_USER);
            arrDatas.Add(bytes);

            m_agent.TimerDatas.Enqueue(arrDatas);*/
        }

        private void SendChangeNickName(Client.ClientData data, OperationContext ctx, bool isSuccess, string strNickName)
        {
            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(isSuccess ? 1 : 0);
            arrDatas.Add(strNickName);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            SendClientData(SOPWebServer.Header.CHANGE_NICKNAME, bytes, data);
            /*arrDatas.Clear();

            arrDatas.Add(ctx);
            arrDatas.Add(SOPWebServer.Header.CHANGE_NICKNAME);
            arrDatas.Add(bytes);

            m_agent.TimerDatas.Enqueue(arrDatas);*/
        }

        private void SendChangePassword(Client.ClientData data, OperationContext ctx, bool isSuccess)
        {
            SendSuccessMessage(SOPWebServer.Header.CHANGE_PASSWORD, data, isSuccess);
        }

        private void SendSuccessMessage(int header, Client.ClientData data, bool isSuccess)
        {
            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(isSuccess ? 1 : 0);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            SendClientData(header, bytes, data);
            /*arrDatas.Clear();

            arrDatas.Add(ctx);
            arrDatas.Add(header);
            arrDatas.Add(bytes);

            m_agent.TimerDatas.Enqueue(arrDatas);*/
        }

        private void SendReject(int nErrorMessage, Client.ClientData data)
        {
            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(nErrorMessage);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            SendClientData(SOPWebServer.Header.REJECT_LOGIN, bytes, data);
            /*arrDatas.Clear();

            arrDatas.Add(ctx);
            arrDatas.Add(SOPWebServer.Header.REJECT_LOGIN);
            arrDatas.Add(bytes);

            m_agent.TimerDatas.Enqueue(arrDatas);*/
        }

        private void SendLogout(Client.ClientData data)
        {
            SendClientData(SOPWebServer.Header.LOGOUT_USER, null, data);
            /*ArrayList arrDatas = new ArrayList();

            arrDatas.Add(ctx);
            arrDatas.Add(SOPWebServer.Header.LOGOUT_USER);
            arrDatas.Add(null);

            m_agent.TimerDatas.Enqueue(arrDatas);*/
        }

        private void SendSOPGenCommanderResult(Client.ClientData data, CommanderErrorType error)
        {
            ArrayList arrDatas = new ArrayList();
            arrDatas.Add((int)error);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            SendClientData(SOPWebServer.Header.CHANGE_SOPGENUSER_COMMANDER, bytes, data);
            /*arrDatas.Clear();

            arrDatas.Add(ctx);
            arrDatas.Add(SOPWebServer.Header.CHANGE_SOPGENUSER_COMMANDER);
            arrDatas.Add(bytes);

            m_agent.TimerDatas.Enqueue(arrDatas);*/
        }

        private int ProcessChangeSOPGenCommander(ArrayList arrDatas, Client.ClientData data)
        {
            if (arrDatas.Count >= 6 && arrDatas[0] is int && arrDatas[1] is string &&
                arrDatas[2] is string && arrDatas[3] is int && arrDatas[4] is int &&
                arrDatas[5] is int)
            {
                int nSOPGenUserID = (int)arrDatas[0];
                string strDisplayText = (string)arrDatas[1];
                string strPhoneNumber = (string)arrDatas[2];
                int nExternal = (int)arrDatas[3];
                int nCommanderMemberID = (int)arrDatas[4];
                int nDayLight = (int)arrDatas[5];

                bool day = (nDayLight & 1) == 1;
                bool night = (nDayLight & 2) == 2;
                CommanderErrorType error;

                if (ChangeSOPGenCommander(nSOPGenUserID, 1, day, strDisplayText, strPhoneNumber, nExternal, nCommanderMemberID, out error) == false)
                {
                    SendSOPGenCommanderResult(data, error);
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                }

                if (ChangeSOPGenCommander(nSOPGenUserID, 0, night, strDisplayText, strPhoneNumber, nExternal, nCommanderMemberID, out error) == false)
                {
                    SendSOPGenCommanderResult(data, error);
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                }

                SendSOPGenCommanderResult(data, error);
                return SOPWebServer.ErrorMessageType.SUCCESS;
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        private bool ChangeSOPGenCommander(int nSOPGenUserID, int nDayLight, bool use, string strDisplayText, string strPhoneNumber, int nExternal, int nCommanderMemberID, out CommanderErrorType error)
        {
            error = CommanderErrorType.SUCCESS;

            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return false;

            string strSQL = "SELECT SOPGenUserID FROM SOPGenuserCommander WHERE SOPGenUserID = " + nSOPGenUserID.ToString() + " and DayLight = " + nDayLight.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (use)
            {
                if (arrResult != null && arrResult.Count > 0)
                {
                    // 주(야)간 책임자가 있다면 갱신
                    strSQL = "UPDATE SOPGenUserCommander "
                                + "SET MemberType = " + nExternal
                                + ",MemberID = " + (nCommanderMemberID < 0 ? "NULL" : nCommanderMemberID.ToString())
                                + ",DisplayText = '" + strDisplayText + "'"
                                + ",CallerPhoneNumber = '" + strPhoneNumber + "'"
                                + " WHERE SOPGenUserID = " + nSOPGenUserID.ToString()
                                + " AND DayLight = " + nDayLight.ToString();

                    arrResult = dbMgr.GetResultData(strSQL);

                    if (arrResult == null)
                    {
                        dbMgr.Close();
                        error = nDayLight == 1 ? CommanderErrorType.FAIL_UPDATE_DAY : CommanderErrorType.FAIL_UPDATE_NIGHT;
                        return false;
                    }
                }
                else
                {
                    // 주(야)간 책임자가 없다면 추가
                    strSQL = string.Format("Insert into SOPGenUserCommander (SOPGenUserID, DayLight, MemberType, MemberID, DisplayText, CallerPhoneNumber) Values ({0}, {1}, {2}, {3}, '{4}', '{5}')",
                        nSOPGenUserID, nDayLight, nExternal, nCommanderMemberID < 0 ? "NULL" : nCommanderMemberID.ToString(), strDisplayText, strPhoneNumber);

                    arrResult = dbMgr.GetResultData(strSQL);

                    if (arrResult == null)
                    {
                        dbMgr.Close();
                        error = nDayLight == 1 ? CommanderErrorType.FAIL_INSERT_DAY : CommanderErrorType.FAIL_INSERT_NIGHT;
                        return false;
                    }
                }
            }
            else
            {
                if (arrResult != null && arrResult.Count > 0)
                {
                    //주(야)간 책임자 데이터가 있으면 삭제.                
                    strSQL = "DELETE FROM SOPGenUserCommander"
                                + " WHERE SOPGenUserID = " + nSOPGenUserID.ToString()
                                + " AND DayLight = " + nDayLight.ToString();

                    arrResult = dbMgr.GetResultData(strSQL);

                    if (arrResult == null)
                    {
                        dbMgr.Close();
                        error = nDayLight == 1 ? CommanderErrorType.FAIL_DELETE_DAY : CommanderErrorType.FAIL_DELETE_NIGHT;
                        return false;
                    }
                }
            }

            dbMgr.Close();
            return true;
        }

        private int ProcessInternalMessage(byte[] bytes)
        {
            SendClientData(SOPWebServer.Header.INTERNAL_MESSAGE, bytes, SOPWebServer.ClientType.LOGIN_SERVER, SOPWebServer.ClientSubType.INTEGRATED_MANAGER);
            /*ArrayList arrDatas = new ArrayList();

            // 전체 통합관리자들에게 보낸다.
            arrDatas.Add(SOPWebServer.ClientType.LOGIN_SERVER);
            arrDatas.Add(SOPWebServer.ClientSubType.INTEGRATED_MANAGER);
            arrDatas.Add(SOPWebServer.Header.INTERNAL_MESSAGE);
            arrDatas.Add(bytes);

            m_agent.TimerDatas.Enqueue(arrDatas);*/
            return SOPWebServer.ErrorMessageType.SUCCESS;
        }

        private int ProcessJoinUser(ArrayList arrDatas, out int nID)
        {
            nID = -1;

            if (arrDatas.Count >= 9 && arrDatas[0] is int && arrDatas[1] is string &&
                arrDatas[2] is string && arrDatas[3] is string && arrDatas[4] is string &&
                arrDatas[5] is string && arrDatas[6] is string && arrDatas[7] is string && arrDatas[8] is int)
            {
                DirectDBManager dbMgr = m_dbMgr.Clone();

                if (dbMgr.Connect() == false)
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                int nMemberID = (int)arrDatas[0];
                string strUserID = (string)arrDatas[1];
                string strPassword = (string)arrDatas[2];
                string strNickName = (string)arrDatas[3];
                string strDisplayText = (string)arrDatas[4];
                string strPhoneNumber = (string)arrDatas[5];
                int nExternal = Convert.ToInt32(arrDatas[6]);
                int nCommanderMemberID = Convert.ToInt32(arrDatas[7]);
                int nDayLight = (int)arrDatas[8];

                string strSQL = string.Format("select id from SOPGenUser where UserID = '{0}' and SiteID = {1}", strUserID, dbMgr.SiteID);
                ArrayList arrResult = dbMgr.GetResultData(strSQL);

                if (arrResult == null)
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                }
                else if (arrResult.Count > 0)
                {
                    dbMgr.Close();
                    nID = 0;
                    return SOPWebServer.ErrorMessageType.ALREADY_USING_ID;
                }

                // Transaction 처리
                if (dbMgr.BeginBatch() == false)
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                }
                
                nID = Data.AlarmManager.GetMaxTableID(dbMgr, "SOPGenUser", true) + 1;

                if (dbMgr.SiteID == 201)
                {
                    // .TODO : 201 경우 nCommanderMemberID 값이 UserLevel 값을 뜻함
                    strSQL = string.Format("Insert into SOPGenUser (ID, MemberID, UserLevel, Password, UserID, NickName, SiteID ) values ({0}, {1}, {2}, '{3}', '{4}', '{5}', {6})",
                        nID, nMemberID < 0 ? "NULL" : nMemberID.ToString(), nCommanderMemberID, strPassword, strUserID, strNickName, dbMgr.SiteID);
                }
                else
                {
                    strSQL = string.Format("Insert into SOPGenUser (ID, MemberID, UserLevel, Password, UserID, NickName, SiteID ) values ({0}, {1}, {2}, '{3}', '{4}', '{5}', {6})",
                        nID, nMemberID < 0 ? "NULL" : nMemberID.ToString(), 2, strPassword, strUserID, strNickName, dbMgr.SiteID);
                }
                

                if (dbMgr.GetBatchData(strSQL) == null)
                {
                    dbMgr.BatchRollback();
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                }

                // 주간 책임자 추가
                if ((nDayLight & 1) == 1)
                {
                    string strCommanderMemberID = nCommanderMemberID < 0 ? "NULL" : nCommanderMemberID.ToString();

                    if (InsertSOPGenUserCommand(dbMgr, nID, 1, nExternal, strCommanderMemberID, strDisplayText, strPhoneNumber) == false)
                    {
                        dbMgr.Close();
                        return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                    }
                }

                // 야간 책임자 추가
                if ((nDayLight & 2) == 2)
                {
                    string strCommanderMemberID = nCommanderMemberID < 0 ? "NULL" : nCommanderMemberID.ToString();

                    if (InsertSOPGenUserCommand(dbMgr, nID, 0, nExternal, strCommanderMemberID, strDisplayText, strPhoneNumber) == false)
                    {
                        dbMgr.Close();
                        return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                    }
                }
                
                if (dbMgr.SiteID == 201)
                {
                    // 담당 빌딩 선택 (우선적으로 보여질 재난 장소(빌딩)을 선택한다)
                    ArrayList arrResult1 = dbMgr.GetBatchData("Select LevelName From SOPGenLevel Where ID = " + nCommanderMemberID);
                    if (arrResult1 != null && arrResult1.Count > 0)
                    {
                        string inCharge = DBUtility2.WebDBManager.GetStringField(arrResult1[0]);

                        arrResult1 = dbMgr.GetBatchData("Select ID, BuildingName From Building");
                                                
                        if (arrResult1 != null && arrResult1.Count > 0)
                        {
                            for (int i = 0; i < arrResult1.Count; i+=2)
                            {
                                int nBuildingID = DBUtility2.WebDBManager.GetIntField(arrResult1[i].ToString(), -1);
                                string strBuildingName = DBUtility2.WebDBManager.GetStringField(arrResult1[i + 1].ToString());

                                if (nBuildingID <= 0)
                                    continue;

                                if (inCharge.Replace(" ", "").Contains(strBuildingName.Replace(" ", "")))
                                {
                                    strSQL = string.Format("Insert into SOPGenUserBuilding (UserID, BuildingID) Values ({0}, {1})", nID, nBuildingID);
                                    break;
                                }
                            }
                        }

                        if (dbMgr.GetBatchData(strSQL) == null)
                        {
                            dbMgr.BatchRollback();
                            dbMgr.Close();
                            return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                        }
                    }
                }

                if (dbMgr.BatchCommit() == false)
                {
                    dbMgr.BatchRollback();
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                }

                dbMgr.Close();
                return SOPWebServer.ErrorMessageType.SUCCESS;
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        private bool InsertSOPGenUserCommand(DirectDBManager dbMgr, int nID, int nOption, int nExternal, string strCommanderMemberID, string strDisplayText, string strPhoneNumber)
        {
            string strSQL = string.Format("Insert into SOPGenUserCommander (SOPGenUserID, DayLight, MemberType, MemberID, DisplayText, CallerPhoneNumber) Values ({0}, {1}, {2}, {3}, '{4}', '{5}')",
                    nID, nOption, nExternal, strCommanderMemberID, strDisplayText, strPhoneNumber);

            if (dbMgr.GetBatchData(strSQL) == null)
            {
                dbMgr.BatchRollback();
                return false;
            }

            return true;
        }

        private int ProcessChangeNickName(ArrayList arrDatas, out string strNickName)
        {
            strNickName = "";

            if (arrDatas.Count >= 2 && arrDatas[0] is int && arrDatas[1] is string)
            {
                DirectDBManager dbMgr = m_dbMgr.Clone();

                if (dbMgr.Connect() == false)
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                int nSOPGenUserID = (int)arrDatas[0];
                strNickName = (string)arrDatas[1];

                string strSQL = string.Format("Update SOPGenUser set NIckName = '{0}' where ID = {1}", strNickName, nSOPGenUserID);

                if (dbMgr.GetResultData(strSQL) == null)
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                }
                else
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.SUCCESS;
                }
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        private int ProcessSetPassword(ArrayList arrDatas)
        {
            if (arrDatas.Count >= 2 && arrDatas[0] is string && arrDatas[1] is string)
            {
                DirectDBManager dbMgr = m_dbMgr.Clone();

                if (dbMgr.Connect() == false)
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                string strGenUserID = (string)arrDatas[0];
                string strNewPassword = (string)arrDatas[1];

                string strSQL = string.Format("Update SOPGenUser set Password = '{0}' where UserID = '{1}' and SiteID = {2}",
                    strNewPassword, strGenUserID, dbMgr.SiteID);

                if (dbMgr.GetResultData(strSQL) == null)
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                }
                else
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.SUCCESS;
                }
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        private int ProcessChangePassword(ArrayList arrDatas)
        {
            if (arrDatas.Count >= 3 && arrDatas[0] is int && arrDatas[1] is string && arrDatas[2] is string)
            {
                DirectDBManager dbMgr = m_dbMgr.Clone();

                if (dbMgr.Connect() == false)
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                int nSOPGenUserID = (int)arrDatas[0];
                string strCurrentPassword = (string)arrDatas[1];
                string strNewPassword = (string)arrDatas[2];

                string strSQL = string.Format("Select ID from SOPGenUser where ID = {0} and Password = '{1}' and SiteID = {2}",
                    nSOPGenUserID, strCurrentPassword, dbMgr.SiteID);
                ArrayList arrResult = dbMgr.GetResultData(strSQL);

                if (arrResult != null && arrResult.Count > 0)
                {
                    strSQL = string.Format("Update SOPGenUser set Password = '{0}' where ID = {1}", strNewPassword, nSOPGenUserID);

                    if (dbMgr.GetResultData(strSQL) == null)
                    {
                        dbMgr.Close();
                        return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                    }
                    else
                    {
                        dbMgr.Close();
                        return SOPWebServer.ErrorMessageType.SUCCESS;
                    }
                }
                else
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                }
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        private int ProcessLogout(ArrayList arrDatas, Client.ClientData data, OperationContext ctx)
        {
            LoginServer.ClientData thisClient = (LoginServer.ClientData)data;

            thisClient.UserID = "";
            thisClient.SOPGenUserID = -1;
            thisClient.UserName = "";
            thisClient.LoginTime = null;

            return SOPWebServer.ErrorMessageType.SUCCESS;
        }

        private int ProcessCheckLogin(ArrayList arrDatas, Client.ClientData data, OperationContext ctx)
        {
            if (arrDatas.Count >= 1 && arrDatas[0] is string)
            {
                DirectDBManager dbMgr = m_dbMgr.Clone();

                if (dbMgr.Connect() == false)
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                string strUserID = (string)arrDatas[0];

                string strSQL = string.Format("select ID, MemberID, NickName from SOPGenUser where UserID = '{0}' and SiteID = {1}", strUserID, dbMgr.SiteID);
                ArrayList arrResult = dbMgr.GetResultData(strSQL);

                if (arrResult == null)
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                }

                if (arrResult.Count < 3)
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.INVALID_ID_OR_PASSWORD;
                }

                VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());
                VariousData<int> regularMemberID = WebDBManager.GetIntField(arrResult[1].ToString());
                string strNickName = WebDBManager.GetStringField(arrResult[2]);

                if (id == null || strNickName == null)
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                }

                string strUserName = "";

                if (regularMemberID != null)
                {
                    strSQL = "Select MemberName from CompanyMember where ID = " + regularMemberID.Data.ToString();
                    arrResult = dbMgr.GetResultData(strSQL);

                    if (arrResult != null && arrResult.Count > 0)
                        strUserName = WebDBManager.GetStringField(arrResult[0], "");
                }

                LoginServer.ClientData thisClient = (LoginServer.ClientData)data;

                thisClient.UserID = strUserID;
                thisClient.SOPGenUserID = id.Data;
                thisClient.UserName = strUserName;
                thisClient.LoginTime = new VariousData<DateTime>(DateTime.Now);

                dbMgr.Close();
                return SOPWebServer.ErrorMessageType.SUCCESS;
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        private int ProcessLoginUser(ArrayList arrDatas, Client.ClientData data, OperationContext ctx)
        {
            if (arrDatas.Count >= 2 && arrDatas[0] is string && arrDatas[1] is string)
            {
                DirectDBManager dbMgr = m_dbMgr.Clone();

                if (dbMgr.Connect() == false)
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                string strUserID = (string)arrDatas[0];
                string strPassword = (string)arrDatas[1];

                string strSQL = string.Format("select ID, MemberID, NickName from SOPGenUser where UserID = '{0}' and Password = '{1}' and SiteID = {2}", strUserID, strPassword, dbMgr.SiteID);
                ArrayList arrResult = dbMgr.GetResultData(strSQL);

                if (arrResult == null)
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                }

                if (arrResult.Count < 3)
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.INVALID_ID_OR_PASSWORD;
                }

                VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());
                VariousData<int> regularMemberID = WebDBManager.GetIntField(arrResult[1].ToString());
                string strNickName = WebDBManager.GetStringField(arrResult[2]);

                if (id == null || strNickName == null)
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                }

                string strUserName = "";

                if (regularMemberID != null)
                {
                    strSQL = "Select MemberName from CompanyMember where ID = " + regularMemberID.Data.ToString();
                    arrResult = dbMgr.GetResultData(strSQL);

                    if (arrResult != null && arrResult.Count > 0)
                        strUserName = WebDBManager.GetStringField(arrResult[0], "");
                }

                LoginServer.ClientData thisClient = (LoginServer.ClientData)data;
                List<Client.ClientData> clientDatas = GetClientDatas();

                foreach (Client.ClientData client in clientDatas)
                {
                    if (client is LoginServer.ClientData)
                    {
                        LoginServer.ClientData clientData = (LoginServer.ClientData)client;

                        if (clientData.SOPGenUserID == id.Data && clientData.UserID == strUserID)
                        {
                            if (clientData == thisClient)
                            {
                                dbMgr.Close();
                                // 이미 로그인 되어있는 상태인데 다시 로그인 요청하는 경우
                                return AcceptLogin(thisClient, ctx, id.Data, strUserID, strUserName, strNickName, thisClient.LoginTime == null ? DateTime.Now : thisClient.LoginTime.Data);
                            }
                            else
                            {
                                dbMgr.Close();
                                // 다른곳에서 이미 같은 아이디로 로그인 하였음
                                return SOPWebServer.ErrorMessageType.ALREADY_USING_ID;
                            }
                        }
                    }
                }

                dbMgr.Close();
                return AcceptLogin(thisClient, ctx, id.Data, strUserID, strUserName, strNickName, DateTime.Now);
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        // 접속한 Client가 로그인된 상태인지 확인한다.
        private void CheckLogin(LoginServer.ClientData data, OperationContext ctx)
        {
            ArrayList arrDatas = new ArrayList();

            byte[] bytes = null;

            arrDatas.Add(data);
            arrDatas.Add(SOPWebServer.Header.CHECK_LOGIN);
            arrDatas.Add(bytes);

            Thread t = new Thread(new ParameterizedThreadStart(CheckLoginThread));
            t.Start(arrDatas);
        }

        private void CheckLoginThread(object arg)
        {
            // 동기화 문제를 해결하기 위하여 일부러 0.1초 늦춘다.
            Thread.Sleep(100);

            ArrayList arrDatas = (ArrayList)arg;
            m_agent.TimerDatas.Enqueue(arrDatas);
        }

        private int AcceptLogin(LoginServer.ClientData data, OperationContext ctx, int nSOPGenUserID, string strUserID, string strUserName, string strNickName, DateTime dtLogin)
        {
            data.UserID = strUserID;
            data.SOPGenUserID = nSOPGenUserID;
            data.UserName = strUserName;
            data.LoginTime = new VariousData<DateTime>(dtLogin);

            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(nSOPGenUserID);
            arrDatas.Add(strUserName);
            arrDatas.Add(strNickName);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            SendClientData(SOPWebServer.Header.ACCEPT_LOGIN, bytes, data);
            /*arrDatas.Clear();

            arrDatas.Add(ctx);
            arrDatas.Add(SOPWebServer.Header.ACCEPT_LOGIN);
            arrDatas.Add(bytes);
            m_agent.TimerDatas.Enqueue(arrDatas);*/

            return SOPWebServer.ErrorMessageType.SUCCESS;
        }
    }
}
