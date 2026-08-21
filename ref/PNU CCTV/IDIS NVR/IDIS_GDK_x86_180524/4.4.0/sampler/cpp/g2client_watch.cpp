// g2client_watch.cpp : implementation file
//

#include "stdafx.h"
#include "g2client_watch.h"
#include "g2client_watch_listener.h"

#include <boost/foreach.hpp>
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
    g2_watch_register_callback(_handle, G2WATCH_CALLBACK::function, function);

#define CALL_LISTENER(param) {  \
    g2watch_listener* ptr = ((g2watch*)(uparam))->_listener; \
    if (ptr) ptr->param;        \
}

//////////////////////////////////////////////////////////////////////////

g2watch::g2watch(void)
    : _handle(G2HNULL)
    , _listener(NULL)
{
    _handle = g2_watch_initialize((G2UPARAM)(this));

    REGISTER_CALLBACK(on_connected);
    REGISTER_CALLBACK(on_disconnected);
    REGISTER_CALLBACK(on_receive_frame_data);
    REGISTER_CALLBACK(on_receive_event);
    REGISTER_CALLBACK(on_receive_device_status);
    REGISTER_CALLBACK(on_receive_ptz_preset);
    REGISTER_CALLBACK(on_receive_ptz_menu);
    REGISTER_CALLBACK(on_receive_camera_title_idr);
    REGISTER_CALLBACK(on_receive_text_in);
    REGISTER_CALLBACK(on_receive_network_camera_information);
    REGISTER_CALLBACK(on_receive_audio_out_not_available);
    REGISTER_CALLBACK(on_receive_command_result_control_color_status);
    REGISTER_CALLBACK(on_receive_command_result_control_color);
    REGISTER_CALLBACK(on_receive_command_result_control_ptz_status);
    REGISTER_CALLBACK(on_receive_command_result_control_ptz);
    REGISTER_CALLBACK(on_receive_network_alarm_result);
    REGISTER_CALLBACK(on_receive_elevator_status_info_response);
    REGISTER_CALLBACK(on_receive_instant_recording_start);
    REGISTER_CALLBACK(on_receive_instant_recording_stop);
    REGISTER_CALLBACK(on_receive_instant_recording_status);
    REGISTER_CALLBACK(on_audio_streaming_started);
    REGISTER_CALLBACK(on_audio_streaming_stopped);
    REGISTER_CALLBACK(on_audio_capturing_started);
    REGISTER_CALLBACK(on_audio_capturing_stopped);
}

g2watch::~g2watch(void)
{
    g2_watch_finalize(_handle);
}

//////////////////////////////////////////////////////////////////////////

void g2watch::startup(int connections, G2HWND focus)
{
    assert(_handle != G2HNULL && "g2client_watch is not initialized");
    g2_watch_startup(_handle, connections, focus);
}

void g2watch::cleanup(void)
{
    assert(_handle != G2HNULL && "g2client_watch is not initialized");
    g2_watch_cleanup(_handle);
}

void g2watch::set_listener(g2watch_listener* listener)
{
    _listener = listener;
}

bool g2watch::set_focus_G2HWND(G2HWND focus)
{
    return g2_watch_set_focus_G2HWND(_handle, focus);
}

//////////////////////////////////////////////////////////////////////////

int g2watch::connect(const G2GUID& root, const G2CONNECT_OPTIONS* options /*= NULL*/, G2CONNECT_RES* res /*= NULL*/)
{
    assert(_handle != G2HNULL && "g2client_watch is not initialized");
    return g2_watch_connect(_handle, &root, options, res);
}

int g2watch::connect_ras(const G2NETWORK_INFO& ni, const G2CONNECT_OPTIONS* options /*= NULL*/, G2CONNECT_RES* res /*= NULL*/)
{
    assert(_handle != G2HNULL && "g2client_watch is not initialized");
    return g2_watch_connect_ras(_handle, &ni, options, res);
}

