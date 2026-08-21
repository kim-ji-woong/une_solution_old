using Common.Model.History;
using dnsEmail;
using SDMS.Model.Alarm;
using SOPManager.BLL.Models.SOP;
using SOPManager.Model.Sop.Component;
using SOPManager.Model.Sop.Config;
using SOPManager.Model.Sop.Category;
using SOPSimulator.BLL.Models.Data;
using SOPSimulator.BLL.Models.Request;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using Common.Model.Option;

using static dnsData.Sensor.Facility;
using SDMS.Model.History;

namespace SOPSimulator.BLL
{
    public class SOPRunManager
    {
        /// <summary>
        /// Status_Normal = 1; Status_Run = 2; Status_Done = 3; Status_Input = 4; Status_Skip = 5;
        /// </summary>
        public enum SectionStatus { NORMAL = 1, RUN = 2, DONE = 3, INPUT = 4, SKIP = 5 }
        private ProcessManager m_processManager = null;

        private static Timer m_timerSOPProgress = null;

        private static bool m_bOnGoing = false; // 데이터 변경 작업이 진행중이면 타이머를 잠시 멈춘다
        private static bool m_bOnTimer = false; // 타이머 조회중이면 기다린다. (디버깅하기 너무 힘들다)

        private static bool m_bChanged = true; // 이력 변경 여부
        public bool Changed
        {
            get { return m_bChanged; }
            set { m_bChanged = value; }
        }

        private static int m_nChanged = 1; // 이력 변경 여부
        public int nChanged
        {
            get { return m_nChanged; }
            set { m_nChanged = value; }
        }

        /// <summary>
        /// Key : DisasterCategoryID/SubDisasterCategoryID/DisasterID/SensorZoneHistoryID
        /// </summary>
        private static Dictionary<string, SOPRunData> m_sopRunDatas = new Dictionary<string, SOPRunData>();
        public List<SOPRunData> SopRunDatas
        {
            get
            {
                List<SOPRunData> datas = new List<SOPRunData>();
                foreach (KeyValuePair<string, SOPRunData> item in m_sopRunDatas)
                {
                    datas.Add(item.Value);
                }

                return datas;
            }
        }

        private static List<int> m_confirmTimeoutCloseSOPs = new List<int>();
        public List<int> ConfirmTimeoutCloseSOPs
        {
            get { return m_confirmTimeoutCloseSOPs; }
            set { m_confirmTimeoutCloseSOPs = value; }
        }

        private static List<int> m_nLastActionStepHistoryIDs = new List<int>(); // 실행중이던 SOP 목록들        
        private static int m_lastAccessComponentHistoryID = -1;

        private static ActionStepHistory m_lastAccessedActionStep  = null; // 마지막에 조작한 SOP
        public ActionStepHistory LastAccessedActionStep
        {
            get { return m_lastAccessedActionStep; }
        }

        private static string[] m_strActionStepNames = { "관심", "주의", "경계", "심각" };

        private int m_nFireCategoryID = 0;
        private int m_nPSMCategoryID = 0;

        public bool OnGoing
        {
            get { return m_bOnGoing; }
            set { m_bOnGoing = value; }
        }

        public SOPRunManager(ProcessManager processManager)
        {
            this.m_processManager = processManager;

            InitTimer();
        }

        private void InitData()
        {
            InitActionStepNames();
            InitCategoryIDs();
        }

        private void InitCategoryIDs()
        {
            string strErrorMessage;
            List<SOPManager.Model.Sop.Category.DisasterCategory> categories = m_processManager.SopDataManager.GetSelectManager().SelectDisasterCategories(out strErrorMessage);

            if (categories != null)
            {
                foreach (var category in categories)
                {
                    if (category.CategoryName.Contains("화재"))
                        m_nFireCategoryID = category.ID;
                    else if (category.CategoryName.Contains("유출") || category.CategoryName.Contains("누출") || category.CategoryName.Contains("오염"))
                        m_nPSMCategoryID = category.ID;
                }
            }
        }

        private void InitActionStepNames()
        {
            //string strErrorMessage;
            //List<Common.Model.Option.Options> options =
            //    m_processManager.CommonDataManager.GetSelectManager().SelectOption(Common.Model.Option.Options.OptionTarget.SOPSimulator, "", out strErrorMessage);
            //if (options != null && options.Count > 0)
            //{
            //    Common.Model.Option.Options sopSimulratorOption = options[0];
            //    string[] actionStepNames = sopSimulratorOption.PropertyValue.Split(',');

            //    if (actionStepNames == null || actionStepNames.Length == 0)
            //        return;

            //    if (actionStepNames.Length != m_strActionStepNames.Length)
            //        m_strActionStepNames = new string[actionStepNames.Length];

            //    for (int i = 0; i < actionStepNames.Length; i++)
            //    {
            //        m_strActionStepNames[i] = actionStepNames[i];
            //    }
            //}
        }

        private void InitTimer()
        {
            if (m_timerSOPProgress == null)
            {
                InitData();

                m_timerSOPProgress = new Timer();
                m_timerSOPProgress.Interval = 500;
                m_timerSOPProgress.Elapsed += M_timerSOPProgress_Elapsed;
                m_timerSOPProgress.Start();
                //M_timerSOPProgress_Elapsed(null, null);
                //M_timerSOPProgress_Elapsed(null, null);
            }
        }

        private void M_timerSOPProgress_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (m_bOnTimer)
                return;

            m_bOnTimer = true;

            // 1. SOP 요청이 들어온 알람을 조회한다 (SDMS에서 상황전파를 했거나 경계 이상의 알람이 뜬 경우)
            //  - 요청이 있으면 ActionStepHistory를 추가하여 실행 상태로 만든다
            //  - 알람이 종료되면 SOP를 종료한다
            UseExcuteSOP();
            // 2. 실행중인 SOP 목록을 조회한다
            List<ActionStepHistory> histories = DisplayHistory();

            // 3. 대기시간을 초과하여 자동종료시켜야 할 SOP가 있는지 확인한다.
            //CheckTimeoutHistory(histories);

            m_bOnTimer = false;
        }

