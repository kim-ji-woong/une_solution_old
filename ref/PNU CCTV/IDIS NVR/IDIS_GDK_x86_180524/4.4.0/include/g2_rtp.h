// g2_rtp.h : header file
//

#ifndef _G2_CLIENT_DLL_RTP_H_
#define _G2_CLIENT_DLL_RTP_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define_rtp.h"
#include "g2_define_live.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_rtp_register_callback(G2HRTP handle, unsigned int type, G2FUN_LISTENER func);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC G2HRTP G2API g2_rtp_initialize(G2UPARAM param);
G2_DLLFUNC void G2API g2_rtp_finalize(G2HRTP handle);
G2_DLLFUNC void G2API g2_rtp_startup(G2HRTP handle, int connections, unsigned short port_range_min, unsigned short port_range_max, unsigned int count_buffering, G2HWND focus);
G2_DLLFUNC void G2API g2_rtp_cleanup(G2HRTP handle);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC int  G2API g2_rtp_connect(G2HRTP handle, const G2GUID* root, const G2CONNECT_OPTIONS* options, G2CONNECT_RES* res);
G2_DLLFUNC void G2API g2_rtp_disconnect(G2HRTP handle, int channel);
G2_DLLFUNC bool G2API g2_rtp_is_connecting(G2HRTP handle, int channel);
G2_DLLFUNC bool G2API g2_rtp_is_connected(G2HRTP handle, int channel);
G2_DLLFUNC bool G2API g2_rtp_is_disconnecting(G2HRTP handle, int channel);
G2_DLLFUNC bool G2API g2_rtp_is_disconnected(G2HRTP handle, int channel);
G2_DLLFUNC bool G2API g2_rtp_is_disconnectable(G2HRTP handle, int channel);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_rtp_set_incomming_port(G2HRTP handle, unsigned short port_range_min, unsigned short port_range_max);
G2_DLLFUNC void G2API g2_rtp_set_buffering_count(G2HRTP handle, int count);
G2_DLLFUNC bool G2API g2_rtp_set_camera_channelset(G2HRTP handle, int channel, const G2CHANNEL_SET* channels, bool force);
G2_DLLFUNC bool G2API g2_rtp_set_camera_list(G2HRTP handle, int channel, unsigned int cameras, bool force);
G2_DLLFUNC bool G2API g2_rtp_set_audio_channelset(G2HRTP handle, int channel, const G2CHANNEL_SET* channels, bool force);
G2_DLLFUNC bool G2API g2_rtp_set_audio_list(G2HRTP handle, int channel, unsigned int audios, bool force);
G2_DLLFUNC bool G2API g2_rtp_set_stream_id(G2HRTP handle, int channel, int camera, int id);
G2_DLLFUNC bool G2API g2_rtp_set_alarm_out(G2HRTP handle, int channel, int id, bool on);
G2_DLLFUNC bool G2API g2_rtp_set_camera_color(G2HRTP handle, int channel, int camera, int type, int value);
G2_DLLFUNC bool G2API g2_rtp_set_ptz_command(G2HRTP handle, int channel, int camera, const G2LIVE_PTZ_COMMAND* command);
G2_DLLFUNC bool G2API g2_rtp_set_ptz_preset(G2HRTP handle, int channel, int camera, const G2LIVE_PTZ_PRESET* preset);
G2_DLLFUNC bool G2API g2_rtp_set_enable_audio_streaming(G2HRTP handle, int channel, int camera, bool enable);
G2_DLLFUNC bool G2API g2_rtp_set_enable_audio_capturing(G2HRTP handle, int channel, int camera, bool enable);
G2_DLLFUNC bool G2API g2_rtp_set_disable_audio_streaming(G2HRTP handle, int channel);
G2_DLLFUNC bool G2API g2_rtp_set_disable_audio_capturing(G2HRTP handle, int channel);
G2_DLLFUNC bool G2API g2_rtp_set_disable_audio(G2HRTP handle, int channel);
G2_DLLFUNC void G2API g2_rtp_set_probe_session_profile(G2HRTP handle, bool active);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_rtp_request_alive_check(G2HRTP handle, int channel);
G2_DLLFUNC bool G2API g2_rtp_request_ptz_menu(G2HRTP handle, int channel, int camera);
G2_DLLFUNC bool G2API g2_rtp_request_ptz_preset(G2HRTP handle, int channel, int camera);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_rtp_get_camera_channelset(G2HRTP handle, int channel, G2CHANNEL_SET* channels);
G2_DLLFUNC bool G2API g2_rtp_get_camera_list(G2HRTP handle, int channel, unsigned int* cameras);
G2_DLLFUNC bool G2API g2_rtp_get_audio_channelset(G2HRTP handle, int channel, G2CHANNEL_SET* channels);
G2_DLLFUNC bool G2API g2_rtp_get_audio_list(G2HRTP handle, int channel, unsigned int* audios);
G2_DLLFUNC int  G2API g2_rtp_get_stream_id(G2HRTP handle, int channel, int camera);
G2_DLLFUNC unsigned int G2API g2_rtp_get_stream_remote(G2HRTP handle, int channel, int camera);
G2_DLLFUNC bool G2API g2_rtp_get_status(G2HRTP handle, int channel, G2DEVICE_STATUS* status);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_rtp_is_enable_multi_stream(G2HRTP handle, int channel, int camera);
G2_DLLFUNC bool G2API g2_rtp_is_enable_audio_in(G2HRTP handle, int channel, int camera);
G2_DLLFUNC bool G2API g2_rtp_is_enable_audio_out(G2HRTP handle, int channel, int camera);
G2_DLLFUNC bool G2API g2_rtp_is_contains_audio_streaming(G2HRTP handle, int channel, int camera);
G2_DLLFUNC bool G2API g2_rtp_is_contains_audio_capturing(G2HRTP handle, int channel, int camera);
G2_DLLFUNC bool G2API g2_rtp_is_contains_audio(G2HRTP handle, int channel);
G2_DLLFUNC bool G2API g2_rtp_is_audio_out_opening(G2HRTP handle, int channel, int camera);
G2_DLLFUNC bool G2API g2_rtp_is_support(G2HRTP handle, int channel, int query);

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_CLIENT_DLL_RTP_H_
