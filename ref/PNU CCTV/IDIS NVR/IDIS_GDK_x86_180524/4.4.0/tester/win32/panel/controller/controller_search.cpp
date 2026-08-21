// controller_search.cpp : implementation
//

#include "stdafx.h"
#include "G2Client.h"
#include "MainFrm.h"
#include "G2ClientView.h"
#include "controller_search.h"
#include "control/play/time_goto_dlg.h"
#include "control/play/select_segment_dlg.h"
#include "control/play/calendar_dlg.h"
#include "control/play/time_table_hour.h"
#include "control/play/time_table_minute.h"
#include "control/play/time_table_minute_idr.h"
#include "control/play/clip_copy_dlg.h"
#include "control/play/select_external_tango_dlg.h"
#include "control/play/search_event_list.h"
#include "control/play/search_condition_dlg.h"
#include "search/search_common.h"
#include "search/search_data_manager.h"
#include "search/search_minute_info.h"
#include <sampler/cpp/g2client_search.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

enum { IDD = IDD_CONTROLLER_SEARCH };

//////////////////////////////////////////////////////////////////////////

controller_search::controller_search(void)
    : _search(NULL)
    , _enable(false)
{
    _timeTable[control_::TIME_TABLE_HOUR] = boost::shared_ptr<time_table_base>(new time_table_hour);
    _timeTable[control_::TIME_TABLE_MINUTE] = boost::shared_ptr<time_table_base>(new time_table_minute);
    _timeTable[control_::TIME_TABLE_MINUTE_IDR] = boost::shared_ptr<time_table_base>(new time_table_minute_idr);

    _eventList = boost::shared_ptr<search_event_list>(new search_event_list);
}

controller_search::~controller_search(void)
{

}

//////////////////////////////////////////////////////////////////////////

void controller_search::DoDataExchange(CDataExchange* pDX)
{
    CDialog::DoDataExchange(pDX);

    DDX_Control(pDX, IDC_BTN_SEARCH_MODE,               _buttons[buttons_::MODE]);
    DDX_Control(pDX, IDC_BTN_SEARCH_FILTER,             _buttons[buttons_::FILTER]);
    DDX_Control(pDX, IDC_BTN_SEARCH_CALENDAR,           _buttons[buttons_::CALENDAR]);
    DDX_Control(pDX, IDC_BTN_SEARCH_GO_TO,              _buttons[buttons_::GO_TO]);
    DDX_Control(pDX, IDC_BTN_SEARCH_ADDITIONAL_MENU,    _buttons[buttons_::ADDITIONAL]);
    DDX_Control(pDX, IDC_BTN_SEARCH_EXPORT_VIDEO,       _buttons[buttons_::SAVE]);
    DDX_Control(pDX, IDC_BTN_SEARCH_EXPORT_VIDEO,       _buttons[buttons_::SAVE]);
    DDX_Control(pDX, IDC_BTN_SEARCH_REW,                _buttons[buttons_::REW]);
    DDX_Control(pDX, IDC_BTN_SEARCH_PREV,               _buttons[buttons_::PREV]);
    DDX_Control(pDX, IDC_BTN_SEARCH_PLAY,               _buttons[buttons_::PLAY]);
    DDX_Control(pDX, IDC_BTN_SEARCH_STOP,               _buttons[buttons_::STOP]);
    DDX_Control(pDX, IDC_BTN_SEARCH_NEXT,               _buttons[buttons_::NEXT]);
    DDX_Control(pDX, IDC_BTN_SEARCH_FF,                 _buttons[buttons_::FF]);
    DDX_Control(pDX, IDC_BTN_SEARCH_LAYOUT_PREV,        _buttons[buttons_::LAYOUT_PREV]);
    DDX_Control(pDX, IDC_BTN_SEARCH_LAYOUT_NEXT,        _buttons[buttons_::LAYOUT_NEXT]);

    DDX_Control(pDX, IDC_CBX_SEARCH_SCREEN_FORMAT,      _cbxFormat);
    DDX_Control(pDX, IDC_SLD_SEARCH_SHUTTLE,            _sldShuttle[shuttle_::PLAY]);
    DDX_Control(pDX, IDC_SLD_SEARCH_FAST_SHUTTLE,       _sldShuttle[shuttle_::FAST]);
    DDX_Control(pDX, IDC_STC_SEARCH_SHUTTLE_SPEED,      _stcSpeed);
}

BEGIN_MESSAGE_MAP(controller_search, CDialog)
    ON_WM_CREATE()
    ON_WM_DESTROY()
    ON_WM_SIZE()
    ON_WM_HSCROLL()

    ON_BN_CLICKED(IDC_BTN_SEARCH_MODE,                          on_btn_mode)
    ON_BN_CLICKED(IDC_BTN_SEARCH_FILTER,                        on_btn_filter)
    ON_BN_CLICKED(IDC_BTN_SEARCH_CALENDAR,                      on_btn_calendar)
    ON_BN_CLICKED(IDC_BTN_SEARCH_GO_TO,                         on_btn_go_to)
    ON_BN_CLICKED(IDC_BTN_SEARCH_ADDITIONAL_MENU,               on_btn_additional)
    ON_BN_CLICKED(IDC_BTN_SEARCH_EXPORT_VIDEO,                  on_btn_clipcopy)
    ON_BN_CLICKED(IDC_BTN_SEARCH_REW,                           on_btn_rew)
    ON_BN_CLICKED(IDC_BTN_SEARCH_PREV,                          on_btn_prev)
    ON_BN_CLICKED(IDC_BTN_SEARCH_STOP,                          on_btn_stop)
    ON_BN_CLICKED(IDC_BTN_SEARCH_PLAY,                          on_btn_play)    
    ON_BN_CLICKED(IDC_BTN_SEARCH_NEXT,                          on_btn_next)
    ON_BN_CLICKED(IDC_BTN_SEARCH_FF,                            on_btn_ff)
    ON_BN_CLICKED(IDC_BTN_SEARCH_LAYOUT_PREV,                   on_btn_layout_prev)
    ON_BN_CLICKED(IDC_BTN_SEARCH_LAYOUT_NEXT,                   on_btn_layout_next)
    
    ON_CBN_SELCHANGE(IDC_CBX_SEARCH_SCREEN_FORMAT,              on_cbx_selchange_format)

    ON_COMMAND(ID_PLAY_GOTO_TIME,                               on_goto_time)
    ON_COMMAND(ID_PLAY_GOTO_FIRST,                              on_goto_first)
    ON_COMMAND(ID_PLAY_GOTO_LAST,                               on_goto_last)
    ON_COMMAND_RANGE(ID_DATASOURCE_SEARCHONLOCAL, ID_DATASOURCE_SEARCHONOTHER, on_search_data_source)
    ON_COMMAND(ID_ADDITIONAL_SELECTSEGMENT,                     on_select_segment)

    ON_MESSAGE(um_controller_::UM_SCREEN_CAMERA_CHANGED,        on_post_screen_camera_changed)
    ON_MESSAGE(um_controller_::UM_RECEIVE_SCOPE_LIST,           on_post_receive_scope_list)
    ON_MESSAGE(um_controller_::UM_RECEIVE_SPOT_LIST,            on_post_receive_spot_list)
    ON_MESSAGE(um_controller_::UM_RECEIVE_EXTERNAL_TANGO_LIST,  on_post_receive_external_tango_list)
    ON_MESSAGE(um_controller_::UM_DISCONNECTED,                 on_post_disconnected)
    ON_MESSAGE(um_controller_::UM_LOAD_TIME_TABLE,              on_post_load_time_table)
    ON_MESSAGE(um_controller_::UM_UPDATE_TIME_TABLE,            on_post_update_time_table)
