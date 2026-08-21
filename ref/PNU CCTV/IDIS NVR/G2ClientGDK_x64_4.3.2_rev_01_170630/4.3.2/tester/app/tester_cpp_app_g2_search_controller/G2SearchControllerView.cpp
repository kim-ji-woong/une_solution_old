// G2SearchControllerView.cpp : implementation file
//

#include "stdafx.h"
#include "G2SearchController.h"
#include "G2SearchControllerView.h"
#include "G2SearchControllerDlg.h"
#include "actuator/screen_actuator.h"
#include "control/message_box.h"
#include "search/search_data_manager.h"

#include <include/g2_main.h>
#include <sampler/cpp/g2client_search_g2.h>
#include <sampler/cpp/app/g2app_frame_relay.h>
#include <boost/bind.hpp>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

client::runner* client::runner::__self_pointer__ = NULL;

//////////////////////////////////////////////////////////////////////////

client::runner* client::runner_ptr(void)
{
    return client::runner::__self_pointer__;
}

//////////////////////////////////////////////////////////////////////////

G2SearchControllerView::G2SearchControllerView(G2APP_FRAME_RELAY_SEARCH_CONTROLLER_OPTION option)
    : _search_g2(new g2search_g2)
    , _relay(new g2app_frame_relay)
    , _controller(new G2SearchControllerDlg)
    , _actuator(new screen_actuator)
    , _relay_channel(client::invalid_::CHANNEL)
    , _search_channel(client::invalid_::CHANNEL)
    , _option(option)
{
    __self_pointer__ = this;

    g2_main_app_initialize(G2LANGUAGE_ID::ENGLISH);
}

G2SearchControllerView::~G2SearchControllerView(void)
{
    g2_main_app_finalize();

    __self_pointer__ = NULL;
}

//////////////////////////////////////////////////////////////////////////

BEGIN_MESSAGE_MAP(G2SearchControllerView, CWnd)
    ON_WM_CREATE()
    ON_WM_DESTROY()
    ON_WM_TIMER()

    ON_MESSAGE(um_runner_::UM_CONNECT_RELAY,                    on_post_connect_relay)
    ON_MESSAGE(um_runner_::UM_CONNECTED_RELAY,                  on_post_connected_relay)
    ON_MESSAGE(um_runner_::UM_DISCONNECTED_RELAY,               on_post_disconnected_relay)
    ON_MESSAGE(um_runner_::UM_CONNECT_SEARCH_G2,                on_post_connect_search_g2)
    ON_MESSAGE(um_runner_::UM_CONNECTED_SEARCH_G2,              on_post_connected_search_g2)
    ON_MESSAGE(um_runner_::UM_DISCONNECTED_SEARCH_G2,           on_post_disconnected_search_g2)
    ON_MESSAGE(um_runner_::UM_SCREEN_NO_IMAGE_LOADED_SEARCH_G2, on_post_screen_no_image_loaded_search_g2)
    ON_MESSAGE(um_runner_::UM_RECTIME_LOADED_SEARCH_G2,         on_post_rectime_info_load_end_search_g2)
    ON_MESSAGE(um_runner_::UM_NO_RECORDED_DATA_SEARCH_G2,       on_post_no_recorded_data_search_g2)
    ON_MESSAGE(um_runner_::UM_RECEIVE_COMMAND_BEGIN_SEARCH_G2,  on_post_receive_command_begin_search_g2)
END_MESSAGE_MAP()

int G2SearchControllerView::OnCreate(LPCREATESTRUCT lpCreateStruct)
{
    if (CWnd::OnCreate(lpCreateStruct) == -1) {
        return -1;
    }

    initialize();

    return 0;
}

void G2SearchControllerView::OnDestroy()
{
    finalize();

    CWnd::OnDestroy();
}

void G2SearchControllerView::OnTimer(UINT_PTR nIDEvent)
{
    if (nIDEvent == TIMER_ID_ALIVE_CHECK) {
        KillTimer(nIDEvent);
        if (_relay.get() && _relay->is_disconnectable(_relay_channel)) {
            SetTimer(nIDEvent, 1000, NULL);
        }
        else if (_controller.get()) {
            _controller->EndDialog(IDCANCEL);
        }
    }
    else if (nIDEvent == TIMER_ID_DISCONNECT) {
        KillTimer(nIDEvent);
        if (_relay.get() && _relay->is_disconnectable(_relay_channel)) {
            _relay->disconnect(_relay_channel);
        }
        else if (_controller.get()) {
            _controller->EndDialog(IDCANCEL);
        }
    }

    CWnd::OnTimer(nIDEvent);
}

//////////////////////////////////////////////////////////////////////////

void G2SearchControllerView::initialize(void)
{
    ctor_network();

    _controller->set_top_most(_option._no_top_most ? false : true);
    if (_option._startup_pos_use) {
        CPoint start_pos(_option._startup_pos.x, _option._startup_pos.y);
        _controller->set_start_pos(start_pos);
    }

    _actuator->create(client::MAX_SCREEN_CAMERA_COUNT,
                      client::MAX_CONNECTIVE_CHANNEL);
    _actuator->set_listener(this);
    _actuator->run();
}

