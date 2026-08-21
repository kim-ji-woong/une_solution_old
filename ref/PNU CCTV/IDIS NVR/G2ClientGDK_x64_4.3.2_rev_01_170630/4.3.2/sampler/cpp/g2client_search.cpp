// g2client_search.cpp : implementation file
//

#include "stdafx.h"
#include "g2client_search.h"
#include "g2client_search_listener.h"
#include "g2client_search_listener_saver.h"

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
    g2_search_register_callback(_handle, G2SEARCH_CALLBACK::function, function);

#define CALL_LISTENER(param) {  \
    g2search_listener* ptr = ((g2search*)(uparam))->_listener; \
    if (ptr) ptr->param; \
}

#define CALL_LISTENER_RESULT(param, result) {  \
    g2search_listener* ptr = ((g2search*)(uparam))->_listener; \
    if (ptr) result = ptr->param; \
}

#define CALL_LISTENER_SAVER(param) {  \
    g2search_listener_saver* ptr = ((g2search*)(uparam))->_listener_saver; \
    if (ptr) ptr->param; \
}

//////////////////////////////////////////////////////////////////////////

g2search::g2search(void)
    : _handle(G2HNULL)
    , _listener(NULL)
    , _listener_saver(NULL)
{
    _handle = g2_search_initialize((G2UPARAM)(this));

    REGISTER_CALLBACK(on_connected);
    REGISTER_CALLBACK(on_disconnected);
    REGISTER_CALLBACK(on_receive_recorded_date);
    REGISTER_CALLBACK(on_receive_recorded_time_hour);
    REGISTER_CALLBACK(on_receive_recorded_time_minute);
    REGISTER_CALLBACK(on_receive_recorded_rechour_minute);
    REGISTER_CALLBACK(on_receive_frame_data);
    REGISTER_CALLBACK(on_receive_no_frame);
    REGISTER_CALLBACK(on_receive_no_recorded_data);
    REGISTER_CALLBACK(on_receive_no_recorded_data_from_search_target);
    REGISTER_CALLBACK(on_receive_find_idr_event_time);
    REGISTER_CALLBACK(on_receive_notify_play_speed_changed);
    REGISTER_CALLBACK(on_receive_notify_play_stop_post);
    REGISTER_CALLBACK(on_receive_notify_end_of_play);
    REGISTER_CALLBACK(on_receive_notify_segment_changed);
    REGISTER_CALLBACK(on_receive_notify_search_mode_changed);
    REGISTER_CALLBACK(on_receive_notify_command_end);
    REGISTER_CALLBACK(on_receive_query_result_event);
    REGISTER_CALLBACK(on_receive_query_result_text_in);
    REGISTER_CALLBACK(on_receive_recorded_date_scope);
    REGISTER_CALLBACK(on_receive_segment_spot);
    REGISTER_CALLBACK(on_receive_error);
    REGISTER_CALLBACK(on_receive_text_in);
    REGISTER_CALLBACK(on_receive_external_tango_info);
    REGISTER_CALLBACK(on_receive_gps_data_start);
    REGISTER_CALLBACK(on_receive_gps_data);
    REGISTER_CALLBACK(on_receive_gps_data_list);
    REGISTER_CALLBACK(on_receive_gps_data_end);
    REGISTER_CALLBACK(on_receive_gps_data_end_count);
    REGISTER_CALLBACK(on_receive_gps_data_export_cancel_result);
    REGISTER_CALLBACK(on_receive_gps_data_measure_result);
    REGISTER_CALLBACK(on_require_prepare_playback);
    REGISTER_CALLBACK(on_require_prepare_load_event_image);
    REGISTER_CALLBACK(on_require_prepare_reload);
    REGISTER_CALLBACK(on_saver_disconnected);
    REGISTER_CALLBACK(on_saver_receive_recorded_date);
    REGISTER_CALLBACK(on_saver_receive_frame_data);
    REGISTER_CALLBACK(on_saver_receive_no_frame);
    REGISTER_CALLBACK(on_saver_receive_no_recorded_data);
    REGISTER_CALLBACK(on_saver_receive_notify_play_speed_changed);
    REGISTER_CALLBACK(on_saver_receive_notify_play_stop_spot);
    REGISTER_CALLBACK(on_saver_receive_notify_end_of_play);
    REGISTER_CALLBACK(on_saver_receive_notify_command_end);
    REGISTER_CALLBACK(on_saver_receive_clipcopy_scope);
    REGISTER_CALLBACK(on_saver_receive_clipcopy_measure_size);
    REGISTER_CALLBACK(on_saver_receive_clipcopy_size);
    REGISTER_CALLBACK(on_saver_receive_clipcopy_data);
    REGISTER_CALLBACK(on_saver_receive_clipcopy_set_password);
    REGISTER_CALLBACK(on_saver_receive_clipcopy_enable_channels);
    REGISTER_CALLBACK(on_saver_receive_bank_space);
    REGISTER_CALLBACK(on_saver_receive_bank_image);
    REGISTER_CALLBACK(on_saver_receive_bank_audio);
    REGISTER_CALLBACK(on_saver_receive_bank_no_image);
    REGISTER_CALLBACK(on_saver_receive_bank_no_audio);
}

