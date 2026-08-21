using System.Collections.Generic;
using AgentFactory.BLL;
using dnsData.Alarm;
using SDMS.IDAL;
using SDMS.Model.Config;
using SDMS.Model.History;
using SDMS.Model.Sensor;
using SDMS.Model.Spatial;
using dnsData.Sensor;
using Common.Model.Option;
using TeamEditor.Model.Sop.Team;
using TeamEditor.BLL;
using System.Threading;
using dnsSopID;
using dnsSMS;

namespace SafetyServer.BLL.Process
{
    using Data.Models;

    public class SMSManager : BaseSMSManager
    {
        private class SMSData
        {
            public AlarmData Alarm
            {
                get;
                set;
            }

            public string Caller
            {
                get;
                set;
            }

            public ICollection<string> PhoneNumbers
            {
                get;
                set;
            }

            public ICollection<int> RegularMemberIDs
            {
                get;
                set;
            }

            public string Message
            {
                get;
                set;
            }

            public int SensorReactionHistoryID { get; set; }

            public SMSData(AlarmData alarm, string strCaller, ICollection<string> phoneNumbers, ICollection<int> regularMemberIDs, string strMessage, int nSensorReactionHistoryID)
            {
                Alarm = alarm;
                Caller = strCaller;
                PhoneNumbers = phoneNumbers;
                RegularMemberIDs = regularMemberIDs;
                Message = strMessage;
                SensorReactionHistoryID = nSensorReactionHistoryID;
            }
        }

        private MainManager m_mainManager = null;

        public SMSManager(Factory factory, MainManager mainManager)
            : base(factory)
        {
            factory.SMSManager = this;
            m_mainManager = mainManager;
        }

        public static SMS.SMSMessageTypes ReactionTypeToMessageType(SensorReactionHistory.ReactionTypes reactionType, Facility.FacilityType sensorType)
        {
            if (reactionType == SensorReactionHistory.ReactionTypes.BEGIN_STATUS)
            {
                if (BaseBroadcastManager.IsFireSensor(sensorType))
                    return SMS.SMSMessageTypes.DETECT_FIRE;
                else if (BaseBroadcastManager.IsPSMSensor(sensorType))
                    return SMS.SMSMessageTypes.DETECT_PSM;
                else if (BaseBroadcastManager.IsSecuritySensor(sensorType))
                    return SMS.SMSMessageTypes.DETECT_SECURITY;
                else if (BaseBroadcastManager.IsEarthquakeSensor(sensorType))
                    return SMS.SMSMessageTypes.DETECT_EARTHQUAKE;
                else if (BaseBroadcastManager.IsTemperatureHumiditySensor(sensorType))
                    return SMS.SMSMessageTypes.DETECT_TH;
                else if (BaseBroadcastManager.IsETCSensor(sensorType))
                    return SMS.SMSMessageTypes.DETECT_ETC;
            }
            else if (reactionType == SensorReactionHistory.ReactionTypes.MALFUNCTION || reactionType == SensorReactionHistory.ReactionTypes.USER_RESET || reactionType == SensorReactionHistory.ReactionTypes.END_STATUS)
            {
                if (BaseBroadcastManager.IsFireSensor(sensorType))
                    return SMS.SMSMessageTypes.RESET_FIRE;
                else if (BaseBroadcastManager.IsPSMSensor(sensorType))
                    return SMS.SMSMessageTypes.RESET_PSM;
                else if (BaseBroadcastManager.IsSecuritySensor(sensorType))
                    return SMS.SMSMessageTypes.RESET_SECURITY;
                else if (BaseBroadcastManager.IsTemperatureHumiditySensor(sensorType))
                    return SMS.SMSMessageTypes.RESET_TH;
                else if (BaseBroadcastManager.IsETCSensor(sensorType))
                    return SMS.SMSMessageTypes.RESET_ETC;
            }
            else if (reactionType == SensorReactionHistory.ReactionTypes.NOTIFY_SIGNAL)
            {
                if (BaseBroadcastManager.IsFireSensor(sensorType))
                    return SMS.SMSMessageTypes.REPORT_FIRE;
                else if (BaseBroadcastManager.IsPSMSensor(sensorType))
                    return SMS.SMSMessageTypes.REPORT_PSM;
                else if (BaseBroadcastManager.IsSecuritySensor(sensorType))
                    return SMS.SMSMessageTypes.REPORT_SECURITY;
                else if (BaseBroadcastManager.IsETCSensor(sensorType))
                    return SMS.SMSMessageTypes.REPORT_ETC;
            }

            return SMS.SMSMessageTypes.UNKNOWN;
        }

