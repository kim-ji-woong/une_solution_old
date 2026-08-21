#include "StdAfx.h"
#include "GLine.h"
#include "GMath.h"
#include "GVector.h"
#include "GEArc.h"
#include <GCE2d_MakeLine.hxx>
#include <Geom2d_Line.hxx>
#include <Geom2dAPI_InterCurveCurve.hxx>
#include <Geom_Line.hxx>
#include <GeomAPI_ExtremaCurveCurve.hxx>

#ifdef DOTNET
using namespace System;
#endif

BEGIN_NS(UnE)
BEGIN_NS(Geometry)

template <class Vertex, class Line, class Real>
static bool _IsInclude(REF_CONST(Vertex) rVertex, REF_CONST(Line) rLine)
{
	Real dLen = OF(rLine, GetDistance(rVertex, false));
	if (dLen <= Geometry::Math::HALF_TOLERANCE())
		return true;

	return false;
}

template<class Vertex, class Line, class Real>
Real _GetDistance(REF_CONST(Vertex) rVertex, REF_CONST(Line) rLine, bool bNoLimit)
{
	REF_CONST(Vertex) vBegin = OF(rLine, GetVertex(true));
	REF_CONST(Vertex) vEnd = OF(rLine, GetVertex(false));

	Real a = OF(rVertex, GetDistance(vBegin));
	Real b = OF(vBegin, GetDistance(vEnd));
	Real c = OF(rVertex, GetDistance(vEnd));

	if (a <= Geometry::Math::COORD_TOLERANCE() || c <= Geometry::Math::COORD_TOLERANCE())
		return 0.0;
	if (b <= Geometry::Math::COORD_TOLERANCE())
		return a;

	Real dCos = (a * a + b * b - c * c) / 2 / a / b;
	INSTANCE(Vertex) vertex = Geometry::Math::GetLinearVertex(vBegin, vEnd, dCos * a);
	Real dLen = OF(vertex, GetDistance(rVertex));

	Line::LineType type = OF(rLine, GetLineType());

#ifdef DOTNET
	if (bNoLimit || type == Line::LineType::LINE) return dLen;
#else
	if (bNoLimit || type == Line::LINE) return dLen;
#endif

	double dAngle1 = Geometry::Math::GetAngle(rVertex, vBegin, vEnd);
	double dAngle2 = Geometry::Math::GetAngle(rVertex, vEnd, vBegin);

	if (dAngle1 <= Geometry::Math::HALF_PI() && dAngle2 <= Geometry::Math::HALF_PI())
		return dLen;

#ifdef DOTNET
	if (type == Line::LineType::HALF_LINE_BEGIN_2_END)
#else
	if (type == Line::HALF_LINE_BEGIN_2_END)
#endif
	{
		if (dAngle1 < Geometry::Math::HALF_PI())
			return dLen;
	}
#ifdef DOTNET
	else if (type == Line::LineType::HALF_LINE_END_2_BEGIN)
#else
	else if (type == Line::HALF_LINE_END_2_BEGIN)
#endif
	{
		if (dAngle2 < Geometry::Math::HALF_PI())
			return dLen;
	}

	return a > c ? c : a;
}

static Handle(Geom_Line) GCE3D_MAKELINE(REF_CONST(Line3D) rLine)
{
	INSTANCE(Vertex3D) vDir = OF(rLine, GetVertex(false)) - OF(rLine, GetVertex(true));
	if (!Vector::SetUnitVector(vDir))
		return 0;

	Geom_Line* pGeomLine = new Geom_Line(GP_POINT3D(OF(rLine, GetVertex(true))), gp_Dir(OF(vDir, x), OF(vDir, y), OF(vDir, z)));
	return pGeomLine;
}

template <class Line, class Vertex, class LineType>
static int _HalfLineToHalfLine(REF_CONST(Line) rLine1, REF_CONST(Line) rLine2, REF(Vertex) rVertex1, REF(Vertex) rVertex2, CBR(LineType) rResultType)
{
	LineType type1 = OF(rLine1, GetLineType());
	LineType type2 = OF(rLine2, GetLineType());

#ifdef DOTNET
	REF_CONST(Vertex) vLine1Fixed  = type1 == LineType::HALF_LINE_BEGIN_2_END ? OF(rLine1, GetVertex(true)) : OF(rLine1, GetVertex(false));
	REF_CONST(Vertex) vLine1Opened = type1 == LineType::HALF_LINE_BEGIN_2_END ? OF(rLine1, GetVertex(false)) : OF(rLine1, GetVertex(true));
	REF_CONST(Vertex) vLine2Fixed  = type2 == LineType::HALF_LINE_BEGIN_2_END ? OF(rLine2, GetVertex(true)) : OF(rLine2, GetVertex(false));
	REF_CONST(Vertex) vLine2Opened = type2 == LineType::HALF_LINE_BEGIN_2_END ? OF(rLine2, GetVertex(false)) : OF(rLine2, GetVertex(true));
#else
	REF_CONST(Vertex) vLine1Fixed  = type1 == Line::HALF_LINE_BEGIN_2_END ? OF(rLine1, GetVertex(true)) : OF(rLine1, GetVertex(false));
	REF_CONST(Vertex) vLine1Opened = type1 == Line::HALF_LINE_BEGIN_2_END ? OF(rLine1, GetVertex(false)) : OF(rLine1, GetVertex(true));
	REF_CONST(Vertex) vLine2Fixed  = type2 == Line::HALF_LINE_BEGIN_2_END ? OF(rLine2, GetVertex(true)) : OF(rLine2, GetVertex(false));
	REF_CONST(Vertex) vLine2Opened = type2 == Line::HALF_LINE_BEGIN_2_END ? OF(rLine2, GetVertex(false)) : OF(rLine2, GetVertex(true));
#endif

	bool include1 = OF(rLine2, IsInclude(vLine1Fixed));
	bool include2 = OF(rLine1, IsInclude(vLine2Fixed));

	if (include1 && include2)
	{
		INSTANCE(Vertex) vertex = Math::GetLinearVertex(vLine1Fixed, vLine1Opened, -100.0);

		if (OF(rLine2, IsInclude(vertex)))
		{
			// 두 Line이 동일한 경우
			OF(rVertex1, CopyFrom(vLine1Fixed));
			OF(rVertex2, CopyFrom(vLine1Opened));
			rResultType = type1;
		}
		else
		{
			// 두 Line이 반대 방향이며, 겹치는 부분이 하나의 선분을 이루는 경우
			OF(rVertex1, CopyFrom(vLine1Fixed));
			OF(rVertex2, CopyFrom(vLine2Fixed));

#ifdef DOTNET
			rResultType = LineType::SEGMENT;
#else
			rResultType = Line::SEGMENT;
#endif
		}
	}
	else if (include1)
	{
		// rLine1이 rLine2에 포함되는 경우
		OF(rVertex1, CopyFrom(vLine1Fixed));
		OF(rVertex2, CopyFrom(vLine1Opened));
		rResultType = type1;
	}
	else if (include2)
	{
		// rLine2가 rLine1에 포함되는 경우
		OF(rVertex1, CopyFrom(vLine2Fixed));
		OF(rVertex2, CopyFrom(vLine2Opened));
		rResultType = type2;
	}
	else
		return 0;

	return 2;
}

