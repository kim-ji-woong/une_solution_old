#pragma once

#include "GeometryAPI.h"
#include "GVertex.h"

#ifndef DOTNET
#include <vector>
#endif

namespace UnE
{
	namespace Geometry
	{
		_DECLARE_CLASS(Line2D)
		_DECLARE_CLASS(Line2F)

		template <class Vertex, class Real>
		//GEOMETRY_DECLARE_EXPORT_CLASS(EArc) ABSTRACT
		TEMPLATE_DECLARE_CLASS(EArc) ABSTRACT
		{
		public:
			ENUM_CLASS EArcType {EARC = 0, ARC};

		public:
			EArc();
			
		public:
			// Radian
			double GetAngle() CONST {return m_dAngle;}
			double GetBeginAngle() CONST {return m_dBeginAngle;}
			double GetEndAngle() CONST;
			
			// EArc의 시작점에서 끝점까지의 방향이 시계방향인가?
			bool IsClockWise() CONST {return m_isClockWise;}
			// 완전한 타원인가?
			bool IsClosed() CONST {return m_isClosed;}

			void SetClosed(bool isClosed) {m_isClosed = isClosed;}

			REF_CONST(Vertex) GetTL() CONST {return m_vTL;}
			REF_CONST(Vertex) GetBL() CONST {return m_vBL;}
			REF_CONST(Vertex) GetBR() CONST {return m_vBR;}
			REF_CONST(Vertex) GetCenter() CONST {return m_vCenter;}

			Real GetA() CONST {return m_dA;}
			Real GetB() CONST {return m_dB;}

			bool CheckValidAngle(double dAngle) CONST;

			static double ValidAngle(double dAngle);

		public:
			// dAngle : Radian
			// dAngle이 범위를 벗어나면 false를 리턴한다.
			virtual bool GetVertex(double dAngle, OUT CBR(INSTANCE(Vertex)) rVertex) CONST = 0;
			virtual EArcType GetType() CONST = 0;
			virtual INSTANCE(Vertex) GetBeginVertex() CONST = 0;
			virtual INSTANCE(Vertex) GetEndVertex() CONST = 0;

		protected:
			// Return값 : radian
			double GetVertexAngle(REF_CONST(Vertex) rVertex) CONST;
			// rEArc를 타원의 바깥으로 dLen 만큼 확대(outside가 false이면 축소)시킨다.
			void _Offset(REF(EArc) rEArc, bool outside, Real dLen) CONST;
			
			static double GetRealAngle(double dBeginAngle, double dEndAngle, bool isClockwise);

		protected:
			// Radian
			double m_dBeginAngle;
			double m_dAngle;	// 전체 각도
			// EArc의 시작점에서 끝점까지의 방향이 시계방향인가?
			bool m_isClockWise;
			// 완전한 타원인가?
			bool m_isClosed;

			// EArc를 둘러싼 직사각형 영역
			INSTANCE(Vertex) m_vTL;
			INSTANCE(Vertex) m_vBL;
			INSTANCE(Vertex) m_vBR;
			INSTANCE(Vertex) m_vCenter;

			// x²/ a²+ y²/ b²= 1의 a
			// 직사각형 너비의 절반
			Real m_dA;
			// x²/ a²+ y²/ b²= 1의 b
			// 직사각형 높이의 절반
			Real m_dB;
		};

