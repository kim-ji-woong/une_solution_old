using SmartCity.BLL.Models.Response;
using SmartCity.IDAL;
using SmartCity.Model;
using System;
using System.Collections.Generic;

namespace SmartCity.BLL
{
    public class LoadManager
    {
        private IDataManager m_dataManager = null;
        private static bool isLoading = false;

        public LoadManager(IDataManager dataManager)
        {
            m_dataManager = dataManager;

            if (!isLoading)
            {
                isLoading = true;
                InitLoads();
            }
        }

        private static Dictionary<int, FacilityType> m_dicFacilityType = null;
        public Dictionary<int, FacilityType> FacilityTypes
        {
            get { return m_dicFacilityType; }
        }

        private static Dictionary<int, FireSensor> m_dicFireSensors = null;
        public Dictionary<int, FireSensor> FireSensors
        {
            get { return m_dicFireSensors; }
        }

        private static Dictionary<int, FloodSensor> m_dicFloodSensors = null;
        public Dictionary<int, FloodSensor> FloodSensors
        {
            get { return m_dicFloodSensors; }
        }

        private static Dictionary<int, HeatSensor> m_dicHeatSensors = null;
        public Dictionary<int, HeatSensor> HeatSensors
        {
            get { return m_dicHeatSensors; }
        }

        private static Dictionary<int, CollapseSensor> m_dicCollapseSensors = null;
        public Dictionary<int, CollapseSensor> CollapseSensors
        {
            get { return m_dicCollapseSensors; }
        }

        private static Dictionary<int, AlertAlarm> m_dicAlertAlarms = null;
        public Dictionary<int, AlertAlarm> AlertAlarms
        {
            get { return m_dicAlertAlarms; }
        }

        private static Dictionary<int, FacilityManual> m_dicFacilityManuals = null;
        public Dictionary<int, FacilityManual> FacilityManuals
        {
            get { return m_dicFacilityManuals; }
        }

        private static Dictionary<int, JobLevel> m_dicJobLevels = null;
        public Dictionary<int, JobLevel> JobLevels
        {
            get { return m_dicJobLevels; }
        }

        private void InitLoads()
        {
            m_dicFacilityType = new Dictionary<int, FacilityType>();
            m_dicFireSensors = new Dictionary<int, FireSensor>();
            m_dicFloodSensors = new Dictionary<int, FloodSensor>();
            m_dicHeatSensors = new Dictionary<int, HeatSensor>();
            m_dicCollapseSensors = new Dictionary<int, CollapseSensor>();
            m_dicAlertAlarms = new Dictionary<int, AlertAlarm>();
            m_dicFacilityManuals = new Dictionary<int, FacilityManual>();
            m_dicJobLevels = new Dictionary<int, JobLevel>();

            LoadFacilityType(m_dicFacilityType);
            LoadFireSensors(m_dicFireSensors);
            LoadFloodSensors(m_dicFloodSensors);
            LoadHeatSensors(m_dicHeatSensors);
            LoadCollapseSensors(m_dicCollapseSensors);
            LoadAlertAlarms(m_dicAlertAlarms);
            LoadFacilityManuals(m_dicFacilityManuals);
            LoadJobLevels(m_dicJobLevels);
        }

        private bool LoadFacilityType(Dictionary<int, FacilityType> dicFacilityType)
        {
            dicFacilityType.Clear();

            string strErrorMessage = null;
            string strAdditionalConditions = "";
            Dictionary<FacilityType.Fields, object> dicConditions = new Dictionary<FacilityType.Fields, object>();

            List<FacilityType> FacilityTypes = m_dataManager.GetSelectManager().SelectFacilityTypes(dicConditions, strAdditionalConditions, out strErrorMessage);

            if (FacilityTypes == null)
                return false;

            foreach (FacilityType type in FacilityTypes)
            {
                dicFacilityType[type.ID] = type;
            }

            return true;
        }

        private bool LoadFireSensors(Dictionary<int, FireSensor> dicFireSensor)
        {
            dicFireSensor.Clear();

            string strErrorMessage = null;
            string strAdditionalConditions = "";
            Dictionary<FireSensor.Fields, object> dicConditions = new Dictionary<FireSensor.Fields, object>();

            List<FireSensor> FireSensors = m_dataManager.GetSelectManager().SelectFireSensors(dicConditions, strAdditionalConditions, out strErrorMessage);

            if (FireSensors == null)
                return false;

            foreach (FireSensor sensor in FireSensors)
            {
                dicFireSensor[sensor.ID] = sensor;
            }

            return true;
        }

