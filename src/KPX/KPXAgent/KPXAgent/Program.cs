using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KPXAgent
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main(string[] arg)
        { 
            if (CheckFileName(arg))
            { 
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FormMain());
            }  
        }

        private static bool CheckFileName(string[] arg)
        {
            string strPath = System.Windows.Forms.Application.ExecutablePath;
            
            if (arg.Count() == 0)
                return true;

            int nProcessID;

            if (int.TryParse(arg[0].Trim(), out nProcessID) == false)
                return true;

            try
            {
                // 원본 Process를 삭제한다.
                System.Diagnostics.Process process = System.Diagnostics.Process.GetProcessById(nProcessID);

                if (process != null)
                {
                    
                    process.Kill(); 
                }
            }
            catch (Exception ex)
            {
                FormMain.SetLog("Program.cs / Process kill / err" + ex.Message);
            }

            int nIndex = strPath.LastIndexOf('\\');
            int nIndex2 = strPath.LastIndexOf('.');

            if (nIndex < 0 || nIndex2 < 0)
                return true;

            System.Threading.Thread.Sleep(5000);
              
            string strFileName = strPath.Substring(nIndex + 1, nIndex2 - nIndex - 1);

            if (strFileName.EndsWith("temp"))
            { 
                nIndex = strPath.LastIndexOf('\\');
                string strFolder = strPath.Substring(0, nIndex + 1);
                string strNewFileName = strFileName.Replace("_temp", "");
                string strNewPath = strFolder + strNewFileName + ".exe";

                try
                {                     
                    System.IO.File.Copy(strPath, strNewPath, true);                     
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                    startInfo.FileName = strNewFileName;
                    startInfo.WorkingDirectory = strFolder;
                    startInfo.ErrorDialog = true;
                    startInfo.Arguments = System.Diagnostics.Process.GetCurrentProcess().Id.ToString(); 
                    System.Diagnostics.Process.Start(startInfo);
                }
                catch (Exception ex)
                {
                    FormMain.SetLog("err/CheckFileName : " + ex.Message);
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    return true;
                }

                return false;
            }
            else
            { 
                string strTempFile = strPath.ToLower().Replace(".exe", "_temp.exe");

                if (System.IO.File.Exists(strTempFile))
                    System.IO.File.Delete(strTempFile);
            }

            return true;
        }
    }
}