g2search::~g2search(void)
{
    g2_search_finalize(_handle);
}

//////////////////////////////////////////////////////////////////////////

void g2search::startup(int connections)
{
    assert(_handle != G2HNULL && "g2client_search is not initialized");
    g2_search_startup(_handle, connections);
}

void g2search::cleanup(void)
{
    assert(_handle != G2HNULL && "g2client_search is not initialized");
    g2_search_cleanup(_handle);
}

void g2search::set_listener(g2search_listener* listener)
{
    _listener = listener;
}

//////////////////////////////////////////////////////////////////////////

int g2search::connect(const G2GUID& root, const G2CONNECT_OPTIONS* options /*= NULL*/, G2CONNECT_RES* res /*= NULL*/)
{
    assert(_handle != G2HNULL && "g2client_search is not initialized");
    return g2_search_connect(_handle, &root, options, res);
}

int g2search::connect_ras(const G2NETWORK_INFO* ni, const G2CONNECT_OPTIONS* options /*= NULL*/, G2CONNECT_RES* res /*= NULL*/)
{
    assert(_handle != G2HNULL && "g2client_search is not initialized");
    return g2_search_connect_ras(_handle, ni, options, res);
}

void g2search::disconnect(int channel)
{
    assert(_handle != G2HNULL && "g2client_search is not initialized");
    return g2_search_disconnect(_handle, channel);
}

bool g2search::is_connecting(int channel) const
{
    assert(_handle != G2HNULL && "g2client_search is not initialized");
    return g2_search_is_connecting(_handle, channel);
}

bool g2search::is_connected(int channel) const
{
    assert(_handle != G2HNULL && "g2client_search is not initialized");
    return g2_search_is_connected(_handle, channel);
}

bool g2search::is_disconnecting(int channel) const
{
    assert(_handle != G2HNULL && "g2client_search is not initialized");
    return g2_search_is_disconnecting(_handle, channel);
}

bool g2search::is_disconnected(int channel) const
{
    assert(_handle != G2HNULL && "g2client_search is not initialized");
    return g2_search_is_disconnected(_handle, channel);
}

bool g2search::is_disconnectable(int channel) const
{
    assert(_handle != G2HNULL && "g2client_search is not initialized");
    return g2_search_is_disconnectable(_handle, channel);
}

//////////////////////////////////////////////////////////////////////////

void g2search::set_invoke_saver(int channel, g2search_listener_saver* listener)
{
    _listener_saver = listener;
    g2_search_set_invoke_saver(_handle, channel);
}

void g2search::set_revoke_saver(int channel)
{
    g2_search_set_revoke_saver(_handle, channel);
    _listener_saver = NULL;
}

//////////////////////////////////////////////////////////////////////////

bool g2search::set_camera_list(int channel, unsigned int cameras)
{
    return g2_search_set_camera_list(_handle, channel, cameras);
}

bool g2search::set_search_target(int channel, G2SEARCH_TARGET::TYPE target)
{
    return g2_search_set_search_target(_handle, channel, target);
}

bool g2search::set_change_segment(int channel, int id)
{
    return g2_search_set_change_segment(_handle, channel, id);
}

