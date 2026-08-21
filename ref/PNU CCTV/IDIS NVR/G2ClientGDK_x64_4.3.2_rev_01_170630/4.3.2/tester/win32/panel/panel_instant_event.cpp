// panel_instant_event.cpp : implementation
//

#include "stdafx.h"
#include "G2Client.h"
#include "MainFrm.h"
#include "panel_instant_event.h"
#include "../include/g2_guid.h"

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
    IDCC_INSTANT_EVENT_LIST = 1024
};

enum TIMER_ID {
    TIMER_ID_REMOVE_LIST_ITEM = 1024
};

enum CONST_DATA {
    REMOVE_LIST_ITEM_TIMER_INTERVAL = 1000 * 60 * 5, // 5 minute
    REMOVE_LIST_ITEM_TIME_BOUND = 60 * 60 // 1 hour
};

//////////////////////////////////////////////////////////////////////////

panel_instant_event::panel_instant_event(void)
    : _parent(NULL)
{

}

panel_instant_event::~panel_instant_event(void)
{

}

//////////////////////////////////////////////////////////////////////////

void panel_instant_event::DoDataExchange(CDataExchange* pDX)
{
    CDialog::DoDataExchange(pDX);
}

//////////////////////////////////////////////////////////////////////////

BEGIN_MESSAGE_MAP(panel_instant_event, CDialog)
    ON_WM_CREATE()
    ON_WM_DESTROY()
    ON_WM_SIZE()
    ON_WM_ERASEBKGND()

    ON_MESSAGE(UM_POST_APPEND_EVENT, on_post_append_event)
END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////

bool panel_instant_event::create(CWnd* parent, unsigned int id)
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

void panel_instant_event::cleanup(void)
{

}

//////////////////////////////////////////////////////////////////////////

BOOL panel_instant_event::PreTranslateMessage(MSG* pMsg)
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

int panel_instant_event::OnCreate(LPCREATESTRUCT lpcs)
{
    if (CDialog::OnCreate(lpcs) == -1) {
        return -1;
    }

    initialize();

    return 0;
}

BOOL panel_instant_event::OnInitDialog()
{
    CDialog::OnInitDialog();

    return TRUE;
}

void panel_instant_event::OnDestroy()
{
    finalize();

    CDialog::OnDestroy();
}

void panel_instant_event::OnSize(unsigned int nType, int cx, int cy)
{
    CDialog::OnSize(nType, cx, cy);

    if (::IsWindow(_list.GetSafeHwnd())) {
        CRect rect;
        GetClientRect(rect);
        _list.MoveWindow(rect);
    }
}

BOOL panel_instant_event::OnEraseBkgnd(CDC* pDC)
{
    return CDialog::OnEraseBkgnd(pDC);
}

void panel_instant_event::OnTimer(UINT_PTR nIDEvent)
{
    if (nIDEvent == TIMER_ID_REMOVE_LIST_ITEM) {
        remove_event_item();
    }
}

//////////////////////////////////////////////////////////////////////////

void panel_instant_event::initialize(void)
{
    CRect rect;
    GetClientRect(&rect);

    BOOL created = _list.Create(WS_CHILD | WS_VISIBLE | WS_CLIPCHILDREN | WS_CLIPSIBLINGS |
                                LVS_REPORT | LVS_SINGLESEL | LVS_NOSORTHEADER | LVS_SHOWSELALWAYS,
                                rect,
                                this,
                                IDCC_INSTANT_EVENT_LIST);
    if (created != TRUE) {
        assert(!"failed to create event list");
        return;
    }

    _list.ModifyStyle(WS_HSCROLL, WS_VSCROLL);
    _list.SetExtendedStyle(_list.GetExtendedStyle() | LVS_EX_FULLROWSELECT | LVS_EX_SUBITEMIMAGES);

    _list.InsertColumn(COLUMN_EVENT, _T("Event"), LVCFMT_LEFT, 20);
    _list.InsertColumn(COLUMN_CAMERA, _T("Camera"), LVCFMT_LEFT, 60);
    _list.InsertColumn(COLUMN_DEVICE, _T("Device"), LVCFMT_LEFT, 50);
    _list.InsertColumn(COLUMN_TIME, _T("Time"), LVCFMT_LEFT, 60);

    client::foundation_ptr()->set_instant_eventer(this);

    SetTimer(TIMER_ID_REMOVE_LIST_ITEM, REMOVE_LIST_ITEM_TIMER_INTERVAL, NULL);
}

