// g2_app_frame_relay.h : header file
//

#ifndef _G2_CLIENT_DLL_APP_FR_H_
#define _G2_CLIENT_DLL_APP_FR_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define_app_frame_relay.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_app_frame_relay_register_callback(G2HANDLE handle, unsigned int type, G2FUN_LISTENER func);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC G2HANDLE G2API g2_app_frame_relay_initialize(G2UPARAM param);
G2_DLLFUNC void G2API g2_app_frame_relay_finalize(G2HANDLE handle);
G2_DLLFUNC void G2API g2_app_frame_relay_startup(G2HANDLE handle, int connections, unsigned int send_queue_size_limit);
G2_DLLFUNC void G2API g2_app_frame_relay_cleanup(G2HANDLE handle);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC int  G2API g2_app_frame_relay_connect(G2HANDLE handle, const G2NETWORK_INFO* ni, const G2CONNECT_OPTIONS* options, G2CONNECT_RES* res);
G2_DLLFUNC void G2API g2_app_frame_relay_disconnect(G2HANDLE handle, int channel);
G2_DLLFUNC bool G2API g2_app_frame_relay_is_connecting(G2HANDLE handle, int channel);
G2_DLLFUNC bool G2API g2_app_frame_relay_is_connected(G2HANDLE handle, int channel);
G2_DLLFUNC bool G2API g2_app_frame_relay_is_disconnecting(G2HANDLE handle, int channel);
G2_DLLFUNC bool G2API g2_app_frame_relay_is_disconnected(G2HANDLE handle, int channel);
G2_DLLFUNC bool G2API g2_app_frame_relay_is_disconnectable(G2HANDLE handle, int channel);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_app_frame_relay_request_alive_check(G2HANDLE handle, int channel, int val);
G2_DLLFUNC bool G2API g2_app_frame_relay_send_site_connected(G2HANDLE handle, int channel, const G2STRING_64* site);
G2_DLLFUNC bool G2API g2_app_frame_relay_send_site_disconnected(G2HANDLE handle, int channel, const G2STRING_64* site, int reason);
G2_DLLFUNC bool G2API g2_app_frame_relay_send_site_product_info(G2HANDLE handle, int channel, const G2STRING_64* site, bool(G2API *get_adaptor)(G2HANDLE, void*), G2HANDLE from_handle, int from_channel);
G2_DLLFUNC bool G2API g2_app_frame_relay_send_site_frame_data(G2HANDLE handle, int channel, const G2STRING_64* site, const G2FRAME* frame);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_app_frame_relay_is_able_to_send(G2HANDLE handle, int channel, unsigned int size);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_app_frame_relay_search_controller_option_read_file(const wchar_t* path, G2APP_FRAME_RELAY_SEARCH_CONTROLLER_OPTION* option);

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_CLIENT_DLL_APP_FR_H_
