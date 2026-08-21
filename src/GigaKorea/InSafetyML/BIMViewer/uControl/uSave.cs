using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BIMViewer.uControl
{
    public partial class uSave : UserControl
    {
        private FormMain m_main = null;
        public uSave(FormMain main)
        {
            InitializeComponent();
            m_main = main;
        }           
    
        private void MouseLeaveEvent(object sender, EventArgs e)
        {
            m_main.HidePnlSave();            
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            m_main.SaveLocalPathXML();
        }

        private void BtnSaveAs_Click(object sender, EventArgs e)
        {
            m_main.SaveAsLocalPathXML();
        }
    }
}
