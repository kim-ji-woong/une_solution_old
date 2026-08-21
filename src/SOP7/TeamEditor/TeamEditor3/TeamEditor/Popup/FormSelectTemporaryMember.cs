using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TeamEditor.Popup
{
    public partial class FormSelectTemporaryMember : Form
    {
        private FormRegularMember m_frmRegularMember = null;
        private FormExternalMember m_frmExternalMember = null;
        private FormUserDefinedTeam m_frmUserDefinedTeam = null;

        private TeamTreeView m_treeRegularTeam = null;
        private TeamTreeView m_treeNormal = null;
        private TeamTreeView m_treeEmergency = null;
        private TeamTreeView m_treeExternalCompanyTeam = null;

        private RadioButton m_prevSelectedRadio = null;
        private object m_selectedTeam = null;
        private object m_selectedMember = null;
        private TemporaryMember.MemberType m_selectedMemberType = TemporaryMember.MemberType.None;

        Int32 iGrdViewHeight = 35;

        public TemporaryMember.MemberType SelectedMemberType
        {
            get { return m_selectedMemberType; }
        }

        public object SelectedTeam
        {
            get { return m_selectedTeam; }
            set { m_selectedTeam = value; }
        }

        public object SelectedMember
        {
            get { return m_selectedMember; }
            set { m_selectedMember = value; }
        }

        private double m_WindowRateWidth = 1;
        public double WindowRateWidth
        {
            get { return m_WindowRateWidth; }
            set { m_WindowRateWidth = value; }
        }

        private double m_WindowRateHeight = 1;
        public double WindowRateHeight
        {
            get { return m_WindowRateHeight; }
            set { m_WindowRateHeight = value; }
        }

        public FormSelectTemporaryMember(TeamTreeView treeRegularTeam, TeamTreeView treeNormal, TeamTreeView treeEmergency, TeamTreeView treeExternalCompanyTeam)//, TeamGrid gridRegular, TeamGrid gridExternal, TeamGrid gridUserDefine)
        {
            InitializeComponent();
            
            m_treeRegularTeam = treeRegularTeam;
            m_treeNormal = treeNormal;
            m_treeEmergency = treeEmergency;
            m_treeExternalCompanyTeam = treeExternalCompanyTeam;

            // 협력업체 및 사용자 정의조직 편집기능이 정의되어있지 않아 감춘다.
            // 구현 완료.
            /*radioExternalCompanyMember.Visible = radioExternalCompanyTeam.Visible = radioUserDefinedTeam.Visible = false;*/
            //radioLevelID.Location = radioExternalCompanyTeam.Location;

            //Init(gridRegular, gridExternal, gridUserDefine);
        }

        public void Init(TeamGrid gridRegular, TeamGrid gridExternal, TeamGrid gridUserDefine)
        {
            SetLevelID();

            m_frmRegularMember = new FormRegularMember(this, gridRegular);
            m_frmRegularMember.TopLevel = false;
            m_frmRegularMember.Dock = DockStyle.Fill;
            m_frmRegularMember.WindowRateWidth = WindowRateWidth;
            m_frmRegularMember.WindowRateHeight = WindowRateHeight;
            m_frmRegularMember.UpdateControl();
            m_frmRegularMember.Show();

            m_frmExternalMember = new FormExternalMember(this, gridExternal);
            m_frmExternalMember.TopLevel = false;
            m_frmExternalMember.Dock = DockStyle.Fill;
            m_frmExternalMember.WindowRateWidth = WindowRateWidth;
            m_frmExternalMember.WindowRateHeight = WindowRateHeight;
            m_frmExternalMember.UpdateControl();
            m_frmExternalMember.Show();

            m_frmUserDefinedTeam = new FormUserDefinedTeam(this, gridUserDefine);
            m_frmUserDefinedTeam.TopLevel = false;
            m_frmUserDefinedTeam.Dock = DockStyle.Fill;
            m_frmUserDefinedTeam.WindowRateWidth = WindowRateWidth;
            m_frmUserDefinedTeam.WindowRateHeight = WindowRateHeight;
            m_frmUserDefinedTeam.UpdateControl();
            m_frmUserDefinedTeam.Show();

            panelForms.Controls.Add(m_frmRegularMember);
            panelForms.Controls.Add(m_frmExternalMember);
            panelForms.Controls.Add(m_frmUserDefinedTeam);

            foreach (DataGridViewColumn column in gridLevelID.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            radio_CheckedChanged(null, null);            
        }

        public void UpdateControl()
        {
            FormMain.Instance.UpdateWindowRate(this, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(picRegularTeam, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(lblRegularTeam, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(picCompanyMember, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(lblCompanyMember, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(picExternalCompanyTeam, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(lblExternalCompanyTeam, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(picExternalCompanyMember, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(lblExternalCompanyMember, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(picLevelID, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(lblLevelID, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(picUserDefinedTeam, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(lblUserDefinedTeam, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(panelForms, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(gridLevelID, WindowRateWidth, WindowRateHeight);

            FormMain.Instance.UpdateWindowRate(btnSelect, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(btnClose, WindowRateWidth, WindowRateHeight);
        }

        private void SetLevelID()
        {
            gridLevelID.Rows.Clear();

            for (int i=1;i<=9;i++)
            {
                DataGridViewRow row = new DataGridViewRow();
                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();

                cell.Value = String.Format("{0} 전체", FormMain.Instance.GetLevelName(i)).Replace("급", "직급");
                row.Cells.Add(cell);
                gridLevelID.Rows.Add(row);

                row.Tag = i;
            }
        }

        private void radio_CheckedChanged(object sender, EventArgs e)
        {
            gridLevelID.Visible = false;
            m_frmRegularMember.Visible = m_frmExternalMember.Visible = m_frmUserDefinedTeam.Visible = false;

            if (radioRegularTeam.Checked)
            {
                picRegularTeam.BackgroundImage = global::TeamEditor.Properties.Resources.__SOPEDIT_Enable2;
                m_frmRegularMember.ShowGrid = false;
                m_frmRegularMember.Visible = true;

                m_frmRegularMember.ResetTeamPath();

                if (m_prevSelectedRadio != radioCompanyMember && m_prevSelectedRadio != radioRegularTeam)
                {
                    m_frmRegularMember.Update(m_treeRegularTeam);
                }

                m_prevSelectedRadio = radioRegularTeam;
                m_selectedMemberType = TemporaryMember.MemberType.RegularTeam;
                SelectedTeam = m_frmRegularMember.SelectedRegularTeam;
                SelectedMember = null;
            }
            else if (radioCompanyMember.Checked)
            {
                picCompanyMember.BackgroundImage = global::TeamEditor.Properties.Resources.__SOPEDIT_Enable2;
                m_frmRegularMember.ShowGrid = true;
                m_frmRegularMember.Visible = true;

                if (m_prevSelectedRadio != radioRegularTeam && m_prevSelectedRadio != radioCompanyMember)
                {
                    m_frmRegularMember.Update(m_treeRegularTeam);
                }

                m_prevSelectedRadio = radioCompanyMember;
                m_selectedMemberType = TemporaryMember.MemberType.CompanyMember;
                SelectedTeam = m_frmRegularMember.SelectedRegularTeam;
                SelectedMember = m_frmRegularMember.SelectedCompanyMember;

                m_frmRegularMember.CheckSelectedMember();
            }
            else if (radioExternalCompanyTeam.Checked)
            {
                picExternalCompanyTeam.BackgroundImage = global::TeamEditor.Properties.Resources.__SOPEDIT_Enable2;
                m_frmExternalMember.ShowGrid = false;
                m_frmExternalMember.Visible = true;

                m_frmExternalMember.ResetTeamPath();

                if (m_prevSelectedRadio != radioExternalCompanyMember && m_prevSelectedRadio != radioExternalCompanyTeam)
                    m_frmExternalMember.Update(m_treeExternalCompanyTeam);

                m_prevSelectedRadio = radioExternalCompanyTeam;
                m_selectedMemberType = TemporaryMember.MemberType.ExternalTeam;
                SelectedTeam = m_frmExternalMember.SelectedTeam;
                SelectedMember = null;
            }
            else if (radioExternalCompanyMember.Checked)
            {
                picExternalCompanyMember.BackgroundImage = global::TeamEditor.Properties.Resources.__SOPEDIT_Enable2;
                m_frmExternalMember.ShowGrid = true;
                m_frmExternalMember.Visible = true;

                if (m_prevSelectedRadio != radioExternalCompanyTeam && m_prevSelectedRadio != radioExternalCompanyMember)
                    m_frmExternalMember.Update(m_treeExternalCompanyTeam);

                m_prevSelectedRadio = radioExternalCompanyMember;
                m_selectedMemberType = TemporaryMember.MemberType.ExternalCompanyMember;

                SelectedTeam = m_frmExternalMember.SelectedTeam;
                SelectedMember = m_frmExternalMember.SelectedExternalCompanyMember;

                m_frmExternalMember.CheckSelectedMember();
            }
            else if (radioLevelID.Checked)
            {
                picLevelID.BackgroundImage = global::TeamEditor.Properties.Resources.__SOPEDIT_Enable2;
                m_prevSelectedRadio = radioLevelID;
                m_selectedTeam = null;
                m_selectedMemberType = TemporaryMember.MemberType.LevelID;
                gridLevelID_CellClick(null, null);
                gridLevelID.Visible = true;
            }
            else if (radioUserDefinedTeam.Checked)
            {
                picUserDefinedTeam.BackgroundImage = global::TeamEditor.Properties.Resources.__SOPEDIT_Enable2;
                m_prevSelectedRadio = radioUserDefinedTeam;
                m_selectedMemberType = TemporaryMember.MemberType.UserDefinedTeam;
                SelectedTeam = m_frmUserDefinedTeam.SelectedTeam;
                SelectedMember = null;
                m_frmUserDefinedTeam.Visible = true;
            }
        }

        private void FormSelectTemporaryMember_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!FormMain.Instance.CloseApplication)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void gridLevelID_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (gridLevelID.SelectedCells.Count == 0)
            {
                SelectedTeam = null;
                SelectedMember = null;
                return;
            }

            int nRowIndex = gridLevelID.SelectedCells[0].RowIndex;

            if (nRowIndex < 0)
            {
                SelectedTeam = null;
                SelectedMember = null;
                return;
            }

            DataGridViewRow row = gridLevelID.Rows[nRowIndex];

            if (row.IsNewRow)
            {
                SelectedTeam = null;
                SelectedMember = null;
            }
            else
            {
                SelectedTeam = null;
                SelectedMember = row.Tag;
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (ValidateForSelectionData())
                return;

            FormMain.Instance.SetTemporaryMember(SelectedTeam, SelectedMember);
            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool ValidateForSelectionData()
        {
            bool isError = false;

            if (radioRegularTeam.Checked || radioExternalCompanyTeam.Checked || radioCompanyMember.Checked || radioExternalCompanyMember.Checked || radioUserDefinedTeam.Checked)
            {
                if (SelectedTeam == null)
                {
                    isError = true;
                    //MessageBox.Show("부서를 선택하세요.", "부서 미지정", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UnE.Utility.UMessageBoxRibbon.Show("부서를 선택하세요.", "부서 미지정", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (radioCompanyMember.Checked || radioExternalCompanyMember.Checked)
                {
                    if (SelectedMember == null)
                    {
                        isError = true;
                        //MessageBox.Show("직원을 선택하세요.", "직원 미지정", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        UnE.Utility.UMessageBoxRibbon.Show("직원을 선택하세요.", "직원 미지정", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else if (radioLevelID.Checked)
            {
                if (SelectedMember == null)
                {
                    isError = true;
                    //MessageBox.Show("직급을 선택하세요.", "직급 미지정", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UnE.Utility.UMessageBoxRibbon.Show("직급을 선택하세요.", "직급 미지정", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            return isError;
        }

        private void Select_Menu(object sender, EventArgs e)
        {
            Control ctl = sender as Control;
            if (ctl == null) return;

            RadioButton rdo = this.Controls[ctl.Tag.ToString()] as RadioButton;
            if (rdo == null) return;

            picRegularTeam.BackgroundImage = global::TeamEditor.Properties.Resources.__SOPEDIT_Disable2;
            picCompanyMember.BackgroundImage = global::TeamEditor.Properties.Resources.__SOPEDIT_Disable2;
            picExternalCompanyTeam.BackgroundImage = global::TeamEditor.Properties.Resources.__SOPEDIT_Disable2;
            picExternalCompanyMember.BackgroundImage = global::TeamEditor.Properties.Resources.__SOPEDIT_Disable2;
            picLevelID.BackgroundImage = global::TeamEditor.Properties.Resources.__SOPEDIT_Disable2;
            picUserDefinedTeam.BackgroundImage = global::TeamEditor.Properties.Resources.__SOPEDIT_Disable2;

            rdo.Checked = true;           

            radio_CheckedChanged(rdo, null);
        }

        private void gridLevelID_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DataGridView gdv = sender as DataGridView;
            if (gdv == null) return;            

            foreach (DataGridViewRow row in gdv.Rows)
            {
                row.MinimumHeight = iGrdViewHeight;
            }
        }

    }
}
