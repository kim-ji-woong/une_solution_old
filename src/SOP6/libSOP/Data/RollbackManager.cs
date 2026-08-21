using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using DBUtility2;

namespace UnE.SOP
{
    public class RollbackManager
    {
        private List<RollbackData> m_datas = new List<RollbackData>();

        public void AddData(RollbackData data)
        {
            m_datas.Add(data);
        }

        public bool Rollback(WebDBManager dbMgr)
        {
            foreach (RollbackData data in m_datas)
            {
                if (data.Rollback(dbMgr) == false)
                    return false;
            }

            m_datas.Clear();
            return true;
        }
    }

    public class RollbackData
    {
        private List<string> m_insertRollbackQueries = null;
        private List<string> m_deleteRollbackQueries = null;
        private List<string> m_updateRollbackQueries = null;

        public RollbackData()
        {
        }

        public RollbackData(string strSQL)
        {
            string str = strSQL.ToLower().Trim();

            if (str.StartsWith("delete"))
            {
                m_deleteRollbackQueries = new List<string>();
                m_deleteRollbackQueries.Add(strSQL);
            }
            else if (str.StartsWith("update"))
            {
                m_updateRollbackQueries = new List<string>();
                m_updateRollbackQueries.Add(strSQL);
            }
        }

        public bool Rollback(WebDBManager dbMgr)
        {
            if (m_insertRollbackQueries != null)
            {
                foreach (string strSQL in m_insertRollbackQueries)
                {
                    if (dbMgr.GetResultData(strSQL) == null)
                        return false;
                }
            }

            if (m_deleteRollbackQueries != null)
            {
                foreach (string strSQL in m_deleteRollbackQueries)
                {
                    if (dbMgr.GetResultData(strSQL) == null)
                        return false;
                }
            }

            if (m_updateRollbackQueries != null)
            {
                foreach (string strSQL in m_updateRollbackQueries)
                {
                    if (dbMgr.GetResultData(strSQL) == null)
                        return false;
                }
            }

            return true;
        }

        // args : Insert 문의 따옴표 여부
        //        1이면 따옴표 필요
        public bool AddInsertRollback(WebDBManager dbMgr, string strSelectSQL, params object[] args)
        {
            ArrayList arrResult = dbMgr.GetResultData(strSelectSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            string strTableName = GetTableName(strSelectSQL);

            int nFieldCount;
            string strFields = GetInsertFields(strSelectSQL, out nFieldCount);

            if (strFields == null || nFieldCount == 0 || args.Count() != nFieldCount)
                return false;

            string strInsertTemplate = MakeInsertQuery(strFields, strTableName);
            m_insertRollbackQueries = new List<string>();

            for (int i=0;i<nResultCount-(nFieldCount-1);i+=nFieldCount)
            {
                string strInsert = strInsertTemplate;

                for (int j=0;j<nFieldCount;j++)
                {
                    string strValue = WebDBManager.GetStringField(arrResult[i + j]);

                    if (strValue == null)
                        strValue = "NULL";
                    else
                    {
                        if ((int)args[j] == 1)
                            strValue = "'" + strValue + "'";
                    }

                    if (j == 0)
                        strInsert += strValue;
                    else
                        strInsert += ", " + strValue;
                }

                strInsert += ")";
                m_insertRollbackQueries.Add(strInsert);
            }

            return true;
        }

        public bool AddDeleteRollback(string strDeleteSQL)
        {
            m_deleteRollbackQueries = new List<string>();
            m_deleteRollbackQueries.Add(strDeleteSQL);
            return true;
        }

        public bool AddUpdateRollback(string strUpdateSQL)
        {
            m_updateRollbackQueries = new List<string>();
            m_updateRollbackQueries.Add(strUpdateSQL);
            return true;
        }

        public bool AddUpdateRollback(WebDBManager dbMgr, string strSelectSQL, params object[] args)
        {
            ArrayList arrResult = dbMgr.GetResultData(strSelectSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            string strTableName = GetTableName(strSelectSQL);

            string strCondition;
            string[] arrFields = GetUpdateFields(strSelectSQL, out strCondition);

            if (arrFields == null)
                return false;

            int nFieldCount = arrFields.Count();

            if (nFieldCount == 0 || args.Count() != nFieldCount || nResultCount != nFieldCount)
                return false;

            string strUpdate = MakeUpdateQuery(arrFields, args, arrResult, strTableName);

            if (strCondition.Length > 0)
                strUpdate += " " + strCondition;

            m_updateRollbackQueries = new List<string>();
            m_updateRollbackQueries.Add(strUpdate);
            return true;
        }

        private string GetTableName(string strSQL)
        {
            string sql = strSQL.ToLower();

            string strTarget = "from";
            int nIndex = sql.IndexOf(strTarget);

            if (nIndex < 0)
                return null;

            string str = strSQL.Substring(nIndex + strTarget.Length).Trim();
            string[] tokens = str.Split(' ');

            return tokens[0].Trim();
        }

        private string GetInsertFields(string strSQL, out int nFieldCount)
        {
            nFieldCount = 0;
            string sql = strSQL.ToLower();

            string strTarget = "select";
            int nIndex = sql.IndexOf(strTarget);

            if (nIndex < 0)
                return null;

            string str = strSQL.Substring(nIndex + strTarget.Length).Trim();
            sql = str.ToLower();

            strTarget = "from";
            nIndex = sql.IndexOf(strTarget);

            if (nIndex < 0)
                return null;

            string strFields = str.Substring(0, nIndex).Trim();
            string[] tokens = strFields.Split(',');
            nFieldCount = tokens.Count();

            return strFields;
        }

        private string[] GetUpdateFields(string strSQL, out string strCondition)
        {
            strCondition = "";

            string sql = strSQL.ToLower();

            string strTarget = "select";
            int nIndex = sql.IndexOf(strTarget);

            if (nIndex < 0)
                return null;

            string str = strSQL.Substring(nIndex + strTarget.Length).Trim();
            sql = str.ToLower();

            strTarget = "from";
            nIndex = sql.IndexOf(strTarget);

            if (nIndex < 0)
                return null;

            string strFields = str.Substring(0, nIndex).Trim();
            string[] arrFields = strFields.Split(',');

            strTarget = "where";
            nIndex = sql.IndexOf(strTarget);

            if (nIndex >= 0)
            {
                strCondition = strSQL.Substring(nIndex);
            }

            return arrFields;
        }

        private string MakeInsertQuery(string strFields, string strTableName)
        {
            return string.Format("Insert into {0} ({1}) values (", strTableName, strFields);
        }

        private string MakeUpdateQuery(string[] arrFields, object[] args, ArrayList arrResult, string strTableName)
        {
            string strUpdate = "Update " + strTableName + " Set ";
            int nFieldCount = arrFields.Count();

            for (int i=0;i<nFieldCount;i++)
            {
                string strValue = WebDBManager.GetStringField(arrResult[i]);

                if (strValue == null)
                    strValue = "NULL";
                else
                {
                    if ((int)args[i] == 1)
                        strValue = "'" + strValue + "'";
                }

                if (i == 0)
                    strUpdate += string.Format("{0} = {1}", arrFields[i], strValue);
                else
                    strUpdate += string.Format(", {0} = {1}", arrFields[i], strValue);
            }

            return strUpdate;
        }
    }
}
