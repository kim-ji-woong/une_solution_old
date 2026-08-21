using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility;
using UnE.Spatial;
using UnE.Sensor;
using System.Collections;
using System.Windows.Forms;

namespace AlarmButtonSimulator
{
    public class DataManager
    {
        private WebDBManager m_dbMgr = null;
        private int m_nSiteID = 100;

        private Dictionary<int, BuildingGroup> m_dicBuildingGroups = new Dictionary<int, BuildingGroup>();
        private Dictionary<int, Building> m_dicBuildings = new Dictionary<int, Building>();
        private Dictionary<int, Zone> m_dicZones = new Dictionary<int, Zone>();
        private Dictionary<int, EquipmentZone> m_dicEquipZones = new Dictionary<int, EquipmentZone>();
        private Dictionary<int, FireSensor> m_dicSensorZones = new Dictionary<int, FireSensor>();
        private Dictionary<int, Circuit> m_dicSensorTags = new Dictionary<int, Circuit>();

        private List<Circuit> m_listAddedSensorTags = new List<Circuit>();

        private DataTeam m_teamRegularRoot = null;
        private ArrayList m_listExternalRootTeams = new ArrayList();
        private Dictionary<int, DataTeam> m_dicRegularTeams = new Dictionary<int, DataTeam>();
        private Dictionary<DataTeam, ArrayList> m_dicRegularTeamMembers = new Dictionary<DataTeam, ArrayList>();
        private Dictionary<int, DataCompanyMember> m_dicRegularMembers = new Dictionary<int, DataCompanyMember>();
        private Dictionary<int, DataTeam> m_dicExternalTeams = new Dictionary<int, DataTeam>();
        private Dictionary<DataTeam, ArrayList> m_dicExternalTeamMembers = new Dictionary<DataTeam, ArrayList>();
        private Dictionary<int, DataExternalMember> m_dicExternalMembers = new Dictionary<int, DataExternalMember>();
        private Dictionary<int, DataTeamControlRoom> m_dicControlRoomTeams = new Dictionary<int, DataTeamControlRoom>();

        private static string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

        public DataTeam RegularTeamRoot
        {
            get { return m_teamRegularRoot; }
        }

        public ArrayList ExternalTeamRootList
        {
            get { return m_listExternalRootTeams; }
        }

        private static DataManager m_instance = null;

        public static DataManager Instance
        {
            get { return m_instance; }
        }

        public DataManager(WebDBManager dbMgr, int nSiteID)
        {
            m_instance = this;
            m_dbMgr = dbMgr;
            m_nSiteID = nSiteID;

            LoadDatas();
            LoadDataMembers();
        }

        public FireSensor GetSensorZone(int nSensorZoneID)
        {
            FireSensor sensorZone;

            if (m_dicSensorZones.TryGetValue(nSensorZoneID, out sensorZone))
                return sensorZone;

            return null;
        }

        public Zone GetZoneForSearch(int nSameNameSearchCnt, string strSearchZoneName)
        {
            Zone zone = null;

            int cnt = 1;

            foreach (Zone item in from items in m_dicZones.Values.AsEnumerable()
                                  where items.ZoneName.IndexOf(strSearchZoneName) != -1
                                  select items)
            {
                if (nSameNameSearchCnt == cnt++)
                {
                    zone = item;
                    break;
                }
            }

            return zone;
        }

        public Circuit GetSensorTagForSearch(int nSameNameSearchCnt, string strSearchSensorName)
        {
            Circuit sensor = null;

            int cnt = 1;

            foreach (Circuit item in from items in m_dicSensorTags.Values.AsEnumerable()
                                       where items.Name.IndexOf(strSearchSensorName) != -1
                                       select items)
            {
                if (nSameNameSearchCnt == cnt++)
                {
                    sensor = item;
                    break;
                }
            }

            return sensor;
        }

        private void LoadDataMembers()
        {
            m_teamRegularRoot = LoadRegularTeam(m_dicRegularTeams);
            m_listExternalRootTeams = LoadExternalTeam(m_dicExternalTeams);

            LoadCompanyMember(m_dicRegularTeams);
            LoadExternalMember(m_dicExternalTeams);
            LoadControlRoomTeams(m_dicControlRoomTeams);
        }

