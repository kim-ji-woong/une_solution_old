using System;
using System.Collections.Generic;
using System.Text;

namespace Vacation.DAL
{
    public class QueryManager
    {
        private bool SetCondition(ref string strCondition, string strTableName, string strFieldName, object data, bool isNullable, bool isCondition)
        {
            string strAnd = isCondition ? " and " : ", ";
            strFieldName = strTableName + "." + strFieldName;

            if (isNullable)
            {
                if (data == null)
                {
                    string strNull = isCondition ? " is NULL" : " = NULL";

                    if (strCondition.Length == 0)
                        strCondition = strFieldName + strNull;
                    else
                        strCondition += strAnd + strFieldName + strNull;

                    return true;
                }
            }
            else if (data == null)
                return false;

            if ((data is int) || (data is long) || (data is float) || (data is double))
            {
                if (strCondition.Length == 0)
                    strCondition = strFieldName + " = " + data.ToString();
                else
                    strCondition += strAnd + strFieldName + " = " + data.ToString();
            }
            else if (data is bool)
            {
                bool bData = (bool)data;
                string strData = bData ? "1" : "0";

                if (strCondition.Length == 0)
                    strCondition = strFieldName + " = " + strData;
                else
                    strCondition += strAnd + strFieldName + " = " + strData;
            }
            else if (data is DateTime)
            {
                string strData = CreateManager.TimeString((DateTime)data);

                if (strCondition.Length == 0)
                    strCondition = strFieldName + " = '" + strData + "'";
                else
                    strCondition += strAnd + strFieldName + " = '" + strData + "'";
            }
            else if (data is string)
            {
                string strData = ((string)data).Replace("'", "''");

                if (strCondition.Length == 0)
                    strCondition = strFieldName + " = '" + strData + "'";
                else
                    strCondition += strAnd + strFieldName + " = '" + strData + "'";
            }
            else
                return false;

            return true;
        }

        protected delegate string GetFieldNameMethod<DataType>(DataType field, out bool isNullable);

        protected bool SetCondition<DataType>(ref string strCondition, Dictionary<DataType, object> dicConditions, GetFieldNameMethod<DataType> method, string strTableName, ref string strErrorMessage)
        {
            bool isNullable;

            if (dicConditions != null)
            {
                foreach (KeyValuePair<DataType, object> pair in dicConditions)
                {
                    string strFieldName = method(pair.Key, out isNullable);

                    if (SetCondition(ref strCondition, strTableName, strFieldName, pair.Value, isNullable, true) == false)
                    {
                        strErrorMessage = "잘못된 데이터 형식입니다.\r\n" + strTableName + "." + pair.Key.ToString();
                        return false;
                    }
                }
            }

            return true;
        }

        protected bool SetData<DataType>(ref string strSets, Dictionary<DataType, object> dicSets, GetFieldNameMethod<DataType> method, string strTableName, ref string strErrorMessage)
        {
            bool isNullable;

            if (dicSets != null)
            {
                foreach (KeyValuePair<DataType, object> pair in dicSets)
                {
                    string strFieldName = method(pair.Key, out isNullable);

                    if (SetCondition(ref strSets, strTableName, strFieldName, pair.Value, isNullable, false) == false)
                    {
                        strErrorMessage = "잘못된 데이터 형식입니다.\r\n" + strTableName + "." + pair.Key.ToString();
                        return false;
                    }
                }
            }

            return true;
        }

        protected string ListToString<DataType>(List<DataType> datas)
        {
            string str = "";

            foreach (DataType data in datas)
            {
                if (str.Length == 0)
                    str += data.ToString();
                else
                    str += ", " + data.ToString();
            }

            return str;
        }

        protected List<int> StringToIntList(string strData)
        {
            if (strData == null)
                return null;

            List<int> datas = new List<int>();

            if (strData.Length == 0)
                return datas;

            int data;
            string[] tokens = strData.Split(',');

            foreach (string strToken in tokens)
            {
                if (int.TryParse(strToken.Trim(), out data))
                    datas.Add(data);
            }

            return datas;
        }

        protected string CheckQueryString(string str)
        {
            if (str == null)
                return str;

            return str.Replace("'", "''");
        }

        protected string GetFieldNames<EnumType>(string strTableName, out int nFieldCount)
        {
            string strFields = "";
            nFieldCount = 0;

            foreach (EnumType type in Enum.GetValues(typeof(EnumType)))
            {
                if (strFields.Length == 0)
                    strFields = strTableName + "." + type.ToString();
                else
                    strFields += ", " + strTableName + "." + type.ToString();

                nFieldCount++;
            }

            return strFields;
        }
    }
}
