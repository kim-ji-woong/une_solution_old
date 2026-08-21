// connective_audio.cpp : implementation file
//

#include "stdafx.h"
#include "MainFrm.h"
#include "G2ClientDoc.h"
#include "G2ClientView.h"
#include "g2client_watch.h"

#include <sampler/cpp/g2client_dualaudio.h>
#include <include/g2_guid.h>


#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

void CG2ClientView::on_g2dualaudio_connection_impl(const int channel)
{
    G2RETURN_IF_FAIL(_audio_info_list.empty() == false);

    typedef std::list<audio_info_ptr>::iterator node_ptr;
    node_ptr node = _audio_info_list.begin();

    /*
    do {
        node_ptr temp = node++;
        int ch = _audio->get_channel_from_root_guid((*temp)->_guid);

        if (channel == ch) {
            switch ((*temp)->_mode) {
            case G2DUALAUDIO_OPEN_MODE::AUDIO_IN:
                _audio->set_audio_in_enable(channel, (*temp)->_camera, (*temp)->_activate);
                break;
            case G2DUALAUDIO_OPEN_MODE::AUDIO_OUT:
                _audio->set_audio_out_enable(channel, (*temp)->_activate);
                break;
            }
            _audio_info_list.erase(temp);
        }
    } while (node != _audio_info_list.end());
    */
}

void CG2ClientView::on_g2dualaudio_connected(G2HDUALAUDIO handle, int channel)
{
    G2RETURN_IF_FAIL(_audio->safe_handle() == handle);

    if (_audio->is_drive_mode(channel, G2DUALAUDIO_DRIVE::IAUD)) {

    }
    else {
        on_g2dualaudio_connection_impl(channel);
    }
}

void CG2ClientView::on_g2dualaudio_disconnected(G2HDUALAUDIO handle, int channel, G2DISCONNECT_REASON::TYPE reason)
{
    G2RETURN_IF_FAIL(_audio->safe_handle() == handle);

    _audio_info_list.clear();
}

void CG2ClientView::on_g2dualaudio_receive_audio_in_opened(G2HDUALAUDIO handle, int channel, const G2GUID& root)
{
    G2RETURN_IF_FAIL(_audio->safe_handle() == handle);
}

void CG2ClientView::on_g2dualaudio_receive_audio_in_closed(G2HDUALAUDIO handle, int channel, const G2GUID& root)
{
    G2RETURN_IF_FAIL(_audio->safe_handle() == handle);
}

void CG2ClientView::on_g2dualaudio_receive_audio_out_opened(G2HDUALAUDIO handle, int channel, const G2GUID& root)
{
    G2RETURN_IF_FAIL(_audio->safe_handle() == handle);
}

void CG2ClientView::on_g2dualaudio_receive_audio_out_closed(G2HDUALAUDIO handle, int channel, const G2GUID& root)
{
    G2RETURN_IF_FAIL(_audio->safe_handle() == handle);
}

void CG2ClientView::on_g2dualaudio_require_select_channel(G2HDUALAUDIO handle, int channel, const G2GUID& root, int count, int& number)
{
    G2RETURN_IF_FAIL(_audio->safe_handle() == handle);
}

bool CG2ClientView::on_g2dualaudio_require_reconnect_imp(G2HDUALAUDIO handle, const G2GUID& root)
{
    G2RETURN_VAL_IF_FAIL(_audio->safe_handle() == handle, false);

    bool reserved = false;

    return reserved;
}

LRESULT CG2ClientView::on_post_connected_dual_audio_in(int selcamera, bool activate)
{
    //const client::g2guid& root = _cameraMap.get_guid_camera(selcamera);
    const int mode = _cameraMap.get_connect_mode(selcamera);
    const int channel = client::find_channel_from_camera(selcamera);
    const int hostCamera = client::find_host_camera_from_camera(selcamera);
    const int hostId = client::find_host_id_from_camera(selcamera);

    if (is_audio_stream_in_live_port(selcamera)) {
        switch(mode) {
        case connect_mode_::WATCH:
            _watch->set_enable_audio_streaming(channel, hostCamera, activate);  
            return 0L;
        }    
    }


    //////////////////////////////////////////////////////////////////////////
    // use dual audio port..
    /*
    if (hostCamera != invalid_::CAMERA_NUMBER)
    {
        audio_info_ptr info(new audio_info(root, hostCamera, G2DUALAUDIO_OPEN_MODE::AUDIO_IN, activate));
        _audio_info_list.push_back(info);

        int audioChannel = _audio->get_channel_from_root_guid(root);

        if (audioChannel != invalid_::CHANNEL && _audio->is_connected(audioChannel)) {
            on_g2dualaudio_connection_impl(channel);
        }
        else {
            G2NETWORK_INFO ni;
            client::g2admin::get().get_device_network_info(root, ni);
            _audio->connect(root, ni);
        }
    }
    */

    return 0L;
}

