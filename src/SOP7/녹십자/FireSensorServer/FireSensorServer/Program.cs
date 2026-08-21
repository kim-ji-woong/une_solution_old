using System;
using System.Windows.Forms;
using System.ServiceProcess;

namespace FireSensorServer
{
    static class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
#if SERVICE
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new FireSensorService()
            };

            ServiceBase.Run(ServicesToRun);
#else
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
#endif
        }
    }
}
