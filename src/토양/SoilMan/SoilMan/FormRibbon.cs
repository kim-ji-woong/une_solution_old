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

namespace SoilMan
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
		}

		public void SelectTab(int nIdx)
		{
			tabControlEx1.SelectedIndex = nIdx;
		}

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

        public UnE.GUI.RibbonButton RedoBtn
        {
            get { return btnRedo; }
            set { btnRedo = value; }
        }
        
        public UnE.GUI.RibbonButton UndoBtn
        {
            get { return btnUndo; }
            set { btnUndo = value; }
        }

        public UnE.GUI.RibbonButton DetailAttribBtn
        {
            get { return btnDetailAttrib; }
            set { btnDetailAttrib = value; }
        }

        public UnE.GUI.RibbonButton SystemConstBtn
        {
            get { return btnSystemConst; }
            set { btnSystemConst = value; }
        }

        public UnE.GUI.RibbonButton CheckValueBtn
        {
            get { return btnCheckValue; }
            set { btnCheckValue = value; }
        }


        public UnE.GUI.RibbonButton SelectBtn
        {
            get { return btnSelect; }
            set { btnSelect = value; }
        }
        public UnE.GUI.RibbonButton DelSelectBtn
        {
            get { return btnDelSelect; }
            set { btnDelSelect = value; }
        }
        public UnE.GUI.RibbonButton DelUnSelectBtn
        {
            get { return btnDelUnSelect; }
            set { btnDelUnSelect = value; }
        }


        public UnE.GUI.RibbonButton DetailLayerBtn
        {
            get { return btnDetailLayer; }
            set { btnDetailLayer = value; }
        }

        public UnE.GUI.RibbonButton LayerBtn
        {
            get { return btnLayer; }
            set { btnLayer = value; }
        }

		protected void InitLeftToolBar()
		{
            btnOpenProject.ID = ID.ID_PROJECT_OPEN;
            btnSaveProject.ID = ID.ID_PROJECT_SAVE;
            btnSaveAsProject.ID = ID.ID_PROJECT_SAVEAS;

            btnOpenDXF.ID = ID.ID_FILE_OPEN_DXF;
            btnOpenDXF2.ID = ID.ID_FILE_OPEN_DXF2;
            btnOpenShape.ID = ID.ID_FILE_OPEN_SHAPE;

            btnOption.ID = ID.ID_FILE_OPTION;

            btnLayer.ID = ID.ID_LAYER;
            btnDetailLayer.ID = ID.ID_DETAIL_LAYER;
            btnDetailAttrib.ID = ID.ID_DETAIL_ATTRIB;

            btnSystemConst.ID = ID.ID_SYSTEM_CONST;
            btnCheckValue.ID = ID.ID_CHECK_VALUE;

            btnUndo.ID = ID.ID_UNDO;
            btnRedo.ID = ID.ID_REDO;

            btnSelect.ID = ID.ID_SELECT;
            btnDelSelect.ID = ID.ID_DELETE_SELECT;
            btnDelUnSelect.ID = ID.ID_DELETE_UNSELECT;
            
            m_arRibbonButtons.Add(btnOpenProject);
            m_arRibbonButtons.Add(btnSaveProject);
            m_arRibbonButtons.Add(btnSaveAsProject);

            m_arRibbonButtons.Add(btnOpenDXF);
            m_arRibbonButtons.Add(btnOpenDXF2);
            m_arRibbonButtons.Add(btnOpenShape);

            m_arRibbonButtons.Add(btnOption);

            m_arRibbonButtons.Add(btnLayer);
            m_arRibbonButtons.Add(btnDetailLayer);
            m_arRibbonButtons.Add(btnDetailAttrib);

            m_arRibbonButtons.Add(btnSystemConst);
            m_arRibbonButtons.Add(btnCheckValue);

            m_arRibbonButtons.Add(btnUndo);
            m_arRibbonButtons.Add(btnRedo);

            m_arRibbonButtons.Add(btnSelect);
            m_arRibbonButtons.Add(btnDelSelect);
            m_arRibbonButtons.Add(btnDelUnSelect);

            btnSelect.CheckButton = true;
            btnDelSelect.CheckButton = true;
            btnDelUnSelect.CheckButton = true;
            btnSelect.IsChecked = true;
		}

		protected void InitTopToolBar()
		{
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
			
		}

		

		
	}
}