int g2watch::connect_ras_event(const G2NETWORK_INFO& ni, const G2CONNECT_OPTIONS* options /*= NULL*/, G2CONNECT_RES* res /*= NULL*/)
{
    assert(_handle != G2HNULL && "g2client_watch is not initialized");
    return g2_watch_connect_ras_event(_handle, &ni, options, res);
}

void g2watch::disconnect(int channel)
{
    assert(_handle != G2HNULL && "g2client_watch is not initialized");
    return g2_watch_disconnect(_handle, channel);
}

bool g2watch::is_connecting(int channel) const
{
    assert(_handle != G2HNULL && "g2client_watch is not initialized");
    return g2_watch_is_connecting(_handle, channel);
}

bool g2watch::is_connected(int channel) const
{
    assert(_handle != G2HNULL && "g2client_watch is not initialized");
    return g2_watch_is_connected(_handle, channel);
}

bool g2watch::is_disconnecting(int channel) const
{
    assert(_handle != G2HNULL && "g2client_watch is not initialized");
    return g2_watch_is_disconnecting(_handle, channel);
}

bool g2watch::is_disconnected(int channel) const
{
    assert(_handle != G2HNULL && "g2client_watch is not initialized");
    return g2_watch_is_disconnected(_handle, channel);
}

bool g2watch::is_disconnectable(int channel) const
{
    assert(_handle != G2HNULL && "g2client_watch is not initialized");
    return g2_watch_is_disconnectable(_handle, channel);
}

//////////////////////////////////////////////////////////////////////////

bool g2watch::set_camera_list(int channel, const std::set<int>& channels, bool force)
{
    G2CHANNEL_SET chs;
    std::copy(channels.begin(), channels.end(), chs._channels);
    chs._len = (unsigned int)std::min<size_t>(channels.size(), G2CHANNEL_SET::MAX_CHANNEL_COUNT);
    return g2_watch_set_camera_channelset(_handle, channel, &chs, force);
}

bool g2watch::set_camera_stream_set(int channel, const std::set<std::pair<int, int> >& streams, bool force)
{
    G2CHANNEL_STREAM_SET chs = { 0 };
    int i = 0;
    typedef std::pair<int, int> CHANNEL_STREAM;
    BOOST_FOREACH(const CHANNEL_STREAM& var, streams) {
        if (i < G2CHANNEL_STREAM_SET::MAX_STREAM_COUNT) {
            G2CHANNEL_STREAM& s = chs._streams[i];
            s._channel = var.first;
            s._stream = var.second;
        }
        i = i + 1;
    }
    chs._len = i;
    return g2_watch_set_camera_stream_set(_handle, channel, &chs, force);
}

bool g2watch::set_camera_stream_set(int channel, const std::set<std::pair<int, int> >& streams, const std::set<std::pair<int, int> >& onetime_streams)
{
    G2CHANNEL_STREAM_SET chs = { 0 };
    G2CHANNEL_STREAM_SET ochs = { 0 };
    int i = 0;
    typedef std::pair<int, int> CHANNEL_STREAM;
    BOOST_FOREACH(const CHANNEL_STREAM& var, streams) {
        if (i < G2CHANNEL_STREAM_SET::MAX_STREAM_COUNT) {
            G2CHANNEL_STREAM& s = chs._streams[i];
            s._channel = var.first;
            s._stream = var.second;
        }
        i = i + 1;
    }
    chs._len = i;
    i = 0;
    BOOST_FOREACH(const CHANNEL_STREAM& var, onetime_streams) {
        if (i < G2CHANNEL_STREAM_SET::MAX_STREAM_COUNT) {
            G2CHANNEL_STREAM& s = ochs._streams[i];
            s._channel = var.first;
            s._stream = var.second;
        }
        i = i + 1;
    }
    ochs._len = i;
    return g2_watch_set_camera_stream_set_onetime(_handle, channel, &chs, &ochs);
}

