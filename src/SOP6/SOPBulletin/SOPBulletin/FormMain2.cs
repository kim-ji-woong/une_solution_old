using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility2;
using System.Collections;

namespace SOPBulletin
{
    public partial class FormMain2 : Form
    {
        private WebDBManager m_dbMgr = null;

        private DockingProgress2 m_dockProgress = null;
        private DockingRealTime2 m_dockRealTime = null;
        private ColorStyle m_colorStyle = new ColorStyle();

        private int m_nProgressPanelSize = 130;
        private bool m_workTimer = false;

        private static FormMain2 m_instatnce = null;
        
        public static FormMain2 Instance
        {
            get { return m_instatnce; }
        }

        public WebDBManager DBManager
        {
            get { return m_dbMgr; }
        }

        public ColorStyle ColorStyle
        {
            get { return m_colorStyle; }
        }

        public FormMain2()
        {
            m_instatnce = this;
            InitializeComponent();

            Utility bulletinConfig = new Utility();
            string szSiteID = bulletinConfig.getinivalue("Server Connection Info", "siteid", Application.StartupPath + "\\config.ini");
            int nSiteID = 1;

            if (szSiteID == null || szSiteID == "")
            {
                szSiteID = "1";
            }
            try
            {
                nSiteID = int.Parse(szSiteID);
            }
            catch (System.Exception)
            {
            }

            m_dbMgr = new WebDBManager(nSiteID);
            /*if (m_nSiteID == 2)
                m_dbMgr = new WebDBManager("SOP4");
            else
                m_dbMgr = new WebDBManager();*/

            this.DoubleBuffered = true;
        }
        
        private void FormMain2_Load(object sender, EventArgs e)
        {
            CreatePane();
            timer1.Start();
            SetMenuButtonEnables();

            this.WindowState = FormWindowState.Maximized;
        }

        public void SetMenuButtonEnables()
        {
            m_dockRealTime.SetMenuButtonEnables(rbtnSaveToHWP, rbtnCloseCurrentLog, rbtnShowPrevLog);
        }

        public void CreatePane()
        {
            m_dockRealTime = new DockingRealTime2();
            m_dockRealTime.TopLevel = false;
            m_dockRealTime.Dock = DockStyle.Fill;

            m_dockProgress = new DockingProgress2();
            m_dockProgress.TopLevel = false;
            m_dockProgress.Dock = DockStyle.Fill;

            panelStatusBottom.Controls.Add(m_dockRealTime);
            panelProgressBottom.Controls.Add(m_dockProgress);

            m_dockRealTime.Show();
            m_dockProgress.Show();

            //m_dockProgress.SetContextMenu(m_dockRealTime.ContextMenu);
            splitContainer1.Panel1MinSize = 200;
            splitContainer1.SplitterDistance = this.Size.Height - m_nProgressPanelSize;
        }

        private void OnTimer(object sender, EventArgs e)
        {
            if (m_workTimer)
                return;

            m_workTimer = true;

            HistoryManager2.LoadCurrentActionStepHistoryList(m_dockRealTime);
            HistoryManager2.FindNewActionStepHistory(m_dockRealTime);

            int nActionStepID;
            bool isRealMode;

            if (HistoryManager2.GetCurrentActionStepID(out nActionStepID, out isRealMode))
                m_dockRealTime.SetCurrentActionStep(nActionStepID, isRealMode);

            m_dockProgress.UpdateProgress(m_dockRealTime.CurrentActionStepHistory);

            m_workTimer = false;
        }

        private void FormMain2_Resize(object sender, EventArgs e)
        {
            int nDistance = this.Size.Height - m_nProgressPanelSize;

            if (nDistance > 0)
                splitContainer1.SplitterDistance = nDistance;
        }

        public void ShowContextMenu(Control ctrl, int x, int y)
        {
            m_dockRealTime.ShowContextMenu(ctrl, x, y);
        }

        private void FormMain2_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = false;
        }

        private void btnSaveToHWP_MouseDown(object sender, MouseEventArgs e)
        {
            m_dockRealTime.tsMenuItemToHWPFile_Click(null, null);
        }

        private void btnCloseCurrentLog_MouseDown(object sender, MouseEventArgs e)
        {
            m_dockRealTime.tsMenuItemCloseCurrentLog_Click(null, null);
        }

        private void btnShowPrevLog_MouseDown(object sender, MouseEventArgs e)
        {
            m_dockRealTime.tsMenuItemShowPrevLogs_Click(null, null);
        }

        private void FormMain2_VisibleChanged(object sender, EventArgs e)
        {
            if( this.Visible == true)
            {
                this.BringToFront();
                this.Activate();
            }
        }
    }
}
