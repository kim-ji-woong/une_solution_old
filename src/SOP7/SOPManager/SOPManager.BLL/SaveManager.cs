using System.Collections.Generic;
using TeamEditor.Model.Sop.Team;

namespace SOPManager.BLL
{
    using IDAL;
    using Model.Sop.Category;
    using Model.Sop.Component;
    using Models.SOP;
    using Models.Response;
    using SOPManager.Model.Sop.Account;

    public class SaveManager
    {
        private IDataManager m_dataManager = null;
        private TeamEditor.IDAL.IDataManager m_teamDataManager = null;
        private ProcessManager m_processManager = null;

        public SaveManager(IDataManager manager, TeamEditor.IDAL.IDataManager teamDataManager, ProcessManager processManager)
        {
            m_dataManager = manager;
            m_teamDataManager = teamDataManager;
            m_processManager = processManager;
        }

        public bool IsRunningVersion(int nVersionID)
        {
            if (m_dataManager == null)
                return false;

            string strErrorMessage;
            return m_dataManager.GetSelectManager().IsRunningVersion(nVersionID, out strErrorMessage);
        }

        public ResponseSave SaveXML(SOPData sopData)
        {
            string strErrorMessage;
            Dictionary<ActionStepData, bool> dicActiveActionSteps = new Dictionary<ActionStepData, bool>();

            if (CheckSOPValidation(sopData.ActionStepDatas, dicActiveActionSteps, out strErrorMessage) == false)
                return GetResponseSaveXML(null, null, null, strErrorMessage);

            if (sopData.DisasterCategory == null)
                return GetResponseSaveXML(null, null, null, Resource.ID.Get("errorMessage").Value("noDisasterCategory"));

            if (sopData.SubDisasterCategory == null)
                return GetResponseSaveXML(null, null, null, Resource.ID.Get("errorMessage").Value("noSubDisasterCategory"));

            if (sopData.Disaster == null)
                return GetResponseSaveXML(null, null, null, Resource.ID.Get("errorMessage").Value("noDisaster"));

            string strXMLFileName;
            string strXML = XMLManager.Save(m_dataManager, sopData, dicActiveActionSteps, out strXMLFileName, out strErrorMessage);

            if (strXML == null || strXML.Length == 0)
                return GetResponseSaveXML(null, null, null, strErrorMessage);

            return GetResponseSaveXML(sopData, strXML, strXMLFileName, "");
        }

        private ResponseSave GetResponseSaveXML(SOPData sopData, string strXML, string strXMLFileName, string strMessage)
        {
            ResponseSave result = new ResponseSave();

            if (sopData == null || strXML == null)
            {
                result.Success = false;
            }
            else
            {
                result.Success = true;
                result.SOPData = sopData;
                result.XMLData = strXML;
                result.XMLFileName = strXMLFileName;
            }

            result.Message = strMessage;
            return result;
        }

