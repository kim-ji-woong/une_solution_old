// search_event_list.cpp : implementation file
//

#include "stdafx.h"
#include "search_event_list.h"
#include "listener/listener_search_event_list.h"
#include <common/client_functional.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

search_event_list::search_event_list(void)
    : _parent(NULL)
    , _listener(NULL)
    , _initialized(false)
    , _enable(false)
    , _hostId(client::invalid_::HOST_ID)
{

}

search_event_list::~search_event_list(void)
{

}

//////////////////////////////////////////////////////////////////////////

IMPLEMENT_DYNAMIC(search_event_list, CWnd)

void search_event_list::DoDataExchange(CDataExchange* pDX)
{
    CWnd::DoDataExchange(pDX);
}

//////////////////////////////////////////////////////////////////////////

BEGIN_MESSAGE_MAP(search_event_list, CWnd)
    ON_WM_CREATE()
    ON_WM_DESTROY()
    ON_WM_SIZE()
    ON_WM_ENABLE()
    ON_WM_TIMER()
    ON_COMMAND(control_::BTN_NEXT, OnBtnNext)
    ON_NOTIFY(LVN_ITEMCHANGED, control_::EVENT_LIST_CTRL, OnLstItemChanged)

    ON_MESSAGE(message_::UM_RECEIVE_RESULT_QUERY, on_receive_result_query)
    ON_MESSAGE(message_::UM_LIST_CLEAR, on_clear_list)
END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////

int search_event_list::OnCreate(LPCREATESTRUCT lpCreateStruct)
{
    if (CWnd::OnCreate(lpCreateStruct) == -1) {
        return -1;
    }

    initialize();

    return 0;
}

void search_event_list::OnDestroy()
{
    CWnd::OnDestroy();

    finalize();
}

void search_event_list::OnSize(UINT nType, int cx, int cy)
{
    CWnd::OnSize(nType, cx, cy);

    if (_list.GetSafeHwnd()) {
        CRect rect;
        GetClientRect(&rect);
        rect.right -= 50;

        _list.SetWindowPos(NULL, 0, 0, rect.Width(), rect.Height(), SWP_NOMOVE | SWP_NOZORDER);
        _btnNext.SetWindowPos(NULL, rect.right + 10, 0, 0, 0, SWP_NOSIZE | SWP_NOZORDER);
    }
}

void search_event_list::OnEnable(BOOL bEnable)
{
    CWnd::OnEnable(bEnable);

    if (_list.GetSafeHwnd()) {
        _list.EnableWindow(bEnable);
        _btnNext.EnableWindow(bEnable && _list.GetItemCount() >= 100);
    }
}

void search_event_list::OnTimer(UINT_PTR nIDEvent)
{
    CWnd::OnTimer(nIDEvent);

    if (nIDEvent == timer_::id_::LOAD_EVENT_IMAGE) {
        KillTimer(nIDEvent);

        int selected = -1;
        POSITION pos = _list.GetFirstSelectedItemPosition();
        int index = (pos) ? _list.GetNextSelectedItem(pos) : -1;
        if (index != -1) {
            const item_data* data = (item_data*)_list.GetItemData(index);
            if (data) {
                selected = data->_index;
            }
        }

        if (selected >= 0) {
            if (_listener) {
                _listener->on_request_load_event_image_search(selected, false);
            }
        }
    }
}

//////////////////////////////////////////////////////////////////////////

bool search_event_list::create(CWnd* parent, const CRect& rect, unsigned int id)
{
    if (parent == NULL ||
        ::IsWindow(parent->GetSafeHwnd()) != TRUE) {
        return false;
    }

    _parent = parent;

    BOOL created = CWnd::Create(NULL,
                                _T("g2client search event list"),
                                WS_VISIBLE | WS_CHILD,
                                rect,
                                parent,
                                id);
    if (created) {

    }
    else {
        assert(!"failed to create event list");
    }
    return (created == TRUE);
}

