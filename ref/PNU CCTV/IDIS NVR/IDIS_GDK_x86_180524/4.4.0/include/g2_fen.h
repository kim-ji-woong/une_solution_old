// g2_fen.h : header file
//

#ifndef _G2_CLIENT_DLL_FEN_H_
#define _G2_CLIENT_DLL_FEN_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define_fen.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_fen_set_UPARAM(G2UPARAM param);
G2_DLLFUNC void G2API g2_fen_register_callback(unsigned int type, G2FUN_LISTENER func);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_fen_startup(unsigned int connections, unsigned int count_direct, unsigned int count_hole_punching, unsigned int count_relay, const wchar_t* path_history);
G2_DLLFUNC void G2API g2_fen_cleanup(void);
G2_DLLFUNC void G2API g2_fen_set_gateway(const wchar_t* address, unsigned short port);
G2_DLLFUNC void G2API g2_fen_set_secure_local(void);
G2_DLLFUNC void G2API g2_fen_set_external(bool flag);
G2_DLLFUNC void G2API g2_fen_set_external_proxy(const wchar_t* address, unsigned short port);
G2_DLLFUNC void G2API g2_fen_set_external_gateway(const wchar_t* address, unsigned short port);
G2_DLLFUNC void G2API g2_fen_set_external_nat(const G2NAT_INFO* ni);
G2_DLLFUNC void G2API g2_fen_set_external_activate(bool activate);
G2_DLLFUNC bool G2API g2_fen_service_join(unsigned int elapse, bool pump_message_waitable);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC int  G2API g2_fen_get_nat_type(void);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_fen_is_startup(void);
G2_DLLFUNC bool G2API g2_fen_is_activate(void);
G2_DLLFUNC bool G2API g2_fen_is_failed_secure_nat_info(void);

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_CLIENT_DLL_FEN_H_
