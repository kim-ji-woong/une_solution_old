using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SOPBulletin
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
				//ModuleManager.Instance.AddRelativePath(".");
				//ModuleManager.Instance.RegisterModules();

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                //Application.Run(new FormMain());
                Application.Run(new FormMain2());
            }
            catch (Exception)
            {
                Application.Exit();
            }
        }
    }
}
