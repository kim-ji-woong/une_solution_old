using System.Collections.Generic;

namespace Common.DAL
{
    using Model;
    using Model.Option;
    using Model.History;
    using IDAL;
    using dnsDBUtil;
    using System.Collections;
    using System.Reflection;
    using UnE.Geometry;
    using System;
    using SOPManager.Model.Sop.Category;
    using System.Text;

    public class SelectManager : QueryManager, ISelect
    {
        private DataManager m_dataManager = null;
        //private WebDBManager m_dbManager = null;

        // 한번의 Select 쿼리를 실행할때마다 Table 정보를 읽기 위하여 Column List와 Column Type에 대한 쿼리를 각각 한번씩 수행한다.
        // 즉, 메인 쿼리 + 2번의 쿼리를 더하게 되는 것인데 매번 같은 테이블에 대하여 이러한 부가정보를 얻기 위하여 2번씩 쿼리를 더하는 것은 성능에 심각한 문제를 만들수 있다.
        // 테이블마다 첫번째 쿼리를 실행할 때에만 부가정보 쿼리를 실행하도록 하고, 그 정보는 아래의 Dictionary에 저장하도록 한다.
        private static Dictionary<string, Dictionary<string, string>> m_dicTableInfos = new Dictionary<string, Dictionary<string, string>>();

        public SelectManager(DataManager dataManager)
        {
            m_dataManager = dataManager;
            m_dbManager = m_dataManager.GetDBManager() as WebDBManager;
        }

        // Option
        public Options SelectOption(Options.OptionTarget eTargetName, int id, out string strErrorMessage)
        {
            string tableName = string.Format("Option{0}", eTargetName.ToString());
            return (Options)SelectDataFromID<Options, Options.Fields>(id, tableName, Options.GetFieldName, Options.Fields.ID, new Options(), out strErrorMessage);
        }

        public List<Options> SelectOption(Options.OptionTarget eTargetName, string strPropertyName, out string strErrorMessage)
        {
            bool isNullable;
            string tableName = string.Format("Option{0}", eTargetName.ToString());
            string strCondition = strPropertyName != null && strPropertyName.Length > 0 ? string.Format("{0} = '{1}'", Options.GetFieldName(Options.Fields.PropertyName, out isNullable), strPropertyName) : null;
            return SelectDatas<Options, Options.Fields>(strCondition, tableName, new Options(), null, out strErrorMessage);
        }

        public List<Options> SelectOptions(Options.OptionTarget eTargetName, out string strErrorMessage)
        {
            return SelectOptions(eTargetName, null, null, out strErrorMessage);
        }

        // topNCount가 null이 아닐 경우 전체 데이터를 받아오지 않고 topNCount 개수만큼만 리턴하도록 한다.
        public List<Options> SelectOptions(Options.OptionTarget eTargetName, string strAdditionalCondition, int? topNCount, out string strErrorMessage)
        {
            string tableName = string.Format("Option{0}", eTargetName.ToString());
            return SelectDatas<Options, Options.Fields>(strAdditionalCondition, tableName, new Options(), topNCount, out strErrorMessage);
        }

        // History
        public ActionStepHistory SelectActionStepHistory(int id, out string strErrorMessage)
        {
            return (ActionStepHistory)SelectDataFromID<ActionStepHistory, ActionStepHistory.Fields>(id, ActionStepHistory.TableName, ActionStepHistory.GetFieldName, ActionStepHistory.Fields.ID, new ActionStepHistory(), out strErrorMessage);
        }

