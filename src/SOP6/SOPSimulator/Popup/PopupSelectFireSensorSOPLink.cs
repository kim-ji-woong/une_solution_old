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

namespace SOPMonitoringSystem.Popup
{
    public partial class PopupSelectFireSensorSOPLink : Form
    {
        private class LinkedSOP
        {
            private int m_nID = -1;
            private string m_strSOPFullPath = "";
            private int m_nGridIndex = -1;
            private List<Building> m_linkedBuildings = new List<Building>();
            private List<Zone> m_linkedZones = new List<Zone>();

            public int ID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }

            public string SOPFullPath
            {
                get { return m_strSOPFullPath; }
                set { m_strSOPFullPath = value; }
            }

            public int GridIndex
            {
                get { return m_nGridIndex; }
                set { m_nGridIndex = value; }
            }

            public List<Building> LinkedBuildings
            {
                get { return m_linkedBuildings; }
            }

            public List<Zone> LinkedZones
            {
                get { return m_linkedZones; }
            }
        }

        private int m_nSiteID = -1;
        private WebDBManager m_dbMgr = null;
        private BuildingGroup m_outdoorZoneBuildingGroup = new BuildingGroup();

        private Dictionary<Building, LinkedSOP> m_dicBuildingSOP = new Dictionary<Building, LinkedSOP>();
        private Dictionary<Zone, LinkedSOP> m_dicZoneSOP = new Dictionary<Zone, LinkedSOP>();
        private List<LinkedSOP> m_removedSOPList = new List<LinkedSOP>();

        private PopupTranslucentForm mTranslucentForm = new PopupTranslucentForm();

        public PopupSelectFireSensorSOPLink(WebDBManager dbMgr, int nSiteID)
        {
            InitializeComponent();

            m_outdoorZoneBuildingGroup.BuildingGroupName = m_outdoorZoneBuildingGroup.DisplayName = "외부영역";
            m_dbMgr = dbMgr;
            m_nSiteID = nSiteID;

            InitGrid();
        }

        private void InitGrid()
        {
            InitGridHeaders(gridSOP);
            InitGridHeaders(gridBuildingGroup);
            InitGridHeaders(gridBuilding);
            InitGridHeaders(gridBuildingGroup2);
            InitGridHeaders(gridBuilding2);
            InitGridHeaders(gridZone);

            gridBuildingGroup.Tag = gridBuilding;
            gridBuildingGroup2.Tag = gridBuilding2;
            gridBuilding2.Tag = gridZone;
        }

