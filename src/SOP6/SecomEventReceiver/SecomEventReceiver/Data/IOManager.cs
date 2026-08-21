using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using DBUtility2;
using UnE.Spatial;
using UnE.Sensor;

namespace SecomEventReceiver
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

        private Dictionary<int, SensorZone> d_SensorZone = new Dictionary<int, SensorZone>();

        private Dictionary<int, EquipmentZone> m_dicEquipZones = new Dictionary<int, EquipmentZone>();

        // Zone에 속해있는 EquipmentZone List
        private Dictionary<Zone, ArrayList> m_dicZoneEquipZones = new Dictionary<Zone, ArrayList>();

        private int m_nSiteID = 1;

        public IOManager(int nSiteID)
        {
            m_nSiteID = nSiteID;

            LoadBuildings();
            LoadZones();
            LoadEquipmentZone();
            LoadSensorZone();

			LoadReciverList();
        }
        

        public void LoadBuildings()
        {
            Dictionary<int, BuildingGroup> dic_BuildingGroup = new Dictionary<int, BuildingGroup>();

            WebDBManager m_dbMgr = S1NetworkServer.Instance.DBManager;
            
            string szText = "SELECT bd.id, bd.BuildingID,  bd.BuildingCode, bd.BuildingName, bd.BuildingGroupID, bd.MaxFloor,bd. MinFloor," +
                           " bdg.GroupName FROM Building as bd, BuildingGroup as bdg " +
                           " WHERE bd.BuildingGroupID = bdg.ID and bdg.SiteID = {0}";

            string strSQL = string.Format(szText, m_nSiteID);

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;
            int nResultCount = arrResult.Count;
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
                    group.GroupID = nBuildingGroupID;
                    group.BuildingGroupName = strBuildingGroupName;

                    dic_BuildingGroup[nBuildingGroupID] = group;
                    building.BuildingGroup = group;
                }

                building.ID = nID;
                building.BuildingName = strBuildingName;

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
        }

        public void LoadZones()
        {
            WebDBManager m_dbMgr = S1NetworkServer.Instance.DBManager;

            //string strSQL = "select id, ZoneName, BuildingID, FloorIndex, AddFloor, Boundary, DXFFileName from Zone";            
            string szText = "select id, ZoneName, BuildingID, FloorIndex, AddFloor, Boundary, DXFFileName from Zone where SiteID = {0}";
            
            string strSQL = string.Format(szText, m_nSiteID);

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
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
            WebDBManager m_dbMgr = S1NetworkServer.Instance.DBManager;

            //string strSQL = "select ID,Type, Connected, EquipZoneID, Data from SensorZone";
            string szText = "SELECT sz.ID,sz.Type,sz.Connected, sz.EquipZoneID, sz.Data " +
                              " FROM SensorZone as sz, EquipmentZone as ez WHERE sz.EquipZoneID = ez.ID and ez.SiteID = {0}";
            
            string strSQL = string.Format(szText, m_nSiteID);

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

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
                {
                    equipZone = null;
                    //continue;
                }

                SensorZone sensorZone = new SensorZone();

                sensorZone.ID = nID;
                sensorZone.SetType(IFacility.ToFacilityType(nType));
                sensorZone.EquipmentZone = equipZone;

                d_SensorZone[nID] = sensorZone;

                if (equipZone != null)
                {
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
        }

        public void LoadEquipmentZone()
        {
            WebDBManager m_dbMgr = S1NetworkServer.Instance.DBManager;

            string szText = "SELECT ID, ZoneName, LinkedZoneIDList, Type FROM EquipmentZone where SiteID = {0}";
            string strSQL = string.Format(szText, m_nSiteID);

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

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

                EquipmentZone.EquipZoneType equipZoneType = EquipmentZone.ToEquipZoneType(nType);

                if (equipZoneType == EquipmentZone.EquipZoneType.NOTUSED)
                    continue;

                EquipmentZone equipZone = new EquipmentZone();

                equipZone.ID = nID;
                equipZone.ZoneName = strEquipZoneName;
                equipZone.ZoneType = equipZoneType;

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

        private ArrayList m_arPSMReciverList = new ArrayList();
        public ArrayList GetPSMReciverList()
        {
            return m_arPSMReciverList;
        }

        // 테스트 용 함수
        //public Reciver FindReciver(string szReciverID, string szIPAddres, string szName )
        //{
            

        //    foreach(Reciver reciver in m_arReciverList)
        //    {
        //        if ( reciver.Place.Contains(szReciverID))
        //        {
        //            foreach(Circuit circuit in reciver.Curcuits.Values)
        //            {
        //                if( circuit.Name == szName)
        //                    return reciver;
        //            }                
        //        }
        //    }
        //    return null;
        //}
        public Reciver FindReciver(int nReciverID)
        {
            foreach (Reciver reciver in m_arReciverList)
            {

               
                if (reciver.ID == nReciverID)
                {
                    return reciver;
                }
                
            }
            return null;
        }
        public Reciver FindReciver(int nReciverID, int nReciverType)
        {
            foreach (Reciver reciver in m_arReciverList)
            {

                if ((int)reciver.Type == nReciverType)
                {
                    if (reciver.ID == nReciverID)
                    {
                        return reciver;
                    }
                }
            }
            return null;
        }

        // 485 Unit ID로 리시버 검색
        public Reciver FindReciverForUnitID(int nUnitID, int nReciverType)
        {
            foreach (Reciver reciver in m_arReciverList)
            {
                if((int)reciver.Type == nReciverType)
                {
                    if (reciver.ReceiverID == nUnitID)
                    {
                        return reciver;
                    }
                }
                
            }
            return null;
        }

        //// 실제 사용되는 함수
        //public Reciver FindReciver(string szReciverID, string szIPAddres )
        //{
        //    string szAddr = GetIPAddress(szIPAddres);
        //    foreach(Reciver reciver in m_arReciverList)
        //    {

        //        if (reciver.Address == szAddr && reciver.Place.Contains(szReciverID))
        //        {
        //            return reciver;
        //        }
        //    }
        //    return null;
        //}

        public ArrayList FindRecivers(string szIPAddress)
        {

            string szAddr = GetIPAddress(szIPAddress);
            ArrayList arResult = new ArrayList();
            foreach (Reciver reciver in m_arReciverList)
            {
                if (reciver.Address == szAddr)
                {
                    arResult.Add(reciver);
                }
            }
            return arResult;
        }


        private ArrayList m_arIPList = new ArrayList();
        public string GetIPAddress(string szIPAddress)
        {
            if (m_arIPList.Contains(szIPAddress))
                return szIPAddress;
            return (string)m_arIPList[1];
        }

        public bool IsValidReciver(string szIP)
        {
            if (m_arIPList.Contains(szIP))
                return true;
            return false;
        }


		public void LoadReciverList()
		{
            WebDBManager m_dbMgr = S1NetworkServer.Instance.DBManager;
            string strSQL = "select ID,Place, IP, MacAddr, Baudrate, Mode, FlowCtrl, Multiport, Timeout, Description, ReciverID, ReciverType from SensorServerInfo where SiteID =" + m_nSiteID.ToString();

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

			for (int i = 0; i < nResultCount - 11; i += 12)
			{
				int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
				string strPlace = WebDBManager.GetStringField(arrResult[i + 1], "");
				string strIP = WebDBManager.GetStringField(arrResult[i + 2], "");
				string strMac = WebDBManager.GetStringField(arrResult[i + 3], "");
				int nBuadrate = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);

				int nMode = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
				int nFlow = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
				int nPort = WebDBManager.GetIntField(arrResult[i + 7].ToString(), -1);
				int nTimeout = WebDBManager.GetIntField(arrResult[i + 8].ToString(), -1);				
				string strDesc = WebDBManager.GetStringField(arrResult[i +9], "");
                int nReciverID = WebDBManager.GetIntField(arrResult[i + 10].ToString(), -1);
				int nReciverType = WebDBManager.GetIntField(arrResult[i + 11].ToString(), -1);
                Reciver reciver = new Reciver();
				reciver.ID = nID;
				reciver.Place = strPlace;
				reciver.Address = strIP;
                reciver.Type = Reciver.ToReciverType(nReciverType);

                if (!m_arIPList.Contains(strIP))
                    m_arIPList.Add(strIP);
                reciver.ReceiverID = nReciverID;
                //reciver.MacAddress = strMac;
                //reciver.Port = nPort;
                //reciver.Mode = nMode;
                //reciver.FlowCtrl = nFlow;
				reciver.Timeout = nTimeout;
				//reciver.BuadRate = nBuadrate;

				LoadCurcuit(reciver);

				m_arReciverList.Add(reciver);

                if (nReciverType == 2)
                    m_arPSMReciverList.Add(reciver);
			}
		}

      

		private bool LoadCurcuit(Reciver reciver)
		{
            WebDBManager m_dbMgr = S1NetworkServer.Instance.DBManager;
            string strSQL = string.Format("select ID, TagNo, SensorName, SensorType, SensorZoneID, Description from SensorTagInfo where SensorServerID = {0}", reciver.ID);
			
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

			if (arrResult == null)
				return false;

            int nResultCount = arrResult.Count;

			for (int i = 0; i < nResultCount - 5; i += 6)
			{
				int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
				int nTagNo = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);

				string strName = WebDBManager.GetStringField(arrResult[i + 2], "");
				int nSensorType = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                int nSensorZoneID = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);

				string strDesc = WebDBManager.GetStringField(arrResult[i + 5], "");

				Circuit circuit = new Circuit();

				circuit.ID = nID;
				circuit.TagNum = nTagNo;
				circuit.ReciverID = reciver.ID;
				circuit.SensorType = IFacility.ToFacilityType(nSensorType);
                circuit.SensorZoneID = nSensorZoneID;
				circuit.Name = strName;

				if (!reciver.Circuits.ContainsKey(nTagNo))
				{
					reciver.Circuits.Add(nTagNo, circuit);
				}
				
			}
			return true;
		}

        public SensorZone GetSensorZone(int nID)
        {
            SensorZone sensorZone;

            if (d_SensorZone.TryGetValue(nID, out sensorZone))
                return sensorZone;

            return null;
        }

        public EquipmentZone GetEquipmentZone(int nEquipZoneID)
        {
            EquipmentZone equipZone;

            if (m_dicEquipZones.TryGetValue(nEquipZoneID, out equipZone))
                return equipZone;

            return null;
        }
    }

    public class SensorZone : IFacility
    {
        private FacilityType m_type = FacilityType.NONE;
        private EquipmentZone m_equipZone = null;

        public override string DisconnectIconPath
        {
            get { return ""; }
        }

        public override string IconPath
        {
            get { return ""; }
        }

        public override int GetLayerID()
        {
            return 0;
        }

        public override FacilityType Type
        {
            get { return m_type; }
        }

        public EquipmentZone EquipmentZone
        {
            get { return m_equipZone; }
            set { m_equipZone = value; }
        }

        public void SetType(FacilityType type)
        {
            m_type = type;
        }
    }
}
