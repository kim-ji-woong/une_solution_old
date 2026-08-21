using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InternalMessagePopup
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            ModuleManager.Instance.AddRelativePath(".");
            ModuleManager.Instance.RegisterModules();

            int x = 0, y = 0;
            int nProcessID = 0;

            if (args.Count() >= 3)
            {
                int.TryParse(args[0].Trim(), out x);
                int.TryParse(args[1].Trim(), out y);
                int.TryParse(args[2].Trim(), out nProcessID);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain(x, y, nProcessID));
        }
    }
}
