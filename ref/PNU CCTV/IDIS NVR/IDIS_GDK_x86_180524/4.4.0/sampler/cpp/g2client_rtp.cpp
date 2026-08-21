// g2client_rtp.cpp : implementation file
//

#include "stdafx.h"
#include "g2client_rtp.h"
#include "g2client_rtp_listener.h"

#include <assert.h>

#if defined(_WIN32)
#if defined(_WIN64)
#pragma comment(lib, "G2ClientGDKx64.lib")
#else
#pragma comment(lib, "G2ClientGDK.lib")
#endif
#pragma warning(disable : 4244 4267 4312 4800)
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

#define REGISTER_CALLBACK(function) \
    g2_rtp_register_callback(_handle, G2RTP_CALLBACK::function, function);

#define CALL_LISTENER(param) {  \
    g2rtp_listener* ptr = ((g2rtp*)(uparam))->_listener; \
    if (ptr) ptr->param;        \
}

//////////////////////////////////////////////////////////////////////////

g2rtp::g2rtp(void)
    : _handle(G2HNULL)
    , _listener(NULL)
{
    _handle = g2_rtp_initialize((G2UPARAM)(this));

    REGISTER_CALLBACK(on_rtsp_connected_device);
    REGISTER_CALLBACK(on_rtsp_connected);
    REGISTER_CALLBACK(on_rtsp_disconnected);
    REGISTER_CALLBACK(on_rtp_connected);
    REGISTER_CALLBACK(on_receive_frame_data);
    REGISTER_CALLBACK(on_receive_event);
    REGISTER_CALLBACK(on_receive_device_status);
    REGISTER_CALLBACK(on_receive_ptz_preset);
    REGISTER_CALLBACK(on_receive_ptz_menu);
    REGISTER_CALLBACK(on_receive_audio_out_not_available);
    REGISTER_CALLBACK(on_audio_streaming_started);
    REGISTER_CALLBACK(on_audio_streaming_stopped);
    REGISTER_CALLBACK(on_audio_capturing_started);
    REGISTER_CALLBACK(on_audio_capturing_stopped);
}

g2rtp::~g2rtp(void)
{
    g2_rtp_finalize(_handle);
}

//////////////////////////////////////////////////////////////////////////

void g2rtp::startup(int connections, const port_range_t& port_range, unsigned int count_buffering, G2HWND focus)
{
    assert(_handle != G2HNULL && "g2client_rtp is not initialized");
    g2_rtp_startup(_handle, connections, port_range.first, port_range.second, count_buffering, focus);
}

void g2rtp::cleanup(void)
{
    assert(_handle != G2HNULL && "g2client_rtp is not initialized");
    g2_rtp_cleanup(_handle);
}

void g2rtp::set_listener(g2rtp_listener* listener)
{
    _listener = listener;
}

//////////////////////////////////////////////////////////////////////////

int g2rtp::connect(const G2GUID& root, const G2CONNECT_OPTIONS* options /*= NULL*/, G2CONNECT_RES* res /*= NULL*/)
{
    assert(_handle != G2HNULL && "g2client_rtp is not initialized");
    return g2_rtp_connect(_handle, &root, options, res);
}

void g2rtp::disconnect(int channel)
{
    assert(_handle != G2HNULL && "g2client_rtp is not initialized");
    return g2_rtp_disconnect(_handle, channel);
}

bool g2rtp::is_connecting(int channel) const
{
    assert(_handle != G2HNULL && "g2client_rtp is not initialized");
    return g2_rtp_is_connecting(_handle, channel);
}

bool g2rtp::is_connected(int channel) const
{
    assert(_handle != G2HNULL && "g2client_rtp is not initialized");
    return g2_rtp_is_connected(_handle, channel);
}

bool g2rtp::is_disconnecting(int channel) const
{
    assert(_handle != G2HNULL && "g2client_rtp is not initialized");
    return g2_rtp_is_disconnecting(_handle, channel);
}

bool g2rtp::is_disconnected(int channel) const
{
    assert(_handle != G2HNULL && "g2client_rtp is not initialized");
    return g2_rtp_is_disconnected(_handle, channel);
}

