// ConnectiveCameraList.cpp : implementation file
//

#include "stdafx.h"
#include "G2SearchControllerView.h"
#include "actuator/screen_actuator.h"

#include <sampler/cpp/g2client_search_g2.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

#pragma warning(disable : 4244)

//////////////////////////////////////////////////////////////////////////

void G2SearchControllerView::set_camera_list()
{
    g2::scoped_criticalsection lock(_lock_cameralist);

    std::set<int> channelSet;
    if (!is_connected()) return;

    for (unsigned int i = 0; i < _option._channels._len; ++i) {
        channelSet.insert(_option._channels._channels[i]);
    }
    
    set_camera_list_impl_search_g2(_search_channel, channelSet, channelSet);
}

void G2SearchControllerView::set_camera_list_impl_search_g2(short channel, const std::set<int>& channelSetInterest, const std::set<int>& channelSetPlay)
{
    if (channelSetInterest.empty()) {
        return;
    }

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

    actuator_ref().set_prepare_drive(channel, true);
    actuator_ref().clear_frame_buffer_play(channel);

    bool preparing = false;
    _search_g2->set_camera_list(channel, channelSetPlay, G2ROLLBACK_INFO(), true, &preparing);

    /////////////////////////////////////////////

    if (preparing != true) {
        actuator_ref().set_prepare_drive(channel, false);
    }
}
