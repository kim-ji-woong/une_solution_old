using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnE.GUI
{
	public partial class FormMain : Form
	{
		public FormMain()
		{
			InitializeComponent();

			
		}

		private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
		{

		}

		private void FormMain_Load(object sender, EventArgs e)
		{
			FormRibbon rbnForm = new FormRibbon();
			
			try
			{
				panelRibbon.Controls.Add(rbnForm);
			}
			catch(Exception ex)
			{
				int h = 0;
				h++;
			}
			rbnForm.Show();
		}

		private void sdfsdfToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}
	}
}
