using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;

namespace SOPMonitoringSystem
{
    public partial class DockingReceiveMessage : Form
    {
        //private FormMain frm;
        //private ArrayList arrResult;
        //private ArrayList arrSOP;
       // private int DBNum;
        //private WebDBManager dbMgr;
        //private bool _DBSetting = false;

        public DockingReceiveMessage()
        {
            InitializeComponent();
        }
        
        public void AddGridData(string time, string disa, string act)
        {
            int nID = dataGridView.Rows.Count + 1;

            DataGridViewRow gridRow = new DataGridViewRow();
            DataGridViewCell cell = new DataGridViewTextBoxCell();

            cell.Value = nID.ToString();
            gridRow.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = time;
            gridRow.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = disa;
            gridRow.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = act;
            gridRow.Cells.Add(cell);

            dataGridView.Rows.Add(gridRow);

            dataGridView.FirstDisplayedScrollingRowIndex = dataGridView.RowCount - 1;
            dataGridView.Rows[dataGridView.RowCount - 1].Selected = true;
        }
    }
}
