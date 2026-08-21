using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using UnE.Sensor; 

namespace KpxPipeMonitoring
{
    public enum FacilityType { Pipe = 0, Tank };

    public class DataManager
    {
        private string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

        private DataTeam m_teamRegularRoot = null;
        private ArrayList m_listExternalRootTeams = new ArrayList();
        private Dictionary<int, DataTeam> m_dicRegularTeams = new Dictionary<int, DataTeam>();
        private Dictionary<DataTeam, ArrayList> m_dicRegularTeamMembers = new Dictionary<DataTeam, ArrayList>();
        private Dictionary<int, CompanyMember> m_dicRegularMembers = new Dictionary<int, CompanyMember>();
        private Dictionary<int, DataTeam> m_dicExternalTeams = new Dictionary<int, DataTeam>();
        private Dictionary<DataTeam, ArrayList> m_dicExternalTeamMembers = new Dictionary<DataTeam, ArrayList>();
        private Dictionary<int, DataExternalMember> m_dicExternalMembers = new Dictionary<int, DataExternalMember>();

        // 직위상세(본부장, 전무, 과장, 부장)...
        private Dictionary<int, string> m_dicJobSubPosition = new Dictionary<int, string>();

        // 시설물 타입별 담당자
        private Dictionary<FacilityType, FacilityManagerGroup> m_dicFacilityManagers = new Dictionary<FacilityType, FacilityManagerGroup>();

        public Dictionary<FacilityType, FacilityManagerGroup> FacilityManagerGroups
        {
            get { return m_dicFacilityManagers; }
        }

        public DataTeam RegularTeamRoot
        {
            get { return m_teamRegularRoot; }
        }

        public ArrayList ExternalTeamRootList
        {
            get { return m_listExternalRootTeams; }
        }
        
        public DataManager()
        {
            m_teamRegularRoot = LoadRegularTeam(MainForm.Instance.dbMgr, m_dicRegularTeams);
            m_listExternalRootTeams = LoadExternalTeam(MainForm.Instance.dbMgr, m_dicExternalTeams);

            LoadCompanyMember(MainForm.Instance.dbMgr, m_dicRegularTeams);
            LoadExternalMember(MainForm.Instance.dbMgr, m_dicExternalTeams);
            LoadFacilityManager();
        }

        public void LoadFacilityManager()
        {
            m_dicFacilityManagers.Clear();

            string strTableName = "FacilityManager";

            string szText = "SELECT id, MemberID, MemberType, FacilityType FROM {1} WHERE SiteID = {0} ORDER BY FacilityType";
            string strSQL = string.Format(szText, MainForm.Instance.SiteID, strTableName);

            ArrayList arrResult = MainForm.Instance.dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nMemberID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nMemberType = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nFacilityType = DBUtility.WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);

                if (nID < 0 || nMemberID < 0)
                    continue;

                FacilityManagerGroup group = GetFacilityManagerGroup(nFacilityType);
                if (group == null)
                    continue;

