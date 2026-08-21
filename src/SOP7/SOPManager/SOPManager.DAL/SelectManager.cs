using System.Collections.Generic;
using System.Collections;

namespace SOPManager.DAL
{   
    using Model.Sop.Category;
    using Model.Sop.Component;
    using Model.Sop.Account;
    using Model.Sop.Config;
    using IDAL;
    using dnsDBUtil;
    using System.Collections;
    using System.Reflection;
    using System.Linq;
    using System;
    using Common.Model.History;
    using System.Text;

    /// <summary>
    /// 쿼리가 성공하면 strErrorMessage가 null이 된다.
    /// strErrorMessage가 null이 아니면 뭔가 문제가 생긴 것이다.
    /// </summary>
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

        public Level SelectLevel(int id, out string strErrorMessage)
        {
            return (Level)SelectDataFromID<Level, Level.Fields>(id, Level.TableName, Level.GetFieldName, Level.Fields.ID, new Level(), out strErrorMessage);
            /*string tableName = Level.TableName;
            string query = "";
            ArrayList res = null;
            Level ret = null;
            string[] notExistMember = null;
            strErrorMessage = null;

            query = string.Format("select * from {0} where ID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                Level temp = new Level();
                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = temp.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var resultList = m_dbManager.SetParamsWithColumnInfo(info, temp, fields, res, out notExistMember);

                if (resultList.Count == 1)
                {
                    temp = resultList[0] as Level;
                    ret = temp;
                }
                else if (resultList.Count > 1)
                {
                    // Not Defined
                    strErrorMessage = "Query Result is wrong !";
                }
                else
                {
                    // Not Defined
                }
            }
            else
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return ret;*/
        }

        public List<Level> SelectLevels(Dictionary<Level.Fields, object> dicConditions, out string strErrorMessage)
        {
            return SelectLevels(dicConditions, null, null, out strErrorMessage);
        }

        public List<Level> SelectLevels(Dictionary<Level.Fields, object> dicConditions, string strAddtionalConditions, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<Level, Level.Fields>(dicConditions, strAddtionalConditions, Level.TableName, Level.GetFieldName, new Level(), topNCount, out strErrorMessage);
        }

        private object SelectDataFromID<DataType, EnumType>(int id, string strTableName, GetFieldNameMethod<EnumType> getFieldName, EnumType idType, DataType model, out string strErrorMessage, out Dictionary<string, string> tableInfo, out ArrayList arrDataResult)
        {
            tableInfo = null;
            arrDataResult = null;

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

            tableInfo = m_dbManager.GetColumnInfoDictionary(strTableName);
            var fields = GetProperties<DataType>();

            Dictionary<string, int> dicFieldIndex;
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

        private object SelectDataFromID<DataType, EnumType>(int id, string strTableName, GetFieldNameMethod<EnumType> getFieldName, EnumType idType, DataType model, out string strErrorMessage)
        {
            Dictionary<string, string> tableInfo;
            ArrayList arrDataResult;
            return SelectDataFromID<DataType, EnumType>(id, strTableName, getFieldName, idType, model, out strErrorMessage, out tableInfo, out arrDataResult);
        }

        private List<DataType> SelectDatas<DataType, EnumType>(Dictionary<EnumType, object> dicConditions, string strTableName, GetFieldNameMethod<EnumType> getFieldName, DataType model, int? topNCount, out string strErrorMessage, out Dictionary<string, string> tableInfo, out ArrayList arrDataResult)
        {
            strErrorMessage = null;
            tableInfo = null;
            arrDataResult = null;
            string strCondition = "";

            if (SetCondition<EnumType>(ref strCondition, dicConditions, getFieldName, strTableName, ref strErrorMessage) == false)
                return null;

            return SelectDatas<DataType, EnumType>(strCondition, strTableName, model, topNCount, out strErrorMessage, out tableInfo, out arrDataResult);
        }

        private List<DataType> SelectDatas<DataType, EnumType>(Dictionary<EnumType, object> dicConditions, string strTableName, GetFieldNameMethod<EnumType> getFieldName, DataType model, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<EnumType>(ref strCondition, dicConditions, getFieldName, strTableName, ref strErrorMessage) == false)
                return null;

            return SelectDatas<DataType, EnumType>(strCondition, strTableName, model, topNCount, out strErrorMessage);
        }

