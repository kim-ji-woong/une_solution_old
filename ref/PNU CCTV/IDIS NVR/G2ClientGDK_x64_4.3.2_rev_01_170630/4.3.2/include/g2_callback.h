// g2_callback.h : header file
//

#ifndef _G2_CLIENT_DLL_CALLBACK_H_
#define _G2_CLIENT_DLL_CALLBACK_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC G2HCALLBACK G2API g2_callback_initialize(G2UPARAM param);
G2_DLLFUNC void G2API g2_callback_finalize(G2HCALLBACK handle);
G2_DLLFUNC void G2API g2_callback_startup(G2HCALLBACK handle, int connections);
G2_DLLFUNC void G2API g2_callback_cleanup(G2HCALLBACK handle);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_callback_set_connection_info(G2HCALLBACK handle, int i, const wchar_t* address, unsigned short port);
G2_DLLFUNC void G2API g2_callback_set_dvrns_name(G2HCALLBACK handle, const wchar_t* name);
G2_DLLFUNC void G2API g2_callback_set_retry_count(G2HCALLBACK handle, int retry);
G2_DLLFUNC void G2API g2_callback_set_limit_count(G2HCALLBACK handle, int limit);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_callback_notify_event_all(G2HCALLBACK handle, const G2EVENT_INFO* ei, const wchar_t* id, int num);
G2_DLLFUNC bool G2API g2_callback_notify_event(G2HCALLBACK handle, int i, const G2EVENT_INFO* ei, const wchar_t* id, int num);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_callback_is_enable_notify(G2HCALLBACK handle, int i);

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_CLIENT_DLL_CALLBACK_H_
