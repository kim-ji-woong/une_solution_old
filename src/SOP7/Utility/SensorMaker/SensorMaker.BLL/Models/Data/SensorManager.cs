using SDMS.Model.Sensor;
using SDMS.IDAL;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Concurrent;

namespace SensorMaker.BLL.Models.Data
{
    using Models.Response;
    using Models.Data.Sensor;
    using SDMS.Model.CCTV;
    using SDMS.Model.Spatial;

    public class SensorManager
    {
        private const string FireSensorType = "fire";
        private const string PSMSensorType = "psm";
        private const string EtcSensorType = "etc";
        private const string CCTVType = "cctv";

        // 전체 화재센서
        private Dictionary<int, FireSensor> m_dicFireSensors = new Dictionary<int, FireSensor>();
        // 사용하지 않는 화재센서
        private ConcurrentDictionary<int, Fire> m_dicDisabledFireSensors = new ConcurrentDictionary<int, Fire>();

        // 전체 화재센서
        public ICollection<FireSensor> FireSensors
        {
            get { return m_dicFireSensors.Values; }
        }

        // 사용하지 않는 화재센서
        public ICollection<Fire> DisabledFireSensors
        {
            get { return m_dicDisabledFireSensors.Values; }
        }

        // 전체 누출센서
        private Dictionary<int, PSMSensor> m_dicPSMSensors = new Dictionary<int, PSMSensor>();
        // 사용하지 않는 누출센서
        private ConcurrentDictionary<int, PSM> m_dicDisabledPSMSensors = new ConcurrentDictionary<int, PSM>();

        // 전체 누출센서
        public ICollection<PSMSensor> PSMSensors
        {
            get { return m_dicPSMSensors.Values; }
        }

        // 사용하지 않는 누출센서
        public ICollection<PSM> DisabledPSMSensors
        {
            get { return m_dicDisabledPSMSensors.Values; }
        }

        // 전체 기타센서
        private Dictionary<int, EtcSensor> m_dicEtcSensors = new Dictionary<int, EtcSensor>();
        // 사용하지 않는 기타센서
        private ConcurrentDictionary<int, ETC> m_dicDisabledEtcSensors = new ConcurrentDictionary<int, ETC>();

        // 전체 기타센서
        public ICollection<EtcSensor> EtcSensors
        {
            get { return m_dicEtcSensors.Values; }
        }

        // 사용하지 않는 기타센서
        public ICollection<ETC> DisabledEtcSensors
        {
            get { return m_dicDisabledEtcSensors.Values; }
        }

        // 전체 CCTV
        private Dictionary<int, CCTVSensor> m_dicCCTVs = new Dictionary<int, CCTVSensor>();
        // 사용하지 않는 CCTV
        private ConcurrentDictionary<int, CCTV> m_dicDisabledCCTVs = new ConcurrentDictionary<int, CCTV>();

        // 전체 CCTV
        public ICollection<CCTVSensor> CCTVs
        {
            get { return m_dicCCTVs.Values; }
        }

        // 사용하지 않는 CCTV
        public ICollection<CCTV> DisabledCCTVs
        {
            get { return m_dicDisabledCCTVs.Values; }
        }

        private Dictionary<int, SensorZone> m_dicSensorZones = new Dictionary<int, SensorZone>();
        // 전체 SensorTagInfo
        private Dictionary<int, TagInfo> m_dicSensorTagInfos = new Dictionary<int, TagInfo>();
        // 센서 타입별 SensorTagInfo(m_dicSensorTagInfos와 개수는 동일함)
        // Key : 상위 4바이트(센서타입)
        //       하위 4바이트(Origin Sensor ID)
        private Dictionary<long, TagInfo> m_dicTypeSensorTagInfos = new Dictionary<long, TagInfo>();
        // 사용하지 않는 SensorTagInfo
        private ConcurrentDictionary<long, TagInfo> m_dicDisabledTypeSensorTagInfos = new ConcurrentDictionary<long, TagInfo>();

        public bool LoadSensorList(IDataManager dataManager, SpatialManager spatialManager)
        {
            m_dicFireSensors.Clear();
            m_dicPSMSensors.Clear();
            m_dicEtcSensors.Clear();
            m_dicSensorZones.Clear();
            m_dicCCTVs.Clear();

            bool success1 = LoadSensorTagInfo(dataManager);
            bool success2 = LoadFireSensors(dataManager, spatialManager);
            bool success3 = LoadPSMSensors(dataManager, spatialManager);
            bool success4 = LoadEtcSensors(dataManager, spatialManager);
            bool success5 = LoadCCTVs(dataManager, spatialManager);

            return success1 && success2 && success3 && success4 && success5;
        }

