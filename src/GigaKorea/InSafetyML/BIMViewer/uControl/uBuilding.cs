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
    public partial class uBuilding : UserControl
    {
        private Project m_uProject = null;

        public uBuilding()
        {
            InitializeComponent();
        }

        public void SetBuildingData(Project project)
        {
            string strBuildingType = null;
            string strPilotiType = null;

            m_uProject = project;

            if (m_uProject == null)
            {
                txtBuilding.Text = "";
                cmbBuildingType.SelectedIndex = 0;
                cmbPilotiType.SelectedIndex = 0;

                return;
            }

            txtBuilding.Text = project.Name;

            foreach (Property prop in m_uProject.Properties)
            {
                if (prop.Name == "건물구조")
                    strBuildingType = prop.Value;

                if (prop.Name == "필로티 구조물 여부")
                    strPilotiType = prop.Value;
            }

            if (strBuildingType != null)
            {
                cmbBuildingType.SelectedItem = strBuildingType;
            }

            if (strPilotiType != null)
            {
                if (strPilotiType == "1")
                    cmbPilotiType.SelectedItem = "예";
                else
                    cmbPilotiType.SelectedItem = "아니오";
            }
        }

        public void UpdateUserData()
        {
            if (m_uProject == null)
                return;

            foreach (Property prop in m_uProject.Properties)
            {
                if (prop.Name == "건물구조")
                {
                    string strBuildingType = (string)cmbBuildingType.SelectedItem;
                    prop.Value = strBuildingType;
                }

                if (prop.Name == "필로티 구조물 여부")
                {
                    int nPilotiType = cmbPilotiType.SelectedIndex;
                    prop.Value = nPilotiType.ToString();
                }
            }
        }
    }
}
