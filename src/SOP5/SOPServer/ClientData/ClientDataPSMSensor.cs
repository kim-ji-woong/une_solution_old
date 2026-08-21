using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Threading;
using SDMS;
using System.Net;
using System.IO;
using SOPServer.Data;
using UnE.Spatial;
using UnE.Sensor;
using UnE.PSM;

namespace SDMSServer
{
    public class ClientDataPSMSensor : ClientData
    {
        private class QueueData
        {
            private TcpLib2.ConnectionState m_state = null;
            private byte[] m_bytes = null;
            private int m_nHeader = 0;
            private ArrayList m_arrDatas = null;

            public TcpLib2.ConnectionState State
            {
                get { return m_state; }
            }

            public byte[] Bytes
            {
                get { return m_bytes; }
            }

            public int Header
            {
                get { return m_nHeader; }
            }

            public ArrayList Datas
            {
                get { return m_arrDatas; }
            }

            public QueueData()
            {
            }

            public QueueData(TcpLib2.ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas)
            {
                m_state = state;
                m_bytes = bytes;
                m_nHeader = nHeader;
                m_arrDatas = arrDatas;
            }
        }

        private static ArrayList m_arrMessageQueue = new ArrayList();
        private static bool m_runThread = false;

        public static bool RunThread
        {
            get { return m_runThread; }
            set
            {
                if (m_runThread != value)
                {
                    m_runThread = value;

                    if (m_runThread)
                    {
                        Thread t = new Thread(ReceiveThread);
                        t.Start();
                    }
                }
            }
        }

        protected static void ReceiveThread()
        {
            while (m_runThread && !NetworkServer.Instance.FinishProcess)
            {
                while (m_arrMessageQueue.Count > 0)
                {
                    QueueData data = (QueueData)m_arrMessageQueue[0];

                    if (data.State != null && data.State.Tag != null)
                    {
                        ClientDataPSMSensor client = (ClientDataPSMSensor)data.State.Tag;
                        client._OnReceive(data.State, data.Bytes, data.Header, data.Datas);
                    }

                    m_arrMessageQueue.RemoveAt(0);

                    if (!m_runThread || NetworkServer.Instance.FinishProcess)
                        return;
                }

                Thread.Sleep(50);
            }
        }

        public ClientDataPSMSensor(ServiceProvider provider)
        {
            m_provider = provider;
            ClientType = TCP_CLIENT.PSM_SENSOR_SERVER;
        }

        protected override bool OnReceive(TcpLib2.ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas)
        {
            RunThread = true;

            QueueData data = new QueueData(state, bytes, nHeader, arrDatas);
            m_arrMessageQueue.Add(data);

            return true;
        }

        protected bool _OnReceive(TcpLib2.ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas)
        {
            if (nHeader == TCP_ID.SDMS_COMMAND)
            {
                ProcessSDMSCommand(state, bytes, arrDatas);
            }

            return true;
        }


        public void ProcessPSMSensorData(TcpLib2.ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas)
        {
            if( nHeader == TCP_ID.PSM_SENSOR_DATA)
            {
                ProcessSensorData(state, bytes, arrDatas);
            }
            else if(nHeader == TCP_ID.TEST_PSM_SENSOR_DATA)
            {
                ProcessSensorData(state, bytes, arrDatas, true);
            }
        }

