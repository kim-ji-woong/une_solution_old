using System;
using System.Collections.Generic;
using System.Text;
using Common.Model.History;

namespace SOPManager.BLL
{
    using IDAL;
    using Model.Sop.Category;
    using Model.Sop.Component;

    public class DeleteManager
    {
        private SOPManager.IDAL.IDataManager m_sopDataManager = null;
        private Common.IDAL.IDataManager m_commonDataManager = null;
        private ProcessManager m_processManager = null;

        public DeleteManager(Common.IDAL.IDataManager commonDataManager, SOPManager.IDAL.IDataManager sopDataManager, ProcessManager processManager)
        {
            m_commonDataManager = commonDataManager;
            m_sopDataManager = sopDataManager;
            m_processManager = processManager;
        }

        public bool DeleteDisasterType(string strDisasterName, int nSubDisasterCategoryID)
        {
            bool isNullable;
            string strCondition = string.Format("{2} = '{0}' and {3} = {1}",
                strDisasterName,
                nSubDisasterCategoryID,
                DisasterType.GetFieldName(DisasterType.Fields.Name, out isNullable),
                DisasterType.GetFieldName(DisasterType.Fields.SubDisasterID, out isNullable));
            return m_sopDataManager.GetDeleteManager().DeleteDisasterType(strCondition);
        }

        public bool DeleteSOPVersions(List<int> versionIDs, out string strErrorMessage)
        {
            strErrorMessage = null;

            RollbackManager rollback = new RollbackManager();

            foreach (int versionID in versionIDs)
            {
                if (DeleteSOPVersion(versionID, true, rollback, false, out strErrorMessage) == false)
                {
                    rollback.Rollback(m_processManager.SopDataManager);
                    return false;
                }
            }

            return true;
        }

        public bool DeleteSOPVersion(int nVersionID, bool deleteVersion, RollbackManager rollback, bool noCommit = false)
        {
            string strErrorMessage;
            return DeleteSOPVersion(nVersionID, deleteVersion, rollback, noCommit, out strErrorMessage);
        }

        private bool DeleteSOPVersion(int nVersionID, bool deleteVersion, RollbackManager rollback, bool noCommit, out string strErrorMessage)
        {
            strErrorMessage = null;

            if (m_sopDataManager == null || m_processManager == null)
                return false;

            // 기존에 실행중인 버전은 삭제 되지 않도록 검사하여 id를 가져오지 못하도록 한다.
            // 수정 : 2014-11-13 skkim
            // 모니터링 중인 SOP의 처리방법
            // - 삭제 : 삭제하지 못하도록 막는다.
            // - 저장 : 반드시 새버전으로 저장하도록 한다.
            if (m_processManager.GetSaveManager().IsRunningVersion(nVersionID))
            {
                return false;
            }

            ISelect selectManager = m_sopDataManager.GetSelectManager();

            bool isNullable;
            string strCondition = string.Format("{0} = {1}", Disaster.GetFieldName(Disaster.Fields.VersionID, out isNullable), nVersionID);
            List<Disaster> disasters = selectManager.SelectDisasters(strCondition, out strErrorMessage);

            if (disasters == null || strErrorMessage != null)
                return false;

            if (disasters.Count == 0)
            {
                if (deleteVersion)
                    return DeleteVersion(nVersionID, rollback);
                return true;
            }

            string strDisasterIDs = "";

            foreach (Disaster disaster in disasters)
            {
                if (strDisasterIDs.Length == 0)
                    strDisasterIDs = disaster.ID.ToString();
                else
                    strDisasterIDs += ", " + disaster.ID.ToString();
            }

            List<ActionStep> actionSteps = selectManager.SelectActionSteps(string.Format("{0} in ({1})", ActionStep.GetFieldName(ActionStep.Fields.DisasterID, out isNullable), strDisasterIDs), out strErrorMessage);

            if (actionSteps == null || strErrorMessage != null)
                return false;

            if (actionSteps.Count == 0)
            {
                if (!DeleteDisaster(nVersionID, rollback))
                    return false;

                if (deleteVersion)
                    return DeleteVersion(nVersionID, rollback);
                return true;
            }

            string strActionStepIDs = "";

            foreach (ActionStep actionStep in actionSteps)
            {
                if (strActionStepIDs.Length == 0)
                    strActionStepIDs = actionStep.ID.ToString();
                else
                    strActionStepIDs += ", " + actionStep.ID.ToString();
            }

            strCondition = string.Format("{0} in ({1})", StepMember.GetFieldName(StepMember.Fields.ActionStepID, out isNullable), strActionStepIDs);
            List<StepMember> stepMembers = selectManager.SelectStepMembers(strCondition, out strErrorMessage);

            if (stepMembers == null)
                return false;

            if (stepMembers.Count == 0)
            {
                if (!DeleteActionStepHistory(strActionStepIDs, rollback))
                    return false;
                if (!DeleteActionStep(strDisasterIDs, rollback))
                    return false;
                if (!DeleteDisaster(nVersionID, rollback))
                    return false;
                if (deleteVersion)
                    return DeleteVersion(nVersionID, rollback);

                if (m_processManager.NetworkManager != null)
                    m_processManager.NetworkManager.SendDeletedActionStepIDs(strActionStepIDs);
                return true;
            }

            string strStepMemberIDs = "";

            foreach (StepMember stepMember in stepMembers)
            {
                if (strStepMemberIDs.Length == 0)
                    strStepMemberIDs = stepMember.ID.ToString();
                else
                    strStepMemberIDs += ", " + stepMember.ID.ToString();
            }

            if (strStepMemberIDs.Length > 0)
            {
                if (!DeleteComponent(strStepMemberIDs, rollback))
                {
                    return false;
                }

                if (!DeleteSectionGrid(strStepMemberIDs, rollback))
                {
                    return false;
                }
            }
            if (!DeleteActionStepHistory(strActionStepIDs, rollback))
            {
                return false;
            }
            if (!DeleteStepMember(strActionStepIDs, rollback))
            {
                return false;
            }
            if (!DeleteActionStep(strDisasterIDs, rollback))
            {
                return false;
            }
            if (!DeleteDisaster(nVersionID, rollback))
            {
                return false;
            }
            if (deleteVersion)
            {
                if (!DeleteVersion(nVersionID, rollback))
                {
                    return false;
                }
            }

            if (m_processManager.NetworkManager != null)
                m_processManager.NetworkManager.SendDeletedActionStepIDs(strActionStepIDs);

            return true;
        }

