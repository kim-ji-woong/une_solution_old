#pragma once

// CColorButton

class CColorButton : public CButton
{
	DECLARE_DYNAMIC(CColorButton)

public:
	CColorButton();
	virtual ~CColorButton();

public:
	void SetColor(COLORREF color);
	COLORREF GetColor() const;

protected:
	DECLARE_MESSAGE_MAP()

protected:
	COLORREF m_crColor;

public:
	afx_msg void OnPaint();
	afx_msg void OnBnClicked();
};


