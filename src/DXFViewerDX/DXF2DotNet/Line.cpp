#include "StdAfx.h"
#include "Line.h"
#include "LineType.h"
#include "IShapeOwner.h"

using namespace UnE::Geometry;

BEGIN_NS(DXFDotNet)

Line::Line(void)
{
	m_arrPoint = gcnew array<System::Drawing::PointF>(2);

	Begin = gcnew Vertex2D();
	End = gcnew Vertex2D();
	m_vSelectedBegin = gcnew Vertex2F();
	m_vSelectedEnd = gcnew Vertex2F();
}

Line::~Line(void)
{

}

Line::Line(Line^ rhs)
{
	m_arrPoint = gcnew array<System::Drawing::PointF>(2);
	

	Begin = gcnew Vertex2D(rhs->m_vBegin->x, rhs->m_vBegin->y);
	End = gcnew Vertex2D(rhs->m_vEnd->x, rhs->m_vEnd->y);
	m_vSelectedBegin = gcnew Vertex2F(rhs->m_vSelectedBegin->x, rhs->m_vSelectedBegin->y);
	m_vSelectedEnd = gcnew Vertex2F(rhs->m_vSelectedEnd->x, rhs->m_vSelectedEnd->y);
}

Line::Line(UnE::Geometry::Vertex2D^ vBegin, UnE::Geometry::Vertex2D^ vEnd)
{
	m_arrPoint = gcnew array<System::Drawing::PointF>(2);

	Begin = gcnew Vertex2D(vBegin->x, vBegin->y);
	End = gcnew Vertex2D(vEnd->x, vEnd->y);

	Vertex2D^ v1 = UnE::Geometry::Math::GetRightVertex(vBegin, vEnd, -0.5);
	Vertex2D^ v2 = UnE::Geometry::Math::GetRightVertex(vEnd, vBegin, 0.5);
	m_vSelectedBegin->SetVertex((float)v1->x, (float)v1->y);
	m_vSelectedEnd->SetVertex((float)v2->x, (float)v2->y);
}

// (x,y)만큼 객체를 옮긴다.
void Line::Move(double x, double y)
{
	m_vBegin->x += x;
	m_vBegin->y += y;
	m_vEnd->x += x;
	m_vEnd->y += y;

	m_arrPoint[0].X = (float)m_vBegin->x;
	m_arrPoint[0].Y =  (float)m_vBegin->y;
	m_arrPoint[1].X = (float)m_vEnd->x;
	m_arrPoint[1].Y =(float)m_vEnd->y;
	
	Vertex2D^ v1 = UnE::Geometry::Math::GetRightVertex(m_vBegin, m_vEnd, -0.5);
	Vertex2D^ v2 = UnE::Geometry::Math::GetRightVertex(m_vEnd, m_vBegin, 0.5);
	m_vSelectedBegin->SetVertex((float)v1->x, (float)v1->y);
	m_vSelectedEnd->SetVertex((float)v2->x, (float)v2->y);
}

Shape::ShapeType Line::GetShapeType()
{
	return ShapeType::LINE;
}

Shape^ Line::Clone()
{
	Line^ line = CreateLine();
	line->CopyFrom(this);

	if (this->m_vBegin == nullptr)
		line->m_vBegin = nullptr;
	else
		line->m_vBegin = gcnew UnE::Geometry::Vertex2D(this->m_vBegin->x, this->m_vBegin->y);

	if (this->m_vEnd == nullptr)
		line->m_vEnd = nullptr;
	else
		line->m_vEnd = gcnew UnE::Geometry::Vertex2D(this->m_vEnd->x, this->m_vEnd->y);

	if (this->m_arrPoint == nullptr || this->m_arrPoint->Length < 2)
		line->m_arrPoint = nullptr;
	else
	{
		line->m_arrPoint = gcnew array<System::Drawing::PointF>(2);
		line->m_arrPoint[0] = this->m_arrPoint[0];
		line->m_arrPoint[1] = this->m_arrPoint[1];
	}

	if (this->m_vBegin != nullptr && this->m_vEnd != nullptr)
	{
		line->m_vSelectedBegin = gcnew Vertex2F(this->m_vSelectedBegin->x, this->m_vSelectedBegin->y);
		line->m_vSelectedEnd = gcnew Vertex2F(this->m_vSelectedEnd->x, this->m_vSelectedEnd->y);
	}

	return line;
}

// Selectable이 false이면 HitTest 검사가 무조건 실패한다.
bool Line::HitTest(double x, double y)
{
	if (!Selectable)
		return false;

	if (m_pOwner == nullptr)
		return false;

	UnE::Geometry::Line2D^ line = gcnew UnE::Geometry::Line2D(m_vBegin, m_vEnd);

	// Line의 경우 화면의 Zoom Value에 따라 측정 범위가 달라지게 된다.
	// 화면 좌표 기준으로 2Pixel 이내면 선택된 것으로 간주한다.
	UnE::Geometry::Vertex2D^ vOrigin = m_pOwner->ScreenToGlobal(0, 0);
	UnE::Geometry::Vertex2D^ v2 = m_pOwner->ScreenToGlobal(2, 0);
	double dLen = vOrigin->GetDistance(v2);

	return line->GetDistance(gcnew UnE::Geometry::Vertex2D(x, y), false) <= dLen;
}


END_NS