        private bool DeleteVersion(int nVersionID, RollbackManager rollback)
        {
            if (rollback != null)
            {
                IRollbackData rollbackData = m_sopDataManager.MakeRollbackDataInstance();

                bool isNullable;
                string strSQL = string.Format("Select {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7} from {8} where ID = {9}",
                    Version.GetFieldName(Version.Fields.ID, out isNullable),
                    Version.GetFieldName(Version.Fields.IsNormal, out isNullable),
                    Version.GetFieldName(Version.Fields.CreateTime, out isNullable),
                    Version.GetFieldName(Version.Fields.LastAccessTime, out isNullable),
                    Version.GetFieldName(Version.Fields.VersionName, out isNullable),
                    Version.GetFieldName(Version.Fields.OwnerID, out isNullable),
                    Version.GetFieldName(Version.Fields.Description, out isNullable),
                    Version.GetFieldName(Version.Fields.SiteID, out isNullable),
                    Version.TableName,
                    nVersionID);

                if (rollbackData.AddInsertRollback(m_sopDataManager, strSQL, 0, 0, 1, 1, 1, 0, 1, 0) == false)
                    return false;

                if (m_sopDataManager.GetDeleteManager().DeleteVersion(nVersionID))
                {
                    rollback.AddData(rollbackData);
                    return true;
                }
                else
                    return false;
            }

            return m_sopDataManager.GetDeleteManager().DeleteVersion(nVersionID);
        }

        private bool DeleteDisaster(int nVersionID, RollbackManager rollback)
        {
            //if (DeleteFacilityType(nVersionID) == false)
            //    return false;

            bool isNullable;
            string strCondition = string.Format("{0} = {1}", Disaster.GetFieldName(Disaster.Fields.VersionID, out isNullable), nVersionID);

            if (rollback != null)
            {
                IRollbackData rollbackData = m_sopDataManager.MakeRollbackDataInstance();

                string strSQL = string.Format("Select {0}, {1}, {2}, {3}, {4}, {5} from {6} where VersionID = {7}",
                    Disaster.GetFieldName(Disaster.Fields.ID, out isNullable),
                    Disaster.GetFieldName(Disaster.Fields.DisasterName, out isNullable),
                    Disaster.GetFieldName(Disaster.Fields.SubDisasterCategoryID, out isNullable),
                    Disaster.GetFieldName(Disaster.Fields.VersionID, out isNullable),
                    Disaster.GetFieldName(Disaster.Fields.UserLevelIDs, out isNullable),
                    Disaster.GetFieldName(Disaster.Fields.Description, out isNullable),
                    Disaster.TableName,
                    nVersionID);

                if (rollbackData.AddInsertRollback(m_sopDataManager, strSQL, 0, 1, 0, 0, 1, 1) == false)
                    return false;

                if (m_sopDataManager.GetDeleteManager().DeleteDisaster(strCondition))
                {
                    rollback.AddData(rollbackData);
                    return true;
                }
                else
                    return false;
            }

            return m_sopDataManager.GetDeleteManager().DeleteDisaster(strCondition);
        }

        //private bool DeleteFacilityType(int nVersionID)
        //{
        //    Dictionary<Disaster.Fields, object> dicConditions1 = new Dictionary<Disaster.Fields, object>();
        //    dicConditions1[Disaster.Fields.VersionID] = nVersionID;

        //    string strErrorMessage;
        //    List<Disaster> disasters = m_sopDataManager.GetSelectManager().SelectDisasters(dicConditions1, out strErrorMessage);

        //    if (disasters == null || disasters.Count == 0)
        //        return false;

        //    Dictionary<SDMS.Model.Sensor.FacilityType.Fields, object> dicSets = new Dictionary<SDMS.Model.Sensor.FacilityType.Fields, object>();
        //    dicSets[SDMS.Model.Sensor.FacilityType.Fields.DisasterID] = null;

        //    Dictionary<SDMS.Model.Sensor.FacilityType.Fields, object> dicConditions2 = new Dictionary<SDMS.Model.Sensor.FacilityType.Fields, object>();
        //    dicConditions2[SDMS.Model.Sensor.FacilityType.Fields.DisasterID] = disasters[0].ID;

        //    return m_processManager.SDMSDataManager.GetUpdateManager().UpdateFacilityType(dicSets, dicConditions2, null, out strErrorMessage);
        //}