template <class Line, class Vertex, class LineType>
static int _HalfLineToSegment(REF_CONST(Line) rHalfLine, REF_CONST(Line) rSegment, REF(Vertex) rVertex1, REF(Vertex) rVertex2, CBR(LineType) rResultType)
{
	LineType type1 = OF(rHalfLine, GetLineType());
	
#ifdef DOTNET
	REF_CONST(Vertex) vLine1Fixed  = type1 == LineType::HALF_LINE_BEGIN_2_END ? OF(rHalfLine, GetVertex(true)) : OF(rHalfLine, GetVertex(false));
	REF_CONST(Vertex) vLine1Opened = type1 == LineType::HALF_LINE_BEGIN_2_END ? OF(rHalfLine, GetVertex(false)) : OF(rHalfLine, GetVertex(true));
#else
	REF_CONST(Vertex) vLine1Fixed  = type1 == Line::HALF_LINE_BEGIN_2_END ? OF(rHalfLine, GetVertex(true)) : OF(rHalfLine, GetVertex(false));
	REF_CONST(Vertex) vLine1Opened = type1 == Line::HALF_LINE_BEGIN_2_END ? OF(rHalfLine, GetVertex(false)) : OF(rHalfLine, GetVertex(true));
#endif
	
	bool include1 = OF(rHalfLine, IsInclude(OF(rSegment, GetVertex(true))));
	bool include2 = OF(rHalfLine, IsInclude(OF(rSegment, GetVertex(false))));

	if (include1 && include2)
	{
		// rSegment가 rHalfLine에 완전히 포함되는 경우
		OF(rVertex1, CopyFrom(OF(rSegment, GetVertex(true))));
		OF(rVertex2, CopyFrom(OF(rSegment, GetVertex(false))));
#ifdef DOTNET
		rResultType = LineType::SEGMENT;
#else
		rResultType = Line::SEGMENT;
#endif
	}
	else if (include1)
	{
		if (OF(vLine1Fixed, GetDistance(OF(rSegment, GetVertex(true)))) <= Math::HALF_TOLERANCE())
		{
			// 두 직선이 한 점에서 만나는 경우
			OF(rVertex1, CopyFrom(vLine1Fixed));
#ifdef DOTNET
		rResultType = LineType::NO_LINE;
#else
		rResultType = Line::NO_LINE;
#endif
			return 1;
		}
		else
		{
			// 두 직선의 겹침 구간이 하나의 선분을 이루는 경우
			OF(rVertex1, CopyFrom(vLine1Fixed));
			OF(rVertex2, CopyFrom(OF(rSegment, GetVertex(true))));
#ifdef DOTNET
		rResultType = LineType::SEGMENT;
#else
		rResultType = Line::SEGMENT;
#endif
		}
	}
	else if (include2)
	{
		if (OF(vLine1Fixed, GetDistance(OF(rSegment, GetVertex(false)))) <= Math::HALF_TOLERANCE())
		{
			// 두 직선이 한 점에서 만나는 경우
			OF(rVertex1, CopyFrom(vLine1Fixed));
#ifdef DOTNET
		rResultType = LineType::NO_LINE;
#else
		rResultType = Line::NO_LINE;
#endif
			return 1;
		}
		else
		{
			// 두 직선의 겹침 구간이 하나의 선분을 이루는 경우
			OF(rVertex1, CopyFrom(vLine1Fixed));
			OF(rVertex2, CopyFrom(OF(rSegment, GetVertex(false))));
#ifdef DOTNET
		rResultType = LineType::SEGMENT;
#else
		rResultType = Line::SEGMENT;
#endif
		}
	}
	else
		return 0;

	return 2;
}

template <class Line, class Vertex, class LineType>
static int _SegmentToSegment(REF_CONST(Line) rLine1, REF_CONST(Line) rLine2, REF(Vertex) rVertex1, REF(Vertex) rVertex2, CBR(LineType) rResultType)
{
	REF_CONST(Vertex) vLine1Begin = OF(rLine1, GetVertex(true));
	REF_CONST(Vertex) vLine1End = OF(rLine1, GetVertex(false));
	REF_CONST(Vertex) vLine2Begin = OF(rLine2, GetVertex(true));
	REF_CONST(Vertex) vLine2End = OF(rLine2, GetVertex(false));
	
	bool include1 = OF(rLine2, IsInclude(vLine1Begin));
	bool include2 = OF(rLine2, IsInclude(vLine1End));
	bool include3 = OF(rLine1, IsInclude(vLine2Begin));
	bool include4 = OF(rLine1, IsInclude(vLine2End));

	if (include1 && include2)
	{
		// rLine1이 rLine2에 완전히 포함되는 경우
		OF(rVertex1, CopyFrom(vLine1Begin));
		OF(rVertex2, CopyFrom(vLine1End));
#ifdef DOTNET
		rResultType = LineType::SEGMENT;
#else
		rResultType = Line::SEGMENT;
#endif
	}
	else if (include1)
	{
		if (OF(vLine1Begin, GetDistance(vLine2Begin)) <= Math::HALF_TOLERANCE() || OF(vLine1Begin, GetDistance(vLine2End)) <= Math::HALF_TOLERANCE())
		{
			// 두 직선이 한 점에서 만나는 경우
			OF(rVertex1, CopyFrom(vLine1Begin));
#ifdef DOTNET
		rResultType = LineType::NO_LINE;
#else
		rResultType = Line::NO_LINE;
#endif
			return 1;
		}
		else
		{
			// 두 직선의 겹침 구간이 하나의 선분을 이루는 경우
			REF_CONST(Vertex) rVertex = OF(rLine1, IsInclude(vLine2Begin)) ? vLine2Begin : vLine2End;
			
			OF(rVertex1, CopyFrom(rVertex));
			OF(rVertex2, CopyFrom(vLine1Begin));
#ifdef DOTNET
		rResultType = LineType::SEGMENT;
#else
		rResultType = Line::SEGMENT;
#endif
		}
	}
	else if (include2)
	{
		if (OF(vLine1End, GetDistance(vLine2Begin)) <= Math::HALF_TOLERANCE() || OF(vLine1End, GetDistance(vLine2End)) <= Math::HALF_TOLERANCE())
		{
			// 두 직선이 한 점에서 만나는 경우
			OF(rVertex1, CopyFrom(vLine1End));
#ifdef DOTNET
		rResultType = LineType::NO_LINE;
#else
		rResultType = Line::NO_LINE;
#endif
			return 1;
		}
		else
		{
			// 두 직선의 겹침 구간이 하나의 선분을 이루는 경우
			REF_CONST(Vertex) rVertex = OF(rLine1, IsInclude(vLine2Begin)) ? vLine2Begin : vLine2End;
			
			OF(rVertex1, CopyFrom(rVertex));
			OF(rVertex2, CopyFrom(vLine1End));
#ifdef DOTNET
		rResultType = LineType::SEGMENT;
#else
		rResultType = Line::SEGMENT;
#endif
		}
	}
	else
	{
		if (include3 && include4)
		{
			// rLine2가 rLine1에 완전히 포함되는 경우
			OF(rVertex1, CopyFrom(vLine2Begin));
			OF(rVertex2, CopyFrom(vLine2End));
#ifdef DOTNET
		rResultType = LineType::SEGMENT;
#else
		rResultType = Line::SEGMENT;
#endif
		}
		else
			return 0;
	}

	return 2;
}

template <class Line, class Vertex, class Real>
INSTANCE(Line) _Offset(REF_CONST(Line) rLine, REF_CONST(Vertex) rVertex, Real dLen)
{
	REF_CONST(Vertex) rBegin = OF(rLine, GetVertex(true));
	REF_CONST(Vertex) rEnd	 = OF(rLine, GetVertex(false));

	INSTANCE(Vertex) vTarget = Math::GetNearestVertex(rVertex, rBegin, rEnd, true);
	Real distance = OF(vTarget, GetDistance(rVertex));

	if (distance <= Math::HALF_TOLERANCE())
		return dnonlynew Line(rBegin, rEnd, OF(rLine, GetLineType()));

	INSTANCE(Vertex) vBegin = rBegin + (rVertex - vTarget) * dLen / distance;
	INSTANCE(Vertex) vEnd = rEnd + (rVertex -  vTarget) * dLen / distance;

	return dnonlynew Line(vBegin, vEnd, OF(rLine, GetLineType()));
}

