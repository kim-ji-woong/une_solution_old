using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOPBulletin
{
    public partial class FormDataList : Form
    {
        private class VisibleData : IComparable
        {
            private int m_nRowIndex = -1;
            private bool m_visible = true;

            public int RowIndex
            {
                get { return m_nRowIndex; }
                set { m_nRowIndex = value; }
            }

            public bool Visible
            {
                get { return m_visible; }
                set { m_visible = value; }
            }

            public VisibleData()
            {
            }

            public VisibleData(int nRowIndex, bool visible)
            {
                m_nRowIndex = nRowIndex;
                m_visible = visible;
            }

            public int CompareTo(object obj)
            {
                VisibleData row1 = this;
                VisibleData row2 = (VisibleData)obj;

                if (row1.RowIndex < row2.RowIndex)
                    return -1;
                else if (row1.RowIndex > row2.RowIndex)
                    return 1;

                return 0;
            }
        }

        private List<VisibleData> m_datas = new List<VisibleData>();

        public FormDataList()
        {
            InitializeComponent();
        }

        private void FormDataList_Load(object sender, EventArgs e)
        {
            InitColumns();
        }
        
        private void InitColumns()
        {
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        public void ClearData()
        {
            m_datas.Clear();
        }

        public void UpdateData(int nRowIndex, bool visible)
        {
            foreach (VisibleData data in m_datas)
            {
                if (data.RowIndex == nRowIndex)
                {
                    data.Visible = visible;
                    return;
                }
            }

            m_datas.Add(new VisibleData(nRowIndex, visible));
        }

        public void DeleteData(int nRowIndex)
        {

        }

        public void ShowList()
        {
            dataGridView1.Rows.Clear();
            m_datas.Sort();

            int nDataCount = m_datas.Count;

            for (int i=0;i<nDataCount-2;i+=3)
            {
                VisibleData data1 = m_datas[i];
                VisibleData data2 = i < nDataCount - 1 ? m_datas[i + 1] : null;
                VisibleData data3 = i < nDataCount - 2 ? m_datas[i + 2] : null;

                DataGridViewRow row = new DataGridViewRow();

                DataGridViewTextBoxCell cellText = new DataGridViewTextBoxCell();
                cellText.Value = data1.RowIndex;
                row.Cells.Add(cellText);

                DataGridViewCheckBoxCell cellCheck = new DataGridViewCheckBoxCell();
                cellCheck.Value = data1.Visible;
                row.Cells.Add(cellCheck);

                cellText = new DataGridViewTextBoxCell();

                if (data2 != null)
                    cellText.Value = data2.RowIndex;

                row.Cells.Add(cellText);

                cellCheck = new DataGridViewCheckBoxCell();
                
                if (data2 != null)
                    cellCheck.Value = data2.Visible;

                row.Cells.Add(cellCheck);

                cellText = new DataGridViewTextBoxCell();

                if (data3 != null)
                    cellText.Value = data3.RowIndex;

                row.Cells.Add(cellText);

                cellCheck = new DataGridViewCheckBoxCell();

                if (data3 != null)
                    cellCheck.Value = data3.Visible;

                row.Cells.Add(cellCheck);
            }

            this.Show();
        }

        private void FormDataList_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!FormMain.Instance.CloseApplication)
            {
                e.Cancel = true;
                this.Hide();
            }
        }
    }
}
