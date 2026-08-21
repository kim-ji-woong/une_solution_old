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
        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();

		public PopupSelectLevel()
		{
			InitializeComponent();

            panel1.Visible = false;
            SetRadioImage();
		}

		private string m_strLevelName = "";
		public string LevelName
		{
			get { return m_strLevelName; }
			set { m_strLevelName = value; }
		}
		private void button1_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;

            FormPageSOP pageLevel = FormMain.Instance.GetPageLevel();
            pageLevel.LevelTabSelected();
		}

		private void radioButton1_CheckedChanged(object sender, EventArgs e)
		{
			m_strLevelName = "예방";
		}

		private void radioButton2_CheckedChanged(object sender, EventArgs e)
		{
			m_strLevelName = "대비";
		}

		private void radioButton3_CheckedChanged(object sender, EventArgs e)
		{
			m_strLevelName = "대응";
		}

		private void radioButton4_CheckedChanged(object sender, EventArgs e)
		{
			m_strLevelName = "복구";
		}

        private void SetRadioImage()
        {
            if (radioButton1.Checked == true)
            {
                rdPictureBox1.BackgroundImage = global::SOPManager.Properties.Resources.rdselect_black;
            }
            else
            {
                rdPictureBox1.BackgroundImage = global::SOPManager.Properties.Resources.rdselect_gray;
            }

            if (radioButton2.Checked == true)
            {
                rdPictureBox2.BackgroundImage = global::SOPManager.Properties.Resources.rdselect_black;
            }
            else
            {
                rdPictureBox2.BackgroundImage = global::SOPManager.Properties.Resources.rdselect_gray;
            }

            if (radioButton3.Checked == true)
            {
                rdPictureBox3.BackgroundImage = global::SOPManager.Properties.Resources.rdselect_black;
            }
            else
            {
                rdPictureBox3.BackgroundImage = global::SOPManager.Properties.Resources.rdselect_gray;
            }

            if (radioButton4.Checked == true)
            {
                rdPictureBox4.BackgroundImage = global::SOPManager.Properties.Resources.rdselect_black;
            }
            else
            {
                rdPictureBox4.BackgroundImage = global::SOPManager.Properties.Resources.rdselect_gray;
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
            m_bLeftMouseDown = true;
            m_ptMove = this.PointToScreen(new Point(e.X, e.Y));
        }

        private void PopupSelectLevel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_bLeftMouseDown == true)
                {
                    Point pt = this.PointToScreen(new Point(e.X, e.Y));
                    int dx = pt.X - m_ptMove.X;
                    int dy = pt.Y - m_ptMove.Y;
                    if (!(dx == 0 && dy == 0))
                    {
                        Point ptCur = this.Location;
                        this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
                        m_ptMove.X += dx;
                        m_ptMove.Y += dy;
                    }
                }
            }
        }

        private void PopupSelectLevel_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
	}
}
