//=============================================================
//
// C4 Engine version 2.9.1
// Copyright 1999-2012, by Terathon Software LLC
//
// This copy is licensed to the following:
//
//     Registered user: Soo Ki Kim
//     Maximum number of users: 1
//     License #C4T0035002
//
// License is granted under terms of the license agreement
// entered by the registed user.
//
// Unauthorized redistribution of source code is strictly
// prohibited. Violators will be prosecuted.
//
//=============================================================


#include "C4Wintab.h"


using namespace C4;


namespace C4
{
	UINT (WINAPI *WTInfoA)(UINT, UINT, void *) = nullptr;
	HCTX (WINAPI *WTOpenA)(HWND, TabletLogContext *, BOOL) = nullptr;
	BOOL (WINAPI *WTClose)(HCTX) = nullptr;
	int (WINAPI *WTPacketsGet)(HCTX, int, void *) = nullptr;
}

// ZYURVUR
