using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Collections;
using System.IO;

namespace IconEditor
{
    public class ZoneManager
    {
        private const string BuildingTag = "[Building]";
        private const string ZoneTag = "[Zone]";
        private const string AlarmZoneTag = "[AlarmZone]";
        private const string DataFileName = "une.dat";

        private Dictionary<int, UnE.Spatial.Building> m_dicBuildings = new Dictionary<int, UnE.Spatial.Building>();
        private Dictionary<int, UnE.Spatial.Zone> m_dicZones = new Dictionary<int, UnE.Spatial.Zone>();
        private Dictionary<int, UnE.Spatial.EquipmentZone> m_dicEquipZones = new Dictionary<int, UnE.Spatial.EquipmentZone>();
        private Dictionary<UnE.Spatial.Zone, List<UnE.Spatial.EquipmentZone>> m_dicZoneEquipZones = new Dictionary<UnE.Spatial.Zone, List<UnE.Spatial.EquipmentZone>>();
        private Dictionary<int, float> m_dicZoneElevation = new Dictionary<int, float>();

        public bool ReadZones()
        {
            string strSiteID = System.Configuration.ConfigurationManager.AppSettings.Get("devSiteID");
            WebDBManager dbMgr = null;

            if (strSiteID != null && strSiteID.Length > 0)
            {
                int nSiteID;

                if (int.TryParse(strSiteID.Trim(), out nSiteID))
                    dbMgr = new WebDBManager(nSiteID);
            }

            if (dbMgr != null)
            {
                ReadDB(dbMgr);
            }

            return ReadData();
        }

        public void ReadElevation()
        {
            m_dicZoneElevation.Clear();
            string strFileName = System.Configuration.ConfigurationManager.AppSettings.Get("zoneElevation");

            if (strFileName == null || strFileName.Length == 0)
                return;

            StreamReader reader = new StreamReader(strFileName, Encoding.UTF8);
            
            while (reader.EndOfStream == false)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    continue;

                string[] tokens = strLine.Split('\t');

                if (tokens.Count() != 2)
                    continue;

                int nZoneID;
                float fElevation;

                if (int.TryParse(tokens[0].Trim(), out nZoneID) && float.TryParse(tokens[1].Trim(), out fElevation))
                {
                    m_dicZoneElevation[nZoneID] = fElevation;
                }
            }

            reader.Close();
        }

        public bool GetElevation(UnE.Spatial.Zone zone, out float fElevation)
        {
            fElevation = 0.0f;

            if (m_dicZoneElevation.TryGetValue(zone.ID, out fElevation) == false)
                return false;

            return true;
        }

        private void ReadDB(WebDBManager dbMgr)
        {
            m_dicBuildings.Clear();
            m_dicZones.Clear();
            m_dicEquipZones.Clear();
            m_dicZoneEquipZones.Clear();

            if (ReadBuildings(dbMgr))
            {
                if (ReadZones(dbMgr))
                {
                    if (ReadEquipZones(dbMgr))
                        WriteData();
                }
            }
        }

