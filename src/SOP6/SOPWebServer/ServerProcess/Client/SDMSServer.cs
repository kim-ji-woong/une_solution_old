using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Collections;
using System.ServiceModel;
using AgentFactory;
using UnE.Spatial;
using UnE.PSM;
using System.Threading;

namespace ServerProcess.Client
{
    using ServerProcess.Data;

    public class SDMSServer : BaseClient
    {
        private static SDMSServer m_instance = null;

        public static SDMSServer Instance
        {
            get { return m_instance; }
        }

        public override int ClientType
        {
            get { return SOPWebServer.ClientType.SDMS; }
        }

        public SDMSServer()
            : base()
        {
            m_instance = this;
        }

        public SDMSServer(Factory factory, IPostOffice postOffice)
            : base(factory, postOffice)
        {
            m_instance = this;
            m_agent = m_agentFactory.MakeAgent(Factory.AgentType.SDMS);
        }

        protected override void OnLoadEvent()
        {
        }

        protected override ClientData MakeClientData(int nClientType, int nClientSubType, OperationContext ctx, string strIP, int nPort)
        {
            ClientData data = base.MakeClientData(nClientType, nClientSubType, ctx, strIP, nPort);

            if (data != null)
            {
                // 동기화 문제를 피하기 위하여 쓰레드를 사용한다.
                // 쓰레드 함수 내에서 0.1초 기다린다.
                Thread t = new Thread(new ParameterizedThreadStart(ProcessFirstConnection));
                t.Start(data);
            }

            return data;
        }

        protected override int OnReceiveEvent(ClientData data, OperationContext ctx, int header, byte[] messages, ArrayList arrDatas)
        {
            if (header == SOPWebServer.Header.SENSOR_MALFUNCTION)
                return ProcessMalfunction(arrDatas);
            else if (header == SOPWebServer.Header.SENSOR_USER_RESET)
                return ProcessUserReset(arrDatas);
            else if (header == SOPWebServer.Header.NOTIFY_DISASTER)
                return ProcessReportDisaster(arrDatas);
            else if (header == SOPWebServer.Header.CHANGE_CONFIG)
                return ProcessChangeConfig(header, arrDatas, messages);
            else if (header == SOPWebServer.Header.REQUEST_SENSOR_REACTION_HISTORY_DATA_LIST)
                return ProcessRequestReactionHistoryList(data);
            else if (header == SOPWebServer.Header.EDIT_SENSOR_ZONE)
                return ProcessEditSensorZone(ctx, header, messages, arrDatas);
            else if (header == SOPWebServer.Header.SDMS_COMMAND)
                return ProcessSDMSCommand(ctx, header, messages, arrDatas);
            else if (header == SOPWebServer.Header.CLEAR_DETECT_REPORT)
                return ProcessClearDetectReport(arrDatas);

            return SOPWebServer.ErrorMessageType.UNKNOWN_HEADER;
        }

