using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using TcpLib2;
using UnE.SOP;
using UnE.Spatial;
using UnE.Sensor;
using libSensorProcess;

namespace SDMS
{
    public class ClientProvider : ClientServiceProvider
    {
        private int m_nProviderNum = 0;

        public int ProviderNum
        {
            get { return m_nProviderNum; }
            set { m_nProviderNum = value; }
        }

        private int m_nProviderType = 1;

        public int ProviderType
        {
            get { return m_nProviderType; }
            set { m_nProviderType = value; }
        }

        private NetworkManager m_mgr = null;
        private int m_nPingCount = 0;
        private byte[] m_arrReceived = null;
        // OnReceive()에서 전달받는 데이터(ReceivedData)가 아직 완결되지 않은 Packet일 경우 다음 OnReceive() 호출시 데이터를
        // 합치기 위한 임시 버퍼
        //private byte[] m_arrTemp = null;

        // 현재 OnReceive()에서 받은 데이터를 처리중인가?
        private bool m_isReadingProcess = false;

        public bool IsReadingProcess
        {
            get { return m_isReadingProcess; }
        }

        public int PingCount
        {
            get { return m_nPingCount; }
            set { m_nPingCount = value; }
        }
        
        public delegate void changeAction();
        public event changeAction Change;

        public ClientProvider(NetworkManager mgr, int nID)
        {
            m_nProviderNum = nID;
            if (nID == 2)
                m_nProviderType = (int)TCP_CLIENT.SDMS_CLIENT_SECOND;
            else
                m_nProviderType = (int)TCP_CLIENT.SDMS_CLIENT;
            m_mgr = mgr;
            this.Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);
            this.Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);


            //libTrs.SetTrsNumber(100150003);
            //libTrs.SetLoginInfo("www.ptalk20.kr", "une0003", "ktp1234!");

            //libTrs.InitPtalk();

           timer1 = new System.Windows.Forms.Timer();

            timer1.Interval = 2000;         
            timer1.Tick += new System.EventHandler(this.timer1_Tick);

