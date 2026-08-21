using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.ServiceModel;
using System.Collections;
using AgentFactory;
using UnE.Sensor;
using UnE.Spatial;

namespace ServerProcess.Client
{
    using ServerProcess.Data;
    public class SecuritySensorServer : BaseClient
    {
        private static SecuritySensorServer m_instance = null;

        public static SecuritySensorServer Instance
        {
            get { return m_instance; }
        }

        public override int ClientType
        {
            get { return SOPWebServer.ClientType.SECURITY_SENSOR_SERVER; }
        }

        public SecuritySensorServer()
            : base()
        {
            m_instance = this;
        }

        public SecuritySensorServer(Factory factory, IPostOffice postOffice)
            : base(factory, postOffice)
        {
            m_instance = this;
            m_agent = m_agentFactory.MakeAgent(Factory.AgentType.Security);
        }

        protected override void OnLoadEvent()
        {
        }

        protected override int OnReceiveEvent(ServerProcess.Client.ClientData data, OperationContext ctx, int header, byte[] bytes, ArrayList arrDatas)
        {
            if (header == SOPWebServer.Header.SENSOR_DATA)
                return ProcessSensorData(arrDatas, true);
            else if (header == SOPWebServer.Header.SENSOR_DATA_TEST)
                return ProcessSensorData(arrDatas, false);
            else if (header == SOPWebServer.Header.SENSOR_MALFUNCTION)
                return _ProcessMalfunction(arrDatas);
            /*else if (header == SOPWebServer.Header.RECEIVER_CONNECT)
                return ProcessReceiverState(arrDatas, true);
            else if (header == SOPWebServer.Header.RECEIVER_DISCONNECT)
                return ProcessReceiverState(arrDatas, false);
            else if (header == SOPWebServer.Header.ALL_RECEIVER_STATE)
                return ProcessAllReceiverState(ctx, arrDatas);*/

            return SOPWebServer.ErrorMessageType.UNKNOWN_HEADER;
        }

