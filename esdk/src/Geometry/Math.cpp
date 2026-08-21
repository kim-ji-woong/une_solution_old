#include "StdAfx.h"
#include "GVertex.h"
#include "GMath.h"
#include "GLine.h"
#include <Math.h>

BEGIN_NS(UnE)
BEGIN_NS(Geometry)

#ifndef DOTNET
	double Math::_COORD_TOLERANCE = 0.0000001;
	float  Math::_HALF_TOLERANCE  = 0.0001f;
	double Math::_HALF_PI			= 1.57079632679489661923;
	double Math::_PI				= 3.14159265358979323846;
	double Math::__3HALF_PI		= 4.71238898038468985769;
	double Math::__2PI			= 6.28318530717958647692;
#endif

double Math::COORD_TOLERANCE()
{
	return Math::_COORD_TOLERANCE;
}

float Math::HALF_TOLERANCE()
{
	return Math::_HALF_TOLERANCE;
}

double Math::HALF_PI()
{
	return Math::_HALF_PI;
}

double Math::PI()
{
	return Math::_PI;
}

double Math::_3HALF_PI()
{
	return Math::__3HALF_PI;
}

double Math::_2PI()
{
	return Math::__2PI;
}

double Math::RadToDeg(double dRadian)
{
	return 180.0 * dRadian / Math::_PI;
}

double Math::DegToRad(double dDegree)
{
	return Math::_PI * dDegree / 180.0;
}

// 소수점 몇자리까지 허용할 것인가를 판단한 다음
// 값을 넘겨준다.
double Math::GetTolerance(double data)
{
	int nCount = (int)log10(data) + 1;
	nCount = 10 - nCount;

	double dTolerance = 0.1;

	for (int i=1;i<nCount;i++)
	{
		dTolerance /= 10.0;
	}

	return dTolerance;
}

void Math::SetHalfTolerance(float fTolerance)
{
	_HALF_TOLERANCE = fTolerance;
}

void Math::SetCoordTolerance(double dTolerance)
{
	_COORD_TOLERANCE = dTolerance;
}

/*double Math::GetDistance(REF_CONST(Vertex3D) v1, REF_CONST(Vertex3D) v2)
{
	double x = OF(v1, x) - OF(v2, x);
	double y = OF(v1, y) - OF(v2, y);
	double z = OF(v1, z) - OF(v2, z);

	return sqrt(x * x + y * y + z * z);
}

float Math::GetDistance(REF_CONST(Vertex3F) v1, REF_CONST(Vertex3F) v2)
{
	double x = OF(v1, x) - OF(v2, x);
	double y = OF(v1, y) - OF(v2, y);
	double z = OF(v1, z) - OF(v2, z);

	return (float)sqrt(x * x + y * y + z * z);
}

double Math::GetDistance(REF_CONST(Vertex2D) v1, REF_CONST(Vertex2D) v2)
{
	double x = OF(v1, x) - OF(v2, x);
	double y = OF(v1, y) - OF(v2, y);
	
	return sqrt(x * x + y * y);
}

float Math::GetDistance(REF_CONST(Vertex2F) v1, REF_CONST(Vertex2F) v2)
{
	double x = OF(v1, x) - OF(v2, x);
	double y = OF(v1, y) - OF(v2, y);
	
	return (float)sqrt(x * x + y * y);
}*/

template <class VertexRefConst>
static double _GetAngle(VertexRefConst v1, VertexRefConst vCenter, VertexRefConst v2)
{
	// 코사인 제2법칙
	// C²= A²+ B²- 2ABcosΘ
	double a = OF(v1, GetDistance(vCenter));
	double b = OF(v2, GetDistance(vCenter));
	double c = OF(v1, GetDistance(v2));

	double cosData = (a * a + b * b - c * c) / 2 / a / b;
	if (cosData < -1.0) cosData = -1.0;
	else if (cosData > 1.0) cosData = 1.0;

	return acos(cosData);
}

// v1과 vCenter가 이루는 직선과 vCenter와 v2가 이루는
// 직선이 서로 만나 이루는 각을 리턴한다.
// Return 값 : Radian
double Math::GetAngle(REF_CONST(Vertex3D) v1, REF_CONST(Vertex3D) vCenter, REF_CONST(Vertex3D) v2)
{
	return _GetAngle<REF_CONST(Vertex3D)>(v1, vCenter, v2);
}

double Math::GetAngle(REF_CONST(Vertex3F) v1, REF_CONST(Vertex3F) vCenter, REF_CONST(Vertex3F) v2)
{
	return _GetAngle<REF_CONST(Vertex3F)>(v1, vCenter, v2);
}

double Math::GetAngle(REF_CONST(Vertex2D) v1, REF_CONST(Vertex2D) vCenter, REF_CONST(Vertex2D) v2)
{
	return _GetAngle<REF_CONST(Vertex2D)>(v1, vCenter, v2);
}

