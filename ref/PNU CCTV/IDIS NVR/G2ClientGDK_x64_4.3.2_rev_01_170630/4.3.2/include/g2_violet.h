// g2_violet.h : header file
//

#ifndef _G2_CLIENT_DLL_VIOLET_H_
#define _G2_CLIENT_DLL_VIOLET_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define_violet.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_violet_register_callback(G2HVIOLET handle, unsigned int type, G2FUN_LISTENER func);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC G2HVIOLET G2API g2_violet_initialize(G2UPARAM param);
G2_DLLFUNC void G2API g2_violet_finalize(G2HVIOLET handle);
G2_DLLFUNC void G2API g2_violet_startup(G2HVIOLET handle, int connections);
G2_DLLFUNC void G2API g2_violet_cleanup(G2HVIOLET handle);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC int  G2API g2_violet_connect(G2HVIOLET handle, const G2GUID* service, const G2CONNECT_OPTIONS* options, G2CONNECT_RES* res);
G2_DLLFUNC void G2API g2_violet_disconnect(G2HVIOLET handle, int channel);
G2_DLLFUNC bool G2API g2_violet_is_connecting(G2HVIOLET handle, int channel);
G2_DLLFUNC bool G2API g2_violet_is_connected(G2HVIOLET handle, int channel);
G2_DLLFUNC bool G2API g2_violet_is_disconnecting(G2HVIOLET handle, int channel);
G2_DLLFUNC bool G2API g2_violet_is_disconnected(G2HVIOLET handle, int channel);
G2_DLLFUNC bool G2API g2_violet_is_disconnectable(G2HVIOLET handle, int channel);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_violet_request_set_layout_monitor_live(G2HVIOLET handle, int channel, int monitor, const G2GUID* layout);
G2_DLLFUNC bool G2API g2_violet_request_set_screen_format(G2HVIOLET handle, int channel, int monitor, int format);
G2_DLLFUNC bool G2API g2_violet_request_set_screen_camera_format(G2HVIOLET handle, int channel, int monitor, int camera, int format);
G2_DLLFUNC bool G2API g2_violet_request_get_cameras_on_monitor(G2HVIOLET handle, int channel, int monitor);
G2_DLLFUNC bool G2API g2_violet_request_system_log_search(G2HVIOLET handle, int channel, const G2SERVICE_SEARCH_OPTION_SYSTEM_LOG* option);
G2_DLLFUNC bool G2API g2_violet_request_system_log_search_stop(G2HVIOLET handle, int channel);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_violet_send_stream_channel_control_via_id(G2HVIOLET handle, int channel, int seq, int id);
G2_DLLFUNC bool G2API g2_violet_send_stream_channel_control_ptz_joystick(G2HVIOLET handle, int channel, int seq, int id, int speed);
G2_DLLFUNC bool G2API g2_violet_send_stream_channel_control(G2HVIOLET handle, int channel, int seq, int len, const char* buf);
G2_DLLFUNC bool G2API g2_violet_send_mouse_input(G2HVIOLET handle, int channel, const G2VIOLET_MOUSE_INPUT* mi);
G2_DLLFUNC bool G2API g2_violet_send_monitor_message(G2HVIOLET handle, int channel, int monitor, const wchar_t* message, unsigned int time_out, unsigned short size, int position);
G2_DLLFUNC bool G2API g2_violet_send_monitor_message_hide(G2HVIOLET handle, int channel, int monitor);
G2_DLLFUNC bool G2API g2_violet_send_screen_message(G2HVIOLET handle, int channel, int monitor, const G2VIOLET_MONITOR_MESSAGE* message);
G2_DLLFUNC bool G2API g2_violet_send_screen_message_hide(G2HVIOLET handle, int channel, int monitor);
G2_DLLFUNC bool G2API g2_violet_send_agent_audio_connect(G2HVIOLET handle, int channel, int monitor, const wchar_t* address, unsigned short port, const wchar_t* id, const wchar_t* password, bool encrypted);
G2_DLLFUNC bool G2API g2_violet_send_agent_audio_disconnect(G2HVIOLET handle, int channel, int monitor);

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_CLIENT_DLL_VIOLET_H_
