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
using System.Runtime.InteropServices;

namespace RoadMan
{
    public partial class FormLayer : Form
    {
        private bool m_sortDescending = true;
        private List<LayerData> m_arrLayers = null;
        private bool m_visibleLayerPriority = false;
        private bool m_mouseEditLayerPriority = false;
        private int m_nLayerPriorityIndex = 0;

        private bool m_isLClicked = false;
        private bool m_prevDragMove = false;

        public FormLayer()
        {
            InitializeComponent();
        }

        public void SetLayers(List<LayerData> arrLayers)
        {
            dataGridView1.Rows.Clear();
            m_arrLayers = arrLayers;

            foreach (LayerData data in arrLayers)
            {
				if (data.Enabled == false)
					continue;

                DataGridViewRow row = new DataGridViewRow();

                DataGridViewCheckBoxCell checkedCell = new DataGridViewCheckBoxCell();
                checkedCell.Value = data.Visible;
                row.Cells.Add(checkedCell);

                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = GetLayerNameValue(data);
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Style.BackColor = data.Color;
                //cell.Style.SelectionBackColor = data.Color;
                row.Cells.Add(cell);

                row.Tag = data;
                dataGridView1.Rows.Add(row);
            }
        }
		
		public void ChangeShowLayer()
		{
			dataGridView1.Rows.Clear();	
			foreach (LayerData data in m_arrLayers)
			{
				if (data.Enabled == false)
					continue;

				DataGridViewRow row = new DataGridViewRow();

				DataGridViewCheckBoxCell checkedCell = new DataGridViewCheckBoxCell();
				checkedCell.Value = data.Visible;
				row.Cells.Add(checkedCell);

				DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
				cell.Value = GetLayerNameValue(data);
				row.Cells.Add(cell);

				cell = new DataGridViewTextBoxCell();
				cell.Style.BackColor = data.Color;
				//cell.Style.SelectionBackColor = data.Color;
				row.Cells.Add(cell);

				row.Tag = data;
				dataGridView1.Rows.Add(row);
			}
		}


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                if (e.ColumnIndex == 0)
                {
                    if (m_sortDescending)
                        dataGridView1.Sort(colVisible, ListSortDirection.Descending);
                    else
                        dataGridView1.Sort(colVisible, ListSortDirection.Ascending);

                    m_sortDescending = !m_sortDescending;
                }
                else if (e.ColumnIndex == 2)
                {
                    dataGridView1.Sort(new GridViewColorColumnSort());
                }
            }
            else if (e.RowIndex >= 0 && e.ColumnIndex == 0)
            {
                DataGridViewCell cell = dataGridView1.Rows[e.RowIndex].Cells[0];
                LayerData layer = (LayerData)dataGridView1.Rows[e.RowIndex].Tag;

                layer.Visible = (bool)cell.Value;

                if (layer.LinkedLayer != null)
                    layer.LinkedLayer.Hidden = !layer.Visible;

                FormMain.Instance.RefreshView();
            }
        }

        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.IsCurrentCellDirty)
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        public List<LayerData> GetLayerList()
        {
            return m_arrLayers;
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (e.RowIndex >= 0 && e.ColumnIndex == 2)
                {
                    DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                    LayerData data = (LayerData)row.Tag;

                    if (data != null)
                    {
                        FormColor frm = new FormColor();
						DialogFormFrame frameColor = new DialogFormFrame(frm);
                        frm.Color = data.Color;
                        frm.Alpha = data.Alpha;
                        frm.Text = "도면층 색상 - " + data.LayerName;

						if (frameColor.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            row.Cells[2].Style.BackColor = frm.Color;

                            if (data.Color == frm.Color && data.Alpha == frm.Alpha)
                                return;
                            else
                            {
                                data.Color = frm.Color;
                                data.Alpha = frm.Alpha;
                                data.LinkedLayer.LineColor = Color.FromArgb(data.Alpha, data.Color);
                                FormMain.Instance.RefreshView();
                            }
                        }
                    }
                }
            }
        }

        private void ShowLayerPriority(bool visible, bool isMouseEdit)
        {
            if (m_visibleLayerPriority == visible && m_mouseEditLayerPriority == isMouseEdit)
                return;

            m_visibleLayerPriority = visible;
            m_mouseEditLayerPriority = isMouseEdit;

            if (m_visibleLayerPriority)
            {
                if (m_mouseEditLayerPriority)
                {
                    menuLayerPriority.Checked = false;
                    menuLayerPriorityOneByOne.Checked = true;

                    menuLayerPriority.Text = "도면층 우선순위 보기";
                    menuLayerPriorityOneByOne.Text = "도면층 우선순위 감추기";
                }
                else
                {
                    menuLayerPriority.Checked = true;
                    menuLayerPriorityOneByOne.Checked = false;

                    menuLayerPriority.Text = "도면층 우선순위 감추기";
                    menuLayerPriorityOneByOne.Text = "도면층 우선순위 매기기";
                }

                menuSortLayer.Enabled = true;
                menuSortLayerInverse.Enabled = true;
                m_nLayerPriorityIndex = 1;
            }
            else
            {
                menuLayerPriority.Checked = false;
                menuLayerPriorityOneByOne.Checked = false;

                menuLayerPriority.Text = "도면층 우선순위 보기";
                menuLayerPriorityOneByOne.Text = "도면층 우선순위 매기기";

                menuSortLayer.Enabled = false;
                menuSortLayerInverse.Enabled = false;
            }

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow)
                    continue;

                LayerData data = (LayerData)row.Tag;

                if (data == null)
                    continue;

                row.Cells[1].Value = GetLayerNameValue(data);
            }
        }

        private void menuLayerPriority_Click(object sender, EventArgs e)
        {
            if (m_visibleLayerPriority)
                ShowLayerPriority(m_mouseEditLayerPriority, false);
            else
                ShowLayerPriority(true, false);
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && dataGridView1.Rows.Count > 0)
            {
                contextMenuStrip1.Show(dataGridView1, e.Location);
            }
        }

        private DataGridViewRow[] GetIndexLayers()
        {
            int nLayerCount = dataGridView1.Rows.Count;

            if (nLayerCount == 0)
                return null;

            DataGridViewRow[] rows = new DataGridViewRow[nLayerCount];

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                LayerData data = (LayerData)row.Tag;
                rows[data.LayerIndex - 1] = row;
            }

            return rows;
        }

        private void menuSortLayer_Click(object sender, EventArgs e)
        {
            DataGridViewRow[] rows = GetIndexLayers();
            dataGridView1.Rows.Clear();

            foreach (DataGridViewRow row in rows)
            {
                dataGridView1.Rows.Add(row);
            }
        }

        private void menuSortLayerInverse_Click(object sender, EventArgs e)
        {
            DataGridViewRow[] rows = GetIndexLayers();
            dataGridView1.Rows.Clear();

            foreach (DataGridViewRow row in rows)
            {
                dataGridView1.Rows.Insert(0, row);
            }
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left && m_visibleLayerPriority && m_mouseEditLayerPriority)
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                    LayerData data = (LayerData)row.Tag;

                    if (data.LayerIndex == m_nLayerPriorityIndex)
                    {
                        m_nLayerPriorityIndex++;
                        return;
                    }
                    else if (data.LayerIndex < m_nLayerPriorityIndex)
                        return;

                    int nTargetIndex = data.LayerIndex;

                    // dxfControl의 Layer 순서 바꾸기
                    int nLayerCount = FormMain.Instance.CurrentDXFControl.Layers.Count;
                    FormMain.Instance.CurrentDXFControl.Layers.Remove(data.LinkedLayer);
                    FormMain.Instance.CurrentDXFControl.Layers.Insert(nLayerCount - m_nLayerPriorityIndex, data.LinkedLayer);

                    // Grid의 Layer 순서 바꾸기
                    foreach (DataGridViewRow _row in dataGridView1.Rows)
                    {
                        if (row == _row)
                            continue;

                        LayerData _data = (LayerData)_row.Tag;

                        if (_data.LayerIndex < m_nLayerPriorityIndex || _data.LayerIndex > nTargetIndex)
                            continue;

                        _data.LayerIndex++;
                        _row.Cells[1].Value = GetLayerNameValue(_data);
                    }

                    data.LayerIndex = m_nLayerPriorityIndex++;
                    row.Cells[1].Value = GetLayerNameValue(data);

                    FormMain.Instance.RefreshView();
                }
            }
        }

        private string GetLayerNameValue(LayerData data)
        {
            if (m_visibleLayerPriority)
            {
                if (m_mouseEditLayerPriority)
                    return data.LayerName + "_(" + data.LayerIndex.ToString() + ")";
                else
                    return data.LayerName + " (" + data.LayerIndex.ToString() + ")";
            }

            return data.LayerName;
        }

        public void HideLayerPriority()
        {
            if (m_visibleLayerPriority)
                ShowLayerPriority(false, false);
        }

        private void dataGridView1_DragEnter(object sender, DragEventArgs e)
        {
            m_prevDragMove = false;
        }

        // row를 nRowIndex에 삽입한다.
        private void InsertLayerToGrid(DataGridViewRow rowSrc, int nRowIndex)
        {
            DataGridViewRow rowTarget = dataGridView1.Rows[nRowIndex];

            if (rowSrc.Index < nRowIndex)
            {
                dataGridView1.Rows.Remove(rowSrc);
                dataGridView1.Rows.Insert(nRowIndex - 1, rowSrc);
            }
            else
            {
                dataGridView1.Rows.Remove(rowSrc);
                dataGridView1.Rows.Insert(nRowIndex, rowSrc);
            }

            LayerData layerSrc = (LayerData)rowSrc.Tag;
            LayerData layerTarget = (LayerData)rowTarget.Tag;

            int nSrcIndex = layerSrc.LayerIndex;
            int nTrgIndex = layerTarget.LayerIndex;
            
            ArrayList layers = FormMain.Instance.CurrentDXFControl.Layers;
            layers.Remove(layerSrc.LinkedLayer);

            if (nSrcIndex > nTrgIndex)
            {
                for (int i=nSrcIndex-1;i>nTrgIndex;i--)
                {
                    DataGridViewRow row = GetRow(i);

                    if (row == null)
                        return;

                    LayerData data = (LayerData)row.Tag;
                    data.LayerIndex++;

                    row.Cells[1].Value = GetLayerNameValue(data);
                }

                layerTarget.LayerIndex++;
                rowTarget.Cells[1].Value = GetLayerNameValue(layerTarget);

                layerSrc.LayerIndex = nTrgIndex;
                rowSrc.Cells[1].Value = GetLayerNameValue(layerSrc);

                layers.Insert(layers.Count - nTrgIndex + 1, layerSrc.LinkedLayer);
            }
            else
            {
                for (int i=nSrcIndex+1;i<nTrgIndex;i++)
                {
                    DataGridViewRow row = GetRow(i);

                    if (row == null)
                        return;

                    LayerData data = (LayerData)row.Tag;
                    data.LayerIndex--;

                    row.Cells[1].Value = GetLayerNameValue(data);
                }

                layerSrc.LayerIndex = nTrgIndex - 1;
                rowSrc.Cells[1].Value = GetLayerNameValue(layerSrc);

                layers.Insert(layers.Count - (nTrgIndex - 1), layerSrc.LinkedLayer);
            }
        }

        private DataGridViewRow GetRow(int nLayerIndex)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                LayerData data = (LayerData)row.Tag;

                if (data.LayerIndex == nLayerIndex)
                    return row;
            }

            return null;
        }

        private void dataGridView1_DragDrop(object sender, DragEventArgs e)
        {
            dataGridView1.AllowDrop = false;
            m_isLClicked = false;

            if (m_prevDragMove && m_visibleLayerPriority && !m_mouseEditLayerPriority)
            {
                DataGridViewRow rowDrag = (DataGridViewRow)e.Data.GetData(typeof(DataGridViewRow));

                if (rowDrag == null)
                    return;

                Point ptCurrent = this.PointToClient(new Point(e.X, e.Y));

                DataGridView.HitTestInfo info = dataGridView1.HitTest(ptCurrent.X, ptCurrent.Y);

                if (info.RowIndex < 0 || info.ColumnIndex < 0)
                    return;

                if (info.RowIndex == rowDrag.Index)
                    return;

                InsertLayerToGrid(rowDrag, info.RowIndex);
                FormMain.Instance.RefreshView();
            }

            m_prevDragMove = false;
        }

        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_isLClicked = true;
            }
        }

        private void dataGridView1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_isLClicked = false;
                dataGridView1.AllowDrop = false;
            }
        }

        private void dataGridView1_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_isLClicked && m_visibleLayerPriority && !m_mouseEditLayerPriority)
            {
                if (dataGridView1.SelectedCells.Count > 0)
                {
                    dataGridView1.AllowDrop = true;
                    DataGridViewRow rowDrag = dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex];
                    dataGridView1.DoDragDrop(rowDrag, DragDropEffects.Move);
                }
            }
        }

        private void dataGridView1_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
            m_prevDragMove = true;

            Point ptCurrent = this.PointToClient(new Point(e.X, e.Y));

            if (ptCurrent.Y <= (dataGridView1.Font.Height / 2))
            {
                if (dataGridView1.FirstDisplayedScrollingRowIndex > 0)
                    dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.FirstDisplayedScrollingRowIndex - 1;
            }
            else if (ptCurrent.Y >= dataGridView1.ClientSize.Height - dataGridView1.Font.Height / 2)
            {
                if (dataGridView1.FirstDisplayedScrollingRowIndex < dataGridView1.Rows.Count - 1)
                    dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.FirstDisplayedScrollingRowIndex + 1;
            }
        }

        private void menuLayerPriorityOneByOne_Click(object sender, EventArgs e)
        {
            if (m_visibleLayerPriority)
                ShowLayerPriority(!m_mouseEditLayerPriority, true);
            else
                ShowLayerPriority(true, true);
        }

        private void FormLayer_Load(object sender, EventArgs e)
        {
            colVisible.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colLayerName.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colLayerColor.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

		private void button1_Click(object sender, EventArgs e)
		{
			FormMain.Instance.HideLayerForm();
		}
    }

    public class GridViewColorColumnSort : System.Collections.IComparer
    {
        private static bool m_isAscending = true;

        public GridViewColorColumnSort()
        {
            m_isAscending = !m_isAscending;
        }

        public int Compare(object obj1, object obj2)
        {
            DataGridViewRow row1 = (DataGridViewRow)obj1;
            DataGridViewRow row2 = (DataGridViewRow)obj2;

            DataGridViewCell cell1 = row1.Cells[2];
            DataGridViewCell cell2 = row2.Cells[2];

            Color col1 = cell1.Style.BackColor;
            Color col2 = cell2.Style.BackColor;

            int nColor1 = ((int)col1.R) * 256 * 256 + ((int)col1.G) * 256 + (int)col1.B;
            int nColor2 = ((int)col2.R) * 256 * 256 + ((int)col2.G) * 256 + (int)col2.B;

            return m_isAscending ? nColor1 - nColor2 : nColor2 - nColor1;
        }
    }
}