        private bool LoadFloodSensors(Dictionary<int, FloodSensor> dicFloodSensor)
        {
            dicFloodSensor.Clear();

            string strErrorMessage = null;
            string strAdditionalConditions = "";
            Dictionary<FloodSensor.Fields, object> dicConditions = new Dictionary<FloodSensor.Fields, object>();

            List<FloodSensor> FloodSensors = m_dataManager.GetSelectManager().SelectFloodSensors(dicConditions, strAdditionalConditions, out strErrorMessage);

            if (FloodSensors == null)
                return false;

            foreach (FloodSensor sensor in FloodSensors)
            {
                dicFloodSensor[sensor.ID] = sensor;
            }

            return true;
        }

        private bool LoadHeatSensors(Dictionary<int, HeatSensor> dicHeatSensor)
        {
            dicHeatSensor.Clear();

            string strErrorMessage = null;
            string strAdditionalConditions = "";
            Dictionary<HeatSensor.Fields, object> dicConditions = new Dictionary<HeatSensor.Fields, object>();

            List<HeatSensor> HeatSensors = m_dataManager.GetSelectManager().SelectHeatSensors(dicConditions, strAdditionalConditions, out strErrorMessage);

            if (HeatSensors == null)
                return false;

            foreach (HeatSensor sensor in HeatSensors)
            {
                dicHeatSensor[sensor.ID] = sensor;
            }

            return true;
        }

        private bool LoadCollapseSensors(Dictionary<int, CollapseSensor> dicCollapseSensor)
        {
            dicCollapseSensor.Clear();

            string strErrorMessage = null;
            string strAdditionalConditions = "";
            Dictionary<CollapseSensor.Fields, object> dicConditions = new Dictionary<CollapseSensor.Fields, object>();

            List<CollapseSensor> CollapseSensors = m_dataManager.GetSelectManager().SelectCollapseSensors(dicConditions, strAdditionalConditions, out strErrorMessage);

            if (CollapseSensors == null)
                return false;

            foreach (CollapseSensor sensor in CollapseSensors)
            {
                dicCollapseSensor[sensor.ID] = sensor;
            }

            return true;
        }

        private bool LoadAlertAlarms(Dictionary<int, AlertAlarm> dicAlertAlarms)
        {
            dicAlertAlarms.Clear();

            string strErrorMessage = null;
            string strAdditionalConditions = "";
            Dictionary<AlertAlarm.Fields, object> dicConditions = new Dictionary<AlertAlarm.Fields, object>();

            List<AlertAlarm> AlertAlarms = m_dataManager.GetSelectManager().SelectAlertAlarms(dicConditions, strAdditionalConditions, out strErrorMessage);

            if (AlertAlarms == null)
                return false;

            foreach (AlertAlarm alarm in AlertAlarms)
            {
                dicAlertAlarms[alarm.ID] = alarm;
            }

            return true;
        }

        private bool LoadFacilityManuals(Dictionary<int, FacilityManual> dicFacilityManuals)
        {
            dicFacilityManuals.Clear();

            string strErrorMessage = null;
            string strAdditionalConditions = "";
            Dictionary<FacilityManual.Fields, object> dicConditions = new Dictionary<FacilityManual.Fields, object>();

            List<FacilityManual> FacilityManuals = m_dataManager.GetSelectManager().SelectFacilityManuals(dicConditions, strAdditionalConditions, out strErrorMessage);

            if (FacilityManuals == null)
                return false;

            foreach (FacilityManual manual in FacilityManuals)
            {
                dicFacilityManuals[manual.ID] = manual;
            }

            return true;
        }

        private bool LoadJobLevels(Dictionary<int, JobLevel> dicJobLevels)
        {
            dicJobLevels.Clear();

            string strErrorMessage = null;
            string strAdditionalConditions = "";
            Dictionary<JobLevel.Fields, object> dicConditions = new Dictionary<JobLevel.Fields, object>();

            List<JobLevel> JobLevels = m_dataManager.GetSelectManager().SelectJobLevels(dicConditions, strAdditionalConditions, out strErrorMessage);

            if (JobLevels == null)
                return false;

            foreach (JobLevel jobLevel in JobLevels)
            {
                dicJobLevels[jobLevel.ID] = jobLevel;
            }

            return true;
        }

