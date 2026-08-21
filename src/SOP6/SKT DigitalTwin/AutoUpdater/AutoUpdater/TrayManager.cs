using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Threading;
using DBUtility2;
using System.Collections;
using System.Configuration;
using System.IO;

namespace AutoUpdater
{
    using Data;
    using Network;

    public class TrayManager
    {
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

        //private System.Windows.Forms.ToolStripMenuItem tsSetSOPSystemFolder;
        private System.Windows.Forms.ToolStripMenuItem tsMenuClose;
        private WebDBManagerEx m_dbMgr = null;
        private bool m_closeSystem = false;
        private int m_nLocalSiteID = 0;

        //private const string SOPSystemFileName = "sop.path";
        private string m_strSOPSystemFolder = "";

        public TrayManager()
        {
            CreateNotifyicon();
            m_strSOPSystemFolder = Directory.GetCurrentDirectory() + "\\..";

            //ReadSOPSystemFolder();
            RunMonitoringThread();
        }

        private void RunMonitoringThread()
        {
            int nSiteID = ReadSiteID();

            if (nSiteID > 0)
            {
                m_nLocalSiteID = nSiteID;
                m_dbMgr = new WebDBManagerEx(m_nLocalSiteID);

                string strOriginalSiteID = ConfigurationManager.AppSettings.Get("siteid");
                string strDBName = ConfigurationManager.AppSettings.Get("dbname");

                if (strOriginalSiteID == null || strOriginalSiteID.Length == 0 || strDBName == null || strDBName.Length == 0)
                    return;

                int nOriginalSiteID;

                if (int.TryParse(strOriginalSiteID, out nOriginalSiteID) == false)
                    return;

                m_dbMgr.OriginalSiteID = nOriginalSiteID;
                m_dbMgr.LocalDBName = m_dbMgr.DatabaseName;
                m_dbMgr.DatabaseName = strDBName;

                DataManager.LoadData(m_dbMgr);
                AlarmManager.RunAlarmMonitoring(m_dbMgr);

                Thread t = new Thread(new ThreadStart(DoMonitoring));
                t.Start();
            }
        }

        private void DoMonitoring()
        {
            while (m_closeSystem == false)
            {
                ReadCommand();
                Thread.Sleep(1000);
            }
        }

        private void ReadCommand()
        {
            string strSQL = "Select TimeStamp, ServerCommand, ClientCommand, ServerParameter, ClientParameter from AutoUpdate where ID = " + m_dbMgr.SiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count < 5)
                return;

            VariousData<DateTime> time = WebDBManager.GetDateTimeField(arrResult[0]);
            VariousData<int> serverCmd = WebDBManager.GetIntField(arrResult[1].ToString());
            VariousData<int> clientCmd = WebDBManager.GetIntField(arrResult[2].ToString());
            string strServerParam = WebDBManager.GetStringField(arrResult[3]);
            string strClientParam = WebDBManager.GetStringField(arrResult[4]);

            if (time == null)
                return;

            string strResultMessage = "";

            if (serverCmd == null && clientCmd == null)
                return;
            else if (serverCmd == null)
            {
                CommandManager.CommandResultType result = ClientCommandManager.ProcessCommand(clientCmd.Data, strClientParam, m_strSOPSystemFolder, m_dbMgr, ref strResultMessage);
                UpdateClientHistory(result, time.Data, strResultMessage);
            }
            else if (clientCmd == null)
            {
                CommandManager.CommandResultType result = ServerCommandManager.ProcessCommand(serverCmd.Data, strServerParam, m_dbMgr, ref strResultMessage);
                UpdateServerHistory(result, time.Data, strResultMessage);
            }
            else
            {
                string strClientResultMessage = "";
                CommandManager.CommandResultType serverResult = ServerCommandManager.ProcessCommand(serverCmd.Data, strServerParam, m_dbMgr, ref strResultMessage);
                CommandManager.CommandResultType clientResult = ServerCommandManager.ProcessCommand(clientCmd.Data, strClientParam, m_dbMgr, ref strClientResultMessage);
                UpdateHistory(serverResult, clientResult, time.Data, strResultMessage, strClientResultMessage);
            }
        }

