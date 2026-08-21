using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Configuration;
using System.Diagnostics;

namespace WatchProcess
{
    public class TrayManager
    {
        private class ProcessInfo
        {
            private string m_strProcessPath = "";
            private string m_strProcessName = "";
            private string m_strProcessParam = "";

            public string Path
            {
                get { return m_strProcessPath; }
                set { m_strProcessPath = value; }
            }

            public string Name
            {
                get { return m_strProcessName; }
                set { m_strProcessName = value; }
            }

            public string Param
            {
                get { return m_strProcessParam; }
                set { m_strProcessParam = value; }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PointInter
        {
            public int X;
            public int Y;
            public static explicit operator Point(PointInter point)
            {
                return new Point(point.X, point.Y);
            }
        }

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out PointInter lpPoint);

        private NotifyIcon m_icon = null;
        private ContextMenuStrip m_contextMenu = null;
        private System.ComponentModel.IContainer components;

        private ToolStripMenuItem tsMenuClose;
        private Timer m_timer = null;

        private string m_strAppName = "프로세스 감시자";
        //private string m_strProcessPath = "";
        //private string m_strProcessName = "";
        //private string m_strProcessParam = "";

        private bool m_prevRun = false;

        private ProcessInfo m_process = null;
        private ProcessInfo m_secondProcess = null;

        public TrayManager()
        {
            if (ReadConfig())
            {
                CreateNotifyicon();

                m_timer = new Timer();
                // 10초에 한번씩 동작
                m_timer.Interval = 1000 * 10;
                m_timer.Tick += OnTimer;
                m_timer.Start();

                // 시작과 동시에 한번 실행시킨다.
                OnTimer(null, null);
            }
        }

        private bool ReadConfig()
        {
            bool bRet = false;

            string strAppName = ConfigurationManager.AppSettings.Get("appName");
            string strProcessPath = ConfigurationManager.AppSettings.Get("processPath");
            string strProcessParam = ConfigurationManager.AppSettings.Get("processParam");
            string strProcessName = ConfigurationManager.AppSettings.Get("processName");

            string strSecondProcessPath = ConfigurationManager.AppSettings.Get("secondProcessPath");
            string strSecondProcessParam = ConfigurationManager.AppSettings.Get("secondProcessParam");
            string strSecondProcessName = ConfigurationManager.AppSettings.Get("secondProcessName");

            if (strAppName != null && strAppName.Length > 0)
                m_strAppName = strAppName;

            //if (strProcessPath == null || strProcessPath.Length == 0)
            //    return false;
            //else
            //    m_strProcessPath = strProcessPath;

            //if (strProcessName == null || strProcessName.Length == 0)
            //    return false;
            //else
            //    m_strProcessName = strProcessName;

            //if (strProcessParam != null && strProcessParam.Length == 0)
            //    m_strProcessParam = strProcessParam;

            if ((strProcessPath != null && strProcessPath.Length != 0) ||
                (strProcessName != null && strProcessName.Length != 0))
            {
                m_process = new ProcessInfo();
                m_process.Path = strProcessPath;
                m_process.Name = strProcessName;

                if (strProcessParam != null && strProcessParam.Length == 0)
                    m_process.Param = strProcessParam;
            }

            if ((strSecondProcessPath != null && strSecondProcessPath.Length != 0) ||
                (strSecondProcessName != null && strSecondProcessName.Length != 0))
            {
                m_secondProcess = new ProcessInfo();
                m_secondProcess.Path = strSecondProcessPath;
                m_secondProcess.Name = strSecondProcessName;

                if (strSecondProcessParam != null && strSecondProcessParam.Length == 0)
                    m_secondProcess.Param = strSecondProcessParam;
            }

            if (m_process != null || m_secondProcess != null)
                bRet = true;

            return bRet;
        }

        private void OnTimer(object sender, EventArgs e)
        {
            if (m_prevRun == false)
            {
                //if (CheckRun() == false)
                //    RunProcess();
                //else
                //    m_prevRun = true;

                bool bRunChk = true;

                if (m_process != null && CheckRun(m_process) == false)
                {
                    bRunChk = false;
                    RunProcess(m_process);
                }

                if (m_secondProcess!= null && CheckRun(m_secondProcess) == false)
                {
                    bRunChk = false;
                    RunProcess(m_secondProcess);
                }

                if (bRunChk == true)
                    m_prevRun = true;
            }
            else
            {
                //if (CheckRun() == false)
                //    m_prevRun = false;
                bool bRunChk = true;

                if (m_process != null && CheckRun(m_process) == false)
                    bRunChk = false;

                if (m_secondProcess != null && CheckRun(m_secondProcess) == false)
                    bRunChk = false;

                if (bRunChk == false)
                    m_prevRun = false;
            }
        }

