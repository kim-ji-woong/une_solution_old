// G2SearchControllerDlg.cpp : implementation file
//

#include "stdafx.h"
#include "G2SearchController.h"
#include "G2SearchControllerView.h"
#include "G2SearchControllerDlg.h"
#include "actuator/screen_actuator.h"
#include "common/client_functional.h"
#include "control/message_box.h"
#include "control/play/select_segment_dlg.h"
#include "control/play/calendar_dlg.h"
#include "control/play/time_table_minute.h"
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

enum  {
    BORDER_TOP    = 24,
    BORDER_LEFT   = 6,
    BORDER_RIGHT  = 6,
    BORDER_BOTTOM = 36,
    BORDER_MARGIN = 2,
};

#define MASK_COLOR              RGB(255,   0, 255)
#define BACK_COLOR              RGB(226, 226, 226)
#define DATE_BKGND_COLOR        RGB( 51,  51,  51)
#define DATE_TEXT_COLOR         RGB(187, 187, 187)
#define SPEED_TEXT_COLOR        RGB( 51,  51,  51)
#define RECONNECT_TEXT_COLOR    RGB(251, 251, 251)

//////////////////////////////////////////////////////////////////////////

G2SearchControllerDlg::G2SearchControllerDlg(CWnd* parent /*= NULL*/)
    : CDialog(IDD_SEARCHCONTROLLER_DIALOG, parent)
    , _ci()
    , _time_table(new time_table_minute)
    , _message(new message_box)
    , _move_step(move_::SEC_10)
    , _enable(false)
    , _is_play(false)
    , _is_top_most(true)
    , _use_start_pos(false)
    , _start_pos(-1, -1)
{
    CBitmap move_bmp[move_::COUNT];

    bool loading_dialog_control_image = _backgnd.LoadBitmap(_T("BMP_DLG_BKGND")) &&
                                        move_bmp[move_::FRAME_1].LoadBitmap(_T("BMP_BTN_MOVE_1F")) &&
                                        move_bmp[move_::SEC_10].LoadBitmap(_T("BMP_BTN_MOVE_10S")) &&
                                        move_bmp[move_::MIN_01].LoadBitmap(_T("BMP_BTN_MOVE_1M")) &&
                                        move_bmp[move_::MIN_05].LoadBitmap(_T("BMP_BTN_MOVE_5M")) &&
                                        move_bmp[move_::MIN_10].LoadBitmap(_T("BMP_BTN_MOVE_10M")) &&
                                        move_bmp[move_::MIN_15].LoadBitmap(_T("BMP_BTN_MOVE_15M")) &&
                                        move_bmp[move_::MIN_30].LoadBitmap(_T("BMP_BTN_MOVE_30M")) &&
                                        move_bmp[move_::HOUR_1].LoadBitmap(_T("BMP_BTN_MOVE_1H"));

    G2RETURN_IF_ASSERT(loading_dialog_control_image);
    
    for (int i = 0; i < move_::COUNT; ++i) {
        _move_image_list[i].Create(button_::MOVE_STEP_WIDTH, 
                                   button_::MOVE_STEP_HEIGHT,
                                   ILC_COLOR24,
                                   4, 4);
        _move_image_list[i].Add(&move_bmp[i], RGB(0,0,0));
    }


    for (int i = 0; i < move_::COUNT; ++i) {
        SAFE_DELETE_GDIOBJ(move_bmp[i]);
    }
}

G2SearchControllerDlg::~G2SearchControllerDlg(void)
{
    SAFE_DELETE_GDIOBJ(_backgnd);
    SAFE_DELETE_GDIOBJ(_font_data);
    SAFE_DELETE_GDIOBJ(_font_speed);
    SAFE_DELETE_GDIOBJ(_brush);
    SAFE_DELETE_GDIOBJ(_brush_bkgnd);
}

//////////////////////////////////////////////////////////////////////////

