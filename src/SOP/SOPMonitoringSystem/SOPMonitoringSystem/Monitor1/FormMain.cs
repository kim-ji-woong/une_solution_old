using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XtremeDockingPane;

namespace SOPMonitoringSystem
{
    public partial class FormMain : Form
    {
        private FormLeftScenario m_frmScenario = null; // 운용 중 시나리오
        private FormLeftDisaster m_frmDisaster = null; // 재난 Tree
        private FormLeftMission m_frmMission = null; // 임무 상세
        //private FormBottomSOPLog m_frmSOPLog = null; // SOP Log
        //private FormRighSummary m_frmSummary = null; // SOP 개요
        //private FormRightDisseminate m_frmDisseminate = null; // 전파 현황
        private FormRightProgress m_frmProgress = null; // SOP 진행 현황
        //private FormRightPersonnel m_frmPersonnel = null; //SOP 요원 현황

        //private FormLeftScenario m_frmScenario = new FormLeftScenario();
        //private FormLeftDisaster m_frmDisaster = new FormLeftDisaster();
        //private FormLeftMission m_frmMission = new FormLeftMission();
        private FormBottomSOPLog m_frmSOPLog = new FormBottomSOPLog();
        private FormRighSummary m_frmSummary = null;
        private FormRightDisseminate m_frmDisseminate = new FormRightDisseminate();
        //private FormRightProgress m_frmProgress = new FormRightProgress();
        private FormRightPersonnel m_frmPersonnel = new FormRightPersonnel();

        private Form[] arrDocking = new Form[8];

        private FormProcess m_frmProcess = null;
        //private DBManager m_dbMgr = null;
        private WebDBManager m_dbMgr = null;
        private SOPDisasterSystem.FormMain m_frmMain2 = null;

        protected string m_strSkinFolder;

        private static FormMain m_frmMain = null;
        private string m_strVersion = "V1.0";
        private bool m_isReadVersion = false;

        public static FormMain Instance()
        {
            return m_frmMain;
        }


        public FormMain()
        {
            InitializeComponent();
            m_strSkinFolder = StylesPath();
            Skin_Load();
            Init();
        }

        private void Init()
        {
            m_frmMain = this;
            //m_dbMgr = new DBManager(m_frmMain);
            m_dbMgr = new WebDBManager(m_frmMain);
            m_frmMain2 = new SOPDisasterSystem.FormMain(this);

            m_frmProcess = new FormProcess(this);
            m_frmProcess.TopLevel = false;
            m_frmProcess.Parent = this;
            panelProcess.Controls.Add(m_frmProcess);
            m_frmProcess.Dock = DockStyle.Fill;
            m_frmProcess.Show();

            showMonitor2();
        }
        
        void showMonitor2()
        {
            Screen[] sc;
            sc = Screen.AllScreens;
            m_frmMain2.StartPosition = FormStartPosition.Manual;
            if(sc.Length == 2)
            {
                m_frmMain.Location = sc[0].Bounds.Location;
                m_frmMain2.Location = sc[1].Bounds.Location;
            }
            else
            {
                m_frmMain2.Location = sc[0].Bounds.Location;
            }
                        
            m_frmMain2.Show();
        }

        public string GetAppVersion()
        {
            if (!m_isReadVersion)
                ReadAppVersion();

            return m_strVersion;
        }

