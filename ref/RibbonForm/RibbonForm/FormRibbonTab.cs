using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnE.GUI.Contorl
{
	public delegate void RibbonTabSelected(object sender, RibbonTabEventArgs e);
	public delegate void RibbonTabDbClicked(object sender, RibbonTabEventArgs e);

	public partial  class FormRibbonTab : Form
	{
		public event RibbonTabSelected OnRibbonTabSelected;
		public event RibbonTabDbClicked OnRibbonTabDbClicked;
		
		private int m_nSavedHeight = 0;

		private bool m_bCollapse = false;
		public bool IsCollapsed
		{
			get { return m_bCollapse; }
		}
		
		private int m_nCollapseHeight = 0;
		public int CollapsedHeight
		{
			get { return m_nCollapseHeight; }
		}

		private TabPage m_SelectedTabPage = null;
		public TabPage SelectedTabPage
		{
			get { return m_SelectedTabPage; }
		}

		private int m_SelectedIndex = -1;
		public int SelectedIndex
		{
			get { return m_SelectedIndex; }
		}

		private Color m_SelectedTabColor = System.Drawing.Color.DarkGray;
		public Color SelectedTabColor
		{
			get { return m_SelectedTabColor; }
			set
			{
				m_SelectedTabColor = value;
				tabControlEx1.SelectedTabColor = value;
			}
		}

		private Color m_TabBackColor = System.Drawing.Color.FromArgb(54, 68, 82);
		public Color TabBackColor
		{
			get { return m_TabBackColor; }
			set
			{
				m_TabBackColor = value;
				tabControlEx1.TabBackColor = value;
			}
		}

		private Color m_TabForeColor = System.Drawing.Color.White;
		public Color TabForeColor
		{
			get { return m_TabForeColor; }
			set
			{
				m_TabForeColor = value;
				tabControlEx1.TabForeColor = value;
			}
		}

		private Size m_TabItemSize = new Size(80, 20);
		public Size TabSize
		{
			get { return m_TabItemSize; }
			set
			{
				m_TabItemSize = value;
				tabControlEx1.ItemSize = value;
			}
		}
		
		public new Font Font
		{
			get { return base.Font; }
			set
			{
				base.Font = value;
				tabControlEx1.Font = value;
			}
		}

		public FormRibbonTab()
		{
			InitializeComponent();

			
		}

		public TabControl.TabPageCollection TabPages
		{
			get { return tabControlEx1.TabPages; }
		}

		
		private void tabControlEx1_OnTabDoubleClicked(object sender, Controls.TabControlExEventArgs e)
		{
			if(m_bCollapse == false)
			{
				m_nSavedHeight = Parent.Size.Height;
				Parent.Size = new Size(Parent.Width, m_nCollapseHeight);
				m_bCollapse = true;

				if (OnRibbonTabDbClicked != null)
				{
					RibbonTabEventArgs args = new RibbonTabEventArgs(m_SelectedTabPage, m_SelectedIndex, TabControlAction.Selected);
					OnRibbonTabDbClicked(this, args);
				}
			}
			else
			{
				Parent.Size = new Size(Parent.Size.Width, m_nSavedHeight);
				m_bCollapse = false;

				if (OnRibbonTabDbClicked != null)
				{
					RibbonTabEventArgs args = new RibbonTabEventArgs(m_SelectedTabPage, m_SelectedIndex, TabControlAction.Deselected);
					OnRibbonTabDbClicked(this, args);
				}
			}
		}

		private void FormRibbon_ParentChanged(object sender, EventArgs e)
		{
			if(this.DesignMode == false)
			{
				if (Parent.Size.Height < this.Size.Height)
				{
					Parent.Size = new Size(Parent.Size.Width, this.Size.Height);
				}
			
			}
			
		}

		private void FormRibbon_Load(object sender, EventArgs e)
		{
			m_nSavedHeight = Parent.Size.Height;

			int nHeight = 0;
			for (int i = 0; i < tabControlEx1.TabCount; i++)
			{
				Rectangle rect = this.tabControlEx1.GetTabRect(i);
				if (rect.Height > nHeight)
					nHeight = rect.Height;
			}
			m_nCollapseHeight = nHeight + 3;
		}

		private void FormRibbon_FormClosing(object sender, FormClosingEventArgs e)
		{

		}

		
		private void tabControlEx1_Selected(object sender, TabControlEventArgs e)
		{
			m_SelectedTabPage = e.TabPage;
			m_SelectedIndex = e.TabPageIndex;
			if( OnRibbonTabSelected != null)
			{
				RibbonTabEventArgs args = new RibbonTabEventArgs(e.TabPage, e.TabPageIndex, e.Action);
				OnRibbonTabSelected(this, args);
			}
		}
	}

	public class RibbonTabEventArgs
	{
		private TabPage m_EventPage = null;
		public TabPage RibbonPage
		{
			get { return m_EventPage; }
		}

		private int m_EventPageIndex = -1;
		public int RibbonPageIndex
		{
			get { return m_EventPageIndex; }
		}
		
		private TabControlAction m_Action = TabControlAction.Selected;
		public TabControlAction Action
		{
			get { return m_Action; }
		}

		public RibbonTabEventArgs(TabPage tabPage, int tabPageIndex, TabControlAction action)
		{
			m_EventPage = tabPage;
			m_EventPageIndex = tabPageIndex;
			m_Action = action;
		}	
	}
}