void G2SearchControllerDlg::DoDataExchange(CDataExchange* pDX)
{
    CDialog::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(G2SearchControllerDlg, CDialog)
    ON_WM_CREATE()
    ON_WM_DESTROY()
    ON_WM_HSCROLL()
    ON_WM_TIMER()
    ON_WM_ERASEBKGND()
    ON_WM_CTLCOLOR()
    ON_WM_SIZE()

    ON_BN_CLICKED(IDC_BTN_SEARCH_DLG_PREV,                                  on_btn_prev)
    ON_BN_CLICKED(IDC_BTN_SEARCH_DLG_MOVE_STEP,                             on_btn_move_step)
    ON_BN_CLICKED(IDC_BTN_SEARCH_DLG_NEXT,                                  on_btn_next)
    ON_BN_CLICKED(IDC_BTN_SEARCH_DLG_PLAY,                                  on_btn_play)
    ON_BN_CLICKED(IDC_BTN_SEARCH_DLG_STOP,                                  on_btn_stop)
    ON_BN_CLICKED(IDC_BTN_SEARCH_DLG_CALENDAR,                              on_btn_calendar)
    ON_NOTIFY(NM_RELEASEDCAPTURE, IDC_SLD_SEARCH_DLG_SPEED,                 on_sld_speed_release_capture)
    ON_COMMAND_RANGE(ID_SEARCH_G2_MOVE_1FRAME, ID_SEARCH_G2_MOVE_1HOUR,     on_move_step)

    ON_MESSAGE(um_controller_::UM_PLAY_GOTO_FIRST,                          on_post_goto_first)
    ON_MESSAGE(um_controller_::UM_PLAY_GOTO_LAST,                           on_post_goto_last)
    ON_MESSAGE(um_controller_::UM_RECEIVE_SCOPE_LIST,                       on_post_receive_scope_list)
    ON_MESSAGE(um_controller_::UM_RECEIVE_SPOT_LIST,                        on_post_receive_spot_list)
    ON_MESSAGE(um_controller_::UM_PLAY_SPEED_CHANGED,                       on_post_play_speed_changed)
    ON_MESSAGE(um_controller_::UM_CONNECTED,                                on_post_connected)
    ON_MESSAGE(um_controller_::UM_DISCONNECTED,                             on_post_disconnected)
    ON_MESSAGE(um_controller_::UM_LOAD_TIME_TABLE,                          on_post_load_time_table)
    ON_MESSAGE(um_controller_::UM_UPDATE_TIME_TABLE,                        on_post_update_time_table)
    ON_MESSAGE(um_controller_::UM_SLIDER_CHANGED,                           on_post_slider_changed)
	ON_WM_WINDOWPOSCHANGING()
END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////

int G2SearchControllerDlg::OnCreate(LPCREATESTRUCT lpcs)
{
    if (CDialog::OnCreate(lpcs) == -1) {
        return -1;
    }

    //////////////////////////////////////////////////////////////////////////

    client::runner* runner = runner_ptr();
    if (runner) {
        _adaptor = runner->search_g2_ptr();
    }
    assert(_adaptor.get());

    // button ////////////////////////////////////////////////////////////////

    CSize move_step_size(button_::MOVE_STEP_WIDTH, button_::MOVE_STEP_HEIGHT);
    CSize move_btn_size(button_::MOVE_BTN_WIDTH, button_::MOVE_BTN_HEIGHT);
    CSize playback_size(button_::PLAYSTOP_WIDTH, button_::PLAYSTOP_HEIGHT);
    CSize calendar_size(button_::CALENDAR_WIDTH, button_::CALENDAR_HEIGHT);

    CRect rct;
    GetClientRect(&rct);

    typedef struct { unsigned id; CString resource; CRect rect; CString tooltip; } BaseInfo;
    const BaseInfo arrBase[] = {
        { IDC_BTN_SEARCH_DLG_PREV,      _T("BMP_BTN_PREV"),     CRect(CPoint(button_::PREV_LEFT,     button_::TOP), move_btn_size),  _T("Prev") },
        { IDC_BTN_SEARCH_DLG_MOVE_STEP, _T("BMP_BTN_MOVE_10S"), CRect(CPoint(button_::MOVE_LEFT,     button_::TOP), move_step_size), _T("Move") },
        { IDC_BTN_SEARCH_DLG_NEXT,      _T("BMP_BTN_NEXT"),     CRect(CPoint(button_::NEXT_LEFT,     button_::TOP), move_btn_size),  _T("Next") },
        { IDC_BTN_SEARCH_DLG_PLAY,      _T("BMP_BTN_PLAY"),     CRect(CPoint(button_::PLAY_LEFT,     button_::TOP), playback_size),  _T("Play") },
        { IDC_BTN_SEARCH_DLG_STOP,      _T("BMP_BTN_STOP"),     CRect(CPoint(button_::STOP_LEFT,     button_::TOP), playback_size),  _T("Stop") },
        { IDC_BTN_SEARCH_DLG_CALENDAR,  _T("BMP_BTN_CALENDAR"), CRect(CPoint(button_::CALENDAR_LEFT, button_::TOP), calendar_size),  _T("Calendar") },
    };

    for (int i = 0; i < button_::COUNT; ++i) {
        bitmap_button::button_info info(arrBase[i].id,
                                        arrBase[i].resource,
                                        arrBase[i].rect,
                                        arrBase[i].tooltip,
                                        ILC_COLOR24,
                                        MASK_COLOR,
                                        BACK_COLOR);

        _btns[i].create(this, info);
    }

    // slider ctrl ///////////////////////////////////////////////////////////

    CRect sld_rect(CPoint(slider_::LEFT, slider_::TOP), CSize(slider_::WIDTH, slider_::HEIGHT));
    _sld_speed.Create(WS_CHILD|WS_VISIBLE|TBS_BOTH|TBS_NOTICKS, sld_rect, this, IDC_SLD_SEARCH_DLG_SPEED);
    _sld_speed.SetParent(this);
    _sld_speed.ModifyStyle(0, TBS_DOWNISLEFT);
    _sld_speed.SetDefaultCtrl(FALSE);
    _sld_speed.SetRange(search::speed_::BOUND_LOWER, search::speed_::BOUND_UPPER, TRUE);
    _sld_speed.SetPos(0);
    _sld_speed.SetTicFreq(1);
    _sld_speed.SetPageSize(1);
    
    // brush & font //////////////////////////////////////////////////////////

    _brush.CreateSolidBrush(DATE_BKGND_COLOR);
    _brush_bkgnd.CreatePatternBrush(&_backgnd);

    create_font(_font_data, text_::FONT_DATE_SIZE);
    create_font(_font_speed, text_::FONT_SPEED_SIZE);

    // static ////////////////////////////////////////////////////////////////

    CRect date_rect(CPoint(text_::DATE_LEFT, text_::DATE_TOP), CSize(text_::DATE_WIDTH, text_::DATE_HEIGHT));
    CRect speed_rect(CPoint(text_::SPEED_LEFT, text_::SPEED_TOP), CSize(text_::SPEED_WIDTH, text_::SPEED_HEIGHT));
    CRect init_rect(CPoint(text_::INIT_LEFT, text_::INIT_TOP), CSize(text_::INIT_WIDTH, text_::INIT_HEIGHT));
    DWORD text_style = WS_CHILD | WS_VISIBLE | SS_CENTER | SS_CENTERIMAGE;

    _stc_date.Create(L"", text_style, date_rect, this, IDC_STC_SEARCH_DLG_DATE);
    _stc_date.SetFont(&_font_data);

    _stc_speed.Create(L"", text_style, speed_rect, this, IDC_STC_SEARCH_DLG_SPEED);
    _stc_speed.SetFont(&_font_speed);

    _stc_init.Create(L"", text_style, init_rect, this, IDC_STC_SEARCH_DLG_INIT_MESSAGE);
    _stc_init.SetFont(&_font_data);

    return 0;
}

void G2SearchControllerDlg::OnDestroy()
{
    CDialog::OnDestroy();

    _adaptor.reset();
    _time_table.reset();
    _message.reset();

    finalize();
}

BOOL G2SearchControllerDlg::OnInitDialog()
{
    CDialog::OnInitDialog();

    initialize();
    on_move_step(ID_SEARCH_G2_MOVE_10SECOND);
    
    _stc_date.SetWindowText(L"");
    _stc_init.SetWindowText(L"Initialize...");

    _btns[button_::CALENDAR].set_text(L"Calendar");
    _btns[button_::CALENDAR].set_font(&_font_data);
    
    //////////////////////////////////////////////////////////////////////////
    
    enable_control(false);

    //////////////////////////////////////////////////////////////////////////

    const CWnd* pWndInsertAfter = _is_top_most ? &wndTopMost : &wndNoTopMost;
    int start_x = 0;
    int start_y = 0;
    UINT nFlags = SWP_NOMOVE;

    if (_use_start_pos) {
        MONITORINFOEX info;
        if (client::get_monitor_info_ex(_start_pos, info)) {
            CRect work_rect = info.rcWork;
            CSize size(G2SearchControllerDlg::const_::CONTROLLER_WIDTH,
                       G2SearchControllerDlg::const_::CONTROLLER_HEIGHT);

            CRect controller_rect(_start_pos, size);
            controller_rect = client::confine_to_dest_rect(controller_rect, work_rect);

            start_x = controller_rect.left;
            start_y = controller_rect.top;
            nFlags = NULL;
        }
    }

    ModifyStyle(0, WS_CLIPCHILDREN | WS_CLIPSIBLINGS);    
    SetWindowPos(pWndInsertAfter, start_x, start_y, const_::CONTROLLER_WIDTH, const_::CONTROLLER_HEIGHT, nFlags);
    SetWindowText(runner_ptr()->option()._title._string);

    return TRUE;
}

void G2SearchControllerDlg::OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar)
{
    CDialog::OnHScroll(nSBCode, nPos, pScrollBar);

    if (pScrollBar) {
        if (pScrollBar == (CScrollBar*)GetDlgItem(IDC_SLD_SEARCH_DLG_SPEED)) {
            request_speed_shuttle();
        }
    }
}

