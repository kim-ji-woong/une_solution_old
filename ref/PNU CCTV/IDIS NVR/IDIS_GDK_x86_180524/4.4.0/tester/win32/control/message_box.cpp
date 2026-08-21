// message_box.cpp : implementation
//

#include "stdafx.h"
#include "message_box.h"
#include <MainFrm.h>
#include <common/client_functional.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

enum { IDD = IDD_DUMMY_CHILD };

enum RESOURCE_ID {
    IDCC_STC_MESSAGE = 1024,
    IDCC_STC_DESCRIPTION
};

enum TIMER_ID {
    TIMER_ID_POSITION_CHECK = 1000,
    TIMER_ID_AUTO_HIDE
};

//////////////////////////////////////////////////////////////////////////

message_box::message_box(void)
    : _parent(NULL)
{

}

message_box::~message_box(void)
{

}

//////////////////////////////////////////////////////////////////////////

void message_box::DoDataExchange(CDataExchange* pDX)
{
    CDialog::DoDataExchange(pDX);
}

//////////////////////////////////////////////////////////////////////////

BEGIN_MESSAGE_MAP(message_box, CDialog)
    ON_WM_CREATE()
    ON_WM_DESTROY()
    ON_WM_SIZE()
    ON_WM_ERASEBKGND()
    ON_WM_CTLCOLOR()
    ON_WM_TIMER()
END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////

bool message_box::create(CWnd* parent, const CSize& size /*= CSize(600, 60)*/)
{
    G2RETURN_VAL_IF_ASSERT(parent != NULL, false);

    _parent = parent;
    _size = size;

    bool created = (CDialog::Create(IDD, parent) == TRUE);
    if (created) {

    }
    else {
        assert(!"failed to create message box");
    }
    return created;
}

//////////////////////////////////////////////////////////////////////////

BOOL message_box::PreTranslateMessage(MSG* pMsg)
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

int message_box::OnCreate(LPCREATESTRUCT lpcs)
{
    if (CDialog::OnCreate(lpcs) == -1) {
        return -1;
    }

    return 0;
}

BOOL message_box::OnInitDialog()
{
    CDialog::OnInitDialog();

    ModifyStyle(WS_VISIBLE, 0);

    initialize();

    SetWindowPos(NULL, 0, 0, _size.cx, _size.cy, SWP_NOMOVE | SWP_NOREDRAW | SWP_NOZORDER);

    return TRUE;
}

void message_box::OnDestroy()
{
    finalize();

    CDialog::OnDestroy();
}

void message_box::OnSize(unsigned int nType, int cx, int cy)
{
    CDialog::OnSize(nType, cx, cy);

    CRect rect, temp;
    GetClientRect(&rect);
    rect.DeflateRect(5, 5);

    if (::IsWindow(_stcString.GetSafeHwnd())) {
        temp = rect;
        temp.bottom = rect.top + rect.Height() / 2;
        _stcString.MoveWindow(temp);
    }

    if (::IsWindow(_stcMessage.GetSafeHwnd())) {
        temp = rect;
        temp.top = rect.top + rect.Height() / 2;
        _stcMessage.MoveWindow(temp);
    }
}

BOOL message_box::OnEraseBkgnd(CDC* pDC)
{
    CRect rect;
    GetClientRect(&rect);

    pDC->FillSolidRect(rect, RGB(93, 93, 93));
    pDC->Draw3dRect(rect, RGB(255, 255, 255), RGB(255, 255, 255));

    return TRUE;
}

HBRUSH message_box::OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor)
{
    HBRUSH hbr = CWnd::OnCtlColor(pDC, pWnd, nCtlColor);

    if (pWnd->GetDlgCtrlID() == IDCC_STC_MESSAGE ||
        pWnd->GetDlgCtrlID() == IDCC_STC_DESCRIPTION) {
        pDC->SetBkMode(TRANSPARENT);
        pDC->SetTextColor(RGB(255, 255, 255));
        hbr = (HBRUSH)_brush.GetSafeHandle();
    }
    return hbr;
}

void message_box::OnTimer(UINT_PTR nIDEvent)
{
    if (nIDEvent == TIMER_ID_POSITION_CHECK) {
        CWnd* parnet = _parent;
        if (parnet == NULL) {
            parnet = foundation_ptr();
        }
        CenterWindow(_parent);
    }

    if (nIDEvent == TIMER_ID_AUTO_HIDE) {
        KillTimer(TIMER_ID_AUTO_HIDE);
        hide();
    }
}

//////////////////////////////////////////////////////////////////////////

void message_box::initialize(void)
{
    G2RETURN_IF_FAIL(this != NULL &&
                     ::IsWindow(GetSafeHwnd()));

    create_font(_font);
    _brush.CreateSolidBrush(RGB(93, 93, 93));

    /////////////////////////////////////////////

    CRect rect;
    GetClientRect(&rect);

    _stcString.Create(_T(""),
                        WS_CHILD | WS_CLIPCHILDREN | WS_CLIPSIBLINGS | WS_VISIBLE |
                        SS_CENTER | SS_ENDELLIPSIS | SS_CENTERIMAGE | SS_NOPREFIX,
                        rect,
                        this,
                        IDCC_STC_MESSAGE);

    _stcMessage.Create(_T(""),
                        WS_CHILD | WS_CLIPCHILDREN | WS_CLIPSIBLINGS | WS_VISIBLE |
                        SS_CENTER | SS_ENDELLIPSIS | SS_CENTERIMAGE | SS_NOPREFIX,
                        rect,
                        this,
                        IDCC_STC_DESCRIPTION);

    _stcString.SetFont(&_font);
    _stcMessage.SetFont(&_font);
}

void message_box::finalize(void)
{
    if (_font.GetSafeHandle()) {
        _font.DeleteObject();
    }

    if (_brush.GetSafeHandle()) {
        _brush.DeleteObject();
    }
}

//////////////////////////////////////////////////////////////////////////

void message_box::set_string(const std::wstring& string)
{
    _stcString.SetWindowText(string.c_str());
    _string = string;
}

void message_box::set_message(const std::wstring& message)
{
    _stcMessage.SetWindowText(message.c_str());
    _message = message;
}

void message_box::set_size(const CSize& size)
{
    if (_size != size) {
        _size = size;
        SetWindowPos(NULL, 0, 0, size.cx, size.cy, SWP_NOMOVE | SWP_NOREDRAW | SWP_NOZORDER);
    }
}

void message_box::show(bool message /*= false*/, bool autohide /*= true*/, int elapse /*= 5000*/)
{
    G2RETURN_IF_ASSERT(_string.empty() != true);

    CRect rect;
    GetClientRect(&rect);
    rect.DeflateRect(5, 5);

    if (message) {
        rect.bottom = rect.top + rect.Height() / 2;
        _stcString.MoveWindow(rect);
    }
    else {
        _stcString.MoveWindow(rect);
    }
    _stcMessage.ShowWindow(message ? SW_SHOW : SW_HIDE);

    /////////////////////////////////////////////

    CWnd* parent = _parent;
    if (parent == NULL) {
        parent = foundation_ptr();
    }
    CenterWindow(parent);

    ShowWindow(SW_SHOW);

    SetTimer(TIMER_ID_POSITION_CHECK, 200, NULL);

    if (autohide) {
        SetTimer(TIMER_ID_AUTO_HIDE, elapse, NULL);
    }
}

void message_box::hide(void)
{
    ShowWindow(SW_HIDE);

    KillTimer(TIMER_ID_POSITION_CHECK);
}
