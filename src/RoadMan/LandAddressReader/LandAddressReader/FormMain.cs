using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LandAddressReader
{
    public partial class FormMain : Form
    {
        private DataManager m_dataMgr = new DataManager();

        public FormMain()
        {
            InitializeComponent();
        }

        private void btnOpenDXF_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "DXF Files|*.dxf|All FIles|*.*";
            dlg.FilterIndex = 0;
            dlg.Title = "DXF 파일 열기";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.Cursor = Cursors.WaitCursor;

                double x = 0, y = 0;
                bool readX = false, readY = false;

                if (textBoxTargetX.Text.Length > 0)
                {
                    if (double.TryParse(textBoxTargetX.Text, out x))
                        readX = true;
                }

                if (textBoxTargetY.Text.Length > 0)
                {
                    if (double.TryParse(textBoxTargetY.Text, out y))
                        readY = true;
                }

                UnE.Geometry.Vertex2D vMovedTarget = readX && readY ? new UnE.Geometry.Vertex2D(x, y) : null;

                int nOverLayerCount, nEmptyLayerCount;
                m_dataMgr.ReadDXF(dlg.FileName, vMovedTarget, out nOverLayerCount, out nEmptyLayerCount);

                labelResult.Text = m_dataMgr.ResultString;
                labelResult.Visible = true;

                labelOverLayers.Text = "Over Layer Count : " + nOverLayerCount.ToString();
                labelEmptyLayers.Text = "Empty Layer Count : " + nEmptyLayerCount.ToString();
                labelOverLayers.Visible = true;
                labelEmptyLayers.Visible = true;

                this.Cursor = Cursors.Arrow;
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();

            dlg.Filter = "XML Files|*.xml|All FIles|*.*";
            dlg.FilterIndex = 0;
            dlg.Title = "XML 파일 저장";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                m_dataMgr.SaveXML(dlg.FileName);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
