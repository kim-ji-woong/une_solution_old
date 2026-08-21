using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data.SqlClient;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace SOPServer
{

	class ConManager
	{
        private MySqlConnection m_dbConnectionMysql;
        private SqlConnection m_dbCoonnection;
		private string m_strConnection;

		// Server Connection Info
		private string m_strServerIP = "";//"127.0.0.1";
		private string m_strServerPort = "";
		private string m_strServerDB = "";

		/// <summary>
		/// UNE
		/// </summary>
		private string m_strServerID = "";
		private string m_strServerPW = "";

        private string m_strServerType = "";
		/// <summary>
		/// 삼천포 DB
		/// </summary>
		//private string m_strServerID = "sa";
		//private string m_strServerPW = "sa1234";

        private DBUtility.Utility m_ini = new DBUtility.Utility();
		private static string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

		public ConManager()
		{
			
			string strSection = "Server Connection Info";
			m_strServerIP = m_ini.getinivalue(strSection, "server_ip");
			m_strServerPort = m_ini.getinivalue(strSection, "server_port");
			m_strServerDB = m_ini.getinivalue(strSection, "server_db");
            m_strServerType = m_ini.getinivalue(strSection,"server_type");
			SOPService.WriteLine("READ IP : " + m_strServerIP);
			try
			{
				string idpass = m_ini.getinivalue(strSection, "dbCon");
				SOPService.WriteLine("READ ENC : " + idpass);
				string strDec = DBUtility.AES256Cipher.AES_decrypt(idpass, key);
				SOPService.WriteLine("READ DEC : " + strDec);
				m_strServerID = strDec.Substring(0, strDec.IndexOf('|'));
				m_strServerPW = strDec.Substring(strDec.IndexOf('|') + 1);

				SOPService.WriteLine("READ ID : "+ m_strServerID);
				SOPService.WriteLine("READ PASS : " + m_strServerPW);
			}
			catch (System.Exception e)
			{
                SOPService.WriteLine(e.Message);
			}
            
			m_strConnection = GetConnectionInfo();
            SOPService.WriteLine(m_strConnection);
            if (m_strServerType == "1")
                m_dbConnectionMysql = new MySqlConnection(m_strConnection);
            else if( m_strServerType == "0")
                m_dbCoonnection = new SqlConnection(m_strConnection);
		}

		private string GetConnectionInfo()
		{
			string strConnection = "";
           // Server = myServerAddress; Database = myDataBase; Uid = myUsername; Pwd = myPassword; CharSet = utf8;
			strConnection = "server=" + m_strServerIP + ";" +
							"database=" + m_strServerDB + ";" +
							"uid=" + m_strServerID + ";" +
							"password=" + m_strServerPW + ";";

			return strConnection;
		}

		public bool OpenConnection()
		{
			try
			{
				m_strConnection = GetConnectionInfo();
                if (m_strServerType == "1")
                {
                    m_strConnection += "CharSet=utf8;";
                    m_dbConnectionMysql = new MySqlConnection(m_strConnection);
                    m_dbConnectionMysql.Open();
                }
                else if (m_strServerType == "0")
                {
                    m_dbCoonnection = new SqlConnection(m_strConnection);
                    m_dbCoonnection.Open();
                }            
				return true;
			}
			catch (Exception e)
			{
                SOPService.WriteLine(e.Message);
				return false;
			}
		}

		public bool CloseConnection()
		{
			try
			{
                if (m_dbCoonnection!= null)
                    m_dbCoonnection.Close();

                if (m_dbConnectionMysql != null)
                    m_dbConnectionMysql.Close();

				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
