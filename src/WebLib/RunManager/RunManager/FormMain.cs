using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RunManager
{
    public partial class FormMain : Form
    {
        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;
        //private DBManager m_dbMgr = new DBManager();
        private DBManagerMySQL m_dbMgr = new DBManagerMySQL();
        private System.Diagnostics.Process outProcess = new System.Diagnostics.Process();
        private bool m_isProcessing = false;

        private bool m_isSingleMode = false;
        private MultiProcessor m_processor = null;

        // 1분(60000 밀리 세컨드)
        private int m_nSaveExcelInterval = 600000;
        private int m_nSaveFileTimeCount = 600000;

        public FormMain()
        {
            InitializeComponent();

            // Create a simple tray menu with only one item.
            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Exit", OnExit);

            // Create a tray icon. In this example we use a
            // standard system icon for simplicity, but you
            // can of course use your own custom icon too.
            trayIcon      = new NotifyIcon();
            trayIcon.Text = "봉사인구 산정 매니저";
            trayIcon.Icon = new Icon(SystemIcons.Application, 40, 40);

            // Add menu to tray icon and show it.
            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible     = true;

            m_processor = new MultiProcessor(outProcess);
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible       = false; // Hide form window.
            ShowInTaskbar = false; // Remove from taskbar.
            base.OnLoad(e);

            timer1.Start();
        }

        private bool RunCheckProcess(string strProcessName)
        {
            System.Diagnostics.Process[] processList = System.Diagnostics.Process.GetProcesses();

            foreach (System.Diagnostics.Process process in processList)
            {
                if (process.ProcessName == strProcessName)
                    return true;
            }

            return false;
        }

        private void OnExit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void OnTimer(object sender, EventArgs e)
        {
            if (!m_dbMgr.IsOpened)
            {
                m_dbMgr.OpenConnection();
                if (!m_dbMgr.IsOpened)
                    return;
            }

            m_nSaveFileTimeCount += timer1.Interval;

            if (m_nSaveExcelInterval <= m_nSaveFileTimeCount)
            {
                m_nSaveFileTimeCount = 0;
                ExcelExporter excel = new ExcelExporter(m_dbMgr);
                excel.SaveFile("UserInfo.csv");
            }

            if (m_isSingleMode)
            {
                if (m_isProcessing)
                    return;

                try
                {
                    string strSQL = "select CONTROLLER from LIB_CONTROLLER";
                    //System.Data.SqlClient.SqlDataReader reader;
                    MySql.Data.MySqlClient.MySqlDataReader reader;
                    m_dbMgr.ReadDB(strSQL, null, out reader);

                    if (reader == null)
                        return;

                    if (reader.Read())
                    {
                        int nRunMode = m_dbMgr.GetField<int>(reader[0], 0);

                        if (nRunMode == 1)
                        {
                            reader.Close();

                            outProcess.StartInfo.FileName = m_dbMgr.OutExePath;
                            outProcess.Start();

                            m_isProcessing = true;

                            string strPath = m_dbMgr.OutExePath;
                            int nDotIndex = strPath.LastIndexOf('.');

                            if (nDotIndex >= 0)
                            {
                                strPath = strPath.Substring(0, nDotIndex);
                            }

                            int i = 0;

                            for (;i<10;i++)
                            {
                                if (!RunCheckProcess(strPath))
                                    System.Threading.Thread.Sleep(1000);
                                else
                                    break;
                            }

                            // 실행 실패
                            if (i == 10)
                            {
                                m_isProcessing = false;
                                return;
                            }

                            // 실행중
                            strSQL = "update LIB_CONTROLLER set CONTROLLER = 2";
                            m_dbMgr.Execute(strSQL);
                            m_isProcessing = false;

                            return;
                        }
                        else if (nRunMode == 2)
                        {
                            string strPath = m_dbMgr.OutExePath;
                            int nDotIndex = strPath.LastIndexOf('.');

                            if (nDotIndex >= 0)
                            {
                                strPath = strPath.Substring(0, nDotIndex);
                            }

                            if (!RunCheckProcess(strPath))
                            {
                                reader.Close();

                                // 실행 종료
                                strSQL = "update LIB_CONTROLLER set CONTROLLER = 0";
                                m_dbMgr.Execute(strSQL);
                                return;
                            }
                        }
                    }
                    else
                    {
                        reader.Close();
                        return;
                    }

                    reader.Close();
                }
                catch (Exception)
                {
                    m_dbMgr.CloseConnection();
                }
            }
            else
            {
                if (m_processor.IsProcessing)
                    return;

                m_processor.Run(m_dbMgr);
            }
        }
    }
}
