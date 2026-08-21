// ConnectiveCameraList.cpp : implementation file
//

#include "stdafx.h"
#include "G2ClientDoc.h"
#include "G2ClientView.h"

#include <sampler/cpp/g2client_watch.h>
#include <sampler/cpp/g2client_search.h>
#include <sampler/cpp/g2client_search_g2.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

#pragma warning(disable : 4244)

//////////////////////////////////////////////////////////////////////////

void CG2ClientView::set_camera_list(short hostId, bool clearLiveBuff /*= false*/, bool force /*= false*/)
{
    G2RETURN_IF_FAIL(client::valid_host_id(hostId));

    connective_host_info info;

    int channel, netmode;
    {
        if (const client::connective_host_info_ptr info = _hostList.get_host_info(hostId)) {
            channel = info->channel();
            netmode = info->mode();
        }
        else {
            TRACE(">> set-camera list is invalid host id\n");
            return;
        }
    }

    client::connect_type_::TYPE connective = client::connect_type_from_connect_mode(netmode);

    g2::scoped_criticalsection lock(_lock_cameralist);

    g2::channels cameraSet;
    std::set<int> channelSetInterested;
    std::set<int> channelSetLive;
    std::set<int> channelSetPlay;
    std::set<int> living;

    // check connection of channel ////////////////////////

    bool connected = is_connected(connect_mode_::MODE(netmode), channel);
    if (!connected) return;

    ///////////////////////////////////////////////////////

    screen_actuator& actuator = actuator_ref();

    unsigned __int64 scrcamras = 0x00ui64;

    int i, countcamera = 0;
    bool activate = false;

    screen_view& screen = actuator.screen_ref();
    countcamera = actuator.count_camera();
    scrcamras   = screen.visible_camera();
    activate    = screen.is_activate();

    if (connective == client::connect_type_::RAS) {
        short hostcamera = client::invalid_::CAMERA_NUMBER;
        int channelext = client::invalid_::CHANNEL_EXT;

        if (activate && netmode == client::connect_mode_::SEARCH_G2) {
            for (i = 0; i < countcamera; ++i) {
                if (_cameraMap.get_host_id(i) == hostId &&
                    client::valid_host_camera(channelext = _cameraMap.get_host_camera(i))) {
                    channelSetInterested.insert(channelext);
                    if (g2::contains_bit(scrcamras, i)) {
                        channelSetPlay.insert(channelext);
                    }
                }
            }
        }
        else if (activate && netmode == client::connect_mode_::SEARCH) {
            for (i = 0; i < countcamera; ++i) {
                if (g2::contains_bit(scrcamras, i) &&
                    _cameraMap.get_host_id(i) == hostId &&
                    client::valid_host_camera(hostcamera = _cameraMap.get_host_camera(i))) {
                    cameraSet.add(hostcamera);
                    living.insert(hostcamera);
                }
            }
        }
        else /*if (activate && netmode == client::connect_mode_::WATCH)*/ {
            G2DEVICE_STATUS status = { 0 };
            _watch->get_status(channel, status);

            for (i = 0; i < countcamera; ++i) {
                if (g2::contains_bit(scrcamras, i) &&
                    _cameraMap.get_host_id(i) == hostId &&
                    client::valid_host_camera(hostcamera = _cameraMap.get_host_camera(i))) {
                    cameraSet.add(hostcamera);
                    living.insert(hostcamera);

                    CString title;
                    title.Format(L"%d. %s", i + 1, status._camera_desc[hostcamera]._string);
                    actuator->set_camera_title(i, title.GetBuffer());
                }
            }
        }
    }

    // recheck connection of channel //////////////////////

    if (connected) {
        switch (netmode) {
            case client::connect_mode_::WATCH:
                set_camera_list_impl_watch(hostId, channel, cameraSet, force);
                break;
            case client::connect_mode_::SEARCH:
                set_camera_list_impl_search(hostId, channel, cameraSet, force);
                break;
            case client::connect_mode_::SEARCH_G2:
                set_camera_list_impl_search_g2(hostId, channel, channelSetInterested, channelSetPlay);
                break;
            default:
                assert(!"undefined connect mode");
                break;
        }
    }

    if (clearLiveBuff) {
        switch (netmode) {
            case client::connect_mode_::WATCH:
                actuator.clear_frame_buffer(hostId, living);
                break;
        }
    }
}

//////////////////////////////////////////////////////////////////////////

void CG2ClientView::set_camera_list_impl_watch(short hostId, short channel, const g2::channels& cameras, bool force /*= false*/)
{
    _watch->set_camera_list(channel, cameras.to_channelset(), force);
}

void CG2ClientView::set_camera_list_impl_search(short hostId, short channel, const g2::channels& cameras, bool force /*= false*/)
{
    unsigned int curcameras = 0;
    if (force != true &&
        cameras.to_uint64() == g2::channels(_search->get_camera_list(channel, curcameras)).to_uint64()) {
        TRACE(L">> search : same camera list...\n");
        return;
    }

    _search->set_camera_list(channel, cameras.to_uint64());

    if (_search->is_drive_mode(channel, G2SEARCH_DRIVE::SAMBA)) {
        screen_actuator& actuator = actuator_ref();
        client::camera_map* cameramap = get_camera_map();

        G2SPOT spot = client::find_last_spot_from_host_id(hostId);
        short hostcamera = cameramap->get_host_camera(client::find_last_camera_from_host_id(hostId));

        actuator.clear_frame_buffer_play(hostId);
        _search->request_reload_current(channel, hostcamera, spot);
    }
}

void CG2ClientView::set_camera_list_impl_search_g2(short hostId, short channel, const std::set<int>& channelSetInterest, const std::set<int>& channelSetPlay)
{
    if (channelSetInterest.empty()) {
        return;
    }

    bool prepareRollback = true;
    bool preparing = false;

    std::set<int> channels;
    if (_search_g2->get_camera_list_interest(channel, channels) == true &&
        channels != channelSetInterest) {
            _search_g2->set_camera_list_interest(channel, channelSetInterest);
    }

    std::set<int>().swap(channels);
    if (_search_g2->get_camera_list(channel, channels) == true &&
        channels == channelSetPlay) {
            TRACE(">> Search_G2 : same Search_G2 channel list...\n");
            return;
    }

    screen_actuator& actuator = actuator_ref();

    unsigned __int64 cameras = client::find_cameras_from_host_id(hostId);

    G2SPOT spotBack = { 0 };
    const int scrcamera = actuator.get_last_displayed_spot(cameras, spotBack);

    if (prepareRollback) {
        actuator.set_prepare_drive(hostId, true);
    }

    actuator.clear_frame_buffer_play(hostId);

    G2ROLLBACK_INFO rollbackInfo;
    rollbackInfo._channelext = client::find_channel_ext_from_camera(scrcamera);
    rollbackInfo._spot = spotBack;

    _search_g2->set_camera_list(channel, channelSetPlay, rollbackInfo, prepareRollback, &preparing);

    /////////////////////////////////////////////

    if (preparing != true) {
        actuator.set_prepare_drive(hostId, false);
    }
}
