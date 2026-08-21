using System.Collections.Generic;
using SDMS.BLL.Models.Alarm;
using SDMS.Model.History;
using SDMS.Model.CCTV;
using SDMS.Model.Sensor;
using SDMS.IDAL;
using TeamEditor.Model.Sop.Team;

namespace SafetyServer.BLL.Data.Models
{
    public class SafetyAlarm : AlarmData
    {
        private string m_strAlarmType = "";
        private string m_strAlarmInfo = "";
        private CCTV m_cctv = null;
        private RegularMember m_member = null;

        public string AlarmType
        {
            get { return m_strAlarmType; }
            set { m_strAlarmType = value; }
        }

        public string AlarmInfo
        {
            get { return m_strAlarmInfo; }
            set { m_strAlarmInfo = value; }
        }

        public CCTV Cctv
        {
            get { return m_cctv; }
            set { m_cctv = value; }
        }

        public RegularMember Member
        {
            get { return m_member; }
            set { m_member = value; }
        }

        public SafetyAlarm()
        {
        }

        public SafetyAlarm(AlarmData alarm)
        {
            Copy(alarm, this);
        }

        public void SetAlarm(AlarmData alarm)
        {
            Copy(alarm, this);
        }

        public static void SetAlarmInfo(List<SafetyAlarm> alarms, IDataManager dataManager, TeamEditor.IDAL.IDataManager teamDataManager)
        {
            // Key : SensorZoneHistory ID
            Dictionary<int, SafetyAlarm> dicAlarms = SetCCTVs(alarms, dataManager);

            if (dicAlarms == null)
                return;

            SetAlarmInfo(dicAlarms, dataManager, teamDataManager);
        }