void G2SearchControllerDlg::OnTimer(UINT_PTR nIDEvent)
{
    CDialog::OnTimer(nIDEvent);

    if (nIDEvent == timer_::id_::BALANCE_AUTO) {
        if (client::valid_channel(_ci._channel)) {
            balance_time_table();
        }
    }
}

BOOL G2SearchControllerDlg::OnEraseBkgnd(CDC* pDC)
{
    CRect rctClient;
    GetClientRect(rctClient);

    CDC dc;
    dc.CreateCompatibleDC(pDC);
    CBitmap buffer;
    buffer.CreateCompatibleBitmap(pDC, rctClient.Width(), rctClient.Height());
    CBitmap *old = reinterpret_cast<CBitmap*>(dc.SelectObject(&buffer));

    CDC memdc;
    memdc.CreateCompatibleDC(pDC);

    CBitmap* oldbitmap = memdc.SelectObject(&_backgnd);
    dc.StretchBlt(0, 0, rctClient.Width(), rctClient.Height(), 
                  &memdc, 
                  0, 0, 16, 20,
                  SRCCOPY);
    
    // display ///////////////////////////////////////////////////////////////
    
    const int cx_client = rctClient.right;
    const int cy_client = rctClient.bottom;


    pDC->BitBlt(0, 0,
        cx_client, cy_client,
        &dc,
        0, 0,
        SRCCOPY);

    // restore ///////////////////////////////////////////////////////////////
    
    memdc.SelectObject(oldbitmap);
    memdc.DeleteDC();
    dc.SelectObject(buffer);
    dc.DeleteDC();

    return TRUE;
}