        private void ReadAppVersion()
        {
            try
            {
                System.IO.StreamReader reader = new System.IO.StreamReader("svnInfo.txt", Encoding.Default);

                string strLine = reader.ReadLine();

                if (strLine != null)
                {
                    int nLen = strLine.Length;
                    int nFirstIndex = -1, nSecondIndex = -1;

                    for (int i = 0; i < nLen; i++)
                    {
                        char ch = strLine.ElementAt(i);

                        if (ch < '0' || ch > '9')
                        {
                            if (nFirstIndex < 0)
                                nFirstIndex = i;
                            else
                            {
                                nSecondIndex = i;
                                break;
                            }
                        }
                    }

                    if (nFirstIndex < 0)
                    {
                        m_strVersion += "." + strLine;
                    }
                    else if (nSecondIndex < 0)
                    {
                        m_strVersion += "." + strLine.Substring(0, nFirstIndex);
                    }
                    else
                    {
                        m_strVersion += "." + strLine.Substring(nFirstIndex + 1, nSecondIndex - nFirstIndex - 1);
                    }
                }

                /*string strTarget = "Revision: ";
                string strTarget2 = "리비전: ";

                while (true)
                {
                    string strLine = reader.ReadLine();
                    if (strLine == null)
                        break;

                    if (strLine.Contains(strTarget))
                    {
                        string strVersion = strLine.Substring(strTarget.Length);
                        m_strVersion += "." + strVersion;
                        break;
                    }
                    else if (strLine.Contains(strTarget2))
                    {
                        string strVersion = strLine.Substring(strTarget2.Length);
                        m_strVersion += "." + strVersion;
                        break;
                    }
                }*/

                reader.Close();
            }
            catch (System.IO.FileNotFoundException)
            {
            }

            m_isReadVersion = true;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            CreatePane();
            this.WindowState = FormWindowState.Maximized;
            this.Text += " " + GetAppVersion();
        }

        private void Skin_Load()
        {
            axSkinFramework1.LoadSkin(m_strSkinFolder + "Vista.cjstyles", "NormalBlack2.ini");
            axSkinFramework1.ApplyWindow(this.Handle.ToInt32());
            this.BackColor = axSkinFramework1.GetColor(XtremeSkinFramework.XTPColorManagerColor.STDCOLOR_BTNFACE);
        }

        public string StylesPath()
        {
            string strExePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            System.IO.Directory.Exists(strExePath + "\\Styles\\");

            return strExePath + "\\Styles\\";
        }

        public void CreatePane()
        {
            // Bottom
            Pane paneSOPLog = axDockingPane.CreatePane(3, 300, 120, DockingDirection.DockBottomOf, null);
            paneSOPLog.Title = "SOP Log";
            paneSOPLog.Options = PaneOptions.PaneNoCloseable;

            // Left
            Pane paneMission = axDockingPane.CreatePane(2, 300, 100, DockingDirection.DockLeftOf, null);
            paneMission.Title = "임무 상세";
            paneMission.Options = PaneOptions.PaneNoCloseable;

            Pane paneDisaster = axDockingPane.CreatePane(1, 300, 50, DockingDirection.DockTopOf, paneMission);
            paneDisaster.Title = "재난 Tree";
            paneDisaster.Options = PaneOptions.PaneNoCloseable;

            Pane paneScenario = axDockingPane.CreatePane(0, 300, 40, DockingDirection.DockTopOf, paneDisaster);
            paneScenario.Title = "운용 중 시나리오";
            paneScenario.Options = PaneOptions.PaneNoCloseable;

            //Right
            Pane panePersonnel = axDockingPane.CreatePane(7, 300, 120, DockingDirection.DockRightOf, null);
            panePersonnel.Title = "SOP 요원 현황";
            panePersonnel.Options = PaneOptions.PaneNoCloseable;

            Pane paneProgress = axDockingPane.CreatePane(6, 300, 120, DockingDirection.DockTopOf, panePersonnel);
            paneProgress.Title = "SOP 진행 현황";
            paneProgress.Options = PaneOptions.PaneNoCloseable;

            Pane paneDisseminate = axDockingPane.CreatePane(5, 300, 130, DockingDirection.DockTopOf, paneProgress);
            paneDisseminate.Title = "전파 현황";
            paneDisseminate.Options = PaneOptions.PaneNoCloseable;

            Pane paneSummary = axDockingPane.CreatePane(4, 300, 60, DockingDirection.DockTopOf, paneDisseminate);
            paneSummary.Title = "SOP 개요";
            paneSummary.Options = PaneOptions.PaneNoCloseable;

            arrDocking[0] = new FormLeftScenario(this);
            m_frmScenario = (FormLeftScenario)arrDocking[0];

            arrDocking[1] = new FormLeftDisaster(this);
            m_frmDisaster = (FormLeftDisaster)arrDocking[1];

            arrDocking[2] = new FormLeftMission(this);
            m_frmMission = (FormLeftMission)arrDocking[2];

            arrDocking[3] = new FormBottomSOPLog();
            m_frmSOPLog = (FormBottomSOPLog)arrDocking[3];

            arrDocking[4] = new FormRighSummary(m_frmDisaster.GetVersionName(), m_frmDisaster.GetVersionOwner(), m_frmDisaster.GetLastAccessTime(), m_frmDisaster.GetDescription());
            m_frmSummary = (FormRighSummary)arrDocking[4];
            
            arrDocking[5] = new FormRightDisseminate();
            m_frmDisseminate = (FormRightDisseminate)arrDocking[5];

            arrDocking[6] = new FormRightProgress(this);
            m_frmProgress = (FormRightProgress)arrDocking[6];

            arrDocking[7] = new FormRightPersonnel();
            m_frmPersonnel = (FormRightPersonnel)arrDocking[7];
        }

