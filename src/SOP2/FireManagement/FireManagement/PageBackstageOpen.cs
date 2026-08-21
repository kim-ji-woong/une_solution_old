using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace FireManagement
{
    public partial class PageBackstageOpen : Form
    {
        private Dictionary<BuildingGroup, List<Building>> m_dicBuildings = new Dictionary<BuildingGroup, List<Building>>();
        private Dictionary<Building, List<Floor>> m_dicFloors = new Dictionary<Building, List<Floor>>();
        private BuildingGroup m_prevBuildingGroup = null;
        private Building m_prevBuilding = null;

        public PageBackstageOpen()
        {
            InitializeComponent();

            if (!FormMain2.Instance.IsPCMode)
                btnOpenDXF.Visible = false;
        }

        private string GetFolderName(string strFilePath)
        {
            int nIndex = strFilePath.LastIndexOf('\\');
            return strFilePath.Substring(0, nIndex + 1);
        }

        private void btnFileOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "fmf Files (*.fmf)|*.fmf| All Files (*.*)|*.*";
            dlg.Multiselect = true;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                EventManager.Instance.ProcessEvent(Event.PREV_OPEN_FMF);

                ClearBuildings();

                FormMain2 frmMain = FormMain2.Instance;
                frmMain.CurrentZone = null;

                string strFolderName = GetFolderName(dlg.FileName);

                foreach (string strFileName in dlg.SafeFileNames)
                {
                    string strFilePath = strFolderName + strFileName;
                    bool isPCMode = FormMain2.Instance.IsPCMode;

                    if (frmMain.FileManager.ImportData(strFilePath, ref isPCMode))
                    {
                        if (frmMain.CurrentZone != null)
                            AddBuilding(frmMain.CurrentZone);
                    }
                    else
                    {
                        if (!FormMain2.Instance.IsPCMode && !isPCMode)
                        {
                            MessageBox.Show("Tablet에서는 PC에서 생성한 파일만 불러올 수 있습니다.");
                            return;
                        }
                    }
                }

                frmMain.CurrentZone = null;

                if (FormMain2.Instance.IsPCMode)
                    ShowBuildings();
                else
                    MessageBox.Show("FMF 갱신 완료");
                //FormMain2.Instance.FileManager.ImportData(dlg.FileName);

                EventManager.Instance.ProcessEvent(Event.POST_OPEN_FMF, dlg.FileName);
            }
        }

        private int GetFloorIndex(float fFloorIndex)
        {
            string strFloorIndex = string.Format("{0:f1}", fFloorIndex);
            int nFloorCount = cboFloor.Items.Count;

            for (int i = 0; i < nFloorCount; i++)
            {
                Floor floor = (Floor)cboFloor.Items[i];

                //if (floor.FloorIndex == nFloorIndex)
                if (strFloorIndex == string.Format("{0:f1}", floor.FloorIndex))
                    return i;
            }

            return -1;
        }

        private void AddBuilding(Zone zone)
        {
            Building building = zone.Building;

            if (building != null)
            {
                BuildingGroup buildingGroup = building.BuildingGroup;

                if (buildingGroup != null)
                {
                    int nBuildingGroupIndex = cboBuildingGroup.Items.IndexOf(buildingGroup);

                    if (nBuildingGroupIndex >= 0)
                        cboBuildingGroup.SelectedIndex = nBuildingGroupIndex;
                    else
                    {
                        cboBuildingGroup.Items.Add(buildingGroup);
                        cboBuildingGroup.SelectedIndex = cboBuildingGroup.Items.Count - 1;
                    }

                    int nBuildingIndex = cboBuilding.Items.IndexOf(building);

                    if (nBuildingIndex >= 0)
                        cboBuilding.SelectedIndex = nBuildingIndex;
                    else
                    {
                        cboBuilding.Items.Add(building);
                        cboBuilding.SelectedIndex = cboBuilding.Items.Count - 1;

                        List<Building> arrBuildings = null;

                        if (m_dicBuildings.ContainsKey(buildingGroup))
                            arrBuildings = m_dicBuildings[buildingGroup];
                        else
                        {
                            arrBuildings = new List<Building>();
                            m_dicBuildings[buildingGroup] = arrBuildings;
                        }

                        arrBuildings.Add(building);
                    }

                    int nFloorIndex = GetFloorIndex(zone.FloorIndex + zone.AddFloor);

                    if (nFloorIndex >= 0)
                        cboFloor.SelectedIndex = nFloorIndex;
                    else
                    {
                        Floor floor = new Floor(zone.FloorIndex + zone.AddFloor);
                        cboFloor.Items.Add(floor);
                        cboFloor.SelectedIndex = cboFloor.Items.Count - 1;

                        List<Floor> arrFloors = null;

                        if (m_dicFloors.ContainsKey(building))
                            arrFloors = m_dicFloors[building];
                        else
                        {
                            arrFloors = new List<Floor>();
                            m_dicFloors[building] = arrFloors;
                        }

                        arrFloors.Add(floor);
                        arrFloors.Sort();
                    }
                }
            }
        }

        private void ClearBuildings()
        {
            cboBuildingGroup.Items.Clear();
            cboBuilding.Items.Clear();
            cboFloor.Items.Clear();

            m_prevBuildingGroup = null;
            m_prevBuilding = null;
        }

        public void ShowBuildings()
        {
            cboBuildingGroup.Visible = true;
            cboBuilding.Visible = true;
            cboFloor.Visible = true;
        }

        public void HideBuildings()
        {
            cboBuildingGroup.Visible = false;
            cboBuilding.Visible = false;
            cboFloor.Visible = false;
        }

        private void cboBuildingGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            BuildingGroup buildingGroup = (BuildingGroup)cboBuildingGroup.Items[cboBuildingGroup.SelectedIndex];
            if (buildingGroup == m_prevBuildingGroup)
                return;

            m_prevBuildingGroup = buildingGroup;

            if (!m_dicBuildings.ContainsKey(buildingGroup))
                return;

            List<Building> arrBuildings = m_dicBuildings[buildingGroup];

            cboBuilding.Items.Clear();

            foreach (Building building in arrBuildings)
            {
                cboBuilding.Items.Add(building);
            }

            cboBuilding.SelectedIndex = 0;
        }

        private void cboBuilding_SelectedIndexChanged(object sender, EventArgs e)
        {
            Building building = (Building)cboBuilding.Items[cboBuilding.SelectedIndex];
            if (building == m_prevBuilding)
                return;

            m_prevBuilding = building;

            if (!m_dicFloors.ContainsKey(building))
                return;

            List<Floor> arrFloors = m_dicFloors[building];

            cboFloor.Items.Clear();

            foreach (Floor floor in arrFloors)
            {
                cboFloor.Items.Add(floor);
            }

            cboFloor.SelectedIndex = 0;
        }

        private void btnOpenDXF_Click(object sender, EventArgs e)
        {
            int nSelectedIndex = cboBuilding.SelectedIndex;
            if (nSelectedIndex < 0)
                return;

            Building building = (Building)cboBuilding.Items[nSelectedIndex];

            nSelectedIndex = cboFloor.SelectedIndex;
            if (nSelectedIndex < 0)
                return;

            FormMain2 frmMain = FormMain2.Instance;

            Floor floor = (Floor)cboFloor.Items[nSelectedIndex];
            Zone zone = frmMain.IOManager.FindZone(building, floor.FloorIndex);

            if (zone == null)
            {
                string strMsg = string.Format("{0} {1}에 해당하는 Zone 정보를 찾을수 없습니다.", building.BuildingName, floor.ToString());
                MessageBox.Show(strMsg);
                return;
            }
            else
            {
                if (frmMain.CurrentZone == zone)
                {
                   // frmMain.ChangeTab(ID.ID_TAB_FIREMANAGEMENT);
                    return;
                }

                frmMain.CurrentZone = zone;
            }

            if (zone.DXFFilePath == "")
            {
                string strMsg = string.Format("{0} {1}에 해당하는 도면 파일이 존재하지 않습니다.", building.BuildingName, floor.ToString());
                MessageBox.Show(strMsg);
                return;
            }
            else
            {
                if (!LoadDXF(zone.DXFFilePath, zone))
                {
                    string strMsg = string.Format("{0} 파일을 여는데 실패하였습니다.", zone.DXFFilePath);
                    MessageBox.Show(strMsg);
                    return;
                }

                EventManager.Instance.ProcessEvent(Event.NEW_DXF_OPENED, true);
            }
        }

        private bool LoadDXF(string strPath, Zone zone)
        {
            int nFECount, nHDCount, nFACount;

            if (FormMain2.Instance.DXFManager.LoadEquipment(strPath, zone, out nFECount, out nHDCount, out nFACount))
            {
                return true;
            }

            return false;
        }
    }
}