HBRUSH G2SearchControllerDlg::OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor)
{
    HBRUSH brush = CDialog::OnCtlColor(pDC, pWnd, nCtlColor);
    if (pWnd == &_stc_date) {
        pDC->SetTextColor(DATE_TEXT_COLOR);
        pDC->SetBkMode(TRANSPARENT);
        brush = (HBRUSH)_brush.GetSafeHandle();
    }
    else if (pWnd == &_stc_speed) {
        pDC->SetTextColor(SPEED_TEXT_COLOR);
        pDC->SetBkMode(TRANSPARENT);
        brush = (HBRUSH)_brush_bkgnd.GetSafeHandle();
    }
    else if (pWnd == &_stc_init) {
        pDC->SetTextColor(RECONNECT_TEXT_COLOR);
        pDC->SetBkMode(TRANSPARENT);
        brush = (HBRUSH)_brush_bkgnd.GetSafeHandle();
    }
    return brush;
}

void G2SearchControllerDlg::OnSize(UINT nType, int cx, int cy)
{
    CDialog::OnSize(nType, cx, cy);

    if (_time_table->is_created()) {
        CRect rect;
        GetClientRect(&rect);

        CRect tb_rect(CPoint(14, const_::PLAYBACK_HEIGHT),
                      CSize(704, rect.bottom - const_::PLAYBACK_HEIGHT - 3));

        _time_table->resize(tb_rect);
    }
}

//////////////////////////////////////////////////////////////////////////

client::message_box* G2SearchControllerDlg::message_box_ptr(void) 
{ 
    return (_message.get() != NULL && ::IsWindow(_message->GetSafeHwnd())) ? _message.get() : NULL; 
}

//////////////////////////////////////////////////////////////////////////

void G2SearchControllerDlg::initialize(void)
{
    CRect rect;
    GetClientRect(&rect);

    CRect tb_rect(CPoint(14, const_::PLAYBACK_HEIGHT),
                  CSize(704, rect.bottom - const_::PLAYBACK_HEIGHT - 3));

    _time_table->create(this, tb_rect, control_::TIME_TABLE);
    _time_table->set_listenr(this);

    _message->create(this);

    reset_speed();
}

void G2SearchControllerDlg::finalize(void)
{
    if (_message != NULL &&
        ::IsWindow(_message->GetSafeHwnd())) {
        _message->DestroyWindow();
    }
}

//////////////////////////////////////////////////////////////////////////

void G2SearchControllerDlg::stop_impl(void)
{
    int channel = _ci._channel;
    int camera = _ci._camera;

    G2RETURN_IF_FAIL(client::valid_channel(channel));
    
    if (_adaptor.get() && _adaptor->is_stopped(channel)) {
        return;
    }

    screen_actuator& actuator = runner_ptr()->actuator_ref();
    actuator.set_ignore_frame(true);
    actuator.stop_search_enter(channel);
    
    G2ROLLBACK_INFO rbi = { 0 };
    rbi._channelext = camera;
    rbi._spot = runner_ptr()->find_last_spot();

    if (_adaptor.get()) {
        _adaptor->request_pause(channel, true, rbi);
        _adaptor->set_play_control_command(channel, G2PLAYER::NONE);
    }

    _ci._playback._speed = G2PLAYER::NONE;

    set_play_speed(G2PLAYER::NONE);    
    
    actuator.stop_search_leave(channel);
    actuator.set_ignore_frame(false);
}

void G2SearchControllerDlg::enable_control(bool enable)
{
    if (_time_table.get()) 
        _time_table->ShowWindow(enable ? SW_SHOW : SW_HIDE);

    for (int i = 0; i < button_::COUNT; ++i) {
        bool show = enable;
        if (i == button_::PLAY) {
            show = enable && (_is_play == false);
        }
        else if (i == button_::STOP) {
            show = enable && _is_play;
        }
        button_show((button_::ID)i, show);
    }

    _sld_speed .ShowWindow(enable ? SW_SHOW : SW_HIDE);

    _stc_speed.ShowWindow(enable ? SW_SHOW : SW_HIDE);
    _stc_date.ShowWindow(enable ? SW_SHOW : SW_HIDE);
    _stc_init.ShowWindow(enable ? SW_HIDE : SW_SHOW);
}

void G2SearchControllerDlg::reset_speed(void)
{
    if (_sld_speed.GetSafeHwnd()) _sld_speed.SetPos(0);
    if (_stc_speed.GetSafeHwnd()) _stc_speed.SetWindowText(L"");
}

int G2SearchControllerDlg::set_play_speed(int command, int client_speed)
{
    int speed = 0;
    if (client::runner* runner = runner_ptr()) {
        speed = runner->set_play_speed(_ci._channel, command, client_speed);
    }
    return speed;
}

