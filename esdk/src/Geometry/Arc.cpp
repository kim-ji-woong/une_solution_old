#include "StdAfx.h"
#include "GArc.h"
#include "GMath.h"
#include <math.h>

#define IsEqualRad(dData1,dData2) fabs(dData1-dData2) <= Math::COORD_TOLERANCE() ? true : false

BEGIN_NS(UnE)
BEGIN_NS(Geometry)

template <class Arc, class EArc>
static bool Equal(REF_CONST(Arc) rArc1, REF_CONST(Arc) rArc2)
{
	if ((REF_CONST(EArc))rArc1 != (REF_CONST(EArc))rArc2)
		return false;
	if (OF(rArc1, GetRadius()) != OF(rArc2, GetRadius()))
		return false;

	return true;
}

Arc2D::Arc2D(void)
{
	m_dRadius = 0.0;
}

Arc2D::Arc2D(REF_CONST(Vertex2D) v1, REF_CONST(Vertex2D) v2, REF_CONST(Vertex2D) v3)
{
	SetArc(v1, v2, v3);
}

// dBeginAngle, dArcAngle : Radian
Arc2D::Arc2D(REF_CONST(Vertex2D) vCenter, double dRadius, double dBeginAngle, double dArcAngle, bool isClockWise)
{
	SetArc(vCenter, dRadius, dBeginAngle, dArcAngle, isClockWise);
}

Arc2D::~Arc2D(void)
{
}

/*#ifdef DOTNET
bool Arc2D::operator== (Arc2D^ op1, Arc2D^ op2)
{
	bool isNull1 = NullChecker::IsNull(op1);
	bool isNull2 = NullChecker::IsNull(op2);

	if (isNull1 && isNull2)
		return true;
	else if (isNull1 || isNull2)
		return false;

	return Equal<Arc2D, EArc2D>(op1, op2);
}

bool Arc2D::operator!= (Arc2D^ op1, Arc2D^ op2)
{
	bool isNull1 = NullChecker::IsNull(op1);
	bool isNull2 = NullChecker::IsNull(op2);

	if (isNull1 && isNull2)
		return false;
	else if (isNull1 || isNull2)
		return true;

	return !Equal<Arc2D, EArc2D>(op1, op2);
}

#else
bool Arc2D::operator== (const Arc2D& rhs) const
{
	return Equal<Arc2D, EArc2D>(THIS_OBJ, rhs);
}

bool Arc2D::operator!= (const Arc2D& rhs) const
{
	return !Equal<Arc2D, EArc2D>(THIS_OBJ, rhs);
}

#endif*/

// dBeginAngle, dArcAngle : Radian
void Arc2D::SetArc(REF_CONST(Vertex2D) vCenter, double dRadius, double dBeginAngle, double dArcAngle, bool isClockWise)
{
	OF(m_vCenter, CopyFrom(vCenter));
	m_dA = m_dB = m_dRadius = dRadius;
	m_dBeginAngle = dBeginAngle;
	m_dAngle = dArcAngle;
	m_isClockWise = isClockWise;
	m_isClosed = dArcAngle >= Math::_2PI() - Math::HALF_TOLERANCE() ? true : false;

	OF(m_vTL, x) = OF(m_vCenter, x) - dRadius;
	OF(m_vTL, y) = OF(m_vCenter, y) + dRadius;
	OF(m_vBL, x) = OF(m_vCenter, x) - dRadius;
	OF(m_vBL, y) = OF(m_vCenter, y) - dRadius;
	OF(m_vBR, x) = OF(m_vCenter, x) + dRadius;
	OF(m_vBR, y) = OF(m_vCenter, y) - dRadius;
}

