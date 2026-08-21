using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using DBUtility2;

namespace SOPBulletin
{
    public class SOPManager
    {
        private WebDBManager m_dbMgr = null;

        // FullPath(Category/SubCategory/Disaster), DisasterInfo
        private Dictionary<string, DisasterInfo> m_dicSOPRegularNormal = new Dictionary<string, DisasterInfo>();
        private Dictionary<string, DisasterInfo> m_dicSOPRegularEmergency = new Dictionary<string, DisasterInfo>();
        private Dictionary<string, DisasterInfo> m_dicSOPNonRegularNormal = new Dictionary<string, DisasterInfo>();
        private Dictionary<string, DisasterInfo> m_dicSOPNonRegularEmergency = new Dictionary<string, DisasterInfo>();

        // DisasterID, VersionInfo
        private Dictionary<int, VersionInfo> m_dicVersionRegularNormal = new Dictionary<int, VersionInfo>();
        private Dictionary<int, VersionInfo> m_dicVersionRegularEmergency = new Dictionary<int, VersionInfo>();
        private Dictionary<int, VersionInfo> m_dicVersionNonRegularNormal = new Dictionary<int, VersionInfo>();
        private Dictionary<int, VersionInfo> m_dicVersionNonRegularEmergency = new Dictionary<int, VersionInfo>();

        // ActionStepID, ActionStepInfo
        private Dictionary<int, ActionStepInfo> m_dicActionStepInfo = new Dictionary<int, ActionStepInfo>();

        //private TreeNode m_prevSelectedNode = null;
        //private int m_nPrevSelectedRow = -1;
        private bool m_isRegular = true;
        private bool m_isNormal = true;

       // private string m_selectedCategoryName = "";
       // private string m_selectedSubCategoryName = "";
        //private string m_selectedDisasterName = "";
        //private ArrayList m_arrSelectedActionSteps = null;
        //private VersionInfo m_selectedVersion = null;
        private ArrayList m_arrCompanyMember = new ArrayList();

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
        // 평일 비상 조직 ID, 연결된 조직 Data
        private Dictionary<int, Data_NormalTeam> m_dicNormalTeam = new Dictionary<int, Data_NormalTeam>();
        // 휴일 비상 조직 ID, 연결된 조직 Data
        private Dictionary<int, Data_EmergencyTeam> m_dicEmergencyTeam = new Dictionary<int, Data_EmergencyTeam>();
        // 정규 조직 ID, 연결된 조직 Data
        private Dictionary<int, Data_RegularTeam> m_dicRegularTeam = new Dictionary<int, Data_RegularTeam>();

        private Dictionary<int, UnE.SOP.Data_ControlRoom> m_dicControlRoom = new Dictionary<int, UnE.SOP.Data_ControlRoom>();
        private Dictionary<int, UnE.SOP.Data_ControlRoomMember> m_dicControlRoomMembers = new Dictionary<int, UnE.SOP.Data_ControlRoomMember>();

        private string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

        private ArrayList m_arrDisaster = new ArrayList();
        public ArrayList DisasterList
        {
            get { return m_arrDisaster; }
            set { m_arrDisaster = value; }
        }

        public Dictionary<int, UnE.SOP.Data_ControlRoom> ControlRoom
        {
            get { return m_dicControlRoom; }
        }

        public Dictionary<int, UnE.SOP.Data_ControlRoomMember> ControlRoomMembers
        {
            get { return m_dicControlRoomMembers; }
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

            ReadDisasterCategory();

            m_isOpened = true;
            return true;
        }

        private void ReadDisasterCategory()
        {
            string strSql = "SELECT * FROM DisasterCategory WHERE SiteID = " + m_dbMgr.SiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSql);

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

