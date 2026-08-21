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

namespace SOPDisasterSystem
{
	//public class DataManager
	//{
	//    static private DataManager m_instance = null;
	//    public static SOPDisasterSystem.DataManager Instance
	//    {
	//        get {
	//            if (m_instance == null)
	//                m_instance = new DataManager();
	//            return m_instance;
	//        }

	//    }
	//    private Dictionary<int, Zone> m_dicZones = new Dictionary<int, Zone>();		
	//    public Dictionary<int, Zone> DicZones
	//    {
	//        get { return m_dicZones; }
	//        set { m_dicZones = value; }
	//    }
		
	//    private Dictionary<int, Building> m_dicBuildings = new Dictionary<int, Building>();		
	//    public Dictionary<int, Building> DicBuildings
	//    {
	//        get { return m_dicBuildings; }
	//        set { m_dicBuildings = value; }
	//    }
	   
	//    private Dictionary<int, BuildingGroup> m_dicBuildingGroup = new Dictionary<int, BuildingGroup>();
	//    public Dictionary<int, BuildingGroup> DicBuildingGroup
	//    {
	//        get { return m_dicBuildingGroup; }
	//        set { m_dicBuildingGroup = value; }
	//    }
		
	//    private Dictionary<int, Zone> m_dicOutdoorZones = new Dictionary<int, Zone>();
	//    public Dictionary<int, Zone> DicOutdoorZones
	//    {
	//        get { return m_dicOutdoorZones; }
	//        set { m_dicOutdoorZones = value; }
	//    }

	//    private float dx = 121902.5858f; //120894.0548f + 1008.531f;
	//    private float dy = 157152.8453f; //157659.0963f - 506.251f;

	//    public ArrayList LoadFireEquipment(string BuildingID, int nFloorIdx)
	//    {
	//        string szSQP = "SELECT ID, EquipID, EquipType, ZoneID, X, Y, Z, Description FROM FireEquipment";
	//        string subSQL = string.Format(" where EquipType = '1' and ZoneID = ( select Zone.ID from Zone where Zone.BuildingID = " +
	//            "( SELECT TOP 1 ID FROM Building where BuildingID = '{0}') and FloorIndex = '{1}')", BuildingID, nFloorIdx);
	//        szSQP += subSQL;

	//        SOPMonitoringSystem.WebDBManager webDB = SOPMonitoringSystem.FormMain.Instance.DBManager;

	//        ArrayList arrResult = webDB.GetResultData(szSQP, 0);
	//        if (arrResult == null)
	//            return null;

	//        int nResultCount = arrResult.Count;

	//        ArrayList arResult = new ArrayList();
	//        for (int i = 0; i < nResultCount - 7; i += 8)
	//        {
	//            try
	//            {
	//                int nID = SOPMonitoringSystem.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
	//                string szEquipID = SOPMonitoringSystem.WebDBManager.GetStringField(arrResult[i + 1], "");
	//                int nEquipType = SOPMonitoringSystem.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
	//                int nZoneID = SOPMonitoringSystem.WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
	//                float fX = SOPMonitoringSystem.WebDBManager.GetFloatField(arrResult[i + 4].ToString(), 0.0f);
	//                float fY = SOPMonitoringSystem.WebDBManager.GetFloatField(arrResult[i + 5].ToString(), 0.0f);
	//                float fZ = SOPMonitoringSystem.WebDBManager.GetFloatField(arrResult[i + 6].ToString(), 0.0f);
	//                string szDesc = SOPMonitoringSystem.WebDBManager.GetStringField(arrResult[i + 7], "");

	//                FireEquipment equip = new FireEquipment();

	//                equip.ID = nID;
	//                equip.EquipID = szEquipID;
	//                equip.ZoneID = nZoneID;
	//                equip.X = fX;
	//                equip.Y = fY;
	//                equip.Z = fZ;
	//                equip.Description = szDesc;
	//                equip.BuildingID = BuildingID;
	//                equip.FloorIndex = nFloorIdx;

	//                arResult.Add(equip);                        
	//            }
	//            catch (Exception)
	//            {
	//                return null;
	//            }
	//        }
	//        return arResult;
	//    }

	//    public void LoadBuildingData()
	//    {
	//        SOPMonitoringSystem.WebDBManager webDB = SOPMonitoringSystem.FormMain.Instance.DBManager;

	//        string strSQL = "select Building.id, BuildingID, BuildingCode, BuildingName, BuildingGroupID, MaxFloor, MinFloor, BuildingGroup.GroupName, BuildingGroup.TextCenter, Building.BroadCastingText ";
	//        strSQL += "from Building, BuildingGroup where Building.BuildingGroupID = BuildingGroup.ID";