        private bool LoadSensorTagInfo(IDataManager dataManager)
        {
            string strErrorMessage;
            ArrayList arrDatas = dataManager.GetSelectManager().JoinSensorZoneTagInfo(null, null, null, out strErrorMessage);

            if (arrDatas == null)
            {
                System.Diagnostics.Trace.WriteLine("LoadSensorTagInfo Error : " + strErrorMessage);
                return false;
            }

            int nDataCount = arrDatas.Count;

            for (int i = 0; i < nDataCount - 1; i += 2)
            {
                if (arrDatas[i] is SensorZone && arrDatas[i + 1] is TagInfo)
                {
                    SensorZone sensorZone = (SensorZone)arrDatas[i];
                    TagInfo tagInfo = (TagInfo)arrDatas[i + 1];

                    m_dicSensorTagInfos[tagInfo.ID] = tagInfo;
                    m_dicSensorZones[sensorZone.ID] = sensorZone;

                    long key = GetSensorTypeKey(sensorZone.SensorType, (int)sensorZone.OrgSensorID);
                    m_dicTypeSensorTagInfos[key] = tagInfo;

                    if (tagInfo.IsActivate == false)
                    {
                        m_dicDisabledTypeSensorTagInfos[key] = tagInfo;
                    }
                }
            }

            return true;
        }

        private long GetSensorTypeKey(int nSensorType, int nSensorID)
        {
            return ((((long)nSensorType) << 32) | (long)nSensorID);
        }

        private int GetSensorType(long key, out int nSensorID)
        {
            nSensorID = (int)(key & 0xffffffff);
            return (int)(key >> 32);
        }

        private SensorZone GetEquipZoneID(int nFacilityType, int nOrgSensorID)
        {
            foreach (KeyValuePair<int, SensorZone> item in m_dicSensorZones)
            {
                SensorZone sz = item.Value;
                if (sz.SensorType == nFacilityType && sz.OrgSensorID == nOrgSensorID)
                    return sz;
            }

            return null;
        }

        private bool LoadFireSensors(IDataManager dataManager, SpatialManager spatialManager)
        {
            string strErrorMessage;
            List<Fire> fireSensors = dataManager.GetSelectManager().SelectFireSensors(null, null, out strErrorMessage);

            if (fireSensors == null)
                return false;

            foreach (Fire fireSensor in fireSensors)
            {
                Zone zone = spatialManager.GetZone(fireSensor.ZoneID);
                
                FireSensor fire = new FireSensor(fireSensor);

                if (zone != null && zone.BuildingID != null)
                    fire.IsIndoor = true;
                else
                    fire.IsIndoor = false;
                
                m_dicFireSensors[fireSensor.ID] = fire;

                long key = GetSensorTypeKey((int)dnsData.Sensor.Facility.FacilityType.FIRE_SENSOR, fireSensor.ID);

                if (m_dicDisabledTypeSensorTagInfos.ContainsKey(key) || (fire.Enabled != null && fire.Enabled == false))
                {
                    m_dicDisabledFireSensors[fireSensor.ID] = fire;
                }
            }

            Dictionary<SensorZone.Fields, object> dicConditions = new Dictionary<SensorZone.Fields, object>();
            dicConditions[SensorZone.Fields.SensorType] = (int)dnsData.Sensor.Facility.FacilityType.FIRE_SENSOR;

            ArrayList arrDatas = dataManager.GetSelectManager().JoinSensorZoneTagInfo(dicConditions, null, null, out strErrorMessage);

            if (arrDatas == null)
                return false;

            int nDataCount = arrDatas.Count;

            for (int i=0;i<nDataCount-1;i+=2)
            {
                if (arrDatas[i] is SensorZone && arrDatas[i + 1] is TagInfo)
                {
                    SensorZone sensorZone = (SensorZone)arrDatas[i];
                    TagInfo tagInfo = (TagInfo)arrDatas[i + 1];

                    FireSensor fire;
                    
                    if (m_dicFireSensors.TryGetValue((int)sensorZone.OrgSensorID, out fire))
                    {
                        fire.SensorTagInfoID = tagInfo.ID;
                        fire.SensorZoneID = sensorZone.ID;
                        fire.TagNo = tagInfo.TagNo;
                        fire.EquipZoneID = sensorZone.EquipZoneID;
                    }
                }
            }

            return true;
        }

