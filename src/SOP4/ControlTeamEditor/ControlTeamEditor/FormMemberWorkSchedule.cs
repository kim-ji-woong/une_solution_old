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
    public partial class FormMemberWorkSchedule : Form, IMainForm
    {
        /// <summary>
        /// 근무조 데이터 변경시 이벤트 발생
        /// </summary>
        public event EventHandler MemberWorkDataChanged;

        private DataManager m_dataMgr = null;
        private WebDBManager m_dbMgr = null;

        private FormSearchMember m_frmSearch = null;
        private FormSearchExternalMember m_frmSearchExternal = null;

        private bool m_closeApplication = false;
        public bool CloseApplication { get { return m_closeApplication; } }

        private static FormMemberWorkSchedule m_instance = null;
        public static FormMemberWorkSchedule Instance { get { return m_instance; } }

        private FormWorkSchedule2 m_frmBasicWorkMember = null;

        private DataControlRoomType m_controlRoomType = null;
        private DataControlRoomType m_fireRoomType = null;
        private DataControlRoomType m_dutyRoomType = null;

        private DataControlTeam m_dutyTeam = null;

        // 각 셀마다 배정되어 있는 Label
        private Dictionary<DataGridViewCell, Label> m_dicCellLable = new Dictionary<DataGridViewCell, Label>();

        // 기본근무자 변경여부 (변경시에 데이터를 리로드하도록 함)
        private bool m_isBasicMemberChanged = false;

        // 데이터를 불러
        private bool m_isDataLoading = false;
        private int m_nSiteID = 1;

        public WebDBManager DBManager
        {
            get { return m_dbMgr; }
        }

        public FormMemberWorkSchedule(int nSiteID)
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            FormMain.SetDoubleBuffer(dataGridControlRoomSchedule, true);
            FormMain.SetDoubleBuffer(dataGridDuty, true);
            FormMain.SetDoubleBuffer(dataGridFireCenterRoomSchedule, true);

            m_instance = this;
            m_nSiteID = nSiteID;

            Init(nSiteID);
            InitEvent();
        }


        private void Init(int nSiteID)
        {
            m_dbMgr = new WebDBManager(nSiteID);
            m_dataMgr = new DataManager(m_dbMgr, nSiteID);
            //m_dataMgr.SiteID = nSiteID;

            m_frmSearch = new FormSearchMember(m_dataMgr, this);
            m_frmSearchExternal = new FormSearchExternalMember(m_dataMgr, this);
        }

        private void InitEvent()
        {
            // Form 이벤트
            Load += FormMemberWorkSchedule_Load;
            FormClosing += FormMemberWorkSchedule_FormClosing;

            // 검색 컨트롤 이벤트
            m_frmSearch.CompanyMemberSelected += m_frmSearch_MemberSelected;
            m_frmSearchExternal.ExternalMemberSelected += m_frmSearch_MemberSelected;

            // Grid 제어실 이벤트
            dataGridControlRoomSchedule.Resize += dataGridControlRoomSchedule_Resize;
            dataGridControlRoomSchedule.CellPainting += dataGrid_CellPainting;
            dataGridControlRoomSchedule.KeyDown += dataGrid_KeyDown;
            dataGridControlRoomSchedule.CellClick += (s, e) => { dataGridControlRoomSchedule_MouseAction(s, e, 0); };
            dataGridControlRoomSchedule.CellDoubleClick += (s, e) => { dataGridControlRoomSchedule_MouseAction(s, e, 1); };
            dataGridControlRoomSchedule.CellEnter += (s, e) => { dataGridControlRoomSchedule_MouseAction(s, e, 2); };

            // Grid 방재센터 이벤트
            dataGridFireCenterRoomSchedule.CellPainting += dataGrid_CellPainting;
            dataGridFireCenterRoomSchedule.KeyDown += dataGrid_KeyDown;
            dataGridFireCenterRoomSchedule.CellClick += (s, e) => { dataGridFireCenterRoomSchedule_MouseAction(s, e, 0); };
            dataGridFireCenterRoomSchedule.CellDoubleClick += (s, e) => { dataGridFireCenterRoomSchedule_MouseAction(s, e, 1); };
            dataGridFireCenterRoomSchedule.CellEnter += (s, e) => { dataGridFireCenterRoomSchedule_MouseAction(s, e, 2); };

            // Grid 당직실 이벤트
            dataGridDuty.KeyDown += dataGrid_KeyDown;
            dataGridDuty.CellClick += (s, e) => { dataGridDuty_MouseAction(s, e, 0); };
            dataGridDuty.CellDoubleClick += (s, e) => { dataGridDuty_MouseAction(s, e, 1); };
            dataGridDuty.CellEnter += (s, e) => { dataGridDuty_MouseAction(s, e, 2); };

            // ComboBox 현재 근무조 이벤트
            cboSchedule.SelectedIndexChanged += cboSchedule_SelectedIndexChanged;
            cboFireSchedule.SelectedIndexChanged += cboFireSchedule_SelectedIndexChanged;

            // ContextMenu 기본 근무직원적용 이벤트
            cmsMain.Opening += cmsMain_Opening;
            cmiInit.Click += cmiInit_Click;

            // Button 닫기버튼 이벤트
            btnCancel.Click += btnCancel_Click;
            btnClose.Click += btnClose_Click;

            // Button 조별 기본근무직원 편집창 이벤트
            btnShowBasicWorkMember.Click += btnShowBasicWorkMember_Click;
        }

        private void FormMemberWorkSchedule_Load(object sender, EventArgs e)
        {
            // 1. 데이터 로드
            m_isDataLoading = true;

            SetControlRoom();
            SetFireCenterRoom();
            SetDutyRoom();

            m_frmSearch.Init();
            m_frmSearchExternal.Init();

            m_isDataLoading = false;
        }

        private void FormMemberWorkSchedule_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_frmBasicWorkMember != null)
            {
                if (m_frmBasicWorkMember.Visible == true)
                {
                    m_frmBasicWorkMember.Close();
                }
            }
        }


        #region Data Load

        /// <summary>
        /// Data Manager에서 가지고있는 데이터를 Grid에 출력
        /// </summary>
        /// <param name="grid">대상 그리드</param>
        /// <param name="team">현재 근무조</param>
        private void DataManagerToGrid(DataGridView grid, DataControlTeam team)
        {
            int nColumnCount = grid.Columns.Count;

            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.Tag != null && (row.Tag is DataControlTeamJobPosition) == true)
                {
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
        }

        /// <summary>
        /// 각각의 셀마다 기본 근무조원 데이터 지정
        /// <param name="grid">대상 그리드</param>
        /// </summary>
        private void SetLabel(DataGridView grid)
        {
            for (int nRow = 0; nRow < grid.Rows.Count; nRow++)
            {
                for (int nCol = 0; nCol < grid.Columns.Count; nCol++)
                {
                    Label lb = new Label();
                    lb.BackColor = Color.Transparent;
                    lb.AutoSize = false;
                    lb.TextAlign = ContentAlignment.BottomCenter;
                    lb.Font = new Font("굴림", 7.2f);

                    grid.Controls.Add(lb);

                    System.Drawing.Rectangle rect = grid.GetCellDisplayRectangle(nCol, nRow, false);

                    lb.Location = new Point(rect.X + 3, rect.Y + 36);
                    lb.Size = new Size(rect.Width - 8, rect.Height - 40);

                    DataGridViewCell cell = grid.Rows[nRow].Cells[nCol];

                    DataControlTeamMember member = m_dataMgr.GetControlTeamBasicMember(grid.Columns[nCol].Tag as DataControlRoom, grid.Tag as DataControlTeam, grid.Rows[nRow].Tag as DataControlTeamJobPosition);

                    if (member == null)
                    {
                        lb.Text = String.Empty;
                    }
                    else if (member.Member == null)
                    {
                        lb.Text = String.Empty;
                    }
                    else
                    {
                        lb.Text = String.Format("({0})", member.Member.MemberName);
                        lb.Tag = member;
                    }

                    lb.Show();

                    if (m_dicCellLable.ContainsKey(cell))
                    {
                        m_dicCellLable[cell].Dispose();
                        m_dicCellLable[cell] = lb;
                    }
                    else
                    {
                        m_dicCellLable.Add(cell, lb);
                    }

                    lb.Click += (s, e) => { lb_MouseAction(s, e, 0); };
                    lb.DoubleClick += (s, e) => { lb_MouseAction(s, e, 1); };

                }
            }
        }

        /// <summary>
        /// Cell의 기본직원(Label)을 클릭하였을 경우 Cell 클릭하였을 때와 동일한 효과를 주도록 함.
        /// <param name="nMouseActionType">0: Cell Click,  1: Cell Double Click</param>
        /// </summary>
        private void lb_MouseAction(object sender, EventArgs e, int nMouseAction)
        {
            Label label = (sender as Label);
            DataGridViewCell cell = null;

            foreach (KeyValuePair<DataGridViewCell, Label> item in from items in m_dicCellLable
                                                                   where items.Value == label
                                                                   select items
                                             )
            {
                cell = item.Key;
                break;
            }

            if (cell == null)
                return;

            if (cell.DataGridView == null)
                return;


            cell.Selected = true;

            Form frmSearchForm = null;

            if (cell.DataGridView == dataGridControlRoomSchedule || cell.DataGridView == dataGridDuty)
            {
                m_frmSearch.Cell = cell;
                frmSearchForm = m_frmSearch;
            }
            else if (cell.DataGridView == dataGridFireCenterRoomSchedule)
            {
                m_frmSearchExternal.Cell = cell;
                frmSearchForm = m_frmSearchExternal;
            }

            RefreshCell(cell);

            if (nMouseAction == 1)
            {
                ShowSearchForm(frmSearchForm);
            }
        }
        

        #region Init Data & Control

        /// <summary>
        /// 당직실 근무데이터 세팅
        /// </summary>
        private void SetDutyRoom()
        {
            dataGridDuty.RowHeadersWidth = 180;
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

            if (m_dutyRoomType != null && m_dutyRoomType.RoomType.Length > 0)
                labelDutyRoom.Text = m_dutyRoomType.RoomType + " 근무표";

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

            dataGridDuty.Tag = m_dutyTeam;
        }

        /// <summary>
        /// 방재센터 근무데이터 세팅
        /// </summary>
        private void SetFireCenterRoom()
        {
            dataGridFireCenterRoomSchedule.RowHeadersWidth = 180;
            labelExternalMemberInfo.Text = "";

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
                labelFireRoom.Text = m_fireRoomType.RoomType + " 근무표";

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
            SetTeamComboBox(cboFireSchedule, teams);
            ReadWorkingTeam(dataGridFireCenterRoomSchedule);

            SetLabel(dataGridFireCenterRoomSchedule);
        }

        /// <summary>
        /// 제어실 근무데이터 세팅
        /// </summary>
        private void SetControlRoom()
        {
            dataGridControlRoomSchedule.RowHeadersWidth = 180;
            labelMemberInfo.Text = "";

            this.m_controlRoomType = m_dataMgr.GetControlRoomType("제어실");

            if (m_controlRoomType == null)
            {
                Dictionary<int, DataControlRoomType> dicRoomTypes = m_dataMgr.GetControlRoomTypes();

                if (dicRoomTypes == null || dicRoomTypes.Count == 0)
                    return;

                m_controlRoomType = dicRoomTypes.ElementAt(0).Value;
            }

            if (m_controlRoomType != null && m_controlRoomType.RoomType.Length > 0)
                labelControlRoom.Text = m_controlRoomType.RoomType + " 근무표";

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
            SetTeamComboBox(cboSchedule, teams);
            ReadWorkingTeam(dataGridControlRoomSchedule);

            SetLabel(dataGridControlRoomSchedule);
        }


        /// <summary>
        /// 현재 근무중인 근무조로 데이터 세팅
        /// </summary>
        /// <param name="grid">대상 그리드</param>
        private void ReadWorkingTeam(DataGridView grid)
        {
            int nColumnCount = grid.Columns.Count;

            for (int j = 0; j < nColumnCount; j++)
            {
                DataGridViewColumn column = grid.Columns[j];

                if (column.Tag == null || (column.Tag is DataControlRoom) == false)
                    continue;

                DataControlRoom room = (DataControlRoom)column.Tag;
                DataControlWorkingTeam work = m_dataMgr.GetWorkTeam(room.ID);

                if (work == null)
                    continue;

                ComboBox cbo = null;

                if (cboSchedule.Items.Contains(work.Team))
                {
                    cbo = cboSchedule;
                }
                else if (cboFireSchedule.Items.Contains(work.Team))
                {
                    cbo = cboFireSchedule;
                }

                if (cbo == null)
                    continue;

                for (int nIndex = 0; nIndex < cbo.Items.Count; nIndex++)
                {
                    if (Object.Equals(cbo.Items[nIndex], work.Team))
                    {
                        cbo.SelectedIndex = nIndex;
                        break;
                    }
                }

                break;
            }
        }

        /// <summary>
        /// 활성화 가능한 현재근무조를 ComboBox의 항목으로 생성
        /// </summary>
        /// <param name="cbo">대상 콤보박스</param>
        /// <param name="teams">활성화 가능한 근무조 목록</param>
        private void SetTeamComboBox(ComboBox cbo, List<DataControlTeam> teams)
        {
            cbo.Items.Clear();

            foreach (DataControlTeam team in teams)
            {
                cbo.Items.Add(team);
            }
        }

        /// <summary>
        /// Column 생성
        /// </summary>
        /// <param name="grid">대상 그리드</param>
        /// <param name="rooms">생성해야할 컬럼정의</param>
        private void SetColumnData(DataGridView grid, List<DataControlRoom> rooms)
        {
            grid.EnableHeadersVisualStyles = false;

            int nColumnCount = grid.Columns.Count;
            int nRoomCount = rooms.Count;

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

        /// <summary>
        /// Row 생성
        /// </summary>
        /// <param name="grid">대상 그리드</param>
        /// <param name="positions">근무표상의 직위</param>
        private void SetRowData(DataGridView grid, List<DataControlTeamJobPosition> positions)
        {
            grid.Rows.Clear();

            grid.EnableHeadersVisualStyles = false;
            int nPositionCount = positions.Count;

            for (int i = 0; i < nPositionCount; i++)
            {
                DataControlTeamJobPosition position = positions[i];
                DataGridViewRow row = MakeNewRow(grid);

                row.HeaderCell.Style.BackColor = Color.FromArgb(252, 213, 181);
                row.HeaderCell.Value = position.JobName;
                row.Tag = position;

            }

        }

        #endregion Data Load & Control Setting

        #endregion Data Load


        #region Data Save

        /// <summary>
        /// 데이터 저장
        /// </summary>
        /// <param name="cell">편집한 Cell</param>
        private void SaveData(DataGridViewCell cell)
        {
            if (m_isDataLoading)
                return;

            DataControlRoom room = cell.OwningColumn.Tag as DataControlRoom;
            DataControlTeam team = cell.DataGridView.Tag as DataControlTeam;
            DataControlTeamJobPosition job = cell.OwningRow.Tag as DataControlTeamJobPosition;

            // 데이터 적용
            DataControlTeamMember member = m_dataMgr.GetControlTeamMember(room, team, job);

            if (member != null)
            {
                DataCompanyMember companyMember = (DataCompanyMember)cell.Value;
                member.Member = companyMember;
            }

            m_dataMgr.SaveControlTeamMember(room, team, job);

            // Basic Member도 같이 데이터를 변경한 경우에 UI데이터도 똑같이 변경 처리

            if (m_dicCellLable.ContainsKey(cell) == true)
            {
                if (m_dicCellLable[cell] != null)
                {
                    Label lb = m_dicCellLable[cell];
                    if (lb.Tag == null)
                    {
                        DataControlTeamMember basicMember = m_dataMgr.GetControlTeamBasicMember(room, team, job);
                        basicMember.Member = member.Member;
                        basicMember.MemberType = member.MemberType;

                        lb.Text = String.Format("({0})", basicMember.Member.MemberName);
                        lb.Tag = basicMember;
                    }
                }
            }

            AfterSaveData();
        }

        /// <summary>
        /// 현재 근무조 저장
        /// </summary>
        /// <param name="grid">대상이 되는 그리드</param>
        /// <param name="team">현재 근무조</param>
        private void SaveDataWorkingTeam(DataGridView grid, DataControlTeam team)
        {
            if (m_isDataLoading)
                return;

            int nColumnCount = grid.Columns.Count;

            for (int j = 0; j < nColumnCount; j++)
            {
                DataGridViewColumn column = grid.Columns[j];

                if (column.Tag == null || (column.Tag is DataControlRoom) == false)
                    continue;

                DataControlRoom room = (DataControlRoom)column.Tag;
                DataControlWorkingTeam work = m_dataMgr.GetWorkTeam(room.ID);

                if (work == null)
                    continue;

                work.Team = team;

                m_dataMgr.SaveControlWorkingTeam(work);
            }

            AfterSaveData();
        }

        /// <summary>
        /// 데이터 저장후 실행
        /// </summary>
        private void AfterSaveData()
        {
            if (MemberWorkDataChanged != null)
                MemberWorkDataChanged(this, new EventArgs());

        }

        #endregion Data Save


        #region Data Delete

        private void DeleteData(DataGridViewCell cell)
        {
            cell.Value = null;

            DataControlRoom room = cell.OwningColumn.Tag as DataControlRoom;
            DataControlTeam team = cell.DataGridView.Tag as DataControlTeam;
            DataControlTeamJobPosition job = cell.OwningRow.Tag as DataControlTeamJobPosition;

            // 데이터 적용
            DataControlTeamMember member = m_dataMgr.GetControlTeamMember(room, team, job);

            if (member != null)
            {
                member.Member = null;
            }
            ////

            m_dataMgr.SaveControlTeamMember(room, team, job);

            AfterSaveData();
        }

        #endregion Data Delete


        #region ETC
        
        #region Win32

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

        #endregion Win32


        #region Search Control

        private void ShowSearchForm(Form frm)
        {
            if (!frm.Visible)
                frm.Show(this);
        }

        private void m_frmSearch_MemberSelected(object sender, EventArgs e)
        {
            SaveData(sender as DataGridViewCell);
        }

        #endregion Search Control


        #region Grid 공통 이벤트

        /// <summary>
        /// 사용자가 선택한 근무조원과 기본 근무조원이 같은지 다른지 확인하여 편집자에게 알려줌
        /// </summary>
        private void dataGrid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            bool bVisible = true;
            DataGridViewCell cell = ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex];

            if (m_dicCellLable.ContainsKey(cell) == false)
                return;

            Label label = m_dicCellLable[cell];

            try
            {
                DataControlRoom room = cell.OwningColumn.Tag as DataControlRoom;
                DataControlTeam team = cell.DataGridView.Tag as DataControlTeam;
                DataControlTeamJobPosition job = cell.OwningRow.Tag as DataControlTeamJobPosition;

                DataControlTeamMember memberCell = m_dataMgr.GetControlTeamMember(room, team, job);

                if (label.Tag is DataControlTeamMember && memberCell.Member != null)
                {
                    DataControlTeamMember memberlebel = label.Tag as DataControlTeamMember;

                    if (memberCell.Member.ID == memberlebel.Member.ID)
                    {
                        bVisible = false;
                    }
                }
            }
            finally
            {
                label.Visible = bVisible;

                if (bVisible && label.Tag != null)
                {
                    cell.Style.BackColor = Color.LightCoral;
                }
                else
                {
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

        /// <summary>
        /// 삭제의 경우에만 사용
        /// </summary>
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

                if (grid == dataGridControlRoomSchedule)
                    labelMemberInfo.Text = "";
                else if (grid == dataGridFireCenterRoomSchedule)
                    labelExternalMemberInfo.Text = "";
                else if (grid == dataGridDuty)
                    labelDutyInfo.Text = "";
            }
        }

        #endregion Grid 공통 이벤트


        #region Grid 제어실 이벤트

        private void dataGridControlRoomSchedule_Resize(object sender, EventArgs e)
        {
            DataGridView grid = sender as DataGridView;

            for (int nRow = 0; nRow < grid.Rows.Count; nRow++)
            {
                for (int nCol = 0; nCol < grid.Columns.Count; nCol++)
                {
                    DataGridViewCell cell = grid.Rows[nRow].Cells[nCol];

                    if (m_dicCellLable.ContainsKey(cell))
                    {
                        Label lb = m_dicCellLable[cell];

                        System.Drawing.Rectangle rect = grid.GetCellDisplayRectangle(nCol, nRow, false);

                        lb.Location = new Point(rect.X + 3, rect.Y + 64);
                        lb.Size = new Size(rect.Width - 8, rect.Height - 66);
                    }
                }
            }
        }

        /// <summary>
        /// 제어실 근무표에 대한 마우스 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="nMouseActionType">0: Cell Click,  1: Cell Double Click,  2: Cell Enter</param>
        private void dataGridControlRoomSchedule_MouseAction(object sender, DataGridViewCellEventArgs e, int nMouseActionType)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                m_frmSearch.Cell = null;
                labelMemberInfo.Text = "";
                return;
            }

            if (e.RowIndex == dataGridControlRoomSchedule.Rows.Count)
            {
                m_frmSearch.Cell = null;
                labelMemberInfo.Text = "";
                return;
            }

            DataGridViewCell cell = dataGridControlRoomSchedule.Rows[e.RowIndex].Cells[e.ColumnIndex];
            m_frmSearch.Cell = cell;

            switch (nMouseActionType)
            {
                case 1:
                    ShowSearchForm(m_frmSearch);
                    break;
                case 2:
                    RefreshCell(cell);
                    break;
            }

        }

        #endregion Grid 제어실 이벤트


        #region Grid 방재센터 이벤트

        private void dataGridFireCenterRoomSchedule_MouseAction(object sender, DataGridViewCellEventArgs e, int nMouseActionType)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                m_frmSearchExternal.Cell = null;
                labelExternalMemberInfo.Text = "";
                return;
            }

            if (e.RowIndex == dataGridFireCenterRoomSchedule.Rows.Count)
            {
                m_frmSearchExternal.Cell = null;
                labelExternalMemberInfo.Text = "";
                return;
            }

            DataGridViewCell cell = dataGridFireCenterRoomSchedule.Rows[e.RowIndex].Cells[e.ColumnIndex];
            m_frmSearchExternal.Cell = cell;

            switch (nMouseActionType)
            {
                case 1:
                    ShowSearchForm(m_frmSearchExternal);
                    break;
                case 2:
                    RefreshCell(cell);
                    break;
            }
        }

        #endregion Grid 방재센터 이벤트


        #region Grid 당직실 이벤트

        private void dataGridDuty_MouseAction(object sender, DataGridViewCellEventArgs e, int nMouseAction)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                labelDutyInfo.Text = "";
                m_frmSearch.Cell = null;
                return;
            }

            DataGridViewCell cell = dataGridDuty.Rows[e.RowIndex].Cells[e.ColumnIndex];
            m_frmSearch.Cell = cell;

            switch (nMouseAction)
            {
                case 1:
                    ShowSearchForm(m_frmSearch);
                    break;
                case 2:
                    RefreshCell(cell);
                    break;
            }
        }

        #endregion Grid 당직실 이벤트


        #region ComboBox 현재 근무조 이벤트

        private void SetGridData(DataGridView grid, DataControlTeam team)
        {
            // Data를 Grid에 출력
            DataManagerToGrid(grid, team);
            // Grid의 태그에 활성화 근무조 객체를 넣음
            grid.Tag = team;
            // Grid의 Label 컨트롤에 정상조원 이름을 적용
            SetLabel(grid);
            // 현재 근무조를 저장
            SaveDataWorkingTeam(grid, team);
        }

        private void cboSchedule_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadWorkScheduleMember();
        }

        private void cboFireSchedule_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadFireWorkScheduleMember();   
        }

        private void LoadWorkScheduleMember()
        {
            if (cboSchedule.SelectedItem is DataControlTeam)
            {
                SetGridData(dataGridControlRoomSchedule, (cboSchedule.SelectedItem as DataControlTeam));
            }
        }

        private void LoadFireWorkScheduleMember()
        {
            if (cboFireSchedule.SelectedItem is DataControlTeam)
            {
                SetGridData(dataGridFireCenterRoomSchedule, (cboFireSchedule.SelectedItem as DataControlTeam));
            }
        }

        #endregion ComboBox 현재 근무조 이벤트


        #region ContextMenu 기본 근무직원적용 이벤트

        /// <summary>
        /// 기본 근무직원으로 적용 가능한 Cell에 대해서만 메뉴 활성화
        /// </summary>
        private void cmsMain_Opening(object sender, CancelEventArgs e)
        {
            DataGridView grid = (((ContextMenuStrip)sender)).SourceControl as DataGridView;

            Point pt = grid.PointToClient(MousePosition);

            System.Windows.Forms.DataGridView.HitTestInfo info = grid.HitTest(pt.X, pt.Y);

            if (info.ColumnIndex < 0 || info.RowIndex < 0)
            {
                e.Cancel = true;
                return;
            }

            DataGridViewCell cell = grid.Rows[info.RowIndex].Cells[info.ColumnIndex];

            if (m_dicCellLable[cell].Tag == null)
            {
                e.Cancel = true;
                return;
            }

            DataControlRoom room = cell.OwningColumn.Tag as DataControlRoom;
            DataControlTeam team = cell.DataGridView.Tag as DataControlTeam;
            DataControlTeamJobPosition job = cell.OwningRow.Tag as DataControlTeamJobPosition;

            DataControlTeamMember cellMember = m_dataMgr.GetControlTeamMember(room, team, job);
            DataControlTeamMember cellBasicMember = m_dicCellLable[cell].Tag as DataControlTeamMember;

            if (cellMember.Member != null && cellMember.Member.ID == cellBasicMember.Member.ID)
            {
                e.Cancel = true;
                return;
            }

            cell.Selected = true;
        }
        
        /// <summary>
        /// 기본 근무직원으로 적용
        /// </summary>
        private void cmiInit_Click(object sender, EventArgs e)
        {
            DataGridView grid = (((sender as ToolStripMenuItem).GetCurrentParent() as ContextMenuStrip).SourceControl as DataGridView);
            DataGridViewCell cell = grid.SelectedCells[0];
            cell.Value = (m_dicCellLable[cell].Tag as DataControlTeamMember).Member;

            RefreshCell(cell);
            SaveData(cell);
        }

        #endregion ContextMenu 기본 근무직원적용 이벤트


        public DataGridViewRow MakeNewRow(DataGridView grid)
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

        public string GetMemberFullPath(DataCompanyMember member)
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
            {
                label = labelMemberInfo;
            }
            else if (cell.DataGridView == dataGridFireCenterRoomSchedule)
            {
                label = labelExternalMemberInfo;
            }
            else if (cell.DataGridView == dataGridDuty)
            {
                label = labelDutyInfo;
            }
            else
            {
                return;
            }

            if (cell.Value != null && cell.Value is DataCompanyMember)
            {
                label.Text = GetMemberFullPath((DataCompanyMember)cell.Value);
            }
            else
            {
                label.Text = "";
            }

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

        private void btnShowBasicWorkMember_Click(object sender, EventArgs e)
        {
            if (m_frmBasicWorkMember == null
                || m_frmBasicWorkMember.IsDisposed == true)
            {
                m_frmBasicWorkMember = new FormWorkSchedule2(m_nSiteID);
                m_frmBasicWorkMember.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

                m_frmBasicWorkMember.MemberWorkDataChanged += (s, eArgs) =>
                {
                    AfterSaveData();
                    m_isBasicMemberChanged = true;
                };
                m_frmBasicWorkMember.FormClosed += (s, eArgs) =>
                {
                    if (m_isBasicMemberChanged == true)
                    {
                        m_isBasicMemberChanged = false;

                        m_isDataLoading = true;

                        foreach (Label label in m_dicCellLable.Values)
                        {
                            if (label != null && label.IsDisposed == false)
                            {
                                label.Dispose();
                            }
                        }

                        m_dicCellLable.Clear();

                        m_dataMgr.LoadData();

                        SetControlRoom();
                        SetFireCenterRoom();
                        SetDutyRoom();

                        m_isDataLoading = false;
                    }
                };
            }

            m_frmBasicWorkMember.ShowInTaskbar = false;
            m_frmBasicWorkMember.StartPosition = FormStartPosition.Manual;
            m_frmBasicWorkMember.Location = this.Location;
            m_frmBasicWorkMember.Size = this.Size;
            if (m_frmBasicWorkMember.Visible == false)
            {
                m_frmBasicWorkMember.Show(this.Owner);
            }
            else
            {
                m_frmBasicWorkMember.TopMost = true;
            }

            //this.btnCancel.PerformClick();
        }

        #endregion ETC


    }
}
