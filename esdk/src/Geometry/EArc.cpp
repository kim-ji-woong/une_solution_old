#include "StdAfx.h"
#include <GCE2d_MakeArcOfEllipse.hxx>
#include <gp_Pnt2d.hxx>
#include <gp_Elips2d.hxx>
#include <gp_Ax2.hxx>
#include <Geom2dAPI_InterCurveCurve.hxx>
#include <GCE2d_MakeLine.hxx>
#include <Geom2d_Line.hxx>
#include "GMath.h"
#include "GVertex.h"
#include "GEArc.h"
#include "GLine.h"
#include "GVector.h"
#include <Geom2d_TrimmedCurve.hxx>
#include <IntRes2d_IntersectionSegment.hxx>
#include <Geom2dAdaptor_Curve.hxx>
#include <CPnts_AbscissaPoint.hxx>
#include <GCE2d_MakeEllipse.hxx>

BEGIN_NS(UnE)
BEGIN_NS(Geometry)

#ifndef COLLECTION_ADD
#ifdef DOTNET
#define COLLECTION_ADD(collection, data) collection->Add(data)
#else
#define COLLECTION_ADD(collection, data) collection.push_back(data)
#endif
#endif

template <class EArc, class Vertex, class Real>
static bool _GetVertex(REF_CONST(EArc) rEArc, double dAngle, CBR(INSTANCE(Vertex)) rVertex, bool angleCheck = true)
{
#ifdef DOTNET
	rVertex = gcnew Vertex();
#endif

	double halfpi = Math::HALF_PI();
	double pi = Math::PI();
	double _3halfpi = Math::_3HALF_PI();
	double _2pi = Math::_2PI();

	if (dAngle < 0.0f)
	{
		int nCount = (int)(-dAngle / _2pi);
		dAngle += _2pi * (nCount + 1);
	}
	else if (dAngle > _2pi)
	{
		int nCount = (int)(dAngle / _2pi);
		dAngle -= _2pi * nCount;
	}

	if (angleCheck)
	{
		if (!OF(rEArc, CheckValidAngle(dAngle)))
			return false;
	}
	/*if (!OF(rEArc, IsClosed()))
	{
		Real dEndAngle = OF(rEArc, GetEndAngle());
		Real dBeginAngle = OF(rEArc, GetBeginAngle());

		if (OF(rEArc, IsClockWise()))
		{
			if (dBeginAngle > dEndAngle)
			{
				if (dAngle < dEndAngle - Math::HALF_TOLERANCE() || dAngle > dBeginAngle + Math::HALF_TOLERANCE())
					return false;
			}
			else
			{
				if (dAngle > dBeginAngle + Math::HALF_TOLERANCE() && dAngle < dEndAngle - Math::HALF_TOLERANCE())
					return false;
			}
		}
		else
		{
			if (dBeginAngle > dEndAngle)
			{
				if (dAngle > dEndAngle + Math::HALF_TOLERANCE() && dAngle < dBeginAngle - Math::HALF_TOLERANCE())
					return false;
			}
			else
			{
				if (dAngle < dBeginAngle - Math::HALF_TOLERANCE() || dAngle > dEndAngle + Math::HALF_TOLERANCE())
					return false;
			}
		}
	}*/

	Real a = OF(rEArc, GetA());
	Real b = OF(rEArc, GetB());
	if (a < Math::HALF_TOLERANCE() || b < Math::HALF_TOLERANCE())
		return false;

	REF_CONST(Vertex) vTL = OF(rEArc, GetTL());
	REF_CONST(Vertex) vBL = OF(rEArc, GetBL());
	REF_CONST(Vertex) vBR = OF(rEArc, GetBR());

	INSTANCE(Vertex) vL = (vTL + vBL) / 2;
	INSTANCE(Vertex) vR = vL + vBR - vBL;
	INSTANCE(Vertex) vB = (vBL + vBR) / 2;
	INSTANCE(Vertex) vT = vB + vTL - vBL;

	if (dAngle <= Math::HALF_TOLERANCE() || dAngle >= (_2pi - Math::HALF_TOLERANCE()))
	{
		OF(rVertex, CopyFrom(vR));
	}
	else if (dAngle >= (halfpi - Math::HALF_TOLERANCE()) &&
		dAngle <= (halfpi + Math::HALF_TOLERANCE()))
	{
		OF(rVertex, CopyFrom(vT));
	}
	else if (dAngle >= (pi - Math::HALF_TOLERANCE()) &&
		dAngle <= (pi + Math::HALF_TOLERANCE()))
	{
		OF(rVertex, CopyFrom(vL));
	}
	else if (dAngle >= (_3halfpi - Math::HALF_TOLERANCE()) &&
		dAngle <= (_3halfpi + Math::HALF_TOLERANCE()))
	{
		OF(rVertex, CopyFrom(vB));
	}
	else
	{
		double dLengthX, dLengthY;

		if (dAngle < halfpi)
		{
			double dTanData = tan(dAngle);

			dLengthX = sqrt(1.0 / (1.0 / a / a + dTanData * dTanData / b / b));
			dLengthY = sqrt(1.0 / (1.0 / a / a / dTanData / dTanData + 1.0 / b / b));
		}
		else if (dAngle < pi)
		{
			double dTanData = tan(pi - dAngle);

			dLengthX = -sqrt(1.0 / (1.0 / a / a + dTanData * dTanData / b / b));
			dLengthY = sqrt(1.0 / (1.0 / a / a / dTanData / dTanData + 1.0 / b / b));
		}
		else if (dAngle < _3halfpi)
		{
			double dTanData = tan(dAngle - pi);

			dLengthX = -sqrt(1.0 / (1.0 / a / a + dTanData * dTanData / b / b));
			dLengthY = -sqrt(1.0 / (1.0 / a / a / dTanData / dTanData + 1.0 / b / b));
		}
		else
		{
			double dTanData = tan(_2pi - dAngle);

			dLengthX = sqrt(1.0 / (1.0 / a / a + dTanData * dTanData / b / b));
			dLengthY = -sqrt(1.0 / (1.0 / a / a / dTanData / dTanData + 1.0 / b / b));
		}

		REF_CONST(Vertex) vCenter = OF(rEArc, GetCenter());
		INSTANCE(Vertex) vResult = vCenter + (vR - vCenter) * Real(dLengthX) / a;
		vResult = vResult + (vT - vCenter) * Real(dLengthY) / b;

		OF(rVertex, CopyFrom(vResult));
	}

	return true;
}

static bool MakeEArcHandle(REF_CONST(EArc2D) rEArc, Handle(Geom2d_TrimmedCurve)& rHandle)
{
	REF_CONST(Vertex2D) vTL = OF(rEArc, GetTL());
	REF_CONST(Vertex2D) vBL = OF(rEArc, GetBL());
	REF_CONST(Vertex2D) vBR = OF(rEArc, GetBR());
	REF_CONST(Vertex2D) vCenter = OF(rEArc, GetCenter());

	INSTANCE(Vertex2D) vTR = vTL + vBR - vBL;
	//INSTANCE(Vertex2D) vR  = (vTR + vBR) / 2;

	INSTANCE(Vertex2D) vDir;// = vR - vCenter;
	/*if (!Vector::SetUnitVector(vDir))
		return false;*/

	/*double dBeginAngle, dEndAngle;

	if (OF(rEArc, IsClockWise()))
	{
		dBeginAngle = OF(rEArc, GetEndAngle());
		dEndAngle = OF(rEArc, GetBeginAngle());
	}
	else
	{
		dBeginAngle = OF(rEArc, GetBeginAngle());
		dEndAngle = OF(rEArc, GetEndAngle());
	}*/

	double a = OF(rEArc, GetA());
	double b = OF(rEArc, GetB());
	
	if (a < b)
	{
		double temp = a;
		a = b;
		b = temp;

		//dBeginAngle -= Math::HALF_PI();
		//dEndAngle -= Math::HALF_PI();

		//if (dBeginAngle < 0.0) dBeginAngle += Math::_2PI();
		//if (dEndAngle < 0.0) dEndAngle += Math::_2PI();

		INSTANCE(Vertex2D) vT  = (vTL + vTR) / 2;
		vDir = vT - vCenter;
	}
	else
	{
		INSTANCE(Vertex2D) vR  = (vTR + vBR) / 2;
		vDir = vR - vCenter;
	}

	if (!Vector::SetUnitVector(vDir))
		return false;
	
	gp_Ax2d axis(GP_POINT2D(vCenter), GP_DIR2D(vDir));
	gp_Elips2d EE(axis, a, b);
	//rHandle = GCE2d_MakeArcOfEllipse(EE, dBeginAngle, dEndAngle);

	INSTANCE(Vertex2D) vBegin = OF(rEArc, GetBeginVertex());
	INSTANCE(Vertex2D) vEnd = OF(rEArc, GetEndVertex());
	//double dAngle = OF(rEArc, GetAngle());
	gp_Pnt2d ptBegin = GP_POINT2D(vBegin);
	gp_Pnt2d ptEnd   = GP_POINT2D(vEnd);

	if (OF(rEArc, IsClockWise()))
	{
		double dBeginAngle = OF(rEArc, GetBeginAngle());
		rHandle = GCE2d_MakeArcOfEllipse(EE, ptEnd, dBeginAngle, true);
		//rHandle = GCE2d_MakeArcOfEllipse(EE, ptEnd, dAngle, true);
	}
	else
	{
		double dEndAngle = OF(rEArc, GetEndAngle());
		rHandle = GCE2d_MakeArcOfEllipse(EE, ptBegin, dEndAngle, true);
		//rHandle = GCE2d_MakeArcOfEllipse(EE, ptBegin, dAngle, true);
	}

	return true;
}

