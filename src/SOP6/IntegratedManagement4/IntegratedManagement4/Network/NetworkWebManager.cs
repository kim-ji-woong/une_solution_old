using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOPWebClient;
using DBUtility2;
using System.Collections;
using System.Threading;

namespace IntegratedManagement4
{
    public class NetworkWebManager : IPostMan
    {
        private PostBox m_postBox = null;
        private WebDBManager m_dbMgr = null;
        private bool m_shutdownThread = false;
        private bool m_isConnected = false;
        private int m_nPort = -1;
        private DateTime m_dtLastSendMessage = new DateTime();

        public bool IsConnected
        {
            get { return m_isConnected; }
        }

        public NetworkWebManager(WebDBManager dbMgr)
        {
            m_dbMgr = dbMgr;
            InitLog();

            int nPort = ReadServerPort();
            SetPostBox(nPort);

            Thread t = new Thread(new ThreadStart(ConnectionThread));
            t.Start();
        }

        private int ReadServerPort()
        {
            string strSQL = "Select Port from SensorServerPort where Name = '" + SOPWebServer.ServerPort.SOP_WEB_SERVER + "' and SiteID = " + m_dbMgr.SiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            VariousData<int> port = WebDBManager.GetIntField(arrResult[0].ToString());

            if (port == null)
                return -1;

            return port.Data;
        }

        private string GetSOPWebServerURL()
        {
            string strSQL = "Select PropertyValue from OptionSOPSimulator where PropertyName = 'SOPWebServerURL' and SiteID = " + m_dbMgr.SiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return m_dbMgr.WebServerURL;

            string strWebServerURL = WebDBManager.GetStringField(arrResult[0]);

            if (strWebServerURL == null)
                return m_dbMgr.WebServerURL;

            return strWebServerURL;
        }

        private void SetPostBox(int nPort)
        {
            if (nPort > 0)
            {
                m_postBox = new PostBox();
                m_postBox.WebServerURL = GetSOPWebServerURL();
                m_postBox.PostMan = this;

                m_nPort = nPort;
            }
        }

        private void ConnectionThread()
        {
            int nPrevMonth = DateTime.Now.Month;

            while (m_shutdownThread == false)
            {
                if (m_isConnected == false)
                {
                    int nPort = ReadServerPort();

                    if (m_nPort != nPort)
                        SetPostBox(nPort);

                    if (m_postBox != null)
                    {
                        if (m_postBox.Connect(SOPWebServer.ClientType.LOGIN_SERVER, SOPWebServer.ClientSubType.INTEGRATED_MANAGER))
                        {
                            m_isConnected = true;
                        }
                    }
                }
                else
                {
                    TimeSpan span = DateTime.Now - m_dtLastSendMessage;

                    // 마지막 메시지를 보낸 이후 3초 이상 지났는지 확인한다.
                    if (span.TotalSeconds > 3.0)
                    {
                        // 접속이 유지되고 있는지 확인한다.
                        SendMessage(SOPWebServer.Header.ARE_YOU_THERE, null);
                    }
                }

                Thread.Sleep(1000);

                // 한달에 한번 통신로그 삭제
                if (DateTime.Now.Month != nPrevMonth)
                {
                    nPrevMonth = DateTime.Now.Month;

                    // 시간이 경과한 로그 삭제
                    Thread t = new Thread(DeleteLogThread);
                    t.Start();
                }
            }
        }

        // 1달이 경과한 통신로그 삭제
        private void DeleteLogThread()
        {
            try
            {
                string strPath = System.Windows.Forms.Application.ExecutablePath;
                string szParentPath = System.IO.Path.GetDirectoryName(strPath);

                string[] arrFiles = System.IO.Directory.GetFiles(szParentPath + "\\logs");

                List<string> keys = new List<string>();
                keys.Add("IntegratedManager.log-");
                keys.Add(ClientDataSDMS.LogFileName + "-");
                keys.Add(ClientDataSOPSimulator.LogFileName + "-");

                DateTime dtNow = DateTime.Now;

                foreach (string strFile in arrFiles)
                {
                    foreach (string strKey in keys)
                    {
                        int len = strKey.Length;

                        if (DeleteLogFile(strFile, strKey, len, dtNow))
                            break;
                    }
                }
            }
            catch (System.IO.DirectoryNotFoundException)
            {
            }
        }

        public void ReleaseThread()
        {
            m_shutdownThread = true;
        }

