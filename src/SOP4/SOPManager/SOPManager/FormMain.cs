using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows;
using System.Runtime.InteropServices;
using UnE.GUI;
using DBUtility;

namespace SOPManager
{
	public partial class FormMain : Form, ITextPictureBoxOwner, IRibbonButtonOwner, IMenuCommandOwner
	{
		#region Status Bar Key 상태
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
		public static extern short GetKeyState(int keyCode);
		public void OnIdle(object sender, EventArgs e)
		{
			// Update the panels when the program is idle.
			bool CapsLock = (((ushort)GetKeyState(0x14 /*VK_CAPITAL*/)) & 0x01) == 0x01;
			bool NumLock = (((ushort)GetKeyState(0x90 /*VK_NUMLOCK*/)) & 0x01) == 0x01;
			bool Hangul = (((ushort)GetKeyState(0x15 /*VK_NUMLOCK*/)) & 0x01) == 0x01;

			if (mStatsCaps != null)
			{
				mStatsCaps.Text = CapsLock ? "CAP" : "";
			}
			if (mStatusNum != null)
			{
				mStatusNum.Text = NumLock ? "NUM" : "";
			}
			if (mStatusHanguel != null)
			{
				mStatusHanguel.Text = Hangul == false ? "영문" : "한글";
			}
		}
		#endregion

		private static FormMain m_instance = null;

		public static SOPManager.FormMain Instance
		{
			get { return m_instance; }
		}

        private FormRibbon mRibbonForm = new FormRibbon();
        private FormOpenDB form = null;
        private FormFrameDialog mOpenForm = null;
        private UnE.GUI.DialogFormFrame frameNewSOP = null;
        
		// Form 최소 사이즈
		protected Size m_nMinSize = new Size(1600, 900);

		// 선택된 탭
		protected int m_nSelectTab = 0;
		public int SelectedTab
		{
			get { return m_nSelectTab; }
			set { m_nSelectTab = value; }
		}

		// 공용으로 사용될 SOP Tree Form
		protected BarLevelTree m_sopTree = new BarLevelTree();
		public SOPManager.BarLevelTree SopTree
		{
			get { return m_sopTree; }
		}

		protected WebDBManager m_dbMgr = null;
		public WebDBManager DBManager
		{
			get { return m_dbMgr; }
		}

		//private TabPage m_tapPageCopySrc = null;

		//private FormNewSOP m_formNewSOP;
		//private PageBackstagePage m_pagePage;
		private FormPageSOP m_pageSOP;
		private PageBackstageHelp m_pageHelp;

		private Dictionary<int, Data_NormalTeam> m_dicNormalTeam = new Dictionary<int, Data_NormalTeam>();
		private PopupSpecialMessage m_frmSpecialMessage = null;

		bool m_isFirst = false;

		private int m_nSOPGenUserID = -1;
		private string m_strSOPGenUserID = "";
		private string m_strSOPGenUserRealName = "";

		private VersionInfo m_versionCurrent = null;

        public bool UseStepMember
        {
            get { return false; }
        }

		//////////////////////////////////////////////////////////////////////////
		ArrayList m_arrFullPath = new ArrayList();

		public ArrayList FullPath
		{
			get { return m_arrFullPath; }
			set { m_arrFullPath = value; }
		}

		//////////////////////////////////////////////////////////////////////////
		// DB List
		private ArrayList m_arrDisaster = new ArrayList();
		private ArrayList m_arrSubDisaster = new ArrayList();
		private ArrayList m_arrDetail = new ArrayList();
		private ArrayList m_arrNormalTeam = new ArrayList();
		private ArrayList m_arrEmergencyTeam = new ArrayList();
		private ArrayList m_arrCheckTask = new ArrayList();
		private ArrayList m_arrRegularTeam = new ArrayList();
        private ArrayList m_arrControlRoom = new ArrayList();
		private ArrayList m_arrActionStep = new ArrayList();
		private ArrayList m_arrUserDefinedTeam = new ArrayList();
		private ArrayList m_arrExternalTeam = new ArrayList();
        private ArrayList m_arrExternalCompanyTeam = new ArrayList();
		private ArrayList m_arrSOPVersion = new ArrayList();

		public ArrayList DisasterCategory
		{
			get { return m_arrDisaster; }
			set { m_arrDisaster = value; }
		}

		public ArrayList SubDisasterCategory
		{
			get { return m_arrSubDisaster; }
			set { m_arrSubDisaster = value; }
		}

		public ArrayList DetailDisasterCategory
		{
			get { return m_arrDetail; }
			set { m_arrDetail = value; }
		}

		public ArrayList TemporaryNormalTeam
		{
			get { return m_arrNormalTeam; }
			set { m_arrNormalTeam = value; }
		}

		public ArrayList TemporaryEmergencyTeam
		{
			get { return m_arrEmergencyTeam; }
			set { m_arrEmergencyTeam = value; }
		}

		public ArrayList CheckTask
		{
			get { return m_arrCheckTask; }
			set { m_arrCheckTask = value; }
		}

		public ArrayList RegularTeam
		{
			get { return m_arrRegularTeam; }
			set { m_arrRegularTeam = value; }
		}
        public ArrayList ControlRoom
        {
            get { return m_arrControlRoom; }
            set { m_arrControlRoom = value; }
        }
		public ArrayList ActionStep
		{
			get { return m_arrActionStep; }
			set { m_arrActionStep = value; }
		}
		public ArrayList UserDefinedTeam
		{
			get { return m_arrUserDefinedTeam; }
			set { m_arrUserDefinedTeam = value; }
		}
		public ArrayList ExternalTeam
		{
			get { return m_arrExternalTeam; }
			set { m_arrExternalTeam = value; }
		}

		public ArrayList SOPVersion
		{
			get { return m_arrSOPVersion; }
			set { m_arrSOPVersion = value; }
		}

		public override string Text
		{
			get
			{
				if (this.Parent == null)
					return "";
				return this.Parent.Text;
			}
			set
			{
				if (this.Parent != null)
					this.Parent.Text = value;
			}
		}

        public void SetTitleText(string szText)
        {
            string[] sepList = { "/" };
            string[] textList = szText.Split(sepList, StringSplitOptions.RemoveEmptyEntries);
            if (textList.Length < 3)
            {
                Text = FormFrame.Instance.Title + " - " + szText;
                return;
            }
            Text = FormFrame.Instance.Title + " - " + textList[2];
        }

		protected ArrayList m_arRibbonButtons = new ArrayList();

		/// <summary>
		/// UI 생성되기 이전에 필요한 DB Data 로드
		/// Form 생성 이전에 호출
		/// </summary>
		public void LoadBaseData()
		{
            //ReadSiteID();

			ReadDisasterCategory();
			ReadSubDisasterCategory();

			ReadDisasterType();
			ReadDisaster();

			ReadTeamporaryNormalTeam();
			ReadTeamporaryEmergencyTeam();
			
            //ReadCheckTask();
			ReadRegularTeam();
            ReadControlRoom();
			ReadActionStep();
			ReadUserDefinedTeam();
			ReadExternalTeam();
			ReadVersion(); 
		}

        private int m_nSiteID = 1;
        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        private void ReadSiteID()
        {
            //WebDBManager dbMan = new WebDBManager();
            Utility util = new Utility();
            string szSection = "Server Connection Info";
            string szText = util.getinivalue(szSection, "siteid");
            if(!int.TryParse(szText, out m_nSiteID))
            {
                m_nSiteID = 1;
            }
        }

		/// <summary>
		/// UI가 생성된 이후에 사용될 DB Data 로드
		/// Form Load 이벤트에서 호출
		/// </summary>
		public void LoadExtraData()
		{
		}

