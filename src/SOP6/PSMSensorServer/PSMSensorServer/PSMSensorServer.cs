using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Reflection;
using System.IO;
using DBUtility2;

namespace PSMSensorServer
{
	public partial class PSMSensorServer : ServiceBase
	{
        private static log4net.ILog logger = null;
		private System.Timers.Timer tmrTimer = null;
		public PSMSensorServer()
		{
			InitializeComponent();
		}

		private PSMNetworkServer server = null;
        private NetworkWebClient m_clientWeb = null;
        private PSMSensorManager sensor = null;

        private WebDBManager m_dbMgr = null;

		private static StreamWriter file = null;

		private static bool bEnableLog = true;

		public static void WriteLine(string szMsg)
		{
            if (bEnableLog == true && file != null)
            {
                DateTime dtNow = DateTime.Now;
                string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);

                file.WriteLine(strTime + " : " + szMsg);
                file.Flush();
            }
		}

        static void ExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception ex = (Exception)args.ExceptionObject;

            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
            logger.Debug("프로그램 오류", ex);
            logger.Debug("Line: " + trace.GetFrame(0).GetFileLineNumber());

        } 

		protected override void OnStart(string[] args)
		{
			try
			{
				log4net.Config.DOMConfigurator.Configure();
			}
			catch (System.Exception)
			{

			}

            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(ExceptionHandler);

			string szPath = Assembly.GetEntryAssembly().Location;
			string szFullPath = Directory.GetParent(szPath).FullName;
			
			if (bEnableLog == true)
				file = new System.IO.StreamWriter(szFullPath + "//SensorServer2.log");

            string strSiteID = System.Configuration.ConfigurationManager.AppSettings["SiteID"].ToString();
            string strWebServerURL = System.Configuration.ConfigurationManager.AppSettings["WebServerURL"].ToString();
            string strDBName = System.Configuration.ConfigurationManager.AppSettings["DBName"].ToString();
            int nSiteID = int.Parse(strSiteID);

            file.WriteLine(nSiteID);

            m_dbMgr = new WebDBManager(nSiteID);
            m_dbMgr.DatabaseName = strDBName;
            m_dbMgr.WebServerURL = strWebServerURL;
            
            file.WriteLine(m_dbMgr.DatabaseName);
            file.WriteLine(m_dbMgr.WebServerURL);

            tmrTimer = new System.Timers.Timer();
			tmrTimer.Interval = 2000;
			tmrTimer.Elapsed += tmrTimer_Elapsed;
			tmrTimer.Start();
        }

		protected override void OnStop()
		{	
			tmrTimer.Stop();
			tmrTimer.Enabled = false;

            try
            {
                /*if (client != null)
                {
                    if (client.ClientProvider != null && client.ClientProvider.IsClientDisposed != false)
                        client.ClientProvider.Close();
                    client.ShutdownSensorThread = true;
                }*/

                if (m_clientWeb != null)
                {
                    m_clientWeb.Close();
                }
            }
            catch(Exception)
            { }
           

            if (sensor != null)
                sensor.StopServer();


			if( server != null)
			{
				server.NetworkServerClosing();
			}		
	
            /*if( client != null)
            {
                client.ReleaseThread();
            }*/

            try
            {
                if (file != null)
                {
                    file.Close();
                }
            }
            catch (Exception)
            { }
		}

        void GasDetector_OnNotifyAlarm(int nComm, int nAlarmUnit, float fValue, int nChannel, int nStatus)
        {
            /*if (client != null)
            {
                if (nStatus == 1)
                {
                    client.SendSensorData(nComm, nAlarmUnit, nChannel, (nChannel + 1), true);
                }
                else
                {
                    client.SendSensorData(nComm, nAlarmUnit, nChannel, nStatus, true);
                }

                System.Diagnostics.Trace.WriteLine("Alarm : " + nComm + "," + nAlarmUnit + "," + fValue + "," + nChannel + "," + nStatus);
            }*/

            if (m_clientWeb != null)
            {
                if (nStatus == 1)
                {
                    m_clientWeb.SendSensorData(nComm, nAlarmUnit, nChannel, (nChannel + 1), true);
                }
                else
                {
                    m_clientWeb.SendSensorData(nComm, nAlarmUnit, nChannel, nStatus, true);
                }

                System.Diagnostics.Trace.WriteLine("Alarm : " + nComm + "," + nAlarmUnit + "," + fValue + "," + nChannel + "," + nStatus);
            }
        }

		void tmrTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
            PSMSensorServer.WriteLine("DB Wait Timer");
            // Just DB Check
            ArrayList arrResult = m_dbMgr.GetResultData("Select * from Site");

            if (arrResult != null)
			//if (mDBConMan.OpenConnection())
			{
				PSMSensorServer.WriteLine("Open Connection");
				tmrTimer.Stop();
				tmrTimer.Enabled = false;

                server = new PSMNetworkServer(m_dbMgr);
                
                m_clientWeb = new NetworkWebClient(m_dbMgr);

                sensor = new PSMSensorManager(/*client, */m_clientWeb);

                // 새로 접속하니까 일단 모두 접속이 끊긴 것으로 초기화
                sensor.SaveAllSensorServerInfo(false);

                server.NetworkServerLoad();

                sensor.BeginServer(GasDetector_OnNotifyAlarm);

                //mDBConMan.CloseConnection();
			}
            else
            {
                PSMSensorServer.WriteLine("DB Error : " + m_dbMgr.LastErrorMessage);
            }
		}   
	}
}
