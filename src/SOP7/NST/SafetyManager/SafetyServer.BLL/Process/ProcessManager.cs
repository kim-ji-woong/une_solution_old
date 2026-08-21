using System;
using System.Collections.Generic;
using AgentFactory.BLL;
using dnsData.Alarm;
using dnsData.Sensor;
using SDMS.Model.Config;
using SDMS.Model.Alarm;
using SDMS.Model.History;
using SDMS.Model.Sensor;
using SDMS.Model.Spatial;
using dnsSopID;
using System.IO;
using TeamEditor.Model.Sop.Team;

namespace SafetyServer.BLL.Process
{
    public class ProcessManager : BaseProcessManager
    {
        private MainManager m_mainManager = null;
        private NetvisionManager m_netvisionManager = null;

        public ProcessManager(Factory factory, MainManager mainManager)
            : base(factory)
        {
            m_mainManager = mainManager;
            m_netvisionManager = new NetvisionManager();
            factory.ProcessManager = this;
        }

        public override void NewAlarm(AlarmData alarm, List<int> alarmSensorZoneIDs)
        {
            // 알람 발생전 할일
            List<ClientMessage> messages = m_processAgent.PrevNewAlarm(alarm, m_mainManager.AlarmManager);
            ProcessClientMessages(messages);

            ProcessAlarm(alarm);

            int? buildingID;
            int? fieldID;

            if (ReadBuildingNField(alarm, out buildingID, out fieldID))
            {                
                
                m_netvisionManager.SendAlarmAsync(alarm.SensorType, true, buildingID, fieldID, alarm.TimeStamp, alarm.AlarmDepth, alarm.Message, "", alarm, m_mainManager);
            }

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

        private string GetMemberID(Facility.FacilityType sensorType, int nSensorZoneID)
        {
            if (Facility.IsETCSensorType(sensorType))
            {
                string strErrorMessage = null;
                SensorZone sz = m_mainManager.SDMSDataManager.GetSelectManager().SelectSensorZone(nSensorZoneID, out strErrorMessage);
                if (sz == null)
                    return "";

                ETC etcSensor = m_mainManager.SDMSDataManager.GetSelectManager().SelectETCSensor((int)sz.OrgSensorID, out strErrorMessage);
                if (etcSensor == null)
                    return "";

                int nRegularMemberID;
                if (!int.TryParse(etcSensor.Department, out nRegularMemberID))
                    return "";

                RegularMember member = m_mainManager.TeamDataManager.GetSelectManager().SelectRegularMember(nRegularMemberID, out strErrorMessage);
                if (member == null)
                    return "";

                return member.MemberID;
            }

            return "";
        }

        private bool ReadBuildingNField(AlarmData alarm, out int? buildingID, out int? fieldID)
        {
            buildingID = null;
            fieldID = null;

            string strErrorMessage;
            int nZoneID;

            if (alarm.IsManual)
            {
                if (int.TryParse(alarm.ReactionHistoryParam1, out nZoneID) == false)
                    return false;
            }
            else
            {
                SensorZone sensorZone = m_mainManager.SDMSDataManager.GetSelectManager().SelectSensorZone(alarm.SensorZoneID, out strErrorMessage);

                if (sensorZone == null)
                    return false;

                EquipmentZone equipZone = m_mainManager.SDMSDataManager.GetSelectManager().SelectEquipmentZone(sensorZone.EquipZoneID, out strErrorMessage);

                if (equipZone == null)
                    return false;

                if (equipZone.LinkedZoneIDs.Count == 0)
                    return false;

                nZoneID = equipZone.LinkedZoneIDs[0];
            }

            Zone zone = m_mainManager.SDMSDataManager.GetSelectManager().SelectZone(nZoneID, out strErrorMessage);

            if (zone == null)
                return false;

            if (zone.BuildingID != null && zone.FloorIndex == null)
                buildingID = zone.BuildingID;
            else if (zone.BuildingID != null && zone.FloorIndex != null)
                fieldID = nZoneID;
            else if (zone.BuildingID == null && zone.FloorIndex == null)
                fieldID = -1;

            return true;
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

            int? buildingID;
            int? fieldID;

            if (ReadBuildingNField(alarm, out buildingID, out fieldID))
            {
                string strMemberID = GetMemberID(alarm.SensorType, alarm.SensorZoneID);
                m_netvisionManager.SendAlarmAsync(alarm.SensorType, false, buildingID, fieldID, alarm.TimeStamp, alarm.AlarmDepth, "", strMemberID, alarm, m_mainManager);
            }

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
            Dictionary<CurrentAlarm.Fields, object> dicSets = new Dictionary<CurrentAlarm.Fields, object>();
            dicSets[CurrentAlarm.Fields.AlarmDepth] = alarm.AlarmDepth;
            if (alarm.AlarmDepth >= 3)
            {
                dicSets[CurrentAlarm.Fields.SopStatus] = 0;
            }

            Dictionary<CurrentAlarm.Fields, object> dicConditions = new Dictionary<CurrentAlarm.Fields, object>();
            dicConditions[CurrentAlarm.Fields.SensorZoneHistoryID] = alarm.SensorZoneHistoryID;

            string strErrorMessage;
            if (m_mainManager.SDMSDataManager.GetUpdateManager().UpdateCurrentAlarm(dicSets, dicConditions, null, out strErrorMessage) == false)
            {
                System.Diagnostics.Trace.WriteLine("ProcessAlarm Error : " + strErrorMessage);
                return SMS.SMSMessageTypes.UNKNOWN;
            }

            SMS.SMSMessageTypes messageType;
            string strCaller = NeedSMS(alarm, out messageType);

            if (strCaller != null)
            {
                if (phoneNumberClear)
                {
                    alarm.PhoneNumbers.Clear();
                    alarm.Emails.Clear();
                    alarm.RegularMemberIDs.Clear();
                    alarm.ExternalMemberIDs.Clear();
                }


                // 테스트 "esh@soulbrain.co.kr"
                Dictionary<string, string> dicPhoneNumbers = new Dictionary<string, string>();
                Dictionary<string, string> dicEmails = new Dictionary<string, string>();
                ReadSample(ref dicPhoneNumbers, ref dicEmails, alarm.SensorType);

                //int nReceiverCount = m_factory.SMSManager.GetPhoneNumbers(alarm, messageType);  // alarm.Emails 수신자 또한 여기서 추가한다
                int nReceiverCount = dicPhoneNumbers.Count + dicEmails.Count;

                if (nReceiverCount > 0)
                {
                    //int nResult = m_factory.SMSManager.SendSMS(alarm, strCaller, alarm.PhoneNumbers.Values, alarm.RegularMemberIDs.Values, alarm.Message, alarm.SensorReactionHistoryID);
                    //m_factory.EmailManager.SendEmail(alarm, strCaller, alarm.Emails.Values, alarm.RegularMemberIDs.Values, alarm.Message, alarm.SensorReactionHistoryID);

                    // TODO: 테스트 용도로 임시 고정값에게만 문자 및 메일 송신
                    int nResult = -1;
                    if (dicPhoneNumbers.Count > 0)
                        nResult = m_factory.SMSManager.SendSMS(alarm, strCaller, dicPhoneNumbers.Values, alarm.RegularMemberIDs.Values, alarm.Message, alarm.SensorReactionHistoryID);

                    if (nResult == ErrorMessageType.SUCCESS)
                    {
                        SensorZoneHistory.DetectionType detectStatus = alarm.IsReal ? SensorZoneHistory.DetectionType.Real : SensorZoneHistory.DetectionType.Test;
                        ((AlarmManager)m_mainManager.AlarmManager).AddReactionHistory(alarm, (int)SensorReactionHistory.ReactionTypes.SEND_SMS, DateTime.Now, alarm.Message, alarm.ReactionHistoryParam1, alarm.ReactionHistoryParam2, alarm.ReactionHistoryParam3, alarm.ReactionHistoryParam4, alarm.ReactionHistoryParam5, m_mainManager.SDMSDataManager);
                    }

                    // TODO: 메일 보내기
                    if (dicEmails.Count > 0)
                        nResult = m_factory.EmailManager.SendEmail(alarm, strCaller, dicEmails.Values, alarm.RegularMemberIDs.Values, alarm.Message, alarm.SensorReactionHistoryID);
                    // TODO: 작업 완료 후 메일도 Reaction history 추가
                    //if (nResult == ErrorMessageType.SUCCESS)
                    //{
                    //    SensorZoneHistory.DetectionType detectStatus = alarm.IsReal ? SensorZoneHistory.DetectionType.Real : SensorZoneHistory.DetectionType.Test;
                    //    ((AlarmManager)m_mainManager.AlarmManager).AddReactionHistory(alarm, (int)SensorReactionHistory.ReactionTypes.SEND_SMS, DateTime.Now, alarm.Message, alarm.ReactionHistoryParam1, alarm.ReactionHistoryParam2, alarm.ReactionHistoryParam3, alarm.ReactionHistoryParam4, alarm.ReactionHistoryParam5, m_mainManager.SDMSDataManager);
                    //}

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

        public override string NeedSMSCaller()
        {
            return null;
        }

        public override string NeedEmailCaller()
        {
            return null;
        }

        public static void SetNetVisionBaseURL(string strURL)
        {
            NetvisionManager.SetBaseURL(strURL);
        }
    }
}
