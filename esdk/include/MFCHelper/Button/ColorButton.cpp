// ColorButton.cpp : 구현 파일입니다.
//

#include "stdafx.h"
#include "ColorButton.h"

// CColorButton

IMPLEMENT_DYNAMIC(CColorButton, CButton)

CColorButton::CColorButton()
{
	m_crColor = RGB(0,0,0);
}

CColorButton::~CColorButton()
{
}


BEGIN_MESSAGE_MAP(CColorButton, CButton)
	ON_WM_PAINT()
	ON_WM_CREATE()
	ON_CONTROL_REFLECT(BN_CLICKED, &CColorButton::OnBnClicked)
END_MESSAGE_MAP()



// CColorButton 메시지 처리기입니다.



void CColorButton::OnPaint()
{
	CPaintDC dc(this); // device context for painting
	// TODO: 여기에 메시지 처리기 코드를 추가합니다.
	// 그리기 메시지에 대해서는 CButton::OnPaint()을(를) 호출하지 마십시오.
	RECT rc;
	GetClientRect(&rc);

	CBrush *pOldBrush, brush(m_crColor);
	CPen *pOldPen, pen(PS_SOLID, 1, RGB(0,0,0));

	pOldBrush = dc.SelectObject(&brush);
	pOldPen = dc.SelectObject(&pen);

	dc.Rectangle(&rc);

	dc.SelectObject(pOldBrush);
	dc.SelectObject(pOldPen);
}

void CColorButton::OnBnClicked()
{
	// TODO: 여기에 컨트롤 알림 처리기 코드를 추가합니다.
	CColorDialog dlg(m_crColor);
	//dlg.SetCurrentColor(m_crColor);

	if (dlg.DoModal() == IDOK)
	{
		COLORREF col = dlg.GetColor();
		if (col == m_crColor) return;

		m_crColor = col;
		Invalidate();
	}
}

void CColorButton::SetColor(COLORREF color)
{
	m_crColor = color;
}

COLORREF CColorButton::GetColor() const
{
	return m_crColor;
}
