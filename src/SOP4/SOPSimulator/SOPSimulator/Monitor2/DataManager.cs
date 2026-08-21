using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using SOPMonitoringSystem;
using UnE.SOP;

namespace SOPMonitoringSystem
{
	public class DataManager
	{
		private static DataManager m_Instance = null;
		public static DataManager Instance
		{
			get
			{
				if (m_Instance == null)
					m_Instance = new DataManager();
				return m_Instance;
			}
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

        // Zone에 속해있는 EquipmentZone List
        private Dictionary<Zone, ArrayList> m_dicZoneEquipZones = new Dictionary<Zone, ArrayList>();
        private Dictionary<int, EquipmentZone> m_dicEquipZones = new Dictionary<int, EquipmentZone>();

        private Dictionary<int, SecurityFacilityType> m_dicSecurityFacilityType = new Dictionary<int, SecurityFacilityType>();
        public Dictionary<int, SecurityFacilityType> DicSecurityFacilityType
        {
            get { return m_dicSecurityFacilityType; }
            set { m_dicSecurityFacilityType = value; }
        }

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

        private Dictionary<int, Data_NormalTeam> m_dicTemporaryNormalTeams = new Dictionary<int, Data_NormalTeam>();
        private Dictionary<int, Data_EmergencyTeam> m_dicTemporaryEmergencyTeams = new Dictionary<int, Data_EmergencyTeam>();

        private int m_nSiteID = 1;
        public void Init()
        {
            m_nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;

            LoadBuildingData();
            LoadZones();
            LoadEquipZones();
            LoadSecurityFacilityType();
            LoadTemporaryTeams();
        }

		public ArrayList LoadFireEquipment(string BuildingID, int nFloorIdx)
		{
            //string szSQP = "SELECT ID, EquipID, EquipType, ZoneID, X, Y, Z, Description FROM FireEquipment";

            //if (FormSOP.Instance.SimulationMode)
            //{
            //    // SQLite 문법
            //    string subSQL = string.Format(" where EquipType = '1' and ZoneID = ( select Zone.ID from Zone where Zone.BuildingID = " +
            //        "( SELECT ID FROM Building where BuildingID = '{0}') and FloorIndex = '{1}' LIMIT 0, 1)", BuildingID, nFloorIdx);
            //    szSQP += subSQL;
            //}
            //else
            //{
            //    // MS-SQL 문법
            //    szSQP = "SELECT ID, EquipID, EquipType, ZoneID, X, Y, Z, Description FROM FireEquipment";
            //    string subSQL = string.Format(" where EquipType = '1' and ZoneID = ( select Zone.ID from Zone where Zone.BuildingID = " +
            //        "( SELECT TOP 1 ID FROM Building where BuildingID = '{0}') and FloorIndex = '{1}')", BuildingID, nFloorIdx);
            //    szSQP += subSQL;
            //}

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT fe.ID, fe.EquipID, fe.EquipType, fe.ZoneID, fe.X, fe.Y, fe.Z, fe.Description FROM FireEquipment as fe ");
            sb.AppendFormat(" inner join Zone as z ON fe.ZoneID = z.ID and z.SiteID = {0} and z.FloorIndex = {1} and z.AddFloor is null and fe.EquipType = '1'", UnE.SOP.ProxySOP.Instance.SiteID, nFloorIdx);
            sb.AppendFormat(" inner join Building as bd On bd.ID = z.BuildingID and bd.BuildingID = '{0}'", BuildingID);

            string szSQL = sb.ToString();

			SOPMonitoringSystem.WebDBManager webDB = SOPMonitoringSystem.FormSOP.Instance.DBManager;

            ArrayList arrResult = webDB.GetResultData(szSQL, 0);
			if (arrResult == null)
				return null;

			int nResultCount = arrResult.Count;

			ArrayList arResult = new ArrayList();
			for (int i = 0; i < nResultCount - 7; i += 8)
			{
				try
				{
					int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
					string szEquipID = WebDBManager.GetStringField(arrResult[i + 1], "");
					int nEquipType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
					int nZoneID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
					float fX = WebDBManager.GetFloatField(arrResult[i + 4].ToString(), 0.0f);
					float fY = WebDBManager.GetFloatField(arrResult[i + 5].ToString(), 0.0f);
					float fZ = WebDBManager.GetFloatField(arrResult[i + 6].ToString(), 0.0f);
					string szDesc = WebDBManager.GetStringField(arrResult[i + 7], "");

					FireEquipment equip = new FireEquipment();
					equip.ID = nID;
					equip.EquipID = szEquipID;
					equip.ZoneID = nZoneID;
					equip.X = fX;
					equip.Y = fY;
					equip.Z = fZ;
					equip.Description = szDesc;
					equip.BuildingID = BuildingID;
					equip.FloorIndex = nFloorIdx;

					arResult.Add(equip);
				}
				catch (Exception)
				{
					return null;
				}
			}
			return arResult;
		}

		public void LoadBuildingData()
		{
			WebDBManager webDB = SOPMonitoringSystem.FormSOP.Instance.DBManager;

			//string strSQL = "select Building.id, BuildingID, BuildingCode, BuildingName, BuildingGroupID, MaxFloor, MinFloor,"
			//		 + "BuildingGroup.GroupName, BuildingGroup.TextCenter, Building.BroadCastingText ";
			//strSQL += "from Building, BuildingGroup where Building.BuildingGroupID = BuildingGroup.ID";


            string szText = "SELECT bd.id, bd.BuildingID,  bd.BuildingCode, bd.BuildingName, bd.BuildingGroupID, bd.MaxFloor, " +
                            " bd.MinFloor, bg.GroupName, bg.DisplayText, bg.TextCenter, bd.BroadCastingText, bd.DisplayText FROM Building as bd " +
                            " INNER JOIN BuildingGroup as bg ON bd.BuildingGroupID = bg.ID AND bg.SiteID = {0}";

            string strSQL = string.Format(szText, m_nSiteID);

			ArrayList arrResult = webDB.GetResultData(strSQL, 0);
			if (arrResult == null)
				return;

			int nResultCount = arrResult.Count;

			for (int i = 0; i < nResultCount - 11; i += 12)
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
                    string strBuildingGroupDisplayName = WebDBManager.GetStringField(arrResult[i + 8], "");
					string strGroupNamePos = WebDBManager.GetStringField(arrResult[i + 9], "");                    
					string strBroadcastName = WebDBManager.GetStringField(arrResult[i + 10], "");
                    string strDisplayText = WebDBManager.GetStringField(arrResult[i + 11], "");

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

                    if (strBuildingGroupDisplayName == null || strBuildingGroupDisplayName.Equals("null"))
                        strBuildingGroupDisplayName = "";

					Building building = new Building();

                    if (m_dicBuildingGroup.ContainsKey(nBuildingGroupID))
                    {
                        building.BuildingGroup = m_dicBuildingGroup[nBuildingGroupID];
                    }
                    else
                    {
                        BuildingGroup group = new BuildingGroup();
                        group.BuildingGroupName = strBuildingGroupName;
                        group.DisplayName = strBuildingGroupDisplayName;
                        group.GroupID = nBuildingGroupID;
                        //group.BuildingList.Add(building);

                        string[] xy = strGroupNamePos.Split(',');
                        float x = 0.0f, y = 0.0f;
                        if( xy.Length == 2)
                        {                            
                            float.TryParse(xy[0], out x);
                            float.TryParse(xy[1], out y);
                        }                       
                        group.TextCenterX = x;
                        group.TextCenterY = y;
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
                    building.DisplayText = strDisplayText;

                    building.BuildingGroup.BuildingList.Add(building);

					m_dicBuildings[nID] = building;

				}
				catch (System.Exception ex)
				{
					MessageBox.Show(ex.StackTrace);
				}
			}
		}