END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////

bool controller_search::create(CWnd* parent)
{
    bool created = (CDialog::Create(IDD, parent) == TRUE);
    if (created) {
        _parent = parent;
    }
    else {
        assert(!"failed to create live controller");
    }
    return created;
}

//////////////////////////////////////////////////////////////////////////

BOOL controller_search::PreTranslateMessage(MSG* pMsg)
{
    bool bReturn = false;

    if (pMsg->message == WM_KEYDOWN) {
        switch(pMsg->wParam) {
            case VK_RETURN:
            case VK_ESCAPE:
                bReturn = true;
                break;
        }
    }

    return (bReturn) ? TRUE : CDialog::PreTranslateMessage(pMsg);
}

//////////////////////////////////////////////////////////////////////////

int controller_search::OnCreate(LPCREATESTRUCT lpcs)
{
    if (CDialog::OnCreate(lpcs) == -1) {
        return -1;
    }

    client::runner* runner = runner_ptr();
    if (runner) {
        _search = &(runner->search_ref());
    }
    assert(_search);

    return 0;
}

BOOL controller_search::OnInitDialog()
{
    CDialog::OnInitDialog();

    initialize();

    return TRUE;
}

void controller_search::OnDestroy()
{
    CDialog::OnDestroy();
}

void controller_search::OnSize(unsigned int nType, int cx, int cy)
{
    CDialog::OnSize(nType, cx, cy);

    CRect rect;
    GetClientRect(&rect);
    rect.top = default_::playback_::HEIGHT;
    rect.DeflateRect(10, 10);

    if (_timeTable[control_::TIME_TABLE_HOUR]->is_initialized() &&
        _timeTable[control_::TIME_TABLE_MINUTE]->is_initialized() &&
        _timeTable[control_::TIME_TABLE_MINUTE_IDR]->is_initialized()) {
        
        _timeTable[control_::TIME_TABLE_HOUR]->SetWindowPos(NULL, 0, 0, rect.Width(), rect.Height(), SWP_NOMOVE | SWP_NOZORDER);
        _timeTable[control_::TIME_TABLE_MINUTE]->SetWindowPos(NULL, 0, 0, rect.Width(), rect.Height(), SWP_NOMOVE | SWP_NOZORDER);
        _timeTable[control_::TIME_TABLE_MINUTE_IDR]->SetWindowPos(NULL, 0, 0, rect.Width(), rect.Height(), SWP_NOMOVE | SWP_NOZORDER);
    }

    if (_eventList->is_initialized()) {
        _eventList->SetWindowPos(NULL, 0, 0, rect.Width(), rect.Height(), SWP_NOMOVE | SWP_NOZORDER);
    }
}

void controller_search::OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar)
{
    CDialog::OnHScroll(nSBCode, nPos, pScrollBar);

    UINT nID = pScrollBar->GetDlgCtrlID();
    switch (nID) {
        case IDC_SLD_SEARCH_SHUTTLE:
        case IDC_SLD_SEARCH_FAST_SHUTTLE:
            change_slider_play_speed(nID);
            break;
    }
}

//////////////////////////////////////////////////////////////////////////

void controller_search::initialize(void)
{
    CString string;
    for (int i = 1; i < 7; ++i) {
        string.Format(_T("Layout %dx%d"), i, i);
        _cbxFormat.AddString(string);
    }
    _cbxFormat.SetCurSel(screen_formatter_::LAYOUT_4x4);

    _sldShuttle[shuttle_::PLAY].SetRange(1, 23, TRUE);
    _sldShuttle[shuttle_::PLAY].SetPos(10);
    _sldShuttle[shuttle_::PLAY].SetTicFreq(1);
    _sldShuttle[shuttle_::PLAY].SetPageSize(1);

    _sldShuttle[shuttle_::FAST].SetRange(20, 82, TRUE);
    _sldShuttle[shuttle_::FAST].SetPos(20);
    _sldShuttle[shuttle_::FAST].SetTicFreq(1);
    _sldShuttle[shuttle_::FAST].SetPageSize(1);
    _sldShuttle[shuttle_::FAST].ShowWindow(SW_HIDE);

    _stcSpeed.SetWindowText(_T("x 1.0"));

    CRect rect;
    GetClientRect(&rect);
    rect.top = default_::playback_::HEIGHT;
    rect.DeflateRect(10, 10);

    _timeTable[control_::TIME_TABLE_HOUR]->create(this, rect, control_::TIME_TABLE_HOUR);
    _timeTable[control_::TIME_TABLE_MINUTE]->create(this, rect, control_::TIME_TABLE_MINUTE);
    _timeTable[control_::TIME_TABLE_MINUTE_IDR]->create(this, rect, control_::TIME_TABLE_MINUTE_IDR);
    _timeTable[control_::TIME_TABLE_HOUR]->set_listenr(this);
    _timeTable[control_::TIME_TABLE_MINUTE]->set_listenr(this);
    _timeTable[control_::TIME_TABLE_MINUTE_IDR]->set_listenr(this);

    _eventList->create(this, rect, control_::EVENT_LIST);
    _eventList->set_listenr(this);
    _eventList->show(false);
}

void controller_search::finalize(void)
{

}

void controller_search::screen_end_of_play(short hostId)
{
    set_play_speed(G2SEARCH_PLAYBACK::STOP, 0);
    enable_controls(G2SEARCH_PLAYBACK::STOP);
}

void controller_search::set_play_speed(int command, int clientSpeed)
{
    G2RETURN_IF_FAIL(client::valid_host_id(_playbackInfo._hostId));

    int speed = (command != G2SEARCH_PLAYBACK::STOP) ? clientSpeed : 0;

    screen_actuator& actuator = runner_ptr()->actuator_ref();
    actuator.set_search_play_speed(_playbackInfo._hostId, speed, speed != 10);
}

void controller_search::load_time_table(int channel)
{
    G2RETURN_IF_FAIL(client::valid_channel(channel));

    g2::scoped_criticalsection lock(_lock_data);
    search_data_ptr data = client::search_data_manager::get().find_search_data(search::SEARCH_LOCAL, channel);
    G2RETURN_IF_FAIL(data.get() != NULL);

    G2SPOT& spot = data->spot_standard();
    
    if (g2_spot_is_valid(&spot) != true) {
        _search->request_playback(channel, G2SEARCH_PLAYBACK::LAST, spot);
        return;
    }

    CTime time = g2_time_to_time32_t(&spot._time);
    TRACE("load time table [%S]\n", time.Format(_T("%Y-%m-%d %H:%M:%S")));
    
    G2SEARCH_TIMELAPSE::MODE timeMode = _search->get_timelapse_mode(channel);
    short hostId = _playbackInfo._hostId;
    screen_actuator& actuator = runner_ptr()->actuator_ref();
    unsigned __int64 visible_camera = actuator.visible_camera();
    
    std::set<int> cameras;
    for (int i = 0; i < MAX_SCREEN_CAMERA_COUNT; ++i) {
        if (g2::contains_bit(visible_camera, i) &&
            actuator.get_camera_mode(i) == client::screen::camera_::SEARCH &&
            hostId == client::find_host_id_from_camera(i) &&
            channel == client::find_channel_from_camera(i)) {
            cameras.insert(client::find_host_camera_from_screen_camera(i));
        }
    }

    int id = (timeMode == G2SEARCH_TIMELAPSE::HOUR)     ? control_::TIME_TABLE_HOUR : 
             (timeMode == G2SEARCH_TIMELAPSE::MINUTE)   ? control_::TIME_TABLE_MINUTE :
                                                          control_::TIME_TABLE_MINUTE_IDR ;
    _timeTable[id]->update(data, cameras);

    change_search_mode(_playbackInfo._mode);

    runner_ptr()->hide_screen_message();
    set_enable(true);
}