static bool MakeEArcHandle(REF_CONST(EArc2F) rEArc, Handle(Geom2d_TrimmedCurve)& rHandle)
{
	REF_CONST(Vertex2F) vTL = OF(rEArc, GetTL());
	REF_CONST(Vertex2F) vBL = OF(rEArc, GetBL());
	REF_CONST(Vertex2F) vBR = OF(rEArc, GetBR());
	REF_CONST(Vertex2F) _vCenter = OF(rEArc, GetCenter());
	INSTANCE(Vertex2D) vCenter = dnonlynew Vertex2D(OF(_vCenter, x), OF(_vCenter, y));

	INSTANCE(Vertex2F) vTR = vTL + vBR - vBL;
	
	INSTANCE(Vertex2F) _vDir;// = vR - vCenter;
	
	float a = OF(rEArc, GetA());
	float b = OF(rEArc, GetB());

	if (a < b)
	{
		float temp = a;
		a = b;
		b = temp;

		//dBeginAngle -= Math::HALF_PI();
		//dEndAngle -= Math::HALF_PI();

		//if (dBeginAngle < 0.0) dBeginAngle += Math::_2PI();
		//if (dEndAngle < 0.0) dEndAngle += Math::_2PI();

		INSTANCE(Vertex2F) vT = (vTL + vTR) / 2;
		_vDir = vT - _vCenter;
	}
	else
	{
		INSTANCE(Vertex2F) vR = (vTR + vBR) / 2;
		_vDir = vR - _vCenter;
	}

	INSTANCE(Vertex2D) vDir = dnonlynew Vertex2D(OF(_vDir, x), OF(_vDir, y));

	if (!Vector::SetUnitVector(vDir))
		return false;

	gp_Ax2d axis(GP_POINT2D(vCenter), GP_DIR2D(vDir));
	gp_Elips2d EE(axis, a, b);
	//rHandle = GCE2d_MakeArcOfEllipse(EE, dBeginAngle, dEndAngle);

	INSTANCE(Vertex2F) vBegin = OF(rEArc, GetBeginVertex());
	INSTANCE(Vertex2F) vEnd = OF(rEArc, GetEndVertex());
	//double dAngle = OF(rEArc, GetAngle());
	gp_Pnt2d ptBegin(OF(vBegin, x), OF(vBegin, y));
	gp_Pnt2d ptEnd(OF(vEnd, x), OF(vEnd, y));

	if (OF(rEArc, IsClockWise()))
	{
		double dBeginAngle = OF(rEArc, GetBeginAngle());
		rHandle = GCE2d_MakeArcOfEllipse(EE, ptEnd, dBeginAngle, true);
		//rHandle = GCE2d_MakeArcOfEllipse(EE, ptEnd, dAngle, true);
	}
	else
	{
		double dEndAngle = OF(rEArc, GetEndAngle());
		rHandle = GCE2d_MakeArcOfEllipse(EE, ptBegin, dEndAngle, true);
		//rHandle = GCE2d_MakeArcOfEllipse(EE, ptBegin, dAngle, true);
	}

	return true;
}

static bool MakeEllipseHandle(REF_CONST(EArc2D) rEArc, Handle_Geom2d_Ellipse& rHandle)
{
	REF_CONST(Vertex2D) vTL = OF(rEArc, GetTL());
	REF_CONST(Vertex2D) vBL = OF(rEArc, GetBL());
	REF_CONST(Vertex2D) vBR = OF(rEArc, GetBR());
	REF_CONST(Vertex2D) vCenter = OF(rEArc, GetCenter());

	INSTANCE(Vertex2D) vTR = vTL + vBR - vBL;
	INSTANCE(Vertex2D) vDir;

	double a = OF(rEArc, GetA());
	double b = OF(rEArc, GetB());

	if (a < b)
	{
		double temp = a;
		a = b;
		b = temp;

		INSTANCE(Vertex2D) vT = (vTL + vTR) / 2;
		vDir = vT - vCenter;
	}
	else
	{
		INSTANCE(Vertex2D) vR = (vTR + vBR) / 2;
		vDir = vR - vCenter;
	}

	if (!Vector::SetUnitVector(vDir))
		return false;

	gp_Ax2d axis(GP_POINT2D(vCenter), GP_DIR2D(vDir));
	gp_Elips2d EE(axis, a, b);

	rHandle = GCE2d_MakeEllipse(EE);
	return true;
}

static bool MakeEllipseHandle(REF_CONST(EArc2F) rEArc, Handle_Geom2d_Ellipse& rHandle)
{
	REF_CONST(Vertex2F) vTL = OF(rEArc, GetTL());
	REF_CONST(Vertex2F) vBL = OF(rEArc, GetBL());
	REF_CONST(Vertex2F) vBR = OF(rEArc, GetBR());
	REF_CONST(Vertex2F) _vCenter = OF(rEArc, GetCenter());
	INSTANCE(Vertex2D) vCenter = dnonlynew Vertex2D(OF(_vCenter, x), OF(_vCenter, y));

	INSTANCE(Vertex2F) vTR = vTL + vBR - vBL;
	INSTANCE(Vertex2F) _vDir;

	double a = OF(rEArc, GetA());
	double b = OF(rEArc, GetB());

	if (a < b)
	{
		double temp = a;
		a = b;
		b = temp;

		INSTANCE(Vertex2F) vT = (vTL + vTR) / 2;
		_vDir = vT - _vCenter;
	}
	else
	{
		INSTANCE(Vertex2F) vR = (vTR + vBR) / 2;
		_vDir = vR - _vCenter;
	}

	INSTANCE(Vertex2D) vDir = dnonlynew Vertex2D(OF(_vDir, x), OF(_vDir, y));

	if (!Vector::SetUnitVector(vDir))
		return false;

	gp_Ax2d axis(GP_POINT2D(vCenter), GP_DIR2D(vDir));
	gp_Elips2d EE(axis, a, b);

	rHandle = GCE2d_MakeEllipse(EE);
	return true;
}

template <class EArc>
static bool Equal(REF_CONST(EArc) earc1, REF_CONST(EArc) earc2)
{
	if (OF(earc1, GetBeginAngle()) != OF(earc2, GetBeginAngle()))
		return false;
	if (OF(earc1, GetAngle()) != OF(earc2, GetAngle()))
		return false;
	if (OF(earc1, IsClockWise()) != OF(earc2, IsClockWise()))
		return false;
	if (OF(earc1, IsClosed()) != OF(earc2, IsClosed()))
		return false;
	if (OF(earc1, GetTL()) != OF(earc2, GetTL()))
		return false;
	if (OF(earc1, GetBL()) != OF(earc2, GetTL()))
		return false;
	if (OF(earc1, GetBR()) != OF(earc2, GetBR()))
		return false;
	if (OF(earc1, GetA()) != OF(earc2, GetA()))
		return false;
	if (OF(earc1, GetB()) != OF(earc2, GetB()))
		return false;

	return true;
}

EArc2D::EArc2D(void)
{
}

// vTL, vBL, vBR : 타원이 존재하는 직사각형 영역
// vBR과 vTR의 중점이 0도이며, 각도의 진행은 반시계 방향이다.
// dBeginAngle, dEArcAngle : Radian
// dEArcAngle이 2PI 이상이면 완전한 타원이 된다.
EArc2D::EArc2D(REF_CONST(Vertex2D) vTL, REF_CONST(Vertex2D) vBL, REF_CONST(Vertex2D) vBR, double dBeginAngle, double dEArcAngle, bool isClockWise)
{
	SetEArc(vTL, vBL, vBR, dBeginAngle, dEArcAngle, isClockWise);
}

EArc2D::~EArc2D(void)
{
}

/*#ifdef DOTNET
bool EArc2D::operator== (EArc2D^ op1, EArc2D^ op2)
{
	bool isNull1 = NullChecker::IsNull(op1);
	bool isNull2 = NullChecker::IsNull(op2);

	if (isNull1 && isNull2)
		return true;
	else if (isNull1 || isNull2)
		return false;

	return Equal<EArc2D>(op1, op2);
}

bool EArc2D::operator!= (EArc2D^ op1, EArc2D^ op2)
{
	bool isNull1 = NullChecker::IsNull(op1);
	bool isNull2 = NullChecker::IsNull(op2);

	if (isNull1 && isNull2)
		return false;
	else if (isNull1 || isNull2)
		return true;

	return !Equal<EArc2D>(op1, op2);
}

#else
bool EArc2D::operator== (const EArc2D& rhs) const
{
	return Equal<EArc2D>(THIS_OBJ, rhs);
}

bool EArc2D::operator!= (const EArc2D& rhs) const
{
	return !Equal<EArc2D>(THIS_OBJ, rhs);
}

#endif*/

// vTL, vBL, vBR : 타원이 존재하는 직사각형 영역
// vBR과 vTR의 중점이 0도이며, 각도의 진행은 반시계 방향이다.
// dBeginAngle, dEArcAngle : Radian
// dEArcAngle이 2PI 이상이면 완전한 타원이 된다.
bool EArc2D::SetEArc(REF_CONST(Vertex2D) vTL, REF_CONST(Vertex2D) vBL, REF_CONST(Vertex2D) vBR, double dBeginAngle, double dEArcAngle, bool isClockWise)
{
	if (fabs(Math::GetAngle(vTL, vBL, vBR) - Math::HALF_PI()) > Math::HALF_TOLERANCE())
		return false;

	OF(m_vTL, CopyFrom(vTL));
	OF(m_vBL, CopyFrom(vBL));
	OF(m_vBR, CopyFrom(vBR));

	m_dA = OF(vBL, GetDistance(vBR)) / 2;
	m_dB = OF(vTL, GetDistance(vBL)) / 2;

	m_dBeginAngle = dBeginAngle;
	m_dAngle	  = dEArcAngle;
	m_isClockWise = isClockWise;

	if (GetAngle() >= Math::_2PI() - Math::HALF_TOLERANCE())
		SetClosed(true);
	else
		SetClosed(false);

	m_vCenter = (vTL + vBR) / 2;

	return true;
}

// dAngle : Radian
// dAngle이 범위를 벗어나면 false를 리턴한다.
bool EArc2D::GetVertex(double dAngle, OUT CBR(INSTANCE(Vertex2D)) rVertex) CONST
{
	return _GetVertex<EArc2D, Vertex2D, double>(THIS_OBJ, dAngle, rVertex);
}

