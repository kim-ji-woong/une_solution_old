using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KpxPipeMonitoring
{
    public static class Program
    { 
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary> 
        [STAThread]
        static void Main(string[] arg)
        { 
            if (CheckFileName(arg))
            { 
                if (ProcessChecker.IsOnlyProcess("KpxPipeMonitoring"))
                { 
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new MainForm());
                }
            }
        }
        public static void SetSystemLog(string content)
        {
            string filePath = @"D:\Tomcat 7.0\webapps\ROOT\SOP\KPX\SoundBtn.log";
            string dirPath = @"D:\Tomcat 7.0\webapps\ROOT\SOP\KPX";

            DirectoryInfo di = new DirectoryInfo(dirPath);
            FileInfo fi = new FileInfo(filePath);

            try
            {
                if (!di.Exists) Directory.CreateDirectory(dirPath);
                if (!fi.Exists)
                {
                    using (StreamWriter sw = new StreamWriter(filePath))
                    {
                        sw.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]    " + content);
                        sw.Close();
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(filePath))
                    {
                        sw.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]    " + content);
                        sw.Close();
                    }
                }
            }
            catch (Exception)
            {

            }
        } 
        private static bool CheckFileName(string[] arg)
        { 
            string strPath = System.Windows.Forms.Application.ExecutablePath;
            string strStartupPath = System.Windows.Forms.Application.StartupPath + "\\";
            bool reboot = false;
            System.IO.DirectoryInfo DirInfo = new System.IO.DirectoryInfo(Application.StartupPath);
            foreach (var item in DirInfo.GetFiles())
            {
                if (item.Name.Contains("_temp") && item.Extension != ".exe")
                {
                    string strFolder = strStartupPath + item.Name;
                    string strNewFileName = item.Name.Replace("_temp", "");
                    string strNewPath = strStartupPath + strNewFileName;

                    System.IO.File.Copy(strStartupPath + item.Name, strNewPath, true);
                    System.IO.File.Delete(strStartupPath + item.Name);

                    SetSystemLog("copy : " + strStartupPath + item.Name + "new path : " + strNewPath + " / delete : " + strStartupPath + item.Name);
                    reboot = true;
                }
            } 

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
                SetSystemLog(ex.Message);
            }

            System.Threading.Thread.Sleep(5000);
             
            int nIndex = strPath.LastIndexOf('\\');
            int nIndex2 = strPath.LastIndexOf('.');

            if (nIndex < 0 || nIndex2 < 0)
                return true;

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
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    SetSystemLog(ex.Message);
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
