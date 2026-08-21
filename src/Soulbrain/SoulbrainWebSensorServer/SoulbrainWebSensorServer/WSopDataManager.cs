using dnsDBUtil;
using SDMS.DAL;
using SDMS.Model.Alarm;
using SDMS.Model.History;
using SDMS.Model.Sensor;
using SDMS.Model.Spatial;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamEditor.Model.Sop.Team;

namespace SoulbrainWebSensorServer
{
    public class WSopDataManager
    {
        private DataManager m_dataManager = null;
        private TeamEditor.DAL.DataManager m_memberDataManager = null;
        //private WebDBManager m_dbManager = null;
        private LogManager m_logMgr = new LogManager();

        private static string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

        private List<string> m_listETCSensors = null;
        public List<string> ETCSensors
        {
            get { return m_listETCSensors; }
        }

        private List<string> m_listPSMSensors = null;
        public List<string> PSMSensors
        {
            get { return m_listPSMSensors; }
        }

        private int m_nRegularMemberMaxID = 0;
        public int RegularMemberMaxID
        {
            get { return m_nRegularMemberMaxID; }
            set { m_nRegularMemberMaxID = value; }
        }

        private string m_strAlarmETCUrl = "";
        public string AlarmETCUrl
        {
            get { return m_strAlarmETCUrl; }
            set { m_strAlarmETCUrl = value; }
        }

        private string m_strAlarmPSMUrl = "";
        public string AlarmPSMUrl
        {
            get { return m_strAlarmPSMUrl; }
            set { m_strAlarmPSMUrl = value; }
        }

        public WSopDataManager(DataManager dataManager, TeamEditor.DAL.DataManager memberDataManager)
        {
            //m_dbManager = dbManager;
            m_dataManager = dataManager;
            m_memberDataManager = memberDataManager;

            InitURL();

            // 기존 생성된 ETC 및 PSM 센서 불러오기
            LoadETCSensors();
            LoadPSMSensors();
        }

        private void InitURL()
        {
            string strAlarmETCUrl = ConfigurationManager.AppSettings.Get("Alarm_ETC_URL");
            if (strAlarmETCUrl == null || strAlarmETCUrl.Length == 0)
                strAlarmETCUrl = "http://192.168.254.201:44379/api/EtcSensor";

            string strAlarmPSMUrl = ConfigurationManager.AppSettings.Get("Alarm_PSM_URL");
            if (strAlarmPSMUrl == null || strAlarmETCUrl.Length == 0)
                strAlarmPSMUrl = "http://192.168.254.201:44379/api/PSMSensor";

            m_strAlarmETCUrl = strAlarmETCUrl;
            m_strAlarmPSMUrl = strAlarmPSMUrl;
        }

        private void LoadETCSensors()
        {
            m_listETCSensors = new List<string>();

            Dictionary<ETC.Fields, object> dicConditions_ETC = new Dictionary<ETC.Fields, object>();
            string strAdditionalConditions = "";
            string strErrorMessage = "";

            List<ETC> listETCSensors = m_dataManager.GetSelectManager().SelectETCSensors(dicConditions_ETC, strAdditionalConditions, out strErrorMessage);

            if (listETCSensors == null)
                return;

            foreach (ETC sensor in listETCSensors)
            {
                if (!m_listETCSensors.Contains(sensor.UniqueKey))
                    m_listETCSensors.Add(sensor.UniqueKey);
            }
        }

        private void LoadPSMSensors()
        {
            m_listPSMSensors = new List<string>();

            Dictionary<PSM.Fields, object> dicConditions_PSM = new Dictionary<PSM.Fields, object>();
            string strAdditionalConditions = "";
            string strErrorMessage = "";

            List<PSM> listPSMSensors = m_dataManager.GetSelectManager().SelectPSMSensors(dicConditions_PSM, strAdditionalConditions, out strErrorMessage);

            if (listPSMSensors == null)
                return;

            foreach (PSM sensor in listPSMSensors)
            {
                if (!m_listPSMSensors.Contains(sensor.UniqueKey))
                    m_listPSMSensors.Add(sensor.UniqueKey);
            }
        }