        protected bool ProcessSDMSCommand(TcpLib2.ConnectionState state, byte[] bytes, ArrayList arrDatas)
        {
            if (arrDatas == null)
                return false;

            int nDataCount = arrDatas.Count;

            if (nDataCount == 0 || (arrDatas[0] is byte) == false)
                return false;

            byte cmd = (byte)arrDatas[0];

            if (cmd == SDMSCommandType.PSM_SENSOR_DATA)
                return ProcessSensorData(state, bytes, arrDatas);

            return false;
        }

        
        protected bool ProcessSensorData(TcpLib2.ConnectionState state, byte[] bytes, ArrayList arrDatas, bool bTest = false)
        {
            if (arrDatas.Count < 4)
                return false;

            if ((arrDatas[1] is int) && (arrDatas[2] is int) && (arrDatas[3] is int))
            {
                int nSensorTagInfoID = (int)arrDatas[1];
                int nSensorZoneID = (int)arrDatas[2];
                int nAlarmDepth = (int)arrDatas[3];
                int nSensorZoneHistoryID, nPrevSensorHistoryID, nSensorData;
                SensorZone sensorZone;

                if (!PSMManager.Instance.ProcessSensorData(nSensorTagInfoID, nSensorZoneID, nAlarmDepth, bTest, out nSensorZoneHistoryID, out sensorZone, out nPrevSensorHistoryID, out nSensorData))
                    return false;
                if(arrDatas.Count == 6)    //광교 데이터만 해당 .
                {
                    int windDirection = (int)arrDatas[4];
                    int windSpeed = (int)arrDatas[5];
                    PostProcessSensorData(nSensorZoneHistoryID, nPrevSensorHistoryID, sensorZone, nSensorZoneID, new DBUtility.VariousData<int>(nSensorData), bTest, windDirection, windSpeed);
                } 
                else
                {
                    PostProcessSensorData(nSensorZoneHistoryID, nPrevSensorHistoryID, sensorZone, nSensorZoneID, new DBUtility.VariousData<int>(nSensorData), bTest);
                }
                
                
            }

            return true;
        }

        private bool GetPSMAlarmDepth(TimeHistory history, out int nAlarmDepth)
        {
            nAlarmDepth = -1;

            if (history == null)
                return false;

            if (history.LastReactionLog == null)
                return false;

            if (history.LastReactionLog.Param5 == null)
                return false;

            if (int.TryParse(history.LastReactionLog.Param5, out nAlarmDepth))
                return true;

            return false;
        }