        private bool ReloadFireSensors(IDataManager dataManager, int nZoneID, out List<FireSensor> fireSensors, out string strErrorMessage)
        {
            fireSensors = null;

            Dictionary<Fire.Fields, object> dicConditions = new Dictionary<Fire.Fields, object>();
            dicConditions[Fire.Fields.ZoneID] = nZoneID;

            List<Fire> sensors = dataManager.GetSelectManager().SelectFireSensors(dicConditions, null, out strErrorMessage);

            if (sensors == null)
                return false;

            FireSensor fireSensor;
            fireSensors = new List<FireSensor>();

            foreach (Fire sensor in sensors)
            {
                if (m_dicFireSensors.TryGetValue(sensor.ID, out fireSensor))
                {
                    fireSensor.Name = sensor.Name;
                    fireSensor.PositionName = sensor.PositionName;
                    fireSensor.Department = sensor.Department;
                    fireSensor.DepartmentPhoneNumber = sensor.DepartmentPhoneNumber;
                    fireSensor.Enabled = sensor.Enabled;
                    fireSensor.SensorSubType = sensor.SensorSubType;
                    fireSensor.X = sensor.X;
                    fireSensor.Y = sensor.Y;
                    fireSensor.Z = sensor.Z; 

                    fireSensors.Add(fireSensor);

                    long key = GetSensorTypeKey((int)dnsData.Sensor.Facility.FacilityType.FIRE_SENSOR, fireSensor.ID);

                    if (fireSensor.Enabled != null && fireSensor.Enabled == false)
                    {
                        m_dicDisabledFireSensors[fireSensor.ID] = fireSensor;
                    }
                    else
                    {
                        Fire temp;
                        m_dicDisabledFireSensors.TryRemove(fireSensor.ID, out temp);
                    }
                }
            }

            return true;
        }

        private bool LoadPSMSensors(IDataManager dataManager, SpatialManager spatialManager)
        {
            string strErrorMessage;
            List<PSM> psmSensors = dataManager.GetSelectManager().SelectPSMSensors(null, null, out strErrorMessage);

            if (psmSensors == null)
                return false;

            foreach (PSM psmSensor in psmSensors)
            {
                PSMSensor psmData = new PSMSensor(psmSensor);

                Zone zone = spatialManager.GetZone(psmSensor.ZoneID);
                if (zone != null && zone.BuildingID != null)
                    psmData.IsIndoor = true;
                else
                    psmData.IsIndoor = false;

                EquipmentZoneData equipZoneData = spatialManager.GetEquipmentZone(psmData.EquipZoneID);

                if (equipZoneData != null)
                {
                    psmData.LinkedZones.AddRange(equipZoneData.LinkedZoneDatas);

                    //if (equipZoneData.LinkedZoneDatas.Count > 0)
                    //{
                    //    Zone zone = equipZoneData.LinkedZoneDatas[0];
                    //    psmData.IsIndoor = zone.BuildingID != null;
                    //}
                }

                m_dicPSMSensors[psmSensor.ID] = psmData;

                long key = GetSensorTypeKey((int)dnsData.Sensor.Facility.FacilityType.PSM_SENSOR, psmSensor.ID);

                if (m_dicDisabledTypeSensorTagInfos.ContainsKey(key) || (psmSensor.Enabled != null && psmSensor.Enabled == false))
                {
                    m_dicDisabledPSMSensors[psmSensor.ID] = psmData;
                }
            }

            List<int> psmTypeIDs = GetPSMSensorTypeIDs(dataManager);

            Dictionary<SensorZone.Fields, object> dicConditions = new Dictionary<SensorZone.Fields, object>();
            //dicConditions[SensorZone.Fields.SensorType] = (int)dnsData.Sensor.Facility.FacilityType.PSM_SENSOR;
            string strAdditionalConditions = "SensorType in (" + string.Join(",", psmTypeIDs) + ")";
            //string strAdditionalConditions = "SensorType in (" + string.Join(",", dnsData.Sensor.Facility.GetPSMTypeAllNumberToList()) + ")";

            ArrayList arrDatas = dataManager.GetSelectManager().JoinSensorZoneTagInfo(dicConditions, null, strAdditionalConditions, out strErrorMessage);

            if (arrDatas == null)
                return false;

            int nDataCount = arrDatas.Count;

            for (int i = 0; i < nDataCount - 1; i += 2)
            {
                if (arrDatas[i] is SensorZone && arrDatas[i + 1] is TagInfo)
                {
                    SensorZone sensorZone = (SensorZone)arrDatas[i];
                    TagInfo tagInfo = (TagInfo)arrDatas[i + 1];

                    PSMSensor psm;

                    if (m_dicPSMSensors.TryGetValue((int)sensorZone.OrgSensorID, out psm))
                    {
                        psm.SensorTagInfoID = tagInfo.ID;
                        psm.SensorZoneID = sensorZone.ID;
                        psm.FacilityType = sensorZone.SensorType;
                        psm.EquipZoneID = sensorZone.EquipZoneID;
                    }
                }
            }

            return true;
        }

