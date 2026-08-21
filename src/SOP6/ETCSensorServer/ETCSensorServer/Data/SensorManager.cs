using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using UnE.Sensor;
using System.Collections;

namespace ETCSensorServer.Data
{
    public class SensorManager
    {
        // Key : Building ID
        private Dictionary<int, SensorTagInfo> m_dicWindSensors = new Dictionary<int, SensorTagInfo>();
        // Key : Building ID
        private Dictionary<int, SensorTagInfo> m_dicPowerOffSensors = new Dictionary<int, SensorTagInfo>();
        private WebDBManager m_dbMgr = null;

        public SensorManager(WebDBManager dbMgr)
        {
            m_dbMgr = dbMgr;
        }

        private void ReadSensors()
        {
            m_dicPowerOffSensors.Clear();
            m_dicWindSensors.Clear();

            string strSQL = "Select sti.ID, sti.SensorServerID, sti.TagNo, sti.TagID, sti.SensorName, sti.SensorType, sti.EquipZoneID, sti.SensorZoneID, ez.LinkedZoneIDList ";
            strSQL += "from SensorTagInfo as sti, EquipmentZone as ez where sti.EquipZoneID = ez.ID and SensorType in (" + GetEtcSensorTypes() + ")";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (strSQL == null)
                return;

            // Key : Zone ID
            Dictionary<int, List<SensorTagInfo>> dicPowerOffSenosrs = new Dictionary<int, List<SensorTagInfo>>();
            Dictionary<int, List<SensorTagInfo>> dicWindSenosrs = new Dictionary<int, List<SensorTagInfo>>();
            List<SensorTagInfo> sensors = null;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 8; i += 9)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> sensorServerID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> tagNo = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<int> tagID = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                string strSensorName = WebDBManager.GetStringField(arrResult[i + 4]);
                VariousData<int> sensorType = WebDBManager.GetIntField(arrResult[i + 5].ToString());
                VariousData<int> equipZoneID = WebDBManager.GetIntField(arrResult[i + 6].ToString());
                VariousData<int> sensorZoneID = WebDBManager.GetIntField(arrResult[i + 7].ToString());
                string strLinkedZoneIDList = WebDBManager.GetStringField(arrResult[i + 8]);

                if (id == null || sensorServerID == null || tagNo == null || strSensorName == null || equipZoneID == null || sensorZoneID == null || strLinkedZoneIDList == null)
                    continue;

                SensorTagInfo sensor = new SensorTagInfo();

                sensor.ID = id.Data;
                sensor.SensorServerID = sensorServerID.Data;
                sensor.TagNo = tagNo.Data;
                sensor.SensorName = strSensorName;
                sensor.SensorType = IFacility.ToFacilityType(sensorType.Data);
                sensor.EquipZoneID = equipZoneID.Data;
                sensor.SensorZoneID = sensorZoneID.Data;

                if (tagID != null)
                    sensor.TagID = tagID.Data;

                List<int> zoneIDs = ToIDs(strLinkedZoneIDList);

                foreach (int nZoneID in zoneIDs)
                {
                    if (sensor.SensorType == IFacility.FacilityType.BLACKOUT)
                    {
                        if (dicPowerOffSenosrs.TryGetValue(nZoneID, out sensors) == false)
                        {
                            sensors = new List<SensorTagInfo>();
                            dicPowerOffSenosrs[nZoneID] = sensors;
                        }

                        sensors.Add(sensor);
                    }
                    else if (sensor.SensorType == IFacility.FacilityType.STRONG_WIND)
                    {
                        if (dicWindSenosrs.TryGetValue(nZoneID, out sensors) == false)
                        {
                            sensors = new List<SensorTagInfo>();
                            dicWindSenosrs[nZoneID] = sensors;
                        }

                        sensors.Add(sensor);
                    }
                }
            }

            SetBuildingSensors(m_dicPowerOffSensors, dicPowerOffSenosrs);
            SetBuildingSensors(m_dicWindSensors, dicWindSenosrs);
        }

        private void SetBuildingSensors(Dictionary<int, SensorTagInfo> dicBuildingSensors, Dictionary<int, List<SensorTagInfo>> dicZoneSensors)
        {
            string strZoneIDs = "";

            foreach (KeyValuePair<int, List<SensorTagInfo>> pair in dicZoneSensors)
            {
                if (strZoneIDs.Length == 0)
                    strZoneIDs = pair.Key.ToString();
                else
                    strZoneIDs += ", " + pair.Key.ToString();
            }

            if (strZoneIDs.Length == 0)
                return;

            string strSQL = "Select ID, BuildingID from Zone where ID in (" + strZoneIDs + ")";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;
            List<SensorTagInfo> sensors = null;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                VariousData<int> zoneID = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> buildingID = WebDBManager.GetIntField(arrResult[i + 1].ToString());

                if (zoneID == null || buildingID == null)
                    continue;

                if (dicZoneSensors.TryGetValue(zoneID.Data, out sensors))
                {
                    if (sensors.Count > 0)
                        dicBuildingSensors[buildingID.Data] = sensors[0];
                }
            }
        }

        private List<int> ToIDs(string strIDs)
        {
            int nID;
            List<int> ids = new List<int>();
            string[] tokens = strIDs.Split(',');

            foreach (string strID in tokens)
            {
                if (int.TryParse(strID.Trim(), out nID))
                {
                    if (ids.Contains(nID) == false)
                    {
                        ids.Add(nID);
                    }
                }
            }

            return ids;
        }

        private string GetEtcSensorTypes()
        {
            string strSensorTypes = ((int)IFacility.FacilityType.STRONG_WIND).ToString();
            strSensorTypes += ", " + ((int)IFacility.FacilityType.BLACKOUT).ToString();
            return strSensorTypes;
        }

        public SensorTagInfo GetPowerOffSensor(int nBuildingID)
        {
            SensorTagInfo sensor;

            if (m_dicPowerOffSensors.TryGetValue(nBuildingID, out sensor))
                return sensor;

            return null;
        }

        public SensorTagInfo GetWindSensor(int nBuildingID)
        {
            SensorTagInfo sensor;

            if (m_dicWindSensors.TryGetValue(nBuildingID, out sensor))
                return sensor;

            return null;
        }

        public bool IsActivate(SensorTagInfo sensor)
        {
            string strSQL = "Select DeActivate from SensorTagInfo where ID = " + sensor.ID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return false;

            string strDeActivate = WebDBManager.GetStringField(arrResult[0]);

            if (strDeActivate == null)
                return false;

            return strDeActivate == "N" || strDeActivate == "n";
        }
    }
}