        private int ProcessSDMSCommand(OperationContext ctx, int header, byte[] messages, ArrayList arrDatas)
        {
            if (arrDatas != null)
            {
                int nDataCount = arrDatas.Count;

                if (nDataCount > 0 && arrDatas[0] is byte)
                {
                    byte cmd = (byte)arrDatas[0];

                    if (cmd == SOPWebServer.SDMSCommandType.CHANGE_PSM_SENSOR_STATUS)
                    {
                        if (ProcessChangePSMSensorStatus(arrDatas))
                            return SendDataToSDMSClient(SOPWebServer.Header.SDMS_COMMAND, arrDatas);
                        else
                            return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                    }
                    else if (cmd == SOPWebServer.SDMSCommandType.PSM_SENSOR_ALARM_LEVEL)
                    {
                        if (SavePSMSensorAlarmLevel(arrDatas))
                            return SendDataToSDMSClient(SOPWebServer.Header.SDMS_COMMAND, arrDatas);
                        else
                            return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                    }
                    else if (cmd == SOPWebServer.SDMSCommandType.CHANGE_TAG_ACTIVATION)
                    {
                        if (SaveChangeTagActivation(arrDatas))
                            return SendDataToSDMSClient(SOPWebServer.Header.SDMS_COMMAND, arrDatas);
                        else
                            return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                    }
                    else
                        return SendDataToSDMSClient(SOPWebServer.Header.SDMS_COMMAND, arrDatas);
                }
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        private bool SaveChangeTagActivation(ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;

            if (nDataCount > 2 && (arrDatas[1] is int))
            {
                int endOfDatas = (int)arrDatas[1] + 2;

                for (int i = 2; i < endOfDatas; i++)
                {
                    int tagID = (int)arrDatas[i];
                    string deActivationCode = (string)arrDatas[++i];
                    bool isActive = true;

                    if (deActivationCode == "N" || deActivationCode == "n")
                        isActive = true;
                    else if (deActivationCode == "Y" || deActivationCode == "y")
                        isActive = false;
                    else
                        return false;

                    SensorZoneManager.Instance.SetSensorActivation(tagID, isActive);
                }

                return true;
            }

            return false;
        }

        private bool SavePSMSensorAlarmLevel(ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;

            if (nDataCount >= 5 && (arrDatas[1] is int) && (arrDatas[2] is float) && (arrDatas[3] is float) && (arrDatas[4] is float))
            {
                int nPSMSensorID = (int)arrDatas[1];
                float fLevel1 = (float)arrDatas[2];
                float fLevel2 = (float)arrDatas[3];
                float fLevel3 = (float)arrDatas[4];

                string strSQL = string.Format("Update PSMSensor set LimitLevel1 = {0}, LimitLevel2 = {1}, LimitLevel3 = {2} where ID = {3}",
                    fLevel1, fLevel2, fLevel3, nPSMSensorID);

                DirectDBManager dbMgr = m_dbMgr.Clone();

                if (dbMgr.Connect() == false)
                    return false;

                if (dbMgr.GetResultData(strSQL) != null)
                {
                    PSMSensor sensor = PSMManager.Instance.GetSensor(nPSMSensorID);

                    if (sensor != null)
                    {
                        sensor.LimitLevel1 = fLevel1;
                        sensor.LimitLevel2 = fLevel2;
                        sensor.LimitLevel3 = fLevel3;
                    }

                    dbMgr.Close();
                    return true;
                }

                dbMgr.Close();
            }

            return false;
        }

        private int SendDataToSDMSClient(int header, ArrayList arrDatas)
        {
            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            SendClientData(header, bytes, SOPWebServer.ClientType.SDMS, -1);
            return SOPWebServer.ErrorMessageType.SUCCESS;
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
                int nSOPGenUserID = (int)arrDatas[5];

                PSMSensor sensor = PSMManager.Instance.GetSensor(nSensorID);

                if (sensor != null)
                {
                    bool prevOff = PSMManager.IsOff(sensor);
                    PSMSensor.Status _status = PSMSensor.ToStatus((int)status);

                    bool spreadStatus = sensor.SensorStatus == PSMSensor.Status.Off4Work || _status == PSMSensor.Status.Off4Work;
                    sensor.SensorStatus = _status;

                    if (beginTime != 0)
                    {
                        DateTime dtBegin = DateTime.FromBinary(beginTime);
                        sensor.BeginWorkTime = new VariousData<DateTime>(dtBegin);
                    }
                    else
                        sensor.BeginWorkTime = null;

                    if (endTime != 0)
                    {
                        DateTime dtEnd = DateTime.FromBinary(endTime);
                        sensor.EndWorkTime = new VariousData<DateTime>(dtEnd);
                    }
                    else
                        sensor.EndWorkTime = null;

                    bool currentOff = PSMManager.IsOff(sensor);

                    DirectDBManager dbMgr = m_dbMgr.Clone();

                    if (dbMgr.Connect() == false)
                        return false;

                    bool isSuccess = ClearPSMSensorAlarmNChangeStatusDB(sensor, nSOPGenUserID, spreadStatus, prevOff != currentOff, dbMgr);
                    dbMgr.Close();
                    return isSuccess;
                }
            }

            return false;
        }

        // spreadStatus : 같은 센서를 공유하는 다른 Tank들에도 상태정보가 변경된 것을 전파할 것인가?
        public static bool ClearPSMSensorAlarmNChangeStatusDB(PSMSensor sensor, int nSOPGenUserID, bool spreadStatus, bool onOffIsChanged, DirectDBManager dbMgr)
        {
            if (onOffIsChanged)
            {
                // sensor와 관련된 알람을 해제한다.
                ClearPSMSensorAlarm(sensor, nSOPGenUserID, dbMgr);

                if (sensor.SensorStatus == PSMSensor.Status.On)
                {
                    RequestPSMSensorAlarm(sensor);
                }
            }

            if (spreadStatus)
            {
                List<PSMSensor> sensors = sensor.GetSameSensors();
                sensors.Add(sensor);
                return ChangePSMSensorStatusDBDatas(sensors, dbMgr);
            }

            return ChangePSMSensorStatusDBData(sensor, dbMgr);
        }

        private static bool ChangePSMSensorStatusDBDatas(List<PSMSensor> sensors, DirectDBManager dbMgr)
        {
            foreach (PSMSensor sensor in sensors)
            {
                if (!ChangePSMSensorStatusDBData(sensor, dbMgr))
                    return false;
            }

            return true;
        }

        private static bool ChangePSMSensorStatusDBData(PSMSensor sensor, DirectDBManager dbMgr)
        {
            string strSQL = "Select SensorID from PSMSensorSchedule where SensorID = " + sensor.ID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            if (arrResult.Count == 0)
                return InsertPSMSensorStatus(sensor, dbMgr);
            else
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

                if (id == null)
                    return InsertPSMSensorStatus(sensor, dbMgr);
            }

            return UpdatePSMSensorStatus(sensor, dbMgr);
        }

