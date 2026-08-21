// connective_watch.cpp : implementation file
//

#include "stdafx.h"
#include "MainFrm.h"
#include "G2ClientDoc.h"
#include "G2ClientView.h"

#include <sampler/cpp/g2client_watch.h>
#include <panel/controller/controller_watch.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

void CG2ClientView::on_g2watch_connected(G2HWATCH handle, int channel)
{
    G2RETURN_IF_FAIL(_watch->safe_handle() == handle);

    connective_host_info_ptr hi = _hostList.get_host_info(channel, client::connect_mode_::WATCH);
    G2RETURN_IF_FAIL(hi != NULL);

    const short hostId = hi->host_id();
    if (client::valid_host_id(hostId)) {

    }
    else {
        assert(!"occur broken artifact channel");
        _watch->disconnect(channel);
        return;
    }

    reset_screen_view(hostId, true, true);
    set_disconnect_by_me(hostId, false);

    screen_actuator& actuator = actuator_ref();
    actuator.set_activate_screen(true);
    actuator.setup_buffer(hostId, frame_buffer::TYPE_WATCH);
}

void CG2ClientView::on_g2watch_disconnected(G2HWATCH handle, int channel, G2DISCONNECT_REASON::TYPE reason)
{
    G2RETURN_IF_FAIL(_watch->safe_handle() == handle);

    connective_host_info_ptr hi = _hostList.get_host_info(channel, client::connect_mode_::WATCH);
    G2RETURN_IF_FAIL(hi != NULL && client::valid_host_id(hi->host_id()));
    const short hostId = hi->host_id();

    //////////////////////////////////////////////////////////////////////////

    if (reason != G2DISCONNECT_REASON::LOGOUT) {

        G2STRING_128 sreason = { 0 };
        g2_get_string_disconnect_reason(reason, &sreason);

        std::wstring message = _hostList.site_name_from_host_id(hostId);
        message += L" : ";
        message += sreason._string;
        show_screen_message(L"Device disconnected.", message);
    }

    //////////////////////////////////////////////////////////////////////////
    
    unsigned __int64 cameras = _cameraMap.get_cameras_by_host_id(hostId);
    screen_actuator& actuator = actuator_ref();

    bool reconnect = (is_disconnect_by_me(hostId) != true) ?
                      reconnect_device(hostId, reason) : false;

    if (reconnect) {
        actuator.set_camera_status(cameras, screen::camera_status_::RECONNECTING);
        return;
    }
        
    //////////////////////////////////////////////////////////////////////////
    
    release_host_id(hostId, true, false);

    actuator.fire_disconnected();
}

void CG2ClientView::on_g2watch_receive_frame_data(G2HWATCH handle, int channel, const G2FRAME& frame)
{
    G2RETURN_IF_FAIL(_watch->safe_handle() == handle);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::WATCH);
    const short hostcamera = frame._index._channel;

    if (client::valid_host_id(hostId) != true ||
        client::valid_camera(hostcamera) != true) return;

    unsigned __int64 refcameras = 0x00ui64;
    short basecamera = _cameraMap.reference_camera(hostId, hostcamera, refcameras);

    if (client::valid_camera(basecamera) != true ||
        refcameras == 0x00ui64) {
        return;
    }

    screen_actuator& actuator = actuator_ref();

    for (short i = 0; i < actuator.count_camera(); ++i) {
        if (g2::contains_bit(refcameras, i) &&
            actuator.is_camera_visible(i)) {
            if (actuator.is_camera_no_video(i)) {
                actuator.set_camera_status(i, screen::camera_status_::ENABLE);
            }
        }
    }
    actuator.put_frame(G2FRAME::FROM_WATCH, frame, hostId, basecamera, refcameras);
}

void CG2ClientView::on_g2watch_receive_event(G2HWATCH handle, int channel, const G2EVENT_INFO& info)
{
    G2RETURN_IF_FAIL(_watch->safe_handle() == handle);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::WATCH);
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    G2DEVICE_STATUS status;
    _watch->get_status(channel, status);

    foundation_ptr()->append_instant_event(info, status);
}

