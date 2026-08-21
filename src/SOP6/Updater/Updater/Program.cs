using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using System.IO;

namespace Updater
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main(string[] argc)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string szPath = Assembly.GetEntryAssembly().Location;
            string szFullPath = Directory.GetParent(szPath).FullName;
            string szName = Application.ExecutablePath.Replace(szFullPath+ "\\", "").ToLower();

#if !DEBUG
            if (szName == "Updater.EXE".ToLower())
            {
                try
                {
                    File.Copy(Application.ExecutablePath, szFullPath + "\\UpdateOrg.exe", true);
                    //if (!ProcessManager.Instance.RunCheckProcess("UpdateOrg"))
                    {
                        ProcessManager.Instance.RunStartProcess("UpdateOrg", "1 1 1");
                    }
                }
                catch (System.Exception)
                {                	
                }                
            }
            else
#endif
            {
                if (argc == null || argc.Length == 0)
                    return;

                Application.Run(new FormUpdate());
            }
        }
    }
}
