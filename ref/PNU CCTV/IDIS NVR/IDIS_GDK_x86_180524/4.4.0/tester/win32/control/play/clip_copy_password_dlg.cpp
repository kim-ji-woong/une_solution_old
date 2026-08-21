// clip_copy_password_dlg.cpp : implementation file
//

#include "stdafx.h"
#include "clip_copy_password_dlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

clip_copy_password_dlg::clip_copy_password_dlg(CWnd* parent /*=NULL*/)
    : CDialog(IDD_PLAY_CLIPCOPY_PASSWORD, parent)
    , _parent(parent)
{
    _password.Empty();
}

clip_copy_password_dlg::~clip_copy_password_dlg(void)
{

}

//////////////////////////////////////////////////////////////////////////

void clip_copy_password_dlg::DoDataExchange(CDataExchange* pDX)
{
    CDialog::DoDataExchange(pDX);
}

IMPLEMENT_DYNAMIC(clip_copy_password_dlg, CDialog)

BEGIN_MESSAGE_MAP(clip_copy_password_dlg, CDialog)
END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////

BOOL clip_copy_password_dlg::OnInitDialog()
{
    CDialog::OnInitDialog();

    if (CEdit* editor = (CEdit*)GetDlgItem(IDC_EDT_PASSWORD)) {
        editor->SetLimitText(client::max_::PASSWORD_CLIPCOPY);
        editor->SetFocus();
    }

    CenterWindow(_parent);

    return TRUE;
}

void clip_copy_password_dlg::OnOK()
{
    GetDlgItemText(IDC_EDT_PASSWORD, _password);

    _password.Trim();
    if (_password.IsEmpty()) {
        MessageBox(L"Please, Enter password.");
        SetDlgItemText(IDC_EDT_PASSWORD, _T(""));
    }
    else {
        CDialog::OnOK();
    }
}