bool g2watch::set_audio_list(int channel, const std::set<int>& channels, bool force)
{
    G2CHANNEL_SET chs;
    std::copy(channels.begin(), channels.end(), chs._channels);
    chs._len = (unsigned int)std::min<size_t>(channels.size(), G2CHANNEL_SET::MAX_CHANNEL_COUNT);
    return g2_watch_set_audio_channelset(_handle, channel, &chs, force);
}
bool g2watch::set_stream_id(int channel, int camera, int id)
{
    return g2_watch_set_stream_id(_handle, channel, camera, id);
}

bool g2watch::set_alarm_out(int channel, int id, bool on)
{
    return g2_watch_set_alarm_out(_handle, channel, id, on);
}

bool g2watch::set_camera_color(int channel, int camera, int type, int value)
{
    return g2_watch_set_camera_color(_handle, channel, camera, type, value);
}

bool g2watch::set_ptz_command(int channel, int camera, const G2LIVE_PTZ_COMMAND& command)
{
    return g2_watch_set_ptz_command(_handle, channel, camera, &command);
}

bool g2watch::set_ptz_preset(int channel, int camera, const G2LIVE_PTZ_PRESET& preset)
{
    return g2_watch_set_ptz_preset(_handle, channel, camera, &preset);
}

bool g2watch::set_network_alarm(int channel, const G2LIVE_NETWORK_ALARM_INFO& info)
{
    return g2_watch_set_network_alarm(_handle, channel, &info);
}

bool g2watch::set_enable_audio_streaming(int channel, int camera, bool enable)
{
    return g2_watch_set_enable_audio_streaming(_handle, channel, camera, enable);
}

bool g2watch::set_enable_audio_capturing(int channel, int camera, bool enable)
{
    return g2_watch_set_enable_audio_capturing(_handle, channel, camera, enable);
}

bool g2watch::set_disable_audio_streaming(int channel)
{
    return g2_watch_set_disable_audio_streaming(_handle, channel);
}

bool g2watch::set_disable_audio(int channel)
{
    return g2_watch_set_disable_audio(_handle, channel);
}

void g2watch::set_probe_session_profile(bool active)
{
    g2_watch_set_probe_session_profile(_handle, active);
}

//////////////////////////////////////////////////////////////////////////

bool g2watch::request_alive_check(int channel)
{
    return g2_watch_request_alive_check(_handle, channel);
}

bool g2watch::request_device_status(int channel)
{
    return g2_watch_request_device_status(_handle, channel);
}

bool g2watch::request_ptz_menu(int channel, int camera)
{
    return g2_watch_request_ptz_menu(_handle, channel, camera);
}

bool g2watch::request_ptz_preset(int channel, int camera)
{
    return g2_watch_request_ptz_preset(_handle, channel, camera);
}

bool g2watch::request_stream_channel_control(int channel, int camera, int seq, signed char* buf, int len)
{
    return g2_watch_request_stream_channel_control(_handle, channel, camera, seq, buf, len);
}

bool g2watch::request_instant_recording_start(int channel, std::vector<G2INSTANT_RECORD_SETTING> setting)
{
    G2INSTANT_RECORD_SETTING_LIST list;
    list._len = setting.size();
    list._list = &setting[0];
    return g2_watch_request_instant_recording_start(_handle, channel, &list);
}

bool g2watch::request_instant_recording_stop(int channel, std::set<int> channels)
{
    G2CHANNEL_SET chs;
    std::copy(channels.begin(), channels.end(), chs._channels);
    chs._len = (unsigned int)std::min<size_t>(channels.size(), G2CHANNEL_SET::MAX_CHANNEL_COUNT);
    return g2_watch_request_instant_recording_stop(_handle, channel, &chs);
}

bool g2watch::request_instant_recording_status(int channel, std::set<int> channels)
{
    G2CHANNEL_SET chs;
    std::copy(channels.begin(), channels.end(), chs._channels);
    chs._len = (unsigned int)std::min<size_t>(channels.size(), G2CHANNEL_SET::MAX_CHANNEL_COUNT);
    return g2_watch_request_instant_recording_status(_handle, channel, &chs);
}