        //private void RunProcess()
        //{
        //    string strProcessFolder = "";
        //    int index1 = m_strProcessPath.LastIndexOf('\\');
        //    int index2 = m_strProcessPath.LastIndexOf('/');

        //    if (index1 < 0 && index2 < 0)
        //        return;
        //    else if (index1 < 0)
        //    {
        //        strProcessFolder = m_strProcessPath.Substring(0, index2);
        //    }
        //    else if (index2 < 0)
        //    {
        //        strProcessFolder = m_strProcessPath.Substring(0, index1);
        //    }
        //    else
        //    {
        //        int index = index1 < index2 ? index2 : index1;
        //        strProcessFolder = m_strProcessPath.Substring(0, index1);
        //    }

        //    ProcessStartInfo info = new ProcessStartInfo();
        //    info.WorkingDirectory = strProcessFolder;
        //    info.FileName = m_strProcessPath;

        //    if (m_strProcessParam.Length > 0)
        //        info.Arguments = m_strProcessParam;

        //    Process.Start(info);
        //}
        private void RunProcess(ProcessInfo process)
        {
            string strProcessFolder = "";
            int index1 = process.Path.LastIndexOf('\\');
            int index2 = process.Path.LastIndexOf('/');

            if (index1 < 0 && index2 < 0)
                return;
            else if (index1 < 0)
            {
                strProcessFolder = process.Path.Substring(0, index2);
            }
            else if (index2 < 0)
            {
                strProcessFolder = process.Path.Substring(0, index1);
            }
            else
            {
                int index = index1 < index2 ? index2 : index1;
                strProcessFolder = process.Path.Substring(0, index1);
            }

            ProcessStartInfo info = new ProcessStartInfo();
            info.WorkingDirectory = strProcessFolder;
            info.FileName = process.Path;

            if (process.Param.Length > 0)
                info.Arguments = process.Param;

            Process.Start(info);
        }

        //private bool CheckRun()
        //{
        //    Process[] processes = Process.GetProcessesByName(m_strProcessName);
        //    return processes.Length > 0;
        //}
        private bool CheckRun(ProcessInfo process)
        {
            Process[] processes = Process.GetProcessesByName(process.Name);
            return processes.Length > 0;
        }

        private void CreateNotifyicon()
        {
            this.components = new System.ComponentModel.Container();
            this.m_contextMenu = new System.Windows.Forms.ContextMenuStrip();

            this.m_contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsMenuClose = new System.Windows.Forms.ToolStripMenuItem();

            // Initialize contextMenu1
            this.m_contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuClose});
            this.m_contextMenu.Size = new System.Drawing.Size(181, 70);

            // Create the NotifyIcon.
            this.m_icon = new System.Windows.Forms.NotifyIcon(this.components);

            // The Icon property sets the icon that will appear
            // in the systray for this application.
            m_icon.Icon = global::WatchProcess.Properties.Resources.SDMS_BLUE;

            // The ContextMenu property sets the menu that will
            // appear when the systray icon is right clicked.
            m_icon.ContextMenuStrip = this.m_contextMenu;

            // The Text property sets the text that will be displayed,
            // in a tooltip, when the mouse hovers over the systray icon.
            m_icon.Text = m_strAppName;
            m_icon.Visible = true;

            // Handle the DoubleClick event to activate the form.
            m_icon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.trayIcon_MouseClick);

            // 
            // tsMenuClose
            // 
            this.tsMenuClose.Name = "tsMenuClose";
            this.tsMenuClose.Size = new System.Drawing.Size(180, 22);
            this.tsMenuClose.Text = "종료";
            this.tsMenuClose.Click += new System.EventHandler(this.tsMenuClose_Click);
        }

        private void tsMenuClose_Click(object sender, EventArgs e)
        {
            m_timer.Stop();
            Application.Exit();
        }

        private void trayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                m_contextMenu.Show();
        }
    }
}