// rLine과 만나지 않으면 0을 리턴한다.
// 교차점이 하나만 존재할 경우 rVertex1에만 값이 담겨지며 1이 리턴된다.
// 교차점이 두 개 존재할 경우 rVertex1과 rVertex2에 각각 값이 담겨지며, 2가 리턴된다.
int EArc2D::IntersectLine(REF_CONST(Line2D) rLine, OUT CBR(INSTANCE(Vertex2D)) rVertex1, OUT CBR(INSTANCE(Vertex2D)) rVertex2) CONST
{
#ifdef DOTNET
	rVertex1 = gcnew Vertex2D();
	rVertex2 = gcnew Vertex2D();
#endif

	REF_CONST(Vertex2D) vBegin = OF(rLine, GetVertex(true));
	REF_CONST(Vertex2D) vEnd = OF(rLine, GetVertex(false));

	// rLine의 시작점과 끝점이 같으면 계산하지 않는다.
	if (OF(vBegin, GetDistance(vEnd)) <= UnE::Geometry::Math::HALF_TOLERANCE())
		return 0;

	// 타원의 방정식 x²/ A²+ y²/ B²= 1을 적용하여 계산하기 위해서는 타원의 중점을 원점에 오도록 위치이동 시킨후
	// 타원의 장축이 X축에 대하여 기울어진 만큼 직선도 회전시켜야 한다.
	// 따라서, 직선도 그만큼 위치이동 및 회전시킨다. 
	INSTANCE(Line2D) newLine;
	double theta = CoordTranslate(rLine, newLine);

	int nResult = _IntersectLine(newLine, rVertex1, rVertex2);

	if (nResult == 2)
	{
		rVertex1 = CoordTranslate(rVertex1, -theta) + m_vCenter;
		rVertex2 = CoordTranslate(rVertex2, -theta) + m_vCenter;
	}
	else if (nResult == 1)
		rVertex1 = CoordTranslate(rVertex1, -theta) + m_vCenter;

	return nResult;
	//REF_CONST(Vertex2D) _vBegin = OF(rLine, GetVertex(true));
	//REF_CONST(Vertex2D) _vEnd = OF(rLine, GetVertex(false));

	//// 타원의 방정식 x²/ A²+ y²/ B²= 1을 적용하여 계산하기 위해서는 타원의 중점을 원점에 오도록 위치이동 시켜야 한다.
	//// 따라서, 직선도 그만큼 위치이동 시킨다. 
	//INSTANCE(Vertex2D) vBegin = dnonlynew Vertex2D(OF(_vBegin, x) - OF(m_vCenter, x), OF(_vBegin, y) - OF(m_vCenter, y));
	//INSTANCE(Vertex2D) vEnd = dnonlynew Vertex2D(OF(_vEnd, x) - OF(m_vCenter, x), OF(_vEnd, y) - OF(m_vCenter, y));

	//// rLine 직선의 방정식 y = ax + b
	//// x = constant 형태의 직선일 경우
	//// 직선의 x값 : c
	//double a, b, c = 0.0;
	//bool bXEq = false;

	//if (fabs(OF(vBegin, x) - OF(vEnd, x)) < UnE::Geometry::Math::HALF_TOLERANCE())
	//{
	//	a = b = 0.0;
	//	c = OF(vBegin, x);
	//	bXEq = true;
	//}
	//else if (fabs(OF(vBegin, y) - OF(vEnd, y)) < UnE::Geometry::Math::HALF_TOLERANCE())
	//{
	//	a = 0.0;
	//	b = OF(vBegin, y);
	//}
	//else
	//{
	//	a = (OF(vEnd, y) - OF(vBegin, y)) / (OF(vEnd, x) - OF(vBegin, x));
	//	b = OF(vEnd, y) - a * OF(vEnd, x);
	//}

	//// 타원의 방정식 x²/ A²+ y²/ B²= 1
	//double A = m_dA;
	//double B = m_dB;

	//// 판별식
	//double dist;
	//if (bXEq)	// x = const 형태의 직선일 경우
	//{
	//	if (fabs(A) < UnE::Geometry::Math::HALF_TOLERANCE())
	//		dist = -1.0;
	//	else
	//		dist = 1 - c * c / A / A;
	//}
	//else
	//{
	//	double _A = A * A * a * a + B * B;
	//	double _B = 2 * a * b * A * A;
	//	double _C = A * A * b * b - A * A * B * B;

	//	dist = _B * _B - 4 * _A * _C;
	//	//dist = a * a * b * b / B / B / B / B - (1 / A / A + a * a / B / B) * (b * b / B / B - 1);
	//}

	//if (fabs(dist) < UnE::Geometry::Math::HALF_TOLERANCE()) // 교점이 하나이다.
	//{
	//	double x;

	//	if (bXEq)
	//	{
	//		x = c;

	//		double data = B * B - B * B * c * c / A / A;
	//		double y1 = sqrt(data);
	//		double y2 = -sqrt(data);

	//		if (fabs(y1 - y2) < UnE::Geometry::Math::HALF_TOLERANCE())
	//		{
	//			OF(rVertex1, x) = x + OF(m_vCenter, x);
	//			OF(rVertex1, y) = y1 + OF(m_vCenter, y);

	//			if (!OF(rLine, IsInclude(rVertex1)) || (!m_isClosed && !IsInclude(rVertex1)))
	//				return 0;
	//			else
	//				return 1;
	//		}
	//		else
	//			return InsertsectLineResult(x, y1, x, y2, rVertex1, rVertex2, rLine);
	//	}
	//	else
	//	{
	//		double _A = A * A * a * a + B * B;
	//		double _B = 2 * a * b * A * A;
	//		dist = sqrt(fabs(dist));

	//		if (_A < UnE::Geometry::Math::HALF_TOLERANCE())
	//			return 0;

	//		x = -_B / 2 / _A;
	//		/*double x1 = (-a * b / B / B + dist) / (1 / A / A + a * a / B / B);
	//		double x2 = (-a * b / B / B - dist) / (1 / A / A + a * a / B / B);

	//		if (x1 >= OF(vEnd, x) && x1 <= OF(vBegin, x))
	//			x = x1;
	//		else if (x2 >= OF(vEnd, x) && x2 <= OF(vBegin, x))
	//			x = x2;
	//		else
	//			return 0;*/
	//	}

	//	double y = a * x + b;

	//	OF(rVertex1, x) = x + OF(m_vCenter, x);
	//	OF(rVertex1, y) = y + OF(m_vCenter, y);

	//	if (!OF(rLine, IsInclude(rVertex1)))
	//		return 0;

	//	if (m_isClosed)
	//	{
	//		return 1;
	//	}
	//	else
	//	{
	//		if (IsInclude(rVertex1))
	//			return 1;
	//	}
	//}
	//else if (dist < 0.0)
	//	return 0;	// 교점이 존재하지 않는다.
	//else	// 교점이 두개다.
	//{
	//	double x1, y1, x2, y2;

	//	if (bXEq)
	//	{
	//		dist = sqrt(dist) * B;
	//		x1 = x2 = c;
	//		y1 = dist;
	//		y2 = -dist;
	//	}
	//	else
	//	{
	//		double _A = A * A * a * a + B * B;
	//		double _B = 2 * a * b * A * A;
	//		double _C = A * A * b * b - A * A * B * B;
	//		dist = sqrt(dist);

	//		x1 = (-_B + dist) / 2 / _A;
	//		x2 = (-_B - dist) / 2 / _A;
	//		y1 = a * x1 + b;
	//		y2 = a * x2 + b;

	//		/*dist = sqrt(4 * dist);
	//		x1 = (-2.0 * a * b / B / B + dist) / (2.0 * (1 / A / A + a * a / B / B));
	//		y1 = a * x1 + b;
	//		x2 = (-2.0 * a * b / B / B - dist) / (2.0 * (1 / A / A + a * a / B / B));
	//		y2 = a * x2 + b;*/
	//	}

	//	return InsertsectLineResult(x1, y1, x2, y2, rVertex1, rVertex2, rLine);
	//}

	//return 0;
/*#ifdef DOTNET
	rVertex1 = gcnew Vertex2D();
	rVertex2 = gcnew Vertex2D();
#endif

	Handle(Geom2d_Line) hLine;

	REF_CONST(Vertex2D) vBegin = OF(rLine, GetVertex(true));
	REF_CONST(Vertex2D) vEnd = OF(rLine, GetVertex(false));
	double dLen = OF(vBegin, GetDistance(vEnd));

	bool isOnePoint = false;

	// rLine이 한 점일 경우
	if (dLen < UnE::Geometry::Math::HALF_TOLERANCE())
	{
		isOnePoint = true;
		INSTANCE(Vertex2D) vEnd2 = dnonlynew Vertex2D(OF(vEnd, x), OF(vEnd, y) + 100.0);

		hLine = GCE2d_MakeLine(GP_POINT2D(vBegin), GP_POINT2D(vEnd2));
	}
	else
		hLine = GCE2D_MAKELINE(rLine);
	
	Handle(Geom2d_TrimmedCurve) hEArc;
	if (!MakeEArcHandle(THIS_OBJ, hEArc))
		return 0;

	Geom2dAPI_InterCurveCurve inter(hEArc, hLine);

	int nPointCount = inter.NbPoints();

	if (nPointCount == 1)
	{
		gp_Pnt2d pt = inter.Point(1);
		OF(rVertex1, SetVertex(pt.X(), pt.Y()));

		if (isOnePoint)
		{
			if (OF(vBegin, GetDistance(rVertex1)) > UnE::Geometry::Math::HALF_TOLERANCE())
				nPointCount = 0;
		}
	}
	else if (nPointCount == 2)
	{
		gp_Pnt2d pt1 = inter.Point(1);
		OF(rVertex1, SetVertex(pt1.X(), pt1.Y()));

		gp_Pnt2d pt2 = inter.Point(2);
		OF(rVertex2, SetVertex(pt2.X(), pt2.Y()));

		if (isOnePoint)
		{
			if (OF(vBegin, GetDistance(rVertex1)) < UnE::Geometry::Math::HALF_TOLERANCE() ||
				OF(vBegin, GetDistance(rVertex2)) < UnE::Geometry::Math::HALF_TOLERANCE())
				nPointCount = 1;
			else
				nPointCount = 0;
		}
	}

	return nPointCount;*/
}

int EArc2D::_IntersectLine(REF_CONST(Line2D) rLine, CBR(INSTANCE(Vertex2D)) rVertex1, CBR(INSTANCE(Vertex2D)) rVertex2) CONST
{
	REF_CONST(Vertex2D) vBegin = OF(rLine, GetVertex(true));
	REF_CONST(Vertex2D) vEnd = OF(rLine, GetVertex(false));

	// rLine 직선의 방정식 y = ax + b
	// x = constant 형태의 직선일 경우
	// 직선의 x값 : c
	double a, b, c = 0.0;
	bool xIsConst = false;

	if (fabs(OF(vBegin, x) - OF(vEnd, x)) < UnE::Geometry::Math::HALF_TOLERANCE())
	{
		a = b = 0.0;
		c = OF(vBegin, x);
		xIsConst = true;
	}
	else if (fabs(OF(vBegin, y) - OF(vEnd, y)) < UnE::Geometry::Math::HALF_TOLERANCE())
	{
		a = 0.0;
		b = OF(vBegin, y);
	}
	else
	{
		a = (OF(vEnd, y) - OF(vBegin, y)) / (OF(vEnd, x) - OF(vBegin, x));
		b = OF(vEnd, y) - a * OF(vEnd, x);
	}

	int nResultCount = 0;

	if (xIsConst)
		nResultCount = _IntersectLine(rLine, rVertex1, rVertex2, c);
	else
		nResultCount = _IntersectLine(rLine, rVertex1, rVertex2, a, b);

	if (nResultCount == 0)
		return 0;

	INSTANCE(Vertex2D) vR = dnonlynew Vertex2D(100, 0);
	INSTANCE(Vertex2D) vO = dnonlynew Vertex2D(0, 0);

	if (nResultCount == 1)
	{
		if (OF(rLine, IsInclude(rVertex1)) == false)
			nResultCount = 0;
		else
		{
			double dAngle = UnE::Geometry::Math::GetAngle(rVertex1, vO, vR);
			if (OF(rVertex1, x) < 0.0)
				dAngle = UnE::Geometry::Math::_2PI() - dAngle;

			if (!IsInclude(dAngle))
				nResultCount = 0;
		}
	}
	else// if (nResultCount == 2)
	{
		if (OF(rLine, IsInclude(rVertex1)) == false)
		{
			nResultCount--;
			rVertex1 = rVertex2;
		}
		else
		{
			double dAngle = UnE::Geometry::Math::GetAngle(rVertex1, vO, vR);
			if (OF(rVertex1, x) < 0.0)
				dAngle = UnE::Geometry::Math::_2PI() - dAngle;

			if (!IsInclude(dAngle))
			{
				nResultCount--;
				rVertex1 = rVertex2;
			}
		}

		if (OF(rLine, IsInclude(rVertex2)) == false)
			nResultCount--;
		else
		{
			double dAngle = UnE::Geometry::Math::GetAngle(rVertex2, vO, vR);
			if (OF(rVertex2, x) < 0.0)
				dAngle = UnE::Geometry::Math::_2PI() - dAngle;

			if (!IsInclude(dAngle))
				nResultCount--;
		}
	}

	return nResultCount;
}

