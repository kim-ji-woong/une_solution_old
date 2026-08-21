using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMSSender
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            UnE.SOP.ModuleManager.Instance.AddRelativePath(".");
            UnE.SOP.ModuleManager.Instance.AddRelativePath("common");
            UnE.SOP.ModuleManager.Instance.RegisterModules();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            FormMain frmMain = new FormMain();
            frmMain.SetStartLocation(args);

            Application.Run(frmMain);
        }
    }
}