bool g2rtp::is_disconnectable(int channel) const
{
    assert(_handle != G2HNULL && "g2client_rtp is not initialized");
    return g2_rtp_is_disconnectable(_handle, channel);
}

//////////////////////////////////////////////////////////////////////////

void g2rtp::set_incomming_port(const port_range_t& port_range)
{
    g2_rtp_set_incomming_port(_handle, port_range.first, port_range.second);
}

void g2rtp::set_buffering_count(int count)
{
    g2_rtp_set_buffering_count(_handle, count);
}

bool g2rtp::set_camera_list(int channel, unsigned int cameras, bool force /*= false*/)
{
    return g2_rtp_set_camera_list(_handle, channel, cameras, force);
}

bool g2rtp::set_audio_list(int channel, unsigned int audios, bool force /*= true*/)
{
    return g2_rtp_set_audio_list(_handle, channel, audios, force);
}

bool g2rtp::set_stream_id(int channel, int camera, int id)
{
    return g2_rtp_set_stream_id(_handle, channel, camera, id);
}

bool g2rtp::set_alarm_out(int channel, int id, bool on)
{
    return g2_rtp_set_alarm_out(_handle, channel, id, on);
}

bool g2rtp::set_camera_color(int channel, int camera, int type, int value)
{
    return g2_rtp_set_camera_color(_handle, channel, camera, type, value);
}

bool g2rtp::set_ptz_command(int channel, int camera, const G2LIVE_PTZ_COMMAND& command)
{
    return g2_rtp_set_ptz_command(_handle, channel, camera, &command);
}

bool g2rtp::set_ptz_preset(int channel, int camera, const G2LIVE_PTZ_PRESET& preset)
{
    return g2_rtp_set_ptz_preset(_handle, channel, camera, &preset);
}

bool g2rtp::set_enable_audio_streaming(int channel, int camera, bool enable)
{
    return g2_rtp_set_enable_audio_streaming(_handle, channel, camera, enable);
}

bool g2rtp::set_enable_audio_capturing(int channel, int camera, bool enable)
{
    return g2_rtp_set_enable_audio_capturing(_handle, channel, camera, enable);
}

bool g2rtp::set_disable_audio_streaming(int channel)
{
    return g2_rtp_set_disable_audio_streaming(_handle, channel);
}

bool g2rtp::set_disable_audio(int channel)
{
    return g2_rtp_set_disable_audio(_handle, channel);
}

//////////////////////////////////////////////////////////////////////////

bool g2rtp::request_alive_check(int channel)
{
    return g2_rtp_request_alive_check(_handle, channel);
}

bool g2rtp::request_ptz_menu(int channel, int camera)
{
    return g2_rtp_request_ptz_menu(_handle, channel, camera);
}

bool g2rtp::request_ptz_preset(int channel, int camera)
{
    return g2_rtp_request_ptz_preset(_handle, channel, camera);
}

void g2rtp::set_probe_session_profile(bool active)
{
    g2_rtp_set_probe_session_profile(_handle, active);
}

//////////////////////////////////////////////////////////////////////////

bool g2rtp::get_camera_list(int channel, unsigned int& cameras) const
{
    return g2_rtp_get_camera_list(_handle, channel, &cameras);
}

bool g2rtp::get_audio_list(int channel, unsigned int& audios) const
{
    return g2_rtp_get_audio_list(_handle, channel, &audios);
}

int g2rtp::get_stream_id(int channel, int camera) const
{
    return g2_rtp_get_stream_id(_handle, channel, camera);
}

unsigned int g2rtp::get_stream_remote(int channel, int camera) const
{
    return g2_rtp_get_stream_remote(_handle, channel, camera);
}

bool g2rtp::get_status(int channel, G2DEVICE_STATUS& status) const
{
    return g2_rtp_get_status(_handle, channel, &status);
}

//////////////////////////////////////////////////////////////////////////

bool g2rtp::is_enable_multi_stream(int channel, int camera) const
{
    return g2_rtp_is_enable_multi_stream(_handle, channel, camera);
}

bool g2rtp::is_enable_audio_in(int channel, int camera) const
{
    return g2_rtp_is_enable_audio_in(_handle, channel, camera);
}

