// panel_device_tree.cpp : implementation
//

#include "stdafx.h"
#include "G2Client.h"
#include "MainFrm.h"
#include "panel_device_tree.h"
#include "control/device_tree.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

//////////////////////////////////////////////////////////////////////////

using namespace client;

//////////////////////////////////////////////////////////////////////////

enum { IDD = IDD_DUMMY_CHILD };

enum RESOURCE_ID {
    IDCC_DEVICE_TREE = 1024
};

//////////////////////////////////////////////////////////////////////////

panel_device_tree::panel_device_tree(void)
    : _parent(NULL)
{
    _tree.reset(new device_tree());
}

panel_device_tree::~panel_device_tree(void)
{
    finalize();
}

//////////////////////////////////////////////////////////////////////////

void panel_device_tree::DoDataExchange(CDataExchange* pDX)
{
    CDialog::DoDataExchange(pDX);
}

//////////////////////////////////////////////////////////////////////////

BEGIN_MESSAGE_MAP(panel_device_tree, CDialog)
    ON_WM_CREATE()
    ON_WM_DESTROY()
    ON_WM_ERASEBKGND()
    ON_WM_SIZE()
END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////

bool panel_device_tree::create(CWnd* parent, unsigned int id)
{
    bool created = (CDialog::Create(IDD, parent) == TRUE);
    if (created) {
        SetDlgCtrlID(id);
        _parent = parent;
    }
    else {
        assert(!"failed to create tree panel");
    }
    return created;
}

void panel_device_tree::cleanup(void)
{

}

//////////////////////////////////////////////////////////////////////////

BOOL panel_device_tree::PreTranslateMessage(MSG* pMsg)
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

int panel_device_tree::OnCreate(LPCREATESTRUCT lpcs)
{
    if (CDialog::OnCreate(lpcs) == -1) {
        return -1;
    }

    ModifyStyle(0, WS_CLIPCHILDREN | WS_CLIPSIBLINGS);

    return 0;
}

BOOL panel_device_tree::OnInitDialog()
{
    CDialog::OnInitDialog();

    initialize();

    return TRUE;
}

void panel_device_tree::OnDestroy()
{
    CDialog::OnDestroy();
}

BOOL panel_device_tree::OnEraseBkgnd(CDC* pDC)
{
    return TRUE;    // CDialog::OnEraseBkgnd(pDC);
}

void panel_device_tree::OnSize(unsigned int nType, int cx, int cy)
{
    CDialog::OnSize(nType, cx, cy);

    if (_tree.get() && ::IsWindow(_tree->GetSafeHwnd())) {
        _tree->SetWindowPos(NULL, 0, 0, cx, cy, SWP_NOMOVE);
    }
}

//////////////////////////////////////////////////////////////////////////

void panel_device_tree::initialize(void)
{
    CRect rect;
    GetClientRect(rect);

    _tree->Create(WS_CHILD | WS_VISIBLE,
                  rect,
                  this,
                  IDCC_DEVICE_TREE);

    client::foundation_ptr()->set_device_tree(_tree);
}

void panel_device_tree::finalize(void)
{
    if (_tree.get() && ::IsWindow(_tree->GetSafeHwnd())) {
        _tree->DestroyWindow();
    }
}