        public ResponseSensorInfo GetFirstSensor(int nFacilityType)
        {
            ResponseSensorInfo response = new ResponseSensorInfo();

            if (nFacilityType == (int)SmartCity.Data.FacilityType.FIRE_SENSOR)
            {
                foreach (KeyValuePair<int, FireSensor> pair in m_dicFireSensors)
                {
                    FireSensor fire = pair.Value;
                    Sensor sensor = new Sensor();

                    sensor.ID = fire.ID;
                    sensor.SensorID = fire.SensorID;
                    sensor.State = fire.State;
                    sensor.Addr = fire.Addr;

                    response.Sensor = sensor;
                    break;
                }
            }
            else if (nFacilityType == (int)SmartCity.Data.FacilityType.FLOOD_SENSOR)
            {
                foreach (KeyValuePair<int, FloodSensor> pair in m_dicFloodSensors)
                {
                    FloodSensor flood = pair.Value;
                    Sensor sensor = new Sensor();

                    sensor.ID = flood.ID;
                    sensor.SensorID = flood.SensorID;
                    sensor.State = flood.State;
                    sensor.Addr = flood.Addr;

                    response.Sensor = sensor;
                    break;
                }
            }
            else if (nFacilityType == (int)SmartCity.Data.FacilityType.HEAT_SENSOR)
            {
                foreach (KeyValuePair<int, HeatSensor> pair in m_dicHeatSensors)
                {
                    HeatSensor heat = pair.Value;
                    Sensor sensor = new Sensor();

                    sensor.ID = heat.ID;
                    sensor.SensorID = heat.SensorID;
                    sensor.State = heat.State;
                    sensor.Addr = heat.Addr;

                    response.Sensor = sensor;
                    break;
                }
            }
            else if (nFacilityType == (int)SmartCity.Data.FacilityType.COLLAPSE_SENSOR)
            {
                foreach (KeyValuePair<int, CollapseSensor> pair in m_dicCollapseSensors)
                {
                    CollapseSensor collapse = pair.Value;
                    Sensor sensor = new Sensor();

                    sensor.ID = collapse.ID;
                    sensor.SensorID = collapse.SensorID;
                    sensor.State = collapse.State;
                    sensor.Addr = collapse.Addr;

                    response.Sensor = sensor;
                    break;
                }
            }

            if (response.Sensor != null)
                response.Success = true;
            else
            {
                response.Success = false;
                response.Message = "해당 센서가 없습니다.";
            }
                
            return response;
        }

        public ResponseSensorInfo GetSensorInfo(int nID, int nFacilityType)
        {
            ResponseSensorInfo response = new ResponseSensorInfo();
            string strErrorMessage = "";

            if (nFacilityType == (int)SmartCity.Data.FacilityType.FIRE_SENSOR)
            {
                if (m_dicFireSensors.ContainsKey(nID))
                {
                    FireSensor fire = m_dataManager.GetSelectManager().SelectFireSensor(nID, out strErrorMessage);

                    Sensor sensor = new Sensor();
                    sensor.ID = fire.ID;
                    sensor.SensorID = fire.SensorID;
                    sensor.State = fire.State;
                    sensor.Addr = fire.Addr;

                    response.Sensor = sensor;
                }
            }
            else if (nFacilityType == (int)SmartCity.Data.FacilityType.FLOOD_SENSOR)
            {
                if (m_dicFloodSensors.ContainsKey(nID))
                {
                    FloodSensor flood = m_dataManager.GetSelectManager().SelectFloodSensor(nID, out strErrorMessage);

                    Sensor sensor = new Sensor();
                    sensor.ID = flood.ID;
                    sensor.SensorID = flood.SensorID;
                    sensor.State = flood.State;
                    sensor.Addr = flood.Addr;

                    response.Sensor = sensor;
                }
            }
            else if (nFacilityType == (int)SmartCity.Data.FacilityType.HEAT_SENSOR)
            {
                if (m_dicHeatSensors.ContainsKey(nID))
                {
                    HeatSensor heat = m_dataManager.GetSelectManager().SelectHeatSensor(nID, out strErrorMessage);

                    Sensor sensor = new Sensor();
                    sensor.ID = heat.ID;
                    sensor.SensorID = heat.SensorID;
                    sensor.State = heat.State;
                    sensor.Addr = heat.Addr;

                    response.Sensor = sensor;
                }
            }
            else if (nFacilityType == (int)SmartCity.Data.FacilityType.COLLAPSE_SENSOR)
            {
                if (m_dicCollapseSensors.ContainsKey(nID))
                {
                    CollapseSensor collapse = m_dataManager.GetSelectManager().SelectCollapseSensor(nID, out strErrorMessage);

                    Sensor sensor = new Sensor();
                    sensor.ID = collapse.ID;
                    sensor.SensorID = collapse.SensorID;
                    sensor.State = collapse.State;
                    sensor.Addr = collapse.Addr;

                    response.Sensor = sensor;
                }
            }

            if (response.Sensor != null)
                response.Success = true;
            else
            {
                response.Success = false;
                response.Message = "해당 센서가 없습니다.";
            }

            return response;
        }

