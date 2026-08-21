using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

namespace SimulationScenarioMaker
{
    public class DBManager
    {
        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder refval, int size, string filepath);

        string m_strDBName = "";
        public DBManager(string strDBName)
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

        public SqlConnection Connect()
        {
            string strEncrypt2 = GetInValue("ServerInfo", m_strDBName);

            string szKey = new string(new char[] { 'U', 'N', 'E', 'A', 'E', 'S', 'K', 'E', 'Y' });
            string key = "";
            UnE.Utility.Properties.GetProperty(szKey, ref key);
            string strConnection = DBUtility.AES256Cipher.AES_decrypt(strEncrypt2, key);
             
            strConnection = @"" + strConnection;
            SqlConnection conn = new SqlConnection(strConnection);
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message, "DB 연결 에러");
                return null;
            }
            return conn;
        }

        public void ExecuteSQL(string sql)
        {
            SqlConnection conn = Connect();

            if (conn == null)
                return;

            ExecuteSQL(sql, conn);
            conn.Close();
        }

        public void ExecuteSQL(string sql, SqlConnection conn)
        {
            if (conn == null)
                return;
            SqlCommand cmd = new SqlCommand(sql, conn);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message, "DB 연결 에러");
                return;
            }
        }

        // SQL 문을 실행하고, SqlDataReader 객체를 리턴합니다.
        public SqlDataReader ExecuteReader(string sql, SqlConnection conn)
        {
            if (conn == null)
                return null;

            SqlCommand cmd = new SqlCommand(sql, conn);
            try
            {
                return cmd.ExecuteReader();
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message, "DB 연결 에러");
                return null;
            }
        }
    }
}