        public override int GetPhoneNumbers(AlarmData alarm, SMS.SMSMessageTypes type)
        {
            SensorZone sensorZone = m_mainManager.SensorManager.GetSensorZone(alarm.SensorZoneID);

            if (sensorZone == null)
                return 0;

            bool isDetectTime = true, allCompanyMember = false;
            Facility.FacilityType facilityType = Facility.FacilityType.NONE;

            if (type == SMS.SMSMessageTypes.DETECT_FIRE)
                facilityType = Facility.FacilityType.FIRE_SENSOR;
            else if (type == SMS.SMSMessageTypes.DETECT_PSM)
                facilityType = Facility.FacilityType.PSM_SENSOR;
            else if (type == SMS.SMSMessageTypes.DETECT_SECURITY)
                facilityType = Facility.FacilityType.Security_Sensor;
            else if (type == SMS.SMSMessageTypes.DETECT_EARTHQUAKE)
            {
                AddEarthquakePhoneNumbners(alarm);
                return alarm.PhoneNumbers.Count;
            }
            else if (type == SMS.SMSMessageTypes.DETECT_TH)
                facilityType = Facility.FacilityType.TEMPERATURE_HUMIDITY;
            else if (type == SMS.SMSMessageTypes.DETECT_ETC)
                facilityType = alarm.SensorType;
            else if (type == SMS.SMSMessageTypes.REPORT_FIRE || type == SMS.SMSMessageTypes.REPORT_PSM ||
                        type == SMS.SMSMessageTypes.REPORT_SECURITY || type == SMS.SMSMessageTypes.REPORT_ETC)
            {
                // 재난전파시 담당자를 따로 지정하여 사용하는가?
                // 이 값이 false이면 재난 전파시 전직원에게 문자메시지를 발송한다.
                if (UseFacilityManagerType())
                {
                    isDetectTime = false;
                }
                else
                {
                    // 재난신고이지만 별도의 ReportFacilityManager를 사용하지 않으므로 isDetectTime = true로 설정한다.
                    isDetectTime = true;
                    allCompanyMember = true;
                }

                // 탐지시에 사용했던 문자메시지 수신자들을 모두 지운다.
                alarm.PhoneNumbers.Clear();
                alarm.Emails.Clear();

                if (type == SMS.SMSMessageTypes.REPORT_FIRE)
                    facilityType = Facility.FacilityType.FIRE_SENSOR;
                else if (type == SMS.SMSMessageTypes.REPORT_PSM)
                    facilityType = Facility.FacilityType.PSM_SENSOR;
                else if (type == SMS.SMSMessageTypes.REPORT_SECURITY)
                    facilityType = Facility.FacilityType.Security_Sensor;
                else if (type == SMS.SMSMessageTypes.REPORT_ETC)
                    facilityType = alarm.SensorType;
            }
            else if (type == SMS.SMSMessageTypes.RESET_FIRE || type == SMS.SMSMessageTypes.RESET_PSM || type == SMS.SMSMessageTypes.RESET_SECURITY ||
                        type == SMS.SMSMessageTypes.RESET_TH || type == SMS.SMSMessageTypes.RESET_ETC)
            {
                // 신호복구의 경우 탐지시에 문자메시지를 받았던 수신자들에게 그대로 다시 보낸다.
                return alarm.PhoneNumbers.Count;
            }
            else
                return 0;

            EquipmentZone equipZone = m_mainManager.SensorManager.GetEquipmentZone(sensorZone.EquipZoneID);

            if (equipZone != null)
            {
                FacilityManagerGroup group = m_mainManager.MemberManager.GetEquipZoneFacilityManagerGroup(facilityType, equipZone, isDetectTime);
                AddPhoneNumberFromGroup(group, alarm.PhoneNumbers, alarm.Emails, alarm.RegularMemberIDs);

                // EquipZone FacilityManager 뿐만 아니라 건물 Manager와 전체 Manager까지 모두 포함한다.
                if (equipZone.LinkedZoneIDs.Count > 0)
                {
                    Zone zone = m_mainManager.SensorManager.GetZone(equipZone.LinkedZoneIDs[0]);

                    if (zone != null)
                        AddPhoneNumbers(facilityType, zone, alarm.PhoneNumbers, alarm.Emails, alarm.RegularMemberIDs, isDetectTime);
                }
                else
                    AddPhoneNumberFromGroup(m_mainManager.MemberManager.GetEntireFacilityManagerGroup(facilityType, isDetectTime), alarm.PhoneNumbers, alarm.Emails, alarm.RegularMemberIDs);
            }

            if (allCompanyMember)
                m_mainManager.MemberManager.AddAllRegularMemberPhoneNumbers(alarm.PhoneNumbers, alarm.RegularMemberIDs);

            return alarm.PhoneNumbers.Count;
        }

