using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoilMan.Popup
{
    public partial class FormDetailAttrib : Form
    {
        private class LongInDouble : IComparable
        {
            private double m_dData = 0.0;

            public double Data
            {
                get { return m_dData; }
                set { m_dData = value; }
            }

            public LongInDouble()
            {
            }

            public LongInDouble(double data)
            {
                m_dData = data;
            }

            public override string ToString()
            {
                return m_dData < 0.0 ? "" : string.Format("{0:###,###,###,###,###,###}", (long)m_dData);
            }

            public int CompareTo(object obj)
            {
                LongInDouble data1 = this;
                LongInDouble data2 = (LongInDouble)obj;

                if (data1.Data < 0 && data2.Data < 0)
                    return 0;
                else if (data1.Data < 0)
                    return -1;
                else if (data2.Data < 0)
                    return 1;

                if (data1.Data == data2.Data)
                    return 0;

                if (data1.Data < data2.Data)
                    return -1;

                return 1;
            }
        }

        private libShapeFile.ShapeInfo m_shapeInfo = null;
        private SelectionManager m_selectionMgr = null;
        //private DataGridViewRow m_selectedRow = null;

        private DataGridViewSelectedRowCollection selectedRows = null;
        private int m_nMaxGridRowCount = 1000;
        // Grid에 전체 Row를 모두 보여주지 못할 만큼 데이터가 많을 경우
        // 전체 Row는 m_rows에 넣어놓고 Grid에는 한번에 m_nMaxGridRowCount개씩의 Row만 보여준다.
        //private List<DataGridViewRow> m_rows = new List<DataGridViewRow>();
        private List<Drawing.Polygon> m_polygons = new List<Drawing.Polygon>();

        private int m_nCurrentPageIndex = -1;
        private int NO_Index = -1, LandID_Index = -1, Area_Index = -1, Addr_Index = -1, Cost_Index = -1;

        public SelectionManager SelectionManager
        {
            get { return m_selectionMgr; }
            set { m_selectionMgr = value; }
        }

        //public Drawing.Polygon SelectedShape
        //{
        //    get { return m_selectedRow == null ? null : (Drawing.Polygon)m_selectedRow.Tag; }
        //}

        public List<Drawing.Polygon> SelectedShapes
        {
            get
            {
                selectedRows = dataGridView1.SelectedRows;
                if (selectedRows == null || selectedRows.Count == 0)
                    return null;
                
                List<Drawing.Polygon> tempList = new List<Drawing.Polygon>();
                foreach(DataGridViewRow row in selectedRows)
                {
                    tempList.Add((Drawing.Polygon)row.Tag);
                }
                return tempList;
            }
        }
        bool designMode = (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime);
        public FormDetailAttrib()
        {
            InitializeComponent();

            mInputBox.KeyDown += new KeyEventHandler(OnKeyDownHandler);
            ((DataGridViewEx)dataGridView1).OnPressEnter += FormDetailAttrib_OnPressEnter;
        }
        public void Clear()
        {

            dataGridView1.ClearSelection();
            dataGridView1.Rows.Clear();
            panel2.Visible = false;
        }

        private List<Drawing.Polygon> deleteList = new List<Drawing.Polygon>();
        private void SetGrid(Drawing.ShapeLayer layer)
        {
            if (m_shapeInfo == null)
                return;

            int nFieldCount = m_shapeInfo.GetFieldCount();

            if (nFieldCount == 0)
                return;

            dataGridView1.ClearSelection();
            dataGridView1.Rows.Clear();

            for (int i=0;i<nFieldCount;i++)
            {
                string strFieldName = m_shapeInfo.GetFieldName(i);

                if (strFieldName == "NO")
                    NO_Index = i;
                else if (strFieldName == "지목")
                    LandID_Index = i;
                else if (strFieldName == "면적")
                    Area_Index = i;
                else if (strFieldName == "주소")
                    Addr_Index = i;
                else if (strFieldName == "공시지가")
                    Cost_Index = i;
            }

            int nRowCount = 0;

            foreach (DXFViewer.Shape shape in layer.Shapes)
            {
                if (shape is Drawing.Polygon)
                {
                    SetPolygonInfo((Drawing.Polygon)shape, nFieldCount, m_shapeInfo);
                    AddRow((Drawing.Polygon)shape, nFieldCount, ref nRowCount);
                }
                else if (shape is Drawing.PolygonList)
                {

                    Drawing.PolygonList polygonList = (Drawing.PolygonList)shape;                    
                    
                    List<Drawing.Polygon> polyList = polygonList.GetPolygons(null);
                    FormMain.Instance.DXFControl_BeginRead("Set Grid Data", "GRD", polyList.Count);
                    int nCount = 1;

                    foreach (Drawing.Polygon polygon in polyList)
                    {
                        if (polygon != null)
                        {
                            SetPolygonInfo(polygon, nFieldCount, m_shapeInfo);
                        }
                    }

                    // 주소를 기준으로 정렬한다.
                    polyList.Sort();

                    foreach (Drawing.Polygon polygon in polyList)
                    {
                        if (polygon != null)
                        {
                            AddRow(polygon, nFieldCount, ref nRowCount);
                            FormMain.Instance.DXFControl_ReadEntity("Polygon", nCount++);
                        }
                    }

                    /*foreach (Drawing.Polygon polygon in polyList)
                    {
                        if (polygon != null)
                        {
                            SetPolygonInfo(polygon, nFieldCount, m_shapeInfo);

                            PolygonInfo info = (PolygonInfo)polygon.Tag;

                            //////////// Test Code //////////////////
                            //if (!info.Address.Contains("경기도 양주시 은현면"))
                            //{
                            //     deleteList.Add(polygon);
                            //    continue;
                            //}/
                            //////////// Test Code //////////////////


                            AddRow(polygon, nFieldCount, ref nRowCount);
                            FormMain.Instance.DXFControl_ReadEntity("Polygon", nCount++);
                        }
                    }*/
                    FormMain.Instance.DXFControl_EndRead("Set Grid Data", "GRD");
                }

                /*if (!(shape is Drawing.Polygon))
                    continue;

                Drawing.Polygon polygon = (Drawing.Polygon)shape;

                int nShapeID = polygon.ID;

                if (nShapeID < 0)
                    continue;

                string strNo = null, strLandID = null, strArea = null, strAddr = null, strCost = null;

                strNo = GetShapeInfoString(nShapeID, NO_Index, 0, nFieldCount);
                strLandID = GetShapeInfoString(nShapeID, LandID_Index, 1, nFieldCount);
                strArea = GetShapeInfoString(nShapeID, Area_Index, 2, nFieldCount);
                strAddr = GetShapeInfoString(nShapeID, Addr_Index, 3, nFieldCount);
                strCost = GetShapeInfoString(nShapeID, Cost_Index, 4, nFieldCount);
                

                DataGridViewRow row = new DataGridViewRow();

                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = strNo == null ? "" : strNo;
                row.Cells.Add(cell);
                cell.ReadOnly = true;
                cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                cell = new DataGridViewTextBoxCell();
                cell.Value = strLandID == null ? "" : strLandID;
                row.Cells.Add(cell);
                cell.ReadOnly = true;
                cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                cell = new DataGridViewTextBoxCell();
                cell.Value = strArea == null ? "" : strArea;
                row.Cells.Add(cell);
                cell.ReadOnly = true;
                cell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;

                dataGridView1.Rows.Add(row);
                row.Tag = shape;

                nRowCount++;

                if (nRowCount >= 1000)
                    break;*/
            }

            int nTotalRowCount = m_polygons.Count;

            if (nTotalRowCount <= m_nMaxGridRowCount)
            {
                //dataGridView1.Dock = DockStyle.Fill;
                panel2.Visible = false;
                //labelTotalPageCount.Visible = false;
                //textBoxPageIndex.Visible = false;
                //btnPrev.Visible = btnNext.Visible = btnMove.Visible = false;
            }
            else
            {
                int nPageCount = nTotalRowCount / m_nMaxGridRowCount;

                if (nPageCount * m_nMaxGridRowCount < nTotalRowCount)
                    nPageCount++;

                panel2.Visible = true;
                dataGridView1.BringToFront();
                labelTotalPageCount.Text = "/ " + nPageCount.ToString();
                btnMove.Location = new Point(labelTotalPageCount.Location.X + labelTotalPageCount.Size.Width + 6, btnMove.Location.Y);
                btnNext.Location = new Point(btnMove.Location.X, btnNext.Location.Y);

                btnPrev.Enabled = false;
            }

            m_nCurrentPageIndex = 1;
            textBoxPageIndex.Text = m_nCurrentPageIndex.ToString();
        }

        public void SetPolygonInfo(Drawing.Polygon polygon, int nFieldCount, libShapeFile.ShapeInfo shapeInfo, Dictionary<string, Data.ShapeAttrib> shapeAttribs = null)
        {
            PolygonInfo info = null;// new PolygonInfo();

            if (polygon.Tag == null)
            {
                info = new PolygonInfo();
                polygon.Tag = info;
            }
            else
                info = (PolygonInfo)polygon.Tag;

            int nShapeID = polygon.ID;

            if (nShapeID < 0)
                return;

            string strNo = null, strLandID = null;//, strArea = null, strAddr = null, strCost = null;

            strNo = GetShapeInfoString(nShapeID, NO_Index, 0, nFieldCount, shapeInfo);
            strLandID = GetShapeInfoString(nShapeID, LandID_Index, 1, nFieldCount, shapeInfo);
            /*strArea = GetShapeInfoString(nShapeID, Area_Index, 2, nFieldCount);
            strAddr = GetShapeInfoString(nShapeID, Addr_Index, 3, nFieldCount);
            strCost = GetShapeInfoString(nShapeID, Cost_Index, 4, nFieldCount);*/

            if (strNo != null)
                info.Code = strNo;

            if (strLandID != null)
            {
                info.Jibun = strLandID;

                if (info.Jibun.EndsWith("임"))
                    info.Land = LandType.Mountain;
                else if (info.Jibun.EndsWith("답"))
                    info.Land = LandType.RiceField;
                else if (info.Jibun.EndsWith("전"))
                    info.Land = LandType.Field;
                else
                    info.Land = LandType.General;
            }

            if (nShapeID >= 0)
            {
                if (info.Area < 0.0 && Area_Index >= 0)
                {
                    string strArea = shapeInfo.GetFieldData(nShapeID, Area_Index);

                    double dArea;

                    if (double.TryParse(strArea, out dArea))
                        info.Area = dArea;
                }

                if (info.Address.Length == 0 && Addr_Index >= 0)
                {
                    string strAddr = shapeInfo.GetFieldData(nShapeID, Addr_Index);
                    info.Address = strAddr;
                }

                if (info.Cost < 0.0 && Cost_Index >= 0)
                {
                    string strCost = shapeInfo.GetFieldData(nShapeID, Cost_Index);

                    double dCost;

                    if (double.TryParse(strCost, out dCost))
                        info.Cost = dCost;
                }
            }

            if (info.Area < 0.0)
            {
                if (shapeAttribs != null)
                {
                    Data.ShapeAttrib attr;
                    if (shapeAttribs.TryGetValue(info.Code, out attr))
                    {
                        info.Area = attr.Area;
                    }
                }

                if (info.Area < 0.0)
                {
                    float fArea = polygon.GetArea();
                    info.Area = fArea;
                    /*int nSubPolygonCount = polygon.GetSubPolygonCount();
                    double dArea = 0.0;

                    for (int i = 0; i < nSubPolygonCount; i++)
                    {
                        UnE.Geometry.PolygonF subPolygon = polygon.GetSubPolygon(i);
                        dArea += subPolygon.GetArea();
                    }

                    info.Area = dArea;*/
                }
            }

            if (info.Address.Length == 0)
            {
                if (info.Code.Length > 0)
                    SetCodeAddress(info.Code, info);
            }

            if (info.Cost < 0.0)
            {
                if (shapeAttribs != null)
                {
                    Data.ShapeAttrib attr;
                    if (shapeAttribs.TryGetValue(info.Code, out attr))
                    {
                        info.Cost = attr.Cost;
                    }
                }

                if (info.Code.Length > 0)
                    SetPublicCost(info.Code, info);
            }
        }

        private void SetCodeAddress(string strCode, PolygonInfo info)
        {
            if (strCode.Length != 19)
                return;

            // 법정동코드 데이터를 완전히 읽을때까지 최대 5초동안 대기한다.
            if (FormMain.Instance.CodeAddress == null)
            {
                for (int i = 0; i < 5; i++)
                {
                    System.Threading.Thread.Sleep(1000);

                    if (FormMain.Instance.CodeAddress != null)
                        break;
                }

                if (FormMain.Instance.CodeAddress == null)
                    return;
            }

            string strAddr;
            string strDongCode = strCode.Substring(0, 10);

            if (FormMain.Instance.CodeAddress.TryGetValue(strDongCode, out strAddr))
            {
                char ch = strCode.ElementAt(10);

                if (ch == '2')
                    strAddr += " 산 ";
                else if (ch == '1')
                    strAddr += " ";
                else
                    return;

                string strMajorCode = strCode.Substring(11, 4);
                string strMinorCode = strCode.Substring(15, 4);
                info.Address = strAddr + GetSubAddress(strMajorCode, strMinorCode);
            }
        }

        private void SetPublicCost(string strCode, PolygonInfo info)
        {
            if (strCode.Length != 19)
                return;

            // 공시지가 데이터를 완전히 읽을때까지 최대 5초동안 대기한다.
            if (FormMain.Instance.CodeCost == null)
            {
                for (int i = 0; i < 5; i++)
                {
                    System.Threading.Thread.Sleep(1000);

                    if (FormMain.Instance.CodeCost != null)
                        break;
                }

                if (FormMain.Instance.CodeCost == null)
                    return;
            }

            double dCost;

            if (FormMain.Instance.CodeCost.TryGetValue(strCode, out dCost))
            {
                info.Cost = dCost;
            }
        }

        private string GetSubAddress(string strMajorCode, string strMinorCode)
        {
            int nMajor, nMinor;

            if (!int.TryParse(strMajorCode, out nMajor))
                return "";

            if (!int.TryParse(strMinorCode, out nMinor))
                return "";

            if (nMinor == 0)
                return nMajor.ToString();

            return nMajor.ToString() + "-" + nMinor.ToString();
        }

        private void AddRow(Drawing.Polygon polygon, int nFieldCount)
        {
            PolygonInfo info = (PolygonInfo)polygon.Tag;

            if (info == null)
                return;

            /*int nShapeID = polygon.ID;

            if (nShapeID < 0)
                return;

            string strNo = null, strLandID = null, strArea = null, strAddr = null, strCost = null;

            strNo = GetShapeInfoString(nShapeID, NO_Index, 0, nFieldCount);
            strLandID = GetShapeInfoString(nShapeID, LandID_Index, 1, nFieldCount);
            strArea = GetShapeInfoString(nShapeID, Area_Index, 2, nFieldCount);
            strAddr = GetShapeInfoString(nShapeID, Addr_Index, 3, nFieldCount);
            strCost = GetShapeInfoString(nShapeID, Cost_Index, 4, nFieldCount);*/

            DataGridViewRow row = new DataGridViewRow();

            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            cell.Value = info.Code;//strNo == null ? "" : strNo;
            cell.ToolTipText = info.Code;//strNo == null ? "" : strNo;
            row.Cells.Add(cell);
            cell.ReadOnly = true;
            cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            cell = new DataGridViewTextBoxCell();

            string szJimok = "";
            if (info.Land == LandType.Mountain)
                szJimok = "임";
            else if ( info.Land == LandType.RiceField)
                szJimok = "답";
            else if (info.Land == LandType.Field)
                szJimok = "전";
            else
                szJimok = "일반";

            cell.Value = szJimok;// strLandID == null ? "" : strLandID;
            cell.ToolTipText = szJimok;
            row.Cells.Add(cell);
            cell.ReadOnly = true;
            cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            cell = new DataGridViewTextBoxCell();
            cell.Value = new LongInDouble(info.Area);
            //cell.Value = info.Area < 0.0 ? "" : string.Format("{0:###,###,###,###,###,###}", (long)info.Area);// strArea == null ? "" : strArea;
            cell.ToolTipText = cell.Value.ToString();
            row.Cells.Add(cell);
            cell.ReadOnly = true;
            cell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;

            cell = new DataGridViewTextBoxCell();
            cell.Value = info.Address;
            cell.ToolTipText = (string)cell.Value;
            row.Cells.Add(cell);
            cell.ReadOnly = true;
            cell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;

            cell = new DataGridViewTextBoxCell();
            cell.Value = new LongInDouble(info.Cost);
            //cell.Value = info.Cost < 0.0 ? "" : string.Format("{0:###,###,###,###,###,###}", (long)info.Cost);
            cell.ToolTipText = cell.Value.ToString();
            row.Cells.Add(cell);
            cell.ReadOnly = true;
            cell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;

            row.Tag = polygon;           
            dataGridView1.Rows.Add(row);
            
        }

        private void AddRow(Drawing.Polygon polygon, int nFieldCount, ref int nRowCount)
        {
            if (nRowCount++ < m_nMaxGridRowCount)
            {
                AddRow(polygon, nFieldCount);
            }

            m_polygons.Add(polygon);
        }

        private string GetShapeInfoString(int nShapeID, int nColumnIndex, int nDefIndex, int nFieldCount, libShapeFile.ShapeInfo shapeInfo)
        {
            if (nColumnIndex >= 0)
                return shapeInfo.GetFieldData(nShapeID, nColumnIndex);

            if (nDefIndex < nFieldCount)
                return shapeInfo.GetFieldData(nShapeID, nDefIndex);

            return null;
        }

        public void Select(Drawing.Polygon shape)
        {
            if (shape != null)
            {
                int nPageIndex;
                DataGridViewRow row = FindRow(shape, out nPageIndex);

                if (row != null)
                {
                    int nTotalPageCount = GetTotalPageCount();

                    btnPrev.Enabled = nPageIndex > 1;
                    btnNext.Enabled = nPageIndex < nTotalPageCount;

                    MovePage(nPageIndex);
                    SelectRow(row);
                }
            }
        }

        public void Unselect()
        {
            //dataGridView1.ClearSelection();
        }

        private void SelectRow(DataGridViewRow row)
        {
            dataGridView1.CurrentCell = row.Cells[0];
            row.Selected = true;
        }

        private DataGridViewRow FindRow(Drawing.Polygon shape, out int nPageIndex)
        {
            nPageIndex = -1;

            if (shape.ID < 0)
                return null;

            int nTotalRowCount = m_polygons.Count;

            for (int i = 0; i < nTotalRowCount;i++ )
            {
                Drawing.Polygon polygon = m_polygons[i];

                if (shape == polygon)
                {
                    nPageIndex = i / m_nMaxGridRowCount + 1;
                    MovePage(nPageIndex);

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.Tag != null && (Drawing.Polygon)row.Tag == polygon)
                            return row;
                    }

                    break;

                    //return dataGridView1.Rows[i % m_nMaxGridRowCount];
                }
            }

            /*if (dataGridView1.Rows.Count > shape.ID)
            {
                object obj = dataGridView1.Rows[shape.ID].Tag;

                if (shape == obj)
                    return dataGridView1.Rows[shape.ID];
            }

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                object obj = row.Tag;

                if (shape == obj)
                    return dataGridView1.Rows[shape.ID];
            }*/

            return null;
        }

        private void FormDetailAttrib_Load(object sender, EventArgs e)
        {
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridView1.ColumnHeadersHeight = 25;

            dataGridView1.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(220, 230, 242);
            dataGridView1.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;

            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.HeaderCell.Style.BackColor = System.Drawing.Color.FromArgb(235, 241, 222);
            }
        }

        public void SetShapeInfo(libShapeFile.ShapeInfo shapeInfo, Drawing.ShapeLayer layer, bool always = false)
        {
            if (m_shapeInfo != shapeInfo || always)
            {
                dataGridView1.Rows.Clear();
                m_shapeInfo = shapeInfo;

                if (m_shapeInfo != null)
                    SetGrid(layer);
            }
        }

        private TextBox mInputBox = new TextBox();
        private DataGridViewCell mCurrentEditCell = null;
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (bProcessAddRow == true)
                return;


            if( e.ColumnIndex == 4)
            {
                CheckInputBox();

                mCurrentEditCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                Rectangle rect = dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                mInputBox.SetBounds(rect.X, rect.Y, rect.Width, rect.Height);
                mInputBox.AcceptsReturn = true;

                mInputBox.Text = mCurrentEditCell.Value.ToString();
                
                dataGridView1.Controls.Add(mInputBox);
                mInputBox.Visible = true;
                mInputBox.Focus();
            }
            else
            {
                CheckInputBox();
                mCurrentEditCell = null;
                mInputBox.Visible = false;
            }
                
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                mInputBox.Visible = false;
                mCurrentEditCell = null;
            }
        }

       

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F13)
            {
                e.Handled = true;
            }
        }

        private int GetPageIndex()
        {
            if (textBoxPageIndex.Text.Length == 0)
                return -1;

            int nPageIndex;

            if (!int.TryParse(textBoxPageIndex.Text, out nPageIndex))
                return -1;

            if (nPageIndex > GetTotalPageCount() || nPageIndex <= 0)
                return -1;

            return nPageIndex;
        }

        private int GetTotalPageCount()
        {
            int nTotalRowCount = m_polygons.Count;
            int nPageCount = nTotalRowCount / m_nMaxGridRowCount;

            if (nPageCount * m_nMaxGridRowCount < nTotalRowCount)
                nPageCount++;

            return nPageCount;
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            bProcessRevers = true;
            int nPageIndex = GetPageIndex();

            if (nPageIndex <= 1)
                return;

            btnNext.Enabled = true;

            if (nPageIndex == 2)
                btnPrev.Enabled = false;

            MovePage(nPageIndex - 1);

            if (dataGridView1.SelectedRows != null && dataGridView1.SelectedRows.Count > 0)
            {
                dataGridView1.ClearSelection();
            }
            bProcessRevers = false;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            bProcessRevers = true;
            int nPageIndex = GetPageIndex();
            int nTotalPageCount = GetTotalPageCount();

            if (nPageIndex >= nTotalPageCount)
                return;

            btnPrev.Enabled = true;

            if (nPageIndex == nTotalPageCount - 1)
                btnMove.Enabled = false;

            MovePage(nPageIndex + 1);
            if (dataGridView1.SelectedRows != null && dataGridView1.SelectedRows.Count > 0)
            {                
                dataGridView1.ClearSelection();
            }
            bProcessRevers = false;
        }

        private void btnMove_Click(object sender, EventArgs e)
        {
            CheckInputBox();

            bProcessRevers = true;
            int nPageIndex = GetPageIndex();
            int nTotalPageCount = GetTotalPageCount();

            btnPrev.Enabled = nPageIndex > 1;
            btnNext.Enabled = nPageIndex < nTotalPageCount;

            MovePage(nPageIndex);
            if (dataGridView1.SelectedRows != null && dataGridView1.SelectedRows.Count > 0)
                dataGridView1.ClearSelection();
            bProcessRevers = false;


        }

        private void MovePage(int nPageIndex)
        {
            if (m_nCurrentPageIndex == nPageIndex)
                return;

           
            dataGridView1.Rows.Clear();

            int nTotalRowCount = m_polygons.Count;
            int nBeginIndex = (nPageIndex - 1) * m_nMaxGridRowCount;
            int nEndIndex = nBeginIndex + m_nMaxGridRowCount;

            if (nEndIndex > nTotalRowCount)
                nEndIndex = nTotalRowCount;

            int nFieldCount = m_shapeInfo.GetFieldCount();

            if (nFieldCount > 0)
            {
                for (int i = nBeginIndex; i < nEndIndex; i++)
                {
                    bProcessAddRow = true;
                    AddRow(m_polygons[i], nFieldCount);
                    bProcessAddRow = false;
                }
            }

            m_nCurrentPageIndex = nPageIndex;
            textBoxPageIndex.Text = m_nCurrentPageIndex.ToString();
           
        }

        private void btnDeleteSel_Click(object sender, EventArgs e)
        {
            //List<Drawing.Polygon> polygons = deleteList;
            List<Drawing.Polygon> polygons = SelectedShapes;
            if (polygons == null || polygons.Count == 0)
                return;
            libShapeFile.ShapeInfo shapeInfo = polygons[0].ShapeInfo;
            Drawing.ShapeLayer shapeLayer = (Drawing.ShapeLayer)polygons[0].GetLayer();
            FormMain.Instance.RemoveShapes(polygons, shapeInfo, shapeLayer);
        }


        private void button1_Click(object sender, EventArgs e)
        {            
            List<Drawing.Polygon> selPolygon = SelectedShapes;
            if (selPolygon == null || selPolygon.Count == 0)
                return;

            Dictionary<int, Drawing.Polygon> temp = new Dictionary<int, Drawing.Polygon>();
            foreach(Drawing.Polygon poly in selPolygon)
            {
                temp.Add(poly.ID, poly);
            }

            DXFViewer.Layer layer =  FormMain.Instance.Layer지적도;
            foreach (DXFViewer.Shape shape in layer.Shapes)
            {
                if (shape is Drawing.PolygonList)
                {
                    Drawing.PolygonList polygonList = (Drawing.PolygonList)shape;
                    List<Drawing.Polygon> polygons = polygonList.GetPolygons(temp);


                    libShapeFile.ShapeInfo shapeInfo = polygons[0].ShapeInfo;
                    Drawing.ShapeLayer shapeLayer = (Drawing.ShapeLayer)polygons[0].GetLayer();
                    FormMain.Instance.RemoveShapes(polygons, shapeInfo, shapeLayer);
                }
            }
        }
        
        void FormDetailAttrib_OnPressEnter()
        {
            CheckInputBox();

            bProcessRevers = true;

            DataGridViewRow row = dataGridView1.CurrentRow;
            if( row != null)
            {
                int nIdx = row.Index + 1;
                if (nIdx < dataGridView1.Rows.Count)
                {
                    row.Selected = false;
                    dataGridView1.Rows[nIdx].Selected = true;
                    this.dataGridView1.CurrentCell = dataGridView1.Rows[nIdx].Cells[4];
                    DataGridViewCellEventArgs e = new DataGridViewCellEventArgs(4, nIdx);
                    dataGridView1_CellClick(dataGridView1, e);
                    
                }
            }
            bProcessRevers = false;
        }

        private bool bProcessRevers = false;
        private bool bProcessAddRow = false;
        private void btnReverse_Click(object sender, EventArgs e)
        {
            bProcessRevers = true;
            foreach(DataGridViewRow row in dataGridView1.Rows)
            {
                row.Selected = !row.Selected;
            }

            bProcessRevers = false;
            FormMain.Instance.RefreshView();
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            dataGridView1.SelectAll();

            int nCount = 0;
            float fAreaTotal = 0.0f;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                bool bSelected = row.Selected;
                if( bSelected == true)
                {
                    nCount++;
                    string szValue = row.Cells[2].Value.ToString();
                    float fValue = 0.0f;
                    if( float.TryParse(szValue, out fValue))
                    {
                        fAreaTotal += fValue;
                    }                    
                }
            }

            lbSelectRow.Text = string.Format("선택열 : {0}", nCount);
            lbSelectArea.Text = fAreaTotal < 0.0f ? "" : string.Format("총면적 : {0:###,###,###,###,###,###}", (long)fAreaTotal);// strArea == null ? "" : strArea;


        }

        private void dataGridView1_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            if (bProcessAddRow == true)
                return;

            DataGridViewRow row = e.Row;
            if (e.StateChanged == DataGridViewElementStates.Selected)
            {
                Drawing.Polygon poly = (Drawing.Polygon)row.Tag;
                if( poly != null)
                {
                    if (row.Selected == true)
                    {
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            cell.Style.Font = new Font(dataGridView1.DefaultCellStyle.Font, FontStyle.Bold);
                        }                       
                        m_selectionMgr.SelectShape(poly);
                    }
                    else
                    {
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            cell.Style.Font = null;
                        }
                        m_selectionMgr.UnselectShape(poly);
                    }
                }                
            }       
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (bProcessRevers == true)
                return;
            FormMain.Instance.RefreshView();
            System.Diagnostics.Trace.WriteLine("Select Changed Row");
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            int nCal = e.ColumnIndex;
            if (nCal == 4)
            {
                int nRow = e.RowIndex;
                if (nRow < 0)
                    return;
                DataGridViewRow row = dataGridView1.Rows[nRow];
                if( row != null)
                {
                    Drawing.Polygon poly = (Drawing.Polygon)row.Tag;
                    if( poly != null)
                    {
                        PolygonInfo info = (PolygonInfo)poly.Tag;
                        if( info != null)
                        {
                            string szText = row.Cells[nCal].Value.ToString();
                            double value = 0.0;
                            if( double.TryParse(szText, out value))
                            {
                                info.Cost = value;
                                FormMain.Instance.ChangedData = true;
                            }
                            else
                            {
                                row.Cells[nCal].Value = info.Cost;
                            }
                        }
                    }
                }
            }
        }

        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            DataGridView.HitTestInfo info = dataGridView1.HitTest(e.X, e.Y);
            if( info.ColumnIndex == 4)
            {
                bProcessRevers = true;
                
            }
            else
            {
                bProcessRevers = false;
            }
            
        }
        private void CheckInputBox()
        {
            if (mInputBox.Visible == true)
            {
                string szText = mInputBox.Text;
                if (mCurrentEditCell != null)
                {
                    double nValue = 0.0;
                    if (double.TryParse(szText, out nValue))
                    {
                        mCurrentEditCell.Value = nValue < 0.0 ? "" : string.Format("{0:###,###,###,###,###,###}", (long)nValue);
                        mInputBox.Visible = false;
                        mCurrentEditCell = null;
                    }
                    else
                    {
                        mInputBox.Visible = false;
                        mCurrentEditCell = null;
                    }
                }
            }
        }
        private void dataGridView1_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            CheckInputBox();
        }

        private void dataGridView1_Scroll(object sender, ScrollEventArgs e)
        {
            CheckInputBox();
            mCurrentEditCell = null;
            mInputBox.Visible = false;
        }

        private void lbSelectArea_Click(object sender, EventArgs e)
        {

        }

    }

    public class PolygonInfo
    {
        // 면적(m²)
        // 0보다 작은 값은 아직 값이 지정되지 않은 상태를 의미한다.
        private double m_dArea = -1.0;
        private string m_strAddr = "";
        // 공시지가(원)
        // 0보다 작은 값은 아직 값이 지정되지 않은 상태를 의미한다.
        private double m_dCost = -1.0;
        private string m_strCode = "";
        private string m_strJibun = "";
        private LandType m_landType = LandType.Unknown;

        // 면적(m²)
        // 0보다 작은 값은 아직 값이 지정되지 않은 상태를 의미한다.
        public double Area
        {
            get { return m_dArea; }
            set { m_dArea = value; }
        }

        public string Address
        {
            get { return m_strAddr; }
            set { m_strAddr = value; }
        }

        // 공시지가(원)
        // 0보다 작은 값은 아직 값이 지정되지 않은 상태를 의미한다.
        public double Cost
        {
            get { return m_dCost; }
            set { m_dCost = value; }
        }

        public string Code
        {
            get { return m_strCode; }
            set { m_strCode = value; }
        }

        public string Jibun
        {
            get { return m_strJibun; }
            set { m_strJibun = value; }
        }

        public LandType Land
        {
            get { return m_landType; }
            set { m_landType = value; }
        }
    }

    public delegate void EnterKeyPressed();
    public class DataGridViewEx : DataGridView
    {
        public event EnterKeyPressed OnPressEnter;
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                if (OnPressEnter != null)
                    OnPressEnter();
                return true;
            }
            else
                return base.ProcessDialogKey(keyData);
        }
    }
}
