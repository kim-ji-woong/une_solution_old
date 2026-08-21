using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Geometry;
using System.IO;
using System.Runtime.InteropServices;

namespace libShapeFile
{
    // Polygon 클래스는 여러개의 폐곡선으로 이루어진다.
    // 즉, 외부 Polygon안에 여러개의 구멍이 있는 모습으로 만들어질수 있다.
    // 각 Polygon들은 서로 교차하지 않으며, Vertex에서 만날수는 있다.
    public class Polygon : Shape
    {
        private List<List<Vertex2F>> m_polygons = new List<List<Vertex2F>>();

        public Polygon()
            : base()
        {
        }

        public Polygon(int nID)
            : base(nID)
        {
        }

        public int SubPolygonCount
        {
            get { return m_polygons.Count(); }
        }

        public List<Vertex2F> GetSubPolygon(int nIndex)
        {
            if (nIndex >= SubPolygonCount)
                return null;

            return m_polygons[nIndex];
        }

        public void AddSubPolygon(List<Vertex2F> vertices)
        {
            m_polygons.Add(vertices);
        }

        public void RemoveSubPolygon(int nIndex)
        {
            if (nIndex >= SubPolygonCount)
                return;

            m_polygons.RemoveAt(nIndex);
        }

        public void Clear()
        {
            m_polygons.Clear();
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
                int numParts = BitConverter.ToInt32(buffer, 0);

                shapeFileStream.Read(buffer, 0, sizeof(int));
                int numPoints = BitConverter.ToInt32(buffer, 0);

                int[] arrParts = null;

                if (numParts > 0)
                {
                    arrParts = new int[numParts];
                    shapeFileStream.Read(buffer, 0, sizeof(int) * numParts);
                }
                else
                    arrParts = null;

                for (int j = 0; j < numParts; ++j)
                    arrParts[j] = BitConverter.ToInt32(buffer, j * sizeof(int));

                if (numParts > 0)
                {
                    Polygon polygon = new Polygon(nIndex);
                    polygon.m_boundingXY = box;
                    shapes.Add(polygon);

                    List<Vertex2F> vertices = null;

                    for (int j = 0, k = 0; j < numPoints; j++)
                    {
                        if (k < numParts)
                        {
                            if (j == arrParts[k])
                            {
                                vertices = new List<Vertex2F>();
                                polygon.AddSubPolygon(vertices);
                                k++;
                            }
                        }

                        shapeFileStream.Read(buffer, 0, sizeof(double));
                        x = BitConverter.ToDouble(buffer, 0);

                        shapeFileStream.Read(buffer, 0, sizeof(double));
                        y = BitConverter.ToDouble(buffer, 0);

                        vertices.Add(new Vertex2F((float)x, (float)y));
                    }
                }

                nIndex++;
            }