void search_event_list::set_listenr(listener_search_event_list* listener)
{
    _listener = listener;
}

void search_event_list::set_host_id(short hostId)
{
    G2RETURN_IF_FAIL(_hostId != hostId);

    _hostId = hostId;

    clear(true);
}

void search_event_list::clear(bool post /*= false*/)
{
    if (_list.GetItemCount() == 0) return;

    if (post == true) {
        PostMessage(message_::UM_LIST_CLEAR);
        return;
    }

    for (int i = 0, count = _list.GetItemCount(); i < count; ++i) {
        item_data* data = (item_data*)_list.GetItemData(i);
        SAFE_DELETE(data);
    }
    _list.DeleteAllItems();
}

void search_event_list::set_enable(bool enable)
{
    G2RETURN_IF_FAIL(GetSafeHwnd());

    EnableWindow(enable ? TRUE : FALSE);
}

void search_event_list::show(bool show)
{
    G2RETURN_IF_FAIL(GetSafeHwnd());

    ShowWindow(show ? SW_SHOW : SW_HIDE);
}

void search_event_list::focus(void)
{
    G2RETURN_IF_FAIL(_list.GetSafeHwnd());

    _list.SetFocus();
}

//////////////////////////////////////////////////////////////////////////

void search_event_list::initialize(void)
{
    CRect rect;
    GetClientRect(&rect);
    rect.right -= 50;

    client::create_font(_font);
    
    BOOL created = _list.Create(WS_CHILD | WS_VISIBLE | WS_CLIPCHILDREN | WS_CLIPSIBLINGS | WS_VSCROLL | WS_HSCROLL |
                                LVS_REPORT | LVS_SINGLESEL | LVS_SHOWSELALWAYS,
                                rect,
                                this,
                                control_::EVENT_LIST_CTRL);

    if (created) {
        _list.SetExtendedStyle(LVS_EX_FULLROWSELECT | LVS_EX_GRIDLINES);
        _list.SetFont(&_font);

        _list.InsertColumn(column_::EVENT, L"Event", LVCFMT_LEFT, 200);
        _list.InsertColumn(column_::CONTENT, L"Device", LVCFMT_LEFT, 250);
        _list.InsertColumn(column_::TIME, L"Time", LVCFMT_LEFT, 150);
    }

    _btnNext.Create(L"Next", WS_CHILD | WS_VISIBLE | WS_CLIPCHILDREN | WS_CLIPSIBLINGS, CRect(CPoint(rect.right + 10, 0), CSize(40, 20)), this, control_::BTN_NEXT);
    _btnNext.SetFont(&_font);
    _btnNext.EnableWindow(FALSE);

    _initialized = true;
}

void search_event_list::finalize(void)
{
    _initialized = false;

    if (_font.GetSafeHandle()) {
        _font.DeleteObject();
    }

    clear();
}

//////////////////////////////////////////////////////////////////////////

void search_event_list::OnBtnNext(void)
{
    if (_listener) {
        _listener->on_request_more_event_search();
    }
}

void search_event_list::OnLstItemChanged(NMHDR* pNMHDR, LRESULT* pResult)
{
    LPNMLISTVIEW pNMLV = reinterpret_cast<LPNMLISTVIEW>(pNMHDR);
    *pResult = 0;

    if ((pNMLV->uChanged & LVIF_STATE) &&
        (pNMLV->uNewState & LVIS_SELECTED)) {
        int selected = pNMLV->iItem;
        if (selected >= 0) {
            SetTimer(timer_::id_::LOAD_EVENT_IMAGE, 200, NULL);
        }
    }
}

