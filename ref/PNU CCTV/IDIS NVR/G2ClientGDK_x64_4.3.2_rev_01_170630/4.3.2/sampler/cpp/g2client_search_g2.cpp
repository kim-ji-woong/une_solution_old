// g2client_search_g2.cpp : implementation file
//

#include "stdafx.h"
#include "g2client_search_g2.h"
#include "g2client_search_g2_listener.h"
#include "g2client_search_g2_listener_sole.h"
#include "g2client_search_g2_listener_saver.h"
#include "g2_define_play.h"

#include <assert.h>
#include <vector>

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
    g2_search_g2_register_callback(_handle, G2SEARCH_G2_CALLBACK::function, function);

#define CALL_LISTENER(param) {  \
    g2search_g2_listener* ptr = ((g2search_g2*)(uparam))->_listener; \
    if (ptr) ptr->param;        \
}

#define CALL_LISTENER_RESULT(param, result) {  \
    g2search_g2_listener* ptr = ((g2search_g2*)(uparam))->_listener; \
    if (ptr) result = ptr->param; \
}

#define CALL_LISTENER_SOLE(param) {  \
    g2search_g2_listener_sole* ptr = ((g2search_g2*)(uparam))->_listener_sole; \
    if (ptr) ptr->param;        \
}

#define CALL_LISTENER_SAVER(param) {  \
    g2search_g2_listener_saver* ptr = ((g2search_g2*)(uparam))->_listener_saver; \
    if (ptr) ptr->param;        \
}

#ifndef LOWORD
#define LOWORD(l) ((unsigned short)(((unsigned int)(l)) & 0xffff))
#endif
#ifndef HIWORD
#define HIWORD(l) ((unsigned short)((((unsigned int)(l)) >> 16) & 0xffff))
#endif

//////////////////////////////////////////////////////////////////////////

g2search_g2::g2search_g2(void)
    : _handle(G2HNULL)
    , _listener(NULL)
    , _listener_sole(NULL)
    , _listener_saver(NULL)
{
    _handle = g2_search_g2_initialize((G2UPARAM)(this));

    REGISTER_CALLBACK(on_connected);
    REGISTER_CALLBACK(on_disconnected);
    REGISTER_CALLBACK(on_query_options_search_base);
    REGISTER_CALLBACK(on_query_options_player);
    REGISTER_CALLBACK(on_receive_record_time_info_load);
    REGISTER_CALLBACK(on_receive_record_time_info_load_end);
    REGISTER_CALLBACK(on_receive_frame_data);
    REGISTER_CALLBACK(on_receive_text_in);
    REGISTER_CALLBACK(on_receive_event);
    REGISTER_CALLBACK(on_receive_notify_command_begin);
    REGISTER_CALLBACK(on_receive_notify_command_end);
    REGISTER_CALLBACK(on_receive_notify_play_speed_changed);
    REGISTER_CALLBACK(on_receive_notify_frame_not_found);
    REGISTER_CALLBACK(on_receive_notify_out_of_scope);
    REGISTER_CALLBACK(on_receive_notify_get_rollback_info);
    REGISTER_CALLBACK(on_receive_notify_player_error);
    REGISTER_CALLBACK(on_receive_event_log_load_end);
    REGISTER_CALLBACK(on_receive_event_log_load_stop);
    REGISTER_CALLBACK(on_receive_text_in_log_load_end);
    REGISTER_CALLBACK(on_receive_text_in_log_load_stop);
    REGISTER_CALLBACK(on_receive_scope_list);
    REGISTER_CALLBACK(on_receive_spot_list);
    REGISTER_CALLBACK(on_receive_no_recorded_data);
    REGISTER_CALLBACK(on_receive_db_info);
    REGISTER_CALLBACK(on_receive_db_info_external);
    REGISTER_CALLBACK(on_receive_db_selected);
    REGISTER_CALLBACK(on_receive_virtual_channelmap);
    REGISTER_CALLBACK(on_require_prepare_rollback);
    REGISTER_CALLBACK(on_sole_connected);
    REGISTER_CALLBACK(on_sole_disconnected);
    REGISTER_CALLBACK(on_sole_query_options_player);
    REGISTER_CALLBACK(on_sole_receive_record_time_info_load);
    REGISTER_CALLBACK(on_sole_receive_record_time_info_load_end);
    REGISTER_CALLBACK(on_sole_receive_frame_data);
    REGISTER_CALLBACK(on_sole_receive_text_in);
    REGISTER_CALLBACK(on_sole_receive_event);
    REGISTER_CALLBACK(on_sole_receive_notify_command_begin);
    REGISTER_CALLBACK(on_sole_receive_notify_command_end);
    REGISTER_CALLBACK(on_sole_receive_notify_play_speed_changed);
    REGISTER_CALLBACK(on_sole_receive_notify_frame_not_found);
    REGISTER_CALLBACK(on_sole_receive_notify_out_of_scope);
    REGISTER_CALLBACK(on_sole_receive_notify_get_rollback_info);
    REGISTER_CALLBACK(on_sole_receive_notify_player_error);
    REGISTER_CALLBACK(on_sole_receive_scope_list);
    REGISTER_CALLBACK(on_sole_receive_spot_list);
    REGISTER_CALLBACK(on_sole_receive_no_recorded_data);
    REGISTER_CALLBACK(on_sole_require_prepare_rollback);
    REGISTER_CALLBACK(on_saver_connected);
    REGISTER_CALLBACK(on_saver_disconnected);
    REGISTER_CALLBACK(on_saver_receive_frame_data);
    REGISTER_CALLBACK(on_saver_receive_notify_out_of_scope);
    REGISTER_CALLBACK(on_saver_receive_notify_get_rollback_info);
    REGISTER_CALLBACK(on_saver_receive_notify_player_error);
    REGISTER_CALLBACK(on_saver_receive_scope_list);
    REGISTER_CALLBACK(on_saver_receive_no_recorded_data);
    REGISTER_CALLBACK(on_saver_receive_clipcopy_size);
    REGISTER_CALLBACK(on_saver_receive_clipcopy_data);
    REGISTER_CALLBACK(on_saver_receive_clipcopy_set_password);
    REGISTER_CALLBACK(on_saver_receive_clipcopy_canceled);
    REGISTER_CALLBACK(on_saver_receive_clipcopy_job_started);
    REGISTER_CALLBACK(on_saver_receive_clipcopy_job_finished);
    REGISTER_CALLBACK(on_saver_receive_clipcopy_section_begin);
    REGISTER_CALLBACK(on_saver_receive_clipcopy_section_end);
}

