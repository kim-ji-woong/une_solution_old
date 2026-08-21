using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Data.SqlClient;

namespace HSMS
{
    public partial class FormManager : Form
    {
        public FormManager()
        {
            InitializeComponent();
        }

        private void LoadManager()
        {
            DataManager mgr = FormMain.Instance.DataMgr;
            foreach (Manager manager in mgr.Managers)
            {
                if (manager.Worker != null)
                {
                    m_arManagers.Add(manager.Worker);
                    m_arEditedManagers.Add(manager.Worker);
                }
            }
        }

        private ArrayList m_arManagers = new ArrayList();
        private ArrayList m_arEditedManagers = new ArrayList();

        private void SetGridManager()
        {
            gridManager.ClearSelection();
            gridManager.Rows.Clear();

            foreach (DataWorker worker in m_arEditedManagers)
            {
                AddToGridManager(worker);
            }
        }

        private void AddToGridManager(DataWorker worker)
        {
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


        private void btnEdit_Click(object sender, EventArgs e)
        {
            FormEditManager frm = new FormEditManager();
            foreach (DataWorker mgr in m_arEditedManagers)
            {
                if (mgr != null)
                    frm.AddManager(mgr);
            }

            frm.ShowInTaskbar = false;
            if (PageBackstageHome.ShowTranslucentSubForm(frm) == System.Windows.Forms.DialogResult.OK)
            {
                ArrayList arManagers = frm.Managers;
                m_arEditedManagers = arManagers;
                SetGridManager();
            }
        }

        private void FormManager_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < gridManager.Columns.Count; i++)
            {
                gridManager.Columns[i].SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            }

            gridManager.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gridManager.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            LoadManager();
            SetGridManager();
        }

        private void FormManager_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            ArrayList arDelete = new ArrayList();
            ArrayList arAdd = new ArrayList();
            
            foreach(DataWorker worker in m_arManagers)
            {
                if (!m_arEditedManagers.Contains(worker))
                {
                    arDelete.Add(worker);

                    EditManager edit = new EditManager();
                    edit.SQLType = ChangedData.DELETE;
                    edit.Manager = worker;
                    AddManager(edit);                    
                }              
            }
            
            foreach (DataWorker worker in m_arEditedManagers)
            {
                if (!m_arManagers.Contains(worker))
                {
                    arAdd.Add(worker);

                    EditManager edit = new EditManager();
                    edit.SQLType = ChangedData.INSERT;
                    edit.Manager = worker;
                    AddManager(edit);
                }
            }

            SaveToDB();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private ArrayList m_arEditData = new ArrayList();
        public void AddManager(EditManager mgr)
        {
            m_arEditData.Add(mgr);
        }    

        public void SaveToDB()
        {
            if (m_arEditData.Count == 0)
                return;

            DBConn con = new DBConn("HSMS");
            foreach (EditManager mgr in m_arEditData)
            {
                mgr.Update(con);
            }
        }
    }
}
