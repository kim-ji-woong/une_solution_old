// search_condition_dlg.cpp : implementation file
//

#include "stdafx.h"
#include "search_condition_dlg.h"

#include <include/g2_define_admin.h>
#include <sampler/cpp/g2client_admin.h>
#include <utility/g2channels.h>
#include <algorithm>
#include <functional>
#include <boost/bind.hpp>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

search_condition_dlg::search_condition_dlg(CWnd* parent /*=NULL*/)
    : CDialog(IDD_SEARCH_CONDITION, parent)
    , _parent(parent)
    , _mode(G2SEARCH_QUERY::UNDEFINED)
    , _cameras(0)
    , _idr(false)
{

}

search_condition_dlg::~search_condition_dlg(void)
{

}

//////////////////////////////////////////////////////////////////////////

void search_condition_dlg::DoDataExchange(CDataExchange* pDX)
{
    CDialog::DoDataExchange(pDX);

    DDX_Control(pDX, IDC_CHK_SEARCH_FIRST,              _chkFirst);
    DDX_Control(pDX, IDC_CHK_SEARCH_LAST,               _chkLast);
    DDX_Control(pDX, IDC_DTP_SEARCH_FROM_DATE,          _dtcFromDate);
    DDX_Control(pDX, IDC_DTP_SEARCH_FROM_TIME,          _dtcFromTime);
    DDX_Control(pDX, IDC_DTP_SEARCH_TO_DATE,            _dtcToDate);
    DDX_Control(pDX, IDC_DTP_SEARCH_TO_TIME,            _dtcToTime);
    DDX_Control(pDX, IDC_SEARCH_CONDITION_DEVICE_TREE,  _tree);
    DDX_Control(pDX, IDC_RDO_MODE_EVENT,                _rdoEvent);
    DDX_Control(pDX, IDC_RDO_MODE_TEXTIN,               _rdoTextIn);
    DDX_Control(pDX, IDC_STC_QUERY_CONTROL,             _stcCotnrol);
}

BEGIN_MESSAGE_MAP(search_condition_dlg, CDialog)
    ON_WM_TIMER()
    ON_COMMAND(IDC_CHK_SEARCH_FIRST,                            on_chk_first)
    ON_COMMAND(IDC_CHK_SEARCH_LAST,                             on_chk_last)
    ON_COMMAND_RANGE(IDC_RDO_MODE_EVENT, IDC_RDO_MODE_TEXTIN,   on_select_mode)
    
    ON_NOTIFY(NM_CLICK, IDC_SEARCH_CONDITION_DEVICE_TREE,       OnNMClickTree)
    ON_MESSAGE(message_::TREE_CHANGE_CHECK,                     on_tree_change_check)
END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////

void search_condition_dlg::OnTimer(UINT_PTR nIDEvent)
{
    CDialog::OnTimer(nIDEvent);

    if (nIDEvent == timer_::id_::CHANGE_QUERY_MODE) {
        KillTimer(timer_::id_::CHANGE_QUERY_MODE);
        change_condition_mode(_mode);
    }
}

BOOL search_condition_dlg::OnInitDialog()
{
    CDialog::OnInitDialog();

    _chkFirst.SetCheck(BST_CHECKED);
    _chkLast.SetCheck(BST_CHECKED);
    _dtcFromDate.EnableWindow(FALSE);
    _dtcFromTime.EnableWindow(FALSE);
    _dtcToDate.EnableWindow(FALSE);
    _dtcToTime.EnableWindow(FALSE);

    if (_mode == G2SEARCH_QUERY::EVENT) {
        _rdoEvent.SetCheck(BST_CHECKED);
    }
    else {
        _rdoTextIn.SetCheck(BST_CHECKED);
    }
    
    CRect rect;
    _stcCotnrol.GetWindowRect(rect);
    ScreenToClient(rect);

    _eventCtrl.create(this, rect, control_::QUERY_CONTROL_EVENT);
    _textInCtrl.create(this, rect, control_::QUERY_CONTROL_TEXT_IN);

    _eventCtrl.set_query_condition(_conditionEvent);
    _textInCtrl.set_query_condition(_conditionTextIn);
    _textInCtrl.set_device_idr(_idr);

    /////////////////////////////////////////////

    if (_parent != NULL &&
        ::IsWindow(_parent->GetSafeHwnd())) {
        CenterWindow(_parent);
    }

    SetTimer(timer_::id_::CHANGE_QUERY_MODE, 250, NULL);

    return TRUE;
}