// 세 점을 이용하여 원의 중점 및 반지름을 구한다.
// Return 값 : true이면 값을 구하였다.
//             false이면 원을 구성하기에 충분치 않은 데이터이다.
bool Arc2D::GetCircleInfo(REF_CONST(Vertex2D) v1, REF_CONST(Vertex2D) v2, REF_CONST(Vertex2D) v3, CBR(INSTANCE(Vertex2D)) rCenter, CBR(double) rRadius)
{
	// v1과 v2 사이의 거리
	double dL1 = OF(v1, GetDistance(v2));

	if (dL1 < Math::HALF_TOLERANCE() || OF(v2, GetDistance(v3)) < Math::HALF_TOLERANCE() || OF(v1, GetDistance(v3)) < Math::HALF_TOLERANCE())
		return false;

	// v1과 v2의 가운데 위치하는 점
	INSTANCE(Vertex2D) vM = Math::GetLinearVertex(v1, v2, dL1/2);

	// v1과 v2가 이루는 직선과, v1과 v3가 이루는 직선이
	// 만나서 이루는 각
	double dTheta1 = Math::GetAngle(v2, v1, v3);
	// v1과 v3가 이루는 직선과, v3와 v2가 이루는 직선이
	// 만나서 이루는 각
	double dTheta2 = Math::GetAngle(v1, v3, v2);

	// 세 점이 한 직선상에 있다.
	if (IsEqualRad(dTheta1, 0.0)) return false;
	if (IsEqualRad(dTheta1, Math::PI())) return false;
	if (IsEqualRad(dTheta2, 0.0)) return false;
	if (IsEqualRad(dTheta2, Math::PI())) return false;

	// vC : 원의 중점
	// vQ2 : vM에서 vC 방향으로 직선을 연장하여 원과 만나는 점
	// vQ1 : vQ2에서 그은 원의 접선과 v1, v3를 잇는 직선이 만나는 점
	// vQ3 : vM, vQ2를 잇는 직선과 v1, v3를 잇는 직선이 만나는 점
	INSTANCE(Vertex2D) vQ2;
//	INSTANCE(Vertex2D) vC;
	INSTANCE(Vertex2D) vQ1/*, vQ3*/;

	double dLength1 = OF(v1, GetDistance(v3));
	double dLength2 = OF(v2, GetDistance(v3));

	if (dLength1 == dLength2)	// v3가 vQ2인 경우
	{
		rRadius = dLength1 * sin(dTheta2 / 2) / sin(Math::PI() - dTheta2);
		rCenter = Math::GetLinearVertex(v3, vM, rRadius);
	}
	else if (dTheta1 == Math::HALF_PI())
	{
		rRadius = OF(v2, GetDistance(v3)) / 2;
		rCenter = Math::GetLinearVertex(v2, v3, rRadius);
	}
	else if (dTheta2 == Math::HALF_PI())
	{
		rRadius = OF(v1, GetDistance(v2)) / 2;
		rCenter = Math::GetLinearVertex(v1, v2, rRadius);
	}
	else if (dTheta1 < Math::HALF_PI() && dTheta2 > Math::HALF_PI())
	{
		return GetCircleInfo(v2, v3, v1, rCenter, rRadius);
	}
	else if (dTheta1 > Math::HALF_PI() && dTheta2 < Math::HALF_PI())
	{
		return GetCircleInfo(v2, v1, v3, rCenter, rRadius);
	}
	else
	{
		double dL2 = dL1 / 2 * tan(dTheta1);	// vM과 vQ3 사이의 거리
		double dL3 = dL1 / 2 / tan(dTheta2 / 2);// vM과 vQ2 사이의 거리
		double dL4 = dL3 - dL2;					// vQ2와 vQ3 사이의 거리

		// vQ1, vQ2, vQ3로 이루어진 삼각형과
		// v1, vM, vQ3로 이루어진 삼각형은 닮은꼴이다.
		// 따라서, v1, vM 사이의 거리와 vQ1, vQ2 사이의 거리의 비는
		// vM, vQ3 사이의 거리와 vQ2, vQ3 사이의 거리의 비와 같다.
		double dL5 = dL1 / 2 * dL4 / dL2;		// vQ1과 vQ2 사이의 거리
		double dL6 = dL2 / sin(dTheta1);		// v1과 vQ3 사이의 거리
		double dL7 = dL6 * dL4 / dL2;			// vQ1과 vQ3 사이의 거리
		double dL8 = dL6 + dL7;					// v1과 vQ1 사이의 거리

		vQ1 = Math::GetLinearVertex(v1, v3, dL8);

		// vQ1, vQ2를 잇는 직선과 v1, v2를 잇는 직선은 평행하다.
		vQ2 = vQ1 + (v1 - v2) * dL5 / dL1;
		
		rRadius = dL1 / 2 / sin(dTheta2);
		rCenter = Math::GetLinearVertex(vQ2, vM, rRadius);
	}

	return true;
}

