// g2client_play_saver.cpp : implementation file
//

#include "stdafx.h"
#include "g2client_play_saver.h"
#include "g2client_play_saver_listener.h"

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
    g2_play_saver_register_callback(_handle, G2PLAY_SAVER_CALLBACK::function, function);

#define CALL_LISTENER(param) {  \
    g2play_saver_listener* ptr = ((g2play_saver*)(uparam))->_listener; \
    if (ptr) ptr->param; \
}

#ifndef LOWORD
#define LOWORD(l)           ((unsigned short)(((unsigned int)(l)) & 0xffff))
#endif
#ifndef HIWORD
#define HIWORD(l)           ((unsigned short)((((unsigned int)(l)) >> 16) & 0xffff))
#endif

//////////////////////////////////////////////////////////////////////////

g2play_saver::g2play_saver(void)
    : _handle(G2HNULL)
    , _listener(NULL)
{
    _handle = g2_play_saver_initialize((G2UPARAM)(this));

    REGISTER_CALLBACK(on_connected);
    REGISTER_CALLBACK(on_disconnected);
    REGISTER_CALLBACK(on_receive_record_channels);
    REGISTER_CALLBACK(on_receive_frame_data);
    REGISTER_CALLBACK(on_receive_notify_out_of_scope);
    REGISTER_CALLBACK(on_receive_notify_player_error);
    REGISTER_CALLBACK(on_receive_scope_list);
    REGISTER_CALLBACK(on_receive_no_recorded_data);
    REGISTER_CALLBACK(on_receive_clipcopy_size);
    REGISTER_CALLBACK(on_receive_clipcopy_data);
    REGISTER_CALLBACK(on_receive_clipcopy_canceled);
    REGISTER_CALLBACK(on_receive_clipcopy_set_password);
    REGISTER_CALLBACK(on_receive_clipcopy_job_started);
    REGISTER_CALLBACK(on_receive_clipcopy_job_finished);
}

g2play_saver::~g2play_saver(void)
{
    g2_play_saver_finalize(_handle);
}

//////////////////////////////////////////////////////////////////////////

void g2play_saver::startup(int connections)
{
    assert(_handle != G2HNULL && "g2client_play_saver is not initialized");
    g2_play_saver_startup(_handle, connections);
}

void g2play_saver::cleanup(void)
{
    assert(_handle != G2HNULL && "g2client_play_saver is not initialized");
    g2_play_saver_cleanup(_handle);
}

void g2play_saver::set_listener(g2play_saver_listener* listener)
{
    _listener = listener;
}

//////////////////////////////////////////////////////////////////////////

int g2play_saver::connect(const G2GUID& service, const G2CONNECT_OPTIONS* options /*= NULL*/, G2CONNECT_RES* res /*= NULL*/)
{
    assert(_handle != G2HNULL && "g2client_play_saver is not initialized");
    return g2_play_saver_connect(_handle, &service, options, res);
}

void g2play_saver::disconnect(int channel)
{
    assert(_handle != G2HNULL && "g2client_play_saver is not initialized");
    return g2_play_saver_disconnect(_handle, channel);
}

bool g2play_saver::is_connecting(int channel) const
{
    assert(_handle != G2HNULL && "g2client_play_saver is not initialized");
    return g2_play_saver_is_connecting(_handle, channel);
}

bool g2play_saver::is_connected(int channel) const
{
    assert(_handle != G2HNULL && "g2client_play_saver is not initialized");
    return g2_play_saver_is_connected(_handle, channel);
}

bool g2play_saver::is_disconnecting(int channel) const
{
    assert(_handle != G2HNULL && "g2client_play_saver is not initialized");
    return g2_play_saver_is_disconnecting(_handle, channel);
}

bool g2play_saver::is_disconnected(int channel) const
{
    assert(_handle != G2HNULL && "g2client_play_saver is not initialized");
    return g2_play_saver_is_disconnected(_handle, channel);
}

