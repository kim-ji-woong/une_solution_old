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
    public partial class uHwall : UserControl
    {
        public uHwall()
        {
            InitializeComponent();
        }
        private Wall m_uHwall = null;
        public void SetHwallData(Wall wall, string strLevelName)
        {
            m_uHwall = wall;
            txtObject.Text = " 난간";
            lblFloor.Text = strLevelName;
            txtHeight.Text = m_uHwall.Height.ToString() + " ";
            txtThick.Text = m_uHwall.Thick.ToString() + " ";

            txtMaterial.Text = " ";            
            foreach (Property prop in m_uHwall.Properties)
            {
                if (prop.Name == "재질")
                    txtMaterial.Text = " " + prop.Value;                
            }
        }

        public void UpdateUserData()
        {
            foreach (Property prop in m_uHwall.Properties)
            {
                if (prop.Name == "재질")
                    prop.Value = txtMaterial.Text.Trim();               
            }
            m_uHwall.Thick = double.Parse(txtThick.Text.Trim());
            m_uHwall.Height = double.Parse(txtHeight.Text.Trim());
        }
    }
}
