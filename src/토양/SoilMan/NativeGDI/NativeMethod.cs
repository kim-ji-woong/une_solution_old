#region Copyright and License

/****************************************************************************
**
** Copyright (C) 2008 - 2011 Winston Fletcher.
** All rights reserved.
**
** This file is part of the EGIS.ShapeFileLib class library of Easy GIS .NET.
** 
** Easy GIS .NET is free software: you can redistribute it and/or modify
** it under the terms of the GNU Lesser General Public License version 3 as
** published by the Free Software Foundation and appearing in the file
** lgpl-license.txt included in the packaging of this file.
**
** Easy GIS .NET is distributed in the hope that it will be useful,
** but WITHOUT ANY WARRANTY; without even the implied warranty of
** MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
** GNU General Public License for more details.
**
** You should have received a copy of the GNU General Public License and
** GNU Lesser General Public License along with Easy GIS .NET.
** If not, see <http://www.gnu.org/licenses/>.
**
****************************************************************************/

#endregion

using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Xml;
using System.Security.Permissions;


[assembly: CLSCompliant(true)]
//give the EGIS.Controls access to the internal methods
//[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("EGIS.Controls,     PublicKey=0024000004800000940000000602000000240000525341310004000001000100ad413f7f4a7f27fbb045d205cfc65fe64665694533fc72b0d82433368f98f7bd82c18b98ee2f5fe417ed1427a9e6ff84e5dce034638bb7761ea22c9881b8fa09ac621ad78ebb3002b3dbb876f479fa0b2bccd95fc1d54c7fc87b5dc084d575fb304387c9bbd4ce6a5bf91328ae3ecc3f5472a14ce8e572d7d01d01483fe1f2d0")]
//[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("EGIS.Web.Controls, PublicKey=0024000004800000940000000602000000240000525341310004000001000100ad413f7f4a7f27fbb045d205cfc65fe64665694533fc72b0d82433368f98f7bd82c18b98ee2f5fe417ed1427a9e6ff84e5dce034638bb7761ea22c9881b8fa09ac621ad78ebb3002b3dbb876f479fa0b2bccd95fc1d54c7fc87b5dc084d575fb304387c9bbd4ce6a5bf91328ae3ecc3f5472a14ce8e572d7d01d01483fe1f2d0")]
namespace NativeGDI
{

    #region GDI Utils and Native Methods

    public sealed class NativeMethods
    {
        public static int ColorToGDIColor(Color c)
        {
            int color = 0;
            color = c.B & 0xff;
            color = (color << 8) | (c.G & 0xff);
            color = (color << 8) | (c.R & 0xff);
            return color;
        }

        private NativeMethods() { }

        /// <summary>
        /// constant representing the OPAQUE Background Mode
        /// </summary>
        public const int OPAQUE = 2;

        /// <summary>
        /// constant representing the TRANSPARENT Background Mode
        /// </summary>
        public const int TRANSPARENT = 1;

        // Pen Style constants
        public const int PS_SOLID = 0;
        public const int PS_DASH = 1;
        public const int PS_DOT = 2;
        public const int PS_DASHDOT = 3;
        public const int NULL_BRUSH = 5;

        //BitBlt constants
        public const int SRCCOPY = 0xcc0020;

        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        public static unsafe extern int Polyline(IntPtr hdc, Point* points, int count);

