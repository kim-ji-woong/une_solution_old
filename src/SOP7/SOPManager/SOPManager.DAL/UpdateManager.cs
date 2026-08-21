namespace SOPManager.DAL
{
    using Model.Sop.Account;
    using Model.Sop.Category;
    using Model.Sop.Component;
    using Model.Sop.Config;
    using IDAL;
    using dnsDBUtil;
    using System.Collections;
    using System.Reflection;
    using System.Linq;
    using System;

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

        public bool UpdateLevel(Level level, string strCondition = null)
        {
            if (level != null)
            {
                string tableName = Level.TableName;
                string query = "";
                ArrayList res = null;

                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = level.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                string valueString = m_dbManager.ConvertUpdateParamsToString(info, level, fields);

                if (valueString != null)
                {
                    query = string.Format("update {0} set {1}", tableName, valueString);

                    bool isNullable;

                    if (strCondition == null || strCondition.Length == 0)
                        strCondition = string.Format("{0} = {1}", Level.GetFieldName(Level.Fields.ID, out isNullable), level.ID);

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

        public bool UpdateUser(User user, string strCondition = null)
        {
            if (user != null)
            {
                string tableName = User.TableName;
                string query = "";
                ArrayList res = null;

                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = user.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                string valueString = m_dbManager.ConvertUpdateParamsToString(info, user, fields);

                if (valueString != null)
                {
                    query = string.Format("update {0} set {1}", tableName, valueString);

                    bool isNullable;

                    if (strCondition == null || strCondition.Length == 0)
                        strCondition = string.Format("{0} = {1}", User.GetFieldName(User.Fields.ID, out isNullable), user.ID);

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

        public bool UpdateOption(Option option, string strCondition = null)
        {
            if (option != null)
            {
                string tableName = Option.TableName;
                string query = "";
                ArrayList res = null;

                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = option.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                string valueString = m_dbManager.ConvertUpdateParamsToString(info, option, fields);

                if (valueString != null)
                {
                    query = string.Format("update {0} set {1}", tableName, valueString);

                    bool isNullable;

                    if (strCondition == null || strCondition.Length == 0)
                        strCondition = string.Format("{0} = {1} and {2} = '{3}' and {4} = '{5}'",
                            Option.GetFieldName(Option.Fields.UserID, out isNullable), option.UserID,
                            Option.GetFieldName(Option.Fields.Category, out isNullable), option.Category,
                            Option.GetFieldName(Option.Fields.SubCategory, out isNullable), option.SubCategory);

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

        public bool UpdateActionStep(ActionStep actionStep, string strCondition = null)
        {
            if (actionStep != null)
            {
                string tableName = ActionStep.TableName;
                string query = "";
                ArrayList res = null;

                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = actionStep.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                string valueString = m_dbManager.ConvertUpdateParamsToString(info, actionStep, fields);

                if (valueString != null)
                {
                    query = string.Format("update {0} set {1}", tableName, valueString);

                    bool isNullable;

                    if (strCondition == null || strCondition.Length == 0)
                        strCondition = string.Format("{0} = {1}", ActionStep.GetFieldName(ActionStep.Fields.ID, out isNullable), actionStep.ID);

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

        public bool UpdateAnnotation(Annotation annotation, string strCondition = null)
        {
            if (annotation != null)
            {
                string tableName = Annotation.TableName;
                string query = "";
                ArrayList res = null;

                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = annotation.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                string valueString = m_dbManager.ConvertUpdateParamsToString(info, annotation, fields);

                if (valueString != null)
                {
                    query = string.Format("update {0} set {1}", tableName, valueString);

                    bool isNullable;

                    if (strCondition == null || strCondition.Length == 0)
                        strCondition = string.Format("{0} = {1}", Annotation.GetFieldName(Annotation.Fields.ID, out isNullable), annotation.ID);

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

        public bool UpdateArrow(Arrow arrow, string strCondition = null)
        {
            if (arrow != null)
            {
                string tableName = Arrow.TableName;
                string query = "";
                ArrayList res = null;

                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = arrow.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                string valueString = m_dbManager.ConvertUpdateParamsToString(info, arrow, fields);

                if (valueString != null)
                {
                    query = string.Format("update {0} set {1}", tableName, valueString);

                    bool isNullable;

                    if (strCondition == null || strCondition.Length == 0)
                        strCondition = string.Format("{0} = {1}", Arrow.GetFieldName(Arrow.Fields.ID, out isNullable), arrow.ID);

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

        public bool UpdateDecision(Decision decision, string strCondition = null)
        {
            if (decision != null)
            {
                string tableName = Decision.TableName;
                string query = "";
                ArrayList res = null;

                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = decision.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                string valueString = m_dbManager.ConvertUpdateParamsToString(info, decision, fields);

                if (valueString != null)
                {
                    query = string.Format("update {0} set {1}", tableName, valueString);

                    bool isNullable;

                    if (strCondition == null || strCondition.Length == 0)
                        strCondition = string.Format("{0} = {1}", Decision.GetFieldName(Decision.Fields.ID, out isNullable), decision.ID);

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

        public bool UpdateDisaster(Disaster disaster, string strCondition = null)
        {
            if (disaster != null)
            {
                string tableName = Disaster.TableName;
                string query = "";
                ArrayList res = null;

                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = disaster.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                string valueString = m_dbManager.ConvertUpdateParamsToString(info, disaster, fields);

                if (valueString != null)
                {
                    string[] separator = { ", " };
                    string[] valueTemp = valueString.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    int findIdx = 0;

                    // 이전 필드 값이 ", "가 들어가는 경우 fields의 Index와 valueTemp의 Index가 맞지 않으므로 찾는 field 이름이 포함된 valueTemp의 Index를 찾음
                    for (int i = 0; i < valueTemp.Length; i++)
                    {
                        if (valueTemp[i].StartsWith(fields[4].Name, StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }

                        findIdx++;
                    }

                    string prevReplace = valueTemp[findIdx];
                    string nextReplace = "";

                    // List 프로퍼티가 포함된 (Column 이름 = System.Collections.Generic.List`1[System.Int32])
                    // 따위의 값을 (Column 이름 = String) 형태로 교체
                    if (disaster.UserLevelIDs != null)
                    {
                        if (disaster.UserLevelIDs.Count > 0)
                        {
                            string levelTemp = string.Join(", ", disaster.UserLevelIDs.ToList());
                            nextReplace = prevReplace.Replace(prevReplace.Split('=')[1], string.Format(" '{0}'", levelTemp));
                        }
                        else
                        {
                            nextReplace = prevReplace.Replace(prevReplace.Split('=')[1], " NULL");
                        }
                    }
                    else
                    {
                        nextReplace = prevReplace.Replace(prevReplace.Split('=')[1], " NULL");
                    }

                    valueString = valueString.Replace(prevReplace, nextReplace);

                    query = string.Format("update {0} set {1}", tableName, valueString);

                    bool isNullable;

                    if (strCondition == null || strCondition.Length == 0)
                        strCondition = string.Format("{0} = {1}", Disaster.GetFieldName(Disaster.Fields.ID, out isNullable), disaster.ID);

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

        public bool UpdateDisasterType(DisasterType disasterType, string strCondition = null)
        {
            if (disasterType != null)
            {
                string tableName = DisasterType.TableName;
                string query = "";
                ArrayList res = null;

                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = disasterType.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                string valueString = m_dbManager.ConvertUpdateParamsToString(info, disasterType, fields);

                if (valueString != null)
                {
                    query = string.Format("update {0} set {1}", tableName, valueString);

                    bool isNullable;

                    if (strCondition == null || strCondition.Length == 0)
                        strCondition = string.Format("{0} = {1}", DisasterType.GetFieldName(DisasterType.Fields.ID, out isNullable), disasterType.ID);

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

        public bool UpdateDisasterCategory(DisasterCategory disasterCategory, string strCondition = null)
        {
            if (disasterCategory != null)
            {
                string tableName = DisasterCategory.TableName;
                string query = "";
                ArrayList res = null;

                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = disasterCategory.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                string valueString = m_dbManager.ConvertUpdateParamsToString(info, disasterCategory, fields);

                if (valueString != null)
                {
                    query = string.Format("update {0} set {1}", tableName, valueString);

                    bool isNullable;

                    if (strCondition == null || strCondition.Length == 0)
                        strCondition = string.Format("{0} = {1}", DisasterCategory.GetFieldName(DisasterCategory.Fields.ID, out isNullable), disasterCategory.ID);

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

        public bool UpdateEndPoint(EndPoint endPoint, string strCondition = null)
        {
            if (endPoint != null)
            {
                string tableName = EndPoint.TableName;
                string query = "";
                ArrayList res = null;

                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = endPoint.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                string valueString = m_dbManager.ConvertUpdateParamsToString(info, endPoint, fields);

                if (valueString != null)
                {
                    query = string.Format("update {0} set {1}", tableName, valueString);

                    bool isNullable;

                    if (strCondition == null || strCondition.Length == 0)
                        strCondition = string.Format("{0} = {1}", EndPoint.GetFieldName(EndPoint.Fields.ID, out isNullable), endPoint.ID);

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

        public bool UpdateExternalProgram(ExternalProgram program, string strCondition = null)
        {
            if (program != null)
            {
                string tableName = ExternalProgram.TableName;
                string query = "";
                ArrayList res = null;

                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = program.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                string valueString = m_dbManager.ConvertUpdateParamsToString(info, program, fields);

                if (valueString != null)
                {
                    query = string.Format("update {0} set {1}", tableName, valueString);

                    bool isNullable;

                    if (strCondition == null || strCondition.Length == 0)
                        strCondition = string.Format("{0} = {1}", ExternalProgram.GetFieldName(ExternalProgram.Fields.ID, out isNullable), program.ID);

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

        public bool UpdateExternalProgramParameter(ExternalProgramParameter parameter, string strCondition = null)
        {
            if (parameter != null)
            {
                string tableName = ExternalProgramParameter.TableName;
                string query = "";
                ArrayList res = null;

                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = parameter.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                string valueString = m_dbManager.ConvertUpdateParamsToString(info, parameter, fields);

                if (valueString != null)
                {
                    query = string.Format("update {0} set {1}", tableName, valueString);

                    bool isNullable;

                    if (strCondition == null || strCondition.Length == 0)
                        strCondition = string.Format("{0} = {1} and {2} = {3}",
                            ExternalProgramParameter.GetFieldName(ExternalProgramParameter.Fields.ProgramID, out isNullable), parameter.ProgramID,
                            ExternalProgramParameter.GetFieldName(ExternalProgramParameter.Fields.ParameterIndex, out isNullable), parameter.ParameterIndex);

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

        public bool UpdateGrid(SectionGrid grid, string strCondition = null)
        {
            if (grid != null)
            {
                string tableName = SectionGrid.TableName;
                string query = "";
                ArrayList res = null;

                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = grid.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                string valueString = m_dbManager.ConvertUpdateParamsToString(info, grid, fields);

                if (valueString != null)
                {
                    query = string.Format("update {0} set {1}", tableName, valueString);

                    bool isNullable;

                    if (strCondition == null || strCondition.Length == 0)
                        strCondition = string.Format("{0} = {1}", SectionGrid.GetFieldName(SectionGrid.Fields.ID, out isNullable), grid.ID);

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

        public bool UpdateGridColumn(SectionGridColumn column, string strCondition = null)
        {
            if (column != null)
            {
                string tableName = SectionGridColumn.TableName;
                string query = "";
                ArrayList res = null;

                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = column.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                string valueString = m_dbManager.ConvertUpdateParamsToString(info, column, fields);

                if (valueString != null)
                {
                    query = string.Format("update {0} set {1}", tableName, valueString);

                    bool isNullable;

                    if (strCondition == null || strCondition.Length == 0)
                        strCondition = string.Format("{0} = {1} and {2} = {3}", SectionGridColumn.GetFieldName(SectionGridColumn.Fields.GridID, out isNullable), column.GridID,
                            SectionGridColumn.GetFieldName(SectionGridColumn.Fields.ColumnIndex, out isNullable), column.ColumnIndex);

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

        public bool UpdateGridRow(SectionGridRow row, string strCondition = null)
        {
            if (row != null)
            {
                string tableName = SectionGridRow.TableName;
                string query = "";
                ArrayList res = null;

                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = row.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                string valueString = m_dbManager.ConvertUpdateParamsToString(info, row, fields);

                if (valueString != null)
                {
                    query = string.Format("update {0} set {1}", tableName, valueString);

                    bool isNullable;

                    if (strCondition == null || strCondition.Length == 0)
                        strCondition = string.Format("{0} = {1} and {2} = {3}", SectionGridRow.GetFieldName(SectionGridRow.Fields.GridID, out isNullable), row.GridID,
                            SectionGridRow.GetFieldName(SectionGridRow.Fields.RowIndex, out isNullable), row.RowIndex);

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

        public bool UpdateInternalTransmission(InternalTransmission internalTransmission, string strCondition = null)
        {
            if (internalTransmission != null)
            {
                string tableName = InternalTransmission.TableName;
                string query = "";
                ArrayList res = null;

                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = internalTransmission.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                string valueString = m_dbManager.ConvertUpdateParamsToString(info, internalTransmission, fields);

                if (valueString != null)
                {
                    string[] separator = { ", " };
                    string[] valueTemp = valueString.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    int findIdx = 0;

                    // 이전 필드 값이 ", "가 들어가는 경우 fields의 Index와 valueTemp의 Index가 맞지 않으므로 찾는 field 이름이 포함된 valueTemp의 Index를 찾음
                    for (int i = 0; i < valueTemp.Length; i++)
                    {
                        if (valueTemp[i].StartsWith(fields[4].Name, StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }

                        findIdx++;
                    }

                    string prevReplace = valueTemp[findIdx];
                    string nextReplace = "";

                    if (internalTransmission.TeamList != null)
                    {
                        if (internalTransmission.TeamList.Count > 0)
                        {
                            nextReplace = CreateManager.MakeTeamListString(internalTransmission.TeamList);
                            /*for (int i = 0; i < internalTransmission.TeamList.Count; i++)
                            {
                                nextReplace += string.Format("{0}({1}), ", internalTransmission.TeamList[i].Value, internalTransmission.TeamList[i].Key);
                            }


                            if (nextReplace.EndsWith(", "))
                            {
                                nextReplace = nextReplace.Substring(0, nextReplace.Length - 2);
                            }*/

                            nextReplace = string.Format(" '{0}'", nextReplace);
                            nextReplace = prevReplace.Replace(prevReplace.Split('=')[1], nextReplace);
                        }
                        else
                        {
                            nextReplace = prevReplace.Replace(prevReplace.Split('=')[1], " NULL");
                        }
                    }
                    else
                    {
                        nextReplace = prevReplace.Replace(prevReplace.Split('=')[1], " NULL");
                    }

                    valueString = valueString.Replace(prevReplace, nextReplace);

                    query = string.Format("update {0} set {1}", tableName, valueString);

                    bool isNullable;

                    if (strCondition == null || strCondition.Length == 0)
                        strCondition = string.Format("{0} = {1}", InternalTransmission.GetFieldName(InternalTransmission.Fields.ID, out isNullable), internalTransmission.ID);

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

        public bool UpdateLink(Link link, string strCondition = null)
        {
            if (link != null)
            {
                string tableName = Link.TableName;
                string query = "";
                ArrayList res = null;

                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = link.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                string valueString = m_dbManager.ConvertUpdateParamsToString(info, link, fields);

                if (valueString != null)
                {
                    string[] separator = { ", " };
                    string[] valueTemp = valueString.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    int findIdx = 0;

                    // 이전 필드 값이 ", "가 들어가는 경우 fields의 Index와 valueTemp의 Index가 맞지 않으므로 찾는 field 이름이 포함된 valueTemp의 Index를 찾음
                    for (int i = 0; i < valueTemp.Length; i++)
                    {
                        if (valueTemp[i].StartsWith(fields[1].Name, StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }

                        findIdx++;
                    }

                    string prevReplace = valueTemp[findIdx];
                    string nextReplace = "";

                    if (link.LinkedComponentIDList != null)
                    {
                        if (link.LinkedComponentIDList.Count > 0)
                        {
                            string componentTemp = string.Join("; ", link.LinkedComponentIDList.ToList());
                            nextReplace = prevReplace.Replace(prevReplace.Split('=')[1], string.Format(" '{0}'", componentTemp));
                        }
                        else
                        {
                            nextReplace = prevReplace.Replace(prevReplace.Split('=')[1], " NULL");
                        }
                    }
                    else
                    {
                        nextReplace = prevReplace.Replace(prevReplace.Split('=')[1], " NULL");
                    }

                    valueString = valueString.Replace(prevReplace, nextReplace);

                    query = string.Format("update {0} set {1}", tableName, valueString);

                    bool isNullable;

                    if (strCondition == null || strCondition.Length == 0)
                        strCondition = string.Format("{0} = {1}", Link.GetFieldName(Link.Fields.ID, out isNullable), link.ID);

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

        public bool UpdateProcess(Process process, string strCondition = null)
        {
            if (process != null)
            {
                string tableName = Process.TableName;
                string query = "";
                ArrayList res = null;

                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = process.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                string valueString = m_dbManager.ConvertUpdateParamsToString(info, process, fields);

                if (valueString != null)
                {
                    string[] separator = { ", " };
                    string[] valueTemp = valueString.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    int findIdx = 0;

                    // 이전 필드 값이 ", "가 들어가는 경우 fields의 Index와 valueTemp의 Index가 맞지 않으므로 찾는 field 이름이 포함된 valueTemp의 Index를 찾음
                    for (int i = 0; i < valueTemp.Length; i++)
                    {
                        if (valueTemp[i].StartsWith(fields[1].Name, StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }

                        findIdx++;
                    }

                    string prevReplace = valueTemp[findIdx];
                    string nextReplace = "";

                    if (process.TeamList != null)
                    {
                        if (process.TeamList.Count > 0)
                        {
                            nextReplace = CreateManager.MakeTeamListString(process.TeamList);
                            /*for (int i = 0; i < process.TeamList.Count; i++)
                            {
                                nextReplace += string.Format("{0}({1}), ", process.TeamList[i].Value, process.TeamList[i].Key);
                            }

                            if (nextReplace.EndsWith(", "))
                            {
                                nextReplace = nextReplace.Substring(0, nextReplace.Length - 2);
                            }*/

                            nextReplace = string.Format(" '{0}'", nextReplace);
                            nextReplace = prevReplace.Replace(prevReplace.Split('=')[1], nextReplace);
                        }
                        else
                        {
                            nextReplace = prevReplace.Replace(prevReplace.Split('=')[1], " NULL");
                        }
                    }
                    else
                    {
                        nextReplace = prevReplace.Replace(prevReplace.Split('=')[1], " NULL");
                    }

                    valueString = valueString.Replace(prevReplace, nextReplace);

                    query = string.Format("update {0} set {1}", tableName, valueString);

                    bool isNullable;

                    if (strCondition == null || strCondition.Length == 0)
                        strCondition = string.Format("{0} = {1}", Process.GetFieldName(Process.Fields.ID, out isNullable), process.ID);

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

        public bool UpdateProcessMission(ProcessMission processMission, string strCondition = null)
        {
            if (processMission != null)
            {
                string tableName = ProcessMission.TableName;
                string query = "";
                ArrayList res = null;

                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = processMission.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                string valueString = m_dbManager.ConvertUpdateParamsToString(info, processMission, fields);

                if (valueString != null)
                {
                    query = string.Format("update {0} set {1}", tableName, valueString);

                    bool isNullable;

                    if (strCondition == null || strCondition.Length == 0)
                        strCondition = string.Format("{0} = {1}", ProcessMission.GetFieldName(ProcessMission.Fields.ID, out isNullable), processMission.ID);

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

        public bool UpdateProcessExternalMission(ProcessExternalMission processExternalMission, string strCondition = null)
        {
            if (processExternalMission != null)
            {
                string tableName = ProcessExternalMission.TableName;
                string query = "";
                ArrayList res = null;

                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = processExternalMission.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                string valueString = m_dbManager.ConvertUpdateParamsToString(info, processExternalMission, fields);

                if (valueString != null)
                {
                    query = string.Format("update {0} set {1}", tableName, valueString);

                    bool isNullable;

                    if (strCondition == null || strCondition.Length == 0)
                        strCondition = string.Format("{0} = {1} and {2} = {3} and {4} = {5} and {6} = {7}",
                            ProcessExternalMission.GetFieldName(ProcessExternalMission.Fields.ProcessID, out isNullable),
                            processExternalMission.ProcessID,
                            ProcessExternalMission.GetFieldName(ProcessExternalMission.Fields.OrderIndex, out isNullable),
                            processExternalMission.OrderIndex,
                            ProcessExternalMission.GetFieldName(ProcessExternalMission.Fields.ProgramID, out isNullable),
                            processExternalMission.ProgramID,
                            ProcessExternalMission.GetFieldName(ProcessExternalMission.Fields.ParameterIndex, out isNullable),
                            processExternalMission.ParameterIndex);

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

        public bool UpdateStepMember(StepMember stepMember, string strCondition = null)
        {
            if (stepMember != null)
            {
                string tableName = StepMember.TableName;
                string query = "";
                ArrayList res = null;

                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = stepMember.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                string valueString = m_dbManager.ConvertUpdateParamsToString(info, stepMember, fields);

                if (valueString != null)
                {
                    query = string.Format("update {0} set {1}", tableName, valueString);

                    bool isNullable;

                    if (strCondition == null || strCondition.Length == 0)
                        strCondition = string.Format("{0} = {1}", StepMember.GetFieldName(StepMember.Fields.ID, out isNullable), stepMember.ID);

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

        public bool UpdateSubDisasterCategory(SubDisasterCategory subDisasterCategory, string strCondition = null)
        {
            if (subDisasterCategory != null)
            {
                string tableName = SubDisasterCategory.TableName;
                string query = "";
                ArrayList res = null;

                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = subDisasterCategory.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                string valueString = m_dbManager.ConvertUpdateParamsToString(info, subDisasterCategory, fields);

                if (valueString != null)
                {
                    query = string.Format("update {0} set {1}", tableName, valueString);

                    bool isNullable;

                    if (strCondition == null || strCondition.Length == 0)
                        strCondition = string.Format("{0} = {1}", SubDisasterCategory.GetFieldName(SubDisasterCategory.Fields.ID, out isNullable), subDisasterCategory.ID);

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

        public bool UpdateVersion(Model.Sop.Category.Version version, string strCondition = null)
        {
            if (version != null)
            {
                string tableName = Model.Sop.Category.Version.TableName;
                string query = "";
                ArrayList res = null;

                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = version.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                string valueString = m_dbManager.ConvertUpdateParamsToString(info, version, fields);

                if (valueString != null)
                {
                    query = string.Format("update {0} set {1}", tableName, valueString);

                    bool isNullable;

                    if (strCondition == null || strCondition.Length == 0)
                        strCondition = string.Format("{0} = {1}", Model.Sop.Category.Version.GetFieldName(Model.Sop.Category.Version.Fields.ID, out isNullable), version.ID);

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

        public bool UpdateSpecialMessage(SpecialMessage message, string strCondition = null)
        {
            if (message != null)
            {
                string tableName = SpecialMessage.TableName;
                string query = "";
                ArrayList res = null;

                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = message.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                string valueString = m_dbManager.ConvertUpdateParamsToString(info, message, fields);

                if (valueString != null)
                {
                    query = string.Format("update {0} set {1}", tableName, valueString);

                    bool isNullable;

                    if (strCondition == null || strCondition.Length == 0)
                        strCondition = string.Format("{0} = {1}", SpecialMessage.GetFieldName(SpecialMessage.Fields.ID, out isNullable), message.ID);

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

        public bool UpdateSession(Session session, string strCondition = null)
        {
            if (session != null)
            {
                string tableName = Session.TableName;
                string query = "";
                ArrayList res = null;

                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = session.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                string valueString = m_dbManager.ConvertUpdateParamsToString(info, session, fields);

                if (valueString != null)
                {
                    query = string.Format("update {0} set {1}", tableName, valueString);

                    bool isNullable;

                    if (strCondition == null || strCondition.Length == 0)
                        strCondition = string.Format("{0} = {1}", Session.GetFieldName(Session.Fields.ID, out isNullable), session.ID);

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

        public bool UpdateLinkedSop(LinkedSop linkedSop, string strCondition = null)
        {
            if (linkedSop != null)
            {
                string tableName = LinkedSop.TableName;
                string query = "";
                ArrayList res = null;

                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = linkedSop.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                string valueString = m_dbManager.ConvertUpdateParamsToString(info, linkedSop, fields);

                if (valueString != null)
                {
                    query = string.Format("update {0} set {1}", tableName, valueString);

                    bool isNullable;

                    if (strCondition == null || strCondition.Length == 0)
                        strCondition = string.Format("{0} = {1}", LinkedSop.GetFieldName(LinkedSop.Fields.ID, out isNullable), linkedSop.ID);

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

        public string GetErrorMessage()
        {
            return m_strErrorMessage;
        }
    }
}
