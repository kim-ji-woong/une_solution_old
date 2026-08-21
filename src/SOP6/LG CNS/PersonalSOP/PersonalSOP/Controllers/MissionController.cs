using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Web;
using System.Web.Mvc;
using PersonalSOP.Models;
using PersonalSOP.Network;

namespace PersonalSOP.Controllers
{
    using Common;
    using DBUtility2;

    public class MissionController : Controller
    {
        /// <summary>
        /// 음수-ExternalCompanyMemberID, 양수-CompanyMemberID
        /// </summary>
        private int m_nUserID = -1;

        // key 0:TemporaryNormalTeam, 1:TemporaryEmergencyTeam, 
        private Dictionary<int, List<int>> m_dicTeamIDList = new Dictionary<int, List<int>>();
        private int m_nRegularOrExternalTeamID = -1;

        // GET: Mission
        public ActionResult Index()
        {
            return View();
        }

        private string m_strUserName = "";

        private void GetUserName()
        {
            StringBuilder sb = new StringBuilder();
            if (m_nUserID < 0)
                sb.AppendFormat("Select Name From ExternalCompanyMember Where ID={0}", m_nUserID);
            else
                sb.AppendFormat("Select MemberName From CompanyMember Where ID={0}", m_nUserID);

            ArrayList arrResult = NetworkWebManager.Instance.DBMgr.GetResultData(sb.ToString());
            if (arrResult == null || arrResult.Count == 0)
                return;

            m_strUserName = DBUtility2.WebDBManager.GetStringField(arrResult[0]);
        }

        //public ActionResult DisplayMission(int ash = -1, int uid = -1)
        public ActionResult DisplayMission(string ash = "", string uid = "")
        {
            Session[ParameterManager.MissionLastViewCount] = 0; // 초기화
            //Session[ParameterManager.ActionStepHistoryID] = ash;
            //Session[ParameterManager.UserID] = uid;

            //int nActionStepHistoryID = ash;
            //int nUserID = uid;


            int nActionStepHistoryID, nUserID;
            ParameterManager.SetAccount(ash, uid, Session, out nActionStepHistoryID, out nUserID);
            if (nActionStepHistoryID <= 0 || nUserID <= 0)
                return View(new Dictionary<int, List<ProcessMission>>());

            m_nUserID = nUserID;
            GetUserName();
            ViewData["UserName"] = m_strUserName;

            LoadRegularNExternalMemberList();
            LoadTemporaryMemberList(true);
            LoadTemporaryMemberList(false);

            int nActionStepID;
            int nStepMemberID = GetStepMemberID(nActionStepHistoryID, out nActionStepID);
            Session[ParameterManager.StepMemberID] = nStepMemberID;

            Dictionary<string, string> dicOptions = ReadActionStepHistoryOptions(nActionStepHistoryID);
            LoadProcessMission(nStepMemberID, nActionStepID, dicOptions);

            Session[ParameterManager.MissionList] = m_dicMissions;
            return View(m_dicMissions);
        }

        private Dictionary<string, string> ReadActionStepHistoryOptions(int nActionStepHistoryID)
        {
            Dictionary<string, string> dicOptions = new Dictionary<string, string>();

            string strSQL = "Select RealMode, Position, DisasterOption from ActionStepHistory where ID = " + nActionStepHistoryID.ToString();
            ArrayList arrResult = NetworkWebManager.Instance.DBMgr.GetResultData(strSQL);

            if (arrResult == null)
                return dicOptions;

            if (arrResult.Count >= 3)
            {
                VariousData<int> realMode = WebDBManager.GetIntField(arrResult[0].ToString());
                string strPosition = WebDBManager.GetStringField(arrResult[1]);
                string strOptions = WebDBManager.GetStringField(arrResult[2]);

                if (realMode == null || strPosition == null)
                    return dicOptions;

                dicOptions["{SOPFullMode}"] = realMode.Data == 1 ? "실제" : "훈련";
                dicOptions["{location}"] = strPosition;

                if (strOptions != null)
                    ParseDisasterOptions(strOptions, dicOptions);
            }

            return dicOptions;
        }