        private void axDockingPane_AttachPaneEvent(object sender, AxXtremeDockingPane._DDockingPaneEvents_AttachPaneEvent e)
        {
            int nIndex = e.item.Id;

            if (nIndex == 0)
                e.item.Handle = arrDocking[0].Handle.ToInt32();
            else if (nIndex == 1)
                e.item.Handle = arrDocking[1].Handle.ToInt32();
            else if (nIndex == 2)
                e.item.Handle = arrDocking[2].Handle.ToInt32();
            else if (nIndex == 3)
                e.item.Handle = arrDocking[3].Handle.ToInt32();
            else if (nIndex == 4)
                e.item.Handle = arrDocking[4].Handle.ToInt32();
            else if (nIndex == 5)
                e.item.Handle = arrDocking[5].Handle.ToInt32();
            else if (nIndex == 6)
                e.item.Handle = arrDocking[6].Handle.ToInt32();
            else if (nIndex == 7)
                e.item.Handle = arrDocking[7].Handle.ToInt32();
        }

        private void axDockingPane_ResizeEvent(object sender, EventArgs e)
        {
            int left, top, right, bottom;

            axDockingPane.GetClientRect(out left, out top, out right, out bottom);
            panelProcess.SetBounds(left, top, right - left, bottom - top);
        }

        public WebDBManager GetDBManager()
        {
            return m_dbMgr;
        }

        public FormProcess GetProcess()
        {
            return m_frmProcess;
        }

        public FormLeftScenario GetScenario()
        {
            return m_frmScenario;
        }

        public FormLeftDisaster GetDisaster()
        {
            return m_frmDisaster;
        }

        public FormLeftMission GetMission()
        {
            return m_frmMission;
        }

        public FormBottomSOPLog GetSOPLog()
        {
            return m_frmSOPLog;
        }

        public FormRighSummary GetSummary()
        {
            return m_frmSummary;
        }

        public FormRightDisseminate GetDisseminate()
        {
            return m_frmDisseminate;
        }

        public FormRightProgress GetProgress()
        {
            return m_frmProgress;
        }

        public FormRightPersonnel GetPersonnel()
        {
            return m_frmPersonnel;
        }

        public SOPDisasterSystem.FormMain GetMonitor2()
        {
            return m_frmMain2;
        }

        public void OnSelectedSOP(int nDepth, string strSOPFullName, TreeNode node)
        {
            if (m_frmProcess != null)
            {
                m_frmProcess.OnSelectedSOP(nDepth, strSOPFullName, node);
            }
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            GetProcess().StopTimer();
            GetProcess().StopThread();
            Application.Exit();
        }

        public void OnSelectedScenario(string strFullName)
        {
            TreeNode node = GetDisaster().FindNode(strFullName);
            GetProcess().SetCurrentNode(node, true);

            //this.m_frmProcess
        }
    }
}
