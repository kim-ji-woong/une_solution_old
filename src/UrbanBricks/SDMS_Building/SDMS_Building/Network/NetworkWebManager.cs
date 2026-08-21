using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility2;
using libSensorProcess;
using SDMS;
using SDMS_Building.Data;
using SDMS_Building.History;
using SOPWebClient;
using UnE.Sensor;
using UnE.Spatial;

namespace SDMS_Building.Network
{
    public class NetworkWebManager : IPostMan
    {
        private PostBox m_postBox = null;
        private WebDBManager m_dbMgr = null;

        private int m_nClientType = SOPWebServer.ClientType.SDMS;
        private int m_nClientSubType = SOPWebServer.ClientSubType.POWER_PLANT;

        private bool m_shutdownThread = false;
        private bool m_isConnected = false;
        public bool IsConnected
        {
            get { return m_isConnected; }
        }

        private byte[] m_arrReceived = null;

        private DateTime m_dtLastSendMessage = new DateTime();

        private ClientProviderInternal m_providerInternal = null;
        public ClientProviderInternal ClientProviderInternal
        {
            get { return m_providerInternal; }
        }

        // 초기화가 완료되기 전까지 SOP Server에는 접속하지 않는다.
        private bool m_waitSOPServer = true;
        public bool WaitForSOPServer
        {
            get { return m_waitSOPServer; }
            set { m_waitSOPServer = false; }
        }

        // 현재 OnReceive()에서 받은 데이터를 처리중인가?
        private bool m_isReadingProcess = false;
        public bool IsReadingProcess
        {
            get { return m_isReadingProcess; }
        }

        private bool m_bIsLogOpened = false;
        public bool IsLogOpened
        {
            get { return m_bIsLogOpened; }
            set { m_bIsLogOpened = value; }
        }

        private int m_nPort = -1;

        private static NetworkWebManager m_manager = null;

        public static NetworkWebManager Instance
        {
            get
            {
                if (m_manager == null)
                    m_manager = new NetworkWebManager();
                return m_manager;
            }
        }

        public NetworkWebManager()
        {
            InitLog();

            m_dbMgr = FormMain.Instance.DBManager;
            m_providerInternal = new ClientProviderInternal(this);
            
            int nPort = GetServerPort();
            SetPostBox(nPort);

            Thread t = new Thread(new ThreadStart(ConnectionThread));
            t.Name = "SDMS.Connection";
            t.Start();

            // 시간이 경과한 로그 삭제
            t = new Thread(DeleteLog);
            t.Name = "SDMS.LogDelete";
            t.Start();
        }

        private int GetServerPort()
        {
            string strSQL = "Select Port from SensorServerPort where Name = '" + SOPWebServer.ServerPort.SOP_WEB_SERVER + "' and SiteID = " + m_dbMgr.SiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            m_nPort = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            return m_nPort;
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

        #region Log
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
            if (ConnectionLogEx.MakeInstance())
                m_bIsLogOpened = true;
            else
                m_bIsLogOpened = false;
        }

        private void WriteSendLog(int header, byte[] bytes)
        {
            if (header == SOPWebServer.Header.ARE_YOU_THERE)
                return;

            int nBytesLength = bytes == null ? 0 : bytes.Length;

            string strLog = string.Format("SendMessage : Header({0}), Length({1})", header, nBytesLength);
            string strBytes = "";

            for (int i = 0; i < nBytesLength; i++)
            {
                byte b = bytes[i];

                if (strBytes.Length == 0)
                    strBytes = string.Format("\r\n\t\t{0:X2}", (int)b);
                else
                    strBytes += string.Format(" {0:X2}", (int)b);
            }

            WriteLineLog(strLog + strBytes);
        }

        public void RecvLog(byte[] bytes, int nLine)
        {
            if (!IsLogOpened)
                return;

            if (bytes[0] != SOPWebServer.Header.ARE_YOU_THERE /*|| !m_exceptPingLog*/)
            {
                int nHeader = BitConverter.ToInt16(bytes, 0);
                string strLog = string.Format("RecvMessage : Header({0}), Length({1}), SDMS({2})", (int)nHeader, (int)bytes.Length, nLine);
                string strBytes = "";

                foreach (byte b in bytes)
                {
                    if (strBytes.Length == 0)
                        strBytes = string.Format("\r\n\t\t{0:X2}", (int)b);
                    else
                        strBytes += string.Format(" {0:X2}", (int)b);
                }

                WriteLineLog(strLog + strBytes);
            }
        }
        
