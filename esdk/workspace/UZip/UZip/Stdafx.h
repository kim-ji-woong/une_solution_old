// stdafx.h : 자주 사용하지만 자주 변경되지는 않는
// 표준 시스템 포함 파일 및 프로젝트 관련 포함 파일이
// 들어 있는 포함 파일입니다.

#pragma once



#ifndef _SECURE_ATL
#define _SECURE_ATL 1
#endif

#ifndef VC_EXTRALEAN
#define VC_EXTRALEAN          
#endif



#define _ATL_CSTRING_EXPLICIT_CONSTRUCTORS    
#define _AFX_ALL_WARNINGS

#include <afxwin.h>       
#include <afxext.h>        
#include <afxdisp.h>



#ifndef _AFX_NO_OLE_SUPPORT
#include <afxdtctl.h>  
#endif
#ifndef _AFX_NO_AFXCMN_SUPPORT
#include <afxcmn.h>             
#endif // _AFX_NO_AFXCMN_SUPPORT

#include <afxcontrolbars.h> 