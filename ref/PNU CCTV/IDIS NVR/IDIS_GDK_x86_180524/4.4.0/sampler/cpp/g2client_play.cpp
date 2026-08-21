// g2client_play.cpp : implementation file
//

#include "stdafx.h"
#include "g2client_play.h"
#include "g2client_play_listener.h"
#include "g2client_play_listener_sole.h"

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
    g2_play_register_callback(_handle, G2PLAY_CALLBACK::function, function);

#define CALL_LISTENER(param) {  \
    g2play_listener* ptr = ((g2play*)(uparam))->_listener; \
    if (ptr) ptr->param;        \
}

#ifndef LOWORD
#define LOWORD(l)           ((unsigned short)(((unsigned int)(l)) & 0xffff))
#endif
#ifndef HIWORD
#define HIWORD(l)           ((unsigned short)((((unsigned int)(l)) >> 16) & 0xffff))
#endif

//////////////////////////////////////////////////////////////////////////

g2play::g2play(void)
    : _handle(G2HNULL)
    , _listener(NULL)
{
    _handle = g2_play_initialize((G2UPARAM)(this));

    REGISTER_CALLBACK(on_connected);
    REGISTER_CALLBACK(on_disconnected);
    REGISTER_CALLBACK(on_receive_hard_disk_info);
    REGISTER_CALLBACK(on_receive_record_channels);
    REGISTER_CALLBACK(on_receive_record_time_info_load);
    REGISTER_CALLBACK(on_receive_record_time_info_load_end);
    REGISTER_CALLBACK(on_receive_frame_data);
    REGISTER_CALLBACK(on_receive_notify_command_begin);
    REGISTER_CALLBACK(on_receive_notify_command_end);
    REGISTER_CALLBACK(on_receive_notify_play_speed_changed);
    REGISTER_CALLBACK(on_receive_notify_frame_not_found);
    REGISTER_CALLBACK(on_receive_notify_out_of_scope);
    REGISTER_CALLBACK(on_receive_notify_player_error);
    REGISTER_CALLBACK(on_receive_scope_list);
    REGISTER_CALLBACK(on_receive_spot_list);
    REGISTER_CALLBACK(on_receive_no_recorded_data);
    REGISTER_CALLBACK(on_receive_record_failover_scope);
    REGISTER_CALLBACK(on_receive_event_log_load);
    REGISTER_CALLBACK(on_receive_event_log_load_end);
    REGISTER_CALLBACK(on_receive_event_log_load_fail);
    REGISTER_CALLBACK(on_receive_event_log_load_stop);
    REGISTER_CALLBACK(on_receive_text_in_log_load);
    REGISTER_CALLBACK(on_receive_text_in_log_load_end);
    REGISTER_CALLBACK(on_receive_text_in_log_load_fail);
    REGISTER_CALLBACK(on_receive_text_in_log_load_stop);
    REGISTER_CALLBACK(on_receive_device_log_load);
    REGISTER_CALLBACK(on_receive_device_log_load_end);
    REGISTER_CALLBACK(on_receive_device_log_load_fail);
    REGISTER_CALLBACK(on_receive_device_log_load_stop);
    REGISTER_CALLBACK(on_receive_service_log_load);
    REGISTER_CALLBACK(on_receive_service_log_load_end);
    REGISTER_CALLBACK(on_receive_service_log_load_fail);
    REGISTER_CALLBACK(on_receive_service_log_load_stop);
    REGISTER_CALLBACK(on_require_prepare_rollback);
	REGISTER_CALLBACK(on_get_frame_buf_status);
}

g2play::~g2play(void)
{
    g2_play_finalize(_handle);
}

//////////////////////////////////////////////////////////////////////////

void g2play::startup(int connections)
{
    assert(_handle != G2HNULL && "g2client_play is not initialized");
    g2_play_startup(_handle, connections);
}

void g2play::cleanup(void)
{
    assert(_handle != G2HNULL && "g2client_play is not initialized");
    g2_play_cleanup(_handle);
}

void g2play::set_listener(g2play_listener* listener)
{
    _listener = listener;
}

//////////////////////////////////////////////////////////////////////////

