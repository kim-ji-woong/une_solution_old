// g2client_play.cpp : implementation file
//

#include "stdafx.h"
#include "g2client_play_sole.h"
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
    g2_play_sole_register_callback(_handle, G2PLAY_SOLE_CALLBACK::function, function);

#define CALL_LISTENER_SOLE(param) { \
    g2play_listener_sole* ptr = ((g2play_sole*)(uparam))->_listener; \
    if (ptr) ptr->param;            \
}

#ifndef LOWORD
#define LOWORD(l)           ((unsigned short)(((unsigned int)(l)) & 0xffff))
#endif
#ifndef HIWORD
#define HIWORD(l)           ((unsigned short)((((unsigned int)(l)) >> 16) & 0xffff))
#endif

//////////////////////////////////////////////////////////////////////////

g2play_sole::g2play_sole(void)
    : _handle(G2HNULL)
    , _listener(NULL)
{
    _handle = g2_play_sole_initialize((G2UPARAM)(this));

    REGISTER_CALLBACK(on_get_options_search_base);
    REGISTER_CALLBACK(on_get_options_player);
    REGISTER_CALLBACK(on_connected);
    REGISTER_CALLBACK(on_disconnected);
    REGISTER_CALLBACK(on_receive_record_channel);
    REGISTER_CALLBACK(on_command_begin);
    REGISTER_CALLBACK(on_command_end);
    REGISTER_CALLBACK(on_play_speed_changed);
    REGISTER_CALLBACK(on_frame_loaded);
    REGISTER_CALLBACK(on_frame_not_found);
    REGISTER_CALLBACK(on_out_of_scope);
    REGISTER_CALLBACK(on_player_error);
    REGISTER_CALLBACK(on_get_rollback_info);
    REGISTER_CALLBACK(on_receive_text_in);
    REGISTER_CALLBACK(on_receive_record_time_info_loaded);
    REGISTER_CALLBACK(on_receive_record_time_info_load_end);
    REGISTER_CALLBACK(on_receive_spot_list);
    REGISTER_CALLBACK(on_receive_scope_list);
    REGISTER_CALLBACK(on_get_frame_buf_status);
}

g2play_sole::~g2play_sole(void)
{
    g2_play_sole_finalize(_handle);
}

//////////////////////////////////////////////////////////////////////////

void g2play_sole::startup(int connections)
{
    assert(_handle != G2HNULL && "g2client_play is not initialized");
    g2_play_sole_startup(_handle, connections);
}

void g2play_sole::cleanup(void)
{
    assert(_handle != G2HNULL && "g2client_play is not initialized");
    g2_play_sole_cleanup(_handle);
}

void g2play_sole::set_listener(g2play_listener_sole* listener)
{
    _listener = listener;
}

//////////////////////////////////////////////////////////////////////////

bool g2play_sole::connect(const G2GUID& site, const G2CONNECT_OPTIONS* options /*= NULL*/, G2CONNECT_RES* res /*= NULL*/)
{
    assert(_handle != G2HNULL && "g2client_play is not initialized");
    return g2_play_sole_connect(_handle, &site, options, res);
}

bool g2play_sole::disconnect(int channel)
{
    assert(_handle != G2HNULL && "g2client_play is not initialized");
    return g2_play_sole_disconnect_with_channel(_handle, channel);
}

bool g2play_sole::is_connecting(int channel) const
{
    assert(_handle != G2HNULL && "g2client_play is not initialized");
    return g2_play_sole_is_connecting(_handle, channel);
}

bool g2play_sole::is_connected(int channel) const
{
    assert(_handle != G2HNULL && "g2client_play is not initialized");
    return g2_play_sole_is_connected(_handle, channel);
}

bool g2play_sole::is_disconnecting(int channel) const
{
    assert(_handle != G2HNULL && "g2client_play is not initialized");
    return g2_play_sole_is_disconnecting(_handle, channel);
}

bool g2play_sole::is_disconnected(int channel) const
{
    assert(_handle != G2HNULL && "g2client_play is not initialized");
    return g2_play_sole_is_disconnected(_handle, channel);
}

bool g2play_sole::is_disconnectable(int channel) const
{
    assert(_handle != G2HNULL && "g2client_play is not initialized");
    return g2_play_sole_is_disconnectable(_handle, channel);
}

//////////////////////////////////////////////////////////////////////////

