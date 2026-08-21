#include "StdAfx.h"
#include "PolyLine.h"
#include "LineType.h"

using namespace UnE::Geometry;

BEGIN_NS(DXFDotNet)

PolyLine::PolyLine(void)
{
	m_arrPoint = nullptr;
	m_polygon = nullptr;
	m_vTL = gcnew UnE::Geometry::Vertex2D();
	m_vBR = gcnew UnE::Geometry::Vertex2D();
	m_isInitArea = false;
	m_vSelectedBegin = gcnew UnE::Geometry::Vertex2F();
	m_vSelectedEnd = gcnew UnE::Geometry::Vertex2F();
	m_isClosed = false;

	m_brushPolygon = nullptr;
}

PolyLine::~PolyLine(void)
{
}

// (x,y)만큼 객체를 옮긴다.
void PolyLine::Move(double x, double y)
{
	if (m_arrPoint == nullptr)
		return;

	int nPointCount = m_arrPoint->Length;

	for (int i=0;i<nPointCount;i++)
	{
		m_arrPoint[i].X += (float)x;
		m_arrPoint[i].Y += (float)y;

		Vertex2D^ vertex = m_polygon->GetVertex(i);
		vertex->x += x;
		vertex->y += y;
	}

	if (m_isInitArea)
	{
		m_vTL->x += x;
		m_vTL->y += y;
		m_vBR->x += x;
		m_vBR->y += y;
	}

	ResetSelectedVertex(nPointCount);
}

void PolyLine::ResetSelectedVertex(int nPointCount)
{
	if (nPointCount > 0)
	{
		Vertex2D^ v1 = gcnew Vertex2D(m_arrPoint[0].X, m_arrPoint[0].Y);
		Vertex2D^ v2 = gcnew Vertex2D(m_arrPoint[nPointCount - 1].X, m_arrPoint[nPointCount - 1].Y);
		Vertex2D^ v3 = UnE::Geometry::Math::GetRightVertex(v1, v2, -0.5);
		Vertex2D^ v4 = UnE::Geometry::Math::GetRightVertex(v2, v1, 0.5);

		m_vSelectedBegin->SetVertex((float)v3->x, (float)v3->y);
		m_vSelectedEnd->SetVertex((float)v4->x, (float)v4->y);
	}
}

void PolyLine::CheckClosed(int nPointCount)
{
	if (nPointCount <= 1)
		m_isClosed = false;
	else
	{
		float fDistance = (float)System::Math::Sqrt((m_arrPoint[0].X - m_arrPoint[nPointCount - 1].X) * (m_arrPoint[0].X - m_arrPoint[nPointCount - 1].X) +
			(m_arrPoint[0].Y - m_arrPoint[nPointCount - 1].Y) * (m_arrPoint[0].Y - m_arrPoint[nPointCount - 1].Y));

		if (fDistance <= UnE::Geometry::Math::HALF_TOLERANCE())
			m_isClosed = true;
		else
			m_isClosed = false;
	}
}



Shape::ShapeType PolyLine::GetShapeType()
{
	return ShapeType::POLYLINE;
}

void PolyLine::SetVertex(System::Collections::ArrayList^ arrVertices)
{
	if (arrVertices == nullptr)
	{
		m_arrPoint = nullptr;
		m_polygon = nullptr;


		return;
	}


	int nPointCount = arrVertices->Count;

	if (nPointCount == 0)
	{
		m_arrPoint = nullptr;
		m_polygon = nullptr;
		return;
	}

	m_arrPoint = gcnew array<System::Drawing::PointF>(nPointCount);
	m_polygon = gcnew UnE::Geometry::Polygon();


	for (int i=0;i<nPointCount;i++)
	{
		Vertex2D^ vertex = (Vertex2D^)arrVertices[i];

		m_arrPoint[i].X = (float)vertex->x;
		m_arrPoint[i].Y = (float)vertex->y;

		m_polygon->AddVertex(vertex);

		if (i == 0)
		{
			m_isInitArea = true;

			m_vTL->x = vertex->x;
			m_vTL->y = vertex->y;
			m_vBR->x = vertex->x;
			m_vBR->y = vertex->y;
		}
		else
		{
			if (m_vTL->x > vertex->x) m_vTL->x = vertex->x;
			if (m_vTL->y < vertex->y) m_vTL->y = vertex->y;
			if (m_vBR->x < vertex->x) m_vBR->x = vertex->x;
			if (m_vBR->y > vertex->y) m_vBR->y = vertex->y;
		}
	}

	ResetSelectedVertex(nPointCount);
	CheckClosed(nPointCount);
}

