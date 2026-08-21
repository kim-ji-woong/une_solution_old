using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using Sections;

using UnE.SOP;
using UnE.SOP.Sections;

namespace SOPMonitoringSystem
{
    public partial class BarPage : Form
    {
        private int m_nCheckCount = 0;

        public BarPage()
        {
            InitializeComponent();
            dataGridView.BackgroundColor = Color.White;
        }

        public void ClearGrid()
        {
            m_nCheckCount = 0;
            dataGridView.Rows.Clear();
        }

        public void VisibleChange(EventArgs e)
        {
            if (this.Visible)
            {
                //FormSOP.Instance.FrmMain2.ApplyWindow(this.Handle.ToInt32());
                dataGridView.BackgroundColor = Color.White;

            }
        }
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (this.Visible)
            {
                //FormSOP.Instance.FrmMain2.ApplyWindow(this.Handle.ToInt32());
                dataGridView.BackgroundColor = Color.White;
                
            }
        }
        private bool bFirst = true;
        public void SetDataGrid(SectionTabPage page)
        {
            ClearGrid();

            if (page == null)
                return;

            ArrayList arList = new ArrayList();
            foreach (PanelSectionEx pane in page.Controls)
            {
                DataGridViewRow gridRow = new DataGridViewRow();

                DataGridViewCell cell = new DataGridViewTextBoxCell();
                cell.Value = pane.GetTitle();
                gridRow.Cells.Add(cell);

                DataGridViewCheckBoxCell checkcell = new DataGridViewCheckBoxCell();
                checkcell.Tag = pane;
                checkcell.ValueType = typeof(bool);
                checkcell.Value = bFirst == true ? true : pane.Visible;
                checkcell.TrueValue = true;
                checkcell.FalseValue = false;

                gridRow.Cells.Add(checkcell);
                dataGridView.Rows.Add(gridRow);
                if ((bool)checkcell.Value == true)
                {
                    m_nCheckCount++;
                    arList.Add(checkcell);
                }
            }

            if (bFirst == true)
                bFirst = false;

            if(m_nCheckCount == 1)
            {
                DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)arList[0];
                checkCell.ReadOnly = true;
            }
        }

        private void CellReadOnly(DataGridViewCheckBoxCell exceptCell)
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)row.Cells[1];

                if ((bool)cell.Value == true && cell != exceptCell)
                {
                    cell.ReadOnly = true;
                    break;
                }
            }
        }

        private void CellFree()
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)row.Cells[1];
                cell.ReadOnly = false;
            }
        }

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewTextBoxCell cell = (DataGridViewTextBoxCell)dataGridView.Rows[e.RowIndex].Cells[0];
            DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)dataGridView.Rows[e.RowIndex].Cells[1];
            if (checkCell.ReadOnly)
                return;

            PanelSectionEx pane = (PanelSectionEx)checkCell.Tag;
            if (pane != null)
            {
                switch ((bool)checkCell.Value)
                {
                    case true:
                        if (m_nCheckCount == 1)
                            break;
                        if (m_nCheckCount == 2)
                        {
                            CellReadOnly(checkCell);
                        }
                        m_nCheckCount--;
                        checkCell.Value = false;
                        pane.Hide();
                        break;
                    case false:
                        if (m_nCheckCount == 1)
                        {
                            CellFree();
                        }
                        pane = (PanelSectionEx)checkCell.Tag;
                        pane.Show();
                        checkCell.Value = true;
                        m_nCheckCount++;
                        break;
                }
                FormSOP.Instance.GetPageHome().ShowPanel(pane);
            }            
        }
    }
}