//////////////////////////////////////////////////////////////////////////

bool g2watch::send_command_control_color_status(int channel, int camera)
{
    return g2_watch_send_command_control_color_status(_handle, channel, camera);
}

bool g2watch::send_command_control_color(int channel, int camera, const G2LIVE_COMMAND_CONTROL_COLOR* control)
{
    return g2_watch_send_command_control_color(_handle, channel, camera, control);
}

//////////////////////////////////////////////////////////////////////////

bool g2watch::send_command_control_ptz_status(int channel, int camera)
{
    return g2_watch_send_command_control_ptz_status(_handle, channel, camera);
}

bool g2watch::send_command_control_ptz_status_relative(int channel, int camera)
{
    return g2_watch_send_command_control_ptz_status_relative(_handle, channel, camera);
}

bool g2watch::send_command_control_ptz_status_relative_IDIS(int channel, int camera)
{
	return g2_watch_send_command_control_ptz_status_relative_IDIS(_handle, channel, camera);
}

bool g2watch::send_command_control_ptz(int channel, int camera, const G2LIVE_COMMAND_CONTROL_PTZ* control)
{
    return g2_watch_send_command_control_ptz(_handle, channel, camera, control);
}

bool g2watch::send_command_control_ptz_IDIS(int channel, int camera, const G2LIVE_COMMAND_CONTROL_PTZ_STATUS* status, bool req_result)
{
	return g2_watch_send_command_control_ptz_IDIS(_handle, channel, camera, status, req_result);
}

bool g2watch::send_network_alarm_info(int channel, const G2LIVE_NETWORK_ALARM_INFO* info)
{
    return g2_watch_send_network_alarm_info(_handle, channel, info);
}

bool g2watch::send_elevator_status_info(int channel, const G2LIVE_ELEVATOR_STATUS_INFO* info)
{
    return g2_watch_send_elevator_status_info(_handle, channel, info);
}

//////////////////////////////////////////////////////////////////////////

bool g2watch::get_server_network_info(int channel, G2SERVER_NETWORK_INFO& ni) const
{
    return g2_watch_get_server_network_info(_handle, channel, &ni);
}

bool g2watch::get_product_info(int channel, G2_PRODUCT_INFO& pi) const
{
    return g2_watch_get_product_info(_handle, channel, &pi);
}

bool g2watch::get_remote_watch_caps(int channel, G2_PRODUCT_INFO_CAPS::REMOTE_WATCH& caps) const
{
    return g2_watch_get_remote_watch_caps(_handle, channel, &caps);
}

bool g2watch::get_authority(int channel, G2RAS_AUTHORITY& auth) const
{
    return g2_watch_get_authority(_handle, channel, &auth);
}

bool g2watch::get_camera_list(int channel, std::set<int>& channels) const
{
    G2CHANNEL_SET chs = { 0 };
    if (g2_watch_get_camera_channelset(_handle, channel, &chs)) {
        if (chs._len > 0) {
            std::set<int>(chs._channels, chs._channels + chs._len).swap(channels);
        }
        return true;
    }
    return false;
}

bool g2watch::get_camera_stream_set(int channel, std::set<std::pair<int, int> >& streams) const
{
    G2CHANNEL_STREAM_SET chs = { 0 };
    bool res = false;
    if (streams.empty() != true) streams.clear();
    if (g2_watch_get_camera_stream_set(_handle, channel, &chs)) {
        for (unsigned int i = 0; i < chs._len; ++i) {
            const G2CHANNEL_STREAM& s = chs._streams[i];
            streams.insert(std::pair<int, int>(s._channel, s._stream));
        }
        res = true;
    }
    return res;
}