        private bool CheckTimeoutHistory(List<ActionStepHistory> histories)
        {
            if (histories == null || histories.Count == 0)
                return false;

            string strErrorMessage;
            List<Options> options = m_processManager.CommonDataManager.GetSelectManager().SelectOptions(Options.OptionTarget.SOPSimulator, out strErrorMessage);

            if (options == null)
                return false;

            // Key : Category ID. -1은 기타
            // Value : Timeout 시간(초)
            // Sop 즉시 종료
            Dictionary<int, int> dicCategoryTimeoutSeconds = new Dictionary<int, int>();
            // Sop 확인후 종료
            Dictionary<int, int> dicCategoryConfirmTimeoutSeconds = new Dictionary<int, int>();
            // 재난 탐지로 실행된 Sop 즉시 종료
            Dictionary<int, int> dicDetectCategoryTimeoutSeconds = new Dictionary<int, int>();
            // 재난 탐지로 실행된 Sop 확인후 종료
            Dictionary<int, int> dicDetectCategoryConfirmTimeoutSeconds = new Dictionary<int, int>();

            foreach (Options option in options)
            {
                if (string.Compare(option.PropertyName, "FireSOPWaitEndTime", true) == 0)
                    SetTimeoutSeconds(dicCategoryTimeoutSeconds, dicCategoryConfirmTimeoutSeconds, m_nFireCategoryID, option.PropertyValue);
                else if (string.Compare(option.PropertyName, "PSMSOPWaitEndTime", true) == 0)
                    SetTimeoutSeconds(dicCategoryTimeoutSeconds, dicCategoryConfirmTimeoutSeconds, m_nPSMCategoryID, option.PropertyValue);
                else if (string.Compare(option.PropertyName, "ETCSOPWaitEndTime", true) == 0)
                    SetTimeoutSeconds(dicCategoryTimeoutSeconds, dicCategoryConfirmTimeoutSeconds, -1, option.PropertyValue);
                else if (string.Compare(option.PropertyName, "FireSOPRecoverEndTime", true) == 0)
                    SetTimeoutSeconds(dicDetectCategoryTimeoutSeconds, dicDetectCategoryConfirmTimeoutSeconds, m_nFireCategoryID, option.PropertyValue);
                else if (string.Compare(option.PropertyName, "PSMSOPRecoverEndTime", true) == 0)
                    SetTimeoutSeconds(dicDetectCategoryTimeoutSeconds, dicDetectCategoryConfirmTimeoutSeconds, m_nPSMCategoryID, option.PropertyValue);
                else if (string.Compare(option.PropertyName, "ETCSOPRecoverEndTime", true) == 0)
                    SetTimeoutSeconds(dicDetectCategoryTimeoutSeconds, dicDetectCategoryConfirmTimeoutSeconds, -1, option.PropertyValue);
            }

            DateTime dtNow = DateTime.Now;
            bool bCloseSOP = false;

            foreach (KeyValuePair<string, SOPRunData> datas in m_sopRunDatas)
            {
                SOPRunData data = datas.Value;
                foreach (ActionStepData actionStep in data.SOPData.ActionStepDatas)
                {
                    ActionStepHistory history = actionStep._ActionStepHistory;
                    if (history == null || history.EndTime != null)
                        continue;

                    int nCategoryID = data.SOPData.DisasterCategory.ID == m_nFireCategoryID || data.SOPData.DisasterCategory.ID == m_nPSMCategoryID ? data.SOPData.DisasterCategory.ID : -1;
                    int nSeconds;

                    if (data.SensorZoneHistoryID == null)
                    {
                        if (dicCategoryTimeoutSeconds.TryGetValue(nCategoryID, out nSeconds))
                        {
                            if (CheckTimeoutSeconds(dtNow, nSeconds, history))
                                CloseSOP(history);
                        }
                        else if (dicCategoryConfirmTimeoutSeconds.TryGetValue(nCategoryID, out nSeconds))
                        {
                            if (CheckTimeoutSeconds(dtNow, nSeconds, history))
                            {
                                if (!m_confirmTimeoutCloseSOPs.Contains(history.ID))
                                {
                                    //System.Diagnostics.Trace.WriteLine("확인후 종료할 것 : " + history.ID);                            
                                    m_confirmTimeoutCloseSOPs.Add(history.ID);
                                    bCloseSOP = true;
                                }
                            }
                        }  
                    }
                    else
                    {
                        Dictionary<CurrentAlarm.Fields, object> dicCondition = new Dictionary<CurrentAlarm.Fields, object>();
                        dicCondition.Add(CurrentAlarm.Fields.SensorZoneHistoryID, data.SensorZoneHistoryID);

                        List<CurrentAlarm> alarms = m_processManager.SdmsManager.GetSelectManager().SelectCurrentAlarms(dicCondition, "", out strErrorMessage);
                        if (alarms != null && alarms.Count == 0)
                        {
                            Dictionary<SensorReactionHistory.Fields, object> dicCondition2 = new Dictionary<SensorReactionHistory.Fields, object>();
                            dicCondition2.Add(SensorReactionHistory.Fields.SensorZoneHistoryID, data.SensorZoneHistoryID);

                            List<SensorReactionHistory> reactionHistories = m_processManager.SdmsManager.GetSelectManager().SelectSensorReactionHistories(dicCondition2, "ReactionType in (21,50,64)", out strErrorMessage);
                            if (reactionHistories != null && reactionHistories.Count > 0)
                            {                                
                                if (dicDetectCategoryTimeoutSeconds.TryGetValue(nCategoryID, out nSeconds))
                                {
                                    if (CheckTimeoutDetectSeconds(dtNow, nSeconds, reactionHistories[0].Time))
                                        CloseSOP(history);

                                }
                                else if (dicDetectCategoryConfirmTimeoutSeconds.TryGetValue(nCategoryID, out nSeconds))
                                {
                                    if (CheckTimeoutDetectSeconds(dtNow, nSeconds, reactionHistories[0].Time))
                                    {
                                        if (!m_confirmTimeoutCloseSOPs.Contains(history.ID))
                                        {
                                            //System.Diagnostics.Trace.WriteLine("확인후 종료할 것 : " + history.ID);                            
                                            m_confirmTimeoutCloseSOPs.Add(history.ID);
                                            bCloseSOP = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return bCloseSOP;
        }

        private bool CheckTimeoutSeconds(DateTime dtNow, int nSeconds, ActionStepHistory history)
        {
            DateTime dtPrev = history.LastAccessedTime != null ? (DateTime)history.LastAccessedTime : history.BeginTime;
            TimeSpan span = dtNow - dtPrev;
            double time = span.TotalSeconds;

            if (time >= nSeconds)
                return true;

            return false;
        }

        private bool CheckTimeoutDetectSeconds(DateTime dtNow, int nSeconds, DateTime dtDetectEndTime)
        {
            DateTime dtPrev = dtDetectEndTime;
            TimeSpan span = dtNow - dtPrev;
            double time = span.TotalSeconds;

            if (time >= nSeconds)
                return true;

            return false;
        }

        private void SetTimeoutSeconds(Dictionary<int, int> dicCategoryTimeoutSeconds, Dictionary<int, int> dicCategoryConfirmTimeoutSeconds, int nCategoryID, string strValue)
        {
            if (strValue == null)
                return;

            string[] tokens = strValue.Split(';');

            if (tokens.Length < 3)
                return;

            int nTime, nType, nMode;
            int nSeconds = -1;

            if (int.TryParse(tokens[0].Trim(), out nTime) && int.TryParse(tokens[1].Trim(), out nType) && int.TryParse(tokens[2].Trim(), out nMode))
            {
                if (nType == 0)
                {
                    // second
                    nSeconds = nTime;
                }
                else if (nType == 1)
                {
                    // minute
                    nSeconds = nTime * 60;
                }
                else if (nType == 2)
                {
                    // hour
                    nSeconds = nTime * 3600;
                }
                else
                    return;

                if (nMode == 0)
                {
                    // 자동종료
                    dicCategoryTimeoutSeconds[nCategoryID] = nSeconds;
                }
                else if (nMode == 1)
                {
                    // 확인후 종료
                    dicCategoryConfirmTimeoutSeconds[nCategoryID] = nSeconds;
                }
            }
        }

        private List<ActionStepHistory> DisplayHistory()
        {
            if (m_bOnGoing)
                return null;

            // 1. 실행중인 SOP 목록을 조회한다            
            string strErrorMessage = null;

            Common.IDAL.ISelect select = m_processManager.CommonDataManager.GetSelectManager();
            SOPManager.IDAL.ISelect selectSop = m_processManager.SopDataManager.GetSelectManager();

            string strCondition = "EndTime IS NULL";
            string actionStepHistoryIDs = string.Join(", ", m_nLastActionStepHistoryIDs);
            
            if (actionStepHistoryIDs.Length > 0)
            {
                // 실행 상태였던 SOP가 여전히 실행중인지도 조회한다
                strCondition += " or ID in (" + actionStepHistoryIDs + ")";
            }

            List<ActionStepHistory> histories = select.SelectActionStepHistories(strCondition, out strErrorMessage);
            if (histories == null)
                return histories;

            if (histories.Count == 0)
            {
                m_lastAccessedActionStep = null;
                m_nLastActionStepHistoryIDs.Clear();
            }

            SOPManager.BLL.ProcessManager processMgr =
                new SOPManager.BLL.ProcessManager(m_processManager.CommonDataManager, m_processManager.SopDataManager, m_processManager.TeamDataManager, m_processManager.SdmsManager);

            Dictionary<string, SOPRunData> dicTemp = CopySOPRunDatas();     

            List<int> deleteHistoryIDs = new List<int>();
            List<int> historyIDs = new List<int>();

            ActionStepHistory lastHistory = new ActionStepHistory();

            SOPManager.BLL.LoadManager sopLoadManager = processMgr.GetLoadManager();

            foreach (ActionStepHistory history in histories)
            {
                if (history.EndTime != null)
                {
                    deleteHistoryIDs.Add(history.ID);
                    continue;
                }

                if (lastHistory.LastAccessedTime == null || lastHistory.LastAccessedTime < history.LastAccessedTime)
                    lastHistory = history;

                int versionID;
                string strKey = MakeKey(selectSop, history.ActionStepID, out versionID);
                if (strKey.Length == 0)
                    continue;

                if (versionID == -1)
                    continue;

                if (history.SensorZoneHistoryID != null)
                    strKey += "/" + history.SensorZoneHistoryID;

                if (!dicTemp.ContainsKey(strKey))
                {
                    dicTemp.Add(strKey, new SOPRunData());

                    SOPManager.BLL.Models.Response.ResponseOpen res = sopLoadManager.OpenDB(versionID);                    
                    if (!res.Success)
                        continue;

                    SOPRunData data = new SOPRunData();
                    data.Key = strKey;
                    data.SensorZoneHistoryID = history.SensorZoneHistoryID;
                    data.Position = history.Position;
                    data.SOPData = res.SOPData;
                    
                    foreach (ActionStepData actionStepData in res.SOPData.ActionStepDatas)
                    {
                        if (actionStepData.ActionStep == null)
                            continue;

                        if (actionStepData.StepMemberDatas != null && actionStepData.StepMemberDatas.Count > 0)
                        {
                            foreach (SectionData item in actionStepData.StepMemberDatas[0].Sections)
                            {
                                item.Message = ReplaceMessage(item.Message, history.Position, history.BeginTime.ToString(), sopLoadManager);
                            }
                        }

                        if (actionStepData.ActionStep.ID == history.ActionStepID)
                        {
                            actionStepData._ActionStepHistory = history;

                            // 수동 시작한 경우 진행중 임무를 만든다.
                            List<ComponentHistory> componentHistories = select.SelectComponentHistories("ActionStepHistoryID=" + history.ID, out strErrorMessage);
                            if (componentHistories != null && componentHistories.Count == 0)
                            {
                                SectionData fristSection = actionStepData.StepMemberDatas[0].Sections[0];
                                CheckAutoSection(actionStepData.StepMemberDatas[0].Arrows, actionStepData.StepMemberDatas[0].Sections, fristSection.ComponentType, fristSection.ID, fristSection.Text, "", history);
                            }
                            break;
                        }
                    }
                    dicTemp[strKey] = data;
                }
                else
                {
                    if (!m_nLastActionStepHistoryIDs.Contains(history.ID)) // 새로 시작한 SOP만
                    {
                        SOPRunData sopRunData = dicTemp[strKey];
                        
                        foreach (ActionStepData actionStepData in sopRunData.SOPData.ActionStepDatas)
                        {
                            if (actionStepData.ActionStep == null)
                                continue;

                            if (actionStepData._ActionStepHistory == null && actionStepData.ActionStep.ID == history.ActionStepID)
                            {
                                actionStepData._ActionStepHistory = history;

                                if (actionStepData.StepMemberDatas != null && actionStepData.StepMemberDatas.Count > 0)
                                {
                                    foreach (SectionData item in actionStepData.StepMemberDatas[0].Sections)
                                    {
                                        item.Message = ReplaceMessage(item.Message, history.Position, history.BeginTime.ToString(), sopLoadManager);
                                    }
                                }

                                // 수동 시작한 경우 진행중 임무를 만든다.
                                List<ComponentHistory> componentHistories = select.SelectComponentHistories("ActionStepHistoryID=" + history.ID, out strErrorMessage);
                                if (componentHistories != null && componentHistories.Count == 0)
                                {
                                    SectionData fristSection = actionStepData.StepMemberDatas[0].Sections[0];
                                    CheckAutoSection(actionStepData.StepMemberDatas[0].Arrows, actionStepData.StepMemberDatas[0].Sections, fristSection.ComponentType, fristSection.ID, fristSection.Text, "", history);
                                }
                                break;
                            }
                        } 
                    }
                }

                historyIDs.Add(history.ID);
            }

            // 종료된 SOP가 있는지 ?
            int deleteCount = deleteHistoryIDs.Count;
            List<string> deleteKeys = new List<string>();
            for (int i = 0; i < deleteCount; i++)
            {                
                foreach (KeyValuePair<string, SOPRunData> item in dicTemp)
                {
                    bool check = false;
                    foreach (ActionStepData actionStepData in item.Value.SOPData.ActionStepDatas)
                    {
                        if (actionStepData._ActionStepHistory != null)
                        {
                            if (actionStepData._ActionStepHistory.ID == deleteHistoryIDs[i])
                            {
                                actionStepData._ActionStepHistory = null;
                                actionStepData.ComponentHistoryData = null;
                                actionStepData.CurrentSection = null;
                            }
                            // ComponentHistory 기록도 삭제해야 할까?
                        }

                        if (actionStepData._ActionStepHistory != null)
                            check = true;
                    }

                    if (!check)
                        deleteKeys.Add(item.Key); // 실행중인 SOP 한 개도 없다면 제거한다
                }
            }

            int deleteKeysCount = deleteKeys.Count;
            for (int i = 0; i < deleteKeysCount; i++)
            {
                dicTemp.Remove(deleteKeys[i]);
            }

            actionStepHistoryIDs = string.Join(", ", historyIDs);
            bool chgComponentHistory = false;
            // 2. 마지막 m_nLastComponentHistoryID 이후로 기록된 History가 있는지 조회한다 (없으면 변경사항 없는것)
            if (actionStepHistoryIDs.Length > 0)
            {
                strCondition = "ActionStepHistoryID In (" + actionStepHistoryIDs + ") and ID > " + m_lastAccessComponentHistoryID;
                List<ComponentHistory> componentHistories = select.SelectComponentHistories(strCondition, out strErrorMessage);

                if (componentHistories == null)
                    return histories;

                strCondition = "ComponentHistoryID > " + m_lastAccessComponentHistoryID;
                List<ComponentHistoryDetail> componentHistoryDetails = select.SelectComponentHistoryDetails(strCondition, out strErrorMessage);
                if (componentHistoryDetails == null)
                    return histories;

                foreach (ComponentHistory componentHistory in componentHistories)
                {
                    foreach (KeyValuePair<string, SOPRunData> item in dicTemp)
                    {
                        foreach (ActionStepData actionStepData in item.Value.SOPData.ActionStepDatas)
                        {
                            if (actionStepData._ActionStepHistory == null)
                                continue;

                            if (actionStepData._ActionStepHistory.ID == componentHistory.ActionStepHistoryID)
                            {
                                if (actionStepData.ComponentHistoryData == null)
                                    actionStepData.ComponentHistoryData = new List<ComponentHistoryData>();

                                ComponentHistoryData data2 = new ComponentHistoryData();
                                data2.ComponentHistory = componentHistory;

                                m_lastAccessComponentHistoryID = componentHistory.ID;
                                chgComponentHistory = true;

                                actionStepData.ComponentHistoryData.Add(data2);

                                foreach (ComponentHistoryDetail detail in componentHistoryDetails)
                                {
                                    if (componentHistory.ID == detail.ComponentHistoryID)
                                    {
                                        // 임무 체크, 상황 전파 기록 담아준다
                                        data2._ComponentHistoryDetails = detail;

                                        if (actionStepData.StepMemberDatas != null || actionStepData.StepMemberDatas.Count > 0)
                                        {
                                            if (detail.Datai > 1) // 0, 1값인 것만 체크한다
                                                continue;

                                            StepMemberData stepMemberData = actionStepData.StepMemberDatas[0];

                                            foreach (SectionData section in stepMemberData.Sections)
                                            {
                                                if (section.ID == componentHistory.ComponentID && section.ComponentType == componentHistory.ComponentType)
                                                {
                                                    bool allCheck = true; // 모든 임무가 체크됐는지 확인
                                                    bool isChecked = (detail.Datai != null && detail.Datai == 1) ? true : false;
                                                    SetCheckedMission(section, detail.DataIndex, isChecked, ref allCheck);
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }

                                if (componentHistory.Status == 2)
                                {
                                    foreach (SectionData sectionData in actionStepData.StepMemberDatas[0].Sections)
                                    {
                                        if (sectionData.ComponentType == componentHistory.ComponentType && sectionData.ID == componentHistory.ComponentID)
                                        {
                                            actionStepData.CurrentSection = sectionData;
                                        }
                                    }
                                }

                                break;
                            }
                        }
                    }
                }

                //if (componentHistories.Count > 0)
                //{
                //    m_lastAccessComponentHistoryID = componentHistories[componentHistories.Count - 1].ID;
                //    chgComponentHistory = true;                    
                //}
            }

            m_sopRunDatas = dicTemp;
            m_nLastActionStepHistoryIDs = historyIDs;
            m_lastAccessedActionStep = lastHistory;

            bool bCloseSOP = CheckTimeoutHistory(histories);
            if (deleteCount > 0 || chgComponentHistory || bCloseSOP) // 종료된 SOP가 있거나 진행이력이 있다면
            {
                m_bChanged = true;
                m_nChanged = new Random().Next();
            }
            else
                m_bChanged = false;

            return histories;
        }

        private Dictionary<string, SOPRunData> CopySOPRunDatas()
        {
            Dictionary<string, SOPRunData> dicTemp = new Dictionary<string, SOPRunData>();

            foreach (KeyValuePair<string, SOPRunData> item in m_sopRunDatas)
            {
                if (!dicTemp.ContainsKey(item.Key))
                {
                    dicTemp.Add(item.Key, new SOPRunData());
                }

                dicTemp[item.Key].Key = item.Value.Key;
                dicTemp[item.Key].Position = item.Value.Position;
                dicTemp[item.Key].SensorZoneHistoryID = item.Value.SensorZoneHistoryID;                
                dicTemp[item.Key].SOPData = (SOPData)item.Value.SOPData.Clone();
            }

            return dicTemp;
        }

        private string MakeKey(SOPManager.IDAL.ISelect selectSop, int actionStepID, out int versionID)
        {
            versionID = -1;

            string strErrorMessage;
            SOPManager.Model.Sop.Category.ActionStep actionStep = selectSop.SelectActionStep(actionStepID, out strErrorMessage);
            if (actionStep == null || actionStep.DisasterID < 1)
                return "";

            int disasterID = actionStep.DisasterID;

            SOPManager.Model.Sop.Category.Disaster disaster = selectSop.SelectDisaster(disasterID, out strErrorMessage);
            if (disaster == null || disaster.SubDisasterCategoryID < 1)
                return ""; ;

            int subDisasterCategoryID = disaster.SubDisasterCategoryID;            
            versionID = disaster.VersionID;
            SOPManager.Model.Sop.Category.SubDisasterCategory subDisasterCategory = selectSop.SelectSubDisasterCategory(subDisasterCategoryID, out strErrorMessage);
            if (subDisasterCategory == null || subDisasterCategory.DisasterCategoryID < 1)
                return ""; ;

            int disasterCategoryID = subDisasterCategory.DisasterCategoryID;

            string strKey = disasterCategoryID + "/" + subDisasterCategoryID + "/" + disasterID;
            return strKey;
        }

        private void UseExcuteSOP()
        {
            SDMS.IDAL.ISelect select = m_processManager.SdmsManager.GetSelectManager();

            string strErrorMessage = null;

            Dictionary<SDMS.Model.Alarm.CurrentAlarm.Fields, object> dicCondition = new Dictionary<SDMS.Model.Alarm.CurrentAlarm.Fields, object>();
            //dicCondition.Add(SDMS.Model.Alarm.CurrentAlarm.Fields.SopStatus, 0); // SOP 실행요청

            List<SDMS.Model.Alarm.CurrentAlarm> alarms = select.SelectCurrentAlarms(dicCondition, "", out strErrorMessage);
            if (alarms == null)
                return;

            List<int> sensorZoneHistories = new List<int>();

            for (int i = 0; i < alarms.Count; i++)
            {
                CurrentAlarm alarm = alarms[i];
                sensorZoneHistories.Add(alarm.SensorZoneHistoryID);
                if (alarm.SopStatus == 0)
                {
                    SDMS.Model.History.SensorZoneHistory hist = select.SelectSensorZoneHistory(alarm.SensorZoneHistoryID, out strErrorMessage);
                    if (hist == null)
                        return;

                    SDMS.Model.Spatial.Zone zone = select.SelectZone(hist.ZoneID, out strErrorMessage);
                    SOPManager.BLL.Models.SOP.SOPData sopData = GetLinkedSOP(alarm, zone);
                    if (sopData != null)
                    {
                        SOPManager.BLL.Models.SOP.ActionStepData actionStepData = GetExcuteActionStep(sopData, alarm.AlarmDepth);
                        if (actionStepData != null)
                        {
                            string key = sopData.DisasterCategory.ID + "/" + sopData.SubDisasterCategory.ID + "/" + sopData.Disaster.ID + "/" + alarm.SensorZoneHistoryID;
                            if (!m_sopRunDatas.ContainsKey(key))
                                BeginSOP(alarm.TimeStamp, actionStepData, zone.ZoneName, alarm.SensorZoneHistoryID);

                            SetAlarmSopStatus(alarm.SensorZoneHistoryID, 1);
                        }
                    }
                }
            }

            //// 알람 신호가 종료되었는가 ?
            //foreach (KeyValuePair<string, SOPRunData> item in m_sopRunDatas)
            //{
            //    if (item.Value.SensorZoneHistoryID != null)
            //    {
            //        if (!sensorZoneHistories.Contains((int)item.Value.SensorZoneHistoryID))
            //        {
            //            foreach (ActionStepData data in item.Value.SOPData.ActionStepDatas)
            //            {
            //                if (data._ActionStepHistory == null)
            //                    continue;

            //                data._ActionStepHistory.DetectEndTime = DateTime.Now;
            //                //CloseSOP(data._ActionStepHistory);
            //            }
            //        }
            //    }
            //}

        }
        private SectionData GetNextSection(List<ArrowData> arrowDatas, List<SectionData> sectionDatas, int componentType, int componentID)
        {
            return GetNextSection(arrowDatas, sectionDatas, componentType, componentID, "");
        }
        private SectionData GetNextSection(List<ArrowData> arrowDatas, List<SectionData> sectionDatas, int componentType, int componentID, string decisionValue)
        {
            int curComponentType = componentType << 24;
            int curComponentID = curComponentType | componentID;

            int arrowCount = arrowDatas.Count;
            for (var i = 0; i < arrowCount; i++)
            {
                ArrowData arrowData = arrowDatas[i];

                int beginComponentID = arrowData.BeginComponentID;
                int endComponentID = arrowData.EndComponentID;

                if (curComponentID == beginComponentID)
                {
                    string text = arrowData.Text;

                    int endSectionType, endSectionID;
                    GetComponentInfo(arrowData.EndComponentID, out endSectionType, out endSectionID);

                    if (endSectionType == 2) // 설명으로 이어진 화살표는 패스
                        continue;

                    // 판단문은 분기에 맞춰 다음 임무로 진행한다
                    if (componentType == 1)
                    {
                        string resultText = text.ToLower();

                        if (decisionValue.ToLower() != resultText)
                            continue;
                    }

                    int dataCount = sectionDatas.Count;
                    for (var j = 0; j < dataCount; j++)
                    {
                        if (sectionDatas[j].ComponentType == endSectionType && sectionDatas[j].ID == endSectionID)
                        {
                            // 다음 임무 추출
                            return sectionDatas[j];
                        }
                    }
                }
            }

            if (componentType != 3)
            {
                int nextSectionNumber = -1;
                int sectionsCount = sectionDatas.Count;
                for (int i = 0; i < sectionsCount; i++)
                {
                    if (sectionDatas[i].ID == componentID && sectionDatas[i].ComponentType == componentType)
                    {
                        if (sectionDatas[i].SectionNumber == null)
                            break;

                        nextSectionNumber = (int)sectionDatas[i].SectionNumber + 1;
                        continue;
                    }

                    if (nextSectionNumber > 0 && sectionDatas[i].SectionNumber != null && (int)sectionDatas[i].SectionNumber == nextSectionNumber)
                        return sectionDatas[i];
                } 
            }

            return null;
        }

        private void GetComponentInfo(int arrowComponenID, out int componentType, out int componentID)
        {
            componentType = arrowComponenID >> 24;
            componentID = arrowComponenID & 0xffffff;
        }

        private SOPManager.BLL.Models.SOP.SOPData GetLinkedSOP(CurrentAlarm alarm, SDMS.Model.Spatial.Zone zone)
        {
            // 대표 Type
            int nSensorType = 0;
            if (dnsData.Sensor.Facility.IsPSMSensorType(dnsData.Sensor.Facility.ToFacilityType(alarm.SensorType)))
                nSensorType = (int)FacilityType.PSM_SENSOR;
            else if (dnsData.Sensor.Facility.IsETCSensorType(dnsData.Sensor.Facility.ToFacilityType(alarm.SensorType)))
                nSensorType = (int)FacilityType.ETC;
            else if (dnsData.Sensor.Facility.IsSVMSSensorType(dnsData.Sensor.Facility.ToFacilityType(alarm.SensorType)))
                nSensorType = (int)FacilityType.Intrusion_S1;
            else if (dnsData.Sensor.Facility.IsSecurityType(dnsData.Sensor.Facility.ToFacilityType(alarm.SensorType)))
                nSensorType = (int)FacilityType.Security_Sensor;
            else if (dnsData.Sensor.Facility.IsEarthquakeSensorType(dnsData.Sensor.Facility.ToFacilityType(alarm.SensorType)))
                nSensorType = (int)FacilityType.Earthquake;
            
            Dictionary<LinkedSop.Fields, object> dicCondition = new Dictionary<LinkedSop.Fields, object>();            
            dicCondition.Add(LinkedSop.Fields.FacilityTypeID, nSensorType);
            if (zone.BuildingID != null)
                dicCondition.Add(LinkedSop.Fields.LinkedBuildingID, zone.BuildingID);
            dicCondition.Add(LinkedSop.Fields.LinkedZoneID, zone.ID);

            string strErrorMessage = null;
            List<LinkedSop> sops = m_processManager.SopDataManager.GetSelectManager().SelectLinkedSops(dicCondition, out strErrorMessage);
            if (sops == null)
                return null;
            
            LinkedSop sop = null;

            if (sops.Count == 0)
            {
                if (zone.BuildingID != null)
                    dicCondition.Remove(LinkedSop.Fields.LinkedBuildingID);    
                dicCondition.Remove(LinkedSop.Fields.LinkedZoneID);

                sops = m_processManager.SopDataManager.GetSelectManager().SelectLinkedSops(dicCondition, out strErrorMessage);
                if (sops != null && sops.Count > 0)
                    sop = sops[0];
            }                
            else
                sop = sops[0];

            SOPManager.BLL.Models.SOP.SOPData sopData = null;
            if (sop != null)
            {
                sopData = GetLinkedSOP(sops[0].DisasterCategoryID, sops[0].SubDisasterCategoryID, sops[0].DisasterName, alarm);
            }

            return sopData;
        }

        private SOPManager.BLL.Models.SOP.SOPData GetLinkedSOP(int disasterCategoryID, int subDisasterCategoryID, string disasterName, CurrentAlarm alarm)
        {
            string strErrorMessage = null;

            Dictionary<SOPManager.Model.Sop.Category.Disaster.Fields, object> dicCondition = new Dictionary<SOPManager.Model.Sop.Category.Disaster.Fields, object>();
            dicCondition.Add(SOPManager.Model.Sop.Category.Disaster.Fields.SubDisasterCategoryID, subDisasterCategoryID);
            dicCondition.Add(SOPManager.Model.Sop.Category.Disaster.Fields.DisasterName, disasterName);

            string strCondition = string.Format("{0}.OwnerID = {1}.ID AND {0}.ID = {2}.VersionID AND {2}.{3}={4} AND {2}.{5}='{6}' AND {3}=(Select {7} From {8} Where {9}={10})",
                SOPManager.Model.Sop.Category.Version.TableName,
                SOPManager.Model.Sop.Account.User.TableName,
                SOPManager.Model.Sop.Category.Disaster.TableName,
                SOPManager.Model.Sop.Category.Disaster.Fields.SubDisasterCategoryID,
                subDisasterCategoryID,
                SOPManager.Model.Sop.Category.Disaster.Fields.DisasterName,
                disasterName,
                SOPManager.Model.Sop.Category.SubDisasterCategory.Fields.ID,
                SOPManager.Model.Sop.Category.SubDisasterCategory.TableName,
                SOPManager.Model.Sop.Category.SubDisasterCategory.Fields.DisasterCategoryID,
                disasterCategoryID);

            System.Collections.ArrayList arrResult =
             m_processManager.SopDataManager.GetSelectManager().JoinDisasterUserVersion(strCondition, out strErrorMessage);
            if (arrResult == null)
                return null;

            SOPManager.Model.Sop.Category.Version selectedVersion = null;
            SOPManager.Model.Sop.Category.Version selectedVersion2 = null; // 실행되야 할 반대의 모드를 넣어줌 (평일/휴일) > 해당 모드 없으면 이거로 실행시킴

            int resultCount = arrResult.Count;
            if (resultCount == 0)
                return null;

            bool isNormal = GetIsNormal();

            for (int i = 0; i < resultCount; i += 3)
            {
                SOPManager.Model.Sop.Category.Version version = arrResult[i + 2] as SOPManager.Model.Sop.Category.Version;
                if (version == null)
                    continue;

                if (version.IsNormal == isNormal && (selectedVersion == null || selectedVersion.LastAccessTime < version.LastAccessTime))
                    selectedVersion = version;
                else if (version.IsNormal != isNormal && (selectedVersion2 == null || selectedVersion2.LastAccessTime < version.LastAccessTime))
                    selectedVersion2 = version;
            }

            if (selectedVersion == null && selectedVersion2 == null)
                return null;

            SOPManager.BLL.ProcessManager processMgr =
                new SOPManager.BLL.ProcessManager(m_processManager.CommonDataManager, m_processManager.SopDataManager, m_processManager.TeamDataManager, m_processManager.SdmsManager);

            if (selectedVersion != null)
            {
                SOPManager.BLL.Models.Response.ResponseOpen response = processMgr.GetLoadManager().OpenDB(selectedVersion.ID);
                return response.SOPData;
            }
            else
            {
                SOPManager.BLL.Models.Response.ResponseOpen response = processMgr.GetLoadManager().OpenDB(selectedVersion2.ID);
                return response.SOPData;
            }
        }

        /// <summary>
        /// 현재 시간은 주간 근무 시간인가?
        /// </summary>
        /// <returns></returns>
        private bool GetIsNormal()
        {
            string strErrorMessage;

            List<Options> options = m_processManager.CommonDataManager.GetSelectManager().SelectOptions(Options.OptionTarget.SOPSimulator, out strErrorMessage);
            if (options == null || options.Count == 0)
                return true;
            
            string strWorkingBeginHour = "";
            string strWorkingEndHour = "";

            foreach (Options option in options)
            {
                if (option.PropertyName == "WorkingBeginHour")
                {
                    strWorkingBeginHour = option.PropertyValue;
                }
                if (option.PropertyName == "WorkingEndHour")
                {
                    strWorkingEndHour = option.PropertyValue;
                }

                if (strWorkingBeginHour.Length > 0 && strWorkingEndHour.Length > 0)
                    break;
            }

            DateTime dtNow = DateTime.Now;
            DateTime beginDate;
            DateTime endDate;

            if (!DateTime.TryParse(dtNow.Year + "-" + dtNow.Month + "-" + dtNow.Day + " " + strWorkingBeginHour + ":00", out beginDate))
                return true;
            if (!DateTime.TryParse(dtNow.Year + "-" + dtNow.Month + "-" + dtNow.Day + " " + strWorkingEndHour + ":59", out endDate))
                return true;

            if (dtNow.DayOfWeek >= DayOfWeek.Monday && dtNow.DayOfWeek <= DayOfWeek.Friday && dtNow >= beginDate && dtNow <= endDate)
                return true;
            else 
                return false;
        }

        /// <summary>
        /// 실행할 SOP 단계를 선택한다
        /// 알람으로 실행되는 SOP의 경우 알람단계에 맞는 단계로 실행한다
        /// 수동으로 시작하는 경우 마지막 단계로 실행한다
        /// </summary>
        /// <param name="sopData"></param>
        /// <param name="alarmDepth"></param>
        /// <returns></returns>
        private SOPManager.BLL.Models.SOP.ActionStepData GetExcuteActionStep(SOPManager.BLL.Models.SOP.SOPData sopData, int? alarmDepth)
        {
            List<int> depthIndexs = GetContainsStepIndex(sopData);
            if (depthIndexs.Count == 0)
                return null;

            int excuteDepth = -1;

            // 수동 실행
            if (alarmDepth == null)
                excuteDepth = depthIndexs.Max();
            else
            {
                if (depthIndexs.Contains((int)alarmDepth)) // 알람 단계에 맞는 SOP 단계가 있는지
                {                    
                    excuteDepth = (int)alarmDepth;
                }
                else
                {
                    if (alarmDepth > 0 && depthIndexs.Contains((int)alarmDepth - 1)) // 알람 단계보다 한 단계 낮는 SOP 단계가 있는지
                        excuteDepth = (int)alarmDepth - 1;
                    else if (depthIndexs.Contains((int)alarmDepth + 1)) // 알람 단계보다 한 단계 높은 SOP 단계가 있는지
                        excuteDepth = (int)alarmDepth + 1;
                    else if (depthIndexs.Count > 0) // 다 없으면 젤 아래 단계로
                        excuteDepth = depthIndexs.Min(); 
                }
            }

            if (excuteDepth > 0)
            {
                SOPManager.BLL.Models.SOP.ActionStepData data = GetActionStep(sopData, excuteDepth - 1);
                return data;
            }

            return null;
        }

        /// <summary>
        /// SopData에 들어있는 단계 정보
        /// </summary>
        /// <param name="sopData"></param>
        /// <returns></returns>
        private List<int> GetContainsStepIndex(SOPManager.BLL.Models.SOP.SOPData sopData)
        {
            List<int> depthIndexs = new List<int>();

            int actionStepsCount = sopData.ActionStepDatas.Count;
            for (int i = 0; i < actionStepsCount; i++)
            {
                SOPManager.BLL.Models.SOP.ActionStepData actionStepData = sopData.ActionStepDatas[i];
                if (actionStepData.ActionStep == null)
                    continue;

                for (int j = 0; j < m_strActionStepNames.Length; j++)
                {
                    if (actionStepData.StepName == m_strActionStepNames[j])
                    {
                        depthIndexs.Add(j + 1);
                        break;
                    }
                }
                
            }

            return depthIndexs;
        }

        private SOPManager.BLL.Models.SOP.ActionStepData GetActionStep(SOPManager.BLL.Models.SOP.SOPData sopData, int index)
        {
            int actionStepsCount = sopData.ActionStepDatas.Count;
            for (int i = 0; i < actionStepsCount; i++)
            {
                SOPManager.BLL.Models.SOP.ActionStepData actionStepData = sopData.ActionStepDatas[i];
                if (actionStepData.ActionStep == null)
                    continue;

                if (m_strActionStepNames.Length > index && actionStepData.StepName == m_strActionStepNames[index])
                {
                    return actionStepData;
                }
            }

            return null;
        }

        /// <summary>
        /// SOP를 실행상태로 만든다
        /// </summary>
        private ActionStepHistory ExcuteSOP(DateTime? beginTime, int actionStepID, string position, int? userID, int? sensorZoneHistoryID)
        {
            if (sensorZoneHistoryID != null)
            {
                // 중복 실행 방지
                string strErrorMessage = null;
                Dictionary<ActionStepHistory.Fields, object> dicConditions = new Dictionary<ActionStepHistory.Fields, object>();
                dicConditions.Add(ActionStepHistory.Fields.ActionStepID, actionStepID);
                dicConditions.Add(ActionStepHistory.Fields.SensorZoneHistoryID, (int)sensorZoneHistoryID);
                List<ActionStepHistory> actionStepHistories = m_processManager.CommonDataManager.GetSelectManager().SelectActionStepHistories(dicConditions, null, out strErrorMessage);
                if (actionStepHistories == null || actionStepHistories.Count > 0)
                {
                    return null;
                }
            }

            DateTime dtTime = DateTime.Now;
            if (beginTime != null)
                dtTime = (DateTime)beginTime;

            ActionStepHistory history = m_processManager.CommonDataManager.GetCreateManager().CreateActionStepHistory(
                actionStepID, dtTime, true, null, dtTime, null, dtTime, position, userID, null, null, sensorZoneHistoryID, null);

            return history;
        }

        /// <summary>
        /// SOP를 종료상태로 만든다
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool CloseSOP(ActionStepHistory actionStepHistory)
        {
            string strErrorMessage = null;

            actionStepHistory.EndTime = DateTime.Now;

            if (!m_processManager.CommonDataManager.GetUpdateManager().UpdateActionStepHistory(actionStepHistory, out strErrorMessage))
                return false;

            // 센서 신호로 실행된 SOP라면 SensorZoneHistoryID가 있다.
            // 해당 SOP가 종료되었음을 DB에 기록한다
            if (actionStepHistory.SensorZoneHistoryID > 0)
            {
                SetAlarmSopStatus((int)actionStepHistory.SensorZoneHistoryID, 2);
            }

            return true;
        }

        public void CloseSOPByUser(int actionStepHistoryID, DateTime? endTime, int? userID)
        {
            string strErrorMessage = null;
            ActionStepHistory history = m_processManager.CommonDataManager.GetSelectManager().SelectActionStepHistory(actionStepHistoryID, out strErrorMessage);
            history.EndTime = (endTime == null) ? DateTime.Now : endTime;
            history.LastAccessedUserID = userID;

            string strCondition = string.Format("ID={0}", history.ID);

            if (m_processManager.CommonDataManager.GetUpdateManager().UpdateActionStepHistory(history, out strErrorMessage))
            {
                if (m_confirmTimeoutCloseSOPs.Contains(history.ID))
                    m_confirmTimeoutCloseSOPs.Remove(history.ID);

                // 알람 신호로 실행된 SOP가 모두 종료되었음을 DB에 기록한다
                if (history.SensorZoneHistoryID != null && history.SensorZoneHistoryID > 0)
                {
                    strCondition = string.Format("SensorZoneHistoryID={0}", history.SensorZoneHistoryID);
                    List<ActionStepHistory> histories = m_processManager.CommonDataManager.GetSelectManager().SelectActionStepHistories(strCondition, out strErrorMessage);
                    if (histories != null && histories.Count > 0)
                        SetAlarmSopStatus((int)history.SensorZoneHistoryID, 2);
                }
            }
        }

        /// <summary>
        /// SdmsAlarmCurrent 테이블 SopStatus 업데이트
        /// SOP 실행 상태를 업데이트 한다
        /// </summary>
        /// <param name="sopStatus">-1: SOP 시작 하기전, 0: SOP 실행 요청, 1: SOP 실행중, 2: SOP종료</param>
        private void SetAlarmSopStatus(int sensorZoneHistoryID, int sopStatus)
        {
            // 센서 신호로 실행된 SOP라면 SensorZoneHistoryID가 있다.
            // 해당 SOP가 실행중임을 DB에 기록한다

            string strErrorMessage = null;

            Dictionary<CurrentAlarm.Fields, object> dicSets = new Dictionary<CurrentAlarm.Fields, object>();
            dicSets.Add(CurrentAlarm.Fields.SopStatus, sopStatus); 

            Dictionary<CurrentAlarm.Fields, object> dicConditions = new Dictionary<CurrentAlarm.Fields, object>();
            dicConditions.Add(CurrentAlarm.Fields.SensorZoneHistoryID, sensorZoneHistoryID);
            m_processManager.SdmsManager.GetUpdateManager().UpdateCurrentAlarm(dicSets, dicConditions, "", out strErrorMessage);
        }

        /// <summary>
        /// 시작
        /// </summary>
        public bool BeginSOP(DateTime beginTime, ActionStepData actionStepData, string position, int? sensorZoneHistoryID)
        {
            ActionStepHistory newHistory = ExcuteSOP(beginTime, actionStepData.ActionStep.ID, position, null, sensorZoneHistoryID);
            if (newHistory != null)
            {
                List<ArrowData> arrowDatas = actionStepData.StepMemberDatas[0].Arrows;
                List<SectionData> sectionDatas = actionStepData.StepMemberDatas[0].Sections;
                SectionData currentSectionData = actionStepData.StepMemberDatas[0].Sections[0]; // SOP가 새로 시작했으므로 첫번째가 현재section이 된다

                CheckAutoSection(arrowDatas, sectionDatas, currentSectionData.ComponentType, currentSectionData.ID, currentSectionData.Text, "", newHistory);
                return true;
            }

            return false; 
        }

        private void CheckAutoSection(List<ArrowData> arrowDatas, List<SectionData> sectionDatas, int componentType, int componentID, string sectionText, string decisionValue, ActionStepHistory history)
        {
            // 시작
            ProgressSOP(history, componentID, componentType, (int)SectionStatus.DONE, null, sectionText); 

            while (true)
            {
                SectionData nextSection = GetNextSection(arrowDatas, sectionDatas, componentType, componentID, decisionValue);
                if (nextSection != null)
                {
                    // 실행중
                    ProgressSOP(history, nextSection.ID, nextSection.ComponentType, (int)SectionStatus.RUN, null, nextSection.Text);
                    if (nextSection.AutoRun == null || nextSection.AutoRun == false)
                        break;
                    else
                    {
                        // 상황전파 수행
                        if (nextSection.ComponentType == 6)
                        {

                        }

                        // 완료
                        ProgressSOP(history, nextSection.ID, nextSection.ComponentType, (int)SectionStatus.DONE, null, nextSection.Text);
                        componentType = nextSection.ComponentType;
                        componentID = nextSection.ID;
                        //_decisionValue = ""; // 어떻게 해야 함 ???????
                    }
                }
                else
                {
                    // 종료
                    if (componentType == 3)
                    {
                        CloseSOP(history);
                    }
                    break;
                }
            }
        }

        public void RunSection(RequestProgressSOP data)
        {
            m_bOnGoing = true;

            if (!m_sopRunDatas.ContainsKey(data.SopKey))
            {
                m_bOnGoing = false;
                return;
            }

            SOPRunData datas = m_sopRunDatas[data.SopKey];
            foreach (ActionStepData runData in datas.SOPData.ActionStepDatas)
            {
                if (runData.ActionStep == null)
                    continue;

                if (runData._ActionStepHistory == null)
                {
                    if (data.ActionStepHistoryID == -1)
                    {
                        if (runData.ActionStep.ID == data.ActionStepID)
                        {
                            BeginSOP(DateTime.Now, runData, datas.Position, datas.SensorZoneHistoryID);
                            break; 
                        }
                    }
                    continue;
                }

                else if (runData._ActionStepHistory.ID != data.ActionStepHistoryID)
                    continue;
                
                if ((runData.CurrentSection.ID == data.ComponentID && runData.CurrentSection.ComponentType == data.ComponentType) ||
                    (data.Skip && data.ComponentType != 3))
                {
                    // 현재 임무의 다음 버튼을 눌렀거나
                    // 다른 임무의 다음 버튼을 누른 경우 (건너뛰기)

                    // 다음버튼이 눌러진 임무 종료 처리 후 다음 임무를 찾아서 실행 처리한다
                    StepMemberData stepMemberData = runData.StepMemberDatas[0];
                    CheckAutoSection(stepMemberData.Arrows, stepMemberData.Sections, data.ComponentType, data.ComponentID, data.Text, data.DecisionValue, runData._ActionStepHistory);
                }
                else
                {
                    // 현재 임무와 다른 임무인가 ? (건너뛰기) Y: 선택한 임무 진행중 처리
                    // 종료를 누른거라면?
                    if (data.ComponentType == 3)
                    {
                        CloseSOPByUser(runData._ActionStepHistory.ID, null, (int)data.AccessedUserID);
                    }
                    else
                    {
                        ProgressSOP(runData._ActionStepHistory, data.ComponentID, data.ComponentType, (int)SectionStatus.RUN, data.AccessedUserID, data.Text);
                    }
                }

                break;
            }

            //DisplayHistory();

            m_bOnGoing = false;
        }

        private ComponentHistory ProgressSOP(ActionStepHistory actionStepHistory, int componentID, int componentType, int status, int? userID, string text, string addDescription = "")
        {
            string strErrorMessage = null;

            string strDescription = GetStringStatus(componentType, status, text);
            if (addDescription.Length > 0)
                strDescription += "_" + addDescription;

            strDescription = strDescription.Replace("'", "''");

            ComponentHistory history = m_processManager.CommonDataManager.GetCreateManager().CreateComponentHistory(
                actionStepHistory.ID, componentID, componentType, DateTime.Now, status, null, null, null
                , userID, null, null, null, null, strDescription);

            actionStepHistory.LastAccessedTime = DateTime.Now;
            m_processManager.CommonDataManager.GetUpdateManager().UpdateActionStepHistory(actionStepHistory, out strErrorMessage);

            return history;
        }

        /// <summary>
        /// 현재 실행중인 Sop 진행 데이터안에서 ActionStepHistoryID와 같은 데이터를 가져온다
        /// </summary>
        /// <param name="sopKey"></param>
        /// <param name="actionStepHistoryID"></param>
        /// <returns></returns>
        public ActionStepData GetActionStepData(string sopKey, int actionStepHistoryID)
        {
            if (!m_sopRunDatas.ContainsKey(sopKey))
                return null;

            foreach (ActionStepData actionStepData in m_sopRunDatas[sopKey].SOPData.ActionStepDatas)
            {
                if (actionStepData.ActionStep == null)
                    continue;

                if (actionStepData._ActionStepHistory == null)
                    continue;

                if (actionStepData._ActionStepHistory.ID == actionStepHistoryID)
                {
                    return actionStepData;
                }
            }

            return null;
        }

        public bool ProgressMission(string sopKey, int actionStepHistoryID, int componentType, int componentID, int dataIndex, int componentStatus, int? userID, bool isChecked)
        {
            try
            {
                m_bOnGoing = true;

                ActionStepData actionStepData = GetActionStepData(sopKey, actionStepHistoryID);
                if (actionStepData == null)
                    return false;

                if (actionStepData.StepMemberDatas == null || actionStepData.StepMemberDatas.Count == 0)
                    return false;

                StepMemberData stepMemberData = actionStepData.StepMemberDatas[0];

                foreach (SectionData section in stepMemberData.Sections)
                {
                    if (section.ID == componentID && section.ComponentType == componentType)
                    {
                        string addDescription = (dataIndex + 1) + "번째";
                        if (isChecked)
                            addDescription += " 체크";
                        else
                            addDescription += " 체크 해제";

                        ComponentHistory history = ProgressSOP(actionStepData._ActionStepHistory, componentID, componentType, componentStatus, userID, section.Text, addDescription);
                        if (history == null)
                            return false;

                        int nData = (isChecked == true) ? 1 : 0;

                        ComponentHistoryDetail detail = m_processManager.CommonDataManager.GetCreateManager().CreateComponentHistoryDetail(
                            history.ID, dataIndex, nData, null, null, DateTime.Now);

                        if (detail == null)
                            return false;
                                                
                        bool allCheck = true; // 모든 임무가 체크됐는지 확인
                        SetCheckedMission(section, dataIndex, isChecked, ref allCheck);
                        if (componentType == 0)
                        {                                 
                            // 현재 임무가 아닌 다른 임무를 체크했다 ? -> 체크한 임무를 현재임무로 바꿔줌
                            if (componentStatus != 2 && isChecked)
                            {
                                ProgressSOP(actionStepData._ActionStepHistory, componentID, componentType, 2, userID, section.Text, addDescription);
                            }
                        }

                        if (allCheck && isChecked)
                        {
                            CheckAutoSection(stepMemberData.Arrows, stepMemberData.Sections, componentType, componentID, section.Text, "", actionStepData._ActionStepHistory);
                        }

                        break;
                    }
                }

                m_bOnGoing = false;
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                m_bOnGoing = false;
                return false;
            }
        }

        private void SetCheckedMission(SectionData sectionData, int dataIndex, bool isChecked, ref bool allCheck)
        {
            // 프로세스 컴포넌트는 여러 개의 Mission이 있으므로 모든 Mission이 체크됐는지 확인한다.
            if (sectionData.ComponentType == 0)
            {
                sectionData.Missions[dataIndex].Checked = isChecked;
                foreach (ProcessMissionData mission in sectionData.Missions)
                {
                    if (!mission.Checked)
                    {
                        allCheck = false;
                        break;
                    }
                }
            }
            else
            {
                sectionData.Checked = isChecked;
                if (!isChecked)
                    allCheck = false;
            }

            sectionData.Checked = allCheck;
        }

        public bool ProgressSpread(string sopKey, int actionStepHistoryID, int componentType, int componentID, int dataIndex, int componentStatus, int? userID, bool isSMS, bool isEmail, bool isBroadcast, bool isSiren, string strMessage)
        {
            try
            {
                m_bOnGoing = true;

                ActionStepData actionStepData = GetActionStepData(sopKey, actionStepHistoryID);
                if (actionStepData == null)
                    return false;

                foreach (SectionData section in actionStepData.StepMemberDatas[0].Sections)
                {
                    if (section.ID == componentID && section.ComponentType == componentType)
                    {
                        List<string> messages = new List<string>();
                        if (componentType == 0)
                        {
                            if (dataIndex == -1) // 전체 세부 목록 모두 전파
                            {
                                for (int i = 0; i < section.Missions.Count; i++)
                                {
                                    messages.Add(section.Missions[i].MissionText);
                                }
                            }
                            else
                            {
                                if (section.Missions.Count < dataIndex)
                                    return false;

                                messages.Add(section.Missions[dataIndex].MissionText);
                            }
                        }
                        else
                        {
                            // 상황전파 컴포넌트는 UI에서 메시지 내용을 변경할 수 있으므로 받아온다
                            messages.Add(strMessage);
                        }

                        SOPManager.BLL.ProcessManager processMgr = new SOPManager.BLL.ProcessManager(
                            m_processManager.CommonDataManager, m_processManager.SopDataManager, m_processManager.TeamDataManager, m_processManager.SdmsManager);

                        for (int i = 0; i < messages.Count; i++)
                        {
                            string message = ReplaceMessage(messages[i], actionStepData._ActionStepHistory.Position, actionStepData._ActionStepHistory.BeginTime.ToString(), processMgr.GetLoadManager());

                            //if (ProgressSpread(section.Receivers, message, isSMS, isEmail, isBroadcast, isSiren))
                            {
                                int dataI = 2; // 0: 체크해제, 1: 체크, 10: 문자메시지전파, 20: 메일전파, 30: 방송전파
                                string addDescription = "";
                                if (isSMS)
                                {
                                    addDescription += "문자메시지전파";
                                    dataI = 10;

                                    if (SendSMS(section.Receivers, message))
                                    {
                                        ComponentHistory history = ProgressSOP(actionStepData._ActionStepHistory, componentID, componentType, componentStatus, userID, section.Text, addDescription);
                                        if (history == null)
                                            return false;

                                        int index = dataIndex;
                                        if (index == -1)
                                            index = i;

                                        ComponentHistoryDetail detail = m_processManager.CommonDataManager.GetCreateManager().CreateComponentHistoryDetail(
                                            history.ID, index, dataI, null, null, DateTime.Now);
                                    }
                                }
                                if (isEmail)
                                {
                                    addDescription += "메일전파";
                                    dataI = 20;

                                    if (SendEmail(message))
                                    {
                                        ComponentHistory history = ProgressSOP(actionStepData._ActionStepHistory, componentID, componentType, componentStatus, userID, section.Text, addDescription);
                                        if (history == null)
                                            return false;

                                        int index = dataIndex;
                                        if (index == -1)
                                            index = i;

                                        ComponentHistoryDetail detail = m_processManager.CommonDataManager.GetCreateManager().CreateComponentHistoryDetail(
                                            history.ID, index, dataI, null, null, DateTime.Now);
                                    }
                                }
                                if (isBroadcast)
                                {
                                    addDescription += "방송전파";
                                    dataI = 30;

                                    if (SendBroadcast(message))
                                    {
                                        ComponentHistory history = ProgressSOP(actionStepData._ActionStepHistory, componentID, componentType, componentStatus, userID, section.Text, addDescription);
                                        if (history == null)
                                            return false;

                                        int index = dataIndex;
                                        if (index == -1)
                                            index = i;

                                        ComponentHistoryDetail detail = m_processManager.CommonDataManager.GetCreateManager().CreateComponentHistoryDetail(
                                            history.ID, index, dataI, null, null, DateTime.Now);
                                    }
                                }
                            } 
                        }
                        break;
                    }
                }

                m_bOnGoing = false;
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("SOPRunManager.cs ProgressInternalTransmission() : " + ex.Message);
                m_bOnGoing = false;
                return false;
            }
        }
        private bool SendSMS(List<Receiver> receivers, string message)
        {
            bool result = m_processManager.GetSMSManager().ProgressInternalSpread(receivers, message);
            return result;
        }

        private bool SendEmail(string message)
        {
            if (m_processManager.SopSimulatorDataManager.SiteID == 10)
            {
                IEmailClient clientMail = EmailClientFactory.CreateMailClient();
                string strResultMsg = "";

                if (clientMail != null)
                {
                    Dictionary<string, string> dicMail = new Dictionary<string, string>();
                    dicMail["esh@soulbrain.co.kr"] = "esh@soulbrain.co.kr";

                    EmailContent contents = new EmailContent();
                    contents.Caller = "";
                    contents.EmailList.AddRange(dicMail.Values);
                    contents.Message = message;
                    //contents.SensorReactionHistoryID = data.SensorReactionHistoryID;

                    contents.Title = message;
                    contents.Subject = message;
                    contents.TimeStamp = DateTime.Now;

                    // 수신자번호 가운데 빈문자열이 있으면 없앤다.
                    int nIndex = contents.EmailList.IndexOf("");

                    if (nIndex >= 0)
                        contents.EmailList.RemoveAt(nIndex);

                    clientMail.SendEmail(contents, ref strResultMsg);

                    return true;
                } 
            }

            return false;
        }

        private bool SendBroadcast(string message)
        {
            return true;
        }

        private string GetStringStatus(int componentType, int nStatus, string text)
        {
            string strType = "";
            if (componentType == 0)
                strType = "Process";
            else if (componentType == 1)
                strType = "Decision";
            else if (componentType == 3)
                strType = "EndPoint";
            else if (componentType == 6)
                strType = "Internal";

            //NORMAL = 1, RUN = 2, DONE = 3, INPUT = 4, SKIP = 5
            string strStatus = "";
            if (nStatus == (int)SectionStatus.NORMAL)
                strStatus = "대기";
            else if (nStatus == (int)SectionStatus.RUN)
                strStatus = "실행중";
            else if (nStatus == (int)SectionStatus.DONE)
                strStatus = "완료";

            string strDescription = string.Format("{0}_{1}_{2}", strType, text, strStatus);

            return strDescription;
        }

        public string ReplaceMessage(string message, string position, string time, SOPManager.BLL.LoadManager loadManager)
        {
            string retrunMessage = message;
            // 특수 문자가 있니 ?
            if (message.Contains("{") && message.Contains("}"))
            {                
                SOPManager.BLL.Models.Request.RequestParseSpecialMessage req = new SOPManager.BLL.Models.Request.RequestParseSpecialMessage();
                req.Message = message;
                req.Location = position;
                req.Time = time;
                
                SOPManager.BLL.Models.Response.ResponseParseSpecialMessage res = loadManager.ParseSpecialMessage(req);
                retrunMessage = res.ParseMessage;
            }

            return retrunMessage;
        }
    }
}
