using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UnEService
{
    public abstract class DBManager
    {
        public static string Host
        {
            get;
            set;
        }

        public static string ID
        {
            get;
            set;
        }

        public static string PW
        {
            get;
            set;
        }

        public static string CharSet
        {
            get;
            set;
        }

        public DateTime CreateTime
        {
            get;
            set;
        }

        public long TransactionKey
        {
            get;
            set;
        }

        private bool m_isRemoved = false;

        public bool IsRemoved
        {
            get { return m_isRemoved; }
            set { m_isRemoved = value; }
        }

        public abstract string BatchCommit();
        public abstract string BatchRollback();

        protected static bool IsSelectQuery(string strSQL)
        {
            strSQL = strSQL.Trim().ToLower();
            return strSQL.StartsWith("select");
        }

        protected static void AddNullData(List<string> datas)
        {
            datas.Add("~");
        }

        protected static void AddData(List<string> datas, object data)
        {
            datas.Add("!" + data.ToString());
        }

        protected static string[] MakeSuccess(List<string> datas)
        {
            string[] results = null;

            if (datas == null)
            {
                results = new string[2];

                results[0] = "1";
                results[1] = "0";
            }
            else
            {
                int nDataCount = datas.Count + 2;
                results = new string[nDataCount];

                results[0] = "1";
                results[1] = datas.Count.ToString();

                for (int i = 2; i < nDataCount; i++)
                {
                    results[i] = datas[i - 2];
                }
            }

            return results;
        }
    }
}