        private int ProcessSensorData(ArrayList arrDatas, bool isReal)
        {
            if (arrDatas.Count >= 4 && arrDatas[0] is int && arrDatas[1] is int && arrDatas[2] is int && arrDatas[3] is int)
            {
                int nSensorType = (int)arrDatas[0];
                int nSensorTagID = (int)arrDatas[1];
                int nSensorZoneID = (int)arrDatas[2];
                int nSensorData = (int)arrDatas[3];

                IFacility.FacilityType sensorType = IFacility.ToFacilityType(nSensorType);
                SensorZoneGroup group = SensorZoneManager.Instance.GetSensorZoneGroup(nSensorZoneID);

                if (group == null)
                    return SOPWebServer.ErrorMessageType.UNKNOWN_SENSOR_ID;

                SensorZone sensorZone = SensorZoneManager.Instance.GetSensorZone(nSensorZoneID);

                if (sensorZone == null)
                    return SOPWebServer.ErrorMessageType.UNKNOWN_SENSOR_ID;

                if (nSensorData > 0)
                {
                    // 알람 발생
                    AlarmData alarm;
                    int nResult = AddAlarm(group, nSensorTagID, sensorZone, isReal, out alarm);

                    if (alarm != null)
                    {
                        DirectDBManager dbMgr = m_dbMgr.Clone();

                        if (dbMgr.Connect() == false)
                            return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                        m_agentFactory.ProcessManager.NewAlarm(dbMgr, alarm);
                        dbMgr.Close();
                    }

                    return nResult;
                }
                else
                {
                    // 알람 해제
                    AlarmData alarm = group.CurrentAlarm;
                    int nResult = RemoveAlarm(group, sensorZone, isReal);

                    if (alarm != null && group.CurrentAlarm == null)
                    {
                        DirectDBManager dbMgr = m_dbMgr.Clone();

                        if (dbMgr.Connect() == false)
                            return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                        alarm.Status = BaseProcessManager.ReactionType.END_STATUS;
                        m_agentFactory.ProcessManager.ClearAlarm(dbMgr, alarm);

                        dbMgr.Close();
                    }

                    return nResult;
                }
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        private int _ProcessMalfunction(ArrayList arrDatas)
        {
            if (arrDatas.Count >= 4 && arrDatas[0] is int && arrDatas[1] is int && arrDatas[2] is int && arrDatas[3] is string)
            {
                int nSensorZoneHistoryID = (int)arrDatas[0];
                int nSensorZoneID = (int)arrDatas[1];
                int nSOPGenUserID = (int)arrDatas[2];
                string strDescription = (string)arrDatas[3];

                return ProcessMalfunction(nSensorZoneHistoryID, nSensorZoneID, nSOPGenUserID, strDescription);
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        public int ProcessMalfunction(int nSensorZoneHistoryID, int nSensorZoneID, int nSOPGenUserID, string strDescription)
        {
            SensorZone sensorZone = SensorZoneManager.Instance.GetSensorZone(nSensorZoneID);
            SensorZoneGroup group = SensorZoneManager.Instance.GetSensorZoneGroup(nSensorZoneID);

            if (sensorZone == null || group == null)
                return SOPWebServer.ErrorMessageType.UNKNOWN_SENSOR_ID;

            // 알람 해제
            AlarmData alarm = group.CurrentAlarm;
            int nResult = RemoveAlarm_Malfunction(group, sensorZone, nSOPGenUserID);

            if (alarm != null && group.CurrentAlarm == null)
            {
                DirectDBManager dbMgr = m_dbMgr.Clone();

                if (dbMgr.Connect() == false)
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                alarm.Status = BaseProcessManager.ReactionType.MALFUNCTION;
                m_agentFactory.ProcessManager.ClearAlarm(dbMgr, alarm);
                dbMgr.Close();
            }

            return nResult;
        }

        // 오작동 처리
        private int RemoveAlarm_Malfunction(SensorZoneGroup group, SensorZone sensorZone, int nSOPGenUserID)
        {
            // Transaction 처리를 위하여 객체를 새로 만든다.
            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

            if (dbMgr.BeginBatch() == false)
            {
                dbMgr.Close();
                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
            }

            DateTime timeStamp = DateTime.Now;

            // 오작동 처리는 SDMS에서 사용자에 의하여 보내기 때문에 특정 센서 뿐만 아니라
            // SensorZoneGroup내에 있는 모든 센서 데이터를 초기화 시킨다.
            if (group.RemoveAllSensorData(dbMgr) == false)
            //if (group.RemoveSensorData(sensorZone, dbMgr) == false)
            {
                WriteLog("RemoveAllSensorData 실패 : " + sensorZone.ID.ToString());
                dbMgr.Close();
                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
            }

            // sensorZone의 신호는 복구되었지만 같은 영역에 다른 신호가 아직 남아있는 상황
            if (group.GetSensorDatas().Count > 0 && group.CurrentAlarm != null)
            {
                if (dbMgr.BatchCommit())
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.SUCCESS;
                }
                else
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                }
            }

            AlarmData alarm = group.CurrentAlarm;

            if (alarm == null)
            {
                if (dbMgr.BatchCommit())
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.SUCCESS;
                }
                else
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                }
            }

            string strMessage = GetMalfunctionMessage(sensorZone.EquipZone, sensorZone.Type, alarm.IsReal);
            string strEquipZoneID = sensorZone.EquipZone == null ? null : sensorZone.EquipZone.ID.ToString();
            ProcessManager.DetectionStatus detectionStatus = alarm.IsReal ? ProcessManager.DetectionStatus.REAL : ProcessManager.DetectionStatus.TEST;
            VariousData<int> status = new VariousData<int>((int)detectionStatus);

            if (AlarmManager.Instance.RemoveAlarm(alarm, timeStamp, (int)ProcessManager.ReactionType.MALFUNCTION, strMessage, strEquipZoneID, sensorZone.ID.ToString(), nSOPGenUserID.ToString(), null, null, status, dbMgr))
            {
                if (dbMgr.BatchCommit())
                {
                    alarm.Message = strMessage;
                    group.CurrentAlarm = null;
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.SUCCESS;
                }
                else
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                }
            }

            dbMgr.BatchRollback();
            dbMgr.Close();
            WriteLog("RemoveAlarm 실패 : " + sensorZone.ID.ToString());
            return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
        }

