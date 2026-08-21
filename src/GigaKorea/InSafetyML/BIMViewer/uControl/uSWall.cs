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
    public partial class uSWall : UserControl
    {
        public uSWall()
        {
            InitializeComponent();
        }
        private Wall m_uSwall = null;
        public void SetSwallData(Wall wall, string strLevelName)
        {
            m_uSwall = wall;
            txtObject.Text = " 구조벽";
            lblFloor.Text = strLevelName;

            //벽, 두께, 높이, 자재, 마감재
            txtHeight.Text = m_uSwall.Height.ToString() + " ";
            txtThick.Text = m_uSwall.Thick.ToString() + " ";

            txtMaterial.Text = " ";
            txtFinMaterial.Text = "";
            foreach(Property prop in m_uSwall.Properties)
            {
                if (prop.Name == "재질")
                    txtMaterial.Text = " " + prop.Value;
                else if (prop.Name == "마감재")
                    txtFinMaterial.Text = " " + prop.Value;
            }
        }

        public void UpdateUserData()
        {
            foreach (Property prop in m_uSwall.Properties)
            {
               if (prop.Name == "마감재")
                    prop.Value = txtFinMaterial.Text.Trim();
            }
        }
    }
}