void G2SearchControllerView::finalize(void)
{
    dtor_network();
}

void G2SearchControllerView::ctor_network(void)
{
    _search_g2->startup(client::MAX_CONNECTIVE_CHANNEL);
    _search_g2->set_listener(this);

    _relay->startup(client::MAX_CONNECTIVE_CHANNEL);
    _relay->set_listener(this);

    search_data_manager::get().initialize(search::SEARCH_LOCAL_G2,  client::MAX_CONNECTIVE_CHANNEL);
    _endofplayHost.clear();
}

void G2SearchControllerView::dtor_network(void)
{
    disconnect_search();

    _search_g2->cleanup();
    _relay->cleanup();

    search_data_manager::get().cleanup();
    _endofplayHost.clear();
}

void G2SearchControllerView::cleanup(void)
{
    _endofplayHost.clear();
}

//////////////////////////////////////////////////////////////////////////

int G2SearchControllerView::set_play_speed(short channel, int command, int clientSpeed /*= client::search::speed_::PLAY_SPEED_UNDEFINED*/)
{
    int speed = _I32_MIN;

    switch (clientSpeed) {
        case search::speed_::BACK_FASTEST_MORE:         speed = -120; break;
        case search::speed_::BACK_FASTEST:              speed = -100; break;
        case search::speed_::BACK_FASTER_MORE:          speed =  -80; break;
        case search::speed_::BACK_FASTER:               speed =  -60; break;
        case search::speed_::BACK_FAST_MORE:            speed =  -50; break;
        case search::speed_::BACK_FAST:                 speed =  -40; break;
        case search::speed_::BACK_NORMAL_TRIPLE:        speed =  -30; break;
        case search::speed_::BACK_NORMAL_TWICE_HALF:    speed =  -25; break;
        case search::speed_::BACK_NORMAL_TWICE:         speed =  -20; break;
        case search::speed_::BACK_NORMAL_HFAST:         speed =  -15; break;
        case search::speed_::BACK_NORMAL:               speed =  -10; break;
        case search::speed_::BACK_SLOW:                 speed =   -5; break;
        case search::speed_::PLAY_STOP:                 speed =    0; break;
        case search::speed_::PLAY_SLOW:                 speed =   +5; break;
        case search::speed_::PLAY_NORMAL:               speed =  +10; break;
        case search::speed_::PLAY_NORMAL_HFAST:         speed =  +15; break;
        case search::speed_::PLAY_NORMAL_TWICE:         speed =  +20; break;
        case search::speed_::PLAY_NORMAL_TWICE_HALF:    speed =  +25; break;
        case search::speed_::PLAY_NORMAL_TRIPLE:        speed =  +30; break;
        case search::speed_::PLAY_FAST:                 speed =  +40; break;
        case search::speed_::PLAY_FAST_MORE:            speed =  +50; break;
        case search::speed_::PLAY_FASTER:               speed =  +60; break;
        case search::speed_::PLAY_FASTER_MORE:          speed =  +80; break;
        case search::speed_::PLAY_FASTEST:              speed = +100; break;
        case search::speed_::PLAY_FASTEST_MORE:         speed = +120; break;

        case search::speed_::PLAY_FASTEST_INFINITE:
        case search::speed_::BACK_FASTEST_INFINITE:     speed = _I32_MAX; break;
        default:
            //assert(!"not defined speed for client play");
            break;
    }

    if (speed != _I32_MIN) {
        bool ignoreSound = false;
        actuator_ref().set_search_play_speed(channel, speed, ignoreSound);
    }
    return speed;
}

void G2SearchControllerView::show_screen_message(const std::wstring& string, bool autohide /*= true*/, int elapse /*= 5000*/)
{
    if (_controller.get())
        _controller->show_screen_message(string, autohide, elapse);
}

void G2SearchControllerView::show_screen_message(const std::wstring& string, const std::wstring& message, bool autohide /*= true*/, int elapse /*= 5000*/)
{
    if (_controller.get())
        _controller->show_screen_message(string, message, autohide, elapse);
}

void G2SearchControllerView::hide_screen_message(void)
{
    if (_controller.get())
        _controller->hide_screen_message();
}

//////////////////////////////////////////////////////////////////////////

void G2SearchControllerView::on_screen_image_loaded(const G2FRAME& frame)
{
    if (_relay.get()) {
        unsigned int size = frame._index._data_size + (16 * 1024);

        for ( ; _relay->is_connected(_relay_channel); ) {
            if (_relay->is_able_to_send(_relay_channel, size)) {
                if (_relay->send_site_frame_data(_relay_channel, _option._site, frame)) {
                    break;
                }
            }
            Sleep(10);
        }
    }

    if (_controller.get())
        _controller->on_screen_image_drew(frame._index._spot, frame._index._channel);
}

void G2SearchControllerView::on_screen_no_image_loaded(short channel, short camera)
{
    PostMessage(um_runner_::UM_SCREEN_NO_IMAGE_LOADED_SEARCH_G2, WPARAM(channel), LPARAM(camera));
}
