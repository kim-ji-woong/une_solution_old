// controller_watch.cpp : implementation
//

#include "stdafx.h"
#include "G2Client.h"
#include "MainFrm.h"
#include "G2ClientView.h"
#include "controller_watch.h"

#include <control/ptz/ptz_preset_dlg.h>
#include <control/ptz/ptz_advanced_dlg.h>
#include <sampler/cpp/g2client_watch.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

//////////////////////////////////////////////////////////////////////////

using namespace client;

//////////////////////////////////////////////////////////////////////////

enum { IDD = IDD_CONTROLLER_WATCH };

//////////////////////////////////////////////////////////////////////////

controller_watch::controller_watch(void)
    : _watch(NULL)
    , _selcamera(invalid_::CAMERA_NUMBER)
    , _channel(invalid_::CHANNEL)
    , _hostId(invalid_::HOST_ID)
    , _channelext(invalid_::CHANNEL_EXT)
    , _ptzMenuOn(false)
    , _presetMode(ptz_preset_dlg::MODE_UNDEFIND)
{

}

controller_watch::~controller_watch(void)
{

}

//////////////////////////////////////////////////////////////////////////

void controller_watch::DoDataExchange(CDataExchange* pDX)
{
    CDialog::DoDataExchange(pDX);

    DDX_Control(pDX, IDC_CBX_SCREEN_FORMAT,         _cbxFormat);
    DDX_Control(pDX, IDC_CBX_COLOR_EFFECT_CMD,      _cbxColorEffect);
    DDX_Control(pDX, IDC_CBX_ALARMOUT,              _cbxAlarmout);
}

//////////////////////////////////////////////////////////////////////////

BEGIN_MESSAGE_MAP(controller_watch, CDialog)
    ON_WM_CREATE()
    ON_WM_DESTROY()
    ON_WM_SIZE()
    ON_WM_ERASEBKGND()

    ON_CBN_SELCHANGE(IDC_CBX_SCREEN_FORMAT,                             on_cbx_selchange_format)
    ON_BN_CLICKED(IDC_BTN_LAYOUT_PREV,                                  on_btn_prev_camera)
    ON_BN_CLICKED(IDC_BTN_LAYOUT_NEXT,                                  on_btn_next_camera)
    ON_BN_CLICKED(IDC_BTN_PTZ_ADVANCED,                                 on_btn_ptz_advanced)

    ON_COMMAND_RANGE(IDC_BTN_PTZ_SET_PRESET, IDC_BTN_PTZ_VIEW_PRESET,   on_btn_ptz_preset)
    ON_COMMAND_RANGE(IDC_BTN_PTZ_N, IDC_BTN_PTZ_MENU,                   on_camera_ptz_direction)
    ON_COMMAND_RANGE(IDC_BTN_PTZ_ZOOM_IN, IDC_BTN_PTZ_IRIS_CLOSE,       on_camera_ptz_control)

    ON_CBN_SELCHANGE(IDC_CBX_COLOR_EFFECT_CMD,                          on_cbx_selchange_color_effect_cmd)
    ON_BN_CLICKED(IDC_BTN_COLOR_EFFECT_UP,                              on_btn_color_effect_up)
    ON_BN_CLICKED(IDC_BTN_COLOR_EFFECT_DOWN,                            on_btn_color_effect_down)

    ON_CBN_SELCHANGE(IDC_CBX_ALARMOUT,                                  on_cbx_selchange_alarmout)
    ON_BN_CLICKED(IDC_BTN_ALARMOUT_ON,                                  on_btn_alarmout_on)
    ON_BN_CLICKED(IDC_BTN_ALARMOUT_OFF,                                 on_btn_alarmout_off)

    ON_MESSAGE(um_controller_::UM_SCREEN_CAMERA_CHANGED,                on_post_screen_camera_changed)
    ON_MESSAGE(um_controller_::UM_PTZ_RECEVIE_MENU,                     on_post_receive_ptz_menu)
    ON_MESSAGE(um_controller_::UM_PTZ_RECEVIE_PRESET,                   on_post_receive_ptz_preset)
