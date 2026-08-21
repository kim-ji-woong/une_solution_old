// g2client_device_info_listener.h : header file
//

#ifndef _G2_CLIENT_DLL_SAMPLER_DEVICE_INFO_LISTENER_H_
#define _G2_CLIENT_DLL_SAMPLER_DEVICE_INFO_LISTENER_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_device_info.h>

namespace client {

//////////////////////////////////////////////////////////////////////////

class g2device_info_listener
{
public:
    virtual void on_g2device_info_receive_product_info(G2HDEVICE_INFO handle, int channel, G2DISCONNECT_REASON::TYPE reason, const G2_PRODUCT_INFO& gpi) = 0;
    virtual void on_g2device_info_failed(G2HDEVICE_INFO handle, int channel, G2DISCONNECT_REASON::TYPE reason) = 0;
    virtual void on_g2device_info_canceled(G2HDEVICE_INFO handle, int channel) = 0;
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2_CLIENT_DLL_SAMPLER_DEVICE_INFO_LISTENER_H_
