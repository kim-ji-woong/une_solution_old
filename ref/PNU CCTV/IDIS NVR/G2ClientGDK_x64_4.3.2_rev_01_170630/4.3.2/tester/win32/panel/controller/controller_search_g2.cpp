// controller_search_g2.cpp : implementation
//

#include "stdafx.h"
#include "G2Client.h"
#include "MainFrm.h"
#include "G2ClientView.h"
#include "controller_search_g2.h"
#include "control/play/time_goto_dlg.h"
#include "control/play/select_segment_dlg.h"
#include "control/play/calendar_dlg.h"
#include "control/play/time_table_minute_g2.h"
#include "control/play/search_g2_event_list.h"
#include "control/play/clip_copy_dlg.h"
#include "control/play/search_g2_condition_dlg.h"
#include "search/search_data_manager.h"
#include "search/search_minute_info.h"

#include <sampler/cpp/g2client_search_g2.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

controller_search_g2::controller_search_g2(void)
    : _adaptor(NULL)
    , _ci()
    , _time_table(new time_table_minute_g2)
    , _event_list(new search_g2_event_list)
    , _move_step(move_::FRAME_1)
    , _mode(mode_::TIMELAPSE)
    , _enable(false)
{

}

controller_search_g2::~controller_search_g2(void)
{

}

//////////////////////////////////////////////////////////////////////////

void controller_search_g2::DoDataExchange(CDataExchange* pDX)
{
    CDialog::DoDataExchange(pDX);

    DDX_Control(pDX, IDC_CBX_SEARCH_G2_SCREEN_FORMAT, _cbx_format);
    DDX_Control(pDX, IDC_SLD_SEARCH_G2_SPEED, _sld_speed);
    DDX_Control(pDX, IDC_STC_SEARCH_G2_SPEED, _stc_speed);
    DDX_Control(pDX, IDC_BTN_SEARCH_G2_MODE, _btn_mode);
    DDX_Control(pDX, IDC_BTN_SEARCH_G2_PLAY, _btn_play);
    DDX_Control(pDX, IDC_BTN_SEARCH_G2_STOP, _btn_stop);
    DDX_Control(pDX, IDC_BTN_SEARCH_G2_CLIPCOPY, _btn_clipcopy);
}

//////////////////////////////////////////////////////////////////////////

BEGIN_MESSAGE_MAP(controller_search_g2, CDialog)
    ON_WM_CREATE()
    ON_WM_DESTROY()
    ON_WM_SIZE()
    ON_WM_ERASEBKGND()
    ON_WM_HSCROLL()
    ON_WM_TIMER()
    ON_CBN_SELCHANGE(IDC_CBX_SEARCH_G2_SCREEN_FORMAT,                       on_cbx_selchange_format)
    ON_BN_CLICKED(IDC_BTN_SEARCH_G2_MODE,                                   on_btn_mode)
    ON_BN_CLICKED(IDC_BTN_SEARCH_G2_FILTER,                                 on_btn_filter)
    ON_BN_CLICKED(IDC_BTN_SEARCH_G2_LAYOUT_PREV,                            on_btn_layout_prev)
    ON_BN_CLICKED(IDC_BTN_SEARCH_G2_LAYOUT_NEXT,                            on_btn_layout_next)
    ON_BN_CLICKED(IDC_BTN_SEARCH_G2_GO_TO,                                  on_btn_go_to)
    ON_BN_CLICKED(IDC_BTN_SEARCH_G2_BACKWARD,                               on_btn_backward)
    ON_BN_CLICKED(IDC_BTN_SEARCH_G2_FORWARD,                                on_btn_forward)
    ON_BN_CLICKED(IDC_BTN_SEARCH_G2_MOVE_STEP,                              on_btn_move_step)
    ON_BN_CLICKED(IDC_BTN_SEARCH_G2_PLAY,                                   on_btn_play)
    ON_BN_CLICKED(IDC_BTN_SEARCH_G2_STOP,                                   on_btn_stop)
    ON_BN_CLICKED(IDC_BTN_SEARCH_G2_CALENDAR,                               on_btn_calendar)
    ON_BN_CLICKED(IDC_BTN_SEARCH_G2_CLIPCOPY,                               on_btn_clipcopy)
    ON_COMMAND(ID_SEARCH_G2_GOTO_TIME,                                      on_goto_time)
    ON_COMMAND(ID_SEARCH_G2_GOTO_FIRST,                                     on_goto_first)
    ON_COMMAND(ID_SEARCH_G2_GOTO_LAST,                                      on_goto_last)
    ON_COMMAND_RANGE(ID_SEARCH_G2_MOVE_1FRAME, ID_SEARCH_G2_MOVE_60MINUTE,  on_move_step)
    ON_NOTIFY(NM_RELEASEDCAPTURE, IDC_SLD_SEARCH_G2_SPEED,                  on_sld_speed_release_capture)
    ON_MESSAGE(um_controller_::UM_SCREEN_CAMERA_CHANGED,                    on_post_screen_camera_changed)
    ON_MESSAGE(um_controller_::UM_RECEIVE_SCOPE_LIST,                       on_post_receive_scope_list)
    ON_MESSAGE(um_controller_::UM_RECEIVE_SPOT_LIST,                        on_post_receive_spot_list)
    ON_MESSAGE(um_controller_::UM_ENABLE_CONTROLS,                          on_post_enable_controls)
    ON_MESSAGE(um_controller_::UM_DISCONNECTED,                             on_post_disconnected)
    ON_MESSAGE(um_controller_::UM_LOAD_TIME_TABLE,                          on_post_load_time_table)
    ON_MESSAGE(um_controller_::UM_UPDATE_TIME_TABLE,                        on_post_update_time_table)
