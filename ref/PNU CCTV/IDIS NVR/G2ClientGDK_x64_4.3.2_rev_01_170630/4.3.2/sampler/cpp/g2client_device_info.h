// g2client_device_info.h : header file
//

#ifndef _G2_CLIENT_DLL_SAMPLER_DEVICE_INFO_H_
#define _G2_CLIENT_DLL_SAMPLER_DEVICE_INFO_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_device_info.h>

namespace client {
    class g2device_info_listener;

//////////////////////////////////////////////////////////////////////////

class g2device_info
{
public:
    g2device_info(void);
    virtual ~g2device_info(void);

private:
    G2HDEVICE_INFO _handle;
    g2device_info_listener* _listener;

public:
    void startup(int connections);
    void cleanup(void);
    void set_listener(g2device_info_listener* listener);

    G2HDEVICE_INFO safe_handle(void) const { return this != NULL ? _handle : G2HNULL; }

public:
    int  connect_ras(const G2NETWORK_INFO& ni, const G2CONNECT_OPTIONS* options = NULL, G2CONNECT_RES* res = NULL);
    void disconnect(int channel);
    bool is_connecting(int channel) const;
    bool is_connected(int channel) const;
    bool is_disconnecting(int channel) const;
    bool is_disconnected(int channel) const;
    bool is_disconnectable(int channel) const;

public:
    bool get_product_info(int channel, G2_PRODUCT_INFO& pi) const;
    bool get_authority(int channel, G2RAS_AUTHORITY& auth) const;

public:
    bool is_authenticated(int channel) const;
    bool is_IDR_X032(int channel) const;
    bool is_IDR(int channel) const;

protected:
    static G2RESULT G2CALLBACK on_receive_product_info(G2HDEVICE_INFO handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_failed(G2HDEVICE_INFO handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_canceled(G2HDEVICE_INFO handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2_CLIENT_DLL_SAMPLER_DEVICE_INFO_H_