bool g2search::set_change_search_mode(int channel, bool event_search)
{
    return g2_search_set_change_search_mode(_handle, channel, event_search);
}

bool g2search::set_external_tango(int channel, int type, int number)
{
    return g2_search_set_external_tango(_handle, channel, type, number);
}

void g2search::set_query_mode(int channel, G2SEARCH_QUERY::MODE mode)
{
    g2_search_set_query_mode(_handle, channel, mode);
}

void g2search::set_query_cameras(int channel, unsigned int cameras)
{
    g2_search_set_query_cameras(_handle, channel, cameras);
}

void g2search::set_invoke_on_play(int channel, const G2SPOT& spot)
{
    g2_search_set_invoke_on_play(_handle, channel, &spot);
}

void g2search::set_revoke_on_play(int channel)
{
    g2_search_set_revoke_on_play(_handle, channel);
}

void g2search::set_probe_session_profile(bool active)
{
    g2_search_set_probe_session_profile(_handle, active);
}

//////////////////////////////////////////////////////////////////////////

bool g2search::get_server_network_info(int channel, G2SERVER_NETWORK_INFO& ni) const
{
    return g2_search_get_server_network_info(_handle, channel, &ni);
}

bool g2search::get_product_info(int channel, G2_PRODUCT_INFO& pi) const
{
    return g2_search_get_product_info(_handle, channel, &pi);
}

bool g2search::get_remote_search_caps(int channel, G2_PRODUCT_INFO_CAPS::REMOTE_SEARCH& caps) const
{
    return g2_search_get_remote_search_caps(_handle, channel, &caps);
}

bool g2search::get_authority(int channel, G2RAS_AUTHORITY& auth) const
{
    return g2_search_get_authority(_handle, channel, &auth);
}

bool g2search::get_camera_list(int channel, unsigned int& cameras) const
{
    return g2_search_get_camera_list(_handle, channel, &cameras);
}

bool g2search::get_time_last(int channel, G2TIME& time) const
{
    return g2_search_get_time_last(_handle, channel, &time);
}

G2SEARCH_DRIVE::MODE g2search::get_drive_mode(int channel) const
{
    return (G2SEARCH_DRIVE::MODE)g2_search_get_drive_mode(_handle, channel);
}

G2SEARCH_QUERY::MODE g2search::get_query_mode(int channel) const
{
    return (G2SEARCH_QUERY::MODE)g2_search_get_query_mode(_handle, channel);
}

G2SEARCH_TIMELAPSE::MODE g2search::get_timelapse_mode(int channel) const
{
    return (G2SEARCH_TIMELAPSE::MODE)g2_search_get_timelapse_mode(_handle, channel);
}

int g2search::get_play_speed(int channel) const
{
    return g2_search_get_play_speed(_handle, channel);
}

int g2search::get_current_command(int channel) const
{
    return g2_search_get_current_command(_handle, channel);
}

G2SEARCH_TARGET::TYPE g2search::get_search_target(int channel) const
{
    return (G2SEARCH_TARGET::TYPE)g2_search_get_search_target(_handle, channel);
}

bool g2search::get_query_cameras(int channel, unsigned int& cameras) const
{
    return g2_search_get_query_cameras(_handle, channel, &cameras);
}

bool g2search::get_query_condition_event(int channel, G2EVENT_QUERY_CONDITION& condition) const
{
    return g2_search_get_query_condition_event(_handle, channel, &condition);
}

bool g2search::get_query_condition_text_in(int channel, G2TEXT_IN_QUERY_CONDITION& condition) const
{
    return g2_search_get_query_condition_text_in(_handle, channel, &condition);
}

bool g2search::get_query_result_text_in(int channel, int count, G2TEXT_IN data[]) const
{
    return g2_search_get_query_result_text_in(_handle, channel, count, data);
}

bool g2search::get_query_result_event(int channel, int selected, G2SEARCH_LOG_INFO& data) const
{
    return g2_search_get_query_result_event(_handle, channel, selected, &data);
}

bool g2search::is_drive_mode(int channel, G2SEARCH_DRIVE::MODE mode) const
{
    return g2_search_is_drive_mode(_handle, channel, mode);
}

