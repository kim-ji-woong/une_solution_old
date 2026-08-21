// g2_app_frame_relay_server.h : header file
//

#ifndef _G2_CLIENT_DLL_APP_FR_SERVER_H_
#define _G2_CLIENT_DLL_APP_FR_SERVER_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define_app_frame_relay.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_app_frame_relay_server_register_callback(G2HANDLE handle, unsigned int type, G2FUN_LISTENER func);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC G2HANDLE G2API g2_app_frame_relay_server_initialize(G2UPARAM param);
G2_DLLFUNC void G2API g2_app_frame_relay_server_finalize(G2HANDLE handle);
G2_DLLFUNC void G2API g2_app_frame_relay_server_startup(G2HANDLE handle, const G2APP_FRAME_RELAY_SERVER_OPTION* option);
G2_DLLFUNC void G2API g2_app_frame_relay_server_cleanup(G2HANDLE handle);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_app_frame_relay_server_disconnect(G2HANDLE handle, int channel);
G2_DLLFUNC bool G2API g2_app_frame_relay_server_is_connected(G2HANDLE handle, int channel);
G2_DLLFUNC bool G2API g2_app_frame_relay_server_is_disconnecting(G2HANDLE handle, int channel);
G2_DLLFUNC bool G2API g2_app_frame_relay_server_is_disconnected(G2HANDLE handle, int channel);
G2_DLLFUNC bool G2API g2_app_frame_relay_server_is_disconnectable(G2HANDLE handle, int channel);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_app_frame_relay_server_request_site_product_info(G2HANDLE handle, int channel);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_app_frame_relay_server_is_usable_TCP_port(unsigned short port);
G2_DLLFUNC bool G2API g2_app_frame_relay_server_search_controller_option_make_file(const wchar_t* path, const G2APP_FRAME_RELAY_SEARCH_CONTROLLER_OPTION* option);
G2_DLLFUNC bool G2API g2_app_frame_relay_server_search_controller_run(const wchar_t* path, const wchar_t* option_file);

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_CLIENT_DLL_APP_FR_SERVER_H_
