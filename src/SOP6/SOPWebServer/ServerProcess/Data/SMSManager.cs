using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgentFactory;
using UnE.Spatial;
using UnE.Sensor;
using DBUtility2;
using System.Collections;
using System.Threading;

namespace ServerProcess.Data
{
    public class SMSManager : BaseSMSManager
    {
        private class SMSData
        {
            public DirectDBManager DBManager
            {
                get;
                set;
            }

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

            public List<string> PhoneNumbers
            {
                get;
                set;
            }

            public List<int> RegularMemberIDs
            {
                get;
                set;
            }

            public List<int> ExternalMemberIDs
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

            public SMSData(DirectDBManager dbMgr, AlarmData alarm, string strCaller, List<string> phoneNumbers, List<int> regularMemberIDs, List<int> externalMemberIDs, string strMessage, int nSensorReactionHistoryID)
            {
                DBManager = dbMgr;
                Alarm = alarm;
                Caller = strCaller;
                PhoneNumbers = phoneNumbers;
                RegularMemberIDs = regularMemberIDs;
                ExternalMemberIDs = externalMemberIDs;
                Message = strMessage;
                SensorReactionHistoryID = nSensorReactionHistoryID;
            }
        }

        public SMSManager(Factory factory)
            : base(factory)
        {
        }

        public static SMSMessageType ReactionTypeToMessageType(BaseProcessManager.ReactionType reactionType, IFacility.FacilityType sensorType)
        {
            if (reactionType == BaseProcessManager.ReactionType.BEGIN_STATUS)
            {
                if (BaseBroadcastManager.IsFireSensor(sensorType))
                    return SMSMessageType.DETECT_FIRE;
                else if (BaseBroadcastManager.IsPSMSensor(sensorType))
                    return SMSMessageType.DETECT_PSM;
                else if (BaseBroadcastManager.IsSecuritySensor(sensorType))
                    return SMSMessageType.DETECT_SECURITY;
                else if (BaseBroadcastManager.IsEarthquakeSensor(sensorType))
                    return SMSMessageType.DETECT_EARTHQUAKE;
                else if (BaseBroadcastManager.IsTemperatureHumiditySensor(sensorType))
                    return SMSMessageType.DETECT_TH;
                else if (BaseBroadcastManager.IsETCSensor(sensorType))
                    return SMSMessageType.DETECT_ETC;
            }
            else if (reactionType == BaseProcessManager.ReactionType.MALFUNCTION || reactionType == BaseProcessManager.ReactionType.USER_RESET || reactionType == BaseProcessManager.ReactionType.END_STATUS)
            {
                if (BaseBroadcastManager.IsFireSensor(sensorType))
                    return SMSMessageType.RESET_FIRE;
                else if (BaseBroadcastManager.IsPSMSensor(sensorType))
                    return SMSMessageType.RESET_PSM;
                else if (BaseBroadcastManager.IsSecuritySensor(sensorType))
                    return SMSMessageType.RESET_SECURITY;
                else if (BaseBroadcastManager.IsTemperatureHumiditySensor(sensorType))
                    return SMSMessageType.RESET_TH;
                else if (BaseBroadcastManager.IsETCSensor(sensorType))
                    return SMSMessageType.RESET_ETC;
            }
            else if (reactionType == BaseProcessManager.ReactionType.NOTIFY_SIGNAL)
            {
                if (BaseBroadcastManager.IsFireSensor(sensorType))
                    return SMSMessageType.REPORT_FIRE;
                else if (BaseBroadcastManager.IsPSMSensor(sensorType))
                    return SMSMessageType.REPORT_PSM;
                else if (BaseBroadcastManager.IsSecuritySensor(sensorType))
                    return SMSMessageType.REPORT_SECURITY;
                else if (BaseBroadcastManager.IsETCSensor(sensorType))
                    return SMSMessageType.REPORT_ETC;
            }
            
            return SMSMessageType.UNKNOWN;
        }