bool Arc2D::SetArc(REF_CONST(Vertex2D) v1, REF_CONST(Vertex2D) v2, REF_CONST(Vertex2D) v3)
{
	if (!GetCircleInfo(v1, v2, v3, m_vCenter, m_dRadius))
		return false;

	m_dA = m_dB = m_dRadius;
	m_isClosed = false;
	m_isClockWise = Math::IsRightSideFromLine(v2, v1, v3) ? true : false;
	
	INSTANCE(Vertex2D) vOpp = m_vCenter * 2 - v1;

	if (Math::IsRightSideFromLine(v3, v1, vOpp))
	{
		if (m_isClockWise)
		{
			// Arc가 이루는 각도가 PI보다 같거나 작다
			m_dAngle = Math::GetAngle(v1, m_vCenter, v3);
		}
		else
		{
			// Arc가 이루는 각도가 PI보다 크다
			m_dAngle = Math::_2PI() - Math::GetAngle(v1, m_vCenter, v3);
		}
	}
	else
	{
		if (m_isClockWise)
		{
			// Arc가 이루는 각도가 PI보다 크다
			m_dAngle = Math::_2PI() - Math::GetAngle(v1, m_vCenter, v3);
		}
		else
		{
			// Arc가 이루는 각도가 PI보다 같거나 작다
			m_dAngle = Math::GetAngle(v1, m_vCenter, v3);
		}
	}

	INSTANCE(Vertex2D) vR = dnonlynew Vertex2D(OF(m_vCenter, x) + m_dRadius, OF(m_vCenter, y));
	if (OF(v1 ,y) >= OF(vR, y)) m_dBeginAngle = Math::GetAngle(v1, m_vCenter, vR);
	else m_dBeginAngle = Math::_2PI() - Math::GetAngle(v1, m_vCenter, vR);

	OF(m_vTL, x) = OF(m_vCenter, x) - m_dRadius;
	OF(m_vTL, y) = OF(m_vCenter, y) + m_dRadius;
	OF(m_vBL, x) = OF(m_vCenter, x) - m_dRadius;
	OF(m_vBL, y) = OF(m_vCenter, y) - m_dRadius;
	OF(m_vBR, x) = OF(m_vCenter, x) + m_dRadius;
	OF(m_vBR, y) = OF(m_vCenter, y) - m_dRadius;

	return true;
}

// dAngle : Radian
// dAngle이 범위를 벗어나면 false를 리턴한다.
bool Arc2D::GetVertex(double dAngle, OUT CBR(INSTANCE(Vertex2D)) rVertex) CONST
{
#ifdef DOTNET
	rVertex = gcnew Vertex2D();
#endif

	if (!CheckValidAngle(dAngle))
		return false;

	OF(rVertex, x) = OF(m_vCenter, x) + m_dRadius * cos(dAngle);
	OF(rVertex, y) = OF(m_vCenter, y) + m_dRadius * sin(dAngle);
	return true;
}

INSTANCE(Vertex2D) Arc2D::GetBeginVertex() CONST
{
	INSTANCE(Vertex2D) vertex = dnonlynew Vertex2D();

	OF(vertex, x) = OF(m_vCenter, x) + m_dRadius * cos(m_dBeginAngle);
	OF(vertex, y) = OF(m_vCenter, y) + m_dRadius * sin(m_dBeginAngle);
	return vertex;
}

