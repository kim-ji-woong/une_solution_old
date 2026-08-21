using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DXFViewer;
using UnE.Geometry;

namespace RoadMan
{
    public partial class FormResultProperty : Form
    {
        //private DateTimePicker m_timePicker = null;
        private ProcessResult m_result = null;
        private FormResultPropertyHistory m_frmHistory = null;
        private PanelDXFViewer m_panel = null;
        private SelectionManager m_selectionMgr = null;

        private const int STREET = 1;
        private const int BEGIN_TIME = 2;
        private const int END_TIME = 3;
        private const int PROJECT_COST = 4;
        private const int ACCUMUL_LENGTH = 5;
        private const int ACCUMUL_AREA = 6;
        private const int EDIT = 7;

        public FormResultPropertyHistory ResultPropertyHistoryForm
        {
            get { return m_frmHistory; }
            set { m_frmHistory = value; }
        }

        public FormResultProperty(ProcessResult result, PanelDXFViewer panel)
        {
            InitializeComponent();

            m_result = result;
            m_panel = panel;
            m_selectionMgr = new SelectionManager(panel);
        }

        private void FormResultProperty_Shown(object sender, EventArgs e)
        {
            if (m_result != null && m_result.ProcessSchedule != null)
            {
                this.Text = m_result.ProcessSchedule.ScheduleName + " - 집행 진행상황 속성";
            }

            InitGridHeader();
            //InitData();
            InitGrid();

            dataGridView1.Select();

            /*m_timePicker = new DateTimePicker();
            dataGridView1.Controls.Add(m_timePicker);
            m_timePicker.Visible = false;

            m_timePicker.CloseUp += new EventHandler(DateTimePicker_CloseUp);*/
        }

