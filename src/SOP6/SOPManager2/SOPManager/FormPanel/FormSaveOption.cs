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

            ribbonButton1.Font = new Font(Program.prgFont, 10f, FontStyle.Bold);
            ribbonButton2.Font = new Font(Program.prgFont, 10f, FontStyle.Bold);
            ribbonButton3.Font = new Font(Program.prgFont, 10f, FontStyle.Bold);
            ribbonButton4.Font = new Font(Program.prgFont, 10f, FontStyle.Bold);

            UpdateControlSize();
		}

        public void UpdateControlSize()
        {
            Double[] dWindowRate = FormMain.Instance.GetCurWindowRate();
            double WindowRateWidth = dWindowRate[0];
            double WindowRateHeight = dWindowRate[1];

            this.Size = new System.Drawing.Size((int)(this.Size.Width * WindowRateWidth), (int)(this.Size.Height * WindowRateHeight));

            FormMain.Instance.UpdateWindowRate(mIconBox, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(label1, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(ribbonButton1, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(ribbonButton2, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(ribbonButton3, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(ribbonButton4, WindowRateWidth, WindowRateHeight);
        }

        private void HaveControl(Control pctl, double WindowRateWidth, double WindowRateHeight)
        {
            foreach (Control ctl in pctl.Controls)
            {
                if (ctl.Controls.Count > 0)
                    HaveControl(ctl, WindowRateWidth, WindowRateHeight);

                FormMain.Instance.UpdateWindowRate(ctl, WindowRateWidth, WindowRateHeight);
            }
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

        public ToolStripStatusLabel GetStatusLabel()
        {
            return null;
        }

        private void RibbonBtn_MouseDown(object sender, MouseEventArgs e)
        {
            UnE.GUI.RibbonButton rbtn = sender as UnE.GUI.RibbonButton;
            if (rbtn == null) return;

            rbtn.ForeColor = Color.Black;
        }

        private void RibbonBtn_MouseUp(object sender, MouseEventArgs e)
        {
            UnE.GUI.RibbonButton rbtn = sender as UnE.GUI.RibbonButton;
            if (rbtn == null) return;

            rbtn.ForeColor = Color.White;
        }
    }
}