        // 정규조직 불러오기
        public bool LoadRegular(out List<Regular> regulars, out string strErrorMessage)
        {
            strErrorMessage = "";
            regulars = m_memberDataManager.GetSelectManager().SelectRegulars(out strErrorMessage);
            
            if (regulars == null)
                return false;

            return true;
        }

        // 정규조직 멤버 불러오기
        public bool LoadRegularMembers(out List<RegularMember> regularMembers, out string strErrorMessage)
        {
            regularMembers = null;

            strErrorMessage = "";
            regularMembers = m_memberDataManager.GetSelectManager().SelectRegularMembers(out strErrorMessage);

            if (regularMembers == null)
                return false;

            return true;
        }

        // ETC Sensor 현재 수치 값 및 상태 업데이트
        public bool UpdateETCSensor(DataDevice device)
        {
            if (device == null)
                return false;

            List<DataSensor> listSensorData = device.SensorDataList;
            if (listSensorData == null)
                return false;

            foreach (DataSensor sensor in listSensorData)
            {
                if (sensor.ModelName == CommonString.MODEL_DEBUGGING ||
                    sensor.SensorName == "" || sensor.Value == "")
                    continue;

                int nEnabled = 0;
                bool bEnabled = false;
                string strErrorMessage = null;
                string strAdditionalConditions = "";

                if (sensor.SensorStatus != CommonString.STATUS_OFFLINE)
                    nEnabled = 1;

                bEnabled = (nEnabled == 1);

                try
                {
                    if (CommonString.IsPSMSensorType(sensor.SensorName))
                    {
                        Dictionary<PSM.Fields, object> dicSets = new Dictionary<PSM.Fields, object>();
                        dicSets.Add(PSM.Fields.CurrentData, sensor.Value);
                        dicSets.Add(PSM.Fields.Status, sensor.SensorStatus);
                        dicSets.Add(PSM.Fields.Enabled, bEnabled);
                        dicSets.Add(PSM.Fields.Name, device.DeviceName);

                        // DeviceId + _ + Material (UniqueKey) 조건으로 업데이트
                        Dictionary<PSM.Fields, object> dicConditions = new Dictionary<PSM.Fields, object>();
                        //dicConditions.Add(PSM.Fields.ID, nSensorID);
                        strAdditionalConditions = string.Format("{0} = '{1}_{2}'", PSM.Fields.UniqueKey, device.DeviceId, sensor.SensorName);
                        m_dataManager.GetUpdateManager().UpdatePSMSensor(dicSets, dicConditions, strAdditionalConditions, out strErrorMessage);

                    }
                    else if (CommonString.IsETCSensorType(sensor.SensorName))
                    {
                        Dictionary<ETC.Fields, object> dicSets = new Dictionary<ETC.Fields, object>();
                        dicSets.Add(ETC.Fields.CurrentData, sensor.Value);
                        dicSets.Add(ETC.Fields.Status, sensor.SensorStatus);
                        dicSets.Add(ETC.Fields.Enabled, bEnabled);
                        dicSets.Add(ETC.Fields.Name, device.DeviceName);

                        Dictionary<ETC.Fields, object> dicConditions = new Dictionary<ETC.Fields, object>();
                        //dicConditions.Add(ETC.Fields.ID, nSensorID);
                        strAdditionalConditions = string.Format("{0} = '{1}_{2}'", ETC.Fields.UniqueKey, device.DeviceId, sensor.SensorName);
                        m_dataManager.GetUpdateManager().UpdateETCSensors(dicSets, dicConditions, strAdditionalConditions, out strErrorMessage);
                    }
                }
                catch (Exception e)
                {
                    //m_logMgr.Log_Info("UpdateETCSensor 실패(예외처리: " + e.Message + ")");
                    Logger.Instance.Write("UpdateETCSensor 실패(예외처리: " + e.Message + ")");
                    return false;
                }

                if (strErrorMessage != null)
                    Logger.Instance.Write("Sensor 수치 값 업데이트 실패 " + strErrorMessage);
                    //m_logMgr.Log_Info("Sensor 수치 값 업데이트 실패 " + strErrorMessage);
            }
            

            return true;
        }

