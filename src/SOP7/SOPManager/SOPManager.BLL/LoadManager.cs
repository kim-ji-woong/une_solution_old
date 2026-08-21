using System;
using System.Collections.Generic;
using System.Collections;

namespace SOPManager.BLL
{
    using Model.Sop.Category;
    using Model.Sop.Component;
    using Model.Sop.Account;
    using Model.Sop.Config;
    using IDAL;
    using TeamEditor.Model.Sop.Team;
    using Models.SOP;
    using Models.Response;
    using SOPManager.BLL.Models.Request;
    using System.Linq;

    public class LoadManager
    {
        private IDataManager m_dataManager = null;
        private ProcessManager m_processManager = null;

        public LoadManager(IDataManager manager, ProcessManager processManager)
        {
            m_dataManager = manager;
            m_processManager = processManager;
        }

        public StepMember MakeNewStepMember(out string strStepMemberName)
        {
            strStepMemberName = "";

            if (m_processManager.TeamDataManager == null)
                return null;

            TeamEditor.IDAL.ISelect selectManager = m_processManager.TeamDataManager.GetSelectManager();

            string strErrorMessage;
            List<Regular> teams = selectManager.SelectRegulars(out strErrorMessage);

            if (teams == null || strErrorMessage != null || teams.Count == 0)
                return null;

            Regular team = teams[0];

            StepMember stepMember = new StepMember();
            stepMember.TeamID = team.ID;
            stepMember.TeamType = (int)StepMember.MemberTeamType.RegularTeam;
            strStepMemberName = team.TeamName;

            return stepMember;
        }

        public ResponseDisasterCategories DisasterCategories(bool isNormal)
        {
            Dictionary<DisasterCategory, List<SubDisasterCategory>> dicDisasterCategories = new Dictionary<DisasterCategory, List<SubDisasterCategory>>();
            Dictionary<SubDisasterCategory, List<VersionDisasterData>> dicSubDisasterCategories = new Dictionary<SubDisasterCategory, List<VersionDisasterData>>();
            Dictionary<Disaster, List<ActionStep>> dicDisasterActionSteps = new Dictionary<Disaster, List<ActionStep>>();
            Dictionary<int, Version> dicVersions = new Dictionary<int, Version>();
            List<string> actionStepNames = new List<string>();

            if (LoadSOPCategories(dicDisasterCategories, dicSubDisasterCategories, dicDisasterActionSteps, dicVersions, actionStepNames, isNormal) == false)
                return MakeResponseDisasterCategories(null, "SOP의 최상위 카테고리(재난분야) 데이터가 존재하지 않습니다.");

            List<ActionStep> actionSteps;
            List<VersionDisasterData> disasters;
            List<DisasterCategoryData> disasterCategoryDatas = new List<DisasterCategoryData>();

            //Version version;

            foreach (KeyValuePair<DisasterCategory, List<SubDisasterCategory>> pair in dicDisasterCategories)
            {
                DisasterCategoryData data = new DisasterCategoryData();
                data.DisasterCategory = pair.Key;

                foreach (SubDisasterCategory sdc in pair.Value)
                {
                    SubDisasterCategoryData sdcData = new SubDisasterCategoryData();
                    sdcData.SubDisasterCategory = sdc;

                    if (dicSubDisasterCategories.TryGetValue(sdc, out disasters))
                    {
                        foreach (VersionDisasterData versionDisasterData in disasters)
                        //foreach (KeyValuePair<string, List<Disaster>> pairDisaster in dicDisasters)
                        {
                            //List<DisasterData> disasterDatas = new List<DisasterData>();

                            foreach (DisasterData disasterData in versionDisasterData.DisasterDatas)
                            {
                                //disasterDatas.Add(disasterData);

                                if (dicDisasterActionSteps.TryGetValue(disasterData.Disaster, out actionSteps))
                                {
                                    int nActionStepCount = actionSteps.Count;

                                    for (int i = 0; i < nActionStepCount; i++)
                                    {
                                        ActionStep actionStep = actionSteps[i];

                                        if (actionStepNames.Count <= i)
                                            continue;

                                        ActionStepData actionStepData = new ActionStepData();
                                        actionStepData.StepName = actionStepNames[i];

                                        if (actionStep == null)
                                        {
                                            disasterData.ActionSteps.Add(actionStepData);
                                        }
                                        else
                                        {
                                            actionStepData.ActionStep = actionStep;
                                            disasterData.ActionSteps.Add(actionStepData);
                                        }
                                    }
                                }
                            }

                            sdcData.DisasterDatas.Add(versionDisasterData);
                            //sdcData.Disasters[pairDisaster.Key] = disasterDatas;
                        }
                    }

                    data.SubDisasterCategories.Add(sdcData);
                }

                disasterCategoryDatas.Add(data);
            }

            return MakeResponseDisasterCategories(disasterCategoryDatas, "");
        }

        private ResponseDisasterCategories MakeResponseDisasterCategories(List<DisasterCategoryData> disasterCategoryDatas, string strMessage)
        {
            ResponseDisasterCategories result = new ResponseDisasterCategories();

            if (disasterCategoryDatas == null)
            {
                result.Success = false;
            }
            else
            {
                result.Success = true;
                result.DisasterCategoryDatas.AddRange(disasterCategoryDatas);
            }

            result.Message = strMessage;
            return result;
        }