bool g2watch::get_camera_stream_set(int channel, int camera, std::set<std::pair<int, int> >& streams) const
{
    G2CHANNEL_STREAM_SET chs = { 0 };
    bool res = false;
    if (streams.empty() != true) streams.clear();
    if (g2_watch_get_camera_stream(_handle, channel, camera, &chs)) {
        for (unsigned int i = 0; i < chs._len; ++i) {
            const G2CHANNEL_STREAM& s = chs._streams[i];
            streams.insert(std::pair<int, int>(s._channel, s._stream));
        }
        res = true;
    }
    return res;
}

bool g2watch::get_audio_list(int channel, std::set<int>& channels) const
{
    G2CHANNEL_SET chs = { 0 };
    if (g2_watch_get_audio_channelset(_handle, channel, &chs)) {
        if (chs._len > 0) {
            std::set<int>(chs._channels, chs._channels + chs._len).swap(channels);
        }
        return true;
    }
    return false;
}

int g2watch::get_stream_id(int channel, int camera) const
{
    return g2_watch_get_stream_id(_handle, channel, camera);
}

unsigned int g2watch::get_stream_remote(int channel, int camera) const
{
    return g2_watch_get_stream_remote(_handle, channel, camera);
}

bool g2watch::get_status(int channel, G2DEVICE_STATUS& status) const
{
    return g2_watch_get_status(_handle, channel, &status);
}

bool g2watch::get_status_stream_info(int channel, int camera, int stream_id, G2DEVICE_STATUS_STREAM_INFO& status) const
{
    return g2_watch_get_status_stream_info(_handle, channel, camera, stream_id, &status);
}

bool g2watch::get_status_ip_camera_info(int channel, int camera, G2DEVICE_STATUS_IP_CAMERA_INFO& status) const
{
    return g2_watch_get_status_ip_camera_info(_handle, channel, camera, &status);
}

bool g2watch::get_status_ptz_advanced_info(int channel, int camera, G2DEVICE_STATUS_PTZ_ADVANCED_INFO& status) const
{
    return g2_watch_get_status_ptz_advanced_info(_handle, channel, camera, &status);
}

int g2watch::get_status_ptz(int channel, int camera) const
{
    return g2_watch_get_status_ptz(_handle, channel, camera);
}

unsigned int g2watch::get_status_ptz_function(int channel, int camera) const
{
    return g2_watch_get_status_ptz_function(_handle, channel, camera);
}

bool g2watch::get_status_command_control_color(int channel, int camera, G2LIVE_COMMAND_CONTROL_COLOR* control, G2LIVE_COMMAND_CONTROL_COLOR_RANGE* range) const
{
    return g2_watch_get_status_command_control_color(_handle, channel, camera, control, range);
}

//////////////////////////////////////////////////////////////////////////

bool g2watch::is_enable_multi_stream(int channel, int camera) const
{
    return g2_watch_is_enable_multi_stream(_handle, channel, camera);
}

bool g2watch::is_enable_command_control_color(int channel, int camera) const
{
    return g2_watch_is_enable_command_control_color(_handle, channel, camera);
}

bool g2watch::is_enable_command_control_ptz(int channel, int camera) const
{
    return g2_watch_is_enable_command_control_ptz(_handle, channel, camera);
}

bool g2watch::is_enable_command_control_ptz_relative(int channel, int camera) const
{
    return g2_watch_is_enable_command_control_ptz_relative(_handle, channel, camera);
}

bool g2watch::is_enable_command_control_ptz_relative_IDIS(int channel, int camera) const
{
	return g2_watch_is_enable_command_control_ptz_relative_IDIS(_handle, channel, camera);
}

bool g2watch::is_enable_command_control_ptz_relative_IDIS_one_click_move(int channel, int camera) const
{
	return g2_watch_is_enable_command_control_ptz_relative_IDIS_one_click_move(_handle, channel, camera);
}

bool g2watch::is_enable_audio_in(int channel, int camera) const
{
    return g2_watch_is_enable_audio_in(_handle, channel, camera);
}

bool g2watch::is_enable_audio_out(int channel, int camera) const
{
    return g2_watch_is_enable_audio_out(_handle, channel, camera);
}

