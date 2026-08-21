using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnE.Geometry;
using System.Runtime.InteropServices;

namespace libShapeFile
{
    public class Point : Shape
    {
        private Vertex2F m_vertex = null;

        public Vertex2F Vertex
        {
            get { return m_vertex; }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct SHPoint
        {
            public ShapeType type;
            public double x;
            public double y;
        }

        public Point()
            : base()
        {
            m_vertex = new Vertex2F();
            m_boundingXY = new Box(0.0, 0.0, 0.0, 0.0);
        }

        public Point(double x, double y, int nID)
            : base(nID)
        {
            m_vertex = new Vertex2F((float)x, (float)y);
            m_boundingXY = new Box(x, y, x, y);
        }

        public static bool Load(IFileEventListener listener, FileStream shapeFileStream, RecordHeader[] recordHeaders, List<Shape> shapes)
        {
            shapeFileStream.Seek(ShapeFileMainHeader.MAIN_HEADER_LENGTH, SeekOrigin.Begin);

            int nIndex = 0;
            byte[] buffer = SharedBuffer;

            while (nIndex < recordHeaders.Length)
            {
                if (shapeFileStream.Position != recordHeaders[nIndex].Offset)
                {
                    System.Diagnostics.Trace.WriteLine("offset wrong");
                    shapeFileStream.Seek(recordHeaders[nIndex].Offset, SeekOrigin.Begin);
                }

                shapeFileStream.Read(buffer, 0, recordHeaders[nIndex].ContentLength + 8);

                SHPoint pt = Converter<SHPoint>.Convert(buffer, 8);
                shapes.Add(new Point(pt.x, pt.y, nIndex));

                if (listener != null)
                {
                    listener.ReadEntity("Point", nIndex);
                }

                ++nIndex;
            }

            return true;
        }
    }

    public class PointM : Shape2
    {
        protected Vertex2FM m_vertex = null;

        public Vertex2FM Vertex
        {
            get { return m_vertex; }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct SHPointM
        {
            public ShapeType type;
            public double x;
            public double y;
            public double m;
        }

        public PointM()
            : base()
        {
            m_vertex = new Vertex2FM();
        }

	    public PointM(double x, double y, double m, int nID)
            : base(nID)
        {
            m_vertex = new Vertex2FM((float)x, (float)y, m);
        }

        public static bool Load(IFileEventListener listener, FileStream shapeFileStream, RecordHeader[] recordHeaders, List<Shape> shapes)
        {
            shapeFileStream.Seek(ShapeFileMainHeader.MAIN_HEADER_LENGTH, SeekOrigin.Begin);

            int nIndex = 0;
            byte[] buffer = SharedBuffer;

            while (nIndex < recordHeaders.Length)
            {
                if (shapeFileStream.Position != recordHeaders[nIndex].Offset)
                {
                    System.Diagnostics.Trace.WriteLine("offset wrong");
                    shapeFileStream.Seek(recordHeaders[nIndex].Offset, SeekOrigin.Begin);
                }

                shapeFileStream.Read(buffer, 0, recordHeaders[nIndex].ContentLength + 8);

                SHPointM pt = Converter<SHPointM>.Convert(buffer, 8);
                PointM point = new PointM(pt.x, pt.y, pt.m, nIndex);
                shapes.Add(point);

                point.MinMeasure = pt.m;
                point.MaxMeasure = pt.m;

                if (listener != null)
                {
                    listener.ReadEntity("pointM", nIndex);
                }

                ++nIndex;
            }

            return true;
        }

        public void SetVertex(Vertex2FM vertex)
        {
            m_vertex = vertex;
        }
    }

    public class PointZ : Shape3
    {
        protected Vertex3FM m_vertex = null;

        public Vertex3FM Vertex
        {
            get { return m_vertex; }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct SHPointZ
        {
            public ShapeType type;
            public double x;
            public double y;
            public double z;
            public double m;
        }

        public PointZ()
            : base()
        {
            m_vertex = new Vertex3FM();
        }

        public PointZ(double x, double y, double z, double m, int nID)
            : base(nID)
        {
            m_vertex = new Vertex3FM((float)x, (float)y, (float)z, m);
        }

        public static bool Load(IFileEventListener listener, FileStream shapeFileStream, RecordHeader[] recordHeaders, List<Shape> shapes)
        {
            shapeFileStream.Seek(ShapeFileMainHeader.MAIN_HEADER_LENGTH, SeekOrigin.Begin);

            int nIndex = 0;
            byte[] buffer = SharedBuffer;

            while (nIndex < recordHeaders.Length)
            {
                if (shapeFileStream.Position != recordHeaders[nIndex].Offset)
                {
                    System.Diagnostics.Trace.WriteLine("offset wrong");
                    shapeFileStream.Seek(recordHeaders[nIndex].Offset, SeekOrigin.Begin);
                }

                shapeFileStream.Read(buffer, 0, recordHeaders[nIndex].ContentLength + 8);

                SHPointZ pt = Converter<SHPointZ>.Convert(buffer, 8);
                PointZ point = new PointZ(pt.x, pt.y, pt.z, pt.m, nIndex);
                shapes.Add(point);

                point.MinMeasure = pt.m;
                point.MaxMeasure = pt.m;
                point.MinZ = pt.z;
                point.MaxZ = pt.z;

                if (listener != null)
                {
                    listener.ReadEntity("pointZ", nIndex);
                }

                ++nIndex;
            }

            return true;
        }

        public void SetVertex(Vertex3FM vertex)
        {
            m_vertex = vertex;
        }
    }
}
