namespace Common.DAL
{
    using Model;
    using Model.Option;
    using Model.History;
    using IDAL;
    using dnsDBUtil;
    using System.Collections;
    using System.Reflection;
    using System.Collections.Generic;

    public class UpdateManager : QueryManager, IUpdate
    {
        private string m_strErrorMessage = null;
        private DataManager m_dataManager = null;
        //private WebDBManager m_dbManager = null;

        public UpdateManager(DataManager dataManager)
        {
            m_dataManager = dataManager;
            m_dbManager = m_dataManager.GetDBManager() as WebDBManager;
        }

        // Option
        // strCondition : where를 제외한 조건문
        public bool UpdateOption(Options.OptionTarget eTargetName, Options option, string strCondition = null)
        {
            if (option != null)
            {
                string tableName = string.Format("Option{0}", eTargetName.ToString());
                string query = "";
                ArrayList res = null;

                if (eTargetName != Options.OptionTarget.NOT_DEFINED)
                {
                    var info = m_dbManager.GetColumnInfoDictionary(tableName);
                    var fields = option.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    string valueString = m_dbManager.ConvertUpdateParamsToString(info, option, fields);

                    if (valueString != null)
                    {
                        query = string.Format("update {0} set {1}", tableName, valueString);

                        if (strCondition != null)
                        {
                            query = query.Insert(query.Length, string.Format(" where {0}", strCondition));
                        }
                        else
                        {
                            query = query.Insert(query.Length, string.Format(" where ID = {0}", option.ID));
                        }

                        res = m_dbManager.GetResultData(query);

                        if (res != null)
                        {
                            return true;
                        }
                        else
                        {
                            m_strErrorMessage = m_dbManager.LastErrorMessage;
                        }
                    }
                    else
                    {
                        // Not Defined
                        m_strErrorMessage = "Converting Update Query Error";
                    }
                }
                else
                {
                    // Not Defined
                    m_strErrorMessage = "TargetName Not Defined";
                }
            }
            else
            {
                // Not Defined
                m_strErrorMessage = "Error";
            }

            return false;
        }

