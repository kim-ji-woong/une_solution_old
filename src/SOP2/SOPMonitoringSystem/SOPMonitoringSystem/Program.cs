using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

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

            if (args.Count() < 2)
            {
                MessageBox.Show("시작할 수 없습니다.");
            }
            else
            {
				ModuleManager.Instance.AddRelativePath(".");
				ModuleManager.Instance.RegisterModules();

                int nSOPGenUserID = -1;
                string strSOPGenUserRealName = args[1];

                try
                {
                    nSOPGenUserID = int.Parse(args[0]);
                }
                catch (Exception)
                {
                    MessageBox.Show("첫번째 Paramter는 정수 형태이어야 합니다.");
                }

                if (nSOPGenUserID >= 0)
                {
					logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
					AppDomain currentDomain = AppDomain.CurrentDomain;
					currentDomain.UnhandledException += new UnhandledExceptionEventHandler(ExceptionHandler);

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new FormMain(nSOPGenUserID, strSOPGenUserRealName));
                    //Application.Run(new FormFrame(new FormMain(nSOPGenUserID, strSOPGenUserRealName)));
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
