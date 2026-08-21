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

namespace MessageServer
{
	public partial class MessageService : ServiceBase
	{
		private static log4net.ILog logger = null;

		public static log4net.ILog Logger
		{
			get { return MessageService.logger; }
			set { MessageService.logger = value; }
		}

		private System.Timers.Timer tmrTimer = null;
		public MessageService()
		{
			InitializeComponent();
		}

		private MessageBroker server = null;

		private static bool bEnableLog = true;

		public static bool EnableLog
		{
			get { return MessageService.bEnableLog; }
			set { MessageService.bEnableLog = value; }
		}
		public static void WriteLine(string szMsg)
		{
			if (bEnableLog == true)
                logger.Debug(szMsg);
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

            SMSDBManager.Instance.Port = "3306";
            SMSDBManager.Instance.Connect();

            server = new MessageBroker();
            server.MessageLoop();
		}

		protected override void OnStop()
		{
			


			if( server != null)
			{
				server.Close();
			}	
		
            if(SMSDBManager.Instance.IsConnect == true)
            {
                SMSDBManager.Instance.Close();
            }
        }
				
		void tmrTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{			
			

           
		}   
	}
}
