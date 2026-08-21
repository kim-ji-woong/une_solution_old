using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Geometry;
using System.IO;

namespace libShapeFile
{
    // PolyLine 클래스는 여러개의 Line List로 이루어진다.
    public class PolyLine : Shape
    {
        private List<List<Vertex2F>> m_polyLines = new List<List<Vertex2F>>();

        public PolyLine()
            : base()
        {
        }

        public PolyLine(int nID)
            : base(nID)
        {
        }

        public int SubPolyLineCount
        {
            get { return m_polyLines.Count(); }
        }

        public List<Vertex2F> GetSubPolyLine(int nIndex)
        {
            if (nIndex >= SubPolyLineCount)
                return null;

            return m_polyLines[nIndex];
        }

        public void AddSubPolyLine(List<Vertex2F> vertices)
        {
            m_polyLines.Add(vertices);
        }

        public void RemoveSubPolyLine(int nIndex)
        {
            if (nIndex >= SubPolyLineCount)
                return;

            m_polyLines.RemoveAt(nIndex);
        }

        public void Clear()
        {
            m_polyLines.Clear();
        }

        public static bool Load(IFileEventListener listener, FileStream shapeFileStream, RecordHeader[] recordHeaders, List<Shape> shapes)
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
                    PolyLine polyLine = new PolyLine(nIndex);
                    polyLine.m_boundingXY = box;
                    shapes.Add(polyLine);

                    List<Vertex2F> vertices = null;

                    for (int j = 0, k = 0; j < numPoints; j++)
                    {
                        if (k < numParts)
                        {
                            if (j == arrParts[k])
                            {
                                vertices = new List<Vertex2F>();
                                polyLine.AddSubPolyLine(vertices);
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
                if (listener != null)
                {
                    listener.ReadEntity("PolyLine", nIndex);
                }
                nIndex++;
            }

            return true;
        }
    }

    public class PolyLineM : Shape2
    {
        private List<List<Vertex2FM>> m_polyLines = new List<List<Vertex2FM>>();

        public PolyLineM()
            : base()
        {
        }

        public PolyLineM(int nID)
            : base(nID)
        {
        }

        public int SubPolyLineCount
        {
            get { return m_polyLines.Count(); }
        }

        public List<Vertex2FM> GetSubPolyLine(int nIndex)
        {
            if (nIndex >= SubPolyLineCount)
                return null;

            return m_polyLines[nIndex];
        }

        public void AddSubPolyLine(List<Vertex2FM> vertices)
        {
            m_polyLines.Add(vertices);
        }

        public void RemoveSubPolyLine(int nIndex)
        {
            if (nIndex >= SubPolyLineCount)
                return;

            m_polyLines.RemoveAt(nIndex);
        }

        public void Clear()
        {
            m_polyLines.Clear();
        }

        public static bool Load(IFileEventListener listener, FileStream shapeFileStream, RecordHeader[] recordHeaders, List<Shape> shapes)
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
                    PolyLineM polyLine = new PolyLineM(nIndex);
                    polyLine.m_boundingXY = box;
                    shapes.Add(polyLine);

                    List<Vertex2FM> vertices = null;

                    for (int j = 0, k = 0; j < numPoints; j++)
                    {
                        if (k < numParts)
                        {
                            if (j == arrParts[k])
                            {
                                vertices = new List<Vertex2FM>();
                                polyLine.AddSubPolyLine(vertices);
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
                    polyLine.MinMeasure = BitConverter.ToDouble(buffer, 0);

                    shapeFileStream.Read(buffer, 0, sizeof(double));
                    polyLine.MaxMeasure = BitConverter.ToDouble(buffer, 0);

                    for (int j = 0, k = 0; j < numPoints; j++)
                    {
                        if (k < numParts)
                        {
                            if (j == arrParts[k])
                            {
                                vertices = polyLine.GetSubPolyLine(k++);

                                if (vertices == null)
                                    return false;
                            }
                        }

                        Vertex2FM vertex = vertices[j - arrParts[k - 1]];
                        
                        shapeFileStream.Read(buffer, 0, sizeof(double));
                        vertex.m = BitConverter.ToDouble(buffer, 0);
                    }
                }
                if (listener != null)
                {
                    listener.ReadEntity("PolyLineM", nIndex);
                }
                nIndex++;
            }

            return true;
        }
    }

    public class PolyLineZ : Shape3
    {
        private List<List<Vertex3FM>> m_polyLines = new List<List<Vertex3FM>>();

        public PolyLineZ()
            : base()
        {
        }

        public PolyLineZ(int nID)
            : base(nID)
        {
        }

        public int SubPolyLineCount
        {
            get { return m_polyLines.Count(); }
        }

        public List<Vertex3FM> GetSubPolyLine(int nIndex)
        {
            if (nIndex >= SubPolyLineCount)
                return null;

            return m_polyLines[nIndex];
        }

        public void AddSubPolyLine(List<Vertex3FM> vertices)
        {
            m_polyLines.Add(vertices);
        }

        public void RemoveSubPolyLine(int nIndex)
        {
            if (nIndex >= SubPolyLineCount)
                return;

            m_polyLines.RemoveAt(nIndex);
        }

        public void Clear()
        {
            m_polyLines.Clear();
        }

        public static bool Load(IFileEventListener listener, FileStream shapeFileStream, RecordHeader[] recordHeaders, List<Shape> shapes)
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
                    PolyLineZ polyLine = new PolyLineZ(nIndex);
                    polyLine.m_boundingXY = box;
                    shapes.Add(polyLine);

                    List<Vertex3FM> vertices = null;

                    for (int j = 0, k = 0; j < numPoints; j++)
                    {
                        if (k < numParts)
                        {
                            if (j == arrParts[k])
                            {
                                vertices = new List<Vertex3FM>();
                                polyLine.AddSubPolyLine(vertices);
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
                    polyLine.MinZ = BitConverter.ToDouble(buffer, 0);

                    shapeFileStream.Read(buffer, 0, sizeof(double));
                    polyLine.MaxZ = BitConverter.ToDouble(buffer, 0);

                    for (int j = 0, k = 0; j < numPoints; j++)
                    {
                        if (k < numParts)
                        {
                            if (j == arrParts[k])
                            {
                                vertices = polyLine.GetSubPolyLine(k++);

                                if (vertices == null)
                                    return false;
                            }
                        }

                        Vertex3FM vertex = vertices[j - arrParts[k - 1]];

                        shapeFileStream.Read(buffer, 0, sizeof(double));
                        vertex.z = (float)BitConverter.ToDouble(buffer, 0);
                    }

                    shapeFileStream.Read(buffer, 0, sizeof(double));
                    polyLine.MinMeasure = BitConverter.ToDouble(buffer, 0);

                    shapeFileStream.Read(buffer, 0, sizeof(double));
                    polyLine.MaxMeasure = BitConverter.ToDouble(buffer, 0);

                    for (int j = 0, k = 0; j < numPoints; j++)
                    {
                        if (k < numParts)
                        {
                            if (j == arrParts[k])
                            {
                                vertices = polyLine.GetSubPolyLine(k++);

                                if (vertices == null)
                                    return false;
                            }
                        }

                        Vertex3FM vertex = vertices[j - arrParts[k - 1]];

                        shapeFileStream.Read(buffer, 0, sizeof(double));
                        vertex.m = BitConverter.ToDouble(buffer, 0);
                    }
                }
                if (listener != null)
                {
                    listener.ReadEntity("PolyLineZ", nIndex);
                }
                nIndex++;
            }

            return true;
        }
    }
}
