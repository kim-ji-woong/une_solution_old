using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOPManager
{
	public partial class FormSaveOption : Form
	{

		private bool m_bDBSave = false;
		public bool SaveDB
		{
			get { return m_bDBSave; }
			set { m_bDBSave = value; }
		}

		public FormSaveOption()
		{
			InitializeComponent();
			mIconBox.BackgroundImage = Bitmap.FromHicon(System.Drawing.SystemIcons.Question.Handle);
		}

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

		private void button1_Click(object sender, EventArgs e)
		{
			// Save to XML
			m_bDBSave = false;
			DialogResult = DialogResult.Yes;
			this.Close();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			// Save to DB
			m_bDBSave = true;
			DialogResult = DialogResult.Yes;
			this.Close();
		}

		private void button3_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.No;
			this.Close();
		}

		private void button4_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			this.Close();
		}
	}
}