		GEOMETRY_EXPORT_CLASS(EArc2D) : PUBLIC EArc<Vertex2D, double>
		{
		public:
			EArc2D(void);
			virtual ~EArc2D(void);
			// vTL, vBL, vBR : 타원이 존재하는 직사각형 영역
			// vBR과 vTR의 중점이 0도이며, 각도의 진행은 반시계 방향이다.
			// dBeginAngle, dEArcAngle : Radian
			// dEArcAngle이 2PI 이상이면 완전한 타원이 된다.
			EArc2D(REF_CONST(Vertex2D) vTL, REF_CONST(Vertex2D) vBL, REF_CONST(Vertex2D) vBR, double dBeginAngle, double dEArcAngle, bool isClockWise);

/*#ifdef DOTNET
			static bool operator== (EArc2D^ op1, EArc2D^ op2);
			static bool operator!= (EArc2D^ op1, EArc2D^ op2);
#else
			bool operator== (const EArc2D& rhs) const;
			bool operator!= (const EArc2D& rhs) const;
#endif*/

		public:
			// vTL, vBL, vBR : 타원이 존재하는 직사각형 영역
			// vBR과 vTR의 중점이 0도이며, 각도의 진행은 반시계 방향이다.
			// dBeginAngle, dEndAngle : Radian
			// dEArcAngle이 2PI 이상이면 완전한 타원이 된다.
			bool SetEArc(REF_CONST(Vertex2D) vTL, REF_CONST(Vertex2D) vBL, REF_CONST(Vertex2D) vBR, double dBeginAngle, double dEArcAngle, bool isClockWise);
			// rEArc를 타원의 바깥으로 dLen 만큼 확대(outside가 false이면 축소)시킨다.
			INSTANCE(EArc2D) Offset(bool outside, double dLen) CONST;
			// v1과 v2를 지나는 직선을 기준으로 현재의 EArc 객체와 대칭되는 객체를 만들어 리턴한다.
			// v1과 v2가 동일한 좌표일 경우 false를 리턴한다.
			bool Mirror(REF_CONST(Vertex2D) v1, REF_CONST(Vertex2D) v2, OUT CBR(INSTANCE(EArc2D)) rResult);
		
		public:
			// dAngle : Radian
			// dAngle이 범위를 벗어나면 false를 리턴한다.
			virtual bool GetVertex(double dAngle, OUT CBR(INSTANCE(Vertex2D)) rVertex) CONST override;
			virtual EArcType GetType() CONST override {return ENUM_OF(EArcType, EARC);}
			virtual INSTANCE(Vertex2D) GetBeginVertex() CONST override;
			virtual INSTANCE(Vertex2D) GetEndVertex() CONST override;

			// rLine과 만나지 않으면 0을 리턴한다.
			// 교차점이 하나만 존재할 경우 rVertex1에만 값이 담겨지며 1이 리턴된다.
			// 교차점이 두 개 존재할 경우 rVertex1과 rVertex2에 각각 값이 담겨지며, 2가 리턴된다.
			virtual int IntersectLine(REF_CONST(Line2D) rLine, OUT CBR(INSTANCE(Vertex2D)) rVertex1, OUT CBR(INSTANCE(Vertex2D)) rVertex2) CONST;
			// rEArc와 만나지 않으면 0을 리턴한다.
			// Return 값 : 두 EArc가 만나서 생기는 (Vertex의 개수) + (EArc 개수 * 100)
			//             만일, 두 EArc가 만나서 하나의 Vertex와 하나의 EArc가 생성된다면 101이 리턴된다.
#ifdef DOTNET
			virtual int IntersectEArc(EArc2D^ rEArc, OUT System::Collections::ArrayList^% rArrVertex, OUT System::Collections::ArrayList^% rArrEArc);
#else
			virtual int IntersectEArc(const EArc2D& rEArc, std::vector<Vertex2D>& rArrVertex, std::vector<EArc2D*>& rArrEArc) const;
#endif
			// EArc위의 한점 vertex로부터 EArc의 진행방향으로 len만큼 떨어진 곳의 좌표를 구한다.
			// vertex가 EArc의 각도 범위에 포함되지 않아도 상관없다.
			virtual bool GetLinearVertex(REF_CONST(Vertex2D) vertex, double len, OUT CBR(INSTANCE(Vertex2D)) rResult);
			// dAngle : Radian
			// EArc의 dAngle 위치에 있는 좌표로부터 EArc의 진행방향으로 len만큼 떨어진 곳의 좌표를 구한다.
			// dAngle이 EArc의 각도 범위에 포함되지 않아도 상관없다.
			virtual bool GetLinearVertex(double dAngle, double len, OUT CBR(INSTANCE(Vertex2D)) rResult);

		protected:
			bool _Mirror(REF_CONST(Vertex2D) v1, REF_CONST(Vertex2D) v2, REF(EArc2D) rResult);
			// rVertex와 타원이 이루는 각도만 검사한다.
			//bool IsInclude(REF_CONST(Vertex2D) rVertex) CONST;
			// dAngle : Radian
			bool IsInclude(double dAngle) CONST;
			//int InsertsectLineResult(double x1, double y1, double x2, double y2, CBR(INSTANCE(Vertex2D)) rVertex1, CBR(INSTANCE(Vertex2D)) rVertex2, REF_CONST(Line2D) rLine) CONST;

			// Return 값 : 타원의 회전각(Radian)
			double CoordTranslate(REF_CONST(Line2D) rLine, OUT CBR(INSTANCE(Line2D)) result) CONST;
			INSTANCE(Vertex2D)  CoordTranslate(REF_CONST(Vertex2D) rVertex, double theta) CONST;
			int _IntersectLine(REF_CONST(Line2D) rLine, CBR(INSTANCE(Vertex2D)) rVertex1, CBR(INSTANCE(Vertex2D)) rVertex2) CONST;
			// y = ax + b 인 직선과 타원의 교점
			int _IntersectLine(REF_CONST(Line2D) rLine, CBR(INSTANCE(Vertex2D)) rVertex1, CBR(INSTANCE(Vertex2D)) rVertex2, double a, double b) CONST;
			// x = c 인 직선과 타원의 교점
			int _IntersectLine(REF_CONST(Line2D) rLine, CBR(INSTANCE(Vertex2D)) rVertex1, CBR(INSTANCE(Vertex2D)) rVertex2, double c) CONST;

			// EArc와의 교차점 검사이후 사용
			void SetIntersectResult(REF_CONST(EArc2D) earc1, REF_CONST(EArc2D) earc2);

		protected:
			virtual void NewObject(CBR(POINTER(EArc2D)) pEArc) CONST;
			// vBegin에서 vEnd방향으로(반시계 방향) 향하는 rEArc를 만든다.
			virtual void MakeSub(POINTER(EArc2D) pEArc, REF_CONST(Vertex2D) vBegin, REF_CONST(Vertex2D) vEnd) CONST;
			virtual void SetIntersectResult(double dBeginAngle, double dEArcAngle, bool clockwise);
		};