                AddFacilityManager(nID, nMemberID, nMemberType, nFacilityType, group);
            }
        }

        private void AddFacilityManager(int nID, int nMemberID, int nMemberType, int nFacilityType, FacilityManagerGroup group)
        {
            FacilityManager mgr = new FacilityManager();
            mgr.ID = nID;
            mgr.MemberID = nMemberID;
            mgr.MemberType = nMemberType;
            mgr.Type = IFacility.ToFacilityType(nFacilityType);
            
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
                group.ExternalTeams.Add(mgr);
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

        private FacilityManagerGroup GetFacilityManagerGroup(int nFacilityType)
        {
            Dictionary<FacilityType, FacilityManagerGroup> dicFacilityManagers = m_dicFacilityManagers;

            FacilityManagerGroup group = null;
            FacilityType type = (FacilityType)nFacilityType;

            if (dicFacilityManagers.TryGetValue(type, out group) == false)
            {
                group = new FacilityManagerGroup();
                dicFacilityManagers[type] = group;
            }

            return group;
        }

        public bool LoadExternalMember(DBUtility.WebDBManager dbMgr, Dictionary<int, DataTeam> dicTeams)
        {
            m_dicExternalMembers.Clear();
            
            StringBuilder sb1 = new StringBuilder();
            
            sb1.Append("Select eml.ExternalCompanyTeamID, eml.ExternalCompanyMemberID, ecm.Name, ecm.PhoneNumber ");
            sb1.Append("from ExternalCompanyMember as ecm, ExternalMemberList as eml, ExternalTeam as et ");
            sb1.AppendFormat("where eml.ExternalCompanyMemberID = ecm.ID and et.ID = eml.ExternalCompanyTeamID and et.SiteID = {0}", MainForm.Instance.SiteID);

            string szSQL = sb1.ToString();

            ArrayList arrResult = dbMgr.GetResultData(szSQL, 0);
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

        private bool LoadJobSubPositions(DBUtility.WebDBManager dbMgr)
        {
            m_dicJobSubPosition.Clear();

            string strSQL = "Select ID, Name from JobSubPosition";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                DBUtility.VariousData<int> id = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString());
                string strSubPositionName = DBUtility.WebDBManager.GetStringField(arrResult[i + 1]);

                if (id == null || strSubPositionName == null)
                    continue;

                m_dicJobSubPosition[id.Data] = strSubPositionName;
            }

            return true;
        }

        public bool LoadCompanyMember(DBUtility.WebDBManager dbMgr, Dictionary<int, DataTeam> dicTeams)
        {
            m_dicRegularMembers.Clear();

            string strSQL = string.Format("SELECT TeamID FROM Site WHERE ID = {0}", MainForm.Instance.SiteID);
            ArrayList arrResult1 = dbMgr.GetResultData(strSQL, 0);
            if (arrResult1 == null || arrResult1.Count == 0)
                return false;

            int nTeamID = DBUtility.WebDBManager.GetIntField(arrResult1[0].ToString(), -1);
            if (nTeamID == -1)
                return false;

            ArrayList arrResult2 = ExecuteTeamList(dbMgr, nTeamID);

            if (arrResult2 == null || arrResult2.Count == 0)
                return false;

            string szTeamList = "";
            for (int i = 0; i < arrResult2.Count - 2; i += 3)
            {
                string szTeamID = DBUtility.WebDBManager.GetStringField(arrResult2[i].ToString(), "");
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

            LoadJobSubPositions(dbMgr);

            string szText = "select rm.RegularTeamID, rm.CompanyMemberID, rm.PositionID, rm.SubPositionID, MemberName, LevelID, MemberID, OfficePhoneNumber, PhoneNumber " +
                            " FROM CompanyMember as cm, RegularMemberList as rm WHERE cm.ID = rm.CompanyMemberID and rm.RegularTeamID in ({0})";

            strSQL = string.Format(szText, szTeamList);
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null) return false;

            int nCount = arrResult.Count;
            if (nCount == 0) return true;

            CompanyMember member;

            for (int i = 0; i < nCount - 8; i += 9)
            {
                int nRegularTeamID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), 0);
                int nPositionID = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
                int nSubPositionID = DBUtility.WebDBManager.GetIntField(arrResult[i + 3].ToString(), 0);
                string strMemberName = DBUtility.WebDBManager.GetStringField(arrResult[i + 4], "");
                int nLevelID = DBUtility.WebDBManager.GetIntField(arrResult[i + 5].ToString(), 0);
                string strMemberID = DBUtility.WebDBManager.GetStringField(arrResult[i + 6], "");
                string strOfficePhoneNumber = DBUtility.WebDBManager.GetStringField(arrResult[i + 7], "");
                string strPhoneNumber = DBUtility.WebDBManager.GetStringField(arrResult[i + 8], "");

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
                    member = new CompanyMember();

                    member.ID = nID;
                    member.MemberName = strMemberName;
                    member.LevelID = nLevelID;
                    member.MemberID = strMemberID;
                    member.OfficePhoneNumber = strOfficePhoneNumber;
                    member.PhoneNumber = strPhoneNumber;

                    if (nSubPositionID > 0)
                    {
                        string strSubJobPositionName = "";

                        if (m_dicJobSubPosition.TryGetValue(nSubPositionID, out strSubJobPositionName))
                        {
                            member.SubJobPositionName = strSubJobPositionName;
                        }
                    }

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
                ////////////////////////////////////////////////////////////////
            }

            foreach (KeyValuePair<DataTeam, ArrayList> pair in m_dicRegularTeamMembers)
            {
                pair.Value.Sort();
            }

            return true;
        }

        // dicTeams : ID별 Team
        private ArrayList LoadExternalTeam(DBUtility.WebDBManager dbMgr, Dictionary<int, DataTeam> dicTeams)
        {
            dicTeams.Clear();
            
            ArrayList arrExternalRootTeams = new ArrayList();
            string szText2 = "SELECT et.ID, et.TeamName, et.ParentTeamID " +
                             " FROM ExternalTeam as et WHERE et.SiteID = {0} ";

            string szSQL = string.Format(szText2, MainForm.Instance.SiteID);

            ArrayList arrResult = dbMgr.GetResultData(szSQL, 0);
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


                DataTeam data = new DataTeam();
                data.ID = nID;
                data.TeamName = szTeamName;
                data.External = true;

                if (nParentTeamID == -1)
                {
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
        private DataTeam LoadRegularTeam(DBUtility.WebDBManager dbMgr, Dictionary<int, DataTeam> dicTeams)
        {
            dicTeams.Clear();
            //string szSQL = "SELECT R.ID, R.TeamName, R.ParentTeamID FROM RegularTeam as R";

            string strSQL = string.Format("SELECT TeamID FROM Site WHERE ID = {0}", MainForm.Instance.SiteID);
            ArrayList arrResult1 = dbMgr.GetResultData(strSQL, 0);
            if (arrResult1 == null || arrResult1.Count == 0)
                return null;

            int nTeamID = DBUtility.WebDBManager.GetIntField(arrResult1[0].ToString(), -1);
            if (nTeamID == -1)
                return null;

            ArrayList arrResult = ExecuteTeamList(dbMgr, nTeamID);

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

        public static ArrayList ExecuteTeamList(DBUtility.WebDBManager dbMgr, int nRootTeamID, string strTableName = "RegularTeam")
        {
            string strSQL = "Select ID, TeamName, ParentTeamID from " + strTableName + " order by ParentTeamID, ID";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return null;

            if (nRootTeamID == 0)
                return arrResult;

            int nResultCount = arrResult.Count;

            ArrayList arrNewResult = new ArrayList();
            Dictionary<int, int> dicParentID = new Dictionary<int, int>();

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strTeamName = DBUtility.WebDBManager.GetStringField(arrResult[i + 1], "");
                DBUtility.VariousData<int> parentID = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString());

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
    }

    public class CompanyMember : DataCompanyMember
    {
        private string m_strSubJobPositionName = "";

        public string SubJobPositionName
        {
            get { return m_strSubJobPositionName; }
            set { m_strSubJobPositionName = value; }
        }
    }
}