g2search_g2::~g2search_g2(void)
{
    g2_search_g2_finalize(_handle);
}

//////////////////////////////////////////////////////////////////////////

void g2search_g2::startup(int connections)
{
    assert(_handle != G2HNULL && "g2client_search is not initialized");
    g2_search_g2_startup(_handle, connections);
}

void g2search_g2::cleanup(void)
{
    assert(_handle != G2HNULL && "g2client_search is not initialized");
    g2_search_g2_cleanup(_handle);
}

void g2search_g2::set_listener(g2search_g2_listener* listener)
{
    _listener = listener;
}

void g2search_g2::set_listener_sole(g2search_g2_listener_sole* listener)
{
    _listener_sole = listener;
}

//////////////////////////////////////////////////////////////////////////

int g2search_g2::connect(const G2GUID& root, const G2CONNECT_OPTIONS* options /*= NULL*/, G2CONNECT_RES* res /*= NULL*/)
{
    assert(_handle != G2HNULL && "g2client_search is not initialized");
    return g2_search_g2_connect(_handle, &root, options, res);
}

int g2search_g2::connect_ras(const G2NETWORK_INFO& ni, bool port_unity, const G2CONNECT_OPTIONS* options /*= NULL*/, G2CONNECT_RES* res /*= NULL*/)
{
    assert(_handle != G2HNULL && "g2client_search is not initialized");
    return g2_search_g2_connect_ras(_handle, &ni, port_unity, options, res);
}

void g2search_g2::disconnect(int channel)
{
    assert(_handle != G2HNULL && "g2client_search is not initialized");
    return g2_search_g2_disconnect(_handle, channel);
}

bool g2search_g2::is_connecting(int channel) const
{
    assert(_handle != G2HNULL && "g2client_search is not initialized");
    return g2_search_g2_is_connecting(_handle, channel);
}

bool g2search_g2::is_connected(int channel) const
{
    assert(_handle != G2HNULL && "g2client_search is not initialized");
    return g2_search_g2_is_connected(_handle, channel);
}

bool g2search_g2::is_disconnecting(int channel) const
{
    assert(_handle != G2HNULL && "g2client_search is not initialized");
    return g2_search_g2_is_disconnecting(_handle, channel);
}

bool g2search_g2::is_disconnected(int channel) const
{
    assert(_handle != G2HNULL && "g2client_search is not initialized");
    return g2_search_g2_is_disconnected(_handle, channel);
}

bool g2search_g2::is_disconnectable(int channel) const
{
    assert(_handle != G2HNULL && "g2client_search is not initialized");
    return g2_search_g2_is_disconnectable(_handle, channel);
}

//////////////////////////////////////////////////////////////////////////

void g2search_g2::set_invoke_saver(int channel, g2search_g2_listener_saver* listener)
{
    _listener_saver = listener;
    g2_search_g2_set_invoke_saver(_handle, channel);
}

void g2search_g2::set_revoke_saver(int channel)
{
    g2_search_g2_set_revoke_saver(_handle, channel);
    _listener_saver = NULL;
}

//////////////////////////////////////////////////////////////////////////

bool g2search_g2::set_search_target(int channel, G2SEARCH_TARGET::TYPE target)
{
    return g2_search_g2_set_search_target(_handle, channel, target);
}

bool g2search_g2::set_camera_list(int channel, const std::set<int>& channels, const G2ROLLBACK_INFO& rbi, bool prepare_rollback, bool* preparing)
{
    G2CHANNEL_SET channelset;
    std::copy(channels.begin(), channels.end(), channelset._channels);
    channelset._len = (unsigned int)std::min<size_t>(channels.size(), G2CHANNEL_SET::MAX_CHANNEL_COUNT);
    return g2_search_g2_set_camera_list(_handle, channel, &channelset, &rbi, prepare_rollback, preparing);
}

bool g2search_g2::set_camera_list_interest(int channel, const std::set<int>& channels)
{
    G2CHANNEL_SET channelset;
    std::copy(channels.begin(), channels.end(), channelset._channels);
    channelset._len = (unsigned int)std::min<size_t>(channels.size(), G2CHANNEL_SET::MAX_CHANNEL_COUNT);
    return g2_search_g2_set_camera_list_interest(_handle, channel, &channelset);
}

bool g2search_g2::set_player_scope(int channel, const G2SCOPE& scope)
{
    return g2_search_g2_set_player_scope(_handle, channel, &scope);
}

bool g2search_g2::set_player_scope_reset(int channel)
{
    return g2_search_g2_set_player_scope_reset(_handle, channel);
}

bool g2search_g2::set_player_audio_play(int channel, G2PLAYER::AUDIO_PLAY::TYPE audio)
{
    return g2_search_g2_set_player_audio_play(_handle, channel, audio);
}

void g2search_g2::set_play_control_command(int channel, int command)
{
    g2_search_g2_set_play_control_command(_handle, channel, command);
}

void g2search_g2::set_event_query_mode(int channel, int mode)
{
    g2_search_g2_set_event_query_mode(_handle, channel, mode);
}

void g2search_g2::set_event_query_cameras(int channel, const std::set<int>& channels)
{
    G2CHANNEL_SET channelset;
    std::copy(channels.begin(), channels.end(), channelset._channels);
    channelset._len = (unsigned int)std::min<size_t>(channels.size(), G2CHANNEL_SET::MAX_CHANNEL_COUNT);

    g2_search_g2_set_event_query_cameras(_handle, channel, &channelset);
}