bool g2play_sole::set_camera_list(int channel, const G2SEARCH_G2_SOLE_CHANNEL_MAP& chs)
{
    return g2_play_sole_request_set_camera_list(_handle, channel, &chs);
}

//////////////////////////////////////////////////////////////////////////

bool g2play_sole::request_search_base_cleanup(int channel, int tag)
{
    return g2_play_sole_request_search_base_cleanup(_handle, channel, tag);
}

bool g2play_sole::request_record_channel(int channel, int tag, const G2GUID& camera)
{
    return g2_play_sole_request_record_channel(_handle, channel, tag, &camera);
}

bool g2play_sole::request_record_time_info(int channel, int tag, int id, int resolution, const G2CHANNEL_SET& chs, const G2SCOPE& scope, int direction)
{
    return g2_play_sole_request_record_time_info(_handle, channel, tag, id, resolution, &chs, &scope, direction);
}

bool g2play_sole::request_record_time_info_load_stop(int channel, int tag)
{
    return g2_play_sole_request_record_time_info_load_stop(_handle, channel, tag);
}

bool g2play_sole::request_set_player_scope(int channel, int tag, const G2SCOPE& scope)
{
    return g2_play_sole_request_set_player_scope(_handle, channel, tag, &scope);
}

bool g2play_sole::request_play(int channel, int tag, int speed)
{
    return g2_play_sole_request_play(_handle, channel, tag, speed);
}

bool g2play_sole::request_play(int channel, int tag, const G2PLAYBACK_COMMAND& command)
{
    return g2_play_sole_request_play_with_command(_handle, channel, tag, &command);
}

bool g2play_sole::request_play_audio(int channel, int tag, int audio)
{
    return g2_play_sole_request_play_audio(_handle, channel, tag, audio);
}

bool g2play_sole::request_pause(int channel, int tag, const G2ROLLBACK_INFO& rbi, bool rollback)
{
    return g2_play_sole_request_pause(_handle, channel, tag, &rbi, rollback);
}

bool g2play_sole::request_stop(int channel, int tag)
{
    return g2_play_sole_request_stop(_handle, channel, tag);
}

bool g2play_sole::request_move_to_first(int channel, int tag)
{
    return g2_play_sole_request_move_to_first(_handle, channel, tag);
}

bool g2play_sole::request_move_to_last(int channel, int tag)
{
    return g2_play_sole_request_move_to_last(_handle, channel, tag);
}

bool g2play_sole::request_move_to_spot(int channel, int tag, const G2SPOT& spot, int precision, bool forward, bool discard)
{
    return g2_play_sole_request_move_to_spot(_handle, channel, tag, &spot, precision, forward, discard);
}

bool g2play_sole::request_prev_step(int channel, int tag)
{
    return g2_play_sole_request_step_prev(_handle, channel, tag);
}

bool g2play_sole::request_next_step(int channel, int tag)
{
    return g2_play_sole_request_step_next(_handle, channel, tag);
}

bool g2play_sole::request_notify_end_of_play(int channel, int tag)
{
    return g2_play_sole_request_notify_end_of_play(_handle, channel, tag);
}

bool g2play_sole::request_snapshot(int channel, int tag, const G2SEARCH_G2_SOLE_SNAPSHOT_LOADER_OPTION_LIST& options)
{
    return g2_play_sole_request_snapshot(_handle, channel, tag, &options);
}

bool g2play_sole::reuqest_snapshot_cancel(int channel, int tag)
{
    return g2_play_sole_request_snapshot_cancel(_handle, channel, tag);
}

bool g2play_sole::request_frame_channelset(int channel, int tag, int id)
{
    return g2_play_sole_request_frame_channelset(_handle, channel, tag, id);
}

bool g2play_sole::request_spot_list(int channel, int tag, int id, int play_channel, const G2TIME& time, bool adjacent)
{
    return g2_play_sole_request_spot_list(_handle, channel, tag, id, play_channel, &time, adjacent);
}

bool g2play_sole::request_scope_list(int channel, int tag, int id, int play_channel, const G2TIME& from, const G2TIME& to)
{
    return g2_play_sole_request_scope_list(_handle, channel, tag, id, play_channel, &from, &to);
}

bool g2play_sole::request_recorded_scope(int channel, int tag, int id, int play_channel)
{
    return g2_play_sole_request_recorded_scope(_handle, channel, tag, id, play_channel);
}

