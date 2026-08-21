using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using DBUtility;

namespace SOPMonitoringSystem
{
    public class SOPManager
    {
        private WebDBManager m_dbMgr = null;

        // FullPath(Category/SubCategory/Disaster), DisasterInfo
        private Dictionary<string, DisasterInfo> m_dicSOPRegularNormal = new Dictionary<string, DisasterInfo>();
        private Dictionary<string, DisasterInfo> m_dicSOPRegularEmergency = new Dictionary<string, DisasterInfo>();
        private Dictionary<string, DisasterInfo> m_dicSOPNonRegularNormal = new Dictionary<string, DisasterInfo>();
        private Dictionary<string, DisasterInfo> m_dicSOPNonRegularEmergency = new Dictionary<string, DisasterInfo>();

        // FullPath(Category/SubCategory/Disaster)별 모든 버전의 DisasterInfo Version
        private Dictionary<string, ArrayList> m_dicSOPRegularNormalDisasterList = new Dictionary<string, ArrayList>();
        private Dictionary<string, ArrayList> m_dicSOPRegularEmergencyDisasterList = new Dictionary<string, ArrayList>();
        private Dictionary<string, ArrayList> m_dicSOPNonRegularNormalDisasterList = new Dictionary<string, ArrayList>();
        private Dictionary<string, ArrayList> m_dicSOPNonRegularEmergencyDisasterList = new Dictionary<string, ArrayList>();

        // DisasterID, VersionInfo
        private Dictionary<int, VersionInfo> m_dicVersionRegularNormal = new Dictionary<int, VersionInfo>();
        private Dictionary<int, VersionInfo> m_dicVersionRegularEmergency = new Dictionary<int, VersionInfo>();
        private Dictionary<int, VersionInfo> m_dicVersionNonRegularNormal = new Dictionary<int, VersionInfo>();
        private Dictionary<int, VersionInfo> m_dicVersionNonRegularEmergency = new Dictionary<int, VersionInfo>();

        // ActionStepID, ActionStepInfo
        private Dictionary<int, ActionStepInfo> m_dicActionStepInfo = new Dictionary<int, ActionStepInfo>();
        // Key : 상위 4바이트(1이면 실제모드, 0이면 훈련모드), 하위 4바이트(ActionStepID)
        // Value : ActionStepHistory ID
        private Dictionary<long, int> m_dicActionStepHistory = new Dictionary<long, int>();
        private int m_nCurrentActionStepID = -1;
        private bool m_isCurrentRealMode = false;

       // private TreeNode m_prevSelectedNode = null;
        //private int m_nPrevSelectedRow = -1;
        private bool m_isRegular = true;
        private bool m_isNormal = true;

        //private string m_selectedCategoryName = "";
       // private string m_selectedSubCategoryName = "";
        //private string m_selectedDisasterName = "";
        //private ArrayList m_arrSelectedActionSteps = null;
        //private VersionInfo m_selectedVersion = null;
        private ArrayList m_arrCompanyMember = new ArrayList();

        private string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

        // 비상 조직 ID, 연결된 상시 조직의 ID List
        private Dictionary<int, ArrayList> m_dicTemporaryNormalTeamID = new Dictionary<int, ArrayList>();
        private Dictionary<int, ArrayList> m_dicTemporaryEmergencyTeamID = new Dictionary<int, ArrayList>();
        // RegularTeam별 팀원 List
        // RegularTeam ID, Data_CompanyMember List
        private Dictionary<int, ArrayList> m_dicRegularTeamMember = new Dictionary<int, ArrayList>();
        // 사용자 정의조직 ID, 연결된 정의 조직 Data
        private Dictionary<int, Data_ExternalTeam> m_dicUserDefinedTeam = new Dictionary<int, Data_ExternalTeam>();
        // 외부 조직 ID, 연결된 외부 조직 Data
        private Dictionary<int, Data_ExternalTeam> m_dicExternalTeam = new Dictionary<int, Data_ExternalTeam>();

        // 협력업체 팀들
        private ArrayList m_arrExternalCompanyTeams = new ArrayList();
        // 협력업체 팀원들
        private ArrayList m_arrExternalCompanyMembers = new ArrayList();

        public ArrayList ExternalCompanyTeams
        {
            get { return m_arrExternalCompanyTeams; }
        }

        public ArrayList ExternalCompanyMembers
        {
            get { return m_arrExternalCompanyMembers; }
        }

        private ArrayList m_arrDisaster = new ArrayList();
        public ArrayList DisasterList
        {
            get { return m_arrDisaster; }
            set { m_arrDisaster = value; }
        }

        private bool m_isOpened = false;

        public SOPManager(WebDBManager dbMgr)
        {
            m_dbMgr = dbMgr;
        }