        private void InitGridHeader()
        {
            colNo.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colAddr.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colBeginTime.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colEndTime.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colProjectCost.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colProjectLength.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colProjectArea.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colEdit.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        // ProcessSchedule과 데이터를 동기화시킨다.
        /*private void InitData()
        {
            if (m_result.ProcessSchedule != null)
            {
                foreach (ScheduleProperty prop in m_result.ProcessSchedule.Properties)
                {
                    if (FindResultProperty(prop) == null)
                    {
                        ResultProperty prop2 = new ResultProperty();
                        prop2.ScheduleProperty = prop;
                        m_result.ResultProperties.Add(prop2);
                    }
                }
            }
        }

        private ResultProperty FindResultProperty(ScheduleProperty prop)
        {
            foreach (ResultProperty prop2 in m_result.ResultProperties)
            {
                if (prop2.ScheduleProperty == prop)
                    return prop2;
            }

            return null;
        }*/

        private void InitGrid()
        {
            foreach (ResultProperty prop in m_result.ResultProperties)
            {
                DataGridViewRow row = MakeNewRow();
                row.Tag = prop;

                _UpdateRow(row, prop);
            }
        }

        public void UpdateRow(DataGridViewRow row, ResultProperty prop)
        {
            _UpdateRow(row, prop);
            m_panel.ProcessResultForm.UpdateProcessSchedule(prop.ScheduleProperty.Schedule);
        }

        private void _UpdateRow(DataGridViewRow row, ResultProperty prop)
        {
            row.Cells[0].Value = row.Index + 1;
            row.Cells[STREET].Value = prop.ScheduleProperty == null ? "" : prop.ScheduleProperty.StreetName;

            VariousData<DateTime> beginTime = null, endTime = null;
            VariousData<long> projectCost = null;
            VariousData<int> accumulLength = null, accumulArea = null;
            GetStatistics(prop, ref beginTime, ref endTime, ref projectCost, ref accumulLength, ref accumulArea);

            if (beginTime != null)
            {
                row.Cells[BEGIN_TIME].Value = ScheduleProperty.GetDateTimeString(beginTime.Data);
                row.Cells[BEGIN_TIME].Tag = new VariousData<DateTime>(beginTime.Data);
            }
            else
            {
                row.Cells[BEGIN_TIME].Value = null;
                row.Cells[BEGIN_TIME].Tag = null;
            }

            if (endTime != null)
            {
                row.Cells[END_TIME].Value = ScheduleProperty.GetDateTimeString(endTime.Data);
                row.Cells[END_TIME].Tag = new VariousData<DateTime>(endTime.Data);
            }
            else
            {
                row.Cells[END_TIME].Value = null;
                row.Cells[END_TIME].Tag = null;
            }

            SetProjectCost(row.Cells[PROJECT_COST], projectCost == null ? null : new VariousData<long>(projectCost.Data));
            SetCompleteLength(row.Cells[ACCUMUL_LENGTH], accumulLength == null ? null : new VariousData<int>(accumulLength.Data));
            SetCompleteArea(row.Cells[ACCUMUL_AREA], accumulArea == null ? null : new VariousData<int>(accumulArea.Data));

            row.Cells[EDIT].Value = "열기";
        }

        private void GetStatistics(ResultProperty prop, ref VariousData<DateTime> beginTime, ref VariousData<DateTime> endTime,
            ref VariousData<long> projectCost, ref VariousData<int> accumulLength, ref VariousData<int> accumulArea)
        {
            foreach (ResultPropertyData data in prop.PropertyDatas)
            {
                if (data.BeginTime != null)
                {
                    if (beginTime == null)
                        beginTime = data.BeginTime;
                    else if (beginTime.Data > data.BeginTime.Data)
                        beginTime = data.BeginTime;
                }

                if (data.EndTime != null)
                {
                    if (endTime == null)
                    {
                        endTime = data.EndTime;
                    }
                    else if (endTime.Data < data.EndTime.Data)
                    {
						endTime = data.EndTime;
                    }
                }
                
                if (data.ProjectCost != null)
                {
                    if (projectCost == null)
                        projectCost = new VariousData<long>(data.ProjectCost.Data);
                    else
                        projectCost.Data += data.ProjectCost.Data;
                }

                if (data.UnitLength != null)
                {
                    if (accumulLength == null)
                        accumulLength = new VariousData<int>(data.UnitLength.Data);
                    else
                        accumulLength.Data += data.UnitLength.Data;
                }

                if (data.UnitArea != null)
                {
                    if (accumulArea == null)
                        accumulArea = new VariousData<int>(data.UnitArea.Data);
                    else
                        accumulArea.Data += data.UnitArea.Data;
                }
            }
        }

        private void SetProjectCost(DataGridViewCell cell, VariousData<long> nProjectCost)
        {
            if (nProjectCost != null)
            {
                SetProjectCost(cell, nProjectCost.Data);
                cell.Tag = nProjectCost;
            }
            else
            {
                cell.Value = null;
                cell.Tag = null;
            }
        }

        private void SetProjectCost(DataGridViewCell cell, long nProjectCost)
        {
            if (nProjectCost == 0)
                cell.Value = "0원";
            else
                cell.Value = string.Format("{0:###,###,###,###,###,###}원", nProjectCost);
        }

        private void SetCompleteLength(DataGridViewCell cell, VariousData<int> nLength)
        {
            if (nLength == null)
            {
                cell.Value = null;
                cell.Tag = null;
            }
            else
            {
                SetCompleteLength(cell, nLength.Data);
                cell.Tag = nLength;
            }
        }

        private void SetCompleteLength(DataGridViewCell cell, int nLength)
        {
            if (nLength == 0)
                cell.Value = "0";
            else
                cell.Value = string.Format("{0:###,###,###,###,###,###}", nLength);
        }

        private void SetCompleteArea(DataGridViewCell cell, VariousData<int> nArea)
        {
            if (nArea == null)
            {
                cell.Value = null;
                cell.Tag = null;
            }
            else
            {
                SetCompleteArea(cell, nArea.Data);
                cell.Tag = nArea;
            }
        }

        private void SetCompleteArea(DataGridViewCell cell, int nArea)
        {
            if (nArea == 0)
                cell.Value = "0";
            else
                cell.Value = string.Format("{0:###,###,###,###,###,###}", nArea);
        }

        private void SetCompleteDirection(DataGridViewCell cell, VariousData<bool> fromBegin)
        {
            if (fromBegin == null)
            {
                cell.Value = null;
            }
            else
            {
                SetCompleteDirection(cell, fromBegin.Data);
            }
        }

        private void SetCompleteDirection(DataGridViewCell cell, bool fromBegin)
        {
            cell.Value = fromBegin ? "시점으로부터" : "종점으로부터";
            cell.Tag = fromBegin;
        }

        /*private void DateTimePicker_CloseUp(object sender, EventArgs e)
        {
            string strTime = string.Format("{0}-{1}-{2}", m_timePicker.Value.Year, m_timePicker.Value.Month, m_timePicker.Value.Day);

            if (m_timePicker.Tag != null)
            {
                DataGridViewCell cell = (DataGridViewCell)m_timePicker.Tag;
                cell.Value = strTime;

                if (cell.Tag == null)
                    cell.Tag = new VariousData<DateTime>(m_timePicker.Value);
                else
                    ((VariousData<DateTime>)cell.Tag).Data = m_timePicker.Value;

                m_timePicker.Hide();
            }
        }*/

        private void dataGridView1_Scroll(object sender, ScrollEventArgs e)
        {
            //m_timePicker.Hide();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                if (row.IsNewRow)
                    return;

                m_selectionMgr.SelectRow(row);
            }
            /*if (m_timePicker == null)
                return;

            m_timePicker.Hide();

            if ((e.ColumnIndex == BEGIN_TIME || e.ColumnIndex == END_TIME) && e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                DataGridViewCell cell = row.Cells[e.ColumnIndex];

                Rectangle rect = dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);

                if (cell.Value != null)
                {
                    DateTime date = new DateTime();

                    if (GetDateTime(cell.Value, ref date))
                    {
                        m_timePicker.Value = date;
                    }
                }

                m_timePicker.Location = new Point(rect.Left, rect.Top);
                m_timePicker.Tag = cell;

                m_timePicker.Size = new Size(rect.Width, rect.Height);
                m_timePicker.Show();
            }*/
        }

