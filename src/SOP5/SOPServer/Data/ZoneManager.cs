using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using DBUtility;
using UnE.Spatial;

namespace SDMSServer
{
	class ZoneManager
	{
		private static ZoneManager m_Instance = null;
		public static ZoneManager Instance
		{
			get
			{
				if (m_Instance == null)
					m_Instance = new ZoneManager();
				return m_Instance;
			}
		}

        private int m_nSiteID = 1;
        protected ZoneManager()
        {
            m_nSiteID = NetworkServer.Instance.SiteID;
        }


		private Dictionary<int, Zone> m_dicZones = new Dictionary<int, Zone>();
		public Dictionary<int, Zone> DicZones
		{
			get { return m_dicZones; }
			set { m_dicZones = value; }
		}

		private Dictionary<int, Building> m_dicBuildings = new Dictionary<int, Building>();
		public Dictionary<int, Building> DicBuildings
		{
			get { return m_dicBuildings; }
			set { m_dicBuildings = value; }
		}

		private Dictionary<int, BuildingGroup> m_dicBuildingGroup = new Dictionary<int, BuildingGroup>();
		public Dictionary<int, BuildingGroup> DicBuildingGroup
		{
			get { return m_dicBuildingGroup; }
			set { m_dicBuildingGroup = value; }
		}

		private Dictionary<int, Zone> m_dicOutdoorZones = new Dictionary<int, Zone>();
		public Dictionary<int, Zone> DicOutdoorZones
		{
			get { return m_dicOutdoorZones; }
			set { m_dicOutdoorZones = value; }
		}

		//Building에 속해있는 Zone List(Building ID, Zone List)
		private Dictionary<int, ArrayList> m_dicBuildingZones = new Dictionary<int, ArrayList>();
		public Dictionary<int, ArrayList> DicBuildingZones
		{
			get { return m_dicBuildingZones; }
			set { m_dicBuildingZones = value; }
		}

        private Dictionary<int, EquipmentZone> m_dicEquipZones = new Dictionary<int, EquipmentZone>();
        public Dictionary<int, EquipmentZone> DicEquipZones
        {
            get { return m_dicEquipZones; }
            set { m_dicEquipZones = value; }
        }

        // Zone에 속해있는 EquipmentZone List
        private Dictionary<Zone, ArrayList> m_dicZoneEquipZones = new Dictionary<Zone, ArrayList>();

		private float dx = 121902.5858f; //120894.0548f + 1008.531f;
		public float Dx
		{
			get { return dx; }
			set { dx = value; }
		}
		private float dy = 157152.8453f; //157659.0963f - 506.251f;
		public float Dy
		{
			get { return dy; }
			set { dy = value; }
		}

		public void LoadBuildingData()
		{
			WebDBManager webDB = NetworkServer.Instance.DBManager;

			//string strSQL = "select Building.id, BuildingID, BuildingCode, BuildingName, BuildingGroupID, MaxFloor, MinFloor,"
			//		 + "BuildingGroup.GroupName, BuildingGroup.TextCenter, Building.BroadCastingText ";
			//strSQL += "from Building, BuildingGroup where Building.BuildingGroupID = BuildingGroup.ID";

            string szText = "SELECT bd.id, bd.BuildingID,  bd.BuildingCode, bd.BuildingName, bd.BuildingGroupID, bd.MaxFloor,bd. MinFloor," +
                            " bdg.GroupName, bdg.TextCenter, bd.BroadCastingText FROM Building as bd, BuildingGroup as bdg " +
                            " WHERE bd.BuildingGroupID = bdg.ID and bdg.SiteID = {0}";

            string strSQL = string.Format(szText, m_nSiteID);

			ArrayList arrResult = webDB.GetResultData(strSQL, 0);
			if (arrResult == null)
				return;

			int nResultCount = arrResult.Count;

			for (int i = 0; i < nResultCount - 9; i += 10)
			{
				try
				{
					int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
					string strBuildingID = WebDBManager.GetStringField(arrResult[i + 1], "");
					string strBuildingCode = WebDBManager.GetStringField(arrResult[i + 2], "");
					string strBuildingName = WebDBManager.GetStringField(arrResult[i + 3], "");
					int nBuildingGroupID = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
					int nMaxFloorID = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
					int nMinFloorID = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
					string strBuildingGroupName = WebDBManager.GetStringField(arrResult[i + 7], "");
					string strGroupNamePos = WebDBManager.GetStringField(arrResult[i + 8], "");
					string strBroadcastName = WebDBManager.GetStringField(arrResult[i + 9], "");

					if (strBroadcastName == null || strBroadcastName.Equals("null"))
					{
						strBroadcastName = strBuildingName;
					}
					else
					{
						int nIdx = strBroadcastName.IndexOf('*');
						if (nIdx != -1)
						{
							strBroadcastName = strBroadcastName.Substring(0, nIdx);
						}
					}

					Building building = new Building();

					if (m_dicBuildingGroup.ContainsKey(nBuildingGroupID))
					{
						building.BuildingGroup = m_dicBuildingGroup[nBuildingGroupID];
					}
					else
					{
						BuildingGroup group = new BuildingGroup();
						group.BuildingGroupName = strBuildingGroupName;
						group.GroupID = nBuildingGroupID;
						//group.BuildingList.Add(building);

                        if(strGroupNamePos != "" && strGroupNamePos != "null")
                        {
                            string[] xy = strGroupNamePos.Split(',');
                            float x, y;
                            float.TryParse(xy[0], out x);
                            float.TryParse(xy[1], out y);
                            group.TextCenterX = x;
                            group.TextCenterY = y;
                        }
						
						m_dicBuildingGroup[nBuildingGroupID] = group;
						building.BuildingGroup = group;
					}

					building.ID = nID;
					building.BuildingName = strBuildingName;
					building.MaxFloorIndex = nMaxFloorID;
					building.MinFloorIndex = nMinFloorID;
					building.BuildingCode = strBuildingCode;
					building.BuildingID = strBuildingID;
					building.BroadcastName = strBroadcastName;
					building.BuildingGroup.BuildingList.Add(building);

					m_dicBuildings[nID] = building;

				}
				catch (System.Exception)
				{
					//MessageBox.Show(ex.StackTrace);
				}
			}
		}

