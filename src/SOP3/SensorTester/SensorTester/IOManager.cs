using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using DBUtility;

namespace SensorTester
{
    public class IOManager
    {
        //BuildingGroup
        private Dictionary<BuildingGroup, ArrayList> d_BuildingGroups = new Dictionary<BuildingGroup, ArrayList>();
        internal Dictionary<BuildingGroup, ArrayList> D_BuildingGroups
        {
            get { return d_BuildingGroups; }
            set { d_BuildingGroups = value; }
        }

        //Building(Building ID, Building)
        private Dictionary<int, Building> d_Building = new Dictionary<int, Building>();

        //Building에 속해있는 Zone List(Building ID, Zone List)
        private Dictionary<int, ArrayList> d_BuildingZones = new Dictionary<int, ArrayList>();

        public Dictionary<int, ArrayList> D_BuildingZones
        {
            get { return d_BuildingZones; }
            set { d_BuildingZones = value; }
        }

        //Zone(Zone ID, Zone)
        private Dictionary<int, Zone> d_Zones = new Dictionary<int, Zone>();
        public Dictionary<int, Zone> D_Zones
        {
            get { return d_Zones; }
            set { d_Zones = value; }
        }

        //BuildingID가 -1인 외부 공간들...
        private Dictionary<int, Zone> d_OutdoorZones = new Dictionary<int, Zone>();

        //EquipmentZone에 속해있는 SensorZone List(EquipmentZone, SensorZone List)
        private Dictionary<EquipmentZone, ArrayList> d_EquipZoneSensor = new Dictionary<EquipmentZone, ArrayList>();
        public Dictionary<EquipmentZone, ArrayList> D_EquipZoneSensor
        {
            get { return d_EquipZoneSensor; }
            set { d_EquipZoneSensor = value; }
        }

        //SensorZone(SensorZone ID, SensorZone)
        private Dictionary<int, SensorZone> d_SensorZone = new Dictionary<int, SensorZone>();

        private Dictionary<int, EquipmentZone> m_dicEquipZones = new Dictionary<int, EquipmentZone>();

        // Zone에 속해있는 EquipmentZone List
        private Dictionary<Zone, ArrayList> m_dicZoneEquipZones = new Dictionary<Zone, ArrayList>();

        public IOManager()
        {
            LoadBuildings();
            LoadZones();
            LoadEquipmentZone();
            LoadSensorZone();

			LoadReciverList();
        }
        

        public void LoadBuildings()
        {
            
            Dictionary<int, BuildingGroup> dic_BuildingGroup = new Dictionary<int, BuildingGroup>();

            WebDBManager m_dbMgr = FormMain.Instance.DbMgr;
            
            //string strSQL = "select Building.ID as f1, BuildingID as f2, BuildingCode as f3, BuildingName as f4, BuildingGroupID as f5, MaxFloor, MinFloor, BuildingGroup.GroupName as f6 "
            //    + "from Building, BuildingGroup where Building.BuildingGroupID = BuildingGroup.ID";

            string strSQL = "select Building.id, BuildingID, BuildingCode, BuildingName, BuildingGroupID, MaxFloor, MinFloor, BuildingGroup.GroupName ";
            strSQL += "from Building, BuildingGroup where Building.BuildingGroupID = BuildingGroup.ID";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            int nResultCount = arrResult.Count;

            if (arrResult == null)
                return;

            for (int i = 0; i < nResultCount - 7; i += 8)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strBuildingID = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strBuildingCode = WebDBManager.GetStringField(arrResult[i + 2], "");
                string strBuildingName = WebDBManager.GetStringField(arrResult[i + 3], "");
                int nBuildingGroupID = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                int nMaxFloorID = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                int nMinFloorID = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                string strBuildingGroupName = WebDBManager.GetStringField(arrResult[i + 7], "");

                Building building = new Building();

                //BuildingGroup
                if (dic_BuildingGroup.ContainsKey(nBuildingGroupID))
                    building.BuildingGroup = dic_BuildingGroup[nBuildingGroupID];
                else
                {
                    BuildingGroup group = new BuildingGroup();
                    group.ID = nBuildingGroupID;
                    group.BuildingGroupName = strBuildingGroupName;

                    dic_BuildingGroup[nBuildingGroupID] = group;
                    building.BuildingGroup = group;
                }

                building.ID = nID;
                building.BuildingName = strBuildingName;
                //building.MaxFloorIndex = nMaxFloorID;
                //building.MinFloorIndex = nMinFloorID;
                //building.BuildingCode = strBuildingCode;
                //building.BuildingID = strBuildingID;

                //Building, 사전에 데이터를 집어넣음
                d_Building[nID] = building;

                //BuildingGroup & ArrayList
                if (d_BuildingGroups.ContainsKey(building.BuildingGroup))
                {
                    ArrayList arrBuildings = d_BuildingGroups[building.BuildingGroup];
                    arrBuildings.Add(building);
                }
                else
                {
                    ArrayList arrBuildings = new ArrayList();
                    arrBuildings.Add(building);

                    d_BuildingGroups[building.BuildingGroup] = arrBuildings;
                }
            }
            // 외부 공간을 위한 BuildingGroup
            //BuildingGroup outdoorGroup = new BuildingGroup();
            //outdoorGroup.BuildingGroupName = "외부 공간";
            //d_BuildingGroups[outdoorGroup] = new ArrayList();
        }

