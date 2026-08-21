// g2app_frame_relay.h : header file
//

#ifndef _G2_CLIENT_DLL_SAMPLER_APP_FRAME_RELAY_H_
#define _G2_CLIENT_DLL_SAMPLER_APP_FRAME_RELAY_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/app/g2_app_frame_relay.h>
#include <string>

namespace client {
    class g2app_frame_relay_listener;

//////////////////////////////////////////////////////////////////////////

class g2app_frame_relay
{
public:
    g2app_frame_relay(void);
    virtual ~g2app_frame_relay(void);

private:
    G2HANDLE _handle;
    g2app_frame_relay_listener* _listener;

public:
    void startup(int connections, unsigned int send_queue_size_limit = 0);
    void cleanup(void);
    void set_listener(g2app_frame_relay_listener* listener);

    G2HANDLE safe_handle(void) const { return this != NULL ? _handle : G2HNULL; }

public:
    int  connect(const G2NETWORK_INFO& ni, const G2CONNECT_OPTIONS* options = NULL, G2CONNECT_RES* res = NULL);
    void disconnect(int channel);
    bool is_connecting(int channel) const;
    bool is_connected(int channel) const;
    bool is_disconnecting(int channel) const;
    bool is_disconnected(int channel) const;
    bool is_disconnectable(int channel) const;

public:
    bool request_alive_check(int channel, int val);
    bool send_site_connected(int channel, const G2STRING_64& site);
    bool send_site_disconnected(int channel, const G2STRING_64& site, int reason);
    bool send_site_product_info(int channel, const G2STRING_64& site, bool(G2API *get_adaptor)(G2HANDLE, void*), G2HANDLE from_handle, int from_channel);
    bool send_site_frame_data(int channel, const G2STRING_64& site, const G2FRAME& frame);

public:
    bool is_able_to_send(int channel, unsigned int size) const;

public:
    static bool search_controller_option_read_file(const std::wstring& path, G2APP_FRAME_RELAY_SEARCH_CONTROLLER_OPTION& option);

protected:
    static G2RESULT G2CALLBACK on_connected(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_disconnected(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_request_site_product_info(G2HANDLE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2_CLIENT_DLL_SAMPLER_APP_FRAME_RELAY_H_
