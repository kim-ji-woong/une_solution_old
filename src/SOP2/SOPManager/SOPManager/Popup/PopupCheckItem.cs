using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace SOPManager
{
    public partial class PopupCheckItem : Form
    {
        private Sections.SectionProcess m_section;
        private string m_strMission;

        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();

        public PopupCheckItem()
        {
            InitializeComponent();
            dataGridViewMission.Rows.Add();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SetCheckTask();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public void GetFirstRowData()
        {
            PropertiesProcess propertiesProcess = FormMain.Instance.GetPageLevel().GetPropertiesProcess();
            if (dataGridViewMission.Rows[0].Cells[2].Value == null)
                propertiesProcess.CheckItem = "";
            else
                propertiesProcess.CheckItem = (string)dataGridViewMission.Rows[0].Cells[2].Value.ToString();
            
        }

        private void AddGridData(int nProcessID)
        {
            dataGridViewMission.Rows.Clear();
            ArrayList arrCheckTask = FormMain.Instance.CheckTask;

            foreach (Data_CheckTask data in arrCheckTask)
            {
                if(nProcessID == data.ProcessID)
                {
                    DataGridViewRow gridRow = new DataGridViewRow();

                    DataGridViewCell cell = new DataGridViewTextBoxCell();
                    cell.Value = data.Category;
                    gridRow.Cells.Add(cell);

                    cell = new DataGridViewTextBoxCell();
                    cell.Value = data.SubCategory;
                    gridRow.Cells.Add(cell);

                    cell = new DataGridViewTextBoxCell();
                    cell.Value = data.TaskName;
                    gridRow.Cells.Add(cell);

                    cell = new DataGridViewTextBoxCell();
                    cell.Value = data.TargetCount;
                    gridRow.Cells.Add(cell);

                    cell = new DataGridViewTextBoxCell();
                    cell.Value = data.Position;
                    gridRow.Cells.Add(cell);

                    dataGridViewMission.Rows.Add(gridRow);
                }
            }
        }

        private void dataGridViewMission_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                foreach (DataGridViewCell cell in dataGridViewMission.SelectedCells)
                {
                    int nColumnCount = dataGridViewMission.ColumnCount;
                    if (cell.ColumnIndex + 1 < nColumnCount)
                    {
                        e.Handled = true;
                        dataGridViewMission.Rows[cell.RowIndex].Cells[cell.ColumnIndex + 1].Selected = true;
                    }
//                     else if (cell.ColumnIndex + 1 > nColumnCount)
//                     {
//                         e.Handled = true;
//                         dataGridViewMission.Rows[cell.RowIndex].Cells[0].Selected = true;
//                     }
                    else if (cell.ColumnIndex + 1 == nColumnCount)
                    {
                        if (cell.RowIndex < dataGridViewMission.RowCount - 1)
                            dataGridViewMission.Rows[cell.RowIndex].Cells[0].Selected = true;
                        else
                        {
                            if (dataGridViewMission.Rows[cell.RowIndex].Cells[0].Value == null || dataGridViewMission.Rows[cell.RowIndex].Cells[1].Value == null || dataGridViewMission.Rows[cell.RowIndex].Cells[2].Value == null) break;

                            string strDivision = dataGridViewMission.Rows[cell.RowIndex].Cells[0].Value.ToString();
                            string strValue = dataGridViewMission.Rows[cell.RowIndex].Cells[1].Value.ToString();
                            if (strDivision != "" && strValue != "")
                            {
                                AddGridRow_Mission(strDivision, strValue);
                                dataGridViewMission.Rows[cell.RowIndex + 1].Cells[0].Selected = true;
                            }
                        }
                    }
                    else
                    {
                        if (dataGridViewMission.Rows[cell.RowIndex].Cells[0].Value == null || dataGridViewMission.Rows[cell.RowIndex].Cells[1].Value == null) break;

                        if (cell.RowIndex == dataGridViewMission.RowCount)
                        {
                            string strDivision = dataGridViewMission.Rows[cell.RowIndex].Cells[0].Value.ToString();
                            string strValue = dataGridViewMission.Rows[cell.RowIndex].Cells[1].Value.ToString();
                            if (strDivision != "" && strValue != "")
                            {
                                AddGridRow_Mission(strDivision, strValue);
                                dataGridViewMission.Rows[cell.RowIndex + 1].Cells[0].Selected = true;
                            }
                        }
                    }
                    return;
                }
            }
            else if (e.KeyCode == Keys.Delete)
            {
                foreach (DataGridViewCell cell in dataGridViewMission.SelectedCells)
                {
                    if (dataGridViewMission.RowCount > 1)
                        dataGridViewMission.Rows.Remove(dataGridViewMission.Rows[cell.RowIndex]);
                }
            }
            else
            {
                foreach (DataGridViewCell cell in dataGridViewMission.SelectedCells)
                {
                    if (cell.ColumnIndex == 3)
                    {
                        
                    }
                }
            }
        }

        private void AddGridRow_Mission(string strCategory, string strSubCategory)
        {
            DataGridViewRow gridRow = new DataGridViewRow();
            DataGridViewCell cell = new DataGridViewTextBoxCell();
            cell.Value = strCategory;
            gridRow.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strSubCategory;
            gridRow.Cells.Add(cell);

            dataGridViewMission.Rows.Add(gridRow);
        }
        
        private void dataGridViewMission_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView grid = (DataGridView)sender;
            if (grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null) return;

            if (e.ColumnIndex == 3)
            {
                string strValue = grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

                bool isCheck = false;
                if(strValue != "")
                    isCheck = FormMain.Instance.GetPageLevel().numericCheck(strValue);

                if (!isCheck)
                {
                    MessageBox.Show("숫자 입력만 가능합니다.");
                    grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "";
                    
                    //grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = false;
                    //grid.BeginEdit(true);
                    //return;
                }
            }
        }

        public void GetCheckItem(Sections.SectionProcess section, string strMission)
        {
            m_section = section;
            m_strMission = strMission;
            Sections.SectionDataProcess data = (Sections.SectionDataProcess)section.Data;
            string strID = data.ComponentID;

            //if (data.CheckedItems == null || data.CheckedItems.Count == 0) return;

            dataGridViewMission.Rows.Clear();

            foreach (Sections.MissionItem info in data.MissionItems)
            {
                if (info.ArrCheckItem == null) 
                    info.ArrCheckItem = new ArrayList();

                if (info.ArrCheckItem.Count == 0)
                {
                    dataGridViewMission.Rows.Add();
                    return;
                }

                if (info.Mission == strMission)
                {
                    foreach (Sections.CheckedItem check in info.ArrCheckItem)
                    {
                        DataGridViewRow gridRow = new DataGridViewRow();
                        DataGridViewCell cell = new DataGridViewTextBoxCell();
                        cell.Value = check.Category;
                        gridRow.Cells.Add(cell);

                        cell = new DataGridViewTextBoxCell();
                        cell.Value = check.SubCategory;
                        gridRow.Cells.Add(cell);

                        cell = new DataGridViewTextBoxCell();
                        cell.Value = check.Item;
                        gridRow.Cells.Add(cell);

                        cell = new DataGridViewTextBoxCell();
                        cell.Value = check.ItemCount;
                        gridRow.Cells.Add(cell);

                        cell = new DataGridViewTextBoxCell();
                        cell.Value = check.Location;
                        gridRow.Cells.Add(cell);

                        dataGridViewMission.Rows.Add(gridRow);
                    }
                    break;
                }
            }
        }

        private void SetCheckTask()
        {
            Sections.SectionDataProcess data = (Sections.SectionDataProcess)m_section.Data;
            data.CheckedItems.Clear();
            foreach (Sections.MissionItem info in data.MissionItems)
            {
                if(info.ArrCheckItem == null)
                    info.ArrCheckItem = new ArrayList();

                if(info.Mission == m_strMission)
                {
                    info.ArrCheckItem.Clear();
                    foreach (DataGridViewRow row in dataGridViewMission.Rows)
                    {
                        if (row.Cells[2].Value != null)
                        {
                            Sections.CheckedItem check = new Sections.CheckedItem();
                            check.Category = row.Cells[0].Value.ToString();
                            check.SubCategory = row.Cells[1].Value.ToString();
                            check.Item = row.Cells[2].Value.ToString();
                            check.ItemCount = int.Parse(row.Cells[3].Value.ToString());

                            if (row.Cells[4].Value == null)
                                check.Location = "";
                            else
                                check.Location = row.Cells[4].Value.ToString();

                            //data.CheckedItems.Add(check);
                            info.ArrCheckItem.Add(check);
                        }
                    }
                }
            }
        }

        private void PopupCheckItem_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = this.PointToScreen(new Point(e.X, e.Y));
            }
        }

        private void PopupCheckItem_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_bLeftMouseDown == true)
                {
                    Point pt = this.PointToScreen(new Point(e.X, e.Y));
                    int dx = pt.X - m_ptMove.X;
                    int dy = pt.Y - m_ptMove.Y;
                    if (!(dx == 0 && dy == 0))
                    {
                        Point ptCur = this.Location;
                        this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
                        m_ptMove.X += dx;
                        m_ptMove.Y += dy;
                    }
                }
            }
        }

        private void PopupCheckItem_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;
        }

        private void label3_MouseDown(object sender, MouseEventArgs e)
        {
            PopupCheckItem_MouseDown(sender, e);
        }

        private void label3_MouseMove(object sender, MouseEventArgs e)
        {
            PopupCheckItem_MouseMove(sender, e);
        }

        private void label3_MouseUp(object sender, MouseEventArgs e)
        {
            PopupCheckItem_MouseUp(sender, e);
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            PopupCheckItem_MouseDown(sender, e);
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            PopupCheckItem_MouseMove(sender, e);
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            PopupCheckItem_MouseUp(sender, e);
        }
    }
}
