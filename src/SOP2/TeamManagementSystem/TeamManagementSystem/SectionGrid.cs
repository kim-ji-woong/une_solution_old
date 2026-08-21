using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Runtime.InteropServices;

namespace TeamManagementSystem
{
    public partial class SectionGrid
    {
        protected FormMain m_frmMain = null;
        protected Control m_frmParent = null;

        //protected DataGridView m_dataGrid = null;
        //protected DataGridViewTextBoxColumn[] m_arrColumn = null;//new DataGridViewTextBoxColumn[Count];

        //protected MergedDataGridView m_dataGrid = null;
        protected DataGridView m_dataGrid = null;
        protected RichTextBox m_titleTextBox = null;
        /*protected DataTable m_table = null;
        protected DataColumn[] m_column = null;*/

        protected SectionGrid m_sectionParent = null;

        protected ArrayList m_arrChildSection = new ArrayList();

        protected bool m_isHidden = false;
        protected EditBox m_editBox = new EditBox();

        //protected int m_nCount = 0;
        protected int x = 0, y = 0;
        int m_nWidth = 0, m_nHeight = 0;
        int m_nDiffTextX = 0, m_nDiffTextY = 0;
        int m_nDiffEditX = 0, m_nDiffEditY = 0;
        int m_nInterpolationX = 0, m_nInterpolationY = 0;
        int m_nDiff = 4;

        // Child Section의 영역
        protected int m_nChildBegin = int.MaxValue;
        protected int m_nChildEnd = int.MinValue;
        protected static bool[] m_autoAlignRefresh = new bool[3] { false, false, false };

        protected static Pen BOUNDARY_PEN = new Pen(Color.FromArgb(185, 255, 185), 1);
        protected static SolidBrush BOUNDARY_BRUSH = new SolidBrush(Color.FromArgb(100, 128, 128, 192));
        protected static Pen LINK_PEN = new Pen(Color.Gray, 2);

        private static int m_nHorzSpace = 15;    // 수평 간격
        private static int m_nVertSpace = 50;    // 수직 간격

        private static bool m_multiSelect = false;
        //private static ArrayList m_arrSelectedSections = new ArrayList();
        private static ArrayList[] m_arrSelectedSections = new ArrayList[3] {new ArrayList(), new ArrayList(), new ArrayList()};

        private int m_nTag;
        private int m_nSectionType = 0;
        private bool m_readOnly = true;
        private bool m_isDragDrop = false;

        public static bool GetAutoRefresh(int nSectionType)
        {
            return m_autoAlignRefresh[nSectionType];
        }

        public static void SetAutoRefresh(int nSectionType, bool refresh)
        {
            m_autoAlignRefresh[nSectionType] = refresh;
        }

        /*public bool AutoAlignRefresh
        {
            get { return m_autoAlignRefresh; }
            set {
                m_autoAlignRefresh = value; 
            }
        }*/

        public int Tag
        {
            get { return m_nTag; }
            set { m_nTag = value; }
        }
        
        public bool IsDragDrop
        {
            get { return m_isDragDrop; }
            set { m_isDragDrop = value; }
        }