void panel_instant_event::finalize(void)
{
    KillTimer(TIMER_ID_REMOVE_LIST_ITEM);

    clear();
}

//////////////////////////////////////////////////////////////////////////

LRESULT panel_instant_event::on_post_append_event(WPARAM wParam, LPARAM lParam)
{
    g2::scoped_criticalsection lock(_lock_exec);

    G2RETURN_VAL_IF_FAIL(wParam != NULL, 1L);
    G2RETURN_VAL_IF_FAIL(lParam != NULL, 1L);

    G2DEVICE_STATUS* wp = reinterpret_cast<G2DEVICE_STATUS*>(wParam);
    G2EVENT_INFO* lp = reinterpret_cast<G2EVENT_INFO*>(lParam);
    G2DEVICE_STATUS status = *wp;
    G2EVENT_INFO info = *lp;
    SAFE_DELETE(wp);
    SAFE_DELETE(lp);

    G2STRING_64 strEvent = { 0 };
    g2_get_string_event_type(info._level1, info._level2, &strEvent);

    int index = _list.InsertItem(0, strEvent._string);
    if (index >= 0) {
        _list.SetItemText(index, COLUMN_CAMERA, info._label._string);
        _list.SetItemText(index, COLUMN_DEVICE, status._sitename._string);

        CTime time(g2_time_to_time32_t(&info._spot._time));
        _list.SetItemText(index, COLUMN_TIME, time.Format(_T("%m-%d %H:%M:%S")));

        event_list_ctrl::event_data* data = new event_list_ctrl::event_data(info);
        _list.SetItemData(index, (DWORD_PTR)data);
    }

    return 0L;
}

void panel_instant_event::remove_event_item(void)
{
    g2::scoped_criticalsection lock(_lock_exec);

    G2RETURN_IF_FAIL(_list.GetItemCount() < 100);

    COleDateTime timeBound = COleDateTime::GetCurrentTime() - COleDateTimeSpan(REMOVE_LIST_ITEM_TIME_BOUND);

    for (int i = _list.GetItemCount() - 1; i >= 0; --i) {
        event_list_ctrl::event_data* data = (event_list_ctrl::event_data*)(_list.GetItemData(i));
        if (data) {
            if (data->_time > timeBound) {
                break;
            }
            SAFE_DELETE(data);
        }
        _list.DeleteItem(i);
    }
}

//////////////////////////////////////////////////////////////////////////

void panel_instant_event::append_event(const G2EVENT_INFO& info, const G2DEVICE_STATUS& status)
{
    G2EVENT_INFO* event = new G2EVENT_INFO;
    *event = info;

    G2DEVICE_STATUS* s = new G2DEVICE_STATUS;
    *s = status;

    PostMessage(UM_POST_APPEND_EVENT, WPARAM(s), LPARAM(event));
}

void panel_instant_event::clear(void)
{
    g2::scoped_criticalsection lock(_lock_exec);

    if (::IsWindow(_list.GetSafeHwnd()) &&
        _list.GetItemCount() > 0) {
        for (int i = _list.GetItemCount() - 1; i >= 0; --i) {
            event_list_ctrl::event_data* data = (event_list_ctrl::event_data*)(_list.GetItemData(i));
            SAFE_DELETE(data);
        }
        _list.DeleteAllItems();
    }
}
