#include "StdAfx.h"
#include "Arc.h"
#include "LineType.h"


using namespace UnE::Geometry;

BEGIN_NS(DXFDotNet)

Arc::Arc(void)
{
	m_vCenter = nullptr;
	m_dBeginAngle = m_dArcAngle = 0.0;
	m_dRadius = 0.0;
	m_isCircle = false;
}

Arc::~Arc(void)
{
}

// (x,y)만큼 객체를 옮긴다.
void Arc::Move(double x, double y)
{
	if (m_vCenter != nullptr)
	{
		m_vCenter->x += x;
		m_vCenter->y += y;
	}
}

Shape::ShapeType Arc::GetShapeType()
{
	return ShapeType::ARC;
}

Shape^ Arc::Clone()
{
	Arc^ arc = CreateArc();
	arc->CopyFrom(this);

	if (this->m_vCenter == nullptr)
		arc->m_vCenter = nullptr;
	else
		arc->m_vCenter	= gcnew UnE::Geometry::Vertex2D(this->m_vCenter->x, this->m_vCenter->y);

	arc->m_dBeginAngle	= this->m_dBeginAngle;
	arc->m_dArcAngle	= this->m_dArcAngle;
	arc->m_dRadius		= this->m_dRadius;
	arc->m_isCircle		= this->m_isCircle;

	return arc;
}

// Selectable이 false이면 HitTest 검사가 무조건 실패한다.
bool Arc::HitTest(double x, double y)
{
	if (!Selectable)
		return false;
	
	if (m_vCenter == nullptr)
		return false;

	double dLen = m_vCenter->GetDistance(gcnew UnE::Geometry::Vertex2D(x, y));
	return dLen <= m_dRadius;
}

END_NS