// y = ax + b 인 직선과 타원의 교점
int EArc2D::_IntersectLine(REF_CONST(Line2D) rLine, CBR(INSTANCE(Vertex2D)) rVertex1, CBR(INSTANCE(Vertex2D)) rVertex2, double a, double b) CONST
{
	double A = a * a * m_dA * m_dA + m_dB * m_dB;
	double B = 2 * a * b * m_dA * m_dA;
	double C = b * b * m_dA * m_dA - m_dA * m_dA * m_dB * m_dB;
	double D = B * B - 4 * A * C;

	if (fabs(D) < UnE::Geometry::Math::HALF_TOLERANCE())
	{
		double x = -B / 2 / A;
		double y = a * x + b;
		OF(rVertex1, SetVertex(x, y));
		return 1;
	}
	else if (D < 0)
		return 0;

	double x1 = (-B + sqrt(D)) / 2 / A;
	double y1 = a * x1 + b;
	double x2 = (-B - sqrt(D)) / 2 / A;
	double y2 = a * x2 + b;

	OF(rVertex1, SetVertex(x1, y1));
	OF(rVertex2, SetVertex(x2, y2));
	return 2;
}

// x = c 인 직선과 타원의 교점
int EArc2D::_IntersectLine(REF_CONST(Line2D) rLine, CBR(INSTANCE(Vertex2D)) rVertex1, CBR(INSTANCE(Vertex2D)) rVertex2, double c) CONST
{
	double D = m_dB * m_dB - m_dB * m_dB * c * c / m_dA / m_dA;

	if (fabs(D) < UnE::Geometry::Math::HALF_TOLERANCE())
	{
		OF(rVertex1, SetVertex(c, 0.0));
		return 1;
	}
	else if (D < 0.0)
		return 0;

	OF(rVertex1, SetVertex(c, sqrt(D)));
	OF(rVertex2, SetVertex(c, -sqrt(D)));
	return 2;
}

// Return 값 : 타원의 회전각(Radian)
double EArc2D::CoordTranslate(REF_CONST(Line2D) rLine, OUT CBR(INSTANCE(Line2D)) result) CONST
{
	INSTANCE(Vertex2D) vR = m_vCenter * 2 - (m_vTL + m_vBL) / 2;
	INSTANCE(Vertex2D) vX = dnonlynew Vertex2D(OF(m_vCenter, x + 100), OF(m_vCenter, y));

	double theta = UnE::Geometry::Math::GetAngle(vR, m_vCenter, vX);

	if (OF(vR, y) < OF(m_vCenter, y))
		theta = UnE::Geometry::Math::_2PI() - theta;

	// m_vCenter만큼 좌표 이동
	INSTANCE(Vertex2D) vBegin = OF(rLine, GetVertex(true)) - m_vCenter;
	INSTANCE(Vertex2D) vEnd = OF(rLine, GetVertex(false)) - m_vCenter;

	// theta만큼 회전 이동
	vBegin = CoordTranslate(vBegin, theta);
	vEnd = CoordTranslate(vEnd, theta);

	result = dnonlynew Line2D(vBegin, vEnd, OF(rLine, GetLineType()));
	return theta;
}

INSTANCE(Vertex2D) EArc2D::CoordTranslate(REF_CONST(Vertex2D) rVertex, double theta) CONST
{
	double radius = sqrt(OF(rVertex, x) * OF(rVertex, x) + OF(rVertex, y) * OF(rVertex, y));

	if (radius < UnE::Geometry::Math::HALF_TOLERANCE())
		return dnonlynew Vertex2D(OF(rVertex, x), OF(rVertex, y));

	double cosData = (radius * radius + OF(rVertex, x) * OF(rVertex, x) - OF(rVertex, y) * OF(rVertex, y)) / 2 / radius / OF(rVertex, x);
	double alpha = acos(cosData);

	if (OF(rVertex, y) < 0.0)
		alpha = UnE::Geometry::Math::_2PI() - alpha;

	double x = radius * cos(alpha - theta);
	double y = radius * sin(alpha - theta);
	return dnonlynew Vertex2D(x, y);
}

/*int EArc2D::InsertsectLineResult(double x1, double y1, double x2, double y2, CBR(INSTANCE(Vertex2D)) rVertex1, CBR(INSTANCE(Vertex2D)) rVertex2, REF_CONST(Line2D) rLine) CONST
{
	OF(rVertex1, x) = x1 + OF(m_vCenter, x);
	OF(rVertex1, y) = y1 + OF(m_vCenter, y);
	OF(rVertex2, x) = x2 + OF(m_vCenter, x);
	OF(rVertex2, y) = y2 + OF(m_vCenter, y);

	int nResult = 2;

	if (!OF(rLine, IsInclude(rVertex1)) || (!m_isClosed && !IsInclude(rVertex1)))
	{
		OF(rVertex1, x) = x2 + OF(m_vCenter, x);
		OF(rVertex1, y) = y2 + OF(m_vCenter, y);
		nResult--;
	}

	if (!OF(rLine, IsInclude(rVertex2)) || (!m_isClosed && !IsInclude(rVertex2)))
	{
		nResult--;
	}

	return nResult;
}*/

// dAngle : Radian
bool EArc2D::IsInclude(double dAngle) CONST
{
	if (m_isClosed)
		return true;

	double dBeginAngle = m_dBeginAngle;

	while (dBeginAngle < 0.0)
		dBeginAngle += UnE::Geometry::Math::_2PI();

	while (dBeginAngle > UnE::Geometry::Math::_2PI())
		dBeginAngle -= UnE::Geometry::Math::_2PI();

	if (m_isClockWise)
	{
		if (m_dAngle <= dBeginAngle)
		{
			if (dAngle >= m_dAngle && dAngle <= dBeginAngle)
				return true;
		}
		else
		{
			if (dAngle <= dBeginAngle)
				return true;
			else if (Geometry::Math::_2PI() - (m_dAngle - dBeginAngle) <= dAngle)
				return true;
		}
	}
	else
	{
		if (m_dAngle + dBeginAngle <= Geometry::Math::_2PI())
		{
			if (dAngle >= dBeginAngle && dAngle <= m_dAngle + dBeginAngle)
				return true;
		}
		else
		{
			if (dAngle >= dBeginAngle)
				return true;
			else if (m_dAngle + dBeginAngle - Geometry::Math::_2PI() >= dAngle)
				return true;
		}
	}

	return false;
}

/*bool EArc2D::IsInclude(REF_CONST(Vertex2D) rVertex) CONST
{
	INSTANCE(Vertex2D) vL = (m_vTL + m_vBL) / 2;
	INSTANCE(Vertex2D) vR = m_vCenter * 2 - vL;

	double dAngle = Geometry::Math::GetAngle(vR, m_vCenter, rVertex);

	if (Geometry::Math::IsRightSideFromLine(rVertex, vR, m_vCenter))
		dAngle = Geometry::Math::_2PI() - dAngle;

	return IsInclude(dAngle);
}*/

// rEArc와 만나지 않으면 0을 리턴한다.
// Return 값 : 두 EArc가 만나서 생기는 (Vertex의 개수) + (EArc 개수 * 100)
//             만일, 두 EArc가 만나서 하나의 Vertex와 하나의 EArc가 생성된다면 101이 리턴된다.
#ifdef DOTNET
int EArc2D::IntersectEArc(EArc2D^ rEArc, OUT System::Collections::ArrayList^% rArrVertex, OUT System::Collections::ArrayList^% rArrEArc)
#else
int EArc2D::IntersectEArc(const EArc2D& rEArc, std::vector<Vertex2D>& rArrVertex, std::vector<EArc2D*>& rArrEArc) const
#endif
{
#ifdef DOTNET
	rArrVertex = gcnew System::Collections::ArrayList();
	rArrEArc = gcnew System::Collections::ArrayList();
#endif
	
	Handle(Geom2d_TrimmedCurve) hEArc1, hEArc2;
	
	if (!MakeEArcHandle(THIS_OBJ, hEArc1))
		return 0;
	if (!MakeEArcHandle(rEArc, hEArc2))
		return 0;

	Geom2dAPI_InterCurveCurve inter(hEArc1, hEArc2);
	
	int nSegmentError = 0;
	int nSegmentCount = inter.NbSegments();

	if (nSegmentCount > 0)
	{
		Handle(Geom2d_Curve) seg1, seg2;
		INSTANCE(Vertex2D) v1 = dnonlynew Vertex2D();
		INSTANCE(Vertex2D) v2 = dnonlynew Vertex2D();
		
		gp_Pnt2d pt1, pt2;

		for (int i=1;i<=nSegmentCount;i++)
		{
			try
			{
				inter.Segment(i, seg1, seg2);

				double dAngle1 = seg1->FirstParameter();
				double dAngle2 = seg1->LastParameter();

				seg1->D0(dAngle1, pt1);
				seg1->D0(dAngle2, pt2);

				OF(v1, SetVertex(pt1.X(), pt1.Y()));
				OF(v2, SetVertex(pt2.X(), pt2.Y()));

				POINTER(EArc2D) pEArc;

				if (OF(rEArc, GetType()) != ENUM_OF(EArcType, EARC))
				{
					OF(rEArc, NewObject(pEArc));
					OF(rEArc, MakeSub(pEArc, v1, v2));
				}
				else
				{
					NewObject(pEArc);
					MakeSub(pEArc, v1, v2);
				}

				pEArc->SetIntersectResult(POINTER_VALUE(this), rEArc);
				COLLECTION_ADD(rArrEArc, pEArc);
			}
			catch (Standard_Failure& /*rFail*/)
			{
				// Vertex를 Segment로 잘못 계산하였음.
				// Segment 구간의 시작과 끝점이 동일함.
				double U;

				const Geom2dInt_GInter& rIntersector = inter.Intersector();

				IntRes2d_IntersectionSegment Seg = rIntersector.Segment(i);

				if (Seg.IsOpposite())
				{
					if (Seg.HasFirstPoint())
					{
						IntRes2d_IntersectionPoint IP1 = Seg.FirstPoint();
						U = IP1.ParamOnFirst();
					}
					else
					{
						U = seg1->FirstParameter();
					}
				}
				else
				{
					if (Seg.HasFirstPoint())
					{
						IntRes2d_IntersectionPoint IP1 = Seg.FirstPoint();
						U = IP1.ParamOnFirst();
					}
					else
					{
						U = seg1->FirstParameter();
					}
				}

				nSegmentError++;
				hEArc1->D0(U, pt1);

				OF(v1, SetVertex(pt1.X(), pt1.Y()));
				COLLECTION_ADD(rArrVertex, v1);
			}
		}
	}
	
	int nPointCount = inter.NbPoints();
	
	for (int i=1;i<=nPointCount;i++)
	{
		gp_Pnt2d pt = inter.Point(i);

		INSTANCE(Vertex2D) vertex = dnonlynew Vertex2D(pt.X(), pt.Y());
		COLLECTION_ADD(rArrVertex, vertex);
	}

	return (nSegmentCount - nSegmentError) * 100 + (nPointCount + nSegmentError);
}