void g2search_g2::set_probe_session_profile(bool active)
{
    g2_search_g2_set_probe_session_profile(_handle, active);
}

//////////////////////////////////////////////////////////////////////////

bool g2search_g2::get_server_network_info(int channel, G2SERVER_NETWORK_INFO& ni) const
{
    return g2_search_g2_get_server_network_info(_handle, channel, &ni);
}

bool g2search_g2::get_product_info(int channel, G2_PRODUCT_INFO& pi) const
{
    return g2_search_g2_get_product_info(_handle, channel, &pi);
}

bool g2search_g2::get_remote_search_caps(int channel, G2_PRODUCT_INFO_CAPS::REMOTE_SEARCH& caps) const
{
    return g2_search_g2_get_remote_search_caps(_handle, channel, &caps);
}

bool g2search_g2::get_remote_db_info(int channel, G2SEARCH_G2_REMOTE_DB& info) const
{
    return g2_search_g2_get_remote_db_info(_handle, channel, &info);
}

bool g2search_g2::get_authority(int channel, G2RAS_AUTHORITY& auth) const
{
    return g2_search_g2_get_authority(_handle, channel, &auth);
}

bool g2search_g2::get_camera_list(int channel, std::set<int>& channels) const
{
    G2CHANNEL_SET chs = { 0 };
    if (g2_search_g2_get_camera_list(_handle, channel, &chs)) {
        if (chs._len > 0) {
            std::set<int>(chs._channels, chs._channels + chs._len).swap(channels);
        }
        return true;
    }
    return false;
}

bool g2search_g2::get_camera_list_interest(int channel, std::set<int>& channels) const
{
    G2CHANNEL_SET chs = { 0 };
    if (g2_search_g2_get_camera_list_interest(_handle, channel, &chs)) {
        if (chs._len > 0) {
            std::set<int>(chs._channels, chs._channels + chs._len).swap(channels);
        }
        return true;
    }
    return false;
}

int g2search_g2::get_play_speed(int channel) const
{
    return g2_search_g2_get_play_speed(_handle, channel);
}

int g2search_g2::get_play_control_command(int channel) const
{
    return g2_search_g2_get_play_control_command(_handle, channel);
}

int g2search_g2::get_remote_selected_db(int channel) const
{
    return g2_search_g2_get_remote_selected_db(_handle, channel);
}

int g2search_g2::get_current_command(int channel) const
{
    return g2_search_g2_get_current_command(_handle, channel);
}

int g2search_g2::get_event_query_mode(int channel) const
{
    return g2_search_g2_get_event_query_mode(_handle, channel);
}

bool g2search_g2::get_event_query_cameras(int channel, std::set<int>& channels) const
{
    G2CHANNEL_SET chs = { 0 };
    std::copy(channels.begin(), channels.end(), chs._channels);
    chs._len = channels.size();
    return g2_search_g2_get_event_query_cameras(_handle, channel, &chs);
}

bool g2search_g2::get_option_query_event(int channel, G2SEARCH_G2_EVENT_SEARCH_OPTIONS& options) const
{
    return g2_search_g2_get_option_query_event(_handle, channel, &options);
}

bool g2search_g2::get_option_query_text_in(int channel, G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS& options) const
{
    return g2_search_g2_get_option_query_text_in(_handle, channel, &options);
}

bool g2search_g2::get_clipcopy_size_info(int channel, G2CLIPCOPY_SIZE_INFO& csi)
{
    return g2_search_g2_get_clipcopy_size_info(_handle, channel, &csi);
}

//////////////////////////////////////////////////////////////////////////

bool g2search_g2::is_drive_mode(int channel, G2SEARCH_DRIVE::MODE mode) const
{
    return g2_search_g2_is_drive_mode(_handle, channel, mode);
}

bool g2search_g2::is_loading_record_time_info(int channel) const
{
    return g2_search_g2_is_loading_record_time_info(_handle, channel);
}

bool g2search_g2::is_stopped(int channel) const
{
    return g2_search_g2_is_stopped(_handle, channel);
}

bool g2search_g2::is_support(int channel, G2SEARCH_SUPPORT::QUERY query) const
{
    return g2_search_g2_is_support(_handle, channel, query);
}

bool g2search_g2::is_probe_session_profile(void) const
{
    return g2_search_g2_is_probe_session_profile(_handle);
}

//////////////////////////////////////////////////////////////////////////

bool g2search_g2::request_db_info(int channel)
{
    return g2_search_g2_request_db_info(_handle, channel);
}

bool g2search_g2::request_db_select(int channel, G2SEARCH_G2_REMOTE_DB::STORAGE id)
{
    return request_db_select(channel, id, -1, -1);
}

bool g2search_g2::request_db_select(int channel, G2SEARCH_G2_REMOTE_DB::STORAGE id, int external_type, int external_num)
{
    return g2_search_g2_request_db_select(_handle, channel, id, external_type, external_num);
}

bool g2search_g2::request_record_time_info(int channel, int resolution, int direction, const G2SCOPE& scope, int count, int command)
{
    return g2_search_g2_request_record_time_info(_handle, channel, resolution, direction, &scope, count, command);
}

bool g2search_g2::request_record_time_info_load_stop(int channel)
{
    return g2_search_g2_request_record_time_info_load_stop(_handle, channel);
}

bool g2search_g2::request_record_time_info_on_time(int channel, int resolution, int direction, const G2TIME& from, const G2TIME& to, int count, int command, G2SCOPE* res_scope)
{
    return g2_search_g2_request_record_time_info_on_time(_handle, channel, resolution, direction, &from, &to, count, command, res_scope);
}

bool g2search_g2::request_reload_current(int channel)
{
    return g2_search_g2_request_reload_current(_handle, channel);
}

bool g2search_g2::request_reload_recent(int channel)
{
    return g2_search_g2_request_reload_recent(_handle, channel);
}

bool g2search_g2::request_play(int channel, const G2PLAYBACK_COMMAND& command)
{
    return g2_search_g2_request_play(_handle, channel, &command);
}

