using System;
using System.Collections.Generic;
using System.Text;

namespace UnE.Geomini
{
    public class Vertex2D
    {
        public double x = 0;
        public double y = 0;

        /*public static bool operator ==(Vertex2D obj1, Vertex2D obj2)
        {
            bool isNull1 = NullChecker.IsNull(obj1);
            bool isNull2 = NullChecker.IsNull(obj2);

            if (isNull1 && isNull2)
                return true;
            else if (isNull1 || isNull2)
                return false;

            return obj1.GetDistance(obj2) <= Math.HALF_TOLERANCE() ? true : false;
        }

        public static bool operator !=(Vertex2D obj1, Vertex2D obj2)
        {
            return !(obj1 == obj2);
        }*/

        public static Vertex2D operator +(Vertex2D obj1, Vertex2D obj2)
        {
            return new Vertex2D(obj1.x + obj2.x, obj1.y + obj2.y);
        }

        public static Vertex2D operator -(Vertex2D obj1, Vertex2D obj2)
        {
            return new Vertex2D(obj1.x - obj2.x, obj1.y - obj2.y);
        }

        public static Vertex2D operator *(Vertex2D obj, double data)
        {
            return new Vertex2D(obj.x * data, obj.y * data);
        }

        public static Vertex2D operator /(Vertex2D obj, double data)
        {
            if (data <= Math.COORD_TOLERANCE())
                throw new System.DivideByZeroException();

            return new Vertex2D(obj.x / data, obj.y / data);
        }

        public Vertex2D()
        {
        }

        public Vertex2D(Vertex2D vertex)
        {
            this.x = vertex.x;
            this.y = vertex.y;
        }

        public Vertex2D(double x, double y)
        {
            SetVertex(x, y);
        }

        public void SetVertex(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public void CopyFrom(Vertex2D vertex)
        {
            this.x = vertex.x;
            this.y = vertex.y;
        }

        public double GetDistance(Vertex2D vertex)
        {
            double width = this.x - vertex.x;
            double height = this.y - vertex.y;
            return System.Math.Sqrt(width * width + height * height);
        }

        // v1과 v2를 지나는 직선을 기준으로 현재의 버텍스와 대칭되는 객체를 만들어 리턴한다.
        // v1과 v2가 동일한 좌표일 경우 false를 리턴한다.
        public bool Mirror(Vertex2D v1, Vertex2D v2, out Vertex2D result)
        {
            result = null;

            if (v1 == v2)
                return false;

            double dLen = this.GetDistance(v1);
            double dAngle = Math.GetAngle(this, v1, v2);
            double dH = dLen * System.Math.Cos(dAngle);

            Vertex2D vCenter = Math.GetLinearVertex(v1, v2, dH);
            result = vCenter * 2 - this;
            return true;
        }
    }

    public class Vertex3D
    {
        public double x = 0;
        public double y = 0;
        public double z = 0;

        /*public static bool operator ==(Vertex3D obj1, Vertex3D obj2)
        {
            bool isNull1 = NullChecker.IsNull(obj1);
            bool isNull2 = NullChecker.IsNull(obj2);

            if (isNull1 && isNull2)
                return true;
            else if (isNull1 || isNull2)
                return false;

            return obj1.GetDistance(obj2) <= Math.HALF_TOLERANCE() ? true : false;
        }

        public static bool operator !=(Vertex3D obj1, Vertex3D obj2)
        {
            return !(obj1 == obj2);
        }*/

        public static Vertex3D operator +(Vertex3D obj1, Vertex3D obj2)
        {
            return new Vertex3D(obj1.x + obj2.x, obj1.y + obj2.y, obj1.z + obj2.z);
        }

        public static Vertex3D operator -(Vertex3D obj1, Vertex3D obj2)
        {
            return new Vertex3D(obj1.x - obj2.x, obj1.y - obj2.y, obj1.z - obj2.z);
        }

        public static Vertex3D operator *(Vertex3D obj, double data)
        {
            return new Vertex3D(obj.x * data, obj.y * data, obj.z * data);
        }

        public static Vertex3D operator /(Vertex3D obj, double data)
        {
            if (data <= Math.COORD_TOLERANCE())
                throw new System.DivideByZeroException();

            return new Vertex3D(obj.x / data, obj.y / data, obj.z / data);
        }

        public Vertex3D()
        {
        }

        public Vertex3D(Vertex3D vertex)
        {
            this.x = vertex.x;
            this.y = vertex.y;
            this.z = vertex.z;
        }

        public Vertex3D(double x, double y, double z)
        {
            SetVertex(x, y, z);
        }

        public void SetVertex(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public void CopyFrom(Vertex3D vertex)
        {
            this.x = vertex.x;
            this.y = vertex.y;
            this.z = vertex.z;
        }

        public double GetDistance(Vertex3D vertex)
        {
            double _x = this.x - vertex.x;
            double _y = this.y - vertex.y;
            double _z = this.z - vertex.z;
            return System.Math.Sqrt(_x * _x + _y * _y + _z * _z);
        }

        // v1과 v2를 지나는 직선을 기준으로 현재의 버텍스와 대칭되는 객체를 만들어 리턴한다.
        // v1과 v2가 동일한 좌표일 경우 false를 리턴한다.
        public bool Mirror(Vertex3D v1, Vertex3D v2, out Vertex3D result)
        {
            result = null;

            if (v1 == v2)
                return false;

            double dLen = this.GetDistance(v1);
            double dAngle = Math.GetAngle(this, v1, v2);
            double dH = dLen * System.Math.Cos(dAngle);

            Vertex3D vCenter = Math.GetLinearVertex(v1, v2, dH);
            result = vCenter * 2 - this;
            return true;
        }

        // v1, v2, v3를 지나는 평면을 기준으로 현재의 버텍스와 대칭되는 객체를 만들어 리턴한다.
        // v1, v2, v3가 평면을 구성하지 못할 경우 false를 리턴한다.
        public bool Mirror(Vertex3D v1, Vertex3D v2, Vertex3D v3, out Vertex3D result)
        {
            result = null;

            double a, b, c, d;  // ax + by + cz + d = 0
            if (!Math.MakePlane(v1, v2, v3, out a, out b, out c, out d))
                return false;

            Vertex3D vTarget = Math.GetNearestVertex(this, a, b, c, d);
            result = vTarget * 2 - this;
            return true;
        }
    }
}
