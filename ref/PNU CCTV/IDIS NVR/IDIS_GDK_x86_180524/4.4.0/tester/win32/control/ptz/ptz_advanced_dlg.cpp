// ptz_advanced_dlg.cpp : implementation file
//

#include "stdafx.h"
#include "ptz_advanced_dlg.h"
#include <sampler/cpp/g2client_watch.h>
#include <include/g2_define_watch.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

enum { IDD = IDD_PTZ_ADVANCED };

enum MENU_ID {
    IDM_PTZ_MENU_ON = 30001,
    IDM_PTZ_MENU_OFF,

    IDM_PTZ_INDEX_BEGIN = 30011,
    IDM_PTZ_INDEX_END   = 30026,
};

BYTE ptz_advanced_dlg::_indexReposit[COUNTOF_INDEXER] = { 1, 1, 1 };

//////////////////////////////////////////////////////////////////////////

ptz_advanced_dlg::ptz_advanced_dlg(CWnd* parent /*=NULL*/)
    : CDialog(IDD, parent)
    , _parent(parent)
    , _watch(NULL)
    , _channel(client::invalid_::CHANNEL)
    , _activateCTRL(false)
    , _commandId(-1)
{
    memset(&_menu, 0, sizeof(_menu));
}

ptz_advanced_dlg::~ptz_advanced_dlg(void)
{

}

//////////////////////////////////////////////////////////////////////////

