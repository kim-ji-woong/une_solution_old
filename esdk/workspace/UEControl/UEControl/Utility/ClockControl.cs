using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace UnE
{
    namespace GUI
    {
        public partial class ClockControl : UserControl
        {
            private Clock m_clock = new Clock();
            private bool m_useFixedTime = false;
            private int m_nFixedHour = 0;
            private int m_nFixedMinute = 0;

            public GUI.Clock Clock
            {
                get { return m_clock; }
            }

            public bool UseFixedTime
            {
                get { return m_useFixedTime; }
                set { m_useFixedTime = value; }
            }

            public int FixedHour
            {
                get { return m_nFixedHour; }
                set { m_nFixedMinute = value; }
            }

            public int FixedMinute
            {
                get { return m_nFixedMinute; }
                set { m_nFixedMinute = value; }
            }

            public int EdgeThick
            {
                get { return m_clock.EdgeThick; }
                set { m_clock.EdgeThick = value; }
            }

            public int FixedWidth
            {
                get { return m_clock.FixedWidth; }
                set { m_clock.FixedWidth = value; }
            }

            public int FixedHeight
            {
                get { return m_clock.FixedHeight; }
                set { m_clock.FixedHeight = value; }
            }

            public int LongMovementWidth
            {
                get { return m_clock.LongMovementWidth; }
                set { m_clock.LongMovementWidth = value; }
            }

            public int LongMovementHeight
            {
                get { return m_clock.LongMovementHeight; }
                set { m_clock.LongMovementHeight = value; }
            }

            public int ShortMovementWidth
            {
                get { return m_clock.ShortMovementWidth; }
                set { m_clock.ShortMovementWidth = value; }
            }

            public int ShortMovementHeight
            {
                get { return m_clock.ShortMovementHeight; }
                set { m_clock.ShortMovementHeight = value; }
            }

            public int CenterRectSize
            {
                get { return m_clock.CenterRectSize; }
                set { m_clock.CenterRectSize = value; }
            }

            public Color EdgeColor
            {
                get
                {
                    if (m_clock.EdgeBrush != null && m_clock.EdgeBrush.GetType() == typeof(SolidBrush))
                        return ((SolidBrush)m_clock.EdgeBrush).Color;

                    return Color.Transparent;
                }
                set
                {
                    if (m_clock.EdgeBrush != null && m_clock.EdgeBrush.GetType() == typeof(SolidBrush))
                        ((SolidBrush)m_clock.EdgeBrush).Color = value;
                    else if (value == Color.Transparent)
                        m_clock.EdgeBrush = null;
                    else
                    {
                        SolidBrush br = new SolidBrush(value);
                        m_clock.EdgeBrush = br;
                    }
                }
            }

            public Color FixedBrush
            {
                get
                {
                    if (m_clock.FixedBrush != null && m_clock.FixedBrush.GetType() == typeof(SolidBrush))
                        return ((SolidBrush)m_clock.FixedBrush).Color;

                    return Color.Transparent;
                }
                set
                {
                    if (m_clock.FixedBrush != null && m_clock.FixedBrush.GetType() == typeof(SolidBrush))
                        ((SolidBrush)m_clock.FixedBrush).Color = value;
                    else if (value == Color.Transparent)
                        m_clock.FixedBrush = null;
                    else
                    {
                        SolidBrush br = new SolidBrush(value);
                        m_clock.FixedBrush = br;
                    }
                }
            }

            public Color MovementBrush
            {
                get
                {
                    if (m_clock.MovementBrush != null && m_clock.MovementBrush.GetType() == typeof(SolidBrush))
                        return ((SolidBrush)m_clock.MovementBrush).Color;

                    return Color.Transparent;
                }
                set
                {
                    if (m_clock.MovementBrush != null && m_clock.MovementBrush.GetType() == typeof(SolidBrush))
                        ((SolidBrush)m_clock.MovementBrush).Color = value;
                    else if (value == Color.Transparent)
                        m_clock.MovementBrush = null;
                    else
                    {
                        SolidBrush br = new SolidBrush(value);
                        m_clock.MovementBrush = br;
                    }
                }
            }

            public Color CenterPointBrush
            {
                get
                {
                    if (m_clock.CenterPointBrush != null && m_clock.CenterPointBrush.GetType() == typeof(SolidBrush))
                        return ((SolidBrush)m_clock.CenterPointBrush).Color;

                    return Color.Transparent;
                }
                set
                {
                    if (m_clock.CenterPointBrush != null && m_clock.CenterPointBrush.GetType() == typeof(SolidBrush))
                        ((SolidBrush)m_clock.CenterPointBrush).Color = value;
                    else if (value == Color.Transparent)
                        m_clock.CenterPointBrush = null;
                    else
                    {
                        SolidBrush br = new SolidBrush(value);
                        m_clock.CenterPointBrush = br;
                    }
                }
            }

            public Color InnerBrush
            {
                get
                {
                    if (m_clock.InnerBrush != null && m_clock.InnerBrush.GetType() == typeof(SolidBrush))
                        return ((SolidBrush)m_clock.InnerBrush).Color;

                    return Color.Transparent;
                }
                set
                {
                    if (m_clock.InnerBrush != null && m_clock.InnerBrush.GetType() == typeof(SolidBrush))
                        ((SolidBrush)m_clock.InnerBrush).Color = value;
                    else if (value == Color.Transparent)
                        m_clock.InnerBrush = null;
                    else
                    {
                        SolidBrush br = new SolidBrush(value);
                        m_clock.InnerBrush = br;
                    }
                }
            }

            public bool Fixed1Visible
            {
                get { return m_clock.GetFixedVisible(1); }
                set { m_clock.SetFixedVisible(1, value); }
            }

            public bool Fixed2Visible
            {
                get { return m_clock.GetFixedVisible(2); }
                set { m_clock.SetFixedVisible(2, value); }
            }

            public bool Fixed3Visible
            {
                get { return m_clock.GetFixedVisible(3); }
                set { m_clock.SetFixedVisible(3, value); }
            }

            public bool Fixed4Visible
            {
                get { return m_clock.GetFixedVisible(4); }
                set { m_clock.SetFixedVisible(4, value); }
            }

            public bool Fixed5Visible
            {
                get { return m_clock.GetFixedVisible(5); }
                set { m_clock.SetFixedVisible(5, value); }
            }

            public bool Fixed6Visible
            {
                get { return m_clock.GetFixedVisible(6); }
                set { m_clock.SetFixedVisible(6, value); }
            }

            public bool Fixed7Visible
            {
                get { return m_clock.GetFixedVisible(7); }
                set { m_clock.SetFixedVisible(7, value); }
            }

            public bool Fixed8Visible
            {
                get { return m_clock.GetFixedVisible(8); }
                set { m_clock.SetFixedVisible(8, value); }
            }

            public bool Fixed9Visible
            {
                get { return m_clock.GetFixedVisible(9); }
                set { m_clock.SetFixedVisible(9, value); }
            }

            public bool Fixed10Visible
            {
                get { return m_clock.GetFixedVisible(10); }
                set { m_clock.SetFixedVisible(10, value); }
            }

            public bool Fixed11Visible
            {
                get { return m_clock.GetFixedVisible(11); }
                set { m_clock.SetFixedVisible(11, value); }
            }

            public bool Fixed12Visible
            {
                get { return m_clock.GetFixedVisible(0); }
                set { m_clock.SetFixedVisible(0, value); }
            }

            public ClockControl()
            {
                InitializeComponent();
                SetClockSize();
            }

            private void SetClockSize()
            {
                if (m_clock != null)
                {
                    if (this.Size.Width < this.Size.Height)
                        m_clock.RectSize = this.Size.Width;
                    else
                        m_clock.RectSize = this.Size.Height;
                }
            }

            private void ClockControl_Paint(object sender, PaintEventArgs e)
            {
                if (m_clock != null)
                {
                    if (m_useFixedTime)
                        m_clock.Draw(e.Graphics, m_nFixedHour, m_nFixedMinute);
                    else
                    {
                        DateTime dtNow = DateTime.Now;
                        m_clock.Draw(e.Graphics, dtNow.Hour, dtNow.Minute);
                    }
                }
            }

            private void ClockControl_Resize(object sender, EventArgs e)
            {
                SetClockSize();
            }
        }

        public class Clock
        {
            private int m_nEdgeThick = 3;
            private int m_nRectSize = 60;
            private int m_nBeginX = 0;
            private int m_nBeginY = 0;
            private int m_nFixedWidth = 2;
            private int m_nFixedHeight = 6;
            private int m_nLongMovementWidth = 2;
            private int m_nLongMovementHeight = 20;
            private int m_nShortMovementWidth = 2;
            private int m_nShortMovementHeight = 12;
            private int m_nCenterRectSize = 4;

            private Brush m_brEdge = new SolidBrush(Color.White);
            private Brush m_brFixed = new SolidBrush(Color.White);
            private Brush m_brMovement = new SolidBrush(Color.White);
            private Brush m_brCenterPoint = new SolidBrush(Color.White);
            private Brush m_brInner = null;

            private bool[] m_arrShowFixed = new bool[12] { true, true, true, true, true, true, true, true, true, true, true, true };

            public int EdgeThick
            {
                get { return m_nEdgeThick; }
                set { m_nEdgeThick = value; }
            }

            public int RectSize
            {
                get { return m_nRectSize; }
                set { m_nRectSize = value; }
            }

            public int X
            {
                get { return m_nBeginX; }
                set { m_nBeginX = value; }
            }

            public int Y
            {
                get { return m_nBeginY; }
                set { m_nBeginY = value; }
            }

            public int FixedWidth
            {
                get { return m_nFixedWidth; }
                set { m_nFixedWidth = value; }
            }

            public int FixedHeight
            {
                get { return m_nFixedHeight; }
                set { m_nFixedHeight = value; }
            }

            public int LongMovementWidth
            {
                get { return m_nLongMovementWidth; }
                set { m_nLongMovementWidth = value; }
            }

            public int LongMovementHeight
            {
                get { return m_nLongMovementHeight; }
                set { m_nLongMovementHeight = value; }
            }

            public int ShortMovementWidth
            {
                get { return m_nShortMovementWidth; }
                set { m_nShortMovementWidth = value; }
            }

            public int ShortMovementHeight
            {
                get { return m_nShortMovementHeight; }
                set { m_nShortMovementHeight = value; }
            }

            public int CenterRectSize
            {
                get { return m_nCenterRectSize; }
                set { m_nCenterRectSize = value; }
            }

            public Brush EdgeBrush
            {
                get { return m_brEdge; }
                set { m_brEdge = value; }
            }

            public Brush FixedBrush
            {
                get { return m_brFixed; }
                set { m_brFixed = value; }
            }

            public Brush MovementBrush
            {
                get { return m_brMovement; }
                set { m_brMovement = value; }
            }

            public Brush CenterPointBrush
            {
                get { return m_brCenterPoint; }
                set { m_brCenterPoint = value; }
            }

            public Brush InnerBrush
            {
                get { return m_brInner; }
                set { m_brInner = value; }
            }

            // nIndex : 0에서 11 사이
            public void SetFixedVisible(int nIndex, bool visible)
            {
                if (nIndex < 0 || nIndex > 11)
                    return;

                m_arrShowFixed[nIndex] = visible;
            }

            // nIndex : 0에서 11 사이
            public bool GetFixedVisible(int nIndex)
            {
                if (nIndex < 0 || nIndex > 11)
                    return false;

                return m_arrShowFixed[nIndex];
            }

            public void Draw(Graphics g, int hour, int min)
            {
                System.Drawing.Drawing2D.SmoothingMode mode = g.SmoothingMode;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                DrawClockEdge(g);
                DrawClockFixed(g);
                DrawMovement(g, hour, min);

                g.SmoothingMode = mode;
            }

            private void DrawClockEdge(Graphics g)
            {
                if (m_nRectSize <= 0 || m_brEdge == null || m_nEdgeThick <= 0)
                    return;

                Rectangle rectBig = new Rectangle(X, Y, m_nRectSize, m_nRectSize);
                Rectangle rectSmall = new Rectangle(X + m_nEdgeThick, Y + m_nEdgeThick, m_nRectSize - m_nEdgeThick * 2, m_nRectSize - m_nEdgeThick * 2);

                if (m_brInner == null)
                {
                    GraphicsPath path = new GraphicsPath();

                    path.AddEllipse(rectBig);
                    path.AddEllipse(rectSmall);

                    g.FillPath(m_brEdge, path);
                }
                else
                {
                    g.FillEllipse(m_brEdge, rectBig);
                    g.FillEllipse(m_brInner, rectSmall);
                }
            }

            private void DrawClockFixed(Graphics g)
            {
                if (m_brFixed == null)
                    return;

                int w = m_nFixedWidth;
                int h = m_nFixedHeight;
                int r = m_nRectSize / 2;
                int cx = X + r;
                int cy = Y + r;

                if (w < 2)
                    w = 2;

                UnE.Geometry.Vertex2D vCenter = new UnE.Geometry.Vertex2D(cx, cy);
                UnE.Geometry.Vertex2D vertex = new UnE.Geometry.Vertex2D();

                PointF[] arrPt = new PointF[4] { new PointF(), new PointF(), new PointF(), new PointF() };

                for (int i = 0; i < 12; i++)
                {
                    if (!m_arrShowFixed[i])
                        continue;

                    double angle = System.Math.PI / 6 * i;
                    vertex.x = cx + System.Math.Sin(angle) * r;
                    vertex.y = cy - System.Math.Cos(angle) * r;

                    vertex = UnE.Geometry.Math.GetLinearVertex(vertex, vCenter, m_nEdgeThick / 2);
                    UnE.Geometry.Vertex2D v1 = UnE.Geometry.Math.GetRightVertex(vertex, vCenter, w / 2);
                    UnE.Geometry.Vertex2D v2 = vertex * 2 - v1;

                    arrPt[0].X = (float)v1.x;
                    arrPt[0].Y = (float)v1.y;
                    arrPt[1].X = (float)v2.x;
                    arrPt[1].Y = (float)v2.y;

                    UnE.Geometry.Vertex2D vTemp = UnE.Geometry.Math.GetLinearVertex(vertex, vCenter, h);
                    UnE.Geometry.Vertex2D v3 = UnE.Geometry.Math.GetRightVertex(vTemp, vertex, w / 2);

                    arrPt[2].X = (float)v3.x;
                    arrPt[2].Y = (float)v3.y;
                    arrPt[3].X = arrPt[0].X - arrPt[1].X + arrPt[2].X;
                    arrPt[3].Y = arrPt[0].Y - arrPt[1].Y + arrPt[2].Y;

                    g.FillPolygon(m_brFixed, arrPt);
                }
            }

            private void DrawMovement(Graphics g, int hour, int min)
            {
                int r = m_nRectSize / 2;

                int cx = X + r;
                int cy = Y + r;

                UnE.Geometry.Vertex2D vCenter = new UnE.Geometry.Vertex2D(cx, cy);

                DrawBar(g, vCenter, m_nLongMovementHeight, m_nLongMovementWidth, System.Math.PI * min / 30, m_brMovement);
                DrawBar(g, vCenter, m_nShortMovementHeight, m_nShortMovementWidth, System.Math.PI * ((hour % 12) * 60 + min) / 360, m_brMovement);

                if (m_nCenterRectSize > 0 && m_brCenterPoint != null)
                {
                    // 시계 중심점
                    Rectangle rectCenter = new Rectangle(cx - m_nCenterRectSize / 2, cy - m_nCenterRectSize / 2, m_nCenterRectSize, m_nCenterRectSize);
                    g.FillEllipse(m_brCenterPoint, rectCenter);
                }
            }

            private void DrawBar(Graphics g, UnE.Geometry.Vertex2D vCenter, int h, int w, double angle, Brush br)
            {
                if (br == null)
                    return;

                if (w < 2)
                    w = 2;

                double x = vCenter.x + System.Math.Sin(angle) * h;
                double y = vCenter.y - System.Math.Cos(angle) * h;
                UnE.Geometry.Vertex2D vertex = new UnE.Geometry.Vertex2D(x, y);

                UnE.Geometry.Vertex2D v1 = UnE.Geometry.Math.GetRightVertex(vertex, vCenter, w / 2);
                UnE.Geometry.Vertex2D v2 = vertex * 2 - v1;
                UnE.Geometry.Vertex2D v3 = UnE.Geometry.Math.GetRightVertex(vCenter, vertex, w / 2);

                PointF[] arrPt = new PointF[4];

                arrPt[0] = new PointF((float)v1.x, (float)v1.y);
                arrPt[1] = new PointF((float)v2.x, (float)v2.y);
                arrPt[2] = new PointF((float)v3.x, (float)v3.y);
                arrPt[3] = new PointF(arrPt[0].X - arrPt[1].X + arrPt[2].X, arrPt[0].Y - arrPt[1].Y + arrPt[2].Y);

                g.FillPolygon(br, arrPt);
            }
        }
    }
}