END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////

BOOL controller_search_g2::PreTranslateMessage(MSG* pMsg)
{
    bool proceed = true;
    if (pMsg->message == WM_KEYDOWN) {
        switch(pMsg->wParam) {
            case VK_RETURN:
            case VK_ESCAPE:
                proceed = false;
                break;
        }
    }
    return (proceed) ? CDialog::PreTranslateMessage(pMsg) : TRUE;
}

//////////////////////////////////////////////////////////////////////////

int controller_search_g2::OnCreate(LPCREATESTRUCT lpcs)
{
    if (CDialog::OnCreate(lpcs) == -1) {
        return -1;
    }

    client::runner* runner = runner_ptr();
    if (runner) {
        _adaptor = &(runner->search_g2_ref());
    }
    assert(_adaptor);

    return 0;
}

BOOL controller_search_g2::OnInitDialog()
{
    CDialog::OnInitDialog();

    initialize();

    return TRUE;
}

void controller_search_g2::OnDestroy()
{
    CDialog::OnDestroy();
}

void controller_search_g2::OnSize(unsigned int nType, int cx, int cy)
{
    CDialog::OnSize(nType, cx, cy);

    CRect rect;
    GetClientRect(&rect);
    rect.top = const_::PLAYBACK_HEIGHT;
    rect.DeflateRect(10, 10);

    if (_time_table->is_initialized()) {
        _time_table->SetWindowPos(NULL, 0, 0, rect.Width(), rect.Height(), SWP_NOMOVE | SWP_NOZORDER);
    }

    if (_event_list->is_initialized()) {
        _event_list->SetWindowPos(NULL, 0, 0, rect.Width(), rect.Height(), SWP_NOMOVE | SWP_NOZORDER);
    }
}

BOOL controller_search_g2::OnEraseBkgnd(CDC* pDC)
{
    return CDialog::OnEraseBkgnd(pDC);
}

void controller_search_g2::OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar)
{
    CDialog::OnHScroll(nSBCode, nPos, pScrollBar);

    if (pScrollBar) {
        if (pScrollBar == (CScrollBar*)GetDlgItem(IDC_SLD_SEARCH_G2_SPEED)) {
            request_speed_shuttle();
        }
    }
}

void controller_search_g2::OnTimer(UINT_PTR nIDEvent)
{
    CDialog::OnTimer(nIDEvent);

    if (nIDEvent == timer_::id_::BALANCE_AUTO) {
        if (client::valid_channel(_ci._channel)) {
            balance_time_table();
        }
    }
}

//////////////////////////////////////////////////////////////////////////

void controller_search_g2::initialize(void)
{
    CString string;
    for (int i = 1; i < 7; ++i) {
        string.Format(_T("Layout %dx%d"), i, i);
        _cbx_format.AddString(string);
    }

    _cbx_format.SetCurSel(screen_formatter_::LAYOUT_4x4);
    _sld_speed.SetRange(-13, 13, TRUE);
    _sld_speed.SetPos(0);
    _sld_speed.SetTicFreq(1);
    _sld_speed.SetPageSize(1);

    CRect rect;
    GetClientRect(&rect);
    rect.top = const_::PLAYBACK_HEIGHT;
    rect.DeflateRect(10, 10);

    _time_table->create(this, rect, control_::TIME_TABLE);
    _time_table->set_listenr(this);
    _event_list->create(this, rect, control_::EVENT_LIST);
    _event_list->set_listenr(this);
    _event_list->show(false);
}

void controller_search_g2::finalize(void)
{

}

//////////////////////////////////////////////////////////////////////////

void controller_search_g2::stop_impl(void)
{
    int   channel = _ci._channel;
    int   host_id = _ci._host_id;
    int channelext = _ci._channelext;

    G2RETURN_IF_FAIL(client::valid_host_id(host_id));
    G2RETURN_IF_FAIL(client::valid_channel(channel));

    if (_adaptor->is_stopped(channel)) {
        return;
    }

    screen_actuator& actuator = runner_ptr()->actuator_ref();
    actuator.stop_search_enter(host_id);

    G2ROLLBACK_INFO rbi = { 0 };
    rbi._channelext = channelext;
    rbi._spot = client::find_last_spot_from_host_id(host_id);

    _adaptor->request_pause(channel, true, rbi);
    _adaptor->set_play_control_command(channel, G2PLAYER::NONE);
    _ci._playback._speed = G2PLAYER::NONE;

    set_play_speed(G2PLAYER::NONE);

    actuator.stop_search_leave(host_id);

    /////////////////////////////////////////////

    _btn_play.ShowWindow(SW_SHOW);
    _btn_stop.ShowWindow(SW_HIDE);
}

void controller_search_g2::enable_control(int channel)
{
    PostMessage(um_controller_::UM_ENABLE_CONTROLS, channel);
}

void controller_search_g2::reset_speed(void)
{
    _sld_speed.SetPos(0);
    _stc_speed.SetWindowText(L"");
}

int controller_search_g2::set_play_speed(int command, int client_speed)
{
    G2RETURN_VAL_IF_FAIL(client::valid_host_id(_ci._host_id), 0);

    int speed = 0;
    if (client::runner* runner = runner_ptr()) {
        speed = runner->set_play_speed(_ci._host_id, command, client_speed);
    }
    return speed;
}