void search_condition_dlg::OnOK()
{
    CTime fromDate, fromTime, toDate, toTime;
    _dtcFromDate.GetTime(fromDate);
    _dtcFromTime.GetTime(fromTime);
    _dtcToDate.GetTime(toDate);
    _dtcToTime.GetTime(toTime);

    CTime fromDateTime(fromDate.GetYear(), fromDate.GetMonth(), fromDate.GetDay(), fromTime.GetHour(), fromTime.GetMinute(), fromTime.GetSecond());
    CTime toDateTime(toDate.GetYear(), toDate.GetMonth(), toDate.GetDay(), toTime.GetHour(), toTime.GetMinute(), toTime.GetSecond());

    if (_chkFirst.GetCheck() == BST_UNCHECKED && _chkLast.GetCheck() == BST_UNCHECKED) {
        if (fromDateTime >= toDateTime) {
            MessageBox(L"The end time must be later than the start time", NULL, MB_OK);
            return;
        }
    }
    if (_chkFirst.GetCheck() == BST_CHECKED) {
        fromDateTime = 0;
    }
    if (_chkLast.GetCheck() == BST_CHECKED) {
        toDateTime = 0;
    }

    g2_time_from_time32_t(&_begin, static_cast<time_t>(fromDateTime.GetTime()));
    g2_time_from_time32_t(&_end, static_cast<time_t>(toDateTime.GetTime()));

    //G2DEVICE_LEAF device;
    g2::channels cameras, alarmins, audios, textins;
    
    if (_mode == G2SEARCH_QUERY::EVENT) {
        for (std::map<HTREEITEM, treeItemData>::const_iterator itr(_treeItems.begin());
             itr != _treeItems.end();
             ++itr) {
             if (_tree.GetCheck(itr->first) == TRUE && itr->second._type != item_::ROOT) {
                 if (itr->second._type == item_::CAMERA)
                     cameras.add(itr->second._channel);
                 else if (itr->second._type == item_::ALARMIN)
                     alarmins.add(itr->second._channel);
                 else if (itr->second._type == item_::AUDIOIN)
                     audios.add(itr->second._channel);
                 else if (itr->second._type == item_::TEXTIN)
                     textins.add(itr->second._channel);
             }
        }

        _conditionEvent._begin = _begin;
        _conditionEvent._end = _end;

        if (_idr) {
            //_eventIdrCtrl.get_query_condition(_conditionEvent, cameras);
        }
        else {
            _eventCtrl.get_query_condition(_conditionEvent, cameras.to_uint32());
            _conditionEvent._alarm_in = alarmins.to_uint32();
            _conditionEvent._audio_in = audios.to_uint32();
            _conditionEvent._text_in = textins.to_uint32();

            if (cameras.empty()) {
                MessageBox(L"There is no selected device.", NULL, MB_OK);
                return;
            }
        }
        _cameras = cameras.to_uint32();
    }
    else if (_mode == G2SEARCH_QUERY::TEXT_IN) {
        for (std::map<HTREEITEM, treeItemData>::const_iterator itr(_treeItems.begin());
             itr != _treeItems.end();
             ++itr) {
             if (_tree.GetCheck(itr->first) == TRUE && itr->second._type != item_::ROOT) {
                 if (itr->second._type == item_::TEXTIN)
                     textins.add(itr->second._channel);
             }
        }

        if (textins.empty()) {
            MessageBox(L"There is no selected device.", NULL, MB_OK);
            return;
        }

        _textInCtrl.get_query_condition(_conditionTextIn);
        _conditionTextIn._begin = _begin;
        _conditionTextIn._end = _end;
        _conditionTextIn._channels = textins.to_uint32();
    }
    else {
        assert(!"Unknown query mode");
    }

    CDialog::OnOK();
}