        private FacilityType GetFacilityType(int nID, out string strErrorMessage)
        {
            FacilityType facilityType = null;

            facilityType = m_dataManager.GetSelectManager().SelectFacilityType(nID, out strErrorMessage);

            return facilityType;
        }

        private Material GetMaterialType(int nID, out string strErrorMessage)
        {
            Material materialType = null;

            materialType = m_dataManager.GetSelectManager().SelectMaterial(nID, out strErrorMessage);

            return materialType;
        }

        private SensorZone GetSensorZone(int nID, out string strErrorMessage)
        {
            SensorZone sensorZone = null;

            sensorZone = m_dataManager.GetSelectManager().SelectSensorZone(nID, out strErrorMessage);

            return sensorZone;
        }

        private TagInfo GetTagInfo(int nSensorZoneID, out string strErrorMessage)
        {
            TagInfo tagInfo = null;
            string strAdditionalConditions = "";

            Dictionary<TagInfo.Fields, object> dicConditions = new Dictionary<TagInfo.Fields, object>();
            dicConditions.Add(TagInfo.Fields.SensorZoneID, nSensorZoneID);

            List<TagInfo> tagInfos = m_dataManager.GetSelectManager().SelectSensorTagInfo(dicConditions, strAdditionalConditions, out strErrorMessage);

            if (tagInfos == null)
                return tagInfo;

            foreach (TagInfo tag in tagInfos)
            {
                tagInfo = tag;
            }

            return tagInfo;
        }

        private SensorZoneHistory GetSensorZoneHistory(int nID, out string strErrorMessage)
        {
            SensorZoneHistory zoneHistory = null;
            Dictionary<SensorZoneHistory.Fields, object> dicConditions = new Dictionary<SensorZoneHistory.Fields, object>();
            dicConditions[SensorZoneHistory.Fields.ID] = nID;
            string strAdditionalConditions = "DetectionStatus != 3";

            // .TODO: 테스트 알람인지 실제 알람인지 조건 필요.
            //zoneHistory = m_dataManager.GetSelectManager().SelectSensorZoneHistory(nID, out strErrorMessage);
            List<SensorZoneHistory> sensorZoneHistorys = m_dataManager.GetSelectManager().SelectSensorZoneHistories(dicConditions, strAdditionalConditions, out strErrorMessage);

            if (sensorZoneHistorys == null)
            {

            } 
            else if (sensorZoneHistorys.Count == 0)
            {
                strErrorMessage = "테스트 신호를 제외한 해당 SensorZoneHistory 조회되지 않음"; 
            }
            else if (sensorZoneHistorys.Count > 0)
            {
                zoneHistory = sensorZoneHistorys[0];
            }

            return zoneHistory;
        }

