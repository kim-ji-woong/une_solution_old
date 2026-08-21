// g2client_live.cpp : implementaton file
//

#include "stdafx.h"
#include "g2client_live.h"
#include "g2client_live_listener.h"

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
    g2_live_register_callback(_handle, G2LIVE_CALLBACK::function, function);

#define CALL_LISTENER(param) {  \
    g2live_listener* ptr = ((g2live*)(uparam))->_listener; \
    if (ptr) ptr->param;        \
}

//////////////////////////////////////////////////////////////////////////

g2live::g2live(void)
    : _handle(G2HNULL)
    , _listener(NULL)
{
    _handle = g2_live_initialize((G2UPARAM)(this));

    REGISTER_CALLBACK(on_connected);
    REGISTER_CALLBACK(on_disconnected);
    REGISTER_CALLBACK(on_receive_stream_channels);
    REGISTER_CALLBACK(on_receive_frame_data);
    REGISTER_CALLBACK(on_receive_text_in);
    REGISTER_CALLBACK(on_receive_camera_status);
    REGISTER_CALLBACK(on_receive_event);
    REGISTER_CALLBACK(on_receive_ptz_menu);
    REGISTER_CALLBACK(on_receive_ptz_preset);
    REGISTER_CALLBACK(on_receive_service_log_load);
    REGISTER_CALLBACK(on_receive_service_log_load_end);
    REGISTER_CALLBACK(on_receive_service_log_load_fail);
    REGISTER_CALLBACK(on_receive_service_log_load_stop);
    REGISTER_CALLBACK(on_receive_audio_out_not_available);
    REGISTER_CALLBACK(on_receive_notify_append_device);
    REGISTER_CALLBACK(on_receive_notify_remove_device);
    REGISTER_CALLBACK(on_receive_network_alarm_result);
    REGISTER_CALLBACK(on_audio_streaming_started);
    REGISTER_CALLBACK(on_audio_streaming_stopped);
    REGISTER_CALLBACK(on_audio_capturing_started);
    REGISTER_CALLBACK(on_audio_capturing_stopped);
    REGISTER_CALLBACK(on_receive_command_result_control_color_status);
    REGISTER_CALLBACK(on_receive_command_result_control_color);
    REGISTER_CALLBACK(on_receive_command_result_control_ptz_status);
    REGISTER_CALLBACK(on_receive_command_result_control_ptz);
}

g2live::~g2live(void)
{
    g2_live_finalize(_handle);
}

//////////////////////////////////////////////////////////////////////////

void g2live::startup(int connections, G2HWND focus)
{
    assert(_handle != G2HNULL && "g2client_live is not initialized");
    g2_live_startup(_handle, connections, focus);
}

void g2live::cleanup(void)
{
    assert(_handle != G2HNULL && "g2client_live is not initialized");
    g2_live_cleanup(_handle);
}

void g2live::set_listener(g2live_listener* listener)
{
    _listener = listener;
}

//////////////////////////////////////////////////////////////////////////

int g2live::connect(const G2GUID& service, const G2CONNECT_OPTIONS* options /*= NULL*/, G2CONNECT_RES* res /*= NULL*/)
{
    assert(_handle != G2HNULL && "g2client_live is not initialized");
    return g2_live_connect(_handle, &service, options, res);
}

void g2live::disconnect(int channel)
{
    assert(_handle != G2HNULL && "g2client_live is not initialized");
    return g2_live_disconnect(_handle, channel);
}

bool g2live::is_connecting(int channel) const
{
    assert(_handle != G2HNULL && "g2client_live is not initialized");
    return g2_live_is_connecting(_handle, channel);
}

bool g2live::is_connected(int channel) const
{
    assert(_handle != G2HNULL && "g2client_live is not initialized");
    return g2_live_is_connected(_handle, channel);
}

