#include "StdAfx.h"
#include "Line.h"
#include "LineType.h"
#include "IPainter.h"
#include "EditBox.h"

using namespace UnE::Geometry;

BEGIN_NS(DXFViewer)

Line::Line(void)
{
	m_arrPoint = gcnew array<System::Drawing::PointF>(2);
	//m_arrPointGL = new float[4];

	Begin = gcnew Vertex2D();
	End = gcnew Vertex2D();
	m_vSelectedBegin = gcnew Vertex2F();
	m_vSelectedEnd = gcnew Vertex2F();
}

Line::~Line(void)
{
	//delete [] m_arrPointGL;
}

Line::Line(Line^ rhs)
{
	m_arrPoint = gcnew array<System::Drawing::PointF>(2);
	//m_arrPointGL = new float[4];

	Begin = gcnew Vertex2D(rhs->m_vBegin->x, rhs->m_vBegin->y);
	End = gcnew Vertex2D(rhs->m_vEnd->x, rhs->m_vEnd->y);
	m_vSelectedBegin = gcnew Vertex2F(rhs->m_vSelectedBegin->x, rhs->m_vSelectedBegin->y);
	m_vSelectedEnd = gcnew Vertex2F(rhs->m_vSelectedEnd->x, rhs->m_vSelectedEnd->y);
}

Line::Line(UnE::Geometry::Vertex2D^ vBegin, UnE::Geometry::Vertex2D^ vEnd)
{
	m_arrPoint = gcnew array<System::Drawing::PointF>(2);
	//m_arrPointGL = new float[4];

	Begin = gcnew Vertex2D(vBegin->x, vBegin->y);
	End = gcnew Vertex2D(vEnd->x, vEnd->y);

	Vertex2D^ v1 = UnE::Geometry::Math::GetRightVertex(vBegin, vEnd, -0.5);
	Vertex2D^ v2 = UnE::Geometry::Math::GetRightVertex(vEnd, vBegin, 0.5);
	m_vSelectedBegin = gcnew Vertex2F((float)v1->x, (float)v1->y);
	m_vSelectedEnd = gcnew Vertex2F((float)v2->x, (float)v2->y);
}

// (x,y)만큼 객체를 옮긴다.
void Line::Move(double x, double y)
{
	m_vBegin->x += x;
	m_vBegin->y += y;
	m_vEnd->x += x;
	m_vEnd->y += y;

	m_arrPoint[0].X = /*m_arrPointGL[0] = */(float)m_vBegin->x;
	m_arrPoint[0].Y = /*m_arrPointGL[1] = */(float)m_vBegin->y;
	m_arrPoint[1].X = /*m_arrPointGL[2] = */(float)m_vEnd->x;
	m_arrPoint[1].Y = /*m_arrPointGL[3] = */(float)m_vEnd->y;
	
	Vertex2D^ v1 = UnE::Geometry::Math::GetRightVertex(m_vBegin, m_vEnd, -0.5);
	Vertex2D^ v2 = UnE::Geometry::Math::GetRightVertex(m_vEnd, m_vBegin, 0.5);
	m_vSelectedBegin->SetVertex((float)v1->x, (float)v1->y);
	m_vSelectedEnd->SetVertex((float)v2->x, (float)v2->y);
}

