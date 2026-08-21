using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DivisysCCTVInfo
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
#if TRAY_ICON
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