	//        ArrayList arrResult = webDB.GetResultData(strSQL, 0);
	//        if (arrResult == null)
	//            return;

	//        int nResultCount = arrResult.Count;

	//        for (int i = 0; i < nResultCount - 9; i += 10)
	//        {
	//            try
	//            {
	//                int nID = SOPMonitoringSystem.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
	//                string strBuildingID = SOPMonitoringSystem.WebDBManager.GetStringField(arrResult[i + 1], "");
	//                string strBuildingCode = SOPMonitoringSystem.WebDBManager.GetStringField(arrResult[i + 2], "");
	//                string strBuildingName = SOPMonitoringSystem.WebDBManager.GetStringField(arrResult[i + 3], "");
	//                int nBuildingGroupID = SOPMonitoringSystem.WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
	//                int nMaxFloorID = SOPMonitoringSystem.WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
	//                int nMinFloorID = SOPMonitoringSystem.WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
	//                string strBuildingGroupName = SOPMonitoringSystem.WebDBManager.GetStringField(arrResult[i + 7], "");
	//                string strGroupNamePos = SOPMonitoringSystem.WebDBManager.GetStringField(arrResult[i + 8], "");

	//                string strBroadcastName = SOPMonitoringSystem.WebDBManager.GetStringField(arrResult[i + 9], "");
	//                if (strBroadcastName == null || strBroadcastName.Equals("null"))
	//                {
	//                    strBroadcastName = strBuildingName;
	//                }
	//                else
	//                {
	//                    int nIdx = strBroadcastName.IndexOf('*');
	//                    if (nIdx != -1)
	//                    {
	//                        strBroadcastName = strBroadcastName.Substring(0, nIdx);
	//                    }
	//                }

	//                Building building = new Building();

	//                if (m_dicBuildingGroup.ContainsKey(nBuildingGroupID))
	//                    building.BuildingGroup = m_dicBuildingGroup[nBuildingGroupID];
	//                else
	//                {
	//                    BuildingGroup group = new BuildingGroup();
	//                    group.BuildingGroupName = strBuildingGroupName;

	//                    m_dicBuildingGroup[nBuildingGroupID] = group;
	//                    building.BuildingGroup = group;
	//                }

	//                building.BuildingName = strBuildingName;
	//                building.MaxFloorIndex = nMaxFloorID;
	//                building.MinFloorIndex = nMinFloorID;
	//                building.BuildingCode = strBuildingCode;
	//                building.BuildingID = strBuildingID;
	//                building.BroadcastName = strBroadcastName;
	//                m_dicBuildings[nID] = building;
	//            }
	//            catch (System.Exception ex)
	//            {
	//                MessageBox.Show(ex.StackTrace);
	//            }
	//        }
	//    }

	//    private UnE.Geometry.Polygon MakeZonePolygon(string szBoundary)
	//    {
	//        if (szBoundary == null || szBoundary == "")
	//            return null;
	//        UnE.Geometry.Polygon poly = new UnE.Geometry.Polygon();
	//        int start_idx = 0;
	//        bool bEnd = false;
	//        do
	//        {
	//            int idx = szBoundary.IndexOf(',', start_idx);
	//            if (idx == -1)
	//                break;
	//            string szPosX = szBoundary.Substring(start_idx, idx - start_idx);
	//            start_idx = idx + 1;


	//            idx = szBoundary.IndexOf(',', start_idx);
	//            string szPosY = "";

	//            if (idx == -1)
	//            {
	//                int nLength = szBoundary.Length - start_idx;
	//                szPosY = szBoundary.Substring(start_idx, nLength);
	//                bEnd = true;
	//            }
	//            else
	//                szPosY = szBoundary.Substring(start_idx, idx - start_idx);

	//            start_idx = idx + 1;
	//            double x = Double.Parse(szPosX);
	//            double y = Double.Parse(szPosY);
	//            UnE.Geometry.Vertex2D pos = new UnE.Geometry.Vertex2D(x, y);
				 
	//            float pos3DX = ((float)x - dx);
	//            float pos3DZ = dy + (float)y;
				
	//            poly.AddVertex(pos);
	//            if (bEnd == true)
	//                break;
	//        } while (start_idx < szBoundary.Length);
	//        return poly;
	//    }

	//    public void LoadZones()
	//    {
	//        SOPMonitoringSystem.WebDBManager webDB = SOPMonitoringSystem.FormMain.Instance.DBManager;

	//        string strSQL = "select id, ZoneName, BuildingID, FloorIndex, Boundary, DXFFileName, DXFAccessedTime, _3DFileName, _3DAccessedTime, BroadcastName from Zone";
	//        ArrayList arrResult = webDB.GetResultData(strSQL, 0);

