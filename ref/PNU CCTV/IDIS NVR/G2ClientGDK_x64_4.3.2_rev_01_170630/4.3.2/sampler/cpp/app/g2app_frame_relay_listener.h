// g2app_frame_relay_listener.h : header file
//

#ifndef _G2_CLIENT_DLL_SAMPLER_APP_FRAME_RELAY_LISTENER_H_
#define _G2_CLIENT_DLL_SAMPLER_APP_FRAME_RELAY_LISTENER_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/app/g2_app_frame_relay.h>
#include <string>

namespace client {

//////////////////////////////////////////////////////////////////////////

class g2app_frame_relay_listener
{
public:
    virtual void on_g2app_frame_relay_connected(G2HANDLE handle, int channel) = 0;
    virtual void on_g2app_frame_relay_disconnected(G2HANDLE handle, int channel, int reason) = 0;
    virtual void on_g2app_frame_relay_receive_request_site_product_info(G2HANDLE handle, int channel) = 0;
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2_CLIENT_DLL_SAMPLER_APP_FRAME_RELAY_LISTENER_H_
