#include "stdafx.h"
#include "EditIntEx.h"

IMPLEMENT_DYNAMIC(CEditIntEx, CEditInt)

CEditIntEx::CEditIntEx()
{
}

CEditIntEx::~CEditIntEx()
{
}


BEGIN_MESSAGE_MAP(CEditIntEx, CEditInt)
END_MESSAGE_MAP()

LRESULT CEditIntEx::WindowProc(UINT message, WPARAM wParam, LPARAM lParam)
{
	if (message == WM_KEYDOWN || message == WM_SYSKEYDOWN || message == WM_GETDLGCODE)
	{
		if (wParam == VK_RETURN)
		{
			return SendMessage(WM_KILLFOCUS);
		}
	}

	return CEditInt::WindowProc(message, wParam, lParam);
}