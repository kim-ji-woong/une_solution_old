using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using DBUtility;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;
using System.Net;

namespace RestoreService
{
	public class RestoreManager
	{
		private string m_szTomcatName;
		private string m_szDatabaseName;
		private string m_szSOPServerName = "SOPServer";
		private string m_szBroadcastServerName = "BroadcastServer";
		private string m_szSensorMonitorName = "SensorMonitor";
		


		private string m_szMDFPath = "";

		private DBConManager m_ConnMan = null;
		
		public RestoreManager()
		{
		}

		private string GetLocationMDF()
		{
			WebDBManager dbMgr = NetworkManager.Instance.DBManager;
			string szSQL = "SELECT filename FROM master.sys.sysfiles";

			ArrayList arrResult = dbMgr.GetResultData(szSQL, 0);
			if (arrResult == null)
				return "";

			int nCount = arrResult.Count;
			if (nCount == 0)
				return "";

			m_szMDFPath = WebDBManager.GetStringField(arrResult[0], "");
			if( m_szMDFPath == "")
				return "";

			int nIdx = m_szMDFPath.LastIndexOf("\\");
			if (nIdx <= 0)
				return "";

			m_szMDFPath = m_szMDFPath.Substring(0, nIdx);
			return m_szMDFPath;
		}

		private bool ExistRestoreDB()
		{
			WebDBManager dbMgr = NetworkManager.Instance.DBManager;
			string szSQL = "SELECT name FROM master.sys.databases;";

			ArrayList arrResult = dbMgr.GetResultData(szSQL, 0);
			if (arrResult == null || arrResult.Count == 0)
				return false;

			for (int i = 0; i < arrResult.Count; i++)
			{
				string szName = WebDBManager.GetStringField(arrResult[i], "");
				if (szName == "SOP3_RESTORE")
				{
					return true;
				}
			}
			return false;
		}

		private System.Timers.Timer tmrTimer = null;
		private bool bCheckSQL = false;

		private void CheckTomcatServer()
		{
			WebDBManager dbMgr = NetworkManager.Instance.DBManager;
			string szURL = dbMgr.WebServerURL + "/Download.jsp";

			while (true)
			{
				try
				{
					WebRequest request = WebRequest.Create(szURL);
					HttpWebResponse response = (HttpWebResponse)request.GetResponse();
					if (response == null || response.StatusCode != HttpStatusCode.OK)
					{

					}
					else
					{
						break;						
					}
				}
				catch (System.Exception)
				{					
				}

				ServiceManager.StartService(m_szTomcatName, 0);
				Thread.Sleep(500);
			}
			
		}

		private void CheckSOPServerConnect()
		{
			while (true)
			{
				try
				{
					if (NetworkManager.Instance.SerivceProvider != null)
					{
						if (NetworkManager.Instance.SerivceProvider.IsConnected)
							break;
					}
				}
				catch (System.Exception)
				{
				}
				Thread.Sleep(500);
			}
		}

		private void CheckSQLServerConnect()
		{
			if (m_ConnMan == null)
				m_ConnMan = new DBConManager();
			tmrTimer = new System.Timers.Timer();

			tmrTimer.Interval = 1000;
			tmrTimer.Elapsed += tmrTimer_Elapsed;
			tmrTimer.Start();
			bCheckSQL = false;
			
			while (bCheckSQL == false)
			{
				ServiceManager.StartService(m_szDatabaseName, 0);
				Thread.Sleep(500);
			}
		}

		private void tmrTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			m_ConnMan = new DBConManager();
			if (m_ConnMan.OpenConnection())
			{				
				tmrTimer.Stop();
				tmrTimer.Enabled = false;
				bCheckSQL = true;
				m_ConnMan.CloseConnection();
			}
		}
		
		
		public void RestoreProcess()
		{
			m_szTomcatName = ServiceManager.FindServiceName("tomcat");
			m_szDatabaseName = ServiceManager.FindServiceName("MSSQL$");


			if (ExistRestoreDB() == false)
				return;

			if (GetLocationMDF() == "")
				return;

			ServiceManager.StopService(m_szBroadcastServerName, 1000);

			ServiceManager.StopService(m_szSensorMonitorName, 1000);

			ServiceManager.StopService(m_szSOPServerName, 1000);

            string szPsmService = ServiceManager.FindServiceName("GasSensor");
            if (szPsmService != "")
            {
                ServiceManager.StopService(szPsmService, 1000);
            }

            string szGasPlcService = ServiceManager.FindServiceName("GasLevel");
            if (szGasPlcService != "")
            {
                ServiceManager.StopService(szGasPlcService, 1000);
            }

			if (RestoreDatabase())
			{
				ServiceManager.StopService(m_szDatabaseName, 0);
				ServiceManager.StartService(m_szDatabaseName, 0);

				// SQL서버가 접속 가능상태일때까지 대기
				CheckSQLServerConnect();

				ServiceManager.StartService(m_szTomcatName, 0);
				// 톰켓 서버가 접속 가능상태일때까지 대기
				CheckTomcatServer();

				ServiceManager.StartService(m_szSOPServerName, 0);
				// SOP서버가 접속 상태 일때까지 대기	
				CheckSOPServerConnect();
			}
			else
			{
				ServiceManager.StartService(m_szTomcatName, 1000);
				CheckTomcatServer();
				
				ServiceManager.StartService(m_szSOPServerName, 1000);
				CheckSOPServerConnect();
			}


            if (szGasPlcService != "")
            {
                ServiceManager.StartService(szGasPlcService, 1000);
            }

            if (szPsmService != "")
            {
                ServiceManager.StartService(szPsmService, 1000);
            }

            ServiceManager.StartService(m_szSensorMonitorName, 1000);
			
		}