        private void ParseDisasterOptions(string strOptions, Dictionary<string, string> dicOptions)
        {
            string[] tokens = strOptions.Split(';');

            foreach(string strToken in tokens)
            {
                SetDisasterOption(strToken.Trim(), dicOptions);
            }
        }

        private void SetDisasterOption(string strOption, Dictionary<string, string> dicOptions)
        {
            int nIndex = strOption.LastIndexOf('_');
            int nIndex2 = strOption.LastIndexOf('/');

            if (nIndex > 0 && nIndex2 > nIndex)
            {
                string strTag = strOption.Substring(nIndex + 1, nIndex2 - nIndex - 1).Trim();
                string strValue = strOption.Substring(nIndex2 + 1).Trim();
                dicOptions["{" + strTag + "}"] = strValue;
            }
        }
                
        public ActionResult DisplayMission2()
        {
            Dictionary<int, List<ProcessMission>> dicMissionList = Session[ParameterManager.MissionList] as Dictionary<int, List<ProcessMission>>;
            if (dicMissionList == null || dicMissionList.Count == 0)
                return new HttpStatusCodeResult(204);

            int nActionStepHistoryID = int.Parse(Session[ParameterManager.ActionStepHistoryID].ToString());

            Dictionary<int, Dictionary<int, bool>> dicDetails = new Dictionary<int, Dictionary<int, bool>>();
            bool bRefresh = false;            

            StringBuilder sb = new StringBuilder();
            sb.Append("Select ComponentID, DataIndex, Datai ");
            sb.Append("  From ComponentHistory as ch, ComponentHistoryDetail as chd ");
            sb.Append(" Where ch.ID = chd.ComponentHistoryID ");
            sb.AppendFormat(" And ch.ActionStepHistoryID = {0} ", nActionStepHistoryID);
            sb.Append("   And ComponentType = 0 ");

            ArrayList arrResult = NetworkWebManager.Instance.DBMgr.GetResultData(sb.ToString());
            if (arrResult != null && arrResult.Count > 0)
            {
                for (int i = 0; i < arrResult.Count; i += 3)
                {
                    int nComponentID = DBUtility2.WebDBManager.GetIntField(arrResult[i].ToString(), -1); // 1:checked, 4:unChecked
                    int nDataIndex = DBUtility2.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                    int nDatai = DBUtility2.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                    bool bChecked = (nDatai == 1) ? true : false;

                    // 마지막 값만 저장
                    if (!dicDetails.ContainsKey(nComponentID))
                        dicDetails.Add(nComponentID, new Dictionary<int, bool>());
                    if (!dicDetails[nComponentID].ContainsKey(nDataIndex))
                        dicDetails[nComponentID].Add(nDataIndex, bChecked);

                    dicDetails[nComponentID][nDataIndex] = bChecked;
                }
            }

            foreach (KeyValuePair<int, List<ProcessMission>> item in dicMissionList)
            {
                int nProcessID = item.Key;
                int nIndex = 0;

                List<ProcessMission> missionList = item.Value;
                foreach (ProcessMission mission in missionList)
                {
                    bool orgChecked = mission.IsChecked;
                    foreach (KeyValuePair<int, Dictionary<int, bool>> item2 in dicDetails)
                    {
                        int nComponentID = item2.Key;

                        if (nProcessID != nComponentID)
                            continue;

                        foreach (KeyValuePair<int, bool> item3 in item2.Value)
                        {
                            if (nProcessID == nComponentID && nIndex == item3.Key)
                            {
                                if (orgChecked != item3.Value)
                                {
                                    mission.IsChecked = item3.Value;
                                    bRefresh = true;
                                }
                            }
                        }
                    }
                    nIndex++;
                }
            }

            int nMissionLastViewCount = 0;
            object missionLastViewCount = Session[ParameterManager.MissionLastViewCount];

            if (missionLastViewCount != null && missionLastViewCount is int)
                nMissionLastViewCount = (int)missionLastViewCount;
            

            if (!bRefresh && nMissionLastViewCount > 0)
            {
                return new HttpStatusCodeResult(204);
            }
            else
            {
                Session[ParameterManager.MissionLastViewCount] = dicMissionList.Count;
                return View(dicMissionList);
            }
        }

