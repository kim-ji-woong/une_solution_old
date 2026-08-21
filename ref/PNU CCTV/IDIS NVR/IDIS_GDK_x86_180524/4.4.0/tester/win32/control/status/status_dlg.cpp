// status_dlg.cpp : implementation file
//

#include "stdafx.h"
#include "status_dlg.h"
#include <sampler/cpp/g2client_status.h>

//#include <boost/bind.hpp>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

enum { IDD = IDD_STATUS };

//////////////////////////////////////////////////////////////////////////


StatusViewer::StatusViewer(CWnd* parent /*=NULL*/)
    : CDialog(IDD, parent)
    , _parent(parent)
    , _data(_T(""))
    , _channel(-1)
    , _reconnect(false)
{

}

StatusViewer::~StatusViewer(void)
{

}

//////////////////////////////////////////////////////////////////////////

CString StatusViewer::data(void) const 
{
    return _data;
}

void StatusViewer::DoDataExchange(CDataExchange* pDX)
{
    CDialog::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(StatusViewer, CDialog)

    ON_BN_CLICKED(IDC_STATUS_BTN_UPDATE_STATUS, OnBtnUpdateStatus)

END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////

BOOL StatusViewer::OnInitDialog()
{
    CDialog::OnInitDialog();

    SetWindowText(_T("Status Viewer"));

    return TRUE;

}

void StatusViewer::OnBtnUpdateStatus()
{
    // updating routine should come here
    // call sampler's request
    SetDlgItemText(IDC_STATUS_EDT, _T(""));
    int channel = _channel;
    client::runner* runner = client::runner_ptr();
    if (_reconnect) {
        runner->connect_status();
    }
    else{
        runner->status_ref().request_status(channel);
    }
}
