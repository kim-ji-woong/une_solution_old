using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Spatial;
using DBUtility;
using System.Collections;
using UnE.Sensor;

namespace SDMS
{
    public class SensorTagHistoryManager
    {
        private static SensorTagHistoryManager m_instance = new SensorTagHistoryManager();

        public static SensorTagHistoryManager Instance
        {
            get { return m_instance; }
        }

        // Key : 상위 4바이트(SensorServer ID), 하위 4바이트(TagID)
        private Dictionary<long, SensorTagInfo> m_dicFireSensorTags = new Dictionary<long, SensorTagInfo>();
        // Key : 상위 4바이트(SensorServer ID), 하위 4바이트(TagID)
        private Dictionary<long, SensorTagInfo> m_dicPSMSensorTags = new Dictionary<long, SensorTagInfo>();
        // Key : 상위 4바이트(SensorServer ID), 하위 4바이트(TagID)
        private Dictionary<long, SensorTagInfo> m_dicIntrusionSensorTags = new Dictionary<long, SensorTagInfo>();
        // 각 센서들의 마지막 상태값
        // Key : 상위 4바이트(SensorServer ID), 하위 4바이트(TagID)
        // Value : 상태값(0이면 정상, 그 외는 알람)
        private Dictionary<long, int> m_dicSensorLastValue = new Dictionary<long, int>();
        // SensorZone별 SensorTag
        // Key : SensorZone ID
        private Dictionary<int, SensorTagInfo> m_dicSensorZoneSensorTags = new Dictionary<int, SensorTagInfo>();

        private List<SensorTagHistory> m_sensorFireHistories = new List<SensorTagHistory>();
        private List<SensorTagHistory> m_sensorPSMHistories = new List<SensorTagHistory>();
        private List<SensorTagHistory> m_sensorIntrusionHistories = new List<SensorTagHistory>();

        private int m_nFireLastReadHistoryID = -1;
        private int m_nPSMLastReadHistoryID = -1;
        private int m_nIntrusionLastReadHistoryID = -1;

        private object m_lockObject = new object();

        private SensorTagHistoryManager()
        {
        }

        public bool LoadSensorTags(WebDBManager dbMgr)
        {
            string strSQL = "Select ID, SensorServerID, TagNo, SensorName, SensorType, EquipZoneID, SensorZoneID from SensorTagInfo"; 
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-6;i+=7)
            { 
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> sensorServerID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> tagID = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                string strTagName = WebDBManager.GetStringField(arrResult[i + 3]);
                VariousData<int> sensorType = WebDBManager.GetIntField(arrResult[i + 4].ToString());
                VariousData<int> equipZoneID = WebDBManager.GetIntField(arrResult[i + 5].ToString());
                VariousData<int> sensorZoneID = WebDBManager.GetIntField(arrResult[i + 6].ToString());
                 
                if (id == null || sensorServerID == null || tagID == null || strTagName == null)
                    continue;

                UnE.Sensor.IFacility.FacilityType type = UnE.Sensor.IFacility.ToFacilityType(sensorType.Data);



                //if (type >= UnE.Sensor.IFacility.FacilityType.Intrusion_S1 || type == UnE.Sensor.IFacility.FacilityType.EmergencyBell_S1)
                //    type = UnE.Sensor.IFacility.FacilityType.FIRE_SENSOR;


                if (type == UnE.Sensor.IFacility.FacilityType.Fire_S1 || type == UnE.Sensor.IFacility.FacilityType.FireF1_S1)
                    type = UnE.Sensor.IFacility.FacilityType.FIRE_SENSOR;

                Dictionary<long, SensorTagInfo> tags = GetSensorTagDictionary(type);

                if (tags == null)
                    continue;

                SensorTagInfo tag = new SensorTagInfo();

                tag.ID = id.Data;
                tag.ServerID = sensorServerID.Data;
                tag.TagID = tagID.Data;
                tag.TagName = strTagName;
                tag.SensorType = type;

                if (equipZoneID != null)
                    tag.EquipmentZone = ZoneManager.Instance.GetEquipZone(equipZoneID.Data);

                if (sensorZoneID != null)
                    tag.SensorZoneID = sensorZoneID.Data;

                long key = MakeKey(sensorServerID.Data, tagID.Data);
                tags[key] = tag;

                if (sensorZoneID != null)
                    m_dicSensorZoneSensorTags[sensorZoneID.Data] = tag;
            }

            return true;
        }

        private long MakeKey(int nSensorServerID, int nTagNo)
        {
            long key = (((long)nSensorServerID) << 32) | ((long)nTagNo);
            return key;
        }