        public List<ActionStepHistory> SelectActionStepHistories(Dictionary<ActionStepHistory.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectActionStepHistories(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<ActionStepHistory> SelectActionStepHistories(Dictionary<ActionStepHistory.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<ActionStepHistory.Fields>(out nFieldCount), ActionStepHistory.TableName);

            string strCondition = "";

            if (SetCondition<ActionStepHistory.Fields>(ref strCondition, dicConditions, ActionStepHistory.GetFieldName, ActionStepHistory.TableName, ref strErrorMessage) == false)
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
            List<ActionStepHistory> histories = new List<ActionStepHistory>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                ActionStepHistory model = ReadActionStepHistory(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    histories.Add(model);
            }

            return histories;
        }

        public List<ActionStepHistory> SelectActionStepHistories(string strCondition, out string strErrorMessage)
        {
            return SelectActionStepHistories(strCondition, null, out strErrorMessage);
        }

        public List<ActionStepHistory> SelectActionStepHistories(string strCondition, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<ActionStepHistory, ActionStepHistory.Fields>(strCondition, ActionStepHistory.TableName, new ActionStepHistory(), topNCount, out strErrorMessage);
        }

        public ComponentHistory SelectComponentHistory(int id, out string strErrorMessage)
        {
            return (ComponentHistory)SelectDataFromID<ComponentHistory, ComponentHistory.Fields>(id, ComponentHistory.TableName, ComponentHistory.GetFieldName, ComponentHistory.Fields.ID, new ComponentHistory(), out strErrorMessage);
        }

        public List<ComponentHistory> SelectComponentHistories(Dictionary<ComponentHistory.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectComponentHistories(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<ComponentHistory> SelectComponentHistories(Dictionary<ComponentHistory.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<ComponentHistory.Fields>(out nFieldCount), ComponentHistory.TableName);

            string strCondition = "";

            if (SetCondition<ComponentHistory.Fields>(ref strCondition, dicConditions, ComponentHistory.GetFieldName, ComponentHistory.TableName, ref strErrorMessage) == false)
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
            List<ComponentHistory> histories = new List<ComponentHistory>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                ComponentHistory model = ReadComponentHistory(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    histories.Add(model);
            }

            return histories;
        }

        public List<ComponentHistory> SelectComponentHistories(string strCondition, out string strErrorMessage)
        {
            return SelectComponentHistories(strCondition, null, out strErrorMessage);
        }

        public List<ComponentHistory> SelectComponentHistories(string strCondition, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<ComponentHistory, ComponentHistory.Fields>(strCondition, ComponentHistory.TableName, new ComponentHistory(), topNCount, out strErrorMessage);
        }

        public ComponentHistoryDetail SelectComponentHistoryDetail(int id, out string strErrorMessage)
        {
            return (ComponentHistoryDetail)SelectDataFromID<ComponentHistoryDetail, ComponentHistoryDetail.Fields>(id, ComponentHistoryDetail.TableName, ComponentHistoryDetail.GetFieldName, ComponentHistoryDetail.Fields.ID, new ComponentHistoryDetail(), out strErrorMessage);
        }

        public List<ComponentHistoryDetail> SelectComponentHistoryDetails(string strCondition, out string strErrorMessage)
        {
            return SelectComponentHistoryDetails(strCondition, null, out strErrorMessage);
        }

        public List<ComponentHistoryDetail> SelectComponentHistoryDetails(string strCondition, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<ComponentHistoryDetail, ComponentHistoryDetail.Fields>(strCondition, ComponentHistoryDetail.TableName, new ComponentHistoryDetail(), topNCount, out strErrorMessage);
        }

        public ActionStepAutoClose SelectActionStepAutoClose(int id, out string strErrorMessage)
        {
            return (ActionStepAutoClose)SelectDataFromID<ActionStepAutoClose, ActionStepAutoClose.Fields>(id, ActionStepAutoClose.TableName, ActionStepAutoClose.GetFieldName, ActionStepAutoClose.Fields.ID, new ActionStepAutoClose(), out strErrorMessage);
        }

        public List<ActionStepAutoClose> SelectActionStepAutoCloses(string strCondition, out string strErrorMessage)
        {
            return SelectActionStepAutoCloses(strCondition, null, out strErrorMessage);
        }

        public List<ActionStepAutoClose> SelectActionStepAutoCloses(string strCondition, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<ActionStepAutoClose, ActionStepAutoClose.Fields>(strCondition, ActionStepAutoClose.TableName, new ActionStepAutoClose(), topNCount, out strErrorMessage);
        }

        public Shelter SelectShelter(int id, out string strErrorMessage)
        {
            Dictionary<string, int> dicFieldIndex = null;
            ArrayList arrDataResult = null;
            Shelter shelter = (Shelter)SelectDataFromID<Shelter, Shelter.Fields>(id, Shelter.TableName, Shelter.GetFieldName, Shelter.Fields.ID, new Shelter(), out strErrorMessage, ref dicFieldIndex, ref arrDataResult);

            if (shelter != null && dicFieldIndex != null && arrDataResult != null)
            {
                bool isNullable;
                string strFieldName = Shelter.GetFieldName(Shelter.Fields.Boundary, out isNullable).ToLower();

                int index;

                if (dicFieldIndex.TryGetValue(strFieldName, out index))
                {
                    if (arrDataResult.Count > index)
                    {
                        if (arrDataResult[index] is string)
                        {
                            string strBoundary = (string)arrDataResult[index];

                            if (strBoundary.Length > 0)
                                shelter.Boundary = StringToShelterBoundary(strBoundary);
                        }
                    }
                }
            }

            return shelter;
        }

        public List<Shelter> SelectShelters(string strCondition, out string strErrorMessage)
        {
            return SelectShelters(strCondition, null, out strErrorMessage);
        }

        public List<Shelter> SelectShelters(string strCondition, int? topNCount, out string strErrorMessage)
        {
            Dictionary<string, int> dicFieldIndex = null;
            ArrayList arrDataResult = null;
            List<Shelter> shelters = SelectDatas<Shelter, Shelter.Fields>(strCondition, Shelter.TableName, new Shelter(), topNCount, out strErrorMessage, ref dicFieldIndex, ref arrDataResult);

            if (shelters != null && dicFieldIndex != null && arrDataResult != null)
            {
                bool isNullable;
                string strFieldName = Shelter.GetFieldName(Shelter.Fields.Boundary, out isNullable).ToLower();

                int index;

                if (dicFieldIndex.TryGetValue(strFieldName, out index))
                {
                    int nFieldCount = dicFieldIndex.Count;
                    int nShelterCount = shelters.Count;

                    for (int i = 0; i < nShelterCount; i++)
                    {
                        Shelter shelter = shelters[i];
                        int nFieldIndex = i * nFieldCount + index;

                        if (arrDataResult.Count > nFieldIndex)
                        {
                            if (arrDataResult[nFieldIndex] is string)
                            {
                                string strBoundary = (string)arrDataResult[nFieldIndex];

                                if (strBoundary.Length > 0)
                                    shelter.Boundary = StringToShelterBoundary(strBoundary);
                            }
                        }
                    }
                }
            }

            return shelters;
        }

        public Site SelectSite(int id, out string strErrorMessage)
        {
            Dictionary<string, int> dicFieldIndex = null;
            ArrayList arrDataResult = null;
            return (Site)SelectDataFromID<Site, Site.Fields>(id, Site.TableName, Site.GetFieldName, Site.Fields.ID, new Site(), out strErrorMessage, ref dicFieldIndex, ref arrDataResult);
        }

        public List<Site> SelectSites(string strCondition, out string strErrorMessage)
        {
            return SelectSites(strCondition, null, out strErrorMessage);
        }

        public List<Site> SelectSites(string strCondition, int? topNCount, out string strErrorMessage)
        {
            Dictionary<string, int> dicFieldIndex = null;
            ArrayList arrDataResult = null;
            return SelectDatas<Site, Site.Fields>(strCondition, Site.TableName, new Site(), topNCount, out strErrorMessage, ref dicFieldIndex, ref arrDataResult);
        }

        private object SelectDataFromID<DataType, EnumType>(int id, string strTableName, GetFieldNameMethod<EnumType> getFieldName, EnumType idType, DataType model, out string strErrorMessage)
        {
            Dictionary<string, int> dicFieldIndex = null;
            ArrayList arrDataResult = null;
            return SelectDataFromID<DataType, EnumType>(id, strTableName, getFieldName, idType, model, out strErrorMessage, ref dicFieldIndex, ref arrDataResult);
        }

        private object SelectDataFromID<DataType, EnumType>(int id, string strTableName, GetFieldNameMethod<EnumType> getFieldName, EnumType idType, DataType model, out string strErrorMessage, ref Dictionary<string, int> dicFieldIndex, ref ArrayList arrDataResult)
        {
            int nFieldCount;
            string strFields = GetFieldNames<EnumType>(strTableName, out nFieldCount);

            bool isNullable;

            string strSQL = string.Format("Select {0} from {1} where {2} = {3}", strFields, strTableName, getFieldName(idType, out isNullable), id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            Dictionary<string, string> tableInfo;

            if (m_dicTableInfos.TryGetValue(strTableName, out tableInfo) == false)
            {
                tableInfo = m_dbManager.GetColumnInfoDictionary(strTableName);
                m_dicTableInfos[strTableName] = tableInfo;
            }

            var fields = GetProperties<DataType>();

            List<string> fieldNames = GetFieldNameIndex<EnumType>(out dicFieldIndex);
            string[] notExistMember;

            strErrorMessage = null;
            int nResultCount = arrResult.Count;

            if (nResultCount < nFieldCount)
            {
                return null;
            }

            arrDataResult = SortWithProperties(ParseArray(arrResult, 0, nFieldCount), ref fields, fieldNames, dicFieldIndex);

            if (arrDataResult == null)
            {
                strErrorMessage = "Query Result is wrong !";
                return null;
            }

            List<object> datas = m_dbManager.SetParamsWithColumnInfo(tableInfo, model, fields, arrDataResult, out notExistMember);

            if (datas == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            if (datas.Count == 0)
                return null;

            return datas[0];
        }

        private List<DataType> SelectDatas<DataType, EnumType>(string strCondition, string strTableName, DataType model, int? topNCount, out string strErrorMessage)
        {
            Dictionary<string, int> dicFieldIndex = null;
            ArrayList arrDataResult = null;
            return SelectDatas<DataType, EnumType>(strCondition, strTableName, model, topNCount, out strErrorMessage, ref dicFieldIndex, ref arrDataResult);
        }

        private List<DataType> SelectDatas<DataType, EnumType>(string strCondition, string strTableName, DataType model, int? topNCount, out string strErrorMessage, ref Dictionary<string, int> dicFieldIndex, ref ArrayList arrDataResult)
        {
            strErrorMessage = null;

            int nFieldCount;
            string strFields = GetFieldNames<EnumType>(strTableName, out nFieldCount);

            string strSQL = string.Format("Select {0} from {1}", strFields, strTableName);

            if (strCondition != null && strCondition.Length > 0)
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

            Dictionary<string, string> tableInfo;

            if (m_dicTableInfos.TryGetValue(strTableName, out tableInfo) == false)
            {
                tableInfo = m_dbManager.GetColumnInfoDictionary(strTableName);
                m_dicTableInfos[strTableName] = tableInfo;
            }

            var fields = GetProperties<DataType>();

            List<string> fieldNames = GetFieldNameIndex<EnumType>(out dicFieldIndex);
            string[] notExistMember;

            strErrorMessage = null;
            int nResultCount = arrResult.Count;

            List<DataType> results = new List<DataType>();

            if (nResultCount < nFieldCount)
            {
                return results;
            }

            arrDataResult = SortWithProperties(arrResult, ref fields, fieldNames, dicFieldIndex);

            if (arrDataResult == null)
            {
                strErrorMessage = "Query Result is wrong !";
                return null;
            }

            List<object> datas = m_dbManager.SetParamsWithColumnInfo(tableInfo, model, fields, arrDataResult, out notExistMember);

            if (datas == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            foreach (object data in datas)
            {
                results.Add((DataType)data);
            }

            return results;
        }


        public KakaoInfo SelectKakaoInfo(out string strErrorMessage)
        {
            strErrorMessage = null;

            string strSQL = string.Format("Select {0} from {1}", GetFieldNames<KakaoInfo.Fields>(), KakaoInfo.GetTableName());
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            if (arrResult.Count == 0)
            {
                strErrorMessage = "DB에 카카오 알림톡 정보가 없습니다";
                return null;
            }

            KakaoInfo kakao = new KakaoInfo();
            kakao.ID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            kakao.CountryCode = WebDBManager.GetIntField(arrResult[1].ToString(), -1);
            kakao.SenderKey = WebDBManager.GetStringField(arrResult[2]);
            kakao.BsID = WebDBManager.GetStringField(arrResult[3]);
            kakao.BsPasswd = WebDBManager.GetStringField(arrResult[4]);

            if (kakao.ID == -1 || kakao.CountryCode == -1 || kakao.SenderKey.Length == 0 || kakao.BsID.Length == 0 || kakao.BsPasswd.Length == 0)
                return null;

            return kakao;
        }

        public string GetCurrentTime()
        {
            string strTime = "";
            if (m_dbManager.DatabaseType == WebDBManager.DBType.sqlserver)
            {
                System.Collections.ArrayList arrResult = m_dbManager.GetResultData("Select convert(varchar(19), GetDate(), 120)");

                if (arrResult == null || arrResult.Count < 1)
                    return "";

                strTime = WebDBManager.GetStringField(arrResult[0]);
            }
            else if (m_dbManager.DatabaseType == WebDBManager.DBType.mysql)
            {
                System.Collections.ArrayList arrResult = m_dbManager.GetResultData("SELECT current_date(), current_time()");

                if (arrResult == null || arrResult.Count < 2)
                    return "";

                string strDate = WebDBManager.GetStringField(arrResult[0]);
                strTime = strDate + WebDBManager.GetStringField(arrResult[1]);
            }
            else
            {
                DateTime dtNow = DateTime.Now;
                strTime = string.Format("{0}{1:00}{2:00}{3:00}{4:00}{5:00}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);
                return strTime;
            }

            strTime = strTime.Replace("-", "");
            strTime = strTime.Replace(":", "");
            strTime = strTime.Replace(" ", "");

            int nIndex = strTime.LastIndexOf('.');

            if (nIndex >= 0)
                strTime = strTime.Substring(0, nIndex);

            return strTime;
        }

        public UserHistory SelectUserHistory(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<UserHistory.Fields>(out nFieldCount), UserHistory.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                UserHistory model = ReadUserHistory(arrResult, 0, out strErrorMessage);

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

        public List<UserHistory> SelectUserHistories(Dictionary<UserHistory.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectUserHistories(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<UserHistory> SelectUserHistories(Dictionary<UserHistory.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<UserHistory.Fields>(out nFieldCount), UserHistory.TableName);

            string strCondition = "";

            if (SetCondition<UserHistory.Fields>(ref strCondition, dicConditions, UserHistory.GetFieldName, UserHistory.TableName, ref strErrorMessage) == false)
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
            List<UserHistory> userHistories = new List<UserHistory>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                UserHistory model = ReadUserHistory(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    userHistories.Add(model);
            }

            return userHistories;
        }

        private UserHistory ReadUserHistory(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            UserHistory model = new UserHistory();
            bool isNullable;

            foreach (UserHistory.Fields field in UserHistory.Fields.GetValues(typeof(UserHistory.Fields)))
            {
                string strFieldName = UserHistory.GetFieldName(field, out isNullable);

                if (field == UserHistory.Fields.ID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.ID = data.Data;
                    }
                }
                else if (field == UserHistory.Fields.Time)
                {
                    VariousData<DateTime> data = WebDBManager.GetDateTimeField(arrResult[index]);

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.Time = data.Data;
                }
                else if (field == UserHistory.Fields.UserID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.UserID = data.Data;
                    }
                }
                else if (field == UserHistory.Fields.TargetType)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.TargetType = data.Data;
                    }
                }
                else if (field == UserHistory.Fields.ActionType)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.ActionType = data.Data;
                    }
                }
                
                else if (field == UserHistory.Fields.HistoryContent)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.HistoryContent = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.HistoryContent = str;
                }

                index++;
            }

            return model;
        }

        public ArrayList JoinActionStepHistoryActionStep(Dictionary<ActionStepHistory.Fields, object> dicConditions1, Dictionary<ActionStep.Fields, object> dicConditions2, string strAdditionalConditions, out string strErrorMessage)
        {
            return JoinActionStepHistoryActionStep(dicConditions1, dicConditions2, strAdditionalConditions, null, out strErrorMessage);
        }

        public ArrayList JoinActionStepHistoryActionStep(Dictionary<ActionStepHistory.Fields, object> dicConditions1, Dictionary<ActionStep.Fields, object> dicConditions2, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strHistoryTableName = ActionStepHistory.TableName;
            string strActionStepTableName = ActionStep.TableName;

            int nHistoryFieldCount, nActionStepFieldCount;

            string strHistoryFields = GetFieldNames<ActionStepHistory.Fields>(strHistoryTableName, out nHistoryFieldCount);
            string strActionStepFields = GetFieldNames<ActionStep.Fields>(strActionStepTableName, out nActionStepFieldCount);

            int nFieldsCount = nHistoryFieldCount + nActionStepFieldCount;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Select {0}, {1} ", strHistoryFields, strActionStepFields);
            sb.AppendFormat("  From {0}, {1} ", strHistoryTableName, strActionStepTableName);
            sb.AppendFormat(" Where {0}.{1} = {2}.{3} ", strHistoryTableName, ActionStepHistory.Fields.ActionStepID
                                                       , strActionStepTableName, ActionStep.Fields.ID);

            string strCondition1 = "";
            if (SetCondition<ActionStepHistory.Fields>(ref strCondition1, dicConditions1, ActionStepHistory.GetFieldName, strHistoryTableName, ref strErrorMessage) == false)
                return null;

            if (strCondition1.Length > 0)
                sb.AppendFormat(" and {0}", strCondition1);

            string strCondition2 = "";
            if (SetCondition<ActionStep.Fields>(ref strCondition2, dicConditions2, ActionStep.GetFieldName, strActionStepTableName, ref strErrorMessage) == false)
                return null;

            if (strCondition2.Length > 0)
                sb.AppendFormat(" and {0}", strCondition2);

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                sb.AppendFormat(" and {0}", strAdditionalConditions);
            }

            ArrayList arrResult = topNCount == null ? m_dbManager.GetResultData(sb.ToString()) : m_dbManager.GetResultData(sb.ToString(), (int)topNCount);

            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            ArrayList arrDatas = new ArrayList();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - (nFieldsCount - 1); i += nFieldsCount)
            {
                ActionStepHistory reactionHistory = ReadActionStepHistory(arrResult, i, out strErrorMessage);
                if (reactionHistory == null)
                    return null;
                else
                    arrDatas.Add(reactionHistory);

                ActionStep history = ReadActionStep(arrResult, i + nHistoryFieldCount, out strErrorMessage);

                if (history == null)
                    return null;
                else
                    arrDatas.Add(history);
            }

