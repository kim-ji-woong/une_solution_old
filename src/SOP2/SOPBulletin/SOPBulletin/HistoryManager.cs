using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SOPBulletin
{
    public class HistoryManager
    {
        private WebDBManager m_dbMgr = null;
        private SOPManager m_sopMgr = null;
        private ArrayList m_arrActionStepHistory = new ArrayList();
        // long : Section Type(4Byte) + Section ID(4Byte)
        private Dictionary<long, SectionData> m_dicSections = new Dictionary<long, SectionData>();

        // 현재 실행중인 ActionStepHistory
        // ActionStepID(0보다 크면 실제 모드, 0보다 작으면 모의훈련모드), ActionStepDetailLog----------------ActionStepHistoryID
        private Dictionary<int, ActionStepDetailLog> m_dicActionStepHistory = new Dictionary<int, ActionStepDetailLog>();
        // ActionStep별 Section 객체들
        // Key : 상위 4바이트(1이면 RealMode, 0이면 가상모드), 하위 4바이트(ActionStep ID)
        // Value : Component ID별 Section 객체
        private Dictionary<long, Dictionary<int, Sections.Section>> m_dicActionStepSections = new Dictionary<long, Dictionary<int, Sections.Section>>();

        private string m_strControlUserName = "";
        private ActionStepHistoryData m_currentData = null;
        private Sections.PanelSection m_panelHidden = new Sections.PanelSection();

        public HistoryManager(WebDBManager dbMgr, SOPManager sopMgr)
        {
            m_dbMgr = dbMgr;
            m_sopMgr = sopMgr;
            m_panelHidden = new Sections.PanelSection();
            m_panelHidden.Visible = false;
        }

        public void LoadControlUser()
        {
            string strSQL = "select MemberName from sop3.dbo.CompanyMember where id in (select MemberID from sop3.dbo.SOPGenUser where ID in (select UserID from sop3.dbo.ControlUser))";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return;

            m_strControlUserName = WebDBManager.GetStringField(arrResult[0], "");
        }

        public bool CheckFinishActionStep()
        {
            DateTime dtDefault = new DateTime();

            foreach (ActionStepHistoryData data in m_arrActionStepHistory)
            {
                if (data.EndTime != null || data.CancelTime != null)
                    continue;

                string strFormat = "select id, EndTime, CancelTime from ActionStepHistory where (EndTime is not NULL or CancelTime is not NULL) ";
                strFormat += "and id = (select max(id) from ActionStepHistory where ActionStepID = {0} and RealMode = {1})";

                string strSQL = string.Format(strFormat, data.ActionStepID, data.RealMode ? 1 : 0);

                ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
                if (arrResult == null)
                    return false;

                if (arrResult.Count == 0)
                    continue;

                DateTime dtEnd = WebDBManager.GetDateTimeField(arrResult[1], dtDefault);
                DateTime dtCancel = WebDBManager.GetDateTimeField(arrResult[2], dtDefault);

                if (dtEnd != dtDefault)
                    data.EndTime = new TimeInfo(dtEnd);
                else if (dtCancel != dtDefault)
                    data.CancelTime = new TimeInfo(dtCancel);
            }

            return true;
        }

        // 초기 History Loading
        public bool LoadHistory()
        {
            LoadControlUser();

            m_dicSections.Clear();

            Dictionary<string, DisasterInfo> dicRegularNormal = m_sopMgr.GetSOPDictionary(true, true);
            Dictionary<string, DisasterInfo> dicRegularAbnormal = m_sopMgr.GetSOPDictionary(true, false);
            Dictionary<string, DisasterInfo> dicNonregularNormal = m_sopMgr.GetSOPDictionary(false, true);
            Dictionary<string, DisasterInfo> dicNonregularAbnormal = m_sopMgr.GetSOPDictionary(false, false);

            if (!LoadHistory(dicRegularNormal, true, true, true))
                return false;
            if (!LoadHistory(dicRegularNormal, false, true, true))
                return false;
            if (!LoadHistory(dicRegularAbnormal, true, true, false))
                return false;
            if (!LoadHistory(dicRegularAbnormal, false, true, false))
                return false;
            if (!LoadHistory(dicNonregularNormal, true, false, true))
                return false;
            if (!LoadHistory(dicNonregularNormal, false, false, true))
                return false;
            if (!LoadHistory(dicNonregularAbnormal, true, false, false))
                return false;
            if (!LoadHistory(dicNonregularAbnormal, false, false, false))
                return false;

            ReadCurrentActionStep();

            return true;
        }

        private void ReadCurrentActionStep()
        {
            string strSQL = "select ActionStepID, RealMode from CurrentActionStep where id = 1";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
            {
                m_currentData = null;
                return;
            }

            int nActionStepID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            bool isRealMode = WebDBManager.GetIntField(arrResult[1].ToString(), 0) == 0 ? false : true;

            foreach (ActionStepHistoryData data in m_arrActionStepHistory)
            {
                if (data.ActionStepID == nActionStepID && data.RealMode == isRealMode)
                {
                    m_currentData = data;
                    return;
                }
            }

            m_currentData = null;
        }

        private bool LoadHistory(Dictionary<string, DisasterInfo> dicData, bool isRealMode, bool isRegular, bool isNormal)
        {
            // ActionStep ID, Disaster
            Dictionary<int, DisasterInfo> dicDisaster = new Dictionary<int, DisasterInfo>();
            // Disaster, Disaster Full Path
            Dictionary<DisasterInfo, string> dicDisasterFullPath = new Dictionary<DisasterInfo, string>();

            bool isFirst = true;
            string strSQL = "select id, ActionStepID, BeginTime, Position from ActionStepHistory where EndTime is null and CancelTime is null and id in (";

            foreach (KeyValuePair<string, DisasterInfo> pair in dicData)
            {
                DisasterInfo disaster = pair.Value;
                dicDisasterFullPath[disaster] = pair.Key;

                foreach (ActionStepInfo actionStep in disaster.ActionSteps)
                {
                    dicDisaster[actionStep.ActionStepID] = disaster;

                    string strSubSQL = string.Format("(select max(id) from ActionStepHistory where BeginTime = (select max(BeginTime) from ActionStepHistory where ActionStepID = {0} and RealMode = {1}))",
                        actionStep.ActionStepID, isRealMode ? 1 : 0);

                    if (isFirst)
                        isFirst = false;
                    else
                        strSubSQL = ", " + strSubSQL;

                    strSQL += strSubSQL;
                }
            }

            if (isFirst)
                return true;

            strSQL += ")";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null) return false;

            int nResultCount = arrResult.Count;
            
            DateTime dtDefault = new DateTime();

            string strActionStepIDs = "";
            ArrayList arrHistoryID = new ArrayList();
            ArrayList arrActionStepID = new ArrayList();
            ArrayList arrBeginTime = new ArrayList();
            ArrayList arrDisaster = new ArrayList();
            ArrayList arrPosition = new ArrayList();

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nActionStepID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                DateTime dtBegin = WebDBManager.GetDateTimeField(arrResult[i + 2], dtDefault);
                string strPosition = WebDBManager.GetStringField(arrResult[i + 3], "");

                if (string.Compare(strPosition, "null", true) == 0)
                    strPosition = "-";

                if (!dicDisaster.ContainsKey(nActionStepID))
                    continue;

                DisasterInfo disaster = dicDisaster[nActionStepID];

                if (!dicDisasterFullPath.ContainsKey(disaster))
                    continue;

                if (strActionStepIDs.Length == 0)
                    strActionStepIDs = nActionStepID.ToString();
                else
                    strActionStepIDs += ", " + nActionStepID.ToString();

                arrHistoryID.Add(nID);
                arrActionStepID.Add(nActionStepID);
                arrBeginTime.Add(dtBegin);
                arrDisaster.Add(disaster);
                arrPosition.Add(strPosition);

                ActionStepDetailLog log = new ActionStepDetailLog();

                log.BeginTime = new TimeInfo(dtBegin);
                log.HistoryID = nID;
                log.IsRealMode = isRealMode;

                //SetActionStepHistory(nActionStepID, nID);
                SetActionStepHistory(nActionStepID, log);
            }

            if (!LoadActionStep(strActionStepIDs, arrHistoryID, arrActionStepID, arrBeginTime, arrDisaster, arrPosition, isRealMode, dicDisaster, dicDisasterFullPath, isRegular, isNormal))
                return false;

            return true;
        }

        private bool LoadActionStep(string strActionstepIDs, ArrayList arrActionStepHistoryID, ArrayList arrActionStepID, ArrayList arrActionStepBeginTime, ArrayList arrDisaster, ArrayList arrPosition, bool isRealMode, Dictionary<int, DisasterInfo> dicDisaster, Dictionary<DisasterInfo, string> dicDisasterFullPath, bool isRegular, bool isNormal)
        {
            if (strActionstepIDs.Length == 0)
                return true;

            // ActionStepInfo, StepMemberData List
            Dictionary<ActionStepInfo, ArrayList> dicStepMember = new Dictionary<ActionStepInfo, ArrayList>();
            //ArrayList arrActionSteps = new ArrayList();

            string strSQL = string.Format("select sm.ID, sm.TeamID, tt.TeamName, tt.LevelNo, sm.TeamType, sm.ActionStepID from StepMember as sm, TemporaryNormalTeam as tt where sm.TeamType = 0 and sm.TeamID = tt.ID and sm.ActionStepID in ({0}) order by ActionStepID",
                strActionstepIDs);
            if (!LoadActionSteps(strSQL, arrActionStepID, arrDisaster, dicStepMember))
                return false;

            strSQL = string.Format("select sm.ID, sm.TeamID, tt.TeamName, tt.LevelNo, sm.TeamType, sm.ActionStepID from StepMember as sm, TemporaryEmergencyTeam as tt where sm.TeamType = 1 and sm.TeamID = tt.ID and sm.ActionStepID in ({0}) order by ActionStepID",
                strActionstepIDs);
            if (!LoadActionSteps(strSQL, arrActionStepID, arrDisaster, dicStepMember))
                return false;

            strSQL = string.Format("select sm.ID, sm.TeamID, tt.TeamName, sm.TeamType, sm.ActionStepID from StepMember as sm, ExternalTeam as tt where sm.TeamType = 2 and sm.TeamID = tt.ID and sm.ActionStepID in ({0}) order by ActionStepID",
                strActionstepIDs);
            if (!LoadActionSteps(strSQL, arrActionStepID, arrDisaster, dicStepMember))
                return false;

            strSQL = string.Format("select sm.ID, sm.TeamID, tt.TeamName, sm.TeamType, sm.ActionStepID from StepMember as sm, UserDefinedTeam as tt where sm.TeamType = 3 and sm.TeamID = tt.ID and sm.ActionStepID in ({0}) order by ActionStepID",
                strActionstepIDs);
            if (!LoadActionSteps(strSQL, arrActionStepID, arrDisaster, dicStepMember))
                return false;

            strSQL = string.Format("select sm.ID, sm.TeamID, tt.TeamName, sm.TeamType, sm.ActionStepID from StepMember as sm, RegularTeam as tt where sm.TeamType = 4 and sm.TeamID = tt.ID and sm.ActionStepID in ({0}) order by ActionStepID",
                strActionstepIDs);
            if (!LoadActionSteps(strSQL, arrActionStepID, arrDisaster, dicStepMember))
                return false;

            foreach (KeyValuePair<ActionStepInfo, ArrayList> pair in dicStepMember)
            {
                ActionStepInfo actionStep = pair.Key;
                ArrayList arrStepMembers = pair.Value;

                ActionStepHistoryData data = LoadCurrentActionSteps(actionStep.ActionStepID, arrActionStepHistoryID, arrActionStepID, arrActionStepBeginTime, arrDisaster, arrPosition, isRealMode, dicDisaster, dicDisasterFullPath, isRegular, isNormal);
                if (data == null)
                    return false;

                long nKey = actionStep.ActionStepID;
                if (isRealMode)
                    nKey |= (1 << 32);

                Dictionary<int, Sections.Section> dicSections = null;
                bool isNewActionStep = true;

                if (m_dicActionStepSections.ContainsKey(nKey))
                {
                    dicSections = m_dicActionStepSections[nKey];
                    isNewActionStep = false;
                }
                else
                {
                    dicSections = new Dictionary<int, Sections.Section>();
                    m_dicActionStepSections[nKey] = dicSections;
                }

                IOManager ioMgr = new IOManager();

                foreach (StepMemberData stepMember in arrStepMembers)
                {
                    data.StepMembers.Add(stepMember);

                    if (isNewActionStep)
                    {
                        if (!ioMgr.LoadSections(m_dbMgr, stepMember.StepMemberID, dicSections, m_panelHidden))
                            return false;
                    }
                }

                if (!LoadComponentHistory(data, arrStepMembers, arrActionStepHistoryID, arrActionStepID, arrActionStepBeginTime, arrDisaster, isRealMode, dicDisaster, dicDisasterFullPath, isRegular, isNormal))
                    return false;
            }

            return true;
        }

        public bool GetTeamName(string strTeamList, int nBeginIndex, int nEndIndex, ArrayList arrTeamNameList)
        {
            string strToken = strTeamList.Substring(nBeginIndex, nEndIndex - nBeginIndex);

            int nIndex1 = strTeamList.IndexOf('(', nBeginIndex);
            int nIndex2 = strTeamList.IndexOf(')', nBeginIndex);

            if (nIndex1 < 0 || nIndex2 < 0)
                return false;

            string strTeamID = strTeamList.Substring(nBeginIndex, nIndex1 - nBeginIndex);
            string strTeamType = strTeamList.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);

            strTeamID = Utility.TrimString(strTeamID);
            strTeamType = Utility.TrimString(strTeamType);
            int nTeamID = -1;

            try
            {
                nTeamID = int.Parse(strTeamID);
            }
            catch (Exception)
            {
                return false;
            }

            if (strTeamType == "0")
            {
                Data_NormalTeam team = FormMain.Instance.SOPManager.GetTemporaryNormalTeam(nTeamID);
                arrTeamNameList.Add(team.TeamName);
            }
            else if (strTeamType == "1")
            {
                Data_EmergencyTeam team = FormMain.Instance.SOPManager.GetTemporaryEmergencyTeam(nTeamID);
                arrTeamNameList.Add(team.TeamName);
            }
            else if (strTeamType == "2")
            {
                Data_ExternalTeam team = FormMain.Instance.SOPManager.GetExternalTeam(nTeamID);
                arrTeamNameList.Add(team.TeamName);
            }
            else if (strTeamType == "3")
            {
                Data_ExternalTeam team = FormMain.Instance.SOPManager.GetUserDefinedTeam(nTeamID);
                arrTeamNameList.Add(team.TeamName);
            }
            else if (strTeamType == "4")
            {
                Data_RegularTeam team = FormMain.Instance.SOPManager.GetRegularTeam(nTeamID);
                arrTeamNameList.Add(team.TeamName);
            }
            else
                return false;

            return true;
        }

        private void SetProcessTeamList(int nComponentID, SectionData section, Dictionary<int, ArrayList> dicProcessTeamList)
        {
            ArrayList arrTeamNameList = null;

            if (dicProcessTeamList.ContainsKey(nComponentID))
                return;
            else
            {
                arrTeamNameList = new ArrayList();
                dicProcessTeamList[nComponentID] = arrTeamNameList;
            }

            string strSQL = string.Format("select TeamList from Process where id = {0}", nComponentID);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 1);

            if (arrResult == null || arrResult.Count == 0)
                return;

            string strTeamList = WebDBManager.GetStringField(arrResult[0], "");

            int nBeginIndex = 0;
            int nLen = strTeamList.Length;

            while (nBeginIndex < nLen)
            {
                int nDotIndex = strTeamList.IndexOf(',', nBeginIndex);
                if (nDotIndex < 0) break;

                if (!GetTeamName(strTeamList, nBeginIndex, nDotIndex, arrTeamNameList))
                    return;

                nBeginIndex = nDotIndex + 1;
            }

            if (!GetTeamName(strTeamList, nBeginIndex, nLen, arrTeamNameList))
                return;
        }

        // nActionStepID에 해당하는 Process List를 얻어온다.
        private void LoadProcessList(int nActionStepID)
        {
            if (!m_dicActionStepHistory.ContainsKey(nActionStepID))
                return;

            ActionStepDetailLog log = m_dicActionStepHistory[nActionStepID];

            string strSQL = string.Format("select id from Process where StepMemberID in (select id from StepMember where ActionStepID = {0})", nActionStepID);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return;

            foreach (object result in arrResult)
            {
                int nProcessID = WebDBManager.GetIntField(result.ToString(), 0);

                if (nProcessID > 0)
                    log.SetMissionStatus(nProcessID, ActionStepDetailLog.Status.WAITING);
            }
        }

        private bool LoadComponentHistory(ActionStepHistoryData actionStepHistory, ArrayList arrStepMembers, ArrayList arrActionStepHistoryID, ArrayList arrActionStepID, ArrayList arrActionStepBeginTime, ArrayList arrDisaster, bool isRealMode, Dictionary<int, DisasterInfo> dicDisaster, Dictionary<DisasterInfo, string> dicDisasterFullPath, bool isRegular, bool isNormal)
        {
            if (arrStepMembers.Count == 0)
                return true;

            int nActionStepID = actionStepHistory.ActionStepID;
            int nIndex = arrActionStepID.IndexOf(nActionStepID);

            if (nIndex < 0)
                return false;

            long nKey = nActionStepID;
            if (isRealMode)
                nKey |= (1 << 32);

            if (!m_dicActionStepSections.ContainsKey(nKey))
                return false;

            Dictionary<int, Sections.Section> dicSections = m_dicActionStepSections[nKey];

            LoadProcessList(nActionStepID);

            int nActionStepHistoryID = (int)arrActionStepHistoryID[nIndex];
            DateTime dtBegin = (DateTime)arrActionStepBeginTime[nIndex];
            DisasterInfo disaster = (DisasterInfo)arrDisaster[nIndex];

            string strSQL = string.Format("select ID, ComponentID, ComponentType, Time, Status, Task, CompleteCount, Description, ShowBoard from ComponentHistory where ActionStepHistoryID = {0}",
                nActionStepHistoryID);

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null) return false;

            int nResultCount = arrResult.Count;
            DateTime dtDefault = new DateTime();

            // SectionData, Section Status
            Dictionary<SectionData, int> dicSectionStatus = new Dictionary<SectionData, int>();
            ArrayList arrSections4Log = new ArrayList();
            ArrayList arrSectionStatus4Log = new ArrayList();
            ArrayList arrSectionProcessDirections4Log = new ArrayList();
            ArrayList arrDescription = new ArrayList();
            ArrayList arrTask = new ArrayList();
            ArrayList arrTime = new ArrayList();
            // 상황판에 보여줄 것인가?
            ArrayList arrShowBoard = new ArrayList();
            ArrayList arrComponentHistoryID = new ArrayList();
            // Process의 Team List
            // ComponentID, Team Name List(string)
            Dictionary<int, ArrayList> dicProcessTeamList = new Dictionary<int, ArrayList>();

            //ArrayList arrAllSections = GetAllPanelSections(arrPanels);

            for (int i = 0; i < nResultCount - 8; i += 9)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nComponentID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nComponentType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                DateTime time = WebDBManager.GetDateTimeField(arrResult[i + 3], dtDefault);
                int nStatus = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                string strTask = WebDBManager.GetStringField(arrResult[i + 5].ToString(), "");
                int nCompleteCount = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                string strDescription = WebDBManager.GetStringField(arrResult[i + 7].ToString(), "");
                bool showBoard = WebDBManager.GetIntField(arrResult[i + 8].ToString(), 0) == 0 ? false : true;

                if (nStatus == 4)       // 입력대기
                    continue;
                else if (nStatus == 5)  // 건너뛰기
                    continue;

                if (!dicSections.ContainsKey(nComponentID))
                    continue;

                Sections.Section _section = dicSections[nComponentID];

                /*Sections.Section section = FindSection(nComponentID, nComponentType, arrAllSections);
                if (section == null)
                    continue;

                section.CompleteCount = nCompleteCount;*/

                string strText = "";
                StepMemberData stepMember = FindStepMember(nComponentID, nComponentType, arrStepMembers, ref strText);

                SectionData section = new SectionData();

                section.ID = nComponentID;
                section.SectionType = (SectionData.ComponentType)nComponentType;
                section.StepMember = stepMember;
                section.Text = strText;
                section.Section = _section;

                dicSectionStatus[section] = nStatus;

                int nDirections = nStatus >> 16;
                nStatus = nStatus & 0x0000ffff;

                if (nComponentType == (int)SectionData.ComponentType.PROCESS)
                {
                    SetProcessTeamList(nComponentID, section, dicProcessTeamList);

                    if (m_dicActionStepHistory.ContainsKey(nActionStepID))
                    {
                        ActionStepDetailLog log = m_dicActionStepHistory[nActionStepID];
                        log.SetMissionStatus(nComponentID, (ActionStepDetailLog.Status)(nStatus - 1));
                    }
                }

                if (nComponentType != (int)SectionData.ComponentType.ENDPOINT)
                    strTask = strText;

                // SOP Log창 기록을 위한 List
                arrSections4Log.Add(section);
                arrSectionStatus4Log.Add(nStatus);
                arrSectionProcessDirections4Log.Add(nDirections);
                arrDescription.Add(strDescription);
                arrTask.Add(strTask);
                arrTime.Add(time);
                arrShowBoard.Add(showBoard);
                arrComponentHistoryID.Add(nID);
            }

            AddSOPSectionLog(actionStepHistory, arrComponentHistoryID, arrSections4Log, arrSectionStatus4Log, arrSectionProcessDirections4Log, arrTask, arrTime, arrDescription, arrShowBoard, isRealMode, dicProcessTeamList);

            return true;
        }

        private string GetComponentText(int nComponentID, int nComponentType)
        {
            long nComponentKey = ((long)nComponentType << 32) | (long)nComponentID;

            if (m_dicSections.ContainsKey(nComponentKey))
            {
                SectionData section = m_dicSections[nComponentKey];
                return section.Text;
            }

            string strTableName = "";

            if (nComponentType == (int)SectionData.ComponentType.DECISION)
                strTableName = "Decision";
            else if (nComponentType == (int)SectionData.ComponentType.ENDPOINT)
                strTableName = "EndPoint";
            else if (nComponentType == (int)SectionData.ComponentType.EXTERNAL)
                strTableName = "ExternalTransmission";
            else if (nComponentType == (int)SectionData.ComponentType.INTERNAL)
                strTableName = "InternalTransmission";
            else if (nComponentType == (int)SectionData.ComponentType.LINK)
                strTableName = "Link";
            else if (nComponentType == (int)SectionData.ComponentType.PROCESS)
                strTableName = "Process";
            else if (nComponentType == (int)SectionData.ComponentType.TRANSMISSION)
                strTableName = "Transmission";
            else if (nComponentType == (int)SectionData.ComponentType.TRANSSOP)
                strTableName = "TransSOP";
            else
                return "";

            string strSQL = string.Format("select text from {0} where id = {1}", strTableName, nComponentID);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return "";

            return WebDBManager.GetStringField(arrResult[0], "");
        }

        private StepMemberData FindStepMember(int nComponentID, int nComponentType, ArrayList arrStepMembers, ref string strText)
        {
            long nComponentKey = ((long)nComponentType << 32) | (long)nComponentID;

            if (m_dicSections.ContainsKey(nComponentKey))
            {
                SectionData section = m_dicSections[nComponentKey];
                strText = section.Text;
                return section.StepMember;
            }

            string strTableName = "";

            if (nComponentType == (int)SectionData.ComponentType.DECISION)
                strTableName = "Decision";
            else if (nComponentType == (int)SectionData.ComponentType.ENDPOINT)
                strTableName = "EndPoint";
            else if (nComponentType == (int)SectionData.ComponentType.EXTERNAL)
                strTableName = "ExternalTransmission";
            else if (nComponentType == (int)SectionData.ComponentType.INTERNAL)
                strTableName = "InternalTransmission";
            else if (nComponentType == (int)SectionData.ComponentType.LINK)
                strTableName = "Link";
            else if (nComponentType == (int)SectionData.ComponentType.PROCESS)
                strTableName = "Process";
            else if (nComponentType == (int)SectionData.ComponentType.TRANSMISSION)
                strTableName = "Transmission";
            else if (nComponentType == (int)SectionData.ComponentType.TRANSSOP)
                strTableName = "TransSOP";
            else
                return null;

            string strSQL = string.Format("select StepMemberID, text from {0} where id = {1}", strTableName, nComponentID);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return null;

            int nStepMemberID = WebDBManager.GetIntField(arrResult[0].ToString(), 0);
            strText = WebDBManager.GetStringField(arrResult[1], "");

            foreach (StepMemberData stepMember in arrStepMembers)
            {
                if (stepMember.StepMemberID == nStepMemberID)
                    return stepMember;
            }

            return null;
        }

        private void AddSOPSectionLog(ActionStepHistoryData actionStepHistory, int nComponentHistoryID, SectionData section, bool isRealMode, int nStatus, int nProcessDirections, ArrayList arrSections, string strTask, DateTime time, string strDescription, bool showBoard, Dictionary<int, ArrayList> dicProcessTeamList)
        {
            SectionData.State state;

            if (nStatus == 1)
                state = SectionData.State.NORMAL;
            else if (nStatus == 2)
                state = SectionData.State.RUN;
            else if (nStatus == 3)
                state = SectionData.State.DONE;
            else if (nStatus == 5)
                state = SectionData.State.SKIP;
            else
            {
                // 입력대기는 SOP Log 창에 표시하지 않는다.
                return;
            }

            AddSectionHistory(actionStepHistory, nComponentHistoryID, section, state, nProcessDirections, time, strTask, showBoard, dicProcessTeamList);
        }

        private void AddSOPSectionLog(ActionStepHistoryData actionStepHistory, ArrayList arrComponentHistoryID, ArrayList arrSections, ArrayList arrStatus, ArrayList arrProcessDirections, ArrayList arrTask, ArrayList arrTime, ArrayList arrDescription, ArrayList arrShowBoard, bool isRealMode, Dictionary<int, ArrayList> dicProcessTeamList)
        {
            int nSectionCount = arrSections.Count;

            for (int i = 0; i < nSectionCount; i++)
            {
                SectionData section = (SectionData)arrSections[i];

                int nComponentHistoryID = (int)arrComponentHistoryID[i];
                int nStatus = (int)arrStatus[i];
                int nProcessDirections = (int)arrProcessDirections[i];
                string strDescription = (string)arrDescription[i];
                string strTask = (string)arrTask[i];
                DateTime time = (DateTime)arrTime[i];
                bool showBoard = (bool)arrShowBoard[i];

                AddSOPSectionLog(actionStepHistory, nComponentHistoryID, section, isRealMode, nStatus, nProcessDirections, arrSections, strTask, time, strDescription, showBoard, dicProcessTeamList);
            }
        }

        // dicStepMember : ActionStepInfo, StepMemberData List
        private bool LoadActionSteps(string strSQL, ArrayList arrActionStepID, ArrayList arrDisaster, Dictionary<ActionStepInfo, ArrayList> dicStepMember)
        {
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null) return false;

            int nResultCount = arrResult.Count;

            int nPrevActionStepID = -2;
            int nIndex = -1;

            ArrayList arrStepMember = null;

            for (int i = 0; i < nResultCount - 5; i += 6)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nTeamID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                string strTeamName = WebDBManager.GetStringField(arrResult[i + 2].ToString(), "");
                int nLevelNo = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                int nTeamType = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                int nActionStepID = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);

                if (nActionStepID != nPrevActionStepID)
                {
                    arrStepMember = FindStepMemberList(nActionStepID, dicStepMember);

                    if (arrStepMember == null)
                    {
                        nIndex = arrActionStepID.IndexOf(nActionStepID);
                        if (nIndex < 0)
                            continue;

                        DisasterInfo disaster = (DisasterInfo)arrDisaster[nIndex];
                        ActionStepInfo actionStep = disaster.FindActionStep(nActionStepID);
                        if (actionStep == null)
                            continue;

                        arrStepMember = new ArrayList();
                        dicStepMember[actionStep] = arrStepMember;
                    }

                    nPrevActionStepID = nActionStepID;
                }

                StepMemberData data = new StepMemberData(strTeamName, nTeamID, nTeamType, nID, nLevelNo);
                arrStepMember.Add(data);
            }

            return true;
        }

        private ArrayList FindStepMemberList(int nActionStepID, Dictionary<ActionStepInfo, ArrayList> dicStepMember)
        {
            foreach (KeyValuePair<ActionStepInfo, ArrayList> pair in dicStepMember)
            {
                if (pair.Key.ActionStepID == nActionStepID)
                    return pair.Value;
            }

            return null;
        }

        private ActionStepHistoryData LoadCurrentActionSteps(int nActionStepID, ArrayList arrActionStepHistoryID, ArrayList arrActionStepID, ArrayList arrActionStepBeginTime, ArrayList arrDisaster, ArrayList arrPosition, bool isRealMode, Dictionary<int, DisasterInfo> dicDisaster, Dictionary<DisasterInfo, string> dicDisasterFullPath, bool isRegular, bool isNormal)
        {
            int nIndex = arrActionStepID.IndexOf(nActionStepID);

            if (nIndex < 0)
                return null;

            int nActionStepHistoryID = (int)arrActionStepHistoryID[nIndex];
            DateTime dtBegin = (DateTime)arrActionStepBeginTime[nIndex];
            DisasterInfo disaster = (DisasterInfo)arrDisaster[nIndex];
            string strPosition = (string)arrPosition[nIndex];

            string strDisasterPath = dicDisasterFullPath[disaster];
            string strActionStepPath = GetActionStepPath(disaster.ActionSteps, nActionStepID);

            if (strActionStepPath.Length == 0)
                return null;

            ActionStepHistoryData data = FindActionStepHistory(nActionStepHistoryID);

            if (data == null)
            {
                data = new ActionStepHistoryData();

                data.ActionStepHistoryID = nActionStepHistoryID;
                data.ActionStepID = nActionStepID;
                data.ActionStepPath = strDisasterPath + "/" + strActionStepPath;
                data.NormalMode = isNormal;
                data.RealMode = isRealMode;
                data.RegularMode = isRegular;
                data.Position = strPosition;
                data.BeginTime = new TimeInfo(dtBegin);

                // 이미 같은 SOP가 존재하면 지워준다.
                RemoveActionStepHistory(data.ActionStepID, data.RealMode);

                m_arrActionStepHistory.Add(data);
            }

            return data;
        }

        private void RemoveActionStepHistory(int nActionStepID, bool isRealMode)
        {
            foreach (ActionStepHistoryData data in m_arrActionStepHistory)
            {
                if (data.ActionStepID == nActionStepID &&
                    data.RealMode == isRealMode)
                {
                    m_arrActionStepHistory.Remove(data);
                    return;
                }
            }
        }

        public ActionStepHistoryData FindActionStepHistory(int nActionStepHistoryID)
        {
            foreach (ActionStepHistoryData data in m_arrActionStepHistory)
            {
                if (data.ActionStepHistoryID == nActionStepHistoryID)
                    return data;
            }

            return null;
        }

        private string GetActionStepPath(ArrayList arrActionSteps, int nActionStepID)
        {
            string strPath = "";

            foreach (ActionStepInfo actionStep in arrActionSteps)
            {
                if (actionStep.ActionStepID == nActionStepID)
                {
                    strPath = actionStep.ActionStepName;

                    if (actionStep.ParentStepID > 0)
                    {
                        string strParentPath = GetActionStepPath(arrActionSteps, actionStep.ParentStepID);
                        if (strParentPath.Length > 0)
                            strPath = strParentPath + "/" + strPath;
                    }

                    return strPath;
                }
            }

            return strPath;
        }

        private void SetActionStepHistory(int nActionStepID, ActionStepDetailLog log)
        {
            //m_dicActionStepHistory[nActionStepID] = nActionStepHistoryID;
            m_dicActionStepHistory[nActionStepID] = log;
        }

        private void AddDecisionHistory(ActionStepHistoryData actionStepHistory, int nComponentHistoryID, SectionData section, SectionData.State state, int nProcessDirections, DateTime time, string strTask, bool showBoard)
        {
            int nBeginPosition = -1;

            if ((nProcessDirections & (int)SectionData.ProcessDirection.TOP) == (int)SectionData.ProcessDirection.TOP)
                nBeginPosition = 0;
            else if ((nProcessDirections & (int)SectionData.ProcessDirection.BOTTOM) == (int)SectionData.ProcessDirection.BOTTOM)
                nBeginPosition = 2;
            else if ((nProcessDirections & (int)SectionData.ProcessDirection.RIGHT) == (int)SectionData.ProcessDirection.RIGHT)
                nBeginPosition = 1;
            else if ((nProcessDirections & (int)SectionData.ProcessDirection.LEFT) == (int)SectionData.ProcessDirection.LEFT)
                nBeginPosition = 3;
            else
                return;

            string strStatus = "";
            int nBeginComponentID = ((int)section.SectionType << 24) | section.ID;

            string strSQL = string.Format("select EndComponentID, Text from Arrow where BeginComponentID = {0} and BeginComponentPosition = {1} and StepMemberID = {2}",
                nBeginComponentID, nBeginPosition, section.StepMember.StepMemberID);

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult != null && arrResult.Count > 0)
            {
                int nEndComponentID = WebDBManager.GetIntField(arrResult[0].ToString(), 0);
                string strArrowText = WebDBManager.GetStringField(arrResult[1], "");

                int nEndComponentType = nEndComponentID >> 24;
                nEndComponentID = nEndComponentID & 0xffffff;
                string strEndComponentText = GetComponentText(nEndComponentID, nEndComponentType);

                strStatus = string.Format("{0}({1}) 으로 분기", strArrowText, strEndComponentText);
            }

            /*string strStatus = "-";
            if (!GetStatusString(state, ref strStatus))
                return;*/

            ComponentHistoryData data = FindComponentHistory(nComponentHistoryID, actionStepHistory);

            if (data == null)
            {
                data = new ComponentHistoryData();

                data.ActionStepHistory = actionStepHistory;
                data.ComponentHistoryID = nComponentHistoryID;
                data.Section = section;
                //data.Status = "-";
                data.Status = strStatus;
                data.Task = strTask;
                data.Time = time;
                data.Visible = showBoard;

                actionStepHistory.ComponentHistoryList.Add(data);
            }
            else
            {
                data.Task = strTask;
                data.Time = time;
                data.Visible = showBoard;
                data.Status = strStatus;
            }
        }

        private ComponentHistoryData FindComponentHistory(int nComponentHistoryID, ActionStepHistoryData actionStepHistory)
        {
            foreach (ComponentHistoryData data in actionStepHistory.ComponentHistoryList)
            {
                if (data.ComponentHistoryID == nComponentHistoryID)
                    return data;
            }

            return null;
        }

        private void AddInternalHistory(ActionStepHistoryData actionStepHistory, int nComponentHistoryID, SectionData section, SectionData.State state, DateTime time, string strTask, bool showBoard)
        {
            string strStatus = "-";
            if (!GetStatusString(state, ref strStatus))
                return;

            ComponentHistoryData data = FindComponentHistory(nComponentHistoryID, actionStepHistory);

            if (data == null)
            {
                data = new ComponentHistoryData();

                data.ActionStepHistory = actionStepHistory;
                data.ComponentHistoryID = nComponentHistoryID;
                data.Section = section;
                data.Status = strStatus;
                data.Task = strTask;
                data.Time = time;
                data.Visible = showBoard;

                actionStepHistory.ComponentHistoryList.Add(data);
            }
            else
            {
                data.Status = strStatus;
                data.Task = strTask;
                data.Time = time;
                data.Visible = showBoard;
            }
        }

        private void AddExternalHistory(ActionStepHistoryData actionStepHistory, int nComponentHistoryID, SectionData section, SectionData.State state, DateTime time, string strTask, bool showBoard)
        {
            string strStatus = "-";
            if (!GetStatusString(state, ref strStatus))
                return;

            ComponentHistoryData data = FindComponentHistory(nComponentHistoryID, actionStepHistory);

            if (data == null)
            {
                data = new ComponentHistoryData();

                data.ActionStepHistory = actionStepHistory;
                data.ComponentHistoryID = nComponentHistoryID;
                data.Section = section;
                data.Status = strStatus;
                data.Task = strTask;
                data.Time = time;
                data.Visible = showBoard;

                actionStepHistory.ComponentHistoryList.Add(data);
            }
            else
            {
                data.Status = strStatus;
                data.Task = strTask;
                data.Time = time;
                data.Visible = showBoard;
            }
        }

        private void AddTransmissionHistory(ActionStepHistoryData actionStepHistory, int nComponentHistoryID, SectionData section, SectionData.State state, DateTime time, string strTask, bool showBoard)
        {
            string strStatus = "-";
            if (!GetStatusString(state, ref strStatus))
                return;

            ComponentHistoryData data = FindComponentHistory(nComponentHistoryID, actionStepHistory);

            if (data == null)
            {
                data = new ComponentHistoryData();

                data.ActionStepHistory = actionStepHistory;
                data.ComponentHistoryID = nComponentHistoryID;
                data.Section = section;
                data.Status = strStatus;
                data.Task = strTask;
                data.Time = time;
                data.Visible = showBoard;

                actionStepHistory.ComponentHistoryList.Add(data);
            }
            else
            {
                data.Status = strStatus;
                data.Task = strTask;
                data.Time = time;
                data.Visible = showBoard;
            }
        }

        private void AddEndPointHistory(ActionStepHistoryData actionStepHistory, int nComponentHistoryID, SectionData section, DateTime time, string strTask, bool showBoard)
        {
            ComponentHistoryData data = FindComponentHistory(nComponentHistoryID, actionStepHistory);

            if (data == null)
            {
                data = new ComponentHistoryData();

                data.ActionStepHistory = actionStepHistory;
                data.ComponentHistoryID = nComponentHistoryID;
                data.Section = section;
                data.Status = "-";
                data.Task = strTask;
                data.Time = time;
                data.Visible = showBoard;

                actionStepHistory.ComponentHistoryList.Add(data);
            }
            else
            {
                data.Task = strTask;
                data.Time = time;
                data.Visible = showBoard;
            }
        }

        private void AddLinkHistory(ActionStepHistoryData actionStepHistory, int nComponentHistoryID, SectionData section, DateTime time, string strTask, bool showBoard)
        {
            ComponentHistoryData data = FindComponentHistory(nComponentHistoryID, actionStepHistory);

            if (data == null)
            {
                data = new ComponentHistoryData();

                data.ActionStepHistory = actionStepHistory;
                data.ComponentHistoryID = nComponentHistoryID;
                data.Section = section;
                data.Status = "-";
                data.Task = strTask;
                data.Time = time;
                data.Visible = showBoard;

                actionStepHistory.ComponentHistoryList.Add(data);
            }
            else
            {
                data.Task = strTask;
                data.Time = time;
                data.Visible = showBoard;
            }
        }

        private void AddProcessHistory(ActionStepHistoryData actionStepHistory, int nComponentHistoryID, SectionData section, SectionData.State state, DateTime time, string strTask, ArrayList arrTeamList, bool showBoard)
        {
            string strStatus = "-";
            if (!GetStatusString(state, ref strStatus))
                return;

            ComponentHistoryData data = FindComponentHistory(nComponentHistoryID, actionStepHistory);

            if (data == null)
            {
                data = new ComponentHistoryData();

                data.ActionStepHistory = actionStepHistory;
                data.ComponentHistoryID = nComponentHistoryID;
                data.Section = section;
                data.Status = strStatus;
                data.Task = strTask;
                data.Time = time;
                data.Visible = showBoard;

                foreach (string strTeamName in arrTeamList)
                {
                    data.TeamList.Add(strTeamName);
                }

                actionStepHistory.ComponentHistoryList.Add(data);
            }
            else
            {
                data.Status = strStatus;
                data.Task = strTask;
                data.Time = time;
                data.Visible = showBoard;
            }
        }

        private void AddTransSOPHistory(ActionStepHistoryData actionStepHistory, int nComponentHistoryID, SectionData section, DateTime time, string strTask, bool showBoard)
        {
            ComponentHistoryData data = FindComponentHistory(nComponentHistoryID, actionStepHistory);

            if (data == null)
            {
                data = new ComponentHistoryData();

                data.ActionStepHistory = actionStepHistory;
                data.ComponentHistoryID = nComponentHistoryID;
                data.Section = section;
                data.Status = "-";
                data.Task = strTask;
                data.Time = time;
                data.Visible = showBoard;

                actionStepHistory.ComponentHistoryList.Add(data);;
            }
            else
            {
                data.Task = strTask;
                data.Time = time;
                data.Visible = showBoard;
            }
        }

        private bool GetStatusString(SectionData.State state, ref string strStatus)
        {
            if (state == SectionData.State.NORMAL)        // 대기
                strStatus = "대기";
            else if (state == SectionData.State.INPUT)    // 입력 대기
            {
                strStatus = "입력대기";
                // 입력 대기는 로그를 기록하지 않는다.
                return false;
            }
            else if (state == SectionData.State.RUN)      // 실행중
                strStatus = "실행중";
            else if (state == SectionData.State.SKIP)     // 건너뛰기
                strStatus = "건너뛰기";
            else if (state == SectionData.State.DONE)     // 실행 완료
                strStatus = "완료";
            else
                return false;

            return true;
        }

        public void AddSectionHistory(ActionStepHistoryData actionStepHistory, int nComponentHistoryID, SectionData section, SectionData.State state, int nProcessDirections, DateTime time, string strTask, bool showBoard, Dictionary<int, ArrayList> dicProcessTeamList)
        {
            if (string.Compare(strTask, "null", true) == 0)
                strTask = "-";

            SectionData.ComponentType type = section.SectionType;

            if (type == SectionData.ComponentType.DECISION)
            {
                AddDecisionHistory(actionStepHistory, nComponentHistoryID, section, state, nProcessDirections, time, strTask, showBoard);
            }
            else if (type == SectionData.ComponentType.INTERNAL)
            {
                AddInternalHistory(actionStepHistory, nComponentHistoryID, section, state, time, strTask, showBoard);
            }
            else if (type == SectionData.ComponentType.EXTERNAL)
            {
                AddExternalHistory(actionStepHistory, nComponentHistoryID, section, state, time, strTask, showBoard);
            }
            else if (type == SectionData.ComponentType.TRANSMISSION)
            {
                AddTransmissionHistory(actionStepHistory, nComponentHistoryID, section, state, time, strTask, showBoard);
            }
            else if (type == SectionData.ComponentType.ENDPOINT)
            {
                AddEndPointHistory(actionStepHistory, nComponentHistoryID, section, time, strTask, showBoard);
            }
            else if (type == SectionData.ComponentType.LINK)
            {
                AddLinkHistory(actionStepHistory, nComponentHistoryID, section, time, strTask, showBoard);
            }
            else if (type == SectionData.ComponentType.PROCESS)
            {
                ArrayList arrTeamList = dicProcessTeamList.ContainsKey(section.ID) ? dicProcessTeamList[section.ID] : null;
                AddProcessHistory(actionStepHistory, nComponentHistoryID, section, state, time, strTask, arrTeamList, showBoard);
            }
            else if (type == SectionData.ComponentType.TRANSSOP)
            {
                AddTransSOPHistory(actionStepHistory, nComponentHistoryID, section, time, strTask, showBoard);
            }
        }

        public int GetProcessingMissionCount(int nActionStepID)
        {
            if (!m_dicActionStepHistory.ContainsKey(nActionStepID))
                return -1;

            ActionStepDetailLog log = m_dicActionStepHistory[nActionStepID];
            return log.ProcessingMissionCount;
        }

        public int GetSkippedMissionCount(int nActionStepID)
        {
            if (!m_dicActionStepHistory.ContainsKey(nActionStepID))
                return -1;

            ActionStepDetailLog log = m_dicActionStepHistory[nActionStepID];
            return log.SkippedMissionCount;
        }

        public int GetTotalMissionCount(int nActionStepID)
        {
            if (!m_dicActionStepHistory.ContainsKey(nActionStepID))
                return -1;

            ActionStepDetailLog log = m_dicActionStepHistory[nActionStepID];
            return log.TotalMissionCount;
        }

        public int GetCompletedMissionCount(int nActionStepID)
        {
            if (!m_dicActionStepHistory.ContainsKey(nActionStepID))
                return -1;

            ActionStepDetailLog log = m_dicActionStepHistory[nActionStepID];
            return log.CompletedMissionCount;
        }

        public ArrayList ActionStepHistoryList
        {
            get { return m_arrActionStepHistory; }
            set { m_arrActionStepHistory = value; }
        }

        public string ControlUserName
        {
            get { return m_strControlUserName; }
        }

        public ActionStepHistoryData CurrentActionStepHistory
        {
            get { return m_currentData; }
        }
    }
}
