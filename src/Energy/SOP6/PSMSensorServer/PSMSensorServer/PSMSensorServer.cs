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
        private NetworkWebClient client = null;
        private PSMSensorManager sensor = null;
        


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
				file = new System.IO.StreamWriter(szFullPath + "//SensorServer2.log");

			//mDBConMan = new ConManager();
			

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
                if (client != null)
                {
                    client.Close();
                    client.ShutdownSensorThread = true;
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

        void GasDetector_OnNotifyAlarm(int nComm, int nAlarmUnit, float fValue, int nChannel, int nStatus, int windDirection = -1,int windSpeed = -1)
        {
            if (client != null)
            {
                if (nStatus == 1)
                {
                    client.SendSensorData(nComm, nAlarmUnit, nChannel, (nChannel + 1), true, windDirection, windSpeed);
                }
                else
                {
                    client.SendSensorData(nComm, nAlarmUnit, nChannel, nStatus, true, windDirection, windSpeed);
                }

                System.Diagnostics.Trace.WriteLine("Alarm : " + nComm + "," + nAlarmUnit + "," + fValue + "," + nChannel + "," + nStatus);
            }
         
        }

        void FireSensorDetector_OnNotifyAlarm(int sensorType, int sensorTagID, int sensorZoneID)
        {
            if (client != null)
            {
                client.SendFireSensorData(sensorType, sensorTagID, sensorZoneID);
            }
        }

        string strSaveData = "1";
        public void LoadSaveData()
        {
            Utility ini = new Utility();
            strSaveData = ini.getinivalue("Server Connection Info", "save_data");            
        }

		//private ConManager mDBConMan = null;
		void tmrTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
            PSMSensorServer.WriteLine("DB Wait Timer");
			//if (mDBConMan.OpenConnection())
			//{
				PSMSensorServer.WriteLine("Open Connection");
				tmrTimer.Stop();
				tmrTimer.Enabled = false;

                server = new PSMNetworkServer();
                

                WebDBManager dbMgr = PSMNetworkServer.Instance.DBManager;

                client = new NetworkWebClient(dbMgr);
                sensor = new PSMSensorManager(client);
                
                LoadSaveData();

                if (strSaveData == "1")
                    sensor.SavePSMData = true;
                else
                    sensor.SavePSMData = false;

                // 새로 접속하니까 일단 모두 접속이 끊긴 것으로 초기화
                sensor.SaveAllSensorServerInfo(false);

                server.NetworkServerLoad();
                sensor.BeginServer(GasDetector_OnNotifyAlarm, FireSensorDetector_OnNotifyAlarm);                

                //mDBConMan.CloseConnection();
			//}			
		}   
	}
}
