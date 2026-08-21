#include "StdAfx.h"
#include "EArc.h"
#include "LineType.h"
#include "Layer.h"
#include "IPainter.h"
#include "EditBox.h"

using namespace UnE::Geometry;

BEGIN_NS(DXFViewer)

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

bool EArc::Draw(System::Drawing::Graphics^ g, bool bDrawText)
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

bool EArc::DrawGDI(System::Drawing::Graphics^ g)
{
	if (m_vTL == nullptr)
		return false;

	System::Drawing::Pen^ pen = m_lineType->GetPen();
	pen->Color = GetColor();

	/*if (Selectable && Selected)
	{
	LineType^ lineType = m_pOwner->GetSelectedLineType();
	System::Drawing::Color penColor = pen->Color;
	pen = lineType->GetPen();
	pen->Color = penColor;
	}*/

	float m11 = 0.0f, m12 = 0.0f, m21 = 0.0f, m22 = 0.0f, dx = 0.0f, dy = 0.0f;

	if (m_dXAxisAngle != 0.0)
	{
		System::Drawing::Drawing2D::Matrix^ matrix = g->Transform;
		m11 = matrix->Elements[0];
		m12 = matrix->Elements[1];
		m21 = matrix->Elements[2];
		m22 = matrix->Elements[3];
		dx = matrix->Elements[4];
		dy = matrix->Elements[5];

		g->TranslateTransform((float)m_vTL->x, (float)m_vTL->y);
		g->RotateTransform((float)m_dXAxisAngle);
		g->TranslateTransform((float)-m_vTL->x, (float)-m_vTL->y);
	}

	if (m_pOwnLayer->Owner->DownToTop())
	{
		g->ScaleTransform(1.0f, -1.0f);
		m_vTL->y = -m_vTL->y;
	}

	if (Selectable && Selected && m_selectedShowingType != SelectedShowingType::NONE)
	{
		if (m_selectedShowingType == SelectedShowingType::EDIT_BOX)
		{
			Draw(g, pen);
			m_editBox->Draw(g, (float)(m_vTL->x + m_dWidth / 2), (float)(m_vTL->y - m_dHeight / 2));
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

	if (m_pOwnLayer->Owner->DownToTop())
	{
		g->ScaleTransform(1.0f, -1.0f);
		m_vTL->y = -m_vTL->y;
	}

	if (m_dXAxisAngle != 0.0)
		g->Transform = gcnew System::Drawing::Drawing2D::Matrix(m11, m12, m21, m22, dx, dy);

	return true;
}

void EArc::Draw(System::Drawing::Graphics^ g, System::Drawing::Pen^ pen)
{
	float fLineWidth = SetScalePenWidth(pen, g);

	if (m_isEllipse)
		g->DrawEllipse(pen, (float)m_vTL->x, (float)m_vTL->y, (float)m_dWidth, (float)m_dHeight);
	else
		g->DrawArc(pen, (float)m_vTL->x, (float)m_vTL->y, (float)m_dWidth, (float)m_dHeight, (float)m_dBeginAngle, (float)m_dEArcAngle);

	pen->Width = fLineWidth;
}

Shape::ShapeType EArc::GetShapeType()
{
	return ShapeType::EARC;
}

Shape^ EArc::Clone()
{
	EArc^ eArc = gcnew EArc();
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
	//return m_vTL->x <= x && m_vTL->y >= y && m_vTL->x + m_dWidth <= x && m_vTL->y - m_dHeight <= y;
}

bool EArc::CheckClipBounds(System::Drawing::Graphics^ g, UnE::Geometry::Vertex2D^ vClipTL, UnE::Geometry::Vertex2D^ vClipBR)
{
	UnE::Geometry::Vertex2D^ vTL = this->BoundaryTL;
	UnE::Geometry::Vertex2D^ vBR = this->BoundaryBR;

	if (vTL == nullptr || vBR == nullptr)
		return false;

	return CheckClipBounds(vClipTL, vClipBR, vTL, vBR);
}

END_NS