        public ResponseSave SaveDB(int nUserID, SOPData sopData)
        {
            string strErrorMessage;
            Dictionary<ActionStepData, bool> dicActiveActionSteps = new Dictionary<ActionStepData, bool>();

            if (CheckSOPValidation(sopData.ActionStepDatas, dicActiveActionSteps, out strErrorMessage) == false)
                return GetResponseSaveDB(null, strErrorMessage);

            if (sopData.DisasterCategory == null)
                return GetResponseSaveDB(null, Resource.ID.Get("errorMessage").Value("noDisasterCategory"));

            if (sopData.SubDisasterCategory == null)
                return GetResponseSaveDB(null, Resource.ID.Get("errorMessage").Value("noSubDisasterCategory"));

            if (sopData.Disaster == null)
                return GetResponseSaveDB(null, Resource.ID.Get("errorMessage").Value("noDisaster"));

            RollbackManager rollback = new RollbackManager();

            if (CheckNSave(sopData.DisasterCategory, rollback) == false)
            {
                rollback.Rollback(m_dataManager);
                return GetResponseSaveDB(null, Resource.ID.Get("errorMessage").Value("failSaveDisasterCategory"));
            }

            sopData.SubDisasterCategory.DisasterCategoryID = sopData.DisasterCategory.ID;

            if (CheckNSave(sopData.SubDisasterCategory, rollback) == false)
            {
                rollback.Rollback(m_dataManager);
                return GetResponseSaveDB(null, Resource.ID.Get("errorMessage").Value("failSaveSubDisasterCategory"));
            }

            Version version = sopData.Version;

            if (CheckNSave(ref version, nUserID, rollback) == false)
            {
                rollback.Rollback(m_dataManager);
                return GetResponseSaveDB(null, Resource.ID.Get("errorMessage").Value("failSaveVersion"));
            }

            sopData.Disaster.VersionID = version.ID;
            sopData.Disaster.SubDisasterCategoryID = sopData.SubDisasterCategory.ID;

            if (Save(sopData.Disaster, rollback) == false)
            {
                rollback.Rollback(m_dataManager);
                return GetResponseSaveDB(null, Resource.ID.Get("errorMessage").Value("failSave"));
            }

            foreach (ActionStepData actionStepData in sopData.ActionStepDatas)
            {
                if (actionStepData.ActionStep == null || actionStepData.StepMemberDatas == null ||
                    actionStepData.StepMemberDatas.Count == 0)
                    continue;

                if (dicActiveActionSteps.ContainsKey(actionStepData) == false)
                    continue;

                actionStepData.ActionStep.DisasterID = sopData.Disaster.ID;

                if (Save(actionStepData.ActionStep, rollback) == false)
                {
                    rollback.Rollback(m_dataManager);
                    return GetResponseSaveDB(null, Resource.ID.Get("errorMessage").Value("failSaveActionStep"));
                }

                SetSectionNumbers(actionStepData.StepMemberDatas);

                foreach (StepMemberData stepMemberData in actionStepData.StepMemberDatas)
                {
                    if (stepMemberData.StepMember == null)
                        continue;

                    stepMemberData.StepMember.ActionStepID = actionStepData.ActionStep.ID;

                    if (Save(stepMemberData.StepMember, rollback) == false)
                    {
                        rollback.Rollback(m_dataManager);
                        return GetResponseSaveDB(null, Resource.ID.Get("errorMessage").Value("failSaveStepMember"));
                    }

                    Dictionary<Section, SectionData> dicSectionDatas = new Dictionary<Section, SectionData>();
                    Dictionary<long, Section> dicGridSections = new Dictionary<long, Section>();
                    SectionGrid grid = null;

                    int nGridRowCount, nGridColumnCount;
                    GetGridSize(stepMemberData.Sections, out nGridColumnCount, out nGridRowCount);

                    if (nGridColumnCount > 0 && nGridRowCount > 0)
                    {
                        grid = SaveGrid(stepMemberData, nGridRowCount, nGridColumnCount, rollback);

                        if (grid == null)
                        {
                            rollback.Rollback(m_dataManager);
                            return GetResponseSaveDB(null, Resource.ID.Get("errorMessage").Value("failSaveGrid"));
                        }

                        List<Section> sections = SectionDataToSections(stepMemberData.Sections, grid.ID, stepMemberData.StepMember.ID, dicGridSections, dicSectionDatas);

                        foreach (Section section in sections)
                        {
                            section.GridID = grid.ID;
                            section.StepMemberID = stepMemberData.StepMember.ID;

                            if (Save(section, rollback) == false)
                            {
                                rollback.Rollback(m_dataManager);
                                return GetResponseSaveDB(null, Resource.ID.Get("errorMessage").Value("failSaveSection"));
                            }

                            SectionData sectionData;

                            if (dicSectionDatas.TryGetValue(section, out sectionData))
                            {
                                sectionData.ID = section.ID;
                                sectionData.GridID = section.GridID;
                            }
                        }
                    }

                    Dictionary<Arrow, ArrowData> dicArrowDatas = new Dictionary<Arrow, ArrowData>();
                    List<Arrow> arrows = ArrowDataToArrows(stepMemberData.Arrows, dicGridSections, stepMemberData.StepMember.ID, dicArrowDatas);

                    foreach (Arrow arrow in arrows)
                    {
                        if (Save(arrow, rollback) == false)
                        {
                            rollback.Rollback(m_dataManager);
                            return GetResponseSaveDB(null, Resource.ID.Get("errorMessage").Value("failSaveArrow"));
                        }

                        ArrowData arrowData;

                        if (dicArrowDatas.TryGetValue(arrow, out arrowData))
                        {
                            arrowData.ID = arrow.ID;
                            arrowData.BeginComponentID = arrow.BeginComponentID;
                            arrowData.EndComponentID = arrow.EndComponentID;
                        }
                    }
                }
            }

            return GetResponseSaveDB(sopData, "");
        }

        private void SetSectionNumbers(List<StepMemberData> stepMemberDatas)
        {
            SectionData beginSection = null;
            Dictionary<SectionData, List<SectionData>> dicLinkedSections = new Dictionary<SectionData, List<SectionData>>();

            List<SectionData> linkedSections;

            foreach (StepMemberData stepMemberData in stepMemberDatas)
            {
                Dictionary<long, SectionData> dicSections = new Dictionary<long, SectionData>();

                foreach (SectionData sectionData in stepMemberData.Sections)
                {
                    if (sectionData.ComponentType == (int)Section.SectionType.Endpoint && sectionData.IsBegin == true)
                    {
                        beginSection = sectionData;
                    }

                    long key = ((((long)sectionData.GridColumnIndex) << 32) | ((long)sectionData.GridRowIndex));
                    dicSections[key] = sectionData;
                }

                foreach (ArrowData arrowData in stepMemberData.Arrows)
                {
                    if (arrowData.BeginComponentColumnIndex >= 0 && arrowData.BeginComponentRowIndex >= 0 &&
                        arrowData.EndComponentColumnIndex >= 0 && arrowData.EndComponentRowIndex >= 0)
                    {
                        long keyBegin = ((((long)arrowData.BeginComponentColumnIndex) << 32) | ((long)arrowData.BeginComponentRowIndex));
                        long keyEnd = ((((long)arrowData.EndComponentColumnIndex) << 32) | ((long)arrowData.EndComponentRowIndex));

                        SectionData sectionBegin, sectionEnd;

                        if (dicSections.TryGetValue(keyBegin, out sectionBegin) && dicSections.TryGetValue(keyEnd, out sectionEnd))
                        {
                            if (dicLinkedSections.TryGetValue(sectionBegin, out linkedSections) == false)
                            {
                                linkedSections = new List<SectionData>();
                                dicLinkedSections[sectionBegin] = linkedSections;
                            }

                            if (linkedSections.Contains(sectionEnd) == false)
                            {
                                linkedSections.Add(sectionEnd);
                            }
                        }
                    }
                }
            }

            if (beginSection == null)
                return;

            int nSectionNumber = 1;
            beginSection.SectionNumber = nSectionNumber;
            SetSectionNumbers(beginSection, dicLinkedSections, ref nSectionNumber);
        }