void controller_search_g2::request_speed_shuttle(void)
{
    int pos = _sld_speed.GetPos();
    int cltspeed = (search::speed_::BOUND_LOWER <= pos &&
                    search::speed_::BOUND_UPPER >= pos) ? pos : search::speed_::PLAY_STOP;
    int svrspeed = G2PLAYER::NONE;

    switch (pos) {
        case search::speed_::BACK_FASTEST_MORE:
        case search::speed_::BACK_FASTEST:
            svrspeed = G2PLAYER::BACK_FASTEST;
            break;
        case search::speed_::BACK_FASTER_MORE:
        case search::speed_::BACK_FASTER:
            svrspeed = G2PLAYER::BACK_FASTER;
            break;
        case search::speed_::BACK_FASTEST_INFINITE:
        case search::speed_::BACK_FAST_MORE:
        case search::speed_::BACK_FAST:
            svrspeed = G2PLAYER::BACK_FAST;
            break;
        case search::speed_::BACK_NORMAL_INFINITE:
        case search::speed_::BACK_NORMAL_TRIPLE:
        case search::speed_::BACK_NORMAL_TWICE:
        case search::speed_::BACK_NORMAL_HFAST:
        case search::speed_::BACK_NORMAL:
            svrspeed = G2PLAYER::BACK_NORMAL;
            break;
        case search::speed_::BACK_SLOW:
            svrspeed = G2PLAYER::BACK_SLOW;
            break;
        case search::speed_::PLAY_STOP:
            svrspeed = G2PLAYER::NONE;
            break;
        case search::speed_::PLAY_SLOW:
            svrspeed = G2PLAYER::PLAY_SLOW;
            break;
        case search::speed_::PLAY_NORMAL:
        case search::speed_::PLAY_NORMAL_HFAST:
        case search::speed_::PLAY_NORMAL_TWICE:
        case search::speed_::PLAY_NORMAL_TRIPLE:
        case search::speed_::PLAY_NORMAL_INFINITE:
            svrspeed = G2PLAYER::PLAY_NORMAL;
            break;
        case search::speed_::PLAY_FAST:
        case search::speed_::PLAY_FAST_MORE:
        case search::speed_::PLAY_FASTEST_INFINITE:
            svrspeed = G2PLAYER::PLAY_FAST;
            break;
        case search::speed_::PLAY_FASTER:
        case search::speed_::PLAY_FASTER_MORE:
            svrspeed = G2PLAYER::PLAY_FASTER;
            break;
        case search::speed_::PLAY_FASTEST:
        case search::speed_::PLAY_FASTEST_MORE:
            svrspeed = G2PLAYER::PLAY_FASTEST;
            break;
        default:
            assert(!"speed slider control range is abnormal");
            break;
    }
    cltspeed = client::revise_speed(svrspeed, cltspeed);
    int speed = set_play_speed(svrspeed, cltspeed);

    CString string;
    if (speed != 0) {
        if (speed == _I32_MAX) {
            string = (svrspeed > 0) ? _T("+¡Äx") : _T("-¡Äx");
        }
        else {
            string.Format(L"%+.1fx", static_cast<float>(speed) / 10.0F);
        }

    }
    _stc_speed.SetWindowText(string);

    if (svrspeed == _ci._playback._speed) {
        return;
    }

    G2ROLLBACK_INFO rbi = { 0 };
    rbi._channelext = _ci._channelext;
    rbi._spot = client::find_last_spot_from_host_id(_ci._host_id);

    _ci._playback._speed = svrspeed;
    _ci._playback._rbi = rbi;

    if (svrspeed == G2PLAYER::NONE) {
        stop_impl();
    }
    else {
        _adaptor->request_play(_ci._channel, _ci._playback);
        _adaptor->set_play_control_command(_ci._channel, _ci._playback._speed);
    }
}

void controller_search_g2::load_time_table(int channel)
{
    G2RETURN_IF_FAIL(client::valid_channel(channel));

    g2::scoped_criticalsection lock(_cs_data);
    search_data_ptr data = client::search_data_manager::get().find_search_data(search::SEARCH_LOCAL_G2, channel);
    G2RETURN_IF_FAIL(data.get() != NULL);

    std::set<int> cameras;
    if (_adaptor && _adaptor->get_camera_list_interest(channel, cameras)) {
        _time_table->update(data, cameras);
    }
}

bool controller_search_g2::balance_time_table(void)
{
    int channel = _ci._channel;
    G2RETURN_VAL_IF_FAIL(client::valid_channel(channel), false);

    g2::scoped_criticalsection lock(_cs_data);

    search_data_ptr data = client::search_data_manager::get().find_search_data(search::SEARCH_LOCAL_G2, channel);
    G2RETURN_VAL_IF_ASSERT(data.get() != NULL, false);

    G2SCOPE scope;
    g2_scope_make_invalid(&scope);

    int reqcount = 2;
    scope._begin = data->spot_minute_last();

    lock.free();

    return _adaptor->request_record_time_info(channel, G2RECORD_TIME_INFO::MINUTE, search::direction_::DIRECTION_RIGHT, scope, reqcount, search::request_::REQUEST_OVERWRITE_RIGHT);
}

void controller_search_g2::change_search_mode(int mode)
{
    G2RETURN_IF_FAIL(_adaptor->is_connected(_ci._channel));
    
    if (mode == _mode) return;
    
    _mode = mode;
    _event_list->set_host_id(_ci._host_id);

    CString string;
    if (_mode == mode_::TIMELAPSE) {
        string = L"Event";

        _time_table->show(true);
        _event_list->show(false);
    }
    else {
        string = L"Time";

        _event_list->show(true);
        _time_table->show(false);
    }

    _btn_mode.SetWindowText(string);
}

//////////////////////////////////////////////////////////////////////////

