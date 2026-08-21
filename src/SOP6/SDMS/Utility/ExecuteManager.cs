using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SDMS
{
    public class ExecuteManager
    {
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        internal static extern bool SetWindowPos(
                int hWnd, // window handle
                int hWndInsertAfter, // placement-order handle
                int X, // horizontal position
                int Y, // vertical position
                int cx, // width
                int cy, // height
                uint uFlags); // window positioning flags

        const uint SWP_NOSIZE = 0x1;
        const uint SWP_NOMOVE = 0x2;
        const uint SWP_SHOWWINDOW = 0x40;
        const uint SWP_NOACTIVATE = 0x10;
        const int HWND_TOPMOST = -1;
        const int HWND_NOTOPMOST = -2;

        private static void BringFront(IntPtr ptr)
        {
            SetWindowPos((int)ptr, HWND_TOPMOST, 0, 0, 0, 0,
                                          SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
            // 최상위 윈도우 속성을 제거한다. 하지만 윈도우는 다른 윈도우보다 앞에 존재한다. 
            SetWindowPos((int)ptr, HWND_NOTOPMOST, 0, 0, 0, 0,
                                          SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
        }

        public enum APP_TYPE { SIEMENS_SENSOR_SIMULATOR = 0 };

        public ExecuteManager()
        {
        }

        /// <summary>
        /// 미리 정의된 형식의 P/g 실행
        /// </summary>
        public Process Run(APP_TYPE type)
        {
            if (type == APP_TYPE.SIEMENS_SENSOR_SIMULATOR)
            {
                Process p = RunSiemensSensorSimaultor(NetworkWebManager.Instance.ServerIP, "\"지멘스 센서 시뮬레이터\"");
                if (p != null)
                {
                    BringFront(p.MainWindowHandle);
                    return p;
                }
            }
            return null;
        }

        /// <summary>
        /// 사용자 정의 P/g 실행
        /// </summary>
        /// <param name="strPgName">P/g 이름</param>
        /// <param name="strExt">P/g 확장자</param>
        public Process Run(string strPgName, string strExt)
        {
            Process p = RunCheckProcess(strPgName);
            if (p == null)
            {
                string strValue = NetworkWebManager.Instance.ServerIP + " " + strPgName + " " + FormFrame.Instance.Location.X.ToString() + "," + FormFrame.Instance.Location.Y.ToString();
                return RunStartProcess(String.Format("{0}.{1}", strPgName, strExt), strValue);
            }

            BringFront(p.MainWindowHandle);

            return p;
        }

        //strProcessName을 가진 프로그램이 실행중인지 체크
        public Process RunCheckProcess(string strProcessName)
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

        private Process RunSiemensSensorSimaultor(string strServerIP, string strTitle)
        {
            /*if( UnE.SOP.ProxySOP.Instance.SiteID == 100)
            {
                Process p1 = RunCheckProcess("S1SensorTester");
                if (p1 == null)
                {
                    string strValue = strServerIP + " " + strTitle + " " + FormFrame.Instance.Location.X.ToString() + "," + FormFrame.Instance.Location.Y.ToString();
                    return RunStartProcess("S1SensorTester.exe", strValue);
                }
                return p1;
            }*/
            Process p = RunCheckProcess("SensorTester");
            if (p == null)
            {
                string strValue = strServerIP + " " + strTitle + " " + FormFrame.Instance.Location.X.ToString() + "," + FormFrame.Instance.Location.Y.ToString();
                return RunStartProcess("SensorTester.exe", strValue);
            }
            return p;
        }

    }
}