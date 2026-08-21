using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SDMSServer
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

		// EquipZone 별 시설물 담당자
		private Dictionary<Facility.FacilityType, Dictionary<int, FacilityManagerGroup>> m_dicEquipZoneFacilityManager = new Dictionary<Facility.FacilityType, Dictionary<int, FacilityManagerGroup>>();
        private DataTeamDuty m_teamDuty = new DataTeamDuty();

        private static string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

        public Dictionary<Zone, ArrayList> ZoneFireEquipments
        {
            get { return m_dicZoneFireEquipments; }
        }


		private static DataManager m_Instance = null;
		public static DataManager Instance
		{
			get 
			{
				if (m_Instance == null)
				{
					m_Instance = new DataManager(NetworkServer.Instance.DBManager);
				}
				return m_Instance; 
			}
		}

        public DataTeamDuty TeamDuty
        {
            get { return m_teamDuty; }
        }


        private int m_nSiteID = 1;

        private DataManager(DBUtility.WebDBManager dbMgr)
        {
            m_nSiteID = NetworkServer.Instance.SiteID;

            m_teamRegularRoot = LoadRegularTeam(dbMgr, m_dicRegularTeams);
            m_listExternalRootTeams = LoadExternalTeam(dbMgr, m_dicExternalTeams);

            LoadCompanyMember(dbMgr, m_dicRegularTeams);
            LoadExternalMember(dbMgr, m_dicExternalTeams);
        }

        public bool LoadCompanyMember(DBUtility.WebDBManager dbMgr, Dictionary<int, DataTeam> dicTeams)
        {
            // site ID구분이 없더라도 dicTeams에 RegularTeamID 가 없는 경우 저장되지 않는다.
            // 전체 인원이 많아지는 경우 ReqularTeam에 SiteID를 참조하는 방향을 고려해볼것. skkim 2015.01.14
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

                if (nLevelID < 0)
                {
                    // nLevelID가 0보다 작은 직원은 삭제된 직원이다.
                    continue;
                }

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
            //string szSQL = "SELECT ID, Name, PhoneNumber, IsTeamLeader, TeamID FROM ExternalCompanyMember";
            string szText = "SELECT ecm.ID, Name, ecm.PhoneNumber, ecm.IsTeamLeader, ecm.TeamID FROM ExternalCompanyMember as ecm, ExternalTeam as et " +
                            " WHERE ecm.TeamID = et.ID AND et.SiteID = {0}";

            string szSQL = string.Format(szText, m_nSiteID);

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

            // 공백문자나 '-' 등의 기호를 제거한다.
            for (int i = 0; i < nLen; i++)
            {
                char ch = strPhoneNumber[i];

                if (ch != ' ' && ch != '\t' && ch != '-')
                    strResult += ch;
            }

            int nLen2 = strResult.Length;

            // 숫자 이외의 기호가 들어있으면 잘못된 전화번호다.
            for (int i = 0; i < nLen2; i++)
            {
                char ch = strResult[i];

                if (ch < '0' || ch > '9')
                    return "";
            }

            return strResult;
        }

        // dicTeams : ID별 Team
        private ArrayList LoadExternalTeam(DBUtility.WebDBManager dbMgr, Dictionary<int, DataTeam> dicTeams)
        {
           // string strSQL = "Select ID, TeamName from ExternalTeam";
            string szText = "SELECT ID, TeamName FROM ExternalTeam WHERE SiteID = {0}";
            string strSQL = string.Format(szText, m_nSiteID);

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

            //string szSQL = "SELECT ID, TeamName, ParentTeamID, CompanyID FROM ExternalCompanyTeam";
            string szText2 = "SELECT ect.ID, ect.TeamName, ect.ParentTeamID, ect.CompanyID " +
                             " FROM ExternalCompanyTeam as ect, ExternalTeam as et WHERE ect.CompanyID = et.ID AND et.SiteID = {0}";

            string szSQL = string.Format(szText2, m_nSiteID);

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
            //string szSQL = "SELECT R.ID, R.TeamName, R.ParentTeamID FROM RegularTeam as R";
            //ArrayList arrResult = dbMgr.GetResultData(szSQL, 0);
           
            // Site별로 사용할 수 있도록 수정 , Edit by skkim 2015.01.14
            // SiteID로 본부 아이디를 가져온다.
            string szSQL = string.Format("SELECT TeamID FROM Site WHERE ID = {0}", m_nSiteID);
            ArrayList arrResult1 = dbMgr.GetResultData(szSQL, 0);
            if (arrResult1 == null || arrResult1.Count == 0)
                return null;

            int nTeamID = DBUtility.WebDBManager.GetIntField(arrResult1[0].ToString(), -1);
            if (nTeamID == -1)
                return null;
            
            string strSQL = string.Format("sp_TeamList2 {0}", nTeamID);
            ArrayList arrResult = dbMgr.GetStoredProcedureData(strSQL, 0);
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
		    
        public void LoadFireEquipment()
        {
			DBUtility.WebDBManager dbMgr = NetworkServer.Instance.DBManager;

            //string strSQL = "Select ID, RFIDTag, EquipID, RFIDTagID, DxfObjID, EquipType, ZoneID, X, Y, Z, Description from FireEquipment";

            string szText = "SELECT fe.ID, fe.RFIDTag, fe.EquipID, fe.RFIDTagID, fe.DxfObjID, fe.EquipType,fe.EquipSubType, " +
                            " fe.ZoneID, fe.X, fe.Y, fe.Z, fe.CreateDate, fe.Duration, fe.Description " +
                            " FROM FireEquipment as fe, Zone as zo WHERE fe.ZoneID = zo.ID and zo.SiteID = {0}";
            
            string strSQL = string.Format(szText, m_nSiteID);

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
                    type = Facility.FacilityType.FA;
                else
                    continue;

                FireEquipment equip = new FireEquipment();
                equip.ID = nID;
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
								
                ArrayList arrEquipments = null;

                if (m_dicZoneFireEquipments.ContainsKey(equip.Zone))
                    arrEquipments = m_dicZoneFireEquipments[equip.Zone];
                else
                {
                    arrEquipments = new ArrayList();
                    m_dicZoneFireEquipments[equip.Zone] = arrEquipments;
                }

                arrEquipments.Add(equip);
                //dicEquip[nID] = equip;
            }
        }

        private Dictionary<int, FireEquipmentHistory> LoadFireEquipmentHistory()
        {
            //string strSQL = "select ID, FireEquipmentID, Time, status, CheckersOpinion from FireEquipmentHistory order by FireEquipmentID";
            string szText = "SELECT feh.ID, feh.FireEquipmentID,feh.SOPGenUserID, feh.Time,feh.Status,feh.CheckersOpinion,feh.Description " +
                            " FROM FireEquipmentHistory as feh, FireEquipment as fe, Zone as z WHERE fe.ID = feh.FireEquipmentID AND fe.ZoneID = z.ID AND z.SiteID = {0}";
            
            string strSQL = string.Format(szText, m_nSiteID);

			ArrayList arrResult = NetworkServer.Instance.DBManager.GetResultData(strSQL, 0);
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

			DBUtility.WebDBManager dbMgr = NetworkServer.Instance.DBManager;

           // string strSQL = "select id, MemberID, MemberType, FacilityType, LevelLimit, Description, UpperLimit from FacilityManager order by FacilityType";
            string szText = "SELECT id, MemberID, MemberType, FacilityType, LevelLimit, Description, UpperLimit FROM FacilityManager WHERE SiteID = {0} order by FacilityType";
            string strSQL = string.Format(szText, m_nSiteID);

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
                string strDescription = DBUtility.WebDBManager.GetStringField(arrResult[i + 5], "");
                int nUppderLimit = DBUtility.WebDBManager.GetIntField(arrResult[i + 6].ToString(), 0);

                if (nID < 0 || nMemberID < 0)
                    continue;

                FacilityManagerGroup group = GetFacilityManagerGroup(nFacilityType);
                if (group == null)
                    continue;

                AddFacilityManager(nID, nMemberID, nMemberType, nFacilityType, nLevelLimit, nUppderLimit, strDescription, group);
            }

            //strSQL = "select id, MemberID, MemberType, FacilityType, LevelLimit, BuildingID, Description, UpperLimit from BuildingFacilityManager order by FacilityType";
            szText = "SELECT id, MemberID, MemberType, FacilityType, LevelLimit, BuildingID, Description, UpperLimit " + 
                     " FROM BuildingFacilityManager WHERE SiteID = {0} order by FacilityType";

            strSQL = string.Format(szText, m_nSiteID);

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
                string strDescription = DBUtility.WebDBManager.GetStringField(arrResult[i + 6], "");
                int nUpperLimit = DBUtility.WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0);

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

                AddFacilityManager(nID, nMemberID, nMemberType, nFacilityType, nLevelLimit, nUpperLimit, strDescription, group);
            }

			// Add EquipZone Facility Manager
            
			//strSQL = "select id, MemberID, MemberType, FacilityType, LevelLimit, EquipZoneID, UpperLimit, Description from EquipZoneFacilityManager order by FacilityType";
            szText = "select id, MemberID, MemberType, FacilityType, LevelLimit, EquipZoneID, UpperLimit, Description "+
                      " from EquipZoneFacilityManager WHERE SiteID = {0} order by FacilityType";
            strSQL = string.Format(szText, m_nSiteID);

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

		public ArrayList GetAllCompanyMember()
		{
			ArrayList arrPhoneNumber = new ArrayList();
			foreach (KeyValuePair<int, DataCompanyMember> pair in m_dicRegularMembers)
			{
				arrPhoneNumber.Add(pair.Value);
			}
			return arrPhoneNumber;
		}

        public DataCompanyMember GetRegularMember(int nID)
        {
            if (!m_dicRegularMembers.ContainsKey(nID))
                return null;

            return m_dicRegularMembers[nID];
        }

        private void AddFacilityManager(int nID, int nMemberID, int nMemberType, int nFacilityType, int nLevelLimit, int nUpperLimit, string strDescription, FacilityManagerGroup group)
        {
            FacilityManager mgr = new FacilityManager();
            mgr.ID = nID;
            mgr.MemberID = nMemberID;
            mgr.MemberType = nMemberType;
            mgr.Type = Facility.ToFacilityType(nFacilityType);
            mgr.LevelLimit = nLevelLimit;
            mgr.UpperLimit = nUpperLimit;
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
                DataTeam team = GetCompany(ExternalTeamRootList, nMemberID);
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

        // 건물별 시설물 담당자 얻어오기
        public FacilityManagerGroup GetBuildingFacilityManagerGroup(Facility.FacilityType type, Building building, bool alwaysGet = false)
        {
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

        public DataCompanyMember GetReqularTeamMembers(int nTeamID, int nID)
        {
            foreach (KeyValuePair<DataTeam, ArrayList> pair in m_dicRegularTeamMembers)
            {
                if (pair.Key.ID == nTeamID)
                {
                    foreach (DataCompanyMember member in pair.Value)
                    {
                        if (member.ID == nID)
                        {
                            return member;
                        }
                    }                    
                }
            }
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
                    return team.CompanyName + " " + team.TeamName;
                }
            }

            return "";
        }

        // arrMemberIDs에 속해있는 EquipZoneFacilityManager들을 삭제한다.
        // Return 값 : 삭제된 데이터가 존재하는가?
        public bool RemoveEquipZoneFacilityManagers(ArrayList arrMemberIDs, Facility.FacilityType type, int nMemberType)
        {
            bool isDeleted = false;

            foreach (KeyValuePair<Facility.FacilityType, Dictionary<int, FacilityManagerGroup>> pair in m_dicEquipZoneFacilityManager)
            {
                if (pair.Key != type)
                    continue;

                foreach (KeyValuePair<int, FacilityManagerGroup> _pair in pair.Value)
                {
                    FacilityManagerGroup group = _pair.Value;
                    ArrayList arrMembers = null;

                    if (nMemberType == 0)
                        arrMembers = group.CompanyMembers;
                    else if (nMemberType == 1)
                        arrMembers = group.RegularTeams;
                    else
                        continue;

                    int nMgrCount = arrMembers.Count;

                    for (int i=nMgrCount-1;i>=0;i--)
                    {
                        FacilityManager mgr = (FacilityManager)arrMembers[i];

                        if (arrMemberIDs.Contains(mgr.MemberID))
                        {
                            arrMembers.RemoveAt(i);
                            isDeleted = true;
                        }
                    }
                }
            }

            return isDeleted;
        }

        // arrMemberIDs에 속해있는 BuildingFacilityManager들을 삭제한다.
        // Return 값 : 삭제된 데이터가 존재하는가?
        public bool RemoveBuildingFacilityManagers(ArrayList arrMemberIDs, Facility.FacilityType type, int nMemberType)
        {
            bool isDeleted = false;

            foreach (KeyValuePair<Facility.FacilityType, Dictionary<Building, FacilityManagerGroup>> pair in m_dicBuildingFacilityManager)
            {
                if (pair.Key != type)
                    continue;

                foreach (KeyValuePair<Building, FacilityManagerGroup> _pair in pair.Value)
                {
                    FacilityManagerGroup group = _pair.Value;
                    ArrayList arrMembers = null;

                    if (nMemberType == 0)
                        arrMembers = group.CompanyMembers;
                    else if (nMemberType == 1)
                        arrMembers = group.RegularTeams;
                    else
                        continue;

                    int nMgrCount = arrMembers.Count;

                    for (int i = nMgrCount - 1; i >= 0; i--)
                    {
                        FacilityManager mgr = (FacilityManager)arrMembers[i];

                        if (arrMemberIDs.Contains(mgr.MemberID))
                        {
                            arrMembers.RemoveAt(i);
                            isDeleted = true;
                        }
                    }
                }
            }

            return isDeleted;
        }

        // arrMemberIDs에 속해있는 FacilityManager들을 삭제한다.
        // Return 값 : 삭제된 데이터가 존재하는가?
        public bool RemoveEntireFacilityManagers(ArrayList arrMemberIDs, Facility.FacilityType type, int nMemberType)
        {
            bool isDeleted = false;

            foreach (KeyValuePair<Facility.FacilityType, FacilityManagerGroup> pair in this.m_dicEntireFacilityManagers)
            {
                if (pair.Key != type)
                    continue;

                FacilityManagerGroup group = pair.Value;
                ArrayList arrMembers = null;

                if (nMemberType == 0)
                    arrMembers = group.CompanyMembers;
                else if (nMemberType == 1)
                    arrMembers = group.RegularTeams;
                else
                    continue;

                int nMgrCount = arrMembers.Count;

                for (int i = nMgrCount - 1; i >= 0; i--)
                {
                    FacilityManager mgr = (FacilityManager)arrMembers[i];

                    if (arrMembers.Contains(mgr.MemberID))
                    {
                        arrMembers.RemoveAt(i);
                        isDeleted = true;
                    }
                }
            }

            return isDeleted;
        }

		public static bool GetTranningMode()
		{
            string szSQL = "SELECT PropertyValue FROM OptionSDMS where PropertyName ='TranningMode' and SiteID = " + NetworkServer.Instance.SiteID;
			ArrayList arResult = NetworkServer.Instance.DBManager.GetResultData(szSQL, 0);
			if (arResult == null || arResult.Count == 0)
			{
				return false;
			}
			else
			{
				int value = DBUtility.WebDBManager.GetIntField(arResult[0].ToString(), 0);
				if( value == 1)
					return true;
			}
			return false;	
		}	
    }
}
