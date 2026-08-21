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

namespace S1SensorServer
{
	public partial class S1SensorServer : ServiceBase
	{
        private static log4net.ILog logger = null;
		private System.Timers.Timer tmrTimer = null;
        public S1SensorServer()
		{
			InitializeComponent();
		}

		private S1NetworkServer server = null;
        private NetworkClient client = null;

		private static StreamWriter file = null;

		private static bool bEnableLog = true;
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

            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(ExceptionHandler);

			string szPath = Assembly.GetEntryAssembly().Location;
			string szFullPath = Directory.GetParent(szPath).FullName;
			
			if (bEnableLog == true)
				file = new System.IO.StreamWriter(szFullPath + "//s1SensorServer2.log");

            server = new S1NetworkServer();
            DBUtility.WebDBManager dbMgr = S1NetworkServer.Instance.DBManager;
            client = new NetworkClient(dbMgr, null, S1NetworkServer.Instance.SiteID);
            server.NetworkServerLoad();

		}

		protected override void OnStop()
		{	

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
           

			if( server != null)
			{
				server.NetworkServerClosing();
			}		
	
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
