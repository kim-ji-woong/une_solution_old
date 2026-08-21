using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DXFViewer;
using UnE.Utility;
using UnE.Utility.Print;
using System.Threading;
using System.IO;

using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace RoadMan
{
	public partial class FormMain : Form, IMenuCommandOwner
    {
        public enum DockingType { NONE = 0, LAYER = 1, PROCESS_SCHEDULE = 2, PROCESS_RESULT = 4 };
        public enum StatusType { STATUS = 0, COORD };
        public enum ToolbarMode { NORMAL = 0, SCREEN_CAPTURE };

        private Panel m_panelLayer = null;
        private Panel m_panelProcessSchedule = null;
        private Panel m_panelProcessResult = null;
        private int m_nDockingMode = 0;
        private XMLManager m_xmlMgr = new XMLManager();
        private string m_strPrjPath = "";

        private ToolbarMode m_toolbarMode = ToolbarMode.NORMAL;
        private PanelDXFViewer m_panelSelected = null;
        private FormOption m_frmOption = null;

        // 리포트 생성을 위한 최대 대기시간(초)
        private int m_nMonitorReportLimitSeconds = 30;
        private int m_nCurrentMonitorTime = -1;
        private string m_strReportResultFilePath = "";
        private string m_strReportFilePath = "";

        private MenuButton m_ctrlSearch = null;
        private MenuButton m_ctrlMemo = null;
        private MenuButton m_ctrlReport = null;
        private MenuButton m_ctrlLayer = null;
        private MenuButton m_ctrlProcessLayer = null;
        private MenuButton m_ctrlProcessSchedule = null;
        private MenuButton m_ctrlProcessResult = null;
        private MenuButton m_ctrlUndo = null;
        private MenuButton m_ctrlRedo = null;
        private MenuButton m_ctrlScreenShot = null;
        private MenuButton m_ctrlOption = null;
        private MenuButton m_ctrlPrint = null;

        private string m_strInitProjectFile = null;
        private int m_nReportProcessID = -1;

        private static FormMain m_instance = null;
        public static FormMain Instance
        {
            get { return m_instance; }
        }

        public int DockingMode
        {
            get { return m_nDockingMode; }
            set
            {
                if (m_nDockingMode != value)
                    SetDockingMode(value);
            }
        }

        public DXFViewer.DXFControl CurrentDXFControl
        {
            get
            {
                if (tabControlEx1.SelectedTab == null)
                    return null;

                PanelDXFViewer panel = (PanelDXFViewer)tabControlEx1.SelectedTab.Tag;
                return panel.DXFControl;
            }
        }

        public PanelDXFViewer CurrentPanel
        {
            get
            {
				if (tabControlEx1.TabCount == 0)
					return null;
                if (tabControlEx1.SelectedTab == null)
                    return null;

                return (PanelDXFViewer)tabControlEx1.SelectedTab.Tag;
            }
        }

        public Panel PanelLayer
        {
            get { return m_panelLayer; }
        }

        public Panel PanelProcessSchedule
        {
            get { return m_panelProcessSchedule; }
        }

        public Panel PanelProcessResult
        {
            get { return m_panelProcessResult; }
        }

        public PanelDXFViewer SelectedPanel
        {
            get { return m_panelSelected; }
            set { m_panelSelected = value; }
        }

        public FormOption OptionForm
        {
            get { return m_frmOption; }
            set { m_frmOption = value; }
        }

        public FormMain(string strProjectFile)
        {
            m_instance = this;
            InitializeComponent();

            // Menu와 버튼을 링크
            MenuLink();
			//dxfControl1.PrintDocument = new DXFViewer.UPrintDocument();

			Color textColor = Color.FromArgb(75, 71, 86);
			Color backColor = Color.White;

			UnE.Utility.CustomMenuHelper helper = new UnE.Utility.CustomMenuHelper(this);
			helper.MakeCustomLookMenu(MainMenuStrip, backColor, textColor, new VariousData<Color>(textColor), new VariousData<Color>(backColor));
            helper.MakeCustomLookMenu(contextMenuStrip1, textColor, backColor);
			this.MouseWheel += FormMain_MouseWheel;

			UnE.Utility.UMessageBox.FrameColor = Color.FromArgb(53, 50, 61);
            m_strInitProjectFile = strProjectFile;
        }

        private void MenuLink()
        {
            m_ctrlSearch = new MenuButton(rbtnSearch, menuSearch, this.toolBarButton_Click);
            m_ctrlMemo = new MenuButton(rbtnMemo, menuMemo, this.toolBarButton_Click);
            m_ctrlReport = new MenuButton(rbtnReport, menuReport, this.toolBarButton_Click);
            m_ctrlLayer = new MenuButton(rbtnLayer, menuLayer, this.toolBarButton_Click);
            m_ctrlProcessLayer = new MenuButton(rbtnProcessLayer, menuProcessLayer, this.toolBarButton_Click);
            m_ctrlProcessSchedule = new MenuButton(rbtnProcessSchedule, menuProcessSchedule, this.toolBarButton_Click);
            m_ctrlProcessResult = new MenuButton(rbtnProcessResult, menuProcessResult, this.toolBarButton_Click);
            m_ctrlUndo = new MenuButton(rbtnUndo, menuUndo, this.toolBarButton_Click);
            m_ctrlRedo = new MenuButton(rbtnRedo, menuRedo, this.toolBarButton_Click);
            m_ctrlScreenShot = new MenuButton(rbtnScreenShot, menuScreenCapture, this.toolBarButton_Click);
            m_ctrlOption = new MenuButton(rbtnOptions, menuOption, this.toolBarButton_Click);
            m_ctrlPrint = new MenuButton(rbtnPrint, menuPrint, this.toolBarButton_Click);
        }

		void FormMain_MouseWheel(object sender, MouseEventArgs e)
		{
			PanelDXFViewer panel = CurrentPanel;
			if( panel != null)
			{
				if( panel.DXFControl != null)
				{
					Point pt = PointToScreen(new Point(e.X, e.Y));
					Point newPt = panel.DXFControl.PointToClient(pt);
					MouseEventArgs newEvent = new MouseEventArgs(e.Button, e.Clicks, newPt.X, newPt.Y, e.Delta);
					panel.DXFControl.OnMouseWheel(sender, newEvent);
				}
			
			}
			
		}

		public void RunCommand(int nCommandID)
		{

		}
		
		public void CheckedChanged(int nCommandID, bool bChecked)
		{
		}

		public ToolStripStatusLabel GetStatusLabel()
		{
			return tsLabelStatusWork;
		}

        private void FormMain_Load(object sender, EventArgs e)
        {
            InitTabControl();
            InitDockingForms();
            //dxfControl1.ExternalPainter = new DXFExternPainter(dxfControl1);
			//CreateFormPrintPageSetup();

            SetToolbarMode(ToolbarMode.NORMAL);

            statusClockTimer.Start();

			splitContainerMain.Panel1MinSize = 40;
			splitContainerMain.SplitterDistance = 400;

			updateCmdTimer.Enabled = true;
			updateCmdTimer.Start();

            if (m_strInitProjectFile != null)
                OpenFile(m_strInitProjectFile);
        }

        private void InitTabControl()
        {
            tabControlEx1.UseCloseButton = false;
        }

        public void SetToolbarMode(ToolbarMode mode)
        {
            if (mode == ToolbarMode.NORMAL)
            {
                m_ctrlUndo.Visible = m_ctrlRedo.Visible = m_ctrlLayer.Visible = m_ctrlProcessLayer.Visible = true;
                m_ctrlProcessSchedule.Visible = m_ctrlProcessResult.Visible = m_ctrlScreenShot.Visible = true;
                m_ctrlOption.Visible = m_ctrlPrint.Visible = m_ctrlMemo.Visible = m_ctrlReport.Visible = true;
                pictureBox1.Visible = pictureBox2.Visible = pictureBox3.Visible = pictureBox4.Visible = true;

                rbtnSelectFullScreen.Visible = rbtnSaveScreenCaptureImage.Visible = rbtnCloseScreenCapture.Visible = false;

				m_ctrlSearch.Visible = true;
            }
            else if (mode == ToolbarMode.SCREEN_CAPTURE)
            {
                rbtnSelectFullScreen.Location = m_ctrlSearch.Location;
                rbtnSaveScreenCaptureImage.Location = new Point(rbtnSelectFullScreen.Location.X + rbtnSelectFullScreen.Size.Width, rbtnSelectFullScreen.Location.Y);
                rbtnCloseScreenCapture.Location = new Point(rbtnSaveScreenCaptureImage.Location.X + rbtnSaveScreenCaptureImage.Size.Width, rbtnSaveScreenCaptureImage.Location.Y);

                m_ctrlUndo.Visible = m_ctrlRedo.Visible = m_ctrlLayer.Visible = m_ctrlProcessLayer.Visible = false;
                m_ctrlProcessSchedule.Visible = m_ctrlProcessResult.Visible = m_ctrlScreenShot.Visible = false;
                m_ctrlOption.Visible = m_ctrlPrint.Visible = m_ctrlMemo.Visible = m_ctrlReport.Visible = false;
                pictureBox1.Visible = pictureBox2.Visible = pictureBox3.Visible = pictureBox4.Visible = false;

                rbtnSelectFullScreen.Visible = rbtnSaveScreenCaptureImage.Visible = rbtnCloseScreenCapture.Visible = true;

				m_ctrlSearch.Visible = false;
            }

            m_toolbarMode = mode;
        }

        private void InitDockingForms()
        {
            m_panelProcessSchedule = new Panel();
            m_panelProcessSchedule.Dock = DockStyle.Fill;

            splitContainerLeftDown.Panel1.Controls.Add(m_panelProcessSchedule);
            m_panelProcessSchedule.Show();

            m_panelProcessResult = new Panel();
            m_panelProcessResult.Dock = DockStyle.Fill;

            splitContainerLeftDown.Panel2.Controls.Add(m_panelProcessResult);
            m_panelProcessResult.Show();

            m_panelLayer = new Panel();
            m_panelLayer.Dock = DockStyle.Fill;

            splitContainerLeft.Panel1.Controls.Add(m_panelLayer);
            m_panelLayer.Show();
             
            SetDockingMode(m_nDockingMode);
        }

        private void SetDockingMode(int nMode)
        {
            bool showLayer = (nMode & (int)DockingType.LAYER) == (int)DockingType.LAYER;
            bool showSchedule = (nMode & (int)DockingType.PROCESS_SCHEDULE) == (int)DockingType.PROCESS_SCHEDULE;
            bool showResult = (nMode & (int)DockingType.PROCESS_RESULT) == (int)DockingType.PROCESS_RESULT;


			bool bSetSplitPos = false;
			if (splitContainerMain.Panel1Collapsed == true)
			{
				bSetSplitPos = true;
			}

            if (showLayer)
            {
				
                splitContainerMain.Panel1Collapsed = false;
                splitContainerLeft.Panel1Collapsed = false;

                if (showSchedule)
                {
                    splitContainerLeft.Panel2Collapsed = false;
                    splitContainerLeftDown.Panel1Collapsed = false;

                    if (showResult)
                        splitContainerLeftDown.Panel2Collapsed = false;
                    else
                        splitContainerLeftDown.Panel2Collapsed = true;
                }
                else
                {
                    splitContainerLeftDown.Panel1Collapsed = true;

                    if (showResult)
                    {
                        splitContainerLeft.Panel2Collapsed = false;
                        splitContainerLeftDown.Panel2Collapsed = false;
                    }
                    else
                    {
                        splitContainerLeft.Panel2Collapsed = true;
                        splitContainerLeftDown.Panel2Collapsed = true;
                    }
                }

				if (bSetSplitPos == true)
				{
					splitContainerMain.SplitterDistance = m_nSplitDistance;
				}
            }
            else
            {
                splitContainerLeft.Panel1Collapsed = true;

                if (showSchedule)
                {
                    splitContainerMain.Panel1Collapsed = false;
                    splitContainerLeft.Panel2Collapsed = false;
                    splitContainerLeftDown.Panel1Collapsed = false;

                    if (showResult)
                        splitContainerLeftDown.Panel2Collapsed = false;
                    else
                        splitContainerLeftDown.Panel2Collapsed = true;
                }
                else
                {
                    splitContainerLeftDown.Panel1Collapsed = true;

                    if (showResult)
                    {
                        splitContainerMain.Panel1Collapsed = false;
                        splitContainerLeft.Panel2Collapsed = false;
                        splitContainerLeftDown.Panel2Collapsed = false;
                    }
                    else
                    {
                        splitContainerMain.Panel1Collapsed = true;
                        splitContainerLeft.Panel2Collapsed = true;
                        splitContainerLeftDown.Panel2Collapsed = true;
                    }
                }
            }

            m_nDockingMode = nMode;

            PanelDXFViewer panel = CurrentPanel;

            if (panel != null)
                panel.DockingMode = m_nDockingMode;
        }

        private void HideLayerPriority()
        {
            PanelDXFViewer panel = CurrentPanel;

            if (panel == null)
                return;

            panel.LayerForm.HideLayerPriority();
        }

        private void menu_Click(object sender, EventArgs e)
        {
			

            HideLayerPriority();

            if (sender == menuNewProject)
                menuNewProject_Click(sender, e);
            else if (sender == menuOpenProject)
                menuOpenProject_Click(sender, e);
            else if (sender == menuSaveProject)
                menuSaveProject_Click(sender, e);
            else if (sender == menuSaveAsProject)
                menuSaveAsProject_Click(sender, e);
        }

        private void menuNewProject_Click(object sender, EventArgs e)
        {
            FormNewProject frm = new FormNewProject();
			DialogFormFrame frameNew = new DialogFormFrame(frm);
			if (frameNew.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (System.IO.File.Exists(frm.DXFPath))
                {
                    RemoveAllTabPages();
                    TabPage page = AddTabPage();

                    PanelDXFViewer panel = CurrentPanel;
                    PanelDXFViewer.SetTabPageText(page, frm.DXFPath, PanelDXFViewer.LoadingResult.GOING_ON);

                    this.Cursor = Cursors.WaitCursor;
                    List<LayerData> arrLayers = panel.DataManager.OpenDXF(frm.DXFPath, panel);
                    //List<LayerData> arrLayers = m_dataMgr.OpenDXF(frm.DXFPath, dxfControl1);

                    if (arrLayers != null)
                    {
                        panel.DataManager.Clear();

                        m_strPrjPath = "";

                        FormFrame.Instance.Text = FormFrame.Instance.AppName + " - 새 프로젝트";
                        panel.LayerForm.SetLayers(arrLayers);
                        panel.DataManager.SetSelectableLayers();

                        panel.SelectPanel();
                        EnableToolbars(true);

                        m_ctrlLayer.Checked = true;
                        m_ctrlProcessSchedule.Checked = false;
                        m_ctrlProcessResult.Checked = false;

                        ShowDockingForms();
                        //page.Text = GetProjectName(frm.DXFPath);
                        PanelDXFViewer.SetTabPageText(page, frm.DXFPath, PanelDXFViewer.LoadingResult.SUCCESS);

						UndoRedoManager.Instance.Reset();
                    }
                    else
                    {
                        //page.Text = "로딩실패 - " + GetProjectName(frm.DXFPath);
                        PanelDXFViewer.SetTabPageText(page, frm.DXFPath, PanelDXFViewer.LoadingResult.FAIL);
                    }

                    this.Cursor = Cursors.Arrow;
                }

				UndoRedoManager.Instance.BeginRedoUndo();
            }

        }

        private void EnableToolbars(bool enabled)
        {
            m_ctrlLayer.Enabled = m_ctrlProcessSchedule.Enabled = m_ctrlProcessResult.Enabled = enabled;
            m_ctrlProcessLayer.Enabled = m_ctrlPrint.Enabled = m_ctrlOption.Enabled = enabled;
            m_ctrlScreenShot.Enabled = m_ctrlMemo.Enabled = enabled;
			m_ctrlSearch.Enabled = enabled;

            if (enabled)
            {
                if (m_nReportProcessID < 0)
                //if (m_nCurrentMonitorTime < 0)
                    m_ctrlReport.Enabled = enabled;
            }
        }

        private void toolBarButton_Click(object sender, EventArgs e)
        {
            HideLayerPriority();

			if (m_ctrlLayer.Equals(sender))
			{
				m_ctrlLayer.Checked = !m_ctrlLayer.Checked;

				ShowLayerForm();
			}
			else if (m_ctrlProcessSchedule.Equals(sender))
			{
				m_ctrlProcessSchedule.Checked = !m_ctrlProcessSchedule.Checked;

                ShowProcessScheduleForm();
			}
			else if (m_ctrlProcessResult.Equals(sender))
			{
				m_ctrlProcessResult.Checked = !m_ctrlProcessResult.Checked;

                ShowProcessResultForm();
			}
			else if (m_ctrlProcessLayer.Equals(sender))
			{
                PanelDXFViewer panel = CurrentPanel;

                if (panel != null)
                {
                    FormProcessLayer frm = new FormProcessLayer();
					DialogFormFrame frameProcess = new DialogFormFrame(frm);
                    frm.AllLayers = panel.LayerForm.GetLayerList();
                    frm.CompleteLayers = panel.DataManager.CompleteLayers;
                    frm.IncompleteLayers = panel.DataManager.IncompleteLayers;
                    frm.PartialLayers = panel.DataManager.PartialLayers;

					if (frameProcess.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                    {
                        panel.DataManager.SetLayerList(panel.DataManager.CompleteLayers, frm.CompleteLayers);
                        panel.DataManager.SetLayerList(panel.DataManager.IncompleteLayers, frm.IncompleteLayers);
                        panel.DataManager.SetLayerList(panel.DataManager.PartialLayers, frm.PartialLayers);
                    }
                }
			}
			else if (m_ctrlPrint.Equals(sender))
			{
                PanelDXFViewer panel = CurrentPanel;

                if (panel == null)
                    return;

                DialogFormFrame frameSetup = panel.PrintFrame;				
				frameSetup.TopMost = true;
				frameSetup.Show();
				
			}
            else if (m_ctrlOption.Equals(sender))
            {
                if (m_frmOption == null)
                {
                    m_frmOption = new FormOption();					
					DialogFormFrame frameOption = new DialogFormFrame(m_frmOption);
					frameOption.Show(this);
				
                }
            }
			else if(m_ctrlMemo.Equals(sender))
			{
				bool bCheck = m_ctrlMemo.Checked;				
				m_ctrlMemo.Checked = !bCheck;
				PanelDXFViewer panel = CurrentPanel;
				if (panel != null)
				{
					panel.MemoMode = !bCheck;
				}
			}
            else if (m_ctrlScreenShot.Equals(sender))
            {
                SetToolbarMode(ToolbarMode.SCREEN_CAPTURE);
				PanelDXFViewer panel = CurrentPanel;
				if (panel!= null)
				{
					/*panel.ScreenSelectMode = true;
					panel.ScreenCaptuer.CaptureRectWindow();

					rbtnSaveScreenCaptureImage.Enabled = false;*/
                    // 화면캡쳐를 하면 Default로 FullScreen Capture 상태로 만든다.
                    toolBarButton_Click(rbtnSelectFullScreen, null);
				}				
            }
			else if (sender == this.rbtnSelectFullScreen)
			{
				PanelDXFViewer panel = CurrentPanel;
				if (panel != null)
				{
					panel.ScreenSelectMode = true;					
					panel.ScreenCaptuer.CaptureFullScreen();
					panel.ScreenCaptuer.CaptureRectWindow();
					rbtnSaveScreenCaptureImage.Enabled = true;
				}
			}
			else if(sender == this.rbtnSaveScreenCaptureImage)
			{
				PanelDXFViewer panel = CurrentPanel;
				if (panel != null)
				{
					panel.ScreenCaptuer.SaveImage();
				}
			}
            else if (sender == rbtnCloseScreenCapture)
            {
                SetToolbarMode(ToolbarMode.NORMAL);
				PanelDXFViewer panel = CurrentPanel;
				if (panel != null)
				{					
					panel.ScreenCaptuer.CancelRectWindow();
				}
            }
            else if (m_ctrlReport.Equals(sender))
            {
                ProcessReport();
            }
			else if (m_ctrlUndo.Equals(sender))
			{

				menuUndo_Click(null, null);				
					
			}
			else if(m_ctrlRedo.Equals(sender))
			{
				menuRedo_Click(null, null);
			}
			//else if (sender == m_ctrlHome)
			//	dxfControl1.LoadHomeMatrix(true);
        }

        private void ProcessReport()
        {
            if (CheckInstallHWP())
            {
                int nIndex = 0, nXMLCount = 0;
                string strFolderPath = GetReportFolderPath();
                string strErrorMessage = "";

                foreach (TabPage page in tabControlEx1.TabPages)
                {
                    PanelDXFViewer panel = (PanelDXFViewer)page.Tag;

                    if (panel == null)
                        continue;

                    if (!panel.SaveReportRawData(nIndex++, strFolderPath, ref strErrorMessage))
                    {
                        UnE.Utility.UMessageBox.Show(this, "리포트 파일을 저장할 수 없습니다.\r\n" + strErrorMessage, "오류");
                        return;
                    }
                    else
                        nXMLCount++;
                }

                if (nXMLCount == 0)
                    UnE.Utility.UMessageBox.Show(this, "리포트 파일에 저장할 내용이 없습니다.", "오류");
                else
                {
                    /*SaveFileDialog dlg = new SaveFileDialog();
                    
                    dlg.Filter = "아래한글 files (*.hwp)|*.hwp|All files (*.*)|*.*";
			        dlg.RestoreDirectory = true;

                    if (dlg.ShowDialog() == DialogResult.Cancel)
                    {
                        DeleteFolder(strFolderPath, false);
                        return;
                    }

                    m_strReportFilePath = dlg.FileName;*/
                    m_strReportFilePath = "nouse";
                    m_ctrlReport.Enabled = false;

                    string strResultFilePath = strFolderPath + "\\result.txt";

                    System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
                    info.Arguments = "\"" + strFolderPath + "\" \"" + m_strReportFilePath + "\" \"" + strResultFilePath + "\"";
                    info.CreateNoWindow = true;
                    info.FileName = Application.StartupPath + "\\HWPReportMaker.exe";

                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    process.StartInfo = info;

                    if (process.Start())
                        m_nReportProcessID = process.Id;

                    // Report 제작이 끝났는지 Timer로 확인한다.(최대 10초)
                    MonitorReportBuild(strResultFilePath);
                }
            }
        }

        private void MonitorReportBuild(string strResultFilePath)
        {
            m_nCurrentMonitorTime = 0;
            m_strReportResultFilePath = strResultFilePath;
            timer1.Start();
        }

        private string GetReportFolderPath()
        {
            string strFolderPath = Application.StartupPath + "\\Report";

            if (System.IO.Directory.Exists(strFolderPath))
            {
                string[] arrFiles = System.IO.Directory.GetFiles(strFolderPath);
                string[] arrFolders = System.IO.Directory.GetDirectories(strFolderPath);

                foreach (string strFile in arrFiles)
                {
                    System.IO.File.Delete(strFile);
                }

                foreach (string strFolder in arrFolders)
                {
                    DeleteFolder(strFolder);
                }
            }
            else
                System.IO.Directory.CreateDirectory(strFolderPath);

            return strFolderPath;
        }

        private void DeleteFolder(string strFolderPath, bool selfDelete = true)
        {
            string[] arrFiles = System.IO.Directory.GetFiles(strFolderPath);
            string[] arrFolders = System.IO.Directory.GetDirectories(strFolderPath);

            foreach (string strFile in arrFiles)
            {
                System.IO.File.Delete(strFile);
            }

            foreach (string strFolder in arrFolders)
            {
                DeleteFolder(strFolder);
            }

            if (selfDelete)
                System.IO.Directory.Delete(strFolderPath);
        }

        private bool CheckInstallHWP()
        {
            const string HwpRoot = @"Applications\Hwp.exe";

            Microsoft.Win32.RegistryKey R = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(HwpRoot);

            if (R == null)
            {
                UnE.Utility.UMessageBox.Show(this, "아래 한글이 설치되지 않았습니다.", "저장 실패", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

		public void EnsureSaveImage()
		{
			rbtnSaveScreenCaptureImage.Enabled = true;
		}

		public void ShowLayerForm()
        {
            if (m_ctrlLayer.Checked)
                SetDockingMode(m_nDockingMode | (int)DockingType.LAYER);
            else
                SetDockingMode(m_nDockingMode & (~(int)DockingType.LAYER));
        }

		public void HideLayerForm()
		{
			m_ctrlLayer.Checked = false;
			SetDockingMode(m_nDockingMode & (~(int)DockingType.LAYER));
			m_ctrlLayer.Refresh();
		}

		public void ShowProcessScheduleForm()
        {
            if (m_ctrlProcessSchedule.Checked)
                SetDockingMode(m_nDockingMode | (int)DockingType.PROCESS_SCHEDULE);
            else
                SetDockingMode(m_nDockingMode & (~(int)DockingType.PROCESS_SCHEDULE));
        }

		public void HideProcessScheduleForm()
		{
			m_ctrlProcessSchedule.Checked = false;
			SetDockingMode(m_nDockingMode & (~(int)DockingType.PROCESS_SCHEDULE));
			m_ctrlProcessSchedule.Refresh();
		}

        public void ShowProcessResultForm()
        {
            if (m_ctrlProcessResult.Checked)
                SetDockingMode(m_nDockingMode | (int)DockingType.PROCESS_RESULT);
            else
                SetDockingMode(m_nDockingMode & (~(int)DockingType.PROCESS_RESULT));
        }

		public void HideProcessResultForm()
		{
			m_ctrlProcessResult.Checked = false;
			SetDockingMode(m_nDockingMode & (~(int)DockingType.PROCESS_RESULT));
			m_ctrlProcessResult.Refresh();
		}

        public void RefreshView()
        {
            DXFControl ctrl = CurrentDXFControl;

            if (ctrl != null)
            {
                //ctrl.Refresh();
                CurrentPanel.DXFRefresh();
            }

            //dxfControl1.Refresh();
        }

        private void menuSaveAsProject_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();

            dlg.Filter = "Project Files|*.prj|All FIles|*.*";
            dlg.FilterIndex = 0;
            dlg.Title = "다른 이름으로 Project 파일 저장";

            XMLManager.FileOption option = XMLManager.FileOption.NO_PASSWORD;
            string strPassword = "";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                m_strPrjPath = dlg.FileName;

                if (System.IO.File.Exists(m_strPrjPath))
                {
                    if (!OverWrite(m_strPrjPath, ref option, ref strPassword))
                        return;
                }
                else
                {
                    if (!GetSaveOption(ref option, ref strPassword))
                        return;
                }
            }
            else
                return;

            this.Cursor = Cursors.WaitCursor;

            if (m_xmlMgr.SaveProject(tabControlEx1.TabPages, m_strPrjPath, option, strPassword))
            {
                string strPrjName = GetProjectName(m_strPrjPath);
                FormFrame.Instance.Text = FormFrame.Instance.AppName + " - " + strPrjName;

                UnE.Utility.UMessageBox.Show(this, "프로젝트가 생성되었습니다.", "프로젝트", MessageBoxButtons.OK, MessageBoxIcon.Information);
               
            }
            else
			{
				string szErrorMsg = m_xmlMgr.ErrorMessage;
				if( szErrorMsg == "")
					szErrorMsg = "프로젝트 저장이 실패 하였습니다. (알수 없는 오류)";
                UnE.Utility.UMessageBox.Show(this, szErrorMsg, "프로젝트 생성 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
            

            this.Cursor = Cursors.Arrow;
        }

        private void menuSaveProject_Click(object sender, EventArgs e)
        {
            string strPrjPath = "";
            string strPassword = "";
            XMLManager.FileOption option = XMLManager.FileOption.NO_PASSWORD;

            if (m_strPrjPath == "")
            {
                SaveFileDialog dlg = new SaveFileDialog();

                dlg.Filter = "Project Files|*.prj|All FIles|*.*";
                dlg.FilterIndex = 0;
                dlg.Title = "Project 파일 저장";

                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    strPrjPath = dlg.FileName;

                    if (!GetSaveOption(ref option, ref strPassword))
                        return;
                }
                else
                    return;
            }
            else
            {
                if (System.IO.File.Exists(m_strPrjPath))
                {
                    if (!OverWrite(m_strPrjPath, ref option, ref strPassword))
                        return;
                }
                else
                {
                    if (!GetSaveOption(ref option, ref strPassword))
                        return;
                }

                strPrjPath = m_strPrjPath;
            }

            this.Cursor = Cursors.WaitCursor;

			if (m_xmlMgr.SaveProject(tabControlEx1.TabPages, strPrjPath, option, strPassword))
			{
				m_strPrjPath = strPrjPath;

				string strPrjName = GetProjectName(m_strPrjPath);
				FormFrame.Instance.Text = FormFrame.Instance.AppName + " - " + strPrjName;

                UnE.Utility.UMessageBox.Show(this, "프로젝트가 성공적으로 저장 되었습니다.", "프로젝트", MessageBoxButtons.OK, MessageBoxIcon.Information);				
			}
			else
			{
				string szErrorMsg = m_xmlMgr.ErrorMessage;
				if (szErrorMsg == "")
					szErrorMsg = "프로젝트 저장이 실패 하였습니다. (알수 없는 오류)";
                UnE.Utility.UMessageBox.Show(this, szErrorMsg, "프로젝트 저장 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);				
			}

            this.Cursor = Cursors.Arrow;
        }

        private bool GetSaveOption(ref XMLManager.FileOption option, ref string strPassword)
        {
            FormSaveOption frm = new FormSaveOption();
			DialogFormFrame frameOption = new DialogFormFrame(frm);
			if (frameOption.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                option = frm.FileOption;
                strPassword = frm.Password;
            }
            else
                return false;

            return true;
        }

        private bool CheckPassword(byte[] arrKey, bool open4Save, ref string strPassword)
        {
            FormPassword frm = new FormPassword();
            frm.Open4Save = open4Save;
			DialogFormFrame framePass = new DialogFormFrame(frm);
			if (framePass.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                byte[] userInputKey = XMLManager.MakeKey(frm.Password);

                for (int i = 0; i < 4; i++)
                {
                    if (userInputKey[i] != arrKey[i])
                    {
                        UnE.Utility.UMessageBox.Show(this, "암호가 일치하지 않습니다.", "비밀번호 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //MessageBox.Show("암호가 일치하지 않습니다.");
                        return false;
                    }
                }

                strPassword = frm.Password;
            }
            else
                return false;

            return true;
        }

        private bool OverWrite(string strPath, ref XMLManager.FileOption option, ref string strPassword)
        {
            byte[] arrKey;

            if (!XMLManager.ReadProjectOption(strPath, out option, out arrKey))
            {
				string szMsg = "기존에 저장되어 있던 파일을 읽을수 없습니다.\r\n" + m_strPrjPath;
                UnE.Utility.UMessageBox.Show(this, szMsg, "프로젝트 열기 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);                
                return false;
            }

            if (option == XMLManager.FileOption.PASSWORD_READ_WRITE ||
                option == XMLManager.FileOption.PASSWORD_SAVE_ONLY)
            {
                if (!CheckPassword(arrKey, true, ref strPassword))
                    return false;
            }

            return true;
        }

        private void menuOpenProject_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "Project Files|*.prj|All FIles|*.*";
            dlg.FilterIndex = 0;
            dlg.Title = "Project 파일 열기";

            //XMLManager.FileOption option = XMLManager.FileOption.NO_PASSWORD;
            //string strPassword = "";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                OpenFile(dlg.FileName);
            }	
        }

        private bool OpenFile(string strPath)
        {
            XMLManager.FileOption option = XMLManager.FileOption.NO_PASSWORD;
            string strPassword = "";

            byte[] arrKey;

            if (!XMLManager.ReadProjectOption(strPath, out option, out arrKey))
            {
                UnE.Utility.UMessageBox.Show(this, "잘못된 형식의 파일입니다.", "프로젝트 열기 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (option == XMLManager.FileOption.PASSWORD_READ_WRITE)
            {
                if (!CheckPassword(arrKey, false, ref strPassword))
                    return false;
            }


            this.Enabled = false;

            UndoRedoManager.Instance.Reset();

            FormMain.Instance.RemoveAllTabPages();
            //m_dataMgr.Clear();

            m_strPrjPath = strPath;

            Dictionary<TabPage, DXFDatas> dicDXFDatas = m_xmlMgr.LoadProject(strPath, option, strPassword);
            //Dictionary<TabPage, List<LayerData>> dicDXFLayers = m_xmlMgr.LoadProject(dlg.FileName, option, strPassword);

            Cursor.Current = Cursors.WaitCursor;

            if (dicDXFDatas == null)
            {
                string szErrorMsg = m_xmlMgr.ErrorMessage;
                if (szErrorMsg == "")
                    szErrorMsg = "프로젝트 열기가 실패 하였습니다. (알수 없는 오류)";
                UnE.Utility.UMessageBox.Show(this, szErrorMsg, "프로젝트 열기 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //m_dataMgr.Restore();
                return false;
            }
            else
            {
                string strPrjName = GetProjectName(m_strPrjPath);
                FormFrame.Instance.Text = FormFrame.Instance.AppName + " - " + strPrjName;
                OpenDXFs(dicDXFDatas);
            }
            this.Enabled = true;
            Cursor.Current = Cursors.Arrow;
            UndoRedoManager.Instance.BeginRedoUndo();

            return true;
        }

        private void OpenDXFs(Dictionary<TabPage, DXFDatas> dicDXFDatas)
        {
            int nTabPageCount = tabControlEx1.TabPages.Count;

            for (int i = 0; i < nTabPageCount;i++)
            {
                TabPage page = tabControlEx1.TabPages[i];
                
                PanelDXFViewer panel = (PanelDXFViewer)page.Tag;
                panel.DXFControl.OpenNRefresh = false;
            }

            foreach (KeyValuePair<TabPage, DXFDatas> pair in dicDXFDatas)
            {
                PanelDXFViewer panel = (PanelDXFViewer)pair.Key.Tag;
                PanelDXFViewer.SetTabPageText(pair.Key, panel.DXFFilePath, PanelDXFViewer.LoadingResult.GOING_ON);
            }

            tabControlEx1.Refresh();

            foreach (KeyValuePair<TabPage, DXFDatas> pair in dicDXFDatas)
            {
                //OpenDXF(pair);
                Thread t = new Thread(new ParameterizedThreadStart(OpenDXF));
                t.Start(pair);
            }
        }

        private void OpenDXF(object arg)
        {
            KeyValuePair<TabPage, DXFDatas> pair = (KeyValuePair<TabPage, DXFDatas>)arg;
            PanelDXFViewer panel = (PanelDXFViewer)pair.Key.Tag;

            bool isOpened = panel.DXFControl.OpenDXF(panel.DXFFilePath);//panel.OpenDXF(pair.Value);

            if (isOpened)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    panel.PostOpenDXF(pair.Value);

                    if (panel == CurrentPanel)
                    {
                        panel.SelectPanel();
                        EnableToolbars(true);

                        m_ctrlLayer.Checked = true;
                        m_ctrlProcessSchedule.Checked = false;
                        m_ctrlProcessResult.Checked = false;

                        ShowDockingForms();
                    }
                });
            }

            this.Invoke((MethodInvoker)delegate
            {
                panel.DXFControl.Refresh();
                panel.DXFControl.SaveHomeMatrix();
                tabControlEx1.Refresh();
            });
        }

        private void ShowDockingForms()
        {
            ShowLayerForm();
            ShowProcessScheduleForm();
            ShowProcessResultForm();

            m_ctrlLayer.Refresh();
            m_ctrlProcessSchedule.Refresh();
            m_ctrlProcessResult.Refresh();
        }

        public string GetProjectName(string strFullPath)
        {
            int nIndex1 = strFullPath.LastIndexOf('\\');
            int nIndex2 = strFullPath.LastIndexOf('.');

            string strPrjName = "";

            if (nIndex1 >= 0)
            {
                if (nIndex1 < nIndex2)
                    strPrjName = strFullPath.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
                else
                    strPrjName = strFullPath.Substring(nIndex1 + 1);
            }
            else
            {
                if (nIndex2 >= 0)
                    strPrjName = strFullPath.Substring(0, nIndex2);
                else
                    strPrjName = strFullPath;
            }

            return strPrjName;
        }

        public void SetStatusText(StatusType type, string strText)
        {
            if (type == StatusType.STATUS)
                tsLabelStatusWork.Text = strText;
            else if (type == StatusType.COORD)
                tsLabelCoord.Text = strText;
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (m_nReportProcessID > 0)
            {
                timer1.Stop();

                try
                {
                    System.Diagnostics.Process process = System.Diagnostics.Process.GetProcessById(m_nReportProcessID);

                    if (process != null)
                        process.Kill();
                }
                catch (ArgumentException)
                {
                }
            }

            statusClockTimer.Stop();
            statusClockTimer.Enabled = false;
        }

        private void statusClockTimer_Tick(object sender, EventArgs e)
        {
            DateTime dtNow = DateTime.Now;
            tsLabelClock.Text = dtNow.ToLongDateString() + " " + dtNow.ToLongTimeString();
        }

        public void ShowEditBoxHatchProperty(EditBoxHatch hatch)
        {
            if (hatch.LinkedScheduleProperty == null)
                return;

            if (hatch.LinkedScheduleProperty.Schedule == null)
                return;

            if (CurrentPanel == null)
                return;

            ProcessSchedule schedule = CurrentPanel.ProcessScheduleForm.GetCurrentVisibleSchedule();

            if (schedule == hatch.LinkedScheduleProperty.Schedule)
                CurrentPanel.ProcessScheduleForm.SelectScheduleProperty(hatch.LinkedScheduleProperty);
            else
            {
                if (!m_ctrlProcessSchedule.Checked)
                {
                    m_ctrlProcessSchedule.Checked = true;
                    SetDockingMode(m_nDockingMode | (int)DockingType.PROCESS_SCHEDULE);
                }

                if (schedule != null)
                    CurrentPanel.ProcessScheduleForm.CloseScheduleProperty();

                CurrentPanel.ProcessScheduleForm.SelectSchedule(hatch.LinkedScheduleProperty.Schedule.ScheduleName);
                CurrentPanel.ProcessScheduleForm.SelectScheduleProperty(hatch.LinkedScheduleProperty);
            }
        }

        public TabPage AddTabPage()
        {
            TabPage page = new TabPage();

            PanelDXFViewer panel = new PanelDXFViewer();
            panel.Dock = DockStyle.Fill;
            page.Controls.Add(panel);
            page.Tag = panel;
            tabControlEx1.TabPages.Add(page);

            return page;
        }

        public TabPage InsertTabPage(int nIndex)
        {
            if (nIndex < 0 || nIndex > tabControlEx1.TabPages.Count)
                return null;

            TabPage page = new TabPage();

            PanelDXFViewer panel = new PanelDXFViewer();
            panel.Dock = DockStyle.Fill;
            page.Controls.Add(panel);
            page.Tag = panel;
            tabControlEx1.TabPages.Insert(nIndex, page);

            return page;
        }

        public void RemoveTabPage(TabPage page)
        {
			PanelDXFViewer panel = (PanelDXFViewer)page.Tag;
			if( panel != null)
			{
				if( panel.MemoMode == true)
				{
					panel.MemoMode = false;
				}
			}

            tabControlEx1.TabPages.Remove(page);

            if (tabControlEx1.TabPages.Count == 0)
                OnPostClear();
        }

        public int GetTabPageCount()
        {
            return tabControlEx1.TabPages.Count;
        }

        public TabPage GetTabPage(int nIndex)
        {
            if (nIndex < 0 || nIndex >= GetTabPageCount())
                return null;

            return tabControlEx1.TabPages[nIndex];
        }

        public TabPage RemoveTabPage(int nIndex)
        {
            TabPage page = GetTabPage(nIndex);


			if (page != null)
			{
				PanelDXFViewer panel = (PanelDXFViewer)page.Tag;
				if (panel != null)
				{
					if (panel.MemoMode == true)
					{
						panel.MemoMode = false;
					}
				}
				tabControlEx1.TabPages.RemoveAt(nIndex);
			}

            if (tabControlEx1.TabPages.Count == 0)
                OnPostClear();

            return page;
        }

		public void CloseMemo()
		{
			foreach (TabPage page in tabControlEx1.TabPages)
			{
				PanelDXFViewer pane = (PanelDXFViewer)page.Tag;
				if( pane != null)
				{
					pane.MemoMode = false;
				}
			}
		}

        public void RemoveAllTabPages()
        {
			CloseMemo();

            tabControlEx1.TabPages.Clear();

            OnPostClear();
        }

        private void OnPostClear()
        {
            m_ctrlLayer.Checked = false;
            m_ctrlProcessSchedule.Checked = false;
            m_ctrlProcessResult.Checked = false;

            ShowLayerForm();
            ShowProcessScheduleForm();
            ShowProcessResultForm();

            EnableToolbars(false);
            m_panelSelected = null;

			m_ctrlMemo.Checked = false;
			
        }

        private void tabControlEx1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlEx1.SelectedTab == null ||
                (tabControlEx1.SelectedTab.Tag != null && ((PanelDXFViewer)tabControlEx1.SelectedTab.Tag).LoadingStatus != PanelDXFViewer.LoadingResult.SUCCESS))
            {
                OnPostClear();
            }
            else
            {
                PanelDXFViewer panel = (PanelDXFViewer)tabControlEx1.SelectedTab.Tag;
                panel.SelectPanel();

				foreach(TabPage page in tabControlEx1.TabPages)
				{
					PanelDXFViewer pane = (PanelDXFViewer)page.Tag;
					if( pane != panel)
					{
						pane.MemoMode = false;
					}
				}
				if( m_ctrlMemo.Checked == true)
				{
					panel.MemoMode = true;
				}
				else
				{
					panel.MemoMode = false;
				}
				

                EnableToolbars(true);

                m_nDockingMode = panel.DockingMode;
                m_ctrlLayer.Checked = (m_nDockingMode & (int)DockingType.LAYER) == (int)DockingType.LAYER;
                m_ctrlProcessSchedule.Checked = (m_nDockingMode & (int)DockingType.PROCESS_SCHEDULE) == (int)DockingType.PROCESS_SCHEDULE;
                m_ctrlProcessResult.Checked = (m_nDockingMode & (int)DockingType.PROCESS_RESULT) == (int)DockingType.PROCESS_RESULT;
                /*m_ctrlLayer.Checked = true;
                m_ctrlProcessSchedule.Checked = false;
                m_ctrlProcessResult.Checked = false;*/

                ShowDockingForms();
            }
        }

        private TabPage InsertNLoadTabPage(int nIndex)
        {
            FormNewProject frm = new FormNewProject();
			frm.Text = "도면파일 선택";
			DialogFormFrame frameNew = new DialogFormFrame(frm);


			if (frameNew.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (TabPage _page in tabControlEx1.TabPages)
                {
                    PanelDXFViewer _panel = (PanelDXFViewer)_page.Tag;

                    if (_panel.DXFFilePath == frm.DXFPath)
                    {
                        if (UnE.Utility.UMessageBox.Show(this, "이미 같은 도면 파일을 사용하고 있습니다.\r\n같은 파일에 대한 새로운 탭을 생성하시겠습니까?", "주의", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                            return null;
                        else
                            break;
                    }
                }

                TabPage page = InsertTabPage(nIndex);

                if (page == null)
                    return null;

                PanelDXFViewer.SetTabPageText(page, frm.DXFPath, PanelDXFViewer.LoadingResult.GOING_ON);

                PanelDXFViewer panel = (PanelDXFViewer)page.Tag;
                panel.DXFFilePath = frm.DXFPath;

                Thread t = new Thread(new ParameterizedThreadStart(OpenNewDXF));
                t.Start(page);

                return page;
            }

            return null;
        }

        private void OpenNewDXF(object arg)
        {
            TabPage page = (TabPage)arg;
            PanelDXFViewer panel = (PanelDXFViewer)page.Tag;

            panel.DXFControl.OpenNRefresh = false;

            bool isOpened = panel.DXFControl.OpenDXF(panel.DXFFilePath);

            if (isOpened)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    panel.PostOpenDXF(null);

                    if (panel == CurrentPanel)
                    {
                        panel.SelectPanel();
                        EnableToolbars(true);

                        m_ctrlLayer.Checked = true;
                        m_ctrlProcessSchedule.Checked = false;
                        m_ctrlProcessResult.Checked = false;

                        ShowDockingForms();
                    }
                });
            }

            this.Invoke((MethodInvoker)delegate
            {
				UndoRedoManager.Instance.Reset();

                panel.DXFControl.Refresh();
                panel.DXFControl.SaveHomeMatrix();
                tabControlEx1.Refresh();
            });
        }

        private void tabControlEx1_OnTabMouseUp(object sender, UnE.Controls.TabControlMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                int nPageIndex = tabControlEx1.TabPages.IndexOf(e.Page);

                if (nPageIndex < 0)
                {
                    menuDeleteTab.Enabled = menuAddTabToLeft.Enabled = menuAddTabToRight.Enabled = menuMoveToLeft.Enabled = menuMoveToRight.Enabled = false;
                }
                else
                {
                    menuAddTabToLeft.Enabled = menuAddTabToRight.Enabled = true;
                    menuDeleteTab.Enabled = tabControlEx1.TabPages.Count > 1;
                    menuMoveToLeft.Enabled = nPageIndex > 0;
                    menuMoveToRight.Enabled = nPageIndex < tabControlEx1.TabPages.Count - 1;
                }

                contextMenuStrip1.Tag = e.Page;
                contextMenuStrip1.Show(tabControlEx1, e.Point.X, e.Point.Y);
            }
        }

        private void menuAddTabToLeft_Click(object sender, EventArgs e)
        {
            TabPage page = (TabPage)contextMenuStrip1.Tag;
            int nIndex = tabControlEx1.TabPages.IndexOf(page);

            if (nIndex >= 0)
                InsertNLoadTabPage(nIndex);
        }

        private void menuAddTabToRight_Click(object sender, EventArgs e)
        {
            TabPage page = (TabPage)contextMenuStrip1.Tag;
            int nIndex = tabControlEx1.TabPages.IndexOf(page);

            if (nIndex >= 0)
                InsertNLoadTabPage(nIndex + 1);
        }

        private void menuDeleteTab_Click(object sender, EventArgs e)
        {
            TabPage page = (TabPage)contextMenuStrip1.Tag;

            string strMessage = string.Format("[{0}] 탭을 정말 삭제하시겠습니까?", page.Text);
            if (UnE.Utility.UMessageBox.Show(this, strMessage, "주의", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
			{
				UndoRedoManager.Instance.Reset();

                tabControlEx1.TabPages.Remove(page);
            }
        }

        private void menuMoveToLeft_Click(object sender, EventArgs e)
        {
            TabPage page = (TabPage)contextMenuStrip1.Tag;
            int nIndex = tabControlEx1.TabPages.IndexOf(page);

            if (nIndex > 0)
            {
                tabControlEx1.TabPages.Remove(page);
                tabControlEx1.TabPages.Insert(nIndex - 1, page);
                tabControlEx1.Refresh();
            }
        }

        private void menuMoveToRight_Click(object sender, EventArgs e)
        {
            TabPage page = (TabPage)contextMenuStrip1.Tag;
            int nIndex = tabControlEx1.TabPages.IndexOf(page);

            if (nIndex >= 0)
            {
                tabControlEx1.TabPages.Remove(page);
                tabControlEx1.TabPages.Insert(nIndex + 1, page);
                tabControlEx1.Refresh();
            }
        }

		private void FormMain_SizeChanged(object sender, EventArgs e)
		{
			int i = 0;
			i++;
		}

		public void FormMain_LocationChanged(object sender, EventArgs e)
		{
			PanelDXFViewer panel = CurrentPanel;
			if (panel != null)
			{
				panel.PanelDXFViewer_LocationChanged(panel, e);
			}
		}

		private void tabControlEx1_SizeChanged(object sender, EventArgs e)
		{
			PanelDXFViewer panel = CurrentPanel;
			if (panel != null)
			{
				panel.PanelDXFViewer_LocationChanged(panel, e);
				panel.PanelDXFViewer_SizeChanged(panel, e);
			}
		}

		private void rbtnHidePanel_Click(object sender, EventArgs e)
		{
			splitContainerMain.Panel1MinSize = 40;
			splitContainerMain.SplitterDistance = 40;
			rbtnHidePanel.Visible = false;
			rbtnShowPanel.Location = rbtnHidePanel.Location;
			rbtnShowPanel.Visible = true;
		}

		private void rbtnShowPanel_Click(object sender, EventArgs e)
		{
			splitContainerMain.SplitterDistance = m_nSplitDistance;
			rbtnShowPanel.Visible = false;
			rbtnHidePanel.Location = rbtnShowPanel.Location;
			rbtnHidePanel.Visible = true;
			
		}

		private int m_nSplitDistance = 400;
		private void splitContainerMain_SplitterMoving(object sender, SplitterCancelEventArgs e)
		{
			m_nSplitDistance = e.SplitX;
		}

		private void splitContainerMain_SplitterMoved(object sender, SplitterEventArgs e)
		{			
			
		}


		private FormSearch mSearchForm = null;
		private DialogFormFrame mFrameSearch = null;


		private void BeginSearch()
		{
			

			if (mSearchForm != null && mSearchForm.Visible == true)
			{
				if (mFrameSearch != null && mFrameSearch.Visible == true)
				{

					EndLoadSearch();

					mFrameSearch.Visible = false;
					return;
				}
			}

			PanelDXFViewer pane = FormMain.Instance.CurrentPanel;
			if (pane != null)
			{
				pane.ProcessResultForm.ClosePropertyForm();
				pane.ProcessScheduleForm.CloseScheduleProperty();
			}

			menuSearch.Checked = true;
			m_ctrlSearch.Checked = true;
			m_ctrlSearch.Refresh();

			if (mSearchForm == null)
			{
				mSearchForm = new FormSearch();
				mSearchForm.PrintShowHeader = Options.Instance.PrintHeader;
				mSearchForm.PrintShowDate = Options.Instance.PrintDate;
				mSearchForm.PrintHeaderText = Options.Instance.PrintHeaderText;
			}
			if (mFrameSearch == null)
			{
				mFrameSearch = new DialogFormFrame(mSearchForm, false);
				mFrameSearch.TopMost = true;
				mFrameSearch.ShowInTaskbar = true;

				//mFrameSearch.Parent = this;
				mFrameSearch.StartPosition = FormStartPosition.CenterScreen;
				mFrameSearch.Show();
			}
			mFrameSearch.Show();
			mSearchForm.ShowOptionPane();
		}
		private void ribbonButton1_Click(object sender, EventArgs e)
		{
			BeginSearch();			
		}

		public void EndLoadSearch()
		{
			m_ctrlSearch.Checked = false;
			m_ctrlSearch.Refresh();
			menuSearch.Checked = false;

            if (mFrameSearch != null)
			    mFrameSearch.Visible = false;

			if (mSearchForm!= null)
			{
				Options.Instance.PrintHeader = mSearchForm.PrintShowHeader;
				Options.Instance.PrintDate = mSearchForm.PrintShowDate;
				Options.Instance.PrintHeaderText = mSearchForm.PrintHeaderText;
			}			
			
		}

        private bool IsAliveReport()
        {
            if (m_nReportProcessID < 0)
            {
                return false;
            }

            if (File.Exists(m_strReportResultFilePath))
            {
                File.Delete(m_strReportResultFilePath);
                m_nReportProcessID = -1;
                return false;
            }

            return true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!IsAliveReport())
            {
                timer1.Stop();
                m_ctrlReport.Enabled = true;
            }

            //if (File.Exists(m_strReportResultFilePath)/* && File.Exists(m_strReportFilePath)*/)
            //{
            //    StreamReader reader = new StreamReader(m_strReportResultFilePath, Encoding.UTF8);
            //    VariousData<bool> result = null;

            //    while (!reader.EndOfStream)
            //    {
            //        string strLine = reader.ReadLine();

            //        if (strLine.Contains("1"))
            //        {
            //            result = new VariousData<bool>(true);
            //            break;
            //        }
            //        else if (strLine.Contains("0"))
            //        {
            //            result = new VariousData<bool>(false);
            //            break;
            //        }
            //    }

            //    reader.Close();

            //    if (result != null)
            //    {
            //        timer1.Stop();
            //        File.Delete(m_strReportResultFilePath);
            //        m_nCurrentMonitorTime = -1;
            //        m_ctrlReport.Enabled = true;

            //        if (!result.Data)
            //            UnE.Utility.UMessageBox.Show(this, "보고서가 생성되었습니다.\r\n" + m_strReportFilePath, "알림");
            //        else
            //            UnE.Utility.UMessageBox.Show(this, "보고서 생성에 실패하였습니다.\r\n" + m_strReportFilePath, "알림");
            //    }
            //}
            //else
            //{
            //    if (++m_nCurrentMonitorTime > m_nMonitorReportLimitSeconds)
            //    {
            //        timer1.Stop();
            //        m_ctrlReport.Enabled = true;
            //        m_nCurrentMonitorTime = -1;
            //        UnE.Utility.UMessageBox.Show(this, "보고서 생성에 실패하였습니다.\r\n" + m_strReportFilePath, "알림");
            //    }
            //}
        }

        private void menuHelp_Click(object sender, EventArgs e)
        {
            ShowHelp();
        }

        public void ShowHelp()
        {
            string strPath = Application.StartupPath + "\\help.chm";

            if (File.Exists(strPath))
                Help.ShowHelp(this, strPath);
        }

		private void updateCmdTimer_Tick(object sender, EventArgs e)
		{
			if( UndoRedoManager.Instance.UndoCount > 0)
			{
				rbtnUndo.Enabled = true;
				menuUndo.Enabled = true;
			}
			else
			{
				rbtnUndo.Enabled = false;
				menuUndo.Enabled = false;
			}

			if (UndoRedoManager.Instance.RedoCount > 0)
			{
				rbtnRedo.Enabled = true;
				menuRedo.Enabled = true;
			}
			else
			{
				rbtnRedo.Enabled = false;
				menuRedo.Enabled = false;
			}

		}

		private void menuSearch_Click(object sender, EventArgs e)
		{

			BeginSearch();
		}

		private void menuUndo_Click(object sender, EventArgs e)
		{
			if( UndoRedoManager.Instance.UndoCount > 0)
			{
				PanelDXFViewer pane = FormMain.Instance.CurrentPanel;
				if( pane!= null)
				{
					pane.ProcessResultForm.ClosePropertyForm();
					pane.ProcessScheduleForm.CloseScheduleProperty();
				}
				UndoRedoManager.Instance.Undo();
			}
		}

		private void menuRedo_Click(object sender, EventArgs e)
		{
			if (UndoRedoManager.Instance.RedoCount > 0)
			{
				PanelDXFViewer pane = FormMain.Instance.CurrentPanel;
				if (pane != null)
				{
					pane.ProcessResultForm.ClosePropertyForm();
					pane.ProcessScheduleForm.CloseScheduleProperty();
				}
				UndoRedoManager.Instance.Redo();
			}
		}
    }
}