        [DllImport("USER32.DLL", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);


        // nSectionType : RegularTeam(0), 평일비상조직(1), 휴일비상조직(2)
        public SectionGrid(FormMain frmMain, Control frmParent, int nColumnCount, int nSectionType, bool readOnly = true)
        {
            m_frmMain = frmMain;
            m_frmParent = frmParent;
            m_nSectionType = nSectionType;
            m_readOnly = readOnly;

            CreateDataGrid(nColumnCount);
            CreateTitle();
            SetInitSize();
        }

        //public void CreateGridView(int nColumn)
        //{
        //    m_arrColumn = new DataGridViewTextBoxColumn[nColumn];
        //    m_dataGrid = new SectionGridView(this);
        //    m_dataGrid.Parent = m_frmParent;
            
        //    //((System.ComponentModel.ISupportInitialize)(this.m_dataGrid)).BeginInit();

        //    //DataGridViewTextBoxColumn Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
        //    //DataGridViewTextBoxColumn Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
        //    for (int i = 0; i < nColumn; i++)
        //        m_arrColumn[i] = new System.Windows.Forms.DataGridViewTextBoxColumn();
            
        //    m_dataGrid.AllowUserToAddRows = false;
        //    m_dataGrid.AllowUserToDeleteRows = false;
        //    m_dataGrid.AllowUserToResizeColumns = false;
        //    m_dataGrid.AllowUserToResizeRows = false;
        //    m_dataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        //    //m_dataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
        //    //Column1,
        //    //Column2});

        //    DataGridViewColumn[] arrColumn = new DataGridViewColumn[nColumn];
        //    for (int i=0; i<m_arrColumn.Count(); i++)
        //    {
        //        arrColumn[i] = new System.Windows.Forms.DataGridViewColumn();
        //        m_dataGrid.Columns.Add(arrColumn[i]);
        //    }
            
        //    m_dataGrid.Dock = System.Windows.Forms.DockStyle.None;
        //    m_dataGrid.Location = new System.Drawing.Point(100, 10);
        //    m_dataGrid.MultiSelect = false;
        //    m_dataGrid.Name = "Grid";
        //    m_dataGrid.ReadOnly = false;
        //    m_dataGrid.ColumnHeadersVisible = false;
        //    m_dataGrid.RowHeadersVisible = false;
        //    m_dataGrid.RowTemplate.Height = 23;
        //    m_dataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
        //    m_dataGrid.Size = new System.Drawing.Size(350, 50);
        //    m_dataGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
        //    // 
        //    //// Column1
        //    //// 
        //    //Column1.HeaderText = "구분";
        //    //Column1.Name = "Column1";
        //    //Column1.ReadOnly = true;
        //    //Column1.Width = 80;
        //    //// 
        //    //// Column2
        //    //// 
        //    //Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
        //    //Column2.HeaderText = "내용";
        //    //Column2.Name = "Column2";
        //    //Column2.ReadOnly = true;

        //    for (int i = 0; i < arrColumn.Count(); i++)
        //    {
        //        arrColumn[i].HeaderText = (i+1).ToString();
        //        arrColumn[i].Name = "Column" + (i+1).ToString();
        //    }
            
        //    m_nCount = 0;
        //    //((System.ComponentModel.ISupportInitialize)(this.m_dataGrid)).EndInit();
        //}

        //public void AddRowData()
        //{
        //    m_nCount++;
        //    DataGridViewRow gridRow = new DataGridViewRow();
        //    DataGridViewCell cell = null;

        //    for(int i = 0; i<m_arrColumn.Count(); i++)
        //    {
        //        cell = new DataGridViewTextBoxCell();
        //        cell.Value = "aaa" + i.ToString();
        //        gridRow.Cells.Add(cell);
        //    }

        //    m_dataGrid.Rows.Add(gridRow);

        //    m_dataGrid.Size = new System.Drawing.Size(m_arrColumn.Count() * 100 + 3, m_nCount * 23 + 3);
        //}

        public bool ReadOnly
        {
            get { return m_readOnly; }
            set
            {
                if (m_readOnly != value)
                {
                    m_readOnly = value;
                    m_titleTextBox.ReadOnly = value;
                    m_dataGrid.ReadOnly = value;
                    m_dataGrid.AllowUserToAddRows = !value;

                    if (!m_dataGrid.ReadOnly)
                    {
                        m_dataGrid.Visible = true;
                        m_dataGrid.Left = m_titleTextBox.Left;
                        m_dataGrid.Top = m_titleTextBox.Bottom;
                    }

                    if (!m_dataGrid.AllowUserToAddRows)
                    {
                        if (m_dataGrid.Rows.Count == 0)
                            m_dataGrid.Visible = false;
                        else
                            m_dataGrid.Height -= m_titleTextBox.Height;
                        
                        m_editBox.RectSize = new Size(m_editBox.RectSize.Width, m_editBox.RectSize.Height - m_titleTextBox.Height);
                    }
                    /*else
                        m_editBox.RectSize = new Size(m_editBox.RectSize.Width, m_editBox.RectSize.Height + m_titleTextBox.Height);*/

                    foreach (SectionGrid section in m_arrChildSection)
                    {
                        section.ReadOnly = value;
                    }
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////
        public void CreateDataGrid(int nColumn)
        {
            int nColWidth = 100;

            //m_dataGrid = new System.Windows.Forms.DataGrid();
            //m_dataGrid = new MergedDataGridView();
            m_dataGrid = new DataGridView();
            m_dataGrid.Parent = m_frmParent;
            //m_dataGrid.CaptionVisible = false;
            //m_dataGrid.AllowUserToAddRows = false;
            m_dataGrid.ColumnHeadersVisible = false;
            m_dataGrid.DataMember = "";
            //m_dataGrid.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            m_dataGrid.Location = new System.Drawing.Point(12, 12);
            m_dataGrid.Name = "dataGrid1";
            m_dataGrid.RowHeadersVisible = false;
            //m_dataGrid.RowHeaderWidth = 0;
            //m_dataGrid.BorderStyle = BorderStyle.None;
            m_dataGrid.BorderStyle = BorderStyle.FixedSingle;
            //m_dataGrid.ReadOnly = true;
            
            // HScroll이 생기는 현상을 막기 위하여 필요한 너비보다 3을 더한다.
            m_dataGrid.Size = new System.Drawing.Size(nColWidth * nColumn + 3, 100);
            m_dataGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            m_dataGrid.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUp);
            m_dataGrid.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMouseMove);
            m_dataGrid.RowsAdded += new DataGridViewRowsAddedEventHandler(this.dataGrid_RowsAdded);
            m_dataGrid.KeyDown += new KeyEventHandler(this.dataGrid_KeyDown);
            m_dataGrid.Enter += new EventHandler(this.dataGrid_Enter);
            m_dataGrid.Leave += new EventHandler(this.dataGrid_Leave);
            m_dataGrid.CellValueChanged += new DataGridViewCellEventHandler(dataGrid_CellValueChanged);

            m_dataGrid.ScrollBars = ScrollBars.None;

            m_dataGrid.RowsDefaultCellStyle.BackColor = Color.White;
            m_dataGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.AliceBlue;

            /*m_table = new DataTable("classtable");
            m_column = new DataColumn[nColumn];

            for (int i = 0; i < nColumn; i++)
            {
                m_column[i] = new DataColumn();
                m_column[i].DataType = Type.GetType("System.String");
                m_column[i].ColumnName = "Column" + i.ToString();
                m_column[i].AllowDBNull = false;
                m_table.Columns.Add(m_column[i]);
            }
            m_nCount = 0;*/

            //AddDataSource();

            for (int i = 0; i < nColumn; i++)
            {
                DataGridViewColumn column;

                if (m_nSectionType == 0 || i == 0)
                {
                    column = new DataGridViewColumn(new DataGridViewTextBoxCellEx());
                    column.Width = nColWidth;
                }
                else
                {
                    column = new DataGridViewCheckBoxColumn(false);
                    column.Width = 20;
                    m_dataGrid.Size = new Size(m_dataGrid.Size.Width - (nColWidth - column.Width), m_dataGrid.Size.Height);
                }

                column.HeaderText = string.Format("Text{0}", i + 1);
                column.Name = string.Format("Column{0}", i + 1);
                column.Resizable = DataGridViewTriState.False;

                m_dataGrid.Columns.Add(column);

                /*m_dataGrid.Columns.Add(string.Format("Column{0}", i + 1), string.Format("Text{0}", i + 1));

                DataGridViewColumn col = m_dataGrid.Columns[m_dataGrid.Columns.Count - 1];
                col.Resizable = DataGridViewTriState.False;

                if (m_nSectionType == 0 || i == 0)
                    col.Width = nColWidth;
                else
                {
                    col.Width = 20;
                    m_dataGrid.Size = new Size(m_dataGrid.Size.Width - (nColWidth - col.Width), m_dataGrid.Size.Height);
                }*/
            }

            //m_dataGrid.AddMergedRow(0, "TeamName");

            // TextBox 및 Grid의 크기에 맞게 Section 크기를 변경시킨다.
            dataGrid_RowsAdded(null, null);
        }

        private void CreateTitle()
        {
            // RichTextBox(팀 이름)
            m_titleTextBox = new RichTextBox();
            m_titleTextBox.Parent = m_frmParent;
            m_titleTextBox.Text = "";
            m_titleTextBox.Multiline = false;
            m_titleTextBox.BorderStyle = BorderStyle.None;
            m_titleTextBox.Font = new Font(m_dataGrid.DefaultCellStyle.Font, m_dataGrid.DefaultCellStyle.Font.Style);
            m_titleTextBox.SetBounds(0, 0, m_dataGrid.Size.Width, m_dataGrid.Rows[0].Height);
            m_titleTextBox.SelectionAlignment = HorizontalAlignment.Center;
            m_titleTextBox.BackColor = Color.LightGray;

            m_titleTextBox.MouseDown += new MouseEventHandler(this.OnMouseDown);
            m_titleTextBox.MouseUp += new MouseEventHandler(this.OnMouseUp);
            m_titleTextBox.MouseMove += new MouseEventHandler(this.OnMouseMove);
            m_titleTextBox.Enter += new EventHandler(titleTextBox_Enter);
            m_titleTextBox.Leave += new EventHandler(titleTextBox_Leave);

            m_titleTextBox.AllowDrop = true;
            m_titleTextBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.titleTextBox_DragDrop);
        }

        public void EditTextBoxData(string strTitle)
        {
            m_titleTextBox.Text = strTitle;
        }

        public string GetTitle()
        {
            return m_titleTextBox.Text;
        }

        public DataGridView GetDataGrid()
        {
            return m_dataGrid;
        }

        public  SectionGrid GetSectionParent()
        {
            return m_sectionParent;
        }

        public void AddRowData(string strName, string strPhoneNumber, int nMemberID, bool check, bool refresh = false)
        {
            m_dataGrid.Visible = true;

            DataGridViewRow row = new DataGridViewRow();
            row.Resizable = DataGridViewTriState.False;

            if (m_frmMain.EditMode)
            {
                bool isCheck = false;
                for (int i = 0; i < m_dataGrid.Rows.Count-1; i++)
                {
                    //중복확인
                    if ((int)m_dataGrid.Rows[i].Cells[0].Tag == nMemberID)
                    {
                        isCheck = true;
                    }
                }
                
                if(!isCheck)
                {
                    DataGridViewTextBoxCellEx cell = new DataGridViewTextBoxCellEx();
                    cell.Value = strName;
                    cell.Tag = nMemberID;
                    cell.PrevText = strName;
                    row.Cells.Add(cell);

                    //if (m_dataGrid.Columns.Count >= 2)
                    if (m_nSectionType == 0)
                    {
                        cell = new DataGridViewTextBoxCellEx();
                        cell.Value = strPhoneNumber;
                        cell.PrevText = strPhoneNumber;
                        row.Cells.Add(cell);
                    }
                    else
                    {
                        DataGridViewCheckBoxCell checkCell = new DataGridViewCheckBoxCell();
                        checkCell.Value = check;
                        row.Cells.Add(checkCell);
                    }

                    m_autoAlignRefresh[m_nSectionType] = refresh;
                    m_dataGrid.Rows.Add(row);
                    m_autoAlignRefresh[m_nSectionType] = true;
                }
            }
            else
            {
                DataGridViewTextBoxCellEx cell = new DataGridViewTextBoxCellEx();
                cell.Value = strName;
                cell.Tag = nMemberID;
                cell.PrevText = strName;
                row.Cells.Add(cell);

                //if (m_dataGrid.Columns.Count >= 2)
                if (m_nSectionType == 0)
                {
                    cell = new DataGridViewTextBoxCellEx();
                    cell.Value = strPhoneNumber;
                    cell.PrevText = strPhoneNumber;
                    row.Cells.Add(cell);
                }
                else
                {
                    DataGridViewCheckBoxCell checkCell = new DataGridViewCheckBoxCell();
                    checkCell.Value = check;
                    row.Cells.Add(checkCell);
                }

                m_autoAlignRefresh[m_nSectionType] = refresh;
                m_dataGrid.Rows.Add(row);
                m_autoAlignRefresh[m_nSectionType] = true;
            }

            int nGridHeight = 0;
            
            foreach (DataGridViewRow gridRow in m_dataGrid.Rows)
            {
                nGridHeight += gridRow.Height;
            }

            // Grid가 Scroll 되는 것을 막기 위하여 전체 크기와 필요 크기간에 약간의 간격을 둔다.
            m_dataGrid.Height = nGridHeight + 3;
        }

        private void dataGrid_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            // Grid가 Scroll 되는 것을 막기 위하여 전체 크기와 필요 크기간에 약간의 간격을 둔다.
            int nGridHeight = 3;

            foreach (DataGridViewRow gridRow in m_dataGrid.Rows)
            {
                nGridHeight += gridRow.Height;
                gridRow.Resizable = DataGridViewTriState.False;
            }

            m_dataGrid.Height = nGridHeight;

            if (m_titleTextBox == null)
            {
                //x = m_dataGrid.Left - m_nDiff / 2;
                //y = m_dataGrid.Top - m_dataGrid.Rows[0].Height - m_nDiff / 2;
                this.Size = new Size(this.Size.Width, nGridHeight + m_dataGrid.Rows[0].Height + m_nDiff);
            }
            else
            {
                //x = m_dataGrid.Left - m_nDiff / 2;
                //y = m_titleTextBox.Top - m_nDiff / 2;
                this.Size = new Size(this.Size.Width, nGridHeight + m_titleTextBox.Height + m_nDiff);
            }
            
            if (m_autoAlignRefresh[m_nSectionType])
                m_frmMain.AutoAlign(true, m_frmMain.GetSections(m_nSectionType));
        }

