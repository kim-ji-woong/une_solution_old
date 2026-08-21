using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Windows.Forms;
using System.ServiceProcess;

namespace SOPWebServer
{
    static class Program
    {
        private static TrayManager m_trayManager = null;

        static void Main()
        {
#if SERVICE
			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[] 
			{ 
				new SOPWebService() 
			};

            

			ServiceBase.Run(ServicesToRun);
#elif TRAY_ICON
            m_trayManager = new TrayManager();
            Application.Run();
#else
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
#endif
        }
    }
}