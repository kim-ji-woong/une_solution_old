using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility;

namespace ControlTeamEditor
{
    public partial class FormWorkSchedule2 : Form, IMainForm
    {
        private DataManager m_dataMgr = null;
        private WebDBManager m_dbMgr = null;

        private FormSearchMember m_frmSearch = null;
        private FormSearchExternalMember m_frmSearchExternal = null;

        private bool m_closeApplication = false;

        private static FormWorkSchedule2 m_instance = null;

        private DataControlRoomType m_controlRoomType = null;
        private DataControlRoomType m_fireRoomType = null;
        private DataControlRoomType m_dutyRoomType = null;

        private Dictionary<ComboBoxDataGridView, Label> m_dicGridViewLabel = new Dictionary<ComboBoxDataGridView, Label>();
        private List<ComboBoxDataGridView> m_gridControlRooms = new List<ComboBoxDataGridView>();
        private List<ComboBoxDataGridView> m_gridFireCenterRooms = new List<ComboBoxDataGridView>();

        //private Color CURRENT_WORKING_TEAM_BACK_COLOR = Color.FromArgb(255, 255, 0);
        private Color CURRENT_WORKING_TEAM_BACK_COLOR = Color.FromArgb(146, 208, 80);
        //private Color CURRENT_WORKING_TEAM_BACK_COLOR = Color.FromArgb(155, 194, 230);

        private bool m_systemCall = false;
        private bool m_dataChanged = false;
        
        public bool CloseApplication
        {
            get { return m_closeApplication; }
        }

        public static FormWorkSchedule2 Instance
        {
            get { return m_instance; }
        }

        public FormWorkSchedule2(int nSiteID)
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            FormMain.SetDoubleBuffer(this.dataGridControlRoomScheduleA, true);
            FormMain.SetDoubleBuffer(this.dataGridControlRoomScheduleB, true);
            FormMain.SetDoubleBuffer(this.dataGridControlRoomScheduleC, true);
            FormMain.SetDoubleBuffer(this.dataGridControlRoomScheduleD, true);
            FormMain.SetDoubleBuffer(this.dataGridFireCenterRoomScheduleA, true);
            FormMain.SetDoubleBuffer(this.dataGridFireCenterRoomScheduleB, true);
            FormMain.SetDoubleBuffer(this.dataGridFireCenterRoomScheduleC, true);


            Init(nSiteID);
        }

        private void Init(int nSiteID)
        {
            m_dbMgr = new WebDBManager(nSiteID);
            m_dataMgr = new DataManager(m_dbMgr, nSiteID);

            m_frmSearch = new FormSearchMember(m_dataMgr, this);
            m_frmSearchExternal = new FormSearchExternalMember(m_dataMgr, this);

            m_frmSearch.CompanyMemberSelected += m_frmSearch_MemberSelected;
            m_frmSearchExternal.ExternalMemberSelected += m_frmSearch_MemberSelected;
        }

        private void m_frmSearch_MemberSelected(object sender, EventArgs e)
        {
            SaveData(sender as DataGridViewCell);
        }