		GEOMETRY_EXPORT_CLASS(EArc2F) : PUBLIC EArc<Vertex2F, float>
		{
		public:
			EArc2F(void);
			virtual ~EArc2F(void);
			// vTL, vBL, vBR : 타원이 존재하는 직사각형 영역
			// vBR과 vTR의 중점이 0도이며, 각도의 진행은 반시계 방향이다.
			// dBeginAngle, dEArcAngle : Radian
			// dEArcAngle이 2PI 이상이면 완전한 타원이 된다.
			EArc2F(REF_CONST(Vertex2F) vTL, REF_CONST(Vertex2F) vBL, REF_CONST(Vertex2F) vBR, double dBeginAngle, double dEArcAngle, bool isClockWise);

/*#ifdef DOTNET
			static bool operator== (EArc2F^ op1, EArc2F^ op2);
			static bool operator!= (EArc2F^ op1, EArc2F^ op2);
#else
			bool operator== (const EArc2F& rhs) const;
			bool operator!= (const EArc2F& rhs) const;
#endif*/

		public:
			// vTL, vBL, vBR : 타원이 존재하는 직사각형 영역
			// vBR과 vTR의 중점이 0도이며, 각도의 진행은 반시계 방향이다.
			// dBeginAngle, dEndAngle : Radian
			// dEArcAngle이 2PI 이상이면 완전한 타원이 된다.
			bool SetEArc(REF_CONST(Vertex2F) vTL, REF_CONST(Vertex2F) vBL, REF_CONST(Vertex2F) vBR, double dBeginAngle, double dEArcAngle, bool isClockWise);
			// rEArc를 타원의 바깥으로 dLen 만큼 확대(outside가 false이면 축소)시킨다.
			INSTANCE(EArc2F) Offset(bool outside, float dLen) CONST;
			// v1과 v2를 지나는 직선을 기준으로 현재의 EArc 객체와 대칭되는 객체를 만들어 리턴한다.
			// v1과 v2가 동일한 좌표일 경우 false를 리턴한다.
			bool Mirror(REF_CONST(Vertex2F) v1, REF_CONST(Vertex2F) v2, OUT CBR(INSTANCE(EArc2F)) rResult);

		public:
			// dAngle : Radian
			// dAngle이 범위를 벗어나면 false를 리턴한다.
			virtual bool GetVertex(double dAngle, OUT CBR(INSTANCE(Vertex2F)) rVertex) CONST override;
			virtual EArcType GetType() CONST override{ return ENUM_OF(EArcType, EARC); }
			virtual INSTANCE(Vertex2F) GetBeginVertex() CONST override;
			virtual INSTANCE(Vertex2F) GetEndVertex() CONST override;

			// rLine과 만나지 않으면 0을 리턴한다.
			// 교차점이 하나만 존재할 경우 rVertex1에만 값이 담겨지며 1이 리턴된다.
			// 교차점이 두 개 존재할 경우 rVertex1과 rVertex2에 각각 값이 담겨지며, 2가 리턴된다.
			virtual int IntersectLine(REF_CONST(Line2F) rLine, OUT CBR(INSTANCE(Vertex2F)) rVertex1, OUT CBR(INSTANCE(Vertex2F)) rVertex2) CONST;
			// rEArc와 만나지 않으면 0을 리턴한다.
			// Return 값 : 두 EArc가 만나서 생기는 (Vertex의 개수) + (EArc 개수 * 100)
			//             만일, 두 EArc가 만나서 하나의 Vertex와 하나의 EArc가 생성된다면 101이 리턴된다.
#ifdef DOTNET
			virtual int IntersectEArc(EArc2F^ rEArc, OUT System::Collections::ArrayList^% rArrVertex, OUT System::Collections::ArrayList^% rArrEArc);
#else
			virtual int IntersectEArc(const EArc2F& rEArc, std::vector<Vertex2F>& rArrVertex, std::vector<EArc2F*>& rArrEArc) const;
#endif
			// EArc위의 한점 vertex로부터 EArc의 진행방향으로 len만큼 떨어진 곳의 좌표를 구한다.
			// vertex가 EArc의 각도 범위에 포함되지 않아도 상관없다.
			virtual bool GetLinearVertex(REF_CONST(Vertex2F) vertex, float len, OUT CBR(INSTANCE(Vertex2F)) rResult);
			// dAngle : Radian
			// EArc의 dAngle 위치에 있는 좌표로부터 EArc의 진행방향으로 len만큼 떨어진 곳의 좌표를 구한다.
			// dAngle이 EArc의 각도 범위에 포함되지 않아도 상관없다.
			virtual bool GetLinearVertex(double dAngle, float len, OUT CBR(INSTANCE(Vertex2F)) rResult);

		protected:
			bool _Mirror(REF_CONST(Vertex2F) v1, REF_CONST(Vertex2F) v2, REF(EArc2F) rResult);
			// rVertex와 타원이 이루는 각도만 검사한다.
			//bool IsInclude(REF_CONST(Vertex2D) rVertex) CONST;
			// dAngle : Radian
			bool IsInclude(double dAngle) CONST;
			//int InsertsectLineResult(double x1, double y1, double x2, double y2, CBR(INSTANCE(Vertex2D)) rVertex1, CBR(INSTANCE(Vertex2D)) rVertex2, REF_CONST(Line2D) rLine) CONST;

			// Return 값 : 타원의 회전각(Radian)
			double CoordTranslate(REF_CONST(Line2F) rLine, OUT CBR(INSTANCE(Line2F)) result) CONST;
			INSTANCE(Vertex2F)  CoordTranslate(REF_CONST(Vertex2F) rVertex, double theta) CONST;
			int _IntersectLine(REF_CONST(Line2F) rLine, CBR(INSTANCE(Vertex2F)) rVertex1, CBR(INSTANCE(Vertex2F)) rVertex2) CONST;
			// y = ax + b 인 직선과 타원의 교점
			int _IntersectLine(REF_CONST(Line2F) rLine, CBR(INSTANCE(Vertex2F)) rVertex1, CBR(INSTANCE(Vertex2F)) rVertex2, float a, float b) CONST;
			// x = c 인 직선과 타원의 교점
			int _IntersectLine(REF_CONST(Line2F) rLine, CBR(INSTANCE(Vertex2F)) rVertex1, CBR(INSTANCE(Vertex2F)) rVertex2, float c) CONST;

			// EArc와의 교차점 검사이후 사용
			void SetIntersectResult(REF_CONST(EArc2F) earc1, REF_CONST(EArc2F) earc2);

		protected:
			virtual void NewObject(CBR(POINTER(EArc2F)) pEArc) CONST;
			// vBegin에서 vEnd방향으로(반시계 방향) 향하는 rEArc를 만든다.
			virtual void MakeSub(POINTER(EArc2F) pEArc, REF_CONST(Vertex2F) vBegin, REF_CONST(Vertex2F) vEnd) CONST;
			virtual void SetIntersectResult(double dBeginAngle, double dEArcAngle, bool clockwise);
		};