template <class Line>
static bool Equal(REF_CONST(Line) rLine1, REF_CONST(Line) rLine2)
{
	if (OF(rLine1, GetLineType()) != OF(rLine2, GetLineType())) return false;
	if (OF(rLine1, GetVertex(true)) != OF(rLine2, GetVertex(true))) return false;
	if (OF(rLine1, GetVertex(false)) != OF(rLine2, GetVertex(false))) return false;

	return true;
}

Line3D::Line3D()
{
}

Line3D::Line3D(LineType type)
{
	SetLineType(type);
}

Line3D::Line3D(Line3DRefConst rhs)
{
	SetLineType(OF(rhs, m_lineType));
	SetVertex(OF(rhs, m_vBegin), true);
	SetVertex(OF(rhs, m_vEnd), false);
}

Line3D::Line3D(Vertex3DRefConst vBegin, Vertex3DRefConst vEnd)
{
	SetVertex(vBegin, true);
	SetVertex(vEnd, false);
}

Line3D::Line3D(Vertex3DRefConst vBegin, Vertex3DRefConst vEnd, LineType type)
{
	SetLineType(type);
	SetVertex(vBegin, true);
	SetVertex(vEnd, false);
}

Line3D::~Line3D(void)
{
}

/*#ifdef DOTNET
bool Line3D::operator== (Line3D^ op1, Line3D^ op2)
{
	bool isNull1 = NullChecker::IsNull(op1);
	bool isNull2 = NullChecker::IsNull(op2);

	if (isNull1 && isNull2)
		return true;
	else if (isNull1 || isNull2)
		return false;

	return Equal<Line3D>(op1, op2);
}

bool Line3D::operator!= (Line3D^ op1, Line3D^ op2)
{
	bool isNull1 = NullChecker::IsNull(op1);
	bool isNull2 = NullChecker::IsNull(op2);

	if (isNull1 && isNull2)
		return false;
	else if (isNull1 || isNull2)
		return true;

	return !Equal<Line3D>(op1, op2);
}

#else
bool Line3D::operator== (const Line3D& rhs) const
{
	return Equal<Line3D>(THIS_OBJ, rhs);
}

bool Line3D::operator!= (const Line3D& rhs) const
{
	return !Equal<Line3D>(THIS_OBJ, rhs);
}

#endif*/

// rVertex가 Line내에 포함되어 있는지 알려준다.
bool Line3D::IsInclude(REF_CONST(Vertex3D) rVertex) CONST
{
	return _IsInclude<Vertex3D, Line3D, double>(rVertex, THIS_OBJ);
}

// noLimit이 true이면 실제 LineType에 상관없이 무한히 뻗은 직선이라 가정하고 계산한다.
// noLimit이 false이면 실제 LineType을 고려하여 가장 가까운 거리를 구한다.
double Line3D::GetDistance(REF_CONST(Vertex3D) rVertex, bool noLimit) CONST
{
	return _GetDistance<Vertex3D, Line3D, double>(rVertex, THIS_OBJ, noLimit);
}

// 현재 직선에서 rVertex 방향으로 dLen 만큼 떨어진 객체를 만들어 리턴한다.
// LineType은 현재 직선과 동일하다.
INSTANCE(Line3D) Line3D::Offset(REF_CONST(Vertex3D) rVertex, double dLen) CONST
{
	return _Offset<Line3D, Vertex3D, double>(THIS_OBJ, rVertex, dLen);
}

// v1, v2, v3를 지나는 평면을 기준으로 현재의 직선과 대칭되는 객체를 만들어 리턴한다.
// LineType은 현재 직선과 동일하다.
// v1, v2, v3가 평면을 구성하지 못할 경우 false를 리턴한다.
bool Line3D::Mirror(REF_CONST(Vertex3D) v1, REF_CONST(Vertex3D) v2, REF_CONST(Vertex3D) v3, OUT CBR(INSTANCE(Line3D)) rResult)
{
#ifdef DOTNET
	rResult = nullptr;
#endif

	double a, b, c, d;	// ax + by + cz + d = 0
	if (!Math::MakePlane(v1, v2, v3, a, b, c, d))
		return false;

	INSTANCE(Vertex3D) _vBegin = Math::GetNearestVertex(m_vBegin, a, b, c, d);
	INSTANCE(Vertex3D) _vEnd   = Math::GetNearestVertex(m_vEnd, a, b, c, d);

	INSTANCE(Vertex3D) vBegin = _vBegin * 2 - m_vBegin;
	INSTANCE(Vertex3D) vEnd	  = _vEnd * 2 - m_vEnd;

	rResult = dnonlynew Line3D(vBegin, vEnd, m_lineType);
	return true;
}

Line2D::Line2D(void)
{
}

Line2D::Line2D(LineType type)
{
	SetLineType(type);
}

Line2D::Line2D(Line2DRefConst rhs)
{
	SetLineType(OF(rhs, m_lineType));
	SetVertex(OF(rhs, m_vBegin), true);
	SetVertex(OF(rhs, m_vEnd), false);
}

Line2D::Line2D(Vertex2DRefConst vBegin, Vertex2DRefConst vEnd)
{
	SetVertex(vBegin, true);
	SetVertex(vEnd, false);
}

Line2D::Line2D(Vertex2DRefConst vBegin, Vertex2DRefConst vEnd, LineType type)
{
	SetLineType(type);
	SetVertex(vBegin, true);
	SetVertex(vEnd, false);
}

Line2D::~Line2D(void)
{
}

/*#ifdef DOTNET
bool Line2D::operator== (Line2D^ op1, Line2D^ op2)
{
	bool isNull1 = NullChecker::IsNull(op1);
	bool isNull2 = NullChecker::IsNull(op2);

	if (isNull1 && isNull2)
		return true;
	else if (isNull1 || isNull2)
		return false;

	return Equal<Line2D>(op1, op2);
}

bool Line2D::operator!= (Line2D^ op1, Line2D^ op2)
{
	bool isNull1 = NullChecker::IsNull(op1);
	bool isNull2 = NullChecker::IsNull(op2);

	if (isNull1 && isNull2)
		return false;
	else if (isNull1 || isNull2)
		return true;

	return !Equal<Line2D>(op1, op2);
}

#else
bool Line2D::operator== (const Line2D& rhs) const
{
	return Equal<Line2D>(THIS_OBJ, rhs);
}

bool Line2D::operator!= (const Line2D& rhs) const
{
	return !Equal<Line2D>(THIS_OBJ, rhs);
}

#endif*/