void controller_search_g2::on_cbx_selchange_format(void)
{
    int cursel = _cbx_format.GetCurSel();

    screen_actuator& actuator = runner_ptr()->actuator_ref();
    actuator.set_layout(screen_formatter_::LAYOUT(cursel));

    /////////////////////////////////////////////

    BOOL enable = (cursel == screen_formatter_::LAYOUT_6x6) ? FALSE : TRUE;
    if (CButton* btn_prev = (CButton*)GetDlgItem(IDC_BTN_SEARCH_G2_LAYOUT_PREV)) {
        btn_prev->EnableWindow(enable);
    }
    if (CButton* btnNext = (CButton*)GetDlgItem(IDC_BTN_SEARCH_G2_LAYOUT_NEXT)) {
        btnNext->EnableWindow(enable);
    }
}

void controller_search_g2::on_btn_mode(void)
{
    int mode = _event_list->is_show() ? mode_::TIMELAPSE : mode_::EVENT;
    change_search_mode(mode);
}

void controller_search_g2::on_btn_filter(void)
{
    int host_id = _ci._host_id;
    int channel = _ci._channel;
    G2RETURN_IF_FAIL(valid_channel(channel));
    G2RETURN_IF_FAIL(_adaptor->is_connected(channel));

    stop_impl();

    G2SEARCH_G2_EVENT_SEARCH_OPTIONS    event;
    G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS  textin;
    G2CHANNEL_SET channels;
    std::set<int> cameras;
    std::vector<G2EVENT::TYPE> supported;
    int mode;

    G2_PRODUCT_INFO_CAPS::REMOTE_SEARCH caps = { 0 };
    if (_adaptor->get_remote_search_caps(channel, caps)) {
        supported.assign(reinterpret_cast<G2EVENT::TYPE*>(caps.events), reinterpret_cast<G2EVENT::TYPE*>(caps.events) + caps.events_len);
    }

    _adaptor->get_option_query_event(channel, event);
    _adaptor->get_option_query_text_in(channel, textin);
    _adaptor->get_event_query_cameras(channel, cameras);
    mode = _adaptor->get_event_query_mode(channel);

    channels._len = 0;
    for (std::set<int>::const_iterator itr = cameras.begin();
        itr != cameras.end();
        ++itr) {
            channels._channels[channels._len++] = *itr;
    }

    client::connective_host_list* host_list = runner_ptr()->get_host_list();
    std::wstring site = host_list->site_name_from_host_id(host_id);

    G2_PRODUCT_INFO pi;
    _adaptor->get_product_info(channel, pi);
   
    search_g2_condition_dlg dlg(supported, runner_ptr());
    dlg.set_condition_event(event);
    dlg.set_condition_text_in(textin);
    dlg.set_query_mode((G2SEARCH_G2_QUERY::MODE)mode);
    dlg.set_query_cameras(channels);
    dlg.set_device_info(site.c_str(), pi); 

    if (dlg.DoModal() != IDOK || _adaptor->is_connected(channel) != true) {
        return;
    }

    _event_list->clear(false);

    if (dlg.get_query_mode() == G2SEARCH_G2_QUERY::EVENT) {
        G2SEARCH_G2_EVENT_SEARCH_OPTIONS options; 
        dlg.get_event_condition(options);
        G2CHANNEL_SET cameras       = dlg.get_query_cameras();

        std::set<int> channels;
        for (unsigned i = 0; i < cameras._len; ++i) {
            channels.insert(cameras._channels[i]);
        }

        _adaptor->set_event_query_cameras(channel, channels);
        _adaptor->set_event_query_mode(channel, G2SEARCH_G2_QUERY::EVENT);
        _adaptor->request_event_log_search(channel, options);
    }
    else {
        G2SEARCH_G2_TEXT_IN_SEARCH_OPTIONS options;
        dlg.get_text_in_condition(options);

        _adaptor->set_event_query_mode(channel, G2SEARCH_G2_QUERY::TEXT_IN);
        _adaptor->request_text_in_log_search(channel, options);
    }
}

void controller_search_g2::on_btn_layout_prev(void)
{
    screen_actuator& actuator = runner_ptr()->actuator_ref();
    actuator.set_layout_page_prev();
}

void controller_search_g2::on_btn_layout_next(void)
{
    screen_actuator& actuator = runner_ptr()->actuator_ref();
    actuator.set_layout_page_next();
}

void controller_search_g2::on_btn_go_to(void)
{
    G2RETURN_IF_FAIL(_enable);

    CMenu menu, *popup = NULL;
    if (menu.LoadMenu(IDR_MENU_SEARCH_G2) &&
        (popup = menu.GetSubMenu(play_menu_::CONTEXT_GOTO))) {
            // success loaded
    }
    G2RETURN_IF_FAIL(popup != NULL);

    CRect rect;
    ::GetWindowRect(GetDlgItem(IDC_BTN_SEARCH_G2_GO_TO)->GetSafeHwnd(), &rect);
    popup->TrackPopupMenu(TPM_LEFTALIGN | TPM_NOANIMATION, rect.left, rect.bottom, this);
}

void controller_search_g2::on_btn_backward(void)
{
    G2RETURN_IF_FAIL(_enable);

    int channel = _ci._channel;
    int host_id = _ci._host_id;

    on_btn_stop();

    if (_move_step == move_::FRAME_1) {
        _adaptor->request_prev_step(channel);
    }
    else {
        int factor = 1;
        switch (_move_step) {
            case move_::MIN_01: factor =  1; break;
            case move_::MIN_05: factor =  5; break;
            case move_::MIN_10: factor = 10; break;
            case move_::MIN_15: factor = 15; break;
            case move_::MIN_30: factor = 30; break;
            case move_::MIN_60: factor = 60; break;
        }

        G2SPOT spot = client::find_last_spot_from_host_id(host_id);
        CTime time = CTime(g2_time_to_time32_t(&spot._time)) - CTimeSpan(0, 0, factor, 0);
        g2_time_from_time32_t(&spot._time, (unsigned long)time.GetTime());
        spot._tick = UINT_MAX;

        _adaptor->request_move_to_spot(channel, spot, G2PLAYER::PRECISION::SECOND, false);
    }

    _adaptor->set_play_control_command(channel, G2PLAYER::PREV_STEP);
}