        private void InitGridHeaders(DataGridView grid)
        {
            foreach (DataGridViewColumn column in grid.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private void PopupSelectFireSensorSOPLink_Load(object sender, EventArgs e)
        {
            LoadLinkedSOP();
            SetBuildingGroup(gridBuildingGroup);
            SetBuildingGroup(gridBuildingGroup2);
        }

        private void LoadLinkedSOP()
        {
            m_dicBuildingSOP.Clear();
            m_dicZoneSOP.Clear();

            string strSQL = "Select ID, SOPName, LinkedBuildingID, LinkedZoneID FROM FireSensorSOPLink where SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-3;i+=4)
            {
                VariousData<int> nID = WebDBManager.GetIntField(arrResult[i].ToString());
                string strSOPFullpath = WebDBManager.GetStringField(arrResult[i + 1]);
                string strBuildingIDs = WebDBManager.GetStringField(arrResult[i + 2]);
                string strZoneIDs = WebDBManager.GetStringField(arrResult[i + 3]);

                if (nID == null || strSOPFullpath == null)
                    continue;

                LinkedSOP sop = new LinkedSOP();

                sop.ID = nID.Data;
                sop.SOPFullPath = strSOPFullpath;

                if (strBuildingIDs != null)
                    SetLinkedBuildings(sop, strBuildingIDs);

                if (strZoneIDs != null)
                    SetLinkedZones(sop, strZoneIDs);

                DataGridViewRow row = Popup.PopupStartEvent.MakeNewRow(gridSOP);

                sop.GridIndex = row.Index + 1;
                row.Cells[0].Value = sop.GridIndex;
                row.Cells[1].Value = sop.SOPFullPath;
                row.Tag = sop;

                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.ReadOnly = true;
                }
            }
        }

        private void SetLinkedBuildings(LinkedSOP sop, string strBuildingIDs)
        {
            if (strBuildingIDs == null)
                return;

            List<int> ids = ParseID(strBuildingIDs);

            if (ids == null)
                return;

            foreach (int nBuildingID in ids)
            {
                Building building = DataManager.Instance.GetBuilding(nBuildingID);

                if (building == null)
                    continue;

                sop.LinkedBuildings.Add(building);
                m_dicBuildingSOP[building] = sop;
            }
        }

        private void SetLinkedZones(LinkedSOP sop, string strZoneIDs)
        {
            if (strZoneIDs == null)
                return;

            List<int> ids = ParseID(strZoneIDs);

            if (ids == null)
                return;

            foreach (int nZoneID in ids)
            {
                Zone zone = DataManager.Instance.GetZone(nZoneID);

                if (zone == null)
                    continue;

                sop.LinkedZones.Add(zone);
                m_dicZoneSOP[zone] = sop;
            }
        }

        private List<int> ParseID(string strIDs)
        {
            // ID 중복을 막기 위하여 일단 Dictionary에 저장한다.
            Dictionary<int, int> dicIDs = new Dictionary<int,int>();
            string[] tokens = strIDs.Split(',');

            int nID;

            foreach (string strToken in tokens)
            {
                string token = strToken.Trim();

                string[] tokens2 = token.Split('-');

                if (tokens2.Count() == 1)
                {
                    if (!int.TryParse(token, out nID) || nID <= 0)
                        return null;
                    else
                        dicIDs[nID] = nID;
                }
                else if (tokens2.Count() == 2)
                {
                    int nBeginID, nEndID;

                    if (!int.TryParse(tokens2[0].Trim(), out nID) || nID <= 0)
                        return null;
                    else
                        nBeginID = nID;

                    if (!int.TryParse(tokens2[1].Trim(), out nID) || nID <= 0)
                        return null;
                    else
                        nEndID = nID;

                    if (nBeginID > nEndID)
                    {
                        int temp = nBeginID;
                        nBeginID = nEndID;
                        nEndID = temp;
                    }

                    for (int i = nBeginID; i <= nEndID; i++)
                    {
                        dicIDs[i] = i;
                    }
                }
                else
                    return null;
            }

            List<int> ids = new List<int>();

            foreach (KeyValuePair<int, int> pair in dicIDs)
            {
                ids.Add(pair.Value);
            }

            return ids;
        }

        private void SetBuildingGroup(DataGridView grid)
        {
            grid.Rows.Clear();

            foreach (KeyValuePair<int, BuildingGroup> pair in DataManager.Instance.DicBuildingGroup)
            {
                DataGridViewRow row = Popup.PopupStartEvent.MakeNewRow(grid);
                row.Cells[0].Value = pair.Value;
                row.Cells[0].ReadOnly = true;
            }

            if (DataManager.Instance.DicOutdoorZones.Count > 0)
            {
                DataGridViewRow row = Popup.PopupStartEvent.MakeNewRow(grid);
                row.Cells[0].Value = m_outdoorZoneBuildingGroup;
                row.Cells[0].ReadOnly = true;
            }

            if (grid.Rows.Count > 0)
            {
                grid.Rows[0].Cells[0].Selected = false;
                grid.Rows[0].Cells[0].Selected = true;
            }
        }

        private void SetBuilding(DataGridView grid, BuildingGroup buildingGroup, bool clearRows)
        {
            if (clearRows)
                grid.Rows.Clear();

            if (buildingGroup == m_outdoorZoneBuildingGroup)
            {
                foreach (KeyValuePair<int, Zone> pair in DataManager.Instance.DicOutdoorZones)
                {
                    if (pair.Value.ID < 0)
                        continue;

                    DataGridViewRow row = Popup.PopupStartEvent.MakeNewRow(grid);

                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        cell.ReadOnly = true;
                    }

                    if (row.Cells.Count == 1)
                        row.Cells[0].Value = pair.Value;
                    else if (row.Cells.Count == 3)
                    {
                        row.Cells[0].Value = row.Index + 1;
                        row.Cells[1].Value = pair.Value;
                        row.Cells[2].ReadOnly = false;

                        LinkedSOP sop;

                        if (m_dicZoneSOP.TryGetValue(pair.Value, out sop))
                        {
                            row.Cells[2].Value = sop.GridIndex.ToString();
                            row.Cells[2].Tag = sop;
                        }
                    }
                }
            }
            else
            {
                foreach (Building building in buildingGroup.BuildingList)
                {
                    DataGridViewRow row = Popup.PopupStartEvent.MakeNewRow(grid);

                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        cell.ReadOnly = true;
                    }

                    if (row.Cells.Count == 1)
                        row.Cells[0].Value = building;
                    else if (row.Cells.Count == 3)
                    {
                        row.Cells[0].Value = row.Index + 1;
                        row.Cells[1].Value = building;
                        row.Cells[2].ReadOnly = false;

                        LinkedSOP sop;

                        if (m_dicBuildingSOP.TryGetValue(building, out sop))
                        {
                            row.Cells[2].Value = sop.GridIndex.ToString();
                            row.Cells[2].Tag = sop;
                        }
                    }
                }
            }