// rLine과 만나지 않으면 0을 리턴한다.
// 교차점이 하나만 존재할 경우 rVertex1에만 값이 담겨지며 1이 리턴된다.
// 교차점이 두 개 존재할 경우 rVertex1과 rVertex2에 각각 값이 담겨진다.
// 교차점이 두 개인 경우는 직선에 해당하기 때문에 rResultType을 읽어 어떠한 형태의 직선인지 알아낼 수 있다.
int Line2D::IntersectLine(REF_CONST(Line2D) rLine, CBR(INSTANCE(Vertex2D)) rVertex1, CBR(INSTANCE(Vertex2D)) rVertex2, CBR(LineType) rResultType)
{
#ifdef DOTNET
	rVertex1 = gcnew Vertex2D();
	rVertex2 = gcnew Vertex2D();
#endif

	REF_CONST(Vertex2D) vBegin1 = this->GetVertex(true);
	REF_CONST(Vertex2D) vEnd1 = this->GetVertex(false);
	REF_CONST(Vertex2D) vBegin2 = OF(rLine, GetVertex(true));
	REF_CONST(Vertex2D) vEnd2 = OF(rLine, GetVertex(false));

	double dLen1 = OF(vBegin1, GetDistance(vEnd1));
	double dLen2 = OF(vBegin2, GetDistance(vEnd2));

	// this Line이 한 점일 경우
	if (dLen1 < UnE::Geometry::Math::HALF_TOLERANCE())
	{
		if (dLen2 < UnE::Geometry::Math::HALF_TOLERANCE())
		{
			if (OF(vBegin1, GetDistance(vBegin2)) < UnE::Geometry::Math::HALF_TOLERANCE())
			{
				OF(rVertex1, SetVertex(OF(vBegin1, x), OF(vBegin1, y)));
				return 1;
			}
		}
		else
		{
			double dLen3 = OF(vBegin1, GetDistance(vBegin2));
			double dLen4 = OF(vBegin1, GetDistance(vEnd2));

			if (dLen3 < UnE::Geometry::Math::HALF_TOLERANCE() ||
				dLen4 < UnE::Geometry::Math::HALF_TOLERANCE())
			{
				OF(rVertex1, SetVertex(OF(vBegin1, x), OF(vBegin1, y)));
				return 1;
			}

			double dAngle = UnE::Geometry::Math::GetAngle(vBegin2, vBegin1, vEnd2);

			if (OF(rLine, m_lineType) == ENUM_OF(LineType, LINE))
			{
				if (dAngle < UnE::Geometry::Math::HALF_TOLERANCE() ||
					Math::PI() - dAngle < UnE::Geometry::Math::HALF_TOLERANCE())
				{
					OF(rVertex1, SetVertex(OF(vBegin1, x), OF(vBegin1, y)));
					return 1;
				}
			}
			else if (OF(rLine, m_lineType) == ENUM_OF(LineType, HALF_LINE_BEGIN_2_END))
			{
				if (Math::PI() - dAngle < UnE::Geometry::Math::HALF_TOLERANCE())
				{
					OF(rVertex1, SetVertex(OF(vBegin1, x), OF(vBegin1, y)));
					return 1;
				}
				else if (dAngle < UnE::Geometry::Math::HALF_TOLERANCE())
				{
					if (dLen4 < dLen3)
					{
						OF(rVertex1, SetVertex(OF(vBegin1, x), OF(vBegin1, y)));
						return 1;
					}
				}
			}
			else if (OF(rLine, m_lineType) == ENUM_OF(LineType, HALF_LINE_END_2_BEGIN))
			{
				if (Math::PI() - dAngle < UnE::Geometry::Math::HALF_TOLERANCE())
				{
					OF(rVertex1, SetVertex(OF(vBegin1, x), OF(vBegin1, y)));
					return 1;
				}
				else if (dAngle < UnE::Geometry::Math::HALF_TOLERANCE())
				{
					if (dLen3 < dLen4)
					{
						OF(rVertex1, SetVertex(OF(vBegin1, x), OF(vBegin1, y)));
						return 1;
					}
				}
			}
			else if (OF(rLine, m_lineType) == ENUM_OF(LineType, SEGMENT))
			{
				if (Math::PI() - dAngle < UnE::Geometry::Math::HALF_TOLERANCE())
				{
					OF(rVertex1, SetVertex(OF(vBegin1, x), OF(vBegin1, y)));
					return 1;
				}
			}
		}

		return 0;
	}
	// rLine이 한 점일 경우
	else if (dLen2 < UnE::Geometry::Math::HALF_TOLERANCE())
	{
		double dLen3 = OF(vBegin2, GetDistance(vBegin1));
		double dLen4 = OF(vBegin2, GetDistance(vEnd1));

		if (dLen3 < UnE::Geometry::Math::HALF_TOLERANCE() ||
			dLen4 < UnE::Geometry::Math::HALF_TOLERANCE())
		{
			OF(rVertex1, SetVertex(OF(vBegin2, x), OF(vBegin2, y)));
			return 1;
		}

		double dAngle = UnE::Geometry::Math::GetAngle(vBegin1, vBegin2, vEnd1);

		if (this->m_lineType == ENUM_OF(LineType, LINE))
		{
			if (dAngle < UnE::Geometry::Math::HALF_TOLERANCE() ||
				Math::PI() - dAngle < UnE::Geometry::Math::HALF_TOLERANCE())
			{
				OF(rVertex1, SetVertex(OF(vBegin2, x), OF(vBegin2, y)));
				return 1;
			}
		}
		else if (this->m_lineType == ENUM_OF(LineType, HALF_LINE_BEGIN_2_END))
		{
			if (Math::PI() - dAngle < UnE::Geometry::Math::HALF_TOLERANCE())
			{
				OF(rVertex1, SetVertex(OF(vBegin2, x), OF(vBegin2, y)));
				return 1;
			}
			else if (dAngle < UnE::Geometry::Math::HALF_TOLERANCE())
			{
				if (dLen4 < dLen3)
				{
					OF(rVertex1, SetVertex(OF(vBegin2, x), OF(vBegin2, y)));
					return 1;
				}
			}
		}
		else if (this->m_lineType == ENUM_OF(LineType, HALF_LINE_END_2_BEGIN))
		{
			if (Math::PI() - dAngle < UnE::Geometry::Math::HALF_TOLERANCE())
			{
				OF(rVertex1, SetVertex(OF(vBegin2, x), OF(vBegin2, y)));
				return 1;
			}
			else if (dAngle < UnE::Geometry::Math::HALF_TOLERANCE())
			{
				if (dLen3 < dLen4)
				{
					OF(rVertex1, SetVertex(OF(vBegin2, x), OF(vBegin2, y)));
					return 1;
				}
			}
		}
		else if (this->m_lineType == ENUM_OF(LineType, SEGMENT))
		{
			if (Math::PI() - dAngle < UnE::Geometry::Math::HALF_TOLERANCE())
			{
				OF(rVertex1, SetVertex(OF(vBegin2, x), OF(vBegin2, y)));
				return 1;
			}
		}

		return 0;
	}

	Handle(Geom2d_Line) hLine1 = GCE2D_MAKELINE((THIS_OBJ));
	Handle(Geom2d_Line) hLine2 = GCE2D_MAKELINE(rLine);

	Geom2dAPI_InterCurveCurve inter(hLine1, hLine2);

	int nSegmentCount = inter.NbSegments();

	if (nSegmentCount > 0)
	{
		LineType type1 = this->GetLineType();
		LineType type2 = OF(rLine, GetLineType());

		if (type1 == ENUM_OF(LineType, LINE))
		{
			if (type2 == ENUM_OF(LineType, LINE))
			{
				rResultType = ENUM_OF(LineType, LINE);
				OF(rVertex1, CopyFrom(this->GetVertex(true)));
				OF(rVertex2, CopyFrom(this->GetVertex(false)));
			}
			else// if (type2 == ENUM_OF(LineType, HALF_LINE_BEGIN_2_END) || type2 == ENUM_OF(LineType, HALF_LINE_END_2_BEGIN) || type2 == ENUM_OF(LineType, SEGMENT))
			{
				rResultType = OF(rLine, GetLineType());
				OF(rVertex1, CopyFrom(OF(rLine, GetVertex(true))));
				OF(rVertex2, CopyFrom(OF(rLine, GetVertex(false))));
			}
		}
		else if (type1 == ENUM_OF(LineType, HALF_LINE_BEGIN_2_END) || type1 == ENUM_OF(LineType, HALF_LINE_END_2_BEGIN))
		{
			if (type2 == ENUM_OF(LineType, LINE))
			{
				rResultType = this->GetLineType();
				OF(rVertex1, CopyFrom(this->GetVertex(true)));
				OF(rVertex2, CopyFrom(this->GetVertex(false)));
			}
			else if (type2 == ENUM_OF(LineType, HALF_LINE_BEGIN_2_END) || type2 == ENUM_OF(LineType, HALF_LINE_END_2_BEGIN))
			{
				return Line2D::HalfLineToHalfLine(THIS_OBJ, rLine, rVertex1, rVertex2, rResultType);
			}
			else// if (type2 == SEGMENT)
			{
				return Line2D::HalfLineToSegment(THIS_OBJ, rLine, rVertex1, rVertex2, rResultType);
			}
		}
		else// if (type1 == SEGMENT)
		{
			if (type2 == ENUM_OF(LineType, LINE))
			{
				rResultType = this->GetLineType();
				OF(rVertex1, CopyFrom(this->GetVertex(true)));
				OF(rVertex2, CopyFrom(this->GetVertex(false)));
			}
			else if (type2 == ENUM_OF(LineType, HALF_LINE_BEGIN_2_END) || type2 == ENUM_OF(LineType, HALF_LINE_END_2_BEGIN))
			{
				return Line2D::HalfLineToSegment(rLine, THIS_OBJ, rVertex1, rVertex2, rResultType);
			}
			else// if (type2 == SEGMENT)
			{
				return Line2D::SegmentToSegment(THIS_OBJ, rLine, rVertex1, rVertex2, rResultType);
			}
		}

		return 2;
	}

	int nPointCount = inter.NbPoints();

	if (nPointCount == 1)
	{
		gp_Pnt2d pt = inter.Point(1);
		OF(rVertex1, SetVertex(pt.X(), pt.Y()));

		if (IsInclude(rVertex1) == 0 || OF(rLine, IsInclude(rVertex1)) == 0)
			nPointCount = 0;
	}

	return nPointCount;
}

