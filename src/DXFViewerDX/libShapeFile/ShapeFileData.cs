using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace libShapeFile
{
    /// <summary>
    /// Enumeration representing a ShapeType. Currently supported shape types are Point, PolyLine, Polygon and PolyLineM
    /// </summary>
    public enum ShapeType
    {
        NullShape = 0,
        Point = 1,
        PolyLine = 3,
        Polygon = 5,
        MultiPoint = 8,
        PointZ = 11,
        PolyLineZ = 13,
        PolygonZ = 15,
        MultiPointZ = 18,
        PointM = 21,
        PolyLineM = 23,
        PolygonM = 25,
        MultiPointM = 28,
        MultiPath = 31
    }

    internal class EndianUtils
    {
        private EndianUtils()
        {
        }

        public static byte[] GetBytesBE(int x)
        {
            byte[] b = new byte[4];
            b[3] = (byte)(x & 0xff);
            b[2] = (byte)((x >> 8) & 0xff);
            b[1] = (byte)((x >> 16) & 0xff);
            b[0] = (byte)((x >> 24) & 0xff);
            return b;
        }

        public static int ReadIntBE(byte[] data, int offset)
        {
            int result = data[offset];
            result = (result << 8) | data[offset + 1];
            result = (result << 8) | data[offset + 2];
            result = (result << 8) | data[offset + 3];
            return result;
        }

        public static int ReadIntLE(byte[] data, int offset)
        {
            return BitConverter.ToInt32(data, offset);
        }

        public static double ReadDoubleLE(byte[] data, int offset)
        {
            return BitConverter.ToDouble(data, offset);
        }

        public static float ReadFloatLE(byte[] data, int offset)
        {
            return BitConverter.ToSingle(data, offset);
        }

        /// <summary>
        /// swaps the bytes ordering of the data at offset
        /// ie. bytes offset, ofset+1, offset+2, offset+3 become offset+3, offset+2, offset+1, offset
        /// This can be used to convert a BE int to a LE int and vice-versa
        /// Note that no bounds checks are performed so offset must be &lt;= data.length-4
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        public static void SwapIntBytes(byte[] data, int offset)
        {
            byte temp = data[offset];
            data[offset] = data[offset + 3];
            data[offset + 3] = temp;

            temp = data[offset + 1];
            data[offset + 1] = data[offset + 2];
            data[offset + 2] = temp;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RecordHeader
    {
        public int RecordNumber;
        public int Offset;
        public int ContentLength;

        public RecordHeader(int recNum)
        {
            RecordNumber = recNum;
            ContentLength = 0;
            Offset = 0;
        }

        public void readFromIndexFile(byte[] data, int dataOffset)
        {
            Offset = EndianUtils.ReadIntBE(data, dataOffset) << 1; //offset in bytes
            ContentLength = EndianUtils.ReadIntBE(data, dataOffset + 4) << 1; //*2 because length is in words not bytes
        }
    }

    internal sealed class ShapeFileExConstants
    {
        public const int SHAPE_FILE_EX_MAIN_HEADER_LENGTH = 76;

        //public const int MAX_REC_LENGTH = 1<<23;//1<<20;
        public const int MAX_REC_LENGTH = 1 << 20;//1<<20;
        //public const int MAX_REC_LENGTH = 1 << 20;

        private ShapeFileExConstants()
        {
        }
    }

    public static class Converter<Type>
    {
        public static Type Convert(byte[] bytes)
        {
            return Convert(bytes, 0);
        }

        public static Type Convert(byte[] bytes, int nOffset)
        {
            int nBufSize = Marshal.SizeOf(typeof(Type));
            byte[] buf = new byte[nBufSize];

            Array.Copy(bytes, nOffset, buf, 0, nBufSize);

            GCHandle handle = GCHandle.Alloc(buf, GCHandleType.Pinned);

            Type data = (Type)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(Type));
            handle.Free();

            return data;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct ShapeFileMainHeader
    {
        internal const int MAIN_HEADER_LENGTH = 100;
        public int FileCode;
        public int UnusedByte1;
        public int UnusedByte2;
        public int UnusedByte3;
        public int UnusedByte4;
        public int UnusedByte5;
        public int FileLength;
        public int Version;
        public ShapeType ShapeType;
        public double Xmin;
        public double Ymin;
        public double Xmax;
        public double Ymax;
        public double Zmin;
        public double Zmax;
        public double Mmin;
        public double Mmax;

        public ShapeFileMainHeader(byte[] data)
        {
            //first convert any BE ints in the data to LE
            //swap FileCode
            EndianUtils.SwapIntBytes(data, 0);
            //no need to swap unused bytes
            //swap File Length
            EndianUtils.SwapIntBytes(data, 24);

            this = Converter<ShapeFileMainHeader>.Convert(data);

            //adjust FileLength to be number of bytes (not num words)
            FileLength *= 2;
        }

        public override string ToString()
        {
            string str = "Filecode = " + FileCode + ", FileLength = " + FileLength + ", Version = " + Version + ", ShapeType = " + ShapeType;
            str += ", XMin = " + Xmin + ", Ymin = " + Ymin + ", Xmax = " + Xmax + ", Ymax = " + Ymax + ", MMin = " + Mmin + ", Mmax = " + Mmax;
            return str;
        }

    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct Box
    {
        internal double xmin;
        internal double ymin;
        internal double xmax;
        internal double ymax;

        public Box(byte[] data, int dataOffset)
        {
            xmin = BitConverter.ToDouble(data, dataOffset);
            ymin = BitConverter.ToDouble(data, dataOffset + sizeof(double));
            xmax = BitConverter.ToDouble(data, dataOffset + sizeof(double) * 2);
            ymax = BitConverter.ToDouble(data, dataOffset + sizeof(double) * 3);
        }

        public Box(double xMin, double yMin, double xMax, double yMax)
        {
            xmin = xMin;
            ymin = yMin;
            xmax = xMax;
            ymax = yMax;
        }

        public override string ToString()
        {
            return "{" + xmin + "," + ymin + "," + xmax + "," + ymax + "}";
        }

        public EGIS.ShapeFileLib.RectangleD ToRectangleD()
        {
            return EGIS.ShapeFileLib.RectangleD.FromLTRB(xmin, ymin, xmax, ymax);
        }

        public System.Drawing.RectangleF ToRectangleF()
        {
            return System.Drawing.RectangleF.FromLTRB((float)xmin, (float)ymin, (float)xmax, (float)ymax);
        }

        public double Width
        {
            get
            {
                return xmax - xmin;
            }
        }

        public double Height
        {
            get
            {
                return ymax - ymin;
            }
        }
    }

    public class Vertex2FM : UnE.Geometry.Vertex2F
    {
        public double m;

        public Vertex2FM()
        {
        }

        public Vertex2FM(float x, float y, double m)
            : base(x, y)
        {
            this.m = m;
        }
    }

    public class Vertex3FM : UnE.Geometry.Vertex3F
    {
        public double m;

        public Vertex3FM()
        {
        }

        public Vertex3FM(float x, float y, float z, double m)
            : base(x, y, z)
        {
            this.m = m;
        }
    }
}