        private bool GetDateTime(object obj, ref DateTime date)
        {
            string strText = obj.ToString();
            string[] arrDate = strText.Split('-');

            if (arrDate.Count() != 3)
                return false;

            int nYear, nMonth, nDay;

            if (!int.TryParse(arrDate[0], out nYear))
                return false;

            if (!int.TryParse(arrDate[1], out nMonth))
                return false;

            if (!int.TryParse(arrDate[2], out nDay))
                return false;

            try
            {
                date = new DateTime(nYear, nMonth, nDay);
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }

            return true;
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            /*if (dataGridView1.CurrentCellAddress.X == colStartDirection.DisplayIndex)
            {
                ComboBox cb = e.Control as ComboBox;

                if (cb != null)
                {
                    cb.Tag = dataGridView1.CurrentCell;

                    if (dataGridView1.CurrentCell != null)
                    {
                        if (dataGridView1.CurrentCell.Tag == null)
                        {
                            ComboBoxText text = new ComboBoxText();
                            text.Control = cb;
                            dataGridView1.CurrentCell.Tag = text;
                        }
                    }

                    cb.DropDownStyle = ComboBoxStyle.DropDownList;
                }
            }*/
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            DataGridViewCell cell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];

            if (e.ColumnIndex == PROJECT_COST)
            {
                if (cell.Tag != null)
                {
                    long nCost = (long)cell.Tag;
                    cell.Value = nCost;
                }
            }
            else if (e.ColumnIndex == ACCUMUL_LENGTH)
            {
                if (cell.Tag != null)
                {
                    int nLength = (int)cell.Tag;
                    cell.Value = nLength;
                }
            }
            else if (e.ColumnIndex == ACCUMUL_AREA)
            {
                if (cell.Tag != null)
                {
                    int nArea = (int)cell.Tag;
                    cell.Value = nArea;
                }
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];

