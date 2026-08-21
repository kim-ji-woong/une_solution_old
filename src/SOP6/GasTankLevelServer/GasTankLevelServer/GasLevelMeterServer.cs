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

namespace GasLevelServer
{
	public partial class GasLevelMeterServer : ServiceBase
	{
        private static log4net.ILog logger = null;
		private System.Timers.Timer tmrTimer = null;
		public GasLevelMeterServer()
		{
			InitializeComponent();
		}

		private LevelMeterNetworkServer server = null;
        private NetworkClient client = null;
        private LevelMeterManager sensor = null;

		private static StreamWriter file = null;

		private static bool bEnableLog = true;
		public static void WriteLine(string szMsg)
		{
			if (bEnableLog == true && file != null)
				file.WriteLine(szMsg);
		}

        static void ExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception ex = (Exception)args.ExceptionObject;

            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
            logger.Debug("프로그램 오류", ex);
            logger.Debug("Line: " + trace.GetFrame(0).GetFileLineNumber());

        } 
        
        private UnE.Log.LogFileCleanupTask m_CleanUpTask = null;

		protected override void OnStart(string[] args)
		{
            try
            {
                log4net.Config.DOMConfigurator.Configure();

                m_CleanUpTask = new UnE.Log.LogFileCleanupTask();
                m_CleanUpTask.CleanUp();
                //m_CleanUpTask.BeginDailyTask(m_CleanUpTask.CleanUp);

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
                file = new System.IO.StreamWriter(szFullPath + "//ServerRun.log");

			mDBConMan = new ConManager();
			

			tmrTimer = new System.Timers.Timer();
			tmrTimer.Interval = 2000;
			tmrTimer.Elapsed += tmrTimer_Elapsed;
			tmrTimer.Start();
		}

		protected override void OnStop()
		{
            if (client != null)
            {
                if (client.ClientProvider.IsConnected == true)
                    client.ClientProvider.Close();  
            }

			tmrTimer.Stop();
			tmrTimer.Enabled = false;

			if( server != null)
			{
				server.NetworkServerClosing();
			}		
	
            if( client != null)
            {
                client.ReleaseThread();
            }

            if (sensor != null)
                sensor.StopServer();

			if (file != null)
			{
				file.Close();
			}


		}
		private ConManager mDBConMan = null;
		void tmrTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
            GasLevelMeterServer.WriteLine("DB Wait Timer");
			if (mDBConMan.OpenConnection())
			{
				GasLevelMeterServer.WriteLine("Open Connection");
				tmrTimer.Stop();
				tmrTimer.Enabled = false;


                server = new LevelMeterNetworkServer();

                WebDBManager dbMgr = LevelMeterNetworkServer.Instance.DBManager;
         
                client = new NetworkClient(dbMgr, null, LevelMeterNetworkServer.Instance.SiteID);
                sensor = new LevelMeterManager(client);

				server.NetworkServerLoad();
                sensor.BeginServer(GasDetector_OnNotifyAlarm);

				mDBConMan.CloseConnection();
			}
		}

        void GasDetector_OnNotifyAlarm(int nComm, int nAlarmUnit, float fValue, int nChannel, int nStatus)
        {

        }
	}
}