        public void PostProcessSensorData(int nSensorZoneHistoryID, int nPrevSensorHistoryID, SensorZone sensorZone, int nOriginSensorZoneID, DBUtility.VariousData<int> sensorData = null, bool bTest = false, int windDirection =-1, int windSpeed=-1)
        {
            PSMSensor psmSensor = PSMManager.Instance.GetSensor(sensorZone.LinkedSensorID);

            int nData = sensorData == null ? sensorZone.SensorData : sensorData.Data;
            bool bConnected = sensorZone.IsConnected;

            // comment by skkim : AbnormalSensorManager에서 대행
            // 임시로 무시된 Sensor List에서 해제할 것이 있는지 검사
            if (sensorZone.ID > 0 && nData == 0)
            {
                m_provider.RemoveTempIgnoreSensor(sensorZone.ID);
            }

            if ((nData >= (int)UnE.Alarm.AlarmType.PSM_ALARM_1 && nData <= (int)UnE.Alarm.AlarmType.PSM_ALARM_3) && nSensorZoneHistoryID != -1)
            //if ((nData >= (int)PSMManager.HistoryDataType.PSM_ALARM_1 && nData <= (int)PSMManager.HistoryDataType.PSM_ALARM_3) && nSensorZoneHistoryID != -1)
            {
                TimeHistory prevHistory;

                // nSensorZoneHistoryID에 해당하는 로그가 이미 존재하는지 여부를 확인
                if (!m_provider.CheckSituation(nSensorZoneHistoryID, out prevHistory))
                {
                    SensorReactionLog.DetectionStatus type = SensorReactionLog.DetectionStatus.REAL;
                    if (bTest == true)
                        type = SensorReactionLog.DetectionStatus.TEST;
                    TimeHistory hs = new TimeHistory(nSensorZoneHistoryID, DateTime.Now, type);
                    m_provider.AddTimeHistory(hs);
                    PingCount = 0;

                    if (windDirection >= 0 && windSpeed >= 0)       // 광교인 경우만 실행됨.
                    {
                        m_provider.SendSensorZoneDataWithWind(nData, sensorZone.ID, TCP_CLIENT.SDMS_CLIENT, windDirection, windSpeed);
                    }
                    else
                    {
                        m_provider.SendSensorZoneData(nData, sensorZone.ID, TCP_CLIENT.SDMS_CLIENT);
                    }

                    PingCount = 0;

                    SensorReactionLog log = null;

                    if( bTest == true)
                    {
                        log = CreateTestPSMSensorDetect(nSensorZoneHistoryID, nData - (int)UnE.Alarm.AlarmType.PSM_ALARM_1 + 1, sensorZone, psmSensor, nOriginSensorZoneID);
                        //log = CreateTestPSMSensorDetect(nSensorZoneHistoryID, nData - (int)PSMManager.HistoryDataType.CLEAR_PSM_ALARM, sensorZone, psmSensor, nOriginSensorZoneID);
                    }
                    else
                    {
                        log = CreatePSMSensorDetect(nSensorZoneHistoryID, nData - (int)UnE.Alarm.AlarmType.PSM_ALARM_1 + 1, sensorZone, psmSensor, nOriginSensorZoneID);
                        //log = CreatePSMSensorDetect(nSensorZoneHistoryID, nData - (int)PSMManager.HistoryDataType.CLEAR_PSM_ALARM, sensorZone, psmSensor, nOriginSensorZoneID);
                    }
                   

                    if( log != null)
                    {
                        m_provider.AddReactionLog(log);

                        ClientDataSDMS.RunBroadcast(log, m_provider, BroadcastManager.SituationType.DETECT_PSM);

                        if (NetworkServer.Instance.SiteID == 3)
                        {
                            new Thread(() =>
                            {
                                FTPManager ftpMgr = new FTPManager();
                                ftpMgr.FindImage(m_provider, log, psmSensor.Name); 
                            }).Start();
                        }
                        else
                            m_provider.SendSMS(log, SMSManager.SMSMessageType.DETECT_PSM);

                        // Send Reaction Log
                        m_provider.SendSensorReactionLog(log, TCP_CLIENT.SDMS_CLIENT_SECOND);

                        if (hs != null)
                            hs.LastReactionLog = log;
                    }                    
                }
                else
                {
                    // nSensorZoneHistoryID에 해당하는 로그가 이미 존재한다.
                    int nPrevAlarmDepth;

                    if (!GetPSMAlarmDepth(prevHistory, out nPrevAlarmDepth))
                        return;

                    // 현재 상태와 이전 상태가 같은 값이면 무시한다.
                    if (nPrevAlarmDepth + (int)UnE.Alarm.AlarmType.PSM_ALARM_1 - 1 == nData)
                    //if (nPrevAlarmDepth + (int)PSMManager.HistoryDataType.CLEAR_PSM_ALARM == nData)
                        return;

                    SensorReactionLog log = null;
                    if( bTest == true)
                    {
                        log = ChangeTestPSMSensorAlarmDepth(nSensorZoneHistoryID, nData - (int)UnE.Alarm.AlarmType.PSM_ALARM_1 + 1, nPrevAlarmDepth, sensorZone, psmSensor, nOriginSensorZoneID);
                        //log = ChangeTestPSMSensorAlarmDepth(nSensorZoneHistoryID, nData - (int)PSMManager.HistoryDataType.CLEAR_PSM_ALARM, nPrevAlarmDepth, sensorZone, psmSensor, nOriginSensorZoneID);
                    }
                    else
                    {
                        log = ChangePSMSensorAlarmDepth(nSensorZoneHistoryID, nData - (int)UnE.Alarm.AlarmType.PSM_ALARM_1 + 1, nPrevAlarmDepth, sensorZone, psmSensor, nOriginSensorZoneID);
                        //log = ChangePSMSensorAlarmDepth(nSensorZoneHistoryID, nData - (int)PSMManager.HistoryDataType.CLEAR_PSM_ALARM, nPrevAlarmDepth, sensorZone, psmSensor, nOriginSensorZoneID);
                    }
                    
                    m_provider.AddReactionLog(log);

                    // Send Reaction Log
                    m_provider.SendSensorReactionLog(log, TCP_CLIENT.SDMS_CLIENT_SECOND);

                    TimeHistory hs = m_provider.FindTimeHistory(nSensorZoneHistoryID);

                    if (hs != null)
                    {
                        hs.LastReactionLog = log;
                        hs.DetectStatus = log.Status;
                    }
                        
                }
            }
            else if (nData == (int)UnE.Alarm.AlarmType.NO_ALARM && nSensorZoneHistoryID != -1)
            //else if ((nData == 0 || nData == (int)PSMManager.HistoryDataType.CLEAR_PSM_ALARM) && nSensorZoneHistoryID != -1)
            {
                if (nPrevSensorHistoryID > 0)
                {
                    TimeHistory history = m_provider.FindTimeHistory(nPrevSensorHistoryID);

                    if (history != null && history.LastReactionLog != null/* && history.LastReactionLog.Type == SensorReactionLog.ReactionType.BEGIN_PSM_STATUS*/)
                    {
                        byte nClientType = TCP_CLIENT.SDMS_CLIENT;
                        Thread.Sleep(5);
                        PingCount = 0;


                        try
                        {
                            m_provider.SendSensorZoneData(nData, sensorZone.ID, nClientType);
                        }
                        catch(Exception)
                        {

                        }

                        PingCount = 0;
                        // 누출 상황 종료
                        Thread.Sleep(5);
                        nClientType = TCP_CLIENT.SDMS_CLIENT;

                        try
                        {
                            m_provider.SendClearDetectReport(nPrevSensorHistoryID, nClientType);
                        }
                        catch (Exception)
                        {

                        }

                       

                        Thread.Sleep(5);
                        m_provider.RemoveTimeHistory(history);
                        m_provider.RemoveSituation(nSensorZoneHistoryID, false);

                        SensorManager.Instance.RemoveSensorHistory(nPrevSensorHistoryID);
                        SensorManager.Instance.RemoveSensorHistory(nSensorZoneHistoryID);

                        if (history.LastReactionLog.Type == libSensorProcess.ReactionType.BEGIN_PSM_STATUS ||
                            history.LastReactionLog.Type == libSensorProcess.ReactionType.CHANGE_PSM_ALARM_DEPTH ||
                            history.LastReactionLog.Type == libSensorProcess.ReactionType.NOTIFY_PSM)
						{
                            bool notifyPSM = history.LastReactionLog.Type == libSensorProcess.ReactionType.NOTIFY_PSM;

                            int nSensorID = sensorZone.ID;
                            SensorReactionLog log = CreateIgnorePSMDetect(nPrevSensorHistoryID, nSensorID);

							m_provider.AddReactionLog(log);

                            // 자동 복구시 문자 전송
                            if (notifyPSM)
                            {
                                if (DataManager.Instance.UseReportFacilityManagers)
                                    m_provider.SendSMS(log, SMSManager.SMSMessageType.REPORT_PSM);
                                else
                                    m_provider.SendSMSToAllCompanyMember(log, SMSManager.SMSMessageType.REPORT_PSM);
                            }
                            else
                                m_provider.SendSMS(log, ServiceProvider.GetSMSMessageTypeFromLog(log));

                            // SensorZoneGroup 비우기
                            SensorZoneGroup sensorZoneGroup = NetworkServer.Instance.IOManager.GetSensorZoneGroup(nSensorID);

                            if (sensorZoneGroup != null)
                                sensorZoneGroup.SensorDatas.Clear();
						}

                    }
                }
            }
        }

