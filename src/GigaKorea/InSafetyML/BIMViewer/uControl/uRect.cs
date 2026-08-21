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
    public partial class uRect : UserControl
    {
        public uRect()
        {
            InitializeComponent();
        }
        public void ShowRectData(Column column, string sHeight, string strLevelName)
        {
            txtObject.Text = " 사각기둥";
            lblFloor.Text = strLevelName;

            double dThick, dWidth;           
            dThick = column.RectData.TopLeft.GetDistance(column.RectData.BottomLeft);
            dWidth = column.RectData.BottomLeft.GetDistance(column.RectData.BottomRight);

            txtThick.Text= dThick.ToString() + " ";
            txtWidth.Text = dWidth.ToString() + " ";
            txtHeight.Text = sHeight + " ";

            txtMaterial.Text = "";               
            foreach(Property prop in column.Properties)
            {
                if (prop.Name == "재질")
                    txtMaterial.Text = " " + prop.Value;
            }
        }       
    }
}
