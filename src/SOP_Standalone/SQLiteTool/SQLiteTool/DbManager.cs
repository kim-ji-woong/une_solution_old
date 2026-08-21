using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteTool
{
    public class DbManager
    {
        private string m_strFilePath = "";
        private SQLiteConnection conn = null;

        public DbManager()
        {

        }

        public bool Connect(string filePath, string pw = "")
        {
            m_strFilePath = filePath;

            string connectionString = "Data Source=" + m_strFilePath + ";Version=3;";
            if (pw.Length > 0)
                connectionString += "Password=" + pw;

            conn = new SQLiteConnection(connectionString);
            conn.Open();

            bool suc = ConnectionTest();
            return suc;
        } 

        private bool ConnectionTest()
        {
            try
            {
                DataTable dt = GetDataTable("SELECT ID FROM SITE");
                if (dt == null)
                    return false;
                else
                    return true;
            }
            catch (Exception)
            {
                return false;
            }         
        }

        public bool ChgPw(string pw = "")
        {
            bool suc = false;

            try
            {
                if (pw.Length == 0)
                    conn.ChangePassword(String.Empty);
                else
                    conn.ChangePassword(pw);

                Close();
                if (pw.Length == 0)
                    suc = Connect(m_strFilePath, "");
                else
                    suc = Connect(m_strFilePath, pw);
            }
            catch (Exception)
            {
                return suc;
            }
            return suc;
        }

        private DataSet GetDataSet(string query)
        {
            DataSet dataSet = new DataSet();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter();
            adapter.SelectCommand = new SQLiteCommand(query, conn);
            adapter.Fill(dataSet);

            return dataSet;
        }

        public DataTable GetDataTable(string query)
        {
            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
            {
                cmd.CommandType = CommandType.Text;
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
                {
                    using (DataTable dt = new DataTable())
                    {
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public void Close()
        {
            if (conn != null)
                conn.Close();
        }
    }
}
