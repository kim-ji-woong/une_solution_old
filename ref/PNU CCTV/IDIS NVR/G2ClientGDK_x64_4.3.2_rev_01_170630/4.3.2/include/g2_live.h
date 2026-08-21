// g2_live.h : header file
//

#ifndef _G2_CLIENT_DLL_LIVE_H_
#define _G2_CLIENT_DLL_LIVE_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define_live.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_live_register_callback(G2HLIVE handle, unsigned int type, G2FUN_LISTENER func);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC G2HLIVE G2API g2_live_initialize(G2UPARAM param);
G2_DLLFUNC void G2API g2_live_finalize(G2HLIVE handle);
G2_DLLFUNC void G2API g2_live_startup(G2HLIVE handle, int connections, G2HWND focus);
G2_DLLFUNC void G2API g2_live_cleanup(G2HLIVE handle);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC int  G2API g2_live_connect(G2HLIVE handle, const G2GUID* service, const G2CONNECT_OPTIONS* options, G2CONNECT_RES* res);
G2_DLLFUNC void G2API g2_live_disconnect(G2HLIVE handle, int channel);
G2_DLLFUNC bool G2API g2_live_is_connecting(G2HLIVE handle, int channel);
G2_DLLFUNC bool G2API g2_live_is_connected(G2HLIVE handle, int channel);
G2_DLLFUNC bool G2API g2_live_is_disconnecting(G2HLIVE handle, int channel);
G2_DLLFUNC bool G2API g2_live_is_disconnected(G2HLIVE handle, int channel);
G2_DLLFUNC bool G2API g2_live_is_disconnectable(G2HLIVE handle, int channel);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_live_set_camera_list(G2HLIVE handle, int channel, const G2CHANNEL_SET* channels, bool force);
G2_DLLFUNC bool G2API g2_live_set_camera_list_interest(G2HLIVE handle, int channel, const G2CHANNEL_SET* channels, bool force);
G2_DLLFUNC bool G2API g2_live_set_camera_stream_set(G2HLIVE handle, int channel, const G2CHANNEL_STREAM_SET* streams, bool force);
G2_DLLFUNC bool G2API g2_live_set_camera_stream_channel(G2HLIVE handle, int channel, int channelext, int stream_id);
G2_DLLFUNC bool G2API g2_live_set_audio_list(G2HLIVE handle, int channel, const G2CHANNEL_SET* channels, bool force);
G2_DLLFUNC bool G2API g2_live_set_stream_id(G2HLIVE handle, int channel, const G2GUID* camera, int id);
G2_DLLFUNC bool G2API g2_live_set_alarm_out(G2HLIVE handle, int channel, const G2GUID* alarm, bool on);
G2_DLLFUNC bool G2API g2_live_set_beep_control(G2HLIVE handle, int channel, const G2GUID* root, bool on);
G2_DLLFUNC bool G2API g2_live_set_camera_color(G2HLIVE handle, int channel, const G2GUID* camera, int type, int value);
G2_DLLFUNC bool G2API g2_live_set_ptz_command(G2HLIVE handle, int channel, const G2GUID* camera, const G2LIVE_PTZ_COMMAND* command);
G2_DLLFUNC bool G2API g2_live_set_ptz_preset(G2HLIVE handle, int channel, const G2GUID* camera, const G2LIVE_PTZ_PRESET* preset);
G2_DLLFUNC bool G2API g2_live_set_enable_audio_streaming(G2HLIVE handle, int channel, const G2GUID* camera, bool enable);
G2_DLLFUNC bool G2API g2_live_set_enable_audio_capturing(G2HLIVE handle, int channel, const G2GUID* root, int camera, bool enable);
G2_DLLFUNC bool G2API g2_live_set_disable_audio_streaming(G2HLIVE handle, int channel, const G2GUID* root);
G2_DLLFUNC bool G2API g2_live_set_disable_audio_capturing(G2HLIVE handle, int channel, const G2GUID* root);
G2_DLLFUNC bool G2API g2_live_set_disable_audio(G2HLIVE handle, int channel);
G2_DLLFUNC bool G2API g2_live_set_disable_audio_root_guid(G2HLIVE handle, int channel, const G2GUID* root);
G2_DLLFUNC void G2API g2_live_set_probe_session_profile(G2HLIVE handle, bool active);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_live_request_alive_check(G2HLIVE handle, int channel, int check);
G2_DLLFUNC bool G2API g2_live_request_stream_channels(G2HLIVE handle, int channel);
G2_DLLFUNC bool G2API g2_live_request_stream_channels_each(G2HLIVE handle, int channel, const G2GUID camera[], unsigned int count);
G2_DLLFUNC bool G2API g2_live_request_ptz_menu(G2HLIVE handle, int channel, const G2GUID* camera);
G2_DLLFUNC bool G2API g2_live_request_ptz_preset(G2HLIVE handle, int channel, const G2GUID* camera);
G2_DLLFUNC bool G2API g2_live_request_system_log_search(G2HLIVE handle, int channel, const G2SERVICE_SEARCH_OPTION_SYSTEM_LOG* option);
G2_DLLFUNC bool G2API g2_live_request_system_log_search_stop(G2HLIVE handle, int channel);
G2_DLLFUNC bool G2API g2_live_request_debug_log_search(G2HLIVE handle, int channel, const G2SERVICE_SEARCH_OPTION_DEBUG_LOG* option);
G2_DLLFUNC bool G2API g2_live_request_debug_log_search_stop(G2HLIVE handle, int channel);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_live_send_command_control_color(G2HLIVE handle, int channel, const G2GUID* cameraGUID, int camera, const G2LIVE_COMMAND_CONTROL_COLOR* control);
G2_DLLFUNC bool G2API g2_live_send_command_control_color_status(G2HLIVE handle, int channel, const G2GUID* cameraGUID, int camera);
G2_DLLFUNC bool G2API g2_live_send_command_control_ptz(G2HLIVE handle, int channel, const G2GUID* cameraGUID, int camera, const G2LIVE_COMMAND_CONTROL_PTZ* control, bool req_result);
G2_DLLFUNC bool G2API g2_live_send_command_control_ptz_status(G2HLIVE handle, int channel, const G2GUID* cameraGUID, int camera);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_live_get_camera_list(G2HLIVE handle, int channel, G2CHANNEL_SET* channels);
G2_DLLFUNC bool G2API g2_live_get_camera_list_interest(G2HLIVE handle, int channel, G2CHANNEL_SET* channels);
G2_DLLFUNC bool G2API g2_live_get_camera_stream_set(G2HLIVE handle, int channel, G2CHANNEL_STREAM_SET* streams);
G2_DLLFUNC bool G2API g2_live_get_camera_stream(G2HLIVE handle, int channel, int channelext, G2CHANNEL_STREAM_SET* streams);
G2_DLLFUNC G2GUID G2API g2_live_get_guid_from_channelext(G2HLIVE handle, int channel, int channelext);
G2_DLLFUNC int  G2API g2_live_get_channelext_from_guid(G2HLIVE handle, int channel, const G2GUID* camera);
G2_DLLFUNC int  G2API g2_live_get_stream_id(G2HLIVE handle, int channel, const G2GUID* camera);
G2_DLLFUNC int  G2API g2_live_get_stream_count(G2HLIVE handle, int channel, const G2GUID* camera);
G2_DLLFUNC unsigned int G2API g2_live_get_stream_remote(G2HLIVE handle, int channel, const G2GUID* camera);
G2_DLLFUNC bool G2API g2_live_get_camera_status(G2HLIVE handle, int channel, const G2GUID* camera, G2LIVE_CAMERA_STATUS* status);
G2_DLLFUNC bool G2API g2_live_get_camera_status_stream_info(G2HLIVE handle, int channel, const G2GUID* camera, int stream_id, G2DEVICE_STATUS_STREAM_INFO* status);
G2_DLLFUNC unsigned int G2API g2_live_get_status_ptz_function(G2HLIVE handle, int channel, const G2GUID* camera);
G2_DLLFUNC bool G2API g2_live_get_status_command_control_color(G2HLIVE handle, int channel, const G2GUID* camera, G2LIVE_COMMAND_CONTROL_COLOR* control, G2LIVE_COMMAND_CONTROL_COLOR_RANGE* range);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_live_is_enable_multi_stream(G2HLIVE handle, int channel, const G2GUID* camera);
G2_DLLFUNC bool G2API g2_live_is_enable_audio_in(G2HLIVE handle, int channel, const G2GUID* camera);
G2_DLLFUNC bool G2API g2_live_is_enable_audio_out(G2HLIVE handle, int channel, const G2GUID* root, int camera);
G2_DLLFUNC bool G2API g2_live_is_contains_audio_streaming(G2HLIVE handle, int channel, const G2GUID* camera);
G2_DLLFUNC bool G2API g2_live_is_contains_audio_capturing(G2HLIVE handle, int channel, const G2GUID* root, int camera);
G2_DLLFUNC bool G2API g2_live_is_contains_audio_capturing_root(G2HLIVE handle, int channel, const G2GUID* root);
G2_DLLFUNC bool G2API g2_live_is_contains_audio(G2HLIVE handle, int channel, const G2GUID* root);
G2_DLLFUNC bool G2API g2_live_is_audio_out_opening(G2HLIVE handle, int channel, const G2GUID* root, int camera);
G2_DLLFUNC bool G2API g2_live_is_support(G2HLIVE handle, int channel, const G2GUID* camera, int query);
G2_DLLFUNC bool G2API g2_live_is_enable_command_control_ptz(G2HLIVE handle, int channel, const G2GUID* camera);
G2_DLLFUNC bool G2API g2_live_is_enable_command_control_color(G2HLIVE handle, int channel, const G2GUID* camera);

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_CLIENT_DLL_LIVE_H_