// EArc위의 한점 vertex로부터 EArc의 진행방향으로 len만큼 떨어진 곳의 좌표를 구한다.
// vertex가 EArc의 각도 범위에 포함되지 않아도 상관없다.
bool EArc2D::GetLinearVertex(REF_CONST(Vertex2D) vertex, double len, OUT CBR(INSTANCE(Vertex2D)) rResult)
{
	double dAngle = GetVertexAngle(vertex);
	return GetLinearVertex(dAngle, len, rResult);
}

// dAngle : Radian
// EArc의 dAngle 위치에 있는 좌표로부터 EArc의 진행방향으로 len만큼 떨어진 곳의 좌표를 구한다.
// dAngle이 EArc의 각도 범위에 포함되지 않아도 상관없다.
bool EArc2D::GetLinearVertex(double dAngle, double len, OUT CBR(INSTANCE(Vertex2D)) rResult)
{
#ifdef DOTNET
	rResult = NULL_PTR;
#endif

	Handle_Geom2d_Ellipse hEllipse;

	if (!MakeEllipseHandle(THIS_OBJ, hEllipse))
		return false;

	if (IsClockWise())
		len = -len;

	Geom2dAdaptor_Curve c(hEllipse);
	double param = CPnts_AbscissaPoint(c, len, dAngle, 0.001).Parameter();
	
	bool isClosed = m_isClosed;
	m_isClosed = true;

	GetVertex(param, rResult);
	m_isClosed = isClosed;
	return true;
}

INSTANCE(Vertex2D) EArc2D::GetBeginVertex() CONST
{
	INSTANCE(Vertex2D) v = dnonlynew Vertex2D();
	_GetVertex<EArc2D, Vertex2D, double>(THIS_OBJ, m_dBeginAngle, v, false);
	return v;
}

INSTANCE(Vertex2D) EArc2D::GetEndVertex() CONST
{
	INSTANCE(Vertex2D) v = dnonlynew Vertex2D();
	_GetVertex<EArc2D, Vertex2D, double>(THIS_OBJ, m_isClockWise ? m_dBeginAngle - m_dAngle : m_dBeginAngle + m_dAngle, v, false);
	return v;
}

void EArc2D::NewObject(CBR(POINTER(EArc2D)) pEArc) CONST
{
	pEArc = geonew EArc2D();
}

// vBegin에서 vEnd방향으로(반시계 방향) 향하는 rEArc를 만든다.
void EArc2D::MakeSub(POINTER(EArc2D) pEArc, REF_CONST(Vertex2D) vBegin, REF_CONST(Vertex2D) vEnd) CONST
{
	pEArc->m_isClockWise = false;
	pEArc->m_isClosed = false;

	pEArc->OF(m_vTL, CopyFrom(this->m_vTL));
	pEArc->OF(m_vBL, CopyFrom(this->m_vBL));
	pEArc->OF(m_vBR, CopyFrom(this->m_vBR));
	pEArc->OF(m_vCenter, CopyFrom(this->m_vCenter));

	pEArc->m_dA = m_dA;
	pEArc->m_dB = m_dB;

	double theta1 = GetVertexAngle(vBegin);
	double theta2 = GetVertexAngle(vEnd);

	// 반시계 방향이다.
	double dEArcAngle = theta2 - theta1;
	if (dEArcAngle < Math::_2PI()) dEArcAngle += Math::_2PI();

	pEArc->m_dBeginAngle = theta1;
	pEArc->m_dAngle = dEArcAngle;
}

// rEArc를 타원의 바깥으로 dLen 만큼 확대(outside가 false이면 축소)시킨다.
INSTANCE(EArc2D) EArc2D::Offset(bool outside, double dLen) CONST
{
	INSTANCE(EArc2D) earc = dnonlynew EArc2D();
	_Offset(earc, outside, dLen);
	return earc;
}

// v1과 v2를 지나는 직선을 기준으로 현재의 EArc 객체와 대칭되는 객체를 만들어 리턴한다.
// v1과 v2가 동일한 좌표일 경우 false를 리턴한다.
bool EArc2D::Mirror(REF_CONST(Vertex2D) v1, REF_CONST(Vertex2D) v2, OUT CBR(INSTANCE(EArc2D)) rResult)
{
#ifdef DOTNET
	rResult = nullptr;
#endif

	if (OF(v1, GetDistance(v2)) <= Math::HALF_TOLERANCE())
		return false;

	rResult = dnonlynew EArc2D();
	return _Mirror(v1, v2, rResult);	
}

bool EArc2D::_Mirror(REF_CONST(Vertex2D) v1, REF_CONST(Vertex2D) v2, REF(EArc2D) rResult)
{
	INSTANCE(Vertex2D) vTR = m_vTL + m_vBR - m_vBL;
	INSTANCE(Vertex2D) _vTR = Math::GetNearestVertex(vTR, v1, v2, true);
	INSTANCE(Vertex2D) _vBL = Math::GetNearestVertex(m_vBL, v1, v2, true);
	INSTANCE(Vertex2D) _vBR = Math::GetNearestVertex(m_vBR, v1, v2, true);

	// Mirror를 하면 좌우가 바뀌므로 왼쪽과 오른쪽을 바꾸어준다.
	OF(rResult, m_vTL) = _vTR * 2 - vTR;
	OF(rResult, m_vBL) = _vBR * 2 - m_vBR;
	OF(rResult, m_vBR) = _vBL * 2 - m_vBL;
	OF(rResult, m_vCenter) = (OF(rResult, m_vTL) + OF(rResult, m_vBR)) / 2;
	OF(rResult, m_isClockWise) = !m_isClockWise;
	OF(rResult, m_isClosed) = m_isClosed;
	OF(rResult, m_dA) = m_dA;
	OF(rResult, m_dB) = m_dB;
	//OF(rResult, m_dBeginAngle) = m_dBeginAngle;
	OF(rResult, m_dAngle) = m_dAngle;

	if (m_dBeginAngle < Math::HALF_PI())
		OF(rResult, m_dBeginAngle) = Math::HALF_PI() + (Math::HALF_PI() - m_dBeginAngle);
	else if (m_dBeginAngle < Math::PI())
		OF(rResult, m_dBeginAngle) = Math::HALF_PI() - (m_dBeginAngle - Math::HALF_PI());
	else if (m_dBeginAngle < Math::_3HALF_PI())
		OF(rResult, m_dBeginAngle) = Math::_3HALF_PI() + (Math::_3HALF_PI() - m_dBeginAngle);
	else// if (m_dBeginAngle < Math::_2PI())
		OF(rResult, m_dBeginAngle) = Math::_3HALF_PI() - (m_dBeginAngle - Math::_3HALF_PI());

	return true;
}

// EArc와의 교차점 검사이후 사용
void EArc2D::SetIntersectResult(REF_CONST(EArc2D) earc1, REF_CONST(EArc2D) earc2)
{
	INSTANCE(Vertex2D) vBegin = GetBeginVertex();
	INSTANCE(Vertex2D) vEnd = GetEndVertex();

	double dBeginAngle = GetVertexAngle(vBegin);
	double dEndAngle = GetVertexAngle(vEnd);

	bool clockwise = IsClockWise();

	if (OF(earc1, IsClockWise()) == OF(earc2, IsClockWise()))
	{
		if (IsClockWise() != OF(earc1, IsClockWise()))
		{
			double temp = dBeginAngle;
			dBeginAngle = dEndAngle;
			dEndAngle = temp;
			clockwise = !IsClockWise();
		}
	}

	double dEArcAngle = GetRealAngle(dBeginAngle, dEndAngle, clockwise);
	SetIntersectResult(dBeginAngle, dEArcAngle, clockwise);
}

void EArc2D::SetIntersectResult(double dBeginAngle, double dEArcAngle, bool clockwise)
{
	SetEArc(GetTL(), GetBL(), GetBR(), dBeginAngle, dEArcAngle, clockwise);
}

EArc2F::EArc2F(void)
{
}

// vTL, vBL, vBR : 타원이 존재하는 직사각형 영역
// vBR과 vTR의 중점이 0도이며, 각도의 진행은 반시계 방향이다.
// dBeginAngle, dEArcAngle : Radian
// dEArcAngle이 2PI 이상이면 완전한 타원이 된다.
EArc2F::EArc2F(REF_CONST(Vertex2F) vTL, REF_CONST(Vertex2F) vBL, REF_CONST(Vertex2F) vBR, double dBeginAngle, double dEArcAngle, bool isClockWise)
{
	SetEArc(vTL, vBL, vBR, dBeginAngle, dEArcAngle, isClockWise);
}

EArc2F::~EArc2F(void)
{
}

/*#ifdef DOTNET
bool EArc2F::operator== (EArc2F^ op1, EArc2F^ op2)
{
	bool isNull1 = NullChecker::IsNull(op1);
	bool isNull2 = NullChecker::IsNull(op2);

	if (isNull1 && isNull2)
		return true;
	else if (isNull1 || isNull2)
		return false;

	return Equal<EArc2F>(op1, op2);
}

bool EArc2F::operator!= (EArc2F^ op1, EArc2F^ op2)
{
	bool isNull1 = NullChecker::IsNull(op1);
	bool isNull2 = NullChecker::IsNull(op2);

	if (isNull1 && isNull2)
		return false;
	else if (isNull1 || isNull2)
		return true;

	return !Equal<EArc2F>(op1, op2);
}

#else
bool EArc2F::operator== (const EArc2F& rhs) const
{
	return Equal<EArc2F>(THIS_OBJ, rhs);
}

bool EArc2F::operator!= (const EArc2F& rhs) const
{
	return !Equal<EArc2F>(THIS_OBJ, rhs);
}

#endif*/

