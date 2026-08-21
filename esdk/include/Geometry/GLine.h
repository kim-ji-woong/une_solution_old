#pragma once
#include "GeometryAPI.h"
#include "GVertex.h"

#ifndef GCE2D_MAKELINE
#define GCE2D_MAKELINE(Line2DObj) GCE2d_MakeLine(GP_POINT2D(OF(Line2DObj, GetVertex(true))), GP_POINT2D(OF(Line2DObj, GetVertex(false))))
#endif

// 직선과 반직선, 선분을 표현한다.
// 모든 Line class의 Default 속성은 선분이다.
// [2012/07/27] 김지웅

namespace UnE
{
	namespace Geometry
	{
		_DECLARE_CLASS(EArc2D)
		_DECLARE_CLASS(EArc2F)

		template <class Vertex, class Real>
		//GEOMETRY_DECLARE_EXPORT_CLASS(Line) ABSTRACT
		TEMPLATE_DECLARE_CLASS(Line) ABSTRACT
		{
		public:
			// HALF_LINE_BEGIN_2_END : 시작점에서 끝점 방향으로 끝없이 이어진 반직선
			// HALF_LINE_END_2_BEGIN : 끝점에서 시작점 방향으로 끝없이 이어진 반직선
			ENUM_CLASS LineType {LINE = 0, HALF_LINE_BEGIN_2_END, HALF_LINE_END_2_BEGIN, SEGMENT, NO_LINE};

		public:
			Line()
			{
				m_lineType = ENUM_OF(LineType, SEGMENT);
				m_vBegin = dnonlynew Vertex();
				m_vEnd	 = dnonlynew Vertex();
			}
			
		public:
			LineType GetLineType() CONST {return m_lineType;}
			void SetLineType(LineType type)	{m_lineType = type;}

			void SetVertex(REF_CONST(Vertex) rVertex, bool isBegin)
			{
				if (isBegin) OF(m_vBegin, CopyFrom(rVertex));
				else OF(m_vEnd, CopyFrom(rVertex));
			}

			// rVertex가 Line내에 포함되어 있는지 알려준다.
			virtual bool IsInclude(REF_CONST(Vertex) rVertex) CONST = 0;
			// noLimit이 true이면 실제 LineType에 상관없이 무한히 뻗은 직선이라 가정하고 계산한다.
			// noLimit이 false이면 실제 LineType을 고려하여 가장 가까운 거리를 구한다.
			virtual Real GetDistance(REF_CONST(Vertex) rVertex, bool noLimit) CONST = 0;
			
			REF_CONST(Vertex) GetVertex(bool isBegin) CONST	{return isBegin ? m_vBegin : m_vEnd;}

		protected:
			LineType m_lineType;
			INSTANCE(Vertex) m_vBegin;
			INSTANCE(Vertex) m_vEnd;
		};

		GEOMETRY_EXPORT_CLASS(Line3D) : PUBLIC Line<Vertex3D, double>
		{
		public:
			Line3D();
			Line3D(LineType type);
			Line3D(Line3DRefConst rhs);
			Line3D(Vertex3DRefConst vBegin, Vertex3DRefConst vEnd);
			Line3D(Vertex3DRefConst vBegin, Vertex3DRefConst vEnd, LineType type);
			virtual ~Line3D(void);

/*#ifdef DOTNET
			static bool operator== (Line3D^ op1, Line3D^ op2);
			static bool operator!= (Line3D^ op1, Line3D^ op2);
#else
			bool operator== (const Line3D& rhs) const;
			bool operator!= (const Line3D& rhs) const;
#endif*/

		public:
			// rVertex가 Line내에 포함되어 있는지 알려준다.
			virtual bool IsInclude(REF_CONST(Vertex3D) rVertex) CONST override;
			// noLimit이 true이면 실제 LineType에 상관없이 무한히 뻗은 직선이라 가정하고 계산한다.
			// noLimit이 false이면 실제 LineType을 고려하여 가장 가까운 거리를 구한다.
			virtual double GetDistance(REF_CONST(Vertex3D) rVertex, bool noLimit) CONST override;
			
		public:
			// 현재 직선에서 rVertex 방향으로 dLen 만큼 떨어진 객체를 만들어 리턴한다.
			// LineType은 현재 직선과 동일하다.
			INSTANCE(Line3D) Offset(REF_CONST(Vertex3D) rVertex, double dLen) CONST;
			// v1, v2, v3를 지나는 평면을 기준으로 현재의 직선과 대칭되는 객체를 만들어 리턴한다.
			// LineType은 현재 직선과 동일하다.
			// v1, v2, v3가 평면을 구성하지 못할 경우 false를 리턴한다.
			bool Mirror(REF_CONST(Vertex3D) v1, REF_CONST(Vertex3D) v2, REF_CONST(Vertex3D) v3, OUT CBR(INSTANCE(Line3D)) rResult);
		};