void G2SearchControllerDlg::request_speed_shuttle(void)
{
    int pos = _sld_speed.GetSafeHwnd() ? _sld_speed.GetPos() : 0;
    int cltspeed = (search::speed_::BOUND_LOWER <= pos &&
                    search::speed_::BOUND_UPPER >= pos) ? pos : search::speed_::PLAY_STOP;
    int svrspeed = G2PLAYER::NONE;

    switch (cltspeed) {
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
        case search::speed_::BACK_NORMAL_TRIPLE:
        case search::speed_::BACK_NORMAL_TWICE_HALF:
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
        case search::speed_::PLAY_NORMAL_TWICE_HALF:
        case search::speed_::PLAY_NORMAL_TRIPLE:
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

    int speed = set_play_speed(svrspeed, cltspeed);

    CString string;
    if (speed != 0) {
        if (speed == _I32_MAX) {
            string = (svrspeed > 0) ? _T("+∞x") : _T("-∞x");
        }
        else {
            string.Format(L"%+.1fx", static_cast<float>(speed) / 10.0F);
        }
    }
    if (_stc_speed.GetSafeHwnd())
        _stc_speed.SetWindowText(string);

    if (svrspeed == _ci._playback._speed) {
        return;
    }

    G2ROLLBACK_INFO rbi = { 0 };
    rbi._channelext = _ci._camera;
    rbi._spot = runner_ptr()->find_last_spot();

    _ci._playback._speed = svrspeed;
    _ci._playback._rbi = rbi;

    if (svrspeed == G2PLAYER::NONE) {
        stop_impl();
    }
    else if (_adaptor.get()) {
        _adaptor->request_play(_ci._channel, _ci._playback);
        _adaptor->set_play_control_command(_ci._channel, _ci._playback._speed);
    }
}

void G2SearchControllerDlg::load_time_table(int channel)
{
    G2RETURN_IF_FAIL(client::valid_channel(channel));

    g2::scoped_criticalsection lock(_cs_data);
    search_data_ptr data = client::search_data_manager::get().find_search_data(search::SEARCH_LOCAL_G2, channel);
    G2RETURN_IF_FAIL(data.get() != NULL);

    G2SPOT spot = data->spot_standard();

    CTime time(g2_time_to_time32_t(&spot._time));
    if (_stc_date.GetSafeHwnd()) _stc_date.SetWindowText(time.Format(_T("%Y-%m-%d %H:%M:%S")));

    std::set<int> cameras;
    if (_adaptor.get()) {
        _adaptor->get_camera_list_interest(channel, cameras);
    }

    if (_time_table.get() && cameras.empty() == false) {
        _time_table->update(data, cameras);
    }
}

bool G2SearchControllerDlg::balance_time_table(void)
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

    return _adaptor.get() ? _adaptor->request_record_time_info(channel, G2RECORD_TIME_INFO::MINUTE, search::direction_::DIRECTION_RIGHT, scope, reqcount, search::request_::REQUEST_OVERWRITE_RIGHT) : false;
}

bool G2SearchControllerDlg::button_show(button_::ID id, bool show)
{
    if (_btns[id]) {
        _btns[id].show(show);
        return true;
    }
    return false;
}

//////////////////////////////////////////////////////////////////////////

void G2SearchControllerDlg::on_screen_image_drew(const G2SPOT& spot, int channelext)
{
    int channel = _ci._channel;
    G2RETURN_IF_FAIL(client::valid_channel(channel));

    g2::scoped_criticalsection lock(_cs_data);
    search_data_ptr data = client::search_data_manager::get().find_search_data(search::SEARCH_LOCAL_G2, channel);
    G2RETURN_IF_FAIL(data.get() != NULL);

    CTime time(g2_time_to_time32_t(&spot._time));
    if (_stc_date.GetSafeHwnd()) _stc_date.SetWindowText(time.Format(_T("%Y-%m-%d %H:%M:%S")));

    if (_time_table.get()) {
        _time_table->screen_image_drew(spot);
    }

    data->set_spot_selected(spot);

    if (_adaptor.get() && _adaptor->get_play_control_command(channel) != G2PLAYER::MOVE_TO_SPOT) {
        if (client::is_same_date(CTime(g2_time_to_time32_t(&spot._time)), CTime(g2_time_to_time32_t(&data->spot_standard()._time))) == false ||
            spot._segment != data->spot_standard()._segment) {
            const search_minute_info* info = data->get_minute_info();
            bool request = true;

            search::SEARCH_MINUTE_INFO_LIST list;
            if (info && info->get_minute_info(channelext, list)) {
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
                if (_adaptor.get()) {
                    _adaptor->request_record_time_info_on_time(channel, G2RECORD_TIME_INFO::MINUTE, search::direction_::DIRECTION_RIGHT, scope._begin._time, scope._end._time, 512, search::request_::REQUEST_INIT, NULL);
                }
            }
        }
    }
    lock.free();

    return;
}

void G2SearchControllerDlg::on_changed_select_time_table(const G2SPOT& spot)
{
    G2RETURN_IF_FAIL(g2_spot_is_valid(&spot));

    reset_speed();

    int channel = _ci._channel;
    if (_adaptor.get() && _adaptor->is_connected(channel)) {
        _adaptor->request_move_to_spot(channel, spot, G2PLAYER::PRECISION::MINUTE, true);
        _adaptor->set_play_control_command(channel, G2PLAYER::MOVE_TO_SPOT);
    }
}

//////////////////////////////////////////////////////////////////////////

void G2SearchControllerDlg::on_receive_notify_command_begin_play(int channel, int command)
{

}

void G2SearchControllerDlg::on_receive_notify_command_end_play(int channel, int command)
{

}

void G2SearchControllerDlg::on_receive_notify_play_speed_changed_play(int channel, int speed)
{
    G2RETURN_IF_FAIL(this != NULL && ::IsWindow(GetSafeHwnd()));
    G2RETURN_IF_FAIL(client::valid_channel(channel));
    
    set_play_speed(speed);
    PostMessage(um_controller_::UM_PLAY_SPEED_CHANGED, channel);
}

