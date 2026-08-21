using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOPSimulator
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
            if (args == null || args.Count() == 0)
            {
                Application.Run(new Network.FormLogin());
            }
            else if (args.Count() >= 2)
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

                if (nSOPGenUserID >= 0)
                {
                    logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                    AppDomain currentDomain = AppDomain.CurrentDomain;
                    currentDomain.UnhandledException += new UnhandledExceptionEventHandler(ExceptionHandler);
                        
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    bool bSituationRoomMode = bCCTVMode;

                    Application.Run(new FormMain(nSOPGenUserID, strSOPGenUserRealName, isSimulationMode, onlySDMS, nMonitor, bSituationRoomMode));
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
