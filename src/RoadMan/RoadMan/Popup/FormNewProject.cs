using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RoadMan
{
    public partial class FormNewProject : Form
    {
        private string m_strDXFPath = "";

        public string DXFPath
        {
            get { return m_strDXFPath; }
            set { m_strDXFPath = value; }
        }

        public FormNewProject()
        {
            InitializeComponent();
        }

        private void btnDXFPath_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "DXF Files|*.dxf|All FIles|*.*";
            dlg.FilterIndex = 0;
            dlg.Title = "DXF 파일 열기";


            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBoxDXFPath.Text = dlg.FileName;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            m_strDXFPath = textBoxDXFPath.Text;
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