        private static bool InsertPSMSensorStatus(PSMSensor sensor, DirectDBManager dbMgr)
        {
            string strBeginTime = GetTimeString(sensor.BeginWorkTime);
            string strEndTime = GetTimeString(sensor.EndWorkTime);

            string strSQL = string.Format("Insert into PSMSensorSchedule (SensorID, Status, BeginTime, EndTime, Description) values ({0}, {1}, {2}, {3}, NULL)",
                sensor.ID, (int)sensor.SensorStatus, strBeginTime, strEndTime);

            return dbMgr.GetResultData(strSQL) != null;
        }

        private static bool UpdatePSMSensorStatus(PSMSensor sensor, DirectDBManager dbMgr)
        {
            string strBeginTime = GetTimeString(sensor.BeginWorkTime);
            string strEndTime = GetTimeString(sensor.EndWorkTime);

            string strSQL = string.Format("Update PSMSensorSchedule set Status = {0}, BeginTime = {1}, EndTime = {2} where SensorID = {3}",
                (int)sensor.SensorStatus, strBeginTime, strEndTime, sensor.ID);

            return dbMgr.GetResultData(strSQL) != null;
        }

        private static string GetTimeString(VariousData<DateTime> time)
        {
            if (time == null)
                return "NULL";

            string strTime = string.Format("'{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}'",
                time.Data.Year, time.Data.Month, time.Data.Day,
                time.Data.Hour, time.Data.Minute, time.Data.Second);

            return strTime;
        }

        // PSMSensorServer에게 sensor와 관련된 알람이 존재하면 보내줄 것을 요청한다.
        private static void RequestPSMSensorAlarm(PSMSensor sensor)
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(SOPWebServer.ServerCommandType.REQUEST_PSM_SENSOR_ALARM);
            arrDatas.Add(sensor.ID);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            PSMSensorServer.Instance.SendClientData(SOPWebServer.Header.SERVER_COMMAND, bytes, SOPWebServer.ClientType.PSM_SENSOR_SERVER, -1);
        }

        // sensor와 관련된 알람을 해제한다.
        public static void ClearPSMSensorAlarm(PSMSensor sensor, int nSOPGenUserID, DirectDBManager dbMgr)
        {
            EquipmentZone equipZone = SensorZoneManager.Instance.GetEquipmentZone(sensor.EquipZoneID);

            SensorZoneGroup group;
            SensorZone sensorZone = SensorZoneManager.Instance.GetSensorZone(sensor.ID, UnE.Sensor.IFacility.FacilityType.PSM_SENSOR, equipZone, out group);

            if (sensorZone == null || group == null)
                return;

            AlarmData alarm = group.CurrentAlarm;

            if (alarm == null)
                return;

            VariousData<int> status = new VariousData<int>((int)ProcessManager.DetectionStatus.REAL);
            AlarmManager.Instance.RemoveAlarm(alarm, DateTime.Now, (int)ProcessManager.ReactionType.MALFUNCTION, "탐지된 누출신호가 무시됩니다.", equipZone.ID.ToString(), sensorZone.ID.ToString(), nSOPGenUserID.ToString(), null, null, status, dbMgr);
        }

