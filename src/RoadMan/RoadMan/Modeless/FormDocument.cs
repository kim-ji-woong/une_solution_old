using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace RoadMan
{
    public partial class FormDocument : Form
    {
        private class ColumnHeader
        {
            private DataGridView m_grid = null;
            private int m_nHeaderRowIndex = 0;
            private int m_nHeaderRowCount = 1;
            private int m_nBeginColumnIndex = 0;
            private int m_nEndColumnIndex = 0;

            public DataGridView Grid
            {
                get { return m_grid; }
                set { m_grid = value; }
            }

            public int HeaderRowIndex
            {
                get { return m_nHeaderRowIndex; }
                set { m_nHeaderRowIndex = value; }
            }

            public int HeaderRowCount
            {
                get { return m_nHeaderRowCount; }
                set { m_nHeaderRowCount = value; }
            }

            public int BeginColumnIndex
            {
                get { return m_nBeginColumnIndex; }
                set { m_nBeginColumnIndex = value; }
            }

            public int EndColumnIndex
            {
                get { return m_nEndColumnIndex; }
                set { m_nEndColumnIndex = value; }
            }
        }

        private ArrayList m_arrColumnHeaders = new ArrayList();
        
        public FormDocument()
        {
            InitializeComponent();

            InitGrid();
        }

        private void InitGrid()
        {
            SetHeaderCell(dataGridViewDate, "시설결정일", 0, 2, 6, 7);
            SetHeaderCell(dataGridViewDecision, "시설의 결정", 0, 2, 8, 9);
            SetHeaderCell(dataGridViewResult, "집행여부", 0, 1, 10, 12);
            SetHeaderCell(dataGridViewArea, "면적(㎡)", 1, 1, 10, 12);
            SetHeaderCell(dataGridViewIncomplete, "미개설 시설현황", 0, 1, 13, 21);
            SetHeaderCell(dataGridViewType, "지목현황(㎡)", 1, 1, 13, 15);
            SetHeaderCell(dataGridViewOwner, "소유구분(㎡)", 1, 1, 16, 19);

            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridView1.ColumnHeadersHeight *= 3;

            SetAlign(colNo, DataGridViewContentAlignment.MiddleCenter, DataGridViewContentAlignment.MiddleCenter);
            SetAlign(colCity, DataGridViewContentAlignment.MiddleCenter, DataGridViewContentAlignment.MiddleCenter);
            SetAlign(colType, DataGridViewContentAlignment.MiddleCenter, DataGridViewContentAlignment.MiddleCenter);
            SetAlign(colSubType, DataGridViewContentAlignment.MiddleCenter, DataGridViewContentAlignment.MiddleCenter);
            SetAlign(colTypeName, DataGridViewContentAlignment.MiddleCenter, DataGridViewContentAlignment.MiddleCenter);
            SetAlign(colStatus, DataGridViewContentAlignment.MiddleCenter, DataGridViewContentAlignment.MiddleCenter);
            SetAlign(colLastDate, DataGridViewContentAlignment.BottomCenter, DataGridViewContentAlignment.MiddleCenter);
            SetAlign(colFirstDate, DataGridViewContentAlignment.BottomCenter, DataGridViewContentAlignment.MiddleCenter);
            SetAlign(colArea, DataGridViewContentAlignment.BottomCenter, DataGridViewContentAlignment.MiddleRight);
            SetAlign(colInsertArea, DataGridViewContentAlignment.BottomCenter, DataGridViewContentAlignment.MiddleRight);
            SetAlign(colComplete, DataGridViewContentAlignment.BottomCenter, DataGridViewContentAlignment.MiddleRight);
            SetAlign(colIncomplete, DataGridViewContentAlignment.BottomCenter, DataGridViewContentAlignment.MiddleRight);
            SetAlign(colPartialComplete, DataGridViewContentAlignment.BottomCenter, DataGridViewContentAlignment.MiddleRight);
            SetAlign(colRiceField, DataGridViewContentAlignment.BottomCenter, DataGridViewContentAlignment.MiddleRight);
            SetAlign(colField, DataGridViewContentAlignment.BottomCenter, DataGridViewContentAlignment.MiddleRight);
            SetAlign(colLand, DataGridViewContentAlignment.BottomCenter, DataGridViewContentAlignment.MiddleRight);
            SetAlign(colETC, DataGridViewContentAlignment.BottomCenter, DataGridViewContentAlignment.MiddleRight);
            SetAlign(colNational, DataGridViewContentAlignment.BottomCenter, DataGridViewContentAlignment.MiddleRight);
            SetAlign(colPublic, DataGridViewContentAlignment.BottomCenter, DataGridViewContentAlignment.MiddleRight);
            SetAlign(colPrivate, DataGridViewContentAlignment.BottomCenter, DataGridViewContentAlignment.MiddleRight);
            SetAlign(colAvgCost, DataGridViewContentAlignment.MiddleCenter, DataGridViewContentAlignment.MiddleRight);
            SetAlign(colConstCost, DataGridViewContentAlignment.MiddleCenter, DataGridViewContentAlignment.MiddleRight);
            SetAlign(colOutlineCost, DataGridViewContentAlignment.MiddleCenter, DataGridViewContentAlignment.MiddleRight);

            colAvgCost.HeaderText = "\r\n" + colAvgCost.HeaderText;
            colConstCost.HeaderText = "\r\n" + colConstCost.HeaderText;

            //dataGridView1.Paint += new PaintEventHandler(dataGridView1_Paint);
        }

        /*void dataGridView1_Paint(object sender, PaintEventArgs e)
        {
            //get the column header cell
            Rectangle rect = this.dataGridView1.GetCellDisplayRectangle(0, -1, true);

            rect.X += 1;
            rect.Y += 1;
            rect.Width = rect.Width * 2 - 2;
            rect.Height = rect.Height / 2 - 2;
            e.Graphics.FillRectangle(new
                SolidBrush(this.dataGridView1.ColumnHeadersDefaultCellStyle.BackColor), rect);
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            e.Graphics.DrawString("Header Text",
                this.dataGridView1.ColumnHeadersDefaultCellStyle.Font,
                new SolidBrush(this.dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor),
                rect,
                format);
        }*/

        private void SetAlign(DataGridViewColumn column, DataGridViewContentAlignment alignHeader, DataGridViewContentAlignment alignOthers)
        {
            column.HeaderCell.Style.Alignment = alignHeader;
            column.DefaultCellStyle.Alignment = alignOthers;
        }

        private int GetHeaderCellHeight(DataGridView grid)
        {
            return grid.ColumnHeadersHeight - 2;
        }

        private void SetHeaderCell(DataGridView grid, string strHeaderText, int nHeaderRowIndex, int nHeaderRowCount, int nBeginColumnIndex, int nEndColumnIndex)
        {
            grid.AllowUserToAddRows = false;
            grid.RowHeadersVisible = false;
            grid.Enabled = false;
            grid.BorderStyle = BorderStyle.None;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;

            grid.Size = new Size(GetMergedCellWidth(nBeginColumnIndex, nEndColumnIndex), GetHeaderCellHeight(grid) * nHeaderRowCount);
            grid.ColumnHeadersHeight = grid.Size.Height + 2;
            grid.Location = new Point(GetMergedCellWidth(0, nBeginColumnIndex - 1) + 1, GetHeaderCellHeight(grid) * nHeaderRowIndex + 1 + dataGridView1.Location.Y);

            grid.Columns[0].Width = grid.Size.Width;
            grid.Columns[0].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            if (nHeaderRowCount == 1)
                grid.Columns[0].HeaderText = strHeaderText;
            else
                grid.Columns[0].HeaderText = "\r\n" + strHeaderText + "\r\n";

            ColumnHeader header = new ColumnHeader();

            header.Grid = grid;
            header.HeaderRowIndex = nHeaderRowIndex;
            header.HeaderRowCount = nHeaderRowCount;
            header.BeginColumnIndex = nBeginColumnIndex;
            header.EndColumnIndex = nEndColumnIndex;

            m_arrColumnHeaders.Add(header);
        }

        private int GetMergedCellWidth(int nBeginColumnIndex, int nEndColumnIndex)
        {
            int nWidth = 0;

            for (int i=nBeginColumnIndex;i<=nEndColumnIndex;i++)
            {
                nWidth += dataGridView1.Columns[i].Width;
            }

            return nWidth;
        }

        private void ResizeHeaders()
        {
            foreach (ColumnHeader header in m_arrColumnHeaders)
            {
                header.Grid.Size = new Size(GetMergedCellWidth(header.BeginColumnIndex, header.EndColumnIndex),
                    header.Grid.Size.Height);
                header.Grid.Location = new Point(GetMergedCellWidth(0, header.BeginColumnIndex - 1) + 1 - dataGridView1.HorizontalScrollingOffset,
                    GetHeaderCellHeight(header.Grid) * header.HeaderRowIndex + 1 + dataGridView1.Location.Y);
            }
        }

        private void dataGridView1_Resize(object sender, EventArgs e)
        {
            ResizeHeaders();
        }

        private void dataGridView1_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            ResizeHeaders();
        }

        private void dataGridView1_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                ResizeHeaders();
            }
        }
    }
}
