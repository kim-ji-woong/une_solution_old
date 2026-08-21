using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnE.Geometry;
using DXFViewer;
using System.Drawing;

namespace VirtualSeoul
{
    public class Level : IComparable
    {
        private string m_strID = "";
        private int m_nFloorIndex = 0;
        // 층높이(cm)
        private int m_nHeight = 0;
        // 층고(cm)
        private int m_nElevation = 0;
        private List<POI> m_pois = new List<POI>();

        public string ID
        {
            get { return m_strID; }
            set { m_strID = value; }
        }

        public int FloorIndex
        {
            get { return m_nFloorIndex; }
            set { m_nFloorIndex = value; }
        }

        public int Height
        {
            get { return m_nHeight; }
            set { m_nHeight = value; }
        }

        public int Elevation
        {
            get { return m_nElevation; }
            set { m_nElevation = value; }
        }

        public string Name
        {
            get
            {
                if (m_nFloorIndex < 0)
                    return string.Format("지하 {0}층", -m_nFloorIndex);

                return string.Format("{0}층", m_nFloorIndex + 1);
            }
        }

        public List<POI> POIs
        {
            get { return m_pois; }
        }

        public override string ToString()
        {
            return Name;
        }

        public int CompareTo(object obj)
        {
            Level level = (Level)obj;
            return this.m_nElevation.CompareTo(level.m_nElevation);
        }
    }

    public class POIType
    {
        private string m_strCode = "";
        private string m_strName = "";

        private Vertex2D m_vTL = null;
        private Vertex2D m_vBL = null;
        private Vertex2D m_vBR = null;

        // 선형
        private LinkedPath m_path = new LinkedPath();
        // 채움
        private List<LinkedPath> m_listPolygons = new List<LinkedPath>();
        private List<TextData> m_listText = new List<TextData>();

        public string Code
        {
            get { return m_strCode; }
            set { m_strCode = value; }
        }

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        public Vertex2D TL
        {
            get { return m_vTL; }
        }

        public Vertex2D BL
        {
            get { return m_vBL; }
        }

        public Vertex2D BR
        {
            get { return m_vBR; }
        }

        public override string ToString()
        {
            return m_strName;
        }

        public POIType(string strName)
        {
            m_strName = strName;
        }

        public bool LoadPOI(string strPath, string strCode, Graphics g)
        {
            FileStream fs = new FileStream(strPath, FileMode.Open);
            BinaryReader reader = new BinaryReader(fs);

            m_strCode = strCode;
            reader.ReadString();
            
            if (ReadPath(m_path, reader) == false)
            {
                reader.Close();
                return false;
            }

            if (ReadPolygons(m_listPolygons, reader) == false)
            {
                reader.Close();
                return false;
            }

            if (ReadTextDatas(m_listText, reader, g) == false)
            {
                reader.Close();
                return false;
            }

            reader.Close();
            return true;
        }