int Line2D::HalfLineToHalfLine(REF_CONST(Line2D) rLine1, REF_CONST(Line2D) rLine2, REF(Vertex2D) rVertex1, REF(Vertex2D) rVertex2, CBR(LineType) rResultType)
{
	return _HalfLineToHalfLine<Line2D, Vertex2D, Line2D::LineType>(rLine1, rLine2, rVertex1, rVertex2, rResultType);
	/*LineType type1 = OF(rLine1, GetLineType());
	LineType type2 = OF(rLine2, GetLineType());

	REF_CONST(Vertex2D) vLine1Fixed  = type1 == ENUM_OF(LineType, HALF_LINE_BEGIN_2_END) ? OF(rLine1, m_vBegin) : OF(rLine1, m_vEnd);
	REF_CONST(Vertex2D) vLine1Opened = type1 == ENUM_OF(LineType, HALF_LINE_BEGIN_2_END) ? OF(rLine1, m_vEnd) : OF(rLine1, m_vBegin);
	REF_CONST(Vertex2D) vLine2Fixed  = type2 == ENUM_OF(LineType, HALF_LINE_BEGIN_2_END) ? OF(rLine2, m_vBegin) : OF(rLine2, m_vEnd);
	REF_CONST(Vertex2D) vLine2Opened = type2 == ENUM_OF(LineType, HALF_LINE_BEGIN_2_END) ? OF(rLine2, m_vEnd) : OF(rLine2, m_vBegin);

	bool include1 = OF(rLine2, IsInclude(vLine1Fixed));
	bool include2 = OF(rLine1, IsInclude(vLine2Fixed));

	if (include1 && include2)
	{
		INSTANCE(Vertex2D) vertex = Math::GetLinearVertex(vLine1Fixed, vLine1Opened, -100.0);

		if (OF(rLine2, IsInclude(vertex)))
		{
			// 두 Line이 동일한 경우
			OF(rVertex1, CopyFrom(vLine1Fixed));
			OF(rVertex2, CopyFrom(vLine1Opened));
			rResultType = type1;
		}
		else
		{
			// 두 Line이 반대 방향이며, 겹치는 부분이 하나의 선분을 이루는 경우
			OF(rVertex1, CopyFrom(vLine1Fixed));
			OF(rVertex2, CopyFrom(vLine2Fixed));
			rResultType = ENUM_OF(LineType, SEGMENT);
		}
	}
	else if (include1)
	{
		// rLine1이 rLine2에 포함되는 경우
		OF(rVertex1, CopyFrom(vLine1Fixed));
		OF(rVertex2, CopyFrom(vLine1Opened));
		rResultType = type1;
	}
	else if (include2)
	{
		// rLine2가 rLine1에 포함되는 경우
		OF(rVertex1, CopyFrom(vLine2Fixed));
		OF(rVertex2, CopyFrom(vLine2Opened));
		rResultType = type2;
	}
	else
		return 0;

	return 2;*/
}

int Line2D::HalfLineToSegment(REF_CONST(Line2D) rHalfLine, REF_CONST(Line2D) rSegment, REF(Vertex2D) rVertex1, REF(Vertex2D) rVertex2, CBR(LineType) rResultType)
{
	return _HalfLineToSegment<Line2D, Vertex2D, Line2D::LineType>(rHalfLine, rSegment, rVertex1, rVertex2, rResultType);
	/*LineType type1 = OF(rHalfLine, GetLineType());
	
	REF_CONST(Vertex2D) vLine1Fixed  = type1 == ENUM_OF(LineType, HALF_LINE_BEGIN_2_END) ? OF(rHalfLine, m_vBegin) : OF(rHalfLine, m_vEnd);
	REF_CONST(Vertex2D) vLine1Opened = type1 == ENUM_OF(LineType, HALF_LINE_BEGIN_2_END) ? OF(rHalfLine, m_vEnd) : OF(rHalfLine, m_vBegin);
	
	bool include1 = OF(rHalfLine, IsInclude(OF(rSegment, m_vBegin)));
	bool include2 = OF(rHalfLine, IsInclude(OF(rSegment, m_vEnd)));

	if (include1 && include2)
	{
		// rSegment가 rHalfLine에 완전히 포함되는 경우
		OF(rVertex1, CopyFrom(OF(rSegment, m_vBegin)));
		OF(rVertex2, CopyFrom(OF(rSegment, m_vEnd)));
		rResultType = ENUM_OF(LineType, SEGMENT);
	}
	else if (include1)
	{
		if (vLine1Fixed == OF(rSegment, m_vBegin))
		{
			// 두 직선이 한 점에서 만나는 경우
			OF(rVertex1, CopyFrom(vLine1Fixed));
			rResultType = ENUM_OF(LineType, NO_LINE);
			return 1;
		}
		else
		{
			// 두 직선의 겹침 구간이 하나의 선분을 이루는 경우
			OF(rVertex1, CopyFrom(vLine1Fixed));
			OF(rVertex2, CopyFrom(OF(rSegment, m_vBegin)));
			rResultType = ENUM_OF(LineType, SEGMENT);
		}
	}
	else if (include2)
	{
		if (vLine1Fixed == OF(rSegment, m_vEnd))
		{
			// 두 직선이 한 점에서 만나는 경우
			OF(rVertex1, CopyFrom(vLine1Fixed));
			rResultType = ENUM_OF(LineType, NO_LINE);
			return 1;
		}
		else
		{
			// 두 직선의 겹침 구간이 하나의 선분을 이루는 경우
			OF(rVertex1, CopyFrom(vLine1Fixed));
			OF(rVertex2, CopyFrom(OF(rSegment, m_vEnd)));
			rResultType = ENUM_OF(LineType, SEGMENT);
		}
	}
	else
		return 0;

	return 2;*/
}

