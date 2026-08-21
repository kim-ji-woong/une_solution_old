using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using DBUtility2;

namespace UnE.CCTV
{
	internal class ZoneManager
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

		private Dictionary<int, EquipmentZone> m_dicEquipZones = new Dictionary<int, EquipmentZone>();

		public Dictionary<int, EquipmentZone> DicEquipZones
		{
			get { return m_dicEquipZones; }
			set { m_dicEquipZones = value; }
		}

		// Zone별 EquipmentZone List
		private Dictionary<Zone, ArrayList> m_dicZoneEquipZones = new Dictionary<Zone, ArrayList>();

		private Dictionary<int, EquipmentZone> m_dicOutdoorEquipZones = new Dictionary<int, EquipmentZone>();

		public Dictionary<int, EquipmentZone> DicOutdoorEquipZones
		{
			get { return m_dicOutdoorEquipZones; }
			set { m_dicOutdoorEquipZones = value; }
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

		private BuildingGroup m_outdoorBuildingGroup = new BuildingGroup();

		public BuildingGroup OutdoorBuildingGroup
		{
			get { return m_outdoorBuildingGroup; }
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

		private float dx = 0.0f;//121902.5858f; //120894.0548f + 1008.531f;

		public float Dx
		{
			get { return dx; }
			set { dx = value; }
		}

        private float dy = 0.0f;//157152.8453f; //157659.0963f - 506.251f;

		public float Dy
		{
			get { return dy; }
			set { dy = value; }
		}

        private int m_nSiteID = 1;


		public ZoneManager()
		{
            m_nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;

			m_outdoorBuildingGroup.BuildingGroupName = "외부 영역";
		}        
       
        /*private Dictionary<int, Shelter> m_dicSafeZones = new Dictionary<int, Shelter>();
        public Dictionary<int, Shelter> DicSafeZones
        {
            get { return m_dicSafeZones; }   
        }
        public void LoadSafeZone()
        {
            WebDBManager webDB = FormMain.Instance.DBManager;
            string szText = "SELECT ID, ShelterName, Boundary, Description FROM Shelter";
            string strSQL = string.Format(szText, m_nSiteID);

			ArrayList arrResult = webDB.GetResultData(strSQL);
			if (arrResult == null)
				return;

			int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string szShelterName = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strBoundary = WebDBManager.GetStringField(arrResult[i + 2], "");

                Shelter s = new Shelter();
                s.ID = nID;
                s.ShelterName = szShelterName;

                try
                {
                    s.Polygon = MakeZonePolygon(strBoundary);
                }
                catch (System.Exception)
                {
                    //MessageBox.Show("Make polygo error!!");
                }

                m_dicSafeZones.Add(nID, s);

            }
        }*/
		public void LoadBuildingData()
		{
			WebDBManager webDB = FormMain.Instance.DBManager;

			//string strSQL = "select Building.id, BuildingID, BuildingCode, BuildingName, BuildingGroupID, MaxFloor, MinFloor,"
			//		 + "BuildingGroup.GroupName, BuildingGroup.TextCenter, Building.BroadCastingText ";
			//strSQL += "from Building, BuildingGroup where Building.BuildingGroupID = BuildingGroup.ID";

            string szText = "SELECT bd.id, bd.BuildingID, bd.BuildingCode, bd.BuildingName, bd.BuildingGroupID, " +
                            "  bd.MaxFloor, bd.MinFloor, bg.GroupName, bg.DisplayText, bg.TextCenter, bd.BroadCastingText, bd.DisplayText " +
                            "  FROM Building AS bd INNER JOIN BuildingGroup AS bg ON bd.BuildingGroupID = bg.ID AND bg.SiteID = {0} " +
                            "  ORDER BY bg.ID,  bd.ID";

            string strSQL = string.Format(szText, m_nSiteID);

			ArrayList arrResult = webDB.GetResultData(strSQL);
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
						float x = 0.0f, y= 0.0f;
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

                polygons.Add(polygon);
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
			WebDBManager webDB = FormMain.Instance.DBManager;

            string strSQL = "select id, ZoneName, BuildingID, FloorIndex, Boundary, DXFFileName, DXFAccessedTime, _3DFileName, _3DAccessedTime, BroadcastName, AddFloor, Azimuth, DisplayText from Zone" +
                            " where SiteID = " + m_nSiteID.ToString();

			ArrayList arrResult = webDB.GetResultData(strSQL);

			if (arrResult == null)
				return;

			DateTime dtDefault = new DateTime();
			int nResultCount = arrResult.Count;

			for (int i = 0; i < nResultCount - 12; i += 13)
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
                float dAzimuth = WebDBManager.GetFloatField(arrResult[i + 11].ToString(), 0.0f);
                string strDisplayText = WebDBManager.GetStringField(arrResult[i + 12], "");
                
                Zone zone = new Zone();

				zone.ID = nID;
				zone.ZoneName = strZoneName;
				zone.FloorIndex = nFloorIndex;
                zone.Azimuth = dAzimuth;

				zone.DXFFilePath = strDXFFileName;

				if (zone.DXFFilePath.Length > 0 && zone.DXFFilePath != "null")
				{
					//파일경로에서 파일이름을 따옴
					string tmp = strDXFFileName;
					string[] buf = tmp.Split('\\');

                    if (buf.Length > 1)
                    {
                        if (tmp.ToLower() == "blank.png")
                        {
                            zone.DXFFileName = "blank.png";
                            zone.DXFFilePath = "blank.png";
                        }
                        else
                            zone.DXFFileName = buf[1];
                    }
                    else
                    {
                        zone.DXFFileName = "blank.png";
                        zone.DXFFilePath = "blank.png";
                    }
				}
				else
				{
					zone.DXFFileName = "blank.png";
                    zone.DXFFilePath = "blank.png";
				}

				if (strBroadcastName == "null" || strBroadcastName == "")
					zone.BroadcastName = strZoneName;
				else
					zone.BroadcastName = strBroadcastName;

                if (strDisplayText == "null" || strDisplayText == "")
                    zone.DisplayText = strZoneName;
                else
                    zone.DisplayText = strDisplayText;

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
					//if (addFloor.Equals(zone.AddFloor))
					//{
					return zone;
					//}
				}
			}
			return null;
		}

