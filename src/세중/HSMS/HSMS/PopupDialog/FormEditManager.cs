using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HSMS
{
    public partial class FormEditManager : Form
    {
        private ArrayList m_arManagers = new ArrayList();
        public ArrayList Managers
        {
            get { return m_arManagers; }
        }

        public void AddManager(DataWorker worker)
        {
            if (!m_arManagers.Contains(worker))
                m_arManagers.Add(worker);
        }

        public FormEditManager()
        {
            InitializeComponent();
        }

       

        private void FormEditManager_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void FormEditManager_Load(object sender, EventArgs e)
        {
            SetWorkerTreeNode();

            SetGridManager();
        }

        private bool CheckData(DataWorker worker)
        {
            foreach (DataGridViewRow row in gridManager.Rows)
            {
                DataWorker w = (DataWorker)row.Tag;
                if (worker == w)
                {
                    return false;
                }
            }
            return true;
        }

        private void AddToGridManager(DataWorker worker)
        {
            if (!CheckData(worker))
                return;

            int nID = gridManager.Rows.Count + 1;

            DataGridViewRow row2 = new DataGridViewRow();
            row2.Tag = worker;

            DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
            cell1.Value = nID;
            row2.Cells.Add(cell1);

            DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
            cell2.Value = worker.MemberID;
            row2.Cells.Add(cell2);
          
            DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
            cell3.Value = worker.Name;
            row2.Cells.Add(cell3);

            DataGridViewTextBoxCell cell4 = new DataGridViewTextBoxCell();
            cell4.Value = worker.Company.CompanyName;
            row2.Cells.Add(cell4);

            DataGridViewTextBoxCell cell5 = new DataGridViewTextBoxCell();
            if (worker.Team == null)
                cell5.Value = "";
            else
                cell5.Value = worker.Team.Name;
            row2.Cells.Add(cell5);

            string szPositionName = "";
            if (worker.JobPosition != null)
                szPositionName = worker.JobPosition.Name;

            DataGridViewTextBoxCell cell6 = new DataGridViewTextBoxCell();
            cell6.Value = szPositionName;
            row2.Cells.Add(cell6);

            DataGridViewTextBoxCell cell7 = new DataGridViewTextBoxCell();
            cell7.Value = worker.MobilePhoneNumber;
            row2.Cells.Add(cell7);

            gridManager.Rows.Add(row2);
        }

        private void SetGridManager()
        {
            for (int i = 0; i < gridMember.Columns.Count; i++)
            {
                gridMember.Columns[i].SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            }

            for (int i = 0; i < gridManager.Columns.Count; i++)
            {
                gridManager.Columns[i].SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            }

            gridMember.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gridMember.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gridManager.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gridManager.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            foreach (DataWorker worker in m_arManagers)
            {
                AddToGridManager(worker);
            }
        }

        private void SetWorkerTreeNode()
        {
            treeViewTeam.Nodes.Clear();
            TreeNode companyNode = null;

            Dictionary<string, DataCompany> companies = ERPManager.Instance.DicComapny;
            foreach (KeyValuePair<string, DataCompany> pair in companies)
            {
                DataCompany company = pair.Value;
                companyNode = new TreeNode(company.CompanyName);
                companyNode.Tag = company;
                treeViewTeam.Nodes.Add(companyNode);

                foreach (DataDepartment team in company.Departments)
                {
                    if (team.Workers.Count == 0)
                        continue;
                    TreeNode teamNode = new TreeNode(team.Name);
                    teamNode.Tag = team;
                    companyNode.Nodes.Add(teamNode);
                }
            }

            Dictionary<string, DataWorker> workers = ERPManager.Instance.DicCompanyWorkers;
            bool m_bFirst = true;
            TreeNode UnteamNode = null;
            foreach (KeyValuePair<string, DataWorker> pair in workers)
            {
                DataWorker worker = pair.Value;
                if (worker.Team == null)
                {
                    // 팀이 없는 작업자가 존재하면 팀없음 노드를 추가한다.
                    if (m_bFirst == true)
                    {
                        m_bFirst = false;
                        UnteamNode = new TreeNode("Unknown Team");
                        companyNode.Nodes.Add(UnteamNode);
                        break;
                    }
                }
            }

            treeViewTeam.ExpandAll();
        }

        private void treeViewTeam_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Action == TreeViewAction.Unknown)
                return;
        }

        private void treeViewTeam_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Action == TreeViewAction.Unknown)
                return;

            TreeNode node = e.Node;

            ArrayList arWorker = null;

            if (node.Text == "Unknown Team")
            {
                arWorker = new ArrayList();
                Dictionary<string, DataWorker> workers = ERPManager.Instance.DicCompanyWorkers;
                foreach (KeyValuePair<string, DataWorker> pair in workers)
                {
                    DataWorker worker = pair.Value;
                    if (worker.Team == null)
                    {
                        arWorker.Add(worker);
                    }
                }
            }
            else
            {
                object obj = node.Tag;
                if (obj.GetType() == typeof(DataCompany))
                    return;
                DataDepartment team = (DataDepartment)obj;
                arWorker = team.Workers;
            }


            gridMember.Rows.Clear();
            foreach (DataWorker w in arWorker)
            {
                DataGridViewRow row = new DataGridViewRow();

                DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
                cell1.Value = w.MemberID;
                row.Cells.Add(cell1);

                DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
                cell2.Value = w.Name;
                row.Cells.Add(cell2);

                string szPositionName = "";
                if (w.JobPosition != null)
                    szPositionName = w.JobPosition.Name;

                DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
                cell3.Value = szPositionName;
                row.Cells.Add(cell3);

                DataGridViewTextBoxCell cell4 = new DataGridViewTextBoxCell();
                cell4.Value = w.MobilePhoneNumber;
                row.Cells.Add(cell4);
                row.Tag = w;

                gridMember.Rows.Add(row);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection arRows = gridManager.SelectedRows;
            if (arRows != null && arRows.Count > 0)
            {
                for (int i = 0; i < arRows.Count; i++)
                {
                    DataGridViewRow row = arRows[i];
                    gridManager.Rows.Remove(row);
                    //DataWorker worker = (DataWorker)row.Tag;
                    
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection arRows = gridMember.SelectedRows;
            if (arRows != null && arRows.Count > 0)
            {
                for (int i = 0; i < arRows.Count; i++)
                {
                    DataGridViewRow row = arRows[i];
                    DataWorker worker = (DataWorker)row.Tag;
                    AddToGridManager(worker);
                }
            }
        }

        private void radioNoLimit_CheckedChanged(object sender, EventArgs e)
        {
            if (radioNoLimit.Checked == true)
            {
                gridMember.SelectAll();
            }
            
        }

        private void gridMember_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            radioNoLimit.Checked = false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            m_arManagers.Clear();
            foreach (DataGridViewRow row in gridManager.Rows)
            {                
                DataWorker worker = (DataWorker)row.Tag;
                m_arManagers.Add(worker);
            }
            
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        

    }
}
