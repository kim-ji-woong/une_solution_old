// panel_controller.cpp : implementation
//

#include "stdafx.h"
#include "G2Client.h"
#include "MainFrm.h"
#include "panel_controller.h"
#include "controller/controller_base.h"
#include "controller/controller_watch.h"
#include "controller/controller_search.h"
#include "controller/controller_search_g2.h"

#include <algorithm>
#include <functional>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

enum { IDD = IDD_DUMMY_CHILD };

enum RESOURCE_ID {
    IDCC_CONTROLLER_LIVE = 1024,
    IDCC_CONTROLLER_PLAY,
    IDCC_CONTROLLER_SEARCH,
    IDCC_CONTROLLER_LOG
};

//////////////////////////////////////////////////////////////////////////

panel_controller::panel_controller(void)
    : _parent(NULL)
    , _prevType(CONTROLLER_LIVE)
{

}

panel_controller::~panel_controller(void)
{
    finalize();
}

//////////////////////////////////////////////////////////////////////////

controller_watch* panel_controller::controller_live_ptr(void)
{
    return (_container[CONTROLLER_LIVE] != NULL &&
            ::IsWindow(_container[CONTROLLER_LIVE]->safe_hwnd())) ? reinterpret_cast<controller_watch*>(_container[CONTROLLER_LIVE]) : NULL;
}

controller_search* panel_controller::controller_search_ptr(void)
{
    return (_container[CONTROLLER_SEARCH] != NULL &&
            ::IsWindow(_container[CONTROLLER_SEARCH]->safe_hwnd())) ? reinterpret_cast<controller_search*>(_container[CONTROLLER_SEARCH]) : NULL;
}

controller_search_g2* panel_controller::controller_search_g2_ptr(void)
{
    return (_container[CONTROLLER_SEARCH_G2] != NULL &&
        ::IsWindow(_container[CONTROLLER_SEARCH_G2]->safe_hwnd())) ? reinterpret_cast<controller_search_g2*>(_container[CONTROLLER_SEARCH_G2]) : NULL;
}

std::vector<controller_base*> panel_controller::controller_bunch(void)
{
    return _container;
}

//////////////////////////////////////////////////////////////////////////

void panel_controller::DoDataExchange(CDataExchange* pDX)
{
    CDialog::DoDataExchange(pDX);
}

//////////////////////////////////////////////////////////////////////////

BEGIN_MESSAGE_MAP(panel_controller, CDialog)
    ON_WM_CREATE()
    ON_WM_DESTROY()
    ON_WM_SIZE()
    ON_WM_ERASEBKGND()

    ON_MESSAGE(um_controller_::UM_SCREEN_CAMERA_CHANGED,    on_post_screen_camera_changed)
    ON_MESSAGE(um_controller_::UM_UPDATE_TIME_TABLE,        on_post_update_time_table)
END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////

bool panel_controller::create(CWnd* parent, unsigned int id)
{
    bool created = (CDialog::Create(IDD, parent) == TRUE);
    if (created) {
        SetDlgCtrlID(id);
        _parent = parent;
    }
    else {
        assert(!"failed to create event panel");
    }
    return created;
}

//////////////////////////////////////////////////////////////////////////

BOOL panel_controller::PreTranslateMessage(MSG* pMsg)
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

int panel_controller::OnCreate(LPCREATESTRUCT lpcs)
{
    if (CDialog::OnCreate(lpcs) == -1) {
        return -1;
    }

    ModifyStyle(WS_CLIPCHILDREN | WS_CLIPSIBLINGS, 0);

    return 0;
}

BOOL panel_controller::OnInitDialog()
{
    CDialog::OnInitDialog();

    initialize();

    return TRUE;
}

void panel_controller::OnDestroy()
{
    CDialog::OnDestroy();
}

void panel_controller::OnSize(unsigned int nType, int cx, int cy)
{
    CDialog::OnSize(nType, cx, cy);

    for(std::vector<controller_base*>::iterator itr(_container.begin());
        itr != _container.end();
        ++itr) {
        controller_base* controller = *itr;
        if (controller) {
            controller->SetWindowPos(NULL, 0, 0, cx, cy, SWP_NOMOVE | SWP_NOZORDER);
        }
    }
}

