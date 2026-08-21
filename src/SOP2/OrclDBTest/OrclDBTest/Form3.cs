using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace OrclDBTest
{
    public partial class Form3 : Form
    {
        private OracleManager m_insaMgr = null;
        private SMSManager m_smsMgr = new SMSManager();
        private ArrayList m_arrTeams = new ArrayList();
        private ArrayList m_arrTeamMembers = new ArrayList();

        public Form3()
        {
            char[] arrID = new char[] { 'i', 'n', 's', 'a', '_', 'u', 's', 'e', 'r' };
            char[] arrPW = new char[] { 'i', 'n', 's', 'a', '1', '2', '3' };

            m_insaMgr = new OracleManager(new string(arrID), new string(arrPW), "ORA8");

            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            if (!m_insaMgr.OpenConnection())
                return;
            if (!m_smsMgr.OpenConnection())
                return;

            if (!LoadRegularTeam())
                return;

            if (!LoadRegularTeamMember())
                return;

            InitTeamGrid();
            InitTeamMemberGrid();
        }

        private bool LoadRegularTeam()
        {
            return m_insaMgr.LoadTeamList(m_arrTeams);
        }

        private bool LoadRegularTeamMember()
        {
            return m_insaMgr.LoadCompanyMemberList(m_arrTeamMembers, m_arrTeams);
        }

        private void InitTeamGrid()
        {
            foreach (RegularTeam team in m_arrTeams)
            {
                DataGridViewRow row = new DataGridViewRow();

                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = team.TeamCode;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = team.ParentTeamCode;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = team.TeamName;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = team.TeamID;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = team.TeamManager;
                row.Cells.Add(cell);

                teamDataGridView.Rows.Add(row);
            }
        }

        private void InitTeamMemberGrid()
        {
            foreach (RegularTeamMember member in m_arrTeamMembers)
            {
                DataGridViewRow row = new DataGridViewRow();

                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = member.EMPNO;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();

                if (member.Team != null)
                    cell.Value = member.Team.TeamCode;
                else
                    cell.Value = "";

                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = member.LEVELNO;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = member.MailAddress;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = member.NAME;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = member.TelNo;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = member.HandPhoneNumber;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = member.Title;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = member.IsTeamLeader ? "Y" : "N";
                row.Cells.Add(cell);

                memberDataGridView.Rows.Add(row);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormSMS frm = new FormSMS(m_smsMgr);
            frm.ShowDialog();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();

            dlg.Filter = "TXT Files|*.txt|All FIles|*.*";
            dlg.FilterIndex = 0;
            dlg.Title = "TXT 파일로 저장";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.IO.StreamWriter writer = new System.IO.StreamWriter(dlg.FileName, false, System.Text.Encoding.UTF8);
                WriteTeamDatas(writer);
                WriteMemberDatas(writer);
                writer.Close();
            }
        }

        private void WriteTeamDatas(System.IO.StreamWriter writer)
        {
            writer.WriteLine("[부서 Data]");
            writer.WriteLine("부서코드\t상위코드\t부서명\t부서직제\t부서명");

            foreach (DataGridViewRow row in teamDataGridView.Rows)
            {
                string strTeamCode = (string)row.Cells[0].Value;
                string strParentTeamCode = (string)row.Cells[1].Value;
                string strTeamName = (string)row.Cells[2].Value;
                string strTeamOrder = (string)row.Cells[3].Value;
                string strTeamLeader = (string)row.Cells[4].Value;

                writer.Write(strTeamCode + "\t");
                writer.Write(strParentTeamCode + "\t");
                writer.Write(strTeamName + "\t");
                writer.Write(strTeamOrder + "\t\t");
                writer.WriteLine(strTeamLeader);
            }
        }

        private void WriteMemberDatas(System.IO.StreamWriter writer)
        {
            writer.WriteLine("[직원 Data]");
            writer.WriteLine("사번\t부서코드\t직급\t메일주소\t이름\t전화번호\t핸드폰\t직책\t부서장여부");

            foreach (DataGridViewRow row in memberDataGridView.Rows)
            {
                string strMemberID = (string)row.Cells[0].Value;
                string strTeamCode = (string)row.Cells[1].Value;
                string strLevelID = (string)row.Cells[2].Value;
                string strMailAddr = (string)row.Cells[3].Value;
                string strName = (string)row.Cells[4].Value;
                string strPhoneNumber = (string)row.Cells[5].Value;
                string strMobileNumber = (string)row.Cells[6].Value;
                string strTitle = (string)row.Cells[7].Value;
                string strYN = (string)row.Cells[8].Value;

                writer.Write(strMemberID + "\t");
                writer.Write(strTeamCode + "\t");
                writer.Write(strLevelID + "\t");
                writer.Write(strMailAddr + "\t");
                writer.Write(strName + "\t");
                writer.Write(strPhoneNumber + "\t");
                writer.Write(strMobileNumber + "\t");
                writer.Write(strTitle + "\t");
                writer.WriteLine(strYN);
            }
        }
    }
}
