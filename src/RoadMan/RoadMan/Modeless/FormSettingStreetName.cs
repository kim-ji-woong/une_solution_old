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

namespace RoadMan
{
    public partial class FormSettingStreetName : Form, ISettingStreet
    {
        private Dictionary<string, List<Shape>> m_dicStreetShapes = new Dictionary<string, List<Shape>>();
        private List<ProcessSchedule> m_listProcessSchedules = null;
        private PanelDXFViewer m_panel = null;
        private PanelDXFViewer.ActivityType m_prevActivityType = PanelDXFViewer.ActivityType.NONE;
        private DataGridViewRow m_rowSelected = null;

        public FormSettingStreetName(PanelDXFViewer panel)
        {
            InitializeComponent();

            m_panel = panel;

            if (m_panel != null)
            {
                m_dicStreetShapes = m_panel.DataManager.StreetShapes;
                m_listProcessSchedules = m_panel.ProcessScheduleForm.ProcessSchedules;
            }
        }

        private void FormSettingStreetName_Load(object sender, EventArgs e)
        {
            InitGridHeader();
            InitGrid();

            if (m_panel != null)
            {
                m_panel.SettingStreet = this;
				m_prevActivityType = m_panel.Activity;
                m_panel.Activity = PanelDXFViewer.ActivityType.SETTING_STREET;
            }

            dataGridView1.ClearSelection();
        }

        private void InitGrid()
        {
            if (m_listProcessSchedules == null)
                return;

            List<string> streetNames = GetStreetNames();
            int nIndex = 0;
            List<Shape> shapes;

            foreach (string strStreetName in streetNames)
            {
                DataGridViewRow row = MakeNewRow();

                row.Cells[0].Value = ++nIndex;
                row.Cells[1].Value = /*" " + */strStreetName;
                row.Cells[1].Tag = strStreetName;

                if (m_dicStreetShapes != null)
                {
                    if (m_dicStreetShapes.TryGetValue(strStreetName, out shapes))
                    {
                        row.Cells[2].Value = shapes.Count;
                        row.Tag = shapes;
                    }
                }

                if (row.Cells[2].Value == null)
                    row.Cells[2].Value = 0;

                row.Cells[3].Value = "실행";
            }
        }

        private DataGridViewRow MakeNewRow()
        {
            int nIndex = -1;

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

        private List<string> GetStreetNames()
        {
            Dictionary<string, string> dicStreetNames = new Dictionary<string,string>();
            
            foreach (ProcessSchedule schedule in m_listProcessSchedules)
            {
                foreach (ScheduleProperty prop in schedule.Properties)
                {
                    if (prop.StreetName.Length == 0)
                        continue;

                    dicStreetNames[prop.StreetName] = "";
                }
            }

            List<string> streetNames = new List<string>();

            foreach (KeyValuePair<string, string> pair in dicStreetNames)
            {
                streetNames.Add(pair.Key);
            }

            streetNames.Sort();
            return streetNames;
        }

        private void InitGridHeader()
        {
            colNo.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colStreetName.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colObjectCount.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colInitialize.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                List<Shape> shapes = (List<Shape>)row.Tag;

                if (shapes != null)
                {
                    row.Cells[2].Value = 0;
                    SelectShapes(shapes, false, true);
                    shapes.Clear();
                }
            }
        }

        private List<Shape> GetSelectedShapes(out DataGridViewRow row)
        {
            row = null;

            if (dataGridView1.SelectedCells.Count == 0)
                return null;

            DataGridViewCell cell = dataGridView1.SelectedCells[0];
            row = dataGridView1.Rows[cell.RowIndex];

            List<Shape> shapes = null;

            if (row.Tag == null)
            {
                shapes = new List<Shape>();
                row.Tag = shapes;

                string strStreetName = (string)row.Cells[1].Tag;

                if (strStreetName != null && m_dicStreetShapes != null)
                    m_dicStreetShapes[strStreetName] = shapes;
            }
            else
                shapes = (List<Shape>)row.Tag;

            return shapes;
        }

        // Return 값 : 변경된 값이 있는가?
        public static bool SelectShapes(PanelDXFViewer panel, List<Shape> shapes, bool selected, bool refresh)
        {
            bool isChanged = false;

            foreach (Shape shape in shapes)
            {
                if (SelectShape(panel, shape, selected))
                    isChanged = true;
            }

            if (refresh && isChanged)
                panel.DXFControl.Refresh();

            return isChanged;
        }

        // Return 값 : 변경된 값이 있는가?
        private bool SelectShapes(List<Shape> shapes, bool selected, bool refresh)
        {
            return SelectShapes(m_panel, shapes, selected, refresh);
        }