END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////

bool controller_watch::create(CWnd* parent)
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

BOOL controller_watch::PreTranslateMessage(MSG* pMsg)
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

int controller_watch::OnCreate(LPCREATESTRUCT lpcs)
{
    if (CDialog::OnCreate(lpcs) == -1) {
        return -1;
    }

    client::runner* runner = runner_ptr();
    if (runner) {
        _watch = &(runner->watch_ref());
    }
    assert(_watch);

    return 0;
}

BOOL controller_watch::OnInitDialog()
{
    CDialog::OnInitDialog();

    initialize();

    return TRUE;
}

void controller_watch::OnDestroy()
{
    CDialog::OnDestroy();
}

void controller_watch::OnSize(unsigned int nType, int cx, int cy)
{
    CDialog::OnSize(nType, cx, cy);
}

BOOL controller_watch::OnEraseBkgnd(CDC* pDC)
{
    return CDialog::OnEraseBkgnd(pDC);
}

//////////////////////////////////////////////////////////////////////////

void controller_watch::initialize(void)
{
    CString string;
    for (int i = 1; i < 7; ++i) {
        string.Format(_T("Layout %dx%d"), i, i);
        _cbxFormat.AddString(string);
    }
    _cbxFormat.SetCurSel(screen_formatter_::LAYOUT_4x4);
  
    _cbxColorEffect.AddString(_T("REVERT"));
    _cbxColorEffect.AddString(_T("BRIGHT"));
    _cbxColorEffect.AddString(_T("CONTRAST"));
    _cbxColorEffect.AddString(_T("HUE"));
    _cbxColorEffect.AddString(_T("SATURATION"));
    _cbxColorEffect.SetCurSel(0);

    enable_ptz_control(G2LIVE_CAMERA_STATUS::PTZ_NONE);
    enable_image_control(FALSE);
    enable_alarmout_control(FALSE);
}

void controller_watch::finalize(void)
{

}

void controller_watch::enable_ptz_control(char modePTZ)
{
    if (modePTZ == G2LIVE_CAMERA_STATUS::PTZ_NORMAL ||
        modePTZ == G2LIVE_CAMERA_STATUS::PTZ_ADVANCED) {
        for (int i = IDC_STC_ZOOM; i <= IDC_BTN_PTZ_VIEW_PRESET; ++i) {
            GetDlgItem(i)->EnableWindow(TRUE);
        }

        if (modePTZ == G2LIVE_CAMERA_STATUS::PTZ_ADVANCED) {
            GetDlgItem(IDC_BTN_PTZ_ADVANCED)->EnableWindow(TRUE);
        }
    }
    else {
        for (int i = IDC_STC_ZOOM; i <= IDC_BTN_PTZ_ADVANCED; ++i) {
            GetDlgItem(i)->EnableWindow(FALSE);
        }
    }
}

void controller_watch::enable_image_control(BOOL enabled)
{
    int end = IDC_BTN_COLOR_EFFECT_DOWN;

    if (enabled && _cbxColorEffect.GetCurSel() == 0) {
        end = IDC_CBX_COLOR_EFFECT_CMD;
    }

    for (int i = IDC_STC_COLOR_EFFECT; i <= end; ++i) {
        GetDlgItem(i)->EnableWindow(enabled);
    }
}

void controller_watch::enable_alarmout_control(BOOL enabled)
{
    int alarmout_count = 0;

    _cbxAlarmout.ResetContent();

    if (enabled) {
        G2_PRODUCT_INFO info;
        if (_watch->get_product_info(_channel, info)) {
            if (info.basic_info.hybrid) {
                alarmout_count = info.device_hybird.count_alarm_out;
            }
            else {
                alarmout_count = info.device.count_alarm_out;
            }

            if (alarmout_count > 0) {
                _cbxAlarmout.AddString(L"");
                for (int i = 0; i < alarmout_count; ++i) {
                    CString content;
                    content.Format(L"%d", i);
                    _cbxAlarmout.AddString(content.GetBuffer());
                }
                _cbxAlarmout.SetCurSel(0);
            }
        }
    }

    int end = IDC_BTN_ALARMOUT_OFF;
    if (enabled && alarmout_count > 0) {
        end = IDC_CBX_ALARMOUT;
    }

    for (int i = IDC_STC_ALARMOUT; i <= end; ++i) {
        GetDlgItem(i)->EnableWindow(enabled);
    }
}