        public bool Load(bool isRegular, bool isNormal)
        {
            if (m_isOpened)
            {
                if (m_isRegular == isRegular && m_isNormal == isNormal)
                    return true;
            }

            m_isRegular = isRegular;
            m_isNormal = isNormal;

            if (!LoadVersion())
                return Cancel();

            if (!LoadSOP())
                return Cancel();

            if (!LoadCompanyMember())
                return Cancel();

            if (!LoadTemporaryTeam())
                return Cancel();

            if (!LoadOtherTeams())
                return Cancel();

            LoadExternalCompanyTeams();
            LoadExternalCompanyMembers();

            ReadDisasterCategory();

            m_isOpened = true;
            return true;
        }

        public bool LoadRegularMember()
        {
            if (!LoadCompanyMember())
                return Cancel();

            if (!LoadTemporaryTeam())
                return Cancel();

            return true;
        }

        private void LoadExternalCompanyTeams()
        {
            WebDBManager dbMgr = FormMain.Instance.DBManager;

            string strSQL = "select id, TeamName, ParentTeamID, CompanyID from ExternalCompanyTeam";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            // TeamID, Child Team
            Dictionary<int, ExternalCompanyTeam> dicTeam = new Dictionary<int, ExternalCompanyTeam>();
            // TeamID, Parent TeamID
            Dictionary<int, int> dicParent = new Dictionary<int, int>();

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strTeamName = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nParentTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nCompanyID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);

                if (nID < 0)
                    continue;

                ExternalCompanyTeam team = new ExternalCompanyTeam();
                team.ID = nID;
                team.TeamName = strTeamName;
                team.CompanyID = nCompanyID;

                dicTeam[nID] = team;

                if (nParentTeamID < 0)
                    m_arrExternalCompanyTeams.Add(team);
                else
                    dicParent[nID] = nParentTeamID;
            }