bool g2play_sole::request_frame_spot_list(int channel, int tag, int id, int play_channel, const G2SCOPE& scope, bool fcontinue)
{
    return g2_play_sole_request_frame_spot_list(_handle, channel, tag, id, play_channel, &scope, &fcontinue);
}


//////////////////////////////////////////////////////////////////////////

bool g2play_sole::get_frame_from(int* from)
{
    return g2_play_sole_get_frame_from(_handle, from);
}

bool g2play_sole::is_player_stopped(int channel, int tag)
{
    return g2_play_sole_is_player_stopped_with_tag(_handle, channel, tag);
}

bool g2play_sole::is_player_stopped(int channel)
{
    return g2_play_sole_is_player_stopped(_handle, channel);
}

bool g2play_sole::is_started()
{
    return g2_play_sole_is_started(_handle);
}

bool g2play_sole::channel_from_site(const G2GUID site, int* channel)
{
    return g2_play_sole_channel_from_site(_handle, &site, channel);
}

//////////////////////////////////////////////////////////////////////////

G2RESULT g2play_sole::on_get_options_search_base(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SEARCH_G2_SOLE_BASE_OPTIONS* options = (G2SEARCH_G2_SOLE_BASE_OPTIONS*)(lparam);
    CALL_LISTENER_SOLE(on_g2play_sole_get_options_search_base(handle, wparam, options));
    return 1L;
}

G2RESULT g2play_sole::on_get_options_player(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SEARCH_G2_SOLE_PLAYER_OPTIONS* options = (G2SEARCH_G2_SOLE_PLAYER_OPTIONS*)(lparam);
    CALL_LISTENER_SOLE(on_g2play_sole_get_options_player(handle, wparam, options));
    return 1L;
}

G2RESULT g2play_sole::on_connected(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER_SOLE(on_g2play_sole_connected(handle, wparam));
    return 1L;
}

G2RESULT g2play_sole::on_disconnected(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER_SOLE(on_g2play_sole_disconnected(handle, wparam, (G2DISCONNECT_REASON::TYPE)lparam));
    return 1L;
}

G2RESULT g2play_sole::on_probe_frameload(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SEARCH_G2_SOLE_PROBE_FRAMELOAD* p = (G2SEARCH_G2_SOLE_PROBE_FRAMELOAD*)(lparam);
    CALL_LISTENER_SOLE(on_g2play_sole_probe_frameload(handle, wparam, p->_ips, p->_bps));
    return 1L;
}

G2RESULT g2play_sole::on_command_begin(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SEARCH_G2_SOLE_CHANNEL_TAG* c = (G2SEARCH_G2_SOLE_CHANNEL_TAG*)(wparam);
    CALL_LISTENER_SOLE(on_g2play_sole_receive_notify_command_begin(handle, c->_channel, c->_tag, (G2PLAYER::COMMAND_AND_SPEED)lparam));
    return 1L;
}

G2RESULT g2play_sole::on_command_end(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SEARCH_G2_SOLE_CHANNEL_TAG* c = (G2SEARCH_G2_SOLE_CHANNEL_TAG*)(wparam);
    CALL_LISTENER_SOLE(on_g2play_sole_receive_notify_command_end(handle, c->_channel, c->_tag, (G2PLAYER::COMMAND_AND_SPEED)lparam));
    return 1L;
}

G2RESULT g2play_sole::on_play_speed_changed(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SEARCH_G2_SOLE_CHANNEL_TAG* c = (G2SEARCH_G2_SOLE_CHANNEL_TAG*)(wparam);
    CALL_LISTENER_SOLE(on_g2play_sole_receive_notify_play_speed_changed(handle, c->_channel, c->_tag, (G2PLAYER::COMMAND_AND_SPEED)lparam));
    return 1L;
}

G2RESULT g2play_sole::on_frame_loaded(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SEARCH_G2_SOLE_CHANNEL_TAG* c = (G2SEARCH_G2_SOLE_CHANNEL_TAG*)(wparam);
    G2FRAME* frame = (G2FRAME*)(lparam);
    CALL_LISTENER_SOLE(on_g2play_sole_receive_frame_data(handle, c->_channel, c->_tag, *frame));
    return 1L;
}

