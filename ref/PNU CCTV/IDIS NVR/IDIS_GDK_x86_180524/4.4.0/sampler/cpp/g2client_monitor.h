// g2client_monitor.h : header file
//

#ifndef _G2_CLIENT_DLL_SAMPLER_MONITOR_H_
#define _G2_CLIENT_DLL_SAMPLER_MONITOR_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_monitor.h>
#include <vector>

namespace client {
    class g2monitor_listener;

//////////////////////////////////////////////////////////////////////////

class g2monitor
{
protected:
    g2monitor(void);

public:
    ~g2monitor(void);

    static g2monitor& get(void);
    static g2monitor* from_handle(G2HMONITOR handle);

private:
    G2HMONITOR _handle;
    g2monitor_listener* _listener;

public:
    void startup(void);
    void cleanup(void);
    void set_listener(g2monitor_listener* listener);

    G2HMONITOR safe_handle(void) const { return this != NULL ? _handle : G2HNULL; }

public:
    bool connect(const G2GUID& service, const G2CONNECT_OPTIONS* options = NULL, G2CONNECT_RES* res = NULL);
    void disconnect(void);
    bool is_connecting(void) const;
    bool is_connected(void) const;
    bool is_disconnecting(void) const;
    bool is_disconnected(void) const;
    bool is_disconnectable(void) const;

public:
    void set_activate(bool activate);
    void set_disconnect_by_me(bool me);
    bool set_device_list_interest_for_live(const std::vector<G2GUID>& GUIDs, bool invoke);
    bool set_device_list_interest_for_status(const std::vector<G2GUID>& GUIDs);
    bool set_device_list_interest_for_health(const std::vector<G2GUID>& GUIDs);

public:
    bool request_alive_check(void);
    bool request_update_device_list_interest_for_status(bool force);
    bool request_update_device_list_interest_for_health(bool force);
    void request_delete_device_status_and_health(void);
    bool request_user_alarm_in_command(const G2MONITORING_USER_ALARM_IN_REQUEST& request);
    bool request_system_log_search(const G2SERVICE_SEARCH_OPTION_SYSTEM_LOG& option);
    bool request_system_log_search_stop(void);

public:
    bool get_service_guid(G2GUID& service) const;
    G2GUID get_service_guid(void) const;

public:
    bool is_activate(void) const;
    bool is_monitoring(void) const;
    bool is_disconnect_by_me(void) const;

protected:
    static G2RESULT G2CALLBACK on_connected(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_disconnected(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_require_reconnect(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_event(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_service_log_load(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_service_log_load_end(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_service_log_load_fail(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_service_log_load_stop(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_status_connected(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_status_disconnected(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_status_receive_device_status(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_status_receive_device_health(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_status_receive_event(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2_CLIENT_DLL_SAMPLER_MONITOR_H_