bool g2search::is_query_mode(int channel, G2SEARCH_QUERY::MODE mode) const
{
    return g2_search_is_query_mode(_handle, channel, mode);
}

bool g2search::is_timelapse_mode(int channel, G2SEARCH_TIMELAPSE::MODE mode) const
{
    return g2_search_is_timelapse_mode(_handle, channel, mode);
}

bool g2search::is_event_search_mode(int channel) const
{
    return g2_search_is_event_search_mode(_handle, channel);
}

bool g2search::is_stopped(int channel) const
{
    return g2_search_is_stopped(_handle, channel);
}

bool g2search::is_loading(int channel) const
{
    return g2_search_is_loading(_handle, channel);
}

bool g2search::is_support(int channel, G2SEARCH_SUPPORT::QUERY query) const
{
    return g2_search_is_support(_handle, channel, query);
}

//////////////////////////////////////////////////////////////////////////

bool g2search::request_record_date(int channel)
{
    return g2_search_request_record_date(_handle, channel);
}

bool g2search::request_record_time(int channel, const G2TIME& date)
{
    return g2_search_request_record_time(_handle, channel, &date);
}

bool g2search::request_record_hour(int channel, const G2SPOT& spot, int length, bool direction, bool check_diff_prev_req /*= true*/)
{
    return g2_search_request_record_hour(_handle, channel, &spot, length, direction, check_diff_prev_req);
}

bool g2search::request_record_hour_command(int channel, int command, const G2SPOT& spot)
{
    return g2_search_request_record_hour_command(_handle, channel, command, &spot);
}

bool g2search::request_playback(int channel, int command, const G2SPOT& spot)
{
    return g2_search_request_playback(_handle, channel, command, &spot);
}

bool g2search::request_reload_current(int channel, int camera, const G2SPOT& spot)
{
    return g2_search_request_reload_current(_handle, channel, camera, &spot);
}

bool g2search::request_notify_end_of_play(int channel)
{
    return g2_search_request_notify_end_of_play(_handle, channel);
}

bool g2search::request_query_specific_event(int channel, int seq_number, const G2TIME& begin, const G2TIME& end, int event_type, int camera)
{
    return g2_search_request_query_specific_event(_handle, channel, seq_number, &begin, &end, event_type, camera);
}

bool g2search::request_query_event(int channel, const G2EVENT_QUERY_CONDITION& condition)
{
    return g2_search_request_query_event(_handle, channel, &condition);
}

bool g2search::request_query_text_in(int channel, const G2TEXT_IN_QUERY_CONDITION& condition)
{
    return g2_search_request_query_text_in(_handle, channel, &condition);
}

bool g2search::request_event_image(int channel, int selected, bool last)
{
    return g2_search_request_event_image(_handle, channel, selected, last);
}

bool g2search::request_event_search_stop_idr(int channel)
{
    return g2_search_request_event_search_stop_idr(_handle, channel);
}

//////////////////////////////////////////////////////////////////////////

bool g2search::request_clipcopy_get_enable_channel(int channel)
{
    return g2_search_request_clipcopy_enable_channelset(_handle, channel);
}

bool g2search::request_clipcopy_scope(int channel, const G2TIME& from, const G2TIME& to, unsigned int cameras)
{
    return g2_search_request_clipcopy_scope(_handle, channel, &from, &to, cameras);
}

bool g2search::request_clipcopy_measure_size(int channel, const G2SCOPE& scope, unsigned __int64 freespace, bool slice)
{
    return g2_search_request_clipcopy_measure_size(_handle, channel, &scope, freespace, slice);
}

bool g2search::request_clipcopy_size(int channel)
{
    return g2_search_request_clipcopy_size(_handle, channel);
}

bool g2search::request_clipcopy(int channel, bool start)
{
    return g2_search_request_clipcopy(_handle, channel, start);
}

bool g2search::request_clipcopy_password(int channel, bool use, const G2STRING_32& password)
{
    return g2_search_request_clipcopy_password(_handle, channel, use, &password);
}

bool g2search::request_clipcopy_text_in(int channel, bool include)
{
    return g2_search_request_clipcopy_text_in(_handle, channel, include);
}