INSTANCE(Vertex2D) Arc2D::GetEndVertex() CONST
{
	INSTANCE(Vertex2D) vertex = dnonlynew Vertex2D();
	double dAngle = m_isClockWise ? m_dBeginAngle - m_dAngle : m_dBeginAngle + m_dAngle;

	OF(vertex, x) = OF(m_vCenter, x) + m_dRadius * cos(dAngle);
	OF(vertex, y) = OF(m_vCenter, y) + m_dRadius * sin(dAngle);
	return vertex;
}

void Arc2D::NewObject(CBR(POINTER(EArc2D)) pEArc) CONST
{
	pEArc = geonew Arc2D();
}

// vBegin에서 vEnd방향으로(반시계 방향) 향하는 rEArc를 만든다.
void Arc2D::MakeSub(POINTER(EArc2D) pEArc, REF_CONST(Vertex2D) vBegin, REF_CONST(Vertex2D) vEnd) CONST
{
	EArc2D::MakeSub(pEArc, vBegin, vEnd);
	POINTER(Arc2D) pArc = (POINTER(Arc2D))pEArc;
	pArc->m_dRadius = m_dRadius;
}

// rArc를 원의 바깥으로 dLen 만큼 확대(outside가 false이면 축소)시킨다.
INSTANCE(Arc2D) Arc2D::Offset(bool outside, double dLen) CONST
{
	INSTANCE(Arc2D) arc = dnonlynew Arc2D();
	_Offset(arc, outside, dLen);
	OF(arc, m_dRadius) = OF(arc, m_dA);
	return arc;
}

// v1과 v2를 지나는 직선을 기준으로 현재의 Arc 객체와 대칭되는 객체를 만들어 리턴한다.
// v1과 v2가 동일한 좌표일 경우 false를 리턴한다.
bool Arc2D::Mirror(REF_CONST(Vertex2D) v1, REF_CONST(Vertex2D) v2, OUT CBR(INSTANCE(Arc2D)) rResult)
{
#ifdef DOTNET
	rResult = nullptr;
#endif

	if (OF(v1, GetDistance(v2)) <= Math::HALF_TOLERANCE())
		return false;

	INSTANCE(Vertex2D) vBegin = GetBeginVertex();

	rResult = dnonlynew Arc2D();
	OF(rResult, m_dRadius) = m_dRadius;

	//return _Mirror(v1, v2, rResult);
	if (!_Mirror(v1, v2, rResult))
		return false;

	INSTANCE(Vertex2D) _vBegin;
	if (!OF(vBegin, Mirror(v1, v2, _vBegin)))
		return false;

	REF_CONST(Vertex2D) vCenter = OF(rResult, m_vCenter);
	INSTANCE(Vertex2D) vR = dnonlynew Vertex2D(OF(vCenter, x) + m_dRadius, OF(vCenter, y));

	double dBeginAngle = Math::GetAngle(vR, vCenter, _vBegin);
	if (OF(_vBegin, y) < OF(vCenter, y))
		dBeginAngle = Math::_2PI() - dBeginAngle;

	OF(rResult, m_dBeginAngle) = dBeginAngle;

	OF(rResult, OF(m_vTL, x)) = OF(vCenter, x) - m_dRadius;
	OF(rResult, OF(m_vTL, y)) = OF(vCenter, y) + m_dRadius;
	OF(rResult, OF(m_vBL, x)) = OF(vCenter, x) - m_dRadius;
	OF(rResult, OF(m_vBL, y)) = OF(vCenter, y) - m_dRadius;
	OF(rResult, OF(m_vBR, x)) = OF(vCenter, x) + m_dRadius;
	OF(rResult, OF(m_vBR, y)) = OF(vCenter, y) - m_dRadius;

	return true;
}

