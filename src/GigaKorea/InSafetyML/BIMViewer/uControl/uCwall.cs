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
    public partial class uCWall : UserControl
    {
        public uCWall()
        {
            InitializeComponent();
        }
        private Wall m_uCwall = null;

        public void SetSwallData(Wall wall, string strLevelName)
        {
            m_uCwall = wall;
            txtObject.Text = " 유리벽";
            lblFloor.Text = strLevelName;

            //벽, 두께, 높이, 자재, 마감재
            txtHeight.Text = m_uCwall.Height.ToString() + " ";
            txtThick.Text = m_uCwall.Thick.ToString() + " ";

            txtMaterial.Text = " 유리";

        }
    }
}
