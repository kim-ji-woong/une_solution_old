// connective_connection.cpp : implementation file
//

#include "stdafx.h"
#include "G2SearchControllerView.h"
#include "G2SearchControllerDlg.h"
#include "actuator/screen_actuator.h"
#include "search/search_data_manager.h"

#include <include/g2_define_live.h>
#include <sampler/cpp/g2client_search_g2.h>
#include <sampler/cpp/app/g2app_frame_relay.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

bool G2SearchControllerView::connect_relay()
{
    G2CONNECT_RES res;
    if (_relay.get()) {
        _relay_channel = _relay->connect(_option._server_ni, NULL, &res);
    }

    return client::valid_channel(_relay_channel);
}

bool G2SearchControllerView::connect_search()
{
    G2CONNECT_RES res;
    bool port_unity = _option._no_port_unity ? false : true;

    if (_search_g2.get()) {
        _search_channel = _search_g2->connect_ras(_option._device_ni, port_unity, NULL, &res);
    }
    
    return client::valid_channel(_search_channel);
}

void G2SearchControllerView::disconnect_search(void)
{
    if (is_disconnectable()) {
        _search_g2->disconnect(_search_channel);
    }
}

bool G2SearchControllerView::is_connected(void)
{
    return _search_g2.get() ? _search_g2->is_connected(_search_channel) : false;
}

bool G2SearchControllerView::is_disconnected(void)
{
    return _search_g2.get() ? _search_g2->is_disconnected(_search_channel) : true;
}

bool G2SearchControllerView::is_disconnectable(void)
{
    return _search_g2.get() ? _search_g2->is_disconnectable(_search_channel) : false;
}

//////////////////////////////////////////////////////////////////////////

G2SPOT G2SearchControllerView::find_last_spot(void)
{
    G2SPOT spot = { 0 };
    unsigned __int64 cameras = 0x00ui64;

    for (unsigned int i = 0; i < _option._channels._len; ++i) {
        cameras |= 0x01ui64 << _option._channels._channels[i];
    }

    int camera = actuator_ref().get_last_displayed_spot(cameras, spot);

    return spot;
}

//////////////////////////////////////////////////////////////////////////

LRESULT G2SearchControllerView::on_post_connect_relay(WPARAM wParam, LPARAM lParam)
{
    connect_relay();

    return 0L;
}

LRESULT G2SearchControllerView::on_post_connected_relay(WPARAM wParam, LPARAM lParam)
{
    PostMessage(um_runner_::UM_CONNECT_SEARCH_G2);

    SetTimer(TIMER_ID_ALIVE_CHECK, 10000, NULL);

    return 0L;
}

LRESULT G2SearchControllerView::on_post_disconnected_relay(WPARAM wParam, LPARAM lParam)
{
    KillTimer(TIMER_ID_ALIVE_CHECK);

    _relay_channel = client::invalid_::CHANNEL;

    if (_controller.get()) {
        _controller->EndDialog(IDCANCEL);
    }

    return 0L;
}

//////////////////////////////////////////////////////////////////////////

LRESULT G2SearchControllerView::on_post_connect_search_g2(WPARAM wParam, LPARAM lParam)
{
    connect_search();

    return 0L;
}

LRESULT G2SearchControllerView::on_post_connected_search_g2(WPARAM wParam, LPARAM lParam)
{
    const int channel = static_cast<int>(wParam);

    G2RETURN_VAL_IF_FAIL(client::valid_channel(channel), 1L);

    if (_relay.get() && _relay->is_connected(_relay_channel))
        _relay->send_site_connected(_relay_channel, _option._site);

    if (_controller.get())
        _controller->PostMessage(um_controller_::UM_CONNECTED, wParam, lParam);

    return 0L;
}

LRESULT G2SearchControllerView::on_post_disconnected_search_g2(WPARAM wParam, LPARAM lParam)
{
    const int channel = static_cast<int>(wParam);
    const int reason = static_cast<int>(lParam);

    G2RETURN_VAL_IF_FAIL(client::valid_channel(channel), 1L);

    if (_relay.get() && _relay->is_connected(_relay_channel))
        _relay->send_site_disconnected(_relay_channel, _option._site, reason);
    
    if (_controller.get())
        _controller->PostMessage(um_controller_::UM_DISCONNECTED, wParam, lParam);
    
    _search_channel = client::invalid_::CHANNEL;

    SetTimer(TIMER_ID_DISCONNECT, _option._message_duration, NULL);

    return 0L;
}

LRESULT G2SearchControllerView::on_post_screen_no_image_loaded_search_g2(WPARAM wParam, LPARAM lParam)
{
    const short channel = static_cast<short>(wParam);
    const short camera = static_cast<short>(lParam);

    G2RETURN_VAL_IF_FAIL(channel == _search_channel, 1L);

    if (_endofplayHost.find(channel) == _endofplayHost.end()) {
        return 1L;
    }
    _endofplayHost.insert(channel);

    _search_g2->request_notify_end_of_play(channel);

    if (_controller.get())
        _controller->screen_end_of_play(channel);

    return 0L;
}

LRESULT G2SearchControllerView::on_post_rectime_info_load_end_search_g2(WPARAM wParm, LPARAM lParm)
{
    int channel     = static_cast<int>(wParm);
    int resolution  = static_cast<int>(LOWORD(lParm));
    int command     = static_cast<int>(HIWORD(lParm));

    G2RETURN_VAL_IF_FAIL(channel == _search_channel, 1L);

    if (resolution == G2RECORD_TIME_INFO::DAY) {
        search_data_ptr data = client::search_data_manager::get().find_search_data(search::SEARCH_LOCAL_G2, channel);
        if (g2_spot_is_valid(&data->spot_selected()) == false) {
            _search_g2->request_move_to_last(channel);
            _search_g2->set_play_control_command(channel, G2PLAYER::MOVE_TO_SPOT);
        }
    }
    else if (resolution == G2RECORD_TIME_INFO::MINUTE && _controller.get()) {
        if (client::search::request_::REQUEST_OVERWRITE_LEFT  == command ||
            client::search::request_::REQUEST_OVERWRITE_RIGHT == command) {
            _controller->update_site(channel);
        }
        else {
            _controller->load_site(channel, _option._channels._len > 0 ? _option._channels._channels[0] : 0);
            _controller->set_enable(true);
        }
    }

    return 0L;
}

LRESULT G2SearchControllerView::on_post_no_recorded_data_search_g2(WPARAM wParm, LPARAM lParm)
{
    int channel = static_cast<int>(wParm);

    G2RETURN_VAL_IF_FAIL(channel == _search_channel, 1L);

    int duration = _option._message_duration > 0 ? _option._message_duration : 0;
    show_screen_message(L"There is no recorded image.", true, duration);
    g2::looper_<client::runner>::sleep(1000);
    _search_g2->disconnect(channel);

    return 0L;
}

LRESULT G2SearchControllerView::on_post_receive_command_begin_search_g2(WPARAM wParm, LPARAM lParm)
{
    int channel = static_cast<int>(wParm);
    int command = static_cast<int>(lParm);

    G2RETURN_VAL_IF_FAIL(channel == _search_channel, 1L);

    if (command != G2PLAYER::NEXT_STEP) {
        if (command != G2PLAYER::PLAY_SLOW &&
            command != G2PLAYER::PLAY_NORMAL) {
            actuator_ref().clear_frame_buffer_play(channel);
        }
    }

    if (_controller.get())
        _controller->on_receive_notify_command_begin_play(channel, command);

    return 0L;
}
