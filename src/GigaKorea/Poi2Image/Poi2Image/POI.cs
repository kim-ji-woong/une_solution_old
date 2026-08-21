using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnE.Geometry;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Poi2Image
{
    public class POI
    {
        private class Icon
        {
            private List<GraphicsPath> m_edgePath = null;
            private List<GraphicsPath> m_fillPath = null;
            private List<TextData> m_textDatas = null;

            private Vertex2D m_vTL = null;
            private Vertex2D m_vBR = null;
            private Vertex2D m_vPos = null;
            private int m_nTextMoveX = 0;
            private int m_nTextMoveY = 0;

            public Vertex2D BoundaryTL
            {
                get { return m_vTL; }
                set { m_vTL = value; }
            }

            public Vertex2D BoundaryBR
            {
                get { return m_vBR; }
                set { m_vBR = value; }
            }

            public double Width
            {
                get { return m_vBR.x - m_vTL.x; }
            }

            public double Height
            {
                get { return m_vTL.y - m_vBR.y; }
            }

            public List<GraphicsPath> EdgePath
            {
                get { return m_edgePath; }
                set { m_edgePath = value; }
            }

            public List<GraphicsPath> FillPath
            {
                get { return m_fillPath; }
                set { m_fillPath = value; }
            }

            public List<TextData> TextDatas
            {
                get { return m_textDatas; }
                set { m_textDatas = value; }
            }

            public int TextMoveX
            {
                get { return m_nTextMoveX; }
                set { m_nTextMoveX = value; }
            }

            public int TextMoveY
            {
                get { return m_nTextMoveY; }
                set { m_nTextMoveY = value; }
            }

            public void Render(Graphics g, Pen pen, Brush brush, double x, double y, double dScale)
            {
                //g.ScaleTransform((float)dScale, (float)dScale);
                double dMoveX = x - m_vPos.x;
                double dMoveY = y - m_vPos.y;
                bool needTranslate = System.Math.Abs(dMoveX) > UnE.Geometry.Math.HALF_TOLERANCE() || System.Math.Abs(dMoveY) > UnE.Geometry.Math.HALF_TOLERANCE();

                if (needTranslate)
                    g.TranslateTransform((float)(x - m_vPos.x), (float)(y - m_vPos.y));

                if (brush != null && m_fillPath != null)
                {
                    foreach (GraphicsPath path in m_fillPath)
                    {
                        g.FillPath(brush, path);
                    }
                }

                if (pen != null && m_edgePath != null)
                {
                    foreach (GraphicsPath path in m_edgePath)
                    {
                        g.DrawPath(pen, path);
                    }
                }

                if (pen != null)
                {
                    foreach (TextData text in m_textDatas)
                    {
                        if (text.BoundaryTL == null)
                            SetTextBoundary(g, text);

                        text.Render(g, (float)text.Position.x + m_nTextMoveX, (float)text.Position.y + m_nTextMoveY, pen.Color);
                        //text.Render(g, (float)m_vPos.x, (float)m_vPos.y, pen.Color);
                    }
                }

                if (needTranslate)
                    g.TranslateTransform((float)(-dMoveX), (float)(-dMoveY));
            }

            public void GetTextBoundary(ref Vertex2D vTL, ref Vertex2D vBR)
            {
                Bitmap bitmap = new Bitmap(100, 100);
                Graphics g = Graphics.FromImage(bitmap);

                foreach (TextData text in TextDatas)
                {
                    Font font = text.GetFont();
                    SizeF size = g.MeasureString(text.Text, font);
                    font.Dispose();

                    double tlX = text.Position.x - size.Width / 2;
                    double tlY = text.Position.y + size.Height / 2;
                    double brX = text.Position.x + size.Width / 2;
                    double brY = text.Position.y - size.Height / 2;

                    if (vTL == null)
                    {
                        vTL = new Vertex2D(tlX, tlY);
                        vBR = new Vertex2D(brX, brY);
                    }
                    else
                    {
                        if (vTL.x > tlX)
                            vTL.x = tlX;
                        if (vTL.y < tlY)
                            vTL.y = tlY;
                        if (vBR.x < brX)
                            vBR.x = brX;
                        if (vBR.y > brY)
                            vBR.y = brY;
                    }

                    text.BoundaryTL = new Vertex2D(tlX, tlY);
                    text.BoundaryBR = new Vertex2D(brX, brY);
                }
            }

            private void SetTextBoundary(Graphics g, TextData text)
            {
                Font font = text.GetFont();
                SizeF size = g.MeasureString(text.Text, font);
                font.Dispose();

                double tlX = text.Position.x - size.Width / 2;
                double tlY = text.Position.y + size.Height / 2;
                double brX = text.Position.x + size.Width / 2;
                double brY = text.Position.y - size.Height / 2;

                if (m_vTL.x > tlX)
                    m_vTL.x = tlX;
                if (m_vTL.y < tlY)
                    m_vTL.y = tlY;
                if (m_vBR.x < brX)
                    m_vBR.x = brX;
                if (m_vBR.y > brY)
                    m_vBR.y = brY;

                text.BoundaryTL = new Vertex2D(tlX, tlY);
                text.BoundaryBR = new Vertex2D(brX, brY);
            }

            public bool HitTest(double x, double y)
            {
                if (x >= m_vTL.x && x <= m_vBR.x &&
                    y <= m_vTL.y && y >= m_vBR.y)
                    return true;

                return false;
            }

            public void SetPosition(double x, double y)
            {
                if (m_vPos == null)
                    m_vPos = new Vertex2D(x, y);
                else
                    m_vPos.SetVertex(x, y);
            }
        }

        private string m_strName = "";
        // 선형
        private LinkedPath m_path = new LinkedPath();
        // 채움
        private List<LinkedPath> m_listPolygons = new List<LinkedPath>();
        private List<TextData> m_listText = new List<TextData>();

        private Icon m_icon = new Icon();

        private int m_nTextMoveX = 0;
        private int m_nTextMoveY = 0;

        public int TextMoveX
        {
            get { return m_nTextMoveX; }
            set
            {
                m_nTextMoveX = value;

                if (m_icon != null)
                    m_icon.TextMoveX = m_nTextMoveX;
            }
        }

        public int TextMoveY
        {
            get { return m_nTextMoveY; }
            set
            {
                m_nTextMoveY = value;

                if (m_icon != null)
                    m_icon.TextMoveY = m_nTextMoveY;
            }
        }

        public Size Size
        {
            get
            {
                int x = 0, y = 0;
                double width = m_icon.BoundaryBR.x - m_icon.BoundaryTL.x;
                double height = m_icon.BoundaryTL.y - m_icon.BoundaryBR.y;
                return new Size((int)width + x, (int)height + y);
            }
        }

        public Vertex2D TL
        {
            get { return m_icon.BoundaryTL; }
        }

        public Vertex2D BR
        {
            get { return m_icon.BoundaryBR; }
        }

        public static POI FromFile(string strPath, double dScale = 1.0)
        {
            FileStream fs = new FileStream(strPath, FileMode.Open);
            BinaryReader reader = new BinaryReader(fs);

            POI poi = new POI();

            string strPOIName = reader.ReadString();
            poi.m_strName = strPOIName;

            if (ReadPath(poi.m_path, reader, dScale) == false)
                return null;

            if (ReadPolygons(poi.m_listPolygons, reader, dScale) == false)
                return null;

            if (ReadTextDatas(poi.m_listText, reader, dScale) == false)
                return null;

            reader.Close();

            poi.Move(0, 0);

            return poi;
        }

        public void SaveFile(string strPath)
        {
            FileStream fs = new FileStream(strPath, FileMode.Create);
            BinaryWriter writer = new BinaryWriter(fs);

            writer.Write(m_strName);

            if (WriteBoundary(m_path, writer) == false)
                return;

            if (WriteFill(writer) == false)
                return;

            if (WriteText(writer) == false)
                return;

            writer.Close();
        }

        private bool WriteText(BinaryWriter writer)
        {
            int nTextCount = m_listText.Count;
            writer.Write(nTextCount);

            for (int i = 0; i < nTextCount; i++)
            {
                TextData text = m_listText[i];

                writer.Write(text.Text);
                writer.Write(text.Position.x + m_nTextMoveX);
                writer.Write(text.Position.y + m_nTextMoveY);
                writer.Write(text.FontSize);
                writer.Write(text.TextAngle);
            }

            return true;
        }

        private bool WriteFill(BinaryWriter writer)
        {
            int nPolygonCount = m_listPolygons.Count;
            writer.Write(nPolygonCount);

            for (int i = 0; i < nPolygonCount; i++)
            {
                LinkedPath polygon = m_listPolygons[i];

                if (!WriteBoundary(polygon, writer))
                    return false;
            }

            return true;
        }

        private bool WriteBoundary(LinkedPath path, BinaryWriter writer)
        {
            int nPathCount = path.Path.Count;
            writer.Write(nPathCount);

            for (int i = 0; i < nPathCount; i++)
            {
                PathItem item = path.Path[i];

                if (item.GetDrawType() == PathItem.DrawType.Line)
                {
                    if (item.Line == null)
                        return false;

                    Vertex2D vBegin = item.Line.GetVertex(true);
                    Vertex2D vEnd = item.Line.GetVertex(false);

                    writer.Write((int)item.GetDrawType());
                    writer.Write(vBegin.x);
                    writer.Write(vBegin.y);
                    writer.Write(vEnd.x);
                    writer.Write(vEnd.y);
                }
                else if (item.GetDrawType() == PathItem.DrawType.Arc)
                {
                    if (item.Arc == null)
                        return false;

                    Vertex2D vCenter = item.Arc.GetCenter();

                    writer.Write((int)item.GetDrawType());
                    writer.Write(vCenter.x);
                    writer.Write(vCenter.y);
                    writer.Write(item.Arc.GetRadius());
                    writer.Write(item.Arc.GetBeginAngle());
                    writer.Write(item.Arc.GetAngle());
                    writer.Write(item.Arc.IsClockWise());
                }
                else if (item.GetDrawType() == PathItem.DrawType.EArc)
                {
                    if (item.EArc == null)
                        return false;

                    Vertex2D vTL = item.EArc.GetTL();
                    Vertex2D vBL = item.EArc.GetBL();
                    Vertex2D vBR = item.EArc.GetBR();

                    writer.Write((int)item.GetDrawType());
                    writer.Write(vTL.x);
                    writer.Write(vTL.y);
                    writer.Write(vBL.x);
                    writer.Write(vBL.y);
                    writer.Write(vBR.x);
                    writer.Write(vBR.y);
                    writer.Write(item.EArc.GetBeginAngle());
                    writer.Write(item.EArc.GetAngle());
                    writer.Write(item.EArc.IsClockWise());
                }
            }

            return true;
        }

        private void Move(double dMoveX, double dMoveY)
        {
            m_icon.SetPosition(dMoveX, dMoveY);

            Vertex2D vTL = null, vBR = null;

            m_icon.EdgePath = MakePath(dMoveX, dMoveY, ref vTL, ref vBR);
            m_icon.FillPath = MakePolygons(dMoveX, dMoveY, ref vTL, ref vBR);
            m_icon.TextDatas = m_listText;

            if (vTL == null || (vBR.x - vTL.x < 1.0) || (vTL.y - vBR.y < 1.0))
            {
                if (m_icon.TextDatas.Count > 0)
                    m_icon.GetTextBoundary(ref vTL, ref vBR);
            }

            m_icon.BoundaryTL = vTL;
            m_icon.BoundaryBR = vBR;
        }

        private List<GraphicsPath> MakePolygons(double x, double y, ref Vertex2D vTL, ref Vertex2D vBR)
        {
            List<GraphicsPath> paths = new List<GraphicsPath>();

            foreach (LinkedPath polygon in m_listPolygons)
            {
                if (polygon.Path.Count > 0)
                {
                    GraphicsPath path = MakeGraphicsPath(polygon.Path, x, y);

                    if (path != null)
                        paths.Add(path);

                    foreach (PathItem item in polygon.Path)
                    {
                        item.CheckBoundary(x, y, ref vTL, ref vBR);
                    }
                }
            }

            return paths;
        }

        private static GraphicsPath MakeGraphicsPath(List<PathItem> items, double x = 0.0, double y = 0.0)
        {
            GraphicsPath path = new GraphicsPath();

            foreach (PathItem item in items)
            {
                AddPath(path, item, x, y);
            }

            return path;
        }

        private List<GraphicsPath> MakePath(double x, double y, ref Vertex2D vTL, ref Vertex2D vBR)
        {
            if (m_path.Path.Count == 0)
                return null;

            List<GraphicsPath> pathList = new List<GraphicsPath>();

            foreach (PathItem item in m_path.Path)
            {
                GraphicsPath path = new GraphicsPath();
                AddPath(path, item, x, y);
                pathList.Add(path);

                item.CheckBoundary(x, y, ref vTL, ref vBR);
            }

            return pathList;
        }

        private static void AddPath(GraphicsPath path, PathItem item, double x, double y)
        {
            if (item.GetDrawType() == PathItem.DrawType.Line)
            {
                Vertex2D vBegin = null, vEnd = null, vMiddle = null;
                item.GetVertex(out vBegin, out vEnd, out vMiddle);

                PointF ptBegin = new PointF((float)(vBegin.x + x), (float)(vBegin.y + y));
                PointF ptEnd = new PointF((float)(vEnd.x + x), (float)(vEnd.y + y));

                path.AddLine(ptBegin, ptEnd);
            }
            else if (item.GetDrawType() == PathItem.DrawType.Arc || item.GetDrawType() == PathItem.DrawType.EArc)
            {
                EArc2D earc = item.GetEArc();

                if (earc != null)
                {
                    Vertex2D vTL = earc.GetTL();
                    Vertex2D vBL = earc.GetBL();
                    Vertex2D vBR = earc.GetBR();

                    // 타원의 축이 좌표축과 일치하는지 검사
                    Vertex2D vTop = new Vertex2D(vBL.x, vBL.y + 100);
                    double angle = UnE.Geometry.Math.GetAngle(vTL, vBL, vTop);

                    if (angle <= UnE.Geometry.Math.HALF_TOLERANCE())
                    {
                        RectangleF rect = new RectangleF((float)(vBL.x + x), (float)(vBL.y + y), (float)vBL.GetDistance(vBR), (float)vBL.GetDistance(vTL));

                        // Degree
                        float fBeginAngle = (float)UnE.Geometry.Math.RadToDeg(earc.GetBeginAngle());
                        float fEArcAngle = (float)UnE.Geometry.Math.RadToDeg(earc.GetAngle());

                        if (earc.IsClockWise())
                            fEArcAngle = -fEArcAngle;

                        path.AddArc(rect, fBeginAngle, fEArcAngle);
                    }
                    else
                    {
                        double dBeginAngle = EArc2D.ValidAngle(earc.GetBeginAngle());
                        double dEndAngle = earc.GetEndAngle();
                        double dEArcAngle = earc.GetAngle();
                        int nPointCount = (int)(100 * dEArcAngle / UnE.Geometry.Math._2PI());
                        double dAngle = earc.IsClockWise() ? -dEArcAngle / nPointCount : dEArcAngle / nPointCount;

                        Vertex2D vBegin = earc.GetBeginVertex();
                        PointF[] points = new PointF[nPointCount + 1];
                        points[0] = new PointF((float)vBegin.x, (float)vBegin.y);

                        Vertex2D vertex;

                        for (int i=1;i<=nPointCount;i++)
                        {
                            double dTheta = dBeginAngle + dAngle * i;

                            if (earc.GetVertex(dTheta, out vertex))
                                points[i] = new PointF((float)vertex.x, (float)vertex.y);
                            else
                                return;
                        }

                        path.AddLines(points);
                    }
                }
            }
        }

        private static bool ReadTextDatas(List<TextData> textDatas, BinaryReader reader, double dScale)
        {
            try
            {
                int nTextCount = reader.ReadInt32();

                for (int i = 0; i < nTextCount; i++)
                {
                    string strText = reader.ReadString();
                    double x = reader.ReadDouble() * dScale;
                    double y = reader.ReadDouble() * dScale;
                    float fFontSize = (float)(reader.ReadSingle() * dScale);
                    double dTextAngle = reader.ReadDouble();

                    TextData data = new TextData();

                    data.Text = strText;
                    data.Position = new Vertex2D(x, y);
                    data.FontSize = fFontSize;
                    data.TextAngle = dTextAngle;

                    textDatas.Add(data);
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private static bool ReadPolygons(List<LinkedPath> polygons, BinaryReader reader, double dScale)
        {
            int nPolygonCount = reader.ReadInt32();

            for (int i = 0; i < nPolygonCount; i++)
            {
                LinkedPath polygon = new LinkedPath();

                if (ReadPath(polygon, reader, dScale) == false)
                    return false;

                polygons.Add(polygon);
            }

            return true;
        }

        private static bool ReadPath(LinkedPath path, BinaryReader reader, double dScale)
        {
            int nPathCount = reader.ReadInt32();

            for (int i = 0; i < nPathCount; i++)
            {
                int nType = reader.ReadInt32();
                PathItem item = null;

                if (nType == (int)PathItem.DrawType.Line)
                    item = ReadLinePath(reader, dScale);
                else if (nType == (int)PathItem.DrawType.Arc)
                    item = ReadArcPath(reader, dScale);
                else if (nType == (int)PathItem.DrawType.EArc)
                    item = ReadEArcPath(reader, dScale);

                if (item == null)
                    return false;

                path.Path.Add(item);
            }

            return true;
        }

        private static PathItem ReadEArcPath(BinaryReader reader, double dScale)
        {
            try
            {
                double dTLX = reader.ReadDouble() * dScale;
                double dTLY = reader.ReadDouble() * dScale;
                double dBLX = reader.ReadDouble() * dScale;
                double dBLY = reader.ReadDouble() * dScale;
                double dBRX = reader.ReadDouble() * dScale;
                double dBRY = reader.ReadDouble() * dScale;
                double dBeginAngle = reader.ReadDouble();
                double dEArcAngle = reader.ReadDouble();
                bool isClockWise = reader.ReadBoolean();

                Vertex2D vTL = new Vertex2D(dTLX, dTLY);
                Vertex2D vBL = new Vertex2D(dBLX, dBLY);
                Vertex2D vBR = new Vertex2D(dBRX, dBRY);
                EArc2D earc = new EArc2D(vTL, vBL, vBR, dBeginAngle, dEArcAngle, isClockWise);

                PathItem item = new PathItem();
                item.SetEArc(earc);
                return item;
            }
            catch (Exception)
            {
            }

            return null;
        }

        private static PathItem ReadArcPath(BinaryReader reader, double dScale)
        {
            try
            {
                double dCenterX = reader.ReadDouble() * dScale;
                double dCenterY = reader.ReadDouble() * dScale;
                double dRadius = reader.ReadDouble() * dScale;
                double dBeginAngle = reader.ReadDouble();
                double dArcAngle = reader.ReadDouble();
                bool isClockWise = reader.ReadBoolean();

                Vertex2D vCenter = new Vertex2D(dCenterX, dCenterY);
                Arc2D arc = new Arc2D(vCenter, dRadius, dBeginAngle, dArcAngle, isClockWise);

                PathItem item = new PathItem();
                item.SetArc(arc);
                return item;
            }
            catch (Exception)
            {
            }

            return null;
        }

        private static PathItem ReadLinePath(BinaryReader reader, double dScale)
        {
            try
            {
                double dBeginX = reader.ReadDouble() * dScale;
                double dBeginY = reader.ReadDouble() * dScale;
                double dEndX = reader.ReadDouble() * dScale;
                double dEndY = reader.ReadDouble() * dScale;

                Vertex2D vBegin = new Vertex2D(dBeginX, dBeginY);
                Vertex2D vEnd = new Vertex2D(dEndX, dEndY);

                PathItem item = new PathItem();
                item.SetLine(new Line2D(vBegin, vEnd));
                return item;
            }
            catch (Exception)
            {
            }

            return null;
        }

        public void Render(Graphics g, double dScale, Color colorImage)
        {
            g.ScaleTransform(1.0f, -1.0f);

            // UNE 전용 (선 14포인트, 비트맵 여분 +30) >> 선 두께로 인해서 시작 좌표를 (X+15, Y-15)
            g.TranslateTransform((float)-m_icon.BoundaryTL.x + 15, (float)-m_icon.BoundaryTL.y - 15);
            //g.TranslateTransform((float)m_icon.BoundaryBR.x, (float)m_icon.BoundaryBR.y);

            // UNE 전용 (선 14포인트, 비트맵 여분 +30)
            Pen pen = new Pen(colorImage, 14.0f);
            Brush brush = new SolidBrush(colorImage);

            /*Pen pen = new Pen(Color.FromArgb(142, 36, 108));
            Brush brush = new SolidBrush(Color.FromArgb(142, 36, 108));
            //Pen pen = new Pen(Color.White);
            //Brush brush = new SolidBrush(Color.White);
            pen.Width = 6;*/
            /*Vertex2D vCenter = (m_icon.BoundaryTL + m_icon.BoundaryBR) / 2;
            g.TranslateTransform((float)vCenter.x, (float)vCenter.y);
            //g.TranslateTransform(0.0f, 50.0f);
            //g.ScaleTransform(1.0f, -1.0f);
            g.TranslateTransform((float)-vCenter.x, (float)-vCenter.y);*/

            m_icon.Render(g, pen, brush, 0, 0, dScale);
            //m_icon.Render(g, pen, brush, 100, 100, dScale);

            g.ScaleTransform(1.0f, -1.0f);

            pen.Dispose();
            brush.Dispose();
        }

        public void Scale(double dScale, Graphics g)
        {
            Vertex2D vTL = TL;
            Vertex2D _vTL = null, _vBR = null;

            foreach (PathItem item in m_path.Path)
            {
                item.Scale(vTL, dScale, ref _vTL, ref _vBR);
            }

            foreach (LinkedPath path in m_listPolygons)
            {
                foreach (PathItem item in path.Path)
                {
                    item.Scale(vTL, dScale, ref _vTL, ref _vBR);
                }
            }

            foreach (TextData text in m_listText)
            {
                text.Scale(g, vTL, dScale, ref _vTL, ref _vBR);
            }

            if (_vTL == null)
                return;

            // Scale 조절로 인하여 영역이 바뀌었으니 중심점을 기준으로 영역을 재설정한다.
            Vertex2D vCenter = (_vTL + _vBR) / 2;

            foreach (PathItem item in m_path.Path)
            {
                item.Move(-vCenter.x, -vCenter.y);
            }

            foreach (LinkedPath path in m_listPolygons)
            {
                foreach (PathItem item in path.Path)
                {
                    item.Move(-vCenter.x, -vCenter.y);
                }
            }

            foreach (TextData text in m_listText)
            {
                text.Move(-vCenter.x, -vCenter.y);
            }

            Move(0, 0);
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

    public class PathItem
    {
        public enum DrawType { None = 0, Line, Arc, EArc };

        private Line2D m_line = null;
        private Arc2D m_arc = null;
        private EArc2D m_earc = null;

        // m_innerXXX : 교차점 계산에 의하여 잘려진 결과 선형
        // m_XXX : 원래 선형
        private Line2D m_innerLine = null;
        private Arc2D m_innerArc = null;
        private EArc2D m_innerEArc = null;
        // 교차점 계산결과 이 선형은 사용하지 않게될 경우 m_innerPass는 true가 된다.
        private bool m_innerPass = false;

        private DrawType m_drawType = DrawType.None;

        // PathItem은 Polygon의 일부분이 되는데, 이 객체가 Arc 또는 EArc 타입일 경우
        // 해당 곡선이 Polygon 안쪽을 향해 있으면 false, 바깥쪽을 향해 있으면 true를 리턴한다.
        private bool m_arcIsOutside = false;

        public bool ArcIsOutside
        {
            get { return m_arcIsOutside; }
            set { m_arcIsOutside = value; }
        }

        public Line2D Line
        {
            get { return m_line; }
        }

        public Arc2D Arc
        {
            get { return m_arc; }
        }

        public EArc2D EArc
        {
            get { return m_earc; }
        }

        public void SetLine(Line2D line, Vertex2D vBegin = null)
        {
            m_drawType = DrawType.Line;

            if (vBegin == null)
            {
                m_line = new Line2D(line.GetVertex(true), line.GetVertex(false));
            }
            else
            {
                Vertex2D v1 = line.GetVertex(true);
                Vertex2D v2 = line.GetVertex(false);

                double len1 = v1.GetDistance(vBegin);
                double len2 = v2.GetDistance(vBegin);

                if (len1 < len2)
                    m_line = new Line2D(v1, v2);
                else
                    m_line = new Line2D(v2, v1);
            }
        }

        public void SetArc(Arc2D arc, Vertex2D vBegin = null)
        {
            m_drawType = DrawType.Arc;

            if (vBegin == null)
            {
                m_arc = new Arc2D(arc.GetCenter(), arc.GetRadius(), arc.GetBeginAngle(), arc.GetAngle(), arc.IsClockWise());
            }
            else
            {
                Vertex2D v1 = arc.GetBeginVertex();
                Vertex2D v2 = arc.GetEndVertex();

                double len1 = v1.GetDistance(vBegin);
                double len2 = v2.GetDistance(vBegin);

                if (len1 < len2)
                    m_arc = new Arc2D(arc.GetCenter(), arc.GetRadius(), arc.GetBeginAngle(), arc.GetAngle(), arc.IsClockWise());
                else
                    m_arc = new Arc2D(arc.GetCenter(), arc.GetRadius(), arc.GetEndAngle(), arc.GetAngle(), !arc.IsClockWise());
            }
        }

        public void SetEArc(EArc2D earc, Vertex2D vBegin = null)
        {
            m_drawType = DrawType.EArc;

            if (vBegin == null)
            {
                m_earc = new EArc2D(earc.GetTL(), earc.GetBL(), earc.GetBR(), earc.GetBeginAngle(), earc.GetAngle(), earc.IsClockWise());
            }
            else
            {
                Vertex2D v1 = earc.GetBeginVertex();
                Vertex2D v2 = earc.GetEndVertex();

                double len1 = v1.GetDistance(vBegin);
                double len2 = v2.GetDistance(vBegin);

                if (len1 < len2)
                    m_earc = new EArc2D(earc.GetTL(), earc.GetBL(), earc.GetBR(), earc.GetBeginAngle(), earc.GetAngle(), earc.IsClockWise());
                else
                    m_earc = new EArc2D(earc.GetTL(), earc.GetBL(), earc.GetBR(), earc.GetEndAngle(), earc.GetAngle(), !earc.IsClockWise());
            }
        }

        public DrawType GetDrawType()
        {
            return m_drawType;
        }

        public bool GetVertex(out Vertex2D vBegin, out Vertex2D vEnd, out Vertex2D vMiddle)
        {
            vBegin = vEnd = vMiddle = null;

            if (m_drawType == DrawType.Line)
            {
                if (m_line != null)
                {
                    vMiddle = null;
                    vBegin = m_line.GetVertex(true);
                    vEnd = m_line.GetVertex(false);
                    return true;
                }
            }
            else if (m_drawType == DrawType.Arc || m_drawType == DrawType.EArc)
            {
                EArc2D earc = m_earc;

                if (m_drawType == DrawType.Arc)
                    earc = m_arc;

                if (earc != null)
                {
                    vBegin = earc.GetBeginVertex();
                    vEnd = earc.GetEndVertex();

                    if (earc.GetVertex(earc.GetBeginAngle() + earc.GetAngle() / 2, out vMiddle) == false)
                        return false;
                }

                if (UnE.Geometry.Math.IsRightSideFromLine(vMiddle, vBegin, vEnd) == 1)
                    m_arcIsOutside = true;
                else
                    m_arcIsOutside = false;
            }

            return true;
        }

        // offset 만큼 이동시킨 거리에 객체의 복사본을 만들어 리턴한다.
        // isClockwise : 전체 Polygon의 진행방향이 시계방향인가?
        public PathItem Offset(double offset, bool isClockwise)
        {
            PathItem item = null;

            if (m_drawType == DrawType.Line)
            {
                if (m_line != null)
                {
                    if (isClockwise == false)
                        offset = -offset;

                    Vertex2D vBegin = UnE.Geometry.Math.GetRightVertex(m_line.GetVertex(true), m_line.GetVertex(false), -offset);
                    Vertex2D vEnd = UnE.Geometry.Math.GetRightVertex(m_line.GetVertex(false), m_line.GetVertex(true), offset);

                    item = new PathItem();
                    item.SetLine(new Line2D(vBegin, vEnd));
                }
            }
            else if (m_drawType == DrawType.Arc)
            {
                if (m_arc != null)
                {
                    Arc2D arc = m_arc.Offset(!m_arcIsOutside, offset);

                    if (arc != null)
                    {
                        item = new PathItem();
                        item.SetArc(arc);
                        item.m_arcIsOutside = m_arcIsOutside;
                    }
                }
            }
            else if (m_drawType == DrawType.EArc)
            {
                if (m_earc != null)
                {
                    EArc2D earc = m_earc.Offset(!m_arcIsOutside, offset);

                    if (earc != null)
                    {
                        item = new PathItem();
                        item.SetEArc(earc);
                        item.m_arcIsOutside = m_arcIsOutside;
                    }
                }
            }

            return item;
        }

        // item1과 item2와의 교차점을 계산하여 그 결과를 item1과 item2에 각각 반영한다.
        // Return 값 : 1(계산 성공), 2(계산 성공하였으며, items 개수가 하나 증가함), 0(계산 실패)
        public static int CalcIntersection(PathItem item1, PathItem item2, List<PathItem> items, int nItem1Index)
        {
            int nIndex = nItem1Index;
            int nResult = 0;

            PathItem itemOrigin1 = item1;
            PathItem itemOrigin2 = item2;
            int nItem2Index = 0;

            while (item1 != null)
            {
                while (item1.m_innerPass)
                {
                    nIndex--;

                    if (nIndex < 0)
                        nIndex = items.Count - 1;

                    if (nIndex == nItem1Index)
                    {
                        System.Diagnostics.Trace.WriteLine("교차점을 찾을수 없음");
                        return 0;
                    }

                    item1 = items[nIndex];
                }

                if (item1.m_drawType == DrawType.Line)
                {
                    if (item2.m_drawType == DrawType.Line)
                        nResult = CalcIntersectionLineToLine(item1, item2);
                    else if (item2.m_drawType == DrawType.Arc || item2.m_drawType == DrawType.EArc)
                        nResult = CalcIntersectionLineToEArc(item1, item2);
                }
                else if (item1.m_drawType == DrawType.Arc || item1.m_drawType == DrawType.EArc)
                {
                    if (item2.m_drawType == DrawType.Line)
                        nResult = CalcIntersectionEArcToLine(item1, item2);
                    else if (item2.m_drawType == DrawType.Arc || item2.m_drawType == DrawType.EArc)
                        nResult = CalcIntersectionEArcToEArc(item1, item2);
                }

                if (nResult == 1)
                    break;
                else if (nResult == 0)
                    continue;
                else if (nResult == -1)
                {
                    if (item1.m_drawType == DrawType.Line && item2.m_drawType == DrawType.Line)
                    {
                        // 두 직선이 한점에서 만나면서 일직선을 이루어야 하는데, 벽체의 두께가 서로 달라서 평행하게 되어버린 경우
                        item1.m_innerLine = new Line2D(item1.m_line);

                        // 두 벽체 사이에 임시 PathItem을 하나 끼워넣는다.
                        PathItem itemTemp = new PathItem();
                        itemTemp.SetLine(new Line2D(item1.m_line.GetVertex(false), item2.m_line.GetVertex(true)));
                        itemTemp.m_innerLine = new Line2D(itemTemp.m_line);
                        items.Insert(items.Count - 1, itemTemp);

                        return 2;
                    }

                    System.Diagnostics.Trace.WriteLine("교차점을 찾을수 없음");
                    return 0;
                }
                else if (nResult == -2)
                {
                    if (itemOrigin2 == items[0])
                    {
                        do
                        {
                            nItem2Index++;

                            if (nItem2Index >= items.Count || items[nItem2Index] == itemOrigin1)
                            {
                                System.Diagnostics.Trace.WriteLine("교차점을 찾을수 없음");
                                return 0;
                            }

                            item2 = items[nItem2Index];
                        }
                        while (item2.m_innerPass == false);
                    }
                    else
                        break;
                }
            }

            return 1;
        }

        // Return 값 : 1(계산 성공)
        //             0(계산결과 item1은 사용하지 않게됨)
        //            -1(계산 실패)
        //            -2(계산결과 item2를 사용하지 않게됨)
        private static int CalcIntersectionEArcToEArc(PathItem item1, PathItem item2)
        {
            EArc2D earcItem1 = item1.m_earc;
            EArc2D earcItem2 = item2.m_earc;

            if (item1.m_drawType == DrawType.Arc)
            {
                if (item1.m_innerArc != null)
                    earcItem1 = item1.m_innerArc;
                else
                    earcItem1 = item1.m_arc;
            }
            else
            {
                if (item1.m_innerEArc != null)
                    earcItem1 = item1.m_innerEArc;
            }

            if (item2.m_drawType == DrawType.Arc)
            {
                if (item2.m_innerArc != null)
                    earcItem2 = item2.m_innerArc;
                else
                    earcItem2 = item2.m_arc;
            }
            else
            {
                if (item2.m_innerEArc != null)
                    earcItem2 = item2.m_innerEArc;
            }

            if (earcItem1 == null || earcItem2 == null)
                return -1;

            ArrayList arrVertices, arrEArcs;
            int nResult = earcItem1.IntersectEArc(earcItem2, out arrVertices, out arrEArcs);

            List<Vertex2D> vertices = new List<Vertex2D>();

            if (nResult == 0)
            {
                EArc2D earc1 = null, earc2 = null;

                if (item1.m_drawType == DrawType.Arc)
                    earc1 = new Arc2D(earcItem1.GetCenter(), ((Arc2D)earcItem1).GetRadius(), 0.0, UnE.Geometry.Math._2PI(), true);
                else if (item1.m_drawType == DrawType.EArc)
                    earc1 = new EArc2D(earcItem1.GetTL(), earcItem1.GetBL(), earcItem1.GetBR(), 0.0, UnE.Geometry.Math._2PI(), true);

                if (item2.m_drawType == DrawType.Arc)
                    earc2 = new Arc2D(earcItem2.GetCenter(), ((Arc2D)earcItem2).GetRadius(), 0.0, UnE.Geometry.Math._2PI(), true);
                else if (item2.m_drawType == DrawType.EArc)
                    earc2 = new EArc2D(earcItem2.GetTL(), earcItem2.GetBL(), earcItem2.GetBR(), 0.0, UnE.Geometry.Math._2PI(), true);

                nResult = earc1.IntersectEArc(earc2, out arrVertices, out arrEArcs);

                if (nResult == 0)
                {
                    item1.m_innerPass = true;
                    return 0;
                }
                else
                    AddEArcVertices(vertices, arrVertices, arrEArcs);
            }
            else
                AddEArcVertices(vertices, arrVertices, arrEArcs);

            Vertex2D vNear = GetNearVertex(earcItem1, vertices);

            if (vNear == null)
                return -1;

            EArc2D innerEArc1, innerEArc2;

            if (IsValidEArcVertex(earcItem1, vNear, true, out innerEArc1) == false)
            {
                item1.m_innerPass = true;
                return 0;
            }

            if (IsValidEArcVertex(earcItem2, vNear, false, out innerEArc2) == false)
            {
                item2.m_innerPass = true;
                return -2;
            }

            if (item1.m_drawType == DrawType.Arc)
            {
                item1.m_innerArc = (Arc2D)innerEArc1;
            }
            else if (item1.m_drawType == DrawType.EArc)
            {
                item1.m_innerEArc = innerEArc1;
            }

            if (item2.m_drawType == DrawType.Arc)
            {
                item2.m_innerArc = (Arc2D)innerEArc2;
            }
            else if (item2.m_drawType == DrawType.EArc)
            {
                item2.m_innerEArc = innerEArc2;
            }

            return 1;
        }

        private static void AddEArcVertices(List<Vertex2D> vertices, ArrayList arrVertices, ArrayList arrEArcs)
        {
            foreach (Vertex2D vertex in arrVertices)
            {
                vertices.Add(vertex);
            }

            foreach (EArc2D earc in arrEArcs)
            {
                vertices.Add(earc.GetBeginVertex());
                vertices.Add(earc.GetEndVertex());
            }
        }

        // Return 값 : 1(계산 성공)
        //             0(계산결과 item1은 사용하지 않게됨)
        //            -1(계산 실패)
        //            -2(계산결과 item2를 사용하지 않게됨)
        private static int CalcIntersectionEArcToLine(PathItem item1, PathItem item2)
        {
            EArc2D earc = item1.m_earc;

            if (item1.m_drawType == DrawType.Arc)
            {
                if (item1.m_innerArc != null)
                    earc = item1.m_innerArc;
                else
                    earc = item1.m_arc;
            }
            else
            {
                if (item1.m_innerEArc != null)
                    earc = item1.m_innerEArc;
            }

            if (earc == null)
                return -1;

            Line2D line = item2.m_line;

            if (item2.m_innerLine != null)
                line = item2.m_innerLine;

            if (line == null)
                return -1;

            Vertex2D v1, v2;
            int nResult = earc.IntersectLine(line, out v1, out v2);

            if (nResult == 2)
            {
                v1 = GetNearVertex(earc, v1, v2);
            }
            else if (nResult == 0)
            {
                // 두 직선이 만나지 않을 경우 직선과 타원을 연장시킨다.
                Line2D line2 = new Line2D(line.GetVertex(true), line.GetVertex(false), Line2D.LineType.HALF_LINE_END_2_BEGIN);
                EArc2D earc1 = null;

                if (item1.m_drawType == DrawType.Arc)
                    earc1 = new Arc2D(earc.GetCenter(), ((Arc2D)earc).GetRadius(), 0.0, UnE.Geometry.Math._2PI(), true);
                else if (item1.m_drawType == DrawType.EArc)
                    earc1 = new EArc2D(earc.GetTL(), earc.GetBL(), earc.GetBR(), 0.0, UnE.Geometry.Math._2PI(), true);

                nResult = line2.IntersectEArc(earc1, out v1, out v2);

                if (nResult == 0)
                {
                    item1.m_innerPass = true;
                    return 0;
                }
                else if (nResult == 2)
                {
                    double len1 = line.GetVertex(false).GetDistance(v1);
                    double len2 = line.GetVertex(false).GetDistance(v2);

                    if (len2 < len1)
                        v1 = v2;
                }
            }

            EArc2D innerEArc;

            if (IsValidEArcVertex(earc, v1, true, out innerEArc) == false)
            {
                item1.m_innerPass = true;
                return 0;
            }

            if (item1.m_drawType == DrawType.Arc)
            {
                item1.m_innerArc = (Arc2D)innerEArc;
            }
            else if (item1.m_drawType == DrawType.EArc)
            {
                item1.m_innerEArc = innerEArc;
            }

            item2.m_innerLine = new Line2D(v1, line.GetVertex(false));
            return 1;
        }

        // earc 위의 두점 v1과 v2가 있다.
        // 이 가운데 earc의 시작점과 더 가까운 점을 찾아 리턴한다.
        private static Vertex2D GetNearVertex(EArc2D earc, Vertex2D v1, Vertex2D v2)
        {
            double dAngle1 = GetEArcAngle(earc, v1);
            double dAngle2 = GetEArcAngle(earc, v2);
            double dBeginAngle = GetEArcAngle(earc, earc.GetBeginVertex());

            double dEArcAngle1 = GetEArcAngle(dBeginAngle, dAngle1, earc.IsClockWise());
            double dEArcAngle2 = GetEArcAngle(dBeginAngle, dAngle2, earc.IsClockWise());
            return dEArcAngle1 < dEArcAngle2 ? v1 : v2;
        }

        // vertices 요소 가운데 earc의 시작점과 가장 가까운 점을 찾아 리턴한다.
        // 이 가운데 earc의 시작점과 더 가까운 점을 찾아 리턴한다.
        private static Vertex2D GetNearVertex(EArc2D earc, List<Vertex2D> vertices)
        {
            double dMinAngle = -1.0;
            Vertex2D vNear = null;

            double dBeginAngle = GetEArcAngle(earc, earc.GetBeginVertex());

            foreach (Vertex2D vertex in vertices)
            {
                double dAngle = GetEArcAngle(earc, vertex);
                double dEArcAngle = GetEArcAngle(dBeginAngle, dAngle, earc.IsClockWise());

                if (vNear == null || dEArcAngle < dMinAngle)
                {
                    vNear = vertex;
                    dMinAngle = dEArcAngle;
                }
            }

            return vNear;
        }

        private static double GetEArcAngle(double dBeginAngle, double dEndAngle, bool isClockwise)
        {
            if (isClockwise)
            {
                if (dEndAngle < dBeginAngle)
                    return dBeginAngle - dEndAngle;
                else
                    return UnE.Geometry.Math._2PI() - (dEndAngle - dBeginAngle);
            }
            else
            {
                if (dEndAngle > dBeginAngle)
                    return dEndAngle - dBeginAngle;
                else
                    return UnE.Geometry.Math._2PI() - (dBeginAngle - dEndAngle);
            }
        }

        // Return 값 : 1(계산 성공)
        //             0(계산결과 item1은 사용하지 않게됨)
        //            -1(계산 실패)
        //            -2(계산결과 item2를 사용하지 않게됨)
        private static int CalcIntersectionLineToEArc(PathItem item1, PathItem item2)
        {
            EArc2D earc = item2.m_earc;

            if (item2.m_drawType == DrawType.Arc)
            {
                if (item2.m_innerArc != null)
                    earc = item2.m_innerArc;
                else
                    earc = item2.m_arc;
            }
            else
            {
                if (item2.m_innerEArc != null)
                    earc = item2.m_innerEArc;
            }

            if (earc == null)
                return -1;

            Line2D line = item1.m_line;

            if (item1.m_innerLine != null)
                line = item1.m_innerLine;

            Vertex2D v1, v2;
            int nResult = line.IntersectEArc(earc, out v1, out v2);

            if (nResult == 2)
            {
                double len1 = line.GetVertex(true).GetDistance(v1);
                double len2 = line.GetVertex(true).GetDistance(v2);

                if (len2 < len1)
                    v1 = v2;
            }
            else if (nResult == 0)
            {
                // 두 직선이 만나지 않을 경우 직선과 타원을 연장시킨다.
                Line2D line1 = new Line2D(line.GetVertex(true), line.GetVertex(false), Line2D.LineType.HALF_LINE_BEGIN_2_END);
                EArc2D earc2 = null;

                if (item2.m_drawType == DrawType.Arc)
                    earc2 = new Arc2D(earc.GetCenter(), ((Arc2D)earc).GetRadius(), 0.0, UnE.Geometry.Math._2PI(), true);
                else if (item2.m_drawType == DrawType.EArc)
                    earc2 = new EArc2D(earc.GetTL(), earc.GetBL(), earc.GetBR(), 0.0, UnE.Geometry.Math._2PI(), true);

                nResult = line1.IntersectEArc(earc2, out v1, out v2);

                if (nResult == 0)
                {
                    item1.m_innerPass = true;
                    return 0;
                }
                else if (nResult == 2)
                {
                    double len1 = line.GetVertex(true).GetDistance(v1);
                    double len2 = line.GetVertex(true).GetDistance(v2);

                    if (len2 < len1)
                        v1 = v2;
                }
            }

            EArc2D innerEArc;

            if (IsValidEArcVertex(earc, v1, false, out innerEArc) == false)
            {
                item2.m_innerPass = true;
                return -2;
            }

            if (item1.m_innerLine == null)
            {
                item1.m_innerLine = new Line2D(line.GetVertex(true), v1);
            }
            else
            {
                double len1 = line.GetVertex(true).GetDistance(item1.m_innerLine.GetVertex(false));
                double len2 = line.GetVertex(true).GetDistance(v1);

                if (len1 < len2)
                {
                    item1.m_innerLine.SetVertex(item1.m_innerLine.GetVertex(false), true);
                    item1.m_innerLine.SetVertex(v1, false);
                }
                else
                {
                    item1.m_innerPass = true;
                    return 0;
                }
            }

            if (item2.m_drawType == DrawType.Arc)
                item2.m_innerArc = (Arc2D)innerEArc;
            else if (item2.m_drawType == DrawType.EArc)
                item2.m_innerEArc = innerEArc;

            return 1;
        }

        private static bool IsValidEArcVertex(EArc2D earc, Vertex2D vertex, bool inverse, out EArc2D result)
        {
            result = null;

            double dAngle = GetEArcAngle(earc, vertex);
            Vertex2D vBegin = earc.GetBeginVertex();
            Vertex2D vEnd = earc.GetEndVertex();

            // vertex가 earc내에 속해 있는가?
            if (earc.CheckValidAngle(dAngle))
            {
                if (inverse)
                {
                    if (vertex.GetDistance(vBegin) <= 0.1)
                        return false;
                }
                else
                {
                    if (vertex.GetDistance(vEnd) <= 0.1)
                        return false;
                }

                double dBeginAngle = GetEArcAngle(earc, vBegin);
                double dEndAngle = GetEArcAngle(earc, vEnd);
                double dEArcAngle = 0.0;

                if (inverse)
                {
                    dEArcAngle = GetEArcAngle(dBeginAngle, dAngle, earc.IsClockWise());
                }
                else
                {
                    dEArcAngle = GetEArcAngle(dEndAngle, dAngle, earc.IsClockWise());
                    /*if (dAngle > dBeginAngle)
                    {
                        if (earc.IsClockWise())
                            dEArcAngle = dAngle - dEndAngle;
                        else
                        {
                            if (dEndAngle > dAngle)
                                dEArcAngle = dEndAngle - dAngle;
                            else
                                dEArcAngle = UnE.Geometry.Math._2PI() - (dAngle - dEndAngle);
                        }
                    }
                    else
                    {
                        if (earc.IsClockWise())
                        {
                            if (dEndAngle < dAngle)
                                dEArcAngle = dAngle - dEndAngle;
                            else
                                dEArcAngle = UnE.Geometry.Math._2PI() - (dEndAngle - dAngle);
                        }
                        else
                            dEArcAngle = dEndAngle - dAngle;
                    }*/
                }

                if (inverse)
                {
                    if (earc is Arc2D)
                        result = new Arc2D(earc.GetCenter(), ((Arc2D)earc).GetRadius(), dBeginAngle, dEArcAngle, earc.IsClockWise());
                    else
                        result = new EArc2D(earc.GetTL(), earc.GetBL(), earc.GetBR(), dBeginAngle, dEArcAngle, earc.IsClockWise());
                }
                else
                {
                    if (earc is Arc2D)
                        result = new Arc2D(earc.GetCenter(), ((Arc2D)earc).GetRadius(), dAngle, dEArcAngle, earc.IsClockWise());
                    else
                        result = new EArc2D(earc.GetTL(), earc.GetBL(), earc.GetBR(), dAngle, dEArcAngle, earc.IsClockWise());
                }

                return true;
            }

            if (inverse)
            {
                // vertex가 earc의 연장선에 있다면 earc의 시작점 보다는 끝점에 더 가까워야 한다.
                return vBegin.GetDistance(vertex) > vEnd.GetDistance(vertex);
            }
            //else
            // vertex가 earc의 연장선에 있다면 earc의 끝점 보다는 시작점에 더 가까워야 한다.
            return vBegin.GetDistance(vertex) < vEnd.GetDistance(vertex);
        }

        public static double GetEArcAngle(EArc2D earc, Vertex2D vertex)
        {
            Vertex2D vCenter = earc.GetCenter();
            Vertex2D vRight = new Vertex2D(vCenter.x + earc.GetBR().GetDistance(earc.GetBL()), vCenter.y);

            double dAngle = 0.0;

            if (vertex.y < vCenter.y)
                dAngle = UnE.Geometry.Math._2PI() - UnE.Geometry.Math.GetAngle(vertex, vCenter, vRight);
            else
                dAngle = UnE.Geometry.Math.GetAngle(vertex, vCenter, vRight);

            return dAngle;
        }

        // Return 값 : 1(계산 성공)
        //             0(계산결과 item1은 사용하지 않게됨)
        //            -1(계산 실패)
        private static int CalcIntersectionLineToLine(PathItem item1, PathItem item2)
        {
            Line2D itemLine1 = item1.m_line;
            Line2D itemLine2 = item2.m_line;

            if (item1.m_innerLine != null)
                itemLine1 = item1.m_innerLine;

            if (item2.m_innerLine != null)
                itemLine2 = item2.m_innerLine;

            if (itemLine1 == null || itemLine2 == null)
                return -1;

            Vertex2D v1, v2;
            Line2D.LineType lineType;
            int nResult = itemLine1.IntersectLine(itemLine2, out v1, out v2, out lineType);

            if (nResult == 2)
            {
                System.Diagnostics.Trace.WriteLine("Error");
                return -1;
            }
            else if (nResult == 0)
            {
                // 두 직선이 만나지 않을 경우 각각의 직선을 연장시켜 만나는 점을 찾는다.
                Line2D line1 = new Line2D(itemLine1.GetVertex(true), itemLine1.GetVertex(false), Line2D.LineType.HALF_LINE_BEGIN_2_END);
                Line2D line2 = new Line2D(itemLine2.GetVertex(true), itemLine2.GetVertex(false), Line2D.LineType.HALF_LINE_END_2_BEGIN);

                nResult = line1.IntersectLine(line2, out v1, out v2, out lineType);

                if (nResult == 0)
                {
                    System.Diagnostics.Trace.WriteLine("Error");
                    return -1;
                }
            }

            if (item1.m_innerLine == null)
            {
                item1.m_innerLine = new Line2D(itemLine1.GetVertex(true), v1);
            }
            else
            {
                item1.m_innerLine.SetVertex(v1, false);
                /*double len1 = itemLine1.GetVertex(true).GetDistance(item1.m_innerLine.GetVertex(false));
                double len2 = itemLine1.GetVertex(true).GetDistance(v1);

                if (len1 < len2)
                {
                    item1.m_innerLine.SetVertex(item1.m_innerLine.GetVertex(false), true);
                    item1.m_innerLine.SetVertex(v1, false);
                }
                else
                {
                    item1.m_innerPass = true;
                    return 0;
                }*/
            }

            item2.m_innerLine = new Line2D(v1, itemLine2.GetVertex(false));
            return 1;
        }

        public void InnerToCenter()
        {
            if (m_drawType == DrawType.Line)
            {
                m_line = m_innerLine;
                m_innerLine = null;
            }
            else if (m_drawType == DrawType.Arc)
            {
                m_arc = m_innerArc;
                m_innerArc = null;
            }
            else if (m_drawType == DrawType.EArc)
            {
                m_earc = m_innerEArc;
                m_innerEArc = null;
            }
        }

        public EArc2D GetEArc()
        {
            if (m_drawType == DrawType.Arc)
                return m_arc;
            else if (m_drawType == DrawType.EArc)
                return m_earc;

            return null;
        }

        public void CheckBoundary(double x, double y, ref Vertex2D vTL, ref Vertex2D vBR)
        {
            if (m_drawType == DrawType.Line)
            {
                Vertex2D vBegin = m_line.GetVertex(true);
                Vertex2D vEnd = m_line.GetVertex(false);

                SetBoundary(vBegin.x + x, vBegin.y + y, ref vTL, ref vBR);
                SetBoundary(vEnd.x + x, vEnd.y + y, ref vTL, ref vBR);
            }
            else if (m_drawType == DrawType.Arc || m_drawType == DrawType.EArc)
            {
                EArc2D arc = m_drawType == DrawType.Arc ? m_arc : m_earc;

                Vertex2D _vTL = arc.GetTL();
                Vertex2D _vBL = arc.GetBL();
                Vertex2D _vBR = arc.GetBR();

                SetBoundary(_vTL.x + x, _vTL.y + y, ref vTL, ref vBR);
                SetBoundary(_vBL.x + x, _vBL.y + y, ref vTL, ref vBR);
                SetBoundary(_vBR.x + x, _vBR.y + y, ref vTL, ref vBR);
            }
        }

        public static void SetBoundary(double x, double y, ref Vertex2D vTL, ref Vertex2D vBR)
        {
            if (vTL == null)
            {
                vTL = new Vertex2D(x, y);
                vBR = new Vertex2D(x, y);
            }
            else
            {
                if (vTL.x > x)
                    vTL.x = x;
                if (vTL.y < y)
                    vTL.y = y;
                if (vBR.x < x)
                    vBR.x = x;
                if (vBR.y > y)
                    vBR.y = y;
            }
        }

        public void Move(double x, double y)
        {
            if (m_drawType == DrawType.Line)
                MoveLine(x, y);
            else if (m_drawType == DrawType.Arc)
                MoveArc(x, y);
            else if (m_drawType == DrawType.EArc)
                MoveEArc(x, y);
        }

        private void MoveEArc(double x, double y)
        {
            if (m_earc == null)
                return;

            Vertex2D vTL = m_earc.GetTL();
            Vertex2D vBL = m_earc.GetBL();
            Vertex2D vBR = m_earc.GetBR();

            vTL.x += x;
            vTL.y += y;
            vBL.x += x;
            vBL.y += y;
            vBR.x += x;
            vBR.y += y;

            m_earc = new EArc2D(vTL, vBL, vBR, m_earc.GetBeginAngle(), m_earc.GetAngle(), m_earc.IsClockWise());
        }

        private void MoveArc(double x, double y)
        {
            if (m_arc == null)
                return;

            Vertex2D vCenter = m_arc.GetCenter();
            double dRadius = m_arc.GetRadius();

            vCenter.x += x;
            vCenter.y += y;
            m_arc = new Arc2D(vCenter, dRadius, m_arc.GetBeginAngle(), m_arc.GetAngle(), m_arc.IsClockWise());
        }

        private void MoveLine(double x, double y)
        {
            if (m_line == null)
                return;

            Vertex2D vBegin = m_line.GetVertex(true);
            Vertex2D vEnd = m_line.GetVertex(false);

            vBegin.x += x;
            vBegin.y += y;
            vEnd.x += x;
            vEnd.y += y;

            m_line.SetVertex(vBegin, true);
            m_line.SetVertex(vEnd, false);
        }

        public void Scale(Vertex2D vPos, double dScale, ref Vertex2D vTL, ref Vertex2D vBR)
        {
            if (m_drawType == DrawType.Line)
                ScaleLine(vPos, dScale, ref vTL, ref vBR);
            else if (m_drawType == DrawType.Arc)
                ScaleArc(vPos, dScale, ref vTL, ref vBR);
            else if (m_drawType == DrawType.EArc)
                ScaleEArc(vPos, dScale, ref vTL, ref vBR);
        }

        private void ScaleEArc(Vertex2D vPos, double dScale, ref Vertex2D _vTL, ref Vertex2D _vBR)
        {
            if (m_earc == null)
                return;

            Vertex2D vTL = m_earc.GetTL();
            Vertex2D vBL = m_earc.GetBL();
            Vertex2D vBR = m_earc.GetBR();

            vTL = UnE.Geometry.Math.GetLinearVertex(vPos, vTL, vPos.GetDistance(vTL) * dScale);
            vBL = UnE.Geometry.Math.GetLinearVertex(vPos, vBL, vPos.GetDistance(vBL) * dScale);
            vBR = UnE.Geometry.Math.GetLinearVertex(vPos, vBR, vPos.GetDistance(vBR) * dScale);

            m_earc = new EArc2D(vTL, vBL, vBR, m_earc.GetBeginAngle(), m_earc.GetAngle(), m_earc.IsClockWise());

            Vertex2D vArcTL = m_earc.GetTL();
            Vertex2D vArcBR = m_earc.GetBR();
            SetBoundary(vArcTL.x, vArcTL.y, ref _vTL, ref _vBR);
            SetBoundary(vArcBR.x, vArcBR.y, ref _vTL, ref _vBR);
        }

        private void ScaleArc(Vertex2D vPos, double dScale, ref Vertex2D vTL, ref Vertex2D vBR)
        {
            if (m_arc == null)
                return;

            Vertex2D vCenter = m_arc.GetCenter();
            double dRadius = m_arc.GetRadius();

            vCenter = UnE.Geometry.Math.GetLinearVertex(vPos, vCenter, vPos.GetDistance(vCenter) * dScale);
            dRadius *= dScale;

            m_arc = new Arc2D(vCenter, dRadius, m_arc.GetBeginAngle(), m_arc.GetAngle(), m_arc.IsClockWise());

            Vertex2D vArcTL = m_arc.GetTL();
            Vertex2D vArcBR = m_arc.GetBR();
            SetBoundary(vArcTL.x, vArcTL.y, ref vTL, ref vBR);
            SetBoundary(vArcBR.x, vArcBR.y, ref vTL, ref vBR);
        }

        private void ScaleLine(Vertex2D vPos, double dScale, ref Vertex2D vTL, ref Vertex2D vBR)
        {
            if (m_line == null)
                return;

            Vertex2D vBegin = m_line.GetVertex(true);
            Vertex2D vEnd = m_line.GetVertex(false);

            vBegin = UnE.Geometry.Math.GetLinearVertex(vPos, vBegin, vPos.GetDistance(vBegin) * dScale);
            vEnd = UnE.Geometry.Math.GetLinearVertex(vPos, vEnd, vPos.GetDistance(vEnd) * dScale);

            m_line.SetVertex(vBegin, true);
            m_line.SetVertex(vEnd, false);

            SetBoundary(vBegin.x, vBegin.y, ref vTL, ref vBR);
            SetBoundary(vEnd.x, vEnd.y, ref vTL, ref vBR);
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

        public void Render(Graphics g, float x, float y, Color color)
        {
            if (System.Math.Abs(m_dTextAngle) > UnE.Geometry.Math.HALF_TOLERANCE())
            {
                g.TranslateTransform(x, y);
                g.RotateTransform((float)m_dTextAngle);
                g.TranslateTransform(-x, -y);
            }

            g.ScaleTransform(1.0f, -1.0f);
            y = -y;

            // 현재 Y축 Scale값을 가져온다.
            float x1 = g.Transform.Elements[3];
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

        public void Move(double x, double y)
        {
            m_vPos.x += x;
            m_vPos.y += y;
        }

        public void Scale(Graphics g, Vertex2D vPos, double dScale, ref Vertex2D vTL, ref Vertex2D vBR)
        {
            m_vPos = UnE.Geometry.Math.GetLinearVertex(vPos, m_vPos, vPos.GetDistance(m_vPos) * dScale);
            m_fFontSize *= (float)dScale;

            Font font = GetFont();
            SizeF size = g.MeasureString(m_strText, font);

            PathItem.SetBoundary(m_vPos.x, m_vPos.y, ref vTL, ref vBR);
            PathItem.SetBoundary(m_vPos.x + size.Width, m_vPos.y - size.Height, ref vTL, ref vBR);
        }
    }
}
