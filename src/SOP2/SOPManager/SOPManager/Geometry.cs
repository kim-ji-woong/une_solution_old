using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SOPManager
{
    class Geometry
    {
        const double COORD_TOLERANCE = 0.0000000001;
        const double HALF_TOLERANCE = 0.001;

        // 점이 폴리곤 내부에 있는지 검색
        // 폴리곤의 시작점과 끝점이 다를 경우, 시작점과 끝점이 연결된 폐곡선으로 간주한다.
        // 물론 폴리곤의 시작점과 끝점이 같아도 상관없다.
        // Return : 1이면 pt가 폴리곤의 내부에 위치한다.
        //          0이면 pt가 폴리곤의 외부에 위치한다.
        //         -1이면 pt가 폴리곤의 경계에 위치한다.
        public static int PolygonHitTest(PointF[] arrBoundary, PointF pt)
        {
            if (arrBoundary == null)
                return 0;

            int nPointNumber = arrBoundary.Count();
	        if (nPointNumber < 3) return 0;

	        PointF ptBegin = arrBoundary[0];

	        // 시작점과 끝점이 같은지 검사한다.
	        bool isFirstLastSame = arrBoundary[nPointNumber - 1] == ptBegin;
	        if (isFirstLastSame) nPointNumber--;

	        PointF ptEnd = arrBoundary[nPointNumber-1];
	
	        float x;
	        int nCount = 0;
	        PointF ptPrev = ptEnd;

	        for (int i=0;i<nPointNumber;i++)
	        {
		        PointF point = arrBoundary[i];

		        if (IsIncludeInLine(pt, ptPrev, point)) return -1;
		        else
		        {
			        // X축과 평행한 선분은 계산하지 않는다.
			        if (System.Math.Abs(ptPrev.Y - point.Y) > 0.0)
			        {
				        if (GetXFromLine(ptPrev, point, pt.Y, out x))
				        {
					        if (System.Math.Abs(x - pt.X) == 0.0) return -1;
					        else if (x > pt.X)
					        {
						        CheckPointCount(ptPrev, point, pt.Y, ref nCount);
					        }
				        }
			        }
		        }

		        ptPrev = point;
	        }

	        if (nCount % 2 == 0) return 0;
	        return 1;
        }

        public static bool IsIncludeInLine(PointF pt, PointF ptBegin, PointF ptEnd)
        {
	        double d1 = GetDistance(pt, ptBegin);
	        double d2 = GetDistance(pt, ptEnd);

            if (d1 <= HALF_TOLERANCE || d2 <= HALF_TOLERANCE) return true;
	        if (System.Math.Abs(GetAngle(ptBegin, pt, ptEnd) - System.Math.PI) > HALF_TOLERANCE) return false;

	        return true;
        }

        public static double GetDistance(PointF pt1, PointF pt2)
        {
            return System.Math.Sqrt((pt2.X - pt1.X) * (pt2.X - pt1.X) + (pt2.Y - pt1.Y) * (pt2.Y - pt1.Y));
        }

        public static double GetAngle(PointF pt1, PointF ptCenter, PointF pt2)
        {
            // 코사인 제2법칙
            // C²= A²+ B²- 2ABcosΘ
            double a = GetDistance(pt1, ptCenter);
            double b = GetDistance(pt2, ptCenter);
            double c = GetDistance(pt1, pt2);

            double cosData = (a * a + b * b - c * c) / 2 / a / b;
            if (cosData < -1.0) cosData = -1.0;
            else if (cosData > 1.0) cosData = 1.0;

            return System.Math.Acos(cosData);
        }

        // pt1과 pt2를 잇는 직선에서 특정 좌표가 y값을 가지는 경우 x값을 알려준다.
        // y값을 가질수 없거나 해가 무수히 많은 경우 false를 리턴한다.
        // bNoLimit : true일 경우 pt1과 pt2를 지나는 직선이 무한한 길이를 가진다고 가정한다.
        //           false일 경우 직선의 범위는 pt1과 pt2를 잇는 구간으로 한정된다.
        private static bool GetXFromLine(PointF pt1, PointF pt2, double y, out float x)
        {
            x = 0;

	        if (pt1.Y == pt2.Y)
	        {
		        if (y == pt1.Y)
		        {
			        x = (pt1.X + pt2.X) / 2;
			        return true;
		        }
		        else
                    return false;
	        }

	        x = (float)((pt2.X - pt1.X) / (pt2.Y - pt1.Y) * (y - pt1.Y) + pt1.X);

	        if (pt1.X < pt2.X)
	        {
		        if (x < pt1.X - HALF_TOLERANCE || x > pt2.X + HALF_TOLERANCE) return false;
	        }
	        else
	        {
		        if (x < pt2.X - HALF_TOLERANCE || x > pt1.X + HALF_TOLERANCE) return false;
	        }

	        if (pt1.Y < pt2.Y)
	        {
		        if (y < pt1.Y - HALF_TOLERANCE || y > pt2.Y + HALF_TOLERANCE) return false;
	        }
	        else
	        {
		        if (y < pt2.Y - HALF_TOLERANCE || y > pt1.Y + HALF_TOLERANCE) return false;
	        }

	        return true;
        }

        private static void CheckPointCount(PointF ptLineBegin, PointF ptLineEnd, float y, ref int rCount)
        {
	        float fMaxY, fMinY;
	
	        if (ptLineBegin.Y < ptLineEnd.Y)
	        {
		        fMinY = ptLineBegin.Y;
		        fMaxY = ptLineEnd.Y;
	        }
	        else
	        {
		        fMinY = ptLineEnd.Y;
		        fMaxY = ptLineBegin.Y;
	        }

	        // y가 ptLineBegin과 ptLineEnd 사이에 있거나, y가 둘 중 최소점과 일치하는 경우 rCount를 증가시킨다.
	        // y가 둘 중 최대점과 일치하는 경우 rCount를 증가시키지 않는다.
	        if (y < fMaxY && y >= fMinY)
	        {
		        rCount++;
	        }
        }

        public static PointF GetLinearPoint(PointF pt1, PointF pt2, double dLength)
        {
            // pt1과 pt2 사이의 거리
	        double dL = GetDistance(pt1,pt2);

	        if (dL == 0.0) return pt1;

	        PointF pt3 = new PointF();
	        pt3.X = (float)(pt1.X + dLength * (pt2.X - pt1.X) / dL);
            pt3.Y = (float)(pt1.Y + dLength * (pt2.Y - pt1.Y) / dL);

	        return pt3;
        }

        // 직선(ptLineBegin ~ ptLineEnd) 위의 점 중에서 점 pt와 가장 가까운 점을 리턴한다.
        // noLimit이 true이면 직선은 무한히 긴 것으로 간주하며, false이면 [ptLine ~ ptLineEnd] 사이의 제한된 길이만 가진다.
        public static PointF GetNearestPoint(PointF pt, PointF ptLineBegin, PointF ptLineEnd, bool noLimit = false)
        {
            double dLen = GetDistance(pt, ptLineBegin);
            double dAngle = GetAngle(pt, ptLineBegin, ptLineEnd);
            double dH = dLen * System.Math.Cos(dAngle);

            PointF ptResult = GetLinearPoint(ptLineBegin, ptLineEnd, dH);

            if (noLimit || IsIncludeInLine(ptResult, ptLineBegin, ptLineEnd))
            {
                return ptResult;
            }

            double d1 = GetDistance(pt, ptLineBegin);
            double d2 = GetDistance(pt, ptLineEnd);
            return d1 < d2 ? ptLineBegin : ptLineEnd;
        }
    }
}
