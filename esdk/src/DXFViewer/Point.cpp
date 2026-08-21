#include "StdAfx.h"
#include "Point.h"
#include "IPainter.h"
#include "EditBox.h"

using namespace UnE::Geometry;

BEGIN_NS(DXFViewer)

Point::Point(void)
{
	m_point = gcnew System::Drawing::PointF();

	m_vertex = gcnew Vertex2D();
	m_vSelected = gcnew Vertex2F();
}

Point::~Point(void)
{
}

Point::Point(Point^ rhs)
{
	m_point = gcnew System::Drawing::PointF();

	m_vertex = gcnew Vertex2D(rhs->m_vertex->x, rhs->m_vertex->y);
	m_vSelected = gcnew Vertex2F(rhs->m_vSelected->x, rhs->m_vSelected->y);
}

Point::Point(double x, double y)
{
	m_point = gcnew System::Drawing::PointF();

	m_vertex = gcnew Vertex2D(x, y);
	m_vSelected = gcnew Vertex2F((float)x, (float)y);
}

// (x,y)만큼 객체를 옮긴다.
void Point::Move(double x, double y)
{
	m_vertex->x += x;
	m_vertex->y += y;
	
	m_point->X = /*m_arrPointGL[0] = */(float)m_vertex->x;
	m_point->Y = /*m_arrPointGL[1] = */(float)m_vertex->y;
	
	m_vSelected->SetVertex((float)x, (float)y);
}

bool Point::Draw(System::Drawing::Graphics^ g, bool bDrawText)
{
	if (m_vertex == nullptr || m_pOwner == nullptr)
		return false;

	if (m_pOwner->Renderer == IPainter::RendererType::GDI_PLUS)
	{
		return DrawGDI(g);
	}
	else if (m_pOwner->Renderer == IPainter::RendererType::OPEN_GL)
	{
		return DrawGL();
	}

	return false;
}

bool Point::DrawGDI(System::Drawing::Graphics^ g)
{
	return true;
}

bool Point::DrawGL()
{
	return true;
}

Shape::ShapeType Point::GetShapeType()
{
	return ShapeType::POINT;
}

Shape^ Point::Clone()
{
	Point^ point = gcnew Point();
	point->CopyFrom(this);

	if (this->m_vertex == nullptr)
		point->m_vertex = nullptr;
	else
		point->m_vertex = gcnew UnE::Geometry::Vertex2D(this->m_vertex->x, this->m_vertex->y);

	if (this->m_point == nullptr)
		point->m_point = nullptr;
	else
		point->m_point = this->m_point;

	if (this->m_vertex != nullptr)
		point->m_vSelected = gcnew Vertex2F(this->m_vSelected->x, this->m_vSelected->y);

	return point;
}

// Selectable이 false이면 HitTest 검사가 무조건 실패한다.
bool Point::HitTest(double x, double y)
{
	if (m_vertex == nullptr)
		return false;

	if (!Selectable)
		return false;

	if (m_pOwner == nullptr)
		return false;

	// Line의 경우 화면의 Zoom Value에 따라 측정 범위가 달라지게 된다.
	// 화면 좌표 기준으로 2Pixel 이내면 선택된 것으로 간주한다.
	UnE::Geometry::Vertex2D^ vOrigin = m_pOwner->ScreenToGlobal(0, 0);
	UnE::Geometry::Vertex2D^ v2 = m_pOwner->ScreenToGlobal(2, 0);
	double dLen = vOrigin->GetDistance(v2);

	return m_vertex->GetDistance(gcnew UnE::Geometry::Vertex2D(x, y)) <= dLen;
}

bool Point::CheckClipBounds(System::Drawing::Graphics^ g, UnE::Geometry::Vertex2D^ vClipTL, UnE::Geometry::Vertex2D^ vClipBR)
{
	if (m_vertex == nullptr)
		return false;

	UnE::Geometry::Vertex2D^ vTL = m_vertex;
	UnE::Geometry::Vertex2D^ vBR = m_vertex;

	return CheckClipBounds(vClipTL, vClipBR, vTL, vBR);
}

END_NS
