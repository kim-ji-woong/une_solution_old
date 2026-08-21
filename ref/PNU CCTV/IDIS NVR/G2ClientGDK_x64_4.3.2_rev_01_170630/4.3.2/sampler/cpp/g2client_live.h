// g2client_live.h : header file
//

#ifndef _G2_CLIENT_DLL_SAMPLER_LIVE_H_
#define _G2_CLIENT_DLL_SAMPLER_LIVE_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_live.h>
#include <vector>
#include <set>
#include <utility>

namespace client {
    class g2live_listener;

//////////////////////////////////////////////////////////////////////////

class g2live
{
public:
    g2live(void);
    virtual ~g2live(void);

private:
    G2HLIVE _handle;
    g2live_listener* _listener;

public:
    void startup(int connections, G2HWND focus);
    void cleanup(void);
    void set_listener(g2live_listener* listener);

    G2HLIVE safe_handle(void) const { return this != NULL ? _handle : G2HNULL; }

public:
    int  connect(const G2GUID& service, const G2CONNECT_OPTIONS* options = NULL, G2CONNECT_RES* res = NULL);
    void disconnect(int channel);
    bool is_connecting(int channel) const;
    bool is_connected(int channel) const;
    bool is_disconnecting(int channel) const;
    bool is_disconnected(int channel) const;
    bool is_disconnectable(int channel) const;

public:
    bool set_camera_list(int channel, const std::set<int>& channels, bool force);
    bool set_camera_list_interest(int channel, const std::set<int>& channels, bool force);
    bool set_camera_stream_set(int channel, const std::set<std::pair<int, int> >& streams, bool force);
    bool set_camera_stream_channel(int channel, int channelext, int stream_id);
    bool set_audio_list(int channel, const std::set<int>& channels, bool force);
    bool set_stream_id(int channel, const G2GUID& camera, int id);
    bool set_alarm_out(int channel, const G2GUID& alarm, bool on);
    bool set_camera_color(int channel, const G2GUID& camera, int type, int value);
    bool set_ptz_command(int channel, const G2GUID& camera, const G2LIVE_PTZ_COMMAND& command);
    bool set_ptz_preset(int channel, const G2GUID& camera, const G2LIVE_PTZ_PRESET& preset);
    bool set_enable_audio_streaming(int channel, const G2GUID& camera, bool enable);
    bool set_enable_audio_capturing(int channel, const G2GUID& root, int camera, bool enable);
    bool set_disable_audio_streaming(int channel, const G2GUID& root);
    bool set_disable_audio_capturing(int channel, const G2GUID& root);
    bool set_disable_audio(int channel);
    bool set_disable_audio(int channel, const G2GUID& root);
    void set_probe_session_profile(bool active);

public:
    bool request_stream_channels(int channel);
    bool request_stream_channels_each(int channel, const std::vector<G2GUID>& cameras);
    bool request_alive_check(int channel, int check);
    bool request_ptz_menu(int channel, const G2GUID& camera);
    bool request_ptz_preset(int channel, const G2GUID& camera);
    bool request_system_log_search(int channel, const G2SERVICE_SEARCH_OPTION_SYSTEM_LOG& option);
    bool request_system_log_search_stop(int channel);

public:
    bool send_command_control_color(int channel, const G2GUID& cameraGUID, int camera, const G2LIVE_COMMAND_CONTROL_COLOR& control);
    bool send_command_control_color_status(int channel, const G2GUID& cameraGUID, int camera);
    bool send_command_control_ptz(int channel, const G2GUID& cameraGUID, int camera, const G2LIVE_COMMAND_CONTROL_PTZ& control, bool req_result);
    bool send_command_control_ptz_status(int channel, const G2GUID& cameraGUID, int camera);

public:
    bool get_camera_list(int channel, std::set<int>& channels) const;
    bool get_camera_list_interest(int channel, std::set<int>& channels) const;
    bool get_camera_stream_set(int channel, std::set<std::pair<int, int> >& streams) const;
    bool get_camera_stream_set(int channel, int channelext, std::set<std::pair<int, int> >& streams) const;
    G2GUID get_guid_from_channelext(int channel, int channelext) const;
    int  get_channelext_from_guid(int channel, const G2GUID& camera) const;
    int  get_stream_id(int channel, const G2GUID& camera) const;
    int  get_stream_count(int channel, const G2GUID& camera) const;
    unsigned int get_stream_remote(int channel, const G2GUID& camera) const;
    bool get_camera_status(int channel, const G2GUID& camera, G2LIVE_CAMERA_STATUS& status) const;
    int  get_camera_status_ptz_function(int channel, const G2GUID& camera) const;
    bool get_camera_status_stream_info(int channel, const G2GUID& camera, int stream_id, G2DEVICE_STATUS_STREAM_INFO& status);

public:
    bool is_enable_multi_stream(int channel, const G2GUID& camera) const;
    bool is_enable_audio_in(int channel, const G2GUID& camera) const;
    bool is_enable_audio_out(int channel, const G2GUID& root, int camera) const;
    bool is_contains_audio_streaming(int channel, const G2GUID& root) const;
    bool is_contains_audio_capturing(int channel, const G2GUID& root, int camera) const;
    bool is_contains_audio_capturing(int channel, const G2GUID& root) const;
    bool is_contains_audio(int channel, const G2GUID& root) const;
    bool is_audio_out_opening(int channel, const G2GUID& root, int camera) const;
    bool is_support(int channel, const G2GUID& camera, G2LIVE_SUPPORT::QUERY query) const;
    bool is_enable_command_control_ptz(int channel, const G2GUID& camera) const;
    bool is_enable_command_control_color(int channel, const G2GUID& camera) const;

protected:
    static G2RESULT G2CALLBACK on_connected(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_disconnected(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_stream_channels(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_frame_data(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_text_in(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_camera_status(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_event(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_ptz_menu(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_ptz_preset(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_service_log_load(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_service_log_load_end(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_service_log_load_fail(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_service_log_load_stop(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_audio_out_not_available(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_notify_append_device(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_notify_remove_device(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_network_alarm_result(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_audio_streaming_started(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_audio_streaming_stopped(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_audio_capturing_started(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_audio_capturing_stopped(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_probe_session_profile(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_command_result_control_color_status(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_command_result_control_color(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_command_result_control_ptz_status(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_command_result_control_ptz(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);

};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2_CLIENT_DLL_SAMPLER_LIVE_H_