int Line2D::SegmentToSegment(REF_CONST(Line2D) rLine1, REF_CONST(Line2D) rLine2, REF(Vertex2D) rVertex1, REF(Vertex2D) rVertex2, CBR(LineType) rResultType)
{
	return _SegmentToSegment<Line2D, Vertex2D, Line2D::LineType>(rLine1, rLine2, rVertex1, rVertex2, rResultType);
	/*REF_CONST(Vertex2D) vLine1Begin = OF(rLine1, m_vBegin);
	REF_CONST(Vertex2D) vLine1End = OF(rLine1, m_vEnd);
	REF_CONST(Vertex2D) vLine2Begin = OF(rLine2, m_vBegin);
	REF_CONST(Vertex2D) vLine2End = OF(rLine2, m_vEnd);
	
	bool include1 = OF(rLine2, IsInclude(vLine1Begin));
	bool include2 = OF(rLine2, IsInclude(vLine1End));
	bool include3 = OF(rLine1, IsInclude(vLine2Begin));
	bool include4 = OF(rLine1, IsInclude(vLine2End));

	if (include1 && include2)
	{
		// rLine1이 rLine2에 완전히 포함되는 경우
		OF(rVertex1, CopyFrom(vLine1Begin));
		OF(rVertex2, CopyFrom(vLine1End));
		rResultType = ENUM_OF(LineType, SEGMENT);
	}
	else if (include1)
	{
		if (vLine1Begin == vLine2Begin || vLine1Begin == vLine2End)
		{
			// 두 직선이 한 점에서 만나는 경우
			OF(rVertex1, CopyFrom(vLine1Begin));
			rResultType = ENUM_OF(LineType, NO_LINE);
			return 1;
		}
		else
		{
			// 두 직선의 겹침 구간이 하나의 선분을 이루는 경우
			REF_CONST(Vertex2D) rVertex = OF(rLine1, IsInclude(vLine2Begin)) ? vLine2Begin : vLine2End;
			
			OF(rVertex1, CopyFrom(rVertex));
			OF(rVertex2, CopyFrom(vLine1Begin));
			rResultType = ENUM_OF(LineType, SEGMENT);
		}
	}
	else if (include2)
	{
		if (vLine1End == vLine2Begin || vLine1End == vLine2End)
		{
			// 두 직선이 한 점에서 만나는 경우
			OF(rVertex1, CopyFrom(vLine1End));
			rResultType = ENUM_OF(LineType, NO_LINE);
			return 1;
		}
		else
		{
			// 두 직선의 겹침 구간이 하나의 선분을 이루는 경우
			REF_CONST(Vertex2D) rVertex = OF(rLine1, IsInclude(vLine2Begin)) ? vLine2Begin : vLine2End;
			
			OF(rVertex1, CopyFrom(rVertex));
			OF(rVertex2, CopyFrom(vLine1End));
			rResultType = ENUM_OF(LineType, SEGMENT);
		}
	}
	else
	{
		if (include3 && include4)
		{
			// rLine2가 rLine1에 완전히 포함되는 경우
			OF(rVertex1, CopyFrom(vLine2Begin));
			OF(rVertex2, CopyFrom(vLine2End));
			rResultType = ENUM_OF(LineType, SEGMENT);
		}
		else
			return 0;
	}

	return 2;*/
}

// rVertex가 Line내에 포함되어 있는지 알려준다.
bool Line2D::IsInclude(REF_CONST(Vertex2D) rVertex) CONST
{
	return _IsInclude<Vertex2D, Line2D, double>(rVertex, THIS_OBJ);
}

// noLimit이 true이면 실제 LineType에 상관없이 무한히 뻗은 직선이라 가정하고 계산한다.
// noLimit이 false이면 실제 LineType을 고려하여 가장 가까운 거리를 구한다.
double Line2D::GetDistance(REF_CONST(Vertex2D) rVertex, bool noLimit) CONST
{
	return _GetDistance<Vertex2D, Line2D, double>(rVertex, THIS_OBJ, noLimit);
}

// rEArc와 만나지 않으면 0을 리턴한다.
// 교차점이 하나만 존재할 경우 rVertex1에만 값이 담겨지며 1이 리턴된다.
// 교차점이 두 개 존재할 경우 rVertex1과 rVertex2에 각각 값이 담겨지며, 2가 리턴된다.
int Line2D::IntersectEArc(REF_CONST(EArc2D) rEArc, OUT CBR(INSTANCE(Vertex2D)) rVertex1, OUT CBR(INSTANCE(Vertex2D)) rVertex2) CONST
{
	return OF(rEArc, IntersectLine(THIS_OBJ, rVertex1, rVertex2));
}

// 현재 직선에서 rVertex 방향으로 dLen 만큼 떨어진 객체를 만들어 리턴한다.
// LineType은 현재 직선과 동일하다.
INSTANCE(Line2D) Line2D::Offset(REF_CONST(Vertex2D) rVertex, double dLen) CONST
{
	return _Offset<Line2D, Vertex2D, double>(THIS_OBJ, rVertex, dLen);
}

// 현재 직선에서 오른쪽 방향으로(rightSide가 false이면 왼쪽 방향) dLen 만큼 떨어진 객체를 만들어 리턴한다.
// LineType은 현재 직선과 동일하다.
// 방향은 직선의 시작점과 끝점을 기준으로 판단한다.
INSTANCE(Line2D) Line2D::Offset(bool rightSide, double dLen) CONST
{
	if (!rightSide) dLen = -dLen;

	INSTANCE(Vertex2D) vBegin = Math::GetRightVertex(m_vBegin, m_vEnd, dLen);
	INSTANCE(Vertex2D) vEnd   = Math::GetRightVertex(m_vEnd, m_vBegin, -dLen);

	return dnonlynew Line2D(vBegin, vEnd, m_lineType);
}

// v1과 v2를 지나는 직선을 기준으로 현재의 직선과 대칭되는 객체를 만들어 리턴한다.
// LineType은 현재 직선과 동일하다.
// v1과 v2가 동일한 좌표일 경우 false를 리턴한다.
bool Line2D::Mirror(REF_CONST(Vertex2D) v1, REF_CONST(Vertex2D) v2, OUT CBR(INSTANCE(Line2D)) rResult)
{
#ifdef DOTNET
	rResult = nullptr;
#endif

	if (OF(v1, GetDistance(v2)) <= Math::HALF_TOLERANCE())
		return false;

	INSTANCE(Vertex2D) _v1 = Math::GetNearestVertex(m_vBegin, v1, v2, true);
	INSTANCE(Vertex2D) _v2 = Math::GetNearestVertex(m_vEnd, v1, v2, true);

	INSTANCE(Vertex2D) vBegin = _v1 * 2 - m_vBegin;
	INSTANCE(Vertex2D) vEnd	  = _v2 * 2 - m_vEnd;

	rResult = dnonlynew Line2D(vBegin, vEnd, m_lineType);
	return true;
}

Line2F::Line2F(void)
{
}

Line2F::Line2F(LineType type)
{
	SetLineType(type);
}

Line2F::Line2F(Line2FRefConst rhs)
{
	SetLineType(OF(rhs, m_lineType));
	SetVertex(OF(rhs, m_vBegin), true);
	SetVertex(OF(rhs, m_vEnd), false);
}

Line2F::Line2F(Vertex2FRefConst vBegin, Vertex2FRefConst vEnd)
{
	SetVertex(vBegin, true);
	SetVertex(vEnd, false);
}

Line2F::Line2F(Vertex2FRefConst vBegin, Vertex2FRefConst vEnd, LineType type)
{
	SetLineType(type);
	SetVertex(vBegin, true);
	SetVertex(vEnd, false);
}

Line2F::~Line2F(void)
{
}

/*#ifdef DOTNET
bool Line2F::operator== (Line2F^ op1, Line2F^ op2)
{
	bool isNull1 = NullChecker::IsNull(op1);
	bool isNull2 = NullChecker::IsNull(op2);

	if (isNull1 && isNull2)
		return true;
	else if (isNull1 || isNull2)
		return false;

	return Equal<Line2F>(op1, op2);
}

bool Line2F::operator!= (Line2F^ op1, Line2F^ op2)
{
	bool isNull1 = NullChecker::IsNull(op1);
	bool isNull2 = NullChecker::IsNull(op2);

	if (isNull1 && isNull2)
		return false;
	else if (isNull1 || isNull2)
		return true;

	return !Equal<Line2F>(op1, op2);
}

#else
bool Line2F::operator== (const Line2F& rhs) const
{
	return Equal<Line2F>(THIS_OBJ, rhs);
}

bool Line2F::operator!= (const Line2F& rhs) const
{
	return !Equal<Line2F>(THIS_OBJ, rhs);
}

#endif*/

