using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BroadcastServer
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
            m_trayManager = new TrayManager();
            Application.Run();
            /*Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());*/
        }
    }
}