        private bool LoadVersion()
        {
            m_dicVersionRegularNormal.Clear();
            m_dicVersionRegularEmergency.Clear();
            m_dicVersionNonRegularNormal.Clear();
            m_dicVersionNonRegularEmergency.Clear();

            //string strSQL = "select version.ID, version.VersionName, version.isRegular, version.isNormal, CompanyMember.MemberName, version.CreateTime, version.LastAccessTime, version.Description, Disaster.ID ";
            //strSQL += "from Version, SOPGenUser, CompanyMember, Disaster ";
            //strSQL += "where version.OwnerID = SOPGenUser.ID and SOPGenUser.MemberID = CompanyMember.ID and Version.ID = Disaster.VersionID order by Version.CreateTime";

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT v.ID, v.VersionName, v.isRegular, v.isNormal, sgu.NickName, v.CreateTime, v.LastAccessTime, v.Description, dis.ID FROM Version as v ");
            sb.Append(" INNER JOIN SOPGenUser as sgu ON v.OwnerID = sgu.ID and sgu.SiteID = {0} ");
            sb.Append(" INNER JOIN Disaster as dis ON v.ID = dis.VersionID ");
            sb.Append(" ORDER BY v.CreateTime");
            
            string strSQL = string.Format(sb.ToString(), m_dbMgr.SiteID);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

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

        private bool LoadSOP()
        {
            m_dicSOPRegularNormal.Clear();
            m_dicSOPRegularEmergency.Clear();
            m_dicSOPNonRegularNormal.Clear();
            m_dicSOPNonRegularEmergency.Clear();

            //string strSQL = "select disaster.id, disaster.DisasterName, sc.SubCategoryName, dc.CategoryName, disaster.VersionID, Version.isRegular, Version.isNormal from disaster, SubDisasterCategory as sc, DisasterCategory as dc, Version ";
            //strSQL += "where disaster.SubDisasterID = sc.id and sc.DisasterID = dc.id and disaster.VersionID = Version.ID order by DisasterName";

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT dis.id, dis.DisasterName, sc.SubCategoryName, dc.CategoryName, dis.VersionID, v.isRegular, v.isNormal FROM Disaster as dis ");
            sb.Append(" INNER JOIN SubDisasterCategory as sc ON dis.SubDisasterID = sc.id ");
            sb.Append(" INNER JOIN DisasterCategory as dc ON  sc.DisasterID = dc.id and dc.SiteID = {0} ");
            sb.Append(" INNER JOIN Version as v ON dis.VersionID = v.ID ");
            sb.Append(" ORDER BY dis.DisasterName");

            string strSQL = string.Format(sb.ToString(), m_dbMgr.SiteID);

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
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

                string strFullPath = strCategoryName + "/" + strSubCategoryName + "/" + strDisasterName;
                DisasterInfo disaster = new DisasterInfo();
                dicDisaster[nID] = disaster;

                Dictionary<string, DisasterInfo> dicSOP = GetSOPDictionary(isRegular, isNormal);
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
            arrResult = m_dbMgr.GetResultData(strSQL);
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
            return true;
        }

        public bool IsOpened
        {
            get { return m_isOpened; }
        }

        public ArrayList CompanyMemberList
        {
            get { return m_arrCompanyMember; }
        }

        private bool LoadCompanyMember()
        {
            //string strSQL = "select * from CompanyMember";
            string strSQL = string.Format("SELECT TeamID FROM Site WHERE ID = {0}", m_dbMgr.SiteID);
            ArrayList arrResult1 = m_dbMgr.GetResultData(strSQL);
            if (arrResult1 == null || arrResult1.Count == 0)
                return false;

            int nTeamID = WebDBManager.GetIntField(arrResult1[0].ToString(), -1);
            if (nTeamID == -1)
                return false;

            ArrayList arrResult2 = ExecuteTeamList(m_dbMgr, nTeamID);
            //strSQL = string.Format("sp_TeamList2 {0}", nTeamID);
            //ArrayList arrResult2 = m_dbMgr.GetStoredProcedureData(strSQL, 0);
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

            string szText = "select c.ID, c.MemberName, r.ID, c.LevelID, c.MemberID, c.PhoneNumber from RegularMemberList as l, CompanyMember as c, RegularTeam as r ";
            szText += "where l.CompanyMemberID = c.ID and l.RegularTeamID = r.ID and r.ID in ({0})";
            //string szText = "select ID, MemberName, RegularTeamID, LevelID, PositionID, MemberID, SecondRegularTeamID, SecondPositionID, PhoneNumber " +
            //                " FROM CompanyMember WHERE RegularTeamID in ({0})";

            strSQL = string.Format(szText, szTeamList);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null) return false;

            int nCount = arrResult.Count;
            if (nCount == 0) return true;

            for (int i = 0; i < nCount - 5; i += 6)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strMemberName = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nRegularTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
                int nLevelID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), 0);
                //int nPositionID = WebDBManager.GetIntField(arrResult[i + 4].ToString(), 0);
                string strMemberID = WebDBManager.GetStringField(arrResult[i + 4], "");
                //int nSecondRegularTeamID = WebDBManager.GetIntField(arrResult[i + 6].ToString(), 0);
                //int nSecondPositionID = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0);
                string strPhoneNumber = WebDBManager.GetStringField(arrResult[i + 5], "");