        public bool LoadSOPCategories(Dictionary<DisasterCategory, List<SubDisasterCategory>> dicDisasterCategories, Dictionary<SubDisasterCategory, List<VersionDisasterData>> dicSubDisasterCategories, Dictionary<Disaster, List<ActionStep>> dicDisasterActionSteps, Dictionary<int, Version> dicVersions, List<string> actionStepNames, bool isNormal)
        {
            if (m_dataManager == null)
                return false;

            dicDisasterCategories.Clear();
            dicSubDisasterCategories.Clear();

            string strErrorMessage;
            ISelect selectManager = m_dataManager.GetSelectManager();
            List<DisasterCategory> disasterCategories = selectManager.SelectDisasterCategories(out strErrorMessage);

            if (strErrorMessage != null)
                return false;

            List<Common.Model.Option.Options> options = m_processManager.CommonDataManager.GetSelectManager().SelectOption(Common.Model.Option.Options.OptionTarget.SOPSimulator, "StandardActionStepNames", out strErrorMessage);

            if (options == null || strErrorMessage != null)
                return false;

            string strCondition = string.Format("isNormal = {0}", isNormal ? 1 : 0);

            List<Version> versions = selectManager.SelectVersions(strCondition, out strErrorMessage);

            if (versions == null || strErrorMessage != null)
                return false;

            foreach (Version version in versions)
            {
                dicVersions[version.ID] = version;
            }

            List<string> standardActionStepNames = GetStandardActionStepNames(options);
            actionStepNames.AddRange(standardActionStepNames);

            foreach (DisasterCategory disasterCategory in disasterCategories)
            {
                List<SubDisasterCategory> subDisasterCategories = selectManager.SelectSubDisasterCategories(disasterCategory, out strErrorMessage);

                if (strErrorMessage != null)
                    return false;

                if (subDisasterCategories == null)
                    continue;

                dicDisasterCategories[disasterCategory] = subDisasterCategories;

                foreach (SubDisasterCategory subDisasterCategory in subDisasterCategories)
                {
                    // 같은 이름을 가진 다른 버전의 Disaster가 있을수 있다.
                    // 그래서, List가 아닌 Dictionary를 사용한다.
                    // Key : Disaster 이름
                    // Value : 버전별로 정렬된 Disaster.
                    Dictionary<string, List<Disaster>> dicDisasters = selectManager.SelectDisasters(subDisasterCategory, isNormal, out strErrorMessage);

                    if (strErrorMessage != null)
                        return false;

                    if (dicDisasters == null)
                        continue;

                    List<VersionDisasterData> versionDisasterDatas = new List<VersionDisasterData>();

                    foreach (KeyValuePair<string, List<Disaster>> pair in dicDisasters)
                    {
                        VersionDisasterData versionDisasterData = new VersionDisasterData();
                        versionDisasterData.DisasterName = pair.Key;

                        foreach (Disaster disaster in pair.Value)
                        {
                            List<ActionStep> actionSteps = selectManager.SelectActionSteps(disaster, out strErrorMessage);

                            if (actionSteps == null || strErrorMessage != null)
                                return false;

                            List<ActionStep> disasterActionSteps = MakeDefaultActionSteps(standardActionStepNames);

                            foreach (ActionStep actionStep in actionSteps)
                            {
                                AddActionStep(actionStep, disasterActionSteps, standardActionStepNames);
                            }

                            dicDisasterActionSteps[disaster] = disasterActionSteps;

                            DisasterData disasterData = new DisasterData();

                            disasterData.Disaster = disaster;

                            Version version;
                            
                            if (dicVersions.TryGetValue(disaster.VersionID, out version))
                            {
                                disasterData.Version = version;

                                User user = m_dataManager.GetSelectManager().SelectUser(version.OwnerID, out strErrorMessage);

                                if (user != null)
                                {
                                    disasterData.Owner = user.UserID;
                                }
                            }

                            versionDisasterData.DisasterDatas.Add(disasterData);
                        }

                        versionDisasterDatas.Add(versionDisasterData);
                    }

                    dicSubDisasterCategories[subDisasterCategory] = versionDisasterDatas;

                    /*foreach (KeyValuePair<string, List<Disaster>> pair in dicDisasters)
                    {
                        foreach (Disaster disaster in pair.Value)
                        {
                            List<ActionStep> actionSteps = selectManager.SelectActionSteps(disaster, out strErrorMessage);

                            if (actionSteps == null || strErrorMessage != null)
                                return false;

                            List<ActionStep> disasterActionSteps = MakeDefaultActionSteps(standardActionStepNames);

                            foreach (ActionStep actionStep in actionSteps)
                            {
                                AddActionStep(actionStep, disasterActionSteps, standardActionStepNames);
                            }

                            dicDisasterActionSteps[disaster] = disasterActionSteps;
                            //dicDisasterActionSteps[disaster] = actionSteps;
                        }
                    }*/
                }
            }

            return true;
        }

        private void AddActionStep(ActionStep actionStep, List<ActionStep> actionSteps, List<string> standardActionStepNames)
        {
            int nIndex = standardActionStepNames.IndexOf(actionStep.StepName);

            if (nIndex >= 0)
                actionSteps[nIndex] = actionStep;
        }

        private List<ActionStep> MakeDefaultActionSteps(List<string> standardActionStepNames)
        {
            List<ActionStep> actionSteps = new List<ActionStep>();

            foreach (string strActionStepName in standardActionStepNames)
            {
                actionSteps.Add(null);
            }

            return actionSteps;
        }

        private List<string> GetStandardActionStepNames(List<Common.Model.Option.Options> options)
        {
            List<string> actionStepNames = new List<string>();

            if (options.Count == 0)
            {
                actionStepNames.Add("관심");
                actionStepNames.Add("주의");
                actionStepNames.Add("경계");
                actionStepNames.Add("심각");
            }
            else
            {
                string[] tokens = options[0].PropertyValue.Split(',');

                foreach (string strToken in tokens)
                {
                    string strName = strToken.Trim();

                    if (strName.Length > 0)
                        actionStepNames.Add(strName);
                }
            }

            return actionStepNames;
        }

