// select_scope_dlg.cpp : implementation file
//

#include "stdafx.h"
#include "select_scope_dlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

enum { IDD = IDD_SELECT_SCOPE };

//////////////////////////////////////////////////////////////////////////

select_scope_dlg::select_scope_dlg(CWnd* parent /*=NULL*/)
    : CDialog(IDD, parent)
    , _parent(parent)
    , _selected(-1)
{

}

select_scope_dlg::~select_scope_dlg(void)
{

}

//////////////////////////////////////////////////////////////////////////

void select_scope_dlg::DoDataExchange(CDataExchange* pDX)
{
    CDialog::DoDataExchange(pDX);
    DDX_Control(pDX, IDC_LST_SELECT_SEGMENT, _list);
}

BEGIN_MESSAGE_MAP(select_scope_dlg, CDialog)
    ON_NOTIFY(NM_CLICK, IDC_LST_SELECT_SEGMENT, OnClickList)
END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////

BOOL select_scope_dlg::OnInitDialog()
{
    CDialog::OnInitDialog();

    CRect rect;
    _list.GetClientRect(&rect);

    SetWindowText(L"ClipCopy scope list");
    _list.InsertColumn(0, L"ClipCopy scope list", LVCFMT_LEFT, rect.Width());

    /////////////////////////////////////////////

    int index = 0;
    CString string;

    for (ScopeList::const_iterator itr(_scopeList.begin());
         itr != _scopeList.end();
         ++itr) {
        const G2SPOT& begin = (*itr)._begin;
        const G2SPOT& end = (*itr)._end;

        CTime btime(g2_time_to_time32_t(&begin._time));
        CTime etime(g2_time_to_time32_t(&end._time));

        string.Format(_T("Segment(%d) %s ~ Segment(%d) %s"),
                      begin._segment,
                      btime.Format(_T("%Y-%m-%d %H:%M:%S")).operator LPCTSTR(),
                      end._segment,
                      etime.Format(_T("%Y-%m-%d %H:%M:%S")).operator LPCTSTR());

        _list.InsertItem(index++, string);
    }

    if (index != 0) {
        _list.SetItemState(0, LVIS_SELECTED | LVIS_FOCUSED, LVIS_SELECTED | LVIS_FOCUSED);
    }
    else {
        _selected = -1;
    }

    /////////////////////////////////////////////

    if (_parent != NULL &&
        ::IsWindow(_parent->GetSafeHwnd())) {
        CenterWindow(_parent);
    }

    return TRUE;
}

//////////////////////////////////////////////////////////////////////////

void select_scope_dlg::set_scope_list(const std::vector<G2SCOPE>& list)
{
    _scopeList = list;
}

void select_scope_dlg::OnClickList(NMHDR *pNMHDR, LRESULT *pResult)
{
    NM_LISTVIEW* pNMListView = (NM_LISTVIEW *)pNMHDR;
    _selected = pNMListView->iItem;
    GetDlgItem(IDOK)->EnableWindow(_selected >= 0);

    *pResult = 0;
}

void select_scope_dlg::OnOK()
{
    if (_selected == -1) {
        MessageBox(L"Select a scope", NULL, MB_OK);
        return;
    }

    CDialog::OnOK();
}
