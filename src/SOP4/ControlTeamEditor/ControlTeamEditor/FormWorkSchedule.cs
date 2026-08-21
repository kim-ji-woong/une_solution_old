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
    public partial class FormWorkSchedule : Form, IMainForm
    {
        private DataManager m_dataMgr = null;
        private WebDBManager m_dbMgr = null;
        
        private FormSearchMember m_frmSearch = null;
        private FormSearchExternalMember m_frmSearchExternal = null;

        private bool m_closeApplication = false;

        private static FormWorkSchedule m_instance = null;

        private DataControlRoomType m_controlRoomType = null;
        private DataControlRoomType m_fireRoomType = null;
        private DataControlRoomType m_dutyRoomType = null;

        private DataControlTeam m_dutyTeam = null;

        private int m_nRadioSpace = 6;

        public bool CloseApplication
        {
            get { return m_closeApplication; }
        }

        public static FormWorkSchedule Instance
        {
            get { return m_instance; }
        }

        public FormWorkSchedule(int nSiteID)
        {
            m_instance = this;
            InitializeComponent();

            this.DoubleBuffered = true;
            FormMain.SetDoubleBuffer(this.dataGridDuty, true);
            FormMain.SetDoubleBuffer(this.dataGridFireCenterRoomSchedule, true);
            FormMain.SetDoubleBuffer(this.dataGridControlRoomSchedule, true);

            Init(nSiteID);
        }

        private void Init(int nSiteID)
        {
            m_dbMgr = new WebDBManager(nSiteID);
            m_dataMgr = new DataManager(m_dbMgr, nSiteID);

            m_frmSearch = new FormSearchMember(m_dataMgr, this);
            m_frmSearchExternal = new FormSearchExternalMember(m_dataMgr, this);
        }

        private void SetDutyRoom()
        {
            dataGridDuty.RowHeadersWidth = 120;
            labelDutyInfo.Text = "";

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

            if (m_dutyRoomType.RoomType.Length > 0)
                groupBoxDutyRoom.Text = m_dutyRoomType.RoomType + " 근무표";

            List<DataControlRoom> rooms = m_dataMgr.GetControlRooms(m_dutyRoomType);

            if (rooms == null || rooms.Count == 0)
                return;

            List<DataControlTeamJobPosition> positions = m_dataMgr.GetJobPositions(m_dutyRoomType);

            if (positions == null || positions.Count == 0)
                return;

            List<DataControlTeam> teams = m_dataMgr.GetControlTeams(m_dutyRoomType);

            if (teams == null)
                return;

            if (teams.Count > 0)
                m_dutyTeam = teams[0];

            SetColumnData(dataGridDuty, rooms);
            SetRowData(dataGridDuty, positions);
            DataManagerToGrid(dataGridDuty, m_dutyTeam);
        }

        private void SetFireCenterRoom()
        {
            dataGridFireCenterRoomSchedule.RowHeadersWidth = 120;
            labelExternalMemberInfo.Text = "";

            radioFireScheduleA.Visible = false;
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

            if (m_fireRoomType.RoomType.Length > 0)
                groupBoxFireRoom.Text = m_fireRoomType.RoomType + " 근무표";

            List<DataControlRoom> rooms = m_dataMgr.GetControlRooms(m_fireRoomType);

            if (rooms == null || rooms.Count == 0)
                return;

            List<DataControlTeamJobPosition> positions = m_dataMgr.GetJobPositions(m_fireRoomType);

            if (positions == null || positions.Count == 0)
                return;

            List<DataControlTeam> teams = m_dataMgr.GetControlTeams(m_fireRoomType);

            if (teams == null)
                return;

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

            SetColumnData(dataGridFireCenterRoomSchedule, rooms);
            SetRowData(dataGridFireCenterRoomSchedule, positions);
            SetRadioControl(radioFireScheduleA, teams, this.radioFireCenterRoom_CheckedChanged);
            SetComboBox(dataGridFireCenterRoomSchedule, teams, rooms.Count);
            ReadWorkingTeam(dataGridFireCenterRoomSchedule);
        }

        private void SetControlRoom()
        {
            dataGridControlRoomSchedule.RowHeadersWidth = 120;
            labelMemberInfo.Text = "";

            radioScheduleA.Visible = false;
            this.m_controlRoomType = m_dataMgr.GetControlRoomType("제어실");

            if (m_controlRoomType == null)
            {
                Dictionary<int, DataControlRoomType> dicRoomTypes = m_dataMgr.GetControlRoomTypes();

                if (dicRoomTypes == null || dicRoomTypes.Count == 0)
                    return;

                m_controlRoomType = dicRoomTypes.ElementAt(0).Value;
            }

            if (m_controlRoomType != null && m_controlRoomType.RoomType.Length > 0)
                groupBoxControlRoom.Text = m_controlRoomType.RoomType + " 근무표";

            List<DataControlRoom> rooms = m_dataMgr.GetControlRooms(m_controlRoomType);

            if (rooms == null || rooms.Count == 0)
                return;

            List<DataControlTeamJobPosition> positions = m_dataMgr.GetJobPositions(m_controlRoomType);

            if (positions == null || positions.Count == 0)
                return;

            List<DataControlTeam> teams = m_dataMgr.GetControlTeams(m_controlRoomType);

            if (teams == null)
                return;

            SetColumnData(dataGridControlRoomSchedule, rooms);
            SetRowData(dataGridControlRoomSchedule, positions);
            SetRadioControl(radioScheduleA, teams, this.radioControlRoom_CheckedChanged);
            SetComboBox(dataGridControlRoomSchedule, teams, rooms.Count);
            ReadWorkingTeam(dataGridControlRoomSchedule);
        }

        private void ReadWorkingTeam(DataGridView grid)
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
                        cell.Value = work.Team;
                    }

                    break;
                }
            }
        }

        private void SetRadioControl(RadioButton firstRadio, List<DataControlTeam> teams, System.EventHandler handler)
        {
            if (teams.Count > 1)
            {
                firstRadio.Text = teams[0].DisplayText;
                firstRadio.Show();

                RadioButton prev = firstRadio;
                prev.Tag = teams[0];

                for (int i = 1; i < teams.Count; i++)
                {
                    DataControlTeam team = teams[i];
                    RadioButton radio = new RadioButton();

                    radio.AutoSize = true;
                    radio.Location = new System.Drawing.Point(prev.Location.X + prev.Size.Width + m_nRadioSpace, prev.Location.Y);
                    radio.TabIndex = prev.TabIndex + 1;
                    radio.TabStop = prev.TabStop;
                    radio.Text = team.DisplayText;
                    radio.UseVisualStyleBackColor = prev.UseVisualStyleBackColor;

                    prev.Parent.Controls.Add(radio);
                    radio.Show();

                    prev = radio;

                    prev.Tag = team;
                    prev.CheckedChanged += new EventHandler(handler);
                }

                firstRadio.Checked = true;
            }
        }

        private void SetColumnData(DataGridView grid, List<DataControlRoom> rooms)
        {
            int nColumnCount = grid.Columns.Count;
            int nRoomCount = rooms.Count;

            for (int i = 0; i < nRoomCount && i < nColumnCount; i++)
            {
                DataControlRoom room = rooms[i];
                DataGridViewColumn column = grid.Columns[i];

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
            int nPositionCount = positions.Count;

            for (int i = 0; i < nPositionCount; i++)
            {
                DataControlTeamJobPosition position = positions[i];
                DataGridViewRow row = MakeNewRow(grid);

                row.HeaderCell.Value = position.JobName;
                row.Tag = position;
            }
        }

        private void SetComboBox(ComboBoxDataGridView grid, List<DataControlTeam> teams, int nRoomCount)
        {
            if (teams.Count > 1)
            {
                DataGridViewRow row = MakeNewRow(grid);
                row.HeaderCell.Value = "현재 근무조";

                for (int i = 0; i < nRoomCount; i++)
                {
                    ComboBox cbo = new ComboBox();

                    foreach (DataControlTeam team in teams)
                    {
                        cbo.Items.Add(team);
                    }

                    grid.SetComboBox(row.Cells[i], cbo);
                }
            }
        }

        public static DataGridViewRow MakeNewRow(DataGridView grid)
        {
            if (grid.AllowUserToAddRows)
            {
                DataGridViewRow row = (DataGridViewRow)grid.Rows[grid.Rows.Count - 1].Clone();
                grid.Rows.Add(row);

                return grid.Rows[grid.Rows.Count - 2];
            }
            else
            {
                grid.AllowUserToAddRows = true;

                DataGridViewRow row = (DataGridViewRow)grid.Rows[grid.Rows.Count - 1].Clone();
                grid.Rows.Add(row);

                grid.AllowUserToAddRows = false;
            }

            return grid.Rows[grid.Rows.Count - 1];
        }

        private void ShowSearchForm(Form frm)
        {
            if (!frm.Visible)
                frm.Show(this);
        }

        private void dataGridControlRoomSchedule_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                m_frmSearch.Cell = null;
                labelMemberInfo.Text = "";
                return;
            }

            if (e.RowIndex == dataGridControlRoomSchedule.Rows.Count - 1)
            {
                m_frmSearch.Cell = null;
                labelMemberInfo.Text = "";
                return;
            }

            DataGridViewCell cell = dataGridControlRoomSchedule.Rows[e.RowIndex].Cells[e.ColumnIndex];
            m_frmSearch.Cell = cell;
        }

        private void dataGridControlRoomSchedule_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                m_frmSearch.Cell = null;
                labelMemberInfo.Text = "";
                return;
            }

            if (e.RowIndex == dataGridControlRoomSchedule.Rows.Count - 1)
            {
                m_frmSearch.Cell = null;
                labelMemberInfo.Text = "";
                return;
            }

            DataGridViewCell cell = dataGridControlRoomSchedule.Rows[e.RowIndex].Cells[e.ColumnIndex];
            m_frmSearch.Cell = cell;

            ShowSearchForm(m_frmSearch);
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

        private void dataGridControlRoomSchedule_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                m_frmSearch.Cell = null;
                labelMemberInfo.Text = "";
                return;
            }

            if (e.RowIndex == dataGridControlRoomSchedule.Rows.Count - 1)
            {
                m_frmSearch.Cell = null;
                labelMemberInfo.Text = "";
                return;
            }

            DataGridViewCell cell = dataGridControlRoomSchedule.Rows[e.RowIndex].Cells[e.ColumnIndex];
            m_frmSearch.Cell = cell;
            RefreshCell(cell);
        }

        public static string GetMemberFullPath(DataCompanyMember member)
        {
            if (member.TeamPositions.Count == 0)
                return member.MemberName;

            KeyValuePair<DataTeam, JobPosition> pair = member.TeamPositions.ElementAt(0);
            string strTeamPath = FormSearchMember.GetTeamFullPath(pair.Key);
            return String.Format("- {0}", strTeamPath.Length == 0 ? member.MemberName : strTeamPath + "  >> " + member.MemberName);
        }

        public void RefreshCell(DataGridViewCell cell)
        {
            Label label = null;

            if (cell.DataGridView == dataGridControlRoomSchedule)
                label = labelMemberInfo;
            else if (cell.DataGridView == dataGridFireCenterRoomSchedule)
                label = labelExternalMemberInfo;
            else if (cell.DataGridView == dataGridDuty)
                label = labelDutyInfo;
            else
                return;

            if (cell.Value != null && cell.Value is DataCompanyMember)
                label.Text = GetMemberFullPath((DataCompanyMember)cell.Value);
            else
                label.Text = "";
        }

        private void dataGridFireCenterRoomSchedule_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                m_frmSearchExternal.Cell = null;
                labelExternalMemberInfo.Text = "";
                return;
            }

            if (e.RowIndex == dataGridFireCenterRoomSchedule.Rows.Count - 1)
            {
                m_frmSearchExternal.Cell = null;
                labelExternalMemberInfo.Text = "";
                return;
            }

            DataGridViewCell cell = dataGridFireCenterRoomSchedule.Rows[e.RowIndex].Cells[e.ColumnIndex];
            m_frmSearchExternal.Cell = cell;
        }

        private void dataGridFireCenterRoomSchedule_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                m_frmSearchExternal.Cell = null;
                labelExternalMemberInfo.Text = "";
                return;
            }

            if (e.RowIndex == dataGridFireCenterRoomSchedule.Rows.Count - 1)
            {
                m_frmSearchExternal.Cell = null;
                labelExternalMemberInfo.Text = "";
                return;
            }

            DataGridViewCell cell = dataGridFireCenterRoomSchedule.Rows[e.RowIndex].Cells[e.ColumnIndex];
            m_frmSearchExternal.Cell = cell;

            ShowSearchForm(m_frmSearchExternal);
        }

        private void dataGridFireCenterRoomSchedule_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                m_frmSearchExternal.Cell = null;
                labelExternalMemberInfo.Text = "";
                return;
            }

            if (e.RowIndex == dataGridFireCenterRoomSchedule.Rows.Count - 1)
            {
                m_frmSearchExternal.Cell = null;
                labelExternalMemberInfo.Text = "";
                return;
            }

            DataGridViewCell cell = dataGridFireCenterRoomSchedule.Rows[e.RowIndex].Cells[e.ColumnIndex];
            m_frmSearchExternal.Cell = cell;
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
                cell.Value = null;

                if (grid == dataGridControlRoomSchedule)
                    labelMemberInfo.Text = "";
                else if (grid == dataGridFireCenterRoomSchedule)
                    labelExternalMemberInfo.Text = "";
                else if (grid == dataGridDuty)
                    labelDutyInfo.Text = "";
            }
        }

        private void dataGridDuty_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                labelDutyInfo.Text = "";
                m_frmSearch.Cell = null;
                return;
            }

            DataGridViewCell cell = dataGridDuty.Rows[e.RowIndex].Cells[e.ColumnIndex];
            m_frmSearch.Cell = cell;

            ShowSearchForm(m_frmSearch);
        }

        private void dataGridDuty_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                labelDutyInfo.Text = "";
                m_frmSearch.Cell = null;
                return;
            }

            DataGridViewCell cell = dataGridDuty.Rows[e.RowIndex].Cells[e.ColumnIndex];
            m_frmSearch.Cell = cell;
        }

        private void dataGridDuty_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                labelDutyInfo.Text = "";
                m_frmSearch.Cell = null;
                return;
            }

            DataGridViewCell cell = dataGridDuty.Rows[e.RowIndex].Cells[e.ColumnIndex];
            m_frmSearch.Cell = cell;
            RefreshCell(cell);
        }

        private void FormWorkSchedule_Load(object sender, EventArgs e)
        {
            SetControlRoom();
            SetFireCenterRoom();
            SetDutyRoom();

            m_frmSearch.Init();
            m_frmSearchExternal.Init();

            SetFillLastColumn(dataGridControlRoomSchedule);
            SetFillLastColumn(dataGridFireCenterRoomSchedule);
            SetFillLastColumn(dataGridDuty);
        }

        private void SetFillLastColumn(DataGridView grid)
        {
            int nColumnCount = grid.Columns.Count;
            grid.Columns[nColumnCount - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void radioControlRoom_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radio = (RadioButton)sender;

            if (radio.Tag == null || radio.Checked == false)
                return;

            if (radio.Tag is DataControlTeam)
            {
                SetGridData(dataGridControlRoomSchedule, (DataControlTeam)radio.Tag);
            }
        }

        private void radioFireCenterRoom_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radio = (RadioButton)sender;

            if (radio.Tag == null || radio.Checked == false)
                return;

            if (radio.Tag is DataControlTeam)
            {
                SetGridData(dataGridFireCenterRoomSchedule, (DataControlTeam)radio.Tag);
            }
        }

        private void SetGridData(DataGridView grid, DataControlTeam team)
        {
            if (grid.Tag != null)
                GridToDataManager(grid, (DataControlTeam)grid.Tag);

            DataManagerToGrid(grid, team);
            grid.Tag = team;
        }

        private void DataManagerToGrid(DataGridView grid, DataControlTeam team)
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

        private void SetWorkingTeam(DataGridView grid)
        {
            int nRowCount = grid.Rows.Count;
            int nColumnCount = grid.Columns.Count;

            for (int i=nRowCount-1;i>=0;i--)
            {
                DataGridViewRow row = grid.Rows[i];

                if (row.Tag == null)
                {
                    for (int j=0;j<nColumnCount;j++)
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

        private void btnOK_Click(object sender, EventArgs e)
        {
            GridToDataManager(dataGridControlRoomSchedule, (DataControlTeam)dataGridControlRoomSchedule.Tag);
            GridToDataManager(dataGridFireCenterRoomSchedule, (DataControlTeam)dataGridFireCenterRoomSchedule.Tag);
            GridToDataManager(dataGridDuty, m_dutyTeam);

            SetWorkingTeam(dataGridControlRoomSchedule);
            SetWorkingTeam(dataGridFireCenterRoomSchedule);

            m_dataMgr.SaveControlTeamMembers();
            m_dataMgr.SaveControlWorkingTeams();

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
