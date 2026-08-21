using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Threading;
using System.Collections;
using SOPWebClient;
using SDMS;
using UnE.SOP;
using UnE.Sensor;
using System.Windows.Forms;
using System.Collections.Concurrent;
using libSOPPolicy;

namespace SOPMonitoringSystem
{
    public class NetworkWebManager : IPostMan, UnE.SOP.SMS.ISMSOwner
    {
        private ClientProviderInternal m_providerInternal = null;

        private PostBox m_postBox = null;
        private WebDBManager m_dbMgr = null;
        private bool m_shutdownThread = false;
        private bool m_isConnected = false;
        private int m_nPort = -1;
        private DateTime m_dtLastSendMessage = new DateTime();

        private string m_strTryLoginID = null;
        private string m_strTryLoginPW = null;
        private int m_nClientType = 0;
        private int m_nClientSubType = 0;

        private log4net.ILog logger = null;
        private bool m_lockConnection = false;
        private bool m_processMessage = false;

        private static NetworkWebManager m_instance = null;

        private List<SensorDetectSignal> m_arrDetectSignals = new List<SensorDetectSignal>();
        // 이미 처리된 알람인지 확인하기 위한 데이터
        // Key : SensorZoneHistory ID
        private Dictionary<int, SensorAlarmData> m_dicSensorZoneHistories = new Dictionary<int, SensorAlarmData>();
        private Data.EtcSensorManager m_etcSensorManager = null;

        public ClientProviderInternal ClientProviderInternal
        {
            get { return m_providerInternal; }
        }

        public static NetworkWebManager Instance
        {
            get { return m_instance; }
        }

        public bool IsConnected
        {
            get { return m_isConnected; }
        }

        public NetworkWebManager(WebDBManager dbMgr, int nClientType = SOPWebServer.ClientType.SOP_SIMULATOR, int nClientSubType = SOPWebServer.ClientSubType.SOP_SIMULATOR)
        {
            m_instance = this;
            m_dbMgr = dbMgr;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            m_nClientType = nClientType;
            m_nClientSubType = nClientSubType;

            m_etcSensorManager = new Data.EtcSensorManager(m_dbMgr);

            int nPort = ReadServerPort();
            SetPostBox(nPort);

            m_providerInternal = new ClientProviderInternal(this);

            // UI에서 모든 준비가 끝나면 접속 시도하도록 한다.
            LockConnection();

            Thread t = new Thread(new ThreadStart(ConnectionThread));
            t.Start();

            // 시간이 경과한 로그 삭제
            t = new Thread(DeleteLog);
            t.Name = "LogDelete";
            t.Start();

            if (UnE.SOP.SMS.SMSManager.Instance != null)
                UnE.SOP.SMS.SMSManager.Instance.SMSOwner = this;
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
                m_postBox.Port = nPort;

                m_postBox.PostMan = this;

                m_nPort = nPort;
            }
        }

        private void ConnectionThread()
        {
            List<int> controlActionStepHistoryIDs = new List<int>();

            while (m_shutdownThread == false)
            {
                if (m_isConnected == false)
                {
                    int nPort = ReadServerPort();

                    if (m_nPort != nPort)
                        SetPostBox(nPort);

                    if (m_postBox != null && m_lockConnection == false)
                    {
                        if (m_postBox.Connect(m_nClientType, m_nClientSubType))
                        {
                            SetConnection(true);
                        }
                    }
                }
                else
                {
                    /*if (FormSOP.Instance.HasControl)
                        SendMessage(SOPWebServer.Header.CONFIRM_HAS_CONTROL, null);*/
                    // 서버에게 제어권을 소유한 클라이언트가 계속 접속이 유지되고 있음을 알려준다.
                    if (FormSOP.Instance != null && FormSOP.Instance.GetControlActionStepHistoryIDs(controlActionStepHistoryIDs) > 0)
                        SendConfirmHasControl(controlActionStepHistoryIDs);
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

                    if (m_strTryLoginID != null && m_strTryLoginPW != null)
                    {
                        LoginUser(m_strTryLoginID, m_strTryLoginPW);
                        m_strTryLoginID = m_strTryLoginPW = null;
                    }
                }

                Thread.Sleep(500);

                if (m_providerInternal.IsConnected)
                {
                    if (m_providerInternal.PingCount > 5)
                    {
                        m_providerInternal.PingCount = 0;

                        try
                        {
                            WriteLog("[Internal] PING COUNT EXCEPTION");
                            m_providerInternal.Close();
                        }
                        catch (System.Exception)
                        {

                        }

                    }
                    // IsReadingProcess가 true이면 OnReceive에서 받은 데이터를 처리중이므로 다른 Data를 수신할 수 없는 상태임
                    else if (m_providerInternal.IsReadingProcess)
                        m_providerInternal.SendData(TCP_ID.I_AM_HERE);
                    else
                        m_providerInternal.PingCount++;
                }

                if (!m_providerInternal.IsConnected)
                {
                    if (FormSOP.Instance != null)
                    {
                        int nPort = IntegratedManagement4.InternalMessage.GetInternalServerPort(FormSOP.Instance.DBManager, FormSOP.Instance.DBManager.SiteID);

                        try
                        {
                            if (nPort > 0)
                            {
                                m_providerInternal.Connect("127.0.0.1", nPort);
                            }
                        }
                        catch (System.Exception)
                        {

                        }
                    }
                }

                Thread.Sleep(500);
            }
        }

        private void SendConfirmHasControl(List<int> controlActionStepHistoryIDs)
        {
            ArrayList arrDatas = new ArrayList();

            foreach (int nActionStepHistoryID in controlActionStepHistoryIDs)
            {
                arrDatas.Add(nActionStepHistoryID);
            }

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            SendMessage(SOPWebServer.Header.CONFIRM_HAS_CONTROL, bytes);
        }

        public void ReleaseThread()
        {
            m_shutdownThread = true;
        }

        // dtTarget이 dtNow보다 1달 이전의 시간인가?
        private bool IsPassedTime(DateTime dtNow, int nYear, int nMonth, int nDay)
        {
            DateTime dtLog = new DateTime(nYear, nMonth, nDay);
            TimeSpan span = dtNow - dtLog;
            return span.TotalDays > 30.0;
        }

        // 1달이 경과한 통신로그 삭제
        private void DeleteLog()
        {
            try
            {
                string strPath = System.Windows.Forms.Application.ExecutablePath;
                string szParentPath = System.IO.Path.GetDirectoryName(strPath);

                string[] arrFiles = System.IO.Directory.GetFiles(szParentPath + "\\logs");

                string strKey = "SOPSimulator.log-";
                int len = strKey.Length;

                DateTime dtNow = DateTime.Now;
                int nYear, nMonth, nDay;

                foreach (string strFile in arrFiles)
                {
                    int nIndex = strFile.IndexOf(strKey);

                    if (nIndex < 0)
                        continue;

                    string strDate = strFile.Substring(nIndex + len);

                    int nIndex1 = strDate.IndexOf('-');
                    int nIndex2 = strDate.LastIndexOf('-');

                    if (nIndex1 < 0 || nIndex2 < 0 || nIndex1 == nIndex2)
                        continue;

                    string strYear = strDate.Substring(0, nIndex1);
                    string strMonth = strDate.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
                    string strDay = strDate.Substring(nIndex2 + 1);

                    if (!int.TryParse(strYear, out nYear))
                        continue;
                    if (!int.TryParse(strMonth, out nMonth))
                        continue;
                    if (!int.TryParse(strDay, out nDay))
                        continue;

                    if (IsPassedTime(dtNow, nYear, nMonth, nDay))
                        System.IO.File.Delete(strFile);
                }
            }
            catch (System.IO.DirectoryNotFoundException)
            {
            }
        }

        public void OnMessage(int header, byte[] messages)
        {
            // 메인폼이 종료되면 더이상 외부 메시지는 받아들이지 않는다.
            if (FormSOP.Instance != null && FormSOP.Instance.CloseThread)
                return;

            if (m_processMessage)
                return;

            m_processMessage = true;
            ArrayList arrDatas = messages == null ? null : SOPWebServer.BinaryHelper.ReadBytes(messages);

            RecvLog(header, messages);

            if (header == SOPWebServer.Header.CLOSE_CONNECTION)
            {
                SetConnection(false);
            }
            else if (header == SOPWebServer.Header.SENSOR_SIGNAL_FOR_SOP)
            {
                // 실제 재난상황으로 전파되었음
                ProcessSensorSignal(arrDatas);
            }
            else if (header == SOPWebServer.Header.REQUEST_CLIENT_INFO)
                ProcessRequestClientInfo();
            //else if (header == SOPWebServer.Header.GIVE_CONTROL)
            //    ProcessGiveControl();
            else if (header == SOPWebServer.Header.REJECT_REQUEST_CONTROL)
                ProcessRejectRequestControl();
            else if (header == SOPWebServer.Header.CANCEL_REQUEST_CONTROL)
                ProcessCancelRequestControl(arrDatas);
            else if (header == SOPWebServer.Header.REQUEST_CONTROL)
                ProcessRequestControl(arrDatas);
            else if (header == SOPWebServer.Header.CONTROL_CLIENT)
            {
                ProcessControlClient(arrDatas);
            }
            else if (header == SOPWebServer.Header.CLEAR_DETECT_REPORT)
                ProcessClearDetect(arrDatas);
            else if (header == SOPWebServer.Header.SOP_SELECT_MISSION)
                ProcessSelectMission(arrDatas);
            else if (header == SOPWebServer.Header.CHANGE_CONFIG)
                ProcessChangedConfig(arrDatas);
            else if (header == SOPWebServer.Header.CHANGE_WORK_MEMBER)
                NeedToUpdateWorkingMemberData();
            else if (header == SOPWebServer.Header.SOP_SIMULATOR_COMMAND)
                ProcessSimulatorCommand(arrDatas);
            else if (header == SOPWebServer.Header.EARTHQUAKE_SENSOR_DETECT)
            {
                ProcessEarthquakeSensorDetect(arrDatas);
            }
            //else if (header == SOPWebServer.Header.ETC_SENSOR_DETECT)
            //{
            //    m_etcSensorManager.ProcessEtcSensorDetect(arrDatas);
            //}
            else if (header == SOPWebServer.Header.ALARM_DATA_LIST)
            {
                ProcessAlarmDataList(arrDatas);
            }
            else if (header == SOPWebServer.Header.ALARM_DATA_LIST2)
            {
                ProcessAlarmDataList2(arrDatas);
            }
            else if (header == SOPWebServer.Header.RESPONSE_COMPONENT_HISTORY_LIST)
                ProcessComponentHistoryDataList(arrDatas);
            else if (header == SOPWebServer.Header.RUN_SOP)
                ProcessRunSOP(arrDatas);
            else if (header == SOPWebServer.Header.SELECT_SOP_COMPONENT)
                ProcessSelectSOPComponent(arrDatas);
            else if (header == SOPWebServer.Header.ACCEPT_LOGIN)
            {
                ProcessAcceptLogin(arrDatas);
            }
            else if (header == SOPWebServer.Header.REJECT_LOGIN)
            {
                ProcessRejectLogin(arrDatas);
            }

            m_processMessage = false;
        }