        public ArrayList ExecuteTeamList(int nRootTeamID, string strTableName = "RegularTeam")
        {
            string strSQL = "Select ID, TeamName, ParentTeamID from " + strTableName + " order by ParentTeamID, ID";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

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

        public bool LoadControlRoomTeams(Dictionary<int, DataTeamControlRoom> dicTeams)
        {
            dicTeams.Clear();

            string strSQL = "select cr.ID, cr.RoomType, cr.LocationName, crt.TypeName from ControlRoom as cr, ControlRoomType as crt ";
            strSQL += "where cr.RoomType = crt.ID and crt.SiteID = " + m_nSiteID.ToString() + " order by cr.RoomType";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

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
            arrResult = m_dbMgr.GetResultData(strSQL, 0);

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

        public bool LoadExternalMember(Dictionary<int, DataTeam> dicTeams)
        {
            m_dicExternalMembers.Clear();
            //string szSQL = "SELECT ID, Name, PhoneNumber, IsTeamLeader, TeamID FROM ExternalCompanyMember";


            StringBuilder sb1 = new StringBuilder();
            /*sb1.Append("SELECT ecm.ID, ecm.Name, ecm.PhoneNumber, ecm.IsTeamLeader, ecm.TeamID ");
            sb1.Append(" FROM ExternalCompanyMember as ecm ");
            sb1.Append(" INNER JOIN ExternalCompanyTeam as ect ON ecm.TeamID = ect.ID ");
            sb1.AppendFormat(" INNER JOIN ExternalTeam as et on ect.CompanyID = et.ID and et.SiteID = {0}", m_nSiteID);*/

            sb1.Append("Select eml.ExternalCompanyTeamID, eml.ExternalCompanyMemberID, ecm.Name, ecm.PhoneNumber ");
            sb1.Append("from ExternalCompanyMember as ecm, ExternalMemberList as eml, ExternalTeam as et ");
            sb1.AppendFormat("where eml.ExternalCompanyMemberID = ecm.ID and et.ID = eml.ExternalCompanyTeamID and et.SiteID = {0}", m_nSiteID);

            string szSQL = sb1.ToString();

            ArrayList arrResult = m_dbMgr.GetResultData(szSQL, 0);
            if (arrResult == null)
                return false;

            int nCount = arrResult.Count;
            if (nCount == 0)
                return true;

            DataExternalMember member;

            for (int i = 0; i < nCount - 3; i += 4)
            {
                int nTeamID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), 0);
                //bool nLeader = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0) == 1;
                string strMemberName = DBUtility.WebDBManager.GetStringField(arrResult[i + 2], "");
                string szPhoneNumber = DBUtility.WebDBManager.GetStringField(arrResult[i + 3].ToString(), "");

                if (!dicTeams.ContainsKey(nTeamID))
                    return false;

                DataTeam team = dicTeams[nTeamID];

                if (string.Compare(szPhoneNumber, "null", true) == 0 || szPhoneNumber == "")
                    szPhoneNumber = "";
                else
                    szPhoneNumber = DBUtility.AES256Cipher.AES_decrypt(szPhoneNumber, key);

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

                /*DataExternalMember data = new DataExternalMember();
                data.ID = nID;
                data.Name = strMemberName;
                data.PhoneNumber = szPhoneNumber;
                data.TeamLeader = nLeader;
                data.Team = team;*/

                ArrayList arrMembers = null;

                if (m_dicExternalTeamMembers.ContainsKey(team))
                    arrMembers = m_dicExternalTeamMembers[team];
                else
                {
                    arrMembers = new ArrayList();
                    m_dicExternalTeamMembers[team] = arrMembers;
                }

                //m_dicExternalMembers[nID] = data;
                arrMembers.Add(member);
            }

            return false;
        }