        private void SetSectionNumbers(SectionData sectionData, Dictionary<SectionData, List<SectionData>> dicLinkedSections, ref int nSectionNumber)
        {
            List<SectionData> sections;

            if (dicLinkedSections.TryGetValue(sectionData, out sections))
            {
                List<SectionData> sectionDatas = new List<SectionData>();

                foreach (SectionData section in sections)
                {
                    if (section.ComponentType == (int)Section.SectionType.Annotation)
                        continue;

                    if (section.SectionNumber == null)
                    {
                        section.SectionNumber = ++nSectionNumber;
                        sectionDatas.Add(section);
                    }
                }

                foreach (SectionData section in sectionDatas)
                {
                    SetSectionNumbers(section, dicLinkedSections, ref nSectionNumber);
                }
            }
        }

        private List<Arrow> ArrowDataToArrows(List<ArrowData> arrowDatas, Dictionary<long, Section> dicGridSections, int nStepMemberID, Dictionary<Arrow, ArrowData> dicArrowDatas)
        {
            List<Arrow> arrows = new List<Arrow>();

            foreach (ArrowData arrowData in arrowDatas)
            {
                Section sectionBegin = GetSection(arrowData.BeginComponentColumnIndex, arrowData.BeginComponentRowIndex, dicGridSections);

                if (sectionBegin == null)
                    continue;

                Section sectionEnd = GetSection(arrowData.EndComponentColumnIndex, arrowData.EndComponentRowIndex, dicGridSections);

                if (sectionEnd == null)
                    continue;

                Arrow arrow = new Arrow();

                arrow.BeginComponentPosition = arrowData.BeginComponentPosition;
                arrow.BeginSection = sectionBegin;
                arrow.EndComponentPosition = arrowData.EndComponentPosition;
                arrow.EndSection = sectionEnd;
                arrow.StepMemberID = nStepMemberID;
                arrow.Text = arrowData.Text;

                arrows.Add(arrow);
                dicArrowDatas[arrow] = arrowData;
            }

            return arrows;
        }

        private Section GetSection(int nGridColumnIndex, int nGridRowIndex, Dictionary<long, Section> dicGridSections)
        {
            Section section;
            long gridIndex = (((long)nGridColumnIndex) << 32 | ((long)nGridRowIndex));

            if (dicGridSections.TryGetValue(gridIndex, out section))
                return section;

            return null;
        }

        // dicGridSections : Grid 위치별 Section들
        //                   Key => 상위 4바이트 : GridColumnIndex, 하위 4바이트 : GridRowIndex
        private List<Section> SectionDataToSections(List<SectionData> sectionDatas, int nGridID, int nStepMemberID, Dictionary<long, Section> dicGridSections, Dictionary<Section, SectionData> dicSectionDatas)
        {
            List<Section> sections = new List<Section>();

            foreach (SectionData sectionData in sectionDatas)
            {
                Section section = null;

                if (sectionData.ComponentType == (int)Section.SectionType.Annotation)
                {
                    section = MakeAnnotation(sectionData, nGridID, nStepMemberID);
                }
                else if (sectionData.ComponentType == (int)Section.SectionType.Decision)
                {
                    section = MakeDecision(sectionData, nGridID, nStepMemberID);
                }
                else if (sectionData.ComponentType == (int)Section.SectionType.Endpoint)
                {
                    section = MakeEndPoint(sectionData, nGridID, nStepMemberID);
                }
                else if (sectionData.ComponentType == (int)Section.SectionType.Internal)
                {
                    section = MakeInternal(sectionData, nGridID, nStepMemberID);
                }
                else if (sectionData.ComponentType == (int)Section.SectionType.Process)
                {
                    section = MakeProcess(sectionData, nGridID, nStepMemberID);
                }

                if (section == null)
                    continue;

                long gridIndex = (((long)section.GridColumnIndex) << 32 | ((long)section.GridRowIndex));
                dicGridSections[gridIndex] = section;

                sections.Add(section);
                dicSectionDatas[section] = sectionData;
            }

            return sections;
        }

        private Process MakeProcess(SectionData sectionData, int nGridID, int nStepMemberID)
        {
            Process process = new Process();
            MakeSection(process, sectionData, nGridID, nStepMemberID);

            process.Text = sectionData.Text;
            process.OnlyTeamLeader = sectionData.OnlyTeamLeader;
            process.AutoRun = sectionData.AutoRun == null ? false : (bool)sectionData.AutoRun;

            if (sectionData.Missions != null)
                process.Missions.AddRange(sectionData.Missions);

            if (sectionData.Receivers != null)
            {
                foreach (Receiver receiver in sectionData.Receivers)
                {
                    process.AddTeam(receiver.TeamType, receiver.TeamID);
                }
            }

            /*if (sectionData.TeamID != null && sectionData.TeamType != null)
            {
                process.AddTeam((int)sectionData.TeamType, (int)sectionData.TeamID);
            }*/

            return process;
        }

        private InternalTransmission MakeInternal(SectionData sectionData, int nGridID, int nStepMemberID)
        {
            InternalTransmission _internal = new InternalTransmission();
            MakeSection(_internal, sectionData, nGridID, nStepMemberID);

            _internal.Text = sectionData.Text;
            _internal.UseSMS = sectionData.IsSMS == null ? false : (bool)sectionData.IsSMS;
            _internal.UseBroadcast = sectionData.IsBroadcast == null ? false : (bool)sectionData.IsBroadcast;
            _internal.UseEmail = sectionData.IsEmail == null ? false : (bool)sectionData.IsEmail;
            _internal.Message = sectionData.Message;

            if (sectionData.Receivers != null)
            {
                foreach (Receiver receiver in sectionData.Receivers)
                {
                    _internal.AddTeam(receiver.TeamType, receiver.TeamID);
                }
            }

            /*if (sectionData.TeamID != null && sectionData.TeamType != null)
            {
                _internal.AddTeam((int)sectionData.TeamType, (int)sectionData.TeamID);
            }*/

            _internal.OnlyTeamLeader = sectionData.OnlyTeamLeader;
            _internal.AutoRun = sectionData.AutoRun == null ? false : (bool)sectionData.AutoRun;

            return _internal;
        }

