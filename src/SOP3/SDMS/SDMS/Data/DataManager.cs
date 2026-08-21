using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using DBUtility;

namespace SDMS
{
    public class DataManager
    {
        class FireEquipmentHistory
        {
            private int m_nHistoryID = -1;
            private int m_nEquipID = -1;
            private DateTime m_time = new DateTime();
            private int m_nStatus = -1;
            private string m_strOpinion = "";

            public int HistoryID
            {
                get { return m_nHistoryID; }
                set { m_nHistoryID = value; }
            }

            public int EquipID
            {
                get { return m_nEquipID; }
                set { m_nEquipID = value; }
            }

            public DateTime LastCheckedTime
            {
                get { return m_time; }
                set { m_time = value; }
            }

            public int Status
            {
                get { return m_nStatus; }
                set { m_nStatus = value; }
            }

            public string CheckersOpinion
            {
                get { return m_strOpinion; }
                set { m_strOpinion = value; }
            }
        }

        private DataTeam m_teamRegularRoot = null;
        private ArrayList m_listExternalRootTeams = new ArrayList();
        private Dictionary<int, DataTeam> m_dicRegularTeams = new Dictionary<int, DataTeam>();
        private Dictionary<DataTeam, ArrayList> m_dicRegularTeamMembers = new Dictionary<DataTeam, ArrayList>();
        private Dictionary<int, DataCompanyMember> m_dicRegularMembers = new Dictionary<int, DataCompanyMember>();
        private Dictionary<int, DataTeam> m_dicExternalTeams = new Dictionary<int, DataTeam>();
        private Dictionary<DataTeam, ArrayList> m_dicExternalTeamMembers = new Dictionary<DataTeam, ArrayList>();
        private Dictionary<int, DataExternalMember> m_dicExternalMembers = new Dictionary<int, DataExternalMember>();
        private Dictionary<Zone, ArrayList> m_dicZoneFireEquipments = new Dictionary<Zone, ArrayList>();
        // 시설물 타입별 발전소 전체 담당자
        private Dictionary<Facility.FacilityType, FacilityManagerGroup> m_dicEntireFacilityManagers = new Dictionary<Facility.FacilityType, FacilityManagerGroup>();
        // 건물별 시설물 담당자
        private Dictionary<Facility.FacilityType, Dictionary<Building, FacilityManagerGroup>> m_dicBuildingFacilityManager = new Dictionary<Facility.FacilityType, Dictionary<Building, FacilityManagerGroup>>();
        // 외부 Zone별 시설물 담당자
        private Dictionary<Facility.FacilityType, Dictionary<Zone, FacilityManagerGroup>> m_dicOutdoorFacilityManager = new Dictionary<Facility.FacilityType, Dictionary<Zone, FacilityManagerGroup>>();

		// EquipZone 별 시설물 담당장
		private Dictionary<Facility.FacilityType, Dictionary<int, FacilityManagerGroup>> m_dicEquipZoneFacilityManager = new Dictionary<Facility.FacilityType, Dictionary<int, FacilityManagerGroup>>();

        // 당직자용 데이터
        private DataTeamDuty m_teamDuty = new DataTeamDuty();

        private static string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

        public DataCompanyMember GetCompanyMember(int nMemberID)
        {
            if(m_dicRegularMembers.ContainsKey(nMemberID))
            {
                return m_dicRegularMembers[nMemberID];
            }
            return null;
        }

        public DataExternalMember GetExternalMember(int nMemberID)
        {
            if (m_dicExternalMembers.ContainsKey(nMemberID))
            {
                return m_dicExternalMembers[nMemberID];
            }
            return null;
        }

        public Dictionary<Zone, ArrayList> ZoneFireEquipments
        {
            get { return m_dicZoneFireEquipments; }
        }

        public DataTeamDuty TeamDuty
        {
            get { return m_teamDuty; }
        }

        public FacilityManager NewFacilityManagerDuty()
        {           
            FacilityManager facilityManagerDuty = new FacilityManager();

            facilityManagerDuty.MemberID = 0;
            facilityManagerDuty.MemberType = 6;

            return facilityManagerDuty;
        }

        public DataManager(DBUtility.WebDBManager dbMgr)
        {
            //Dictionary<int, DataTeam> dicRegularTeams = new Dictionary<int, DataTeam>();
            //Dictionary<int, DataTeam> dicExternalTeams = new Dictionary<int, DataTeam>();
        
            m_teamRegularRoot = LoadRegularTeam(dbMgr, m_dicRegularTeams);
            m_listExternalRootTeams = LoadExternalTeam(dbMgr, m_dicExternalTeams);

            LoadCompanyMember(dbMgr, m_dicRegularTeams);
            LoadExternalMember(dbMgr, m_dicExternalTeams);
        }


		public void ReloadCompanyMember()
		{
			WebDBManager dbMgr = FormMain.Instance.DBManager;
			m_teamRegularRoot = LoadRegularTeam(dbMgr, m_dicRegularTeams);
			m_listExternalRootTeams = LoadExternalTeam(dbMgr, m_dicExternalTeams);

			LoadCompanyMember(dbMgr, m_dicRegularTeams);
			LoadExternalMember(dbMgr, m_dicExternalTeams);
		}


        public bool LoadCompanyMember(DBUtility.WebDBManager dbMgr, Dictionary<int, DataTeam> dicTeams)
        {
            string strSQL = "select ID, MemberName, RegularTeamID, LevelID, PositionID, MemberID, SecondRegularTeamID, SecondPositionID, OfficePhoneNumber, PhoneNumber from CompanyMember";

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null) return false;

            int nCount = arrResult.Count;
            if (nCount == 0) return true;

            for (int i = 0; i < nCount - 9; i += 10)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strMemberName = DBUtility.WebDBManager.GetStringField(arrResult[i + 1], "");
                int nRegularTeamID = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
                int nLevelID = DBUtility.WebDBManager.GetIntField(arrResult[i + 3].ToString(), 0);
                int nPositionID = DBUtility.WebDBManager.GetIntField(arrResult[i + 4].ToString(), 0);
                string strMemberID = DBUtility.WebDBManager.GetStringField(arrResult[i + 5], "");
                int nSecondRegularTeamID = DBUtility.WebDBManager.GetIntField(arrResult[i + 6].ToString(), 0);
                int nSecondPositionID = DBUtility.WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0);
                string strOfficePhoneNumber = DBUtility.WebDBManager.GetStringField(arrResult[i + 8], "");
                string strPhoneNumber = DBUtility.WebDBManager.GetStringField(arrResult[i + 9], "");

                if (string.Compare(strPhoneNumber, "null", true) == 0 || strPhoneNumber == "")
                    strPhoneNumber = "";
                else
                    strPhoneNumber = DBUtility.AES256Cipher.AES_decrypt(strPhoneNumber, key);

                strPhoneNumber = ValidPhoneNumber(strPhoneNumber);

                if (string.Compare(strOfficePhoneNumber, "null", true) == 0)
                    strOfficePhoneNumber = "";

                if (!dicTeams.ContainsKey(nRegularTeamID))
                    continue;

                DataTeam team = dicTeams[nRegularTeamID];

