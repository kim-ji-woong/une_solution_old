using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.GUI;

namespace SOPManager
{
	public partial class FormRibbon : UnE.GUI.Contorl.FormRibbonTab
	{
		private IRibbonButtonOwner m_Ownwer = null;
		public IRibbonButtonOwner RibbonButtonOwner
		{
			get { return m_Ownwer; }
			set 
			{
				m_Ownwer = value;

				if (m_Ownwer != null)
					SetRibbonButtonOwner();
			}
		}

		private ArrayList m_arRibbonButtons = new ArrayList();
		public ArrayList RibbonButtons
		{
			get { return m_arRibbonButtons; }
			set { m_arRibbonButtons = value; }
		}

		public FormRibbon()
		{
			InitializeComponent();
			TopLevel = false;
			Dock = DockStyle.Fill;
						
			InitLeftToolBar();
			InitTopToolBar();

            if (FormMain.Instance.SiteID == 201)
            {
                btnHelp.Visible = false;
                btnHelp2.Visible = false;
            }
        }

        // 171201 KYJ
        //public void SelectTab(int nIdx)
        //{
        //    tabControlEx1.SelectedIndex = nIdx;
        //}

		private void SetRibbonButtonOwner()
		{
			for(int i = 0 ; i < tabControlEx1.TabCount; i++)
			{
				TabPage page = tabControlEx1.TabPages[i];
				foreach(Control ctrl in page.Controls)
				{
					// 패널 위에 있는경우
					if( typeof(Panel).IsAssignableFrom(ctrl.GetType()))
					{
						Panel panel = (Panel)ctrl;
						foreach(Control rbtn in panel.Controls)
						{
							if(typeof(RibbonButton).IsAssignableFrom(rbtn.GetType()))
							{
								RibbonButton rb = (RibbonButton)rbtn;
								rb.Owner = m_Ownwer;
							}
						}
					}
					else
					{
						// 페이지에 직접 추가된 경우
						foreach (Control rbtn in page.Controls)
						{
							if (typeof(RibbonButton).IsAssignableFrom(rbtn.GetType()))
							{
								RibbonButton rb = (RibbonButton)rbtn;
								rb.Owner = m_Ownwer;
							}
						}
					}

				}
			}
            //Resize Event 발생 후 Window 해상도 변경 시 호출 되는 이벤트
            FormMain.Instance.event_WinRateChanged += Instance_event_WinRateChanged;
		}

		public RibbonButton FindRibbonButton(int nCommandID)
		{
			foreach(RibbonButton rbtn in m_arRibbonButtons)
			{
				if (rbtn.ID == nCommandID)
					return rbtn;
			}
			return null;
		}

		public RibbonButton FindRibbonButton(string szCaption)
		{
			foreach (RibbonButton rbtn in m_arRibbonButtons)
			{
				if (rbtn.Text == szCaption)
					return rbtn;
			}
			return null;
		}
		protected void InitLeftToolBar()
		{
			//btnOpen.ForeColor = Color.White;
			//btnOpen.Text = "열기";
			btnOpen.ID = ID.ID_FILE_OPEN;

			//btnOpenXML.ForeColor = Color.White;
			//btnOpenXML.Text = "XML 열기";
			btnOpenXML.ID = ID.ID_XML_OPEN;
			
			//btnSave.ForeColor = Color.White;
			//btnSave.Text = "저장";
			btnSave.ID = ID.ID_FILE_SAVE;

			//btnSaveAs.ForeColor = Color.White;
			btnSaveAs.ID = ID.ID_FILE_SAVE_AS;
            //btnSaveAs.Text = "다른이름저장";

			//btnSaveXML.ForeColor = Color.White;
			//btnSaveXML.Text = "XML 저장";
			btnSaveXML.ID = ID.ID_XML_SAVE;

			//btnSaveAsXML.ForeColor = Color.White;
			btnSaveAsXML.ID = ID.ID_XML_SAVEAS;


			//btnNewSOP.ForeColor = Color.White;
			//btnNewSOP.Text = "새 SOP";
			btnNewSOP.ID = ID.ID_FILE_NEWSOP;

			//btnDelete.ForeColor = Color.White;
			//btnDelete.Text = "삭제";
			btnDelete.ID = ID.ID_FILE_DELETE;


			//btnStart.ForeColor = Color.White;
			//btnStart.Text = "시작하기";
			btnStart.ID = ID.ID_FILE_START;
            btnHelp.ID = ID.ID_HELP;


			m_arRibbonButtons.Add(btnNewSOP);
			m_arRibbonButtons.Add(btnSaveXML);
			m_arRibbonButtons.Add(btnSave);
            m_arRibbonButtons.Add(btnSaveAs);
			m_arRibbonButtons.Add(btnOpenXML);
			m_arRibbonButtons.Add(btnOpen);
			m_arRibbonButtons.Add(btnDelete);

			m_arRibbonButtons.Add(btnStart);
            m_arRibbonButtons.Add(btnHelp);
		}

