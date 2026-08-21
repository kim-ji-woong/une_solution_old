// g2client_monitor.cpp : implementation file
//

#include "stdafx.h"
#include "g2client_guid.h"
#include "g2client_monitor.h"
#include "g2client_monitor_listener.h"

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
    g2_monitor_register_callback(_handle, G2MONITOR_CALLBACK::function, function);

#define CALL_LISTENER(param) {  \
    g2monitor_listener* ptr = ((g2monitor*)(uparam))->_listener; \
    if (ptr) ptr->param;        \
}

//////////////////////////////////////////////////////////////////////////

g2monitor::g2monitor(void)
    : _handle(G2HNULL)
    , _listener(NULL)
{
    _handle = g2_monitor_initialize((G2UPARAM)(this));

    REGISTER_CALLBACK(on_connected);
    REGISTER_CALLBACK(on_disconnected);
    REGISTER_CALLBACK(on_require_reconnect);
    REGISTER_CALLBACK(on_receive_event);
    REGISTER_CALLBACK(on_receive_service_log_load);
    REGISTER_CALLBACK(on_receive_service_log_load_end);
    REGISTER_CALLBACK(on_receive_service_log_load_fail);
    REGISTER_CALLBACK(on_receive_service_log_load_stop);
    REGISTER_CALLBACK(on_status_connected);
    REGISTER_CALLBACK(on_status_disconnected);
    REGISTER_CALLBACK(on_status_receive_device_status);
    REGISTER_CALLBACK(on_status_receive_device_health);
    REGISTER_CALLBACK(on_status_receive_event);
}

g2monitor::~g2monitor(void)
{
    g2_monitor_finalize(_handle);
    _handle = G2HNULL;
}

g2monitor& g2monitor::get(void)
{
    static g2monitor s_singleton;
    return s_singleton;
}

g2monitor* g2monitor::from_handle(G2HADMIN handle)
{
    return &get();
}

//////////////////////////////////////////////////////////////////////////


void g2monitor::startup(void)
{
    assert(_handle != G2HNULL && "g2client_monitor is not initialized");
    g2_monitor_startup(_handle);
}

void g2monitor::cleanup(void)
{
    assert(_handle != G2HNULL && "g2client_monitor is not initialized");
    g2_monitor_cleanup(_handle);
}

void g2monitor::set_listener(g2monitor_listener* listener)
{
    _listener = listener;
}

//////////////////////////////////////////////////////////////////////////

bool g2monitor::connect(const G2GUID& service, const G2CONNECT_OPTIONS* options /*= NULL*/, G2CONNECT_RES* res /*= NULL*/)
{
    assert(_handle != G2HNULL && "g2client_monitor is not initialized");
    return g2_monitor_connect(_handle, &service, options, res);
}

void g2monitor::disconnect(void)
{
    assert(_handle != G2HNULL && "g2client_monitor is not initialized");
    g2_monitor_disconnect(_handle);
}

bool g2monitor::is_connecting(void) const
{
    assert(_handle != G2HNULL && "g2client_monitor is not initialized");
    return g2_monitor_is_connecting(_handle);
}

bool g2monitor::is_connected(void) const
{
    assert(_handle != G2HNULL && "g2client_monitor is not initialized");
    return g2_monitor_is_connected(_handle);
}

bool g2monitor::is_disconnecting(void) const
{
    assert(_handle != G2HNULL && "g2client_monitor is not initialized");
    return g2_monitor_is_disconnecting(_handle);
}

bool g2monitor::is_disconnected(void) const
{
    assert(_handle != G2HNULL && "g2client_monitor is not initialized");
    return g2_monitor_is_disconnected(_handle);
}

bool g2monitor::is_disconnectable(void) const
{
    assert(_handle != G2HNULL && "g2client_monitor is not initialized");
    return g2_monitor_is_disconnectable(_handle);
}

//////////////////////////////////////////////////////////////////////////

void g2monitor::set_disconnect_by_me(bool me)
{
    g2_monitor_set_disconnect_by_me(_handle, me);
}

void g2monitor::set_activate(bool activate)
{
    g2_monitor_set_activate(_handle, activate);
}

bool g2monitor::set_device_list_interest_for_live(const std::vector<G2GUID>& GUIDs, bool invoke)
{
    return g2_monitor_set_device_list_interest_for_live(_handle, GUIDs.empty() ? NULL : &GUIDs[0], GUIDs.size(), invoke);
}

bool g2monitor::set_device_list_interest_for_status(const std::vector<G2GUID>& GUIDs)
{
    return g2_monitor_set_device_list_interest_for_status(_handle, GUIDs.empty() ? NULL : &GUIDs[0], GUIDs.size());
}

bool g2monitor::set_device_list_interest_for_health(const std::vector<G2GUID>& GUIDs)
{
    return g2_monitor_set_device_list_interest_for_health(_handle, GUIDs.empty() ? NULL : &GUIDs[0], GUIDs.size());
}

//////////////////////////////////////////////////////////////////////////

bool g2monitor::request_alive_check(void)
{
    return g2_monitor_request_alive_check(_handle);
}

bool g2monitor::request_update_device_list_interest_for_status(bool force)
{
    return g2_monitor_request_update_device_list_interest_for_status(_handle, force);
}