G2RESULT g2play_sole::on_frame_not_found(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SEARCH_G2_SOLE_CHANNEL_TAG* c = (G2SEARCH_G2_SOLE_CHANNEL_TAG*)(wparam);
    G2SPOT_PRECISION* data = (G2SPOT_PRECISION*)(lparam);
    CALL_LISTENER_SOLE(on_g2play_sole_receive_notify_frame_not_found(handle, c->_channel, c->_tag, data->_spot, (G2PLAYER::PRECISION::TYPE)data->_precision));
    return 1L;
}

G2RESULT g2play_sole::on_out_of_scope(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SEARCH_G2_SOLE_CHANNEL_TAG* c = (G2SEARCH_G2_SOLE_CHANNEL_TAG*)(wparam);
    CALL_LISTENER_SOLE(on_g2play_sole_receive_notify_out_of_scope(handle, c->_channel, c->_tag, (G2PLAYER::OUT_OF_SCOPE::TYPE)lparam));
    return 1L;
}

G2RESULT g2play_sole::on_player_set_scope(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SEARCH_G2_SOLE_CHANNEL_TAG* c = (G2SEARCH_G2_SOLE_CHANNEL_TAG*)(wparam);
    G2SCOPE* scope = (G2SCOPE*)(lparam);
    CALL_LISTENER_SOLE(on_g2play_sole_receive_notify_player_set_scope(handle, c->_channel, c->_tag, *scope));
    return 1L;
}

G2RESULT g2play_sole::on_player_error(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SEARCH_G2_SOLE_CHANNEL_TAG* c = (G2SEARCH_G2_SOLE_CHANNEL_TAG*)(wparam);
    CALL_LISTENER_SOLE(on_g2play_sole_receive_notify_player_error(handle, c->_channel, c->_tag, (G2PLAYER::PLAYER_ERROR::TYPE)lparam));
    return 1L;
}

G2RESULT g2play_sole::on_get_rollback_info(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SEARCH_G2_SOLE_CHANNEL_TAG* c = (G2SEARCH_G2_SOLE_CHANNEL_TAG*)(wparam);
    G2ROLLBACK_INFO* rbi = (G2ROLLBACK_INFO*)(lparam);
    CALL_LISTENER_SOLE(on_g2play_sole_require_prepare_rollback(handle, c->_channel, c->_tag, *rbi));
    return 1L;
}

G2RESULT g2play_sole::on_get_frame_buf_status(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SEARCH_G2_SOLE_CHANNEL_TAG* c = (G2SEARCH_G2_SOLE_CHANNEL_TAG*)(wparam);
    CALL_LISTENER_SOLE(on_g2play_sole_get_frame_buf_status(handle, c->_channel, c->_tag, (int*)lparam));
    return 1L;
}

G2RESULT g2play_sole::on_receive_text_in(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SEARCH_G2_SOLE_CHANNEL_TAG* c = (G2SEARCH_G2_SOLE_CHANNEL_TAG*)(wparam);
    G2TEXT_IN* in = (G2TEXT_IN*)(lparam);
    CALL_LISTENER_SOLE(on_g2play_sole_receive_text_in(handle, c->_channel, c->_tag, *in));
    return 1L;
}

G2RESULT g2play_sole::on_receive_snapshot_frame(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SEARCH_G2_SOLE_CHANNEL_TAG_ID* c = (G2SEARCH_G2_SOLE_CHANNEL_TAG_ID*)(wparam);
    G2FRAME* f = (G2FRAME*)(lparam);
    CALL_LISTENER_SOLE(on_g2play_sole_receive_snapshot_frame(handle, c->_channel, c->_tag, c->_id, *f));
    return 1L;
}

G2RESULT g2play_sole::on_receive_snapshot_begin(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SEARCH_G2_SOLE_CHANNEL_TAG_ID* c = (G2SEARCH_G2_SOLE_CHANNEL_TAG_ID*)(wparam);
    CALL_LISTENER_SOLE(on_g2play_sole_receive_snapshot_begin(handle, c->_channel, c->_tag, c->_id));
    return 1L;
}

G2RESULT g2play_sole::on_receive_snapshot_end(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SEARCH_G2_SOLE_CHANNEL_TAG_ID* c = (G2SEARCH_G2_SOLE_CHANNEL_TAG_ID*)(wparam);
    CALL_LISTENER_SOLE(on_g2play_sole_receive_snapshot_end(handle, c->_channel, c->_tag, c->_id, (G2SEARCH_G2_SOLE_SNAPSHOP_LOADER::FINISH_REASON)(lparam)));
    return 1L;
}