		public FormMain(int nSOPGenUserID, string strSOPGenUserID, string strSOPGenUserRealName)
		{
            ReadSiteID();

			UnE.Utility.UMessageBox.FrameColor = Color.FromArgb(43, 43, 43);

			UnE.GUI.DialogFormFrame.BorderColor = Color.FromArgb(43, 43, 43);
			UnE.GUI.DialogFormFrame.TitleBarColor = Color.FromArgb(43, 43, 43);

			UnE.Utility.UMessageBox.CloseButtonImage = global::SOPManager.Properties.Resources.CloseWindow_Normal;
			m_instance = this;

			m_nSOPGenUserID = nSOPGenUserID;
			m_strSOPGenUserID = strSOPGenUserID;
			m_strSOPGenUserRealName = strSOPGenUserRealName;

            m_dbMgr = new SOPManager.WebDBManager(m_nSiteID);
            /*if(m_nSiteID == 1)
			    m_dbMgr = new WebDBManager("SOP3");
            else if( m_nSiteID == 2)
                m_dbMgr = new WebDBManager("SOP4");*/
			//m_dbMgr = new WebDBManager("SOP4");

			LoadBaseData();

			InitializeComponent();
		
			Application.Idle += new System.EventHandler(OnIdle);

			FindNormalPath();

			GetDefaultBoundary();

            UnE.GUI.DialogFormFrame.BorderColor = Color.FromArgb(81, 81, 255);
            UnE.GUI.DialogFormFrame.TitleBarColor = Color.FromArgb(81, 81, 255);
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{			
			return base.ProcessCmdKey(ref msg, keyData);
		}

		public void OnStartFrame()
		{	
			FormStart start = new FormStart();
			UnE.GUI.DialogFormFrame frame = new UnE.GUI.DialogFormFrame(start);
			frame.ShowMaxButton = false;
			frame.ShowMinButton = false;
			frame.Sizable = false;
			if (frame.ShowDialog(this) == DialogResult.OK)
			{
				int nSelected = start.OpenType;
				if (nSelected == 1)
				{					
					frameNewSOP = new UnE.GUI.DialogFormFrame(new FormNewSOP2());
					frameNewSOP.StartPosition = FormStartPosition.CenterParent;
					frameNewSOP.Text = "새 SOP";
					frameNewSOP.Sizable = false;
					//frameNewSOP.Size = new Size(1150, 740);		
					frameNewSOP.ShowMaxButton = false;
					frameNewSOP.ShowMinButton = false;
					frameNewSOP.Sizable = false;
					if (frameNewSOP.ShowDialog(this) == DialogResult.OK)
					{
					}
				}
				else if (nSelected == 2)
				{
					mOpenForm = new FormFrameDialog(new FormOpenDB());
					mOpenForm.StartPosition = FormStartPosition.CenterParent;
					mOpenForm.Text = "시나리오 열기";
					mOpenForm.Sizable = false;
					mOpenForm.Size = new Size(950, 670);
					mOpenForm.PictureBoxSize = new Size(20, 20);
					mOpenForm.PictureBoxTitleImage = global::SOPManager.Properties.Resources.열기_normal;
					mOpenForm.ShowDialog();
				}
				else if (nSelected == 3)
				{
					OpenSOPXML();
				}
			}
		}

		private void FormMain_Load(object sender, EventArgs e)
		{
			CreateBackstageView();

			InitPanel();

			InitTabButton();
            
			LoadExtraData();

			OnShowForm(typeof(FormOpenDB), true);

			m_tmrCmdUpdate.Enabled = true;
			m_tmrCmdUpdate.Start();

			FormMain_Activated(null, null);

			//this.BeginInvoke(new Action(() => OnStartFrame()));
		}
		
		protected void InitPanel()
		{

			panelTop.Controls.Add(mRibbonForm);
			mRibbonForm.RibbonButtonOwner = this;
			mRibbonForm.Show();
			panelContent.Dock = DockStyle.Fill;
			panelSection.Dock = DockStyle.Fill;

			panelSection.Visible = false;

			form = new FormOpenDB();
			form.Dock = DockStyle.Fill;
			panelForm.Controls.Add(form);
            
			panelContent.Visible = false;
			panelSection.Visible = true;
		}		

		protected void InitTabButton()
		{
			m_nSelectTab = 0;
		}	   

		private void ArrangeRibbonButtonAddGap(RibbonButton btnPrev, RibbonButton btnNext, int gap)
		{
			btnNext.Location = new Point(btnPrev.Location.X + btnPrev.Size.Width + gap, btnPrev.Location.Y);
		}

		private void ArrangeRibbonButton(RibbonButton btnPrev, RibbonButton btnNext)
		{
			btnNext.Location = new Point(btnPrev.Location.X + btnPrev.Size.Width, btnPrev.Location.Y);
		}
        
		#region Change Tab Mouse 이벤트, Select Tab

		public void SelectTab(int nTab)
		{
			m_nSelectTab = nTab;
			if (m_nSelectTab == 0)
			{
				//pictureBoxFile.BackgroundImage = global::SOPManager.Properties.Resources.Tab_Pressed;
				//pictureBoxSOP.BackgroundImage = global::SOPManager.Properties.Resources.Tab_Normal;

				panelContent.Visible = true;
				panelSection.Visible = false;

				//panelRibbon.Visible = false;

				//if (panelGap.Visible == false)
				
				//panelGap.Visible = true;
			}
			else if (m_nSelectTab == 1)
			{
			   // pictureBoxFile.BackgroundImage = global::SOPManager.Properties.Resources.Tab_Normal;
				//pictureBoxSOP.BackgroundImage = global::SOPManager.Properties.Resources.Tab_Pressed;

			   // panelRibbon.Visible = true;
				//panelGap.Visible = false;


				panelContent.Visible = false;
				panelSection.Visible = true;



			}
		}

		public void TextPictureBox_MouseDown(TextPictureBox pictureBox, MouseEventArgs e)
		{
			if (e != null)
			{
				if (e.Button != System.Windows.Forms.MouseButtons.Left)
					return;
			}

			//if (pictureBox == pictureBoxFile)
			//{
			//	if (m_nSelectTab == 1)
			//	{
			//		SelectTab(0);
			//	}
			//}
			//else if (pictureBox == pictureBoxSOP)
			//{
			//	if (m_nSelectTab == 0)
			//	{
			//		SelectTab(1);
			//	}
			//}
		}
		#endregion


		private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
		{
			m_tmrCmdUpdate.Stop();
			m_tmrCmdUpdate.Enabled = false;

			if(m_frmForMessage != null)
			{
				m_frmForMessage.Close();
			}
		}

		private void m_tmrCmdUpdate_Tick(object sender, EventArgs e)
		{
			m_arRibbonButtons = mRibbonForm.RibbonButtons;
			foreach (RibbonButton rb in m_arRibbonButtons)
			{
				OnRibbonButtonUpdate(rb, rb.ID);
			}
		}

		public void OnRibbonButtonMouseDown(object sender, MouseEventArgs e)
		{
			RibbonButton btnRB = (RibbonButton)sender;
			btnRB.Refresh();
		}

		private bool IsShowForm(Type formType)
		{
			foreach (Control control in panelForm.Controls)
			{
				if (control.GetType() == formType)
				{
					return control.Visible;
				}
			}
			return false;
		}

		private void OnShowForm(Type formType, bool bShow)
		{
			foreach (Control control in panelForm.Controls)
			{
				if (control.GetType() == formType)
				{
					control.Visible = bShow;
					break;
				}
			}
		}

		public Form GetForm(Type formType)
		{
			foreach (Control control in panelForm.Controls)
			{
				if (control.GetType() == formType)
				{
					return (Form)control;
				}
			}
			return null;
		}

		public void OnRibbonButtonMouseUp(object sender, MouseEventArgs e)
		{
			RibbonButton btnRB = (RibbonButton)sender;

			OnRibbonButtonExecute(sender, btnRB.ID);

			btnRB.Refresh();
		}

		private static ArrayList GetDefaultBoundary()
		{
			ArrayList arrBoundary = new ArrayList();
			float fWidth = 150.0f, fHeight = 80.0f;

			// Bezier Curve를 그리기 위한 기준점 설정
			UnE.Geometry.Vertex2D[] arrCurvePoints = new UnE.Geometry.Vertex2D[4];

			arrCurvePoints[0] = new UnE.Geometry.Vertex2D(0, 0);
			arrCurvePoints[1] = new UnE.Geometry.Vertex2D(fWidth / 3, fWidth * 0.2);
			arrCurvePoints[2] = new UnE.Geometry.Vertex2D(fWidth * 2 / 3, -fWidth * 0.2);
			arrCurvePoints[3] = new UnE.Geometry.Vertex2D(fWidth, 0);
			////////////////////////////////////////////////////////////////

			// Bezier Curve 얻어오기
			int nResultCount = 100;
			UnE.Geometry.Vertex2D[] arrResultPoints = new UnE.Geometry.Vertex2D[nResultCount];

			UnE.Geometry.BezierCurve2D bezier = new UnE.Geometry.BezierCurve2D();

			if (!bezier.Calc(arrCurvePoints, arrCurvePoints.Count(), arrResultPoints, nResultCount))
				return arrBoundary;
			////////////////////////////////////////////////////////////////

			// Boundary Vertex 설정
			for (int i = 0; i < nResultCount; i++)
			{
				UnE.Geometry.Vertex2D vertex = arrResultPoints[i];
				arrBoundary.Add(new PointF((float)vertex.x, (float)vertex.y));
			}

			for (int i = nResultCount - 1; i >= 0; i--)
			{
				UnE.Geometry.Vertex2D vertex = arrResultPoints[i];
				arrBoundary.Add(new PointF((float)vertex.x, (float)vertex.y - fHeight));
			}
			////////////////////////////////////////////////////////////////

			return arrBoundary;
		}

		private void ReadDB()
		{
			ReadDisasterCategory();
			ReadSubDisasterCategory();
			ReadDisasterType();
			ReadDisaster();
			ReadTeamporaryNormalTeam();
			ReadTeamporaryEmergencyTeam();
			//ReadCheckTask();
			ReadRegularTeam();
            ReadControlRoom();
			ReadActionStep();
			ReadUserDefinedTeam();
			ReadExternalTeam();
			ReadVersion();
		}

		public static Bitmap GetImageByName(string imageName)
		{
			//System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("neutral");
			System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
			string resourceName = "SOPManager.Properties.Resources";
			var rm = new System.Resources.ResourceManager(resourceName, asm);
			return (Bitmap)rm.GetObject(imageName);
		}

		private void CreateBackstageView()
		{
			if (m_pageSOP == null)
				m_pageSOP = new FormPageSOP();

			if (m_pageHelp == null)				
				m_pageHelp = new PageBackstageHelp();
		}

		public void CreateSOP(List<SelectTeamInfo> arSelectedTeamList)
		{
			string strRegular = "미등록모드";
			string strWeekday = "야간 및 휴일";
			
			bool bRegular = SopDocManager.Instance.RegularMode;
			bool bWeek = SopDocManager.Instance.WeekMode;

			if (bRegular)
				strRegular = "등록모드";
			if (bWeek)
				strWeekday = "평일";

			this.Text = "SOP Manager  V 2.0 - " + strRegular + ", " + strWeekday;
			
			NewSOP();
			
			if (m_pageSOP != null)
			{

				m_pageSOP.Show();

				// SOP에 해당하는 TabPage추가
				m_pageSOP.AddTabPage();

				string szLevelName = m_pageSOP.GetTabPageName();
				string szPath = SopDocManager.Instance.GetLevelPath();

				szPath = szPath + "/" + szLevelName;			

				// SOP TreeNode 추가
				m_pageSOP.GetBarLevelTree().AddTreeNode();
				
				// LevelProperty에 타이틀 추가
				m_pageSOP.GetPropertiesLevel().SetTitleText(szPath);
				
				// 선택된 팀으로 새로운 작업 패널 추가

				m_pageSOP.AddPane(arSelectedTeamList);
				// 패널 위치 조정
				m_pageSOP.PanelResize();

				// 패널 정보를 변경
				m_pageSOP.GetBarPage().SetDataGrid();
			}
			
			// 새 SOP창 닫기
			if (frameNewSOP.Visible == true)
			{
				frameNewSOP.Close();
			}

			// RibbonTab 을 편집 모드로 변경
			mRibbonForm.SelectTab(1);

			// 제거 필요 -> SelectTab제거
			SelectTab(1);

			// 컴포넌트와 속성창 열기
			ShowRightPane(true);
		}

		private void OnRibbonButtonUpdate(RibbonButton btn, int nID)
		{
			switch (nID)
			{
				case ID.ID_FILE_START:					
					break;
				case ID.ID_FILE_OPEN:
					break;
				case ID.ID_FILE_NEWSOP:
					if (frameNewSOP != null && frameNewSOP.Visible == true)
					{
						btn.IsChecked = true;
					}
					else
					{
						btn.IsChecked = false;
					}
					break;
				case ID.ID_FILE_DELETE:
					break;
				case ID.ID_EDIT_LEVEL_DEL:
					if (m_pageSOP.GetTabPages().Count == 0)
						btn.Enabled = false;
					else
						btn.Enabled = true;
					break;
				case ID.ID_EDIT_LEVEL_ADD:
					if (m_pageSOP.GetBarLevelTree().ExistNode())
						btn.Enabled = true;
					else
						btn.Enabled = false;
					break;
				case ID.ID_EDIT_LEVEL_COPY:
					if (m_pageSOP.GetTabPages().Count == 0)
						btn.Enabled = false;
					else
						btn.Enabled = true;
					break;
				case ID.ID_EDIT_LEVEL_PASTE:
					if (LevelClipboard.Instance.IsContainsData == false)
						btn.Enabled = false;
					else
						btn.Enabled = true;
					break;

				case ID.ID_EDIT_UNDO:
					if (UndoRedoManager.Instance.UndoCount > 0)
						btn.Enabled = true;
					else
						btn.Enabled = false;
					break;
				case ID.ID_EDIT_REDO:
					if (UndoRedoManager.Instance.RedoCount > 0)
						btn.Enabled = true;
					else
						btn.Enabled = false;
					break;

				case ID.ID_PANE_DELETE:
					TabPage page = m_pageSOP.GetCurrentTabPage();
					if (page != null && Sections.PanelSectionEx.GetTabPageTeamList(page).Count > 0)
					{
						btn.Enabled = true;
					}
					else
					{
						btn.Enabled = false;
					}
					break;
				case ID.ID_PANE_PASTE:			
					bool bEnabled = PanelClipboard.Instance.IsContainsData;
					btn.Enabled = bEnabled;
					break;
				case ID.ID_PANE_COPY:
				case ID.ID_PANE_ADD:
					TabPage page2 = m_pageSOP.GetCurrentTabPage();
					if (page2 != null)
					{
						btn.Enabled = true;
					}
					else
					{
						btn.Enabled = false;
					}
					break;
				case ID.ID_VIEW_RIGHTPANE:
					bool bShow = m_pageSOP.IsShowRightPane;
					btn.IsChecked = bShow;
					break;

				case ID.ID_EDIT_COPY:
				case ID.ID_EDIT_DELETE:
				case ID.ID_EDIT_CUT:
					bool bSelected = m_pageSOP.IsSelectedSection();
					btn.Enabled = bSelected;
					break;
				case ID.ID_EDIT_PASTE:
					int nCount = SectionClipboardEx.Instance.EditSectionCount;
					if (nCount > 0)
						btn.Enabled = true;
					else
						btn.Enabled = false;
					break;
			};

			btn.Refresh();
		}

		public void ShowRightPane(bool bShow)
		{			
			if (m_pageSOP != null)
				m_pageSOP.ShowRightPane(bShow);
		}
        
        public bool SaveAsToDB()
        {
            if (CheckSOP())
            {
                if (SaveAsSOP())
                {
                    UndoRedoManager.Instance.Reset();



                    this.SetStatusText("DB에 다른이름으로 저장이 완료되었습니다.");

                    m_pageSOP.ClearModify();

                    return true;
                }
                else
                {


                }
            }
            return false;
        }

		public bool SaveToDB()
		{
			if (CheckSOP())
			{
				if( SaveSOP())
				{
					UndoRedoManager.Instance.Reset();
					this.SetStatusText("DB에 저장이 완료되었습니다.");

					m_pageSOP.ClearModify();

					return true;
				}		
				else
				{
					
					
				}
			}
			return false;
		}

		public bool SaveToXML(bool bSaveAs = false)
		{
			if (CheckSOP())
			{
				if( bSaveAs == false)
				{
					if (SaveSOPXML())
					{
						UndoRedoManager.Instance.Reset();

						string szFilePath = SopDocManager.Instance.FilePath;
						this.SetStatusText(szFilePath + " 저장이 완료되었습니다.");

						m_pageSOP.ClearModify();

						return true;
					}
				}
				else
				{
					if (SaveAsSOPXML())
					{
						UndoRedoManager.Instance.Reset();

						string szFilePath = SopDocManager.Instance.FilePath;
						this.SetStatusText("새파일  "+ szFilePath + "에 저장이 완료되었습니다.");

						m_pageSOP.ClearModify();

						return true;
					}
				}
			}
			return false;
		}

		private void OnRibbonButtonExecute(object sender, int nID)
		{
			RibbonButton btnRB = (RibbonButton)sender;
			switch (nID)
			{
				case ID.ID_FILE_START:
					BeginInvoke(new Action(() => OnStartFrame()));
					break;
				case ID.ID_FILE_OPEN:	
					
					if( m_pageSOP.InitSectionPanel() == true)
					{
						mOpenForm = new FormFrameDialog(new FormOpenDB());
						mOpenForm.StartPosition = FormStartPosition.CenterParent;
						mOpenForm.Text = "시나리오 열기";
						mOpenForm.Sizable = false;
						mOpenForm.Size = new Size(950, 670);
						mOpenForm.PictureBoxSize = new Size(20, 20);
						mOpenForm.PictureBoxTitleImage = global::SOPManager.Properties.Resources.열기_normal;
						mOpenForm.ShowDialog();
					}					
					break;
				case ID.ID_FILE_DELETE:
					//if (m_pageSOP.InitSectionPanel() == true)
					//{
					FormOpenDB dbOpenForm = new FormOpenDB();
					dbOpenForm.SelectChangePage(nID);
					mOpenForm = new FormFrameDialog(dbOpenForm);
					mOpenForm.StartPosition = FormStartPosition.CenterParent;
					mOpenForm.Text = "시나리오 삭제";
					mOpenForm.Sizable = false;
					mOpenForm.Size = new Size(950, 670);
					mOpenForm.PictureBoxSize = new Size(20, 20);
					mOpenForm.PictureBoxTitleImage = global::SOPManager.Properties.Resources.삭제_normal;
					mOpenForm.ShowDialog();
					//}	
					break;
				case ID.ID_FILE_SAVE:
					SaveToDB();
					break;
				case ID.ID_FILE_SAVE_AS:
                    SaveAsToDB();
					break;

				case ID.ID_XML_SAVE:
					SaveToXML(false);					
					break;
				case ID.ID_XML_OPEN:
					UndoRedoManager.Instance.Reset();
					OpenSOPXML();					
					break;
				case ID.ID_XML_SAVEAS:
					SaveToXML(true);
					break;

				case ID.ID_FILE_NEWSOP:
					if (m_pageSOP.InitSectionPanel())
					{
						//m_formNewSOP = ;
						frameNewSOP = new UnE.GUI.DialogFormFrame(new FormNewSOP2());
						frameNewSOP.StartPosition = FormStartPosition.CenterParent;
						frameNewSOP.Text = "새 SOP";
						frameNewSOP.Sizable = false;
						//frameNewSOP.Size = new Size(1150, 740);

						frameNewSOP.ShowMaxButton = false;
						frameNewSOP.ShowMinButton = false;
						frameNewSOP.Sizable = false;
						if (frameNewSOP.ShowDialog(this) == DialogResult.OK)
						{
						}						
					}					
					break;
				
				case ID.ID_EDIT_UNDO:
					UndoRedoManager.Instance.Undo();
					break;
				case ID.ID_EDIT_REDO:
					UndoRedoManager.Instance.Redo();
					break;
				case ID.ID_APP_EXIT:
					this.Close();
					break;
				case ID.ID_EDIT_LEVEL_ADD:
					{
						AddLevel();
					}
					break;
				case ID.ID_EDIT_LEVEL_DEL:

					RemoveLevel();
					break;

				case ID.ID_EDIT_LEVEL_COPY:
					ActionStepTabPage copyTab =  m_pageSOP.GetCurrentTabPage();
					LevelClipboard.Instance.CopyTab(copyTab);
					break;

				case ID.ID_EDIT_LEVEL_PASTE:
					ActionStepTabPage pasteTab =  m_pageSOP.GetCurrentTabPage();
					LevelClipboard.Instance.PasteTab(pasteTab);
					break;

				case ID.ID_PANE_ADD:
					{
						if (UnE.Utility.UMessageBox.Show("현재 패널의 오른쪽에 새로운 패널을 추가합니다.\r\n이 작업은 현재 열려있는 모든 탭들에 영향을 주게 됩니다.\r\n계속하시겠습니까?", "알림", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
							== DialogResult.Yes)
						{

							TabPage tabPage = m_pageSOP.GetCurrentTabPage();
							ArrayList arrOldTeams = Sections.PanelSectionEx.GetTabPageTeamList(tabPage);
							if (arrOldTeams != null)
							{
								bool bWeekly = SopDocManager.Instance.WeekMode;
                                Sections.SOPTeam.SOPTeamType nTeamType = (bWeekly == true ? Sections.SOPTeam.SOPTeamType.Normal : Sections.SOPTeam.SOPTeamType.Holiday);


								PopupSelectTeam3 frm = new PopupSelectTeam3(nTeamType, arrOldTeams);								
								frm.Text = "새로운 Panel 생성";
								UnE.GUI.DialogFormFrame frame = new UnE.GUI.DialogFormFrame(frm);
								if (frame.ShowDialog(this) == DialogResult.OK)
								{
									StepMemberData data = new StepMemberData();
									data.TeamID = frm.SelectedTeamID;
									data.TeamName = frm.SelectedTeamName;
									data.TeamType = frm.SelectedTeamType;

									int nCurrentIndex = Sections.PanelSectionEx.GetLastTabPagePanelIndex(tabPage);

									UndoRedoManager.Instance.SaveSnapshot();

									m_pageSOP.AddPanel(data, nCurrentIndex + 1);
									m_pageSOP.GetBarPage().InsertDataGrid(nCurrentIndex + 1, data);
									m_pageSOP.PanelResize();
								}
							}

						}
					}
					break;

				case ID.ID_PANE_COPY:
					Sections.PanelSectionEx paneCopy = m_pageSOP.GetCurrentPanel();
					if (paneCopy != null)
					{
						PanelClipboard.Instance.CopyPanel(paneCopy);
					}
					break;

				case ID.ID_PANE_PASTE:
					Sections.PanelSectionEx panePaste = m_pageSOP.GetCurrentPanel();
					if (panePaste != null)
					{
						PanelClipboard.Instance.PastePanel(panePaste);
					}
					break;


				case ID.ID_PANE_DELETE:
					m_pageSOP.DeletePanelLast();
					m_pageSOP.PanelResize();
					break;

				case ID.ID_VIEW_RIGHTPANE:
					bool bShow = m_pageSOP.IsShowRightPane;
					ShowRightPane(!bShow);
					break;

				case ID.ID_EDIT_COPY:
					Sections.PanelSectionEx panel = m_pageSOP.GetCurrentPanel();
					if (panel != null)
					{
						SectionClipboardEx.Instance.Copy(panel);
						panel.Refresh();
					}

					break;

				case ID.ID_EDIT_CUT:
					Sections.PanelSectionEx panel2 = m_pageSOP.GetCurrentPanel();
					if (panel2 != null)
					{
						SectionClipboardEx.Instance.Cut(panel2);
						panel2.Refresh();
					}					
					break;

				case ID.ID_EDIT_PASTE:
					Sections.PanelSectionEx panel3 = m_pageSOP.GetCurrentPanel();
					if (panel3 != null)
					{
						SectionClipboardEx.Instance.Paste(panel3);
						SectionClipboardEx.Instance.Canel();
						panel3.Refresh();
					}
					break;
				
				case ID.ID_EDIT_DELETE:
					Sections.PanelSectionEx panel4 = m_pageSOP.GetCurrentPanel();
					if (panel4 != null)
					{
						panel4.Delete();
						panel4.Refresh();
					}
					break;

				default:
					break;
			};
		}

		public void AddLevel()
		{
			PopupSelectLevel form = new PopupSelectLevel();
			UnE.GUI.DialogFormFrame frame = new DialogFormFrame(form);
			if (frame.ShowDialog(this) == DialogResult.OK)
			{
				string szLevelName = form.LevelName;

				TabPage tabPage = m_pageSOP.GetCurrentTabPage();
				//if (tabPage == null)
				{
					ArrayList arrTeams = (ArrayList)m_pageSOP.UsingTeam.Clone();

					if (arrTeams != null)
					{
						UndoRedoManager.Instance.SaveSnapshot();

						m_pageSOP.AddTabPage(szLevelName);
						m_pageSOP.AddPane(arrTeams, null, true);
						m_pageSOP.GetBarPage().SetDataGrid(arrTeams);
					}
				}
			}
		}

		public void RemoveLevel(string szName = "")
		{
			if (szName == "")
				m_pageSOP.RemoveTabPage();
			else
			{
				ActionStepTabPage page = m_pageSOP.GetTabPage(szName);
				if (page != null)
				{
					m_pageSOP.RemoveTabPage(page);
				}			
			}

			if (m_pageSOP.GetTabPages().Count == 0)
			{
				// Properties 초기화
				m_pageSOP.GetPropertiesLevel().ClearSelection();
				// Panel 리스트 초기화
				m_pageSOP.GetBarPage().ClearGrid();
			}

			RefreshLevelProperties();
		}

		public void RefreshLevelProperties()
		{
			m_pageSOP.GetPropertiesLevel().Refresh();
			m_pageSOP.GetBarLevelTree().Refresh();
		}
		
		private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
		{
			//if (m_formNewSOP != null)
			//	m_formNewSOP.Dispose();
			if (m_pageSOP != null)
				m_pageSOP.Dispose();
			if (m_pageHelp != null)
				m_pageHelp.Dispose();
		}

		private void FormMain_Activated(object sender, EventArgs e)
		{
			if (!m_isFirst)
			{
				m_pageSOP.Location = new Point(0, 0);
				m_pageSOP.Dock = DockStyle.Fill;
				m_pageSOP.TopLevel = false;
				m_pageSOP.Parent = this;
				panelSectionContent.Controls.Add(m_pageSOP);
				m_pageSOP.Show();
				m_isFirst = true;
			}
		}

		public void NewSOP()
		{
			m_pageSOP.RemoveAll();

			// 기존 Section들의 ID 정보 초기화
			//Sections.SectionData.ClearIDList();

			ArrayListClear();
			ReadDB();

			FindNormalPath();

			m_versionCurrent = null;


			UndoRedoManager.Instance.Reset();
		}

		private void ArrayListClear()
		{
			DisasterCategory.Clear();
			SubDisasterCategory.Clear();
			DetailDisasterCategory.Clear();
			TemporaryNormalTeam.Clear();
			TemporaryEmergencyTeam.Clear();
			CheckTask.Clear();
			RegularTeam.Clear();
			ActionStep.Clear();
			UserDefinedTeam.Clear();
			ExternalTeam.Clear();
			SOPVersion.Clear();
		}
		//////////////////////////////////////////////////////////////////////////
		//public FormNewSOP GetPageDisaster()
		//{
		//	return m_formNewSOP;
		//}

		public FormPageSOP GetPageLevel()
		{
			return m_pageSOP;
		}

		public PageBackstageHelp GetPageHelp()
		{
			return m_pageHelp;
		}


		// 사용중인 Disaster의 이름을 변경하는 경우 별도로 UI를 변경한다
		public void ChangeDisasterName(string szOrgName, string szNewName)
		{
			BarLevelTree tree = m_pageSOP.GetBarLevelTree();
			tree.ChangeDisasterName(szOrgName, szNewName);

			SopDocManager.Instance.DisasterName = szNewName;
			m_pageSOP.GetPropertiesLevel().Refresh();
		}
		 
		//////////////////////////////////////////////////////////////////////////
		// Read DB 
		private void ReadDisasterCategory()
		{
            // Site별로 사용할 수 있도록 수정 , Edit by skkim 2015.01.08
            string strSql = string.Format("SELECT ID,CategoryName FROM DisasterCategory where SiteID = {0}", m_nSiteID);            
			ArrayList arrResult = m_dbMgr.GetResultData(strSql, 0);
            if (arrResult == null)
                return;

			for (int i = 0; i < arrResult.Count - 1; i += 2)
			{
				Data_DisasterCategory dataNew = new Data_DisasterCategory();
				dataNew.ID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
				dataNew.CategoryName = WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");

				DisasterCategory.Add(dataNew);
			}
		}

		private void ReadSubDisasterCategory()
		{
            // Site별로 사용할 수 있도록 수정 , Edit by skkim 2015.01.08
			string szText = "SELECT sdc.ID, sdc.DisasterID, sdc.SubCategoryName " +
                            " FROM SubDisasterCategory as sdc , DisasterCategory as dc " + 
                            " WHERE sdc.DisasterID = dc.ID and dc.SiteID = {0}";

            string strSql = string.Format(szText, m_nSiteID);
            
			ArrayList arrResult = m_dbMgr.GetResultData(strSql, 0);
            if (arrResult == null)
                return;

			for (int i = 0; i < arrResult.Count - 2; i += 3)
			{
				Data_SubDisasterCategory dataNew = new Data_SubDisasterCategory();
				dataNew.ID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
				dataNew.DisasterID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), 0);
				dataNew.CategoryName = WebDBManager.GetStringField(arrResult[i + 2].ToString(), "");

				SubDisasterCategory.Add(dataNew);
			}
		}

