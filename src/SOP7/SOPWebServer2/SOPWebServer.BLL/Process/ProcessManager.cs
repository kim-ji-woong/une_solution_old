using System;
using System.Collections.Generic;
using AgentFactory.BLL;
using dnsData.Alarm;
using dnsData.Sensor;
using SDMS.Model.Config;
using SDMS.Model.Alarm;
using SDMS.Model.History;
using dnsSopID;
using System.IO;
using System.Collections;
using dnsDBUtil;

namespace SOPWebServer.BLL.Process
{
    public class ProcessManager : BaseProcessManager
    {
        private MainManager m_mainManager = null;
        private static string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

        public ProcessManager(Factory factory, MainManager mainManager)
            : base(factory)
        {
            m_mainManager = mainManager;
            factory.ProcessManager = this;
        }

        public override void NewAlarm(AlarmData alarm, List<int> alarmSensorZoneIDs)
        {
            // 알람 발생전 할일
            List<ClientMessage> messages = m_processAgent.PrevNewAlarm(alarm, m_mainManager.AlarmManager);
            ProcessClientMessages(messages);

            ProcessAlarm(alarm);

            int nAlarmType = alarm.Status == SDMS.Model.History.SensorReactionHistory.ReactionTypes.NOTIFY_SIGNAL ? (int)CurrentAlarm.AlarmTypes.Report : (int)CurrentAlarm.AlarmTypes.Detect;
            int nSopStatus = -1; //(-1: SOP 시작 하기전, 0: SOP 실행 요청, 1: SOP 실행중)
            // 알람은 주의, 경계, 심각 3단계로 표현
            // eSOP는 경계, 심각단계에서 자동실행되며, 주의 단계의 경우 ‘상황전파’ 버튼을 통한 실행

            if (alarm.AlarmDepth >= 3)
                nSopStatus = 0;

            m_mainManager.SDMSDataManager.GetCreateManager().CreateCurrentAlarm(alarm.SensorZoneHistoryID, (int)alarm.SensorType, nAlarmType, alarm.TimeStamp, nSopStatus, alarm.AlarmDepth, alarmSensorZoneIDs);

            // 알람 발생후 할일
            messages = m_processAgent.PostNewAlarm(alarm, m_mainManager.AlarmManager);
            ProcessClientMessages(messages);
        }

        // 알람상태가 prevAlarm에서 alarm으로 바뀌었다.
        public override void ChangeAlarm(AlarmData alarm, AlarmData prevAlarm)
        {
            // 알람 변경전 할일
            List<ClientMessage> messages = m_processAgent.PrevChangeAlarm(alarm, prevAlarm, m_mainManager.AlarmManager);
            ProcessClientMessages(messages);

            ProcessAlarm(alarm);

            // 알람 변경후 할일
            messages = m_processAgent.PostChangeAlarm(alarm, prevAlarm, m_mainManager.AlarmManager);
            ProcessClientMessages(messages);
        }

        // 알람에 관련된 센서 정보가 변경되었다.
        public override void UpdateAlarm(AlarmData alarm, List<int> alarmSensorZoneIDs)
        {
            Dictionary<CurrentAlarm.Fields, object> dicSets = new Dictionary<CurrentAlarm.Fields, object>();
            dicSets[CurrentAlarm.Fields.AlarmSensorZoneIDs] = ListToString(alarmSensorZoneIDs);

            Dictionary<CurrentAlarm.Fields, object> dicConditions = new Dictionary<CurrentAlarm.Fields, object>();
            dicConditions[CurrentAlarm.Fields.SensorZoneHistoryID] = alarm.SensorZoneHistoryID;

            string strErrorMessage;
            m_mainManager.SDMSDataManager.GetUpdateManager().UpdateCurrentAlarm(dicSets, dicConditions, null, out strErrorMessage);
        }

        private string ListToString<DataType>(List<DataType> datas)
        {
            string str = "";

            foreach (DataType data in datas)
            {
                if (str.Length == 0)
                    str += data.ToString();
                else
                    str += ", " + data.ToString();
            }

            return str;
        }

        public override void ReportAlarm(AlarmData alarm, List<int> alarmSensorZoneIDs)
        {
            // 재난 신고전 할일
            List<ClientMessage> messages = m_processAgent.PrevReportAlarm(alarm, m_mainManager.AlarmManager);
            ProcessClientMessages(messages);

            SMS.SMSMessageTypes messageType = ProcessAlarm(alarm);

            if (alarm.SensorZoneID >= Header.ManualReportDefaultID)
            {
                // 수동 신고
                int nZoneID = -1;

                if (int.TryParse(alarm.ReactionHistoryParam1, out nZoneID))
                {
                    // 수동신고는 센서탐지 과정이 없다.
                    int nSopStatus = -1; //(-1: SOP 시작 하기전, 0: SOP 실행 요청, 1: SOP 실행중)

                    if (alarm.AlarmDepth >= 3)
                        nSopStatus = 0;

                    m_mainManager.SDMSDataManager.GetCreateManager().CreateCurrentAlarm(alarm.SensorZoneHistoryID, (int)alarm.SensorType, (int)CurrentAlarm.AlarmTypes.Report, alarm.TimeStamp, nSopStatus, alarm.AlarmDepth, alarmSensorZoneIDs);
                }
            }
            else
            {
                Dictionary<CurrentAlarm.Fields, object> dicConditions = new Dictionary<CurrentAlarm.Fields, object>();
                dicConditions[CurrentAlarm.Fields.SensorZoneHistoryID] = alarm.SensorZoneHistoryID;

                string strErrorMessage;
                List<CurrentAlarm> alarms = m_mainManager.SDMSDataManager.GetSelectManager().SelectCurrentAlarms(dicConditions, "", out strErrorMessage);

                if (alarms == null)
                {
                    // DB 오류
                    System.Diagnostics.Trace.WriteLine(strErrorMessage);
                    return;
                }
                else if (alarms.Count == 0)
                {
                    int nSopStatus = -1; //(-1: SOP 시작 하기전, 0: SOP 실행 요청, 1: SOP 실행중)

                    if (alarm.AlarmDepth >= 3)
                        nSopStatus = 0;

                    m_mainManager.SDMSDataManager.GetCreateManager().CreateCurrentAlarm(alarm.SensorZoneHistoryID, (int)alarm.SensorType, (int)CurrentAlarm.AlarmTypes.Report, alarm.TimeStamp, nSopStatus, alarm.AlarmDepth, alarmSensorZoneIDs);
                }
                else
                {
                    Dictionary<CurrentAlarm.Fields, object> dicSets = new Dictionary<CurrentAlarm.Fields, object>();
                    dicSets[CurrentAlarm.Fields.AlarmType] = (int)CurrentAlarm.AlarmTypes.Report;
                    if (alarm.AlarmDepth >= 3)
                    {
                        dicSets[CurrentAlarm.Fields.SopStatus] = 0;
                    }
                    m_mainManager.SDMSDataManager.GetUpdateManager().UpdateCurrentAlarm(dicSets, dicConditions, "", out strErrorMessage);
                }
            }

            // 재난 신고후 할일
            messages = m_processAgent.PostReportAlarm(alarm, m_mainManager.AlarmManager);
            ProcessClientMessages(messages);
        }

