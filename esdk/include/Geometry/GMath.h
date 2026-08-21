#pragma once

#include "GeometryAPI.h"
#include "GVertex.h"

namespace UnE
{
	namespace Geometry
	{
		GEOMETRY_DECLARE_EXPORT_CLASS(Math)
		{
			// 상수 정의
		public:
			static double COORD_TOLERANCE();
			static float  HALF_TOLERANCE();

			static double HALF_PI();
			static double PI();
			static double _3HALF_PI();
			static double _2PI();

		private:
#ifdef DOTNET
			static double _COORD_TOLERANCE = 0.0000001;
			static float  _HALF_TOLERANCE  = 0.0001f;

			static double _HALF_PI	= 1.57079632679489661923;
			static double _PI		= 3.14159265358979323846;
			static double __3HALF_PI = 4.71238898038468985769;
			static double __2PI		= 6.28318530717958647692;
#else
			static double _COORD_TOLERANCE;
			static float  _HALF_TOLERANCE;

			static double _HALF_PI;
			static double _PI;
			static double __3HALF_PI;
			static double __2PI;
#endif
			// Geometry Method
		public:
			/*static double GetDistance(REF_CONST(Vertex3D) v1, REF_CONST(Vertex3D) v2);
			static float  GetDistance(REF_CONST(Vertex3F) v1, REF_CONST(Vertex3F) v2);
			static double GetDistance(REF_CONST(Vertex2D) v1, REF_CONST(Vertex2D) v2);
			static float  GetDistance(REF_CONST(Vertex2F) v1, REF_CONST(Vertex2F) v2);*/

			// 소수점 몇자리까지 허용할 것인가를 판단한 다음
			// 값을 넘겨준다.
			static double GetTolerance(double data);
			static void SetHalfTolerance(float fTolerance);
			static void SetCoordTolerance(double dTolerance);

			static double RadToDeg(double dRadian);
			static double DegToRad(double dDegree);

			// v1과 vCenter가 이루는 직선과 vCenter와 v2가 이루는
			// 직선이 서로 만나 이루는 각을 리턴한다.
			// Return 값 : Radian
			static double GetAngle(REF_CONST(Vertex3D) v1, REF_CONST(Vertex3D) vCenter, REF_CONST(Vertex3D) v2);
			static double GetAngle(REF_CONST(Vertex3F) v1, REF_CONST(Vertex3F) vCenter, REF_CONST(Vertex3F) v2);
			static double GetAngle(REF_CONST(Vertex2D) v1, REF_CONST(Vertex2D) vCenter, REF_CONST(Vertex2D) v2);
			static double GetAngle(REF_CONST(Vertex2F) v1, REF_CONST(Vertex2F) vCenter, REF_CONST(Vertex2F) v2);

			// v1과 v2를 잇는 직선상에서 v1으로부터 v2 방향으로 dLength 만큼
			// 떨어진 거리의 점을 구한다.
			static INSTANCE(Vertex3D) GetLinearVertex(REF_CONST(Vertex3D) v1, REF_CONST(Vertex3D) v2, double dLength);
			static Vertex3FInstance GetLinearVertex(REF_CONST(Vertex3F) v1, REF_CONST(Vertex3F) v2, float fLength);
			static INSTANCE(Vertex2D) GetLinearVertex(REF_CONST(Vertex2D) v1, REF_CONST(Vertex2D) v2, double dLength);
			static Vertex2FInstance GetLinearVertex(REF_CONST(Vertex2F) v1, REF_CONST(Vertex2F) v2, float fLength);

			// v1과 v2를 지나는 직선과 수직이며 v1을 지나는 직선이 있다.
			// 이 직선상에 존재하며 v1으로부터 거리 dDistance 만큼 오른쪽(XY 좌표계에서 v2를 원점,
			// v1을 양의 Y축에 놓았을 경우)으로 떨어진 거리의 점을 구한다.
			static INSTANCE(Vertex2D) GetRightVertex(REF_CONST(Vertex2D) v1, REF_CONST(Vertex2D) v2, double dDistance);
			static Vertex2FInstance GetRightVertex(REF_CONST(Vertex2F) v1, REF_CONST(Vertex2F) v2, float fDistance);

			// vBegin과 vEnd를 잇는 직선이 있다.
			// 가상 좌표계에서 vEnd를 원점, vBegin을 양의 Y축에 있다고 가정하였을 때,
			// rVertex가 양의 X축에 있는지 여부를 알려준다.
			// Return 값 : 1 (직선의 오른쪽에 있다. => 양의 X축)
			//             0 (직선의 왼쪽에 있다. => 음의 X축)
			//            -1 (직선위에 존재한다.)
			static int IsRightSideFromLine(REF_CONST(Vertex2D) rVertex, REF_CONST(Vertex2D) vBegin, REF_CONST(Vertex2D) vEnd);
			static int IsRightSideFromLine(REF_CONST(Vertex2F) rVertex, REF_CONST(Vertex2F) vBegin, REF_CONST(Vertex2F) vEnd);

			// vLineBegin과 vLineEnd를 잇는 직선위에서 rVertex와 가장 가까운 점을 알려준다.
			// noLimit : true이면 직선은 무한한 길이를 갖고 있으며, false이면 직선은 vLineBegin과 vLineEnd 사이의 제한된 길이를 가진다.
			static INSTANCE(Vertex3D) GetNearestVertex(REF_CONST(Vertex3D) rVertex, REF_CONST(Vertex3D) vLineBegin, REF_CONST(Vertex3D) vLineEnd, bool noLimit);
			static Vertex3FInstance GetNearestVertex(REF_CONST(Vertex3F) rVertex, REF_CONST(Vertex3F) vLineBegin, REF_CONST(Vertex3F) vLineEnd, bool noLimit);
			static INSTANCE(Vertex2D) GetNearestVertex(REF_CONST(Vertex2D) rVertex, REF_CONST(Vertex2D) vLineBegin, REF_CONST(Vertex2D) vLineEnd, bool noLimit);
			static Vertex2FInstance GetNearestVertex(REF_CONST(Vertex2F) rVertex, REF_CONST(Vertex2F) vLineBegin, REF_CONST(Vertex2F) vLineEnd, bool noLimit);
			// 평면(ax + by + cz + d = 0) 위에서 rVertex와 가장 가까운 점을 알려준다.
			static INSTANCE(Vertex3D) GetNearestVertex(REF_CONST(Vertex3D) rVertex, double a, double b, double c, double d);

			// v1, v2, v3를 지나는 평면의 방정식을 구한다.(ax + by + cz + d = 0)
			// v1, v2, v3가 평면을 구성하지 못할 경우 false를 리턴한다.
			static bool MakePlane(REF_CONST(Vertex3D) v1, REF_CONST(Vertex3D) v2, REF_CONST(Vertex3D) v3, OUT CBR(double) a, OUT CBR(double) b, OUT CBR(double) c, OUT CBR(double) d);

			// rVertex가 vBegin과 vEnd 사이에 위치하는지 검사한다.
			/*static bool IsIncludeInLine(REF_CONST(Vertex3D) rVertex, REF_CONST(Vertex3D) vBegin, REF_CONST(Vertex3D) vEnd);
			static bool IsIncludeInLine(REF_CONST(Vertex3F) rVertex, REF_CONST(Vertex3F) vBegin, REF_CONST(Vertex3F) vEnd);
			static bool IsIncludeInLine(REF_CONST(Vertex2D) rVertex, REF_CONST(Vertex2D) vBegin, REF_CONST(Vertex2D) vEnd);
			static bool IsIncludeInLine(REF_CONST(Vertex2F) rVertex, REF_CONST(Vertex2F) vBegin, REF_CONST(Vertex2F) vEnd);*/
		};
	}
}
