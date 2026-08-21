using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data.SqlClient;
using DBUtility;

namespace RestoreService
{

	class DBConManager
	{
		private SqlConnection m_dbConnection;
		public SqlConnection Connection
		{
			get { return m_dbConnection; }
		}

		private string m_strConnection;

		private string m_strServerIP = "";
		private string m_strServerPort = "";
		private string m_strServerDB = "";

		private string m_strServerID = "";
		private string m_strServerPW = "";
		private static string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

		public DBConManager()
		{
			WebDBManager dbMgr = NetworkManager.Instance.DBManager;
			string strSection = "Server Connection Info";
			m_strServerIP = dbMgr.LoadIni("server_ip", strSection);
			m_strServerPort = dbMgr.LoadIni("server_port", strSection);
			m_strServerDB = dbMgr.LoadIni("server_db", strSection); 

			try
			{
				string idpass = dbMgr.LoadIni("dbCon", strSection);
				string strDec = DBUtility.AES256Cipher.AES_decrypt(idpass, key);				
				m_strServerID = strDec.Substring(0, strDec.IndexOf('|'));
				m_strServerPW = strDec.Substring(strDec.IndexOf('|') + 1);
				
			}
			catch (System.Exception)
			{
			}

			m_strConnection = GetConnectionInfo();
			m_dbConnection = new SqlConnection(m_strConnection);
		}

		private string GetConnectionInfo()
		{
			string strConnection = "";

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
				m_dbConnection = new SqlConnection(m_strConnection);
				m_dbConnection.Open();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public bool CloseConnection()
		{
			try
			{
				m_dbConnection.Close();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
