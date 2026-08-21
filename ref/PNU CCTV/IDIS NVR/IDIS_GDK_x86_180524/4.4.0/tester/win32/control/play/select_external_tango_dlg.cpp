// select_external_tango_dlg.cpp : implementation file
//

#include "stdafx.h"
#include "select_external_tango_dlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

enum { IDD = IDD_SELECT_EXTERNAL_TANGO };

//////////////////////////////////////////////////////////////////////////

select_external_tango_dlg::select_external_tango_dlg(CWnd* parent /*=NULL*/)
    : CDialog(IDD, parent)
    , _parent(parent)
    , _selected(-1)
{

}

select_external_tango_dlg::~select_external_tango_dlg(void)
{

}

//////////////////////////////////////////////////////////////////////////

void select_external_tango_dlg::DoDataExchange(CDataExchange* pDX)
{
    CDialog::DoDataExchange(pDX);
    DDX_Control(pDX, IDC_LST_SELECT_EXTERNAL_TANGO, _list);
}

BEGIN_MESSAGE_MAP(select_external_tango_dlg, CDialog)
    ON_NOTIFY(NM_CLICK, IDC_LST_SELECT_EXTERNAL_TANGO, OnClickList)
END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////

BOOL select_external_tango_dlg::OnInitDialog()
{
    CDialog::OnInitDialog();

    CRect rect;
    _list.GetClientRect(&rect);

    SetWindowText(L"Search on Other");

    _list.InsertColumn(0, L"Type", LVCFMT_LEFT, 110);
    _list.InsertColumn(1, L"Capacity", LVCFMT_LEFT, 80);
    _list.InsertColumn(2, L"Format", LVCFMT_LEFT, 110);

    /////////////////////////////////////////////

    int index = 0;
    for (std::vector<G2SEARCH_EXTERNAL_DISK>::const_iterator itr(_tangoList.begin());
         itr != _tangoList.end();
         ++itr, ++index) {
        const G2SEARCH_EXTERNAL_DISK& info = *itr;
        _list.InsertItem(index, string_tango_disk_type(info._disk_type, info._number));
        _list.SetItemText(index, 1, string_tango_disk_capacity(info._capacity));
        _list.SetItemText(index, 2, string_tango_disk_format(info._storage_type));
    }

    _list.SetItemState(0, LVIS_SELECTED | LVIS_FOCUSED, LVIS_SELECTED | LVIS_FOCUSED);
    _selected = 0;

    /////////////////////////////////////////////

    if (_parent != NULL &&
        ::IsWindow(_parent->GetSafeHwnd())) {
        CenterWindow(_parent);
    }

    return TRUE;
}

//////////////////////////////////////////////////////////////////////////

void select_external_tango_dlg::set_external_tango_info(const std::vector<G2SEARCH_EXTERNAL_DISK>& info)
{
    _tangoList = info;
}

void select_external_tango_dlg::OnClickList(NMHDR *pNMHDR, LRESULT *pResult)
{
    NM_LISTVIEW* pNMListView = (NM_LISTVIEW *)pNMHDR;
    _selected = pNMListView->iItem;
    GetDlgItem(IDOK)->EnableWindow(_selected >= 0);

    *pResult = 0;
}

void select_external_tango_dlg::OnOK()
{
    if (_selected == -1) {
        MessageBox(L"Select a external disk", NULL, MB_OK);
        return;
    }

    CDialog::OnOK();
}

CString select_external_tango_dlg::string_tango_disk_type(int type, int number)
{
    CString string;
    switch (type) {
        case G2SEARCH_EXTERNAL_DISK::UNKNOWN      : string = L"Unknown";  break;
        case G2SEARCH_EXTERNAL_DISK::IDE_HDD      : string = L"Internal"; break;
        case G2SEARCH_EXTERNAL_DISK::SCSI_HDD     : string = L"SCSI";     break;
        case G2SEARCH_EXTERNAL_DISK::USB_HDD      : string = L"USB";      break;
        case G2SEARCH_EXTERNAL_DISK::IDE_CDRW     : string = L"CD-RW";    break;
        case G2SEARCH_EXTERNAL_DISK::IDE_DVDRW    : string = L"DVD-RW";   break;
        case G2SEARCH_EXTERNAL_DISK::SW_RAID_DISK : string = L"SW-RAID";  break;
        case G2SEARCH_EXTERNAL_DISK::FLASH_MEMORY : string = L"SDCard";   break;
        case G2SEARCH_EXTERNAL_DISK::ESATA_HDD    : string = L"ESATA";    break;
        case G2SEARCH_EXTERNAL_DISK::ISCSI_HDD    : string = L"iSCSI";    break;
        default: assert(!"undefined disk type"); break;
    }
    string.AppendFormat(L" %d", number + 1);

    return string;
}

CString select_external_tango_dlg::string_tango_disk_capacity(unsigned __int64 capacity)
{
    const unsigned __int64 thousand = 1000;
    
    CString string;
    (capacity < 1000 * 1000 * 1000)            ? string.Format(L"%u MB", static_cast<unsigned int>(capacity / 1000 / 1000)) :
    (capacity < thousand * 1000 * 1000 * 1000) ? string.Format(L"%.2f GB", 1. * capacity / 1000 / 1000 / 1000) :
                                                 string.Format(L"%.2f TB", 1. * capacity / 1000 / 1000 / 1000 / 1000);
    return string;
}

CString select_external_tango_dlg::string_tango_disk_format(int format)
{
    CString string;
    switch (format) {
        case G2SEARCH_EXTERNAL_DISK::NOT_USING : string = L"Not Use";  break;
        case G2SEARCH_EXTERNAL_DISK::RECORD    : string = L"Record";   break;
        case G2SEARCH_EXTERNAL_DISK::ARCHIVE   : string = L"Archive";  break;
        case G2SEARCH_EXTERNAL_DISK::CLIP_COPY : string = L"ClipCopy"; break;
        default: assert("undefined disk format"); break;
    }
    return string;
}