        private Dictionary<long, SensorTagInfo> GetSensorTagDictionary(UnE.Sensor.IFacility.FacilityType type)
        {
            if (type == UnE.Sensor.IFacility.FacilityType.FIRE_SENSOR ||
                (type >= UnE.Sensor.IFacility.FacilityType.FireSensor_TypeA && type <= UnE.Sensor.IFacility.FacilityType.FireSensor_MonitoringType) ||
                type == UnE.Sensor.IFacility.FacilityType.Fire_S1 || type == UnE.Sensor.IFacility.FacilityType.FireF1_S1)
 
            {
                return m_dicFireSensorTags;
            }
            else if (type == UnE.Sensor.IFacility.FacilityType.PSM_SENSOR)
                return m_dicPSMSensorTags;
            else if (type >= UnE.Sensor.IFacility.FacilityType.Intrusion_S1 && type <= UnE.Sensor.IFacility.FacilityType.ExternalAlarmBell
                && type != UnE.Sensor.IFacility.FacilityType.Fire_S1 && type != UnE.Sensor.IFacility.FacilityType.FireF1_S1)
                return m_dicIntrusionSensorTags;

            return null;
        }

        private List<SensorTagHistory> GetSensorTagHistroies(UnE.Sensor.IFacility.FacilityType type)
        {
            if (type == UnE.Sensor.IFacility.FacilityType.FIRE_SENSOR ||
                (type >= UnE.Sensor.IFacility.FacilityType.FireSensor_TypeA && type <= UnE.Sensor.IFacility.FacilityType.FireSensor_MonitoringType)||
                type == UnE.Sensor.IFacility.FacilityType.Fire_S1 || type == UnE.Sensor.IFacility.FacilityType.FireF1_S1)
            {
                return m_sensorFireHistories;
            }
            else if (type == UnE.Sensor.IFacility.FacilityType.PSM_SENSOR)
                return m_sensorPSMHistories;
            else if (type >= UnE.Sensor.IFacility.FacilityType.Intrusion_S1 && type <= UnE.Sensor.IFacility.FacilityType.ExternalAlarmBell
                && type != UnE.Sensor.IFacility.FacilityType.Fire_S1 && type != UnE.Sensor.IFacility.FacilityType.FireF1_S1)
                return m_sensorIntrusionHistories;

            return null;
        }

        public SensorTagInfo GetSensorTagFromSensorZone(int nSensorZoneID)
        {
            SensorTagInfo tag = null;

            if (m_dicSensorZoneSensorTags.TryGetValue(nSensorZoneID, out tag))
                return tag;

            return null;
        }

        // dicSensorTags에서 찾고 없으면 DB에서 읽어온다.
        private SensorTagInfo GetSensorTagInfo(long key, Dictionary<long, SensorTagInfo> dicSensorTags, WebDBManager dbMgr)
        {
            SensorTagInfo tag = null;

            if (dicSensorTags.TryGetValue(key, out tag))
                return tag;

            int nSensorServerID = (int)(key >> 32);
            int nTagNo = (int)(key & 0xffffffff);

            string strSQL = "Select ID, SensorName, SensorType, EquipZoneID, SensorZoneID from SensorTagInfo  where SensorServerID = " + nSensorServerID.ToString() + " and TagNo = " + nTagNo.ToString(); 
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;

            if (nResultCount < 5)
                return null;

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());
            string strTagName = WebDBManager.GetStringField(arrResult[1]);
            VariousData<int> sensorType = WebDBManager.GetIntField(arrResult[2].ToString());
            VariousData<int> equipZoneID = WebDBManager.GetIntField(arrResult[3].ToString());
            VariousData<int> sensorZoneID = WebDBManager.GetIntField(arrResult[4].ToString());

            if (id == null || strTagName == null)
                return null;

            UnE.Sensor.IFacility.FacilityType type = UnE.Sensor.IFacility.ToFacilityType(sensorType.Data);
            
            if (type == UnE.Sensor.IFacility.FacilityType.Fire_S1 || type == UnE.Sensor.IFacility.FacilityType.FireF1_S1)
                type = UnE.Sensor.IFacility.FacilityType.FIRE_SENSOR;

            Dictionary<long, SensorTagInfo> tags = GetSensorTagDictionary(type);

            if (tags == null)
                return null;

            tag = new SensorTagInfo();

            tag.ID = id.Data;
            tag.ServerID = nSensorServerID;
            tag.TagID = nTagNo;
            tag.TagName = strTagName;
            tag.SensorType = type;

            if (equipZoneID != null)
                tag.EquipmentZone = ZoneManager.Instance.GetEquipZone(equipZoneID.Data);

            if (sensorZoneID != null)
                tag.SensorZoneID = sensorZoneID.Data;

            tags[key] = tag;

            if (tags == dicSensorTags)
                return tag;

