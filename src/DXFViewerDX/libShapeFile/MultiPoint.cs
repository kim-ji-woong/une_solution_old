using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnE.Geometry;

namespace libShapeFile
{
    public class MultiPoint : Shape
    {
        private List<Vertex2F> m_vertices = new List<Vertex2F>();

        public MultiPoint()
            : base()
        {
        }

        public MultiPoint(int nID)
            : base(nID)
        {
        }

        public void AddVertex(Vertex2F vertex)
        {
            m_vertices.Add(vertex);
        }

        public int GetVertexCount()
        {
            return m_vertices.Count();
        }

        public Vertex2F GetVertex(int nIndex)
        {
            if (nIndex >= GetVertexCount())
                return null;

            return m_vertices[nIndex];
        }

        public static bool Load(FileStream shapeFileStream, RecordHeader[] recordHeaders, List<Shape> shapes)
        {
            shapeFileStream.Seek(ShapeFileMainHeader.MAIN_HEADER_LENGTH, SeekOrigin.Begin);

            int nIndex = 0;
            byte[] buffer = SharedBuffer;
            RecordHeader[] pgRecs = recordHeaders;
            double x, y;

            while (nIndex < pgRecs.Length)
            {
                if (shapeFileStream.Position != pgRecs[nIndex].Offset)
                {
                    System.Diagnostics.Trace.WriteLine("offset wrong");
                    shapeFileStream.Seek(pgRecs[nIndex].Offset, SeekOrigin.Begin);
                }

                shapeFileStream.Seek(12, SeekOrigin.Current);

                shapeFileStream.Read(buffer, 0, sizeof(double) * 4);
                Box box = Converter<Box>.Convert(buffer);

                shapeFileStream.Read(buffer, 0, sizeof(int));
                int numPoints = BitConverter.ToInt32(buffer, 0);

                if (numPoints > 0)
                {
                    MultiPoint multiPoint = new MultiPoint(nIndex);
                    multiPoint.m_boundingXY = box;
                    shapes.Add(multiPoint);

                    for (int j = 0; j < numPoints; j++)
                    {
                        shapeFileStream.Read(buffer, 0, sizeof(double));
                        x = BitConverter.ToDouble(buffer, 0);

                        shapeFileStream.Read(buffer, 0, sizeof(double));
                        y = BitConverter.ToDouble(buffer, 0);

                        multiPoint.AddVertex(new Vertex2F((float)x, (float)y));
                    }
                }

                nIndex++;
            }

            return true;
        }
    }

    public class MultiPointM : Shape2
    {
        private List<Vertex2FM> m_vertices = new List<Vertex2FM>();

        public MultiPointM()
            : base()
        {
        }

        public MultiPointM(int nID)
            : base(nID)
        {
        }

        public void AddVertex(Vertex2FM vertex)
        {
            m_vertices.Add(vertex);
        }

        public int GetVertexCount()
        {
            return m_vertices.Count();
        }

        public Vertex2FM GetVertex(int nIndex)
        {
            if (nIndex >= m_vertices.Count())
                return null;

            return m_vertices[nIndex];
        }

        public static bool Load(FileStream shapeFileStream, RecordHeader[] recordHeaders, List<Shape> shapes)
        {
            shapeFileStream.Seek(ShapeFileMainHeader.MAIN_HEADER_LENGTH, SeekOrigin.Begin);

            int nIndex = 0;
            byte[] buffer = SharedBuffer;
            RecordHeader[] pgRecs = recordHeaders;
            double x, y;

            while (nIndex < pgRecs.Length)
            {
                if (shapeFileStream.Position != pgRecs[nIndex].Offset)
                {
                    System.Diagnostics.Trace.WriteLine("offset wrong");
                    shapeFileStream.Seek(pgRecs[nIndex].Offset, SeekOrigin.Begin);
                }

                shapeFileStream.Seek(12, SeekOrigin.Current);

                shapeFileStream.Read(buffer, 0, sizeof(double) * 4);
                Box box = Converter<Box>.Convert(buffer);

                shapeFileStream.Read(buffer, 0, sizeof(int));
                int numPoints = BitConverter.ToInt32(buffer, 0);

                if (numPoints > 0)
                {
                    MultiPointM multiPoint = new MultiPointM(nIndex);
                    multiPoint.m_boundingXY = box;
                    shapes.Add(multiPoint);

                    for (int j = 0; j < numPoints; j++)
                    {
                        shapeFileStream.Read(buffer, 0, sizeof(double));
                        x = BitConverter.ToDouble(buffer, 0);

                        shapeFileStream.Read(buffer, 0, sizeof(double));
                        y = BitConverter.ToDouble(buffer, 0);

                        multiPoint.AddVertex(new Vertex2FM((float)x, (float)y, 0));
                    }

                    shapeFileStream.Read(buffer, 0, sizeof(double));
                    multiPoint.MinMeasure = BitConverter.ToDouble(buffer, 0);

                    shapeFileStream.Read(buffer, 0, sizeof(double));
                    multiPoint.MaxMeasure = BitConverter.ToDouble(buffer, 0);

                    for (int j = 0; j < numPoints; j++)
                    {
                        Vertex2FM vertex = multiPoint.GetVertex(j);

                        if (vertex == null)
                            return false;

                        shapeFileStream.Read(buffer, 0, sizeof(double));
                        vertex.m = BitConverter.ToDouble(buffer, 0);
                    }
                }

                nIndex++;
            }

            return true;
        }
    }

