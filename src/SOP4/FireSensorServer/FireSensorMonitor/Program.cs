using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;

namespace SensorMonitor
{
	static class Program
	{
		/// <summary>
		/// 해당 응용 프로그램의 주 진입점입니다.
		/// </summary>
		static void Main()
		{
#if SERVICE
			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[] 
			{ 
				new SOPMonitor() 
			};
			ServiceBase.Run(ServicesToRun);
#else
            try
            {
                log4net.Config.DOMConfigurator.Configure();
            }
            catch (System.Exception)
            {

            }

            
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(ExceptionHandler);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
#endif


        }
#if SERVICE
#else
        static void ExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception ex = (Exception)args.ExceptionObject;

            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
   

        }
#endif

	}
}