            return null;
        }

        // dtLimit 이전의 로그는 폐기한다.
        public void ProcessDeleteSensorTagHistory(DateTime dtLimit)
        {
            int nIndexFire = FindSensorHistoryIndex(ref dtLimit, true, m_sensorFireHistories, UnE.Sensor.IFacility.FacilityType.FIRE_SENSOR);
            int nIndexPSM = FindSensorHistoryIndex(ref dtLimit, true, m_sensorPSMHistories, UnE.Sensor.IFacility.FacilityType.PSM_SENSOR);
            int nIndexIntrusion = FindSensorHistoryIndex(ref dtLimit, true, m_sensorIntrusionHistories, UnE.Sensor.IFacility.FacilityType.Intrusion_S1);

            lock (m_lockObject)
            {
                for (int i = nIndexFire - 1; i >= 0; i--)
                {
                    m_sensorFireHistories.RemoveAt(i);
                }

                for (int i = nIndexPSM - 1; i >= 0; i--)
                {
                    m_sensorPSMHistories.RemoveAt(i);
                }

                for (int i = nIndexIntrusion - 1; i >= 0; i--)
                {
                    m_sensorIntrusionHistories.RemoveAt(i);
                }
            }
        }

        private bool LoadSensorTagHistories(WebDBManager dbMgr, int nHistoryType, ref int nLastReadHistoryID)
        {
            string strSQL = string.Empty;
            if (nHistoryType == (int)UnE.Sensor.IFacility.FacilityType.Intrusion_S1)
            {
                string strTypeIDs = (int)IFacility.FacilityType.Intrusion_S1 + "," + (int)IFacility.FacilityType.Loiter_S1 + ","
                    + (int)IFacility.FacilityType.Collapse_S1 + "," + (int)IFacility.FacilityType.Theft_S1 + "," + (int)IFacility.FacilityType.Neglect_S1 + ","
                    + (int)IFacility.FacilityType.VirtualFence_S1 + "," + (int)IFacility.FacilityType.EmergencyBell_S1 + "," + (int)IFacility.FacilityType.GeneralIntrusionT1_S1 + ","
                    + (int)IFacility.FacilityType.GeneralIntrusionT2_S1 + "," + (int)IFacility.FacilityType.InternalIntrusionT3_S1 + "," + (int)IFacility.FacilityType.VaultIntrusionT4_S1 + ","
                    + (int)IFacility.FacilityType.CustomerEmergencyC1_S1 + "," + (int)IFacility.FacilityType.CustomerEmergencyC2_S1 + "," + (int)IFacility.FacilityType.RescueQQ_S1 + ","
                    + (int)IFacility.FacilityType.GasG1_S1 + "," + (int)IFacility.FacilityType.BlackoutAbnormalityU1_S1 + "," + (int)IFacility.FacilityType.LeakAbnormalityU4_S1 + "," 
                    + (int)IFacility.FacilityType.SynthesisAlertAbnormalityU8_S1 + "," + (int)IFacility.FacilityType.ExternalAlarmBell;
                strSQL = "Select sth.ID, st.SensorServerID, st.TagNo, sth.TimeStamp, sth.value, sth.HistoryType from SensorTagHistory as sth, SensorTagInfo as st ";
                strSQL += "where sth.SensorTagInfoID = st.ID and sth.HistoryType in (" + strTypeIDs + ")";
                strSQL += " and sth.ID > " + nLastReadHistoryID.ToString();
                strSQL += " order by sth.TimeStamp";
            }
            else if (nHistoryType == (int)UnE.Sensor.IFacility.FacilityType.FIRE_SENSOR)
            {
                string strTypeIDs = ((int)UnE.Sensor.IFacility.FacilityType.FIRE_SENSOR).ToString();
                strTypeIDs += ", " + ((int)UnE.Sensor.IFacility.FacilityType.Fire_S1).ToString();
                strTypeIDs += ", " + ((int)UnE.Sensor.IFacility.FacilityType.FireF1_S1).ToString();
                strSQL = "Select sth.ID, st.SensorServerID, st.TagNo, sth.TimeStamp, sth.value, sth.HistoryType from SensorTagHistory as sth, SensorTagInfo as st ";
                strSQL += "where sth.SensorTagInfoID = st.ID and sth.HistoryType in (" + strTypeIDs + ") and sth.ID > " + nLastReadHistoryID.ToString();
                strSQL += " order by sth.TimeStamp";
            }
            else
            {
                strSQL = "Select sth.ID, st.SensorServerID, st.TagNo, sth.TimeStamp, sth.value, sth.HistoryType from SensorTagHistory as sth, SensorTagInfo as st ";
                strSQL += "where sth.SensorTagInfoID = st.ID and sth.HistoryType = " + nHistoryType.ToString() + " and sth.ID > " + nLastReadHistoryID.ToString();
                strSQL += " order by sth.TimeStamp";
            }
            //string strSQL = "Select ID, SensorServerID, TagNo, TimeStamp, value, HistoryType from SensorTagHistory where HistoryType = " + nHistoryType.ToString() + " and ID > " + nLastReadHistoryID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            const int ON = (int)'N';
            const int OFF = (int)'F';
            const int RESET = (int)'R';

            int nPrevValue;
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 5; i += 6)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> sensorServerID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> tagNo = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<DateTime> timeStamp = WebDBManager.GetDateTimeField(arrResult[i + 3]);
                VariousData<int> value = WebDBManager.GetIntField(arrResult[i + 4].ToString());
                VariousData<int> historyType = WebDBManager.GetIntField(arrResult[i + 5].ToString());

                if (id == null || sensorServerID == null || tagNo == null || timeStamp == null || value == null || historyType == null)
                    continue;

                nLastReadHistoryID = id.Data;

                UnE.Sensor.IFacility.FacilityType type = UnE.Sensor.IFacility.ToFacilityType(historyType.Data);
                Dictionary<long, SensorTagInfo> dicSensorTags = GetSensorTagDictionary(type);
                List<SensorTagHistory> sensorTagHistories = GetSensorTagHistroies(type);

                if (dicSensorTags == null || sensorTagHistories == null)
                    continue;

                long key = MakeKey(sensorServerID.Data, tagNo.Data);

                SensorTagInfo sensorTag = GetSensorTagInfo(key, dicSensorTags, dbMgr);

                if (sensorTag == null)
                    continue;

                SensorTagHistory history = new SensorTagHistory();
                history.ID = id.Data;
                history.SensorTag = sensorTag;
                history.TimeStamp = timeStamp.Data;

                // 이전값이 알람이었거나, 현재값이 알람이 아닌경우 history를 저장하지 않는다.
                if (m_dicSensorLastValue.TryGetValue(key, out nPrevValue))
                {
                    // 이전값이 알람이 아닐 경우
                    if (nPrevValue == OFF)
                    {
                        if (value.Data != OFF)
                            sensorTagHistories.Add(history);
                    }
                }
                else
                {
                    if (value.Data != OFF)
                        sensorTagHistories.Add(history);
                }

                m_dicSensorLastValue[key] = value.Data;
            }

            return true;
        }

        // dicSensorHistoryCount : Value(SensorTag별 알람 History Count)
        // dicEquipZoneHistoryCount : Value(EquipZone별 알람 History Count)
        // Return 값 : 값이 바뀌었는지 여부. true이면 값이 바뀌었음
        public bool LoadFireSensorTagHistories(ArrayList arrSelectZoneList, Dictionary<int, UnE.Spatial.Zone> dicPrevZones, Dictionary<SensorTagInfo, int> dicSensorHistoryCount, Dictionary<EquipmentZone, int> dicEquipZoneHistoryCount, ref int nPrevBeginIndex, ref int nPrevEndIndex, WebDBManager dbMgr, DateTime dtBegin, DateTime dtEnd, UnE.Sensor.IFacility.FacilityType type)
        {
            lock (m_lockObject)
            {
                bool allZones = ZoneManager.Instance.DicZones.Count == arrSelectZoneList.Count;

                bool isEmpty = dicSensorHistoryCount.Count == 0;

                if (!LoadSensorTagHistories(dbMgr, (int)type, ref m_nFireLastReadHistoryID))
                {
                    nPrevBeginIndex = nPrevEndIndex = -1;
                    dicSensorHistoryCount.Clear();
                    dicEquipZoneHistoryCount.Clear();
                    return !isEmpty;
                }

                List<SensorTagHistory> sensorTagHistories = GetSensorTagHistroies(type);

                if (sensorTagHistories == null)
                {
                    nPrevBeginIndex = nPrevEndIndex = -1;
                    dicSensorHistoryCount.Clear();
                    dicEquipZoneHistoryCount.Clear();
                    return !isEmpty;
                }

                // m_sensorHistories는 이미 시간순으로 정렬되어 있다.
                int nIndexBegin = FindSensorHistoryIndex(ref dtBegin, true, sensorTagHistories, type);
                int nIndexEnd = FindSensorHistoryIndex(ref dtEnd, false, sensorTagHistories, type);

                if (nIndexBegin < 0 || nIndexEnd < nIndexBegin)
                {
                    nPrevBeginIndex = nPrevEndIndex = -1;
                    dicSensorHistoryCount.Clear();
                    dicEquipZoneHistoryCount.Clear();
                    return !isEmpty;
                }

                bool isSameZones = false;

                if (CheckPrevZones(allZones, arrSelectZoneList, dicPrevZones))
                {
                    isSameZones = true;
                }
                else
                {
                    dicPrevZones.Clear();

                    foreach (Zone zone in arrSelectZoneList)
                    {
                        dicPrevZones[zone.ID] = zone;
                    }
                }

                if (nIndexBegin == nPrevBeginIndex && nIndexEnd == nPrevEndIndex && isSameZones)
                {
                    // 이전값과 같음
                    return false;
                }

                nPrevBeginIndex = nIndexBegin;
                nPrevEndIndex = nIndexEnd;
                dicSensorHistoryCount.Clear();
                dicEquipZoneHistoryCount.Clear();

                int nHistoryCount = 0;

                for (int i = nIndexBegin; i <= nIndexEnd; i++)
                {
                    SensorTagHistory history = sensorTagHistories[i];

                    if (!allZones)
                    {
                        if (!ContainsHistoryZone(history, dicPrevZones))
                            continue;
                    }

                    if (dicSensorHistoryCount.TryGetValue(history.SensorTag, out nHistoryCount))
                        dicSensorHistoryCount[history.SensorTag] = nHistoryCount + 1;
                    else
                        dicSensorHistoryCount[history.SensorTag] = 1;

                    if (history.SensorTag.EquipmentZone == null)
                    {
                        UnE.Sensor.ISensor sensorZone = SensorManager.Instance.GetSensorZone(history.SensorTag.SensorZoneID);

                        if (sensorZone != null && sensorZone.EquipZoneID > 0)
                        {
                            UnE.Spatial.EquipmentZone equipZone = ZoneManager.Instance.GetEquipZone(sensorZone.EquipZoneID);

                            if (equipZone != null)
                                history.SensorTag.EquipmentZone = equipZone;
                        }
                    }

                    if (history.SensorTag.EquipmentZone != null)
                    {
                        if (dicEquipZoneHistoryCount.TryGetValue(history.SensorTag.EquipmentZone, out nHistoryCount))
                            dicEquipZoneHistoryCount[history.SensorTag.EquipmentZone] = nHistoryCount + 1;
                        else
                            dicEquipZoneHistoryCount[history.SensorTag.EquipmentZone] = 1;
                    }
                }
            }

            return true;
        }

        // dicSensorHistoryCount : Value(SensorTag별 알람 History Count)
        // dicEquipZoneHistoryCount : Value(EquipZone별 알람 History Count)
        // Return 값 : 값이 바뀌었는지 여부. true이면 값이 바뀌었음
        public bool LoadIntrusionSensorTagHistories(ArrayList arrSelectZoneList, Dictionary<int, UnE.Spatial.Zone> dicPrevZones, Dictionary<SensorTagInfo, int> dicSensorHistoryCount, Dictionary<EquipmentZone, int> dicEquipZoneHistoryCount, ref int nPrevBeginIndex, ref int nPrevEndIndex, WebDBManager dbMgr, DateTime dtBegin, DateTime dtEnd, UnE.Sensor.IFacility.FacilityType type)
        {
            lock (m_lockObject)
            {
                bool allZones = ZoneManager.Instance.DicZones.Count == arrSelectZoneList.Count;

                bool isEmpty = dicSensorHistoryCount.Count == 0;

                if (!LoadSensorTagHistories(dbMgr, (int)type, ref m_nIntrusionLastReadHistoryID))
                {
                    nPrevBeginIndex = nPrevEndIndex = -1;
                    dicSensorHistoryCount.Clear();
                    dicEquipZoneHistoryCount.Clear();
                    return !isEmpty;
                }

                List<SensorTagHistory> sensorTagHistories = GetSensorTagHistroies(type);

                if (sensorTagHistories == null)
                {
                    nPrevBeginIndex = nPrevEndIndex = -1;
                    dicSensorHistoryCount.Clear();
                    dicEquipZoneHistoryCount.Clear();
                    return !isEmpty;
                }

                // m_sensorHistories는 이미 시간순으로 정렬되어 있다.
                int nIndexBegin = FindSensorHistoryIndex(ref dtBegin, true, sensorTagHistories, type);
                int nIndexEnd = FindSensorHistoryIndex(ref dtEnd, false, sensorTagHistories, type);

                if (nIndexBegin < 0 || nIndexEnd < nIndexBegin)
                {
                    nPrevBeginIndex = nPrevEndIndex = -1;
                    dicSensorHistoryCount.Clear();
                    dicEquipZoneHistoryCount.Clear();
                    return !isEmpty;
                }

                bool isSameZones = false;

                if (CheckPrevZones(allZones, arrSelectZoneList, dicPrevZones))
                {
                    isSameZones = true;
                }
                else
                {
                    dicPrevZones.Clear();

                    foreach (Zone zone in arrSelectZoneList)
                    {
                        dicPrevZones[zone.ID] = zone;
                    }
                }

                if (nIndexBegin == nPrevBeginIndex && nIndexEnd == nPrevEndIndex && isSameZones)
                {
                    // 이전값과 같음
                    return false;
                }

                nPrevBeginIndex = nIndexBegin;
                nPrevEndIndex = nIndexEnd;
                dicSensorHistoryCount.Clear();
                dicEquipZoneHistoryCount.Clear();

                int nHistoryCount = 0;

                for (int i = nIndexBegin; i <= nIndexEnd; i++)
                {
                    SensorTagHistory history = sensorTagHistories[i];

                    if (!allZones)
                    {
                        if (!ContainsHistoryZone(history, dicPrevZones))
                            continue;
                    }

                    if (dicSensorHistoryCount.TryGetValue(history.SensorTag, out nHistoryCount))
                        dicSensorHistoryCount[history.SensorTag] = nHistoryCount + 1;
                    else
                        dicSensorHistoryCount[history.SensorTag] = 1;

                    if (history.SensorTag.EquipmentZone == null)
                    {
                        UnE.Sensor.ISensor sensorZone = SensorManager.Instance.GetSensorZone(history.SensorTag.SensorZoneID);

                        if (sensorZone != null && sensorZone.EquipZoneID > 0)
                        {
                            UnE.Spatial.EquipmentZone equipZone = ZoneManager.Instance.GetEquipZone(sensorZone.EquipZoneID);

                            if (equipZone != null)
                                history.SensorTag.EquipmentZone = equipZone;
                        }
                    }

                    if (history.SensorTag.EquipmentZone != null)
                    {
                        if (dicEquipZoneHistoryCount.TryGetValue(history.SensorTag.EquipmentZone, out nHistoryCount))
                            dicEquipZoneHistoryCount[history.SensorTag.EquipmentZone] = nHistoryCount + 1;
                        else
                            dicEquipZoneHistoryCount[history.SensorTag.EquipmentZone] = 1;
                    }
                }
            }

            return true;
        }

        // dicSensorHistoryCount : Value(SensorTag별 알람 History Count)
        // dicEquipZoneHistoryCount : Value(EquipZone별 알람 History Count)
        // Return 값 : 값이 바뀌었는지 여부. true이면 값이 바뀌었음
        public bool LoadPSMSensorTagHistories(ArrayList arrSelectZoneList, Dictionary<int, UnE.Spatial.Zone> dicPrevZones, Dictionary<SensorTagInfo, int> dicSensorHistoryCount, Dictionary<UnE.PSM.PSMTank, int> dicTankHistoryCount, Dictionary<EquipmentZone, int> dicEquipZoneHistoryCount, Dictionary<UnE.PSM.PSMMaterial, int> dicMaterialHistoryCount, ref int nPrevBeginIndex, ref int nPrevEndIndex, WebDBManager dbMgr, DateTime dtBegin, DateTime dtEnd, UnE.Sensor.IFacility.FacilityType type)
        {
            lock (m_lockObject)
            {
                bool allZones = ZoneManager.Instance.DicZones.Count == arrSelectZoneList.Count;

                bool isEmpty = dicSensorHistoryCount.Count == 0;

                if (!LoadSensorTagHistories(dbMgr, (int)type, ref m_nPSMLastReadHistoryID))
                {
                    nPrevBeginIndex = nPrevEndIndex = -1;
                    dicSensorHistoryCount.Clear();
                    dicTankHistoryCount.Clear();
                    dicEquipZoneHistoryCount.Clear();
                    dicMaterialHistoryCount.Clear();
                    return !isEmpty;
                }

                List<SensorTagHistory> sensorTagHistories = GetSensorTagHistroies(type);

                if (sensorTagHistories == null)
                {
                    nPrevBeginIndex = nPrevEndIndex = -1;
                    dicSensorHistoryCount.Clear();
                    dicTankHistoryCount.Clear();
                    dicEquipZoneHistoryCount.Clear();
                    dicMaterialHistoryCount.Clear();
                    return !isEmpty;
                }

                // m_sensorHistories는 이미 시간순으로 정렬되어 있다.
                int nIndexBegin = FindSensorHistoryIndex(ref dtBegin, true, sensorTagHistories, type);
                int nIndexEnd = FindSensorHistoryIndex(ref dtEnd, false, sensorTagHistories, type);

                if (nIndexBegin < 0 || nIndexEnd < nIndexBegin)
                {
                    nPrevBeginIndex = nPrevEndIndex = -1;
                    dicSensorHistoryCount.Clear();
                    dicTankHistoryCount.Clear();
                    dicEquipZoneHistoryCount.Clear();
                    dicMaterialHistoryCount.Clear();
                    return !isEmpty;
                }

                bool isSameZones = false;

                if (CheckPrevZones(allZones, arrSelectZoneList, dicPrevZones))
                {
                    isSameZones = true;
                }
                else
                {
                    dicPrevZones.Clear();

                    foreach (Zone zone in arrSelectZoneList)
                    {
                        dicPrevZones[zone.ID] = zone;
                    }
                }

                if (nIndexBegin == nPrevBeginIndex && nIndexEnd == nPrevEndIndex && isSameZones)
                {
                    // 이전값과 같음
                    return false;
                }

                nPrevBeginIndex = nIndexBegin;
                nPrevEndIndex = nIndexEnd;

                dicSensorHistoryCount.Clear();
                dicTankHistoryCount.Clear();
                dicEquipZoneHistoryCount.Clear();
                dicMaterialHistoryCount.Clear();

                int nHistoryCount = 0;

                for (int i = nIndexBegin; i <= nIndexEnd; i++)
                {
                    SensorTagHistory history = sensorTagHistories[i];

                    if (!allZones)
                    {
                        if (!ContainsHistoryZone(history, dicPrevZones))
                            continue;
                    }

                    if (dicSensorHistoryCount.TryGetValue(history.SensorTag, out nHistoryCount))
                        dicSensorHistoryCount[history.SensorTag] = nHistoryCount + 1;
                    else
                        dicSensorHistoryCount[history.SensorTag] = 1;

                    UnE.PSM.PSMTank tank = GetPSMTank(history);

                    if (tank != null)
                    {
                        if (dicTankHistoryCount.TryGetValue(tank, out nHistoryCount))
                            dicTankHistoryCount[tank] = nHistoryCount + 1;
                        else
                            dicTankHistoryCount[tank] = 1;
                    }

                    if (history.SensorTag.EquipmentZone == null)
                    {
                        UnE.Sensor.ISensor sensorZone = SensorManager.Instance.GetSensorZone(history.SensorTag.SensorZoneID);

                        if (sensorZone != null && sensorZone.EquipZoneID > 0)
                        {
                            UnE.Spatial.EquipmentZone equipZone = ZoneManager.Instance.GetEquipZone(sensorZone.EquipZoneID);

                            if (equipZone != null)
                                history.SensorTag.EquipmentZone = equipZone;
                        }
                    }

                    if (history.SensorTag.EquipmentZone != null)
                    {
                        if (dicEquipZoneHistoryCount.TryGetValue(history.SensorTag.EquipmentZone, out nHistoryCount))
                            dicEquipZoneHistoryCount[history.SensorTag.EquipmentZone] = nHistoryCount + 1;
                        else
                            dicEquipZoneHistoryCount[history.SensorTag.EquipmentZone] = 1;
                    }

                    if (tank != null && tank.Material != null)
                    {
                        if (dicMaterialHistoryCount.TryGetValue(tank.Material, out nHistoryCount))
                            dicMaterialHistoryCount[tank.Material] = nHistoryCount + 1;
                        else
                            dicMaterialHistoryCount[tank.Material] = 1;
                    }
                }
            }

            return true;
        }

        private UnE.PSM.PSMTank GetPSMTank(SensorTagHistory history)
        {
            if (SensorManager.Instance.DicPSMSensorZone.ContainsKey(history.SensorTag.SensorZoneID))
            {
                UnE.PSM.PSMSensorZone sensorZone = (UnE.PSM.PSMSensorZone)SensorManager.Instance.DicPSMSensorZone[history.SensorTag.SensorZoneID];

                if (sensorZone != null)
                {
                    if (sensorZone.OrgSensor != null)
                    {
                        if (sensorZone.OrgSensor.LinkedTankList.Count > 0)
                        {
                            return sensorZone.OrgSensor.LinkedTankList[0];
                        }
                    }
                }
            }

            return null;
        }

        // Return 값 : true이면 이전 값과 같다.
        private bool CheckPrevZones(bool allZones, ArrayList arrZones, Dictionary<int, Zone> dicPrevZones)
        {
            if (allZones)
            {
                if (arrZones.Count == dicPrevZones.Count)
                    return true;
            }
            else
            {
                if (arrZones.Count != dicPrevZones.Count)
                    return false;

                foreach (Zone zone in arrZones)
                {
                    if (!dicPrevZones.ContainsKey(zone.ID))
                        return false;
                }

                return true;
            }

            return false;
        }

        private bool ContainsHistoryZone(SensorTagHistory history, Dictionary<int, Zone> dicZones)
        {
            if (history.SensorTag.EquipmentZone == null)
                return false;

            Zone target = null;

            foreach (Zone zone in history.SensorTag.EquipmentZone.LinkedZoneList)
            {
                if (dicZones.TryGetValue(zone.ID, out target))
                    return true;
            }

            return false;
        }

        // Return 값 : Value(SensorTag별 알람 History Count)
        /*public Dictionary<SensorTagInfo, int> LoadSensorTagHistories(WebDBManager dbMgr, DateTime dtBegin, DateTime dtEnd, UnE.Sensor.IFacility.FacilityType type)
        {
            Dictionary<SensorTagInfo, int> dicHistoryCount = new Dictionary<SensorTagInfo,int>();

            if (!LoadSensorTagHistories(dbMgr))
                return dicHistoryCount;

            List<SensorTagHistory> sensorTagHistories = GetSensorTagHistroies(type);

            if (sensorTagHistories == null)
                return dicHistoryCount;

            // m_sensorHistories는 이미 시간순으로 정렬되어 있다.
            int nIndexBegin = FindSensorHistoryIndex(ref dtBegin, true, sensorTagHistories);
            int nIndexEnd = FindSensorHistoryIndex(ref dtEnd, false, sensorTagHistories);

            if (nIndexBegin < 0 || nIndexEnd < nIndexBegin)
                return dicHistoryCount;

            int nHistoryCount = 0;

            for (int i=nIndexBegin;i<=nIndexEnd;i++)
            {
                SensorTagHistory history = m_sensorFireHistories[i];

                if (dicHistoryCount.TryGetValue(history.SensorTag, out nHistoryCount))
                    dicHistoryCount[history.SensorTag] = nHistoryCount + 1;
                else
                    dicHistoryCount[history.SensorTag] = 1;
            }

            return dicHistoryCount;
        }*/

        private int FindSensorHistoryIndex(ref DateTime timeStamp, bool isBegin, List<SensorTagHistory> sensorTagHistories, UnE.Sensor.IFacility.FacilityType type)
        {
            int nHistoryCount = sensorTagHistories.Count;

            if (nHistoryCount <= 0)
                return -1;

            return FindHistoryIndex(ref timeStamp, isBegin, nHistoryCount / 2, 0, nHistoryCount, sensorTagHistories, type);
        }

        private int FindHistoryIndex(ref DateTime timeStamp, bool isBeign, int nIndex, int nRangeBegin, int nRangeEnd, List<SensorTagHistory> sensorTagHistories, UnE.Sensor.IFacility.FacilityType type)
        {
            List<SensorTagHistory> prevHistories = null;

            if (type == UnE.Sensor.IFacility.FacilityType.FIRE_SENSOR)
                prevHistories = m_sensorFireHistories;
            else if (type == UnE.Sensor.IFacility.FacilityType.PSM_SENSOR)
                prevHistories = m_sensorPSMHistories;
            else if (type == UnE.Sensor.IFacility.FacilityType.Intrusion_S1)
                prevHistories = m_sensorIntrusionHistories;
            else
                return -1;

            SensorTagHistory historyCurrent = sensorTagHistories[nIndex];

            if (isBeign)
            {
                if (timeStamp <= historyCurrent.TimeStamp)
                {
                    if (nIndex - 1 >= 0 && nIndex - 1 >= nRangeBegin)
                    {
                        SensorTagHistory historyPrev = prevHistories[nIndex - 1];

                        if (historyPrev.TimeStamp < timeStamp)
                            return nIndex;
                        else
                            return FindHistoryIndex(ref timeStamp, isBeign, (nRangeBegin + nIndex) / 2, nRangeBegin, nIndex, sensorTagHistories, type);
                    }
                    else
                        return nIndex;
                }
                else
                {
                    if (nIndex + 1 < nRangeEnd)
                    {
                        SensorTagHistory historyNext = prevHistories[nIndex + 1];

                        if (timeStamp < historyNext.TimeStamp)
                            return nIndex + 1;
                        else
                            return FindHistoryIndex(ref timeStamp, isBeign, (nIndex + nRangeEnd) / 2, nIndex + 1, nRangeEnd, sensorTagHistories, type);
                    }
                    //else
                    //    return -1;
                }
            }
            else
            {
                if (historyCurrent.TimeStamp <= timeStamp)
                {
                    if (nIndex + 1 < nRangeEnd)
                    {
                        SensorTagHistory historyNext = prevHistories[nIndex + 1];

                        if (timeStamp < historyNext.TimeStamp)
                            return nIndex;
                        else
                            return FindHistoryIndex(ref timeStamp, isBeign, (nIndex + nRangeEnd) / 2, nIndex + 1, nRangeEnd, sensorTagHistories, type);
                    }
                    else
                        return nIndex;
                }
                else
                {
                    if (nIndex - 1 >= 0 && nIndex - 1 >= nRangeBegin)
                    {
                        SensorTagHistory historyPrev = prevHistories[nIndex - 1];

                        if (historyPrev.TimeStamp <= timeStamp)
                            return nIndex - 1;
                        else
                            return FindHistoryIndex(ref timeStamp, isBeign, (nRangeBegin + nIndex) / 2, nRangeBegin, nIndex, sensorTagHistories, type);
                    }
                    //else
                    //    return -1;
                }
            }

            return -1;
        }
    }

    public class SensorTagInfo
    {
        private int m_nID = -1;
        private int m_nServerID = -1;
        private int m_nTagID = -1;
        private string m_strTagName = "";
        private EquipmentZone m_equipZone = null;
        private int m_nSensorZoneID = -1;
        private UnE.Sensor.IFacility.FacilityType m_sensorType = UnE.Sensor.IFacility.FacilityType.NONE;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }
        
        public int ServerID
        {
            get { return m_nServerID; }
            set { m_nServerID = value; }
        }

        public int TagID
        {
            get { return m_nTagID; }
            set { m_nTagID = value; }
        }

        public string TagName
        {
            get { return m_strTagName; }
            set { m_strTagName = value; }
        }

        public EquipmentZone EquipmentZone
        {
            get { return m_equipZone; }
            set { m_equipZone = value; }
        }

        public int SensorZoneID
        {
            get { return m_nSensorZoneID; }
            set { m_nSensorZoneID = value; }
        }

        public UnE.Sensor.IFacility.FacilityType SensorType
        {
            get { return m_sensorType; }
            set { m_sensorType = value; }
        }
    }

    public class SensorTagHistory
    {
        private int m_nID = -1;
        private SensorTagInfo m_sensorTag = null;
        private DateTime m_timeStamp = new DateTime();
        private int m_sensorValue = 0;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public SensorTagInfo SensorTag
        {
            get { return m_sensorTag; }
            set { m_sensorTag = value; }
        }

        public DateTime TimeStamp
        {
            get { return m_timeStamp; }
            set { m_timeStamp = value; }
        }

        public int SensorValue
        {
            get { return m_sensorValue; }
            set { m_sensorValue = value; }
        }
    }
}
