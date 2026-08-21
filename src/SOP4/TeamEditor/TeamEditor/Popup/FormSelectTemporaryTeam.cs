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
using System.Collections;

namespace TeamEditor.Popup
{
    public partial class FormSelectTemporaryTeam : Form
    {
        private bool m_isNormal = true;
        private Team m_teamSelected = null;

        public bool IsNormal
        {
            get { return m_isNormal; }
            set { m_isNormal = value; }
        }

        public Team SelectedTeam
        {
            get { return m_teamSelected; }
        }

        public FormSelectTemporaryTeam()
        {
            InitializeComponent();
        }

        public FormSelectTemporaryTeam(bool isNormal)
        {
            InitializeComponent();

            m_isNormal = isNormal;
        }

        private void FormSelectTemporaryTeam_Load(object sender, EventArgs e)
        {
            if (m_isNormal)
            {
                this.Text = "평일 비상조직 선택";
                LoadNormalTeam();
            }
            else
            {
                this.Text = "야간 및 휴일 비상조직 선택";
                LoadEmergencyTeam();
            }
        }

        private void LoadNormalTeam()
        {
            List<TemporaryNormalTeam> teams = DataManager.GetTemporaryNormalRootTeams();

            foreach (TemporaryNormalTeam team in teams)
            {
                AddTeam(team);
            }
        }

        private void LoadEmergencyTeam()
        {
            List<TemporaryEmergencyTeam> teams = DataManager.GetTemporaryEmergencyRootTeams();

            foreach (TemporaryEmergencyTeam team in teams)
            {
                AddTeam(team);
            }
        }

        private void AddTeam(Team team)
        {
            DataGridViewRow row = new DataGridViewRow();
            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            cell.Value = team.TeamName;
            row.Cells.Add(cell);
            row.Tag = team;
        }

        private void checkBoxEditMode_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxEditMode.Checked)
            {
                dataGridView1.ReadOnly = false;
                dataGridView1.AllowUserToAddRows = true;
            }
            else
            {
                dataGridView1.ReadOnly = true;
                dataGridView1.AllowUserToAddRows = false;
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count == 0)
            {
                MessageBox.Show("팀을 선택해 주세요.");
                return;
            }

            int nRowIndex = dataGridView1.SelectedCells[0].RowIndex;
            DataGridViewRow row = dataGridView1.Rows[nRowIndex];

            if (row.IsNewRow)
            {
                MessageBox.Show("팀을 선택해 주세요.");
                return;
            }

            if (row.Tag == null)
            {
                string strTeamName = row.Cells[0].Value.ToString();

                m_teamSelected = null;
                List<TemporaryNormalTeam> teams = DataManager.GetTemporaryNormalRootTeams();

                foreach (TemporaryNormalTeam normalTeam in teams)
                {
                    if (normalTeam.TeamName == strTeamName)
                    {
                        m_teamSelected = normalTeam;
                        break;
                    }
                }

                if (m_teamSelected == null)
                {
                    TemporaryNormalTeam team = new TemporaryNormalTeam();
                    team.TeamName = strTeamName;

                    if (team.TeamName.Length == 0)
                    {
                        MessageBox.Show("팀을 선택해 주세요.");
                        return;
                    }

                    m_teamSelected = team;
                    teams.Add(team);
                }
            }
            else
                m_teamSelected = (Team)row.Tag;

            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
