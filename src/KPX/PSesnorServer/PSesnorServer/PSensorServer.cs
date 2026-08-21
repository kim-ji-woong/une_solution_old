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

namespace PSensorServer
{
	public partial class PSensorServer : ServiceBase
	{
        private static log4net.ILog logger = null;
		private System.Timers.Timer tmrTimer = null;
        public PSensorServer()
		{
			InitializeComponent();
		}

		//private S1NetworkServer server = null;
        private JubixNetworkClient client = null;

		private static StreamWriter file = null;

		private static bool bEnableLog = false;

        private UnE.Log.LogFileCleanupTask m_CleanUpTask;

		public static void WriteLine(string szMsg)
		{
            if (bEnableLog == true && file != null)
            {
                file.WriteLine(szMsg);
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

            try
            {
                m_CleanUpTask = new UnE.Log.LogFileCleanupTask();
                m_CleanUpTask.CleanUp();
                m_CleanUpTask.BeginDailyTask(m_CleanUpTask.CleanUp);
            }
            catch(Exception)
            {
            }


            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(ExceptionHandler);

			string szPath = Assembly.GetEntryAssembly().Location;
			string szFullPath = Directory.GetParent(szPath).FullName;
			
			if (bEnableLog == true)
				file = new System.IO.StreamWriter(szFullPath + "//KPXPipeSensor2.log");

            DBUtility.WebDBManager dbMgr = KPXServerManager.Instance.DBManager;
            KPXServerManager.Instance.BeginCommander();
            string szIP = KPXServerManager.Instance.LoggerIP;
            client = new JubixNetworkClient(dbMgr, szIP, KPXServerManager.Instance.SiteID);
		}

		protected override void OnStop()
		{
            try
            {
                KPXServerManager.Instance.StopCommander();
            }
            catch(Exception)
            { }
            

            try
            {
                if (client != null)
                {
                    if (client.ClientProvider != null && client.ClientProvider.IsClientDisposed != false)
                        client.ClientProvider.Close();
                    client.ShutdownSensorThread = true;
                }
            }
            catch(Exception)
            { }           
				
	
            if( client != null)
            {
                client.ReleaseThread();
            }

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
        
	}
}
