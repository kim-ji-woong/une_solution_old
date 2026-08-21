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
using DBUtility;

namespace SOPMonitoringSystem.Popup
{
    public partial class PopupSelectIntrusionSensorSOPLink : Form
    {
        private class SOPList
        {
            private int m_nID = -1;
            private string m_strSOPName = string.Empty;
            private int m_nGridIndex = -1;

            public int ID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }
            public string SOPName
            {
                get { return m_strSOPName; }
                set { m_strSOPName = value; }
            }
            public int GridIndex
            {
                get { return m_nGridIndex; }
                set { m_nGridIndex = value; }
            }
        }
        private class LinkedSOP
        {
            private int m_nID = -1;
            private int m_nSopID = -1;
            private string m_strSOPName = "";
            private int m_nGridIndex = -1;
            private List<Building> m_linkedBuildings = new List<Building>();
            private List<Zone> m_linkedZones = new List<Zone>();
            private int m_nSecurityFacilityType = -1;

            public int ID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }
            public int SopID
            {
                get { return m_nSopID; }
                set { m_nSopID = value; }
            }

            public string SOPName
            {
                get { return m_strSOPName; }
                set { m_strSOPName = value; }
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

            public int SecurityFacilityType
            {
                get { return m_nSecurityFacilityType; }
                set { m_nSecurityFacilityType = value; }
            }
        }

        private int m_nSiteID = -1;
        private WebDBManager m_dbMgr = null;
        private BuildingGroup m_outdoorZoneBuildingGroup = new BuildingGroup();
         
        private Dictionary<int, Dictionary<Building, LinkedSOP>> m_dicBuildingSOP = new Dictionary<int, Dictionary<Building, LinkedSOP>>();
        private Dictionary<int, Dictionary<Zone, LinkedSOP>> m_dicZoneSOP = new Dictionary<int, Dictionary<Zone, LinkedSOP>>();
        private List<LinkedSOP> m_removedLinkedSOPList = new List<LinkedSOP>();
        private List<SOPList> m_removedSOPList = new List<SOPList>();


        private PopupTranslucentForm mTranslucentForm = new PopupTranslucentForm();

        private int m_curSopGridIndex { get { return Convert.ToInt32(gridSOP.CurrentRow.Cells["colNo"].Value); } }
        private int m_curSopID { get { return Convert.ToInt32(gridSOP.CurrentRow.Cells["colSopID"].Value); } }
        private string m_curSopName { get { return gridSOP.CurrentRow.Cells["colSOPFullPath"].Value.ToString(); } }
        private int m_curSecurityID { get { return Convert.ToInt32(gridSecurityFacilityType.CurrentRow.Cells["colSecurityID"].Value); } }
        private int m_nTempSopID = -1; //DB에 등록되지 전 임시 SOP ID

        #region 초기화
        public PopupSelectIntrusionSensorSOPLink(WebDBManager dbMgr, int nSiteID)
        {
            InitializeComponent();

            tabControl1.TabPages.Remove(tabPageZoneSignal);

            m_outdoorZoneBuildingGroup.BuildingGroupName = m_outdoorZoneBuildingGroup.DisplayName = "외부영역";
            m_dbMgr = dbMgr;
            m_nSiteID = nSiteID;
            InitGrid();
        }

