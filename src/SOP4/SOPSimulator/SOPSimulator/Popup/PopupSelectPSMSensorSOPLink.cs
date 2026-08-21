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
using UnE.PSM;

namespace SOPMonitoringSystem.Popup
{
    public partial class PopupSelectPSMSensorSOPLink : Form
    {
        private class LinkedSOP
        {
            private int m_nID = -1;
            private string m_strSOPFullPath = "";
            private int m_nGridIndex = -1;
            private List<PSMLocationData> m_linkedLocations = new List<PSMLocationData>();

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

            public List<PSMLocationData> LinkedLocations
            {
                get { return m_linkedLocations; }
            }
        }

        private class PSMLocationData
        {
            private PSMMaterial m_material = null;
            private string m_strLocationName = "";
            private List<int> m_linkedEquipZoneIDs = new List<int>();
            private List<int> m_tankIDs = new List<int>();



            public PSMMaterial Material
            {
                get { return m_material; }
                set { m_material = value; }
            }

            public string LocationName
            {
                get
                {
                    if (m_material == null)
                        return m_strLocationName;

                    return m_strLocationName + "(" + m_material.Name + ")";
                }
                set { m_strLocationName = value; }
            }

            public string OriginLocationName
            {
                get { return m_strLocationName; }
            }

            public List<int> TankIDs
            {
                get { return m_tankIDs; }
            }

            public List<int> LinkedEquipZoneIDs
            {
                get { return m_linkedEquipZoneIDs; }
            }

            public PSMLocationData()
            {
            }

            public PSMLocationData(PSMMaterial material, string strLocationName)
            {
                m_material = material;
                m_strLocationName = strLocationName;
            }

            public override string ToString()
            {
                return LocationName;
            }
        }

        private int m_nSiteID = -1;
        private WebDBManager m_dbMgr = null;
        private Dictionary<int, PSMMaterial> m_dicPSMMaterials = new Dictionary<int, PSMMaterial>();
        // Key : PSMMaterial ID
        // Value : 유해화학물질 탱크들의 위치
        private Dictionary<int, List<PSMLocationData>> m_dicPSMMaterialLocations = new Dictionary<int, List<PSMLocationData>>();
        // Key : Tank ID
        private Dictionary<int, PSMLocationData> m_dicTankLocations = new Dictionary<int, PSMLocationData>();
        private Dictionary<PSMLocationData, LinkedSOP> m_dicLocationSOP = new Dictionary<PSMLocationData, LinkedSOP>();
        private List<LinkedSOP> m_removedSOPList = new List<LinkedSOP>();

        /*private BuildingGroup m_outdoorZoneBuildingGroup = new BuildingGroup();

        private Dictionary<Building, LinkedSOP> m_dicBuildingSOP = new Dictionary<Building, LinkedSOP>();
        private Dictionary<Zone, LinkedSOP> m_dicZoneSOP = new Dictionary<Zone, LinkedSOP>();*/

        private PopupTranslucentForm mTranslucentForm = new PopupTranslucentForm();

        public PopupSelectPSMSensorSOPLink(WebDBManager dbMgr, int nSiteID)
        {
            InitializeComponent();

            m_dbMgr = dbMgr;
            m_nSiteID = nSiteID;

            InitGrid();
        }

        private void InitGrid()
        {
            InitGridHeaders(gridSOP);
            InitGridHeaders(gridMaterial);
            InitGridHeaders(gridLocation);

            gridMaterial.Tag = gridLocation;
        }