void Arc2D::SetIntersectResult(double dBeginAngle, double dEArcAngle, bool clockwise)
{
	SetArc(GetCenter(), GetRadius(), dBeginAngle, dEArcAngle, clockwise);
}

Arc3D::Arc3D(void)
{
	m_dRadius = 0.0;
}

Arc3D::~Arc3D(void)
{
}

/*#ifdef DOTNET
bool Arc3D::operator== (Arc3D^ op1, Arc3D^ op2)
{
	bool isNull1 = NullChecker::IsNull(op1);
	bool isNull2 = NullChecker::IsNull(op2);

	if (isNull1 && isNull2)
		return true;
	else if (isNull1 || isNull2)
		return false;

	return Equal<Arc3D, EArc3D>(op1, op2);
}

bool Arc3D::operator!= (Arc3D^ op1, Arc3D^ op2)
{
	bool isNull1 = NullChecker::IsNull(op1);
	bool isNull2 = NullChecker::IsNull(op2);

	if (isNull1 && isNull2)
		return false;
	else if (isNull1 || isNull2)
		return true;

	return !Equal<Arc3D, EArc3D>(op1, op2);
}

#else
bool Arc3D::operator== (const Arc3D& rhs) const
{
	return Equal<Arc3D, EArc3D>(THIS_OBJ, rhs);
}

bool Arc3D::operator!= (const Arc3D& rhs) const
{
	return !Equal<Arc3D, EArc3D>(THIS_OBJ, rhs);
}

#endif*/

Arc3D::Arc3D(REF_CONST(Vertex3D) v1, REF_CONST(Vertex3D) v2, REF_CONST(Vertex3D) v3)
{
	SetArc(v1, v2, v3);
}

Arc3D::Arc3D(REF_CONST(Vertex3D) vTL, REF_CONST(Vertex3D) vBL, REF_CONST(Vertex3D) vBR, double dRadius, double dBeginAngle, double dArcAngle, bool isClockWise)
{
	SetArc(vTL, vBL, vBR, dRadius, dBeginAngle, dArcAngle, isClockWise);
}

bool Arc3D::SetArc(REF_CONST(Vertex3D) v1, REF_CONST(Vertex3D) v2, REF_CONST(Vertex3D) v3)
{
	if (!GetCircleInfo(v1, v2, v3, m_vCenter, m_dRadius))
		return false;

	m_dA = m_dB = m_dRadius;
	m_isClockWise = false;
	m_isClosed = false;

	INSTANCE(Vertex3D) vR = GetRightAngleVertex(v1, v2, v3, m_vCenter, m_dRadius);
	INSTANCE(Vertex3D) vTR = vR + (v1 - m_vCenter);

	m_vBR = vR * 2 - vTR;
	m_vTL = m_vCenter * 2 - m_vBR;
	m_vBL = m_vTL + m_vBR - vTR;
	/*m_vTL = vR + (m_vCenter - v1);
	m_vBL = m_vTL + (m_vCenter - vR) * 2;
	m_vBR = v1 + (m_vCenter - vR);*/

	m_dBeginAngle = Math::HALF_PI();
	m_dAngle = Math::GetAngle(v3, m_vCenter, v1);

	if (OF(v3, GetDistance(m_vBL)) < OF(v3, GetDistance(m_vBR)))
		m_dAngle = Math::_2PI() - m_dAngle;
	//m_dEndAngle = Math::GetAngle(vR, m_vCenter, v3);
	//
	//Vertex3D vTemp = Math::GetNearestVertex(v3, m_vTL, m_vBL, true);
	//if (Math::GetDistance(vTemp, m_vBL) < m_dRadius) m_dEndAngle = Math::_2PI() - m_dEndAngle;

	//if (m_dEndAngle <= Math::HALF_PI())
	//	m_dArcAngle = Math::HALF_PI() - m_dEndAngle;
	//else if (m_dEndAngle <= Math::PI())
	//	m_dArcAngle = Math::_2PI() - (m_dEndAngle - Math::HALF_PI());
	//else //if (m_dEndAngle <= Math::_2PI())
	//	m_dArcAngle = Math::_2PI() - m_dEndAngle + Math::HALF_PI();
	
	return true;
}

