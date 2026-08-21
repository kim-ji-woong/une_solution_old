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

using SDMSServer;



namespace SOPServer
{
	public partial class SOPService : ServiceBase
	{
        private static log4net.ILog logger = null;
		//private ControlMonitoring.ControlMonitor monitor = null;
		private System.Timers.Timer tmrTimer = null;
		public SOPService()
		{
			InitializeComponent();
		}

		private NetworkServer server = null;
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

        #pragma warning disable
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
				file = new System.IO.StreamWriter(szFullPath + "//server.log");

			mDBConMan = new ConManager();
			

			tmrTimer = new System.Timers.Timer();
			tmrTimer.Interval = 2000;
			tmrTimer.Elapsed += tmrTimer_Elapsed;
			tmrTimer.Start();
		}

		protected override void OnStop()
		{	

			tmrTimer.Stop();
			tmrTimer.Enabled = false;

			if( server != null)
			{
				server.NetworkServerClosing();
			}
			
			/*if( monitor != null)
			{
				monitor.Stop();
			}*/

			if (file != null)
			{
				file.Close();
			}
		}
		private ConManager mDBConMan = null;
		void tmrTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
            SOPService.WriteLine("DB Wait Timer");
			if (mDBConMan.OpenConnection())
			{
				SOPService.WriteLine("Open Connection");
				tmrTimer.Stop();
				tmrTimer.Enabled = false;

				server = new NetworkServer();
				server.NetworkServerLoad();
				//monitor = new ControlMonitoring.ControlMonitor();
				//monitor.Start();

				mDBConMan.CloseConnection();
			}			
		}   
	}
}
