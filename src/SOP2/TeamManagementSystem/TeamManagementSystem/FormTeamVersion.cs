using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace TeamManagementSystem
{
    public partial class FormTeamVersion : Form
    {
        private FormMain m_Main = null;
        
        private ArrayList m_arrVersionData = new ArrayList();


        public FormTeamVersion()
        {
            InitializeComponent();
        }

        public void SetMain(FormMain main)
        {
            m_Main = main;
        }

        public void GetVersionInfo()
        {
            if (m_Main == null) return;

            ArrayList arrVersion = m_Main.TeamVersion;
            if (arrVersion == null) return;

            dataGridViewVersion.Rows.Clear();
            foreach (Data_TeamVersion data in arrVersion)
            {
                DataGridViewRow row = new DataGridViewRow();
                DataGridViewCell cell = new DataGridViewTextBoxCell();

                cell.Value = data.VersionID;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = data.VersionName;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = data.UserName;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = data.CreateTime;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                if (data.Description == "null")
                    cell.Value = "";
                else
                    cell.Value = data.Description;
                row.Cells.Add(cell);

                dataGridViewVersion.Rows.Add(row);                
            }
        }

        private void btnNewProject_Click(object sender, EventArgs e)
        {
            m_Main.VersionName = "";

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            //버전 선택
            m_Main.VersionName = dataGridViewVersion.SelectedCells[1].Value.ToString();
            m_Main.VersionID = int.Parse(dataGridViewVersion.SelectedCells[0].Value.ToString());
            
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void FormTeamVersion_Load(object sender, EventArgs e)
        {
            GetVersionInfo();
        }

        private void dataGridViewVersion_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = new DataGridViewTextBoxCell();
            foreach (DataGridViewRow row in dataGridViewVersion.Rows)
            {
                string str = row.Cells[0].Value.ToString();
                string str1 = row.Cells[1].Value.ToString();
                string str2 = row.Cells[2].Value.ToString();
                string str3 = row.Cells[3].Value.ToString();
                string str4 = row.Cells[4].Value.ToString();
            }
            
        }

        private void dataGridViewVersion_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            btnLoad_Click(sender, e);
        }

        private void dataGridViewVersion_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnLoad_Click(sender, e);
            }
        }

    }
}