        public void LoadZones()
        {
            WebDBManager m_dbMgr = FormMain.Instance.DbMgr;
            //WebDBManager m_dbMgr = new WebDBManager();

            string strSQL = "select id, ZoneName, BuildingID, FloorIndex, AddFloor, Boundary, DXFFileName from Zone";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 6; i += 7)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strZoneName = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nBuildingID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nFloorIndex = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                string strAddFloor = WebDBManager.GetStringField(arrResult[i + 4], "0.0");
                string strBoundary = WebDBManager.GetStringField(arrResult[i + 5], "");
                string strDXFFileName = WebDBManager.GetStringField(arrResult[i + 6], "");
                //DateTime dtDXF = dbMgr.GetDateTimeField(arrResult[i + 7], dtDefault);
                //string str3DFileName = reader[7].ToString();
                //DateTime dt3D = dbMgr.GetDateTimeField(arrResult[i + 9], dtDefault);

                Zone zone = new Zone();

                zone.ID = nID;
                zone.ZoneName = strZoneName;
                zone.FloorIndex = nFloorIndex;

                //지하나 .2.5인 층들 
                try
                {
                    //strAddFloor가 비었다면 0.0f
                    zone.AddFloor = string.Compare(strAddFloor, "null", true) == 0 ? 0.0f : float.Parse(strAddFloor);
                }
                catch (Exception)
                {
                    zone.AddFloor = 0.0f;
                }

                if (d_Building.ContainsKey(nBuildingID))
                    zone.Building = d_Building[nBuildingID];

                d_Zones[nID] = zone;

                //외부공간
                if (nBuildingID < 0)
                    d_OutdoorZones[nID] = zone;