        public override int GetPhoneNumbers(DirectDBManager dbMgr, AlarmData alarm, SMSMessageType type)
        {
            using (DdMonitor.Lock(MemberManager.Instance.MemberCriticalSection))
            {
                SensorZone sensorZone = SensorZoneManager.Instance.GetSensorZone(alarm.SensorZoneID);

                if (sensorZone == null || sensorZone.EquipZone == null)
                    return 0;

                bool isDetectTime = true, allCompanyMember = false;
                IFacility.FacilityType facilityType = IFacility.FacilityType.NONE;

                if (type == SMSMessageType.DETECT_FIRE)
                    facilityType = IFacility.FacilityType.FIRE_SENSOR;
                else if (type == SMSMessageType.DETECT_PSM)
                    facilityType = IFacility.FacilityType.PSM_SENSOR;
                else if (type == SMSMessageType.DETECT_SECURITY)
                    facilityType = IFacility.FacilityType.Security_Sensor;
                else if (type == SMSMessageType.DETECT_EARTHQUAKE)
                { 
                    AddEarthquakePhoneNumbners(alarm);
                    return alarm.PhoneNumbers.Count;
                }
                else if (type == SMSMessageType.DETECT_TH)
                    facilityType = IFacility.FacilityType.TEMPERATURE_HUMIDITY;
                else if (type == SMSMessageType.DETECT_ETC)
                    facilityType = alarm.SensorType;
                else if (type == SMSMessageType.REPORT_FIRE || type == SMSMessageType.REPORT_PSM || 
                         type == SMSMessageType.REPORT_SECURITY || type == SMSMessageType.REPORT_ETC)
                {
                    if (MemberManager.Instance.UseReportFacilityManagers)
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

                    if (type == SMSMessageType.REPORT_FIRE)
                        facilityType = IFacility.FacilityType.FIRE_SENSOR;
                    else if (type == SMSMessageType.REPORT_PSM)
                        facilityType = IFacility.FacilityType.PSM_SENSOR;
                    else if (type == SMSMessageType.REPORT_SECURITY)
                        facilityType = IFacility.FacilityType.Security_Sensor;
                    else if (type == SMSMessageType.REPORT_ETC)
                        facilityType = alarm.SensorType;
                }
                else if (type == SMSMessageType.RESET_FIRE || type == SMSMessageType.RESET_PSM || type == SMSMessageType.RESET_SECURITY || 
                         type == SMSMessageType.RESET_TH || type == SMSMessageType.RESET_ETC)
                {
                    // 신호복구의 경우 탐지시에 문자메시지를 받았던 수신자들에게 그대로 다시 보낸다.
                    return alarm.PhoneNumbers.Count;
                }
                else
                    return 0;

                FacilityManagerGroup group = MemberManager.Instance.GetEquipZoneFacilityManagerGroup(facilityType, sensorZone.EquipZone, isDetectTime);
                AddPhoneNumberFromGroup(dbMgr, group, alarm.PhoneNumbers, alarm.RegularMemberIDs, alarm.ExternalMemberIDs);

                // EquipZone FacilityManager 뿐만 아니라 건물 Manager와 전체 Manager까지 모두 포함한다.
                if (sensorZone.EquipZone.LinkedZoneList != null && sensorZone.EquipZone.LinkedZoneList.Count > 0)
                    AddPhoneNumbers(dbMgr, facilityType, (Zone)sensorZone.EquipZone.LinkedZoneList[0], alarm.PhoneNumbers, alarm.RegularMemberIDs, alarm.ExternalMemberIDs, isDetectTime);
                else
                    AddPhoneNumberFromGroup(dbMgr, MemberManager.Instance.GetEntireFacilityManagerGroup(facilityType, isDetectTime), alarm.PhoneNumbers, alarm.RegularMemberIDs, alarm.ExternalMemberIDs);

                if (allCompanyMember)
                    MemberManager.Instance.AddAllCompanyMemberPhoneNumbers(alarm.PhoneNumbers, alarm.RegularMemberIDs);
            }

            return alarm.PhoneNumbers.Count;
        }

