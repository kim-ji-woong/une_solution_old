// MainFrm.cpp : implementation of the CMainFrame class
//

#include "stdafx.h"
#include "G2Client.h"
#include "G2ClientDoc.h"
#include "G2ClientView.h"
#include "G2ClientSide.h"
#include "MainFrm.h"

#include <control/message_box.h>
#include <control/device_tree.h>
#include <device/device_info_dlg.h>
#include <control/status/status_dlg.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

static unsigned int indicators[] = {
	ID_SEPARATOR,           // status line indicator
	ID_INDICATOR_CAPS,
	ID_INDICATOR_NUM,
	ID_INDICATOR_SCRL,
};

//////////////////////////////////////////////////////////////////////////

CMainFrame::CMainFrame(void)
    : _instant_eventer(NULL)
{
    g_mainframe = this;
}

CMainFrame::~CMainFrame(void)
{
    g_mainframe = NULL;
}

//////////////////////////////////////////////////////////////////////////

CMainFrame* g_mainframe = NULL;

//////////////////////////////////////////////////////////////////////////

IMPLEMENT_DYNCREATE(CMainFrame, CFrameWnd)

BEGIN_MESSAGE_MAP(CMainFrame, CFrameWnd)
    ON_WM_CREATE()
    ON_WM_CLOSE()
    ON_WM_QUERYENDSESSION()
    ON_WM_GETMINMAXINFO()

    ON_COMMAND(ID_MAIN_SYSTEM_ADD,                                          on_main_system_add)
    ON_COMMAND(ID_MAIN_SYSTEM_REMOVE,                                       on_main_system_remove)
    ON_COMMAND(ID_MAIN_SYSTEM_MODIFY,                                       on_main_system_modify)
    ON_COMMAND(ID_MAIN_SYSTEM_STATUS,                                       on_main_system_status)

    ON_MESSAGE(UM_POST_INITIALIZE,                                          OnInvokeInitialize)
    ON_MESSAGE(UM_POST_EMPTY_DEVICE,                                        on_post_empty_device)
    ON_MESSAGE(UM_POST_SCREEN_SELECTED,                                     on_post_screen_selected)
END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////

BOOL CMainFrame::PreCreateWindow(CREATESTRUCT& cs)
{
    if (!CFrameWnd::PreCreateWindow(cs)) {
        return FALSE;
    }
    cs.style |= WS_CLIPCHILDREN | WS_CLIPSIBLINGS;
    cs.style &= ~FWS_ADDTOTITLE;

    return TRUE;
}

BOOL CMainFrame::OnCreateClient(LPCREATESTRUCT lpcs, CCreateContext* pContext)
{
    // create splitter window
    if (_splitter.CreateStatic(this, 1, 2) != TRUE) {
        return FALSE;
    }

    if (_splitter.CreateView(0, 0, RUNTIME_CLASS(CG2ClientSide), CSize(200, 700), pContext) != TRUE ||
        _splitter.CreateView(0, 1, RUNTIME_CLASS(CG2ClientView), CSize(800, 700), pContext) != TRUE) {
        _splitter.DestroyWindow();
        return FALSE;
    }

    _splitter.set_control_cx(200);
    _splitter.set_content_cx(800);

    /////////////////////////////////////////////

    construct_menu_bar();

    return TRUE;
}

#ifdef _DEBUG
void CMainFrame::AssertValid() const
{
    CFrameWnd::AssertValid();
}

void CMainFrame::Dump(CDumpContext& dc) const
{
    CFrameWnd::Dump(dc);
}
#endif //_DEBUG

//////////////////////////////////////////////////////////////////////////

int CMainFrame::OnCreate(LPCREATESTRUCT lpCreateStruct)
{
    if (CFrameWnd::OnCreate(lpCreateStruct) == -1) {
        return -1;
    }

    client::runner* runner = client::runner_ptr();
    G2RETURN_VAL_IF_FAIL(runner != NULL, 1L);

    return 0;
}

void CMainFrame::OnClose()
{
    if (MessageBox(_T("Do you want to exit?"), _T("G2Client"), MB_ICONQUESTION | MB_YESNO) != IDYES) {
        return;
    }

    exit_imp();

    CFrameWnd::OnClose();
}

BOOL CMainFrame::OnQueryEndSession()
{
    bool logoff = false;
    if (const MSG* msg = GetCurrentMessage()) {
        if (msg->lParam & ENDSESSION_LOGOFF) {
            logoff = true;
        }
    }

    exit_imp();

    return TRUE;
}

//////////////////////////////////////////////////////////////////////////