        private SensorReactionLog CreateIgnorePSMDetect(int nHistoryID, int nSensorID)
        {
            SensorReactionLog log = new SensorReactionLog();

            log.LogTime = DateTime.Now;
            log.SensorHistoryID = nHistoryID;
            log.Type = libSensorProcess.ReactionType.IGNORE_PSM_DETECT;
            log.Status = SensorReactionLog.DetectionStatus.REAL;
            int nEquipZoneID = SensorManager.Instance.GetEquipmentZoneID(nSensorID);
            if (nEquipZoneID == -1)
            {
                log.Message = "탐지된 누출신호가 현장 복구되었습니다";
            }
            else
            {
                EquipmentZone equipZone = ZoneManager.Instance.GetEquipmentZone(nEquipZoneID);
                if (equipZone != null)
                {
                    // update by mwkim 2016-05-11 : BroadcastName -> DisplayText
                    string szLocationName = equipZone.DisplayText;
                    log.Message = string.Format("[{0}]에서 탐지된 누출신호가 현장 복구되었습니다", szLocationName);
                }
                log.Param1 = nEquipZoneID.ToString();
            }

            log.Param2 = nSensorID.ToString();

            return log;
        }	

        private SensorReactionLog ChangePSMSensorAlarmDepth(int nHistoryID, int nAlarmDepth, int nPrevAlarmDepth, SensorZone sensorZone, PSMSensor psmSensor, int nOriginSensorZoneID)
        {
            SensorReactionLog log = new SensorReactionLog();

            log.LogTime = DateTime.Now;
            log.SensorHistoryID = nHistoryID;
            log.Type = libSensorProcess.ReactionType.CHANGE_PSM_ALARM_DEPTH;
            log.Status = SensorReactionLog.DetectionStatus.REAL;

            string strLocationName;
            log.Message = GetPSMSensorChangeAlarmDepthString(psmSensor, nAlarmDepth, nPrevAlarmDepth, out strLocationName);

            log.Param1 = sensorZone.EquipZone == null ? "" : sensorZone.EquipZone.ID.ToString();
            log.Param2 = sensorZone.ID.ToString();
            log.Param3 = nOriginSensorZoneID.ToString();
            log.Param4 = strLocationName;
            log.Param5 = nAlarmDepth.ToString();

            return log;
        }

