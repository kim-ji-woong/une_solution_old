using Common.Model.History;
using dnsDBUtil;
using SOPManager.Model.Sop.Category;
using SOPSimulator.IDAL;
using System.Collections;
using System.Collections.Generic;

namespace SOPSimulator.DAL
{
    public class SelectManager : QueryManager, ISelect
    {
        private DataManager m_dataManager = null;
        //private WebDBManager m_dbManager = null;

        public SelectManager(DataManager dataManager)
        {
            m_dataManager = dataManager;
            m_dbManager = m_dataManager.GetDBManager() as WebDBManager;
        }

        public ArrayList JoinHistoryComponentActionStep(int actionStepHistoryID, out string strErrorMessage)
        {
            string strHistoryComponentTableName = ComponentHistory.TableName;
            string strHistoryActionStepTableName = ActionStepHistory.TableName;
            string strCategoryActionStepTableName = ActionStep.TableName;

            string strCondition = string.Format("{0}.{1} = {2}.{3} And {2}.{4} = {5}.{6} And {2}.{3} = {7}"
                                                , strHistoryComponentTableName
                                                , ComponentHistory.Fields.ActionStepHistoryID
                                                , strHistoryActionStepTableName
                                                , ActionStepHistory.Fields.ID
                                                , ActionStepHistory.Fields.ActionStepID
                                                , strCategoryActionStepTableName
                                                , ActionStep.Fields.ID
                                                , actionStepHistoryID);

            return JoinHistoryComponentActionStep(strCondition, out strErrorMessage);
        }

        private ArrayList JoinHistoryComponentActionStep(string strCondition, out string strErrorMessage)
        {
            string strHistoryComponentTableName = ComponentHistory.TableName;
            string strHistoryActionStepTableName = ActionStepHistory.TableName;
            string strCategoryActionStepTableName = ActionStep.TableName;

            int nHistoryComponentFieldCount, nHistoryActionStepFieldCount, nActionStepFieldCount;
            string strHistoryComponentFields = GetFieldNames<ComponentHistory.Fields>(strHistoryComponentTableName, out nHistoryComponentFieldCount);
            string strHistoryActionStepFields = GetFieldNames<ActionStepHistory.Fields>(strHistoryActionStepTableName, out nHistoryActionStepFieldCount);
            string strActionStepFields = GetFieldNames<ActionStep.Fields>(strCategoryActionStepTableName, out nActionStepFieldCount);

            int nFieldsCount = nHistoryComponentFieldCount + nHistoryActionStepFieldCount + nActionStepFieldCount;

            string strSQL = string.Format("Select {0}, {1}, {2} from {3}, {4}, {5} "
                , strHistoryComponentFields, strHistoryActionStepFields, strActionStepFields, strHistoryComponentTableName, strHistoryActionStepTableName, strCategoryActionStepTableName);

            if (strCondition != null && strCondition.Length > 0)
            {
                strSQL += " where " + strCondition;
            }

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            var historyComponentTableInfo = m_dbManager.GetColumnInfoDictionary(strHistoryComponentTableName);
            var historyActionStepTableInfo = m_dbManager.GetColumnInfoDictionary(strHistoryActionStepTableName);
            var actionStepTableInfo = m_dbManager.GetColumnInfoDictionary(strCategoryActionStepTableName);
            var historyComponentFields = GetProperties<ComponentHistory>();
            var historyActionFields = GetProperties<ActionStepHistory>();
            var actionStepFields = GetProperties<ActionStep>();

            Dictionary<string, int> dicHistoryComponentFieldIndex, dicHistoryActionStepFieldIndex, dicActionStepFieldIndex;
            List<string> historyComponentFieldNames = GetFieldNameIndex<ComponentHistory.Fields>(out dicHistoryComponentFieldIndex);
            List<string> historyActionStepFieldNames = GetFieldNameIndex<ActionStepHistory.Fields>(out dicHistoryActionStepFieldIndex);
            List<string> actionStepFieldNames = GetFieldNameIndex<ActionStep.Fields>(out dicActionStepFieldIndex);
            string[] notExistMember;

            strErrorMessage = null;
            int nResultCount = arrResult.Count;

            ArrayList arrDatas = new ArrayList();

            for (int i = 0; i < nResultCount - (nFieldsCount - 1); i += nFieldsCount)
            {
                ArrayList arrHistoryComponentResult = SortWithProperties(ParseArray(arrResult, i, nHistoryComponentFieldCount), ref historyComponentFields, historyComponentFieldNames, dicHistoryComponentFieldIndex);
                ArrayList arrHistoryActionStepResult = SortWithProperties(ParseArray(arrResult, i + nHistoryComponentFieldCount, nHistoryActionStepFieldCount), ref historyActionFields, historyActionStepFieldNames, dicHistoryActionStepFieldIndex);
                ArrayList arrActionStepResult = SortWithProperties(ParseArray(arrResult, i + nHistoryComponentFieldCount + nHistoryActionStepFieldCount, nActionStepFieldCount), ref actionStepFields, actionStepFieldNames, dicActionStepFieldIndex);

                if (arrHistoryComponentResult == null || arrHistoryActionStepResult == null || arrActionStepResult == null)
                    return null;

                List<object> historyComponents = m_dbManager.SetParamsWithColumnInfo(historyComponentTableInfo, new ComponentHistory(), historyComponentFields, arrHistoryComponentResult, out notExistMember);
                List<object> historyActionSteps = m_dbManager.SetParamsWithColumnInfo(historyActionStepTableInfo, new ActionStepHistory(), historyActionFields, arrHistoryActionStepResult, out notExistMember);
                List<object> actionSteps = m_dbManager.SetParamsWithColumnInfo(actionStepTableInfo, new ActionStep(), actionStepFields, arrActionStepResult, out notExistMember);

                if (historyComponents == null || historyActionSteps == null || actionSteps == null ||
                    historyComponents.Count != 1 || historyActionSteps.Count != 1 || actionSteps.Count != 1)
                    return null;

                arrDatas.Add(historyComponents[0]);
                arrDatas.Add(historyActionSteps[0]);
                arrDatas.Add(actionSteps[0]);
            }

            return arrDatas;
        }
    }
}