        public SOPManager.Data_SubDisasterCategory GetSubDiasterCategory(int nSubDisasterCategoryID)
        {
            foreach (SOPManager.Data_SubDisasterCategory subCategory in m_arrSubDisaster)
            {
                if (subCategory.ID == nSubDisasterCategoryID)
                    return subCategory;
            }

            return null;
        }

        public SOPManager.Data_SubDisasterCategory GetSubDisasterCategory(SOPManager.Data_Disaster disaster)
        {
            if (disaster == null)
                return null;

            return GetSubDiasterCategory(disaster.SubDisasterID);
        }

		private ArrayList m_arrDisasterType = new ArrayList();
		public ArrayList DisasterTypeList
		{
			get { return m_arrDisasterType; }
			set { m_arrDisasterType = value; }
		}		
		
		private void ReadDisasterType()
		{
            // Site별로 사용할 수 있도록 수정 , Edit by skkim 2015.01.08
            //string strSQL = "SELECT * FROM DisasterType as dt WHERE ID NOT IN ( SELECT b.ID FROM [DisasterType] as b , Disaster as c WHERE cast([Name] as varbinary(2000)) = cast(c.DisasterName as varbinary(2000)) and b.SubDisasterID = c.SubDisasterID)";
            string szText = "SELECT dt.ID, dt.Name, dt.SubDisasterID " +
                            " FROM DisasterType as dt, SubDisasterCategory as sdc , DisasterCategory as dc" +
                            " WHERE dt.ID NOT IN " +
                            "  ( SELECT b.ID FROM DisasterType as b , Disaster as c " +
                            "    WHERE b.Name = c.DisasterName" +
                            "    AND b.SubDisasterID = c.SubDisasterID" +
                            "   )" +
                            " AND dt.SubDisasterID = sdc.ID and sdc.DisasterID = dc.ID and dc.SiteID = {0}";
           
            string strSql = string.Format(szText, m_nSiteID);

            ArrayList arrResult = m_dbMgr.GetResultData(strSql, 0);
            if (arrResult == null)
                return;
			for (int i = 0; i < arrResult.Count - 2; i += 3)
			{
				Data_Disaster dataNew = new Data_Disaster();
				//dataNew.ID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
				dataNew.DisasterName = WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");
				dataNew.SubDisasterID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);

				DetailDisasterCategory.Add(dataNew);
			}
		}


