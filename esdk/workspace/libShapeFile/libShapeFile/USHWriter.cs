using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace libShapeFile
{
    public class USHWriter
    {
        public const string VERSION_NAME = "V1.0";

        private ShapeList.RealType m_realType = ShapeList.RealType.FLOAT;

        public bool Write(string strPath, ShapeList shapes, ShapeInfo shapeInfo, ShapeList.RealType realType, Encoding encoding)
        {
            FileStream fs = new FileStream(strPath, FileMode.Create, FileAccess.Write);
            BinaryWriter writer = new BinaryWriter(fs, encoding);

            writer.Write(encoding.CodePage);
            WriteString(writer, VERSION_NAME, encoding);

            m_realType = realType;

            if (realType == ShapeList.RealType.FLOAT)
                writer.Write((byte)0);
            else if (realType == ShapeList.RealType.DOUBLE)
                writer.Write((byte)1);
            else
            {
                writer.Close();
                return false;
            }

            if (!shapes.WriteObjectList(writer, realType))
            {
                writer.Close();
                return false;
            }

            if (!shapes.WriteShapeAttrib(writer, shapeInfo, encoding))
            {
                writer.Close();
                return false;
            }

            writer.Close();
            return true;
        }

        public static void WriteString(BinaryWriter writer, string str, Encoding encoding)
        {
            byte[] bytes = encoding.GetBytes(str);
            int len = bytes.Length;

            writer.Write(len);
            writer.Write(bytes);
        }
    }

    public class ShapeList
    {
        public enum RealType : int { FLOAT = 0, DOUBLE };

        protected List<Shape> m_shapes = null;

        public virtual int ShapeCount
        {
            get
            {
                if (m_shapes == null)
                    return 0;

                return m_shapes.Count;
            }
        }

        public virtual int ObjectType
        {
            get
            {
                if (m_shapes == null || m_shapes.Count == 0)
                    return -1;

                Shape shape = m_shapes[0];

                int nObjectType = -1;

                if (shape is Point)
                    nObjectType = (int)ShapeType.Point;
                else if (shape is PolyLine)
                    nObjectType = (int)ShapeType.PolyLine;
                else if (shape is Polygon)
                    nObjectType = (int)ShapeType.Polygon;
                else if (shape is MultiPoint)
                    nObjectType = (int)ShapeType.MultiPoint;
                else if (shape is PointZ)
                    nObjectType = (int)ShapeType.PointZ;
                else if (shape is PolyLineZ)
                    nObjectType = (int)ShapeType.PolyLineZ;
                else if (shape is PolygonZ)
                    nObjectType = (int)ShapeType.PolygonZ;
                else if (shape is MultiPointZ)
                    nObjectType = (int)ShapeType.MultiPointZ;
                else if (shape is PointM)
                    nObjectType = (int)ShapeType.PointM;
                else if (shape is PolyLineM)
                    nObjectType = (int)ShapeType.PolyLineM;
                else if (shape is PolygonM)
                    nObjectType = (int)ShapeType.PolygonM;
                else if (shape is MultiPointM)
                    nObjectType = (int)ShapeType.MultiPointM;

                return nObjectType;
            }
        }

        public List<Shape> Shapes
        {
            set { m_shapes = value; }
        }

        public virtual bool WriteShapeAttrib(BinaryWriter writer, ShapeInfo shapeInfo, Encoding encoding)
        {
            int nFieldCount = shapeInfo.GetFieldCount();
            writer.Write(nFieldCount);

            for (int i=0;i<nFieldCount;i++)
            {
                string strFieldName = shapeInfo.GetFieldName(i);
                USHWriter.WriteString(writer, strFieldName, encoding);
            }

            foreach (Shape shape in m_shapes)
            {
                for (int i=0;i<nFieldCount;i++)
                {
                    string strFieldData = shapeInfo.GetFieldData(shape.ID, i);
                    USHWriter.WriteString(writer, strFieldData, encoding);
                }
            }

            return true;
        }

        public virtual bool WriteObjectList(BinaryWriter writer, RealType realType)
        {
            int nShapeCount = ShapeCount;
            writer.Write(nShapeCount);

            if (nShapeCount == 0)
            {
                writer.Close();
                return true;
            }

            int nObjectType = ObjectType;

            if (nObjectType == (int)ShapeType.Point)
                WritePoints(writer, nObjectType, realType);
            else if (nObjectType == (int)ShapeType.PolyLine)
                WritePolyLines(writer, nObjectType, realType);
            else if (nObjectType == (int)ShapeType.Polygon)
                WritePolygons(writer, nObjectType, realType);
            else if (nObjectType == (int)ShapeType.MultiPoint)
                WriteMultiPoints(writer, nObjectType, realType);
            else if (nObjectType == (int)ShapeType.PointZ)
                WritePointZs(writer, nObjectType, realType);
            else if (nObjectType == (int)ShapeType.PolyLineZ)
                WritePolyLineZs(writer, nObjectType, realType);
            else if (nObjectType == (int)ShapeType.PolygonZ)
                WritePolygonZs(writer, nObjectType, realType);
            else if (nObjectType == (int)ShapeType.MultiPointZ)
                WriteMultiPointZs(writer, nObjectType, realType);
            else if (nObjectType == (int)ShapeType.PointM)
                WritePointMs(writer, nObjectType, realType);
            else if (nObjectType == (int)ShapeType.PolyLineM)
                WritePolyLineMs(writer, nObjectType, realType);
            else if (nObjectType == (int)ShapeType.PolygonM)
                WritePolygonMs(writer, nObjectType, realType);
            else if (nObjectType == (int)ShapeType.MultiPointM)
                WriteMultiPointMs(writer, nObjectType, realType);

            if (nObjectType < 0)
                return false;

            return true;
        }

        protected virtual bool WritePoints(BinaryWriter writer, int nObjectType, RealType realType)
        {
            writer.Write(nObjectType);

            if (realType == RealType.FLOAT)
            {
                foreach (libShapeFile.Point point in m_shapes)
                {
                    WriteMinMax(writer, point);
                    writer.Write(point.Vertex.x);
                    writer.Write(point.Vertex.y);
                }
            }
            else if (realType == RealType.DOUBLE)
            {
                foreach (libShapeFile.Point point in m_shapes)
                {
                    WriteMinMax(writer, point);
                    writer.Write((double)point.Vertex.x);
                    writer.Write((double)point.Vertex.y);
                }
            }
            else
                return false;

            return true;
        }

        protected virtual bool WritePolyLines(BinaryWriter writer, int nObjectType, RealType realType)
        {
            writer.Write(nObjectType);

            foreach (libShapeFile.PolyLine polyline in m_shapes)
            {
                WriteMinMax(writer, polyline);

                int nSubPolyLineCount = polyline.SubPolyLineCount;
                writer.Write(nSubPolyLineCount);

                for (int i = 0; i < nSubPolyLineCount; i++)
                {
                    List<UnE.Geometry.Vertex2F> vertices = polyline.GetSubPolyLine(i);

                    if (realType == RealType.FLOAT)
                        WriteVerticesF(writer, vertices);
                    else
                        WriteVerticesD(writer, vertices);
                }
            }

            return true;
        }

        protected virtual bool WritePolygons(BinaryWriter writer, int nObjectType, RealType realType)
        {
            writer.Write(nObjectType);

            foreach (libShapeFile.Polygon polygon in m_shapes)
            {
                WriteMinMax(writer, polygon);

                int nSubPolygonCount = polygon.SubPolygonCount;
                writer.Write(nSubPolygonCount);

                for (int i = 0; i < nSubPolygonCount; i++)
                {
                    List<UnE.Geometry.Vertex2F> vertices = polygon.GetSubPolygon(i);

                    if (realType == RealType.FLOAT)
                        WriteVerticesF(writer, vertices);
                    else
                        WriteVerticesD(writer, vertices);
                }
            }

            return true;
        }

        protected virtual bool WriteMultiPoints(BinaryWriter writer, int nObjectType, RealType realType)
        {
            writer.Write(nObjectType);

            foreach (libShapeFile.MultiPoint multiPoint in m_shapes)
            {
                WriteMinMax(writer, multiPoint);

                int nPointCount = multiPoint.GetVertexCount();
                writer.Write(nPointCount);

                if (realType == RealType.FLOAT)
                    WriteVerticesF(writer, multiPoint.Vertices);
                else
                    WriteVerticesD(writer, multiPoint.Vertices);
            }

            return true;
        }

        protected virtual bool WritePointZs(BinaryWriter writer, int nObjectType, RealType realType)
        {
            writer.Write(nObjectType);

            if (realType == RealType.FLOAT)
            {
                foreach (libShapeFile.PointZ point in m_shapes)
                {
                    WriteMinMax(writer, point);

                    writer.Write(point.Vertex.x);
                    writer.Write(point.Vertex.y);
                    writer.Write(point.Vertex.z);
                    writer.Write(point.Vertex.m);
                }
            }
            else if (realType == RealType.DOUBLE)
            {
                foreach (libShapeFile.PointZ point in m_shapes)
                {
                    WriteMinMax(writer, point);

                    writer.Write((double)point.Vertex.x);
                    writer.Write((double)point.Vertex.y);
                    writer.Write((double)point.Vertex.z);
                    writer.Write((double)point.Vertex.m);
                }
            }
            else
                return false;

            return true;
        }

        protected virtual bool WritePolyLineZs(BinaryWriter writer, int nObjectType, RealType realType)
        {
            writer.Write(nObjectType);

            foreach (libShapeFile.PolyLineZ polyline in m_shapes)
            {
                WriteMinMax(writer, polyline);

                int nSubPolyLineCount = polyline.SubPolyLineCount;
                writer.Write(nSubPolyLineCount);

                for (int i = 0; i < nSubPolyLineCount; i++)
                {
                    List<Vertex3FM> vertices = polyline.GetSubPolyLine(i);

                    if (realType == RealType.FLOAT)
                        WriteVerticesF(writer, vertices);
                    else
                        WriteVerticesD(writer, vertices);
                }
            }

            return true;
        }

        protected virtual bool WritePolygonZs(BinaryWriter writer, int nObjectType, RealType realType)
        {
            writer.Write(nObjectType);

            foreach (libShapeFile.PolygonZ polygon in m_shapes)
            {
                WriteMinMax(writer, polygon);

                int nSubPolygonCount = polygon.SubPolygonCount;
                writer.Write(nSubPolygonCount);

                for (int i = 0; i < nSubPolygonCount; i++)
                {
                    List<Vertex3FM> vertices = polygon.GetSubPolygon(i);

                    if (realType == RealType.FLOAT)
                        WriteVerticesF(writer, vertices);
                    else
                        WriteVerticesD(writer, vertices);
                }
            }

            return true;
        }

        protected virtual bool WriteMultiPointZs(BinaryWriter writer, int nObjectType, RealType realType)
        {
            writer.Write(nObjectType);

            foreach (libShapeFile.MultiPointZ multiPoint in m_shapes)
            {
                WriteMinMax(writer, multiPoint);

                int nPointCount = multiPoint.GetVertexCount();
                writer.Write(nPointCount);

                if (realType == RealType.FLOAT)
                    WriteVerticesF(writer, multiPoint.Vertices);
                else
                    WriteVerticesD(writer, multiPoint.Vertices);
            }

            return true;
        }

        protected virtual bool WritePointMs(BinaryWriter writer, int nObjectType, RealType realType)
        {
            writer.Write(nObjectType);

            if (realType == RealType.FLOAT)
            {
                foreach (libShapeFile.PointM point in m_shapes)
                {
                    WriteMinMax(writer, point);
                    writer.Write(point.Vertex.x);
                    writer.Write(point.Vertex.y);
                    writer.Write(point.Vertex.m);
                }
            }
            else if (realType == RealType.DOUBLE)
            {
                foreach (libShapeFile.PointM point in m_shapes)
                {
                    WriteMinMax(writer, point);
                    writer.Write((double)point.Vertex.x);
                    writer.Write((double)point.Vertex.y);
                    writer.Write((double)point.Vertex.m);
                }
            }
            else
                return false;

            return true;
        }

        protected virtual bool WritePolyLineMs(BinaryWriter writer, int nObjectType, RealType realType)
        {
            writer.Write(nObjectType);

            foreach (libShapeFile.PolyLineM polyline in m_shapes)
            {
                WriteMinMax(writer, polyline);

                int nSubPolyLineCount = polyline.SubPolyLineCount;
                writer.Write(nSubPolyLineCount);

                for (int i = 0; i < nSubPolyLineCount; i++)
                {
                    List<Vertex2FM> vertices = polyline.GetSubPolyLine(i);

                    if (realType == RealType.FLOAT)
                        WriteVerticesF(writer, vertices);
                    else
                        WriteVerticesD(writer, vertices);
                }
            }

            return true;
        }

        protected virtual bool WritePolygonMs(BinaryWriter writer, int nObjectType, RealType realType)
        {
            writer.Write(nObjectType);

            foreach (libShapeFile.PolygonM polygon in m_shapes)
            {
                WriteMinMax(writer, polygon);

                int nSubPolygonCount = polygon.SubPolygonCount;
                writer.Write(nSubPolygonCount);

                for (int i = 0; i < nSubPolygonCount; i++)
                {
                    List<Vertex2FM> vertices = polygon.GetSubPolygon(i);

                    if (realType == RealType.FLOAT)
                        WriteVerticesF(writer, vertices);
                    else
                        WriteVerticesD(writer, vertices);
                }
            }

            return true;
        }

        protected virtual bool WriteMultiPointMs(BinaryWriter writer, int nObjectType, RealType realType)
        {
            writer.Write(nObjectType);

            foreach (libShapeFile.MultiPointM multiPoint in m_shapes)
            {
                WriteMinMax(writer, multiPoint);

                int nPointCount = multiPoint.GetVertexCount();
                writer.Write(nPointCount);

                if (realType == RealType.FLOAT)
                    WriteVerticesF(writer, multiPoint.Vertices);
                else
                    WriteVerticesD(writer, multiPoint.Vertices);
            }

            return true;
        }

        protected void WriteMinMax(BinaryWriter writer, Shape shape)
        {
            writer.Write(shape.MinX);
            writer.Write(shape.MinY);
            writer.Write(shape.MaxX);
            writer.Write(shape.MaxY);
        }

        protected virtual void WriteVerticesD(BinaryWriter writer, List<UnE.Geometry.Vertex2D> vertices)
        {
            writer.Write((long)vertices.Count);

            foreach (UnE.Geometry.Vertex2D vertex in vertices)
            {
                writer.Write(vertex.x);
                writer.Write(vertex.y);
            }
        }

        protected virtual void WriteVerticesD(BinaryWriter writer, List<UnE.Geometry.Vertex2F> vertices)
        {
            writer.Write((long)vertices.Count);

            foreach (UnE.Geometry.Vertex2F vertex in vertices)
            {
                writer.Write((double)vertex.x);
                writer.Write((double)vertex.y);
            }
        }

        protected virtual void WriteVerticesF(BinaryWriter writer, List<UnE.Geometry.Vertex2D> vertices)
        {
            writer.Write((long)vertices.Count);

            foreach (UnE.Geometry.Vertex2D vertex in vertices)
            {
                writer.Write((float)vertex.x);
                writer.Write((float)vertex.y);
            }
        }

        protected virtual void WriteVerticesF(BinaryWriter writer, List<UnE.Geometry.Vertex2F> vertices)
        {
            writer.Write((long)vertices.Count);

            foreach (UnE.Geometry.Vertex2F vertex in vertices)
            {
                writer.Write(vertex.x);
                writer.Write(vertex.y);
            }
        }

        protected virtual void WriteVerticesD(BinaryWriter writer, List<Vertex2FM> vertices)
        {
            writer.Write((long)vertices.Count);

            foreach (Vertex2FM vertex in vertices)
            {
                writer.Write((double)vertex.x);
                writer.Write((double)vertex.y);
                writer.Write(vertex.m);
            }
        }

        protected virtual void WriteVerticesF(BinaryWriter writer, List<Vertex2FM> vertices)
        {
            writer.Write((long)vertices.Count);

            foreach (Vertex2FM vertex in vertices)
            {
                writer.Write(vertex.x);
                writer.Write(vertex.y);
                writer.Write(vertex.m);
            }
        }

        protected virtual void WriteVerticesD(BinaryWriter writer, List<Vertex3FM> vertices)
        {
            writer.Write((long)vertices.Count);

            foreach (Vertex3FM vertex in vertices)
            {
                writer.Write((double)vertex.x);
                writer.Write((double)vertex.y);
                writer.Write((double)vertex.z);
                writer.Write(vertex.m);
            }
        }

        protected virtual void WriteVerticesF(BinaryWriter writer, List<Vertex3FM> vertices)
        {
            writer.Write((long)vertices.Count);

            foreach (Vertex3FM vertex in vertices)
            {
                writer.Write(vertex.x);
                writer.Write(vertex.y);
                writer.Write(vertex.z);
                writer.Write(vertex.m);
            }
        }
    }
}
