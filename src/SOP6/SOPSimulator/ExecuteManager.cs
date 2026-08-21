using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SOPMonitoringSystem
{
    public class ExecuteManager
    {
        

        public enum APP_TYPE { CONTROLROOM_WORKER_EDITOR = 0, SMS_SENDER = 1, BROADCAST_TESTER  = 2};

        public ExecuteManager()
        {
        }

        public void Run(APP_TYPE type)
        {
            if (type == APP_TYPE.CONTROLROOM_WORKER_EDITOR)
            {
                System.Diagnostics.Process process = RunControlTeamEditor("\"제어실 근무자 입력\"");
                if( process != null)
                {
                    UnE.Win32.NativeMethods.BringFront(process.MainWindowHandle);
                }
            }
            else if( type == APP_TYPE.SMS_SENDER)
            {
                System.Diagnostics.Process process = RunSendSMS();
                if (process != null)
                {
                    UnE.Win32.NativeMethods.BringFront(process.MainWindowHandle);
                }
            }
            else if (type == APP_TYPE.BROADCAST_TESTER)
            {
                System.Diagnostics.Process process = RunBroadRunner();
                if( process != null)
                {
                    UnE.Win32.NativeMethods.BringFront(process.MainWindowHandle);
                }
            }
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

        private System.Diagnostics.Process RunBroadcastTester()
        {
            System.Diagnostics.Process process = RunCheckProcess("TTSClientDotNet");
            if (process == null)
            {
                System.Drawing.Point location = FormSOP.Instance.Parent.Location;
                string strOpt = string.Format("{0} {1}", location.X + 200, location.Y + 200);
                return RunStartProcess("TTSClientDotNet.exe", strOpt);
            }
            return process;
        }

        private System.Diagnostics.Process RunBroadRunner()
        {
            System.Diagnostics.Process process = RunCheckProcess("BroadRunner");
            if (process == null)
            {
                string strOpt = UnE.SOP.ProxySOP.Instance.SiteID.ToString() + " " + FormSOP.Instance.DBManager.DatabaseName;
                return RunStartProcess("BroadRunner.exe", strOpt);
            }
            return process;
        }

        private System.Diagnostics.Process RunSendSMS()
        {
            System.Diagnostics.Process process = RunCheckProcess("SMSSender");
            if (process == null)
            {
                bool isDayLight = Popup.SOPLoader.IsNormal(DateTime.Now);
                Sections.SectionCommander commander = null;

                if (isDayLight)
                {
                    if (FormSOP.Instance.SOPGenUserCommanderDayLight != null)
                        commander = FormSOP.Instance.SOPGenUserCommanderDayLight;
                    else if (FormSOP.Instance.SOPGenUserCommanderNightHoliday != null)
                        commander = FormSOP.Instance.SOPGenUserCommanderNightHoliday;
                }
                else
                {
                    if (FormSOP.Instance.SOPGenUserCommanderNightHoliday != null)
                        commander = FormSOP.Instance.SOPGenUserCommanderNightHoliday;
                    else if (FormSOP.Instance.SOPGenUserCommanderDayLight != null)
                        commander = FormSOP.Instance.SOPGenUserCommanderDayLight;
                }

                System.Drawing.Point location = FormSOP.Instance.Parent.Location;
                string strOpt = string.Format("{0} {1}", location.X + 200, location.Y + 200);

                if (commander != null)
                    strOpt += " " + commander.CallerPhoneNumber;

                return RunStartProcess("SMSSender.exe", strOpt);
            }
            return process;
        }

        private System.Diagnostics.Process RunControlTeamEditor(string strTitle)
        {
            System.Diagnostics.Process process = RunCheckProcess("ControlTeamEditor2");
            if (process == null)
            {
                string strValue = strTitle + " " + FormFrame.Instance.Location.X.ToString() + "," + FormFrame.Instance.Location.Y.ToString() + " " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();
                return RunStartProcess("ControlTeamEditor2.exe", strValue);
            }
            //else
            //{
            //    IntPtr hWnd = FindWindowEx(IntPtr.Zero, IntPtr.Zero, null, process.MainWindowTitle);
            //    if (hWnd != IntPtr.Zero)
            //    {
            //        Control f = Form.FromHandle(hWnd);
            //        if (f != null)
            //        {
            //            f.BringToFront();
            //        }
            //    } 
            //}
            return process;
        }
    }
}
