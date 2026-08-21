using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageEditor
{
    public partial class FormNewImage : Form
    {
        private int m_nPaintSizeWidth = 0;
        public int PaintSizeWidth
        {
            get { return m_nPaintSizeWidth; }
            set { m_nPaintSizeWidth = value; }
        }

        private int m_nPaintSizeHeight = 0;
        public int PaintSizeHeight
        {
            get { return m_nPaintSizeHeight; }
            set { m_nPaintSizeHeight = value; }
        }
        

        public FormNewImage()
        {
            InitializeComponent();
            txtWidth.Enabled = false;
            txtHeight.Enabled = false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(radioButton1.Checked)
            {
                m_nPaintSizeWidth = 640;
                m_nPaintSizeHeight = 480;
            }
            else if (radioButton2.Checked)
            {
                m_nPaintSizeWidth = 800;
                m_nPaintSizeHeight = 600;
            }
            else if (radioButton3.Checked)
            {
                m_nPaintSizeWidth = 1024;
                m_nPaintSizeHeight = 768;
            }
            else if (radioButton4.Checked)
            {
                if (txtWidth.Text == null || txtHeight.Text == "")
                {
                    UnE.Utility.UMessageBox.Show("빈칸이 있습니다.", "빈칸", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                else
                {
                    m_nPaintSizeWidth = Convert.ToInt32(txtWidth.Text);
                    m_nPaintSizeHeight = Convert.ToInt32(txtHeight.Text);
                }
            }

            FormMain.Instance.ContentForm.SetPaintSize(m_nPaintSizeWidth, m_nPaintSizeHeight);
            FormMain.Instance.PropertiesForm.SetImageGrid(m_nPaintSizeWidth, m_nPaintSizeHeight, "새 이미지");

            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if(txtWidth.Enabled)
            {
                txtWidth.Enabled = false;
                txtHeight.Enabled = false;
            }
            else
            {
                txtWidth.Enabled = true;
                txtHeight.Enabled = true;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