void controller_search::change_shuttle_mode(int mode)
{
    int id = -1, pos = 0;
    if (mode == shuttle_::PLAY) {
        _sldShuttle[shuttle_::PLAY].ShowWindow(SW_SHOW);
        _sldShuttle[shuttle_::FAST].ShowWindow(SW_HIDE);

        id = IDC_SLD_SEARCH_SHUTTLE;
    }
    else if (mode == shuttle_::FAST) {
        _sldShuttle[shuttle_::FAST].ShowWindow(SW_SHOW);
        _sldShuttle[shuttle_::PLAY].ShowWindow(SW_HIDE);

        id = IDC_SLD_SEARCH_FAST_SHUTTLE;
    }
    else {
        assert(!"unknown type");
    }

    if (id != -1) {
        change_slider_play_speed(id);
    }
}

void controller_search::change_search_mode(int mode)
{
    G2RETURN_IF_FAIL(_search->is_connected(_playbackInfo._channel));

    _eventList->set_host_id(_playbackInfo._hostId);

    CString string;
    if (mode == mode_::TIMELAPSE) {
        string = L"Event";

        _timeTable[control_::TIME_TABLE_HOUR]->show(false);
        _timeTable[control_::TIME_TABLE_MINUTE]->show(false);
        _timeTable[control_::TIME_TABLE_MINUTE_IDR]->show(false);

        if (_search->is_connected(_playbackInfo._channel)) {
            int mode = _search->get_timelapse_mode(_playbackInfo._channel);
            int id = (mode == G2SEARCH_TIMELAPSE::HOUR)     ? control_::TIME_TABLE_HOUR :
                     (mode == G2SEARCH_TIMELAPSE::MINUTE)   ? control_::TIME_TABLE_MINUTE :
                                                              control_::TIME_TABLE_MINUTE_IDR;

            _timeTable[id]->show(true);
            _eventList->show(false);
            _search->set_change_search_mode(_playbackInfo._channel, false);
        }
    }
    else {
        string = L"Time";

        _eventList->show(true);
        _timeTable[control_::TIME_TABLE_HOUR]->show(false);
        _timeTable[control_::TIME_TABLE_MINUTE]->show(false);
        _timeTable[control_::TIME_TABLE_MINUTE_IDR]->show(false);

        _search->set_change_search_mode(_playbackInfo._channel, true);
    }

    _buttons[buttons_::MODE].SetWindowText(string);
}

void controller_search::create_event_progress(short channel, const G2TIME& begin, const G2TIME& end)
{
    G2RETURN_IF_FAIL(valid_channel(channel));

    _progress.create(this);
    _progress.range(begin, end);
    _progress.channel(_search, channel);
    _progress.show(true);
}

void controller_search::enable_controls(int command)
{
    BOOL enable = TRUE;

    int id = control_::TIME_TABLE_MINUTE;
    if (_search->is_connected(_playbackInfo._channel)) {
        int mode = _search->get_timelapse_mode(_playbackInfo._channel);
        id = (mode == G2SEARCH_TIMELAPSE::HOUR)     ? control_::TIME_TABLE_HOUR :
             (mode == G2SEARCH_TIMELAPSE::MINUTE)   ? control_::TIME_TABLE_MINUTE :
                                                      control_::TIME_TABLE_MINUTE_IDR;
    }

    if (command == G2SEARCH_PLAYBACK::PLAY) {
        std::for_each(_buttons, _buttons + buttons_::END_PLAYBACK, boost::bind(&CButton::EnableWindow, _1, FALSE));
        _eventList->set_enable(false);
        _timeTable[id]->set_enable(false);
        _buttons[buttons_::STOP].EnableWindow(TRUE);
        enable = FALSE;
    }
    else if (command == G2SEARCH_PLAYBACK::STOP) {
        std::for_each(_buttons, _buttons + buttons_::END_PLAYBACK, boost::bind(&CButton::EnableWindow, _1, TRUE));
        _eventList->set_enable(true);
        _timeTable[id]->set_enable(true);
    }
    else if (command == G2SEARCH_PLAYBACK::REW) {
        std::for_each(_buttons, _buttons + buttons_::END_PLAYBACK, boost::bind(&CButton::EnableWindow, _1, FALSE));
        _eventList->set_enable(false);
        _timeTable[id]->set_enable(false);
        _buttons[buttons_::STOP].EnableWindow(TRUE);
        enable = FALSE;
    }
    else if (command == G2SEARCH_PLAYBACK::PREV) {
    }
    else if (command == G2SEARCH_PLAYBACK::NEXT) {
    }
    else if (command == G2SEARCH_PLAYBACK::FF) {
        std::for_each(_buttons, _buttons + buttons_::END_PLAYBACK, boost::bind(&CButton::EnableWindow, _1, FALSE));
        _eventList->set_enable(false);
        _timeTable[id]->set_enable(false);
        _buttons[buttons_::STOP].EnableWindow(TRUE);
        enable = FALSE;
    }
    else {
        std::for_each(_buttons, _buttons + buttons_::END_PLAYBACK, boost::bind(&CButton::EnableWindow, _1, TRUE));
        _eventList->set_enable(true);
        _timeTable[id]->set_enable(true);
    }
}

void controller_search::change_slider_play_speed(int id)
{
    screen_actuator& actuator = runner_ptr()->actuator_ref();
    
    const short channel = _playbackInfo._channel;
    const short hostId = _playbackInfo._hostId;

    int speed = _I32_MIN;
    bool ignoreSound = true;
    
    if (id == IDC_SLD_SEARCH_SHUTTLE) {
        _playbackInfo._playSpeed = _sldShuttle[shuttle_::PLAY].GetPos();
        if (_playbackInfo._playSpeed == 10) {
            ignoreSound = false;
        }
        speed = _playbackInfo._playSpeed;

        if (speed > 20) {
            speed = _I32_MAX;
            _playbackInfo._playSpeed = speed;
        }
    }
    else if (id == IDC_SLD_SEARCH_FAST_SHUTTLE) {
        _playbackInfo._fastSpeed = _sldShuttle[shuttle_::FAST].GetPos();
        speed = (_search->get_current_command(channel) == G2SEARCH_PLAYBACK::REW) ? -_playbackInfo._fastSpeed : _playbackInfo._fastSpeed;

        if (abs(speed) > 80) {
            speed = _I32_MAX;
            _playbackInfo._fastSpeed = speed;
        }
    }
    else {
        return;
    }

    CString string;
    if (speed == _I32_MAX) {
        string = _T("x ¡Ä");
    }
    else {
        string.Format(_T("x %.1f"), static_cast<float>(abs(speed)) / 10.0F);
    }
    _stcSpeed.SetWindowText(string);

    if (is_stopped() != true) {
        actuator.set_search_play_speed(hostId, speed, ignoreSound);
    }
}