        private List<int> GetPSMSensorTypeIDs(IDataManager dataManager)
        {
            Dictionary<FacilityType.Fields, object> dicConditions = new Dictionary<FacilityType.Fields, object>();
            dicConditions[FacilityType.Fields.LinkedTableName] = PSM.TableName;

            string strErrorMessage;
            List<FacilityType> types = dataManager.GetSelectManager().SelectFacilityTypes(dicConditions, null, out strErrorMessage);

            List<int> ids = new List<int>();

            if (types == null)
            {
                System.Diagnostics.Trace.WriteLine("GetPSMSensorTypeIDs Error : " + strErrorMessage);
                return ids;
            }

            foreach (FacilityType type in types)
            {
                ids.Add(type.ID);
            }

            return ids;
        }

        private bool ReloadPSMSensors(IDataManager dataManager, int nZoneID, out List<PSMSensor> psmSensors, out string strErrorMessage)
        {
            strErrorMessage = null;
            psmSensors = null;

            Dictionary<PSM.Fields, object> dicConditions = new Dictionary<PSM.Fields, object>();
            dicConditions[PSM.Fields.ZoneID] = nZoneID;

            List<PSM> sensors = dataManager.GetSelectManager().SelectPSMSensors(dicConditions, null, out strErrorMessage);

            if (sensors == null)
                return false;

            PSMSensor psmSensor;
            psmSensors = new List<PSMSensor>();

            foreach (PSM sensor in sensors)
            {
                if (m_dicPSMSensors.TryGetValue(sensor.ID, out psmSensor))
                {
                    psmSensor.Name = sensor.Name;
                    psmSensor.PositionName = sensor.PositionName;
                    psmSensor.Department = sensor.Department;
                    psmSensor.DepartmentPhoneNumber = sensor.DepartmentPhoneNumber;
                    psmSensor.Enabled = sensor.Enabled;
                    psmSensor.Status = sensor.Status;
                    psmSensor.X = sensor.X;
                    psmSensor.Y = sensor.Y;
                    psmSensor.Z = sensor.Z;

                    psmSensors.Add(psmSensor);

                    long key = GetSensorTypeKey((int)dnsData.Sensor.Facility.FacilityType.PSM_SENSOR, psmSensor.ID);

                    if (psmSensor.Enabled != null && psmSensor.Enabled == false)
                    {
                        m_dicDisabledPSMSensors[psmSensor.ID] = psmSensor;
                    }
                    else
                    {
                        PSM temp;
                        m_dicDisabledPSMSensors.TryRemove(psmSensor.ID, out temp);
                    }
                }
            }

            return true;
        }

        private bool LoadEtcSensors(IDataManager dataManager, SpatialManager spatialManager)
        {
            string strErrorMessage;
            List<ETC> etcSensors = dataManager.GetSelectManager().SelectETCSensors(null, null, out strErrorMessage);

            if (etcSensors == null)
                return false;

            foreach (ETC etcSensor in etcSensors)
            {
                EtcSensor etc = new EtcSensor(etcSensor);

                Zone zone = spatialManager.GetZone(etcSensor.ZoneID);

                if (zone != null && zone.BuildingID != null)
                    etc.IsIndoor = true;
                else
                    etc.IsIndoor = false;

                m_dicEtcSensors[etcSensor.ID] = etc;

                long key = GetSensorTypeKey((int)dnsData.Sensor.Facility.FacilityType.ETC, etc.ID);

                if (m_dicDisabledTypeSensorTagInfos.ContainsKey(key) || (etc.Enabled != null && etc.Enabled == false))
                {
                    m_dicDisabledEtcSensors[etcSensor.ID] = etc;
                }
            }

            Dictionary<SensorZone.Fields, object> dicConditions = new Dictionary<SensorZone.Fields, object>();
            //dicConditions[SensorZone.Fields.SensorType] = (int)dnsData.Sensor.Facility.FacilityType.ETC;
            string strAdditionalConditions = "SensorType in (" + string.Join(",", dnsData.Sensor.Facility.GetETCTypeAllNumberToList()) + ")";

            ArrayList arrDatas = dataManager.GetSelectManager().JoinSensorZoneTagInfo(dicConditions, null, strAdditionalConditions, out strErrorMessage);

            if (arrDatas == null)
                return false;

            int nDataCount = arrDatas.Count;

            for (int i = 0; i < nDataCount - 1; i += 2)
            {
                if (arrDatas[i] is SensorZone && arrDatas[i + 1] is TagInfo)
                {
                    SensorZone sensorZone = (SensorZone)arrDatas[i];
                    TagInfo tagInfo = (TagInfo)arrDatas[i + 1];

                    EtcSensor etc;

                    if (m_dicEtcSensors.TryGetValue((int)sensorZone.OrgSensorID, out etc))
                    {
                        etc.SensorTagInfoID = tagInfo.ID;
                        etc.SensorZoneID = sensorZone.ID;
                        etc.FacilityType = sensorZone.SensorType;
                        etc.EquipZoneID = sensorZone.EquipZoneID;
                    }
                }
            }

            return true;
        }

