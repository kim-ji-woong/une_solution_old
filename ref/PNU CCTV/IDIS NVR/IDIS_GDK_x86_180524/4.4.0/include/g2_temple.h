// g2_temple.h : header file
//

#ifndef _G2_CLIENT_DLL_TEMPLE_H_
#define _G2_CLIENT_DLL_TEMPLE_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define_temple.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC int G2API g2_temple_pump_message(G2HWND handle, unsigned int filter_min, unsigned int filter_max, unsigned int flags);
G2_DLLFUNC int G2API g2_temple_pump_message_def(void);

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_CLIENT_DLL_TEMPLE_H_