            return true;
        }
    }

    public class PolygonM : Shape2
    {
        private List<List<Vertex2FM>> m_Polygons = new List<List<Vertex2FM>>();

        public PolygonM()
            : base()
        {
        }

        public PolygonM(int nID)
            : base(nID)
        {
        }

        public int SubPolygonCount
        {
            get { return m_Polygons.Count(); }
        }

        public List<Vertex2FM> GetSubPolygon(int nIndex)
        {
            if (nIndex >= SubPolygonCount)
                return null;

            return m_Polygons[nIndex];
        }

        public void AddSubPolygon(List<Vertex2FM> vertices)
        {
            m_Polygons.Add(vertices);
        }

        public void RemoveSubPolygon(int nIndex)
        {
            if (nIndex >= SubPolygonCount)
                return;

            m_Polygons.RemoveAt(nIndex);
        }

        public void Clear()
        {
            m_Polygons.Clear();
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
                int numParts = BitConverter.ToInt32(buffer, 0);

                shapeFileStream.Read(buffer, 0, sizeof(int));
                int numPoints = BitConverter.ToInt32(buffer, 0);

                int[] arrParts = null;

                if (numParts > 0)
                {
                    arrParts = new int[numParts];
                    shapeFileStream.Read(buffer, 0, sizeof(int) * numParts);
                }
                else
                    arrParts = null;

                for (int j = 0; j < numParts; ++j)
                    arrParts[j] = BitConverter.ToInt32(buffer, j * sizeof(int));

                if (numParts > 0)
                {
                    PolygonM Polygon = new PolygonM(nIndex);
                    Polygon.m_boundingXY = box;
                    shapes.Add(Polygon);

                    List<Vertex2FM> vertices = null;

                    for (int j = 0, k = 0; j < numPoints; j++)
                    {
                        if (k < numParts)
                        {
                            if (j == arrParts[k])
                            {
                                vertices = new List<Vertex2FM>();
                                Polygon.AddSubPolygon(vertices);
                                k++;
                            }
                        }

                        shapeFileStream.Read(buffer, 0, sizeof(double));
                        x = BitConverter.ToDouble(buffer, 0);

                        shapeFileStream.Read(buffer, 0, sizeof(double));
                        y = BitConverter.ToDouble(buffer, 0);

                        vertices.Add(new Vertex2FM((float)x, (float)y, 0));
                    }

                    shapeFileStream.Read(buffer, 0, sizeof(double));
                    Polygon.MinMeasure = BitConverter.ToDouble(buffer, 0);

                    shapeFileStream.Read(buffer, 0, sizeof(double));
                    Polygon.MaxMeasure = BitConverter.ToDouble(buffer, 0);

                    for (int j = 0, k = 0; j < numPoints; j++)
                    {
                        if (k < numParts)
                        {
                            if (j == arrParts[k])
                            {
                                vertices = Polygon.GetSubPolygon(k++);

                                if (vertices == null)
                                    return false;
                            }
                        }

                        Vertex2FM vertex = vertices[j - arrParts[k - 1]];

                        shapeFileStream.Read(buffer, 0, sizeof(double));
                        vertex.m = BitConverter.ToDouble(buffer, 0);
                    }
                }

                nIndex++;
            }

            return true;
        }
    }

    public class PolygonZ : Shape3
    {
        private List<List<Vertex3FM>> m_Polygons = new List<List<Vertex3FM>>();

        public PolygonZ()
            : base()
        {
        }

        public PolygonZ(int nID)
            : base(nID)
        {
        }

        public int SubPolygonCount
        {
            get { return m_Polygons.Count(); }
        }

        public List<Vertex3FM> GetSubPolygon(int nIndex)
        {
            if (nIndex >= SubPolygonCount)
                return null;

            return m_Polygons[nIndex];
        }

        public void AddSubPolygon(List<Vertex3FM> vertices)
        {
            m_Polygons.Add(vertices);
        }

        public void RemoveSubPolygon(int nIndex)
        {
            if (nIndex >= SubPolygonCount)
                return;

            m_Polygons.RemoveAt(nIndex);
        }

        public void Clear()
        {
            m_Polygons.Clear();
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
                int numParts = BitConverter.ToInt32(buffer, 0);

                shapeFileStream.Read(buffer, 0, sizeof(int));
                int numPoints = BitConverter.ToInt32(buffer, 0);

                int[] arrParts = null;

                if (numParts > 0)
                {
                    arrParts = new int[numParts];
                    shapeFileStream.Read(buffer, 0, sizeof(int) * numParts);
                }
                else
                    arrParts = null;

                for (int j = 0; j < numParts; ++j)
                    arrParts[j] = BitConverter.ToInt32(buffer, j * sizeof(int));

                if (numParts > 0)
                {
                    PolygonZ Polygon = new PolygonZ(nIndex);
                    Polygon.m_boundingXY = box;
                    shapes.Add(Polygon);

                    List<Vertex3FM> vertices = null;

                    for (int j = 0, k = 0; j < numPoints; j++)
                    {
                        if (k < numParts)
                        {
                            if (j == arrParts[k])
                            {
                                vertices = new List<Vertex3FM>();
                                Polygon.AddSubPolygon(vertices);
                                k++;
                            }
                        }

                        shapeFileStream.Read(buffer, 0, sizeof(double));
                        x = BitConverter.ToDouble(buffer, 0);

                        shapeFileStream.Read(buffer, 0, sizeof(double));
                        y = BitConverter.ToDouble(buffer, 0);

                        vertices.Add(new Vertex3FM((float)x, (float)y, 0, 0));
                    }

                    shapeFileStream.Read(buffer, 0, sizeof(double));
                    Polygon.MinZ = BitConverter.ToDouble(buffer, 0);

                    shapeFileStream.Read(buffer, 0, sizeof(double));
                    Polygon.MaxZ = BitConverter.ToDouble(buffer, 0);

                    for (int j = 0, k = 0; j < numPoints; j++)
                    {
                        if (k < numParts)
                        {
                            if (j == arrParts[k])
                            {
                                vertices = Polygon.GetSubPolygon(k++);

                                if (vertices == null)
                                    return false;
                            }
                        }

                        Vertex3FM vertex = vertices[j - arrParts[k - 1]];

                        shapeFileStream.Read(buffer, 0, sizeof(double));
                        vertex.z = (float)BitConverter.ToDouble(buffer, 0);
                    }

                    shapeFileStream.Read(buffer, 0, sizeof(double));
                    Polygon.MinMeasure = BitConverter.ToDouble(buffer, 0);

                    shapeFileStream.Read(buffer, 0, sizeof(double));
                    Polygon.MaxMeasure = BitConverter.ToDouble(buffer, 0);

                    for (int j = 0, k = 0; j < numPoints; j++)
                    {
                        if (k < numParts)
                        {
                            if (j == arrParts[k])
                            {
                                vertices = Polygon.GetSubPolygon(k++);

                                if (vertices == null)
                                    return false;
                            }
                        }

                        Vertex3FM vertex = vertices[j - arrParts[k - 1]];

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
