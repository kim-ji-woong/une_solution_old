using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HWPReportMaker
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length < 3)
                return;

            string strFolderPath = GetPath(args[0]);
            string strTargetPath = GetPath(args[1]);
            string strResultFilePath = GetPath(args[2]);

            /*ApplicationContext ctx = new ApplicationContext();
            ctx.ThreadExit += new EventHandler(ExitMain);

            FormMain frm = new FormMain();
            frm.MakeReport(strFolderPath, strTargetPath, strResultFilePath, false);
            ctx.MainForm = frm;

            frm.Dispose();
            Application.Exit();*/
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain(strFolderPath, strTargetPath, strResultFilePath));
        }

        static string GetPath(string strPath)
        {
            if (strPath.StartsWith("\""))
            {
                strPath = strPath.Substring(1);

                if (strPath.EndsWith("\""))
                    strPath = strPath.Substring(0, strPath.Length - 1);
            }
            
            return strPath;
        }

        static void ExitMain(object sender, EventArgs e)
        {
            try
            {
                foreach (System.Diagnostics.Process process in System.Diagnostics.Process.GetProcesses())
                {
                    if (process.ProcessName.ToUpper().StartsWith("HwpReportMaker"))
                    {
                        process.Kill();
                        break;
                    }
                }
            }
            catch
            (Exception)
            {
            }
        }
    }
}
