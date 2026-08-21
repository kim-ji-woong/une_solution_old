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

namespace CadToXML
{
    public partial class FormWallLine : Form
    {
        private Floor m_floor = null;
        private Layer m_layer = null;
        private int m_nWallIndex = -1;

        public FormWallLine(Floor floor)
        {
            InitializeComponent();
            this.Text = floor.Name;
            m_floor = floor;

            if (m_floor == null || m_floor.Walls.Count == 0)
            {
                labelWallCount.Text = "";
                labelWallIndex.Text = "";
                btnPrev.Enabled = btnNext.Enabled = false;
            }
            else
            {
                m_nWallIndex = 0;
                labelWallCount.Text = m_floor.Walls.Count.ToString() + "개";
                labelWallIndex.Text = (m_nWallIndex + 1).ToString();
                CheckEnable();

                Layer layer = new Layer(dxfControl);
                dxfControl.Layers.Add(layer);
                m_layer = layer;

                Wall wall = m_floor.Walls[m_nWallIndex];
                AddLine(wall);

                Vertex2D vCenter = (wall.Begin + wall.End) / 2;
                dxfControl.MoveAll(-vCenter.x, -vCenter.y);
            }
        }

        private void AddLine(Wall wall)
        {
            if (m_layer != null)
            {
                //Line prev = GetPrevLine();
                //Line first = GetFirstLine();

                Line line = new Line(wall.Begin + dxfControl.MovedVertex, wall.End + dxfControl.MovedVertex);
                CheckIntersect(line);
                m_layer.Add(line);

                //CheckIntersect(line, prev, first);
                SetWallVertex();
            }
        }

        private Line GetPrevLine()
        {
            int nShapeCount = m_layer.Shapes.Count;

            if (nShapeCount == 0)
                return null;

            for (int i=nShapeCount-1;i>=0;i--)
            {
                Shape shape = (Shape)m_layer.Shapes[i];

                if (shape is Line)
                    return (Line)shape;
            }

            return null;
        }

        private Line GetFirstLine()
        {
            int nShapeCount = m_layer.Shapes.Count;

            if (nShapeCount == 0)
                return null;

            for (int i = 0; i < nShapeCount; i++)
            {
                Shape shape = (Shape)m_layer.Shapes[i];

                if (shape is Line)
                    return (Line)shape;
            }

            return null;
        }

        private void CheckIntersect(Line line)
        {
            List<Vertex2D> addCircles = new List<Vertex2D>();

            foreach (Shape shape in m_layer.Shapes)
            {
                if (shape is Line)
                {
                    Vertex2D vertex = GetIntersect(line, (Line)shape);

                    if (vertex != null)
                    {
                        addCircles.Add(vertex);
                    }
                }
            }

            foreach (Vertex2D vertex in addCircles)
            {
                AddCircle(vertex);
            }
        }

        private void CheckIntersect(Line line, Line prev, Line first)
        {
            if (prev == null)
                return;

            Vertex2D vertex = GetIntersect(line, prev);

            if (vertex != null)
            {
                AddCircle(vertex);
            }

            if (prev == first)
                return;

            vertex = GetIntersect(line, first);

            if (vertex != null)
            {
                AddCircle(vertex);
            }
        }

        private void AddCircle(Vertex2D vCenter)
        {
            Arc arc = new Arc();
            arc.Center = vCenter;
            arc.Radius = 300.0;
            arc.IsCircle = true;
            arc.SetOwnColor(Color.Red);
            arc.SetColorOption(Shape.ControlType.BYOWN);

            m_layer.Add(arc);
        }

        private Vertex2D GetIntersect(Line line1, Line line2)
        {
            double dTolerance = 0.001;

            if (line1.Begin.GetDistance(line2.End) <= dTolerance)
                return line1.Begin;

            if (line1.Begin.GetDistance(line2.Begin) <= dTolerance)
                return line1.Begin;

            if (line1.End.GetDistance(line2.End) <= dTolerance)
                return line1.End;

            if (line1.End.GetDistance(line2.Begin) <= dTolerance)
                return line1.End;

            return null;
        }

