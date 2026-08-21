using System;
using System.Collections.Generic;
using System.Text;

namespace UnE.Geomini
{
    public class Math
    {
        public static double _2PI()
        {
            return 6.28318530717958647692;
        }

        public static double _3HALF_PI()
        {
            return 4.71238898038468985769;
        }

        public static double PI()
        {
            return 3.14159265358979323846;
        }

        public static double HALF_PI()
        {
            return 1.57079632679489661923;
        }

        public static double COORD_TOLERANCE()
        {
            return 0.0000001;
        }

        public static float HALF_TOLERANCE()
        {
            return 0.0001f;
        }

        public static double GetAngle(Vertex2D v1, Vertex2D vCenter, Vertex2D v2)
        {
            // 코사인 제2법칙
            // C²= A²+ B²- 2ABcosΘ
            double a = v1.GetDistance(vCenter);
            double b = v2.GetDistance(vCenter);
            double c = v1.GetDistance(v2);

            double cosData = (a * a + b * b - c * c) / 2 / a / b;
            if (cosData < -1.0) cosData = -1.0;
            else if (cosData > 1.0) cosData = 1.0;

            return System.Math.Acos(cosData);
        }

        public static double GetAngle(Vertex3D v1, Vertex3D vCenter, Vertex3D v2)
        {
            // 코사인 제2법칙
            // C²= A²+ B²- 2ABcosΘ
            double a = v1.GetDistance(vCenter);
            double b = v2.GetDistance(vCenter);
            double c = v1.GetDistance(v2);

            double cosData = (a * a + b * b - c * c) / 2 / a / b;
            if (cosData < -1.0) cosData = -1.0;
            else if (cosData > 1.0) cosData = 1.0;

            return System.Math.Acos(cosData);
        }

        public static Vertex2D GetLinearVertex(Vertex2D v1, Vertex2D v2, double dLength)
        {
            double dL = v1.GetDistance(v2);

            if (dL <= Math.COORD_TOLERANCE())
                return new Vertex2D(v1);

            Vertex2D v3 = v1 + (v2 - v1) * dLength / dL;
            return v3;
        }

        public static Vertex3D GetLinearVertex(Vertex3D v1, Vertex3D v2, double dLength)
        {
            double dL = v1.GetDistance(v2);

            if (dL <= Math.COORD_TOLERANCE())
                return new Vertex3D(v1);

            Vertex3D v3 = v1 + (v2 - v1) * dLength / dL;
            return v3;
        }

        // vLineBegin과 vLineEnd를 잇는 직선위에서 vertex 가장 가까운 점을 알려준다.
        // noLimit : true이면 직선은 무한한 길이를 갖고 있으며, false이면 직선은 vLineBegin과 vLineEnd 사이의 제한된 길이를 가진다.
        public static Vertex2D GetNearestVertex(Vertex2D vertex, Vertex2D vLineBegin, Vertex2D vLineEnd, bool noLimit)
        {
            double dLen = vertex.GetDistance(vLineBegin);
            double dLen2 = vertex.GetDistance(vLineEnd);

            if (dLen <= Math.HALF_TOLERANCE() || dLen2 <= Math.HALF_TOLERANCE())
                return vertex;

            double dAngle = Math.GetAngle(vertex, vLineBegin, vLineEnd);
            double dH = dLen * System.Math.Cos(dAngle);

            Vertex2D vertex2 = Math.GetLinearVertex(vLineBegin, vLineEnd, dH);
            Line2D line = new Line2D(vLineBegin, vLineEnd);

            if (noLimit || line.IsInclude(vertex2))
            {
                return vertex2;
            }

            return dLen < dLen2 ? new Vertex2D(vLineBegin) : new Vertex2D(vLineEnd);
        }

        // vLineBegin과 vLineEnd를 잇는 직선위에서 vertex 가장 가까운 점을 알려준다.
        // noLimit : true이면 직선은 무한한 길이를 갖고 있으며, false이면 직선은 vLineBegin과 vLineEnd 사이의 제한된 길이를 가진다.
        public static Vertex3D GetNearestVertex(Vertex3D vertex, Vertex3D vLineBegin, Vertex3D vLineEnd, bool noLimit)
        {
            double dLen = vertex.GetDistance(vLineBegin);
            double dLen2 = vertex.GetDistance(vLineEnd);

            if (dLen <= Math.HALF_TOLERANCE() || dLen2 <= Math.HALF_TOLERANCE())
                return vertex;

            double dAngle = Math.GetAngle(vertex, vLineBegin, vLineEnd);
            double dH = dLen * System.Math.Cos(dAngle);

            Vertex3D vertex2 = Math.GetLinearVertex(vLineBegin, vLineEnd, dH);
            Line3D line = new Line3D(vLineBegin, vLineEnd);

            if (noLimit || line.IsInclude(vertex2))
            {
                return vertex2;
            }

            return dLen < dLen2 ? new Vertex3D(vLineBegin) : new Vertex3D(vLineEnd);
        }

        public static Vertex2D GetRightVertex(Vertex2D v1, Vertex2D v2, double dDistance)
        {
            double dLen = v1.GetDistance(v2);
            if (dLen == 0.0)
                return new Vertex2D(v1);

            Vertex2D vResult = new Vertex2D();
            vResult.x = dDistance / dLen * (v1.y - v2.y) + v1.x;
            vResult.y = dDistance / dLen * (v2.x - v1.x) + v1.y;
            return vResult;
        }

        // v1, v2, v3를 지나는 평면의 방정식을 구한다.(ax + by + cz + d = 0)
        // v1, v2, v3가 평면을 구성하지 못할 경우 false를 리턴한다.
        public static bool MakePlane(Vertex3D v1, Vertex3D v2, Vertex3D v3, out double a, out double b, out double c, out double d)
        {
            a = b = c = d = 0.0;

            if (v1.GetDistance(v2) <= Math.HALF_TOLERANCE() || v2.GetDistance(v3) <= Math.HALF_TOLERANCE() || v3.GetDistance(v1) <= Math.HALF_TOLERANCE())
                return false;

            Line3D line = new Line3D(v1, v2, Line3D.LineType.LINE);
            
            if (line.IsInclude(v3))
                return false;

            a = v1.y * (v2.z - v3.z) + v2.y * (v3.z - v1.z) + v3.y * (v1.z - v2.z);
            b = v1.z * (v2.x - v3.x) + v2.z * (v3.x - v1.x) + v3.z * (v1.x - v2.x);
            c = v1.x * (v2.y - v3.y) + v2.x * (v3.y - v1.y) + v3.x * (v1.y - v2.y);
            d = -(v1.x * (v2.y * v3.z - v3.y * v2.z) + v2.x * (v3.y * v1.z - v1.y * v3.z) + v3.x * (v1.y * v2.z - v2.y * v1.z));
            return true;
        }

        // 평면(ax + by + cz + d = 0) 위에서 rVertex와 가장 가까운 점을 알려준다.
        public static Vertex3D GetNearestVertex(Vertex3D vertex, double a, double b, double c, double d)
        {
            double k = -(a * vertex.x + b * vertex.y + c * vertex.z + d) / (a * a + b * b + c * c);
	        return new Vertex3D(a * k + vertex.x, b * k + vertex.y, c * k + vertex.z);
        }
    }
}