void controller_search_g2::on_btn_forward(void)
{
    G2RETURN_IF_FAIL(_enable);

    int channel = _ci._channel;
    int host_id = _ci._host_id;

    on_btn_stop();

    if (_move_step == move_::FRAME_1) {
        _adaptor->request_next_step(channel);
    }
    else {
        int factor = 1;
        switch (_move_step) {
            case move_::MIN_01: factor =  1; break;
            case move_::MIN_05: factor =  5; break;
            case move_::MIN_10: factor = 10; break;
            case move_::MIN_15: factor = 15; break;
            case move_::MIN_30: factor = 30; break;
            case move_::MIN_60: factor = 60; break;
        }

        G2SPOT spot = client::find_last_spot_from_host_id(host_id);
        CTime time = CTime(g2_time_to_time32_t(&spot._time)) + CTimeSpan(0, 0, factor, 0);
        g2_time_from_time32_t(&spot._time, (unsigned long)time.GetTime());
        spot._tick = 0;

        _adaptor->request_move_to_spot(channel, spot, G2PLAYER::PRECISION::SECOND, true);
    }

    _adaptor->set_play_control_command(channel, G2PLAYER::NEXT_STEP);
}

void controller_search_g2::on_btn_move_step(void)
{
    G2RETURN_IF_FAIL(_enable);

    CMenu menu, *popup = NULL;
    if (menu.LoadMenu(IDR_MENU_SEARCH_G2) &&
        (popup = menu.GetSubMenu(play_menu_::CONTEXT_MOVE))) {
            // success loaded
    }
    G2RETURN_IF_FAIL(popup != NULL);

    unsigned int select = _move_step + ID_SEARCH_G2_MOVE_1FRAME;
    if ((unsigned int)ID_SEARCH_G2_MOVE_1FRAME <= select &&
        select <= (unsigned int)ID_SEARCH_G2_MOVE_60MINUTE) {
            popup->CheckMenuItem(select, MF_CHECKED | MF_BYCOMMAND);
    }

    CRect rect;
    ::GetWindowRect(GetDlgItem(IDC_BTN_SEARCH_G2_MOVE_STEP)->GetSafeHwnd(), &rect);
    popup->TrackPopupMenu(TPM_LEFTALIGN | TPM_NOANIMATION, rect.left, rect.bottom, this);
}

void controller_search_g2::on_move_step(UINT nID)
{
    CString string;
    switch (nID) {
        case ID_SEARCH_G2_MOVE_1FRAME:   string = L"1F";  break;
        case ID_SEARCH_G2_MOVE_1MINUTE:  string = L"1M";  break;
        case ID_SEARCH_G2_MOVE_5MINUTE:  string = L"5M";  break;
        case ID_SEARCH_G2_MOVE_10MINUTE: string = L"10"; break;
        case ID_SEARCH_G2_MOVE_15MINUTE: string = L"15"; break;
        case ID_SEARCH_G2_MOVE_30MINUTE: string = L"30"; break;
        case ID_SEARCH_G2_MOVE_60MINUTE: string = L"60";  break;
    }

    if (string.IsEmpty() != true) {
        _move_step = nID - ID_SEARCH_G2_MOVE_1FRAME;
        GetDlgItem(IDC_BTN_SEARCH_G2_MOVE_STEP)->SetWindowText(string);
    }
}

void controller_search_g2::on_btn_play(void)
{
    G2RETURN_IF_FAIL(_enable);
    G2RETURN_IF_FAIL(_adaptor != NULL);

    int   channel = _ci._channel;
    int   host_id = _ci._host_id;
    int channelext = _ci._channelext;

    G2RETURN_IF_FAIL(client::valid_channel(channel));
    G2RETURN_IF_FAIL(_adaptor->is_stopped(channel));

    _btn_stop.ShowWindow(SW_SHOW);
    _btn_play.ShowWindow(SW_HIDE);

    set_play_speed(G2PLAYER::PLAY_NORMAL);

    G2ROLLBACK_INFO rbi = { 0 };
    rbi._spot = client::find_last_spot_from_host_id(host_id);
    rbi._channelext = channelext;

    G2PLAYBACK_COMMAND cmd;
    cmd._speed = G2PLAYER::PLAY_NORMAL;
    cmd._rbi = rbi;

    _adaptor->request_play(channel, cmd);
    _adaptor->set_play_control_command(channel, G2PLAYER::PLAY_NORMAL);
}

void controller_search_g2::on_btn_stop(void)
{
    stop_impl();
    reset_speed();
}

//////////////////////////////////////////////////////////////////////////

void controller_search_g2::on_goto_time(void)
{
    G2RETURN_IF_FAIL(_enable);
    G2RETURN_IF_FAIL(_adaptor != NULL);
    G2RETURN_IF_FAIL(client::valid_channel(_ci._channel));

    int channel = _ci._channel;

    CTime current = CTime::GetCurrentTime();
    G2TIME time;
    g2_time_from_time32_t(&time, (unsigned long)current.GetTime());

    time_goto_dlg dlg(runner_ptr());
    dlg.set_support_date(true);
    dlg.set_current_time(time);
    if(dlg.DoModal() == IDOK) {
        stop_impl();
        time = dlg.current_time();

        std::set<int> channels;
        _adaptor->get_camera_list(channel, channels);
        if (channels.empty() != true) {
            _adaptor->request_spot_list(channel, time, channels);
        }
    }
}

