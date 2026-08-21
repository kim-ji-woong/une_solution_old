#include "StdAfx.h"
#include "Hatch.h"
#include "LineType.h"
#include "EditBox.h"
#include "IPainter.h"
#include "poly2tri/sweep/cdt.h"
#include <map>

using namespace UnE::Geometry;

BEGIN_NS(DXFViewer)

void Hatch::PathItem::SetLine(UnE::Geometry::Vertex2D^ vBegin, UnE::Geometry::Vertex2D^ vEnd)
{
	m_line = gcnew Line2D(vBegin, vEnd);
	m_drawType = DrawType::Line;
}

void Hatch::PathItem::SetLine(UnE::Geometry::Line2D^ line)
{
	m_line = line;
	m_drawType = DrawType::Line;
}

void Hatch::PathItem::SetArc(UnE::Geometry::Arc2D^ arc)
{
	m_arc = arc;
	m_drawType = DrawType::Arc;
}

void Hatch::PathItem::SetEArc(UnE::Geometry::EArc2D^ earc)
{
	m_earc = earc;
	m_drawType = DrawType::EArc;
}

Hatch::Hatch(void)
{
	//m_arrPoint = nullptr;
	m_polygon = gcnew Polygon();
	m_vTL = gcnew UnE::Geometry::Vertex2D();
	m_vBR = gcnew UnE::Geometry::Vertex2D();
	m_isInitArea = false;
	m_brush = gcnew System::Drawing::SolidBrush(System::Drawing::Color::Black);

	//m_arrPointGL = 0;
	//m_arrIndex = 0;
	//m_nIndexCount = 0;
	m_dMoveX = m_dMoveY = 0.0;
}

Hatch::~Hatch(void)
{
	//ClearGLBuffer();
}

// (x,y)만큼 객체를 옮긴다.
void Hatch::Move(double x, double y)
{
	//m_ptCenter.X += (float)x;
	//m_ptCenter.Y += (float)y;

	MakePath(x, y);
	
	/*if (m_arrPoint == nullptr)
		return;

	int nPointCount = m_arrPoint->Length;

	for (int i=0;i<nPointCount;i++)
	{
		m_arrPoint[i].X += (float)x;
		m_arrPoint[i].Y += (float)y;

		if (m_arrPointGL != 0)
		{
			m_arrPointGL[i * 2] = m_arrPoint[i].X;
			m_arrPointGL[i * 2 + 1] = m_arrPoint[i].Y;
		}

		Vertex2D^ vertex = m_polygon->GetVertex(i);
		vertex->x += x;
		vertex->y += y;
	}*/

	/*if (m_isInitArea)
	{
		m_vTL->x += x;
		m_vTL->y += y;
		m_vBR->x += x;
		m_vBR->y += y;
	}*/

	m_dMoveX = x;
	m_dMoveY = y;
}

static void SetBoundaryVertex(UnE::Geometry::Vertex2D^% vTL, UnE::Geometry::Vertex2D^% vBR, UnE::Geometry::Vertex2D^ vertex)
{
	if (vTL == nullptr)
	{
		vTL = gcnew UnE::Geometry::Vertex2D(vertex->x, vertex->y);
		vBR = gcnew UnE::Geometry::Vertex2D(vertex->x, vertex->y);
	}
	else
	{
		if (vTL->x > vertex->x)
			vTL->x = vertex->x;
		if (vBR->x < vertex->x)
			vBR->x = vertex->x;
		if (vTL->y < vertex->y)
			vTL->y = vertex->y;
		if (vBR->y > vertex->y)
			vBR->y = vertex->y;
	}
}

