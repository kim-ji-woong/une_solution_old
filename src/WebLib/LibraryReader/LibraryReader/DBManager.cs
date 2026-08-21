using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;
using System.Collections;
using System.Data.SqlClient;

namespace LibraryReader
{
    public class DBManager
    {
        // Server Connection Info
        protected string m_strServerIP = "";
        protected string m_strServerPort = "";
        protected string m_strServerDB = "";
        protected string m_strServerID = "";
        protected string m_strServerPW = "";
        protected string m_strOutExe = "";
        protected string m_strOutName = "";   // m_strOutExe에서 .ext 부분을 뺀 파일 이름
        protected ArrayList m_arrParamFiles = new ArrayList();

        protected int m_nLevel = -1;

        // Path Data Info
        protected string m_strServerURL;

        protected string m_strConnection;
        protected bool m_isConnection = false;

        protected Utility m_ini = new Utility();
        private SqlConnection m_dbConnection;

        public DBManager()
        {
            MakeConnection();
        }

        protected virtual void MakeConnection()
        {
            char[] arrID = new char[] { 'l', 'i', 'b', 'r', 'a', 'r', 'y', '2', '0', '1', '2' };
            char[] arrPW = new char[] { 'l', 'i', 'b', 'r', 'a', 'r', 'y', '2', '0', '1', '2' };

            m_strServerID = new string(arrID);
            m_strServerPW = new string(arrPW);

            // DB 열기
            Loadini_ServerConnectionInfo();
            m_strConnection = GetStringConnection();
            m_dbConnection = new SqlConnection(m_strConnection);

            m_isConnection = OpenConnection();
        }

        public bool IsOpened
        {
            get { return m_isConnection; }
        }

        public string OutExePath
        {
            get { return m_strOutExe; }
        }

        public string OutExeName
        {
            get { return m_strOutName; }
        }

        public ArrayList ParamFiles
        {
            get { return m_arrParamFiles; }
        }

        //{{ get, set
        public string ServerIP
        {
            get { return m_strServerIP; }
            set { m_strServerIP = value; }
        }

        public string ServerPort
        {
            get { return m_strServerPort; }
            set { m_strServerPort = value; }
        }

        public string ServerDB
        {
            get { return m_strServerDB; }
            set { m_strServerDB = value; }
        }

        public string ServerID
        {
            get { return m_strServerID; }
            set { m_strServerID = value; }
        }

        public string ServerPW
        {
            get { return m_strServerPW; }
            set { m_strServerPW = value; }
        }

        // User 권한
        public int Level
        {
            get { return m_nLevel; }
            set { m_nLevel = value; }
        }

        public string ServerURL
        {
            get { return m_strServerURL; }
            set { m_strServerURL = value; }
        }

        public T GetField<T>(object dataSrc, T dataDefault)
        {
            T result;

            try
            {
                result = (T)dataSrc;
            }
            catch (Exception)
            {
                result = dataDefault;
            }

            return result;
        }

        public DateTime GetDateTimeField(object dataSrc, DateTime dtDefault)
        {
            DateTime result;

            try
            {
                result = Convert.ToDateTime(dataSrc);
            }
            catch (Exception)
            {
                result = dtDefault;
            }

            return result;
        }

        // 문자열 앞뒤의 빈문자들을 제거한다.
        public string GetStringField(object dataSrc, string strDefault)
        {
            string result;

            try
            {
                result = (string)dataSrc;
                result = result.TrimStart(new char[] { ' ', '\t', '\r', '\n' });
                result = result.TrimEnd(new char[] { ' ', '\t', '\r', '\n' });
            }
            catch (Exception)
            {
                result = strDefault;
            }

            return result;
        }

        public void Execute(string strSQL, SqlTransaction transaction = null)
        {
            SqlCommand cmd = new SqlCommand(strSQL, m_dbConnection);
            if (transaction != null) cmd.Transaction = transaction;
            cmd.ExecuteNonQuery();
        }

        public void ReadDB(string strSQL, SqlTransaction transaction, out SqlDataReader reader)
        {
            SqlCommand cmd = new SqlCommand(strSQL, m_dbConnection);
            if (transaction != null) cmd.Transaction = transaction;
            reader = cmd.ExecuteReader();
        }

        public void RunStoredProcedure(string strProcName, ArrayList arrFields, ArrayList arrValues, SqlTransaction transaction, out SqlDataReader reader)
        {
            reader = null;

            int nFieldCount = arrFields.Count;
            int nValueCount = arrValues.Count;
            if (nFieldCount != nValueCount) return;

            SqlCommand cmd = new SqlCommand(strProcName, m_dbConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            if (transaction != null) cmd.Transaction = transaction;

            for (int i = 0; i < nFieldCount; i++)
            {
                cmd.Parameters.Add(new SqlParameter((string)arrFields[i], (string)arrValues[i]));
            }

            reader = cmd.ExecuteReader();
        }

        // 해당문자열을 ``으로 감싸서 반환한다 (strQuary:DB이름이나 필드명)
        public string Grave(object obj)
        {
            return "`" + obj.ToString() + "`";
        }

        public string GetStringConnection()
        {
            string strConnection = "";

            strConnection = "server=" + ServerIP + ";" +
                            "database=" + ServerDB + ";" +
                            "port=" + ServerPort + ";" + 
                            "uid=" + ServerID + ";" +
                            "password=" + ServerPW + ";";

            return strConnection;
        }

        public void Loadini_ServerConnectionInfo()
        {
            string strSection = "Connection Info";

            ServerIP = m_ini.getinivalue(strSection, "db_url");
            ServerPort = m_ini.getinivalue(strSection, "db_port");
            ServerDB = m_ini.getinivalue(strSection, "db_name");
            m_strOutExe = m_ini.getinivalue(strSection, "out_exe");

            string strParamFiles = m_ini.getinivalue(strSection, "param_files");
            SetParamFiles(strParamFiles);

            int nIndex = m_strOutExe.LastIndexOf('.');
            if (nIndex >= 0)
                m_strOutName = m_strOutExe.Substring(0, nIndex);
        }

        private void SetParamFiles(string strParamFiles)
        {
            int nLen = strParamFiles.Length;
            int nBeginIndex = -1;

            for (int i = 0; i < nLen; i++)
            {
                char ch = strParamFiles.ElementAt(i);

                if (ch == ' ' || ch == '\t' || ch == '\r' || ch == '\n')
                {
                    if (nBeginIndex >= 0)
                    {
                        string strFileName = strParamFiles.Substring(nBeginIndex, i - nBeginIndex);
                        m_arrParamFiles.Add(strFileName);
                        nBeginIndex = -1;
                    }
                }
                else
                {
                    if (nBeginIndex < 0)
                        nBeginIndex = i;
                }
            }

            if (nBeginIndex >= 0)
            {
                string strFileName = strParamFiles.Substring(nBeginIndex);
                m_arrParamFiles.Add(strFileName);
            }
        }

        public virtual bool OpenConnection()
        {
            try
            {
                m_dbConnection.Open();
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                System.Windows.Forms.Application.Exit();
                return false;
            }
        }

        //Close connection
        public virtual bool CloseConnection()
        {
            try
            {
                m_isConnection = false;
                m_dbConnection.Close();
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                System.Windows.Forms.Application.Exit();
                return false;
            }
        }
    }
}
