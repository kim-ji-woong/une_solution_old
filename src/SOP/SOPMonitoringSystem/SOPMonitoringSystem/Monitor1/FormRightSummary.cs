using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SOPMonitoringSystem
{
    public partial class FormRighSummary : Form
    {
        private string m_strVersionName = "";
        private string m_strOwner = "";
        private DateTime m_dtLastAccess;
        private string m_strDesc = "";

        public FormRighSummary(string strVersionName, string strOwner, DateTime dtLastAccess, string strDesc)
        {
            InitializeComponent();

            m_strVersionName = strVersionName;
            m_strOwner = strOwner;
            m_dtLastAccess = dtLastAccess;
            m_strDesc = strDesc;

            AddGrid("버전명", " " + m_strVersionName);
            AddGrid("작성자", " " + m_strOwner);
            AddGrid("작성일", " " + m_dtLastAccess.ToLongDateString());
            AddGrid("설명", " " + m_strDesc);
        }

        private void AddGrid(string strItemName, string strItemData)
        {
            DataGridViewRow row = new DataGridViewRow();
            DataGridViewCell cell1 = new DataGridViewTextBoxCell();
            DataGridViewCell cell2 = new DataGridViewTextBoxCell();

            //cell1.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            cell1.Value = strItemName;
            cell2.Value = strItemData;

            row.Cells.Add(cell1);
            row.Cells.Add(cell2);

            dataGridSummary.Rows.Add(row);
        }
    }
}