		GEOMETRY_EXPORT_CLASS(Line2D) : PUBLIC Line<Vertex2D, double>
		{
		public:
			Line2D();
			Line2D(LineType type);
			Line2D(Line2DRefConst rhs);
			Line2D(Vertex2DRefConst vBegin, Vertex2DRefConst vEnd);
			Line2D(Vertex2DRefConst vBegin, Vertex2DRefConst vEnd, LineType type);
			virtual ~Line2D(void);

/*#ifdef DOTNET
			static bool operator== (Line2D^ op1, Line2D^ op2);
			static bool operator!= (Line2D^ op1, Line2D^ op2);
#else
			bool operator== (const Line2D& rhs) const;
			bool operator!= (const Line2D& rhs) const;
#endif*/

		public:
			// rLine과 만나지 않으면 0을 리턴한다.
			// 교차점이 하나만 존재할 경우 rVertex1에만 값이 담겨지며 1이 리턴된다.
			// 교차점이 두 개 존재할 경우 rVertex1과 rVertex2에 각각 값이 담겨진다.
			// 교차점이 두 개인 경우는 직선에 해당하기 때문에 rResultType을 읽어 어떠한 형태의 직선인지 알아낼 수 있다.
			int IntersectLine(REF_CONST(Line2D) rLine, OUT CBR(INSTANCE(Vertex2D)) rVertex1, OUT CBR(INSTANCE(Vertex2D)) rVertex2, OUT CBR(LineType) rResultType);
			// rEArc와 만나지 않으면 0을 리턴한다.
			// 교차점이 하나만 존재할 경우 rVertex1에만 값이 담겨지며 1이 리턴된다.
			// 교차점이 두 개 존재할 경우 rVertex1과 rVertex2에 각각 값이 담겨지며, 2가 리턴된다.
			int IntersectEArc(REF_CONST(EArc2D) rEArc, OUT CBR(INSTANCE(Vertex2D)) rVertex1, OUT CBR(INSTANCE(Vertex2D)) rVertex2) CONST;

		public:
			// rVertex가 Line내에 포함되어 있는지 알려준다.
			virtual bool IsInclude(REF_CONST(Vertex2D) rVertex) CONST override;
			// noLimit이 true이면 실제 LineType에 상관없이 무한히 뻗은 직선이라 가정하고 계산한다.
			// noLimit이 false이면 실제 LineType을 고려하여 가장 가까운 거리를 구한다.
			virtual double GetDistance(REF_CONST(Vertex2D) rVertex, bool noLimit) CONST override;

		public:
			// 현재 직선에서 rVertex 방향으로 dLen 만큼 떨어진 객체를 만들어 리턴한다.
			// LineType은 현재 직선과 동일하다.
			INSTANCE(Line2D) Offset(REF_CONST(Vertex2D) rVertex, double dLen) CONST;
			// 현재 직선에서 오른쪽 방향으로(rightSide가 false이면 왼쪽 방향) dLen 만큼 떨어진 객체를 만들어 리턴한다.
			// LineType은 현재 직선과 동일하다.
			// 방향은 직선의 시작점과 끝점을 기준으로 판단한다.
			INSTANCE(Line2D) Offset(bool rightSide, double dLen) CONST;
			// v1과 v2를 지나는 직선을 기준으로 현재의 직선과 대칭되는 객체를 만들어 리턴한다.
			// LineType은 현재 직선과 동일하다.
			// v1과 v2가 동일한 좌표일 경우 false를 리턴한다.
			bool Mirror(REF_CONST(Vertex2D) v1, REF_CONST(Vertex2D) v2, OUT CBR(INSTANCE(Line2D)) rResult);
			
		protected:
			// 두 Line이 겹치는 경우에 대한 상태 분석
			static int HalfLineToHalfLine(REF_CONST(Line2D) rLine1, REF_CONST(Line2D) rLine2, REF(Vertex2D) rVertex1, REF(Vertex2D) rVertex2, CBR(LineType) rResultType);
			static int HalfLineToSegment(REF_CONST(Line2D) rHalfLine, REF_CONST(Line2D) rSegment, REF(Vertex2D) rVertex1, REF(Vertex2D) rVertex2, CBR(LineType) rResultType);
			static int SegmentToSegment(REF_CONST(Line2D) rLine1, REF_CONST(Line2D) rLine2, REF(Vertex2D) rVertex1, REF(Vertex2D) rVertex2, CBR(LineType) rResultType);
		};