        private void ProcessRejectLogin(ArrayList arrDatas)
        {
            if (arrDatas != null && arrDatas.Count >= 1 && arrDatas[0] is int)
            {
                int nErrorMessage = (int)arrDatas[0];
                string strErrorMessage = "";

                if (nErrorMessage == SOPWebServer.ErrorMessageType.INVALID_ID_OR_PASSWORD)
                {
                    nErrorMessage = 1;
                    strErrorMessage = "아이디 혹은 비밀번호가 잘못 입력되었습니다.";
                }
                else if (nErrorMessage == SOPWebServer.ErrorMessageType.ALREADY_USING_ID)
                {
                    nErrorMessage = 2;
                    strErrorMessage = "해당 아이디는 이미 로그인 중입니다.";
                }
                else
                {
                    nErrorMessage = 3;
                    strErrorMessage = "로그인에 실패하였습니다.";
                }

                Popup.Login.FormLogin.Instance.ReceiveLoginResult(false, strErrorMessage);
            }
        }

        private void ProcessAcceptLogin(ArrayList arrDatas)
        {
            if (arrDatas != null && arrDatas.Count >= 3 && arrDatas[0] is int && arrDatas[1] is string && arrDatas[2] is string)
            {
                int nSOPGenUserID = (int)arrDatas[0];
                string strUserName = (string)arrDatas[1];
                string strNickName = (string)arrDatas[2];

                Popup.Login.FormLogin.Instance.ReceiveLoginResult(true, nSOPGenUserID.ToString() + "_" + strNickName);
            }
        }

        private void ProcessSelectSOPComponent(ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;

            if (nDataCount == 3 && arrDatas[0] is int && arrDatas[1] is string && arrDatas[2] is int)
            {
                int nActionStepHistoryID = (int)arrDatas[0];
                string strComponentType = (string)arrDatas[1];
                int nComponentID = (int)arrDatas[2];

                SOPScenarioManager.Instance.SelectSOPComponent(nActionStepHistoryID, strComponentType, nComponentID);
            }
        }

        private void ProcessRunSOP(ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;

            for (int i = 0; i < nDataCount - 1;)
            {
                List<string> parameters = new List<string>();

                if (arrDatas[i] is bool && arrDatas[i + 1] is string)
                {
                    bool isRealMode = (bool)arrDatas[i];
                    string strSOPFullPath = (string)arrDatas[i + 1];

                    if (i + 3 < nDataCount)
                    {
                        if (arrDatas[i + 2] is int && arrDatas[i + 3] is int)
                        {
                            int nSOPUserID = (int)arrDatas[i + 2];
                            int nParameterCount = (int)arrDatas[i + 3];

                            if (i + 3 + nParameterCount < nDataCount)
                            {
                                for (int j=i + 4;j<i + 4 + nParameterCount;j++)
                                {
                                    if (arrDatas[j] is string)
                                    {
                                        parameters.Add((string)arrDatas[j]);
                                    }
                                    else
                                        return;
                                }

                                StubWorker.Instance.RunSOP(isRealMode, strSOPFullPath, nSOPUserID, parameters);
                                i += 3 + nParameterCount;
                            }
                            else
                                return;
                        }
                        else
                            return;
                    }
                    else
                        return;
                }
                else
                    return;
            }
        }

        /*private void ProcessResponseSensorSOPPermit(ArrayList arrDatas)
        {
            if (arrDatas == null)
                return;

            int nDataCount = arrDatas.Count;

            if (nDataCount % 2 != 0)
                return;

            Dictionary<int, StubWorker.SOPProcessType> dicSOPProcessTypes = new Dictionary<int, StubWorker.SOPProcessType>();

            for (int i = 0; i < nDataCount; i += 2)
            {
                if (arrDatas[i] is int && arrDatas[i + 1] is int)
                {
                    int nSensorZoneHistoryID = (int)arrDatas[i];
                    int nSOPGenUserID = (int)arrDatas[i + 1];
                    StubWorker.Instance.SensorSOPPermit(dicSOPProcessTypes, nSensorZoneHistoryID, nSOPGenUserID);
                }
            }

            SendAlarmSOPResult(dicSOPProcessTypes);
        }*/

        /*private void ProcessConfirmNewSOP(ArrayList arrDatas)
        {
            if (arrDatas == null)
                return;

            if (arrDatas.Count == 4 && arrDatas[0] is int && arrDatas[1] is bool && arrDatas[2] is int && arrDatas[3] is int)
            {
                int nActionStepID = (int)arrDatas[0];
                bool isRealMode = (bool)arrDatas[1];
                int nSOPGenUserID = (int)arrDatas[2];
                int nActionStepHistoryID = (int)arrDatas[3];

                FormSOP.Instance.ConfirmNewSOP(nActionStepID, isRealMode, nSOPGenUserID, nActionStepHistoryID);
            }
        }*/

        private void ProcessComponentHistoryDataList(ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;

            for (int i = 0; i < nDataCount - 1;)
            {
                if (ReadComponentHistoryData(arrDatas, ref i) == false)
                    return;
            }
        }

        private bool ReadComponentHistoryData(ArrayList arrDatas, ref int nIndex)
        {
            if (arrDatas[nIndex] is int && arrDatas[nIndex + 1] is int)
            {
                int nActionStepHistoryID = (int)arrDatas[nIndex];
                int nComponentHistoryCount = (int)arrDatas[nIndex + 1];
                nIndex += 2;

                Data_ActionStepHistory actionStepHistory = SOPScenarioManager.Instance.GetActionStepHistory(nActionStepHistoryID);
                UnE.SOP.Sections.SectionTabPage tabPage = actionStepHistory == null ? null : FormSOP.Instance.GetPageHome().GetTabPage(actionStepHistory.ID);
                List<Data_ComponentHistory> componentHistories = new List<Data_ComponentHistory>();

                // ComponentContents 생성이 끝난 후에 ComponentHistory를 업데이트 하도록 한다.
                bool updateComponentHistory = tabPage != null && tabPage.FinishComponentContentsLoading;

                string strTask;
                VariousData<int> completeCount, showBoard, checkedNotify1, checkedNotify2, checkedRun, checkedComplete;

                for (int i = 0; i < nComponentHistoryCount; i++)
                {
                    if (arrDatas[nIndex] is int && arrDatas[nIndex + 1] is int &&
                        arrDatas[nIndex + 2] is int && arrDatas[nIndex + 3] is long &&
                        arrDatas[nIndex + 4] is int && arrDatas[nIndex + 8] is int &&
                        arrDatas[nIndex + 13] is int)
                    {
                        int nComponentHistoryID = (int)arrDatas[nIndex];
                        int nComponentID = (int)arrDatas[nIndex + 1];
                        int nComponentType = (int)arrDatas[nIndex + 2];
                        DateTime timeStamp = DateTime.FromBinary((long)arrDatas[nIndex + 3]);
                        int nStatus = (int)arrDatas[nIndex + 4];
                        int nAccessedUserID = (int)arrDatas[nIndex + 8];
                        int nDetailCount = (int)arrDatas[nIndex + 13];

                        if (DBToString(arrDatas[nIndex + 5], out strTask) == false)
                            return false;
                        if (DBToVarious<int>(arrDatas[nIndex + 6], out completeCount) == false)
                            return false;
                        if (DBToVarious<int>(arrDatas[nIndex + 7], out showBoard) == false)
                            return false;
                        if (DBToVarious<int>(arrDatas[nIndex + 9], out checkedNotify1) == false)
                            return false;
                        if (DBToVarious<int>(arrDatas[nIndex + 10], out checkedNotify2) == false)
                            return false;
                        if (DBToVarious<int>(arrDatas[nIndex + 11], out checkedRun) == false)
                            return false;
                        if (DBToVarious<int>(arrDatas[nIndex + 12], out checkedComplete) == false)
                            return false;

                        nIndex += 14;

                        Data_ComponentHistory componentHistory = new Data_ComponentHistory();

                        componentHistory.ID = nComponentHistoryID;
                        componentHistory.ComponentID = nComponentID;
                        componentHistory.ComponentType = nComponentType;
                        componentHistory.TimeStamp = timeStamp;
                        componentHistory.Status = nStatus;
                        componentHistory.Task = strTask;
                        componentHistory.CompleteCount = completeCount;
                        componentHistory.ShowBoard = showBoard;
                        componentHistory.AccessedUserID = nAccessedUserID;
                        componentHistory.CheckedNotify1 = checkedNotify1;
                        componentHistory.CheckedNotify2 = checkedNotify2;
                        componentHistory.CheckedRun = checkedRun;
                        componentHistory.CheckedComplete = checkedComplete;

                        if (ReadComponentHistoryDetailData(arrDatas, nDetailCount, componentHistory.DetailDatas, ref nIndex) == false)
                            return false;

                        componentHistories.Add(componentHistory);

                        if (actionStepHistory != null && actionStepHistory.MaxComponentHistoryIDFromServer < componentHistory.ID && updateComponentHistory)
                        {
                            actionStepHistory.MaxComponentHistoryIDFromServer = componentHistory.ID;
                        }
                    }
                    else
                        return false;
                }

                if (componentHistories.Count > 0)
                    SOPScenarioManager.Instance.AddNewComponentHistories(nActionStepHistoryID, componentHistories);
            }
            else
                return false;

            return true;
        }

