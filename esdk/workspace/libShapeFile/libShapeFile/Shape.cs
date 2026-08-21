using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libShapeFile
{
    public abstract class Shape
    {
        internal static bool SingleThreaded = true;

        #region shared buffers
        private static byte[] sharedBuffer = new byte[ShapeFileExConstants.MAX_REC_LENGTH];

        private static Point[] sharedPointBuffer = new Point[ShapeFileExConstants.MAX_REC_LENGTH / 8];

        internal static byte[] SharedBuffer
        {
            get
            {
                if (SingleThreaded) return sharedBuffer;
                else return new byte[ShapeFileExConstants.MAX_REC_LENGTH];
            }
        }

        internal static Point[] SharedPointBuffer
        {
            get
            {
                if (SingleThreaded) return sharedPointBuffer;
                else return new Point[ShapeFileExConstants.MAX_REC_LENGTH / 8];
            }
        }

        internal static void EnsureBufferSize(int requiredSize)
        {
            if (sharedBuffer.Length < requiredSize)
            {
                sharedBuffer = new byte[requiredSize + 256];
                sharedPointBuffer = new Point[sharedBuffer.Length / 8];
                //System.Diagnostics.Debug.WriteLine("shared buffer resized to : " + requiredSize);
                System.GC.Collect();
            }
        }
        #endregion

        internal Box m_boundingXY;
        protected int m_nID = -1;

        public double MinX
        {
            get { return m_boundingXY.xmin; }
        }

        public double MaxX
        {
            get { return m_boundingXY.xmax; }
        }

        public double MinY
        {
            get { return m_boundingXY.ymin; }
        }

        public double MaxY
        {
            get { return m_boundingXY.ymax; }
        }

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public Shape()
        {
        }

        public Shape(int nID)
        {
            m_nID = nID;
        }
    }

    public class Shape2 : Shape
    {
        protected double m_minMeasure;
        protected double m_maxMeasure;

        public double MinMeasure
        {
            get { return m_minMeasure; }
            set { m_minMeasure = value; }
        }

        public double MaxMeasure
        {
            get { return m_maxMeasure; }
            set { m_maxMeasure = value; }
        }

        public Shape2()
        {
        }

        public Shape2(int nID)
            : base(nID)
        {
        }
    }

    public class Shape3 : Shape2
    {
        protected double m_minZ;
        protected double m_maxZ;

        public double MinZ
        {
            get { return m_minZ; }
            set { m_minZ = value; }
        }

        public double MaxZ
        {
            get { return m_maxZ; }
            set { m_maxZ = value; }
        }

        public Shape3()
        {
        }

        public Shape3(int nID)
            : base(nID)
        {
        }
    }
}