		public void LoadZones()
		{
			WebDBManager webDB = NetworkServer.Instance.DBManager;

            // update by mwkim 2016-05-11 : DisplayText 컬럼도 조회하도록 쿼리 수정
            string szText = "select id, ZoneName, BuildingID, FloorIndex, Boundary, DXFFileName, DXFAccessedTime, _3DFileName, _3DAccessedTime, BroadcastName, AddFloor, DisplayText " +
                            " from Zone where SiteID = {0}";
            
            string strSQL = string.Format(szText, m_nSiteID);

			ArrayList arrResult = webDB.GetResultData(strSQL, 0);

			if (arrResult == null)
				return;

			DateTime dtDefault = new DateTime();
			int nResultCount = arrResult.Count;

			for (int i = 0; i < nResultCount - 11; i += 12)
			{
				int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
				string strZoneName = WebDBManager.GetStringField(arrResult[i + 1], "");
				int nBuildingID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
				int nFloorIndex = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
				string strBoundary = WebDBManager.GetStringField(arrResult[i + 4], "");
				string strDXFFileName = WebDBManager.GetStringField(arrResult[i + 5], "");
				DateTime dtDXF = WebDBManager.GetDateTimeField(arrResult[i + 6], dtDefault);
				string str3DFileName = WebDBManager.GetStringField(arrResult[i + 7], "");
				DateTime dt3D = WebDBManager.GetDateTimeField(arrResult[i + 8], dtDefault);
				string strBroadcastName = WebDBManager.GetStringField(arrResult[i + 9], "");
				string strAddFloor = WebDBManager.GetStringField(arrResult[i + 10], "0.0");
                string strDisplayText = WebDBManager.GetStringField(arrResult[i + 11], "");

				Zone zone = new Zone();

				zone.ID = nID;
				zone.ZoneName = strZoneName;
				zone.FloorIndex = nFloorIndex;

                if (String.Equals(strBroadcastName.ToUpper(), "NULL") || strBroadcastName == "")
                    zone.BroadcastName = strZoneName;
                else
                    zone.BroadcastName = strBroadcastName;

                if (String.Equals(strDisplayText.ToUpper(), "NULL") || String.IsNullOrWhiteSpace(strDisplayText))
                    zone.DisplayText = strDisplayText;
                else
                    zone.DisplayText = strDisplayText;

				if (m_dicBuildings.ContainsKey(nBuildingID))
				{
					zone.Building = m_dicBuildings[nBuildingID];
					zone.Building.FloorList.Add(zone);
				}
				
				//지하나 .2.5인 층들 
				try
				{
                    //strAddFloor가 비었다면 0.0f
                    if (strAddFloor.Length == 0 || strAddFloor == "null")
                        zone.AddFloor = 0.0f;
                    else
					    zone.AddFloor = float.Parse(strAddFloor);
				}
				catch (Exception)
				{
					zone.AddFloor = 0.0f;
				}

				zone.Floor.FloorIndex = (zone.FloorIndex + zone.AddFloor);

				m_dicZones[nID] = zone;
				if (nBuildingID < 0)
					m_dicOutdoorZones[nID] = zone;

				if (zone.Building != null)
				{
					if (m_dicBuildingZones.ContainsKey(zone.Building.ID))
					{
						ArrayList arrZones = m_dicBuildingZones[zone.Building.ID];
						arrZones.Add(zone);
					}
					else
					{
						ArrayList arrZone = new ArrayList();
						m_dicBuildingZones[zone.Building.ID] = arrZone;
						arrZone.Add(zone);
					}
				}
			}
		}

