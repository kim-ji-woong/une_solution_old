using DBUtility2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.Sensor;

namespace SOPMonitoringSystem.Popup
{
    public partial class PopupSlectETCSensorSOPLink : Form
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

        // Form Move 를 위한 Panel Top 클릭 체크
        private bool m_bLeftMouseDown = false;
        // Form Move 를 위한 Panel Top 클릭 점
        private Point m_ptMove;

        private Font m_fontButton = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular);
        private Font m_fontSaveCloseButton = new System.Drawing.Font("나눔바른고딕", 15F, System.Drawing.FontStyle.Bold);

        private BuildingGroup m_outdoorZoneBuildingGroup = new BuildingGroup();
        private Dictionary<Zone, LinkedSOP> m_dicZoneSOP = new Dictionary<Zone, LinkedSOP>();
        private Dictionary<Building, LinkedSOP> m_dicBuildingSOP = new Dictionary<Building, LinkedSOP>();
        private List<LinkedSOP> m_removedSOPList = new List<LinkedSOP>();

        private Dictionary<IFacility.FacilityType, Dictionary<Building, LinkedSOP>> m_dicETCBuildingSOP = new Dictionary<IFacility.FacilityType, Dictionary<Building, LinkedSOP>>();
        private Dictionary<IFacility.FacilityType, List<LinkedSOP>> m_dicGridSOP = new Dictionary<IFacility.FacilityType, List<LinkedSOP>>();
        private Dictionary<IFacility.FacilityType, List<LinkedSOP>> m_dicRemovedSOPList = new Dictionary<IFacility.FacilityType, List<LinkedSOP>>();

        private Dictionary<string, string> m_dicUseETCSensorZone = new Dictionary<string, string>();

        // 선택한 센서타입
        private int m_nSelectSensorType = -1;

        public PopupSlectETCSensorSOPLink(WebDBManager dbMgr, int nSiteID)
        {
            InitializeComponent();

            m_dbMgr = dbMgr;
            m_nSiteID = nSiteID;

            InitGrid();
            SetRibbonButtonFont();
        }

        private void InitGrid()
        {
            gridBuildingGroup.Tag = gridBuilding;
            gridBuildingGroup2.Tag = gridBuilding2;
            gridBuilding2.Tag = gridZone;
        }

        public void SetRibbonButtonFont()
        {
            tabPageBuildingSignal.Font = m_fontButton;
            tabPageZoneSignal.Font = m_fontButton;
            ribbonButton.Font = m_fontButton;
            btnNewSOP.Font = m_fontButton;
            btnChangeSOP.Font = m_fontButton;
            btnDeleteSOP.Font = m_fontButton;

            btnSave.Font = m_fontSaveCloseButton;
            btnCancel.Font = m_fontSaveCloseButton;
        }

        private void plTitle_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = plTitle.PointToScreen(new Point(e.X, e.Y));
            }
        }

        private void plTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_bLeftMouseDown == true)
                {
                    Point ptCur = this.Location;

                    Point pt = this.PointToScreen(new Point(e.X, e.Y));
                    int dx = pt.X - m_ptMove.X;
                    int dy = pt.Y - m_ptMove.Y;
                    if (!(dx == 0 && dy == 0))
                    {

                        this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
                        m_ptMove.X += dx;
                        m_ptMove.Y += dy;
                    }
                }
            }
        }

        private void plTitle_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                m_bLeftMouseDown = false;
        }

        private void lbTitle_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = plTitle.PointToScreen(new Point(e.X, e.Y));
            }
        }

        private void lbTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_bLeftMouseDown == true)
                {
                    Point ptCur = this.Location;

                    Point pt = this.PointToScreen(new Point(e.X, e.Y));
                    int dx = pt.X - m_ptMove.X;
                    int dy = pt.Y - m_ptMove.Y;
                    if (!(dx == 0 && dy == 0))
                    {

                        this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
                        m_ptMove.X += dx;
                        m_ptMove.Y += dy;
                    }
                }
            }
        }

        private void lbTitle_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                m_bLeftMouseDown = false;
        }

        private void pbTitle_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = plTitle.PointToScreen(new Point(e.X, e.Y));
            }
        }

        private void pbTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_bLeftMouseDown == true)
                {
                    Point ptCur = this.Location;

                    Point pt = this.PointToScreen(new Point(e.X, e.Y));
                    int dx = pt.X - m_ptMove.X;
                    int dy = pt.Y - m_ptMove.Y;
                    if (!(dx == 0 && dy == 0))
                    {

                        this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
                        m_ptMove.X += dx;
                        m_ptMove.Y += dy;
                    }
                }
            }
        }

        private void pbTitle_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                m_bLeftMouseDown = false;
        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tabPageBuildingSignal_Click(object sender, EventArgs e)
        {
            if (tabPageBuildingSignal.IsChecked == false)
            {
                tabPageBuildingSignal.IsChecked = true;
                tabPageZoneSignal.IsChecked = false;
                tabPageBuildingSignal.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.tabFireSensor_Selected;
                tabPageZoneSignal.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.tabFireSensor_Normal;
                tabPageBuildingSignal.ForeColor = System.Drawing.Color.White;
                tabPageZoneSignal.ForeColor = System.Drawing.Color.Black;

                plPageBuildingSignal.Visible = true;
                plPageZoneSignal.Visible = false;

                //RefreshBuildingGrid(gridBuilding);
            }
        }

        private void tabPageZoneSignal_Click(object sender, EventArgs e)
        {
            if (tabPageZoneSignal.IsChecked == false)
            {
                tabPageZoneSignal.IsChecked = true;
                tabPageBuildingSignal.IsChecked = false;
                tabPageZoneSignal.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.tabFireSensor_Selected;
                tabPageBuildingSignal.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.tabFireSensor_Normal;
                tabPageZoneSignal.ForeColor = System.Drawing.Color.White;
                tabPageBuildingSignal.ForeColor = System.Drawing.Color.Black;

                plPageZoneSignal.Visible = true;
                plPageBuildingSignal.Visible = false;

                //RefreshZoneGrid(gridZone);
            }
        }

        private void btnNewSOP_Click(object sender, EventArgs e)
        {
            PageBackstageSOP.QuickSOPButton sop = new PageBackstageSOP.QuickSOPButton();

            PopupSelectSOP form = new PopupSelectSOP();
            form.IsNormal = true;
            form.DisasterTypeID = ID.ID_SOP_POLLUTION;
            form.QuickSOP = sop;
            form.SelectButtonClickEvent += (s, ex) => { AddNewSOP(sop); };

            //ShowTranslucentForm(form, 400, -30, form.Width, form.Size.Height, ID.ID_SHOW_QUICK_MENU);
            form.ShowDialog(this);
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

        private void SetSensorGroup(DataGridView grid)
        {
            grid.Rows.Clear();

            if (UnE.SOP.ProxySOP.Instance.UsePSM && m_nSiteID == 205)
            {
                DataGridViewRow row = Popup.PopupStartEvent.MakeNewRow(grid);
                row.Cells[0].Value = IFacility.GetFacilityTypeString(IFacility.FacilityType.PSM_SENSOR);
                row.Cells[0].ReadOnly = true;
                row.Tag = IFacility.FacilityType.PSM_SENSOR;
            }

            if (UnE.SOP.ProxySOP.Instance.UseBlackout)
            {
                DataGridViewRow row = Popup.PopupStartEvent.MakeNewRow(grid);
                row.Cells[0].Value = IFacility.GetFacilityTypeString(IFacility.FacilityType.BLACKOUT);
                row.Cells[0].ReadOnly = true;
                row.Tag = IFacility.FacilityType.BLACKOUT;
            }

            if (UnE.SOP.ProxySOP.Instance.UseStrongWind)
            {
                DataGridViewRow row = Popup.PopupStartEvent.MakeNewRow(grid);
                row.Cells[0].Value = IFacility.GetFacilityTypeString(IFacility.FacilityType.STRONG_WIND);
                row.Cells[0].ReadOnly = true;
                row.Tag = IFacility.FacilityType.STRONG_WIND;
            }

            if (UnE.SOP.ProxySOP.Instance.UseEarthquake)
            {
                DataGridViewRow row = Popup.PopupStartEvent.MakeNewRow(grid);
                row.Cells[0].Value = IFacility.GetFacilityTypeString(IFacility.FacilityType.Earthquake);
                row.Cells[0].ReadOnly = true;
                row.Tag = IFacility.FacilityType.Earthquake;
            }

            if (UnE.SOP.ProxySOP.Instance.UseTerror)
            {
                DataGridViewRow row = Popup.PopupStartEvent.MakeNewRow(grid);
                row.Cells[0].Value = IFacility.GetFacilityTypeString(IFacility.FacilityType.TERROR);
                row.Cells[0].ReadOnly = true;
                row.Tag = IFacility.FacilityType.TERROR;
            }

            if (UnE.SOP.ProxySOP.Instance.UseSubmergency)
            {
                DataGridViewRow row = Popup.PopupStartEvent.MakeNewRow(grid);
                row.Cells[0].Value = IFacility.GetFacilityTypeString(IFacility.FacilityType.SUBMERGENCY);
                row.Cells[0].ReadOnly = true;
                row.Tag = IFacility.FacilityType.SUBMERGENCY;
            }

            if (UnE.SOP.ProxySOP.Instance.UseCorona)
            {
                DataGridViewRow row = Popup.PopupStartEvent.MakeNewRow(grid);
                row.Cells[0].Value = IFacility.GetFacilityTypeString(IFacility.FacilityType.CORONA);
                row.Cells[0].ReadOnly = true;
                row.Tag = IFacility.FacilityType.CORONA;
            }

            if (grid.Rows.Count > 0)
            {
                grid.Rows[0].Cells[0].Selected = false;
                grid.Rows[0].Cells[0].Selected = true;
                m_nSelectSensorType = (int)grid.Rows[0].Tag;
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

            /*if (DataManager.Instance.DicOutdoorZones.Count > 0)
            {
                DataGridViewRow row = Popup.PopupStartEvent.MakeNewRow(grid);
                row.Cells[0].Value = m_outdoorZoneBuildingGroup;
                row.Cells[0].ReadOnly = true;
            }*/

            if (grid.Rows.Count > 0)
            {
                grid.Rows[0].Cells[0].Selected = false;
                grid.Rows[0].Cells[0].Selected = true;
            }
        }

        private void PopupSlectETCSensorSOPLink_Load(object sender, EventArgs e)
        {
            LoadUseETCSensorZone();
            LoadFacilityTypeLinkedSOP();
            SetSensorGroup(gridSensor);
            SetSensorGroup(gridSensor2);
            SetBuildingGroup(gridBuildingGroup);
            SetBuildingGroup(gridBuildingGroup2);
        }

        private void LoadUseETCSensorZone()
        {
            string strSQL = "Select PropertyValue FROM OptionSOPSimulator where PropertyName = 'LoadUseETCSensorZone' AND SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;
            string strUseETCSensorZone = "";

            for (int i = 0; i < nResultCount; i++)
            {
                strUseETCSensorZone = WebDBManager.GetStringField(arrResult[i]);
            }

            string[] tokens = strUseETCSensorZone.Split(',');

            foreach (string strToken in tokens)
            {
                string strSensor = strToken.Trim();

                m_dicUseETCSensorZone[strSensor] = strSensor;
            }
        }

        private void LoadFacilityTypeLinkedSOP()
        {
            if (UnE.SOP.ProxySOP.Instance.UsePSM && m_nSiteID == 205)
            {
                LoadLinkedSOP(IFacility.FacilityType.PSM_SENSOR);
            }

            if (UnE.SOP.ProxySOP.Instance.UseBlackout)
            {
                LoadLinkedSOP(IFacility.FacilityType.BLACKOUT);
            }

            if (UnE.SOP.ProxySOP.Instance.UseStrongWind)
            {
                LoadLinkedSOP(IFacility.FacilityType.STRONG_WIND);
            }

            if (UnE.SOP.ProxySOP.Instance.UseEarthquake)
            {
                LoadLinkedSOP(IFacility.FacilityType.Earthquake);
            }

            if (UnE.SOP.ProxySOP.Instance.UseTerror)
            {
                LoadLinkedSOP(IFacility.FacilityType.TERROR);
            }

            if (UnE.SOP.ProxySOP.Instance.UseSubmergency)
            {
                LoadLinkedSOP(IFacility.FacilityType.SUBMERGENCY);
            }

            if (UnE.SOP.ProxySOP.Instance.UseCorona)
            {
                LoadLinkedSOP(IFacility.FacilityType.CORONA);
            }
        }

        private void LoadLinkedSOP()
        {
            m_dicBuildingSOP.Clear();
            m_dicZoneSOP.Clear();

            gridSOP.Rows.Clear();

            string strSQL = "Select ID, SOPName, LinkedBuildingID, LinkedZoneID FROM ETCSensorSOPLink where SiteID = " + m_nSiteID.ToString() + " AND Type = " + m_nSelectSensorType;
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 3; i += 4)
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

        private void LoadLinkedSOP(IFacility.FacilityType type)
        {
            m_dicBuildingSOP.Clear();
            m_dicZoneSOP.Clear();

            int nType = (int)type;

            string strSQL = "Select ID, SOPName, LinkedBuildingID, LinkedZoneID FROM ETCSensorSOPLink where SiteID = " + m_nSiteID.ToString() + " AND Type = " + nType;
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;
            int nIndex = 0;

            List<LinkedSOP> linkedSOPs = new List<LinkedSOP>();

            for (int i = 0; i < nResultCount - 3; i += 4)
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
                    SetLinkedBuildings(sop, strBuildingIDs, type);

                // .TODO : Zone 해당 데이터 구현 필요
                /*
                if (strZoneIDs != null)
                    SetLinkedZones(sop, strZoneIDs);
                */

                nIndex++;
                sop.GridIndex = nIndex;
                linkedSOPs.Add(sop);
            }

            m_dicGridSOP[type] = linkedSOPs;
        }

        private void SetLinkedBuildings(LinkedSOP sop, string strBuildingIDs, IFacility.FacilityType type)
        {
            if (strBuildingIDs == null)
                return;

            List<int> ids = ParseID(strBuildingIDs);
            Dictionary<Building, LinkedSOP> dicBuildingSOP = new Dictionary<Building, LinkedSOP>();

            if (ids == null)
                return;

            if (m_dicETCBuildingSOP.ContainsKey(type))
                dicBuildingSOP = m_dicETCBuildingSOP[type];

            foreach (int nBuildingID in ids)
            {
                Building building = DataManager.Instance.GetBuilding(nBuildingID);

                if (building == null)
                    continue;

                sop.LinkedBuildings.Add(building);
                dicBuildingSOP[building] = sop;
            }

            m_dicETCBuildingSOP[type] = dicBuildingSOP;
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

        private void grid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridView grid = (DataGridView)sender;
            /*
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                contextMenuStrip1.Tag = grid;
                Rectangle rect = grid.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                this.contextMenuStrip1.Show(grid, rect.X + e.X, rect.Y + e.Y);
            }
            */
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

            // BuildingCode로 Zone 검색
            ArrayList zones = DataManager.Instance.GetZoneList(building.BuildingCode);

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

            //ShowTranslucentForm(form, 400, -30, form.Width, form.Size.Height, ID.ID_SHOW_QUICK_MENU);
            form.ShowDialog(this);
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
            for (int i = row.Index + 1; i < gridSOP.Rows.Count; i++)
            {
                DataGridViewRow row2 = gridSOP.Rows[i];
                row2.Cells[0].Value = i;

                LinkedSOP sop2 = (LinkedSOP)row2.Tag;
                sop2.GridIndex = i;
            }

            gridSOP.Rows.Remove(row);
        }

        private bool SaveFacilityType(IFacility.FacilityType type)
        {
            int nSOPLinkID = -1;
            m_dicBuildingSOP = new Dictionary<Building, LinkedSOP>();
            m_removedSOPList = new List<LinkedSOP>();
            List<LinkedSOP> linkedSOPs = new List<LinkedSOP>();

            if (m_dicETCBuildingSOP.ContainsKey(type))
                m_dicBuildingSOP = m_dicETCBuildingSOP[type];
            if (m_dicRemovedSOPList.ContainsKey(type))
                m_removedSOPList = m_dicRemovedSOPList[type];
            if (m_dicGridSOP.ContainsKey(type))
                linkedSOPs = m_dicGridSOP[type];

            foreach (LinkedSOP sop in linkedSOPs)
            {

                if (sop.ID < 0)
                {
                    if (!InsertSOP(sop, ref nSOPLinkID, type))
                        return false;
                }
                else
                {
                    if (!UpdateSOP(sop))
                        return false;
                }
            }

            DeleteSOP();

            return true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // 현재 정보 저장
            List<LinkedSOP> linkedSOPs = new List<LinkedSOP>();
            m_dicETCBuildingSOP[(IFacility.FacilityType)m_nSelectSensorType] = m_dicBuildingSOP;
            m_dicRemovedSOPList[(IFacility.FacilityType)m_nSelectSensorType] = m_removedSOPList;

            foreach (DataGridViewRow gridRow in gridSOP.Rows)
            {
                linkedSOPs.Add((LinkedSOP)gridRow.Tag);
            }

            m_dicGridSOP[(IFacility.FacilityType)m_nSelectSensorType] = linkedSOPs;

            if (UnE.SOP.ProxySOP.Instance.UsePSM && m_nSiteID == 205)
            {
                SaveFacilityType(IFacility.FacilityType.PSM_SENSOR);
            }

            if (UnE.SOP.ProxySOP.Instance.UseBlackout)
            {
                SaveFacilityType(IFacility.FacilityType.BLACKOUT);
            }

            if (UnE.SOP.ProxySOP.Instance.UseStrongWind)
            {
                SaveFacilityType(IFacility.FacilityType.STRONG_WIND);
            }

            if (UnE.SOP.ProxySOP.Instance.UseEarthquake)
            {
                SaveFacilityType(IFacility.FacilityType.Earthquake);
            }

            if (UnE.SOP.ProxySOP.Instance.UseTerror)
            {
                SaveFacilityType(IFacility.FacilityType.TERROR);
            }

            if (UnE.SOP.ProxySOP.Instance.UseSubmergency)
            {
                SaveFacilityType(IFacility.FacilityType.SUBMERGENCY);
            }
            
            if (UnE.SOP.ProxySOP.Instance.UseCorona)
            {
                SaveFacilityType(IFacility.FacilityType.CORONA);
            }

            /*
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
            */

            //FormSOP.Instance.GetPageHome().CloseTranslucentForm();
            this.Close();
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

            string strSQL = string.Format("Delete from ETCSensorSOPLink where ID in ({0})", strSOPIDList);
            m_dbMgr.GetResultData(strSQL);
        }

        private bool UpdateSOP(LinkedSOP sop)
        {
            string strBuildingIDs, strZoneIDs;
            GetSOPIDList(sop, out strBuildingIDs, out strZoneIDs);

            string strSQL = string.Format("Update ETCSensorSOPLink set SOPName = '{0}', LinkedBuildingID = {1}, LinkedZoneID = {2} where ID = {3}",
                sop.SOPFullPath,
                strBuildingIDs == null ? "NULL" : "'" + strBuildingIDs + "'",
                strZoneIDs == null ? "NULL" : "'" + strZoneIDs + "'",
                sop.ID);

            return m_dbMgr.GetResultData(strSQL) != null;
        }

        private bool InsertSOP(LinkedSOP sop, ref int nSOPLinkID, IFacility.FacilityType type)
        {
            if (nSOPLinkID < 0)
            {
                nSOPLinkID = GetMaxID();

                if (nSOPLinkID < 0)
                    return false;
            }

            int nType = (int)type;
            string strBuildingIDs, strZoneIDs;
            GetSOPIDList(sop, out strBuildingIDs, out strZoneIDs);

            string strSQL = string.Format("Insert into ETCSensorSOPLink (ID, Type, SOPName, LinkedBuildingID, LinkedZoneID, SiteID) values ({0}, {1}, '{2}', {3}, {4}, {5})",
                nSOPLinkID++, nType, sop.SOPFullPath,
                strBuildingIDs == null ? "NULL" : "'" + strBuildingIDs + "'",
                strZoneIDs == null ? "NULL" : "'" + strZoneIDs + "'",
                m_nSiteID);

            return m_dbMgr.GetResultData(strSQL) != null;
        }

        private int GetMaxID()
        {
            string strSQL = "Select max(ID) from ETCSensorSOPLink where SiteID = " + m_nSiteID.ToString();
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

        private void gridSensor_SelectionChanged(object sender, EventArgs e)
        {
            DataGridView grid = (DataGridView)sender;

            if (grid.SelectedCells.Count == 0)
                return;

            Dictionary<int, int> dicSelectedRowIndeces = new Dictionary<int, int>();

            foreach (DataGridViewCell cell in grid.SelectedCells)
            {
                dicSelectedRowIndeces[cell.RowIndex] = cell.RowIndex;
            }

            foreach (KeyValuePair<int, int> pair in dicSelectedRowIndeces)
            {
                DataGridViewRow row = grid.Rows[pair.Value];

                if (row.Tag != null)
                {
                    string strType = row.Tag.ToString();

                    // Zone 사용유무 확인
                    if (m_dicUseETCSensorZone.ContainsKey(strType))
                    {
                        tabPageZoneSignal.Visible = true;
                    }
                    else
                    {
                        tabPageZoneSignal.Visible = false;
                    }

                    List<LinkedSOP> linkedSOPs = new List<LinkedSOP>();

                    // 전역변수 저장 (처음 제외)
                    if (m_nSelectSensorType != -1)
                    {
                        m_dicETCBuildingSOP[(IFacility.FacilityType)m_nSelectSensorType] = m_dicBuildingSOP;
                        m_dicRemovedSOPList[(IFacility.FacilityType)m_nSelectSensorType] = m_removedSOPList;

                        foreach (DataGridViewRow gridRow in gridSOP.Rows)
                        {
                            linkedSOPs.Add((LinkedSOP)gridRow.Tag);
                        }

                        m_dicGridSOP[(IFacility.FacilityType)m_nSelectSensorType] = linkedSOPs;
                    }

                    // 전역변수 전환
                    m_nSelectSensorType = (int)row.Tag;
                    m_dicBuildingSOP = new Dictionary<Building, LinkedSOP>();
                    m_removedSOPList = new List<LinkedSOP>();
                    linkedSOPs = new List<LinkedSOP>(); ;

                    if (m_dicETCBuildingSOP.ContainsKey((IFacility.FacilityType)m_nSelectSensorType))
                        m_dicBuildingSOP = m_dicETCBuildingSOP[(IFacility.FacilityType)m_nSelectSensorType];

                    if (m_dicRemovedSOPList.ContainsKey((IFacility.FacilityType)m_nSelectSensorType))
                        m_removedSOPList = m_dicRemovedSOPList[(IFacility.FacilityType)m_nSelectSensorType];

                    if (m_dicGridSOP.ContainsKey((IFacility.FacilityType)m_nSelectSensorType))
                        linkedSOPs = m_dicGridSOP[(IFacility.FacilityType)m_nSelectSensorType];


                    // 그리드 표시
                    gridSOP.Rows.Clear();

                    foreach (LinkedSOP sop in linkedSOPs)
                    {
                        DataGridViewRow gridSOPRow = Popup.PopupStartEvent.MakeNewRow(gridSOP);

                        gridSOPRow.Cells[0].Value = sop.GridIndex;
                        gridSOPRow.Cells[1].Value = sop.SOPFullPath;
                        gridSOPRow.Tag = sop;
                    }

                    if (gridBuildingGroup.Rows.Count > 0)
                    {
                        for (int i = 0; gridBuildingGroup.Rows.Count > i; i++)
                        {
                            gridBuildingGroup.Rows[i].Selected = false;
                        }

                        gridBuildingGroup.Rows[0].Selected = true;
                    }
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
            //if (tabControl1.SelectedTab == tabPageBuildingSignal)
            if (tabPageBuildingSignal.IsChecked == true)
                SetCurrentSOP(gridBuilding, sop);
            //else if (tabControl1.SelectedTab == tabPageZoneSignal)
            else if (tabPageZoneSignal.IsChecked == true)
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
    }
}
