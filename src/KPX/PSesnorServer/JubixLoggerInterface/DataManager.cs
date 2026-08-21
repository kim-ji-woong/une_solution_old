using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBUtility;
using System.Collections;

namespace JubixNetwork
{
    public class DataManager
    {
        private string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

        private DataTeam m_teamRegularRoot = null;
        private List<DataTeam> m_listExternalRootTeams = new List<DataTeam>();
        private Dictionary<int, DataTeam> m_dicRegularTeams = new Dictionary<int, DataTeam>();
        private Dictionary<DataTeam, List<CompanyMember>> m_dicRegularTeamMembers = new Dictionary<DataTeam, List<CompanyMember>>();
        private Dictionary<int, CompanyMember> m_dicRegularMembers = new Dictionary<int, CompanyMember>();
        private Dictionary<int, DataTeam> m_dicExternalTeams = new Dictionary<int, DataTeam>();
        private Dictionary<DataTeam, List<ExternalCompanyMember>> m_dicExternalTeamMembers = new Dictionary<DataTeam, List<ExternalCompanyMember>>();
        private Dictionary<int, ExternalCompanyMember> m_dicExternalMembers = new Dictionary<int, ExternalCompanyMember>();

        public DataManager(WebDBManager dbMgr, int nSiteID)
        {
            m_teamRegularRoot = LoadRegularTeam(dbMgr, m_dicRegularTeams, nSiteID);
            m_listExternalRootTeams = LoadExternalTeam(dbMgr, m_dicExternalTeams, nSiteID);

            LoadCompanyMember(dbMgr, m_dicRegularTeams, nSiteID);
            LoadExternalMember(dbMgr, m_dicExternalTeams, nSiteID);
        }

        public List<string> GetFacilityManagerPhoneNumberList()
        {
            string strTableName = "FacilityManager";

            string szText = "SELECT id, MemberID, MemberType, FacilityType FROM {1} WHERE SiteID = {0} ORDER BY FacilityType";
            string strSQL = string.Format(szText, JubixSensorManager.Instance.SiteID, strTableName);

            ArrayList arrResult = JubixSensorManager.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null)
                return null;

            Dictionary<string, string> dicPhoneNumbers = new Dictionary<string, string>();

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nMemberID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nMemberType = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nFacilityType = DBUtility.WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);

                if (nID < 0 || nMemberID < 0)
                    continue;