        private void dataGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (m_readOnly)
                    return;

                ArrayList arrRemove = new ArrayList();

                foreach (DataGridViewCell cell in m_dataGrid.SelectedCells)
                {
                    if (cell.RowIndex == m_dataGrid.Rows.Count - 1)
                        continue;

                    if (!arrRemove.Contains(cell.RowIndex))
                        arrRemove.Add(cell.RowIndex);
                }

                arrRemove.Sort();

                int nCount = arrRemove.Count;

                for (int i = nCount - 1; i >= 0; i--)
                {
                    if (m_dataGrid.Rows.Count <= 1 && (int)arrRemove[i] == 0)
                        continue;
                    else
                        m_dataGrid.Rows.RemoveAt((int)arrRemove[i]);
                }

                if (nCount > 0)
                    dataGrid_RowsAdded(null, null);
            }
        }

        public static ArrayList GetSelectedSections(int nSectionType)
        {
            return m_arrSelectedSections[nSectionType];
        }

        private void SingleSelect(bool isSelect)
        {
            if (isSelect)
            {
                bool refresh = false;
                int nSelectedCount = m_arrSelectedSections[m_nSectionType].Count;

                if (nSelectedCount > 1 || nSelectedCount == 0)
                    refresh = true;
                else
                {
                    if (m_arrSelectedSections[m_nSectionType].Contains(this))
                        return;
                    else
                        refresh = true;
                }

                m_arrSelectedSections[m_nSectionType].Clear();
                m_arrSelectedSections[m_nSectionType].Add(this);

                if (!m_readOnly)
                {
                    m_dataGrid.ReadOnly = false;
                    m_titleTextBox.ReadOnly = false;
                }
                
                //string str = m_titleTextBox.Text;

                m_frmMain.TeamMode = m_nSectionType;

                if (refresh)
                    m_frmMain.Refresh();
            }
            else
            {
                if (m_arrSelectedSections[m_nSectionType].Contains(this))
                {
                    m_arrSelectedSections[m_nSectionType].Remove(this);
                    m_frmMain.Refresh();
                }

                m_dataGrid.ClearSelection();
            }
        }

        private void dataGrid_Enter(object sender, EventArgs e)
        {
            SingleSelect(true);
        }

        private void dataGrid_Leave(object sender, EventArgs e)
        {
            SingleSelect(false);
        }

        private void RollbackCellText(DataGridViewCell cell)
        {
            if (cell.GetType() == typeof(DataGridViewTextBoxCellEx))
            {
                ((DataGridViewTextBoxCellEx)cell).Value = ((DataGridViewTextBoxCellEx)cell).PrevText;
            }
            else
                cell.Value = "";
        }

        // Return 값 : 중복없음(0), 중복이 존재함(-1), 이름이 같은 팀이 존재하지만 ID가 다른것으로 대체되었음(1)
        private int CheckDuplication(string strRegularTeamName, DataGridViewCell cell)
        {
            ArrayList arrTeamID = new ArrayList();

            foreach (DataGridViewRow row in m_dataGrid.Rows)
            {
                if (row.Index == cell.RowIndex)
                    continue;

                if (row.Cells[0].Value == null)
                    continue;

                if (row.Cells[0].Value.ToString() == cell.Value.ToString())
                {
                    try
                    {
                        int nTeamID = (int)row.Cells[0].Tag;
                        arrTeamID.Add(nTeamID);
                    }
                    catch (Exception)
                    {
                        return -1;
                    }
                }
            }

            int nCount = arrTeamID.Count;
            if (nCount == 0)
                return 0;

            ArrayList arrSections = m_frmMain.FindSection(strRegularTeamName, 0);
            int nSectionCount = arrSections.Count;

            if (nCount >= nSectionCount)
                return -1;

            FormTeamList frm = new FormTeamList(arrSections);

            foreach (int nTeamID in arrTeamID)
            {
                SectionGrid section = m_frmMain.FindSection(nTeamID, 0);
                if (section == null) continue;

                string strPath = SectionGrid.GetFullPath(section);
                frm.AddNoUsingItem(strPath);
            }

            if (frm.ShowDialog() == DialogResult.Cancel)
                return -1;

            int nSelectedIndex = frm.GetSelectedIndex();

            SectionGrid selectedSection = (SectionGrid)arrSections[nSelectedIndex];
            cell.Value = selectedSection.GetTitle();
            cell.Tag = selectedSection.Tag;

            return 1;
        }

        private bool CheckEmergencyText(string strRegularTeamName, DataGridViewCell cell)
        {
            if (strRegularTeamName == "")
                return false;

            int nResult = CheckDuplication(strRegularTeamName, cell);

            if (nResult == -1)
            {
                MessageBox.Show("중복된 이름이 존재합니다.\r\n확인후 입력해 주세요");
                RollbackCellText(cell);
                return false;
            }
            else if (nResult == 1)
                return true;

            ArrayList arrSections = m_frmMain.FindSection(strRegularTeamName, 0);
            int nSectionCount = arrSections.Count;

            if (nSectionCount == 0)
            {
                MessageBox.Show(strRegularTeamName + " 이라는 이름의 Team은 존재하지 않습니다.\r\n상시조직도를 다시 확인하신 후 입력해 주세요");
                RollbackCellText(cell);
                return false;
            }

            if (nSectionCount > 1)
            {
                FormTeamList frm = new FormTeamList(arrSections);

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    int nSelectedIndex = frm.GetSelectedIndex();
                    SectionGrid selectedSection = (SectionGrid)arrSections[nSelectedIndex];

                    // selectedSection의 값을 cell에 입력할 것
                    cell.Tag = selectedSection.Tag;
                }
                else    // Cancel
                {
                    RollbackCellText(cell);
                    return false;
                }
            }

            SectionGrid section = (SectionGrid)arrSections[0];
            cell.Tag = section.Tag;

            return true;
        }

        private void dataGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = m_dataGrid.Rows[e.RowIndex].Cells[e.ColumnIndex];

            if (e.ColumnIndex == 0 && (m_nSectionType == 1 || m_nSectionType == 2))
            {
                if (cell.Value == null) return;
                if (!CheckEmergencyText(cell.Value.ToString(), cell))
                    return;
            }

            Type cellType = cell.GetType();

            if (cellType == typeof(DataGridViewTextBoxCellEx))
            {
                ((DataGridViewTextBoxCellEx)cell).PrevText = cell.Value.ToString();

                DataGridViewCheckBoxCell checkCell = new DataGridViewCheckBoxCell();
                checkCell.Value = false;
                m_dataGrid.Rows[e.RowIndex].Cells[1] = checkCell;
            }
            else if (cellType == typeof(DataGridViewTextBoxCell))
            {
                DataGridViewTextBoxCellEx newCell = new DataGridViewTextBoxCellEx();
                newCell.Value = cell.Value;
                newCell.Tag = cell.Tag;
                m_dataGrid.Rows[e.RowIndex].Cells[e.ColumnIndex] = newCell;
                newCell.PrevText = cell.Value.ToString();
            }

        }

        private void titleTextBox_Enter(object sender, EventArgs e)
        {
            SingleSelect(true);
        }

        private void titleTextBox_Leave(object sender, EventArgs e)
        {
            SingleSelect(false);
        }

        public void titleTextBox_DragDrop(object sender, EventArgs e)
        {
            m_frmMain.TeamName = m_titleTextBox.Text;
        }

        /*public void AddRowData(string strValue)
        {
            m_nCount++;
            DataRow row = null;
            row = m_table.NewRow();
            for (int i = 0; i < m_column.Count(); i++ )
            {
                row[i] = strValue;
            }            
            m_table.Rows.Add(row);
            
            //m_dataGrid.Size = new System.Drawing.Size(m_column.Count() * 75, m_nCount * 18);
        }*/

        /*private void AddDataSource()
        {
            m_nWidth = m_dataGrid.Size.Width + m_nDiff;
            m_nHeight = m_dataGrid.Size.Height + m_nDiff;
            x = m_dataGrid.Location.X - m_nDiff / 2;//2;
            y = m_dataGrid.Location.Y - m_nDiff / 2;//2;

            m_editBox.Position = new Point(x, y);
            m_editBox.RectSize = new Size(m_nWidth, m_nHeight);

            m_dataGrid.DataSource = m_table;
        }*/

        private void SetInitSize()
        {
            m_nWidth = m_dataGrid.Size.Width + m_nDiff;
            x = m_dataGrid.Location.X - m_nDiff / 2;//2;
            y = m_dataGrid.Location.Y - m_nDiff / 2;//2;

            if (m_readOnly)
            {
                m_dataGrid.AllowUserToAddRows = false;
                m_dataGrid.Visible = false;
                m_nHeight -= m_titleTextBox.Height;
            }

            m_editBox.Position = new Point(x, y);
            m_editBox.RectSize = new Size(m_nWidth, m_nHeight);
        }

        public SectionGrid GetParentSection()
        {
            return m_sectionParent;
        }

        public SectionGrid GetLastChild()
        {
            int nCount = m_arrChildSection.Count;
            return nCount == 0 ? null : (SectionGrid)m_arrChildSection[nCount - 1];
        }

        public ArrayList GetChildSections()
        {
            return m_arrChildSection;
        }

        public void AddChild(SectionGrid section)
        {
            if (!m_arrChildSection.Contains(section))
            {
                m_arrChildSection.Add(section);
                section.m_sectionParent = this;
            }
        }

        public void RemoveChild(SectionGrid section, bool arrRemove = true)
        {
            if (m_arrChildSection.Contains(section))
            {
                section.RemoveAllChild();

                if (arrRemove)
                {
                    m_arrChildSection.Remove(section);
                    m_frmMain.RemoveSectionIndex.Add(section);
                }
                    section.m_dataGrid.Visible = false;
                    section.m_titleTextBox.Visible = false;
                
                //section.Hide();

                //section.SetPrev(null);
                //section.SetNext(null);
            }
        }

        public void RemoveAllChild()
        {
            foreach (SectionGrid child in m_arrChildSection)
            {
                RemoveChild(child, false);
                m_frmMain.RemoveSectionIndex.Add(child);

            }

            m_arrChildSection.Clear();
        }

        //public void RemoveSection()
        //{
        //    RemoveAllChild();

        //    ArrayList arrSelected = m_arrSelectedSections[m_nSectionType];

        //    foreach (SectionGrid section in arrSelected)
        //    {
        //        arrSelected.Remove(section);
        //        section.m_dataGrid.Visible = false;
        //        section.m_titleTextBox.Visible = false;
        //    }
        //    m_arrSelectedSections[m_nSectionType].Clear();

        //}

        public void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Select(true);
                m_frmMain.ShowContextMenu(m_dataGrid, e.X, e.Y);
            }
            else if (e.Button == MouseButtons.Middle)
            {

                Select(true);
                m_frmMain.Refresh();

                IsDragDrop = true;
            }
            else
            {
                if (sender == m_dataGrid)
                    dataGrid_Enter(sender, null);
                else
                    titleTextBox_Enter(sender, null);
            }