        private bool DeleteActionStepHistory(string strActionStepIDs, RollbackManager rollback)
        {
            string strErrorMessage;
            Common.IDAL.ISelect selectManager = m_commonDataManager.GetSelectManager();

            bool isNullable;
            string strCondition = string.Format("{0} in ({1})", ActionStepHistory.GetFieldName(ActionStepHistory.Fields.ActionStepID, out isNullable), strActionStepIDs);
            List<ActionStepHistory> actionStepHistories = selectManager.SelectActionStepHistories(strCondition, out strErrorMessage);

            if (actionStepHistories == null)
                return false;

            string strActionStepHistoryIDs = "";
            
            foreach (ActionStepHistory actionStepHistory in actionStepHistories)
            {
                if (strActionStepHistoryIDs.Length == 0)
                    strActionStepHistoryIDs = actionStepHistory.ID.ToString();
                else
                    strActionStepHistoryIDs += ", " + actionStepHistory.ID.ToString();
            }

            if (strActionStepHistoryIDs.Length == 0)
                return true;

            strCondition = string.Format("{0} in ({1})", ComponentHistory.GetFieldName(ComponentHistory.Fields.ActionStepHistoryID, out isNullable), strActionStepHistoryIDs);
            List<ComponentHistory> componentHistories = selectManager.SelectComponentHistories(strCondition, out strErrorMessage);

            if (componentHistories == null)
                return false;

            string strComponentHistoryIDs = "";

            foreach (ComponentHistory componentHistory in componentHistories)
            {
                if (strComponentHistoryIDs.Length == 0)
                    strComponentHistoryIDs = componentHistory.ID.ToString();
                else
                    strComponentHistoryIDs += ", " + componentHistory.ID.ToString();
            }

            if (strComponentHistoryIDs != null && strComponentHistoryIDs != "")
            {
                strCondition = string.Format("{0} in ({1})", ComponentHistoryDetail.GetFieldName(ComponentHistoryDetail.Fields.ComponentHistoryID, out isNullable), strComponentHistoryIDs);

                if (rollback != null)
                {
                    IRollbackData rollbackData = m_sopDataManager.MakeRollbackDataInstance();

                    string strSQL = string.Format("Select {0}, {1}, {2}, {3}, {4}, {5}, {6} from {7} where {8}",
                        ComponentHistoryDetail.GetFieldName(ComponentHistoryDetail.Fields.ID, out isNullable),
                        ComponentHistoryDetail.GetFieldName(ComponentHistoryDetail.Fields.ComponentHistoryID, out isNullable),
                        ComponentHistoryDetail.GetFieldName(ComponentHistoryDetail.Fields.DataIndex, out isNullable),
                        ComponentHistoryDetail.GetFieldName(ComponentHistoryDetail.Fields.Datai, out isNullable),
                        ComponentHistoryDetail.GetFieldName(ComponentHistoryDetail.Fields.Dataf, out isNullable),
                        ComponentHistoryDetail.GetFieldName(ComponentHistoryDetail.Fields.Datas, out isNullable),
                        ComponentHistoryDetail.GetFieldName(ComponentHistoryDetail.Fields.Time, out isNullable),
                        ComponentHistoryDetail.TableName,
                        strCondition);

                    if (rollbackData.AddInsertRollback(m_sopDataManager, strSQL, 0, 0, 0, 0, 0, 1, 1) == false)
                        return false;

                    if (m_commonDataManager.GetDeleteManager().DeleteComponentHistoryDetail(strCondition))
                    {
                        rollback.AddData(rollbackData);
                    }
                    else
                        return false;
                }
                else
                {
                    if (m_commonDataManager.GetDeleteManager().DeleteComponentHistoryDetail(strCondition) == false)
                        return false;
                }
            }

            if (strActionStepHistoryIDs != null && strActionStepHistoryIDs != "")
            {
                strCondition = string.Format("{0} in ({1})", ComponentHistory.GetFieldName(ComponentHistory.Fields.ActionStepHistoryID, out isNullable), strActionStepHistoryIDs);

                if (rollback != null)
                {
                    IRollbackData rollbackData = m_sopDataManager.MakeRollbackDataInstance();
                    string query = string.Format("Select {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14} from {15} where ",
                        ComponentHistory.GetFieldName(ComponentHistory.Fields.ID, out isNullable),
                        ComponentHistory.GetFieldName(ComponentHistory.Fields.ActionStepHistoryID, out isNullable),
                        ComponentHistory.GetFieldName(ComponentHistory.Fields.ComponentID, out isNullable),
                        ComponentHistory.GetFieldName(ComponentHistory.Fields.ComponentType, out isNullable),
                        ComponentHistory.GetFieldName(ComponentHistory.Fields.Time, out isNullable),
                        ComponentHistory.GetFieldName(ComponentHistory.Fields.Status, out isNullable),
                        ComponentHistory.GetFieldName(ComponentHistory.Fields.Task, out isNullable),
                        ComponentHistory.GetFieldName(ComponentHistory.Fields.CompleteCount, out isNullable),
                        ComponentHistory.GetFieldName(ComponentHistory.Fields.ShowBoard, out isNullable),
                        ComponentHistory.GetFieldName(ComponentHistory.Fields.AccessedUserID, out isNullable),
                        ComponentHistory.GetFieldName(ComponentHistory.Fields.CheckedNotify1, out isNullable),
                        ComponentHistory.GetFieldName(ComponentHistory.Fields.CheckedNotify2, out isNullable),
                        ComponentHistory.GetFieldName(ComponentHistory.Fields.Description, out isNullable),
                        ComponentHistory.GetFieldName(ComponentHistory.Fields.CheckedRun, out isNullable),
                        ComponentHistory.GetFieldName(ComponentHistory.Fields.CheckedComplete, out isNullable),
                        ComponentHistory.TableName);

                    if (rollbackData.AddInsertRollback(m_sopDataManager, query + strCondition, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0) == false)
                        return false;

                    if (m_commonDataManager.GetDeleteManager().DeleteComponentHistory(strCondition))
                    {
                        rollback.AddData(rollbackData);
                    }
                    else
                        return false;
                }
                else
                {
                    if (m_commonDataManager.GetDeleteManager().DeleteComponentHistory(strCondition) == false)
                        return false;
                }
                
                strCondition = string.Format("{0} in ({1})", ActionStepAutoClose.GetFieldName(ActionStepAutoClose.Fields.ActionStepHistoryID, out isNullable), strActionStepHistoryIDs);
                
                if (rollback != null)
                {
                    IRollbackData rollbackData = m_sopDataManager.MakeRollbackDataInstance();
                    string query = string.Format("Select {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11} from {12} where ",
                        ActionStepAutoClose.GetFieldName(ActionStepAutoClose.Fields.ID, out isNullable),
                        ActionStepAutoClose.GetFieldName(ActionStepAutoClose.Fields.ActionStepHistoryID, out isNullable),
                        ActionStepAutoClose.GetFieldName(ActionStepAutoClose.Fields.ActionStepID, out isNullable),
                        ActionStepAutoClose.GetFieldName(ActionStepAutoClose.Fields.UseCloseNoInput, out isNullable),
                        ActionStepAutoClose.GetFieldName(ActionStepAutoClose.Fields.UseCloseSensorReset, out isNullable),
                        ActionStepAutoClose.GetFieldName(ActionStepAutoClose.Fields.UseCloseSensorResetWaitTime, out isNullable),
                        ActionStepAutoClose.GetFieldName(ActionStepAutoClose.Fields.InputWaitTime, out isNullable),
                        ActionStepAutoClose.GetFieldName(ActionStepAutoClose.Fields.SensorResetWaitTime, out isNullable),
                        ActionStepAutoClose.GetFieldName(ActionStepAutoClose.Fields.BeginTime, out isNullable),
                        ActionStepAutoClose.GetFieldName(ActionStepAutoClose.Fields.SensorZoneID, out isNullable),
                        ActionStepAutoClose.GetFieldName(ActionStepAutoClose.Fields.SensorZoneHistoryID, out isNullable),
                        ActionStepAutoClose.GetFieldName(ActionStepAutoClose.Fields.Description, out isNullable),
                        ActionStepAutoClose.TableName);
                    
                    if (rollbackData.AddInsertRollback(m_sopDataManager, query + strCondition, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1) == false)
                        return false;

                    if (m_commonDataManager.GetDeleteManager().DeleteActionStepAutoClose(strCondition))
                    {
                        rollback.AddData(rollbackData);
                    }
                    else
                        return false;
                }
                else
                {
                    if (m_commonDataManager.GetDeleteManager().DeleteActionStepAutoClose(strCondition) == false)
                        return false;
                }

                strCondition = string.Format("{0} in ({1})", ActionStepHistory.GetFieldName(ActionStepHistory.Fields.ID, out isNullable), strActionStepHistoryIDs);

                if (rollback != null)
                {
                    IRollbackData rollbackData = m_sopDataManager.MakeRollbackDataInstance();
                    string query = string.Format("Select {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13} from {14} where ",
                        ActionStepHistory.GetFieldName(ActionStepHistory.Fields.ID, out isNullable),
                        ActionStepHistory.GetFieldName(ActionStepHistory.Fields.ActionStepID, out isNullable),
                        ActionStepHistory.GetFieldName(ActionStepHistory.Fields.RealMode, out isNullable),
                        ActionStepHistory.GetFieldName(ActionStepHistory.Fields.BeginTime, out isNullable),
                        ActionStepHistory.GetFieldName(ActionStepHistory.Fields.EndTime, out isNullable),
                        ActionStepHistory.GetFieldName(ActionStepHistory.Fields.LastAccessedTime, out isNullable),
                        ActionStepHistory.GetFieldName(ActionStepHistory.Fields.DetectEndTime, out isNullable),
                        ActionStepHistory.GetFieldName(ActionStepHistory.Fields.DetectTime, out isNullable),
                        ActionStepHistory.GetFieldName(ActionStepHistory.Fields.Position, out isNullable),
                        ActionStepHistory.GetFieldName(ActionStepHistory.Fields.LastAccessedUserID, out isNullable),
                        ActionStepHistory.GetFieldName(ActionStepHistory.Fields.Description, out isNullable),
                        ActionStepHistory.GetFieldName(ActionStepHistory.Fields.StartOption, out isNullable),
                        ActionStepHistory.GetFieldName(ActionStepHistory.Fields.DisasterOption, out isNullable),
                        ActionStepHistory.GetFieldName(ActionStepHistory.Fields.SensorZoneHistoryID, out isNullable),
                        ActionStepHistory.TableName);

                    if (rollbackData.AddInsertRollback(m_sopDataManager, query + strCondition, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 1, 0, 1, 0) == false)
                        return false;

                    if (m_commonDataManager.GetDeleteManager().DeleteActionStepHistory(strCondition))
                    {
                        rollback.AddData(rollbackData);
                    }
                    else
                        return false;
                }
                else
                {
                    if (m_commonDataManager.GetDeleteManager().DeleteActionStepHistory(strCondition) == false)
                        return false;
                }
            }
            return true;
        }