        public AlarmData GetAlarmData(DataDevice device, DataSensor sensor)
        {
            AlarmData alarm = null;
            string strUrl = "";
            string strErrorMessage = "";

            SensorZone sensorZone = null;
            TagInfo tagInfo = null;
            int nFacilityTypeID = -1;

            try
            {
                // 타입 ID 구하기
                //type = GetFacilityType(sensor.SensorName, out strErrorMessage);
                ArrayList arrResult = null;

                if (CommonString.IsPSMSensorType(sensor.SensorName))
                {
                    nFacilityTypeID = (int)dnsData.Sensor.Facility.FacilityType.PSM_SENSOR;
                    strUrl = m_strAlarmPSMUrl;
                    string strAdditionalConditions = string.Format("{0}.{1} = '{2}_{3}'", PSM.TableName, PSM.Fields.UniqueKey, device.DeviceId, sensor.SensorName);
                    arrResult = m_dataManager.GetSelectManager().JoinSensorZoneTagInfoPSMMaterial(strAdditionalConditions, out strErrorMessage);
                }
                else if (CommonString.IsETCSensorType(sensor.SensorName))
                {
                    nFacilityTypeID = (int)dnsData.Sensor.Facility.FacilityType.ETC;
                    strUrl = m_strAlarmETCUrl;
                    string strAdditionalConditions = string.Format("{0}.{1} = '{2}_{3}'", ETC.TableName, ETC.Fields.UniqueKey, device.DeviceId, sensor.SensorName);
                    arrResult = m_dataManager.GetSelectManager().JoinSensorZoneTagInfoETCMaterial(strAdditionalConditions, out strErrorMessage);
                }
                else
                {
                    //m_logMgr.Log_Info("GetAlarmData 조회 실패 (" + sensor.SensorName + " 타입을 찾을 수 없습니다.)");
                    Logger.Instance.Write("GetAlarmData 조회 실패 (" + sensor.SensorName + " 타입을 찾을 수 없습니다.)");
                    return alarm;
                }

                if (arrResult == null)
                {
                    //m_logMgr.Log_Info("GetAlarmData 조회 실패 (JoinSensorZoneTagInfoPSMMaterial 실패) " + strErrorMessage);
                    Logger.Instance.Write("GetAlarmData 조회 실패 (JoinSensorZoneTagInfoPSMMaterial 실패) " + strErrorMessage);
                    return alarm;
                }
                else if (arrResult.Count == 0)
                {
                    //m_logMgr.Log_Info("GetAlarmData 조회 실패 (JoinSensorZoneTagInfoPSMMaterial 실패) 해당 값이 없습니다.");
                    Logger.Instance.Write("GetAlarmData 조회 실패 (JoinSensorZoneTagInfoPSMMaterial 실패) 해당 값이 없습니다.");
                    return alarm;
                }

                sensorZone = arrResult[0] as SensorZone;
                tagInfo = arrResult[1] as TagInfo;
                Material mt = arrResult[2] as Material;

            }
            catch (Exception e)
            {
                //m_logMgr.Log_Info("GetAlarmData 조회 실패 " + e.Message);
                Logger.Instance.Write("GetAlarmData 조회 실패 " + e.Message);
                return alarm;
            }
                
            alarm = new AlarmData();
            alarm.DeviceID = device.DeviceId;
            alarm.DeviceName = device.DeviceName;
            alarm.SensorID = sensor.SensorId;
            alarm.SensorZoneID = sensorZone.ID;
            alarm.SensorType = nFacilityTypeID;
            alarm.SensorTagID = tagInfo.ID;
            alarm.URL = strUrl;

            return alarm;
        }