        public bool LoadCompanyMember(Dictionary<int, DataTeam> dicTeams)
        {
            m_dicRegularMembers.Clear();

            string strSQL = string.Format("SELECT TeamID FROM Site WHERE ID = {0}", m_nSiteID);
            ArrayList arrResult1 = m_dbMgr.GetResultData(strSQL, 0);
            if (arrResult1 == null || arrResult1.Count == 0)
                return false;

            int nTeamID = WebDBManager.GetIntField(arrResult1[0].ToString(), -1);
            if (nTeamID == -1)
                return false;

            ArrayList arrResult2 = ExecuteTeamList(nTeamID);
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
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null) return false;

            int nCount = arrResult.Count;
            if (nCount == 0) return true;

            DataCompanyMember member;

            for (int i = 0; i < nCount - 7; i += 8)
            {
                int nRegularTeamID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), 0);
                int nPositionID = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
                string strMemberName = DBUtility.WebDBManager.GetStringField(arrResult[i + 3], "");
                int nLevelID = DBUtility.WebDBManager.GetIntField(arrResult[i + 4].ToString(), 0);
                string strMemberID = DBUtility.WebDBManager.GetStringField(arrResult[i + 5], "");
                //int nSecondRegularTeamID = DBUtility.WebDBManager.GetIntField(arrResult[i + 6].ToString(), 0);
                //int nSecondPositionID = DBUtility.WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0);
                string strOfficePhoneNumber = DBUtility.WebDBManager.GetStringField(arrResult[i + 6], "");
                string strPhoneNumber = DBUtility.WebDBManager.GetStringField(arrResult[i + 7], "");

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