bool g2rtp::is_enable_audio_out(int channel, int camera) const
{
    return g2_rtp_is_enable_audio_out(_handle, channel, camera);
}

bool g2rtp::is_contains_audio_streaming(int channel, int camera) const
{
    return g2_rtp_is_contains_audio_streaming(_handle, channel, camera);
}

bool g2rtp::is_contains_audio_capturing(int channel, int camera) const
{
    return g2_rtp_is_contains_audio_capturing(_handle, channel, camera);
}

bool g2rtp::is_contains_audio(int channel) const
{
    return g2_rtp_is_contains_audio(_handle, channel);
}

bool g2rtp::is_audio_out_opening(int channel, int camera) const
{
    return g2_rtp_is_audio_out_opening(_handle, channel, camera);
}

bool g2rtp::is_support(int channel, int query) const
{
    return g2_rtp_is_support(_handle, channel, query);
}

//////////////////////////////////////////////////////////////////////////

G2RESULT g2rtp::on_rtsp_connected_device(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    bool proceed = true;
    CALL_LISTENER(on_g2rtsp_connected_device(handle, wparam, proceed));
    return proceed ? G2TRUE : G2FALSE;
}

G2RESULT g2rtp::on_rtsp_connected(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    bool proceed = true;
    CALL_LISTENER(on_g2rtsp_connected(handle, wparam, proceed));
    return proceed ? G2TRUE : G2FALSE;
}

G2RESULT g2rtp::on_rtsp_disconnected(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2rtsp_disconnected(handle, wparam, (G2DISCONNECT_REASON::TYPE)lparam));
    return 1L;
}

G2RESULT g2rtp::on_rtp_connected(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2rtp_connected(handle, wparam));
    return 1L;
}

G2RESULT g2rtp::on_receive_frame_data(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2FRAME* data = (G2FRAME*)(lparam);
    CALL_LISTENER(on_g2rtp_receive_frame_data(handle, wparam, *data));
    return 1L;
}

G2RESULT g2rtp::on_receive_event(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2EVENT_INFO* ei = (G2EVENT_INFO*)(lparam);
    CALL_LISTENER(on_g2rtp_receive_event(handle, wparam, *ei));
    return 1L;
}

G2RESULT g2rtp::on_receive_device_status(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2DEVICE_STATUS* data = (G2DEVICE_STATUS*)(lparam);
    CALL_LISTENER(on_g2rtp_receive_device_status(handle, wparam, *data));
    return 1L;
}

G2RESULT g2rtp::on_receive_ptz_preset(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2LIVE_PTZ_PRESET* data = (G2LIVE_PTZ_PRESET*)(lparam);
    CALL_LISTENER(on_g2rtp_receive_ptz_preset(handle, wparam, *data));
    return 1L;
}

G2RESULT g2rtp::on_receive_ptz_menu(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2LIVE_PTZ_MENU* data = (G2LIVE_PTZ_MENU*)(lparam);
    CALL_LISTENER(on_g2rtp_receive_ptz_menu(handle, wparam, *data));
    return 1L;
}

G2RESULT g2rtp::on_receive_audio_out_not_available(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2rtp_receive_audio_out_not_available(handle, wparam));
    return 1L;
}

G2RESULT g2rtp::on_audio_streaming_started(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2rtp_audio_streaming_started(handle, wparam, lparam));
    return 1L;
}

G2RESULT g2rtp::on_audio_streaming_stopped(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2rtp_audio_streaming_stopped(handle, wparam, lparam));
    return 1L;
}

G2RESULT g2rtp::on_audio_capturing_started(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2rtp_audio_capturing_started(handle, wparam, lparam));
    return 1L;
}

G2RESULT g2rtp::on_audio_capturing_stopped(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2rtp_audio_capturing_stopped(handle, wparam, lparam));
    return 1L;
}

G2RESULT g2rtp::on_probe_session_profile(G2HRTP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2PROBE_SESSION_PROFILE* data = (G2PROBE_SESSION_PROFILE*)(lparam);
    CALL_LISTENER(on_g2rtp_probe_session_profile(handle, wparam, *data));
    return 1L;
}