        public void LoadSecurityFacilityType()
        {
            WebDBManager webDB = SOPMonitoringSystem.FormSOP.Instance.DBManager;
             
            string szText = "SELECT st.ID, SubCategoryName, st.ID as SecurityType, FacilityTypeIDs" +
                            "  FROM subdisastercategory sub" +
                            " INNER JOIN securitytypetable st ON sub.ID=st.SecurityType";

            string strSQL = string.Format(szText, m_nSiteID);

            ArrayList arrResult = webDB.GetResultData(strSQL, 0);
            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount; i += 4)
            {
                try
                {
                    int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    string strSubCategoryName = WebDBManager.GetStringField(arrResult[i + 1], ""); 
                    int nSecurityType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1); 
                    string strFacilityTypeIDs = WebDBManager.GetStringField(arrResult[i + 3], "");
                     
                    if (m_dicSecurityFacilityType.ContainsKey(nID)) continue;

                    SecurityFacilityType security = new SecurityFacilityType();
                    ArrayList fList = new ArrayList();

                    string[] facilityTypeIDs = strFacilityTypeIDs.Split(',');
                    foreach (string item in facilityTypeIDs)
                    {
                        fList.Add(item);
                    }
                    m_dicSecurityFacilityType[nID] = security; 

                    security.ID = nID;
                    security.SubCategoryName = strSubCategoryName;
                    security.SecurityType = nSecurityType; 
                    security.FacilityTypeIDs.Add(fList); 
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.StackTrace);
                }
            }
        }

        private UnE.Geometry.Polygon MakeZonePolygon(string szBoundary)
        {
            if (szBoundary == null || szBoundary == "")
                return null;

            string[] arrTokens = szBoundary.Split('#');
            List<UnE.Geometry.Polygon> polygons = new List<UnE.Geometry.Polygon>();

            foreach (string strToken in arrTokens)
            {
                UnE.Geometry.Polygon polygon = StringToPolygon(strToken.Trim());

                if (polygon == null)
                    break;
            }

            // Zone의 Polygon은 여러개일 수 있는데, 첫번째 Polygon만 리턴한다.
            if (polygons.Count == 0)
                return null;

            return polygons[0];
        }

        private UnE.Geometry.Polygon StringToPolygon(string szBoundary)
        {
            UnE.Geometry.Polygon poly = new UnE.Geometry.Polygon();
            int start_idx = 0;
            bool bEnd = false;
            do
            {
                int idx = szBoundary.IndexOf(',', start_idx);
                if (idx == -1)
                    break;
                string szPosX = szBoundary.Substring(start_idx, idx - start_idx);
                start_idx = idx + 1;

                idx = szBoundary.IndexOf(',', start_idx);
                string szPosY = "";

                if (idx == -1)
                {
                    int nLength = szBoundary.Length - start_idx;
                    szPosY = szBoundary.Substring(start_idx, nLength);
                    bEnd = true;
                }
                else
                    szPosY = szBoundary.Substring(start_idx, idx - start_idx);

                start_idx = idx + 1;
                double x = Double.Parse(szPosX);
                double y = Double.Parse(szPosY);
                UnE.Geometry.Vertex2D pos = new UnE.Geometry.Vertex2D(x, y);

                float pos3DX = ((float)x - dx);
                float pos3DZ = dy + (float)y;

                poly.AddVertex(pos);
                if (bEnd == true)
                    break;
            } while (start_idx < szBoundary.Length);
            return poly;
        }

		public void LoadZones()
		{
			WebDBManager webDB = SOPMonitoringSystem.FormSOP.Instance.DBManager;

			string strSQL = "select id, ZoneName, BuildingID, FloorIndex, Boundary, DXFFileName, DXFAccessedTime, _3DFileName, _3DAccessedTime, BroadcastName, AddFloor,DisplayText from Zone WHERE SiteID = " + m_nSiteID.ToString();

			ArrayList arrResult = webDB.GetResultData(strSQL, 0);

			if (arrResult == null)
				return;

			DateTime dtDefault = new DateTime();
			int nResultCount = arrResult.Count;

			for (int i = 0; i < nResultCount -11; i += 12)
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
                string szDisplayText = WebDBManager.GetStringField(arrResult[i + 11], "");
				Zone zone = new Zone();

				zone.ID = nID;
				zone.ZoneName = strZoneName;
                zone.DisplayName = szDisplayText;
				//zone.FloorIndex = nFloorIndex;
				
				if (strBroadcastName == "null" || strBroadcastName == "")
					zone.BroadcastName = strZoneName;
				else
					zone.BroadcastName = strBroadcastName;


				if (m_dicBuildings.ContainsKey(nBuildingID))
				{
					zone.Building = m_dicBuildings[nBuildingID];
					zone.Building.FloorList.Add(zone);
				}

				try
				{
					zone.Polygon = MakeZonePolygon(strBoundary);
				}
				catch (System.Exception)
				{
					//MessageBox.Show("Make polygo error!!");
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

				zone.Floor.FloorIndex = (nFloorIndex + zone.AddFloor);

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

        public void LoadEquipZones()
        {
            WebDBManager dbMgr = SOPMonitoringSystem.FormSOP.Instance.DBManager;

            string strSQL = "select ID, ZoneName, LinkedZoneIDList, Type, BroadcastName from EquipmentZone where SiteID = " + m_nSiteID.ToString();

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 4; i += 5)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strEquipZoneName = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strLinkedZoneIDList = WebDBManager.GetStringField(arrResult[i + 2], "");
                int nType = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
				string strBroadcastName = WebDBManager.GetStringField(arrResult[i + 4], "");
				
                if (nID < 0)
                    continue;

                ArrayList arrLinkedZones = ParseZoneList(strLinkedZoneIDList);
                if (arrLinkedZones == null)
                    continue;

				if (nType < (int)EquipmentZone.EquipZoneType.SENSOR_TYPE ||
					 nType > (int)EquipmentZone.EquipZoneType.OTHER_TYPE)
					continue;

                EquipmentZone equipZone = new EquipmentZone();

                equipZone.ID = nID;
                equipZone.EquipZoneName = strEquipZoneName;
				equipZone.BroadcastName = strBroadcastName;
                equipZone.Type = (EquipmentZone.EquipZoneType)nType;

                foreach (Zone zone in arrLinkedZones)
                {
                    equipZone.LinkedZoneList.Add(zone);

                    if (m_dicZoneEquipZones.ContainsKey(zone))
                    {
                        ArrayList arrEquipZones = m_dicZoneEquipZones[zone];
                        arrEquipZones.Add(equipZone);
                    }
                    else
                    {
                        ArrayList arrEquipZones = new ArrayList();
                        m_dicZoneEquipZones[zone] = arrEquipZones;
                        arrEquipZones.Add(equipZone);
                    }
                }

                m_dicEquipZones[nID] = equipZone;
            }            
        }

        private ArrayList ParseZoneList(string strZoneIDList)
        {
            strZoneIDList = strZoneIDList.Trim();

            string[] arrZoneIDs = strZoneIDList.Split(',');

            ArrayList arrResult = new ArrayList();
            int nZoneID;

            foreach (string strZoneID in arrZoneIDs)
            {
                if (int.TryParse(strZoneID, out nZoneID))
                {
                    Zone zone = GetZone(nZoneID);

                    if (zone != null && !arrResult.Contains(zone))
                        arrResult.Add(zone);
                }
            }

            return arrResult;
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
		
		public Zone GetZone(string buildingID, int floorIndex, float addFloor)
		{            
			Building building = GetBuilding(buildingID);
			if (building == null)
				return null;

			foreach ( Zone zone in building.FloorList)
			{
				if (zone != null && zone.Floor.FloorIndex == ((float)floorIndex + addFloor))
				{
					if (addFloor.Equals(zone.AddFloor))
					{
						return zone;
					}
				}
			}
			return null;
		}

		public Zone  GetZone(string buildingID , float floorIndex)
		{
			foreach (KeyValuePair<int, Zone> kv in m_dicZones)
			{
				Zone obj = kv.Value;
				if (obj.Building != null && buildingID == obj.Building.BuildingID)
				{
					if (obj.Floor.FloorIndex == floorIndex)
					{
						return obj;
					}
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

        public EquipmentZone GetEquipZone(int nEquipZoneID)
        {
            if (m_dicEquipZones.ContainsKey(nEquipZoneID))
                return m_dicEquipZones[nEquipZoneID];

            return null;
        }

		public string GetBuildingID(int nID)
		{
			if (!m_dicBuildings.ContainsKey(nID))
				return "";
			Building b = m_dicBuildings[nID];
			return b.BuildingID;
		}

        public Building GetBuilding(int nID)
        {
            if (!m_dicBuildings.ContainsKey(nID))
                return null;
            Building b = m_dicBuildings[nID];
            return b;
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
		
		public string CheckZoneName(float x, float y)
		{
			Zone zone = GetOutsideZone(x, y);
			return zone == null ? "" : zone.BroadcastName;			
		}

		public Zone GetOutsideZone(float x, float y)
		{
			x += dx;
			y = dy - y;

			UnE.Geometry.Vertex2D vertex = new UnE.Geometry.Vertex2D(x, y);

			foreach (KeyValuePair<int, Zone> kv in m_dicOutdoorZones)
			{
				Zone obj = kv.Value;
				if (obj != null && obj.Polygon != null && obj.Polygon.GetVertexCount() >= 3)
				{
					if (obj.Polygon.HitTest(vertex) != 0)
					{
						return obj;
					}
				}
			}
			return null;
		}      

		public Zone FindZone(Building building, string strFloor)
		{
			Dictionary<int, Building> dicBuildings = DataManager.Instance.DicBuildings;
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

        
        public ArrayList FindNearZone(int nZoneID, bool isInDoor, int nMaxFind)
        {
            ArrayList arResult = new ArrayList();
            Zone zone = GetZone(nZoneID);
            if (zone == null)
                return null;

            // 모든 존으로의 Distance를 구한다.
            // Distance가 적은순으로 Sort 한다.
            SortedList<double, Zone> tempList = new SortedList<double, Zone>();
            UnE.Geometry.Vertex2D pos = zone.Polygon.CalcWeightCenter();
            if (isInDoor == true)
            {
                foreach (KeyValuePair<int, Zone> pair in m_dicZones)
                {
                    Zone zoneTarget = pair.Value;
                    if (zoneTarget.ID == nZoneID)
                        continue;

                    // 실내/실내가 다른경우 검사할 필요 없다.
                    if (zoneTarget.IsOutdoor != isInDoor)
                        continue;

                    // 실내인 경우 다른 건물은 탐색 필요가 없다
                    if (isInDoor == true && zoneTarget.Building != zone.Building)
                    {
                        continue;
                    }

                    // 실내인 경우 다른층은 탐색 필요 없다
					if (isInDoor == true && zoneTarget.Floor.FloorIndex != zone.Floor.FloorIndex)
                    {
                        continue;
                    }

                    double distance = zoneTarget.Polygon.GetDistance(pos);
                    tempList.Add(distance, zoneTarget);
                }
            }            
            if (isInDoor == false)
            {
                foreach (KeyValuePair<int, Zone> pair2 in m_dicOutdoorZones)
                {
                    Zone zoneTarget = pair2.Value;
                    if( zoneTarget.ID == nZoneID)
                        continue;

                    double distance = zoneTarget.Polygon.GetDistance(pos);
                    if (tempList.ContainsKey(distance))
                    {
                        distance += 0.001;
                    }
                    tempList.Add(distance, zoneTarget);
                }   
            }
            
            // MaxFind만큼 담아서 리턴
            foreach (KeyValuePair<double, Zone> pair in tempList)
            {
                arResult.Add(pair.Value);
                if (arResult.Count == nMaxFind)
                {
                    break;
                }
            }
            return arResult;
        }

        public ArrayList GetEquipmentZoneList(Zone zone)
        {
            if (m_dicZoneEquipZones.ContainsKey(zone))
                return m_dicZoneEquipZones[zone];

            return null;
        }

        private List<string> GetHistoryDisasterPositions()
        {
            string strSQL = "select Description from HistoryDisasterPos group by Description, SiteID having SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = SOPMonitoringSystem.FormSOP.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null)
                return null;

            List<string> positions = new List<string>();

            foreach (object item in arrResult)
            {
                string strPosition = WebDBManager.GetStringField(item);

                if (strPosition != null)
                    positions.Add(strPosition);
            }

            return positions;
        }

        public ArrayList LoadHistoryDisasterPosition()
        {
            WebDBManager dbMgr = SOPMonitoringSystem.FormSOP.Instance.DBManager;
            List<string> lastPositionNames = GetHistoryDisasterPositions();

            ArrayList result = new ArrayList();

            foreach (string strPositionName in lastPositionNames)
            {
                string strSQL = "select Description, PosX, PosY, PosZ, FloorIndex, DisasterType, BuildingID, BroadcastName ";

                if (FormSOP.Instance.SimulationMode)
                {
                    // SQLite 문법
                    strSQL += string.Format("from HistoryDisasterPos Where Description = '{0}' and SiteID = {1} order by ID Desc LIMIT 0, 1",
                        strPositionName, m_nSiteID);
                }
                else
                {
                    if (dbMgr.DatabaseType == DBUtility.WebDBManager.DBType.sqlserver)
                    {
                        strSQL = "select top 1 Description, PosX, PosY, PosZ, FloorIndex, DisasterType, BuildingID, BroadcastName ";
                        strSQL += string.Format("from HistoryDisasterPos Where Description = '{0}' and SiteID = {1} order by ID Desc",
                            strPositionName, m_nSiteID);
                    }
                    else if (dbMgr.DatabaseType == DBUtility.WebDBManager.DBType.mysql)
                    {
                        strSQL += string.Format("from HistoryDisasterPos Where Description = '{0}' and SiteID = {1} order by ID Desc LIMIT 0, 1",
                            strPositionName, m_nSiteID);
                    }
                    else
                        throw new NotImplementedException();
                }

                ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

                if (arrResult == null)
                    break;

                int nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 7; i += 8)
                {
                    try
                    {
                        string strNamePos = WebDBManager.GetStringField(arrResult[i], "");
                        float posX = WebDBManager.GetFloatField((string)arrResult[i + 1], 0.0f);
                        float posY = WebDBManager.GetFloatField((string)arrResult[i + 2], 0.0f);
                        float posZ = WebDBManager.GetFloatField((string)arrResult[i + 3], 0.0f);
                        float floorIdx = WebDBManager.GetFloatField((string)arrResult[i + 4], -999.0f);
                        string strDiasterName = WebDBManager.GetStringField((string)arrResult[i + 5], "");
                        string strBuildingID = WebDBManager.GetStringField((string)arrResult[i + 6], "");
                        string szBroadcastName = WebDBManager.GetStringField((string)arrResult[i + 7]);

                        UnE.SOP.HistoryDisasterPosition hisPos = new UnE.SOP.HistoryDisasterPosition();
                        hisPos.PoistionName = strNamePos;
                        System.Drawing.PointF pos = new System.Drawing.PointF(posX, posY);
                        hisPos.X = posX;
                        hisPos.Y = posY;
                        hisPos.Z = posZ;
                        hisPos.DisasterName = strDiasterName;
                        hisPos.FloorIndex = floorIdx;
                        hisPos.BuildingID = strBuildingID;
                        hisPos.BroadcastName = szBroadcastName == null ? strNamePos : szBroadcastName;

                        result.Add(hisPos);
                    }
                    catch (Exception)
                    {
                    }
                }

                if (result.Count > 9)
                    break;
            }

            return result;

            // changed by mwkim 2015-10-21 중복없는 재난위치를 가져오기 위해 Limit 구절을 연속으로 사용

            /*SOPMonitoringSystem.WebDBManager webDB = SOPMonitoringSystem.FormSOP.Instance.DBManager;

            // 한번에 DB에서 가져올 데이터의 양
            int nLimitCount = 20;

            string szText = null;

            if (FormSOP.Instance.SimulationMode)
            {
                // SQLite 문법
                //szText = "select Description, PosX, PosY, PosZ, FloorIndex, DisasterType, BuildingID ";
                //szText += "from HistoryDisasterPos Where SiteID = {0} order by ID Desc LIMIT 0, 5";
                szText = "select Description, PosX, PosY, PosZ, FloorIndex, DisasterType, BuildingID, BroadcastName ";
                szText += "from HistoryDisasterPos Where SiteID = {0} order by ID Desc LIMIT {2}, {1}";
            }
            else
            {
                // MS-SQL 문법
                //szText = "select top 5 Description, PosX, PosY, PosZ, FloorIndex, DisasterType, BuildingID ";
                //szText += "from HistoryDisasterPos Where SiteID = {0} order by ID Desc";

                if (webDB.DatabaseType == DBUtility.WebDBManager.DBType.sqlserver)
                {
                    szText = "select top 20 Description, PosX, PosY, PosZ, FloorIndex, DisasterType, BuildingID , BroadcastName ";
                    szText += "from HistoryDisasterPos Where SiteID = {0} and id not in (select top {2} id from HistoryDisasterPos where SiteID = {0} order by ID Desc) order by ID Desc";
                }
                else if (webDB.DatabaseType == DBUtility.WebDBManager.DBType.mysql)
                {
                    szText = "select Description, PosX, PosY, PosZ, FloorIndex, DisasterType, BuildingID , BroadcastName ";
                    szText += "from HistoryDisasterPos Where SiteID = {0} and id not in (select id from HistoryDisasterPos where SiteID = {0} order by ID Desc LIMIT 0, {2}) order by ID Desc LIMIT 0, 20";
                }
                else
                    throw new NotImplementedException();
            }			
			
			ArrayList result = new ArrayList();

            int nLoopCount = 0;
            while (result.Count < 10)
            {
                nLoopCount++;

                string szSQL = String.Format(szText, m_nSiteID, nLoopCount * nLimitCount, (nLoopCount - 1) * nLimitCount);
                ArrayList arrResult = webDB.GetResultData(szSQL, 0);

                if (arrResult == null)
                    break;

                if (arrResult.Count == 0)
                    break;


                int nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 7; i += 8)
                {
                    try
                    {
                        string strNamePos = WebDBManager.GetStringField(arrResult[i], "");
                        float posX = WebDBManager.GetFloatField((string)arrResult[i + 1], 0.0f);
                        float posY = WebDBManager.GetFloatField((string)arrResult[i + 2], 0.0f);
                        float posZ = WebDBManager.GetFloatField((string)arrResult[i + 3], 0.0f);
                        float floorIdx = WebDBManager.GetFloatField((string)arrResult[i + 4], -999.0f);
                        string strDiasterName = WebDBManager.GetStringField((string)arrResult[i + 5], "");
                        string strBuildingID = WebDBManager.GetStringField((string)arrResult[i + 6], "");
                        string szBroadcastName = WebDBManager.GetStringField((string)arrResult[i + 7], "");

                        UnE.SOP.HistoryDisasterPosition hisPos = new UnE.SOP.HistoryDisasterPosition();
                        hisPos.PoistionName = strNamePos;
                        System.Drawing.PointF pos = new System.Drawing.PointF(posX, posY);
                        hisPos.X = posX;
                        hisPos.Y = posY;
                        hisPos.Z = posZ;
                        hisPos.DisasterName = strDiasterName;
                        hisPos.FloorIndex = floorIdx;
                        hisPos.BuildingID = strBuildingID;
                        hisPos.BroadcastName = szBroadcastName;

                        if ((from items in result.Cast<UnE.SOP.HistoryDisasterPosition>()
                             where items.PoistionName == hisPos.PoistionName
                             && items.FloorIndex == hisPos.FloorIndex
                             && items.BuildingID == hisPos.BuildingID
                             select items).Count() == 0)
                        {
                            result.Add(hisPos);

                            if (result.Count > 9)
                                break;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            return result;*/
        }

        public List<UnE.Spatial.Shelter> LoadShelter()
        {
            UnE.Spatial.ZoneManager.Instance.LoadShelters();
            Dictionary<UnE.Spatial.Shelter, UnE.Spatial.Shelter> shelters = new Dictionary<UnE.Spatial.Shelter, UnE.Spatial.Shelter>();

            foreach (UnE.Spatial.Shelter.ShelterTypes type in Enum.GetValues(typeof(UnE.Spatial.Shelter.ShelterTypes)))
            {
                Dictionary<int, UnE.Spatial.Shelter> dicShelters = UnE.Spatial.ZoneManager.Instance.GetShelters(type);

                if (dicShelters == null)
                    continue;

                foreach (KeyValuePair<int, UnE.Spatial.Shelter> pair in dicShelters)
                {
                    if (shelters.ContainsKey(pair.Value) == false)
                        shelters[pair.Value] = pair.Value;
                }
            }

            return shelters.Values.ToList();

            /*SOPMonitoringSystem.WebDBManager webDB = SOPMonitoringSystem.FormSOP.Instance.DBManager;

            string strSQL = "Select ID, ShelterName, Boundary, Description from Shelter where SiteID = " + this.m_nSiteID.ToString();
            ArrayList arrResult = webDB.GetResultData(strSQL, 0);

            if (arrResult == null)
                return null;

            List<Shelter> shelters = new List<Shelter>();
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-3;i+=4)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strShelterName = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strBoundary = WebDBManager.GetStringField(arrResult[i + 2], "");
                string strDescription = WebDBManager.GetStringField(arrResult[i + 3], "");

                if (nID < 0 || strShelterName.Length == 0 || strShelterName == "null" ||
                    strBoundary.Length == 0 || strBoundary == "null")
                    continue;

                List<UnE.Geometry.Polygon> polygons = GetVertices(strBoundary);

                if (polygons == null)
                    continue;

                Shelter shelter = new Shelter();

                shelter.ID = nID;
                shelter.ShelterName = strShelterName;
                shelter.Description = strDescription;

                foreach (UnE.Geometry.Polygon polygon in polygons)
                {
                    shelter.Boundaries.Add(polygon);
                }

                shelters.Add(shelter);
            }

            return shelters;*/
        }

        private List<UnE.Geometry.Polygon> GetVertices(string strVertices)
        {
            List<UnE.Geometry.Polygon> polygons = new List<UnE.Geometry.Polygon>();

            string[] arrTokens = strVertices.Split('#');

            foreach (string strToken in arrTokens)
            {
                UnE.Geometry.Polygon polygon = GetPolygon(strToken.Trim());

                if (polygon == null)
                    return null;
                else
                    polygons.Add(polygon);
            }

            return polygons;
        }

        private UnE.Geometry.Polygon GetPolygon(string strVertices)
        {
            string[] arrTokens = strVertices.Split(',');
            int nPointCount = arrTokens.Count();

            if (nPointCount < 6 || (nPointCount % 2 == 1))
                return null;

            double x, y;
            UnE.Geometry.Polygon polygon = new UnE.Geometry.Polygon();

            for (int i=0;i<nPointCount;i+=2)
            {
                string strX = arrTokens[i].Trim();
                string strY = arrTokens[i + 1].Trim();

                if (!double.TryParse(strX, out x) || !double.TryParse(strY, out y))
                    return null;

                UnE.Geometry.Vertex2D vertex = new UnE.Geometry.Vertex2D(x, y);
                polygon.AddVertex(vertex);
            }

            return polygon;
        }

        public bool GetUserDefinedTeamInfo(int nTeamID, out string strPhoneNumber, out string strTeamName)
        {
            strPhoneNumber = strTeamName = "";

            string strSQL = "Select TeamName, PhoneNumber from UserDefinedTeam where ID = " + nTeamID.ToString() + " and SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();
            ArrayList arrResult = UnE.SOP.ProxySOP.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count != 2)
                return false;

            strPhoneNumber = WebDBManager.GetStringField(arrResult[0], "");
            strTeamName = WebDBManager.GetStringField(arrResult[1], "");
            return true;
        }

        public void LoadTemporaryTeams()
        {
            LoadTemporaryTeams(true);
            LoadTemporaryTeams(false);
        }

        private void LoadTemporaryTeams(bool isNormal)
        {
            string strTableName = isNormal ? "TemporaryNormalTeam" : "TemporaryEmergencyTeam";
            string strSQL = "Select ID, TeamName, ParentTeamID, RegularTeamLink from " + strTableName + " where SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();
            ArrayList arrResult = UnE.SOP.ProxySOP.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            // Key : Team ID
            // Value : Parent Team ID
            Dictionary<int, int> dicTeamIDs = new Dictionary<int, int>();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                DBUtility.VariousData<int> id = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString());
                string strTeamName = DBUtility.WebDBManager.GetStringField(arrResult[i + 1]);
                DBUtility.VariousData<int> parentTeamID = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString());
                string strRegularTeamIDs = DBUtility.WebDBManager.GetStringField(arrResult[i + 3]);

                if (id == null || strTeamName == null)
                    continue;

                if (parentTeamID != null)
                    dicTeamIDs[id.Data] = parentTeamID.Data;

                if (isNormal)
                {
                    Data_NormalTeam team = new Data_NormalTeam();
                    team.ID = id.Data;
                    team.TeamName = strTeamName;
                    team.Tag = strRegularTeamIDs;

                    m_dicTemporaryNormalTeams[id.Data] = team;
                }
                else
                {
                    Data_EmergencyTeam team = new Data_EmergencyTeam();
                    team.ID = id.Data;
                    team.TeamName = strTeamName;
                    team.Tag = strRegularTeamIDs;

                    m_dicTemporaryEmergencyTeams[id.Data] = team;
                }
            }

            foreach (KeyValuePair<int, int> pair in dicTeamIDs)
            {
                if (isNormal)
                {
                    Data_NormalTeam team = null, teamParent = null;

                    if (!m_dicTemporaryNormalTeams.TryGetValue(pair.Key, out team) || !m_dicTemporaryNormalTeams.TryGetValue(pair.Value, out teamParent))
                        continue;

                    team.ParentTeam = teamParent;
                    teamParent.ChildTeams.Add(team);
                }
                else
                {
                    Data_EmergencyTeam team = null, teamParent = null;

                    if (!m_dicTemporaryEmergencyTeams.TryGetValue(pair.Key, out team) || !m_dicTemporaryEmergencyTeams.TryGetValue(pair.Value, out teamParent))
                        continue;

                    team.ParentTeam = teamParent;
                    teamParent.ChildTeams.Add(team);
                }
            }
        }

        public Data_NormalTeam GetTemporaryNormalTeam(int nTeamID)
        {
            Data_NormalTeam team = null;
            m_dicTemporaryNormalTeams.TryGetValue(nTeamID, out team);
            return team;
        }

        public Data_EmergencyTeam GetTemporaryEmergencyTeam(int nTeamID)
        {
            Data_EmergencyTeam team = null;
            m_dicTemporaryEmergencyTeams.TryGetValue(nTeamID, out team);
            return team;
        }

        public Data_UserDefinedTeam LoadUserDefinedTeam(int nTeamID)
        {
            string strSQL = "Select TeamName, PhoneNumber from UserDefinedTeam where SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString() + " and ID = " + nTeamID.ToString();
            ArrayList arrResult = UnE.SOP.ProxySOP.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count != 2)
                return null;

            string strTeamName = WebDBManager.GetStringField(arrResult[0]);
            string strPhoneNumber = WebDBManager.GetStringField(arrResult[1], "");

            if (strTeamName == null)
                return null;

            Data_UserDefinedTeam team = new Data_UserDefinedTeam();
            team.ID = nTeamID;
            team.TeamName = WebDBManager.GetStringField(arrResult[0]);
            team.PhoneNumber = strPhoneNumber;

            return team;
        }
	}
    public class SecurityFacilityType
    {
        private int nID = 0;
        public int ID
        {
            get { return nID; }
            set { nID = value; }
        }
        private string strSubCategoryName = "";
        public string SubCategoryName
        {
            get { return strSubCategoryName; }
            set { strSubCategoryName = value; }
        }
        private int nSecurityType = 0;
        public int SecurityType
        {
            get { return nSecurityType; }
            set { nSecurityType = value; }
        }
        private ArrayList m_arFacilityTypeIDs = new ArrayList();
        public ArrayList FacilityTypeIDs
        {
            get { return m_arFacilityTypeIDs; }
            set { m_arFacilityTypeIDs = value; }
        }
    }
}