                /*DataCompanyMember data = new DataCompanyMember();
                data.ID = nID;
                data.MemberName = strMemberName;
                data.Team = team;
                data.LevelID = nLevelID;
                data.PositionID = nPositionID;
                data.MemberID = strMemberID;
                data.OfficePhoneNumber = strOfficePhoneNumber;
                data.PhoneNumber = strPhoneNumber;*/

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
                ////////////////////////////////////////////////////////////////
            }

            foreach (KeyValuePair<DataTeam, ArrayList> pair in m_dicRegularTeamMembers)
            {
                pair.Value.Sort();
            }

            return true;
        }

        // dicTeams : ID별 Team
        private ArrayList LoadExternalTeam(Dictionary<int, DataTeam> dicTeams)
        {
            dicTeams.Clear();

            ArrayList arrExternalRootTeams = new ArrayList();
            string szText2 = "SELECT et.ID, et.TeamName, et.ParentTeamID " +
                             " FROM ExternalTeam as et WHERE et.SiteID = {0} ";

            string szSQL = string.Format(szText2, m_nSiteID);

            ArrayList arrResult = m_dbMgr.GetResultData(szSQL, 0);
            if (arrResult == null)
                return null;

            // 자신의 Team, 부모 팀의 ID
            Dictionary<DataTeam, int> dicParentID = new Dictionary<DataTeam, int>();

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string szTeamName = DBUtility.WebDBManager.GetStringField(arrResult[i + 1], "");
                int nParentTeamID = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                //int nCompanyID = DBUtility.WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);


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
        private DataTeam LoadRegularTeam(Dictionary<int, DataTeam> dicTeams)
        {
            dicTeams.Clear();
            //string szSQL = "SELECT R.ID, R.TeamName, R.ParentTeamID FROM RegularTeam as R";

            string strSQL = string.Format("SELECT TeamID FROM Site WHERE ID = {0}", m_nSiteID);
            ArrayList arrResult1 = m_dbMgr.GetResultData(strSQL, 0);
            if (arrResult1 == null || arrResult1.Count == 0)
                return null;

            int nTeamID = WebDBManager.GetIntField(arrResult1[0].ToString(), -1);
            if (nTeamID == -1)
                return null;

            ArrayList arrResult = ExecuteTeamList(nTeamID);
            //strSQL = string.Format("sp_TeamList2 {0}", nTeamID);
            //ArrayList arrResult = dbMgr.GetStoredProcedureData(strSQL, 0);
            if (arrResult == null || arrResult.Count == 0)
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

        private bool LoadDatas()
        {
            if (LoadBuildingGroup())
            {
                if (LoadBuilding())
                {
                    if (LoadZone())
                    {
                        if (LoadEquipmentZone())
                        {
                            if (LoadSensorZone())
                            {
                                if (LoadSensorTag())
                                    return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private bool LoadSensorTag()
        {
            string strSQL = "select st.ID, st.SensorServerID, st.TagNo, st.SensorName, st.SensorType, st.SensorZoneID from SensorTagInfo as st, SensorServerInfo as ss where st.SensorServerID = ss.ID";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            FireSensor sensorZone;
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 5; i += 6)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nReceiverID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nTagID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                string strSensorName = WebDBManager.GetStringField(arrResult[i + 3], "null");
                int nSensorType = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                int nSensorZoneID = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);

                if (nID < 0 || nSensorZoneID < 0)
                    continue;

                //if (nReceiverID == 0)
                //   continue;

                if (!m_dicSensorZones.TryGetValue(nSensorZoneID, out sensorZone))
                    continue;

                Circuit tag = new Circuit();
                tag.ID = nID;
                tag.ReciverID = nReceiverID;
                tag.TagNum = nTagID;
                tag.Name = strSensorName;

                tag.SensorType = IFacility.ToFacilityType(nSensorType);

                tag.SensorZoneID = nSensorZoneID;

                m_dicSensorTags[nID] = tag;
            }

            return true;
        }

        private bool LoadSensorZone()
        {
            string strSQL = "select sz.ID, sz.EquipZoneID, sz.Data from SensorZone as sz, EquipmentZone as ez where EquipZoneID > 0 and EquipZoneID = ez.ID and ez.SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            EquipmentZone equipZone = null;
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nEquipZoneID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nData = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);

                if (nID < 0)
                    continue;

                if (!m_dicEquipZones.TryGetValue(nEquipZoneID, out equipZone))
                    continue;

                FireSensor sensorZone = new FireSensor();
                sensorZone.ID = nID;
                sensorZone.EquipZoneID = nEquipZoneID;
                sensorZone.SensorData = nData;

                m_dicSensorZones[nID] = sensorZone;
            }

            return true;
        }

        private bool LoadBuildingGroup()
        {
            string strSQL = "select ID, GroupName from BuildingGroup where SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strName = WebDBManager.GetStringField(arrResult[i + 1], "null");

                if (nID < 0 || strName == "null" || strName.Length == 0)
                    continue;

                BuildingGroup group = new BuildingGroup();
                group.GroupID = nID;
                group.BuildingGroupName = strName;

                m_dicBuildingGroups[nID] = group;
            }

            return true;
        }

        private bool LoadBuilding()
        {
            string strSQL = "select Building.ID, BuildingName, BuildingGroupID, BroadCastingText, Building.DisplayText from Building, BuildingGroup where Building.BuildingGroupID = BuildingGroup.ID and BuildingGroup.SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            BuildingGroup group;
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 4; i += 5)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nBuildingGroupID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                string strBroadcastingName = WebDBManager.GetStringField(arrResult[i + 3], "null");
                string strDisplayText = WebDBManager.GetStringField(arrResult[i + 4], "null");

                if (nID < 0 || nBuildingGroupID < 0 || (strBroadcastingName == "null" || strBroadcastingName.Length == 0) && (strDisplayText == "null" || strDisplayText.Length == 0))
                    continue;

                if (!m_dicBuildingGroups.TryGetValue(nBuildingGroupID, out group))
                    continue;

                Building building = new Building();
                building.ID = nID;
                building.BuildingName = strDisplayText == "null" || strDisplayText.Length == 0 ? strBroadcastingName : strDisplayText;
                building.BuildingGroup = group;

                m_dicBuildings[nID] = building;
            }

            return true;
        }

        private bool LoadZone()
        {
            string strSQL = "select ID, BuildingID, DisplayText from Zone where SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            Building building = null;
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nBuildingID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                string strBroadcastingName = WebDBManager.GetStringField(arrResult[i + 2], "null");

                if (nID <= 0 || strBroadcastingName == "null" || strBroadcastingName.Length == 0)
                    continue;

                if (nBuildingID < 0)
                    building = null;
                else
                {
                    if (!m_dicBuildings.TryGetValue(nBuildingID, out building))
                        continue;
                }

                Zone zone = new Zone();
                zone.ID = nID;
                zone.ZoneName = strBroadcastingName;
                zone.Building = building;
                //zone.IsOutdoor = building == null;

                m_dicZones[nID] = zone;
            }

            return true;
        }

        private bool LoadEquipmentZone()
        {
            string strSQL = "select ID, LinkedZoneIDList, BroadcastName from EquipmentZone where SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            Zone zone = null;
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strLinkedZoneIDs = WebDBManager.GetStringField(arrResult[i + 1], "null");
                string strBroadcastingName = WebDBManager.GetStringField(arrResult[i + 2], "null");

                if (nID <= 0 || strLinkedZoneIDs == "null" || strLinkedZoneIDs.Length == 0 || strBroadcastingName == "null" || strBroadcastingName.Length == 0)
                    continue;

                List<int> ids = GetIDList(strLinkedZoneIDs);

                if (ids == null)
                    return false;

                EquipmentZone equipZone = new EquipmentZone();

                equipZone.ID = nID;
                equipZone.ZoneName = strBroadcastingName;

                foreach (int nZoneID in ids)
                {
                    if (!m_dicZones.TryGetValue(nZoneID, out zone))
                        continue;
                    else
                        equipZone.LinkedZoneList.Add(zone);
                }

                m_dicEquipZones[nID] = equipZone;
            }

            return true;
        }

        private List<int> GetIDList(string strIDs)
        {
            List<int> ids = new List<int>();
            string[] arrTokens = strIDs.Split(',');

            int nID;

            foreach (string strToken in arrTokens)
            {
                if (int.TryParse(strToken.Trim(), out nID))
                    ids.Add(nID);
                else
                    return null;
            }

            return ids;
        }

        public List<Circuit> GetSensorTagByZone(Zone zone)
        {
            List<Circuit> listSensorTag = new List<Circuit>();

            foreach (EquipmentZone itemEquipmentZone in from itemEquipmentZones in m_dicEquipZones.Values.AsEnumerable()
                                                        where itemEquipmentZones.LinkedZoneList.Contains(zone)
                                                        select itemEquipmentZones
                                                        )
            {
                foreach (FireSensor itemSensorZone in from itemSensorZones in m_dicSensorZones.Values.AsEnumerable()
                                                      where itemSensorZones.EquipZoneID == itemEquipmentZone.ID
                                                      select itemSensorZones
                                                      )
                {
                    foreach (Circuit itemSensorTag in from itemSensorTags in m_dicSensorTags.Values.AsEnumerable()
                                                        where itemSensorTags.SensorZoneID == itemSensorZone.ID
                                                        select itemSensorTags
                                                       )
                    {
                        listSensorTag.Add(itemSensorTag);

                    }

                }

            }


            return listSensorTag;
        }

        public void MakeSensorTagTree(TreeView tree, string strSearchWord)
        {
            // changed by mwkim 2015-11-06. 검색할 단어로 존을 먼저 검색하고 난뒤, 센서를 검색함.
            // 1. Zone 검색
            // 2. SensorTag 검색

            // Z. 검색어가 없는경우에는 모든 센서를 나타낸다.

            // s1 용 센서테스터로 변경작업 2017-04-03 skkim
            // 센서 타입별로 선택가능하도록

            m_listAddedSensorTags.Clear();


            if (String.IsNullOrWhiteSpace(strSearchWord) == false)
            {
                // 이름이 일치하는 Zone을 먼저 찾고,
                // Zone에 매핑되는 SensorZone정보를 찾아 SensorTag를 알아내서 노드를 추가한다.
                foreach (KeyValuePair<int, Zone> pair in m_dicZones)
                {
                    if (pair.Value.ZoneName.IndexOf(strSearchWord, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        foreach (Circuit item in GetSensorTagByZone(pair.Value))
                        {
                            if (m_listAddedSensorTags.Contains(item) == false)
                            {
                                m_listAddedSensorTags.Add(item);
                                AddNode(tree, item);


                            }
                        }
                    }
                }

                foreach (Circuit item in from items in m_dicSensorTags.Values.AsEnumerable()
                                           where items.Name.Contains(strSearchWord)
                                           && m_listAddedSensorTags.Contains(items) == false
                                           select items
                    )
                {
                    AddNode(tree, item);
                }
            }
            else
            {
                foreach (KeyValuePair<int, Circuit> pair in m_dicSensorTags)
                {
                    AddNode(tree, pair.Value);
                }
            }


            tree.ExpandAll();
        }

        private void AddNode(TreeView tree, Circuit tag)
        {
            FireSensor sensorZone = null;

            if (m_dicSensorZones.TryGetValue(tag.SensorZoneID, out sensorZone) == false)
                return;

            EquipmentZone equipZone = null;

            if (m_dicEquipZones.TryGetValue(sensorZone.EquipZoneID, out equipZone) == false)
                return;

            foreach (Zone zone in equipZone.LinkedZoneList)
            {
                if (zone.IsOutdoor)
                {
                    TreeNode root = GetSensorParent(tree, tag);
                    TreeNode grp = GetOutdoorZoneRootNode(root);
                    TreeNode zoneNode = GetZoneNode(grp.Nodes, zone);
                    AddSensorTagNode(zoneNode, tag);
                }
                else if (zone.Building != null)
                {
                    TreeNode root = GetSensorParent(tree, tag);
                    TreeNode grp = GetBuildingGroupNode(root, zone.Building.BuildingGroup);
                    TreeNode buildingNode = GetBuildingNode(grp.Nodes, zone.Building);
                    TreeNode zoneNode = GetZoneNode(grp.Nodes, zone);
                    AddSensorTagNode(zoneNode, tag);
                }

                // 첫번째 Zone에만 넣도록 한다.
                break;
            }
        }

        private TreeNode GetSensorParent(TreeView tree, Circuit tag)
        {
            string szTagName = "화재센서";
            int nTag = 0;
            if (tag.SensorType == IFacility.FacilityType.FIRE_SENSOR || tag.SensorType == IFacility.FacilityType.SecomFire)
            {
                nTag = 0;
            }
            else if (tag.SensorType == IFacility.FacilityType.ExternalAlarmBell)
            {
                nTag = 4000;
                szTagName = "EMPOLL";
            }
            else if (tag.SensorType >= IFacility.FacilityType.Intrusion_S1 && tag.SensorType <= IFacility.FacilityType.EmergencyBell_S1)
            {
                nTag = 900;
                szTagName = "SVMS";
            }
            else if (tag.SensorType >= IFacility.FacilityType.GeneralIntrusionT1_S1 && tag.SensorType < IFacility.FacilityType.ExternalAlarmBell)
            {
                nTag = 1000;
                szTagName = "ACCESS";
            }
            else if (tag.SensorType >= IFacility.FacilityType.SecomExternalAlarmBell && tag.SensorType <= IFacility.FacilityType.SecomWomenAlarmBell)
            {
                nTag = 5001;
                szTagName = "Secom";
            }

            foreach (TreeNode node in tree.Nodes)
            {
                if ((int)node.Tag == nTag)
                    return node;
            }

            TreeNode groupNode = null;
            groupNode = tree.Nodes.Add(szTagName);
            groupNode.Tag = nTag;
            return groupNode;
        }


        private TreeNode GetBuildingGroupNode(TreeNode tree, BuildingGroup group)
        {
            int nOutdoorZoneIndex = -1;

            foreach (TreeNode node in tree.Nodes)
            {
                if (node.Tag == null)
                    nOutdoorZoneIndex = tree.Nodes.IndexOf(node);
                else if (node.Tag == group)
                    return node;
            }

            TreeNode groupNode = null;

            if (nOutdoorZoneIndex < 0)
                groupNode = tree.Nodes.Add(group.BuildingGroupName);
            else
                groupNode = tree.Nodes.Insert(nOutdoorZoneIndex, group.BuildingGroupName);

            groupNode.Tag = group;
            return groupNode;
        }

        private TreeNode AddSensorTagNode(TreeNode node, Circuit tag)
        {
            TreeNode tagNode = node.Nodes.Add(tag.Name);
            tagNode.Tag = tag;
            return tagNode;
        }

        private TreeNode GetOutdoorZoneRootNode(TreeNode tree)
        {
            foreach (TreeNode node in tree.Nodes)
            {
                if (node.Tag == null)
                    return node;
            }

            return tree.Nodes.Add("실외영역");
        }

        private TreeNode GetZoneNode(TreeNodeCollection nodes, Zone zone)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Tag == zone)
                    return node;
            }

            TreeNode zoneNode = nodes.Add(zone.ZoneName);
            zoneNode.Tag = zone;
            return zoneNode;
        }

        private TreeNode GetBuildingNode(TreeNodeCollection nodes, Building building)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Tag == building)
                    return node;
            }

            TreeNode buildingNode = nodes.Add(building.BuildingName);
            buildingNode.Tag = building;
            return buildingNode;
        }


        public Circuit GetSensorTagBySensorZoneID(int nSensorZoneID)
        {
            Circuit sensor = null;

            foreach (Circuit item in from items in m_dicSensorTags.Values.AsEnumerable()
                                       where items.SensorZoneID > 0
                                       && items.SensorZoneID == nSensorZoneID
                                       select items)
            {
                sensor = item;
                break;
            }

            return sensor;
        }

        public Circuit GetSensorTag(int nID)
        {
            Circuit circuit = null;

            if (m_dicSensorTags.TryGetValue(nID, out circuit) == false)
                return null;

            return circuit;
        }

        public EquipmentZone GetEquipZone(int nEquipZoneID)
        {
            EquipmentZone equipZone = null;

            if (m_dicEquipZones.TryGetValue(nEquipZoneID, out equipZone))
                return equipZone;

            return null;
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

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

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

        public DataTeam GetRegularTeam(int nTeamID)
        {
            DataTeam team = null;

            if (m_dicRegularTeams.TryGetValue(nTeamID, out team) == false)
                return null;

            return team;
        }

        public DataTeam GetExternalTeam(int nTeamID)
        {
            DataTeam team = null;

            if (m_dicExternalTeams.TryGetValue(nTeamID, out team) == false)
                return null;

            return team;
        }

        public DataTeamControlRoom GetControlRoomTeam(int nTeamID)
        {
            DataTeamControlRoom team = null;

            if (m_dicControlRoomTeams.TryGetValue(nTeamID, out team) == false)
                return null;

            return team;
        }

        public void AddPhoneNumberFromGroup(Dictionary<string, string> dicPhoneNumbers, FacilityManagerGroup group)
        {
            if (group == null)
                return;

            foreach (FacilityManager mgr in group.CompanyMembers)
            {
                AddPhoneNumber(dicPhoneNumbers, mgr);
            }

            // 171114 KYJ TEST
            //
            foreach (FacilityManager mgr in group.ExternalCompanyMembers)
            {
                AddPhoneNumber(dicPhoneNumbers, mgr);
            }

            foreach (FacilityManager mgr in group.RegularTeams)
            {
                AddPhoneNumber(dicPhoneNumbers, mgr);
            }

            foreach (FacilityManager mgr in group.ExternalTeams)
            {
                AddPhoneNumber(dicPhoneNumbers, mgr);
            }

            foreach (FacilityManager mgr in group.ControlRoomMembers)
            {
                AddPhoneNumber(dicPhoneNumbers, mgr);
            }
        }

        private void AddPhoneNumber(Dictionary<string, string> dicPhoneNumbers, FacilityManager mgr)
        {
            if (mgr.MemberType == 0)
            {
                DataCompanyMember member = (DataCompanyMember)mgr.Tag;

                if (member == null)
                    return;

                dicPhoneNumbers[member.PhoneNumber] = member.PhoneNumber;
            }
            else if (mgr.MemberType == 1 || mgr.MemberType == 4)
            {
                DataTeam team = (DataTeam)mgr.Tag;
                AddRegularTeamPhoneNumber(dicPhoneNumbers, team, mgr);

            }
            else if (mgr.MemberType == 2)
            {
                DataExternalMember member = (DataExternalMember)mgr.Tag;

                if (member == null)
                    return;

                dicPhoneNumbers[member.PhoneNumber] = member.PhoneNumber;
            }
            else if (mgr.MemberType == 3 || mgr.MemberType == 5)
            {
                DataTeam team = (DataTeam)mgr.Tag;
                AddExternalTeamPhoneNumber(dicPhoneNumbers, team);

            }
            else if (mgr.MemberType == 7)
            {
                DataTeamControlRoom team = (DataTeamControlRoom)mgr.Tag;
                AddControlRoomPhoneNumbers(dicPhoneNumbers, team);
            }
        }

        private void AddControlRoomPhoneNumbers(Dictionary<string, string> dicPhoneNumbers, DataTeamControlRoom team)
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

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nMemberType = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nMemberID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);

                if (nMemberType == 1)
                {
                    DataCompanyMember member = GetCompanyMember(nMemberID);

                    if (member != null)
                        dicPhoneNumbers[member.PhoneNumber] = member.PhoneNumber;
                }
                else if (nMemberType == 4)
                {
                    DataExternalMember member = GetExternalMember(nMemberID);

                    if (member != null)
                        dicPhoneNumbers[member.PhoneNumber] = member.PhoneNumber;
                }
            }
        }

        private void AddExternalTeamPhoneNumber(Dictionary<string, string> dicPhoneNumbers, DataTeam team)
        {
            if (team == null)
                return;

            ArrayList arrMembers = DataManager.Instance.GetTeamMembers(team);

            if (arrMembers != null)
            {
                foreach (DataExternalMember member in arrMembers)
                {
                    dicPhoneNumbers[member.PhoneNumber] = member.PhoneNumber;
                }
            }

            foreach (DataTeam childTeam in team.ChildTeams)
            {
                AddExternalTeamPhoneNumber(dicPhoneNumbers, childTeam);
            }
        }

        private void AddRegularTeamPhoneNumber(Dictionary<string, string> dicPhoneNumbers, DataTeam team, FacilityManager mgr)
        {
            if (team == null)
                return;

            ArrayList arrMembers = DataManager.Instance.GetTeamMembers(team);

            if (arrMembers != null)
            {
                foreach (DataCompanyMember member in arrMembers)
                {
                    if (mgr.LevelLimit > 0)
                    {
                        if (mgr.UpperLimit > 0)
                        {
                            // member.LevelID 또는 그 상위 직급에게 문자메시지를 보낸다.
                            if (member.LevelID > 0 && member.LevelID <= mgr.LevelLimit)
                            {
                                dicPhoneNumbers[member.PhoneNumber] = member.PhoneNumber;
                            }
                        }
                        else if (mgr.UpperLimit < 0)
                        {
                            // member.LevelID 또는 그 하위 직급에게 문자메시지를 보낸다.
                            if ((member.LevelID > 0 && member.LevelID >= mgr.LevelLimit) ||
                                member.LevelID == 0)
                            {
                                dicPhoneNumbers[member.PhoneNumber] = member.PhoneNumber;
                            }
                        }
                        else
                        {
                            if (member.LevelID == mgr.LevelLimit)
                            {
                                dicPhoneNumbers[member.PhoneNumber] = member.PhoneNumber;
                            }
                        }
                    }
                    else
                    {
                        dicPhoneNumbers[member.PhoneNumber] = member.PhoneNumber;
                    }
                }
            }

            foreach (DataTeam childTeam in team.ChildTeams)
            {
                AddRegularTeamPhoneNumber(dicPhoneNumbers, childTeam, mgr);
            }
        }
    }
}