void controller_search::stop_impl(void)
{
    const short channel = _playbackInfo._channel;
    const short hostId = _playbackInfo._hostId;

    G2RETURN_IF_FAIL(client::valid_host_id(hostId));
    G2RETURN_IF_FAIL(client::valid_channel(channel));

    if (_search->is_stopped(channel)) {
        enable_controls(G2SEARCH_PLAYBACK::STOP);
        return;
    }

    screen_actuator& actuator = runner_ptr()->actuator_ref();
    actuator.stop_search_enter(hostId);

    set_play_speed(G2SEARCH_PLAYBACK::STOP, 0);

    G2SPOT spot = client::find_last_spot_from_host_id(hostId);
    short hostcamera = client::find_host_camera_from_screen_camera(spot._segment);
    spot._segment = hostcamera;

    _search->request_playback(channel, G2SEARCH_PLAYBACK::STOP, spot);
    _playbackInfo._playSpeed = G2SEARCH_PLAYBACK::STOP;

    actuator.stop_search_leave(hostId);
}

//////////////////////////////////////////////////////////////////////////

void controller_search::on_btn_mode(void)
{
    int mode = _eventList->is_show() ? mode_::TIMELAPSE : mode_::EVENT;

    if (_playbackInfo._mode != mode) {
        _playbackInfo._mode = mode;
        change_search_mode(_playbackInfo._mode);
    }
}

void controller_search::on_btn_filter(void)
{
    const short hostId = _playbackInfo._hostId;
    const short channel = _playbackInfo._channel;
    G2RETURN_IF_FAIL(valid_channel(channel));
    G2RETURN_IF_FAIL(_search->is_connected(channel));

    stop_impl();
    
    G2EVENT_QUERY_CONDITION event;
    G2TEXT_IN_QUERY_CONDITION textin;
    unsigned int cameras;

    _search->get_query_condition_event(channel, event);
    _search->get_query_condition_text_in(channel, textin);
    _search->get_query_cameras(channel, cameras);
    G2SEARCH_QUERY::MODE mode = _search->get_query_mode(channel);
    G2SEARCH_TIMELAPSE::MODE timeMode = _search->get_timelapse_mode(channel);


    client::connective_host_list* host_list = runner_ptr()->get_host_list();
    std::wstring site = host_list->site_name_from_host_id(_playbackInfo._hostId);

    G2_PRODUCT_INFO pi;
    _search->get_product_info(channel, pi);

    search_condition_dlg dlg(runner_ptr());
    dlg.set_condition_event(event);
    dlg.set_condition_text_in(textin);
    dlg.set_query_mode(mode);
    dlg.set_query_cameras(cameras);
    dlg.set_device_info(site.c_str(), pi); 
    dlg.set_device_idr(timeMode == G2SEARCH_TIMELAPSE::MINUTE_IDR);
    
    if (dlg.DoModal() != IDOK ||
        _search->is_connected(channel) != true) {
        return;
    }
    _playbackInfo._event._hostId = hostId;
 
    _search->set_query_cameras(channel, dlg.get_query_cameras());
    
    if (dlg.get_query_mode() == G2SEARCH_QUERY::EVENT) {
        G2EVENT_QUERY_CONDITION condition;
        dlg.get_event_condition(condition);
        condition._begin = dlg.get_time_begin();
        condition._end = dlg.get_time_end();
        condition._reload = true;
        memset(&condition._fs_card_number, 0, sizeof(condition._fs_card_number));

        if (_search->is_drive_mode(channel, G2SEARCH_DRIVE::IDR)) {
            const G2TIME& date = dlg.get_time_begin_idr();

            G2TIME begin = { 0 }, end = { 0 };
            if (g2_time_is_valid(&condition._begin) != true) {
                CTime b(g2_time_to_time32_t(&date));
                g2_time_from_time32_t(&begin, (unsigned int)(CTime(b.GetYear(), b.GetMonth(), b.GetDay(), 0, 0, 0).GetTime()));
            }
            if (g2_time_is_valid(&condition._end) != true) {
                CTime e(g2_time_to_time32_t(&date));
                g2_time_from_time32_t(&end, (unsigned int)(CTime(e.GetYear(), e.GetMonth(), e.GetDay(), 23, 59, 59).GetTime()));
            }
            //create_event_progress(channel, begin, end);
        }

        _search->set_change_search_mode(channel, true);
        _search->set_query_mode(channel, G2SEARCH_QUERY::EVENT);
        _search->request_query_event(channel, condition);
    }
    else {
        G2TEXT_IN_QUERY_CONDITION condition;
        dlg.get_text_in_condition(condition);
        condition._begin = dlg.get_time_begin();
        condition._end = dlg.get_time_end();
        condition._reload = true;

        if (_search->is_drive_mode(channel, G2SEARCH_DRIVE::IDR)) {
            const G2TIME& date = dlg.get_time_begin_idr();

            G2TIME begin, end;
            if (g2_time_is_valid(&condition._begin) != true) {
                CTime b(g2_time_to_time32_t(&date));
                g2_time_from_time32_t(&begin, (unsigned int)(CTime(b.GetYear(), b.GetMonth(), b.GetDay(), 0, 0, 0).GetTime()));
            }
            if (g2_time_is_valid(&condition._end) != true) {
                CTime e(g2_time_to_time32_t(&date));
                g2_time_from_time32_t(&end, (unsigned int)(CTime(e.GetYear(), e.GetMonth(), e.GetDay(), 23, 59, 59).GetTime()));
            }
            create_event_progress(channel, begin, end);
        }

        _search->set_change_search_mode(channel, true);
        _search->set_query_mode(channel, G2SEARCH_QUERY::TEXT_IN);
        _search->request_query_text_in(channel, condition);
    }
}

void controller_search::on_btn_calendar(void)
{
    G2RETURN_IF_FAIL(is_enable());

    short channel = _playbackInfo._channel;
    G2RETURN_IF_FAIL(client::valid_channel(channel));

    calendar_dlg dlg(runner_ptr(), search::SEARCH_LOCAL);
    dlg.set_channel(channel);

    if (dlg.DoModal() == IDOK) {
        stop_impl();

        CTime date = dlg.get_selected_date();
        G2TIME time;
        g2_time_from_time32_t(&time, (unsigned long)date.GetTime());

        _search->request_record_time(channel, time);

        G2SEARCH_TIMELAPSE::MODE timeMode = _search->get_timelapse_mode(channel);
        if (timeMode != G2SEARCH_TIMELAPSE::MINUTE) {
            search_data_ptr data = client::search_data_manager::get().find_search_data(search::SEARCH_LOCAL, channel);
            G2RETURN_IF_FAIL(data.get() != NULL);

            G2SPOT spot;
            g2_spot_make_invalid(&spot);
            data->set_spot_standard(spot);
        }
    }
}

void controller_search::on_btn_go_to(void)
{
    G2RETURN_IF_FAIL(is_enable());

    CMenu menu, *popup = NULL;
    if (menu.LoadMenu(IDR_MENU_SEARCH) &&
       (popup = menu.GetSubMenu(play_menu_::CONTEXT_GOTO))) {
       // success loaded
    }
    G2RETURN_IF_FAIL(popup != NULL);

    CRect rect;
    ::GetWindowRect(_buttons[buttons_::GO_TO].GetSafeHwnd(), &rect);
    popup->TrackPopupMenu(TPM_LEFTALIGN | TPM_NOANIMATION, rect.left, rect.bottom, this);
}