		protected void InitTopToolBar()
		{			
			rBtnUndo.ID = ID.ID_EDIT_UNDO;	
			rBtnRedo.ID = ID.ID_EDIT_REDO;

			btnCopy.ID = ID.ID_EDIT_COPY;
			btnPaste.ID = ID.ID_EDIT_PASTE;
			btnCut.ID = ID.ID_EDIT_CUT;
			btnDelComp.ID = ID.ID_EDIT_DELETE;

			btnPasteStep.ID = ID.ID_EDIT_LEVEL_PASTE;
			btnCopyStep.ID = ID.ID_EDIT_LEVEL_COPY;
			btnDelStep.ID = ID.ID_EDIT_LEVEL_DEL;
			btnAddStep.ID = ID.ID_EDIT_LEVEL_ADD;

            //btnAddPane.ID = ID.ID_PANE_ADD;
            //btnCopyPane.ID = ID.ID_PANE_COPY;
            //btnPastePane.ID = ID.ID_PANE_PASTE;
            //btnDelPane.ID = ID.ID_PANE_DELETE;

			btnShowRightPane.ID = ID.ID_VIEW_RIGHTPANE;

			m_arRibbonButtons.Add(rBtnUndo);
			m_arRibbonButtons.Add(rBtnRedo);

			m_arRibbonButtons.Add(btnCopy);
			m_arRibbonButtons.Add(btnPaste);
			m_arRibbonButtons.Add(btnCut);
			m_arRibbonButtons.Add(btnDelComp);

			m_arRibbonButtons.Add(btnPasteStep);
			m_arRibbonButtons.Add(btnCopyStep);
			m_arRibbonButtons.Add(btnDelStep);
			m_arRibbonButtons.Add(btnAddStep);

            //m_arRibbonButtons.Add(btnAddPane);
            //m_arRibbonButtons.Add(btnAddPane);
            //m_arRibbonButtons.Add(btnCopyPane);
            //m_arRibbonButtons.Add(btnPastePane);
            //m_arRibbonButtons.Add(btnDelPane);

			m_arRibbonButtons.Add(btnShowRightPane);
		}

		private void ArrangeRibbonButtonAddGap(RibbonButton btnPrev, RibbonButton btnNext, int gap)
		{
			btnNext.Location = new Point(btnPrev.Location.X + btnPrev.Size.Width + gap, btnPrev.Location.Y);
		}

		private void ArrangeRibbonButton(RibbonButton btnPrev, RibbonButton btnNext)
		{
			btnNext.Location = new Point(btnPrev.Location.X + btnPrev.Size.Width, btnPrev.Location.Y);
		}

		private void FormRibbon_Load(object sender, EventArgs e)
		{
            FormMain.Instance.OnRibbonButtonMouseUp(btnShowRightPane, null);
            if (btnShowRightPane.IsChecked == false)
                btnShowRightPane.MouseOverImage = global::SOPManager.Properties.Resources.__TOPMENU_PropertyClick;
            else
                btnShowRightPane.MouseOverImage = global::SOPManager.Properties.Resources.__TOPMENU_Property;
        }