void controller_search_g2::on_goto_first(void)
{
    G2RETURN_IF_FAIL(_enable);
    G2RETURN_IF_FAIL(client::valid_channel(_ci._channel));
    int channel = _ci._channel;

    stop_impl();

    _adaptor->request_move_to_first(channel);
    _adaptor->set_play_control_command(channel, G2PLAYER::MOVE_TO_FIRST);
}

void controller_search_g2::on_goto_last(void)
{
    G2RETURN_IF_FAIL(_enable);
    G2RETURN_IF_FAIL(client::valid_channel(_ci._channel));
    int channel = _ci._channel;

    stop_impl();

    _adaptor->request_move_to_last(channel);
    _adaptor->set_play_control_command(channel, G2PLAYER::MOVE_TO_LAST);
}

void controller_search_g2::on_btn_calendar(void)
{
    G2RETURN_IF_FAIL(_enable);

    int channel = _ci._channel;
    G2RETURN_IF_FAIL(client::valid_channel(channel));

    calendar_dlg dlg(runner_ptr(), search::SEARCH_LOCAL_G2);
    dlg.set_channel(channel);

    if (dlg.DoModal() == IDOK) {
        stop_impl();

        CTime date = dlg.get_selected_date();
        G2TIME from, to;
        g2_time_from_time32_t(&from, (unsigned long)date.GetTime());
        g2_time_from_time32_t(&to, (unsigned long)CTime(date.GetYear(), date.GetMonth(), date.GetDay(), 23, 59, 59).GetTime());

        std::set<int> channels;
        if (g2_time_is_valid(&from) &&
            _adaptor->get_camera_list(channel, channels)) {
            _adaptor->request_scope_list(channel, from, to, channels, G2PLAY_SCOPE_TYPE::GOTO);
        }
    }
}

void controller_search_g2::on_btn_clipcopy(void)
{
    stop_impl();

    client::connective_host_list* hostList = runner_ptr()->get_host_list();
    connective_host_info_ptr hi = hostList->get_host_info(_ci._channel, client::connect_mode_::SEARCH_G2);
    G2RETURN_IF_FAIL(hi != NULL);

    clip_copy_dlg dlg(hi->host_id(), _adaptor, runner_ptr());
    dlg.DoModal();
}

void controller_search_g2::on_sld_speed_release_capture(NMHDR *pNMHDR, LRESULT *pResult)
{
    request_speed_shuttle();

    *pResult = 0;
}

//////////////////////////////////////////////////////////////////////////

bool controller_search_g2::create(CWnd* parent)
{
    bool created = (CDialog::Create(IDD_CONTROLLER_SEARCH_G2, parent) != FALSE);
    if (created) {
        _parent = parent;
    }
    else {
        assert(!"failed to create live controller");
    }
    return created;
}

void controller_search_g2::on_screen_layout_changed(int layout, int changed)
{
    G2RETURN_IF_FAIL(layout >= screen_formatter_::LAYOUT_1x1 &&
                     layout < screen_formatter_::LAYOUT_COUNT);
    G2RETURN_IF_FAIL(layout != _cbx_format.GetCurSel());

    _cbx_format.SetCurSel(layout);
}

void controller_search_g2::on_screen_image_drew(short camera, const G2SPOT& spot)
{
    G2RETURN_IF_FAIL(client::valid_camera(camera));

    int channel = _ci._channel;
    G2RETURN_IF_FAIL(client::valid_channel(channel));

    g2::scoped_criticalsection lock(_cs_data);
    search_data_ptr data = client::search_data_manager::get().find_search_data(search::SEARCH_LOCAL_G2, channel);
    G2RETURN_IF_ASSERT(data.get() != NULL);

    _time_table->screen_image_drew(spot);
    data->set_spot_selected(spot);

    if (_adaptor->get_play_control_command(channel) != G2PLAYER::MOVE_TO_SPOT) {
        if (client::is_same_date(CTime(g2_time_to_time32_t(&spot._time)), CTime(g2_time_to_time32_t(&data->spot_standard()._time))) == false ||
            spot._segment != data->spot_standard()._segment) {
            const search_minute_info* info = data->get_minute_info();
            bool request = true;

            search::SEARCH_MINUTE_INFO_LIST list;
            if (info && info->get_minute_info(_ci._channelext, list)) {
                for (unsigned int i = 0; i < list.size(); i += 60) {
                    if (client::is_same_date(list.at(i)._time, g2_time_to_time32_t(&spot._time)) && 
                        spot._segment == list.at(i)._segment) {
                        request = false;
                        break;
                    }
                }
            }

            if (request) {
                G2SCOPE scope = { spot, spot };

                CTime time(g2_time_to_time32_t(&spot._time));
                g2_time_from_time32_t(&scope._begin._time, (unsigned long)CTime(time.GetYear(), time.GetMonth(), time.GetDay(), 0, 0, 0).GetTime());
                g2_time_from_time32_t(&scope._end._time, (unsigned long)CTime(time.GetYear(), time.GetMonth(), time.GetDay(), 23, 59, 59).GetTime());

                TRACE(L"play request record time info : %d-%d-%d\n", time.GetYear(), time.GetMonth(), time.GetDay());

                data->set_spot_standard(spot);
                data->clear_info_minute();
                if (_adaptor) {
                    _adaptor->request_record_time_info_on_time(channel, G2RECORD_TIME_INFO::MINUTE, search::direction_::DIRECTION_RIGHT, scope._begin._time, scope._end._time, 512, search::request_::REQUEST_INIT, NULL);
                }
            }
        }
    }
    lock.free();

    return;
}