    public class MultiPointZ : Shape3
    {
        private List<Vertex3FM> m_vertices = new List<Vertex3FM>();

        public MultiPointZ()
            : base()
        {
        }

        public MultiPointZ(int nID)
            : base(nID)
        {
        }

        public void AddVertex(Vertex3FM vertex)
        {
            m_vertices.Add(vertex);
        }

        public int GetVertexCount()
        {
            return m_vertices.Count();
        }

        public Vertex3FM GetVertex(int nIndex)
        {
            if (nIndex >= m_vertices.Count())
                return null;

            return m_vertices[nIndex];
        }

        public static bool Load(FileStream shapeFileStream, RecordHeader[] recordHeaders, List<Shape> shapes)
        {
            shapeFileStream.Seek(ShapeFileMainHeader.MAIN_HEADER_LENGTH, SeekOrigin.Begin);

            int nIndex = 0;
            byte[] buffer = SharedBuffer;
            RecordHeader[] pgRecs = recordHeaders;
            double x, y;

            while (nIndex < pgRecs.Length)
            {
                if (shapeFileStream.Position != pgRecs[nIndex].Offset)
                {
                    System.Diagnostics.Trace.WriteLine("offset wrong");
                    shapeFileStream.Seek(pgRecs[nIndex].Offset, SeekOrigin.Begin);
                }

                shapeFileStream.Seek(12, SeekOrigin.Current);

                shapeFileStream.Read(buffer, 0, sizeof(double) * 4);
                Box box = Converter<Box>.Convert(buffer);

                shapeFileStream.Read(buffer, 0, sizeof(int));
                int numPoints = BitConverter.ToInt32(buffer, 0);

                if (numPoints > 0)
                {
                    MultiPointZ multiPoint = new MultiPointZ(nIndex);
                    multiPoint.m_boundingXY = box;
                    shapes.Add(multiPoint);

                    for (int j = 0; j < numPoints; j++)
                    {
                        shapeFileStream.Read(buffer, 0, sizeof(double));
                        x = BitConverter.ToDouble(buffer, 0);

                        shapeFileStream.Read(buffer, 0, sizeof(double));
                        y = BitConverter.ToDouble(buffer, 0);

                        multiPoint.AddVertex(new Vertex3FM((float)x, (float)y, 0, 0));
                    }

                    shapeFileStream.Read(buffer, 0, sizeof(double));
                    multiPoint.MinZ = BitConverter.ToDouble(buffer, 0);

                    shapeFileStream.Read(buffer, 0, sizeof(double));
                    multiPoint.MaxZ = BitConverter.ToDouble(buffer, 0);

                    for (int j = 0; j < numPoints; j++)
                    {
                        Vertex3FM vertex = multiPoint.GetVertex(j);

                        if (vertex == null)
                            return false;

                        shapeFileStream.Read(buffer, 0, sizeof(double));
                        vertex.z = (float)BitConverter.ToDouble(buffer, 0);
                    }

                    shapeFileStream.Read(buffer, 0, sizeof(double));
                    multiPoint.MinMeasure = BitConverter.ToDouble(buffer, 0);

                    shapeFileStream.Read(buffer, 0, sizeof(double));
                    multiPoint.MaxMeasure = BitConverter.ToDouble(buffer, 0);

                    for (int j = 0; j < numPoints; j++)
                    {
                        Vertex3FM vertex = multiPoint.GetVertex(j);

                        if (vertex == null)
                            return false;

                        shapeFileStream.Read(buffer, 0, sizeof(double));
                        vertex.m = BitConverter.ToDouble(buffer, 0);
                    }
                }

                nIndex++;
            }

            return true;
        }
    }
}
