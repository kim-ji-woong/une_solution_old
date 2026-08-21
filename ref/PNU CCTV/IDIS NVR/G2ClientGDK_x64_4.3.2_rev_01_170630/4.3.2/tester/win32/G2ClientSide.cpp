// G2ClientSide.cpp : implementation file
//

#include "stdafx.h"
#include "G2Client.h"
#include "G2ClientSide.h"
#include <control/device_tree.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

//////////////////////////////////////////////////////////////////////////

using namespace client;

//////////////////////////////////////////////////////////////////////////

CG2ClientSide::CG2ClientSide(void)
{

}

CG2ClientSide::~CG2ClientSide(void)
{

}

//////////////////////////////////////////////////////////////////////////

IMPLEMENT_DYNCREATE(CG2ClientSide, CView)

BEGIN_MESSAGE_MAP(CG2ClientSide, CView)
    ON_WM_CREATE()
    ON_WM_ERASEBKGND()
    ON_WM_STYLECHANGED()
    ON_WM_SIZE()
END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////

BOOL CG2ClientSide::PreCreateWindow(CREATESTRUCT& cs)
{
    return CView::PreCreateWindow(cs);
}

void CG2ClientSide::OnDraw(CDC* /*pDC*/)
{

}

void CG2ClientSide::OnInitialUpdate()
{
    CView::OnInitialUpdate();
}

int CG2ClientSide::OnCreate(LPCREATESTRUCT lpcs)
{
    int created = CView::OnCreate(lpcs);

    if (created != -1) {
        initialize();
    }
    else {

    }

    return created;
}

BOOL CG2ClientSide::OnEraseBkgnd(CDC* pDC)
{
    return TRUE;    // CView::OnErageBkgnd(pDC);
}

void CG2ClientSide::OnStyleChanged(int nStyleType, LPSTYLESTRUCT lpStyleStruct)
{
    CView::OnStyleChanged(nStyleType,lpStyleStruct);
}

void CG2ClientSide::OnSize(unsigned int nType, int cx, int cy)
{
    CView::OnSize(nType, cx, cy);

    if (::IsWindow(_splitter.GetSafeHwnd())) {
        _splitter.SetWindowPos(NULL, 0, 0, cx, cy, SWP_NOMOVE | SWP_NOZORDER | SWP_NOREDRAW);
    }
}

//////////////////////////////////////////////////////////////////////////

void CG2ClientSide::initialize(void)
{
    if (_splitter.CreateStatic(this, 2, 1) != TRUE) {
        return;
    }

    _tree_panel.create(&_splitter, _splitter.IdFromRowCol(0, 0));
    _event_panel.create(&_splitter, _splitter.IdFromRowCol(1, 0));

    _splitter.create(0, 0, &_tree_panel, CSize(0, 200));
    _splitter.create(1, 0, &_event_panel, CSize(0, 150));

    /////////////////////////////////////////////

    CRect rect;
    GetClientRect(rect);
    _splitter.SetWindowPos(NULL, 0, 0, rect.Width(), rect.Height(), SWP_NOMOVE | SWP_NOZORDER);

    _tree_panel.ShowWindow(SW_SHOW);
    _event_panel.ShowWindow(SW_SHOW);
}

void CG2ClientSide::finalize(void)
{

}