void search_condition_dlg::OnNMClickTree(NMHDR* pNMHDR, LRESULT* pResult)
{
    DWORD pos = ::GetMessagePos();
    CPoint point(LOWORD(pos), HIWORD(pos));

    _tree.ScreenToClient(&point);

    UINT nFlags = 0;
    HTREEITEM item = _tree.HitTest(point, &nFlags);

    if (item) {
        if (nFlags & TVHT_ONITEMSTATEICON) {
            PostMessage(message_::TREE_CHANGE_CHECK, WPARAM(pNMHDR->hwndFrom), LPARAM(item));
        }
    }
    *pResult = 0;
}

void search_condition_dlg::on_chk_first(void)
{
    BOOL enable = (_chkFirst.GetCheck() == BST_CHECKED) ? FALSE : TRUE;

    _dtcFromDate.EnableWindow(enable);
    _dtcFromTime.EnableWindow(enable);
}

void search_condition_dlg::on_chk_last(void)
{
    BOOL enable = (_chkLast.GetCheck() == BST_CHECKED) ? FALSE : TRUE;

    _dtcToDate.EnableWindow(enable);
    _dtcToTime.EnableWindow(enable);
}

void search_condition_dlg::on_select_mode(UINT nID)
{
    G2SEARCH_QUERY::MODE mode = (nID == IDC_RDO_MODE_EVENT) ? G2SEARCH_QUERY::EVENT : G2SEARCH_QUERY::TEXT_IN;

    if (_mode != mode) {
        _mode = mode;
        change_condition_mode(mode);
    }
}

LRESULT search_condition_dlg::on_tree_change_check(WPARAM wParam, LPARAM lParam)
{
    HTREEITEM item = (HTREEITEM)lParam;
    G2RETURN_VAL_IF_FAIL(item != NULL, 1L);

    HTREEITEM parent = _tree.GetParentItem(item);
    std::map<HTREEITEM, treeItemData>::const_iterator citr = _treeItems.find(item);

    if (citr != _treeItems.end()) {
        BOOL checked = _tree.GetCheck(item);
        if (citr->second._type == item_::ROOT) {
            for (std::map<HTREEITEM, treeItemData>::const_iterator itr(_treeItems.begin());
                 itr != _treeItems.end();
                 ++itr) {
                _tree.SetCheck(itr->first, checked);
            }
        }
        else {
            BOOL rootCheck = TRUE;
            for (std::map<HTREEITEM, treeItemData>::const_iterator itr(_treeItems.begin());
                 itr != _treeItems.end();
                 ++itr) {
                if (itr->second._type != item_::ROOT && _tree.GetCheck(itr->first) != TRUE) {
                    rootCheck = FALSE;
                    break;
                }
            }

            if (parent) {
                _tree.SetCheck(parent, rootCheck);
            }
        }
    }

    return 0L;
}

//////////////////////////////////////////////////////////////////////////

void search_condition_dlg::set_condition_event(const G2EVENT_QUERY_CONDITION& condition)
{
    _conditionEvent = condition;
}

void search_condition_dlg::set_condition_text_in(const G2TEXT_IN_QUERY_CONDITION& condition)
{
    _conditionTextIn = condition;
}

void search_condition_dlg::set_query_mode(G2SEARCH_QUERY::MODE mode)
{
    if (mode == G2SEARCH_QUERY::UNDEFINED) {
        mode = G2SEARCH_QUERY::EVENT;
    }

    _mode = mode;
}

