using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ServiceProcess;

namespace AutoUpdater
{
    static class Program
    {
        private static TrayManager m_trayManager = null;

        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
#if SERVICE
			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[] 
			{ 
				new UpdaterService() 
			};

            

			ServiceBase.Run(ServicesToRun);
#else
            m_trayManager = new TrayManager();
            Application.Run();
#endif
        }
    }
}
