// g2client_status.h : header file
//

#ifndef _G2_CLIENT_DLL_SAMPLER_STATUS_H_
#define _G2_CLIENT_DLL_SAMPLER_STATUS_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_status.h>

namespace client {
    class g2status_listener;

//////////////////////////////////////////////////////////////////////////

class g2status
{
public:
    g2status(void);
    virtual ~g2status(void);

private:
    G2HSTATUS _handle;
    g2status_listener* _listener;

public:
    void startup(int connections, int callback_connections);
    void cleanup(void);
    void set_listener(g2status_listener* listener);

    bool callback_server_restart(unsigned short port);
    void callback_server_shutdown(void);
    bool callback_server_is_startup(void) const;

    G2HSTATUS safe_handle(void) const { return this != NULL ? _handle : G2HNULL; }

public:
    int  connect(const G2GUID& root, const G2CONNECT_OPTIONS* options = NULL, G2CONNECT_RES* res = NULL);
    int  connect_ras(const G2NETWORK_INFO& ni, bool idr, const G2CONNECT_OPTIONS* options = NULL, G2CONNECT_RES* res = NULL);
    void disconnect(int channel);
    bool is_connecting(int channel) const;
    bool is_connected(int channel) const;
    bool is_disconnecting(int channel) const;
    bool is_disconnected(int channel) const;
    bool is_disconnectable(int channel) const;

public:
    bool request_alive_check(int channel);
    bool request_panic_record(int channel, bool on);
    bool request_status(int channel);
    bool request_log(int channel, const G2STATUS_LOG_SEARCH_OPTIONS& options, G2STATUS_LOG::RESULT& res);

public:
    bool get_server_network_info(int channel, G2SERVER_NETWORK_INFO& ni) const;
    bool get_product_info(int channel, G2_PRODUCT_INFO& pi) const;
    bool get_remote_status_caps(int channel, G2_PRODUCT_INFO_CAPS::REMOTE_STATUS& caps) const;
    bool get_authority(int channel, G2RAS_AUTHORITY& auth) const;
    bool get_status(int channel, G2MONITORING_DEVICE_STATUS& status) const;

public:
    bool is_support(int channel, G2STATUS_SUPPORT::QUERY query) const;
    bool is_authority(int channel, G2RAS_AUTHORITY::TYPE authority) const;
    bool is_IDR(int channel) const;

protected:
    static G2RESULT G2CALLBACK on_connected(G2HSTATUS handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_disconnected(G2HSTATUS handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_status(G2HSTATUS handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_callback_event(G2HSTATUS handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_log_system(G2HSTATUS handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_log_event(G2HSTATUS handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_log_debug(G2HSTATUS handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2_CLIENT_DLL_SAMPLER_STATUS_H_