        private void InitGridHeaders(DataGridView grid)
        {
            foreach (DataGridViewColumn column in grid.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private void PopupSelectPSMSensorSOPLink_Load(object sender, EventArgs e)
        {
            //LoadLinkedSOP();
            SetMaterials(gridMaterial);
        }

        private void LoadLinkedSOP()
        {
            string strSQL = "Select ID, SOPName, LinkedTankID FROM PSMSensorSOPLink where SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            PSMLocationData location = null;
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-2;i+=3)
            {
                DBUtility.VariousData<int> nID = WebDBManager.GetIntField(arrResult[i].ToString());
                string strSOPFullpath = WebDBManager.GetStringField(arrResult[i + 1]);
                string strTankIDs = WebDBManager.GetStringField(arrResult[i + 2]);

                if (nID == null || strSOPFullpath == null)
                    continue;

                LinkedSOP sop = new LinkedSOP();

                sop.ID = nID.Data;
                sop.SOPFullPath = strSOPFullpath;

                if (strTankIDs != null)
                {
                    List<int> tankIDs = ParseID(strTankIDs);

                    if (tankIDs != null)
                    {
                        foreach (int nTankID in tankIDs)
                        {
                            if (m_dicTankLocations.TryGetValue(nTankID, out location))
                                m_dicLocationSOP[location] = sop;
                        }
                    }
                }

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

        private void SetMaterials(DataGridView grid)
        {
            grid.Rows.Clear();

            string strSQL = "select tank.ID, tank.LocationName, tank.EquipZoneID, tank.MaterialType, mat.MaterialName";
            strSQL += " from PSMTank as tank, PSMMaterial as mat";
            strSQL += " where tank.MaterialType = mat.ID order by mat.ID";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            List<PSMLocationData> locations = null;
            PSMMaterial material = null;
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 4;i += 5 )
            {
                VariousData<int> tankID = WebDBManager.GetIntField(arrResult[i].ToString());
                string strLocationName = WebDBManager.GetStringField(arrResult[i + 1]);
                VariousData<int> equipZoneID = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<int> materialTypeID = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                string strMaterialName = WebDBManager.GetStringField(arrResult[i + 4]);

                if (tankID == null || strLocationName == null || equipZoneID == null || materialTypeID == null || strMaterialName == null)
                    continue;

                if (!m_dicPSMMaterials.TryGetValue(materialTypeID.Data, out material))
                {
                    material = new PSMMaterial();
                    material.ID = materialTypeID.Data;
                    material.Name = strMaterialName;

                    m_dicPSMMaterials[material.ID] = material;

                    DataGridViewRow row = new DataGridViewRow();
                    
                    DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                    cell.Value = material;
                    row.Cells.Add(cell);
                    row.Tag = material;

                    grid.Rows.Add(row);
                }

                if (!m_dicPSMMaterialLocations.TryGetValue(material.ID, out locations))
                {
                    locations = new List<PSMLocationData>();
                    m_dicPSMMaterialLocations[material.ID] = locations;
                }

                PSMLocationData _location = null;

                foreach (PSMLocationData location in locations)
                {
                    if (location.Material == material && location.OriginLocationName == strLocationName)
                    {
                        _location = location;
                        break;
                    }
                }

                if (_location == null)
                {
                    _location = new PSMLocationData(material, strLocationName);
                    locations.Add(_location);
                }

                _location.TankIDs.Add(tankID.Data);
                _location.LinkedEquipZoneIDs.Add(equipZoneID.Data);
                m_dicTankLocations[tankID.Data] = _location;
            }

            LoadLinkedSOP();

            if (grid.Rows.Count > 0)
            {
                grid.Rows[0].Cells[0].Selected = false;
                grid.Rows[0].Cells[0].Selected = true;
            }
        }

        private void SetMaterialLocation(DataGridView grid, PSMMaterial material, bool clearRows)
        {
            if (clearRows)
                grid.Rows.Clear();

            List<PSMLocationData> locations = null;

            if (!m_dicPSMMaterialLocations.TryGetValue(material.ID, out locations))
                return;

            LinkedSOP sop = null;

            foreach (PSMLocationData location in locations)
            {
                DataGridViewRow row = Popup.PopupStartEvent.MakeNewRow(grid);

                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.ReadOnly = true;
                }

                row.Cells[0].Value = row.Index + 1;
                row.Cells[1].Value = location;
                row.Cells[2].ReadOnly = false;

                if (m_dicLocationSOP.TryGetValue(location, out sop))
                    row.Cells[2].Value = sop.GridIndex;
            }

            grid.ClearSelection();
            /*if (grid.Rows.Count > 0)
                grid.Rows[0].Cells[0].Selected = true;*/
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

                if (cell.Value is PSMMaterial)
                {
                    PSMMaterial material = (PSMMaterial)cell.Value;
                    DataGridView grid2 = (DataGridView)grid.Tag;

                    SetMaterialLocation(grid2, material, isFirst);
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

                if (value is PSMMaterial)
                {
                    PSMMaterial material = (PSMMaterial)value;
                    List<PSMLocationData> locations = null;
                    
                    if (m_dicPSMMaterialLocations.TryGetValue(material.ID, out locations))
                    {
                        foreach (PSMLocationData location in locations)
                        {
                            m_dicLocationSOP.Remove(location);
                            sop.LinkedLocations.Remove(location);
                        }
                    }
                }
                else if (value is PSMLocationData)
                {
                    PSMLocationData location = (PSMLocationData)value;
                    m_dicLocationSOP.Remove(location);
                    sop.LinkedLocations.Remove(location);
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

                if (value is PSMMaterial)
                {
                    PSMMaterial material = (PSMMaterial)value;
                    List<PSMLocationData> locations = null;

                    if (m_dicPSMMaterialLocations.TryGetValue(material.ID, out locations))
                    {
                        foreach (PSMLocationData location in locations)
                        {
                            m_dicLocationSOP.Remove(location);
                            sopOld.LinkedLocations.Remove(location);
                        }
                    }
                }
                else if (value is PSMLocationData)
                {
                    PSMLocationData location = (PSMLocationData)value;
                    m_dicLocationSOP.Remove(location);
                    sopOld.LinkedLocations.Add(location);
                }
            }

            cell.Value = sop.GridIndex.ToString();
            cell.Tag = sop;

            if (value is PSMMaterial)
            {
                PSMMaterial material = (PSMMaterial)value;
                List<PSMLocationData> locations = null;

                if (m_dicPSMMaterialLocations.TryGetValue(material.ID, out locations))
                {
                    foreach (PSMLocationData location in locations)
                    {
                        m_dicLocationSOP[location] = sop;
                        sop.LinkedLocations.Add(location);
                    }
                }
            }
            else if (value is PSMLocationData)
            {
                PSMLocationData location = (PSMLocationData)value;
                m_dicLocationSOP[location] = sop;
                sop.LinkedLocations.Add(location);
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
                RefreshBuildingGrid(gridLocation);
        }

        private void RefreshBuildingGrid(DataGridView grid)
        {
            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.Cells[1].Value == null)
                    continue;

                LinkedSOP sop;
                PSMLocationData location = (PSMLocationData)row.Cells[1].Value;

                if (m_dicLocationSOP.TryGetValue(location, out sop))
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

            PageBackstageSOP home = FormSOP.Instance.GetPageHome();
            //home.ShowTranslucentForm(targetForm, x, y, width, height, nCommandID);




            if (targetForm == null)
                return;

            if (mTranslucentForm == null || mTranslucentForm.IsDisposed)
                mTranslucentForm = new PopupTranslucentForm();

            mTranslucentForm.Location = PageBackstageSOP.TranslucentForm.Location;
            mTranslucentForm.Size = PageBackstageSOP.TranslucentForm.Size;

            targetForm.ShowInTaskbar = false;
            if (mTranslucentForm.Visible == true)
            {
                mTranslucentForm.Detach();
            }

            targetForm.StartPosition = FormStartPosition.Manual;
            mTranslucentForm.AddContentForm(targetForm, x, y, targetForm.Size.Width, targetForm.Size.Height, FormSOP.Instance.GetPageHome());
            mTranslucentForm.Parent = FormSOP.Instance.GetPageHome();
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

            if (sop.LinkedLocations.Count > 0)
            {
                strMsg += string.Format("\r\n{0}개의 유해화학물질 영역에 대한 SOP 링크가 삭제됩니다.", sop.LinkedLocations.Count);
            }

            if (MessageBox.Show(this, strMsg, "확인", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                return;

            m_removedSOPList.Add(sop);

            // 연결된 SOP Link 없애기
            foreach (PSMLocationData location in sop.LinkedLocations)
            {
                m_dicLocationSOP.Remove(location);
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

            string strSQL = string.Format("Delete from PSMSensorSOPLink where ID in ({0})", strSOPIDList);
            m_dbMgr.GetResultData(strSQL, 0);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            FormSOP.Instance.GetPageHome().CloseTranslucentForm();
        }

        private bool UpdateSOP(LinkedSOP sop)
        {
            string strTankIDs;
            GetSOPIDList(sop, out strTankIDs);

            string strSQL = string.Format("Update PSMSensorSOPLink set SOPName = '{0}', LinkedTankID = {1} where ID = {2}",
                sop.SOPFullPath,
                strTankIDs == null ? "NULL" : "'" + strTankIDs + "'",
                sop.ID);

            return m_dbMgr.GetResultData(strSQL, 0) != null;
        }

        private bool InsertSOP(LinkedSOP sop, ref int nSOPLinkID)
        {
            if (nSOPLinkID < 0)
            {
                nSOPLinkID = GetMaxID();

                if (nSOPLinkID < 0)
                    return false;
            }

            string strTankIDs;
            GetSOPIDList(sop, out strTankIDs);

            string strSQL = string.Format("Insert into PSMSensorSOPLink (ID, SOPName, LinkedTankID, SiteID) values ({0}, '{1}', {2}, {3})",
                nSOPLinkID++, sop.SOPFullPath,
                strTankIDs == null ? "NULL" : "'" + strTankIDs + "'",
                m_nSiteID);

            return m_dbMgr.GetResultData(strSQL, 0) != null;
        }

        private int GetMaxID()
        {
            string strSQL = "Select max(ID) from PSMSensorSOPLink where SiteID = " + m_nSiteID.ToString();
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

        private void GetSOPIDList(LinkedSOP sop, out string strTankIDs)
        {
            List<int> linkedTankIDs = new List<int>();

            foreach (PSMLocationData location in sop.LinkedLocations)
            {
                foreach (int nTankID in location.TankIDs)
                {
                    linkedTankIDs.Add(nTankID);
                }
            }

            if (linkedTankIDs.Count > 0)
                strTankIDs = MakeIDListString(linkedTankIDs);
            else
                strTankIDs = null;
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
                SetCurrentSOP(gridLocation, sop);
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