        public override void ClearAlarm(AlarmData alarm)
        {
            // 알람 복구전 할일
            List<ClientMessage> messages = m_processAgent.PrevClearAlarm(alarm, m_mainManager.AlarmManager);
            ProcessClientMessages(messages);

            ProcessAlarm(alarm, false);

            Dictionary<CurrentAlarm.Fields, object> dicConditions = new Dictionary<CurrentAlarm.Fields, object>();
            dicConditions[CurrentAlarm.Fields.SensorZoneHistoryID] = alarm.SensorZoneHistoryID;

            string strErrorMessage;
            m_mainManager.SDMSDataManager.GetDeleteManager().DeleteCurrentAlarm(dicConditions, "", out strErrorMessage);

            // 알람 복구후 할일
            messages = m_processAgent.PostClearAlarm(alarm, m_mainManager.AlarmManager);
            ProcessClientMessages(messages);

            // 이미 삭제되었어야 하지만 남아있는 알람이 있는지 확인한다.
            CheckDanglingAlarms();
        }

        private void CheckDanglingAlarms()
        {
            ICollection<AlarmData> alarms = m_mainManager.AlarmManager.CurrentAlarms;
            string strSensorZoneHistoryIDs = "";

            foreach (AlarmData alarm in alarms)
            {
                if (strSensorZoneHistoryIDs.Length == 0)
                    strSensorZoneHistoryIDs = alarm.SensorZoneHistoryID.ToString();
                else
                    strSensorZoneHistoryIDs += "," + alarm.SensorZoneHistoryID.ToString();
            }

            if (strSensorZoneHistoryIDs.Length == 0)
                return;

            bool isNullable;
            string strCondition = string.Format("{0} in ({1}) and {2} = {3}",
                SensorReactionHistory.GetFieldName(SensorReactionHistory.Fields.SensorZoneHistoryID, out isNullable),
                strSensorZoneHistoryIDs,
                SensorReactionHistory.GetFieldName(SensorReactionHistory.Fields.ReactionType, out isNullable),
                (int)SensorReactionHistory.ReactionTypes.END_STATUS);

            string strErrorMessage;
            List<SensorReactionHistory> sensorReactionHistories = m_mainManager.SDMSDataManager.GetSelectManager().SelectSensorReactionHistories(null, strCondition, out strErrorMessage);

            if (sensorReactionHistories == null)
            {
                System.Diagnostics.Trace.WriteLine(strErrorMessage);
                return;
            }

            foreach (SensorReactionHistory srh in sensorReactionHistories)
            {
                m_mainManager.AlarmManager.RemoveCurrentAlarm(srh.SensorZoneHistoryID);
            }
        }

        private void ProcessClientMessages(List<ClientMessage> messages)
        {
            if (messages == null)
                return;

            /*foreach (ClientMessage message in messages)
            {
                if (message.ClientType == SOPWebServer.ClientType.FIRE_SENSOR_SERVER)
                    Client.FireSensorServer.Instance.SendClientData(message.Header, message.Bytes, message.ClientType, message.ClientSubType);
                else if (message.ClientType == SOPWebServer.ClientType.PSM_SENSOR_SERVER)
                    Client.PSMSensorServer.Instance.SendClientData(message.Header, message.Bytes, message.ClientType, message.ClientSubType);
                else if (message.ClientType == SOPWebServer.ClientType.SECURITY_SENSOR_SERVER)
                    Client.SecuritySensorServer.Instance.SendClientData(message.Header, message.Bytes, message.ClientType, message.ClientSubType);
                else if (message.ClientType == SOPWebServer.ClientType.EARTHQUAKE_SENSOR_SERVER)
                    Client.EarthquakeSensorServer.Instance.SendClientData(message.Header, message.Bytes, message.ClientType, message.ClientSubType);
                else if (message.ClientType == SOPWebServer.ClientType.LOGIN_SERVER)
                    Client.LoginServer.Instance.SendClientData(message.Header, message.Bytes, message.ClientType, message.ClientSubType);
                else if (message.ClientType == SOPWebServer.ClientType.TEMPERATURE_HUMIDITY_SERVER)
                    Client.TempHumidityServer.Instance.SendClientData(message.Header, message.Bytes, message.ClientType, message.ClientSubType);
                else if (message.ClientType == SOPWebServer.ClientType.SDMS)
                    Client.SDMSServer.Instance.SendClientData(message.Header, message.Bytes, message.ClientType, message.ClientSubType);
                else if (message.ClientType == SOPWebServer.ClientType.SOP_SIMULATOR)
                {
                    SOPSimulatorManager.ServerInstance.SendClientData(message.Header, message.Bytes, message.ClientType, message.ClientSubType);
                    //Client.SOPSimulatorServer.Instance.SendClientData(message.Header, message.Bytes, message.ClientType, message.ClientSubType);
                }
                else if (message.ClientType == SOPWebServer.ClientType.SOP_MANAGER)
                    Client.SOPManagerServer.Instance.SendClientData(message.Header, message.Bytes, message.ClientType, message.ClientSubType);
                else if (message.ClientType == SOPWebServer.ClientType.SOP_COMMANDER)
                    Client.ServerCommander.Instance.SendClientData(message.Header, message.Bytes, message.ClientType, message.ClientSubType);
                else if (message.ClientType == SOPWebServer.ClientType.ETC)
                    Client.EtcSensorServer.Instance.SendClientData(message.Header, message.Bytes, message.ClientType, message.ClientSubType);
            }*/
        }