double Math::GetAngle(REF_CONST(Vertex2F) v1, REF_CONST(Vertex2F) vCenter, REF_CONST(Vertex2F) v2)
{
	return _GetAngle<REF_CONST(Vertex2F)>(v1, vCenter, v2);
}

template <class Vertex, class Real>
static INSTANCE(Vertex) _GetLinearVertex(REF_CONST(Vertex) v1, REF_CONST(Vertex) v2, Real dLength)
{
	// v1과 v2 사이의 거리
	Real dL = OF(v1, GetDistance(v2));

	if (dL <= Math::COORD_TOLERANCE())
		return dnonlynew Vertex(v1);
	//if (dL == 0.0f) return dnonlynew Vertex(v1);

	INSTANCE(Vertex) v3 = v1 + (v2 - v1) * dLength / dL;
	return v3;
}

// v1과 v2를 잇는 직선상에서 v1으로부터 v2 방향으로 dLength 만큼
// 떨어진 거리의 점을 구한다.
INSTANCE(Vertex3D) Math::GetLinearVertex(REF_CONST(Vertex3D) v1, REF_CONST(Vertex3D) v2, double dLength)
{
	return _GetLinearVertex<Vertex3D, double>(v1, v2, dLength);
}

Vertex3FInstance Math::GetLinearVertex(REF_CONST(Vertex3F) v1, REF_CONST(Vertex3F) v2, float fLength)
{
	return _GetLinearVertex<Vertex3F, float>(v1, v2, fLength);
}

INSTANCE(Vertex2D) Math::GetLinearVertex(REF_CONST(Vertex2D) v1, REF_CONST(Vertex2D) v2, double dLength)
{
	return _GetLinearVertex<Vertex2D, double>(v1, v2, dLength);
}

Vertex2FInstance Math::GetLinearVertex(REF_CONST(Vertex2F) v1, REF_CONST(Vertex2F) v2, float fLength)
{
	return _GetLinearVertex<Vertex2F, float>(v1, v2, fLength);
}

template <class Vertex, class Real>
static INSTANCE(Vertex) _GetRightVertex(REF_CONST(Vertex) v1, REF_CONST(Vertex) v2, Real dDistance)
{
	Real dLen = OF(v1, GetDistance(v2));
	if (dLen == 0.0) return dnonlynew Vertex(v1);

	INSTANCE(Vertex) vResult = dnonlynew Vertex();
	OF(vResult, x) = dDistance / dLen * (OF(v1, y) - OF(v2, y)) + OF(v1, x);
	OF(vResult, y) = dDistance / dLen * (OF(v2, x) - OF(v1, x)) + OF(v1, y);
	return vResult;
}

// v1과 v2를 지나는 직선과 수직이며 v1을 지나는 직선이 있다.
// 이 직선상에 존재하며 v1으로부터 거리 dDistance 만큼 오른쪽(XY 좌표계에서 v2를 원점,
// v1을 양의 Y축에 놓았을 경우)으로 떨어진 거리의 점을 구한다.
INSTANCE(Vertex2D) Math::GetRightVertex(REF_CONST(Vertex2D) v1, REF_CONST(Vertex2D) v2, double dDistance)
{
	return _GetRightVertex<Vertex2D, double>(v1, v2, dDistance);
}

Vertex2FInstance Math::GetRightVertex(REF_CONST(Vertex2F) v1, REF_CONST(Vertex2F) v2, float fDistance)
{
	return _GetRightVertex<Vertex2F, float>(v1, v2, fDistance);
}

template <class Vertex, class Real>
static int _IsRightSideFromLine(REF_CONST(Vertex) rVertex, REF_CONST(Vertex) vBegin, REF_CONST(Vertex) vEnd)
{
	INSTANCE(Vertex) vR = Math::GetRightVertex(vBegin, vEnd, (Real)100.0f);
	double dAngle1 = Math::GetAngle(vEnd, vBegin, rVertex);
	double dAngle2 = Math::GetAngle(rVertex, vBegin, vR);

	INSTANCE(Vertex) v;

	if (dAngle1 < Math::HALF_PI())
	{
		Real dLen = OF(vBegin, GetDistance(rVertex));
		v = Math::GetLinearVertex(vBegin, vEnd, dLen);
	}
	else
	{
		Real dLen = OF(vEnd, GetDistance(rVertex));
		v = Math::GetLinearVertex(vEnd, vBegin, dLen);
	}

	if (OF(v, GetDistance(rVertex)) <= Math::COORD_TOLERANCE()) return -1;

	if (dAngle2 < Math::HALF_PI()) return 1;
	return 0;
}