        private bool DeleteActionStep(string strDisasterIDs, RollbackManager rollback)
        {
            bool isNullable;
            string strCondition = string.Format("{0} in ({1})", ActionStep.GetFieldName(ActionStep.Fields.DisasterID, out isNullable), strDisasterIDs);

            if (rollback != null)
            {
                IRollbackData rollbackData = m_sopDataManager.MakeRollbackDataInstance();
                string query = string.Format("Select {0}, {1}, {2}, {3} from {4} where ", 
                    ActionStep.GetFieldName(ActionStep.Fields.ID, out isNullable),
                    ActionStep.GetFieldName(ActionStep.Fields.StepName, out isNullable),
                    ActionStep.GetFieldName(ActionStep.Fields.DisasterID, out isNullable),
                    ActionStep.GetFieldName(ActionStep.Fields.UserDefinedConfigID, out isNullable),
                    ActionStep.TableName);

                if (rollbackData.AddInsertRollback(m_sopDataManager, query + strCondition, 0, 1, 0, 0) == false)
                    return false;

                if (m_sopDataManager.GetDeleteManager().DeleteActionStep(strCondition))
                {
                    rollback.AddData(rollbackData);
                    return true;
                }
                else
                    return false;
            }

            return m_sopDataManager.GetDeleteManager().DeleteActionStep(strCondition);
        }