        /*public bool LoadSOP(int disasterID, List<ActionStep> actionSteps, List<string> actionStepNames, Dictionary<ActionStep, List<StepMember>> dicActionSteps, Dictionary<StepMember, List<Section>> dicStepMemberSections, Dictionary<StepMember, List<Arrow>> dicStepMemberArrows)
        {
            if (m_dataManager == null)
                return false;

            dicActionSteps.Clear();
            dicStepMemberSections.Clear();
            dicStepMemberArrows.Clear();

            string strErrorMessage;
            ISelect selectManager = m_dataManager.GetSelectManager();

            Disaster disaster = selectManager.SelectDisaster(disasterID, out strErrorMessage);

            if (disaster == null || strErrorMessage != null)
                return false;

            List<ActionStep> _actionSteps = selectManager.SelectActionSteps(disaster, out strErrorMessage);

            if (actionSteps == null || strErrorMessage != null)
                return false;

            List<Common.Model.Option.Options> options = m_processManager.CommonDataManager.GetSelectManager().SelectOption(Common.Model.Option.Options.OptionTarget.SOPSimulator, "StandardActionStepNames", out strErrorMessage);

            if (options == null || strErrorMessage != null)
                return false;

            List<string> standardActionStepNames = GetStandardActionStepNames(options);
            List<ActionStep> disasterActionSteps = MakeDefaultActionSteps(standardActionStepNames);

            foreach (ActionStep actionStep in _actionSteps)
            {
                AddActionStep(actionStep, disasterActionSteps, standardActionStepNames);
            }

            actionStepNames.AddRange(standardActionStepNames);
            actionSteps.AddRange(disasterActionSteps);

            foreach (ActionStep actionStep in _actionSteps)
            {
                List<StepMember> stepMembers = selectManager.SelectStepMembers(actionStep, out strErrorMessage);

                if (stepMembers == null || strErrorMessage != null)
                    return false;

                dicActionSteps[actionStep] = stepMembers;

                foreach (StepMember stepMember in stepMembers)
                {
                    List<Section> sections = new List<Section>();
                    List<Arrow> arrows = new List<Arrow>();

                    if (selectManager.SelectStepMemberComponents(stepMember, sections, arrows, out strErrorMessage) == false)
                        return false;

                    if (strErrorMessage != null)
                        return false;

                    dicStepMemberSections[stepMember] = sections;
                    dicStepMemberArrows[stepMember] = arrows;
                }
            }

            return true;
        }*/

        /// <summary>
        /// 전체 버전정보를 읽어온다.
        /// </summary>
        /// <param name="dicVersions">
        /// 전체 SOP 정보
        /// Key : Version ID
        /// </param>
        public void LoadVersion(Dictionary<int, Version> dicVersions)
        {
            dicVersions.Clear();

            string strCondition = string.Format("{0}.SiteID = {1} AND {0}.OwnerID = {2}.ID AND {0}.ID = {3}.VersionID ORDER BY {0}.CreateTime", 
                Version.TableName, m_dataManager.SiteID, User.TableName, Disaster.TableName);

            string strErrorMessage;
            ArrayList arrResult = m_dataManager.GetSelectManager().JoinDisasterUserVersion(strCondition, out strErrorMessage);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-2;i+=3)
            {
                if (arrResult[i] is Disaster && arrResult[i + 1] is User && arrResult[i + 2] is Version)
                {
                    Disaster disaster = (Disaster)arrResult[i];
                    User user = (User)arrResult[i + 1];
                    Version version = (Version)arrResult[i + 2];

                    dicVersions[version.ID] = version;
                }
            }
        }

        public ResponseActionStepDatas GetDefaultActionStepDatas()
        {
            string strErrorMessage;
            List<Common.Model.Option.Options> options = m_processManager.CommonDataManager.GetSelectManager().SelectOption(Common.Model.Option.Options.OptionTarget.SOPSimulator, "StandardActionStepNames", out strErrorMessage);

            if (options == null || strErrorMessage != null)
                return GetResponseActionStepDatas(null, strErrorMessage);

            List<string> standardActionStepNames = GetStandardActionStepNames(options);

            if (standardActionStepNames == null || standardActionStepNames.Count == 0)
                return GetResponseActionStepDatas(null, Resource.ID.Get("errorMessage").Value("noStandardActionStepNames"));

            List<ActionStepData> actionStepDatas = new List<ActionStepData>();

            foreach (string strActionStepName in standardActionStepNames)
            {
                ActionStepData actionStepData = new ActionStepData();
                actionStepData.StepName = strActionStepName;
                actionStepDatas.Add(actionStepData);
            }

            return GetResponseActionStepDatas(actionStepDatas, "");
        }

        private ResponseActionStepDatas GetResponseActionStepDatas(List<ActionStepData> actionStepDatas, string strMessage)
        {
            ResponseActionStepDatas result = new ResponseActionStepDatas();

            result.Success = actionStepDatas != null;
            result.Message = strMessage;

            if (actionStepDatas != null)
                result.ActionStepDatas.AddRange(actionStepDatas);

            return result;
        }

        public ResponseStepMemberData GetDefaultStepMemberData()
        {
            string strErrorMessage;
            List<Regular> regularTeams = m_processManager.TeamDataManager.GetSelectManager().SelectRegulars(out strErrorMessage);

            if (regularTeams == null || strErrorMessage != null)
                return GetResponseStepMemberData(null, strErrorMessage);

            if (regularTeams.Count == 0)
                return GetResponseStepMemberData(null, Resource.ID.Get("errorMessage").Value("noRegularTeams"));

            Regular team = regularTeams[0];

            StepMember stepMember = new StepMember();
            stepMember.TeamID = team.ID;
            stepMember.TeamType = (int)StepMember.MemberTeamType.RegularTeam;

            StepMemberData stepMemberData = new StepMemberData();
            stepMemberData.StepMember = stepMember;
            stepMemberData.StepMemberName = team.TeamName;

            return GetResponseStepMemberData(stepMemberData, "");
        }