// vBegin과 vEnd를 잇는 직선이 있다.
// 가상 좌표계에서 vEnd를 원점, vBegin을 양의 Y축에 있다고 가정하였을 때,
// rVertex가 양의 X축에 있는지 여부를 알려준다.
// Return 값 : 1 (직선의 오른쪽에 있다. => 양의 X축)
//             0 (직선의 왼쪽에 있다. => 음의 X축)
//            -1 (직선위에 존재한다.)
int Math::IsRightSideFromLine(REF_CONST(Vertex2D) rVertex, REF_CONST(Vertex2D) vBegin, REF_CONST(Vertex2D) vEnd)
{
	return _IsRightSideFromLine<Vertex2D, double>(rVertex, vBegin, vEnd);
}

int Math::IsRightSideFromLine(REF_CONST(Vertex2F) rVertex, REF_CONST(Vertex2F) vBegin, REF_CONST(Vertex2F) vEnd)
{
	return _IsRightSideFromLine<Vertex2F, float>(rVertex, vBegin, vEnd);
}

template <class Vertex, class Line, class Real>
static INSTANCE(Vertex) _GetNearestVertex(REF_CONST(Vertex) rVertex, REF_CONST(Vertex) vLineBegin, REF_CONST(Vertex) vLineEnd, bool noLimit)
{
	Real dLen = OF(rVertex, GetDistance(vLineBegin));
	Real dLen2 = OF(rVertex, GetDistance(vLineEnd));

	if (dLen <= Math::HALF_TOLERANCE() || dLen2 <= Math::HALF_TOLERANCE())
		return rVertex;

	double dAngle = Math::GetAngle(rVertex, vLineBegin, vLineEnd);
	Real dH = (Real)(dLen * cos(dAngle));

	INSTANCE(Vertex) vertex = Math::GetLinearVertex(vLineBegin, vLineEnd, dH);
	INSTANCE(Line) line = dnonlynew Line(vLineBegin, vLineEnd);

	//if (noLimit || Math::IsIncludeInLine(vertex, vLineBegin, vLineEnd))
	if (noLimit || OF(line, IsInclude(vertex)))
	{
		return vertex;
	}

	/*Real d1 = OF(rVertex, GetDistance(vLineBegin));
	Real d2 = OF(rVertex, GetDistance(vLineEnd));
	return d1 < d2 ? dnonlynew Vertex(vLineBegin) : dnonlynew Vertex(vLineEnd);*/
	return dLen < dLen2 ? dnonlynew Vertex(vLineBegin) : dnonlynew Vertex(vLineEnd);
}

// vLineBegin과 vLineEnd를 잇는 직선위에서 rVertex와 가장 가까운 점을 알려준다.
// bNoLimit : true이면 직선은 무한한 길이를 갖고 있으며, false이면 직선은 vLineBegin과 vLineEnd 사이의 제한된 길이를 가진다.
INSTANCE(Vertex3D) Math::GetNearestVertex(REF_CONST(Vertex3D) rVertex, REF_CONST(Vertex3D) vLineBegin, REF_CONST(Vertex3D) vLineEnd, bool noLimit)
{
	return _GetNearestVertex<Vertex3D, Line3D, double>(rVertex, vLineBegin, vLineEnd, noLimit);
}

Vertex3FInstance Math::GetNearestVertex(REF_CONST(Vertex3F) rVertex, REF_CONST(Vertex3F) vLineBegin, REF_CONST(Vertex3F) vLineEnd, bool noLimit)
{
	INSTANCE(Vertex3D) _rVertex = dnonlynew Vertex3D(OF(rVertex, x), OF(rVertex, y), OF(rVertex, z));
	INSTANCE(Vertex3D) _vLineBegin = dnonlynew Vertex3D(OF(vLineBegin, x), OF(vLineBegin, y), OF(vLineBegin, z));
	INSTANCE(Vertex3D) _vLineEnd = dnonlynew Vertex3D(OF(vLineEnd, x), OF(vLineEnd, y), OF(vLineEnd, z));

	INSTANCE(Vertex3D) vResult = _GetNearestVertex<Vertex3D, Line3D, double>(_rVertex, _vLineBegin, _vLineEnd, noLimit);
	return dnonlynew Vertex3F((float)OF(vResult, x), (float)OF(vResult, y), (float)OF(vResult, z));
}

INSTANCE(Vertex2D) Math::GetNearestVertex(REF_CONST(Vertex2D) rVertex, REF_CONST(Vertex2D) vLineBegin, REF_CONST(Vertex2D) vLineEnd, bool noLimit)
{
	return _GetNearestVertex<Vertex2D, Line2D, double>(rVertex, vLineBegin, vLineEnd, noLimit);
}

Vertex2FInstance Math::GetNearestVertex(REF_CONST(Vertex2F) rVertex, REF_CONST(Vertex2F) vLineBegin, REF_CONST(Vertex2F) vLineEnd, bool noLimit)
{
	return _GetNearestVertex<Vertex2F, Line2F, float>(rVertex, vLineBegin, vLineEnd, noLimit);
}