                Data_CompanyMember data = new Data_CompanyMember();
                data.ID = nID;
                data.MemberName = strMemberName;
                data.RegularTeamID = nRegularTeamID;
                data.LevelID = nLevelID;
                //data.PositionID = nPositionID;
                data.MemberID = strMemberID;
                //data.SecondRegularTeamID = nSecondRegularTeamID;
                //data.SecondPositionID = nSecondPositionID;
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

        private bool LoadTemporaryTeam()
        {
            // RegularTeamID, Parent Team ID
            Dictionary<int, int> dicRegularTeam = new Dictionary<int,int>();

            /////////////////////////////////////////////////////////////////////////////
            // RegularTeam의 계층 구조를 Dictionary로 만들기

            string strSQL = string.Format("SELECT TeamID FROM Site WHERE ID = {0}", m_dbMgr.SiteID);
            ArrayList arrResult1 = m_dbMgr.GetResultData(strSQL);
            if (arrResult1 == null || arrResult1.Count == 0)
                return false;

            int nTeamID = WebDBManager.GetIntField(arrResult1[0].ToString(), -1);
            if (nTeamID == -1)
                return false;

            ArrayList arrResult = ExecuteTeamList(m_dbMgr, nTeamID);
            //strSQL = string.Format("sp_TeamList2 {0}", nTeamID);
            //ArrayList arrResult = m_dbMgr.GetStoredProcedureData(strSQL, 0);
            if (arrResult == null || arrResult.Count == 0)
                return false;
            
            //string strSQL = "select id, TeamName, ParentTeamID from RegularTeam";
            //ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            //if (arrResult == null)
            //    return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strTeamName = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nParentTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);

                if (nID < 0)
                    continue;

                dicRegularTeam[nID] = nParentTeamID;

                Data_RegularTeam team = new Data_RegularTeam();

                team.ID = nID;
                team.ParentTeam = GetRegularTeam(nParentTeamID);
                team.TeamName = "";

                m_dicRegularTeam[nID] = team;
            }
            /////////////////////////////////////////////////////////////////////////////