bool Line::Draw(System::Drawing::Graphics^ g, bool bDrawText)
{
	if (m_vBegin == nullptr || m_vEnd == nullptr || m_pOwner == nullptr)
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

	/*System::Drawing::Color color = GetColor();
	glColor3f(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f);
	glLineWidth(m_lineType->GetLineWidth());

	glBegin(GL_LINES);
		glVertex2f(m_arrPoint[0].X, m_arrPoint[0].Y);
		glVertex2f(m_arrPoint[1].X, m_arrPoint[1].Y);
	glEnd();*/

	/*System::Drawing::Pen^ pen = m_lineType->GetPen();
	pen->Color = GetColor();

	if (Selectable && Selected && m_selectedShowingType != SelectedShowingType::NONE)
	{
		if (m_selectedShowingType == SelectedShowingType::EDIT_BOX)
		{
			LineType^ lineType = m_pOwner->GetSelectedLineType();
			System::Drawing::Color penColor = pen->Color;
			pen = lineType->GetPen();
			pen->Color = penColor;

			g->DrawLines(pen, m_arrPoint);

			m_editBox->Draw(g, m_arrPoint[0].X, m_arrPoint[0].Y);
			m_editBox->Draw(g, m_arrPoint[1].X, m_arrPoint[1].Y);
		}
		else if (m_selectedShowingType == SelectedShowingType::BRIGHT_EFFECT)
		{
			float fOldWidth = pen->Width;

			pen->Width += 1;
			g->DrawLines(pen, m_arrPoint);

			pen->Width = fOldWidth;
			
			// 밝게 표현하기 위하여 배경색의 보색으로 그린다.
			g->DrawLines(GetOwner()->SelectedBrightPen1, m_arrPoint);
			// 패턴을 주기 위하여 배경색으로 다시한번 그린다.
			g->DrawLines(GetOwner()->SelectedBrightPen2, m_arrPoint);

			g->FillRectangle(System::Drawing::Brushes::White, m_vSelectedBegin->x, m_vSelectedBegin->y, 1.0f, 1.0f);
			g->FillRectangle(System::Drawing::Brushes::White, m_vSelectedEnd->x, m_vSelectedEnd->y, 1.0f, 1.0f);
		}
	}
	else
		g->DrawLines(pen, m_arrPoint);

	return true;*/
}

bool Line::DrawGDI(System::Drawing::Graphics^ g)
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

			m_editBox->Draw(g, m_arrPoint[0].X, m_arrPoint[0].Y);
			m_editBox->Draw(g, m_arrPoint[1].X, m_arrPoint[1].Y);
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

			g->FillRectangle(System::Drawing::Brushes::White, m_vSelectedBegin->x, m_vSelectedBegin->y, 1.0f, 1.0f);
			g->FillRectangle(System::Drawing::Brushes::White, m_vSelectedEnd->x, m_vSelectedEnd->y, 1.0f, 1.0f);
		}
	}
	else
		DrawLines(pen, g);

	return true;
}

void Line::DrawLines(System::Drawing::Pen^ pen, System::Drawing::Graphics^ g)
{
	float fLineWidth = SetScalePenWidth(pen, g);
	g->DrawLines(pen, m_arrPoint);
	pen->Width = fLineWidth;
}

Shape::ShapeType Line::GetShapeType()
{
	return ShapeType::LINE;
}

Shape^ Line::Clone()
{
	Line^ line = gcnew Line();
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

bool Line::CheckClipBounds(System::Drawing::Graphics^ g, UnE::Geometry::Vertex2D^ vClipTL, UnE::Geometry::Vertex2D^ vClipBR)
{
	if (m_vBegin == nullptr || m_vEnd == nullptr)
		return false;

	UnE::Geometry::Vertex2D^ vTL = gcnew UnE::Geometry::Vertex2D();
	UnE::Geometry::Vertex2D^ vBR = gcnew UnE::Geometry::Vertex2D();

	if (m_vBegin->x < m_vEnd->x)
	{
		vTL->x = m_vBegin->x;
		vBR->x = m_vEnd->x;
	}
	else
	{
		vTL->x = m_vEnd->x;
		vBR->x = m_vBegin->x;
	}

	if (m_vBegin->y < m_vEnd->y)
	{
		vTL->y = m_vEnd->y;
		vBR->y = m_vBegin->y;
	}
	else
	{
		vTL->y = m_vBegin->y;
		vBR->y = m_vEnd->y;
	}

	if (vTL == nullptr || vBR == nullptr)
		return false;

	return CheckClipBounds(vClipTL, vClipBR, vTL, vBR);
}

END_NS