        private void UpdateServerHistory(CommandManager.CommandResultType result, DateTime dtCommand, string strResultMessage)
        {
            // 처리된 Command는 초기화 시킨다.
            string strSQL = "Update AutoUpdate set ServerCommand = NULL, ServerParameter = NULL where ID = " + m_dbMgr.SiteID.ToString();
            m_dbMgr.GetResultData(strSQL);

            string strCommandTime = DateTimeToString(dtCommand);
            string strExecuteTime = DateTimeToString(DateTime.Now);

            // 처리 결과를 남긴다.
            strSQL = "Insert into AutoUpdateHistory (CommandID, CommandTime, ExecuteTime, ServerResult, ClientResult, ServerMessage, ClientMessage) values ";
            strSQL += string.Format("({0}, '{1}', '{2}', {3}, NULL, '{4}', NULL)", m_dbMgr.SiteID, strCommandTime, strExecuteTime, (int)result, strResultMessage);

            m_dbMgr.GetResultData(strSQL);
        }

        private void UpdateClientHistory(CommandManager.CommandResultType result, DateTime dtCommand, string strResultMessage)
        {
            // 처리된 Command는 초기화 시킨다.
            string strSQL = "Update AutoUpdate set ClientCommand = NULL, ClientParameter = NULL where ID = " + m_dbMgr.SiteID.ToString();
            m_dbMgr.GetResultData(strSQL);

            string strCommandTime = DateTimeToString(dtCommand);
            string strExecuteTime = DateTimeToString(DateTime.Now);

            // 처리 결과를 남긴다.
            strSQL = "Insert into AutoUpdateHistory (CommandID, CommandTime, ExecuteTime, ServerResult, ClientResult, ServerMessage, ClientMessage) values ";
            strSQL += string.Format("({0}, '{1}', '{2}', NULL, {3}, NULL, '{4}')", m_dbMgr.SiteID, strCommandTime, strExecuteTime, (int)result, strResultMessage);

            m_dbMgr.GetResultData(strSQL);
        }

        private void UpdateHistory(CommandManager.CommandResultType serverResult, CommandManager.CommandResultType clientResult, DateTime dtCommand, string strServerResultMessage, string strClientResultMessage)
        {
            // 처리된 Command는 초기화 시킨다.
            ClearCommand();

            string strCommandTime = DateTimeToString(dtCommand);
            string strExecuteTime = DateTimeToString(DateTime.Now);

            // 처리 결과를 남긴다.
            string strSQL = "Insert into AutoUpdateHistory (CommandID, CommandTime, ExecuteTime, ServerResult, ClientResult, ServerMessage, ClientMessage) values ";
            strSQL += string.Format("({0}, '{1}', '{2}', {3}, {4}, '{5}', '{6}')",
                m_dbMgr.SiteID, strCommandTime, strExecuteTime,
                (int)serverResult, (int)clientResult, strServerResultMessage, strClientResultMessage);

            m_dbMgr.GetResultData(strSQL);
        }

        private void ClearCommand()
        {
            // 처리된 Command는 초기화 시킨다.
            string strSQL = "Update AutoUpdate set ServerCommand = NULL, ClientCommand = NULL, ServerParameter = NULL, ClientParameter = NULL where ID = " + m_dbMgr.SiteID.ToString();
            m_dbMgr.GetResultData(strSQL);
        }

