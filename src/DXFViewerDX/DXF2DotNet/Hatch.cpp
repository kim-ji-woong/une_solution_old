#include "StdAfx.h"
#include "Hatch.h"
#include "LineType.h"

#include "poly2tri/sweep/cdt.h"
#include <map>

using namespace UnE::Geometry;

BEGIN_NS(DXFDotNet)

Hatch::Hatch(void)
{
	m_arrPoint = nullptr;
	m_polygon = nullptr;
	m_vTL = gcnew UnE::Geometry::Vertex2D();
	m_vBR = gcnew UnE::Geometry::Vertex2D();
	m_isInitArea = false;
	m_brush = gcnew System::Drawing::SolidBrush(System::Drawing::Color::Black);
}

Hatch::~Hatch(void)
{

}

// (x,y)만큼 객체를 옮긴다.
void Hatch::Move(double x, double y)
{
	m_ptCenter.X += (float)x;
	m_ptCenter.Y += (float)y;

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

	UpdatePoint(true);
}


Shape::ShapeType Hatch::GetShapeType()
{
	return ShapeType::HATCH;
}

void Hatch::SetVertex(System::Collections::ArrayList^ arrVertices)
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

}

bool Hatch::UpdatePoint(int nIndex, float x, float y)
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

	return m_polygon->UpdateVertex(nIndex, gcnew UnE::Geometry::Vertex2D(x, y));
	//return true;
}

void Hatch::SetPointSize(int nPointCount)
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

Shape^ Hatch::Clone()
{
	Hatch^ hatch = CreateHatch();
	hatch->CopyFrom(this);

	if (this->m_arrPoint == nullptr || this->m_polygon == nullptr)
	{
		hatch->m_arrPoint = nullptr;
		hatch->m_polygon = nullptr;
	}
	else
	{
		int nPointSize = this->m_arrPoint->Length;

		if (nPointSize == 0)
		{
			hatch->m_arrPoint = nullptr;
			hatch->m_polygon = nullptr;
		}
		else
		{
			
				hatch->m_arrPoint = gcnew array<System::Drawing::PointF>(nPointSize);
				hatch->m_polygon = gcnew UnE::Geometry::Polygon();

				
				for (int i = 0; i < nPointSize; i++)
				{
					hatch->m_arrPoint[i].X = this->m_arrPoint[i].X;
					hatch->m_arrPoint[i].Y = this->m_arrPoint[i].Y;

					

					UnE::Geometry::Vertex2D^ vertex = this->m_polygon->GetVertex(i);
					hatch->m_polygon->AddVertex(gcnew UnE::Geometry::Vertex2D(vertex->x, vertex->y));
				}
			}
		
	}

	hatch->m_brush = gcnew System::Drawing::SolidBrush(this->m_brush->Color);
	hatch->m_ptCenter = this->m_ptCenter;

	return hatch;
}

// Selectable이 false이면 HitTest 검사가 무조건 실패한다.
bool Hatch::HitTest(double x, double y)
{
	if (!Selectable)
		return false;

	if (m_polygon == nullptr)
		return false;
	
	return m_polygon->HitTest(gcnew Vertex2D(x, y)) != 0;
}

int Hatch::GetPointSize()
{
	if (m_arrPoint == nullptr)
		return 0;

	return m_arrPoint->Length;
}

bool Hatch::GetPoint(int nIndex, [System::Runtime::InteropServices::OutAttribute] float% x, [System::Runtime::InteropServices::OutAttribute] float% y)
{
	x = y = 0.0f;

	if (nIndex >= GetPointSize() || nIndex < 0)
		return false;

	x = m_arrPoint[nIndex].X;
	y = m_arrPoint[nIndex].Y;

	return true;
}

static void DeleteTriangles(std::vector<p2t::Triangle*> triangles, std::vector<p2t::Point*> polyline, std::map<p2t::Point*, unsigned int> mapPointIndex)
{
	for (int i = 0; i < triangles.size(); i++)
	{
		delete triangles[i];
	}

	triangles.clear();

	for (int i = 0; i < polyline.size(); i++)
	{
		delete polyline[i];
	}

	polyline.clear();
	mapPointIndex.clear();
}

END_NS