        private bool LoadRegularNExternalMemberList()
        {
            // 0(RegularTeam), 1(CompanyMember), 2(ExternalCompanyTeam), 3(ExternalTeam), 4(ExternalCompanyMember), 
            // 5(UserDefinedTeam), 6(직급, ID가 1이면 1직급, 2면 2직급 모두를 의미)
            int nMemberType = 1;
            int nUserID = m_nUserID;

            StringBuilder sb = new StringBuilder();

            // ExternalCompanyMember
            if (nUserID < 0)
            {
                nUserID = nUserID * -1;
                nMemberType = 4;

                sb.Append("Select ExternalCompanyTeamID, ExternalCompanyMemberID ");
                sb.Append("  From ExternalMemberList ");
                sb.AppendFormat("Where ExternalCompanyMemberID = {0}", nUserID);
            }
            else // CompanyMember
            {
                nMemberType = 1;

                sb.Append("Select RegularTeamID, CompanyMemberID ");
                sb.Append("  From RegularMemberList ");
                sb.AppendFormat("Where CompanyMemberID = {0}", nUserID);
            }

            ArrayList arrResult = NetworkWebManager.Instance.DBMgr.GetResultData(sb.ToString());
            if (arrResult == null || arrResult.Count == 0)
                return false;

            m_dicTeamIDList.Clear();

            for (int i = 0; i < arrResult.Count; i += 2)
            {
                int nTeamID = DBUtility2.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nMemberID = DBUtility2.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);

                m_nRegularOrExternalTeamID = nTeamID;
                LoadTeam(nTeamID, nMemberID);
            }
            return true;
        }
        
        private bool LoadTemporaryMemberList(bool isNormal)
        {
            // 0(평일 비상조직), 1(야간 비상조직), 2(ExternalCompanyTeam), 3(ExternalTeam), 4(ExternalCompanyMember), 
            // 5(UserDefinedTeam), 6(직급, ID가 1이면 1직급, 2면 2직급 모두를 의미)
            int nMemberType = isNormal ? 0 : 1;
            int nNormal = isNormal ? 1 : 0;
            string strTeamName = isNormal ? "TemporaryNormalTeam" : "TemporaryEmergencyTeam";
            int nUserID = m_nUserID;

            StringBuilder sb = new StringBuilder();

            sb.Append("Select A.TemporaryTeamID, ");

            // ExternalCompanyMember
            if (nUserID < 0)
            {
                sb.Append("(CASE C.ExternalCompanyTeamID IS NOT NULL THEN C.ExternalCompanyTeamID ");
                sb.Append("ELSE -1 END)	AS TeamID ");
                sb.Append("FROM	 TemporaryMemberList AS A ");
                sb.Append(string.Format("INNER JOIN {0} AS B ON (A.TemporaryTeamID = B.ID) ", strTeamName));
                sb.Append("LEFT JOIN	ExternalMemberList	AS C ON (A.MemberType = 4 AND A.MemberID = C.ExternalCompanyMemberID) ");
                sb.Append(string.Format("WHERE A.IsNormal = {0} and B.SiteID = {1} and ((A.MemberType = 4 AND A.MemberID = {2}) or (A.MemberType = 3 and A.MemberID = {3}))", nNormal, NetworkWebManager.Instance.DBMgr.SiteID, -nUserID, m_nRegularOrExternalTeamID));
                /*sb.Append("Select A.TemporaryTeamID, A.IsNormal ");
                sb.Append("  From TemporaryMemberList ");
                sb.AppendFormat("Where (MemberType = 3 and MemberID = {0}) or (MemberType = 4 and MemberID = {1})", m_nRegularOrExternalTeamID, -nUserID);*/
            }
            else // CompanyMember
            {
                sb.Append("(CASE WHEN D.RegularTeamID IS NOT NULL THEN D.RegularTeamID ");
                sb.Append("ELSE -1 END)	AS TeamID ");
                sb.Append("FROM	 TemporaryMemberList AS A ");
                sb.Append(string.Format("INNER JOIN {0} AS B ON (A.TemporaryTeamID = B.ID) ", strTeamName));
                sb.Append("LEFT JOIN	RegularMemberList AS D ON (A.MemberType = 1 AND A.MemberID = D.CompanyMemberID) ");
                sb.Append(string.Format("WHERE A.IsNormal = {0} and B.SiteID = {1} and ((A.MemberType = 1 AND A.MemberID = {2}) or (A.MemberType = 0 and A.MemberID = {3}))", nNormal, NetworkWebManager.Instance.DBMgr.SiteID, nUserID, m_nRegularOrExternalTeamID));
                /*sb.Append("Select MemberID, IsNormal ");
                sb.Append("  From TemporaryMemberList ");
                sb.AppendFormat("Where (MemberType = 0 and MemberID = {0}) or (MemberType = 1 and MemberID = {1})", m_nRegularOrExternalTeamID, nUserID);*/
            }

            ArrayList arrResult = NetworkWebManager.Instance.DBMgr.GetResultData(sb.ToString());
            if (arrResult == null || arrResult.Count == 0)
                return false;

            for (int i = 0; i < arrResult.Count; i += 2)
            {
                int nTeamID = DBUtility2.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                List<int> ids;

                if (m_dicTeamIDList.TryGetValue(nMemberType, out ids) == false)
                {
                    ids = new List<int>();
                    m_dicTeamIDList[nMemberType] = ids;
                }

                if (ids.Contains(nTeamID) == false)
                    ids.Add(nTeamID);

                //LoadTeam(nTeamID, nMemberID);
            }
            return true;
        }