        // 화재신호 복구
        private int RemoveAlarm(SensorZoneGroup group, SensorZone sensorZone, bool isReal)
        {
            // Transaction 처리를 위하여 별도의 객체를 만든다.
            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

            if (dbMgr.BeginBatch() == false)
            {
                dbMgr.Close();
                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
            }

            DateTime timeStamp = DateTime.Now;

            if (group.RemoveSensorData(sensorZone, dbMgr) == false)
            {
                dbMgr.BatchRollback();
                dbMgr.Close();
                WriteLog("RemoveSensorData 실패 : " + sensorZone.ID.ToString());
                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
            }

            // sensorZone의 신호는 복구되었지만 같은 영역에 다른 신호가 아직 남아있는 상황
            if (group.GetSensorDatas().Count > 0 && group.CurrentAlarm != null)
            {
                if (dbMgr.BatchCommit())
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.SUCCESS;
                }
                else
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                }
            }

            AlarmData alarm = group.CurrentAlarm;

            if (alarm == null)
            {
                if (dbMgr.BatchCommit())
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.SUCCESS;
                }
                else
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                }
            }

            string strMessage = GetClearSecurityMessage(sensorZone.EquipZone, sensorZone.Type, isReal);
            ProcessManager.DetectionStatus detectionStatus = isReal ? ProcessManager.DetectionStatus.REAL : ProcessManager.DetectionStatus.TEST;
            VariousData<int> status = new VariousData<int>((int)detectionStatus);

            if (AlarmManager.Instance.RemoveAlarm(alarm, timeStamp, (int)ProcessManager.ReactionType.END_STATUS, strMessage, null, null, null, null, null, status, dbMgr))
            {
                if (dbMgr.BatchCommit())
                {
                    alarm.Message = strMessage;
                    group.CurrentAlarm = null;
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.SUCCESS;
                }
                else
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                }
            }

            dbMgr.BatchRollback();
            dbMgr.Close();
            WriteLog("RemoveAlarm 실패 : " + sensorZone.ID.ToString());
            return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
        }

        private int AddAlarm(SensorZoneGroup group, int nSensorTagID, SensorZone sensorZone, bool isReal, out AlarmData alarm)
        {
            alarm = null;

            // 알람발생 신호에 대해서만 센서 비활성화를 검사한다.
            // 이미 알람이 발생한 센서의 경우 센서가 비활성화 상태이더라도 알람을 해제할 수 있어야 한다.
            if (SensorZoneManager.Instance.IsActiveSensor(nSensorTagID) == false)
            {
                WriteLog("AddAlarm 무시(비활성화된 센서) : " + sensorZone.ID.ToString());
                return SOPWebServer.ErrorMessageType.SUCCESS;
            }

            AlarmData currentAlarm = group.CurrentAlarm;
            int nSensorDataCount = group.GetSensorDatas().Count;

            if (currentAlarm == null && nSensorDataCount > 0)
            {
                DirectDBManager dbMgr = m_dbMgr.Clone();

                if (dbMgr.Connect() == false)
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                //  논리적인 오류
                group.ClearSensorDatas(dbMgr);
                dbMgr.Close();
            }
            else if (currentAlarm != null && nSensorDataCount > 0)
            {
                DirectDBManager dbMgr = m_dbMgr.Clone();

                if (dbMgr.Connect() == false)
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                // 이미 알람이 발생중이다.
                // Sensor 데이터만 기록하고 종료한다.
                group.SetSensorData(sensorZone, 1, dbMgr, false);
                AlarmManager.Instance.AddAlarmSensor(group.GetSensors(), currentAlarm.SensorZoneHistoryID, dbMgr);
                dbMgr.Close();
                return SOPWebServer.ErrorMessageType.SUCCESS;
            }
            else
            {
                // group 영역에 대하여 발생한 알람이 없다.
                // Transaction 처리를 위하여 객체를 새로 만든다.
                DirectDBManager dbMgr = m_dbMgr.Clone();

                if (dbMgr.Connect() == false)
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                if (dbMgr.BeginBatch() == false)
                {
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                }

                group.SetSensorData(sensorZone, 1, dbMgr, true);

                DateTime timeStamp = DateTime.Now;
                alarm = AlarmManager.Instance.AddAlarm(sensorZone.ID, 1, null, null, null, timeStamp, dbMgr);

                if (alarm != null)
                {
                    alarm.AlarmDepth = 1;
                    group.CurrentAlarm = alarm;

                    ProcessManager.DetectionStatus detectionStatus = isReal ? ProcessManager.DetectionStatus.REAL : ProcessManager.DetectionStatus.TEST;
                    VariousData<int> status = new VariousData<int>((int)detectionStatus);
                    string strMessage = GetDetectSecurityMessage(group.EquipmentZone, sensorZone.Type, isReal);
                    string strEquipZoneID = group.EquipmentZone == null ? null : group.EquipmentZone.ID.ToString();
                    ProcessManager.ReactionType reactionType = ProcessManager.ReactionType.BEGIN_STATUS;

                    string strParam3 = ((int)sensorZone.Type).ToString();

                    if (AlarmManager.Instance.AddReactionHistory(alarm, (int)reactionType, timeStamp, strMessage, strEquipZoneID, sensorZone.ID.ToString(), strParam3, null, null, status, dbMgr, true))
                    {
                        if (dbMgr.BatchCommit())
                        {
                            alarm.Message = strMessage;
                            alarm.IsReal = isReal;
                            alarm.Status = reactionType;
                            dbMgr.Close();
                            return SOPWebServer.ErrorMessageType.SUCCESS;
                        }
                        else
                        {
                            group.RemoveSensorData(sensorZone, null);
                            group.CurrentAlarm = null;
                        }
                    }
                    else
                    {
                        group.RemoveSensorData(sensorZone, dbMgr);
                        AlarmManager.Instance.RemoveAlarm(alarm);
                        WriteLog("AddReactionHistory 실패 : " + alarm.SensorZoneHistoryID.ToString());
                        alarm = null;
                        dbMgr.BatchRollback();
                    }
                }
                else
                {
                    group.RemoveSensorData(sensorZone, dbMgr);
                    WriteLog("AddAlarm 실패 : " + sensorZone.ID.ToString());
                    dbMgr.BatchRollback();
                }

                dbMgr.Close();
            }

            return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
        }

        public int ProcessReportSecurity(AlarmData alarm, int nSensorZoneHistoryID, int nEquipZoneID, int nSensorZoneID, int nSOPGenUserID)
        {
            // nSOPGenUserID에 해당하는 SOP Simulator가 제어권을 가지도록 한다.
            // SendSOPSimulatorControl(nSOPGenUserID) => 구현할 것

            // Transaction 처리를 위하여 객체를 새로 만든다.
            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

            if (dbMgr.BeginBatch() == false)
            {
                dbMgr.Close();
                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
            }

            DateTime dtNow = DateTime.Now;

            string strMessage = GetSecurityReportString(SensorZoneManager.Instance.GetEquipmentZone(nEquipZoneID), alarm.SensorType);
            string strParam1 = nEquipZoneID.ToString();
            string strParam2 = nSensorZoneID.ToString();
            string strParam3 = nSOPGenUserID.ToString();

            ProcessManager.DetectionStatus detectionStatus = alarm.IsReal ? BaseProcessManager.DetectionStatus.REAL : BaseProcessManager.DetectionStatus.TEST;
            VariousData<int> status = new VariousData<int>((int)detectionStatus);
            BaseProcessManager.ReactionType reactionType = BaseProcessManager.ReactionType.NOTIFY_SIGNAL;

            if (AlarmManager.Instance.AddReactionHistory(alarm, (int)reactionType, dtNow, strMessage, strParam1, strParam2, strParam3, null, null, status, dbMgr, true))
            {
                if (dbMgr.BatchCommit())
                {
                    alarm.Message = strMessage;
                    alarm.Status = reactionType;
                    dbMgr.Close();

                    if (dbMgr.Connect() == false)
                        return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

                    m_agentFactory.ProcessManager.ReportAlarm(dbMgr, alarm);
                    dbMgr.Close();
                    return SOPWebServer.ErrorMessageType.SUCCESS;
                }
                else
                {
                    dbMgr.Close();
                    WriteLog("ProcessReportSecurity 실패 : " + alarm.SensorZoneHistoryID.ToString());
                    return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                }
            }

            dbMgr.BatchRollback();
            dbMgr.Close();
            return SOPWebServer.ErrorMessageType.SUCCESS;
        }

        public static string GetEventTypeString(IFacility.FacilityType sensorType, out string strSensorBrandType)
        {
            string resultMsg = "";
            strSensorBrandType = "";

            switch (sensorType)
            {
                case IFacility.FacilityType.FIRE_SENSOR:
                case IFacility.FacilityType.FireSensor_TypeA:
                case IFacility.FacilityType.FireSensor_TypeB:
                case IFacility.FacilityType.FireSensor_GasEmission:
                case IFacility.FacilityType.FireSensor_ManualControl:
                case IFacility.FacilityType.FireSensor_LightType:
                case IFacility.FacilityType.FireSensor_SiemensType:
                case IFacility.FacilityType.FireSensor_Monitoring:
                case IFacility.FacilityType.FireSensor_SensingLine:
                case IFacility.FacilityType.FireSensor_AnalogSmokeType:
                case IFacility.FacilityType.FireSensor_MonitoringType:
                    resultMsg = "화재";
                    break;
                case IFacility.FacilityType.SecomFire:
                    strSensorBrandType = "SECOM";
                    resultMsg = "화재";
                    break;
                case IFacility.FacilityType.COOLER_SENSOR:
                    resultMsg = "소화 센서";
                    break;
                case IFacility.FacilityType.PRESSURE_SENSOR:
                    resultMsg = "압력 센서";
                    break;
                case IFacility.FacilityType.PSM_SENSOR:
                    resultMsg = "유해화학물질 누출감지 센서";
                    break;
                case IFacility.FacilityType.Intrusion_S1:
                    strSensorBrandType = "SVMS";
                    resultMsg = "침입";
                    break;
                case IFacility.FacilityType.Loiter_S1:
                    strSensorBrandType = "SVMS";
                    resultMsg = "배회";
                    break;
                case IFacility.FacilityType.Collapse_S1:
                    strSensorBrandType = "SVMS";
                    resultMsg = "넘어짐";
                    break;
                case IFacility.FacilityType.Theft_S1:
                    strSensorBrandType = "SVMS";
                    resultMsg = "도난";
                    break;
                case IFacility.FacilityType.Neglect_S1:
                    strSensorBrandType = "SVMS";
                    resultMsg = "방치";
                    break;
                case IFacility.FacilityType.VirtualFence_S1:
                    strSensorBrandType = "SVMS";
                    resultMsg = "(가상펜스)침입";
                    break;
                case IFacility.FacilityType.Fire_S1:
                    strSensorBrandType = "SVMS";
                    resultMsg = "SVMS화재";
                    break;
                case IFacility.FacilityType.EmergencyBell_S1:
                    strSensorBrandType = "SVMS";
                    resultMsg = "비상벨";
                    break;
                case IFacility.FacilityType.GeneralIntrusionT1_S1:
                    strSensorBrandType = "S1Access";
                    resultMsg = "침입";
                    break;
                case IFacility.FacilityType.GeneralIntrusionT2_S1:
                    strSensorBrandType = "S1Access";
                    resultMsg = "침입";
                    break;
                case IFacility.FacilityType.InternalIntrusionT3_S1:
                    strSensorBrandType = "S1Access";
                    resultMsg = "침입";
                    break;
                case IFacility.FacilityType.VaultIntrusionT4_S1:
                    strSensorBrandType = "S1Access";
                    resultMsg = "침입";
                    break;
                case IFacility.FacilityType.FireF1_S1:
                    strSensorBrandType = "S1Access";
                    resultMsg = "ACCESS화재";
                    break;
                case IFacility.FacilityType.CustomerEmergencyC1_S1:
                    strSensorBrandType = "S1Access";
                    resultMsg = "여자화장실 비상벨";
                    break;
                case IFacility.FacilityType.CustomerEmergencyC2_S1:
                    strSensorBrandType = "S1Access";
                    resultMsg = "여자화장실 비상벨";
                    break;
                case IFacility.FacilityType.RescueQQ_S1:
                    strSensorBrandType = "S1Access";
                    resultMsg = "구급";
                    break;
                case IFacility.FacilityType.GasG1_S1:
                    strSensorBrandType = "S1Access";
                    resultMsg = "가스누출";
                    break;
                case IFacility.FacilityType.BlackoutAbnormalityU1_S1:
                    strSensorBrandType = "S1Access";
                    resultMsg = "정전";
                    break;
                case IFacility.FacilityType.LeakAbnormalityU4_S1:
                    strSensorBrandType = "S1Access";
                    resultMsg = "누수";
                    break;
                case IFacility.FacilityType.SynthesisAlertAbnormalityU8_S1:
                    strSensorBrandType = "S1Access";
                    resultMsg = "종합경보반 이상";
                    break;
                case IFacility.FacilityType.ExternalAlarmBell:
                    strSensorBrandType = "S1Access";
                    resultMsg = "외부비상벨 호출";
                    break;
                case IFacility.FacilityType.SecomExternalAlarmBell:
                    strSensorBrandType = "SECOM";
                    resultMsg = "외부비상벨 호출";
                    break;
                case IFacility.FacilityType.SecomWomenAlarmBell:
                    strSensorBrandType = "SECOM";
                    resultMsg = "여자화장실 비상벨";
                    break;
                default:
                    break;
            }
            return resultMsg;
        }

        private string GetSecurityReportString(EquipmentZone equipZone, IFacility.FacilityType sensorType)
        {
            string strSensorBrand = "", strMessage = "";
            string strEventType = GetEventTypeString(sensorType, out strSensorBrand);

            if (equipZone == null)
            {
                if (strSensorBrand.Length > 0)
                    strMessage = string.Format("[{0}] {1} 상황이 신고되었습니다", strSensorBrand, strEventType);
                else
                    strMessage = string.Format("{0} 상황이 신고되었습니다", strEventType);
            }
            else
            {
                if (strSensorBrand.Length > 0)
                    strMessage = string.Format("[{0}][{1}]에서 {2} 상황이 신고되었습니다", strSensorBrand, equipZone.DisplayText, strEventType);
                else
                    strMessage = string.Format("[{0}]에서 {1} 상황이 신고되었습니다", equipZone.DisplayText, strEventType);
            }

            return strMessage;
        }

        private string GetDetectSecurityMessage(EquipmentZone equipZone, IFacility.FacilityType sensorType, bool isReal)
        {
            string strSensorBrand = "", strMessage = "";
            string strEventType = GetEventTypeString(sensorType, out strSensorBrand);

            if (isReal)
            {
                string strTag = GetTrainingModeString();

                if (equipZone == null)
                {
                    if (strSensorBrand.Length > 0)
                        strMessage = string.Format("{0}[{1}] {2} 탐지되었습니다", strTag, strSensorBrand, strEventType);
                    else
                        strMessage = string.Format("{0} {1} 탐지되었습니다", strTag, strEventType);
                }
                else
                {
                    if (strSensorBrand.Length > 0)
                        strMessage = string.Format("{0}[{1}][{2}]에서 {3} 탐지되었습니다", strTag, strSensorBrand, equipZone.DisplayText, strEventType);
                    else
                        strMessage = string.Format("{0}[{1}]에서 {2} 탐지되었습니다", strTag, equipZone.DisplayText, strEventType);
                }
            }
            else
            {
                if (equipZone == null)
                {
                    if (strSensorBrand.Length > 0)
                        strMessage = string.Format("[테스트][{0}] {1} 탐지되었습니다", strSensorBrand, strEventType);
                    else
                        strMessage = string.Format("[테스트] {0} 탐지되었습니다", strEventType);
                }
                else
                {
                    if (strSensorBrand.Length > 0)
                        strMessage = string.Format("[테스트][{0}][{1}]에서 {2} 탐지되었습니다", strSensorBrand, equipZone.DisplayText, strEventType);
                    else
                        strMessage = string.Format("[테스트][{0}]에서 {1} 탐지되었습니다", equipZone.DisplayText, strEventType);
                }
            }

            return strMessage;
        }

        private string GetTrainingModeString()
        {
            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return "";

            string strTag = m_agentFactory.SMSManager.GetTrainingModeString(dbMgr);
            dbMgr.Close();
            return strTag;
        }

        private string GetClearSecurityMessage(EquipmentZone equipZone, IFacility.FacilityType sensorType, bool isReal)
        {
            string strMessage = "상황해제";
            string strSensorBrand;
            string strEventType = GetEventTypeString(sensorType, out strSensorBrand);

            if (isReal)
            {
                string strTag = GetTrainingModeString();

                if (equipZone == null)
                {
                    if (strSensorBrand.Length > 0)
                        strMessage = string.Format("{0}[{1}] {2}신호가 현장 복구되었습니다", strTag, strSensorBrand, strEventType);
                    else
                        strMessage = string.Format("{0} {1}신호가 현장 복구되었습니다", strTag, strEventType);
                }
                else
                {
                    if (strSensorBrand.Length > 0)
                        strMessage = string.Format("{0}[{1}][{2}]에서 탐지된 {3}신호가 현장 복구되었습니다", strTag, strSensorBrand, equipZone.DisplayText, strEventType);
                    else
                        strMessage = string.Format("{0}[{1}]에서 탐지된 {2}신호가 현장 복구되었습니다", strTag, equipZone.DisplayText, strEventType);
                }
            }
            else
            {
                if (equipZone == null)
                {
                    if (strSensorBrand.Length > 0)
                        strMessage = string.Format("[테스트][{0}] {1}신호가 현장 복구되었습니다", strSensorBrand, strEventType);
                    else
                        strMessage = string.Format("[테스트] {0}신호가 현장 복구되었습니다", strEventType);
                }
                else
                {
                    if (strSensorBrand.Length > 0)
                        strMessage = string.Format("[테스트][{0}][{1}]에서 탐지된 {2}신호가 현장 복구되었습니다", strSensorBrand, equipZone.DisplayText, strEventType);
                    else
                        strMessage = string.Format("[테스트][{0}]에서 탐지된 {1}신호가 현장 복구되었습니다", equipZone.DisplayText, strEventType);
                }
            }

            return strMessage;
        }

        private string GetMalfunctionMessage(EquipmentZone equipZone, IFacility.FacilityType sensorType, bool isReal)
        {
            string strMessage = "오작동";
            string strSensorBrand;
            string strEventType = GetEventTypeString(sensorType, out strSensorBrand);

            if (isReal)
            {
                string strTag = GetTrainingModeString();

                if (equipZone == null)
                {
                    if (strSensorBrand.Length > 0)
                        strMessage = string.Format("{0}[{1}] {2}신호가 오작동으로 신고되었습니다", strTag, strSensorBrand, strEventType);
                    else
                        strMessage = string.Format("{0} {1}신호가 오작동으로 신고되었습니다", strTag, strEventType);
                }
                else
                {
                    if (strSensorBrand.Length > 0)
                        strMessage = string.Format("{0}[{1}][{2}]에서 탐지된 {3}신호가 오작동으로 신고되었습니다", strTag, strSensorBrand, equipZone.DisplayText, strEventType);
                    else
                        strMessage = string.Format("{0}[{1}]에서 탐지된 {2}신호가 오작동으로 신고되었습니다", strTag, equipZone.DisplayText, strEventType);
                }
            }
            else
            {
                if (equipZone == null)
                {
                    if (strSensorBrand.Length > 0)
                        strMessage = string.Format("[테스트][{0}] {1}신호가 오작동으로 신고되었습니다", strSensorBrand, strEventType);
                    else
                        strMessage = string.Format("[테스트] {0}신호가 오작동으로 신고되었습니다", strEventType);
                }
                else
                {
                    if (strSensorBrand.Length > 0)
                        strMessage = string.Format("[테스트][{0}][{1}]에서 탐지된 {2}신호가 오작동으로 신고되었습니다", strSensorBrand, equipZone.DisplayText, strEventType);
                    else
                        strMessage = string.Format("[테스트][{0}]에서 탐지된 {1}신호가 오작동으로 신고되었습니다", equipZone.DisplayText, strEventType);
                }
            }

            return strMessage;
        }
    }
}