bool PolyLine::UpdatePoint(int nIndex, float x, float y)
{
	if (nIndex < 0 || m_arrPoint->Length <= nIndex)
		return false;

	m_arrPoint[nIndex].X = x;
	m_arrPoint[nIndex].Y = y;

	if (m_isInitArea)
	{
		if (m_vTL->x > x) m_vTL->x = x;
		if (m_vTL->y < y) m_vTL->y = y;
		if (m_vBR->x < x) m_vBR->x = x;
		if (m_vBR->y > y) m_vBR->y = y;
	}
	else
	{
		m_isInitArea = true;

		m_vTL->x = x;
		m_vTL->y = y;
		m_vBR->x = x;
		m_vBR->y = y;
	}

	ResetSelectedVertex(m_arrPoint->Length);
	CheckClosed(m_arrPoint->Length);

	return m_polygon->UpdateVertex(nIndex, gcnew UnE::Geometry::Vertex2D(x, y));
	//return true;
}

void PolyLine::SetPointSize(int nPointCount)
{
	if (nPointCount < 0)
		return;

	m_isInitArea = false;

	if (nPointCount == 0)
	{
		m_arrPoint = nullptr;
		m_polygon = nullptr;
	}
	else
	{
		m_arrPoint = gcnew array<System::Drawing::PointF>(nPointCount);
		m_polygon = gcnew UnE::Geometry::Polygon();
	
		for (int i=0;i<nPointCount;i++)
			m_polygon->AddVertex(gcnew UnE::Geometry::Vertex2D());
	}
}

int PolyLine::GetVertexSize()
{
	return m_arrPoint->Length;
}

System::Drawing::PointF PolyLine::GetVertex(int nIndex)
{
	if (nIndex < 0 || m_arrPoint->Length <= nIndex)
		return System::Drawing::PointF();

	return m_arrPoint[nIndex];
}


Shape^ PolyLine::Clone()
{
	PolyLine^ pLine = CreatePolyLine();
	pLine->CopyFrom(this);

	if (this->m_arrPoint == nullptr || this->m_polygon == nullptr)
	{
		pLine->m_arrPoint = nullptr;
		pLine->m_polygon = nullptr;
	}
	else
	{
		int nPointCount = this->m_arrPoint->Length;


		if (nPointCount == 0)
		{
			pLine->m_arrPoint = nullptr;
			pLine->m_polygon = nullptr;
		}
		else
		{
			pLine->m_arrPoint = gcnew array<System::Drawing::PointF>(nPointCount);
			pLine->m_polygon = gcnew UnE::Geometry::Polygon();
			

			for (int i=0;i<nPointCount;i++)
			{
				pLine->m_arrPoint[i] = this->m_arrPoint[i];
			
				
				UnE::Geometry::Vertex2D^ vertex = this->m_polygon->GetVertex(i);
				pLine->m_polygon->AddVertex(gcnew UnE::Geometry::Vertex2D(vertex->x, vertex->y));
			}
		}

		pLine->m_vTL = gcnew UnE::Geometry::Vertex2D(this->m_vTL->x, this->m_vTL->y);
		pLine->m_vBR = gcnew UnE::Geometry::Vertex2D(this->m_vBR->x, this->m_vBR->y);

		pLine->m_vSelectedBegin = gcnew Vertex2F(this->m_vSelectedBegin->x, this->m_vSelectedBegin->y);
		pLine->m_vSelectedEnd = gcnew Vertex2F(this->m_vSelectedEnd->x, this->m_vSelectedEnd->y);

		pLine->m_isClosed = this->m_isClosed;
	}

	return pLine;
}

// Selectable이 false이면 HitTest 검사가 무조건 실패한다.
bool PolyLine::HitTest(double x, double y)
{
	if (!Selectable)
		return false;

	if (m_polygon == nullptr)
		return false;
	
	return m_polygon->HitTest(gcnew Vertex2D(x, y)) != 0;
}

UnE::Geometry::Polygon^ PolyLine::GetPolygon()
{
	return m_polygon;
}


END_NS
