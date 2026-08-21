#include "StdAfx.h"
#include "PolyLine.h"
#include "LineType.h"
#include "EditBox.h"
#include "IPainter.h"

using namespace UnE::Geometry;

BEGIN_NS(DXFViewer)

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
	m_arrPointGL = 0;
	m_brushPolygon = nullptr;
}

PolyLine::~PolyLine(void)
{
	delete[] m_arrPointGL;
	m_arrPointGL = 0;
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
		m_arrPointGL[i * 2] = m_arrPoint[i].X;
		m_arrPointGL[i * 2 + 1] = m_arrPoint[i].Y;

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

bool PolyLine::Draw(System::Drawing::Graphics^ g, bool bDrawText)
{
	if (m_arrPoint == nullptr)
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

bool PolyLine::DrawGDI(System::Drawing::Graphics^ g)
{
	System::Drawing::Pen^ pen = m_lineType->GetPen();
	pen->Color = GetColor();

	if (Selectable && Selected && m_selectedShowingType != SelectedShowingType::NONE)
	{
		if (m_selectedShowingType == SelectedShowingType::EDIT_BOX)
		{
			LineType^ lineType = m_pOwner->GetSelectedLineType();
			System::Drawing::Color penColor = pen->Color;
			pen = lineType->GetPen();
			pen->Color = penColor;

			DrawLines(pen, g);

			m_editBox->Draw(g, (float)m_vTL->x, (float)m_vTL->y);
			m_editBox->Draw(g, (float)m_vTL->x, (float)m_vBR->y);
			m_editBox->Draw(g, (float)m_vBR->x, (float)m_vBR->y);
			m_editBox->Draw(g, (float)m_vBR->x, (float)m_vTL->y);
		}
		else if (m_selectedShowingType == SelectedShowingType::BRIGHT_EFFECT)
		{
			float fOldWidth = pen->Width;

			pen->Width += 1;
			DrawLines(pen, g);

			pen->Width = fOldWidth;

			// 밝게 표현하기 위하여 배경색의 보색으로 그린다.
			DrawLines(GetOwner()->SelectedBrightPen1, g);
			// 패턴을 주기 위하여 배경색으로 다시한번 그린다.
			DrawLines(GetOwner()->SelectedBrightPen2, g);

			if (!m_isClosed)
			{
				g->FillRectangle(System::Drawing::Brushes::White, m_vSelectedBegin->x, m_vSelectedBegin->y, 1.0f, 1.0f);
				g->FillRectangle(System::Drawing::Brushes::White, m_vSelectedEnd->x, m_vSelectedEnd->y, 1.0f, 1.0f);
			}
		}
		else if (m_selectedShowingType == SelectedShowingType::DRAW_POLYGON)
		{
			System::Drawing::Color complementaryColor = GetOwner()->SelectedBrightPen1->Color;
			System::Drawing::Color brushColor = System::Drawing::Color::FromArgb(100, complementaryColor.R, complementaryColor.G, complementaryColor.B);
		
			if (m_brushPolygon == nullptr)
				m_brushPolygon = gcnew System::Drawing::SolidBrush(brushColor);
			else
				m_brushPolygon->Color = brushColor;

			g->FillPolygon(m_brushPolygon, m_arrPoint);
		}
	}
	else
		DrawLines(pen, g);

	return true;
}

void PolyLine::DrawLines(System::Drawing::Pen^ pen, System::Drawing::Graphics^ g)
{
	if (m_arrPoint == nullptr || m_arrPoint->Length < 2)
		return;

	float fLineWidth = SetScalePenWidth(pen, g);
	g->DrawLines(pen, m_arrPoint);
	pen->Width = fLineWidth;
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
		delete[] m_arrPointGL;
		m_arrPointGL = 0;
		return;
	}

	delete[] m_arrPointGL;
	m_arrPointGL = 0;

	int nPointCount = arrVertices->Count;

	if (nPointCount == 0)
	{
		m_arrPoint = nullptr;
		m_polygon = nullptr;
		return;
	}

	m_arrPoint = gcnew array<System::Drawing::PointF>(nPointCount);
	m_polygon = gcnew UnE::Geometry::Polygon();
	m_arrPointGL = new float[nPointCount * 2];

	for (int i=0;i<nPointCount;i++)
	{
		Vertex2D^ vertex = (Vertex2D^)arrVertices[i];

		m_arrPoint[i].X = (float)vertex->x;
		m_arrPoint[i].Y = (float)vertex->y;
		m_arrPointGL[i * 2] = m_arrPoint[i].X;
		m_arrPointGL[i * 2 + 1] = m_arrPoint[i].Y;

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
	m_arrPointGL[nIndex * 2] = x;
	m_arrPointGL[nIndex * 2 + 1] = y;

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

	delete[] m_arrPointGL;
	m_arrPointGL = 0;

	if (nPointCount == 0)
	{
		m_arrPoint = nullptr;
		m_polygon = nullptr;
	}
	else
	{
		m_arrPoint = gcnew array<System::Drawing::PointF>(nPointCount);
		m_polygon = gcnew UnE::Geometry::Polygon();
		m_arrPointGL = new float[nPointCount * 2];

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
	PolyLine^ pLine = gcnew PolyLine();
	pLine->CopyFrom(this);

	if (this->m_arrPoint == nullptr || this->m_polygon == nullptr)
	{
		pLine->m_arrPoint = nullptr;
		pLine->m_polygon = nullptr;
	}
	else
	{
		int nPointCount = this->m_arrPoint->Length;

		delete[] pLine->m_arrPointGL;
		pLine->m_arrPointGL = 0;

		if (nPointCount == 0)
		{
			pLine->m_arrPoint = nullptr;
			pLine->m_polygon = nullptr;
		}
		else
		{
			pLine->m_arrPoint = gcnew array<System::Drawing::PointF>(nPointCount);
			pLine->m_polygon = gcnew UnE::Geometry::Polygon();
			pLine->m_arrPointGL = new float[nPointCount * 2];

			for (int i=0;i<nPointCount;i++)
			{
				pLine->m_arrPoint[i] = this->m_arrPoint[i];
				pLine->m_arrPointGL[i * 2] = pLine->m_arrPoint[i].X;
				pLine->m_arrPointGL[i * 2 + 1] = pLine->m_arrPoint[i].Y;
				
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

bool PolyLine::CheckClipBounds(System::Drawing::Graphics^ g, UnE::Geometry::Vertex2D^ vClipTL, UnE::Geometry::Vertex2D^ vClipBR)
{
	if (m_vTL == nullptr || m_vBR == nullptr)
		return false;

	return CheckClipBounds(vClipTL, vClipBR, m_vTL, m_vBR);
}

END_NS
