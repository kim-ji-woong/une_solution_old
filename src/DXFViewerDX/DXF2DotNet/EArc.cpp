#include "StdAfx.h"
#include "EArc.h"
#include "LineType.h"
#include "Layer.h"

using namespace UnE::Geometry;

BEGIN_NS(DXFDotNet)

EArc::EArc(void)
{
	m_vTL = m_vBL = m_vBR = nullptr;
	m_dWidth = m_dHeight = 0.0;
	m_isEllipse = false;
	m_dBeginAngle = 0.0;
	m_dEArcAngle = 0.0;
}

EArc::~EArc(void)
{
}

// (x,y)만큼 객체를 옮긴다.
void EArc::Move(double x, double y)
{
	if (m_vTL != nullptr)
	{
		m_vTL->x += x;
		m_vTL->y += y;
		m_vBL->x += x;
		m_vBL->y += y;
		m_vBR->x += x;
		m_vBR->y += y;
	}
}

Shape::ShapeType EArc::GetShapeType()
{
	return ShapeType::EARC;
}

Shape^ EArc::Clone()
{
	EArc^ eArc = CreateEArc();
	eArc->CopyFrom(this);

	if (this->m_vTL == nullptr)
		eArc->m_vTL = nullptr;
	else
		eArc->m_vTL = gcnew UnE::Geometry::Vertex2D(this->m_vTL->x, this->m_vTL->y);

	eArc->m_dWidth		= this->m_dWidth;
	eArc->m_dHeight		= this->m_dHeight;
	eArc->m_isEllipse	= this->m_isEllipse;
	eArc->m_dBeginAngle = this->m_dBeginAngle;
	eArc->m_dEArcAngle	= this->m_dEArcAngle;

	return eArc;
}

// Selectable이 false이면 HitTest 검사가 무조건 실패한다.
bool EArc::HitTest(double x, double y)
{
	if (!Selectable)
		return false;

	if (m_vTL == nullptr)
		return false;

	Vertex2D^ vTR = m_vBR - m_vBL + m_vTL;
	Vertex2D^ vertex = gcnew Vertex2D(x, y);

	int nDir1 = UnE::Geometry::Math::IsRightSideFromLine(vertex, m_vTL, m_vBL);
	int nDir2 = UnE::Geometry::Math::IsRightSideFromLine(vertex, vTR, m_vBR);

	if (nDir1 < 0 || nDir2 < 0)
	{
		// vTL<->vBL을 잇는 직선 또는 vTR<->vBR을 잇는 직선위에 (x, y)가 존재한다.
		return true;
	}
	else if (nDir1 == nDir2)
	{
		// (x, y)가 vTL<->vBL을 잇는 직선, vTR<->vBR을 잇는 직선으로부터 같은 방향에 놓여 있다.
		return false;
	}

	int nDir3 = UnE::Geometry::Math::IsRightSideFromLine(vertex, vTR, m_vTL);
	int nDir4 = UnE::Geometry::Math::IsRightSideFromLine(vertex, m_vBR, m_vBL);

	if (nDir3 < 0 || nDir4 < 0)
	{
		// vTR<->vTL을 잇는 직선 또는 vBR<->vBL을 잇는 직선위에 (x, y)가 존재한다.
		return true;
	}
	else if (nDir3 == nDir4)
	{
		// (x, y)가 vTR<->vTL을 잇는 직선, vBR<->vBL을 잇는 직선으로부터 같은 방향에 놓여 있다.
		return false;
	}
	return true;	
}


END_NS
