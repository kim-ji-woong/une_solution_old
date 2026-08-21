using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

namespace HSMS
{
    public class DBConn
    {
        
        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder refval, int size, string filepath);

        private string m_strDBName = "";
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DBConn(string strDBName)
        {
            m_strDBName = strDBName;
        }

        public static string GetInValue(string section, string key)
        {
            StringBuilder temp = new StringBuilder(255);
            string strPath = Application.StartupPath + "\\Config.ini";
            GetPrivateProfileString(section, key, "", temp, 255, strPath);

            return temp.ToString();
        }

        public static string Key
        {
            get
            {
                string szKey = new string(new char[] { 'U', 'N', 'E', 'A', 'E', 'S', 'K', 'E', 'Y' });
                string key = "";
                UnE.Utility.Properties.GetProperty(szKey, ref key);
                return key;
            }
        }

        public SqlConnection Connect()
        {           
            string strEncrypt2 = GetInValue("ServerInfo", m_strDBName);

            /*string szKey = new string(new char[] { 'U', 'N', 'E', 'A', 'E', 'S', 'K', 'E', 'Y' });
            string key = "";
            UnE.Utility.Properties.GetProperty(szKey, ref key);*/
            string key = Key;
            string strConnection = DBUtility.AES256Cipher.AES_decrypt(strEncrypt2, key);

            strConnection = @"" + strConnection;
            SqlConnection conn = new SqlConnection(strConnection);
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(e, true);
                logger.Debug("DB연결오류", e);
                logger.Debug("Line: " + trace.GetFrame(0).GetFileLineNumber());

                return null;
            }
            return conn;
        }

        public void ExecuteSQL(string sql, SqlConnection conn, SqlTransaction tranc = null)
        {
            //SqlConnection conn = Connect();
            if (conn == null)
                return;
            SqlCommand cmd = new SqlCommand(sql, conn);
            if (tranc != null)
                cmd.Transaction = tranc;
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(e, true);
                logger.Debug("DB연결오류", e);
                logger.Debug("Line: " + trace.GetFrame(0).GetFileLineNumber());
                return;
            }
        }

        // SQL 문을 실행하고, SqlDataReader 객체를 리턴합니다.
        public SqlDataReader ExecuteReader(string sql, SqlConnection conn, SqlTransaction tranc = null)
        {
            if (conn == null)
                return null;

            SqlCommand cmd = new SqlCommand(sql, conn);
            if (tranc != null)
                cmd.Transaction = tranc;
            try
            {
                return cmd.ExecuteReader();
            }
            catch (Exception e)
            {
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(e, true);
                logger.Debug("DB연결오류", e);
                logger.Debug("Line: " + trace.GetFrame(0).GetFileLineNumber());
                return null;
            }
        }
    }
}