            /*if (e.ColumnIndex == 8)
            {
                if (cell.Tag == null)
                    return;

                ComboBoxText text = (ComboBoxText)cell.Tag;
                string strText = text.Control.Text;

                if (strText.Length == 0)
                {
                    cell.Value = null;
                    cell.Tag = null;
                }
                else if (strText.Length > 0 && text.EndType != ComboBoxText.EndEditType.CANCEL)
                {
                    DataGridViewComboBoxColumn column = (DataGridViewComboBoxColumn)dataGridView1.Columns[e.ColumnIndex];

                    if (!column.Items.Contains(strText))
                    {
                        column.Items.Add(strText);
                        cell.Value = strText;
                        text.EndType = ComboBoxText.EndEditType.NONE;
                    }
                    else
                        cell.Value = strText;
                }
            }
            else */if (e.ColumnIndex == PROJECT_COST)
            {
                if (cell.Value != null && cell.Value.ToString().Length > 0)
                {
                    long nCost;

                    if (!long.TryParse(cell.Value.ToString(), out nCost))
                    {
						string szMsg = "사업비 값이 잘못되었습니다.";
                        UnE.Utility.UMessageBox.Show(this, szMsg, "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);		

                        //MessageBox.Show("사업비 값이 잘못되었습니다.");

                        if (cell.Tag == null)
                            cell.Value = null;
                        else
                            SetProjectCost(cell, (VariousData<long>)cell.Tag);
                    }
                    else
                    {
                        SetProjectCost(cell, nCost);
                        cell.Tag = new VariousData<long>(nCost);
                    }
                }
            }
            else if (e.ColumnIndex == ACCUMUL_LENGTH)
            {
                if (cell.Value != null && cell.Value.ToString().Length > 0)
                {
                    int nLength;

                    if (!int.TryParse(cell.Value.ToString(), out nLength))
                    {
						string szMsg = "길이 값이 잘못되었습니다.";
                        UnE.Utility.UMessageBox.Show(this, szMsg, "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        //MessageBox.Show("길이 값이 잘못되었습니다.");

                        if (cell.Tag == null)
                            cell.Value = null;
                        else
                            SetCompleteLength(cell, (VariousData<int>)cell.Tag);
                    }
                    else
                    {
                        SetCompleteLength(cell, nLength);
                        cell.Tag = new VariousData<int>(nLength);
                    }
                }
            }
            else if (e.ColumnIndex == ACCUMUL_AREA)
            {
                if (cell.Value != null && cell.Value.ToString().Length > 0)
                {
                    int nArea;

                    if (!int.TryParse(cell.Value.ToString(), out nArea))
                    {
						string szMsg = "면적 값이 잘못되었습니다.";
                        UnE.Utility.UMessageBox.Show(this, szMsg, "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        //MessageBox.Show("면적 값이 잘못되었습니다.");

                        if (cell.Tag == null)
                            cell.Value = null;
                        else
                            SetCompleteArea(cell, (VariousData<int>)cell.Tag);
                    }
                    else
                    {
                        SetCompleteArea(cell, nArea);
                        cell.Tag = new VariousData<int>(nArea);
                    }
                }
            }
        }

        private DataGridViewRow MakeNewRow()
        {
            int nIndex = 0;

            if (dataGridView1.AllowUserToAddRows)
            {
                nIndex = dataGridView1.Rows.Count - 1;
                DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[dataGridView1.Rows.Count - 1].Clone();
                dataGridView1.Rows.Add(row);
            }
            else
            {
                dataGridView1.AllowUserToAddRows = true;

                nIndex = dataGridView1.Rows.Count - 1;
                DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[dataGridView1.Rows.Count - 1].Clone();
                dataGridView1.Rows.Add(row);

                dataGridView1.AllowUserToAddRows = false;
            }

            return dataGridView1.Rows[nIndex];
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            /*foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                ResultProperty prop = (ResultProperty)row.Tag;

                if (prop == null)
                    continue;

                ResultPropertyData data = prop.FindPropertyData((VariousData<DateTime>)row.Cells[3].Tag, (VariousData<DateTime>)row.Cells[4].Tag);

                if (data == null)
                {
                    data = new ResultPropertyData();

                    data.BeginTime = (VariousData<DateTime>)row.Cells[3].Tag;
                    data.EndTime = (VariousData<DateTime>)row.Cells[4].Tag;

                    prop.PropertyDatas.Add(data);
                    prop.Sort();
                }

                data.ProjectName = row.Cells[2].Value == null ? "" : row.Cells[2].Value.ToString();
                data.ProjectCost = (VariousData<long>)row.Cells[5].Tag;
                data.AccumulLength = (VariousData<int>)row.Cells[6].Tag;
                data.AccumulArea = (VariousData<int>)row.Cells[7].Tag;

                if (row.Cells[8].Value == null || row.Cells[8].Value.ToString().Length == 0)
                    data.CompleteFromBegin = null;
                else
                    data.CompleteFromBegin = new VariousData<bool>(row.Cells[8].Value.ToString() == "시점으로부터" ? true : false);
            }*/

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == EDIT)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                ResultProperty prop = (ResultProperty)row.Tag;

                if (prop != null)
                {
                    if (m_frmHistory == null)
                        m_frmHistory = new FormResultPropertyHistory(prop, this, row);
                    else if (m_frmHistory.ResultProperty == prop)
                        return;
                    else
                    {
                        m_frmHistory.Close();
                        m_frmHistory = new FormResultPropertyHistory(prop, this, row);
                    }

					m_frmHistory.TopMost = true;
					DialogFormFrame frameHistory = new DialogFormFrame(m_frmHistory);
					frameHistory.Sizable = true;
					frameHistory.MinimizeBox = true;
					frameHistory.MaximizeBox = true;
					frameHistory.ShowMaxButton = true;
					frameHistory.ShowMinButton = true;
					frameHistory.Show(this);
                }
            }
        }

        private class SelectionManager
        {
            // 시점으로부터 시작된 구간
            private List<EditBoxHatch> m_hatchFromBegins = new List<EditBoxHatch>();
            // 종점으로부터 시작된 구간
            private List<EditBoxHatch> m_hatchFromEnds = new List<EditBoxHatch>();
            private ResultProperty m_propSelected = null;
            private PanelDXFViewer m_panel = null;
            
            public SelectionManager(PanelDXFViewer panel)
            {
                m_panel = panel;
            }