            return arrDatas;
        }

        private ActionStepHistory ReadActionStepHistory(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            ActionStepHistory model = new ActionStepHistory();
            bool isNullable;

            foreach (ActionStepHistory.Fields field in ActionStepHistory.Fields.GetValues(typeof(ActionStepHistory.Fields)))
            {
                string strFieldName = ActionStepHistory.GetFieldName(field, out isNullable);

                if (field == ActionStepHistory.Fields.ID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.ID = data.Data;
                    }
                }
                else if (field == ActionStepHistory.Fields.ActionStepID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.ActionStepID = data.Data;
                    }
                }
                else if (field == ActionStepHistory.Fields.RealMode)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.RealMode = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.RealMode = (data.Data == 1) ? true : false;
                    }
                }
                else if (field == ActionStepHistory.Fields.BeginTime)
                {
                    VariousData<DateTime> dt = WebDBManager.GetDateTimeField(arrResult[index]);

                    if (dt == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.BeginTime = dt.Data;
                }
                else if (field == ActionStepHistory.Fields.EndTime)
                {
                    VariousData<DateTime> dt = WebDBManager.GetDateTimeField(arrResult[index]);

                    if (dt == null)
                    {
                        if (isNullable)
                            model.EndTime = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.EndTime = dt.Data;
                }
                else if (field == ActionStepHistory.Fields.LastAccessedTime)
                {
                    VariousData<DateTime> dt = WebDBManager.GetDateTimeField(arrResult[index]);

                    if (dt == null)
                    {
                        if (isNullable)
                            model.LastAccessedTime = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.LastAccessedTime = dt.Data;
                }
                else if (field == ActionStepHistory.Fields.DetectEndTime)
                {
                    VariousData<DateTime> dt = WebDBManager.GetDateTimeField(arrResult[index]);

                    if (dt == null)
                    {
                        if (isNullable)
                            model.DetectEndTime = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.DetectEndTime = dt.Data;
                }
                else if (field == ActionStepHistory.Fields.DetectTime)
                {
                    VariousData<DateTime> dt = WebDBManager.GetDateTimeField(arrResult[index]);

                    if (dt == null)
                    {
                        if (isNullable)
                            model.DetectTime = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.DetectTime = dt.Data;
                }
                else if (field == ActionStepHistory.Fields.Position)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.Position = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Position = data;
                }
                else if (field == ActionStepHistory.Fields.LastAccessedUserID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.LastAccessedUserID = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.LastAccessedUserID = data.Data;
                    }
                }
                else if (field == ActionStepHistory.Fields.StartOption)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.StartOption = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.StartOption = data.Data;
                    }
                }
                else if (field == ActionStepHistory.Fields.DisasterOption)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.DisasterOption = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.DisasterOption = data;
                }
                else if (field == ActionStepHistory.Fields.SensorZoneHistoryID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.SensorZoneHistoryID = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.SensorZoneHistoryID = data.Data;
                    }
                }
                else if (field == ActionStepHistory.Fields.Description)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.Description = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Description = data;
                }
                index++;
            }

            return model;
        }

        private ActionStep ReadActionStep(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            ActionStep model = new ActionStep();
            bool isNullable;

            foreach (ActionStep.Fields field in ActionStep.Fields.GetValues(typeof(ActionStep.Fields)))
            {
                string strFieldName = ActionStep.GetFieldName(field, out isNullable);

                if (field == ActionStep.Fields.ID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.ID = data.Data;
                    }
                }
                else if (field == ActionStep.Fields.StepName)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.StepName = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.StepName = data;
                }
                else if (field == ActionStep.Fields.DisasterID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.DisasterID = data.Data;
                    }
                }
                else if (field == ActionStep.Fields.UserDefinedConfigID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.UserDefinedConfigID = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.UserDefinedConfigID = data.Data;
                    }
                }                
                index++;
            }

            return model;
        }

        private ComponentHistory ReadComponentHistory(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            ComponentHistory model = new ComponentHistory();
            bool isNullable;

            foreach (ComponentHistory.Fields field in ComponentHistory.Fields.GetValues(typeof(ComponentHistory.Fields)))
            {
                string strFieldName = ComponentHistory.GetFieldName(field, out isNullable);

                if (field == ComponentHistory.Fields.ID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.ID = data.Data;
                    }
                }
                else if (field == ComponentHistory.Fields.ActionStepHistoryID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.ActionStepHistoryID = data.Data;
                    }
                }
                else if (field == ComponentHistory.Fields.ComponentID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.ComponentID = data.Data;
                    }
                }
                else if (field == ComponentHistory.Fields.ComponentType)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.ComponentType = data.Data;
                    }
                }
                else if (field == ComponentHistory.Fields.Time)
                {
                    VariousData<DateTime> dt = WebDBManager.GetDateTimeField(arrResult[index]);

                    if (dt == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.Time = dt.Data;
                }
                else if (field == ComponentHistory.Fields.Status)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.ComponentType = data.Data;
                    }
                }
                else if (field == ComponentHistory.Fields.Task)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.Task = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Task = data;
                }
                else if (field == ComponentHistory.Fields.CompleteCount)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.CompleteCount = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.CompleteCount = data.Data;
                    }
                }
                else if (field == ComponentHistory.Fields.ShowBoard)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.ShowBoard = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.ShowBoard = (data.Data == 0) ? false : true;
                    }
                }
                else if (field == ComponentHistory.Fields.AccessedUserID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.AccessedUserID = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.AccessedUserID = data.Data;
                    }
                }
                else if (field == ComponentHistory.Fields.CheckedNotify1)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.CheckedNotify1 = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.CheckedNotify1 = data.Data;
                    }
                }
                else if (field == ComponentHistory.Fields.CheckedNotify2)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.CheckedNotify2 = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.CheckedNotify2 = data.Data;
                    }
                }
                else if (field == ComponentHistory.Fields.CheckedRun)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.CheckedRun = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.CheckedRun = data.Data;
                    }
                }
                else if (field == ComponentHistory.Fields.CheckedComplete)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.CheckedComplete = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.CheckedComplete = data.Data;
                    }
                }
                else if (field == ComponentHistory.Fields.Description)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.Description = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Description = data;
                }
                index++;
            }             

            return model;
        }
    }
}
