// g2_watch.h : header file
//

#ifndef _G2_CLIENT_DLL_WATCH_H_
#define _G2_CLIENT_DLL_WATCH_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define_watch.h"
#include "g2_define_live.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_watch_register_callback(G2HWATCH handle, unsigned int type, G2FUN_LISTENER func);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC G2HWATCH G2API g2_watch_initialize(G2UPARAM param);
G2_DLLFUNC void G2API g2_watch_finalize(G2HWATCH handle);
G2_DLLFUNC void G2API g2_watch_startup(G2HWATCH handle, int connections, G2HWND focus);
G2_DLLFUNC void G2API g2_watch_cleanup(G2HWATCH handle);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC int  G2API g2_watch_connect(G2HWATCH handle, const G2GUID* root, const G2CONNECT_OPTIONS* options, G2CONNECT_RES* res);
G2_DLLFUNC int  G2API g2_watch_connect_ras(G2HWATCH handle, const G2NETWORK_INFO* ni, const G2CONNECT_OPTIONS* options, G2CONNECT_RES* res);
G2_DLLFUNC int  G2API g2_watch_connect_ras_event(G2HWATCH handle, const G2NETWORK_INFO* ni, const G2CONNECT_OPTIONS* options, G2CONNECT_RES* res);
G2_DLLFUNC void G2API g2_watch_disconnect(G2HWATCH handle, int channel);
G2_DLLFUNC bool G2API g2_watch_is_connecting(G2HWATCH handle, int channel);
G2_DLLFUNC bool G2API g2_watch_is_connected(G2HWATCH handle, int channel);
G2_DLLFUNC bool G2API g2_watch_is_disconnecting(G2HWATCH handle, int channel);
G2_DLLFUNC bool G2API g2_watch_is_disconnected(G2HWATCH handle, int channel);
G2_DLLFUNC bool G2API g2_watch_is_disconnectable(G2HWATCH handle, int channel);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_watch_set_focus_G2HWND(G2HWATCH handle, G2HWND focus);
G2_DLLFUNC bool G2API g2_watch_set_camera_channelset(G2HWATCH handle, int channel, const G2CHANNEL_SET* channels, bool force);
G2_DLLFUNC bool G2API g2_watch_set_camera_stream_set(G2HWATCH handle, int channel, const G2CHANNEL_STREAM_SET* streams, bool force);
G2_DLLFUNC bool G2API g2_watch_set_camera_stream_set_onetime(G2HWATCH handle, int channel, const G2CHANNEL_STREAM_SET* streams, const G2CHANNEL_STREAM_SET* onetime_streams);
G2_DLLFUNC bool G2API g2_watch_set_audio_channelset(G2HWATCH handle, int channel, const G2CHANNEL_SET* channels, bool force);
G2_DLLFUNC bool G2API g2_watch_set_stream_id(G2HWATCH handle, int channel, int camera, int stream_id);
G2_DLLFUNC bool G2API g2_watch_set_alarm_out(G2HWATCH handle, int channel, int num, bool on);
G2_DLLFUNC bool G2API g2_watch_set_beep_control(G2HWATCH handle, int channel, bool on);
G2_DLLFUNC bool G2API g2_watch_set_camera_color(G2HWATCH handle, int channel, int camera, int type, int value);
G2_DLLFUNC bool G2API g2_watch_set_ptz_command(G2HWATCH handle, int channel, int camera, const G2LIVE_PTZ_COMMAND* command);
G2_DLLFUNC bool G2API g2_watch_set_ptz_preset(G2HWATCH handle, int channel, int camera, const G2LIVE_PTZ_PRESET* preset);
G2_DLLFUNC bool G2API g2_watch_set_network_alarm(G2HWATCH handle, int channel, const G2LIVE_NETWORK_ALARM_INFO* info);
G2_DLLFUNC bool G2API g2_watch_set_enable_audio_streaming(G2HWATCH handle, int channel, int camera, bool enable);
G2_DLLFUNC bool G2API g2_watch_set_enable_audio_capturing(G2HWATCH handle, int channel, int camera, bool enable);
G2_DLLFUNC bool G2API g2_watch_set_disable_audio_streaming(G2HWATCH handle, int channel);
G2_DLLFUNC bool G2API g2_watch_set_disable_audio_capturing(G2HWATCH handle, int channel);
G2_DLLFUNC bool G2API g2_watch_set_disable_audio(G2HWATCH handle, int channel);
G2_DLLFUNC void G2API g2_watch_set_probe_session_profile(G2HWATCH handle, bool active);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_watch_request_alive_check(G2HWATCH handle, int channel);
G2_DLLFUNC bool G2API g2_watch_request_device_status(G2HWATCH handle, int channel);
G2_DLLFUNC bool G2API g2_watch_request_ptz_menu(G2HWATCH handle, int channel, int camera);
G2_DLLFUNC bool G2API g2_watch_request_ptz_preset(G2HWATCH handle, int channel, int camera);
G2_DLLFUNC bool G2API g2_watch_request_stream_channel_control(G2HWATCH handle, int channel, int camera, int seq, signed char* buf, int len);
G2_DLLFUNC bool G2API g2_watch_request_instant_recording_start(G2HWATCH handle, int channel, const G2INSTANT_RECORD_SETTING_LIST* setting);
G2_DLLFUNC bool G2API g2_watch_request_instant_recording_stop(G2HWATCH handle, int channel, const G2CHANNEL_SET* channels);
G2_DLLFUNC bool G2API g2_watch_request_instant_recording_status(G2HWATCH handle, int channel, const G2CHANNEL_SET* channels);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_watch_send_command_control_color_status(G2HWATCH handle, int channel, int camera);
G2_DLLFUNC bool G2API g2_watch_send_command_control_color(G2HWATCH handle, int channel, int camera, const G2LIVE_COMMAND_CONTROL_COLOR* control);
G2_DLLFUNC bool G2API g2_watch_send_command_control_ptz_status(G2HWATCH handle, int channel, int camera);
G2_DLLFUNC bool G2API g2_watch_send_command_control_ptz_status_relative(G2HWATCH handle, int channel, int camera);
G2_DLLFUNC bool G2API g2_watch_send_command_control_ptz_status_relative_IDIS(G2HWATCH handle, int channel, int camera);
G2_DLLFUNC bool G2API g2_watch_send_command_control_ptz(G2HWATCH handle, int channel, int camera, const G2LIVE_COMMAND_CONTROL_PTZ* control);
G2_DLLFUNC bool G2API g2_watch_send_command_control_ptz_IDIS(G2HWATCH handle, int channel, int camera, const G2LIVE_COMMAND_CONTROL_PTZ_STATUS* status, bool req_result);
G2_DLLFUNC bool G2API g2_watch_send_fish_eye_control(G2HWATCH handle, int channel, const G2FISHEYE_CONTROL* control);
G2_DLLFUNC bool G2API g2_watch_send_network_alarm_info(G2HWATCH handle, int channel, const G2LIVE_NETWORK_ALARM_INFO* info);
G2_DLLFUNC bool G2API g2_watch_send_elevator_status_info(G2HWATCH handle, int channel, const G2LIVE_ELEVATOR_STATUS_INFO* info);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_watch_get_adaptor(G2HWATCH handle, void* ptr);
G2_DLLFUNC bool G2API g2_watch_get_server_network_info(G2HWATCH handle, int channel, G2SERVER_NETWORK_INFO* ni);
G2_DLLFUNC bool G2API g2_watch_get_product_info(G2HWATCH handle, int channel, G2_PRODUCT_INFO* pi);
G2_DLLFUNC bool G2API g2_watch_get_remote_watch_caps(G2HWATCH handle, int channel, G2_PRODUCT_INFO_CAPS::REMOTE_WATCH* caps);
G2_DLLFUNC bool G2API g2_watch_get_remote_setup_info_file(G2HWATCH handle, int channel, const wchar_t* path, const wchar_t* sitename, G2HWND parent);
G2_DLLFUNC bool G2API g2_watch_get_authority(G2HWATCH handle, int channel, G2RAS_AUTHORITY* auth);
G2_DLLFUNC bool G2API g2_watch_get_camera_channelset(G2HWATCH handle, int channel, G2CHANNEL_SET* channels);
G2_DLLFUNC bool G2API g2_watch_get_camera_stream_set(G2HWATCH handle, int channel, G2CHANNEL_STREAM_SET* streams);
G2_DLLFUNC bool G2API g2_watch_get_camera_stream(G2HWATCH handle, int channel, int camera, G2CHANNEL_STREAM_SET* streams);
G2_DLLFUNC bool G2API g2_watch_get_audio_channelset(G2HWATCH handle, int channel, G2CHANNEL_SET* channels);
G2_DLLFUNC int  G2API g2_watch_get_stream_id(G2HWATCH handle, int channel, int camera);
G2_DLLFUNC unsigned int G2API g2_watch_get_stream_remote(G2HWATCH handle, int channel, int camera);
G2_DLLFUNC bool G2API g2_watch_get_status(G2HWATCH handle, int channel, G2DEVICE_STATUS* status);
G2_DLLFUNC bool G2API g2_watch_get_status_stream_info(G2HWATCH handle, int channel, int camera, int stream_id, G2DEVICE_STATUS_STREAM_INFO* status);
G2_DLLFUNC bool G2API g2_watch_get_status_ip_camera_info(G2HWATCH handle, int channel, int camera, G2DEVICE_STATUS_IP_CAMERA_INFO* status);
G2_DLLFUNC bool G2API g2_watch_get_status_ptz_advanced_info(G2HWATCH handle, int channel, int camera, G2DEVICE_STATUS_PTZ_ADVANCED_INFO* status);
G2_DLLFUNC int  G2API g2_watch_get_status_ptz(G2HWATCH handle, int channel, int camera);
G2_DLLFUNC unsigned int G2API g2_watch_get_status_ptz_function(G2HWATCH handle, int channel, int camera);
G2_DLLFUNC int  G2API g2_watch_get_status_stream_eptz(G2HWATCH handle, int channel, int camera, int stream_id);
G2_DLLFUNC unsigned int G2API g2_watch_get_status_stream_eptz_function(G2HWATCH handle, int channel, int camera, int stream_id);
G2_DLLFUNC bool G2API g2_watch_get_status_command_control_color(G2HWATCH handle, int channel, int camera, G2LIVE_COMMAND_CONTROL_COLOR* control, G2LIVE_COMMAND_CONTROL_COLOR_RANGE* range);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_watch_is_ras_event_session(G2HWATCH handle, int channel);
G2_DLLFUNC bool G2API g2_watch_is_enable_multi_stream(G2HWATCH handle, int channel, int camera);
G2_DLLFUNC bool G2API g2_watch_is_enable_command_control_color(G2HWATCH handle, int channel, int camera);
G2_DLLFUNC bool G2API g2_watch_is_enable_command_control_ptz(G2HWATCH handle, int channel, int camera);
G2_DLLFUNC bool G2API g2_watch_is_enable_command_control_ptz_relative(G2HWATCH handle, int channel, int camera);
G2_DLLFUNC bool G2API g2_watch_is_enable_command_control_ptz_relative_IDIS(G2HWATCH handle, int channel, int camera);
G2_DLLFUNC bool G2API g2_watch_is_enable_command_control_ptz_relative_IDIS_one_click_move(G2HWATCH handle, int channel, int camera);
G2_DLLFUNC bool G2API g2_watch_is_enable_audio_in(G2HWATCH handle, int channel, int camera);
G2_DLLFUNC bool G2API g2_watch_is_enable_audio_out(G2HWATCH handle, int channel, int camera);
G2_DLLFUNC bool G2API g2_watch_is_contains_audio_streaming(G2HWATCH handle, int channel, int camera);
G2_DLLFUNC bool G2API g2_watch_is_contains_audio_capturing(G2HWATCH handle, int channel, int camera);
G2_DLLFUNC bool G2API g2_watch_is_contains_audio_capturing_any(G2HWATCH handle, int channel);
G2_DLLFUNC bool G2API g2_watch_is_contains_audio(G2HWATCH handle, int channel);
G2_DLLFUNC bool G2API g2_watch_is_audio_out_opening(G2HWATCH handle, int channel, int camera);
G2_DLLFUNC bool G2API g2_watch_is_support(G2HWATCH handle, int channel, int query);
G2_DLLFUNC bool G2API g2_watch_is_authority(G2HWATCH handle, int channel, int query);
G2_DLLFUNC bool G2API g2_watch_is_probe_session_profile(G2HWATCH handle);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_watch_is_enable_audio_device_play(G2HWND focus);
G2_DLLFUNC bool G2API g2_watch_is_enable_audio_device_record(void);

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_CLIENT_DLL_WATCH_H_
