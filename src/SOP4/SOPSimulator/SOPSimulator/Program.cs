using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;


namespace SOPMonitoringSystem
{
   static class Program
	{


		private static log4net.ILog logger = null;

		/// <summary>
		/// 해당 응용 프로그램의 주 진입점입니다.
		/// </summary>
		[STAThread]		
		static void Main(string[] args)
		{
			try
			{
				log4net.Config.XmlConfigurator.Configure();	
			}
			catch (System.Exception)
			{				
			}

			ModuleManager.Instance.AddRelativePath(".");
            ModuleManager.Instance.AddRelativePath("sqlite3");
			ModuleManager.Instance.RegisterModules();

			if (args.Count() < 2)
			{
				MessageBox.Show("시작할 수 없습니다.");
			}
			else
			{
				int nMonitor = 0;
				int nSOPGenUserID = -1;
				string strSOPGenUserRealName = args[1];
				if (args.Length >= 3)
				{
					int.TryParse(args[2], out nMonitor);
				}

                bool isSimulationMode = false;
                bool onlySDMS = false;

                if (args.Length >= 4)
                {
                    int nRealMode;

                    if (int.TryParse(args[3], out nRealMode))
                        isSimulationMode = nRealMode != 1;
                }

                if (args.Length >= 5)
                {
                    int nWithMonitoringSystem;

                    if (int.TryParse(args[4], out nWithMonitoringSystem))
                        onlySDMS = nWithMonitoringSystem != 1;
                }

				try
				{
					nSOPGenUserID = int.Parse(args[0]);
				}
				catch (Exception)
				{
					MessageBox.Show("첫번째 Paramter는 정수 형태이어야 합니다.");
				}


                bool bCCTVMode = false;
                if (args.Length >= 6)
                {
                    int nCCTVMode;

                    if (int.TryParse(args[5], out nCCTVMode))
                        bCCTVMode = (nCCTVMode == 1);
                }

                //bool bRunSDMS = false;
				if (nSOPGenUserID >= 0)
				{
					/*if (bRunSDMS == true)
					{
						logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
						AppDomain currentDomain = AppDomain.CurrentDomain;
						currentDomain.UnhandledException += new UnhandledExceptionEventHandler(ExceptionHandler);

                        //ProxySOP.Instance.
                        Application.EnableVisualStyles();
						Application.SetCompatibleTextRenderingDefault(false);
						Application.Run(new SDMS.FormMain(nSOPGenUserID, strSOPGenUserRealName, nMonitor, isSimulationMode));            
					}
					else*/
					{
                        logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
						AppDomain currentDomain = AppDomain.CurrentDomain;
						currentDomain.UnhandledException += new UnhandledExceptionEventHandler(ExceptionHandler);
                        /*
                        #region SiteID 체크
                        DBUtility.Utility util = new DBUtility.Utility();
                        string szSiteID = util.getinivalue("Server Connection Info", "siteid");
                        if (szSiteID == null || szSiteID == "")
                        {
                            UnE.Utility.UMessageBox.Show("Site ID가 지정되지 않았습니다. ini파일을 확인하세요", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Application.Exit();
                            return;
                        }

                        int nSiteId = 1;
                        if( int.TryParse(szSiteID, out nSiteId))
                        {
                            UnE.SOP.ProxySOP.Instance.SiteID = nSiteId;
                        }
                        else
                        {
                            UnE.Utility.UMessageBox.Show("잘못된 Site ID입니다.. ini파일을 확인하세요", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Application.Exit();
                            return;
                        }
                        #endregion // SiteID 체크
                        */
                        Application.EnableVisualStyles();
						Application.SetCompatibleTextRenderingDefault(false);


                        bool bSituationRoomMode = bCCTVMode;
                        //FormFrame frame = new FormFrame();
						//Application.Run(new SOPMonitoringSystem.FormSOP(nSOPGenUserID, strSOPGenUserRealName, isSimulationMode, onlySDMS, nMonitor));
                        Application.Run(new FormFrame(new SOPMonitoringSystem.FormSOP(nSOPGenUserID, strSOPGenUserRealName, isSimulationMode, onlySDMS, nMonitor, bSituationRoomMode)));
					}
				}
			}			
		}

		static void ExceptionHandler(object sender, UnhandledExceptionEventArgs args)
		{
			Exception ex = (Exception)args.ExceptionObject;
			logger.Debug("프로그램 오류", ex);

		}     	
    }
}