        private bool DeleteLogFile(string strFile, string strKey, int len, DateTime dtNow)
        {
            int nYear, nMonth, nDay;
            int nIndex = strFile.IndexOf(strKey);

            if (nIndex < 0)
                return false;

            string strDate = strFile.Substring(nIndex + len);

            int nIndex1 = strDate.IndexOf('-');
            int nIndex2 = strDate.LastIndexOf('-');

            if (nIndex1 < 0 || nIndex2 < 0 || nIndex1 == nIndex2)
                return false;

            string strYear = strDate.Substring(0, nIndex1);
            string strMonth = strDate.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
            string strDay = strDate.Substring(nIndex2 + 1);

            if (!int.TryParse(strYear, out nYear))
                return false;
            if (!int.TryParse(strMonth, out nMonth))
                return false;
            if (!int.TryParse(strDay, out nDay))
                return false;

            if (IsPassedTime(dtNow, nYear, nMonth, nDay))
            {
                System.IO.File.Delete(strFile);
                return true;
            }

            return false;
        }

        // dtTarget이 dtNow보다 1달 이전의 시간인가?
        private bool IsPassedTime(DateTime dtNow, int nYear, int nMonth, int nDay)
        {
            if (dtNow.Year - nYear > 1)
                return true;
            else if (dtNow.Year - nYear == 1)
            {
                if (dtNow.Month < 12)
                    return true;
                else if (nMonth > 1)
                    return true;
                else if (dtNow.Day < nDay)
                    return true;
                else
                    return false;
            }
            else if (dtNow.Year > nYear)
                return false;

            if (dtNow.Month - nMonth > 1)
                return true;
            else if (dtNow.Month >= nMonth)
                return false;

            return dtNow.Day < nDay;
        }

        private void WriteLog(object str)
        {
            if (ConnectionLogEx.Instance.IsOpened)
                ConnectionLogEx.Instance.Write(str);
        }

        private void WriteLineLog(object str)
        {
            if (ConnectionLogEx.Instance.IsOpened)
                ConnectionLogEx.Instance.WriteLine(str);
        }

        private void InitLog()
        {
            ConnectionLogEx.MakeInstance();
        }

        public bool SendMessage(int header, byte[] messages)
        {
            if (m_postBox == null || m_isConnected == false)
            {
                m_isConnected = false;
            }
            else
            {
                SendLog(header, messages);

                bool closeConnection;
                bool result = m_postBox.SendMessage(header, messages, out closeConnection);

                if (closeConnection)
                {
                    WriteLog(m_postBox.ErrorMessage);
                    m_isConnected = false;
                }
                else
                    m_dtLastSendMessage = DateTime.Now;

                return result;
            }

            return false;
        }

        public void OnMessage(int header, byte[] messages)
        {
            if (FormMain.Instance.Closing)
                return;

            ArrayList arrDatas = messages == null ? null : SOPWebServer.BinaryHelper.ReadBytes(messages);

            if (header == SOPWebServer.Header.CLOSE_CONNECTION)
            {
                m_isConnected = false;
            }
            else if (header == SOPWebServer.Header.ACCEPT_LOGIN)
            {
                ProcessAcceptLogin(arrDatas);
            }
            else if (header == SOPWebServer.Header.REJECT_LOGIN)
            {
                ProcessRejectLogin(arrDatas);
            }
            else if (header == SOPWebServer.Header.CHECK_LOGIN)
            {
                ProcessCheckLogin();
            }
            else if (header == SOPWebServer.Header.LOGOUT_USER)
            {
                ProcessLogoutUser();
            }
            else if (header == SOPWebServer.Header.JOIN_USER)
            {
                ProcessJoinUser(arrDatas);
            }
            else if (header == SOPWebServer.Header.CHANGE_SOPGENUSER_COMMANDER)
            {
                ProcessChangeSOPGenUserCommander(arrDatas);
            }
            else if (header == SOPWebServer.Header.CHANGE_PASSWORD || header == SOPWebServer.Header.SET_PASSWORD)
            {
                ProcessChangePassword(arrDatas);
            }
            else if (header == SOPWebServer.Header.CHANGE_NICKNAME)
            {
                ProcessChangeNickName(arrDatas);
            }
            else if (header == SOPWebServer.Header.END_RESTORE)
            {
                LoginManager.Instance.OnEndRestore();
            }
            else if (header == SOPWebServer.Header.SERVER_COMMAND)
            {
                ProcessServerCommand(arrDatas);
            }
            else if (header == SOPWebServer.Header.INTERNAL_MESSAGE)
            {
                // SOP Server가 다른 곳에서 전송된 InternalMessage를 대신 전달해 주는 경우
                ProcessInternalMessage(arrDatas, messages);
            }

            RecvLog(header, messages);
        }

        private void ProcessInternalMessage(ArrayList arrDatas, byte[] bytes)
        {
            if (arrDatas != null && arrDatas.Count >= 1 && arrDatas[0] is byte)
            {
                byte msg = (byte)arrDatas[0];

                if (msg == InternalMessage.SDMS_2_SOP_SIMULATOR)
                    FormMain.Instance.NetworkServer.ServiceProvider.SendDataToOther(arrDatas, null, false, ClientData.ClientType.SOP_SIMULATOR);
                else if (msg == InternalMessage.SOP_SIMULATOR_2_SDMS)
                    FormMain.Instance.NetworkServer.ServiceProvider.SendDataToOther(arrDatas, null, false, ClientData.ClientType.SDMS_CLIENT);
            }
        }

