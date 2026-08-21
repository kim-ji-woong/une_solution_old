// g2client_monitor_listener.h : header file
//

#ifndef _G2_CLIENT_DLL_SAMPLER_MONITOR_LISTENER_H_
#define _G2_CLIENT_DLL_SAMPLER_MONITOR_LISTENER_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_monitor.h>
#include <vector>

namespace client {

//////////////////////////////////////////////////////////////////////////

class g2monitor_listener
{
public:
    virtual void on_g2monitor_connected(G2HMONITOR handle, const G2GUID& service) = 0;
    virtual void on_g2monitor_disconnected(G2HMONITOR handle, const G2GUID& service, G2DISCONNECT_REASON::TYPE reason) = 0;
    virtual void on_g2monitor_require_reconnect(G2HMONITOR handle, const G2GUID& service, bool& reconnect) = 0;
    virtual void on_g2monitor_receive_event(G2HMONITOR handle, const G2EVENT_INFO& ei) = 0;
    virtual void on_g2monitor_receive_service_log_load(G2HMONITOR handle, const G2SYSTEM_LOG& log) = 0;
    virtual void on_g2monitor_receive_service_log_load_end(G2HMONITOR handle) = 0;
    virtual void on_g2monitor_receive_service_log_load_fail(G2HMONITOR handle) = 0;
    virtual void on_g2monitor_receive_service_log_load_stop(G2HMONITOR handle) = 0;
    virtual void on_g2monitor_status_connected(G2HMONITOR handle, const G2GUID& service) = 0;
    virtual void on_g2monitor_status_disconnected(G2HMONITOR handle, const G2GUID& service, G2DISCONNECT_REASON::TYPE reason) = 0;
    virtual void on_g2monitor_status_receive_device_status(G2HMONITOR handle, const std::vector<G2MONITORING_DEVICE_STATUS>& status) = 0;
    virtual void on_g2monitor_status_receive_device_health(G2HMONITOR handle, const std::vector<G2MONITORING_DEVICE_HEALTH>& health) = 0;
    virtual void on_g2monitor_status_receive_event(G2HMONITOR handle, const G2EVENT_INFO& ei) = 0;
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2_CLIENT_DLL_SAMPLER_MONITOR_LISTENER_H_