void controller_search::on_btn_additional(void)
{
    G2RETURN_IF_FAIL(is_enable());

    CMenu menu, *popup = NULL;
    if (menu.LoadMenu(IDR_MENU_SEARCH) &&
       (popup = menu.GetSubMenu(play_menu_::CONTEXT_DATA_SOURCE))) {
       // success loaded
    }
    G2RETURN_IF_FAIL(popup != NULL);

    short channel = _playbackInfo._channel;

    if (_search->is_support(channel, G2SEARCH_SUPPORT::EXTERNAL_TANGO_SEARCH) != true) {
        popup->EnableMenuItem(ID_DATASOURCE_SEARCHONLOCAL, MF_BYCOMMAND | MF_GRAYED | MF_DISABLED);
    }
    if (_search->is_support(channel, G2SEARCH_SUPPORT::ARCHIVE) != true) {
        popup->EnableMenuItem(ID_DATASOURCE_SEARCHONARCHIVE, MF_BYCOMMAND | MF_GRAYED | MF_DISABLED);
    }

    bool enable = false;
    if (const search_data_ptr& data = search_data_manager::get().find_search_data(search::SEARCH_LOCAL, channel)) {
        if (data->count_segment() > 1) {
            enable = true;
        }
    }

    if (enable != true) {
        popup->EnableMenuItem(ID_ADDITIONAL_SELECTSEGMENT, MF_BYCOMMAND | MF_GRAYED | MF_DISABLED);
    }

    int check = ID_DATASOURCE_SEARCHONLOCAL;
    switch (_search->get_search_target(channel)) {
        case G2SEARCH_TARGET::HDD      : check = ID_DATASOURCE_SEARCHONLOCAL;   break;
        case G2SEARCH_TARGET::ARCHIVE  : check = ID_DATASOURCE_SEARCHONARCHIVE; break;
        case G2SEARCH_TARGET::EXTERNAL : check = ID_DATASOURCE_SEARCHONOTHER;   break;
        default: assert(false); break;
    }
    popup->CheckMenuItem(check, MF_BYCOMMAND | MF_CHECKED);

    if (is_stopped() != true) {
        popup->EnableMenuItem(ID_DATASOURCE_SEARCHONLOCAL,   MF_BYCOMMAND | MF_GRAYED | MF_DISABLED);
        popup->EnableMenuItem(ID_DATASOURCE_SEARCHONARCHIVE, MF_BYCOMMAND | MF_GRAYED | MF_DISABLED);
        popup->EnableMenuItem(ID_DATASOURCE_SEARCHONOTHER,   MF_BYCOMMAND | MF_GRAYED | MF_DISABLED);
    }

    CRect rect;
    ::GetWindowRect(_buttons[buttons_::ADDITIONAL].GetSafeHwnd(), &rect);
    popup->TrackPopupMenu(TPM_LEFTALIGN | TPM_NOANIMATION, rect.left, rect.bottom, this);
}

void controller_search::on_btn_clipcopy(void)
{
    stop_impl();

    clip_copy_dlg dlg(_playbackInfo._hostId, runner_ptr());
    dlg.DoModal();
}

void controller_search::on_btn_rew(void)
{
    G2RETURN_IF_FAIL(is_enable());

    const short channel = _playbackInfo._channel;
    const short hostId = _playbackInfo._hostId;

    G2RETURN_IF_FAIL(_search->is_stopped(channel));

    enable_controls(G2SEARCH_PLAYBACK::REW);
    change_shuttle_mode(shuttle_::FAST);
    set_play_speed(G2SEARCH_PLAYBACK::REW, -_playbackInfo._fastSpeed);

    G2SPOT spot;
    g2_spot_make_invalid(&spot);

    _search->request_playback(channel, G2SEARCH_PLAYBACK::REW, spot);
}

void controller_search::on_btn_prev(void)
{
    G2RETURN_IF_FAIL(is_enable());

    const short channel = _playbackInfo._channel;
    const short hostId = _playbackInfo._hostId;

    on_btn_stop();

    G2RETURN_IF_FAIL(_search->is_stopped(channel));

    enable_controls(G2SEARCH_PLAYBACK::PREV);

    G2SPOT spot;
    g2_spot_make_invalid(&spot);

    _search->request_playback(channel, G2SEARCH_PLAYBACK::PREV, spot);
}

void controller_search::on_btn_stop(void)
{
    stop_impl();
}

void controller_search::on_btn_play(void)
{
    G2RETURN_IF_FAIL(is_enable());
    G2RETURN_IF_FAIL(_search != NULL);

    const short channel = _playbackInfo._channel;
    const short hostId = _playbackInfo._hostId;

    G2RETURN_IF_FAIL(client::valid_channel(channel));
    G2RETURN_IF_FAIL(_search->is_stopped(channel));

    enable_controls(G2SEARCH_PLAYBACK::PLAY);
    change_shuttle_mode(shuttle_::PLAY);
    set_play_speed(G2SEARCH_PLAYBACK::PLAY, _playbackInfo._playSpeed);

    G2SPOT spot;
    g2_spot_make_invalid(&spot);

    _search->request_playback(channel, G2SEARCH_PLAYBACK::PLAY, spot);
}

void controller_search::on_btn_next(void)
{
    G2RETURN_IF_FAIL(is_enable());

    const short channel = _playbackInfo._channel;
    const short hostId = _playbackInfo._hostId;

    on_btn_stop();

    G2RETURN_IF_FAIL(_search->is_stopped(channel));

    enable_controls(G2SEARCH_PLAYBACK::NEXT);

    G2SPOT spot;
    g2_spot_make_invalid(&spot);

    _search->request_playback(channel, G2SEARCH_PLAYBACK::NEXT, spot);
}

void controller_search::on_btn_ff(void)
{
    G2RETURN_IF_FAIL(is_enable());

    const short channel = _playbackInfo._channel;
    const short hostId = _playbackInfo._hostId;
    G2RETURN_IF_FAIL(_search->is_stopped(channel));

    enable_controls(G2SEARCH_PLAYBACK::FF);
    change_shuttle_mode(shuttle_::FAST);
    set_play_speed(G2SEARCH_PLAYBACK::FF, _playbackInfo._fastSpeed);

    G2SPOT spot;
    g2_spot_make_invalid(&spot);

    _search->request_playback(channel, G2SEARCH_PLAYBACK::FF, spot);
}

void controller_search::on_cbx_selchange_format(void)
{
    const int cursel = _cbxFormat.GetCurSel();

    screen_actuator& actuator = runner_ptr()->actuator_ref();
    actuator.set_layout(screen_formatter_::LAYOUT(cursel));

    /////////////////////////////////////////////

    BOOL enable = (cursel == screen_formatter_::LAYOUT_6x6) ? FALSE : TRUE;

    _buttons[buttons_::LAYOUT_PREV].EnableWindow(enable);
    _buttons[buttons_::LAYOUT_NEXT].EnableWindow(enable);
}

void controller_search::on_btn_layout_prev(void)
{
    screen_actuator& actuator = runner_ptr()->actuator_ref();
    actuator.set_layout_page_prev();
}

void controller_search::on_btn_layout_next(void)
{
    screen_actuator& actuator = runner_ptr()->actuator_ref();
    actuator.set_layout_page_next();
}