        private bool ReadComponentHistoryDetailData(ArrayList arrDatas, int nDetailCount, List<Data_ComponentHistoryDetail> detailDatas, ref int nIndex)
        {
            VariousData<int> datai;
            VariousData<float> dataf;
            string datas;
            VariousData<DateTime> timeStamp;

            for (int i = 0; i < nDetailCount; i++)
            {
                if (nIndex + 5 <= arrDatas.Count)
                {
                    if (arrDatas[nIndex] is int)
                    {
                        int nDataIndex = (int)arrDatas[nIndex];

                        if (DBToVarious<int>(arrDatas[nIndex + 1], out datai) == false)
                            return false;
                        if (DBToVarious<float>(arrDatas[nIndex + 2], out dataf) == false)
                            return false;
                        if (DBToString(arrDatas[nIndex + 3], out datas) == false)
                            return false;
                        if (DBToDateTime(arrDatas[nIndex + 4], out timeStamp) == false)
                            return false;

                        Data_ComponentHistoryDetail detail = new Data_ComponentHistoryDetail();

                        detail.DataIndex = nDataIndex;
                        detail.Datai = datai;
                        detail.Dataf = dataf;
                        detail.Datas = datas;
                        detail.TimeStamp = timeStamp;

                        detailDatas.Add(detail);
                    }
                    else
                        return false;

                    nIndex += 5;
                }
                else
                    return false;
            }

            return true;
        }

        private void ProcessAlarmDataList2(ArrayList arrDatas)
        {
            if (arrDatas == null)
            {
                System.Diagnostics.Trace.WriteLine("ProcessAlarmDataList2 arr null");
                return;
            }

            int nDataCount = arrDatas.Count;
            int nLogChunkSize = 9;

            // Key : SensorZoneHistory ID
            Dictionary<int, StubWorker.SOPProcessType> dicSOPProcessTypes = new Dictionary<int, StubWorker.SOPProcessType>();

            for (int i = 0; i < nDataCount - (nLogChunkSize - 1); i += nLogChunkSize)
            {
                if (arrDatas[i] is int && arrDatas[i + 1] is int && arrDatas[i + 2] is int &&
                    arrDatas[i + 3] is long && arrDatas[i + 4] is int && arrDatas[i + 5] is int &&
                    arrDatas[i + 6] is int && arrDatas[i + 7] is string && arrDatas[i + 8] is string)
                {
                    int nSensorType = (int)arrDatas[i];
                    int nEquipZoneID = (int)arrDatas[i + 1];
                    int nZoneID = (int)arrDatas[i + 2];
                    long nTime = (long)arrDatas[i + 3];
                    int nSensorZoneID = (int)arrDatas[i + 4];
                    int nSensorZoneHistoryID = (int)arrDatas[i + 5];
                    int nSOPGenUserID = (int)arrDatas[i + 6];
                    string strSOPFullPath = (string)arrDatas[i + 7];
                    string strAlarmMessage = (string)arrDatas[i + 8];

                    SensorAlarmData sensorAlarm;

                    // 이미 처리된 알람 신호인가?
                    if (m_dicSensorZoneHistories.TryGetValue(nSensorZoneHistoryID, out sensorAlarm))
                    {
                        sensorAlarm.SOPGenUserID = nSOPGenUserID;

                        if (sensorAlarm.Page != null && FormSOP.Instance.SOPUser != null && FormSOP.Instance.SOPUser.ID == nSOPGenUserID)
                        {
                            // Run SOP
                            System.Diagnostics.Trace.WriteLine("Run SOP from SensorZoneHistory(" + nSensorZoneHistoryID + ")");
                        }
                        else
                            return;
                    }
                    else
                        m_dicSensorZoneHistories[nSensorZoneHistoryID] = new SensorAlarmData(nSensorType, nEquipZoneID, nZoneID, nTime, nSensorZoneID, nSensorZoneHistoryID, nSOPGenUserID);

                    IFacility.FacilityType type = IFacility.ToFacilityType(nSensorType);
                    DateTime timeStamp = DateTime.FromBinary(nTime);
                    StubWorker.SOPProcessType processType = StubWorker.SOPProcessType.None;

                    if (IFacility.IsFireSensorType(type))
                        processType = StubWorker.Instance.OpenSOP_Fire(nZoneID, timeStamp, nSensorZoneID, nSensorZoneHistoryID, nSOPGenUserID, strSOPFullPath, strAlarmMessage);
                    else if (IFacility.IsPSMSensorType(type))
                    {
                        if (nSensorZoneID >= SOPWebServer.Header.ManualReportDefaultID)
                            processType = StubWorker.Instance.OpenSOP_ETC(nZoneID, nEquipZoneID, timeStamp, nSensorZoneID, nSensorZoneHistoryID, nSensorType, nSOPGenUserID, strSOPFullPath, strAlarmMessage);
                        else
                            processType = StubWorker.Instance.OpenSOP_PSM(nEquipZoneID, timeStamp, nSensorZoneID, nSensorZoneHistoryID, nSOPGenUserID, strSOPFullPath, strAlarmMessage);
                    }
                    else if (IFacility.IsSecurityType(type))
                        processType = StubWorker.Instance.OpenSOP_Security(nEquipZoneID, timeStamp, nSensorZoneID, nSensorZoneHistoryID, nSensorType, nSOPGenUserID, strSOPFullPath, strAlarmMessage);
                    else if (IFacility.IsETCSensorType(type))
                        processType = StubWorker.Instance.OpenSOP_ETC(nZoneID, nEquipZoneID, timeStamp, nSensorZoneID, nSensorZoneHistoryID, nSensorType, nSOPGenUserID, strSOPFullPath, strAlarmMessage);
                    else if (IFacility.IsEarthquakeSensorType(type))
                    {
                        /*int nIntensity;
                        float fMagnitude;
                        int nActionStepIndex;
                        string strSOPFullPath = ReadEarthquakeSOP(nSensorZoneHistoryID, out nIntensity, out fMagnitude, out nActionStepIndex);

                        if (strSOPFullPath == null)
                            processType = StubWorker.SOPProcessType.Igonore;
                        else
                        {
                            if (nActionStepIndex >= 0)
                                strSOPFullPath += "/" + UnE.SOP.Sections.SectionTabControl.StandardActionStepNames[nActionStepIndex];

                            processType = StubWorker.Instance.OpenSOP_Earthquake(strSOPFullPath, timeStamp, nSensorZoneID, nSensorZoneHistoryID, nIntensity, fMagnitude, "", nSOPGenUserID);
                        }*/
                    }

                    if (processType != StubWorker.SOPProcessType.None)
                    {
                        dicSOPProcessTypes[nSensorZoneHistoryID] = processType;
                        System.Diagnostics.Trace.WriteLine("ProcessAlarmDataList ProcessSOP : " + processType.ToString());
                    }
                    else
                    {
                        // ProcessType이 None이면 다음에 데이터를 다시 처리할 수 있도록 한다.
                        m_dicSensorZoneHistories.Remove(nSensorZoneHistoryID);
                        System.Diagnostics.Trace.WriteLine("ProcessAlarmDataList ProcessSOP : None");
                    }
                }
                else
                    System.Diagnostics.Trace.WriteLine("ProcessAlarmDataList InvalidData");
            }

            SendAlarmSOPResult(dicSOPProcessTypes);
        }

