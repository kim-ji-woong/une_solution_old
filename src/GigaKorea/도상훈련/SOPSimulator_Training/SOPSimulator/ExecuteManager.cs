using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOPSimulator
{
    public class ExecuteManager2
    {
        public enum APP_TYPE { FIRE_SIMULATOR = 0, TRAINING_LINK = 1, OUTBREAK_INFO = 2 };

        public ExecuteManager2()
        {

        }

        public void Run(APP_TYPE type)
        {
            if (type == APP_TYPE.FIRE_SIMULATOR)
            {
                System.Diagnostics.Process p = RunFireSimulator();
                if (p != null)
                {
                    UnE.Win32.NativeMethods.BringFront(p.MainWindowHandle);

                    // 창 띄우기 신호 전송
                    FormMain.Instance.LinkManager.SendOpenData();
                }
            }
            else if (type == APP_TYPE.TRAINING_LINK)
            {
                System.Diagnostics.Process p = RunTrainingLink();
                if (p != null)
                {
                    UnE.Win32.NativeMethods.BringFront(p.MainWindowHandle);
                }
            }
        }

        public void Run(APP_TYPE type, string strValue)
        {
            if (type == APP_TYPE.OUTBREAK_INFO)
            {
                System.Diagnostics.Process p = RunOutbreakInfo(strValue);
                if (p != null)
                {
                    UnE.Win32.NativeMethods.BringFront(p.MainWindowHandle);
                }
            }
        }

        private System.Diagnostics.Process RunOutbreakInfo(string strValue)
        {
            return RunStartProcess("OutbreakInfo.exe", strValue);
        }

        private System.Diagnostics.Process RunFireSimulator()
        {
            System.Diagnostics.Process process = RunCheckProcess("FireSimulator");

            if (process == null)
            {
                string strValue = "true";
                return RunStartProcess("FireSimulator.exe", strValue);
            }
            return process;
        }

        private System.Diagnostics.Process RunTrainingLink()
        {
            System.Diagnostics.Process process = RunCheckProcess("TrainingLink");

            if (process == null)
            {
                string strValue = "";
                return RunStartProcess("TrainingLink.exe", strValue);
            }
            return process;
        }

        //strProcessName을 가진 프로그램이 실행중인지 체크
        public System.Diagnostics.Process RunCheckProcess(string strProcessName)
        {
            System.Diagnostics.Process[] processList = System.Diagnostics.Process.GetProcesses();

            foreach (System.Diagnostics.Process process in processList)
            {
                if (process.ProcessName == strProcessName)
                    return process;
            }

            return null;
        }

        public System.Diagnostics.Process RunStartProcess(string strFileName, string args)
        {
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = strFileName;
            startInfo.WorkingDirectory = GetExecutablePath();
            startInfo.ErrorDialog = true;
            startInfo.Arguments = args;

            System.Diagnostics.Process process;
            try
            {
                process = System.Diagnostics.Process.Start(startInfo);

                return process;
            }
            catch (Exception)
            {
                //System.Windows.Forms.MessageBox.Show(ex.Message);
            }
            return null;
        }

        private string GetExecutablePath()
        {
            string strExePath = Application.ExecutablePath;
            int nIndex = strExePath.LastIndexOf('\\');
            string strTemp = strExePath.Substring(0, nIndex);

            return strTemp + "\\";
        }
    }
}