            timer1.Start();
        }
        System.Windows.Forms.Timer timer1;
        int nTimerCount = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            //if (nTimerCount == 0)
            //    libTrs.CallPrivate(100150005);

            if (nTimerCount == 1)
            {

                //libTrs.PttOff();
                //libTrs.CallEnd();
                timer1.Stop();
                timer1.Enabled = false;
            }


            nTimerCount++;
        }

        private void ProcessReactionHistoryLogList(byte[] bytes)
        {
            //System.Diagnostics.Trace.WriteLine("Call ProcessReactionHistoryLogList");

            ArrayList arrReactionLog = new ArrayList();

            int nChunkCount = (int)BitConverter.ToInt32(bytes, 2);

            int nLogChunkSize = 10;
            int nLogCount = (nChunkCount) / nLogChunkSize;

            if (nLogCount > 0)
            {
                int nOffset = 6;

                for (int i = 0; i < nLogCount; i++)
                {
                    ReactionLog log = ReadReactionHistoryLog(bytes, ref nOffset, nLogChunkSize);
                    if (log != null)
                        arrReactionLog.Add(log);
                }
            }

            ArrayList arrRemoveProcess = new ArrayList();
            ArrayList arrCurrentLog = new ArrayList();

            foreach (KeyValuePair<int, ProcessIF> pair in ProcessManager.Instance.CurrentDetectProcess)
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

                //if (ProcessManager.Instance.HistoryIDToSOPClosing > 0)
                //{
                //    ProcessManager.Instance.HistoryIDToSOPClosing = -1;
                //}
                //else
                //{
                //    SendSensorCloseToSOPSimulator(process.SensorHistoryID);
                //}
                //SendSensorCloseToSOPSimulator(process.SensorHistoryID);
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
                /*if (log.ReactionType == (int)ReactionType.BEGIN_STATUS ||
                    log.ReactionType == (int)ReactionType.BEGIN_PSM_STATUS ||
                    log.ReactionType == (int)ReactionType.CHANGE_PSM_ALARM_DEPTH  ||
                    log.ReactionType == (int)ReactionType.BEGIN_S1SVMS_STATUS ||
                    log.ReactionType == (int)ReactionType.BEGIN_S1ACCESS_STATUS
                    )*/
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
                            /*try
                            {
                                int nLevel = Convert.ToInt32(log.Parameter5);
                                process.AlarmLevel = nLevel;
                            }
                            catch (Exception)
                            { }*/
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
                else if (info == ReactionTypeInfo.NOTIFY || log.ReactionType == (int)ReactionType.TRAINNING_FIRE)
                /*else if (log.ReactionType == (int)ReactionType.NOTIFY_FIRE
                    || log.ReactionType == (int)ReactionType.TRAINNING_FIRE
                    || log.ReactionType == (int)ReactionType.NOTIFY_PSM
                    || log.ReactionType == (int)ReactionType.NOTIFY_SECURITY
                    )*/
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
                            /*if (log.ReactionType == (int)ReactionType.NOTIFY_PSM)
                            {
                                try
                                {
                                    int nLevel = Convert.ToInt32(log.Parameter5);
                                    process.AlarmLevel = nLevel;
                                }
                                catch (Exception)
                                { }
                            }*/

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
                //else if (log.ReactionType != (int)ReactionType.END_STATUS && log.ReactionType != (int)ReactionType.END_PSM_STATUS)
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
                    FormMain.Instance.SelectSensorDetectProcess(
                        lastAlarmProcess.SensorHistoryID, lastAlarmProcess.DetectSensorID
                        );

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

        // 무시할 알람인지 검사한다.
        // Return값이 false이면 해당 알람은 무시한다.
        private bool CheckIgnoreAlarm(ReactionLog log)
        {
            if (log == null)
                return false;

            if (log.ReactionType == (int)ReactionType.BEGIN_PSM_STATUS ||
                log.ReactionType == (int)ReactionType.CHANGE_PSM_ALARM_DEPTH ||
                log.ReactionType == (int)ReactionType.NOTIFY_PSM)
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

            if (log.ReactionType == (int)ReactionType.BEGIN_STATUS ||
                log.ReactionType == (int)ReactionType.NOTIFY_FIRE ||
                log.ReactionType == (int)ReactionType.TRAINNING_FIRE)
            {

                // 화재알람 수신거부일경우 알람 무시
                bool bRecive = PreferenceManager.Instance.ReciveFireSignal;
                if (bRecive == false)
                    return false;
            }

            // TODO 수현 : 방범신호 수신거부 기능 추가하기
            if (log.ReactionType == (int)ReactionType.BEGIN_SECOM_STATUS ||
                log.ReactionType == (int)ReactionType.NOTIFY_SECURITY)
            {

                // 화재알람 수신거부일경우 알람 무시
                bool bRecive = PreferenceManager.Instance.ReciveSecuritySignal;
                if (bRecive == false)
                    return false;
            }

            return true;
        }

        private void AddProcess(ReactionLog log, bool bAddSelected = true)
        {

            string strSQL = string.Format("select szh.SensorID, sz.Type, sz.OrgSensorID, fs.X, fs.Y, fs.Z, sz.EquipZoneID from SensorZoneHistory as szh, SensorZone as sz, FireSensor as fs where szh.ID = {0} and szh.SensorID = sz.ID and sz.OrgSensorID = fs.ID",
                    log.SensorHistoryID);
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count < 7)
                return;

            int nSensorZoneID = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            int nSensorType = DBUtility.WebDBManager.GetIntField(arrResult[1].ToString(), -1);
            int nSensorID = DBUtility.WebDBManager.GetIntField(arrResult[2].ToString(), -1);
            float x = DBUtility.WebDBManager.GetFloatField(arrResult[3].ToString(), 0.0f);
            float y = DBUtility.WebDBManager.GetFloatField(arrResult[4].ToString(), 0.0f);
            float z = DBUtility.WebDBManager.GetFloatField(arrResult[5].ToString(), 0.0f);
            int nEquipZoneID = DBUtility.WebDBManager.GetIntField(arrResult[6].ToString(), -1);

            if (!ProcessManager.Instance.CurrentDetectProcess.ContainsKey(nSensorZoneID))
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

                ProcessManager.Instance.CurrentDetectProcess[nSensorZoneID] = process;

                FormMain.Instance.Invoke((MethodInvoker)delegate
                {
                    FormMain.Instance.AddSensorDectect(process, bAddSelected);
                });
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
       
        private ReactionLog ReadReactionHistoryLog(byte[] bytes, ref int nOffset, int chunkSize)
        {
            ReactionLog log = new ReactionLog();

            int nLogID = -1;
            int nSensorHistoryID = -1;
            int nReadDataCount = nOffset;

            // Reaction History ID
            byte dataHeader = bytes[nReadDataCount++];
            int nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
            nReadDataCount += 4;
            if (dataHeader == TCP_TYPE.INTEGER)
            {
                nLogID = BitConverter.ToInt32(bytes, nReadDataCount);
                nReadDataCount += 4;
            }
            chunkSize -= 1;
            log.ID = nLogID;

            // Sensor History ID
            dataHeader = bytes[nReadDataCount++];
            nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
            nReadDataCount += 4;
            if (dataHeader == TCP_TYPE.INTEGER)
            {
                nSensorHistoryID = BitConverter.ToInt32(bytes, nReadDataCount);
                nReadDataCount += 4;
            }
            chunkSize -= 1;
            log.SensorHistoryID = nSensorHistoryID;

            // Reaction Type
            int nReactionType = -1;
            dataHeader = bytes[nReadDataCount++];
            nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
            nReadDataCount += 4;
            if (dataHeader == TCP_TYPE.INTEGER)
            {
                nReactionType = BitConverter.ToInt32(bytes, nReadDataCount);
                nReadDataCount += 4;
            }
            chunkSize -= 1;
            log.ReactionType = nReactionType;

            for (int i = 0; i < chunkSize; i++)
            {
                dataHeader = bytes[nReadDataCount++];
                nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
                nReadDataCount += 4;

                // Message , Parameter1
                if (dataHeader == TCP_TYPE.STRING)
                {
                    byte[] bytesBlock = new byte[nDataLength];
                    System.Buffer.BlockCopy(bytes, nReadDataCount, bytesBlock, 0, nDataLength);
                    string szValue = Encoding.UTF8.GetString(bytesBlock, 0, nDataLength);
                    nReadDataCount += nDataLength;

                    switch (i)
                    {
                        case 1: //  Message
                            log.Message = szValue;
                            break;

                        case 2: // Param 1
                            log.Parameter1 = szValue;
                            break;

                        case 3: // Param 2
                            log.Parameter2 = szValue;
                            break;

                        case 4: // Param 3
                            log.Parameter3 = szValue;
                            break;

                        case 5: // Param 4
                            log.Parameter4 = szValue;
                            break;

                        case 6: // Param 5
                            log.Parameter5 = szValue;
                            break;
                    }
                }
                // LogTime
                if (dataHeader == TCP_TYPE.LONG)
                {
                    long value = BitConverter.ToInt64(bytes, nReadDataCount);

                    nReadDataCount += nDataLength;
                    if (value == 0)
                        log.LogTime = DateTime.Now;
                    else
                        log.LogTime = DateTime.FromBinary(value);
                }
            }

            nOffset = nReadDataCount;
            return log;
        }

        private ReactionLog ReadReactionHistoryLog(byte[] bytes)
        {
            int nOffset = 6;
            int chunkSize = (int)BitConverter.ToInt16(bytes, 2);
            return ReadReactionHistoryLog(bytes, ref nOffset, chunkSize);
        }

        public override void OnReceiveData()
        {
            OnReceive(ReceivedData);
        }

        private bool OnReceive(byte[] bytes)
        {
            try
            {
                if (bytes != null)
                {
                    m_isReadingProcess = true;

                    m_arrReceived = bytes;

                    int nBytesCount = m_arrReceived.Count();

                    if (nBytesCount > 0)
                    {
                        // 이미 종료되었어야 할 접속이 유지되고 있는 경우는 해당 접속을 강제로 종료시킨다.
                        if (m_mgr.ClientProvider != this && m_mgr.ClientProviderSub != this)
                        {
                            this.Close();
                            m_isReadingProcess = false;
                            return true;
                        }

                        m_nPingCount = 0;
                        SendData(TCP_ID.I_AM_HERE);

                        m_mgr.RecvLog(m_arrReceived, m_nProviderNum);

                        short nHeader;
                        ArrayList arrDatas = ReadBytes(m_arrReceived, out nHeader);

                        if (nHeader == TCP_ID.ARE_YOU_THERE)
                        {
                            //SendData(TCP_ID.I_AM_HERE);
                        }
                        else if (nHeader == TCP_ID.WHO_ARE_YOU)
                        {
                            SendData(TCP_ID.WHO_I_AM, TCP_TYPE.INTEGER, BitConverter.GetBytes(m_nProviderType));
                        }
                        else if (nHeader == TCP_ID.SENSOR_REACTION_HISTORY_DATA)
                        {
                            bool escapeLoop;
                            bool result = ProcessSensorReactionSensorHistoryData(out escapeLoop);

                            if (escapeLoop)
                                return result;
                            /*lock (m_mgr.Lock)
                            {
                                ReactionLog log = ReadReactionHistoryLog(m_arrReceived);

                                if (log.ReactionType == (int)ReactionType.BEGIN_STATUS || log.ReactionType == (int)ReactionType.BEGIN_PSM_STATUS)
                                {
                                    BeginProcess(log);
                                }
                                else if (log.ReactionType == (int)ReactionType.MALFUNCTION)
                                {
                                    EndProcess(log.SensorHistoryID);
                                    m_isReadingProcess = false;
                                    return true;
                                }
                                else if (log.ReactionType == (int)ReactionType.NOTIFY_FIRE)
                                {
                                    if (FireDetectProcess.SoundPlayer.IsLoadCompleted == true)
                                    {
                                        FireDetectProcess.SoundPlayer.Stop();
                                    }
                                    // 수동 신고
                                    if (log.Parameter2 == "0")
                                    {
                                        Debug.WriteLine("Recive Manual Report Log");
                                        BeginProcess(log);
                                        //FormMain.Instance.SendFireDetectMessageToSOPSimulator();
                                    }
                                }

                                if (log.ReactionType != (int)ReactionType.BEGIN_STATUS && log.ReactionType != (int)ReactionType.BEGIN_PSM_STATUS)
                                {
                                    DlgSelectCase.ProcessingSensorHistoryID = log.SensorHistoryID;
                                }
                                ReactionLogManager.Instance.AddLog(log);
                            }*/
                        }
                        else if (nHeader == TCP_ID.CLEAR_DETECT_REPORT)
                        {
                            lock (m_mgr.Lock)
                            {
                                ProcessClearProcess(m_arrReceived);
                            }
                        }
                        else if (nHeader == TCP_ID.SENSOR_REACTION_HISTORY_DATA_LIST)
                        {
                            lock (m_mgr.Lock)
                            {
                                ProcessReactionHistoryLogList(m_arrReceived);
                            }
                        }
                        else if (nHeader == TCP_ID.SENSOR_ZONE_DATA)
                        {
                            lock (m_mgr.Lock)
                            {
                                ProcessSensorData(m_arrReceived);
                            }
                        }
                        else if (nHeader == TCP_ID.IGNORE_DETECT_REPORT)
                        {
                            lock (m_mgr.Lock)
                            {
                                ProcessIgnoreDetect(m_arrReceived);
                            }
                        }
                        else if (nHeader == TCP_ID.ALL_RECIVER_STATE)
                        {
                            ProcessAllReciverState(arrDatas);
                        }
                        else if (nHeader == TCP_ID.RECIVER_CONNECT || nHeader == TCP_ID.RECIVER_DISCONNECT)
                        {
                            int nReciverID = BitConverter.ToInt32(m_arrReceived, 11);
                            int nConnected = BitConverter.ToInt32(m_arrReceived, 20);
                            ProcessReciverState(nReciverID, nConnected);
                        }
                        else if (nHeader == TCP_ID.CHANGE_CONFIG)
                        {
                            ProcessChangeConfig(arrDatas);
                        }
                        else if (nHeader == TCP_ID.WEATHER_INFO)
                            ProcessWeatherInfo(arrDatas);
                        else if (nHeader == TCP_ID.EDIT_SENSOR_ZONE)
                        {
                            ProcessEditSensorZone(arrDatas);
                        }
                        else if (nHeader == TCP_ID.SDMS_COMMAND)
                            ProcessSDMSCommand(arrDatas);
                        else if (nHeader == TCP_ID.SERVER_COMMAND)
                            ProcessServerCommand(arrDatas);
                        else if (nHeader == TCP_ID.EARTHQUAKE_SENSOR_DETECT)
                            ProcessEarthquake(arrDatas);
                        else if (nHeader == TCP_ID.COLLAPSE_BUILDING_DETECT)
                            ProcessCollapseBuilding(arrDatas);
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }

            m_isReadingProcess = false;
            return true;
        }

        private void ProcessCollapseBuilding(ArrayList arrDatas)
        {
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
                            FormMain.Instance.PageHome.ContentForm.ZoomBuilding(building.BuildingID);
                            FormMain.Instance.PageHome.ContentForm.SelectBuilding(building.BuildingID);
                            FormMain.Instance.SetBuilingCollapseDetect(strBuildingName, isReal);
                        }
                    }
                });
            }
        }

        private void ProcessEarthquake(ArrayList arrDatas)
        {
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
                DBUtility.VariousData<DateTime> time = (long)arrDatas[5] == 0 ? null : new DBUtility.VariousData<DateTime>(DateTime.FromBinary((long)arrDatas[5]));

                // SDMS 지진 이벤트를 발생시킨다.
                FormMain.Instance.PageHome.ContentForm.EarthquakeEvent(nIntensity, fMagnitude, strPosition, true);
                //SDMS.ScriptProxy.Instance.UserObject.SDMSEarthquakeEvent.Invoke(nIntensity, fMagnitude, strPosition, false);
            }
        }

        private void ProcessServerCommand(ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;

            if (nDataCount == 0 || arrDatas[0].GetType() != typeof(byte))
                return;

            byte command = (byte)arrDatas[0];

            if (command == ServerCommandType.DELETE_SENSOR_TAG_HISTORY)
            {
                if (nDataCount < 2 || arrDatas[1].GetType() != typeof(long))
                    return;

                long time = (long)arrDatas[1];
                SensorTagHistoryManager.Instance.ProcessDeleteSensorTagHistory(DateTime.FromBinary(time));
            }
        }

        // escapeLoop : true이면 OnReceive() 루프를 즉시 빠져나올 것
        private bool ProcessSensorReactionSensorHistoryData(out bool escapeLoop)
        {
            escapeLoop = false;

            lock (m_mgr.Lock)
            {
                ReactionLog log = ReadReactionHistoryLog(m_arrReceived);
                //int nLogType = GetReactionLogType(log.ReactionType);

                //if (nLogType == 0)
                ProcessBeginStatusReactionLog(log, ref escapeLoop);
                //else if (nLogType == 1)
                //    ProcessPSMReactionLog(log, ref escapeLoop);

                /*if (log.ReactionType == (int)ReactionType.BEGIN_STATUS)
                {
                    BeginProcess(log);
                }
                else if (log.ReactionType == (int)ReactionType.MALFUNCTION)
                {
                    EndProcess(log.SensorHistoryID);
                    m_isReadingProcess = false;
                    escapeLoop = true;
                    return true;
                }
                else if (log.ReactionType == (int)ReactionType.NOTIFY_FIRE)
                {
                    if (FireDetectProcess.SoundPlayer.IsLoadCompleted == true)
                    {
                        FireDetectProcess.SoundPlayer.Stop();
                    }
                    // 수동 신고
                    if (log.Parameter2 == "0")
                    {
                        Debug.WriteLine("Recive Manual Report Log");
                        BeginProcess(log);
                        //FormMain.Instance.SendFireDetectMessageToSOPSimulator();
                    }
                }

                if (log.ReactionType != (int)ReactionType.BEGIN_STATUS)
                {
                    DlgSelectCase.ProcessingSensorHistoryID = log.SensorHistoryID;
                }
                ReactionLogManager.Instance.AddLog(log);*/
            }

            return true;
        }
        //UnE.TRS.PTalkLib libTrs = new UnE.TRS.PTalkLib();

        private bool ProcessBeginStatusReactionLog(ReactionLog log, ref bool escapeLoop)
        {
            // 무시할 알람인지 검사한다.
            if (CheckIgnoreAlarm(log) == false)
                return true;

            ReactionTypeInfo info = log.GetReactionTypeInfo();

            if (info == ReactionTypeInfo.BEGIN_STATUS)
            /*if (log.ReactionType == (int)ReactionType.BEGIN_STATUS || 
                log.ReactionType == (int)ReactionType.BEGIN_PSM_STATUS ||
                log.ReactionType == (int)ReactionType.BEGIN_S1ACCESS_STATUS ||
                log.ReactionType == (int)ReactionType.BEGIN_S1SVMS_STATUS
                )*/
            {


                string szTemp = log.Message.Replace("\"", ",,,");
                string szMsg = ",,,,,,,,,," + szTemp.Replace(".", ",,,");

                //libTrs.SendGroupTTS(1, szMsg);
                BeginProcess(log);
            }
            
            if (info == ReactionTypeInfo.CHANGE_ALARM_LEVEL)
            //if (log.ReactionType == (int)ReactionType.CHANGE_PSM_ALARM_DEPTH)
            {
                ProcessIF process = ProcessManager.Instance.FindProcess(log.SensorHistoryID);
                if (process != null)
                {
                    // PSM의 경우 ChangeDepth에서 Alarm값을 설정하여 준다. 대피반경에서 사용.
                    // added by skkim 2016-04-07
                    //if (log.ReactionType == (int)ReactionType.CHANGE_PSM_ALARM_DEPTH)
                    {
                        process.SetAlarmLevel(log);
                        /*try
                        {
                            int nLevel = Convert.ToInt32(log.Parameter5);
                            process.AlarmLevel = nLevel;
                        }
                        catch (Exception)
                        { }*/
                    }
                }
            }
            else if (info == ReactionTypeInfo.USER_RESET)
            {
                string szTemp = log.Message.Replace("\"", ",,,");
                string szMsg = ",,,,,,,,,," + szTemp.Replace(".", ",,,");

                //libTrs.SendGroupTTS(1, szMsg);

                EndProcess(log.SensorHistoryID);
                m_isReadingProcess = false;
                escapeLoop = true;

                return true;
            }
            /*else if( log.ReactionType == (int)ReactionType.PSM_USER_RESET)
            {
                string szTemp = log.Message.Replace("\"", ",,,");
                string szMsg = ",,,,,,,,,," + szTemp.Replace(".", ",,,");

                //libTrs.SendGroupTTS(1, szMsg);
            }
            else if (log.ReactionType == (int)ReactionType.MALFUNCTION)
            {

                string szTemp = log.Message.Replace("\"", ",,,");
                string szMsg = ",,,,,,,,,," + szTemp.Replace(".", ",,,");
               
                //libTrs.SendGroupTTS(1, szMsg);

                EndProcess(log.SensorHistoryID);
                m_isReadingProcess = false;
                escapeLoop = true;


                
                

                return true;
            }*/
            else if (info == ReactionTypeInfo.NOTIFY)
            /*else if (log.ReactionType == (int)ReactionType.NOTIFY_FIRE || 
                     log.ReactionType == (int)ReactionType.NOTIFY_PSM ||
                     log.ReactionType == (int)ReactionType.NOTIFY_SECURITY
                )*/
            {
                if (FireDetectProcess.SoundPlayer.IsLoadCompleted == true)
                {
                    FireDetectProcess.SoundPlayer.Stop();
                }
                // 수동 신고
                if (log.Parameter2 == "0")
                {
                    Debug.WriteLine("Recive Manual Report Log");
                    BeginProcess(log);
                    //FormMain.Instance.SendFireDetectMessageToSOPSimulator();
                }
            }

            // 현재 진행중인 화재가 변경되지 않는것이므로 Change PSM alarm depth는 진행중인 historyid를 변경하지 않는다.
            if (info != ReactionTypeInfo.BEGIN_STATUS && info != ReactionTypeInfo.CHANGE_ALARM_LEVEL)
            /*if ( log.ReactionType != (int)ReactionType.BEGIN_STATUS &&
                 log.ReactionType != (int)ReactionType.BEGIN_PSM_STATUS &&
                 log.ReactionType != (int)ReactionType.CHANGE_PSM_ALARM_DEPTH &&
                 log.ReactionType != (int)ReactionType.BEGIN_S1ACCESS_STATUS &&
                 log.ReactionType != (int)ReactionType.BEGIN_S1SVMS_STATUS
                )*/
            {
                DlgSelectCase.ProcessingSensorHistoryID = log.SensorHistoryID;
            }

            ReactionLogManager.Instance.AddLog(log);

            return true;
        }

        //private bool ProcessPSMReactionLog(ReactionLog log, ref bool escapeLoop)
        //{
        //    if (log.ReactionType == (int)ReactionType.BEGIN_PSM_STATUS)
        //    {
        //        BeginPSMProcess(log);
        //    }
        //}

        // Return 값 : 0(화재 탐지), 1(유해화학물질 누출)
        /*private int GetReactionLogType(int nReactionType)
        {
            if (nReactionType >= (int)ReactionType.BEGIN_STATUS && nReactionType <= (int)ReactionType.END_STATUS)
                return 0;
            else if (nReactionType >= (int)ReactionType.BEGIN_PSM_STATUS && nReactionType <= (int)ReactionType.END_PSM_STATUS)
                return 1;

            return -1;
        }*/


        



        private void ProcessSDMSCommand(ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;

            if (nDataCount == 0 || (arrDatas[0] is byte) == false)
                return;

            byte cmd = (byte)arrDatas[0];
            Debug.WriteLine("SDMS CommandType : " + cmd);
            if (cmd == SDMSCommandType.CHANGE_PSM_SENSOR_STATUS)
            {
                ProcessChangePSMSensorStatus(arrDatas);
            }
            else if (cmd == SDMSCommandType.REFRESH_PSM_SENSOR_LIFE_TIME)
                ProcessRefreshPSMSensorLifeTime(arrDatas);
            else if (cmd == SDMSCommandType.SDMS_PUBLIC_MESSAGE)
                ProcessSDMSPublicMessage(arrDatas, nDataCount);
            else if (cmd == SDMSCommandType.SDMS_PUBLIC_MESSAGE_ID)
                ProcessSDMSPublicMessageID(arrDatas, nDataCount);
            else if (cmd == SDMSCommandType.PSM_SENSOR_ALARM_LEVEL)
                ProcessPSMAlarmLevel(arrDatas, nDataCount);
            else if (cmd == SDMSCommandType.SET_VIEW)
            {
                ProcessSetView(arrDatas, nDataCount);
            }
            else if (cmd == SDMSCommandType.CHANGE_TAG_ACTIVATION)
            {
                Change();
            }
        }

        


        private void ProcessSetView(ArrayList arrDatas, int nDataCount)
        {
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

        private void ProcessPSMAlarmLevel(ArrayList arrDatas, int nDataCount)
        {
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

        private void ProcessSDMSPublicMessageID(ArrayList arrDatas, int nDataCount)
        {
            if (nDataCount < 2)
                return;

            if ((arrDatas[1] is int) == false)
                return;

            int nMessageID = (int)arrDatas[1];
            FormMain.Instance.ReadSDMSMessage(nMessageID);
        }

        private void ProcessSDMSPublicMessage(ArrayList arrDatas, int nDataCount)
        {
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

        private void ProcessRefreshPSMSensorLifeTime(ArrayList arrDatas)
        {
            SDMS.PopupDialog.FormPSMList frmPSMList = SDMS.PopupDialog.FormPSMList.Instance;

            if (frmPSMList != null && frmPSMList.IsDisposed == false)
                frmPSMList.RefreshSensorLifeTime();
        }

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

        private void ProcessWeatherInfo(ArrayList arrDatas)
        {
            FormMain.Instance.UpdateWeatherInfo();
        }

        private void ProcessChangeConfig(ArrayList arrDatas)
        {
            if (arrDatas == null)
                return;

            if (arrDatas.Count < 3)
                return;

            try
            {
                byte byteClientType = (byte)arrDatas[0];
                string strPropertyName = (string)arrDatas[1];
                string strPropertyValue = (string)arrDatas[2];

                if (byteClientType != TCP_CLIENT.SDMS_CLIENT)
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
            catch (Exception e)
            {
                ConnectionLogEx.Instance.WriteLine(e.StackTrace);
            }
        }

        private int GetReciverID(byte[] bytes, int nIdx = 0)
        {
            int nData = BitConverter.ToInt32(bytes, nIdx);
            return nData;
        }

        private bool IsReciverConnected(byte[] bytes, int nIdx = 0)
        {
            int nData = BitConverter.ToInt32(bytes, nIdx);
            if (nData == 1)
                return true;

            return false;
        }

        private void ProcessAllReciverState(ArrayList arRecivers)
        {
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
                this.PingCount = 0;
            }
        }

        private void ProcessReciverState(int nReciverID, int nConnect)
        {

            ReciverManager.Instance.UpdateState(nReciverID, (nConnect > 0 ? true : false), (nConnect > 10 ? true : false));
        }

        private void ProcessChangeEquipZoneCCTV(int nEquipZoneID)
        {
            FormMain.Instance.PageHome.Invoke((MethodInvoker)delegate
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
        }

        private void ProcessChangeCompanyMember()
        {
            FormMain.Instance.PageHome.Invoke((MethodInvoker)delegate
            {
                FormMain.Instance.DataManager.ReloadCompanyMember();
            });
        }

        private void ProcessChangeFacilityManager()
        {
            FormMain.Instance.PageHome.Invoke((MethodInvoker)delegate
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
        }

        private void ProcessIgnoreDetect(byte[] bytes)
        {
            int nSensorHistoryID = BitConverter.ToInt32(bytes, 11);
            EndProcess(nSensorHistoryID);
        }

        /// <summary>
        /// 센서 접속정보 변경에 따른 이벤트 생성 함수
        /// </summary>
        /// <param name="bytes"></param>
        private void ProcessSensorData(byte[] bytes)
        {
            int nSensorZoneID = BitConverter.ToInt32(bytes, 11); // 11
            int nSensorType = BitConverter.ToInt32(bytes, 20); // 20
            int nConnected = BitConverter.ToInt32(bytes, 29); //
            int nZoneID = BitConverter.ToInt32(bytes, 38); //
            int nSensorData = BitConverter.ToInt32(bytes, 47);
            int nSensorID = BitConverter.ToInt32(bytes, 56);

            if (bytes.Length >= 66)
            {
                int wDircetion = BitConverter.ToInt32(bytes, 65);       //wind direction : 광교
                int wSpeed = BitConverter.ToInt32(bytes, 74);   


                FormMain.Instance.Invoke((MethodInvoker)delegate
                {
                    FormMain.Instance.PageHome.ChangeTab(UnE.View.Content.ContentOwnerTab.M3D_TAB);
                    
                    FormMain.Instance.PageHome.ContentForm.PushViewState(true);
				    FormMain.Instance.PageHome.ContentForm.HideZoneVolume();
                    FormMain.Instance.PageHome.ContentForm.HideEvacCircle();
                    
                    FormMain.Instance.PageHome.ContentForm.ShowPollutionView(wDircetion, wSpeed);
                    
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
                        System.Diagnostics.Trace.WriteLine("Send SOP SensorClose: " + nSensorID);
                        FormMain.Instance.SendSensorCloseMessageToSOPSimulator(nSensorID, nSensorHistoryID);
                    });
                }
            }
            catch (Exception)
            {
            }
           
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

        private void ProcessClearProcess(byte[] bytes)
        {
            
            int nSensorHistoryID = BitConverter.ToInt32(bytes, 11);
            //SendSensorCloseToSOPSimulator(nSensorHistoryID);
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
                    FormMain.Instance.SetNormalMode(0);
                    PageBackstageHome.Instance.ContentForm.HideZoneVolume();
                    ConnectionLogEx.Instance.WriteLine("Hide All Zone Volume");
                    PageBackstageHome.Instance.ContentForm.HidePollutioinView();
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

        public void ProcessEditSensorZone(ArrayList arDatas)
        {
            int nSensorZoneID = (int)arDatas[0];
            int nOrgEquipZoneID = (int)arDatas[1];
            int nNewEquipZoneID = (int)arDatas[2];
            int nNewZoneID = (int)arDatas[3];

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
                            szTarget.m_ZoneID = nNewZoneID;
                            szTarget.m_ZoneIDDB = nNewZoneID;

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
                        targetSensor.m_ZoneID = nNewZoneID;
                        targetSensor.m_ZoneIDDB = nNewZoneID;
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
                                szTarget.m_ZoneID = nNewZoneID;
                                szTarget.m_ZoneIDDB = nNewZoneID;

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
                    ConfirmDialogManager.Instance.RemoveDialog(nHistoryID, nSensorID);
                });
                
                if (ProcessManager.Instance.CurrentDetectProcess.Count == 0)
                {
                    FormMain.Instance.Invoke((MethodInvoker)delegate
                    {
                        ConfirmDialogManager.Instance.CloseAllDialog();
                        FormMain.Instance.SetNormalMode(process.SensorHistoryID);

                    });
                }
                
            }
            else
            {
                if (ProcessManager.Instance.CurrentDetectProcess.Count == 0)
                {
                    FormMain.Instance.Invoke((MethodInvoker)delegate
                    {
                        ConfirmDialogManager.Instance.CloseAllDialog();
                        FormMain.Instance.SetNormalMode(0);
                    });
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
                        FormMain.Instance.SetNormalMode(0);

                        if (UnE.SOP.ProxySOP.Instance.ShowCCTVForm)
                        {
                            FormMain.Instance.CCTVPipe.Send("ShowDefaultCCTV()");
                        }

                    }
                });
            }
        }

        private void BeginPSMProcess(ReactionLog log)
        {
            //int nSensorZoneID = -1;

            //string strLocationName = log.Parameter1;

            //if (!int.TryParse(log.Parameter2, out nSensorZoneID))
            //    return;

            //EquipmentZone zone = null;
            //ISensor sensor = null;

            //if (!SensorManager.Instance.DicAllSenor.TryGetValue(nSensorZoneID, out sensor))
            //    return;

            //sensor.EquipZoneID

            //if (nZoneID > 0)
            //    zone = ZoneManager.Instance.GetEquipZone(nZoneID);

            //if (nSensorZoneID > 0)
            //{
            //    if (SensorManager.Instance.DicAllSenor.ContainsKey(nSensorZoneID))
            //    {
            //        sensor = SensorManager.Instance.DicAllSenor[nSensorZoneID];
            //    }
            //}
            //else // 수동 신고
            //{
            //    Debug.WriteLine("Recive Manual Report Log");
            //    SensorHistoryManager.Instance.AddSensorHistory(log);
            //    //SensorHistoryManager.Instance.AddSensorHistoryID(log.SensorHistoryID, log.ID);
            //    FormMain.Instance.Invoke((MethodInvoker)delegate
            //    {
            //        FormMain.Instance.SelectCCTVTab(false);
            //    });


            //    System.Diagnostics.Trace.WriteLine(string.Format("BeginProcess Maual"));
            //    FireSensor ss = new FireSensor();
            //    ss.ID = nZoneID * 1000;
            //    Zone realzone = ZoneManager.Instance.GetZone(nZoneID);
            //    ProcessManager.Instance.BeginProcess(ss, realzone, log.SensorHistoryID, ProcessType.FireAlarm, false);
            //    FireDetectProcess process = (FireDetectProcess)ProcessManager.Instance.FindProcess(log.SensorHistoryID);
            //    if (process != null)
            //    {
            //        process.LastLog = log;
            //        FormMain.Instance.Invoke((MethodInvoker)delegate
            //        {
            //            FormMain.Instance.PageHome.ChangeTab(PageBackstageHome.Tab.CCTV_TAB);
            //            FormMain.Instance.AddFireDectect(process, !bAddOnly);
            //        });
            //    }
            //    return;
            //}

            //System.Diagnostics.Trace.WriteLine(string.Format("SensorZoneID : {0}", nSensorZoneID));

            //if (sensor != null)
            //{
            //    SensorHistoryManager.Instance.AddSensorHistory(log);
            //    //SensorHistoryManager.Instance.AddSensorHistoryID(log.SensorHistoryID, log.ID);
            //    System.Diagnostics.Trace.WriteLine(string.Format("BeginProcess"));
            //    ProcessManager.Instance.BeginProcess(sensor, log, ProcessType.FireAlarm, !bAddOnly);
            //}
        }

        private ProcessIF BeginProcess(ReactionLog log, bool bAddOnly = false)
        {
            int nZoneID = -1, nSensorZoneID = -1;

            int.TryParse(log.Parameter1, out nZoneID);
            int.TryParse(log.Parameter2, out nSensorZoneID);

            EquipmentZone zone = null;
            ISensor sensor = null;

            if (nZoneID > 0)
                zone = ZoneManager.Instance.GetEquipZone(nZoneID);

            if (nSensorZoneID > 0)
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
                //SensorHistoryManager.Instance.AddSensorHistoryID(log.SensorHistoryID, log.ID);
                FormMain.Instance.Invoke((MethodInvoker)delegate
                {
                    FormMain.Instance.SelectCCTVTab(false);
                });


                //System.Diagnostics.Trace.WriteLine(string.Format("BeginProcess Maual"));
                FireSensor ss = new FireSensor();
                ss.ID = nZoneID + 50000;
                Zone realzone = ZoneManager.Instance.GetZone(nZoneID);
                
                ProcessManager.Instance.BeginProcess(ss, realzone, log.SensorHistoryID, ProcessType.FireAlarm, false);
                
                ProcessIF process = ProcessManager.Instance.FindProcess(log.SensorHistoryID);
                if (process != null)
                {
                    process.LastLog = log;
                    FormMain.Instance.Invoke((MethodInvoker)delegate
                    {
                        FormMain.Instance.PageHome.ChangeTab(UnE.View.Content.ContentOwnerTab.CCTV_TAB);
                        FormMain.Instance.AddSensorDectect(process, !bAddOnly);
                    });
                }
                return process;
            }

            //System.Diagnostics.Trace.WriteLine(string.Format("SensorZoneID : {0}", nSensorZoneID));

            if (sensor != null)
            {
                ProcessType type = GetProcessType(sensor.Type);
                SensorHistoryManager.Instance.AddSensorHistory(log);
                //SensorHistoryManager.Instance.AddSensorHistoryID(log.SensorHistoryID, log.ID);
                //System.Diagnostics.Trace.WriteLine(string.Format("BeginProcess"));
                
                /*** 같은 EquipmentZone인 센서인 경우 기존 EquipmentZone에 대한 Process를 수행 : by hypark ***/
                ProcessIF sameEquipZoneProcess = ProcessManager.Instance.FindSameEquipmentZoneProcess(sensor.EquipZoneID);
                if (sameEquipZoneProcess != null)
                {
                    ProcessManager.Instance.RemoveSleepProcess(nSensorZoneID);           //혹시 있을지 모를 프로세스를 지운다. 
                    ProcessIF process = ProcessManager.Instance.MakeSleepProcess(sensor, log, type, !bAddOnly);     //sleepDetectProcess에 add 까지 캡슐화
                    FormMain.Instance.SendSensorSameSensorGroupRunningToSOPSimulator(process.SensorHistoryID, sameEquipZoneProcess.SensorHistoryID);        
                    return sameEquipZoneProcess;
                }                    
               
                ProcessIF beginProcess = ProcessManager.Instance.BeginProcess(sensor, log, type, !bAddOnly);
                return beginProcess;
            }
            return null;
        }

        private ProcessType GetProcessType(IFacility.FacilityType sensorType)
        {
            switch(sensorType)
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
            }
            return ProcessType.FireAlarm;
        }

        public void SendData(short header, List<KeyValuePair<byte, byte[]>> arList)
        {
            if (header < 0)
                return;

            if (arList == null || arList.Count >= 10000)
                return;

            int dataLength = 0;

            foreach (KeyValuePair<byte, byte[]> pair in arList)
            {
                dataLength += pair.Value.Length;
                dataLength += 5;
            }

            byte[] sndData = new byte[dataLength + 6];

            byte[] nHader = BitConverter.GetBytes(header);
            byte[] nCount = BitConverter.GetBytes(arList.Count);

            sndData[0] = nHader[0];
            sndData[1] = nHader[1];

            sndData[2] = nCount[0];
            sndData[3] = nCount[1];
            sndData[4] = nCount[2];
            sndData[5] = nCount[3];

            int nDataCount = 6;
            foreach (KeyValuePair<byte, byte[]> pair in arList)
            {
                byte[] datas = pair.Value;

                sndData[nDataCount++] = pair.Key;
                byte[] lengthData = BitConverter.GetBytes(datas.Length);
                for (int i = 0; i < 4; i++)
                {
                    sndData[nDataCount++] = lengthData[i];
                }
                for (int i = 0; i < datas.Length; i++)
                {
                    sndData[nDataCount++] = datas[i];
                }
            }
            if (this.IsClientDisposed == false)
                m_mgr.Send(sndData, this, ProviderNum);
        }

        // header 1 Byte로만 이루어진 데이터
        public void SendData(short header)
        {
            byte[] bytes = new byte[6];

            byte[] nHader = BitConverter.GetBytes(header);
            byte[] nCount = BitConverter.GetBytes(0);

            bytes[0] = nHader[0];
            bytes[1] = nHader[1];

            bytes[2] = nCount[0];
            bytes[3] = nCount[1];
            bytes[4] = nCount[2];
            bytes[5] = nCount[3];

            if (this.Client.Client != null)
            {
                if (this.Client.Client.Connected == true)
                    m_mgr.Send(bytes, this, ProviderNum);
            }
        }

        public void SendData(short header, byte dataHeader, byte[] datas)
        {
            if (header < 0)
                return;

            if (datas.Length >= 10000)
                return;
            if (datas == null || datas.Length == 0)
                return;

            byte[] sndData = new byte[datas.Length + 11];

            byte[] nHader = BitConverter.GetBytes(header);
            byte[] nCount = BitConverter.GetBytes(1);

            // SET MESSAGE HeADER
            sndData[0] = nHader[0];
            sndData[1] = nHader[1];

            // SET DATA COUNT
            sndData[2] = nCount[0];
            sndData[3] = nCount[1];
            sndData[4] = nCount[2];
            sndData[5] = nCount[3];

            // SET DATA TYPE
            sndData[6] = dataHeader;

            // SET DATA LENGTH
            byte[] lengthData = BitConverter.GetBytes(datas.Length);
            for (int i = 0; i < 4; i++)
            {
                if (lengthData.Length > i)
                {
                    sndData[7 + i] = lengthData[i];
                }
            }

            // SET DATA
            for (int i = 0; i < datas.Length; i++)
            {
                sndData[i + 11] = datas[i];
            }

            if (this.IsClientDisposed == false)
                m_mgr.Send(sndData, this, ProviderNum);
        }

        public override void OnDropConnection()
        {
            m_mgr.OnDropConnection(m_nProviderNum);
            //m_arrTemp = null;

            if (m_nProviderNum == 2)
            {
                try
                {
                    FormMain.Instance.Invoke((MethodInvoker)delegate
                    {
                        foreach (Reciver reciver in ReciverManager.Instance.DicReciverList.Values)
                        {
                            reciver.State = 0;
                            int nReciverID = reciver.ID;
                            bool bConnected = false;
                            ReciverManager.Instance.UpdateState(nReciverID, bConnected, false);
                        }
                    });
                }
                catch (System.Exception)
                {
                }
            }
        }

        public new void Close()
        {
            base.Close();
            //m_arrTemp = null;
        }

        public static byte[] MakeBytes(int data)
        {
            int nDataLength = sizeof(int);
            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.INTEGER;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = BitConverter.GetBytes(data);

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(long data)
        {
            int nDataLength = sizeof(long);
            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.LONG;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = BitConverter.GetBytes(data);

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(float data)
        {
            int nDataLength = sizeof(float);
            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.FLOAT;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = BitConverter.GetBytes(data);

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(double data)
        {
            int nDataLength = sizeof(double);
            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.DOUBLE;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = BitConverter.GetBytes(data);

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(string data)
        {
            UTF8Encoding enc = new UTF8Encoding();
            byte[] datas = enc.GetBytes(data);

            int nDataLength = datas.Length;

            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.STRING;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = datas[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(bool data)
        {
            int nDataLength = sizeof(bool);
            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.BOOLEAN;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = BitConverter.GetBytes(data);

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(short data)
        {
            int nDataLength = sizeof(short);
            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.SHORT;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = BitConverter.GetBytes(data);

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(byte data)
        {
            int nDataLength = sizeof(byte);
            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.BYTE;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = BitConverter.GetBytes(data);

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
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
                    bytes = MakeBytes((int)data);
                else if (type == typeof(long))
                    bytes = MakeBytes((long)data);
                else if (type == typeof(float))
                    bytes = MakeBytes((float)data);
                else if (type == typeof(bool))
                    bytes = MakeBytes((bool)data);
                else if (type == typeof(double))
                    bytes = MakeBytes((double)data);
                else if (type == typeof(short))
                    bytes = MakeBytes((short)data);
                else if (type == typeof(byte))
                    bytes = MakeBytes((byte)data);
                else if (type == typeof(string))
                    bytes = MakeBytes((string)data);
                
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

                if (type == TCP_TYPE.INTEGER)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 9, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        int nData = BitConverter.ToInt32(bytes, nIndex - 4);
                        arrResult.Add(nData);
                    }
                }
                else if (type == TCP_TYPE.FLOAT)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 9, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        float fData = BitConverter.ToSingle(bytes, nIndex - 4);
                        arrResult.Add(fData);
                    }
                }
                else if (type == TCP_TYPE.DOUBLE)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 13, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        double dData = BitConverter.ToDouble(bytes, nIndex - 8);
                        arrResult.Add(dData);
                    }
                }
                else if (type == TCP_TYPE.LONG)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 13, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        long lData = BitConverter.ToInt64(bytes, nIndex - 8);
                        arrResult.Add(lData);
                    }
                }
                else if (type == TCP_TYPE.BOOLEAN)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 6, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        bool bData = BitConverter.ToBoolean(bytes, nIndex - 1);
                        arrResult.Add(bData);
                    }
                }
                else if (type == TCP_TYPE.SHORT)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 7, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        short sData = BitConverter.ToInt16(bytes, nIndex - 2);
                        arrResult.Add(sData);
                    }
                }
                else if (type == TCP_TYPE.BYTE)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 6, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        byte data = bytes[nIndex - 1];
                        arrResult.Add(data);
                    }
                }
                else if (type == TCP_TYPE.STRING)
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

        public void SendRequestDataList()
        {
            SendData(TCP_ID.REQUEST_SENSOR_REACTION_HISTORY_DATA_LIST);
        }

        public void SendChangedConfig(byte byteClientType, string strPropertyName, string strPropertyValue)
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(byteClientType);
            arrDatas.Add(strPropertyName);
            arrDatas.Add(strPropertyValue);

            byte[] bytes = MakeBytes(TCP_ID.CHANGE_CONFIG, arrDatas);

            m_mgr.Send(bytes, this, ProviderNum);
        }


        // 이전 시점에 AllClear를 수행했는지 여부
        private bool m_bPrevClearOp = false;

        // 현재 진행중인 화재신호가 유효한 값인지 DB에서 검사한다.
        public void CheckValidProcess()
        {
            // 이전 시점에 AllClear를 수행 한 경우 부하문제로 한번은 건너뛰도록 한다.
            // AllClear는 3D에 많은 부하를 주므로 호출을 적절히 수행해야 함
            // edit by skkim 2016-08-03
            if (m_bPrevClearOp == true)
            {
                m_bPrevClearOp = false;
                return;
            }

            if (ProcessManager.Instance.CurrentDetectProcess.Count == 0)
            {
                if (FormMain.Instance != null && FormMain.Instance.IsHandleCreated)
                {
                    FormMain.Instance.Invoke((MethodInvoker)delegate
                    {
                        FormMain.Instance.ClearAllFireDetect();
                        m_bPrevClearOp = true;
                    });
                }
                return;
            }

            m_bPrevClearOp = false;

            // 오류있는 코드로 부하만 생성하므로 코멘트 처리함 skkim 2016-08-03
            // LiveOn Sensor Check는 DB부하가 많이 걸리는 작업이므로 필요시 SensorTester의 루틴을 참고
            // SendRequestDataList는 서버에 많은 부하가 걸리는 작업이므로 자주 호출하면 안됨
            //try
            //{
            //    string strSubSQL = "";
            //    Dictionary<int, ProcessIF> dicSensorZoneProcess = new Dictionary<int, ProcessIF>();

            //    foreach (KeyValuePair<int, ProcessIF> pair in ProcessManager.Instance.CurrentDetectProcess)
            //    {

            //        int nHistoryID = pair.Value.SensorHistoryID;
            //        dicSensorZoneProcess[nHistoryID] = pair.Value;

            //        if (strSubSQL.Length == 0)
            //            strSubSQL = nHistoryID.ToString();
            //        else
            //            strSubSQL += ", " + nHistoryID.ToString();
            //    }

            //    int nCount = ProcessManager.Instance.CurrentDetectProcess.Count;

            //    if (strSubSQL != "")
            //    {
            //        string strSQL = string.Format("SELECT Count(ReactionType), SensorHistoryID FROM SensorReactionHistory where SensorHistoryID in ({0}) and ReactionType in (23, 33, 50) group by SensorHistoryID", strSubSQL);

            //        ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);
            //        if (arrResult != null)
            //        {
            //            int nResultCount = arrResult.Count;

            //            for (int i = 0; i < nResultCount - 1; i += 2)
            //            {

            //                int nData = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), 0);
            //                int nSensorHistroyID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
            //                if (nData == 0)
            //                {
            //                    dicSensorZoneProcess.Remove(nSensorHistroyID);

            //                    ProcessIF process = null;
            //                    if (dicSensorZoneProcess.TryGetValue(nSensorHistroyID, out process))
            //                    {
            //                        EndProcess(process);
            //                        RemoveProcess(process);
            //                    }
            //                }
            //            }
                        
            //            if (ProcessManager.Instance.CurrentDetectProcess.Count != nCount)
            //            {
            //                SendRequestDataList();
            //            }
            //        }
            //    }

            //    if (ProcessManager.Instance.CurrentDetectProcess.Count == 0)
            //    {
            //        if (FormMain.Instance != null && FormMain.Instance.IsHandleCreated)
            //        {
            //            FormMain.Instance.Invoke((MethodInvoker)delegate
            //            {
            //                FormMain.Instance.ClearAllFireDetect();
            //            });
            //        }
            //        return;
            //    }
            //}
            //catch (Exception)
            //{
            //}
        }

        public void SendSensorZoneListInEquipZone(List<SensorZoneUpdateData> sensorZoneUpdateDatas)
        {
            ArrayList arrDatas = new ArrayList();

            foreach (SensorZoneUpdateData data in sensorZoneUpdateDatas)
            {
                if (data.SensorZone == null)
                    continue;

                arrDatas.Add(data.SensorZone.ID);
                arrDatas.Add(data.OriginEquipZone);
                arrDatas.Add(data.ChangedEquipZone);
                arrDatas.Add(data.Zone);

            }

            byte[] bytes = MakeBytes(TCP_ID.EDIT_SENSOR_ZONE, arrDatas);
            Send(bytes, 0, bytes.Length);
        }
    }
}