        private string DateTimeToString(DateTime time)
        {
            return string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);
        }

        private int ReadSiteID()
        {
            Utility util = new Utility("..\\config.ini");
            string szSection = "Server Connection Info";
            string szText = util.getinivalue(szSection, "siteid");

            int nSiteID = -1;
            int.TryParse(szText, out nSiteID);
            return nSiteID;
        }

        private void CreateNotifyicon()
        {
            this.components = new System.ComponentModel.Container();
            this.m_contextMenu = new System.Windows.Forms.ContextMenuStrip();

            this.m_contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            //this.tsSetSOPSystemFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuClose = new System.Windows.Forms.ToolStripMenuItem();

            // Initialize contextMenu1
            this.m_contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            //this.tsSetSOPSystemFolder,
            this.tsMenuClose});
            this.m_contextMenu.Size = new System.Drawing.Size(217, 70);

            // Create the NotifyIcon.
            this.m_icon = new System.Windows.Forms.NotifyIcon(this.components);

            // The Icon property sets the icon that will appear
            // in the systray for this application.
            m_icon.Icon = global::AutoUpdater.Properties.Resources.SDMS_BLUE;

            // The ContextMenu property sets the menu that will
            // appear when the systray icon is right clicked.
            m_icon.ContextMenuStrip = this.m_contextMenu;

            // The Text property sets the text that will be displayed,
            // in a tooltip, when the mouse hovers over the systray icon.
            m_icon.Text = "유엔이 업데이트 관리자";
            m_icon.Visible = true;

            // Handle the DoubleClick event to activate the form.
            m_icon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.trayIcon_MouseClick);

            // 
            // tsSetSOPSystemFolder
            // 
            /*this.tsSetSOPSystemFolder.Name = "tsSetSOPSystemFolder";
            this.tsSetSOPSystemFolder.Size = new System.Drawing.Size(216, 22);
            this.tsSetSOPSystemFolder.Text = "SOPSystem 경로 설정하기";
            this.tsSetSOPSystemFolder.Click += new System.EventHandler(this.tsSetSOPSystemFolder_Click);*/

            // 
            // tsMenuClose
            // 
            this.tsMenuClose.Name = "tsMenuClose";
            this.tsMenuClose.Size = new System.Drawing.Size(216, 22);
            this.tsMenuClose.Text = "종료";
            this.tsMenuClose.Click += new System.EventHandler(this.tsMenuClose_Click);
        }

        private void tsMenuClose_Click(object sender, EventArgs e)
        {
            AlarmManager.Close();
            NetworkWebManager.Instance.Close();
            m_closeSystem = true;
            Application.Exit();
        }

        /*private void tsSetSOPSystemFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();

            dlg.Description = "SOP System이 설치된 폴더를 선택하세요.";

            if (m_strSOPSystemFolder.Length > 0)
                dlg.SelectedPath = m_strSOPSystemFolder;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                WriteSOPSystemFolder(dlg.SelectedPath);
            }
        }*/

        private void trayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                m_contextMenu.Show();
        }

        /*private void ReadSOPSystemFolder()
        {
            if (File.Exists(SOPSystemFileName))
            {
                StreamReader reader = new StreamReader(SOPSystemFileName, false);

                if (reader.EndOfStream == false)
                {
                    m_strSOPSystemFolder = reader.ReadLine().Trim();
                }

                reader.Close();
            }
        }

        private void WriteSOPSystemFolder(string strPath)
        {
            m_strSOPSystemFolder = strPath;

            StreamWriter writer = new StreamWriter(SOPSystemFileName, false, System.Text.Encoding.UTF8);
            writer.Write(strPath);
            writer.Close();
        }*/
    }

    public class WebDBManagerEx : WebDBManager
    {
        private int m_nOriginalSiteID = 0;
        private string m_strLocalDBName = "";

        public int OriginalSiteID
        {
            get { return m_nOriginalSiteID; }
            set { m_nOriginalSiteID = value; }
        }

        public string LocalDBName
        {
            get { return m_strLocalDBName; }
            set { m_strLocalDBName = value; }
        }

        public WebDBManagerEx(int nSiteID)
            : base(nSiteID)
        {
        }
    }
}
