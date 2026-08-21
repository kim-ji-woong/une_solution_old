// g2_user.h : header file
//

#ifndef _G2_CLIENT_DLL_USER_H_
#define _G2_CLIENT_DLL_USER_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define_user.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_user_set_UPARAM(G2UPARAM uparam);
G2_DLLFUNC void G2API g2_user_register_callback(unsigned int type, G2FUN_LISTENER func);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_user_is_connected(void);
G2_DLLFUNC bool G2API g2_user_is_login(void);
G2_DLLFUNC bool G2API g2_user_is_logout(void);
G2_DLLFUNC bool G2API g2_user_is_administrator(void);
G2_DLLFUNC bool G2API g2_user_is_authority(int type);
G2_DLLFUNC bool G2API g2_user_is_federation(void);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_user_get_network_info(G2NETWORK_INFO* info);
G2_DLLFUNC bool G2API g2_user_get_id(G2STRING_32* string);
G2_DLLFUNC bool G2API g2_user_get_name(G2STRING_64* string);
G2_DLLFUNC bool G2API g2_user_get_description(G2STRING_128* string);
G2_DLLFUNC void G2API g2_user_get_string_authority(int type, G2STRING_128* string);
G2_DLLFUNC void G2API g2_user_get_string_authority_desc(int type, G2STRING_256* string);
G2_DLLFUNC void G2API g2_user_get_string_peer_version(G2STRING_64* string);

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_CLIENT_DLL_USER_H_