bool g2search_g2::request_pause(int channel, bool rollback, const G2ROLLBACK_INFO& rbi)
{
    return g2_search_g2_request_pause(_handle, channel, rollback, &rbi);
}

bool g2search_g2::request_stop(int channel)
{
    return g2_search_g2_request_stop(_handle, channel);
}

bool g2search_g2::request_goto_time_first_of(int channel, const G2TIME& time, bool load_adjacent_frame /*= false*/, bool forward /*= true*/, bool* found_spot /*= NULL*/)
{
    return g2_search_g2_request_goto_time_first_of(_handle, channel, &time, load_adjacent_frame, forward, found_spot);
}

bool g2search_g2::request_move_to_first(int channel)
{
    return g2_search_g2_request_move_to_first(_handle, channel);
}

bool g2search_g2::request_move_to_last(int channel)
{
    return g2_search_g2_request_move_to_last(_handle, channel);
}

bool g2search_g2::request_move_to_spot(int channel, const G2SPOT& spot, int precision, bool forward /*= true*/)
{
    return g2_search_g2_request_move_to_spot(_handle, channel, &spot, precision, forward);
}

bool g2search_g2::request_move_to_play(int channel, const G2PLAYBACK_COMMAND& command)
{
    return g2_search_g2_request_move_to_play(_handle, channel, &command);
}

bool g2search_g2::request_prev_step(int channel)
{
    return g2_search_g2_request_prev_step(_handle, channel);
}

bool g2search_g2::request_next_step(int channel)
{
    return g2_search_g2_request_next_step(_handle, channel);
}

bool g2search_g2::request_notify_end_of_play(int channel)
{
    return g2_search_g2_request_notify_end_of_play(_handle, channel);
}

bool g2search_g2::request_scope_list(int channel, const G2TIME& from, const G2TIME& to, const std::set<int>& channels, int type)
{
    G2CHANNEL_SET chs = { 0 };
    std::copy(channels.begin(), channels.end(), chs._channels);
    chs._len = channels.size();
    return g2_search_g2_request_scope_list(_handle, channel, &from, &to, &chs, type);
}

bool g2search_g2::request_spot_list(int channel, const G2TIME& time, const std::set<int>& channels, bool load_adjacent_frame /*= false*/)
{
    G2CHANNEL_SET chs = { 0 };
    std::copy(channels.begin(), channels.end(), chs._channels);
    chs._len = channels.size();
    return g2_search_g2_request_spot_list(_handle, channel, &time, &chs, load_adjacent_frame);
}

bool g2search_g2::request_clipcopy_measure_size(int channel, const std::set<int>& channels, const G2SCOPE& scope, unsigned __int64 free_space, const std::vector<int>& ordered_set, bool slice, unsigned __int64 slice_size, bool exlude_player)
{
    G2CHANNEL_SET chs = { 0 };
    std::copy(channels.begin(), channels.end(), chs._channels);
    chs._len = channels.size();
    return g2_search_g2_request_clipcopy_measure_size(_handle, channel, &chs, &scope, free_space, &ordered_set[0], (unsigned int)ordered_set.size(), slice, slice_size, exlude_player);
}

bool g2search_g2::request_clipcopy_info(int channel, const std::set<int>& channels)
{
    G2CHANNEL_SET chs = { 0 };
    std::copy(channels.begin(), channels.end(), chs._channels);
    chs._len = channels.size();
    return g2_search_g2_request_clipcopy_info(_handle, channel, &chs);
}

bool g2search_g2::request_clipcopy_password(int channel, const wchar_t* password)
{
    return g2_search_g2_request_clipcopy_password(_handle, channel, password);
}

bool g2search_g2::request_clipcopy_text_in(int channel, bool include)
{
    return g2_search_g2_request_clipcopy_text_in(_handle, channel, include);
}

bool g2search_g2::request_clipcopy_gps_data(int channel, bool include)
{
    return g2_search_g2_request_clipcopy_gps_data(_handle, channel, include);
}

bool g2search_g2::request_clipcopy_event(int channel, bool include)
{
    return g2_search_g2_request_clipcopy_event(_handle, channel, include);
}

bool g2search_g2::request_clipcopy_cancel(int channel)
{
    return g2_search_g2_request_clipcopy_cancel(_handle, channel);
}

bool g2search_g2::request_clipcopy_size(int channel)
{
    return g2_search_g2_request_clipcopy_size(_handle, channel);
}

bool g2search_g2::request_clipcopy_data(int channel)
{
    return g2_search_g2_request_clipcopy_data(_handle, channel);
}

bool g2search_g2::request_event_log_search(int channel, const G2SEARCH_G2_EVENT_SEARCH_OPTIONS& option)
{
    return g2_search_g2_request_event_log_search(_handle, channel, &option);
}

bool g2search_g2::request_event_log_search_next(int channel)
{
    return g2_search_g2_request_event_log_search_next(_handle, channel);
}

bool g2search_g2::request_event_log_search_stop(int channel)
{
    return g2_search_g2_request_event_log_search_stop(_handle, channel);
}

bool g2search_g2::request_text_in_log_search(int channel, const G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS& option)
{
    return g2_search_g2_request_text_in_log_search(_handle, channel, &option);
}

bool g2search_g2::request_text_in_log_search_next(int channel)
{
    return g2_search_g2_request_text_in_log_search_next(_handle, channel);
}

bool g2search_g2::request_text_in_log_search_stop(int channel)
{
    return g2_search_g2_request_text_in_log_search_stop(_handle, channel);
}

bool g2search_g2::sole_set_camera_list(int channel, const G2CHANNEL_SET* channels)
{
    return g2_search_g2_sole_set_camera_list(_handle, channel, channels);
}

bool g2search_g2::sole_set_player_scope(int channel, int camera, const G2SCOPE* scope)
{
    return g2_search_g2_sole_set_player_scope(_handle, channel, camera, scope);
}