        private void InitGrid()
        {
            InitGridHeaders(gridSOP);
            InitGridHeaders(gridSecurityFacilityType);
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

        private void PopupSelectIntrusionSensorSOPLink_Load(object sender, EventArgs e)
        {
            LoadLinkedSOP();
            SetSecurityFacilityType(gridSecurityFacilityType);
            SetBuildingGroup(gridBuildingGroup);
            SetBuildingGroup(gridBuildingGroup2);
        }
        #endregion

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
                if (!m_dicBuildingSOP.ContainsKey(sop.SecurityFacilityType))
                    m_dicBuildingSOP.Add(sop.SecurityFacilityType, new Dictionary<Building, LinkedSOP>());
                m_dicBuildingSOP[sop.SecurityFacilityType][building] = sop;
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
                if (!m_dicZoneSOP.ContainsKey(sop.SecurityFacilityType))
                    m_dicZoneSOP.Add(sop.SecurityFacilityType, new Dictionary<Zone, LinkedSOP>());
                m_dicZoneSOP[sop.SecurityFacilityType][zone] = sop; 
            }
        }

        private List<int> ParseID(string strIDs)
        {
            // ID 중복을 막기 위하여 일단 Dictionary에 저장한다.
            Dictionary<int, int> dicIDs = new Dictionary<int, int>();
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

        #region Grid 바인딩
        private void SetSecurityFacilityType(DataGridView grid)
        {
            grid.Rows.Clear();

            foreach (KeyValuePair<int, SecurityFacilityType> pair in DataManager.Instance.DicSecurityFacilityType)
            {
                DataGridViewRow row = Popup.PopupStartEvent.MakeNewRow(grid);
                row.Cells[0].Value = pair.Value.ID;
                row.Cells[1].Value = pair.Value.SubCategoryName;
                row.Cells[1].ReadOnly = true;
            }

            if (grid.Rows.Count > 0)
            {
                grid.Rows[0].Cells[0].Selected = false;
                grid.Rows[0].Cells[0].Selected = true;
            }
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
                        row.Cells["colBuildingID"].Value = row.Index + 1;
                        row.Cells["colBuilding"].Value = pair.Value;
                        row.Cells["colLinkedSOP"].ReadOnly = false;

                        LinkedSOP sop;

                        if (m_dicZoneSOP.ContainsKey(m_curSecurityID))
                        {
                            if (m_dicZoneSOP[m_curSecurityID].TryGetValue(pair.Value, out sop))
                            {
                                if (sop.SecurityFacilityType == m_curSecurityID)
                                {
                                    row.Cells["colLinkedSOP"].Value = sop.GridIndex.ToString();
                                    row.Cells["colLinkedSOP"].Tag = sop;
                                }
                            }
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
                        row.Cells["colBuildingID"].Value = row.Index + 1;
                        row.Cells["colBuilding"].Value = building;
                        row.Cells["colLinkedSOP"].ReadOnly = false;

                        LinkedSOP sop;

                        if (m_dicBuildingSOP.ContainsKey(m_curSecurityID))
                        {
                            if (m_dicBuildingSOP[m_curSecurityID].TryGetValue(building, out sop))
                            {
                                if (sop.SecurityFacilityType == m_curSecurityID)
                                {
                                    row.Cells["colLinkedSOP"].Value = sop.GridIndex.ToString();
                                    row.Cells["colLinkedSOP"].Tag = sop;
                                }
                            }
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

                    if (m_dicZoneSOP.ContainsKey(m_curSecurityID))
                    {
                        if (m_dicZoneSOP[m_curSecurityID].TryGetValue(zone, out sop))
                        {
                            row.Cells[2].Value = sop.GridIndex.ToString();
                            row.Cells[2].Tag = sop;
                        }
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

                if (m_dicZoneSOP.ContainsKey(m_curSecurityID))
                {
                    if (m_dicZoneSOP[m_curSecurityID].TryGetValue(zone, out sop))
                    {
                        row.Cells[2].Value = sop.GridIndex.ToString();
                        row.Cells[2].Tag = sop;
                    }
                }
            }

            if (grid.Rows.Count > 0)
                grid.Rows[0].Cells[0].Selected = true;
        }
        #endregion

        #region Grid 이벤트
        private void grid_SelectionChanged(object sender, EventArgs e)
        {
            DataGridView grid = (DataGridView)sender;

            if (grid.SelectedCells.Count == 0)
                return;

            Dictionary<int, int> dicSelectedRowIndeces = new Dictionary<int, int>();

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

        private void gridSecurityFacilityType_SelectionChanged(object sender, EventArgs e)
        {
            DataGridView grid = gridBuildingGroup;

            if (grid.SelectedCells.Count == 0)
                return;

            Dictionary<int, int> dicSelectedRowIndeces = new Dictionary<int, int>();

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

        private void gridSOP_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
                if (gridBuilding.SelectedCells.Count <= 0) return;

                DataGridViewRow row = gridSOP.Rows[e.RowIndex];
                if (row.IsNewRow) return;

                SOPList sopList = (SOPList)row.Tag;
                if (dicSopList.ContainsKey(sopList.ID))
                {
                    if (dicSopList[sopList.ID] == null)
                        dicSopList[sopList.ID] = new Dictionary<int, LinkedSOP>();

                    if (dicSopList[sopList.ID].ContainsKey(m_curSecurityID))
                    {
                        SetCurrentSOP(gridBuilding, dicSopList[sopList.ID][m_curSecurityID]);
                    }
                    else
                    {
                        LinkedSOP linkedSop = new LinkedSOP();
                        linkedSop.GridIndex = sopList.GridIndex;
                        linkedSop.SopID = sopList.ID;
                        linkedSop.SOPName = sopList.SOPName;
                        dicSopList[sopList.ID].Add(m_curSecurityID, linkedSop);
                        SetCurrentSOP(gridBuilding, dicSopList[sopList.ID][m_curSecurityID]);
                    }
                } 
            }
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
        #endregion

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

        private LinkedSOP GetLinkedSOP(int nGridIndex)
        {
            foreach (DataGridViewRow row in gridSOP.Rows)
            {
                if (row.Tag != null && row.Tag is SOPList)
                {
                    SOPList sop = (SOPList)row.Tag;

                    if (sop.GridIndex == nGridIndex)
                    {
                        if (!dicSopList[sop.ID].ContainsKey(m_curSecurityID))
                        {
                            LinkedSOP linkedSop = new LinkedSOP();
                            linkedSop.GridIndex = sop.GridIndex;
                            linkedSop.SopID = sop.ID;
                            linkedSop.SOPName = sop.SOPName;
                            linkedSop.SecurityFacilityType = m_curSecurityID;
                            
                            dicSopList[sop.ID].Add(m_curSecurityID, linkedSop);
                        }

                        return dicSopList[sop.ID][m_curSecurityID];
                    }
                }
            }

            return null;
        }  

        #region 버튼 이벤트
        private void btnNewSOP_Click(object sender, EventArgs e)
        {
            PageBackstageSOP.QuickSOPButton sop = new PageBackstageSOP.QuickSOPButton();

            PopupSelectSOP form = new PopupSelectSOP();
            form.IsNormal = true;
            form.DisasterTypeID = ID.ID_SOP_SECURITY;
            form.QuickSOP = sop;
            form.SelectButtonClickEvent += (s, ex) => { AddNewSOP(sop); };

            ShowTranslucentForm(form, 400, -30, form.Width, form.Size.Height, ID.ID_SHOW_QUICK_MENU);
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
            form.DisasterTypeID = ID.ID_SOP_SECURITY;
            form.QuickSOP = sop;
            form.SelectButtonClickEvent += (s, ex) => { ChangeSOP(sop, row); };

            ShowTranslucentForm(form, 400, -30, form.Width, form.Size.Height, ID.ID_SHOW_QUICK_MENU);
        }

        private void btnDeleteSOP_Click(object sender, EventArgs e)
        {
            if (gridSOP.SelectedCells.Count == 0)
            {
                MessageBox.Show(this, "삭제할 SOP를 선택하세요");
                return;
            }

            DataGridViewRow row = gridSOP.SelectedCells[0].OwningRow;

            int LinkedBuildingCnt = 0;
            int LinkedZoneCnt = 0;
            SOPList sop = (SOPList)row.Tag;

            m_removedSOPList.Add(sop);

            foreach (KeyValuePair<int, LinkedSOP> item in dicSopList[sop.ID])
            {
                m_removedLinkedSOPList.Add(item.Value);
                LinkedBuildingCnt += item.Value.LinkedBuildings.Count;
                LinkedZoneCnt += item.Value.LinkedZones.Count;

                // 연결된 건물들의 SOP Link 없애기
                foreach (Building building in item.Value.LinkedBuildings)
                {
                    m_dicBuildingSOP[item.Value.SecurityFacilityType].Remove(building);
                }

                // 연결된 Zone들의 SOP Link 없애기
                foreach (Zone zone in item.Value.LinkedZones)
                {
                    m_dicZoneSOP[item.Value.SecurityFacilityType].Remove(zone);
                }
            }

            string strMsg = string.Format("[{0}]을 삭제하시겠습니까?", sop.SOPName);

            if (LinkedBuildingCnt > 0 && LinkedZoneCnt > 0)
            {
                strMsg += string.Format("\r\n{0}개의 건물과 {1}개의 영역의 SOP 링크가 삭제됩니다.", LinkedBuildingCnt, LinkedZoneCnt);
            }
            else if (LinkedBuildingCnt > 0)
            {
                strMsg += string.Format("\r\n{0}개의 건물과 연결된 SOP 링크가 삭제됩니다.", LinkedBuildingCnt);
            }
            else if (LinkedZoneCnt > 0)
            {
                strMsg += string.Format("\r\n{0}개의 영역과 연결된 SOP 링크가 삭제됩니다.", LinkedZoneCnt);
            }

            if (MessageBox.Show(this, strMsg, "확인", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                return;

            // row 이후 행들의 Grid Index 바꾸기
            for (int i = row.Index + 1; i < gridSOP.Rows.Count; i++)
            {
                DataGridViewRow row2 = gridSOP.Rows[i];
                row2.Cells[0].Value = i;

                SOPList sop2 = (SOPList)row2.Tag;
                sop2.GridIndex = i;
            }

            gridSOP.Rows.Remove(row);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in gridSOP.Rows)
            {
                SOPList sop = (SOPList)row.Tag;
                UpdateSOP(sop); 
            }

            DeleteSOP();
            FormSOP.Instance.GetPageHome().CloseTranslucentForm();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            FormSOP.Instance.GetPageHome().CloseTranslucentForm();
        }
        #endregion

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

        #region SOP 관련 함수
        Dictionary<int, Dictionary<int, LinkedSOP>> dicSopList = new Dictionary<int, Dictionary<int, LinkedSOP>>();
        private void LoadLinkedSOP()
        {
            m_dicBuildingSOP.Clear();
            m_dicZoneSOP.Clear();

            string strSQL = "SELECT ID, SOPNAME FROM securitysensorsoplist";
            ArrayList arrSopList = m_dbMgr.GetResultData(strSQL.ToString(), 0);
            if (arrSopList == null) return;

            for (int i = 0; i < arrSopList.Count; i += 2)
            {
                DBUtility.VariousData<int> nID = WebDBManager.GetIntField(arrSopList[i].ToString());
                string strSOPName = WebDBManager.GetStringField(arrSopList[i + 1]);

                SOPList sopList = new SOPList();
                sopList.ID = Convert.ToInt32(nID.Data);
                sopList.SOPName = strSOPName.ToString();

                DataGridViewRow row = Popup.PopupStartEvent.MakeNewRow(gridSOP);

                sopList.GridIndex = Convert.ToInt32(row.Index + 1);
                row.Cells["colNo"].Value = sopList.GridIndex;
                row.Cells["colSOPFullPath"].Value = sopList.SOPName;
                row.Cells["colSopID"].Value = sopList.ID;
                row.Tag = sopList;

                foreach (DataGridViewCell cell in row.Cells)
                    cell.ReadOnly = true;

                if (!dicSopList.ContainsKey(sopList.ID))
                    dicSopList.Add(sopList.ID, new Dictionary<int, LinkedSOP>());
            }

            strSQL = "SELECT ID, SOPID, LinkedBuildingID, LinkedZoneID, SecurityTypeID, " +
                     "       (select sopname from SecuritySensorSopList where id=link.sopid) as SopName " +
                     "  FROM SecuritySensorSopLink link WHERE SiteID=" + m_nSiteID;
            ArrayList arrSopLinkList = m_dbMgr.GetResultData(strSQL, 0);
            if (arrSopLinkList == null) return;

            for (int i = 0; i < arrSopLinkList.Count; i += 6)
            {
                DBUtility.VariousData<int> nID = WebDBManager.GetIntField(arrSopLinkList[i].ToString());
                DBUtility.VariousData<int> nSopID = WebDBManager.GetIntField(arrSopLinkList[i + 1].ToString());
                string strBuildingIDs = WebDBManager.GetStringField(arrSopLinkList[i + 2]);
                string strZoneIDs = WebDBManager.GetStringField(arrSopLinkList[i + 3]);
                int nSecurityTypeID = WebDBManager.GetIntField(arrSopLinkList[i + 4].ToString(), -1);
                string strSopName = WebDBManager.GetStringField(arrSopLinkList[i + 5]);

                if (nID == null || nSopID == null || strSopName == null) continue;

                LinkedSOP sop = new LinkedSOP();
                sop.ID = nID.Data;
                sop.SopID = nSopID.Data;
                sop.SOPName = strSopName;
                sop.SecurityFacilityType = nSecurityTypeID;

                foreach (DataGridViewRow row in gridSOP.Rows)
                {
                    if (Convert.ToInt32(row.Cells["colSopID"].Value) == nSopID.Data && row.Cells["colSOPFullPath"].Value.ToString() == sop.SOPName)
                        sop.GridIndex = Convert.ToInt32(row.Cells["colNo"].Value);
                }

                if (strBuildingIDs != null)
                    SetLinkedBuildings(sop, strBuildingIDs);

                if (strZoneIDs != null)
                    SetLinkedZones(sop, strZoneIDs);

                if (dicSopList[sop.SopID] == null)
                {
                    dicSopList[sop.SopID] = new Dictionary<int, LinkedSOP>();
                    dicSopList[sop.SopID].Add(sop.SecurityFacilityType, sop);
                }

                if (!dicSopList[sop.SopID].ContainsKey(sop.SecurityFacilityType))
                    dicSopList[sop.SopID].Add(sop.SecurityFacilityType, sop);
            }
        }

        private void AddNewSOP(PageBackstageSOP.QuickSOPButton sop)
        {
            string strSOPFullPath = RemoveActionStep(sop.SOPNormalPath);

            foreach (DataGridViewRow row in gridSOP.Rows)
            {
                if (row.Cells["colSOPFullPath"].Value.ToString() == strSOPFullPath) return;
            }

            DataGridViewRow row2 = Popup.PopupStartEvent.MakeNewRow(gridSOP);

            SOPList sopList = new SOPList();
            sopList.SOPName = strSOPFullPath;
            sopList.GridIndex = row2.Index + 1;
            sopList.ID = m_nTempSopID;
            m_nTempSopID--;

            row2.Cells["colNo"].Value = sopList.GridIndex;
            row2.Cells["colSOPFullPath"].Value = sopList.SOPName;
            row2.Tag = sopList;

            dicSopList.Add(sopList.ID, new Dictionary<int, LinkedSOP>()); 
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

                    m_dicBuildingSOP[sop.SecurityFacilityType].Remove(building);
                    sop.LinkedBuildings.Remove(building);
                }
                else if (value is Zone)
                {
                    Zone zone = (Zone)value;

                    m_dicZoneSOP[sop.SecurityFacilityType].Remove(zone);
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

                    m_dicBuildingSOP[sopOld.SecurityFacilityType].Remove(building);
                    sopOld.LinkedBuildings.Remove(building);
                }
                else if (value is Zone)
                {
                    Zone zone = (Zone)value;

                    m_dicZoneSOP[sopOld.SecurityFacilityType].Remove(zone);
                    sopOld.LinkedZones.Remove(zone);
                }
            }

            sop.SecurityFacilityType = m_curSecurityID;
            sop.SopID = m_curSopID;
            sop.SOPName = m_curSopName;
            sop.GridIndex = sop.GridIndex;

            if (value is Building)
            {
                Building building = (Building)value;

                if (!m_dicBuildingSOP.ContainsKey(sop.SecurityFacilityType)) 
                    m_dicBuildingSOP.Add(sop.SecurityFacilityType, new Dictionary<Building, LinkedSOP>());                     
                m_dicBuildingSOP[sop.SecurityFacilityType][building] = sop;
                sop.LinkedBuildings.Add(building);
            }
            else if (value is Zone)
            {
                Zone zone = (Zone)value;

                if (!m_dicZoneSOP.ContainsKey(sop.SecurityFacilityType))
                    m_dicZoneSOP.Add(sop.SecurityFacilityType, new Dictionary<Zone, LinkedSOP>());
                m_dicZoneSOP[sop.SecurityFacilityType][zone] = sop;
                m_dicZoneSOP[sop.SecurityFacilityType][zone] = sop;
                sop.LinkedZones.Add(zone);
            }

            cell.Value = sop.GridIndex.ToString();
            cell.Tag = sop;
        }

        private void ChangeSOP(PageBackstageSOP.QuickSOPButton sop, DataGridViewRow row)
        {
            string strSOPFullPath = RemoveActionStep(sop.SOPNormalPath);

            foreach (DataGridViewRow row2 in gridSOP.Rows)
            {
                if (row == row2) continue;

                // 변경하고자 하는 SOP가 이미 존재한다.
                if (row2.Cells["colSOPFullPath"].Value.ToString() == strSOPFullPath) return;
            }

            SOPList sop2 = (SOPList)row.Tag;
            sop2.SOPName = strSOPFullPath;
            row.Cells["colSOPFullPath"].Value = sop2.SOPName; 
        }

        private void DeleteSOP()
        {
            string strIDList = string.Empty;
            string strSopIDList = string.Empty;

            foreach (SOPList sop in m_removedSOPList)
            {
                if (strIDList.Length == 0)
                    strSopIDList = sop.ID.ToString();
                else
                    strSopIDList += ", " + sop.ID.ToString();
            }
            foreach (LinkedSOP sop in m_removedLinkedSOPList)
            {
                if (strIDList.Length == 0)
                    strIDList = sop.ID.ToString();
                else
                    strIDList += ", " + sop.ID.ToString();
            }

            if (strIDList.Length != 0)
                m_dbMgr.GetResultData(string.Format("Delete from securitysensorsoplink where ID in ({0})", strIDList), 0);

            if (strSopIDList.Length != 0)
                m_dbMgr.GetResultData(string.Format("Delete from securitysensorsoplist where ID in ({0})", strSopIDList), 0);
        } 

        private bool UpdateSOP(SOPList sop)
        {
            foreach (KeyValuePair<int, LinkedSOP> item in dicSopList[sop.ID])
            {
                if (item.Value.SecurityFacilityType < 0) continue;

                string strBuildingIDs, strZoneIDs;
                GetSOPIDList(item.Value, out strBuildingIDs, out strZoneIDs);

                string strSQL = string.Empty;

                strSQL = string.Format("SELECT count(*) FROM securitysensorsoplink WHERE SOPID={0} AND SecurityTypeID={1}", item.Value.SopID, item.Value.SecurityFacilityType);
                ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

                DBUtility.VariousData<int> nCount = WebDBManager.GetIntField(arrResult[0].ToString());
                if (nCount.Data > 0) //Update
                {
                    //SOP가 변경된 경우를 위해 UPDATE 처리
                    strSQL = string.Format("UPDATE securitysensorsoplist SET SOPName='{0}' WHERE ID={1}", sop.SOPName, sop.ID);
                    if (m_dbMgr.GetResultData(strSQL, 0) == null) return false;

                    strSQL = string.Format(
                        "UPDATE securitysensorsoplink SET LinkedBuildingID={0}, LinkedZoneID={1} " +
                        "WHERE SOPID={2} AND SecurityTypeID={3} AND SiteID={4}",
                            strBuildingIDs == null ? "NULL" : "'" + strBuildingIDs + "'",
                            strZoneIDs == null ? "NULL" : "'" + strZoneIDs + "'",
                            item.Value.SopID,
                            item.Value.SecurityFacilityType,
                            m_nSiteID);

                    if (m_dbMgr.GetResultData(strSQL, 0) == null) return false;
                }
                else //Insert
                {
                    if (sop.ID <= 0)
                    {
                        sop.ID = GetMaxSopID();
                        if (sop.ID < 0) return false;

                        strSQL = string.Format("INSERT INTO securitysensorsoplist (ID, SOPName) VALUES ({0}, '{1}')", sop.ID, sop.SOPName);
                        if (m_dbMgr.GetResultData(strSQL, 0) == null) return false;
                    }

                    int nSOPLinkID = GetMaxID();
                    if (nSOPLinkID < 0) return false;

                    strSQL = string.Format(
                        "INSERT INTO securitysensorsoplink (ID, SOPID, LinkedBuildingID, LinkedZoneID, SecurityTypeID, SiteID) " +
                        "VALUES ({0}, {1}, {2}, {3}, {4}, {5})",
                                nSOPLinkID++, sop.ID,
                                strBuildingIDs == null ? "NULL" : "'" + strBuildingIDs + "'",
                                strZoneIDs == null ? "NULL" : "'" + strZoneIDs + "'",
                                item.Value.SecurityFacilityType,
                                m_nSiteID);

                    if (m_dbMgr.GetResultData(strSQL, 0) == null) return false;
                }
            }

            return true;
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
        #endregion
        private int GetMaxSopID()
        {
            string strSQL = "Select max(ID) from securitysensorsoplist";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return -1;

            if (arrResult.Count == 0)
                return 1;

            DBUtility.VariousData<int> nID = WebDBManager.GetIntField(arrResult[0].ToString());

            if (nID == null)
                return 1;

            return nID.Data + 1;
        }
        private int GetMaxID()
        {
            string strSQL = "Select max(ID) from securitysensorsoplink where SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return -1;

            if (arrResult.Count == 0)
                return 1;

            DBUtility.VariousData<int> nID = WebDBManager.GetIntField(arrResult[0].ToString());

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

            for (int i = 1; i < nCount; i++)
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

        private void tsMenuSelectAll_Click(object sender, EventArgs e)
        {
            DataGridView grid = (DataGridView)contextMenuStrip1.Tag;
            grid.SelectAll();
        }
    }
}
