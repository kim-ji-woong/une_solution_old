// ptz_preset_dlg.cpp : implementation file
//

#include "stdafx.h"
#include "ptz_preset_dlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

enum { IDD = IDD_PTZ_PRESET };

//////////////////////////////////////////////////////////////////////////

ptz_preset_dlg::ptz_preset_dlg(CWnd* parent /*=NULL*/, int mode /*= MODE_PRESET*/)
    : CDialog(IDD, parent)
    , _mode(mode)
    , _parent(parent)
    , _selected(-1)
{
    memset(&_preset, 0, sizeof(_preset));
}

ptz_preset_dlg::~ptz_preset_dlg(void)
{

}

//////////////////////////////////////////////////////////////////////////

void ptz_preset_dlg::DoDataExchange(CDataExchange* pDX)
{
    CDialog::DoDataExchange(pDX);
    DDX_Control(pDX, IDC_LST_PRESET, _list);
}

BEGIN_MESSAGE_MAP(ptz_preset_dlg, CDialog)
    ON_WM_TIMER()
    ON_NOTIFY(NM_CLICK, IDC_LST_PRESET, OnClickList)
END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////

BOOL ptz_preset_dlg::OnInitDialog()
{
    CDialog::OnInitDialog();

    CString title = (_mode == MODE_PRESET) ? L"Set preset" : L"View preset";
    SetWindowText(title);

    struct { LPCTSTR _name; int _width; int _format; } elements[] = {
        { L"#", 30, LVCFMT_CENTER },
        { L"Title", 110, LVCFMT_LEFT }
    };

    _list.ModifyStyle(0, LVS_SHOWSELALWAYS);
    _list.SetExtendedStyle(LVS_EX_FULLROWSELECT);

    CRect rect;
    _list.GetClientRect(&rect);
    int length = rect.Width() - elements[0]._width;
    length -= GetSystemMetrics(SM_CXVSCROLL) + GetSystemMetrics(SM_CXBORDER) * 2;
    length = __max(length, 0);

    elements[COLUMN_TITLE]._width = length;

    for (int i = 0; i < _countof(elements); ++i) {
        _list.InsertColumn(i, elements[i]._name, elements[i]._format, elements[i]._width, i);
    }

    /////////////////////////////////////////////

    init_preset();

    if (_mode == MODE_PREVIEW) {
        GetDlgItem(IDC_STC_TITLE)->ShowWindow(SW_HIDE);
        GetDlgItem(IDC_EDT_TITLE)->ShowWindow(SW_HIDE);
    }
    else {
        SetTimer(timer_::id_::CHECK_VALID, timer_::elapse_::CHECK_VALID, NULL);
    }

    if (_parent != NULL &&
        ::IsWindow(_parent->GetSafeHwnd())) {
        CenterWindow(_parent);
    }

    return TRUE;
}

//////////////////////////////////////////////////////////////////////////

void ptz_preset_dlg::OnTimer(UINT_PTR nIDEvent)
{
    CDialog::OnTimer(nIDEvent);

    if (nIDEvent == timer_::id_::CHECK_VALID) {
        bool input = false;

        if (CEdit* control = (CEdit*)GetDlgItem(IDC_EDT_TITLE)) {
            if (control->IsWindowVisible()) {
                CString title;
                control->GetWindowText(title);
                input = (title.IsEmpty() != true);
            }
        }
        GetDlgItem(IDOK)->EnableWindow((_selected >= 0 && input) ? TRUE : FALSE);
    }
}

void ptz_preset_dlg::OnClickList(NMHDR *pNMHDR, LRESULT *pResult)
{
    NM_LISTVIEW* pNMListView = (NM_LISTVIEW *)pNMHDR;
    _selected = pNMListView->iItem;
    *pResult = 0;
}

void ptz_preset_dlg::OnOK()
{
    POSITION pos = _list.GetFirstSelectedItemPosition();
    if (!pos) {
        _selected = -1;
    }

    CString string;
    bool cancel = false;

    while (pos) {
        _selected = _list.GetNextSelectedItem(pos);
        if (_mode == MODE_PREVIEW) continue;
        string.Format(L"Do you want to apply to preset %d?", _selected + 1);
        if (MessageBox(string, NULL, MB_ICONQUESTION | MB_YESNO) != IDYES) {
            cancel = true;
            break;
        }
        string.Empty();
        GetDlgItem(IDC_EDT_TITLE)->GetWindowText(string);
        _list.SetItemText(_selected, COLUMN_TITLE, string);
    }

    if (cancel) return;
    if (_mode == MODE_PRESET) {
        string.Empty();
        size_t temp = 0;
        for (size_t i = 0; i < _preset._g1._count; ++i) {
            string = _list.GetItemText((int)i, COLUMN_TITLE);
            wcstombs_s(&temp, _preset._g1._preset[i], G2LIVE_PTZ_PRESET::MAX_LEN_PRESET + 1, string, _TRUNCATE);
        }
    }

    CDialog::OnOK();
}

//////////////////////////////////////////////////////////////////////////

void ptz_preset_dlg::init_preset(void)
{
    GetDlgItem(IDC_EDT_TITLE)->SendMessage(EM_SETLIMITTEXT, WPARAM(G2LIVE_PTZ_PRESET::MAX_LEN_PRESET + 1));

    _list.DeleteAllItems();

    CString string;
    CString format = (_preset._g1._count > 99) ? L"%3d" : L"%2d";

    int idx = -1;

    for (int i = 0, count = _preset._g1._count; i < count; ++i) {
        string.Format(format, i + 1);
        idx = _list.InsertItem(i, string);
        if (idx != -1) {
            _list.SetItemText(idx, 1, CString(_preset._g1._preset[i]));
        }
    }
    if (_preset._g1._count > 0) {
        _list.SetColumnWidth(0, _preset._g1._count >= 100 ? 50 : 30);
        _list.SetSelectionMark(0);
        _list.SetItemState(0, LVIS_SELECTED | LVIS_FOCUSED, LVIS_SELECTED | LVIS_FOCUSED);
        _list.SetFocus();
        _selected = 0;
    }
}
