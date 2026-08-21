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
		protected void InitLeftToolBar()
		{
			btnOpenXML.ForeColor = Color.White;
			btnOpenXML.Text = "XML 열기";
			btnOpenXML.ID = ID.ID_XML_OPEN;
			
			btnStart.ForeColor = Color.White;
			btnStart.Text = "시작하기";
			btnStart.ID = ID.ID_FILE_START;

            btnFinish.ForeColor = Color.White;
            btnFinish.Text = "중지하기";
            btnFinish.ID = ID.ID_FILE_FINISH;

			m_arRibbonButtons.Add(btnOpenXML);
			m_arRibbonButtons.Add(btnStart);
            m_arRibbonButtons.Add(btnFinish);
		}

		protected void InitTopToolBar()
		{			
            //rBtnUndo.ID = ID.ID_EDIT_UNDO;	
            //rBtnRedo.ID = ID.ID_EDIT_REDO;

            //m_arRibbonButtons.Add(rBtnUndo);
            //m_arRibbonButtons.Add(rBtnRedo);
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
            RibbonButton btn = new RibbonButton();
            btn.ID = ID.ID_VIEW_RIGHTPANE;
            FormMain.Instance.OnRibbonButtonMouseUp(btn, null);
		}

	    public void SimulatorMessage(SOPSimulator.SimulatorMessage msg)
        {
            if (msg == SOPSimulator.SimulatorMessage.DISABLE_START)
            {
                btnStart.Enabled = false;
                btnFinish.Enabled = true;
            }
            else if (msg == SOPSimulator.SimulatorMessage.ENABLE_START)
            {
                btnStart.Enabled = true;
                btnFinish.Enabled = false;
            }
        }
	}
}