        private void AddEarthquakePhoneNumbners(AlarmData alarm)
        {
            if (alarm.Tag != null && alarm.Tag is UnE.Earthquake.EarthquakeOption)
            {
                UnE.Earthquake.EarthquakeOption option = (UnE.Earthquake.EarthquakeOption)alarm.Tag;

                if (option.UseSMS)
                    AddAllMembers(alarm);
            }
        }

        private void AddAllMembers(AlarmData alarm)
        {
            alarm.PhoneNumbers.Clear();
            alarm.RegularMemberIDs.Clear();
            alarm.ExternalMemberIDs.Clear();

            List<DataCompanyMember> regularMembers = MemberManager.Instance.GetAllRegularMember();

            foreach (DataCompanyMember member in regularMembers)
            {
                alarm.PhoneNumbers[member.PhoneNumber] = member.PhoneNumber;
                alarm.RegularMemberIDs[member.ID] = member.ID;

            }

            List<DataExternalMember> externalMembers = MemberManager.Instance.GetAllExternalMember();
            
            foreach (DataExternalMember member in externalMembers)
            {
                alarm.PhoneNumbers[member.PhoneNumber] = member.PhoneNumber;
                alarm.ExternalMemberIDs[member.ID] = member.ID;
            }
        }
        
        private void AddPhoneNumbers(DirectDBManager dbMgr, IFacility.FacilityType type, Zone zone, Dictionary<string, string> dicPhoneNumbers, Dictionary<int, int> dicRegularMemberIDs, Dictionary<int, int> dicExternalMemberIDs, bool isDetectTime)
        {
            Building building = zone.Building;

            //Facility.FacilityType type = (Facility.FacilityType)sensor.Type;
            FacilityManagerGroup group = null;

            if (building == null)
                group = MemberManager.Instance.GetOutdoorFacilityManagerGroup((int)type, zone, isDetectTime);
            else
                group = MemberManager.Instance.GetBuildingFacilityManagerGroup((int)type, building, isDetectTime);

            AddPhoneNumberFromGroup(dbMgr, group, dicPhoneNumbers, dicRegularMemberIDs, dicExternalMemberIDs);

            // 건물별 담당자 뿐만 아니라 전체 담당자에게도 문자메시지를 보낸다.
            AddPhoneNumberFromGroup(dbMgr, MemberManager.Instance.GetEntireFacilityManagerGroup(type, isDetectTime), dicPhoneNumbers, dicRegularMemberIDs, dicExternalMemberIDs);
        }

        private void AddPhoneNumberFromGroup(DirectDBManager dbMgr, FacilityManagerGroup group, Dictionary<string, string> dicPhoneNumbers, Dictionary<int, int> dicRegularMemberIDs, Dictionary<int, int> dicExternalMemberIDs)
        {
            if (group == null)
                return;

            foreach (FacilityManager mgr in group.CompanyMembers)
            {
                AddPhoneNumber(dbMgr, mgr, dicPhoneNumbers, dicRegularMemberIDs, dicExternalMemberIDs);
            }

            // 171114 KYJ TEST
            //
            foreach (FacilityManager mgr in group.ExternalCompanyMembers)
            {
                AddPhoneNumber(dbMgr, mgr, dicPhoneNumbers, dicRegularMemberIDs, dicExternalMemberIDs);
            }

            foreach (FacilityManager mgr in group.RegularTeams)
            {
                AddPhoneNumber(dbMgr, mgr, dicPhoneNumbers, dicRegularMemberIDs, dicExternalMemberIDs);
            }

            foreach (FacilityManager mgr in group.ExternalTeams)
            {
                AddPhoneNumber(dbMgr, mgr, dicPhoneNumbers, dicRegularMemberIDs, dicExternalMemberIDs);
            }

            foreach (FacilityManager mgr in group.ControlRoomMembers)
            {
                AddPhoneNumber(dbMgr, mgr, dicPhoneNumbers, dicRegularMemberIDs, dicExternalMemberIDs);
            }
        }