        private ResponseStepMemberData GetResponseStepMemberData(StepMemberData data, string strMessage)
        {
            ResponseStepMemberData result = new ResponseStepMemberData();
            result.Success = data != null;
            result.Message = strMessage;
            result.StepMemberData = data;
            return result;
        }

        // DisasterID와 isNormal이 서로 다를 경우
        // 주간/야간이 바뀌는 경우가 된다.
        public ResponseDisasterVersions GetDisasterVersions(int nDisasterID, bool isNormal)
        {
            string strErrorMessage;
            ArrayList arrResult = m_dataManager.GetSelectManager().JoinDisasterUserVersion(nDisasterID, out strErrorMessage);

            if (arrResult == null)
                return GetResponseDisasterVersions(null, null, strErrorMessage);

            int nResultCount = arrResult.Count;

            if (nResultCount % 3 != 0)
                return GetResponseDisasterVersions(null, null, "잘못된 결과를 받았습니다.");

            List<VersionData> versions = new List<VersionData>();
            VersionData currentVersion = null;

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                if (arrResult[i] is Disaster && arrResult[i + 1] is User && arrResult[i + 2] is Version)
                {
                    Disaster disaster = (Disaster)arrResult[i];
                    User user = (User)arrResult[i + 1];
                    Version version = (Version)arrResult[i + 2];

                    VersionData versionData = new VersionData(version);
                    versionData.Owner = user.UserID;

                    versions.Add(versionData);

                    if (disaster.ID == nDisasterID)
                        currentVersion = versionData;
                }
            }

            if (currentVersion != null)
            {
                if (currentVersion.IsNormal != isNormal)
                {
                    return GetDisasterVersions(currentVersion, isNormal);
                }
            }

