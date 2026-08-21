// g2client_rtp_listener.h : header file
//

#ifndef _G2_CLIENT_DLL_SAMPLER_RTP_LISTENER_H_
#define _G2_CLIENT_DLL_SAMPLER_RTP_LISTENER_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_rtp.h>
#include <string>

namespace client {

//////////////////////////////////////////////////////////////////////////

class g2rtp_listener
{
public:
    virtual void on_g2rtsp_connected_device(G2HRTP handle, int channel, bool& proceed) = 0;
    virtual void on_g2rtsp_connected(G2HRTP handle, int channel, bool& proceed) = 0;
    virtual void on_g2rtsp_disconnected(G2HRTP handle, int channel, G2DISCONNECT_REASON::TYPE reason) = 0;
    virtual void on_g2rtp_connected(G2HRTP handle, int channel) = 0;
    virtual void on_g2rtp_rtp_disconnected(G2HRTP handle, int channel, G2DISCONNECT_REASON::TYPE reason) = 0;
    virtual void on_g2rtp_receive_frame_data(G2HRTP handle, int channel, const G2FRAME& frame) = 0;
    virtual void on_g2rtp_receive_event(G2HRTP handle, int channel, const G2EVENT_INFO& info) = 0;
    virtual void on_g2rtp_receive_device_status(G2HRTP handle, int channel, const G2DEVICE_STATUS& status) = 0;
    virtual void on_g2rtp_receive_ptz_preset(G2HRTP handle, int channel, const G2LIVE_PTZ_PRESET& preset) = 0;
    virtual void on_g2rtp_receive_ptz_menu(G2HRTP handle, int channel, const G2LIVE_PTZ_MENU& menu) = 0;
    virtual void on_g2rtp_receive_audio_out_not_available(G2HRTP handle, int channel) = 0;
    virtual void on_g2rtp_audio_streaming_started(G2HRTP handle, int channel, int camera) = 0;
    virtual void on_g2rtp_audio_streaming_stopped(G2HRTP handle, int channel, int camera) = 0;
    virtual void on_g2rtp_audio_capturing_started(G2HRTP handle, int channel, int camera) = 0;
    virtual void on_g2rtp_audio_capturing_stopped(G2HRTP handle, int channel, int camera) = 0;
    virtual void on_g2rtp_probe_session_profile(G2HRTP handle, int channel, const G2PROBE_SESSION_PROFILE& probe) = 0;
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2_CLIENT_DLL_SAMPLER_RTP_LISTENER_H_
