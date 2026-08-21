using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using DBUtility2;

namespace RtspUrlEditor
{
    public partial class FormEquipZoneCCTV : Form
    {
        private FormCCTVList m_frmCCTVList = null;
        private bool m_use4CCTV = true;

        public FormEquipZoneCCTV()
        {
            InitializeComponent();

            int nCCTVCount;

            if (ReadConfig("CCTVCount", out nCCTVCount))
            {
                if (nCCTVCount == 6)
                {
                    pictureBoxScreen.Image = global::RtspUrlEditor.Properties.Resources.cctv_6;
                    colCCTV5.Visible = colCCTV6.Visible = true;
                    m_use4CCTV = false;
                }
                else// if (nCCTVCount == 4)
                {
                    pictureBoxScreen.Image = global::RtspUrlEditor.Properties.Resources.cctv_4;
                    colCCTV5.Visible = colCCTV6.Visible = false;
                    m_use4CCTV = true;
                }
            }
            else
            {
                pictureBoxScreen.Image = global::RtspUrlEditor.Properties.Resources.cctv_4;
                colCCTV5.Visible = colCCTV6.Visible = false;
                m_use4CCTV = true;
            }

            InitData();
        }

        private void InitData()
        {
            Dictionary<int, List<Building>> dicBuildingGroups = FormMain.Instance.DataManager.BuildingGroups;

            foreach (KeyValuePair<int, List<Building>> pair in dicBuildingGroups)
            {
                if (pair.Value.Count == 0)
                    continue;

                string strBuildingGroupName = pair.Value[0].BuildingGroupName;
                BuildingGroup buildingGroup = new BuildingGroup();

                buildingGroup.ID = pair.Key;
                buildingGroup.Name = strBuildingGroupName;

                cboBuildingGroup.Items.Add(buildingGroup);
            }

            List<Zone> zones = FormMain.Instance.DataManager.OutdoorZones;

            if (zones.Count > 0)
            {
                BuildingGroup buildingGroup = new BuildingGroup();
                buildingGroup.IsOutdoor = true;

                cboBuildingGroup.Items.Add(buildingGroup);
            }
        }

        private bool ReadConfig(string strName, out int value)
        {
            string strValue = System.Configuration.ConfigurationManager.AppSettings[strName].ToString().Trim();
            return int.TryParse(strValue, out value);
        }

