using System;

namespace Dashboard.DAL
{
    using Dashboard.Model;
    using dnsDBUtil;
    using IDAL;
    using System.Collections;
    using System.Collections.Generic;

    public class SelectManager : QueryManager, ISelect
    {
        private DataManager m_dataManager = null;

        public SelectManager(DataManager dataManager)
        {
            m_dataManager = dataManager;
            m_dbManager = m_dataManager.GetDBManager() as WebDBManager;
        }

        public CurrentWorkPermit SelectCurrentWorkPermit(string strPlantPrcsID, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;
            bool isNullable;

            string strSQL = string.Format("select {0} from {1} where {2} = {3}", GetFieldNames<CurrentWorkPermit.Fields>(out nFieldCount), CurrentWorkPermit.TableName, CurrentWorkPermit.GetFieldName(CurrentWorkPermit.Fields.PLANT_PRCS_ID, out isNullable), strPlantPrcsID);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                CurrentWorkPermit model = ReadCurrentWorkPermit(arrResult, 0, out strErrorMessage);

                if (model == null)
                    return null;

                return model;
            }
            else
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        public List<CurrentWorkPermit> SelectCurrentWorkPermits(Dictionary<CurrentWorkPermit.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectCurrentWorkPermits(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<CurrentWorkPermit> SelectCurrentWorkPermits(Dictionary<CurrentWorkPermit.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<CurrentWorkPermit.Fields>(out nFieldCount), CurrentWorkPermit.TableName);

            string strCondition = "";

            if (SetCondition<CurrentWorkPermit.Fields>(ref strCondition, dicConditions, CurrentWorkPermit.GetFieldName, CurrentWorkPermit.TableName, ref strErrorMessage) == false)
                return null;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            if (strCondition.Length > 0)
            {
                if (strCondition.Trim().ToLower().StartsWith("order by"))
                    strSQL += " " + strCondition;
                else
                    strSQL += " where " + strCondition;
            }

            ArrayList arrResult = topNCount == null ? m_dbManager.GetResultData(strSQL) : m_dbManager.GetResultData(strSQL, (int)topNCount);

            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            int nResultCount = arrResult.Count;
            List<CurrentWorkPermit> currentWorkPermits = new List<CurrentWorkPermit>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                CurrentWorkPermit model = ReadCurrentWorkPermit(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    currentWorkPermits.Add(model);
            }

            return currentWorkPermits;
        }


        private CurrentWorkPermit ReadCurrentWorkPermit(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            CurrentWorkPermit model = new CurrentWorkPermit();
            bool isNullable;

            foreach (CurrentWorkPermit.Fields field in CurrentWorkPermit.Fields.GetValues(typeof(CurrentWorkPermit.Fields)))
            {
                string strFieldName = CurrentWorkPermit.GetFieldName(field, out isNullable);

                if (field == CurrentWorkPermit.Fields.GENERAL_CNT)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.GENERAL_CNT = data.Data;
                    }
                }
                else if (field == CurrentWorkPermit.Fields.FIRE_CNT)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.FIRE_CNT = data.Data;
                    }
                }
                else if (field == CurrentWorkPermit.Fields.HIGH_CNT)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.HIGH_CNT = data.Data;
                    }
                }
                else if (field == CurrentWorkPermit.Fields.ELEC_CNT)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.ELEC_CNT = data.Data;
                    }
                }
                else if (field == CurrentWorkPermit.Fields.CLOSENESS_CNT)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.CLOSENESS_CNT = data.Data;
                    }
                }
                else if (field == CurrentWorkPermit.Fields.CRANE_CNT)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.CRANE_CNT = data.Data;
                    }
                }
                else if (field == CurrentWorkPermit.Fields.DIGG_CNT)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.DIGG_CNT = data.Data;
                    }
                }
                else if (field == CurrentWorkPermit.Fields.RADI_CNT)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.RADI_CNT = data.Data;
                    }
                }
                else if (field == CurrentWorkPermit.Fields.TOTAL_CNT)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.TOTAL_CNT = data.Data;
                    }
                }
                else if (field == CurrentWorkPermit.Fields.PLANT_PRCS_ID)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.PLANT_PRCS_ID = str;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.PLANT_PRCS_ID = str;
                }
                else if (field == CurrentWorkPermit.Fields.UpdateTime)
                {
                    VariousData<DateTime> data = WebDBManager.GetDateTimeField(arrResult[index]);

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.UpdateTime = data.Data;
                }

                index++;
            }

            return model;
        }
    }

}
