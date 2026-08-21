using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace UnE
{
    namespace Controls
    {
        public class MergedDataGridView : DataGridView
        {
            public class MergedColumns
            {
                private int m_nBeginColumnIndex = -1;
                private int m_nEndColumnIndex = -1;

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

                public MergedColumns()
                {
                }

                public MergedColumns(int nBeginColumnIndex, int nEndColumnIndex)
                {
                    m_nBeginColumnIndex = nBeginColumnIndex;
                    m_nEndColumnIndex = nEndColumnIndex;
                }
            }

            public class MergedCells
            {
                public enum CellType { DrawingCell = 0, NoDrawingCell, NotInclude };

                private int m_nMinRowIndex = -1;
                private int m_nMinColumnIndex = -1;
                private int m_nMaxRowIndex = -1;
                private int m_nMaxColumnIndex = -1;

                private bool m_drawingFirst = true;
                private MergedDataGridView m_grid = null;

                public int MinRowIndex
                {
                    get { return m_nMinRowIndex; }
                    set { m_nMinRowIndex = value; }
                }

                public int MinColumnIndex
                {
                    get { return m_nMinColumnIndex; }
                    set { m_nMinColumnIndex = value; }
                }

                public int MaxRowIndex
                {
                    get { return m_nMaxRowIndex; }
                    set { m_nMaxRowIndex = value; }
                }

                public int MaxColumnIndex
                {
                    get { return m_nMaxColumnIndex; }
                    set { m_nMaxColumnIndex = value; }
                }

                public bool DrawingFirst
                {
                    get { return m_drawingFirst; }
                    set
                    {
                        m_drawingFirst = value;

                        if (m_grid != null)
                        {
                            for (int i = m_nMinRowIndex; i <= m_nMaxRowIndex; i++)
                            {
                                for (int j = m_nMinColumnIndex; j <= m_nMaxColumnIndex; j++)
                                {
                                    m_grid.Rows[i].Cells[j].ReadOnly = true;
                                }
                            }

                            if (m_drawingFirst)
                                m_grid.Rows[m_nMinRowIndex].Cells[m_nMinColumnIndex].ReadOnly = false;
                            else
                                m_grid.Rows[m_nMaxRowIndex].Cells[m_nMaxColumnIndex].ReadOnly = false;
                        }
                    }
                }

                public MergedCells(MergedDataGridView grid)
                {
                    m_grid = grid;
                }

                public MergedCells(MergedDataGridView grid, int nMinRowIndex, int nMinColumnIndex, int nMaxRowIndex, int nMaxColumnIndex)
                {
                    m_nMinRowIndex = nMinRowIndex;
                    m_nMinColumnIndex = nMinColumnIndex;
                    m_nMaxRowIndex = nMaxRowIndex;
                    m_nMaxColumnIndex = nMaxColumnIndex;

                    m_grid = grid;
                }

                public CellType GetCellType(int nRowIndex, int nColumnIndex)
                {
                    if (m_drawingFirst)
                    {
                        if (nRowIndex == m_nMinRowIndex && nColumnIndex == m_nMinColumnIndex)
                            return CellType.DrawingCell;
                    }
                    else
                    {
                        if (nRowIndex == m_nMaxRowIndex && nColumnIndex == m_nMaxColumnIndex)
                            return CellType.NoDrawingCell;
                    }

                    if (nRowIndex >= m_nMinRowIndex && nRowIndex <= m_nMaxRowIndex &&
                            nColumnIndex >= m_nMinColumnIndex && nColumnIndex <= m_nMaxColumnIndex)
                        return CellType.NoDrawingCell;

                    return CellType.NotInclude;
                }
            }

            private List<MergedColumns> m_mergedColumns = new List<MergedColumns>();
            private List<MergedCells> m_mergedCells = new List<MergedCells>();
            private System.Windows.Forms.VisualStyles.VisualStyleRenderer selectedRenderer = null;//new System.Windows.Forms.VisualStyles.VisualStyleRenderer(System.Windows.Forms.VisualStyles.VisualStyleElement.Header.Item.Normal);

            private IMergedDataGridViewOwner m_owner = null;

            public IMergedDataGridViewOwner Owner
            {
                get { return m_owner; }
                set { m_owner = value; }
            }

            public MergedDataGridView()
            {
                try
                {
                    selectedRenderer = new System.Windows.Forms.VisualStyles.VisualStyleRenderer(System.Windows.Forms.VisualStyles.VisualStyleElement.Header.Item.Normal);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine(e.Message);
                    selectedRenderer = null;
                }

                this.CellPainting += new DataGridViewCellPaintingEventHandler(this.OnCellPainting);
                this.Scroll += new System.Windows.Forms.ScrollEventHandler(this.OnScroll);
                this.DoubleBuffered = true;
            }

            public MergedColumns MergeColumns(int nBeginColumnIndex, int nEndColumnIndex)
            {
                if (nBeginColumnIndex < 0 || nEndColumnIndex >= this.Columns.Count || nBeginColumnIndex >= nEndColumnIndex)
                    return null;

                if (!CheckDuplicate(nBeginColumnIndex, nEndColumnIndex))
                    return null;

                MergedColumns merge = new MergedColumns(nBeginColumnIndex, nEndColumnIndex);
                m_mergedColumns.Add(merge);
                return merge;
            }

            public MergedCells MergeCells(int nMinRowIndex, int nMinColumnIndex, int nMaxRowIndex, int nMaxColumnIndex)
            {
                if (nMinRowIndex < 0 || nMinColumnIndex < 0 || nMaxRowIndex >= this.Rows.Count || nMaxColumnIndex >= this.Columns.Count)
                    return null;

                if (!CheckDuplicate(nMinRowIndex, nMinColumnIndex, nMaxRowIndex, nMaxColumnIndex))
                    return null;

                MergedCells merge = new MergedCells(this, nMinRowIndex, nMinColumnIndex, nMaxRowIndex, nMaxColumnIndex);
                m_mergedCells.Add(merge);
                return merge;
            }

            public void RemoveMergedColumns(MergedColumns merge)
            {
                m_mergedColumns.Remove(merge);
            }

            public void RemoveMergedCells(MergedCells merge)
            {
                m_mergedCells.Remove(merge);
            }

            public int GetMergedColumnsCount()
            {
                return m_mergedColumns.Count;
            }

            public MergedColumns GetMergedColumns(int nIndex)
            {
                if (nIndex < 0 || nIndex >= GetMergedColumnsCount())
                    return null;

                return m_mergedColumns[nIndex];
            }

            public void RemoveMergedColumns(int nIndex)
            {
                if (nIndex < 0 || nIndex >= GetMergedColumnsCount())
                    return;

                m_mergedColumns.RemoveAt(nIndex);
            }

            public void ClearMergedColumns()
            {
                m_mergedColumns.Clear();
            }

            public int GetMergedCellsCount()
            {
                return m_mergedCells.Count;
            }

            public MergedCells GetMergedCells(int nIndex)
            {
                if (nIndex < 0 || nIndex >= GetMergedCellsCount())
                    return null;

                return m_mergedCells[nIndex];
            }

            public void RemoveMergedCells(int nIndex)
            {
                if (nIndex < 0 || nIndex >= GetMergedCellsCount())
                    return;

                m_mergedCells.RemoveAt(nIndex);
            }

            public void RemoveMergedCells()
            {
                m_mergedCells.Clear();
            }

            // Return 값 : 중복된 값이 있으면 false, 없으면 true
            private bool CheckDuplicate(int nBeginColumnIndex, int nEndColumnIndex)
            {
                foreach (MergedColumns merge in m_mergedColumns)
                {
                    if (nBeginColumnIndex >= merge.BeginColumnIndex && nBeginColumnIndex <= merge.EndColumnIndex)
                        return false;
                    if (nEndColumnIndex >= merge.BeginColumnIndex && nEndColumnIndex <= merge.EndColumnIndex)
                        return false;

                    if (merge.BeginColumnIndex >= nBeginColumnIndex && merge.BeginColumnIndex <= nEndColumnIndex)
                        return false;
                    if (merge.EndColumnIndex >= nBeginColumnIndex && merge.EndColumnIndex <= nEndColumnIndex)
                        return false;
                }

                return true;
            }

            // Return 값 : 중복된 값이 있으면 false, 없으면 true
            private bool CheckDuplicate(int nMinRowIndex, int nMinColumnIndex, int nMaxRowIndex, int nMaxColumnIndex)
            {
                foreach (MergedCells merge in m_mergedCells)
                {
                    if (IsInclude(nMinRowIndex, nMinColumnIndex, merge))
                        return false;
                    if (IsInclude(nMinRowIndex, nMaxColumnIndex, merge))
                        return false;
                    if (IsInclude(nMaxRowIndex, nMinColumnIndex, merge))
                        return false;
                    if (IsInclude(nMaxRowIndex, nMaxColumnIndex, merge))
                        return false;

                    if (IsInclude(merge.MinRowIndex, merge.MinColumnIndex, nMinRowIndex, nMinColumnIndex, nMaxRowIndex, nMaxColumnIndex))
                        return false;
                    if (IsInclude(merge.MinRowIndex, merge.MaxColumnIndex, nMinRowIndex, nMinColumnIndex, nMaxRowIndex, nMaxColumnIndex))
                        return false;
                    if (IsInclude(merge.MaxRowIndex, merge.MinColumnIndex, nMinRowIndex, nMinColumnIndex, nMaxRowIndex, nMaxColumnIndex))
                        return false;
                    if (IsInclude(merge.MaxRowIndex, merge.MaxColumnIndex, nMinRowIndex, nMinColumnIndex, nMaxRowIndex, nMaxColumnIndex))
                        return false;
                }

                return true;
            }

            private bool IsInclude(int nRowIndex, int nColumnIndex, MergedCells merge)
            {
                if (nRowIndex >= merge.MinRowIndex && nRowIndex <= merge.MaxRowIndex &&
                    nColumnIndex >= merge.MinColumnIndex && nColumnIndex <= merge.MaxColumnIndex)
                    return true;

                return false;
            }

            private bool IsInclude(int nRowIndex, int nColumnIndex, int nMinRowIndex, int nMinColumnIndex, int nMaxRowIndex, int nMaxColumnIndex)
            {
                if (nRowIndex >= nMinRowIndex && nRowIndex <= nMaxRowIndex &&
                    nColumnIndex >= nMinColumnIndex && nColumnIndex <= nMaxColumnIndex)
                    return true;

                return false;
            }

            private void DrawColumn(DataGridViewCellPaintingEventArgs e)
            {
                MergedColumns firstMerge = null, notFirstMerge = null;

                foreach (MergedColumns merge in m_mergedColumns)
                {
                    if (e.ColumnIndex == merge.BeginColumnIndex)
                    {
                        firstMerge = merge;
                        break;
                    }
                    else if (e.ColumnIndex > merge.BeginColumnIndex && e.ColumnIndex <= merge.EndColumnIndex)
                    {
                        notFirstMerge = merge;
                        break;
                    }
                }

                if (firstMerge != null)
                {
                    e.PaintBackground(e.ClipBounds, true);

                    Rectangle r = e.CellBounds;

                    for (int i = firstMerge.BeginColumnIndex + 1; i <= firstMerge.EndColumnIndex; i++)
                    {
                        Rectangle r1 = this.GetCellDisplayRectangle(i, e.RowIndex, true);
                        r.Width += r1.Width;
                    }

                    r.Width -= 1;
                    r.Height -= 1;

                    DataGridViewColumn column = this.Columns[e.ColumnIndex];

                    using (SolidBrush brBk = new SolidBrush(column.HeaderCell.InheritedStyle.BackColor))
                    using (SolidBrush brFr = new SolidBrush(column.HeaderCell.InheritedStyle.ForeColor))
                    {
                        if (this.EnableHeadersVisualStyles && selectedRenderer != null)
                            selectedRenderer.DrawBackground(e.Graphics, r);
                        else
                            e.Graphics.FillRectangle(brBk, r);

                        StringFormat sf = new StringFormat();
                        sf.Alignment = StringAlignment.Center;
                        sf.LineAlignment = StringAlignment.Center;

                        r.Y += 2;
                        e.Graphics.DrawString(column.HeaderText, e.CellStyle.Font, brFr, r, sf);
                        sf.Dispose();
                    }
                }
                else if (notFirstMerge != null)
                {
                    using (Pen p = new Pen(this.GridColor))
                    {
                        e.Graphics.DrawLine(p, e.CellBounds.Left, e.CellBounds.Bottom - 1,
                            e.CellBounds.Right, e.CellBounds.Bottom - 1);
                        e.Graphics.DrawLine(p, e.CellBounds.Right - 1, e.CellBounds.Top,
                            e.CellBounds.Right - 1, e.CellBounds.Bottom);
                    }
                }
                else
                {
                    e.PaintBackground(e.ClipBounds, true);

                    Rectangle r = e.CellBounds;

                    r.Width -= 1;
                    r.Height -= 1;

                    DataGridViewColumn column = this.Columns[e.ColumnIndex];

                    using (SolidBrush brBk = new SolidBrush(column.HeaderCell.InheritedStyle.BackColor))
                    using (SolidBrush brFr = new SolidBrush(column.HeaderCell.InheritedStyle.ForeColor))
                    {
                        if (this.EnableHeadersVisualStyles && selectedRenderer != null)
                            selectedRenderer.DrawBackground(e.Graphics, r);
                        else
                            e.Graphics.FillRectangle(brBk, r);

                        StringFormat sf = new StringFormat();
                        sf.Alignment = StringAlignment.Center;
                        sf.LineAlignment = StringAlignment.Center;

                        r.Y += 2;
                        e.Graphics.DrawString(column.HeaderText, e.CellStyle.Font, brFr, r, sf);
                        sf.Dispose();
                    }
                }

                if (m_owner != null)
                    m_owner.OnPostDrawColumn(this, e);

                e.Handled = true;
            }

            private StringFormat GetStringFormat(DataGridViewContentAlignment align)
            {
                StringFormat sf = new StringFormat();

                if (align == DataGridViewContentAlignment.BottomCenter)
                {
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Far;
                }
                else if (align == DataGridViewContentAlignment.BottomLeft)
                {
                    sf.Alignment = StringAlignment.Near;
                    sf.LineAlignment = StringAlignment.Far;
                }
                else if (align == DataGridViewContentAlignment.BottomRight)
                {
                    sf.Alignment = StringAlignment.Far;
                    sf.LineAlignment = StringAlignment.Far;
                }
                else if (align == DataGridViewContentAlignment.MiddleCenter)
                {
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                }
                else if (align == DataGridViewContentAlignment.MiddleLeft)
                {
                    sf.Alignment = StringAlignment.Near;
                    sf.LineAlignment = StringAlignment.Center;
                }
                else if (align == DataGridViewContentAlignment.MiddleRight)
                {
                    sf.Alignment = StringAlignment.Far;
                    sf.LineAlignment = StringAlignment.Center;
                }
                else if (align == DataGridViewContentAlignment.TopCenter)
                {
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Near;
                }
                else if (align == DataGridViewContentAlignment.TopLeft)
                {
                    sf.Alignment = StringAlignment.Near;
                    sf.LineAlignment = StringAlignment.Near;
                }
                else if (align == DataGridViewContentAlignment.TopRight)
                {
                    sf.Alignment = StringAlignment.Far;
                    sf.LineAlignment = StringAlignment.Near;
                }

                return sf;
            }

            private void DrawCell(DataGridViewCellPaintingEventArgs e)
            {
                MergedCells drawingCell = null, noDrawingCell = null;
                bool rightDrawing = false, bottomDrawing = false;

                foreach (MergedCells merge in m_mergedCells)
                {
                    MergedCells.CellType cellType = merge.GetCellType(e.RowIndex, e.ColumnIndex);

                    if (cellType == MergedCells.CellType.DrawingCell)
                    {
                        drawingCell = merge;
                        break;
                    }
                    else if (cellType == MergedCells.CellType.NoDrawingCell)
                    {
                        if (e.ColumnIndex < merge.MaxColumnIndex)
                        {
                            if (e.RowIndex == merge.MaxRowIndex)
                                bottomDrawing = true;
                        }
                        else if (e.ColumnIndex == merge.MaxColumnIndex)
                        {
                            rightDrawing = true;

                            if (e.RowIndex == merge.MaxRowIndex)
                                bottomDrawing = true;
                        }

                        noDrawingCell = merge;
                        break;
                    }
                }

                if (drawingCell != null)
                {
                    e.PaintBackground(e.ClipBounds, true);

                    Rectangle r = this.GetCellDisplayRectangle(drawingCell.MinColumnIndex, drawingCell.MinRowIndex, true);

                    for (int i = drawingCell.MinColumnIndex + 1; i <= drawingCell.MaxColumnIndex; i++)
                    {
                        Rectangle r1 = this.GetCellDisplayRectangle(i, e.RowIndex, true);
                        r.Width += r1.Width;
                    }

                    for (int i = drawingCell.MinRowIndex + 1; i <= drawingCell.MaxRowIndex; i++)
                    {
                        Rectangle r1 = this.GetCellDisplayRectangle(e.ColumnIndex, i, true);
                        r.Height += r1.Height;
                    }

                    r.Width -= 1;
                    r.Height -= 1;

                    DataGridViewCell cell = this.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    DataGridViewContentAlignment align = cell.InheritedStyle.Alignment;

                    using (SolidBrush brBk = new SolidBrush(GetDrawingCellColor(e, cell, true)))
                    using (SolidBrush brFr = new SolidBrush(GetDrawingCellColor(e, cell, false)))
                    {
                        e.Graphics.FillRectangle(brBk, r);

                        StringFormat sf = GetStringFormat(align);
                        r.Y += 2;

                        string strCellValue = cell == null || cell.Value == null ? "" : cell.Value.ToString();
                        e.Graphics.DrawString(strCellValue, e.CellStyle.Font, brFr, r, sf);

                        sf.Dispose();
                    }
                }
                else if (noDrawingCell != null)
                {
                    using (Pen p = new Pen(this.GridColor))
                    {
                        if (bottomDrawing)
                        {
                            e.Graphics.DrawLine(p, e.CellBounds.Left, e.CellBounds.Bottom - 1,
                                e.CellBounds.Right, e.CellBounds.Bottom - 1);
                        }

                        if (rightDrawing)
                        {
                            e.Graphics.DrawLine(p, e.CellBounds.Right - 1, e.CellBounds.Top,
                                e.CellBounds.Right - 1, e.CellBounds.Bottom);
                        }
                    }
                }
                else
                {
                    if (m_owner != null)
                        m_owner.OnPostDrawCell(this, e);

                    return;
                }

                if (m_owner != null)
                    m_owner.OnPostDrawCell(this, e);

                e.Handled = true;
            }

            private Color GetDrawingCellColor(DataGridViewCellPaintingEventArgs e, DataGridViewCell cell, bool isBackround)
            {
                if (cell.Selected)
                {
                    if (isBackround)
                        return e.CellStyle.SelectionBackColor;
                    else
                        return e.CellStyle.SelectionForeColor;
                }

                if (isBackround)
                    return e.CellStyle.BackColor;

                return e.CellStyle.ForeColor;
            }

            /*private void DrawCell(DataGridViewCellPaintingEventArgs e)
            {
                MergedCells firstMerge = null, notFirstMerge = null;
                bool rightDrawing = false, bottomDrawing = false;

                foreach (MergedCells merge in m_mergedCells)
                {
                    if (e.ColumnIndex == merge.MinColumnIndex && e.RowIndex == merge.MinRowIndex)
                    {
                        firstMerge = merge;
                        break;
                    }
                    else if (e.ColumnIndex >= merge.MinColumnIndex && e.ColumnIndex <= merge.MaxColumnIndex &&
                        e.RowIndex >= merge.MinRowIndex && e.RowIndex <= merge.MaxRowIndex)
                    {
                        if (e.ColumnIndex < merge.MaxColumnIndex)
                        {
                            if (e.RowIndex == merge.MaxRowIndex)
                                bottomDrawing = true;
                        }
                        else if (e.ColumnIndex == merge.MaxColumnIndex)
                        {
                            rightDrawing = true;

                            if (e.RowIndex == merge.MaxRowIndex)
                                bottomDrawing = true;
                        }

                        notFirstMerge = merge;
                        break;
                    }
                }

                if (firstMerge != null)
                {
                    e.PaintBackground(e.ClipBounds, true);

                    Rectangle r = e.CellBounds;

                    for (int i = firstMerge.MinColumnIndex + 1; i <= firstMerge.MaxColumnIndex; i++)
                    {
                        Rectangle r1 = this.GetCellDisplayRectangle(i, e.RowIndex, true);
                        r.Width += r1.Width;
                    }

                    for (int i = firstMerge.MinRowIndex + 1; i <= firstMerge.MaxRowIndex; i++)
                    {
                        Rectangle r1 = this.GetCellDisplayRectangle(e.ColumnIndex, i, true);
                        r.Height += r1.Height;
                    }

                    r.Width -= 1;
                    r.Height -= 1;

                    DataGridViewCell cell = this.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    DataGridViewContentAlignment align = cell.InheritedStyle.Alignment;

                    using (SolidBrush brBk = new SolidBrush(e.CellStyle.BackColor))
                    using (SolidBrush brFr = new SolidBrush(e.CellStyle.ForeColor))
                    {
                        e.Graphics.FillRectangle(brBk, r);

                        StringFormat sf = GetStringFormat(align);
                        r.Y += 2;

                        string strCellValue = cell == null || cell.Value == null ? "" : cell.Value.ToString();
                        e.Graphics.DrawString(strCellValue, e.CellStyle.Font, brFr, r, sf);
                    }
                    e.Handled = true;
                }
                else if (notFirstMerge != null)
                {
                    using (Pen p = new Pen(this.GridColor))
                    {
                        if (bottomDrawing)
                        {
                            e.Graphics.DrawLine(p, e.CellBounds.Left, e.CellBounds.Bottom - 1,
                                e.CellBounds.Right, e.CellBounds.Bottom - 1);
                        }

                        if (rightDrawing)
                        {
                            e.Graphics.DrawLine(p, e.CellBounds.Right - 1, e.CellBounds.Top,
                                e.CellBounds.Right - 1, e.CellBounds.Bottom);
                        }
                    }
                    e.Handled = true;
                }
            }*/

            private void OnCellPainting(object sender, DataGridViewCellPaintingEventArgs e)
            {
                if (e.RowIndex == -1)
                {
                    DrawColumn(e);
                }
                else
                {
                    DrawCell(e);
                }
            }

            private void OnScroll(object sender, ScrollEventArgs e)
            {
                foreach (MergedCells merge in m_mergedCells)
                {
                    for (int i = merge.MinRowIndex; i <= merge.MaxRowIndex; i++)
                    {
                        for (int j = merge.MinColumnIndex; j <= merge.MaxColumnIndex; j++)
                        {
                            this.InvalidateCell(j, i);
                        }
                    }
                }
            }
        }

        public interface IMergedDataGridViewOwner
        {
            void OnPostDrawColumn(MergedDataGridView grid, DataGridViewCellPaintingEventArgs e);
            void OnPostDrawCell(MergedDataGridView grid, DataGridViewCellPaintingEventArgs e);
        }
    }
}
