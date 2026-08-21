using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DXFView
{
    public partial class Form1 : Form
    {
        private static Form1 m_instance = null;

        public static Form1 Instance
        {
            get { return m_instance; }
        }

        public Form1()
        {
            m_instance = this;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "DXF Viewer";
        }

        private void dxfControl1_MouseMove(object sender, MouseEventArgs e)
        {
            UnE.Geometry.Vertex2D vertex = dxfControl1.ScreenToGlobal(e.X, e.Y);

            if (vertex != null)
                toolStripStatusLabel1.Text = string.Format("({0}, {1})", vertex.x, vertex.y);
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "DXF Files|*.dxf|All FIles|*.*";
            dlg.FilterIndex = 0;
            dlg.Title = "DXF 파일 열기";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                dxfControl1.BackColor = Color.Black;
                bool isSuccess = dxfControl1.OpenDXF(dlg.FileName);
                toolStripStatusLabel1.Text = "";

                if (!isSuccess)
                {
                    string strError = "DXF 불러오기가 실패하였습니다.";
                    MessageBox.Show(strError);
                }
                else
                {
                    ReadShapeTypes();
                    this.Text = dlg.FileName;
                    panelLeft.Layers = dxfControl1.Layers;
                    panelLeft.Blocks = dxfControl1.Blocks;
                    panelLeft.Init();
                }
            }
        }

        private void ReadShapeTypes()
        {
            int nTypeCount = 0;
            Dictionary<string, int> dicShapeTypeCount = new Dictionary<string, int>();

            foreach (DXFViewer.Layer layer in dxfControl1.Layers)
            {
                foreach (DXFViewer.Shape shape in layer.Shapes)
                {
                    string str = shape.GetType().ToString();

                    if (dicShapeTypeCount.TryGetValue(str, out nTypeCount))
                        dicShapeTypeCount[str] = ++nTypeCount;
                    else
                        dicShapeTypeCount[str] = 1;
                }
            }

            foreach (KeyValuePair<string, int> pair in dicShapeTypeCount)
            {
                System.Diagnostics.Trace.WriteLine(pair.Key + " : " + pair.Value.ToString());
            }
        }

        public void RefreshView()
        {
            dxfControl1._Refresh();
        }
    }
}