bool g2search_g2::sole_set_player_scope_reset(int channel, int camera)
{
    return g2_search_g2_sole_set_player_scope_reset(_handle, channel, camera);
}

bool g2search_g2::sole_set_player_audio_play(int channel, int camera, G2PLAYER::AUDIO_PLAY::TYPE audio)
{
    return g2_search_g2_sole_set_player_audio_play(_handle, channel, camera, audio);
}

bool g2search_g2::sole_get_camera_list(int channel, G2CHANNEL_SET* channels)
{
    return g2_search_g2_sole_get_camera_list(_handle, channel, channels);
}

bool g2search_g2::sole_request_record_time_info(int channel, int camera, int resolution, int direction, const G2SCOPE* scope, int count)
{
    return g2_search_g2_sole_request_record_time_info(_handle, channel, camera, resolution, direction, scope, count);
}

bool g2search_g2::sole_request_record_time_info_load_stop(int channel, int camera)
{
    return g2_search_g2_sole_request_record_time_info_load_stop(_handle, channel, camera);
}

bool g2search_g2::sole_request_record_time_info_on_time(int channel, int camera, int resolution, int direction, const G2TIME* from, const G2TIME* to, int count, G2SCOPE* res_scope)
{
    return g2_search_g2_sole_request_record_time_info_on_time(_handle, channel, camera, resolution, direction, from, to, count, res_scope);
}

bool g2search_g2::sole_request_play(int channel, int camera, const G2PLAYBACK_COMMAND* command)
{
    return g2_search_g2_sole_request_play(_handle, channel, camera, command);
}

bool g2search_g2::sole_request_pause(int channel, int camera, bool rollback, const G2ROLLBACK_INFO& rbi)
{
    return g2_search_g2_sole_request_pause(_handle, channel, camera, rollback, &rbi);
}

bool g2search_g2::sole_request_stop(int channel, int camera)
{
    return g2_search_g2_sole_request_stop(_handle, channel, camera);
}

bool g2search_g2::sole_request_move_to_first(int channel, int camera)
{
    return g2_search_g2_sole_request_move_to_first(_handle, channel, camera);
}

bool g2search_g2::sole_request_move_to_last(int channel, int camera)
{
    return g2_search_g2_sole_request_move_to_last(_handle, channel, camera);
}

bool g2search_g2::sole_request_move_to_spot(int channel, int camera, const G2SPOT* spot, int precision, bool forward)
{
    return g2_search_g2_sole_request_move_to_spot(_handle, channel, camera, spot, precision, forward);
}

bool g2search_g2::sole_request_move_to_play(int channel, int camera, const G2PLAYBACK_COMMAND* command)
{
    return g2_search_g2_sole_request_move_to_play(_handle, channel, camera, command);
}

bool g2search_g2::sole_request_prev_step(int channel, int camera)
{
    return g2_search_g2_sole_request_prev_step(_handle, channel, camera);
}

bool g2search_g2::sole_request_next_step(int channel, int camera)
{
    return g2_search_g2_sole_request_next_step(_handle, channel, camera);
}

bool g2search_g2::sole_request_notify_end_of_play(int channel, int camera)
{
    return g2_search_g2_sole_request_notify_end_of_play(_handle, channel, camera);
}

bool g2search_g2::sole_request_scope_list(int channel, int camera, const G2TIME* from, const G2TIME* to)
{
    return g2_search_g2_sole_request_scope_list(_handle, channel, camera, from, to);
}

bool g2search_g2::sole_request_spot_list(int channel, int camera, const G2TIME* time, bool load_adjacent_frame /*= false*/)
{
    return g2_search_g2_sole_request_spot_list(_handle, channel, camera, time, load_adjacent_frame);
}

bool g2search_g2::sole_is_loading_record_time_info(int channel, int camera)
{
    return g2_search_g2_sole_is_loading_record_time_info(_handle, channel, camera);
}

bool g2search_g2::sole_is_stopped(int channel, int camera)
{
    return g2_search_g2_sole_is_stopped(_handle, channel, camera);
}

//////////////////////////////////////////////////////////////////////////

G2RESULT g2search_g2::on_connected(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2search_g2_connected(handle, (int)wparam));
    return 1L;
}

G2RESULT g2search_g2::on_disconnected(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2search_g2_disconnected(handle, (int)wparam, (G2DISCONNECT_REASON::TYPE)lparam));
    return 1L;
}

G2RESULT g2search_g2::on_query_options_search_base(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SEARCH_G2_OPTIONS_SEARCH_BASE* o = (G2SEARCH_G2_OPTIONS_SEARCH_BASE*)lparam;
    CALL_LISTENER(on_g2search_g2_query_options_search_base(handle, (int)wparam, *o));
    return 1L;
}

G2RESULT g2search_g2::on_query_options_player(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SEARCH_G2_OPTIONS_PLAYER* o = (G2SEARCH_G2_OPTIONS_PLAYER*)lparam;
    CALL_LISTENER(on_g2search_g2_query_options_player(handle, (int)wparam, *o));
    return 1L;
}

G2RESULT g2search_g2::on_receive_record_time_info_load(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2RECORD_TIME_INFO* r = (G2RECORD_TIME_INFO*)lparam;
    CALL_LISTENER(on_g2search_g2_receive_record_time_info_load(handle, (int)wparam, *r));
    return 1L;
}

G2RESULT g2search_g2::on_receive_record_time_info_load_end(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2search_g2_receive_record_time_info_load_end(handle, (int)wparam, (G2RECORD_TIME_INFO::RESOLUTION)LOWORD(lparam), (G2RECORD_TIME_INFO::COMMAND)HIWORD(lparam)));
    return 1L;
}

G2RESULT g2search_g2::on_receive_frame_data(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2FRAME* f = (G2FRAME*)lparam;
    CALL_LISTENER(on_g2search_g2_receive_frame_data(handle, (int)wparam, *f));
    return 1L;
}

G2RESULT g2search_g2::on_receive_text_in(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2EVENT* e = (G2EVENT*)lparam;
    CALL_LISTENER(on_g2search_g2_receive_text_in(handle, (int)wparam, *e));
    return 1L;
}

