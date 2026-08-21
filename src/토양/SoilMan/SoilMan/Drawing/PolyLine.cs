using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Geometry;

namespace SoilMan.Drawing
{
    public class PolyLine : BoundingShape
    {
        private List<System.Drawing.PointF[]> m_arrVertices = new List<System.Drawing.PointF[]>();
        
        public PolyLine()
        {
            // 선택되지 않도록 한다.
            Selectable = false;
        }

        public PolyLine(List<List<Vertex2F>> arrVertices, double dScale, Vertex2F vCenter)
        {
            m_arrVertices.Clear();

            foreach (List<Vertex2F> vertices in arrVertices)
            {
                int nVertexCount = vertices.Count();

                if (nVertexCount == 0)
                    continue;

                System.Drawing.PointF[] arrPoints = new System.Drawing.PointF[nVertexCount];
                m_arrVertices.Add(arrPoints);

                for (int i=0;i<nVertexCount;i++)
                {
                    Vertex2F vertex = vertices[i];
                    //Vertex2F vertex2 = ScaleTransfer(vertex.x, vertex.y, dScale, vCenter);

                    arrPoints[i].X = (float)vertex.x;
                    arrPoints[i].Y = (float)vertex.y;
                }
            }

            // 선택되지 않도록 한다.
            Selectable = false;
        }

        public void AddVertices(List<Vertex2F> vertices, double dScale, Vertex2F vCenter)
        {
            int nVertexCount = vertices.Count();

            if (nVertexCount == 0)
                return;

            System.Drawing.PointF[] points = new System.Drawing.PointF[nVertexCount];

            for (int i = 0; i < nVertexCount;i++ )
            {
                Vertex2F vertex = vertices[i];
                //Vertex2F vertex2 = ScaleTransfer(vertex.x, vertex.y, dScale, vCenter);

                points[i].X = (float)vertex.x;
                points[i].Y = (float)vertex.y;
            }

            m_arrVertices.Add(points);
        }

        public override Vertex2D Position
        {
            get { return new Vertex2D((m_dMinX + m_dMaxX) / 2, (m_dMinY + m_dMaxY) / 2); }
        }

        public override Vertex2D BoundaryTL
        {
            get { return new Vertex2D(m_dMinX, m_dMaxY); }
        }

        public override Vertex2D BoundaryBR
        {
            get { return new Vertex2D(m_dMaxX, m_dMinY); }
        }

        // (x,y)만큼 객체를 옮긴다.
        public override void Move(double x, double y)
        {           

            if (mbGenLevelPolyline == true)
            {
                foreach (List<System.Drawing.PointF[]> list in m_levelPolylines)
                {
                    for (int j = 0; j < list.Count; j++)
                    {
                        System.Drawing.PointF[] vertices = list[j];
                        int nVertexCount = vertices.Count();
                        for (int i = 0; i < nVertexCount; i++)
                        {
                            vertices[i].X += (float)x;
                            vertices[i].Y += (float)y;
                        }
                    }
                }
            }
            else
            {
                foreach (System.Drawing.PointF[] vertices in m_arrVertices)
                {
                    int nVertexCount = vertices.Count();

                    for (int i = 0; i < nVertexCount; i++)
                    {
                        vertices[i].X += (float)x;
                        vertices[i].Y += (float)y;
                    }
                }
            }   
        }

        private List<List<System.Drawing.PointF[]>> m_levelPolylines = new List<List<System.Drawing.PointF[]>>();
        private bool mbGenLevelPolyline = false;
        public void GenerateLevelPolyline()
        {
            for (int i = 1; i < 10; i++)
            {
                List<System.Drawing.PointF[]> levels2 = new List<System.Drawing.PointF[]>();
                foreach (System.Drawing.PointF[] vertices in m_arrVertices)
                {
                    List<System.Drawing.PointF> arTarget = new List<System.Drawing.PointF>(vertices);
                    List<System.Drawing.PointF> arResult = NativeGDI.NativeMethods.DouglasPeuckerReduction(arTarget, (10 - i) * 10.0);
                    System.Drawing.PointF[] ptLevel = arResult.ToArray();
                    levels2.Add(ptLevel);
                }
                m_levelPolylines.Add(levels2);
            }

            List<System.Drawing.PointF[]> levels = new List<System.Drawing.PointF[]>();
            foreach (System.Drawing.PointF[] vertices in m_arrVertices)
            {
                System.Drawing.PointF[] ptOrg = (System.Drawing.PointF[])vertices.Clone();
                levels.Add(ptOrg);
            }
            m_levelPolylines.Add(levels);

            mbGenLevelPolyline = true;
        }

        public override DXFViewer.Shape.ShapeType GetShapeType()
        {
            return DXFViewer.Shape.ShapeType.NONE;
        }

        public override DXFViewer.Shape Clone()
        {
            PolyLine polyLine = new PolyLine();

            foreach (System.Drawing.PointF[] vertices in m_arrVertices)
            {
                int nVertexCount = vertices.Count();
                if (nVertexCount == 0)
                    continue;

                System.Drawing.PointF[] vertices2 = new System.Drawing.PointF[nVertexCount];

                for (int i = 0; i < nVertexCount; i++)
                {
                    vertices2[i].X = vertices[i].X;
                    vertices2[i].Y = vertices[i].Y;
                }

                polyLine.m_arrVertices.Add(vertices2);
            }

            return polyLine;
        }

