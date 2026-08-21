// search_event_progress.cpp : implementation file
//

#include "stdafx.h"
#include "search_event_progress.h"

#include <sampler/cpp/g2client_search.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
#ifdef _UNITY
#define THIS_FILE __FILE__
#else
static char THIS_FILE[] = __FILE__;
#endif
#endif

#pragma warning(push)
#pragma warning(disable : 4244)

using namespace client;

//////////////////////////////////////////////////////////////////////////

search_event_progress::search_event_progress(CWnd* parent /*= NULL*/)
    : CDialog(IDD_SEARCH_EVENT_PROGRESS, parent)
    , _parent(parent)
    , _progressing(false)
    , _channel(client::invalid_::CHANNEL)
    , _previous(0)
    , _client(NULL)
{

}

search_event_progress::~search_event_progress(void)
{

}

//////////////////////////////////////////////////////////////////////////

void search_event_progress::DoDataExchange(CDataExchange* pDX)
{
    CDialog::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(search_event_progress, CDialog)
    ON_WM_TIMER()
    ON_WM_SHOWWINDOW()
    ON_BN_CLICKED(IDC_SEARCH_EVENT_BTN_STOP, OnBtnStop)
    ON_MESSAGE(message_::POST_DESTROY, OnPostDestroy)
END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////

BOOL search_event_progress::PreTranslateMessage(MSG* pMsg)
{
    bool result = false;
    if (pMsg->message == WM_KEYDOWN) {
        switch (pMsg->wParam) {
        case VK_ESCAPE:
        case VK_RETURN:
            result = true;
            break;
        default:
            break;
        }
    }
    return (result) ? TRUE : CDialog::PreTranslateMessage(pMsg);
}

BOOL search_event_progress::OnInitDialog()
{
    CDialog::OnInitDialog();

    ModifyStyle(0, WS_CLIPCHILDREN | WS_CLIPSIBLINGS);

    CSliderCtrl* slider = (CSliderCtrl*)GetDlgItem(IDC_SEARCH_EVENT_PRG_RATIO);
    if (slider) {
        slider->SetRange(0, 100);
        slider->SetPos(0);
    }

    SetDlgItemText(IDC_SEARCH_EVENT_STC_STRING, _T("Searching"));
    SetDlgItemText(IDC_SEARCH_EVENT_STC_RATIO, _T(" 0%%"));

    return TRUE;
}

void search_event_progress::OnTimer(UINT_PTR nIDEvent)
{
    CDialog::OnTimer(nIDEvent);

    if (nIDEvent == timer_::id_::CHECK_POSITION) {
        if (_parent) {
            ::SetWindowPos(safe_handle(), HWND_TOP, 0, 0, 0, 0,
                           SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_NOOWNERZORDER);
        }
    }
}

void search_event_progress::OnShowWindow(BOOL bShow, UINT nStatus)
{
    CDialog::OnShowWindow(bShow, nStatus);

    if (bShow) {
        SetTimer(timer_::id_::CHECK_POSITION, 100, NULL);
    }
    else {
        KillTimer(timer_::id_::CHECK_POSITION);
    }
}

void search_event_progress::OnBtnStop()
{
    G2RETURN_IF_ASSERT(client::valid_channel(_channel));

    if (g2search* client = dynamic_cast<g2search*>(_client)) {
        client->request_event_search_stop_idr(_channel);
    }
}

LRESULT search_event_progress::OnPostDestroy(WPARAM wParam, LPARAM lParam)
{
    if (safe_handle()) {
        show(false);
        destroy(false);
    }
    return 0L;
}

//////////////////////////////////////////////////////////////////////////

bool search_event_progress::create(CWnd* parent)
{
    destroy();

    assert(parent != NULL);

    _parent = parent;

    return (CDialog::Create(IDD_SEARCH_EVENT_PROGRESS, parent) == TRUE);
}

void search_event_progress::destroy(bool post /*= false*/)
{
    if (safe_handle()) return;
    if (post) {
        PostMessage(message_::POST_DESTROY);
    }
    else {
        DestroyWindow();
    }
}

void search_event_progress::range(const G2TIME& begin, const G2TIME& end)
{
    G2RETURN_IF_ASSERT(safe_handle());

    if (g2_time_is_valid(&begin) && g2_time_is_valid(&end)) {
        _begin = begin._time;
        _end = end._time;

        SendMessage(IDC_SEARCH_EVENT_PRG_RATIO, PBM_SETPOS, 0);

        GetDlgItem(IDC_SEARCH_EVENT_PRG_RATIO)->ShowWindow(SW_SHOW);
        GetDlgItem(IDC_SEARCH_EVENT_STC_RATIO)->ShowWindow(SW_SHOW);
        _progressing = true;
    }
    else {
        GetDlgItem(IDC_SEARCH_EVENT_PRG_RATIO)->ShowWindow(SW_HIDE);
        GetDlgItem(IDC_SEARCH_EVENT_STC_RATIO)->ShowWindow(SW_HIDE);
        _progressing = false;
    }

    _previous = 0;
}

void search_event_progress::channel(g2search* adaptor, short channel)
{
    _client  = adaptor;
    _channel = channel;
}

void search_event_progress::progress(const G2TIME& time)
{
    G2RETURN_IF_FAIL(safe_handle());

    if (_previous == time._time) return;

    _previous = time._time;

    CString string;
    string.Format(_T("Search %s"), CTime(g2_time_to_time32_t(&time)).Format(_T("%Y-%m-%d %H:%M:%S")));

    SetDlgItemText(IDC_SEARCH_EVENT_STC_STRING, string);

    if (_progressing) {
        unsigned int pos = _previous - _begin;
        unsigned int rat = static_cast<int>(static_cast<float>(pos) / static_cast<float>(_end - _begin) * 100.0F + 0.5F);
        SendMessage(IDC_SEARCH_EVENT_PRG_RATIO, PBM_SETPOS, rat);
        string.Format(_T("%2d%%"), rat);
        SetDlgItemText(IDC_SEARCH_EVENT_STC_RATIO, string);
    }
}

void search_event_progress::show(bool show)
{
    G2RETURN_IF_FAIL(safe_handle());

    if (_parent) {
        CenterWindow(_parent);
    }

    ShowWindow(show ? SW_SHOW : SW_HIDE);
}

//////////////////////////////////////////////////////////////////////////

#pragma warning(pop)