        // 1달이 경과한 통신로그 삭제
        private void DeleteLog()
        {
            try
            {
                string strPath = System.Windows.Forms.Application.ExecutablePath;
                string szParentPath = System.IO.Path.GetDirectoryName(strPath);

                string[] arrFiles = System.IO.Directory.GetFiles(szParentPath + "\\logs");

                string strKey = "SDMSClient.log-";
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

        // dtTarget이 dtNow보다 1달 이전의 시간인가?
        private bool IsPassedTime(DateTime dtNow, int nYear, int nMonth, int nDay)
        {
            DateTime dtLog = new DateTime(nYear, nMonth, nDay);
            TimeSpan span = dtNow - dtLog;
            return span.TotalDays > 30.0;
        }

        #endregion

        private void ConnectionThread()
        {
            DateTime dtPrev = DateTime.Now;

            while (!m_shutdownThread)
            {
                if (m_waitSOPServer == false)
                {
                    if (m_isConnected == true)
                    {
                        TimeSpan span = DateTime.Now - m_dtLastSendMessage;

                        // 마지막 메시지를 보낸 이후 3초 이상 지났는지 확인한다.
                        if (span.TotalSeconds > 3.0)
                        {
                            // 접속이 유지되고 있는지 확인한다.
                            if (SendMessage(SOPWebServer.Header.ARE_YOU_THERE, null) == false)
                            {
                                m_isConnected = false;
                                m_postBox.Dispose();
                                m_postBox = null;
                            }
                        }
                    }

                    if (m_isConnected == false)
                    {
                        int nPort = GetServerPort();

                        if (m_postBox == null || (m_postBox != null && m_postBox.Port != nPort))
                            SetPostBox(nPort);

                        if (m_postBox != null)
                        {
                            if (m_postBox.Connect(m_nClientType, m_nClientSubType))
                            {
                                m_isConnected = true;
                            }
                        }
                    }
                }

                if (m_providerInternal.IsConnected)
                {
                    if (m_providerInternal.PingCount > 5)
                    {
                        WriteLineLog("Ping Internal Close!!" + m_providerInternal.PingCount);
                        m_providerInternal.PingCount = 0;

                        try
                        {
                            m_providerInternal.Close();
                        }
                        catch (System.Exception)
                        {
                        }
                    }

                    // IsReadingProcess가 true이면 OnReceive에서 받은 데이터를 처리중이므로 다른 Data를 수신할 수 없는 상태임
                    else if (m_providerInternal.IsReadingProcess)
                    {
                        m_providerInternal.SendData(SOPWebServer.Header.I_AM_HERE);
                    }
                    else
                        m_providerInternal.PingCount++;
                }

                if (!m_providerInternal.IsConnected)
                {
                    int nPort = IntegratedManagement4.InternalMessage.GetInternalServerPort(FormMain.Instance.DBManager, m_dbMgr.SiteID);

                    try
                    {
                        if (nPort > 0)
                            m_providerInternal.Connect("127.0.0.1", nPort);
                    }
                    catch (System.Exception)
                    {
                    }
                }

                Thread.Sleep(400);

                // 날짜가 경과하면 한달이 지난 로그를 삭제한다.
                if (DateTime.Now.Day != dtPrev.Day)
                    DeleteLog();
                
                // TODO: 20초에 한번씩 진행중인 화재들에 대한 유효성 검사를 실시한다.
                //CheckValidProcess(ref dtPrev);
            }
        }

        public void ReleaseThread()
        {
            m_shutdownThread = true;
        }

        #region SendMessage
        public bool SendMessage(int header, byte[] messages)
        {
            if (m_postBox == null || m_isConnected == false)
            {
                m_isConnected = false;
            }
            else
            {
                bool closeConnection;
                bool result = m_postBox.SendMessage(header, messages, out closeConnection);

                if (closeConnection)
                {
                    //WriteLineLog(m_postBox.ErrorMessage);
                    m_isConnected = false;
                }
                else if (result == true)
                {
                    m_dtLastSendMessage = DateTime.Now;
                    WriteSendLog(header, messages);
                }

                return result;
            }

            return false;
        }

        public bool SendMessage(int nNum, short header, int data1, int data2, int data3, int data4)
        {
            if (!m_isConnected)
                return false;

            lock (this)
            {
                ArrayList arrDatas = new ArrayList();
                arrDatas.Add(data1);
                arrDatas.Add(data2);
                arrDatas.Add(data3);
                arrDatas.Add(data4);

                byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
                SendMessage(header, bytes);
            }
            return true;
        }

        public bool SendMessage(int nNum, short header, int data1, int data2, int data3)
        {
            if (!m_isConnected)
                return false;

            lock (this)
            {
                ArrayList arrDatas = new ArrayList();
                arrDatas.Add(data1);
                arrDatas.Add(data2);
                arrDatas.Add(data3);

                byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
                SendMessage(header, bytes);
            }
            return true;
        }

        public bool SendMessage(int nNum, short header, int data1, int data2, int data3, string strData)
        {
            if (!m_isConnected)
                return false;

            lock (this)
            {
                ArrayList arrDatas = new ArrayList();
                arrDatas.Add(data1);
                arrDatas.Add(data2);
                arrDatas.Add(data3);
                arrDatas.Add(strData);

                byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
                SendMessage(header, bytes);
            }
            return true;
        }

        public bool SendMessage(int nNum, short header, int data1, int data2)
        {
            if (!m_isConnected)
                return false;

            lock (this)
            {
                ArrayList arrDatas = new ArrayList();
                arrDatas.Add(data1);
                arrDatas.Add(data2);

                byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
                SendMessage(header, bytes);
            }
            return true;
        }
        #endregion

        public void OnMessage(int header, byte[] messages)
        {
            if (FormMain.Instance.Exit)
                return;

            m_arrReceived = messages;

            ArrayList arrDatas = messages == null ? null : SOPWebServer.BinaryHelper.ReadBytes(messages);

            if (header == SOPWebServer.Header.ARE_YOU_THERE)
            {
                //SendData(SOPWebServer.Header.I_AM_HERE);
            }
            else if (header == SOPWebServer.Header.CLOSE_CONNECTION)
            {
                m_isConnected = false;

                if (m_postBox != null)
                    m_postBox.Dispose();

                m_postBox = null;
            }
            else if (header == SOPWebServer.Header.SENSOR_REACTION_HISTORY_DATA)
            {
                bool escapeLoop;
                bool result = ProcessSensorReactionSensorHistoryData(arrDatas, out escapeLoop);

                if (escapeLoop)
                    return;
            }
            else if (header == SOPWebServer.Header.CLEAR_DETECT_REPORT)
                ProcessClearProcess(arrDatas);
            else if (header == SOPWebServer.Header.SENSOR_REACTION_HISTORY_DATA_LIST)
                ProcessReactionHistoryLogList(arrDatas);
            else if (header == SOPWebServer.Header.SENSOR_ZONE_DATA)
                ProcessSensorData(arrDatas);
            else if (header == SOPWebServer.Header.IGNORE_DETECT_REPORT)
                ProcessIgnoreDetect(arrDatas);
            else if (header == SOPWebServer.Header.ALL_RECEIVER_STATE)
                ProcessAllReciverState(arrDatas);
            else if (header == SOPWebServer.Header.RECEIVER_CONNECT || header == SOPWebServer.Header.RECEIVER_DISCONNECT)
                ProcessReciverState(arrDatas);
            else if (header == SOPWebServer.Header.CHANGE_CONFIG)
                ProcessChangeConfig(arrDatas);
            //else if (header == SOPWebServer.Header.WEATHER_INFO)
            //    ProcessWeatherInfo();
            else if (header == SOPWebServer.Header.EDIT_SENSOR_ZONE)
                ProcessEditSensorZone(arrDatas);
            else if (header == SOPWebServer.Header.SDMS_COMMAND)
                ProcessSDMSCommand(arrDatas);
            else if (header == SOPWebServer.Header.SERVER_COMMAND)
                ProcessServerCommand(arrDatas);
            else if (header == SOPWebServer.Header.EARTHQUAKE_SENSOR_DETECT)
                ProcessEarthquake(arrDatas);
            else if (header == SOPWebServer.Header.COLLAPSE_BUILDING_DETECT)
                ProcessCollapseBuilding(arrDatas);
            else if (header == SOPWebServer.Header.ALARM_STEP)
            {
                ProcessChangeAlarmStep(arrDatas);
            }

            WriteSendLog(header, messages);
        }

        #region SOPWebServer.Header.SENSOR_REACTION_HISTORY_DATA
        // escapeLoop : true이면 OnReceive() 루프를 즉시 빠져나올 것
        private bool ProcessSensorReactionSensorHistoryData(ArrayList arrDatas, out bool escapeLoop)
        {
            escapeLoop = false;

            ReactionLog log = ReadReactionHistoryLog(arrDatas);
            ProcessBeginStatusReactionLog(log, ref escapeLoop);

            return true;
        }
        private ReactionLog ReadReactionHistoryLog(ArrayList arrDatas)
        {
            ReactionLog log = new ReactionLog();

            // Reaction History ID
            log.ID = (int)arrDatas[0];

            // Sensor History ID
            log.SensorHistoryID = (int)arrDatas[1];

            // Reaction Type            
            log.ReactionType = (int)arrDatas[2];

            for (int i = 3; i < arrDatas.Count; i++)
            {
                if (arrDatas[i] is string)
                {
                    string szValue = (string)arrDatas[i];
                    switch (i)
                    {
                        case 4: //  Message
                            log.Message = szValue;
                            break;

                        case 5: // Param 1
                            log.Parameter1 = szValue;
                            break;

                        case 6: // Param 2
                            log.Parameter2 = szValue;
                            break;

                        case 7: // Param 3
                            log.Parameter3 = szValue;
                            break;

                        case 8: // Param 4
                            log.Parameter4 = szValue;
                            break;

                        case 9: // Param 5
                            log.Parameter5 = szValue;
                            break;
                    }
                }
                else if (arrDatas[i] is long)
                {
                    long value = (long)arrDatas[i];

                    if (value == 0)
                        log.LogTime = DateTime.Now;
                    else
                        log.LogTime = DateTime.FromBinary(value);
                }
            }

            return log;
        }

        private bool ProcessBeginStatusReactionLog(ReactionLog log, ref bool escapeLoop)
        {
            // 무시할 알람인지 검사한다.
            if (CheckIgnoreAlarm(log) == false)
                return true;

            ReactionTypeInfo info = log.GetReactionTypeInfo();

            if (info == ReactionTypeInfo.BEGIN_STATUS)
            {
                string szTemp = log.Message.Replace("\"", ",,,");
                string szMsg = ",,,,,,,,,," + szTemp.Replace(".", ",,,");

                BeginProcess(log);
            }

            if (info == ReactionTypeInfo.CHANGE_ALARM_LEVEL)
            {
                ProcessIF process = ProcessManager.Instance.FindProcess(log.SensorHistoryID);
                if (process != null)
                {
                    // PSM의 경우 ChangeDepth에서 Alarm값을 설정하여 준다. 대피반경에서 사용.
                    // added by skkim 2016-04-07
                    //if (log.ReactionType == (int)ReactionType.CHANGE_PSM_ALARM_DEPTH)
                    {
                        process.SetAlarmLevel(log);
                    }
                }
            }
            else if (info == ReactionTypeInfo.USER_RESET)
            {
                string szTemp = log.Message.Replace("\"", ",,,");
                string szMsg = ",,,,,,,,,," + szTemp.Replace(".", ",,,");

                EndProcess(log.SensorHistoryID);
                m_isReadingProcess = false;
                escapeLoop = true;

                return true;
            }
            else if (info == ReactionTypeInfo.NOTIFY)
            {
                if (FireDetectProcess.SoundPlayer.IsLoadCompleted == true)
                {
                    FireDetectProcess.SoundPlayer.Stop();
                }
                // 수동 신고
                if (Convert.ToInt32(log.Parameter2) >= SOPWebServer.Header.ManualReportDefaultID)
                {
                    System.Diagnostics.Debug.WriteLine("Recive Manual Report Log");
                    BeginProcess(log);
                }
            }

            // 현재 진행중인 화재가 변경되지 않는것이므로 Change PSM alarm depth는 진행중인 historyid를 변경하지 않는다.
            if (info != ReactionTypeInfo.BEGIN_STATUS && info != ReactionTypeInfo.CHANGE_ALARM_LEVEL)
            {
                //DlgSelectCase.ProcessingSensorHistoryID = log.SensorHistoryID;
            }

            ReactionLogManager.Instance.AddLog(log);

            return true;
        }

        // 무시할 알람인지 검사한다.
        // Return값이 false이면 해당 알람은 무시한다.
        private bool CheckIgnoreAlarm(ReactionLog log)
        {
            if (log == null)
                return false;

            int nSensorType = GetSensorType(log);
            IFacility.FacilityType sensorType = IFacility.ToFacilityType(nSensorType);

            if (log.ReactionType == (int)ReactionType.BEGIN_STATUS ||
                log.ReactionType == (int)ReactionType.CHANGE_ALARM_DEPTH ||
                log.ReactionType == (int)ReactionType.NOTIFY_SIGNAL)
            {
                if (IFacility.IsFireSensorType(sensorType))
                {
                    // 화재알람 수신거부일경우 알람 무시
                    bool bRecive = PreferenceManager.Instance.ReciveFireSignal;
                    if (bRecive == false)
                        return false;
                }
                else if (IFacility.IsPSMSensorType(sensorType))
                {
                    // PSM알람 수신거부일경우 알람 무시
                    bool bRecive = PreferenceManager.Instance.RecivePSMSignal;
                    if (bRecive == false)
                        return false;


                    int nSensorZoneID;

                    if (log.Parameter2 != null && int.TryParse(log.Parameter2, out nSensorZoneID))
                    {
                        ISensor sensorZone = SensorManager.Instance.GetSensorZone(nSensorZoneID);

                        if (sensorZone != null)
                        {
                            UnE.PSM.PSMSensor sensor = PSMManager.Instance.GetSensor(sensorZone.OrgSensorID);

                            if (sensor != null && sensor.SensorStatus == UnE.PSM.PSMSensor.Status.LocalOff)
                                return false;
                        }
                    }
                }
                else if (IFacility.IsSecurityType(sensorType))
                {
                    // 방범알람 수신거부일경우 알람 무시
                    bool bRecive = PreferenceManager.Instance.ReciveSecuritySignal;
                    if (bRecive == false)
                        return false;
                }
            }

            return true;
        }
        #endregion

        #region SOPWebServer.Header.CLEAR_DETECT_REPORT
        private void ProcessClearProcess(ArrayList arrDatas)
        {
            if (arrDatas != null && arrDatas.Count >= 1 && arrDatas[0] is int)
            {
                int nSensorHistoryID = (int)arrDatas[0];
                if (ProcessManager.Instance.SleepDetectProcess.Count > 0)
                {
                    ProcessIF sleepProcess = ProcessManager.Instance.FindSleepProcess(nSensorHistoryID);
                    if (sleepProcess != null && sleepProcess.ToString() != "")
                    {
                        RemoveSleepProcess(sleepProcess);       //sleep process 에 대한 해제 신호시 제거.
                    }
                }

                if (ProcessManager.Instance.CurrentDetectProcess.Count == 0)
                {
                    FormMain.Instance.Invoke((MethodInvoker)delegate
                    {
                        FormMain.Instance.SetNormalMode();
                        FormMain.Instance.ContentManager.ContentForm.HideZoneVolume();
                        ConnectionLogEx.Instance.WriteLine("Hide All Zone Volume");
                        FormMain.Instance.ContentManager.ContentForm.HidePollutioinView();
                        if (FireDetectProcess.SoundPlayer.SoundLocation != null)
                            FireDetectProcess.SoundPlayer.Stop();
                    });
                }
                ProcessIF process = ProcessManager.Instance.FindProcess(nSensorHistoryID);

                if (process != null && process.ToString() != "")
                {
                    Debug.WriteLine("EndProcess :" + nSensorHistoryID.ToString());
                    Debug.WriteLine("EndProcess :" + process);
                    EndProcess(process);

                    RemoveProcess(process);
                    if (ProcessManager.Instance.HistoryIDToSOPClosing < 0)
                    {
                        SendSensorCloseToSOPSimulator(process.SensorHistoryID);
                    }
                }
            }
        }

        private void RemoveSleepProcess(ProcessIF process)
        {
            if (process != null)
            {
                // 해당 History ID 제거
                SensorHistoryManager.Instance.RemoveSensorHistory(process.SensorHistoryID);
                ProcessManager.Instance.RemoveSleepProcess(process.DetectSensorID);
            }
        }

        private void SendSensorCloseToSOPSimulator(int nSensorHistoryID)
        {
            try
            {
                int nSensorID = SensorHistoryManager.Instance.GetSensorID(nSensorHistoryID);
                System.Diagnostics.Trace.WriteLine("SensorClose Message : " + nSensorHistoryID + ", " + nSensorID);
                if (nSensorID > 0)
                {
                    FormMain.Instance.Invoke((MethodInvoker)delegate
                    {
                        //System.Diagnostics.Trace.WriteLine("Send SOP SensorClose: " + nSensorID);
                        //MainForm.Instance.SendSensorCloseMessageToSOPSimulator(nSensorID, nSensorHistoryID);
                    });
                }
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region SOPWebServer.Header.SENSOR_REACTION_HISTORY_DATA_LIST
        private void ProcessReactionHistoryLogList(ArrayList arrDatas)
        {
            if (arrDatas == null)
                return;

            ArrayList arrReactionLog = new ArrayList();

            int nDataCount = arrDatas.Count;
            int nLogChunkSize = 10;
            for (int i = 0; i < nDataCount - (nLogChunkSize - 1); i += nLogChunkSize)
            {
                ArrayList arrDatas2 = new ArrayList();
                for (int j = i; j < i + nLogChunkSize; j++)
                {
                    arrDatas2.Add(arrDatas[j]);
                }

                ReactionLog log = ReadReactionHistoryLog(arrDatas2);
                if (log != null)
                    arrReactionLog.Add(log);
            }

            ArrayList arrRemoveProcess = new ArrayList();
            ArrayList arrCurrentLog = new ArrayList();

            foreach (KeyValuePair<string, ProcessIF> pair in ProcessManager.Instance.CurrentDetectProcess)
            {
                ReactionLog log = FindLog(arrReactionLog, pair.Value.SensorHistoryID);

                // 현재 진행중인 화재 가운데 arrReactionLog에 포함되지 않은것은 이미 종료된 화재이다.
                if (log == null)
                {
                    EndProcess(pair.Value);
                    //RemoveProcess(pair.Value);
                    arrRemoveProcess.Add(pair.Value);
                }
                else
                {
                    arrCurrentLog.Add(log);
                    arrReactionLog.Remove(log);
                }
            }
            foreach (KeyValuePair<int, ProcessIF> pair in ProcessManager.Instance.SleepDetectProcess)
            {
                ReactionLog log = FindLog(arrReactionLog, pair.Value.SensorHistoryID);

                // 현재 진행중인 화재 가운데 arrReactionLog에 포함되지 않은것은 이미 종료된 신홓
                // Sleep 프로세스만 지운다. by hypark                
                if (log == null)
                {
                    RemoveSleepProcess(pair.Value);
                }
            }

            foreach (ProcessIF process in arrRemoveProcess)
            {
                RemoveProcess(process);

                if (ProcessManager.Instance.HistoryIDToSOPClosing < 0)
                {
                    SendSensorCloseToSOPSimulator(process.SensorHistoryID);
                }
            }

            bool bRunSimulator = false;

            // 새로운 화재 상황에 대한 Log
            ReactionLog lastAlarmLog = null;
            ProcessIF lastAlarmProcess = null;
            foreach (ReactionLog log in arrReactionLog)
            {
                // 무시할 알람인지 검사한다.
                if (CheckIgnoreAlarm(log) == false)
                    continue;

                ReactionTypeInfo info = log.GetReactionTypeInfo();

                if (info == ReactionTypeInfo.BEGIN_STATUS || info == ReactionTypeInfo.CHANGE_ALARM_LEVEL)
                {

                    ProcessIF process = ProcessManager.Instance.FindProcess(log.SensorHistoryID);
                    if (process == null)
                    {
                        process = BeginProcess(log, true);
                    }

                    if (process != null)
                    {
                        // PSM의 경우 ChangeDepth에서 Alarm값을 설정하여 준다. 대피반경에서 사용.
                        // added by skkim 2016-04-07
                        if (info == ReactionTypeInfo.CHANGE_ALARM_LEVEL)
                        //if (log.ReactionType == (int)ReactionType.CHANGE_PSM_ALARM_DEPTH)
                        {
                            process.SetAlarmLevel(log);
                        }

                        lastAlarmLog = log;
                        lastAlarmProcess = process;
                        FormMain.Instance.Invoke((MethodInvoker)delegate
                        {
                            FormMain.Instance.AddSensorDectect(process, false, false);
                        });
                    }
                    Thread.Sleep(300);
                }
                else if (info == ReactionTypeInfo.NOTIFY/* || log.ReactionType == (int)ReactionType.TRAINNING_FIRE*/)
                {
                    // 화재 신고 또는 탐지 부터 받을시  해당 프로세스가 없는 경우 추가 해준다.
                    ProcessIF process = ProcessManager.Instance.FindProcess(log.SensorHistoryID);
                    if (process == null)
                    {
                        BeginProcess(log, true);
                        process = ProcessManager.Instance.FindProcess(log.SensorHistoryID);
                        if (process != null)
                        {
                            process.SetAlarmLevel(log);

                            lastAlarmLog = log;
                            lastAlarmProcess = process;

                            FormMain.Instance.Invoke((MethodInvoker)delegate
                            {
                                FormMain.Instance.AddSensorDectect(process, false, false);
                                bRunSimulator = true;
                            });
                        }
                    }
                }
                else if (info != ReactionTypeInfo.END_STATUS)
                {
                    AddProcess(log, false);
                }

                if (log.ReactionType == (int)ReactionType.IGNORE_SOP
                    || log.ReactionType == (int)ReactionType.RUN_N_CANCEL_SOP
                    || log.ReactionType == (int)ReactionType.FINISH_SOP)
                {
                    // 종료되지 않은 수동신고의 경우 종료 할 수 있도록 프로세스를 추가해준다.
                    int nHistoryID = log.SensorHistoryID;
                    int nZoneID = SensorHistoryManager.Instance.GetManualFireReportZone(nHistoryID);
                    if (nZoneID != -1)
                    {
                        log.Parameter1 = nZoneID.ToString();
                        log.Parameter2 = "0";
                        BeginProcess(log, true);
                    }
                }

                // 화재신고가 포함되어 있는경우 SOP시뮬레이터 기동
                if (bRunSimulator == true)
                    FormMain.Instance.SendDetectMessageToSOPSimulator();

                ReactionLogManager.Instance.AddLog(log);
            }

            // 마지막 프로세스에 대해 select를 지정한다.
            if (lastAlarmProcess != null)
            {
                FormMain.Instance.Invoke((MethodInvoker)delegate
                {
                    FormMain.Instance.SelectSensorDetectProcess(lastAlarmProcess.SensorHistoryID, lastAlarmProcess.DetectSensorID);
                    lastAlarmProcess.BeginProcess();
                });
            }

            FormMain.Instance.Invoke((MethodInvoker)delegate
            {
                FormMain.Instance.SelectLastFireDectectProcess();

                ProcessIF processLast = FormMain.Instance.LastSensorDetectProcess;
                ProcessIF processCurrent = FormMain.Instance.CurrentSensorDetectProcess;
                if (processLast != null && processLast != processCurrent)
                {
                    bool bSelected = processLast.Select();
                    if (bSelected)
                    {
                        ReactionLogManager.Instance.ProcessLog(processLast.LastLog, true);
                    }
                }
            });

            // 기존에 진행되고 있던 화재 상황에 대한 Log
            foreach (ReactionLog log in arrCurrentLog)
            {
                ReactionLogManager.Instance.AddLog(log);
            }
        }

        private ReactionLog FindLog(ArrayList arrReactionLog, int nSensorHistoryID)
        {
            foreach (ReactionLog log in arrReactionLog)
            {
                if (log.SensorHistoryID == nSensorHistoryID)
                    return log;
            }

            return null;
        }

        private void AddProcess(ReactionLog log, bool bAddSelected = true)
        {
            string strSQL = string.Format("select szh.SensorID, sz.Type, sz.OrgSensorID, fs.X, fs.Y, fs.Z, sz.EquipZoneID from SensorZoneHistory as szh, SensorZone as sz, FireSensor as fs where szh.ID = {0} and szh.SensorID = sz.ID and sz.OrgSensorID = fs.ID",
                    log.SensorHistoryID);
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count < 7)
                return;

            int nSensorZoneID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            int nSensorType = WebDBManager.GetIntField(arrResult[1].ToString(), -1);
            int nSensorID = WebDBManager.GetIntField(arrResult[2].ToString(), -1);
            float x = WebDBManager.GetFloatField(arrResult[3].ToString(), 0.0f);
            float y = WebDBManager.GetFloatField(arrResult[4].ToString(), 0.0f);
            float z = WebDBManager.GetFloatField(arrResult[5].ToString(), 0.0f);
            int nEquipZoneID = WebDBManager.GetIntField(arrResult[6].ToString(), -1);

            if (!ProcessManager.Instance.CurrentDetectProcess.ContainsKey(nSensorZoneID.ToString()))
            {
                ProcessIF process = null;

                if (nSensorType == (int)IFacility.FacilityType.FIRE_SENSOR || nSensorType == (int)IFacility.FacilityType.FireSensor_TypeA || nSensorType == (int)IFacility.FacilityType.FireSensor_SiemensType || nSensorType == (int)IFacility.FacilityType.FireSensor_AnalogSmokeType)
                    process = new FireDetectProcess();
                else if (nSensorType == (int)IFacility.FacilityType.PSM_SENSOR)
                    process = new GasDetectProcess();
                else
                    return;

                if (!SensorManager.Instance.DicAllSenor.ContainsKey(nSensorZoneID))
                    return;

                EquipmentZone zone = ZoneManager.Instance.GetEquipZone(nEquipZoneID);
                if (zone == null)
                    return;

                ISensor sensor = SensorManager.Instance.DicAllSenor[nSensorZoneID];

                process.DetectSensorID = nSensorZoneID;
                process.SensorHistoryID = log.SensorHistoryID;
                process.TargetSensor = sensor;
                process.TargetZone = zone;
                process.LastLog = log;

                ProcessManager.Instance.CurrentDetectProcess[nSensorZoneID.ToString()] = process;

                FormMain.Instance.Invoke((MethodInvoker)delegate
                {
                    FormMain.Instance.AddSensorDectect(process, bAddSelected);
                });
            }
        }
        #endregion

        #region SOPWebServer.Header.SENSOR_ZONE_DATA
        private void ProcessSensorData(ArrayList arrDatas)
        {
            if (arrDatas != null && arrDatas.Count >= 6 && arrDatas[0] is int && arrDatas[1] is int && arrDatas[2] is int && arrDatas[3] is int && arrDatas[4] is int && arrDatas[5] is int)
            {
                int nSensorZoneID = (int)arrDatas[0]; // 11
                int nSensorType = (int)arrDatas[1]; //20
                int nConnected = (int)arrDatas[2]; //29
                int nZoneID = (int)arrDatas[3]; //38
                int nSensorData = (int)arrDatas[4]; //47
                int nSensorID = (int)arrDatas[5]; //56

                if (arrDatas.Count >= 8)
                {
                    int wDircetion = (int)arrDatas[6]; //65      //wind direction : 광교
                    int wSpeed = (int)arrDatas[7]; //74

                    FormMain.Instance.Invoke((MethodInvoker)delegate
                    {
                        FormMain.Instance.ContentManager.ChangeTab(UnE.View.Content.ContentOwnerTab.M3D_TAB);

                        FormMain.Instance.ContentManager.ContentForm.PushViewState(true);
                        FormMain.Instance.ContentManager.ContentForm.HideZoneVolume();
                        FormMain.Instance.ContentManager.ContentForm.HideEvacCircle();
                        FormMain.Instance.ContentManager.ContentForm.ShowPollutionView(wDircetion, wSpeed);

                    });
                }

                if (SensorManager.Instance.DicAllSenor.ContainsKey(nSensorZoneID))
                {
                    ISensor sensor = SensorManager.Instance.DicAllSenor[nSensorZoneID];

                    bool bPrevState = sensor.Connected;
                    sensor.Connected = nConnected == 1;
                    sensor.SensorData = nSensorData;

                    if (sensor.POI != null)
                    {
                        sensor.POI.Facility.Connected = (nConnected == 1);
                        ReactionLog log = SensorHistoryManager.Instance.SensorZoneHistoryLog;
                        //int nHistoryID = SensorHistoryManager.Instance.SensorZoneHistoryID;

                        // 무시할 알람인지 검사한다.
                        if (CheckIgnoreAlarm(log) == false)
                            return;

                        if (bPrevState != sensor.Connected && sensor.Connected == false)
                        {
                            ProcessManager.Instance.BeginProcess(sensor, log, ProcessType.DisconnectSensor);
                        }
                        else if (bPrevState != sensor.Connected && sensor.Connected == true)
                        {
                            ProcessManager.Instance.BeginProcess(sensor, log, ProcessType.ConnectSensor);
                        }
                    }
                }
            }
        }
        #endregion

        #region SOPWebServer.Header.IGNORE_DETECT_REPORT
        private void ProcessIgnoreDetect(ArrayList arrDatas)
        {
            if (arrDatas != null && arrDatas.Count >= 1 && arrDatas[0] is int)
            {
                int nSensorHistoryID = (int)arrDatas[0];
                EndProcess(nSensorHistoryID);
            }
        }
        #endregion

        #region SOPWebServer.Header.ALL_RECEIVER_STATE
        private void ProcessAllReciverState(ArrayList arRecivers)
        {
            if (arRecivers == null)
                return;

            int nReciverID = -1;
            bool bConnected = false;

            int nDataCount = arRecivers.Count;

            for (int i = 0; i < nDataCount; i += 2)
            {
                nReciverID = (int)arRecivers[i];
                bConnected = (int)arRecivers[i + 1] > 0 ? true : false;


                bool bRecivePoll = (int)arRecivers[i + 1] > 10 ? true : false;
                if (ReciverManager.Instance.DicReciverList.ContainsKey(nReciverID))
                {
                    Reciver reciver = ReciverManager.Instance.DicReciverList[nReciverID];
                    ReciverManager.Instance.UpdateState(nReciverID, bConnected, bRecivePoll);
                }
            }
        }
        #endregion

        #region header == SOPWebServer.Header.RECEIVER_CONNECT
        private void ProcessReciverState(ArrayList arrDatas)
        {
            if (arrDatas != null && arrDatas.Count >= 2 && arrDatas[0] is int && arrDatas[1] is int)
            {
                int nReciverID = (int)arrDatas[0];
                int nConnect = (int)arrDatas[1];
                ReciverManager.Instance.UpdateState(nReciverID, (nConnect > 0 ? true : false), (nConnect > 10 ? true : false));
            }
        }
        #endregion

        #region SOPWebServer.Header.CHANGE_CONFIG
        private void ProcessChangeConfig(ArrayList arrDatas)
        {
            if (arrDatas == null)
                return;

            if (arrDatas.Count < 3)
                return;

            try
            {
                if (arrDatas[0] is int && arrDatas[1] is string && arrDatas[2] is string)
                {
                    int nClientType = (int)arrDatas[0];
                    string strPropertyName = (string)arrDatas[1];
                    string strPropertyValue = (string)arrDatas[2];

                    if (nClientType != SOPWebServer.ClientType.SDMS)
                        return;

                    if (strPropertyName == SOP.SDMSConfig.PropertyName)
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

                            if (((nConfigValue & (int)SOP.SDMSConfig.ConfigType.EQUIPZONE_FACILITY_MANAGER) == (int)SOP.SDMSConfig.ConfigType.EQUIPZONE_FACILITY_MANAGER) ||
                                ((nConfigValue & (int)SOP.SDMSConfig.ConfigType.BUILDING_FACILITY_MANAGER) == (int)SOP.SDMSConfig.ConfigType.BUILDING_FACILITY_MANAGER) ||
                                ((nConfigValue & (int)SOP.SDMSConfig.ConfigType.ENTIRE_FACILITY_MANAGER) == (int)SOP.SDMSConfig.ConfigType.ENTIRE_FACILITY_MANAGER))
                                ProcessChangeFacilityManager();
                        }
                    }
                    else if (strPropertyName == SOP.SDMSConfig.GetPropertyName(SOP.SDMSConfig.ConfigType.EQUIPZONE_CCTV))
                    {
                        int nEquipZoneID;

                        if (int.TryParse(strPropertyValue, out nEquipZoneID))
                        {
                            ProcessChangeEquipZoneCCTV(nEquipZoneID);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ConnectionLogEx.Instance.WriteLine(e.StackTrace);
            }
        }

        private void ProcessChangeCompanyMember()
        {
            FormMain.Instance.Invoke((MethodInvoker)delegate
            {
                FormMain.Instance.DataManager.ReloadCompanyMember();
            });
        }

        private void ProcessChangeFacilityManager()
        {
            // TODO: FacilityManager 변경된 항목 적용
            /*
            FormMain.Instance.Invoke((MethodInvoker)delegate
            {                
                // EditFacilityManager 의 데이터 삭제
                PageBackstageHome.Instance.RemoveEditManagerData();

                // FormEditManager 닫기
                if (PageBackstageHome.TranslucentForm.InnerSubForm != null &&
                    PageBackstageHome.TranslucentForm.InnerSubForm.GetType() == typeof(FormEditManager))
                {
                    PageBackstageHome.TranslucentForm.InnerSubForm.Close();
                }
                // FormManager 닫기

                if (PageBackstageHome.TranslucentForm.InnerForm != null &&
                    PageBackstageHome.TranslucentForm.InnerForm.GetType() == typeof(FormManager))
                {
                    PageBackstageHome.TranslucentForm.InnerForm.Close();
                }
                FormMain.Instance.DataManager.LoadFacilityManager();                
            });
            */
        }

        private void ProcessChangeEquipZoneCCTV(int nEquipZoneID)
        {
            // TODO: EquipZoneCCTV 변경된 항목 적용
            /*
            FormMain.Instance.Invoke((MethodInvoker)delegate
            {
                // CCTV 뷰 닫기
                //if (FormMain.Instance.ShowEquipZoneCCTV)
                // {
                if (FormMain.Instance.CurrentEquipZone.ID == nEquipZoneID)
                {
                    // EditEquipZoneCCTV의 데이터 삭제
                    PageBackstageHome.Instance.RemoveEquipZoneCCTVData();

                    //if (PageBackstageHome.TranslucentForm.InnerForm.GetType() == typeof(Form4CCTV))
                    //{
                    // cCTV뷰 닫기
                    //FormMain.Instance.ShowEquipZoneCCTV = false;
                    // PageBackstageHome.TranslucentForm.InnerForm.Close();
                    //}
                }
                // }
                CCTVManager.Instance.LoadEquipZoneCCTV(nEquipZoneID);
            });
            */
        }
        #endregion

        #region SOPWebServer.Header.EDIT_SENSOR_ZONE
        public void ProcessEditSensorZone(ArrayList arrDatas)
        {
            if (arrDatas != null && arrDatas.Count >= 4 && arrDatas[0] is int && arrDatas[1] is int && arrDatas[2] is int && arrDatas[3] is int)
            {
                int nSensorZoneID = (int)arrDatas[0];
                int nOrgEquipZoneID = (int)arrDatas[1];
                int nNewEquipZoneID = (int)arrDatas[2];
                int nNewZoneID = (int)arrDatas[3];

                // 센서존 삭제
                if (nNewEquipZoneID == 0)
                {
                    EquipmentZone equipZone = ZoneManager.Instance.GetEquipZone(nOrgEquipZoneID);
                    if (equipZone != null)
                    {
                        ISensor szTarget = null;
                        EquipmentZoneObjectList mCurrentList = SensorManager.Instance.FindZoneInSensor(equipZone.ID);
                        if (mCurrentList != null)
                        {
                            foreach (ISensor sz in mCurrentList.SensorList)
                            {
                                if (sz.ID == nSensorZoneID)
                                {
                                    szTarget = sz;
                                    break;
                                }
                            }
                            if (szTarget != null)
                            {
                                szTarget.EquipZoneID = nNewEquipZoneID;
                                szTarget.EquipZoneDB = nNewEquipZoneID;
                                szTarget.ZoneID = nNewZoneID;
                                szTarget.ZoneIDDB = nNewZoneID;

                                mCurrentList.SensorList.Remove(szTarget);
                            }
                        }
                    }
                }

                else
                {
                    // 센서존 추가
                    if (nOrgEquipZoneID == 0)
                    {
                        FireSensor targetSensor = null;

                        if (SensorManager.Instance.DicAllSenor.ContainsKey(nSensorZoneID))
                        {
                            targetSensor = (FireSensor)SensorManager.Instance.DicAllSenor[nSensorZoneID];
                        }
                        else
                        {
                            targetSensor = (FireSensor)SensorManager.Instance.MakeNewFireSensor(nSensorZoneID);
                        }

                        if (targetSensor != null)
                        {
                            targetSensor.EquipZoneID = nNewEquipZoneID;
                            targetSensor.EquipZoneDB = nNewEquipZoneID;
                            targetSensor.ZoneID = nNewZoneID;
                            targetSensor.ZoneIDDB = nNewZoneID;
                            SensorManager.Instance.AddSensor(targetSensor);
                        }
                    }
                    // 센서존 업데이트
                    else
                    {
                        EquipmentZone equipZone = ZoneManager.Instance.GetEquipZone(nOrgEquipZoneID);
                        if (equipZone != null)
                        {
                            ISensor szTarget = null;
                            EquipmentZoneObjectList mCurrentList = SensorManager.Instance.FindZoneInSensor(nOrgEquipZoneID);
                            if (mCurrentList != null)
                            {
                                foreach (ISensor sz in mCurrentList.SensorList)
                                {
                                    if (sz.ID == nSensorZoneID)
                                    {
                                        szTarget = sz;
                                        break;
                                    }
                                }
                                if (szTarget != null)
                                {
                                    szTarget.EquipZoneID = nNewEquipZoneID;
                                    szTarget.EquipZoneDB = nNewEquipZoneID;
                                    szTarget.ZoneID = nNewZoneID;
                                    szTarget.ZoneIDDB = nNewZoneID;

                                    mCurrentList.SensorList.Remove(szTarget);
                                }
                            }

                            EquipmentZoneObjectList newList = SensorManager.Instance.FindZoneInSensor(nNewEquipZoneID);
                            if (newList != null)
                            {
                                if (szTarget != null)
                                {
                                    mCurrentList.SensorList.Add(szTarget);
                                }
                            }
                            else
                            {
                                if (szTarget != null)
                                    SensorManager.Instance.AddSensor((FireSensor)szTarget);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region SOPWebServer.Header.SDMS_COMMAND
        private void ProcessSDMSCommand(ArrayList arrDatas)
        {
            if (arrDatas == null || arrDatas.Count == 0 || (arrDatas[0] is byte) == false)
                return;

            int nDataCount = arrDatas.Count;

            byte cmd = (byte)arrDatas[0];
            // TODO: SDMSCommand 구현
            //Debug.WriteLine("SDMS CommandType : " + cmd);
            //if (cmd == SDMSCommandType.CHANGE_PSM_SENSOR_STATUS)
            //    ProcessChangePSMSensorStatus(arrDatas);
            //else if (cmd == SDMSCommandType.REFRESH_PSM_SENSOR_LIFE_TIME)
            //    ProcessRefreshPSMSensorLifeTime();
            //else if (cmd == SDMSCommandType.SDMS_PUBLIC_MESSAGE)
            //    ProcessSDMSPublicMessage(arrDatas);
            //else if (cmd == SDMSCommandType.SDMS_PUBLIC_MESSAGE_ID)
            //    ProcessSDMSPublicMessageID(arrDatas);
            //else if (cmd == SDMSCommandType.PSM_SENSOR_ALARM_LEVEL)
            //    ProcessPSMAlarmLevel(arrDatas);
            //else if (cmd == SDMSCommandType.SET_VIEW)
            //    ProcessSetView(arrDatas);
            //else if (cmd == SDMSCommandType.CHANGE_TAG_ACTIVATION)
            //    Change();
        }
        /*
        private bool ProcessChangePSMSensorStatus(ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;

            if (nDataCount != 6)
                return false;

            if ((arrDatas[1] is int) && (arrDatas[2] is byte) && (arrDatas[3] is long) && (arrDatas[4] is long) && (arrDatas[5] is int))
            {
                int nSensorID = (int)arrDatas[1];
                byte status = (byte)arrDatas[2];
                long beginTime = (long)arrDatas[3];
                long endTime = (long)arrDatas[4];
                int nSOPGenUser = (int)arrDatas[5];

                FormMain.Instance.PageHome.SetPSMSensorStatus(nSensorID, status, beginTime, endTime);
                return true;
            }

            return false;
        }

        private void ProcessRefreshPSMSensorLifeTime()
        {
            SDMS.PopupDialog.FormPSMList frmPSMList = SDMS.PopupDialog.FormPSMList.Instance;

            if (frmPSMList != null && frmPSMList.IsDisposed == false)
                frmPSMList.RefreshSensorLifeTime();
        }

        private void ProcessSDMSPublicMessage(ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;
            if (nDataCount < 2)
                return;

            if ((arrDatas[1] is int) == false)
                return;

            int nSegment = 7;
            int nMessageCount = (int)arrDatas[1];
            List<PopupDialog.FormMessageReceiver.Message> messages = new List<PopupDialog.FormMessageReceiver.Message>();

            for (int i = 2; i < nDataCount - (nSegment - 1); i += nSegment)
            {
                if ((i - 2) / nSegment >= nMessageCount)
                    return;

                if ((arrDatas[i] is int) && (arrDatas[i + 1] is long) && (arrDatas[i + 2] is string) && (arrDatas[i + 3] is string) &&
                    (arrDatas[i + 4] is string) && (arrDatas[i + 5] is int) && (arrDatas[i + 6] is string))
                {
                    int nID = (int)arrDatas[i];
                    long time = (long)arrDatas[i + 1];
                    string strTitle = (string)arrDatas[i + 2];
                    string strText = (string)arrDatas[i + 3];
                    string strRTF = (string)arrDatas[i + 4];
                    int nSOPGenUserID = (int)arrDatas[i + 5];
                    string strSenderName = (string)arrDatas[i + 6];

                    PopupDialog.FormMessageReceiver.Message message = new PopupDialog.FormMessageReceiver.Message();

                    message.ID = nID;
                    message.Time = DateTime.FromBinary(time);
                    message.Title = strTitle;
                    message.Text = strText;
                    message.RTF = strRTF.Length == 0 ? null : strRTF;
                    message.SOPGenUserID = nSOPGenUserID;
                    message.SenderName = strSenderName;

                    messages.Add(message);
                }
            }

            if (messages.Count > 0)
            {
                FormMain.Instance.ReadSDMSMessage(messages);
                messages.Clear();
            }
        }

        private void ProcessSDMSPublicMessageID(ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;
            if (nDataCount < 2)
                return;

            if ((arrDatas[1] is int) == false)
                return;

            int nMessageID = (int)arrDatas[1];
            FormMain.Instance.ReadSDMSMessage(nMessageID);
        }

        private void ProcessPSMAlarmLevel(ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;
            if (nDataCount >= 5 && (arrDatas[1] is int) && (arrDatas[2] is float) && (arrDatas[3] is float) && (arrDatas[4] is float))
            {
                int nPSMSensorID = (int)arrDatas[1];
                float fLevel1 = (float)arrDatas[2];
                float fLevel2 = (float)arrDatas[3];
                float fLevel3 = (float)arrDatas[4];

                UnE.PSM.PSMSensor sensor = PSMManager.Instance.GetSensor(nPSMSensorID);

                if (sensor != null)
                {
                    sensor.LimitLevel1 = fLevel1;
                    sensor.LimitLevel2 = fLevel2;
                    sensor.LimitLevel3 = fLevel3;
                }
            }
        }

        private void ProcessSetView(ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;
            if (nDataCount >= 2 && (arrDatas[1] is string))
            {
                string szViewName = (string)arrDatas[1];
                if (szViewName != null)
                {
                    FormMain.Instance.Invoke((MethodInvoker)delegate
                    {
                        if (FormMain.Instance.PageHome != null && FormMain.Instance.PageHome.ContentForm != null)
                        {
                            FormMain.Instance.PageHome.ContentForm.View1Click(null, null);
                            FormMain.Instance.PageHome.ContentForm.HomeView(szViewName);
                        }

                    });
                }
            }
        }
        */
        #endregion

        #region SOPWebServer.Header.SERVER_COMMAND
        private void ProcessServerCommand(ArrayList arrDatas)
        {
            if (arrDatas == null || arrDatas.Count == 0 || (arrDatas[0] is byte) == false)
                return;

            int nDataCount = arrDatas.Count;
            byte command = (byte)arrDatas[0];

            if (command == ServerCommandType.DELETE_SENSOR_TAG_HISTORY)
            {
                if (nDataCount < 2 || arrDatas[1].GetType() != typeof(long))
                    return;

                long time = (long)arrDatas[1];
                SensorTagHistoryManager.Instance.ProcessDeleteSensorTagHistory(DateTime.FromBinary(time));
            }
        }
        #endregion

        #region SOPWebServer.Header.EARTHQUAKE_SENSOR_DETECT
        private void ProcessEarthquake(ArrayList arrDatas)
        {
            if (arrDatas == null)
                return;

            int nDataCount = arrDatas.Count;
            if (nDataCount < 6)
                return;

            if (arrDatas[0] is int && arrDatas[1] is float && arrDatas[2] is int && arrDatas[3] is int && arrDatas[4] is string && arrDatas[5] is long)
            {
                int nSensorID = (int)arrDatas[0];
                float fMagnitude = (float)arrDatas[1];
                int nIntensity = (int)arrDatas[2];
                int nAlarmLevel = (int)arrDatas[3];
                string strPosition = (string)arrDatas[4];
                VariousData<DateTime> time = (long)arrDatas[5] == 0 ? null : new VariousData<DateTime>(DateTime.FromBinary((long)arrDatas[5]));

                // SDMS 지진 이벤트를 발생시킨다.
                FormMain.Instance.ContentManager.ContentForm.EarthquakeEvent(nIntensity, fMagnitude, strPosition, true);
                //SDMS.ScriptProxy.Instance.UserObject.SDMSEarthquakeEvent.Invoke(nIntensity, fMagnitude, strPosition, false);
            }
        }
        #endregion

        #region SOPWebServer.Header.COLLAPSE_BUILDING_DETECT
        private void ProcessCollapseBuilding(ArrayList arrDatas)
        {
            // TODO: 재난이력에 남지는 않지만(재난 ComboBox & 리포트) 실제 재난상황과 동일하게 작동하는 신호
            /*
            if (arrDatas == null)
                return;

            int nDataCount = arrDatas.Count;
            if (nDataCount >= 3 && arrDatas[0] is string && arrDatas[1] is bool && arrDatas[2] is bool)
            {
                string strBuildingName = (string)arrDatas[0];
                bool isReal = (bool)arrDatas[1];
                bool finishEvent = (bool)arrDatas[2];

                FormMain.Instance.Invoke((MethodInvoker)delegate
                {
                    Building building = null;

                    foreach (KeyValuePair<int, Building> pair in ZoneManager.Instance.DicBuildings)
                    {
                        if (pair.Value.BuildingName == strBuildingName)
                        {
                            building = pair.Value;
                            break;
                        }
                    }

                    if (building != null)
                    {
                        if (finishEvent)
                        {
                            FormMain.Instance.FinishBuildingCollapse(strBuildingName);
                        }
                        else
                        {
                            FormMain.Instance.ContentManager.ContentForm.ZoomBuilding(building.BuildingID);
                            FormMain.Instance.ContentManager.ContentForm.SelectBuilding(building.BuildingID);
                            FormMain.Instance.SetBuilingCollapseDetect(strBuildingName, isReal);
                        }
                    }
                });
            }
            */
        }
        #endregion

        #region SOPWebServer.Header.ALARM_STEP
        private void ProcessChangeAlarmStep(ArrayList arrDatas)
        {
            if (arrDatas == null)
                return;

            int nDataCount = arrDatas.Count;
            if (nDataCount < 2)
                return;

            if (arrDatas[0] is int && arrDatas[1] is int)
            {
                int nStep = (int)arrDatas[0];
                int nAlarmCount = (int)arrDatas[1];

                for (int i = 0; i < nAlarmCount; i++)
                {
                    int nSensorZoneID = (int)arrDatas[i + 2];
                    FormMain.Instance.DicAlarmStep[nSensorZoneID] = nStep;
                }

                FormMain.Instance.Invoke((MethodInvoker)delegate
                {
                    FormMain.Instance.SetAlarmLevel();
                });
            }
        }
        #endregion

        private int GetSensorType(ReactionLog log)
        {
            string strSQL = "Select sz.Type from SensorZoneHistory as szh, SensorZone as sz where szh.SensorID = sz.ID and szh.ID = " + log.SensorHistoryID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            VariousData<int> type = WebDBManager.GetIntField(arrResult[0].ToString());

            if (type == null)
                return -1;

            return type.Data;
        }

        private ProcessType GetProcessType(IFacility.FacilityType sensorType)
        {
            switch (sensorType)
            {
                case IFacility.FacilityType.PSM_SENSOR:
                    return ProcessType.PSMAlarm;

                case IFacility.FacilityType.FireF1_S1:
                case IFacility.FacilityType.Fire_S1:
                case IFacility.FacilityType.SecomFire:
                    return ProcessType.FireAlarm;

                case IFacility.FacilityType.Intrusion_S1:
                case IFacility.FacilityType.Loiter_S1:
                case IFacility.FacilityType.Collapse_S1:
                case IFacility.FacilityType.Theft_S1:
                case IFacility.FacilityType.Neglect_S1:
                case IFacility.FacilityType.VirtualFence_S1:
                case IFacility.FacilityType.EmergencyBell_S1:
                case IFacility.FacilityType.GeneralIntrusionT1_S1:
                case IFacility.FacilityType.GeneralIntrusionT2_S1:
                case IFacility.FacilityType.InternalIntrusionT3_S1:
                case IFacility.FacilityType.VaultIntrusionT4_S1:
                case IFacility.FacilityType.CustomerEmergencyC1_S1:
                case IFacility.FacilityType.CustomerEmergencyC2_S1:
                case IFacility.FacilityType.RescueQQ_S1:
                case IFacility.FacilityType.GasG1_S1:
                case IFacility.FacilityType.BlackoutAbnormalityU1_S1:
                case IFacility.FacilityType.LeakAbnormalityU4_S1:
                case IFacility.FacilityType.SynthesisAlertAbnormalityU8_S1:
                case IFacility.FacilityType.ExternalAlarmBell:
                case IFacility.FacilityType.SecomExternalAlarmBell:
                case IFacility.FacilityType.SecomWomenAlarmBell:
                    return ProcessType.SecurityAlarm;

                case IFacility.FacilityType.Earthquake:
                    return ProcessType.EarthquakeAlarm;

                case IFacility.FacilityType.TEMPERATURE_HUMIDITY:
                    return ProcessType.TemperatureHumidityAlarm;

                case IFacility.FacilityType.BLACKOUT:
                    return ProcessType.BlackoutAlarm;

                case IFacility.FacilityType.DOOR:
                    return ProcessType.DoorAlarm;

                case IFacility.FacilityType.STRONG_WIND:
                    return ProcessType.StrongWindAlarm;

                case IFacility.FacilityType.SUBMERGENCY:
                    return ProcessType.SubmergencyAlarm;

                case IFacility.FacilityType.TERROR:
                    return ProcessType.TerrorAlarm;

                case IFacility.FacilityType.FIREWALL:
                    return ProcessType.FirewallAlarm;

                case IFacility.FacilityType.CORONA:
                    return ProcessType.CoronaAlarm;
            }
            return ProcessType.FireAlarm;
        }

        #region Process
        private ProcessIF BeginProcess(ReactionLog log, bool bAddOnly = false)
        {
            int nZoneID = -1, nSensorZoneID = -1;

            int.TryParse(log.Parameter1, out nZoneID);
            int.TryParse(log.Parameter2, out nSensorZoneID);

            EquipmentZone zone = null;
            ISensor sensor = null;

            if (nZoneID > 0)
                zone = ZoneManager.Instance.GetEquipZone(nZoneID);

            if (nSensorZoneID > 0 && nSensorZoneID < SOPWebServer.Header.ManualReportDefaultID)
            {
                if (SensorManager.Instance.DicAllSenor.ContainsKey(nSensorZoneID))
                {
                    sensor = SensorManager.Instance.DicAllSenor[nSensorZoneID];
                }
            }
            else // 수동 신고
            {
                Debug.WriteLine("Recive Manual Report Log");
                SensorHistoryManager.Instance.AddSensorHistory(log);
                FormMain.Instance.Invoke((MethodInvoker)delegate
                {
                    //MainForm.Instance.SelectCCTVTab(false);
                });

                ISensor ss = null;
                IFacility.FacilityType type = FormMain.Instance.GetManualReportType(log.SensorHistoryID);
                ProcessType processType = ProcessType.FireAlarm;

                if (type == IFacility.FacilityType.FIRE_SENSOR)
                {
                    ss = new FireSensor();
                }
                else if (type == IFacility.FacilityType.PSM_SENSOR)
                {
                    ss = new UnE.PSM.PSMSensorZone();
                    processType = ProcessType.PSMAlarm;
                }
                else
                {
                    ss = new EtcSensor();
                    if (type == IFacility.FacilityType.TERROR)
                        processType = ProcessType.TerrorAlarm;
                    else if (type == IFacility.FacilityType.SUBMERGENCY)
                        processType = ProcessType.SubmergencyAlarm;
                    else if (type == IFacility.FacilityType.STRONG_WIND)
                        processType = ProcessType.StrongWindAlarm;
                    else if (type == IFacility.FacilityType.CORONA)
                        processType = ProcessType.CoronaAlarm;
                    else if (type == IFacility.FacilityType.BLACKOUT)
                        processType = ProcessType.BlackoutAlarm;
                    else if (type == IFacility.FacilityType.Earthquake)
                        processType = ProcessType.EarthquakeAlarm;
                }

                ss.ID = nSensorZoneID; //nZoneID;

                Zone realzone = ZoneManager.Instance.GetZone(nZoneID);

                ProcessManager.Instance.BeginProcess(ss, realzone, log.SensorHistoryID, processType, false);

                ProcessIF process = ProcessManager.Instance.FindProcess(log.SensorHistoryID);
                if (process != null)
                {
                    process.LastLog = log;
                    process.DetectTime = log.LogTime;

                    int nAlarmLevel = 2;
                    int.TryParse(log.Parameter5, out nAlarmLevel);
                    process.AlarmLevel = nAlarmLevel;

                    FormMain.Instance.Invoke((MethodInvoker)delegate
                    {
                        FormMain.Instance.ChangeTab(UnE.View.Content.ContentOwnerTab.CCTV_TAB);
                        FormMain.Instance.AddSensorDectect(process, !bAddOnly);
                    });
                }
                return process;
            }
            
            if (sensor != null)
            {
                ProcessType type = GetProcessType(sensor.Type);
                SensorHistoryManager.Instance.AddSensorHistory(log);
                
                /*** 같은 EquipmentZone인 센서인 경우 기존 EquipmentZone에 대한 Process를 수행 : by hypark ***/
                ProcessIF sameEquipZoneProcess = ProcessManager.Instance.FindSameEquipmentZoneProcess(sensor.EquipZoneID);
                if (sameEquipZoneProcess != null)
                {
                    ProcessManager.Instance.RemoveSleepProcess(nSensorZoneID);           //혹시 있을지 모를 프로세스를 지운다. 
                    ProcessIF process = ProcessManager.Instance.MakeSleepProcess(sensor, log, type, !bAddOnly);     //sleepDetectProcess에 add 까지 캡슐화
                    //MainForm.Instance.SendSensorSameSensorGroupRunningToSOPSimulator(process.SensorHistoryID, sameEquipZoneProcess.SensorHistoryID);
                    return sameEquipZoneProcess;
                }

                ProcessIF beginProcess = ProcessManager.Instance.BeginProcess(sensor, log, type, !bAddOnly);
                return beginProcess;
            }
            
            return null;
        }
        /// <summary>
        /// 해당 History id에 대한 프로세스 종료
        /// </summary>
        /// <param name="nSensorHistoryID"></param>
        private void EndProcess(int nSensorHistoryID)
        {
            //SendSensorCloseToSOPSimulator(nSensorHistoryID);

            ProcessIF process = ProcessManager.Instance.FindProcess(nSensorHistoryID);
            EndProcess(process);
        }

        /// <summary>
        /// Clear Sensor Data 에서 사용되는 프로세스 종료및 제거 - 상황종료
        /// </summary>
        /// <param name="bytes"></param>
        private void EndProcess(byte[] bytes)
        {
            int nSensorHistoryID = BitConverter.ToInt32(bytes, 11);
            ProcessIF process = ProcessManager.Instance.FindProcess(nSensorHistoryID);
            if (process != null && process.ToString() != "")
            {
                Debug.WriteLine("EndProcess :" + nSensorHistoryID.ToString());
                Debug.WriteLine("EndProcess :" + process);
                EndProcess(process);
                RemoveProcess(process);
            }
        }
        /// <summary>
        /// 해당 프로세스를 종료 시키고 프로세스의 HistoryID에대해 정상 모드로 변경
        /// </summary>
        /// <param name="process"></param>
        public static void EndProcess(ProcessIF process)
        {
            if (process != null)
            {
                int nHistoryID = process.SensorHistoryID;
                int nSensorID = process.TargetSensor.ID;
                ProcessManager.Instance.EndProcess(process);

                FormMain.Instance.Invoke((MethodInvoker)delegate
                {
                    //ConfirmDialogManager.Instance.RemoveDialog(nHistoryID, nSensorID);
                });

                if (ProcessManager.Instance.CurrentDetectProcess.Count == 0)
                {
                    FormMain.Instance.Invoke((MethodInvoker)delegate
                    {
                        //ConfirmDialogManager.Instance.CloseAllDialog();
                        //FormMain.Instance.SetNormalMode(process.SensorHistoryID);

                    });
                }

            }
            else
            {
                if (ProcessManager.Instance.CurrentDetectProcess.Count == 0)
                {
                    FormMain.Instance.Invoke((MethodInvoker)delegate
                    {
                        //ConfirmDialogManager.Instance.CloseAllDialog();
                        //MainForm.Instance.SetNormalMode(0);
                    });
                }
            }
        }
        public static void RemoveProcess(ProcessIF process)
        {
            if (process != null)
            {
                if (process.TargetSensor != null)
                    process.TargetSensor.SoundOn = true;


                // 참조하지 않도록 프로제스 목록에서 제거
                ProcessManager.Instance.RemoveProcess(process);
                // 해당 History ID 제거
                SensorHistoryManager.Instance.RemoveSensorHistory(process.SensorHistoryID);
                // Combo박스에서 제거
                FormMain.Instance.Invoke((MethodInvoker)delegate
                {
                    FormMain.Instance.RemoveSensorDetect(process);

                    if (ProcessManager.Instance.CurrentDetectProcess.Count == 0)
                    {
                        //MainForm.Instance.SetNormalMode(0);

                        if (UnE.SOP.ProxySOP.Instance.ShowCCTVForm)
                        {
                            //MainForm.Instance.CCTVPipe.Send("ShowDefaultCCTV()");
                        }

                    }
                });
            }
        } 
        #endregion

        #region TCP
        public static ArrayList ReadBytes(byte[] bytes, out short nHeader)
        {
            nHeader = 0;

            int nLength = bytes.Length;

            if (nLength < 6)
                return null;

            nHeader = BitConverter.ToInt16(bytes, 0);
            int nChunkCount = BitConverter.ToInt32(bytes, 2);

            ArrayList arrResult = new ArrayList();
            int nIndex = 6;
            bool isNullData;

            for (int i = 0; i < nChunkCount; i++)
            {
                if (nLength <= nIndex)
                    return null;

                byte type = bytes[nIndex];

                if (type == TcpLib2.TCP_TYPE.INTEGER)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 9, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        int nData = BitConverter.ToInt32(bytes, nIndex - 4);
                        arrResult.Add(nData);
                    }
                }
                else if (type == TcpLib2.TCP_TYPE.FLOAT)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 9, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        float fData = BitConverter.ToSingle(bytes, nIndex - 4);
                        arrResult.Add(fData);
                    }
                }
                else if (type == TcpLib2.TCP_TYPE.DOUBLE)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 13, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        double dData = BitConverter.ToDouble(bytes, nIndex - 8);
                        arrResult.Add(dData);
                    }
                }
                else if (type == TcpLib2.TCP_TYPE.LONG)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 13, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        long lData = BitConverter.ToInt64(bytes, nIndex - 8);
                        arrResult.Add(lData);
                    }
                }
                else if (type == TcpLib2.TCP_TYPE.BOOLEAN)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 6, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        bool bData = BitConverter.ToBoolean(bytes, nIndex - 1);
                        arrResult.Add(bData);
                    }
                }
                else if (type == TcpLib2.TCP_TYPE.SHORT)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 7, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        short sData = BitConverter.ToInt16(bytes, nIndex - 2);
                        arrResult.Add(sData);
                    }
                }
                else if (type == TcpLib2.TCP_TYPE.BYTE)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 6, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        byte data = bytes[nIndex - 1];
                        arrResult.Add(data);
                    }
                }
                else if (type == TcpLib2.TCP_TYPE.STRING)
                {
                    if (nLength < nIndex + 5)
                        return null;

                    int nDataLength = BitConverter.ToInt32(bytes, nIndex + 1);

                    if (nDataLength < 0)
                        return null;
                    else if (nDataLength > 0)
                    {
                        if (nLength < nIndex + 5 + nDataLength)
                            return null;

                        string strData = Encoding.UTF8.GetString(bytes, nIndex + 5, nDataLength);
                        arrResult.Add(strData);

                        nIndex += 5 + nDataLength;
                    }
                    else
                    {
                        arrResult.Add("");
                        nIndex += 5;
                    }
                }
                else
                    return null;
            }

            return arrResult;
        }
        private static bool ReadType(byte[] bytes, int nBytesLength, ref int nIndex, int nTotalLength, out bool isNullData)
        {
            isNullData = false;

            if (nBytesLength < nIndex + 5)
                return false;

            int nDataLength = BitConverter.ToInt32(bytes, nIndex + 1);

            if (nDataLength < 0)
                return false;
            else if (nDataLength > 0)
            {
                if (nBytesLength < nIndex + nTotalLength)
                    return false;

                nIndex += nTotalLength;
            }
            else
            {
                isNullData = true;
                nIndex += 5;
            }

            return true;
        }
        public static byte[] MakeBytes(short nHeader, ArrayList arrDatas)
        {
            int nChunkCount = arrDatas == null ? 0 : arrDatas.Count;

            ArrayList arrBytes = new ArrayList();
            int nBytesCount = 0;

            for (int i = 0; i < nChunkCount; i++)
            {
                object data = arrDatas[i];
                Type type = data.GetType();
                byte[] bytes = null;

                if (type == typeof(int))
                    bytes = SOPWebServer.BinaryHelper.MakeBytes((int)data);
                else if (type == typeof(long))
                    bytes = SOPWebServer.BinaryHelper.MakeBytes((long)data);
                else if (type == typeof(float))
                    bytes = SOPWebServer.BinaryHelper.MakeBytes((float)data);
                else if (type == typeof(bool))
                    bytes = SOPWebServer.BinaryHelper.MakeBytes((bool)data);
                else if (type == typeof(double))
                    bytes = SOPWebServer.BinaryHelper.MakeBytes((double)data);
                else if (type == typeof(short))
                    bytes = SOPWebServer.BinaryHelper.MakeBytes((short)data);
                else if (type == typeof(byte))
                    bytes = SOPWebServer.BinaryHelper.MakeBytes((byte)data);
                else if (type == typeof(string))
                    bytes = SOPWebServer.BinaryHelper.MakeBytes((string)data);

                else
                    return null;

                nBytesCount += bytes.Length;
                arrBytes.Add(bytes);
            }

            byte[] _bytes = new byte[6 + nBytesCount];
            byte[] headerBytes = BitConverter.GetBytes(nHeader);
            byte[] lengthBytes = BitConverter.GetBytes(nChunkCount);

            _bytes[0] = headerBytes[0];
            _bytes[1] = headerBytes[1];
            _bytes[2] = lengthBytes[0];
            _bytes[3] = lengthBytes[1];
            _bytes[4] = lengthBytes[2];
            _bytes[5] = lengthBytes[3];

            int nIndex = 6;

            foreach (byte[] bytes in arrBytes)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    _bytes[nIndex + i] = bytes[i];
                }

                nIndex += bytes.Length;
            }

            return _bytes;
        }

        private bool m_exceptPingLog = true;
        public int Send(byte[] bytes, TcpLib2.ClientServiceProvider provider, int nNum)
        {
            lock (this)
            {
                int nResult = provider.Send(bytes, 0, bytes.Length);
                if (nResult > 0)
                {
                    if (!IsLogOpened)
                        return nResult;

                    if (bytes[0] != SOPWebServer.Header.I_AM_HERE || !m_exceptPingLog)
                    {
                        //int nNum = provider.ProviderNum;

                        string szRemotePort = "";
                        try
                        {
                            szRemotePort = provider.Client.Client.LocalEndPoint.ToString();
                        }
                        catch (System.Exception)
                        {
                        }
                        string strLog = string.Format("SendMessage : {0} Header({1}), Length({2}), SDMS({3}) ",
                            szRemotePort, (int)bytes[0], (int)bytes.Length, nNum);
                        string strBytes = "";

                        foreach (byte b in bytes)
                        {
                            if (strBytes.Length == 0)
                                strBytes = string.Format("\r\n\t\t{0:X2}", (int)b);
                            else
                                strBytes += string.Format(" {0:X2}", (int)b);
                        }

                        WriteLineLog(strLog + strBytes);
                    }
                }
                return nResult;
            }
        }
        #endregion

        public void SendChangeFacilityManager()
        {
            if (!m_isConnected)
                return;

            int nChangedConfig = (int)SOP.SDMSConfig.ConfigType.ENTIRE_FACILITY_MANAGER | (int)SOP.SDMSConfig.ConfigType.EQUIPZONE_FACILITY_MANAGER | (int)SOP.SDMSConfig.ConfigType.BUILDING_FACILITY_MANAGER;
            SendChangedConfig(SOPWebServer.ClientType.SDMS, SOP.SDMSConfig.PropertyName, nChangedConfig.ToString());
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
    }

    public class ConnectionLogEx : TcpLib2.ConnectionLog
    {
        private log4net.ILog logger = null;

        public static ConnectionLogEx Instance
        {
            get
            {
                return (ConnectionLogEx)m_instance;
            }
        }

        public static bool MakeInstance()
        {
            if (m_instance == null)
                m_instance = new ConnectionLogEx();

            ConnectionLogEx instance = (ConnectionLogEx)m_instance;
            instance.logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            instance.m_isOpened = true;
            return instance.m_isOpened;
        }

        public override bool Write(object str, bool writeTime = true)
        {
            if (logger != null)
                logger.DebugFormat("{0}", str);

            return true;
        }

        public override bool WriteLine(object str, bool writeTime = true)
        {
            if (logger != null)
                logger.Debug(str);

            return true;
        }
    }
}