        private void SetControlRoom()
        {
            m_gridControlRooms.Add(dataGridControlRoomScheduleA);
            m_gridControlRooms.Add(dataGridControlRoomScheduleB);
            m_gridControlRooms.Add(dataGridControlRoomScheduleC);
            m_gridControlRooms.Add(dataGridControlRoomScheduleD);

            m_dicGridViewLabel[dataGridControlRoomScheduleA] = labelMemberInfoA;
            m_dicGridViewLabel[dataGridControlRoomScheduleB] = labelMemberInfoB;
            m_dicGridViewLabel[dataGridControlRoomScheduleC] = labelMemberInfoC;
            m_dicGridViewLabel[dataGridControlRoomScheduleD] = labelMemberInfoD;

            dataGridControlRoomScheduleA.RowHeadersWidth = dataGridControlRoomScheduleB.RowHeadersWidth = dataGridControlRoomScheduleC.RowHeadersWidth = dataGridControlRoomScheduleD.RowHeadersWidth = 200;
            labelMemberInfoA.Text = labelMemberInfoB.Text = labelMemberInfoC.Text = labelMemberInfoD.Text = "";

            this.m_controlRoomType = m_dataMgr.GetControlRoomType("제어실");

            if (m_controlRoomType == null)
            {
                Dictionary<int, DataControlRoomType> dicRoomTypes = m_dataMgr.GetControlRoomTypes();

                if (dicRoomTypes == null || dicRoomTypes.Count == 0)
                    return;

                m_controlRoomType = dicRoomTypes.ElementAt(0).Value;
            }

            if (m_controlRoomType != null && m_controlRoomType.RoomType.Length > 0)
            {
                labelControlRoomA.Text = m_controlRoomType.RoomType + " A조";
                labelControlRoomB.Text = m_controlRoomType.RoomType + " B조";
                labelControlRoomC.Text = m_controlRoomType.RoomType + " C조";
                labelControlRoomD.Text = m_controlRoomType.RoomType + " D조";
            }

            List<DataControlRoom> rooms = m_dataMgr.GetControlRooms(m_controlRoomType);

            if (rooms == null || rooms.Count == 0)
                return;

            List<DataControlTeamJobPosition> positions = m_dataMgr.GetJobPositions(m_controlRoomType);

            if (positions == null || positions.Count == 0)
                return;

            List<DataControlTeam> teams = m_dataMgr.GetControlTeams(m_controlRoomType);

            if (teams == null)
                return;

            int nTeamCount = teams.Count;
            int nGridCount = m_gridControlRooms.Count;

            for (int i = 0; i < nTeamCount && i < nGridCount; i++)
            {
                ComboBoxDataGridView grid = m_gridControlRooms[i];
                grid.Tag = teams[i];
            }

            foreach (ComboBoxDataGridView grid in m_gridControlRooms)
            {
                SetColumnData(grid, rooms);
                SetRowData(grid, positions);
                SetComboBox(grid, teams, rooms.Count);
                DataManagerToGrid(grid, (DataControlTeam)grid.Tag);
            }

            ReadWorkingTeam(m_gridControlRooms[0]);
        }