            return GetResponseDisasterVersions(versions, currentVersion, "");
        }

        private ResponseDisasterVersions GetDisasterVersions(Version version, bool isNormal)
        {
            string strErrorMessage;
            ArrayList arrResult = m_dataManager.GetSelectManager().JoinDisasterUserVersionFromVersion(version.ID, isNormal, out strErrorMessage);

            if (arrResult == null)
                return GetResponseDisasterVersions(null, null, strErrorMessage);

            int nResultCount = arrResult.Count;

            if (nResultCount % 3 != 0)
                return GetResponseDisasterVersions(null, null, "잘못된 결과를 받았습니다.");

            List<VersionData> versions = new List<VersionData>();
            VersionData currentVersion = null;

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                if (arrResult[i] is Disaster && arrResult[i + 1] is User && arrResult[i + 2] is Version)
                {
                    Disaster disaster = (Disaster)arrResult[i];
                    User user = (User)arrResult[i + 1];
                    Version _version = (Version)arrResult[i + 2];

                    VersionData versionData = new VersionData(_version);
                    versionData.Owner = user.UserID;

                    versions.Add(versionData);

                    currentVersion = versionData;
                }
            }

            return GetResponseDisasterVersions(versions, currentVersion, "");
        }

        private ResponseDisasterVersions GetResponseDisasterVersions(List<VersionData> versions, VersionData currentVersion, string strMessage)
        {
            ResponseDisasterVersions result = new ResponseDisasterVersions();

            if (versions == null/* || currentVersion == null*/)
                result.Success = false;
            else
            {
                result.Success = true;

                result.Versions.AddRange(versions);
                result.CurrentVersion = currentVersion;
            }

            result.Message = strMessage;
            return result;
        }

        public ResponseOpen OpenXML(string strXML)
        {
            string strErrorMessage;
            SOPData sopData = XMLManager.OpenXML(strXML, m_dataManager.SiteID, out strErrorMessage);

            if (sopData == null)
                return GetResponseOpenXML(null, strErrorMessage);

            List<Common.Model.Option.Options> options = m_processManager.CommonDataManager.GetSelectManager().SelectOption(Common.Model.Option.Options.OptionTarget.SOPSimulator, "StandardActionStepNames", out strErrorMessage);

            if (options == null || strErrorMessage != null)
                return GetResponseOpenXML(null, strErrorMessage);

            List<string> standardActionStepNames = GetStandardActionStepNames(options);
            SetStandardActionSteps(sopData.ActionStepDatas, standardActionStepNames);

            return GetResponseOpenXML(sopData, strErrorMessage);
        }

        private void SetStandardActionSteps(List<ActionStepData> actionStepDatas, List<string> standardActionStepNames)
        {
            if (standardActionStepNames == null)
                return;

            int nStandardActionStepCount = standardActionStepNames.Count;

            for (int i=0;i<nStandardActionStepCount;i++)
            {
                string strActionStepName = standardActionStepNames[i];
                
                if (GetActionStepDataIndex(actionStepDatas, strActionStepName) < 0)
                {
                    ActionStepData actionStepData = new ActionStepData();
                    actionStepData = new ActionStepData();
                    actionStepData.StepName = strActionStepName;

                    actionStepDatas.Insert(i, actionStepData);
                }
            }
        }

        private int GetActionStepDataIndex(List<ActionStepData> actionStepDatas, string strActionStepName)
        {
            int nActionStepDataCount = actionStepDatas.Count;
            
            for (int i=0;i<nActionStepDataCount;i++)
            {
                ActionStepData actionStepData = actionStepDatas[i];

                if (actionStepData.StepName == strActionStepName)
                    return i;
            }

            return -1;
        }

        private ResponseOpen GetResponseOpenXML(SOPData sopData, string strMessage)
        {
            ResponseOpen result = new ResponseOpen();

            result.Success = sopData != null;
            result.Message = strMessage;
            result.SOPData = sopData;

            return result;
        }

        public ResponseOpen OpenDB(int nVersionID)
        {
            string strErrorMessage;
            ISelect selectManager = m_dataManager.GetSelectManager();

            ArrayList arrDatas = selectManager.JoinDisasterCategorySubDisasterCategoryDisasterUserVersion(nVersionID, out strErrorMessage);

            if (arrDatas == null)
                return GetResponseOpenDB(null, strErrorMessage);

            if (arrDatas.Count == 5 &&
                arrDatas[0] is DisasterCategory &&
                arrDatas[1] is SubDisasterCategory &&
                arrDatas[2] is Disaster &&
                arrDatas[3] is User &&
                arrDatas[4] is Version)
            {
                DisasterCategory dc = (DisasterCategory)arrDatas[0];
                SubDisasterCategory sdc = (SubDisasterCategory)arrDatas[1];
                Disaster disaster = (Disaster)arrDatas[2];
                User user = (User)arrDatas[3];
                Version version = (Version)arrDatas[4];

                SOPData sopData = new SOPData();

                sopData.DisasterCategory = dc;
                sopData.SubDisasterCategory = sdc;
                sopData.Disaster = disaster;
                sopData.Version = version;

                List<ActionStep> actionSteps = selectManager.SelectActionSteps(disaster, out strErrorMessage);

                if (actionSteps == null)
                    return GetResponseOpenDB(null, strErrorMessage);

                List<Regular> regularTeams = m_processManager.TeamDataManager.GetSelectManager().SelectRegulars(out strErrorMessage);

                if (regularTeams == null || strErrorMessage != null)
                    return GetResponseOpenDB(null, strErrorMessage);

                if (regularTeams.Count == 0)
                    return GetResponseOpenDB(null, Resource.ID.Get("errorMessage").Value("noRegularTeams"));

                List<Common.Model.Option.Options> options = m_processManager.CommonDataManager.GetSelectManager().SelectOption(Common.Model.Option.Options.OptionTarget.SOPSimulator, "StandardActionStepNames", out strErrorMessage);

                if (options == null || strErrorMessage != null)
                    return GetResponseOpenDB(null, strErrorMessage);

                List<string> standardActionStepNames = GetStandardActionStepNames(options);
                ActionStepData[] disasterActionSteps = standardActionStepNames != null && standardActionStepNames.Count > 0 ? new ActionStepData[standardActionStepNames.Count] : null;

                foreach (ActionStep actionStep in actionSteps)
                {
                    ActionStepData actionStepData = new ActionStepData();

                    int nIndex = standardActionStepNames.IndexOf(actionStep.StepName);

                    if (nIndex >= 0)
                        disasterActionSteps[nIndex] = actionStepData;
                    else
                        continue;

                    if (!LoadActionStepData(actionStepData, actionStep, regularTeams[0].TeamName, out strErrorMessage))
                        return GetResponseOpenDB(null, strErrorMessage);
                }

                if (disasterActionSteps != null)
                {
                    for (int i=0;i<disasterActionSteps.Length;i++)
                    {
                        ActionStepData actionStepData = disasterActionSteps[i];

                        if (actionStepData != null)
                        {
                            sopData.ActionStepDatas.Add(actionStepData);
                            continue;
                        }

                        actionStepData = new ActionStepData();
                        actionStepData.StepName = standardActionStepNames[i];
                        sopData.ActionStepDatas.Add(actionStepData);
                    }
                }

                return GetResponseOpenDB(sopData, "");
            }

            return GetResponseOpenDB(null, string.Format(Resource.ID.Get("errorMessageFormat").Value("unknownSOPVersion"), nVersionID));
        }

        public bool LoadActionStepData(ActionStepData actionStepData, ActionStep actionStep, string teamName, out string strErrorMessage)
        {
            ISelect selectManager = m_dataManager.GetSelectManager();

            actionStepData.ActionStep = actionStep;
            actionStepData.StepName = actionStep.StepName;

            List<StepMember> stepMembers = selectManager.SelectStepMembers(actionStep, out strErrorMessage);

            if (stepMembers == null)
                return false;

            foreach (StepMember stepMember in stepMembers)
            {
                StepMemberData stepMemberData = new StepMemberData();
                actionStepData.StepMemberDatas.Add(stepMemberData);

                stepMemberData.StepMember = stepMember;
                // StepMember Name은 노출되지 않으니 최상위 팀의 이름으로 정한다.
                stepMemberData.StepMemberName = teamName;

                Dictionary<int, SectionData> dicSectionDatas = new Dictionary<int, SectionData>();
                stepMemberData.Sections = LoadSectionDatas(stepMember, dicSectionDatas, ref strErrorMessage);

                if (stepMemberData.Sections == null)
                    return false;

                stepMemberData.Arrows = LoadArrowDatas(stepMember, dicSectionDatas, ref strErrorMessage);

                if (stepMemberData.Arrows == null)
                    return false;

                if (LoadGrid(stepMemberData, ref strErrorMessage) == false)
                    return false;
            }

            return true;
        }

        private bool LoadGrid(StepMemberData stepMemberData, ref string strErrorMessage)
        {
            Dictionary<SectionGrid.Fields, object> dicConditions = new Dictionary<SectionGrid.Fields, object>();
            dicConditions[SectionGrid.Fields.StepMemberID] = stepMemberData.StepMember.ID;

            List<SectionGrid> grids = m_dataManager.GetSelectManager().SelectGrids(dicConditions, out strErrorMessage);

            if (grids == null)
                return false;

            if (grids.Count == 0)
            {
                strErrorMessage = "Cell 정보가 존재하지 않습니다.";
                return false;
            }

            SectionGrid grid = grids[0];

            Dictionary<SectionGridColumn.Fields, object> dicCondition1 = new Dictionary<SectionGridColumn.Fields, object>();
            dicCondition1[SectionGridColumn.Fields.GridID] = grid.ID;

            List<SectionGridColumn> columns = m_dataManager.GetSelectManager().SelectGridColumns(dicCondition1, out strErrorMessage);

            if (columns == null)
                return false;

            if (columns.Count == 0)
            {
                strErrorMessage = "Cell Column 정보가 존재하지 않습니다.";
                return false;
            }

            columns.Sort();

            foreach (SectionGridColumn column in columns)
            {
                stepMemberData.GridColumnWidth.Add(column.Width);
            }

            Dictionary<SectionGridRow.Fields, object> dicCondition2 = new Dictionary<SectionGridRow.Fields, object>();
            dicCondition2[SectionGridRow.Fields.GridID] = grid.ID;

            List<SectionGridRow> rows = m_dataManager.GetSelectManager().SelectGridRows(dicCondition2, out strErrorMessage);

            if (rows == null)
                return false;

            if (rows.Count == 0)
            {
                strErrorMessage = "Cell Row 정보가 존재하지 않습니다.";
                return false;
            }

            rows.Sort();

            foreach (SectionGridRow row in rows)
            {
                stepMemberData.GridRowHeight.Add(row.Height);
            }

            return true;
        }

        private List<ArrowData> LoadArrowDatas(StepMember stepMember, Dictionary<int, SectionData> dicSectionDatas, ref string strErrorMessage)
        {
            List<Arrow> arrows = m_dataManager.GetSelectManager().SelectArrows(stepMember.ID, out strErrorMessage);

            if (arrows == null)
                return null;

            List<ArrowData> arrowDatas = new List<ArrowData>();
            SectionData sectionBegin, sectionEnd;

            foreach (Arrow arrow in arrows)
            {
                ArrowData arrowData = new ArrowData();

                if (dicSectionDatas.TryGetValue(arrow.BeginComponentID, out sectionBegin) == false ||
                    dicSectionDatas.TryGetValue(arrow.EndComponentID, out sectionEnd) == false)
                    continue;

                arrowData.BeginComponentID = arrow.BeginComponentID;
                arrowData.BeginComponentColumnIndex = sectionBegin.GridColumnIndex;
                arrowData.BeginComponentRowIndex = sectionBegin.GridRowIndex;
                arrowData.BeginComponentPosition = arrow.BeginComponentPosition;
                arrowData.EndComponentID = arrow.EndComponentID;
                arrowData.EndComponentColumnIndex = sectionEnd.GridColumnIndex;
                arrowData.EndComponentRowIndex = sectionEnd.GridRowIndex;
                arrowData.EndComponentPosition = arrow.EndComponentPosition;
                arrowData.ID = arrow.ID;
                arrowData.Text = arrow.Text;

                arrowDatas.Add(arrowData);
            }

            return arrowDatas;
        }

        // dicSectionDatas.Key : Component + Component Type 정보(처음 1Byte는 Type 정보, 뒤 3Byte는 ComponentID)
        private List<SectionData> LoadSectionDatas(StepMember stepMember, Dictionary<int, SectionData> dicSectionDatas, ref string strErrorMessage)
        {
            ISelect selectManager = m_dataManager.GetSelectManager();

            List<Annotation> annotations = selectManager.SelectAnnotations(stepMember.ID, out strErrorMessage);

            if (annotations == null)
                return null;

            List<Decision> decisions = selectManager.SelectDecisions(stepMember.ID, out strErrorMessage);

            if (decisions == null)
                return null;

            List<EndPoint> endpoints = selectManager.SelectEndPoints(stepMember.ID, out strErrorMessage);

            if (endpoints == null)
                return null;

            List<InternalTransmission> internals = selectManager.SelectInternalTransmissions(stepMember.ID, out strErrorMessage);

            if (internals == null)
                return null;

            List<Process> processes = selectManager.SelectProcesses(stepMember.ID, out strErrorMessage);

            if (processes == null)
                return null;

            List<int> processIDs = new List<int>();

            foreach (Process process in processes)
            {
                processIDs.Add(process.ID);
            }

            List<ExternalProgram> externalPrograms = selectManager.SelectExternalPrograms("", out strErrorMessage);

            if (externalPrograms == null)
                return null;

            Dictionary<int, ExternalProgram> dicExternalPrograms = ToDictionary(externalPrograms);
            List<ProcessMission> processMissions = selectManager.SelectProcessMissions(processIDs, out strErrorMessage);

            if (processMissions == null)
                return null;

            List<ProcessExternalMission> processExternalMissions = selectManager.SelectProcessExternalMissions(processIDs, out strErrorMessage);

            if (processExternalMissions == null)
                return null;

            List<SectionData> sectionDatas = new List<SectionData>();

            ToSectionDatas(annotations, sectionDatas);
            ToSectionDatas(decisions, sectionDatas);
            ToSectionDatas(endpoints, sectionDatas);
            ToSectionDatas(internals, sectionDatas);
            ToSectionDatas(processes, processMissions, processExternalMissions, dicExternalPrograms, sectionDatas);

            foreach (SectionData sectionData in sectionDatas)
            {
                int nType = (sectionData.ComponentType << 24);
                int key = (nType | sectionData.ID);
                dicSectionDatas[key] = sectionData;
            }

            // SectionNumber가 null일 경우 맨 뒤로 보냄
            List<SectionData> sortSectionDatas = sectionDatas.OrderBy(x => x.SectionNumber != null ? x.SectionNumber : 9999999).ToList();

            return sortSectionDatas;
        }

        private Dictionary<int, ExternalProgram> ToDictionary(List<ExternalProgram> externalPrograms)
        {
            Dictionary<int, ExternalProgram> dicExternalPrograms = new Dictionary<int, ExternalProgram>();

            foreach (ExternalProgram externalProgram in externalPrograms)
            {
                dicExternalPrograms[externalProgram.ID] = externalProgram;
            }

            return dicExternalPrograms;
        }

        private void ToSectionDatas(List<Process> processes, List<ProcessMission> processMissions, List<ProcessExternalMission> processExternalMissions, Dictionary<int, ExternalProgram> dicExternalPrograms, List<SectionData> sectionDatas)
        {
            Dictionary<int, SectionData> dicSectionDatas = new Dictionary<int, SectionData>();

            foreach (Process process in processes)
            {
                SectionData sectionData = ToSectionData(process);
                sectionData.Text = process.Text;
                sectionData.OnlyTeamLeader = process.OnlyTeamLeader;
                sectionData.AutoRun = process.AutoRun;

                foreach (Receiver receiver in process.TeamList)
                {
                    if (sectionData.Receivers == null)
                        sectionData.Receivers = new List<Receiver>();

                    sectionData.Receivers.Add(receiver);
                }

                sectionDatas.Add(sectionData);
                dicSectionDatas[process.ID] = sectionData;
            }

            foreach (ProcessMission processMission in processMissions)
            {
                SectionData sectionData;

                if (dicSectionDatas.TryGetValue(processMission.ProcessID, out sectionData))
                {
                    if (sectionData.Missions == null)
                        sectionData.Missions = new List<ProcessMissionData>();

                    sectionData.Missions.Add(ProcessMissionDataSorter.ToMissionData(processMission));
                }
            }

            foreach (ProcessExternalMission processExternalMission in processExternalMissions)
            {
                SectionData sectionData;

                if (dicSectionDatas.TryGetValue(processExternalMission.ProcessID, out sectionData))
                {
                    if (sectionData.Missions == null)
                        sectionData.Missions = new List<ProcessMissionData>();

                    ProcessMissionData missionData = ProcessMissionDataSorter.ToMissionData(processExternalMission);
                    sectionData.Missions.Add(missionData);

                    ExternalProgram externalProgram;

                    if (missionData.ProgramID != null && dicExternalPrograms.TryGetValue((int)missionData.ProgramID, out externalProgram))
                    {
                        missionData.ProgramName = externalProgram.Description != null && externalProgram.Description.Length > 0 ? externalProgram.Description : externalProgram.ExeName;
                    }
                }
            }

            foreach (KeyValuePair<int, SectionData> pair in dicSectionDatas)
            {
                ProcessMissionDataSorter.Sort(pair.Value.Missions);
            }
        }

        private void ToSectionDatas(List<InternalTransmission> internals, List<SectionData> sectionDatas)
        {
            foreach (InternalTransmission _internal in internals)
            {
                SectionData sectionData = ToSectionData(_internal);
                sectionData.Text = _internal.Text;
                sectionData.Message = _internal.Message;
                sectionData.IsSMS = _internal.UseSMS;
                sectionData.IsBroadcast = _internal.UseBroadcast;
                sectionData.IsEmail = _internal.UseEmail;
                sectionData.OnlyTeamLeader = _internal.OnlyTeamLeader;
                sectionData.AutoRun = _internal.AutoRun;
                sectionData.UseSiren = _internal.UseSiren;

                foreach (Receiver receiver in _internal.TeamList)
                {
                    if (sectionData.Receivers == null)
                        sectionData.Receivers = new List<Receiver>();

                    sectionData.Receivers.Add(receiver);
                }

                sectionDatas.Add(sectionData);
            }
        }

        private void ToSectionDatas(List<EndPoint> endpoints, List<SectionData> sectionDatas)
        {
            foreach (EndPoint endpoint in endpoints)
            {
                SectionData sectionData = ToSectionData(endpoint);
                sectionData.Text = endpoint.Text;
                sectionData.IsBegin = endpoint.IsBegin;

                sectionDatas.Add(sectionData);
            }
        }

        private void ToSectionDatas(List<Decision> decisions, List<SectionData> sectionDatas)
        {
            foreach (Decision decision in decisions)
            {
                SectionData sectionData = ToSectionData(decision);
                sectionData.Text = decision.Text;
                sectionData.TeamID = decision.TeamID;
                sectionData.TeamType = decision.TeamType;
                sectionData.AutoRunScript = decision.AutoRunScript;
                sectionData.AutoRunScriptVariableTypes = decision.AutoRunScriptVariableTypes;
                sectionData.Description = decision.Description;

                sectionDatas.Add(sectionData);
            }
        }

        private void ToSectionDatas(List<Annotation> annotations, List<SectionData> sectionDatas)
        {
            foreach (Annotation annotation in annotations)
            {
                SectionData sectionData = ToSectionData(annotation);
                sectionData.Text = annotation.Text;
                sectionDatas.Add(sectionData);
            }
        }

        private SectionData ToSectionData(Section section)
        {
            SectionData sectionData = new SectionData();

            sectionData.ID = section.ID;
            sectionData.ComponentType = section.ComponentType;
            sectionData.ComponentID = section.ComponentID;
            sectionData.GridID = section.GridID;
            sectionData.GridColumnIndex = section.GridColumnIndex;
            sectionData.GridRowIndex = section.GridRowIndex;
            sectionData.Width = section.Width;
            sectionData.Height = section.Height;
            sectionData.SectionNumber = section.SectionNumber;

            return sectionData;
        }

        private ResponseOpen GetResponseOpenDB(SOPData sopData, string strMessage)
        {
            ResponseOpen result = new ResponseOpen();

            result.Success = sopData != null;
            result.Message = strMessage;
            result.SOPData = sopData;

            return result;
        }

        // nProgramID : 0보다 작으면 전체 리스트를 얻어온다.
        public ResponseExternalProgram GetExternalPrograms(int nProgramID)
        {
            string strErrorMessage;
            List<ExternalProgram> programs = null;

            if (nProgramID >= 0)
            {
                Dictionary<ExternalProgram.Fields, object> dicConditions = new Dictionary<ExternalProgram.Fields, object>();
                dicConditions[ExternalProgram.Fields.ID] = nProgramID;
                programs = m_dataManager.GetSelectManager().SelectExternalPrograms(dicConditions, out strErrorMessage);
            }
            else
            {
                programs = m_dataManager.GetSelectManager().SelectExternalPrograms("", out strErrorMessage);
            }

            if (programs == null)
                return GetResponseExternalProgram(null, strErrorMessage);

            Dictionary<int, ExternalProgramData> dicProgramDatas = new Dictionary<int, ExternalProgramData>();
            List<ExternalProgramData> programDatas = new List<ExternalProgramData>();

            string strProgramIDs = "";

            foreach (ExternalProgram program in programs)
            {
                if (strProgramIDs.Length == 0)
                    strProgramIDs = program.ID.ToString();
                else
                    strProgramIDs += ", " + program.ID.ToString();

                ExternalProgramData programData = new ExternalProgramData();
                programData.Program = program;

                dicProgramDatas[program.ID] = programData;
                programDatas.Add(programData);
            }

            if (strProgramIDs.Length == 0)
            {
                return GetResponseExternalProgram(programDatas, "");
            }

            bool isNullable;
            string strCondition = string.Format("{0} in ({1})", ExternalProgramParameter.GetFieldName(ExternalProgramParameter.Fields.ProgramID, out isNullable), strProgramIDs);
            List<ExternalProgramParameter> parameters = m_dataManager.GetSelectManager().SelectExternalProgramParameters(strCondition, out strErrorMessage);

            if (parameters == null)
                return GetResponseExternalProgram(null, strErrorMessage);

            foreach (ExternalProgramParameter parameter in parameters)
            {
                ExternalProgramData programData;

                if (dicProgramDatas.TryGetValue(parameter.ProgramID, out programData))
                {
                    programData.Parameters.Add(parameter);
                }
            }

            foreach (ExternalProgramData programData in programDatas)
            {
                programData.Parameters.Sort();
            }

            return GetResponseExternalProgram(programDatas, "");
        }

        private ResponseExternalProgram GetResponseExternalProgram(List<ExternalProgramData> programDatas, string strMessage)
        {
            ResponseExternalProgram result = new ResponseExternalProgram();

            result.Success = programDatas != null;
            result.Message = strMessage;

            if (programDatas != null)
            {
                result.Programs.AddRange(programDatas);
            }

            return result;
        }

        public ResponseOption GetOption(RequestOption data)
        {
            ResponseOption result = new ResponseOption();

            Dictionary<Option.Fields, object> dicCondition = new Dictionary<Option.Fields, object>();
            dicCondition.Add(Option.Fields.UserID, data.UserID);
            dicCondition.Add(Option.Fields.Category, data.Category);

            string strErrorMessage = null;
            List<Option> options = m_processManager.SopDataManager.GetSelectManager().SelectOptions(dicCondition, out strErrorMessage);
            if (options == null)
            {
                result.Success = false;
                result.Message = "사용자의 옵션 정보를 읽을 수 없습니다.";
                return result;
            }

            result.Success = true;
            result.Options = options;
            return result;
        }

        public ResponseParseSpecialMessage ParseSpecialMessage(RequestParseSpecialMessage data)
        {
            string strErrorMessage;
            DateTime? time = data.GetTime(out strErrorMessage);

            if (time == null)
                return new ResponseParseSpecialMessage(false, "", strErrorMessage);

            dnsData.Script.SOP.DataParameter parameter = new dnsData.Script.SOP.DataParameter(data.Message, (DateTime)time);

            if (data.Location != null && data.Location.Length > 0)
                parameter.Place = data.Location;

            parameter.RealMode = data.IsRealMode;
            parameter.NormalMode = data.IsNormalMode;

            string strKey, strValue;

            foreach (string strVariable in data.Variables)
            {
                if (RequestParseSpecialMessage.GetVariableData(strVariable, out strKey, out strValue))
                {
                    parameter.AddData(strKey, strValue);
                }
            }

            string strParse = dnsData.Script.SOP.Parse(parameter);
            return new ResponseParseSpecialMessage(true, strParse, null);
        }

        public ResponseSpecialMessageList GetSpecialMessageList()
        {
            string strErrorMessage;
            List<SpecialMessage> messages = m_dataManager.GetSelectManager().SelectSpecialMessages(null, out strErrorMessage);

            if (messages == null)
                return new ResponseSpecialMessageList(false, null, strErrorMessage);

            return new ResponseSpecialMessageList(true, messages, null);
        }

        public ResponseLinkedSOPs GetLinkedSOPs()
        {
            ResponseLinkedSOPs linkedSOPs = new ResponseLinkedSOPs();

            string strErrorMessage;
            Dictionary<LinkedSop.Fields, object> dicCondition = new Dictionary<LinkedSop.Fields, object>();
            List<LinkedSop> linkedSops = m_dataManager.GetSelectManager().SelectLinkedSops(dicCondition, out strErrorMessage);

            if (linkedSops == null)
            {
                linkedSOPs.Success = false;
                linkedSOPs.Message = strErrorMessage;
                return linkedSOPs;
            }

            linkedSOPs.Success = true;
            linkedSOPs.LinkedSops = linkedSops;
            return linkedSOPs;
        }
    }
}