int g2play::connect(const G2GUID& service, const G2CONNECT_OPTIONS* options /*= NULL*/, G2CONNECT_RES* res /*= NULL*/, G2PLAY_USAGE::TYPE usage /*= G2PLAY_USAGE::PLAYBACK*/)
{
    assert(_handle != G2HNULL && "g2client_play is not initialized");
    return g2_play_connect(_handle, &service, options, res, usage);
}

void g2play::disconnect(int channel)
{
    assert(_handle != G2HNULL && "g2client_play is not initialized");
    return g2_play_disconnect(_handle, channel);
}

bool g2play::is_connecting(int channel) const
{
    assert(_handle != G2HNULL && "g2client_play is not initialized");
    return g2_play_is_connecting(_handle, channel);
}

bool g2play::is_connected(int channel) const
{
    assert(_handle != G2HNULL && "g2client_play is not initialized");
    return g2_play_is_connected(_handle, channel);
}

bool g2play::is_disconnecting(int channel) const
{
    assert(_handle != G2HNULL && "g2client_play is not initialized");
    return g2_play_is_disconnecting(_handle, channel);
}

bool g2play::is_disconnected(int channel) const
{
    assert(_handle != G2HNULL && "g2client_play is not initialized");
    return g2_play_is_disconnected(_handle, channel);
}

bool g2play::is_disconnectable(int channel) const
{
    assert(_handle != G2HNULL && "g2client_play is not initialized");
    return g2_play_is_disconnectable(_handle, channel);
}

//////////////////////////////////////////////////////////////////////////

bool g2play::set_camera_list(int channel, const std::set<int>& channels, const G2ROLLBACK_INFO& rbi, bool prepare_rollback, bool* preparing)
{
    G2CHANNEL_SET chs = { 0 };
    std::copy(channels.begin(), channels.end(), chs._channels);
    chs._len = channels.size();
    return g2_play_set_camera_list(_handle, channel, &chs, &rbi, prepare_rollback, preparing);
}

bool g2play::set_camera_list_interest(int channel, const std::set<int>& channels)
{
    G2CHANNEL_SET chs = { 0 };
    std::copy(channels.begin(), channels.end(), chs._channels);
    chs._len = channels.size();
    return g2_play_set_camera_list_interest(_handle, channel, &chs);
}

void g2play::set_play_control_command(int channel, int command)
{
    g2_play_set_play_control_command(_handle, channel, command);
}

void g2play::set_probe_session_profile(bool active)
{
    g2_play_set_probe_session_profile(_handle, active);
}

//////////////////////////////////////////////////////////////////////////

bool g2play::request_alive_check(int channel, int check)
{
    return g2_play_request_alive_check(_handle, channel, check);
}

bool g2play::request_hard_disk_info(int channel)
{
    return g2_play_request_hard_disk_info(_handle, channel);
}

bool g2play::request_record_channels(int channel)
{
    return g2_play_request_record_channels(_handle, channel);
}

bool g2play::request_record_channels_each(int channel, const std::vector<G2GUID>& cameras)
{
    return g2_play_request_record_channels_each(_handle, channel, cameras.empty() ? NULL : &cameras[0], cameras.size());
}

bool g2play::request_record_time_info(int channel, int resolution, int direction, const G2SCOPE& scope, int count, int command)
{
    return g2_play_request_record_time_info(_handle, channel, resolution, direction, &scope, count, command);
}

bool g2play::request_record_time_info_load_stop(int channel)
{
    return g2_play_request_record_time_info_load_stop(_handle, channel);
}

bool g2play::request_reload_current(int channel)
{
    return g2_play_request_reload_current(_handle, channel);
}

bool g2play::request_play(int channel, const G2PLAYBACK_COMMAND& command)
{
    return g2_play_request_play(_handle, channel, &command);
}

bool g2play::request_pause(int channel, bool rollback, const G2ROLLBACK_INFO& rbi)
{
    return g2_play_request_pause(_handle, channel, rollback, &rbi);
}

bool g2play::request_move_to_first(int channel)
{
    return g2_play_request_move_to_first(_handle, channel);
}

bool g2play::request_move_to_last(int channel)
{
    return g2_play_request_move_to_last(_handle, channel);
}