// vTL, vBL, vBR : 타원이 존재하는 직사각형 영역
// vBR과 vTR의 중점이 0도이며, 각도의 진행은 반시계 방향이다.
// dBeginAngle, dEArcAngle : Radian
// dEArcAngle이 2PI 이상이면 완전한 타원이 된다.
bool EArc2F::SetEArc(REF_CONST(Vertex2F) vTL, REF_CONST(Vertex2F) vBL, REF_CONST(Vertex2F) vBR, double dBeginAngle, double dEArcAngle, bool isClockWise)
{
	if (fabs(Math::GetAngle(vTL, vBL, vBR) - Math::HALF_PI()) > Math::HALF_TOLERANCE())
		return false;

	OF(m_vTL, CopyFrom(vTL));
	OF(m_vBL, CopyFrom(vBL));
	OF(m_vBR, CopyFrom(vBR));

	m_dA = OF(vBL, GetDistance(vBR)) / 2;
	m_dB = OF(vTL, GetDistance(vBL)) / 2;

	m_dBeginAngle = dBeginAngle;
	m_dAngle = dEArcAngle;
	m_isClockWise = isClockWise;

	if (GetAngle() >= Math::_2PI() - Math::HALF_TOLERANCE())
		SetClosed(true);
	else
		SetClosed(false);

	m_vCenter = (vTL + vBR) / 2;

	return true;
}

// dAngle : Radian
// dAngle이 범위를 벗어나면 false를 리턴한다.
bool EArc2F::GetVertex(double dAngle, OUT CBR(INSTANCE(Vertex2F)) rVertex) CONST
{
	return _GetVertex<EArc2F, Vertex2F, float>(THIS_OBJ, dAngle, rVertex);
}

// rLine과 만나지 않으면 0을 리턴한다.
// 교차점이 하나만 존재할 경우 rVertex1에만 값이 담겨지며 1이 리턴된다.
// 교차점이 두 개 존재할 경우 rVertex1과 rVertex2에 각각 값이 담겨지며, 2가 리턴된다.
int EArc2F::IntersectLine(REF_CONST(Line2F) rLine, OUT CBR(INSTANCE(Vertex2F)) rVertex1, OUT CBR(INSTANCE(Vertex2F)) rVertex2) CONST
{
#ifdef DOTNET
	rVertex1 = gcnew Vertex2F();
	rVertex2 = gcnew Vertex2F();
#endif

	REF_CONST(Vertex2F) vBegin = OF(rLine, GetVertex(true));
	REF_CONST(Vertex2F) vEnd = OF(rLine, GetVertex(false));

	// rLine의 시작점과 끝점이 같으면 계산하지 않는다.
	if (OF(vBegin, GetDistance(vEnd)) <= UnE::Geometry::Math::HALF_TOLERANCE())
		return 0;

	// 타원의 방정식 x²/ A²+ y²/ B²= 1을 적용하여 계산하기 위해서는 타원의 중점을 원점에 오도록 위치이동 시킨후
	// 타원의 장축이 X축에 대하여 기울어진 만큼 직선도 회전시켜야 한다.
	// 따라서, 직선도 그만큼 위치이동 및 회전시킨다. 
	INSTANCE(Line2F) newLine;
	double theta = CoordTranslate(rLine, newLine);

	int nResult = _IntersectLine(newLine, rVertex1, rVertex2);

	if (nResult == 2)
	{
		rVertex1 = CoordTranslate(rVertex1, -theta) + m_vCenter;
		rVertex2 = CoordTranslate(rVertex2, -theta) + m_vCenter;
	}
	else if (nResult == 1)
		rVertex1 = CoordTranslate(rVertex1, -theta) + m_vCenter;

	return nResult;
}

int EArc2F::_IntersectLine(REF_CONST(Line2F) rLine, CBR(INSTANCE(Vertex2F)) rVertex1, CBR(INSTANCE(Vertex2F)) rVertex2) CONST
{
	REF_CONST(Vertex2F) vBegin = OF(rLine, GetVertex(true));
	REF_CONST(Vertex2F) vEnd = OF(rLine, GetVertex(false));

	// rLine 직선의 방정식 y = ax + b
	// x = constant 형태의 직선일 경우
	// 직선의 x값 : c
	float a, b, c = 0.0f;
	bool xIsConst = false;

	if (fabs(OF(vBegin, x) - OF(vEnd, x)) < UnE::Geometry::Math::HALF_TOLERANCE())
	{
		a = b = 0.0f;
		c = OF(vBegin, x);
		xIsConst = true;
	}
	else if (fabs(OF(vBegin, y) - OF(vEnd, y)) < UnE::Geometry::Math::HALF_TOLERANCE())
	{
		a = 0.0f;
		b = OF(vBegin, y);
	}
	else
	{
		a = (OF(vEnd, y) - OF(vBegin, y)) / (OF(vEnd, x) - OF(vBegin, x));
		b = OF(vEnd, y) - a * OF(vEnd, x);
	}

	int nResultCount = 0;

	if (xIsConst)
		nResultCount = _IntersectLine(rLine, rVertex1, rVertex2, c);
	else
		nResultCount = _IntersectLine(rLine, rVertex1, rVertex2, a, b);

	if (nResultCount == 0)
		return 0;

	INSTANCE(Vertex2F) vR = dnonlynew Vertex2F(100, 0);
	INSTANCE(Vertex2F) vO = dnonlynew Vertex2F(0, 0);

	if (nResultCount == 1)
	{
		if (OF(rLine, IsInclude(rVertex1)) == false)
			nResultCount = 0;
		else
		{
			double dAngle = UnE::Geometry::Math::GetAngle(rVertex1, vO, vR);
			if (OF(rVertex1, x) < 0.0)
				dAngle = UnE::Geometry::Math::_2PI() - dAngle;

			if (!IsInclude(dAngle))
				nResultCount = 0;
		}
	}
	else// if (nResultCount == 2)
	{
		if (OF(rLine, IsInclude(rVertex1)) == false)
		{
			nResultCount--;
			rVertex1 = rVertex2;
		}
		else
		{
			double dAngle = UnE::Geometry::Math::GetAngle(rVertex1, vO, vR);
			if (OF(rVertex1, x) < 0.0)
				dAngle = UnE::Geometry::Math::_2PI() - dAngle;

			if (!IsInclude(dAngle))
			{
				nResultCount--;
				rVertex1 = rVertex2;
			}
		}

		if (OF(rLine, IsInclude(rVertex2)) == false)
			nResultCount--;
		else
		{
			double dAngle = UnE::Geometry::Math::GetAngle(rVertex2, vO, vR);
			if (OF(rVertex2, x) < 0.0)
				dAngle = UnE::Geometry::Math::_2PI() - dAngle;

			if (!IsInclude(dAngle))
				nResultCount--;
		}
	}

	return nResultCount;
}

// y = ax + b 인 직선과 타원의 교점
int EArc2F::_IntersectLine(REF_CONST(Line2F) rLine, CBR(INSTANCE(Vertex2F)) rVertex1, CBR(INSTANCE(Vertex2F)) rVertex2, float a, float b) CONST
{
	float A = a * a * m_dA * m_dA + m_dB * m_dB;
	float B = 2 * a * b * m_dA * m_dA;
	float C = b * b * m_dA * m_dA - m_dA * m_dA * m_dB * m_dB;
	float D = B * B - 4 * A * C;

	if (fabs(D) < UnE::Geometry::Math::HALF_TOLERANCE())
	{
		float x = -B / 2 / A;
		float y = a * x + b;
		OF(rVertex1, SetVertex(x, y));
		return 1;
	}
	else if (D < 0)
		return 0;

	float x1 = (-B + sqrt(D)) / 2 / A;
	float y1 = a * x1 + b;
	float x2 = (-B - sqrt(D)) / 2 / A;
	float y2 = a * x2 + b;

	OF(rVertex1, SetVertex(x1, y1));
	OF(rVertex2, SetVertex(x2, y2));
	return 2;
}

// x = c 인 직선과 타원의 교점
int EArc2F::_IntersectLine(REF_CONST(Line2F) rLine, CBR(INSTANCE(Vertex2F)) rVertex1, CBR(INSTANCE(Vertex2F)) rVertex2, float c) CONST
{
	float D = m_dB * m_dB - m_dB * m_dB * c * c / m_dA / m_dA;

	if (fabs(D) < UnE::Geometry::Math::HALF_TOLERANCE())
	{
		OF(rVertex1, SetVertex(c, 0.0f));
		return 1;
	}
	else if (D < 0.0f)
		return 0;

	OF(rVertex1, SetVertex(c, sqrt(D)));
	OF(rVertex2, SetVertex(c, -sqrt(D)));
	return 2;
}

// Return 값 : 타원의 회전각(Radian)
double EArc2F::CoordTranslate(REF_CONST(Line2F) rLine, OUT CBR(INSTANCE(Line2F)) result) CONST
{
	INSTANCE(Vertex2F) vR = m_vCenter * 2 - (m_vTL + m_vBL) / 2;
	INSTANCE(Vertex2F) vX = dnonlynew Vertex2F(OF(m_vCenter, x + 100), OF(m_vCenter, y));

	double theta = UnE::Geometry::Math::GetAngle(vR, m_vCenter, vX);

	if (OF(vR, y) < OF(m_vCenter, y))
		theta = UnE::Geometry::Math::_2PI() - theta;

	// m_vCenter만큼 좌표 이동
	INSTANCE(Vertex2F) vBegin = OF(rLine, GetVertex(true)) - m_vCenter;
	INSTANCE(Vertex2F) vEnd = OF(rLine, GetVertex(false)) - m_vCenter;

	// theta만큼 회전 이동
	vBegin = CoordTranslate(vBegin, theta);
	vEnd = CoordTranslate(vEnd, theta);

	result = dnonlynew Line2F(vBegin, vEnd, OF(rLine, GetLineType()));
	return theta;
}