bool g2play_saver::is_disconnectable(int channel) const
{
    assert(_handle != G2HNULL && "g2client_play_saver is not initialized");
    return g2_play_saver_is_disconnectable(_handle, channel);
}

//////////////////////////////////////////////////////////////////////////

bool g2play_saver::set_camera_list(int channel, const std::set<int>& channels, const G2ROLLBACK_INFO& rbi)
{
    G2CHANNEL_SET chs = { 0 };
    std::copy(channels.begin(), channels.end(), chs._channels);
    chs._len = channels.size();
    return g2_play_saver_set_camera_list(_handle, channel, &chs, &rbi);
}

bool g2play_saver::set_camera_list_interest(int channel, const std::set<int>& channels)
{
    G2CHANNEL_SET chs = { 0 };
    std::copy(channels.begin(), channels.end(), chs._channels);
    chs._len = channels.size();
    return g2_play_saver_set_camera_list_interest(_handle, channel, &chs);
}

//////////////////////////////////////////////////////////////////////////

bool g2play_saver::request_record_channels(int channel, const std::vector<G2GUID>& cameras)
{
    return g2_play_saver_request_record_channels(_handle, channel, cameras.empty() ? NULL : &cameras[0], cameras.size());
}

bool g2play_saver::request_play(int channel, const G2PLAYBACK_COMMAND& command)
{
    return g2_play_saver_request_play(_handle, channel, &command);
}

bool g2play_saver::request_pause(int channel, bool rollback, const G2ROLLBACK_INFO& rbi)
{
    return g2_play_saver_request_pause(_handle, channel, rollback, &rbi);
}

bool g2play_saver::request_move_to_spot(int channel, const G2SPOT& spot, int precision, bool forward)
{
    return g2_play_saver_request_move_to_spot(_handle, channel, &spot, precision, forward);
}

bool g2play_saver::request_notify_end_of_play(int channel)
{
    return g2_play_saver_request_notify_end_of_play(_handle, channel);
}

bool g2play_saver::request_scope_list(int channel, const G2TIME& from, const G2TIME& to, const std::set<int>& channels)
{
    G2CHANNEL_SET chs = { 0 };
    std::copy(channels.begin(), channels.end(), chs._channels);
    chs._len = channels.size();
    return g2_play_saver_request_scope_list(_handle, channel, &from, &to, &chs);
}

bool g2play_saver::request_clipcopy_measure_size(int channel, const std::set<int>& channels, const G2SCOPE& scope, unsigned __int64 free_space)
{
    G2CHANNEL_SET chs = { 0 };
    std::copy(channels.begin(), channels.end(), chs._channels);
    chs._len = channels.size();
    return g2_play_saver_request_clipcopy_measure_size(_handle, channel, &chs, &scope, free_space);
}

bool g2play_saver::request_clipcopy_password(int channel, const wchar_t* password)
{
    return g2_play_saver_request_clipcopy_password(_handle, channel, password);
}

bool g2play_saver::request_clipcopy_cancel(int channel)
{
    return g2_play_saver_request_clipcopy_cancel(_handle, channel);
}

bool g2play_saver::request_clipcopy_size(int channel)
{
    return g2_play_saver_request_clipcopy_size(_handle, channel);
}

bool g2play_saver::request_clipcopy_data(int channel)
{
    return g2_play_saver_request_clipcopy_data(_handle, channel);
}

//////////////////////////////////////////////////////////////////////////

bool g2play_saver::get_clipcopy_size_info(int channel, G2CLIPCOPY_SIZE_INFO& csi) const
{
    return g2_play_saver_get_clipcopy_size_info(_handle, channel, &csi);
}

//////////////////////////////////////////////////////////////////////////

G2RESULT g2play_saver::on_connected(G2HPLAY_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2play_saver_connected(handle, wparam));
    return 1L;
}

