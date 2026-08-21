// g2client_dualaudio.h : header file
//

#ifndef _G2_CLIENT_DLL_SAMPLER_DUALAUDIO_H_
#define _G2_CLIENT_DLL_SAMPLER_DUALAUDIO_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_dualaudio.h>

namespace client {
    class g2dualaudio_listener;

//////////////////////////////////////////////////////////////////////////

class g2dualaudio
{
public:
    g2dualaudio(void);
    virtual ~g2dualaudio(void);

private:
    G2HDUALAUDIO _handle;
    g2dualaudio_listener* _listener;

public:
    void startup(int connections, G2HWND focus);
    void cleanup(void);
    void set_listener(g2dualaudio_listener* listener);

    G2HDUALAUDIO safe_handle(void) const { return this != NULL ? _handle : G2HNULL; }

public:
    int  connect(const G2GUID& site, const G2CONNECT_OPTIONS* options = NULL, G2CONNECT_RES* res = NULL);
    int  connect_ras(const G2NETWORK_INFO& ni, const G2CONNECT_OPTIONS* options = NULL, G2CONNECT_RES* res = NULL);
    void disconnect(int channel);
    bool is_connecting(int channel) const;
    bool is_connected(int channel) const;
    bool is_disconnecting(int channel) const;
    bool is_disconnected(int channel) const;
    bool is_disconnectable(int channel) const;

public:
    bool set_audio_in_enable(int channel, int number, bool enable);
    bool set_audio_out_enable(int channel, bool enable);
    void set_audio_in_disable(int channel);
    void set_audio_out_disable(int channel);
    void set_audio_in_disable_all(G2HDUALAUDIO handle);
    void set_audio_out_disable_all(G2HDUALAUDIO handle);

    void append_reserve(const G2GUID& site, G2DUALAUDIO_OPEN_MODE::MODE mode, const G2DUALAUDIO_RESERVE& param);
    void remove_reserve(const G2GUID& site);

public:
    bool get_reserved_parameter(const G2GUID& site, G2DUALAUDIO_OPEN_MODE::MODE mode, G2DUALAUDIO_RESERVE& param) const;
    G2GUID get_site_guid(int channel) const;
    int  get_channel_from_site_guid(const G2GUID& site) const;

public:
    bool is_drive_mode(int channel, G2DUALAUDIO_DRIVE::MODE mode) const;
    bool is_server_full_channel(const G2GUID& site) const;
    bool is_reserved(const G2GUID& site, G2DUALAUDIO_OPEN_MODE::MODE mode) const;
    bool is_contains_site_guid(const G2GUID& site) const;
    bool is_require_reconnect(int channel) const;
    bool is_last_use_audio_in(const G2GUID& site) const;
    bool is_last_use_audio_out(const G2GUID& site) const;
    bool is_activate_audio_in(const G2GUID& site) const;
    bool is_activate_audio_out(const G2GUID& site) const;
    bool is_activate_dualaudio(const G2GUID& site) const;
    bool is_opening_audio_out(int channel) const;

protected:
    static G2RESULT G2CALLBACK on_connected(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_disconnected(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_audio_in_opened(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_audio_in_closed(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_audio_out_opened(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_receive_audio_out_closed(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_require_select_channel(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
    static G2RESULT G2CALLBACK on_require_reconnect_imp(G2HLIVE handle, G2WPARAM wparam, G2LPARAM lparam, G2UPARAM uparam);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2_CLIENT_DLL_SAMPLER_DUALAUDIO_H_