void Hatch::AddPath(System::Drawing::Drawing2D::GraphicsPath^ path, Hatch::PathItem^ item, double x, double y)
{
	if (item->DrawingType == PathItem::DrawType::Line)
	{
		UnE::Geometry::Vertex2D^ vBegin = item->Line->GetVertex(true);
		UnE::Geometry::Vertex2D^ vEnd = item->Line->GetVertex(false);
		vBegin->SetVertex(vBegin->x + x, vBegin->y + y);
		vEnd->SetVertex(vEnd->x + x, vEnd->y + y);

		System::Drawing::PointF ptBegin((float)vBegin->x, (float)vBegin->y);
		System::Drawing::PointF ptEnd((float)vEnd->x, (float)vEnd->y);

		path->AddLine(ptBegin, ptEnd);

		m_polygon->AddVertex(vEnd);

		SetBoundaryVertex(m_vTL, m_vBR, vBegin);
		SetBoundaryVertex(m_vTL, m_vBR, vEnd);
	}
	else if (item->DrawingType == PathItem::DrawType::Arc || item->DrawingType == PathItem::DrawType::EArc)
	{
		EArc2D^ earc = nullptr;
		
		if (item->DrawingType == PathItem::DrawType::Arc)
		{
			if (item->Arc == nullptr)
				return;

			Vertex2D^ vCenter = gcnew Vertex2D(item->Arc->GetCenter()->x + x, item->Arc->GetCenter()->y + y);
			item->Arc->SetArc(vCenter, item->Arc->GetRadius(), item->Arc->GetBeginAngle(), item->Arc->GetAngle(), item->Arc->IsClockWise());
			earc = item->Arc;
		}
		else
		{
			if (item->EArc == nullptr)
				return;

			UnE::Geometry::Vertex2D^ vTL = gcnew UnE::Geometry::Vertex2D(item->EArc->GetTL()->x + x, item->EArc->GetTL()->y + y);
			UnE::Geometry::Vertex2D^ vBL = gcnew UnE::Geometry::Vertex2D(item->EArc->GetBL()->x + x, item->EArc->GetBL()->y + y);
			UnE::Geometry::Vertex2D^ vBR = gcnew UnE::Geometry::Vertex2D(item->EArc->GetBR()->x + x, item->EArc->GetBR()->y + y);

			item->EArc->SetEArc(vTL, vBL, vBR, item->EArc->GetBeginAngle(), item->EArc->GetAngle(), item->EArc->IsClockWise());
			earc = item->EArc;
		}

		if (earc != nullptr)
		{
			UnE::Geometry::Vertex2D^ vTL = earc->GetTL();
			UnE::Geometry::Vertex2D^ vBL = earc->GetBL();
			UnE::Geometry::Vertex2D^ vBR = earc->GetBR();

			System::Drawing::RectangleF rect((float)vBL->x, (float)vBL->y, (float)vBL->GetDistance(vBR), (float)vBL->GetDistance(vTL));

			// Degree
			float fBeginAngle = (float)UnE::Geometry::Math::RadToDeg(earc->GetBeginAngle());
			float fEArcAngle = (float)UnE::Geometry::Math::RadToDeg(earc->GetAngle());

			if (earc->IsClockWise())
				fEArcAngle = -fEArcAngle;

			path->AddArc(rect, fBeginAngle, fEArcAngle);

			UnE::Geometry::Vertex2D^ vBegin = earc->GetBeginVertex();
			UnE::Geometry::Vertex2D^ vEnd = earc->GetEndVertex();
			UnE::Geometry::Vertex2D^ vMiddle;

			if (earc->GetVertex(earc->GetBeginAngle() + earc->GetAngle() / 2, vMiddle) == false)
				return;

			// m_polygon에는 근사적으로 EArc의 중간점만 삽입한다.
			// 정밀한 HitTest가 필요하면 보간하여야 한다.
			m_polygon->AddVertex(vMiddle);
			m_polygon->AddVertex(vEnd);

			SetBoundaryVertex(m_vTL, m_vBR, vBegin);
			SetBoundaryVertex(m_vTL, m_vBR, vMiddle);
			SetBoundaryVertex(m_vTL, m_vBR, vEnd);
		}
	}
}

void Hatch::AddLine(UnE::Geometry::Vertex2D^ vBegin, UnE::Geometry::Vertex2D^ vEnd)
{
	PathItem^ item = gcnew PathItem();
	item->SetLine(vBegin, vEnd);
	m_pathItems->Add(item);
}

void Hatch::AddArc(UnE::Geometry::Arc2D^ arc)
{
	PathItem^ item = gcnew PathItem();
	item->SetArc(arc);
	m_pathItems->Add(item);
}

void Hatch::AddEArc(UnE::Geometry::EArc2D^ earc)
{
	PathItem^ item = gcnew PathItem();
	item->SetEArc(earc);
	m_pathItems->Add(item);
}