G2RESULT g2play_saver::on_disconnected(G2HPLAY_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2play_saver_disconnected(handle, wparam, static_cast<G2DISCONNECT_REASON::TYPE>(lparam)));
    return 1L;
}

G2RESULT g2play_saver::on_receive_record_channels(G2HPLAY_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2PARAM_BUNCH* data = (G2PARAM_BUNCH*)(lparam);
    const G2PLAY_CHANNEL_INFO* info = (G2PLAY_CHANNEL_INFO*)(data->_bunch);
    std::vector<G2PLAY_CHANNEL_INFO> bunch(info, info + data->_len);
    CALL_LISTENER(on_g2play_saver_receive_record_channels(handle, wparam, bunch));
    return 1L;
}

G2RESULT g2play_saver::on_receive_frame_data(G2HPLAY_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2FRAME* frame = (G2FRAME*)(lparam);
    CALL_LISTENER(on_g2play_saver_receive_frame_data(handle, wparam, *frame));
    return 1L;
}

G2RESULT g2play_saver::on_receive_notify_out_of_scope(G2HPLAY_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2play_saver_receive_notify_out_of_scope(handle, wparam, static_cast<G2PLAYER::OUT_OF_SCOPE::TYPE>(lparam)));
    return 1L;
}

G2RESULT g2play_saver::on_receive_notify_player_error(G2HPLAY_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2play_saver_receive_notify_player_error(handle, wparam, static_cast<G2PLAYER::PLAYER_ERROR::TYPE>(lparam)));
    return 1L;
}

G2RESULT g2play_saver::on_receive_scope_list(G2HPLAY_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2PLAY_SCOPE_LIST* data = (G2PLAY_SCOPE_LIST*)(lparam);
    std::vector<G2SCOPE> scopes(data->_list._scopes, data->_list._scopes + data->_list._len);
    CALL_LISTENER(on_g2play_saver_receive_scope_list(handle, wparam, scopes));
    return 1L;
}

G2RESULT g2play_saver::on_receive_no_recorded_data(G2HPLAY_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2play_saver_receive_no_recorded_data(handle, wparam));
    return 1L;
}

G2RESULT g2play_saver::on_receive_clipcopy_size(G2HPLAY_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2PLAY_SAVER_PARAM_CLIPCOPY_SIZE_INFO* param = (G2PLAY_SAVER_PARAM_CLIPCOPY_SIZE_INFO*)(lparam);
    CALL_LISTENER(on_g2play_saver_receive_clipcopy_size(handle, wparam, static_cast<G2CLIPCOPY_STATUS::TYPE>(param->_status), param->_info));
    return 1L;
}

G2RESULT g2play_saver::on_receive_clipcopy_data(G2HPLAY_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2CLIPCOPY_DATA* data = (G2CLIPCOPY_DATA*)(lparam);
    CALL_LISTENER(on_g2play_saver_receive_clipcopy_data(handle, wparam, data->_offset, data->_size, data->_data, data->_progress));
    return 1L;
}

G2RESULT g2play_saver::on_receive_clipcopy_canceled(G2HPLAY_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2play_saver_receive_clipcopy_canceled(handle, wparam));
    return 1L;
}

G2RESULT g2play_saver::on_receive_clipcopy_set_password(G2HPLAY_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2play_saver_receive_clipcopy_set_password(handle, wparam, lparam));
    return 1L;
}

G2RESULT g2play_saver::on_receive_clipcopy_job_started(G2HPLAY_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2play_saver_receive_clipcopy_job_started(handle, wparam, static_cast<G2CLIPCOPY_JOB::TYPE>(lparam)));
    return 1L;
}

G2RESULT g2play_saver::on_receive_clipcopy_job_finished(G2HPLAY_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2play_saver_receive_clipcopy_job_finished(handle, wparam, static_cast<G2CLIPCOPY_JOB::TYPE>(lparam)));
    return 1L;
}