//             else
//             {
//                 DragEventArgs drag = GetDragEventArgs(e.X, e.Y);
//                 m_frmMain.splitContainer_Panel1_DragEnter(sender, drag);
//             }
        }
        
        private DragEventArgs GetDragEventArgs(int X, int Y)
        {
            DataObject data = new DataObject();
            DragEventArgs drag = new DragEventArgs(data, 1, X, Y, DragDropEffects.None, DragDropEffects.None);

            return drag;
        }

        // (x, y)좌표는 화면의 왼쪽 끝에서부터 시작한 값이다.
        // 화면에 Panel이 여러개 동시에 보일 경우 이 좌표가 어느 Panel 위에 속하게 되는지를 알려준다.
        // 
        // Return 값 : -1(Panel 외부), 0(상시 조직), 1(평일 비상 조직), 2(휴일 비상 조직)
        private int GetMousePosInPanel(int x, int y, ArrayList arrCurrentViews, out int nPrevWidth)
        {
            nPrevWidth = 0;

            if (x < 0 || y < 0)
                return -1;

            SplitterPanel[] arrPanel = new SplitterPanel[3] {m_frmMain.GetVeiw1(), m_frmMain.GetVeiw2(), m_frmMain.GetVeiw3()};

            if (y >= arrPanel[0].Height)
                return -1;
            
            int nViewCount = arrCurrentViews.Count;

            for (int i = 0; i < nViewCount; i++)
            {
                int nViewIndex = (int)arrCurrentViews[i];

                if (x < nPrevWidth + arrPanel[nViewIndex].Width)
                    return nViewIndex;

                nPrevWidth += arrPanel[nViewIndex].Width;
            }

            return -1;
        }

        public void OnMouseUp(object sender, MouseEventArgs e)
        {
            ArrayList arrSelected = m_arrSelectedSections[m_nSectionType];
            int nSectionID = 0;
            foreach (SectionGrid section in arrSelected)
            {
                if (section != null)
                    nSectionID = section.Tag;
            }

            if (e.Button == MouseButtons.Left)
            {
                m_frmMain.GetTeamProperties().SetGridData(m_titleTextBox.Text, m_dataGrid, nSectionID);
            }
            else if (e.Button == MouseButtons.Middle)
            {
                if (!m_frmMain.EditMode) return;

                ArrayList arrCurrentViews = m_frmMain.CurrentView;
                int nViewCount = arrCurrentViews.Count;
                if (nViewCount <= 1) return;

                SplitterPanel panel1 = m_frmMain.GetVeiw1();
                SplitterPanel panel2 = m_frmMain.GetVeiw2();
                SplitterPanel panel3 = m_frmMain.GetVeiw3();

                int left = panel1.Left;
                int right = panel1.Right;
                int area = panel1.Width * panel1.Height;

                int width2 = panel2.Width;
                int left2 = panel2.Left;
                int right2 = panel2.Right;
                int area2 = panel2.Width * panel2.Height;

                int width3 = panel3.Width;
                int left3 = panel3.Left;
                int right3 = panel3.Right;
                int area3 = panel3.Width * panel3.Height;

                if (panel1.Width > e.X)
                {
                    m_frmMain.FoucsPanel(1);
                    m_nSectionType = 0;
                }

                int _left, _top, _right, _bottom;
                GetBoundary(out _left, out _top, out _right, out _bottom);

                int _X = m_titleTextBox.Left + e.X;
                int _Y = 0;

                if (sender.GetType() == typeof(DataGridView))
                    _Y = m_dataGrid.Top + e.Y;
                else
                    _Y = m_titleTextBox.Top + e.Y;

                int nPrevPanelWidth;
                int nPanelIndex = GetMousePosInPanel(_X, _Y, arrCurrentViews, out nPrevPanelWidth);
                if (nPanelIndex <= 0) return;

                int nX = _X - nPrevPanelWidth;
                int nY = _Y;

                bool isSelect = m_frmMain.SelectSection(nPanelIndex, nX, nY, m_titleTextBox.Text, nSectionID);

                if (isSelect)
                {
                    this.Select(false);
                    m_frmMain.AutoAlign(true, m_frmMain.GetSections(nPanelIndex));
                    //m_frmMain.Refresh();
                }

                /*if (panel1.Width < _X && panel1.Width + panel2.Width > _X)
                {
                    int nX = _X - panel1.Width;
                    int nY = _Y;
                    bool isSelect = m_frmMain.SelectSection(m_frmMain.TeamMode, nX, nY, m_titleTextBox.Text, nSectionID);
                    if (isSelect)
                    {
                        this.Select(false);
                        m_frmMain.Refresh();
                    }
                }*/
            }
        }
        
        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (IsDragDrop)
            {
                Select(e.X, e.Y);



                //SendMessage(m_frmMain.GetProcess().Handle, 0x0207, (IntPtr)0, (IntPtr)0);
            }
        }

        public SectionGrid Select(int x, int y)
        {
            int nLeft, nRight, nTop, nBottom;
            GetBoundary(out nLeft, out nTop, out nRight, out nBottom);
            if (x >= nLeft && x <= nRight && y >= nTop && y <= nBottom)
            {
                if (m_multiSelect)
                {
                    if (!m_arrSelectedSections[m_nSectionType].Contains(this))
                    {
                        m_arrSelectedSections[m_nSectionType].Add(this);
                    }
                }
                else
                {
                    m_arrSelectedSections[m_nSectionType].Clear();
                    m_arrSelectedSections[m_nSectionType].Add(this);
                }

                return this;
            }

            foreach (SectionGrid child in m_arrChildSection)
            {
                SectionGrid section = child.Select(x, y);
                if (section != null)
                    return section;
            }

            return null;
        }

        public void Select(bool isSelect, bool alsoChild = false)
        {
            if (isSelect)
            {
//                 if (m_arrSelectedSections[m_nSectionType].Contains(this))
//                     m_arrSelectedSections[m_nSectionType].Remove(this);

                if (m_multiSelect)
                {
                    if (!m_arrSelectedSections[m_nSectionType].Contains(this))
                    {
                        m_arrSelectedSections[m_nSectionType].Add(this);
                    }
                }
                else
                {
                    m_arrSelectedSections[m_nSectionType].Clear();
                    m_arrSelectedSections[m_nSectionType].Add(this);
                }
            }
            else
            {
                if (m_arrSelectedSections[m_nSectionType].Contains(this))
                    m_arrSelectedSections[m_nSectionType].Remove(this);

                m_dataGrid.ClearSelection();

                m_dataGrid.ReadOnly = true;
                m_titleTextBox.ReadOnly = true;
            }

            if (alsoChild)
            {
                foreach (SectionGrid child in m_arrChildSection)
                {
                    child.Select(isSelect, true);
                }
            }
        }

        //[DllImport("gdi32")]
        //public static extern int RoundRect(int hdc, int x1, int y1, int x2, int y2, int x3, int y3);

        protected virtual void DrawRectangle(Graphics g, int xLeft, int yTop, int xRight, int yBottom)
        {
            g.DrawRectangle(BOUNDARY_PEN, xLeft, yTop, xRight - xLeft, yBottom - yTop);
            //RoundRect((int)g.GetHdc(), xLeft, yTop, xRight, yBottom, 5, 5);
            //g.ReleaseHdc();
        }

        private void GetBoundary(out int left, out int top, out int right, out int bottom)
        {
            left = m_titleTextBox.Left - m_nDiff / 2;
            top = m_titleTextBox.Top - m_nDiff / 2;
            right = m_titleTextBox.Right + m_nDiff / 2;

            if (m_dataGrid.Visible && m_dataGrid.Rows.Count > 0)
                bottom = m_dataGrid.Bottom + m_nDiff / 2;
            else
                bottom = m_titleTextBox.Bottom + m_nDiff / 2;
        }

        public void DrawSection(Graphics g)
        {
            int left, top, right, bottom;
            GetBoundary(out left, out top, out right, out bottom);

            //m_editBox.Position = new Point(x + m_nDiff, y + m_nDiff);
            m_editBox.Position = new Point(left, top);

            DrawRectangle(g, left, top, right, bottom);
            //DrawRectangle(g, x, y, x + m_nWidth, y + m_nHeight);
            DrawSectionLink(g);

            if (m_arrSelectedSections[m_nSectionType].Contains(this))
                m_editBox.Draw(g);

            foreach (SectionGrid child in m_arrChildSection)
            {
                child.DrawSection(g);
            }
        }

        private void DrawSectionLink(Graphics g)
        {
            int left, top, right, bottom;
            GetBoundary(out left, out top, out right, out bottom);

            // Link To Parent
            if (m_sectionParent != null)
            {
                int parentLeft, parentTop, parentRight, parentBottom;
                m_sectionParent.GetBoundary(out parentLeft, out parentTop, out parentRight, out parentBottom);

                //int x1 = x + m_nWidth / 2;
                int x1 = (left + right) / 2;
                int y2;

                if (m_sectionParent.m_arrChildSection.Count > 1)
                {
                    //int nSpace = y - (m_sectionParent.y + m_sectionParent.m_nHeight);
                    int nSpace = top - parentBottom;

                    //y2 = y - nSpace / 2;
                    y2 = top - nSpace / 2 - 1;
                }
                else
                {
                    //y2 = m_sectionParent.y + m_sectionParent.m_nHeight;
                    y2 = parentBottom;
                }

                g.DrawLine(LINK_PEN, x1, top, x1, y2);
            }

            // Link To Child
            if (m_arrChildSection == null)
                return;

            int nChildCount = m_arrChildSection.Count;
            if (nChildCount <= 1)
                return;

            SectionGrid firstChild = (SectionGrid)m_arrChildSection[0];

            int firstLeft, firstTop, firstRight, firstBottom;
            firstChild.GetBoundary(out firstLeft, out firstTop, out firstRight, out firstBottom);

            //int _x = x + m_nWidth / 2;
            int _x = (left + right) / 2;
            //int nBeginY = y + m_nHeight;
            int nBeginY = bottom;
            //int nChildSpace = firstChild.y - nBeginY;
            int nChildSpace = firstTop - nBeginY;
            int nEndY = nBeginY + nChildSpace / 2;

            g.DrawLine(LINK_PEN, _x, nBeginY, _x, nEndY);

            //if (nChildCount > 1)
            {
                SectionGrid lastChild = (SectionGrid)m_arrChildSection[nChildCount - 1];

                int lastLeft, lastTop, lastRight, lastBottom;
                lastChild.GetBoundary(out lastLeft, out lastTop, out lastRight, out lastBottom);

                //int nBeginX = m_nChildBegin + firstChild.m_nWidth / 2;
                int nBeginX = (firstLeft + firstRight) / 2;
                //int nEndX = m_nChildEnd - lastChild.m_nWidth / 2;
                int nEndX = (lastLeft + lastRight) / 2;
                g.DrawLine(LINK_PEN, nBeginX, nEndY, nEndX, nEndY);
            }
        }

        public void Remove()
        {
            if (m_frmParent == null)
                return;

            foreach (SectionGrid section in m_arrChildSection)
            {
                section.Remove();
            }

            m_frmParent.Controls.Remove(m_titleTextBox);
            m_frmParent.Controls.Remove(m_dataGrid);
        }

        public static string GetFullPath(SectionGrid section)
        {
            string strPath = section.GetTitle();
            SectionGrid parent = section.GetParentSection();

            while (parent != null)
            {
                strPath = parent.GetTitle() + "\\" + strPath;
                parent = parent.GetParentSection();
            }

            return strPath;
        }

        public virtual Point Position
        {
            get
            {
                return new Point(x, y);
            }
            set
            {
                if (x != value.X || y != value.Y)
                {
                    x = value.X;
                    y = value.Y;

                    m_editBox.Position = value;

                    m_titleTextBox.Left = x + m_nDiff / 2;
                    m_titleTextBox.Top = y + m_nDiff / 2;

                    m_dataGrid.Left = x + m_nDiff / 2;
                    m_dataGrid.Top = m_titleTextBox.Bottom;

                    m_nDiffTextX = 3;
                    m_nDiffTextY = 3;
                }
            }
        }

        public virtual Size Size
        {
            get
            {
                return new Size(m_nWidth, m_nHeight);
            }
            set
            {
                m_nWidth = value.Width;
                m_nHeight = value.Height;

                m_editBox.Position = new Point(x, y);
                m_editBox.RectSize = new Size(m_nWidth, m_nHeight);
            }
        }

        public int ChildBegin
        {
            get { return m_nChildBegin; }
            set
            {
                m_nChildBegin = value;

                if (m_sectionParent != null)
                {
                    if (m_sectionParent.m_nChildBegin > m_nChildBegin)
                        m_sectionParent.ChildBegin = m_nChildBegin;
                }

                int _x = (m_nChildBegin + m_nChildEnd) / 2 - m_nWidth / 2;
                this.Position = new Point(_x, y);
            }
        }

        public int ChildEnd
        {
            get { return m_nChildEnd; }
            set
            {
                m_nChildEnd = value;

                if (m_sectionParent != null)
                {
                    if (m_sectionParent.m_nChildEnd < m_nChildEnd)
                        m_sectionParent.ChildEnd = m_nChildEnd;
                }

                int _x = (m_nChildBegin + m_nChildEnd) / 2 - m_nWidth / 2;
                this.Position = new Point(_x, y);
            }
        }

        // 수평 간격
        public static int HorzSpace
        {
            get { return m_nHorzSpace; }
        }

        // 수직 간격
        public static int VertSpace
        {
            get { return m_nVertSpace; }
        }

        public static bool MultiSelect
        {
            get { return m_multiSelect; }
            set { m_multiSelect = value; }
        }

        public int SectionType
        {
            get { return m_nSectionType; }
        }
    }

    public class Vertex2D
    {
        public double x, y;

        public Vertex2D()
        {
            x = y = 0.0;
        }

        public Vertex2D(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class DataGridViewTextBoxCellEx : DataGridViewTextBoxCell
    {
        private string m_strPrev = "";
        private string m_strGroupName = "";
        
        public string PrevText
        {
            get { return m_strPrev; }
            set { m_strPrev = value; }
        }

        public string GroupName
        {
            get { return m_strGroupName; }
            set { m_strGroupName = value; }
        }

    }

}
