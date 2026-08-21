using dnsDBUtil;
using SmartCity.IDAL;
using SmartCity.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SmartCity.DAL
{
    public class SelectManager : QueryManager, ISelectManager
    {
        private string m_strErrorMessage = null;
        private DataManager m_dataManager = null;
        private WebDBManager m_dbManager = null;

        public SelectManager(DataManager dataManager)
        {
            m_dataManager = dataManager;
            m_dbManager = m_dataManager.GetDBManager() as WebDBManager;
        }

        public AccountUser SelectAccountUser(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<AccountUser.Fields>(out nFieldCount), AccountUser.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                AccountUser model = ReadAccountUser(arrResult, 0, out strErrorMessage);

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

        private AccountUser ReadAccountUser(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            AccountUser model = new AccountUser();
            bool isNullable;

            foreach (AccountUser.Fields field in AccountUser.Fields.GetValues(typeof(AccountUser.Fields)))
            {
                string strFieldName = AccountUser.GetFieldName(field, out isNullable);

                if (field == AccountUser.Fields.ID)
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
                else if (field == AccountUser.Fields.UserID)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.UserID = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.UserID = str;
                }
                else if (field == AccountUser.Fields.Password)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.Password = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Password = str;
                }
                else if (field == AccountUser.Fields.NickName)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.NickName = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.NickName = str;
                }
                else if (field == AccountUser.Fields.UserLevel)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.UserLevel = data.Data;
                    }
                }
                else if (field == AccountUser.Fields.FacilityType)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.ListFacilityType = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.FacilityType = str;
                        model.ListFacilityType = StringToIntList(str);
                    }
                        
                }

                index++;
            }

            return model;
        }

        public List<AccountUser> SelectAccountUsers(Dictionary<AccountUser.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<AccountUser.Fields>(out nFieldCount), AccountUser.TableName);

            string strCondition = "";

            if (SetCondition<AccountUser.Fields>(ref strCondition, dicConditions, AccountUser.GetFieldName, AccountUser.TableName, ref strErrorMessage) == false)
                return null;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;
            List<AccountUser> accountUsers = new List<AccountUser>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                AccountUser model = ReadAccountUser(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    accountUsers.Add(model);
            }

            return accountUsers;
        }

        FacilityType ISelectManager.SelectFacilityType(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<FacilityType.Fields>(out nFieldCount), FacilityType.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                FacilityType model = ReadFacilityType(arrResult, 0, out strErrorMessage);

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

        private FacilityType ReadFacilityType(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            FacilityType model = new FacilityType();
            bool isNullable;

            foreach (FacilityType.Fields field in FacilityType.Fields.GetValues(typeof(FacilityType.Fields)))
            {
                string strFieldName = FacilityType.GetFieldName(field, out isNullable);

                if (field == FacilityType.Fields.ID)
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
                else if (field == FacilityType.Fields.FacilityType)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.FacilityTypeName = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.FacilityTypeName = str;
                }
                else if (field == FacilityType.Fields.LinkedTableName)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.LinkedTableName = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.LinkedTableName = str;
                }
                else if (field == FacilityType.Fields.Description)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
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
                        model.Description = str;
                }
                

                index++;
            }

            return model;
        }

        List<FacilityType> ISelectManager.SelectFacilityTypes(Dictionary<FacilityType.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<FacilityType.Fields>(out nFieldCount), FacilityType.TableName);

            string strCondition = "";

            if (SetCondition<FacilityType.Fields>(ref strCondition, dicConditions, FacilityType.GetFieldName, FacilityType.TableName, ref strErrorMessage) == false)
                return null;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;
            List<FacilityType> facilityTypes = new List<FacilityType>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                FacilityType model = ReadFacilityType(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    facilityTypes.Add(model);
            }

            return facilityTypes;
        }

        AccountLevel ISelectManager.SelectAccountLevel(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<AccountLevel.Fields>(out nFieldCount), AccountLevel.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                AccountLevel model = ReadAccountLevel(arrResult, 0, out strErrorMessage);

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

        private AccountLevel ReadAccountLevel(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            AccountLevel model = new AccountLevel();
            bool isNullable;

            foreach (AccountLevel.Fields field in AccountLevel.Fields.GetValues(typeof(AccountLevel.Fields)))
            {
                string strFieldName = AccountLevel.GetFieldName(field, out isNullable);

                if (field == AccountLevel.Fields.ID)
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
                else if (field == AccountLevel.Fields.LevelName)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.LevelName = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.LevelName = str;
                }
               
                index++;
            }

            return model;
        }

        List<AccountLevel> ISelectManager.SelectAccountLevels(Dictionary<AccountLevel.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<AccountLevel.Fields>(out nFieldCount), AccountLevel.TableName);

            string strCondition = "";

            if (SetCondition<AccountLevel.Fields>(ref strCondition, dicConditions, AccountLevel.GetFieldName, AccountLevel.TableName, ref strErrorMessage) == false)
                return null;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;
            List<AccountLevel> accountLevels = new List<AccountLevel>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                AccountLevel model = ReadAccountLevel(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    accountLevels.Add(model);
            }

            return accountLevels;
        }

        AccountSession ISelectManager.SelectAccountSession(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<AccountSession.Fields>(out nFieldCount), AccountSession.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                AccountSession model = ReadAccountSession(arrResult, 0, out strErrorMessage);

                if (model == null)
                    return null;

                return model;
            }
            else
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;

            throw new NotImplementedException();
        }

        private AccountSession ReadAccountSession(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            AccountSession model = new AccountSession();
            bool isNullable;

            foreach (AccountSession.Fields field in AccountSession.Fields.GetValues(typeof(AccountSession.Fields)))
            {
                string strFieldName = AccountSession.GetFieldName(field, out isNullable);

                if (field == AccountSession.Fields.ID)
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
                else if (field == AccountSession.Fields.AccountUserID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.AccountUserID = -1;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.AccountUserID = data.Data;
                }
                else if (field == AccountSession.Fields.SessionKey)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.SessionKey = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.SessionKey = str;
                }
                else if (field == AccountSession.Fields.CreateDate)
                {
                    VariousData<DateTime> data = WebDBManager.GetDateTimeField(arrResult[index]);

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.CreateDate = data.Data;
                }
                else if (field == AccountSession.Fields.UpdateDate)
                {
                    VariousData<DateTime> data = WebDBManager.GetDateTimeField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.UpdateDate = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.UpdateDate = data.Data;
                }

                index++;
            }

            return model;
        }

        List<AccountSession> ISelectManager.SelectAccountSessions(Dictionary<AccountSession.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<AccountSession.Fields>(out nFieldCount), AccountSession.TableName);

            string strCondition = "";

            if (SetCondition<AccountSession.Fields>(ref strCondition, dicConditions, AccountSession.GetFieldName, AccountSession.TableName, ref strErrorMessage) == false)
                return null;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;
            List<AccountSession> accountSessions = new List<AccountSession>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                AccountSession model = ReadAccountSession(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    accountSessions.Add(model);
            }

            return accountSessions;

            throw new NotImplementedException();
        }

        Options ISelectManager.SelectOptions(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<Options.Fields>(out nFieldCount), Options.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                Options model = ReadOptions(arrResult, 0, out strErrorMessage);

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

        List<Options> ISelectManager.SelectOptions(Dictionary<Options.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<Options.Fields>(out nFieldCount), Model.Options.TableName);

            string strCondition = "";

            if (SetCondition<Options.Fields>(ref strCondition, dicConditions, Model.Options.GetFieldName, Model.Options.TableName, ref strErrorMessage) == false)
                return null;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;
            List<Options> Options = new List<Options>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                Options model = ReadOptions(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    Options.Add(model);
            }

            return Options;
        }

        private Options ReadOptions(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            Options model = new Options();
            bool isNullable;

            foreach (Options.Fields field in Options.Fields.GetValues(typeof(Options.Fields)))
            {
                string strFieldName = Options.GetFieldName(field, out isNullable);

                if (field == Options.Fields.ID)
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
                else if (field == Options.Fields.PropertyName)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.PropertyName = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.PropertyName = str;
                }
                else if (field == Options.Fields.PropertyValue)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.PropertyValue = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.PropertyValue = str;
                }
                else if (field == Options.Fields.Description)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
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
                        model.Description = str;
                }


                index++;
            }

            return model;
        }

        FireSensor ISelectManager.SelectFireSensor(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<FireSensor.Fields>(out nFieldCount), FireSensor.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                FireSensor model = ReadFireSensor(arrResult, 0, out strErrorMessage);

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

        private FireSensor ReadFireSensor(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            FireSensor model = new FireSensor();
            bool isNullable;

            foreach (FireSensor.Fields field in FireSensor.Fields.GetValues(typeof(FireSensor.Fields)))
            {
                string strFieldName = FireSensor.GetFieldName(field, out isNullable);

                if (field == FireSensor.Fields.ID)
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
                else if (field == FireSensor.Fields.SensorID)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.SensorID = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.SensorID = str;
                }
                else if (field == FireSensor.Fields.State)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.State = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.State = str;
                }
                else if (field == FireSensor.Fields.Addr)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.Addr = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Addr = str;
                }
                else if (field == FireSensor.Fields.OccurTime)
                {
                    VariousData<DateTime> data = WebDBManager.GetDateTimeField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.OccurTime = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.OccurTime = data.Data;
                }
                else if (field == FireSensor.Fields.CloseTime)
                {
                    VariousData<DateTime> data = WebDBManager.GetDateTimeField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.CloseTime = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.CloseTime = data.Data;
                }
                else if (field == FireSensor.Fields.IsAfterFire)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.IsAfterFire = 0;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.IsAfterFire = data.Data;
                }
                else if (field == FireSensor.Fields.AlarmPeriodStart)
                {
                    VariousData<DateTime> data = WebDBManager.GetDateTimeField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.AlarmPeriodStart = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.AlarmPeriodStart = data.Data;
                }
                else if (field == FireSensor.Fields.AlarmPeriodEnd)
                {
                    VariousData<DateTime> data = WebDBManager.GetDateTimeField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.AlarmPeriodEnd = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.AlarmPeriodEnd = data.Data;
                }
                else if (field == FireSensor.Fields.WeakStart)
                {
                    VariousData<DateTime> data = WebDBManager.GetDateTimeField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.WeakStart = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.WeakStart = data.Data;
                }
                else if (field == FireSensor.Fields.IsInitReact)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.IsInitReact = 0;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.IsInitReact = data.Data;
                }
                else if (field == FireSensor.Fields.Demander)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.Demander = 0;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Demander = data.Data;
                }
                else if (field == FireSensor.Fields.DeathToll)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.DeathToll = 0;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.DeathToll = data.Data;
                }
                else if (field == FireSensor.Fields.Message)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
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
                        model.Message = str;
                }
                else if (field == FireSensor.Fields.IsUserModifity)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.IsUserModifity = 0;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.IsUserModifity = data.Data;
                }

                index++;
            }

            return model;
        }

        List<FireSensor> ISelectManager.SelectFireSensors(Dictionary<FireSensor.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<FireSensor.Fields>(out nFieldCount), Model.FireSensor.TableName);

            string strCondition = "";

            if (SetCondition<FireSensor.Fields>(ref strCondition, dicConditions, Model.FireSensor.GetFieldName, Model.FireSensor.TableName, ref strErrorMessage) == false)
                return null;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;
            List<FireSensor> FireSensors = new List<FireSensor>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                FireSensor model = ReadFireSensor(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    FireSensors.Add(model);
            }

            return FireSensors;
        }

        FloodSensor ISelectManager.SelectFloodSensor(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<FloodSensor.Fields>(out nFieldCount), FloodSensor.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                FloodSensor model = ReadFloodSensor(arrResult, 0, out strErrorMessage);

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

        private FloodSensor ReadFloodSensor(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            FloodSensor model = new FloodSensor();
            bool isNullable;

            foreach (FloodSensor.Fields field in FloodSensor.Fields.GetValues(typeof(FloodSensor.Fields)))
            {
                string strFieldName = FloodSensor.GetFieldName(field, out isNullable);

                if (field == FloodSensor.Fields.ID)
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
                else if (field == FloodSensor.Fields.SensorID)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.SensorID = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.SensorID = str;
                }
                else if (field == FloodSensor.Fields.State)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.State = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.State = str;
                }
                else if (field == FloodSensor.Fields.Addr)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.Addr = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Addr = str;
                }
                else if (field == FloodSensor.Fields.MeasureTime)
                {
                    VariousData<DateTime> data = WebDBManager.GetDateTimeField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.MeasureTime = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.MeasureTime = data.Data;
                }
                else if (field == FloodSensor.Fields.Depth)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.Depth = 0;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Depth = data.Data;
                }
                else if (field == FloodSensor.Fields.Flow)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.Flow = 0;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Flow = data.Data;
                }
                else if (field == FloodSensor.Fields.Message)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
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
                        model.Message = str;
                }
                else if (field == FloodSensor.Fields.IsUserModifity)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.IsUserModifity = 0;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.IsUserModifity = data.Data;
                }

                index++;
            }

            return model;
        }

        List<FloodSensor> ISelectManager.SelectFloodSensors(Dictionary<FloodSensor.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<FloodSensor.Fields>(out nFieldCount), Model.FloodSensor.TableName);

            string strCondition = "";

            if (SetCondition<FloodSensor.Fields>(ref strCondition, dicConditions, Model.FloodSensor.GetFieldName, Model.FloodSensor.TableName, ref strErrorMessage) == false)
                return null;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;
            List<FloodSensor> FloodSensors = new List<FloodSensor>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                FloodSensor model = ReadFloodSensor(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    FloodSensors.Add(model);
            }

            return FloodSensors;
        }

        HeatSensor ISelectManager.SelectHeatSensor(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<HeatSensor.Fields>(out nFieldCount), HeatSensor.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                HeatSensor model = ReadHeatSensor(arrResult, 0, out strErrorMessage);

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

        private HeatSensor ReadHeatSensor(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            HeatSensor model = new HeatSensor();
            bool isNullable;

            foreach (HeatSensor.Fields field in HeatSensor.Fields.GetValues(typeof(HeatSensor.Fields)))
            {
                string strFieldName = HeatSensor.GetFieldName(field, out isNullable);

                if (field == HeatSensor.Fields.ID)
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
                else if (field == HeatSensor.Fields.SensorID)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.SensorID = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.SensorID = str;
                }
                else if (field == HeatSensor.Fields.State)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.State = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.State = str;
                }
                else if (field == HeatSensor.Fields.Addr)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.Addr = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Addr = str;
                }
                else if (field == HeatSensor.Fields.OccurTime)
                {
                    VariousData<DateTime> data = WebDBManager.GetDateTimeField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.OccurTime = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.OccurTime = data.Data;
                }
                else if (field == HeatSensor.Fields.Temperature)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.Temperature = 0;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Temperature = data.Data;
                }
                else if (field == HeatSensor.Fields.Humidity)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.Humidity = 0;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Humidity = data.Data;
                }
                else if (field == HeatSensor.Fields.Direction)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.Direction = 0;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Direction = data.Data;
                }
                else if (field == HeatSensor.Fields.Speed)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.Speed = 0;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Speed = data.Data;
                }
                else if (field == HeatSensor.Fields.MeasPeriodStart)
                {
                    VariousData<DateTime> data = WebDBManager.GetDateTimeField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.MeasPeriodStart = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.MeasPeriodStart = data.Data;
                }
                else if (field == HeatSensor.Fields.MeasPeriodEnd)
                {
                    VariousData<DateTime> data = WebDBManager.GetDateTimeField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.MeasPeriodEnd = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.MeasPeriodEnd = data.Data;
                }
                else if (field == HeatSensor.Fields.PreliminaryDate)
                {
                    VariousData<DateTime> data = WebDBManager.GetDateTimeField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.PreliminaryDate = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.PreliminaryDate = data.Data;
                }
                else if (field == HeatSensor.Fields.AdvisoryDate)
                {
                    VariousData<DateTime> data = WebDBManager.GetDateTimeField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.AdvisoryDate = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.AdvisoryDate = data.Data;
                }
                else if (field == HeatSensor.Fields.AlertDate)
                {
                    VariousData<DateTime> data = WebDBManager.GetDateTimeField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.AlertDate = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.AlertDate = data.Data;
                }
                else if (field == HeatSensor.Fields.DeathToll)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.DeathToll = 0;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.DeathToll = data.Data;
                }
                else if (field == HeatSensor.Fields.Message)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
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
                        model.Message = str;
                }
                else if (field == HeatSensor.Fields.IsUserModifity)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.IsUserModifity = 0;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.IsUserModifity = data.Data;
                }

                index++;
            }

            return model;
        }

        List<HeatSensor> ISelectManager.SelectHeatSensors(Dictionary<HeatSensor.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<HeatSensor.Fields>(out nFieldCount), Model.HeatSensor.TableName);

            string strCondition = "";

            if (SetCondition<HeatSensor.Fields>(ref strCondition, dicConditions, Model.HeatSensor.GetFieldName, Model.HeatSensor.TableName, ref strErrorMessage) == false)
                return null;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;
            List<HeatSensor> HeatSensors = new List<HeatSensor>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                HeatSensor model = ReadHeatSensor(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    HeatSensors.Add(model);
            }

            return HeatSensors;
        }

        CollapseSensor ISelectManager.SelectCollapseSensor(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<CollapseSensor.Fields>(out nFieldCount), CollapseSensor.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                CollapseSensor model = ReadCollapseSensor(arrResult, 0, out strErrorMessage);

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

        private CollapseSensor ReadCollapseSensor(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            CollapseSensor model = new CollapseSensor();
            bool isNullable;

            foreach (CollapseSensor.Fields field in CollapseSensor.Fields.GetValues(typeof(CollapseSensor.Fields)))
            {
                string strFieldName = CollapseSensor.GetFieldName(field, out isNullable);

                if (field == CollapseSensor.Fields.ID)
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
                else if (field == CollapseSensor.Fields.SensorID)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.SensorID = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.SensorID = str;
                }
                else if (field == CollapseSensor.Fields.State)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.State = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.State = str;
                }
                else if (field == CollapseSensor.Fields.Addr)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.Addr = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Addr = str;
                }
                else if (field == CollapseSensor.Fields.MeasureTime)
                {
                    VariousData<DateTime> data = WebDBManager.GetDateTimeField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.MeasureTime = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.MeasureTime = data.Data;
                }
                else if (field == CollapseSensor.Fields.Message)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
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
                        model.Message = str;
                }
                else if (field == CollapseSensor.Fields.IsUserModifity)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.IsUserModifity = 0;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.IsUserModifity = data.Data;
                }

                index++;
            }

            return model;
        }


        List<CollapseSensor> ISelectManager.SelectCollapseSensors(Dictionary<CollapseSensor.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<CollapseSensor.Fields>(out nFieldCount), Model.CollapseSensor.TableName);

            string strCondition = "";

            if (SetCondition<CollapseSensor.Fields>(ref strCondition, dicConditions, Model.CollapseSensor.GetFieldName, Model.CollapseSensor.TableName, ref strErrorMessage) == false)
                return null;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;
            List<CollapseSensor> CollapseSensors = new List<CollapseSensor>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                CollapseSensor model = ReadCollapseSensor(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    CollapseSensors.Add(model);
            }

            return CollapseSensors;
        }

        AlertAlarm ISelectManager.SelectAlertAlarm(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<AlertAlarm.Fields>(out nFieldCount), AlertAlarm.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                AlertAlarm model = ReadAlertAlarm(arrResult, 0, out strErrorMessage);

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

        private AlertAlarm ReadAlertAlarm(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            AlertAlarm model = new AlertAlarm();
            bool isNullable;

            foreach (AlertAlarm.Fields field in AlertAlarm.Fields.GetValues(typeof(AlertAlarm.Fields)))
            {
                string strFieldName = AlertAlarm.GetFieldName(field, out isNullable);

                if (field == AlertAlarm.Fields.ID)
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
                else if (field == AlertAlarm.Fields.FacilityType)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.FacilityType = -1;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.FacilityType = data.Data;
                }
                else if (field == AlertAlarm.Fields.SensorID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.SensorID = -1;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.SensorID = data.Data;
                }
                else if (field == AlertAlarm.Fields.RiskLevel)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.RiskLevel = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.RiskLevel = str;
                }
                else if (field == AlertAlarm.Fields.Address)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.Address = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Address = str;
                }
                else if (field == AlertAlarm.Fields.IsCheck)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.IsCheck = 0;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.IsCheck = data.Data;
                }
                else if (field == AlertAlarm.Fields.CreateTime)
                {
                    VariousData<DateTime> data = WebDBManager.GetDateTimeField(arrResult[index]);

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.CreateTime = data.Data;
                }

                index++;
            }

            return model;
        }




        List<AlertAlarm> ISelectManager.SelectAlertAlarms(Dictionary<AlertAlarm.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<AlertAlarm.Fields>(out nFieldCount), AlertAlarm.TableName);

            string strCondition = "";

            if (SetCondition<AlertAlarm.Fields>(ref strCondition, dicConditions, AlertAlarm.GetFieldName, AlertAlarm.TableName, ref strErrorMessage) == false)
                return null;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            strSQL += " order by IsCheck asc, CreateTime desc";

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;
            List<AlertAlarm> alertAlarms = new List<AlertAlarm>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                AlertAlarm model = ReadAlertAlarm(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    alertAlarms.Add(model);
            }

            return alertAlarms;

        }

        FacilityManual ISelectManager.SelectFacilityManual(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<FacilityManual.Fields>(out nFieldCount), FacilityManual.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                FacilityManual model = ReadFacilityManual(arrResult, 0, out strErrorMessage);

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

        private FacilityManual ReadFacilityManual(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            FacilityManual model = new FacilityManual();
            bool isNullable;

            foreach (FacilityManual.Fields field in FacilityManual.Fields.GetValues(typeof(FacilityManual.Fields)))
            {
                string strFieldName = FacilityManual.GetFieldName(field, out isNullable);

                if (field == FacilityManual.Fields.ID)
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
                else if (field == FacilityManual.Fields.FacilityType)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.FacilityType = -1;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.FacilityType = data.Data;
                }
                else if (field == FacilityManual.Fields.ManualType)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.ManualType = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.ManualType = str;
                }
                else if (field == FacilityManual.Fields.ManualTitle)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.ManualTitle = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.ManualTitle = str;
                }
                else if (field == FacilityManual.Fields.ManualMembers)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.ManualMembers = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.ManualMembers = str;
                }
                else if (field == FacilityManual.Fields.Number)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.Number = -1;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Number = data.Data;
                }
                else if (field == FacilityManual.Fields.Manual)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.Manual = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Manual = str;
                }
                
                index++;
            }

            return model;
        }


        List<FacilityManual> ISelectManager.SelectFacilityManuals(Dictionary<FacilityManual.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<FacilityManual.Fields>(out nFieldCount), FacilityManual.TableName);

            string strCondition = "";

            if (SetCondition<FacilityManual.Fields>(ref strCondition, dicConditions, FacilityManual.GetFieldName, FacilityManual.TableName, ref strErrorMessage) == false)
                return null;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            strSQL += " order by Number asc";

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;
            List<FacilityManual> facilityManuals = new List<FacilityManual>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                FacilityManual model = ReadFacilityManual(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    facilityManuals.Add(model);
            }

            return facilityManuals;

        }

        CompanyMember ISelectManager.SelectCompanyMember(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<CompanyMember.Fields>(out nFieldCount), CompanyMember.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                CompanyMember model = ReadCompanyMember(arrResult, 0, out strErrorMessage);

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

        private CompanyMember ReadCompanyMember(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            CompanyMember model = new CompanyMember();
            bool isNullable;

            foreach (CompanyMember.Fields field in CompanyMember.Fields.GetValues(typeof(CompanyMember.Fields)))
            {
                string strFieldName = CompanyMember.GetFieldName(field, out isNullable);

                if (field == CompanyMember.Fields.ID)
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
                else if (field == CompanyMember.Fields.MemberName)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.MemberName = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.MemberName = str;
                }
                else if (field == CompanyMember.Fields.RegularTeamID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.RegularTeamID = -1;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.RegularTeamID = data.Data;
                }
                else if (field == CompanyMember.Fields.LevelID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.LevelID = -1;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.LevelID = data.Data;
                }
                else if (field == CompanyMember.Fields.PhoneNumber)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.PhoneNumber = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.PhoneNumber = str;
                }
                else if (field == CompanyMember.Fields.FacilityTypes)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.FacilityTypes = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.FacilityTypes = str;
                }

                index++;
            }

            return model;
        }

        List<CompanyMember> ISelectManager.SelectCompanyMembers(Dictionary<CompanyMember.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<CompanyMember.Fields>(out nFieldCount), CompanyMember.TableName);

            string strCondition = "";

            if (SetCondition<CompanyMember.Fields>(ref strCondition, dicConditions, CompanyMember.GetFieldName, CompanyMember.TableName, ref strErrorMessage) == false)
                return null;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;
            List<CompanyMember> companyMembers = new List<CompanyMember>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                CompanyMember model = ReadCompanyMember(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    companyMembers.Add(model);
            }

            return companyMembers;
        }

        JobLevel ISelectManager.SelectJobLevel(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<JobLevel.Fields>(out nFieldCount), JobLevel.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                JobLevel model = ReadJobLevel(arrResult, 0, out strErrorMessage);

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

        private JobLevel ReadJobLevel(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            JobLevel model = new JobLevel();
            bool isNullable;

            foreach (JobLevel.Fields field in JobLevel.Fields.GetValues(typeof(JobLevel.Fields)))
            {
                string strFieldName = JobLevel.GetFieldName(field, out isNullable);

                if (field == JobLevel.Fields.ID)
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
                else if (field == JobLevel.Fields.LevelName)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.LevelName = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.LevelName = str;
                }
                else if (field == JobLevel.Fields.LevelNo)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.LevelNo = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.LevelNo = data.Data;
                }

                index++;
            }

            return model;
        }

        List<JobLevel> ISelectManager.SelectJobLevels(Dictionary<JobLevel.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<JobLevel.Fields>(out nFieldCount), JobLevel.TableName);

            string strCondition = "";

            if (SetCondition<JobLevel.Fields>(ref strCondition, dicConditions, JobLevel.GetFieldName, JobLevel.TableName, ref strErrorMessage) == false)
                return null;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;
            List<JobLevel> jobLevels = new List<JobLevel>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                JobLevel model = ReadJobLevel(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    jobLevels.Add(model);
            }

            return jobLevels;
        }

        RegularTeam ISelectManager.SelectRegularTeam(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<RegularTeam.Fields>(out nFieldCount), RegularTeam.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                RegularTeam model = ReadRegularTeam(arrResult, 0, out strErrorMessage);

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

        List<RegularTeam> ISelectManager.SelectRegularTeams(Dictionary<RegularTeam.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<RegularTeam.Fields>(out nFieldCount), RegularTeam.TableName);

            string strCondition = "";

            if (SetCondition<RegularTeam.Fields>(ref strCondition, dicConditions, RegularTeam.GetFieldName, RegularTeam.TableName, ref strErrorMessage) == false)
                return null;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;
            List<RegularTeam> regularTeams = new List<RegularTeam>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                RegularTeam model = ReadRegularTeam(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    regularTeams.Add(model);
            }

            return regularTeams;
        }

        private RegularTeam ReadRegularTeam(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            RegularTeam model = new RegularTeam();
            bool isNullable;

            foreach (RegularTeam.Fields field in RegularTeam.Fields.GetValues(typeof(RegularTeam.Fields)))
            {
                string strFieldName = RegularTeam.GetFieldName(field, out isNullable);

                if (field == RegularTeam.Fields.ID)
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
                else if (field == RegularTeam.Fields.TeamName)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.TeamName = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.TeamName = str;
                }
                else if (field == RegularTeam.Fields.ParentTeamID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.ParentTeamID = -1;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.ParentTeamID = data.Data;
                }

                index++;
            }

            return model;
        }
    }
}