		GEOMETRY_EXPORT_CLASS(EArc3D) : PUBLIC EArc<Vertex3D, double>
		{
		public:
			EArc3D(void);
			virtual ~EArc3D(void);
			// vTL, vBL, vBR : 타원이 존재하는 직사각형 영역
			// vBR과 vTR의 중점이 0도이며, 각도의 진행은 반시계 방향이다.
			// dBeginAngle, dEArcAngle : Radian
			// dEArcAngle이 2PI 이상이면 완전한 타원이 된다.
			EArc3D(REF_CONST(Vertex3D) vTL, REF_CONST(Vertex3D) vBL, REF_CONST(Vertex3D) vBR, double dBeginAngle, double dEArcAngle, bool isClockWise);

/*#ifdef DOTNET
			static bool operator== (EArc3D^ op1, EArc3D^ op2);
			static bool operator!= (EArc3D^ op1, EArc3D^ op2);
#else
			bool operator== (const EArc3D& rhs) const;
			bool operator!= (const EArc3D& rhs) const;
#endif*/

		public:
			// vTL, vBL, vBR : 타원이 존재하는 직사각형 영역
			// vBR과 vTR의 중점이 0도이며, 각도의 진행은 반시계 방향이다.
			// dBeginAngle, dEndAngle : Radian
			// dEArcAngle이 2PI 이상이면 완전한 타원이 된다.
			bool SetEArc(REF_CONST(Vertex3D) vTL, REF_CONST(Vertex3D) vBL, REF_CONST(Vertex3D) vBR, double dBeginAngle, double dEArcAngle, bool isClockWise);
			// rEArc를 타원의 바깥으로 dLen 만큼 확대(outside가 false이면 축소)시킨다.
			INSTANCE(EArc3D) Offset(bool outside, double dLen) CONST;
			// v1, v2, v3를 지나는 평면을 기준으로 현재의 EArc와 대칭되는 객체를 만들어 리턴한다.
			// v1, v2, v3가 평면을 구성하지 못할 경우 false를 리턴한다.
			bool Mirror(REF_CONST(Vertex3D) v1, REF_CONST(Vertex3D) v2, REF_CONST(Vertex3D) v3, OUT CBR(INSTANCE(EArc3D)) rResult);
		
		public:
			// dAngle : Radian
			// dAngle이 범위를 벗어나면 false를 리턴한다.
			virtual bool GetVertex(double dAngle, OUT CBR(INSTANCE(Vertex3D)) rVertex) CONST override;
			virtual EArcType GetType() CONST override {return ENUM_OF(EArcType, EARC);}
			virtual INSTANCE(Vertex3D) GetBeginVertex() CONST override;
			virtual INSTANCE(Vertex3D) GetEndVertex() CONST override;

		protected:
			bool _Mirror(double a, double b, double c, double d, REF(EArc3D) rResult);

		protected:
			virtual void NewObject(CBR(POINTER(EArc3D)) pEArc) CONST;
		};

#include "GEArc.inl"

	}
}