	//        if (arrResult == null)
	//            return;

	//        DateTime dtDefault = new DateTime();
	//        int nResultCount = arrResult.Count;

	//        for (int i = 0; i < nResultCount - 9; i += 10)
	//        {
	//            int nID = SOPMonitoringSystem.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
	//            string strZoneName = SOPMonitoringSystem.WebDBManager.GetStringField(arrResult[i + 1], "");
	//            int nBuildingID = SOPMonitoringSystem.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
	//            int nFloorIndex = SOPMonitoringSystem.WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
	//            string strBoundary = SOPMonitoringSystem.WebDBManager.GetStringField(arrResult[i + 4], "");
	//            string strDXFFileName = SOPMonitoringSystem.WebDBManager.GetStringField(arrResult[i + 5], "");
	//            DateTime dtDXF = SOPMonitoringSystem.WebDBManager.GetDateTimeField(arrResult[i + 6], dtDefault);
	//            string str3DFileName = SOPMonitoringSystem.WebDBManager.GetStringField(arrResult[i + 7], "");
	//            DateTime dt3D = SOPMonitoringSystem.WebDBManager.GetDateTimeField(arrResult[i + 8], dtDefault);
	//            string strBroadcastName = SOPMonitoringSystem.WebDBManager.GetStringField(arrResult[i + 9], "");

	//            Zone zone = new Zone();

	//            zone.ID = nID;
	//            zone.ZoneName = strZoneName;
	//            zone.FloorIndex = nFloorIndex;
	//            if (strBroadcastName == "null" || strBroadcastName == "")
	//                zone.BroadcastName = strZoneName;
	//            else
	//                zone.BroadcastName = strBroadcastName;


	//            if (m_dicBuildings.ContainsKey(nBuildingID))
	//                zone.Building = m_dicBuildings[nBuildingID];

	//            try
	//            {
	//                zone.Polygon = MakeZonePolygon(strBoundary);
	//            }
	//            catch (System.Exception ex)
	//            {
	//                MessageBox.Show("Make polygo error!!");
	//            }

	//            m_dicZones[nID] = zone;

	//            if (nBuildingID < 0)
	//                m_dicOutdoorZones[nID] = zone;

	//        }

	//    }

	//    public Zone  GetZone(string buildingID , int floorIndex)
	//    {
	//        foreach (KeyValuePair<int, Zone> kv in m_dicZones)
	//        {
	//            SOPDisasterSystem.Zone obj = (SOPDisasterSystem.Zone)(kv.Value);
	//            if (obj.Building != null && buildingID == obj.Building.BuildingID)
	//            {
	//                if (obj.FloorIndex == floorIndex)
	//                {
	//                    return obj;
	//                }
	//            }
	//        }
	//        return null;
	//    }

	//    public Zone GetZone(int nZoneID)
	//    {
	//        Zone zone;
	//        if (m_dicZones.TryGetValue(nZoneID,out zone))
	//        {
	//            return zone;
	//        }
	//        return null;
	//    }
			

	//    public string GetBuildingID(int nID)
	//    {
	//        if (!m_dicBuildings.ContainsKey(nID))
	//            return "";
	//        Building b = m_dicBuildings[nID];
	//        return b.BuildingID;
	//    }

	//    public Building GetBuilding(string szBuildingID)
	//    {
	//        foreach (KeyValuePair<int, Building> kv in m_dicBuildings)
	//        {
	//            SOPDisasterSystem.Building obj = (SOPDisasterSystem.Building)(kv.Value);
	//            if (szBuildingID == obj.BuildingID)
	//            {
	//                return obj;                    
	//            }
	//        }
	//        return null;
	//    }

	//    public string CheckZoneName(float x, float y)
	//    {
	//        string szResult = "";
	//        foreach (KeyValuePair<int, Zone> kv in m_dicOutdoorZones)
	//        {
	//            SOPDisasterSystem.Zone obj = (SOPDisasterSystem.Zone)(kv.Value);
	//            if (obj != null && obj.Polygon != null && obj.Polygon.GetVertexCount() >= 3)
	//            {
	//                if (obj.Polygon.HitTest(new UnE.Geometry.Vertex2D(x, y)) == 1)
	//                {
	//                    szResult += obj.BroadcastName;
	//                }
	//            }
	//        }
	//        return szResult;
	//        //return "...";
	//    }

	//}


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