        private EndPoint MakeEndPoint(SectionData sectionData, int nGridID, int nStepMemberID)
        {
            if (sectionData.IsBegin == null)
                return null;

            EndPoint endpoint = new EndPoint();
            MakeSection(endpoint, sectionData, nGridID, nStepMemberID);

            endpoint.Text = sectionData.Text;
            endpoint.IsBegin = (bool)sectionData.IsBegin;
            
            return endpoint;
        }

        private Decision MakeDecision(SectionData sectionData, int nGridID, int nStepMemberID)
        {
            Decision decision = new Decision();
            MakeSection(decision, sectionData, nGridID, nStepMemberID);

            decision.Text = sectionData.Text;
            decision.TeamID = sectionData.TeamID;
            decision.TeamType = sectionData.TeamType;
            decision.AutoRunScript = sectionData.AutoRunScript;
            decision.AutoRunScriptVariableTypes = sectionData.AutoRunScriptVariableTypes;
            decision.Description = sectionData.Description;

            return decision;
        }

        private Annotation MakeAnnotation(SectionData sectionData, int nGridID, int nStepMemberID)
        {
            Annotation annotation = new Annotation();
            MakeSection(annotation, sectionData, nGridID, nStepMemberID);
            annotation.Text = sectionData.Text;

            return annotation;
        }

        private void MakeSection(Section section, SectionData sectionData, int nGridID, int nStepMemberID)
        {
            section.GridID = nGridID;
            section.GridColumnIndex = sectionData.GridColumnIndex;
            section.GridRowIndex = sectionData.GridRowIndex;
            section.Width = sectionData.Width;
            section.Height = sectionData.Height;
            section.ComponentID = sectionData.ComponentID;
            section.StepMemberID = nStepMemberID;
            section.SectionNumber = sectionData.SectionNumber;
        }

        private ResponseSave GetResponseSaveDB(SOPData sopData, string strMessage)
        {
            ResponseSave result = new ResponseSave();

            if (sopData == null)
            {
                result.Success = false;
            }
            else
            {
                result.Success = true;
                result.SOPData = sopData;
            }

            result.Message = strMessage;
            return result;
        }