        // 재난전파시 담당자를 따로 지정하여 사용하는가?
        // 이 값이 false이면 재난 전파시 전직원에게 문자메시지를 발송한다.
        private bool UseFacilityManagerType()
        {
            string strErrorMessage;
            List<Common.Model.Option.Options> options = m_mainManager.CommonDataManager.GetSelectManager().SelectOption(Common.Model.Option.Options.OptionTarget.SDMS, "UseFacilityManagerType", out strErrorMessage);

            if (options == null || options.Count == 0)
                return false;

            string strPropertyValue = options[0].PropertyValue;

            if (strPropertyValue == null || strPropertyValue.Length == 0)
                return false;

            strPropertyValue = strPropertyValue.Trim();

            if (strPropertyValue == "1" || string.Compare(strPropertyValue, "true", true) == 0)
                return true;

            return false;
        }

        private void AddEarthquakePhoneNumbners(AlarmData alarm)
        {
            if (alarm.Tag != null && alarm.Tag is EarthquakeOption)
            {
                EarthquakeOption option = (EarthquakeOption)alarm.Tag;

                if (option.UseSMS)
                    AddAllMembers(alarm);
            }
        }

        private void AddAllMembers(AlarmData alarm)
        {
            alarm.PhoneNumbers.Clear();
            alarm.RegularMemberIDs.Clear();

            ICollection<RegularMember> regularMembers = m_mainManager.MemberManager.GetAllRegularMember();

            foreach (RegularMember member in regularMembers)
            {
                alarm.PhoneNumbers[member.PhoneNumber] = member.PhoneNumber;
                alarm.Emails[member.Email] = member.Email;
                alarm.RegularMemberIDs[member.ID] = member.ID;

            }
        }

        private void AddPhoneNumbers(Facility.FacilityType type, Zone zone, Dictionary<string, string> dicPhoneNumbers, Dictionary<string, string> dicEmails, Dictionary<int, int> dicRegularMemberIDs, bool isDetectTime)
        {
            Building building = zone.BuildingID == null ? null : m_mainManager.SensorManager.GetBuilding((int)zone.BuildingID);

            FacilityManagerGroup group = null;

            if (building == null)
                group = m_mainManager.MemberManager.GetOutdoorFacilityManagerGroup((int)type, zone, isDetectTime);
            else
                group = m_mainManager.MemberManager.GetBuildingFacilityManagerGroup((int)type, building, isDetectTime);

            AddPhoneNumberFromGroup(group, dicPhoneNumbers, dicEmails, dicRegularMemberIDs);

            // 건물별 담당자 뿐만 아니라 전체 담당자에게도 문자메시지를 보낸다.
            AddPhoneNumberFromGroup(m_mainManager.MemberManager.GetEntireFacilityManagerGroup(type, isDetectTime), dicPhoneNumbers, dicEmails, dicRegularMemberIDs);
        }

        private void AddPhoneNumberFromGroup(FacilityManagerGroup group, Dictionary<string, string> dicPhoneNumbers, Dictionary<string, string> dicEmails, Dictionary<int, int> dicRegularMemberIDs)
        {
            if (group == null)
                return;

            foreach (FacilityManagerEx mgr in group.CompanyMembers)
            {
                AddPhoneNumber(mgr, dicPhoneNumbers, dicEmails, dicRegularMemberIDs);
            }

            foreach (FacilityManagerEx mgr in group.RegularTeams)
            {
                AddPhoneNumber(mgr, dicPhoneNumbers, dicEmails, dicRegularMemberIDs);
            }
        }

        private void AddPhoneNumber(FacilityManagerEx mgr, Dictionary<string, string> dicPhoneNumbers, Dictionary<string, string> dicMails, Dictionary<int, int> dicRegularMemberIDs)
        {
            if (mgr.MemberType == (int)TemporaryMemberData.MemberType.RegularMember)
            {
                RegularMember member = (RegularMember)mgr.Tag;

                if (member == null)
                    return;

                dicPhoneNumbers[member.PhoneNumber] = member.PhoneNumber;
                dicMails[member.Email] = member.Email;
                dicRegularMemberIDs[member.ID] = member.ID;
            }
            else if (mgr.MemberType == (int)TemporaryMemberData.MemberType.RegularTeam)
            {
                Regular team = (Regular)mgr.Tag;
                AddRegularTeamPhoneNumber(mgr, team, dicPhoneNumbers, dicMails, dicRegularMemberIDs);
            }
        }