bool g2search::request_clipcopy_structure(int channel, int version, bool exclude_player, bool avoid_seek)
{
    return g2_search_request_clipcopy_structure(_handle, channel, version, exclude_player, avoid_seek);
}

bool g2search::request_clipcopy_cancel(int channel)
{
    return g2_search_request_clipcopy_cancel(_handle, channel);
}

bool g2search::request_mini_bank_begin(int channel)
{
    return g2_search_request_mini_bank_begin(_handle, channel);
}

bool g2search::request_mini_bank_end(int channel)
{
    return g2_search_request_mini_bank_begin(_handle, channel);
}

bool g2search::request_mini_bank_space(int channel, const G2TIME& from, const G2TIME& to, unsigned int cameras, bool audio)
{
    return g2_search_request_mini_bank_space(_handle, channel, &from, &to, cameras, audio);
}

bool g2search::request_mini_bank(int channel, int command, const G2TIME& time, int msec)
{
    return g2_search_request_mini_bank(_handle, channel, command, &time, msec);
}

bool g2search::request_gps_data_measure(int channel, const G2TIME& from, const G2TIME& to)
{
    return g2_search_request_gps_data_measure(_handle, channel, &from, &to);
}

bool g2search::request_gps_data_measuer_result(int channel)
{
    return g2_search_request_gps_data_measure_result(_handle, channel);
}

bool g2search::request_gps_data(int channel, const G2TIME& time)
{
    return g2_search_request_gps_data(_handle, channel, &time);
}

bool g2search::request_gps_data_next(int channel)
{
    return g2_search_request_gps_data_next(_handle, channel);
}

bool g2search::request_gps_data_export_cancel(int channel)
{
    return g2_search_request_gps_data_export_cancel(_handle, channel);
}

//////////////////////////////////////////////////////////////////////////

G2RESULT g2search::on_connected(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2search_connected(handle, wparam));
    return 1L;
}

G2RESULT g2search::on_disconnected(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2search_disconnected(handle, wparam, (G2DISCONNECT_REASON::TYPE)lparam));
    return 1L;
}

G2RESULT g2search::on_receive_recorded_date(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2PARAM_BUNCH* p = (G2PARAM_BUNCH*)lparam;
    G2TIME* buf = (G2TIME*)(p->_bunch);
    std::vector<G2TIME> data(buf, buf + p->_len);
    CALL_LISTENER(on_g2search_receive_recorded_date(handle, wparam, data));
    return 1L;
}

G2RESULT g2search::on_receive_recorded_time_hour(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2PARAM_BUNCH* p = (G2PARAM_BUNCH*)lparam;
    CALL_LISTENER(on_g2search_receive_recorded_time_hour(handle, wparam, (const bool(*)[24])(p->_bunch), p->_len));
    return 1L;
}

G2RESULT g2search::on_receive_recorded_time_minute(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2search_receive_recorded_time_minute(handle, wparam, (const unsigned char(*)[24 * 60])lparam));
    return 1L;
}

G2RESULT g2search::on_receive_recorded_rechour_minute(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2PARAM_BUNCH* p = (G2PARAM_BUNCH*)lparam;
    G2RECORD_TIME_INFO* buf = (G2RECORD_TIME_INFO*)(p->_bunch);
    std::vector<G2RECORD_TIME_INFO> data(buf, buf + p->_len);
    CALL_LISTENER(on_g2search_receive_recorded_rechour_minute(handle, wparam, data));
    return 1L;
}

G2RESULT g2search::on_receive_frame_data(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2FRAME* f = (G2FRAME*)lparam;
    CALL_LISTENER(on_g2search_receive_frame_data(handle, wparam, *f));
    return 1L;
}

G2RESULT g2search::on_receive_no_frame(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2search_receive_no_frame(handle, wparam));
    return 1L;
}

G2RESULT g2search::on_receive_no_recorded_data(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2search_receive_no_recorded_data(handle, wparam));
    return 1L;
}

G2RESULT g2search::on_receive_no_recorded_data_from_search_target(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2search_receive_no_recorded_data_from_search_target(handle, wparam, (G2SEARCH_TARGET::TYPE)lparam));
    return 1L;
}