        public void LoadEquipmentZones()
        {
            WebDBManager dbMgr = NetworkServer.Instance.DBManager;

            // update by mwkim 2016-05-11 : DisplayText 컬럼도 로드하도록 쿼리 수정
            string szText = "SELECT ID, ZoneName, LinkedZoneIDList, Type, BroadcastName, DisplayText FROM EquipmentZone where ID > 0 AND SiteID = {0}";
            
            string strSQL = string.Format(szText, m_nSiteID);

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 5; i += 6)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strEquipZoneName = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strLinkedZoneIDList = WebDBManager.GetStringField(arrResult[i + 2], "");
                int nType = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                string strBroadcastName = WebDBManager.GetStringField(arrResult[i + 4], "");
                string strDisplayText = WebDBManager.GetStringField(arrResult[i + 5], "");

                if (nID < 0)
                    continue;

                ArrayList arrLinkedZones = GetZoneObjectList(strLinkedZoneIDList);
                if (arrLinkedZones == null)
                    continue;

                EquipmentZone equipZone = new EquipmentZone();

                equipZone.ID = nID;
                equipZone.ZoneName = strEquipZoneName;
                equipZone.ZoneType = (EquipmentZone.EquipZoneType)nType;
                equipZone.BroadcastName = strBroadcastName;
                equipZone.DisplayText = strDisplayText;

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

        private ArrayList GetZoneObjectList(string strZoneIDList)
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

            if (!m_dicZones.ContainsKey(nZoneID))
                return false;

            Zone zone = m_dicZones[nZoneID];

            if (!arrZones.Contains(zone))
                arrZones.Add(zone);

            return true;
        }

		public ArrayList GetZoneList(int nBuildingID)
		{
			if (DicBuildingZones.ContainsKey(nBuildingID))
				return DicBuildingZones[nBuildingID];

			return null;
		}

		public ArrayList GetZoneList(string buildingID)
		{
			ArrayList arList = new ArrayList();
			foreach (KeyValuePair<int, Zone> kv in m_dicZones)
			{
				Zone obj = kv.Value;
				if (obj.Building != null && buildingID == obj.Building.BuildingID)
				{
					arList.Add(obj);
				}
			}
			return arList;
		}

		public Zone GetZone(string buildingID, float floorIndex)
		{
			Building building = GetBuilding(buildingID);
			if (building == null)
				return null;

			foreach (Zone zone in building.FloorList)
			{
				if (zone != null && zone.Floor.FloorIndex == floorIndex)
				{
					return zone;
				}
			}
			return null;
		}		

		public Zone GetZone(int nZoneID)
		{
			if (m_dicZones.ContainsKey(nZoneID))
				return m_dicZones[nZoneID];

			return null;
		}

		public string GetBuildingID(int nID)
		{
			if (!m_dicBuildings.ContainsKey(nID))
				return "";
			Building b = m_dicBuildings[nID];
			return b.BuildingID;
		}

		public Building GetBuilding(string szBuildingID)
		{
			foreach (KeyValuePair<int, Building> kv in m_dicBuildings)
			{
				Building obj = kv.Value;
				if (szBuildingID == obj.BuildingID)
				{
					return obj;
				}
			}
			return null;
		}

		public Zone FindZone(Building building, string strFloor)
		{
			Dictionary<int, Building> dicBuildings = ZoneManager.Instance.DicBuildings;
			foreach (KeyValuePair<int, Building> pair in dicBuildings)
			{
				Building obj = pair.Value;
				if (obj.BuildingID == building.BuildingID)
				{
					ArrayList arZone = GetZoneList(building.ID);
					if (arZone == null)
						return null;
					foreach (Zone zone in arZone)
					{
						if (strFloor == zone.Floor.ToString())
							return zone;
					}
				}
			}
			return null;
		}

        public ArrayList GetEquipmentZoneList(Zone zone)
        {
            if (m_dicZoneEquipZones.ContainsKey(zone))
                return m_dicZoneEquipZones[zone];

            return null;
        }

        public EquipmentZone GetEquipmentZone(int nEquipZoneID)
        {
            if (m_dicEquipZones.ContainsKey(nEquipZoneID))
                return m_dicEquipZones[nEquipZoneID];

            return null;
        }
	}
}