G2RESULT g2play_sole::on_receive_record_channel(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SEARCH_G2_SOLE_CHANNEL_TAG* c = (G2SEARCH_G2_SOLE_CHANNEL_TAG*)(wparam);
    CALL_LISTENER_SOLE(on_g2play_sole_receive_record_channel(handle, c->_channel, c->_tag, (int)(lparam)));
    return 1L;
}

G2RESULT g2play_sole::on_receive_record_time_info_loaded(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SEARCH_G2_SOLE_CHANNEL_TAG_ID* c = (G2SEARCH_G2_SOLE_CHANNEL_TAG_ID*)(wparam);
    G2RECORD_TIME_INFO* rti = (G2RECORD_TIME_INFO*)(lparam);
    CALL_LISTENER_SOLE(on_g2play_sole_receive_record_time_info_load(handle, c->_channel, c->_tag, c->_id, *rti));
    return 1L;
}

G2RESULT g2play_sole::on_receive_record_time_info_load_end(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SEARCH_G2_SOLE_CHANNEL_TAG_ID* c = (G2SEARCH_G2_SOLE_CHANNEL_TAG_ID*)(wparam);
    CALL_LISTENER_SOLE(on_g2play_sole_receive_record_time_info_load_end(handle, c->_channel, c->_tag, c->_id, lparam));
    return 1L;
}

G2RESULT g2play_sole::on_receive_frame_channelset(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SEARCH_G2_SOLE_CHANNEL_TAG_ID* c = (G2SEARCH_G2_SOLE_CHANNEL_TAG_ID*)(wparam);
    G2CHANNEL_SET* chs = (G2CHANNEL_SET*)(lparam);
    CALL_LISTENER_SOLE(on_g2play_sole_receive_frame_channelset(handle, c->_channel, c->_tag, c->_id, *chs));
    return 1L;
}

G2RESULT g2play_sole::on_receive_spot_list(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SEARCH_G2_SOLE_CHANNEL_TAG_ID* c = (G2SEARCH_G2_SOLE_CHANNEL_TAG_ID*)(wparam);
    G2SPOT_LIST* data = (G2SPOT_LIST*)(lparam);
    std::vector<G2SPOT> spots(data->_spots, data->_spots + data->_len);
    CALL_LISTENER_SOLE(on_g2play_sole_receive_spot_list(handle, c->_channel, c->_tag, c->_id, spots));
    return 1L;
}

G2RESULT g2play_sole::on_receive_scope_list(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SEARCH_G2_SOLE_CHANNEL_TAG_ID* c = (G2SEARCH_G2_SOLE_CHANNEL_TAG_ID*)(wparam);
    G2SCOPE_LIST* list = (G2SCOPE_LIST*)(lparam);
    std::vector<G2SCOPE> scopes(list->_scopes, list->_scopes + list->_len);
    CALL_LISTENER_SOLE(on_g2play_sole_receive_scope_list(handle, c->_channel, c->_tag, c->_id, scopes));
    return 1L;
}

G2RESULT g2play_sole::on_receive_recorded_scope(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SEARCH_G2_SOLE_CHANNEL_TAG_ID* c = (G2SEARCH_G2_SOLE_CHANNEL_TAG_ID*)(wparam);
    G2SEARCH_G2_SOLE_RECORDED_SCOPE* s = (G2SEARCH_G2_SOLE_RECORDED_SCOPE*)(lparam);
    CALL_LISTENER_SOLE(on_g2play_sole_receive_recorded_scope(handle, c->_channel, c->_tag, c->_id, *s));
    return 1L;
}

G2RESULT g2play_sole::on_receive_frame_spot_list(G2HPLAY_SOLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SEARCH_G2_SOLE_CHANNEL_TAG_ID* c = (G2SEARCH_G2_SOLE_CHANNEL_TAG_ID*)(wparam);
    G2SPOT_LIST* data = (G2SPOT_LIST*)(lparam);
    std::vector<G2SPOT> spots(data->_spots, data->_spots + data->_len);
    CALL_LISTENER_SOLE(on_g2play_sole_receive_frame_spot_list(handle, c->_channel, c->_tag, c->_id, spots));
    return 1L;
}