            if (grid.Rows.Count > 0)
            {
                if (gridZone.Visible)
                    grid.Rows[0].Cells[0].Selected = true;
                else
                    grid.ClearSelection();
            }
        }

        private void SetZone(DataGridView grid, Building building, bool clearRows)
        {
            if (clearRows)
                grid.Rows.Clear();

            ArrayList zones = DataManager.Instance.GetZoneList(building.BuildingID);

            if (zones == null)
                return;

            foreach (Zone zone in zones)
            {
                DataGridViewRow row = Popup.PopupStartEvent.MakeNewRow(grid);

                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.ReadOnly = true;
                }

                if (row.Cells.Count == 1)
                    row.Cells[0].Value = zone;
                else if (row.Cells.Count == 3)
                {
                    row.Cells[0].Value = row.Index + 1;
                    row.Cells[1].Value = zone;
                    row.Cells[2].ReadOnly = false;

                    LinkedSOP sop;

                    if (m_dicZoneSOP.TryGetValue(zone, out sop))
                    {
                        row.Cells[2].Value = sop.GridIndex.ToString();
                        row.Cells[2].Tag = sop;
                    }
                }
            }

            grid.ClearSelection();
            /*if (grid.Rows.Count > 0)
                grid.Rows[0].Cells[0].Selected = true;*/
        }

        private void SetZone(DataGridView grid, Zone zone, bool clearRows)
        {
            if (clearRows)
                grid.Rows.Clear();

            DataGridViewRow row = Popup.PopupStartEvent.MakeNewRow(grid);

            foreach (DataGridViewCell cell in row.Cells)
            {
                cell.ReadOnly = true;
            }

            if (row.Cells.Count == 1)
                row.Cells[0].Value = zone;
            else if (row.Cells.Count == 3)
            {
                row.Cells[0].Value = row.Index + 1;
                row.Cells[1].Value = zone;
                row.Cells[2].ReadOnly = false;

                LinkedSOP sop;

                if (m_dicZoneSOP.TryGetValue(zone, out sop))
                {
                    row.Cells[2].Value = sop.GridIndex.ToString();
                    row.Cells[2].Tag = sop;
                }
            }
        
            if (grid.Rows.Count > 0)
                grid.Rows[0].Cells[0].Selected = true;
        }

        private void grid_SelectionChanged(object sender, EventArgs e)
        {
            DataGridView grid = (DataGridView)sender;

            if (grid.SelectedCells.Count == 0)
                return;

            Dictionary<int, int> dicSelectedRowIndeces = new Dictionary<int,int>();

            foreach (DataGridViewCell cell in grid.SelectedCells)
            {
                dicSelectedRowIndeces[cell.RowIndex] = cell.RowIndex;
            }

            bool isFirst = true;

            foreach (KeyValuePair<int, int> pair in dicSelectedRowIndeces)
            {
                DataGridViewRow row = grid.Rows[pair.Value];

                if (row.IsNewRow)
                    continue;

                DataGridViewCell cell = row.Cells[0];

                if (cell.Value is BuildingGroup)
                {
                    BuildingGroup buildingGroup = (BuildingGroup)cell.Value;
                    DataGridView grid2 = (DataGridView)grid.Tag;

                    SetBuilding(grid2, buildingGroup, isFirst);
                    isFirst = false;
                }
                else if (cell.Value is Building && grid.Tag != null)
                {
                    Building building = (Building)cell.Value;
                    DataGridView grid2 = (DataGridView)grid.Tag;

                    SetZone(grid2, building, isFirst);
                    isFirst = false;
                }
                else if (cell.Value is Zone && grid.Tag != null)
                {
                    Zone zone = (Zone)cell.Value;
                    DataGridView grid2 = (DataGridView)grid.Tag;

                    SetZone(grid2, zone, isFirst);
                    isFirst = false;
                }
            }
        }

        private void grid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView grid = (DataGridView)sender;

            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex == 2)
            {
                object value = grid.Rows[e.RowIndex].Cells[1].Value;
                DataGridViewCell cell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];

                if (cell.Value == null)
                    NULLSOP(value, cell);
                else
                {
                    string strValue = cell.Value.ToString().Trim();

                    if (strValue.Length == 0)
                        NULLSOP(value, cell);
                    else
                    {
                        int nIndex;

                        if (int.TryParse(strValue, out nIndex))
                        {
                            LinkedSOP sop = GetLinkedSOP(nIndex);

                            if (sop == null)
                            {
                                MessageBox.Show(this, nIndex.ToString() + "는 존재하지 않는 SOP 번호입니다.\r\n다시 확인하세요");
                                ResetCell(value, cell);
                            }
                            else
                            {
                                SetSOP(value, cell, sop);
                            }
                        }
                        else
                        {
                            MessageBox.Show(this, strValue + "는 숫자가 아닙니다.\r\n올바른 SOP 번호를 입력하세요");
                            ResetCell(value, cell);
                        }
                    }
                }
            }
        }

        private void ResetCell(object value, DataGridViewCell cell)
        {
            if (cell.Tag != null)
            {
                LinkedSOP sop = (LinkedSOP)cell.Tag;
                cell.Value = sop.GridIndex.ToString();
            }
            else
                NULLSOP(value, cell);
        }

        private void NULLSOP(object value, DataGridViewCell cell)
        {
            if (value == null)
                return;

            if (cell.Tag != null)
            {
                LinkedSOP sop = (LinkedSOP)cell.Tag;

                if (value is Building)
                {
                    Building building = (Building)value;

                    m_dicBuildingSOP.Remove(building);
                    sop.LinkedBuildings.Remove(building);
                }
                else if (value is Zone)
                {
                    Zone zone = (Zone)value;

                    m_dicZoneSOP.Remove(zone);
                    sop.LinkedZones.Remove(zone);
                }
            }

            cell.Value = null;
            cell.Tag = null;
        }

        private void SetSOP(object value, DataGridViewCell cell, LinkedSOP sop)
        {
            if (value == null)
                return;

            if (cell.Tag != null)
            {
                LinkedSOP sopOld = (LinkedSOP)cell.Tag;

                if (value is Building)
                {
                    Building building = (Building)value;

                    m_dicBuildingSOP.Remove(building);
                    sopOld.LinkedBuildings.Remove(building);
                }
                else if (value is Zone)
                {
                    Zone zone = (Zone)value;

                    m_dicZoneSOP.Remove(zone);
                    sopOld.LinkedZones.Remove(zone);
                }
            }

            cell.Value = sop.GridIndex.ToString();
            cell.Tag = sop;

            if (value is Building)
            {
                Building building = (Building)value;

                m_dicBuildingSOP[building] = sop;
                sop.LinkedBuildings.Add(building);
            }
            else if (value is Zone)
            {
                Zone zone = (Zone)value;

                m_dicZoneSOP[zone] = sop;
                sop.LinkedZones.Add(zone);
            }
        }

        private LinkedSOP GetLinkedSOP(int nGridIndex)
        {
            foreach (DataGridViewRow row in gridSOP.Rows)
            {
                if (row.Tag != null && row.Tag is LinkedSOP)
                {
                    LinkedSOP sop = (LinkedSOP)row.Tag;

                    if (sop.GridIndex == nGridIndex)
                        return sop;
                }
            }

            return null;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == null)
                return;

            if (tabControl1.SelectedTab == tabPageBuildingSignal)
                RefreshBuildingGrid(gridBuilding);
            else if (tabControl1.SelectedTab == tabPageZoneSignal)
                RefreshZoneGrid(gridZone);
        }

        private void RefreshBuildingGrid(DataGridView grid)
        {
            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.Cells[1].Value == null)
                    continue;

                LinkedSOP sop;

                object obj = row.Cells[1].Value;
                if( obj is Building)
                {
                    Building building = (Building)row.Cells[1].Value;

                    if (m_dicBuildingSOP.TryGetValue(building, out sop))
                    {
                        row.Cells[2].Value = sop.GridIndex.ToString();
                        row.Cells[2].Tag = sop;
                    }
                    else
                    {
                        row.Cells[2].Value = null;
                        row.Cells[2].Tag = null;
                    }
                }
                else
                {
                    row.Cells[2].Value = null;
                    row.Cells[2].Tag = null;
                }
                
            }
        }

        private void RefreshZoneGrid(DataGridView grid)
        {
            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.Cells[1].Value == null)
                    continue;

                LinkedSOP sop;
                Zone zone = (Zone)row.Cells[1].Value;

                if (m_dicZoneSOP.TryGetValue(zone, out sop))
                {
                    row.Cells[2].Value = sop.GridIndex.ToString();
                    row.Cells[2].Tag = sop;
                }
                else
                {
                    row.Cells[2].Value = null;
                    row.Cells[2].Tag = null;
                }
            }
        }

        private void btnNewSOP_Click(object sender, EventArgs e)
        {
            PageBackstageSOP.QuickSOPButton sop = new PageBackstageSOP.QuickSOPButton();

            PopupSelectSOP form = new PopupSelectSOP();
            form.IsNormal = true;
            form.DisasterTypeID = ID.ID_SOP_FIRE;
            form.QuickSOP = sop;
            form.SelectButtonClickEvent += (s, ex) => { AddNewSOP(sop); };

            ShowTranslucentForm(form, 400, -30, form.Width, form.Size.Height, ID.ID_SHOW_QUICK_MENU);
        }

        private void AddNewSOP(PageBackstageSOP.QuickSOPButton sop)
        {
            string strSOPFullPath = RemoveActionStep(sop.SOPNormalPath);

            foreach (DataGridViewRow row in gridSOP.Rows)
            {
                if (row.Cells[1].Value.ToString() == strSOPFullPath)
                    return;
            }

            LinkedSOP sop2 = new LinkedSOP();
            DataGridViewRow row2 = Popup.PopupStartEvent.MakeNewRow(gridSOP);

            sop2.SOPFullPath = strSOPFullPath;
            sop2.GridIndex = row2.Index + 1;

            row2.Cells[0].Value = sop2.GridIndex;
            row2.Cells[1].Value = sop2.SOPFullPath;
            row2.Tag = sop2;
        }

        private string RemoveActionStep(string strFullPath)
        {
            int nIndex1 = strFullPath.IndexOf('/');

            if (nIndex1 < 0)
                return strFullPath;

            int nIndex2 = strFullPath.IndexOf('/', nIndex1 + 1);

            if (nIndex2 < 0)
                return strFullPath;

            int nIndex3 = strFullPath.IndexOf('/', nIndex2 + 1);

            if (nIndex3 < 0)
                return strFullPath;

            // 세번째 '/' 뒤는 ActionStep 이름이다.
            return strFullPath.Substring(0, nIndex3);
        }

        public void ShowTranslucentForm(Form targetForm, int x, int y, int width, int height, int nCommandID)
        {
            if (targetForm == null)
                return;

            if (mTranslucentForm == null || mTranslucentForm.IsDisposed)
                mTranslucentForm = new PopupTranslucentForm();

            targetForm.ShowInTaskbar = false;
            if (mTranslucentForm.Visible == true)
            {
                mTranslucentForm.Detach();
            }

            targetForm.StartPosition = FormStartPosition.Manual;
            mTranslucentForm.AddContentForm(targetForm, x, y, targetForm.Size.Width, targetForm.Size.Height, this);
            mTranslucentForm.Parent = this;
            mTranslucentForm.ShowInTaskbar = false;
            mTranslucentForm.Show(this);
        }

        private void btnChangeSOP_Click(object sender, EventArgs e)
        {
            if (gridSOP.SelectedCells.Count == 0)
            {
                MessageBox.Show(this, "변경할 SOP를 선택하세요");
                return;
            }

            DataGridViewRow row = gridSOP.SelectedCells[0].OwningRow;

            PageBackstageSOP.QuickSOPButton sop = new PageBackstageSOP.QuickSOPButton();

            PopupSelectSOP form = new PopupSelectSOP();
            form.IsNormal = true;
            form.DisasterTypeID = ID.ID_SOP_FIRE;
            form.QuickSOP = sop;
            form.SelectButtonClickEvent += (s, ex) => { ChangeSOP(sop, row); };

            ShowTranslucentForm(form, 400, -30, form.Width, form.Size.Height, ID.ID_SHOW_QUICK_MENU);
        }

        private void ChangeSOP(PageBackstageSOP.QuickSOPButton sop, DataGridViewRow row)
        {
            string strSOPFullPath = RemoveActionStep(sop.SOPNormalPath);

            foreach (DataGridViewRow row2 in gridSOP.Rows)
            {
                if (row == row2)
                    continue;

                if (row2.Cells[1].Value.ToString() == strSOPFullPath)
                {
                    // 변경하고자 하는 SOP가 이미 존재한다.
                    return;
                }
            }

            LinkedSOP sop2 = (LinkedSOP)row.Tag;
            sop2.SOPFullPath = strSOPFullPath;

            row.Cells[1].Value = sop2.SOPFullPath;
        }

        private void btnDeleteSOP_Click(object sender, EventArgs e)
        {
            if (gridSOP.SelectedCells.Count == 0)
            {
                MessageBox.Show(this, "삭제할 SOP를 선택하세요");
                return;
            }

            DataGridViewRow row = gridSOP.SelectedCells[0].OwningRow;
            LinkedSOP sop = (LinkedSOP)row.Tag;

            string strMsg = string.Format("[{0}]을 삭제하시겠습니까?", sop.SOPFullPath);

            if (sop.LinkedBuildings.Count > 0 && sop.LinkedZones.Count > 0)
            {
                strMsg += string.Format("\r\n{0}개의 건물과 {1}개의 영역의 SOP 링크가 삭제됩니다.", sop.LinkedBuildings.Count, sop.LinkedZones.Count);
            }
            else if (sop.LinkedBuildings.Count > 0)
            {
                strMsg += string.Format("\r\n{0}개의 건물과 연결된 SOP 링크가 삭제됩니다.", sop.LinkedBuildings.Count);
            }
            else if (sop.LinkedZones.Count > 0)
            {
                strMsg += string.Format("\r\n{0}개의 영역과 연결된 SOP 링크가 삭제됩니다.", sop.LinkedZones.Count);
            }

            if (MessageBox.Show(this, strMsg, "확인", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                return;

            m_removedSOPList.Add(sop);

            // 연결된 건물들의 SOP Link 없애기
            foreach (Building building in sop.LinkedBuildings)
            {
                m_dicBuildingSOP.Remove(building);
            }

            // 연결된 Zone들의 SOP Link 없애기
            foreach (Zone zone in sop.LinkedZones)
            {
                m_dicZoneSOP.Remove(zone);
            }

            // row 이후 행들의 Grid Index 바꾸기
            for (int i=row.Index+1;i<gridSOP.Rows.Count;i++)
            {
                DataGridViewRow row2 = gridSOP.Rows[i];
                row2.Cells[0].Value = i;

                LinkedSOP sop2 = (LinkedSOP)row2.Tag;
                sop2.GridIndex = i;
            }

            gridSOP.Rows.Remove(row);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int nSOPLinkID = -1;

            foreach (DataGridViewRow row in gridSOP.Rows)
            {
                LinkedSOP sop = (LinkedSOP)row.Tag;

                if (sop.ID < 0)
                {
                    if (!InsertSOP(sop, ref nSOPLinkID))
                        return;
                }
                else
                {
                    if (!UpdateSOP(sop))
                        return;
                }
            }

            DeleteSOP();
            FormSOP.Instance.GetPageHome().CloseTranslucentForm();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            FormSOP.Instance.GetPageHome().CloseTranslucentForm();
        }

        private void DeleteSOP()
        {
            string strSOPIDList = "";

            foreach (LinkedSOP sop in m_removedSOPList)
            {
                if (strSOPIDList.Length == 0)
                    strSOPIDList = sop.ID.ToString();
                else
                    strSOPIDList += ", " + sop.ID.ToString();
            }

            if (strSOPIDList.Length == 0)
                return;

            string strSQL = string.Format("Delete from FireSensorSOPLink where ID in ({0})", strSOPIDList);
            m_dbMgr.GetResultData(strSQL);
        }

        private bool UpdateSOP(LinkedSOP sop)
        {
            string strBuildingIDs, strZoneIDs;
            GetSOPIDList(sop, out strBuildingIDs, out strZoneIDs);

            string strSQL = string.Format("Update FireSensorSOPLink set SOPName = '{0}', LinkedBuildingID = {1}, LinkedZoneID = {2} where ID = {3}",
                sop.SOPFullPath,
                strBuildingIDs == null ? "NULL" : "'" + strBuildingIDs + "'",
                strZoneIDs == null ? "NULL" : "'" + strZoneIDs + "'",
                sop.ID);

            return m_dbMgr.GetResultData(strSQL) != null;
        }

        private bool InsertSOP(LinkedSOP sop, ref int nSOPLinkID)
        {
            if (nSOPLinkID < 0)
            {
                nSOPLinkID = GetMaxID();

                if (nSOPLinkID < 0)
                    return false;
            }

            string strBuildingIDs, strZoneIDs;
            GetSOPIDList(sop, out strBuildingIDs, out strZoneIDs);

            string strSQL = string.Format("Insert into FireSensorSOPLink (ID, SOPName, LinkedBuildingID, LinkedZoneID, SiteID) values ({0}, '{1}', {2}, {3}, {4})",
                nSOPLinkID++, sop.SOPFullPath,
                strBuildingIDs == null ? "NULL" : "'" + strBuildingIDs + "'",
                strZoneIDs == null ? "NULL" : "'" + strZoneIDs + "'",
                m_nSiteID);

            return m_dbMgr.GetResultData(strSQL) != null;
        }

        private int GetMaxID()
        {
            string strSQL = "Select max(ID) from FireSensorSOPLink where SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return -1;

            if (arrResult.Count == 0)
                return 1;

            VariousData<int> nID = WebDBManager.GetIntField(arrResult[0].ToString());

            if (nID == null)
                return 1;

            return nID.Data + 1;
        }

        private void GetSOPIDList(LinkedSOP sop, out string strBuildingIDs, out string strZoneIDs)
        {
            List<int> linkedBuildngIDs = new List<int>();
            List<int> linkedZoneIDs = new List<int>();

            foreach (Building building in sop.LinkedBuildings)
            {
                linkedBuildngIDs.Add(building.ID);
            }

            foreach (Zone zone in sop.LinkedZones)
            {
                linkedZoneIDs.Add(zone.ID);
            }

            if (linkedBuildngIDs.Count > 0)
                strBuildingIDs = MakeIDListString(linkedBuildngIDs);
            else
                strBuildingIDs = null;

            if (linkedZoneIDs.Count > 0)
                strZoneIDs = MakeIDListString(linkedZoneIDs);
            else
                strZoneIDs = null;
        }

        private string MakeIDListString(List<int> ids)
        {
            ids.Sort();

            string str = ids[0].ToString();

            int prev = ids[0];
            int nCount = ids.Count;
            int nBegin = prev, nEnd = -1;

            for (int i=1;i<nCount;i++)
            {
                int id = ids[i];

                if (id == prev)
                    continue;

                if (id == prev + 1)
                    nEnd = id;
                else
                {
                    if (nEnd >= 0)
                        AddString(ref str, nBegin, nEnd);

                    nBegin = id;
                    nEnd = -1;

                    str += "," + nBegin.ToString();
                }

                prev = id;
            }

            if (nEnd >= 0)
                AddString(ref str, nBegin, nEnd);

            return str;
        }

        private void AddString(ref string str, int nBegin, int nEnd)
        {
            // 세번 이상 숫자가 연속될때만 '-' 기호를 사용한다.
            if (nEnd > nBegin + 1)
                str += "-" + nEnd.ToString();
            else
                str += "," + nEnd.ToString();
        }

        private void gridSOP_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0)
                    return;

                DataGridViewRow row = gridSOP.Rows[e.RowIndex];

                if (row.IsNewRow)
                    return;

                LinkedSOP sop = (LinkedSOP)row.Tag;
                SetCurrentSOP(sop);
            }
        }

        private void SetCurrentSOP(LinkedSOP sop)
        {
            if (tabControl1.SelectedTab == tabPageBuildingSignal)
                SetCurrentSOP(gridBuilding, sop);
            else if (tabControl1.SelectedTab == tabPageZoneSignal)
                SetCurrentSOP(gridZone, sop);
        }

        private void SetCurrentSOP(DataGridView grid, LinkedSOP sop)
        {
            Dictionary<int, int> dicSelectedRowIndeces = new Dictionary<int, int>();

            foreach (DataGridViewCell cell in grid.SelectedCells)
            {
                dicSelectedRowIndeces[cell.RowIndex] = cell.RowIndex;
            }

            foreach (KeyValuePair<int, int> pair in dicSelectedRowIndeces)
            {
                DataGridViewRow row = grid.Rows[pair.Value];

                if (row.IsNewRow)
                    continue;

                SetSOP(row.Cells[grid.ColumnCount - 2].Value, row.Cells[grid.ColumnCount - 1], sop);
            }
        }

        private void tsMenuSelectAll_Click(object sender, EventArgs e)
        {
            DataGridView grid = (DataGridView)contextMenuStrip1.Tag;
            grid.SelectAll();
        }

        private void grid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridView grid = (DataGridView)sender;

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                contextMenuStrip1.Tag = grid;
                Rectangle rect = grid.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                this.contextMenuStrip1.Show(grid, rect.X + e.X, rect.Y + e.Y);
            }
        }
    }
}