        private bool LoadTeam(int teamID, int memberID)
        {
            //Process Table
            //--업무를 수행하는 팀들의 ID List이며 쉼표로 구분됨. ID의 Type은 괄호로 표시됨(0 : 평일 비상 조직-TemporaryNormalTeam, 1 : 휴일 비상 조직-TemporaryEmergencyTeam, 
            //--2 : 외부 기관-ExternalTeam 또는 ExternalCompanyTeam, 3 : 사용자 정의 조직 - UserDefinedTeam, 4 : 상시조직 - RegularTeam) 예: 1(0), 1(3)
            int teamKey = 4;
            string tableName = "RegularTeam";
            if (m_nUserID < 0)
            {
                tableName = "ExternalTeam";
                teamKey = 2;
            }

            string strSQL = string.Format("Select ID from {0} Where ID = {1}", tableName, teamID);

            ArrayList arrResult = NetworkWebManager.Instance.DBMgr.GetResultData(strSQL);
            if (arrResult == null || arrResult.Count == 0)
                return false;

            for (int i = 0; i < arrResult.Count; i += 1)
            {
                int nID = DBUtility2.WebDBManager.GetIntField(arrResult[i].ToString(), -1);

                if (!m_dicTeamIDList.ContainsKey(teamKey))
                    m_dicTeamIDList.Add(teamKey, new List<int>());

                m_dicTeamIDList[teamKey].Add(nID);
            }

            return true;
        }

        private int GetStepMemberID(int actionStepHistoryID, out int nActionStepID)
        {
            nActionStepID = -1;

            StringBuilder sb = new StringBuilder();
            sb.Append("Select sm.ID, sm.ActionStepID ");
            sb.Append("  From ActionStepHistory as ash, StepMember as sm ");
            sb.AppendFormat(" Where ash.ID = {0} ", actionStepHistoryID);
            sb.Append("   And ash.ActionStepID = sm.ActionStepID ");

            ArrayList arrResult = NetworkWebManager.Instance.DBMgr.GetResultData(sb.ToString());
            if (arrResult == null)
                return -1;

            if (arrResult.Count < 2)
                return -1;

            int stepMemberID = DBUtility2.WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            nActionStepID = DBUtility2.WebDBManager.GetIntField(arrResult[1].ToString(), -1);

            return stepMemberID;
        }

        private List<int> m_processIDs = new List<int>();

        private Dictionary<int, List<ProcessMission>> m_dicMissions = new Dictionary<int, List<ProcessMission>>();
        public Dictionary<int, List<ProcessMission>> DicMissions
        {
            get { return m_dicMissions; }
        }

        private void LoadProcess(int stepMemberID)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Select ID, text, TeamList From Process Where StepMemberID={0} ", stepMemberID);

            ArrayList arrResult = NetworkWebManager.Instance.DBMgr.GetResultData(sb.ToString());
            if (arrResult == null || arrResult.Count == 0)
                return;

            m_processIDs.Clear();