                DataCompanyMember data = new DataCompanyMember();
                data.ID = nID;
                data.MemberName = strMemberName;
                data.Team = team;
                data.LevelID = nLevelID;
                data.PositionID = nPositionID;
                data.MemberID = strMemberID;
                data.OfficePhoneNumber = strOfficePhoneNumber;
                data.PhoneNumber = strPhoneNumber;

                ArrayList arrMembers = null;

                if (m_dicRegularTeamMembers.ContainsKey(team))
                    arrMembers = m_dicRegularTeamMembers[team];
                else
                {
                    arrMembers = new ArrayList();
                    m_dicRegularTeamMembers[team] = arrMembers;
                }

                m_dicRegularMembers[nID] = data;
                arrMembers.Add(data);
                ////////////////////////////////////////////////////////////////
            }

            foreach (KeyValuePair<DataTeam, ArrayList> pair in m_dicRegularTeamMembers)
            {
                pair.Value.Sort();
            }

            return true;
        }

        public bool LoadExternalMember(DBUtility.WebDBManager dbMgr, Dictionary<int, DataTeam> dicTeams)
        {
            string szSQL = "SELECT ID, Name, PhoneNumber, IsTeamLeader, TeamID FROM ExternalCompanyMember";

            ArrayList arrResult = dbMgr.GetResultData(szSQL, 0);
            if (arrResult == null)
                return false;

            int nCount = arrResult.Count;
            if (nCount == 0)
                return true;

            for (int i = 0; i < nCount - 4; i += 5)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strMemberName = DBUtility.WebDBManager.GetStringField(arrResult[i + 1], "");
                string szPhoneNumber = DBUtility.WebDBManager.GetStringField(arrResult[i + 2].ToString(), "");
                bool nLeader = DBUtility.WebDBManager.GetIntField(arrResult[i + 3].ToString(), 0) == 1;
                int nTeamID = DBUtility.WebDBManager.GetIntField(arrResult[i + 4].ToString(), 0);

                if (!dicTeams.ContainsKey(nTeamID))
                    return false;

                DataTeam team = dicTeams[nTeamID];

                if (string.Compare(szPhoneNumber, "null", true) == 0 || szPhoneNumber == "")
                    szPhoneNumber = "";
                else
                    szPhoneNumber = DBUtility.AES256Cipher.AES_decrypt(szPhoneNumber, key);

                szPhoneNumber = ValidPhoneNumber(szPhoneNumber);

                DataExternalMember data = new DataExternalMember();
                data.ID = nID;
                data.Name = strMemberName;
                data.PhoneNumber = szPhoneNumber;
                data.TeamLeader = nLeader;
                data.Team = team;

                ArrayList arrMembers = null;

                if (m_dicExternalTeamMembers.ContainsKey(team))
                    arrMembers = m_dicExternalTeamMembers[team];
                else
                {
                    arrMembers = new ArrayList();
                    m_dicExternalTeamMembers[team] = arrMembers;
                }

                m_dicExternalMembers[nID] = data;
                arrMembers.Add(data);
            }

            return false;
        }

        private string ValidPhoneNumber(string strPhoneNumber)
        {
            string strResult = "";
            int nLen = strPhoneNumber.Length;

            for (int i = 0; i < nLen; i++)
            {
                char ch = strPhoneNumber[i];

                if (ch != ' ' && ch != '\t' && ch != '-')
                    strResult += ch;
            }
            return strResult;
        }

        // dicTeams : ID별 Team
        private ArrayList LoadExternalTeam(DBUtility.WebDBManager dbMgr, Dictionary<int, DataTeam> dicTeams)
        {
            string strSQL = "Select ID, TeamName from ExternalTeam";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return null;

            Dictionary<int, DataTeam> dicCompanies = new Dictionary<int, DataTeam>();
            ArrayList arrExternalRootTeams = new ArrayList();

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strTeamName = DBUtility.WebDBManager.GetStringField(arrResult[i + 1], "");

                DataTeam team = new DataTeam();
                team.ID = nID;
                team.TeamName = strTeamName;
                team.External = true;
                team.IsCompany = true;

                dicCompanies[nID] = team;
            }

            string szSQL = "SELECT ID, TeamName, ParentTeamID, CompanyID FROM ExternalCompanyTeam";

            arrResult = dbMgr.GetResultData(szSQL, 0);
            if (arrResult == null)
                return null;

            // 자신의 Team, 부모 팀의 ID
            Dictionary<DataTeam, int> dicParentID = new Dictionary<DataTeam, int>();
            
            nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string szTeamName = DBUtility.WebDBManager.GetStringField(arrResult[i + 1], "");
                int nParentTeamID = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nCompanyID = DBUtility.WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);

                if (!dicCompanies.ContainsKey(nCompanyID))
                    continue;

                DataTeam teamCompany = dicCompanies[nCompanyID];

                DataTeam data = new DataTeam();
                data.ID = nID;
                data.TeamName = szTeamName;
                data.External = true;
                data.CompanyName = teamCompany.TeamName;
                
                if (nParentTeamID == -1)
                {
                    data.ParentTeam = teamCompany;

                    if (!arrExternalRootTeams.Contains(teamCompany))
                    {
                        arrExternalRootTeams.Add(teamCompany);
                    }
                }

                dicParentID[data] = nParentTeamID;
                dicTeams[nID] = data;
            }

            foreach (KeyValuePair<DataTeam, int> pair in dicParentID)
            {
                if (pair.Key.ParentTeam != null)
                    continue;

                if (!dicTeams.ContainsKey(pair.Value))
                    continue;

                DataTeam teamParent = dicTeams[pair.Value];
                pair.Key.ParentTeam = teamParent;
            }

            return arrExternalRootTeams;
        }

        // dicTeams : ID별 Team
        private DataTeam LoadRegularTeam(DBUtility.WebDBManager dbMgr, Dictionary<int, DataTeam> dicTeams)
        {
            string szSQL = "SELECT R.ID, R.TeamName, R.ParentTeamID FROM RegularTeam as R";

            ArrayList arrResult = dbMgr.GetResultData(szSQL, 0);
            if (arrResult == null)
                return null;

            // 자신의 Team, 부모 팀의 ID
            Dictionary<DataTeam, int> dicParentID = new Dictionary<DataTeam, int>();
            
            int nCount = arrResult.Count;

            for (int i = 0; i < nCount - 2; i += 3)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string szTeamName = DBUtility.WebDBManager.GetStringField(arrResult[i + 1], "");
                int nParentTeamID = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);

                DataTeam data = new DataTeam();
                data.ID = nID;
                data.TeamName = szTeamName;
                data.External = false;

                dicTeams[nID] = data;
                dicParentID[data] = nParentTeamID;
            }

            DataTeam teamRoot = null;

            foreach (KeyValuePair<DataTeam, int> pair in dicParentID)
            {
                if (pair.Value < 0)
                {
                    teamRoot = pair.Key;
                    teamRoot.IsCompany = true;
                    continue;
                }

                if (!dicTeams.ContainsKey(pair.Value))
                    continue;

                DataTeam teamParent = dicTeams[pair.Value];
                pair.Key.ParentTeam = teamParent;
            }

            return teamRoot;
        }

        public bool LoadPOI(BaseViewEx view, bool isIndoor)
        {
            if (!CCTVManager.Instance.LoadCCTV(view, isIndoor))
                return false;

            if (!SensorManager.Instance.LoadAllSensor(view, isIndoor))
                return false;

            return true;
        }


		private SortedList<int, int> m_arGroupEquipPair = new SortedList<int, int>();			
		public void LoadFireEquipmentGroup()
		{
			DBUtility.WebDBManager dbMgr = FormMain.Instance.DBManager;
			string strSQL = "Select id, linkedEquipID from FireEquipmentGroup";
			ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

			if (arrResult == null)
				return;
			int nResultCount = arrResult.Count;			
			
			for (int i = 0; i < nResultCount - 1; i += 2)
			{
				int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
				int nEquipType = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
			
				if (nID < 0)
					continue;
				m_arGroupEquipPair.Add(nEquipType, nID);		
			}
		}
    
        public void LoadFireEquipment()
        {
			LoadFireEquipmentGroup();

            DBUtility.WebDBManager dbMgr = FormMain.Instance.DBManager;

            string strSQL = "Select ID, RFIDTag, EquipID, RFIDTagID, DxfObjID, EquipType, ZoneID, X, Y, Z, Description from FireEquipment";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            Dictionary<int, FireEquipmentHistory> dicHistory = LoadFireEquipmentHistory();

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 10; i += 11)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strRFIDTag = DBUtility.WebDBManager.GetStringField(arrResult[i + 1], "");
                string strEquipID = DBUtility.WebDBManager.GetStringField(arrResult[i + 2], "");
                string strRFIDTagID = DBUtility.WebDBManager.GetStringField(arrResult[i + 3], "");
                string strDxfObjID = DBUtility.WebDBManager.GetStringField(arrResult[i + 4], "");
                int nEquipType = DBUtility.WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                int nZoneID = DBUtility.WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                float x = DBUtility.WebDBManager.GetFloatField(arrResult[i + 7].ToString(), 0.0f);
                float y = DBUtility.WebDBManager.GetFloatField(arrResult[i + 8].ToString(), 0.0f);
                float z = DBUtility.WebDBManager.GetFloatField(arrResult[i + 9].ToString(), 0.0f);
                string strDescription = DBUtility.WebDBManager.GetStringField(arrResult[i + 10], "");

                if (nID < 0)
                    continue;

                Facility.FacilityType type = Facility.FacilityType.NONE;

                if (nEquipType == 1)
                    type = Facility.FacilityType.FE;
                else if (nEquipType == 2)
                    type = Facility.FacilityType.HD;
				else if (nEquipType == 3)
				{
					type = Facility.FacilityType.FA;
					continue;
				}
				else
					continue;

                FireEquipment equip = new FireEquipment();

                equip.ID = nID;

				int groupID = -1;
				if (m_arGroupEquipPair.TryGetValue(nID, out groupID))
				{
					equip.GroupID = groupID;
				}
				else
				{
					equip.GroupID = -1;
				}


                equip.Description = strDescription;
                equip.EquipID = strEquipID;
                equip.RFIDTag = strRFIDTag;
                equip.SetType(type);
               
                equip.Zone = ZoneManager.Instance.GetZone(nZoneID);

                if (equip.Zone == null)
                    continue;
                Zone zone = equip.Zone;

                if (dicHistory.ContainsKey(equip.ID))
                {
                    FireEquipmentHistory history = dicHistory[equip.ID];

                    equip.LastCheckedTime = history.LastCheckedTime;
                    equip.Status = (FireEquipment.EquipmentStatus)history.Status;
                    equip.CheckersOpinion = history.CheckersOpinion;
                }
				
				equip.X = x;
				equip.Y = 0.1f;
				equip.Z = y;

				float dx = 0;
				float dz = 0;
				if (zone.IsOutdoor == false)
				{
					UnE.Geometry.Vertex2D pos = zone.Polygon.CalcWeightCenter();					
					dx = (float)pos.x;
					dz = (float)pos.y;
					float pos3DX = x - dx;
					float pos3DZ = dz - y;
					equip.X = pos3DX;
					equip.Y = 0.1f;
					equip.Z = pos3DZ;
				}

                ArrayList arrEquipments = null;

                if (m_dicZoneFireEquipments.ContainsKey(equip.Zone))
                    arrEquipments = m_dicZoneFireEquipments[equip.Zone];
                else
                {
                    arrEquipments = new ArrayList();
                    m_dicZoneFireEquipments[equip.Zone] = arrEquipments;
                }
                arrEquipments.Add(equip);
            }
        }

        private Dictionary<int, FireEquipmentHistory> LoadFireEquipmentHistory()
        {
            string strSQL = "select ID, FireEquipmentID, Time, status, CheckersOpinion from FireEquipmentHistory order by FireEquipmentID";

            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);
            Dictionary<int, FireEquipmentHistory> dicHistory = new Dictionary<int, FireEquipmentHistory>();

            if (arrResult == null)
                return dicHistory;

            int nResultCount = arrResult.Count;
            DateTime dtDefault = new DateTime();

            int nPrevEquipID = -1;
            DateTime dtPrev = new DateTime();
            string strPrevOpinion = "";
            int nPrevStatus = -1;
            int nPrevHistoryID = -1;

            for (int i = 0; i < nResultCount - 4; i += 5)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nEquipID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                DateTime dtLastChecked = DBUtility.WebDBManager.GetDateTimeField(arrResult[i + 2], dtDefault);
                int nStatus = DBUtility.WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                string strOpinion = DBUtility.WebDBManager.GetStringField(arrResult[i + 4], "");

                if (nEquipID < 0 || nStatus < 0)
                    continue;

                if (nEquipID != nPrevEquipID)
                {
                    if (nPrevEquipID > 0)
                        AddFireEquipmentHistory(dicHistory, nPrevHistoryID, nPrevEquipID, dtPrev, nPrevStatus, strPrevOpinion);
                }

                nPrevHistoryID = nID;
                nPrevEquipID = nEquipID;
                dtPrev = dtLastChecked;
                nPrevStatus = nStatus;
                strPrevOpinion = strOpinion;
            }

            if (nPrevEquipID > 0)
                AddFireEquipmentHistory(dicHistory, nPrevHistoryID, nPrevEquipID, dtPrev, nPrevStatus, strPrevOpinion);

            return dicHistory;
        }

        private void AddFireEquipmentHistory(Dictionary<int, FireEquipmentHistory> dicHistory, int nHistoryID, int nEquipID, DateTime dtLastChecked, int nStatus, string strCheckersOpinion)
        {
            FireEquipmentHistory history = new FireEquipmentHistory();

            history.HistoryID = nHistoryID;
            history.EquipID = nEquipID;
            history.LastCheckedTime = dtLastChecked;
            history.Status = nStatus;
            history.CheckersOpinion = strCheckersOpinion;

            dicHistory[nEquipID] = history;
        }

		public void AddEquipZoneFacilityManager(FacilityManager mgr, EquipmentZone zone, Facility.FacilityType type)
		{
            if (m_dicEquipZoneFacilityManager.ContainsKey(type))
			{
				Dictionary<int, FacilityManagerGroup> dicManagers = m_dicEquipZoneFacilityManager[type];

				if (dicManagers.ContainsKey(zone.ID))
				{
					FacilityManagerGroup group = dicManagers[zone.ID];
					group.AddManager(mgr);
				}
				else
				{
					FacilityManagerGroup group = new FacilityManagerGroup();
					group.Type = type;
					group.EquipZone = zone;

					group.AddManager(mgr);
					dicManagers[zone.ID] = group;
				}
			}
			else
			{
				Dictionary<int, FacilityManagerGroup> dicManagers = new Dictionary<int, FacilityManagerGroup>();
				m_dicEquipZoneFacilityManager[type] = dicManagers;

				FacilityManagerGroup group = new FacilityManagerGroup();
				group.EquipZone = zone;
				group.Type = type;

				group.AddManager(mgr);
				dicManagers[zone.ID] = group;
			}
		}

        public void AddBuildingFacilityManager(FacilityManager mgr, Building building, Facility.FacilityType type)
        {
            if (m_dicBuildingFacilityManager.ContainsKey(type))
            {
                Dictionary<Building, FacilityManagerGroup> dicManagers = m_dicBuildingFacilityManager[type];

                if (dicManagers.ContainsKey(building))
                {
                    FacilityManagerGroup group = dicManagers[building];
                    group.AddManager(mgr);
                }
                else
                {
                    FacilityManagerGroup group = new FacilityManagerGroup();
                    group.Building = building;
                    group.Type = type;

                    group.AddManager(mgr);
                    dicManagers[building] = group;
                }
            }
            else
            {
                Dictionary<Building, FacilityManagerGroup> dicManagers = new Dictionary<Building, FacilityManagerGroup>();
                m_dicBuildingFacilityManager[type] = dicManagers;

                FacilityManagerGroup group = new FacilityManagerGroup();
                group.Building = building;
                group.Type = type;

                group.AddManager(mgr);
                dicManagers[building] = group;
            }
        }

        public void AddOutdoorFacilityManager(FacilityManager mgr, Zone zone, Facility.FacilityType type)
        {
            if (m_dicOutdoorFacilityManager.ContainsKey(type))
            {
                Dictionary<Zone, FacilityManagerGroup> dicManagers = m_dicOutdoorFacilityManager[type];

                if (dicManagers.ContainsKey(zone))
                {
                    FacilityManagerGroup group = dicManagers[zone];
                    group.AddManager(mgr);
                }
                else
                {
                    FacilityManagerGroup group = new FacilityManagerGroup();
                    group.Zone = zone;
                    group.Type = type;

                    group.AddManager(mgr);
                    dicManagers[zone] = group;
                }
            }
            else
            {
                Dictionary<Zone, FacilityManagerGroup> dicManagers = new Dictionary<Zone, FacilityManagerGroup>();
                m_dicOutdoorFacilityManager[type] = dicManagers;

                FacilityManagerGroup group = new FacilityManagerGroup();
                group.Zone = zone;
                group.Type = type;

                group.AddManager(mgr);
                dicManagers[zone] = group;
            }
        }

        public void AddFacilityManager(FacilityManager mgr, Facility.FacilityType type)
        {
            if (m_dicEntireFacilityManagers.ContainsKey(type))
            {
                FacilityManagerGroup group = m_dicEntireFacilityManagers[type];
                group.AddManager(mgr);
            }
            else
            {
                FacilityManagerGroup group = new FacilityManagerGroup();
                group.Type = type;

                group.AddManager(mgr);
                m_dicEntireFacilityManagers[type] = group;
            }
        }

        public void LoadFacilityManager()
        {
            m_dicEntireFacilityManagers.Clear();
            m_dicBuildingFacilityManager.Clear();
            m_dicOutdoorFacilityManager.Clear();
			m_dicEquipZoneFacilityManager.Clear();

            DBUtility.WebDBManager dbMgr = FormMain.Instance.DBManager;

            string strSQL = "select id, MemberID, MemberType, FacilityType, LevelLimit, UpperLimit, Description from FacilityManager order by FacilityType";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 6; i += 7)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nMemberID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nMemberType = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nFacilityType = DBUtility.WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                int nLevelLimit = DBUtility.WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                int nUseUpper = DBUtility.WebDBManager.GetIntField(arrResult[i + 5].ToString(), 0);
                string strDescription = DBUtility.WebDBManager.GetStringField(arrResult[i + 6], "");

                if (nID < 0 || nMemberID < 0)
                    continue;

                FacilityManagerGroup group = GetFacilityManagerGroup(nFacilityType);
                if (group == null)
                    continue;

                AddFacilityManager(nID, nMemberID, nMemberType, nFacilityType, nLevelLimit, nUseUpper, strDescription, group);
            }

            strSQL = "select id, MemberID, MemberType, FacilityType, LevelLimit, BuildingID, UpperLimit, Description from BuildingFacilityManager order by FacilityType";
            arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            nResultCount = arrResult.Count;


            for (int i = 0; i < nResultCount - 7; i += 8)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nMemberID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nMemberType = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nFacilityType = DBUtility.WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                int nLevelLimit = DBUtility.WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                int nBuildingID = DBUtility.WebDBManager.GetIntField(arrResult[i + 5].ToString(), 0);
				int nUseUpper = DBUtility.WebDBManager.GetIntField(arrResult[i + 6].ToString(), 0);
                string strDescription = DBUtility.WebDBManager.GetStringField(arrResult[i + 7], "");
				

                if (nID < 0 || nMemberID < 0)
                    continue;

                if (nBuildingID == 0)
                    continue;

                FacilityManagerGroup group = null;

                if (nBuildingID > 0)
                {
                    if (!ZoneManager.Instance.DicBuildings.ContainsKey(nBuildingID))
                        continue;

                    Building building = ZoneManager.Instance.DicBuildings[nBuildingID];
                    group = GetBuildingFacilityManagerGroup(nFacilityType, building);
                }
                else if (nBuildingID < 0)
                {
                    Zone zone = ZoneManager.Instance.GetZone(-nBuildingID);

                    if (zone == null)
                        continue;

                    group = GetOutdoorFacilityManagerGroup(nFacilityType, zone);
                }

                if (group == null)
                    continue;

				AddFacilityManager(nID, nMemberID, nMemberType, nFacilityType, nLevelLimit, nUseUpper, strDescription, group);
            }


			// Add EquipZone Facility Manager
			strSQL = "select id, MemberID, MemberType, FacilityType, LevelLimit, EquipZoneID, UpperLimit, Description from EquipZoneFacilityManager order by FacilityType";
			arrResult = dbMgr.GetResultData(strSQL, 0);

			if (arrResult == null)
				return;

			nResultCount = arrResult.Count;


			for (int i = 0; i < nResultCount - 7; i += 8)
			{
				int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
				int nMemberID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
				int nMemberType = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
				int nFacilityType = DBUtility.WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
				int nLevelLimit = DBUtility.WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
				int nEquipZoneID = DBUtility.WebDBManager.GetIntField(arrResult[i + 5].ToString(), 0);
				int nUseUpper = DBUtility.WebDBManager.GetIntField(arrResult[i + 6].ToString(), 0);
				string strDescription = DBUtility.WebDBManager.GetStringField(arrResult[i + 7], "");


				if (nID < 0 || nMemberID < 0)
					continue;

				if (nEquipZoneID == 0)
					continue;

				FacilityManagerGroup group = null;

				if (nEquipZoneID > 0)
				{
					if (!ZoneManager.Instance.DicEquipZones.ContainsKey(nEquipZoneID))
						continue;

					EquipmentZone zone = ZoneManager.Instance.DicEquipZones[nEquipZoneID];
					group = GetEquipZoneFacilityManagerGroup(nFacilityType, zone);
				}				

				if (group == null)
					continue;

				AddFacilityManager(nID, nMemberID, nMemberType, nFacilityType, nLevelLimit, nUseUpper, strDescription, group);
			}
        }

        private void AddFacilityManager(int nID, int nMemberID, int nMemberType, int nFacilityType, int nLevelLimit, int nUseUpperLevel, string strDescription, FacilityManagerGroup group)
        {
            FacilityManager mgr = new FacilityManager();
            mgr.ID = nID;
            mgr.MemberID = nMemberID;
            mgr.MemberType = nMemberType;
            mgr.Type = Facility.ToFacilityType(nFacilityType);
            mgr.LevelLimit = nLevelLimit;
			mgr.UpperLimit = nUseUpperLevel;
            mgr.Description = strDescription;

            if (nMemberType == 0)
            {
                if (!m_dicRegularMembers.ContainsKey(nMemberID))
                    return;

                DataCompanyMember member = m_dicRegularMembers[nMemberID];
                mgr.Tag = member;
                group.CompanyMembers.Add(mgr);
            }
            else if (nMemberType == 1)
            {
                if (!m_dicRegularTeams.ContainsKey(nMemberID))
                    return;

                DataTeam team = m_dicRegularTeams[nMemberID];
                mgr.Tag = team;
                group.RegularTeams.Add(mgr);
            }
            else if (nMemberType == 2)
            {
                if (!m_dicExternalMembers.ContainsKey(nMemberID))
                    return;

                DataExternalMember member = m_dicExternalMembers[nMemberID];
                mgr.Tag = member;
                group.ExternalCompanyMembers.Add(mgr);
            }
            else if (nMemberType == 3)
            {
                if (!m_dicExternalTeams.ContainsKey(nMemberID))
                    return;

                DataTeam team = m_dicExternalTeams[nMemberID];
                mgr.Tag = team;
                group.ExternalTeams.Add(mgr);
            }
            else if (nMemberType == 4)
            {
                DataTeam team = GetCompany(m_dicRegularTeams);
                if (team == null)
                    return;

                mgr.Tag = team;
                group.RegularTeams.Add(mgr);
            }
            else if (nMemberType == 5)
            {
                DataTeam team = GetCompany(FormMain.Instance.DataManager.ExternalTeamRootList, nMemberID);
                if (team == null)
                    return;

                mgr.Tag = team;
                group.RegularTeams.Add(mgr);
            }
            else if (nMemberType == 6)
            {
                DataTeam team = TeamDuty;

                mgr.Tag = team;
                group.RegularTeams.Add(mgr);
            }
        }

        private DataTeam GetCompany(Dictionary<int, DataTeam> dicTeams)
        {
            foreach (KeyValuePair<int, DataTeam> pair in dicTeams)
            {
                if (pair.Value.IsCompany)
                    return pair.Value;
            }

            return null;
        }

        private DataTeam GetCompany(ArrayList arrCompanies, int nCompanyID)
        {
            foreach (DataTeam team in arrCompanies)
            {
                if (team.ID == nCompanyID)
                    return team;
            }

            return null;
        }

        private FacilityManagerGroup GetOutdoorFacilityManagerGroup(int nFacilityType, Zone zone)
        {
            FacilityManagerGroup group = null;

            if (nFacilityType >= 0 && nFacilityType <= 2)
            {
                Facility.FacilityType typeFire = Facility.FacilityType.FIRE_SENSOR;

                if (m_dicOutdoorFacilityManager.ContainsKey(typeFire))
                {
                    Dictionary<Zone, FacilityManagerGroup> dicManagers = m_dicOutdoorFacilityManager[typeFire];

                    if (dicManagers.ContainsKey(zone))
                        group = dicManagers[zone];
                    else
                    {
                        group = new FacilityManagerGroup();
                        group.Zone = zone;

                        dicManagers[zone] = group;
                    }
                }
                else
                {
                    Dictionary<Zone, FacilityManagerGroup> dicManagers = new Dictionary<Zone, FacilityManagerGroup>();
                    group = new FacilityManagerGroup();
                    group.Zone = zone;
                    dicManagers[zone] = group;

                    m_dicOutdoorFacilityManager[typeFire] = dicManagers;
                    m_dicOutdoorFacilityManager[Facility.FacilityType.COOLER_SENSOR] = dicManagers;
                    m_dicOutdoorFacilityManager[Facility.FacilityType.PRESSURE_SENSOR] = dicManagers;
                }
            }
            else if (nFacilityType == 3)
            {
                Facility.FacilityType type = Facility.FacilityType.CCTV;

                if (m_dicOutdoorFacilityManager.ContainsKey(type))
                {
                    Dictionary<Zone, FacilityManagerGroup> dicManagers = m_dicOutdoorFacilityManager[type];

                    if (dicManagers.ContainsKey(zone))
                        group = dicManagers[zone];
                    else
                    {
                        group = new FacilityManagerGroup();
                        group.Zone = zone;

                        dicManagers[zone] = group;
                    }
                }
                else
                {
                    Dictionary<Zone, FacilityManagerGroup> dicManagers = new Dictionary<Zone, FacilityManagerGroup>();
                    group = new FacilityManagerGroup();
                    group.Zone = zone;
                    dicManagers[zone] = group;

                    m_dicOutdoorFacilityManager[type] = dicManagers;
                }
            }
            else if (nFacilityType >= 4 && nFacilityType <= 6)
            {
                Facility.FacilityType typeFE = Facility.FacilityType.FE;

                if (m_dicOutdoorFacilityManager.ContainsKey(typeFE))
                {
                    Dictionary<Zone, FacilityManagerGroup> dicManagers = m_dicOutdoorFacilityManager[typeFE];

                    if (dicManagers.ContainsKey(zone))
                        group = dicManagers[zone];
                    else
                    {
                        group = new FacilityManagerGroup();
                        group.Zone = zone;

                        dicManagers[zone] = group;
                    }
                }
                else
                {
                    Dictionary<Zone, FacilityManagerGroup> dicManagers = new Dictionary<Zone, FacilityManagerGroup>();
                    group = new FacilityManagerGroup();
                    group.Zone = zone;
                    dicManagers[zone] = group;

                    m_dicOutdoorFacilityManager[typeFE] = dicManagers;
                    m_dicOutdoorFacilityManager[Facility.FacilityType.HD] = dicManagers;
                    m_dicOutdoorFacilityManager[Facility.FacilityType.FA] = dicManagers;
                }
            }

            return group;
        }

		private FacilityManagerGroup GetEquipZoneFacilityManagerGroup(int nFacilityType, EquipmentZone zone)
		{
			FacilityManagerGroup group = null;

			if (nFacilityType >= 0 && nFacilityType <= 2)
			{
				Facility.FacilityType typeFire = Facility.FacilityType.FIRE_SENSOR;

				if (m_dicEquipZoneFacilityManager.ContainsKey(typeFire))
				{
					Dictionary<int, FacilityManagerGroup> dicManagers = m_dicEquipZoneFacilityManager[typeFire];

					if (dicManagers.ContainsKey(zone.ID))
						group = dicManagers[zone.ID];
					else
					{
						group = new FacilityManagerGroup();
						group.EquipZone = zone;

						dicManagers[zone.ID] = group;
					}
				}
				else
				{
					Dictionary<int, FacilityManagerGroup> dicManagers = new Dictionary<int, FacilityManagerGroup>();
					group = new FacilityManagerGroup();
					group.EquipZone = zone;
					dicManagers[zone.ID] = group;

					m_dicEquipZoneFacilityManager[typeFire] = dicManagers;
					m_dicEquipZoneFacilityManager[Facility.FacilityType.COOLER_SENSOR] = dicManagers;
					m_dicEquipZoneFacilityManager[Facility.FacilityType.PRESSURE_SENSOR] = dicManagers;
				}
			}
			else if (nFacilityType == 3)
			{
				Facility.FacilityType type = Facility.FacilityType.CCTV;

				if (m_dicEquipZoneFacilityManager.ContainsKey(type))
				{
					Dictionary<int, FacilityManagerGroup> dicManagers = m_dicEquipZoneFacilityManager[type];

					if (dicManagers.ContainsKey(zone.ID))
						group = dicManagers[zone.ID];
					else
					{
						group = new FacilityManagerGroup();
						group.EquipZone = zone;

						dicManagers[zone.ID] = group;
					}
				}
				else
				{
					Dictionary<int, FacilityManagerGroup> dicManagers = new Dictionary<int, FacilityManagerGroup>();
					group = new FacilityManagerGroup();
					group.EquipZone = zone;
					dicManagers[zone.ID] = group;

					m_dicEquipZoneFacilityManager[type] = dicManagers;
				}
			}
			else if (nFacilityType >= 4 && nFacilityType <= 6)
			{
				Facility.FacilityType typeFE = Facility.FacilityType.FE;

				if (m_dicEquipZoneFacilityManager.ContainsKey(typeFE))
				{
					Dictionary<int, FacilityManagerGroup> dicManagers = m_dicEquipZoneFacilityManager[typeFE];

					if (dicManagers.ContainsKey(zone.ID))
						group = dicManagers[zone.ID];
					else
					{
						group = new FacilityManagerGroup();
						group.EquipZone = zone;

						dicManagers[zone.ID] = group;
					}
				}
				else
				{
					Dictionary<int, FacilityManagerGroup> dicManagers = new Dictionary<int, FacilityManagerGroup>();
					group = new FacilityManagerGroup();
					group.EquipZone = zone;
					dicManagers[zone.ID] = group;

					m_dicEquipZoneFacilityManager[typeFE] = dicManagers;
					m_dicEquipZoneFacilityManager[Facility.FacilityType.HD] = dicManagers;
					m_dicEquipZoneFacilityManager[Facility.FacilityType.FA] = dicManagers;
				}
			}

			return group;
		}

        private FacilityManagerGroup GetBuildingFacilityManagerGroup(int nFacilityType, Building building)
        {
            FacilityManagerGroup group = null;

            if (nFacilityType >= 0 && nFacilityType <= 2)
            {
                Facility.FacilityType typeFire = Facility.FacilityType.FIRE_SENSOR;

                if (m_dicBuildingFacilityManager.ContainsKey(typeFire))
                {
                    Dictionary<Building, FacilityManagerGroup> dicManagers = m_dicBuildingFacilityManager[typeFire];

                    if (dicManagers.ContainsKey(building))
                        group = dicManagers[building];
                    else
                    {
                        group = new FacilityManagerGroup();
                        group.Building = building;

                        dicManagers[building] = group;
                    }
                }
                else
                {
                    Dictionary<Building, FacilityManagerGroup> dicManagers = new Dictionary<Building, FacilityManagerGroup>();
                    group = new FacilityManagerGroup();
                    group.Building = building;
                    dicManagers[building] = group;

                    m_dicBuildingFacilityManager[typeFire] = dicManagers;
                    m_dicBuildingFacilityManager[Facility.FacilityType.COOLER_SENSOR] = dicManagers;
                    m_dicBuildingFacilityManager[Facility.FacilityType.PRESSURE_SENSOR] = dicManagers;
                }
            }
            else if (nFacilityType == 3)
            {
                Facility.FacilityType type = Facility.FacilityType.CCTV;

                if (m_dicBuildingFacilityManager.ContainsKey(type))
                {
                    Dictionary<Building, FacilityManagerGroup> dicManagers = m_dicBuildingFacilityManager[type];

                    if (dicManagers.ContainsKey(building))
                        group = dicManagers[building];
                    else
                    {
                        group = new FacilityManagerGroup();
                        group.Building = building;

                        dicManagers[building] = group;
                    }
                }
                else
                {
                    Dictionary<Building, FacilityManagerGroup> dicManagers = new Dictionary<Building, FacilityManagerGroup>();
                    group = new FacilityManagerGroup();
                    group.Building = building;
                    dicManagers[building] = group;

                    m_dicBuildingFacilityManager[type] = dicManagers;
                }
            }
            else if (nFacilityType >= 4 && nFacilityType <= 6)
            {
                Facility.FacilityType typeFE = Facility.FacilityType.FE;

                if (m_dicBuildingFacilityManager.ContainsKey(typeFE))
                {
                    Dictionary<Building, FacilityManagerGroup> dicManagers = m_dicBuildingFacilityManager[typeFE];

                    if (dicManagers.ContainsKey(building))
                        group = dicManagers[building];
                    else
                    {
                        group = new FacilityManagerGroup();
                        group.Building = building;

                        dicManagers[building] = group;
                    }
                }
                else
                {
                    Dictionary<Building, FacilityManagerGroup> dicManagers = new Dictionary<Building, FacilityManagerGroup>();
                    group = new FacilityManagerGroup();
                    group.Building = building;
                    dicManagers[building] = group;

                    m_dicBuildingFacilityManager[typeFE] = dicManagers;
                    m_dicBuildingFacilityManager[Facility.FacilityType.HD] = dicManagers;
                    m_dicBuildingFacilityManager[Facility.FacilityType.FA] = dicManagers;
                }
            }

            return group;
        }

        private FacilityManagerGroup GetFacilityManagerGroup(int nFacilityType)
        {
            FacilityManagerGroup group = null;

            if (nFacilityType >= 0 && nFacilityType <= 2)
            {
                Facility.FacilityType typeFire = Facility.FacilityType.FIRE_SENSOR;

                if (m_dicEntireFacilityManagers.ContainsKey(typeFire))
                    group = m_dicEntireFacilityManagers[typeFire];
                else
                {
                    group = new FacilityManagerGroup();
                    group.Type = typeFire;

                    m_dicEntireFacilityManagers[typeFire] = group;
                    m_dicEntireFacilityManagers[Facility.FacilityType.COOLER_SENSOR] = group;
                    m_dicEntireFacilityManagers[Facility.FacilityType.PRESSURE_SENSOR] = group;
                }
            }
            else if (nFacilityType == 3)
            {
                Facility.FacilityType type = Facility.FacilityType.CCTV;

                if (m_dicEntireFacilityManagers.ContainsKey(type))
                    group = m_dicEntireFacilityManagers[type];
                else
                {
                    group = new FacilityManagerGroup();
                    group.Type = type;
                    m_dicEntireFacilityManagers[type] = group;
                }
            }
            else if (nFacilityType >= 4 && nFacilityType <= 6)
            {
                Facility.FacilityType typeFE = Facility.FacilityType.FE;

                if (m_dicEntireFacilityManagers.ContainsKey(typeFE))
                    group = m_dicEntireFacilityManagers[typeFE];
                else
                {
                    group = new FacilityManagerGroup();
                    group.Type = typeFE;

                    m_dicEntireFacilityManagers[typeFE] = group;
                    m_dicEntireFacilityManagers[Facility.FacilityType.HD] = group;
                    m_dicEntireFacilityManagers[Facility.FacilityType.FA] = group;
                }
            }

            return group;
        }

        public ArrayList GetFireEquipments(Zone zone)
        {
            if (m_dicZoneFireEquipments.ContainsKey(zone))
                return m_dicZoneFireEquipments[zone];

            return null;
        }

        // 시설물 타입별 발전소 전체 담당자 얻어오기
        public FacilityManagerGroup GetEntireFacilityManagerGroup(Facility.FacilityType type, bool alwaysGet = false)
        {
            if (m_dicEntireFacilityManagers.ContainsKey(type))
                return m_dicEntireFacilityManagers[type];

            if (alwaysGet)
            {
                FacilityManagerGroup group = new FacilityManagerGroup();
                group.Type = type;

                if (type == Facility.FacilityType.FIRE_SENSOR ||
                    type == Facility.FacilityType.COOLER_SENSOR ||
                    type == Facility.FacilityType.PRESSURE_SENSOR)
                {
                    m_dicEntireFacilityManagers[Facility.FacilityType.FIRE_SENSOR] = group;
                    m_dicEntireFacilityManagers[Facility.FacilityType.COOLER_SENSOR] = group;
                    m_dicEntireFacilityManagers[Facility.FacilityType.PRESSURE_SENSOR] = group;
                }
                else if (type == Facility.FacilityType.FE ||
                    type == Facility.FacilityType.HD ||
                    type == Facility.FacilityType.FA)
                {
                    m_dicEntireFacilityManagers[Facility.FacilityType.FE] = group;
                    m_dicEntireFacilityManagers[Facility.FacilityType.HD] = group;
                    m_dicEntireFacilityManagers[Facility.FacilityType.FA] = group;
                }
                else
                    m_dicEntireFacilityManagers[type] = group;

                return group;
            }

            return null;
        }

		// EquipZone별 시설물 담당자 얻어오기
		public FacilityManagerGroup GetEquipZoneFacilityManagerGroup(Facility.FacilityType type, EquipmentZone zone, bool alwaysGet = false)
		{
			if (zone == null)
				return null;

			if (m_dicEquipZoneFacilityManager.ContainsKey(type))
			{
				Dictionary<int, FacilityManagerGroup> dicManagers = m_dicEquipZoneFacilityManager[type];
				
				if (dicManagers.ContainsKey(zone.ID))
					return dicManagers[zone.ID];

				if (alwaysGet)
				{
					FacilityManagerGroup group = new FacilityManagerGroup();
					group.Type = type;
					group.EquipZone = zone;

					dicManagers[zone.ID] = group;
					return group;
				}
			}

			if (alwaysGet)
			{
				Dictionary<int, FacilityManagerGroup> dicManagers = new Dictionary<int, FacilityManagerGroup>();

				if (type == Facility.FacilityType.FIRE_SENSOR ||
					type == Facility.FacilityType.COOLER_SENSOR ||
					type == Facility.FacilityType.PRESSURE_SENSOR)
				{
					m_dicEquipZoneFacilityManager[Facility.FacilityType.FIRE_SENSOR] = dicManagers;
					m_dicEquipZoneFacilityManager[Facility.FacilityType.COOLER_SENSOR] = dicManagers;
					m_dicEquipZoneFacilityManager[Facility.FacilityType.PRESSURE_SENSOR] = dicManagers;
				}
				else if (type == Facility.FacilityType.FE ||
					type == Facility.FacilityType.HD ||
					type == Facility.FacilityType.FA)
				{
					m_dicEquipZoneFacilityManager[Facility.FacilityType.FE] = dicManagers;
					m_dicEquipZoneFacilityManager[Facility.FacilityType.HD] = dicManagers;
					m_dicEquipZoneFacilityManager[Facility.FacilityType.FA] = dicManagers;
				}
				else
					m_dicEquipZoneFacilityManager[type] = dicManagers;

				FacilityManagerGroup group = new FacilityManagerGroup();
				group.Type = type;
				group.EquipZone = zone;

				dicManagers[zone.ID] = group;
				return group;
			}

			return null;
		}

        // 건물별 시설물 담당자 얻어오기
        public FacilityManagerGroup GetBuildingFacilityManagerGroup(Facility.FacilityType type, Building building, bool alwaysGet = false)
        {
            if (building == null)
                return null;

            if (m_dicBuildingFacilityManager.ContainsKey(type))
            {
                Dictionary<Building, FacilityManagerGroup> dicManagers = m_dicBuildingFacilityManager[type];


                if (dicManagers.ContainsKey(building))
                    return dicManagers[building];

                if (alwaysGet)
                {
                    FacilityManagerGroup group = new FacilityManagerGroup();
                    group.Type = type;
                    group.Building = building;

                    dicManagers[building] = group;
                    return group;
                }
            }

            if (alwaysGet)
            {
                Dictionary<Building, FacilityManagerGroup> dicManagers = new Dictionary<Building, FacilityManagerGroup>();
                
                if (type == Facility.FacilityType.FIRE_SENSOR ||
                    type == Facility.FacilityType.COOLER_SENSOR ||
                    type == Facility.FacilityType.PRESSURE_SENSOR)
                {
                    m_dicBuildingFacilityManager[Facility.FacilityType.FIRE_SENSOR] = dicManagers;
                    m_dicBuildingFacilityManager[Facility.FacilityType.COOLER_SENSOR] = dicManagers;
                    m_dicBuildingFacilityManager[Facility.FacilityType.PRESSURE_SENSOR] = dicManagers;
                }
                else if (type == Facility.FacilityType.FE ||
                    type == Facility.FacilityType.HD ||
                    type == Facility.FacilityType.FA)
                {
                    m_dicBuildingFacilityManager[Facility.FacilityType.FE] = dicManagers;
                    m_dicBuildingFacilityManager[Facility.FacilityType.HD] = dicManagers;
                    m_dicBuildingFacilityManager[Facility.FacilityType.FA] = dicManagers;
                }
                else
                    m_dicBuildingFacilityManager[type] = dicManagers;

                FacilityManagerGroup group = new FacilityManagerGroup();
                group.Type = type;
                group.Building = building;

                dicManagers[building] = group;
                return group;
            }

            return null;
        }

        // 외부 영역별 시설물 담당자 얻어오기
        public FacilityManagerGroup GetOutdoorFacilityManagerGroup(Facility.FacilityType type, Zone zone, bool alwaysGet = false)
        {
            if (m_dicOutdoorFacilityManager.ContainsKey(type))
            {
                Dictionary<Zone, FacilityManagerGroup> dicManagers = m_dicOutdoorFacilityManager[type];

                if (dicManagers.ContainsKey(zone))
                    return dicManagers[zone];

                if (alwaysGet)
                {
                    FacilityManagerGroup group = new FacilityManagerGroup();
                    group.Type = type;
                    group.Zone = zone;

                    dicManagers[zone] = group;
                    return group;
                }
            }

            if (alwaysGet)
            {
                Dictionary<Zone, FacilityManagerGroup> dicManagers = new Dictionary<Zone, FacilityManagerGroup>();
                
                if (type == Facility.FacilityType.FIRE_SENSOR ||
                    type == Facility.FacilityType.COOLER_SENSOR ||
                    type == Facility.FacilityType.PRESSURE_SENSOR)
                {
                    m_dicOutdoorFacilityManager[Facility.FacilityType.FIRE_SENSOR] = dicManagers;
                    m_dicOutdoorFacilityManager[Facility.FacilityType.COOLER_SENSOR] = dicManagers;
                    m_dicOutdoorFacilityManager[Facility.FacilityType.PRESSURE_SENSOR] = dicManagers;
                }
                else if (type == Facility.FacilityType.FE ||
                    type == Facility.FacilityType.HD ||
                    type == Facility.FacilityType.FA)
                {
                    m_dicOutdoorFacilityManager[Facility.FacilityType.FE] = dicManagers;
                    m_dicOutdoorFacilityManager[Facility.FacilityType.HD] = dicManagers;
                    m_dicOutdoorFacilityManager[Facility.FacilityType.FA] = dicManagers;
                }
                else
                    m_dicOutdoorFacilityManager[type] = dicManagers;

                FacilityManagerGroup group = new FacilityManagerGroup();
                group.Type = type;
                group.Zone = zone;

                dicManagers[zone] = group;
                return group;
            }

            return null;
        }

        public DataTeam RegularTeamRoot
        {
            get { return m_teamRegularRoot; }
        }

        public ArrayList ExternalTeamRootList
        {
            get { return m_listExternalRootTeams; }
        }

        // 정규조직 혹은 외부협력업체 팀원들 리스트를 리턴
        public ArrayList GetTeamMembers(DataTeam team)
        {
            if (team.External)
            {
                if (m_dicExternalTeamMembers.ContainsKey(team))
                    return m_dicExternalTeamMembers[team];
            }

            if (m_dicRegularTeamMembers.ContainsKey(team))
                return m_dicRegularTeamMembers[team];

            return null;
        }

        // 첫번째 담당자의 이름과 전화번호를 알려준다.
        public string GetFacilityManagerName(FacilityManagerGroup group, ref string strPhoneNumber)
        {
            if (group == null)
                return "";

            if (group.RegularTeams.Count > 0)
            {
                FacilityManager mgr = (FacilityManager)group.RegularTeams[0];
                return GetFacilityManagerName(mgr, ref strPhoneNumber);
            }

            if (group.CompanyMembers.Count > 0)
            {
                FacilityManager mgr = (FacilityManager)group.CompanyMembers[0];
                return GetFacilityManagerName(mgr, ref strPhoneNumber);
            }

            if (group.ExternalTeams.Count > 0)
            {
                FacilityManager mgr = (FacilityManager)group.ExternalTeams[0];
                return GetFacilityManagerName(mgr, ref strPhoneNumber);
            }

            if (group.ExternalCompanyMembers.Count > 0)
            {
                FacilityManager mgr = (FacilityManager)group.ExternalCompanyMembers[0];
                return GetFacilityManagerName(mgr, ref strPhoneNumber);
            }

            return "";
        }

        // 첫번째 담당자의 이름과 전화번호를 알려준다.
        public string GetFacilityManagerName(FacilityManager mgr, ref string strPhoneNumber)
        {
            if (mgr.MemberType == 0)
            {
                if (m_dicRegularMembers.ContainsKey(mgr.MemberID))
                {
                    DataCompanyMember member = m_dicRegularMembers[mgr.MemberID];
                    strPhoneNumber = member.OfficePhoneNumber;
                    return member.MemberName;
                }
            }
            else if (mgr.MemberType == 1)
            {
                if (m_dicRegularTeams.ContainsKey(mgr.MemberID))
                {
                    DataTeam team = m_dicRegularTeams[mgr.MemberID];

                    if (m_dicRegularTeamMembers.ContainsKey(team))
                    {
                        ArrayList arrCompanyMembers = m_dicRegularTeamMembers[team];

                        foreach (DataCompanyMember member in arrCompanyMembers)
                        {
                            if (mgr.LevelLimit <= 0)
                            {
                                strPhoneNumber = member.OfficePhoneNumber;
                                break;
                            }
                            else if ((mgr.UpperLimit > 0 && member.LevelID <= mgr.LevelLimit) ||
                                (mgr.UpperLimit < 0 && member.LevelID >= mgr.LevelLimit) ||
                                (mgr.UpperLimit == 0 && member.LevelID == mgr.LevelLimit))
                            {
                                strPhoneNumber = member.OfficePhoneNumber;
                                break;
                            }
                            /*else if (mgr.UpperLimit && member.LevelID <= mgr.LevelLimit)
                            {
                                strPhoneNumber = member.OfficePhoneNumber;
                                break;
                            }
                            else if (!mgr.UpperLimit && member.LevelID >= mgr.LevelLimit)
                            {
                                strPhoneNumber = member.OfficePhoneNumber;
                                break;
                            }*/
                        }
                    }

                    return team.TeamName;
                }
            }
            else if (mgr.MemberType == 2)
            {
                if (m_dicExternalMembers.ContainsKey(mgr.MemberID))
                {
                    DataExternalMember member = m_dicExternalMembers[mgr.MemberID];
                    strPhoneNumber = member.PhoneNumber;
                    return member.Team.CompanyName + " " + member.Team.TeamName + " " + member.Name;
                }
            }
            else if (mgr.MemberType == 3)
            {
                if (m_dicExternalTeams.ContainsKey(mgr.MemberID))
                {
                    DataTeam team = m_dicExternalTeams[mgr.MemberID];

                    if (m_dicExternalTeamMembers.ContainsKey(team))
                    {
                        ArrayList arrExternalMembers = m_dicExternalTeamMembers[team];

                        foreach (DataExternalMember member in arrExternalMembers)
                        {
                            strPhoneNumber = member.PhoneNumber;
                            break;
                        }
                    }

                    return team.CompanyName + " " + team.TeamName;
                }
            }

            return "";
        }

        public string GetEditPassword()
        {
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData("Select Password from SDMSEditPassword", 0);

            if (arrResult != null && arrResult.Count > 0)
            {
                string strPassword = DBUtility.AES256Cipher.AES_decrypt(arrResult[0].ToString(), key);
                return strPassword;
            }

            return null;
        }        
    }
}