bool g2play::request_move_to_spot(int channel, const G2SPOT& spot, int precision, bool forward)
{
    return g2_play_request_move_to_spot(_handle, channel, &spot, precision, forward);
}

bool g2play::request_prev_step(int channel)
{
    return g2_play_request_prev_step(_handle, channel);
}

bool g2play::request_next_step(int channel)
{
    return g2_play_request_next_step(_handle, channel);
}

bool g2play::request_notify_end_of_play(int channel)
{
    return g2_play_request_notify_end_of_play(_handle, channel);
}

bool g2play::request_scope_list(int channel, const G2TIME& from, const G2TIME& to, const std::set<int>& channels, int type)
{
    G2CHANNEL_SET chs = { 0 };
    std::copy(channels.begin(), channels.end(), chs._channels);
    chs._len = channels.size();
    return g2_play_request_scope_list(_handle, channel, &from, &to, &chs, type);
}

bool g2play::request_spot_list(int channel, const G2TIME& time, const std::set<int>& channels)
{
    G2CHANNEL_SET chs = { 0 };
    std::copy(channels.begin(), channels.end(), chs._channels);
    chs._len = channels.size();
    return g2_play_request_spot_list(_handle, channel, &time, &chs);
}

bool g2play::request_event_log_search(int channel, const G2SERVICE_SEARCH_OPTION_EVENT_LOG& option)
{
    return g2_play_request_event_log_search(_handle, channel, &option);
}

bool g2play::request_event_log_search_stop(int channel)
{
    return g2_play_request_event_log_search_stop(_handle, channel);
}

bool g2play::request_text_in_log_search(int channel, const G2SERVICE_SEARCH_OPTION_TEXT_IN_LOG& option)
{
    return g2_play_request_text_in_log_search(_handle, channel, &option);
}

bool g2play::request_text_in_log_search_next(int channel)
{
    return g2_play_request_text_in_log_search_next(_handle, channel);
}

bool g2play::request_text_in_log_search_stop(int channel)
{
    return g2_play_request_text_in_log_search_stop(_handle, channel);
}

bool g2play::request_device_log_search(int channel, const G2SERVICE_SEARCH_OPTION_SYSTEM_LOG& option)
{
    return g2_play_request_device_log_search(_handle, channel, &option);
}

bool g2play::request_device_log_search_stop(int channel)
{
    return g2_play_request_device_log_search_stop(_handle, channel);
}

bool g2play::request_system_log_search(int channel, const G2SERVICE_SEARCH_OPTION_SYSTEM_LOG& option)
{
    return g2_play_request_system_log_search(_handle, channel, &option);
}

bool g2play::request_system_log_search_stop(int channel)
{
    return g2_play_request_system_log_search_stop(_handle, channel);
}

//////////////////////////////////////////////////////////////////////////

bool g2play::get_camera_list(int channel, std::set<int>& channels) const
{
    G2CHANNEL_SET chs = { 0 };
    if (g2_play_get_camera_list(_handle, channel, &chs)) {
        if (chs._len > 0) {
            std::set<int>(chs._channels, chs._channels + chs._len).swap(channels);
        }
        return true;
    }
    return false;
}

bool g2play::get_camera_list_interest(int channel, std::set<int>& channels) const
{
    G2CHANNEL_SET chs = { 0 };
    if (g2_play_get_camera_list_interest(_handle, channel, &chs)) {
        if (chs._len > 0) {
            std::set<int>(chs._channels, chs._channels + chs._len).swap(channels);
        }
        return true;
    }
    return false;
}

int g2play::get_play_speed(int channel) const
{
    return g2_play_get_play_speed(_handle, channel);
}

int g2play::get_play_control_command(int channel) const
{
    return g2_play_get_play_control_command(_handle, channel);
}

int g2play::get_current_command(int channel) const
{
    return g2_play_get_current_command(_handle, channel);
}

bool g2play::is_stopped(int channel) const
{
    return g2_play_is_stopped(_handle, channel);
}

//////////////////////////////////////////////////////////////////////////

G2RESULT g2play::on_connected(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2play_connected(handle, wparam));
    return 1L;
}

G2RESULT g2play::on_disconnected(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2play_disconnected(handle, wparam, (G2DISCONNECT_REASON::TYPE)lparam));
    return 1L;
}