        private void checkBoxContinue_CheckedChanged(object sender, EventArgs e)
        {
            if (m_layer == null)
                return;

            m_layer.RemoveAll();

            if (checkBoxContinue.Checked)
            {
                for (int i=0;i<=m_nWallIndex;i++)
                {
                    AddLine(m_floor.Walls[i]);
                }
            }
            else
            {
                AddLine(m_floor.Walls[m_nWallIndex]);
            }

            dxfControl._Refresh();
        }

        private void RemoveLine(Wall wall)
        {
            double dTolerance = 0.001;
            Line line = null;

            Vertex2D vWallBegin = wall.Begin + dxfControl.MovedVertex;
            Vertex2D vWallEnd = wall.End + dxfControl.MovedVertex;

            foreach (Shape shape in m_layer.Shapes)
            {
                if (shape is Line)
                {
                    Line _line = (Line)shape;

                    if ((_line.Begin.GetDistance(vWallBegin) < dTolerance && _line.End.GetDistance(vWallEnd) < dTolerance) ||
                        (_line.End.GetDistance(vWallBegin) < dTolerance && _line.Begin.GetDistance(vWallEnd) < dTolerance))
                    {
                        line = _line;
                        break;
                    }
                }
            }

            if (line == null)
                return;

            List<Arc> removes = new List<Arc>();

            foreach (Shape shape in m_layer.Shapes)
            {
                if (shape is Arc)
                {
                    Arc arc = (Arc)shape;

                    if (arc.Center.GetDistance(line.Begin) < dTolerance || arc.Center.GetDistance(line.End) < dTolerance)
                    {
                        removes.Add(arc);

                        if (removes.Count >= 2)
                            break;
                    }
                }
            }

            foreach (Arc arc in removes)
            {
                m_layer.Remove(arc);
            }

            m_layer.Remove(line);
            SetWallVertex();
        }

        private void CheckEnable()
        {
            btnPrev.Enabled = m_nWallIndex > 0;
            btnNext.Enabled = m_nWallIndex < m_floor.Walls.Count - 1;
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (checkBoxContinue.Checked)
                RemoveLine(m_floor.Walls[m_nWallIndex--]);
            else
            {
                m_layer.RemoveAll();
                AddLine(m_floor.Walls[--m_nWallIndex]);
            }

            labelWallIndex.Text = (m_nWallIndex + 1).ToString();

            dxfControl._Refresh();
            CheckEnable();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (checkBoxContinue.Checked)
            {
                AddLine(m_floor.Walls[++m_nWallIndex]);
            }
            else
            {
                m_layer.RemoveAll();
                AddLine(m_floor.Walls[++m_nWallIndex]);
            }

            labelWallIndex.Text = (m_nWallIndex + 1).ToString();

            dxfControl._Refresh();
            CheckEnable();
        }

        private void SetWallVertex()
        {
            if (m_layer.Shapes.Count > 1)
            {
                string str = GetWallVertex(m_floor.Walls[m_nWallIndex - 1], m_nWallIndex);

                str += "\r\n\r\n";
                str += GetWallVertex(m_floor.Walls[m_nWallIndex], m_nWallIndex + 1);

                textBoxWallVertex.Text = str;
            }
        }

        private string GetWallVertex(Wall wall, int nIndex)
        {
            Vertex2D vWallBegin = wall.Begin;// + dxfControl.MovedVertex;
            Vertex2D vWallEnd = wall.End;// + dxfControl.MovedVertex;

            string str = string.Format("[Wall_{0}]\r\nBegin : {1:F1}, {2:F1}\r\nEnd : {3:F1}, {4:F1}", nIndex, vWallBegin.x, vWallBegin.y, vWallEnd.x, vWallEnd.y);
            return str;
        }

        private void btnToEnd_Click(object sender, EventArgs e)
        {
            while (btnNext.Enabled)
            {
                btnNext_Click(btnNext, null);
            }
        }
    }
}
