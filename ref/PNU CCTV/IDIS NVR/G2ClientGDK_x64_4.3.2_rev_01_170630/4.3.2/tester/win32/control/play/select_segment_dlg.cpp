// select_segment_dlg.cpp : implementation file
//

#include "stdafx.h"
#include "select_segment_dlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

enum { IDD = IDD_SELECT_SEGMENT };

//////////////////////////////////////////////////////////////////////////

select_segment_dlg::select_segment_dlg(CWnd* parent /*=NULL*/)
    : CDialog(IDD, parent)
    , _mode(SELECT_MODE_SCOPE)
    , _parent(parent)
    , _selected(-1)
    , _count(0)
{

}

select_segment_dlg::~select_segment_dlg(void)
{

}

//////////////////////////////////////////////////////////////////////////

void select_segment_dlg::DoDataExchange(CDataExchange* pDX)
{
    CDialog::DoDataExchange(pDX);
    DDX_Control(pDX, IDC_LST_SELECT_SEGMENT, _list);
}

BEGIN_MESSAGE_MAP(select_segment_dlg, CDialog)
    ON_NOTIFY(NM_CLICK, IDC_LST_SELECT_SEGMENT, OnClickList)
END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////

BOOL select_segment_dlg::OnInitDialog()
{
    CDialog::OnInitDialog();

    CRect rect;
    _list.GetClientRect(&rect);

    CString strTitle;
    CString strColumn;

    if (_mode == SELECT_MODE_SCOPE) {
        strTitle  = L"Segment scope list";
        strColumn = L"Segment scope list";
    }
    else if (_mode == SELECT_MODE_ID) {
        strTitle  = L"Select a segment";
        strColumn = L"Segment";
    }
    else {
        strTitle  = L"Segment spot list";
        strColumn = L"Segment spot list";
    }

    SetWindowText(strTitle);
    _list.InsertColumn(0, strColumn, LVCFMT_LEFT, rect.Width());

    /////////////////////////////////////////////

    int index = 0;
    CString strText;

    if (_mode == SELECT_MODE_ID) {
        assert(_count > 0);
        _count = (_count < 1) ? 1 : _count;

        for (int i = 0; i < _count; ++i) {
            strText.Format(_T("%d"), index + 1);
            _list.InsertItem(index++, strText);
        }
    }
    else {
        for (SpotList::const_iterator itr(_spotList.begin());
             itr != _spotList.end();
             ++itr) {
            if (_mode == SELECT_MODE_SCOPE) {
                strText.Format(_T("%d : %s"),
                               index + 1,
                               CTime(g2_time_to_time32_t(&itr->_time)).Format(_T("%Y-%m-%d %H:%M:%S")).operator LPCTSTR());
            }
            else if (_mode == SELECT_MODE_SPOT) {
                strText.Format(_T("%d"), index + 1);
            }

            _list.InsertItem(index++, strText);
        }
    }

    if (_selected >= 0 &&
        _selected < _list.GetItemCount()) {

    }
    else {
        _selected = 0;
    }

    _list.SetItemState(_selected, LVIS_SELECTED | LVIS_FOCUSED, LVIS_SELECTED | LVIS_FOCUSED);

    /////////////////////////////////////////////

    if (_parent != NULL &&
        ::IsWindow(_parent->GetSafeHwnd())) {
        CenterWindow(_parent);
    }

    return TRUE;
}

//////////////////////////////////////////////////////////////////////////

void select_segment_dlg::set_scope_info(const std::vector<G2SCOPE>& list)
{
    int segmentId = -1;
    for (ScopeList::const_iterator itr(list.begin());
         itr != list.end();
         ++itr) {
        if (segmentId < 0 ||
            segmentId != itr->_begin._segment) {
            segmentId = itr->_begin._segment;
            _spotList.push_back(itr->_begin);
        }
    }
}

void select_segment_dlg::set_spot_info(const std::vector<G2SPOT>& list)
{
    for (SpotList::const_iterator itr(list.begin());
         itr != list.end();
         ++itr) {
        _spotList.push_back(*itr);
    }
}

void select_segment_dlg::OnClickList(NMHDR *pNMHDR, LRESULT *pResult)
{
    NM_LISTVIEW* pNMListView = (NM_LISTVIEW *)pNMHDR;
    _selected = pNMListView->iItem;
    GetDlgItem(IDOK)->EnableWindow(_selected >= 0);

    *pResult = 0;
}

void select_segment_dlg::OnOK()
{
    if (_selected == -1) {
        MessageBox(L"Select a segment", NULL, MB_OK);
        return;
    }

    CDialog::OnOK();
}

G2SPOT select_segment_dlg::selected_spot(void) const
{
    return (_selected >= 0 &&
            _selected < static_cast<int>(_spotList.size())) ?
            _spotList[_selected] : G2SPOT();
}
