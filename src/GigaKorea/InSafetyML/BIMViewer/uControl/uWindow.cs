using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BIMViewer.BIM;

namespace BIMViewer.uControl
{
    public partial class uWindow : UserControl
    {
        public uWindow()
        {
            InitializeComponent();
        }
        private Window m_uWindow = null;
        public void SetWindowData(Window window, string strLevelName)
        {
            m_uWindow = window;
            txtObject.Text = " 창문";
            lblFloor.Text = strLevelName;

            //높이, 두께, 바닥높이
            txtHeight.Text = m_uWindow.Height.ToString() + " ";
            txtThick.Text = m_uWindow.Thick.ToString() + " ";
            txtElevation.Text = m_uWindow.Elevation.ToString() + " ";
        }

        public void UpdateUserData()
        {
            foreach (Property prop in m_uWindow.Properties)
            {
                if (prop.Name == "Thick")
                    prop.Value = txtThick.Text.Trim();                
            }
            m_uWindow.Thick = float.Parse(txtThick.Text.Trim());
            m_uWindow.Height = float.Parse(txtHeight.Text.Trim());
            m_uWindow.Elevation = float.Parse(txtElevation.Text.Trim());
        }
    }
}