        // Return 값 : 변경된 값이 있는가?
        public static bool SelectShape(PanelDXFViewer panel, Shape shape, bool selected)
        {
            bool isChanged = false;

            if (selected)
            {
                if (shape.Selected == false | shape.SelectedShowing != Shape.SelectedShowingType.DRAW_POLYGON)
                    isChanged = true;

                shape.Selected = true;
                shape.SelectedShowing = Shape.SelectedShowingType.DRAW_POLYGON;
                panel.AddFixedSelection(shape);
            }
            else
            {
                if (shape.Selected == true | shape.SelectedShowing != Shape.SelectedShowingType.BRIGHT_EFFECT)
                    isChanged = true;

                shape.Selected = false;
                shape.SelectedShowing = Shape.SelectedShowingType.BRIGHT_EFFECT;
                panel.RemoveFixedSelection(shape);
            }

            return isChanged;
        }

        // Return 값 : 변경된 값이 있는가?
        private bool SelectShape(Shape shape, bool selected)
        {
            return SelectShape(m_panel, shape, selected);
        }

        public void SetShape(Shape shape)
        {
            DataGridViewRow row;
            List<Shape> shapes = GetSelectedShapes(out row);

            if (shapes == null)
                return;

            if (shapes.Contains(shape))
            {
                // 하나만 선택되어 있는 경우 같은 Shape을 다시 선택하면 선택을 해제한다. 
                if (shapes.Count == 1)
                {
                    shapes.Remove(shape);

                    if (SelectShape(shape, false))
                        m_panel.DXFControl.Refresh();
                }
                // 여러개가 선택되어 있는 경우 방금 선택한 Shape만 남기고 나머지는 모두 선택을 해제한다.
                else
                {
                    SelectShapes(shapes, false, false);
                    shapes.Clear();

                    shapes.Add(shape);
                    SelectShape(shape, true);
                    m_panel.DXFControl.Refresh();
                }

                row.Cells[2].Value = shapes.Count;
                return;
            }

            bool isChanged = SelectShapes(shapes, false, false);
            shapes.Clear();

            if (shape == null)
            {
                row.Cells[2].Value = 0;
            }
            else
            {
                row.Cells[2].Value = 1;
                shapes.Add(shape);

                if (SelectShape(shape, true))
                    isChanged = true;
            }

            UpdateDataManager(shapes);

            if (isChanged)
                m_panel.DXFControl.Refresh();
        }

        private void UpdateDataManager(List<Shape> shapes)
        {
            if (m_rowSelected != null)
            {
                if (m_rowSelected.Cells[1].Value != null)
                {
                    string strSteetName = m_rowSelected.Cells[1].Value.ToString();
					//strSteetName = strSteetName.Trim();

                    List<Shape> shapesTrg;

                    if (!m_panel.DataManager.StreetShapes.TryGetValue(strSteetName, out shapesTrg))
                    {
                        shapesTrg = new List<Shape>();
                        m_panel.DataManager.StreetShapes[strSteetName] = shapesTrg;
                    }

                    if (shapesTrg != shapes)
                    {
                        shapesTrg.Clear();
                        shapesTrg.AddRange(shapes);
                    }

                    foreach (Shape shape in shapes)
                    {
                        m_panel.DataManager.ShapeStreets[shape] = strSteetName;
                    }
                }
            }
        }

        public void AddShape(Shape shape)
        {
            DataGridViewRow row;
            List<Shape> shapes = GetSelectedShapes(out row);

            if (shapes == null)
                return;

			if (shape == null)
				return;

            bool isChanged = false;

            if (shapes.Contains(shape))
            {
                shapes.Remove(shape);
                isChanged = SelectShape(shape, false);
            }
            else
            {
                shapes.Add(shape);
                isChanged = SelectShape(shape, true);
            }         

            if (isChanged)
                m_panel.DXFControl.Refresh();

            row.Cells[2].Value = shapes.Count;
            UpdateDataManager(shapes);
        }

        public void ClearShape()
        {
            DataGridViewRow row;
            List<Shape> shapes = GetSelectedShapes(out row);

            if (shapes == null)
                return;

            SelectShapes(shapes, false, true);
            row.Cells[2].Value = 0;
        }

        private void FormSettingStreetName_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_panel != null)
            {
                m_panel.SettingStreet = null;
                m_panel.Activity = m_prevActivityType;
            }
        }

        public new void Close()
        {
            base.Close();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

            if (m_rowSelected == row)
                return;

            if (m_rowSelected != null)
            {
                m_panel.ClearFixedSelection();
            }

            m_rowSelected = row;
            List<Shape> shapes = (List<Shape>)m_rowSelected.Tag;

			if (shapes != null)
			{
                if (Options.Instance.ZoomOnSelectStreet == true)
				{
					m_panel.DataManager.ObjectZoom(shapes, m_panel);
				}
				else
				{
					SelectShapes(shapes, true, false);
				}
				
			}

            m_panel.DXFControl.Refresh();
        }
    }
}