        public static unsafe void DrawPolyline(IntPtr hdc, Point[] points, int count)
        {
            fixed (Point* ptr = points)
            {
                Polyline(hdc, ptr, count);
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        public static extern unsafe int Polygon(IntPtr hdc, Point* points, int count);

        public static unsafe void DrawPolygon(IntPtr hdc, Point[] points, int count)
        {
            fixed (Point* ptr = points)
            {
                Polygon(hdc, ptr, count);
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        public static extern int Ellipse(IntPtr hdc, int left, int top, int right, int bottom);

        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr gdiobj);

        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        public static extern IntPtr GetStockObject(int index);

        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        public static extern IntPtr CreatePen(int fnPenStyle, int nWidth, int rgbColor);

        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        public static extern IntPtr CreateSolidBrush(int rgbColor);

        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        public static extern int DeleteObject(IntPtr gdiobj);

        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        public static extern int SetBkMode(IntPtr hdc, int mode);


        internal const uint PAGE_READONLY = 0x02;
        internal const uint PAGE_READWRITE = 0x04;


        [DllImport("kernel32.dll", SetLastError = true)]
        static extern Microsoft.Win32.SafeHandles.SafeFileHandle CreateFile(string lpFileName, uint dwDesiredAccess,
          uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition,
          uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr CreateFileMapping(Microsoft.Win32.SafeHandles.SafeFileHandle hFile, IntPtr lpAttributes,
            uint flProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName);

        internal static IntPtr MapFile(System.IO.FileStream fs)
        {
            return CreateFileMapping(fs.SafeFileHandle, IntPtr.Zero, /*fs.CanWrite ? PAGE_READWRITE :*/ PAGE_READONLY, 0, 0, null);
        }

        internal enum FileMapAccess { FILE_MAP_WRITE = 0x02, FILE_MAP_READ = 0x04 };

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, FileMapAccess dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, uint dwNumberOfBytesToMap);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int UnmapViewOfFile(IntPtr lpBaseAddress);

        //HANDLE CreateFileMapping(HANDLE hFile,LPSECURITY_ATTRIBUTES lpAttributes, DWORD flProtect, DWORD dwMaximumSizeHigh, DWORD dwMaximumSizeLow, LPCTSTR lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int CloseHandle(IntPtr handle);

        //[DllImport("gdipluslib.dll")]
        //internal static extern IntPtr CreateGraphics(IntPtr hdc);

        //[DllImport("gdipluslib.dll")]
        //internal static extern IntPtr CreateGraphicsFromImage(ref Bitmap bm);


        //[DllImport("gdipluslib.dll")]
        //internal static extern void ReleaseGraphics(IntPtr graphics);

        //[DllImport("gdipluslib.dll")]
        ////internal static extern void DrawLines(ref Graphics graphics, IntPtr pen, Point[] points, int count);
        //internal static extern void DrawLines(IntPtr graphics, IntPtr pen, Point[] points, int count);

        //[DllImport("gdipluslib.dll")]
        //internal static extern IntPtr CreateGdiplusPen(int color, float width);

        //[DllImport("gdipluslib.dll")]
        //internal static extern void ReleaseGdiplusPen(IntPtr pen);

        //        internal static unsafe void TestGeometry()
        //{
        //    double[] pts = {0,0, 0,10, 5,10, 5,5, 1,5, 1,0, 0,0};

        //            fixed(double* ptr = pts)
        //            {

        //    bool intersects;
        //    intersects = PolygonRectIntersect(ptr, 7, 0,0, 10,11);

        //    Console.Out.WriteLine("TestGeometry test1 pass:" + intersects);

        //    intersects = PolygonRectIntersect(ptr, 7, 100,0, 110,10);
        //    Console.Out.WriteLine("TestGeometry test2 pass:" + !intersects);

        //    intersects = PolygonRectIntersect(ptr, 7, 2,0, 10,4);
        //    Console.Out.WriteLine("TestGeometry test3 pass:" + !intersects);

        //            }
        //}
        static float point2SegDist2(float x, float y, PointF A, PointF B)
        {
            float dx = B.X - A.X;
            float dy = B.Y - A.Y;

            float lenAB2 = dx * dx + dy * dy;

            float du = x - A.X;
            float dv = y - A.Y;
            float dot = dx * du + dy * dv;
            if (lenAB2 == 0.0)
                return du * du + dv * dv;
            if (dot <= 0.0)
            {
                return du * du + dv * dv;
            }
            else if (dot >= lenAB2)
            {
                du = x - B.X;
                dy = y - B.Y;
                return du * du + dv * dv;
            }
            else
            {
                float slash = du * dy - dv * dx;
                return slash * slash / lenAB2;
            }
        }

        /// <span class="code-SummaryComment"><summary></span>
        /// Uses the Douglas Peucker algorithm to reduce the number of points.
        /// <span class="code-SummaryComment"></summary></span>
        /// <span class="code-SummaryComment"><param name="Points">The points.</param></span>
        /// <span class="code-SummaryComment"><param name="Tolerance">The tolerance.</param></span>
        /// <span class="code-SummaryComment"><returns></returns></span>
        public static List<PointF> DouglasPeuckerReduction(List<PointF> Points, Double Tolerance)
        {
            if (Points == null || Points.Count < 5)
            {
                return Points;
            }

            Int32 firstPoint = 0;
            Int32 lastPoint = Points.Count - 1;
            List<Int32> pointIndexsToKeep = new List<Int32>();

            //Add the first and last index to the keepers
            pointIndexsToKeep.Add(firstPoint);
            pointIndexsToKeep.Add(lastPoint);

            //The first and the last point cannot be the same
            while (Points[firstPoint].Equals(Points[lastPoint]))
            {
                lastPoint--;
            }

            DouglasPeuckerReduction(Points, firstPoint, lastPoint,
            Tolerance, ref pointIndexsToKeep);

            List<PointF> returnPoints = new List<PointF>();
            pointIndexsToKeep.Sort();
            foreach (Int32 index in pointIndexsToKeep)
            {
                returnPoints.Add(Points[index]);
            }

            return returnPoints;
        }

        public static List<Point> DouglasPeuckerReduction(List<Point> Points, Double Tolerance)
        {
            if (Points == null || Points.Count < 4)
            {
                return Points;
            }

            Int32 firstPoint = 0;
            Int32 lastPoint = Points.Count - 1;
            List<Int32> pointIndexsToKeep = new List<Int32>();

            //Add the first and last index to the keepers
            pointIndexsToKeep.Add(firstPoint);
            pointIndexsToKeep.Add(lastPoint);


            List<Point> returnPoints = new List<Point>();
            //The first and the last point cannot be the same
            while (Points[firstPoint].Equals(Points[lastPoint]))
            {
                lastPoint--;

                // 모두 동일한 점인 경우 
                if (lastPoint < 0)
                {
                    return Points;
                }
            }

            DouglasPeuckerReduction(Points, firstPoint, lastPoint,
            Tolerance, ref pointIndexsToKeep);

            pointIndexsToKeep.Sort();
            foreach (Int32 index in pointIndexsToKeep)
            {
                returnPoints.Add(Points[index]);
            }

            return returnPoints;
        }

        /// <span class="code-SummaryComment"><summary></span>
        /// Douglases the peucker reduction.
        /// <span class="code-SummaryComment"></summary></span>
        /// <span class="code-SummaryComment"><param name="points">The points.</param></span>
        /// <span class="code-SummaryComment"><param name="firstPoint">The first point.</param></span>
        /// <span class="code-SummaryComment"><param name="lastPoint">The last point.</param></span>
        /// <span class="code-SummaryComment"><param name="tolerance">The tolerance.</param></span>
        /// <span class="code-SummaryComment"><param name="pointIndexsToKeep">The point index to keep.</param></span>
        private static void DouglasPeuckerReduction(List<PointF>
            points, Int32 firstPoint, Int32 lastPoint, Double tolerance,
            ref List<Int32> pointIndexsToKeep)
        {
            Double maxDistance = 0;
            Int32 indexFarthest = 0;

            for (Int32 index = firstPoint; index < lastPoint; index++)
            {
                Double distance = PerpendicularDistance
                    (points[firstPoint], points[lastPoint], points[index]);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    indexFarthest = index;
                }
            }

            if (maxDistance > tolerance && indexFarthest != 0)
            {
                //Add the largest point that exceeds the tolerance
                pointIndexsToKeep.Add(indexFarthest);

                DouglasPeuckerReduction(points, firstPoint,
                indexFarthest, tolerance, ref pointIndexsToKeep);
                DouglasPeuckerReduction(points, indexFarthest,
                lastPoint, tolerance, ref pointIndexsToKeep);
            }
        }

        private static void DouglasPeuckerReduction(List<Point>
            points, Int32 firstPoint, Int32 lastPoint, Double tolerance,
            ref List<Int32> pointIndexsToKeep)
        {
            Double maxDistance = 0;
            Int32 indexFarthest = 0;

            for (Int32 index = firstPoint; index < lastPoint; index++)
            {
                Double distance = PerpendicularDistance
                    (points[firstPoint], points[lastPoint], points[index]);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    indexFarthest = index;
                }
            }

            if (maxDistance > tolerance && indexFarthest != 0)
            {
                //Add the largest point that exceeds the tolerance
                pointIndexsToKeep.Add(indexFarthest);

                DouglasPeuckerReduction(points, firstPoint,
                indexFarthest, tolerance, ref pointIndexsToKeep);
                DouglasPeuckerReduction(points, indexFarthest,
                lastPoint, tolerance, ref pointIndexsToKeep);
            }
        }

        /// <span class="code-SummaryComment"><summary></span>
        /// The distance of a point from a line made from point1 and point2.
        /// <span class="code-SummaryComment"></summary></span>
        /// <span class="code-SummaryComment"><param name="pt1">The PT1.</param></span>
        /// <span class="code-SummaryComment"><param name="pt2">The PT2.</param></span>
        /// <span class="code-SummaryComment"><param name="p">The p.</param></span>
        /// <span class="code-SummaryComment"><returns></returns></span>
        public static Double PerpendicularDistance
            (PointF Point1, PointF Point2, PointF Point)
        {
            //Area = |(1/2)(x1y2 + x2y3 + x3y1 - x2y1 - x3y2 - x1y3)|   *Area of triangle
            //Base = v((x1-x2)²+(x1-x2)²)                               *Base of Triangle*
            //Area = .5*Base*H                                          *Solve for height
            //Height = Area/.5/Base

            Double area = Math.Abs(.5 * (Point1.X * Point2.Y + Point2.X *
            Point.Y + Point.X * Point1.Y - Point2.X * Point1.Y - Point.X *
            Point2.Y - Point1.X * Point.Y));
            Double bottom = Math.Sqrt(Math.Pow(Point1.X - Point2.X, 2) +
            Math.Pow(Point1.Y - Point2.Y, 2));
            Double height = area / bottom * 2;

            return height;

            //Another option
            //Double A = Point.X - Point1.X;
            //Double B = Point.Y - Point1.Y;
            //Double C = Point2.X - Point1.X;
            //Double D = Point2.Y - Point1.Y;

            //Double dot = A * C + B * D;
            //Double len_sq = C * C + D * D;
            //Double param = dot / len_sq;

            //Double xx, yy;

            //if (param < 0)
            //{
            //    xx = Point1.X;
            //    yy = Point1.Y;
            //}
            //else if (param > 1)
            //{
            //    xx = Point2.X;
            //    yy = Point2.Y;
            //}
            //else
            //{
            //    xx = Point1.X + param * C;
            //    yy = Point1.Y + param * D;
            //}

            //Double d = DistanceBetweenOn2DPlane(Point, new Point(xx, yy));
        }


        //  재귀호출을 이용한 Douglas-Peucker 알고리즘;
        public static void DouglasPeucker(double tolerance, PointF[] Vertex, int istart, int iend, int[] mark)
        {
            if (iend <= istart + 1) // 종료조건;
                return;

            // Vertex[istart] to Vertex[iend]을 잇는 선분을 기준으로 분할지점을 찾음;
            int ibreak = istart; // 선분에서 가장 먼 꼭지점;
            double maxdist2 = 0.0; // 가장 먼 꼭지점까지 거리제곱;
            double tol2 = tolerance * tolerance; // 임계값의 제곱;
            // 주어진 선분에서 가장 먼 꼭지점을 찾는다;
            for (int i = istart + 1; i < iend; i++)
            {
                double dist2 = point2SegDist2(Vertex[i].X, Vertex[i].Y, Vertex[istart], Vertex[iend]);
                // test with current max distance squared
                if (dist2 <= maxdist2)
                    continue;
                // 현재까지 가장 먼 꼭지점;
                ibreak = i;
                maxdist2 = dist2;
            }
            if (maxdist2 > tol2)
            {
                // 가장 먼 꼭지점까지 거리가 임계값을 넘으면==> 분할;
                mark[ibreak] = 1;      // Vertex[ibreak]를 마킹;
                // 재귀적으로 Vertex[ibreak] 좌/우를 분할시도;
                DouglasPeucker(tolerance, Vertex, istart, ibreak, mark);  //polyline Vertex[istart] to Vertex[ibreak]
                DouglasPeucker(tolerance, Vertex, ibreak, iend, mark);  //polyline Vertex[ibreak] to Vertex[iend]
            }
            return;
        }




        //class NativeGeomUtilWin32
        //{
        //    [DllImport("geomutil_lib.dll", CallingConvention = CallingConvention.Cdecl)]
        //    internal static unsafe extern int SimplifyDouglasPeuckerInt(int* input, int inputCount, int tolerance, int* output, ref int outputCount);

        //    [DllImport("geomutil_lib.dll", CallingConvention = CallingConvention.Cdecl)]
        //    internal static unsafe extern int PolygonRectIntersect(void* points, int pointCount, double rMinX, double rMinY, double rMaxX, double rMaxY);

        //    [DllImport("geomutil_lib.dll", CallingConvention = CallingConvention.Cdecl)]
        //    internal static unsafe extern int PolyLineRectIntersect(void* points, int pointCount, double rMinX, double rMinY, double rMaxX, double rMaxY);

        //}

        //class NativeGeomUtilX64
        //{
        //    [DllImport("geomutil_libx64.dll", CallingConvention = CallingConvention.Cdecl)]
        //    internal static unsafe extern int SimplifyDouglasPeuckerInt(int* input, int inputCount, int tolerance, int* output, ref int outputCount);

        //    [DllImport("geomutil_libx64.dll", CallingConvention = CallingConvention.Cdecl)]
        //    internal static unsafe extern int PolygonRectIntersect(void* points, int pointCount, double rMinX, double rMinY, double rMaxX, double rMaxY);

        //    [DllImport("geomutil_libx64.dll", CallingConvention = CallingConvention.Cdecl)]
        //    internal static unsafe extern int PolyLineRectIntersect(void* points, int pointCount, double rMinX, double rMinY, double rMaxX, double rMaxY);

        //}


        static bool IsWin32Process()
        {
            return (IntPtr.Size == 4);
        }

        //internal static unsafe int SimplifyDouglasPeuckerInt(int* input, int inputCount, int tolerance, int* output, ref int outputCount)
        //{
        //    if (IsWin32Process())
        //    {
        //        return NativeGeomUtilWin32.SimplifyDouglasPeuckerInt(input, inputCount, tolerance, output, ref outputCount);
        //    }
        //    return NativeGeomUtilX64.SimplifyDouglasPeuckerInt(input, inputCount, tolerance, output, ref outputCount);

        //}


        //internal static unsafe bool PolygonRectIntersect(void* points, int pointCount, double rMinX, double rMinY, double rMaxX, double rMaxY)
        //{
        //    if (IsWin32Process())
        //    {
        //        return (NativeGeomUtilWin32.PolygonRectIntersect(points, pointCount, rMinX, rMinY, rMaxX, rMaxY) != 0);
        //    }
        //    return (NativeGeomUtilX64.PolygonRectIntersect(points, pointCount, rMinX, rMinY, rMaxX, rMaxY) != 0);
        //}

        //internal static unsafe bool PolyLineRectIntersect(void* points, int pointCount, double rMinX, double rMinY, double rMaxX, double rMaxY)
        //{
        //    if (IsWin32Process())
        //    {
        //        int c = NativeGeomUtilWin32.PolyLineRectIntersect(points, pointCount, rMinX, rMinY, rMaxX, rMaxY);
        //        //Console.Out.WriteLine("c = " + c);
        //        return c != 0;
        //    }
        //    return NativeGeomUtilX64.PolyLineRectIntersect(points, pointCount, rMinX, rMinY, rMaxX, rMaxY) != 0;
        //}

        //internal static unsafe int SimplifyDouglasPeucker(System.Drawing.Point[] input, int inputCount, int tolerance, Point[] output, ref int outputCount)
        //{
        //    fixed (Point* inputPtr = input)
        //    {
        //        fixed (Point* outputPtr = output)
        //        {
        //            return SimplifyDouglasPeuckerInt((int*)inputPtr, inputCount, tolerance, (int*)outputPtr, ref outputCount);
        //        }
        //    }
        //}


        //internal static unsafe bool PolygonRectIntersect(double[] data, int dataLength, double rMinX, double rMinY, double rMaxX, double rMaxY)
        //{
        //    fixed (double* ptr = data)
        //    {
        //        return PolygonRectIntersect(ptr, dataLength >> 1, rMinX, rMinY, rMaxX, rMaxY);
        //    }
        //}


        //internal static unsafe bool PolyLineRectIntersect(double[] data, int dataLength, double rMinX, double rMinY, double rMaxX, double rMaxY)
        //{
        //    fixed (double* ptr = data)
        //    {
        //        return PolyLineRectIntersect(ptr, dataLength >> 1, rMinX, rMinY, rMaxX, rMaxY);
        //    }
        //}



    }