bool Arc3D::SetArc(REF_CONST(Vertex3D) vTL, REF_CONST(Vertex3D) vBL, REF_CONST(Vertex3D) vBR, double dRadius, double dBeginAngle, double dArcAngle, bool isClockWise)
{
	m_isClockWise = isClockWise;

	OF(m_vTL, CopyFrom(vTL));
	OF(m_vBL, CopyFrom(vBL));
	OF(m_vBR, CopyFrom(vBR));

	m_vCenter = (m_vTL + m_vBR) / 2;

	m_isClosed = dArcAngle >= Math::_2PI() - Math::HALF_TOLERANCE() ? true : false;

	m_dAngle = dArcAngle;
	m_dBeginAngle = dBeginAngle;
	m_dA = m_dB = m_dRadius = dRadius;

	return true;
}

// 원 위의 세 점 v1, v2, v3가 있고 원의 중점 vCenter가 있다.
// v1에서 v2와 v3를 차례대로 지나가는 방향으로 90도 회전한 곳의 좌표를 구한다.
INSTANCE(Vertex3D) Arc3D::GetRightAngleVertex(REF_CONST(Vertex3D) v1, REF_CONST(Vertex3D) v2, REF_CONST(Vertex3D) v3, REF_CONST(Vertex3D) vCenter, double dRadius)
{
	double dTheta1 = Math::GetAngle(v1, vCenter, v2);
	double dTheta2 = Math::GetAngle(v2, vCenter, v3);
	double dTheta3 = Math::GetAngle(v3, vCenter, v1);

	INSTANCE(Vertex3D) v;

	// v2가 v1의 반대편에 있는 경우
	if (fabs(dTheta1 - Math::PI()) <= Math::HALF_TOLERANCE())
	{
		if (dTheta3 == Math::HALF_PI()) return Math::GetLinearVertex(v3, vCenter, dRadius*2);
		else 
		{
			v = GetRightAngleVertex(v1, v3, v2, vCenter, dRadius);
			return Math::GetLinearVertex(v, vCenter, dRadius*2);
		}
	}

	if (dTheta1 == Math::HALF_PI()) return v2;
	else if (dTheta1 < Math::HALF_PI())
	{
		double dL1 = dRadius / cos(Math::HALF_PI() - dTheta1);
		double dL2 = dRadius * tan(Math::HALF_PI() - dTheta1);
		INSTANCE(Vertex3D) vQ = Math::GetLinearVertex(vCenter, v2, dL1);

		v = vQ + (vCenter - v1) * dL2 / dRadius;
	}
	else
	{
		double dL1 = dRadius / cos(dTheta1 - Math::HALF_PI());
		double dL2 = dRadius * tan(dTheta1 - Math::HALF_PI());
		INSTANCE(Vertex3D) vQ = Math::GetLinearVertex(vCenter, v2, dL1);

		v = vQ + (v1 - vCenter) * dL2 / dRadius;
	}

	// v1, C, v2가 이루는 각 중 π보다 작은 쪽에 v3가 존재하는 경우
	if (fabs(dTheta1 - dTheta2 - dTheta3) <= Math::HALF_TOLERANCE())
	{
		return Math::GetLinearVertex(v, vCenter, dRadius*2);
	}

	return v;
}