        private void ReadSample(ref Dictionary<string, string> phoneNumbers, ref Dictionary<string, string> emails, Facility.FacilityType sensorType)
        {
            

            string type = "";
            if (sensorType == Facility.FacilityType.FIRE_SENSOR)
                type = "#FIRE";            
            else if (sensorType == Facility.FacilityType.Intrusion_S1 || sensorType == Facility.FacilityType.Loiter_S1 || sensorType == Facility.FacilityType.Collapse_S1 || sensorType == Facility.FacilityType.Theft_S1 || sensorType == Facility.FacilityType.Neglect_S1 || sensorType == Facility.FacilityType.VirtualFence_S1 || sensorType == Facility.FacilityType.Fire_S1)
                type = "#SVMS";
            else if (Facility.IsETCSensorType(sensorType) || Facility.IsPSMSensorType(sensorType))
                type = "#ETC";


            if (type.Length == 0)
                return;

            string path = AppDomain.CurrentDomain.BaseDirectory + "\\receiver.txt";

            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string categoryName = "";
                    string tag = "";
                    while (true)
                    {
                        string str = sr.ReadLine();
                        str = str.Trim();

                        if (str == "End;")
                            break;

                        if (str.Length == 0)
                            continue;

                        if (categoryName.Length > 0 && str.Contains("#"))
                            break;

                        if (str.Contains("#") && str == type)
                        {
                            categoryName = str;
                            continue;
                        }

                        if (categoryName.Length == 0)
                            continue;

                        if (str == "PhoneNumber")
                        {
                            tag = str;
                            continue;
                        }
                        else if (str == "Email")
                        {
                            tag = str;
                            continue;
                        }

                        if (str.Length > 0)
                        {
                            if (tag == "PhoneNumber")
                            {
                                if (!phoneNumbers.ContainsKey(str))
                                    phoneNumbers.Add(str, str);
                            }
                            else if (tag == "Email")
                            {
                                if (!emails.ContainsKey(str))
                                    emails.Add(str, str);
                            }
                        }
                    }
                }
            }
        }

        private SMS.SMSMessageTypes ProcessAlarm(AlarmData alarm, bool phoneNumberClear = true)
        {
            string strErrorMessage;
            if (alarm.Status == SensorReactionHistory.ReactionTypes.BEGIN_STATUS || alarm.Status == SensorReactionHistory.ReactionTypes.CHANGE_ALARM_DEPTH)
            {
                Dictionary<CurrentAlarm.Fields, object> dicSets = new Dictionary<CurrentAlarm.Fields, object>();
                dicSets[CurrentAlarm.Fields.AlarmDepth] = alarm.AlarmDepth;
                
                if (alarm.AlarmDepth >= 3 /*|| alarm.AlarmDepth != prevAlarm.AlarmDepth*/)
                    dicSets[CurrentAlarm.Fields.SopStatus] = 0;

                Dictionary<CurrentAlarm.Fields, object> dicConditions = new Dictionary<CurrentAlarm.Fields, object>();
                dicConditions[CurrentAlarm.Fields.SensorZoneHistoryID] = alarm.SensorZoneHistoryID;
                                
                if (m_mainManager.SDMSDataManager.GetUpdateManager().UpdateCurrentAlarm(dicSets, dicConditions, null, out strErrorMessage) == false)
                {
                    System.Diagnostics.Trace.WriteLine("ProcessAlarm Error : " + strErrorMessage);
                    return SMS.SMSMessageTypes.UNKNOWN;
                } 
            }

            // 테스트 "esh@soulbrain.co.kr"
            //    Dictionary<string, string> dicPhoneNumbers = new Dictionary<string, string>();
            //    Dictionary<string, string> dicEmails = new Dictionary<string, string>();
            //    ReadSample(ref dicPhoneNumbers, ref dicEmails, alarm.SensorType);

            SMS.SMSMessageTypes messageType;
            messageType = Process.SMSManager.ReactionTypeToMessageType(alarm.Status, alarm.SensorType);

            Dictionary<string, string> dicPhoneNumbers = new Dictionary<string, string>();
            Dictionary<string, string> dicEmails = new Dictionary<string, string>();
            List<TeamEditor.Model.Sop.Team.RegularMember> regularMembers = m_mainManager.TeamDataManager.GetSelectManager().SelectRegularMembers(out strErrorMessage);

            if (regularMembers == null)
            {
                System.Diagnostics.Trace.WriteLine("ProcessAlarm Error : " + strErrorMessage);
                return SMS.SMSMessageTypes.UNKNOWN;
            }
            
            int nSensorZoneID = alarm.SensorZoneID;
            int nSensorType = (int)alarm.SensorType;


            // 재난, 건물, 건물그룹 별 담당자 리스트 만들기 및 문자, 메일 보내기
            if (messageType != SMS.SMSMessageTypes.UNKNOWN)
            {
                string strAdditionalConditions;

                SensorZoneHistory sensorZoneHistory = null;
                SDMS.Model.Sensor.SensorZone sensorZone = null;
                SDMS.Model.Spatial.EquipmentZone equipmentZone = null;
                SDMS.Model.Spatial.Zone zone = null;
                SDMS.Model.Spatial.Building building = null;
                SDMS.Model.Spatial.BuildingGroup group = null;

                if (nSensorZoneID >= 1000000)
                {
                    strAdditionalConditions = string.Format("{0}.{1} = {2}", SensorZoneHistory.TableName, SensorZoneHistory.Fields.ID, alarm.SensorZoneHistoryID);

                    ArrayList arrResult = m_mainManager.SDMSDataManager.GetSelectManager().JoinSensorZoneHistoryZoneBuildingBuildingGroup(strAdditionalConditions, out strErrorMessage);

                    if (arrResult == null)
                    {
                        System.Diagnostics.Trace.WriteLine("ProcessAlarm Error : " + strErrorMessage);
                        return SMS.SMSMessageTypes.UNKNOWN;
                    }

                    int nResultCount = arrResult.Count;

                    if (arrResult.Count == 0 || arrResult.Count != 4)
                    {
                        System.Diagnostics.Trace.WriteLine("ProcessAlarm Error : JoinSensorZoneHistoryZoneBuildingBuildingGroup 조회를 하지 못하였습니다.");
                        return SMS.SMSMessageTypes.UNKNOWN;
                    }

                    sensorZoneHistory = arrResult[0] as SensorZoneHistory;
                    zone = arrResult[1] as SDMS.Model.Spatial.Zone;
                    building = arrResult[2] as SDMS.Model.Spatial.Building;
                    group = arrResult[3] as SDMS.Model.Spatial.BuildingGroup;
                }
                else
                {
                    int nZoneID = -1;

                    SDMS.Model.Sensor.SensorZone sz = m_mainManager.SDMSDataManager.GetSelectManager().SelectSensorZone(nSensorZoneID, out strErrorMessage);
                    if (sz != null)
                    {
                        if (Facility.IsFireSensorType((Facility.FacilityType)nSensorType))
                        {
                            SDMS.Model.Sensor.Fire fire = m_mainManager.SDMSDataManager.GetSelectManager().SelectFireSensor((int)sz.OrgSensorID, out strErrorMessage);
                            if (fire != null)
                                nZoneID = fire.ZoneID;
                        }
                        else if (Facility.IsPSMSensorType((Facility.FacilityType)nSensorType))
                        {
                            SDMS.Model.Sensor.PSM psm = m_mainManager.SDMSDataManager.GetSelectManager().SelectPSMSensor((int)sz.OrgSensorID, out strErrorMessage);
                            if (psm != null)
                                nZoneID = psm.ZoneID;
                        }
                        else if (Facility.IsETCSensorType((Facility.FacilityType)nSensorType))
                        {
                            SDMS.Model.Sensor.ETC etc = m_mainManager.SDMSDataManager.GetSelectManager().SelectETCSensor((int)sz.OrgSensorID, out strErrorMessage);
                            if (etc != null)
                                nZoneID = etc.ZoneID;
                        }
                        else if (Facility.IsSVMSSensorType((Facility.FacilityType)nSensorType))
                        {
                            SDMS.Model.CCTV.CCTV cctv = m_mainManager.SDMSDataManager.GetSelectManager().SelectCCTV((int)sz.OrgSensorID, out strErrorMessage);
                            if (cctv != null)
                                nZoneID = (cctv.ZoneID == null) ? -1 : (int)cctv.ZoneID;
                        }
                    }

                    if (nZoneID <= 0)
                    {
                        System.Diagnostics.Trace.WriteLine("ProcessAlarm Error : 센서 Zone을 확인할 수 없습니다.");
                        return SMS.SMSMessageTypes.UNKNOWN;
                    }

                    strAdditionalConditions = string.Format("{0}.{1} = {2}", SDMS.Model.Sensor.SensorZone.TableName, SDMS.Model.Sensor.SensorZone.Fields.ID, nSensorZoneID);
                    strAdditionalConditions += string.Format(" And {0}.{1} = {2}", SDMS.Model.Spatial.Zone.TableName, SDMS.Model.Spatial.Zone.Fields.ID, nZoneID);

                    ArrayList arrResult = m_mainManager.SDMSDataManager.GetSelectManager().JoinSensorZoneEquipmentZoneZoneBuildingBuildingGroup(strAdditionalConditions, out strErrorMessage);

                    if (arrResult == null)
                    {
                        System.Diagnostics.Trace.WriteLine("ProcessAlarm Error : " + strErrorMessage);
                        return SMS.SMSMessageTypes.UNKNOWN;
                    }

                    int nResultCount = arrResult.Count;

                    if (arrResult.Count == 0 || arrResult.Count != 5)
                    {
                        System.Diagnostics.Trace.WriteLine("ProcessAlarm Error : JoinSensorZoneEquipmentZoneZoneBuildingBuildingGroup 조회를 하지 못하였습니다.");
                        return SMS.SMSMessageTypes.UNKNOWN;
                    }

                    sensorZone = arrResult[0] as SDMS.Model.Sensor.SensorZone;
                    equipmentZone = arrResult[1] as SDMS.Model.Spatial.EquipmentZone;
                    zone = arrResult[2] as SDMS.Model.Spatial.Zone;
                    building = arrResult[3] as SDMS.Model.Spatial.Building;
                    group = arrResult[4] as SDMS.Model.Spatial.BuildingGroup;
                }

                DateTime dtTime = alarm.TimeStamp;

                string strYear = dtTime.ToString("yyyy") + "년";
                string strMonth = dtTime.ToString("MM") + "월";
                string strDay = dtTime.ToString("dd") + "일";

                string strHour = dtTime.ToString("HH") + "시";
                string strMinute = dtTime.ToString("mm") + "분";
                string strSecond = dtTime.ToString("ss") + "초";

                string strDate = strYear + " " + strMonth + " " + strDay + " " + strHour + " " + strMinute + " " + strSecond;
                string strLocation = zone.DisplayText;

                if (equipmentZone != null)
                    strLocation = equipmentZone.DisplayText;

                int nFacilityType = (int)Facility.FacilityType.NONE;

                if (Facility.IsFireSensorType((Facility.FacilityType)nSensorType))
                    nFacilityType = (int)Facility.FacilityType.FIRE_SENSOR;
                else if (Facility.IsPSMSensorType((Facility.FacilityType)nSensorType))
                    nFacilityType = (int)Facility.FacilityType.PSM_SENSOR;
                else if (Facility.IsETCSensorType((Facility.FacilityType)nSensorType))
                    nFacilityType = (int)Facility.FacilityType.ETC;
                else if (Facility.IsSVMSSensorType((Facility.FacilityType)nSensorType))
                    nFacilityType = (int)Facility.FacilityType.Intrusion_S1;


                string strCaller = NeedSMSCaller();

                if (strCaller != null && regularMembers.Count > 0 && nFacilityType != (int)Facility.FacilityType.NONE)
                {
                    // 타입 담당자 리스트 만들기
                    Dictionary<SDMS.Model.Config.SpreadMessage.Fields, object> dicConditionsSpreadMessage = new Dictionary<SpreadMessage.Fields, object>();
                    dicConditionsSpreadMessage[SDMS.Model.Config.SpreadMessage.Fields.FacilityType] = nFacilityType;
                    dicConditionsSpreadMessage[SDMS.Model.Config.SpreadMessage.Fields.BuilidingGroupID] = null;
                    dicConditionsSpreadMessage[SDMS.Model.Config.SpreadMessage.Fields.BuilidingID] = null;
                    dicConditionsSpreadMessage[SDMS.Model.Config.SpreadMessage.Fields.MessageType] = (int)SDMS.Model.Config.SpreadMessage.MessageTypes.SMS;

                    List<SDMS.Model.Config.SpreadMessage> spreadMessages = m_mainManager.SDMSDataManager.GetSelectManager().SelectSpreadMessages(dicConditionsSpreadMessage, null, out strErrorMessage);

                    if (spreadMessages == null)
                    {
                        System.Diagnostics.Trace.WriteLine("ProcessAlarm Error : " + strErrorMessage);
                        return SMS.SMSMessageTypes.UNKNOWN;
                    }

                    if (spreadMessages.Count > 0)
                    {
                        SpreadMessage message = spreadMessages[0];

                        if (alarm.Status != SensorReactionHistory.ReactionTypes.END_STATUS)
                        {
                            string strMessage = message.Message;
                            strMessage = strMessage.Replace("{location}", strLocation);
                            strMessage = strMessage.Replace("{date}", strDate);

                            alarm.Message = strMessage;
                        }
                            

                        string strRegularID = message.RegularID;
                        string strRegularMemberID = message.RegularMemberID;

                        ReadPhoneNumbers(strRegularID, strRegularMemberID, regularMembers, out dicPhoneNumbers);

                        int nResult = -1;
                        if (dicPhoneNumbers.Count > 0)
                            nResult = m_factory.SMSManager.SendSMS(alarm, strCaller, dicPhoneNumbers.Values, alarm.RegularMemberIDs.Values, alarm.Message, alarm.SensorReactionHistoryID);

                        if (nResult == ErrorMessageType.SUCCESS)
                        {
                            SensorZoneHistory.DetectionType detectStatus = alarm.IsReal ? SensorZoneHistory.DetectionType.Real : SensorZoneHistory.DetectionType.Test;
                            ((AlarmManager)m_mainManager.AlarmManager).AddReactionHistory(alarm, (int)SensorReactionHistory.ReactionTypes.SEND_SMS, DateTime.Now, alarm.Message, alarm.ReactionHistoryParam1, alarm.ReactionHistoryParam2, alarm.ReactionHistoryParam3, alarm.ReactionHistoryParam4, alarm.ReactionHistoryParam5, m_mainManager.SDMSDataManager);
                        }
                    }

                    // 빌딩그룹 담당자 리스트 만들기
                    dicConditionsSpreadMessage = new Dictionary<SpreadMessage.Fields, object>();
                    dicConditionsSpreadMessage[SDMS.Model.Config.SpreadMessage.Fields.FacilityType] = nFacilityType;
                    dicConditionsSpreadMessage[SDMS.Model.Config.SpreadMessage.Fields.BuilidingGroupID] = group.ID;
                    dicConditionsSpreadMessage[SDMS.Model.Config.SpreadMessage.Fields.BuilidingID] = null;
                    dicConditionsSpreadMessage[SDMS.Model.Config.SpreadMessage.Fields.MessageType] = (int)SDMS.Model.Config.SpreadMessage.MessageTypes.SMS;

                    spreadMessages = m_mainManager.SDMSDataManager.GetSelectManager().SelectSpreadMessages(dicConditionsSpreadMessage, null, out strErrorMessage);

                    if (spreadMessages == null)
                    {
                        System.Diagnostics.Trace.WriteLine("ProcessAlarm Error : " + strErrorMessage);
                        return SMS.SMSMessageTypes.UNKNOWN;
                    }

                    if (spreadMessages.Count > 0)
                    {
                        SpreadMessage message = spreadMessages[0];

                        if (alarm.Status != SensorReactionHistory.ReactionTypes.END_STATUS)
                        {
                            string strMessage = message.Message;
                            strMessage = strMessage.Replace("{location}", strLocation);
                            strMessage = strMessage.Replace("{date}", strDate);

                            alarm.Message = strMessage;
                        }

                        string strRegularID = message.RegularID;
                        string strRegularMemberID = message.RegularMemberID;

                        ReadPhoneNumbers(strRegularID, strRegularMemberID, regularMembers, out dicPhoneNumbers);

                        int nResult = -1;
                        if (dicPhoneNumbers.Count > 0)
                            nResult = m_factory.SMSManager.SendSMS(alarm, strCaller, dicPhoneNumbers.Values, alarm.RegularMemberIDs.Values, alarm.Message, alarm.SensorReactionHistoryID);

                        if (nResult == ErrorMessageType.SUCCESS)
                        {
                            SensorZoneHistory.DetectionType detectStatus = alarm.IsReal ? SensorZoneHistory.DetectionType.Real : SensorZoneHistory.DetectionType.Test;
                            ((AlarmManager)m_mainManager.AlarmManager).AddReactionHistory(alarm, (int)SensorReactionHistory.ReactionTypes.SEND_SMS, DateTime.Now, alarm.Message, alarm.ReactionHistoryParam1, alarm.ReactionHistoryParam2, alarm.ReactionHistoryParam3, alarm.ReactionHistoryParam4, alarm.ReactionHistoryParam5, m_mainManager.SDMSDataManager);
                        }
                    }

                    // 빌딩 담당자 리스트 만들기
                    dicConditionsSpreadMessage = new Dictionary<SpreadMessage.Fields, object>();
                    dicConditionsSpreadMessage[SDMS.Model.Config.SpreadMessage.Fields.FacilityType] = nFacilityType;
                    dicConditionsSpreadMessage[SDMS.Model.Config.SpreadMessage.Fields.BuilidingGroupID] = group.ID;
                    dicConditionsSpreadMessage[SDMS.Model.Config.SpreadMessage.Fields.BuilidingID] = building.ID;
                    dicConditionsSpreadMessage[SDMS.Model.Config.SpreadMessage.Fields.MessageType] = (int)SDMS.Model.Config.SpreadMessage.MessageTypes.SMS;

                    spreadMessages = m_mainManager.SDMSDataManager.GetSelectManager().SelectSpreadMessages(dicConditionsSpreadMessage, null, out strErrorMessage);

                    if (spreadMessages == null)
                    {
                        System.Diagnostics.Trace.WriteLine("ProcessAlarm Error : " + strErrorMessage);
                        return SMS.SMSMessageTypes.UNKNOWN;
                    }

                    if (spreadMessages.Count > 0)
                    {
                        SpreadMessage message = spreadMessages[0];

                        if (alarm.Status != SensorReactionHistory.ReactionTypes.END_STATUS)
                        {
                            string strMessage = message.Message;
                            strMessage = strMessage.Replace("{location}", strLocation);
                            strMessage = strMessage.Replace("{date}", strDate);

                            alarm.Message = strMessage;
                        }

                        string strRegularID = message.RegularID;
                        string strRegularMemberID = message.RegularMemberID;

                        ReadPhoneNumbers(strRegularID, strRegularMemberID, regularMembers, out dicPhoneNumbers);

                        int nResult = -1;
                        if (dicPhoneNumbers.Count > 0)
                            nResult = m_factory.SMSManager.SendSMS(alarm, strCaller, dicPhoneNumbers.Values, alarm.RegularMemberIDs.Values, alarm.Message, alarm.SensorReactionHistoryID);

                        if (nResult == ErrorMessageType.SUCCESS)
                        {
                            SensorZoneHistory.DetectionType detectStatus = alarm.IsReal ? SensorZoneHistory.DetectionType.Real : SensorZoneHistory.DetectionType.Test;
                            ((AlarmManager)m_mainManager.AlarmManager).AddReactionHistory(alarm, (int)SensorReactionHistory.ReactionTypes.SEND_SMS, DateTime.Now, alarm.Message, alarm.ReactionHistoryParam1, alarm.ReactionHistoryParam2, alarm.ReactionHistoryParam3, alarm.ReactionHistoryParam4, alarm.ReactionHistoryParam5, m_mainManager.SDMSDataManager);
                        }
                    }
                }

                string strEmailCaller = NeedEmailCaller();

                if (strEmailCaller != null && regularMembers.Count > 0 && nFacilityType != (int)Facility.FacilityType.NONE)
                {
                    // 타입 담당자 리스트 만들기
                    Dictionary<SDMS.Model.Config.SpreadMessage.Fields, object> dicConditionsSpreadMessage = new Dictionary<SpreadMessage.Fields, object>();
                    dicConditionsSpreadMessage[SDMS.Model.Config.SpreadMessage.Fields.FacilityType] = nFacilityType;
                    dicConditionsSpreadMessage[SDMS.Model.Config.SpreadMessage.Fields.BuilidingGroupID] = null;
                    dicConditionsSpreadMessage[SDMS.Model.Config.SpreadMessage.Fields.BuilidingID] = null;
                    dicConditionsSpreadMessage[SDMS.Model.Config.SpreadMessage.Fields.MessageType] = (int)SDMS.Model.Config.SpreadMessage.MessageTypes.EMAIL;

                    List<SDMS.Model.Config.SpreadMessage> spreadMessages = m_mainManager.SDMSDataManager.GetSelectManager().SelectSpreadMessages(dicConditionsSpreadMessage, null, out strErrorMessage);

                    if (spreadMessages == null)
                    {
                        System.Diagnostics.Trace.WriteLine("ProcessAlarm Error : " + strErrorMessage);
                        return SMS.SMSMessageTypes.UNKNOWN;
                    }

                    if (spreadMessages.Count > 0)
                    {
                        SpreadMessage message = spreadMessages[0];

                        if (alarm.Status != SensorReactionHistory.ReactionTypes.END_STATUS)
                        {
                            string strMessage = message.Message;
                            strMessage = strMessage.Replace("{location}", strLocation);
                            strMessage = strMessage.Replace("{date}", strDate);

                            alarm.Message = strMessage;
                        }

                        string strRegularID = message.RegularID;
                        string strRegularMemberID = message.RegularMemberID;

                        ReadEmails(strRegularID, strRegularMemberID, regularMembers, out dicEmails);

                        int nResult = -1;
                        if (dicEmails.Count > 0)
                            nResult = m_factory.EmailManager.SendEmail(alarm, strEmailCaller, dicEmails.Values, alarm.RegularMemberIDs.Values, alarm.Message, alarm.SensorReactionHistoryID);
                        // TODO: 작업 완료 후 메일도 Reaction history 추가
                        if (nResult == ErrorMessageType.SUCCESS)
                        {
                            SensorZoneHistory.DetectionType detectStatus = alarm.IsReal ? SensorZoneHistory.DetectionType.Real : SensorZoneHistory.DetectionType.Test;
                            ((AlarmManager)m_mainManager.AlarmManager).AddReactionHistory(alarm, (int)SensorReactionHistory.ReactionTypes.SEND_SMS, DateTime.Now, alarm.Message, alarm.ReactionHistoryParam1, alarm.ReactionHistoryParam2, alarm.ReactionHistoryParam3, alarm.ReactionHistoryParam4, alarm.ReactionHistoryParam5, m_mainManager.SDMSDataManager);
                        }
                    }

                    // 빌딩그룹 담당자 리스트 만들기
                    dicConditionsSpreadMessage = new Dictionary<SpreadMessage.Fields, object>();
                    dicConditionsSpreadMessage[SDMS.Model.Config.SpreadMessage.Fields.FacilityType] = nFacilityType;
                    dicConditionsSpreadMessage[SDMS.Model.Config.SpreadMessage.Fields.BuilidingGroupID] = group.ID;
                    dicConditionsSpreadMessage[SDMS.Model.Config.SpreadMessage.Fields.BuilidingID] = null;
                    dicConditionsSpreadMessage[SDMS.Model.Config.SpreadMessage.Fields.MessageType] = (int)SDMS.Model.Config.SpreadMessage.MessageTypes.EMAIL;

                    spreadMessages = m_mainManager.SDMSDataManager.GetSelectManager().SelectSpreadMessages(dicConditionsSpreadMessage, null, out strErrorMessage);

                    if (spreadMessages == null)
                    {
                        System.Diagnostics.Trace.WriteLine("ProcessAlarm Error : " + strErrorMessage);
                        return SMS.SMSMessageTypes.UNKNOWN;
                    }

                    if (spreadMessages.Count > 0)
                    {
                        SpreadMessage message = spreadMessages[0];

                        if (alarm.Status != SensorReactionHistory.ReactionTypes.END_STATUS)
                        {
                            string strMessage = message.Message;
                            strMessage = strMessage.Replace("{location}", strLocation);
                            strMessage = strMessage.Replace("{date}", strDate);

                            alarm.Message = strMessage;
                        }

                        string strRegularID = message.RegularID;
                        string strRegularMemberID = message.RegularMemberID;

                        ReadEmails(strRegularID, strRegularMemberID, regularMembers, out dicEmails);

                        int nResult = -1;
                        if (dicEmails.Count > 0)
                            nResult = m_factory.EmailManager.SendEmail(alarm, strEmailCaller, dicEmails.Values, alarm.RegularMemberIDs.Values, alarm.Message, alarm.SensorReactionHistoryID);
                        // TODO: 작업 완료 후 메일도 Reaction history 추가
                        if (nResult == ErrorMessageType.SUCCESS)
                        {
                            SensorZoneHistory.DetectionType detectStatus = alarm.IsReal ? SensorZoneHistory.DetectionType.Real : SensorZoneHistory.DetectionType.Test;
                            ((AlarmManager)m_mainManager.AlarmManager).AddReactionHistory(alarm, (int)SensorReactionHistory.ReactionTypes.SEND_SMS, DateTime.Now, alarm.Message, alarm.ReactionHistoryParam1, alarm.ReactionHistoryParam2, alarm.ReactionHistoryParam3, alarm.ReactionHistoryParam4, alarm.ReactionHistoryParam5, m_mainManager.SDMSDataManager);
                        }
                    }


                    // 빌딩 담당자 리스트 만들기
                    dicConditionsSpreadMessage = new Dictionary<SpreadMessage.Fields, object>();
                    dicConditionsSpreadMessage[SDMS.Model.Config.SpreadMessage.Fields.FacilityType] = nFacilityType;
                    dicConditionsSpreadMessage[SDMS.Model.Config.SpreadMessage.Fields.BuilidingGroupID] = group.ID;
                    dicConditionsSpreadMessage[SDMS.Model.Config.SpreadMessage.Fields.BuilidingID] = building.ID;
                    dicConditionsSpreadMessage[SDMS.Model.Config.SpreadMessage.Fields.MessageType] = (int)SDMS.Model.Config.SpreadMessage.MessageTypes.EMAIL;

                    spreadMessages = m_mainManager.SDMSDataManager.GetSelectManager().SelectSpreadMessages(dicConditionsSpreadMessage, null, out strErrorMessage);

                    if (spreadMessages == null)
                    {
                        System.Diagnostics.Trace.WriteLine("ProcessAlarm Error : " + strErrorMessage);
                        return SMS.SMSMessageTypes.UNKNOWN;
                    }

                    if (spreadMessages.Count > 0)
                    {
                        SpreadMessage message = spreadMessages[0];

                        if (alarm.Status != SensorReactionHistory.ReactionTypes.END_STATUS)
                        {
                            string strMessage = message.Message;
                            strMessage = strMessage.Replace("{location}", strLocation);
                            strMessage = strMessage.Replace("{date}", strDate);

                            alarm.Message = strMessage;
                        }

                        string strRegularID = message.RegularID;
                        string strRegularMemberID = message.RegularMemberID;

                        ReadEmails(strRegularID, strRegularMemberID, regularMembers, out dicEmails);

                        int nResult = -1;
                        if (dicEmails.Count > 0)
                            nResult = m_factory.EmailManager.SendEmail(alarm, strEmailCaller, dicEmails.Values, alarm.RegularMemberIDs.Values, alarm.Message, alarm.SensorReactionHistoryID);
                        // TODO: 작업 완료 후 메일도 Reaction history 추가
                        if (nResult == ErrorMessageType.SUCCESS)
                        {
                            SensorZoneHistory.DetectionType detectStatus = alarm.IsReal ? SensorZoneHistory.DetectionType.Real : SensorZoneHistory.DetectionType.Test;
                            ((AlarmManager)m_mainManager.AlarmManager).AddReactionHistory(alarm, (int)SensorReactionHistory.ReactionTypes.SEND_SMS, DateTime.Now, alarm.Message, alarm.ReactionHistoryParam1, alarm.ReactionHistoryParam2, alarm.ReactionHistoryParam3, alarm.ReactionHistoryParam4, alarm.ReactionHistoryParam5, m_mainManager.SDMSDataManager);
                        }
                    }
                }
            }
            


            Broadcast.SituationTypes situationType;

            if (NeedBroadcast(alarm, out situationType))
            {
                int nRepeatCount;
                bool useSiren;
                string strMessage = m_factory.BroadcastManager.GetBroadcastMessage(alarm, situationType, out nRepeatCount, out useSiren);

                if (m_factory.BroadcastManager.RunBroadcast(strMessage, nRepeatCount, useSiren))
                {
                    SensorZoneHistory.DetectionType detectionStatus = alarm.IsReal ? SensorZoneHistory.DetectionType.Real : SensorZoneHistory.DetectionType.Test;
                    ((AlarmManager)m_mainManager.AlarmManager).AddReactionHistory(alarm, (int)SensorReactionHistory.ReactionTypes.RUN_BROADCAST, DateTime.Now, strMessage, alarm.ReactionHistoryParam1, alarm.ReactionHistoryParam2, alarm.ReactionHistoryParam3, null, alarm.ReactionHistoryParam5, m_mainManager.SDMSDataManager);
                }
            }

            return messageType;
        }

        private bool ReadPhoneNumbers(string strRegularID, string strRegularMemberID, List<TeamEditor.Model.Sop.Team.RegularMember> regularMembers, out Dictionary<string, string> dicPhoneNumbers)
        {
            dicPhoneNumbers = new Dictionary<string, string>();

            if (strRegularID != null)
            {
                string[] arrRegularID = strRegularID.Split(',');

                foreach (string strID in arrRegularID)
                {
                    int nRegularID;

                    if (Int32.TryParse(strID, out nRegularID))
                    {
                        foreach (TeamEditor.Model.Sop.Team.RegularMember member in regularMembers)
                        {
                            if (member.RegularID == nRegularID)
                            {
                                if (member.PhoneNumber != null && member.PhoneNumber != "")
                                {
                                    string strPhoneNumber = DecryptString(member.PhoneNumber);
                                    dicPhoneNumbers[strPhoneNumber] = strPhoneNumber;
                                }
                            }
                        }
                    }
                }
            }

            if (strRegularMemberID != null)
            {
                string[] arrRegularMemberID = strRegularMemberID.Split(',');

                foreach (string strID in arrRegularMemberID)
                {
                    int nRegularMemberID;

                    if (Int32.TryParse(strID, out nRegularMemberID))
                    {
                        foreach (TeamEditor.Model.Sop.Team.RegularMember member in regularMembers)
                        {
                            if (member.ID == nRegularMemberID)
                            {
                                if (member.PhoneNumber != null && member.PhoneNumber != "")
                                {
                                    string strPhoneNumber = DecryptString(member.PhoneNumber);
                                    dicPhoneNumbers[strPhoneNumber] = strPhoneNumber;
                                }

                                break;
                            }
                        }
                    }
                }
            }

            return true;
        }

        private bool ReadEmails(string strRegularID, string strRegularMemberID, List<TeamEditor.Model.Sop.Team.RegularMember> regularMembers, out Dictionary<string, string> dicEmails)
        {
            dicEmails = new Dictionary<string, string>();

            if (strRegularID != null)
            {
                string[] arrRegularID = strRegularID.Split(',');

                foreach (string strID in arrRegularID)
                {
                    int nRegularID;

                    if (Int32.TryParse(strID, out nRegularID))
                    {
                        foreach (TeamEditor.Model.Sop.Team.RegularMember member in regularMembers)
                        {
                            if (member.RegularID == nRegularID)
                            {
                                if (member.PhoneNumber != null && member.PhoneNumber != "")
                                {
                                    string strEmail = member.Email;
                                    dicEmails[strEmail] = strEmail;
                                }
                            }
                        }
                    }
                }
            }

            if (strRegularMemberID != null)
            {
                string[] arrRegularMemberID = strRegularMemberID.Split(',');

                foreach (string strID in arrRegularMemberID)
                {
                    int nRegularMemberID;

                    if (Int32.TryParse(strID, out nRegularMemberID))
                    {
                        foreach (TeamEditor.Model.Sop.Team.RegularMember member in regularMembers)
                        {
                            if (member.ID == nRegularMemberID)
                            {
                                if (member.PhoneNumber != null && member.PhoneNumber != "")
                                {
                                    string strEmail = member.Email;
                                    dicEmails[strEmail] = strEmail;
                                }

                                break;
                            }
                        }
                    }
                }
            }

            return true;
        }

        private static string DecryptString(string str)
        {
            return AES256Cipher.AES_decrypt(str, key);
        }

        // Return 값 : 문자발송이 필요한 상황이면 발신자 번호를 리턴한다.
        //             문자발송이 필요하지 않은 상황이면 null을 리턴한다.
        public override string NeedSMS(AlarmData alarm, out SMS.SMSMessageTypes messageType)
        {
            messageType = Process.SMSManager.ReactionTypeToMessageType(alarm.Status, alarm.SensorType);

            if (messageType == SMS.SMSMessageTypes.UNKNOWN)
                return null;

            Dictionary<SMS.Fields, object> dicConditions = new Dictionary<SMS.Fields, object>();
            dicConditions[SMS.Fields.MessageType] = (int)messageType;

            string strErrorMessage;
            List<SMS> configs = m_mainManager.SDMSDataManager.GetSelectManager().SelectSMSConfigs(dicConditions, "", out strErrorMessage);

            if (configs == null || configs.Count == 0)
                return null;

            if (configs[0].UseSMS == false)
                return null;

            List<Common.Model.Option.Options> options = m_mainManager.CommonDataManager.GetSelectManager().SelectOption(Common.Model.Option.Options.OptionTarget.SDMS, "SMSCaller", out strErrorMessage);

            if (options == null || options.Count == 0)
                return null;

            return options[0].PropertyValue;
        }

        public override string NeedSMSCaller()
        {
            // OptionSOPSimulator UseSMS 판단 유무 파악
            Dictionary<Common.Model.Option.Options.Fields, object> dicConditions = new Dictionary<Common.Model.Option.Options.Fields, object>();
            dicConditions[Common.Model.Option.Options.Fields.PropertyName] = "UseSMS";

            string strErrorMessage;
            List<Common.Model.Option.Options> configs = m_mainManager.CommonDataManager.GetSelectManager().SelectOption(Common.Model.Option.Options.OptionTarget.SOPSimulator, "UseSMS", out strErrorMessage);

            if (configs == null || configs.Count == 0)
                return null;

            if (configs[0].PropertyValue == "false")
                return null;

            List<Common.Model.Option.Options> options = m_mainManager.CommonDataManager.GetSelectManager().SelectOption(Common.Model.Option.Options.OptionTarget.SDMS, "SMSCaller", out strErrorMessage);

            if (options == null || options.Count == 0)
                return null;

            return options[0].PropertyValue;
        }

        public override string NeedEmailCaller()
        {
            // OptionSOPSimulator UseEmail 판단 유무 파악
            Dictionary<Common.Model.Option.Options.Fields, object> dicConditions = new Dictionary<Common.Model.Option.Options.Fields, object>();
            dicConditions[Common.Model.Option.Options.Fields.PropertyName] = "UseEmail";

            string strErrorMessage;
            List<Common.Model.Option.Options> configs = m_mainManager.CommonDataManager.GetSelectManager().SelectOption(Common.Model.Option.Options.OptionTarget.SOPSimulator, "UseEmail", out strErrorMessage);

            if (configs == null || configs.Count == 0)
                return null;

            if (configs[0].PropertyValue == "false")
                return null;

            List<Common.Model.Option.Options> options = m_mainManager.CommonDataManager.GetSelectManager().SelectOption(Common.Model.Option.Options.OptionTarget.SDMS, "EmailCaller", out strErrorMessage);

            if (options == null || options.Count == 0)
                return null;

            return options[0].PropertyValue;
        }

        public override bool NeedBroadcast(AlarmData alarm, out Broadcast.SituationTypes situationType)
        {
            situationType = BaseBroadcastManager.ReactionTypeToSituationType(alarm.Status, alarm.SensorType);

            if (situationType == Broadcast.SituationTypes.Unknown)
                return false;
            else if (situationType == Broadcast.SituationTypes.DETECT_EARTHQUAKE)
            {
                if (alarm.Tag != null && alarm.Tag is EarthquakeOption)
                {
                    EarthquakeOption option = (EarthquakeOption)alarm.Tag;
                    return option.UseBroadcast;
                }
                else
                    return false;
            }

            Dictionary<Broadcast.Fields, object> dicConditions = new Dictionary<Broadcast.Fields, object>();
            dicConditions[Broadcast.Fields.SituationType] = (int)situationType;
            dicConditions[Broadcast.Fields.SiteID] = m_mainManager.SDMSDataManager.SiteID;

            string strErrorMessage;
            List<Broadcast> configs = m_mainManager.SDMSDataManager.GetSelectManager().SelectBroadcastConfigs(dicConditions, "", out strErrorMessage);

            if (configs == null || configs.Count == 0)
                return false;

            return configs[0].UseBroadcast;
        }
    }
}