G2RESULT g2search::on_receive_find_idr_event_time(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2TIME* t = (G2TIME*)lparam;
    CALL_LISTENER(on_g2search_receive_find_idr_event_time(handle, wparam, *t));
    return 1L;
}

G2RESULT g2search::on_receive_notify_play_speed_changed(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2search_receive_notify_play_speed_changed(handle, wparam, (G2SEARCH_PLAYBACK::COMMAND)lparam));
    return 1L;
}

G2RESULT g2search::on_receive_notify_play_stop_post(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2search_receive_notify_play_stop_post(handle, wparam, (G2SEARCH_DRIVE::MODE)lparam));
    return 1L;
}

G2RESULT g2search::on_receive_notify_end_of_play(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2search_receive_notify_end_of_play(handle, wparam));
    return 1L;
}

G2RESULT g2search::on_receive_notify_segment_changed(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2search_receive_notify_segment_changed(handle, wparam, lparam));
    return 1L;
}

G2RESULT g2search::on_receive_notify_search_mode_changed(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2search_receive_notify_search_mode_changed(handle, wparam, (G2SEARCH_MODE::TYPE)lparam));
    return 1L;
}

G2RESULT g2search::on_receive_notify_command_end(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2search_receive_notify_command_end(handle, wparam, (G2SEARCH_PLAYBACK::COMMAND)lparam));
    return 1L;
}

G2RESULT g2search::on_receive_query_result_event(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2PARAM_BUNCH* p = (G2PARAM_BUNCH*)lparam;
    G2SEARCH_LOG_INFO* buf = (G2SEARCH_LOG_INFO*)(p->_bunch);
    std::vector<G2SEARCH_LOG_INFO> data(buf, buf + p->_len);
    CALL_LISTENER(on_g2search_receive_query_result_event(handle, wparam, data));
    return 1L;
}

G2RESULT g2search::on_receive_query_result_text_in(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2PARAM_BUNCH* p = (G2PARAM_BUNCH*)lparam;
    G2TEXT_IN* buf = (G2TEXT_IN*)(p->_bunch);
    std::vector<G2TEXT_IN> data(buf, buf + p->_len);
    CALL_LISTENER(on_g2search_receive_query_result_text_in(handle, wparam, data));
    return 1L;
}

G2RESULT g2search::on_receive_recorded_date_scope(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2PARAM_BUNCH* p = (G2PARAM_BUNCH*)lparam;
    G2SCOPE* buf = (G2SCOPE*)(p->_bunch);
    std::vector<G2SCOPE> data(buf, buf+ p->_len);
    CALL_LISTENER(on_g2search_receive_recorded_date_scope(handle, wparam, data));
    return 1L;
}

G2RESULT g2search::on_receive_segment_spot(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2PARAM_BUNCH* p = (G2PARAM_BUNCH*)lparam;
    G2SPOT* buf = (G2SPOT*)(p->_bunch);
    std::vector<G2SPOT> data(buf, buf + p->_len);
    CALL_LISTENER(on_g2search_receive_segment_spot(handle, wparam, data));
    return 1L;
}

G2RESULT g2search::on_receive_error(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2search_receive_error(handle, wparam));
    return 1L;
}

G2RESULT g2search::on_receive_text_in(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    const G2TEXT_IN_ELEMENT* e = (G2TEXT_IN_ELEMENT*)lparam;
    CALL_LISTENER(on_g2search_receive_text_in(handle, wparam, *e));
    return 1L;
}

G2RESULT g2search::on_receive_external_tango_info(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2PARAM_BUNCH* p = (G2PARAM_BUNCH*)lparam;
    const G2SEARCH_EXTERNAL_DISK* buf = (G2SEARCH_EXTERNAL_DISK*)(p->_bunch);
    std::vector<G2SEARCH_EXTERNAL_DISK> data(buf, buf + p->_len);
    CALL_LISTENER(on_g2search_receive_external_tango_info(handle, wparam, data));
    return 1L;
}

G2RESULT g2search::on_receive_gps_data_start(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2search_receive_gps_data_start(handle, wparam));
    return 1L;
}