		GEOMETRY_EXPORT_CLASS(Line2F) : PUBLIC Line<Vertex2F, float>
		{
		public:
			Line2F();
			Line2F(LineType type);
			Line2F(Line2FRefConst rhs);
			Line2F(Vertex2FRefConst vBegin, Vertex2FRefConst vEnd);
			Line2F(Vertex2FRefConst vBegin, Vertex2FRefConst vEnd, LineType type);
			virtual ~Line2F(void);

/*#ifdef DOTNET
			static bool operator== (Line2F^ op1, Line2F^ op2);
			static bool operator!= (Line2F^ op1, Line2F^ op2);
#else
			bool operator== (const Line2F& rhs) const;
			bool operator!= (const Line2F& rhs) const;
#endif*/

		public:
			// rLine과 만나지 않으면 0을 리턴한다.
			// 교차점이 하나만 존재할 경우 rVertex1에만 값이 담겨지며 1이 리턴된다.
			// 교차점이 두 개 존재할 경우 rVertex1과 rVertex2에 각각 값이 담겨진다.
			// 교차점이 두 개인 경우는 직선에 해당하기 때문에 rResultType을 읽어 어떠한 형태의 직선인지 알아낼 수 있다.
			int IntersectLine(REF_CONST(Line2F) rLine, OUT CBR(INSTANCE(Vertex2F)) rVertex1, OUT CBR(INSTANCE(Vertex2F)) rVertex2, OUT CBR(LineType) rResultType);
			// rEArc와 만나지 않으면 0을 리턴한다.
			// 교차점이 하나만 존재할 경우 rVertex1에만 값이 담겨지며 1이 리턴된다.
			// 교차점이 두 개 존재할 경우 rVertex1과 rVertex2에 각각 값이 담겨지며, 2가 리턴된다.
			int IntersectEArc(REF_CONST(EArc2F) rEArc, OUT CBR(INSTANCE(Vertex2F)) rVertex1, OUT CBR(INSTANCE(Vertex2F)) rVertex2) CONST;

		public:
			// rVertex가 Line내에 포함되어 있는지 알려준다.
			virtual bool IsInclude(REF_CONST(Vertex2F) rVertex) CONST override;
			// noLimit이 true이면 실제 LineType에 상관없이 무한히 뻗은 직선이라 가정하고 계산한다.
			// noLimit이 false이면 실제 LineType을 고려하여 가장 가까운 거리를 구한다.
			virtual float GetDistance(REF_CONST(Vertex2F) rVertex, bool noLimit) CONST override;

		public:
			// 현재 직선에서 rVertex 방향으로 dLen 만큼 떨어진 객체를 만들어 리턴한다.
			// LineType은 현재 직선과 동일하다.
			INSTANCE(Line2F) Offset(REF_CONST(Vertex2F) rVertex, float dLen) CONST;
			// 현재 직선에서 오른쪽 방향으로(rightSide가 false이면 왼쪽 방향) dLen 만큼 떨어진 객체를 만들어 리턴한다.
			// LineType은 현재 직선과 동일하다.
			// 방향은 직선의 시작점과 끝점을 기준으로 판단한다.
			INSTANCE(Line2F) Offset(bool rightSide, float dLen) CONST;
			// v1과 v2를 지나는 직선을 기준으로 현재의 직선과 대칭되는 객체를 만들어 리턴한다.
			// LineType은 현재 직선과 동일하다.
			// v1과 v2가 동일한 좌표일 경우 false를 리턴한다.
			bool Mirror(REF_CONST(Vertex2F) v1, REF_CONST(Vertex2F) v2, OUT CBR(INSTANCE(Line2F)) rResult);

		protected:
			// 두 Line이 겹치는 경우에 대한 상태 분석
			static int HalfLineToHalfLine(REF_CONST(Line2F) rLine1, REF_CONST(Line2F) rLine2, REF(Vertex2F) rVertex1, REF(Vertex2F) rVertex2, CBR(LineType) rResultType);
			static int HalfLineToSegment(REF_CONST(Line2F) rHalfLine, REF_CONST(Line2F) rSegment, REF(Vertex2F) rVertex1, REF(Vertex2F) rVertex2, CBR(LineType) rResultType);
			static int SegmentToSegment(REF_CONST(Line2F) rLine1, REF_CONST(Line2F) rLine2, REF(Vertex2F) rVertex1, REF(Vertex2F) rVertex2, CBR(LineType) rResultType);
		};
	}
}