void controller_watch::receive_ptz_menu(short channel, const G2LIVE_PTZ_MENU& menu)
{
    G2LIVE_PTZ_MENU* ptzmenu = new G2LIVE_PTZ_MENU;
    *ptzmenu = menu;

    PostMessage(um_controller_::UM_PTZ_RECEVIE_MENU, WPARAM(channel), LPARAM(ptzmenu));
}

void controller_watch::receive_ptz_preset(short channel, const G2LIVE_PTZ_PRESET& preset)
{
    G2LIVE_PTZ_PRESET* ptzpreset = new G2LIVE_PTZ_PRESET;
    *ptzpreset = preset;

    PostMessage(um_controller_::UM_PTZ_RECEVIE_PRESET, WPARAM(channel), LPARAM(ptzpreset));
}

//////////////////////////////////////////////////////////////////////////

void controller_watch::on_screen_layout_changed(int layout, int changed)
{
    G2RETURN_IF_FAIL(layout >= screen_formatter_::LAYOUT_1x1 &&
                     layout < screen_formatter_::LAYOUT_COUNT);
    G2RETURN_IF_FAIL(layout != _cbxFormat.GetCurSel());

    _cbxFormat.SetCurSel(layout);
}

//////////////////////////////////////////////////////////////////////////

void controller_watch::on_cbx_selchange_format(void)
{
    const int cursel = _cbxFormat.GetCurSel();

    screen_actuator& actuator = runner_ptr()->actuator_ref();
    actuator.set_layout(screen_formatter_::LAYOUT(cursel));

    /////////////////////////////////////////////

    BOOL enable = (cursel == screen_formatter_::LAYOUT_6x6) ? FALSE : TRUE;
    if (CButton* btnPrev = (CButton*)GetDlgItem(IDC_BTN_LAYOUT_PREV)) {
        btnPrev->EnableWindow(enable);
    }
    if (CButton* btnNext = (CButton*)GetDlgItem(IDC_BTN_LAYOUT_NEXT)) {
        btnNext->EnableWindow(enable);
    }
}

void controller_watch::on_btn_prev_camera(void)
{
    screen_actuator& actuator = runner_ptr()->actuator_ref();
    actuator.set_layout_page_prev();
}

void controller_watch::on_btn_next_camera(void)
{
    screen_actuator& actuator = runner_ptr()->actuator_ref();
    actuator.set_layout_page_next();
}

void controller_watch::on_btn_ptz_preset(UINT nID)
{
    G2RETURN_IF_FAIL(_presetMode == ptz_preset_dlg::MODE_UNDEFIND);

    if (_watch->is_connected(_channel)) {
        _presetMode = nID - IDC_BTN_PTZ_SET_PRESET;
        _watch->request_ptz_preset(_channel, _selcamera);
    }
}

void controller_watch::on_btn_ptz_advanced(void)
{
    if (_watch->is_connected(_channel)) {
        _watch->request_ptz_menu(_channel, _selcamera);
    }
}

void controller_watch::on_camera_ptz_direction(UINT nID)
{
    G2LIVE_PTZ_COMMAND command = { 0 };
    
    bool cmdMenu = false;
    if (nID == IDC_BTN_PTZ_MENU) {
        cmdMenu = true;
        _ptzMenuOn = !_ptzMenuOn;

        command._command = (_ptzMenuOn) ? G2LIVE_PTZ_COMMAND::PTZ_MENU_ON : G2LIVE_PTZ_COMMAND::PTZ_MENU_OFF;

        if (_watch->is_connected(_channel)) {
            _watch->set_ptz_command(_channel, _selcamera, command);
        }
        return;
    }

    int cmd = nID - IDC_BTN_PTZ_N;
    if (G2LIVE_PTZ_COMMAND::PTZ_MOVE_N <= cmd &&
        cmd <= G2LIVE_PTZ_COMMAND::PTZ_MOVE_NW) {
        command._command = cmd;

        if (_watch->is_connected(_channel)) {
            _watch->set_ptz_command(_channel, _selcamera, command);
        }
    }
}