LRESULT CG2ClientView::on_post_connected_dual_audio_out(int selcamera, bool activate)
{
    //const client::g2guid& root = _cameraMap.get_guid_camera(selcamera);
    const int mode = _cameraMap.get_connect_mode(selcamera);
    const int channel = client::find_channel_from_camera(selcamera);
    const int hostCamera = client::find_host_camera_from_camera(selcamera);

    if (is_audio_stream_in_live_port(selcamera)) {
        switch(mode) {
        case connect_mode_::WATCH:
            _watch->set_enable_audio_capturing(channel, 0, activate);
            return 0L;
        }    
    }

    //////////////////////////////////////////////////////////////////////////
    // use dual audio port..
    /*
    if (hostCamera != invalid_::CAMERA_NUMBER)
    {
        audio_info_ptr info(new audio_info(root, hostCamera, G2DUALAUDIO_OPEN_MODE::AUDIO_OUT, activate));
        _audio_info_list.push_back(info);

        int audioChannel = _audio->get_channel_from_root_guid(root);

        if (audioChannel != invalid_::CHANNEL && _audio->is_connected(audioChannel)) {
            on_g2dualaudio_connection_impl(channel);
        }
        else {
            G2NETWORK_INFO ni;
            client::g2admin::get().get_device_network_info(root, ni);
            _audio->connect(root, ni);
        }
    }
    */

    return 0L;
}

bool CG2ClientView::is_audio_stream_in_live_port(int selcamera)
{
    bool bUseDualAudioPort = false;

    //const client::g2guid& root = _cameraMap.get_guid_camera(selcamera);
    const int channel = client::find_channel_from_camera(selcamera);
    const int mode = _cameraMap.get_connect_mode(selcamera);

    switch (mode) {
    case connect_mode_::WATCH:
        bUseDualAudioPort = _watch->is_support(channel, G2LIVE_SUPPORT::AUDIO_STREAM_IN_WATCH_PORT);
        break;
    }

    return bUseDualAudioPort;
}

bool CG2ClientView::is_activate_dual_audio_speaker(int selcamera)
{
    bool activate = false;

    //const client::g2guid& root = _cameraMap.get_guid_camera(selcamera);
    const int mode = _cameraMap.get_connect_mode(selcamera);
    const int channel = client::find_channel_from_camera(selcamera);
    const int hostCamera = client::find_host_camera_from_camera(selcamera);

    if (mode == connect_mode_::UNDEFINED) {
        return activate;
    }

    if (is_audio_stream_in_live_port(selcamera)) {
        switch (mode) {
        case connect_mode_::WATCH:
            activate = _watch->is_contains_audio_streaming(channel, hostCamera);
            break;
        }
    }
    else {
        //activate = _audio->is_activate_audio_in(root);
    }

    return activate;
}

bool CG2ClientView::is_activate_dual_audio_microphone(int selcamera)
{
    bool activate = false;

    //const client::g2guid& root = _cameraMap.get_guid_camera(selcamera);
    const int mode = _cameraMap.get_connect_mode(selcamera);
    const int channel = client::find_channel_from_camera(selcamera);

    if (mode == connect_mode_::UNDEFINED) {
        return activate;
    }

    if (is_audio_stream_in_live_port(selcamera)) {
        switch (mode) {
        case connect_mode_::WATCH:
            activate = _watch->is_contains_audio_capturing(channel);
            break;
        }
    }
    else {
        //activate = _audio->is_activate_audio_out(root);
    }

    return activate;
}

bool CG2ClientView::is_activate_dual_audio(const G2GUID& root) 
{
    return _audio->is_activate_dualaudio(root);
}
