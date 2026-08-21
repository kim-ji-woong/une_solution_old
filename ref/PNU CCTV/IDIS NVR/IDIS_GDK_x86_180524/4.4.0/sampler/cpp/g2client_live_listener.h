// g2client_live_listener.h : header file
//

#ifndef _G2_CLIENT_DLL_SAMPLER_LIVE_LISTENER_H_
#define _G2_CLIENT_DLL_SAMPLER_LIVE_LISTENER_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_live.h>
#include <vector>

namespace client {

//////////////////////////////////////////////////////////////////////////

class g2live_listener
{
public:
    virtual void on_g2live_connected(G2HLIVE handle, int channel) = 0;
    virtual void on_g2live_disconnected(G2HLIVE handle, int channel, G2DISCONNECT_REASON::TYPE reason) = 0;
    virtual void on_g2live_receive_stream_channels(G2HLIVE handle, int channel, const std::vector<G2LIVE_CHANNEL_INFO>& bunch) = 0;
    virtual void on_g2live_receive_frame_data(G2HLIVE handle, int channel, const G2FRAME& frame) = 0;
    virtual void on_g2live_receive_text_in(G2HLIVE handle, int channel, const G2TEXT_IN& data) = 0;
    virtual void on_g2live_receive_camera_status(G2HLIVE handle, int channel, const std::vector<G2LIVE_CAMERA_STATUS>& bunch) = 0;
    virtual void on_g2live_receive_event(G2HLIVE handle, int channel, const G2EVENT_INFO& info) = 0;
    virtual void on_g2live_receive_ptz_menu(G2HLIVE handle, int channel, const G2LIVE_PTZ_MENU& menu) = 0;
    virtual void on_g2live_receive_ptz_preset(G2HLIVE handle, int channel, const G2LIVE_PTZ_PRESET& preset) = 0;
    virtual void on_g2live_receive_service_log_load(G2HLIVE handle, int channel, const G2SYSTEM_LOG& log) = 0;
    virtual void on_g2live_receive_service_log_load_end(G2HLIVE handle, int channel) = 0;
    virtual void on_g2live_receive_service_log_load_fail(G2HLIVE handle, int channel) = 0;
    virtual void on_g2live_receive_service_log_load_stop(G2HLIVE handle, int channel) = 0;
    virtual void on_g2live_receive_audio_out_not_available(G2HLIVE handle, int channel, const G2GUID& root) = 0;
    virtual void on_g2live_receive_notify_append_device(G2HLIVE handle, int channel, const G2GUID& root) = 0;
    virtual void on_g2live_receive_notify_remove_device(G2HLIVE handle, int channel, const G2GUID& root) = 0;
    virtual void on_g2live_receive_network_alarm_result(G2HLIVE handle, int channel, const G2GUID& root, const G2LIVE_NETWORK_ALARM_RESULT& result) = 0;
    virtual void on_g2live_audio_streaming_started(G2HLIVE handle, int channel, int channelext) = 0;
    virtual void on_g2live_audio_streaming_stopped(G2HLIVE handle, int channel, int channelext) = 0;
    virtual void on_g2live_audio_capturing_started(G2HLIVE handle, int channel, const G2GUID& root, int camera) = 0;
    virtual void on_g2live_audio_capturing_stopped(G2HLIVE handle, int channel, const G2GUID& root, int camera) = 0;
    virtual void on_g2live_probe_session_profile(G2HLIVE handle, int channel, const G2PROBE_SESSION_PROFILE& probe) = 0;
    virtual void on_g2live_receive_command_result_control_color_status(G2HLIVE handle, int channel, const G2GUID& cameraGUID, int camera, const G2LIVE_COMMAND_CONTROL_COLOR& control, const G2LIVE_COMMAND_CONTROL_COLOR_RANGE& range) = 0;
    virtual void on_g2live_receive_command_result_control_color(G2HLIVE handle, int channel, const G2GUID& cameraGUID, int camera, const G2LIVE_COMMAND_CONTROL_COLOR& control, G2LIVE_COMMAND_RESULT::TYPE result) = 0;
    virtual void on_g2live_receive_command_result_control_ptz_status(G2HLIVE handle, int channel, const G2GUID& cameraGUID, int camera, const G2LIVE_COMMAND_CONTROL_PTZ& control, const G2LIVE_COMMAND_CONTROL_PTZ_RANGE& range) = 0;
    virtual void on_g2live_receive_command_result_control_ptz(G2HLIVE handle, int channel, const G2GUID& cameraGUID, int camera, G2LIVE_COMMAND_RESULT::TYPE result) = 0;
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2_CLIENT_DLL_SAMPLER_LIVE_LISTENER_H_
