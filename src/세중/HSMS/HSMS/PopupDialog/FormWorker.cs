using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Data.SqlClient;

namespace HSMS
{
    public partial class FormWorker : Form
    {
        private DBConn m_ConnectionHSMS = null;
        private DataManager m_DataMgr = null;
        private ArrayList m_arrTempDataGrid = null;
        private ArrayList m_arrDBData = null;

        private ArrayList m_arrWorkersSave = new ArrayList();

        private Dictionary<string, DataWorker> m_dicGridData = new Dictionary<string, DataWorker>();


        public FormWorker()
        {
            InitializeComponent();

            m_ConnectionHSMS = new DBConn("HSMS");
            m_DataMgr = FormMain.Instance.DataMgr;

            //DB에 저장되어있는 데이터
            m_arrDBData = FormMain.Instance.DataMgr.GetWorkers();
        }

        private void FormWorker_Load(object sender, EventArgs e)        
        {
            SetGridView();
            LoadDataGridView();
            SetWorkerTreeNode();
            cboWorkerLevel.SelectedIndex = 0;
        }

        private void SetGridView()
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
        }

        private void LoadDataGridView()
        {
            gridManager.Rows.Clear();

            int nCount = 0;
            m_arrTempDataGrid = FormMain.Instance.DataMgr.GetWorkers();
            foreach (DataWorker worker in m_arrTempDataGrid)
            {
                nCount++;
                DataGridViewRow row = new DataGridViewRow();

                string strCompanyName = "";
                if (worker.Company != null)
                    strCompanyName = worker.Company.CompanyName;

                string strTeam = "";
                if (worker.Team != null)
                    strTeam = worker.Team.Name;

                DataGridViewTextBoxCell cell0 = new DataGridViewTextBoxCell();
                cell0.Value = nCount;
                row.Cells.Add(cell0);

                DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
                cell1.Value = worker.Name;
                row.Cells.Add(cell1);

                DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
                cell2.Value = strTeam;
                row.Cells.Add(cell2);
                
                DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
                cell3.Value = strCompanyName;
                row.Cells.Add(cell3);

                //string strJobPosition = "";
                //if (worker.JobPosition != null)
                //    strJobPosition = worker.JobPosition.Name;

                //DataGridViewTextBoxCell cell4 = new DataGridViewTextBoxCell();
                //cell4.Value = strJobPosition;
                //row.Cells.Add(cell4);

                DataGridViewTextBoxCell cell4 = new DataGridViewTextBoxCell();
                cell4.Value = worker.EnterLevel;
                row.Cells.Add(cell4);

                DataGridViewTextBoxCell cell5 = new DataGridViewTextBoxCell();
                cell5.Value = worker.Sensor;
                row.Cells.Add(cell5);

                DataGridViewTextBoxCell cell6 = new DataGridViewTextBoxCell();
                cell6.Value = worker.MemberID;
                row.Cells.Add(cell6);

                row.Tag = worker;

                gridManager.Rows.Add(row);
                m_dicGridData[worker.MemberID] = worker;
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

                string strCompanyName = "";
                if (w.Company != null)
                    strCompanyName = w.Company.CompanyName;

                string strTeam = "";
                if (w.Team != null)
                    strTeam = w.Team.Name;

                DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
                cell2.Value = w.Name;
                row.Cells.Add(cell2);


                DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
                cell3.Value = strTeam;
                row.Cells.Add(cell3);

                
                DataGridViewTextBoxCell cell4 = new DataGridViewTextBoxCell();
                cell4.Value = strCompanyName;
                row.Cells.Add(cell4);
                row.Tag = w;

                DataGridViewTextBoxCell cell5 = new DataGridViewTextBoxCell();
                cell5.Value = w.Sensor;
                row.Cells.Add(cell5);
                row.Tag = w;

                gridMember.Rows.Add(row);
            } 
        }
        
        

        private void btnAdd_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection arRows = gridMember.SelectedRows;

            int nRowCount = gridManager.Rows.Count;

            int nCount = 0;
            if (gridManager.Rows.Count == 0)
                nCount = 0;
            else
                nCount = (int)gridManager.Rows[nRowCount - 1].Cells[0].Value;