G2RESULT g2search::on_receive_gps_data(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2TEXT_IN* p = (G2TEXT_IN*)lparam;
    CALL_LISTENER(on_g2search_receive_gps_data(handle, wparam, *p));
    return 1L;
}

G2RESULT g2search::on_receive_gps_data_list(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2PARAM_BUNCH* p = (G2PARAM_BUNCH*)lparam;
    G2TEXT_IN* buf = (G2TEXT_IN*)(p->_bunch);
    std::vector<G2TEXT_IN> data(buf, buf + p->_len);
    CALL_LISTENER(on_g2search_receive_gps_data_list(handle, wparam, data));
    return 1L;
}

G2RESULT g2search::on_receive_gps_data_end(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2search_receive_gps_data_end(handle, wparam));
    return 1L;
}

G2RESULT g2search::on_receive_gps_data_end_count(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2search_receive_gps_data_end_count(handle, wparam, lparam));
    return 1L;
}

G2RESULT g2search::on_receive_gps_data_export_cancel_result(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2search_receive_gps_data_export_cancel_result(handle, wparam, lparam));
    return 1L;
}

G2RESULT g2search::on_receive_gps_data_measure_result(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SEARCH_PARAM_GPS_DATA_MEASURE_RESULT* p = (G2SEARCH_PARAM_GPS_DATA_MEASURE_RESULT*)lparam;
    CALL_LISTENER(on_g2search_receive_gps_data_measure_result(handle, wparam, p->_count, p->_done));
    return 1L;
}

G2RESULT g2search::on_require_prepare_playback(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SEARCH_PARAM_PREPARE_PLAYBACK* p = (G2SEARCH_PARAM_PREPARE_PLAYBACK*)lparam;
    CALL_LISTENER(on_g2search_require_prepare_playback(handle, wparam, (G2SEARCH_PLAYBACK::COMMAND)p->_command, p->_spot));
    return 1L;
}

G2RESULT g2search::on_require_prepare_load_event_image(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SEARCH_PARAM_PREPARE_LOAD_EVENT_IMAGE* p = (G2SEARCH_PARAM_PREPARE_LOAD_EVENT_IMAGE*)lparam;
    bool res = true;
    CALL_LISTENER_RESULT(on_g2search_require_prepare_load_event_image(handle, wparam, p->_selected, p->_last), res);
    return res ? G2TRUE : G2FALSE;
}

G2RESULT g2search::on_require_prepare_reload(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    bool res = true;
    CALL_LISTENER_RESULT(on_g2search_require_prepare_reload(handle, wparam), res);
    return res ? G2TRUE : G2FALSE;
}

G2RESULT g2search::on_probe_session_profile(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2PROBE_SESSION_PROFILE* data = (G2PROBE_SESSION_PROFILE*)(lparam);
    CALL_LISTENER(on_g2search_probe_session_profile(handle, wparam, *data));
    return 1L;
}

//////////////////////////////////////////////////////////////////////////

G2RESULT g2search::on_saver_connected(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER_SAVER(on_g2search_saver_connected(handle, wparam));
    return 1L;
}

G2RESULT g2search::on_saver_disconnected(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER_SAVER(on_g2search_saver_disconnected(handle, wparam, (G2DISCONNECT_REASON::TYPE)lparam));
    return 1L;
}

G2RESULT g2search::on_saver_receive_recorded_date(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2PARAM_BUNCH* p = (G2PARAM_BUNCH*)lparam;
    G2TIME* buf = (G2TIME*)(p->_bunch);
    std::vector<G2TIME> data(buf, buf+ p->_len);
    CALL_LISTENER_SAVER(on_g2search_saver_receive_recorded_date(handle, wparam, data));
    return 1L;
}

G2RESULT g2search::on_saver_receive_frame_data(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2FRAME* f = (G2FRAME*)lparam;
    CALL_LISTENER_SAVER(on_g2search_saver_receive_frame_data(handle, wparam, *f));
    return 1L;
}

G2RESULT g2search::on_saver_receive_no_frame(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER_SAVER(on_g2search_saver_receive_no_frame(handle, wparam));
    return 1L;
}