void CMainFrame::on_main_system_add()
{
    device_info_dlg dlg(this);

    if (dlg.DoModal() == IDOK) {
        _manager.add(dlg.deviceInfo());
    }
}

void CMainFrame::on_main_system_remove()
{
    CString site = _manager.selected_site();
    if (site.IsEmpty()) return;

    CString text;
    text.Format(L"Do you want to remove \"%s\"", site);
    if (MessageBox(text, _T("G2Client"), MB_YESNO) == IDYES) {
        _manager.remove_selected();
    }
}

void CMainFrame::on_main_system_modify()
{
    CString site = _manager.selected_site();
    if (site.IsEmpty()) return;

    device_info_manager::valueType info;
    if (_manager.get_info(site, info) == false) return;

    device_info_dlg dlg(this);
    dlg.setDeviceInfo(info);
    if (dlg.DoModal() == IDOK) {
        _manager.modify(site, dlg.deviceInfo());
    }
}

void CMainFrame::on_main_system_status()
{
    StatusViewer dlg(this);
    _status_viewer = &dlg;
    client::runner* runner = runner_ptr();
    runner->connect_status();
    if (dlg.DoModal() == IDOK) {
        _status_viewer = NULL;
    }
}
 
void CMainFrame::OnGetMinMaxInfo(MINMAXINFO* lpMMI)
{
    lpMMI->ptMinTrackSize.x = MIN_FRAME_SIZE_CX;
    lpMMI->ptMinTrackSize.y = MIN_FRAME_SIZE_CY;

    CFrameWnd::OnGetMinMaxInfo(lpMMI);
}

//////////////////////////////////////////////////////////////////////////

LRESULT CMainFrame::OnInvokeInitialize(WPARAM wParam, LPARAM lParam)
{
    initialize();

    if (client::runner* runner = client::runner_ptr()) {
        runner->initialize();
    }
    else {
        assert(!"runner object is not initiated");
    }

    SetForegroundWindow();

    return 0L;
}

LRESULT CMainFrame::on_post_empty_device(WPARAM wParam, LPARAM lParam)
{
    MessageBox(_T("There is no Device"), NULL, MB_OK);

    return 0L;
}

LRESULT CMainFrame::on_post_screen_selected(WPARAM wParam, LPARAM lParam)
{
    G2RETURN_VAL_IF_FAIL(client::app::get().is_initialized(), 1L);

    const short camera = static_cast<short>(wParam);
    CString title = _T("G2Client ");

    short hostId = client::find_host_id_from_camera(camera);
    client::connective_host_list* hostList = runner_ptr()->get_host_list();

    CString address = hostList->address_from_host_id(hostId).c_str();
    CString site = hostList->site_name_from_host_id(hostId).c_str();

    if (!(address.IsEmpty() || site.IsEmpty()))
        title.Format(_T("%s - %s"), address, site);

    SetWindowText(title);

    return 0L;
}

//////////////////////////////////////////////////////////////////////////

client::runner* CMainFrame::get_runner(void)
{
    return static_cast<client::runner*>(GetActiveView());
}

//////////////////////////////////////////////////////////////////////////
client::StatusViewer* CMainFrame::get_status_viewer(void)
{
    return _status_viewer;
}

//////////////////////////////////////////////////////////////////////////

bool CMainFrame::initialize(void)
{
    client::app::get().set_initialized();

    return true;
}

bool CMainFrame::finalize(void)
{
    return true;
}

bool CMainFrame::finalize_prev(void)
{
    if (_instant_eventer != NULL &&
        ::IsWindow(_instant_eventer->GetSafeHwnd())) {
            _instant_eventer->clear();
    }
    return true;
}

void CMainFrame::exit_imp(void)
{
    static bool entry = false;

    if (entry) {
        assert(client::app::get().is_finalize());
        return;
    }

    entry = true;

    client::app::get().set_finalize();

    EnableWindow(FALSE);

    /////////////////////////////////////////////

    if (client::runner* runner = runner_ptr()) {
        runner->finalize();
    }

    /////////////////////////////////////////////

    finalize_prev();
    finalize();
}

void CMainFrame::construct_menu_bar(void)
{
    enum { menu_system = 0, menu_view, menu_help };

    CMenu menu;
    menu.LoadMenu(IDR_MAINFRAME);
}

void CMainFrame::append_instant_event(const G2EVENT_INFO& info, const G2DEVICE_STATUS& status)
{
    G2RETURN_IF_FAIL(_instant_eventer != NULL &&
        ::IsWindow(_instant_eventer->GetSafeHwnd()));

    _instant_eventer->append_event(info, status);
}
