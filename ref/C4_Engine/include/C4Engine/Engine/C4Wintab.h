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


#ifndef C4Wintab_h
#define C4Wintab_h


#include "C4Memory.h"


namespace C4
{
	#define WTI_DEFSYSCTX			4
	#define WTI_DEVICES				100
	#define DVC_NPRESSURE			15
	#define WT_DEFBASE				0x7FF0
	#define WT_PACKET				WT_DEFBASE
	#define WT_PROXIMITY			(WT_DEFBASE + 5)
	#define CXO_SYSTEM				0x0001
	#define CXO_MESSAGES			0x0004
	#define PK_NORMAL_PRESSURE		0x0400
	#define PK_ORIENTATION			0x1000
	
	typedef DWORD WTPKT;
	typedef DWORD FIX32;
	
	DECLARE_HANDLE(HCTX);
	
	struct TabletAxis
	{
		LONG		axMin;
		LONG		axMax;
		UINT		axUnits;
		FIX32		axResolution;
	};
	
	struct TabletLogContext
	{
		char		lcName[40];
		UINT		lcOptions;
		UINT		lcStatus;
		UINT		lcLocks;
		UINT		lcMsgBase;
		UINT		lcDevice;
		UINT		lcPktRate;
		WTPKT		lcPktData;
		WTPKT		lcPktMode;
		WTPKT		lcMoveMask;
		DWORD		lcBtnDnMask;
		DWORD		lcBtnUpMask;
		LONG		lcInOrgX;
		LONG		lcInOrgY;
		LONG		lcInOrgZ;
		LONG		lcInExtX;
		LONG		lcInExtY;
		LONG		lcInExtZ;
		LONG		lcOutOrgX;
		LONG		lcOutOrgY;
		LONG		lcOutOrgZ;
		LONG		lcOutExtX;
		LONG		lcOutExtY;
		LONG		lcOutExtZ;
		FIX32		lcSensX;
		FIX32		lcSensY;
		FIX32		lcSensZ;
		BOOL		lcSysMode;
		int			lcSysOrgX;
		int			lcSysOrgY;
		int			lcSysExtX;
		int			lcSysExtY;
		FIX32		lcSysSensX;
		FIX32		lcSysSensY;
	};
	
	struct TabletPacket
	{
		unsigned_int32	normalPressure;
	};
	
	extern UINT (WINAPI *WTInfoA)(UINT, UINT, void *);
	extern HCTX (WINAPI *WTOpenA)(HWND, TabletLogContext *, BOOL);
	extern BOOL (WINAPI *WTClose)(HCTX);
	extern int (WINAPI *WTPacketsGet)(HCTX, int, void *);
}


#endif

// ZYURVUR