        private int ProcessEditSensorZone(OperationContext ctx, int header, byte[] messages, ArrayList arrDatas)
        {
            if (arrDatas != null)
            {
                int nDataCount = arrDatas.Count;

                if (nDataCount % 4 != 0)
                    return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;

                for (int i=0;i<nDataCount;i+=4)
                {
                    int nSensorZoneID = (int)arrDatas[i];
                    int nOriginEquipZoneID = (int)arrDatas[i + 1];
                    int nChangedEquipZoneID = (int)arrDatas[i + 2];
                    int nZoneID = (int)arrDatas[i + 3];

                    SensorZone sensorZone = SensorZoneManager.Instance.GetSensorZone(nSensorZoneID);

                    if (sensorZone == null)
                        continue;

                    EquipmentZone equipZoneOrigin = SensorZoneManager.Instance.GetEquipmentZone(nOriginEquipZoneID);
                    EquipmentZone equipZoneChanged = SensorZoneManager.Instance.GetEquipmentZone(nChangedEquipZoneID);

                    if (equipZoneOrigin != null)
                    {
                        List<SensorZone> sensorZones = SensorZoneManager.Instance.GetEquipZoneSensorZones(equipZoneOrigin);

                        if (sensorZones != null)
                        {
                            sensorZones.Remove(sensorZone);
                        }
                    }

                    if (equipZoneChanged != null)
                    {
                        List<SensorZone> sensorZones = SensorZoneManager.Instance.GetEquipZoneSensorZones(equipZoneChanged);
                        
                        if (sensorZones == null)
                        {
                            sensorZones = new List<SensorZone>();
                            SensorZoneManager.Instance.SetEquipZoneSensorZones(equipZoneChanged, sensorZones);
                        }

                        if (!sensorZones.Contains(sensorZone))
                            sensorZones.Add(sensorZone);
                    }

                    sensorZone.ZoneID = nZoneID;
                    sensorZone.EquipZone = equipZoneChanged;

                    if (!UpdateSensorZoneDB(sensorZone))
                        return SOPWebServer.ErrorMessageType.DB_EXCEPTION;
                }

                SendEditSensorZone(ctx, header, messages);
                return SOPWebServer.ErrorMessageType.SUCCESS;
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        private void SendEditSensorZone(OperationContext ctx, int header, byte[] bytes)
        {
            ClientData thisClientData = null;

            if (m_postOffice != null)
            {
                IPostMan postMan = m_postOffice.GetPostMan(ctx);

                if (postMan != null)
                    thisClientData = postMan.ClientData;
            }

            SendClientData(header, bytes, SOPWebServer.ClientType.SDMS, -1, thisClientData);
            FireSensorServer.Instance.SendClientData(header, bytes, SOPWebServer.ClientType.FIRE_SENSOR_SERVER, -1);
            PSMSensorServer.Instance.SendClientData(header, bytes, SOPWebServer.ClientType.PSM_SENSOR_SERVER, -1);
        }

        private bool UpdateSensorZoneDB(SensorZone sensorZone)
        {
            if (sensorZone == null)
                return true;

            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return false;

            string strSQL = string.Format("Update SensorZone set EquipZoneID = {0}, Zone = {1} where ID = {2}",
                sensorZone.EquipZone == null ? 0 : sensorZone.EquipZone.ID,
                sensorZone.ZoneID,
                sensorZone.ID);

            bool isSuccess = dbMgr.GetResultData(strSQL) != null;
            dbMgr.Clone();
            return isSuccess;
        }

        private int ProcessRequestReactionHistoryList(ClientData data)
        {
            List<AlarmData> alarms = AlarmManager.Instance.CurrentAlarms;

            if (alarms.Count == 0)
                return SOPWebServer.ErrorMessageType.SUCCESS;

            ArrayList arrDatas = new ArrayList();
            //arrDatas.Add(alarms.Count);

            foreach (AlarmData alarm in alarms)
            {
                arrDatas.Add(alarm.SensorReactionHistoryID);
                arrDatas.Add(alarm.SensorZoneHistoryID);
                arrDatas.Add((int)alarm.Status);
                arrDatas.Add(alarm.TimeStamp.ToBinary());
                arrDatas.Add(alarm.Message);
                arrDatas.Add(alarm.ReactionHistoryParam1);
                arrDatas.Add(alarm.ReactionHistoryParam2);
                arrDatas.Add(alarm.ReactionHistoryParam3);
                arrDatas.Add(alarm.ReactionHistoryParam4);
                arrDatas.Add(alarm.ReactionHistoryParam5);
            }

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            SendClientData(SOPWebServer.Header.SENSOR_REACTION_HISTORY_DATA_LIST, bytes, data);
            /*arrDatas.Clear();

            arrDatas.Add(ctx);
            arrDatas.Add(SOPWebServer.Header.SENSOR_REACTION_HISTORY_DATA_LIST);
            arrDatas.Add(bytes);

            m_agent.TimerDatas.Enqueue(arrDatas);*/

            return SOPWebServer.ErrorMessageType.SUCCESS;
        }

        private int ProcessChangeConfig(int header, ArrayList arrDatas, byte[] bytes)
        {
            if (arrDatas != null && arrDatas.Count >= 3 && arrDatas[0] is int && arrDatas[1] is string && arrDatas[2] is string)
            {
                try
                {
                    int nClientType = (int)arrDatas[0];
                    string strPropertyName = (string)arrDatas[1];
                    string strPropertyValue = (string)arrDatas[2];

                    if (nClientType != SOPWebServer.ClientType.SDMS)
                        return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;

                    if (strPropertyName == SOP.SDMSConfig.PropertyName)
                    {
                        int nConfigValue;

                        if (int.TryParse(strPropertyValue, out nConfigValue))
                        {
                            if (((nConfigValue & (int)SOP.SDMSConfig.ConfigType.EQUIPZONE_FACILITY_MANAGER) == (int)SOP.SDMSConfig.ConfigType.EQUIPZONE_FACILITY_MANAGER) ||
                                ((nConfigValue & (int)SOP.SDMSConfig.ConfigType.BUILDING_FACILITY_MANAGER) == (int)SOP.SDMSConfig.ConfigType.BUILDING_FACILITY_MANAGER) ||
                                ((nConfigValue & (int)SOP.SDMSConfig.ConfigType.ENTIRE_FACILITY_MANAGER) == (int)SOP.SDMSConfig.ConfigType.ENTIRE_FACILITY_MANAGER))
                                return ProcessChangeFacilityManager(header, bytes);
                        }
                    }
                    else if (strPropertyName == SOP.SDMSConfig.GetPropertyName(SOP.SDMSConfig.ConfigType.EQUIPZONE_CCTV))
                    {
                        int nEquipZoneID;

                        if (int.TryParse(strPropertyValue, out nEquipZoneID))
                        {
                            return ProcessChangeEquipZoneCCTV(header, bytes);
                        }
                        else
                            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
                    }
                    else
                        return SOPWebServer.ErrorMessageType.UNKNOWN_CONFIG;
                }
                catch (Exception ex)
                {
                    WriteLog("ProcessChangedConfig : " + ex.Message);
                }
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        private int ProcessChangeEquipZoneCCTV(int header, byte[] bytes)
        {
            SendClientData(header, bytes, SOPWebServer.ClientType.SDMS, -1);
            /*ArrayList arrDatas = new ArrayList();

            arrDatas.Add(SOPWebServer.ClientType.SDMS);
            arrDatas.Add(SOPWebServer.ClientSubType.SDMS);
            arrDatas.Add(header);
            arrDatas.Add(bytes);

            m_agent.TimerDatas.Enqueue(arrDatas);*/

            return SOPWebServer.ErrorMessageType.SUCCESS;
        }

        private int ProcessChangeFacilityManager(int header, byte[] bytes)
        {
            SendClientData(header, bytes, SOPWebServer.ClientType.SDMS, -1);
            /*ArrayList arrDatas = new ArrayList();
            
            arrDatas.Add(SOPWebServer.ClientType.SDMS);
            arrDatas.Add(SOPWebServer.ClientSubType.SDMS);
            arrDatas.Add(header);
            arrDatas.Add(bytes);

            m_agent.TimerDatas.Enqueue(arrDatas);*/

            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

            MemberManager.Instance.LoadFacilityManager(dbMgr);
            dbMgr.Close();
            return SOPWebServer.ErrorMessageType.SUCCESS;
        }

        private int ProcessReportDisaster(ArrayList arrDatas)
        {
            if (arrDatas.Count >= 4 && arrDatas[0] is int && arrDatas[1] is int && arrDatas[2] is int && arrDatas[3] is int)
            {
                int nSensorZoneHistoryID = (int)arrDatas[0];
                // 수동화재신고일 경우 EquipZone ID가 아니라 Zone ID 이다.
                int nEquipZoneID = (int)arrDatas[1];
                int nSensorZoneID = (int)arrDatas[2];
                int nSOPGenUserID = (int)arrDatas[3];
                
                // 수동 신고
                if (nSensorZoneHistoryID == 0 && SOPWebServer.Header.ManualReportDefaultID <= nSensorZoneID)
                {
                    int nType = nSensorZoneID - SOPWebServer.Header.ManualReportDefaultID;
                    UnE.Sensor.IFacility.FacilityType facility = UnE.Sensor.IFacility.ToFacilityType(nType);
                    
                    string strMemo = "";
                    if (arrDatas.Count > 4)
                        strMemo = (string)arrDatas[4];

                    int nAlarmStep = 2;
                    if (arrDatas.Count > 5)
                        nAlarmStep = (int)arrDatas[5];

                    if (facility == UnE.Sensor.IFacility.FacilityType.FIRE_SENSOR)
                        return FireSensorServer.Instance.ProcessManualReportFire(nSensorZoneID, nEquipZoneID, nSOPGenUserID, strMemo, nAlarmStep);
                    else if (facility == UnE.Sensor.IFacility.FacilityType.PSM_SENSOR)
                        return PSMSensorServer.Instance.ProcessManualReportPSM(nSensorZoneID, nEquipZoneID, nSOPGenUserID, strMemo, nAlarmStep);
                    else
                        return EtcSensorServer.Instance.ProcessManualReportEtc(nSensorZoneID, nEquipZoneID, nSOPGenUserID, strMemo, nAlarmStep);
                }

                SensorZone sensorZone = SensorZoneManager.Instance.GetSensorZone(nSensorZoneID);

                if (sensorZone != null)
                {
                    AlarmData alarm = AlarmManager.Instance.GetAlarm(nSensorZoneHistoryID);
                    
                    if (alarm == null)
                        return SOPWebServer.ErrorMessageType.NO_SENSORZONE_HISTORY_ALARM;
                    else if (alarm.Status == BaseProcessManager.ReactionType.NOTIFY_SIGNAL)
                        return SOPWebServer.ErrorMessageType.ALREADY_PROCESSED;

                    if (BaseBroadcastManager.IsFireSensor(sensorZone.Type))
                        return FireSensorServer.Instance.ProcessReportFire(alarm, nSensorZoneHistoryID, nEquipZoneID, nSensorZoneID, nSOPGenUserID);
                    else if (BaseBroadcastManager.IsPSMSensor(sensorZone.Type))
                        return PSMSensorServer.Instance.ProcessReportPSM(alarm, nSensorZoneHistoryID, nEquipZoneID, nSensorZoneID, nSOPGenUserID);
                    else if (BaseBroadcastManager.IsSecuritySensor(sensorZone.Type))
                        return SecuritySensorServer.Instance.ProcessReportSecurity(alarm, nSensorZoneHistoryID, nEquipZoneID, nSensorZoneID, nSOPGenUserID);
                }
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        private int ProcessUserReset(ArrayList arrDatas)
        {
            if (arrDatas.Count >= 4 && arrDatas[0] is int && arrDatas[1] is int && arrDatas[2] is int && arrDatas[3] is string)
            {
                int nSensorZoneHistoryID = (int)arrDatas[0];
                int nSensorZoneID = (int)arrDatas[1];
                int nSOPGenUserID = (int)arrDatas[2];
                string strDescription = (string)arrDatas[3];

                SensorZone sensorZone = SensorZoneManager.Instance.GetSensorZone(nSensorZoneID);

                if (sensorZone != null)
                {
                    if (BroadcastManager.IsPSMSensor(sensorZone.Type))
                        return PSMSensorServer.Instance.ProcessUserReset(nSensorZoneHistoryID, nSensorZoneID, nSOPGenUserID, strDescription);
                    else if (BroadcastManager.IsEarthquakeSensor(sensorZone.Type))
                        return EarthquakeSensorServer.Instance.ProcessUserReset(nSensorZoneHistoryID, nSensorZoneID, nSOPGenUserID, strDescription);
                    else if (BroadcastManager.IsTemperatureHumiditySensor(sensorZone.Type))
                        return TempHumidityServer.Instance.ProcessUserReset(nSensorZoneHistoryID, nSensorZoneID, nSOPGenUserID, strDescription);
                }
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        private int ProcessMalfunction(ArrayList arrDatas)
        {
            if (arrDatas.Count >= 4 && arrDatas[0] is int && arrDatas[1] is int && arrDatas[2] is int && arrDatas[3] is string)
            {
                int nSensorZoneHistoryID = (int)arrDatas[0];
                int nSensorZoneID = (int)arrDatas[1];
                int nSOPGenUserID = (int)arrDatas[2];
                string strDescription = (string)arrDatas[3];

                SensorZone sensorZone = SensorZoneManager.Instance.GetSensorZone(nSensorZoneID);

                if (sensorZone != null)
                {
                    if (BaseBroadcastManager.IsFireSensor(sensorZone.Type))
                        return FireSensorServer.Instance.ProcessMalfunction(nSensorZoneHistoryID, nSensorZoneID, nSOPGenUserID, strDescription);
                    else if (BaseBroadcastManager.IsSecuritySensor(sensorZone.Type))
                        return SecuritySensorServer.Instance.ProcessMalfunction(nSensorZoneHistoryID, nSensorZoneID, nSOPGenUserID, strDescription);
                    else if (BaseBroadcastManager.IsETCSensor(sensorZone.Type))
                        return EtcSensorServer.Instance.ProcessMalfunction(nSensorZoneHistoryID, nSensorZoneID, nSOPGenUserID, strDescription);
                }
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        public void SendSensorZoneData(int nData, AlarmData alarm)
        {
            if (alarm.SensorZoneID == 0)
                return;

            SensorZone sensorZone = SensorZoneManager.Instance.GetSensorZone(alarm.SensorZoneID);

            if (sensorZone == null || sensorZone.EquipZone == null)
                return;

            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(alarm.SensorZoneID);
            arrDatas.Add((int)alarm.SensorType);
            arrDatas.Add(sensorZone.IsConnected ? 1 : 0);
            arrDatas.Add(sensorZone.EquipZone.ID);
            arrDatas.Add(nData);
            arrDatas.Add(sensorZone.LinkedSensorID);
            
            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);

            // Debuggin을 위한 Break Point를 사용하면 아래의 로직으로 인하여 서버가 Block된다.
            // Timer를 사용하도록 한다.
            SendClientData(SOPWebServer.Header.SENSOR_ZONE_DATA, bytes, SOPWebServer.ClientType.SDMS, -1);
            //SendMessageToClient(-1, -1, SOPWebServer.Header.SENSOR_ZONE_DATA, bytes, null);
        }

        public void SendSensorReactionLog(AlarmData alarm)
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(alarm.SensorReactionHistoryID);
            arrDatas.Add(alarm.SensorZoneHistoryID);
            arrDatas.Add((int)alarm.Status);
            arrDatas.Add(alarm.TimeStamp.ToBinary());
            arrDatas.Add(alarm.Message);
            arrDatas.Add(alarm.ReactionHistoryParam1);
            arrDatas.Add(alarm.ReactionHistoryParam2);
            arrDatas.Add(alarm.ReactionHistoryParam3);
            arrDatas.Add(alarm.ReactionHistoryParam4);
            arrDatas.Add(alarm.ReactionHistoryParam5);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);

            // Debuggin을 위한 Break Point를 사용하면 아래의 로직으로 인하여 서버가 Block된다.
            // Timer를 사용하도록 한다.
            SendClientData(SOPWebServer.Header.SENSOR_REACTION_HISTORY_DATA, bytes, SOPWebServer.ClientType.SDMS, -1);
            //SendMessageToClient(-1, -1, SOPWebServer.Header.SENSOR_REACTION_HISTORY_DATA, bytes, null);
        }

        private void ProcessFirstConnection(object arg)
        {
            // 동기화 문제를 피하기 위하여 0.1초 기다린다.
            Thread.Sleep(100);

            BaseAgent.MethodProcessType processType = m_agent.CheckMethod(BaseAgent.MethodType.Etc, "ProcessFirstConnection");

            if (processType == BaseAgent.MethodProcessType.PreProcess)
                m_agent.RunMethod(BaseAgent.MethodType.Etc, "ProcessFirstConnection", m_dbMgr, this, AlarmManager.Instance);

            ClientData data = (ClientData)arg;

            // 현재 진행중인 화재들에 대한 마지막 Log List를 전송한다.
            ProcessRequestReactionHistoryList(data);
            SendLastReadSDMSMessageID(data);

            if (processType == BaseAgent.MethodProcessType.PostProcess)
                m_agent.RunMethod(BaseAgent.MethodType.Etc, "ProcessFirstConnection", m_dbMgr, this, AlarmManager.Instance);
        }

        public void SendClearAlarm(AlarmData alarm)
        {
            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(alarm.SensorZoneHistoryID);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);

            // Debuggin을 위한 Break Point를 사용하면 아래의 로직으로 인하여 서버가 Block된다.
            // Timer를 사용하도록 한다.
            SendClientData(SOPWebServer.Header.CLEAR_DETECT_REPORT, bytes, SOPWebServer.ClientType.SDMS, -1);
            //SendMessageToClient(-1, -1, SOPWebServer.Header.CLEAR_DETECT_REPORT, bytes, null);
        }

        private int ProcessClearDetectReport(ArrayList arrDatas)
        {
            int nPrevSensorHistoryID = (int)arrDatas[0];
            int nGenUserID = (int)arrDatas[1];

            if (nPrevSensorHistoryID < 0)
                return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;

            AlarmData alarm = AlarmManager.Instance.GetAlarm(nPrevSensorHistoryID);

            if (alarm == null)
                return SOPWebServer.ErrorMessageType.DB_EXCEPTION;

            SensorZoneGroup group = SensorZoneManager.Instance.GetSensorZoneGroup(alarm.SensorZoneID);
            SensorZone sensorZone = SensorZoneManager.Instance.GetSensorZone(alarm.SensorZoneID);

            // 알람 해제
            //int nResult = FireSensorServer.Instance.RemoveAlarm(group, sensorZone, true);

            int nResult = -1;

            if (alarm.IsManual)
            {
                if (alarm.SensorType == UnE.Sensor.IFacility.FacilityType.FIRE_SENSOR)
                    nResult = FireSensorServer.Instance.RemoveManualAlarm(alarm);
                else if (alarm.SensorType == UnE.Sensor.IFacility.FacilityType.PSM_SENSOR)
                    nResult = PSMSensorServer.Instance.RemoveManualAlarm(alarm);
                else 
                    nResult = EtcSensorServer.Instance.RemoveManualAlarm(alarm);
            }
            else
                nResult = FireSensorServer.Instance.RemoveAlarm(group, sensorZone, true);

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

        protected override void OnTimerEvent()
        {
            base.OnTimerEvent();

            /*List<Client.ClientData> clients = GetClientDatas();

            if (clients.Count == 0)
                return;

            List<Client.ClientData> removeClients = new List<Client.ClientData>();

            foreach (Client.ClientData client in clients)
            {
                IClientChannel channel = client.PostMan.ClientChannel;

                if (channel.State == CommunicationState.Opened)
                    client.PostMan.OnRing(SOPWebServer.Header.ARE_YOU_THERE, null);
                else
                    removeClients.Add(client);
            }

            foreach (Client.ClientData client in removeClients)
            {
                RemoveClient(client);
            }

            removeClients.Clear();
            clients.Clear();*/

            // 1. SDMSConfig 변경이 있는가를 2초에 한번씩 검사한다.
            // 2. 새로운 SDMSMessage가 있는지 검사하여, 있으면 SDMS Client들에게 알린다.
            if (DateTime.Now.Second % 2 == 0)
            {
                DirectDBManager dbMgr = m_dbMgr.Clone();
                
                if (dbMgr.Connect())
                {
                    Data.SDMSConfigWatcher.Instance.Watch(dbMgr);
                    Data.SDMSMessageWatcher.Instance.ReadNewMessage(dbMgr);

                    dbMgr.Close();
                }
            }
        }
        
        private void SendLastReadSDMSMessageID(ClientData data)
        {
            int nLastReadID = Data.SDMSMessageWatcher.Instance.LastReadID;

            if (nLastReadID < 0)
                return;

            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(SOPWebServer.SDMSCommandType.SDMS_PUBLIC_MESSAGE_ID);
            arrDatas.Add(nLastReadID);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            SendClientData(SOPWebServer.Header.SDMS_COMMAND, bytes, SOPWebServer.ClientType.SDMS, -1);
        }

        public void SendChangedConfig(int nConfigData)
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(SOPWebServer.ClientType.SDMS);
            arrDatas.Add(SOP.SDMSConfig.PropertyName);
            arrDatas.Add(nConfigData.ToString());

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            SendClientData(SOPWebServer.Header.CHANGE_CONFIG, bytes, SOPWebServer.ClientType.SDMS, -1);
        }
    }
}