        private bool ReadTextDatas(List<TextData> textDatas, BinaryReader reader, Graphics g)
        {
            try
            {
                int nTextCount = reader.ReadInt32();

                for (int i = 0; i < nTextCount; i++)
                {
                    string strText = reader.ReadString();
                    double x = reader.ReadDouble();
                    double y = reader.ReadDouble();
                    float fFontSize = (float)(reader.ReadSingle());
                    double dTextAngle = reader.ReadDouble();

                    TextData data = new TextData();

                    data.Text = strText;
                    data.Position = new Vertex2D(x, y);
                    data.FontSize = fFontSize;
                    data.TextAngle = dTextAngle;

                    textDatas.Add(data);

                    SizeF size = g.MeasureString(strText, data.GetFont());
                    SetBoundary(data.Position);
                    SetBoundary(new Vertex2D(x + size.Width, y - size.Height));
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private bool ReadPolygons(List<LinkedPath> polygons, BinaryReader reader)
        {
            int nPolygonCount = reader.ReadInt32();

            for (int i = 0; i < nPolygonCount; i++)
            {
                LinkedPath polygon = new LinkedPath();

                if (ReadPath(polygon, reader) == false)
                    return false;

                polygons.Add(polygon);
            }

            return true;
        }

        private bool ReadPath(LinkedPath path, BinaryReader reader)
        {
            int nPathCount = reader.ReadInt32();

            for (int i = 0; i < nPathCount; i++)
            {
                int nType = reader.ReadInt32();
                PathItem item = null;

                if (nType == (int)PathItem.DrawType.Line)
                    item = ReadLinePath(reader);
                else if (nType == (int)PathItem.DrawType.Arc)
                    item = ReadArcPath(reader);
                else if (nType == (int)PathItem.DrawType.EArc)
                    item = ReadEArcPath(reader);

                if (item == null)
                    return false;

                path.Path.Add(item);
            }

            return true;
        }

        private PathItem ReadEArcPath(BinaryReader reader)
        {
            try
            {
                double dTLX = reader.ReadDouble();
                double dTLY = reader.ReadDouble();
                double dBLX = reader.ReadDouble();
                double dBLY = reader.ReadDouble();
                double dBRX = reader.ReadDouble();
                double dBRY = reader.ReadDouble();
                double dBeginAngle = reader.ReadDouble();
                double dEArcAngle = reader.ReadDouble();
                bool isClockWise = reader.ReadBoolean();

                Vertex2D vTL = new Vertex2D(dTLX, dTLY);
                Vertex2D vBL = new Vertex2D(dBLX, dBLY);
                Vertex2D vBR = new Vertex2D(dBRX, dBRY);
                EArc2D earc = new EArc2D(vTL, vBL, vBR, dBeginAngle, dEArcAngle, isClockWise);

                SetBoundary(vTL);
                SetBoundary(vBL);
                SetBoundary(vBR);

                PathItem item = new PathItem();
                item.SetEArc(earc);
                return item;
            }
            catch (Exception)
            {
            }

            return null;
        }

        private PathItem ReadArcPath(BinaryReader reader)
        {
            try
            {
                double dCenterX = reader.ReadDouble();
                double dCenterY = reader.ReadDouble();
                double dRadius = reader.ReadDouble();
                double dBeginAngle = reader.ReadDouble();
                double dArcAngle = reader.ReadDouble();
                bool isClockWise = reader.ReadBoolean();

                Vertex2D vCenter = new Vertex2D(dCenterX, dCenterY);
                Arc2D arc = new Arc2D(vCenter, dRadius, dBeginAngle, dArcAngle, isClockWise);

                SetBoundary(arc.GetTL());
                SetBoundary(arc.GetBL());
                SetBoundary(arc.GetBR());

                PathItem item = new PathItem();
                item.SetArc(arc);
                return item;
            }
            catch (Exception)
            {
            }

            return null;
        }

        private PathItem ReadLinePath(BinaryReader reader)
        {
            try
            {
                double dBeginX = reader.ReadDouble();
                double dBeginY = reader.ReadDouble();
                double dEndX = reader.ReadDouble();
                double dEndY = reader.ReadDouble();

                Vertex2D vBegin = new Vertex2D(dBeginX, dBeginY);
                Vertex2D vEnd = new Vertex2D(dEndX, dEndY);

                SetBoundary(vBegin);
                SetBoundary(vEnd);

                PathItem item = new PathItem();
                item.SetLine(new Line2D(vBegin, vEnd));
                return item;
            }
            catch (Exception)
            {
            }

            return null;
        }

        private void SetBoundary(Vertex2D vertex)
        {
            if (m_vTL == null)
            {
                m_vTL = new Vertex2D(vertex);
                m_vBL = new Vertex2D(vertex);
                m_vBR = new Vertex2D(vertex);
            }
            else
            {
                if (m_vTL.x > vertex.x)
                    m_vTL.x = vertex.x;
                if (m_vBL.x > vertex.x)
                    m_vBL.x = vertex.x;
                if (m_vBR.x < vertex.x)
                    m_vBR.x = vertex.x;

                if (m_vTL.y < vertex.y)
                    m_vTL.y = vertex.y;
                if (m_vBL.y > vertex.y)
                    m_vBL.y = vertex.y;
                if (m_vBR.y > vertex.y)
                    m_vBR.y = vertex.y;
            }
        }

        public POI MakePOI(Vertex2D vPos)
        {
            POI poi = new POI();

            foreach (PathItem item in m_path.Path)
            {
                if (item.GetDrawType() == PathItem.DrawType.Line)
                {
                    Vertex2D vBegin = null, vEnd = null, vMiddle = null;

                    if (item.GetVertex(out vBegin, out vEnd, out vMiddle) == false)
                        continue;

                    DXFViewer.Line line = new Line(new Vertex2D(vBegin), new Vertex2D(vEnd));
                    line.Move(vPos.x, vPos.y);
                    poi.Shapes.Add(line);
                }
                else if (item.GetDrawType() == PathItem.DrawType.Arc)
                {
                    Arc2D arc = (Arc2D)item.GetEArc();

                    if (arc == null)
                        continue;

                    DXFViewer.Arc _arc = new DXFViewer.Arc();

                    _arc.Center = new Vertex2D(arc.GetCenter());
                    _arc.BeginAngle = UnE.Geometry.Math.RadToDeg(arc.GetBeginAngle());
                    _arc.ArcAngle = UnE.Geometry.Math.RadToDeg(arc.GetAngle());
                    _arc.Radius = arc.GetRadius();
                    
                    if (arc.IsClockWise() == false)
                    {
                        _arc.BeginAngle = UnE.Geometry.Math.RadToDeg(arc.GetBeginAngle());
                    }

                    _arc.IsCircle = arc.IsClosed();
                    _arc.Move(vPos.x, vPos.y);
                    poi.Shapes.Add(_arc);
                }
                else if (item.GetDrawType() == PathItem.DrawType.EArc)
                {
                    EArc2D earc = item.GetEArc();

                    if (earc == null)
                        continue;

                    DXFViewer.EArc _earc = new DXFViewer.EArc();

                    _earc.TopLeft = new Vertex2D(earc.GetTL());
                    _earc.BottomLeft = new Vertex2D(earc.GetBL());
                    _earc.BottomRight = new Vertex2D(earc.GetBR());
                    _earc.BeginAngle = UnE.Geometry.Math.RadToDeg(earc.GetBeginAngle());
                    _earc.EArcAngle = UnE.Geometry.Math.RadToDeg(earc.GetAngle());
                    _earc.Width = _earc.BottomLeft.GetDistance(_earc.BottomRight);
                    _earc.Height = _earc.BottomLeft.GetDistance(_earc.TopLeft);

                    if (earc.IsClockWise() == false)
                    {
                        _earc.BeginAngle = UnE.Geometry.Math.RadToDeg(earc.GetEndAngle());
                    }

                    _earc.IsEllipse = earc.IsClosed();
                    _earc.Move(vPos.x, vPos.y);
                    poi.Shapes.Add(_earc);
                }
            }

            foreach (LinkedPath path in m_listPolygons)
            {
                Hatch hatch = MakeHatch(path);
                hatch.Move(vPos.x, vPos.y);
                poi.Shapes.Add(hatch);
            }

            foreach (TextData data in m_listText)
            {
                Text text = new Text();
                text.Font = data.GetFont();
                text.Title = data.Text;
                text.SetPosition(vPos + data.Position);
                text.Angle = data.TextAngle;

                poi.Shapes.Add(text);
            }

            poi.POIType = this;
            return poi;
        }

        private Hatch MakeHatch(LinkedPath path)
        {
            Hatch hatch = new Hatch();

            foreach (PathItem item in path.Path)
            {
                if (item.GetDrawType() == PathItem.DrawType.Line)
                {
                    Vertex2D vBegin = null, vEnd = null, vMiddle = null;

                    if (item.GetVertex(out vBegin, out vEnd, out vMiddle) == false)
                        continue;

                    DXFViewer.Line line = new Line(new Vertex2D(vBegin), new Vertex2D(vEnd));
                    hatch.AddLine(vBegin, vEnd);
                }
                else if (item.GetDrawType() == PathItem.DrawType.Arc)
                {
                    Arc2D arc = (Arc2D)item.GetEArc();

                    if (arc == null)
                        continue;

                    Arc2D _arc = new Arc2D(new Vertex2D(arc.GetCenter()), arc.GetRadius(), arc.GetBeginAngle(), arc.GetAngle(), arc.IsClockWise());
                    hatch.AddArc(_arc);
                }
                else if (item.GetDrawType() == PathItem.DrawType.EArc)
                {
                    EArc2D earc = item.GetEArc();

                    if (earc == null)
                        continue;

                    EArc2D _earc = new EArc2D(new Vertex2D(earc.GetTL()), new Vertex2D(earc.GetBL()), new Vertex2D(earc.GetBR()), earc.GetBeginAngle(), earc.GetAngle(), earc.IsClockWise());
                    hatch.AddEArc(_earc);
                }
            }

            hatch.MakePath(0.0, 0.0);
            return hatch;
        }
    }

    public class LinkedPath
    {
        private List<PathItem> m_listPath = new List<PathItem>();
        private Vertex2D m_vTL = null;
        private Vertex2D m_vBR = null;

        public Vertex2D BoundaryTL
        {
            get { return m_vTL; }
        }

        public Vertex2D BoundaryBR
        {
            get { return m_vBR; }
        }

        public List<PathItem> Path
        {
            get { return m_listPath; }
        }

        public PathItem AddLine(Line2D line)
        {
            PathItem item = new PathItem();
            item.SetLine(line);
            m_listPath.Add(item);
            SetBoundary(ref m_vTL, ref m_vBR, line.GetVertex(true));
            SetBoundary(ref m_vTL, ref m_vBR, line.GetVertex(false));
            return item;
        }

        public PathItem AddLine(Vertex2D vBegin, Vertex2D vEnd)
        {
            PathItem item = new PathItem();
            item.SetLine(new Line2D(vBegin, vEnd));
            m_listPath.Add(item);
            SetBoundary(ref m_vTL, ref m_vBR, vBegin);
            SetBoundary(ref m_vTL, ref m_vBR, vEnd);
            return item;
        }

        public PathItem AddArc(Arc2D arc)
        {
            PathItem item = new PathItem();
            item.SetArc(arc);
            m_listPath.Add(item);
            SetBoundary(ref m_vTL, ref m_vBR, arc.GetTL());
            SetBoundary(ref m_vTL, ref m_vBR, arc.GetBR());
            return item;
        }

        public PathItem AddEArc(EArc2D earc)
        {
            PathItem item = new PathItem();
            item.SetEArc(earc);
            m_listPath.Add(item);
            SetBoundary(ref m_vTL, ref m_vBR, earc.GetTL());
            SetBoundary(ref m_vTL, ref m_vBR, earc.GetBR());
            return item;
        }

        public static void SetBoundary(ref Vertex2D vTL, ref Vertex2D vBR, Vertex2D vertex)
        {
            if (vTL == null)
            {
                vTL = new Vertex2D(vertex);
                vBR = new Vertex2D(vertex);
            }
            else
            {
                if (vTL.x > vertex.x)
                    vTL.x = vertex.x;
                if (vTL.y < vertex.y)
                    vTL.y = vertex.y;
                if (vBR.x < vertex.x)
                    vBR.x = vertex.x;
                if (vBR.y > vertex.y)
                    vBR.y = vertex.y;
            }
        }
    }

    public class TextData
    {
        private Vertex2D m_vPos = new Vertex2D();
        private Vertex2D m_vBoundaryTL = null;
        private Vertex2D m_vBoundaryBR = null;
        private string m_strText = "";
        private float m_fFontSize = 10;
        // Degree
        private double m_dTextAngle = 0.0;

        public Vertex2D Position
        {
            get { return m_vPos; }
            set { m_vPos = value; }
        }

        public string Text
        {
            get { return m_strText; }
            set { m_strText = value; }
        }

        public float FontSize
        {
            get { return m_fFontSize; }
            set { m_fFontSize = value; }
        }

        // Degree
        public double TextAngle
        {
            get { return m_dTextAngle; }
            set { m_dTextAngle = value; }
        }

        public Vertex2D BoundaryTL
        {
            get { return m_vBoundaryTL; }
            set { m_vBoundaryTL = value; }
        }

        public Vertex2D BoundaryBR
        {
            get { return m_vBoundaryBR; }
            set { m_vBoundaryBR = value; }
        }

        public Font GetFont()
        {
            return new Font("돋움", m_fFontSize);
        }

        public void Render(Graphics g, float x, float y, Color color, float fScaleY)
        {
            g.ScaleTransform(1.0f, -1.0f);
            y = -y;

            // 현재 Y축 Scale값을 가져온다.
            float x1 = fScaleY;
            //float x1 = g.Transform.Elements[3];
            // 폰트의 길이와 Y축의 곱이 실제 픽셀당 거리
            float h = x1 * m_fFontSize;

            // 1 픽셀미만이면 의미없으므로 Cutoff를 1로 한다.
            // 자간이 좁아지면 Graphics에서 예외가 발생하므로 작은값은 피한다.
            if (h > 1.0f || h < -1.0)
            {
                Font font = GetFont();//new Font("돋움", m_fFontSize);
                Brush brush = new SolidBrush(color);

                g.DrawString(m_strText, font, brush, x, y);
                //SizeF size = g.MeasureString(m_strText, font);
                //g.DrawString(m_strText, font, brush, x - size.Width / 2, y - size.Height / 2 + 13);

                brush.Dispose();
                font.Dispose();
            }

            g.ScaleTransform(1.0f, -1.0f);
        }
    }

    public class POI
    {
        private string m_strID = "";
        private Vertex2D m_vPos = null;
        private List<Shape> m_shapes = new List<Shape>();
        private Level m_level = null;
        private POIType m_poiType = null;
        private Vertex2D m_vTL = null;
        private Vertex2D m_vBL = null;
        private Vertex2D m_vBR = null;
        private string m_strName = "";
        private Vertex2D m_vFirstShapePosition = null;

        public string ID
        {
            get { return m_strID; }
            set { m_strID = value; }
        }

        public Vertex2D Position
        {
            get { return m_vPos; }
            set { m_vPos = value; }
        }

        public List<Shape> Shapes
        {
            get { return m_shapes; }
        }

        public Level Level
        {
            get { return m_level; }
            set { m_level = value; }
        }

        public POIType POIType
        {
            get { return m_poiType; }
            set { m_poiType = value; }
        }

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        public Vertex2D TL
        {
            get { return m_vTL; }
            set { m_vTL = value; }
        }

        public Vertex2D BL
        {
            get { return m_vBL; }
            set { m_vBL = value; }
        }

        public Vertex2D BR
        {
            get { return m_vBR; }
            set { m_vBR = value; }
        }

        public bool HitTest(Vertex2D vertex)
        {
            if (vertex.x >= m_vTL.x && vertex.x <= m_vBR.x &&
                vertex.y <= m_vTL.y && vertex.y >= m_vBR.y)
                return true;

            return false;
        }

        public void Move(Vertex2D vPos)
        {
            m_vPos = vPos;

            if (m_vFirstShapePosition != null && m_shapes.Count > 0)
            {
                Shape firstShape = m_shapes[0];
                Vertex2D vMove = m_vFirstShapePosition - (firstShape.Position - m_vPos);

                foreach (Shape shape in Shapes)
                {
                    shape.Move(vMove.x, vMove.y);
                }
            }

            TL = m_poiType.TL + m_vPos;
            BL = m_poiType.BL + m_vPos;
            BR = m_poiType.BR + m_vPos;
        }

        public void SetShapePosition()
        {
            if (m_shapes.Count == 0)
                return;

            Shape firstShape = m_shapes[0];
            m_vFirstShapePosition = firstShape.Position - m_vPos;
        }
    }
}