        private bool CheckSOPValidation(List<ActionStepData> actionStepDatas, Dictionary<ActionStepData, bool> dicActiveActionSteps, out string strErrorMessage)
        {
            int activeActionStepCount = 0;

            foreach (ActionStepData actionStepData in actionStepDatas)
            {
                if (actionStepData.StepMemberDatas.Count == 0)
                    continue;

                // SOP는 반드시 시작 Component와 종료 Component가 하나 이상씩 존재해야 한다.
                bool? begin = null, end = null;
                int nSectionCount = 0;

                foreach (StepMemberData stepMemberData in actionStepData.StepMemberDatas)
                {
                    foreach (SectionData sectionData in stepMemberData.Sections)
                    {
                        nSectionCount++;
                        if (sectionData.ComponentType == (int)Section.SectionType.Endpoint)
                        {
                            if (sectionData.IsBegin != null)
                            {
                                if ((bool)sectionData.IsBegin)
                                    begin = true;
                                else
                                    end = true;

                                if (begin != null && end != null)
                                    break;
                            }
                        }
                    }

                    if (begin != null && end != null)
                        break;
                }

                if (nSectionCount == 0)
                    continue;

                if (begin == null || begin == false)
                {
                    strErrorMessage = string.Format(Resource.ID.Get("errorMessageFormat").Value("noBeginComponent"), actionStepData.StepName);
                    return false;
                }
                else if (end == null || end == false)
                {
                    strErrorMessage = string.Format(Resource.ID.Get("errorMessageFormat").Value("noEndComponent"), actionStepData.StepName);
                    return false;
                }

                activeActionStepCount++;
                dicActiveActionSteps[actionStepData] = true;
            }

            if (activeActionStepCount == 0)
            {
                strErrorMessage = Resource.ID.Get("errorMessage").Value("noSOPDatas");
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        /*private bool SaveSOP(int nUserID, DisasterCategory dc, SubDisasterCategory sdc, Disaster disaster, ref Version version, Dictionary<ActionStep, List<StepMember>> dicActionSteps, Dictionary<StepMember, List<Section>> dicStepMemberSections, Dictionary<StepMember, List<Arrow>> dicStepMemberArrows)
        {
            if (m_dataManager == null || m_dataManager.GetCreateManager() == null)
                return false;

            if (dc == null || sdc == null || disaster == null)
                return false;

            RollbackManager rollback = new RollbackManager();

            if (CheckNSave(dc, rollback) == false)
                return false;

            sdc.DisasterCategoryID = dc.ID;

            if (CheckNSave(sdc, rollback) == false)
            {
                rollback.Rollback(m_dataManager);
                return false;
            }

            if (CheckNSave(ref version, nUserID, rollback) == false)
            {
                rollback.Rollback(m_dataManager);
                return false;
            }

            disaster.VersionID = version.ID;
            disaster.SubDisasterCategoryID = sdc.ID;

            if (Save(disaster, rollback) == false)
            {
                rollback.Rollback(m_dataManager);
                return false;
            }

            foreach (KeyValuePair<ActionStep, List<StepMember>> pair in dicActionSteps)
            {
                pair.Key.DisasterID = disaster.ID;

                if (Save(pair.Key, rollback) == false)
                {
                    rollback.Rollback(m_dataManager);
                    return false;
                }

                foreach (StepMember stepMember in pair.Value)
                {
                    stepMember.ActionStepID = pair.Key.ID;

                    if (Save(stepMember, rollback) == false)
                    {
                        rollback.Rollback(m_dataManager);
                        return false;
                    }

                    List<Section> sections;
                    List<Arrow> arrows;
                    SectionGrid grid = null;

                    if (dicStepMemberSections.TryGetValue(stepMember, out sections))
                    {
                        int nGridRowCount, nGridColumnCount;
                        GetGridSize(sections, out nGridColumnCount, out nGridRowCount);

                        if (nGridColumnCount > 0 && nGridRowCount > 0)
                        {
                            grid = SaveGrid(stepMember.ID, nGridRowCount, nGridColumnCount, rollback);

                            if (grid == null)
                                return false;

                            foreach (Section section in sections)
                            {
                                section.GridID = grid.ID;
                                section.StepMemberID = stepMember.ID;

                                if (Save(section, rollback) == false)
                                {
                                    rollback.Rollback(m_dataManager);
                                    return false;
                                }
                            }
                        }
                    }

                    if (grid != null && dicStepMemberArrows.TryGetValue(stepMember, out arrows))
                    {
                        foreach (Arrow arrow in arrows)
                        {
                            arrow.StepMemberID = stepMember.ID;

                            if (Save(arrow, rollback) == false)
                            {
                                rollback.Rollback(m_dataManager);
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }*/

        private SectionGrid SaveGrid(StepMemberData stepMemberData, int nGridRowCount, int nGridColumnCount, RollbackManager rollback)
        {
            int nStepMemberID = stepMemberData.StepMember.ID;

            ICreate createManager = m_dataManager.GetCreateManager();
            SectionGrid grid = createManager.CreateGrid(nStepMemberID);

            if (grid == null)
                return null;
            else
            {
                IRollbackData rollbackData = m_dataManager.MakeRollbackDataInstance();

                if (rollbackData.AddDeleteRollback(string.Format("Delete from {0} where ID = {1}", SectionGrid.TableName, grid.ID)) == false)
                    return null;

                rollback.AddData(rollbackData);
            }

            //int nDefaultCellWidth = 300;
            //int nDefaultCellHeight = 200;

            IRollbackData _rollbackData = m_dataManager.MakeRollbackDataInstance();

            if (_rollbackData.AddDeleteRollback(string.Format("Delete from {0} where GridID = {1}", SectionGridRow.TableName, grid.ID)) == false)
                return null;

            rollback.AddData(_rollbackData);

            _rollbackData = m_dataManager.MakeRollbackDataInstance();

            if (_rollbackData.AddDeleteRollback(string.Format("Delete from {0} where GridID = {1}", SectionGridColumn.TableName, grid.ID)) == false)
                return null;

            rollback.AddData(_rollbackData);

            for (int i=0;i<nGridRowCount;i++)
            {
                int nCellHeight = stepMemberData.GridRowHeight[i];
                SectionGridRow row = createManager.CreateGridRow(grid.ID, i, nCellHeight/*nDefaultCellHeight*/);

                if (row == null)
                    return null;
            }

            for (int i = 0; i < nGridColumnCount; i++)
            {
                int nCellWidth = stepMemberData.GridColumnWidth[i];
                SectionGridColumn column = createManager.CreateGridColumn(grid.ID, i, nCellWidth/*nDefaultCellWidth*/);

                if (column == null)
                    return null;
            }

            return grid;
        }

        private void GetGridSize(List<SectionData> sectionDatas, out int nGridColumnCount, out int nGridRowCount)
        {
            int nMaxColumnIndex = -1, nMaxRowIndex = -1;

            foreach (SectionData sectionData in sectionDatas)
            {
                if (sectionData.GridColumnIndex > nMaxColumnIndex)
                    nMaxColumnIndex = sectionData.GridColumnIndex;

                if (sectionData.GridRowIndex > nMaxRowIndex)
                    nMaxRowIndex = sectionData.GridRowIndex;
            }

            nGridColumnCount = nMaxColumnIndex + 1;
            nGridRowCount = nMaxRowIndex + 1;
        }

        private bool Save(Arrow arrow, RollbackManager rollback)
        {
            if (arrow.MakeComponentID() == false)
                return false;

            Arrow _arrow = m_dataManager.GetCreateManager().CreateArrow(arrow.BeginComponentID, arrow.BeginComponentPosition, arrow.EndComponentID, arrow.EndComponentPosition, arrow.StepMemberID, arrow.Text);

            if (_arrow == null)
                return false;
            else
            {
                IRollbackData rollbackData = m_dataManager.MakeRollbackDataInstance();

                if (rollbackData.AddDeleteRollback(string.Format("Delete from {0} where ID = {1}", Arrow.TableName, _arrow.ID)) == false)
                    return false;

                rollback.AddData(rollbackData);
                arrow.ID = _arrow.ID;
            }

            return true;
        }

        private bool Save(Section section, RollbackManager rollback)
        {
            if (section.ComponentType == (int)Section.SectionType.Annotation)
                return SaveAnnotation((Annotation)section, rollback);
            else if (section.ComponentType == (int)Section.SectionType.Decision)
                return SaveDecision((Decision)section, rollback);
            else if (section.ComponentType == (int)Section.SectionType.Endpoint)
                return SaveEndpoint((EndPoint)section, rollback);
            else if (section.ComponentType == (int)Section.SectionType.Internal)
                return SaveInternal((InternalTransmission)section, rollback);
            else if (section.ComponentType == (int)Section.SectionType.Process)
                return SaveProcess((Process)section, rollback);

            return true;
        }

        private bool SaveProcess(Process process, RollbackManager rollback)
        {
            Process _process = m_dataManager.GetCreateManager().CreateProcess(process.GridID, process.GridRowIndex, process.GridColumnIndex, process.Width, process.Height, process.Text, process.TeamList, process.ComponentID, process.StepMemberID, process.AutoRun, process.OnlyTeamLeader, process.SectionNumber);

            if (_process == null)
                return false;
            else
            {
                IRollbackData rollbackData = m_dataManager.MakeRollbackDataInstance();

                if (rollbackData.AddDeleteRollback(string.Format("Delete from {0} where ID = {1}", Process.TableName, _process.ID)) == false)
                    return false;

                rollback.AddData(rollbackData);
                process.ID = _process.ID;

                foreach (ProcessMissionData missionData in process.Missions)
                {
                    if (missionData.MissionType == ProcessMissionData.MissionDataType.Normal)
                    {
                        ProcessMission _mission = m_dataManager.GetCreateManager().CreateProcessMission(missionData.MissionText, process.ID);

                        if (_mission == null)
                            return false;
                        else
                        {
                            rollbackData = m_dataManager.MakeRollbackDataInstance();

                            if (rollbackData.AddDeleteRollback(string.Format("Delete from {0} where ID = {1}", ProcessMission.TableName, _mission.ID)) == false)
                                return false;

                            rollback.AddData(rollbackData);
                            missionData.ID = _mission.ID;
                            missionData.ProcessID = _mission.ProcessID;
                        }
                    }
                    else if (missionData.MissionType == ProcessMissionData.MissionDataType.External)
                    {
                        List<ProcessExternalMission> externalMissions = ProcessMissionDataSorter.GetExternalMissions(missionData);

                        if (externalMissions == null)
                            return false;

                        foreach (ProcessExternalMission externalMission in externalMissions)
                        {
                            ProcessExternalMission _externalMission = m_dataManager.GetCreateManager().CreateProcessExternalMission(process.ID, externalMission.OrderIndex, externalMission.ProgramID, externalMission.ParameterIndex, externalMission.Value);

                            if (_externalMission == null)
                                return false;
                            else
                            {
                                bool isNullable;
                                rollbackData = m_dataManager.MakeRollbackDataInstance();

                                if (rollbackData.AddDeleteRollback(string.Format("Delete from {0} where {1} = {2} and {3} = {4} and {5} = {6} and {7} = {8}",
                                        ProcessExternalMission.TableName,
                                        ProcessExternalMission.GetFieldName(ProcessExternalMission.Fields.ProcessID, out isNullable),
                                        _externalMission.ProcessID,
                                        ProcessExternalMission.GetFieldName(ProcessExternalMission.Fields.OrderIndex, out isNullable),
                                        _externalMission.OrderIndex,
                                        ProcessExternalMission.GetFieldName(ProcessExternalMission.Fields.ProgramID, out isNullable),
                                        _externalMission.ProgramID,
                                        ProcessExternalMission.GetFieldName(ProcessExternalMission.Fields.ParameterIndex, out isNullable),
                                        _externalMission.ParameterIndex)) == false)
                                    return false;

                                rollback.AddData(rollbackData);
                            }
                        }
                    }
                }
            }

            return true;
        }

        private bool SaveInternal(InternalTransmission internalSection, RollbackManager rollback)
        {
            InternalTransmission _internal = m_dataManager.GetCreateManager().CreateInternalTransmission(internalSection.GridID, internalSection.GridRowIndex, internalSection.GridColumnIndex, internalSection.Width, internalSection.Height, internalSection.Text, internalSection.ComponentID, internalSection.UseSMS, internalSection.UseBroadcast, internalSection.UseEmail == null ? false : (bool)internalSection.UseEmail, internalSection.StepMemberID, internalSection.AutoRun, internalSection.Message, internalSection.TeamList, internalSection.UseSiren, internalSection.OnlyTeamLeader, internalSection.SectionNumber);

            if (_internal == null)
                return false;
            else
            {
                IRollbackData rollbackData = m_dataManager.MakeRollbackDataInstance();

                if (rollbackData.AddDeleteRollback(string.Format("Delete from {0} where ID = {1}", InternalTransmission.TableName, _internal.ID)) == false)
                    return false;

                rollback.AddData(rollbackData);
                internalSection.ID = _internal.ID;
            }

            return true;
        }

        private bool SaveEndpoint(EndPoint endpoint, RollbackManager rollback)
        {
            EndPoint _endpoint = m_dataManager.GetCreateManager().CreateEndPoint(endpoint.GridID, endpoint.GridRowIndex, endpoint.GridColumnIndex, endpoint.Width, endpoint.Height, endpoint.Text, endpoint.ComponentID, endpoint.IsBegin, endpoint.StepMemberID, endpoint.SectionNumber);

            if (_endpoint == null)
                return false;
            else
            {
                IRollbackData rollbackData = m_dataManager.MakeRollbackDataInstance();

                if (rollbackData.AddDeleteRollback(string.Format("Delete from {0} where ID = {1}", EndPoint.TableName, _endpoint.ID)) == false)
                    return false;

                rollback.AddData(rollbackData);
                endpoint.ID = _endpoint.ID;
            }

            return true;
        }

        private bool SaveDecision(Decision decision, RollbackManager rollback)
        {
            Decision _decision = m_dataManager.GetCreateManager().CreateDecision(decision.GridID, decision.GridRowIndex, decision.GridColumnIndex, decision.Width, decision.Height, decision.Text, decision.ComponentID, decision.StepMemberID, decision.TeamID, decision.TeamType, decision.SectionNumber, decision.Description);

            if (_decision == null)
                return false;
            else
            {
                IRollbackData rollbackData = m_dataManager.MakeRollbackDataInstance();

                if (rollbackData.AddDeleteRollback(string.Format("Delete from {0} where ID = {1}", Decision.TableName, _decision.ID)) == false)
                    return false;

                rollback.AddData(rollbackData);
                decision.ID = _decision.ID;
            }

            return true;
        }

        private bool SaveAnnotation(Annotation annotation, RollbackManager rollback)
        {
            Annotation _annotation = m_dataManager.GetCreateManager().CreateAnnotation(annotation.GridID, annotation.GridRowIndex, annotation.GridColumnIndex, annotation.Width, annotation.Height, annotation.Text, annotation.ComponentID, annotation.StepMemberID, annotation.SectionNumber);

            if (_annotation == null)
                return false;
            else
            {
                IRollbackData rollbackData = m_dataManager.MakeRollbackDataInstance();

                if (rollbackData.AddDeleteRollback(string.Format("Delete from {0} where ID = {1}", Annotation.TableName, _annotation.ID)) == false)
                    return false;

                rollback.AddData(rollbackData);
                annotation.ID = _annotation.ID;
            }

            return true;
        }

        private bool Save(StepMember stepMember, RollbackManager rollback)
        {
            if (stepMember.TeamType < 0)
            {
                string strErrorMessage;
                List<Regular> regularTeams = m_teamDataManager.GetSelectManager().SelectRegulars(out strErrorMessage);

                if (strErrorMessage != null || regularTeams == null)
                    return false;

                if (regularTeams.Count == 0)
                    return false;

                stepMember.TeamType = (int)StepMember.MemberTeamType.RegularTeam;
                stepMember.TeamID = regularTeams[0].ID;
            }

            StepMember _stepMember = m_dataManager.GetCreateManager().CreateStepMember(stepMember.TeamID, stepMember.TeamType, stepMember.ActionStepID);

            if (_stepMember == null)
                return false;
            else
            {
                IRollbackData rollbackData = m_dataManager.MakeRollbackDataInstance();

                if (rollbackData.AddDeleteRollback(string.Format("Delete from {0} where ID = {1}", ActionStep.TableName, _stepMember.ID)) == false)
                    return false;

                rollback.AddData(rollbackData);
                stepMember.ID = _stepMember.ID;
            }

            return true;
        }

        private bool Save(ActionStep actionStep, RollbackManager rollback)
        {
            if (actionStep.StepName.Length == 0)
                return false;

            ActionStep _actionStep = m_dataManager.GetCreateManager().CreateActionStep(actionStep.StepName, actionStep.DisasterID);

            if (_actionStep == null)
                return false;
            else
            {
                IRollbackData rollbackData = m_dataManager.MakeRollbackDataInstance();

                if (rollbackData.AddDeleteRollback(string.Format("Delete from {0} where ID = {1}", ActionStep.TableName, _actionStep.ID)) == false)
                    return false;

                rollback.AddData(rollbackData);
                actionStep.ID = _actionStep.ID;
            }
 
            return true;
        }

        private bool Save(Disaster disaster, RollbackManager rollback)
        {
            if (disaster.DisasterName == null || disaster.DisasterName.Length == 0)
                return false;

            Disaster _disaster = m_dataManager.GetCreateManager().CreateDisaster(disaster.DisasterName, disaster.SubDisasterCategoryID, disaster.VersionID);

            if (_disaster == null)
                return false;
            else
            {
                IRollbackData rollbackData = m_dataManager.MakeRollbackDataInstance();

                if (rollbackData.AddDeleteRollback(string.Format("Delete from {0} where ID = {1}", Disaster.TableName, _disaster.ID)) == false)
                    return false;

                rollback.AddData(rollbackData);
                disaster.ID = _disaster.ID;
            }

            return true;
        }

        // DB에 저장되어 있는지 확인하여, 이미 저장되어 있으면 그냥 true를 리턴하고 빠져나온다.
        // 그렇지 않다면 rollback에 RollbackData를 넣은후 DB에 값을 저장한다.
        private bool CheckNSave(ref Version version, int nUserID, RollbackManager rollback)
        {
            if (version == null || version.ID < 0)
            {
                string strVersionName = "V1.0";
                bool isNormal = true;
                string strDescription = null;

                if (version != null)
                {
                    strVersionName = version.VersionName.Length > 0 ? version.VersionName : "V1.0";
                    isNormal = version.IsNormal;
                    strDescription = version.Description;
                }

                System.DateTime dtNow = System.DateTime.Now;

                Version _version = m_dataManager.GetCreateManager().CreateVersion(isNormal, dtNow, dtNow, strVersionName, nUserID, m_dataManager.SiteID, strDescription);

                if (_version == null)
                    return false;
                else
                {
                    IRollbackData rollbackData = m_dataManager.MakeRollbackDataInstance();

                    if (rollbackData.AddDeleteRollback(string.Format("Delete from {0} where ID = {1}", Version.TableName, _version.ID)) == false)
                        return false;

                    rollback.AddData(rollbackData);
                    version = _version;
                }
            }
            else
            {
                string strPrevTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", version.LastAccessTime.Year, version.LastAccessTime.Month, version.LastAccessTime.Day, version.LastAccessTime.Hour, version.LastAccessTime.Minute, version.LastAccessTime.Second);

                System.DateTime dtNow = System.DateTime.Now;
                version.LastAccessTime = dtNow;
                version.SiteID = m_dataManager.SiteID;

                IRollbackData rollbackData = m_dataManager.MakeRollbackDataInstance();

                if (rollbackData.AddUpdateRollback(string.Format("Update {0} set LastAccessTime = '{1}' where ID = {2}", Version.TableName, strPrevTime, version.ID)) == false)
                    return false;

                if (m_dataManager.GetUpdateManager().UpdateVersion(version, "ID = " + version.ID.ToString()) == false)
                    return false;

                rollback.AddData(rollbackData);

                // 기존 버전을 삭제한다.
                if (m_processManager.GetDeleteManager().DeleteSOPVersion(version.ID, false, rollback, true) == false)
                    return false;
            }

            return true;
        }

        // DB에 저장되어 있는지 확인하여, 이미 저장되어 있으면 그냥 true를 리턴하고 빠져나온다.
        // 그렇지 않다면 rollback에 RollbackData를 넣은후 DB에 값을 저장한다.
        private bool CheckNSave(SubDisasterCategory sdc, RollbackManager rollback)
        {
            if (sdc.ID < 0)
            {
                if (sdc.SubCategoryName == null || sdc.SubCategoryName.Length == 0)
                    return false;

                Dictionary<SubDisasterCategory.Fields, object> dicCondition = new Dictionary<SubDisasterCategory.Fields, object>();
                dicCondition[SubDisasterCategory.Fields.SubCategoryName] = sdc.SubCategoryName;

                string strErrorMessage;
                List<SubDisasterCategory> subDisasterCategories = m_dataManager.GetSelectManager().SelectSubDisasterCategories(dicCondition, out strErrorMessage);

                if (subDisasterCategories == null)
                    return false;

                if (subDisasterCategories.Count > 0)
                {
                    sdc.ID = subDisasterCategories[0].ID;
                }
                else
                {
                    SubDisasterCategory _sdc = m_dataManager.GetCreateManager().CreateSubDisasterCategory(sdc.DisasterCategoryID, sdc.SubCategoryName);

                    if (_sdc == null)
                        return false;
                    else
                    {
                        IRollbackData rollbackData = m_dataManager.MakeRollbackDataInstance();

                        if (rollbackData.AddDeleteRollback(string.Format("Delete from {0} where ID = {1}", SubDisasterCategory.TableName, _sdc.ID)) == false)
                            return false;

                        rollback.AddData(rollbackData);
                        sdc.ID = _sdc.ID;
                    }
                }
            }

            return true;
        }

        // DB에 저장되어 있는지 확인하여, 이미 저장되어 있으면 그냥 true를 리턴하고 빠져나온다.
        // 그렇지 않다면 rollback에 RollbackData를 넣은후 DB에 값을 저장한다.
        private bool CheckNSave(DisasterCategory dc, RollbackManager rollback)
        {
            if (dc.ID < 0)
            {
                if (dc.CategoryName == null || dc.CategoryName.Length == 0)
                    return false;

                Dictionary<DisasterCategory.Fields, object> dicCondition = new Dictionary<DisasterCategory.Fields, object>();
                dicCondition[DisasterCategory.Fields.CategoryName] = dc.CategoryName;

                string strErrorMessage;
                List<DisasterCategory> disasterCategories = m_dataManager.GetSelectManager().SelectDisasterCategories(dicCondition, out strErrorMessage);

                if (disasterCategories == null)
                    return false;

                if (disasterCategories.Count > 0)
                {
                    dc.ID = disasterCategories[0].ID;
                }
                else
                {
                    DisasterCategory _dc = m_dataManager.GetCreateManager().CreateDisasterCategory(dc.CategoryName, m_dataManager.SiteID);

                    if (_dc == null)
                        return false;
                    else
                    {
                        IRollbackData rollbackData = m_dataManager.MakeRollbackDataInstance();

                        if (rollbackData.AddDeleteRollback(string.Format("Delete from {0} where ID = {1}", DisasterCategory.TableName, _dc.ID)) == false)
                            return false;

                        rollback.AddData(rollbackData);
                        dc.ID = _dc.ID;
                    }
                }
            }

            return true;
        }

        public ResponseOption SaveAccountOption(SOPManager.Model.Sop.Account.Option option)
        {
            string strErrorMessage = null;
            Option result = null;

            if (option.ID <= 0)
            {
                int? id = GetAccountOptionID(option, out strErrorMessage);

                if (id == null && strErrorMessage != null)
                {
                    ResponseOption response = new ResponseOption();
                    response.Success = false;
                    response.Message = strErrorMessage;
                    return response;
                }
            }

            if (option.ID <= 0)
            {   // 없으면 생성
                result = m_dataManager.GetCreateManager().CreateOption(option.UserID, option.Category, option.SubCategory, option.PropertyValue1, option.PropertyValue2, option.PropertyValue3, option.PropertyValue4);
            }
            else
            {   // 있으면 업데이트
                m_dataManager.GetUpdateManager().UpdateOption(option);
                result = m_dataManager.GetSelectManager().SelectOption(option.ID, out strErrorMessage);                
            }

            ResponseOption res = new ResponseOption();
            if (result == null)
            {
                res.Success = false;
                res.Message = strErrorMessage;
            }
            else
            {
                res.Success = true;
                if (res.Options == null)
                    res.Options = new List<Option>();
                res.Options.Add(result);
            }

            return res;
        }

        private int? GetAccountOptionID(Model.Sop.Account.Option option, out string strErrorMessage)
        {
            Dictionary<Model.Sop.Account.Option.Fields, object> dicConditions = new Dictionary<Option.Fields, object>();
            dicConditions[Model.Sop.Account.Option.Fields.UserID] = option.UserID;
            dicConditions[Model.Sop.Account.Option.Fields.Category] = option.Category;
            dicConditions[Model.Sop.Account.Option.Fields.SubCategory] = option.SubCategory;

            List<Model.Sop.Account.Option> options = m_dataManager.GetSelectManager().SelectOptions(dicConditions, out strErrorMessage);

            if (options == null || options.Count == 0)
                return null;

            option.ID = options[0].ID;
            return option.ID;
        }
    }
}