void CG2ClientView::on_g2watch_receive_device_status(G2HWATCH handle, int channel, const G2DEVICE_STATUS& status)
{
    G2RETURN_IF_FAIL(_watch->safe_handle() == handle);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::WATCH);
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));
    
    unsigned __int64 cameras = _cameraMap.get_cameras_by_host_id(hostId);
    int maxcamera = sizeof(status._camera) / sizeof(signed char);

    //////////////////////////////////////////////////////////////////////////
    // add site info

    std::wstring site = _hostList.site_name_from_host_id(hostId);
    if (foundation_ptr()->site_has_children(site.c_str())) {
        foundation_ptr()->remvoe_site_children(site.c_str());
    }

    for (int i = 0; i < maxcamera; ++i) {
        if (status._camera_desc[i]._len) {
            foundation_ptr()->add_site_child(site.c_str(), status._camera_desc[i]._string, i, true);
        }
    }

    //////////////////////////////////////////////////////////////////////////
    // set camera map

    unsigned __int64 visible = actuator_ref().visible_camera();
    if (_cameraMap.count_cameras(hostId) == 1) {
        short scrcamera = g2::find_first_biton(cameras);
        if (_cameraMap.get_host_camera(scrcamera) == client::invalid_::CAMERA_NUMBER) {
            for (int i = 0; i < maxcamera; ++i) {
                if (status._camera[i] != G2DEVICE_STATUS::NOT_VALUE &&
                    g2::contains_bit(visible, scrcamera)) {
                    camera_map_element element(hostId, i,
                                               client::connect_mode_::WATCH,
                                               client::invalid_::CHANNEL_EXT);
                    _cameraMap.set_camera_element(scrcamera++, element);
                }
            }
            cameras = _cameraMap.get_cameras_by_host_id(hostId);
        }
    }
    cameras = _cameraMap.get_cameras_by_host_id(hostId);
    actuator_ref().set_camera_mode(cameras, client::screen::camera_::WATCH);
    
    //////////////////////////////////////////////////////////////////////////
    // update camera status

    short hostcamera = invalid_::CAMERA_NUMBER; 
    screen::camera_status_::TYPE currstatus = screen::camera_status_::UNDEFINED;
    screen_actuator& actuator = actuator_ref();

    for (short i = 0, count = _cameraMap.size(); i < count; ++i) {
        if (g2::contains_bit(cameras, i) != true) continue;
        hostcamera = _cameraMap.get_host_camera(i);
        if (hostcamera < 0 ||
            hostcamera >= maxcamera) {
            continue;
        }

        switch (status._camera[hostcamera]) {
            case G2DEVICE_STATUS::VIDEOLOSS:
                currstatus = screen::camera_status_::NO_VIDEO;
                break;
            case G2DEVICE_STATUS::INACTIVE:
                currstatus = screen::camera_status_::INACTIVATE;
                break;
            case G2DEVICE_STATUS::ACTIVE:
                currstatus = screen::camera_status_::ENABLE;
                break;
            case G2DEVICE_STATUS::NOTCONNECTED:
                currstatus = screen::camera_status_::NOT_CONNECTED;
                break;
            case G2DEVICE_STATUS::MULTISTREAM:
                currstatus = screen::camera_status_::ENABLE;
                break;
            default:
                currstatus = screen::camera_status_::UNDEFINED;
                break;
        }
        actuator.set_camera_status(i, currstatus, false);
        actuator.set_use_camera_ptz(i, status._ptz[hostcamera]);
    }
    actuator.update_camera(cameras);

    //////////////////////////////////////////////////////////////////////////
    
    set_camera_list(hostId);
}

void CG2ClientView::on_g2watch_receive_ptz_preset(G2HWATCH handle, int channel, int camera, const G2LIVE_PTZ_PRESET& preset)
{
    G2RETURN_IF_FAIL(_watch->safe_handle() == handle);

    const int hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::WATCH);
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    if (client::controller_watch* controller = controller_live_ptr()) {
        controller->receive_ptz_preset(channel, preset);
    }
}

void CG2ClientView::on_g2watch_receive_ptz_menu(G2HWATCH handle, int channel, int camera, const G2LIVE_PTZ_MENU& menu)
{
    G2RETURN_IF_FAIL(_watch->safe_handle() == handle);

    const int hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::WATCH);
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    if (client::controller_watch* controller = controller_live_ptr()) {
        controller->receive_ptz_menu(channel, menu);
    }
}

void CG2ClientView::on_g2watch_receive_camera_title_idr(G2HWATCH handle, int channel, int camera, const std::wstring& title)
{

}