        private void ProcessAlarmDataList(ArrayList arrDatas)
        {
            if (arrDatas == null)
            {
                System.Diagnostics.Trace.WriteLine("ProcessAlarmDataList arr null");
                return;
            }

            // 센서에 의한 알람이 발생하면 SOPWebServer는 하나의 SOPSimulator Client에게만 Sensor 신호를 보낸다.
            // 그 신호를 받은 Client는 SOP를 실행한다.
            /*if (FormSOP.Instance.HasControl == false)
            {
                System.Diagnostics.Trace.WriteLine("ProcessAlarmDataList HasControl false");
                return;
            }*/

            int nDataCount = arrDatas.Count;
            int nLogChunkSize = 7;

            // Key : SensorZoneHistory ID
            Dictionary<int, StubWorker.SOPProcessType> dicSOPProcessTypes = new Dictionary<int, StubWorker.SOPProcessType>();

            for (int i = 0; i < nDataCount - (nLogChunkSize - 1); i += nLogChunkSize)
            {
                if (arrDatas[i] is int && arrDatas[i + 1] is int && arrDatas[i + 2] is int &&
                    arrDatas[i + 3] is long && arrDatas[i + 4] is int && arrDatas[i + 5] is int &&
                    arrDatas[i + 6] is int)
                {
                    int nSensorType = (int)arrDatas[i];
                    int nEquipZoneID = (int)arrDatas[i + 1];
                    int nZoneID = (int)arrDatas[i + 2];
                    long nTime = (long)arrDatas[i + 3];
                    int nSensorZoneID = (int)arrDatas[i + 4];
                    int nSensorZoneHistoryID = (int)arrDatas[i + 5];
                    int nSOPGenUserID = (int)arrDatas[i + 6];

                    SensorAlarmData sensorAlarm;

                    // 이미 처리된 알람 신호인가?
                    if (m_dicSensorZoneHistories.TryGetValue(nSensorZoneHistoryID, out sensorAlarm))
                    {
                        sensorAlarm.SOPGenUserID = nSOPGenUserID;

                        if (sensorAlarm.Page != null && FormSOP.Instance.SOPUser != null && FormSOP.Instance.SOPUser.ID == nSOPGenUserID)
                        {
                            // Run SOP
                            System.Diagnostics.Trace.WriteLine("Run SOP from SensorZoneHistory(" + nSensorZoneHistoryID + ")");
                        }
                        else
                            return;
                    }
                    else
                        m_dicSensorZoneHistories[nSensorZoneHistoryID] = new SensorAlarmData(nSensorType, nEquipZoneID, nZoneID, nTime, nSensorZoneID, nSensorZoneHistoryID, nSOPGenUserID);

                    IFacility.FacilityType type = IFacility.ToFacilityType(nSensorType);
                    DateTime timeStamp = DateTime.FromBinary(nTime);
                    StubWorker.SOPProcessType processType = StubWorker.SOPProcessType.None;

                    if (IFacility.IsFireSensorType(type))
                        processType = StubWorker.Instance.OpenSOP_Fire(nZoneID, timeStamp, nSensorZoneID, nSensorZoneHistoryID, nSOPGenUserID);
                    else if (IFacility.IsPSMSensorType(type))
                        processType = StubWorker.Instance.OpenSOP_PSM(nEquipZoneID, timeStamp, nSensorZoneID, nSensorZoneHistoryID, nSOPGenUserID);
                    else if (IFacility.IsSecurityType(type))
                        processType = StubWorker.Instance.OpenSOP_Security(nEquipZoneID, timeStamp, nSensorZoneID, nSensorZoneHistoryID, nSensorType, nSOPGenUserID);
                    else if (IFacility.IsETCSensorType(type))
                        processType = StubWorker.Instance.OpenSOP_ETC(nZoneID, nEquipZoneID, timeStamp, nSensorZoneID, nSensorZoneHistoryID, nSensorType, nSOPGenUserID);
                    else if (IFacility.IsEarthquakeSensorType(type))
                    {
                        int nIntensity;
                        float fMagnitude;
                        int nActionStepIndex;
                        string strSOPFullPath = ReadEarthquakeSOP(nSensorZoneHistoryID, out nIntensity, out fMagnitude, out nActionStepIndex);

                        if (strSOPFullPath == null)
                            processType = StubWorker.SOPProcessType.Igonore;
                        else
                        {
                            if (nActionStepIndex >= 0)
                                strSOPFullPath += "/" + UnE.SOP.Sections.SectionTabControl.StandardActionStepNames[nActionStepIndex];

                            processType = StubWorker.Instance.OpenSOP_Earthquake(strSOPFullPath, timeStamp, nSensorZoneID, nSensorZoneHistoryID, nIntensity, fMagnitude, "", nSOPGenUserID);
                        }
                    }

                    if (processType != StubWorker.SOPProcessType.None)
                    {
                        dicSOPProcessTypes[nSensorZoneHistoryID] = processType;
                        System.Diagnostics.Trace.WriteLine("ProcessAlarmDataList ProcessSOP : " + processType.ToString());
                    }
                    else
                    {
                        // ProcessType이 None이면 다음에 데이터를 다시 처리할 수 있도록 한다.
                        m_dicSensorZoneHistories.Remove(nSensorZoneHistoryID);
                        System.Diagnostics.Trace.WriteLine("ProcessAlarmDataList ProcessSOP : None");
                    }
                }
                else
                    System.Diagnostics.Trace.WriteLine("ProcessAlarmDataList InvalidData");
            }

            SendAlarmSOPResult(dicSOPProcessTypes);
        }

        private string ReadEarthquakeSOP(int nSensorZoneHistoryID, out int nIntensity, out float fMagnitude, out int nActionStepIndex)
        {
            nIntensity = -1;
            fMagnitude = -1.0f;
            nActionStepIndex = -1;

            string strSQL = string.Format("Select Param4 from SensorReactionHistory where SensorHistoryID = {0} and ReactionType = {1}", nSensorZoneHistoryID, (int)libSensorProcess.ReactionType.BEGIN_STATUS);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return null;

            string strValue = WebDBManager.GetStringField(arrResult[0]);

            if (strValue == null)
                return null;

            double value;

            if (ReadLastDouble(strValue, out value) == false || value < 0.0)
                return null;

            if (strValue.Contains("진도"))
                nIntensity = (int)value;
            else if (strValue.Contains("규모"))
                fMagnitude = (float)value;
            else
                return null;

            List<UnE.Earthquake.EarthquakeOption> options = LoadEarthquakeOptions();

            if (options == null)
                return null;

            UnE.Earthquake.EarthquakeOption option = UnE.Earthquake.EarthquakeOption.GetOption(nIntensity, fMagnitude, options);

            if (option == null/* || option.RunSOP == false*/)
                return null;

            string strSOPFullPath = GetEarthquakeSOPPath();

            nActionStepIndex = options.IndexOf(option);
            return strSOPFullPath;
            //return option.LinkedSOP;
        }

        private string GetEarthquakeSOPPath()
        {
            string strSQL = "Select SOPName from ETCSensorSOPLink where Type = 50";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return "";

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount;i++)
            {
                string strSOPPath = WebDBManager.GetStringField(arrResult[i]);

                if (strSOPPath == null)
                    continue;

                return strSOPPath;
            }