            public void SelectRow(DataGridViewRow row)
            {
                ResultProperty prop = (ResultProperty)row.Tag;

                bool needRefresh = false;

                if (prop == m_propSelected)
                    return;
                else
                {
                    // 기존 Hatch 해제
                    needRefresh = ClearSelection();
                }

                m_propSelected = prop;

                if (m_propSelected == null)
                {
                    if (needRefresh)
                        m_panel.DXFControl.Refresh();
                    return;
                }

                List<Shape> shapes;

                if (!m_panel.DataManager.StreetShapes.TryGetValue(m_propSelected.ScheduleProperty.StreetName, out shapes))
                {
                    if (needRefresh)
                        m_panel.DXFControl.Refresh();
                    return;
                }

                if (Options.Instance.ZoomOnSelectStreet == true)
                {
                    // 도로 ZoomIn
                    m_panel.DataManager.ObjectZoom(shapes, m_panel, true);
                }

                //m_panel.DataManager.ObjectZoom(m_propSelected.ScheduleProperty.StreetName, m_panel, true);
                // 중심선을 이용한 Hatch 계산 및 그리기
                MakeHatch(shapes);

                m_panel.DXFControl.Refresh();
            }

            private void MakeHatch(List<Shape> shapes)
            {
                DXFExternPainter externPainter = (DXFExternPainter)m_panel.DXFControl.ExternalPainter;

                if (externPainter == null)
                    return;

                Layer layer = externPainter.GetLayer(DXFExternPainter.LayerType.PROCESS_RESULT);

                if (layer == null)
                    return;

                int nAccumulLengthFromBegin, nAccumulLengthFromEnd;
                GetAccumulLength(out nAccumulLengthFromBegin, out nAccumulLengthFromEnd);

                StreetCenterLine2 centerLine;

                if (!m_panel.DataManager.StreetCenterLines.TryGetValue(m_propSelected.ScheduleProperty.StreetName, out centerLine))
                    return;

                double dTotalLength = centerLine.TotalLength;

                if (dTotalLength <= (double)(nAccumulLengthFromBegin + nAccumulLengthFromEnd))
                {
                    foreach (Shape shape in shapes)
                    {
                        PolyLineEx polyLine = ToPolyLineEx(shape);

                        if (polyLine != null)
                            MakeFullPolygonHatch(polyLine, m_hatchFromBegins);
                    }
                }
                else
                {
                    MakeHatch(nAccumulLengthFromBegin, shapes, true);
                    MakeHatch(nAccumulLengthFromEnd, shapes, false);
                }

                AddToLayer(layer);
            }

            private void AddToLayer(Layer layer)
            {
                foreach (EditBoxHatch hatch in m_hatchFromBegins)
                {
                    layer.Add(hatch);
                }

                foreach (EditBoxHatch hatch in m_hatchFromEnds)
                {
                    layer.Add(hatch);
                }
            }

            private PolyLineEx ToPolyLineEx(Shape shape)
            {
                if (shape.GetShapeType() == Shape.ShapeType.POLYLINE)
                {
                    PolyLineEx polyTrg = new PolyLineEx();
                    PolyLine polySrc = (PolyLine)shape;

                    int nVertexCount = polySrc.GetVertexSize();
                    polyTrg.SetPointSize(nVertexCount);

                    for (int i=0;i<nVertexCount;i++)
                    {
                        PointF pt = polySrc.GetVertex(i);
                        polyTrg.UpdatePoint(i, pt.X, pt.Y);
                    }

                    return polyTrg;
                }
                else if (shape.GetShapeType() == Shape.ShapeType.HATCH)
                {
                    PolyLineEx polyTrg = new PolyLineEx();
                    Hatch hatch = (Hatch)shape;

                    int nVertexCount = hatch.GetPointSize();
                    polyTrg.SetPointSize(nVertexCount);

                    float x, y;

                    for (int i = 0; i < nVertexCount; i++)
                    {
                        if (hatch.GetPoint(i, out x, out y))
                        {
                            polyTrg.UpdatePoint(i, x, y);
                        }
                    }

                    return polyTrg;
                }

                return null;
            }

            private void MakeHatch(int nAccumulLength, List<Shape> shapes, bool dirFromBegin)
            {
                if (nAccumulLength == 0)
                    return;

                bool fullPolygon = false;
                List<Shape> targetShapes = new List<Shape>();
                Vertex2D prevVertex;
                Vertex2D vertex = GetVertex(nAccumulLength, shapes, dirFromBegin, ref fullPolygon, targetShapes, out prevVertex);

                List<EditBoxHatch> hatchList = dirFromBegin ? m_hatchFromBegins : m_hatchFromEnds;

                if (vertex == null)
                {
                    if (!fullPolygon)
                        return;
                    else
                    {
                        foreach (Shape shape in targetShapes)
                        {
                            MakeFullPolygonHatch(shape, hatchList);
                        }
                    }
                }
                else
                {
                    int nTargetCount = targetShapes.Count;

                    for (int i=0;i<nTargetCount-1;i++)
                    {
                        Shape shape = targetShapes[i];
                        MakeFullPolygonHatch(shape, hatchList);
                    }

                    if (nTargetCount > 0)
                        MakeHatch(targetShapes[nTargetCount - 1], vertex, prevVertex, hatchList);
                }
            }

