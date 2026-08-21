using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace libShapeFile
{
    public class USHReader
    {
        private UnE.Geometry.Vertex2F m_vTL = null;
        private UnE.Geometry.Vertex2F m_vBR = null;

        public UnE.Geometry.Vertex2F TopLeft
        {
            get { return m_vTL; }
        }

        public UnE.Geometry.Vertex2F BottomRight
        {
            get { return m_vBR; }
        }

        public List<Shape> Read(string strPath, out ShapeInfo shapeInfo)
        {
            shapeInfo = null;

            FileStream fs = new FileStream(strPath, FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(fs);

            Encoding encoding = GetEncoding(reader);

            if (encoding == null)
            {
                reader.Close();
                return null;
            }

            string strVersionName = ReadString(reader, encoding);
            byte realTypeByte = reader.ReadByte();

            ShapeList.RealType realType = ShapeList.RealType.FLOAT;

            if (realTypeByte == (byte)ShapeList.RealType.FLOAT)
                realType = ShapeList.RealType.FLOAT;
            else if (realTypeByte == (byte)ShapeList.RealType.DOUBLE)
                realType = ShapeList.RealType.DOUBLE;
            else
            {
                reader.Close();
                return null;
            }

            List<Shape> shapes = ReadObjectList(reader, realType);

            if (shapes == null)
            {
                reader.Close();
                return null;
            }

            shapeInfo = ReadShapeInfo(reader, shapes, encoding);

            if (shapeInfo == null)
            {
                reader.Close();
                return null;
            }

            reader.Close();
            return shapes;
        }

        private ShapeInfo ReadShapeInfo(BinaryReader reader, List<Shape> shapes, Encoding encoding)
        {
            ShapeInfo shapeInfo = new ShapeInfo();

            int nFieldCount = reader.ReadInt32();

            for (int i=0;i<nFieldCount;i++)
            {
                string strFieldName = ReadString(reader, encoding);
                shapeInfo.AddFieldName(strFieldName);
            }

            int nShapeCount = shapes.Count;

            for (int i=0;i<nShapeCount;i++)
            {
                List<string> fieldDatas = new List<string>();

                for (int j=0;j<nFieldCount;j++)
                {
                    string strFieldData = ReadString(reader, encoding);
                    fieldDatas.Add(strFieldData);
                }

                shapeInfo.AddFieldDatas(fieldDatas);
            }

            return shapeInfo;
        }

        private List<Shape> ReadObjectList(BinaryReader reader, ShapeList.RealType realType)
        {
            int nShapeCount = reader.ReadInt32();

            List<Shape> shapes = null;

            if (nShapeCount == 0)
                return new List<Shape>();

            int nObjectType = reader.ReadInt32();

            if (nObjectType == (int)ShapeType.Point)
                shapes = ReadPoints(reader, realType, nShapeCount);
            else if (nObjectType == (int)ShapeType.PolyLine)
                shapes = ReadPolyLines(reader, realType, nShapeCount);
            else if (nObjectType == (int)ShapeType.Polygon)
                shapes = ReadPolygons(reader, realType, nShapeCount);
            else if (nObjectType == (int)ShapeType.MultiPoint)
                shapes = ReadMultiPoints(reader, realType, nShapeCount);
            else if (nObjectType == (int)ShapeType.PointZ)
                shapes = ReadPointZs(reader, realType, nShapeCount);
            else if (nObjectType == (int)ShapeType.PolyLineZ)
                shapes = ReadPolyLineZs(reader, realType, nShapeCount);
            else if (nObjectType == (int)ShapeType.PolygonZ)
                shapes = ReadPolygonZs(reader, realType, nShapeCount);
            else if (nObjectType == (int)ShapeType.MultiPointZ)
                shapes = ReadMultiPointZs(reader, realType, nShapeCount);
            else if (nObjectType == (int)ShapeType.PointM)
                shapes = ReadPointMs(reader, realType, nShapeCount);
            else if (nObjectType == (int)ShapeType.PolyLineM)
                shapes = ReadPolyLineMs(reader, realType, nShapeCount);
            else if (nObjectType == (int)ShapeType.PolygonM)
                shapes = ReadPolygonMs(reader, realType, nShapeCount);
            else if (nObjectType == (int)ShapeType.MultiPointM)
                shapes = ReadMultiPointMs(reader, realType, nShapeCount);
            else
                return null;

            return shapes;
        }

        private List<Shape> ReadPoints(BinaryReader reader, ShapeList.RealType realType, int nObjectCount)
        {
            List<Shape> shapes = new List<Shape>();

            if (realType == ShapeList.RealType.FLOAT)
            {
                for (int i=0;i<nObjectCount;i++)
                {
                    float x = reader.ReadSingle();
                    float y = reader.ReadSingle();

                    Point pt = new Point(x, y, i);
                    shapes.Add(pt);
                }
            }
            else if (realType == ShapeList.RealType.DOUBLE)
            {
                for (int i=0;i<nObjectCount;i++)
                {
                    double x = reader.ReadDouble();
                    double y = reader.ReadDouble();

                    Point pt = new Point(x, y, i);
                    shapes.Add(pt);
                }
            }

            return shapes;
        }

        private List<Shape> ReadPolyLines(BinaryReader reader, ShapeList.RealType realType, int nObjectCount)
        {
            List<Shape> shapes = new List<Shape>();

            if (realType == ShapeList.RealType.FLOAT)
            {
                for (int i = 0; i < nObjectCount; i++)
                {
                    Int32 nSubPolyLineCount = reader.ReadInt32();
                    PolyLine polyline = new PolyLine(i);
                    shapes.Add(polyline);

                    for (int j = 0; j < nSubPolyLineCount; j++)
                    {
                        List<UnE.Geometry.Vertex2F> vertices = new List<UnE.Geometry.Vertex2F>();
                        polyline.AddSubPolyLine(vertices);

                        int nVertexCount = reader.ReadInt32();

                        for (int k = 0; k < nVertexCount; k++)
                        {
                            Single x = reader.ReadSingle();
                            Single y = reader.ReadSingle();

                            UnE.Geometry.Vertex2F vertex = new UnE.Geometry.Vertex2F((float)x, (float)y);
                            vertices.Add(vertex);
                        }
                    }
                }
            }
            else if (realType == ShapeList.RealType.DOUBLE)
            {
                for (int i = 0; i < nObjectCount; i++)
                {
                    Int32 nSubPolyLineCount = reader.ReadInt32();
                    PolyLine polyline = new PolyLine(i);
                    shapes.Add(polyline);

                    for (int j = 0; j < nSubPolyLineCount; j++)
                    {
                        List<UnE.Geometry.Vertex2F> vertices = new List<UnE.Geometry.Vertex2F>();
                        polyline.AddSubPolyLine(vertices);

                        int nVertexCount = reader.ReadInt32();

                        for (int k = 0; k < nVertexCount; k++)
                        {
                            Double x = reader.ReadDouble();
                            Double y = reader.ReadDouble();

                            UnE.Geometry.Vertex2F vertex = new UnE.Geometry.Vertex2F((float)x, (float)y);
                            vertices.Add(vertex);
                        }
                    }
                }
            }

            return shapes;
        }

        private List<Shape> ReadPolygons(BinaryReader reader, ShapeList.RealType realType, int nObjectCount)
        {
            List<Shape> shapes = new List<Shape>();

         
            if (realType == ShapeList.RealType.FLOAT)
            {
                for (int i = 0; i < nObjectCount; i++)
                {
                    double minx = reader.ReadDouble();
                    double minY = reader.ReadDouble();
                    double maxx = reader.ReadDouble();
                    double maxY = reader.ReadDouble();

                    if (m_vTL == null)
                    {
                        m_vTL = new UnE.Geometry.Vertex2F((float)minx, (float)maxY);
                        m_vBR = new UnE.Geometry.Vertex2F((float)maxx, (float)minY);
                    }
                    else
                    {
                        if (m_vTL.x > minx)
                            m_vTL.x = (float)minx;
                        if (m_vTL.y < maxY)
                            m_vTL.y = (float)maxY;
                        if (m_vBR.x < maxx)
                            m_vBR.x = (float)maxx;
                        if (m_vBR.y > minY)
                            m_vBR.y = (float)minY;
                    }

                    Int32 nSubPolygonCount = reader.ReadInt32();
                    Polygon polygon = new Polygon(i);
                    shapes.Add(polygon);

                    Box box = new Box(minx, minY, maxx, maxY);
                    polygon.m_boundingXY = box;
                    
                    for (int j = 0; j < nSubPolygonCount; j++)
                    {
                        List<UnE.Geometry.Vertex2F> vertices = new List<UnE.Geometry.Vertex2F>();
                        polygon.AddSubPolygon(vertices);

                        Int64 nVertexCount = reader.ReadInt64();

                        for (int k = 0; k < nVertexCount; k++)
                        {
                            Single x = reader.ReadSingle();
                            Single y = reader.ReadSingle();

                            UnE.Geometry.Vertex2F vertex = new UnE.Geometry.Vertex2F(x, y);
                            vertices.Add(vertex);
                        }
                    }
                }
            }
            else if (realType == ShapeList.RealType.DOUBLE)
            {
                for (int i = 0; i < nObjectCount; i++)
                {
                    Int32 nSubPolygonCount = reader.ReadInt32();
                    Polygon polygon = new Polygon(i);
                    shapes.Add(polygon);

                    for (int j = 0; j < nSubPolygonCount; j++)
                    {
                        List<UnE.Geometry.Vertex2F> vertices = new List<UnE.Geometry.Vertex2F>();
                        polygon.AddSubPolygon(vertices);

                        int nVertexCount = reader.ReadInt32();

                        for (int k = 0; k < nVertexCount; k++)
                        {
                            Double x = reader.ReadDouble();
                            Double y = reader.ReadDouble();

                            UnE.Geometry.Vertex2F vertex = new UnE.Geometry.Vertex2F((float)x, (float)y);
                            vertices.Add(vertex);
                        }
                    }
                }
            }

            return shapes;
        }

        private List<Shape> ReadMultiPoints(BinaryReader reader, ShapeList.RealType realType, int nObjectCount)
        {
            List<Shape> shapes = new List<Shape>();

            if (realType == ShapeList.RealType.FLOAT)
            {
                for (int i = 0; i < nObjectCount; i++)
                {
                    MultiPoint multiPoint = new MultiPoint(i);
                    shapes.Add(multiPoint);

                    int nPointCount = reader.ReadInt32();

                    for (int j = 0; j < nPointCount; j++)
                    {
                        float x = reader.ReadSingle();
                        float y = reader.ReadSingle();

                        UnE.Geometry.Vertex2F vertex = new UnE.Geometry.Vertex2F(x, y);
                        multiPoint.Vertices.Add(vertex);
                    }
                }
            }
            else if (realType == ShapeList.RealType.DOUBLE)
            {
                for (int i = 0; i < nObjectCount; i++)
                {
                    MultiPoint multiPoint = new MultiPoint(i);
                    shapes.Add(multiPoint);

                    int nPointCount = reader.ReadInt32();

                    for (int j = 0; j < nPointCount; j++)
                    {
                        double x = reader.ReadDouble();
                        double y = reader.ReadDouble();

                        UnE.Geometry.Vertex2F vertex = new UnE.Geometry.Vertex2F((float)x, (float)y);
                        multiPoint.Vertices.Add(vertex);
                    }
                }
            }

            return shapes;
        }

        private List<Shape> ReadPointZs(BinaryReader reader, ShapeList.RealType realType, int nObjectCount)
        {
            List<Shape> shapes = new List<Shape>();

            if (realType == ShapeList.RealType.FLOAT)
            {
                for (int i = 0; i < nObjectCount; i++)
                {
                    float x = reader.ReadSingle();
                    float y = reader.ReadSingle();
                    float z = reader.ReadSingle();
                    double m = reader.ReadDouble();

                    PointZ pt = new PointZ(x, y, z, m, i);
                    shapes.Add(pt);
                }
            }
            else if (realType == ShapeList.RealType.DOUBLE)
            {
                for (int i = 0; i < nObjectCount; i++)
                {
                    double x = reader.ReadDouble();
                    double y = reader.ReadDouble();
                    double z = reader.ReadDouble();
                    double m = reader.ReadDouble();

                    PointZ pt = new PointZ(x, y, z, m, i);
                    shapes.Add(pt);
                }
            }

            return shapes;
        }

        private List<Shape> ReadPolyLineZs(BinaryReader reader, ShapeList.RealType realType, int nObjectCount)
        {
            List<Shape> shapes = new List<Shape>();

            if (realType == ShapeList.RealType.FLOAT)
            {
                for (int i = 0; i < nObjectCount; i++)
                {
                    int nSubPolyLineCount = reader.ReadInt32();
                    PolyLineZ polyline = new PolyLineZ(i);
                    shapes.Add(polyline);

                    for (int j = 0; j < nSubPolyLineCount; j++)
                    {
                        List<Vertex3FM> vertices = new List<Vertex3FM>();
                        polyline.AddSubPolyLine(vertices);

                        int nVertexCount = reader.ReadInt32();

                        for (int k = 0; k < nVertexCount; k++)
                        {
                            float x = reader.ReadSingle();
                            float y = reader.ReadSingle();
                            float z = reader.ReadSingle();
                            double m = reader.ReadDouble();

                            Vertex3FM vertex = new Vertex3FM(x, y, z, m);
                            vertices.Add(vertex);
                        }
                    }
                }
            }
            else if (realType == ShapeList.RealType.DOUBLE)
            {
                for (int i = 0; i < nObjectCount; i++)
                {
                    int nSubPolyLineCount = reader.ReadInt32();
                    PolyLineZ polyline = new PolyLineZ(i);
                    shapes.Add(polyline);

                    for (int j = 0; j < nSubPolyLineCount; j++)
                    {
                        List<Vertex3FM> vertices = new List<Vertex3FM>();
                        polyline.AddSubPolyLine(vertices);

                        int nVertexCount = reader.ReadInt32();

                        for (int k = 0; k < nVertexCount; k++)
                        {
                            double x = reader.ReadDouble();
                            double y = reader.ReadDouble();
                            double z = reader.ReadDouble();
                            double m = reader.ReadDouble();

                            Vertex3FM vertex = new Vertex3FM((float)x, (float)y, (float)z, m);
                            vertices.Add(vertex);
                        }
                    }
                }
            }

            return shapes;
        }

        private List<Shape> ReadPolygonZs(BinaryReader reader, ShapeList.RealType realType, int nObjectCount)
        {
            List<Shape> shapes = new List<Shape>();

            if (realType == ShapeList.RealType.FLOAT)
            {
                for (int i = 0; i < nObjectCount; i++)
                {
                    int nSubPolygonCount = reader.ReadInt32();
                    PolygonZ polygon = new PolygonZ(i);
                    shapes.Add(polygon);

                    for (int j = 0; j < nSubPolygonCount; j++)
                    {
                        List<Vertex3FM> vertices = new List<Vertex3FM>();
                        polygon.AddSubPolygon(vertices);

                        int nVertexCount = reader.ReadInt32();

                        for (int k = 0; k < nVertexCount; k++)
                        {
                            float x = reader.ReadSingle();
                            float y = reader.ReadSingle();
                            float z = reader.ReadSingle();
                            double m = reader.ReadDouble();

                            Vertex3FM vertex = new Vertex3FM(x, y, z, m);
                            vertices.Add(vertex);
                        }
                    }
                }
            }
            else if (realType == ShapeList.RealType.DOUBLE)
            {
                for (int i = 0; i < nObjectCount; i++)
                {
                    int nSubPolygonCount = reader.ReadInt32();
                    PolygonZ polygon = new PolygonZ(i);
                    shapes.Add(polygon);

                    for (int j = 0; j < nSubPolygonCount; j++)
                    {
                        List<Vertex3FM> vertices = new List<Vertex3FM>();
                        polygon.AddSubPolygon(vertices);

                        int nVertexCount = reader.ReadInt32();

                        for (int k = 0; k < nVertexCount; k++)
                        {
                            double x = reader.ReadDouble();
                            double y = reader.ReadDouble();
                            double z = reader.ReadDouble();
                            double m = reader.ReadDouble();

                            Vertex3FM vertex = new Vertex3FM((float)x, (float)y, (float)z, m);
                            vertices.Add(vertex);
                        }
                    }
                }
            }

            return shapes;
        }

        private List<Shape> ReadMultiPointZs(BinaryReader reader, ShapeList.RealType realType, int nObjectCount)
        {
            List<Shape> shapes = new List<Shape>();

            if (realType == ShapeList.RealType.FLOAT)
            {
                for (int i = 0; i < nObjectCount; i++)
                {
                    MultiPointZ multiPoint = new MultiPointZ(i);
                    shapes.Add(multiPoint);

                    int nPointCount = reader.ReadInt32();

                    for (int j = 0; j < nPointCount; j++)
                    {
                        float x = reader.ReadSingle();
                        float y = reader.ReadSingle();
                        float z = reader.ReadSingle();
                        double m = reader.ReadDouble();

                        Vertex3FM vertex = new Vertex3FM(x, y, z, m);
                        multiPoint.Vertices.Add(vertex);
                    }
                }
            }
            else if (realType == ShapeList.RealType.DOUBLE)
            {
                for (int i = 0; i < nObjectCount; i++)
                {
                    MultiPointZ multiPoint = new MultiPointZ(i);
                    shapes.Add(multiPoint);

                    int nPointCount = reader.ReadInt32();

                    for (int j = 0; j < nPointCount; j++)
                    {
                        double x = reader.ReadDouble();
                        double y = reader.ReadDouble();
                        double z = reader.ReadDouble();
                        double m = reader.ReadDouble();

                        Vertex3FM vertex = new Vertex3FM((float)x, (float)y, (float)z, m);
                        multiPoint.Vertices.Add(vertex);
                    }
                }
            }

            return shapes;
        }

        private List<Shape> ReadPointMs(BinaryReader reader, ShapeList.RealType realType, int nObjectCount)
        {
            List<Shape> shapes = new List<Shape>();

            if (realType == ShapeList.RealType.FLOAT)
            {
                for (int i = 0; i < nObjectCount; i++)
                {
                    float x = reader.ReadSingle();
                    float y = reader.ReadSingle();
                    double m = reader.ReadDouble();

                    PointM pt = new PointM(x, y, m, i);
                    shapes.Add(pt);
                }
            }
            else if (realType == ShapeList.RealType.DOUBLE)
            {
                for (int i = 0; i < nObjectCount; i++)
                {
                    double x = reader.ReadDouble();
                    double y = reader.ReadDouble();
                    double m = reader.ReadDouble();

                    PointM pt = new PointM(x, y, m, i);
                    shapes.Add(pt);
                }
            }

            return shapes;
        }

        private List<Shape> ReadPolyLineMs(BinaryReader reader, ShapeList.RealType realType, int nObjectCount)
        {
            List<Shape> shapes = new List<Shape>();

            if (realType == ShapeList.RealType.FLOAT)
            {
                for (int i = 0; i < nObjectCount; i++)
                {
                    int nSubPolyLineCount = reader.ReadInt32();
                    PolyLineM polyline = new PolyLineM(i);
                    shapes.Add(polyline);

                    for (int j = 0; j < nSubPolyLineCount; j++)
                    {
                        List<Vertex2FM> vertices = new List<Vertex2FM>();
                        polyline.AddSubPolyLine(vertices);

                        int nVertexCount = reader.ReadInt32();

                        for (int k = 0; k < nVertexCount; k++)
                        {
                            float x = reader.ReadSingle();
                            float y = reader.ReadSingle();
                            double m = reader.ReadDouble();

                            Vertex2FM vertex = new Vertex2FM(x, y, m);
                            vertices.Add(vertex);
                        }
                    }
                }
            }
            else if (realType == ShapeList.RealType.DOUBLE)
            {
                for (int i = 0; i < nObjectCount; i++)
                {
                    int nSubPolyLineCount = reader.ReadInt32();
                    PolyLineM polyline = new PolyLineM(i);
                    shapes.Add(polyline);

                    for (int j = 0; j < nSubPolyLineCount; j++)
                    {
                        List<Vertex2FM> vertices = new List<Vertex2FM>();
                        polyline.AddSubPolyLine(vertices);

                        int nVertexCount = reader.ReadInt32();

                        for (int k = 0; k < nVertexCount; k++)
                        {
                            double x = reader.ReadDouble();
                            double y = reader.ReadDouble();
                            double m = reader.ReadDouble();

                            Vertex2FM vertex = new Vertex2FM((float)x, (float)y, m);
                            vertices.Add(vertex);
                        }
                    }
                }
            }

            return shapes;
        }

        private List<Shape> ReadPolygonMs(BinaryReader reader, ShapeList.RealType realType, int nObjectCount)
        {
            List<Shape> shapes = new List<Shape>();

            if (realType == ShapeList.RealType.FLOAT)
            {
                for (int i = 0; i < nObjectCount; i++)
                {
                    int nSubPolygonCount = reader.ReadInt32();
                    PolygonM polygon = new PolygonM(i);
                    shapes.Add(polygon);

                    for (int j = 0; j < nSubPolygonCount; j++)
                    {
                        List<Vertex2FM> vertices = new List<Vertex2FM>();
                        polygon.AddSubPolygon(vertices);

                        int nVertexCount = reader.ReadInt32();

                        for (int k = 0; k < nVertexCount; k++)
                        {
                            float x = reader.ReadSingle();
                            float y = reader.ReadSingle();
                            double m = reader.ReadDouble();

                            Vertex2FM vertex = new Vertex2FM(x, y, m);
                            vertices.Add(vertex);
                        }
                    }
                }
            }
            else if (realType == ShapeList.RealType.DOUBLE)
            {
                for (int i = 0; i < nObjectCount; i++)
                {
                    int nSubPolygonCount = reader.ReadInt32();
                    PolygonM polygon = new PolygonM(i);
                    shapes.Add(polygon);

                    for (int j = 0; j < nSubPolygonCount; j++)
                    {
                        List<Vertex2FM> vertices = new List<Vertex2FM>();
                        polygon.AddSubPolygon(vertices);

                        int nVertexCount = reader.ReadInt32();

                        for (int k = 0; k < nVertexCount; k++)
                        {
                            double x = reader.ReadDouble();
                            double y = reader.ReadDouble();
                            double m = reader.ReadDouble();

                            Vertex2FM vertex = new Vertex2FM((float)x, (float)y, m);
                            vertices.Add(vertex);
                        }
                    }
                }
            }

            return shapes;
        }

        private List<Shape> ReadMultiPointMs(BinaryReader reader, ShapeList.RealType realType, int nObjectCount)
        {
            List<Shape> shapes = new List<Shape>();

            if (realType == ShapeList.RealType.FLOAT)
            {
                for (int i = 0; i < nObjectCount; i++)
                {
                    MultiPointM multiPoint = new MultiPointM(i);
                    shapes.Add(multiPoint);

                    int nPointCount = reader.ReadInt32();

                    for (int j = 0; j < nPointCount; j++)
                    {
                        float x = reader.ReadSingle();
                        float y = reader.ReadSingle();
                        double m = reader.ReadDouble();

                        Vertex2FM vertex = new Vertex2FM(x, y, m);
                        multiPoint.Vertices.Add(vertex);
                    }
                }
            }
            else if (realType == ShapeList.RealType.DOUBLE)
            {
                for (int i = 0; i < nObjectCount; i++)
                {
                    MultiPointM multiPoint = new MultiPointM(i);
                    shapes.Add(multiPoint);

                    int nPointCount = reader.ReadInt32();

                    for (int j = 0; j < nPointCount; j++)
                    {
                        double x = reader.ReadDouble();
                        double y = reader.ReadDouble();
                        double m = reader.ReadDouble();

                        Vertex2FM vertex = new Vertex2FM((float)x, (float)y, m);
                        multiPoint.Vertices.Add(vertex);
                    }
                }
            }

            return shapes;
        }

        private Encoding GetEncoding(BinaryReader reader)
        {
            int nCodePage = reader.ReadInt32();

            try
            {
                Encoding encoding = Encoding.GetEncoding(nCodePage);
                return encoding;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }

            return null;
        }

        private string ReadString(BinaryReader reader, Encoding encoding)
        {
            Int32 len = reader.ReadInt32();
            byte[] bytes = reader.ReadBytes(len);
            return encoding.GetString(bytes, 0, len);
        }
    }
}