        void Instance_event_WinRateChanged()
        {
            double[] mCurWindowRate = FormMain.Instance.GetCurWindowRate();
            //double mCurWindowWidthRate = mCurWindowRate[0]; // 2.0 or 1.0
            //double mCurWindowHeightRate = mCurWindowRate[1]; // 2.0 or 1.0
            double mCurWindowWidthRate = FormMain.Instance.WindowWidthRate; 
            double mCurWindowHeightRate = FormMain.Instance.WindowHeightRate;

            Int32 iAddWidth = 0;
            Int32 iAddHeight = 0;

            tabControlEx1.Font = new System.Drawing.Font(Program.prgFont, tabControlEx1.Font.Size * (float)mCurWindowWidthRate, FontStyle.Bold);
            Int32 iTabItemHeight = tabControlEx1.ItemSize.Height;
            tabControlEx1.ItemSize = new System.Drawing.Size((int)((double)tabControlEx1.ItemSize.Width * mCurWindowWidthRate)
                                                                                     , (int)((double)tabControlEx1.ItemSize.Height * mCurWindowHeightRate));

            iTabItemHeight = tabControlEx1.ItemSize.Height - iTabItemHeight;

            List<Control> mctlList = new List<Control>(20);

            for (Int32 index = 0; index < doubleBufferedPanel1.Controls.Count; index++)
            {
                Control ctl = doubleBufferedPanel1.Controls[index];

                mctlList.Add(ctl);

                if (ctl.GetType().Name != "RibbonButton") continue;

                iAddWidth = ((int)((float)ctl.Size.Width * mCurWindowWidthRate)) - ctl.Size.Width;
                iAddHeight = ((int)((float)ctl.Size.Height * mCurWindowHeightRate)) - ctl.Size.Height;

                ctl.Size = new Size(ctl.Size.Width + iAddWidth, ctl.Size.Height + iAddHeight);
                ((RibbonButton)ctl).CustomImageRect = new Rectangle(0, 0, ctl.Size.Width, ctl.Size.Height);
            }          

            mctlList.Sort(delegate(Control A, Control B)
                        {
                            if (A.Location.X < B.Location.X) return -1;
                            else if (A.Location.X > B.Location.X) return 1;
                            return 0;
                        });

            Int32 iCtlDistance = 0;
            for (Int32 index = 0; index < mctlList.Count; index++)
            {
                mctlList[index].Location = new Point(iCtlDistance, 7);
                iCtlDistance += mctlList[index].Size.Width + mctlList[index].Margin.Right;
            }

            //
            iAddWidth = 0;
            iAddHeight = 0;
            mctlList.Clear();

            for (Int32 index = 0; index < tabPage2.Controls.Count; index++)
            {
                Control ctl = tabPage2.Controls[index];

                mctlList.Add(ctl);

                if (ctl.GetType().Name != "RibbonButton") continue;

                iAddWidth = ((int)((float)ctl.Size.Width * mCurWindowWidthRate)) - ctl.Size.Width;
                iAddHeight = ((int)((float)ctl.Size.Height * mCurWindowHeightRate)) - ctl.Size.Height;

                ctl.Size = new Size(ctl.Size.Width + iAddWidth, ctl.Size.Height + iAddHeight);
                ((RibbonButton)ctl).CustomImageRect = new Rectangle(0, 0, ctl.Size.Width, ctl.Size.Height);
            }

            mctlList.Sort(delegate(Control A, Control B)
            {
                if (A.Location.X < B.Location.X) return -1;
                else if (A.Location.X > B.Location.X) return 1;
                return 0;
            });

            iCtlDistance = 0;
            for (Int32 index = 0; index < mctlList.Count; index++)
            {
                mctlList[index].Location = new Point(iCtlDistance, 7);
                iCtlDistance += mctlList[index].Size.Width + 4;
            }

            this.Parent.Size = new Size(this.Parent.Size.Width, this.Parent.Size.Height + iAddHeight + iTabItemHeight);
        }

        private void btnShowRightPane_Click(object sender, EventArgs e)
        {
            if (btnShowRightPane.IsChecked == false)
                btnShowRightPane.MouseOverImage = global::SOPManager.Properties.Resources.__TOPMENU_PropertyClick;
            else
                btnShowRightPane.MouseOverImage = global::SOPManager.Properties.Resources.__TOPMENU_Property;
        }
	}
}
