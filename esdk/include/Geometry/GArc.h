#pragma once

#include "GeometryAPI.h"
#include "GEArc.h"

namespace UnE
{
	namespace Geometry
	{
		GEOMETRY_EXPORT_CLASS(Arc2D) : PUBLIC EArc2D
		{
		public:
			Arc2D(void);
			virtual ~Arc2D(void);
			Arc2D(REF_CONST(Vertex2D) v1, REF_CONST(Vertex2D) v2, REF_CONST(Vertex2D) v3);
			// dBeginAngle, dArcAngle : Radian
			Arc2D(REF_CONST(Vertex2D) vCenter, double dRadius, double dBeginAngle, double dArcAngle, bool isClockWise);

/*#ifdef DOTNET
			static bool operator== (Arc2D^ op1, Arc2D^ op2);
			static bool operator!= (Arc2D^ op1, Arc2D^ op2);
#else
			bool operator== (const Arc2D& rhs) const;
			bool operator!= (const Arc2D& rhs) const;
#endif*/

		public:
			bool SetArc(REF_CONST(Vertex2D) v1, REF_CONST(Vertex2D) v2, REF_CONST(Vertex2D) v3);
			// dBeginAngle, dArcAngle : Radian
			void SetArc(REF_CONST(Vertex2D) vCenter, double dRadius, double dBeginAngle, double dArcAngle, bool isClockWise);

			double GetRadius() CONST {return m_dRadius;}
			// rArc를 원의 바깥으로 dLen 만큼 확대(outside가 false이면 축소)시킨다.
			INSTANCE(Arc2D) Offset(bool outside, double dLen) CONST;
			// v1과 v2를 지나는 직선을 기준으로 현재의 Arc 객체와 대칭되는 객체를 만들어 리턴한다.
			// v1과 v2가 동일한 좌표일 경우 false를 리턴한다.
			bool Mirror(REF_CONST(Vertex2D) v1, REF_CONST(Vertex2D) v2, OUT CBR(INSTANCE(Arc2D)) rResult);

		public:
			// dAngle : Radian
			// dAngle이 범위를 벗어나면 false를 리턴한다.
			virtual bool GetVertex(double dAngle, OUT CBR(INSTANCE(Vertex2D)) rVertex) CONST override;
			virtual EArcType GetType() CONST override {return ENUM_OF(EArcType, ARC);}
			virtual INSTANCE(Vertex2D) GetBeginVertex() CONST override;
			virtual INSTANCE(Vertex2D) GetEndVertex() CONST override;

		protected:
			// 세 점을 이용하여 원의 중점 및 반지름을 구한다.
			// Return 값 : true이면 값을 구하였다.
			//             false이면 원을 구성하기에 충분치 않은 데이터이다.
			static bool GetCircleInfo(REF_CONST(Vertex2D) v1, REF_CONST(Vertex2D) v2, REF_CONST(Vertex2D) v3, CBR(INSTANCE(Vertex2D)) rCenter, CBR(double) rRadius);

			virtual void NewObject(CBR(POINTER(EArc2D)) pEArc) CONST override;
			// vBegin에서 vEnd방향으로(반시계 방향) 향하는 rEArc를 만든다.
			virtual void MakeSub(POINTER(EArc2D) pEArc, REF_CONST(Vertex2D) vBegin, REF_CONST(Vertex2D) vEnd) CONST override;
			virtual void SetIntersectResult(double dBeginAngle, double dEArcAngle, bool clockwise) override;

		protected:
			double m_dRadius;
		};

		GEOMETRY_EXPORT_CLASS(Arc3D) : PUBLIC EArc3D
		{
		public:
			Arc3D(void);
			virtual ~Arc3D(void);
			Arc3D(REF_CONST(Vertex3D) v1, REF_CONST(Vertex3D) v2, REF_CONST(Vertex3D) v3);
			Arc3D(REF_CONST(Vertex3D) vTL, REF_CONST(Vertex3D) vBL, REF_CONST(Vertex3D) vBR, double dRadius, double dBeginAngle, double dArcAngle, bool isClockWise);

/*#ifdef DOTNET
			static bool operator== (Arc3D^ op1, Arc3D^ op2);
			static bool operator!= (Arc3D^ op1, Arc3D^ op2);
#else
			bool operator== (const Arc3D& rhs) const;
			bool operator!= (const Arc3D& rhs) const;
#endif*/

		public:
			bool SetArc(REF_CONST(Vertex3D) v1, REF_CONST(Vertex3D) v2, REF_CONST(Vertex3D) v3);
			bool SetArc(REF_CONST(Vertex3D) vTL, REF_CONST(Vertex3D) vBL, REF_CONST(Vertex3D) vBR, double dRadius, double dBeginAngle, double dArcAngle, bool isClockWise);

			double GetRadius() CONST {return m_dRadius;}
			// rArc를 원의 바깥으로 dLen 만큼 확대(outside가 false이면 축소)시킨다.
			INSTANCE(Arc3D) Offset(bool outside, double dLen) CONST;
			// v1, v2, v3를 지나는 평면을 기준으로 현재의 Arc와 대칭되는 객체를 만들어 리턴한다.
			// v1, v2, v3가 평면을 구성하지 못할 경우 false를 리턴한다.
			bool Mirror(REF_CONST(Vertex3D) v1, REF_CONST(Vertex3D) v2, REF_CONST(Vertex3D) v3, OUT CBR(INSTANCE(Arc3D)) rResult);

		public:
			// dAngle : Radian
			// dAngle이 범위를 벗어나면 false를 리턴한다.
			virtual bool GetVertex(double dAngle, OUT CBR(INSTANCE(Vertex3D)) rVertex) CONST override;
			virtual EArcType GetType() CONST override {return ENUM_OF(EArcType, ARC);}
			virtual INSTANCE(Vertex3D) GetBeginVertex() CONST override;
			virtual INSTANCE(Vertex3D) GetEndVertex() CONST override;

		protected:
			virtual void NewObject(CBR(POINTER(EArc3D)) pEArc) CONST override;
			
		protected:
			// 세 점을 이용하여 원의 중점 및 반지름을 구한다.
			// Return 값 : true이면 값을 구하였다.
			//             false이면 원을 구성하기에 충분치 않은 데이터이다.
			static bool GetCircleInfo(REF_CONST(Vertex3D) v1, REF_CONST(Vertex3D) v2, REF_CONST(Vertex3D) v3, CBR(INSTANCE(Vertex3D)) rCenter, CBR(double) rRadius);
			// 원 위의 세 점 v1, v2, v3가 있고 원의 중점 vCenter가 있다.
			// v1에서 v2와 v3를 차례대로 지나가는 방향으로 90도 회전한 곳의 좌표를 구한다.
			static INSTANCE(Vertex3D) GetRightAngleVertex(REF_CONST(Vertex3D) v1, REF_CONST(Vertex3D) v2, REF_CONST(Vertex3D) v3, REF_CONST(Vertex3D) vCenter, double dRadius);

		protected:
			double m_dRadius;
		};
	}
}