bool g2live::is_disconnecting(int channel) const
{
    assert(_handle != G2HNULL && "g2client_live is not initialized");
    return g2_live_is_disconnecting(_handle, channel);
}

bool g2live::is_disconnected(int channel) const
{
    assert(_handle != G2HNULL && "g2client_live is not initialized");
    return g2_live_is_disconnected(_handle, channel);
}

bool g2live::is_disconnectable(int channel) const
{
    assert(_handle != G2HNULL && "g2client_live is not initialized");
    return g2_live_is_disconnectable(_handle, channel);
}

//////////////////////////////////////////////////////////////////////////

bool g2live::set_camera_list(int channel, const std::set<int>& channels, bool force)
{
    G2CHANNEL_SET channelset;
    std::copy(channels.begin(), channels.end(), channelset._channels);
    channelset._len = (unsigned int)std::min<size_t>(channels.size(), G2CHANNEL_SET::MAX_CHANNEL_COUNT);
    return g2_live_set_camera_list(_handle, channel, &channelset, force);
}

bool g2live::set_camera_list_interest(int channel, const std::set<int>& channels, bool force)
{
    G2CHANNEL_SET channelset;
    std::copy(channels.begin(), channels.end(), channelset._channels);
    channelset._len = (unsigned int)std::min<size_t>(channels.size(), G2CHANNEL_SET::MAX_CHANNEL_COUNT);
    return g2_live_set_camera_list_interest(_handle, channel, &channelset, force);
}

bool g2live::set_camera_stream_set(int channel, const std::set<std::pair<int, int> >& streams, bool force)
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
    return g2_live_set_camera_stream_set(_handle, channel, &chs, force);
}

bool g2live::set_camera_stream_channel(int channel, int channelext, int stream_id)
{
    return g2_live_set_camera_stream_channel(_handle, channel, channelext, stream_id);
}

bool g2live::set_audio_list(int channel, const std::set<int>& channels, bool force)
{
    G2CHANNEL_SET channelset;
    std::copy(channels.begin(), channels.end(), channelset._channels);
    return g2_live_set_audio_list(_handle, channel, &channelset, force);
}

bool g2live::set_stream_id(int channel, const G2GUID& camera, int id)
{
    return g2_live_set_stream_id(_handle, channel, &camera, id);
}

bool g2live::set_alarm_out(int channel, const G2GUID& alarm, bool on)
{
    return g2_live_set_alarm_out(_handle, channel, &alarm, on);
}

bool g2live::set_camera_color(int channel, const G2GUID& camera, int type, int value)
{
    return g2_live_set_camera_color(_handle, channel, &camera, type, value);
}

bool g2live::set_ptz_command(int channel, const G2GUID& camera, const G2LIVE_PTZ_COMMAND& command)
{
    return g2_live_set_ptz_command(_handle, channel, &camera, &command);
}

bool g2live::set_ptz_preset(int channel, const G2GUID& camera, const G2LIVE_PTZ_PRESET& preset)
{
    return g2_live_set_ptz_preset(_handle, channel, &camera, &preset);
}

bool g2live::set_enable_audio_streaming(int channel, const G2GUID& camera, bool enable)
{
    return g2_live_set_enable_audio_streaming(_handle, channel, &camera, enable);
}

bool g2live::set_enable_audio_capturing(int channel, const G2GUID& root, int camera, bool enable)
{
    return g2_live_set_enable_audio_capturing(_handle, channel, &root, camera, enable);
}

bool g2live::set_disable_audio_streaming(int channel, const G2GUID& root)
{
    return g2_live_set_disable_audio_streaming(_handle, channel, &root);
}

bool g2live::set_disable_audio_capturing(int channel, const G2GUID& root)
{
    return g2_live_set_disable_audio_capturing(_handle, channel, &root);
}

bool g2live::set_disable_audio(int channel)
{
    return g2_live_set_disable_audio(_handle, channel);
}