            return "";
        }

        private List<UnE.Earthquake.EarthquakeOption> LoadEarthquakeOptions()
        {
            string strSQL = "Select MinIntens, MaxIntens, IntensOption, UseSMS, SMSMessage, UseBroadcast, BroadcastMessage from OptionEarthquake";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            List<UnE.Earthquake.EarthquakeOption> options = new List<UnE.Earthquake.EarthquakeOption>();
            int nResultData = arrResult.Count;

            for (int i = 0; i < nResultData - 6; i += 7)
            {
                VariousData<float> min = WebDBManager.GetFloatField(arrResult[i].ToString());
                VariousData<float> max = WebDBManager.GetFloatField(arrResult[i + 1].ToString());
                VariousData<int> option = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<int> useSMS = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                string strSMS = WebDBManager.GetStringField(arrResult[i + 4]);
                VariousData<int> useBroadcast = WebDBManager.GetIntField(arrResult[i + 5].ToString());
                string strBroadcast = WebDBManager.GetStringField(arrResult[i + 6]);
                //VariousData<int> runSOP = WebDBManager.GetIntField(arrResult[i + 7].ToString());
                //string strLinkedSOP = WebDBManager.GetStringField(arrResult[i + 8]);

                if (min == null || max == null || option == null || useSMS == null || useBroadcast == null/* || runSOP == null*/)
                    continue;

                UnE.Earthquake.EarthquakeOption opt = new UnE.Earthquake.EarthquakeOption();
                opt.Minimum = min.Data;
                opt.Maximum = max.Data;
                opt.SetMinMaxOption(option.Data);
                opt.UseSMS = useSMS.Data == 1 ? true : false;
                opt.SMSMessage = strSMS == null ? "" : strSMS;
                opt.UseBroadcast = useBroadcast.Data == 1 ? true : false;
                opt.BroadcastMessage = strBroadcast == null ? "" : strBroadcast;
                //opt.RunSOP = runSOP.Data == 1 ? true : false;
                //opt.LinkedSOP = strLinkedSOP == null ? "" : strLinkedSOP;

                options.Add(opt);
            }

            options.Sort();
            return options;
        }

        private bool ReadLastDouble(string str, out double num)
        {
            int len = str.Length;
            num = 0;

            bool begin = false, readDot = false;
            int count = 0;

            for (int i = len - 1; i >= 0; i--)
            {
                char ch = str.ElementAt(i);

                if (begin == false)
                {
                    if (ch >= '0' && ch <= '9')
                    {
                        num = ch - '0';
                        count = 1;
                        begin = true;
                    }
                    else if (ch == '.')
                    {
                        readDot = true;
                        begin = true;
                    }
                }
                else
                {
                    if (ch >= '0' && ch <= '9')
                    {
                        num = num + (ch - '0') * System.Math.Pow(10, count);
                        count++;
                    }
                    else if (ch == '.')
                    {
                        if (readDot)
                            break;
                        else
                        {
                            num = num * System.Math.Pow(10, -count);
                            readDot = true;
                            count = 0;
                        }
                    }
                    else
                        break;
                }
            }

            return count > 0 || readDot;
        }

        // Key : SensorZoneHistory ID
        private void SendAlarmSOPResult(Dictionary<int, StubWorker.SOPProcessType> dicSOPProcessTypes)
        {
            if (dicSOPProcessTypes.Count == 0)
                return;

            ArrayList arrDatas = new ArrayList();

            foreach (KeyValuePair<int, StubWorker.SOPProcessType> pair in dicSOPProcessTypes)
            {
                arrDatas.Add(pair.Key);
                arrDatas.Add((int)pair.Value);
            }

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            SendMessage(SOPWebServer.Header.ALARM_SOP_RESULT, bytes);
        }

        /*private void ProcessEtcSensorDetect(ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;

            if (nDataCount < 8)
                return;

            if (arrDatas[0] is int && arrDatas[1] is int && arrDatas[2] is int && arrDatas[3] is long && arrDatas[5] is int && arrDatas[6] is bool && arrDatas[7] is string)
            {
                int nSensorType = (int)arrDatas[0];
                int nSensorZoneID = (int)arrDatas[1];
                int nSensorZoneHistoryID = (int)arrDatas[2];
                DateTime timeStamp = DateTime.FromBinary((long)arrDatas[3]);
                int nAlarmLevel = (int)arrDatas[5];
                bool runSOP = (bool)arrDatas[6];
                string strSOPPath = (string)arrDatas[7];

                IFacility.ToFacilityType(nSensorType)

                StubWorker.Instance.OpenSOP_Etc(strSOPPath, time == null ? DateTime.Now : time.Data, -1, nIntensity, fMagnitude, strPosition);
            }
        }*/

        private void ProcessEarthquakeSensorDetect(ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;

            if (nDataCount < 9)
                return;

            if (arrDatas[0] is int && arrDatas[1] is float && arrDatas[2] is int && arrDatas[3] is string && arrDatas[4] is long && arrDatas[5] is bool && arrDatas[6] is int && arrDatas[7] is bool && arrDatas[8] is string)
            {
                int nSensorID = (int)arrDatas[0];
                float fMagnitude = (float)arrDatas[1];
                int nIntensity = (int)arrDatas[2];
                //int nAlarmLevel = (int)arrDatas[3];
                string strPosition = (string)arrDatas[3];
                VariousData<DateTime> time = (long)arrDatas[4] == 0 ? null : new VariousData<DateTime>(DateTime.FromBinary((long)arrDatas[4]));
                bool isReal = (bool)arrDatas[5];
                int nSensorZoneHistoryID = (int)arrDatas[6];
                bool runSOP = (bool)arrDatas[7];
                string strSOPPath = (string)arrDatas[8];

                StubWorker.Instance.OpenSOP_Earthquake(strSOPPath, time == null ? DateTime.Now : time.Data, nSensorID, nSensorZoneHistoryID, nIntensity, fMagnitude, strPosition, -1);

                // SDMS 지진 이벤트를 발생시킨다.
                /*ProxyMessenger.Instance.EarthquakeEvent(nIntensity, fMagnitude, strPosition, false);
                //SDMS.ScriptProxy.Instance.UserObject.SDMSEarthquakeEvent.Invoke(nIntensity, fMagnitude, strPosition, false);

                

                // SDMS 지진 이벤트가 끝나기를 기다린다.
                do
                {
                    ProxyMessenger.Instance.Ask_EarthquakeEventIsFinished();
                    System.Threading.Thread.Sleep(100);
                }
                while (ProxyMessenger.Instance.EarthquakeEventIsFinished == false);
                //while (SDMS.ScriptProxy.Instance.UserObject.SDMSEarthquakeEventIsFinished() == false)
                //{
                //    System.Threading.Thread.Sleep(100);
                //}

                int nActionStepID = GetEarthquakeLinkedActionStepID();

                if (nActionStepID < 0)
                    return;

                //if (CheckPrevEarthquakeSOP(fMagnitude, nIntensity, nActionStepID) == false)
                //{
                //    // 기존에 더 큰 지진세기로 진행중인 SOP가 존재한다.
                //    return;
                //}

                if (FormSOP.Instance.HasControl == false)
                    return;

                FormSOP.Instance.Invoke((MethodInvoker)delegate
                {
                    TreeNode node = SOPScenarioManager.Instance.GetBarLevelTree().FindActionStepNode(nActionStepID);

                    if (node == null)
                        return;

                    UnE.SOP.Workstate.WorkflowOptionEarthquake option = new UnE.SOP.Workstate.WorkflowOptionEarthquake();

                    if (nIntensity >= 0)
                    {
                        option.Intensity = nIntensity;
                        option.Mode = UnE.SOP.Workstate.WorkflowOptionEarthquake.PowerMode.Intensity;
                    }

                    if (fMagnitude >= 0.0f)
                    {
                        option.Magnitude = fMagnitude;
                        option.Mode = UnE.SOP.Workstate.WorkflowOptionEarthquake.PowerMode.Magnitude;
                    }

                    if (strPosition.Length > 0)
                    {
                        option.PositionName = strPosition;
                        option.HasPosition = true;
                    }

                    if (time != null)
                        option.DetectTime = time;
                    else
                        option.DetectTime = new DBUtility.VariousData<DateTime>(DateTime.Now);

                    SOPScenarioManager.Instance.GetBarLevelTree().SelectNode(node);
                    FormSOP.Instance.RunWorkflow(option);
                });*/
            }
        }

        private void ProcessSimulatorCommand(ArrayList arrDatas)
        {
            if (arrDatas.Count == 0 || (arrDatas[0] is byte) == false)
                return;

            byte command = (byte)arrDatas[0];

            if (command == SOPSimulatorCommandType.RESET_USER_DEFINED_TEAM_NAMES)
            {
                if (arrDatas.Count == 2 && (arrDatas[1] is int))
                {
                    int nActionStepHistoryID = (int)arrDatas[1];
                    FormSOP.Instance.GetPageHome().SOPTeamMemberManager.ResetUserDefinedTeamNames(nActionStepHistoryID);
                }
            }
        }

        private void NeedToUpdateWorkingMemberData()
        {
            FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                FormSOP.Instance.NeedToUpdateWorkingMemberData();
            });
        }

        private void ProcessChangedConfig(ArrayList arrDatas)
        {
            if (arrDatas == null)
                return;

            if (arrDatas.Count >= 3 && arrDatas[0] is int && arrDatas[1] is string && arrDatas[2] is string)
            {
                try
                {
                    int nClientType = (int)arrDatas[0];
                    string strPropertyName = (string)arrDatas[1];
                    string strPropertyValue = (string)arrDatas[2];

                    if (nClientType == SOPWebServer.ClientType.SDMS && strPropertyName == SOP.SDMSConfig.PropertyName)
                    {
                        int nConfigValue;

                        if (int.TryParse(strPropertyValue, out nConfigValue))
                        {
                            if (((nConfigValue & (int)SOP.SDMSConfig.ConfigType.COMPANY_MEMBER) == (int)SOP.SDMSConfig.ConfigType.COMPANY_MEMBER) ||
                                ((nConfigValue & (int)SOP.SDMSConfig.ConfigType.REGULAR_TEAM) == (int)SOP.SDMSConfig.ConfigType.REGULAR_TEAM) ||
                                ((nConfigValue & (int)SOP.SDMSConfig.ConfigType.TEMPORARY_MEMBER) == (int)SOP.SDMSConfig.ConfigType.TEMPORARY_MEMBER) ||
                                ((nConfigValue & (int)SOP.SDMSConfig.ConfigType.TEMPARARY_NORMAL_TEAM) == (int)SOP.SDMSConfig.ConfigType.TEMPARARY_NORMAL_TEAM) ||
                                ((nConfigValue & (int)SOP.SDMSConfig.ConfigType.TEMPARAY_EMERGENCY_TEAM) == (int)SOP.SDMSConfig.ConfigType.TEMPARAY_EMERGENCY_TEAM) ||
                                ((nConfigValue & (int)SOP.SDMSConfig.ConfigType.EXTERNAL_MEMBER) == (int)SOP.SDMSConfig.ConfigType.EXTERNAL_MEMBER) ||
                                ((nConfigValue & (int)SOP.SDMSConfig.ConfigType.EXTERNAL_TEAM) == (int)SOP.SDMSConfig.ConfigType.EXTERNAL_TEAM))
                                ProcessChangeCompanyMember();

                            if (((nConfigValue & (int)SOP.SDMSConfig.ConfigType.EXTERNAL_MEMBER) == (int)SOP.SDMSConfig.ConfigType.EXTERNAL_MEMBER) ||
                                ((nConfigValue & (int)SOP.SDMSConfig.ConfigType.EXTERNAL_TEAM) == (int)SOP.SDMSConfig.ConfigType.EXTERNAL_TEAM))
                                ProcessChangeExternalMember();
                        }
                    }
                    else if (nClientType == SOPWebServer.ClientType.SOP_SIMULATOR)
                    {
                        if (strPropertyName == SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.USE_BROADCAST))
                        {
                            int nValue;

                            if (int.TryParse(strPropertyValue, out nValue))
                            {
                                FormSOP.Instance.UseBroadcast = nValue == 0 ? false : true;
                            }
                        }
                        else if (strPropertyName == SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.USE_SMS))
                        {
                            int nValue;

                            if (int.TryParse(strPropertyValue, out nValue))
                            {
                                FormSOP.Instance.SMSOn = nValue == 0 ? false : true;
                            }
                        }
                        else if (strPropertyName == SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.SMS_TO_EXTERNAL_MEMBER))
                        {
                            int nValue;

                            if (int.TryParse(strPropertyValue, out nValue))
                            {
                                FormSOP.Instance.SmsExternalCompanyMemberOn = nValue == 0 ? false : true;
                            }
                        }
                        else if (strPropertyName == SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.WORKING_BEGIN_HOUR))
                        {
                            int nHour, nMinute;

                            if (ParseTime(strPropertyValue, out nHour, out nMinute))
                            {
                                PageBackstageOption pageOption = FormSOP.Instance.GetPageOption();
                                pageOption.BeginHour = nHour;
                                pageOption.BeginMinute = nMinute;
                            }
                        }
                        else if (strPropertyName == SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.WORKING_END_HOUR))
                        {
                            int nHour, nMinute;

                            if (ParseTime(strPropertyValue, out nHour, out nMinute))
                            {
                                PageBackstageOption pageOption = FormSOP.Instance.GetPageOption();
                                pageOption.EndHour = nHour;
                                pageOption.EndMinute = nMinute;
                            }
                        }
                        else if (strPropertyName == SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.RUN_SOP_ON_LOADED))
                        {
                            ProcessRunSOPOnLoaded(strPropertyValue);
                        }
                        else if (strPropertyName == SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.SOP_AUTO_CLOSE))
                        {
                            ProcessSOPAutoClose(strPropertyValue);
                        }
                        else if (strPropertyName == SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.USE_CONFIRMSENDSMS))
                        {
                            ProcessUseConfirmSendSMS(strPropertyValue);
                        }
                        else if (strPropertyName == SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.USE_VIRTUALMODE_IN_SENSOR))
                        {
                            ProcessUseVirtualModeInSensor(strPropertyValue);
                        }
                    }
                }
                catch (Exception e)
                {
                    WriteLog("ProcessChangedConfig Error : " + e.StackTrace);
                }
            }
        }

        private void ProcessUseConfirmSendSMS(string strValue)
        {
            int data;

            if (int.TryParse(strValue.Trim(), out data) == false)
                return;

            UnE.SOP.ProxySOP.Instance.ConfirmSendSMS = data == 1;
        }

        private void ProcessUseVirtualModeInSensor(string strValue)
        {
            int data;

            if (int.TryParse(strValue.Trim(), out data) == false)
                return;

            FormSOP.Instance.VirtualModeInSensor = data == 1;
        }

        private void ProcessSOPAutoClose(string strValue)
        {
            string[] tokens = strValue.Split(';');

            if (tokens.Count() != 7)
                return;

            int nCategoryID;

            if (int.TryParse(tokens[0].Trim(), out nCategoryID) == false)
                return;

            string strCategoryName = tokens[1].Trim();
            FormSOP.Instance.GetPageOption().SetAutoCloseDB(nCategoryID, strCategoryName, tokens[2].Trim(), tokens[3].Trim(), tokens[4].Trim(), tokens[5].Trim(), tokens[6].Trim());
        }

        private void ProcessRunSOPOnLoaded(string strValue)
        {
            FormSOP.Instance.GetPageOption().SetSOPOnSensorDetect(strValue);
            ProxyMessenger.Instance.OpenSopOnSensorDetect(ProxySOP.Instance.OpenSOPOnSensorDetect);
        }

        private bool ParseTime(string strValue, out int nHour, out int nMinute)
        {
            nHour = nMinute = 0;
            int nIndex = strValue.IndexOf(':');

            if (nIndex <= 0 || nIndex == strValue.Length - 1)
                return false;

            string strHour = strValue.Substring(0, nIndex);
            string strMinute = strValue.Substring(nIndex + 1);

            if (!int.TryParse(strHour, out nHour))
                return false;

            if (!int.TryParse(strMinute, out nMinute))
                return false;

            return true;
        }

        private void ProcessChangeCompanyMember()
        {
            FormSOP.Instance.SOPManager.LoadRegularMember();
        }

        private void ProcessChangeExternalMember()
        {
            FormSOP.Instance.SOPManager.LoadExternalCompany();
        }

        private void ProcessSelectMission(ArrayList arrDatas)
        {
            if (arrDatas != null && arrDatas.Count >= 4 && arrDatas[0] is int && arrDatas[1] is int && arrDatas[2] is int && arrDatas[3] is int)
            {
                int nActionStepHistory = (int)arrDatas[0];
                int nReal = (int)arrDatas[1];
                int nComponentID = (int)arrDatas[2];
                string strRowIndex = (string)arrDatas[3];

                FormSOP.Instance.Invoke((MethodInvoker)delegate
                {
                    //if (FormSOP.Instance.HasControl == false)
                    {
                        PageBackstageSOP page = FormSOP.Instance.GetPageHome();
                        if (page != null && page.Visible == true)
                        {
                            page.OnCurrentSelectedMission(nActionStepHistory, nReal, nComponentID, strRowIndex);
                        }
                    }
                });
            }            
        }

        private void ProcessClearDetect(ArrayList arrDatas)
        {
            if (arrDatas != null && arrDatas.Count >= 1 && arrDatas[0] is int)
            {
                int nSensorZoneHistoryID = (int)arrDatas[0];
                SensorDetectSignal signal = FindDetectSignal(nSensorZoneHistoryID);

                if (signal != null)
                    RemoveDetectSignal(signal);

                // 현재 진행중인 재난상황에 대하여 SOP List를 팝업시킨다.
                /*SOPMonitoringSystem.Popup.PopupSensorOn popup = SOPMonitoringSystem.Popup.PopupSensorOn.Instance;

                if (popup.Visible == true)
                {
                    if (popup.SensorHistoryID == nSensorZoneHistoryID)
                    {
                        FormSOP.Instance.Invoke((MethodInvoker)delegate
                        {
                            popup.Visible = false;
                            ShowDetectSignal();
                        });
                    }
                }*/
            }
        }

        private void ProcessCancelRequestControl(ArrayList arrDatas)
        {
            if (arrDatas != null && arrDatas.Count >= 2 && arrDatas[0] is int && arrDatas[1] is string)
            {
                int nActionStepHistoryID = (int)arrDatas[0];
                string strUserID = (string)arrDatas[1];

                FormSOP.Instance.Invoke((MethodInvoker)delegate
                {
                    FormSOP.Instance.HideRequestControl(nActionStepHistoryID, strUserID);
                });
            }
        }

        /*private void ProcessGiveControl()
        {
            FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                FormSOP.Instance.CloseRequestProgress();
                FormSOP.Instance.SetControl(true);
                SupervisorSOPClose.SupervisorSOPObtainControlAuthority();
            });
        }*/

        private void ProcessRejectRequestControl()
        {
            FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                FormSOP.Instance.CloseRequestProgress();
            });
        }

        private void ProcessRequestControl(ArrayList arrDatas)
        {
            if (arrDatas != null && arrDatas.Count >= 4)
            {
                if (arrDatas[0] is int && arrDatas[1] is string && arrDatas[2] is string && arrDatas[3] is string)
                {
                    int nActionStepHistoryID = (int)arrDatas[0];
                    string strUserID = (string)arrDatas[1];
                    string strNickName = (string)arrDatas[2];
                    string strIP = (string)arrDatas[3];

                    FormSOP.Instance.Invoke((MethodInvoker)delegate
                    {
                        FormSOP.Instance.ShowRequestControl(nActionStepHistoryID, strUserID, strNickName, strIP);
                    });
                }
            }
        }

        private VariousData<bool> GetActionStepNormalMode(int nActionStepID)
        {
            string strSQL = "select isNormal from ActionStep as step, Disaster as d, Version as v where step.DisasterID = d.ID and d.VersionID = v.ID and step.ID = " + nActionStepID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return null;

            VariousData<int> isNormal = WebDBManager.GetIntField(arrResult[0].ToString());

            if (isNormal == null)
                return null;

            return new VariousData<bool>(isNormal.Data == 1);
        }

        private void ProcessControlClient(ArrayList arrDatas)
        {
            if (arrDatas == null/* || arrDatas.Count < 14*/)
                return;

            int nDataCount = arrDatas.Count;
            List<int> runningActionStepHistoryIDs = new List<int>();

            int thisUserID = FormSOP.Instance.SOPUser == null ? -1 : FormSOP.Instance.SOPUser.ID;

            for (int i = 0; i < nDataCount - 13; i += 14)
            {
                if (arrDatas[i] is int && arrDatas[i + 1] is int && arrDatas[i + 2] is int && arrDatas[i + 3] is bool && arrDatas[i + 4] is long && arrDatas[i + 8] is int)
                {
                    int nActionStepHistoryID = (int)arrDatas[i];
                    int nSOPGenUserID = (int)arrDatas[i + 1];
                    int nActionStepID = (int)arrDatas[i + 2];
                    bool isRealMode = (bool)arrDatas[i + 3];
                    DateTime dtBegin = DateTime.FromBinary((long)arrDatas[i + 4]);
                    int nSensorZoneHistoryID = (int)arrDatas[i + 8];

                    if (FormSOP.Instance.UseSOPMonitoring == false && thisUserID != nSOPGenUserID)
                        continue;

                    VariousData<DateTime> endTime, cancelTime, detectTime;
                    string strPosition, strDisasterOption;
                    VariousData<int> selectedComponentID, selectedComponentType, startOption;

                    if (DBToDateTime(arrDatas[i + 5], out endTime) == false ||
                        DBToDateTime(arrDatas[i + 6], out cancelTime) == false ||
                        DBToDateTime(arrDatas[i + 7], out detectTime) == false)
                        continue;

                    if (DBToString(arrDatas[i + 9], out strPosition) == false ||
                        DBToString(arrDatas[i + 13], out strDisasterOption) == false)
                        continue;

                    if (DBToVarious<int>(arrDatas[i + 10], out selectedComponentID) == false ||
                        DBToVarious<int>(arrDatas[i + 11], out selectedComponentType) == false ||
                        DBToVarious<int>(arrDatas[i + 12], out startOption) == false)
                        continue;

                    Data_ActionStepHistory actionStepHistory = SOPScenarioManager.Instance.GetActionStepHistory(nActionStepHistoryID);
                    bool newActionStepHistory = false;

                    if (actionStepHistory == null)
                    {
                        actionStepHistory = new Data_ActionStepHistory();
                        newActionStepHistory = true;

                        VariousData<bool> isNormal = GetActionStepNormalMode(nActionStepID);

                        if (isNormal != null)
                            actionStepHistory.IsNormal = isNormal.Data;
                    }

                    actionStepHistory.ID = nActionStepHistoryID;
                    actionStepHistory.ActionStepID = nActionStepID;
                    actionStepHistory.RealMode = isRealMode;
                    actionStepHistory.BeginTime = dtBegin;
                    actionStepHistory.EndTime = endTime;
                    actionStepHistory.CancelTime = cancelTime;
                    actionStepHistory.SensorZoneHistoryID = nSensorZoneHistoryID;
                    actionStepHistory.DetectTime = detectTime == null ? new DateTime() : detectTime.Data;
                    actionStepHistory.Position = strPosition;

                    if (selectedComponentID != null)
                        actionStepHistory.SelectedSectionID = selectedComponentID.Data;

                    if (selectedComponentType != null)
                        actionStepHistory.SelectedSectionType = selectedComponentType.Data;

                    if (startOption != null)
                        actionStepHistory.StartOption = startOption.Data;

                    HistoryDisasterNoPosition info = new HistoryDisasterNoPosition();
                    info.HistoryActionStepID = nActionStepHistoryID;

                    if (strDisasterOption != null)
                        info.DisasterOptions = strDisasterOption;

                    actionStepHistory.HistoryDisasterNoPositionInfo = info;

                    if (newActionStepHistory)
                    {
                        SOPScenarioManager.Instance.AddActionStepHistory(actionStepHistory);
                    }

                    SOPScenarioManager.Instance.SetSOPControl(nActionStepHistoryID, nSOPGenUserID);
                    FormSOP.Instance.SetSOPControl(nActionStepHistoryID, nSOPGenUserID);
                    runningActionStepHistoryIDs.Add(nActionStepHistoryID);
                }
            }

            FormSOP.Instance.SetRunningActionStepHistoryIDs(runningActionStepHistoryIDs);
            /*for (int i=0;i<nDataCount-1;i+=2)
            {
                if (arrDatas[i] is int && arrDatas[i + 1] is int)
                {
                    int nActionStepHistoryID = (int)arrDatas[i];
                    int nSOPGenUserID = (int)arrDatas[i + 1];

                    FormSOP.Instance.SetSOPControl(nActionStepHistoryID, nSOPGenUserID);
                }
            }*/
        }

        private bool DBToString(object obj, out string str)
        {
            if (obj is int)
                str = null;
            else if (obj is string)
                str = (string)obj;
            else
            {
                str = null;
                return false;
            }

            return true;
        }

        private bool DBToVarious<T>(object obj, out VariousData<T> data)
        {
            if (obj is int && (int)obj < 0)
                data = null;
            else if (obj is T)
                data = new VariousData<T>((T)obj);
            else
            {
                data = null;
                return false;
            }

            return true;
        }

        private bool DBToDateTime(object obj, out VariousData<DateTime> data)
        {
            if (obj is int)
                data = null;
            else if (obj is long)
            {
                data = new VariousData<DateTime>(DateTime.FromBinary((long)obj));
            }
            else
            {
                data = null;
                return false;
            }

            return true;
        }

        private void ProcessRequestClientInfo()
        {
            int nSOPGenUserID = ProxySOP.Instance.SOPGenUserID;
            int nUserLevel = ProxySOP.Instance.SOPUserLevel;

            string strSQL = "Select UserID, NickName from SOPGenUser where ID = " + nSOPGenUserID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count < 2)
                return;

            string strUserID = WebDBManager.GetStringField(arrResult[0]);
            string strNickName = WebDBManager.GetStringField(arrResult[1]);

            if (strUserID == null || strNickName == null)
                return;

            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(nSOPGenUserID);
            arrDatas.Add(strUserID);
            arrDatas.Add(strNickName);
            arrDatas.Add(nUserLevel);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            SendMessage(SOPWebServer.Header.REPLY_CLIENT_INFO, bytes);
        }

        // 실제 재난상황 전파
        private void ProcessSensorSignal(ArrayList arrDatas)
        {
            if (arrDatas != null && arrDatas.Count >= 9)
            {
                if (arrDatas[0] is int && arrDatas[1] is int && arrDatas[2] is int && arrDatas[3] is long && arrDatas [4] is float && arrDatas[5] is float && arrDatas[6] is float && arrDatas[7] is int && arrDatas[8] is int)
                {
                    int nOriginSensorID = (int)arrDatas[0];
                    int nSensorZoneHistoryID = (int)arrDatas[1];
                    int nZoneID = (int)arrDatas[2];
                    DateTime detectTime = DateTime.FromBinary((long)arrDatas[3]);
                    float x = (float)arrDatas[4];
                    float y = (float)arrDatas[5];
                    float z = (float)arrDatas[6];
                    bool isVirtualMode = (int)arrDatas[7] == 1;
                    int nSensorZoneID = (int)arrDatas[8];

                    if (nSensorZoneHistoryID < 0)
                    {
                        // 수동신고일 경우
                        // 다른 방식으로 SOP 처리를 한다.
                        return;
                    }

                    BaseSOPUser sopUser = FormSOP.Instance.SOPUser;

                    int nDisasterID = sopUser.GetReportDisasterID(nSensorZoneID, m_dbMgr.SiteID, m_dbMgr);

                    if (nDisasterID > 0)
                        StubWorker.Instance.OpenReportSOP_Fire(nDisasterID, nZoneID, detectTime, nSensorZoneID, nSensorZoneHistoryID);

                    System.Diagnostics.Trace.WriteLine("ProcessSensorSignal");
                    System.Diagnostics.Trace.WriteLine("OriginSensorID : " + nOriginSensorID);
                    System.Diagnostics.Trace.WriteLine("SensorZoneHistoryID : " + nSensorZoneHistoryID);
                    System.Diagnostics.Trace.WriteLine("ZoneID : " + nZoneID);
                    System.Diagnostics.Trace.WriteLine("DetectTime : " + detectTime.ToLongTimeString());

                    /*SensorDetectSignal signal = FindDetectSignal(nSensorZoneHistoryID);

                    if (signal != null)
                        return;

                    signal = new SensorDetectSignal(nOriginSensorID, nSensorZoneHistoryID, nEquipZoneID, detectTime, x, y, z);
                    signal.RealMode = !isVirtualMode;
                    AddDetectSignal(signal);*/
                }
            }
        }

        private DateTime m_dtPrevSend = new DateTime();
        private ConcurrentDictionary<int, List<byte[]>> m_dicHeaderBytes = new ConcurrentDictionary<int, List<byte[]>>();

        // 최근 1초 이내에 같은 데이터를 보낸적이 있는지 확인하여, 같은 데이터를 서버에게 1초 이내에 다시 보내지 않도록 한다.
        private bool CheckSameData(int header, byte[] messages)
        {
            DateTime dtNow = DateTime.Now;

            if (m_dtPrevSend.Year == dtNow.Year && m_dtPrevSend.Month == dtNow.Month && m_dtPrevSend.Day == dtNow.Day &&
                m_dtPrevSend.Hour == dtNow.Hour && m_dtPrevSend.Minute == dtNow.Minute && m_dtPrevSend.Second == dtNow.Second)
            {
                List<byte[]> bytes;

                if (m_dicHeaderBytes.TryGetValue(header, out bytes))
                {
                    int nMessageCount = messages.Count();

                    foreach (byte[] byteList in bytes)
                    {
                        int nBytesCount = byteList.Count();

                        if (nMessageCount != nBytesCount)
                            continue;

                        bool diff = false;

                        for (int i = 0; i < nMessageCount; i++)
                        {
                            if (byteList[i] != messages[i])
                            {
                                diff = true;
                                break;
                            }
                        }

                        if (diff)
                            continue;
                        else
                            return false;
                    }

                    bytes.Add(messages);
                    return true;
                }
                else
                {
                    bytes = new List<byte[]>();
                    bytes.Add(messages);
                    return true;
                }
            }

            // 시간이 변경되었다.
            ConcurrentDictionary<int, List<byte[]>> dicHeaderBytes = m_dicHeaderBytes;
            m_dicHeaderBytes = new ConcurrentDictionary<int, List<byte[]>>();

            List<byte[]> _bytes = new List<byte[]>();
            _bytes.Add(messages);
            m_dicHeaderBytes[header] = _bytes;

            m_dtPrevSend = dtNow;
            dicHeaderBytes.Clear();
            return true;
        }

        public bool SendMessage(int header, byte[] messages)
        {
            if (m_postBox == null || m_isConnected == false)
            {
                SetConnection(false);
            }
            else
            {
                if (CheckSameData(header, messages) == false)
                    return true;

                SendLog(header, messages);

                bool closeConnection;
                bool result = m_postBox.SendMessage(header, messages, out closeConnection);

                if (closeConnection)
                {
                    WriteLog(m_postBox.ErrorMessage);
                    SetConnection(false);
                }
                else
                    m_dtLastSendMessage = DateTime.Now;

                return result;
            }

            return false;
        }

        public void RecvLog(int header, byte[] bytes)
        {
            MessageLog(header, bytes, "RecvMessage");
        }

        private void SendLog(int header, byte[] bytes)
        {
            MessageLog(header, bytes, "SendMessage");
        }

        private void MessageLog(int header, byte[] bytes, string strMessageTag)
        {
            if (header != SOPWebServer.Header.ARE_YOU_THERE &&
                header != SOPWebServer.Header.I_AM_HERE &&
                header != SOPWebServer.Header.CONTROL_CLIENT &&
                header != SOPWebServer.Header.CONFIRM_HAS_CONTROL)
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

                WriteLog(strLog);
            }
        }

        private void WriteLog(object str)
        {
            if (logger != null)
                logger.Debug(str);
        }

        private void LockConnection()
        {
            m_lockConnection = true;
        }

        public void ReleaseConnection()
        {
            m_lockConnection = false;
        }

        public void AddDetectSignal(SensorDetectSignal signal)
        {
            if (!m_arrDetectSignals.Contains(signal))
                m_arrDetectSignals.Add(signal);
        }

        public void RemoveDetectSignal(SensorDetectSignal signal)
        {
            m_arrDetectSignals.Remove(signal);
            m_dicSensorZoneHistories.Remove(signal.SensorHistoryID);
        }

        public SensorDetectSignal FindDetectSignal(int nSensorHistoryID)
        {
            foreach (SensorDetectSignal signal in m_arrDetectSignals)
            {
                if (signal.SensorHistoryID == nSensorHistoryID)
                    return signal;
            }

            return null;
        }

        public void RemoveSensorHistory(int nSensorHistoryID)
        {
            foreach (SensorDetectSignal signal in m_arrDetectSignals)
            {
                if (signal.SensorHistoryID == nSensorHistoryID)
                {
                    m_arrDetectSignals.Remove(signal);
                    break;
                }
            }
        }

        // userIDs : 첫번째 ID는 제어권을 넘겨받는다.
        //           나머지 ID들은 제어권을 넘겨받지 못한다.
        public void SendControl(List<string> userIDs, string strUserIP, int nActionStepHistoryID)
        {
            int nUserIDCount = userIDs.Count;

            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(nActionStepHistoryID);
            arrDatas.Add(nUserIDCount);

            foreach (string strUserID in userIDs)
            {
                arrDatas.Add(strUserID);
            }

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            SendMessage(SOPWebServer.Header.GIVE_CONTROL, bytes);
        }

        public void SendIgnoreSOP(int nSensorZoneHistoryID)
        {
            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(nSensorZoneHistoryID);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            SendMessage(SOPWebServer.Header.IGNORE_SOP, bytes);
        }

        public void SendSelectMission(int nActionStepHistory, int nRealMode, int nCompHistoryID, string strRowIndex)
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(nActionStepHistory);
            arrDatas.Add(nRealMode);
            arrDatas.Add(nCompHistoryID);
            arrDatas.Add(strRowIndex);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            SendMessage(SOPWebServer.Header.SOP_SELECT_MISSION, bytes);
        }

        public void SendResetUserDefinedTeamNames(int nActionStepHistoryID)
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(SOPSimulatorCommandType.RESET_USER_DEFINED_TEAM_NAMES);
            arrDatas.Add(nActionStepHistoryID);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            SendMessage(SOPWebServer.Header.SOP_SIMULATOR_COMMAND, bytes);
        }

        public void SendChangedConfig(int nClientType, string strPropertyName, string strPropertyValue)
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(nClientType);
            arrDatas.Add(strPropertyName);
            arrDatas.Add(strPropertyValue);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            SendMessage(SOPWebServer.Header.CHANGE_CONFIG, bytes);
        }

        public void SendChangedWorkingMemberData()
        {
            SendMessage(SOPWebServer.Header.CHANGE_WORK_MEMBER, null);
        }

        public void SendRejectRequestControl(List<string> userIDs, int nActionStepHistoryID)
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(nActionStepHistoryID);
            
            foreach (string strUserID in userIDs)
            {
                arrDatas.Add(strUserID);
            }

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            SendMessage(SOPWebServer.Header.REJECT_REQUEST_CONTROL, bytes);
            /*ArrayList arrDatas = new ArrayList();

            arrDatas.Add(strUserID);
            arrDatas.Add(strUserIP);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            SendMessage(SOPWebServer.Header.REJECT_REQUEST_CONTROL, bytes);*/
        }

        public void SendRunSOP(int nSensorHistoryID, int nActionStepHistoryID)
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(nSensorHistoryID);
            arrDatas.Add(nActionStepHistoryID);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            SendMessage(SOPWebServer.Header.RUN_SOP, bytes);
        }

        public void SetConnection(bool isConnected)
        {
            m_isConnected = isConnected;

            if (m_isConnected == false)
            {
                SOPScenarioManager.Instance.ClearSOPControls();
                //if (FormSOP.Instance.HasControl)
                //{
                //    // 서버와의 접속이 끊어지면 즉시 제어권을 상실한다.
                //    FormSOP.Instance.Invoke((MethodInvoker)delegate
                //    {
                //        FormSOP.Instance.SetControl(false);
                //    });
                //}
            }
        }

        public bool SendSMS(ArrayList arrPhoneNumbers, string strSendPhoneNumber, string strMsg)
        {
            if (m_isConnected == false)
                return false;

            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(strSendPhoneNumber);
            arrDatas.Add(strMsg);
            arrDatas.Add(arrPhoneNumbers.Count);
            arrDatas.AddRange(arrPhoneNumbers);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            return SendMessage(SOPWebServer.Header.SEND_SMS, bytes);
        }

        /*public bool SendNewSOP(int nActionStepID, bool isRealMode, int nSOPGenUserID)
        {
            if (m_isConnected == false)
                return false;

            string strSQL = "Select max(ID) from ActionStepHistory";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nMaxActionStepHistoryID = 0;

            if (arrResult.Count > 0)
            {
                VariousData<int> maxID = WebDBManager.GetIntField(arrResult[0].ToString());

                if (maxID != null)
                    nMaxActionStepHistoryID = maxID.Data;
            }

            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(nActionStepID);
            arrDatas.Add(isRealMode);
            arrDatas.Add(nSOPGenUserID);
            arrDatas.Add(nMaxActionStepHistoryID);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            return SendMessage(SOPWebServer.Header.SEND_NEW_SOP, bytes);
        }*/

        public bool SendConfirmSOPControl(int nActionStepHistoryID)
        {
            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(nActionStepHistoryID);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            return SendMessage(SOPWebServer.Header.CONFIRM_SOP_CONTROL, bytes);
        }

        public bool SendQueryComponentHistory()
        {
            List<Data_ActionStepHistory> histories = SOPScenarioManager.Instance.GetRunningActionStepHistories();
            ArrayList arrDatas = new ArrayList();

            foreach (Data_ActionStepHistory history in histories)
            {
                arrDatas.Add(history.ID);
                arrDatas.Add(history.MaxComponentHistoryIDFromServer);
            }

            if (arrDatas.Count > 0)
            {
                byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
                return SendMessage(SOPWebServer.Header.SOP_CURRENT_SELECT_MISSION, bytes);
            }

            return false;
        }

        public SensorAlarmData GetSensorAlarmData(int nSensorZoneHistoryID)
        {
            SensorAlarmData alarm;

            if (m_dicSensorZoneHistories.TryGetValue(nSensorZoneHistoryID, out alarm))
                return alarm;

            return null;
        }

        public bool LoginUser(string szID, string szPass)
        {
            if (m_isConnected == false)
            {
                m_strTryLoginID = szID;
                m_strTryLoginPW = szPass;
                return false;
            }

            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(szID);
            arrDatas.Add(szPass);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            return SendMessage(SOPWebServer.Header.LOGIN_USER, bytes);
        }
    }

    public class SensorDetectSignal
    {
        // SensorZone이 아닌 개별 Sensor의 ID
        private int m_nSensorID = -1;
        private int m_nSensorHistoryID = -1;
        private int m_nEquipZoneID = -1;
        private DateTime m_detectTime;
        private float x = 0.0f;
        private float y = 0.0f;
        private float z = 0.0f;
        private int m_nActionStepHistoryID = -1;
        private int m_nActionStepID = -1;
        private string m_szPositionName = "";
        private bool m_bRealMode = true;

        public bool RealMode
        {
            get { return m_bRealMode; }
            set { m_bRealMode = value; }
        }
        public int SensorID
        {
            get { return m_nSensorID; }
            set { m_nSensorID = value; }
        }

        public int SensorHistoryID
        {
            get { return m_nSensorHistoryID; }
            set { m_nSensorHistoryID = value; }
        }

        public int EquipZoneID
        {
            get { return m_nEquipZoneID; }
            set { m_nEquipZoneID = value; }
        }

        public DateTime DetectTime
        {
            get { return m_detectTime; }
            set { m_detectTime = value; }
        }

        public float X
        {
            get { return x; }
            set { x = value; }
        }

        public float Y
        {
            get { return y; }
            set { y = value; }
        }

        public float Z
        {
            get { return z; }
            set { z = value; }
        }

        public int ActionStepHistoryID
        {
            get { return m_nActionStepHistoryID; }
            set { m_nActionStepHistoryID = value; }
        }

        public int ActionStepID
        {
            get { return m_nActionStepID; }
            set { m_nActionStepID = value; }
        }

        public string PositionName
        {
            get { return m_szPositionName; }
            set { m_szPositionName = value; }
        }

        public SensorDetectSignal()
        {
        }

        public SensorDetectSignal(int nOriginSensorID, int nSensorHistoryID, int nEquipZoneID, DateTime detectTime, float x, float y, float z)
        {
            m_nSensorID = nOriginSensorID;
            m_nSensorHistoryID = nSensorHistoryID;
            m_nEquipZoneID = nEquipZoneID;
            m_detectTime = detectTime;
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    public class SensorAlarmData
    {
        private int m_nSensorType = -1;
        private int m_nEquipZoneID = -1;
        private int m_nZoneID = -1;
        private DateTime m_dtTimeStamp;
        private int m_nSensorZoneID = -1;
        private int m_nSensorZoneHistoryID = -1;
        private int m_nSOPGenUserID = -1;
        private UnE.SOP.Sections.SectionTabPage m_page = null;

        public int SensorType
        {
            get { return m_nSensorType; }
            set { m_nSensorType = value; }
        }

        public int EquipZoneID
        {
            get { return m_nEquipZoneID; }
            set { m_nEquipZoneID = value; }
        }

        public int ZoneID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }

        public DateTime TimeStamp
        {
            get { return m_dtTimeStamp; }
            set { m_dtTimeStamp = value; }
        }

        public int SensorZoneID
        {
            get { return m_nSensorZoneID; }
            set { m_nSensorZoneID = value; }
        }

        public int SensorZoneHistoryID
        {
            get { return m_nSensorZoneHistoryID; }
            set { m_nSensorZoneHistoryID = value; }
        }

        // SOP 제어권을 가진 User의 ID
        public int SOPGenUserID
        {
            get { return m_nSOPGenUserID; }
            set { m_nSOPGenUserID = value; }
        }

        public UnE.SOP.Sections.SectionTabPage Page
        {
            get { return m_page; }
            set { m_page = value; }
        }

        public SensorAlarmData()
        {
        }

        public SensorAlarmData(int nSensorType, int nEquipZoneID, int nZoneID, long detectTime, int nSensorZoneID, int nSensorZoneHistoryID, int nSOPGenUserID)
        {
            m_nSensorType = nSensorType;
            m_nEquipZoneID = nEquipZoneID;
            m_nZoneID = nZoneID;
            m_dtTimeStamp = DateTime.FromBinary(detectTime);
            m_nSensorZoneID = nSensorZoneID;
            m_nSensorZoneHistoryID = nSensorZoneHistoryID;
            m_nSOPGenUserID = nSOPGenUserID;
        }
    }
}