// 세 점을 이용하여 원의 중점 및 반지름을 구한다.
// Return 값 : true이면 값을 구하였다.
//             false이면 원을 구성하기에 충분치 않은 데이터이다.
bool Arc3D::GetCircleInfo(REF_CONST(Vertex3D) v1, REF_CONST(Vertex3D) v2, REF_CONST(Vertex3D) v3, CBR(INSTANCE(Vertex3D)) rCenter, CBR(double) rRadius)
{
	// v1과 v2 사이의 거리
	double dL1 = OF(v1, GetDistance(v2));

	if (dL1 < Math::HALF_TOLERANCE() || OF(v2, GetDistance(v3)) < Math::HALF_TOLERANCE() || OF(v1, GetDistance(v3)) < Math::HALF_TOLERANCE())
		return false;

	// v1과 v2의 가운데 위치하는 점
	INSTANCE(Vertex3D) vM = (v1 + v2) / 2;

	// v1과 v2가 이루는 직선과, v1과 v3가 이루는 직선이
	// 만나서 이루는 각
	double dTheta1 = Math::GetAngle(v2, v1, v3);
	// v1과 v3가 이루는 직선과, v3와 v2가 이루는 직선이
	// 만나서 이루는 각
	double dTheta2 = Math::GetAngle(v1, v3, v2);

	// 세 점이 한 직선상에 있다.
	if (IsEqualRad(dTheta1,0.0)) return false;
	if (IsEqualRad(dTheta1,Math::PI())) return false;
	if (IsEqualRad(dTheta2,0.0)) return false;
	if (IsEqualRad(dTheta2,Math::PI())) return false;

	// vC : 원의 중점
	// vQ2 : vM에서 vC 방향으로 직선을 연장하여 원과 만나는 점
	// vQ1 : vQ2에서 그은 원의 접선과 v1, v3를 잇는 직선이 만나는 점
	// vQ3 : vM, vQ2를 잇는 직선과 v1, v3를 잇는 직선이 만나는 점
	INSTANCE(Vertex3D) vQ2;
//	INSTANCE(Vertex3D) vC;
	INSTANCE(Vertex3D) vQ1/*, vQ3*/;

	double dLength1 = OF(v1, GetDistance(v3));
	double dLength2 = OF(v2, GetDistance(v3));

	if (dLength1 == dLength2)	// v3가 vQ2인 경우
	{
		rRadius = dLength1 * sin(dTheta2 / 2) / sin(Math::PI() - dTheta2);
		rCenter = Math::GetLinearVertex(v3, vM, rRadius);
	}
	else if (dTheta1 == Math::HALF_PI())
	{
		rRadius = OF(v2, GetDistance(v3)) / 2;
		rCenter = Math::GetLinearVertex(v2, v3, rRadius);
	}
	else if (dTheta2 == Math::HALF_PI())
	{
		rRadius = OF(v1, GetDistance(v2)) / 2;
		rCenter = Math::GetLinearVertex(v1, v2, rRadius);
	}
	else if (dTheta1 < Math::HALF_PI() && dTheta2 > Math::HALF_PI())
	{
		return GetCircleInfo(v2, v3, v1, rCenter, rRadius);
	}
	else if (dTheta1 > Math::HALF_PI() && dTheta2 < Math::HALF_PI())
	{
		return GetCircleInfo(v2, v1, v3, rCenter, rRadius);
	}
	else
	{
		double dL2 = dL1 / 2 * tan(dTheta1);	// vM과 vQ3 사이의 거리
		double dL3 = dL1 / 2 / tan(dTheta2 / 2);// vM과 vQ2 사이의 거리
		double dL4 = dL3 - dL2;					// vQ2와 vQ3 사이의 거리

		// vQ1, vQ2, vQ3로 이루어진 삼각형과
		// v1, vM, vQ3로 이루어진 삼각형은 닮은꼴이다.
		// 따라서, v1, vM 사이의 거리와 vQ1, vQ2 사이의 거리의 비는
		// vM, vQ3 사이의 거리와 vQ2, vQ3 사이의 거리의 비와 같다.
		double dL5 = dL1 / 2 * dL4 / dL2;		// vQ1과 vQ2 사이의 거리
		double dL6 = dL2 / sin(dTheta1);		// v1과 vQ3 사이의 거리
		double dL7 = dL6 * dL4 / dL2;			// vQ1과 vQ3 사이의 거리
		double dL8 = dL6 + dL7;					// v1과 vQ1 사이의 거리

		vQ1 = Math::GetLinearVertex(v1, v3, dL8);

		// vQ1, vQ2를 잇는 직선과 v1, v2를 잇는 직선은 평행하다.
		vQ2 = vQ1 + (v1 - v2) * dL5 / dL1;
		
		rRadius = dL1 / 2 / sin(dTheta2);
		rCenter = Math::GetLinearVertex(vQ2, vM, rRadius);
	}

	return true;
}