        private void panelCCTV_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void panelCCTV_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(CCTV)))
            {
                CCTVPanel panel = (CCTVPanel)sender;
                CCTV cctv = (CCTV)e.Data.GetData(typeof(CCTV));
                panel.Connect(cctv);
            }
        }

        private void panelCCTV_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(CCTV)))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void btnCCTVList_Click(object sender, EventArgs e)
        {
            if (m_frmCCTVList == null || m_frmCCTVList.IsDisposed)
            {
                m_frmCCTVList = new FormCCTVList();
            }

            m_frmCCTVList.Show(this);
        }

        private void FormEquipZoneCCTV_Load(object sender, EventArgs e)
        {
            if (cboBuildingGroup.Items.Count > 0)
                cboBuildingGroup.SelectedIndex = 0;
        }

        private void cboBuildingGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboBuildingGroup.SelectedIndex < 0)
                return;

            gridEquipZoneCCTV.Rows.Clear();
            cboZone.Items.Clear();
            cboBuilding.Items.Clear();

            BuildingGroup buildingGroup = (BuildingGroup)cboBuildingGroup.Items[cboBuildingGroup.SelectedIndex];

            if (buildingGroup.IsOutdoor)
            {
                cboBuilding.Visible = false;
                List<Zone> outdoorZones = FormMain.Instance.DataManager.OutdoorZones;

                foreach (Zone zone in outdoorZones)
                {
                    cboZone.Items.Add(zone);
                }

                if (cboZone.Items.Count > 0)
                    cboZone.SelectedIndex = 0;
            }
            else
            {
                cboBuilding.Visible = true;
                List<Building> buildings;

                if (FormMain.Instance.DataManager.BuildingGroups.TryGetValue(buildingGroup.ID, out buildings))
                {
                    foreach (Building building in buildings)
                    {
                        cboBuilding.Items.Add(building);
                    }

                    if (cboBuilding.Items.Count > 0)
                        cboBuilding.SelectedIndex = 0;
                }
            }
        }

        private void cboBuilding_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboBuilding.SelectedIndex < 0)
                return;

            gridEquipZoneCCTV.Rows.Clear();
            cboZone.Items.Clear();

            Building building = (Building)cboBuilding.Items[cboBuilding.SelectedIndex];

            foreach (Zone zone in building.Zones)
            {
                cboZone.Items.Add(zone);
            }

            if (cboZone.Items.Count > 0)
                cboZone.SelectedIndex = 0;
        }

        private void cboZone_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboZone.SelectedIndex < 0)
                return;

            gridEquipZoneCCTV.Rows.Clear();

            Zone zone = (Zone)cboZone.Items[cboZone.SelectedIndex];
            List<EquipmentZone> equipZones = zone.EquipZones;

            string strEquipZoneIDs = "";

            foreach (EquipmentZone equipZone in equipZones)
            {
                if (strEquipZoneIDs.Length == 0)
                    strEquipZoneIDs = equipZone.ID.ToString();
                else
                    strEquipZoneIDs += ", " + equipZone.ID.ToString();
            }

            if (strEquipZoneIDs.Length > 0)
            {
                string strSQL = "Select EquipZoneID, CCTV1, CCTV2, CCTV3, CCTV4, CCTV5, CCTV6 from EquipZoneCCTV where EquipZoneID in (" + strEquipZoneIDs + ")";
                ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);

                if (arrResult == null)
                    return;

                int nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 6; i += 7)
                {
                    VariousData<int> equipZoneID = WebDBManager.GetIntField(arrResult[i].ToString());
                    VariousData<int> cctv1 = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                    VariousData<int> cctv2 = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                    VariousData<int> cctv3 = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                    VariousData<int> cctv4 = WebDBManager.GetIntField(arrResult[i + 4].ToString());
                    VariousData<int> cctv5 = WebDBManager.GetIntField(arrResult[i + 5].ToString());
                    VariousData<int> cctv6 = WebDBManager.GetIntField(arrResult[i + 6].ToString());

                    if (equipZoneID == null)
                        continue;

                    EquipmentZone equipZone = FormMain.Instance.DataManager.GetEquipmentZone(equipZoneID.Data);

                    if (equipZone == null)
                        continue;

                    AddGrid(equipZone, cctv1, cctv2, cctv3, cctv4, cctv5, cctv6);
                }
            }
        }

        private void AddGrid(EquipmentZone equipZone, VariousData<int> cctv1, VariousData<int> cctv2, VariousData<int> cctv3, VariousData<int> cctv4, VariousData<int> cctv5, VariousData<int> cctv6)
        {
            int nRowIndex = gridEquipZoneCCTV.Rows.Add();

            if (nRowIndex < 0)
                return;

            DataGridViewRow row = gridEquipZoneCCTV.Rows[nRowIndex];

            row.Cells[0].Value = equipZone.Name;
            SetCCTVCell(row.Cells[1], cctv1);
            SetCCTVCell(row.Cells[2], cctv2);
            SetCCTVCell(row.Cells[3], cctv3);
            SetCCTVCell(row.Cells[4], cctv4);
            SetCCTVCell(row.Cells[5], cctv5);
            SetCCTVCell(row.Cells[6], cctv6);

            row.Tag = equipZone;
        }

        private void SetCCTVCell(DataGridViewCell cell, VariousData<int> cctv)
        {
            cell.Value = cctv == null ? "" : cctv.Data.ToString();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            gridEquipZoneCCTV.ClearSelection();
            int nCCTVCount = m_use4CCTV ? 4 : 6;

            foreach (DataGridViewRow row in gridEquipZoneCCTV.Rows)
            {
                for (int i=1;i<=nCCTVCount;i++)
                {
                    if (CheckValidCCTV(row.Cells[i]) == false)
                        return;
                }
            }

            foreach (DataGridViewRow row in gridEquipZoneCCTV.Rows)
            {
                if (SaveEquipZoneCCTV(row) == false)
                {
                    MessageBox.Show("저장에 실패하였습니다.");
                    return;
                }
            }

            MessageBox.Show("데이터가 변경되었습니다.");
        }

        private bool SaveEquipZoneCCTV(DataGridViewRow row)
        {
            EquipmentZone equipZone = (EquipmentZone)row.Tag;

            string strCCTV1 = GetCCTVID(row.Cells[1]);
            string strCCTV2 = GetCCTVID(row.Cells[2]);
            string strCCTV3 = GetCCTVID(row.Cells[3]);
            string strCCTV4 = GetCCTVID(row.Cells[4]);
            string strCCTV5 = GetCCTVID(row.Cells[5]);
            string strCCTV6 = GetCCTVID(row.Cells[6]);

            string strSQL = "";

            if (m_use4CCTV)
            {
                strSQL = string.Format("Update EquipZoneCCTV set CCTV1 = {0}, CCTV2 = {1}, CCTV3 = {2}, CCTV4 = {3} where EquipZoneID = {4}",
                    strCCTV1, strCCTV2, strCCTV3, strCCTV4, equipZone.ID);
            }
            else
            {
                strSQL = string.Format("Update EquipZoneCCTV set CCTV1 = {0}, CCTV2 = {1}, CCTV3 = {2}, CCTV4 = {3}, CCTV5 = {4}, CCTV6 = {5} where EquipZoneID = {6}",
                    strCCTV1, strCCTV2, strCCTV3, strCCTV4, strCCTV5, strCCTV6, equipZone.ID);
            }

            return FormMain.Instance.DBManager.GetResultData(strSQL) != null;
        }

        private string GetCCTVID(DataGridViewCell cell)
        {
            string strCCTV = cell.Value == null ? "" : cell.Value.ToString().Trim();

            if (strCCTV.Length == 0)
                return "NULL";

            return "'" + strCCTV + "'";
        }

        private bool CheckValidCCTV(DataGridViewCell cell)
        {
            if (cell.Value == null)
                return true;

            string strCCTV = cell.Value.ToString().Trim();

            if (strCCTV.Length == 0)
                return true;

            int nCCTVID;

            if (int.TryParse(strCCTV, out nCCTVID) == false)
            {
                cell.Selected = true;
                MessageBox.Show(string.Format("{0}는 CCTV ID로 사용할 수 없습니다.\r\n올바른 CCTV ID를 입력하세요.", strCCTV));
                return false;
            }

            CCTV cctv = FormMain.Instance.DataManager.GetCCTV(nCCTVID);

            if (cctv == null)
            {
                cell.Selected = true;
                MessageBox.Show(string.Format("{0}는 존재하지 않는 CCTV ID입니다.\r\n올바른 CCTV ID를 입력하세요.", nCCTVID));
                return false;
            }

            return true;
        }

        private void gridEquipZoneCCTV_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                foreach (DataGridViewCell cell in gridEquipZoneCCTV.SelectedCells)
                {
                    cell.Value = null;
                }
            }
        }
    }
}
