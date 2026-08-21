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
    public partial class PageBackstageNew : Form
    {
        public PageBackstageNew()
        {
            InitializeComponent();

            Init();
        }

        private void Init()
        {
            InspectionItem();
            DocumentVersion();
            InitComboBox();
            InitGrid();
        }

        private void InitGrid()
        {
            DataGridViewRow row = new DataGridViewRow();
            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();

            cell.Value = "소화기";
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = "FE";
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = "";
            row.Cells.Add(cell);
            
            dataGridEquipment.Rows.Add(row);
            row = new DataGridViewRow();

            cell = new DataGridViewTextBoxCell();
            cell.Value = "소화전";
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = "HD";
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = "";
            row.Cells.Add(cell);

            dataGridEquipment.Rows.Add(row);
            row = new DataGridViewRow();

            cell = new DataGridViewTextBoxCell();
            cell.Value = "발신기";
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = "FA";
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = "";
            row.Cells.Add(cell);

            dataGridEquipment.Rows.Add(row);
        }
        
        private void InspectionItem()
        {
        
        }

        private void DocumentVersion()
        {
            string[] strValue = new string[] { "버전", "파일 생성일", "작성자", "설명" };
            for(int i = 0; i < 4; i++)
            {
                DataGridViewRow gridRow = new DataGridViewRow();
                DataGridViewCell cell = null;

                cell = new DataGridViewTextBoxCell();
                cell.Value = strValue[i];
                gridRow.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = "V_1.0";
                gridRow.Cells.Add(cell);

                dataGridVersion.Rows.Add(gridRow);
            }

            dataGridVersion.Rows[1].Cells[1].Value = DateTime.Today.ToString();
        }

        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cboBuildingGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nSelectedIndex = cboBuildingGroup.SelectedIndex;
            if (nSelectedIndex < 0)
                return;

            cboBuilding.Items.Clear();

            BuildingGroup buildingGroup = (BuildingGroup)cboBuildingGroup.Items[nSelectedIndex];

            if (buildingGroup.ID > 0)
            {
                IOManager mgr = FormMain2.Instance.IOManager;
                if (!mgr.AllBuildingGroups.ContainsKey(buildingGroup))
                    return;

                IOManager ioMgr = FormMain2.Instance.IOManager;
                ArrayList arrBuildings = ioMgr.AllBuildingGroups[buildingGroup];

                foreach (Building building in arrBuildings)
                {
                    ArrayList arrZones = ioMgr.GetBuildingZones(building.ID);

                    if (arrZones != null)
                    {
                        // Zone이 하나도 없는 빌딩, 즉 도면이 하나도 없는 빌딩은 콤보박스에 보여주지 않는다.
                        cboBuilding.Items.Add(building);
                    }
                }
            }
            else
            {
                Dictionary<int, Zone> dicOutdoorZones = FormMain2.Instance.IOManager.OutdoorZones;

                foreach (KeyValuePair<int, Zone> pair in dicOutdoorZones)
                {
                    cboBuilding.Items.Add(pair.Value);
                }
            }

            if (cboBuilding.Items.Count > 0)
                cboBuilding.SelectedIndex = 0;
        }

        private void AddBuildingZone(Dictionary<int, Zone> dicZones, Building building)
        {
            ArrayList arrFloor = new ArrayList();

            foreach (KeyValuePair<int, Zone> pair in dicZones)
            {
                if (pair.Value.Building == building)
                    arrFloor.Add(new Floor(pair.Value.FloorIndex + pair.Value.AddFloor));
            }

            arrFloor.Sort();

            foreach (Floor floor in arrFloor)
            {
                cboFloor.Items.Add(floor);
            }
        }

        private void cboBuilding_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nSelectedIndex = cboBuilding.SelectedIndex;
            if (nSelectedIndex < 0)
                return;

            cboFloor.Items.Clear();

            Object obj = cboBuilding.Items[nSelectedIndex];
            Type type = obj.GetType();
            
            //if (nSelectedIndex < m_arrBuildings.Count)
            if (type == typeof(Building))
            {
                //Building building = (Building)m_arrBuildings[nSelectedIndex];
                Building building = (Building)obj;

                AddBuildingZone(FormMain2.Instance.IOManager.AllZones, building);

                //for (int i = building.MinFloorIndex; i <= building.MaxFloorIndex; i++)
                //{
                //    cboFloor.Items.Add(new Floor(i));
                //    /*if (i < 0)
                //        cboFloor.Items.Add(string.Format("지하 {0}층", -i));
                //    else
                //        cboFloor.Items.Add(string.Format("{0}층", i + 1));*/
                //}
            }
            else
            {
                cboFloor.Items.Add("-");
            }

            if (cboFloor.Items.Count > 0)
                cboFloor.SelectedIndex = 0;
        }

        private void cboFloor_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnDlgOpen_Click(object sender, EventArgs e)
        {
            //FileDialog dlg = new OpenFileDialog();
            //dlg.Filter = "dxf Files (*.dxf)|*.dxf| All Files (*.*)|*.*";

            //if (dlg.ShowDialog() == DialogResult.OK)
            //{
            //    if (textFile.Text == dlg.FileName)
            //        return;

            //    textFile.Text = dlg.FileName;
            //    LoadDXF(dlg.FileName);
            //}
        }

        private bool LoadDXF(string strPath, Zone zone)
        {
            int nFECount, nHDCount, nFACount;

            if (FormMain2.Instance.DXFManager.LoadEquipment(strPath, zone, out nFECount, out nHDCount, out nFACount))
            {
                dataGridEquipment.Rows[0].Cells[2].Value = nFECount.ToString();
                dataGridEquipment.Rows[1].Cells[2].Value = nHDCount.ToString();
                dataGridEquipment.Rows[2].Cells[2].Value = nFACount.ToString();
                dataGridEquipment.Columns[2].Visible = true;

                //EventManager.Instance.ProcessEvent(Event.NEW_DXF_OPENED);
                // 적용 버튼을 누르기 전에는 안보이도록 한다.
                //FormMain2.Instance.DXFControl.Visible = false;

                return true;
            }

            return false;
        }

        public void ReloadData()
        {
            cboBuildingGroup.Items.Clear();
            cboBuilding.Items.Clear();
            cboFloor.Items.Clear();

            InitComboBox();
        }

        private void InitComboBox()
        {
            /*LoadBuildings();
            LoadZones();

            foreach (Building building in m_arrBuildings)
            {
                cboBuilding.Items.Add(building.BuildingName);
            }

            foreach (Zone zone in m_arrOutdoorZones)
            {
                cboBuilding.Items.Add(zone.ZoneName);
            }*/

            IOManager mgr = FormMain2.Instance.IOManager;

            foreach (KeyValuePair<BuildingGroup, ArrayList> pair in mgr.AllBuildingGroups)
            {
                cboBuildingGroup.Items.Add(pair.Key);
            }

            if (cboBuildingGroup.Items.Count > 0)
                cboBuildingGroup.SelectedIndex = 0;

            /*foreach (KeyValuePair<int, Building> pair in mgr.AllBuildings)
            {
                cboBuilding.Items.Add(pair.Value);
            }

            foreach (KeyValuePair<int, Zone> pair in mgr.OutdoorZones)
            {
                cboBuilding.Items.Add(pair.Value);
            }

            if (cboBuilding.Items.Count > 0)
                cboBuilding.SelectedIndex = 0;*/
        }

        /*public void OpenZone(Zone zone)
        {
            FormMain frmMain = FormMain2.Instance;
            Building building = zone.Building;

            if (building != null)
            {
                BuildingGroup buildingGroup = building.BuildingGroup;

                if (buildingGroup != null)
                {
                    int nItemCount = cboBuildingGroup.Items.Count;

                    for (int i=0;i<nItemCount;i++)
                    {
                        BuildingGroup group = (BuildingGroup)cboBuildingGroup.Items[i];

                        if (group.BuildingGroupName == buildingGroup.BuildingGroupName)
                        {
                            cboBuildingGroup.SelectedIndex = i;

                            int nBuildingCount = cboBuilding.Items.Count;

                            for (int j = 0; j < nBuildingCount; j++)
                            {
                                Building _building = (Building)cboBuilding.Items[j];

                                if (_building.BuildingName == building.BuildingName)
                                {
                                    cboBuilding.SelectedIndex = j;

                                    int nFloorCount = cboFloor.Items.Count;

                                    for (int k = 0; k < nFloorCount; k++)
                                    {
                                        Floor floor = (Floor)cboFloor.Items[k];

                                        if (floor.FloorIndex == zone.FloorIndex)
                                        {
                                            cboFloor.SelectedIndex = k;
                                            break;
                                        }
                                    }

                                    break;
                                }
                            }

                            break;
                        }
                    }
                }
            }

            if (zone.DXFFilePath == "")
            {
                Floor floor = new Floor(zone.FloorIndex);
                string strMsg = string.Format("{0} {1}에 해당하는 도면 파일이 존재하지 않습니다.", building.BuildingName, floor.ToString());
                MessageBox.Show(strMsg);
                return;
            }
            else
            {
                if (!LoadDXF(zone.DXFFilePath))
                {
                    string strMsg = string.Format("{0} 파일을 여는데 실패하였습니다.", zone.DXFFilePath);
                    MessageBox.Show(strMsg);
                    return;
                }

                EventManager.Instance.ProcessEvent(Event.NEW_DXF_OPENED);
            }
        }*/

        //private void Test()
        //{

        //    IOManager mgr = FormMain2.Instance.IOManager;

        //    foreach (BuildingGroup group in cboBuildingGroup.Items)
        //    {
        //        if (!mgr.AllBuildingGroups.ContainsKey(group))
        //            return;

        //        ArrayList arrBuildings = mgr.AllBuildingGroups[group];
        //        int nCount = 0;

        //        foreach (Building building in arrBuildings)
        //        {
        //            for (int i = building.MinFloorIndex; i <= building.MaxFloorIndex; i++)
        //            {
        //                Zone zone = mgr.FindZone(building, i);
        //                if (zone == null)
        //                    continue;
        //                if (zone.DXFFilePath == "")
        //                    continue;

        //                FormMain2.Instance.CurrentZone = zone;

        //                if (!LoadDXF(zone.DXFFilePath, zone))
        //                {
        //                    continue;
        //                }
        //                else
        //                {
        //                    DXFManager dxfMgr = FormMain2.Instance.DXFManager;

        //                    if (dxfMgr.SaveToDB())
        //                    {
        //                        IOManager ioMgr = FormMain2.Instance.IOManager;
        //                        ioMgr.ApplyEquipments(dxfMgr.Equipments, FormMain2.Instance.CurrentZone);
        //                        ioMgr.ApplyEquipmentHistory(dxfMgr.EquipmentHistory);

        //                        System.Diagnostics.Trace.WriteLine(string.Format("[{0}] {1} DB 저장 성공", ++nCount, zone.DXFFilePath));
        //                    }
        //                    else
        //                    {
        //                        System.Diagnostics.Trace.WriteLine(string.Format("[{0}] {1} DB 저장 실패", ++nCount, zone.DXFFilePath));
        //                        return;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    MessageBox.Show("DB 저장 끝");
        //}

        private void btnApply_Click(object sender, EventArgs e)
        {
            //Test();
            //return;
            /*if (dataGridEquipment.Columns[2].Visible == false)
            {
                MessageBox.Show("먼저 DXF 파일을 열어주세요.");
                return;
            }*/

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
                string strMsg = string.Format("{0} {1}에 해당하는 Zone 정보가 DB에 존재하지 않습니다.", building.BuildingName, floor.ToString());
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

                EventManager.Instance.ProcessEvent(Event.NEW_DXF_OPENED);
            }

            /*string strMsg2 = string.Format("로딩된 소방설비 데이터를 {0} {1}에 적용하시겠습니까?", building.BuildingName, floor.ToString());
            
            if (MessageBox.Show(strMsg2, "소방설비 등록", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                EventManager.Instance.ProcessEvent(Event.NEW_DXF_OPENED);
                //ApplyNewEquipment(zone);
            }*/
        }

        private void ApplyNewEquipment(Zone zone)
        {
            if (FormMain2.Instance.DXFManager.SetEquipmentZone(zone))
            {
                MessageBox.Show("소방설비 등록 완료");
                //FormMain2.Instance.DXFControl.Visible = true;
            }
            else
                MessageBox.Show("소방설비 등록 실패");
        }

        /*private void LoadZones()
        {
            WebDBManager dbMgr = FormMain2.Instance.DBManager;

            string strSQL = "Select id, ZoneName from Zone where BuildingID = -1";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i <= nResultCount - 1; i += 2)
            {
                int nID = dbMgr.GetIntField(arrResult[i].ToString(), -1);
                string strZoneName = dbMgr.GetStringField(arrResult[i + 1], "");

                Zone zone = new Zone();

                zone.ZoneName = strZoneName;
                m_arrOutdoorZones.Add(zone);
            }
        }*/

        /*public ArrayList Buildings
        {
            get { return m_arrBuildings; }
            set { m_arrBuildings = value; }
        }*/
    }
}
