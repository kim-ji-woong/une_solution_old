using System;
using System.Linq;
using System.Windows.Forms;

namespace UnE.CCTV
{
    internal static class Program
    {
        //private static log4net.ILog logger = null;

        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            if (ProcessChecker.IsOnlyProcess("libCCTV"))
            {
                try
                {
                    log4net.Config.XmlConfigurator.Configure();
                }
                catch (System.Exception)
                {
                }

                //ModuleManager.Instance.AddRelativePath(".");
                //ModuleManager.Instance.RegisterModules();

                if (args.Count() < 3)
                {
                    MessageBox.Show("시작할 수 없습니다.");
                }
                else
                {
                    int nMonitor = 0;

                    try
                    {
                        nMonitor = int.Parse(args[0]);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("첫번째 Paramter는 정수 형태이어야 합니다.");
                    }

                    int nSiteID = 0;

                    try
                    {
                        nSiteID = int.Parse(args[1]);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("두번째 Paramter는 정수 형태이어야 합니다.");
                    }

                    string szPipeName = args[2];

                    //logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                    AppDomain currentDomain = AppDomain.CurrentDomain;
                    currentDomain.UnhandledException += new UnhandledExceptionEventHandler(ExceptionHandler);

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    CCTVFormFrame frame = new UnE.CCTV.CCTVFormFrame(new FormMain(nSiteID, szPipeName), nMonitor);
                    frame.MinimumSize = new System.Drawing.Size(600, 480);
                    frame.Refresh();
                    Application.Run(frame);
                    
                }
            }
        }

        private static void ExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception ex = (Exception)args.ExceptionObject;

            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
            //logger.Debug("프로그램 오류", ex);
            //logger.Debug("Line: " + trace.GetFrame(0).GetFileLineNumber());
        }
    }
}