//////////////////////////////////////////////////////////////////////////

void controller_search::on_goto_time(void)
{
    G2RETURN_IF_FAIL(is_enable());
    G2RETURN_IF_FAIL(_search != NULL);
    G2RETURN_IF_FAIL(client::valid_channel(_playbackInfo._channel));

    const short channel = _playbackInfo._channel;

    CTime current = CTime::GetCurrentTime();
    G2TIME time;
    g2_time_from_time32_t(&time, (unsigned long)current.GetTime());

    time_goto_dlg dlg(runner_ptr());
    dlg.set_support_date(true);
    dlg.set_current_time(time);
    if(dlg.DoModal() == IDOK) {
        stop_impl();
        time = dlg.current_time();

        G2RETURN_IF_FAIL(_search->is_connected(channel));
        G2SEARCH_TIMELAPSE::MODE timeMode = _search->get_timelapse_mode(channel);

        G2SPOT spot = { 0 };
        spot._time = time;        

        if (timeMode == G2SEARCH_TIMELAPSE::MINUTE) {
            _search->request_record_hour_command(channel, G2SEARCH_PLAYBACK::GOTO_SEC, spot);
        }
        else {
            _search->request_playback(channel, G2SEARCH_PLAYBACK::GOTO_SEC, spot);
        }
    }
}

void controller_search::on_goto_first(void)
{
    G2RETURN_IF_FAIL(is_enable());
    G2RETURN_IF_FAIL(client::valid_channel(_playbackInfo._channel));
    const short channel = _playbackInfo._channel;

    stop_impl();

    G2SPOT spot;
    g2_spot_make_invalid(&spot);

    _search->request_playback(channel, G2SEARCH_PLAYBACK::FIRST, spot);
}

void controller_search::on_goto_last(void)
{
    G2RETURN_IF_FAIL(is_enable());
    G2RETURN_IF_FAIL(client::valid_channel(_playbackInfo._channel));
    const short channel = _playbackInfo._channel;

    stop_impl();

    G2SPOT spot;
    g2_spot_make_invalid(&spot);

    _search->request_playback(channel, G2SEARCH_PLAYBACK::LAST, spot);
}

void controller_search::on_search_data_source(UINT nID)
{
    stop_impl();

    int selected = nID - ID_DATASOURCE_SEARCHONLOCAL;
    int channel = _playbackInfo._channel;
    if (_search->is_connected(channel)) {
        if(search_data_ptr& data = search_data_manager::get().find_search_data(search::SEARCH_LOCAL, channel)) {
            data->set_reserve_reload(true);
        }
        _search->set_search_target(channel, G2SEARCH_TARGET::TYPE(selected));
    }    
}

void controller_search::on_select_segment(void)
{
    short channel = _playbackInfo._channel;
    short hostId = _playbackInfo._hostId;

    G2RETURN_IF_FAIL(valid_channel(channel));

    stop_impl();

    int count = 0;
    search_data_ptr data;
    search::SEARCH_HOUR_INFO_DVR_LIST hours;
    if (search_data_manager::get().find_search_data(search::SEARCH_LOCAL, channel, data) &&
        data->get_hour_info_dvr(hours)) {
        count = static_cast<int>(hours.size());
    }

    G2RETURN_IF_FAIL(count > 0);

    select_segment_dlg dlg(runner_ptr());
    dlg.set_mode(select_segment_dlg::SELECT_MODE_ID);
    dlg.set_segment_count(count);
    dlg.set_segment_id(data->segment_id());

    if (dlg.DoModal() != IDOK) return;
    if (_search->is_connected(channel) != true) return;

    int id = dlg.selected();

    if (id != data->segment_id()) {
        if (search_data_ptr data = search_data_manager::get().find_search_data_search(channel)) {
            data->set_segment_id(id);
        }
    }
}

//////////////////////////////////////////////////////////////////////////

void controller_search::on_screen_layout_changed(int layout, int changed)
{
    G2RETURN_IF_FAIL(layout >= screen_formatter_::LAYOUT_1x1 &&
                     layout < screen_formatter_::LAYOUT_COUNT);
    G2RETURN_IF_FAIL(layout != _cbxFormat.GetCurSel());

    _cbxFormat.SetCurSel(layout);
}

void controller_search::on_screen_image_drew(short camera, const G2SPOT& spot)
{
    G2RETURN_IF_FAIL(client::valid_camera(camera));

    const short channel = _playbackInfo._channel;
    G2RETURN_IF_FAIL(client::valid_channel(channel));

    g2::scoped_criticalsection lock(_lock_data);
    search_data_ptr data = client::search_data_manager::get().find_search_data(search::SEARCH_LOCAL, channel);
    G2RETURN_IF_FAIL(data.get() != NULL);

    data->set_spot_selected(spot);

    int mode = _search->get_timelapse_mode(channel);
    int id = (mode == G2SEARCH_TIMELAPSE::HOUR)     ? control_::TIME_TABLE_HOUR :
             (mode == G2SEARCH_TIMELAPSE::MINUTE)   ? control_::TIME_TABLE_MINUTE : 
                                                      control_::TIME_TABLE_MINUTE_IDR;
    _timeTable[id]->screen_image_drew(spot);

    int command = _search->get_current_command(channel);

    if (client::is_same_date(CTime(g2_time_to_time32_t(&spot._time)),
        CTime(g2_time_to_time32_t(&data->spot_standard()._time))) != true) {
        G2SCOPE scope = { spot, spot };

        CTime time(g2_time_to_time32_t(&spot._time));
        g2_time_from_time32_t(&scope._begin._time, (unsigned long)CTime(time.GetYear(), time.GetMonth(), time.GetDay(), 0, 0, 0).GetTime());
        g2_time_from_time32_t(&scope._end._time, (unsigned long)CTime(time.GetYear(), time.GetMonth(), time.GetDay(), 23, 59, 59).GetTime());

        TRACE(L"play request record time info : %d-%d-%d\n", time.GetYear(), time.GetMonth(), time.GetDay());

        data->set_spot_standard(spot);

        if (_search->is_stopped(channel)) {
            data->clear_info_minute();
            if (mode != G2SEARCH_TIMELAPSE::MINUTE) {
                PostMessage(um_controller_::UM_LOAD_TIME_TABLE, WPARAM(channel));
            }
        }
        lock.free();

        if (mode == G2SEARCH_TIMELAPSE::MINUTE || mode == G2SEARCH_TIMELAPSE::HOUR) {
            runner_ptr()->show_screen_message(L"Loading record time information...", false);
            _search->request_record_hour(channel, scope._begin, search::MAX_HOUR_DATA, false);
            set_enable(false);
        }
    }

    if (command != G2SEARCH_PLAYBACK::PLAY &&
        command != G2SEARCH_PLAYBACK::REW &&
        command != G2SEARCH_PLAYBACK::FF) {
        enable_controls(G2SEARCH_PLAYBACK::STOP);   
    }

    return;
}

void controller_search::on_changed_select_time_table(const G2SPOT& spot)
{
    G2RETURN_IF_FAIL(g2_spot_is_valid(&spot));

    stop_impl();

    const short channel = _playbackInfo._channel;
    if (_search->is_connected(channel)) {
        int mode = _search->is_timelapse_mode(channel, G2SEARCH_TIMELAPSE::MINUTE_IDR) ? G2SEARCH_PLAYBACK::GOTO_SEC : 
                                                                                         G2SEARCH_PLAYBACK::GOTO_SPOT;
        _search->request_playback(channel, mode, spot);
    }
}