        private bool ReadData()
        {
            m_dicBuildings.Clear();
            m_dicZones.Clear();
            m_dicEquipZones.Clear();
            m_dicZoneEquipZones.Clear();

            if (File.Exists(DataFileName) == false)
                return false;

            try
            {
                StreamReader reader = new StreamReader(DataFileName, Encoding.UTF8);
                bool readBuilding = false, readZone = false, readAlarmZone = false;
                // Building : 1, Zone : 2, AlarmZone = 3
                int nCurrentMode = 0;

                while (reader.EndOfStream == false)
                {
                    string strLine = reader.ReadLine().Trim();

                    if (strLine.Length == 0)
                        continue;

                    if (strLine == BuildingTag)
                    {
                        readBuilding = true;
                        nCurrentMode = 1;
                    }
                    else if (strLine == ZoneTag)
                    {
                        readZone = true;
                        nCurrentMode = 2;
                    }
                    else if (strLine == AlarmZoneTag)
                    {
                        readAlarmZone = true;
                        nCurrentMode = 3;
                    }
                    else
                    {
                        if (nCurrentMode == 1)
                            ReadBuilding(strLine);
                        else if (nCurrentMode == 2)
                            ReadZone(strLine);
                        else if (nCurrentMode == 3)
                            ReadAlarmZone(strLine);
                    }
                }

                reader.Close();
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        private void WriteData()
        {
            StreamWriter writer = new StreamWriter(DataFileName, false, Encoding.UTF8);

            WriteBuilding(writer);
            WriteZone(writer);
            WriteAlarmZone(writer);

            writer.Close();
        }

        private void WriteBuilding(StreamWriter writer)
        {
            writer.WriteLine(BuildingTag);

            foreach (KeyValuePair<int, UnE.Spatial.Building> pair in m_dicBuildings)
            {
                string strLine = string.Format("{0}\t{1}", pair.Value.ID, pair.Value.BuildingName);
                writer.WriteLine(strLine);
            }
        }

        private void ReadBuilding(string strLine)
        {
            string[] tokens = strLine.Split('\t');

            if (tokens.Count() < 2)
                return;

            int nBuildingID;

            if (int.TryParse(tokens[0].Trim(), out nBuildingID) == false)
                return;

            UnE.Spatial.Building building = new UnE.Spatial.Building();
            building.ID = nBuildingID;
            building.BuildingName = building.DisplayText = tokens[1].Trim();

            m_dicBuildings[nBuildingID] = building;
        }

        private void WriteZone(StreamWriter writer)
        {
            writer.WriteLine(ZoneTag);

            foreach (KeyValuePair<int, UnE.Spatial.Zone> pair in m_dicZones)
            {
                string strLine = string.Format("{0}\t{1}\t{2}\t{3}", pair.Value.ID, pair.Value.ZoneName, pair.Value.Building.ID, pair.Value.DisplayText);
                writer.WriteLine(strLine);
            }
        }

        private void ReadZone(string strLine)
        {
            string[] tokens = strLine.Split('\t');

            if (tokens.Count() < 4)
                return;

            UnE.Spatial.Building building;
            int nZoneID, nBuildingID;

            if (int.TryParse(tokens[0].Trim(), out nZoneID) == false)
                return;

            if (int.TryParse(tokens[2].Trim(), out nBuildingID) == false)
                return;

            if (m_dicBuildings.TryGetValue(nBuildingID, out building) == false)
                return;

            UnE.Spatial.Zone zone = new UnE.Spatial.Zone();
            zone.ID = nZoneID;
            zone.ZoneName = zone.BroadcastName = tokens[1].Trim();
            zone.Building = building;
            zone.DisplayText = tokens[3].Trim();

            building.FloorList.Add(zone);
            m_dicZones[nZoneID] = zone;
        }

        private void WriteAlarmZone(StreamWriter writer)
        {
            writer.WriteLine(AlarmZoneTag);

            foreach (KeyValuePair<int, UnE.Spatial.EquipmentZone> pair in m_dicEquipZones)
            {
                string strLine = string.Format("{0}\t{1}\t{2}\t{3}", pair.Value.ID, pair.Value.ZoneName, pair.Value.LinkedZone.ID, pair.Value.BroadcastName);
                writer.WriteLine(strLine);
            }
        }

        private void ReadAlarmZone(string strLine)
        {
            string[] tokens = strLine.Split('\t');

            if (tokens.Count() < 4)
                return;

            UnE.Spatial.Zone zone;
            int nEquipZoneID, nZoneID;

            if (int.TryParse(tokens[0].Trim(), out nEquipZoneID) == false)
                return;

            if (int.TryParse(tokens[2].Trim(), out nZoneID) == false)
                return;

            if (m_dicZones.TryGetValue(nZoneID, out zone) == false)
                return;

            UnE.Spatial.EquipmentZone equipZone = new UnE.Spatial.EquipmentZone();
            equipZone.ID = nEquipZoneID;
            equipZone.ZoneName = equipZone.DisplayText = tokens[1].Trim();
            equipZone.LinkedZone = zone;
            equipZone.BroadcastName = tokens[3].Trim();

            List<UnE.Spatial.EquipmentZone> zoneEquipZones;

            if (m_dicZoneEquipZones.TryGetValue(zone, out zoneEquipZones) == false)
            {
                zoneEquipZones = new List<UnE.Spatial.EquipmentZone>();
                m_dicZoneEquipZones[zone] = zoneEquipZones;
            }

            zoneEquipZones.Add(equipZone);
            m_dicEquipZones[nEquipZoneID] = equipZone;
        }

        private bool ReadBuildings(WebDBManager dbMgr)
        {
            string strSQL = "Select b.ID, BuildingName from Building as b, BuildingGroup as bg where b.BuildingGroupID = bg.ID and bg.SiteID = " + dbMgr.SiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strBuildingName = WebDBManager.GetStringField(arrResult[i + 1]);

                if (id == null || strBuildingName == null)
                    continue;

                UnE.Spatial.Building building = new UnE.Spatial.Building();
                building.ID = id.Data;
                building.BuildingName = building.DisplayText = strBuildingName;

                m_dicBuildings[building.ID] = building;
            }

            return true;
        }

        private bool ReadZones(WebDBManager dbMgr)
        {
            string strBuildingIDs = "";

            foreach (KeyValuePair<int, UnE.Spatial.Building> pair in m_dicBuildings)
            {
                if (strBuildingIDs.Length == 0)
                    strBuildingIDs = pair.Value.ID.ToString();
                else
                    strBuildingIDs += ", " + pair.Value.ID.ToString();
            }

            if (strBuildingIDs.Length == 0)
                return false;

            string strSQL = "Select ID, ZoneName, BuildingID, FloorIndex, AddFloor, SceneName from Zone as z, ZoneScene as zs where BuildingID in (" + strBuildingIDs + ") and z.ID = zs.ZoneID";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 5; i += 6)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strZoneName = WebDBManager.GetStringField(arrResult[i + 1]);
                VariousData<int> buildingID = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<int> floorIndex = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                VariousData<float> addFloor = WebDBManager.GetFloatField(arrResult[i + 4].ToString());
                string strSceneName = WebDBManager.GetStringField(arrResult[i + 5]);

                if (id == null || strZoneName == null || buildingID == null || floorIndex == null || strSceneName == null)
                    continue;

                UnE.Spatial.Building building;

                if (m_dicBuildings.TryGetValue(buildingID.Data, out building) == false)
                    continue;

                UnE.Spatial.Zone zone = new UnE.Spatial.Zone();
                zone.ID = id.Data;
                zone.ZoneName = zone.BroadcastName = strZoneName;
                zone.FloorIndex = floorIndex.Data;
                zone.Building = building;
                zone.DisplayText = strSceneName;

                building.FloorList.Add(zone);

                if (addFloor != null)
                {
                    if (zone.FloorIndex < 0)
                        zone.AddFloor = -addFloor.Data;
                    else
                        zone.AddFloor = addFloor.Data;
                }

                m_dicZones[zone.ID] = zone;
            }

