using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using DBUtility;

namespace SVMSEventReciver
{
	internal class DBConnectionManager
	{   	
		private string m_strConnection;

		// Server Connection Info
		private string m_strServerIP = "";//"127.0.0.1";
		private string m_strServerPort = "";
		private string m_strServerDB = "";
        private string m_strDBType = "1";
		/// <summary>
		/// UNE
		/// </summary>
		private string m_strServerID = "";
		private string m_strServerPW = "";

		/// <summary>
		/// 삼천포 DB
		/// </summary>
		//private string m_strServerID = "sa";
		//private string m_strServerPW = "sa1234";

		private Utility m_ini = new Utility();
		private static string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

        public DBConnectionManager()
		{
			
			string strSection = "Server Connection Info";
			m_strServerIP = m_ini.getinivalue(strSection, "server_ip");
			m_strServerPort = m_ini.getinivalue(strSection, "server_port");
			m_strServerDB = m_ini.getinivalue(strSection, "server_db");
           
			try
			{
				string idpass = m_ini.getinivalue(strSection, "dbCon");
                //GasLevelMeterServer.WriteLine("READ ENC : " + idpass);
				string strDec = DBUtility.AES256Cipher.AES_decrypt(idpass, key);
               // GasLevelMeterServer.WriteLine("READ DEC : " + strDec);
				m_strServerID = strDec.Substring(0, strDec.IndexOf('|'));
				m_strServerPW = strDec.Substring(strDec.IndexOf('|') + 1);

			}
			catch (System.Exception e)
			{
               
			}            
			m_strConnection = GetConnectionInfo();
          
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

	}
}
