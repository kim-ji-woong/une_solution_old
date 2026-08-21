using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using DBUtility2;
using UnE.Spatial;
using UnE.Sensor;
using SDMS;
using SDMS_Building.Content;

namespace SDMS_Building.Data
{
    public class DataManager
    {
        private class FireEquipmentHistory
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

        private int m_nTemp = 1; // 중복값을 막기위해 +한다

        private DataTeam m_teamRegularRoot = null;
        private ArrayList m_listExternalRootTeams = new ArrayList();
        private Dictionary<int, DataTeam> m_dicRegularTeams = new Dictionary<int, DataTeam>();
        private Dictionary<DataTeam, ArrayList> m_dicRegularTeamMembers = new Dictionary<DataTeam, ArrayList>();
        private Dictionary<int, DataCompanyMember> m_dicRegularMembers = new Dictionary<int, DataCompanyMember>();
        private Dictionary<int, DataTeam> m_dicExternalTeams = new Dictionary<int, DataTeam>();
        private Dictionary<DataTeam, ArrayList> m_dicExternalTeamMembers = new Dictionary<DataTeam, ArrayList>();
        private Dictionary<int, DataExternalMember> m_dicExternalMembers = new Dictionary<int, DataExternalMember>();
        private Dictionary<Zone, ArrayList> m_dicZoneFireEquipments = new Dictionary<Zone, ArrayList>();
        private Dictionary<int, DataTeamControlRoom> m_dicControlRoomTeams = new Dictionary<int, DataTeamControlRoom>();

        // 시설물 타입별 발전소 전체 담당자(탐지시)
        private Dictionary<IFacility.FacilityType, FacilityManagerGroup> m_dicEntireFacilityManagers = new Dictionary<IFacility.FacilityType, FacilityManagerGroup>();
        // 시설물 타입별 발전소 전체 담당자(전파시)
        private Dictionary<IFacility.FacilityType, FacilityManagerGroup> m_dicEntireFacilityManagersReport = new Dictionary<IFacility.FacilityType, FacilityManagerGroup>();

        // 건물별 시설물 담당자(탐지시)
        private Dictionary<IFacility.FacilityType, Dictionary<Building, FacilityManagerGroup>> m_dicBuildingFacilityManager = new Dictionary<IFacility.FacilityType, Dictionary<Building, FacilityManagerGroup>>();
        // 건물별 시설물 담당자(전파시)
        private Dictionary<IFacility.FacilityType, Dictionary<Building, FacilityManagerGroup>> m_dicBuildingFacilityManagerReport = new Dictionary<IFacility.FacilityType, Dictionary<Building, FacilityManagerGroup>>();

        // 외부 Zone별 시설물 담당자(탐지시)
        private Dictionary<IFacility.FacilityType, Dictionary<Zone, FacilityManagerGroup>> m_dicOutdoorFacilityManager = new Dictionary<IFacility.FacilityType, Dictionary<Zone, FacilityManagerGroup>>();
        // 외부 Zone별 시설물 담당자(전파시)
        private Dictionary<IFacility.FacilityType, Dictionary<Zone, FacilityManagerGroup>> m_dicOutdoorFacilityManagerReport = new Dictionary<IFacility.FacilityType, Dictionary<Zone, FacilityManagerGroup>>();

        // EquipZone 별 시설물 담당자(탐지시)
        private Dictionary<IFacility.FacilityType, Dictionary<int, FacilityManagerGroup>> m_dicEquipZoneFacilityManager = new Dictionary<IFacility.FacilityType, Dictionary<int, FacilityManagerGroup>>();
        // EquipZone 별 시설물 담당자(전파시)
        private Dictionary<IFacility.FacilityType, Dictionary<int, FacilityManagerGroup>> m_dicEquipZoneFacilityManagerReport = new Dictionary<IFacility.FacilityType, Dictionary<int, FacilityManagerGroup>>();

        // 방범센서 유형별 시설물 담당자(탐지시)
        // Key : SecurityTypeTable의 ID
        private Dictionary<int, FacilityManagerGroup> m_dicSecurityFacilityManagers = new Dictionary<int, FacilityManagerGroup>();
        // 방범센서 유형별 시설물 담당자(전파시)
        // Key : SecurityTypeTable의 ID
        private Dictionary<int, FacilityManagerGroup> m_dicSecurityFacilityManagersReport = new Dictionary<int, FacilityManagerGroup>();

        // 교대 근무자에 병합되었음
        // [2017/06/05] 김지웅
        // 당직자용 데이터
        //private DataTeamDuty m_teamDuty = new DataTeamDuty();
        
        // 방재장비 데이터
        private Dictionary<int, DisasterPreventionEquipmentType> m_dicDisasterPreventionEquipmentType = new Dictionary<int, DisasterPreventionEquipmentType>();
        private Dictionary<int, DisasterPreventionEquipmentLocation> m_dicDisasterPreventionEquipmentLocation = new Dictionary<int, DisasterPreventionEquipmentLocation>();
        private Dictionary<int, DisasterPreventionEquipment> m_dicDisasterPreventionEquipment = new Dictionary<int, DisasterPreventionEquipment>();
        //

        // 재난전파시 담당자를 따로 지정하여 사용하는가?
        // 이 값이 false이면 재난 전파시 전직원에게 문자메시지를 발송한다.
        // [2017-06-09] 김지웅
        private bool m_useReportFacilityManagers = false;

        private enum FacilityType { TYPE = 0, BUILDING, FLOOR };
        private int m_nFacility = 1;
        private int m_nBuilding = 1;
        private int m_nEquipZone = 1;

        public bool UseReportFacilityManagers
        {
            get { return m_useReportFacilityManagers; }
        }

        private static string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

        public DataCompanyMember GetCompanyMember(int nMemberID)
        {
            if (m_dicRegularMembers.ContainsKey(nMemberID))
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

        private int m_nSiteID = 1;
        public DataManager(WebDBManager dbMgr)
        {
            m_nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;

            m_teamRegularRoot = LoadRegularTeam(dbMgr, m_dicRegularTeams);
            m_listExternalRootTeams = LoadExternalTeam(dbMgr, m_dicExternalTeams);

            LoadCompanyMember(dbMgr, m_dicRegularTeams);
            LoadExternalMember(dbMgr, m_dicExternalTeams);
            LoadControlRoomTeams(dbMgr, m_dicControlRoomTeams);
        }

        public void ReloadCompanyMember()
        {
            WebDBManager dbMgr = FormMain.Instance.DBManager;
            m_teamRegularRoot = LoadRegularTeam(dbMgr, m_dicRegularTeams);
            m_listExternalRootTeams = LoadExternalTeam(dbMgr, m_dicExternalTeams);

            LoadCompanyMember(dbMgr, m_dicRegularTeams);
            LoadExternalMember(dbMgr, m_dicExternalTeams);
        }

        public DataTeamControlRoom GetRootControlRoomTeam(Dictionary<int, DataTeamControlRoom> dicTeams = null)
        {
            if (dicTeams == null)
                dicTeams = m_dicControlRoomTeams;

            DataTeamControlRoom team;
            int nID = DataTeamControlRoom.MakeID(0, 0, 0);

            if (!dicTeams.TryGetValue(nID, out team))
            {
                team = new DataTeamControlRoom();
                team.ID = nID;

                dicTeams[nID] = team;
            }

            return team;
        }

        public bool LoadControlRoomTeams(WebDBManager dbMgr, Dictionary<int, DataTeamControlRoom> dicTeams)
        {
            dicTeams.Clear();

            string strSQL = "select cr.ID, cr.RoomType, cr.LocationName, crt.TypeName from ControlRoom as cr, ControlRoomType as crt ";
            strSQL += "where cr.RoomType = crt.ID and crt.SiteID = " + m_nSiteID.ToString() + " order by cr.RoomType";

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            DataTeamControlRoom teamRoot = GetRootControlRoomTeam(dicTeams);

            List<int> controlRoomIDs = new List<int>();
            List<int> roomTypeIDs = new List<int>();
            string strRoomTypeIDs = "";

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                int nControlRoomID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nRoomTypeID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                string strLocationName = WebDBManager.GetStringField(arrResult[i + 2]);
                string strRoomType = WebDBManager.GetStringField(arrResult[i + 3]);

                if (nControlRoomID < 0 || nRoomTypeID < 0 || strLocationName == null || strRoomType == null)
                    continue;

                int nID = DataTeamControlRoom.MakeID(nRoomTypeID, nControlRoomID, 0);

                DataTeamControlRoom team = new DataTeamControlRoom();
                team.ID = nID;
                team.ParentTeam = teamRoot;

                if (strLocationName == strRoomType)
                    team.TeamName = strLocationName;
                else
                    team.TeamName = strLocationName + " " + strRoomType;

                dicTeams[nID] = team;

                if (!roomTypeIDs.Contains(nRoomTypeID))
                {
                    roomTypeIDs.Add(nRoomTypeID);

                    if (strRoomTypeIDs.Length == 0)
                        strRoomTypeIDs = nRoomTypeID.ToString();
                    else
                        strRoomTypeIDs += ", " + nRoomTypeID.ToString();
                }

                if (!controlRoomIDs.Contains(nControlRoomID))
                    controlRoomIDs.Add(nControlRoomID);
            }

            if (roomTypeIDs.Count == 0)
                return true;

            strSQL = string.Format("Select ID, JobName, RoomType from ControlTeamJobPosition where RoomType in ({0})", strRoomTypeIDs);
            arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                int nPositionID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strJobName = WebDBManager.GetStringField(arrResult[i + 1]);
                int nRoomTypeID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);

                if (nPositionID < 0 || nRoomTypeID < 0 || strJobName == null)
                    continue;

                foreach (int nControlRoomID in controlRoomIDs)
                {
                    int nID = DataTeamControlRoom.MakeID(nRoomTypeID, nControlRoomID, nPositionID);

                    DataTeamControlRoom team = new DataTeamControlRoom();
                    team.TeamName = strJobName;
                    team.ID = nID;

                    int nParentTeamID = DataTeamControlRoom.MakeID(nRoomTypeID, nControlRoomID, 0);
                    DataTeamControlRoom parentTeam;

                    if (m_dicControlRoomTeams.TryGetValue(nParentTeamID, out parentTeam))
                        team.ParentTeam = parentTeam;

                    dicTeams[nID] = team;
                }
            }