            /////////////////////////////////////////////////////////////////////////////
            // 평일 비상조직에 대한 RegularTeamID Link List 만들기
            strSQL = "select t.ID, t.TeamName, t.ParentTeamID, t.GroupName, l.MemberID, l.MemberType from TemporaryMemberList as l, TemporaryNormalTeam as t ";
            strSQL += "where l.TemporaryTeamID = t.ID and l.IsNormal = 1 and t.SiteID = " + m_dbMgr.SiteID.ToString();
            //strSQL = "select id, TeamName, ParentTeamID, GroupName, LevelNo, RegularTeamLink from TemporaryNormalTeam WHERE SiteID = " + m_nSiteID.ToString();
            arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 5; i += 6)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strTeamName = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nParentTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                string strGroupName = WebDBManager.GetStringField(arrResult[i + 3], "");
                int nMemberID = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                int nMemberType = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                //int nLevelNo = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                //string strRegularTeamLink = WebDBManager.GetStringField(arrResult[i + 5], "");

                //ArrayList arrTeamIDList = GetRegularTeamList(strRegularTeamLink, dicRegularTeam);
                //if (arrTeamIDList != null)
                //    m_dicTemporaryNormalTeamID[nID] = arrTeamIDList;

                Data_NormalTeam team = new Data_NormalTeam();

                team.ID = nID;
                //team.LevelNo = nLevelNo;
                team.ParentTeam = GetTemporaryNormalTeam(nParentTeamID);
                team.TeamName = strTeamName;
                team.GroupName = strGroupName;

                m_dicNormalTeam[nID] = team;
            }
            /////////////////////////////////////////////////////////////////////////////

            /////////////////////////////////////////////////////////////////////////////
            // 야간 및 휴일 비상조직에 대한 RegularTeamID Link List 만들기
            strSQL = "select t.ID, t.TeamName, t.ParentTeamID, t.GroupName, l.MemberID, l.MemberType from TemporaryMemberList as l, TemporaryEmergencyTeam as t ";
            strSQL += "where l.TemporaryTeamID = t.ID and l.IsNormal = 0 and t.SiteID = " + m_dbMgr.SiteID.ToString();
            //strSQL = "select id, TeamName, ParentTeamID, GroupName, LevelNo, RegularTeamLink from TemporaryEmergencyTeam WHERE SiteID = " + m_nSiteID.ToString();
            arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 5; i += 6)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strTeamName = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nParentTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                string strGroupName = WebDBManager.GetStringField(arrResult[i + 3], "");
                int nMemberID = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                int nMemberType = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                //int nLevelNo = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                //string strRegularTeamLink = WebDBManager.GetStringField(arrResult[i + 5], "");

                //ArrayList arrTeamIDList = GetRegularTeamList(strRegularTeamLink, dicRegularTeam);
                //if (arrTeamIDList != null)
                //    m_dicTemporaryEmergencyTeamID[nID] = arrTeamIDList;

                Data_EmergencyTeam team = new Data_EmergencyTeam();

                team.ID = nID;
                //team.LevelNo = nLevelNo;
                team.ParentTeam = GetTemporaryEmergencyTeam(nParentTeamID);
                team.TeamName = strTeamName;
                team.GroupName = strGroupName;

                m_dicEmergencyTeam[nID] = team;
            }
            /////////////////////////////////////////////////////////////////////////////

            return true;
        }

        private bool LoadOtherTeam(WebDBManager dbMgr, string strTableName, Dictionary<int, Data_ExternalTeam> dicOtherTeam)
        {
            string strSQL = string.Format("select id, TeamName, PhoneNumber, FaxNumber from {0} WHERE SiteID = {1}", strTableName, m_dbMgr.SiteID);
            //string strSQL = string.Format("select id, TeamName, PhoneNumber, FaxNumber from {0}", strTableName);
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

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
            if (!LoadOtherTeam(m_dbMgr, "UserDefinedTeam", m_dicUserDefinedTeam))
                return false;
            if (!LoadOtherTeam(m_dbMgr, "ExternalTeam", m_dicExternalTeam))
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
            strID = strID.Trim();

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

        public Data_NormalTeam GetTemporaryNormalTeam(int nTeamID)
        {
            if (!m_dicNormalTeam.ContainsKey(nTeamID))
                return null;

            return m_dicNormalTeam[nTeamID];
        }

        public Data_EmergencyTeam GetTemporaryEmergencyTeam(int nTeamID)
        {
            if (!m_dicEmergencyTeam.ContainsKey(nTeamID))
                return null;

            return m_dicEmergencyTeam[nTeamID];
        }

        public Data_RegularTeam GetRegularTeam(int nTeamID)
        {
            if (!m_dicRegularTeam.ContainsKey(nTeamID))
                return null;

            return m_dicRegularTeam[nTeamID];
        }

        public bool LoadControlRoom()
        {
            m_dicControlRoom.Clear();

            string strSQL = "select cr.ID, cr.RoomType, cr.LocationName, crt.TypeName from ControlRoom as cr, ControlRoomType as crt ";
            strSQL += "where cr.RoomType = crt.ID and crt.SiteID = " + m_dbMgr.SiteID.ToString() + " order by cr.RoomType";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            //Data_ControlRoom teamRoot = GetRootControlRoomTeam(dicTeams);

            List<int> controlRoomIDs = new List<int>();
            List<int> roomTypeIDs = new List<int>();
            string strRoomTypeIDs = "";

            UnE.SOP.Data_ControlRoom root = new UnE.SOP.Data_ControlRoom();
            root.TeamName = UnE.SOP.Data_ControlRoom.ROOT_NAME;
            root.ID = UnE.SOP.Data_ControlRoom.ROOT_ID;
            root.ParentTeamID = -1;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                int nControlRoomID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nRoomTypeID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                string strLocationName = WebDBManager.GetStringField(arrResult[i + 2]);
                string strRoomType = WebDBManager.GetStringField(arrResult[i + 3]);

                if (nControlRoomID < 0 || nRoomTypeID < 0 || strLocationName == null || strRoomType == null)
                    continue;

                int nID = UnE.SOP.Data_ControlRoom.MakeID(nRoomTypeID, nControlRoomID, 0);

                UnE.SOP.Data_ControlRoom team = new UnE.SOP.Data_ControlRoom();
                team.ID = nID;
                //team.ParentTeam = teamRoot;

                team.ParentTeamID = root.ID;
                root.ChildTeams.Add(team);

                if (strLocationName == strRoomType)
                    team.TeamName = strLocationName;
                else
                    team.TeamName = strLocationName + " " + strRoomType;

                m_dicControlRoom[nID] = team;

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
            arrResult = m_dbMgr.GetResultData(strSQL);

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
                    int nID = UnE.SOP.Data_ControlRoom.MakeID(nRoomTypeID, nControlRoomID, nPositionID);

                    UnE.SOP.Data_ControlRoom team = new UnE.SOP.Data_ControlRoom();
                    team.TeamName = strJobName;
                    team.ID = nID;

                    int nParentTeamID = UnE.SOP.Data_ControlRoom.MakeID(nRoomTypeID, nControlRoomID, 0);
                    UnE.SOP.Data_ControlRoom parentTeam;

                    if (m_dicControlRoom.TryGetValue(nParentTeamID, out parentTeam))
                    {
                        m_dicControlRoom[nID] = team;
                        parentTeam.ChildTeams.Add(team);
                    }
                }
            }

            m_dicControlRoom[root.ID] = root;
            return true;
        }

        public bool LoadControlRoomMembers()
        {
            m_dicControlRoomMembers.Clear();

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT cm.ID as MemberID, cm.MemberName, cm.PhoneNumber, RoomID, TeamID, JobPosition, MemberType, cr.RoomType");
            sb.Append("  FROM controlteammembers as ctm");
            sb.Append(" INNER JOIN companymember as cm ON ctm.MemberID=cm.id");
            sb.Append(" INNER JOIN controlroom as cr ON ctm.RoomID = cr.ID");
            sb.Append(" WHERE MemberType = 1");
            sb.Append("   AND TeamID in (select teamID from controlworkingteam)");
            sb.Append(" UNION ALL ");
            sb.Append("SELECT ctm.ID as MemberID, ecm.Name as MemberName, ecm.PhoneNumber, RoomID, TeamID, JobPosition, MemberType, cr.RoomType");
            sb.Append("  FROM controlteammembers as ctm ");
            sb.Append(" INNER JOIN externalcompanymember as ecm ON ctm.MemberID=ecm.ID");
            sb.Append(" INNER JOIN controlroom as cr ON ctm.RoomID = cr.ID");
            sb.Append(" WHERE MemberType = 4");
            sb.Append("   AND TeamID in (select teamID from controlworkingteam)");

            ArrayList arrResult = m_dbMgr.GetResultData(sb.ToString());

            if (arrResult == null) return false;

            for (int i = 0; i < arrResult.Count; i += 8)
            {
                int nMemberID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strMemberName = WebDBManager.GetStringField(arrResult[i + 1]);
                string strPhoneNumber = WebDBManager.GetStringField(arrResult[i + 2]);
                int nRoomID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                int nTeamID = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                int nJobPosition = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                int nMemberType = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                int nRoomType = WebDBManager.GetIntField(arrResult[i + 7].ToString(), -1);

                if (strPhoneNumber == null)
                    strPhoneNumber = "";
                else
                    strPhoneNumber = AES256Cipher.AES_decrypt(strPhoneNumber, key);

                strPhoneNumber = ValidPhoneNumber(strPhoneNumber);

                UnE.SOP.Data_ControlRoomMember data = new UnE.SOP.Data_ControlRoomMember();
                data.MemberID = nMemberID;
                data.MemberName = strMemberName;
                data.PhoneNumber = strPhoneNumber;
                data.RoomID = nRoomID;
                data.TeamID = nTeamID;
                data.JobPosition = nJobPosition;
                data.MemberType = nMemberType;
                data.RoomType = nRoomType;

                int nID = UnE.SOP.Data_ControlRoomMember.MakeID(data.RoomType, data.RoomID, data.JobPosition);

                if (!m_dicControlRoomMembers.ContainsKey(nID))
                    m_dicControlRoomMembers.Add(nID, data);
            }

            return true;
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