            return true;
        }

        private bool ReadEquipZones(WebDBManager dbMgr)
        {
            string strSQL = "Select ID, ZoneName, LinkedZoneIDList, VolumeName from EquipmentZone as ez, EquipZoneVolume as ezv where ez.ID = ezv.EquipZoneID and ez.SiteID = " + dbMgr.SiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strZoneName = WebDBManager.GetStringField(arrResult[i + 1]);
                string strLinkedZoneIDList = WebDBManager.GetStringField(arrResult[i + 2]);
                string strVolumeName = WebDBManager.GetStringField(arrResult[i + 3]);

                if (id == null || strZoneName == null || strLinkedZoneIDList == null || strVolumeName == null)
                    continue;

                UnE.Spatial.Zone zone = null;
                string[] zoneIDs = strLinkedZoneIDList.Trim().Split(',');

                foreach (string strZoneID in zoneIDs)
                {
                    int nZoneID;

                    if (int.TryParse(strZoneID.Trim(), out nZoneID))
                    {
                        if (m_dicZones.TryGetValue(nZoneID, out zone))
                        {
                            if (zone.Building != null)
                                break;
                            else
                                zone = null;
                        }
                    }
                }

                if (zone == null)
                    continue;

                UnE.Spatial.EquipmentZone equipZone = new UnE.Spatial.EquipmentZone();
                equipZone.ID = id.Data;
                equipZone.ZoneName = equipZone.DisplayText = strZoneName;
                equipZone.FloorIndex = zone.FloorIndex;
                equipZone.AddFloor = zone.AddFloor;
                equipZone.Building = zone.Building;
                equipZone.LinkedZone = zone;
                equipZone.BroadcastName = strVolumeName;

                List<UnE.Spatial.EquipmentZone> zoneEquipZones = null;

                if (m_dicZoneEquipZones.TryGetValue(zone, out zoneEquipZones) == false)
                {
                    zoneEquipZones = new List<UnE.Spatial.EquipmentZone>();
                    m_dicZoneEquipZones[zone] = zoneEquipZones;
                }

                zoneEquipZones.Add(equipZone);
                m_dicEquipZones[equipZone.ID] = equipZone;
            }

            return true;
        }

        public List<UnE.Spatial.Building> GetBuildings()
        {
            return m_dicBuildings.Values.ToList();
        }
    }
}