        private bool ReloadEtcSensors(IDataManager dataManager, int nZoneID, out List<EtcSensor> etcSensors, out string strErrorMessage)
        {
            etcSensors = null;

            Dictionary<ETC.Fields, object> dicConditions = new Dictionary<ETC.Fields, object>();
            dicConditions[ETC.Fields.ZoneID] = nZoneID;

            List<ETC> sensors = dataManager.GetSelectManager().SelectETCSensors(dicConditions, null, out strErrorMessage);

            if (sensors == null)
                return false;

            EtcSensor etcSensor;
            etcSensors = new List<EtcSensor>();

            foreach (ETC sensor in sensors)
            {
                if (m_dicEtcSensors.TryGetValue(sensor.ID, out etcSensor))
                {
                    etcSensor.Name = sensor.Name;
                    etcSensor.PositionName = sensor.PositionName;
                    etcSensor.Department = sensor.Department;
                    etcSensor.DepartmentPhoneNumber = sensor.DepartmentPhoneNumber;
                    etcSensor.Enabled = sensor.Enabled;
                    etcSensor.MaterialType = sensor.MaterialType;
                    etcSensor.Status = sensor.Status;
                    etcSensor.X = sensor.X;
                    etcSensor.Y = sensor.Y;
                    etcSensor.Z = sensor.Z;

                    etcSensors.Add(etcSensor);

                    long key = GetSensorTypeKey((int)dnsData.Sensor.Facility.FacilityType.ETC, etcSensor.ID);

                    if (etcSensor.Enabled != null && etcSensor.Enabled == false)
                    {
                        m_dicDisabledEtcSensors[etcSensor.ID] = etcSensor;
                    }
                    else
                    {
                        ETC temp;
                        m_dicDisabledEtcSensors.TryRemove(etcSensor.ID, out temp);
                    }
                }
            }

            return true;
        }

        /*private bool LoadSensorZones(IDataManager dataManager)
        {
            string strErrorMessage;
            List<SensorZone> sensorZones = dataManager.GetSelectManager().SelectSensorZones(null, null, out strErrorMessage);

            if (sensorZones == null)
                return false;

            foreach (SensorZone sensorZone in sensorZones)
            {
                m_dicSensorZones[sensorZone.ID] = sensorZone;
            }

            return true;
        }*/

        private bool LoadCCTVs(IDataManager dataManager, SpatialManager spatialManager)
        {
            string strErrorMessage;
            List<CCTV> cctvs = dataManager.GetSelectManager().SelectCCTVs(null, null, out strErrorMessage);

            if (cctvs == null)
                return false;

            foreach (CCTV cctv in cctvs)
            {
                CCTVSensor cctvSensor = new CCTVSensor(cctv);
                m_dicCCTVs[cctv.ID] = cctvSensor;

                if (cctv.Enabled != null && cctv.Enabled == false)
                {
                    m_dicDisabledCCTVs[cctv.ID] = cctvSensor;
                }
            }

            return true;
        }

        private bool ReloadCCTVs(IDataManager dataManager, int nZoneID, out List<CCTVSensor> cctvSensors, out string strErrorMessage)
        {
            cctvSensors = null;

            Dictionary<CCTV.Fields, object> dicConditions = new Dictionary<CCTV.Fields, object>();
            dicConditions[CCTV.Fields.ZoneID] = nZoneID;

            List<CCTV> sensors = dataManager.GetSelectManager().SelectCCTVs(dicConditions, null, out strErrorMessage);

            if (sensors == null)
                return false;

            CCTVSensor cctvSensor;
            cctvSensors = new List<CCTVSensor>();

            foreach (CCTV sensor in sensors)
            {
                if (m_dicCCTVs.TryGetValue(sensor.ID, out cctvSensor))
                {
                    cctvSensor.CameraName = sensor.CameraName;
                    cctvSensor.PositionName = sensor.PositionName;
                    cctvSensor.Enabled = sensor.Enabled;
                    cctvSensor.Type = sensor.Type;
                    cctvSensor.URL = sensor.URL;
                    cctvSensor.BigURL = sensor.BigURL;
                    cctvSensor.SmallURL = sensor.SmallURL;
                    cctvSensor.X = sensor.X;
                    cctvSensor.Y = sensor.Y;
                    cctvSensor.Z = sensor.Z;

                    cctvSensors.Add(cctvSensor);

                    long key = GetSensorTypeKey((int)dnsData.Sensor.Facility.FacilityType.CCTV, cctvSensor.ID);

                    if (cctvSensor.Enabled != null && cctvSensor.Enabled == false)
                    {
                        m_dicDisabledCCTVs[cctvSensor.ID] = cctvSensor;
                    }
                    else
                    {
                        CCTV temp;
                        m_dicDisabledCCTVs.TryRemove(cctvSensor.ID, out temp);
                    }
                }
            }

            return true;
        }

