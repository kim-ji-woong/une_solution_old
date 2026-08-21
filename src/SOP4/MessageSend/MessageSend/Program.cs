using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MessageSend
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                MessageBox.Show("실행 할 수 없습니다.");
                Application.Exit();
                return;
            }
            SOPMonitoringSystem.ModuleManager.Instance.RegisterModules();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain(args));
        }
    }
}