void search_condition_dlg::set_query_cameras(unsigned int cameras)
{
    _cameras = cameras;
}

void search_condition_dlg::set_device_info(const CString& root, const G2_PRODUCT_INFO& pi)
{
    _root= root;
    _productInfo = pi;
}

void search_condition_dlg::set_device_idr(bool idr)
{
    _idr = idr;
}

void search_condition_dlg::get_event_condition(G2EVENT_QUERY_CONDITION& condition)
{
    condition = _conditionEvent;
}

void search_condition_dlg::get_text_in_condition(G2TEXT_IN_QUERY_CONDITION& condition)
{
    condition = _conditionTextIn;
}

//////////////////////////////////////////////////////////////////////////

void search_condition_dlg::change_condition_mode(G2SEARCH_QUERY::MODE mode)
{
    if (mode == G2SEARCH_QUERY::EVENT) {
        if (_idr) {
            _eventCtrl.show(false);
        }
        else {
            _eventCtrl.show(true);
        }
        _textInCtrl.show(false);
    }
    else {
        _textInCtrl.show(true);
        _eventCtrl.show(false);
    }

    change_device_tree(mode);
}

void search_condition_dlg::change_device_tree(G2SEARCH_QUERY::MODE mode)
{
    G2RETURN_IF_FAIL(_root.IsEmpty() == FALSE);
    G2RETURN_IF_FAIL(_tree.GetSafeHwnd() != NULL);

    _tree.DeleteAllItems();
    std::map<HTREEITEM, treeItemData>().swap(_treeItems);

    HTREEITEM top = _tree.InsertItem(_root, TVI_ROOT);
    _treeItems.insert(std::make_pair(top, treeItemData(0, item_::ROOT)));
    
    int cameraCount = _productInfo.device.count_camera;
    int alarmCount = _productInfo.device.count_alarm_in;
    int audioCount = _productInfo.device.count_audio_in;
    int textInCount = _productInfo.device.count_text_in;

    if (mode == G2SEARCH_QUERY::EVENT) {
        CString child;
        for (int i = 0; i < cameraCount; ++i) {
            child.Format(L"Camera %d", i + 1);
            HTREEITEM item = _tree.InsertItem(child, top, TVI_LAST);
            _treeItems.insert(std::make_pair(item, treeItemData(i, item_::CAMERA)));
        }
        for (int i = 0; i < alarmCount; ++i) {
            child.Format(L"AlarmIn %d", i + 1);
            HTREEITEM item = _tree.InsertItem(child, top, TVI_LAST);
            _treeItems.insert(std::make_pair(item, treeItemData(i, item_::ALARMIN)));
        }
        for (int i = 0; i < audioCount; ++i) {
            child.Format(L"AudioIn %d", i + 1);
            HTREEITEM item = _tree.InsertItem(child, top, TVI_LAST);
            _treeItems.insert(std::make_pair(item, treeItemData(i, item_::AUDIOIN)));
        }
        for (int i = 0; i < textInCount; ++i) {
            child.Format(L"TextIn %d", i + 1);
            HTREEITEM item = _tree.InsertItem(child, top, TVI_LAST);
            _treeItems.insert(std::make_pair(item, treeItemData(i, item_::TEXTIN)));
        }
    }
    else {
        for (int i = 0; i < textInCount; ++i) {
            CString child;
            child.Format(L"TextIn %d", i + 1);
            HTREEITEM item = _tree.InsertItem(child, top, TVI_LAST);
            _treeItems.insert(std::make_pair(item, treeItemData(i, item_::TEXTIN)));
        }
    }

    _tree.SetCheck(top, TRUE);
    _tree.Expand(top, TVE_EXPAND);

    PostMessage(message_::TREE_CHANGE_CHECK, NULL, LPARAM(top));
}