                //
                if (zone.Building != null)
                {
                    if (d_BuildingZones.ContainsKey(zone.Building.ID))
                    {
                        ArrayList arrZones = d_BuildingZones[zone.Building.ID];
                        arrZones.Add(zone);
                    }
                    else
                    {
                        ArrayList arrZone = new ArrayList();
                        d_BuildingZones[zone.Building.ID] = arrZone;
                        arrZone.Add(zone);
                    }
                }
            }
        }

        public void LoadSensorZone()
        {
            WebDBManager m_dbMgr = FormMain.Instance.DbMgr;
            //WebDBManager m_dbMgr = new WebDBManager();

            string strSQL = "select ID,Type, Connected, EquipZoneID, Data from SensorZone";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 4; i += 5)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nType = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nConnected = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nEquipZoneID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                int nData = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);

                EquipmentZone equipZone = null;

                if (m_dicEquipZones.ContainsKey(nEquipZoneID))
                    equipZone = m_dicEquipZones[nEquipZoneID];
                else
                    continue;

                SensorZone sensorZone = new SensorZone();

                sensorZone.ID = nID;
                sensorZone.Type = nType;
                sensorZone.EquipZone = equipZone;

                d_SensorZone[nID] = sensorZone;

                if (d_EquipZoneSensor.ContainsKey(equipZone))
                {
                    ArrayList arrZones = d_EquipZoneSensor[equipZone];
                    arrZones.Add(sensorZone);
                }
                else
                {
                    ArrayList arrZones = new ArrayList();
                    arrZones.Add(sensorZone);

                    d_EquipZoneSensor[equipZone] = arrZones;
                }
            }
        }

        public void LoadEquipmentZone()
        {
            WebDBManager m_dbMgr = FormMain.Instance.DbMgr;

            string strSQL = "select ID, ZoneName, LinkedZoneIDList, Type from EquipmentZone";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strEquipZoneName = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strLinkedZoneIDList = WebDBManager.GetStringField(arrResult[i + 2], "");
                int nType = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);

                if (nID < 0)
                    continue;

                ArrayList arrLinkedZones = GetZoneList(strLinkedZoneIDList);
                if (arrLinkedZones == null)
                    continue;

                if (nType < (int)EquipmentZone.EquipZoneType.SENSOR_TYPE ||
                    nType > (int)EquipmentZone.EquipZoneType.FA_TYPE)
                    continue;

                EquipmentZone equipZone = new EquipmentZone();

                equipZone.ID = nID;
                equipZone.EquipZoneName = strEquipZoneName;
                equipZone.Type = (EquipmentZone.EquipZoneType)nType;

                foreach (Zone zone in arrLinkedZones)
                {
                    equipZone.LinkedZoneList.Add(zone);
                    ArrayList arrEquipZones = null;

                    if (m_dicZoneEquipZones.ContainsKey(zone))
                    {
                        arrEquipZones = m_dicZoneEquipZones[zone];
                    }
                    else
                    {
                        arrEquipZones = new ArrayList();
                        m_dicZoneEquipZones[zone] = arrEquipZones;
                    }

                    if (!arrEquipZones.Contains(equipZone))
                        arrEquipZones.Add(equipZone);
                }

                m_dicEquipZones[nID] = equipZone;
            }
        }

        private ArrayList GetZoneList(string strZoneIDList)
        {
            int nBeginIndex = 0;
            int nIndex = strZoneIDList.IndexOf(',', nBeginIndex);
            int nLen = strZoneIDList.Length;

            ArrayList arrZones = new ArrayList();

            while (nIndex >= 0)
            {
                string strZoneID = strZoneIDList.Substring(nBeginIndex, nIndex - nBeginIndex);
                
                if (!AddZone(strZoneID, arrZones))
                    return null;

                int nPrevBeginIndex = nBeginIndex;

                for (int i = nIndex + 1; i < nLen; i++)
                {
                    char ch = strZoneIDList.ElementAt(i);

                    if (ch != ' ' && ch != '\t' && ch != '\r' && ch != '\n')
                    {
                        nBeginIndex = i;
                        break;
                    }
                }

                if (nPrevBeginIndex == nBeginIndex)
                    return null;

                nIndex = strZoneIDList.IndexOf(',', nBeginIndex);
            }

            string strZoneID2 = strZoneIDList.Substring(nBeginIndex);
            bool result = AddZone(strZoneID2, arrZones);

            return result ? arrZones : null;
        }

        private bool AddZone(string strZoneID, ArrayList arrZones)
        {
            int nZoneID;

            if (!int.TryParse(strZoneID, out nZoneID))
                return false;

            if (!d_Zones.ContainsKey(nZoneID))
                return false;

            Zone zone = d_Zones[nZoneID];

            if (!arrZones.Contains(zone))
                arrZones.Add(zone);

            return true;
        }

        public ArrayList GetEquipmentZoneList(Zone zone)
        {
            if (m_dicZoneEquipZones.ContainsKey(zone))
                return m_dicZoneEquipZones[zone];

            return null;
        }

		private ArrayList m_arReciverList = new ArrayList();
		public ArrayList GetReciverList()
		{
			return m_arReciverList;
		}

		public void LoadReciverList()
		{			
			object [,] recivers = {
				{ 1,  "경영지원처 당직실",				"172.18.101.110" },
				{ 2,  "발전운영처",						"172.18.101.111" },
				{ 3,  "발전기술처",						"172.18.101.112" },
				{ 4,  "#1,2 중앙제어실_자탐",			"172.18.101.113" },
				{ 5,  "#1,2 중앙제어실_CO2",			"172.18.101.114" },
				{ 6,  "#1,2 중앙제어실_분말, 포소화",	"172.18.101.115" },
				{ 7,  "#1 터빈건물 1층_물분무",			"172.18.101.116" },	
				{ 8,  "#2 터빈건물 1층_물분무",			"172.18.101.117" },
				{ 9,  "#1,2 자재창고_자탐",				"172.18.101.118" },
				{ 10, "#1,2 EP제어건물_자탐, 하론",		"172.18.101.119" },
				{ 11, "#1,2 운탄제어실_스프링쿨러",		"172.18.101.120" },
				{ 12, "#1,2 EP제어건물_CO2",			"172.18.101.121" },
				{ 13, "#3,4 중앙제어실_자탐",			"172.18.101.122" },
				{ 14, "#3,4 중앙제어실_CO2",			"172.18.101.123" },
				{ 15, "#3,4 중앙제어실_분말",			"172.18.101.124" },
				{ 16, "#3,4 중앙제어실_하론",			"172.18.101.125" },
				{ 17, "#3 터빈건물 1층_물분무",			"172.18.101.126" },
				{ 18, "#4 터빈건물 1층_물분무",			"172.18.101.127" },
				{ 19, "#3,4 Cable Vault Room_물분무",	"172.18.101.128" },
				{ 20, "#3,4 운전원실_CO2",				"172.18.101.129" },
				{ 21, "탈황제어실_R형 수신기",			"172.18.101.130" },
				{ 22, "#5,6 중앙제어실_자탐",			"172.18.101.131" },
				{ 23, "#5,6 중앙제어실_물분무",			"172.18.101.132" },
				{ 24, "#5,6 중앙제어실_가스계",			"172.18.101.133" },
				{ 25, "#5,6 운탄제어실_이보접점",		"172.18.101.134" }
			};

			for (int i = 0; i < 25; i++)
			{
				Reciver reciver = new Reciver();
				reciver.ID = (int)recivers[i,0];
				reciver.UserText = (string)recivers[i, 1];
				reciver.Address = (string)recivers[i, 2];
				reciver.Port = 4000;
				reciver.EquipZoneID = i;
				m_arReciverList.Add(reciver);
			}
		}
    }
}