            private void MakeHatch(Shape shape, Vertex2D vPos, Vertex2D vPrev, List<EditBoxHatch> hatchList)
            {
                Polygon polygon = null;

                if (shape is PolyLine)
                    polygon = ((PolyLine)shape).GetPolygon();
                else if (shape is Hatch)
                    polygon = ((Hatch)shape).Polygon;
                else
                    return;

                // vPos가 폴리곤의 외부에 있다.
                if (polygon.HitTest(vPos) == 0)
                    return;

                double dLen = vPos.GetDistance(vPrev);

                if (dLen <= UnE.Geometry.Math.HALF_TOLERANCE())
                    return;

                Vertex2D vRight = UnE.Geometry.Math.GetRightVertex(vPos, vPrev, 100.0);
                Vertex2D vLeft = UnE.Geometry.Math.GetRightVertex(vPos, vPrev, -100.0);

                Line2D lineRight = new Line2D(vPos, vRight, Line2D.LineType.HALF_LINE_BEGIN_2_END);
                Line2D lineLeft = new Line2D(vPos, vLeft, Line2D.LineType.HALF_LINE_BEGIN_2_END);
                Line2D lineBack = new Line2D(vPos, vPrev, Line2D.LineType.HALF_LINE_BEGIN_2_END);

                int nVertexCount = polygon.GetVertexCount();

                Line2D.LineType lineTypeTemp;
                Vertex2D vTemp1, vTemp2;
                vRight = vLeft = vPrev = null;

                double dRightMin = -1.0, dLeftMin = -1.0, dBackMin = -1.0;

                Vertex2D v1 = polygon.GetVertex(nVertexCount - 1);

                for (int i=0;i<nVertexCount;i++)
                {
                    Vertex2D v2 = polygon.GetVertex(i);
                    Line2D line = new Line2D(v1, v2, Line2D.LineType.SEGMENT);

                    int nResult = lineRight.IntersectLine(line, out vTemp1, out vTemp2, out lineTypeTemp);

                    if (nResult > 0)
                    {
                        double dist = vPos.GetDistance(vTemp1);

                        if (dRightMin < 0.0 || dRightMin > dist)
                        {
                            vRight = vTemp1;
                            dRightMin = dist;
                        }

                        // lineRight와 만나는 점은 여러점일 수 있다.
                        // 그 중 vPos와 가장 가까운 점을 선택해야 하므로 전체 Polygon Vertext를 모두 검사하여야 한다.
                        /*if (vLeft != null && vPrev != null)
                            break;*/
                    }

                    nResult = lineLeft.IntersectLine(line, out vTemp1, out vTemp2, out lineTypeTemp);

                    if (nResult > 0)
                    {
                        double dist = vPos.GetDistance(vTemp1);

                        if (dLeftMin < 0.0 || dLeftMin > dist)
                        {
                            vLeft = vTemp1;
                            dLeftMin = dist;
                        }

                        /*if (vRight != null && vPrev != null)
                            break;*/
                    }

                    nResult = lineBack.IntersectLine(line, out vTemp1, out vTemp2, out lineTypeTemp);

                    if (nResult > 0)
                    {
                        double dist = vPos.GetDistance(vTemp1);

                        if (dBackMin < 0.0 || dBackMin > dist)
                        {
                            vPrev = vTemp1;
                            dBackMin = dist;
                        }

                        /*if (vRight != null && vLeft != null)
                            break;*/
                    }

                    v1 = v2;
                }

                if (vRight == null || vLeft == null || vPrev == null)
                    return;

                EditBoxHatch hatch = new EditBoxHatch(polygon);

                hatch.AddEditBoxVertex(vRight);
                hatch.AddEditBoxVertex(vPrev);
                hatch.AddEditBoxVertex(vLeft);
                //hatch.Calc();
                hatch.VisibleEditBox = false;

                hatchList.Add(hatch);
            }

            private void MakeFullPolygonHatch(Shape shape, List<EditBoxHatch> hatchList)
            {
                Polygon polygon = null;

                if (shape is PolyLine)
                    polygon = ((PolyLine)shape).GetPolygon();
                else if (shape is Hatch)
                    polygon = ((Hatch)shape).Polygon;
                else
                    return;

                EditBoxHatch hatch = new EditBoxHatch(polygon);
                hatch.SetFullPolygon();
                hatch.VisibleEditBox = false;

                hatchList.Add(hatch);
            }