    #endregion

    #region EndianUtils

    public class EndianUtils
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

        //public static byte[] GetBytesBE(short x)
        //{
        //    byte[] b = new byte[2];
        //    b[1] = (byte)(x & 0xff);
        //    b[0] = (byte)((x >> 8) & 0xff);
        //    return b;
        //}



        public static int ReadIntBE(byte[] data, int offset)
        {
            int result = data[offset];
            result = (result << 8) | data[offset + 1];
            result = (result << 8) | data[offset + 2];
            result = (result << 8) | data[offset + 3];
            return result;
        }

        public static unsafe int ReadIntLE(byte[] data, int offset)
        {
            int result;
            fixed (byte* bPtr = data)
            {
                //convert the byte array to a double Ptr and then store the dereferenced pointer in result
                result = *(int*)(bPtr + offset);
            }
            return result;
        }

        //public static unsafe double ReadDoubleBE(byte[] data, int offset)
        //{
        //    long result = data[offset];
        //    result= (result<<8)|data[offset+1];
        //    result= (result<<8)|data[offset+2];
        //    result= (result<<8)|data[offset+3];
        //    result= (result<<8)|data[offset+4];
        //    result= (result<<8)|data[offset+5];
        //    result= (result<<8)|data[offset+6];
        //    result= (result<<8)|data[offset+7];

        //    //convert the address of result to a long ptr and return the de-referenced pointer
        //    return *(double*)(&result);
        //}

        public static unsafe double ReadDoubleLE(byte[] data, int offset)
        {
            double result;
            fixed (byte* bPtr = data)
            {
                //convert the byte array to a double Ptr and then store the dereferenced pointer in result
                result = *(double*)(bPtr + offset);
            }
            return result;
        }

        public static unsafe float ReadFloatLE(byte[] data, int offset)
        {
            float result;
            fixed (byte* bPtr = data)
            {
                //convert the byte array to a float Ptr and then store the dereferenced pointer in result
                result = *(float*)(bPtr + offset);
            }
            return result;
        }

        //public static unsafe void WriteFloatLE(float f, byte[] data, int offset)
        //{		
        //    int n = *(int*)(&f);
        //    data[offset+1] = (byte)((n&0xff00)>>8);
        //    data[offset+2] = (byte)((n&0xff0000)>>16);
        //    data[offset+3] = (byte)((n&0xff000000)>>24);			
        //}

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

    #endregion

}