bool Hatch::Draw(System::Drawing::Graphics^ g, bool bDrawText)
{
	//if (m_arrPoint == nullptr)
	//	return false;

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

bool Hatch::DrawGDI(System::Drawing::Graphics^ g)
{
	if (m_path == nullptr)
		return true;

	m_brush->Color = GetColor();

	g->FillPath(m_brush, m_path);
	//g->FillPolygon(m_brush, m_arrPoint);

	if (Selectable && Selected)
	{
		if (m_selectedShowingType == SelectedShowingType::EDIT_BOX)
			m_editBox->Draw(g, m_ptCenter.X, m_ptCenter.Y);
		else if (m_selectedShowingType == SelectedShowingType::BRIGHT_EFFECT ||
			m_selectedShowingType == SelectedShowingType::DRAW_POLYGON)
		{
			System::Drawing::Color oldColor = m_brush->Color;
			m_brush->Color = System::Drawing::Color::FromArgb(100, 255 - oldColor.R, 255 - oldColor.G, 255 - oldColor.B);
			g->FillPath(m_brush, m_path);
			//g->FillPolygon(m_brush, m_arrPoint);
			m_brush->Color = oldColor;
		}
	}

	return true;
}

Shape::ShapeType Hatch::GetShapeType()
{
	return ShapeType::HATCH;
}

/*void Hatch::SetVertex(System::Collections::ArrayList^ arrVertices)
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
		ClearGLBuffer();
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

	CalcGLBuffer();
}

bool Hatch::UpdatePoint(int nIndex, float x, float y)
{
	if (nIndex < 0 || m_arrPoint->Length <= nIndex)
		return false;

	m_arrPoint[nIndex].X = x;
	m_arrPoint[nIndex].Y = y;

	ClearGLBuffer();

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
		ClearGLBuffer();
	}
	else
	{
		m_arrPoint = gcnew array<System::Drawing::PointF>(nPointCount);
		m_polygon = gcnew UnE::Geometry::Polygon();

		for (int i=0;i<nPointCount;i++)
			m_polygon->AddVertex(gcnew UnE::Geometry::Vertex2D());
	}
}*/

Shape^ Hatch::Clone()
{
	Hatch^ hatch = gcnew Hatch();
	hatch->CopyFrom(this);

	if (m_pathItems->Count > 0)
	{
		for each (PathItem^ item in m_pathItems)
		{
			hatch->m_pathItems->Add(item);
		}

		hatch->MakePath(m_dMoveX, m_dMoveY);
	}

	/*if (this->m_arrPoint == nullptr || this->m_polygon == nullptr)
	{
		hatch->m_arrPoint = nullptr;
		hatch->m_polygon = nullptr;
		hatch->ClearGLBuffer();
	}
	else
	{
		int nPointSize = this->m_arrPoint->Length;

		if (nPointSize == 0)
		{
			hatch->m_arrPoint = nullptr;
			hatch->m_polygon = nullptr;
			hatch->ClearGLBuffer();
		}
		else
		{
			//if (this->m_nIndexCount > 0)
			{
				hatch->m_arrPoint = gcnew array<System::Drawing::PointF>(nPointSize);
				hatch->m_polygon = gcnew UnE::Geometry::Polygon();

				//hatch->ClearGLBuffer();

				//hatch->m_arrPointGL = new float[nPointSize];
				//hatch->m_arrIndex = new unsigned int[this->m_nIndexCount];
				//hatch->m_nIndexCount = this->m_nIndexCount;

				for (int i = 0; i < nPointSize; i++)
				{
					hatch->m_arrPoint[i].X = this->m_arrPoint[i].X;
					hatch->m_arrPoint[i].Y = this->m_arrPoint[i].Y;

					UnE::Geometry::Vertex2D^ vertex = this->m_polygon->GetVertex(i);
					hatch->m_polygon->AddVertex(gcnew UnE::Geometry::Vertex2D(vertex->x, vertex->y));
				}
			}
		}
	}*/

	hatch->m_brush = gcnew System::Drawing::SolidBrush(this->m_brush->Color);
	//hatch->m_ptCenter = this->m_ptCenter;

	return hatch;
}

void Hatch::MakePath(double x, double y)
{
	m_polygon->Clear();
	m_path = gcnew System::Drawing::Drawing2D::GraphicsPath();

	m_vTL = m_vBR = nullptr;

	for each (PathItem^ item in m_pathItems)
	{
		AddPath(m_path, item, x, y);
	}

	if (m_vTL == nullptr)
	{
		m_vTL = gcnew UnE::Geometry::Vertex2D();
		m_vBR = gcnew UnE::Geometry::Vertex2D();
		m_ptCenter.X = 0.0f;
		m_ptCenter.Y = 0.0f;
	}
	else
	{
		m_ptCenter.X = (float)((m_vTL->x + m_vBR->x) / 2);
		m_ptCenter.Y = (float)((m_vTL->y + m_vBR->y) / 2);
	}
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

/*int Hatch::GetPointSize()
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
}*/

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

/*void Hatch::CalcGLBuffer()
{
	if (m_pOwner == nullptr || m_pOwner->Renderer != IPainter::RendererType::OPEN_GL)
		return;

	std::vector<p2t::Point*> polyline;
	std::map<p2t::Point*, unsigned int> mapPointIndex;

	for (int i = 0; i< m_arrPoint->Length; ++i)
	{
		p2t::Point* pt = new p2t::Point(m_arrPoint[i].X, m_arrPoint[i].Y);
		polyline.push_back(pt);

		mapPointIndex[pt] = (unsigned int)i;
	}

	p2t::CDT* cdt = new p2t::CDT(polyline);
	cdt->Triangulate();

	ClearGLBuffer();

	std::vector<p2t::Triangle*> triangles = cdt->GetTriangles();
	int nTriangleSize = (int)triangles.size();

	if (nTriangleSize == 0)
	{
		int nPointCount = polyline.size();

		for (int i = 0; i < nPointCount; i++)
		{
			delete polyline[i];
		}

		polyline.clear();
		return;
	}

	m_arrPointGL = new float[m_arrPoint->Length * 2];

	for (int i = 0; i< m_arrPoint->Length; ++i)
	{
		m_arrPointGL[i * 2] = m_arrPoint[i].X;
		m_arrPointGL[i * 2 + 1] = m_arrPoint[i].Y;
	}

	m_nIndexCount = nTriangleSize * 3;
	m_arrIndex = new unsigned int[m_nIndexCount];

	for (int i = 0; i < nTriangleSize; i++)
	{
		p2t::Triangle* tri = triangles[i];

		p2t::Point* p1 = tri->GetPoint(0);
		p2t::Point* p2 = tri->GetPoint(1);
		p2t::Point* p3 = tri->GetPoint(2);

		std::map<p2t::Point*, unsigned int>::iterator iter1 = mapPointIndex.find(p1);
		std::map<p2t::Point*, unsigned int>::iterator iter2 = mapPointIndex.find(p2);
		std::map<p2t::Point*, unsigned int>::iterator iter3 = mapPointIndex.find(p3);

		if (iter1 == mapPointIndex.end() ||
			iter2 == mapPointIndex.end() ||
			iter3 == mapPointIndex.end())
		{
			DeleteTriangles(triangles, polyline, mapPointIndex);
			ClearGLBuffer();			
			return;
		}

		m_arrIndex[i * 3] = iter1->second;
		m_arrIndex[i * 3 + 1] = iter2->second;
		m_arrIndex[i * 3 + 2] = iter3->second;
	}

	DeleteTriangles(triangles, polyline, mapPointIndex);
}

void Hatch::ClearGLBuffer()
{
	if (m_arrPointGL != 0)
	{
		delete[] m_arrPointGL;
		m_arrPointGL = 0;
	}

	if (m_arrIndex != 0)
	{
		delete[] m_arrIndex;
		m_arrIndex = 0;
	}

	m_nIndexCount = 0;
}

float* Hatch::GetVertexArray()
{
	return m_arrPointGL;
}

unsigned int* Hatch::GetIndexArray()
{
	return m_arrIndex;
}

int Hatch::GetIndexArrayCount()
{
	return m_nIndexCount;
}*/

bool Hatch::CheckClipBounds(System::Drawing::Graphics^ g, UnE::Geometry::Vertex2D^ vClipTL, UnE::Geometry::Vertex2D^ vClipBR)
{
	UnE::Geometry::Vertex2D^ vTL = this->BoundaryTL;
	UnE::Geometry::Vertex2D^ vBR = this->BoundaryBR;

	if (vTL == nullptr || vBR == nullptr)
		return false;

	return CheckClipBounds(vClipTL, vClipBR, vTL, vBR);
}

END_NS
