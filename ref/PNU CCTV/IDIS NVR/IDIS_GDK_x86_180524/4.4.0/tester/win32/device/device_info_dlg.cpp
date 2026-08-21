// device_info_dlg.cpp : implementation file
//

#include "stdafx.h"
#include "G2Client.h"
#include "device_info_dlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

//////////////////////////////////////////////////////////////////////////

enum { IDD = IDD_DEVICE_INFO };

//////////////////////////////////////////////////////////////////////////

device_info_dlg::device_info_dlg(CWnd* parent)
    : CDialog(IDD, parent)
{

}

device_info_dlg::~device_info_dlg(void)
{

}

//////////////////////////////////////////////////////////////////////////

void device_info_dlg::DoDataExchange(CDataExchange* pDX)
{
    CDialog::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(device_info_dlg, CDialog)
    ON_WM_CREATE()
    ON_WM_DESTROY()
END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////

BOOL device_info_dlg::PreTranslateMessage(MSG* pMsg)
{
    return CDialog::PreTranslateMessage(pMsg);
}

BOOL device_info_dlg::OnInitDialog()
{
    CDialog::OnInitDialog();

    SetDlgItemText(IDC_DEVICE_INFO_EDT_SITE,        _info._site);
    SetDlgItemText(IDC_DEVICE_INFO_EDT_ADDRESS,     _info._address);
    SetDlgItemText(IDC_DEVICE_INFO_EDT_ID,          _info._id);
    SetDlgItemText(IDC_DEVICE_INFO_EDT_PASSWORD,    _info._password);

    SetDlgItemInt(IDC_DEVICE_INFO_EDT_PORT_ADMIN,   _info._adminPort);
    SetDlgItemInt(IDC_DEVICE_INFO_EDT_PORT_WATCH,   _info._watchPort);
    SetDlgItemInt(IDC_DEVICE_INFO_EDT_PORT_SEARCH,  _info._searchPort);
    SetDlgItemInt(IDC_DEVICE_INFO_EDT_PORT_AUDIO,   _info._audioPort);
    
    return TRUE;
}

void device_info_dlg::OnOK()
{
    GetDlgItemText(IDC_DEVICE_INFO_EDT_SITE,     _info._site);
    GetDlgItemText(IDC_DEVICE_INFO_EDT_ADDRESS,  _info._address);
    GetDlgItemText(IDC_DEVICE_INFO_EDT_ID,       _info._id);
    GetDlgItemText(IDC_DEVICE_INFO_EDT_PASSWORD, _info._password);

    _info._adminPort  = GetDlgItemInt(IDC_DEVICE_INFO_EDT_PORT_ADMIN);
    _info._watchPort  = GetDlgItemInt(IDC_DEVICE_INFO_EDT_PORT_WATCH);
    _info._searchPort = GetDlgItemInt(IDC_DEVICE_INFO_EDT_PORT_SEARCH);
    _info._audioPort  = GetDlgItemInt(IDC_DEVICE_INFO_EDT_PORT_AUDIO);

    CDialog::OnOK();
}

void device_info_dlg::OnCancel()
{
    CDialog::OnCancel();
}

//////////////////////////////////////////////////////////////////////////

int device_info_dlg::OnCreate(LPCREATESTRUCT lpCreateStruct)
{
    if (CDialog::OnCreate(lpCreateStruct) == -1) {
        return -1;
    }
    return 0;
}

void device_info_dlg::OnDestroy()
{
    CDialog::OnDestroy();
}

//////////////////////////////////////////////////////////////////////////