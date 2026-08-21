using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SOPManager
{
	public partial class PopupSelectLevel : Form
	{
        //private bool m_bLeftMouseDown = false;
        //private Point m_ptMove = new Point();

        private System.Collections.ArrayList m_stepAlreadyAdd = null;

		public PopupSelectLevel()
		{
			InitializeComponent();

            panel1.Visible = false;
            SetRadioImage();
            UpdateControlSize();

            SetActionStepNames();

            m_stepAlreadyAdd = FormMain.Instance.GetPageLevel().GetTabPage();
            foreach (TabPage item in m_stepAlreadyAdd)
            {
                if (item.Text == rdLabel1.Text)
                {
                    rdPictureBox1.Enabled = false;
                    rdLabel1.Enabled = false;
                    rdPictureBox1.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_EnableFalse;
                }
                else if (item.Text == rdLabel2.Text)
                {
                    rdPictureBox2.Enabled = false;
                    rdLabel2.Enabled = false;
                    rdPictureBox2.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_EnableFalse;
                }
                else if (item.Text == rdLabel3.Text)
                {
                    rdPictureBox3.Enabled = false;
                    rdLabel3.Enabled = false;
                    rdPictureBox3.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_EnableFalse;
                }
                else if (item.Text == rdLabel4.Text)
                {
                    rdPictureBox4.Enabled = false;
                    rdLabel4.Enabled = false;
                    rdPictureBox4.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_EnableFalse;
                }
            }

            SetStandardActionStepNames();
        }

        private void SetActionStepNames()
        {
            int nCount = Data_ActionStep.StandardActionStepNames.Count();

            if (nCount >= 1)
                rdLabel1.Text = Data_ActionStep.StandardActionStepNames[0];

            if (nCount >= 2)
                rdLabel2.Text = Data_ActionStep.StandardActionStepNames[1];

            if (nCount >= 3)
                rdLabel3.Text = Data_ActionStep.StandardActionStepNames[2];

            if (nCount >= 4)
                rdLabel4.Text = Data_ActionStep.StandardActionStepNames[3];
        }

        public void UpdateControlSize()
        {
            Double[] dWindowRate = FormMain.Instance.GetCurWindowRate();
            double WindowRateWidth = dWindowRate[0];
            double WindowRateHeight = dWindowRate[1];

            this.Size = new System.Drawing.Size((int)(this.Size.Width * WindowRateWidth), (int)(this.Size.Height * WindowRateHeight));
            
            FormMain.Instance.UpdateWindowRate(groupBox1, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(rdPictureBox1, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(rdLabel1, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(rdPictureBox2, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(rdLabel2, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(rdPictureBox3, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(rdLabel3, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(rdPictureBox4, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(rdLabel4, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(btnOK, WindowRateWidth, WindowRateHeight);
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

		private string m_strLevelName = "";
		public string LevelName
		{
			get { return m_strLevelName; }
			set { m_strLevelName = value; }
		}

		private void radioButton1_CheckedChanged(object sender, EventArgs e)
		{
			m_strLevelName = rdLabel1.Text;
		}

		private void radioButton2_CheckedChanged(object sender, EventArgs e)
		{
			m_strLevelName = rdLabel2.Text;
        }

		private void radioButton3_CheckedChanged(object sender, EventArgs e)
		{
			m_strLevelName = rdLabel3.Text;
        }

		private void radioButton4_CheckedChanged(object sender, EventArgs e)
		{
			m_strLevelName = rdLabel4.Text;
        }

        private void SetRadioImage()
        {
            if (radioButton1.Checked == true)
            {
                rdPictureBox1.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Enable2;
            }
            else
            {
                if (!rdPictureBox1.Enabled)
                    rdPictureBox1.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_EnableFalse;
                else
                    rdPictureBox1.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Disable2;
            }

            if (radioButton2.Checked == true)
            {
                rdPictureBox2.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Enable2;
            }
            else
            {
                if (!rdPictureBox2.Enabled)
                    rdPictureBox2.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_EnableFalse;
                else
                    rdPictureBox2.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Disable2;
            }

            if (radioButton3.Checked == true)
            {
                rdPictureBox3.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Enable2;
            }
            else
            {
                if (!rdPictureBox3.Enabled)
                    rdPictureBox3.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_EnableFalse;
                else
                    rdPictureBox3.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Disable2;
            }

            if (radioButton4.Checked == true)
            {
                rdPictureBox4.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Enable2;
            }
            else
            {
                if (!rdPictureBox4.Enabled)
                    rdPictureBox4.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_EnableFalse;
                else
                    rdPictureBox4.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Disable2;
            }
        }

        private void rdPictureBox1_Click(object sender, EventArgs e)
        {
            rdLabel1_Click(sender, e);
        }

        private void rdPictureBox2_Click(object sender, EventArgs e)
        {
            rdLabel2_Click(sender, e);
        }

        private void rdPictureBox3_Click(object sender, EventArgs e)
        {
            rdLabel3_Click(sender, e);
        }

        private void rdPictureBox4_Click(object sender, EventArgs e)
        {
            rdLabel4_Click(sender, e);
        }

        private void rdLabel1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked == false)
            {
                radioButton1.Checked = !radioButton1.Checked;
                SetRadioImage();
            }
        }

        private void rdLabel2_Click(object sender, EventArgs e)
        {
            if (radioButton2.Checked == false)
            {
                radioButton2.Checked = !radioButton2.Checked;
                SetRadioImage();
            }
        }

        private void rdLabel3_Click(object sender, EventArgs e)
        {
            if (radioButton3.Checked == false)
            {
                radioButton3.Checked = !radioButton3.Checked;
                SetRadioImage();
            }
        }

        private void rdLabel4_Click(object sender, EventArgs e)
        {
            if (radioButton4.Checked == false)
            {
                radioButton4.Checked = !radioButton4.Checked;
                SetRadioImage();
            }
        }

        private void PopupSelectLevel_MouseDown(object sender, MouseEventArgs e)
        {
           // m_bLeftMouseDown = true;
            //m_ptMove = this.PointToScreen(new Point(e.X, e.Y));
        }

        private void PopupSelectLevel_MouseMove(object sender, MouseEventArgs e)
        {
            //if (e.Button == MouseButtons.Left)
            //{
            //    if (m_bLeftMouseDown == true)
            //    {
            //        Point pt = this.PointToScreen(new Point(e.X, e.Y));
            //        int dx = pt.X - m_ptMove.X;
            //        int dy = pt.Y - m_ptMove.Y;
            //        if (!(dx == 0 && dy == 0))
            //        {
            //            Point ptCur = this.Location;
            //            this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
            //            m_ptMove.X += dx;
            //            m_ptMove.Y += dy;
            //        }
            //    }
            //}
        }

        private void PopupSelectLevel_MouseUp(object sender, MouseEventArgs e)
        {
            //if (e.Button == System.Windows.Forms.MouseButtons.Left)
            //    m_bLeftMouseDown = false;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

		private void btnSelect_Click(object sender, EventArgs e)
		{
            if (m_strLevelName.Length == 0)
            {
                UnE.Utility.UMessageBoxRibbon.Show("추가할 단계를 선택하세요. ", "오류");
                return;
            }

			FormPageSOP pageLevel = FormMain.Instance.GetPageLevel();
			pageLevel.LevelTabSelected();

			DialogResult = DialogResult.OK;
			this.Close();
		}

        public void SetStandardActionStepNames()
        {
            this.radioButton1.Text = this.rdLabel1.Text = Data_ActionStep.StandardActionStepNames[0];
            this.radioButton2.Text = this.rdLabel2.Text = Data_ActionStep.StandardActionStepNames[1];
            this.radioButton3.Text = this.rdLabel3.Text = Data_ActionStep.StandardActionStepNames[2];
            this.radioButton4.Text = this.rdLabel4.Text = Data_ActionStep.StandardActionStepNames[3];
        }
    }
}