                AddFacilityManager(nID, nMemberID, nMemberType, nFacilityType, dicPhoneNumbers);
            }

            return dicPhoneNumbers.Values.ToList();
        }

        private void AddFacilityManager(int nID, int nMemberID, int nMemberType, int nFacilityType, Dictionary<string, string> dicPhoneNumbers)
        {
            if (nMemberType == 0)
            {
                if (!m_dicRegularMembers.ContainsKey(nMemberID))
                    return;

                CompanyMember member = m_dicRegularMembers[nMemberID];

                if (dicPhoneNumbers.ContainsKey(member.PhoneNumber) == false)
                    dicPhoneNumbers[member.PhoneNumber] = member.PhoneNumber;
            }
            else if (nMemberType == 1)
            {
                if (!m_dicRegularTeams.ContainsKey(nMemberID))
                    return;

                DataTeam team = m_dicRegularTeams[nMemberID];
                AddTeamMemberPhoneNumbers(team, dicPhoneNumbers);
            }
            else if (nMemberType == 2)
            {
                if (!m_dicExternalMembers.ContainsKey(nMemberID))
                    return;

                ExternalCompanyMember member = m_dicExternalMembers[nMemberID];

                if (dicPhoneNumbers.ContainsKey(member.PhoneNumber) == false)
                    dicPhoneNumbers[member.PhoneNumber] = member.PhoneNumber;
            }
            else if (nMemberType == 3)
            {
                if (!m_dicExternalTeams.ContainsKey(nMemberID))
                    return;

                DataTeam team = m_dicExternalTeams[nMemberID];
                AddTeamMemberPhoneNumbers(team, dicPhoneNumbers);
            }
            else if (nMemberType == 4)
            {
                DataTeam team = GetCompany(m_dicRegularTeams);
                if (team == null)
                    return;

                AddTeamMemberPhoneNumbers(team, dicPhoneNumbers);
            }
            else if (nMemberType == 5)
            {
                DataTeam team = GetCompany(m_listExternalRootTeams, nMemberID);
                if (team == null)
                    return;

                AddTeamMemberPhoneNumbers(team, dicPhoneNumbers);
            }
        }

        private void AddTeamMemberPhoneNumbers(DataTeam team, Dictionary<string, string> dicPhoneNumbers)
        {
            if (team.External == false)
                AddRegularTeamMemberPhoneNumbers(team, dicPhoneNumbers);
            else
                AddExternalTeamMemberPhoneNumbers(team, dicPhoneNumbers);
        }

        private void AddRegularTeamMemberPhoneNumbers(DataTeam team, Dictionary<string, string> dicPhoneNumbers)
        {
            List<CompanyMember> members = null;

            if (m_dicRegularTeamMembers.TryGetValue(team, out members))
            {
                foreach (CompanyMember member in members)
                {
                    if (dicPhoneNumbers.ContainsKey(member.PhoneNumber) == false)
                        dicPhoneNumbers[member.PhoneNumber] = member.PhoneNumber;
                }
            }

            foreach (DataTeam child in team.Children)
            {
                AddRegularTeamMemberPhoneNumbers(child, dicPhoneNumbers);
            }
        }

        private void AddExternalTeamMemberPhoneNumbers(DataTeam team, Dictionary<string, string> dicPhoneNumbers)
        {
            List<ExternalCompanyMember> members = null;

            if (m_dicExternalTeamMembers.TryGetValue(team, out members))
            {
                foreach (ExternalCompanyMember member in members)
                {
                    if (dicPhoneNumbers.ContainsKey(member.PhoneNumber) == false)
                        dicPhoneNumbers[member.PhoneNumber] = member.PhoneNumber;
                }
            }

            foreach (DataTeam child in team.Children)
            {
                AddExternalTeamMemberPhoneNumbers(child, dicPhoneNumbers);
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

        private DataTeam GetCompany(List<DataTeam> arrCompanies, int nCompanyID)
        {
            foreach (DataTeam team in arrCompanies)
            {
                if (team.ID == nCompanyID)
                    return team;
            }

            return null;
        }

        public bool LoadExternalMember(WebDBManager dbMgr, Dictionary<int, DataTeam> dicTeams, int nSiteID)
        {
            m_dicExternalMembers.Clear();

            StringBuilder sb1 = new StringBuilder();

            sb1.Append("Select eml.ExternalCompanyTeamID, eml.ExternalCompanyMemberID, ecm.Name, ecm.PhoneNumber ");
            sb1.Append("from ExternalCompanyMember as ecm, ExternalMemberList as eml, ExternalTeam as et ");
            sb1.AppendFormat("where eml.ExternalCompanyMemberID = ecm.ID and et.ID = eml.ExternalCompanyTeamID and et.SiteID = {0}", nSiteID);

            string szSQL = sb1.ToString();

            ArrayList arrResult = dbMgr.GetResultData(szSQL, 0);
            if (arrResult == null)
                return false;

            int nCount = arrResult.Count;
            if (nCount == 0)
                return true;

            ExternalCompanyMember member;

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
                    member = new ExternalCompanyMember();

                    member.ID = nID;
                    member.Name = strMemberName;
                    member.PhoneNumber = szPhoneNumber;
                    member.Team = team;

                    m_dicExternalMembers[nID] = member;
                }

                List<ExternalCompanyMember> arrMembers = null;

                if (m_dicExternalTeamMembers.ContainsKey(team))
                    arrMembers = m_dicExternalTeamMembers[team];
                else
                {
                    arrMembers = new List<ExternalCompanyMember>();
                    m_dicExternalTeamMembers[team] = arrMembers;
                }

                arrMembers.Add(member);
            }

            return false;
        }

        public bool LoadCompanyMember(DBUtility.WebDBManager dbMgr, Dictionary<int, DataTeam> dicTeams, int nSiteID)
        {
            m_dicRegularMembers.Clear();

            string strSQL = string.Format("SELECT TeamID FROM Site WHERE ID = {0}", nSiteID);
            ArrayList arrResult1 = dbMgr.GetResultData(strSQL, 0);
            if (arrResult1 == null || arrResult1.Count == 0)
                return false;

            int nTeamID = WebDBManager.GetIntField(arrResult1[0].ToString(), -1);
            if (nTeamID == -1)
                return false;

            ArrayList arrResult2 = ExecuteTeamList(dbMgr, nTeamID);

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

            string szText = "select rm.RegularTeamID, rm.CompanyMemberID, rm.PositionID , MemberName, LevelID, MemberID, OfficePhoneNumber, PhoneNumber " +
                            " FROM CompanyMember as cm, RegularMemberList as rm WHERE cm.ID = rm.CompanyMemberID and rm.RegularTeamID in ({0})";

            strSQL = string.Format(szText, szTeamList);
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null) return false;

            int nCount = arrResult.Count;
            if (nCount == 0) return true;

            CompanyMember member;

            for (int i = 0; i < nCount - 7; i += 8)
            {
                int nRegularTeamID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), 0);
                int nPositionID = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
                string strMemberName = DBUtility.WebDBManager.GetStringField(arrResult[i + 3], "");
                int nLevelID = DBUtility.WebDBManager.GetIntField(arrResult[i + 4].ToString(), 0);
                string strMemberID = DBUtility.WebDBManager.GetStringField(arrResult[i + 5], "");
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
                    member = new CompanyMember();

                    member.ID = nID;
                    member.Name = strMemberName;
                    member.PhoneNumber = strPhoneNumber;

                    m_dicRegularMembers[nID] = member;
                }

                List<CompanyMember> arrMembers = null;

                if (m_dicRegularTeamMembers.ContainsKey(team))
                    arrMembers = m_dicRegularTeamMembers[team];
                else
                {
                    arrMembers = new List<CompanyMember>();
                    m_dicRegularTeamMembers[team] = arrMembers;
                }

                arrMembers.Add(member);
                ////////////////////////////////////////////////////////////////
            }

            foreach (KeyValuePair<DataTeam, List<CompanyMember>> pair in m_dicRegularTeamMembers)
            {
                pair.Value.Sort();
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

        // dicTeams : ID별 Team
        private List<DataTeam> LoadExternalTeam(WebDBManager dbMgr, Dictionary<int, DataTeam> dicTeams, int nSiteID)
        {
            dicTeams.Clear();

            List<DataTeam> arrExternalRootTeams = new List<DataTeam>();
            string szText2 = "SELECT et.ID, et.TeamName, et.ParentTeamID " +
                             " FROM ExternalTeam as et WHERE et.SiteID = {0} ";

            string szSQL = string.Format(szText2, nSiteID);

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
        private DataTeam LoadRegularTeam(WebDBManager dbMgr, Dictionary<int, DataTeam> dicTeams, int nSiteID)
        {
            dicTeams.Clear();
            //string szSQL = "SELECT R.ID, R.TeamName, R.ParentTeamID FROM RegularTeam as R";

            string strSQL = string.Format("SELECT TeamID FROM Site WHERE ID = {0}", nSiteID);
            ArrayList arrResult1 = dbMgr.GetResultData(strSQL, 0);
            if (arrResult1 == null || arrResult1.Count == 0)
                return null;

            int nTeamID = WebDBManager.GetIntField(arrResult1[0].ToString(), -1);
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

        public static ArrayList ExecuteTeamList(WebDBManager dbMgr, int nRootTeamID, string strTableName = "RegularTeam")
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
    }

    public class DataTeam
    {
        private int m_nID = -1;
        private string m_strTeamName = "";
        private bool m_isExternal = false;
        private DataTeam m_teamParent = null;
        private bool m_isCompany = false;
        private string m_strCompanyName = "";
        private List<DataTeam> m_children = new List<DataTeam>();

        public List<DataTeam> Children
        {
            get { return m_children; }
            set { m_children = value; }
        }

        public string CompanyName
        {
            get { return m_strCompanyName; }
            set { m_strCompanyName = value; }
        }

        public bool IsCompany
        {
            get { return m_isCompany; }
            set { m_isCompany = value; }
        }

        public bool External
        {
            get { return m_isExternal; }
            set { m_isExternal = value; }
        }

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string TeamName
        {
            get { return m_strTeamName; }
            set { m_strTeamName = value; }
        }

        public DataTeam ParentTeam
        {
            get { return m_teamParent; }
            set
            {
                if (m_teamParent != value)
                {
                    if (value != null)
                    {
                        if (value.m_children.Contains(this) == false)
                            value.m_children.Add(this);
                    }

                    if (m_teamParent != null)
                    {
                        m_teamParent.m_children.Remove(this);
                    }

                    m_teamParent = value;
                }
            }
        }
    }

    public class CompanyMember
    {
        private int m_nID = -1;
        private string m_strName = "";
        private string m_strPhoneNumber = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        public string PhoneNumber
        {
            get { return m_strPhoneNumber; }
            set { m_strPhoneNumber = value; }
        }
    }

    public class ExternalCompanyMember
    {
        private int m_nID = -1;
        private string m_strName = "";
        private string m_strPhoneNumber = "";
        private DataTeam m_team = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        public string PhoneNumber
        {
            get { return m_strPhoneNumber; }
            set { m_strPhoneNumber = value; }
        }

        public DataTeam Team
        {
            get { return m_team; }
            set { m_team = value; }
        }
    }
}