void ptz_advanced_dlg::DoDataExchange(CDataExchange* pDX)
{
    CDialog::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(ptz_advanced_dlg, CDialog)
    ON_COMMAND_RANGE(IDM_PTZ_MENU_ON, IDM_PTZ_MENU_OFF,         on_menu_on_off)
    ON_COMMAND_RANGE(IDM_PTZ_INDEX_BEGIN, IDM_PTZ_INDEX_END,    on_menu_index)

    ON_CONTROL_RANGE(BN_CLICKED, IDC_BTN_SPEED, IDC_BTN_ESC,    on_btn_ptz)
END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////

BOOL ptz_advanced_dlg::OnInitDialog()
{
    CDialog::OnInitDialog();

    if (_parent != NULL &&
        ::IsWindow(_parent->GetSafeHwnd())) {
        CenterWindow(_parent);
    }

    init_menu();

    return TRUE;
}

void ptz_advanced_dlg::on_menu_on_off(UINT nID)
{
    G2RETURN_IF_ASSERT(_commandId != -1);

    bool activate = (nID == IDM_PTZ_MENU_ON) ? true : false;
    int command = secure_command(_commandId, activate);
    int argument = secure_argument(_commandId);

    apply_command(command, argument);
}

void ptz_advanced_dlg::on_menu_index(UINT nID)
{
    G2RETURN_IF_ASSERT(_commandId != -1);

    int index = (nID - IDM_PTZ_INDEX_BEGIN) + 1;
    if (_commandId == IDC_BTN_SPEED) {
        int speed = index - 1;
        speed = (speed < 0) ? 0 :
                (speed > 15) ? 15 : speed;

        apply_command(G2LIVE_PTZ_COMMAND::PTZ_SET_SPEED, speed);
        return;
    }

    unsigned int indexer = _UI32_MAX;

    switch (_commandId) {
        case IDC_BTN_TOUR_NUMBER:
            indexer = INDEXER_TOUR;
            break;
        case IDC_BTN_PATTERN_NUMBER:
            indexer = INDEXER_PATTERN;
            break;
        default:
            assert(!"undefined command control");
            break;
    }

    if (indexer != _UI32_MAX) {
        CString string;
        string.Format(L"%d", index);
        GetDlgItem(_commandId)->SetWindowText(string);
        _indexReposit[indexer] = index;
    }
}

void ptz_advanced_dlg::on_btn_ptz(UINT nID)
{
    if (nID == IDC_BTN_MOVE_ORIGIN ||
        nID == IDC_BTN_ENTER ||
        nID == IDC_BTN_ESC ||
        nID == IDC_CHK_CTRL) {
        int command = secure_command(nID);

        int argument = -1;
        if (command == G2LIVE_PTZ_COMMAND::PTZ_SETCTRL_ON) {
            if (CButton* ctrl = (CButton*)GetDlgItem(IDC_CHK_CTRL)) {
                _menu._active_CTRL = (ctrl->GetCheck() == BST_CHECKED);
                argument = _menu._active_CTRL ? 1 : 0;
            }
        }
        apply_command(command, argument);
    }
    else {
        CMenu menu;
        if (create_popup_menu(menu, nID)) {
            _commandId = nID;
            CRect rect;
            GetDlgItem(nID)->GetWindowRect(&rect);
            menu.TrackPopupMenu(TPM_LEFTALIGN | TPM_RIGHTBUTTON | TPM_NOANIMATION,
                                rect.right, rect.top,
                                this);
        }
    }
}

//////////////////////////////////////////////////////////////////////////

void ptz_advanced_dlg::set_menu(const G2LIVE_PTZ_MENU& menu)
{
    _menu = menu;
}

void ptz_advanced_dlg::set_connector(client::g2watch* watch, short channel, short camera)
{
    G2RETURN_IF_ASSERT(watch != NULL);
    G2RETURN_IF_ASSERT(client::valid_channel(channel));

    _watch      = watch;
    _channel    = channel;
    _camera     = camera;
}

//////////////////////////////////////////////////////////////////////////

void ptz_advanced_dlg::init_menu(void)
{
    for (unsigned int i = IDC_BTN_SPEED; i <= IDC_CHK_CTRL; ++i) {
        GetDlgItem(i)->EnableWindow(FALSE);
    }

    if (_menu._function & G2LIVE_PTZ_MENU::FUNCTION_SPEED) {
        GetDlgItem(IDC_BTN_SPEED)->EnableWindow(TRUE);
    }
    if (_menu._function & G2LIVE_PTZ_MENU::FUNCTION_AUTOPAN) {
        GetDlgItem(IDC_BTN_AUTO_PAN)->EnableWindow(TRUE);
    }
    if (_menu._function & G2LIVE_PTZ_MENU::FUNCTION_TOUR) {
        GetDlgItem(IDC_BTN_TOUR)->EnableWindow(TRUE);
        GetDlgItem(IDC_BTN_TOUR_NUMBER)->EnableWindow(TRUE);
    }
    if (_menu._function & G2LIVE_PTZ_MENU::FUNCTION_PATTERN) {
        GetDlgItem(IDC_BTN_PATTERN)->EnableWindow(TRUE);
        GetDlgItem(IDC_BTN_PATTERN_NUMBER)->EnableWindow(TRUE);
    }
    if (_menu._function & G2LIVE_PTZ_MENU::FUNCTION_OSDMENU) {
        GetDlgItem(IDC_BTN_DEVICE_MENU)->EnableWindow(TRUE);
    }
    if (_menu._function & G2LIVE_PTZ_MENU::FUNCTION_LIGHT) {
        GetDlgItem(IDC_BTN_LIGHT)->EnableWindow(TRUE);
    }
    if (_menu._function & G2LIVE_PTZ_MENU::FUNCTION_PUMP) {
        GetDlgItem(IDC_BTN_PUMP)->EnableWindow(TRUE);
    }
    if (_menu._function & G2LIVE_PTZ_MENU::FUNCTION_WIPER) {
        GetDlgItem(IDC_BTN_WIPER)->EnableWindow(TRUE);
    }
    if (_menu._function & G2LIVE_PTZ_MENU::FUNCTION_POWER) {
        GetDlgItem(IDC_BTN_POWER)->EnableWindow(TRUE);
    }
    if (_menu._function & G2LIVE_PTZ_MENU::FUNCTION_AUX) {
        GetDlgItem(IDC_BTN_AUX)->EnableWindow(TRUE);
    }
    if (_menu._function & G2LIVE_PTZ_MENU::FUNCTION_ORIGIN) {
        GetDlgItem(IDC_BTN_MOVE_ORIGIN)->EnableWindow(TRUE);
    }
    if (_menu._function & G2LIVE_PTZ_MENU::FUNCTION_CTRL) {
        GetDlgItem(IDC_CHK_CTRL)->EnableWindow(TRUE);
        CButton* ctrl = (CButton*)GetDlgItem(IDC_CHK_CTRL);
        ctrl->SetCheck(_menu._active_CTRL ? BST_CHECKED : BST_UNCHECKED);
    }

    GetDlgItem(IDC_BTN_TOUR_NUMBER)->SetWindowText(L"1");
    GetDlgItem(IDC_BTN_PATTERN_NUMBER)->SetWindowText(L"1");
}

void ptz_advanced_dlg::apply_command(int cmd, int arg /*= 0*/)
{
    G2RETURN_IF_ASSERT(_watch != NULL);
    G2RETURN_IF_FAIL(client::valid_channel(_channel));
    G2RETURN_IF_FAIL(cmd != -1);

    G2LIVE_PTZ_COMMAND command = { 0 };
    command._command    = cmd;
    command._argument   = arg;

    if (_watch->is_connected(_channel)) {
        _watch->set_ptz_command(_channel, _camera, command);
    }
}

int ptz_advanced_dlg::secure_command(unsigned int id, bool activate /*= false*/)
{
    int command = -1;

    switch (id) {
        case IDC_BTN_SPEED:
            command = G2LIVE_PTZ_COMMAND::PTZ_SET_SPEED;
            break;
        case IDC_BTN_AUTO_PAN:
            command = (activate) ? G2LIVE_PTZ_COMMAND::PTZ_AUTO_PAN_ON : G2LIVE_PTZ_COMMAND::PTZ_AUTO_PAN_OFF;
            break;
        case IDC_BTN_TOUR:
            command = (activate) ? G2LIVE_PTZ_COMMAND::PTZ_TOUR_ON : G2LIVE_PTZ_COMMAND::PTZ_TOUR_OFF;
            break;
        case IDC_BTN_PATTERN:
            command = (activate) ? G2LIVE_PTZ_COMMAND::PTZ_PATTERN_ON : G2LIVE_PTZ_COMMAND::PTZ_PATTERN_OFF;
            break;
        case IDC_BTN_DEVICE_MENU:
            command = (activate) ? G2LIVE_PTZ_COMMAND::PTZ_MENU_ON : G2LIVE_PTZ_COMMAND::PTZ_MENU_OFF;
            break;
        case IDC_BTN_LIGHT:
            command = (activate) ? G2LIVE_PTZ_COMMAND::PTZ_LIGHT_TURNON : G2LIVE_PTZ_COMMAND::PTZ_LIGHT_TURNOFF;
            break;
        case IDC_BTN_PUMP:
            command = (activate) ? G2LIVE_PTZ_COMMAND::PTZ_PUMP_TURNON : G2LIVE_PTZ_COMMAND::PTZ_PUMP_TURNOFF;
            break;
        case IDC_BTN_WIPER:
            command = (activate) ? G2LIVE_PTZ_COMMAND::PTZ_WIPER_TURNON : G2LIVE_PTZ_COMMAND::PTZ_WIPER_TURNOFF;
            break;
        case IDC_BTN_POWER:
            command = (activate) ? G2LIVE_PTZ_COMMAND::PTZ_POWER_TURNON : G2LIVE_PTZ_COMMAND::PTZ_POWER_TURNOFF;
            break;
        case IDC_BTN_AUX:
            command = (activate) ? G2LIVE_PTZ_COMMAND::PTZ_AUX_TURNON : G2LIVE_PTZ_COMMAND::PTZ_AUX_TURNOFF;
            break;
        case IDC_BTN_MOVE_ORIGIN:
            command = G2LIVE_PTZ_COMMAND::PTZ_MOVETO_ORIGIN;
            break;
        case IDC_BTN_ENTER:
            command = G2LIVE_PTZ_COMMAND::PTZ_MENU_ENTER;
            break;
        case IDC_BTN_ESC:
            command = G2LIVE_PTZ_COMMAND::PTZ_MENU_ESC;
            break;
        case IDC_CHK_CTRL:
            command = G2LIVE_PTZ_COMMAND::PTZ_SETCTRL_ON;
        default:
            assert(!"undefined command control");
            break;
    }

    return command;
}

int ptz_advanced_dlg::secure_argument(unsigned int id)
{
    int indexer = -1;
    switch (id) {
        case IDC_BTN_TOUR:
            indexer = INDEXER_TOUR;
            break;
        case IDC_BTN_PATTERN:
            indexer = INDEXER_PATTERN;
            break;
    }

    return (indexer != -1) ? _indexReposit[indexer] : -1;
}

bool ptz_advanced_dlg::create_popup_menu(CMenu& menu, unsigned int id)
{
    if (::IsMenu(menu)) {
        menu.DestroyMenu();
    }

    G2RETURN_VAL_IF_ASSERT(menu.CreatePopupMenu() == TRUE, false);

    if (id == IDC_BTN_SPEED ||
        id == IDC_BTN_TOUR_NUMBER ||
        id == IDC_BTN_PATTERN_NUMBER) {
        CString string;
        for (int i = IDM_PTZ_INDEX_BEGIN; i <= IDM_PTZ_INDEX_END; ++i) {
            string.Format(L"%d", i - IDM_PTZ_INDEX_BEGIN + 1);
            menu.AppendMenu(MF_STRING, i, string);
        }
    }
    else {
        menu.AppendMenu(MF_STRING, IDM_PTZ_MENU_ON,  L"On");
        menu.AppendMenu(MF_STRING, IDM_PTZ_MENU_OFF, L"Off");
    }

    return true;
}
