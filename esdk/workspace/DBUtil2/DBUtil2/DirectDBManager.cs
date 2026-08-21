using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace DBUtility2
{
    // 빠른 DB 제어를 위한 클래스
    // WebDBManager보다 100배 이상 빠르지만 DB 접속정보를 알고 있어야 한다
    public abstract class DirectDBManager
    {
        public enum DBType { sqlserver = 0, mysql, TypeCount };

        protected string Host;
        protected string ID;
        protected string PW;
        protected string DBName;
        protected string CharSet = "utf8";
        protected string m_strErrorMessage = "";
        protected int m_nSiteID = 0;
        protected bool m_isConnected = false;

        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        public bool IsConnected
        {
            get { return m_isConnected; }
        }

        public string ErrorMessage
        {
            get { return m_strErrorMessage; }
        }

        public static DirectDBManager MakeInstance(DBType dbType, string strHost, string strID, string strPW, string strDBName, string strCharSet = "utf8")
        {
            DirectDBManager mgr = null;

#if NoDirect
            mgr = new NoDirectDBManager();
            ((NoDirectDBManager)mgr).DatabaseType = dbType;
            
#else
            if (dbType == DBType.sqlserver)
                mgr = new SqlServerManager();
            else if (dbType == DBType.mysql)
                mgr = new MySQLManager();
#endif

            mgr.Host = strHost;
            mgr.ID = strID;
            mgr.PW = strPW;
            mgr.DBName = strDBName;
            mgr.CharSet = strCharSet;

            return mgr;
        }

        public abstract bool Connect();
        public abstract ArrayList GetResultData(string strSQL);
        public abstract bool BeginBatch();
        public abstract ArrayList GetBatchData(string strSQL);
        public abstract bool BatchCommit();
        public abstract bool BatchRollback();
        public abstract void Close();
        public abstract DirectDBManager Clone();

        protected static bool IsSelectQuery(string strSQL)
        {
            strSQL = strSQL.Trim().ToLower();
            return strSQL.StartsWith("select");
        }

        protected static void AddNullData(ArrayList datas)
        {
            datas.Add("~");
        }

        protected static void AddData(ArrayList datas, object data)
        {
            datas.Add("!" + data.ToString());
        }
    }
}