// dAngle : Radian
// dAngle이 범위를 벗어나면 false를 리턴한다.
bool Arc3D::GetVertex(double dAngle, OUT CBR(INSTANCE(Vertex3D)) rVertex) CONST
{
	if (!CheckValidAngle(dAngle))
		return false;

	INSTANCE(Vertex3D) vR = m_vCenter + (m_vBR - m_vBL) / 2;
	rVertex = Math::GetLinearVertex(m_vCenter, vR, m_dRadius * cos(dAngle));
	rVertex = rVertex + (m_vBL - m_vTL) * (m_dRadius * sin(dAngle) / OF(m_vBL, GetDistance(m_vTL)));

	return true;
}

INSTANCE(Vertex3D) Arc3D::GetBeginVertex() CONST
{
	INSTANCE(Vertex3D) vR = m_vCenter + (m_vBR - m_vBL) / 2;
	INSTANCE(Vertex3D) vBegin = Math::GetLinearVertex(m_vCenter, vR, m_dRadius * cos(m_dBeginAngle));
	vBegin = vBegin + (m_vTL - m_vBL) * (m_dRadius * sin(m_dBeginAngle) / OF(m_vBL, GetDistance(m_vTL)));
	return vBegin;
}

INSTANCE(Vertex3D) Arc3D::GetEndVertex() CONST
{
	INSTANCE(Vertex3D) vR = m_vCenter + (m_vBR - m_vBL) / 2;
	double dAngle = m_isClockWise ? m_dBeginAngle - m_dAngle : m_dBeginAngle + m_dAngle;

	INSTANCE(Vertex3D) vEnd = Math::GetLinearVertex(m_vCenter, vR, m_dRadius * cos(dAngle));
	vEnd = vEnd + (m_vTL - m_vBL) * (m_dRadius * sin(dAngle) / OF(m_vBL, GetDistance(m_vTL)));
	return vEnd;
}

void Arc3D::NewObject(CBR(POINTER(EArc3D)) pEArc) CONST
{
	pEArc = geonew Arc3D();
}

// rArc를 원의 바깥으로 dLen 만큼 확대(outside가 false이면 축소)시킨다.
INSTANCE(Arc3D) Arc3D::Offset(bool outside, double dLen) CONST
{
	INSTANCE(Arc3D) arc = dnonlynew Arc3D();
	_Offset(arc, outside, dLen);
	OF(arc, m_dRadius) = OF(arc, m_dA);
	return arc;
}

// v1, v2, v3를 지나는 평면을 기준으로 현재의 Arc와 대칭되는 객체를 만들어 리턴한다.
// v1, v2, v3가 평면을 구성하지 못할 경우 false를 리턴한다.
bool Arc3D::Mirror(REF_CONST(Vertex3D) v1, REF_CONST(Vertex3D) v2, REF_CONST(Vertex3D) v3, OUT CBR(INSTANCE(Arc3D)) rResult)
{
#ifdef DOTNET
	rResult = nullptr;
#endif

	double a, b, c, d;	// ax + by + cz + d = 0
	if (!Math::MakePlane(v1, v2, v3, a, b, c, d))
		return false;

	INSTANCE(Vertex3D) vBegin = GetBeginVertex();

	rResult = dnonlynew Arc3D();
	OF(rResult, m_dRadius) = m_dRadius;

	return _Mirror(a, b, c, d, rResult);
}

END_NS
END_NS