void controller_watch::on_camera_ptz_control(UINT nID)
{
    G2LIVE_PTZ_COMMAND command = { 0 };

    int cmd = -1;
    switch (nID) {
        case IDC_BTN_PTZ_ZOOM_IN:
            cmd = G2LIVE_PTZ_COMMAND::PTZ_ZOOM_IN;
            break;
        case IDC_BTN_PTZ_ZOOM_OUT:
            cmd = G2LIVE_PTZ_COMMAND::PTZ_ZOOM_OUT;
            break;
        case IDC_BTN_PTZ_FOCUS_FAR:
            cmd = G2LIVE_PTZ_COMMAND::PTZ_FOCUS_FAR;
            break;
        case IDC_BTN_PTZ_FOCUS_NEAR:
            cmd = G2LIVE_PTZ_COMMAND::PTZ_FOCUS_NEAR;
            break;
        case IDC_BTN_PTZ_IRIS_OPEN:
            cmd = G2LIVE_PTZ_COMMAND::PTZ_IRIS_OPEN;
            break;
        case IDC_BTN_PTZ_IRIS_CLOSE:
            cmd = G2LIVE_PTZ_COMMAND::PTZ_IRIS_CLOSE;
            break;
    }

    if (cmd != -1) {
        command._command = cmd;
        if (_watch->is_connected(_channel)) {
            _watch->set_ptz_command(_channel, _selcamera, command);
        }
    }
}

void controller_watch::on_cbx_selchange_color_effect_cmd(void)
{
    BOOL enable = FALSE;

    if (_cbxColorEffect.GetCurSel() == 0) {
        _watch->set_camera_color(_channel, _selcamera, G2LIVE_CAMERA_COLOR::REVERT, G2LIVE_CAMERA_COLOR::DEFAULT);
    }
    else {
        enable = TRUE;
    }

    if (CButton* btnUp = (CButton*)GetDlgItem(IDC_BTN_COLOR_EFFECT_UP)) {
        btnUp->EnableWindow(enable);
    }
    if (CButton* btnDown = (CButton*)GetDlgItem(IDC_BTN_COLOR_EFFECT_DOWN)) {
        btnDown->EnableWindow(enable);
    }
}

void controller_watch::on_btn_color_effect_up(void)
{
    int type;

    switch (_cbxColorEffect.GetCurSel()) {
    case 1: type = G2LIVE_CAMERA_COLOR::BRIGHTNESS; break;
    case 2: type = G2LIVE_CAMERA_COLOR::CONTRAST;   break;
    case 3: type = G2LIVE_CAMERA_COLOR::HUE;        break;
    case 4: type = G2LIVE_CAMERA_COLOR::SATURATION; break;
    default: return;
    }

    _watch->set_camera_color(_channel, _selcamera, type, G2LIVE_CAMERA_COLOR::UP);
}

void controller_watch::on_btn_color_effect_down(void)
{
    int type;

    switch (_cbxColorEffect.GetCurSel()) {
    case 1: type = G2LIVE_CAMERA_COLOR::BRIGHTNESS; break;
    case 2: type = G2LIVE_CAMERA_COLOR::CONTRAST;   break;
    case 3: type = G2LIVE_CAMERA_COLOR::HUE;        break;
    case 4: type = G2LIVE_CAMERA_COLOR::SATURATION; break;
    default: return;
    }

    _watch->set_camera_color(_channel, _selcamera, type, G2LIVE_CAMERA_COLOR::DOWN);
}