        private void ProcessServerCommand(ArrayList arrDatas)
        {
            if (arrDatas != null && arrDatas.Count >= 1 && arrDatas[0] is byte)
            {
                int nCommand = (byte)arrDatas[0];

                if (nCommand == SOPWebServer.ServerCommandType.RUN_SDMS)
                    FormMain.Instance.ExecuteManager.Run(ExecuteManager.APP_TYPE.SDMS);
                else if (nCommand == SOPWebServer.ServerCommandType.UPDATE_SYSTEM)
                    FormMain.Instance.CheckNUpdateSystem(null, true);
            }
        }

        private void ProcessChangeNickName(ArrayList arrDatas)
        {
            if (arrDatas != null && arrDatas.Count >= 1 && arrDatas[0] is int && arrDatas[1] is string)
            {
                int nSuccess = (int)arrDatas[0];
                string strNickName = (string)arrDatas[1];
                LoginManager.Instance.OnChangeNickName(nSuccess, strNickName);
            }
        }

        private void ProcessChangePassword(ArrayList arrDatas)
        {
            if (arrDatas != null && arrDatas.Count >= 1 && arrDatas[0] is int)
            {
                int nSuccess = (int)arrDatas[0];
                LoginManager.Instance.OnChangePassword(nSuccess);
            }
        }

        private void ProcessChangeSOPGenUserCommander(ArrayList arrDatas)
        {
            if (arrDatas != null && arrDatas.Count >= 1 && arrDatas[0] is int)
            {
                int nErrorMessage = (int)arrDatas[0];
                LoginManager.Instance.OnChangeSOPGenUserCommander((LoginManager.CommanderErrorType)nErrorMessage);
            }
        }

        private void ProcessJoinUser(ArrayList arrDatas)
        {
            if (arrDatas != null && arrDatas.Count >= 1 && arrDatas[0] is int)
            {
                int nSOPGenUserID = (int)arrDatas[0];
                LoginManager.Instance.OnJoinUser(nSOPGenUserID);
            }
        }

        private void ProcessLogoutUser()
        {
            LoginManager.Instance.OnLogout();
        }

        private void ProcessCheckLogin()
        {
            LoginManager.Instance.OnCheckLogin();
        }

        private void ProcessRejectLogin(ArrayList arrDatas)
        {
            if (arrDatas != null && arrDatas.Count >= 1 && arrDatas[0] is int)
            {
                int nErrorMessage = (int)arrDatas[0];

                if (nErrorMessage == SOPWebServer.ErrorMessageType.INVALID_ID_OR_PASSWORD)
                    nErrorMessage = 1;
                else if (nErrorMessage == SOPWebServer.ErrorMessageType.ALREADY_USING_ID)
                    nErrorMessage = 2;
                else
                    nErrorMessage = 3;

                LoginManager.Instance.OnRejectLogin(nErrorMessage);
            }
        }

        private void ProcessAcceptLogin(ArrayList arrDatas)
        {
            if (arrDatas != null && arrDatas.Count >= 3 && arrDatas[0] is int && arrDatas[1] is string && arrDatas[2] is string)
            {
                int nSOPGenUserID = (int)arrDatas[0];
                string strUserName = (string)arrDatas[1];
                string strNickName = (string)arrDatas[2];

                LoginManager.Instance.OnAcceptLogin(nSOPGenUserID, strUserName, strNickName);
            }
        }

        private void RecvLog(int header, byte[] bytes)
        {
            MessageLog(header, bytes, "RecvMessage");
        }

        private void SendLog(int header, byte[] bytes)
        {
            MessageLog(header, bytes, "SendMessage");
        }

        private void MessageLog(int header, byte[] bytes, string strMessageTag)
        {
            if (!ConnectionLogEx.Instance.IsOpened)
                return;

            if (header != SOPWebServer.Header.ARE_YOU_THERE)
            {
                string strLog = "";

                if (bytes == null)
                {
                    strLog = string.Format(strMessageTag + " : Header({0}), Length(0)", header);
                }
                else
                {
                    strLog = string.Format(strMessageTag + " : Header({0}), Length({1})", header, bytes.Length);
                    string strBytes = "";

                    foreach (byte b in bytes)
                    {
                        if (strBytes.Length == 0)
                            strBytes = string.Format("\r\n\t\t{0:X2}", (int)b);
                        else
                            strBytes += string.Format(" {0:X2}", (int)b);
                    }

                    strLog += strBytes;
                }
                
                WriteLineLog(strLog);
            }
        }