void controller_search_g2::on_changed_select_time_table(const G2SPOT& spot)
{
    G2RETURN_IF_FAIL(g2_spot_is_valid(&spot));

    //stop_impl();

    int channel = _ci._channel;
    if (_adaptor->is_connected(channel)) {
        _adaptor->request_move_to_spot(channel, spot, G2PLAYER::PRECISION::MINUTE, true);
        _adaptor->set_play_control_command(channel, G2PLAYER::MOVE_TO_SPOT);
    }
}

void controller_search_g2::on_request_more_event_search_g2(void)
{
    int channel = _ci._channel;
    G2RETURN_IF_FAIL(_adaptor->is_connected(channel));

    int queryMode = _adaptor->get_event_query_mode(channel);
    if (queryMode == G2SEARCH_G2_QUERY::EVENT) {
        _adaptor->request_event_log_search_next(channel);
    }
    else if (queryMode == G2SEARCH_G2_QUERY::TEXT_IN) {
        _adaptor->request_text_in_log_search_next(channel);
    }
    else {
        assert(!"undefined event search query type");
    }

    runner_ptr()->show_screen_message(L"Search...", true, 10000);
    set_enable(false);
}

void controller_search_g2::on_request_load_event_image_search_g2(G2SPOT spot)
{
    int channel = _ci._channel;
    G2RETURN_IF_FAIL(_adaptor->is_connected(channel));

    _adaptor->request_move_to_spot(channel, spot, G2PLAYER::PRECISION::MINUTE, true);
    _adaptor->set_play_control_command(channel, G2PLAYER::MOVE_TO_SPOT);
}

//////////////////////////////////////////////////////////////////////////

void controller_search_g2::on_receive_notify_command_begin_play(int channel, int command)
{

}

void controller_search_g2::on_receive_notify_command_end_play(int channel, int command)
{

}

void controller_search_g2::on_receive_notify_play_speed_changed_play(int channel, int speed)
{
    G2RETURN_IF_FAIL(this != NULL && ::IsWindow(GetSafeHwnd()));
    G2RETURN_IF_FAIL(client::valid_channel(channel));

    enable_control(channel);
    set_play_speed(speed);
}

void controller_search_g2::on_receive_scope_list(int channel, const std::vector<G2SCOPE>& scopes, int type)
{
    G2RETURN_IF_FAIL(client::valid_channel(channel));

    if (type == G2PLAY_SCOPE_TYPE::GOTO) {
        if (scopes.size() == 1) {
            _adaptor->request_move_to_spot(channel, scopes[0]._begin, G2PLAYER::PRECISION::FRAME, true);
            _adaptor->set_play_control_command(channel, G2PLAYER::COMMAND_MOVE);
        }
        else if (scopes.size() > 1) {
            std::vector<G2SCOPE>* buf = new std::vector<G2SCOPE>(scopes);
            PostMessage(um_controller_::UM_RECEIVE_SCOPE_LIST, WPARAM(channel), LPARAM(buf));
        }
    }
}

void controller_search_g2::on_receive_spot_list(int channel, const std::vector<G2SPOT>& spots)
{
    G2RETURN_IF_FAIL(client::valid_channel(channel));

    if (spots.size() == 1) {
        _adaptor->request_move_to_spot(channel, spots[0], G2PLAYER::PRECISION::SECOND, true);
        _adaptor->set_play_control_command(channel, G2PLAYER::COMMAND_MOVE);
    }
    else if (spots.size() > 1) {
        std::vector<G2SPOT>* buf = new std::vector<G2SPOT>(spots);
        PostMessage(um_controller_::UM_RECEIVE_SPOT_LIST, WPARAM(channel), LPARAM(buf));
    }
}

void controller_search_g2::on_receive_event_query_result(int channel, const std::vector<G2EVENT>& events)
{
    G2RETURN_IF_FAIL(client::valid_channel(channel));
    G2RETURN_IF_FAIL(_ci._channel == channel);

    if (events.empty()) {
        set_enable(true);
        return;
    }

    change_search_mode(mode_::EVENT);

    if (_event_list) {
        _event_list->insert(events);
    }

    runner_ptr()->hide_screen_message();
    set_enable(true);
}

void controller_search_g2::on_receive_text_in_query_result(int channel, const std::vector<G2EVENT>& events)
{
    G2RETURN_IF_FAIL(client::valid_channel(channel));

    if (events.empty()) {
        runner_ptr()->show_screen_message(L"No result.", true, 3000);
        set_enable(true);
        return;
    }

    change_search_mode(mode_::EVENT);

    if (_event_list) {
        _event_list->insert(events, G2SEARCH_G2_QUERY::TEXT_IN);
    }

    runner_ptr()->hide_screen_message();
    set_enable(true);
}

//////////////////////////////////////////////////////////////////////////

void controller_search_g2::request_stop_sync(void)
{
    if (GetDlgItem(IDC_BTN_SEARCH_G2_STOP)->IsWindowVisible()) {
        on_btn_stop();
    }
}

void controller_search_g2::request_first(void)
{
    PostMessage(WM_COMMAND, ID_SEARCH_G2_GOTO_FIRST);
}

void controller_search_g2::request_last(void)
{
    PostMessage(WM_COMMAND, ID_SEARCH_G2_GOTO_LAST);
}

void controller_search_g2::request_stop(void)
{
    HWND control = GetDlgItem(IDC_BTN_SEARCH_G2_STOP)->GetSafeHwnd();
    if (control != NULL &&
        ::IsWindow(control)) {
        PostMessage(WM_COMMAND, MAKEWPARAM(IDC_BTN_SEARCH_G2_STOP, BN_CLICKED), LPARAM(GetDlgItem(IDC_BTN_SEARCH_G2_STOP)->GetSafeHwnd()));
    }
}

