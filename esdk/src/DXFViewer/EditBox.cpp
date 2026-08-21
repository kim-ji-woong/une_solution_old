#include "StdAfx.h"
#include "EditBox.h"
#include "IPainter.h"
#include "Shape.h"

BEGIN_NS(DXFViewer)

EditBox::EditBox(IPainter^ owner)
{
	m_pOwner = owner;
	m_arrPoint = gcnew array<System::Drawing::PointF>(4);
}

EditBox::~EditBox(void)
{
}

bool EditBox::Draw(System::Drawing::Graphics^ g, float x, float y)
{
	if (m_pOwner == nullptr)
		return false;

	float fHalfSize = m_pOwner->EditBoxLength / 2;

	m_arrPoint[0].X = x - fHalfSize;
	m_arrPoint[0].Y = y + fHalfSize;
	m_arrPoint[1].X = m_arrPoint[0].X;
	m_arrPoint[1].Y = y - fHalfSize;
	m_arrPoint[2].X = x + fHalfSize;
	m_arrPoint[2].Y = m_arrPoint[1].Y;
	m_arrPoint[3].X = m_arrPoint[2].X;
	m_arrPoint[3].Y = m_arrPoint[0].Y;

	float fLineWidth = Shape::SetScalePenWidth(m_pOwner->EditBoxPen, g);

	g->FillPolygon(m_pOwner->EditBoxBrush, m_arrPoint);
	g->DrawPolygon(m_pOwner->EditBoxPen, m_arrPoint);

	m_pOwner->EditBoxPen->Width = fLineWidth;

	return true;
}

void EditBox::SetOwner(IPainter^ owner)
{
	m_pOwner = owner;
}

END_NS