BOOL panel_controller::OnEraseBkgnd(CDC* pDC)
{
    return CDialog::OnEraseBkgnd(pDC);
}

//////////////////////////////////////////////////////////////////////////

void panel_controller::initialize(void)
{
    _container.resize(CONTROLLER_COUNT, NULL);

    _container[CONTROLLER_LIVE]   = new controller_watch();
    _container[CONTROLLER_SEARCH] = new controller_search();
    _container[CONTROLLER_SEARCH_G2] = new controller_search_g2();

    std::for_each(_container.begin(), _container.end(), std::bind2nd(std::mem_fun(&client::controller_base::create), this));

    _container[CONTROLLER_LIVE]->ShowWindow(SW_SHOW);
}

void panel_controller::finalize(void)
{
    for(std::vector<controller_base*>::iterator itr(_container.begin());
        itr != _container.end();
        ++itr) {
        SAFE_DELETE(*itr);
    }
}

//////////////////////////////////////////////////////////////////////////

void panel_controller::change_controller(int type)
{
    G2RETURN_IF_ASSERT(type != client::view_::VIEW_UNDEFINED);

    std::for_each(_container.begin(), _container.end(), std::bind2nd(std::mem_fun(&CWnd::ShowWindow), SW_HIDE));

    controller_base* controllerptr = NULL;
    switch (type) {
        case client::view_::VIEW_SCREEN:
            controllerptr = _container[_prevType];
            break;
        default:
            assert(!"undefined view type");
            break;
    }

    if (controllerptr != NULL) {
        controllerptr->ShowWindow(SW_SHOW);
    }
}

//////////////////////////////////////////////////////////////////////////

LRESULT panel_controller::on_post_screen_camera_changed(WPARAM wParam, LPARAM lParam)
{
    const short camera = static_cast<short>(wParam);
    G2RETURN_VAL_IF_FAIL(client::valid_camera(camera), 1L);

    std::for_each(_container.begin(), _container.end(), std::bind2nd(std::mem_fun(&CWnd::ShowWindow), SW_HIDE));

    controller_base* controllerptr = NULL;

    switch (client::find_mode_from_camera(camera))  {
        case connect_mode_::SEARCH:
            controllerptr = _container[CONTROLLER_SEARCH];
            _prevType = CONTROLLER_SEARCH;
            break;
        case connect_mode_::SEARCH_G2:
            controllerptr = _container[CONTROLLER_SEARCH_G2];
            _prevType = CONTROLLER_SEARCH_G2;
            break;
        default:
            controllerptr = _container[CONTROLLER_LIVE];
            _prevType = CONTROLLER_LIVE;
            break;
    }

    if (controllerptr != NULL) {
        controllerptr->ShowWindow(SW_SHOW);
        controllerptr->PostMessage(um_controller_::UM_SCREEN_CAMERA_CHANGED, WPARAM(camera));
    }

    return 0L;
}

LRESULT panel_controller::on_post_update_time_table(WPARAM wParam, LPARAM lParam)
{
    const short camera = static_cast<short>(wParam);
    G2RETURN_VAL_IF_FAIL(client::valid_camera(camera), 1L);

    std::for_each(_container.begin(), _container.end(), std::bind2nd(std::mem_fun(&CWnd::ShowWindow), SW_HIDE));

    controller_base* controllerptr = NULL;

    switch (client::find_mode_from_camera(camera))  {
        case connect_mode_::SEARCH:
            controllerptr = _container[CONTROLLER_SEARCH];
            _prevType = CONTROLLER_SEARCH;
            break;
        case connect_mode_::SEARCH_G2:
            controllerptr = _container[CONTROLLER_SEARCH_G2];
            _prevType = CONTROLLER_SEARCH_G2;
            break;
        default:
            controllerptr = _container[CONTROLLER_LIVE];
            _prevType = CONTROLLER_LIVE;
            break;
    }

    if (controllerptr != NULL) {
        controllerptr->ShowWindow(SW_SHOW);
        controllerptr->PostMessage(um_controller_::UM_UPDATE_TIME_TABLE);
    }

    return 0L;
}