        public bool MoveSensor(IDataManager dataManager, string strSensorType, int nSensorID, float x, float z, out string strErrorMessage)
        {
            if (strSensorType == FireSensorType)
            {
                Fire sensor = dataManager.GetSelectManager().SelectFireSensor(nSensorID, out strErrorMessage);

                if (sensor == null)
                {
                    if (strErrorMessage != null)
                        return false;
                    else
                    {
                        strErrorMessage = string.Format("[{0}] Sensor ID {1}에 해당하는 센서가 존재하지 않습니다.", strSensorType, nSensorID);
                        return false;
                    }
                }

                sensor.X = x;
                sensor.Z = z;
                return dataManager.GetUpdateManager().UpdateFireSensor(sensor, out strErrorMessage);
            }
            else if (strSensorType == PSMSensorType)
            {
                PSM sensor = dataManager.GetSelectManager().SelectPSMSensor(nSensorID, out strErrorMessage);

                if (sensor == null)
                {
                    if (strErrorMessage != null)
                        return false;
                    else
                    {
                        strErrorMessage = string.Format("[{0}] Sensor ID {1}에 해당하는 센서가 존재하지 않습니다.", strSensorType, nSensorID);
                        return false;
                    }
                }

                sensor.X = x;
                sensor.Z = z;
                return dataManager.GetUpdateManager().UpdatePSMSensor(sensor, out strErrorMessage);
            }
            else if (strSensorType == EtcSensorType)
            {
                ETC sensor = dataManager.GetSelectManager().SelectETCSensor(nSensorID, out strErrorMessage);

                if (sensor == null)
                {
                    if (strErrorMessage != null)
                        return false;
                    else
                    {
                        strErrorMessage = string.Format("[{0}] Sensor ID {1}에 해당하는 센서가 존재하지 않습니다.", strSensorType, nSensorID);
                        return false;
                    }
                }

                sensor.X = x;
                sensor.Z = z;
                return dataManager.GetUpdateManager().UpdateETCSensor(sensor, out strErrorMessage);
            }
            else if (strSensorType.StartsWith(CCTVType))
            {
                CCTV cctv = dataManager.GetSelectManager().SelectCCTV(nSensorID, out strErrorMessage);

                if (cctv == null)
                {
                    if (strErrorMessage != null)
                        return false;
                    else
                    {
                        strErrorMessage = string.Format("[{0}] Sensor ID {1}에 해당하는 센서가 존재하지 않습니다.", strSensorType, nSensorID);
                        return false;
                    }
                }

                cctv.X = x;
                cctv.Z = z;
                return dataManager.GetUpdateManager().UpdateCCTV(cctv, out strErrorMessage);
            }

            strErrorMessage = "알려지지 않은 센서타입입니다. : " + strSensorType;
            return false;
        }

        public void CheckDisabledSensors(IDataManager dataManager)
        {
            ReadDisabledSenosrZones(dataManager);
            ReadDisabledFireSensors(dataManager);
            ReadDisabledPSMSensors(dataManager);
            ReadDisabledEtcSensors(dataManager);
            ReadDisabledCCTVs(dataManager);
        }

        private bool ReadDisabledFireSensors(IDataManager dataManager)
        {
            Dictionary<Fire.Fields, object> dicConditions = new Dictionary<Fire.Fields, object>();
            dicConditions[Fire.Fields.Enabled] = false;

            string strErrorMessage;
            List<Fire> fireSensors = dataManager.GetSelectManager().SelectFireSensors(dicConditions, null, out strErrorMessage);

            if (fireSensors == null)
                return false;

            Dictionary<int, int> prevIDs = new Dictionary<int, int>();
            Dictionary<int, Fire> disabledSensors = new Dictionary<int, Fire>();

            foreach (int id in m_dicDisabledFireSensors.Keys)
            {
                prevIDs[id] = id;
            }

            foreach (Fire sensor in fireSensors)
            {
                prevIDs.Remove(sensor.ID);
                disabledSensors[sensor.ID] = sensor;
            }

            Fire temp;

            foreach (KeyValuePair<int, int> pair in prevIDs)
            {
                m_dicDisabledFireSensors.TryRemove(pair.Key, out temp);
            }

            foreach (KeyValuePair<int, Fire> pair in disabledSensors)
            {
                m_dicDisabledFireSensors[pair.Key] = pair.Value;
            }

            foreach (KeyValuePair<long, TagInfo> pair in m_dicDisabledTypeSensorTagInfos)
            {
                int nSensorID;
                int nSensorType = GetSensorType(pair.Key, out nSensorID);
                dnsData.Sensor.Facility.FacilityType type = dnsData.Sensor.Facility.ToFacilityType(nSensorType);

                if (dnsData.Sensor.Facility.IsFireSensorType(type))
                {
                    FireSensor fire;

                    if (m_dicFireSensors.TryGetValue(nSensorID, out fire))
                    {
                        m_dicDisabledFireSensors[nSensorID] = fire;
                    }
                }
            }

            return true;
        }