        private bool DeleteSectionGrid(string strStepMemberIDs, RollbackManager rollback)
        {
            bool isNullable;
            string strErrorMessage;
            ISelect selectManager = m_sopDataManager.GetSelectManager();

            string strCondition = string.Format("{0} in ({1})", SectionGrid.GetFieldName(SectionGrid.Fields.StepMemberID, out isNullable), strStepMemberIDs);
            List<SectionGrid> gridList = selectManager.SelectGrids(strCondition, out strErrorMessage);

            if (gridList == null || strErrorMessage != null)
                return false;

            string strGridIDs = "";

            foreach (SectionGrid grid in gridList)
            {
                if (strGridIDs.Length == 0)
                    strGridIDs = grid.ID.ToString();
                else
                    strGridIDs += ", " + grid.ID.ToString();
            }

            if (strGridIDs.Length == 0)
                return true;

            IDelete deleteManager = m_sopDataManager.GetDeleteManager();
            string strCondition2 = string.Format("{0} in ({1})", SectionGridRow.GetFieldName(SectionGridRow.Fields.GridID, out isNullable), strGridIDs);

            if (rollback != null)
            {
                if (deleteManager.DeleteGridRow(strCondition2) == false)
                    return false;

                IRollbackData rollbackData = m_sopDataManager.MakeRollbackDataInstance();
                string query = string.Format("Select {0}, {1}, {2} from {3} where ",
                    SectionGridRow.GetFieldName(SectionGridRow.Fields.GridID, out isNullable),
                    SectionGridRow.GetFieldName(SectionGridRow.Fields.RowIndex, out isNullable),
                    SectionGridRow.GetFieldName(SectionGridRow.Fields.Height, out isNullable),
                    SectionGridRow.TableName);

                if (rollbackData.AddInsertRollback(m_sopDataManager, query + strCondition2, 0, 0, 0) == false)
                    return false;

                rollback.AddData(rollbackData);

                if (deleteManager.DeleteGridColumn(strCondition2) == false)
                    return false;

                rollbackData = m_sopDataManager.MakeRollbackDataInstance();
                query = string.Format("Select {0}, {1}, {2} from {3} where ",
                    SectionGridColumn.GetFieldName(SectionGridColumn.Fields.GridID, out isNullable),
                    SectionGridColumn.GetFieldName(SectionGridColumn.Fields.ColumnIndex, out isNullable),
                    SectionGridColumn.GetFieldName(SectionGridColumn.Fields.Width, out isNullable),
                    SectionGridColumn.TableName);

                if (rollbackData.AddInsertRollback(m_sopDataManager, query + strCondition2, 0, 0, 0) == false)
                    return false;

                rollback.AddData(rollbackData);

                if (deleteManager.DeleteGrid(strCondition) == false)
                    return false;

                rollbackData = m_sopDataManager.MakeRollbackDataInstance();
                query = string.Format("Select {0}, {1} from {2} where ",
                    SectionGrid.GetFieldName(SectionGrid.Fields.ID, out isNullable),
                    SectionGrid.GetFieldName(SectionGrid.Fields.StepMemberID, out isNullable),
                    SectionGrid.TableName);

                if (rollbackData.AddInsertRollback(m_sopDataManager, query + strCondition, 0, 0) == false)
                    return false;

                rollback.AddData(rollbackData);
            }
            else
            {
                if (deleteManager.DeleteGridRow(strCondition2) == false)
                    return false;
                if (deleteManager.DeleteGridColumn(strCondition2) == false)
                    return false;
                if (deleteManager.DeleteGrid(strCondition) == false)
                    return false;
            }

            return true;
        }