void controller_search_g2::screen_end_of_play(int host_id)
{
    reset_speed();
    set_play_speed(G2PLAYER::NONE, client::search::speed_::PLAY_STOP);
}

bool controller_search_g2::is_origin_pos_speed(void)
{
    return (_sld_speed.GetPos() == 0);
}

void controller_search_g2::load_site(int channel)
{
    G2RETURN_IF_FAIL(client::valid_channel(channel));

    PostMessage(um_controller_::UM_LOAD_TIME_TABLE, WPARAM(channel));
}

void controller_search_g2::update_site(int channel)
{
    G2RETURN_IF_FAIL(client::valid_channel(channel));

    PostMessage(um_controller_::UM_UPDATE_TIME_TABLE, WPARAM(channel));
}

//////////////////////////////////////////////////////////////////////////

LRESULT controller_search_g2::on_post_screen_camera_changed(WPARAM wParam, LPARAM lParam)
{
    int camera = static_cast<int>(wParam);
    G2RETURN_VAL_IF_FAIL(client::valid_camera(camera), 1L);

    _ci._selcamera  = camera;
    _ci._channel    = client::find_channel_from_camera(camera);
    _ci._channelext = client::find_host_camera_from_camera(camera);
    _ci._host_id    = client::find_host_id_from_camera(camera);

    /////////////////////////////////////////////

    if (client::valid_channel(_ci._channel)) {
        if (_adaptor->is_stopped(_ci._channel)) {
            _btn_play.ShowWindow(SW_SHOW);
            _btn_stop.ShowWindow(SW_HIDE);
        }
        else {
            _btn_stop.ShowWindow(SW_SHOW);
            _btn_play.ShowWindow(SW_HIDE);

            //load_site(_ci._channel);
        }

        _sld_speed.EnableWindow(TRUE);
    }
    else {
        _ci.reset();

        _btn_play.ShowWindow(SW_SHOW);
        _btn_stop.ShowWindow(SW_HIDE);

        reset_speed();
        _sld_speed.EnableWindow(FALSE);
    }

    return 0L;
}

LRESULT controller_search_g2::on_post_receive_scope_list(WPARAM wParam, LPARAM lParam)
{
    int channel = static_cast<int>(wParam);
    std::auto_ptr<std::vector<G2SCOPE> > buf((std::vector<G2SCOPE>*)lParam);

    G2RETURN_VAL_IF_ASSERT(client::valid_channel(channel), 1L);
    G2RETURN_VAL_IF_ASSERT(buf.get() != NULL, 1L);

    std::vector<G2SCOPE> list(*buf);
    buf.reset();

    /////////////////////////////////////////////

    select_segment_dlg dlg(runner_ptr());
    dlg.set_mode(select_segment_dlg::SELECT_MODE_SCOPE);
    dlg.set_scope_info(list);

    if (dlg.DoModal() != IDOK) return 0L;

    _adaptor->request_move_to_spot(channel, dlg.selected_spot(), G2PLAYER::PRECISION::FRAME, true);
    _adaptor->set_play_control_command(channel, G2PLAYER::COMMAND_MOVE);

    return 0L;
}

LRESULT controller_search_g2::on_post_receive_spot_list(WPARAM wParam, LPARAM lParam)
{
    int channel = static_cast<int>(wParam);
    std::auto_ptr<std::vector<G2SPOT> > buf((std::vector<G2SPOT>*)lParam);

    G2RETURN_VAL_IF_ASSERT(client::valid_channel(channel), 1L);
    G2RETURN_VAL_IF_ASSERT(buf.get() != NULL, 1L);

    std::vector<G2SPOT> list(*buf);
    buf.reset();

    /////////////////////////////////////////////

    select_segment_dlg dlg(runner_ptr());
    dlg.set_mode(select_segment_dlg::SELECT_MODE_SPOT);
    dlg.set_spot_info(list);

    if (dlg.DoModal() != IDOK) return 0L;

    _adaptor->request_move_to_spot(channel, dlg.selected_spot(), G2PLAYER::PRECISION::SECOND, true);
    _adaptor->set_play_control_command(channel, G2PLAYER::COMMAND_MOVE);

    return 0L;
}

LRESULT controller_search_g2::on_post_enable_controls(WPARAM wParam, LPARAM lParam)
{
    int channel = _ci._channel;
    G2RETURN_VAL_IF_FAIL(client::valid_channel(channel), 1L);

    bool stopped = _adaptor->is_stopped(channel);

    _btn_stop.ShowWindow(stopped ? SW_HIDE : SW_SHOW);
    _btn_play.ShowWindow(stopped ? SW_SHOW : SW_HIDE);

    if (stopped) {
        std::set<int> channels;
        if (_adaptor->get_camera_list(channel, channels) &&
            channels.empty()) {
            reset_speed();
        }
    }

    return 0L;
}

LRESULT controller_search_g2::on_post_disconnected(WPARAM wParam, LPARAM lParam)
{
    int channel = static_cast<int>(lParam);
    search_data_manager::get().remove_search_data(search::SEARCH_LOCAL_G2, channel);

    if (_event_list) {
        _event_list->clear(true);
    }

    if (_time_table) {
        _time_table->clear();
    }

    return 0L;
}

LRESULT controller_search_g2::on_post_load_time_table(WPARAM wParam, LPARAM lParam)
{
    int channel = static_cast<int>(wParam);
    load_time_table(channel);

    return 0L;
}

LRESULT controller_search_g2::on_post_update_time_table(WPARAM wParam, LPARAM lParam)
{
    load_time_table(_ci._channel);

    return 0L;
}
