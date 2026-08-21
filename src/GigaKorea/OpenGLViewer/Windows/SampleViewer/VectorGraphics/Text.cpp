#include "stdafx.h"
#include "Text.h"
#include "VectorCtrl.h"
#include <string>
#include "Layer.h"

namespace VectorGraphics
{
	Text::Text()
	{
		m_dAngle = 0.0;
		m_dFontSize = 10.0;
		m_strFontName = L"¸¼Àº °íµñ";
		m_strContents = L"";
	}


	Text::~Text()
	{
	}

	void Text::Draw()
	{
		Layer* pLayer = GetLayer();

		if (pLayer == 0)
			return;

		VectorCtrl* pCtrl = pLayer->GetControl();

		if (pCtrl == 0)
			return;

		double dViewportWeight = pCtrl->GetViewportWeight();

		int x, y;
		pCtrl->GlobalToScreen(m_vPos, &x, &y);

		HDC hdc = pCtrl->GetHDC();

		int nEscapement = (int)(m_dAngle * 10);
		HFONT hFont = CreateFont((int)(m_dFontSize / dViewportWeight), 0, nEscapement,
			0, 0, 0, 0, 0, HANGUL_CHARSET, 3, 2, 1, VARIABLE_PITCH | FF_ROMAN, m_strFontName.data());
		HGDIOBJ hOldFont = SelectObject(hdc, hFont);

		SetTextColor(hdc, pLayer->GetColor());
		SetBkMode(hdc, TRANSPARENT);
		TextOut(hdc, x, y, m_strContents.c_str(), m_strContents.length());

		SelectObject(hdc, hOldFont);
		DeleteObject(hFont);
	}

	void Text::SetPosition(const Vertex2D& vPos)
	{
		m_vPos = vPos;
	}

	const Vertex2D& Text::GetPosition()
	{
		return m_vPos;
	}

	void Text::SetContents(const std::wstring& str)
	{
		m_strContents = str;
	}

	void Text::SetAngle(double dAngle)
	{
		m_dAngle = dAngle;
	}

	void Text::SetFontName(const std::wstring& strFontName)
	{
		m_strFontName = strFontName;
	}

	void Text::SetFontSize(double dFontSize)
	{
		m_dFontSize = dFontSize;
	}

	const std::wstring& Text::GetContents()
	{
		return m_strContents;
	}

	double Text::GetAngle()
	{
		return m_dAngle;
	}

	const std::wstring& Text::GetFontName()
	{
		return m_strFontName;
	}

	double Text::GetFontSize()
	{
		return m_dFontSize;
	}
}