bool g2live::set_disable_audio(int channel, const G2GUID& root)
{
    return g2_live_set_disable_audio_root_guid(_handle, channel, &root);
}

void g2live::set_probe_session_profile(bool active)
{
    g2_live_set_probe_session_profile(_handle, active);
}

//////////////////////////////////////////////////////////////////////////

bool g2live::request_stream_channels(int channel)
{
    return g2_live_request_stream_channels(_handle, channel);
}

bool g2live::request_stream_channels_each(int channel, const std::vector<G2GUID>& cameras)
{
    return g2_live_request_stream_channels_each(_handle, channel, cameras.empty() ? NULL : &cameras[0], cameras.size());
}

bool g2live::request_alive_check(int channel, int check)
{
    return g2_live_request_alive_check(_handle, channel, check);
}

bool g2live::request_ptz_menu(int channel, const G2GUID& camera)
{
    return g2_live_request_ptz_menu(_handle, channel, &camera);
}

bool g2live::request_ptz_preset(int channel, const G2GUID& camera)
{
    return g2_live_request_ptz_preset(_handle, channel, &camera);
}

bool g2live::request_system_log_search(int channel, const G2SERVICE_SEARCH_OPTION_SYSTEM_LOG& option)
{
    return g2_live_request_system_log_search(_handle, channel, &option);
}

bool g2live::request_system_log_search_stop(int channel)
{
    return g2_live_request_system_log_search_stop(_handle, channel);
}

//////////////////////////////////////////////////////////////////////////

bool g2live::send_command_control_color(int channel, const G2GUID& cameraGUID, int camera, const G2LIVE_COMMAND_CONTROL_COLOR& control)
{
    return g2_live_send_command_control_color(_handle, channel, &cameraGUID, camera, &control);
}

bool g2live::send_command_control_color_status(int channel, const G2GUID& cameraGUID, int camera)
{
    return g2_live_send_command_control_color_status(_handle, channel, &cameraGUID, camera);
}

bool g2live::send_command_control_ptz(int channel, const G2GUID& cameraGUID, int camera, const G2LIVE_COMMAND_CONTROL_PTZ& control, bool req_result)
{
    return g2_live_send_command_control_ptz(_handle, channel, &cameraGUID, camera, &control, req_result);
}

bool g2live::send_command_control_ptz_status(int channel, const G2GUID& cameraGUID, int camera)
{
    return g2_live_send_command_control_ptz_status(_handle, channel, &cameraGUID, camera);
}

//////////////////////////////////////////////////////////////////////////

bool g2live::get_camera_list(int channel, std::set<int>& channels) const
{
    G2CHANNEL_SET chs = { 0 };
    if (g2_live_get_camera_list(_handle, channel, &chs)) {
        if (chs._len > 0) {
            std::set<int>(chs._channels, chs._channels + chs._len).swap(channels);
        }
        return true;
    }
    return false;
}

bool g2live::get_camera_list_interest(int channel, std::set<int>& channels) const
{
    G2CHANNEL_SET chs = { 0 };
    if (g2_live_get_camera_list_interest(_handle, channel, &chs)) {
        if (chs._len > 0) {
            std::set<int>(chs._channels, chs._channels + chs._len).swap(channels);
        }
        return true;
    }
    return false;
}

bool g2live::get_camera_stream_set(int channel, std::set<std::pair<int, int> >& streams) const
{
    G2CHANNEL_STREAM_SET chs = { 0 };
    bool res = false;
    if (streams.empty() != true) streams.clear();
    if (g2_live_get_camera_stream_set(_handle, channel, &chs)) {
        for (unsigned int i = 0; i < chs._len; ++i) {
            const G2CHANNEL_STREAM& s = chs._streams[i];
            streams.insert(std::pair<int, int>(s._channel, s._stream));
        }
        res = true;
    }
    return res;
}