bool g2watch::is_contains_audio_streaming(int channel, int camera) const
{
    return g2_watch_is_contains_audio_streaming(_handle, channel, camera);
}

bool g2watch::is_contains_audio_capturing(int channel, int camera) const
{
    return g2_watch_is_contains_audio_capturing(_handle, channel, camera);
}

bool g2watch::is_contains_audio_capturing(int channel) const
{
    return g2_watch_is_contains_audio_capturing_any(_handle, channel);
}

bool g2watch::is_contains_audio(int channel) const
{
    return g2_watch_is_contains_audio(_handle, channel);
}

bool g2watch::is_audio_out_opening(int channel, int camera) const
{
    return g2_watch_is_audio_out_opening(_handle, channel, camera);
}

bool g2watch::is_support(int channel, G2LIVE_SUPPORT::QUERY query) const
{
    return g2_watch_is_support(_handle, channel, query);
}

bool g2watch::is_enable_audio_device_record(void) const
{
    return g2_watch_is_enable_audio_device_record();
}

bool g2watch::is_enable_audio_device_play(G2HWND focus) const
{
    return g2_watch_is_enable_audio_device_play(focus);
}

bool g2watch::is_probe_perofmance(void) const
{
    return g2_watch_is_probe_session_profile(_handle);
}

//////////////////////////////////////////////////////////////////////////

G2RESULT g2watch::on_connected(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2watch_connected(handle, wparam));
    return 1L;
}

G2RESULT g2watch::on_disconnected(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2watch_disconnected(handle, wparam, (G2DISCONNECT_REASON::TYPE)lparam));
    return 1L;
}

G2RESULT g2watch::on_receive_frame_data(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2FRAME* f = (G2FRAME*)lparam;
    CALL_LISTENER(on_g2watch_receive_frame_data(handle, wparam, *f));
    return 1L;
}

G2RESULT g2watch::on_receive_event(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2EVENT_INFO* ei = (G2EVENT_INFO*)lparam;
    CALL_LISTENER(on_g2watch_receive_event(handle, wparam, *ei));
    return 1L;
}

G2RESULT g2watch::on_receive_device_status(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2DEVICE_STATUS* s = (G2DEVICE_STATUS*)lparam;
    CALL_LISTENER(on_g2watch_receive_device_status(handle, wparam, *s));
    return 1L;
}

G2RESULT g2watch::on_receive_ptz_preset(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2LIVE_PARAM_PTZ_PRESET* p = (G2LIVE_PARAM_PTZ_PRESET*)lparam;
    CALL_LISTENER(on_g2watch_receive_ptz_preset(handle, wparam, p->_camera, p->_data));
    return 1L;
}

G2RESULT g2watch::on_receive_ptz_menu(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2LIVE_PARAM_PTZ_MENU* p = (G2LIVE_PARAM_PTZ_MENU*)lparam;
    CALL_LISTENER(on_g2watch_receive_ptz_menu(handle, wparam, p->_camera, p->_data));
    return 1L;
}

G2RESULT g2watch::on_receive_camera_title_idr(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2DEVICE_NAME* p = (G2DEVICE_NAME*)lparam;
    CALL_LISTENER(on_g2watch_receive_camera_title_idr(handle, wparam, p->_number, std::wstring(p->_name._string)));
    return 1L;
}

G2RESULT g2watch::on_receive_text_in(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2TEXT_IN* p = (G2TEXT_IN*)lparam;
    CALL_LISTENER(on_g2watch_receive_text_in(handle, wparam, *p));
    return 1L;
}

G2RESULT g2watch::on_receive_network_camera_information(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2watch_receive_network_camera_information(handle, wparam));
    return 1L;
}

G2RESULT g2watch::on_receive_audio_out_not_available(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2watch_receive_audio_out_not_available(handle, wparam));
    return 1L;
}

