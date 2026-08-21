using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ControlTeamEditor
{
    public partial class FormSearchExternalMember : Form
    {
        public event EventHandler ExternalMemberSelected;

        private DataManager m_dataMgr = null;
        private DataGridViewCell m_cell = null;
        private IMainForm m_frmMain = null;

        public DataGridViewCell Cell
        {
            get { return m_cell; }
            set { m_cell = value; }
        }

        public FormSearchExternalMember(DataManager dataMgr, IMainForm frmMain)
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            FormMain.SetDoubleBuffer(dataGridMemberList, true);

            m_dataMgr = dataMgr;
            m_frmMain = frmMain;
            //Init();
        }

        public void Init()
        {
            int nColumnCount = dataGridMemberList.Columns.Count;

            for (int i = 0; i < nColumnCount; i++)
            //foreach (DataGridViewColumn column in dataGridMemberList.Columns)
            {
                DataGridViewColumn column = dataGridMemberList.Columns[i];
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            List<DataCompanyMember> members = m_dataMgr.SearchExternalCompanyMembers(textBoxName.Text);
            SetData(members);
        }

        private void SetData(List<DataCompanyMember> members)
        {
            dataGridMemberList.Rows.Clear();

            if (members == null)
                return;

            int nMemberCount = members.Count;

            for (int i = 0; i < nMemberCount; i++)
            {
                DataCompanyMember member = members[i];

                if (member.TeamPositions.Count == 0)
                    continue;

                // 첫번째 팀 정보를 얻어온다.
                KeyValuePair<DataTeam, JobPosition> pair = member.TeamPositions.ElementAt(0);
                string strTeamFullPath = FormSearchMember.GetTeamFullPath(pair.Key);

                DataGridViewRow row = FormWorkSchedule.MakeNewRow(dataGridMemberList);
                row.HeaderCell.Value = (i + 1).ToString();

                row.Cells[0].Value = strTeamFullPath;
                row.Cells[1].Value = member.MemberName;
                row.Cells[2].Value = pair.Value.SubPositionName;

                row.Tag = member;
            }

            members.Clear();
        }

        private void FormSearchExternalMember_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!m_frmMain.CloseApplication)
            {
                this.Hide();
                e.Cancel = true;
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (m_cell == null || dataGridMemberList.SelectedCells.Count == 0)
                return;

            DataGridViewRow row = dataGridMemberList.Rows[dataGridMemberList.SelectedCells[0].RowIndex];
            SelectRow(row);
        }

        private void SelectRow(DataGridViewRow row)
        {
            if (row.Tag == null)
                return;

            if (row.Tag is DataCompanyMember)
            {
                m_cell.Value = (DataCompanyMember)row.Tag;
                m_frmMain.RefreshCell(m_cell);

                if (ExternalMemberSelected != null)
                    ExternalMemberSelected(m_cell, null);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void textBoxName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                btnSearch_Click(null, null);
            }
        }

        private void dataGridMemberList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || m_cell == null)
                return;

            DataGridViewRow row = dataGridMemberList.Rows[e.RowIndex];
            SelectRow(row);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.btnClose.PerformClick();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

    }
}
