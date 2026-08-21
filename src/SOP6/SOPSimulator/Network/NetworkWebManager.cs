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

        private log4net.ILog logger = null;
        private bool m_lockConnection = false;

        private static NetworkWebManager m_instance = null;

        private List<SensorDetectSignal> m_arrDetectSignals = new List<SensorDetectSignal>();
        // 이미 처리된 알람인지 확인하기 위한 데이터
        private Dictionary<int, int> m_dicSensorZoneHistories = new Dictionary<int, int>();

        public ClientProviderInternal ClientProviderInternal
        {
            get { return m_providerInternal; }
        }

        public static NetworkWebManager Instance
        {
            get { return m_instance; }
        }

        public NetworkWebManager(WebDBManager dbMgr)
        {
            m_instance = this;
            m_dbMgr = dbMgr;
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
                m_postBox.PostMan = this;
                m_postBox.Port = nPort;

                m_nPort = nPort;
            }
        }

        private void ConnectionThread()
        {
            while (m_shutdownThread == false)
            {
                if (m_isConnected == false)
                {
                    int nPort = ReadServerPort();

                    if (m_nPort != nPort)
                        SetPostBox(nPort);

                    if (m_postBox != null && m_lockConnection == false)
                    {
                        if (m_postBox.Connect(SOPWebServer.ClientType.SOP_SIMULATOR, SOPWebServer.ClientSubType.SOP_SIMULATOR))
                        {
                            SetConnection(true);
                        }
                    }
                }
                else
                {
                    // 서버에게 제어권을 소유한 클라이언트가 계속 접속이 유지되고 있음을 알려준다.
                    if (FormSOP.Instance.HasControl)
                        SendMessage(SOPWebServer.Header.CONFIRM_HAS_CONTROL, null);
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

                Thread.Sleep(500);
            }
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
            if (FormSOP.Instance.CloseThread)
                return;

            ArrayList arrDatas = messages == null ? null : SOPWebServer.BinaryHelper.ReadBytes(messages);

            RecvLog(header, messages);

            if (header == SOPWebServer.Header.CLOSE_CONNECTION)
            {
                SetConnection(false);
            }
            else if (header == SOPWebServer.Header.SENSOR_SIGNAL_FOR_SOP)
                ProcessSensorSignal(arrDatas);
            else if (header == SOPWebServer.Header.REQUEST_CLIENT_INFO)
                ProcessRequestClientInfo();
            else if (header == SOPWebServer.Header.GIVE_CONTROL)
                ProcessGiveControl();
            else if (header == SOPWebServer.Header.REJECT_REQUEST_CONTROL)
                ProcessRejectRequestControl();
            else if (header == SOPWebServer.Header.CANCEL_REQUEST_CONTROL)
                ProcessCancelRequestControl(arrDatas);
            else if (header == SOPWebServer.Header.REQUEST_CONTROL)
                ProcessRequestControl(arrDatas);
            else if (header == SOPWebServer.Header.CONTROL_CLIENT)
                ProcessControlClient(arrDatas);
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
                ProcessEarthquakeSensorDetect(arrDatas);
            else if (header == SOPWebServer.Header.ALARM_DATA_LIST)
                ProcessAlarmDataList(arrDatas);
        }

        private void ProcessAlarmDataList(ArrayList arrDatas)
        {
            if (arrDatas == null)
            {
                System.Diagnostics.Trace.WriteLine("ProcessAlarmDataList arr null");
                return;
            }

            if (FormSOP.Instance.HasControl == false)
            {
                System.Diagnostics.Trace.WriteLine("ProcessAlarmDataList HasControl false");
                return;
            }

            int nDataCount = arrDatas.Count;
            int nLogChunkSize = 6;

            // Key : SensorZoneHistory ID
            Dictionary<int, StubWorker.SOPProcessType> dicSOPProcessTypes = new Dictionary<int, StubWorker.SOPProcessType>();

            for (int i = 0; i < nDataCount - (nLogChunkSize - 1); i += nLogChunkSize)
            {
                if (arrDatas[i] is int && arrDatas[i + 1] is int && arrDatas[i + 2] is int &&
                    arrDatas[i + 3] is long && arrDatas[i + 4] is int && arrDatas[i + 5] is int)
                {
                    int nSensorType = (int)arrDatas[i];
                    int nEquipZoneID = (int)arrDatas[i + 1];
                    int nZoneID = (int)arrDatas[i + 2];
                    long nTime = (long)arrDatas[i + 3];
                    int nSensorZoneID = (int)arrDatas[i + 4];
                    int nSensorZoneHistoryID = (int)arrDatas[i + 5];

                    // 이미 처리된 알람 신호인가?
                    if (m_dicSensorZoneHistories.ContainsKey(nSensorZoneHistoryID))
                        return;
                    else
                        m_dicSensorZoneHistories[nSensorZoneHistoryID] = nSensorZoneHistoryID;

                    IFacility.FacilityType type = IFacility.ToFacilityType(nSensorType);
                    DateTime timeStamp = DateTime.FromBinary(nTime);
                    StubWorker.SOPProcessType processType = StubWorker.SOPProcessType.None;

                    if (IFacility.IsFireSensorType(type))
                        processType = StubWorker.Instance.OpenSOP_Fire(nZoneID, timeStamp, nSensorZoneID, nSensorZoneHistoryID);
                    else if (IFacility.IsPSMSensorType(type))
                        processType = StubWorker.Instance.OpenSOP_PSM(nEquipZoneID, timeStamp, nSensorZoneID, nSensorZoneHistoryID);
                    else if (IFacility.IsSecurityType(type))
                        processType = StubWorker.Instance.OpenSOP_Security(nEquipZoneID, timeStamp, nSensorZoneID, nSensorZoneHistoryID, nSensorType);

                    if (processType != StubWorker.SOPProcessType.None)
                    {
                        dicSOPProcessTypes[nSensorZoneHistoryID] = processType;
                        System.Diagnostics.Trace.WriteLine("ProcessAlarmDataList ProcessSOP : " + processType.ToString());
                    }
                    else
                        System.Diagnostics.Trace.WriteLine("ProcessAlarmDataList ProcessSOP : None");
                }
                else
                    System.Diagnostics.Trace.WriteLine("ProcessAlarmDataList InvalidData");
            }

            SendAlarmSOPResult(dicSOPProcessTypes);
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

        private void ProcessEarthquakeSensorDetect(ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;

            if (nDataCount < 8)
                return;

            if (arrDatas[0] is int && arrDatas[1] is float && arrDatas[2] is int && arrDatas[3] is string && arrDatas[4] is long && arrDatas[5] is bool && arrDatas[6] is bool && arrDatas[7] is string)
            {
                int nSensorID = (int)arrDatas[0];
                float fMagnitude = (float)arrDatas[1];
                int nIntensity = (int)arrDatas[2];
                //int nAlarmLevel = (int)arrDatas[3];
                string strPosition = (string)arrDatas[3];
                VariousData<DateTime> time = (long)arrDatas[4] == 0 ? null : new VariousData<DateTime>(DateTime.FromBinary((long)arrDatas[4]));
                bool isReal = (bool)arrDatas[5];
                bool runSOP = (bool)arrDatas[6];
                string strSOPPath = (string)arrDatas[7];

                StubWorker.Instance.OpenSOP_Earthquake(strSOPPath, time == null ? DateTime.Now : time.Data, -1, nIntensity, fMagnitude, strPosition);

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
                    }
                }
                catch (Exception e)
                {
                    WriteLog("ProcessChangedConfig Error : " + e.StackTrace);
                }
            }
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
                    if (FormSOP.Instance.HasControl == false)
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
            if (arrDatas != null && arrDatas.Count >= 3 && arrDatas[0] is string && arrDatas[1] is string && arrDatas[2] is string)
            {
                string strUserID = (string)arrDatas[0];
                string strUserNickName = (string)arrDatas[1];
                string strIP = (string)arrDatas[2];

                FormSOP.Instance.Invoke((MethodInvoker)delegate
                {
                    FormSOP.Instance.HideRequestControl(strUserID);
                });
            }
        }

        private void ProcessGiveControl()
        {
            FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                FormSOP.Instance.CloseRequestProgress();
                FormSOP.Instance.SetControl(true);
                SupervisorSOPClose.SupervisorSOPObtainControlAuthority();
            });
        }

        private void ProcessRejectRequestControl()
        {
            FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                FormSOP.Instance.CloseRequestProgress();
            });
        }

        private void ProcessRequestControl(ArrayList arrDatas)
        {
            if (arrDatas != null && arrDatas.Count >= 3)
            {
                if (arrDatas[0] is string && arrDatas[1] is string && arrDatas[2] is string)
                {
                    string strUserID = (string)arrDatas[0];
                    string strNickName = (string)arrDatas[1];
                    string strIP = (string)arrDatas[2];

                    FormSOP.Instance.Invoke((MethodInvoker)delegate
                    {
                        FormSOP.Instance.ShowRequestControl(strUserID, strNickName, strIP);
                    });
                }
            }
        }

        private void ProcessControlClient(ArrayList arrDatas)
        {
            if (arrDatas != null && arrDatas.Count > 0 && arrDatas[0] is int)
            {
                int nControlUserID = (int)arrDatas[0];
                bool hasControl = ProxySOP.Instance.SOPGenUserID == nControlUserID;

                if (FormSOP.Instance.HasControl != hasControl)
                {
                    FormSOP.Instance.Invoke((MethodInvoker)delegate
                    {
                        FormSOP.Instance.SetControl(hasControl);

                        if (hasControl)
                            SupervisorSOPClose.SupervisorSOPObtainControlAuthority();
                    });
                }
            }
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

        private void ProcessSensorSignal(ArrayList arrDatas)
        {
            if (arrDatas != null && arrDatas.Count >= 8)
            {
                if (arrDatas[0] is int && arrDatas[1] is int && arrDatas[2] is int && arrDatas[3] is long && arrDatas [4] is float && arrDatas[5] is float && arrDatas[6] is float && arrDatas[7] is int)
                {
                    int nOriginSensorID = (int)arrDatas[0];
                    int nSensorZoneHistoryID = (int)arrDatas[1];
                    int nEquipZoneID = (int)arrDatas[2];
                    DateTime detectTime = DateTime.FromBinary((long)arrDatas[3]);
                    float x = (float)arrDatas[4];
                    float y = (float)arrDatas[5];
                    float z = (float)arrDatas[6];
                    bool isVirtualMode = (int)arrDatas[7] == 1;

                    if (nSensorZoneHistoryID < 0)
                    {
                        // 수동신고일 경우
                        return;
                    }

                    SensorDetectSignal signal = FindDetectSignal(nSensorZoneHistoryID);

                    if (signal != null)
                        return;

                    signal = new SensorDetectSignal(nOriginSensorID, nSensorZoneHistoryID, nEquipZoneID, detectTime, x, y, z);
                    signal.RealMode = !isVirtualMode;
                    AddDetectSignal(signal);
                }
            }
        }

        public bool SendMessage(int header, byte[] messages)
        {
            if (m_postBox == null || m_isConnected == false)
            {
                SetConnection(false);
            }
            else
            {
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
        public void SendControl(List<string> userIDs, string strUserIP)
        {
            int nUserIDCount = userIDs.Count;

            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(nUserIDCount);

            foreach (string strUserID in userIDs)
            {
                arrDatas.Add(strUserID);
            }

            arrDatas.Add(strUserIP);

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

        public void SendRejectRequestControl(string strUserID, string strUserIP)
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(strUserID);
            arrDatas.Add(strUserIP);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            SendMessage(SOPWebServer.Header.REJECT_REQUEST_CONTROL, bytes);
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
                if (FormSOP.Instance.HasControl)
                {
                    // 서버와의 접속이 끊어지면 즉시 제어권을 상실한다.
                    FormSOP.Instance.Invoke((MethodInvoker)delegate
                    {
                        FormSOP.Instance.SetControl(false);
                    });
                }
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
}