        private void AddPhoneNumber(DirectDBManager dbMgr, FacilityManager mgr, Dictionary<string, string> dicPhoneNumbers, Dictionary<int, int> dicRegularMemberIDs, Dictionary<int, int> dicExternalMemberIDs)
        {
            if (mgr.MemberType == 0)
            {
                DataCompanyMember member = (DataCompanyMember)mgr.Tag;

                if (member == null)
                    return;

                dicPhoneNumbers[member.PhoneNumber] = member.PhoneNumber;
                dicRegularMemberIDs[member.ID] = member.ID;
            }
            else if (mgr.MemberType == 1 || mgr.MemberType == 4)
            {
                DataTeam team = (DataTeam)mgr.Tag;
                AddRegularTeamPhoneNumber(mgr, team, dicPhoneNumbers, dicRegularMemberIDs);
            }
            else if (mgr.MemberType == 2)
            {
                DataExternalMember member = (DataExternalMember)mgr.Tag;

                if (member == null)
                    return;

                dicPhoneNumbers[member.PhoneNumber] = member.PhoneNumber;
                dicExternalMemberIDs[member.ID] = member.ID;
            }
            else if (mgr.MemberType == 3 || mgr.MemberType == 5)
            {
                DataTeam team = (DataTeam)mgr.Tag;
                AddExternalTeamPhoneNumber(team, dicPhoneNumbers, dicExternalMemberIDs);
            }
            else if (mgr.MemberType == 6)
            {
                // 사용하지 않음
            }
            else if (mgr.MemberType == 7)
            {
                DataTeamControlRoom team = (DataTeamControlRoom)mgr.Tag;
                AddControlRoomPhoneNumbers(dbMgr, team, dicPhoneNumbers, dicRegularMemberIDs, dicExternalMemberIDs);
            }
        }

        private void AddControlRoomPhoneNumbers(DirectDBManager dbMgr, DataTeamControlRoom team, Dictionary<string, string> dicPhoneNumbers, Dictionary<int, int> dicRegularMemberIDs, Dictionary<int, int> dicExternalMemberIDs)
        {
            int nRoomID = team.ControlRoomID;
            int nPositionID = team.ControlTeamJobPositionID;
            string strSQL = "";

            if (nRoomID == 0)
            {
                strSQL = "select ctm.MemberType, ctm.MemberID ";
                strSQL += "from ControlRoom as cr, ControlWorkingTeam as cwt, ControlTeamMembers as ctm ";
                strSQL += "where cr.ID = cwt.RoomID and ctm.RoomID = cr.ID and ctm.TeamID = cwt.TeamID and ctm.MemberID is not NULL";
            }
            else if (nPositionID == 0)
            {
                strSQL = "select ctm.MemberType, ctm.MemberID ";
                strSQL += "from ControlRoom as cr, ControlWorkingTeam as cwt, ControlTeamMembers as ctm ";
                strSQL += string.Format("where cr.ID = cwt.RoomID and ctm.RoomID = cr.ID and ctm.TeamID = cwt.TeamID and cr.ID = {0} and ctm.MemberID is not NULL", nRoomID);
            }
            else
            {
                strSQL = "select ctm.MemberType, ctm.MemberID ";
                strSQL += "from ControlRoom as cr, ControlWorkingTeam as cwt, ControlTeamMembers as ctm ";
                strSQL += "where cr.ID = cwt.RoomID and ctm.RoomID = cr.ID and ctm.TeamID = cwt.TeamID and ";
                strSQL += string.Format("ctm.JobPosition = {0} and cr.ID = {1} and ctm.MemberID is not NULL", nPositionID, nRoomID);
            }

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nMemberType = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nMemberID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);

