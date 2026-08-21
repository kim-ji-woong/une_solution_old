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
    public partial class uDoor : UserControl
    {       
        public uDoor()
        {
            InitializeComponent();
            cmbYN.SelectedIndex = 0;
        }

        private Door m_uDoor = null;
        public void SetDoorData(Door door, string strLevelName)
        {
            m_uDoor = door;                   
            txtObject.Text = " 문";
            lblFloor.Text = strLevelName;
        
            txtHeight.Text = m_uDoor.Height.ToString() + " ";
            txtThick.Text = m_uDoor.Thick.ToString() + " ";

            cmbYN.SelectedIndex = 0;
            foreach(Property prop in m_uDoor.Properties)
            {
                if (prop.Name == "방화문유무" && prop.Value == "1")
                    cmbYN.SelectedIndex = 1;
            }
        }       
        public void UpdateUserData()
        { 
            double dThick, dHeight;
            double.TryParse(txtThick.Text, out dThick);
            double.TryParse(txtHeight.Text, out dHeight);

            m_uDoor.Thick = (float)dThick;
            m_uDoor.Height = (float)dHeight;

           foreach(Property prop in m_uDoor.Properties)
           {
                if (prop.Name == "Thick")
                    prop.Value = dThick.ToString();
                else if (prop.Name == "방화문유무")
                    prop.Value = cmbYN.SelectedIndex.ToString();
           }
        }
    }
}