            foreach (KeyValuePair<int, int> pair in dicParent)
            {
                if (!dicTeam.ContainsKey(pair.Key))
                    continue;

                if (!dicTeam.ContainsKey(pair.Value))
                    continue;

                ExternalCompanyTeam team = dicTeam[pair.Key];
                ExternalCompanyTeam teamParent = dicTeam[pair.Value];
                team.ParentTeam = teamParent;

                m_arrExternalCompanyTeams.Add(team);
            }
        }

        private void LoadExternalCompanyMembers()
        {
            if (m_arrExternalCompanyTeams.Count == 0)
                return;

            string strSQL = "select id, Name, PhoneNumber, IsTeamLeader, TeamID from ExternalCompanyMember";
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

            for (int i = 0; i < nResultCount - 4; i += 5)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strName = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strPhoneNumber = WebDBManager.GetStringField(arrResult[i + 2], "");
                bool isTeamLeader = WebDBManager.GetIntField(arrResult[i + 3].ToString(), 0) == 0 ? false : true;
                int nTeamID = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);

                if (nID < 0)
                    continue;

                ExternalCompanyTeam team = FindExternalCompanyTeam(nTeamID);
                if (team == null)
                    continue;

                if (string.Compare(strPhoneNumber, "null", true) == 0)
                    strPhoneNumber = "";

                try
                {
                    if (strPhoneNumber.Length > 0)
                        strPhoneNumber = DBUtility.AES256Cipher.AES_decrypt(strPhoneNumber, key);
                }
                catch (System.Exception)
                {                	
                }
              

                ExternalCompanyMember member = new ExternalCompanyMember();
                member.ID = nID;
                member.MemberName = strName;
                member.PhoneNumber = strPhoneNumber;
                member.IsTeamLeader = isTeamLeader;
                member.Team = team;

                m_arrExternalCompanyMembers.Add(member);
            }
        }

        public ExternalCompanyTeam FindExternalCompanyTeam(int nTeamID)
        {
            foreach (ExternalCompanyTeam team in m_arrExternalCompanyTeams)
            {
                if (team.ID == nTeamID)
                    return team;
            }

            return null;
        }

        private void ReadDisasterCategory()
        {
            string strSql = "SELECT * FROM DisasterCategory";
            ArrayList arrResult = m_dbMgr.GetResultData(strSql, 0);

            for (int i = 0; i < arrResult.Count - 1; i += 2)
            {
                Data_DisasterCategory dataNew = new Data_DisasterCategory();
                dataNew.ID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                dataNew.CategoryName = WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");

                m_arrDisaster.Add(dataNew);
            }
        }

        private bool Cancel()
        {
            m_dicVersionRegularNormal.Clear();
            m_dicVersionRegularEmergency.Clear();
            m_dicVersionNonRegularNormal.Clear();
            m_dicVersionNonRegularEmergency.Clear();

            m_dicSOPRegularNormal.Clear();
            m_dicSOPRegularEmergency.Clear();
            m_dicSOPNonRegularNormal.Clear();
            m_dicSOPNonRegularEmergency.Clear();

            m_dicActionStepInfo.Clear();

            m_arrDisaster.Clear();

            m_isOpened = false;

            return false;
        }

        // DisasterID, VersionInfo
        public Dictionary<int, VersionInfo> GetVersionDictionary(bool isRegular, bool isNormal)
        {
            if (isRegular)
            {
                if (isNormal)
                    return m_dicVersionRegularNormal;
                else
                    return m_dicVersionRegularEmergency;
            }
            else
            {
                if (isNormal)
                    return m_dicVersionNonRegularNormal;
            }

            return m_dicVersionNonRegularEmergency;
        }

        // FullPath(Category/SubCategory/Disaster), DisasterInfo
        public Dictionary<string, DisasterInfo> GetSOPDictionary(bool isRegular, bool isNormal)
        {
            if (isRegular)
            {
                if (isNormal)
                    return m_dicSOPRegularNormal;
                else
                    return m_dicSOPRegularEmergency;
            }
            else
            {
                if (isNormal)
                    return m_dicSOPNonRegularNormal;
            }

            return m_dicSOPNonRegularEmergency;
        }

        // FullPath(Category/SubCategory/Disaster), DisasterInfo List
        public Dictionary<string, ArrayList> GetSOPDisasterListDictionary(bool isRegular, bool isNormal)
        {
            if (isRegular)
            {
                if (isNormal)
                    return m_dicSOPRegularNormalDisasterList;
                else
                    return m_dicSOPRegularEmergencyDisasterList;
            }
            else
            {
                if (isNormal)
                    return m_dicSOPNonRegularNormalDisasterList;
            }

            return m_dicSOPNonRegularEmergencyDisasterList;
        }

        private bool LoadVersion()
        {
            m_dicVersionRegularNormal.Clear();
            m_dicVersionRegularEmergency.Clear();
            m_dicVersionNonRegularNormal.Clear();
            m_dicVersionNonRegularEmergency.Clear();

            string strSQL = "select version.ID, version.VersionName, version.isRegular, version.isNormal, CompanyMember.MemberName, version.CreateTime, version.LastAccessTime, version.Description, Disaster.ID ";
            strSQL += "from Version, SOPGenUser, CompanyMember, Disaster ";
            strSQL += "where version.OwnerID = SOPGenUser.ID and SOPGenUser.MemberID = CompanyMember.ID and Version.ID = Disaster.VersionID order by Version.CreateTime";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            DateTime dtDefault = new DateTime();

            int nCount = arrResult.Count;

            for (int i = 0; i < nCount - 8; i += 9)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strVersionName = WebDBManager.GetStringField(arrResult[i + 1], "");
                bool isRegular = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0) == 0 ? false : true;
                bool isNormal = WebDBManager.GetIntField(arrResult[i + 3].ToString(), 0) == 0 ? false : true;
                string strMemberName = WebDBManager.GetStringField(arrResult[i + 4], "");
                DateTime dtBegin = WebDBManager.GetDateTimeField(arrResult[i + 5], dtDefault);
                DateTime dtEnd = WebDBManager.GetDateTimeField(arrResult[i + 6], dtDefault);
                string strDesc = WebDBManager.GetStringField(arrResult[i + 7], "");
                int nDisasterID = WebDBManager.GetIntField(arrResult[i + 8].ToString(), 0);

                VersionInfo version = new VersionInfo();

                version.VersionID = nID;
                version.VersionName = strVersionName;
                version.UserName = strMemberName;
                version.BeginTime = dtBegin;
                version.EndTime = dtEnd;
                version.Description = strDesc;
                version.IsNormal = isNormal;
                version.IsRegular = isRegular;

                Dictionary<int, VersionInfo> dicVersion = GetVersionDictionary(isRegular, isNormal);
                dicVersion[nDisasterID] = version;
            }

            return true;
        }
		char szDeli = (char)0x06;
        private bool LoadSOP()
        {
            m_dicSOPRegularNormal.Clear();
            m_dicSOPRegularEmergency.Clear();
            m_dicSOPNonRegularNormal.Clear();
            m_dicSOPNonRegularEmergency.Clear();

            string strSQL = "select disaster.id, disaster.DisasterName, sc.SubCategoryName, dc.CategoryName, disaster.VersionID, Version.isRegular, Version.isNormal from disaster, SubDisasterCategory as sc, DisasterCategory as dc, Version ";
            strSQL += "where disaster.SubDisasterID = sc.id and sc.DisasterID = dc.id and disaster.VersionID = Version.ID order by DisasterName";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null) return false;

            int nCount = arrResult.Count;
            if (nCount == 0) return true;

            string strDisasterIDs = "";
            Dictionary<int, DisasterInfo> dicDisaster = new Dictionary<int, DisasterInfo>();

            for (int i = 0; i < nCount - 6; i += 7)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strDisasterName = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strSubCategoryName = WebDBManager.GetStringField(arrResult[i + 2], "");
                string strCategoryName = WebDBManager.GetStringField(arrResult[i + 3], "");
                int nVersionID = WebDBManager.GetIntField(arrResult[i + 4].ToString(), 0);
                bool isRegular = WebDBManager.GetIntField(arrResult[i + 5].ToString(), 0) == 0 ? false : true;
                bool isNormal = WebDBManager.GetIntField(arrResult[i + 6].ToString(), 0) == 0 ? false : true;

				string strFullPath = strCategoryName + szDeli + strSubCategoryName + szDeli + strDisasterName;
                DisasterInfo disaster = new DisasterInfo();
                dicDisaster[nID] = disaster;

                Dictionary<string, DisasterInfo> dicSOP = GetSOPDictionary(isRegular, isNormal);
                Dictionary<string, ArrayList> dicSOPDisasterList = GetSOPDisasterListDictionary(isRegular, isNormal);

                ArrayList arrDisasters = null;

                if (dicSOPDisasterList.ContainsKey(strFullPath))
                    arrDisasters = dicSOPDisasterList[strFullPath];
                else
                {
                    arrDisasters = new ArrayList();
                    dicSOPDisasterList[strFullPath] = arrDisasters;
                }

                arrDisasters.Add(disaster);

                /*if (dicSOP.ContainsKey(strFullPath))
                    arrDisasters = dicSOP[strFullPath];
                else
                {
                    arrDisasters = new ArrayList();
                    dicSOP[strFullPath] = arrDisasters;
                }

                arrDisasters.Add(disaster);*/

                disaster.DisasterID = nID;
                disaster.VersionID = nVersionID;

                if (strDisasterIDs.Length == 0)
                    strDisasterIDs = nID.ToString();
                else
                    strDisasterIDs += ", " + nID.ToString();

                if (dicSOP.ContainsKey(strFullPath))
                {
                    DisasterInfo disasterLatest = dicSOP[strFullPath];
                    Dictionary<int, VersionInfo> dicVersion = GetVersionDictionary(isRegular, isNormal);

                    if (dicVersion != null)
                    {
                        if (dicVersion.ContainsKey(disasterLatest.DisasterID) && dicVersion.ContainsKey(disaster.DisasterID))
                        {
                            VersionInfo versionLatest = dicVersion[disasterLatest.DisasterID];
                            VersionInfo versionCurrent = dicVersion[disaster.DisasterID];

                            if (versionLatest.BeginTime < versionCurrent.BeginTime)
                                dicSOP[strFullPath] = disaster;
                        }
                    }
                }
                else
                    dicSOP[strFullPath] = disaster;
            }

            if (strDisasterIDs.Length == 0)
                return true;

            strSQL = string.Format("select ID, StepName, PeriodType, BeginTime, EndTIme, WeekDayOption, Iteration, IterationType, ProcessTime, ProcessTimeType, DisasterID, ParentStepID from ActionStep where DisasterID in ({0})", strDisasterIDs);
            arrResult = m_dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null) return false;

            DateTime dtDefault = new DateTime();

            nCount = arrResult.Count;

            for (int i = 0; i < nCount - 9; i += 12)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strStepName = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nPeriodType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
                DateTime dtBegin = WebDBManager.GetDateTimeField(arrResult[i + 3], dtDefault);
                DateTime dtEnd = WebDBManager.GetDateTimeField(arrResult[i + 4], dtDefault);
                int nWeekdayOpt = WebDBManager.GetIntField(arrResult[i + 5].ToString(), 127);
                int nIteration = WebDBManager.GetIntField(arrResult[i + 6].ToString(), 1);
                int nIterationType = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0);
                int nProcessTime = WebDBManager.GetIntField(arrResult[i + 8].ToString(), 0);
                int nProcessTimeType = WebDBManager.GetIntField(arrResult[i + 9].ToString(), 5);
                int nDisasterID = WebDBManager.GetIntField(arrResult[i + 10].ToString(), -1);
                int nParentStepID = WebDBManager.GetIntField(arrResult[i + 11].ToString(), -1);

                if (!dicDisaster.ContainsKey(nDisasterID))
                    continue;

                ActionStepInfo actionStep = new ActionStepInfo();

                actionStep.ActionStepID = nID;
                actionStep.ActionStepName = strStepName;
                actionStep.ParentStepID = nParentStepID;
                actionStep.PeriodType = nPeriodType;
                actionStep.BeginTime = dtBegin;
                actionStep.EndTime = dtEnd;
                actionStep.WeekDayOption = nWeekdayOpt;
                actionStep.Iteration = nIteration;
                actionStep.IterationType = nIterationType;
                actionStep.ProcessTime = nProcessTime;
                actionStep.ProcessTimeType = nProcessTimeType;
                actionStep.DisasterID = nDisasterID;

                DisasterInfo disaster = dicDisaster[nDisasterID];
                disaster.ActionSteps.Add(actionStep);

                m_dicActionStepInfo[actionStep.ActionStepID] = actionStep;
            }

            SortDisasterList();

            /*DisasterCompare.m_dicVersion = _GetVersionDictionary(m_isRegular, m_isNormal);

            SortDisasterArray(m_dicSOPRegularNormal);
            SortDisasterArray(m_dicSOPRegularEmergency);
            SortDisasterArray(m_dicSOPNonRegularNormal);
            SortDisasterArray(m_dicSOPNonRegularEmergency);*/

            return true;
        }

        private void SortDisasterList()
        {
            SortDisasterList(GetSOPDisasterListDictionary(true, true));
            SortDisasterList(GetSOPDisasterListDictionary(true, false));
            SortDisasterList(GetSOPDisasterListDictionary(false, true));
            SortDisasterList(GetSOPDisasterListDictionary(false, false));
        }

        private void SortDisasterList(Dictionary<string, ArrayList> dicSOPDisasterList)
        {
            foreach (KeyValuePair<string, ArrayList> pair in dicSOPDisasterList)
            {
                pair.Value.Sort();
            }
        }

        /*private void SortDisasterArray(Dictionary<string, ArrayList> dicSOP)
        {
            DisasterCompare cmp = new DisasterCompare();

            foreach (KeyValuePair<string, ArrayList> pair in dicSOP)
            {
                ArrayList arrDisasters = pair.Value;
                arrDisasters.Sort(cmp);
            }
        }*/

        public bool IsOpened
        {
            get { return m_isOpened; }
        }

        public ArrayList CompanyMemberList
        {
            get { return m_arrCompanyMember; }
        }

        // strPhoneNumber에 빈칸이나 '-'등이 들어있을 경우 없앤다. 
        private string ValidPhoneNumber(string strPhoneNumber)
        {
            string strResult = "";
            int nLen = strPhoneNumber.Length;

            for (int i = 0; i < nLen; i++)
            {
                char ch = strPhoneNumber.ElementAt(i);

                if (ch != ' ' && ch != '\t' && ch != '-')
                    strResult += ch;
            }

            return strResult;
        }

        private bool LoadCompanyMember()
        {
            string strSQL = "select ID, MemberName, RegularTeamID, LevelID, PositionID, MemberID, SecondRegularTeamID, SecondPositionID, OfficePhoneNumber, PhoneNumber from CompanyMember";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null) return false;

            int nCount = arrResult.Count;
            if (nCount == 0) return true;

            m_arrCompanyMember.Clear();
            m_dicRegularTeamMember.Clear();

            for (int i = 0; i < nCount - 9; i += 10)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strMemberName = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nRegularTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
                int nLevelID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), 0);
                int nPositionID = WebDBManager.GetIntField(arrResult[i + 4].ToString(), 0);
                string strMemberID = WebDBManager.GetStringField(arrResult[i + 5], "");
                int nSecondRegularTeamID = WebDBManager.GetIntField(arrResult[i + 6].ToString(), 0);
                int nSecondPositionID = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0);
                string strOfficePhoneNumber = WebDBManager.GetStringField(arrResult[i + 8], "");
                //string strPhoneNumber = WebDBManager.GetStringField(arrResult[i + 9], "");
                string strPhoneNumber = WebDBManager.GetStringField(arrResult[i + 9], "");

                if (string.Compare(strPhoneNumber, "null", true) == 0 || strPhoneNumber == "")
                    strPhoneNumber = "";
                else
                    strPhoneNumber = DBUtility.AES256Cipher.AES_decrypt(strPhoneNumber, key);

                strPhoneNumber = ValidPhoneNumber(strPhoneNumber);

                if (string.Compare(strOfficePhoneNumber, "null", true) == 0)
                    strOfficePhoneNumber = "";

                Data_CompanyMember data = new Data_CompanyMember();
                data.ID = nID;
                data.MemberName = strMemberName;
                data.RegularTeamID = nRegularTeamID;
                data.LevelID = nLevelID;
                data.PositionID = nPositionID;
                data.MemberID = strMemberID;
                data.SecondRegularTeamID = nSecondRegularTeamID;
                data.SecondPositionID = nSecondPositionID;
                data.OfficePhoneNumber = strOfficePhoneNumber;
                data.PhoneNumber = strPhoneNumber;

                m_arrCompanyMember.Add(data);

                ////////////////////////////////////////////////////////////////
                // 팀별 팀원 정보 저장
                ArrayList arrTeamMemberList = null;

                if (m_dicRegularTeamMember.ContainsKey(nRegularTeamID))
                    arrTeamMemberList = m_dicRegularTeamMember[nRegularTeamID];
                else
                {
                    arrTeamMemberList = new ArrayList();
                    m_dicRegularTeamMember[nRegularTeamID] = arrTeamMemberList;
                }

                arrTeamMemberList.Add(data);
                ////////////////////////////////////////////////////////////////
            }

            return true;
        }

        private bool LoadTemporaryTeam()
        {
            // RegularTeamID, Parent Team ID
            Dictionary<int, int> dicRegularTeam = new Dictionary<int,int>();

            /////////////////////////////////////////////////////////////////////////////
            // RegularTeam의 계층 구조를 Dictionary로 만들기
            string strSQL = "select id, ParentTeamID from RegularTeam";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nParentTeamID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);

                if (nID < 0)
                    continue;

                dicRegularTeam[nID] = nParentTeamID;
            }
            /////////////////////////////////////////////////////////////////////////////

            /////////////////////////////////////////////////////////////////////////////
            // 평일 비상조직에 대한 RegularTeamID Link List 만들기
            strSQL = "select id, RegularTeamLink from TemporaryNormalTeam";
            arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strRegularTeamLink = WebDBManager.GetStringField(arrResult[i + 1], "");

                ArrayList arrTeamIDList = GetRegularTeamList(strRegularTeamLink, dicRegularTeam);
                if (arrTeamIDList != null)
                    m_dicTemporaryNormalTeamID[nID] = arrTeamIDList;
            }
            /////////////////////////////////////////////////////////////////////////////

            /////////////////////////////////////////////////////////////////////////////
            // 야간 및 휴일 비상조직에 대한 RegularTeamID Link List 만들기
            strSQL = "select id, RegularTeamLink from TemporaryEmergencyTeam";
            arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strRegularTeamLink = WebDBManager.GetStringField(arrResult[i + 1], "");

                ArrayList arrTeamIDList = GetRegularTeamList(strRegularTeamLink, dicRegularTeam);
                if (arrTeamIDList != null)
                    m_dicTemporaryEmergencyTeamID[nID] = arrTeamIDList;
            }
            /////////////////////////////////////////////////////////////////////////////

            return true;
        }

        private bool LoadOtherTeam(WebDBManager dbMgr, string strTableName, Dictionary<int, Data_ExternalTeam> dicOtherTeam)
        {
            string strSQL = string.Format("select id, TeamName, PhoneNumber, FaxNumber from {0}", strTableName);
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strTeamName = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strPhoneNumber = WebDBManager.GetStringField(arrResult[i + 2], "");
                string strFaxNumber = WebDBManager.GetStringField(arrResult[i + 3], "");

                Data_ExternalTeam data = new Data_ExternalTeam(nID, strTeamName, strPhoneNumber, strFaxNumber);
                dicOtherTeam[nID] = data;
            }

            return true;
        }

        // 사용자 정의 조직과 외부 조직 정보를 읽어온다.
        public bool LoadOtherTeams()
        {
            WebDBManager dbMgr = FormMain.Instance.DBManager;

            if (!LoadOtherTeam(dbMgr, "UserDefinedTeam", m_dicUserDefinedTeam))
                return false;
            if (!LoadOtherTeam(dbMgr, "ExternalTeam", m_dicExternalTeam))
                return false;

            return true;
        }

        // TeamID, ... 형태로 되어 있는 strTeamList를 분석하여 Team ID들을 얻어온다.
        // ID가 양수이면 하위 조직도 포함하며, 음수이면 자기 자신만 포함한다.
        // ex) 1, 1, -2, 5
        private ArrayList GetRegularTeamList(string strTeamList, Dictionary<int, int> dicRegularTeam)
        {
            int nBeginIndex = 0;
            int nLen = strTeamList.Length;

            ArrayList arrRegularTeamIDList = new ArrayList();

            while (nBeginIndex < nLen)
            {
                int nDotIndex = strTeamList.IndexOf(',', nBeginIndex);
                if (nDotIndex < 0) break;

                if (!GetRegularTeamList(strTeamList, nBeginIndex, nDotIndex, dicRegularTeam, arrRegularTeamIDList))
                    return null;
                
                nBeginIndex = nDotIndex + 1;
            }

            if (!GetRegularTeamList(strTeamList, nBeginIndex, nLen, dicRegularTeam, arrRegularTeamIDList))
                return null;

            return arrRegularTeamIDList;
        }

        private bool GetRegularTeamList(string strTeamList, int nBeginIndex, int nEndIndex, Dictionary<int, int> dicRegularTeam, ArrayList arrRegularTeamList)
        {
            if (strTeamList.Length == 0)
                return true;

            string strID = strTeamList.Substring(nBeginIndex, nEndIndex - nBeginIndex);
            strID = Utility.TrimString(strID);
			if (strID == null || strID.ToString() == "null")
				return true;
            try
            {
                int nID = int.Parse(strID);
                GetRegularTeamList(nID, dicRegularTeam, arrRegularTeamList);
            }
            catch (Exception)
            {
                return true;
            }

            return true;
        }

        private void GetRegularTeamList(int nID, Dictionary<int, int> dicRegularTeam, ArrayList arrRegularTeamList)
        {
            if (nID < 0)
                arrRegularTeamList.Add(-nID);
            else
            {
                foreach (KeyValuePair<int, int> pair in dicRegularTeam)
                {
                    if (pair.Key == nID)
                    {
                        if (!arrRegularTeamList.Contains(pair.Key))
                            arrRegularTeamList.Add(pair.Key);
                    }
                    else if (pair.Value == nID)
                    {
                        if (!arrRegularTeamList.Contains(pair.Key))
                            arrRegularTeamList.Add(pair.Key);
                        GetRegularTeamList(pair.Key, dicRegularTeam, arrRegularTeamList);
                    }
                }
            }
        }

        // 특정 비상조직에 속해있는 전체 팀원 리스트를 얻어온다.
        // 팀원들이 중복해서 입력되지는 않는다.
        // nTeamID : isNormal이 true이면 평일 비상조직, false이면 야간 및 휴일 비상조직의 ID
        // arrMemberList : 전체 팀원 리스트
        public bool GetCompanyMemberList(int nTeamID, bool isNormal, ref ArrayList arrMemberList)
        {
            //if (arrMemberList == null)
             //   return false;

            Dictionary<int, ArrayList> dicTemporaryTeamID = isNormal ? m_dicTemporaryNormalTeamID : m_dicTemporaryEmergencyTeamID;

            if (!dicTemporaryTeamID.ContainsKey(nTeamID))
                return false;

            ArrayList arrRegularTeamIDList = dicTemporaryTeamID[nTeamID];

            foreach (int nRegularTeamID in arrRegularTeamIDList)
            {
                if (nRegularTeamID < 0)
                    continue;

                if (!m_dicRegularTeamMember.ContainsKey(nRegularTeamID))
                {
                    // 조직체계상 nRegularTeamID에 해당하는 팀은 존재하지만 티원은 한명도 없는 경우
                    continue;
                }

                ArrayList arrTeamMember = m_dicRegularTeamMember[nRegularTeamID];
                
                if (arrTeamMember == null)
                    return false;

                foreach (Data_CompanyMember member in arrTeamMember)
                {
                    //arrRegularTeamIDList.Add(member);
                    arrMemberList.Add(member);
                }
            }

            return true;
        }

        public bool GetRegularCompanyMemberList(int nRegularTeamID, ref ArrayList arrMemberList)
        {
            // 조직체계상 nRegularTeamID에 해당하는 팀은 존재하지만 티원은 한명도 없는 경우
            if (!m_dicRegularTeamMember.ContainsKey(nRegularTeamID))
                return true;

            ArrayList arrTeamMember = m_dicRegularTeamMember[nRegularTeamID];

            if (arrTeamMember == null)
                return false;

            foreach (Data_CompanyMember member in arrTeamMember)
            {
                arrMemberList.Add(member);
            }

            return true;
        }

        public Data_ExternalTeam GetUserDefinedTeamMember(int nUserDefinedTeamID)
        {
            if (!m_dicUserDefinedTeam.ContainsKey(nUserDefinedTeamID))
                return null;

            return m_dicUserDefinedTeam[nUserDefinedTeamID];
        }

        public ActionStepInfo GetActionStepInfo(int nActionStepID)
        {
            if (m_dicActionStepInfo.ContainsKey(nActionStepID))
            {
                return m_dicActionStepInfo[nActionStepID];
            }

            return null;
        }

        public VersionInfo GetActionStepVersionInfo(int nActionStepID)
        {
            if (m_dicActionStepInfo.ContainsKey(nActionStepID))
            {
                ActionStepInfo actionStep = m_dicActionStepInfo[nActionStepID];

                if (m_dicVersionRegularNormal.ContainsKey(actionStep.DisasterID))
                    return m_dicVersionRegularNormal[actionStep.DisasterID];
                else if (m_dicVersionRegularEmergency.ContainsKey(actionStep.DisasterID))
                    return m_dicVersionRegularEmergency[actionStep.DisasterID];
                else if (m_dicVersionNonRegularNormal.ContainsKey(actionStep.DisasterID))
                    return m_dicVersionNonRegularNormal[actionStep.DisasterID];
                else if (m_dicVersionNonRegularEmergency.ContainsKey(actionStep.DisasterID))
                    return m_dicVersionNonRegularEmergency[actionStep.DisasterID];
            }

            return null;
        }

        public ArrayList GetSOPDisasterList(string strFullPath, bool isRegular, bool isNormal)
        {
            Dictionary<string, ArrayList> dicSOPDisasterList = GetSOPDisasterListDictionary(isRegular, isNormal);

            if (dicSOPDisasterList.ContainsKey(strFullPath))
                return dicSOPDisasterList[strFullPath];

            return null;
        }

        public Data_ExternalTeam GetUserDefinedTeam(int nTeamID)
        {
            if (!m_dicUserDefinedTeam.ContainsKey(nTeamID))
                return null;

            return m_dicUserDefinedTeam[nTeamID];
        }

        public Data_ExternalTeam GetExternalTeam(int nTeamID)
        {
            if (!m_dicExternalTeam.ContainsKey(nTeamID))
                return null;

            return m_dicExternalTeam[nTeamID];
        }

        public void SetActionStepHistoryID(int nActionStepID, bool isRealMode, int nActionStepHistoryID)
        {
            long nHi = isRealMode ? (1 << 32) : 0;
            long nLow = nActionStepID;
            long nKey = nHi | nLow;
            m_dicActionStepHistory[nKey] = nActionStepHistoryID;
        }

        public int GetActionStepHistoryID(int nActionStepID, bool isRealMode)
        {
            long nHi = isRealMode ? (1 << 32) : 0;
            long nLow = nActionStepID;
            long nKey = nHi | nLow;

            if (m_dicActionStepHistory.ContainsKey(nKey))
                return m_dicActionStepHistory[nKey];

            //string strActionStepInfo = string.Format("ActionStepID = {0}, RealMode = {1}", nActionStepID, isRealMode);
            //string strMsg = strActionStepInfo + "\r\n";
            //strMsg += "m_dicActionstepHistory Count : " + m_dicActionStepHistory.Count.ToString() + "\r\n";

            //int nCount = m_dicActionStepHistory.Count;

            //for (int i = 0; i < nCount; i++)
            //{
            //    KeyValuePair<long, int> pair = m_dicActionStepHistory.ElementAt(i);

            //    int nStepID = (int)(pair.Key & 0xffffffff);
            //    int hi = (int)((pair.Key >> 32) & 0xffffffff);

            //    bool realMode = hi == 0 ? false : true;

            //    strActionStepInfo = string.Format("index : {0}, ActionStepID = {1}, RealMode = {2}\r\n", i, nStepID, realMode);
            //    strMsg += strActionStepInfo;
            //}

            //FormMain.Instance.Invoke((MethodInvoker)delegate
            //{
            //    MessageBox.Show(strMsg);
            //}); 

            return -1;
        }

        public void SetCurrentActionStep(int nActionStepID, bool isRealMode)
        {
            m_nCurrentActionStepID = nActionStepID;
            m_isCurrentRealMode = isRealMode;
        }

        public int GetCUrrentActionStep(out bool isRealMode)
        {
            isRealMode = m_isCurrentRealMode;
            return m_nCurrentActionStepID;
        }
    }

    /*public class DisasterCompare : IComparer
    {
        public static Dictionary<int, VersionInfo> m_dicVersion = null;

        int IComparer.Compare(Object obj1, Object obj2)
        {
            if (m_dicVersion == null)
                return 0;

            DisasterInfo disaster1 = (DisasterInfo)obj1;
            DisasterInfo disaster2 = (DisasterInfo)obj2;

            if (!m_dicVersion.ContainsKey(disaster1.DisasterID))
                return 0;
            if (!m_dicVersion.ContainsKey(disaster2.DisasterID))
                return 0;

            VersionInfo version1 = m_dicVersion[disaster1.DisasterID];
            VersionInfo version2 = m_dicVersion[disaster2.DisasterID];

            if (version1.BeginTime == version2.BeginTime)
                return 0;

            return version1.BeginTime < version2.BeginTime ? -1 : 1;
        }
    }*/
}