        private SensorReactionLog ChangeTestPSMSensorAlarmDepth(int nHistoryID, int nAlarmDepth, int nPrevAlarmDepth, SensorZone sensorZone, PSMSensor psmSensor, int nOriginSensorZoneID)
        {
            SensorReactionLog log = new SensorReactionLog();

            log.LogTime = DateTime.Now;
            log.SensorHistoryID = nHistoryID;
            log.Type = libSensorProcess.ReactionType.CHANGE_PSM_ALARM_DEPTH;
            log.Status = SensorReactionLog.DetectionStatus.TEST;

            string strLocationName;
            log.Message = GetTestPSMSensorChangeAlarmDepthString(psmSensor, nAlarmDepth, nPrevAlarmDepth, out strLocationName);

            log.Param1 = sensorZone.EquipZone == null ? "" : sensorZone.EquipZone.ID.ToString();
            log.Param2 = sensorZone.ID.ToString();
            log.Param3 = nOriginSensorZoneID.ToString();
            log.Param4 = strLocationName;
            log.Param5 = nAlarmDepth.ToString();

            return log;
        }

        private SensorReactionLog CreatePSMSensorDetect(int nHistoryID, int nAlarmDepth, SensorZone sensorZone, PSMSensor psmSensor, int nOriginSensorID)
        {
            SensorReactionLog log = new SensorReactionLog();

            log.LogTime = DateTime.Now;
            log.SensorHistoryID = nHistoryID;
            log.Type = libSensorProcess.ReactionType.BEGIN_PSM_STATUS;
            log.Status = SensorReactionLog.DetectionStatus.REAL;

            string strLocationName;
            log.Message = GetPSMSensorDetectString(psmSensor, out strLocationName);

            log.Param1 = sensorZone.EquipZone == null ? "" : sensorZone.EquipZone.ID.ToString();
            log.Param2 = sensorZone.ID.ToString();
            log.Param3 = nOriginSensorID.ToString();
            log.Param4 = strLocationName;
            log.Param5 = nAlarmDepth.ToString();

            return log;
        }

        private SensorReactionLog CreateTestPSMSensorDetect(int nHistoryID, int nAlarmDepth, SensorZone sensorZone, PSMSensor psmSensor, int nOriginSensorID)
        {
            SensorReactionLog log = new SensorReactionLog();

            log.LogTime = DateTime.Now;
            log.SensorHistoryID = nHistoryID;
            log.Type = libSensorProcess.ReactionType.BEGIN_PSM_STATUS;
            log.Status = SensorReactionLog.DetectionStatus.TEST;

            string strLocationName;
            log.Message = GetTestPSMSensorDetectString(psmSensor, out strLocationName);

            log.Param1 = sensorZone.EquipZone == null ? "" : sensorZone.EquipZone.ID.ToString();
            log.Param2 = sensorZone.ID.ToString();
            log.Param3 = nOriginSensorID.ToString();
            log.Param4 = strLocationName;
            log.Param5 = nAlarmDepth.ToString();

            return log;
        }

        public static string GetTestPSMSensorDetectString(PSMSensor sensor, out string strLocationName)
        {
            strLocationName = "";
            string szResult = "";
            if (sensor == null)
            {
                szResult = "[테스트] 유해화학물질 누출이 탐지되었습니다";
            }
            else
            {
                PSMMaterial material = PSMManager.Instance.GetMaterial(sensor.MaterialType);
                //PSMMaterial material = sensor.GetLinkedMaterial();
                string strMaterialName = material == null ? "유해화학물질" : material.Name;

                // update by mwkim 2016-05-11 : Sensor가 연결된 Tank의 LocationName을 EquipmentZone의 DisplayText로 변경
                //strLocationName = sensor.GetLinkedLocationName();
                EquipmentZone equipZone = ZoneManager.Instance.GetEquipmentZone(sensor.EquipZoneID);
                strLocationName = equipZone != null ? equipZone.DisplayText : "";
                //strLocationName = sensor.EquipmentZone.DisplayText;

                szResult = string.Format("[테스트][{0}]에서 {1} 누출이 탐지되었습니다", strLocationName, strMaterialName);
            }
            return szResult;
        }

