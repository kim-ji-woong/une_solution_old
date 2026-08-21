// g2client_watch_listener.h : header file
//

#ifndef _G2_CLIENT_DLL_SAMPLER_WATCH_LISTENER_H_
#define _G2_CLIENT_DLL_SAMPLER_WATCH_LISTENER_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_watch.h>
#include <string>

namespace client {

//////////////////////////////////////////////////////////////////////////

class g2watch_listener
{
public:
    virtual void on_g2watch_connected(G2HWATCH handle, int channel) = 0;
    virtual void on_g2watch_disconnected(G2HWATCH handle, int channel, G2DISCONNECT_REASON::TYPE reason) = 0;
    virtual void on_g2watch_receive_frame_data(G2HWATCH handle, int channel, const G2FRAME& frame) = 0;
    virtual void on_g2watch_receive_event(G2HWATCH handle, int channel, const G2EVENT_INFO& ei) = 0;
    virtual void on_g2watch_receive_device_status(G2HWATCH handle, int channel, const G2DEVICE_STATUS& status) = 0;
    virtual void on_g2watch_receive_ptz_preset(G2HWATCH handle, int channel, int camera, const G2LIVE_PTZ_PRESET& preset) = 0;
    virtual void on_g2watch_receive_ptz_menu(G2HWATCH handle, int channel, int camera, const G2LIVE_PTZ_MENU& menu) = 0;
    virtual void on_g2watch_receive_camera_title_idr(G2HWATCH handle, int channel, int camera, const std::wstring& title) = 0;
    virtual void on_g2watch_receive_text_in(G2HWATCH handle, int channel, const G2TEXT_IN& data) = 0;
    virtual void on_g2watch_receive_network_camera_information(G2HWATCH handle, int channel) = 0;
    virtual void on_g2watch_receive_audio_out_not_available(G2HWATCH handle, int channel) = 0;
    virtual void on_g2watch_receive_command_result_control_color_status(G2HWATCH handle, int channel, int camera, const G2LIVE_COMMAND_CONTROL_COLOR& control, const G2LIVE_COMMAND_CONTROL_COLOR_RANGE& range) = 0;
    virtual void on_g2watch_receive_command_result_control_color(G2HWATCH handle, int channel, int camera, const G2LIVE_COMMAND_CONTROL_COLOR& control, G2LIVE_COMMAND_RESULT::TYPE result) = 0;
    virtual void on_g2watch_receive_command_result_control_ptz_status(G2HWATCH handle, int channel, int camera, const G2LIVE_COMMAND_CONTROL_PTZ& control, const G2LIVE_COMMAND_CONTROL_PTZ_RANGE& range) = 0;
    virtual void on_g2watch_receive_command_result_control_ptz(G2HWATCH handle, int channel, int camera, G2LIVE_COMMAND_RESULT::TYPE result) = 0;
    virtual void on_g2watch_receive_network_alarm_result(G2HWATCH handle, int channel, const G2LIVE_NETWORK_ALARM_RESULT& result) = 0;
    virtual void on_g2watch_receive_elevator_status_info_response(G2HWATCH handle, int channel, unsigned int seq_number) = 0;
    virtual void on_g2watch_receive_instant_recording_start(G2HWATCH handle, int channel, const G2PARAM_INSTANT_RECORDING_CHANNEL_STATUS& status) = 0;
    virtual void on_g2watch_receive_instant_recording_stop(G2HWATCH handle, int channel, G2INSTANT_RECORDING_RESULT::TYPE result) = 0;
    virtual void on_g2watch_receive_instant_recording_status(G2HWATCH handle, int channel, const G2PARAM_INSTANT_RECORDING_CHANNEL_STATUS& status) = 0;
    virtual void on_g2watch_audio_streaming_started(G2HWATCH handle, int channel, int camera) = 0;
    virtual void on_g2watch_audio_streaming_stopped(G2HWATCH handle, int channel, int camera) = 0;
    virtual void on_g2watch_audio_capturing_started(G2HWATCH handle, int channel, int camera) = 0;
    virtual void on_g2watch_audio_capturing_stopped(G2HWATCH handle, int channel, int camera) = 0;
    virtual void on_g2watch_probe_session_profile(G2HWATCH handle, int channel, const G2PROBE_SESSION_PROFILE& probe) = 0;
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2_CLIENT_DLL_SAMPLER_WATCH_LISTENER_H_