G2RESULT g2play::on_receive_hard_disk_info(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2PARAM_BUNCH* data = (G2PARAM_BUNCH*)(lparam);
    const G2PLAY_HARD_DISK_INFO* info = (G2PLAY_HARD_DISK_INFO*)(data->_bunch);
    std::vector<G2PLAY_HARD_DISK_INFO> bunch(info, info + data->_len);
    CALL_LISTENER(on_g2play_receive_hard_disk_info(handle, wparam, bunch));
    return 1L;
}

G2RESULT g2play::on_receive_record_channels(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2PARAM_BUNCH* data = (G2PARAM_BUNCH*)(lparam);
    const G2PLAY_CHANNEL_INFO* info = (G2PLAY_CHANNEL_INFO*)(data->_bunch);
    std::vector<G2PLAY_CHANNEL_INFO> bunch(info, info + data->_len);
    CALL_LISTENER(on_g2play_receive_record_channels(handle, wparam, bunch));
    return 1L;
}

G2RESULT g2play::on_receive_record_time_info_load(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2RECORD_TIME_INFO* rti = (G2RECORD_TIME_INFO*)(lparam);
    CALL_LISTENER(on_g2play_receive_record_time_info_load(handle, wparam, *rti));
    return 1L;
}

G2RESULT g2play::on_receive_record_time_info_load_end(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2play_receive_record_time_info_load_end(handle, wparam, G2RECORD_TIME_INFO::RESOLUTION(LOWORD(lparam)), G2RECORD_TIME_INFO::COMMAND(HIWORD(lparam))));
    return 1L;
}

G2RESULT g2play::on_receive_frame_data(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2FRAME* frame = (G2FRAME*)(lparam);
    CALL_LISTENER(on_g2play_receive_frame_data(handle, wparam, *frame));
    return 1L;
}

G2RESULT g2play::on_receive_text_in(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2TEXT_IN* in = (G2TEXT_IN*)(lparam);
    CALL_LISTENER(on_g2play_receive_text_in(handle, wparam, *in));
    return 1L;
}

G2RESULT g2play::on_receive_notify_command_begin(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2play_receive_notify_command_begin(handle, wparam, (G2PLAYER::COMMAND_AND_SPEED)lparam));
    return 1L;
}

G2RESULT g2play::on_receive_notify_command_end(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2play_receive_notify_command_end(handle, wparam, (G2PLAYER::COMMAND_AND_SPEED)lparam));
    return 1L;
}

G2RESULT g2play::on_receive_notify_play_speed_changed(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2play_receive_notify_play_speed_changed(handle, wparam, (G2PLAYER::COMMAND_AND_SPEED)lparam));
    return 1L;
}

G2RESULT g2play::on_receive_notify_frame_not_found(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SPOT_PRECISION* data = (G2SPOT_PRECISION*)(lparam);
    CALL_LISTENER(on_g2play_receive_notify_frame_not_found(handle, wparam, data->_spot, (G2PLAYER::PRECISION::TYPE)data->_precision));
    return 1L;
}

G2RESULT g2play::on_receive_notify_out_of_scope(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2play_receive_notify_out_of_scope(handle, wparam, (G2PLAYER::OUT_OF_SCOPE::TYPE)lparam));
    return 1L;
}

G2RESULT g2play::on_receive_notify_player_error(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2play_receive_notify_player_error(handle, wparam, (G2PLAYER::PLAYER_ERROR::TYPE)lparam));
    return 1L;
}

G2RESULT g2play::on_receive_scope_list(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2PLAY_SCOPE_LIST* data = (G2PLAY_SCOPE_LIST*)(lparam);
    std::vector<G2SCOPE> scopes(data->_list._scopes, data->_list._scopes + data->_list._len);
    CALL_LISTENER(on_g2play_receive_scope_list(handle, wparam, scopes, (G2PLAY_SCOPE_TYPE::TYPE)data->_type));
    return 1L;
}

G2RESULT g2play::on_receive_spot_list(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SPOT_LIST* data = (G2SPOT_LIST*)(lparam);
    std::vector<G2SPOT> spots(data->_spots, data->_spots + data->_len);
    CALL_LISTENER(on_g2play_receive_spot_list(handle, wparam, spots));
    return 1L;
}