void G2SearchControllerDlg::on_receive_response_scope_list(int channel, const std::vector<G2SCOPE>& scopes, int type)
{
    G2RETURN_IF_FAIL(client::valid_channel(channel));

    if (type == G2PLAY_SCOPE_TYPE::GOTO) {
        if (_adaptor.get() && scopes.size() == 1) {
            _adaptor->request_move_to_spot(channel, scopes[0]._begin, G2PLAYER::PRECISION::FRAME, true);
            _adaptor->set_play_control_command(channel, G2PLAYER::COMMAND_MOVE);
        }
        else if (scopes.size() > 1) {
            std::vector<G2SCOPE>* buf = new std::vector<G2SCOPE>(scopes);
            PostMessage(um_controller_::UM_RECEIVE_SCOPE_LIST, WPARAM(channel), LPARAM(buf));
        }
    }
}

void G2SearchControllerDlg::on_receive_response_spot_list(int channel, const std::vector<G2SPOT>& spots)
{
    G2RETURN_IF_FAIL(client::valid_channel(channel));

    if (spots.size() == 1 && _adaptor.get()) {
        _adaptor->request_move_to_spot(channel, spots[0], G2PLAYER::PRECISION::SECOND, true);
        _adaptor->set_play_control_command(channel, G2PLAYER::COMMAND_MOVE);
    }
    else if (spots.size() > 1) {
        std::vector<G2SPOT>* buf = new std::vector<G2SPOT>(spots);
        PostMessage(um_controller_::UM_RECEIVE_SPOT_LIST, WPARAM(channel), LPARAM(buf));
    }
}


//////////////////////////////////////////////////////////////////////////


void G2SearchControllerDlg::request_stop_sync(void)
{
    if (GetDlgItem(IDC_BTN_SEARCH_DLG_STOP)->IsWindowVisible()) {
        on_btn_stop();
    }
}

void G2SearchControllerDlg::request_first(void)
{
    PostMessage(um_controller_::UM_PLAY_GOTO_FIRST);
}

void G2SearchControllerDlg::request_last(void)
{
    PostMessage(um_controller_::UM_PLAY_GOTO_LAST);
}

void G2SearchControllerDlg::request_stop(void)
{
    HWND control = GetDlgItem(IDC_BTN_SEARCH_DLG_STOP)->GetSafeHwnd();
    if (control != NULL && ::IsWindow(control)) {
        PostMessage(WM_COMMAND, MAKEWPARAM(IDC_BTN_SEARCH_DLG_STOP, BN_CLICKED), LPARAM(GetDlgItem(IDC_BTN_SEARCH_DLG_STOP)->GetSafeHwnd()));
    }
}

void G2SearchControllerDlg::screen_end_of_play(int channel)
{
    reset_speed();
    set_play_speed(G2PLAYER::NONE, client::search::speed_::PLAY_STOP);
}

bool G2SearchControllerDlg::is_origin_pos_speed(void)
{
    return (_sld_speed.GetSafeHwnd()) ? (_sld_speed.GetPos() == 0) : false;
}

void G2SearchControllerDlg::load_site(int channel, int camera)
{
    G2RETURN_IF_FAIL(client::valid_channel(channel) && channel == _ci._channel);

    _ci._camera = camera;

    PostMessage(um_controller_::UM_LOAD_TIME_TABLE, WPARAM(channel));
}

void G2SearchControllerDlg::update_site(int channel)
{
    G2RETURN_IF_FAIL(client::valid_channel(channel) && channel == _ci._channel);

    PostMessage(um_controller_::UM_UPDATE_TIME_TABLE, WPARAM(channel));
}

void G2SearchControllerDlg::set_init_message(const std::wstring& message)
{
    if (_stc_init.GetSafeHwnd())
        _stc_init.SetWindowText(message.c_str());
}

void G2SearchControllerDlg::show_screen_message(const std::wstring& string, bool autohide /*= true*/, int elapse /*= 5000*/)
{
    G2RETURN_IF_FAIL(_message.get() != NULL);

    _message->hide();
    _message->set_string(string);
    _message->show(false, autohide, elapse);
}

void G2SearchControllerDlg::show_screen_message(const std::wstring& string, const std::wstring& message, bool autohide /*= true*/, int elapse /*= 5000*/)
{
    G2RETURN_IF_FAIL(_message.get() != NULL);

    _message->hide();
    _message->set_string(string);
    _message->set_message(message);
    _message->show(true, autohide, elapse);
}

void G2SearchControllerDlg::hide_screen_message(void)
{
    G2RETURN_IF_FAIL(_message.get() != NULL);

    _message->hide();
}

//////////////////////////////////////////////////////////////////////////

void G2SearchControllerDlg::on_btn_prev(void)
{
    G2RETURN_IF_FAIL(_enable);

    int channel = _ci._channel;

    on_btn_stop();

    if (_move_step == move_::FRAME_1 && _adaptor.get()) {
        _adaptor->request_prev_step(channel);
    }
    else {
        int factor_sec = 0;
        int factor_min = 0;
        switch (_move_step) {
            case move_::SEC_10: factor_sec = 10; break;
            case move_::MIN_01: factor_min =  1; break;
            case move_::MIN_05: factor_min =  5; break;
            case move_::MIN_10: factor_min = 10; break;
            case move_::MIN_15: factor_min = 15; break;
            case move_::MIN_30: factor_min = 30; break;
            case move_::HOUR_1: factor_min = 60; break;
        }

        G2SPOT spot = runner_ptr()->find_last_spot();
        CTime time = CTime(g2_time_to_time32_t(&spot._time)) - CTimeSpan(0, 0, factor_min, factor_sec);
        g2_time_from_time32_t(&spot._time, (unsigned long)time.GetTime());
        spot._tick = UINT_MAX;

        if (_adaptor.get())
            _adaptor->request_move_to_spot(channel, spot, G2PLAYER::PRECISION::SECOND, false);
    }

    if (_adaptor.get())
        _adaptor->set_play_control_command(channel, G2PLAYER::PREV_STEP);
}