void controller_search::on_request_more_event_search(void)
{
    const short channel = _playbackInfo._channel;
    G2RETURN_IF_FAIL(_search->is_connected(channel));

    G2SEARCH_LOG_INFO log;
    bool successed = (_search->is_drive_mode(channel, G2SEARCH_DRIVE::IDR)) ?
                      _search->get_query_result_event(channel, 99, log) :
                      _search->get_query_result_event(channel, 0, log);

    G2RETURN_IF_ASSERT(successed == true);

    int queryMode = _search->get_query_mode(channel);

    if (queryMode == G2SEARCH_QUERY::EVENT) {
        G2EVENT_QUERY_CONDITION condition;
        _search->get_query_condition_event(channel, condition);

        if (_search->is_drive_mode(channel, G2SEARCH_DRIVE::IDR)) {
            create_event_progress(channel, condition._begin, condition._end);
        }
        else {
            condition._reload = false;
            condition._end = log._time;
            if (g2_time_is_valid(&condition._end) != true ||
               (g2_time_is_valid(&condition._end) && g2_time_is_valid(&condition._begin)) &&
                g2_time_compare(&condition._end, &condition._begin) < 0) {
                return;
            }
        }

        _search->request_query_event(channel, condition);
    }
    else if (queryMode == G2SEARCH_QUERY::TEXT_IN) {
        G2TEXT_IN_QUERY_CONDITION condition;
        _search->get_query_condition_text_in(channel, condition);

        if (_search->is_drive_mode(channel, G2SEARCH_DRIVE::IDR)) {
            create_event_progress(channel, condition._begin, condition._end);
        }
        else {
            condition._reload = false;
            condition._end = log._time;
        }

        _search->request_query_text_in(channel, condition);
    }
    else {
        assert(!"undefined event search query type");
    }

    runner_ptr()->show_screen_message(L"Search...", true, 10000);
    set_enable(false);
}

void controller_search::on_request_load_event_image_search(int selected, bool last)
{
    const short channel = _playbackInfo._channel;

    G2RETURN_IF_FAIL(_search->is_connected(channel));

    G2SEARCH_LOG_INFO log = { 0 };
    _search->get_query_result_event(channel, selected, log);

    if (log._cameras == 0) {
        runner_ptr()->show_screen_message(L"There is no recorded image.", true, 3000);
        return;
    }

    _search->request_event_image(channel, selected, last);
}

//////////////////////////////////////////////////////////////////////////

void controller_search::on_receive_no_frame_loaded(int channel) 
{
    G2RETURN_IF_FAIL(client::valid_channel(channel));

    int command = _search->get_current_command(channel);
    if (command == G2SEARCH_PLAYBACK::GOTO_SEC) {
        runner_ptr()->show_screen_message(L"There is no recorded image.", true, 3000);
    }
}

void controller_search::on_receive_no_recorded_data_of_search_target(int channel, G2SEARCH_TARGET::TYPE target) 
{
    G2RETURN_IF_FAIL(client::valid_channel(channel));

    runner_ptr()->show_screen_message(L"There is no recorded image.", true, 3000);
}

void controller_search::on_receive_finding_idr_event_time(int channel, const G2TIME& time) 
{
    G2RETURN_IF_FAIL(client::valid_channel(channel));
    G2RETURN_IF_FAIL(_playbackInfo._channel == channel);
}

void controller_search::on_receive_notify_play_speed_changed(int channel, int speed)
{
    G2RETURN_IF_FAIL(this != NULL && ::IsWindow(GetSafeHwnd()));
    G2RETURN_IF_FAIL(client::valid_channel(channel));
}
    
void controller_search::on_receive_notify_play_stop_post(int channel, G2SEARCH_DRIVE::MODE mode) 
{
    G2RETURN_IF_FAIL(this != NULL && ::IsWindow(GetSafeHwnd()));
    G2RETURN_IF_FAIL(client::valid_channel(channel));

    PostMessage(um_controller_::UM_POST_PLAY_STOPED, channel, mode);
}

void controller_search::on_receive_event_query_result(int channel, const std::vector<G2SEARCH_LOG_INFO>& info) 
{
    short hostId = _playbackInfo._event._hostId;
    _playbackInfo._event._hostId = client::invalid_::HOST_ID;
    //_playbackInfo._event._guid = g2guid();

    G2RETURN_IF_FAIL(client::valid_channel(channel));
    G2RETURN_IF_FAIL(_playbackInfo._channel == channel);

    if (info.empty()) {
        runner_ptr()->show_screen_message(L"No result.", true, 3000);
        set_enable(true);
        return;
    }

    if (_eventList.get() != NULL) {
        _eventList->insert(info);
    }

    runner_ptr()->hide_screen_message();

    if (_playbackInfo._mode != mode_::EVENT) {
        _playbackInfo._mode = mode_::EVENT;
        change_search_mode(_playbackInfo._mode);
    }
    set_enable(true);
}

void controller_search::on_receive_text_in_query_result(int channel, const std::vector<G2TEXT_IN>& data) 
{
    G2RETURN_IF_FAIL(client::valid_channel(channel));
    G2RETURN_IF_FAIL(_playbackInfo._channel == channel);

    if (data.empty()) {
        runner_ptr()->show_screen_message(L"No result.", true, 3000);
        set_enable(true);
        return;
    }

    if (_eventList.get() != NULL) {
        _eventList->insert(data);
    }

    runner_ptr()->hide_screen_message();

    if (_playbackInfo._mode != mode_::EVENT) {
        _playbackInfo._mode = mode_::EVENT;
        change_search_mode(_playbackInfo._mode);
    }
    set_enable(true);
}

void controller_search::on_recieve_recorded_data_scope(int channel, const std::vector<G2SCOPE>& scopes) 
{
    G2RETURN_IF_FAIL(client::valid_channel(channel));
    G2RETURN_IF_FAIL(_playbackInfo._channel == channel);

    if (scopes.size() == 1) {
        _search->request_playback(channel, G2SEARCH_PLAYBACK::GOTO_SPOT, scopes[0]._begin);
    }
    else if (scopes.size() > 1) {
        std::vector<G2SCOPE>* buffer = new std::vector<G2SCOPE>(scopes);
        PostMessage(um_controller_::UM_RECEIVE_SCOPE_LIST, WPARAM(channel), LPARAM(buffer));
    }
    else {
        assert(false);
    }
}

void controller_search::on_receive_segment_spot(int channel, const std::vector<G2SPOT>& spots) 
{
    G2RETURN_IF_FAIL(client::valid_channel(channel));
    G2RETURN_IF_FAIL(_playbackInfo._channel == channel);
    G2RETURN_IF_FAIL(spots.empty() != true);

    if (spots.size() == 1) {
        const G2SPOT& spot = spots[0];
        _search->request_playback(channel, G2SEARCH_PLAYBACK::GOTO_SPOT, spot);
    }
    else if (spots.size() > 1) {
        std::vector<G2SPOT>* buffer = new std::vector<G2SPOT>(spots);
        PostMessage(um_controller_::UM_RECEIVE_SPOT_LIST, WPARAM(channel), LPARAM(buffer));
    }
    else {
        assert(false);
    }
}

