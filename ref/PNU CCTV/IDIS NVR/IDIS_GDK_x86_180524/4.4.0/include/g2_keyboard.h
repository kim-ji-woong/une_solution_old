// g2_keyboard.h : header file
//

#ifndef _G2_SCREEN_KEYBOARD_H_
#define _G2_SCREEN_KEYBOARD_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define.h"

#ifdef __cplusplus
extern "C" {
#endif

///////////////////////////////////////////////////////////////////////////////

typedef void (G2API *G2_SCREEN_KEYBOARD_INIT   )(int, int, int);
typedef int  (G2API *G2_SCREEN_KEYBOARD_DOMODAL)(G2HWND, const TCHAR*, TCHAR*, unsigned int, bool);
typedef void*(G2API *G2_SCREEN_KEYBOARD_BEGIN  )(G2HWND, const TCHAR*, unsigned int, bool);
typedef bool (G2API *G2_SCREEN_KEYBOARD_END    )(void*, TCHAR*, unsigned int);

G2_DLLFUNC void  G2API g2_screen_keyboard_init(int oem, int type, int lang);
G2_DLLFUNC int   G2API g2_screen_keyboard_do_modal(G2HWND parent, const TCHAR* string, TCHAR* buf, unsigned int len, bool english_only);
G2_DLLFUNC void* G2API g2_screen_keyboard_begin(G2HWND parent, const TCHAR* string, unsigned int len, bool english_only);
G2_DLLFUNC bool  G2API g2_screen_keyboard_end(void* handle, TCHAR* buf, unsigned int len);

///////////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_SCREEN_KEYBOARD_H_
