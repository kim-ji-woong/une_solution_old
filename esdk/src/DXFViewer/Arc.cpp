#include "StdAfx.h"
#include "Arc.h"
#include "LineType.h"
#include "EditBox.h"
#include "IPainter.h"

using namespace UnE::Geometry;

BEGIN_NS(DXFViewer)

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

bool Arc::Draw(System::Drawing::Graphics^ g, bool bDrawText)
{
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

bool Arc::DrawGDI(System::Drawing::Graphics^ g)
{
	System::Drawing::Pen^ pen = m_lineType->GetPen();
	pen->Color = GetColor();

	/*if (Selectable && Selected)
	{
	LineType^ lineType = m_pOwner->GetSelectedLineType();
	System::Drawing::Color penColor = pen->Color;
	pen = lineType->GetPen();
	pen->Color = penColor;
	}*/

	if (Selectable && Selected && m_selectedShowingType != SelectedShowingType::NONE)
	{
		if (m_selectedShowingType == SelectedShowingType::EDIT_BOX)
		{
			Draw(g, pen);
			m_editBox->Draw(g, (float)m_vCenter->x, (float)m_vCenter->y);
		}
		else if (m_selectedShowingType == SelectedShowingType::BRIGHT_EFFECT)
		{
			float fOldWidth = pen->Width;

			pen->Width += 1;
			Draw(g, pen);

			pen->Width = fOldWidth;

			// 밝게 표현하기 위하여 배경색의 보색으로 그린다.
			Draw(g, GetOwner()->SelectedBrightPen1);
			// 패턴을 주기 위하여 배경색으로 다시한번 그린다.
			Draw(g, GetOwner()->SelectedBrightPen2);
		}
	}
	else
		Draw(g, pen);

	return true;
}

void Arc::Draw(System::Drawing::Graphics^ g, System::Drawing::Pen^ pen)
{
	float fLineWidth = SetScalePenWidth(pen, g);

	if (!m_isCircle)
		g->DrawArc(pen, (float)(m_vCenter->x - m_dRadius), (float)(m_vCenter->y - m_dRadius), (float)(m_dRadius * 2), (float)(m_dRadius * 2), (float)m_dBeginAngle, (float)m_dArcAngle);
	else
		g->DrawEllipse(pen, (float)(m_vCenter->x - m_dRadius), (float)(m_vCenter->y - m_dRadius), (float)(m_dRadius * 2), (float)(m_dRadius * 2));

	pen->Width = fLineWidth;
}

Shape::ShapeType Arc::GetShapeType()
{
	return ShapeType::ARC;
}

Shape^ Arc::Clone()
{
	Arc^ arc = gcnew Arc();
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

bool Arc::CheckClipBounds(System::Drawing::Graphics^ g, UnE::Geometry::Vertex2D^ vClipTL, UnE::Geometry::Vertex2D^ vClipBR)
{
	if (m_vCenter == nullptr)
		return false;

	UnE::Geometry::Vertex2D^ vTL = gcnew Vertex2D(m_vCenter->x - m_dRadius, m_vCenter->y + m_dRadius);
	UnE::Geometry::Vertex2D^ vBR = gcnew Vertex2D(m_vCenter->x + m_dRadius, m_vCenter->y - m_dRadius);

	return CheckClipBounds(vClipTL, vClipBR, vTL, vBR);
}

END_NS
