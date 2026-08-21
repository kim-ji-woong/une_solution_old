using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Poi2Image
{
    public partial class FormPOI : Form
    {
        private POI m_poi = null;
        private string m_strPOIPath = "";

        public FormPOI()
        {
            InitializeComponent();
            EnableTextPosition();
        }

        private void FormPOI_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Count() == 1)
                {
                    string strFileName = files[0].ToLower();

                    if (strFileName.EndsWith("poi"))
                    {
                        e.Effect = DragDropEffects.Copy;
                        return;
                    }
                }
            }

            e.Effect = DragDropEffects.None;
        }

        private void FormPOI_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files.Count() == 1)
            {
                string strFileName = files[0].ToLower();

                if (strFileName.EndsWith("poi"))
                {
                    m_poi = POI.FromFile(strFileName);

                    if (m_poi != null)
                        m_strPOIPath = strFileName;
                    else
                        m_strPOIPath = "";

                    textBoxMoveX.Text = "0";
                    textBoxMoveY.Text = "0";
                    textBoxScale.Text = "1.0";

                    this.Text = m_strPOIPath;
                    EnableTextPosition();
                    Refresh();
                }
            }
        }

        private void EnableTextPosition()
        {
            textBoxMoveX.Enabled = textBoxMoveY.Enabled = btnApply.Enabled = btnSave.Enabled = m_strPOIPath.Length > 0;
        }

        private void FormPOI_Paint(object sender, PaintEventArgs e)
        {
            /*Pen pen = new Pen(Color.Black);
            UnE.Geometry.Vertex2D v1 = new UnE.Geometry.Vertex2D(50, 100);
            UnE.Geometry.Vertex2D v2 = new UnE.Geometry.Vertex2D(150, 100);
            UnE.Geometry.Vertex2D vCenter = (v1 + v2) / 2;

            e.Graphics.DrawLine(pen, (float)v1.x, (float)v1.y, (float)v2.x, (float)v2.y);

            Brush brush = new SolidBrush(Color.Black);
            e.Graphics.TranslateTransform((float)vCenter.x, (float)vCenter.y);
            e.Graphics.RotateTransform(90.0f);
            e.Graphics.TranslateTransform(-(float)vCenter.x, -(float)vCenter.y);
            e.Graphics.DrawString("가나다라", this.Font, brush, (float)vCenter.x, (float)vCenter.y);
            brush.Dispose();*/
            if (m_poi != null)
            {
                m_poi.Render(e.Graphics, 1.0, Color.Black);
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            string strX = textBoxMoveX.Text.Trim();
            string strY = textBoxMoveY.Text.Trim();

            int x, y;

            if (int.TryParse(strX, out x) && int.TryParse(strY, out y))
            {
                if (m_poi != null)
                {
                    m_poi.TextMoveX = x;
                    m_poi.TextMoveY = y;
                    Refresh();
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string strX = textBoxMoveX.Text.Trim();
            string strY = textBoxMoveY.Text.Trim();

            int x, y;

            if (int.TryParse(strX, out x) && int.TryParse(strY, out y))
            {
                if (m_poi != null)
                {
                    m_poi.SaveFile(m_strPOIPath);
                }
            }
        }

        private void btnApplyScale_Click(object sender, EventArgs e)
        {
            string strScale = textBoxScale.Text.Trim();

            double dScale;

            if (double.TryParse(strScale, out dScale) && dScale > 0.0)
            {
                if (m_poi != null)
                {
                    m_poi.Scale(dScale, this.CreateGraphics());
                    Refresh();
                }
            }
        }
    }
}