INSTANCE(Vertex2F) EArc2F::CoordTranslate(REF_CONST(Vertex2F) rVertex, double theta) CONST
{
	float radius = sqrt(OF(rVertex, x) * OF(rVertex, x) + OF(rVertex, y) * OF(rVertex, y));

	if (radius < UnE::Geometry::Math::HALF_TOLERANCE())
		return dnonlynew Vertex2F(OF(rVertex, x), OF(rVertex, y));

	double cosData = (radius * radius + OF(rVertex, x) * OF(rVertex, x) - OF(rVertex, y) * OF(rVertex, y)) / 2 / radius / OF(rVertex, x);
	double alpha = acos(cosData);

	if (OF(rVertex, y) < 0.0)
		alpha = UnE::Geometry::Math::_2PI() - alpha;

	double x = radius * cos(alpha - theta);
	double y = radius * sin(alpha - theta);
	return dnonlynew Vertex2F((float)x, (float)y);
}

// dAngle : Radian
bool EArc2F::IsInclude(double dAngle) CONST
{
	if (m_isClosed)
	return true;

	double dBeginAngle = m_dBeginAngle;

	while (dBeginAngle < 0.0)
		dBeginAngle += UnE::Geometry::Math::_2PI();

	while (dBeginAngle > UnE::Geometry::Math::_2PI())
		dBeginAngle -= UnE::Geometry::Math::_2PI();

	if (m_isClockWise)
	{
		if (m_dAngle <= dBeginAngle)
		{
			if (dAngle >= m_dAngle && dAngle <= dBeginAngle)
				return true;
		}
		else
		{
			if (dAngle <= dBeginAngle)
				return true;
			else if (Geometry::Math::_2PI() - (m_dAngle - dBeginAngle) <= dAngle)
				return true;
		}
	}
	else
	{
		if (m_dAngle + dBeginAngle <= Geometry::Math::_2PI())
		{
			if (dAngle >= dBeginAngle && dAngle <= m_dAngle + dBeginAngle)
				return true;
		}
		else
		{
			if (dAngle >= dBeginAngle)
				return true;
			else if (m_dAngle + dBeginAngle - Geometry::Math::_2PI() >= dAngle)
				return true;
		}
	}

	return false;
}

// rEArc와 만나지 않으면 0을 리턴한다.
// Return 값 : 두 EArc가 만나서 생기는 (Vertex의 개수) + (EArc 개수 * 100)
//             만일, 두 EArc가 만나서 하나의 Vertex와 하나의 EArc가 생성된다면 101이 리턴된다.
#ifdef DOTNET
int EArc2F::IntersectEArc(EArc2F^ rEArc, OUT System::Collections::ArrayList^% rArrVertex, OUT System::Collections::ArrayList^% rArrEArc)
#else
int EArc2F::IntersectEArc(const EArc2F& rEArc, std::vector<Vertex2F>& rArrVertex, std::vector<EArc2F*>& rArrEArc) const
#endif
{
#ifdef DOTNET
	rArrVertex = gcnew System::Collections::ArrayList();
	rArrEArc = gcnew System::Collections::ArrayList();
#endif

	Handle(Geom2d_TrimmedCurve) hEArc1, hEArc2;

	if (!MakeEArcHandle(THIS_OBJ, hEArc1))
		return 0;
	if (!MakeEArcHandle(rEArc, hEArc2))
		return 0;

	Geom2dAPI_InterCurveCurve inter(hEArc1, hEArc2);

	int nSegmentError = 0;
	int nSegmentCount = inter.NbSegments();

	if (nSegmentCount > 0)
	{
		Handle(Geom2d_Curve) seg1, seg2;
		INSTANCE(Vertex2F) v1 = dnonlynew Vertex2F();
		INSTANCE(Vertex2F) v2 = dnonlynew Vertex2F();

		gp_Pnt2d pt1, pt2;

		for (int i = 1; i <= nSegmentCount; i++)
		{
			try
			{
				inter.Segment(i, seg1, seg2);

				double dAngle1 = seg1->FirstParameter();
				double dAngle2 = seg1->LastParameter();

				seg1->D0(dAngle1, pt1);
				seg1->D0(dAngle2, pt2);

				OF(v1, SetVertex((float)pt1.X(), (float)pt1.Y()));
				OF(v2, SetVertex((float)pt2.X(), (float)pt2.Y()));

				POINTER(EArc2F) pEArc;

				if (OF(rEArc, GetType()) != ENUM_OF(EArcType, EARC))
				{
					OF(rEArc, NewObject(pEArc));
					OF(rEArc, MakeSub(pEArc, v1, v2));
				}
				else
				{
					NewObject(pEArc);
					MakeSub(pEArc, v1, v2);
				}

				pEArc->SetIntersectResult(POINTER_VALUE(this), rEArc);
				COLLECTION_ADD(rArrEArc, pEArc);
			}
			catch (Standard_Failure& /*rFail*/)
			{
				// Vertex를 Segment로 잘못 계산하였음.
				// Segment 구간의 시작과 끝점이 동일함.
				double U;

				const Geom2dInt_GInter& rIntersector = inter.Intersector();

				IntRes2d_IntersectionSegment Seg = rIntersector.Segment(i);

				if (Seg.IsOpposite())
				{
					if (Seg.HasFirstPoint())
					{
						IntRes2d_IntersectionPoint IP1 = Seg.FirstPoint();
						U = IP1.ParamOnFirst();
					}
					else
					{
						U = seg1->FirstParameter();
					}
				}
				else
				{
					if (Seg.HasFirstPoint())
					{
						IntRes2d_IntersectionPoint IP1 = Seg.FirstPoint();
						U = IP1.ParamOnFirst();
					}
					else
					{
						U = seg1->FirstParameter();
					}
				}

				nSegmentError++;
				hEArc1->D0(U, pt1);

				OF(v1, SetVertex((float)pt1.X(), (float)pt1.Y()));
				COLLECTION_ADD(rArrVertex, v1);
			}
		}
	}

	int nPointCount = inter.NbPoints();

	for (int i = 1; i <= nPointCount; i++)
	{
		gp_Pnt2d pt = inter.Point(i);

		INSTANCE(Vertex2F) vertex = dnonlynew Vertex2F((float)pt.X(), (float)pt.Y());
		COLLECTION_ADD(rArrVertex, vertex);
	}

	return (nSegmentCount - nSegmentError) * 100 + (nPointCount + nSegmentError);
}

// EArc위의 한점 vertex로부터 EArc의 진행방향으로 len만큼 떨어진 곳의 좌표를 구한다.
// vertex가 EArc의 각도 범위에 포함되지 않아도 상관없다.
bool EArc2F::GetLinearVertex(REF_CONST(Vertex2F) vertex, float len, OUT CBR(INSTANCE(Vertex2F)) rResult)
{
	double dAngle = GetVertexAngle(vertex);
	return GetLinearVertex(dAngle, len, rResult);
}

// dAngle : Radian
// EArc의 dAngle 위치에 있는 좌표로부터 EArc의 진행방향으로 len만큼 떨어진 곳의 좌표를 구한다.
// dAngle이 EArc의 각도 범위에 포함되지 않아도 상관없다.
bool EArc2F::GetLinearVertex(double dAngle, float len, OUT CBR(INSTANCE(Vertex2F)) rResult)
{
#ifdef DOTNET
	rResult = NULL_PTR;
#endif

	Handle_Geom2d_Ellipse hEllipse;

	if (!MakeEllipseHandle(THIS_OBJ, hEllipse))
		return false;

	if (IsClockWise())
		len = -len;

	Geom2dAdaptor_Curve c(hEllipse);
	double param = CPnts_AbscissaPoint(c, len, dAngle, 0.001).Parameter();

	bool isClosed = m_isClosed;
	m_isClosed = true;

	GetVertex(param, rResult);
	m_isClosed = isClosed;
	return true;
}

INSTANCE(Vertex2F) EArc2F::GetBeginVertex() CONST
{
	INSTANCE(Vertex2F) v = dnonlynew Vertex2F();
	_GetVertex<EArc2F, Vertex2F, float>(THIS_OBJ, m_dBeginAngle, v, false);
	return v;
}

INSTANCE(Vertex2F) EArc2F::GetEndVertex() CONST
{
	INSTANCE(Vertex2F) v = dnonlynew Vertex2F();
	_GetVertex<EArc2F, Vertex2F, float>(THIS_OBJ, m_isClockWise ? m_dBeginAngle - m_dAngle : m_dBeginAngle + m_dAngle, v, false);
	return v;
}

void EArc2F::NewObject(CBR(POINTER(EArc2F)) pEArc) CONST
{
	pEArc = geonew EArc2F();
}

// vBegin에서 vEnd방향으로(반시계 방향) 향하는 rEArc를 만든다.
void EArc2F::MakeSub(POINTER(EArc2F) pEArc, REF_CONST(Vertex2F) vBegin, REF_CONST(Vertex2F) vEnd) CONST
{
	pEArc->m_isClockWise = false;
	pEArc->m_isClosed = false;

	pEArc->OF(m_vTL, CopyFrom(this->m_vTL));
	pEArc->OF(m_vBL, CopyFrom(this->m_vBL));
	pEArc->OF(m_vBR, CopyFrom(this->m_vBR));
	pEArc->OF(m_vCenter, CopyFrom(this->m_vCenter));

	pEArc->m_dA = m_dA;
	pEArc->m_dB = m_dB;

	double theta1 = GetVertexAngle(vBegin);
	double theta2 = GetVertexAngle(vEnd);

	// 반시계 방향이다.
	double dEArcAngle = theta2 - theta1;
	if (dEArcAngle < Math::_2PI()) dEArcAngle += Math::_2PI();

	pEArc->m_dBeginAngle = theta1;
	pEArc->m_dAngle = dEArcAngle;
}

// rEArc를 타원의 바깥으로 dLen 만큼 확대(outside가 false이면 축소)시킨다.
INSTANCE(EArc2F) EArc2F::Offset(bool outside, float dLen) CONST
{
	INSTANCE(EArc2F) earc = dnonlynew EArc2F();
	_Offset(earc, outside, dLen);
	return earc;
}

// v1과 v2를 지나는 직선을 기준으로 현재의 EArc 객체와 대칭되는 객체를 만들어 리턴한다.
// v1과 v2가 동일한 좌표일 경우 false를 리턴한다.
bool EArc2F::Mirror(REF_CONST(Vertex2F) v1, REF_CONST(Vertex2F) v2, OUT CBR(INSTANCE(EArc2F)) rResult)
{
#ifdef DOTNET
	rResult = nullptr;
#endif

	if (OF(v1, GetDistance(v2)) <= Math::HALF_TOLERANCE())
		return false;

	rResult = dnonlynew EArc2F();
	return _Mirror(v1, v2, rResult);
}