bool g2live::get_camera_stream_set(int channel, int channelext, std::set<std::pair<int, int> >& streams) const
{
    G2CHANNEL_STREAM_SET chs = { 0 };
    bool res = false;
    if (streams.empty() != true) streams.clear();
    if (g2_live_get_camera_stream(_handle, channel, channelext, &chs)) {
        for (unsigned int i = 0; i < chs._len; ++i) {
            const G2CHANNEL_STREAM& s = chs._streams[i];
            streams.insert(std::pair<int, int>(s._channel, s._stream));
        }
        res = true;
    }
    return res;
}

G2GUID g2live::get_guid_from_channelext(int channel, int channelext) const
{
    return g2_live_get_guid_from_channelext(_handle, channel, channelext);
}

int g2live::get_channelext_from_guid(int channel, const G2GUID& camera) const
{
    return g2_live_get_channelext_from_guid(_handle, channel, &camera);
}

int g2live::get_stream_id(int channel, const G2GUID& camera) const
{
    return g2_live_get_stream_id(_handle, channel, &camera);
}

int g2live::get_stream_count(int channel, const G2GUID& camera) const
{
    return g2_live_get_stream_count(_handle, channel, &camera);
}

unsigned int g2live::get_stream_remote(int channel, const G2GUID& camera) const
{
    return g2_live_get_stream_remote(_handle, channel, &camera);
}

bool g2live::get_camera_status(int channel, const G2GUID& camera, G2LIVE_CAMERA_STATUS& status) const
{
    return g2_live_get_camera_status(_handle, channel, &camera, &status);
}

int g2live::get_camera_status_ptz_function(int channel, const G2GUID& camera) const
{
    return g2_live_get_status_ptz_function(_handle, channel, &camera);
}

bool g2live::get_camera_status_stream_info(int channel, const G2GUID& camera, int stream_id, G2DEVICE_STATUS_STREAM_INFO& status)
{
    return g2_live_get_camera_status_stream_info(_handle, channel, &camera, stream_id, &status);
}

bool g2live::get_status_command_control_color(int channel, const G2GUID& camera, G2LIVE_COMMAND_CONTROL_COLOR* control, G2LIVE_COMMAND_CONTROL_COLOR_RANGE* range) const
{
	return g2_live_get_status_command_control_color(_handle, channel, &camera, control, range);
}

//////////////////////////////////////////////////////////////////////////

bool g2live::is_enable_multi_stream(int channel, const G2GUID& camera) const
{
    return g2_live_is_enable_multi_stream(_handle, channel, &camera);
}

bool g2live::is_enable_audio_in(int channel, const G2GUID& camera) const
{
    return g2_live_is_enable_audio_in(_handle, channel, &camera);
}

bool g2live::is_enable_audio_out(int channel, const G2GUID& root, int camera) const
{
    return g2_live_is_enable_audio_out(_handle, channel, &root, camera);
}

bool g2live::is_contains_audio_streaming(int channel, const G2GUID& root) const
{
    return g2_live_is_contains_audio_streaming(_handle, channel, &root);
}

bool g2live::is_contains_audio_capturing(int channel, const G2GUID& root, int camera) const
{
    return g2_live_is_contains_audio_capturing(_handle, channel, &root, camera);
}

bool g2live::is_contains_audio_capturing(int channel, const G2GUID& root) const
{
    return g2_live_is_contains_audio_capturing_root(_handle, channel, &root);
}

bool g2live::is_contains_audio(int channel, const G2GUID& root) const
{
    return g2_live_is_contains_audio(_handle, channel, &root);
}

bool g2live::is_support(int channel, const G2GUID& camera, G2LIVE_SUPPORT::QUERY query) const
{
    return g2_live_is_support(_handle, channel, &camera, query);
}

bool g2live::is_audio_out_opening(int channel, const G2GUID& root, int camera) const
{
    return g2_live_is_audio_out_opening(_handle, channel, &root, camera);
}