        public ResponseAlarmList GetAlarmList(int nFacilityType)
        {
            ResponseAlarmList result = new ResponseAlarmList();

            string strErrorMessage = null;
            string strAdditionalConditions = "";
            Dictionary<AlertAlarm.Fields, object> dicConditions = new Dictionary<AlertAlarm.Fields, object>();
            dicConditions[AlertAlarm.Fields.FacilityType] = nFacilityType;

            List<AlertAlarm> AlertAlarms = m_dataManager.GetSelectManager().SelectAlertAlarms(dicConditions, strAdditionalConditions, out strErrorMessage);

            if (AlertAlarms == null)
            {
                result.Message = strErrorMessage;
                result.Success = false;
                return result;
            }

            result.Alarms = AlertAlarms;
            result.Success = true;

            return result;
        }

        public ResponseManualList GetManualList(int nFacilityType)
        {
            ResponseManualList result = new ResponseManualList();

            string strErrorMessage = null;
            string strAdditionalConditions = "";
            Dictionary<FacilityManual.Fields, object> dicConditions = new Dictionary<FacilityManual.Fields, object>();
            dicConditions[FacilityManual.Fields.FacilityType] = nFacilityType;

            List<FacilityManual> FacilityManuals = m_dataManager.GetSelectManager().SelectFacilityManuals(dicConditions, strAdditionalConditions, out strErrorMessage);

            if (FacilityManuals == null)
            {
                result.Message = strErrorMessage;
                result.Success = false;
                return result;
            }

            List<Manual> manuals = new List<Manual>();

            // TODO: 메뉴얼 조합하기!!
            foreach (FacilityManual manual in FacilityManuals)
            {
                Manual manualData = new Manual();
                List<Member> members = new List<Member>();

                if (manual.ManualMembers != null && manual.ManualMembers != "")
                {
                    string strMembers = manual.ManualMembers;

                    List<int> datas = new List<int>();
                    int data;
                    string[] tokens = strMembers.Split(',');

                    foreach (string strToken in tokens)
                    {
                        if (int.TryParse(strToken.Trim(), out data))
                            datas.Add(data);
                    }

                    foreach(int nMemberID in datas)
                    {
                        Member memberData = new Member();

                        CompanyMember member = m_dataManager.GetSelectManager().SelectCompanyMember(nMemberID, out strErrorMessage);
                        if (member == null)
                            continue;

                        if (member.LevelID != -1)
                        {
                            JobLevel job = m_dataManager.GetSelectManager().SelectJobLevel(member.LevelID, out strErrorMessage);
                            if (job == null)
                            {
                                result.Message = strErrorMessage;
                                result.Success = false;
                                return result;
                            }

                            memberData.JobLevel = job;
                        }

                        if (member.RegularTeamID != -1)
                        {
                            RegularTeam regular = m_dataManager.GetSelectManager().SelectRegularTeam(member.RegularTeamID, out strErrorMessage);
                            if (regular == null)
                            {
                                result.Message = strErrorMessage;
                                result.Success = false;
                                return result;
                            }

                            memberData.RegularTeam = regular;
                        }

                        memberData.ID = member.ID;
                        memberData.MemberName = member.MemberName;
                        memberData.PhoneNumber = member.PhoneNumber;
                        memberData.FacilityTypes = member.FacilityTypes;

                        members.Add(memberData);
                    }

                }

                manualData.Members = members;

                manualData.ID = manual.ID;
                manualData.FacilityType = manual.FacilityType;
                manualData.ManualType = manual.ManualType;
                manualData.ManualTitle = manual.ManualTitle;
                manualData.Number = manual.Number;
                manualData.ManualContent = manual.Manual;

                manuals.Add(manualData);
            }

            result.Manuals = manuals;
            result.Success = true;

            return result;
        }
    }
}
