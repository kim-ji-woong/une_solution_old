// g2client_dualaudio_listener.h : header file
//

#ifndef _G2_CLIENT_DLL_SAMPLER_DUALAUDIO_LISTENER_H_
#define _G2_CLIENT_DLL_SAMPLER_DUALAUDIO_LISTENER_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_dualaudio.h>

namespace client {

//////////////////////////////////////////////////////////////////////////

class g2dualaudio_listener
{
public:
    virtual void on_g2dualaudio_connected(G2HDUALAUDIO handle, int channel) = 0;
    virtual void on_g2dualaudio_disconnected(G2HDUALAUDIO handle, int channel, G2DISCONNECT_REASON::TYPE reason) = 0;
    virtual void on_g2dualaudio_receive_audio_in_opened(G2HDUALAUDIO handle, int channel, const G2GUID& site) = 0;
    virtual void on_g2dualaudio_receive_audio_in_closed(G2HDUALAUDIO handle, int channel, const G2GUID& site) = 0;
    virtual void on_g2dualaudio_receive_audio_out_opened(G2HDUALAUDIO handle, int channel, const G2GUID& site) = 0;
    virtual void on_g2dualaudio_receive_audio_out_closed(G2HDUALAUDIO handle, int channel, const G2GUID& site) = 0;
    virtual void on_g2dualaudio_require_select_channel(G2HDUALAUDIO handle, int channel, const G2GUID& site, int count, int& number) = 0;
    virtual bool on_g2dualaudio_require_reconnect_imp(G2HDUALAUDIO handle, const G2GUID& site) = 0;
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2_CLIENT_DLL_SAMPLER_DUALAUDIO_LISTENER_H_