LRESULT search_event_list::on_receive_result_query(WPARAM wParam, LPARAM lParam)
{
    _list.SetRedraw(FALSE);

    clear();

    int index = 0, logIndex = 0;
    LVCOLUMN column = { 0 };

    int mode = static_cast<int>(wParam);

    if (mode == G2SEARCH_QUERY::EVENT) {        
        column.mask = LVCF_FMT;
        if (_list.GetColumn(column_::CONTENT, &column)) {
            column.mask = LVCF_TEXT;
            column.pszText = L"Device";
            _list.SetColumn(column_::CONTENT, &column);
        }

        g2::scoped_criticalsection lock(_syncEvent);

        G2STRING_64 strEvent = { 0 };
        
        for (std::vector<G2SEARCH_LOG_INFO>::const_iterator itr(_eventLog.begin());
             itr != _eventLog.end();
             ++itr, ++logIndex) {
            const G2SEARCH_LOG_INFO& log = *itr;

            g2_get_string_event_type(log._event_level1, log._event_level2, &strEvent);

            index = _list.InsertItem(0, strEvent._string);
            if (index != -1) {
                _list.SetItemText(index, 1, log._label._string);
                _list.SetItemText(index, 2, CTime(g2_time_to_time32_t(&log._time)).Format(_T("%Y-%m-%d %H:%M:%S")));
                _list.SetItemData(index, (DWORD_PTR)new item_data(logIndex));
            }
        }
        std::vector<G2SEARCH_LOG_INFO>().swap(_eventLog);
        lock.free();
    }
    else if (mode == G2SEARCH_QUERY::TEXT_IN) {
        column.mask = LVCF_FMT;
        if (_list.GetColumn(column_::CONTENT, &column)) {
            column.mask = LVCF_TEXT;
            column.pszText = L"Transaction";
            _list.SetColumn(column_::CONTENT, &column);
        }

        g2::scoped_criticalsection lock(_syncTextIn);
        for (std::vector<G2TEXT_IN>::const_iterator itr(_textInLog.begin());
             itr != _textInLog.end();
             ++itr, ++logIndex) {
            const G2TEXT_IN& log = *itr;

            index = _list.InsertItem(0, L"Text-In");
            if (index != -1) {
                _list.SetItemText(index, 1, CString(log._data));
                _list.SetItemText(index, 2, CTime(g2_time_to_time32_t(&log._index._spot._time)).Format(_T("%Y-%m-%d %H:%M:%S")));
                _list.SetItemData(index, (DWORD_PTR)new item_data(logIndex));
            }
        }
        std::vector<G2TEXT_IN>().swap(_textInLog);
        lock.free();
    }   

    if (_list.GetItemCount() > 0) {
        _list.SetSelectionMark(0);
        _list.SetItemState(0, LVIS_SELECTED | LVIS_FOCUSED, LVIS_SELECTED | LVIS_FOCUSED);

        if (const item_data* data = (item_data*)_list.GetItemData(0)) {
            int selected = data->_index;
            if (selected >= 0) {
                if (_listener) {
                    _listener->on_request_load_event_image_search(selected, false);
                }
            }
        }
    }
    _list.SetRedraw(TRUE);

    _btnNext.EnableWindow((_list.GetItemCount() >= 100) ? TRUE : FALSE);

    return 0L;
}

LRESULT search_event_list::on_clear_list(WPARAM wParam, LPARAM lParam)
{
    clear();

    return 0L;
}

//////////////////////////////////////////////////////////////////////////

void search_event_list::insert(const std::vector<G2SEARCH_LOG_INFO>& info)
{
    G2RETURN_IF_FAIL(info.empty() != true);

    g2::scoped_criticalsection lock(_syncEvent);
    _eventLog.insert(_eventLog.begin(), info.begin(), info.end());
    lock.free();

    PostMessage(message_::UM_RECEIVE_RESULT_QUERY, WPARAM(G2SEARCH_QUERY::EVENT));
}

void search_event_list::insert(const std::vector<G2TEXT_IN>& info)
{
    G2RETURN_IF_FAIL(info.empty() != true);

    g2::scoped_criticalsection lock(_syncTextIn);
    _textInLog.insert(_textInLog.begin(), info.begin(), info.end());
    lock.free();

    PostMessage(message_::UM_RECEIVE_RESULT_QUERY, WPARAM(G2SEARCH_QUERY::TEXT_IN));
}
