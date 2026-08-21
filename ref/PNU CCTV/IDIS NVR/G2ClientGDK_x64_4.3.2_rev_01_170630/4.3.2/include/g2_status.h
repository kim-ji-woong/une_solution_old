// g2_status.h : header file
//

#ifndef _G2_CLIENT_DLL_STATUS_H_
#define _G2_CLIENT_DLL_STATUS_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define_status.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_status_register_callback(G2HSTATUS handle, unsigned int type, G2FUN_LISTENER func);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC G2HSTATUS G2API g2_status_initialize(G2UPARAM param);
G2_DLLFUNC void G2API g2_status_finalize(G2HSTATUS handle);
G2_DLLFUNC void G2API g2_status_startup(G2HSTATUS handle, int connections, int callback_connections);
G2_DLLFUNC void G2API g2_status_cleanup(G2HSTATUS handle);
G2_DLLFUNC bool G2API g2_status_callback_server_restart(G2HSTATUS handle, unsigned short port);
G2_DLLFUNC void G2API g2_status_callback_server_shutdown(G2HSTATUS handle);
G2_DLLFUNC bool G2API g2_status_callback_server_is_startup(G2HSTATUS handle);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC int  G2API g2_status_connect(G2HSTATUS handle, const G2GUID* root, const G2CONNECT_OPTIONS* options, G2CONNECT_RES* res);
G2_DLLFUNC int  G2API g2_status_connect_ras(G2HSTATUS handle, const G2NETWORK_INFO* ni, bool idr, const G2CONNECT_OPTIONS* options, G2CONNECT_RES* res);
G2_DLLFUNC void G2API g2_status_disconnect(G2HSTATUS handle, int channel);
G2_DLLFUNC bool G2API g2_status_is_connecting(G2HSTATUS handle, int channel);
G2_DLLFUNC bool G2API g2_status_is_connected(G2HSTATUS handle, int channel);
G2_DLLFUNC bool G2API g2_status_is_disconnecting(G2HSTATUS handle, int channel);
G2_DLLFUNC bool G2API g2_status_is_disconnected(G2HSTATUS handle, int channel);
G2_DLLFUNC bool G2API g2_status_is_disconnectable(G2HSTATUS handle, int channel);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_status_request_alive_check(G2HSTATUS handle, int channel);
G2_DLLFUNC bool G2API g2_status_request_panic_record(G2HSTATUS handle, int channel, bool on);
G2_DLLFUNC bool G2API g2_status_request_instant_recording_start(G2HSTATUS handle, int channel, const G2INSTANT_RECORD_SETTING_LIST* setting);
G2_DLLFUNC bool G2API g2_status_request_instant_recording_stop(G2HSTATUS handle, int channel, const G2CHANNEL_SET* channels);
G2_DLLFUNC bool G2API g2_status_request_instant_recording_status(G2HSTATUS handle, int channel, const G2CHANNEL_SET* channels);
G2_DLLFUNC bool G2API g2_status_request_status(G2HSTATUS handle, int channel);
G2_DLLFUNC bool G2API g2_status_request_log(G2HSTATUS handle, int channel, const G2STATUS_LOG_SEARCH_OPTIONS* options, int* res);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_status_get_adaptor(G2HSTATUS handle, void* ptr);
G2_DLLFUNC bool G2API g2_status_get_server_network_info(G2HSTATUS handle, int channel, G2SERVER_NETWORK_INFO* ni);
G2_DLLFUNC bool G2API g2_status_get_product_info(G2HSTATUS handle, int channel, G2_PRODUCT_INFO* pi);
G2_DLLFUNC bool G2API g2_status_get_remote_status_caps(G2HSTATUS handle, int channel, G2_PRODUCT_INFO_CAPS::REMOTE_STATUS* caps);
G2_DLLFUNC bool G2API g2_status_get_remote_setup_info_file(G2HSTATUS handle, int channel, const wchar_t* path, const wchar_t* sitename, G2HWND parent);
G2_DLLFUNC bool G2API g2_status_get_authority(G2HSTATUS handle, int channel, G2RAS_AUTHORITY* auth);
G2_DLLFUNC bool G2API g2_status_get_status(G2HSTATUS handle, int channel, G2MONITORING_DEVICE_STATUS* status);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_status_is_support(G2HSTATUS handle, int channel, int query);
G2_DLLFUNC bool G2API g2_status_is_authority(G2HSTATUS handle, int channel, int query);
G2_DLLFUNC bool G2API g2_status_is_IDR(G2HSTATUS handle, int channel);

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_CLIENT_DLL_STATUS_H_