void controller_watch::on_cbx_selchange_alarmout(void)
{
    BOOL enable = (_cbxAlarmout.GetCurSel() == 0) ? FALSE : TRUE;

    if (CButton* btnOn = (CButton*)GetDlgItem(IDC_BTN_ALARMOUT_ON)) {
        btnOn->EnableWindow(enable);
    }
    if (CButton* btnOff = (CButton*)GetDlgItem(IDC_BTN_ALARMOUT_OFF)) {
        btnOff->EnableWindow(enable);
    }
}

void controller_watch::on_btn_alarmout_on(void)
{
    int id = _cbxAlarmout.GetCurSel();
    _watch->set_alarm_out(_channel, id - 1, true);
}

void controller_watch::on_btn_alarmout_off(void)
{
    int id = _cbxAlarmout.GetCurSel();
    _watch->set_alarm_out(_channel, id - 1, false);
}

//////////////////////////////////////////////////////////////////////////

LRESULT controller_watch::on_post_screen_camera_changed(WPARAM wParam, LPARAM lParam)
{
    const short camera = static_cast<short>(wParam);
    G2RETURN_VAL_IF_FAIL(client::valid_camera(camera), 1L);

    client::camera_map* cameraMap = runner_ptr()->get_camera_map();
    _selcamera  = cameraMap->get_host_camera(camera);
    _channel    = client::find_channel_from_camera(camera);
    _channelext = client::find_channel_ext_from_camera(camera);
    _hostId     = client::find_host_id_from_camera(camera);

    screen_actuator& actuator = runner_ptr()->actuator_ref();
    char modePTZ = actuator.is_use_camera_ptz(camera);
    BOOL isConnected = (_watch && _watch->is_connected(_channel)) ? TRUE : FALSE;

    enable_ptz_control(modePTZ);
    enable_image_control(isConnected);
    enable_alarmout_control(isConnected);

    return 0L;
}

LRESULT controller_watch::on_post_receive_ptz_preset(WPARAM wParam, LPARAM lParam)
{
    const short channel = static_cast<short>(wParam);
    G2LIVE_PTZ_PRESET* param = reinterpret_cast<G2LIVE_PTZ_PRESET*>(lParam);
    G2RETURN_VAL_IF_FAIL(param != NULL, 1L);

    G2LIVE_PTZ_PRESET preset = *param;
    SAFE_DELETE(param);

    G2RETURN_VAL_IF_FAIL(_channel == channel, 1L);

    G2RETURN_VAL_IF_FAIL(_presetMode != ptz_preset_dlg::MODE_UNDEFIND, 1L);

    ptz_preset_dlg dlg(runner_ptr(), _presetMode);
    dlg.set_preset(preset);
    if (dlg.DoModal() == IDOK) {
        if (_watch->is_connected(_channel)) {
            if (_presetMode == ptz_preset_dlg::MODE_PRESET) {
                _watch->set_ptz_preset(_channel, _selcamera, dlg.preset());
            }
            else {
                G2LIVE_PTZ_COMMAND command = { 0 };
                command._command    = G2LIVE_PTZ_COMMAND::PTZ_MOVE_PRESET;
                command._argument   = dlg.selected();
                _watch->set_ptz_command(_channel, _selcamera, command);
            }
        }
    }
    _presetMode = ptz_preset_dlg::MODE_UNDEFIND;

    return 0L;
}

LRESULT controller_watch::on_post_receive_ptz_menu(WPARAM wParam, LPARAM lParam)
{
    const short channel = static_cast<short>(wParam);
    G2LIVE_PTZ_MENU* param = reinterpret_cast<G2LIVE_PTZ_MENU*>(lParam);
    G2RETURN_VAL_IF_FAIL(param != NULL, 1L);

    G2LIVE_PTZ_MENU menu = *param;
    SAFE_DELETE(param);

    G2RETURN_VAL_IF_FAIL(_channel == channel, 1L);

    ptz_advanced_dlg dlg(runner_ptr());
    dlg.set_menu(menu);
    dlg.set_connector(_watch, _channel, _selcamera);
    dlg.DoModal();

    return 0L;
}