        private static void SetAlarmInfo(Dictionary<int, SafetyAlarm> dicAlarms, IDataManager dataManager, TeamEditor.IDAL.IDataManager teamDataManager)
        {
            // Key : SensorZone ID
            // Value : OriginSensor ID
            Dictionary<int, int?> dicSensorZones = new Dictionary<int, int?>();
            string strSensorZoneIDs = "";

            foreach (KeyValuePair<int, SafetyAlarm> pair in dicAlarms)
            {
                if (dicSensorZones.ContainsKey(pair.Value.SensorZoneID))
                    continue;
                else
                    dicSensorZones[pair.Value.SensorZoneID] = null;

                if (strSensorZoneIDs.Length == 0)
                    strSensorZoneIDs = pair.Value.SensorZoneID.ToString();
                else
                    strSensorZoneIDs += ", " + pair.Value.SensorZoneID.ToString();
            }

            if (strSensorZoneIDs.Length == 0)
                return;

            bool isNullable;
            string strAdditionalConditions = string.Format("{0} in ({1})", SensorZone.GetFieldName(SensorZone.Fields.ID, out isNullable), strSensorZoneIDs);

            string strErrorMessage;
            List<SensorZone> sensorZones = dataManager.GetSelectManager().SelectSensorZones(null, strAdditionalConditions, out strErrorMessage);

            if (sensorZones == null)
            {
                if (strErrorMessage != null)
                    System.Diagnostics.Trace.WriteLine("SetAlarmInfo Fail : " + strErrorMessage);
                return;
            }

            string strOriginSensorIDs = "";

            foreach (SensorZone sensorZone in sensorZones)
            {
                if (sensorZone.SensorType == (int)dnsData.Sensor.Facility.FacilityType.ETC)
                {
                    if (strOriginSensorIDs.Length == 0)
                        strOriginSensorIDs = sensorZone.OrgSensorID.ToString();
                    else
                        strOriginSensorIDs += "," + sensorZone.OrgSensorID.ToString();

                    dicSensorZones[sensorZone.ID] = sensorZone.OrgSensorID;
                }
            }

            strAdditionalConditions = string.Format("{0} in ({1})", ETC.GetFieldName(ETC.Fields.ID, out isNullable), strOriginSensorIDs);
            List<ETC> etcSensors = dataManager.GetSelectManager().SelectETCSensors(null, strAdditionalConditions, out strErrorMessage);

            if (etcSensors == null)
            {
                if (strErrorMessage != null)
                    System.Diagnostics.Trace.WriteLine("SetAlarmInfo Fail : " + strErrorMessage);
                return;
            }

            // Key : EtcSensor ID
            Dictionary<int, int> dicSensorMemberIDs = new Dictionary<int, int>();
            Dictionary<int, RegularMember> dicSensorMembers = new Dictionary<int, RegularMember>();
            Dictionary<int, int> dicRegularMemberIDs = new Dictionary<int, int>();
            Dictionary<int, ETC> dicETCSensors = new Dictionary<int, ETC>();

            string strRegularMemberIDs = "";

            foreach (ETC sensor in etcSensors)
            {
                dicETCSensors[sensor.ID] = sensor;

                if (sensor.Department == null)
                    continue;

                int nRegularMemberID;

                if (int.TryParse(sensor.Department, out nRegularMemberID))
                {
                    dicSensorMemberIDs[sensor.ID] = nRegularMemberID;

                    if (dicRegularMemberIDs.ContainsKey(nRegularMemberID))
                        continue;
                    else
                        dicRegularMemberIDs[nRegularMemberID] = nRegularMemberID;

                    if (strRegularMemberIDs.Length == 0)
                        strRegularMemberIDs = nRegularMemberID.ToString();
                    else
                        strRegularMemberIDs += "," + nRegularMemberID.ToString();
                }
            }

            if (strRegularMemberIDs.Length > 0)
            {
                strAdditionalConditions = string.Format("{0} in ({1})", RegularMember.GetFieldName(RegularMember.Fields.ID, out isNullable), strRegularMemberIDs);
                List<RegularMember> members = teamDataManager.GetSelectManager().SelectRegularMembers(strAdditionalConditions, out strErrorMessage);

                if (members == null)
                {
                    if (strErrorMessage != null)
                        System.Diagnostics.Trace.WriteLine("SetAlarmInfo Fail : " + strErrorMessage);
                    return;
                }

                foreach (var member in members)
                {
                    member.PhoneNumber = "";
                    member.OfficePhoneNumber = "";
                    member.Email = "";

                    foreach (KeyValuePair<int, int> pair in dicSensorMemberIDs)
                    {
                        if (pair.Value == member.ID)
                        {
                            dicSensorMembers[pair.Key] = member;
                        }
                    }
                }
            }

            foreach (KeyValuePair<int, SafetyAlarm> pair in dicAlarms)
            {
                int? originSensorID;

                if (dicSensorZones.TryGetValue(pair.Value.SensorZoneID, out originSensorID))
                {
                    if (originSensorID != null)
                    {
                        ETC sensor;

                        if (dicETCSensors.TryGetValue((int)originSensorID, out sensor))
                        {
                            RegularMember member;
                            dicSensorMembers.TryGetValue((int)originSensorID, out member);

                            if (sensor.Name.ToLower().Contains("areaalarm"))
                            {
                                pair.Value.m_strAlarmType = "위험영역 침범";

                                if (member != null)
                                {
                                    pair.Value.m_strAlarmInfo = pair.Value.ZoneName + ", " + member.MemberName;
                                    pair.Value.m_member = member;
                                }
                            }
                            else if (sensor.Name.ToLower().Contains("noequipment"))
                            {
                                pair.Value.m_strAlarmType = "안전장구 미착용";

                                if (member != null)
                                {
                                    pair.Value.m_strAlarmInfo = member.MemberName;
                                    pair.Value.m_member = member;
                                }
                            }
                        }
                    }
                }
            }
        }