G2RESULT g2search_g2::on_receive_event(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2EVENT* e = (G2EVENT*)lparam;
    CALL_LISTENER(on_g2search_g2_receive_event(handle, (int)wparam, *e));
    return 1L;
}

G2RESULT g2search_g2::on_receive_notify_command_begin(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2search_g2_receive_notify_command_begin(handle, (int)wparam, (G2PLAYER::COMMAND_AND_SPEED)lparam));
    return 1L;
}

G2RESULT g2search_g2::on_receive_notify_command_end(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2search_g2_receive_notify_command_end(handle, (int)wparam, (G2PLAYER::COMMAND_AND_SPEED)lparam));
    return 1L;
}

G2RESULT g2search_g2::on_receive_notify_play_speed_changed(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2search_g2_receive_notify_play_speed_changed(handle, (int)wparam, (G2PLAYER::COMMAND_AND_SPEED)lparam));
    return 1L;
}

G2RESULT g2search_g2::on_receive_notify_frame_not_found(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SPOT_PRECISION* p = (G2SPOT_PRECISION*)lparam;
    CALL_LISTENER(on_g2search_g2_receive_notify_frame_not_found(handle, (int)wparam, p->_spot, (G2PLAYER::PRECISION::TYPE)p->_precision));
    return 1L;
}

G2RESULT g2search_g2::on_receive_notify_out_of_scope(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2search_g2_receive_notify_out_of_scope(handle, (int)wparam, (G2PLAYER::OUT_OF_SCOPE::TYPE)lparam));
    return 1L;
}

G2RESULT g2search_g2::on_receive_notify_get_rollback_info(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2ROLLBACK_INFO* r = (G2ROLLBACK_INFO*)lparam;
    CALL_LISTENER(on_g2search_g2_receive_notify_get_rollback_info(handle, (int)wparam, *r));
    return 1L;
}

G2RESULT g2search_g2::on_receive_notify_player_error(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2search_g2_receive_notify_player_error(handle, (int)wparam, (G2PLAYER::PLAYER_ERROR::TYPE)lparam));
    return 1L;
}

G2RESULT g2search_g2::on_receive_event_log_load_end(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2EVENT_LIST* p = (G2EVENT_LIST*)lparam;
    std::vector<G2EVENT> buf(p->_events, p->_events + p->_len);
    CALL_LISTENER(on_g2search_g2_receive_event_log_load_end(handle, (int)wparam, buf));
    return 1L;
}

G2RESULT g2search_g2::on_receive_event_log_load_stop(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2EVENT_LIST* p = (G2EVENT_LIST*)lparam;
    std::vector<G2EVENT> buf(p->_events, p->_events + p->_len);
    CALL_LISTENER(on_g2search_g2_receive_event_log_load_stop(handle, (int)wparam, buf));
    return 1L;
}

G2RESULT g2search_g2::on_receive_text_in_log_load_end(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2EVENT_LIST* p = (G2EVENT_LIST*)lparam;
    std::vector<G2EVENT> buf(p->_events, p->_events + p->_len);
    CALL_LISTENER(on_g2search_g2_receive_text_in_log_load_end(handle, (int)wparam, buf));
    return 1L;
}

G2RESULT g2search_g2::on_receive_text_in_log_load_stop(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2EVENT_LIST* p = (G2EVENT_LIST*)lparam;
    std::vector<G2EVENT> buf(p->_events, p->_events + p->_len);
    CALL_LISTENER(on_g2search_g2_receive_text_in_log_load_stop(handle, (int)wparam, buf));
    return 1L;
}

G2RESULT g2search_g2::on_receive_scope_list(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SEARCH_G2_SCOPE_LIST* p = (G2SEARCH_G2_SCOPE_LIST*)lparam;
    std::vector<G2SCOPE> buf(p->_list._scopes, p->_list._scopes + p->_list._len);
    CALL_LISTENER(on_g2search_g2_receive_scope_list(handle, (int)wparam, buf, (G2SEARCH_G2_SCOPE_TYPE::TYPE)p->_type));
    return 1L;
}

G2RESULT g2search_g2::on_receive_spot_list(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SPOT_LIST* p = (G2SPOT_LIST*)lparam;
    std::vector<G2SPOT> buf(p->_spots, p->_spots + p->_len);
    CALL_LISTENER(on_g2search_g2_receive_spot_list(handle, (int)wparam, buf));
    return 1L;
}

G2RESULT g2search_g2::on_receive_no_recorded_data(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{    
    CALL_LISTENER(on_g2search_g2_receive_no_recorded_data(handle, (int)wparam));
    return 1L;
}

G2RESULT g2search_g2::on_receive_db_info(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SEARCH_G2_REMOTE_DB* p = (G2SEARCH_G2_REMOTE_DB*)lparam;
    CALL_LISTENER(on_g2search_g2_receive_db_info(handle, (int)wparam, *p));
    return 1L;
}

G2RESULT g2search_g2::on_receive_db_info_external(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2PARAM_BUNCH* p = (G2PARAM_BUNCH*)lparam;
    G2SEARCH_EXTERNAL_DISK* buf = (G2SEARCH_EXTERNAL_DISK*)(p->_bunch);
    std::vector<G2SEARCH_EXTERNAL_DISK> data(buf, buf + p->_len);
    CALL_LISTENER(on_g2search_g2_receive_db_info_external(handle, (int)wparam, data));
    return 1L;
}

G2RESULT g2search_g2::on_receive_db_selected(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2search_g2_receive_db_selected(handle, (int)wparam, LOWORD(lparam), (G2SEARCH_G2_REMOTE_DB::DB_SELECT_RESULT)HIWORD(lparam)));
    return 1L;
}

G2RESULT g2search_g2::on_receive_virtual_channelmap(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2search_g2_receive_virtual_channelmap(handle, (int)wparam));
    return 1L;
}