		public ArrayList LoadFireEquipment(string BuildingID, int nFloorIdx)
		{
			string szSQP = "SELECT ID, EquipID, EquipType, ZoneID, X, Y, Z, Description FROM FireEquipment";
			string subSQL = string.Format(" where EquipType = '1' and ZoneID = ( select Zone.ID from Zone where Zone.BuildingID = " +
				"( SELECT TOP 1 ID FROM Building where BuildingID = '{0}') and FloorIndex = '{1}')", BuildingID, nFloorIdx);
			szSQP += subSQL;

			SOPMonitoringSystem.WebDBManager webDB = SOPMonitoringSystem.FormMain.Instance.DBManager;

			ArrayList arrResult = webDB.GetResultData(szSQP, 0);
			if (arrResult == null)
				return null;

			int nResultCount = arrResult.Count;

			ArrayList arResult = new ArrayList();
			for (int i = 0; i < nResultCount - 7; i += 8)
			{
				try
				{
					int nID = SOPMonitoringSystem.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
					string szEquipID = SOPMonitoringSystem.WebDBManager.GetStringField(arrResult[i + 1], "");
					int nEquipType = SOPMonitoringSystem.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
					int nZoneID = SOPMonitoringSystem.WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
					float fX = SOPMonitoringSystem.WebDBManager.GetFloatField(arrResult[i + 4].ToString(), 0.0f);
					float fY = SOPMonitoringSystem.WebDBManager.GetFloatField(arrResult[i + 5].ToString(), 0.0f);
					float fZ = SOPMonitoringSystem.WebDBManager.GetFloatField(arrResult[i + 6].ToString(), 0.0f);
					string szDesc = SOPMonitoringSystem.WebDBManager.GetStringField(arrResult[i + 7], "");

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
			WebDBManager webDB = SOPMonitoringSystem.FormMain.Instance.DBManager;

			string strSQL = "select Building.id, BuildingID, BuildingCode, BuildingName, BuildingGroupID, MaxFloor, MinFloor,"
					 + "BuildingGroup.GroupName, BuildingGroup.TextCenter, Building.BroadCastingText ";
			strSQL += "from Building, BuildingGroup where Building.BuildingGroupID = BuildingGroup.ID";

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

                        string[] xy = strGroupNamePos.Split(',');
                        float x, y;
                        float.TryParse(xy[0], out x);
                        float.TryParse(xy[1], out y);
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
			WebDBManager webDB = SOPMonitoringSystem.FormMain.Instance.DBManager;

			string strSQL = "select id, ZoneName, BuildingID, FloorIndex, Boundary, DXFFileName, DXFAccessedTime, _3DFileName, _3DAccessedTime, BroadcastName, AddFloor from Zone";
			ArrayList arrResult = webDB.GetResultData(strSQL, 0);

			if (arrResult == null)
				return;

			DateTime dtDefault = new DateTime();
			int nResultCount = arrResult.Count;

			for (int i = 0; i < nResultCount -10; i += 11)
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

				Zone zone = new Zone();

				zone.ID = nID;
				zone.ZoneName = strZoneName;
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
					MessageBox.Show("Make polygo error!!");
				}

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
            WebDBManager dbMgr = SOPMonitoringSystem.FormMain.Instance.DBManager;

            string strSQL = "select ID, ZoneName, LinkedZoneIDList, Type, BroadcastName from EquipmentZone";

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

            //PrintEquipZone();
        }

        //private void PrintEquipZone()
        //{
        //    System.IO.StreamWriter writer = new StreamWriter("c:/test.txt", false, Encoding.UTF8);
        //    System.IO.StreamWriter writer1 = new StreamWriter("c:/test1.txt", false, Encoding.UTF8);
        //    System.IO.StreamWriter writer2 = new StreamWriter("c:/test2.txt", false, Encoding.UTF8);

        //    foreach (KeyValuePair<int, EquipmentZone> pair in m_dicEquipZones)
        //    {
        //        EquipmentZone equipZone = pair.Value;

        //        if (equipZone.LinkedZoneList.Count == 0)
        //            continue;

        //        Zone zone = (Zone)equipZone.LinkedZoneList[0];

        //        /*string strLine = string.Format("{0},{1},{2},{3}",
        //            equipZone.ID,
        //            equipZone.EquipZoneName,
        //            zone.Building.BuildingName,
        //            zone.Floor.ToString()
        //            );

        //        writer.WriteLine(strLine);*/
        //        string strLine1 = string.Format("{0}, {1}",
        //            equipZone.ID,
        //            equipZone.EquipZoneName);

        //        writer.WriteLine(strLine1);
        //        writer1.WriteLine(zone.Building.BuildingName);
        //        writer2.WriteLine(zone.Floor.ToString());
        //        writer.Flush();
        //    }

        //    writer.Close();
        //    writer1.Close();
        //    writer2.Close();
        //}

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
	}

}