        // Selectable이 false이면 HitTest 검사가 무조건 실패한다.
        public override bool HitTest(double x, double y)
        {
            if (Selectable == false || m_attr == null)
                return false;

            if (x >= m_dMinX && x <= m_dMaxX && y >= m_dMinY && y <= m_dMaxY)
                return true;

            return false;
        }

        public override bool CheckClipBounds(System.Drawing.Graphics g, Vertex2D vClipTL, Vertex2D vClipBR)
        {
            DXFViewer.DXFControl ctrl = (DXFViewer.DXFControl)this.GetOwner();
            return CheckClipBounds(vClipTL - ctrl.MovedVertex, vClipBR - ctrl.MovedVertex, new Vertex2D(m_dMinX, m_dMaxY), new Vertex2D(m_dMaxX, m_dMinY));
        }

        public override bool Draw(System.Drawing.Graphics g, bool bDrawText)
        {
            if (m_attr == null)
                return false;

            double dSize = m_attr.GetPointSize();

            if (dSize <= 0.0)
                return false;

            ShapeLayer layer = (ShapeLayer)this.m_pOwnLayer;
            layer.LastDrawingShape = this;

            System.Drawing.Pen pen = m_lineType.GetPen();
            pen.Color = m_attr.GetLineColor();

            if (Selectable && Selected && m_selectedShowingType != DXFViewer.Shape.SelectedShowingType.NONE)
            {
                if (m_selectedShowingType == DXFViewer.Shape.SelectedShowingType.EDIT_BOX)
                {
                    DXFViewer.LineType lineType = m_pOwner.GetSelectedLineType();
                    System.Drawing.Color penColor = pen.Color;
                    pen = lineType.GetPen();
                    pen.Color = penColor;

                    DrawLines(g, pen, m_attr.GetLineThickness());

                    m_editBox.Draw(g, (float)m_dMinX, (float)m_dMinY);
                    m_editBox.Draw(g, (float)m_dMaxX, (float)m_dMaxY);
                }
                else if (m_selectedShowingType == DXFViewer.Shape.SelectedShowingType.BRIGHT_EFFECT)
                {
                    float fOldWidth = pen.Width;

                    DrawLines(g, pen, m_attr.GetLineThickness() + 1);

                    pen.Width = fOldWidth;

                    // 밝게 표현하기 위하여 배경색의 보색으로 그린다.
                    DrawLines(g, GetOwner().SelectedBrightPen1, m_attr.GetLineThickness());
                    // 패턴을 주기 위하여 배경색으로 다시한번 그린다.
                    DrawLines(g, GetOwner().SelectedBrightPen2, m_attr.GetLineThickness());
                }
            }
            else
                DrawLines(g, pen, m_attr.GetLineThickness());

            return true;
        }

        public override void PostDraw(System.Drawing.Graphics g)
        {
            ShapeLayer layer = (ShapeLayer)this.m_pOwnLayer;

            if (layer.PathLine != null)
            {
                System.Drawing.Pen pen = m_lineType.GetPen();
                SetPenWidth(pen, g, m_attr.GetLineThickness());

                g.DrawPath(pen, layer.PathLine);
                layer.PathLine = null;
            }

            GC.Collect();
        }

        private void DrawLines(System.Drawing.Graphics g, System.Drawing.Pen pen, int nLineThick)
        {
            if (nLineThick <= 0)
                return;

            //pen.Width = nLineThick;
            /*SetPenWidth(pen, g, nLineThick);

            foreach (System.Drawing.PointF[] vertices in m_arrVertices)
            {
                g.DrawLines(pen, vertices);
            }*/

            ShapeLayer layer = (ShapeLayer)this.m_pOwnLayer;

            if (layer.PathLine == null)
            //if (FirstElement)
            {
                layer.PathLine = new System.Drawing.Drawing2D.GraphicsPath();
            }


            if (mbGenLevelPolyline == true)
            {
                float fScale = System.Math.Abs(g.Transform.Elements[3]) * 1000.0f;               
                int n = (int)System.Math.Floor(fScale) - 1;
                if (n > 9)
                {
                    n = 9;
                }
                if (n < 0)
                {
                    n = 0;
                }

                List<System.Drawing.PointF[]> target = m_levelPolylines[n];
                foreach (System.Drawing.PointF[] vertices in target)
                {
                    if (vertices.Length >= 3)
                        layer.PathLine.AddLines(vertices);
                }
            }
            else
            {
                foreach (System.Drawing.PointF[] vertices in m_arrVertices)
                {
                    layer.PathLine.AddLines(vertices);
                }
            }            

            // PostDraw(...)에서 담당
            // CheckClipBounds() 호출로 인하여 가장 마지막에 그려질 객체가 누구인지 실시간으로 파악할 수 없으므로
            // 모든 Draw() 함수 호출 이후 마지막에 호출된 객체의 PostDraw()를 호출한다.
            /*if (LastElement)
            {
                SetPenWidth(pen, g, nLineThick);

                g.DrawPath(pen, layer.PathLine);
                layer.PathLine = null;
                GC.Collect();
            }*/
        }
    }
}
