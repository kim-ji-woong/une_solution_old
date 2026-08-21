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
    public partial class uFwall : UserControl
    {
        public uFwall()
        {
            InitializeComponent();
        }
        private Wall m_uFwall = null;
        
        public void SetFwallData(Wall wall, string strLevelName)
        {
            m_uFwall = wall;
            txtObject.Text = " 가벽";
            lblFloor.Text = strLevelName;
            
            txtHeight.Text = m_uFwall.Height.ToString() + " ";
            txtThick.Text = m_uFwall.Thick.ToString() + " ";
            txtMaterial.Text = " ";
            txtFinMaterial.Text = "";
            foreach(Property prop in m_uFwall.Properties)
            {
                if (prop.Name == "재질")
                    txtMaterial.Text = " " + prop.Value;
                else if (prop.Name == "마감재")
                    txtFinMaterial.Text = " " + prop.Value;
            }            
        }
        public void UpdateUserData()
        {
            foreach (Property prop in m_uFwall.Properties)
            {
                if (prop.Name == "재질")
                    prop.Value = txtMaterial.Text.Trim();
                else if (prop.Name == "마감재")
                    prop.Value = txtFinMaterial.Text.Trim();                
            }
            m_uFwall.Thick = double.Parse(txtThick.Text.Trim());
            m_uFwall.Height = double.Parse(txtHeight.Text.Trim());
        }

    }
}