        private void AddRegularTeamPhoneNumber(FacilityManagerEx mgr, Regular team, Dictionary<string, string> dicPhoneNumbers, Dictionary<string, string> dicMails, Dictionary<int, int> dicRegularMemberIDs)
        {
            if (team == null)
                return;

            List<RegularMember> members = m_mainManager.MemberManager.GetRegularTeamMembers(team);

            if (members != null)
            {
                foreach (RegularMember member in members)
                {
                    dicPhoneNumbers[member.PhoneNumber] = member.PhoneNumber;
                    dicMails[member.Email] = member.Email;
                    dicRegularMemberIDs[member.ID] = member.ID;
                }
            }

            Dictionary<Regular.Fields, object> dicConditions = new Dictionary<Regular.Fields, object>();
            dicConditions[Regular.Fields.ParentTeamID] = team.ID;

            string strErrorMessage;
            List<Regular> childTeams = m_mainManager.TeamDataManager.GetSelectManager().SelectRegulars(dicConditions, out strErrorMessage);

            if (childTeams != null)
            {
                foreach (Regular childTeam in childTeams)
                {
                    AddRegularTeamPhoneNumber(mgr, childTeam, dicPhoneNumbers, dicMails, dicRegularMemberIDs);
                }
            }
        }

        // 훈련모드일 경우 훈련모드에 맞는 태그문구를 리턴한다.
        // 그렇지 않을 경우 빈 문자열을 리턴한다.
        public override string GetTrainingModeString()
        {
            string strErrorMessage;
            List<Common.Model.Option.Options> options = m_mainManager.CommonDataManager.GetSelectManager().SelectOption(Common.Model.Option.Options.OptionTarget.SDMS, "TrainingMode", out strErrorMessage);

            if (options == null)
                return "";

            foreach (Common.Model.Option.Options option in options)
            {
                if (option.SiteID == m_mainManager.CommonDataManager.SiteID)
                {
                    if (option.PropertyValue == "1" || option.PropertyValue.ToLower() == "true")
                    {
                        List<Common.Model.Option.Options> options2 = m_mainManager.CommonDataManager.GetSelectManager().SelectOption(Common.Model.Option.Options.OptionTarget.SDMS, "HeaderMsg", out strErrorMessage);

                        if (options2 != null && options2.Count > 0)
                            return "[" + options2[0].PropertyValue + "]";
                        else
                            return "[훈련상황]";
                    }

                    break;
                }
            }

            return "";
        }

        public override int SendSMS(string strCaller, ICollection<string> phoneNumbers, string strMessage, int nSensorReactionHistoryID)
        {
            SMSData data = new SMSData(null, strCaller, phoneNumbers, null, strMessage, nSensorReactionHistoryID);

            Thread t = new Thread(new ParameterizedThreadStart(SendSMSThread));
            t.Start(data);

            return ErrorMessageType.SUCCESS;
        }

        public override int SendSMS(AlarmData alarm, string strCaller, ICollection<string> phoneNumbers, ICollection<int> regularMemberIDs, string strMessage, int nSensorReactionHistoryID)
        {
            SMSData data = new SMSData(alarm, strCaller, phoneNumbers, regularMemberIDs, strMessage, nSensorReactionHistoryID);

            Thread t = new Thread(new ParameterizedThreadStart(SendSMSThread));
            t.Start(data);

            return ErrorMessageType.SUCCESS;
        }

        private void SendSMSThread(object arg)
        {
            SMSData data = (SMSData)arg;

            IMessageClient client = MessageClientFactory.CreateMessageClient(m_mainManager.CommonDataManager, m_mainManager.SDMSDataManager);

            if (client != null)
            {
                MessageContent contents = new MessageContent();
                contents.Caller = data.Caller;
                contents.PhoneNumbers.AddRange(data.PhoneNumbers);
                contents.Message = data.Message;
                //contents.Tag = data.DBManager;
                contents.SensorReactionHistoryID = data.SensorReactionHistoryID;

                // 수신자번호 가운데 빈문자열이 있으면 없앤다.
                int nIndex = contents.PhoneNumbers.IndexOf("");

                if (nIndex >= 0)
                    contents.PhoneNumbers.RemoveAt(nIndex);

                if (client.SendSMS(contents))
                {
                    if (data.Alarm != null)
                    {
                        List<int> regularMemberIDs = new List<int>();
                        regularMemberIDs.AddRange(data.RegularMemberIDs);
                        SaveSMSHistory(data.Alarm, regularMemberIDs, data.Message);
                    }
                }
            }
        }

        private void SaveSMSHistory(AlarmData alarm, List<int> regularMemberIds, string strMessage)
        {
            m_mainManager.SDMSDataManager.GetCreateManager().CreateSMSHistory(alarm.SensorZoneHistoryID, alarm.SensorReactionHistoryID, strMessage, true, regularMemberIds);
        }
    }
}