void CG2ClientView::on_g2watch_receive_text_in(G2HWATCH handle, int channel, const G2TEXT_IN& data)
{
    G2RETURN_IF_FAIL(_watch->safe_handle() == handle);
    G2RETURN_IF_FAIL(client::valid_channel(channel));

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::WATCH);
    G2RETURN_IF_FAIL(valid_host_id(hostId));

    screen_actuator& actuator = actuator_ref();
    G2RETURN_IF_FAIL(actuator.is_layout_1x1());

    int camera = actuator.selected_camera();
    int hostcamera = _cameraMap.get_host_camera(camera);
    if (_cameraMap.get_host_id(camera) != hostId) {
        return;
    }

    bool processed = false;
    const G2CHANNEL_SET& channelset = data._index._record_channels;
    for (int i = 0, count = channelset._len; i < count; ++i) {
        if (channelset._channels[i] == hostcamera) {
            processed = true;
            break;
        }
    }

    if (processed) {
        G2TEXT_IN_ELEMENT element = { 0 };
        element._from = G2TEXT_IN_ELEMENT::FROM_LIVE;
        element._camera = camera;
        element._spot = data._index._spot;

        size_t len = 0;
        std::string text((const char*)data._data, data._index._data_size);
        mbstowcs_s(&len, element._data._string, 256, text.c_str(), _TRUNCATE);
        element._data._len = len;       

        switch (data._index._transaction) {
            case G2TEXT_IN::TRANSACTION_BEGIN:
                element._type = G2TEXT_IN_ELEMENT::TYPE_START;
                break;
            case G2TEXT_IN::TRANSACTION_END:
                element._type = G2TEXT_IN_ELEMENT::TYPE_END;
                break;
            default:
                element._type = G2TEXT_IN_ELEMENT::TYPE_CONTENT;
                break;
        }

        actuator.put_text_in(hostId, camera, element);
    }
}

void CG2ClientView::on_g2watch_receive_audio_out_not_available(G2HWATCH handle, int channel)
{
    G2RETURN_IF_FAIL(_watch->safe_handle() == handle);

    connective_host_info_ptr hi = _hostList.get_host_info(channel, client::connect_mode_::WATCH);
    G2RETURN_IF_FAIL(hi != NULL && client::valid_host_id(hi->host_id()));

    /////////////////////////////////////////////

    std::wstring site = _hostList.site_name_from_host_id(hi->host_id());
    G2STRING_128 reason = { 0 };
    g2_get_string_disconnect_reason(G2DISCONNECT_REASON::FULL_CHANNEL, &reason);

    std::wstring string = L"Two-way Audio";
    string += L" (";
    string += site;
    string += L") : ";
    string += reason._string;

    show_screen_message(string, true, 3000);
}

void CG2ClientView::on_g2watch_audio_streaming_started(G2HWATCH handle, int channel, int camera)
{
    G2RETURN_IF_FAIL(_watch->safe_handle() == handle);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::WATCH);
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    unsigned __int64 cameras = _cameraMap.get_cameras_by_host_id(hostId);

    screen_actuator& actuator = actuator_ref();
    actuator.set_camera_use_dual_audio(cameras, true);
}

void CG2ClientView::on_g2watch_audio_streaming_stopped(G2HWATCH handle, int channel, int camera)
{
    G2RETURN_IF_FAIL(_watch->safe_handle() == handle);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::WATCH);
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    unsigned __int64 cameras = _cameraMap.get_cameras_by_host_id(hostId);

    screen_actuator& actuator = actuator_ref();
    actuator.set_camera_use_dual_audio(cameras, false);
}

void CG2ClientView::on_g2watch_audio_capturing_started(G2HWATCH handle, int channel, int camera)
{
    G2RETURN_IF_FAIL(_watch->safe_handle() == handle);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::WATCH);
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    unsigned __int64 cameras = _cameraMap.get_cameras_by_host_id(hostId);

    screen_actuator& actuator = actuator_ref();
    actuator.set_camera_use_sound_capturing(cameras, true);
}

void CG2ClientView::on_g2watch_audio_capturing_stopped(G2HWATCH handle, int channel, int camera)
{
    G2RETURN_IF_FAIL(_watch->safe_handle() == handle);

    const short hostId = _hostList.find_host_id_from_channel(channel, client::connect_mode_::WATCH);
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    unsigned __int64 cameras = _cameraMap.get_cameras_by_host_id(hostId);

    screen_actuator& actuator = actuator_ref();
    actuator.set_camera_use_sound_capturing(cameras, false);
}