        public static string GetTestPSMSensorChangeAlarmDepthString(PSMSensor sensor, int nAlarmDepth, int nPrevAlarmDepth, out string strLocationName)
        {
            strLocationName = "";
            string szResult = "";
            if (sensor == null)
            {
                szResult = string.Format("[테스트]유해화학물질 누출의 알람 단계가 {0}단계에서 {1}단계로 변경되었습니다.", nPrevAlarmDepth, nAlarmDepth);
            }
            else
            {
                PSMMaterial material = PSMManager.Instance.GetMaterial(sensor.MaterialType);
                //PSMMaterial material = sensor.GetLinkedMaterial();
                string strMaterialName = material == null ? "유해화학물질" : material.Name;
                // update by mwkim 2016-05-11 : Sensor가 연결된 Tank의 LocationName을 EquipmentZone의 DisplayText로 변경
                //strLocationName = sensor.GetLinkedLocationName();
                EquipmentZone equipZone = ZoneManager.Instance.GetEquipmentZone(sensor.EquipZoneID);
                strLocationName = equipZone != null ? equipZone.DisplayText : "";
                //strLocationName = sensor.EquipmentZone.DisplayText;

                szResult = string.Format("[테스트][{0}]에서 탐지된 {1} 누출의 알람 단계가 {2}단계에서 {3}단계로 변경되었습니다", strLocationName, strMaterialName, nPrevAlarmDepth, nAlarmDepth);
            }
            return szResult;
        }

        public static string GetPSMSensorDetectString(PSMSensor sensor, out string strLocationName)
        {
            strLocationName = "";
            string szResult = "";
            if (sensor == null)
            {
                szResult = "유해화학물질 누출이 탐지되었습니다";
            }
            else
            {
                PSMMaterial material = PSMManager.Instance.GetMaterial(sensor.MaterialType);
                //PSMMaterial material = sensor.GetLinkedMaterial();
                string strMaterialName = material == null ? "유해화학물질" : material.Name;

                // update by mwkim 2016-05-11 : Sensor가 연결된 Tank의 LocationName을 EquipmentZone의 DisplayText로 변경
                //strLocationName = sensor.GetLinkedLocationName();
                EquipmentZone equipZone = ZoneManager.Instance.GetEquipmentZone(sensor.EquipZoneID);
                strLocationName = equipZone != null ? equipZone.DisplayText : "";
                //strLocationName = sensor.EquipmentZone.DisplayText;
                szResult = string.Format("[{0}]에서 {1} 누출이 탐지되었습니다", strLocationName, strMaterialName);
            }

            return szResult;
        }

        public static string GetPSMSensorChangeAlarmDepthString(PSMSensor sensor, int nAlarmDepth, int nPrevAlarmDepth, out string strLocationName)
        {
            strLocationName = "";
            string szResult = "";
            if (sensor == null)
            {
                szResult = string.Format("유해화학물질 누출의 알람 단계가 {0}단계에서 {1}단계로 변경되었습니다.", nPrevAlarmDepth, nAlarmDepth);
            }
            else
            {
                PSMMaterial material = PSMManager.Instance.GetMaterial(sensor.MaterialType);
                //PSMMaterial material = sensor.GetLinkedMaterial();
                string strMaterialName = material == null ? "유해화학물질" : material.Name;
                // update by mwkim 2016-05-11 : Sensor가 연결된 Tank의 LocationName을 EquipmentZone의 DisplayText로 변경
                //strLocationName = sensor.GetLinkedLocationName();
                EquipmentZone equipZone = ZoneManager.Instance.GetEquipmentZone(sensor.EquipZoneID);
                strLocationName = equipZone != null ? equipZone.DisplayText : "";
                //strLocationName = sensor.EquipmentZone.DisplayText;

                szResult = string.Format("[{0}]에서 탐지된 {1} 누출의 알람 단계가 {2}단계에서 {3}단계로 변경되었습니다", strLocationName, strMaterialName, nPrevAlarmDepth, nAlarmDepth);
            }
            return szResult;
        }
    }
}
