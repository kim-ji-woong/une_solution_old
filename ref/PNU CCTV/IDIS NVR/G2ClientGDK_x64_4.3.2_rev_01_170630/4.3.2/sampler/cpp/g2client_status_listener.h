// g2client_status_listener.h : header file
//

#ifndef _G2_CLIENT_DLL_SAMPLER_STATUS_LISTENER_H_
#define _G2_CLIENT_DLL_SAMPLER_STATUS_LISTENER_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_status.h>
#include <string>
#include <list>

namespace client {

//////////////////////////////////////////////////////////////////////////

class g2status_listener
{
public:
    virtual void on_g2status_connected(G2HSTATUS handle, int channel) = 0;
    virtual void on_g2status_disconnected(G2HSTATUS handle, int channel, G2DISCONNECT_REASON::TYPE reason) = 0;
    virtual void on_g2status_receive_status(G2HSTATUS handle, int channel, const G2MONITORING_DEVICE_STATUS& status) = 0;
    virtual void on_g2status_receive_callback_event(G2HSTATUS handle, const G2EVENT_INFO& ei) = 0;
    virtual void on_g2status_receive_log_system(G2HSTATUS handle, int channel, const std::list<G2STATUS_LOG_SYSTEM>& logs) = 0;
    virtual void on_g2status_receive_log_event(G2HSTATUS handle, int channel, const std::list<G2STATUS_LOG_EVENT>& logs) = 0;
    virtual void on_g2status_receive_log_debug(G2HSTATUS handle, int channel, const std::list<G2STATUS_LOG_SYSTEM>& logs) = 0;
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2_CLIENT_DLL_SAMPLER_STATUS_LISTENER_H_
