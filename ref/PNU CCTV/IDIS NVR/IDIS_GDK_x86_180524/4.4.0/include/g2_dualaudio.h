// g2_dualaudio.h : header file
//

#ifndef _G2_CLIENT_DLL_DUALAUDIO_H_
#define _G2_CLIENT_DLL_DUALAUDIO_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define_dualaudio.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_dualaudio_register_callback(G2HDUALAUDIO handle, unsigned int type, G2FUN_LISTENER func);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC G2HDUALAUDIO G2API g2_dualaudio_initialize(G2UPARAM param);
G2_DLLFUNC void G2API g2_dualaudio_finalize(G2HDUALAUDIO handle);
G2_DLLFUNC void G2API g2_dualaudio_startup(G2HDUALAUDIO handle, int connections, G2HWND focus);
G2_DLLFUNC void G2API g2_dualaudio_cleanup(G2HDUALAUDIO handle);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC int  G2API g2_dualaudio_connect(G2HDUALAUDIO handle, const G2GUID* site, const G2CONNECT_OPTIONS* options, G2CONNECT_RES* res);
G2_DLLFUNC int  G2API g2_dualaudio_connect_ras(G2HDUALAUDIO handle, const G2NETWORK_INFO* ni, const G2CONNECT_OPTIONS* options, G2CONNECT_RES* res);
G2_DLLFUNC void G2API g2_dualaudio_disconnect(G2HDUALAUDIO handle, int channel);
G2_DLLFUNC bool G2API g2_dualaudio_is_connecting(G2HDUALAUDIO handle, int channel);
G2_DLLFUNC bool G2API g2_dualaudio_is_connected(G2HDUALAUDIO handle, int channel);
G2_DLLFUNC bool G2API g2_dualaudio_is_disconnecting(G2HDUALAUDIO handle, int channel);
G2_DLLFUNC bool G2API g2_dualaudio_is_disconnected(G2HDUALAUDIO handle, int channel);
G2_DLLFUNC bool G2API g2_dualaudio_is_disconnectable(G2HDUALAUDIO handle, int channel);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_dualaudio_set_audio_in_enable(G2HDUALAUDIO handle, int channel, int number, bool enable);
G2_DLLFUNC bool G2API g2_dualaudio_set_audio_out_enable(G2HDUALAUDIO handle, int channel, bool enable);
G2_DLLFUNC void G2API g2_dualaudio_set_audio_in_disable(G2HDUALAUDIO handle, int channel);
G2_DLLFUNC void G2API g2_dualaudio_set_audio_out_disable(G2HDUALAUDIO handle, int channel);
G2_DLLFUNC void G2API g2_dualaudio_set_audio_in_disable_all(G2HDUALAUDIO handle);
G2_DLLFUNC void G2API g2_dualaudio_set_audio_out_disable_all(G2HDUALAUDIO handle);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_dualaudio_append_reserve(G2HDUALAUDIO handle, const G2GUID* site, unsigned int mode, const G2DUALAUDIO_RESERVE* param);
G2_DLLFUNC void G2API g2_dualaudio_remove_reserve(G2HDUALAUDIO handle, const G2GUID* site);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_dualaudio_get_reserved_parameter(G2HDUALAUDIO handle, const G2GUID* site, unsigned int mode, G2DUALAUDIO_RESERVE* param);
G2_DLLFUNC G2GUID G2API g2_dualaudio_get_site_guid(G2HDUALAUDIO handle, int channel);
G2_DLLFUNC int  G2API g2_dualaudio_get_channel_from_site_guid(G2HDUALAUDIO handle, const G2GUID* site);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_dualaudio_is_drive_mode(G2HDUALAUDIO handle, int channel, int mode);
G2_DLLFUNC bool G2API g2_dualaudio_is_server_full_channel(G2HDUALAUDIO handle, const G2GUID* site);
G2_DLLFUNC bool G2API g2_dualaudio_is_reserved(G2HDUALAUDIO handle, const G2GUID* site, unsigned int mode);
G2_DLLFUNC bool G2API g2_dualaudio_is_contains_site_guid(G2HDUALAUDIO handle, const G2GUID* site);
G2_DLLFUNC bool G2API g2_dualaudio_is_require_reconnect(G2HDUALAUDIO handle, int channel);
G2_DLLFUNC bool G2API g2_dualaudio_is_last_use_audio_in(G2HDUALAUDIO handle, const G2GUID* site);
G2_DLLFUNC bool G2API g2_dualaudio_is_last_use_audio_out(G2HDUALAUDIO handle, const G2GUID* site);
G2_DLLFUNC bool G2API g2_dualaudio_is_activate_audio_in(G2HDUALAUDIO handle, const G2GUID* site);
G2_DLLFUNC bool G2API g2_dualaudio_is_activate_audio_out(G2HDUALAUDIO handle, const G2GUID* site);
G2_DLLFUNC bool G2API g2_dualaudio_is_activate_dualaudio(G2HDUALAUDIO handle, const G2GUID* site);
G2_DLLFUNC bool G2API g2_dualaudio_is_opening_audio_out(G2HDUALAUDIO handle, int channel);

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_CLIENT_DLL_DUALAUDIO_H_