        private void SetFireCenterRoom()
        {
            m_gridFireCenterRooms.Add(dataGridFireCenterRoomScheduleA);
            m_gridFireCenterRooms.Add(dataGridFireCenterRoomScheduleB);
            m_gridFireCenterRooms.Add(dataGridFireCenterRoomScheduleC);

            m_dicGridViewLabel[dataGridFireCenterRoomScheduleA] = labelExternalMemberInfoA;
            m_dicGridViewLabel[dataGridFireCenterRoomScheduleB] = labelExternalMemberInfoB;
            m_dicGridViewLabel[dataGridFireCenterRoomScheduleC] = labelExternalMemberInfoC;

            dataGridFireCenterRoomScheduleA.RowHeadersWidth = dataGridFireCenterRoomScheduleB.RowHeadersWidth = dataGridFireCenterRoomScheduleC.RowHeadersWidth = 120;
            labelExternalMemberInfoA.Text = labelExternalMemberInfoB.Text = labelExternalMemberInfoC.Text = "";

            m_fireRoomType = m_dataMgr.GetControlRoomType("통합방재센터");

            if (m_fireRoomType == null)
            {
                Dictionary<int, DataControlRoomType> dicRoomTypes = m_dataMgr.GetControlRoomTypes();

                if (dicRoomTypes == null)
                    return;

                foreach (KeyValuePair<int, DataControlRoomType> pair in dicRoomTypes)
                {
                    if (pair.Value != m_controlRoomType)
                    {
                        m_fireRoomType = pair.Value;
                        break;
                    }
                }

                if (m_fireRoomType == null)
                    return;
            }

            if (m_fireRoomType != null && m_fireRoomType.RoomType.Length > 0)
            {
                labelFireRoomA.Text = m_fireRoomType.RoomType + " A조";
                labelFireRoomB.Text = m_fireRoomType.RoomType + " B조";
                labelFireRoomC.Text = m_fireRoomType.RoomType + " C조";
            }

            List<DataControlRoom> rooms = m_dataMgr.GetControlRooms(m_fireRoomType);

            if (rooms == null || rooms.Count == 0)
                return;

            List<DataControlTeamJobPosition> positions = m_dataMgr.GetJobPositions(m_fireRoomType);

            if (positions == null || positions.Count == 0)
                return;

            List<DataControlTeam> teams = m_dataMgr.GetControlTeams(m_fireRoomType);

            if (teams == null)
                return;

            int nTeamCount = teams.Count;
            int nGridCount = m_gridFireCenterRooms.Count;

            for (int i = 0; i < nTeamCount && i < nGridCount; i++ )
            {
                ComboBoxDataGridView grid = m_gridFireCenterRooms[i];
                grid.Tag = teams[i];
            }

            foreach (DataControlRoom room in rooms)
            {
                foreach (DataControlTeam team in teams)
                {
                    foreach (DataControlTeamJobPosition job in positions)
                    {
                        DataControlTeamMember member = m_dataMgr.GetControlTeamMember(room, team, job);

                        if (member != null)
                            member.MemberType = DataControlTeamMember.ControlMemberType.ExternalMember;
                    }
                }
            }

            foreach (ComboBoxDataGridView grid in m_gridFireCenterRooms)
            {
                SetColumnData(grid, rooms);
                SetRowData(grid, positions);
                SetComboBox(grid, teams, rooms.Count);
                DataManagerToGrid(grid, (DataControlTeam)grid.Tag);
            }

            ReadWorkingTeam(m_gridFireCenterRooms[0]);
        }

        private void SetDutyRoom()
        {
            //dataGridDuty.RowHeadersWidth = 120;
            //labelDutyInfo.Text = "";

            m_dutyRoomType = m_dataMgr.GetControlRoomType("당직실");

            if (m_dutyRoomType == null)
            {
                Dictionary<int, DataControlRoomType> dicRoomTypes = m_dataMgr.GetControlRoomTypes();

                if (dicRoomTypes == null)
                    return;

                foreach (KeyValuePair<int, DataControlRoomType> pair in dicRoomTypes)
                {
                    if (pair.Value != m_controlRoomType && pair.Value != m_fireRoomType)
                    {
                        m_dutyRoomType = pair.Value;
                        break;
                    }
                }

                if (m_dutyRoomType == null)
                    return;
            }

            List<DataControlRoom> rooms = m_dataMgr.GetControlRooms(m_dutyRoomType);

            if (rooms == null || rooms.Count == 0)
                return;

            List<DataControlTeamJobPosition> positions = m_dataMgr.GetJobPositions(m_dutyRoomType);

            if (positions == null || positions.Count == 0)
                return;

            List<DataControlTeam> teams = m_dataMgr.GetControlTeams(m_dutyRoomType);

            if (teams == null)
                return;

            //if (teams.Count > 0)
            //    dataGridDuty.Tag = teams[0];

            //SetColumnData(dataGridDuty, rooms);
            //SetRowData(dataGridDuty, positions);
            //DataManagerToGrid(dataGridDuty, (DataControlTeam)dataGridDuty.Tag);
        }

        private void DataManagerToGrid(DataGridView grid, DataControlTeam team)
        {
            if (team == null)
                return;

            int nColumnCount = grid.Columns.Count;

            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.Tag == null || (row.Tag is DataControlTeamJobPosition) == false)
                    continue;

                DataControlTeamJobPosition position = (DataControlTeamJobPosition)row.Tag;