G2RESULT g2watch::on_receive_command_result_control_color_status(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2LIVE_COMMAND_CONTROL_COLOR_STATUS* s = (G2LIVE_COMMAND_CONTROL_COLOR_STATUS*)lparam;
    CALL_LISTENER(on_g2watch_receive_command_result_control_color_status(handle, wparam, s->_camera, s->_control, s->_range));
    return 1L;
}

G2RESULT g2watch::on_receive_command_result_control_color(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2LIVE_COMMAND_CONTROL_COLOR_RESULT* r = (G2LIVE_COMMAND_CONTROL_COLOR_RESULT*)lparam;
    CALL_LISTENER(on_g2watch_receive_command_result_control_color(handle, wparam, r->_camera, r->_control, (G2LIVE_COMMAND_RESULT::TYPE)r->_result));
    return 1L;
}

G2RESULT g2watch::on_receive_command_result_control_ptz_status(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2LIVE_COMMAND_CONTROL_PTZ_STATUS* s = (G2LIVE_COMMAND_CONTROL_PTZ_STATUS*)lparam;
    CALL_LISTENER(on_g2watch_receive_command_result_control_ptz_status(handle, wparam, s->_camera, s->_control, s->_range));
    return 1L;
}

G2RESULT g2watch::on_receive_command_result_control_ptz(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2LIVE_COMMAND_CONTROL_PTZ_RESULT* r = (G2LIVE_COMMAND_CONTROL_PTZ_RESULT*)lparam;
    CALL_LISTENER(on_g2watch_receive_command_result_control_ptz(handle, wparam, r->_camera, (G2LIVE_COMMAND_RESULT::TYPE)r->_result));
    return 1L;
}

G2RESULT g2watch::on_receive_network_alarm_result(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2LIVE_NETWORK_ALARM_RESULT* r = (G2LIVE_NETWORK_ALARM_RESULT*)lparam;
    CALL_LISTENER(on_g2watch_receive_network_alarm_result(handle, wparam, *r));
    return 1L;
}

G2RESULT g2watch::on_receive_elevator_status_info_response(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2watch_receive_elevator_status_info_response(handle, wparam, lparam));
    return 1L;
}

G2RESULT g2watch::on_receive_instant_recording_start(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2PARAM_INSTANT_RECORDING_CHANNEL_STATUS* s = (G2PARAM_INSTANT_RECORDING_CHANNEL_STATUS*)lparam;
    CALL_LISTENER(on_g2watch_receive_instant_recording_start(handle, wparam, *s));
    return 1L;
}

G2RESULT g2watch::on_receive_instant_recording_stop(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2watch_receive_instant_recording_stop(handle, wparam, (G2INSTANT_RECORDING_RESULT::TYPE)lparam));
    return 1L;
}

G2RESULT g2watch::on_receive_instant_recording_status(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2PARAM_INSTANT_RECORDING_CHANNEL_STATUS* s = (G2PARAM_INSTANT_RECORDING_CHANNEL_STATUS*)lparam;
    CALL_LISTENER(on_g2watch_receive_instant_recording_status(handle, wparam, *s));
    return 1L;
}

G2RESULT g2watch::on_audio_streaming_started(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2watch_audio_streaming_started(handle, wparam, lparam));
    return 1L;
}

G2RESULT g2watch::on_audio_streaming_stopped(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2watch_audio_streaming_stopped(handle, wparam, lparam));
    return 1L;
}

G2RESULT g2watch::on_audio_capturing_started(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2watch_audio_capturing_started(handle, wparam, lparam));
    return 1L;
}

G2RESULT g2watch::on_audio_capturing_stopped(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2watch_audio_capturing_stopped(handle, wparam, lparam));
    return 1L;
}

G2RESULT g2watch::on_probe_session_profile(G2HWATCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2PROBE_SESSION_PROFILE* data = (G2PROBE_SESSION_PROFILE*)(lparam);
    CALL_LISTENER(on_g2watch_probe_session_profile(handle, wparam, *data));
    return 1L;
}
