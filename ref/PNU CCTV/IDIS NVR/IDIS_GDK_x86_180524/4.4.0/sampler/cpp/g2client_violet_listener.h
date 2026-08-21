// g2client_violet_listener.h : header file
//

#ifndef _G2_CLIENT_DLL_SAMPLER_VIOLET_LISTENER_H_
#define _G2_CLIENT_DLL_SAMPLER_VIOLET_LISTENER_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_violet.h>
#include <vector>

namespace client {

//////////////////////////////////////////////////////////////////////////

class g2violet_listener
{
public:
    virtual void on_g2violet_connected(G2HVIOLET handle, int channel) = 0;
    virtual void on_g2violet_disconnected(G2HVIOLET handle, int channel, G2DISCONNECT_REASON::TYPE reason) = 0;
    virtual void on_g2violet_receive_service_log_load(G2HVIOLET handle, int channel, const G2SYSTEM_LOG& log) = 0;
    virtual void on_g2violet_receive_service_log_load_end(G2HVIOLET handle, int channel) = 0;
    virtual void on_g2violet_receive_service_log_load_fail(G2HVIOLET handle, int channel) = 0;
    virtual void on_g2violet_receive_service_log_load_stop(G2HVIOLET handle, int channel) = 0;
    virtual void on_g2violet_receive_agent_audio_connected(G2HVIOLET handle, int channel, int monitor) = 0;
    virtual void on_g2violet_receive_agent_audio_disconnected(G2HVIOLET handle, int channel, int monitor) = 0;
    virtual void on_g2violet_receive_agent_layout_attached(G2HVIOLET handle, int channel, const G2GUID& agent, int monitor, const G2GUID& layout) = 0;
    virtual void on_g2violet_receive_cameras_on_monitor(G2HVIOLET handle, int channel, const G2GUID& agent, int monitor, const std::vector<G2GUID>& cameras) = 0;
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2_CLIENT_DLL_SAMPLER_VIOLET_LISTENER_H_
