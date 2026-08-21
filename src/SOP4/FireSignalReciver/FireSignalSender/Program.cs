using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FireSignalSender
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {

            if (ProcessChecker.IsOnlyProcess("화재신호전송기"))
            {
                try
                {
                    log4net.Config.DOMConfigurator.Configure();
                }
                catch (System.Exception)
                {

                }
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
           
        }
    }
}