        private static Dictionary<int, SafetyAlarm> SetCCTVs(List<SafetyAlarm> alarms, IDataManager dataManager)
        {
            // Key : SensorZoneHistory ID
            Dictionary<int, SafetyAlarm> dicAlarms = new Dictionary<int, SafetyAlarm>();

            string strSensorZoneHistoryIDs = "";

            foreach (SafetyAlarm alarm in alarms)
            {
                if (alarm.FacilityType == dnsData.Sensor.Facility.FacilityType.FIRE_SENSOR)
                {
                    alarm.m_strAlarmType = alarm.PositionName;
                    alarm.m_strAlarmInfo = alarm.ZoneName;
                }
                else
                {
                    dicAlarms[alarm.SensorZoneHistoryID] = alarm;

                    if (strSensorZoneHistoryIDs.Length == 0)
                        strSensorZoneHistoryIDs = alarm.SensorZoneHistoryID.ToString();
                    else
                        strSensorZoneHistoryIDs += "," + alarm.SensorZoneHistoryID.ToString();
                }
            }

            if (strSensorZoneHistoryIDs.Length == 0)
                return dicAlarms;

            Dictionary<SensorReactionHistory.Fields, object> dicConditions = new Dictionary<SensorReactionHistory.Fields, object>();
            dicConditions[SensorReactionHistory.Fields.ReactionType] = (int)SensorReactionHistory.ReactionTypes.BEGIN_STATUS;

            bool isNullable;
            string strAdditionalConditions = string.Format("{0} in ({1})", SensorReactionHistory.GetFieldName(SensorReactionHistory.Fields.SensorZoneHistoryID, out isNullable), strSensorZoneHistoryIDs);

            string strErrorMessage;
            List<SensorReactionHistory> reactionHistories = dataManager.GetSelectManager().SelectSensorReactionHistories(dicConditions, strAdditionalConditions, out strErrorMessage);

            if (reactionHistories == null)
            {
                if (strErrorMessage != null)
                    System.Diagnostics.Trace.WriteLine("SetAlarmInfo Fail : " + strErrorMessage);

                return null;
            }

            string strUniqueKeys = "";
            Dictionary<int, SensorReactionHistory> dicSensorReactionHistories = new Dictionary<int, SensorReactionHistory>();

            foreach (SensorReactionHistory history in reactionHistories)
            {
                if (history.Param4 == null || history.Param4.Length == 0)
                    continue;

                if (strUniqueKeys.Length == 0)
                    strUniqueKeys = "'" + history.Param4 + "'";
                else
                    strUniqueKeys += ", '" + history.Param4 + "'";

                dicSensorReactionHistories[history.SensorZoneHistoryID] = history;
            }

            if (strUniqueKeys.Length == 0)
                return dicAlarms;

            strAdditionalConditions = string.Format("{0} in ({1})", CCTV.GetFieldName(CCTV.Fields.UniqueKey, out isNullable), strUniqueKeys);
            List<CCTV> cctvs = dataManager.GetSelectManager().SelectCCTVs(null, strAdditionalConditions, out strErrorMessage);

            if (cctvs == null)
            {
                if (strErrorMessage != null)
                    System.Diagnostics.Trace.WriteLine("SetAlarmInfo Fail : " + strErrorMessage);

                return null;
            }

            Dictionary<string, CCTV> dicCCTVs = new Dictionary<string, CCTV>();

            foreach (CCTV cctv in cctvs)
            {
                dicCCTVs[cctv.UniqueKey] = cctv;
            }

            foreach (KeyValuePair<int, SafetyAlarm> pair in dicAlarms)
            {
                SensorReactionHistory history;
                
                if (dicSensorReactionHistories.TryGetValue(pair.Value.SensorZoneHistoryID, out history))
                {
                    if (history.Param4 != null && history.Param4.Length > 0)
                    {
                        CCTV cctv;

                        if (dicCCTVs.TryGetValue(history.Param4, out cctv))
                            pair.Value.m_cctv = cctv;
                    }
                }
            }

            return dicAlarms;
        }
    }
}
