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

namespace UnE.Utility.Print
{
	public partial class FormPrintPreview : Form, IMenuCommandOwner
	{
		private ArrayList m_arScaleMenuItems = new ArrayList();
		public FormPrintPreview()
		{
			InitializeComponent();


			Color backColor = Color.FromArgb(75, 71, 86);
			Color textColor = Color.White;

			CustomMenuHelper helper = new CustomMenuHelper(this);
			helper.MakeCustomLookMenu(this.toolStrip1, backColor, textColor);

			m_arScaleMenuItems.Add(toolStripMenuItem1);
			m_arScaleMenuItems.Add(toolStripMenuItem2);
			m_arScaleMenuItems.Add(toolStripMenuItem3);
			m_arScaleMenuItems.Add(toolStripMenuItem4);
			m_arScaleMenuItems.Add(toolStripMenuItem5);
			m_arScaleMenuItems.Add(toolStripMenuItem6);
			m_arScaleMenuItems.Add(toolStripMenuItem7);
			m_arScaleMenuItems.Add(toolStripMenuItem8);
			m_arScaleMenuItems.Add(toolStripMenuItem9);

			this.BackColor = backColor;

		}

		public System.Windows.Forms.PrintPreviewControl PreviewContorl
		{
			get { return this.printPreviewControl1;  }
		}

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			if (printPreviewControl1.Document != null)
				printPreviewControl1.Document.Print();
		}

		private void toolStripButton3_Click(object sender, EventArgs e)
		{
			if (this.ParentForm != null)
				ParentForm.Close();
			DialogResult = DialogResult.OK;
			this.Close();
			
		}

		private void CheckMenuItem(ToolStripMenuItem item, bool bCheck)
		{
			foreach(ToolStripMenuItem menu in m_arScaleMenuItems)
			{
				if( menu == item )
				{
					menu.Checked = bCheck;
				}
				else
				{
					menu.Checked = !bCheck;
				}
			}
		}

		private void scaleMenuItemClicked(object sender, EventArgs e)
		{
			
			ToolStripMenuItem item = (ToolStripMenuItem)sender;

			if (item.Checked == true)
				return;


			if( item.Text == "자동")
			{
				printPreviewControl1.AutoZoom = true;
				CheckMenuItem(item, true);
			}
			else
			{
				printPreviewControl1.AutoZoom = false;
				string tag = (string)item.Tag;
				int nTag = 100;
				if( !int.TryParse(tag, out nTag))
				{
					printPreviewControl1.AutoZoom = true;
					return;
				}
				double dZoom = nTag / 100.0;
				printPreviewControl1.Zoom = dZoom;
				CheckMenuItem(item, true);
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
			return null; ;
		}


		private Point mPrevPt;
		private bool m_bDragMode = false;
		private void printPreviewControl1_MouseDown(object sender, MouseEventArgs e)
		{
			mPrevPt = e.Location;
			m_bDragMode = true;
		}


		private void printPreviewControl1_MouseMove(object sender, MouseEventArgs e)
		{
			if( e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				if( m_bDragMode == true)
				{
					
				}
			}
			
		}

		private void printPreviewControl1_MouseUp(object sender, MouseEventArgs e)
		{
			m_bDragMode = false;
		}

		
	}


}