// rLine과 만나지 않으면 0을 리턴한다.
// 교차점이 하나만 존재할 경우 rVertex1에만 값이 담겨지며 1이 리턴된다.
// 교차점이 두 개 존재할 경우 rVertex1과 rVertex2에 각각 값이 담겨진다.
// 교차점이 두 개인 경우는 직선에 해당하기 때문에 rResultType을 읽어 어떠한 형태의 직선인지 알아낼 수 있다.
int Line2F::IntersectLine(REF_CONST(Line2F) rLine, CBR(INSTANCE(Vertex2F)) rVertex1, CBR(INSTANCE(Vertex2F)) rVertex2, CBR(LineType) rResultType)
{
#ifdef DOTNET
	rVertex1 = gcnew Vertex2F();
	rVertex2 = gcnew Vertex2F();
#endif

	REF_CONST(Vertex2F) vBegin1 = this->GetVertex(true);
	REF_CONST(Vertex2F) vEnd1 = this->GetVertex(false);
	REF_CONST(Vertex2F) vBegin2 = OF(rLine, GetVertex(true));
	REF_CONST(Vertex2F) vEnd2 = OF(rLine, GetVertex(false));

	double dLen1 = OF(vBegin1, GetDistance(vEnd1));
	double dLen2 = OF(vBegin2, GetDistance(vEnd2));

	// this Line이 한 점일 경우
	if (dLen1 < UnE::Geometry::Math::HALF_TOLERANCE())
	{
		if (dLen2 < UnE::Geometry::Math::HALF_TOLERANCE())
		{
			if (OF(vBegin1, GetDistance(vBegin2)) < UnE::Geometry::Math::HALF_TOLERANCE())
			{
				OF(rVertex1, SetVertex(OF(vBegin1, x), OF(vBegin1, y)));
				return 1;
			}
		}
		else
		{
			double dLen3 = OF(vBegin1, GetDistance(vBegin2));
			double dLen4 = OF(vBegin1, GetDistance(vEnd2));

			if (dLen3 < UnE::Geometry::Math::HALF_TOLERANCE() ||
				dLen4 < UnE::Geometry::Math::HALF_TOLERANCE())
			{
				OF(rVertex1, SetVertex(OF(vBegin1, x), OF(vBegin1, y)));
				return 1;
			}

			double dAngle = UnE::Geometry::Math::GetAngle(vBegin2, vBegin1, vEnd2);

			if (OF(rLine, m_lineType) == ENUM_OF(LineType, LINE))
			{
				if (dAngle < UnE::Geometry::Math::HALF_TOLERANCE() ||
					Math::PI() - dAngle < UnE::Geometry::Math::HALF_TOLERANCE())
				{
					OF(rVertex1, SetVertex(OF(vBegin1, x), OF(vBegin1, y)));
					return 1;
				}
			}
			else if (OF(rLine, m_lineType) == ENUM_OF(LineType, HALF_LINE_BEGIN_2_END))
			{
				if (Math::PI() - dAngle < UnE::Geometry::Math::HALF_TOLERANCE())
				{
					OF(rVertex1, SetVertex(OF(vBegin1, x), OF(vBegin1, y)));
					return 1;
				}
				else if (dAngle < UnE::Geometry::Math::HALF_TOLERANCE())
				{
					if (dLen4 < dLen3)
					{
						OF(rVertex1, SetVertex(OF(vBegin1, x), OF(vBegin1, y)));
						return 1;
					}
				}
			}
			else if (OF(rLine, m_lineType) == ENUM_OF(LineType, HALF_LINE_END_2_BEGIN))
			{
				if (Math::PI() - dAngle < UnE::Geometry::Math::HALF_TOLERANCE())
				{
					OF(rVertex1, SetVertex(OF(vBegin1, x), OF(vBegin1, y)));
					return 1;
				}
				else if (dAngle < UnE::Geometry::Math::HALF_TOLERANCE())
				{
					if (dLen3 < dLen4)
					{
						OF(rVertex1, SetVertex(OF(vBegin1, x), OF(vBegin1, y)));
						return 1;
					}
				}
			}
			else if (OF(rLine, m_lineType) == ENUM_OF(LineType, SEGMENT))
			{
				if (Math::PI() - dAngle < UnE::Geometry::Math::HALF_TOLERANCE())
				{
					OF(rVertex1, SetVertex(OF(vBegin1, x), OF(vBegin1, y)));
					return 1;
				}
			}
		}

		return 0;
	}
	// rLine이 한 점일 경우
	else if (dLen2 < UnE::Geometry::Math::HALF_TOLERANCE())
	{
		double dLen3 = OF(vBegin2, GetDistance(vBegin1));
		double dLen4 = OF(vBegin2, GetDistance(vEnd1));

		if (dLen3 < UnE::Geometry::Math::HALF_TOLERANCE() ||
			dLen4 < UnE::Geometry::Math::HALF_TOLERANCE())
		{
			OF(rVertex1, SetVertex(OF(vBegin2, x), OF(vBegin2, y)));
			return 1;
		}

		double dAngle = UnE::Geometry::Math::GetAngle(vBegin1, vBegin2, vEnd1);

		if (this->m_lineType == ENUM_OF(LineType, LINE))
		{
			if (dAngle < UnE::Geometry::Math::HALF_TOLERANCE() ||
				Math::PI() - dAngle < UnE::Geometry::Math::HALF_TOLERANCE())
			{
				OF(rVertex1, SetVertex(OF(vBegin2, x), OF(vBegin2, y)));
				return 1;
			}
		}
		else if (this->m_lineType == ENUM_OF(LineType, HALF_LINE_BEGIN_2_END))
		{
			if (Math::PI() - dAngle < UnE::Geometry::Math::HALF_TOLERANCE())
			{
				OF(rVertex1, SetVertex(OF(vBegin2, x), OF(vBegin2, y)));
				return 1;
			}
			else if (dAngle < UnE::Geometry::Math::HALF_TOLERANCE())
			{
				if (dLen4 < dLen3)
				{
					OF(rVertex1, SetVertex(OF(vBegin2, x), OF(vBegin2, y)));
					return 1;
				}
			}
		}
		else if (this->m_lineType == ENUM_OF(LineType, HALF_LINE_END_2_BEGIN))
		{
			if (Math::PI() - dAngle < UnE::Geometry::Math::HALF_TOLERANCE())
			{
				OF(rVertex1, SetVertex(OF(vBegin2, x), OF(vBegin2, y)));
				return 1;
			}
			else if (dAngle < UnE::Geometry::Math::HALF_TOLERANCE())
			{
				if (dLen3 < dLen4)
				{
					OF(rVertex1, SetVertex(OF(vBegin2, x), OF(vBegin2, y)));
					return 1;
				}
			}
		}
		else if (this->m_lineType == ENUM_OF(LineType, SEGMENT))
		{
			if (Math::PI() - dAngle < UnE::Geometry::Math::HALF_TOLERANCE())
			{
				OF(rVertex1, SetVertex(OF(vBegin2, x), OF(vBegin2, y)));
				return 1;
			}
		}

		return 0;
	}

	Handle(Geom2d_Line) hLine1 = GCE2D_MAKELINE((THIS_OBJ));
	Handle(Geom2d_Line) hLine2 = GCE2D_MAKELINE(rLine);

	Geom2dAPI_InterCurveCurve inter(hLine1, hLine2);

	int nSegmentCount = inter.NbSegments();

	if (nSegmentCount > 0)
	{
		LineType type1 = this->GetLineType();
		LineType type2 = OF(rLine, GetLineType());

		if (type1 == ENUM_OF(LineType, LINE))
		{
			if (type2 == ENUM_OF(LineType, LINE))
			{
				rResultType = ENUM_OF(LineType, LINE);
				OF(rVertex1, CopyFrom(this->GetVertex(true)));
				OF(rVertex2, CopyFrom(this->GetVertex(false)));
			}
			else// if (type2 == ENUM_OF(LineType, HALF_LINE_BEGIN_2_END) || type2 == ENUM_OF(LineType, HALF_LINE_END_2_BEGIN) || type2 == ENUM_OF(LineType, SEGMENT))
			{
				rResultType = OF(rLine, GetLineType());
				OF(rVertex1, CopyFrom(OF(rLine, GetVertex(true))));
				OF(rVertex2, CopyFrom(OF(rLine, GetVertex(false))));
			}
		}
		else if (type1 == ENUM_OF(LineType, HALF_LINE_BEGIN_2_END) || type1 == ENUM_OF(LineType, HALF_LINE_END_2_BEGIN))
		{
			if (type2 == ENUM_OF(LineType, LINE))
			{
				rResultType = this->GetLineType();
				OF(rVertex1, CopyFrom(this->GetVertex(true)));
				OF(rVertex2, CopyFrom(this->GetVertex(false)));
			}
			else if (type2 == ENUM_OF(LineType, HALF_LINE_BEGIN_2_END) || type2 == ENUM_OF(LineType, HALF_LINE_END_2_BEGIN))
			{
				return Line2F::HalfLineToHalfLine(THIS_OBJ, rLine, rVertex1, rVertex2, rResultType);
			}
			else// if (type2 == SEGMENT)
			{
				return Line2F::HalfLineToSegment(THIS_OBJ, rLine, rVertex1, rVertex2, rResultType);
			}
		}
		else// if (type1 == SEGMENT)
		{
			if (type2 == ENUM_OF(LineType, LINE))
			{
				rResultType = this->GetLineType();
				OF(rVertex1, CopyFrom(this->GetVertex(true)));
				OF(rVertex2, CopyFrom(this->GetVertex(false)));
			}
			else if (type2 == ENUM_OF(LineType, HALF_LINE_BEGIN_2_END) || type2 == ENUM_OF(LineType, HALF_LINE_END_2_BEGIN))
			{
				return Line2F::HalfLineToSegment(rLine, THIS_OBJ, rVertex1, rVertex2, rResultType);
			}
			else// if (type2 == SEGMENT)
			{
				return Line2F::SegmentToSegment(THIS_OBJ, rLine, rVertex1, rVertex2, rResultType);
			}
		}

		return 2;
	}

	int nPointCount = inter.NbPoints();

	if (nPointCount == 1)
	{
		gp_Pnt2d pt = inter.Point(1);
		OF(rVertex1, SetVertex((float)pt.X(), (float)pt.Y()));

		if (IsInclude(rVertex1) == 0 || OF(rLine, IsInclude(rVertex1)) == 0)
			nPointCount = 0;
	}

	return nPointCount;
}