G2RESULT g2search_g2::on_require_prepare_rollback(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2search_g2_require_prepare_rollback(handle, (int)wparam, (lparam ? true : false)));
    return 1L;
}

G2RESULT g2search_g2::on_probe_session_profile(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2PROBE_SESSION_PROFILE* data = (G2PROBE_SESSION_PROFILE*)(lparam);
    CALL_LISTENER(on_g2search_g2_probe_session_profile(handle, (int)wparam, *data));
    return 1L;
}

//////////////////////////////////////////////////////////////////////////

G2RESULT g2search_g2::on_sole_connected(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER_SOLE(on_g2search_g2_sole_connected(handle, (int)wparam));
    return 1L;
}

G2RESULT g2search_g2::on_sole_disconnected(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER_SOLE(on_g2search_g2_sole_disconnected(handle, (int)wparam, (G2DISCONNECT_REASON::TYPE)lparam));
    return 1L;
}

G2RESULT g2search_g2::on_sole_query_options_player(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SEARCH_G2_OPTIONS_PLAYER* o = (G2SEARCH_G2_OPTIONS_PLAYER*)lparam;
    CALL_LISTENER_SOLE(on_g2search_g2_sole_query_options_player(handle, LOWORD(wparam), HIWORD(wparam), *o));
    return 1L;
}

G2RESULT g2search_g2::on_sole_receive_record_time_info_load(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2RECORD_TIME_INFO* r = (G2RECORD_TIME_INFO*)lparam;
    CALL_LISTENER_SOLE(on_g2search_g2_sole_receive_record_time_info_load(handle, LOWORD(wparam), HIWORD(wparam), *r));
    return 1L;
}

G2RESULT g2search_g2::on_sole_receive_record_time_info_load_end(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER_SOLE(on_g2search_g2_sole_receive_record_time_info_load_end(handle, LOWORD(wparam), HIWORD(wparam), (G2RECORD_TIME_INFO::RESOLUTION)lparam));
    return 1L;
}

G2RESULT g2search_g2::on_sole_receive_frame_data(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2FRAME* f = (G2FRAME*)lparam;
    CALL_LISTENER_SOLE(on_g2search_g2_sole_receive_frame_data(handle, LOWORD(wparam), HIWORD(wparam), *f));
    return 1L;
}

G2RESULT g2search_g2::on_sole_receive_text_in(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2EVENT* e = (G2EVENT*)lparam;
    CALL_LISTENER_SOLE(on_g2search_g2_sole_receive_text_in(handle, LOWORD(wparam), HIWORD(wparam), *e));
    return 1L;
}

G2RESULT g2search_g2::on_sole_receive_event(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2EVENT* e = (G2EVENT*)lparam;
    CALL_LISTENER_SOLE(on_g2search_g2_sole_receive_event(handle, LOWORD(wparam), HIWORD(wparam), *e));
    return 1L;
}

G2RESULT g2search_g2::on_sole_receive_notify_command_begin(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER_SOLE(on_g2search_g2_sole_receive_notify_command_begin(handle, LOWORD(wparam), HIWORD(wparam), (G2PLAYER::COMMAND_AND_SPEED)lparam));
    return 1L;
}

G2RESULT g2search_g2::on_sole_receive_notify_command_end(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER_SOLE(on_g2search_g2_sole_receive_notify_command_end(handle, LOWORD(wparam), HIWORD(wparam), (G2PLAYER::COMMAND_AND_SPEED)lparam));
    return 1L;
}

G2RESULT g2search_g2::on_sole_receive_notify_play_speed_changed(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER_SOLE(on_g2search_g2_sole_receive_notify_play_speed_changed(handle, LOWORD(wparam), HIWORD(wparam), (G2PLAYER::COMMAND_AND_SPEED)lparam));
    return 1L;
}

G2RESULT g2search_g2::on_sole_receive_notify_frame_not_found(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SPOT_PRECISION* p = (G2SPOT_PRECISION*)lparam;
    CALL_LISTENER_SOLE(on_g2search_g2_sole_receive_notify_frame_not_found(handle, LOWORD(wparam), HIWORD(wparam), p->_spot, (G2PLAYER::PRECISION::TYPE)p->_precision));
    return 1L;
}

G2RESULT g2search_g2::on_sole_receive_notify_out_of_scope(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER_SOLE(on_g2search_g2_sole_receive_notify_out_of_scope(handle, LOWORD(wparam), HIWORD(wparam), (G2PLAYER::OUT_OF_SCOPE::TYPE)lparam));
    return 1L;
}

G2RESULT g2search_g2::on_sole_receive_notify_get_rollback_info(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2ROLLBACK_INFO* r = (G2ROLLBACK_INFO*)lparam;
    CALL_LISTENER_SOLE(on_g2search_g2_sole_receive_notify_get_rollback_info(handle, LOWORD(wparam), HIWORD(wparam), *r));
    return 1L;
}

G2RESULT g2search_g2::on_sole_receive_notify_player_error(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER_SOLE(on_g2search_g2_sole_receive_notify_player_error(handle, LOWORD(wparam), HIWORD(wparam), (G2PLAYER::PLAYER_ERROR::TYPE)lparam));
    return 1L;
}

G2RESULT g2search_g2::on_sole_receive_scope_list(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SEARCH_G2_SCOPE_LIST* p = (G2SEARCH_G2_SCOPE_LIST*)lparam;
    std::vector<G2SCOPE> scopes(p->_list._scopes, p->_list._scopes + p->_list._len);
    CALL_LISTENER_SOLE(on_g2search_g2_sole_receive_scope_list(handle, LOWORD(wparam), HIWORD(wparam), scopes, (G2SEARCH_G2_SCOPE_TYPE::TYPE)p->_type));
    return 1L;
}

G2RESULT g2search_g2::on_sole_receive_spot_list(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SPOT_LIST* p = (G2SPOT_LIST*)lparam;
    std::vector<G2SPOT> spots(p->_spots, p->_spots + p->_len);
    CALL_LISTENER_SOLE(on_g2search_g2_sole_receive_spot_list(handle, LOWORD(wparam), HIWORD(wparam), spots));
    return 1L;
}