        private bool DeleteComponent(string strStepMemberIDs, RollbackManager rollback)
        {
            bool isNullable;
            string strErrorMessage;
            ISelect selectManager = m_sopDataManager.GetSelectManager();

            string strCondition = string.Format("{0} in ({1})", SectionGrid.GetFieldName(SectionGrid.Fields.StepMemberID, out isNullable), strStepMemberIDs);
            List<SectionGrid> grids = selectManager.SelectGrids(strCondition, out strErrorMessage);

            if (grids == null)
                return false;

            string strConditionMission = string.Format("{0} in (Select {1} from {2} where {3})",
                ProcessMission.GetFieldName(ProcessMission.Fields.ProcessID, out isNullable),
                Process.GetFieldName(Process.Fields.ID, out isNullable),
                Process.TableName,
                strCondition);

            if (rollback != null)
            {
                IRollbackData rollbackData = m_sopDataManager.MakeRollbackDataInstance();
                string query = string.Format("Select {0}, {1}, {2} from {3} where ",
                    ProcessMission.GetFieldName(ProcessMission.Fields.ID, out isNullable),
                    ProcessMission.GetFieldName(ProcessMission.Fields.MissionText, out isNullable),
                    ProcessMission.GetFieldName(ProcessMission.Fields.ProcessID, out isNullable),
                    ProcessMission.TableName);

                if (rollbackData.AddInsertRollback(m_sopDataManager, query + strConditionMission, 0, 1, 0) == false)
                    return false;

                if (m_sopDataManager.GetDeleteManager().DeleteProcessMission(strConditionMission))
                {
                    rollback.AddData(rollbackData);
                }
                else
                    return false;
            }
            else
            {
                if (m_sopDataManager.GetDeleteManager().DeleteProcessMission(strConditionMission) == false)
                    return false;
            }

            if (rollback != null)
            {
                IRollbackData rollbackData = m_sopDataManager.MakeRollbackDataInstance();
                string query = string.Format("Select {0}, {1}, {2}, {3}, {4} from {5} where ",
                    ProcessExternalMission.GetFieldName(ProcessExternalMission.Fields.ProcessID, out isNullable),
                    ProcessExternalMission.GetFieldName(ProcessExternalMission.Fields.OrderIndex, out isNullable),
                    ProcessExternalMission.GetFieldName(ProcessExternalMission.Fields.ProgramID, out isNullable),
                    ProcessExternalMission.GetFieldName(ProcessExternalMission.Fields.ParameterIndex, out isNullable),
                    ProcessExternalMission.GetFieldName(ProcessExternalMission.Fields.Value, out isNullable),
                    ProcessExternalMission.TableName);

                if (rollbackData.AddInsertRollback(m_sopDataManager, query + strConditionMission, 0, 0, 0, 0, 1) == false)
                    return false;

                if (m_sopDataManager.GetDeleteManager().DeleteProcessExternalMission(strConditionMission))
                {
                    rollback.AddData(rollbackData);
                }
                else
                    return false;
            }
            else
            {
                if (m_sopDataManager.GetDeleteManager().DeleteProcessExternalMission(strConditionMission) == false)
                    return false;
            }

            if (rollback != null)
            {
                IRollbackData rollbackData = m_sopDataManager.MakeRollbackDataInstance();
                string query = string.Format("Select {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18} from {19} where ",
                    Process.GetFieldName(Process.Fields.ID, out isNullable),
                    Process.GetFieldName(Process.Fields.GridID, out isNullable),
                    Process.GetFieldName(Process.Fields.GridRowIndex, out isNullable),
                    Process.GetFieldName(Process.Fields.GridColumnIndex, out isNullable),
                    Process.GetFieldName(Process.Fields.Width, out isNullable),
                    Process.GetFieldName(Process.Fields.Height, out isNullable),
                    Process.GetFieldName(Process.Fields.Text, out isNullable),
                    Process.GetFieldName(Process.Fields.TeamList, out isNullable),
                    Process.GetFieldName(Process.Fields.ComponentID, out isNullable),
                    Process.GetFieldName(Process.Fields.OnlyTeamLeader, out isNullable),
                    Process.GetFieldName(Process.Fields.StepMemberID, out isNullable),
                    Process.GetFieldName(Process.Fields.VAlign, out isNullable),
                    Process.GetFieldName(Process.Fields.HAlign, out isNullable),
                    Process.GetFieldName(Process.Fields.FontName, out isNullable),
                    Process.GetFieldName(Process.Fields.FontStyle, out isNullable),
                    Process.GetFieldName(Process.Fields.FontSize, out isNullable),
                    Process.GetFieldName(Process.Fields.LineSpace, out isNullable),
                    Process.GetFieldName(Process.Fields.FontColor, out isNullable),
                    Process.GetFieldName(Process.Fields.AutoRun, out isNullable),
                    Process.TableName);

                if (rollbackData.AddInsertRollback(m_sopDataManager, query + strCondition, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0) == false)
                    return false;

                if (m_sopDataManager.GetDeleteManager().DeleteProcess(strCondition))
                {
                    rollback.AddData(rollbackData);
                }
                else
                    return false;
            }
            else
            {
                if (m_sopDataManager.GetDeleteManager().DeleteProcess(strCondition) == false)
                    return false;
            }

            if (rollback != null)
            {
                IRollbackData rollbackData = m_sopDataManager.MakeRollbackDataInstance();
                string query = string.Format("Select {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15} from {16} where ",
                    Annotation.GetFieldName(Annotation.Fields.ID, out isNullable),
                    Annotation.GetFieldName(Annotation.Fields.GridID, out isNullable),
                    Annotation.GetFieldName(Annotation.Fields.GridRowIndex, out isNullable),
                    Annotation.GetFieldName(Annotation.Fields.GridColumnIndex, out isNullable),
                    Annotation.GetFieldName(Annotation.Fields.Width, out isNullable),
                    Annotation.GetFieldName(Annotation.Fields.Height, out isNullable),
                    Annotation.GetFieldName(Annotation.Fields.Text, out isNullable),
                    Annotation.GetFieldName(Annotation.Fields.ComponentID, out isNullable),
                    Annotation.GetFieldName(Annotation.Fields.StepMemberID, out isNullable),
                    Annotation.GetFieldName(Annotation.Fields.VAlign, out isNullable),
                    Annotation.GetFieldName(Annotation.Fields.HAlign, out isNullable),
                    Annotation.GetFieldName(Annotation.Fields.FontName, out isNullable),
                    Annotation.GetFieldName(Annotation.Fields.FontStyle, out isNullable),
                    Annotation.GetFieldName(Annotation.Fields.FontSize, out isNullable),
                    Annotation.GetFieldName(Annotation.Fields.LineSpace, out isNullable),
                    Annotation.GetFieldName(Annotation.Fields.FontColor, out isNullable),
                    Annotation.TableName);

                if (rollbackData.AddInsertRollback(m_sopDataManager, query + strCondition, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0) == false)
                    return false;

                if (m_sopDataManager.GetDeleteManager().DeleteAnnotation(strCondition))
                {
                    rollback.AddData(rollbackData);
                }
                else
                    return false;
            }
            else
            {
                if (m_sopDataManager.GetDeleteManager().DeleteAnnotation(strCondition) == false)
                    return false;
            }
            
            if (rollback != null)
            {
                IRollbackData rollbackData = m_sopDataManager.MakeRollbackDataInstance();
                string query = string.Format("Select {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18}, {19}, {20}, {21}, {22} from {23} where ",
                    InternalTransmission.GetFieldName(InternalTransmission.Fields.ID, out isNullable),
                    InternalTransmission.GetFieldName(InternalTransmission.Fields.GridID, out isNullable),
                    InternalTransmission.GetFieldName(InternalTransmission.Fields.GridRowIndex, out isNullable),
                    InternalTransmission.GetFieldName(InternalTransmission.Fields.GridColumnIndex, out isNullable),
                    InternalTransmission.GetFieldName(InternalTransmission.Fields.Width, out isNullable),
                    InternalTransmission.GetFieldName(InternalTransmission.Fields.Height, out isNullable),
                    InternalTransmission.GetFieldName(InternalTransmission.Fields.Text, out isNullable),
                    InternalTransmission.GetFieldName(InternalTransmission.Fields.ComponentID, out isNullable),
                    InternalTransmission.GetFieldName(InternalTransmission.Fields.UseSMS, out isNullable),
                    InternalTransmission.GetFieldName(InternalTransmission.Fields.UseBroadcast, out isNullable),
                    InternalTransmission.GetFieldName(InternalTransmission.Fields.StepMemberID, out isNullable),
                    InternalTransmission.GetFieldName(InternalTransmission.Fields.Message, out isNullable),
                    InternalTransmission.GetFieldName(InternalTransmission.Fields.TeamList, out isNullable),
                    InternalTransmission.GetFieldName(InternalTransmission.Fields.OnlyTeamLeader, out isNullable),
                    InternalTransmission.GetFieldName(InternalTransmission.Fields.VAlign, out isNullable),
                    InternalTransmission.GetFieldName(InternalTransmission.Fields.HAlign, out isNullable),
                    InternalTransmission.GetFieldName(InternalTransmission.Fields.FontName, out isNullable),
                    InternalTransmission.GetFieldName(InternalTransmission.Fields.FontStyle, out isNullable),
                    InternalTransmission.GetFieldName(InternalTransmission.Fields.FontSize, out isNullable),
                    InternalTransmission.GetFieldName(InternalTransmission.Fields.LineSpace, out isNullable),
                    InternalTransmission.GetFieldName(InternalTransmission.Fields.FontColor, out isNullable),
                    InternalTransmission.GetFieldName(InternalTransmission.Fields.AutoRun, out isNullable),
                    InternalTransmission.GetFieldName(InternalTransmission.Fields.UseSiren, out isNullable),
                    InternalTransmission.TableName);

                if (rollbackData.AddInsertRollback(m_sopDataManager, query + strCondition, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0) == false)
                    return false;

                if (m_sopDataManager.GetDeleteManager().DeleteInternalTransmission(strCondition))
                {
                    rollback.AddData(rollbackData);
                }
                else
                    return false;
            }
            else
            {
                if (m_sopDataManager.GetDeleteManager().DeleteInternalTransmission(strCondition) == false)
                    return false;
            }
            
            if (rollback != null)
            {
                IRollbackData rollbackData = m_sopDataManager.MakeRollbackDataInstance();
                string query = string.Format("Select {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18}, {19} from {20} where ",
                    Decision.GetFieldName(Decision.Fields.ID, out isNullable),
                    Decision.GetFieldName(Decision.Fields.GridID, out isNullable),
                    Decision.GetFieldName(Decision.Fields.GridRowIndex, out isNullable),
                    Decision.GetFieldName(Decision.Fields.GridColumnIndex, out isNullable),
                    Decision.GetFieldName(Decision.Fields.Width, out isNullable),
                    Decision.GetFieldName(Decision.Fields.Height, out isNullable),
                    Decision.GetFieldName(Decision.Fields.Text, out isNullable),
                    Decision.GetFieldName(Decision.Fields.TeamID, out isNullable),
                    Decision.GetFieldName(Decision.Fields.TeamType, out isNullable),
                    Decision.GetFieldName(Decision.Fields.ComponentID, out isNullable),
                    Decision.GetFieldName(Decision.Fields.StepMemberID, out isNullable),
                    Decision.GetFieldName(Decision.Fields.VAlign, out isNullable),
                    Decision.GetFieldName(Decision.Fields.HAlign, out isNullable),
                    Decision.GetFieldName(Decision.Fields.FontName, out isNullable),
                    Decision.GetFieldName(Decision.Fields.FontStyle, out isNullable),
                    Decision.GetFieldName(Decision.Fields.FontSize, out isNullable),
                    Decision.GetFieldName(Decision.Fields.LineSpace, out isNullable),
                    Decision.GetFieldName(Decision.Fields.FontColor, out isNullable),
                    Decision.GetFieldName(Decision.Fields.AutoRunScript, out isNullable),
                    Decision.GetFieldName(Decision.Fields.AutoRunScriptVariableTypes, out isNullable),
                    Decision.TableName);

                if (rollbackData.AddInsertRollback(m_sopDataManager, query + strCondition, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 1, 1) == false)
                    return false;

                if (m_sopDataManager.GetDeleteManager().DeleteDecision(strCondition))
                {
                    rollback.AddData(rollbackData);
                }
                else
                    return false;
            }
            else
            {
                if (m_sopDataManager.GetDeleteManager().DeleteDecision(strCondition) == false)
                    return false;
            }

            if (rollback != null)
            {
                IRollbackData rollbackData = m_sopDataManager.MakeRollbackDataInstance();
                string query = string.Format("Select {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16} from {17} where ",
                    EndPoint.GetFieldName(EndPoint.Fields.ID, out isNullable),
                    EndPoint.GetFieldName(EndPoint.Fields.GridID, out isNullable),
                    EndPoint.GetFieldName(EndPoint.Fields.GridRowIndex, out isNullable),
                    EndPoint.GetFieldName(EndPoint.Fields.GridColumnIndex, out isNullable),
                    EndPoint.GetFieldName(EndPoint.Fields.Width, out isNullable),
                    EndPoint.GetFieldName(EndPoint.Fields.Height, out isNullable),
                    EndPoint.GetFieldName(EndPoint.Fields.Text, out isNullable),
                    EndPoint.GetFieldName(EndPoint.Fields.ComponentID, out isNullable),
                    EndPoint.GetFieldName(EndPoint.Fields.IsBegin, out isNullable),
                    EndPoint.GetFieldName(EndPoint.Fields.StepMemberID, out isNullable),
                    EndPoint.GetFieldName(EndPoint.Fields.VAlign, out isNullable),
                    EndPoint.GetFieldName(EndPoint.Fields.HAlign, out isNullable),
                    EndPoint.GetFieldName(EndPoint.Fields.FontName, out isNullable),
                    EndPoint.GetFieldName(EndPoint.Fields.FontStyle, out isNullable),
                    EndPoint.GetFieldName(EndPoint.Fields.FontSize, out isNullable),
                    EndPoint.GetFieldName(EndPoint.Fields.LineSpace, out isNullable),
                    EndPoint.GetFieldName(EndPoint.Fields.FontColor, out isNullable),
                    EndPoint.TableName);

                if (rollbackData.AddInsertRollback(m_sopDataManager, query + strCondition, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0) == false)
                    return false;

                if (m_sopDataManager.GetDeleteManager().DeleteEndPoint(strCondition))
                {
                    rollback.AddData(rollbackData);
                }
                else
                    return false;
            }
            else
            {
                if (m_sopDataManager.GetDeleteManager().DeleteEndPoint(strCondition) == false)
                    return false;
            }
            
            if (rollback != null)
            {
                IRollbackData rollbackData = m_sopDataManager.MakeRollbackDataInstance();
                string query = string.Format("Select {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16} from {17} where ",
                    Link.GetFieldName(Link.Fields.ID, out isNullable),
                    Link.GetFieldName(Link.Fields.GridID, out isNullable),
                    Link.GetFieldName(Link.Fields.GridRowIndex, out isNullable),
                    Link.GetFieldName(Link.Fields.GridColumnIndex, out isNullable),
                    Link.GetFieldName(Link.Fields.Width, out isNullable),
                    Link.GetFieldName(Link.Fields.Height, out isNullable),
                    Link.GetFieldName(Link.Fields.Text, out isNullable),
                    Link.GetFieldName(Link.Fields.ComponentID, out isNullable),
                    Link.GetFieldName(Link.Fields.LinkedComponentIDList, out isNullable),
                    Link.GetFieldName(Link.Fields.StepMemberID, out isNullable),
                    Link.GetFieldName(Link.Fields.VAlign, out isNullable),
                    Link.GetFieldName(Link.Fields.HAlign, out isNullable),
                    Link.GetFieldName(Link.Fields.FontName, out isNullable),
                    Link.GetFieldName(Link.Fields.FontStyle, out isNullable),
                    Link.GetFieldName(Link.Fields.FontSize, out isNullable),
                    Link.GetFieldName(Link.Fields.LineSpace, out isNullable),
                    Link.GetFieldName(Link.Fields.FontColor, out isNullable),
                    Link.TableName);

                if (rollbackData.AddInsertRollback(m_sopDataManager, query + strCondition, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0) == false)
                    return false;

                if (m_sopDataManager.GetDeleteManager().DeleteLink(strCondition))
                {
                    rollback.AddData(rollbackData);
                }
                else
                    return false;
            }
            else
            {
                if (m_sopDataManager.GetDeleteManager().DeleteLink(strCondition) == false)
                    return false;
            }

            if (rollback != null)
            {
                IRollbackData rollbackData = m_sopDataManager.MakeRollbackDataInstance();
                string query = string.Format("Select {0}, {1}, {2}, {3}, {4}, {5}, {6} from {7} where ",
                    Arrow.GetFieldName(Arrow.Fields.ID, out isNullable),
                    Arrow.GetFieldName(Arrow.Fields.Text, out isNullable),
                    Arrow.GetFieldName(Arrow.Fields.BeginComponentID, out isNullable),
                    Arrow.GetFieldName(Arrow.Fields.BeginComponentPosition, out isNullable),
                    Arrow.GetFieldName(Arrow.Fields.EndComponentID, out isNullable),
                    Arrow.GetFieldName(Arrow.Fields.EndComponentPosition, out isNullable),
                    Arrow.GetFieldName(Arrow.Fields.StepMemberID, out isNullable),
                    Arrow.TableName);

                if (rollbackData.AddInsertRollback(m_sopDataManager, query + strCondition, 0, 1, 0, 0, 0, 0, 0) == false)
                    return false;

                if (m_sopDataManager.GetDeleteManager().DeleteArrow(strCondition))
                {
                    rollback.AddData(rollbackData);
                }
                else
                    return false;
            }
            else
            {
                if (m_sopDataManager.GetDeleteManager().DeleteArrow(strCondition) == false)
                    return false;
            }

            string strGridIDs = "";

            foreach (SectionGrid grid in grids)
            {
                if (strGridIDs.Length == 0)
                    strGridIDs = grid.ID.ToString();
                else
                    strGridIDs += ", " + grid.ID.ToString();
            }

            if (strGridIDs.Length > 0)
            {
                string strCondition2 = string.Format("{0} in ({1})", SectionGridRow.GetFieldName(SectionGridRow.Fields.GridID, out isNullable), strGridIDs);
                string strCondition3 = string.Format("{0} in ({1})", SectionGrid.GetFieldName(SectionGrid.Fields.ID, out isNullable), strGridIDs);

                if (rollback != null)
                {
                    IRollbackData rollbackData = m_sopDataManager.MakeRollbackDataInstance();
                    string query = string.Format("Select {0}, {1}, {2} from {3} where ",
                        SectionGridRow.GetFieldName(SectionGridRow.Fields.GridID, out isNullable),
                        SectionGridRow.GetFieldName(SectionGridRow.Fields.RowIndex, out isNullable),
                        SectionGridRow.GetFieldName(SectionGridRow.Fields.Height, out isNullable),
                        SectionGridRow.TableName);

                    if (rollbackData.AddInsertRollback(m_sopDataManager, query + strCondition2, 0, 0, 0) == false)
                        return false;

                    if (m_sopDataManager.GetDeleteManager().DeleteGridRow(strCondition2))
                    {
                        rollback.AddData(rollbackData);
                    }
                    else
                        return false;

                    rollbackData = m_sopDataManager.MakeRollbackDataInstance();
                    query = string.Format("Select {0}, {1}, {2} from {3} where ",
                        SectionGridColumn.GetFieldName(SectionGridColumn.Fields.GridID, out isNullable),
                        SectionGridColumn.GetFieldName(SectionGridColumn.Fields.ColumnIndex, out isNullable),
                        SectionGridColumn.GetFieldName(SectionGridColumn.Fields.Width, out isNullable),
                        SectionGridColumn.TableName);

                    if (rollbackData.AddInsertRollback(m_sopDataManager, query + strCondition2, 0, 0, 0) == false)
                        return false;

                    if (m_sopDataManager.GetDeleteManager().DeleteGridColumn(strCondition2))
                    {
                        rollback.AddData(rollbackData);
                    }
                    else
                        return false;

                    rollbackData = m_sopDataManager.MakeRollbackDataInstance();
                    query = string.Format("Select {0}, {1} from {2} where ",
                        SectionGrid.GetFieldName(SectionGrid.Fields.ID, out isNullable),
                        SectionGrid.GetFieldName(SectionGrid.Fields.StepMemberID, out isNullable),
                        SectionGrid.TableName);

                    if (rollbackData.AddInsertRollback(m_sopDataManager, query + strCondition3, 0, 0) == false)
                        return false;

                    if (m_sopDataManager.GetDeleteManager().DeleteGridColumn(strCondition2))
                    {
                        rollback.AddData(rollbackData);
                    }
                    else
                        return false;
                }
                else
                {
                    if (m_sopDataManager.GetDeleteManager().DeleteGridRow(strCondition2) == false)
                        return false;
                    if (m_sopDataManager.GetDeleteManager().DeleteGridColumn(strCondition2) == false)
                        return false;
                }
            }

            return true;
        }