bool g2live::is_enable_command_control_ptz(int channel, const G2GUID& camera) const
{
    return g2_live_is_enable_command_control_ptz(_handle, channel, &camera);
}

bool g2live::is_enable_command_control_color(int channel, const G2GUID& camera) const
{
    return g2_live_is_enable_command_control_color(_handle, channel, &camera);
}

//////////////////////////////////////////////////////////////////////////

G2RESULT g2live::on_connected(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2live_connected(handle, wparam));
    return 1L;
}

G2RESULT g2live::on_disconnected(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2live_disconnected(handle, wparam, (G2DISCONNECT_REASON::TYPE)lparam));
    return 1L;
}

G2RESULT g2live::on_receive_stream_channels(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2PARAM_BUNCH* data = (G2PARAM_BUNCH*)(lparam);
    G2LIVE_CHANNEL_INFO* info = (G2LIVE_CHANNEL_INFO*)(data->_bunch);
    std::vector<G2LIVE_CHANNEL_INFO> bunch(info, info + data->_len);
    CALL_LISTENER(on_g2live_receive_stream_channels(handle, wparam, bunch));
    return 1L;
}

G2RESULT g2live::on_receive_frame_data(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2FRAME* data = (G2FRAME*)(lparam);
    CALL_LISTENER(on_g2live_receive_frame_data(handle, wparam, *data));
    return 1L;
}

G2RESULT g2live::on_receive_text_in(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2TEXT_IN* data = (G2TEXT_IN*)(lparam);
    CALL_LISTENER(on_g2live_receive_text_in(handle, wparam, *data));
    return 1L;
}

G2RESULT g2live::on_receive_camera_status(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2PARAM_BUNCH* data = (G2PARAM_BUNCH*)(lparam);
    G2LIVE_CAMERA_STATUS* status = (G2LIVE_CAMERA_STATUS*)(data->_bunch);
    std::vector<G2LIVE_CAMERA_STATUS> bunch(status, status + data->_len);
    CALL_LISTENER(on_g2live_receive_camera_status(handle, wparam, bunch));
    return 1L;
}

G2RESULT g2live::on_receive_event(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2EVENT_INFO* data = (G2EVENT_INFO*)(lparam);
    CALL_LISTENER(on_g2live_receive_event(handle, wparam, *data));
    return 1L;
}

G2RESULT g2live::on_receive_ptz_menu(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2LIVE_PTZ_MENU* data = (G2LIVE_PTZ_MENU*)(lparam);
    CALL_LISTENER(on_g2live_receive_ptz_menu(handle, wparam, *data));
    return 1L;
}

G2RESULT g2live::on_receive_ptz_preset(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2LIVE_PTZ_PRESET* data = (G2LIVE_PTZ_PRESET*)(lparam);
    CALL_LISTENER(on_g2live_receive_ptz_preset(handle, wparam, *data));
    return 1L;
}

G2RESULT g2live::on_receive_service_log_load(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SYSTEM_LOG* data = (G2SYSTEM_LOG*)(lparam);
    CALL_LISTENER(on_g2live_receive_service_log_load(handle, wparam, *data));
    return 1L;
}

G2RESULT g2live::on_receive_service_log_load_end(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2live_receive_service_log_load_end(handle, wparam));
    return 1L;
}

G2RESULT g2live::on_receive_service_log_load_fail(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2live_receive_service_log_load_fail(handle, wparam));
    return 1L;
}

G2RESULT g2live::on_receive_service_log_load_stop(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2live_receive_service_log_load_stop(handle, wparam));
    return 1L;
}

G2RESULT g2live::on_receive_audio_out_not_available(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2GUID* root = (G2GUID*)(lparam);
    CALL_LISTENER(on_g2live_receive_audio_out_not_available(handle, wparam, *root));
    return 1L;
}

G2RESULT g2live::on_receive_notify_append_device(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2GUID* root = (G2GUID*)(lparam);
    CALL_LISTENER(on_g2live_receive_notify_append_device(handle, wparam, *root));
    return 1L;
}