G2RESULT g2play::on_receive_no_recorded_data(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2play_receive_no_recorded_data(handle, wparam));
    return 1L;
}

G2RESULT g2play::on_receive_record_failover_scope(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2FAILOVER_SCOPE_INFO_LIST* data = (G2FAILOVER_SCOPE_INFO_LIST*)(lparam);
    std::vector<G2FAILOVER_SCOPE_INFO> infos(data->_infos, data->_infos + data->_len);
    CALL_LISTENER(on_g2play_receive_record_failover_scope(handle, wparam, infos));
    return 1L;
}

G2RESULT g2play::on_receive_event_log_load(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2EVENT_LOG* data = (G2EVENT_LOG*)(lparam);
    CALL_LISTENER(on_g2play_receive_event_log_load(handle, wparam, *data));
    return 1L;
}

G2RESULT g2play::on_receive_event_log_load_end(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2play_receive_event_log_load_end(handle, wparam));
    return 1L;
}

G2RESULT g2play::on_receive_event_log_load_fail(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2play_receive_event_log_load_fail(handle, wparam));
    return 1L;
}

G2RESULT g2play::on_receive_event_log_load_stop(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2play_receive_event_log_load_stop(handle, wparam));
    return 1L;
}

G2RESULT g2play::on_receive_text_in_log_load(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2EVENT* data = (G2EVENT*)(lparam);
    CALL_LISTENER(on_g2play_receive_text_in_log_load(handle, wparam, *data));
    return 1L;
}

G2RESULT g2play::on_receive_text_in_log_load_end(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2play_receive_text_in_log_load_end(handle, wparam));
    return 1L;
}

G2RESULT g2play::on_receive_text_in_log_load_fail(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2play_receive_text_in_log_load_fail(handle, wparam));
    return 1L;
}

G2RESULT g2play::on_receive_text_in_log_load_stop(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2play_receive_text_in_log_load_stop(handle, wparam));
    return 1L;
}

G2RESULT g2play::on_receive_device_log_load(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SYSTEM_LOG* data = (G2SYSTEM_LOG*)(lparam);
    CALL_LISTENER(on_g2play_receive_device_log_load(handle, wparam, *data));
    return 1L;
}

G2RESULT g2play::on_receive_device_log_load_end(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2play_receive_device_log_load_end(handle, wparam));
    return 1L;
}

G2RESULT g2play::on_receive_device_log_load_fail(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2play_receive_device_log_load_fail(handle, wparam));
    return 1L;
}

G2RESULT g2play::on_receive_device_log_load_stop(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2play_receive_device_log_load_stop(handle, wparam));
    return 1L;
}

G2RESULT g2play::on_receive_service_log_load(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SYSTEM_LOG* data = (G2SYSTEM_LOG*)(lparam);
    CALL_LISTENER(on_g2play_receive_service_log_load(handle, wparam, *data));
    return 1L;
}

G2RESULT g2play::on_receive_service_log_load_end(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2play_receive_service_log_load_end(handle, wparam));
    return 1L;
}

G2RESULT g2play::on_receive_service_log_load_fail(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2play_receive_service_log_load_fail(handle, wparam));
    return 1L;
}

G2RESULT g2play::on_receive_service_log_load_stop(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2play_receive_service_log_load_stop(handle, wparam));
    return 1L;
}

G2RESULT g2play::on_require_prepare_rollback(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2play_require_prepare_rollback(handle, wparam, static_cast<bool>(lparam)));
    return 1L;
}

G2RESULT g2play::on_probe_session_profile(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2PROBE_SESSION_PROFILE* data = (G2PROBE_SESSION_PROFILE*)(lparam);
    CALL_LISTENER(on_g2play_probe_session_profile(handle, wparam, *data));
    return 1L;
}

G2RESULT g2play::on_get_frame_buf_status(G2HPLAY handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{	
	G2SEARCH_G2_SOLE_CHANNEL_TAG* data = (G2SEARCH_G2_SOLE_CHANNEL_TAG*)(wparam);
	CALL_LISTENER(on_g2play_get_frame_buf_status(handle, data->_channel, data->_tag, (int*)lparam));
    return 1L;
}
