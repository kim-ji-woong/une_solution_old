using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace RunQuery
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 모듈이 들어있는 상대 경로 추가
            SOPMonitoringSystem.ModuleManager.Instance.AddRelativePath(".");
            // 하위 경로의 모듈을 등록
            SOPMonitoringSystem.ModuleManager.Instance.RegisterModules();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
