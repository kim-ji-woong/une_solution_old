// g2client_backup_saver.h : header file
//

#ifndef _G2_CLIENT_DLL_SAMPLER_BACKUP_SAVER_H_
#define _G2_CLIENT_DLL_SAMPLER_BACKUP_SAVER_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_backup_saver.h>
#include <vector>
#include <set>
#include <map>

namespace client {
    class g2backup_saver_listener;

//////////////////////////////////////////////////////////////////////////

class g2backup_saver
{
public:
    g2backup_saver(void);
    virtual ~g2backup_saver(void);

private:
    G2HBACKUP_SAVER _handle;
    g2backup_saver_listener* _listener;

public:
    void startup(int connections);
    void cleanup(void);
    void set_listener(g2backup_saver_listener* listener);

    G2HPLAY_SAVER safe_handle(void) const { return this != NULL ? _handle : G2HNULL; }

public:
    int  connect(const G2GUID& service, const G2GUID& site, const G2CONNECT_OPTIONS* options = NULL, G2CONNECT_RES* res = NULL);
    void disconnect(int channel);
    bool is_connecting(int channel) const;
    bool is_connected(int channel) const;
    bool is_disconnecting(int channel) const;
    bool is_disconnected(int channel) const;
    bool is_disconnectable(int channel) const;

public:
    bool set_camera_list(int channel, const std::set<int>& channels, const G2ROLLBACK_INFO& rbi);
    bool set_camera_list_interest(int channel, const std::set<int>& channels);

public:
    bool request_record_channels(int channel, const std::vector<G2GUID>& cameras);
    bool request_query_no_recorded_data(int channel, const std::vector<G2GUID>& cameras);
    bool request_play(int channel, const G2PLAYBACK_COMMAND& command);
    bool request_pause(int channel, bool rollback, const G2ROLLBACK_INFO& rbi);
    bool request_move_to_spot(int channel, const G2SPOT& spot, int precision, bool forward);
    bool request_notify_end_of_play(int channel);
    bool request_scope_list(int channel, const G2TIME& from, const G2TIME& to, const std::set<int>& channels);
    bool request_clipcopy_measure_size(int channel, const std::set<int>& channels, const G2SCOPE& scope, unsigned __int64 free_space);
    bool request_clipcopy_password(int channel, const wchar_t* password);
    bool request_clipcopy_text_in(int channel, bool include);
    bool request_clipcopy_cancel(int channel);
    bool request_clipcopy_size(int channel);
    bool request_clipcopy_data(int channel);

public:
    bool get_clipcopy_size_info(int channel, G2CLIPCOPY_SIZE_INFO& csi) const;

protected:
    static G2RESULT G2CALLBACK on_connected(G2HBACKUP_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_disconnected(G2HBACKUP_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_backup_site_result(G2HBACKUP_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_record_channels(G2HBACKUP_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_response_no_recorded_data(G2HBACKUP handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_frame_data(G2HBACKUP_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_notify_out_of_scope(G2HBACKUP_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_notify_player_error(G2HBACKUP_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_scope_list(G2HBACKUP_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_no_recorded_data(G2HBACKUP_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_clipcopy_size(G2HBACKUP_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_clipcopy_data(G2HBACKUP_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_clipcopy_canceled(G2HBACKUP_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_clipcopy_set_password(G2HBACKUP_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_clipcopy_job_started(G2HBACKUP_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_clipcopy_job_finished(G2HBACKUP_SAVER handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2_CLIENT_DLL_SAMPLER_BACKUP_SAVER_H_
