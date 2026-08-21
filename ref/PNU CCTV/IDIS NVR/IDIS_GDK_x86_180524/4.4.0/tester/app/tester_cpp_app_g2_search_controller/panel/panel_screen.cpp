// panel_screen.cpp : implementation
//

#include "stdafx.h"
#include "panel_screen.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

panel_screen::panel_screen(void)
    : _parent(NULL)
{

}

panel_screen::~panel_screen(void)
{

}

//////////////////////////////////////////////////////////////////////////

void panel_screen::DoDataExchange(CDataExchange* pDX)
{
    CDialog::DoDataExchange(pDX);
}

//////////////////////////////////////////////////////////////////////////

BEGIN_MESSAGE_MAP(panel_screen, CDialog)
    ON_WM_CREATE()
    ON_WM_DESTROY()
    ON_WM_SIZE()
    ON_WM_ERASEBKGND()
END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////

bool panel_screen::create(CWnd* parent, unsigned int id)
{
    bool created = (CDialog::Create(IDD_DUMMY_CHILD, parent) == TRUE);
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

BOOL panel_screen::PreTranslateMessage(MSG* pMsg)
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

int panel_screen::OnCreate(LPCREATESTRUCT lpcs)
{
    if (CDialog::OnCreate(lpcs) == -1) {
        return -1;
    }

    ModifyStyle(0, WS_CLIPCHILDREN | WS_CLIPSIBLINGS);

    return 0;
}

BOOL panel_screen::OnInitDialog()
{
    CDialog::OnInitDialog();

    initialize();

    return TRUE;
}

void panel_screen::OnDestroy()
{
    CDialog::OnDestroy();
}

void panel_screen::OnSize(unsigned int nType, int cx, int cy)
{
    CDialog::OnSize(nType, cx, cy);
}

BOOL panel_screen::OnEraseBkgnd(CDC* pDC)
{
    return TRUE;    // CDialog::OnEraseBkgnd(pDC);
}

//////////////////////////////////////////////////////////////////////////

void panel_screen::initialize(void)
{
    CRect rect;
    GetClientRect(rect);

    _actuator.create(this,
                     rect,
                     IDCC_SCREEN_CTRL,
                     client::MAX_SCREEN_CAMERA_COUNT,
                     client::MAX_CONNECTIVE_CHANNEL);

}

void panel_screen::finalize(void)
{
    _actuator.cleanup();
}