        public bool CheckLogin(string szID)
        {
            if (m_isConnected == false)
                return false;

            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(szID);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            return SendMessage(SOPWebServer.Header.CHECK_LOGIN, bytes);
        }

        public bool Logout(string szID)
        {
            if (m_isConnected == false)
                return false;

            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(szID);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            return SendMessage(SOPWebServer.Header.LOGOUT_USER, bytes);
        }

        public bool LoginUser(string szID, string szPass)
        {
            if (m_isConnected == false)
                return false;

            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(szID);
            arrDatas.Add(szPass);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            return SendMessage(SOPWebServer.Header.LOGIN_USER, bytes);
        }

        public bool RegisterUser(int nMemberID, string szID, string szPass, string szNickName, IntegratedManagement4.PopupDialog.Chief chief)
        {
            if (m_isConnected == false)
                return false;

            chief.CallerPhoneNumber = chief.CallerPhoneNumber.Replace("-", "");

            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(nMemberID);
            arrDatas.Add(szID);
            arrDatas.Add(szPass);
            arrDatas.Add(szNickName);
            arrDatas.Add(chief.DisplayText);
            arrDatas.Add(chief.CallerPhoneNumber);
            arrDatas.Add((int)chief.SOPTYPE);
            arrDatas.Add(chief.ID);

            int nDayLight = 0;
            if (chief.DayLight_Day == true)
                nDayLight |= 1;
            if (chief.DayLight_Night == true)
                nDayLight |= 2;

            arrDatas.Add(nDayLight);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            return SendMessage(SOPWebServer.Header.JOIN_USER, bytes);
        }

        public bool SetPassword(string szGenUserID, string szNewPass)
        {
            if (m_isConnected == false)
                return false;

            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(szGenUserID);
            arrDatas.Add(szNewPass);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            return SendMessage(SOPWebServer.Header.SET_PASSWORD, bytes);
        }

        public bool ChangePassword(int nGenUserID, string szPass, string szNewPass)
        {
            if (m_isConnected == false)
                return false;

            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(nGenUserID);
            arrDatas.Add(szPass);
            arrDatas.Add(szNewPass);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            return SendMessage(SOPWebServer.Header.CHANGE_PASSWORD, bytes);
        }

        public bool ChangeNickName(int nGenUserID, string szNickName)
        {
            if (m_isConnected == false)
                return false;

            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(nGenUserID);
            arrDatas.Add(szNickName);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            return SendMessage(SOPWebServer.Header.CHANGE_NICKNAME, bytes);
        }

        public bool ChangeSOPGenCommander(int szID, IntegratedManagement4.PopupDialog.Chief pchief)
        {
            if (m_isConnected == false)
                return false;

            int nDayLight = 0;
            if (pchief.DayLight_Day == true)
                nDayLight |= 1;
            if (pchief.DayLight_Night == true)
                nDayLight |= 2;

            pchief.CallerPhoneNumber = pchief.CallerPhoneNumber.Replace("-", "");

            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(szID);
            arrDatas.Add(pchief.DisplayText);
            arrDatas.Add(pchief.CallerPhoneNumber);
            arrDatas.Add((int)pchief.SOPTYPE);
            arrDatas.Add(pchief.ID);
            arrDatas.Add(nDayLight);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            return SendMessage(SOPWebServer.Header.CHANGE_SOPGENUSER_COMMANDER, bytes);
        }
    }

    public class ConnectionLogEx : TcpLib2.ConnectionLog
    {
        private log4net.ILog logger = null;
        private static ConnectionLogEx m_instance2 = new ConnectionLogEx();

        public static ConnectionLogEx Instance
        {
            get { return m_instance2; }
        }

        public static bool MakeInstance()
        {
            /*if (m_instance == null)
				m_instance = new ConnectionLogEx();

			ConnectionLogEx instance = (ConnectionLogEx)m_instance;*/
            m_instance2.logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            m_instance2.m_isOpened = true;
            return m_instance2.m_isOpened;
        }

        public override bool Write(object obj, bool writeTime = true)
        {
            if (obj.GetType() == typeof(Exception))
            {
                Exception e = (Exception)obj;
                if (logger != null)
                    logger.Debug(e.Message, e);
            }
            else
            {
                if (logger != null)
                    logger.DebugFormat("{0}", obj.ToString());
            }
            return true;
        }

        public override bool WriteLine(object obj, bool writeTime = true)
        {
            if (obj.GetType() == typeof(Exception))
            {
                Exception e = (Exception)obj;
                if (logger != null)
                    logger.Debug(e.Message, e);
            }
            else
            {
                if (logger != null)
                    logger.Debug(obj.ToString());
            }
            return true;
        }
    }
}
