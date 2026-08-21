using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;

namespace IntegratedManagement4
{
    static class Program
    {
        public static string prgFont = "굴림";

        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
			if(ProcessChecker.IsOnlyProcess("통합관리시스템"))
			{
				try
				{
					log4net.Config.DOMConfigurator.Configure();
				}
				catch (System.Exception)
				{

				}

				try
				{
					Process process = Process.GetCurrentProcess();
					if (process != null)
					{
						process.EnableRaisingEvents = true;
						process.Exited += new EventHandler(OnExitProcess);
					}

					//SOPMonitoringSystem.ModuleManager.Instance.AddRelativePath(".");
					//SOPMonitoringSystem.ModuleManager.Instance.RegisterModules();

					AppDomain currentDomain = AppDomain.CurrentDomain;
					currentDomain.ProcessExit += new EventHandler(OnExitProcess);

					Application.EnableVisualStyles();
					Application.SetCompatibleTextRenderingDefault(false);
					if (args == null || args.Length == 0)
						Application.Run(new FormMain());
					else
					{
						// args : SOPSimulator, SDMS 자동 시작
						if (args[0] == "1")
							Application.Run(new FormMain(true));
						else
							Application.Run(new FormMain(false));
					}

				}
				catch (System.IO.FileNotFoundException e)
				{
					//MessageBox.Show(e.Message);
                    UnE.Utility.UMessageBoxRibbon.Show(e.Message, "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}	
        }
		
		static void OnExitProcess(object sender, EventArgs e)
		{
            IntegratedManagement4.ProcessManager.Instance.AbortAllProcess();
		}
    }
}
