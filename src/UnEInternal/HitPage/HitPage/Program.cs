using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HitPage
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
            DBBackup.WriteLog("HitPage Start");
            ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[] 
			{ 
				new HitService() 
			};

			ServiceBase.Run(ServicesToRun);
#else
            DBBackup.WriteLog("HitPage Start");
            m_trayManager = new TrayManager();
            Application.Run();
#endif
        }
    }
}