void G2SearchControllerDlg::on_btn_move_step(void)
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
        select <= (unsigned int)ID_SEARCH_G2_MOVE_1HOUR) {
        popup->CheckMenuItem(select, MF_CHECKED | MF_BYCOMMAND);
    }

    CRect rect;
    ::GetWindowRect(GetDlgItem(IDC_BTN_SEARCH_DLG_MOVE_STEP)->GetSafeHwnd(), &rect);
    popup->TrackPopupMenu(TPM_LEFTALIGN | TPM_NOANIMATION, rect.left, rect.bottom, this);
}

void G2SearchControllerDlg::on_btn_next(void)
{
    G2RETURN_IF_FAIL(_enable);

    int channel = _ci._channel;

    on_btn_stop();

    if (_move_step == move_::FRAME_1 && _adaptor.get()) {
        _adaptor->request_next_step(channel);
    }
    else {
        int factor_sec = 0;
        int factor_min = 0;
        switch (_move_step) {
            case move_::SEC_10: factor_sec = 10; break;
            case move_::MIN_01: factor_min =  1; break;
            case move_::MIN_05: factor_min =  5; break;
            case move_::MIN_10: factor_min = 10; break;
            case move_::MIN_15: factor_min = 15; break;
            case move_::MIN_30: factor_min = 30; break;
            case move_::HOUR_1: factor_min = 60; break;
        }

        G2SPOT spot = runner_ptr()->find_last_spot();
        CTime time = CTime(g2_time_to_time32_t(&spot._time)) + CTimeSpan(0, 0, factor_min, factor_sec);
        g2_time_from_time32_t(&spot._time, (unsigned long)time.GetTime());
        spot._tick = 0;

        if (_adaptor.get())
            _adaptor->request_move_to_spot(channel, spot, G2PLAYER::PRECISION::SECOND, true);
    }

    if (_adaptor.get())
        _adaptor->set_play_control_command(channel, G2PLAYER::NEXT_STEP);
}

void G2SearchControllerDlg::on_btn_play(void)
{
    G2RETURN_IF_FAIL(_enable);
    G2RETURN_IF_FAIL(_is_play == false);

    _is_play = true;

    int channel = _ci._channel;
    int channelext = _ci._camera;

    G2RETURN_IF_FAIL(client::valid_channel(channel));
    G2RETURN_IF_FAIL(_adaptor.get() && _adaptor->is_stopped(channel));

    set_play_speed(G2PLAYER::PLAY_NORMAL, search::speed_::PLAY_NORMAL);

    G2ROLLBACK_INFO rbi = { 0 };
    rbi._spot = runner_ptr()->find_last_spot();
    rbi._channelext = channelext;

    G2PLAYBACK_COMMAND cmd;
    cmd._speed = G2PLAYER::PLAY_NORMAL;
    cmd._rbi = rbi;

    if (_adaptor.get()) {
        _adaptor->request_play(channel, cmd);
        _adaptor->set_play_control_command(channel, G2PLAYER::PLAY_NORMAL);
    }
}

void G2SearchControllerDlg::on_btn_stop(void)
{
    _is_play = false;

    stop_impl();
    reset_speed();
}

void G2SearchControllerDlg::on_btn_calendar(void)
{
    G2RETURN_IF_FAIL(_enable);

    int channel = _ci._channel;
    G2RETURN_IF_FAIL(client::valid_channel(channel));

    CRect rect;
    ::GetWindowRect(GetDlgItem(IDC_BTN_SEARCH_DLG_CALENDAR)->GetSafeHwnd(), &rect);
    
    calendar_dlg dlg(this, search::SEARCH_LOCAL_G2);
    dlg.set_channel(channel);
    dlg.set_pos(CPoint(rect.left, rect.bottom));

    if (dlg.DoModal() == IDOK) {
        stop_impl();

        CTime date = dlg.get_selected_date();
        G2TIME from, to;
        g2_time_from_time32_t(&from, (unsigned long)date.GetTime());
        g2_time_from_time32_t(&to, (unsigned long)CTime(date.GetYear(), date.GetMonth(), date.GetDay(), 23, 59, 59).GetTime());

        std::set<int> channels;
        if (g2_time_is_valid(&from) && 
            _adaptor.get() &&
            _adaptor->get_camera_list(channel, channels)) {
            _adaptor->request_scope_list(channel, from, to, channels, G2PLAY_SCOPE_TYPE::GOTO);
        }        
    }
}

void G2SearchControllerDlg::on_sld_speed_release_capture(NMHDR* pNMHDR, LRESULT* pResult)
{
    request_speed_shuttle();

    *pResult = 0;
}