// 평면(ax + by + cz + d = 0) 위에서 rVertex와 가장 가까운 점을 알려준다.
INSTANCE(Vertex3D) Math::GetNearestVertex(REF_CONST(Vertex3D) rVertex, double a, double b, double c, double d)
{
	double k = -(a * OF(rVertex, x) + b * OF(rVertex, y) + c * OF(rVertex, z) + d) / (a * a + b * b + c * c);
	return dnonlynew Vertex3D(a * k + OF(rVertex, x), b * k + OF(rVertex, y), c * k + OF(rVertex, z));
}

/*template <class Vertex, class Real>
static bool _IsIncludeInLine(REF_CONST(Vertex) rVertex, REF_CONST(Vertex) vBegin, REF_CONST(Vertex) vEnd)
{
	double d1 = Math::GetDistance(rVertex, vBegin);
	double d2 = Math::GetDistance(rVertex, vEnd);
	if (d1 <= Math::COORD_TOLERANCE() || d2 <= Math::COORD_TOLERANCE()) return true;
	if (fabs(Math::GetAngle(vBegin, rVertex, vEnd) - Math::PI()) > Math::HALF_TOLERANCE()) return false;
	return true;
}

// rVertex가 vBegin과 vEnd 사이에 위치하는지 검사한다.
bool Math::IsIncludeInLine(REF_CONST(Vertex3D) rVertex, REF_CONST(Vertex3D) vBegin, REF_CONST(Vertex3D) vEnd)
{
	return _IsIncludeInLine<Vertex3D, double>(rVertex, vBegin, vEnd);
}

bool Math::IsIncludeInLine(REF_CONST(Vertex3F) rVertex, REF_CONST(Vertex3F) vBegin, REF_CONST(Vertex3F) vEnd)
{
	return _IsIncludeInLine<Vertex3F, float>(rVertex, vBegin, vEnd);
}

bool Math::IsIncludeInLine(REF_CONST(Vertex2D) rVertex, REF_CONST(Vertex2D) vBegin, REF_CONST(Vertex2D) vEnd)
{
	return _IsIncludeInLine<Vertex2D, double>(rVertex, vBegin, vEnd);
}

bool Math::IsIncludeInLine(REF_CONST(Vertex2F) rVertex, REF_CONST(Vertex2F) vBegin, REF_CONST(Vertex2F) vEnd)
{
	return _IsIncludeInLine<Vertex2F, float>(rVertex, vBegin, vEnd);
}*/

// v1, v2, v3를 지나는 평면의 방정식을 구한다.(ax + by + cz + d = 0)
// v1, v2, v3가 평면을 구성하지 못할 경우 false를 리턴한다.
bool Math::MakePlane(REF_CONST(Vertex3D) v1, REF_CONST(Vertex3D) v2, REF_CONST(Vertex3D) v3, OUT CBR(double) a, OUT CBR(double) b, OUT CBR(double) c, OUT CBR(double) d)
{
	a = b = c = d = 0.0;

	if (OF(v1, GetDistance(v2)) <= HALF_TOLERANCE() || OF(v2, GetDistance(v3)) <= HALF_TOLERANCE() || OF(v3, GetDistance(v1)) <= HALF_TOLERANCE())
		return false;

#ifdef DOTNET
	INSTANCE(Line3D) line = dnonlynew Line3D(v1, v2, Line3D::LineType::LINE);
#else
	INSTANCE(Line3D) line = dnonlynew Line3D(v1, v2, Line3D::LINE);
#endif

	if (OF(line, IsInclude(v3)))
		return false;

	a = OF(v1, y) * (OF(v2, z) - OF(v3, z)) + OF(v2, y) * (OF(v3, z) - OF(v1, z)) + OF(v3, y) * (OF(v1, z) - OF(v2, z));
	b = OF(v1, z) * (OF(v2, x) - OF(v3, x)) + OF(v2, z) * (OF(v3, x) - OF(v1, x)) + OF(v3, z) * (OF(v1, x) - OF(v2, x));
	c = OF(v1, x) * (OF(v2, y) - OF(v3, y)) + OF(v2, x) * (OF(v3, y) - OF(v1, y)) + OF(v3, x) * (OF(v1, y) - OF(v2, y));
	d = -(OF(v1, x) * (OF(v2, y) * OF(v3, z) - OF(v3, y) * OF(v2, z)) + OF(v2, x) * (OF(v3, y) * OF(v1, z) - OF(v1, y) * OF(v3, z)) + OF(v3, x) * (OF(v1, y) * OF(v2, z) - OF(v2, y) * OF(v1, z)));
	return true;
}

END_NS
END_NS