        // 알람 리스트 조회
        public List<AlarmData> GetAlarmList()
        {
            string strErrorMessage = null;
            List<CurrentAlarm> currentAlarms = null;
            List<AlarmData> alarms = new List<AlarmData>();

            try
            {
                string strAdditionalConditions = "SensorType in (" + (int)dnsData.Sensor.Facility.FacilityType.PSM_SENSOR + "," + (int)dnsData.Sensor.Facility.FacilityType.ETC + ")";

                Dictionary<CurrentAlarm.Fields, object> dicConditions = new Dictionary<CurrentAlarm.Fields, object>();
                currentAlarms = m_dataManager.GetSelectManager().SelectCurrentAlarms(dicConditions, strAdditionalConditions, out strErrorMessage);

                if (currentAlarms == null)
                {
                    //m_logMgr.Log_Info("GetAlarmList 조회 실패(CurrentAlarm 조회 실패) " + strErrorMessage);
                    Logger.Instance.Write("GetAlarmList 조회 실패(CurrentAlarm 조회 실패) " + strErrorMessage);
                    return null;
                }

                foreach (CurrentAlarm current in currentAlarms)
                {
                    SensorZoneHistory zoneHistory = GetSensorZoneHistory(current.SensorZoneHistoryID, out strErrorMessage);
                    if (zoneHistory == null)
                    {
                        //m_logMgr.Log_Info("GetAlarmList 조회 실패(SensorZoneHistory 조회 실패) " + strErrorMessage);
                        Logger.Instance.Write("GetAlarmList 조회 실패(SensorZoneHistory 조회 실패) " + strErrorMessage);
                        return null;
                    }

                    SensorZone sensorZone = GetSensorZone(zoneHistory.SensorZoneID, out strErrorMessage);
                    if (sensorZone == null)
                    {
                        //m_logMgr.Log_Info("GetAlarmList 조회 실패(SensorZone 조회 실패) " + strErrorMessage);
                        Logger.Instance.Write("GetAlarmList 조회 실패(SensorZone 조회 실패) " + strErrorMessage);
                        return null;
                    }

                    FacilityType facilityType = GetFacilityType(current.SensorType, out strErrorMessage);
                    if (facilityType == null)
                    {
                        //m_logMgr.Log_Info("GetAlarmList 조회 실패(FacilityType 조회 실패) " + strErrorMessage);
                        Logger.Instance.Write("GetAlarmList 조회 실패(FacilityType 조회 실패) " + strErrorMessage);
                        return null;
                    }

                    if (sensorZone.OrgSensorID == null)
                        continue;

                    string strDeviceName = "";
                    string strDeviceID = "";
                    Material material = null;

                    if (dnsData.Sensor.Facility.IsPSMSensorType((dnsData.Sensor.Facility.FacilityType)facilityType.ID))
                    {
                        PSM psmSensor = m_dataManager.GetSelectManager().SelectPSMSensor((int)sensorZone.OrgSensorID, out strErrorMessage);

                        if (psmSensor == null)
                        {
                            //m_logMgr.Log_Info("SelectPSMSensor 조회 실패(psmSensor 조회 실패) " + strErrorMessage);
                            Logger.Instance.Write("SelectPSMSensor 조회 실패(psmSensor 조회 실패) " + strErrorMessage);
                            return null;
                        }

                        strDeviceName = psmSensor.Name;
                        strDeviceID = psmSensor.UniqueKey;

                        if (psmSensor.MaterialType != null)
                            material = GetMaterialType((int)psmSensor.MaterialType, out strErrorMessage);
                        else
                        {
                            Console.WriteLine(psmSensor.UniqueKey);
                            Logger.Instance.Write("PSM Sensor의 Material 타입이 존재하지 않습니다.");
                        }
                    }
                    else
                    {
                        ETC etcSensor = m_dataManager.GetSelectManager().SelectETCSensor((int)sensorZone.OrgSensorID, out strErrorMessage);

                        if (etcSensor == null)
                        {
                            //m_logMgr.Log_Info("SelectETCSensor 조회 실패(etcSensor 조회 실패) " + strErrorMessage);
                            Logger.Instance.Write("SelectETCSensor 조회 실패(etcSensor 조회 실패) " + strErrorMessage);
                            return null;
                        }

                        strDeviceName = etcSensor.Name;
                        strDeviceID = etcSensor.UniqueKey;

                        if (etcSensor.MaterialType != null)
                            material = GetMaterialType((int)etcSensor.MaterialType, out strErrorMessage);
                        else
                        {
                            Console.WriteLine(etcSensor.UniqueKey);
                            Logger.Instance.Write("ETC Sensor의 Material 타입이 존재하지 않습니다.");
                        }
                    }

                    AlarmData alarm = new AlarmData();
                    //alarm.DeviceName = equipment.ZoneName;
                    alarm.DeviceName = strDeviceName;
                    alarm.DeviceID = strDeviceID;
                    alarm.SensorType = facilityType.ID;

                    int? nOrgSensorID = sensorZone.OrgSensorID;
                    if (nOrgSensorID == null)
                        nOrgSensorID = -1;

                    alarm.OrgSensorID = (int)nOrgSensorID;

                    //alarm.SensorName = facilityType.Description;
                    if (material != null)
                        alarm.SensorName = material.MaterialName;

                    alarms.Add(alarm);
                }
            }
            catch (Exception e)
            {
                //m_logMgr.Log_Info("GetAlarmList 실패(예외: " + e.Message + ")");
                Logger.Instance.Write("GetAlarmList 실패(예외: " + e.Message + ")");
                return null;
            }

            return alarms;
        }

        

        public static string EncryptString(string str)
        {
            return AES256Cipher.AES_encrypt(str, key);
        }

        public static string DecryptString(string str)
        {
            if (str == null)
                return null;

            return AES256Cipher.AES_decrypt(str, key);
        }
    }
}
