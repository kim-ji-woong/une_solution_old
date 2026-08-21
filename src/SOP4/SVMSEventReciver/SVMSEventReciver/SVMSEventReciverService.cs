using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Reflection;
using System.IO;


namespace SVMSEventReciver
{
    public partial class SVMSEventReciverService : ServiceBase
    {
        private UnE.Log.LogFileCleanupTask m_CleanUpTask = null;
        private SVMSEventReciver eventReciver = null;

        private static log4net.ILog logger = null;
        
        // DBConnection확인 타이머
        private System.Timers.Timer tmrTimer = null;

        // 실행파일 폴더 위치
        private String szFullPath = ""; 

        public SVMSEventReciverService()
        {
            InitializeComponent();
        }

        private int m_nSiteID = 100;
        private void ReadSiteID()
        {
            DBUtility.Utility util = new DBUtility.Utility();
            string szSiteID = util.getinivalue("Server Connection Info", "siteid");
            if (szSiteID == null || szSiteID == "")
            {
                return;
            }

            int nSiteId = 1;
            if (int.TryParse(szSiteID, out nSiteId))
            {
                m_nSiteID = nSiteId;
            }
            else
            {
                return;
            }
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                log4net.Config.DOMConfigurator.Configure();

                m_CleanUpTask = new UnE.Log.LogFileCleanupTask();

                // CleanUp에서 예외가 발생할경우 DailyTask가 실행 안될수 있다.
                m_CleanUpTask.CleanUp();
                m_CleanUpTask.BeginDailyTask(m_CleanUpTask.CleanUp);

            }
            catch (System.Exception ex)
            {
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
            }

            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(ExceptionHandler);

            string szPath = Assembly.GetEntryAssembly().Location;
            string szFullPath = Directory.GetParent(szPath).FullName;

            // read config
            ReadSiteID();
            // DB연결정보는 config.ini에 있다.
           // mDBConMan = new DBConnectionManager();
           
            eventReciver = new SVMSEventReciver(m_nSiteID);
            eventReciver.ConnectServer();
            eventReciver.RequestCameraList();   

            // DB가 재시작일 경우가 있으므로 타이머로 시작한다.
            //tmrTimer = new System.Timers.Timer();
            //tmrTimer.Interval = 2000;
            //tmrTimer.Elapsed += tmrTimer_Elapsed;
            //tmrTimer.Start();
        }

        protected override void OnStop()
        {
            //tmrTimer.Stop();
            //tmrTimer.Enabled = false;

            eventReciver.Dispose();

        }

        static void ExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception ex = (Exception)args.ExceptionObject;

            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
            logger.Debug("프로그램 오류", ex);
            logger.Debug("Line: " + trace.GetFrame(0).GetFileLineNumber());

        }


        private DBConnectionManager mDBConMan = null;
        void tmrTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // DB가 연결 가능한 상태이면 서버를 시작한다. 
            // 아니면 대기
            //if (mDBConMan.OpenConnection())
            //{
            //    tmrTimer.Stop();
            //    tmrTimer.Enabled = false;
                
            //    eventReciver.ConnectServer();
            //    eventReciver.RequestCameraList();              

            //    mDBConMan.CloseConnection();
            //}
        }
    }
}