        private bool ReadDisabledPSMSensors(IDataManager dataManager)
        {
            Dictionary<PSM.Fields, object> dicConditions = new Dictionary<PSM.Fields, object>();
            dicConditions[PSM.Fields.Enabled] = false;

            string strErrorMessage;
            List<PSM> psmSensors = dataManager.GetSelectManager().SelectPSMSensors(dicConditions, null, out strErrorMessage);

            if (psmSensors == null)
                return false;

            Dictionary<int, int> prevIDs = new Dictionary<int, int>();
            Dictionary<int, PSM> disabledSensors = new Dictionary<int, PSM>();

            foreach (int id in m_dicDisabledPSMSensors.Keys)
            {
                prevIDs[id] = id;
            }

            foreach (PSM sensor in psmSensors)
            {
                prevIDs.Remove(sensor.ID);
                disabledSensors[sensor.ID] = sensor;
            }

            PSM temp;

            foreach (KeyValuePair<int, int> pair in prevIDs)
            {
                m_dicDisabledPSMSensors.TryRemove(pair.Key, out temp);
            }

            foreach (KeyValuePair<int, PSM> pair in disabledSensors)
            {
                m_dicDisabledPSMSensors[pair.Key] = pair.Value;
            }

            foreach (KeyValuePair<long, TagInfo> pair in m_dicDisabledTypeSensorTagInfos)
            {
                int nSensorID;
                int nSensorType = GetSensorType(pair.Key, out nSensorID);
                dnsData.Sensor.Facility.FacilityType type = dnsData.Sensor.Facility.ToFacilityType(nSensorType);

                if (dnsData.Sensor.Facility.IsPSMSensorType(type))
                {
                    PSMSensor psm;

                    if (m_dicPSMSensors.TryGetValue(nSensorID, out psm))
                    {
                        m_dicDisabledPSMSensors[nSensorID] = psm;
                    }
                }
            }

            return true;
        }

        private bool ReadDisabledEtcSensors(IDataManager dataManager)
        {
            Dictionary<ETC.Fields, object> dicConditions = new Dictionary<ETC.Fields, object>();
            dicConditions[ETC.Fields.Enabled] = false;

            string strErrorMessage;
            List<ETC> etcSensors = dataManager.GetSelectManager().SelectETCSensors(dicConditions, null, out strErrorMessage);

            if (etcSensors == null)
                return false;

            Dictionary<int, int> prevIDs = new Dictionary<int, int>();
            Dictionary<int, ETC> disabledSensors = new Dictionary<int, ETC>();

            foreach (int id in m_dicDisabledEtcSensors.Keys)
            {
                prevIDs[id] = id;
            }

            foreach (ETC sensor in etcSensors)
            {
                prevIDs.Remove(sensor.ID);
                disabledSensors[sensor.ID] = sensor;
            }

            ETC temp;

            foreach (KeyValuePair<int, int> pair in prevIDs)
            {
                m_dicDisabledEtcSensors.TryRemove(pair.Key, out temp);
            }

            foreach (KeyValuePair<int, ETC> pair in disabledSensors)
            {
                m_dicDisabledEtcSensors[pair.Key] = pair.Value;
            }

            foreach (KeyValuePair<long, TagInfo> pair in m_dicDisabledTypeSensorTagInfos)
            {
                int nSensorID;
                int nSensorType = GetSensorType(pair.Key, out nSensorID);
                dnsData.Sensor.Facility.FacilityType type = dnsData.Sensor.Facility.ToFacilityType(nSensorType);

                if (dnsData.Sensor.Facility.IsETCSensorType(type))
                {
                    EtcSensor etc;

                    if (m_dicEtcSensors.TryGetValue(nSensorID, out etc))
                    {
                        m_dicDisabledEtcSensors[nSensorID] = etc;
                    }
                }
            }

            return true;
        }