void controller_search::on_receive_external_tango_info(int channel, const std::vector<G2SEARCH_EXTERNAL_DISK>& info) 
{
    G2RETURN_IF_FAIL(client::valid_channel(channel));
    G2RETURN_IF_FAIL(_playbackInfo._channel == channel);

    if (info.empty()) {
        runner_ptr()->show_screen_message(L"There is no external storage.", true, 3000);
        if (search_data_ptr& data = search_data_manager::get().find_search_data(search::SEARCH_LOCAL, channel)) {
            data->set_reserve_reload(false);
        }
        return;
    }

    std::vector<G2SEARCH_EXTERNAL_DISK>* buffer = new std::vector<G2SEARCH_EXTERNAL_DISK>(info);
    PostMessage(um_controller_::UM_RECEIVE_EXTERNAL_TANGO_LIST, WPARAM(channel), LPARAM(buffer));
}

void controller_search::on_receive_prepare_reload(int channel) 
{
    G2RETURN_IF_FAIL(client::valid_channel(channel));
    G2RETURN_IF_FAIL(_playbackInfo._channel == channel);

    if (search_data_ptr& data = search_data_manager::get().find_search_data(search::SEARCH_LOCAL, channel)) {
        data->set_reserve_reload(false);
    }

    runner_ptr()->show_screen_message(L"There is no recorded image.", true, 3000);
}

//////////////////////////////////////////////////////////////////////////

LRESULT controller_search::on_post_screen_camera_changed(WPARAM wParam, LPARAM lParam)
{
    const short camera = static_cast<short>(wParam);
    G2RETURN_VAL_IF_FAIL(client::valid_camera(camera), 1L);

    const short prev_camera = _playbackInfo._selcamera;
    const short prev_channel = _playbackInfo._channel;
    const int prev_mode = _playbackInfo._mode;

    _playbackInfo._selcamera = camera;
    _playbackInfo._channel = client::find_channel_from_camera(camera);
    _playbackInfo._hostId = client::find_host_id_from_camera(camera);
    _playbackInfo._mode = (prev_channel != _playbackInfo._channel) ? mode_::TIMELAPSE : prev_mode;

    /////////////////////////////////////////////

    if (client::valid_channel(_playbackInfo._channel) == true) {
        if (_search->is_stopped(_playbackInfo._channel)) {
            enable_controls(G2SEARCH_PLAYBACK::STOP);
        }
        else {
            enable_controls(G2SEARCH_PLAYBACK::PLAY);
        }

        load_site(_playbackInfo._channel);
    }
    else {
        _playbackInfo.reset();

        enable_controls(G2SEARCH_PLAYBACK::STOP);
    }

    change_search_mode(_playbackInfo._mode);

    return 0L;
}

LRESULT controller_search::on_post_receive_scope_list(WPARAM wParam, LPARAM lParam)
{
    const int channel = static_cast<int>(wParam);
    std::auto_ptr<std::vector<G2SCOPE> > buffer((std::vector<G2SCOPE>*)lParam);

    G2RETURN_VAL_IF_ASSERT(client::valid_channel(channel), 1L);
    G2RETURN_VAL_IF_ASSERT(buffer.get() != NULL, 1L);

    std::vector<G2SCOPE> list(*buffer);
    buffer.reset();

    /////////////////////////////////////////////

    select_segment_dlg dlg(runner_ptr());
    dlg.set_mode(select_segment_dlg::SELECT_MODE_SCOPE);
    dlg.set_scope_info(list);

    if (dlg.DoModal() != IDOK) return 0L;
    if (_search->is_connected(channel) != true) return 0L;

    const G2SPOT& spot = dlg.selected_spot();

    _search->request_playback(channel, G2SEARCH_PLAYBACK::GOTO_SPOT, spot);

    return 0L;
}

LRESULT controller_search::on_post_receive_spot_list(WPARAM wParam, LPARAM lParam)
{
    const int channel = static_cast<int>(wParam);
    std::auto_ptr<std::vector<G2SPOT> > buffer((std::vector<G2SPOT>*)lParam);

    G2RETURN_VAL_IF_ASSERT(client::valid_channel(channel), 1L);
    G2RETURN_VAL_IF_ASSERT(buffer.get() != NULL, 1L);

    std::vector<G2SPOT> list(*buffer);
    buffer.reset();

    /////////////////////////////////////////////

    select_segment_dlg dlg(runner_ptr());
    dlg.set_mode(select_segment_dlg::SELECT_MODE_SPOT);
    dlg.set_spot_info(list);

    if (dlg.DoModal() != IDOK) return 0L;
    if (_search->is_connected(channel) != true) return 0L;

    const G2SPOT& spot = dlg.selected_spot();

    _search->request_playback(channel, G2SEARCH_PLAYBACK::GOTO_SPOT, spot);

    return 0L;
}

LRESULT controller_search::on_post_receive_external_tango_list(WPARAM wParam, LPARAM lParam)
{
    const int channel = static_cast<int>(wParam);
    std::auto_ptr<std::vector<G2SEARCH_EXTERNAL_DISK> > buffer((std::vector<G2SEARCH_EXTERNAL_DISK>*)lParam);

    G2RETURN_VAL_IF_ASSERT(client::valid_channel(channel), 1L);
    G2RETURN_VAL_IF_ASSERT(buffer.get() != NULL, 1L);

    std::vector<G2SEARCH_EXTERNAL_DISK> list(*buffer);
    buffer.reset();

    /////////////////////////////////////////////

    select_external_tango_dlg dlg(runner_ptr());
    dlg.set_external_tango_info(list);

    bool proceed = false;
    if (dlg.DoModal() == IDOK &&
        _search->is_connected(channel)) {
        int selected = dlg.selected();
        if (selected >= 0 &&
            selected < static_cast<int>(list.size())) {
            const G2SEARCH_EXTERNAL_DISK& element = list[selected];
            proceed = _search->set_external_tango(channel, element._disk_type, element._number);
        }
    }

    if (proceed != true) {
        if (search_data_ptr& data = search_data_manager::get().find_search_data(search::SEARCH_LOCAL, channel)) {
            data->set_reserve_reload(false);
        }
    }

    return 0L;
}

LRESULT controller_search::on_post_disconnected(WPARAM wParam, LPARAM lParam)
{
    const int channel = static_cast<int>(lParam);
    search_data_manager::get().remove_search_data(search::SEARCH_LOCAL, channel);

    _eventList->set_host_id(client::invalid_::HOST_ID);

    return 0L;
}

LRESULT controller_search::on_post_load_time_table(WPARAM wParam, LPARAM lParam)
{
    const int channel = static_cast<int>(wParam);
    load_time_table(channel);

    return 0L;
}

LRESULT controller_search::on_post_update_time_table(WPARAM wParam, LPARAM lParam)
{
    load_time_table(_playbackInfo._channel);

    return 0L;
}

//////////////////////////////////////////////////////////////////////////

void controller_search::load_site(int channel)
{
    G2RETURN_IF_FAIL(client::valid_channel(channel));

    PostMessage(um_controller_::UM_LOAD_TIME_TABLE, WPARAM(channel));
}

void controller_search::update_site(int channel)
{
    G2RETURN_IF_FAIL(client::valid_channel(channel));

    PostMessage(um_controller_::UM_UPDATE_TIME_TABLE, WPARAM(channel));
}

bool controller_search::is_stopped(void)
{
    const short channel = _playbackInfo._channel;
    return (client::valid_channel(channel)) ? _search->is_stopped(channel) : true;
}
