// g2client_rtp.h : header file
//

#ifndef _G2_CLIENT_DLL_SAMPLER_RTP_H_
#define _G2_CLIENT_DLL_SAMPLER_RTP_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_rtp.h>
#include <utility>

namespace client {
    class g2rtp_listener;

//////////////////////////////////////////////////////////////////////////

class g2rtp
{
public:
    g2rtp(void);
    virtual ~g2rtp(void);

    typedef std::pair<unsigned short, unsigned short> port_range_t;

private:
    G2HRTP _handle;
    g2rtp_listener* _listener;

public:
    void startup(int connections, const port_range_t& port_range, unsigned int count_buffering, G2HWND focus);
    void cleanup(void);
    void set_listener(g2rtp_listener* listener);

    G2HRTP safe_handle(void) const { return this != NULL ? _handle : G2HNULL; }

public:
    int  connect(const G2GUID& root, const G2CONNECT_OPTIONS* options = NULL, G2CONNECT_RES* res = NULL);
    void disconnect(int channel);
    bool is_connecting(int channel) const;
    bool is_connected(int channel) const;
    bool is_disconnecting(int channel) const;
    bool is_disconnected(int channel) const;
    bool is_disconnectable(int channel) const;

public:
    void set_incomming_port(const port_range_t& port_range);
    void set_buffering_count(int count);
    bool set_camera_list(int channel, unsigned int cameras, bool force = false);
    bool set_audio_list(int channel, unsigned int audios, bool force = true);
    bool set_stream_id(int channel, int camera, int id);
    bool set_alarm_out(int channel, int id, bool on);
    bool set_camera_color(int channel, int camera, int type, int value);
    bool set_ptz_command(int channel, int camera, const G2LIVE_PTZ_COMMAND& command);
    bool set_ptz_preset(int channel, int camera, const G2LIVE_PTZ_PRESET& preset);
    bool set_enable_audio_streaming(int channel, int camera, bool enable);
    bool set_enable_audio_capturing(int channel, int camera, bool enable);
    bool set_disable_audio_streaming(int channel);
    bool set_disable_audio(int channel);
    void set_probe_session_profile(bool active);

public:
    bool request_alive_check(int channel);
    bool request_ptz_menu(int channel, int camera);
    bool request_ptz_preset(int channel, int camera);

public:
    bool get_camera_list(int channel, unsigned int& cameras) const;
    bool get_audio_list(int channel, unsigned int& audios) const;
    int  get_stream_id(int channel, int camera) const;
    unsigned int get_stream_remote(int channel, int camera) const;
    bool get_status(int channel, G2DEVICE_STATUS& status) const;

public:
    bool is_enable_multi_stream(int channel, int camera) const;
    bool is_enable_audio_in(int channel, int camera) const;
    bool is_enable_audio_out(int channel, int camera) const;
    bool is_contains_audio_streaming(int channel, int camera) const;
    bool is_contains_audio_capturing(int channel, int camera) const;
    bool is_contains_audio(int channel) const;
    bool is_audio_out_opening(int channel, int camera) const;
    bool is_support(int channel, int query) const;

protected:
    static G2RESULT G2CALLBACK on_rtsp_connected_device(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_rtsp_connected(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_rtsp_disconnected(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_rtp_connected(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_frame_data(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_event(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_device_status(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_ptz_preset(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_ptz_menu(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_audio_out_not_available(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_audio_streaming_started(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_audio_streaming_stopped(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_audio_capturing_started(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_audio_capturing_stopped(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_probe_session_profile(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2_CLIENT_DLL_SAMPLER_RTP_H_