                if (nMemberType == 1)
                {
                    DataCompanyMember member = MemberManager.Instance.GetRegularMember(nMemberID);

                    if (member != null)
                    {
                        dicPhoneNumbers[member.PhoneNumber] = member.PhoneNumber;
                        dicRegularMemberIDs[member.ID] = member.ID;
                    }
                }
                else if (nMemberType == 4)
                {
                    DataExternalMember member = MemberManager.Instance.GetExternalMember(nMemberID);

                    if (member != null)
                    {
                        dicPhoneNumbers[member.PhoneNumber] = member.PhoneNumber;
                        dicExternalMemberIDs[member.ID] = member.ID;
                    }
                }
            }
        }

        private void AddExternalTeamPhoneNumber(DataTeam team, Dictionary<string, string> dicPhoneNumbers, Dictionary<int, int> dicExternalMemberIDs)
        {
            if (team == null)
                return;

            List<DataExternalMember> members = MemberManager.Instance.GetExternalTeamMembers(team);

            if (members != null)
            {
                foreach (DataExternalMember member in members)
                {
                    dicPhoneNumbers[member.PhoneNumber] = member.PhoneNumber;
                    dicExternalMemberIDs[member.ID] = member.ID;
                }
            }

            foreach (DataTeam childTeam in team.ChildTeams)
            {
                AddExternalTeamPhoneNumber(childTeam, dicPhoneNumbers, dicExternalMemberIDs);
            }
        }

        private void AddRegularTeamPhoneNumber(FacilityManager mgr, DataTeam team, Dictionary<string, string> dicPhoneNumbers, Dictionary<int, int> dicRegularMemberIDs)
        {
            if (team == null)
                return;

            List<DataCompanyMember> members = MemberManager.Instance.GetRegularTeamMembers(team);

            if (members != null)
            {
                foreach (DataCompanyMember member in members)
                {
                    if (mgr.LevelLimit > 0)
                    {
                        if (mgr.UpperLimit > 0)
                        {
                            // member.LevelID 또는 그 상위 직급에게 문자메시지를 보낸다.
                            if (member.LevelID > 0 && member.LevelID <= mgr.LevelLimit)
                            {
                                dicPhoneNumbers[member.PhoneNumber] = member.PhoneNumber;
                                dicRegularMemberIDs[member.ID] = member.ID;
                            }
                        }
                        else if (mgr.UpperLimit < 0)
                        {
                            // member.LevelID 또는 그 하위 직급에게 문자메시지를 보낸다.
                            if ((member.LevelID > 0 && member.LevelID >= mgr.LevelLimit) ||
                                member.LevelID == 0)
                            {
                                dicPhoneNumbers[member.PhoneNumber] = member.PhoneNumber;
                                dicRegularMemberIDs[member.ID] = member.ID;
                            }
                        }
                        else
                        {
                            if (member.LevelID == mgr.LevelLimit)
                            {
                                dicPhoneNumbers[member.PhoneNumber] = member.PhoneNumber;
                                dicRegularMemberIDs[member.ID] = member.ID;
                            }
                        }
                    }
                    else
                    {
                        dicPhoneNumbers[member.PhoneNumber] = member.PhoneNumber;
                        dicRegularMemberIDs[member.ID] = member.ID;
                    }
                }
            }

            foreach (DataTeam childTeam in team.ChildTeams)
            {
                AddRegularTeamPhoneNumber(mgr, childTeam, dicPhoneNumbers, dicRegularMemberIDs);
            }
        }

        // 훈련모드일 경우 훈련모드에 맞는 태그문구를 리턴한다.
        // 그렇지 않을 경우 빈 문자열을 리턴한다.
        public override string GetTrainingModeString(DirectDBManager dbMgr)
        {
            string strSQL = "SELECT PropertyValue FROM OptionSDMS where PropertyName ='TranningMode' and SiteID = " + dbMgr.SiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return "";

            VariousData<int> value = WebDBManager.GetIntField(arrResult[0].ToString());

            if (value != null && value.Data == 1)
            {
                strSQL = "SELECT PropertyValue FROM OptionSDMS where PropertyName ='HeaderMsg' and SiteID = " + dbMgr.SiteID.ToString();
                arrResult = dbMgr.GetResultData(strSQL);

                if (arrResult == null || arrResult.Count == 0)
                    return "[훈련상황]";

                string strTag = WebDBManager.GetStringField(arrResult[0]);

                if (strTag == null)
                    return "[훈련상황]";

                return "[" + strTag + "]";
            }

            return "";
        }

        public override int SendSMS(DirectDBManager dbMgr, string strCaller, List<string> phoneNumbers, string strMessage, int nSensorReactionHistoryID)
        {
            SMSData data = new SMSData(dbMgr.Clone(), null, strCaller, phoneNumbers, null, null, strMessage, nSensorReactionHistoryID);

            Thread t = new Thread(new ParameterizedThreadStart(SendSMSThread));
            t.Start(data);

            return SOPWebServer.ErrorMessageType.SUCCESS;
        }

        public override int SendSMS(DirectDBManager dbMgr, AlarmData alarm, string strCaller, List<string> phoneNumbers, List<int> regularMemberIDs, List<int> externalMemberIDs, string strMessage, int nSensorReactionHistoryID)
        {
            SMSData data = new SMSData(dbMgr.Clone(), alarm, strCaller, phoneNumbers, regularMemberIDs, externalMemberIDs, strMessage, nSensorReactionHistoryID);

            Thread t = new Thread(new ParameterizedThreadStart(SendSMSThread));
            t.Start(data);

            return SOPWebServer.ErrorMessageType.SUCCESS;
        }

        private void SendSMSThread(object arg)
        {
            SMSData data = (SMSData)arg;

            libSMS.IMessageClient client = libSMS.MessageClientFactory.CreateMessageClient(data.DBManager.SiteID);

            if (client != null)
            {

                libSMS.MessageContent contents = new libSMS.MessageContent();
                contents.Caller = data.Caller;
                contents.PhoneNumbers.AddRange(data.PhoneNumbers);
                contents.Message = data.Message;
                contents.Tag = data.DBManager;
                contents.SensorReactionHistoryID = data.SensorReactionHistoryID;

                // 수신자번호 가운데 빈문자열이 있으면 없앤다.
                int nIndex = contents.PhoneNumbers.IndexOf("");

                if (nIndex >= 0)
                    contents.PhoneNumbers.RemoveAt(nIndex);

                if (client.SendSMS(contents))
                {
                    if (data.Alarm != null)
                    {
                        if (data.DBManager.Connect())
                        {
                            SaveSMSHistory(data.DBManager, data.Alarm, data.RegularMemberIDs, data.ExternalMemberIDs, data.Message);
                            data.DBManager.Close();
                        }
                    }
                }
            }
        }

        private void SaveSMSHistory(DirectDBManager dbMgr, AlarmData alarm, List<int> regularMemberIds, List<int> externalMemberIDs, string strMessage)
        {
            string strRegularMemberIDs = "", strExternalMemberIDs = "";

            if (regularMemberIds != null)
            {
                foreach (int nID in regularMemberIds)
                {
                    if (strRegularMemberIDs.Length == 0)
                        strRegularMemberIDs = nID.ToString();
                    else
                        strRegularMemberIDs += "," + nID.ToString();
                }
            }

            if (externalMemberIDs != null)
            {
                foreach (int nID in externalMemberIDs)
                {
                    if (strExternalMemberIDs.Length == 0)
                        strExternalMemberIDs = nID.ToString();
                    else
                        strExternalMemberIDs += "," + nID.ToString();
                }
            }

            string strSQL = "Insert into SDMSSMSHistory (ID,SensorHistoryID, ReactionHistoryID, CompanyMemberIDList, ExternalCompanyMemberIDList, SMSMessage, SendType) ";
            strSQL += string.Format("Select isnull(max(id) + 1, 1), {0}, {1}, '{2}', '{3}', '{4}', 1 from SDMSSMSHistory",
                alarm.SensorZoneHistoryID,
                alarm.SensorReactionHistoryID,
                strRegularMemberIDs,
                strExternalMemberIDs,
                strMessage);

            dbMgr.GetResultData(strSQL);
        }
    }
}