        public bool UpdateOption(Options.OptionTarget target, Dictionary<Options.Fields, object> dicSets, Dictionary<Options.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";
            string strTableName = Options.GetTableName(target);

            if (SetData<Options.Fields>(ref strSets, dicSets, Options.GetFieldName, strTableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<Options.Fields>(ref strCondition, dicConditions, Options.GetFieldName, strTableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", strTableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        // History
        // strCondition : where를 제외한 조건문
        public bool UpdateActionStepHistory(ActionStepHistory actionStepHistory, out string strErrorMessage)
        {
            Dictionary<ActionStepHistory.Fields, object> dicSets = new Dictionary<ActionStepHistory.Fields, object>();
            dicSets[ActionStepHistory.Fields.ActionStepID] = actionStepHistory.ActionStepID;
            dicSets[ActionStepHistory.Fields.BeginTime] = actionStepHistory.BeginTime;
            dicSets[ActionStepHistory.Fields.Description] = actionStepHistory.Description;
            dicSets[ActionStepHistory.Fields.DetectEndTime] = actionStepHistory.DetectEndTime;
            dicSets[ActionStepHistory.Fields.DetectTime] = actionStepHistory.DetectTime;
            dicSets[ActionStepHistory.Fields.DisasterOption] = actionStepHistory.DisasterOption;
            dicSets[ActionStepHistory.Fields.EndTime] = actionStepHistory.EndTime;
            dicSets[ActionStepHistory.Fields.LastAccessedTime] = actionStepHistory.LastAccessedTime;
            dicSets[ActionStepHistory.Fields.LastAccessedUserID] = actionStepHistory.LastAccessedUserID;
            dicSets[ActionStepHistory.Fields.Position] = actionStepHistory.Position;
            dicSets[ActionStepHistory.Fields.RealMode] = actionStepHistory.RealMode;
            dicSets[ActionStepHistory.Fields.SensorZoneHistoryID] = actionStepHistory.SensorZoneHistoryID;
            dicSets[ActionStepHistory.Fields.StartOption] = actionStepHistory.StartOption;

            Dictionary<ActionStepHistory.Fields, object> dicConditions = new Dictionary<ActionStepHistory.Fields, object>();
            dicConditions[ActionStepHistory.Fields.ID] = actionStepHistory.ID;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<ActionStepHistory.Fields>(ref strSets, dicSets, ActionStepHistory.GetFieldName, ActionStepHistory.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<ActionStepHistory.Fields>(ref strCondition, dicConditions, ActionStepHistory.GetFieldName, ActionStepHistory.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", ActionStepHistory.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        // strCondition : where를 제외한 조건문
        public bool UpdateComponentHistory(ComponentHistory componentStepHistory, out string strErrorMessage)
        {
            Dictionary<ComponentHistory.Fields, object> dicSets = new Dictionary<ComponentHistory.Fields, object>();
            dicSets[ComponentHistory.Fields.AccessedUserID] = componentStepHistory.AccessedUserID;
            dicSets[ComponentHistory.Fields.ActionStepHistoryID] = componentStepHistory.ActionStepHistoryID;
            dicSets[ComponentHistory.Fields.CheckedComplete] = componentStepHistory.CheckedComplete;
            dicSets[ComponentHistory.Fields.CheckedNotify1] = componentStepHistory.CheckedNotify1;
            dicSets[ComponentHistory.Fields.CheckedNotify2] = componentStepHistory.CheckedNotify2;
            dicSets[ComponentHistory.Fields.CheckedRun] = componentStepHistory.CheckedRun;
            dicSets[ComponentHistory.Fields.CompleteCount] = componentStepHistory.CompleteCount;
            dicSets[ComponentHistory.Fields.ComponentID] = componentStepHistory.ComponentID;
            dicSets[ComponentHistory.Fields.ComponentType] = componentStepHistory.ComponentType;
            dicSets[ComponentHistory.Fields.Description] = componentStepHistory.Description;
            dicSets[ComponentHistory.Fields.ShowBoard] = componentStepHistory.ShowBoard;
            dicSets[ComponentHistory.Fields.Status] = componentStepHistory.Status;
            dicSets[ComponentHistory.Fields.Task] = componentStepHistory.Task;
            dicSets[ComponentHistory.Fields.Time] = componentStepHistory.Time;

            Dictionary<ComponentHistory.Fields, object> dicConditions = new Dictionary<ComponentHistory.Fields, object>();
            dicConditions[ComponentHistory.Fields.ID] = componentStepHistory.ID;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<ComponentHistory.Fields>(ref strSets, dicSets, ComponentHistory.GetFieldName, ComponentHistory.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<ComponentHistory.Fields>(ref strCondition, dicConditions, ComponentHistory.GetFieldName, ComponentHistory.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", ComponentHistory.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        // strCondition : where를 제외한 조건문
        public bool UpdateComponentHistoryDetail(ComponentHistoryDetail componentHistoryDetail, out string strErrorMessage)
        {
            Dictionary<ComponentHistoryDetail.Fields, object> dicSets = new Dictionary<ComponentHistoryDetail.Fields, object>();
            dicSets[ComponentHistoryDetail.Fields.ComponentHistoryID] = componentHistoryDetail.ComponentHistoryID;
            dicSets[ComponentHistoryDetail.Fields.DataIndex] = componentHistoryDetail.DataIndex;
            dicSets[ComponentHistoryDetail.Fields.Datai] = componentHistoryDetail.Datai;
            dicSets[ComponentHistoryDetail.Fields.Dataf] = componentHistoryDetail.Dataf;
            dicSets[ComponentHistoryDetail.Fields.Datas] = componentHistoryDetail.Datas;
            dicSets[ComponentHistoryDetail.Fields.Datas] = componentHistoryDetail.Datas;

            Dictionary<ComponentHistoryDetail.Fields, object> dicConditions = new Dictionary<ComponentHistoryDetail.Fields, object>();
            dicConditions[ComponentHistoryDetail.Fields.ID] = componentHistoryDetail.ID;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<ComponentHistoryDetail.Fields>(ref strSets, dicSets, ComponentHistoryDetail.GetFieldName, ComponentHistoryDetail.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<ComponentHistoryDetail.Fields>(ref strCondition, dicConditions, ComponentHistoryDetail.GetFieldName, ComponentHistory.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", ComponentHistory.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        // strCondition : where를 제외한 조건문
        public bool UpdateActionStepAutoClose(ActionStepAutoClose actionStepAutoClose, string strCondition = null)
        {
            if (actionStepAutoClose != null)
            {
                string tableName = ActionStepAutoClose.TableName;
                string query = "";
                ArrayList res = null;

                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = actionStepAutoClose.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                string valueString = m_dbManager.ConvertUpdateParamsToString(info, actionStepAutoClose, fields);

                if (valueString != null)
                {
                    query = string.Format("update {0} set {1}", tableName, valueString);

                    if (strCondition != null)
                    {
                        query = query.Insert(query.Length, string.Format(" where {0}", strCondition));
                    }

                    res = m_dbManager.GetResultData(query);

                    if (res != null)
                    {
                        return true;
                    }
                    else
                    {
                        m_strErrorMessage = m_dbManager.LastErrorMessage;
                    }
                }
                else
                {
                    // Not Defined
                    m_strErrorMessage = "Converting Update Query Error";
                }
            }
            else
            {
                // Not Defined
                m_strErrorMessage = "Error";
            }

            return false;
        }

        public bool UpdateShelter(Shelter shelter, string strCondition = null)
        {
            if (shelter != null)
            {
                string tableName = Shelter.TableName;
                string query = "";
                ArrayList res = null;

                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = shelter.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                string valueString = m_dbManager.ConvertUpdateParamsToString(info, shelter, fields);

                if (valueString != null)
                {
                    ChangeShelterBoundaryValueString(shelter, ref valueString);
                    query = string.Format("update {0} set {1}", tableName, valueString);

                    bool isNullable;

                    if (strCondition == null || strCondition.Length == 0)
                        strCondition = string.Format("{0} = {1}", Shelter.GetFieldName(Shelter.Fields.ID, out isNullable), shelter.ID);

                    query = query.Insert(query.Length, string.Format(" where {0}", strCondition));

                    res = m_dbManager.GetResultData(query);

                    if (res != null)
                    {
                        return true;
                    }
                    else
                    {
                        m_strErrorMessage = m_dbManager.LastErrorMessage;
                    }
                }
                else
                {
                    // Not Defined
                    m_strErrorMessage = "Converting Update Query Error";
                }
            }
            else
            {
                // Not Defined
                m_strErrorMessage = "Error";
            }

            return false;
        }

        private void ChangeShelterBoundaryValueString(Shelter shelter, ref string valueString)
        {
            bool isNullable;
            string strFieldName = Shelter.GetFieldName(Shelter.Fields.Boundary, out isNullable).ToLower();

            string strValue = valueString.ToLower();

            if (ChangeShelterBoundaryValueString(shelter, ref valueString, strValue, strFieldName + " =") == false)
            {
                ChangeShelterBoundaryValueString(shelter, ref valueString, strValue, strFieldName + "=");
            }
        }

        private bool ChangeShelterBoundaryValueString(Shelter shelter, ref string valueString, string strLowerValue, string strField)
        {
            int index = strLowerValue.IndexOf(strField);

            if (index < 0)
                return false;

            string strBoundary = shelter.Boundary == null ? strField + "NULL" : strField + "'" + ShelterBoundaryToString(shelter.Boundary) + "'";

            int comma = strLowerValue.IndexOf(',', index + 1);

            if (comma > 0)
            {
                valueString = valueString.Substring(0, index) + strBoundary + valueString.Substring(comma);
            }
            else
            {
                valueString = valueString.Substring(0, index) + strBoundary;
            }

            return true;
        }

        public bool UpdateSite(Site site, string strCondition = null)
        {
            if (site != null)
            {
                string tableName = Site.TableName;
                string query = "";
                ArrayList res = null;

                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = site.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                string valueString = m_dbManager.ConvertUpdateParamsToString(info, site, fields);

                if (valueString != null)
                {
                    query = string.Format("update {0} set {1}", tableName, valueString);

                    if (strCondition != null)
                    {
                        query = query.Insert(query.Length, string.Format(" where {0}", strCondition));
                    }

                    res = m_dbManager.GetResultData(query);

                    if (res != null)
                    {
                        return true;
                    }
                    else
                    {
                        m_strErrorMessage = m_dbManager.LastErrorMessage;
                    }
                }
                else
                {
                    // Not Defined
                    m_strErrorMessage = "Converting Update Query Error";
                }
            }
            else
            {
                // Not Defined
                m_strErrorMessage = "Error";
            }

            return false;
        }

        public string GetErrorMessage()
        {
            return m_strErrorMessage;
        }
    }
}