void G2SearchControllerDlg::on_move_step(UINT nID)
{
    _move_step = nID - ID_SEARCH_G2_MOVE_1FRAME;

    bitmap_button& btn = _btns[button_::MOVE];
    if (btn.GetSafeHwnd()) {
        btn.set_image_list(_move_image_list[_move_step]);
    }
}

LRESULT G2SearchControllerDlg::on_post_goto_first(WPARAM wParam, LPARAM lParam)
{
    G2RETURN_VAL_IF_FAIL(_enable, 1L);
    G2RETURN_VAL_IF_FAIL(client::valid_channel(_ci._channel), 1L);
    int channel = _ci._channel;

    stop_impl();

    if (_adaptor.get()) {
        _adaptor->request_move_to_first(channel);
        _adaptor->set_play_control_command(channel, G2PLAYER::MOVE_TO_FIRST);
    }
    
    return 0L;
}

LRESULT G2SearchControllerDlg::on_post_goto_last(WPARAM wParam, LPARAM lParam)
{
    G2RETURN_VAL_IF_FAIL(_enable, 1L);
    G2RETURN_VAL_IF_FAIL(client::valid_channel(_ci._channel), 1L);
    int channel = _ci._channel;

    stop_impl();

    if (_adaptor.get()) {
        _adaptor->request_move_to_last(channel);
        _adaptor->set_play_control_command(channel, G2PLAYER::MOVE_TO_LAST);
    }

    return 0L;
}

LRESULT G2SearchControllerDlg::on_post_receive_scope_list(WPARAM wParam, LPARAM lParam)
{
    int channel = static_cast<int>(wParam);
    std::auto_ptr<std::vector<G2SCOPE> > buf((std::vector<G2SCOPE>*)lParam);

    G2RETURN_VAL_IF_ASSERT(client::valid_channel(channel), 1L);
    G2RETURN_VAL_IF_ASSERT(buf.get() != NULL, 1L);

    std::vector<G2SCOPE> list(*buf);
    buf.reset();

    /////////////////////////////////////////////

    select_segment_dlg dlg(this);
    dlg.set_mode(select_segment_dlg::SELECT_MODE_SCOPE);
    dlg.set_scope_info(list);

    if (dlg.DoModal() != IDOK) return 0L;

    if (_adaptor.get()) {
        _adaptor->request_move_to_spot(channel, dlg.selected_spot(), G2PLAYER::PRECISION::FRAME, true);
        _adaptor->set_play_control_command(channel, G2PLAYER::COMMAND_MOVE);
    }

    return 0L;
}

LRESULT G2SearchControllerDlg::on_post_receive_spot_list(WPARAM wParam, LPARAM lParam)
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

    if (_adaptor.get()) {
        _adaptor->request_move_to_spot(channel, dlg.selected_spot(), G2PLAYER::PRECISION::SECOND, true);
        _adaptor->set_play_control_command(channel, G2PLAYER::COMMAND_MOVE);
    }

    return 0L;
}

LRESULT G2SearchControllerDlg::on_post_play_speed_changed(WPARAM wParam, LPARAM lParam)
{
    int channel = _ci._channel;

    G2RETURN_VAL_IF_FAIL(client::valid_channel(channel), 1L);
    G2RETURN_VAL_IF_FAIL(_adaptor.get() != NULL, 1L);

    _is_play = _adaptor->is_stopped(channel) ? false : true;

    button_::ID enable_button;
    button_::ID disable_button;

    if (_is_play) {
        enable_button = button_::STOP;
        disable_button = button_::PLAY;
    }
    else {
        enable_button = button_::PLAY;
        disable_button = button_::STOP;

        std::set<int> channels;
        if (_adaptor.get() && _adaptor->get_camera_list(channel, channels) &&
            channels.empty()) {
            reset_speed();
        }
    }

    button_show(enable_button, true);
    button_show(disable_button, false);

    return 0L;
}

LRESULT G2SearchControllerDlg::on_post_connected(WPARAM wParam, LPARAM lParam)
{
    int channel = static_cast<int>(lParam);
    _ci._channel = channel;

    enable_control(true);

	theApp.WriteProcessID();
    return 0L;
}

LRESULT G2SearchControllerDlg::on_post_disconnected(WPARAM wParam, LPARAM lParam)
{
    int channel = static_cast<int>(lParam);
    search_data_manager::get().remove_search_data(search::SEARCH_LOCAL_G2, channel);

    enable_control(false);

    return 0L;
}

LRESULT G2SearchControllerDlg::on_post_load_time_table(WPARAM wParam, LPARAM lParam)
{
    int channel = static_cast<int>(wParam);
    load_time_table(channel);

    return 0L;
}

LRESULT G2SearchControllerDlg::on_post_update_time_table(WPARAM wParam, LPARAM lParam)
{
    int channel = static_cast<int>(wParam);
    load_time_table(channel);

    return 0L;
}

LRESULT G2SearchControllerDlg::on_post_slider_changed(WPARAM wParam, LPARAM lParam)
{
    request_speed_shuttle();

    return 0L;
}



void client::G2SearchControllerDlg::OnWindowPosChanging(WINDOWPOS* lpwndpos)
{
	__super::OnWindowPosChanging(lpwndpos);

	// TODO: 여기에 메시지 처리기 코드를 추가합니다.
	// Dialog가 화면에 나타나지 않게 한다.
	lpwndpos->flags &= ~SWP_SHOWWINDOW;
}