            nCount++;

            if (arRows != null && arRows.Count > 0)
            {
                for (int i = 0; i < arRows.Count; i++)
                {
                    bool isChecked = true;
                    DataGridViewRow row = arRows[i];
                    DataWorker worker = (DataWorker)row.Tag;
                    //DataWorker testworker = worker.
                    DataGridViewRow row2 = new DataGridViewRow();
                    row2.Tag = worker;


                    //중복검사(똑같은 데이터가 있는지)
                    if (m_dicGridData.ContainsKey(worker.MemberID))
                    {

                        DataWorker data = m_dicGridData[worker.MemberID];
                    
                        if (data == worker)
                            isChecked = false;
       
                    }

                    //센서아이디가 없는 데이터는 추가X
                    if (worker.Sensor.Trim() == "")
                    {
                        isChecked = false;
                        MessageBox.Show("센서아이디가 없는 데이터는 추가할 수 없습니다.");
                        continue;
                    }

                    //중복되면 추가안함
                    if (isChecked == false)
                    {
                        MessageBox.Show("중복된 데이터입니다..");
                        continue;
                    }

                    int nEnterLevel = 0;
                    switch(cboWorkerLevel.SelectedIndex)
                    {
                        case 0: nEnterLevel = 1;
                            break;
                        case 1: nEnterLevel = 2;
                            break;
                        case 2: nEnterLevel = 3;
                            break;
                        case 3: nEnterLevel = 4;
                            break;
                        case 4: nEnterLevel = 5;
                            break;
                    }

                    worker.EnterLevel = nEnterLevel;
                    m_dicGridData[worker.MemberID] = worker;

      
                    DataGridViewTextBoxCell cell0 = new DataGridViewTextBoxCell();
                    cell0.Value = nCount;
                    row2.Cells.Add(cell0);

                    DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
                    cell1.Value = worker.Name;
                    row2.Cells.Add(cell1);

                    string strCompanyName = "";
                    if (worker.Company != null)
                        strCompanyName = worker.Company.CompanyName;

                    string strTeam = "";
                    if (worker.Team != null)
                        strTeam = worker.Team.Name;

                    DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
                    cell2.Value = strTeam;
                    row2.Cells.Add(cell2);

                    DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
                    cell3.Value = strCompanyName;
                    row2.Cells.Add(cell3);

                    DataGridViewTextBoxCell cell4 = new DataGridViewTextBoxCell();
                    cell4.Value = nEnterLevel;
                    row2.Cells.Add(cell4);

                    DataGridViewTextBoxCell cell5 = new DataGridViewTextBoxCell();
                    cell5.Value = worker.Sensor;
                    row2.Cells.Add(cell5);

                    DataGridViewTextBoxCell cell6 = new DataGridViewTextBoxCell();
                    cell6.Value = worker.MemberID;
                    row2.Cells.Add(cell6);


                    gridManager.Rows.Add(row2);
                }
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection arRows = gridManager.SelectedRows;
    
            if (arRows != null && arRows.Count > 0)
            {
                for (int i = 0; i < arRows.Count; i++)
                {
                    DataGridViewRow row = arRows[i];
                    gridManager.Rows.Remove(row);

                    DataWorker worker = (DataWorker)row.Tag;

                    m_dicGridData.Remove(worker.MemberID);
                }
            }

            int nCount = 0;
            for (int i = 0; i < gridManager.Rows.Count; i++)
            {
                nCount++;
                gridManager.Rows[i].Cells[0].Value = nCount;
            }
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < gridManager.Rows.Count; i++)
            {
                DataGridViewRow row = gridManager.Rows[i];
                DataWorker worker = (DataWorker)row.Tag;

                //바꿨던 출입등급 원래대로
                worker.EnterLevel = worker.DBEnterLevel;
            }

                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }




        //적용만. 저장버튼을 누르기 전까진 DB에 저장 안됨
        private void btnOK_Click(object sender, EventArgs e)
        {
            m_arrTempDataGrid.Clear();
            DataGridViewRowCollection arRows = gridManager.Rows;
            if (arRows != null && arRows.Count > 0)
            {
                for (int i = 0; i < arRows.Count; i++)
                {
                    DataGridViewRow row = arRows[i];

                    DataWorker worker = (DataWorker)row.Tag;

                    
                    m_arrTempDataGrid.Add(worker);
                }
            }

            UpdateChangeData(m_arrDBData, m_arrTempDataGrid);

            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();            
        }

        private DataWorker FindDataWorker(int nID, ArrayList arrWorkers)
        {
            foreach (DataWorker worker in arrWorkers)
            {
                if (worker.ID == nID)
                    return worker;
            }
            return null;
        }

        private ArrayList m_arrEditWorkers = new ArrayList();

        private void UpdateChangeData(ArrayList arrOrigin, ArrayList arrCurrent)
        {
            //SQLType이 1=수정,2=삭제,3=삽입
            foreach (DataWorker worker in arrCurrent)
            {
                if (worker.ID >= 0)
                {
                    //변경 된 데이터
                    DataWorker worker2 = FindDataWorker(worker.ID, arrOrigin);
                    if (worker2 == null)
                        continue;

                    EditWorker editWorker = null;

                    if (worker.EnterLevel != worker2.DBEnterLevel)
                    {
                        if (editWorker == null)
                            editWorker = new EditWorker();
                        editWorker.EnterLevel = worker.EnterLevel;
                    }

                    if (editWorker != null)
                    {
                        editWorker.SQLType = ChangedData.UPDATE;
                        editWorker.Worker = worker;

                        m_arrEditWorkers.Add(editWorker);
                    }
                }
                else
                {
                    EditWorker editWorker = new EditWorker();
                    editWorker.Worker = worker;
                    editWorker.MemberID = worker.MemberID;
                    editWorker.EnterLevel = worker.EnterLevel;
                    editWorker.SQLType = ChangedData.INSERT;

                    m_arrEditWorkers.Add(editWorker);
                }
            }

            foreach (DataWorker worker in arrOrigin)
            {
                //삭제 된 데이터
                if(FindDataWorker(worker.ID,arrCurrent) == null)
                {
                    EditWorker editWorker = new EditWorker();
                    editWorker.Worker = worker;
                    editWorker.SQLType = ChangedData.DELETE;

                    m_arrEditWorkers.Add(editWorker);
                }
            }

            UpdateDB(m_arrEditWorkers);
            //UpdateDB(m_arrDBData);
        }

        private void UpdateDB(ArrayList arrEditWorkers)
        {
            ArrayList arrDeletes = new ArrayList();
            foreach (EditWorker editWorker in arrEditWorkers)
            {
                //데이터 넣었다가 뺀거면 값을 바꿀 필요가 없음
                if (editWorker.ID < 0)
                {
                    if (editWorker.SQLType == ChangedData.DELETE)
                    {
                        arrDeletes.Add(editWorker);
                    }
                }
            }

            foreach (EditWorker editWorker in arrDeletes)
                arrEditWorkers.Remove(editWorker);

            foreach (EditWorker editWorker in arrEditWorkers)
            {
                editWorker.Update(m_ConnectionHSMS);
            }
        }

        //작업자 추가
        public void WorkerSave(ArrayList arrWorkers)
        {
            
        }


        private void gridManager_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                int nRowIndex = e.RowIndex;

                if(nRowIndex == -1)
                {
                    return;
                }

                int nEnterLevel = 0;

                nEnterLevel = (int)gridManager.Rows[nRowIndex].Cells[4].Value;

                FormEditEnterLevel editEnterLevel = new FormEditEnterLevel(nEnterLevel, nRowIndex,this);
                editEnterLevel.StartPosition = FormStartPosition.Manual;
                editEnterLevel.Location = new Point(this.Location.X + this.Size.Width/2 - 200 , this.Location.Y + this.Size.Height/2 - 200);
                editEnterLevel.ShowDialog();
            }
        }

        public void EditEnterLevel(int nRowIndex, int nEditEnterLevel)
        {
            gridManager.Rows[nRowIndex].Cells[4].Value = nEditEnterLevel;

            DataGridViewRow row = gridManager.Rows[nRowIndex];
            DataWorker worker = (DataWorker)row.Tag;

            worker.EnterLevel = nEditEnterLevel;
        }
    }
}