		public void PostRestoreProcess()
		{
			ServiceManager.StartService(m_szBroadcastServerName, 0);
			ServiceManager.StartService(m_szSensorMonitorName, 0);
		}
		
		private bool RestoreDatabase()
		{	

			ServiceManager.StopService(m_szTomcatName, 1000);
			
			StringBuilder sb = new StringBuilder();
			string szDBName = string.Format("SOP3_BACKUP_{0}", DateTime.Now.ToString("yyyyMMdd_HHmmss"));
			szDBName = szDBName.Replace("-", "_");

			sb.AppendLine("USE MASTER;");
			sb.AppendLine("IF EXISTS (SELECT name FROM sys.databases WHERE name = N'SOP3_RESTORE')");
			sb.AppendLine("BEGIN");

			string szBuff = string.Format("BACKUP DATABASE SOP3_RESTORE TO DISK = '{0}\\SOP3_RESTORE.BAK' WITH INIT;", m_szMDFPath);
			sb.AppendLine(szBuff);
			sb.AppendLine("ALTER DATABASE SOP3_RESTORE SET single_user WITH rollback immediate;");
			sb.AppendLine("DROP DATABASE SOP3_RESTORE;");
			sb.AppendLine("END");
			//sb.AppendLine("GO");

			string szBuff1 = string.Format("IF EXISTS (SELECT name FROM sys.databases WHERE name = N'{0}')", szDBName);
			sb.AppendLine(szBuff1);
			sb.AppendLine("BEGIN");
			szBuff1 = string.Format("DROP DATABASE {0};", szDBName);
			sb.AppendLine(szBuff1);
			sb.AppendLine("END");
			//sb.AppendLine("GO");
            sb.AppendLine("IF EXISTS (SELECT name FROM sys.databases WHERE name = N'SOP_1')");
			sb.AppendLine("BEGIN");

            string szBuff2 = string.Format("BACKUP DATABASE SOP_1 TO DISK = '{0}\\{1}.BAK' WITH INIT;", m_szMDFPath, szDBName);
			sb.AppendLine(szBuff2);
            sb.AppendLine("ALTER DATABASE SOP_1 SET single_user WITH rollback immediate;");
            sb.AppendLine("DROP DATABASE SOP_1;");


			string szBuff3 = string.Format("RESTORE database {1} FROM DISK = '{0}\\{1}.BAK' " +
                "with move 'SOP_1' to '{0}\\{1}.mdf', move 'SOP_1_log' to '{0}\\{1}_log.ldf';", m_szMDFPath, szDBName);
			sb.AppendLine(szBuff3);
            szBuff3 = string.Format("ALTER DATABASE {0} MODIFY FILE (NAME = SOP_1, NEWNAME = {0});", szDBName);
			sb.AppendLine(szBuff3);
            szBuff3 = string.Format("ALTER DATABASE {0} MODIFY FILE (NAME = SOP_1_log, NEWNAME = {0}_log);", szDBName);
			sb.AppendLine(szBuff3);
			sb.AppendLine("END");
			//sb.AppendLine("GO");
            string szBuff4 = string.Format("RESTORE DATABASE SOP_1 FROM DISK = '{0}\\SOP3_RESTORE.BAK' " +
                "with move 'SOP3_RESTORE' to '{0}\\SOP_1.mdf', move 'SOP3_RESTORE_log' to '{0}\\SOP_1_log.ldf';", m_szMDFPath);
			sb.AppendLine(szBuff4);
            sb.AppendLine("ALTER DATABASE SOP_1 MODIFY FILE (NAME = SOP3_RESTORE, NEWNAME = SOP_1);");
            sb.AppendLine("ALTER DATABASE SOP_1 MODIFY FILE (NAME = SOP3_RESTORE_log, NEWNAME = SOP_1_log);");
            sb.AppendLine("ALTER DATABASE SOP_1 SET MULTI_USER;");

			string szSQL = sb.ToString();
			
			m_ConnMan = new DBConManager();
			if (m_ConnMan.OpenConnection())
			{
				try
				{
					SqlConnection conn = m_ConnMan.Connection;
					SqlCommand cmd = conn.CreateCommand();
					cmd.CommandText = szSQL;
					cmd.ExecuteNonQuery();
					m_ConnMan.CloseConnection();
				}
				catch (System.Data.SqlClient.SqlException ex)
				{
					Debug.WriteLine(ex.Message);
					Debug.WriteLine(ex.StackTrace);
				}
				return true;
			}
			return false;
		}
	}
}