bool EArc2F::_Mirror(REF_CONST(Vertex2F) v1, REF_CONST(Vertex2F) v2, REF(EArc2F) rResult)
{
	INSTANCE(Vertex2F) vTR = m_vTL + m_vBR - m_vBL;
	INSTANCE(Vertex2F) _vTR = Math::GetNearestVertex(vTR, v1, v2, true);
	INSTANCE(Vertex2F) _vBL = Math::GetNearestVertex(m_vBL, v1, v2, true);
	INSTANCE(Vertex2F) _vBR = Math::GetNearestVertex(m_vBR, v1, v2, true);

	// Mirror를 하면 좌우가 바뀌므로 왼쪽과 오른쪽을 바꾸어준다.
	OF(rResult, m_vTL) = _vTR * 2 - vTR;
	OF(rResult, m_vBL) = _vBR * 2 - m_vBR;
	OF(rResult, m_vBR) = _vBL * 2 - m_vBL;
	OF(rResult, m_vCenter) = (OF(rResult, m_vTL) + OF(rResult, m_vBR)) / 2;
	OF(rResult, m_isClockWise) = !m_isClockWise;
	OF(rResult, m_isClosed) = m_isClosed;
	OF(rResult, m_dA) = m_dA;
	OF(rResult, m_dB) = m_dB;
	//OF(rResult, m_dBeginAngle) = m_dBeginAngle;
	OF(rResult, m_dAngle) = m_dAngle;

	if (m_dBeginAngle < Math::HALF_PI())
		OF(rResult, m_dBeginAngle) = Math::HALF_PI() + (Math::HALF_PI() - m_dBeginAngle);
	else if (m_dBeginAngle < Math::PI())
		OF(rResult, m_dBeginAngle) = Math::HALF_PI() - (m_dBeginAngle - Math::HALF_PI());
	else if (m_dBeginAngle < Math::_3HALF_PI())
		OF(rResult, m_dBeginAngle) = Math::_3HALF_PI() + (Math::_3HALF_PI() - m_dBeginAngle);
	else// if (m_dBeginAngle < Math::_2PI())
		OF(rResult, m_dBeginAngle) = Math::_3HALF_PI() - (m_dBeginAngle - Math::_3HALF_PI());

	return true;
}

// EArc와의 교차점 검사이후 사용
void EArc2F::SetIntersectResult(REF_CONST(EArc2F) earc1, REF_CONST(EArc2F) earc2)
{
	INSTANCE(Vertex2F) vBegin = GetBeginVertex();
	INSTANCE(Vertex2F) vEnd = GetEndVertex();

	double dBeginAngle = GetVertexAngle(vBegin);
	double dEndAngle = GetVertexAngle(vEnd);

	bool clockwise = IsClockWise();

	if (OF(earc1, IsClockWise()) == OF(earc2, IsClockWise()))
	{
		if (IsClockWise() != OF(earc1, IsClockWise()))
		{
			double temp = dBeginAngle;
			dBeginAngle = dEndAngle;
			dEndAngle = temp;
			clockwise = !IsClockWise();
		}
	}

	double dEArcAngle = GetRealAngle(dBeginAngle, dEndAngle, clockwise);
	SetIntersectResult(dBeginAngle, dEArcAngle, clockwise);
}

void EArc2F::SetIntersectResult(double dBeginAngle, double dEArcAngle, bool clockwise)
{
	SetEArc(GetTL(), GetBL(), GetBR(), dBeginAngle, dEArcAngle, clockwise);
}

EArc3D::EArc3D(void)
{
}

// vTL, vBL, vBR : 타원이 존재하는 직사각형 영역
// vBR과 vTR의 중점이 0도이며, 각도의 진행은 반시계 방향이다.
// dBeginAngle, dEArcAngle : Radian
// dEArcAngle이 2PI 이상이면 완전한 타원이 된다.
EArc3D::EArc3D(REF_CONST(Vertex3D) vTL, REF_CONST(Vertex3D) vBL, REF_CONST(Vertex3D) vBR, double dBeginAngle, double dEArcAngle, bool isClockWise)
{
	SetEArc(vTL, vBL, vBR, dBeginAngle, dEArcAngle, isClockWise);
}

EArc3D::~EArc3D(void)
{
}

/*#ifdef DOTNET
bool EArc3D::operator== (EArc3D^ op1, EArc3D^ op2)
{
	bool isNull1 = NullChecker::IsNull(op1);
	bool isNull2 = NullChecker::IsNull(op2);

	if (isNull1 && isNull2)
		return true;
	else if (isNull1 || isNull2)
		return false;

	return Equal<EArc3D>(op1, op2);
}

bool EArc3D::operator!= (EArc3D^ op1, EArc3D^ op2)
{
	bool isNull1 = NullChecker::IsNull(op1);
	bool isNull2 = NullChecker::IsNull(op2);

	if (isNull1 && isNull2)
		return false;
	else if (isNull1 || isNull2)
		return true;

	return !Equal<EArc3D>(op1, op2);
}

#else
bool EArc3D::operator== (const EArc3D& rhs) const
{
	return Equal<EArc3D>(THIS_OBJ, rhs);
}

bool EArc3D::operator!= (const EArc3D& rhs) const
{
	return !Equal<EArc3D>(THIS_OBJ, rhs);
}

#endif*/

// vTL, vBL, vBR : 타원이 존재하는 직사각형 영역
// vBR과 vTR의 중점이 0도이며, 각도의 진행은 반시계 방향이다.
// dBeginAngle, dEArcAngle : Radian
// dEArcAngle이 2PI 이상이면 완전한 타원이 된다.
bool EArc3D::SetEArc(REF_CONST(Vertex3D) vTL, REF_CONST(Vertex3D) vBL, REF_CONST(Vertex3D) vBR, double dBeginAngle, double dEArcAngle, bool isClockWise)
{
	if (fabs(Math::GetAngle(vTL, vBL, vBR) - Math::HALF_PI()) > Math::HALF_TOLERANCE())
		return false;

	OF(m_vTL, CopyFrom(vTL));
	OF(m_vBL, CopyFrom(vBL));
	OF(m_vBR, CopyFrom(vBR));

	//double major = vBL.GetDistance(vBR) / 2;
	//double minor = vTL.GetDistance(vBL) / 2;
	m_dA = OF(vBL, GetDistance(vBR)) / 2;
	m_dB = OF(vTL, GetDistance(vBL)) / 2;

	m_dBeginAngle = dBeginAngle;
	m_dAngle	  = dEArcAngle;
	m_isClockWise = isClockWise;

	if (GetAngle() >= Math::_2PI() - Math::HALF_TOLERANCE())
		SetClosed(true);
	else
		SetClosed(false);

	m_vCenter = (vTL + vBR) / 2;

	return true;
}

// dAngle : Radian
// dAngle이 범위를 벗어나면 false를 리턴한다.
bool EArc3D::GetVertex(double dAngle, OUT CBR(INSTANCE(Vertex3D)) rVertex) CONST
{
	return _GetVertex<EArc3D, Vertex3D, double>(THIS_OBJ, dAngle, rVertex);
}

INSTANCE(Vertex3D) EArc3D::GetBeginVertex() CONST
{
	INSTANCE(Vertex3D) v = dnonlynew Vertex3D();
	_GetVertex<EArc3D, Vertex3D, double>(THIS_OBJ, m_dBeginAngle, v, false);
	return v;
}

INSTANCE(Vertex3D) EArc3D::GetEndVertex() CONST
{
	INSTANCE(Vertex3D) v = dnonlynew Vertex3D();
	_GetVertex<EArc3D, Vertex3D, double>(THIS_OBJ, m_isClockWise ? m_dBeginAngle - m_dAngle : m_dBeginAngle + m_dAngle, v, false);
	return v;
}

void EArc3D::NewObject(CBR(POINTER(EArc3D)) pEArc) CONST
{
	pEArc = geonew EArc3D();
}

// rEArc를 타원의 바깥으로 dLen 만큼 확대(outside가 false이면 축소)시킨다.
INSTANCE(EArc3D) EArc3D::Offset(bool outside, double dLen) CONST
{
	INSTANCE(EArc3D) earc = dnonlynew EArc3D();
	_Offset(earc, outside, dLen);
	return earc;
}

// v1, v2, v3를 지나는 평면을 기준으로 현재의 EArc와 대칭되는 객체를 만들어 리턴한다.
// v1, v2, v3가 평면을 구성하지 못할 경우 false를 리턴한다.
bool EArc3D::Mirror(REF_CONST(Vertex3D) v1, REF_CONST(Vertex3D) v2, REF_CONST(Vertex3D) v3, OUT CBR(INSTANCE(EArc3D)) rResult)
{
#ifdef DOTNET
	rResult = nullptr;
#endif

	double a, b, c, d;	// ax + by + cz + d = 0
	if (!Math::MakePlane(v1, v2, v3, a, b, c, d))
		return false;

	rResult = dnonlynew EArc3D();

	return _Mirror(a, b, c, d, rResult);
}

bool EArc3D::_Mirror(double a, double b, double c, double d, REF(EArc3D) rResult)
{
	INSTANCE(Vertex3D) vTR = m_vTL + m_vBR - m_vBL;
	INSTANCE(Vertex3D) _vTR = Math::GetNearestVertex(vTR, a, b, c, d);
	INSTANCE(Vertex3D) _vBL = Math::GetNearestVertex(m_vBL, a, b, c, d);
	INSTANCE(Vertex3D) _vBR = Math::GetNearestVertex(m_vBR, a, b, c, d);

	// Mirror를 하면 좌우가 바뀌므로 왼쪽과 오른쪽을 바꾸어준다.
	OF(rResult, m_vTL) = _vTR * 2 - vTR;
	OF(rResult, m_vBL) = _vBR * 2 - m_vBR;
	OF(rResult, m_vBR) = _vBL * 2 - m_vBL;
	OF(rResult, m_vCenter) = (OF(rResult, m_vTL) + OF(rResult, m_vBR)) / 2;
	OF(rResult, m_isClockWise) = !m_isClockWise;
	OF(rResult, m_isClosed) = m_isClosed;
	OF(rResult, m_dA) = m_dA;
	OF(rResult, m_dB) = m_dB;
	//OF(rResult, m_dBeginAngle) = m_dBeginAngle;
	OF(rResult, m_dAngle) = m_dAngle;

	if (m_dBeginAngle < Math::HALF_PI())
		OF(rResult, m_dBeginAngle) = Math::HALF_PI() + (Math::HALF_PI() - m_dBeginAngle);
	else if (m_dBeginAngle < Math::PI())
		OF(rResult, m_dBeginAngle) = Math::HALF_PI() - (m_dBeginAngle - Math::HALF_PI());
	else if (m_dBeginAngle < Math::_3HALF_PI())
		OF(rResult, m_dBeginAngle) = Math::_3HALF_PI() + (Math::_3HALF_PI() - m_dBeginAngle);
	else// if (m_dBeginAngle < Math::_2PI())
		OF(rResult, m_dBeginAngle) = Math::_3HALF_PI() - (m_dBeginAngle - Math::_3HALF_PI());

	return true;
}

END_NS
END_NS
