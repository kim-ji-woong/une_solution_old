using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace RunManager
{
    public class DBManagerMySQL : DBManager
    {
        private MySqlConnection m_dbConnection = null;

        public DBManagerMySQL()
        {
        }

        protected override void MakeConnection()
        {
            char[] arrID = new char[] { 'r', 'o', 'o', 't' };
            char[] arrPW = new char[] { 'l', 'i', 'b', '1', '!', '#', '%', '&', '(' };
            
            m_strServerID = new string(arrID);
            m_strServerPW = new string(arrPW);

            // DB 열기
            Loadini_ServerConnectionInfo();
            m_strConnection = GetStringConnection();
            m_dbConnection = new MySqlConnection(m_strConnection);

            m_isConnection = OpenConnection();
        }

        public void ReadDB(string strSQL, object transaction, out MySqlDataReader reader)
        {
            MySqlCommand cmd = new MySqlCommand(strSQL, m_dbConnection);
            reader = cmd.ExecuteReader();
        }

        public void Execute(string strSQL, object transaction = null)
        {
            MySqlCommand cmd = new MySqlCommand(strSQL, m_dbConnection);
            cmd.ExecuteNonQuery();
        }

        public override bool OpenConnection()
        {
            try
            {
                m_dbConnection.Open();
                return true;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                System.Windows.Forms.Application.Exit();
                return false;
            }
        }

        //Close connection
        public override bool CloseConnection()
        {
            try
            {
                m_isConnection = false;
                m_dbConnection.Close();
                return true;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                System.Windows.Forms.Application.Exit();
                return false;
            }
        }
    }
}