            // Return 값은 타겟 폴리곤내에 위치하게될 Vertex를 의미한다.
            // 폴리곤의 시작점 혹은 끝점에서부터 해당 Vertex까지 폴리곤을 만들려면 어느방향인지를 알아야 하는데
            // prevVertex가 그 방향을 위한 값이다.
            private Vertex2D GetVertex(int nAccumulLength, List<Shape> shapes, bool dirFromBegin, ref bool fullPolygon, List<Shape> targetShapes, out Vertex2D prevVertex)
            {
                prevVertex = null;

                StreetCenterLine2 centerLine;

                if (!m_panel.DataManager.StreetCenterLines.TryGetValue(m_propSelected.ScheduleProperty.StreetName, out centerLine))
                    return null;

                int nLineCount = centerLine.PolyLines.Count;

                double dTargetLength = nAccumulLength;
                double dSourceLength = 0.0;

                if (dirFromBegin)
                {
                    for (int i=0;i<nLineCount;i++)
                    {
                        KeyValuePair<Shape, PolyLineEx> pair = centerLine.PolyLines.ElementAt(i);
                        dSourceLength += pair.Value.LineLength;
                        targetShapes.Add(pair.Key);

                        if (dSourceLength == dTargetLength)
                        {
                            int nVertexCount = pair.Value.GetVertexSize();

                            if (nVertexCount < 2)
                                return null;

                            PointF ptPrev = pair.Value.GetVertex(nVertexCount - 2);
                            prevVertex = new Vertex2D(ptPrev.X, ptPrev.Y);

                            PointF pt = pair.Value.GetVertex(nVertexCount - 1);
                            return new Vertex2D(pt.X, pt.Y);
                        }
                        else if (dSourceLength > dTargetLength)
                        {
                            double dLen = dTargetLength - (dSourceLength - pair.Value.LineLength);

                            int nPrevIndex;
                            Vertex2D vTarget = pair.Value.GetVertex(dLen, dirFromBegin, out nPrevIndex);

                            if (vTarget == null)
                                fullPolygon = true;
                            else
                            {
                                PointF pt = pair.Value.GetVertex(nPrevIndex);
                                prevVertex = new Vertex2D(pt.X, pt.Y);
                            }

                            return vTarget;
                        }
                    }
                }
                else
                {
                    for (int i = nLineCount - 1; i >= 0; i--)
                    {
                        KeyValuePair<Shape, PolyLineEx> pair = centerLine.PolyLines.ElementAt(i);
                        dSourceLength += pair.Value.LineLength;
                        targetShapes.Add(pair.Key);

                        if (dSourceLength == dTargetLength)
                        {
                            if (pair.Value.GetVertexSize() < 2)
                                return null;

                            PointF ptPrev = pair.Value.GetVertex(1);
                            prevVertex = new Vertex2D(ptPrev.X, ptPrev.Y);

                            PointF pt = pair.Value.GetVertex(0);
                            return new Vertex2D(pt.X, pt.Y);
                        }
                        else if (dSourceLength > dTargetLength)
                        {
                            double dLen = dTargetLength - (dSourceLength - pair.Value.LineLength);

                            int nPrevIndex;
                            Vertex2D vTarget = pair.Value.GetVertex(dLen, dirFromBegin, out nPrevIndex);

                            if (vTarget == null)
                                fullPolygon = true;
                            else
                            {
                                PointF pt = pair.Value.GetVertex(nPrevIndex);
                                prevVertex = new Vertex2D(pt.X, pt.Y);
                            }

                            return vTarget;
                        }
                    }
                }

                fullPolygon = true;
                return null;
            }

            private void GetAccumulLength(out int nAccumulLengthFromBegin, out int nAccumulLengthFromEnd)
            {
                nAccumulLengthFromBegin = nAccumulLengthFromEnd = 0;

                foreach (ResultPropertyData data in m_propSelected.PropertyDatas)
                {
                    if (data.DirectionFromBegin == null)
                        continue;

                    if (data.DirectionFromBegin.Data)
                    {
                        if (data.UnitLength != null)
                            nAccumulLengthFromBegin += data.UnitLength.Data;
                    }
                    else
                    {
                        if (data.UnitLength != null)
                            nAccumulLengthFromEnd += data.UnitLength.Data;
                    }
                }
            }

            // Return 값 : true이면 Refresh가 필요
            //             false이면 필요없음
            public bool ClearSelection()
            {
                bool needRefresh = false;

                foreach (Shape shape in m_hatchFromBegins)
                {
                    needRefresh = true;
                    shape.GetLayer().Shapes.Remove(shape);
                }

                foreach (Shape shape in m_hatchFromEnds)
                {
                    needRefresh = true;
                    shape.GetLayer().Shapes.Remove(shape);
                }

                m_hatchFromBegins.Clear();
                m_hatchFromEnds.Clear();

                return needRefresh;
            }
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && dataGridView1.CurrentRow != null)
            {
                ResultProperty prop = (ResultProperty)dataGridView1.CurrentRow.Tag;

                if (prop != null)
                {
                    Rectangle rect = dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                    ShowMenu(e.X + rect.Left, e.Y + rect.Top, prop, dataGridView1.CurrentRow);
                }
            }
        }