bool g2monitor::request_update_device_list_interest_for_health(bool force)
{
    return g2_monitor_request_update_device_list_interest_for_health(_handle, force);
}

void g2monitor::request_delete_device_status_and_health(void)
{
    g2_monitor_request_delete_device_status_and_health(_handle);
}

bool g2monitor::request_user_alarm_in_command(const G2MONITORING_USER_ALARM_IN_REQUEST& request)
{
    return g2_monitor_request_user_alarm_in_command(_handle, &request);
}

bool g2monitor::request_system_log_search(const G2SERVICE_SEARCH_OPTION_SYSTEM_LOG& option)
{
    return g2_monitor_request_system_log_search(_handle, &option);
}

bool g2monitor::request_system_log_search_stop(void)
{
    return g2_monitor_request_system_log_search_stop(_handle);
}

bool g2monitor::get_service_guid(G2GUID& service) const
{
    return g2_monitor_get_service_guid(_handle, &service);
}

G2GUID g2monitor::get_service_guid(void) const
{
    G2GUID service = g2guid::INVALID_GUID;
    g2_monitor_get_service_guid(_handle, &service);
    return service;
}

//////////////////////////////////////////////////////////////////////////

bool g2monitor::is_activate(void) const
{
    return g2_monitor_is_activate(_handle);
}

bool g2monitor::is_monitoring(void) const
{
    return g2_monitor_is_monitoring(_handle);
}

bool g2monitor::is_disconnect_by_me(void) const
{
    return g2_monitor_is_disconnect_by_me(_handle);
}

//////////////////////////////////////////////////////////////////////////

G2RESULT g2monitor::on_connected(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2GUID* service = (G2GUID*)(wparam);
    CALL_LISTENER(on_g2monitor_connected(handle, *service));
    return 1L;
}

G2RESULT g2monitor::on_disconnected(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2GUID* service = (G2GUID*)(wparam);
    CALL_LISTENER(on_g2monitor_disconnected(handle, *service, (G2DISCONNECT_REASON::TYPE)(lparam)));
    return 1L;
}

G2RESULT g2monitor::on_require_reconnect(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2GUID* service = (G2GUID*)(wparam);
    G2BOOL* reconnect = (G2BOOL*)(lparam);
    bool value = (*reconnect != G2FALSE);
    CALL_LISTENER(on_g2monitor_require_reconnect(handle, *service, value));
    *reconnect = value ? G2TRUE : G2FALSE;
    return 1L;
}

G2RESULT g2monitor::on_receive_event(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2EVENT_INFO* ei = (G2EVENT_INFO*)(lparam);
    CALL_LISTENER(on_g2monitor_receive_event(handle, *ei));
    return 1L;
}

G2RESULT g2monitor::on_receive_service_log_load(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2SYSTEM_LOG* log = (G2SYSTEM_LOG*)(lparam);
    CALL_LISTENER(on_g2monitor_receive_service_log_load(handle, *log));
    return 1L;
}

G2RESULT g2monitor::on_receive_service_log_load_end(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2monitor_receive_service_log_load_end(handle));
    return 1L;
}

G2RESULT g2monitor::on_receive_service_log_load_fail(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2monitor_receive_service_log_load_fail(handle));
    return 1L;
}

G2RESULT g2monitor::on_receive_service_log_load_stop(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    CALL_LISTENER(on_g2monitor_receive_service_log_load_stop(handle));
    return 1L;
}

G2RESULT g2monitor::on_status_connected(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2GUID* service = (G2GUID*)(wparam);
    CALL_LISTENER(on_g2monitor_status_connected(handle, *service));
    return 1L;
}

G2RESULT g2monitor::on_status_disconnected(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2GUID* service = (G2GUID*)(wparam);
    CALL_LISTENER(on_g2monitor_status_disconnected(handle, *service, (G2DISCONNECT_REASON::TYPE)(lparam)));
    return 1L;
}

G2RESULT g2monitor::on_status_receive_device_status(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2PARAM_BUNCH* data = (G2PARAM_BUNCH*)(lparam);
    G2MONITORING_DEVICE_STATUS* status = (G2MONITORING_DEVICE_STATUS*)(data->_bunch);
    std::vector<G2MONITORING_DEVICE_STATUS> bunch(status, status + data->_len);
    CALL_LISTENER(on_g2monitor_status_receive_device_status(handle, bunch));
    return 1L;
}

G2RESULT g2monitor::on_status_receive_device_health(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2PARAM_BUNCH* data = (G2PARAM_BUNCH*)(lparam);
    G2MONITORING_DEVICE_HEALTH* health = (G2MONITORING_DEVICE_HEALTH*)(data->_bunch);
    std::vector<G2MONITORING_DEVICE_HEALTH> bunch(health, health + data->_len);
    CALL_LISTENER(on_g2monitor_status_receive_device_health(handle, bunch));
    return 1L;
}

G2RESULT g2monitor::on_status_receive_event(G2HMONITOR handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam)
{
    G2EVENT_INFO* data = (G2EVENT_INFO*)(lparam);
    CALL_LISTENER(on_g2monitor_status_receive_event(handle, *data));
    return 1L;
}