        private List<DataType> SelectDatas<DataType, EnumType>(Dictionary<EnumType, object> dicConditions, string strAdditionalConditions, string strTableName, GetFieldNameMethod<EnumType> getFieldName, DataType model, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<EnumType>(ref strCondition, dicConditions, getFieldName, strTableName, ref strErrorMessage) == false)
                return null;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition == null || strCondition.Length == 0)
                    strCondition = strAdditionalConditions;
                else
                    strCondition += " and " + strAdditionalConditions;
            }

            return SelectDatas<DataType, EnumType>(strCondition, strTableName, model, topNCount, out strErrorMessage);
        }

        private List<DataType> SelectDatas<DataType, EnumType>(string strCondition, string strTableName, DataType model, int? topNCount, out string strErrorMessage, out Dictionary<string, string> tableInfo, out ArrayList arrDataResult)
        {
            strErrorMessage = null;
            tableInfo = null;
            arrDataResult = null;

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

            if (m_dicTableInfos.TryGetValue(strTableName, out tableInfo) == false)
            {
                tableInfo = m_dbManager.GetColumnInfoDictionary(strTableName);
                m_dicTableInfos[strTableName] = tableInfo;
            }

            var fields = GetProperties<DataType>();

            Dictionary<string, int> dicFieldIndex;
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

        private List<DataType> SelectDatas<DataType, EnumType>(string strCondition, string strTableName, DataType model, int? topNCount, out string strErrorMessage)
        {
            Dictionary<string, string> tableInfo;
            ArrayList arrDataResult;
            return SelectDatas<DataType, EnumType>(strCondition, strTableName, model, topNCount, out strErrorMessage, out tableInfo, out arrDataResult);
        }

        public User SelectUser(int id, out string strErrorMessage)
        {
            return (User)SelectDataFromID<User, User.Fields>(id, User.TableName, User.GetFieldName, User.Fields.ID, new User(), out strErrorMessage);
            /*string tableName = User.TableName;
            string query = "";
            ArrayList res = null;
            User ret = null;
            string[] notExistMember = null;
            strErrorMessage = null;

            query = string.Format("select * from {0} where ID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                User temp = new User();
                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = temp.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var resultList = m_dbManager.SetParamsWithColumnInfo(info, temp, fields, res, out notExistMember);

                if (resultList.Count == 1)
                {
                    temp = resultList[0] as User;
                    ret = temp;
                }
                else if (resultList.Count > 1)
                {
                    // Not Defined
                    strErrorMessage = "Query Result is wrong !";
                }
                else
                {
                    // Not Defined
                }
            }
            else
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return ret;*/
        }

        public List<User> SelectUsers(Dictionary<User.Fields, object> dicConditions, out string strErrorMessage)
        {
            return SelectUsers(dicConditions, null, out strErrorMessage);
        }

        public List<User> SelectUsers(Dictionary<User.Fields, object> dicConditions, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<User, User.Fields>(dicConditions, User.TableName, User.GetFieldName, new User(), topNCount, out strErrorMessage);
        }

        public List<User> SelectUsers(string strCondition, out string strErrorMessage)
        {
            return SelectUsers(strCondition, null, out strErrorMessage);
        }

        public List<User> SelectUsers(string strCondition, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<User, User.Fields>(strCondition, User.TableName, new User(), topNCount, out strErrorMessage);
        }

        public Option SelectOption(int id, out string strErrorMessage)
        {
            return (Option)SelectDataFromID<Option, Option.Fields>(id, Option.TableName, Option.GetFieldName, Option.Fields.ID, new Option(), out strErrorMessage);
        }

        public List<Option> SelectOptions(Dictionary<Option.Fields, object> dicConditions, out string strErrorMessage)
        {
            return SelectOptions(dicConditions, null, null, out strErrorMessage);
        }

        public List<Option> SelectOptions(Dictionary<Option.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<Option, Option.Fields>(dicConditions, strAdditionalConditions, Option.TableName, Option.GetFieldName, new Option(), topNCount, out strErrorMessage);
        }

        public ActionStep SelectActionStep(int id, out string strErrorMessage)
        {
            return (ActionStep)SelectDataFromID<ActionStep, ActionStep.Fields>(id, ActionStep.TableName, ActionStep.GetFieldName, ActionStep.Fields.ID, new ActionStep(), out strErrorMessage);
            /*string tableName = ActionStep.TableName;
            string query = "";
            ArrayList res = null;
            ActionStep ret = null;
            string[] notExistMember = null;
            strErrorMessage = null;

            query = string.Format("select * from {0} where ID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                ActionStep temp = new ActionStep();
                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = temp.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var resultList = m_dbManager.SetParamsWithColumnInfo(info, temp, fields, res, out notExistMember);

                if (resultList.Count == 1)
                {
                    temp = resultList[0] as ActionStep;
                    ret = temp;
                }
                else if (resultList.Count > 1)
                {
                    // Not Defined
                    strErrorMessage = "Query Result is wrong !";
                }
                else
                {
                    // Not Defined
                }
            }
            else
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return ret;*/
        }

        // strCondition : where를 제외한 조건문
        public List<ActionStep> SelectActionSteps(string strCondition, out string strErrorMessage)
        {
            return SelectActionSteps(strCondition, null, out strErrorMessage);
        }

        public List<ActionStep> SelectActionSteps(string strCondition, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<ActionStep, ActionStep.Fields>(strCondition, ActionStep.TableName, new ActionStep(), topNCount, out strErrorMessage);
        }

        public List<ActionStep> SelectActionSteps(Dictionary<ActionStep.Fields, object> dicConditions, out string strErrorMessage)
        {
            return SelectActionSteps(dicConditions, null, out strErrorMessage);
        }

        public List<ActionStep> SelectActionSteps(Dictionary<ActionStep.Fields, object> dicConditions, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<ActionStep, ActionStep.Fields>(dicConditions, ActionStep.TableName, ActionStep.GetFieldName, new ActionStep(), topNCount, out strErrorMessage);
        }

        public List<ActionStep> SelectActionSteps(Disaster disaster, out string strErrorMessage)
        {
            Dictionary<ActionStep.Fields, object> dicConditions = new Dictionary<ActionStep.Fields, object>();
            dicConditions[ActionStep.Fields.DisasterID] = disaster.ID;
            return SelectDatas<ActionStep, ActionStep.Fields>(dicConditions, ActionStep.TableName, ActionStep.GetFieldName, new ActionStep(), null, out strErrorMessage);
        }

        public Annotation SelectAnnotation(int id, out string strErrorMessage)
        {
            return (Annotation)SelectDataFromID<Annotation, Annotation.Fields>(id, Annotation.TableName, Annotation.GetFieldName, Annotation.Fields.ID, new Annotation(), out strErrorMessage);
            /*string tableName = Annotation.TableName;
            string query = "";
            ArrayList res = null;
            Annotation ret = null;
            string[] notExistMember = null;
            strErrorMessage = null;

            query = string.Format("select * from {0} where ID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                Annotation temp = new Annotation();
                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = temp.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var resultList = m_dbManager.SetParamsWithColumnInfo(info, temp, fields, res, out notExistMember);

                if (resultList.Count == 1)
                {
                    temp = resultList[0] as Annotation;
                    ret = temp;
                }
                else if (resultList.Count > 1)
                {
                    // Not Defined
                    strErrorMessage = "Query Result is wrong !";
                }
                else
                {
                    // Not Defined
                }
            }
            else
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return ret;*/
        }

        // strCondition : where를 제외한 조건문
        public List<Annotation> SelectAnnotations(string strCondition, out string strErrorMessage)
        {
            return SelectAnnotations(strCondition, null, out strErrorMessage);
        }

        public List<Annotation> SelectAnnotations(string strCondition, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<Annotation, Annotation.Fields>(strCondition, Annotation.TableName, new Annotation(), topNCount, out strErrorMessage);
        }

        public List<Annotation> SelectAnnotations(Dictionary<Annotation.Fields, object> dicConditions, out string strErrorMessage)
        {
            return SelectAnnotations(dicConditions, null, out strErrorMessage);
        }

        public List<Annotation> SelectAnnotations(Dictionary<Annotation.Fields, object> dicConditions, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<Annotation, Annotation.Fields>(dicConditions, Annotation.TableName, Annotation.GetFieldName, new Annotation(), topNCount, out strErrorMessage);
        }

        public List<Annotation> SelectAnnotations(int stepMemberID, out string strErrorMessage)
        {
            Dictionary<Annotation.Fields, object> dicConditions = new Dictionary<Annotation.Fields, object>();
            dicConditions[Annotation.Fields.StepMemberID] = stepMemberID;

            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<Annotation.Fields>(ref strCondition, dicConditions, Annotation.GetFieldName, Annotation.TableName, ref strErrorMessage) == false)
                return null;

            return SelectAnnotations(strCondition, out strErrorMessage);
        }

        public Arrow SelectArrow(int id, out string strErrorMessage)
        {
            return (Arrow)SelectDataFromID<Arrow, Arrow.Fields>(id, Arrow.TableName, Arrow.GetFieldName, Arrow.Fields.ID, new Arrow(), out strErrorMessage);
            /*string tableName = Arrow.TableName;
            string query = "";
            ArrayList res = null;
            Arrow ret = null;
            string[] notExistMember = null;
            strErrorMessage = null;

            query = string.Format("select * from {0} where ID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                Arrow temp = new Arrow();
                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = temp.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var resultList = m_dbManager.SetParamsWithColumnInfo(info, temp, fields, res, out notExistMember);

                if (resultList.Count == 1)
                {
                    temp = resultList[0] as Arrow;
                    ret = temp;
                }
                else if (resultList.Count > 1)
                {
                    // Not Defined
                    strErrorMessage = "Query Result is wrong !";
                }
                else
                {
                    // Not Defined
                }
            }
            else
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return ret;*/
        }

        // strCondition : where를 제외한 조건문
        public List<Arrow> SelectArrows(string strCondition, out string strErrorMessage)
        {
            return SelectArrows(strCondition, null, out strErrorMessage);
        }

        public List<Arrow> SelectArrows(string strCondition, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<Arrow, Arrow.Fields>(strCondition, Arrow.TableName, new Arrow(), topNCount, out strErrorMessage);
        }

        public List<Arrow> SelectArrows(Dictionary<Arrow.Fields, object> dicConditions, out string strErrorMessage)
        {
            return SelectArrows(dicConditions, null, out strErrorMessage);
        }

        public List<Arrow> SelectArrows(Dictionary<Arrow.Fields, object> dicConditions, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<Arrow, Arrow.Fields>(dicConditions, Arrow.TableName, Arrow.GetFieldName, new Arrow(), topNCount, out strErrorMessage);
        }

        public List<Arrow> SelectArrows(int stepMemberID, out string strErrorMessage)
        {
            Dictionary<Arrow.Fields, object> dicConditions = new Dictionary<Arrow.Fields, object>();
            dicConditions[Arrow.Fields.StepMemberID] = stepMemberID;

            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<Arrow.Fields>(ref strCondition, dicConditions, Arrow.GetFieldName, Arrow.TableName, ref strErrorMessage) == false)
                return null;

            return SelectArrows(strCondition, out strErrorMessage);
        }

        public Decision SelectDecision(int id, out string strErrorMessage)
        {
            return (Decision)SelectDataFromID<Decision, Decision.Fields>(id, Decision.TableName, Decision.GetFieldName, Decision.Fields.ID, new Decision(), out strErrorMessage);
            /*string tableName = Decision.TableName;
            string query = "";
            ArrayList res = null;
            Decision ret = null;
            string[] notExistMember = null;
            strErrorMessage = null;

            query = string.Format("select * from {0} where ID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                Decision temp = new Decision();
                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = temp.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var resultList = m_dbManager.SetParamsWithColumnInfo(info, temp, fields, res, out notExistMember);

                if (resultList.Count == 1)
                {
                    temp = resultList[0] as Decision;
                    ret = temp;
                }
                else if (resultList.Count > 1)
                {
                    // Not Defined
                    strErrorMessage = "Query Result is wrong !";
                }
                else
                {
                    // Not Defined
                }
            }
            else
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return ret;*/
        }

        // strCondition : where를 제외한 조건문
        public List<Decision> SelectDecisions(string strCondition, out string strErrorMessage)
        {
            return SelectDecisions(strCondition, null, out strErrorMessage);
        }

        public List<Decision> SelectDecisions(string strCondition, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<Decision, Decision.Fields>(strCondition, Decision.TableName, new Decision(), topNCount, out strErrorMessage);
        }

        public List<Decision> SelectDecisions(Dictionary<Decision.Fields, object> dicConditions, out string strErrorMessage)
        {
            return SelectDecisions(dicConditions, null, out strErrorMessage);
        }

        public List<Decision> SelectDecisions(Dictionary<Decision.Fields, object> dicConditions, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<Decision, Decision.Fields>(dicConditions, Decision.TableName, Decision.GetFieldName, new Decision(), topNCount, out strErrorMessage);
        }

        public List<Decision> SelectDecisions(int stepMemberID, out string strErrorMessage)
        {
            Dictionary<Decision.Fields, object> dicConditions = new Dictionary<Decision.Fields, object>();
            dicConditions[Decision.Fields.StepMemberID] = stepMemberID;

            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<Decision.Fields>(ref strCondition, dicConditions, Decision.GetFieldName, Decision.TableName, ref strErrorMessage) == false)
                return null;

            return SelectDecisions(strCondition, out strErrorMessage);
        }

        public Disaster SelectDisaster(int id, out string strErrorMessage)
        {
            string tableName = Disaster.TableName;
            string query = "";
            ArrayList res = null;
            Disaster ret = null;
            string[] notExistMember = null;
            strErrorMessage = null;

            query = string.Format("select * from {0} where ID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                Disaster temp = new Disaster();

                Dictionary<string, string> tableInfo;

                if (m_dicTableInfos.TryGetValue(tableName, out tableInfo) == false)
                {
                    tableInfo = m_dbManager.GetColumnInfoDictionary(tableName);
                    m_dicTableInfos[tableName] = tableInfo;
                }

                var fields = temp.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                List<object> resultList = m_dbManager.SetParamsWithColumnInfo(tableInfo, temp, fields, res, out notExistMember);

                if (resultList.Count == 1)
                {
                    temp = resultList[0] as Disaster;

                    if (res[4] != null)
                    {
                        temp.UserLevelIDs = res[4].ToString().Split(',').Select(Int32.Parse).ToList();
                    }

                    ret = temp;
                }
                else if (resultList.Count > 1)
                {
                    // Not Defined
                    strErrorMessage = "Query Result is wrong !";
                }
                else
                {
                    // Not Defined
                }
            }
            else
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return ret;
            //return (Disaster)SelectDataFromID<Disaster, Disaster.Fields>(id, Disaster.TableName, Disaster.GetFieldName, Disaster.Fields.ID, new Disaster(), out strErrorMessage);
        }

        // strCondition : where를 제외한 조건문
        public List<Disaster> SelectDisasters(string strCondition, out string strErrorMessage)
        {
            return SelectDisasters(strCondition, null, out strErrorMessage);
        }

        public List<Disaster> SelectDisasters(string strCondition, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            Dictionary<string, string> tableInfo = null;
            ArrayList arrDataResult = null;

            int nFieldCount;
            string strTableName = Disaster.TableName;
            string strFields = GetFieldNames<Disaster.Fields>(strTableName, out nFieldCount);

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

            if (m_dicTableInfos.TryGetValue(strTableName, out tableInfo) == false)
            {
                tableInfo = m_dbManager.GetColumnInfoDictionary(strTableName);
                m_dicTableInfos[strTableName] = tableInfo;
            }

            var fields = GetProperties<Disaster>();

            Dictionary<string, int> dicFieldIndex;
            List<string> fieldNames = GetFieldNameIndex<Disaster.Fields>(out dicFieldIndex);
            string[] notExistMember;

            strErrorMessage = null;
            int nResultCount = arrResult.Count;

            List<Disaster> results = new List<Disaster>();

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

            List<object> datas = m_dbManager.SetParamsWithColumnInfo(tableInfo, new Disaster(), fields, arrDataResult, out notExistMember);

            if (datas == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            int nDataCount = datas.Count;

            for (int i=0;i<nDataCount;i++)
            {
                Disaster disaster = (Disaster)datas[i];
                object UserLevelIDs = arrDataResult[nFieldCount * i + 4];

                if (UserLevelIDs != null)
                    disaster.UserLevelIDs = StringToIntList(UserLevelIDs.ToString());

                results.Add(disaster);
            }

            return results;
            //return SelectDatas<Disaster, Disaster.Fields>(strCondition, Disaster.TableName, new Disaster(), topNCount, out strErrorMessage);
        }

        public List<Disaster> SelectDisasters(Dictionary<Disaster.Fields, object> dicConditions, out string strErrorMessage)
        {
            return SelectDisasters(dicConditions, null, out strErrorMessage);
        }

        public List<Disaster> SelectDisasters(Dictionary<Disaster.Fields, object> dicConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<Disaster.Fields>(ref strCondition, dicConditions, Disaster.GetFieldName, Disaster.TableName, ref strErrorMessage) == false)
                return null;

            return SelectDisasters(strCondition, topNCount, out strErrorMessage);
        }

        // check (CreateTime 기준으로)
        // Key : Disaster Name
        // Value : Disaster의 버전이 최신것부터 정렬
        public Dictionary<string, List<Disaster>> SelectDisasters(SubDisasterCategory subDisasterCategory, bool isNormal, out string strErrorMessage)
        {
            int nFieldCount;
            string strTableName = Disaster.TableName;
            string strFields = GetFieldNames<Disaster.Fields>(strTableName, out nFieldCount);

            bool isNullable;

            string strSQL = string.Format("Select {0} from {1}, {2} where {1}.{3} = {4} and {1}.{5} = {2}.{6} and {2}.{7} = {8} order by {2}.{9} desc",
                strFields,
                strTableName,
                Model.Sop.Category.Version.TableName,
                Disaster.GetFieldName(Disaster.Fields.SubDisasterCategoryID, out isNullable),
                subDisasterCategory.ID,
                Disaster.GetFieldName(Disaster.Fields.VersionID, out isNullable),
                Model.Sop.Category.Version.GetFieldName(Model.Sop.Category.Version.Fields.ID, out isNullable),
                Model.Sop.Category.Version.GetFieldName(Model.Sop.Category.Version.Fields.IsNormal, out isNullable),
                isNormal ? 1 : 0,
                Model.Sop.Category.Version.GetFieldName(Model.Sop.Category.Version.Fields.CreateTime, out isNullable));

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

            var fields = GetProperties<Disaster>();

            Dictionary<string, int> dicFieldIndex;
            List<string> fieldNames = GetFieldNameIndex<Disaster.Fields>(out dicFieldIndex);
            string[] notExistMember;

            strErrorMessage = null;
            int nResultCount = arrResult.Count;

            Dictionary<string, List<Disaster>> dicResults = new Dictionary<string, List<Disaster>>();

            if (nResultCount < nFieldCount)
                return dicResults;

            ArrayList arrDataResult = SortWithProperties(arrResult, ref fields, fieldNames, dicFieldIndex);
            int nUserLevelIndex = fieldNames.IndexOf(Disaster.GetFieldName(Disaster.Fields.UserLevelIDs, out isNullable).ToLower());

            if (arrDataResult == null)
            {
                strErrorMessage = "Query Result is wrong !";
                return null;
            }

            List<object> datas = m_dbManager.SetParamsWithColumnInfo(tableInfo, new Disaster(), fields, arrDataResult, out notExistMember);

            if (datas == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            if (datas.Count > 0 && nUserLevelIndex >= 0)
            {
                List<Disaster> tempDisasters = new List<Disaster>();

                for (int i = 0; i < datas.Count; i++)
                {
                    Disaster temp = (Disaster)datas[i];
                    
                    if (arrDataResult[nUserLevelIndex + (i * nFieldCount)] != null)
                    {
                        temp.UserLevelIDs = arrDataResult[nUserLevelIndex + (i * nFieldCount)].ToString().Split(',').Select(Int32.Parse).ToList();
                    }

                    List<Disaster> disasters;

                    if (dicResults.TryGetValue(temp.DisasterName, out disasters) == false)
                    {
                        disasters = new List<Disaster>();
                        dicResults[temp.DisasterName] = disasters;
                    }

                    disasters.Add(temp);
                }
            }

            return dicResults;
            /*string tableName = Disaster.TableName;
            string query = "";
            ArrayList res = null;
            Dictionary<string, List<Disaster>> ret = null;
            string[] notExistMember = null;
            strErrorMessage = null;

            string[] colInfoArr = m_dbManager.GetColumnNameStringArray(tableName);
            for (int i = 0; i < colInfoArr.Length; i++)
            {
                colInfoArr[i] = colInfoArr[i].Insert(0, "dis.");
            }
            string colInfoStr = string.Join(", ", colInfoArr);

            query = string.Format("select {0} from {1} dis, {2} ver " +
                                  "where SubDisasterCategoryID = {3} and dis.VersionID = ver.ID and ver.isNormal = {4}" +
                                  "order by ver.CreateTime desc", colInfoStr, tableName, Model.Sop.Category.Version.TableName, subDisasterCategory.ID, isNormal ? 1 : 0);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                Disaster temp = new Disaster();
                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = temp.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var resultList = m_dbManager.SetParamsWithColumnInfo(info, temp, fields, res, out notExistMember);

                if (resultList.Count > 0)
                {
                    ret = new Dictionary<string, List<Disaster>>();
                    List<Disaster> listTemp = new List<Disaster>();
                    for (int i = 0; i < resultList.Count; i++)
                    {
                        temp = resultList[i] as Disaster;
                        if (res[4 + (i * info.Count)] != null)
                        {
                            temp.UserLevelIDs = res[4 + (i * info.Count)].ToString().Split(',').Select(Int32.Parse).ToList();
                        }

                        if (ret.Count == 0)
                        {
                            ret.Add(temp.DisasterName, new List<Disaster> { temp });
                        }
                        else
                        {
                            if (ret.ContainsKey(temp.DisasterName))
                            {
                                ret[temp.DisasterName].Add(temp);
                            }
                            else
                            {
                                ret.Add(temp.DisasterName, new List<Disaster> { temp });
                            }
                        }
                    }
                }
                else if (resultList.Count == 0)
                {
                    // 결과 없을 경우
                    ret = new Dictionary<string, List<Disaster>>();
                }
                else
                {
                    // Not Defined
                }
            }
            else
            {
                // Not Defined
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return ret;*/
        }

        public DisasterType SelectDisasterType(int id, out string strErrorMessage)
        {
            return (DisasterType)SelectDataFromID<DisasterType, DisasterType.Fields>(id, DisasterType.TableName, DisasterType.GetFieldName, DisasterType.Fields.ID, new DisasterType(), out strErrorMessage);
            /*string tableName = DisasterType.TableName;
            string query = "";
            ArrayList res = null;
            DisasterType ret = null;
            string[] notExistMember = null;
            strErrorMessage = null;

            query = string.Format("select * from {0} where ID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                DisasterType temp = new DisasterType();
                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = temp.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var resultList = m_dbManager.SetParamsWithColumnInfo(info, temp, fields, res, out notExistMember);

                if (resultList.Count == 1)
                {
                    temp = resultList[0] as DisasterType;
                    ret = temp;
                }
                else if (resultList.Count > 1)
                {
                    // Not Defined
                    strErrorMessage = "Query Result is wrong !";
                }
                else
                {
                    // Not Defined
                }
            }
            else
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return ret;*/
        }

        // ID가 큰 순으로 정렬
        public List<DisasterType> SelectDisasterTypes(string strCondition, out string strErrorMessage)
        {
            return SelectDisasterTypes(strCondition, null, out strErrorMessage);
        }

        public List<DisasterType> SelectDisasterTypes(string strCondition, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<DisasterType, DisasterType.Fields>(strCondition, DisasterType.TableName, new DisasterType(), topNCount, out strErrorMessage);
        }

        public List<DisasterType> SelectDisasterTypes(Dictionary<DisasterType.Fields, object> dicConditions, out string strErrorMessage)
        {
            return SelectDisasterTypes(dicConditions, null, out strErrorMessage);
        }

        public List<DisasterType> SelectDisasterTypes(Dictionary<DisasterType.Fields, object> dicConditions, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<DisasterType, DisasterType.Fields>(dicConditions, DisasterType.TableName, DisasterType.GetFieldName, new DisasterType(), topNCount, out strErrorMessage);
        }

        public DisasterCategory SelectDisasterCategory(int id, out string strErrorMessage)
        {
            return (DisasterCategory)SelectDataFromID<DisasterCategory, DisasterCategory.Fields>(id, DisasterCategory.TableName, DisasterCategory.GetFieldName, DisasterCategory.Fields.ID, new DisasterCategory(), out strErrorMessage);
            /*string tableName = DisasterCategory.TableName;
            string query = "";
            ArrayList res = null;
            DisasterCategory ret = null;
            string[] notExistMember = null;
            strErrorMessage = null;

            query = string.Format("select * from {0} where ID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                DisasterCategory temp = new DisasterCategory();
                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = temp.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var resultList = m_dbManager.SetParamsWithColumnInfo(info, temp, fields, res, out notExistMember);

                if (resultList.Count == 1)
                {
                    temp = resultList[0] as DisasterCategory;
                    ret = temp;
                }
                else if (resultList.Count > 1)
                {
                    // Not Defined
                    strErrorMessage = "Query Result is wrong !";
                }
                else
                {
                    // Not Defined
                }
            }
            else
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return ret;*/
        }

        // strCondition : where를 제외한 조건문
        public List<DisasterCategory> SelectDisasterCategories(string strCondition, out string strErrorMessage)
        {
            return SelectDisasterCategories(strCondition, null, out strErrorMessage);
        }

        public List<DisasterCategory> SelectDisasterCategories(string strCondition, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<DisasterCategory, DisasterCategory.Fields>(strCondition, DisasterCategory.TableName, new DisasterCategory(), topNCount, out strErrorMessage);
        }

        public List<DisasterCategory> SelectDisasterCategories(Dictionary<DisasterCategory.Fields, object> dicConditions, out string strErrorMessage)
        {
            return SelectDisasterCategories(dicConditions, null, out strErrorMessage);
        }

        public List<DisasterCategory> SelectDisasterCategories(Dictionary<DisasterCategory.Fields, object> dicConditions, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<DisasterCategory, DisasterCategory.Fields>(dicConditions, DisasterCategory.TableName, DisasterCategory.GetFieldName, new DisasterCategory(), topNCount, out strErrorMessage);
        }

        public List<DisasterCategory> SelectDisasterCategories(out string strErrorMessage)
        {
            return SelectDisasterCategories((string)null, null, out strErrorMessage);
        }

        public EndPoint SelectEndPoint(int id, out string strErrorMessage)
        {
            return (EndPoint)SelectDataFromID<EndPoint, EndPoint.Fields>(id, EndPoint.TableName, EndPoint.GetFieldName, EndPoint.Fields.ID, new EndPoint(), out strErrorMessage);
            /*string tableName = EndPoint.TableName;
            string query = "";
            ArrayList res = null;
            EndPoint ret = null;
            string[] notExistMember = null;
            strErrorMessage = null;

            query = string.Format("select * from {0} where ID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                EndPoint temp = new EndPoint();
                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = temp.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var resultList = m_dbManager.SetParamsWithColumnInfo(info, temp, fields, res, out notExistMember);

                if (resultList.Count == 1)
                {
                    temp = resultList[0] as EndPoint;
                    ret = temp;
                }
                else if (resultList.Count > 1)
                {
                    // Not Defined
                    strErrorMessage = "Query Result is wrong !";
                }
                else
                {
                    // Not Defined
                }
            }
            else
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return ret;*/
        }

        // strCondition : where를 제외한 조건문
        public List<EndPoint> SelectEndPoints(string strCondition, out string strErrorMessage)
        {
            return SelectEndPoints(strCondition, null, out strErrorMessage);
        }

        public List<EndPoint> SelectEndPoints(string strCondition, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<EndPoint, EndPoint.Fields>(strCondition, EndPoint.TableName, new EndPoint(), topNCount, out strErrorMessage);
        }

        public List<EndPoint> SelectEndPoints(Dictionary<EndPoint.Fields, object> dicConditions, out string strErrorMessage)
        {
            return SelectEndPoints(dicConditions, null, out strErrorMessage);
        }

        public List<EndPoint> SelectEndPoints(Dictionary<EndPoint.Fields, object> dicConditions, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<EndPoint, EndPoint.Fields>(dicConditions, EndPoint.TableName, EndPoint.GetFieldName, new EndPoint(), topNCount, out strErrorMessage);
        }

        public List<EndPoint> SelectEndPoints(int stepMemberID, out string strErrorMessage)
        {
            Dictionary<EndPoint.Fields, object> dicConditions = new Dictionary<EndPoint.Fields, object>();
            dicConditions[EndPoint.Fields.StepMemberID] = stepMemberID;

            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<EndPoint.Fields>(ref strCondition, dicConditions, EndPoint.GetFieldName, EndPoint.TableName, ref strErrorMessage) == false)
                return null;

            return SelectEndPoints(strCondition, out strErrorMessage);
        }

        public ExternalProgram SelectExternalProgram(int id, out string strErrorMessage)
        {
            return (ExternalProgram)SelectDataFromID<ExternalProgram, ExternalProgram.Fields>(id, ExternalProgram.TableName, ExternalProgram.GetFieldName, ExternalProgram.Fields.ID, new ExternalProgram(), out strErrorMessage);
        }

        public List<ExternalProgram> SelectExternalPrograms(string strCondition, out string strErrorMessage)
        {
            return SelectExternalPrograms(strCondition, null, out strErrorMessage);
        }

        public List<ExternalProgram> SelectExternalPrograms(string strCondition, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<ExternalProgram, ExternalProgram.Fields>(strCondition, ExternalProgram.TableName, new ExternalProgram(), topNCount, out strErrorMessage);
        }

        public List<ExternalProgram> SelectExternalPrograms(Dictionary<ExternalProgram.Fields, object> dicConditions, out string strErrorMessage)
        {
            return SelectExternalPrograms(dicConditions, null, out strErrorMessage);
        }

        public List<ExternalProgram> SelectExternalPrograms(Dictionary<ExternalProgram.Fields, object> dicConditions, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<ExternalProgram, ExternalProgram.Fields>(dicConditions, ExternalProgram.TableName, ExternalProgram.GetFieldName, new ExternalProgram(), topNCount, out strErrorMessage);
        }

        public ExternalProgramParameter SelectExternalProgramParameter(int nProgramID, int nParameterIndex, out string strErrorMessage)
        {
            Dictionary<ExternalProgramParameter.Fields, object> dicConditions = new Dictionary<ExternalProgramParameter.Fields, object>();
            dicConditions[ExternalProgramParameter.Fields.ProgramID] = nProgramID;
            dicConditions[ExternalProgramParameter.Fields.ParameterIndex] = nParameterIndex;

            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<ExternalProgramParameter.Fields>(ref strCondition, dicConditions, ExternalProgramParameter.GetFieldName, ExternalProgramParameter.TableName, ref strErrorMessage) == false)
                return null;

            List<ExternalProgramParameter> parameters = SelectExternalProgramParameters(strCondition, out strErrorMessage);

            if (parameters == null || parameters.Count == 0)
                return null;

            return parameters[0];
        }

        public List<ExternalProgramParameter> SelectExternalProgramParameters(int nProgramID, out string strErrorMessage)
        {
            Dictionary<ExternalProgramParameter.Fields, object> dicConditions = new Dictionary<ExternalProgramParameter.Fields, object>();
            dicConditions[ExternalProgramParameter.Fields.ProgramID] = nProgramID;
            
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<ExternalProgramParameter.Fields>(ref strCondition, dicConditions, ExternalProgramParameter.GetFieldName, ExternalProgramParameter.TableName, ref strErrorMessage) == false)
                return null;

            return SelectExternalProgramParameters(strCondition, out strErrorMessage);
        }

        public List<ExternalProgramParameter> SelectExternalProgramParameters(string strCondition, out string strErrorMessage)
        {
            return SelectExternalProgramParameters(strCondition, null, out strErrorMessage);
        }

        public List<ExternalProgramParameter> SelectExternalProgramParameters(string strCondition, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<ExternalProgramParameter, ExternalProgramParameter.Fields>(strCondition, ExternalProgramParameter.TableName, new ExternalProgramParameter(), topNCount, out strErrorMessage);
        }

        public List<ExternalProgramParameter> SelectExternalProgramParameters(Dictionary<ExternalProgramParameter.Fields, object> dicConditions, out string strErrorMessage)
        {
            return SelectExternalProgramParameters(dicConditions, null, out strErrorMessage);
        }

        public List<ExternalProgramParameter> SelectExternalProgramParameters(Dictionary<ExternalProgramParameter.Fields, object> dicConditions, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<ExternalProgramParameter, ExternalProgramParameter.Fields>(dicConditions, ExternalProgramParameter.TableName, ExternalProgramParameter.GetFieldName, new ExternalProgramParameter(), topNCount, out strErrorMessage);
        }

        public SectionGrid SelectGrid(int id, out string strErrorMessage)
        {
            return (SectionGrid)SelectDataFromID<SectionGrid, SectionGrid.Fields>(id, SectionGrid.TableName, SectionGrid.GetFieldName, SectionGrid.Fields.ID, new SectionGrid(), out strErrorMessage);
            /*string tableName = SectionGrid.TableName;
            string query = "";
            ArrayList res = null;
            SectionGrid ret = null;
            string[] notExistMember = null;
            strErrorMessage = null;

            query = string.Format("select * from {0} where ID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                SectionGrid temp = new SectionGrid();
                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = temp.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var resultList = m_dbManager.SetParamsWithColumnInfo(info, temp, fields, res, out notExistMember);

                if (resultList.Count == 1)
                {
                    temp = resultList[0] as SectionGrid;
                    ret = temp;
                }
                else if (resultList.Count > 1)
                {
                    // Not Defined
                    strErrorMessage = "Query Result is wrong !";
                }
                else
                {
                    // Not Defined
                }
            }
            else
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return ret;*/
        }

        // strCondition : where를 제외한 조건문
        public List<SectionGrid> SelectGrids(string strCondition, out string strErrorMessage)
        {
            return SelectGrids(strCondition, null, out strErrorMessage);
        }

        public List<SectionGrid> SelectGrids(string strCondition, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<SectionGrid, SectionGrid.Fields>(strCondition, SectionGrid.TableName, new SectionGrid(), topNCount, out strErrorMessage);
        }

        public List<SectionGrid> SelectGrids(Dictionary<SectionGrid.Fields, object> dicConditions, out string strErrorMessage)
        {
            return SelectGrids(dicConditions, null, out strErrorMessage);
        }

        public List<SectionGrid> SelectGrids(Dictionary<SectionGrid.Fields, object> dicConditions, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<SectionGrid, SectionGrid.Fields>(dicConditions, SectionGrid.TableName, SectionGrid.GetFieldName, new SectionGrid(), topNCount, out strErrorMessage);
        }

        public SectionGridColumn SelectGridColumn(int gridID, int columnIndex, out string strErrorMessage)
        {
            Dictionary<SectionGridColumn.Fields, object> dicConditions = new Dictionary<SectionGridColumn.Fields, object>();
            dicConditions[SectionGridColumn.Fields.GridID] = gridID;
            dicConditions[SectionGridColumn.Fields.ColumnIndex] = columnIndex;

            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<SectionGridColumn.Fields>(ref strCondition, dicConditions, SectionGridColumn.GetFieldName, SectionGridColumn.TableName, ref strErrorMessage) == false)
                return null;

            List<SectionGridColumn> columns = SelectGridColumns(strCondition, out strErrorMessage);

            if (columns == null)
                return null;

            if (columns.Count == 0)
                return null;

            return (SectionGridColumn)columns[0];
            /*string tableName = SectionGridColumn.TableName;
            string query = "";
            ArrayList res = null;
            SectionGridColumn ret = null;
            string[] notExistMember = null;
            strErrorMessage = null;

            query = string.Format("select * from {0} where GridID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                SectionGridColumn temp = new SectionGridColumn();
                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = temp.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var resultList = m_dbManager.SetParamsWithColumnInfo(info, temp, fields, res, out notExistMember);

                if (resultList.Count == 1)
                {
                    temp = resultList[0] as SectionGridColumn;
                    ret = temp;
                }
                else if (resultList.Count > 1)
                {
                    // Not Defined
                    strErrorMessage = "Query Result is wrong !";
                }
                else
                {
                    // Not Defined
                }
            }
            else
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return ret;*/
        }

        // strCondition : where를 제외한 조건문
        public List<SectionGridColumn> SelectGridColumns(string strCondition, out string strErrorMessage)
        {
            return SelectGridColumns(strCondition, null, out strErrorMessage);
        }

        public List<SectionGridColumn> SelectGridColumns(string strCondition, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<SectionGridColumn, SectionGridColumn.Fields>(strCondition, SectionGridColumn.TableName, new SectionGridColumn(), topNCount, out strErrorMessage);
        }

        public List<SectionGridColumn> SelectGridColumns(Dictionary<SectionGridColumn.Fields, object> dicConditions, out string strErrorMessage)
        {
            return SelectGridColumns(dicConditions, null, out strErrorMessage);
        }

        public List<SectionGridColumn> SelectGridColumns(Dictionary<SectionGridColumn.Fields, object> dicConditions, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<SectionGridColumn, SectionGridColumn.Fields>(dicConditions, SectionGridColumn.TableName, SectionGridColumn.GetFieldName, new SectionGridColumn(), topNCount, out strErrorMessage);
        }

        public SectionGridRow SelectGridRow(int gridID, int rowIndex, out string strErrorMessage)
        {
            Dictionary<SectionGridRow.Fields, object> dicConditions = new Dictionary<SectionGridRow.Fields, object>();
            dicConditions[SectionGridRow.Fields.GridID] = gridID;
            dicConditions[SectionGridRow.Fields.RowIndex] = rowIndex;

            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<SectionGridRow.Fields>(ref strCondition, dicConditions, SectionGridRow.GetFieldName, SectionGridRow.TableName, ref strErrorMessage) == false)
                return null;

            List<SectionGridRow> rows = SelectGridRows(strCondition, out strErrorMessage);

            if (rows == null)
                return null;

            if (rows.Count == 0)
                return null;

            return (SectionGridRow)rows[0];
            /*string tableName = SectionGridRow.TableName;
            string query = "";
            ArrayList res = null;
            SectionGridRow ret = null;
            string[] notExistMember = null;
            strErrorMessage = null;

            query = string.Format("select * from {0} where GridID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                SectionGridRow temp = new SectionGridRow();
                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = temp.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var resultList = m_dbManager.SetParamsWithColumnInfo(info, temp, fields, res, out notExistMember);

                if (resultList.Count == 1)
                {
                    temp = resultList[0] as SectionGridRow;
                    ret = temp;
                }
                else if (resultList.Count > 1)
                {
                    // Not Defined
                    strErrorMessage = "Query Result is wrong !";
                }
                else
                {
                    // Not Defined
                }
            }
            else
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return ret;*/
        }

        // strCondition : where를 제외한 조건문
        public List<SectionGridRow> SelectGridRows(string strCondition, out string strErrorMessage)
        {
            return SelectGridRows(strCondition, null, out strErrorMessage);
        }

        public List<SectionGridRow> SelectGridRows(string strCondition, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<SectionGridRow, SectionGridRow.Fields>(strCondition, SectionGridRow.TableName, new SectionGridRow(), topNCount, out strErrorMessage);
        }

        public List<SectionGridRow> SelectGridRows(Dictionary<SectionGridRow.Fields, object> dicConditions, out string strErrorMessage)
        {
            return SelectGridRows(dicConditions, null, out strErrorMessage);
        }

        public List<SectionGridRow> SelectGridRows(Dictionary<SectionGridRow.Fields, object> dicConditions, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<SectionGridRow, SectionGridRow.Fields>(dicConditions, SectionGridRow.TableName, SectionGridRow.GetFieldName, new SectionGridRow(), topNCount, out strErrorMessage);
        }

        private InternalTransmission ReadInternalTransmission(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            InternalTransmission model = new InternalTransmission();
            bool isNullable;

            foreach (InternalTransmission.Fields field in InternalTransmission.Fields.GetValues(typeof(InternalTransmission.Fields)))
            {
                string strFieldName = InternalTransmission.GetFieldName(field, out isNullable);

                if (field == InternalTransmission.Fields.ID)
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
                else if (field == InternalTransmission.Fields.GridID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.GridID = data.Data;
                }
                else if (field == InternalTransmission.Fields.GridRowIndex)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.GridRowIndex = data.Data;
                }
                else if (field == InternalTransmission.Fields.GridColumnIndex)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.GridColumnIndex = data.Data;
                }
                else if (field == InternalTransmission.Fields.Width)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.Width = data.Data;
                }
                else if (field == InternalTransmission.Fields.Height)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.Height = data.Data;
                }
                else if (field == InternalTransmission.Fields.Text)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.Text = data;
                }
                else if (field == InternalTransmission.Fields.UseSMS)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.UseSMS = data.Data == 1;
                }
                else if (field == InternalTransmission.Fields.UseBroadcast)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.UseBroadcast = data.Data == 1;
                }
                else if (field == InternalTransmission.Fields.UseEmail)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.UseEmail = null;
                    }
                    else
                        model.UseEmail = data.Data == 1;
                }
                else if (field == InternalTransmission.Fields.Message)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.Message = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Message = data;
                }
                else if (field == InternalTransmission.Fields.TeamList)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable == false)
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        AddReceiverList(model.TeamList, data);
                }
                else if (field == InternalTransmission.Fields.ComponentID)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.ComponentID = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.ComponentID = data;
                }
                else if (field == InternalTransmission.Fields.OnlyTeamLeader)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.OnlyTeamLeader = null;
                    }
                    else
                        model.OnlyTeamLeader = data.Data == 1;
                }
                else if (field == InternalTransmission.Fields.StepMemberID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.StepMemberID = data.Data;
                }
                else if (field == InternalTransmission.Fields.VAlign)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.VAlign = null;
                    }
                    else
                        model.VAlign = data.Data;
                }
                else if (field == InternalTransmission.Fields.HAlign)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.HAlign = null;
                    }
                    else
                        model.HAlign = data.Data;
                }
                else if (field == InternalTransmission.Fields.FontName)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.FontName = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.FontName = data;
                }
                else if (field == InternalTransmission.Fields.FontStyle)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.FontStyle = null;
                    }
                    else
                        model.FontStyle = data.Data;
                }
                else if (field == InternalTransmission.Fields.FontSize)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.FontSize = null;
                    }
                    else
                        model.FontSize = data.Data;
                }
                else if (field == InternalTransmission.Fields.LineSpace)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.LineSpace = null;
                    }
                    else
                        model.LineSpace = data.Data;
                }
                else if (field == InternalTransmission.Fields.FontColor)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.FontColor = null;
                    }
                    else
                        model.FontColor = data.Data;
                }
                else if (field == InternalTransmission.Fields.AutoRun)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.AutoRun = data.Data == 1;
                }
                else if (field == InternalTransmission.Fields.UseSiren)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.UseSiren = null;
                    }
                    else
                        model.UseSiren = data.Data == 1;
                }
                else if (field == InternalTransmission.Fields.SectionNumber)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.SectionNumber = null;
                    }
                    else
                        model.SectionNumber = data.Data;
                }

                index++;
            }

            return model;
        }

        public InternalTransmission SelectInternalTransmission(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<InternalTransmission.Fields>(out nFieldCount), InternalTransmission.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                InternalTransmission model = ReadInternalTransmission(arrResult, 0, out strErrorMessage);

                if (model == null)
                    return null;

                return model;
            }
            else
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
            /*Dictionary<string, string> tableInfo;
            ArrayList arrDataResult;
            InternalTransmission _internal = (InternalTransmission)SelectDataFromID<InternalTransmission, InternalTransmission.Fields>(id, InternalTransmission.TableName, InternalTransmission.GetFieldName, InternalTransmission.Fields.ID, new InternalTransmission(), out strErrorMessage, out tableInfo, out arrDataResult);

            if (_internal == null)
                return null;

            List<InternalTransmission> internals = new List<InternalTransmission>();
            internals.Add(_internal);
            GetInternalTransmissions(internals, tableInfo, arrDataResult);

            return _internal;*/
        }

        // strCondition : where를 제외한 조건문
        public List<InternalTransmission> SelectInternalTransmissions(string strCondition, out string strErrorMessage)
        {
            return SelectInternalTransmissions(strCondition, null, out strErrorMessage);
        }

        public List<InternalTransmission> SelectInternalTransmissions(string strCondition, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<InternalTransmission.Fields>(out nFieldCount), InternalTransmission.TableName);

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

            int nResultCount = arrResult.Count;
            List<InternalTransmission> internals = new List<InternalTransmission>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                InternalTransmission model = ReadInternalTransmission(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    internals.Add(model);
            }

            return internals;
        }

        public List<InternalTransmission> SelectInternalTransmissions(Dictionary<InternalTransmission.Fields, object> dicConditions, out string strErrorMessage)
        {
            return SelectInternalTransmissions(dicConditions, null, out strErrorMessage);
        }

        public List<InternalTransmission> SelectInternalTransmissions(Dictionary<InternalTransmission.Fields, object> dicConditions, int? topNCount, out string strErrorMessage)
        {
            string strCondition = "";
            strErrorMessage = null;

            if (SetCondition<InternalTransmission.Fields>(ref strCondition, dicConditions, InternalTransmission.GetFieldName, InternalTransmission.TableName, ref strErrorMessage) == false)
                return null;

            return SelectInternalTransmissions(strCondition, topNCount, out strErrorMessage);
        }

        public List<InternalTransmission> SelectInternalTransmissions(int stepMemberID, out string strErrorMessage)
        {
            Dictionary<InternalTransmission.Fields, object> dicConditions = new Dictionary<InternalTransmission.Fields, object>();
            dicConditions[InternalTransmission.Fields.StepMemberID] = stepMemberID;

            return SelectInternalTransmissions(dicConditions, out strErrorMessage);
        }

        /*private List<InternalTransmission> GetInternalTransmissions(List<InternalTransmission> internals, Dictionary<string, string> tableInfo, ArrayList arrDataResult)
        {
            if (internals == null)
                return null;

            bool isNullable;
            string strTeamListFieldName = InternalTransmission.GetFieldName(InternalTransmission.Fields.TeamList, out isNullable);

            int nIndex = -1;
            int fieldCount = tableInfo.Count;

            for (int i=0;i<fieldCount;i++)
            {
                KeyValuePair<string, string> pair = tableInfo.ElementAt(i);

                if (pair.Key == strTeamListFieldName)
                {
                    nIndex = i;
                    break;
                }
            }

            if (nIndex < 0)
                return internals;

            int nInternalCount = internals.Count;

            for (int i=0;i<nInternalCount;i++)
            {
                if (arrDataResult[i * fieldCount + nIndex] == null)
                    continue;

                string strTeamList = arrDataResult[i * fieldCount + nIndex].ToString();
                List<KeyValuePair<int, int>> teamList = StringToTeamList(strTeamList);

                InternalTransmission _internal = internals[i];
                _internal.TeamList.AddRange(teamList);
            }

            return internals;
        }*/

        /*private List<KeyValuePair<int, int>> StringToTeamList(string strTeamList)
        {
            List<KeyValuePair<int, int>> teamList = new List<KeyValuePair<int, int>>();

            int nTeamType, nTeamID;
            string[] tokens = strTeamList.Trim().Split(',');

            foreach (string strToken in tokens)
            {
                int nIndex1 = strToken.IndexOf('(');
                int nIndex2 = strToken.LastIndexOf(')');

                if (nIndex1 > 0 && nIndex2 > nIndex1)
                {
                    string strTeamType = strToken.Substring(0, nIndex1).Trim();
                    string strTeamID = strToken.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1).Trim();

                    if (int.TryParse(strTeamType, out nTeamType) && int.TryParse(strTeamID, out nTeamID))
                    {
                        teamList.Add(new KeyValuePair<int, int>(nTeamType, nTeamID));
                    }
                }
            }

            return teamList;
        }*/

        public Link SelectLink(int id, out string strErrorMessage)
        {
            return (Link)SelectDataFromID<Link, Link.Fields>(id, Link.TableName, Link.GetFieldName, Link.Fields.ID, new Link(), out strErrorMessage);
            /*string tableName = Link.TableName;
            string query = "";
            ArrayList res = null;
            Link ret = null;
            string[] notExistMember = null;
            strErrorMessage = null;

            query = string.Format("select * from {0} where ID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                Link temp = new Link();
                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = temp.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var resultList = m_dbManager.SetParamsWithColumnInfo(info, temp, fields, res, out notExistMember);

                if (resultList.Count == 1)
                {
                    temp = resultList[0] as Link;

                    if (res[8] != null)
                    {
                        string[] splitStr = { ";", " " };
                        List<string> linkedTemp = res[8].ToString().Split(splitStr, StringSplitOptions.RemoveEmptyEntries).ToList();
                        for (int i = 0; i < linkedTemp.Count; i++)
                        {
                            temp.LinkedComponentIDList.Add(linkedTemp[i]);
                        }
                    }

                    ret = temp;
                }
                else if (resultList.Count > 1)
                {
                    // Not Defined
                    strErrorMessage = "Query Result is wrong !";
                }
                else
                {
                    // Not Defined
                }
            }
            else
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return ret;*/
        }

        // strCondition : where를 제외한 조건문
        public List<Link> SelectLinks(string strCondition, out string strErrorMessage)
        {
            return SelectLinks(strCondition, null, out strErrorMessage);
        }

        public List<Link> SelectLinks(string strCondition, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<Link, Link.Fields>(strCondition, Link.TableName, new Link(), topNCount, out strErrorMessage);
        }

        public List<Link> SelectLinks(Dictionary<Link.Fields, object> dicConditions, out string strErrorMessage)
        {
            return SelectLinks(dicConditions, null, out strErrorMessage);
        }

        public List<Link> SelectLinks(Dictionary<Link.Fields, object> dicConditions, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<Link, Link.Fields>(dicConditions, Link.TableName, Link.GetFieldName, new Link(), topNCount, out strErrorMessage);
        }

        public List<Link> SelectLinks(int stepMemberID, out string strErrorMessage)
        {
            Dictionary<Link.Fields, object> dicConditions = new Dictionary<Link.Fields, object>();
            dicConditions[Link.Fields.StepMemberID] = stepMemberID;

            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<Link.Fields>(ref strCondition, dicConditions, Link.GetFieldName, Link.TableName, ref strErrorMessage) == false)
                return null;

            return SelectLinks(strCondition, out strErrorMessage);
        }

        private Process ReadProcess(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            Process model = new Process();
            bool isNullable;

            foreach (Process.Fields field in Process.Fields.GetValues(typeof(Process.Fields)))
            {
                string strFieldName = Process.GetFieldName(field, out isNullable);

                if (field == Process.Fields.ID)
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
                else if (field == Process.Fields.GridID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.GridID = data.Data;
                }
                else if (field == Process.Fields.GridRowIndex)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.GridRowIndex = data.Data;
                }
                else if (field == Process.Fields.GridColumnIndex)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.GridColumnIndex = data.Data;
                }
                else if (field == Process.Fields.Width)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.Width = data.Data;
                }
                else if (field == Process.Fields.Height)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.Height = data.Data;
                }
                else if (field == Process.Fields.Text)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.Text = data;
                }
                else if (field == Process.Fields.TeamList)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable == false)
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        AddReceiverList(model.TeamList, data);
                }
                else if (field == Process.Fields.ComponentID)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.ComponentID = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.ComponentID = data;
                }
                else if (field == Process.Fields.OnlyTeamLeader)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.OnlyTeamLeader = null;
                    }
                    else
                        model.OnlyTeamLeader = data.Data == 1;
                }
                else if (field == Process.Fields.StepMemberID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.StepMemberID = data.Data;
                }
                else if (field == Process.Fields.VAlign)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.VAlign = null;
                    }
                    else
                        model.VAlign = data.Data;
                }
                else if (field == Process.Fields.HAlign)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.HAlign = null;
                    }
                    else
                        model.HAlign = data.Data;
                }
                else if (field == Process.Fields.FontName)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.FontName = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.FontName = data;
                }
                else if (field == Process.Fields.FontStyle)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.FontStyle = null;
                    }
                    else
                        model.FontStyle = data.Data;
                }
                else if (field == Process.Fields.FontSize)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.FontSize = null;
                    }
                    else
                        model.FontSize = data.Data;
                }
                else if (field == Process.Fields.LineSpace)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.LineSpace = null;
                    }
                    else
                        model.LineSpace = data.Data;
                }
                else if (field == Process.Fields.FontColor)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.FontColor = null;
                    }
                    else
                        model.FontColor = data.Data;
                }
                else if (field == Process.Fields.AutoRun)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.AutoRun = data.Data == 1;
                }
                else if (field == Process.Fields.SectionNumber)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.SectionNumber = null;
                    }
                    else
                        model.SectionNumber = data.Data;
                }

                index++;
            }

            return model;
        }

        public Process SelectProcess(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<Process.Fields>(out nFieldCount), Process.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                Process model = ReadProcess(arrResult, 0, out strErrorMessage);

                if (model == null)
                    return null;

                return model;
            }
            else
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
            //return (Process)SelectDataFromID<Process, Process.Fields>(id, Process.TableName, Process.GetFieldName, Process.Fields.ID, new Process(), out strErrorMessage);
        }

        // strCondition : where를 제외한 조건문
        public List<Process> SelectProcesses(string strCondition, out string strErrorMessage)
        {
            return SelectProcesses(strCondition, null, out strErrorMessage);
        }

        public List<Process> SelectProcesses(string strCondition, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<Process.Fields>(out nFieldCount), Process.TableName);

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

            int nResultCount = arrResult.Count;
            List<Process> processes = new List<Process>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                Process model = ReadProcess(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    processes.Add(model);
            }

            return processes;
        }

        private void AddReceiverList(List<Receiver> receiverList, string strTeamList)
        {
            if (strTeamList == null || strTeamList.Length == 0)
                return;

            int nTeamID, nTeamType;
            string[] tokens = strTeamList.Split(',');

            foreach (string strToken in tokens)
            {
                int nIndex1 = strToken.IndexOf('(');
                int nIndex2 = strToken.IndexOf(')');

                if (nIndex1 < 0 || nIndex2 <= nIndex1)
                    continue;

                string strTeamID = strToken.Substring(0, nIndex1).Trim();
                string strTeamType = strToken.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1).Trim();

                if (int.TryParse(strTeamID, out nTeamID) && int.TryParse(strTeamType, out nTeamType))
                {
                    receiverList.Add(new Receiver(nTeamType, nTeamID));
                }
            }
        }

        /*private List<KeyValuePair<int, int>> GetTeamList(string strTeamList)
        {
            int nTeamID, nTeamType;
            string[] tokens = strTeamList.Split(',');
            List<KeyValuePair<int, int>> teamList = new List<KeyValuePair<int, int>>();

            foreach (string strToken in tokens)
            {
                int nIndex1 = strToken.IndexOf('(');
                int nIndex2 = strToken.IndexOf(')');

                if (nIndex1 < 0 || nIndex2 < nIndex1)
                    continue;

                string strTeamID = strToken.Substring(0, nIndex1).Trim();
                string strTeamType = strToken.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1).Trim();

                if (int.TryParse(strTeamID, out nTeamID) && int.TryParse(strTeamType, out nTeamType))
                {
                    teamList.Add(new KeyValuePair<int, int>(nTeamType, nTeamID));
                }
            }

            return teamList;
        }*/

        public List<Process> SelectProcesses(Dictionary<Process.Fields, object> dicConditions, out string strErrorMessage)
        {
            return SelectProcesses(dicConditions, null, out strErrorMessage);
        }

        public List<Process> SelectProcesses(Dictionary<Process.Fields, object> dicConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<Process.Fields>(out nFieldCount), Process.TableName);

            string strCondition = "";

            if (SetCondition<Process.Fields>(ref strCondition, dicConditions, Process.GetFieldName, Process.TableName, ref strErrorMessage) == false)
                return null;

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
            List<Process> processes = new List<Process>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                Process model = ReadProcess(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    processes.Add(model);
            }

            return processes;
        }

        public List<Process> SelectProcesses(int stepMemberID, out string strErrorMessage)
        {
            Dictionary<Process.Fields, object> dicConditions = new Dictionary<Process.Fields, object>();
            dicConditions[Process.Fields.StepMemberID] = stepMemberID;

            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<Process.Fields>(ref strCondition, dicConditions, Process.GetFieldName, Process.TableName, ref strErrorMessage) == false)
                return null;

            return SelectProcesses(strCondition, out strErrorMessage);
        }

        public ProcessMission SelectProcessMission(int id, out string strErrorMessage)
        {
            return (ProcessMission)SelectDataFromID<ProcessMission, ProcessMission.Fields>(id, ProcessMission.TableName, ProcessMission.GetFieldName, ProcessMission.Fields.ID, new ProcessMission(), out strErrorMessage);
            /*string tableName = ProcessMission.TableName;
            string query = "";
            ArrayList res = null;
            ProcessMission ret = null;
            string[] notExistMember = null;
            strErrorMessage = null;

            query = string.Format("select * from {0} where ID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                ProcessMission temp = new ProcessMission();
                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = temp.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var resultList = m_dbManager.SetParamsWithColumnInfo(info, temp, fields, res, out notExistMember);

                if (resultList.Count == 1)
                {
                    temp = resultList[0] as ProcessMission;
                    ret = temp;
                }
                else if (resultList.Count > 1)
                {
                    // Not Defined
                    strErrorMessage = "Query Result is wrong !";
                }
                else
                {
                    // Not Defined
                }
            }
            else
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return ret;*/
        }

        // strCondition : where를 제외한 조건문
        public List<ProcessMission> SelectProcessMissions(string strCondition, out string strErrorMessage)
        {
            return SelectProcessMissions(strCondition, null, out strErrorMessage);
        }

        public List<ProcessMission> SelectProcessMissions(string strCondition, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<ProcessMission, ProcessMission.Fields>(strCondition, ProcessMission.TableName, new ProcessMission(), topNCount, out strErrorMessage);
        }

        public List<ProcessMission> SelectProcessMissions(Dictionary<ProcessMission.Fields, object> dicConditions, out string strErrorMessage)
        {
            return SelectProcessMissions(dicConditions, null, out strErrorMessage);
        }

        public List<ProcessMission> SelectProcessMissions(Dictionary<ProcessMission.Fields, object> dicConditions, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<ProcessMission, ProcessMission.Fields>(dicConditions, ProcessMission.TableName, ProcessMission.GetFieldName, new ProcessMission(), topNCount, out strErrorMessage);
        }

        public List<ProcessMission> SelectProcessMissions(List<int> processIDs, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strProcessIDs = ListToString(processIDs);

            if (strProcessIDs.Length == 0)
                return new List<ProcessMission>();

            bool isNullable;
            string strCondition = string.Format("{0} in ({1})", ProcessMission.GetFieldName(ProcessMission.Fields.ProcessID, out isNullable), strProcessIDs);
            
            return SelectProcessMissions(strCondition, out strErrorMessage);
        }

        public ProcessExternalMission SelectProcessExternalMission(int nProcessID, int nOrderIndex, int nProgramID, int nParameterIndex, out string strErrorMessage)
        {
            Dictionary<ProcessExternalMission.Fields, object> dicConditions = new Dictionary<ProcessExternalMission.Fields, object>();
            dicConditions[ProcessExternalMission.Fields.ProcessID] = nProcessID;
            dicConditions[ProcessExternalMission.Fields.OrderIndex] = nOrderIndex;
            dicConditions[ProcessExternalMission.Fields.ProgramID] = nProgramID;
            dicConditions[ProcessExternalMission.Fields.ParameterIndex] = nParameterIndex;

            List<ProcessExternalMission> missions = SelectProcessExternalMissions(dicConditions, out strErrorMessage);

            if (missions == null || missions.Count == 0)
                return null;

            return missions[0];
        }

        // strCondition : where를 제외한 조건문
        public List<ProcessExternalMission> SelectProcessExternalMissions(string strCondition, out string strErrorMessage)
        {
            return SelectProcessExternalMissions(strCondition, null, out strErrorMessage);
        }

        public List<ProcessExternalMission> SelectProcessExternalMissions(string strCondition, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<ProcessExternalMission, ProcessExternalMission.Fields>(strCondition, ProcessExternalMission.TableName, new ProcessExternalMission(), topNCount, out strErrorMessage);
        }

        public List<ProcessExternalMission> SelectProcessExternalMissions(Dictionary<ProcessExternalMission.Fields, object> dicConditions, out string strErrorMessage)
        {
            return SelectProcessExternalMissions(dicConditions, null, out strErrorMessage);
        }

        public List<ProcessExternalMission> SelectProcessExternalMissions(Dictionary<ProcessExternalMission.Fields, object> dicConditions, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<ProcessExternalMission, ProcessExternalMission.Fields>(dicConditions, ProcessExternalMission.TableName, ProcessExternalMission.GetFieldName, new ProcessExternalMission(), topNCount, out strErrorMessage);
        }

        public List<ProcessExternalMission> SelectProcessExternalMissions(List<int> processIDs, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strProcessIDs = ListToString(processIDs);

            if (strProcessIDs.Length == 0)
                return new List<ProcessExternalMission>();

            bool isNullable;
            string strCondition = string.Format("{0} in ({1})", ProcessExternalMission.GetFieldName(ProcessExternalMission.Fields.ProcessID, out isNullable), strProcessIDs);

            return SelectProcessExternalMissions(strCondition, out strErrorMessage);
        }

        public StepMember SelectStepMember(int id, out string strErrorMessage)
        {
            return (StepMember)SelectDataFromID<StepMember, StepMember.Fields>(id, StepMember.TableName, StepMember.GetFieldName, StepMember.Fields.ID, new StepMember(), out strErrorMessage);
            /*string tableName = StepMember.TableName;
            string query = "";
            ArrayList res = null;
            StepMember ret = null;
            string[] notExistMember = null;
            strErrorMessage = null;

            query = string.Format("select * from {0} where ID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                StepMember temp = new StepMember();
                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = temp.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var resultList = m_dbManager.SetParamsWithColumnInfo(info, temp, fields, res, out notExistMember);

                if (resultList.Count == 1)
                {
                    temp = resultList[0] as StepMember;
                    ret = temp;
                }
                else if (resultList.Count > 1)
                {
                    // Not Defined
                    strErrorMessage = "Query Result is wrong !";
                }
                else
                {
                    // Not Defined
                }
            }
            else
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return ret;*/
        }

        // strCondition : where를 제외한 조건문
        public List<StepMember> SelectStepMembers(string strCondition, out string strErrorMessage)
        {
            return SelectStepMembers(strCondition, null, out strErrorMessage);
        }

        public List<StepMember> SelectStepMembers(string strCondition, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<StepMember, StepMember.Fields>(strCondition, StepMember.TableName, new StepMember(), topNCount, out strErrorMessage);
        }

        public List<StepMember> SelectStepMembers(Dictionary<StepMember.Fields, object> dicConditions, out string strErrorMessage)
        {
            return SelectStepMembers(dicConditions, null, out strErrorMessage);
        }

        public List<StepMember> SelectStepMembers(Dictionary<StepMember.Fields, object> dicConditions, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<StepMember, StepMember.Fields>(dicConditions, StepMember.TableName, StepMember.GetFieldName, new StepMember(), topNCount, out strErrorMessage);
        }

        public List<StepMember> SelectStepMembers(ActionStep actionStep, out string strErrorMessage)
        {
            Dictionary<StepMember.Fields, object> dicConditions = new Dictionary<StepMember.Fields, object>();
            dicConditions[StepMember.Fields.ActionStepID] = actionStep.ID;
            return SelectDatas<StepMember, StepMember.Fields>(dicConditions, StepMember.TableName, StepMember.GetFieldName, new StepMember(), null, out strErrorMessage);
            /*string tableName = StepMember.TableName;
            string query = "";
            ArrayList res = null;
            List<StepMember> ret = null;
            string[] notExistMember = null;
            strErrorMessage = null;

            query = string.Format("select * from {0} where ActionStepID = {1}", tableName, actionStep.ID);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                StepMember temp = new StepMember();
                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = temp.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var resultList = m_dbManager.SetParamsWithColumnInfo(info, temp, fields, res, out notExistMember);

                if (resultList.Count > 0)
                {
                    ret = new List<StepMember>();
                    for (int i = 0; i < resultList.Count; i++)
                    {
                        ret.Add(resultList[i] as StepMember);
                    }
                }
                else if (resultList.Count == 0)
                {
                    // 결과 없을 경우
                    ret = new List<StepMember>();
                }
                else
                {
                    // Not Defined
                }
            }
            else
            {
                // Not Defined
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return ret;*/
        }

        public SubDisasterCategory SelectSubDisasterCategory(int id, out string strErrorMessage)
        {
            return (SubDisasterCategory)SelectDataFromID<SubDisasterCategory, SubDisasterCategory.Fields>(id, SubDisasterCategory.TableName, SubDisasterCategory.GetFieldName, SubDisasterCategory.Fields.ID, new SubDisasterCategory(), out strErrorMessage);
            /*string tableName = SubDisasterCategory.TableName;
            string query = "";
            ArrayList res = null;
            SubDisasterCategory ret = null;
            string[] notExistMember = null;
            strErrorMessage = null;

            query = string.Format("select * from {0} where ID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                SubDisasterCategory temp = new SubDisasterCategory();
                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = temp.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var resultList = m_dbManager.SetParamsWithColumnInfo(info, temp, fields, res, out notExistMember);

                if (resultList.Count == 1)
                {
                    temp = resultList[0] as SubDisasterCategory;
                    ret = temp;
                }
                else if (resultList.Count > 1)
                {
                    // Not Defined
                    strErrorMessage = "Query Result is wrong !";
                }
                else
                {
                    // Not Defined
                }
            }
            else
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return ret;*/
        }

        // strCondition : where를 제외한 조건문
        public List<SubDisasterCategory> SelectSubDisasterCategories(string strCondition, out string strErrorMessage)
        {
            return SelectSubDisasterCategories(strCondition, null, out strErrorMessage);
        }

        public List<SubDisasterCategory> SelectSubDisasterCategories(string strCondition, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<SubDisasterCategory, SubDisasterCategory.Fields>(strCondition, SubDisasterCategory.TableName, new SubDisasterCategory(), topNCount, out strErrorMessage);
        }

        public List<SubDisasterCategory> SelectSubDisasterCategories(Dictionary<SubDisasterCategory.Fields, object> dicConditions, out string strErrorMessage)
        {
            return SelectSubDisasterCategories(dicConditions, null, out strErrorMessage);
        }

        public List<SubDisasterCategory> SelectSubDisasterCategories(Dictionary<SubDisasterCategory.Fields, object> dicConditions, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<SubDisasterCategory, SubDisasterCategory.Fields>(dicConditions, SubDisasterCategory.TableName, SubDisasterCategory.GetFieldName, new SubDisasterCategory(), topNCount, out strErrorMessage);
        }

        public List<SubDisasterCategory> SelectSubDisasterCategories(DisasterCategory disasterCategory, out string strErrorMessage)
        {
            Dictionary<SubDisasterCategory.Fields, object> dicConditions = new Dictionary<SubDisasterCategory.Fields, object>();
            dicConditions[SubDisasterCategory.Fields.DisasterCategoryID] = disasterCategory.ID;
            return SelectDatas<SubDisasterCategory, SubDisasterCategory.Fields>(dicConditions, SubDisasterCategory.TableName, SubDisasterCategory.GetFieldName, new SubDisasterCategory(), null, out strErrorMessage);
            /*string tableName = SubDisasterCategory.TableName;
            string query = "";
            ArrayList res = null;
            List<SubDisasterCategory> ret = null;
            string[] notExistMember = null;
            strErrorMessage = null;

            query = string.Format("select * from {0} where DisasterCategoryID = {1}", tableName, disasterCategory.ID);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                SubDisasterCategory temp = new SubDisasterCategory();
                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = temp.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var resultList = m_dbManager.SetParamsWithColumnInfo(info, temp, fields, res, out notExistMember);

                if (resultList.Count > 0)
                {
                    ret = new List<SubDisasterCategory>();
                    for (int i = 0; i < resultList.Count; i++)
                    {
                        ret.Add(resultList[i] as SubDisasterCategory);
                    }
                }
                else if (resultList.Count == 0)
                {
                    // 결과 없을 경우
                    ret = new List<SubDisasterCategory>();
                }
                else
                {
                    // Not Defined
                }
            }
            else
            {
                // Not Defined
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return ret;*/
        }

        public Model.Sop.Category.Version SelectVersion(int id, out string strErrorMessage)
        {
            return (Model.Sop.Category.Version)SelectDataFromID<Model.Sop.Category.Version, Model.Sop.Category.Version.Fields>(id, Model.Sop.Category.Version.TableName, Model.Sop.Category.Version.GetFieldName, Model.Sop.Category.Version.Fields.ID, new Model.Sop.Category.Version(), out strErrorMessage);
            /*string tableName = Model.Sop.Category.Version.TableName;
            string query = "";
            ArrayList res = null;
            Model.Sop.Category.Version ret = null;
            string[] notExistMember = null;
            strErrorMessage = null;

            query = string.Format("select * from {0} where ID = {1}", tableName, id);
            res = m_dbManager.GetResultData(query);

            if (res != null)
            {
                Model.Sop.Category.Version temp = new Model.Sop.Category.Version();
                var info = m_dbManager.GetColumnInfoDictionary(tableName);
                var fields = temp.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var resultList = m_dbManager.SetParamsWithColumnInfo(info, temp, fields, res, out notExistMember);

                if (resultList.Count == 1)
                {
                    temp = resultList[0] as Model.Sop.Category.Version;
                    ret = temp;
                }
                else if (resultList.Count > 1)
                {
                    // Not Defined
                    strErrorMessage = "Query Result is wrong !";
                }
                else
                {
                    // Not Defined
                }
            }
            else
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return ret;*/
        }

        // strCondition : where를 제외한 조건문
        public List<Model.Sop.Category.Version> SelectVersions(string strCondition, out string strErrorMessage)
        {
            return SelectVersions(strCondition, null, out strErrorMessage);
        }

        public List<Model.Sop.Category.Version> SelectVersions(string strCondition, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<Model.Sop.Category.Version, Model.Sop.Category.Version.Fields>(strCondition, Model.Sop.Category.Version.TableName, new Model.Sop.Category.Version(), topNCount, out strErrorMessage);
        }

        public List<Model.Sop.Category.Version> SelectVersions(Dictionary<Model.Sop.Category.Version.Fields, object> dicConditions, out string strErrorMessage)
        {
            return SelectVersions(dicConditions, null, out strErrorMessage);
        }

        public List<Model.Sop.Category.Version> SelectVersions(Dictionary<Model.Sop.Category.Version.Fields, object> dicConditions, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<Model.Sop.Category.Version, Model.Sop.Category.Version.Fields>(dicConditions, Model.Sop.Category.Version.TableName, Model.Sop.Category.Version.GetFieldName, new Model.Sop.Category.Version(), topNCount, out strErrorMessage);
        }

        public LinkedSop SelectLinkedSop(int id, out string strErrorMessage)
        {
            return (LinkedSop)SelectDataFromID<LinkedSop, LinkedSop.Fields>(id, LinkedSop.TableName, LinkedSop.GetFieldName, LinkedSop.Fields.ID, new LinkedSop(), out strErrorMessage);
        }

        public List<LinkedSop> SelectLinkedSops(Dictionary<LinkedSop.Fields, object> dicConditions, out string strErrorMessage)
        {
            return SelectLinkedSops(dicConditions, null, null, out strErrorMessage);
        }

        public List<LinkedSop> SelectLinkedSops(Dictionary<LinkedSop.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<LinkedSop, LinkedSop.Fields>(dicConditions, strAdditionalConditions, LinkedSop.TableName, LinkedSop.GetFieldName, new LinkedSop(), topNCount, out strErrorMessage);
        }

        public SpecialMessage SelectSpecialMessage(int id, out string strErrorMessage)
        {
            return (SpecialMessage)SelectDataFromID<SpecialMessage, SpecialMessage.Fields>(id, SpecialMessage.TableName, SpecialMessage.GetFieldName, SpecialMessage.Fields.ID, new SpecialMessage(), out strErrorMessage);
        }

        public List<SpecialMessage> SelectSpecialMessages(Dictionary<SpecialMessage.Fields, object> dicConditions, out string strErrorMessage)
        {
            return SelectSpecialMessages(dicConditions, null, null, out strErrorMessage);
        }

        public List<SpecialMessage> SelectSpecialMessages(Dictionary<SpecialMessage.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<SpecialMessage, SpecialMessage.Fields>(dicConditions, strAdditionalConditions, SpecialMessage.TableName, SpecialMessage.GetFieldName, new SpecialMessage(), topNCount, out strErrorMessage);
        }

        public Session SelectSession(int id, out string strErrorMessage)
        {
            return (Session)SelectDataFromID<Session, Session.Fields>(id, Session.TableName, Session.GetFieldName, Session.Fields.ID, new Session(), out strErrorMessage);
        }

        public List<Session> SelectSessions(Dictionary<Session.Fields, object> dicConditions, out string strErrorMessage)
        {
            return SelectSessions(dicConditions, null, null, out strErrorMessage);
        }

        public List<Session> SelectSessions(Dictionary<Session.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            return SelectDatas<Session, Session.Fields>(dicConditions, strAdditionalConditions, Session.TableName, Session.GetFieldName, new Session(), topNCount, out strErrorMessage);
        }

        // check (Annotation, Decision, Endpotion, Internal, Link, Process)
        // StepMember 안에 있는 전체 SOP 컴포넌트를 받아옴
        /*public bool SelectStepMemberComponents(StepMember stepMember, List<Section> sections, List<Arrow> arrows, out string strErrorMessage)
        {
            string[] tableNames = { Annotation.TableName, Decision.TableName, EndPoint.TableName,
                                    InternalTransmission.TableName, Link.TableName, Process.TableName , Arrow.TableName};
            string[] query = new string[tableNames.Length];
            ArrayList[] res = new ArrayList[tableNames.Length];
            string[][] notExistMember = new string[tableNames.Length][];
            strErrorMessage = null;
            object[] temp = new object[tableNames.Length];
            List<object>[] resultList = new List<object>[tableNames.Length];

            for (int i = 0; i < tableNames.Length; i++)
            {
                query[i] = string.Format("select * from {0} where StepMemberID = {1}", tableNames[i], stepMember.ID);
                res[i] = m_dbManager.GetResultData(query[i]);
                if (res[i] != null)
                {
                    switch (i)
                    {
                        case 0:
                            temp[i] = new Annotation();
                            break;
                        case 1:
                            temp[i] = new Decision();
                            break;
                        case 2:
                            temp[i] = new EndPoint();
                            break;
                        case 3:
                            temp[i] = new InternalTransmission();
                            break;
                        case 4:
                            temp[i] = new Link();
                            break;
                        case 5:
                            temp[i] = new Process();
                            break;
                        case 6:
                            temp[i] = new Arrow();
                            break;
                        default:
                            // Not Defined
                            break;
                    }

                    var info = m_dbManager.GetColumnInfoDictionary(tableNames[i]);
                    var fields = temp[i].GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    resultList[i] = m_dbManager.SetParamsWithColumnInfo(info, temp[i], fields, res[i], out notExistMember[i]);

                    switch (i)
                    {
                        case 0:
                            for (int j = 0; j < resultList[i].Count; j++)
                            {
                                sections.Add(resultList[i][j] as Annotation);
                            }
                            break;
                        case 1:
                            for (int j = 0; j < resultList[i].Count; j++)
                            {
                                sections.Add(resultList[i][j] as Decision);
                            }
                            break;
                        case 2:
                            for (int j = 0; j < resultList[i].Count; j++)
                            {
                                sections.Add(resultList[i][j] as EndPoint);
                            }
                            break;
                        case 3:
                            for (int j = 0; j < resultList[i].Count; j++)
                            {
                                temp[i] = resultList[i][j];

                                if (res[i][12 + (j * info.Count)] != null)
                                {
                                    string[] splitStr = { "(", ")", "," };
                                    List<string> teamTemp = res[i][12 + (j * info.Count)].ToString().Split(splitStr, StringSplitOptions.RemoveEmptyEntries).ToList();

                                    if (teamTemp.Count % 2 == 0)
                                    {
                                        int key = -1, value = -1; // Init Not Defined
                                        bool keyRes = false, valueRes = false;

                                        for (int k = 0; k < teamTemp.Count; k += 2)
                                        {
                                            keyRes = Int32.TryParse(teamTemp[k], out key);
                                            valueRes = Int32.TryParse(teamTemp[k + 1], out value);

                                            if (keyRes && valueRes)
                                            {
                                                ((InternalTransmission)temp[i]).TeamList.Add(new KeyValuePair<int, int>(value, key));
                                            }
                                        }
                                    }
                                }

                                sections.Add(temp[i] as InternalTransmission);
                            }
                            break;
                        case 4:
                            for (int j = 0; j < resultList[i].Count; j++)
                            {
                                temp[i] = resultList[i][j];

                                if (res[i][8 + (j * info.Count)] != null)
                                {
                                    string[] splitStr = { ";", " " };
                                    List<string> linkedTemp = res[i][8 + (j * info.Count)].ToString().Split(splitStr, StringSplitOptions.RemoveEmptyEntries).ToList();
                                    for (int k = 0; k < linkedTemp.Count; k++)
                                    {
                                        ((Link)temp[i]).LinkedComponentIDList.Add(linkedTemp[k]);
                                    }
                                }
                                sections.Add(temp[i] as Link);
                            }
                            break;
                        case 5:
                            for (int j = 0; j < resultList[i].Count; j++)
                            {
                                temp[i] = resultList[i][j];
                                if (res[i][7 + (j * info.Count)] != null)
                                {
                                    string[] splitStr = { "(", ")", "," };
                                    List<string> teamTemp = res[i][7 + (j * info.Count)].ToString().Split(splitStr, StringSplitOptions.RemoveEmptyEntries).ToList();

                                    if (teamTemp.Count % 2 == 0)
                                    {
                                        int key = -1, value = -1; // Init Not Defined
                                        bool keyRes = false, valueRes = false;

                                        for (int k = 0; k < teamTemp.Count; k += 2)
                                        {
                                            keyRes = Int32.TryParse(teamTemp[k], out key);
                                            valueRes = Int32.TryParse(teamTemp[k + 1], out value);

                                            if (keyRes && valueRes)
                                            {
                                                ((Process)temp[i]).TeamList.Add(new KeyValuePair<int, int>(value, key));
                                            }
                                        }
                                    }
                                }
                                sections.Add(temp[i] as Process);
                            }
                            break;
                        case 6:
                            for (int j = 0; j < resultList[i].Count; j++)
                            {
                                arrows.Add(resultList[i][j] as Arrow);
                            }
                            break;
                        default:
                            // Not Defined
                            break;
                    }
                }
                else
                {
                    // 각 항목마다 메시지 따로 저장해야할 듯?
                    strErrorMessage = m_dbManager.LastErrorMessage;
                    return false;
                }
            }

            return true;
        }*/

        // check (전부 다)
        /// <summary>
        /// </summary>
        /// <returns>
        /// Disaster, User, Version의 각 객체가 순서대로 ArrayList에 담겨진다.
        /// 에러가 발생하면 null을 리턴한다.
        /// </returns>
        public ArrayList JoinDisasterUserVersion(string strCondition, out string strErrorMessage)
        {
            return JoinDisasterUserVersion(strCondition, null, out strErrorMessage);
        }

        public ArrayList JoinDisasterUserVersion(string strCondition, int? topNCount, out string strErrorMessage)
        {
            string strDisasterTableName = Disaster.TableName;
            string strVersionTableName = Model.Sop.Category.Version.TableName;
            string strUserTableName = User.TableName;

            int nDisasterFieldCount, nVersionFieldCount, nUserFieldCount;
            string strDisasterFields = GetFieldNames<Disaster.Fields>(strDisasterTableName, out nDisasterFieldCount);
            string strVersionFields = GetFieldNames<Model.Sop.Category.Version.Fields>(strVersionTableName, out nVersionFieldCount);
            string strUserFields = GetFieldNames<User.Fields>(strUserTableName, out nUserFieldCount);

            int nFieldsCount = nDisasterFieldCount + nVersionFieldCount + nUserFieldCount;

            string strSQL = string.Format("Select {0}, {1}, {2} from {3}, {4}, {5} ", strDisasterFields, strUserFields, strVersionFields, strDisasterTableName, strUserTableName, strVersionTableName);

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

            Dictionary<string, string> disasterTableInfo, userTableInfo, versionTableInfo;

            if (m_dicTableInfos.TryGetValue(strDisasterTableName, out disasterTableInfo) == false)
            {
                disasterTableInfo = m_dbManager.GetColumnInfoDictionary(strDisasterTableName);
                m_dicTableInfos[strDisasterTableName] = disasterTableInfo;
            }

            if (m_dicTableInfos.TryGetValue(strUserTableName, out userTableInfo) == false)
            {
                userTableInfo = m_dbManager.GetColumnInfoDictionary(strUserTableName);
                m_dicTableInfos[strUserTableName] = userTableInfo;
            }

            if (m_dicTableInfos.TryGetValue(strVersionTableName, out versionTableInfo) == false)
            {
                versionTableInfo = m_dbManager.GetColumnInfoDictionary(strVersionTableName);
                m_dicTableInfos[strVersionTableName] = versionTableInfo;
            }

            var disasterFields = GetProperties<Disaster>(); ;
            var userFields = GetProperties<User>(); ;
            var versionFields = GetProperties<Model.Sop.Category.Version>();

            Dictionary<string, int> dicDiasterFieldIndex, dicUserFieldIndex, dicVersionFieldIndex;
            List<string> disasterFieldNames = GetFieldNameIndex<Disaster.Fields>(out dicDiasterFieldIndex);
            List<string> userFieldNames = GetFieldNameIndex<User.Fields>(out dicUserFieldIndex);
            List<string> versionFieldNames = GetFieldNameIndex<Model.Sop.Category.Version.Fields>(out dicVersionFieldIndex);
            string[] notExistMember;

            strErrorMessage = null;
            int nResultCount = arrResult.Count;

            ArrayList arrDatas = new ArrayList();

            for (int i = 0; i < nResultCount - (nFieldsCount - 1); i += nFieldsCount)
            {
                ArrayList arrDisasterResult = SortWithProperties(ParseArray(arrResult, i, nDisasterFieldCount), ref disasterFields, disasterFieldNames, dicDiasterFieldIndex);
                ArrayList arrUserResult = SortWithProperties(ParseArray(arrResult, i + nDisasterFieldCount, nUserFieldCount), ref userFields, userFieldNames, dicUserFieldIndex);
                ArrayList arrVersionResult = SortWithProperties(ParseArray(arrResult, i + nDisasterFieldCount + nUserFieldCount, nVersionFieldCount), ref versionFields, versionFieldNames, dicVersionFieldIndex);

                if (arrDisasterResult == null || arrVersionResult == null || arrUserResult == null)
                    return null;

                List<object> disasters = m_dbManager.SetParamsWithColumnInfo(disasterTableInfo, new Disaster(), disasterFields, arrDisasterResult, out notExistMember);
                List<object> users = m_dbManager.SetParamsWithColumnInfo(userTableInfo, new User(), userFields, arrUserResult, out notExistMember);
                List<object> versions = m_dbManager.SetParamsWithColumnInfo(versionTableInfo, new Model.Sop.Category.Version(), versionFields, arrVersionResult, out notExistMember);

                if (disasters == null || versions == null || users == null ||
                    disasters.Count != 1 || versions.Count != 1 || users.Count != 1)
                    return null;

                arrDatas.Add(disasters[0]);
                arrDatas.Add(users[0]);
                arrDatas.Add(versions[0]);
            }

            return arrDatas;
        }

        /// <summary>
        /// 특정 Disaster에 연관된 모든 버전정보를 얻어온다.
        /// </summary>
        /// <param name="disasterID">disasterID를 가진 Disaster가 Key값</param>
        /// <param name="strErrorMessage"></param>
        /// <returns>
        /// Disaster, User, Version의 각 객체가 순서대로 ArrayList에 담겨진다.
        /// 에러가 발생하면 null을 리턴한다.
        /// </returns>
        public ArrayList JoinDisasterUserVersion(int disasterID, out string strErrorMessage)
        {
            string strDisasterTableName = Disaster.TableName;
            string strVersionTableName = Model.Sop.Category.Version.TableName;
            string strUserTableName = User.TableName;
            bool isNullable;

            string strSQLServerCollate = "";

            if (m_dbManager.DatabaseType == WebDBManager.DBType.sqlserver)
            {
                // SQL 서버에서 DB Server가 한글 OS가 아닐 경우 Collate 에러가 발생하는것을 방지한다.
                strSQLServerCollate = "collate Korean_Wansung_CI_AS";
            }

            string strCondition = string.Format("{0}.{1} = {2}.{3} and {4}.{5} = {6}.{7} and concat(concat({8} {12}, '/') {12}, {9}) in (Select concat(concat({8} {12}, '/') {12}, {9}) from {0} where {10} = {11})",
                strDisasterTableName,
                Disaster.GetFieldName(Disaster.Fields.VersionID, out isNullable),
                strVersionTableName,
                Model.Sop.Category.Version.GetFieldName(Model.Sop.Category.Version.Fields.ID, out isNullable),
                strVersionTableName,
                Model.Sop.Category.Version.GetFieldName(Model.Sop.Category.Version.Fields.OwnerID, out isNullable),
                strUserTableName,
                User.GetFieldName(User.Fields.ID, out isNullable),
                Disaster.GetFieldName(Disaster.Fields.DisasterName, out isNullable),
                Disaster.GetFieldName(Disaster.Fields.SubDisasterCategoryID, out isNullable),
                Disaster.GetFieldName(Disaster.Fields.ID, out isNullable),
                disasterID,
                strSQLServerCollate);

            return JoinDisasterUserVersion(strCondition, out strErrorMessage);
            /*string strDisasterTableName = Disaster.TableName;
            string strVersionTableName = Model.Sop.Category.Version.TableName;
            string strUserTableName = User.TableName;

            int nDisasterFieldCount, nVersionFieldCount, nUserFieldCount;
            string strDisasterFields = GetFieldNames<Disaster.Fields>(strDisasterTableName, out nDisasterFieldCount);
            string strVersionFields = GetFieldNames<Model.Sop.Category.Version.Fields>(strVersionTableName, out nVersionFieldCount);
            string strUserFields = GetFieldNames<User.Fields>(strUserTableName, out nUserFieldCount);

            int nFieldsCount = nDisasterFieldCount + nVersionFieldCount + nUserFieldCount;
            bool isNullable;

            string strSQL = string.Format("Select {0}, {1}, {2} from {3}, {4}, {5} ", strDisasterFields, strUserFields, strVersionFields, strDisasterTableName, strUserTableName, strVersionTableName);
            strSQL += string.Format(" where {0}.{1} = {2}.{3} and {4}.{5} = {6}.{7} and concat(concat({8}, '/'), {9}) in (Select concat(concat({8}, '/'), {9}) from {0} where {10} = {11})",
                strDisasterTableName,
                Disaster.GetFieldName(Disaster.Fields.VersionID, out isNullable),
                strVersionTableName,
                Model.Sop.Category.Version.GetFieldName(Model.Sop.Category.Version.Fields.ID, out isNullable),
                strVersionTableName,
                Model.Sop.Category.Version.GetFieldName(Model.Sop.Category.Version.Fields.OwnerID, out isNullable),
                strUserTableName,
                User.GetFieldName(User.Fields.ID, out isNullable),
                Disaster.GetFieldName(Disaster.Fields.DisasterName, out isNullable),
                Disaster.GetFieldName(Disaster.Fields.SubDisasterCategoryID, out isNullable),
                Disaster.GetFieldName(Disaster.Fields.ID, out isNullable),
                disasterID);

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            var disasterTableInfo = m_dbManager.GetColumnInfoDictionary(strDisasterTableName);
            var userTableInfo = m_dbManager.GetColumnInfoDictionary(strUserTableName);
            var versionTableInfo = m_dbManager.GetColumnInfoDictionary(strVersionTableName);
            var disasterFields = GetProperties<Disaster>(); ;
            var userFields = GetProperties<User>(); ;
            var versionFields = GetProperties<Model.Sop.Category.Version>();

            Dictionary<string, int> dicDiasterFieldIndex, dicUserFieldIndex, dicVersionFieldIndex;
            List<string> disasterFieldNames = GetFieldNameIndex<Disaster.Fields>(out dicDiasterFieldIndex);
            List<string> userFieldNames = GetFieldNameIndex<User.Fields>(out dicUserFieldIndex);
            List<string> versionFieldNames = GetFieldNameIndex<Model.Sop.Category.Version.Fields>(out dicVersionFieldIndex);
            string[] notExistMember;

            strErrorMessage = null;
            int nResultCount = arrResult.Count;

            ArrayList arrDatas = new ArrayList();

            for (int i = 0; i < nResultCount - (nFieldsCount - 1); i += nFieldsCount)
            {
                ArrayList arrDisasterResult = SortWithProperties(ParseArray(arrResult, i, nDisasterFieldCount), ref disasterFields, disasterFieldNames, dicDiasterFieldIndex);
                ArrayList arrUserResult = SortWithProperties(ParseArray(arrResult, i + nDisasterFieldCount, nUserFieldCount), ref userFields, userFieldNames, dicUserFieldIndex);
                ArrayList arrVersionResult = SortWithProperties(ParseArray(arrResult, i + nDisasterFieldCount + nUserFieldCount, nVersionFieldCount), ref versionFields, versionFieldNames, dicVersionFieldIndex);

                if (arrDisasterResult == null || arrVersionResult == null || arrUserResult == null)
                    return null;

                List<object> disasters = m_dbManager.SetParamsWithColumnInfo(disasterTableInfo, new Disaster(), disasterFields, arrDisasterResult, out notExistMember);
                List<object> users = m_dbManager.SetParamsWithColumnInfo(userTableInfo, new User(), userFields, arrUserResult, out notExistMember);
                List<object> versions = m_dbManager.SetParamsWithColumnInfo(versionTableInfo, new Model.Sop.Category.Version(), versionFields, arrVersionResult, out notExistMember);

                if (disasters == null || versions == null || users == null ||
                    disasters.Count != 1 || versions.Count != 1 || users.Count != 1)
                    return null;

                arrDatas.Add(disasters[0]);
                arrDatas.Add(users[0]);
                arrDatas.Add(versions[0]);
            }

            return arrDatas;*/
        }

        /// <summary>
        /// versionID를 가진 Disaster와 같은 이름을 가진 데이터들을 얻어온다.
        /// </summary>
        /// <param name="versionID">versionID를 가진 Disaster와 같은 이름을 가진 데이터들을 얻어온다.</param>
        /// <param name="isNormal">쿼리조건</param>
        /// <returns>
        /// Disaster, User, Version의 각 객체가 순서대로 ArrayList에 담겨진다.
        /// 에러가 발생하면 null을 리턴한다.
        /// </returns>
        public ArrayList JoinDisasterUserVersionFromVersion(int versionID, bool isNormal, out string strErrorMessage)
        {
            string strDisasterTableName = Disaster.TableName;
            string strVersionTableName = Model.Sop.Category.Version.TableName;
            string strUserTableName = User.TableName;
            bool isNullable;

            string strSQLServerCollate = "";

            if (m_dbManager.DatabaseType == WebDBManager.DBType.sqlserver)
            {
                // SQL 서버에서 DB Server가 한글 OS가 아닐 경우 Collate 에러가 발생하는것을 방지한다.
                strSQLServerCollate = "collate Korean_Wansung_CI_AS";
            }

            string strCondition = string.Format("{0}.{1} = {2}.{3} and {2}.{4} = {5}.{6} and {2}.{7} = {8} and concat(concat({9}, '/') {12}, {10} {12}) in (Select concat(concat({9}, '/') {12}, {10} {12}) from {0} where {1} = {11})",
                strDisasterTableName,
                Disaster.GetFieldName(Disaster.Fields.VersionID, out isNullable),
                strVersionTableName,
                Model.Sop.Category.Version.GetFieldName(Model.Sop.Category.Version.Fields.ID, out isNullable),
                Model.Sop.Category.Version.GetFieldName(Model.Sop.Category.Version.Fields.OwnerID, out isNullable),
                strUserTableName,
                User.GetFieldName(User.Fields.ID, out isNullable),
                Model.Sop.Category.Version.GetFieldName(Model.Sop.Category.Version.Fields.IsNormal, out isNullable),
                isNormal ? 1 : 0,
                Disaster.GetFieldName(Disaster.Fields.SubDisasterCategoryID, out isNullable),
                Disaster.GetFieldName(Disaster.Fields.DisasterName, out isNullable),
                versionID,
                strSQLServerCollate);

            return JoinDisasterUserVersion(strCondition, out strErrorMessage);
        }

        // check
        // 현재 실행중인 SOP 버전인가?
        public bool IsRunningVersion(int versionID, out string strErrorMessage)
        {
            strErrorMessage = null;

            if (versionID < 0)
                return false;

            string szText = "SELECT dis.ID, step.ID as ActionStepID FROM SopCategoryDisaster as dis, SopCategoryActionStep as step, SopHistoryActionStep as ash " +
                            " WHERE dis.VersionID = {0} and step.DisasterID = dis.ID and step.ID = ash.ActionStepID and ash.EndTime is null";
            string strSQL = string.Format(szText, versionID);

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);
            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            if (arrResult.Count == 0)
                return false;


            string szText2 = "SELECT ID FROM SopCategoryDisaster WHERE versionID = {0} and id not in (select dis.ID FROM SopCategoryDisaster as dis, " +
                             "SopCategoryActionStep as step, SopHistoryActionStep as ash WHERE step.DisasterID = dis.ID and step.ID = ash.ActionStepID and " +
                             "( ash.EndTime is null))";
            string strSQL2 = string.Format(szText2, versionID);
            ArrayList arrResult2 = m_dbManager.GetResultData(strSQL2);

            if (arrResult2 == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return true;
            }

            if (arrResult2.Count == 0)
                return true;

            return false;
        }

        /// <summary>
        /// DisasterCategory부터 SubDisasterCategory, Disaster, User, Version 정보를 얻어온다.
        /// </summary>
        /// <param name="versionID"></param>
        /// <param name="strErrorMessage"></param>
        /// <returns></returns>
        public ArrayList JoinDisasterCategorySubDisasterCategoryDisasterUserVersion(int versionID, out string strErrorMessage)
        {
            string strDisasterCategoryTableName = DisasterCategory.TableName;
            string strSubDisasterCategoryTableName = SubDisasterCategory.TableName;
            string strDisasterTableName = Disaster.TableName;
            string strVersionTableName = Model.Sop.Category.Version.TableName;
            string strUserTableName = User.TableName;

            int nDisasterCategoryFieldCount, nSubDisasterCategoryFieldCount, nDisasterFieldCount, nVersionFieldCount, nUserFieldCount;

            string strDisasterCategoryFields = GetFieldNames<DisasterCategory.Fields>(strDisasterCategoryTableName, out nDisasterCategoryFieldCount);
            string strSubDisasterCategoryFields = GetFieldNames<SubDisasterCategory.Fields>(strSubDisasterCategoryTableName, out nSubDisasterCategoryFieldCount);
            string strDisasterFields = GetFieldNames<Disaster.Fields>(strDisasterTableName, out nDisasterFieldCount);
            string strVersionFields = GetFieldNames<Model.Sop.Category.Version.Fields>(strVersionTableName, out nVersionFieldCount);
            string strUserFields = GetFieldNames<User.Fields>(strUserTableName, out nUserFieldCount);

            int nFieldsCount = nDisasterCategoryFieldCount + nSubDisasterCategoryFieldCount + nDisasterFieldCount + nVersionFieldCount + nUserFieldCount;
            bool isNullable;

            string strSQL = string.Format("Select {0}, {1}, {2}, {3}, {4} from {5}, {6}, {7}, {8}, {9} ", strDisasterCategoryFields, strSubDisasterCategoryFields, strDisasterFields, strUserFields, strVersionFields, strDisasterCategoryTableName, strSubDisasterCategoryTableName, strDisasterTableName, strUserTableName, strVersionTableName);
            strSQL += string.Format(" where {0}.{1} = {2}.{3} and {0}.{4} = {5}.{6} and {5}.{7} = {8}.{9} and {2}.{10} = {11}.{12} and {2}.{3} = {13}",
                strDisasterTableName,
                Disaster.GetFieldName(Disaster.Fields.VersionID, out isNullable),
                strVersionTableName,
                Model.Sop.Category.Version.GetFieldName(Model.Sop.Category.Version.Fields.ID, out isNullable),
                Disaster.GetFieldName(Disaster.Fields.SubDisasterCategoryID, out isNullable),
                strSubDisasterCategoryTableName,
                SubDisasterCategory.GetFieldName(SubDisasterCategory.Fields.ID, out isNullable),
                SubDisasterCategory.GetFieldName(SubDisasterCategory.Fields.DisasterCategoryID, out isNullable),
                strDisasterCategoryTableName,
                DisasterCategory.GetFieldName(DisasterCategory.Fields.ID, out isNullable),
                Model.Sop.Category.Version.GetFieldName(Model.Sop.Category.Version.Fields.OwnerID, out isNullable),
                strUserTableName,
                User.GetFieldName(User.Fields.ID, out isNullable),
                versionID);

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            Dictionary<string, string> disasterCategoryTableInfo, subDisasterCategoryTableInfo, disasterTableInfo, userTableInfo, versionTableInfo;

            if (m_dicTableInfos.TryGetValue(strDisasterCategoryTableName, out disasterCategoryTableInfo) == false)
            {
                disasterCategoryTableInfo = m_dbManager.GetColumnInfoDictionary(strDisasterCategoryTableName);
                m_dicTableInfos[strDisasterCategoryTableName] = disasterCategoryTableInfo;
            }

            if (m_dicTableInfos.TryGetValue(strSubDisasterCategoryTableName, out subDisasterCategoryTableInfo) == false)
            {
                subDisasterCategoryTableInfo = m_dbManager.GetColumnInfoDictionary(strSubDisasterCategoryTableName);
                m_dicTableInfos[strSubDisasterCategoryTableName] = subDisasterCategoryTableInfo;
            }

            if (m_dicTableInfos.TryGetValue(strDisasterTableName, out disasterTableInfo) == false)
            {
                disasterTableInfo = m_dbManager.GetColumnInfoDictionary(strDisasterTableName);
                m_dicTableInfos[strDisasterTableName] = disasterTableInfo;
            }

            if (m_dicTableInfos.TryGetValue(strUserTableName, out userTableInfo) == false)
            {
                userTableInfo = m_dbManager.GetColumnInfoDictionary(strUserTableName);
                m_dicTableInfos[strUserTableName] = userTableInfo;
            }

            if (m_dicTableInfos.TryGetValue(strVersionTableName, out versionTableInfo) == false)
            {
                versionTableInfo = m_dbManager.GetColumnInfoDictionary(strVersionTableName);
                m_dicTableInfos[strVersionTableName] = versionTableInfo;
            }

            var disasterCategoryFields = GetProperties<DisasterCategory>();
            var subDisasterCategoryFields = GetProperties<SubDisasterCategory>();
            var disasterFields = GetProperties<Disaster>();
            var userFields = GetProperties<User>();
            var versionFields = GetProperties<Model.Sop.Category.Version>();

            Dictionary<string, int> dicDisasterCategoryFieldIndex, dicSubDisasterCategoryFieldIndex, dicDiasterFieldIndex, dicUserFieldIndex, dicVersionFieldIndex;
            List<string> disasterCategoryFieldNames = GetFieldNameIndex<DisasterCategory.Fields>(out dicDisasterCategoryFieldIndex);
            List<string> subDisasterCategoryFieldNames = GetFieldNameIndex<SubDisasterCategory.Fields>(out dicSubDisasterCategoryFieldIndex);
            List<string> disasterFieldNames = GetFieldNameIndex<Disaster.Fields>(out dicDiasterFieldIndex);
            List<string> userFieldNames = GetFieldNameIndex<User.Fields>(out dicUserFieldIndex);
            List<string> versionFieldNames = GetFieldNameIndex<Model.Sop.Category.Version.Fields>(out dicVersionFieldIndex);
            string[] notExistMember;

            strErrorMessage = null;
            int nResultCount = arrResult.Count;

            ArrayList arrDatas = new ArrayList();

            for (int i = 0; i < nResultCount - (nFieldsCount - 1); i += nFieldsCount)
            {
                ArrayList arrDisasterCategoryResult = SortWithProperties(ParseArray(arrResult, i, nDisasterCategoryFieldCount), ref disasterCategoryFields, disasterCategoryFieldNames, dicDisasterCategoryFieldIndex);
                ArrayList arrSubDisasterCategoryResult = SortWithProperties(ParseArray(arrResult, i + nDisasterCategoryFieldCount, nSubDisasterCategoryFieldCount), ref subDisasterCategoryFields, subDisasterCategoryFieldNames, dicSubDisasterCategoryFieldIndex);
                ArrayList arrDisasterResult = SortWithProperties(ParseArray(arrResult, i + nDisasterCategoryFieldCount + nSubDisasterCategoryFieldCount, nDisasterFieldCount), ref disasterFields, disasterFieldNames, dicDiasterFieldIndex);
                ArrayList arrUserResult = SortWithProperties(ParseArray(arrResult, i + nDisasterCategoryFieldCount + nSubDisasterCategoryFieldCount + nDisasterFieldCount, nUserFieldCount), ref userFields, userFieldNames, dicUserFieldIndex);
                ArrayList arrVersionResult = SortWithProperties(ParseArray(arrResult, i + nDisasterCategoryFieldCount + nSubDisasterCategoryFieldCount + nDisasterFieldCount + nUserFieldCount, nVersionFieldCount), ref versionFields, versionFieldNames, dicVersionFieldIndex);

                if (arrDisasterCategoryResult == null || arrSubDisasterCategoryResult == null ||
                    arrDisasterResult == null || arrVersionResult == null || arrUserResult == null)
                    return null;

                List<object> disasterCategories = m_dbManager.SetParamsWithColumnInfo(disasterCategoryTableInfo, new DisasterCategory(), disasterCategoryFields, arrDisasterCategoryResult, out notExistMember);
                List<object> subDisasterCategories = m_dbManager.SetParamsWithColumnInfo(subDisasterCategoryTableInfo, new SubDisasterCategory(), subDisasterCategoryFields, arrSubDisasterCategoryResult, out notExistMember);
                List<object> disasters = m_dbManager.SetParamsWithColumnInfo(disasterTableInfo, new Disaster(), disasterFields, arrDisasterResult, out notExistMember);
                List<object> users = m_dbManager.SetParamsWithColumnInfo(userTableInfo, new User(), userFields, arrUserResult, out notExistMember);
                List<object> versions = m_dbManager.SetParamsWithColumnInfo(versionTableInfo, new Model.Sop.Category.Version(), versionFields, arrVersionResult, out notExistMember);

                if (disasterCategories == null || subDisasterCategories == null ||
                    disasters == null || versions == null || users == null ||
                    disasterCategories.Count != 1 || subDisasterCategories.Count != 1 ||
                    disasters.Count != 1 || versions.Count != 1 || users.Count != 1)
                    return null;

                arrDatas.Add(disasterCategories[0]);
                arrDatas.Add(subDisasterCategories[0]);
                arrDatas.Add(disasters[0]);
                arrDatas.Add(users[0]);
                arrDatas.Add(versions[0]);
            }

            return arrDatas;
        }

        /// <summary>
        /// DisasterCategory부터 SubDisasterCategory, Disaster, ActionStep 정보를 얻어온다.
        /// </summary>
        /// <param name="actionStepID"></param>
        /// <param name="strErrorMessage"></param>
        /// <returns></returns>
        public ArrayList JoinDisasterCategorySubDisasterCategoryDisasterActionStep(int actionStepID, out string strErrorMessage)
        {
            string strDisasterCategoryTableName = DisasterCategory.TableName;
            string strSubDisasterCategoryTableName = SubDisasterCategory.TableName;
            string strDisasterTableName = Disaster.TableName;
            string strActionStepTableName = ActionStep.TableName;

            int nDisasterCategoryFieldCount, nSubDisasterCategoryFieldCount, nDisasterFieldCount, nActionStepFieldCount;

            string strDisasterCategoryFields = GetFieldNames<DisasterCategory.Fields>(strDisasterCategoryTableName, out nDisasterCategoryFieldCount);
            string strSubDisasterCategoryFields = GetFieldNames<SubDisasterCategory.Fields>(strSubDisasterCategoryTableName, out nSubDisasterCategoryFieldCount);
            string strDisasterFields = GetFieldNames<Disaster.Fields>(strDisasterTableName, out nDisasterFieldCount);
            string strActionStepFields = GetFieldNames<ActionStep.Fields>(strActionStepTableName, out nActionStepFieldCount);

            int nFieldsCount = nDisasterCategoryFieldCount + nSubDisasterCategoryFieldCount + nDisasterFieldCount + nActionStepFieldCount;
            bool isNullable;

            string strSQL = string.Format("Select {0}, {1}, {2}, {3} from {4}, {5}, {6}, {7} ", strDisasterCategoryFields, strSubDisasterCategoryFields, strDisasterFields, strActionStepFields, strDisasterCategoryTableName, strSubDisasterCategoryTableName, strDisasterTableName, strActionStepTableName);
            strSQL += string.Format(" where {0}.{1} = {2}.{3} and {2}.{4} = {5}.{6} and {5}.{7} = {8}.{9} and {8}.{10} = {11}",
                strDisasterCategoryTableName,
                DisasterCategory.GetFieldName(DisasterCategory.Fields.ID, out isNullable),
                strSubDisasterCategoryTableName,
                SubDisasterCategory.GetFieldName(SubDisasterCategory.Fields.DisasterCategoryID, out isNullable),
                SubDisasterCategory.GetFieldName(SubDisasterCategory.Fields.ID, out isNullable),
                strDisasterTableName,
                Disaster.GetFieldName(Disaster.Fields.SubDisasterCategoryID, out isNullable),
                Disaster.GetFieldName(Disaster.Fields.ID, out isNullable),
                strActionStepTableName,
                ActionStep.GetFieldName(ActionStep.Fields.DisasterID, out isNullable),
                ActionStep.GetFieldName(ActionStep.Fields.ID, out isNullable),
                actionStepID);

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            Dictionary<string, string> disasterCategoryTableInfo, subDisasterCategoryTableInfo, disasterTableInfo, actionStepTableInfo;

            if (m_dicTableInfos.TryGetValue(strDisasterCategoryTableName, out disasterCategoryTableInfo) == false)
            {
                disasterCategoryTableInfo = m_dbManager.GetColumnInfoDictionary(strDisasterCategoryTableName);
                m_dicTableInfos[strDisasterCategoryTableName] = disasterCategoryTableInfo;
            }

            if (m_dicTableInfos.TryGetValue(strSubDisasterCategoryTableName, out subDisasterCategoryTableInfo) == false)
            {
                subDisasterCategoryTableInfo = m_dbManager.GetColumnInfoDictionary(strSubDisasterCategoryTableName);
                m_dicTableInfos[strSubDisasterCategoryTableName] = subDisasterCategoryTableInfo;
            }

            if (m_dicTableInfos.TryGetValue(strDisasterTableName, out disasterTableInfo) == false)
            {
                disasterTableInfo = m_dbManager.GetColumnInfoDictionary(strDisasterTableName);
                m_dicTableInfos[strDisasterTableName] = disasterTableInfo;
            }

            if (m_dicTableInfos.TryGetValue(strActionStepTableName, out actionStepTableInfo) == false)
            {
                actionStepTableInfo = m_dbManager.GetColumnInfoDictionary(strActionStepTableName);
                m_dicTableInfos[strActionStepTableName] = actionStepTableInfo;
            }

            var disasterCategoryFields = GetProperties<DisasterCategory>();
            var subDisasterCategoryFields = GetProperties<SubDisasterCategory>();
            var disasterFields = GetProperties<Disaster>();
            var actionStepFields = GetProperties<ActionStep>();

            Dictionary<string, int> dicDisasterCategoryFieldIndex, dicSubDisasterCategoryFieldIndex, dicDiasterFieldIndex, dicActionStepFieldIndex;
            List<string> disasterCategoryFieldNames = GetFieldNameIndex<DisasterCategory.Fields>(out dicDisasterCategoryFieldIndex);
            List<string> subDisasterCategoryFieldNames = GetFieldNameIndex<SubDisasterCategory.Fields>(out dicSubDisasterCategoryFieldIndex);
            List<string> disasterFieldNames = GetFieldNameIndex<Disaster.Fields>(out dicDiasterFieldIndex);
            List<string> actionStepFieldNames = GetFieldNameIndex<ActionStep.Fields>(out dicActionStepFieldIndex);
            string[] notExistMember;

            strErrorMessage = null;
            int nResultCount = arrResult.Count;

            ArrayList arrDatas = new ArrayList();

            for (int i = 0; i < nResultCount - (nFieldsCount - 1); i += nFieldsCount)
            {
                ArrayList arrDisasterCategoryResult = SortWithProperties(ParseArray(arrResult, i, nDisasterCategoryFieldCount), ref disasterCategoryFields, disasterCategoryFieldNames, dicDisasterCategoryFieldIndex);
                ArrayList arrSubDisasterCategoryResult = SortWithProperties(ParseArray(arrResult, i + nDisasterCategoryFieldCount, nSubDisasterCategoryFieldCount), ref subDisasterCategoryFields, subDisasterCategoryFieldNames, dicSubDisasterCategoryFieldIndex);
                ArrayList arrDisasterResult = SortWithProperties(ParseArray(arrResult, i + nDisasterCategoryFieldCount + nSubDisasterCategoryFieldCount, nDisasterFieldCount), ref disasterFields, disasterFieldNames, dicDiasterFieldIndex);
                ArrayList arrActionStepResult = SortWithProperties(ParseArray(arrResult, i + nDisasterCategoryFieldCount + nSubDisasterCategoryFieldCount + nDisasterFieldCount, nActionStepFieldCount), ref actionStepFields, actionStepFieldNames, dicActionStepFieldIndex);

                if (arrDisasterCategoryResult == null || arrSubDisasterCategoryResult == null ||
                    arrDisasterResult == null || arrActionStepResult == null)
                    return null;

                List<object> disasterCategories = m_dbManager.SetParamsWithColumnInfo(disasterCategoryTableInfo, new DisasterCategory(), disasterCategoryFields, arrDisasterCategoryResult, out notExistMember);
                List<object> subDisasterCategories = m_dbManager.SetParamsWithColumnInfo(subDisasterCategoryTableInfo, new SubDisasterCategory(), subDisasterCategoryFields, arrSubDisasterCategoryResult, out notExistMember);
                List<object> disasters = m_dbManager.SetParamsWithColumnInfo(disasterTableInfo, new Disaster(), disasterFields, arrDisasterResult, out notExistMember);
                List<object> actionSteps = m_dbManager.SetParamsWithColumnInfo(actionStepTableInfo, new ActionStep(), actionStepFields, arrActionStepResult, out notExistMember);

                if (disasterCategories == null || subDisasterCategories == null ||
                    disasters == null || actionSteps == null ||
                    disasterCategories.Count != 1 || subDisasterCategories.Count != 1 ||
                    disasters.Count != 1 || actionSteps.Count != 1)
                    return null;

                arrDatas.Add(disasterCategories[0]);
                arrDatas.Add(subDisasterCategories[0]);
                arrDatas.Add(disasters[0]);
                arrDatas.Add(actionSteps[0]);
            }

            return arrDatas;
        }

        public ArrayList SelectSOPHistory(Dictionary<DisasterCategory.Fields, object> dicConditions1, Dictionary<SubDisasterCategory.Fields, object> dicConditions2, Dictionary<Disaster.Fields, object> dicConditions3, Dictionary<ActionStep.Fields, object> dicConditions4, Dictionary<ActionStepHistory.Fields, object> dicConditions5, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectSOPHistory(dicConditions1, dicConditions2, dicConditions3, dicConditions4, dicConditions5, strAdditionalConditions, null, out strErrorMessage);
        }

        public ArrayList SelectSOPHistory(Dictionary<DisasterCategory.Fields, object> dicConditions1, Dictionary<SubDisasterCategory.Fields, object> dicConditions2, Dictionary<Disaster.Fields, object> dicConditions3, Dictionary<ActionStep.Fields, object> dicConditions4, Dictionary<ActionStepHistory.Fields, object> dicConditions5, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            string strDisasterCategoryTableName = DisasterCategory.TableName;
            string strSubDisasterCategoryTableName = SubDisasterCategory.TableName;
            string strDisasterTableName = Disaster.TableName;
            string strActionStepTableName = ActionStep.TableName;
            string strActionStepHistoryTableName = ActionStepHistory.TableName;

            int nDisasterCategoryFieldCount, nSubDisasterCategoryFieldCount, nDisasterFieldCount, nActionStepFieldCount, nActionStepHistoryFieldCount;

            string strDisasterCategoryFields = GetFieldNames<DisasterCategory.Fields>(strDisasterCategoryTableName, out nDisasterCategoryFieldCount);
            string strSubDisasterCategoryFields = GetFieldNames<SubDisasterCategory.Fields>(strSubDisasterCategoryTableName, out nSubDisasterCategoryFieldCount);
            string strDisasterFields = GetFieldNames<Disaster.Fields>(strDisasterTableName, out nDisasterFieldCount);
            string strActionStepFields = GetFieldNames<ActionStep.Fields>(strActionStepTableName, out nActionStepFieldCount);
            string strActionStepHistoryFields = GetFieldNames<ActionStepHistory.Fields>(strActionStepHistoryTableName, out nActionStepHistoryFieldCount);

            int nFieldsCount = nDisasterCategoryFieldCount + nSubDisasterCategoryFieldCount + nDisasterFieldCount + nActionStepFieldCount + nActionStepHistoryFieldCount;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Select {0}, {1}, {2}, {3}, {4}", strDisasterCategoryFields, strSubDisasterCategoryFields, strDisasterFields, strActionStepFields, strActionStepHistoryFields);
            sb.AppendFormat("  From {0}, {1}, {2}, {3}, {4}", strDisasterCategoryTableName, strSubDisasterCategoryTableName, strDisasterTableName, strActionStepTableName, strActionStepHistoryTableName);
            sb.AppendFormat(" Where {0}.{1} = {2}.{3}", strDisasterCategoryTableName, DisasterCategory.Fields.ID, strSubDisasterCategoryTableName, SubDisasterCategory.Fields.DisasterCategoryID);
            sb.AppendFormat("   And {0}.{1} = {2}.{3}", strSubDisasterCategoryTableName, SubDisasterCategory.Fields.ID, strDisasterTableName, Disaster.Fields.SubDisasterCategoryID);
            sb.AppendFormat("   And {0}.{1} = {2}.{3}", strDisasterTableName, Disaster.Fields.ID, strActionStepTableName, ActionStep.Fields.DisasterID);
            sb.AppendFormat("   And {0}.{1} = {2}.{3}", strActionStepHistoryTableName, ActionStepHistory.Fields.ActionStepID, strActionStepTableName, ActionStep.Fields.ID);

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                sb.AppendFormat("{0}", strAdditionalConditions);
            }

            ArrayList arrResult = topNCount == null ? m_dbManager.GetResultData(sb.ToString()) : m_dbManager.GetResultData(sb.ToString(), (int)topNCount);
            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            Dictionary<string, string> disasterCategoryTableInfo, subDisasterCategoryTableInfo, disasterTableInfo, actionStepTableInfo, actionStepHistoryTableInfo;

            if (m_dicTableInfos.TryGetValue(strDisasterCategoryTableName, out disasterCategoryTableInfo) == false)
            {
                disasterCategoryTableInfo = m_dbManager.GetColumnInfoDictionary(strDisasterCategoryTableName);
                m_dicTableInfos[strDisasterCategoryTableName] = disasterCategoryTableInfo;
            }

            if (m_dicTableInfos.TryGetValue(strSubDisasterCategoryTableName, out subDisasterCategoryTableInfo) == false)
            {
                subDisasterCategoryTableInfo = m_dbManager.GetColumnInfoDictionary(strSubDisasterCategoryTableName);
                m_dicTableInfos[strSubDisasterCategoryTableName] = subDisasterCategoryTableInfo;
            }

            if (m_dicTableInfos.TryGetValue(strDisasterTableName, out disasterTableInfo) == false)
            {
                disasterTableInfo = m_dbManager.GetColumnInfoDictionary(strDisasterTableName);
                m_dicTableInfos[strDisasterTableName] = disasterTableInfo;
            }

            if (m_dicTableInfos.TryGetValue(strActionStepTableName, out actionStepTableInfo) == false)
            {
                actionStepTableInfo = m_dbManager.GetColumnInfoDictionary(strActionStepTableName);
                m_dicTableInfos[strActionStepTableName] = actionStepTableInfo;
            }

            if (m_dicTableInfos.TryGetValue(strActionStepHistoryTableName, out actionStepHistoryTableInfo) == false)
            {
                actionStepHistoryTableInfo = m_dbManager.GetColumnInfoDictionary(strActionStepHistoryTableName);
                m_dicTableInfos[strActionStepHistoryTableName] = actionStepHistoryTableInfo;
            }

            var disasterCategoryFields = GetProperties<DisasterCategory>();
            var subDisasterCategoryFields = GetProperties<SubDisasterCategory>();
            var disasterFields = GetProperties<Disaster>();
            var actionStepFields = GetProperties<ActionStep>();
            var actionStepHistoryFields = GetProperties<ActionStepHistory>();

            Dictionary<string, int> dicDisasterCategoryFieldIndex, dicSubDisasterCategoryFieldIndex, dicDiasterFieldIndex, dicActionStepFieldIndex, dicActionStepHistoryFieldIndex;
            List<string> disasterCategoryFieldNames = GetFieldNameIndex<DisasterCategory.Fields>(out dicDisasterCategoryFieldIndex);
            List<string> subDisasterCategoryFieldNames = GetFieldNameIndex<SubDisasterCategory.Fields>(out dicSubDisasterCategoryFieldIndex);
            List<string> disasterFieldNames = GetFieldNameIndex<Disaster.Fields>(out dicDiasterFieldIndex);
            List<string> actionStepFieldNames = GetFieldNameIndex<ActionStep.Fields>(out dicActionStepFieldIndex);
            List<string> actionStepHistoryFieldNames = GetFieldNameIndex<ActionStepHistory.Fields>(out dicActionStepHistoryFieldIndex);
            string[] notExistMember;

            strErrorMessage = null;
            int nResultCount = arrResult.Count;

            ArrayList arrDatas = new ArrayList();

            for (int i = 0; i < nResultCount - (nFieldsCount - 1); i += nFieldsCount)
            {
                ArrayList arrDisasterCategoryResult = SortWithProperties(ParseArray(arrResult, i, nDisasterCategoryFieldCount), ref disasterCategoryFields, disasterCategoryFieldNames, dicDisasterCategoryFieldIndex);
                ArrayList arrSubDisasterCategoryResult = SortWithProperties(ParseArray(arrResult, i + nDisasterCategoryFieldCount, nSubDisasterCategoryFieldCount), ref subDisasterCategoryFields, subDisasterCategoryFieldNames, dicSubDisasterCategoryFieldIndex);
                ArrayList arrDisasterResult = SortWithProperties(ParseArray(arrResult, i + nDisasterCategoryFieldCount + nSubDisasterCategoryFieldCount, nDisasterFieldCount), ref disasterFields, disasterFieldNames, dicDiasterFieldIndex);
                ArrayList arrActionStepResult = SortWithProperties(ParseArray(arrResult, i + nDisasterCategoryFieldCount + nSubDisasterCategoryFieldCount + nDisasterFieldCount, nActionStepFieldCount), ref actionStepFields, actionStepFieldNames, dicActionStepFieldIndex);
                ArrayList arrActionStepHistoryResult = SortWithProperties(ParseArray(arrResult, i + nDisasterCategoryFieldCount + nSubDisasterCategoryFieldCount + nDisasterFieldCount + nActionStepFieldCount, nActionStepHistoryFieldCount), ref actionStepHistoryFields, actionStepHistoryFieldNames, dicActionStepHistoryFieldIndex);

                if (arrDisasterCategoryResult == null || arrSubDisasterCategoryResult == null ||
                    arrDisasterResult == null || arrActionStepResult == null || arrActionStepHistoryResult == null)
                    return null;

                List<object> disasterCategories = m_dbManager.SetParamsWithColumnInfo(disasterCategoryTableInfo, new DisasterCategory(), disasterCategoryFields, arrDisasterCategoryResult, out notExistMember);
                List<object> subDisasterCategories = m_dbManager.SetParamsWithColumnInfo(subDisasterCategoryTableInfo, new SubDisasterCategory(), subDisasterCategoryFields, arrSubDisasterCategoryResult, out notExistMember);
                List<object> disasters = m_dbManager.SetParamsWithColumnInfo(disasterTableInfo, new Disaster(), disasterFields, arrDisasterResult, out notExistMember);
                List<object> actionSteps = m_dbManager.SetParamsWithColumnInfo(actionStepTableInfo, new ActionStep(), actionStepFields, arrActionStepResult, out notExistMember);
                List<object> actionStepHistories = m_dbManager.SetParamsWithColumnInfo(actionStepHistoryTableInfo, new ActionStepHistory(), actionStepHistoryFields, arrActionStepHistoryResult, out notExistMember);

                if (disasterCategories == null || subDisasterCategories == null ||
                    disasters == null || actionStepHistories == null || actionSteps == null ||
                    disasterCategories.Count != 1 || subDisasterCategories.Count != 1 ||
                    disasters.Count != 1 || actionStepHistories.Count != 1 || actionSteps.Count != 1)
                    return null;

                arrDatas.Add(disasterCategories[0]);
                arrDatas.Add(subDisasterCategories[0]);
                arrDatas.Add(disasters[0]);
                arrDatas.Add(actionSteps[0]);
                arrDatas.Add(actionStepHistories[0]);
            }

            return arrDatas;
        }
    }
}