G2RESULT g2search_g2::on_sole_receive_no_recorded_data(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER_SOLE(on_g2search_g2_sole_receive_no_recorded_data(handle, LOWORD(wparam), HIWORD(wparam)));
    return 1L;
}

G2RESULT g2search_g2::on_sole_require_prepare_rollback(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER_SOLE(on_g2search_g2_sole_require_prepare_rollback(handle, LOWORD(wparam), HIWORD(wparam), lparam ? true : false));
    return 1L;
}

//////////////////////////////////////////////////////////////////////////

G2RESULT g2search_g2::on_saver_connected(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER_SAVER(on_g2search_g2_saver_connected(handle, (int)wparam));
    return 1L;
}

G2RESULT g2search_g2::on_saver_disconnected(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER_SAVER(on_g2search_g2_saver_disconnected(handle, (int)wparam, (G2DISCONNECT_REASON::TYPE)lparam));
    return 1L;
}

G2RESULT g2search_g2::on_saver_receive_frame_data(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2FRAME* f = (G2FRAME*)lparam;
    CALL_LISTENER_SAVER(on_g2search_g2_saver_receive_frame_data(handle, (int)wparam, *f));
    return 1L;
}

G2RESULT g2search_g2::on_saver_receive_notify_out_of_scope(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER_SAVER(on_g2search_g2_saver_receive_notify_out_of_scope(handle, (int)wparam, (G2PLAYER::OUT_OF_SCOPE::TYPE)lparam));
    return 1L;
}

G2RESULT g2search_g2::on_saver_receive_notify_get_rollback_info(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2ROLLBACK_INFO* r = (G2ROLLBACK_INFO*)lparam;
    CALL_LISTENER_SAVER(on_g2search_g2_saver_receive_notify_get_rollback_info(handle, (int)wparam, *r));
    return 1L;
}

G2RESULT g2search_g2::on_saver_receive_notify_player_error(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER_SAVER(on_g2search_g2_saver_receive_notify_player_error(handle, (int)wparam, (G2PLAYER::PLAYER_ERROR::TYPE)lparam));
    return 1L;
}

G2RESULT g2search_g2::on_saver_receive_scope_list(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SEARCH_G2_SCOPE_LIST* p = (G2SEARCH_G2_SCOPE_LIST*)lparam;
    std::vector<G2SCOPE> buf(p->_list._scopes, p->_list._scopes + p->_list._len);
    CALL_LISTENER_SAVER(on_g2search_g2_saver_receive_scope_list(handle, (int)wparam, buf, (G2SEARCH_G2_SCOPE_TYPE::TYPE)p->_type));
    return 1L;
}

G2RESULT g2search_g2::on_saver_receive_no_recorded_data(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER_SAVER(on_g2search_g2_saver_receive_no_recorded_data(handle, (int)wparam));
    return 1L;
}

G2RESULT g2search_g2::on_saver_receive_clipcopy_size(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SEARCH_G2_PARAM_CLIPCOPY_SIZE_INFO* param = (G2SEARCH_G2_PARAM_CLIPCOPY_SIZE_INFO*)lparam;
    CALL_LISTENER_SAVER(on_g2search_g2_saver_receive_clipcopy_size(handle, (int)wparam, (G2CLIPCOPY_STATUS::TYPE)param->_status, param->_info, param->_progress));
    return 1L;
}

G2RESULT g2search_g2::on_saver_receive_clipcopy_data(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2CLIPCOPY_DATA* p = (G2CLIPCOPY_DATA*)lparam;
    CALL_LISTENER_SAVER(on_g2search_g2_saver_receive_clipcopy_data(handle, (int)wparam, p->_offset, p->_size, p->_data, p->_progress));
    return 1L;
}

G2RESULT g2search_g2::on_saver_receive_clipcopy_set_password(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER_SAVER(on_g2search_g2_saver_receive_clipcopy_set_password(handle, (int)wparam, (unsigned int)lparam));
    return 1L;
}

G2RESULT g2search_g2::on_saver_receive_clipcopy_canceled(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER_SAVER(on_g2search_g2_saver_receive_clipcopy_canceled(handle, (int)wparam, (G2CLIPCOPY_ERROR::TYPE)lparam));
    return 1L;
}

G2RESULT g2search_g2::on_saver_receive_clipcopy_job_started(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2CLIPCOPY_JOB* p = (G2CLIPCOPY_JOB*)lparam;
    CALL_LISTENER_SAVER(on_g2search_g2_saver_receive_clipcopy_job_started(handle, (int)wparam, (G2CLIPCOPY_JOB::TYPE)p->_job, p->_num, p->_total));
    return 1L;
}

G2RESULT g2search_g2::on_saver_receive_clipcopy_job_finished(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2CLIPCOPY_JOB* p = (G2CLIPCOPY_JOB*)lparam;
    CALL_LISTENER_SAVER(on_g2search_g2_saver_receive_clipcopy_job_finished(handle, (int)wparam, (G2CLIPCOPY_JOB::TYPE)p->_job, p->_num, p->_total));
    return 1L;
}

G2RESULT g2search_g2::on_saver_receive_clipcopy_section_begin(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2PARAM_BUNCH* p = (G2PARAM_BUNCH*)lparam;
    unsigned int* buf = (unsigned int*)p->_bunch;
    CALL_LISTENER_SAVER(on_g2search_g2_saver_receive_clipcopy_section_begin(handle, (int)wparam, buf[0], buf[1]));
    return 1L;
}

G2RESULT g2search_g2::on_saver_receive_clipcopy_section_end(G2HSEARCH_G2 handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2PARAM_BUNCH* p = (G2PARAM_BUNCH*)lparam;
    unsigned int* buf = (unsigned int*)p->_bunch;
    CALL_LISTENER_SAVER(on_g2search_g2_saver_receive_clipcopy_section_end(handle, (int)wparam, buf[0], buf[1]));
    return 1L;
}