            for (int i = 0; i < arrResult.Count; i += 3)
            {
                int nTextID = DBUtility2.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strText = DBUtility2.WebDBManager.GetStringField(arrResult[i + 1], "");
                string strTeamList = DBUtility2.WebDBManager.GetStringField(arrResult[i + 2], "");

                DataManager.Instance.GetTeamList(strTeamList);
            }
        }

        private void LoadProcessMission(int stepMemberID, int nActionStepID, Dictionary<string, string> dicOptions)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Select p.ID, pm.id, text, missionText, p.TeamList ");
            sb.Append("  From Process as p, ProcessMission as pm ");
            sb.AppendFormat(" Where p.StepMemberID = {0} ", stepMemberID);
            sb.Append("   And p.ID = pm.processID ");
            sb.Append("   And missionText not like '#Exec%' ");

            ArrayList arrResult = NetworkWebManager.Instance.DBMgr.GetResultData(sb.ToString());
            if (arrResult == null || arrResult.Count == 0)
                return;

            m_dicMissions.Clear();

            // Section 번호에 따른 정렬을 하기위한 임시 데이터
            ProcessMission tempProcess;
            List<ProcessMission> missions;
            Dictionary<int, ProcessMission> dicTempProcess = new Dictionary<int, ProcessMission>();
            Dictionary<int, List<ProcessMission>> dicTempMissionList = new Dictionary<int, List<ProcessMission>>();

            for (int i = 0; i < arrResult.Count; i += 5)
            {
                int nTextID = DBUtility2.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nMissionTextID = DBUtility2.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                string strText = DBUtility2.WebDBManager.GetStringField(arrResult[i + 2], "");
                string strMissionText = DBUtility2.WebDBManager.GetStringField(arrResult[i + 3], "");
                string strTeamList = DBUtility2.WebDBManager.GetStringField(arrResult[i + 4], "");

                bool bContain = ContainTeam(strTeamList);

                if (!bContain)
                    continue;

                CheckDescriptionText(ref strMissionText);

                if (dicTempProcess.TryGetValue(nTextID, out tempProcess) == false)
                {
                    tempProcess = new ProcessMission();
                    tempProcess.TextID = nTextID;
                    tempProcess.SectionNumber = History.SOPHistoryManager.Instance.GetSectionNumber(nActionStepID, (int)Sections.Section.ComponentType.PROCESS, nTextID);

                    dicTempProcess[nTextID] = tempProcess;

                    missions = new List<ProcessMission>();
                    dicTempMissionList[nTextID] = missions;
                }
                else
                    missions = dicTempMissionList[nTextID];

                ProcessMission mission = new ProcessMission();
                mission.TextID = nTextID;
                mission.MissionTextID = nMissionTextID;
                mission.Text = strText;
                mission.MissionText = strMissionText;

                if (strMissionText.Contains("화재"))//test
                    mission.IsChecked = false;

                missions.Add(mission);

                /*if (!m_dicMissions.ContainsKey(nTextID))
                    m_dicMissions.Add(nTextID, new List<ProcessMission>());

                m_dicMissions[nTextID].Add(mission);*/
            }

            List<ProcessMission> sortedMissions = dicTempProcess.Values.ToList();
            sortedMissions.Sort();

            // Section 번호에 따라 순서대로 넣는다.
            foreach (ProcessMission mission in sortedMissions)
            {
                if (dicTempMissionList.TryGetValue(mission.TextID, out missions))
                {
                    SetOptions(missions, dicOptions);
                    m_dicMissions[mission.TextID] = missions;
                }
            }
        }

        private void SetOptions(List<ProcessMission> missions, Dictionary<string, string> dicOptions)
        {
            foreach (ProcessMission mission in missions)
            {
                foreach (KeyValuePair<string, string> pair in dicOptions)
                {
                    string strMissionText = mission.MissionText.Replace(pair.Key, pair.Value);
                    mission.MissionText = strMissionText;
                }
            }
        }

        private void CheckDescriptionText(ref string str)
        {
            if (str.StartsWith("설명"))
            {
                str = str.Substring("설명".Length).Trim();

                if (str.StartsWith(":"))
                    str = str.Substring(1);
            }
        }

        private bool ContainTeam(string strTeamList)
        {
            int nBeginIndex = 0;
            int nLen = strTeamList.Length;

            bool bContain = false;
            while (nBeginIndex < nLen)
            {
                int nDotIndex = strTeamList.IndexOf(',', nBeginIndex);
                if (nDotIndex < 0) break;

                bContain = ContainTeam(strTeamList, nBeginIndex, nDotIndex);

                if (bContain)
                    return true;

                nBeginIndex = nDotIndex + 1;
            }

            if (!bContain)
            {
                if (!ContainTeam(strTeamList, nBeginIndex, nLen))
                    return false;
            }

            return true;
        }
        private bool ContainTeam(string strTeamList, int nBeginIndex, int nEndIndex)
        {
            string strToken = strTeamList.Substring(nBeginIndex, nEndIndex - nBeginIndex);

            int nIndex1 = strTeamList.IndexOf('(', nBeginIndex);
            int nIndex2 = strTeamList.IndexOf(')', nBeginIndex);

            if (nIndex1 < 0 || nIndex2 < 0)
                return false;

            string strTeamID = strTeamList.Substring(nBeginIndex, nIndex1 - nBeginIndex);
            string strTeamType = strTeamList.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);

            int nTeamID;

            if (!int.TryParse(strTeamID, out nTeamID))
                return false;

            if (nTeamID < 0)
            {
                nTeamID = -nTeamID;
                //includeChildTeams = false;
            }

            foreach (KeyValuePair<int, List<int>> item in m_dicTeamIDList)
            {
                if (item.Key.ToString() == strTeamType)
                {
                    foreach (int teamID in item.Value)
                    {
                        if (nTeamID == teamID)
                            return true;
                    }
                }
            }

            return false;
        }

        [HttpPost]
        public ActionResult CheckValue(bool chk, string name)
        {
            string[] param = name.Split('_');
            int nComponentID;
            int.TryParse(param[0], out nComponentID);
            int nIndex;
            int.TryParse(param[1], out nIndex);

            UpdateStatus(chk, nComponentID, nIndex);

            return View();
        }

        private void UpdateStatus(bool chk, int nComponentID, int nIndex)
        {
            int nComponentHistoryID = NetworkWebManager.Instance.GetMaxTableID("ComponentHistory") + 1;
            int nComponentHistoryDetailID = NetworkWebManager.Instance.GetMaxTableID("ComponentHistoryDetail") + 1;

            int nActionStepHistoryID = int.Parse(Session[ParameterManager.ActionStepHistoryID].ToString());
            int nUserID = int.Parse(Session[ParameterManager.UserID].ToString());
            int nCompleteCount = GetMaxCount(nActionStepHistoryID, nComponentID) + 1;
            string strNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            StringBuilder sb = new StringBuilder();
            sb.Append("Insert Into ComponentHistory (ID, ActionStepHistoryID, ComponentID, ComponentType, Time, Status, CompleteCount, AccessedUserID) ");
            sb.AppendFormat("Values ({0}, {1}, {2}, 0, '{3}', {4}, {5}, {6})", nComponentHistoryID, nActionStepHistoryID, nComponentID, strNow, 3, nCompleteCount, nUserID);
            NetworkWebManager.Instance.DBMgr.GetResultData(sb.ToString());

            sb = new StringBuilder();
            sb.Append("Insert Into ComponentHistoryDetail (ID, ComponentHistoryID, DataIndex, Datai, Time) ");
            sb.AppendFormat("Values ({0}, {1}, {2}, {3}, '{4}')", nComponentHistoryDetailID, nComponentHistoryID, nIndex, (chk) ? 1 : 4, strNow);
            NetworkWebManager.Instance.DBMgr.GetResultData(sb.ToString());
        }

        private int GetMaxCount(int nActionStepHistoryID, int nComponentID)
        {
            string strSQL = string.Format("Select max(CompleteCount) from ComponentHistory Where ActionStepHistoryID={0} And ComponentID={1}", nActionStepHistoryID, nComponentID);
            ArrayList arrResult = NetworkWebManager.Instance.DBMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return 0;

            VariousData<int> id = DBUtility2.WebDBManager.GetIntField(arrResult[0].ToString());

            if (id == null)
                return 0;

            return id.Data;
        }
    }
}