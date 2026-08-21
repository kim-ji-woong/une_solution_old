#include "StdAfx.h"
#include "LineType.h"
#include "DXFControl.h"

BEGIN_NS(DXFDotNet)

LineType::LineType(DXFControl^ ctrl)
{
	m_lineStyle = System::Drawing::Drawing2D::DashStyle::Solid;
	m_nLineWidth = 1;
	m_strLineTypeName = L"Continuous";
	m_ctrl = ctrl;

	SetLineType(m_lineStyle, m_nLineWidth);
}

LineType::~LineType(void)
{
}

LineType::LineType(DXFControl^ ctrl, System::Drawing::Drawing2D::DashStyle lineStyle, int nLineWidth)
{
	m_ctrl = ctrl;
	SetLineType(lineStyle, nLineWidth);
}

System::Drawing::Pen^ LineType::GetPen()
{
	return m_pen;
}

void LineType::SetLineType(System::Drawing::Drawing2D::DashStyle lineStyle, int nLineWidth)
{
	if (m_ctrl == nullptr)
		return;

	System::Collections::Generic::Dictionary<__int64, System::Drawing::Pen^>^ dicPens = m_ctrl->GetLineTypePen();

	__int64 nStyle = (__int64)lineStyle;
	__int64 nWidth = (__int64)nLineWidth;

	m_lineStyle = lineStyle;
	m_nLineWidth = nLineWidth;

	__int64 nKey = (nStyle << 32) | nWidth;

	if (dicPens->ContainsKey(nKey))
		m_pen = dicPens[nKey];
	else
	{
		m_pen = gcnew System::Drawing::Pen(System::Drawing::Color::Black);
		m_pen->Width = (float)nLineWidth;
		m_pen->DashStyle = lineStyle;

		dicPens[nKey] = m_pen;
	}
}

System::Drawing::Drawing2D::DashStyle LineType::GetLineStyle()
{
	return m_lineStyle;
}

int LineType::GetLineWidth()
{
	return m_nLineWidth;
}

END_NS