        private bool DeleteStepMember(string strActionStepIDs, RollbackManager rollback)
        {
            bool isNullable;
            string strCondition = string.Format("{0} in ({1})", StepMember.GetFieldName(StepMember.Fields.ActionStepID, out isNullable), strActionStepIDs);
            
            if (rollback != null)
            {
                IRollbackData rollbackData = m_sopDataManager.MakeRollbackDataInstance();

                string strSQL = string.Format("Select {0}, {1}, {2}, {3} from {4} where {5}",
                    StepMember.GetFieldName(StepMember.Fields.ID, out isNullable),
                    StepMember.GetFieldName(StepMember.Fields.TeamID, out isNullable),
                    StepMember.GetFieldName(StepMember.Fields.TeamType, out isNullable),
                    StepMember.GetFieldName(StepMember.Fields.ActionStepID, out isNullable),
                    StepMember.TableName,
                    strCondition);

                if (rollbackData.AddInsertRollback(m_sopDataManager, strSQL, 0, 0, 0, 0) == false)
                    return false;

                if (m_sopDataManager.GetDeleteManager().DeleteStepMember(strCondition))
                {
                    rollback.AddData(rollbackData);
                }
                else
                    return false;
            }

            return m_sopDataManager.GetDeleteManager().DeleteStepMember(strCondition);
        }
    }
}
