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
    public partial class uCircle : UserControl
    {
        public uCircle()
        {
            InitializeComponent();
        }
        
        public void ShowCircleData(Column column, string sHeight, string strLevelName)
        {
            txtObject.Text = " 원기둥";
            lblFloor.Text = strLevelName;
                      
            txtDiameter.Text = (column.CircleData.Radius * 2).ToString() + " ";
            txtHeight.Text = sHeight + " ";

            txtMaterial.Text = "";
            foreach (Property prop in column.Properties)
            {
                if (prop.Name == "재질")
                    txtMaterial.Text = " " + prop.Value;
            }
        }        
    }
}