                for (int i = 0; i < nColumnCount; i++)
                {
                    DataGridViewColumn column = grid.Columns[i];

                    if (column.Tag == null || (column.Tag is DataControlRoom) == false)
                        continue;

                    DataControlRoom room = (DataControlRoom)column.Tag;
                    DataGridViewCell cell = row.Cells[i];

                    DataControlTeamMember member = m_dataMgr.GetControlTeamBasicMember(room, team, position);

                    if (member == null)
                        cell.Value = null;
                    else
                        cell.Value = member.Member;
                }
            }
        }

        private void GridToDataManager(DataGridView grid, DataControlTeam team)
        {
            int nColumnCount = grid.Columns.Count;

            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.Tag == null || (row.Tag is DataControlTeamJobPosition) == false)
                    continue;

                DataControlTeamJobPosition position = (DataControlTeamJobPosition)row.Tag;

                for (int i = 0; i < nColumnCount; i++)
                {
                    DataGridViewColumn column = grid.Columns[i];

                    if (column.Tag == null || (column.Tag is DataControlRoom) == false)
                        continue;

                    DataControlRoom room = (DataControlRoom)column.Tag;
                    DataGridViewCell cell = row.Cells[i];

                    DataControlTeamMember member = m_dataMgr.GetControlTeamMember(room, team, position);

                    if (member != null)
                    {
                        DataCompanyMember companyMember = (DataCompanyMember)cell.Value;
                        member.Member = companyMember;
                    }
                }
            }
        }

        private void SetColumnData(DataGridView grid, List<DataControlRoom> rooms)
        {
            int nColumnCount = grid.Columns.Count;
            int nRoomCount = rooms.Count;

            grid.EnableHeadersVisualStyles = false;

            for (int i = 0; i < nRoomCount && i < nColumnCount; i++)
            {
                DataControlRoom room = rooms[i];
                DataGridViewColumn column = grid.Columns[i];

                column.HeaderCell.Style.BackColor = Color.FromArgb(147, 205, 221);
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                column.HeaderText = room.DisplayText;
                column.Tag = room;
            }

            if (nRoomCount < nColumnCount)
            {
                for (int i = nRoomCount; i < nColumnCount; i++)
                {
                    DataGridViewColumn column = grid.Columns[i];
                    column.Visible = false;
                }
            }
            else if (nRoomCount > nColumnCount)
            {
                for (int i = nColumnCount; i < nRoomCount; i++)
                {
                    DataControlRoom room = rooms[i];
                    DataGridViewColumn column = (DataGridViewColumn)grid.Columns[0].Clone();
                    grid.Columns.Add(column);

                    column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    column.HeaderText = room.DisplayText;
                    column.Tag = room;
                }

                grid.Columns[nColumnCount - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                grid.Columns[nRoomCount - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        private void SetRowData(DataGridView grid, List<DataControlTeamJobPosition> positions)
        {
            grid.EnableHeadersVisualStyles = false;
            int nPositionCount = positions.Count;

            for (int i = 0; i < nPositionCount; i++)
            {
                DataControlTeamJobPosition position = positions[i];
                DataGridViewRow row = FormWorkSchedule.MakeNewRow(grid);

                row.HeaderCell.Style.BackColor = Color.FromArgb(252, 213, 181);
                row.HeaderCell.Value = position.JobName;
                row.Tag = position;
            }
        }

        private void SetComboBox(ComboBoxDataGridView grid, List<DataControlTeam> teams, int nRoomCount)
        {
            return;

            if (teams.Count > 1)
            {
                grid.EnableHeadersVisualStyles = false;

                DataGridViewRow row = FormWorkSchedule.MakeNewRow(grid);
                row.HeaderCell.Value = "현재 근무조";
                row.HeaderCell.Style.BackColor = Color.FromArgb(247, 150, 70);

                for (int i = 0; i < nRoomCount; i++)
                {
                    ComboBox cbo = new ComboBox();

                    foreach (DataControlTeam team in teams)
                    {
                        cbo.Items.Add(team);
                    }

                    grid.SetComboBox(row.Cells[i], cbo);
                    row.Cells[i].Style.BackColor = row.HeaderCell.Style.BackColor;
                }
            }
        }

        private void ReadWorkingTeam(DataGridView grid)
        {
            return;

            int nRowCount = grid.Rows.Count;
            int nColumnCount = grid.Columns.Count;

            for (int i = nRowCount - 1; i >= 0; i--)
            {
                DataGridViewRow row = grid.Rows[i];

                if (row.Tag == null)
                {
                    for (int j = 0; j < nColumnCount; j++)
                    {
                        DataGridViewColumn column = grid.Columns[j];

                        if (column.Tag == null || (column.Tag is DataControlRoom) == false)
                            continue;

                        DataControlRoom room = (DataControlRoom)column.Tag;
                        DataControlWorkingTeam work = m_dataMgr.GetWorkTeam(room.ID);

                        if (work == null)
                            continue;

                        DataGridViewCell cell = row.Cells[j];
                        cell.Value = work.Team;
                    }

                    break;
                }
            }
        }

        private void dataGridControlRoomSchedule_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ComboBoxDataGridView grid = (ComboBoxDataGridView)sender;
            Label labelInfo;

            if (!m_dicGridViewLabel.TryGetValue(grid, out labelInfo))
                return;

            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                m_frmSearch.Cell = null;
                labelInfo.Text = "";
                return;
            }

            if (e.RowIndex == grid.Rows.Count)
            {
                m_frmSearch.Cell = null;
                labelInfo.Text = "";
                return;
            }

            DataGridViewCell cell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
            m_frmSearch.Cell = cell;
        }

        private void dataGridControlRoomSchedule_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            ComboBoxDataGridView grid = (ComboBoxDataGridView)sender;
            Label labelInfo;

            if (!m_dicGridViewLabel.TryGetValue(grid, out labelInfo))
                return;

            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                m_frmSearch.Cell = null;
                labelInfo.Text = "";
                return;
            }

            if (e.RowIndex == grid.Rows.Count)
            {
                m_frmSearch.Cell = null;
                labelInfo.Text = "";
                return;
            }

            DataGridViewCell cell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
            m_frmSearch.Cell = cell;

            ShowSearchForm(m_frmSearch);
        }

        private void dataGridControlRoomSchedule_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            ComboBoxDataGridView grid = (ComboBoxDataGridView)sender;
            Label labelInfo;

            if (!m_dicGridViewLabel.TryGetValue(grid, out labelInfo))
                return;

            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                m_frmSearch.Cell = null;
                labelInfo.Text = "";
                return;
            }

            if (e.RowIndex == grid.Rows.Count)
            {
                m_frmSearch.Cell = null;
                labelInfo.Text = "";
                return;
            }

            DataGridViewCell cell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
            m_frmSearch.Cell = cell;
            RefreshCell(cell);
        }

        private void dataGrid_KeyDown(object sender, KeyEventArgs e)
        {
            DataGridView grid = (DataGridView)sender;

            if (grid.SelectedCells.Count == 0)
                return;

            DataGridViewCell cell = grid.SelectedCells[0];

            if (cell.RowIndex < 0 || cell.ColumnIndex < 0)
                return;

            if (e.KeyCode == Keys.Delete)
            {
                DeleteData(cell);

                if (grid is ComboBoxDataGridView)
                {
                    Label labelInfo;

                    if (m_dicGridViewLabel.TryGetValue((ComboBoxDataGridView)grid, out labelInfo))
                    {
                        labelInfo.Text = "";
                    }
                }
            }
        }

        private void dataGridFireCenterRoomSchedule_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ComboBoxDataGridView grid = (ComboBoxDataGridView)sender;
            Label labelInfo;

            if (!m_dicGridViewLabel.TryGetValue(grid, out labelInfo))
                return;

            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                m_frmSearchExternal.Cell = null;
                labelInfo.Text = "";
                return;
            }

            if (e.RowIndex == grid.Rows.Count)
            {
                m_frmSearchExternal.Cell = null;
                labelInfo.Text = "";
                return;
            }

            DataGridViewCell cell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
            m_frmSearchExternal.Cell = cell;
        }

        private void dataGridFireCenterRoomSchedule_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            ComboBoxDataGridView grid = (ComboBoxDataGridView)sender;
            Label labelInfo;

            if (!m_dicGridViewLabel.TryGetValue(grid, out labelInfo))
                return;

            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                m_frmSearchExternal.Cell = null;
                labelInfo.Text = "";
                return;
            }

            if (e.RowIndex == grid.Rows.Count)
            {
                m_frmSearchExternal.Cell = null;
                labelInfo.Text = "";
                return;
            }

            DataGridViewCell cell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
            m_frmSearchExternal.Cell = cell;

            ShowSearchForm(m_frmSearchExternal);
        }

        private void dataGridFireCenterRoomSchedule_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            ComboBoxDataGridView grid = (ComboBoxDataGridView)sender;
            Label labelInfo;

            if (!m_dicGridViewLabel.TryGetValue(grid, out labelInfo))
                return;

            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                m_frmSearchExternal.Cell = null;
                labelInfo.Text = "";
                return;
            }

            if (e.RowIndex == grid.Rows.Count)
            {
                m_frmSearchExternal.Cell = null;
                labelInfo.Text = "";
                return;
            }

            DataGridViewCell cell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
            m_frmSearchExternal.Cell = cell;
            RefreshCell(cell);
        }

        private void dataGridDuty_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            m_dataChanged = true;
        }

        private void ShowSearchForm(Form frm)
        {
            if (!frm.Visible)
                frm.Show(this);
        }

        public void RefreshCell(DataGridViewCell cell)
        {
            Label label = null;

            if (cell.DataGridView is ComboBoxDataGridView)
            {
                if (!m_dicGridViewLabel.TryGetValue((ComboBoxDataGridView)cell.DataGridView, out label))
                    return;
            }
            //else if (cell.DataGridView == dataGridDuty)
            //    label = labelDutyInfo;
            else
                return;

            if (cell.Value != null && cell.Value is DataCompanyMember)
                label.Text = FormWorkSchedule.GetMemberFullPath((DataCompanyMember)cell.Value);
            else
                label.Text = "";
        }

        private void FormWorkSchedule2_Load(object sender, EventArgs e)
        {
            SetControlRoom();
            SetFireCenterRoom();
            SetDutyRoom();

            m_dataChanged = false;

            m_frmSearch.Init();
            m_frmSearchExternal.Init();

            InitWorkingTeamColors(m_gridControlRooms, dataGridControlRoomScheduleA);
            InitWorkingTeamColors(m_gridFireCenterRooms, dataGridFireCenterRoomScheduleA);

            SetFillLastColumn(m_gridControlRooms);
            SetFillLastColumn(m_gridFireCenterRooms);
            //SetFillLastColumn(dataGridDuty);
        }

        private void SetFillLastColumn(List<ComboBoxDataGridView> gridViews)
        {
            foreach (ComboBoxDataGridView grid in gridViews)
            {
                SetFillLastColumn(grid);
            }
        }

        private void SetFillLastColumn(DataGridView grid)
        {
            int nColumnCount = grid.Columns.Count;
            grid.Columns[nColumnCount - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void InitWorkingTeamColors(List<ComboBoxDataGridView> gridViews, ComboBoxDataGridView grid)
        {
            DataGridViewRow row = null;
            
            for (int i=grid.Rows.Count - 1;i>=0;i--)
            {
                DataGridViewRow _row = grid.Rows[i];

                if (_row.Tag == null)
                {
                    row = _row;
                    break;
                }
            }

            if (row == null)
                return;

            foreach (DataGridViewCell cell in row.Cells)
            {
                SetCurrentWorkingTeamBackColor(gridViews, cell.ColumnIndex, (DataControlTeam)cell.Value);
            }
        }

        private void dataGridControlRoomSchedule_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (m_systemCall)
                return;

            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            m_dataChanged = true;

            DataGridView grid = (DataGridView)sender;
            DataGridViewRow row = grid.Rows[e.RowIndex];

            if (row.Tag == null)
            {
                DataGridViewCell cell = row.Cells[e.ColumnIndex];

                // 현재 근무조는 모든 Grid에서 공통으로 사용되는 값이므로 하나의 Grid에서 값이 바뀌면 
                // 다른 Grid에도 똑같이 적용시킨다.
                m_systemCall = true;
                dataGridControlRoomScheduleA.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = cell.Value;
                dataGridControlRoomScheduleB.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = cell.Value;
                dataGridControlRoomScheduleC.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = cell.Value;
                dataGridControlRoomScheduleD.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = cell.Value;

                SetCurrentWorkingTeamBackColor(m_gridControlRooms, e.ColumnIndex, (DataControlTeam)cell.Value);
                m_systemCall = false;
            }
        }

        private void SetCurrentWorkingTeamBackColor(List<ComboBoxDataGridView> gridViews, int nColumnIndex, DataControlTeam team)
        {
            foreach (ComboBoxDataGridView grid in gridViews)
            {
                int nColumnCount = grid.Columns.Count;

                foreach (DataGridViewRow row in grid.Rows)
                {
                    if (row.IsNewRow || row.Tag == null)
                        continue;

                    if (grid.Tag == team)
                    {
                        row.Cells[nColumnIndex].Style.BackColor = CURRENT_WORKING_TEAM_BACK_COLOR;
                    }
                    else
                    {
                        row.Cells[nColumnIndex].Style.BackColor = Color.White;

                    }
                }
            }
        }

        private void dataGridFireCenterRoomSchedule_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (m_systemCall)
                return;

            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            m_dataChanged = true;

            DataGridView grid = (DataGridView)sender;
            DataGridViewRow row = grid.Rows[e.RowIndex];

            if (row.Tag == null)
            {
                DataGridViewCell cell = row.Cells[e.ColumnIndex];

                // 현재 근무조는 모든 Grid에서 공통으로 사용되는 값이므로 하나의 Grid에서 값이 바뀌면 
                // 다른 Grid에도 똑같이 적용시킨다.
                m_systemCall = true;
                dataGridFireCenterRoomScheduleA.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = cell.Value;
                dataGridFireCenterRoomScheduleB.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = cell.Value;
                dataGridFireCenterRoomScheduleC.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = cell.Value;

                SetCurrentWorkingTeamBackColor(m_gridFireCenterRooms, e.ColumnIndex, (DataControlTeam)cell.Value);
                m_systemCall = false;
            }
        }

        private const int WM_CLOSE = 0x0010;

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_CLOSE:
                    m_closeApplication = true;
                    break;
            }

            base.WndProc(ref m);
        }

        private void SaveData(DataGridViewCell cell)
        {
            DataControlRoom room = cell.OwningColumn.Tag as DataControlRoom;
            DataControlTeam team = cell.DataGridView.Tag as DataControlTeam;
            DataControlTeamJobPosition job = cell.OwningRow.Tag as DataControlTeamJobPosition;

            // 데이터 적용
            DataControlTeamMember memberBasic = m_dataMgr.GetControlTeamBasicMember(room, team, job);
            DataControlTeamMember memberRealTime = m_dataMgr.GetControlTeamMember(room, team, job);

            if (memberBasic != null)
            {
                DataCompanyMember companyMember = (DataCompanyMember)cell.Value;

                // 고정 근무표와 실시간 근무표의 담당자가 동일한 경우
                // 고정 근무표를 바꾸면 실시간 근무표도 동시에 바뀌도록 한다.
                if (memberRealTime != null)
                {
                    if (memberBasic.Member == memberRealTime.Member)
                    {
                        memberRealTime.Member = companyMember;
                        m_dataMgr.SaveControlTeamMember(room, team, job);
                    }
                }

                memberBasic.Member = companyMember;
            }
            ////

            m_dataMgr.SaveControlTeamBasicMember(room, team, job);

            AfterSaveData();
        }

        private void DeleteData(DataGridViewCell cell)
        {
            cell.Value = null;

            DataControlRoom room = cell.OwningColumn.Tag as DataControlRoom;
            DataControlTeam team = cell.DataGridView.Tag as DataControlTeam;
            DataControlTeamJobPosition job = cell.OwningRow.Tag as DataControlTeamJobPosition;

            // 데이터 적용
            DataControlTeamMember member = m_dataMgr.GetControlTeamBasicMember(room, team, job);

            if (member != null)
            {
                member.Member = null;
            }
            ////

            m_dataMgr.SaveControlTeamBasicMember(room, team, job);

            AfterSaveData();
        }

        private void SaveAllData()
        {
            foreach (ComboBoxDataGridView grid in m_gridControlRooms)
            {
                GridToDataManager(grid, (DataControlTeam)grid.Tag);
            }

            foreach (ComboBoxDataGridView grid in m_gridFireCenterRooms)
            {
                GridToDataManager(grid, (DataControlTeam)grid.Tag);
            }

            //GridToDataManager(dataGridDuty, (DataControlTeam)dataGridDuty.Tag);

            //SetWorkingTeam(dataGridControlRoomScheduleA);
            //SetWorkingTeam(dataGridFireCenterRoomScheduleA);

            m_dataMgr.SaveControlTeamBasicMembers();
            //m_dataMgr.SaveContorlWorkingTeam();

            AfterSaveData();
        }

        /// <summary>
        /// 근무조 데이터 변경시 이벤트 발생
        /// </summary>
        public event EventHandler MemberWorkDataChanged;

        private void AfterSaveData()
        {
            if (MemberWorkDataChanged != null)
                MemberWorkDataChanged(this, new EventArgs());
        }

        private void CloseForm()
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void SetWorkingTeam(DataGridView grid)
        {
            int nRowCount = grid.Rows.Count;
            int nColumnCount = grid.Columns.Count;

            for (int i = nRowCount - 1; i >= 0; i--)
            {
                DataGridViewRow row = grid.Rows[i];

                if (row.Tag == null)
                {
                    for (int j = 0; j < nColumnCount; j++)
                    {
                        DataGridViewColumn column = grid.Columns[j];

                        if (column.Tag == null || (column.Tag is DataControlRoom) == false)
                            continue;

                        DataControlRoom room = (DataControlRoom)column.Tag;
                        DataControlWorkingTeam work = m_dataMgr.GetWorkTeam(room.ID);

                        if (work == null)
                            continue;

                        DataGridViewCell cell = row.Cells[j];

                        if (cell.Value == null)
                            work.Team = null;
                        else if (cell.Value is DataControlTeam)
                            work.Team = (DataControlTeam)cell.Value;
                    }

                    break;
                }
            }
        }

        private void FormWorkSchedule2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (FormMemberWorkSchedule.Instance.Visible == false)
            {
                FormMemberWorkSchedule.Instance.Show(this.Owner);
            }

            //FormMemberWorkSchedule frm = new FormMemberWorkSchedule();
            //frm.Show(this.Owner);
        }

        private void dataGrid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            DataGridViewCell cell = ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex];

            if ((e.RowIndex + 1) % 2 == 1)
            {
                cell.Style.BackColor = Color.LightGray;
            }
            else
            {
                cell.Style.BackColor = Color.White;
            }
        }

    }
}