		public Zone GetZone(string buildingID, int floorIndex)
		{
			foreach (KeyValuePair<int, Zone> kv in m_dicZones)
			{
				Zone obj = kv.Value;
				if (obj.Building != null && buildingID == obj.Building.BuildingID)
				{
					if (obj.FloorIndex == floorIndex && obj.AddFloor == 0.0f)
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

		public string CheckZoneName(float x, float y)
		{
			Zone zone = GetOutsideZone(x, y);
            return zone == null ? "" : zone.DisplayText;
		}

		public Zone GetOutsideZone(float x, float y)
		{
			x += dx;
			y = dy - y;

            x *= 1000;
            y *= 1000;

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

		public ArrayList FindNearZone(int nZoneID, bool isInDoor, int nMaxFind)
		{
			ArrayList arResult = new ArrayList();
			Zone zone = GetZone(nZoneID);
			if (zone == null)
				return null;

            if (zone.Polygon == null)
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
					if (isInDoor == true && zoneTarget.FloorIndex != zone.FloorIndex)
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
					if (zoneTarget.ID == nZoneID)
						continue;

                    if (zoneTarget.Polygon == null)
                        continue;

					double distance = zoneTarget.Polygon.GetDistance(pos);
					

                    for (int i = 0; i < 100; i++)
                    {
                        if (tempList.ContainsKey(distance))
                        {
                            distance += 0.001;
                        }
                        if( !tempList.ContainsKey(distance))
                        {
                            tempList.Add(distance, zoneTarget);
                            break;
                        }
                    }

                       
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

        

		public EquipmentZone CheckEquipmentZone(Zone zone, float x, float y)
		{
			if (zone == null || zone.IsOutdoor == true)
				return null;

			UnE.Geometry.Vertex2D center = zone.Polygon.CalcWeightCenter();
			x += (float)center.x;
			y = (float)(center.y - y);
			UnE.Geometry.Vertex2D vertex = new UnE.Geometry.Vertex2D(x, y);

			foreach (KeyValuePair<int, EquipmentZone> kv in m_dicEquipZones)
			{
				EquipmentZone obj = kv.Value;
				if (obj != null)
				{
					ArrayList arZoneList = obj.LinkedZoneList;
					foreach (Zone linkzone in arZoneList)
					{
						if (linkzone != null && linkzone.ID == zone.ID)
						{
							if (obj.Polygon.HitTest(vertex) != 0)
							{
								return obj;
							}
						}
					}
				}
			}
			return null;
		}

		public EquipmentZone GetEquipZone(int nEquipZoneID)
		{
			if (m_dicEquipZones.ContainsKey(nEquipZoneID))
				return m_dicEquipZones[nEquipZoneID];
			return null;
		}

		public void LoadEquipmentZone()
		{
			WebDBManager webDB = FormMain.Instance.DBManager;

			string strSQL = "select id, ZoneName, Boundary, LinkedZoneIDList, type, BroadcastName, DisplayText from EquipmentZone where ID > 0" +
                            " and SiteID = " + m_nSiteID.ToString();

			ArrayList arrResult = webDB.GetResultData(strSQL);

			if (arrResult == null)
				return;

			int nResultCount = arrResult.Count;
			for (int i = 0; i < nResultCount - 6; i += 7)
			{
				int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
				string strZoneName = WebDBManager.GetStringField(arrResult[i + 1], "");
				string strBoundary = WebDBManager.GetStringField(arrResult[i + 2], "");
				string strLinkedZones = WebDBManager.GetStringField(arrResult[i + 3], "");
				int nType = WebDBManager.GetIntField(arrResult[i + 4].ToString(), 0);
				string strBroadcastName = WebDBManager.GetStringField(arrResult[i + 5], "");
                string strDisplayText = WebDBManager.GetStringField(arrResult[i + 6], "");

				EquipmentZone zone = new EquipmentZone();

				zone.ID = nID;
				zone.ZoneName = strZoneName;
				zone.ZoneType = (EquipmentZone.EquipZoneType)nType;

                if (strBroadcastName == "null" || strBroadcastName == "")
                    zone.BroadcastName = strZoneName;
                else
				    zone.BroadcastName = strBroadcastName;

                if (strDisplayText == "null" || strDisplayText == "")
                    zone.DisplayText = strZoneName;
                else
                    zone.DisplayText = strDisplayText;

				strLinkedZones = strLinkedZones.Trim();
				string[] szIds = strLinkedZones.Split(',');

				for (int j = 0; j < szIds.Length; j++)
				{
					string szID = szIds[j];
					int nZoneID = -1;
					if (int.TryParse(szID, out nZoneID))
					{
						Zone realzone = GetZone(nZoneID);
						if (zone.LinkedZone == null)
						{
							zone.LinkedZone = realzone;
						}

						if (realzone != null)
						{
							zone.LinkedZoneList.Add(realzone);
							ArrayList arrEquipZones = null;

							if (m_dicZoneEquipZones.ContainsKey(realzone))
							{
								arrEquipZones = m_dicZoneEquipZones[realzone];
							}
							else
							{
								arrEquipZones = new ArrayList();
								m_dicZoneEquipZones[realzone] = arrEquipZones;
							}

							if (!arrEquipZones.Contains(zone))
								arrEquipZones.Add(zone);
						}
					}
				}
				if (zone.Building != null)
				{
					zone.Building.EquipZoneList.Add(zone);
				}

				try
				{
					zone.Polygon = MakeZonePolygon(strBoundary);
				}
				catch (System.Exception ex)
				{
					Debug.WriteLine(ex.Message);
					Debug.WriteLine(ex.StackTrace);
				}

				//중복 EquipZone Boundary 점검
				//CheckPolygon(nID, zone.Polygon);

				m_dicEquipZones[nID] = zone;

				if (zone.Building == null)
					m_dicOutdoorEquipZones[nID] = zone;
			}

			//Zone별 회로번호 얻어오기(Table_1)
			//LoadSensorTagInfo();
		}

		#region 중복 EquipZone Boundary 점검

		/*private void CheckPolygon(int nID, UnE.Geometry.Polygon polygon)
        {
            int nVertexCount = polygon.GetVertexCount();
            int nOriginCount = nVertexCount;

            if (nVertexCount < 3)
                return;

            UnE.Geometry.Vertex2D prev = polygon.GetVertex(0);

            for (int i = 1; i < nVertexCount; i++)
            {
                UnE.Geometry.Vertex2D vertex = polygon.GetVertex(i);

                if (prev.GetDistance(vertex) < 0.005)
                {
                    polygon.RemoveVertex(i);
                    i--;
                    nVertexCount--;
                }
                else
                    prev = vertex;
            }

            nVertexCount = polygon.GetVertexCount();

            if (nOriginCount == nVertexCount)
                return;

            if (nVertexCount < 3)
                return;

            WebDBManager webDB = FormMain.Instance.DBManager;
            string strBoundary = GetBoundaryString(polygon);

            string strSQL = "Update EquipmentZone set Boundary = '" + strBoundary + "' where id = " + nID.ToString();
            webDB.GetResultData(strSQL);
        }

        private string GetBoundaryString(UnE.Geometry.Polygon polygon)
        {
            int nVertexCount = polygon.GetVertexCount();
            string strBoundary = "";

            for (int i = 0; i < nVertexCount; i++)
            {
                UnE.Geometry.Vertex2D vertex = polygon.GetVertex(i);

                if (strBoundary.Length == 0)
                    strBoundary = string.Format("{0:0.000}, {1:0.000}", vertex.x, vertex.y);
                else
                    strBoundary += string.Format(", {0:0.000}, {1:0.000}", vertex.x, vertex.y);
            }

            return strBoundary;
        }*/

		#endregion 중복 EquipZone Boundary 점검

		#region Zone별 회로번호 얻어오기(Table_1)

		/*class SensorTagInfo
        {
            private int m_nServerID = -1;
            private int m_nTagID = -1;
            private string m_strTagName = "";
            private bool m_isMonitoring = false;

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

            public bool IsMonitoring
            {
                get { return m_isMonitoring; }
                set { m_isMonitoring = value; }
            }
        }

        private void LoadSensorTagInfo()
        {
            Dictionary<EquipmentZone, ArrayList> dicEquipZoneSensor = new Dictionary<EquipmentZone, ArrayList>();

            WebDBManager dbMgr = FormMain.Instance.DBManager;
            string strSQL = string.Format("select SensorServerID, TagNo, SensorName, SensorType, EquipZoneID, Description from SensorTagInfo");

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 5; i += 6)
            {
                int nServerID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nTagNo = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);

                string strName = WebDBManager.GetStringField(arrResult[i + 2], "");
                int nSensorType = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                int nEquipZone = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);

                string strDesc = WebDBManager.GetStringField(arrResult[i + 5], "");

                if (nEquipZone <= 0)
                    continue;

                SensorTagInfo sensor = new SensorTagInfo();

                sensor.ServerID = nServerID;
                sensor.TagName = strName;
                sensor.TagID = nTagNo;
                sensor.IsMonitoring = nSensorType == 10;

                EquipmentZone equipZone = GetEquipZone(nEquipZone);
                if (equipZone == null)
                    continue;

                ArrayList arrSensors = null;

                if (dicEquipZoneSensor.ContainsKey(equipZone))
                    arrSensors = dicEquipZoneSensor[equipZone];
                else
                {
                    arrSensors = new ArrayList();
                    dicEquipZoneSensor[equipZone] = arrSensors;
                }

                arrSensors.Add(sensor);
            }

            foreach (KeyValuePair<int, EquipmentZone> pair in m_dicEquipZones)
            {
                if (dicEquipZoneSensor.ContainsKey(pair.Value))
                {
                    WriteDB(pair.Value, dicEquipZoneSensor[pair.Value]);
                }
                else
                    WriteDB(pair.Value);
            }
        }

        private void WriteDB(EquipmentZone equipZone)
        {
            WebDBManager dbMgr = FormMain.Instance.DBManager;

            string strFormat = "Insert into table_1 (EquipZoneID, BuildingGroup, BuildingName ,EquipZoneName, ZonePosition, SensorServerID , SensorTagID, SensorName, SensorDescription) values ";
            strFormat += "({0}, '{1}', '{2}', '{3}', '{4}', {5}, {6}, '{7}', '{8}')";

            string strSQL = "";
            string strEquipZoneName = equipZone.ZoneName.Replace('\'', (char)8);

            if (equipZone.LinkedZone.Building == null)
                strSQL = string.Format(strFormat, equipZone.ID, "-", "-", strEquipZoneName, equipZone.LinkedZone.ZoneName, -1, -1, "-", "-");
            else
                strSQL = string.Format(strFormat, equipZone.ID, equipZone.LinkedZone.Building.BuildingGroup.BuildingGroupName, equipZone.LinkedZone.Building.BuildingName, strEquipZoneName, equipZone.LinkedZone.ZoneName, -1, -1, "-", "-");

            if (dbMgr.GetResultData(strSQL) == null)
                return;
        }

        private void WriteDB(EquipmentZone equipZone, ArrayList arrSensors)
        {
            WebDBManager dbMgr = FormMain.Instance.DBManager;

            string strFormat = "Insert into table_1 (EquipZoneID, BuildingGroup, BuildingName ,EquipZoneName, ZonePosition, SensorServerID , SensorTagID, SensorName, SensorDescription) values ";
            strFormat += "({0}, '{1}', '{2}', '{3}', '{4}', {5}, {6}, '{7}', '{8}')";

            string strSQL = "";
            string strEquipZoneName = equipZone.ZoneName.Replace('\'', (char)8);

            foreach (SensorTagInfo sensor in arrSensors)
            {
                if (equipZone.LinkedZone.Building == null)
                    strSQL = string.Format(strFormat, equipZone.ID, "-", "-", strEquipZoneName, equipZone.LinkedZone.ZoneName, sensor.ServerID, sensor.TagID, sensor.TagName, sensor.IsMonitoring ? "감시센서" : "화재센서");
                else
                    strSQL = string.Format(strFormat, equipZone.ID, equipZone.LinkedZone.Building.BuildingGroup.BuildingGroupName, equipZone.LinkedZone.Building.BuildingName, strEquipZoneName, equipZone.LinkedZone.ZoneName, sensor.ServerID, sensor.TagID, sensor.TagName, sensor.IsMonitoring ? "감시센서" : "화재센서");

                if (dbMgr.GetResultData(strSQL) == null)
                    return;
            }
        }*/

		#endregion Zone별 회로번호 얻어오기(Table_1)

		public ArrayList GetEquipmentZoneList(Zone zone)
		{
			if (m_dicZoneEquipZones.ContainsKey(zone))
				return m_dicZoneEquipZones[zone];

			return null;
		}

		public BuildingGroup FindBuildingGroup(string GroupName)
		{
			foreach (KeyValuePair<int, BuildingGroup> kv in m_dicBuildingGroup)
			{
				BuildingGroup group = kv.Value;
				if (group.BuildingGroupName == GroupName)
					return group;
			}
			return null;
		}

		public Building FindBuilding(string BuildingName)
		{
            foreach (KeyValuePair<int, Building> kv in m_dicBuildings)
            {
                Building building = kv.Value;
                if (building.BuildingName == BuildingName || building.DisplayText == BuildingName)
                    return building;
            }
			return null;
		}

		public ArrayList FindZoneList(string GroupName, string BuildingName, string FloorName)
		{
			ArrayList arrSelectZoneList = new ArrayList();
			ArrayList arrGroupList = new ArrayList();
			ArrayList arrBuildingList = new ArrayList();
			ArrayList arrFloorList = new ArrayList();

			bool bAddOutterZone = false;
			if (GroupName == "모든 건물 그룹")
			{
				arrGroupList.AddRange(m_dicBuildingGroup.Values);
				bAddOutterZone = true;
			}
			else
			{
				if (GroupName == "외부 영역")
				{
					bAddOutterZone = true;
				}
				else
				{
					BuildingGroup group = FindBuildingGroup(GroupName);
					if (group != null)
						arrGroupList.Add(group);
				}
			}

			foreach (BuildingGroup group in arrGroupList)
			{
				if (BuildingName == "모든 건물")
				{
					arrBuildingList.AddRange(group.BuildingList);
				}
				else
				{
					Building building = FindBuilding(BuildingName);
					if (building != null)
						arrBuildingList.Add(building);
				}
			}

			foreach (Building building in arrBuildingList)
			{
				if (FloorName == "모든 층")
				{
					arrFloorList.AddRange(building.FloorList);
				}
				else
				{
					foreach (Zone floor in building.FloorList)
					{
						if (floor.Floor.ToString() == FloorName)
						{
							arrFloorList.Add(floor);
							break;
						}
					}
				}
			}

			if (bAddOutterZone == true)
			{
				if (BuildingName == "모든 건물")
				{
					arrFloorList.AddRange(m_dicOutdoorZones.Values);
				}
				else
				{
					foreach (KeyValuePair<int, Zone> kv in m_dicOutdoorZones)
					{
						Zone zone = kv.Value;
						if (zone.ZoneName == BuildingName)
						{
							arrFloorList.Add(zone);
							break;
						}
					}
				}
			}

			return arrFloorList;
		}      
	}

    public class Shelter
    {
        private int m_nID = 0;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        private string szShelterName = "";
        public string ShelterName
        {
            get { return szShelterName; }
            set { szShelterName = value; }
        }

        private UnE.Geometry.Polygon polygon = null;
        public UnE.Geometry.Polygon Polygon
        {
            get { return polygon; }
            set { polygon = value; }
        }
    }

}