		private void ReadDisaster()
		{
            // Site별로 사용할 수 있도록 수정 , Edit by skkim 2015.01.08
			//string strSql = "SELECT * FROM Disaster";
            string szText = "SELECT ds.ID, ds.DisasterName, ds.SubDisasterID, ds.VersionID, ds.Description " +
                            " FROM Disaster as ds, SubDisasterCategory as sdc , DisasterCategory as dc" +
                            " WHERE ds.SubDisasterID = sdc.ID and sdc.DisasterID = dc.ID and dc.SiteID = {0}";

            string strSql = string.Format(szText, m_nSiteID);

			ArrayList arrResult = m_dbMgr.GetResultData(strSql, 0);
            if (arrResult == null)
                return;

			for (int i = 0; i < arrResult.Count - 4; i += 5)
			{
				Data_Disaster dataNew = new Data_Disaster();
				dataNew.ID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
				dataNew.DisasterName = WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");
				dataNew.SubDisasterID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
				dataNew.VersionID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), 0);
				dataNew.Description = WebDBManager.GetStringField(arrResult[i + 4].ToString(), "");

				DetailDisasterCategory.Add(dataNew);
			}
		}

        private void ReadTeamporaryNormalTeam()
        {
            // Site별로 사용할 수 있도록 수정 , Edit by skkim 2015.01.08
            string szText = "select ID, TeamName, ParentTeamID from TemporaryNormalTeam where SiteID = {0}";
            string strSql = string.Format(szText, m_nSiteID);

            ArrayList arrResult = m_dbMgr.GetResultData(strSql, 0);
            if (arrResult == null)
                return;

            for (int i = 0; i < arrResult.Count - 2; i += 3)
            {
                Data_NormalTeam dataNew = new Data_NormalTeam();

                dataNew.ID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                dataNew.TeamName = WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");
                dataNew.ParentTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);

                m_dicNormalTeam[dataNew.ID] = dataNew;
                TemporaryNormalTeam.Add(dataNew);
            }
        }

        //private void ReadTeamporaryNormalTeam()
        //{
        //    // Site별로 사용할 수 있도록 수정 , Edit by skkim 2015.01.08
        //    string szText = "select team.ID, TeamName, ParentTeamID, link.MemberID, link.MemberType from TemporaryNormalTeam as team, TemporaryMemberList as link where link.TemporaryTeamID = team.ID and link.IsNormal = 1 and team.SiteID = {0}";
        //    //string szText = "SELECT ID,TeamName,ParentTeamID,GroupName,LevelNo,Description,RegularTeamLink FROM TemporaryNormalTeam WHERE SiteID = {0}";
        //    string strSql = string.Format(szText, m_nSiteID);

        //    ArrayList arrResult = m_dbMgr.GetResultData(strSql, 0);
        //    Data_TemporaryTeam.TeamType teamType;

        //    for (int i = 0; i < arrResult.Count - 4; i += 5)
        //    {
        //        Data_NormalTeam dataNew = new Data_NormalTeam();

        //        dataNew.ID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
        //        dataNew.TeamName = WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");
        //        dataNew.ParentTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
        //        dataNew.TeamID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
        //        int nMemberType = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);

        //        if (!Data_TemporaryTeam.TryToTeamType(nMemberType, out teamType))
        //            continue;

        //        dataNew.SetTeamType(teamType);

        //        if (teamType == Data_TemporaryTeam.TeamType.JobLevel)
        //            dataNew.LevelNo = dataNew.TeamID;

        //        m_dicNormalTeam[dataNew.ID] = dataNew;
        //        TemporaryNormalTeam.Add(dataNew);
        //    }

        //    /*for (int i = 0; i < arrResult.Count - 6; i += 7)
        //    {
        //        Data_NormalTeam dataNew = new Data_NormalTeam();
        //        dataNew.ID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
        //        dataNew.TeamName = WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");
        //        dataNew.ParentTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
        //        dataNew.GroupName = WebDBManager.GetStringField(arrResult[i + 3].ToString(), "");
        //        dataNew.LevelNo = WebDBManager.GetIntField(arrResult[i + 4].ToString(), 0);
        //        dataNew.Description = WebDBManager.GetStringField(arrResult[i + 5].ToString(), "");
        //        dataNew.RegularTeamLink = WebDBManager.GetStringField(arrResult[i + 6].ToString(), "");

        //        m_dicNormalTeam[dataNew.ID] = dataNew;
        //        TemporaryNormalTeam.Add(dataNew);
        //    }*/
        //}

        private void ReadTeamporaryEmergencyTeam()
        {
            // Site별로 사용할 수 있도록 수정 , Edit by skkim 2015.01.08
            string szText = "select ID, TeamName, ParentTeamID from TemporaryEmergencyTeam where SiteID = {0}";
            string strSql = string.Format(szText, m_nSiteID);

            ArrayList arrResult = m_dbMgr.GetResultData(strSql, 0);
            if (arrResult == null)
                return;
            
            for (int i = 0; i < arrResult.Count - 2; i += 3)
            {
                Data_EmergencyTeam dataNew = new Data_EmergencyTeam();

                dataNew.ID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                dataNew.TeamName = WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");
                dataNew.ParentTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
            
                TemporaryEmergencyTeam.Add(dataNew);
            }
        }

        //private void ReadTeamporaryEmergencyTeam()
        //{
        //    //string strSql = "SELECT * FROM TemporaryEmergencyTeam";
        //    // Site별로 사용할 수 있도록 수정 , Edit by skkim 2015.01.08
        //    string szText = "select team.ID, TeamName, ParentTeamID, link.MemberID, link.MemberType from TemporaryEmergencyTeam as team, TemporaryMemberList as link where link.TemporaryTeamID = team.ID and link.IsNormal = 0 and team.SiteID = {0}";
        //    //string szText = "SELECT ID,TeamName,ParentTeamID,GroupName,LevelNo,Description,RegularTeamLink FROM TemporaryEmergencyTeam WHERE SiteID = {0}";
        //    string strSql = string.Format(szText, m_nSiteID);

        //    ArrayList arrResult = m_dbMgr.GetResultData(strSql, 0);
        //    Data_TemporaryTeam.TeamType teamType;

        //    for (int i = 0; i < arrResult.Count - 6; i += 7)
        //    {
        //        Data_EmergencyTeam dataNew = new Data_EmergencyTeam();

        //        dataNew.ID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
        //        dataNew.TeamName = WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");
        //        dataNew.ParentTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
        //        dataNew.TeamID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
        //        int nMemberType = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);

        //        if (!Data_TemporaryTeam.TryToTeamType(nMemberType, out teamType))
        //            continue;

        //        dataNew.SetTeamType(teamType);

        //        if (teamType == Data_TemporaryTeam.TeamType.JobLevel)
        //            dataNew.LevelNo = dataNew.TeamID;

        //        TemporaryEmergencyTeam.Add(dataNew);
        //    }

        //    /*for (int i = 0; i < arrResult.Count - 6; i += 7)
        //    {
        //        Data_EmergencyTeam dataNew = new Data_EmergencyTeam();
        //        dataNew.ID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
        //        dataNew.TeamName = WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");
        //        dataNew.ParentTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
        //        dataNew.GroupName = WebDBManager.GetStringField(arrResult[i + 3].ToString(), "");
        //        dataNew.LevelNo = WebDBManager.GetIntField(arrResult[i + 4].ToString(), 0);
        //        dataNew.Description = WebDBManager.GetStringField(arrResult[i + 5].ToString(), "");
        //        dataNew.RegularTeamLink = WebDBManager.GetStringField(arrResult[i + 6].ToString(), "");

        //        TemporaryEmergencyTeam.Add(dataNew);
        //    }*/
        //}

        // Comment by skkim 2015.01.08 - 사용안함
        //private void ReadCheckTask()
        //{
        //    string strSql = "SELECT * FROM CheckTask";
        //    ArrayList arrResult = m_dbMgr.GetResultData(strSql, 0);

        //    for (int i = 0; i < arrResult.Count - 6; i += 7)
        //    {
        //        Data_CheckTask dataNew = new Data_CheckTask();
        //        dataNew.ID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
        //        dataNew.ProcessID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), 0);
        //        dataNew.Category = WebDBManager.GetStringField(arrResult[i + 2].ToString(), "");
        //        dataNew.SubCategory = WebDBManager.GetStringField(arrResult[i + 3].ToString(), "");
        //        dataNew.TaskName = WebDBManager.GetStringField(arrResult[i + 4].ToString(), "");
        //        dataNew.TargetCount = WebDBManager.GetIntField(arrResult[i + 5].ToString(), 0);
        //        dataNew.Position = WebDBManager.GetStringField(arrResult[i + 6].ToString(), "");

        //        CheckTask.Add(dataNew);
        //    }
        //}

		private void ReadRegularTeam()
		{
            // Site별로 사용할 수 있도록 수정 , Edit by skkim 2015.01.08
            // SiteID로 본부 아이디를 가져온다.
            string szSQL = string.Format("SELECT TeamID FROM Site WHERE ID = {0}", m_nSiteID);
            ArrayList arrResult1 = m_dbMgr.GetResultData(szSQL, 0);
            if (arrResult1 == null || arrResult1.Count == 0)
                return;

            int nTeamID = WebDBManager.GetIntField(arrResult1[0].ToString(), -1);
            if( nTeamID == -1)
                return;

            ArrayList arrResult = IOManager.ExecuteTeamList(m_dbMgr, nTeamID);
            //string strSQL = string.Format("sp_TeamList2 {0}", nTeamID);
            //ArrayList arrResult = m_dbMgr.GetStoredProcedureData(strSQL, 0);

            if (arrResult == null)
                return;

			for (int i = 0; i < arrResult.Count - 2; i += 3)
			{
				Data_RegularTeam dataNew = new Data_RegularTeam();

				dataNew.ID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
				dataNew.TeamName = WebDBManager.GetStringField(arrResult[i + 1], "");
				dataNew.ParentTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);

				//arrRegularTeams.Add(dataNew);
				RegularTeam.Add(dataNew);
			}

			// arrSrc의 Team 데이터들을 부모노드가 먼저 나오도록 정렬하여 arrTrg에 담는다.
			//SortNAdd(RegularTeam, arrRegularTeams);
		}

        Dictionary<int, Data_ControlRoom> dicControlRoom;
        private void ReadControlRoom()
        { 
            ControlRoom.Clear();
            dicControlRoom = new Dictionary<int, Data_ControlRoom>();
             
            string strSQL = "select cr.ID, cr.RoomType, cr.LocationName, crt.TypeName from ControlRoom as cr, ControlRoomType as crt ";
            strSQL += "where cr.RoomType = crt.ID and crt.SiteID = " + m_nSiteID.ToString() + " order by cr.RoomType";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0) return;

            Data_ControlRoom teamRoot = GetRootControlRoomTeam(dicControlRoom);

            List<int> controlRoomIDs = new List<int>();
            List<int> roomTypeIDs = new List<int>();
            string strRoomTypeIDs = "";
            Dictionary<int, int> dicRoomTypeID = new Dictionary<int, int>();

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                int nControlRoomID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nRoomTypeID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                string strLocationName = WebDBManager.GetStringField(arrResult[i + 2]);
                string strRoomType = WebDBManager.GetStringField(arrResult[i + 3]);

                if (nControlRoomID < 0 || nRoomTypeID < 0 || strLocationName == null || strRoomType == null)
                    continue;

                int nID = Data_ControlRoom.MakeID(nRoomTypeID, nControlRoomID, 0);

                Data_ControlRoom team = new Data_ControlRoom();
                team.ID = nID;
                team.ParentTeam = teamRoot;

                if (strLocationName == strRoomType)
                    team.TeamName = strLocationName;
                else
                    team.TeamName = strLocationName + " " + strRoomType;

                dicControlRoom[nID] = team;
                ControlRoom.Add(team);

                if (dicRoomTypeID.ContainsKey(nRoomTypeID))
                    dicRoomTypeID[nRoomTypeID]++;
                else 
                    dicRoomTypeID.Add(nRoomTypeID, 1); 

                if (!roomTypeIDs.Contains(nRoomTypeID))
                {
                    roomTypeIDs.Add(nRoomTypeID);

                    if (strRoomTypeIDs.Length == 0)
                        strRoomTypeIDs = nRoomTypeID.ToString();
                    else
                        strRoomTypeIDs += ", " + nRoomTypeID.ToString();
                }

                if (!controlRoomIDs.Contains(nControlRoomID))
                    controlRoomIDs.Add(nControlRoomID);
            }

            if (dicRoomTypeID.Count == 0) return;

            strSQL = string.Format("Select ID, JobName, RoomType from ControlTeamJobPosition where RoomType in ({0})", strRoomTypeIDs);
            arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null) return;

            nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                int nPositionID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strJobName = WebDBManager.GetStringField(arrResult[i + 1]);
                int nRoomTypeID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);

                if (nPositionID < 0 || nRoomTypeID < 0 || strJobName == null)
                    continue; 

                foreach (int nControlRoomID in controlRoomIDs)
                {
                    int nID = Data_ControlRoom.MakeID(nRoomTypeID, nControlRoomID, nPositionID);

                    Data_ControlRoom team = new Data_ControlRoom();
                    team.TeamName = strJobName;
                    team.ID = nID;

                    int nParentTeamID = Data_ControlRoom.MakeID(nRoomTypeID, nControlRoomID, 0);
                    Data_ControlRoom parentTeam;

                    if (dicControlRoom.TryGetValue(nParentTeamID, out parentTeam))
                    {
                        team.ParentTeam = parentTeam;

                        dicControlRoom[nID] = team;

                        ControlRoom.Add(team);
                    }
                }
            }

            // 최상위 팀을 추가한다.
            teamRoot.ParentTeamID = -1;
            ControlRoom.Add(teamRoot);
        }
        
        public Data_ControlRoom GetRootControlRoomTeam(Dictionary<int, Data_ControlRoom> dicTeams = null)
        {
            if (dicTeams == null)
                dicTeams = dicControlRoom;

            Data_ControlRoom team;
            int nID = Data_ControlRoom.MakeID(0, 0, 0);

            if (!dicTeams.TryGetValue(nID, out team))
            {
                team = new Data_ControlRoom();
                team.ID = nID;

                dicTeams[nID] = team;
            }

            return team;
        }

        // Comment by skkim 2015.01.08 - 사용안함, Stored Procedure사용
		// arrSrc의 Team 데이터들을 부모노드가 먼저 나오도록 정렬하여 arrTrg에 담는다.
        //private void SortNAdd(ArrayList arrTrg, ArrayList arrSrc, int nParentTeamID = 0)
        //{
        //    ArrayList arrRemove = new ArrayList();

        //    foreach (Data_RegularTeam team in arrSrc)
        //    {
        //        if (team.ParentTeamID == nParentTeamID)
        //        {
        //            arrTrg.Add(team);
        //            arrRemove.Add(team);
        //        }
        //    }

        //    foreach (Data_RegularTeam team in arrRemove)
        //    {
        //        arrSrc.Remove(team);
        //    }

        //    foreach (Data_RegularTeam team in arrRemove)
        //    {
        //        SortNAdd(arrTrg, arrSrc, team.ID);
        //    }
        //}

		private void ReadActionStep()
		{           
			//string strSQL = "SELECT * FROM ActionStep";
            // Site별로 사용할 수 있도록 Query 수정 , Edit by skkim 2015.01.08
            string szText = "SELECT step.ID, step.StepName,step.PeriodType,step.BeginTime,step.EndTime,step.WeekDayOption,step.Iteration," +
                            "step.IterationType,step.ProcessTime,step.ProcessTimeType, step.DisasterID, step.ParentStepID " +
                            " FROM ActionStep as step, Disaster as dis, SubDisasterCategory as sdc, DisasterCategory as dc " +
                            " WHERE step.DisasterID = dis.ID and dis.SubDisasterID = sdc.ID and sdc.DisasterID = dc.ID and dc.SiteID = {0}";
            
            string strSQL = string.Format(szText, m_nSiteID);

			DateTime dtDefault = new DateTime();
			ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null)
                return;

			for (int i = 0; i < arrResult.Count - 11; i += 12)
			{
				Data_ActionStep dataNew = new Data_ActionStep();

				dataNew.ID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
				dataNew.StepName = WebDBManager.GetStringField(arrResult[i + 1], "");
				dataNew.PeriodType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
				dataNew.BeginTime = WebDBManager.GetDateTimeField(arrResult[i + 3].ToString(), dtDefault);
				dataNew.EndTime = WebDBManager.GetDateTimeField(arrResult[i + 4].ToString(), dtDefault);
				dataNew.WeekdayOption = WebDBManager.GetIntField(arrResult[i + 5].ToString(), 0);
				dataNew.Iteration = WebDBManager.GetIntField(arrResult[i + 6].ToString(), 0);
				dataNew.IterationType = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0);
				dataNew.ProcessTime = WebDBManager.GetIntField(arrResult[i + 8].ToString(), 0);
				dataNew.ProcessTimeType = WebDBManager.GetIntField(arrResult[i + 9].ToString(), 0);
				dataNew.DisasterID = WebDBManager.GetIntField(arrResult[i + 10].ToString(), 0);
				dataNew.ParentStepID = WebDBManager.GetIntField(arrResult[i + 11].ToString(), 0);

				ActionStep.Add(dataNew);
			}
		}

		private void ReadUserDefinedTeam()
		{
			//string strSQL = "SELECT * FROM UserDefinedTeam";
            // Site별로 사용할 수 있도록 Query 수정 , Edit by skkim 2015.01.08
            string strSQL = string.Format("SELECT ID,TeamName, PhoneNumber,FaxNumber FROM UserDefinedTeam WHERE SiteID = {0}", m_nSiteID);

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null)
                return;

			for (int i = 0; i < arrResult.Count - 3; i += 4)
			{
				Data_UserDefinedTeam dataNew = new Data_UserDefinedTeam();

				dataNew.ID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
				dataNew.TeamName = WebDBManager.GetStringField(arrResult[i + 1], "");
				dataNew.PhoneNumber = WebDBManager.GetStringField(arrResult[i + 2], "");
				dataNew.FaxNumber = WebDBManager.GetStringField(arrResult[i + 3], "");

                if (dataNew.PhoneNumber == null || dataNew.PhoneNumber == "null")
                    dataNew.PhoneNumber = "";

                if (dataNew.FaxNumber == null || dataNew.FaxNumber == "null")
                    dataNew.FaxNumber = "";

				UserDefinedTeam.Add(dataNew);
			}
		}

		public void ReadExternalTeam()
		{
			ExternalTeam.Clear();

			//string strSQL = "SELECT * FROM ExternalTeam";
            // Site별로 사용할 수 있도록 Query 수정 , Edit by skkim 2015.01.08
            string szText = "SELECT ID, TeamName, PhoneNumber, FaxNumber, ParentTeamID FROM ExternalTeam WHERE SiteID = {0}";
            string strSQL = string.Format(szText, m_nSiteID);
			ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null)
                return;

            Dictionary<int, Data_ExternalTeam> dicCompanies = new Dictionary<int, Data_ExternalTeam>();
           
			for (int i = 0; i < arrResult.Count - 4; i += 5)
			{
				Data_ExternalTeam dataNew = new Data_ExternalTeam();

				dataNew.ID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
				dataNew.TeamName = WebDBManager.GetStringField(arrResult[i + 1], "");
				dataNew.PhoneNumber = WebDBManager.GetStringField(arrResult[i + 2], "");
				dataNew.FaxNumber = WebDBManager.GetStringField(arrResult[i + 3], "");

                dataNew.ParentID = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);

				if (dataNew.PhoneNumber == null || dataNew.PhoneNumber == "null")
					dataNew.PhoneNumber = "";

                if (dataNew.FaxNumber == null || dataNew.FaxNumber == "null")
					dataNew.FaxNumber = "";

				ExternalTeam.Add(dataNew);              

                dicCompanies[dataNew.ID] = dataNew;
			}
            
            foreach(int nID in dicCompanies.Keys)
            {
                Data_ExternalTeam data = dicCompanies[nID];
                if(data.ParentID != -1)
                {
                    data.ParentTeam = dicCompanies[data.ParentID];
                }
                else
                {
                    data.ParentTeam = null;
                }
            }
		}

        //private void ReadExternalCompanyTeam(string strCompanyTeamIDs, Dictionary<int, Data_ExternalTeam> dicCompanies)
        //{
        //    ExternalCompanyTeam.Clear();

        //    if (strCompanyTeamIDs.Length == 0)
        //        return;

        //    string szText = "SELECT ID, TeamName, ParentTeamID, CompanyID FROM ExternalCompanyTeam WHERE CompanyID in ({0})";
        //    string strSQL = string.Format(szText, strCompanyTeamIDs);
        //    ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

        //    Dictionary<int, int> dicParents = new Dictionary<int, int>();
        //    Dictionary<int, Data_ExternalCompanyTeam> dicTeams = new Dictionary<int, Data_ExternalCompanyTeam>();

        //    for (int i = 0; i < arrResult.Count - 3; i += 4)
        //    {
        //        Data_ExternalCompanyTeam dataNew = new Data_ExternalCompanyTeam();

        //        dataNew.ID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
        //        dataNew.TeamName = WebDBManager.GetStringField(arrResult[i + 1], "");
        //        int nParentTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
        //        int nCompanyID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);

        //        Data_ExternalTeam company;

        //        if (!dicCompanies.TryGetValue(nCompanyID, out company))
        //            continue;

        //        dataNew.Company = company;

        //        if (nParentTeamID > 0)
        //            dicParents[dataNew.ID] = nParentTeamID;

        //        dicTeams[dataNew.ID] = dataNew;

        //        ExternalCompanyTeam.Add(dataNew);
        //    }

        //    foreach (KeyValuePair<int, Data_ExternalCompanyTeam> pair in dicTeams)
        //    {
        //        int nParentID;

        //        if (!dicParents.TryGetValue(pair.Key, out nParentID))
        //            continue;

        //        Data_ExternalCompanyTeam team;

        //        if (!dicTeams.TryGetValue(nParentID, out team))
        //            continue;

        //        pair.Value.ParentTeam = team;
        //    }
        //}

		private void ReadVersion()
		{
			//string strSQL = "SELECT * FROM Version";
            // Site별로 사용할 수 있도록 Query 수정 , Edit by skkim 2015.01.08
            string szText = "SELECT ID, isRegular, isNormal, CreateTime, LastAccessTime, VersionName, OwnerID, Description FROM Version WHERE SiteID = {0}";
            string strSQL = string.Format(szText, m_nSiteID);

			DateTime dtDefault = new DateTime();
			ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null)
                return;
			for (int i = 0; i < arrResult.Count - 7; i += 8)
			{
				Data_Version dataNew = new Data_Version();

				dataNew.ID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
				dataNew.Regular = WebDBManager.GetIntField(arrResult[i + 1].ToString(), 0);
				dataNew.Normal = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
				dataNew.CreateTime = WebDBManager.GetDateTimeField(arrResult[i + 3].ToString(), dtDefault);
				dataNew.LastAccessTime = WebDBManager.GetDateTimeField(arrResult[i + 4].ToString(), dtDefault);
				dataNew.VersionName = WebDBManager.GetStringField(arrResult[i + 5], "");
				dataNew.OwnerID = WebDBManager.GetIntField(arrResult[i + 6].ToString(), 0);
				dataNew.Description = WebDBManager.GetStringField(arrResult[i + 7], "");

				SOPVersion.Add(dataNew);
			}
		}
		//////////////////////////////////////////////////////////////////////////

		private void FindNormalPath()
		{
			m_arrFullPath.Clear();
			foreach (Data_NormalTeam data in TemporaryNormalTeam)
			{
				string strPath = data.TeamName;
				TemporaryTeamFullPath fullPath = new TemporaryTeamFullPath();
				if (data.ParentTeamID != 0)
				{
					ArrayList arrPath = FindParent(data.ParentTeamID);
					strPath = GetPath(data.ID, arrPath);
					strPath += data.TeamName;
				}

				fullPath.ID = data.ID;
				fullPath.FullPath = strPath;

				m_arrFullPath.Add(fullPath);
			}
		}

		private ArrayList FindParent(int nParentID)
		{
            ArrayList arrPath = new ArrayList();
			if (m_dicNormalTeam.ContainsKey(nParentID))
			{
                arrPath.Add(m_dicNormalTeam[nParentID].TeamName);

				if (m_dicNormalTeam[nParentID].ParentTeamID != 0)
				{
					FindParent(m_dicNormalTeam[nParentID].ParentTeamID);
				}
			}
            return arrPath;
		}

		private string GetPath(int nID, ArrayList arrPath)
		{
			string strPath = "";
			for (int i = arrPath.Count - 1; i >= 0; i--)
			{
				strPath += arrPath[i] + "/";
			}

			return strPath;
		}

		public string ParseCaption(string strValue)
		{
			string[] result = strValue.Split(new char[] { '>' });
			result = result[2].Split(new char[] { '<' });

			return result[0];
		}

		private bool OpenSOPXML()
		{
			OpenFileDialog dlg = new OpenFileDialog();

			dlg.Filter = "XML Files|*.xml|All FIles|*.*";
			dlg.FilterIndex = 0;
			dlg.Title = "XML 파일 열기";

			if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{
				string strFileName = dlg.FileName;

				XMLManager mgr = new XMLManager();

				SopDocManager.Instance.UseXML = true;
				SopDocManager.Instance.FilePath = strFileName;

				if (mgr.Load(this, strFileName))
				{
					mRibbonForm.SelectTab(1);
					SelectTab(1);
					UndoRedoManager.Instance.SaveSnapshot();

                    int nIndex = strFileName.LastIndexOf('\\');

                    if (nIndex >= 0)
                        FormFrame.Instance.Text = FormFrame.Instance.Title + " - " + strFileName.Substring(nIndex + 1);
				}
				else
				{
                    FormFrame.Instance.Text = FormFrame.Instance.Title;
					string strError = mgr.ErrorMessage.Length == 0 ? "XML 불러오기가 실패하였습니다." : mgr.ErrorMessage;
					UnE.Utility.UMessageBox.Show(this, strError,"불러오기 오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return false;
				}
				return true;
			}
			return false;
		}

		public void OpenSOP(FormOpenDB frm)
		{

			GetPageLevel().RemoveAll();

			ArrayList arrTabPages = GetPageLevel().GetTabPage();
			int nOldTabPageCount = arrTabPages == null ? 0 : arrTabPages.Count;

			// 기존 Section들의 ID 정보 초기화
			Sections.SectionData.ClearIDList();

			//FormOpenDB frm = (FormOpenDB)GetForm(typeof(FormOpenDB));
			VersionInfo version = frm.Version;
			ArrayList arrActionSteps = frm.ActionSteps;
			string strCategoryName = frm.CategoryName;
			string strSubCategoryName = frm.SubCategoryName;
			string strDisasterName = frm.DisasterName;
			// 등록모드인가?
			bool isRegular = frm.IsRegular;
			// 평일모드인가?
			bool isNormal = frm.IsNormal;


			// UI를 셋팅해주는 경우는 NewSOP인 경우로 한정한다. 2014.10.31 skkim
			// 필요 정보는 SopDocManager로 이동
			//m_formNewSOP.SetWeekMode(isNormal);
			//m_formNewSOP.SetRegularMode(isRegular);

			// DB에서 열기는 true, XML열기는 UseXML이 true
			SopDocManager.Instance.UseDB = true;
			// SOP 열기는 true, 새 SOP는 false
			SopDocManager.Instance.IsNewSOP = false;

			SopDocManager.Instance.FilePath = "";
			SopDocManager.Instance.WeekMode = isNormal;
			SopDocManager.Instance.RegularMode = isRegular;

			IOManager mgr = new IOManager();
			if (!mgr.Load(this, m_dbMgr, version, arrActionSteps, strCategoryName, strSubCategoryName, strDisasterName))
			{
                FormFrame.Instance.Text = FormFrame.Instance.Title;
				m_versionCurrent = null;
				UnE.Utility.UMessageBox.Show("SOP 불러오기가 실패하였습니다.", "불러오기 오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}

			string strRegular = "미등록모드";
			string strWeekday = "야간 및 휴일";

			if (isNormal)
				strRegular = "등록모드";
			if (isNormal)
				strWeekday = "평일";

			this.Text = "SOP Manager  V 2.0 - " + strRegular + ", " + strWeekday;
            FormFrame.Instance.Text = FormFrame.Instance.Title + " - " + strDisasterName;

			// UI를 셋팅해주는 경우는 NewSOP인 경우로 한정한다. 2014.10.31 skkim
			// 필요 정보는 SopDocManager로 이동
			//m_formNewSOP.SelectedCategory = strCategoryName;
			//m_formNewSOP.SelectedSubCategory = strSubCategoryName;
			//m_formNewSOP.SelectedDetailCategory = strDisasterName;

			SopDocManager.Instance.CategoryName = strCategoryName;
			SopDocManager.Instance.SubCategoryName = strSubCategoryName;
			SopDocManager.Instance.DisasterName = strDisasterName;

			m_versionCurrent = version;

			// 기존 탭이 남아 있게 되는데, 불러오기 후 해당 탭들을 삭제한다.
			for (int i = 0; i < nOldTabPageCount; i++)
			{
				ActionStepTabPage oldTabPage = (ActionStepTabPage)arrTabPages[0];
				GetPageLevel().RemoveTabPage(oldTabPage, false);
				arrTabPages.RemoveAt(0);
			}
			SelectTab(1);
            mRibbonForm.SelectTab(1);

            // 컴포넌트와 속성창 열기
            ShowRightPane(true);
		}

		public bool SaveXML(string strFileName, out string szError)
		{
			szError = "";
			XMLManager mgr = new XMLManager();

			int nIndex = strFileName.LastIndexOf('\\');
			int nDotIndex = strFileName.LastIndexOf('.');
			string strVersionName = "";

			if (nIndex >= 0 && nDotIndex >= 0)
				strVersionName = strFileName.Substring(nIndex + 1, nDotIndex - 1 - nIndex);
			else if (nIndex >= 0)
				strVersionName = strFileName.Substring(nIndex + 1);
			else if (nDotIndex >= 0)
				strVersionName = strFileName.Substring(0, nDotIndex - 1);
			else
				strVersionName = strFileName;

			if (!mgr.Save(FormMain.Instance, strFileName, strVersionName))
			{
				szError = mgr.ErrorMessage;
				return false;
			}
			return true;
		}

		public bool SaveXML(System.IO.Stream stream, string strVersion, out string szError)
		{
			szError = "";
			XMLManager mgr = new XMLManager();

			if (!mgr.Save(FormMain.Instance, stream, strVersion))
			{
				szError = mgr.ErrorMessage;
				return false;
			}
			return true;
		}

		public bool SaveAsSOPXML()
		{
			// DB또는 파일명이 지정되어 있지 않는 경우 새로 저장한다.
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.Filter = "XML Files|*.xml|All FIles|*.*";
			dlg.FilterIndex = 0;
			dlg.Title = "다른 이름으로 XML 저장";
			dlg.OverwritePrompt = true;

			if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{
				string strFileName = dlg.FileName;
				string szError = "";
				if (!SaveXML(strFileName, out szError))
				{
					UnE.Utility.UMessageBox.Show(szError, "저장 오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return false;
				}

				SopDocManager.Instance.UseXML = true;
				SopDocManager.Instance.IsNewSOP = false;
				SopDocManager.Instance.FilePath = dlg.FileName;

                int nIndex = strFileName.LastIndexOf('\\');
                FormFrame.Instance.Text = FormFrame.Instance.Title + " - " + strFileName.Substring(nIndex + 1);

				return true;
			}
			return false;
		}

		public bool SaveSOPXML()
		{
			// 이미 XML에 저장되어 있는경우 기존 파일에 저장한다.
			if( SopDocManager.Instance.UseXML == true)
			{
				string szPath = SopDocManager.Instance.FilePath;
				bool bPossiblePath = (szPath.IndexOfAny(System.IO.Path.GetInvalidPathChars()) == -1);
				if (szPath != null && szPath != "" && bPossiblePath == true)
				{
					string strFileName = SopDocManager.Instance.FilePath;
					string szError = "";
					if (!SaveXML(strFileName, out szError))
					{
						UnE.Utility.UMessageBox.Show(szError, "저장 오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						return false;
					}
					return true;
				}
			}

			// DB또는 파일명이 지정되어 있지 않는 경우 새로 저장한다.
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.Filter = "XML Files|*.xml|All FIles|*.*";
			dlg.FilterIndex = 0;
			dlg.Title = "XML 파일로 저장";

			if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{
				string strFileName = dlg.FileName;
				string szError = "";
				if (!SaveXML(strFileName, out szError))
				{
					UnE.Utility.UMessageBox.Show(szError, "저장 오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return false;
				}

				SopDocManager.Instance.UseXML = true;
				SopDocManager.Instance.IsNewSOP = false;
				SopDocManager.Instance.FilePath = dlg.FileName;

				return true;
			}
			return false;
		}


		internal bool CheckSubCategory(string szName, string szCategoryName)
		{
            string strSQL = string.Format("select id from DisasterCategory where CategoryName = '{0}' and SiteID = {1}", szCategoryName, FormMain.Instance.SiteID);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return false;

            int nCategoryID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);

            if (nCategoryID < 0)
                return false;

            strSQL = string.Format("Select id from SubDisasterCategory where SubCategoryName = '{0}' and DisasterID = {1}", szName, nCategoryID);
            arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return false;

            /*// szName에 해당하는 SubDisasterCategory가 존재하지 않으면 새로 만든다.
            if (arrResult.Count == 0)
            {
                int nSubDisasterCategoryID = GetMaxTableID("SubDisasterCategory", 0) + 1;

                if (nSubDisasterCategoryID <= 0)
                    return false;

                strSQL = string.Format("Insert into SubDisasterCategory (ID, DisasterID, SubCategoryName) values ({0}, {1}, '{2}')",
                    nSubDisasterCategoryID, nCategoryID, szName);

                if (m_dbMgr.GetResultData(strSQL, 0) == null)
                    return false;
            }*/

			return true;
		}

        private int GetMaxTableID(string strTableName, int nTransaction)
        {
            string strSQL = "Select max(ID) from " + strTableName;
            ArrayList arrResult = nTransaction != 0 ? m_dbMgr.GetBatchData(strSQL) : m_dbMgr.GetResultData(strSQL, 0);//m_dbMgr.GetResultData(strSQL, nTransaction);

            if (arrResult == null)
                return -1;

            if (arrResult.Count == 0)
                return 0;

            return WebDBManager.GetIntField(arrResult[0].ToString(), 0);
        }


		// 재난 유형 (SubDisaster) 삭제시 하위에 포함된 모든 Version을 삭제한다.
		internal bool RemoveSubCategory(Data_SubDisasterCategory data , ArrayList arrDisaster)
		{
			try
			{
				if (!m_dbMgr.BeginBatch())
				{
					return false;
				}

				IOManager ioMgr = new IOManager();
				foreach (Data_Disaster disaster in arrDisaster)
				{
					int nVersionID = disaster.VersionID;
					if (nVersionID != -1)
					{
						if (!ioMgr.DeleteSOPVersion(m_dbMgr, nVersionID, true, true, true))
						{
							m_dbMgr.BatchRollback();

							string szMsg = "아래의 SOP가 사용중 이어서 삭제가 취소됩니다.\n 모니터링 시스템에서 중지 후 삭제 해 주세요\nSOP : {0}";
							string szMsg1= string.Format(szMsg, disaster.DisasterName);

							UnE.Utility.UMessageBox.Show(this, szMsg1, "삭제 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
							return false;
						}
						else
						{

						}
						m_arrDetail.Remove(disaster);

					}
				}		

				string szSQL1 = string.Format("delete from SubDisasterCategory where id = {0}", data.ID);
				m_dbMgr.GetBatchData(szSQL1);

				m_dbMgr.BatchCommit();
				
			}
			catch(Exception )
			{
				m_dbMgr.BatchRollback();
				return false;

			}
			return true;
		}


		internal Data_SubDisasterCategory AddSubCategory(string szName, string szCategoryName)
		{
			m_dbMgr.BeginBatch();

			string szSQL1 = string.Format("select id from DisasterCategory where CategoryName = '{0}'", szCategoryName);
			ArrayList arResult = m_dbMgr.GetBatchData(szSQL1);
			if (arResult == null || arResult.Count == 0)
			{
				m_dbMgr.BatchRollback();
				return null;
			}						
			int nCategoryID = DBUtility.WebDBManager.GetIntField(arResult[0].ToString(), -1);


			string szSQL2 = "SELECT MAX(id) FROM SubDisasterCategory";
            ArrayList arResult2 = m_dbMgr.GetBatchData(szSQL2);
			if (arResult2 == null || arResult2.Count == 0)
			{
				m_dbMgr.BatchRollback();
				return null;
			}
			int nMaxID = DBUtility.WebDBManager.GetIntField(arResult2[0].ToString(), 0);
			nMaxID += 1;

            // 171204 KYJ
            // 사용자 정의일 경우 UserCategory = 1
            string query = "select UserCategory from SubDisasterCategory";
            ArrayList arrRes = m_dbMgr.GetResultData(query, 0);
            if (arrRes == null || arrRes.Count == 0)
            {
                //query = "ALTER TABLE sop_3.subdisastercategory ADD COLUMN UserCategory INT NULL COMMENT '사용자가 추가한 Category인지 여부' AFTER SubCategoryName;";
                //m_dbMgr.GetResultData(query, 0);
                //if (arrRes == null)
                    return null;
            }
			string szSQL3 = string.Format("INSERT INTO SubDisasterCategory (ID, DisasterID, SubCategoryName, UserCategory) VALUES ({0}, {1}, '{2}', 1)", nMaxID, nCategoryID, szName);
            //

            m_dbMgr.GetBatchData(szSQL3);
			m_dbMgr.BatchCommit();


			Data_SubDisasterCategory category = new Data_SubDisasterCategory();
			category.ID = nMaxID;
			category.DisasterID = nCategoryID;
			category.CategoryName = szName;

			return category;
		}

        private bool SaveAsSOP()
        {
            bool bResult = false;

            string strCategory = SopDocManager.Instance.CategoryName;
            string strSubCategory = SopDocManager.Instance.SubCategoryName;
            string strDisaster = SopDocManager.Instance.DisasterName;
           
            if (strCategory != null && strSubCategory != null && strDisaster != null && m_nSOPGenUserID > 0)
            {

                bool isRegular = SopDocManager.Instance.RegularMode;
                bool isNormal = SopDocManager.Instance.WeekMode;

                FormSaveAsVersion saveVersion = new FormSaveAsVersion(m_dbMgr, m_nSOPGenUserID, strCategory, strSubCategory, strDisaster, isRegular, isNormal, m_versionCurrent);
                
                UnE.GUI.DialogFormFrame frameVersion = new UnE.GUI.DialogFormFrame(saveVersion);
                frameVersion.Sizable = true;
                if (frameVersion.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string strNewCategory = saveVersion.SelectedCategory;
                    string strNewSubCategory = saveVersion.SelectedSubCategory;
                    string strNewDisaster = saveVersion.SelectedDetailCategory;

                    IOManager mgr = new IOManager();               
                    int nVersionID = saveVersion.VersionID;
                    SopDocManager.Instance.UseDB = true;
                    SopDocManager.Instance.IsNewSOP = false;

                    string strVersionName = saveVersion.VersionName;
                    // nVersionID가 0보다 크면 기존 버전을 덮어쓴다.
                    string strDescription = saveVersion.Description;

                    if (!CheckSubCategory(strNewSubCategory, strNewCategory))
                    {
                        Data_SubDisasterCategory newCategory = AddSubCategory(strNewSubCategory, strNewCategory);
                        if (newCategory == null)
                        {
                            m_versionCurrent = null;
                            string szMsg = "저장이 실패 하였습니다.\n재난 유형 추가가 실패 하였습니다.";
                            UnE.Utility.UMessageBox.Show(this, szMsg, "저장 실패", MessageBoxButtons.OK, MessageBoxIcon.Error);

                            return false;
                        }
                        else
                        {
                            // SubCategory를 더한다.
                            SubDisasterCategory.Add(newCategory);
                            // DisasterType에 추가해준다.
                            AddDisasterType(newCategory.ID, strNewDisaster);
                        }
                    }

                    VersionInfo version = new VersionInfo();
                    int nDisasterID;
                    if (mgr.Save(this, m_dbMgr, strVersionName, nVersionID, m_nSOPGenUserID, strDescription, ref version, out nDisasterID))
                    {
                        version.UserName = m_strSOPGenUserRealName;
                        m_versionCurrent = version;
                    }
                    else
                    {
                        m_versionCurrent = null;
                        string szMsg = "저장이 실패 하였습니다.\n모니터링에서 사용중인 버전입니다.";
                        UnE.Utility.UMessageBox.Show(this, szMsg, "저장 실패", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    if (form != null)
                    {
                        form.InitTree();
                        form.SelectNode(3, nDisasterID);
                    }

                    m_arrDisasterType.Clear();
                    ReadDisasterType();
                    // 저장에 성공한경우 ActionStep과 Disaster를 다시 읽는다.
                    m_arrDetail.Clear();
                    ReadDisaster();
                    m_arrActionStep.Clear();
                    ReadActionStep();

                    BarLevelTree tree = m_pageSOP.GetBarLevelTree();
                    if (tree != null)
                    {
                        tree.ChangeCategoryName(strCategory, strNewCategory);
                        tree.ChangeSubCategoryName(strSubCategory, strNewSubCategory);
                        tree.ChangeDisasterName(strDisaster, strNewDisaster);
                    }
                    bResult = true;
                }
            }

            SaveLastAccessedSOPTime(DateTime.Now);
            System.Diagnostics.Trace.WriteLine("ActionStepCount : " + m_arrActionStep.Count.ToString());
            return bResult;
        }

		private bool SaveSOP()
		{
			string strCategory = SopDocManager.Instance.CategoryName;
			string strSubCategory = SopDocManager.Instance.SubCategoryName;
			string strDisaster = SopDocManager.Instance.DisasterName;

			bool bResult = false;
			if (strCategory != null && strSubCategory != null && strDisaster != null && m_nSOPGenUserID > 0)
			{
				bool isRegular = SopDocManager.Instance.RegularMode;
				bool isNormal = SopDocManager.Instance.WeekMode;

				IOManager mgr = new IOManager();

				bool bNewVersionOnly = false;
				if(mgr.IsMonitoringDiaster(m_dbMgr, strDisaster))
				{
					if (m_versionCurrent!= null )
					{
						if( m_versionCurrent.VersionID > 0)
						{
							if(mgr.IsMonitoringSOPVersion(m_dbMgr, m_versionCurrent.VersionID, false))
							{
								bNewVersionOnly = true;
							}
						}
						else
						{
							bNewVersionOnly = true;
						}
					}
					else
					{
						bNewVersionOnly = true;
					}
				}
				
				FormSaveVersion saveVersion = new FormSaveVersion(m_dbMgr, m_nSOPGenUserID, strCategory, strSubCategory, strDisaster, isRegular, isNormal, m_versionCurrent);

				if(bNewVersionOnly == true)
				{
					saveVersion.SetNewVersionOnly();
				}
				
				UnE.GUI.DialogFormFrame frameVersion = new UnE.GUI.DialogFormFrame(saveVersion);
				if (frameVersion.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					int nVersionID = saveVersion.VersionID;					
					SopDocManager.Instance.UseDB = true;
					SopDocManager.Instance.IsNewSOP = false;

					string strVersionName = saveVersion.VersionName;
					// nVersionID가 0보다 크면 기존 버전을 덮어쓴다.
					
					string strDescription = saveVersion.Description;


					if (!CheckSubCategory(strSubCategory, strCategory))
					{
						Data_SubDisasterCategory newCategory = AddSubCategory(strSubCategory, strCategory);
						if (newCategory == null)
						{
							m_versionCurrent = null;
							string szMsg = "저장이 실패 하였습니다.\n재난 유형 추가가 실패 하였습니다.";
							UnE.Utility.UMessageBox.Show(this, szMsg, "저장 실패", MessageBoxButtons.OK, MessageBoxIcon.Error);
							
							return false;
						}
						else
						{
							// SubCategory를 더한다.
							SubDisasterCategory.Add(newCategory);
							// DisasterType에 추가해준다.
							AddDisasterType(newCategory.ID, strDisaster);
						}
					}

					
					
					VersionInfo version = new VersionInfo();
					
					int nDisasterID;

					if (mgr.Save(this, m_dbMgr, strVersionName, nVersionID, m_nSOPGenUserID, strDescription, ref version, out nDisasterID))
					{
						version.UserName = m_strSOPGenUserRealName;
						m_versionCurrent = version;
					}
					else
					{
						m_versionCurrent = null;
						string szMsg = "저장이 실패 하였습니다.\n모니터링에서 사용중인 버전입니다.";
						UnE.Utility.UMessageBox.Show(this, szMsg, "저장 실패", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return false;
					}

					if (form != null)
					{
						form.InitTree();
						form.SelectNode(3, nDisasterID);
					}

					m_arrDisasterType.Clear();
					ReadDisasterType();

					// 저장에 성공한경우 ActionStep과 Disaster를 다시 읽는다.
					m_arrDetail.Clear();
					ReadDisaster();

					m_arrActionStep.Clear();
					ReadActionStep();

					bResult = true;
				}
			}

            SaveLastAccessedSOPTime(DateTime.Now);
			System.Diagnostics.Trace.WriteLine("ActionStepCount : " + m_arrActionStep.Count.ToString());

			return bResult;
		}

        public void SaveLastAccessedSOPTime(DateTime time)
        {
            string strSQL = "Select ID from OptionSOPSimulator where PropertyName = 'LastAccessedSOPTime' and SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            string strTime = string.Format("'{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}'", time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);

            if (arrResult.Count == 1)
            {
                VariousData<int> nID = WebDBManager.GetIntField(arrResult[0].ToString());

                if (nID == null)
                    return;

                strSQL = "Update OptionSOPSimulator set PropertyValue = " + strTime + " where ID = " + nID.Data.ToString();
                m_dbMgr.GetResultData(strSQL, 0);
            }
            else
            {
                int nID = GetMaxTableID("OptionSOPSimulator", 0) + 1;

                string strFormat = "Insert into OptionSOPSimulator (ID, PropertyName, PropertyValue, SiteID, Description) values ";
                strFormat += "({0}, 'LastAccessedSOPTime', {1}, {2}, '마지막으로 SOP가 수정된 시간')";

                strSQL = string.Format(strFormat, nID, strTime, m_nSiteID);
                m_dbMgr.GetResultData(strSQL, 0);
            }
        }


		private bool CheckDisasterType(int nSubCategoryID, string szName)
		{
			try
			{
				string strSQL = string.Format("select ID FROM DisasterType where name = '{0}' and SubCategoryID = {1}", szName, nSubCategoryID);

				ArrayList arResult = m_dbMgr.GetResultData(strSQL, 0);
				if (arResult == null)
					return true;
			}
			catch (Exception)
			{

			}
			return false;
		}

		private Data_Disaster AddDisasterType(int nSubCategoryID, string szName)
		{
			if (!CheckDisasterType(nSubCategoryID, szName))
				return null;

			try
			{
                string strSQL = string.Format("select max(id) FROM DisasterType");
				ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

				int nDisasterID;

				if (arrResult == null || arrResult.Count == 0)
					nDisasterID = 0;
				else
					nDisasterID = WebDBManager.GetIntField(arrResult[0].ToString(), 0);

				strSQL = string.Format("INSERT INTO DisasterType (ID, Name, SubDisasterID) VALUES ({0}, '{1}', {2})",
					++nDisasterID, szName, nSubCategoryID);

				m_dbMgr.GetResultData(strSQL, 0);

				Data_Disaster data = new Data_Disaster();
				data.SubDisasterID = nSubCategoryID;
				data.DisasterName = szName;
				//data.ID = nDisasterID;
				return data;
			}
			catch(Exception)
			{

			}
			return null;			
		}

		private void AddVersion(VersionInfo version)
		{
			foreach (Data_Version ver in m_arrSOPVersion)
			{
				if (ver.ID == version.VersionID)
				{
					ver.CreateTime = version.BeginTime;
					ver.LastAccessTime = version.EndTime;
					ver.Description = version.Description;
					ver.VersionName = version.VersionName;
					return;
				}
			}

			Data_Version newVersion = new Data_Version();

			newVersion.ID = version.VersionID;
			newVersion.CreateTime = version.BeginTime;
			newVersion.LastAccessTime = version.EndTime;
			newVersion.Description = version.Description;
			newVersion.VersionName = version.VersionName;

			m_arrSOPVersion.Add(newVersion);
		}

		private bool CheckProcess(Sections.SectionProcess section)
		{
			Sections.SectionDataProcess data = (Sections.SectionDataProcess)section.Data;

			if (data.TeamList.Count < 0)
			{
				UnE.Utility.UMessageBox.Show("임무를 수행할 대상이 지정되지 않은 [프로세스] 태그가 존재합니다.\r\n확인후 저장하십시오.","오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				ZoomNSelectSection(section);
				return false;
			}

			return true;
		}

		private void PrepareCheckEndPoint(Sections.SectionEndPoint section, ref int nStart, ref int nEnd)
		{
			Sections.SectionDataEndPoint data = (Sections.SectionDataEndPoint)section.Data;

			if (data.IsBegin)
			{
				nStart++;
			}
			else
				nEnd++;
		}

		private bool CheckTransSOP(Sections.SectionTransSOP section, ref bool useTransSOP)
		{
			Sections.SectionDataTransSOP data = (Sections.SectionDataTransSOP)section.Data;

			if (data.LinkedActionStepID < 0)
			{
				UnE.Utility.UMessageBox.Show("전환할 SOP 대상이 지정되지 않은 [SOP 전환] 태그가 존재합니다.\r\n확인후 저장하십시오.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				ZoomNSelectSection(section);
				return false;
			}
			else
				useTransSOP = true;

			return true;
		}

		private bool CheckLink(Sections.SectionLink section)
		{
			Sections.SectionDataLink data = (Sections.SectionDataLink)section.Data;

			if (data.LinkedSection == null)
			{
				UnE.Utility.UMessageBox.Show("링크될 대상이 지정되지 않은 [Link] 태그가 존재합니다.\r\n확인후 저장하십시오.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				ZoomNSelectSection(section);
				return false;
			}
			else
			{
				if (!IsValidSection(data.LinkedSection))
				{
					data.LinkedSection = null;
					UnE.Utility.UMessageBox.Show("링크될 대상이 이미 삭제된 [Link] 태그가 존재합니다.\r\n링크될 대상을 다시 지정후 저장하십시오.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					ZoomNSelectSection(section);
					return false;
				}
			}

			return true;
		}

		private bool CheckExternal(Sections.SectionExternal section)
		{
			Sections.SectionDataExternal data = (Sections.SectionDataExternal)section.Data;

			if (data.UseSMS)
			{
				if (data.SMSMessage.Length == 0)
				{
					UnE.Utility.UMessageBox.Show("SMS 메시지 내용이 비어 있는 [외부 상황전파] 태그가 존재합니다.\r\n확인후 저장하십시오.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					ZoomNSelectSection(section);
					return false;
				}
				else if (data.SMSReceivers.Count == 0)
				{
					UnE.Utility.UMessageBox.Show("SMS 수신처가 비어 있는 [외부 상황전파] 태그가 존재합니다.\r\n확인후 저장하십시오.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					ZoomNSelectSection(section);
					return false;
				}
			}

			if (data.UseFax)
			{
				if (data.FaxReceivers.Count == 0)
				{
					UnE.Utility.UMessageBox.Show("Fax 수신처가 비어 있는 [외부 상황전파] 태그가 존재합니다.\r\n확인후 저장하십시오.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					ZoomNSelectSection(section);
					return false;
				}
			}

			return true;
		}

		private bool CheckTransmission(Sections.SectionTransmission section)
		{
			Sections.SectionDataTransmission data = (Sections.SectionDataTransmission)section.Data;
			Sections.SectionDataTransmission.ExternalData external = data.DataExternal;

			if (external.UseSMS)
			{
				if (external.SMSMessage.Length == 0)
				{
					UnE.Utility.UMessageBox.Show("SMS 메시지 내용이 비어 있는 [상황전파] 태그가 존재합니다.\r\n확인후 저장하십시오.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					ZoomNSelectSection(section);
					return false;
				}
				else if (external.SMSReceivers.Count == 0)
				{
					UnE.Utility.UMessageBox.Show("SMS 수신처가 비어 있는 [상황전파] 태그가 존재합니다.\r\n확인후 저장하십시오.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					ZoomNSelectSection(section);
					return false;
				}
			}

			if (external.UseFax)
			{
				if (external.FaxReceivers.Count == 0)
				{
					UnE.Utility.UMessageBox.Show("Fax 수신처가 비어 있는 [상황전파] 태그가 존재합니다.\r\n확인후 저장하십시오.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					ZoomNSelectSection(section);
					return false;
				}
			}

			return true;
		}

		private bool CheckSOP()
		{
			TabControl ctrl = m_pageSOP.TabControls;
			if (ctrl.TabPages.Count == 0)
				return false;

			foreach (TabPage page in ctrl.TabPages)
			{
				int nStart = 0, nEnd = 0;
				bool useTransSOP = false;

				string szStepName = page.Text;
				string szHeader = string.Format("[{0}] - ", szStepName);
				foreach (Sections.PanelSectionEx panel in page.Controls)
				{
					string szTeam = panel.TeamName;
					string szHeader2 = string.Format("[{0}][{1}] - ", szStepName, szTeam);
					foreach (Sections.Section section in panel.Sections)
					{
						Sections.Section.ComponentType type = section.GetComponentType();

						if (type == Sections.Section.ComponentType.PROCESS)
						{
							if (!CheckProcess((Sections.SectionProcess)section))
								return false;
						}
						else if (type == Sections.Section.ComponentType.ENDPOINT) // 시작/끝
						{
							PrepareCheckEndPoint((Sections.SectionEndPoint)section, ref nStart, ref nEnd);
						}
						else if (type == Sections.Section.ComponentType.TRANSSOP)
						{
							if (!CheckTransSOP((Sections.SectionTransSOP)section, ref useTransSOP))
								return false;
						}
						else if (type == Sections.Section.ComponentType.LINK)
						{
							if (!CheckLink((Sections.SectionLink)section))
								return false;
						}
						else if (type == Sections.Section.ComponentType.EXTERNAL)
						{
							if (!CheckExternal((Sections.SectionExternal)section))
								return false;
						}
						else if (type == Sections.Section.ComponentType.TRANSMISSION)
						{
							if (!CheckTransmission((Sections.SectionTransmission)section))
								return false;
						}
					}
				}
				if (nStart == 0)
				{
					TabControl control = (TabControl)page.Parent;
					if (control != null)
					{
						control.SelectedTab = page;
					}

					UnE.Utility.UMessageBox.Show(szHeader + "[시작] 태그가 없습니다.\r\n확인후 저장하십시오.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return false;
				}
				else if (nStart > 1)
				{
					TabControl control = (TabControl)page.Parent;
					if (control != null)
					{
						control.SelectedTab = page;
					}

					UnE.Utility.UMessageBox.Show(szHeader + string.Format("[시작] 태그가 {0}개 존재합니다.\r\n[시작] 태그는 반드시 하나만 존재하여야 합니다.\r\n확인후 저장하십시오.", nStart), "오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return false;
				}
				if (nEnd == 0)
				{
					// TransSOP가 있으면 [종료] 태그를 대신할 수 있다.
					if (!useTransSOP)
					{
						TabControl control = (TabControl)page.Parent;
						if (control != null)
						{
							control.SelectedTab = page;
						}

						UnE.Utility.UMessageBox.Show(szHeader + "[종료] 태그가 없습니다.\r\n확인후 저장하십시오.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						return false;
					}
				}
			}

			return true;
		}

		private void ZoomNSelectSection(Sections.Section section)
		{
			Sections.PanelSectionEx panel = (Sections.PanelSectionEx)section.GetParent();
			if (panel != null)
			{
				TabPage page = (TabPage)panel.Parent;
				if (page != null)
				{
					TabControl control = (TabControl)page.Parent;
					if (control != null)
					{
						control.SelectedTab = page;
					}
				}

				panel.ClearSelection();
				panel.SelectSection(section);
				panel.ZoomSection(section);
			}
		}

		private bool IsValidSection(Sections.Section section)
		{
			Sections.PanelSectionEx panel = (Sections.PanelSectionEx)section.GetParent();

			foreach (Sections.Section _section in panel.Sections)
			{
				if (section == _section)
					return true;
			}

			return false;
		}

		public void SaveExternalTeam(string strTeamName)
		{
			IOManager mgr = new IOManager();
			mgr.AddExternalTeam(m_dbMgr, strTeamName, true);
		}


		private UnE.GUI.DialogFormFrame m_frmForMessage = null;

		public void ShowSpecialMessage()
		{
            try
            {
                this.Invoke(new MethodInvoker(delegate()
                {
                    if (m_frmForMessage == null || m_frmForMessage.IsDisposed == true)
                    {
                        m_frmSpecialMessage = new PopupSpecialMessage();
                        m_frmSpecialMessage.TopMost = true;
                        m_frmForMessage = new UnE.GUI.DialogFormFrame(m_frmSpecialMessage);

                        m_frmForMessage.StartPosition = FormStartPosition.WindowsDefaultLocation;
                    }
                    
                    if (m_frmForMessage.Visible == false)
                        m_frmForMessage.Show(this);
                }));
            }
            catch (System.ObjectDisposedException e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }
		}

		public void HideSpecialMessage()
		{
			this.Invoke(new MethodInvoker(delegate()
			{
				if (m_frmForMessage != null)
				{
					m_frmForMessage.Close();
					m_frmForMessage = null;
				}
			}));
		}

		public VersionInfo CurrentVersion
		{
			get { return m_versionCurrent; }
		}

		public ToolStripStatusLabel GetStatusLabel()
		{
			return mStatusWork;
		}

		public void TextPictureBox_MouseUp(TextPictureBox pictureBox, MouseEventArgs e)
		{

		}

		private void FormMain_SizeChanged(object sender, EventArgs e)
		{

		}

		private void mCheckValidationToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void SimulationToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		public void CheckedChanged(int nCommand, bool bChecked)
		{
			int i = 0;
			i++;
		}

		public void RunCommand(int nCommand)
		{
			int i = 0;
			i++;
		}

		public void SetStatusText(string szMsg)
		{
			this.mStatusWork.Text = szMsg;
		}
	}

	public class TemporaryTeamFullPath
	{
		private int m_nID;
		private string m_strFullPath;

		public int ID
		{
			get { return m_nID; }
			set { m_nID = value; }
		}

		public string FullPath
		{
			get { return m_strFullPath; }
			set { m_strFullPath = value; }
		}
	}
}