            return true;
        }

        public bool LoadCompanyMember(WebDBManager dbMgr, Dictionary<int, DataTeam> dicTeams)
        {
            m_dicRegularMembers.Clear();

            string strSQL = string.Format("SELECT TeamID FROM Site WHERE ID = {0}", m_nSiteID);
            ArrayList arrResult1 = dbMgr.GetResultData(strSQL);
            if (arrResult1 == null || arrResult1.Count == 0)
                return false;

            int nTeamID = WebDBManager.GetIntField(arrResult1[0].ToString(), -1);
            if (nTeamID == -1)
                return false;

            ArrayList arrResult2 = ExecuteTeamList(dbMgr, nTeamID);
            //strSQL = string.Format("sp_TeamList2 {0}", nTeamID);
            //ArrayList arrResult2 = dbMgr.GetStoredProcedureData(strSQL, 0);
            if (arrResult2 == null || arrResult2.Count == 0)
                return false;

            string szTeamList = "";
            for (int i = 0; i < arrResult2.Count - 2; i += 3)
            {
                string szTeamID = WebDBManager.GetStringField(arrResult2[i].ToString(), "");
                if (szTeamList != "")
                {
                    szTeamList += ",";
                }
                szTeamList += szTeamID;
            }

            if (szTeamList == "")
            {
                return false;
            }
            string szText = "select rm.RegularTeamID, rm.CompanyMemberID, rm.PositionID, MemberName, LevelID, MemberID, OfficePhoneNumber, PhoneNumber " +
                            " FROM CompanyMember as cm, RegularMemberList as rm WHERE cm.ID = rm.CompanyMemberID and rm.RegularTeamID in ({0})";

            strSQL = string.Format(szText, szTeamList);
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            int nCount = arrResult.Count;
            if (nCount == 0) return true;

            DataCompanyMember member;

            for (int i = 0; i < nCount - 7; i += 8)
            {
                int nRegularTeamID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                int nID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), 0);
                int nPositionID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
                string strMemberName = WebDBManager.GetStringField(arrResult[i + 3], "");
                int nLevelID = WebDBManager.GetIntField(arrResult[i + 4].ToString(), 0);
                string strMemberID = WebDBManager.GetStringField(arrResult[i + 5], "");
                //int nSecondRegularTeamID = WebDBManager.GetIntField(arrResult[i + 6].ToString(), 0);
                //int nSecondPositionID = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0);
                string strOfficePhoneNumber = WebDBManager.GetStringField(arrResult[i + 6], "");
                string strPhoneNumber = WebDBManager.GetStringField(arrResult[i + 7], "");

                if (string.Compare(strPhoneNumber, "null", true) == 0 || strPhoneNumber == "")
                    strPhoneNumber = "";
                else
                    strPhoneNumber = AES256Cipher.AES_decrypt(strPhoneNumber, key);

                strPhoneNumber = ValidPhoneNumber(strPhoneNumber);

                if (string.Compare(strOfficePhoneNumber, "null", true) == 0)
                    strOfficePhoneNumber = "";

                if (!dicTeams.ContainsKey(nRegularTeamID))
                    continue;

                DataTeam team = dicTeams[nRegularTeamID];

                if (!m_dicRegularMembers.TryGetValue(nID, out member))
                {
                    member = new DataCompanyMember();

                    member.ID = nID;
                    member.MemberName = strMemberName;
                    member.LevelID = nLevelID;
                    member.MemberID = strMemberID;
                    member.OfficePhoneNumber = strOfficePhoneNumber;
                    member.PhoneNumber = strPhoneNumber;

                    m_dicRegularMembers[nID] = member;
                }
                
                ArrayList arrMembers = null;

                if (m_dicRegularTeamMembers.ContainsKey(team))
                    arrMembers = m_dicRegularTeamMembers[team];
                else
                {
                    arrMembers = new ArrayList();
                    m_dicRegularTeamMembers[team] = arrMembers;
                }

                arrMembers.Add(member);
                member.TeamPositions[team] = nPositionID;
            }

            foreach (KeyValuePair<DataTeam, ArrayList> pair in m_dicRegularTeamMembers)
            {
                pair.Value.Sort();
            }

            return true;
        }

        public static ArrayList ExecuteTeamList(WebDBManager dbMgr, int nRootTeamID, string strTableName = "RegularTeam")
        {
            string strSQL = "Select ID, TeamName, ParentTeamID from " + strTableName + " order by ParentTeamID, ID";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            if (nRootTeamID == 0)
                return arrResult;

            int nResultCount = arrResult.Count;

            ArrayList arrNewResult = new ArrayList();
            Dictionary<int, int> dicParentID = new Dictionary<int, int>();

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strTeamName = WebDBManager.GetStringField(arrResult[i + 1], "");
                VariousData<int> parentID = WebDBManager.GetIntField(arrResult[i + 2].ToString());

                if (dicParentID.Count == 0)
                {
                    if (nID == nRootTeamID)
                    {
                        dicParentID[nID] = nID;

                        arrNewResult.Add(arrResult[i]);
                        arrNewResult.Add(arrResult[i + 1]);
                        arrNewResult.Add(arrResult[i + 2]);
                    }
                }
                else
                {
                    if (parentID == null)
                        continue;

                    if (dicParentID.ContainsKey(parentID.Data))
                    {
                        dicParentID[nID] = nID;

                        arrNewResult.Add(arrResult[i]);
                        arrNewResult.Add(arrResult[i + 1]);
                        arrNewResult.Add(arrResult[i + 2]);
                    }
                }
            }

            return arrNewResult;
        }

        public bool LoadExternalMember(WebDBManager dbMgr, Dictionary<int, DataTeam> dicTeams)
        {
            m_dicExternalMembers.Clear();

            StringBuilder sb1 = new StringBuilder();
            sb1.Append("Select eml.ExternalCompanyTeamID, eml.ExternalCompanyMemberID, ecm.Name, ecm.PhoneNumber ");
            sb1.Append("from ExternalCompanyMember as ecm, ExternalMemberList as eml, ExternalTeam as et ");
            sb1.AppendFormat("where eml.ExternalCompanyMemberID = ecm.ID and et.ID = eml.ExternalCompanyTeamID and et.SiteID = {0}", m_nSiteID);

            string szSQL = sb1.ToString();

            ArrayList arrResult = dbMgr.GetResultData(szSQL);
            if (arrResult == null)
                return false;

            int nCount = arrResult.Count;
            if (nCount == 0)
                return true;

            DataExternalMember member;

            for (int i = 0; i < nCount - 3; i += 4)
            {
                int nTeamID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                int nID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), 0);
                //bool nLeader = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0) == 1;
                string strMemberName = WebDBManager.GetStringField(arrResult[i + 2], "");
                string szPhoneNumber = WebDBManager.GetStringField(arrResult[i + 3].ToString(), "");

                if (!dicTeams.ContainsKey(nTeamID))
                    return false;

                DataTeam team = dicTeams[nTeamID];

                if (string.Compare(szPhoneNumber, "null", true) == 0 || szPhoneNumber == "")
                    szPhoneNumber = "";
                else
                    szPhoneNumber = AES256Cipher.AES_decrypt(szPhoneNumber, key);

                szPhoneNumber = ValidPhoneNumber(szPhoneNumber);

                if (!m_dicExternalMembers.TryGetValue(nID, out member))
                {
                    member = new DataExternalMember();

                    member.ID = nID;
                    member.Name = strMemberName;
                    member.PhoneNumber = szPhoneNumber;
                    member.Team = team;
                    //member.TeamLeaders[team] = nLeader;

                    m_dicExternalMembers[nID] = member;
                }
                
                ArrayList arrMembers = null;

                if (m_dicExternalTeamMembers.ContainsKey(team))
                    arrMembers = m_dicExternalTeamMembers[team];
                else
                {
                    arrMembers = new ArrayList();
                    m_dicExternalTeamMembers[team] = arrMembers;
                }
                
                arrMembers.Add(member);
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
        private ArrayList LoadExternalTeam(WebDBManager dbMgr, Dictionary<int, DataTeam> dicTeams)
        {
            dicTeams.Clear();

            ArrayList arrExternalRootTeams = new ArrayList();
            string szText2 = "SELECT et.ID, et.TeamName, et.ParentTeamID " +
                             " FROM ExternalTeam as et WHERE et.SiteID = {0} ";

            string szSQL = string.Format(szText2, m_nSiteID);

            ArrayList arrResult = dbMgr.GetResultData(szSQL);
            if (arrResult == null)
                return null;

            // 자신의 Team, 부모 팀의 ID
            Dictionary<DataTeam, int> dicParentID = new Dictionary<DataTeam, int>();

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string szTeamName = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nParentTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                //int nCompanyID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);


                DataTeam data = new DataTeam();
                data.ID = nID;
                data.TeamName = szTeamName;
                data.External = true;
                //data.CompanyName = szTeamName;

                if (nParentTeamID == -1)
                {
                    //data.ParentTeam = teamCompany;
                    data.IsCompany = true;
                    data.CompanyName = szTeamName;

                    if (!arrExternalRootTeams.Contains(data))
                    {
                        arrExternalRootTeams.Add(data);
                    }
                }
                else
                {
                    dicParentID[data] = nParentTeamID;
                }

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
                pair.Key.CompanyName = teamParent.CompanyName;
            }

            return arrExternalRootTeams;
        }

        // dicTeams : ID별 Team
        private DataTeam LoadRegularTeam(WebDBManager dbMgr, Dictionary<int, DataTeam> dicTeams)
        {
            dicTeams.Clear();
            //string szSQL = "SELECT R.ID, R.TeamName, R.ParentTeamID FROM RegularTeam as R";

            string strSQL = string.Format("SELECT TeamID FROM Site WHERE ID = {0}", m_nSiteID);
            ArrayList arrResult1 = dbMgr.GetResultData(strSQL);
            if (arrResult1 == null || arrResult1.Count == 0)
                return null;

            int nTeamID = WebDBManager.GetIntField(arrResult1[0].ToString(), -1);
            if (nTeamID == -1)
                return null;

            ArrayList arrResult = ExecuteTeamList(dbMgr, nTeamID);
            //strSQL = string.Format("sp_TeamList2 {0}", nTeamID);
            //ArrayList arrResult = dbMgr.GetStoredProcedureData(strSQL, 0);
            if (arrResult == null || arrResult.Count == 0)
                return null;

            // 자신의 Team, 부모 팀의 ID
            Dictionary<DataTeam, int> dicParentID = new Dictionary<DataTeam, int>();

            int nCount = arrResult.Count;

            for (int i = 0; i < nCount - 2; i += 3)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string szTeamName = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nParentTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);

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

        private ISensorTooltipOwner m_ContentView = null;

        public bool LoadPOI(WebDBManager dbMgr, ISensorTooltipOwner view, bool isIndoor)
        {
            m_ContentView = view;
            //if (isIndoor == false)
            {
                Zone zone = FormMain.Instance.GetZone();
                if (zone != null)
                {
                    //  현재 3D 화면위에 있는 모든 POI들을 삭제한다.
                    if (view != null)
                        view.ClearPOI("");

                    if (!CCTVManager.Instance.LoadCCTVFile(view, true, zone.ID, PopupDialog.Controls.uPoiVisible.bVisiblePOICCTV))
                        return false;

                    //LoadFireSensorPOI(dbMgr, false, true);
                }
                if (!CCTVManager.Instance.LoadCCTV2(view, true/*isIndoor*/))
                    return false;
                if (!SensorManager.Instance.LoadAllSensor(view, isIndoor))
                    return false;
            }
            
            return true;
        }

        public void LoadCCTVPOI(WebDBManager dbMgr, bool clearPOI, bool isIndoor)
        {
            if (m_ContentView == null)
                return;

            if (clearPOI)
            {
                //  현재 3D 화면위에 있는 모든 POI들을 삭제한다.
                m_ContentView.ClearPOI("");
            }

            Zone zone = FormMain.Instance.GetZone();
            if (zone != null)
            {
                //if (PopupDialog.Controls.uPoiVisible.bVisiblePOICCTV)
                {
                    CCTVManager.Instance.LoadCCTVFile(m_ContentView, isIndoor, zone.ID, PopupDialog.Controls.uPoiVisible.bVisiblePOICCTV);
                }
            }
        }

        public void LoadSensorPOI(WebDBManager dbMgr, bool clearPOI, bool isIndoor, IFacility.FacilityType type)
        {
            if (m_ContentView == null)
                return;

            if (clearPOI)
                m_ContentView.ClearPOI("");

            Zone zone = FormMain.Instance.GetZone();
            if (zone != null)
            {
                if (type == IFacility.FacilityType.FIRE_SENSOR)
                    LoadFireSensorPOI(dbMgr, zone.ID, isIndoor);
                else if (type == IFacility.FacilityType.DOOR || type == IFacility.FacilityType.FIREWALL)
                    LoadEtcSensorPOI(dbMgr, zone.ID, isIndoor, type);
                else if (type == IFacility.FacilityType.PSM_SENSOR)
                    PSMManager.Instance.LoadPSMSensorPOI(dbMgr, zone.ID, isIndoor, m_ContentView, m_dicFirePOI);
            }
        }
        
        public void ClearPOI(string strPOIType = "")
        {
            if (m_ContentView == null)
                return;

            if (strPOIType.Length == 0)
                m_ContentView.ClearPOI("");
            else
                m_ContentView.ClearPOI(strPOIType);
        }

        private void LoadFireSensorPOI(WebDBManager dbMgr, int nZoneID, bool isIndoor)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Select s.ID, Name, X, Y, Z, sz.ID as sensorZoneID ");
            sb.Append("  From FireSensor as s INNER JOIN SensorZone as sz ON s.ID = sz.OrgSensorID ");
            sb.AppendFormat(" Where ZoneID = {0} ", nZoneID);
            sb.AppendFormat("   And IsIndoor = {0} ", (isIndoor) ? 1 : 0);
            sb.AppendFormat("   And Type = {0} ", (int)IFacility.FacilityType.FIRE_SENSOR);

            ArrayList arrResult = dbMgr.GetResultData(sb.ToString());

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            if (nResultCount > 0)
            {
                List<POI> pois = new List<POI>();
                List<int> poiIDs = new List<int>();
                List<string> poiTypes = new List<string>();
                List<bool> poiVisibles = new List<bool>();

                string strPOIType = Data.CommonString.POI_Fire;
                Zone zone = ZoneManager.Instance.GetZone(nZoneID);

                POI alarmPOI = null;
                float maxY = 0;
                int nAlarmIndex = -1;

                for (int i = 0; i < nResultCount - 5; i += 6)
                {
                    VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                    string strName = WebDBManager.GetStringField(arrResult[i + 1]);
                    VariousData<float> x = WebDBManager.GetFloatField(arrResult[i + 2].ToString());
                    VariousData<float> y = WebDBManager.GetFloatField(arrResult[i + 3].ToString());
                    VariousData<float> z = WebDBManager.GetFloatField(arrResult[i + 4].ToString());
                    VariousData<int> sensorZoneId = WebDBManager.GetIntField(arrResult[i + 5].ToString());

                    if (id == null || strName == null || x == null || y == null || z == null || sensorZoneId == null)
                        continue;

                    FireSensor sensor = new FireSensor();

                    sensor.ID = id.Data;
                    sensor.POI = new POI();
                    sensor.POI.X = x.Data;
                    sensor.POI.Y = y.Data;
                    sensor.POI.Z = z.Data;
                    sensor.POI.Zone = zone;
                    sensor.POI.IsIndoor = isIndoor;
                    sensor.POI.ID = id.Data;

                    if (i == 0)
                        maxY = y.Data;
                    else if (y.Data > maxY)
                        maxY = y.Data;
                    
                    if (sensor.POI.Popup == null)
                    {
                        sensor.POI.Popup = sensor.CreatePopup(null, null);
                        sensor.POI.Facility = sensor;
                    }

                    pois.Add(sensor.POI);
                    poiIDs.Add(sensor.POI.ID);
                    Content.TooltipHandler handler = sensor.POI.Popup as Content.TooltipHandler;
                    string poiType = handler == null ? strPOIType : handler.CurrentPOIType;
                    poiTypes.Add(poiType);

                    if (poiType.ToLower().Contains("alarmon"))
                    {
                        alarmPOI = sensor.POI;
                        nAlarmIndex = pois.Count - 1;
                    }

                    /*if (handler == null)
                        poiTypes.Add(strPOIType);
                    else
                        poiTypes.Add(handler.CurrentPOIType);*/
                                        
                    poiVisibles.Add(PopupDialog.Controls.uPoiVisible.bVisiblePOIFire);

                    m_dicFirePOI[sensorZoneId.Data] = sensor.POI;

                    int nLayerID = sensor.GetLayerID();

                    UnE.View.Content.IFormContent formContent = UnE.View.Content.ViewUtils.GetContentView();
                    formContent.Layers.GetLayer(nLayerID).Add(sensor.POI.ID);
                }

                // 알람이 발생한 POI는 눈에 잘 띄도록 다른 POI들보다 더 위에 위치하도록 하고
                // 순서상 가장 마지막에 그려지도록 한다.
                if (alarmPOI != null)
                {
                    alarmPOI.Y = maxY + 2;

                    POI poi = pois[nAlarmIndex];
                    int poiID = poiIDs[nAlarmIndex];
                    string poiType = poiTypes[nAlarmIndex];
                    bool poiVisible = poiVisibles[nAlarmIndex];

                    pois.RemoveAt(nAlarmIndex);
                    poiIDs.RemoveAt(nAlarmIndex);
                    poiTypes.RemoveAt(nAlarmIndex);
                    poiVisibles.RemoveAt(nAlarmIndex);

                    pois.Add(poi);
                    poiIDs.Add(poiID);
                    poiTypes.Add(poiType);
                    poiVisibles.Add(poiVisible);
                }

                string temp = (++m_nTemp).ToString();

                string strFilePath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\" + temp + "AddFirePOI.txt";
                m_ContentView.AddPOIFile(strPOIType, strFilePath, pois);

                strFilePath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\" + temp + "ShowFirePOI.txt";
                m_ContentView.ShowIconPOIFile(strPOIType, strFilePath, poiIDs, poiTypes, poiVisibles);
            }
        }

        private Dictionary<IFacility.FacilityType, List<EtcSensor>> m_dicEtcSensor = new Dictionary<IFacility.FacilityType, List<EtcSensor>>();
        public Dictionary<IFacility.FacilityType, List<EtcSensor>> DicEtcSensor
        {
            get { return m_dicEtcSensor; }
        }

        private void LoadEtcSensorPOI(WebDBManager dbMgr, int nZoneID, bool isIndoor, IFacility.FacilityType type)
        {
            string typeName = "";
            if (type == IFacility.FacilityType.DOOR)
                typeName = Data.CommonString.POI_Door;
            else if (type == IFacility.FacilityType.FIREWALL)
                typeName = Data.CommonString.POI_FireWall;
            else if (type == IFacility.FacilityType.BLACKOUT)
                typeName = Data.CommonString.POI_Blackout;
            else if (type == IFacility.FacilityType.STRONG_WIND)
                typeName = Data.CommonString.POI_StrongWind;

            m_dicEtcSensor.Remove(type);

            StringBuilder sb = new StringBuilder();
            //sb.Append("Select s.ID, Name, X, Y, Z, sz.ID as sensorZoneID ");
            //sb.AppendFormat("  From {0} as s INNER JOIN SensorZone as sz ON s.ID = sz.OrgSensorID ", typeName + "Sensor");
            //sb.AppendFormat(" Where ZoneID = {0} ", nZoneID);
            //sb.AppendFormat("   And IsIndoor = {0} ", (isIndoor) ? 1 : 0);
            //sb.AppendFormat("   And Type = {0} ", (int)type);

            sb.Append("Select ID, Name, X, Y, Z ");
            sb.AppendFormat("  From {0} ", typeName + "Sensor");
            sb.AppendFormat(" Where ZoneID = {0} ", nZoneID);
            sb.AppendFormat("   And IsIndoor = {0} ", (isIndoor) ? 1 : 0);
            //sb.AppendFormat("   And Type = {0} ", (int)type);

            ArrayList arrResult = dbMgr.GetResultData(sb.ToString());

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            if (nResultCount > 0)
            {
                List<POI> pois = new List<POI>();
                List<int> poiIDs = new List<int>();
                List<string> poiTypes = new List<string>();
                List<bool> poiVisibles = new List<bool>();

                string strPOIType = typeName.Replace("Sensor", "");
                Zone zone = ZoneManager.Instance.GetZone(nZoneID);

                for (int i = 0; i < nResultCount - 4; i += 5)
                {
                    VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                    string strName = WebDBManager.GetStringField(arrResult[i + 1]);
                    VariousData<float> x = WebDBManager.GetFloatField(arrResult[i + 2].ToString());
                    VariousData<float> y = WebDBManager.GetFloatField(arrResult[i + 3].ToString());
                    VariousData<float> z = WebDBManager.GetFloatField(arrResult[i + 4].ToString());
                    //VariousData<int> sensorZoneId = WebDBManager.GetIntField(arrResult[i + 5].ToString());

                    if (id == null || strName == null || x == null || y == null || z == null)
                        continue;

                    EtcSensor sensor = new EtcSensor(type);
                    sensor.SensorName = strName;
                    sensor.ID = id.Data;
                    sensor.POI = new POI();
                    sensor.POI.X = x.Data;
                    sensor.POI.Y = y.Data;
                    sensor.POI.Z = z.Data;
                    sensor.POI.Zone = zone;
                    sensor.POI.IsIndoor = isIndoor;
                    sensor.POI.ID = id.Data;
                    
                    if (sensor.POI.Popup == null)
                    {
                        sensor.POI.Popup = sensor.CreatePopup(null, null);
                        sensor.POI.Facility = sensor;
                    }

                    pois.Add(sensor.POI);
                    poiIDs.Add(sensor.POI.ID);

                    Content.TooltipHandler handler = sensor.POI.Popup as Content.TooltipHandler;
                    if (handler == null)
                        poiTypes.Add(strPOIType);
                    else
                        poiTypes.Add(handler.CurrentPOIType);

                    if (type == IFacility.FacilityType.DOOR)                        
                        poiVisibles.Add(PopupDialog.Controls.uPoiVisible.bVisiblePOIDoor);
                    else
                        poiVisibles.Add(true);
                    //m_dicPOI[sensorZoneId.Data] = sensor.POI;

                    int nLayerID = sensor.GetLayerID();

                    UnE.View.Content.IFormContent formContent = UnE.View.Content.ViewUtils.GetContentView();
                    formContent.Layers.GetLayer(nLayerID).Add(sensor.POI.ID);

                    if (!m_dicEtcSensor.ContainsKey(type))
                        m_dicEtcSensor.Add(type, new List<EtcSensor>());
                    m_dicEtcSensor[type].Add(sensor);
                }
                
                string strFilePath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Add" + strPOIType + "POI.txt"; 
                m_ContentView.AddPOIFile(strPOIType, strFilePath, pois);

                strFilePath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Show" + strPOIType + "POI.txt";
                m_ContentView.ShowIconPOIFile(strPOIType, strFilePath, poiIDs, poiTypes, poiVisibles);
            }
        }

        public void LoadCloseDoorPOI(int nZoneID, ref List<POI> pois, ref List<string> poiTypes)
        {
            if (!m_dicEtcSensor.ContainsKey(IFacility.FacilityType.DOOR))
                return;

            Zone zone = ZoneManager.Instance.GetZone(nZoneID);
            
            pois = new List<POI>();
            poiTypes = new List<string>();

            string strPOIType = Data.CommonString.POI_Door;
                        
            List<ISensor> doors = SensorManager.Instance.DicDoorSensorByZoneID[nZoneID]; //m_dicEtcSensor[IFacility.FacilityType.DOOR];
            List<EtcSensor> curDoors = new List<EtcSensor>();
            foreach (EtcSensor door in doors)
            {
                if (door.POI.Zone.ID == zone.ID)
                {                    
                    curDoors.Add(door);
                    //pois.Add(door.POI);
                    //poiTypes.Add(strPOIType);
                }
            }
            
            foreach (ISensor item in curDoors)
            {
                item.IconPath = strPOIType;
            }

            pois = new List<POI>();
            poiTypes = new List<string>();

            StringBuilder sb = new StringBuilder();
            sb.Append("Select ID, Name, Description ");
            sb.Append("  From DoorSensor ");
            sb.AppendFormat(" Where ZoneID = {0}", nZoneID);
            
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(sb.ToString());
            if (arrResult == null || arrResult.Count == 0)
                return;
            
            for (int i = 0; i < arrResult.Count; i+=3)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strName = WebDBManager.GetStringField(arrResult[i + 1]);
                string strStatus = WebDBManager.GetStringField(arrResult[i + 2]);
                
                for (int j = 0; j < curDoors.Count; j++)
                {
                    EtcSensor sensor = curDoors[j] as EtcSensor;                    
                    if (sensor.ID == nID)
                    {
                        if (sensor.Description != strStatus)
                        {
                            sensor.Description = strStatus;
                            pois.Add(curDoors[j].POI);
                            
                            Content.TooltipHandler handler = curDoors[j].POI.Popup as Content.TooltipHandler;
                            if (handler == null)
                                poiTypes.Add(strPOIType);
                            else
                            {
                                if (strStatus == "닫힘")
                                    handler.CurrentPOIType = strPOIType + Data.CommonString.AlarmTag;
                                else
                                    handler.CurrentPOIType = strPOIType;

                                poiTypes.Add(handler.CurrentPOIType); 
                            }
                        }
                        
                        break;
                    } 
                }


            }
        }
        
        private Dictionary<int, POI> m_dicFirePOI = new Dictionary<int, POI>();
        public Dictionary<int, POI> DicFirePOI
        {
            get { return m_dicFirePOI; }
            set { m_dicFirePOI = value; }
        }

        public void SetPOIIcon(ISensor sensor)
        {            
            if (m_dicFirePOI.ContainsKey(sensor.ID))
            {
                POI poi = m_dicFirePOI[sensor.ID];
                if (poi == null)
                    return;

                if (poi.Facility != null)
                    poi.Facility.IconPath = Data.CommonString.POI_Fire;

                TooltipHandler handler = poi.Popup as TooltipHandler;
                handler.CurrentPOIType = Data.CommonString.POI_Fire;

                ChangePOIIcon(poi, Data.CommonString.POI_Fire);

                if (m_dicFirePOI.ContainsKey(4))
                {
                    POI poi2 = m_dicFirePOI[4];
                    if (poi2 == null)
                        return;

                    if (poi2.Facility != null)
                        poi2.Facility.IconPath = Data.CommonString.POI_Door;

                    if (poi2.Popup != null)
                    {
                        TooltipHandler handler2 = poi2.Popup as TooltipHandler;
                        handler2.CurrentPOIType = Data.CommonString.POI_Door;

                        ChangePOIIcon(poi2, Data.CommonString.POI_Door);
                    }
                }
            }
        }

        private SortedList<int, int> m_arGroupEquipPair = new SortedList<int, int>();

        public void LoadFireEquipmentGroup()
        {
            WebDBManager dbMgr = FormMain.Instance.DBManager;
            //string strSQL = "Select id, linkedEquipID from FireEquipmentGroup";

            string szText = "SELECT feg.id, feg.linkedEquipID FROM FireEquipmentGroup as feg " +
                            " INNER JOIN FireEquipment as fe ON feg.linkedEquipID = fe.ID " +
                            " INNER JOIN Zone as z ON fe.ZoneID = z.ID and z.SiteID = {0} ORDER BY feg.id";

            string strSQL = string.Format(szText, m_nSiteID);

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nEquipType = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);

                if (nID < 0)
                    continue;
                m_arGroupEquipPair.Add(nEquipType, nID);
            }
        }

        public void LoadFireEquipment()
        {
            // return;
            LoadFireEquipmentGroup();

            WebDBManager dbMgr = FormMain.Instance.DBManager;

            //string strSQL = "Select ID, RFIDTag, EquipID, RFIDTagID, DxfObjID, EquipType, ZoneID, X, Y, Z, Description from FireEquipment";

            string szText = "SELECT fe.ID, fe.RFIDTag, fe.EquipID, fe.RFIDTagID,fe. DxfObjID, fe.EquipType, fe.ZoneID, fe.X, fe.Y, fe.Z, fe.Description " +
                            " FROM FireEquipment AS fe " +
                            "   INNER JOIN Zone AS z ON fe.ZoneID = z.ID AND z.SiteID = {0}";

            string strSQL = string.Format(szText, m_nSiteID);

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            Dictionary<int, FireEquipmentHistory> dicHistory = LoadFireEquipmentHistory();

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 10; i += 11)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strRFIDTag = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strEquipID = WebDBManager.GetStringField(arrResult[i + 2], "");
                string strRFIDTagID = WebDBManager.GetStringField(arrResult[i + 3], "");
                string strDxfObjID = WebDBManager.GetStringField(arrResult[i + 4], "");
                int nEquipType = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                int nZoneID = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                float x = WebDBManager.GetFloatField(arrResult[i + 7].ToString(), 0.0f);
                float y = WebDBManager.GetFloatField(arrResult[i + 8].ToString(), 0.0f);
                float z = WebDBManager.GetFloatField(arrResult[i + 9].ToString(), 0.0f);
                string strDescription = WebDBManager.GetStringField(arrResult[i + 10], "");

                if (nID < 0)
                    continue;

                IFacility.FacilityType type = IFacility.FacilityType.NONE;

                if (nEquipType == (int)IFacility.FacilityType.FE)
                {
                    type = IFacility.FacilityType.FE;
                }
                else if (nEquipType == (int)IFacility.FacilityType.HD)
                {
                    type = IFacility.FacilityType.HD;
                }
                else if (nEquipType == (int)IFacility.FacilityType.FA)
                {
                    type = IFacility.FacilityType.FA;
                }
                else if (nEquipType == (int)IFacility.FacilityType.FR)
                {
                    type = IFacility.FacilityType.FR;
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
                equip.TagID = strRFIDTagID;
                equip.Zone = ZoneManager.Instance.GetZone(nZoneID);

                if (equip.Zone == null || equip.Zone.Polygon == null)
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
            //string strSQL = "select ID, FireEquipmentID, Time, status, CheckersOpinion from FireEquipmentHistory order by FireEquipmentID";

            string szText = "SELECT feh.ID, feh.FireEquipmentID, feh.Time, feh.status, CheckersOpinion FROM FireEquipmentHistory AS feh " +
                            " INNER JOIN FireEquipment AS fe ON fe.ID = feh.FireEquipmentID " +
                            " INNER JOIN Zone AS z ON fe.ZoneID = z.ID and z.SiteID = {0} " +
                            " ORDER BY FireEquipmentID";

            string strSQL = string.Format(szText, m_nSiteID);

            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);
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
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nEquipID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                DateTime dtLastChecked = WebDBManager.GetDateTimeField(arrResult[i + 2], dtDefault);
                int nStatus = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                string strOpinion = WebDBManager.GetStringField(arrResult[i + 4], "");

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

        public void AddEquipZoneFacilityManager(FacilityManager mgr, EquipmentZone zone, IFacility.FacilityType type)
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

        public void AddEquipZoneFacilityManagerReport(FacilityManager mgr, EquipmentZone zone, IFacility.FacilityType type)
        {
            if (m_dicEquipZoneFacilityManagerReport.ContainsKey(type))
            {
                Dictionary<int, FacilityManagerGroup> dicManagers = m_dicEquipZoneFacilityManagerReport[type];

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
                m_dicEquipZoneFacilityManagerReport[type] = dicManagers;

                FacilityManagerGroup group = new FacilityManagerGroup();
                group.EquipZone = zone;
                group.Type = type;

                group.AddManager(mgr);
                dicManagers[zone.ID] = group;
            }
        }

        public void AddBuildingFacilityManager(FacilityManager mgr, Building building, IFacility.FacilityType type)
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

        public void AddBuildingFacilityManagerReport(FacilityManager mgr, Building building, IFacility.FacilityType type)
        {
            if (m_dicBuildingFacilityManagerReport.ContainsKey(type))
            {
                Dictionary<Building, FacilityManagerGroup> dicManagers = m_dicBuildingFacilityManagerReport[type];

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
                m_dicBuildingFacilityManagerReport[type] = dicManagers;

                FacilityManagerGroup group = new FacilityManagerGroup();
                group.Building = building;
                group.Type = type;

                group.AddManager(mgr);
                dicManagers[building] = group;
            }
        }

        public void AddOutdoorFacilityManager(FacilityManager mgr, Zone zone, IFacility.FacilityType type)
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

        public void AddOutdoorFacilityManagerReport(FacilityManager mgr, Zone zone, IFacility.FacilityType type)
        {
            if (m_dicOutdoorFacilityManagerReport.ContainsKey(type))
            {
                Dictionary<Zone, FacilityManagerGroup> dicManagers = m_dicOutdoorFacilityManagerReport[type];

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
                m_dicOutdoorFacilityManagerReport[type] = dicManagers;

                FacilityManagerGroup group = new FacilityManagerGroup();
                group.Zone = zone;
                group.Type = type;

                group.AddManager(mgr);
                dicManagers[zone] = group;
            }
        }

        public void AddFacilityManager(FacilityManager mgr, IFacility.FacilityType type)
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

        public void AddFacilityManagerReport(FacilityManager mgr, IFacility.FacilityType type)
        {
            if (m_dicEntireFacilityManagersReport.ContainsKey(type))
            {
                FacilityManagerGroup group = m_dicEntireFacilityManagersReport[type];
                group.AddManager(mgr);
            }
            else
            {
                FacilityManagerGroup group = new FacilityManagerGroup();
                group.Type = type;

                group.AddManager(mgr);
                m_dicEntireFacilityManagersReport[type] = group;
            }
        }

        public void LoadFacilityManager()
        {
            m_dicEntireFacilityManagers.Clear();
            m_dicBuildingFacilityManager.Clear();
            m_dicOutdoorFacilityManager.Clear();
            m_dicEquipZoneFacilityManager.Clear();

            m_dicEntireFacilityManagersReport.Clear();
            m_dicBuildingFacilityManagerReport.Clear();
            m_dicOutdoorFacilityManagerReport.Clear();
            m_dicEquipZoneFacilityManagerReport.Clear();

            WebDBManager dbMgr = FormMain.Instance.DBManager;
            m_useReportFacilityManagers = FormMain.Instance.OptionMgr.UseFacilityManagerType(dbMgr);

            LoadFacilityManager(dbMgr, true);
            LoadBuildingNOutdoorFacilityManager(dbMgr, true);
            LoadEquipZoneFacilityManager(dbMgr, true);

            if (m_useReportFacilityManagers)
            {
                LoadFacilityManager(dbMgr, false);
                LoadBuildingNOutdoorFacilityManager(dbMgr, false);
                LoadEquipZoneFacilityManager(dbMgr, false);
            }
        }
        
        private void LoadEquipZoneFacilityManager(WebDBManager dbMgr, bool isDetectTime)
        {
            string strTableName = isDetectTime ? "EquipZoneFacilityManager" : "EquipZoneFacilityManagerReport";
            string szText = "select id, MemberID, MemberType, FacilityType, LevelLimit, EquipZoneID, UpperLimit, Description from {1} WHERE SiteID = {0} " +
                     "order by FacilityType";

            string strSQL = string.Format(szText, m_nSiteID, strTableName);

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 7; i += 8)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nMemberID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nMemberType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nFacilityType = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                int nLevelLimit = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                int nEquipZoneID = WebDBManager.GetIntField(arrResult[i + 5].ToString(), 0);
                int nUseUpper = WebDBManager.GetIntField(arrResult[i + 6].ToString(), 0);
                string strDescription = WebDBManager.GetStringField(arrResult[i + 7], "");
                int nBuildingID = -1;

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
                    nBuildingID = zone.Building.ID;
                    group = GetEquipZoneFacilityManagerGroup(nFacilityType, zone, isDetectTime);
                }

                if (group == null)
                    continue;

                AddFacilityManager(nID, nMemberID, nMemberType, nFacilityType, nLevelLimit, nUseUpper, strDescription, group, nBuildingID, nEquipZoneID);
            }
        }

        private void LoadBuildingNOutdoorFacilityManager(WebDBManager dbMgr, bool isDetectTime)
        {
            string strTableName = isDetectTime ? "BuildingFacilityManager" : "BuildingFacilityManagerReport";
            string szText = "select id, MemberID, MemberType, FacilityType, LevelLimit, BuildingID, UpperLimit, Description from {1} WHERE SiteID = {0} " +
                     " order by FacilityType";

            string strSQL = string.Format(szText, m_nSiteID, strTableName);

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 7; i += 8)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nMemberID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nMemberType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nFacilityType = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                int nLevelLimit = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                int nBuildingID = WebDBManager.GetIntField(arrResult[i + 5].ToString(), 0);
                int nUseUpper = WebDBManager.GetIntField(arrResult[i + 6].ToString(), 0);
                string strDescription = WebDBManager.GetStringField(arrResult[i + 7], "");

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
                    group = GetBuildingFacilityManagerGroup(nFacilityType, building, isDetectTime);
                }
                else if (nBuildingID < 0)
                {
                    Zone zone = ZoneManager.Instance.GetZone(-nBuildingID);

                    if (zone == null)
                        continue;

                    group = GetOutdoorFacilityManagerGroup(nFacilityType, zone, isDetectTime);
                }

                if (group == null)
                    continue;

                AddFacilityManager(nID, nMemberID, nMemberType, nFacilityType, nLevelLimit, nUseUpper, strDescription, group, nBuildingID);
            }
        }

        private void LoadFacilityManager(WebDBManager dbMgr, bool isDetectTime)
        {
            string strTableName = isDetectTime ? "FacilityManager" : "FacilityManagerReport";

            string szText = "SELECT fm.id, fm.MemberID, fm.MemberType, fm.FacilityType, fm.LevelLimit, fm.UpperLimit, fm.Description FROM {1} as fm " +
                            " WHERE fm.SiteID = {0} ORDER BY FacilityType";
            string strSQL = string.Format(szText, m_nSiteID, strTableName);

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 6; i += 7)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nMemberID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nMemberType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nFacilityType = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                int nLevelLimit = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                int nUseUpper = WebDBManager.GetIntField(arrResult[i + 5].ToString(), 0);
                string strDescription = WebDBManager.GetStringField(arrResult[i + 6], "");

                if (nID < 0 || nMemberID < 0)
                    continue;

                FacilityManagerGroup group = GetFacilityManagerGroup(nFacilityType, isDetectTime);
                if (group == null)
                    continue;

                AddFacilityManager(nID, nMemberID, nMemberType, nFacilityType, nLevelLimit, nUseUpper, strDescription, group);
            }
        }

        private void AddFacilityManager(int nID, int nMemberID, int nMemberType, int nFacilityType, int nLevelLimit, int nUseUpperLevel, string strDescription, FacilityManagerGroup group, int nBuildingID = -1, int nEquipZoneID = -1)
        {
            FacilityManager mgr = new FacilityManager();
            mgr.ID = nID;
            mgr.MemberID = nMemberID;
            mgr.MemberType = nMemberType;
            mgr.Type = IFacility.ToFacilityType(nFacilityType);
            mgr.LevelLimit = nLevelLimit;
            mgr.UpperLimit = nUseUpperLevel;
            mgr.Description = strDescription;

            if (nBuildingID != -1)
            {
                mgr.Building = ZoneManager.Instance.DicBuildings[nBuildingID];
            }

            if (nEquipZoneID != -1)
            {
                mgr.EquipZone = ZoneManager.Instance.DicEquipZones[nEquipZoneID];

            }

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
                group.ExternalTeams.Add(mgr);
            }
            /*else if (nMemberType == 6)
            {
                DataTeam team = TeamDuty;

                mgr.Tag = team;
                group.RegularTeams.Add(mgr);
            }*/
            else if (nMemberType == 7)
            {
                DataTeamControlRoom team = GetControlRoomTeam(nMemberID);
                mgr.Tag = team;
                group.ControlRoomMembers.Add(mgr);
            }
        }

        public DataTeamControlRoom GetControlRoomTeam(int nMemberID)
        {
            DataTeamControlRoom team;

            if (m_dicControlRoomTeams.TryGetValue(nMemberID, out team))
                return team;

            int nRoomTypeID, nControlRoomID, nPositionID;
            DataTeamControlRoom.GetParams(nMemberID, out nRoomTypeID, out nControlRoomID, out nPositionID);

            string strSQL = string.Format("select cr.LocationName, crt.TypeName from ControlRoom as cr, ControlRoomType as crt where cr.RoomType = crt.ID and crt.SiteID = {0} and cr.RoomType = {1} and cr.ID = {2}",
                m_nSiteID, nRoomTypeID, nControlRoomID);
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count != 2)
                return null;

            string strLocationName = WebDBManager.GetStringField(arrResult[0]);
            string strTypeName = WebDBManager.GetStringField(arrResult[1]);

            if (strLocationName == null || strTypeName == null)
                return null;

            DataTeamControlRoom teamParent;
            int nParentTeamID = DataTeamControlRoom.MakeID(nRoomTypeID, nControlRoomID, 0);

            if (!m_dicControlRoomTeams.TryGetValue(nParentTeamID, out teamParent))
            {
                teamParent = new DataTeamControlRoom();
                teamParent.ID = nParentTeamID;

                if (strLocationName == strTypeName)
                    teamParent.TeamName = strLocationName;
                else
                    teamParent.TeamName = strLocationName + " " + strTypeName;

                teamParent.ParentTeam = GetRootControlRoomTeam();
                m_dicControlRoomTeams[nParentTeamID] = teamParent;
            }

            if (nPositionID == 0)
                return teamParent;

            strSQL = "Select JobName from ControlTeamJobPosition where ID = " + nPositionID.ToString();
            arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return null;

            string strJobName = WebDBManager.GetStringField(arrResult[0]);

            if (strJobName == null)
                return null;

            team = new DataTeamControlRoom();
            team.ID = nMemberID;
            team.TeamName = strJobName;
            team.ParentTeam = teamParent;

            m_dicControlRoomTeams[team.ID] = team;
            return team;
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

        private FacilityManagerGroup GetOutdoorFacilityManagerGroup(int nFacilityType, Zone zone, bool isDetectTime)
        {
            Dictionary<IFacility.FacilityType, Dictionary<Zone, FacilityManagerGroup>> dicOutdoorFacilityManager = isDetectTime ? m_dicOutdoorFacilityManager : m_dicOutdoorFacilityManagerReport;
            FacilityManagerGroup group = null;

            if (nFacilityType == (int)IFacility.FacilityType.FIRE_SENSOR || (nFacilityType >= (int)IFacility.FacilityType.FireSensor_TypeA && nFacilityType <= (int)IFacility.FacilityType.FireSensor_MonitoringType))// && nFacilityType <= (int)IFacility.FacilityType.PRESSURE_SENSOR)
            {
                group = GetOutdoorFacilityManagerGroup2(IFacility.FacilityType.FIRE_SENSOR, zone, isDetectTime);
            }
            else if (nFacilityType == (int)IFacility.FacilityType.COOLER_SENSOR)
            {
                group = GetOutdoorFacilityManagerGroup2(IFacility.FacilityType.COOLER_SENSOR, zone, isDetectTime);
            }
            else if (nFacilityType == (int)IFacility.FacilityType.PRESSURE_SENSOR)
            {
                group = GetOutdoorFacilityManagerGroup2(IFacility.FacilityType.PRESSURE_SENSOR, zone, isDetectTime);
            }
            else if (nFacilityType == (int)IFacility.FacilityType.CCTV)
            {
                group = GetOutdoorFacilityManagerGroup2(IFacility.FacilityType.CCTV, zone, isDetectTime);
            }
            else if (nFacilityType == (int)IFacility.FacilityType.PSM_SENSOR)
            {
                group = GetOutdoorFacilityManagerGroup2(IFacility.FacilityType.PSM_SENSOR, zone, isDetectTime);
            }
            else if (nFacilityType >= (int)IFacility.FacilityType.FE && nFacilityType <= (int)IFacility.FacilityType.FR)
            {
                IFacility.FacilityType typeFE = IFacility.FacilityType.FE;

                if (dicOutdoorFacilityManager.ContainsKey(typeFE))
                {
                    Dictionary<Zone, FacilityManagerGroup> dicManagers = dicOutdoorFacilityManager[typeFE];

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

                    dicOutdoorFacilityManager[typeFE] = dicManagers;
                    dicOutdoorFacilityManager[IFacility.FacilityType.HD] = dicManagers;
                    dicOutdoorFacilityManager[IFacility.FacilityType.FA] = dicManagers;
                }
            }
            else if (IFacility.IsSecurityType(IFacility.ToFacilityType(nFacilityType)) == true)
            {
                group = GetOutdoorFacilityManagerGroup2(IFacility.FacilityType.Security_Sensor, zone, isDetectTime);
            }

            return group;
        }

        private FacilityManagerGroup GetOutdoorFacilityManagerGroup2(IFacility.FacilityType type, Zone zone, bool isDetectTime)
        {
            Dictionary<IFacility.FacilityType, Dictionary<Zone, FacilityManagerGroup>> dicOutdoorFacilityManager = isDetectTime ? m_dicOutdoorFacilityManager : m_dicOutdoorFacilityManagerReport;
            FacilityManagerGroup group = null;

            if (dicOutdoorFacilityManager.ContainsKey(type))
            {
                Dictionary<Zone, FacilityManagerGroup> dicManagers = dicOutdoorFacilityManager[type];

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

                dicOutdoorFacilityManager[type] = dicManagers;
            }

            return group;
        }

        private FacilityManagerGroup GetEquipZoneFacilityManagerGroup(int nFacilityType, EquipmentZone zone, bool isDetectTime)
        {
            Dictionary<IFacility.FacilityType, Dictionary<int, FacilityManagerGroup>> dicEquipZoneFacilityManager = isDetectTime ? m_dicEquipZoneFacilityManager : m_dicEquipZoneFacilityManagerReport;
            FacilityManagerGroup group = null;

            if (nFacilityType == (int)IFacility.FacilityType.FIRE_SENSOR || (nFacilityType >= (int)IFacility.FacilityType.FireSensor_TypeA && nFacilityType <= (int)IFacility.FacilityType.FireSensor_MonitoringType))// && nFacilityType <= (int)IFacility.FacilityType.PRESSURE_SENSOR)
            {
                group = GetEquipZoneFacilityManagerGroup2(IFacility.FacilityType.FIRE_SENSOR, zone, isDetectTime);
            }
            else if (nFacilityType == (int)IFacility.FacilityType.COOLER_SENSOR)
            {
                group = GetEquipZoneFacilityManagerGroup2(IFacility.FacilityType.COOLER_SENSOR, zone, isDetectTime);
            }
            else if (nFacilityType == (int)IFacility.FacilityType.PRESSURE_SENSOR)
            {
                group = GetEquipZoneFacilityManagerGroup2(IFacility.FacilityType.PRESSURE_SENSOR, zone, isDetectTime);
            }
            else if (nFacilityType == (int)IFacility.FacilityType.CCTV)
            {
                group = GetEquipZoneFacilityManagerGroup2(IFacility.FacilityType.CCTV, zone, isDetectTime);
            }
            else if (nFacilityType == (int)IFacility.FacilityType.PSM_SENSOR)
            {
                group = GetEquipZoneFacilityManagerGroup2(IFacility.FacilityType.PSM_SENSOR, zone, isDetectTime);
            }
            else if (nFacilityType >= (int)IFacility.FacilityType.FE && nFacilityType <= (int)IFacility.FacilityType.FR)
            {
                IFacility.FacilityType typeFE = IFacility.FacilityType.FE;

                if (dicEquipZoneFacilityManager.ContainsKey(typeFE))
                {
                    Dictionary<int, FacilityManagerGroup> dicManagers = dicEquipZoneFacilityManager[typeFE];

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

                    dicEquipZoneFacilityManager[typeFE] = dicManagers;
                    dicEquipZoneFacilityManager[IFacility.FacilityType.HD] = dicManagers;
                    dicEquipZoneFacilityManager[IFacility.FacilityType.FA] = dicManagers;
                }
            }
            else if (IFacility.IsSecurityType(IFacility.ToFacilityType(nFacilityType)) == true)
            {
                group = GetEquipZoneFacilityManagerGroup2(IFacility.FacilityType.Security_Sensor, zone, isDetectTime);
            }

            return group;
        }

        private FacilityManagerGroup GetEquipZoneFacilityManagerGroup2(IFacility.FacilityType type, EquipmentZone zone, bool isDetectTime)
        {
            Dictionary<IFacility.FacilityType, Dictionary<int, FacilityManagerGroup>> dicEquipZoneFacilityManager = isDetectTime ? m_dicEquipZoneFacilityManager : m_dicEquipZoneFacilityManagerReport;
            FacilityManagerGroup group = null;

            if (dicEquipZoneFacilityManager.ContainsKey(type))
            {
                Dictionary<int, FacilityManagerGroup> dicManagers = dicEquipZoneFacilityManager[type];

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

                dicEquipZoneFacilityManager[type] = dicManagers;
            }

            return group;
        }

        private FacilityManagerGroup GetBuildingFacilityManagerGroup(int nFacilityType, Building building, bool isDetectTime)
        {
            Dictionary<IFacility.FacilityType, Dictionary<Building, FacilityManagerGroup>> dicBuildingFacilityManagers = isDetectTime ? m_dicBuildingFacilityManager : m_dicBuildingFacilityManagerReport;
            FacilityManagerGroup group = null;

            if (nFacilityType == (int)IFacility.FacilityType.FIRE_SENSOR || (nFacilityType >= (int)IFacility.FacilityType.FireSensor_TypeA && nFacilityType <= (int)IFacility.FacilityType.FireSensor_MonitoringType))// && nFacilityType <= (int)IFacility.FacilityType.PRESSURE_SENSOR)
            {
                IFacility.FacilityType typeFire = IFacility.FacilityType.FIRE_SENSOR;

                if (dicBuildingFacilityManagers.ContainsKey(typeFire))
                {
                    Dictionary<Building, FacilityManagerGroup> dicManagers = dicBuildingFacilityManagers[typeFire];

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

                    dicBuildingFacilityManagers[typeFire] = dicManagers;
                    //dicBuildingFacilityManagers[IFacility.FacilityType.COOLER_SENSOR] = dicManagers;
                    //dicBuildingFacilityManagers[IFacility.FacilityType.PRESSURE_SENSOR] = dicManagers;
                }
            }
            else if (nFacilityType == (int)IFacility.FacilityType.COOLER_SENSOR)
            {
                group = GetBuildingFacilityManagerGroup2(IFacility.FacilityType.COOLER_SENSOR, building, isDetectTime);
            }
            else if (nFacilityType == (int)IFacility.FacilityType.PRESSURE_SENSOR)
            {
                group = GetBuildingFacilityManagerGroup2(IFacility.FacilityType.PRESSURE_SENSOR, building, isDetectTime);
            }
            else if (nFacilityType == (int)IFacility.FacilityType.PSM_SENSOR)
            {
                group = GetBuildingFacilityManagerGroup2(IFacility.FacilityType.PSM_SENSOR, building, isDetectTime);
            }
            else if (nFacilityType == (int)IFacility.FacilityType.CCTV)
            {
                group = GetBuildingFacilityManagerGroup2(IFacility.FacilityType.CCTV, building, isDetectTime);
            }
            else if (nFacilityType >= (int)IFacility.FacilityType.FE && nFacilityType <= (int)IFacility.FacilityType.FR)
            {
                IFacility.FacilityType typeFE = IFacility.FacilityType.FE;

                if (dicBuildingFacilityManagers.ContainsKey(typeFE))
                {
                    Dictionary<Building, FacilityManagerGroup> dicManagers = dicBuildingFacilityManagers[typeFE];

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

                    dicBuildingFacilityManagers[typeFE] = dicManagers;
                    dicBuildingFacilityManagers[IFacility.FacilityType.HD] = dicManagers;
                    dicBuildingFacilityManagers[IFacility.FacilityType.FA] = dicManagers;
                }
            }
            else if (IFacility.IsSecurityType(IFacility.ToFacilityType(nFacilityType)) == true)
            {
                group = GetBuildingFacilityManagerGroup2(IFacility.FacilityType.Security_Sensor, building, isDetectTime);
            }

            return group;
        }

        private FacilityManagerGroup GetBuildingFacilityManagerGroup2(IFacility.FacilityType type, Building building, bool isDetectTime)
        {
            Dictionary<IFacility.FacilityType, Dictionary<Building, FacilityManagerGroup>> dicBuildingFacilityManagers = isDetectTime ? m_dicBuildingFacilityManager : m_dicBuildingFacilityManagerReport;
            FacilityManagerGroup group = null;

            if (dicBuildingFacilityManagers.ContainsKey(type))
            {
                Dictionary<Building, FacilityManagerGroup> dicManagers = dicBuildingFacilityManagers[type];

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

                dicBuildingFacilityManagers[type] = dicManagers;
            }

            return group;
        }

        private FacilityManagerGroup GetFacilityManagerGroup(int nFacilityType, bool isDetectTime)
        {
            Dictionary<IFacility.FacilityType, FacilityManagerGroup> dicFacilityManagers = isDetectTime ? m_dicEntireFacilityManagers : m_dicEntireFacilityManagersReport;

            FacilityManagerGroup group = null;

            if (nFacilityType == (int)IFacility.FacilityType.FIRE_SENSOR || (nFacilityType >= (int)IFacility.FacilityType.FireSensor_TypeA && nFacilityType <= (int)IFacility.FacilityType.FireSensor_MonitoringType))// && nFacilityType <= (int)IFacility.FacilityType.PRESSURE_SENSOR)
            {
                IFacility.FacilityType typeFire = IFacility.FacilityType.FIRE_SENSOR;

                if (dicFacilityManagers.ContainsKey(typeFire))
                    group = dicFacilityManagers[typeFire];
                else
                {
                    group = new FacilityManagerGroup();
                    group.Type = typeFire;

                    dicFacilityManagers[typeFire] = group;
                    //dicFacilityManagers[IFacility.FacilityType.COOLER_SENSOR] = group;
                    //dicFacilityManagers[IFacility.FacilityType.PRESSURE_SENSOR] = group;
                }
            }
            else if (nFacilityType == (int)IFacility.FacilityType.COOLER_SENSOR)
            {
                IFacility.FacilityType type = IFacility.FacilityType.COOLER_SENSOR;

                if (dicFacilityManagers.ContainsKey(type))
                    group = dicFacilityManagers[type];
                else
                {
                    group = new FacilityManagerGroup();
                    group.Type = type;
                    dicFacilityManagers[type] = group;
                }
            }
            else if (nFacilityType == (int)IFacility.FacilityType.PRESSURE_SENSOR)
            {
                IFacility.FacilityType type = IFacility.FacilityType.PRESSURE_SENSOR;

                if (dicFacilityManagers.ContainsKey(type))
                    group = dicFacilityManagers[type];
                else
                {
                    group = new FacilityManagerGroup();
                    group.Type = type;
                    dicFacilityManagers[type] = group;
                }
            }
            else if (nFacilityType == (int)IFacility.FacilityType.PSM_SENSOR)
            {
                IFacility.FacilityType type = IFacility.FacilityType.PSM_SENSOR;

                if (dicFacilityManagers.ContainsKey(type))
                    group = dicFacilityManagers[type];
                else
                {
                    group = new FacilityManagerGroup();
                    group.Type = type;
                    dicFacilityManagers[type] = group;
                }
            }
            else if (nFacilityType == (int)IFacility.FacilityType.CCTV)
            {
                IFacility.FacilityType type = IFacility.FacilityType.CCTV;

                if (dicFacilityManagers.ContainsKey(type))
                    group = dicFacilityManagers[type];
                else
                {
                    group = new FacilityManagerGroup();
                    group.Type = type;
                    dicFacilityManagers[type] = group;
                }
            }
            else if (nFacilityType >= (int)IFacility.FacilityType.FE && nFacilityType <= (int)IFacility.FacilityType.FR)
            {
                IFacility.FacilityType typeFE = IFacility.FacilityType.FE;

                if (dicFacilityManagers.ContainsKey(typeFE))
                    group = dicFacilityManagers[typeFE];
                else
                {
                    group = new FacilityManagerGroup();
                    group.Type = typeFE;

                    dicFacilityManagers[typeFE] = group;
                    dicFacilityManagers[IFacility.FacilityType.HD] = group;
                    dicFacilityManagers[IFacility.FacilityType.FA] = group;
                }
            }
            else if (IFacility.IsSecurityType(IFacility.ToFacilityType(nFacilityType)) == true)
            {
                IFacility.FacilityType type = IFacility.FacilityType.Security_Sensor;

                if (dicFacilityManagers.ContainsKey(type))
                    group = dicFacilityManagers[type];
                else
                {
                    group = new FacilityManagerGroup();
                    group.Type = type;
                    dicFacilityManagers[type] = group;
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
        public FacilityManagerGroup GetEntireFacilityManagerGroup(IFacility.FacilityType type, bool isDetectTime, bool alwaysGet = false)
        {
            Dictionary<IFacility.FacilityType, FacilityManagerGroup> dicEntireFacilityManager = isDetectTime ? m_dicEntireFacilityManagers : m_dicEntireFacilityManagersReport;

            if (dicEntireFacilityManager.ContainsKey(type))
                return dicEntireFacilityManager[type];

            if (alwaysGet)
            {
                FacilityManagerGroup group = new FacilityManagerGroup();
                group.Type = type;

                if (type == IFacility.FacilityType.FIRE_SENSOR ||
                    type == IFacility.FacilityType.COOLER_SENSOR ||
                    type == IFacility.FacilityType.PRESSURE_SENSOR)
                {
                    dicEntireFacilityManager[IFacility.FacilityType.FIRE_SENSOR] = group;
                    dicEntireFacilityManager[IFacility.FacilityType.COOLER_SENSOR] = group;
                    dicEntireFacilityManager[IFacility.FacilityType.PRESSURE_SENSOR] = group;
                }
                else if (type == IFacility.FacilityType.FE ||
                    type == IFacility.FacilityType.HD ||
                    type == IFacility.FacilityType.FA)
                {
                    dicEntireFacilityManager[IFacility.FacilityType.FE] = group;
                    dicEntireFacilityManager[IFacility.FacilityType.HD] = group;
                    dicEntireFacilityManager[IFacility.FacilityType.FA] = group;
                }
                else
                    dicEntireFacilityManager[type] = group;

                return group;
            }

            return null;
        }

        // EquipZone별 시설물 담당자 얻어오기
        public FacilityManagerGroup GetEquipZoneFacilityManagerGroup(IFacility.FacilityType type, EquipmentZone zone, bool isDetectTime, bool alwaysGet = false)
        {
            if (zone == null)
                return null;

            Dictionary<IFacility.FacilityType, Dictionary<int, FacilityManagerGroup>> dicEquipZoneFacilityManager = isDetectTime ? m_dicEquipZoneFacilityManager : m_dicEquipZoneFacilityManagerReport;

            if (dicEquipZoneFacilityManager.ContainsKey(type))
            {
                Dictionary<int, FacilityManagerGroup> dicManagers = dicEquipZoneFacilityManager[type];

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

                if (type == IFacility.FacilityType.FIRE_SENSOR ||
                    type == IFacility.FacilityType.COOLER_SENSOR ||
                    type == IFacility.FacilityType.PRESSURE_SENSOR)
                {
                    dicEquipZoneFacilityManager[IFacility.FacilityType.FIRE_SENSOR] = dicManagers;
                    dicEquipZoneFacilityManager[IFacility.FacilityType.COOLER_SENSOR] = dicManagers;
                    dicEquipZoneFacilityManager[IFacility.FacilityType.PRESSURE_SENSOR] = dicManagers;
                }
                else if (type == IFacility.FacilityType.FE ||
                    type == IFacility.FacilityType.HD ||
                    type == IFacility.FacilityType.FA)
                {
                    dicEquipZoneFacilityManager[IFacility.FacilityType.FE] = dicManagers;
                    dicEquipZoneFacilityManager[IFacility.FacilityType.HD] = dicManagers;
                    dicEquipZoneFacilityManager[IFacility.FacilityType.FA] = dicManagers;
                }
                else
                    dicEquipZoneFacilityManager[type] = dicManagers;

                FacilityManagerGroup group = new FacilityManagerGroup();
                group.Type = type;
                group.EquipZone = zone;

                dicManagers[zone.ID] = group;
                return group;
            }

            return null;
        }

        // 건물별 시설물 담당자 얻어오기
        public FacilityManagerGroup GetBuildingFacilityManagerGroup(IFacility.FacilityType type, Building building, bool isDetectTime, bool alwaysGet = false)
        {
            if (building == null)
                return null;

            Dictionary<IFacility.FacilityType, Dictionary<Building, FacilityManagerGroup>> dicBuildingFacilityManager = isDetectTime ? m_dicBuildingFacilityManager : m_dicBuildingFacilityManagerReport;

            if (dicBuildingFacilityManager.ContainsKey(type))
            {
                Dictionary<Building, FacilityManagerGroup> dicManagers = dicBuildingFacilityManager[type];

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

                if (type == IFacility.FacilityType.FIRE_SENSOR ||
                    type == IFacility.FacilityType.COOLER_SENSOR ||
                    type == IFacility.FacilityType.PRESSURE_SENSOR)
                {
                    dicBuildingFacilityManager[IFacility.FacilityType.FIRE_SENSOR] = dicManagers;
                    dicBuildingFacilityManager[IFacility.FacilityType.COOLER_SENSOR] = dicManagers;
                    dicBuildingFacilityManager[IFacility.FacilityType.PRESSURE_SENSOR] = dicManagers;
                }
                else if (type == IFacility.FacilityType.FE ||
                    type == IFacility.FacilityType.HD ||
                    type == IFacility.FacilityType.FA)
                {
                    dicBuildingFacilityManager[IFacility.FacilityType.FE] = dicManagers;
                    dicBuildingFacilityManager[IFacility.FacilityType.HD] = dicManagers;
                    dicBuildingFacilityManager[IFacility.FacilityType.FA] = dicManagers;
                }
                else
                    dicBuildingFacilityManager[type] = dicManagers;

                FacilityManagerGroup group = new FacilityManagerGroup();
                group.Type = type;
                group.Building = building;

                dicManagers[building] = group;
                return group;
            }

            return null;
        }

        // 외부 영역별 시설물 담당자 얻어오기
        public FacilityManagerGroup GetOutdoorFacilityManagerGroup(IFacility.FacilityType type, Zone zone, bool isDetectTime, bool alwaysGet = false)
        {
            Dictionary<IFacility.FacilityType, Dictionary<Zone, FacilityManagerGroup>> dicOutdoorFacilityManager = isDetectTime ? m_dicOutdoorFacilityManager : m_dicOutdoorFacilityManagerReport;

            if (dicOutdoorFacilityManager.ContainsKey(type))
            {
                Dictionary<Zone, FacilityManagerGroup> dicManagers = dicOutdoorFacilityManager[type];

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

                if (type == IFacility.FacilityType.FIRE_SENSOR ||
                    type == IFacility.FacilityType.COOLER_SENSOR ||
                    type == IFacility.FacilityType.PRESSURE_SENSOR)
                {
                    dicOutdoorFacilityManager[IFacility.FacilityType.FIRE_SENSOR] = dicManagers;
                    dicOutdoorFacilityManager[IFacility.FacilityType.COOLER_SENSOR] = dicManagers;
                    dicOutdoorFacilityManager[IFacility.FacilityType.PRESSURE_SENSOR] = dicManagers;
                }
                else if (type == IFacility.FacilityType.FE ||
                    type == IFacility.FacilityType.HD ||
                    type == IFacility.FacilityType.FA)
                {
                    dicOutdoorFacilityManager[IFacility.FacilityType.FE] = dicManagers;
                    dicOutdoorFacilityManager[IFacility.FacilityType.HD] = dicManagers;
                    dicOutdoorFacilityManager[IFacility.FacilityType.FA] = dicManagers;
                }
                else
                    dicOutdoorFacilityManager[type] = dicManagers;

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

            if (group.ControlRoomMembers.Count > 0)
            {
                FacilityManager mgr = (FacilityManager)group.ControlRoomMembers[0];
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

                    //DataTeam team = member.GetFirstTeam();
                    DataTeam team = member.Team;

                    if (team == null)
                        return member.Name;

                    return team.CompanyName + " " + team.TeamName + " " + member.Name;
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
            else if (mgr.MemberType == 7)
            {
                DataTeamControlRoom team = (DataTeamControlRoom)mgr.Tag;
                ArrayList controlRoomMembers = GetControlRoomMembers(team, 1);

                if (controlRoomMembers != null && controlRoomMembers.Count > 0)
                {
                    if (controlRoomMembers[0] is DataCompanyMember)
                    {
                        DataCompanyMember member = (DataCompanyMember)controlRoomMembers[0];
                        strPhoneNumber = member.PhoneNumber;
                        return member.MemberName;
                    }
                    else if (controlRoomMembers[0] is DataExternalMember)
                    {
                        DataExternalMember member = (DataExternalMember)controlRoomMembers[0];
                        strPhoneNumber = member.PhoneNumber;
                        return member.Team.CompanyName + " " + member.Team.TeamName + " " + member.Name;
                    }
                }
            }

            return "";
        }

        private ArrayList GetControlRoomMembers(DataTeamControlRoom team, int nCount = -1)
        {
            int nRoomID = team.ControlRoomID;
            int nPositionID = team.ControlTeamJobPositionID;
            string strSQL = "";

            if (nRoomID == 0)
            {
                strSQL = "select ctm.MemberType, ctm.MemberID ";
                strSQL += "from ControlRoom as cr, ControlWorkingTeam as cwt, ControlTeamMembers as ctm ";
                strSQL += "where cr.ID = cwt.RoomID and ctm.RoomID = cr.ID and ctm.TeamID = cwt.TeamID and ctm.MemberID is not NULL";
            }
            else if (nPositionID == 0)
            {
                strSQL = "select ctm.MemberType, ctm.MemberID ";
                //strSQL = "select ctm.ID, ctm.RoomID, ctm.TeamID, ctm.JobPosition, ctm.MemberType, ctm.MemberID ";
                strSQL += "from ControlRoom as cr, ControlWorkingTeam as cwt, ControlTeamMembers as ctm ";
                strSQL += string.Format("where cr.ID = cwt.RoomID and ctm.RoomID = cr.ID and ctm.TeamID = cwt.TeamID and cr.ID = {0} and ctm.MemberID is not NULL", nRoomID);
            }
            else
            {
                strSQL = "select ctm.MemberType, ctm.MemberID ";
                strSQL += "from ControlRoom as cr, ControlWorkingTeam as cwt, ControlTeamMembers as ctm ";
                strSQL += "where cr.ID = cwt.RoomID and ctm.RoomID = cr.ID and ctm.TeamID = cwt.TeamID and ";
                strSQL += string.Format("ctm.JobPosition = {0} and cr.ID = {1} and ctm.MemberID is not NULL", nPositionID, nRoomID);
            }

            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            ArrayList arrMembers = new ArrayList();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nMemberType = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nMemberID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);

                object member = null;

                if (nMemberType == 1)
                    member = GetCompanyMember(nMemberID);
                else if (nMemberType == 4)
                    member = GetExternalMember(nMemberID);

                if (member != null)
                {
                    arrMembers.Add(member);

                    if (nCount > 0 && arrMembers.Count >= nCount)
                        break;
                }
            }

            return arrMembers;
        }

        #region Disaster Prevention Equipment 방재장비

        public void ClearDisasterPreventionEquipment()
        {
            m_dicDisasterPreventionEquipmentType.Clear();
            m_dicDisasterPreventionEquipmentLocation.Clear();
            m_dicDisasterPreventionEquipment.Clear();
        }

        public void LoadDisasterPreventionEquipment()
        {
            ClearDisasterPreventionEquipment();
            
            // 방재장비 유형 정보 로드
            LoadDisasterPreventionEquipmentType();
            // 방재장비 위치 정보 로드
            LoadDisasterPreventionEquipmentLocation();

            string strSQL = "SELECT ID, TypeID, LocationID, Name, Quantity, Description FROM DisasterPreventionEquipment";

            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);
            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 5; i += 6)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nTypeID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nLocationID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                string strName = WebDBManager.GetStringField(arrResult[i + 3].ToString(), "");
                int nQuantity = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                string strDescription = WebDBManager.GetStringField(arrResult[i + 5].ToString(), "");

                if (nID < 0)
                    continue;

                if (strDescription == "null")
                    strDescription = "";

                if (m_dicDisasterPreventionEquipment.ContainsKey(nID) == true)
                    continue;

                DisasterPreventionEquipment item = new DisasterPreventionEquipment();
                item.ID = nID;
                item.Name = strName;
                item.Quantity = nQuantity;
                item.Description = strDescription;
                item.Index = m_dicDisasterPreventionEquipment.Count + 1;

                if (m_dicDisasterPreventionEquipmentType.ContainsKey(nTypeID) == true)
                    item.Type = m_dicDisasterPreventionEquipmentType[nTypeID];

                if (m_dicDisasterPreventionEquipmentLocation.ContainsKey(nLocationID) == true)
                    item.Location = m_dicDisasterPreventionEquipmentLocation[nLocationID];

                m_dicDisasterPreventionEquipment.Add(nID, item);
            }
        }

        private void LoadDisasterPreventionEquipmentType()
        {
            string strSQL = "SELECT ID, Name FROM DisasterPreventionEquipmentType";

            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);
            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strName = WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");

                if (nID < 0)
                    continue;

                if (m_dicDisasterPreventionEquipmentType.ContainsKey(nID) == true)
                    continue;

                DisasterPreventionEquipmentType item = new DisasterPreventionEquipmentType();
                item.ID = nID;
                item.Name = strName;
                item.Index = m_dicDisasterPreventionEquipmentType.Count + 1;

                m_dicDisasterPreventionEquipmentType.Add(nID, item);
            }
        }

        private void LoadDisasterPreventionEquipmentLocation()
        {
            string strSQL = "SELECT ID, LocationName FROM DisasterPreventionEquipmentLocation";

            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);
            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strLocationName = WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");

                if (nID < 0)
                    continue;

                if (m_dicDisasterPreventionEquipmentLocation.ContainsKey(nID) == true)
                    continue;


                DisasterPreventionEquipmentLocation item = new DisasterPreventionEquipmentLocation();
                item.ID = nID;
                item.Name = strLocationName;
                item.Index = m_dicDisasterPreventionEquipmentLocation.Count + 1;

                m_dicDisasterPreventionEquipmentLocation.Add(nID, item);
            }
        }

        public Dictionary<int, DisasterPreventionEquipment> GetDisasterPreventionEquipment()
        {
            return m_dicDisasterPreventionEquipment;
        }

        public Dictionary<int, DisasterPreventionEquipmentType> GetDisasterPreventionEquipmentType()
        {
            return m_dicDisasterPreventionEquipmentType;
        }

        public Dictionary<int, DisasterPreventionEquipmentLocation> GetDisasterRreventionEquipmentLocation()
        {
            return m_dicDisasterPreventionEquipmentLocation;
        }

        public DisasterPreventionEquipment AddDisasterPreventionEquipment()
        {
            DisasterPreventionEquipment newEquip = new DisasterPreventionEquipment();

            foreach (int nID in from nIDs in m_dicDisasterPreventionEquipment.Keys.Cast<int>()
                                orderby nIDs ascending
                                select nIDs
                               )
            {
                if (nID > 0)
                    newEquip.ID = -1;
                else
                    newEquip.ID = nID - 1;

                break;
            }

            newEquip.Index = m_dicDisasterPreventionEquipment.Count + 1;
            newEquip.Status = DisasterPreventionEquipment.STATUS.NEW;

            m_dicDisasterPreventionEquipment.Add(newEquip.ID, newEquip);

            return newEquip;
        }

        public DisasterPreventionEquipmentType AddDisasterPreventionEquipmentType(string strDisasterPreventionEquipmentType)
        {
            DisasterPreventionEquipmentType newType = new DisasterPreventionEquipmentType();

            foreach (int nID in from nIDs in m_dicDisasterPreventionEquipmentType.Keys.Cast<int>()
                                orderby nIDs ascending
                                select nIDs
                               )
            {
                if (nID > 0)
                    newType.ID = -1;
                else
                    newType.ID = nID - 1;

                break;
            }

            newType.Name = strDisasterPreventionEquipmentType;
            newType.Index = m_dicDisasterPreventionEquipmentType.Count + 1;

            m_dicDisasterPreventionEquipmentType.Add(newType.ID, newType);

            return newType;
        }

        public DisasterPreventionEquipmentLocation AddDisasterPreventionEquipmentLocation(string strDisasterPreventionEquipmentLocation)
        {
            DisasterPreventionEquipmentLocation newLocation = new DisasterPreventionEquipmentLocation();

            foreach (int nID in from nIDs in m_dicDisasterPreventionEquipmentLocation.Keys.Cast<int>()
                                orderby nIDs ascending
                                select nIDs
                               )
            {
                if (nID > 0)
                    newLocation.ID = -1;
                else
                    newLocation.ID = nID - 1;

                break;
            }

            newLocation.Name = strDisasterPreventionEquipmentLocation;
            newLocation.Index = m_dicDisasterPreventionEquipmentLocation.Count + 1;

            m_dicDisasterPreventionEquipmentLocation.Add(newLocation.ID, newLocation);

            return newLocation;
        }

        private bool ValildDisasterPreventionEquipmentData()
        {
            bool isValid = true;

            foreach (DisasterPreventionEquipment disasterPreventionEquipment in from disasterPreventionEquipments in m_dicDisasterPreventionEquipment.Values.Cast<DisasterPreventionEquipment>()
                                                                                where disasterPreventionEquipments.Status != DisasterPreventionEquipment.STATUS.DEL
                                                                                select disasterPreventionEquipments
                                                                                 )
            {
                if (disasterPreventionEquipment.Type == null)
                {
                    System.Windows.Forms.MessageBox.Show("유형을 선택하세요.");
                    isValid = false;
                    break;
                }
                else if (String.IsNullOrWhiteSpace(disasterPreventionEquipment.Name) == true)
                {
                    System.Windows.Forms.MessageBox.Show("장비이름을 입력하세요.");
                    isValid = false;
                    break;
                }
                else if (disasterPreventionEquipment.Location == null)
                {
                    System.Windows.Forms.MessageBox.Show("위치를 선택하세요.");
                    isValid = false;
                    break;
                }

            }

            return isValid;
        }

        public bool SaveDisasterPreventionEquipment()
        {
            bool isSave = false;

            if (ValildDisasterPreventionEquipmentData() == false)
                return isSave;

            List<DisasterPreventionEquipment> liEquipment = new List<DisasterPreventionEquipment>();
            foreach (DisasterPreventionEquipment disasterPreventionEquipment in from disasterPreventionEquipments in this.m_dicDisasterPreventionEquipment.Values.Cast<DisasterPreventionEquipment>()
                                                                                where disasterPreventionEquipments.Status != DisasterPreventionEquipment.STATUS.NON
                                                                                select disasterPreventionEquipments
                                                                               )
            {
                liEquipment.Add(disasterPreventionEquipment);
            }

            string strInsertSQL = "INSERT INTO DisasterPreventionEquipment (ID, TypeID, LocationID, Name, Quantity, Description) VALUES({0}, {1}, {2}, '{3}', {4}, '{5}') ";
            string strUpdateSQL = "UPDATE DisasterPreventionEquipment SET TypeID = {1}, LocationID = {2}, Name = '{3}', Quantity = {4}, Description = '{5}' WHERE ID = {0} ";
            string strDeleteSQL = "DELETE FROM DisasterPreventionEquipment WHERE ID = {0} ";

            string strInsertTypeSQL = "INSERT INTO DisasterPreventionEquipmentType (ID, Name) VALUES({0}, '{1}') ";
            string strInsertLocationSQL = "INSERT INTO DisasterPreventionEquipmentLocation (ID, LocationName) VALUES({0}, '{1}') ";

            string strGetIDSQL = "SELECT MAX(ID) FROM DisasterPreventionEquipment ";
            string strGetTypeIDSQL = "SELECT MAX(ID) FROM DisasterPreventionEquipmentType ";
            string strGetLocationIDSQL = "SELECT MAX(ID) FROM DisasterPreventionEquipmentLocation ";

            ArrayList arrResult = null;
            WebDBManager dbMgr = FormMain.Instance.DBManager;
            foreach (DisasterPreventionEquipment disasterPreventionEquipment in liEquipment)
            {
                if (disasterPreventionEquipment.Type == null || disasterPreventionEquipment.Location == null)
                    continue;

                if (disasterPreventionEquipment.Type.ID < 0)
                {
                    bool isOK = false;

                    foreach (DisasterPreventionEquipmentType orignType in m_dicDisasterPreventionEquipmentType.Values)
                    {
                        if (orignType.Name == disasterPreventionEquipment.Type.Name)
                        {
                            if (orignType.ID < 0)
                                continue;

                            disasterPreventionEquipment.Type = orignType;
                            isOK = true;
                            break;
                        }
                    }

                    if (isOK == false)
                    {
                        arrResult = dbMgr.GetResultData(strGetTypeIDSQL);

                        int nTypeID = -1;
                        if (arrResult == null)
                            return isSave;

                        nTypeID = WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;

                        if (dbMgr.GetResultData(String.Format(strInsertTypeSQL, nTypeID, disasterPreventionEquipment.Type.Name)) != null)
                        {
                            DisasterPreventionEquipmentType newType = new DisasterPreventionEquipmentType();
                            newType.ID = nTypeID;
                            newType.Name = disasterPreventionEquipment.Type.Name;
                            newType.Index = disasterPreventionEquipment.Type.Index;

                            m_dicDisasterPreventionEquipmentType.Remove(disasterPreventionEquipment.Type.ID);
                            m_dicDisasterPreventionEquipmentType.Add(nTypeID, newType);

                            disasterPreventionEquipment.Type = newType;
                        }
                        else
                            return isSave;
                    }

                }

                if (disasterPreventionEquipment.Location.ID < 0)
                {
                    bool isOK = false;

                    foreach (DisasterPreventionEquipmentLocation orignLocation in m_dicDisasterPreventionEquipmentLocation.Values)
                    {
                        if (orignLocation.Name == disasterPreventionEquipment.Location.Name)
                        {
                            if (orignLocation.ID < 0)
                                continue;

                            disasterPreventionEquipment.Location = orignLocation;
                            isOK = true;
                            break;
                        }
                    }

                    if (isOK == false)
                    {
                        arrResult = dbMgr.GetResultData(strGetLocationIDSQL);

                        int nLocationID = -1;
                        if (arrResult == null)
                            return isSave;

                        nLocationID = WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;

                        if (dbMgr.GetResultData(String.Format(strInsertLocationSQL, nLocationID, disasterPreventionEquipment.Location.Name)) != null)
                        {
                            DisasterPreventionEquipmentLocation newLocation = new DisasterPreventionEquipmentLocation();
                            newLocation.ID = nLocationID;
                            newLocation.Name = disasterPreventionEquipment.Location.Name;
                            newLocation.Index = disasterPreventionEquipment.Location.Index;

                            m_dicDisasterPreventionEquipmentLocation.Remove(disasterPreventionEquipment.Location.ID);
                            m_dicDisasterPreventionEquipmentLocation.Add(nLocationID, newLocation);

                            disasterPreventionEquipment.Location = newLocation;
                        }
                        else
                            return isSave;
                    }

                }

                switch (disasterPreventionEquipment.Status)
                {
                    case DisasterPreventionEquipment.STATUS.NEW:

                        arrResult = dbMgr.GetResultData(strGetIDSQL);

                        int nID = -1;
                        if (arrResult == null)
                            return isSave;

                        nID = WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;

                        if (dbMgr.GetResultData(
                            String.Format(strInsertSQL,
                            nID,
                            disasterPreventionEquipment.Type.ID,
                            disasterPreventionEquipment.Location.ID,
                            disasterPreventionEquipment.Name,
                            disasterPreventionEquipment.Quantity,
                            disasterPreventionEquipment.Description
                            )) != null)
                        {
                            m_dicDisasterPreventionEquipment.Remove(disasterPreventionEquipment.ID);
                            m_dicDisasterPreventionEquipment.Add(nID, disasterPreventionEquipment);
                            disasterPreventionEquipment.ID = nID;
                        }
                        else
                            return isSave;

                        break;

                    case DisasterPreventionEquipment.STATUS.UPD:

                        if (dbMgr.GetResultData(
                            String.Format(strUpdateSQL,
                            disasterPreventionEquipment.ID,
                            disasterPreventionEquipment.Type.ID,
                            disasterPreventionEquipment.Location.ID,
                            disasterPreventionEquipment.Name,
                            disasterPreventionEquipment.Quantity,
                            disasterPreventionEquipment.Description
                            )) == null)
                        {
                            return isSave;
                        }

                        break;

                    case DisasterPreventionEquipment.STATUS.DEL:

                        if (dbMgr.GetResultData(String.Format(strDeleteSQL, disasterPreventionEquipment.ID)) != null)
                        {
                            m_dicDisasterPreventionEquipment.Remove(disasterPreventionEquipment.ID);
                        }
                        else
                        {
                            return isSave;
                        }

                        break;
                }

                disasterPreventionEquipment.Status = DisasterPreventionEquipment.STATUS.NON;
            }

            isSave = true;

            return isSave;
        }

        #endregion Disaster Prevention Equipment 방재장비
        
        public string GetEditPassword()
        {
            string szText = "Select Password from SDMSEditPassword WHERE SiteID = {0}";
            string szSQL = string.Format(szText, m_nSiteID);

            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(szSQL);

            if (arrResult != null && arrResult.Count > 0)
            {
                string strPassword = AES256Cipher.AES_decrypt(arrResult[0].ToString(), key);
                return strPassword;
            }

            return null;
        }

        public void ChangePOIIcon(POI poi, string strPOIType)
        {
            m_ContentView.ChangePOIIcon(poi, strPOIType);
        }

        public void ReUpdateFacilityManager()
        {
            if (RemoveFacilityManager() == false || RemoveBuildingFacilityManager() == false || RemoveEquipZoneFacilityManager() == false)
                return;

            m_nFacility = 1;
            m_nBuilding = 1;
            m_nEquipZone = 1;

            ConversionTypeManagerUpdate();
        }
       
        private List<int> GetListEquipZone()
        {
            List<int> listEquipZone = new List<int>();

            string szText = "select ID from EquipmentZone WHERE SiteID = {0}";
            string szSQL = string.Format(szText, m_nSiteID);

            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(szSQL);

            if (arrResult != null)
            {
                int nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount; i++)
                {
                    int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    listEquipZone.Add(nID);
                }
            }


            return listEquipZone;
        }

        private bool RemoveFacilityManager()
        {
            bool bRet = true;

            string szText = "delete from FacilityManager WHERE SiteID = {0}";
            string szSQL = string.Format(szText, m_nSiteID);

            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(szSQL);
            if (arrResult == null)
                bRet = false;

            return bRet;
        }

        private bool RemoveBuildingFacilityManager()
        {
            bool bRet = true;

            string szText = "delete from BuildingFacilityManager WHERE SiteID = {0}";
            string szSQL = string.Format(szText, m_nSiteID);

            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(szSQL);
            if (arrResult == null)
                bRet = false;

            return bRet;
        }

        private bool RemoveEquipZoneFacilityManager()
        {
            bool bRet = true;

            string szText = "delete from EquipZoneFacilityManager WHERE SiteID = {0}";
            string szSQL = string.Format(szText, m_nSiteID);

            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(szSQL);
            if (arrResult == null)
                bRet = false;

            return bRet;
        }


       

        private void ConversionTypeManagerUpdate()
        {

            if (m_dicEntireFacilityManagers.ContainsKey(IFacility.FacilityType.FIRE_SENSOR))
                    ConversionTeamManagerUpdate(m_dicEntireFacilityManagers[IFacility.FacilityType.FIRE_SENSOR], FacilityType.TYPE);
            if (m_dicBuildingFacilityManager.ContainsKey(IFacility.FacilityType.FIRE_SENSOR))
                    ConversionBuildingManagerUpdate(m_dicBuildingFacilityManager[IFacility.FacilityType.FIRE_SENSOR], FacilityType.BUILDING);
            if (m_dicEquipZoneFacilityManager.ContainsKey(IFacility.FacilityType.FIRE_SENSOR))
                    ConversionEquipZoneManagerUpdate(m_dicEquipZoneFacilityManager[IFacility.FacilityType.FIRE_SENSOR], FacilityType.FLOOR); 

            if (UnE.SOP.ProxySOP.Instance.UsePSM == true)
            {
                if (m_dicEntireFacilityManagers.ContainsKey(IFacility.FacilityType.PSM_SENSOR))
                    ConversionTeamManagerUpdate(m_dicEntireFacilityManagers[IFacility.FacilityType.PSM_SENSOR], FacilityType.TYPE);
                if (m_dicBuildingFacilityManager.ContainsKey(IFacility.FacilityType.PSM_SENSOR))
                    ConversionBuildingManagerUpdate(m_dicBuildingFacilityManager[IFacility.FacilityType.PSM_SENSOR], FacilityType.BUILDING);
                if (m_dicEquipZoneFacilityManager.ContainsKey(IFacility.FacilityType.PSM_SENSOR))
                    ConversionEquipZoneManagerUpdate(m_dicEquipZoneFacilityManager[IFacility.FacilityType.PSM_SENSOR], FacilityType.FLOOR);
            }

            if (UnE.SOP.ProxySOP.Instance.UseStrongWind == true)
            {
                if (m_dicEntireFacilityManagers.ContainsKey(IFacility.FacilityType.STRONG_WIND))
                    ConversionTeamManagerUpdate(m_dicEntireFacilityManagers[IFacility.FacilityType.STRONG_WIND], FacilityType.TYPE);
                if (m_dicBuildingFacilityManager.ContainsKey(IFacility.FacilityType.STRONG_WIND))
                    ConversionBuildingManagerUpdate(m_dicBuildingFacilityManager[IFacility.FacilityType.STRONG_WIND], FacilityType.BUILDING);
                if (m_dicEquipZoneFacilityManager.ContainsKey(IFacility.FacilityType.STRONG_WIND))
                    ConversionEquipZoneManagerUpdate(m_dicEquipZoneFacilityManager[IFacility.FacilityType.STRONG_WIND], FacilityType.FLOOR);
            }

            if (UnE.SOP.ProxySOP.Instance.UseEarthquake == true)
            {
                if (m_dicEntireFacilityManagers.ContainsKey(IFacility.FacilityType.Earthquake))
                    ConversionTeamManagerUpdate(m_dicEntireFacilityManagers[IFacility.FacilityType.Earthquake], FacilityType.TYPE);
                if (m_dicBuildingFacilityManager.ContainsKey(IFacility.FacilityType.Earthquake))
                    ConversionBuildingManagerUpdate(m_dicBuildingFacilityManager[IFacility.FacilityType.Earthquake], FacilityType.BUILDING);
                if (m_dicEquipZoneFacilityManager.ContainsKey(IFacility.FacilityType.Earthquake))
                    ConversionEquipZoneManagerUpdate(m_dicEquipZoneFacilityManager[IFacility.FacilityType.Earthquake], FacilityType.FLOOR);
            }

            if (UnE.SOP.ProxySOP.Instance.UseFirewall == true)
            {
                if (m_dicEntireFacilityManagers.ContainsKey(IFacility.FacilityType.FIREWALL))
                    ConversionTeamManagerUpdate(m_dicEntireFacilityManagers[IFacility.FacilityType.FIREWALL], FacilityType.TYPE);
                if (m_dicBuildingFacilityManager.ContainsKey(IFacility.FacilityType.FIREWALL))
                    ConversionBuildingManagerUpdate(m_dicBuildingFacilityManager[IFacility.FacilityType.FIREWALL], FacilityType.BUILDING);
                if (m_dicEquipZoneFacilityManager.ContainsKey(IFacility.FacilityType.FIREWALL))
                    ConversionEquipZoneManagerUpdate(m_dicEquipZoneFacilityManager[IFacility.FacilityType.FIREWALL], FacilityType.FLOOR);
            }

            if (UnE.SOP.ProxySOP.Instance.UseDoor == true)
            {
                if (m_dicEntireFacilityManagers.ContainsKey(IFacility.FacilityType.DOOR))
                    ConversionTeamManagerUpdate(m_dicEntireFacilityManagers[IFacility.FacilityType.DOOR], FacilityType.TYPE);
                if (m_dicBuildingFacilityManager.ContainsKey(IFacility.FacilityType.DOOR))
                    ConversionBuildingManagerUpdate(m_dicBuildingFacilityManager[IFacility.FacilityType.DOOR], FacilityType.BUILDING);
                if (m_dicEquipZoneFacilityManager.ContainsKey(IFacility.FacilityType.DOOR))
                    ConversionEquipZoneManagerUpdate(m_dicEquipZoneFacilityManager[IFacility.FacilityType.DOOR], FacilityType.FLOOR);
            }

            if (UnE.SOP.ProxySOP.Instance.UseBlackout == true)
            {
               if (m_dicEntireFacilityManagers.ContainsKey(IFacility.FacilityType.BLACKOUT))
                    ConversionTeamManagerUpdate(m_dicEntireFacilityManagers[IFacility.FacilityType.BLACKOUT], FacilityType.TYPE);
                if (m_dicBuildingFacilityManager.ContainsKey(IFacility.FacilityType.BLACKOUT))
                    ConversionBuildingManagerUpdate(m_dicBuildingFacilityManager[IFacility.FacilityType.BLACKOUT], FacilityType.BUILDING);
                if (m_dicEquipZoneFacilityManager.ContainsKey(IFacility.FacilityType.BLACKOUT))
                    ConversionEquipZoneManagerUpdate(m_dicEquipZoneFacilityManager[IFacility.FacilityType.BLACKOUT], FacilityType.FLOOR);
            }
        }




        private void ConversionBuildingManagerUpdate(Dictionary<Building, FacilityManagerGroup> dicManagers, FacilityType type)
        {
            // 빌딩 리스트를 구해 for문으로 그룹을 구한다.
            foreach (KeyValuePair<int, Building> item in UnE.Spatial.ZoneManager.Instance.DicBuildings)
            {
                if (dicManagers.ContainsKey(item.Value))
                {
                    FacilityManagerGroup group = dicManagers[item.Value];
                    ConversionTeamManagerUpdate(group, type);
                }
            }
        }

        private void ConversionEquipZoneManagerUpdate(Dictionary<int, FacilityManagerGroup> dicManagers, FacilityType type)
        {
            List<int> listEquipZone = new List<int>();
            listEquipZone = GetListEquipZone();

            foreach (int nEquipZoneID in listEquipZone)
            {
                if (dicManagers.ContainsKey(nEquipZoneID))
                {
                    FacilityManagerGroup group = dicManagers[nEquipZoneID];
                    ConversionTeamManagerUpdate(group, type);
                }
            }
        }


        private void ConversionTeamManagerUpdate(FacilityManagerGroup group, FacilityType type)
        {
            //FacilityManager mgr = (FacilityManager)group.CompanyMembers[0];
            foreach (FacilityManager mgr in group.CompanyMembers)
            {
                if (type == FacilityType.TYPE)
                    AddFacilityManager(mgr);
                else if (type == FacilityType.BUILDING)
                    AddBuildingFacilityManager(mgr);
                else if (type == FacilityType.FLOOR)
                    AddEquipZoneFacilityManager(mgr);
            }

            foreach (FacilityManager mgr in group.RegularTeams)
            {
                if (type == FacilityType.TYPE)
                    AddFacilityManager(mgr);
                else if (type == FacilityType.BUILDING)
                    AddBuildingFacilityManager(mgr);
                else if (type == FacilityType.FLOOR)
                    AddEquipZoneFacilityManager(mgr);
            }

            foreach (FacilityManager mgr in group.ExternalCompanyMembers)
            {
                if (type == FacilityType.TYPE)
                    AddFacilityManager(mgr);
                else if (type == FacilityType.BUILDING)
                    AddBuildingFacilityManager(mgr);
                else if (type == FacilityType.FLOOR)
                    AddEquipZoneFacilityManager(mgr);
            }

            foreach (FacilityManager mgr in group.ExternalTeams)
            {
                if (type == FacilityType.TYPE)
                    AddFacilityManager(mgr);
                else if (type == FacilityType.BUILDING)
                    AddBuildingFacilityManager(mgr);
                else if (type == FacilityType.FLOOR)
                    AddEquipZoneFacilityManager(mgr);
            }

            foreach (FacilityManager mgr in group.ControlRoomMembers)
            {
                if (type == FacilityType.TYPE)
                    AddFacilityManager(mgr);
                else if (type == FacilityType.BUILDING)
                    AddBuildingFacilityManager(mgr);
                else if (type == FacilityType.FLOOR)
                    AddEquipZoneFacilityManager(mgr);
            }
        }

        private bool AddFacilityManager(FacilityManager mgr)
        {
            bool bRet = true;

            int nID = m_nFacility;
            int nMemberID= mgr.MemberID;
            int nMemberType = mgr.MemberType;
            int nFacilityType = Convert.ToInt32(mgr.Type);
            int nLevelLimit = mgr.LevelLimit;
            int nUseUpperLevel = mgr.UpperLimit;
            string strDescription = mgr.Description;

            string szText = "INSERT INTO FacilityManager (ID, MemberID, MemberType, FacilityType, LevelLimit, UpperLimit, SiteID, Description) VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, '{7}') ";
            string szSQL = string.Format(szText, nID, nMemberID, nMemberType, nFacilityType, nLevelLimit, nUseUpperLevel, m_nSiteID, strDescription);

            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(szSQL);
            if (arrResult == null)
                bRet = false;

            m_nFacility++;

            return bRet;
        }

        private bool AddBuildingFacilityManager(FacilityManager mgr)
        {
            bool bRet = true;

            int nID = m_nBuilding;
            int nMemberID = mgr.MemberID;
            int nMemberType = mgr.MemberType;
            int nFacilityType = Convert.ToInt32(mgr.Type);
            int nLevelLimit = mgr.LevelLimit;
            int nUseUpperLevel = mgr.UpperLimit;
            string strDescription = mgr.Description;
            int nBuildingID = mgr.Building.ID;

            string szText = "INSERT INTO BuildingFacilityManager (ID, MemberID, MemberType, FacilityType, LevelLimit, BuildingID, UpperLimit, SiteID, Description) VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, '{8}') ";
            string szSQL = string.Format(szText, nID, nMemberID, nMemberType, nFacilityType, nLevelLimit, nBuildingID, nUseUpperLevel, m_nSiteID, strDescription);

            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(szSQL);
            if (arrResult == null)
                bRet = false;

            m_nBuilding++;

            return bRet;
        }

        private bool AddEquipZoneFacilityManager(FacilityManager mgr)
        {
            bool bRet = true;

            int nID = m_nEquipZone;
            int nMemberID = mgr.MemberID;
            int nMemberType = mgr.MemberType;
            int nFacilityType = Convert.ToInt32(mgr.Type);
            int nLevelLimit = mgr.LevelLimit;
            int nUseUpperLevel = mgr.UpperLimit;
            string strDescription = mgr.Description;
            int nEquipZoneID = mgr.EquipZone.ID;

            string szText = "INSERT INTO EquipZoneFacilityManager (ID, MemberID, MemberType, FacilityType, LevelLimit, EquipZoneID, UpperLimit, SiteID ,Description) VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, '{8}') ";
            string szSQL = string.Format(szText, nID, nMemberID, nMemberType, nFacilityType, nLevelLimit, nEquipZoneID, nUseUpperLevel, m_nSiteID, strDescription);

            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(szSQL);
            if (arrResult == null)
                bRet = false;

            m_nEquipZone++;

            return bRet;
        }
    }
}