int Line2F::HalfLineToHalfLine(REF_CONST(Line2F) rLine1, REF_CONST(Line2F) rLine2, REF(Vertex2F) rVertex1, REF(Vertex2F) rVertex2, CBR(LineType) rResultType)
{
	return _HalfLineToHalfLine<Line2F, Vertex2F, Line2F::LineType>(rLine1, rLine2, rVertex1, rVertex2, rResultType);
}

int Line2F::HalfLineToSegment(REF_CONST(Line2F) rHalfLine, REF_CONST(Line2F) rSegment, REF(Vertex2F) rVertex1, REF(Vertex2F) rVertex2, CBR(LineType) rResultType)
{
	return _HalfLineToSegment<Line2F, Vertex2F, Line2F::LineType>(rHalfLine, rSegment, rVertex1, rVertex2, rResultType);
}

int Line2F::SegmentToSegment(REF_CONST(Line2F) rLine1, REF_CONST(Line2F) rLine2, REF(Vertex2F) rVertex1, REF(Vertex2F) rVertex2, CBR(LineType) rResultType)
{
	return _SegmentToSegment<Line2F, Vertex2F, Line2F::LineType>(rLine1, rLine2, rVertex1, rVertex2, rResultType);
}

// rVertex가 Line내에 포함되어 있는지 알려준다.
bool Line2F::IsInclude(REF_CONST(Vertex2F) rVertex) CONST
{
	return _IsInclude<Vertex2F, Line2F, double>(rVertex, THIS_OBJ);
}

// noLimit이 true이면 실제 LineType에 상관없이 무한히 뻗은 직선이라 가정하고 계산한다.
// noLimit이 false이면 실제 LineType을 고려하여 가장 가까운 거리를 구한다.
float Line2F::GetDistance(REF_CONST(Vertex2F) rVertex, bool noLimit) CONST
{
	return _GetDistance<Vertex2F, Line2F, float>(rVertex, THIS_OBJ, noLimit);
}

// rEArc와 만나지 않으면 0을 리턴한다.
// 교차점이 하나만 존재할 경우 rVertex1에만 값이 담겨지며 1이 리턴된다.
// 교차점이 두 개 존재할 경우 rVertex1과 rVertex2에 각각 값이 담겨지며, 2가 리턴된다.
int Line2F::IntersectEArc(REF_CONST(EArc2F) rEArc, OUT CBR(INSTANCE(Vertex2F)) rVertex1, OUT CBR(INSTANCE(Vertex2F)) rVertex2) CONST
{
	return OF(rEArc, IntersectLine(THIS_OBJ, rVertex1, rVertex2));
}

// 현재 직선에서 rVertex 방향으로 dLen 만큼 떨어진 객체를 만들어 리턴한다.
// LineType은 현재 직선과 동일하다.
INSTANCE(Line2F) Line2F::Offset(REF_CONST(Vertex2F) rVertex, float dLen) CONST
{
	return _Offset<Line2F, Vertex2F, float>(THIS_OBJ, rVertex, dLen);
}

// 현재 직선에서 오른쪽 방향으로(rightSide가 false이면 왼쪽 방향) dLen 만큼 떨어진 객체를 만들어 리턴한다.
// LineType은 현재 직선과 동일하다.
// 방향은 직선의 시작점과 끝점을 기준으로 판단한다.
INSTANCE(Line2F) Line2F::Offset(bool rightSide, float dLen) CONST
{
	if (!rightSide) dLen = -dLen;

	INSTANCE(Vertex2F) vBegin = Math::GetRightVertex(m_vBegin, m_vEnd, dLen);
	INSTANCE(Vertex2F) vEnd = Math::GetRightVertex(m_vEnd, m_vBegin, -dLen);

	return dnonlynew Line2F(vBegin, vEnd, m_lineType);
}

// v1과 v2를 지나는 직선을 기준으로 현재의 직선과 대칭되는 객체를 만들어 리턴한다.
// LineType은 현재 직선과 동일하다.
// v1과 v2가 동일한 좌표일 경우 false를 리턴한다.
bool Line2F::Mirror(REF_CONST(Vertex2F) v1, REF_CONST(Vertex2F) v2, OUT CBR(INSTANCE(Line2F)) rResult)
{
#ifdef DOTNET
	rResult = nullptr;
#endif

	if (OF(v1, GetDistance(v2)) <= Math::HALF_TOLERANCE())
		return false;

	INSTANCE(Vertex2F) _v1 = Math::GetNearestVertex(m_vBegin, v1, v2, true);
	INSTANCE(Vertex2F) _v2 = Math::GetNearestVertex(m_vEnd, v1, v2, true);

	INSTANCE(Vertex2F) vBegin = _v1 * 2 - m_vBegin;
	INSTANCE(Vertex2F) vEnd = _v2 * 2 - m_vEnd;

	rResult = dnonlynew Line2F(vBegin, vEnd, m_lineType);
	return true;
}

END_NS
END_NS