        private void ShowMenu(int x, int y, ResultProperty prop, DataGridViewRow row)
        {
            contextMenuStrip1.Items.Clear();

            if (prop.ScheduleProperty != null && prop.ScheduleProperty.Schedule != null)
            {
                PanelDXFViewer panel = FormMain.Instance.CurrentPanel;

                if (panel != null)
                {
                    List<ProcessResult> results = panel.ProcessResults;

                    ProcessResult resultSource = null;

                    foreach (ProcessResult result in results)
                    {
                        if (result.ProcessSchedule == prop.ScheduleProperty.Schedule)
                        {
                            resultSource = result;
                            break;
                        }
                    }

                    if (resultSource == null)
                        return;

                    foreach (ProcessResult result in results)
                    {
                        if (result.ProcessSchedule == prop.ScheduleProperty.Schedule)
                            continue;

                        ToolStripMenuItem menu = new ToolStripMenuItem("[" + result.ProcessSchedule.ScheduleName + "]으로 이동", null, menuMoveResult_Click);
                        menu.Tag = new MovingResult(result, resultSource, prop, row);
                        contextMenuStrip1.Items.Add(menu);
                    }
                }
            }

            if (contextMenuStrip1.Items.Count > 0)
                contextMenuStrip1.Show(dataGridView1, x, y);
        }

        private void menuMoveResult_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menu = (ToolStripMenuItem)sender;
            MovingResult moving = (MovingResult)menu.Tag;

            if (moving == null || moving.ResultTarget == null || moving.ResultSource == null || moving.ResultProperty == null || moving.Row == null)
                return;

            string strMessage = string.Format("[{0}]을 [{1}]로 옮기시겠습니까?", moving.ResultProperty.ScheduleProperty.StreetName, moving.ResultTarget.ProcessSchedule.ScheduleName);

            if (UnE.Utility.UMessageBox.Show(this, strMessage, "확인", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                moving.ResultSource.ResultProperties.Remove(moving.ResultProperty);
                moving.ResultTarget.ResultProperties.Add(moving.ResultProperty);

                moving.ResultSource.ProcessSchedule.Properties.Remove(moving.ResultProperty.ScheduleProperty);
                moving.ResultTarget.ProcessSchedule.Properties.Add(moving.ResultProperty.ScheduleProperty);
                moving.ResultProperty.ScheduleProperty.Schedule = moving.ResultTarget.ProcessSchedule;

                FormMain.Instance.CurrentPanel.ProcessScheduleForm.CloseScheduleProperty();

                dataGridView1.Rows.Remove(moving.Row);
            }
        }

        private void dataGridView1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            int nRowCount = dataGridView1.Rows.Count;

            for (int i = e.RowIndex; i < nRowCount; i++)
            {
                DataGridViewRow row = dataGridView1.Rows[i];

                if (!row.IsNewRow)
                    row.Cells[0].Value = i + 1;
            }
        }

        private void FormResultProperty_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_selectionMgr.ClearSelection())
                m_panel.DXFControl.Refresh();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (msg.Msg == WindowMessage.WM_KEYDOWN ||
                msg.Msg == WindowMessage.WM_CHAR ||
                msg.Msg == WindowMessage.WM_SYSKEYDOWN)
            {
                if (keyData == Keys.F1)
                {
                    FormMain.Instance.ShowHelp();
                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private class MovingResult
        {
            private ProcessResult m_resultTarget = null;
            private ProcessResult m_resultSource = null;
            private ResultProperty m_property = null;
            private DataGridViewRow m_row = null;

            public ProcessResult ResultTarget
            {
                get { return m_resultTarget; }
                set { m_resultTarget = value; }
            }

            public ProcessResult ResultSource
            {
                get { return m_resultSource; }
                set { m_resultSource = value; }
            }

            public ResultProperty ResultProperty
            {
                get { return m_property; }
                set { m_property = value; }
            }

            public DataGridViewRow Row
            {
                get { return m_row; }
                set { m_row = value; }
            }

            public MovingResult()
            {
            }

            public MovingResult(ProcessResult resultTarget, ProcessResult resultSource, ResultProperty prop, DataGridViewRow row)
            {
                m_resultTarget = resultTarget;
                m_resultSource = resultSource;
                m_property = prop;
                m_row = row;
            }
        }
    }
}
