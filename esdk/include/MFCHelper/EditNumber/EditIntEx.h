#pragma once

#include "EditInt.h"

class CEditIntEx : public CEditInt
{
	DECLARE_DYNAMIC(CEditIntEx)

public:
	CEditIntEx();
	virtual ~CEditIntEx();

protected:
	DECLARE_MESSAGE_MAP()
	virtual LRESULT WindowProc(UINT message, WPARAM wParam, LPARAM lParam);
};