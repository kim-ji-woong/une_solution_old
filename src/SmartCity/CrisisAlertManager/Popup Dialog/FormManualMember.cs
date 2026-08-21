using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrisisAlertManager.Popup_Dialog
{
    public partial class FormManualMember : Form
    {
        FormManualInfo m_FormManualMember = null;

        Dictionary<int, DataTeam> m_dicRegularTeams = null;                                                 // 소속 리스트
        Dictionary<int, DataTeam> m_dicSubTeams = null;                                                     // 부서 리스트
        Dictionary<int, DataCompanyMember> m_dicCompanyMembers = null;                                      // 팀원
        Dictionary<int, DataCompanyMember> m_dicCheckCompanyMembers = null;                                 // 체크 팀원 리스트

        List<DataCompanyMember> m_listManagers = null;                                              // 수신자 리스트
        List<DataCompanyMember> m_listCheckManagers = new List<DataCompanyMember>();                // 체크 수신자 리스트

        List<DataCompanyMember> m_listAddManagers = new List<DataCompanyMember>();                          // 추가된 수신자 리스트
        List<DataCompanyMember> m_listRemoveManagers = new List<DataCompanyMember>();                       // 제거된 수신자 리스트

        public FormManualMember(string strManualMember, FormManualInfo parent)
        {
            InitializeComponent();

            Region = System.Drawing.Region.FromHrgn(FormMain.CreateRoundRectRgn(0, 0, this.Width, this.Height, 10, 10));

            m_FormManualMember = parent;

            // 전파 대상자 불러오기
            LoadManagers(strManualMember);

            // 대상자 추가 그리드 표시
            LoadGridMainTeam();
        }

        #region 폼 이동
        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();
        private bool m_isClicked = false;
        private Point m_ptOrigin = new Point();

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = Control.MousePosition;
                m_ptOrigin = this.Location;
            }

            m_isClicked = true;
        }

        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            if (!m_isClicked)
                return;

            if (!m_bLeftMouseDown)
                return;

            Point ptScreen = Control.MousePosition;

            int dx = ptScreen.X - m_ptMove.X;
            int dy = ptScreen.Y - m_ptMove.Y;

            if (dx == 0 && dy == 0)
                return;

            Point ptCur = this.Location;
            this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
            m_ptMove.X += dx;
            m_ptMove.Y += dy;
        }

        private void Form_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;

            m_isClicked = false;
        }
        #endregion

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
        }

        private void LoadManagers(string strManualMember)
        {
            m_listManagers = new List<DataCompanyMember>();
            gridManager.Rows.Clear();

            m_dicCompanyMembers = FormMain.Instance.DataManager.CompanyMembers;

            if (strManualMember == null || strManualMember == "")
                return;

            string[] arrManualMember = strManualMember.Split(',');
            int nCount = arrManualMember.Length;

            for (int i = 0; i < nCount; i++)
            {
                string strMember = arrManualMember[i];
                strMember = strMember.Trim();

                int nMemberID = Int32.Parse(strMember);

                if (m_dicCompanyMembers.ContainsKey(nMemberID))
                {
                    DataCompanyMember member = m_dicCompanyMembers[nMemberID];
                    m_listManagers.Add(member);
                }
            }

            foreach (DataCompanyMember manager in m_listManagers)
            {
                int nRowIndex = gridManager.Rows.Add();
                string strColInfo = "";

                strColInfo = manager.Level.LevelName + " " + manager.MemberName;

                gridManager.Rows[nRowIndex].Cells[colInfo.Index].Value = strColInfo;
                gridManager.Rows[nRowIndex].Tag = manager;
            }
        }

        private void LoadGridMainTeam()
        {
            gridMainTeam.Rows.Clear();

            m_dicRegularTeams = new Dictionary<int, DataTeam>();
            m_dicRegularTeams = FormMain.Instance.DataManager.GetMainTeams();

            foreach (KeyValuePair<int, DataTeam> item in m_dicRegularTeams)
            {
                DataTeam data = item.Value;

                int nRowIndex = gridMainTeam.Rows.Add();
                gridMainTeam.Rows[nRowIndex].Cells[colTeamName.Index].Value = data.TeamName;
                gridMainTeam.Rows[nRowIndex].Tag = data;
            }

            if (m_dicRegularTeams.Count > 0)
            {
                DataTeam data = (DataTeam)gridMainTeam.Rows[0].Tag;

                if (data.ID == -1)
                    return;

                LoadGridSubTeam(data.ID);
            }
        }

        private void LoadGridSubTeam(int nParentID)
        {
            gridSubTeam.Rows.Clear();
            lbSubTeam.Text = "";

            m_dicSubTeams = new Dictionary<int, DataTeam>();
            m_dicSubTeams = FormMain.Instance.DataManager.GetSubTeams(nParentID);

            if (m_dicSubTeams == null)
                return;

            foreach (KeyValuePair<int, DataTeam> item in m_dicSubTeams)
            {
                DataTeam data = item.Value;

                int nRowIndex = gridSubTeam.Rows.Add();
                gridSubTeam.Rows[nRowIndex].Cells[colSubTeamName.Index].Value = data.TeamName;
                gridSubTeam.Rows[nRowIndex].Tag = data;
            }

            if (m_dicSubTeams.Count > 0)
            {
                DataTeam data = (DataTeam)gridSubTeam.Rows[0].Tag;

                if (data.ID == -1)
                    return;

                lbSubTeam.Text = data.TeamName;

                LoadGridMember(data.ID);
            }
        }

        private void LoadGridMember(int nTeamID)
        {
            gridMember.Rows.Clear();
            m_dicCheckCompanyMembers = new Dictionary<int, DataCompanyMember>();

            m_dicCompanyMembers = new Dictionary<int, DataCompanyMember>();
            m_dicCompanyMembers = FormMain.Instance.DataManager.GetCompanyMembers(nTeamID);

            if (m_dicCompanyMembers == null)
                return;


            foreach (KeyValuePair<int, DataCompanyMember> item in m_dicCompanyMembers)
            {
                DataCompanyMember data = item.Value;

                int nRowIndex = gridMember.Rows.Add();
                if (data.Level != null)
                    gridMember.Rows[nRowIndex].Cells[colName.Index].Value = data.Level.LevelName + " " + data.MemberName;
                else
                    gridMember.Rows[nRowIndex].Cells[colName.Index].Value = data.MemberName;
                gridMember.Rows[nRowIndex].Tag = data;

                // 담당자 리스트에 포함된다면 체크, 체크된 리스트에 추가
                foreach (DataCompanyMember manager in m_listManagers)
                {
                    if (data.ID == manager.ID)
                    {
                        gridMember.Rows[nRowIndex].Cells[colCheck.Index].Value = true;
                        m_dicCheckCompanyMembers[manager.ID] = manager;
                    }
                }
            }
        }

        private void gridMainTeam_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            gridSubTeam.Rows.Clear();
            gridMember.Rows.Clear();

            DataTeam data = new DataTeam();

            foreach (DataGridViewCell cell in gridMainTeam.SelectedCells)
            {
                if (cell.RowIndex < 0)
                    continue;

                DataGridViewRow row = gridMainTeam.Rows[cell.RowIndex];
                if (row.Tag == null)
                    continue;
                else
                {
                    data = (DataTeam)row.Tag;
                    break;
                }
            }

            if (data.ID == -1)
                return;

            LoadGridSubTeam(data.ID);
        }

        private void gridSubTeam_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataTeam data = new DataTeam();
            lbSubTeam.Text = "";

            foreach (DataGridViewCell cell in gridSubTeam.SelectedCells)
            {
                if (cell.RowIndex < 0)
                    continue;

                DataGridViewRow row = gridSubTeam.Rows[cell.RowIndex];
                if (row.Tag == null)
                    continue;
                else
                {
                    data = (DataTeam)row.Tag;
                    break;
                }
            }

            if (data.ID == -1)
                return;

            lbSubTeam.Text = data.TeamName;

            LoadGridMember(data.ID);
        }

        private void gridMember_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            // 그리드 셀의 체크박스 값 변화를 감지하기 위한 이벤트 핸들러
            if (gridMember.IsCurrentCellDirty)
            {
                gridMember.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void gridMember_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // 체크 유무를 확인하여 
            if (gridMember.Columns[e.ColumnIndex].Name == "colCheck")
            {
                DataGridViewRow row = gridMember.Rows[e.RowIndex];
                string strCheck = row.Cells[colCheck.Index].Value.ToString();

                if (strCheck == "True")
                {   // 체크시 체크 팀원 리스트에 저장
                    if (row.Tag == null)
                        return;

                    DataCompanyMember companyMember = (DataCompanyMember)row.Tag;
                    m_dicCheckCompanyMembers[companyMember.ID] = companyMember;
                }
                else if (strCheck == "False")
                {   // 체크 해제시 체크 팀원 리스트에서 제거
                    if (row.Tag == null)
                        return;

                    DataCompanyMember companyMember = (DataCompanyMember)row.Tag;
                    m_dicCheckCompanyMembers.Remove(companyMember.ID);
                }
            }
        }

        private void gridManager_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {   // 그리드 셀의 체크박스 값 변화를 감지하기 위한 이벤트 핸들러

            // 체크 유무를 확인하여 
            if (gridManager.Columns[e.ColumnIndex].Name == "colManagerCheck")
            {
                DataGridViewRow row = gridManager.Rows[e.RowIndex];
                string strCheck = row.Cells[colManagerCheck.Index].Value.ToString();

                if (strCheck == "True")
                {   // 체크시 체크 담당자 리스트에 저장
                    if (row.Tag == null)
                        return;

                    DataCompanyMember manager = (DataCompanyMember)row.Tag;
                    m_listCheckManagers.Add(manager);
                }
                else if (strCheck == "False")
                {   // 체크 해제시 체크 담당자 리스트에서 제거
                    if (row.Tag == null)
                        return;

                    DataCompanyMember manager = (DataCompanyMember)row.Tag;
                    m_listCheckManagers.Remove(manager);
                }

            }
        }

        private void gridManager_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {   // 그리드 셀의 체크박스 값 변화를 감지하기 위한 이벤트 핸들러
            if (gridManager.IsCurrentCellDirty)
            {
                gridManager.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void btnAddManager_Click(object sender, EventArgs e)
        {
            // 체크 팀원 리스트를 추가하는 방식
            if (m_dicCheckCompanyMembers.Count == 0)
                return;

            foreach (KeyValuePair<int, DataCompanyMember> pair in m_dicCheckCompanyMembers)
            {
                DataCompanyMember data = pair.Value;

                if (FindGridManager(data))
                    continue;

                // 그리드 표시
                int nRowIndex = gridManager.Rows.Add();
                if (data.Level != null)
                    gridManager.Rows[nRowIndex].Cells[colInfo.Index].Value = data.Level.LevelName + " " + data.MemberName;
                else
                    gridManager.Rows[nRowIndex].Cells[colInfo.Index].Value = data.MemberName;
                gridManager.Rows[nRowIndex].Tag = data;

                m_listManagers.Add(data);

                // 추가 또는 제거 담당자 리스트 관리
                if (m_listRemoveManagers.Contains(data))
                    m_listRemoveManagers.Remove(data);
                else
                    m_listAddManagers.Add(data);
            }
        }

        private bool FindGridManager(DataCompanyMember manager)
        {
            bool bRet = false;

            for (int i = 0; i < gridManager.Rows.Count; i++)
            {
                DataGridViewRow row = gridManager.Rows[i];

                if (row.Tag == null)
                    continue;

                DataCompanyMember data = (DataCompanyMember)row.Tag;

                if (data.ID == manager.ID)
                    bRet = true;
            }

            return bRet;
        }

        private void btnRemoveManager_Click(object sender, EventArgs e)
        {
            if (m_listCheckManagers.Count == 0)
                return;

            List<DataCompanyMember> listCheckManagers = new List<DataCompanyMember>();

            foreach (DataCompanyMember item in m_listCheckManagers)
            {
                listCheckManagers.Add(item);
            }

            // 체크 담당자 리스트를 제거하는 방식
            foreach (DataCompanyMember manager in listCheckManagers)
            {
                for (int nRowIndex = 0; nRowIndex < gridManager.Rows.Count; nRowIndex++)
                {
                    if (gridManager.Rows[nRowIndex].Tag == manager)
                    {
                        // 추가 또는 제거 담당자 리스트 관리
                        if (m_listAddManagers.Contains(manager))
                            m_listAddManagers.Remove(manager);
                        else
                            m_listRemoveManagers.Add(manager);

                        // 그리드 표시
                        gridManager.Rows.RemoveAt(nRowIndex);
                        m_listManagers.Remove(manager);
                        m_listCheckManagers.Remove(manager);
                        break;
                    }
                }
            }
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            string strManualMember = "";

            foreach (DataCompanyMember member in m_listManagers)
            {
                if (strManualMember == "")
                    strManualMember += member.ID;
                else
                    strManualMember += ", " + member.ID;
            }

            m_FormManualMember.ManualMember = strManualMember;
            this.DialogResult = DialogResult.Yes;
        }

        private void cbMember_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox check = sender as CheckBox;
            if (check == null)
                return;

            foreach (DataGridViewRow row in gridMember.Rows)
            {
                row.Cells[colCheck.Index].Value = check.Checked;
            }
        }

        private void cbManager_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox check = sender as CheckBox;
            if (check == null)
                return;

            foreach (DataGridViewRow row in gridManager.Rows)
            {
                row.Cells[colManagerCheck.Index].Value = check.Checked;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
        }
    }
}