G2RESULT g2search::on_saver_receive_no_recorded_data(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER_SAVER(on_g2search_saver_receive_no_recorded_data(handle, wparam));
    return 1L;
}

G2RESULT g2search::on_saver_receive_notify_play_speed_changed(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER_SAVER(on_g2search_saver_receive_notify_play_speed_changed(handle, wparam, lparam));
    return 1L;
}

G2RESULT g2search::on_saver_receive_notify_play_stop_spot(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER_SAVER(on_g2search_saver_receive_notify_play_stop_spot(handle, wparam, (G2SEARCH_DRIVE::MODE)lparam));
    return 1L;
}

G2RESULT g2search::on_saver_receive_notify_end_of_play(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER_SAVER(on_g2search_saver_receive_notify_end_of_play(handle, wparam));
    return 1L;
}

G2RESULT g2search::on_saver_receive_notify_command_end(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER_SAVER(on_g2search_saver_receive_notify_command_end(handle, wparam, lparam));
    return 1L;
}

G2RESULT g2search::on_saver_receive_clipcopy_scope(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2PARAM_BUNCH* p = (G2PARAM_BUNCH*)lparam;
    G2SCOPE* buf = (G2SCOPE*)(p->_bunch);
    std::vector<G2SCOPE> data(buf, buf + p->_len);
    CALL_LISTENER_SAVER(on_g2search_saver_receive_clipcopy_scope(handle, wparam, data));
    return 1L;
}

G2RESULT g2search::on_saver_receive_clipcopy_measure_size(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER_SAVER(on_g2search_saver_receive_clipcopy_measure_size(handle, wparam));
    return 1L;
}

G2RESULT g2search::on_saver_receive_clipcopy_size(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SEARCH_PARAM_CLIPCOPY_SIZE* p = (G2SEARCH_PARAM_CLIPCOPY_SIZE*)lparam;
    CALL_LISTENER_SAVER(on_g2search_saver_receive_clipcopy_size(handle, wparam, (G2CLIPCOPY_STATUS::TYPE)p->_status, p->_size, p->_begin, p->_end));
    return 1L;
}

G2RESULT g2search::on_saver_receive_clipcopy_data(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SEARCH_PARAM_CLIPCOPY_DATA* p = (G2SEARCH_PARAM_CLIPCOPY_DATA*)lparam;
    CALL_LISTENER_SAVER(on_g2search_saver_receive_clipcopy_data(handle, wparam, p->_offset, p->_size, p->_data, p->_progress, p->_completed));
    return 1L;
}

G2RESULT g2search::on_saver_receive_clipcopy_set_password(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER_SAVER(on_g2search_saver_receive_clipcopy_password(handle, wparam));
    return 1L;
}

G2RESULT g2search::on_saver_receive_clipcopy_enable_channels(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER_SAVER(on_g2search_saver_receive_clipcopy_enable_channels(handle, wparam, lparam));
    return 1L;
}

G2RESULT g2search::on_saver_receive_bank_space(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SEARCH_PARAM_BANK_SPACE* p = (G2SEARCH_PARAM_BANK_SPACE*)lparam;
    CALL_LISTENER_SAVER(on_g2search_saver_receive_bank_space(handle, wparam, p->_image_index_number, p->_audio_index_number, p->_start_time, p->_start_msec, p->_image_size, p->_audio_size));
    return 1L;
}

G2RESULT g2search::on_saver_receive_bank_image(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER_SAVER(on_g2search_saver_receive_bank_image(handle, wparam, (const unsigned char*)lparam));
    return 1L;
}

G2RESULT g2search::on_saver_receive_bank_audio(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER_SAVER(on_g2search_saver_receive_bank_audio(handle, wparam, (const unsigned char*)lparam));
    return 1L;
}

G2RESULT g2search::on_saver_receive_bank_no_image(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER_SAVER(on_g2search_saver_receive_bank_no_image(handle, wparam));
    return 1L;
}

G2RESULT g2search::on_saver_receive_bank_no_audio(G2HSEARCH handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER_SAVER(on_g2search_saver_receive_bank_no_audio(handle, wparam));
    return 1L;
}