        private bool ReadDisabledCCTVs(IDataManager dataManager)
        {
            Dictionary<CCTV.Fields, object> dicConditions = new Dictionary<CCTV.Fields, object>();
            dicConditions[CCTV.Fields.Enabled] = false;

            string strErrorMessage;
            List<CCTV> cctvSensors = dataManager.GetSelectManager().SelectCCTVs(dicConditions, null, out strErrorMessage);

            if (cctvSensors == null)
                return false;

            Dictionary<int, int> prevIDs = new Dictionary<int, int>();
            Dictionary<int, CCTV> disabledSensors = new Dictionary<int, CCTV>();

            foreach (int id in m_dicDisabledCCTVs.Keys)
            {
                prevIDs[id] = id;
            }

            foreach (CCTV sensor in cctvSensors)
            {
                prevIDs.Remove(sensor.ID);
                disabledSensors[sensor.ID] = sensor;
            }

            CCTV temp;

            foreach (KeyValuePair<int, int> pair in prevIDs)
            {
                m_dicDisabledCCTVs.TryRemove(pair.Key, out temp);
            }

            foreach (KeyValuePair<int, CCTV> pair in disabledSensors)
            {
                m_dicDisabledCCTVs[pair.Key] = pair.Value;
            }

            return true;
        }

        private bool ReadDisabledSenosrZones(IDataManager dataManager)
        {
            Dictionary<TagInfo.Fields, object> dicConditions = new Dictionary<TagInfo.Fields, object>();
            dicConditions[TagInfo.Fields.Activate] = false;

            string strErrorMessage;
            ArrayList arrDatas = dataManager.GetSelectManager().JoinSensorZoneTagInfo(null, dicConditions, null, out strErrorMessage);

            if (arrDatas == null)
                return false;

            Dictionary<long, long> prevKeys = new Dictionary<long, long>();
            
            foreach (long key in m_dicDisabledTypeSensorTagInfos.Keys)
            {
                prevKeys[key] = key;
            }

            int nDataCount = arrDatas.Count;
            Dictionary<long, TagInfo> disabledTagInfos = new Dictionary<long, TagInfo>();

            for (int i=0;i<nDataCount-1;i+=2)
            {
                if (arrDatas[i] is SensorZone && arrDatas[i + 1] is TagInfo)
                {
                    SensorZone sensorZone = (SensorZone)arrDatas[i];
                    TagInfo tag = (TagInfo)arrDatas[i + 1];

                    long key = GetSensorTypeKey(sensorZone.SensorType, (int)sensorZone.OrgSensorID);
                    disabledTagInfos[key] = tag;

                    prevKeys.Remove(key);
                }
            }

            TagInfo temp;

            foreach (KeyValuePair<long, long> pair in prevKeys)
            {
                m_dicDisabledTypeSensorTagInfos.TryRemove(pair.Key, out temp);
            }

            foreach (KeyValuePair<long, TagInfo> pair in disabledTagInfos)
            {
                m_dicDisabledTypeSensorTagInfos[pair.Key] = pair.Value;
            }

            return true;
        }

        public bool ReloadSensors(IDataManager dataManager, int nZoneID, out List<FireSensor> fireSensors, out List<PSMSensor> psmSensors, out List<EtcSensor> etcSensors, out List<CCTVSensor> cctvSensors, out string strErrorMessage)
        {
            psmSensors = null;
            etcSensors = null;
            cctvSensors = null;

            if (ReloadFireSensors(dataManager, nZoneID, out fireSensors, out strErrorMessage) == false)
                return false;
            if (ReloadPSMSensors(dataManager, nZoneID, out psmSensors, out strErrorMessage) == false)
                return false;
            if (ReloadEtcSensors(dataManager, nZoneID, out etcSensors, out strErrorMessage) == false)
                return false;
            if (ReloadCCTVs(dataManager, nZoneID, out cctvSensors, out strErrorMessage) == false)
                return false;

            return true;
        }

        public static bool IsFireSensor(string strSensorType)
        {
            if (string.Compare(strSensorType, FireSensorType, true) == 0)
            {
                return true;
            }

            return false;
        }

        public static bool IsPSMSensor(string strSensorType)
        {
            if (string.Compare(strSensorType, PSMSensorType, true) == 0)
            {
                return true;
            }

            return false;
        }

        public static bool IsEtcSensor(string strSensorType)
        {
            if (string.Compare(strSensorType, EtcSensorType, true) == 0)
            {
                return true;
            }

            return false;
        }

        public static bool IsCCTVType(string strSensorType)
        {
            if (strSensorType.StartsWith(CCTVType))
            //if (string.Compare(strSensorType, CCTVType, true) == 0)
            {
                return true;
            }

            return false;
        }
    }
}