G2RESULT g2live::on_receive_notify_remove_device(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2GUID* root = (G2GUID*)(lparam);
    CALL_LISTENER(on_g2live_receive_notify_remove_device(handle, wparam, *root));
    return 1L;
}

G2RESULT g2live::on_receive_network_alarm_result(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2LIVE_NETWORK_ALARM_RESULT_PARAM* p = (G2LIVE_NETWORK_ALARM_RESULT_PARAM*)(lparam);
    CALL_LISTENER(on_g2live_receive_network_alarm_result(handle, wparam, p->_guid, p->_result));
    return 1L;
}

G2RESULT g2live::on_audio_streaming_started(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2live_audio_streaming_started(handle, wparam, lparam));
    return 1L;
}

G2RESULT g2live::on_audio_streaming_stopped(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2live_audio_streaming_stopped(handle, wparam, lparam));
    return 1L;
}

G2RESULT g2live::on_audio_capturing_started(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2LIVE_PARAM_LIVE_AUDIO_OUT* p = (G2LIVE_PARAM_LIVE_AUDIO_OUT*)(lparam);
    CALL_LISTENER(on_g2live_audio_capturing_started(handle, wparam, p->_root, p->_camera));
    return 1L;
}

G2RESULT g2live::on_audio_capturing_stopped(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2LIVE_PARAM_LIVE_AUDIO_OUT* p = (G2LIVE_PARAM_LIVE_AUDIO_OUT*)(lparam);
    CALL_LISTENER(on_g2live_audio_capturing_stopped(handle, wparam, p->_root, p->_camera));
    return 1L;
}

G2RESULT g2live::on_probe_session_profile(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2PROBE_SESSION_PROFILE* data = (G2PROBE_SESSION_PROFILE*)(lparam);
    CALL_LISTENER(on_g2live_probe_session_profile(handle, wparam, *data));
    return 1L;
}

G2RESULT g2live::on_receive_command_result_control_color_status(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2LIVE_PARAM_COMMAND_CONTROL_COLOR_STATUS* s = (G2LIVE_PARAM_COMMAND_CONTROL_COLOR_STATUS*)lparam;
    CALL_LISTENER(on_g2live_receive_command_result_control_color_status(handle, wparam, s->_camera, s->_status._camera, s->_status._control, s->_status._range));
    return 1L;
}
    
G2RESULT g2live::on_receive_command_result_control_color(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2LIVE_PARAM_COMMAND_CONTROL_COLOR_RESULT* r = (G2LIVE_PARAM_COMMAND_CONTROL_COLOR_RESULT*)lparam;
    CALL_LISTENER(on_g2live_receive_command_result_control_color(handle, wparam, r->_camera, r->_result._camera, r->_result._control, (G2LIVE_COMMAND_RESULT::TYPE)r->_result._result));
    return 1L;
}

G2RESULT g2live::on_receive_command_result_control_ptz_status(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2LIVE_PARAM_COMMAND_CONTROL_PTZ_STATUS* s = (G2LIVE_PARAM_COMMAND_CONTROL_PTZ_STATUS*)lparam;
    CALL_LISTENER(on_g2live_receive_command_result_control_ptz_status(handle, wparam, s->_camera, s->_status._camera, s->_status._control, s->_status._range));
    return 1L;
}

G2RESULT g2live::on_receive_command_result_control_ptz(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2LIVE_PARAM_COMMAND_CONTROL_PTZ_RESULT* r = (G2LIVE_PARAM_COMMAND_CONTROL_PTZ_RESULT*)lparam;
    CALL_LISTENER(on_g2live_receive_command_result_control_ptz(handle, wparam, r->_camera, r->_result._camera, (G2LIVE_COMMAND_RESULT::TYPE)r->_result._result));
    return 1L;
}
