// g2_main.h : header file
//

#ifndef _G2_CLIENT_DLL_MAIN_H_
#define _G2_CLIENT_DLL_MAIN_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define_main.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_main_app_initialize(int language);
G2_DLLFUNC void G2API g2_main_app_finalize(void);
G2_DLLFUNC void G2API g2_main_set_language(int language);
G2_DLLFUNC void G2API g2_main_verbose_set_level(int level);
G2_DLLFUNC void G2API g2_main_verbose_callback_invoke(G2VERBOSE_CALLBACK fn);
G2_DLLFUNC void G2API g2_main_verbose_callback_revoke(void);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC int  G2API g2_main_get_language(void);
G2_DLLFUNC int  G2API g2_main_verbose_get_level(void);
G2_DLLFUNC bool G2API g2_main_verbose_is_activate(void);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_main_dvrns_setup(const wchar_t* address, unsigned short port);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_main_date_time_formatter_set_format(int date, int time);
G2_DLLFUNC void G2API g2_main_date_time_formatter_set_string(const wchar_t* am, const wchar_t* pm);
G2_DLLFUNC void G2API g2_main_date_time_formatter_get_string_date(const G2TIME* time, G2STRING_32* res);
G2_DLLFUNC void G2API g2_main_date_time_formatter_get_string_time(const G2TIME* time, G2STRING_32* res);
G2_DLLFUNC void G2API g2_main_date_time_formatter_get_string_date_time(const G2TIME* time, G2STRING_64* res);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_main_get_string(int id, G2STRING_256* str);
G2_DLLFUNC const wchar_t* G2API g2_main_get_string_wchar_t_ptr(int id);

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_CLIENT_DLL_MAIN_H_
