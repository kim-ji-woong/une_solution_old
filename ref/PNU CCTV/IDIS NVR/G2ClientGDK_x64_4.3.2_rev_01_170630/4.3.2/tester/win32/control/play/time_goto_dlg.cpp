// time_goto_dlg.cpp : implementation file
//

#include "stdafx.h"
#include "time_goto_dlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

enum { IDD = IDD_GOTO_TIME };

//////////////////////////////////////////////////////////////////////////

time_goto_dlg::time_goto_dlg(CWnd* parent /*= NULL*/)
    : CDialog(IDD, parent)
    , _parent(parent)
    , _centerBase(parent)
    , _supportDate(false)
{

}

time_goto_dlg::~time_goto_dlg(void)
{

}

//////////////////////////////////////////////////////////////////////////


void time_goto_dlg::DoDataExchange(CDataExchange* pDX)
{
    CDialog::DoDataExchange(pDX);
    DDX_Control(pDX, IDC_GOTOTIME_DTP_DATE, _ctrlDate);
    DDX_Control(pDX, IDC_GOTOTIME_DTP_TIME, _ctrlTime);
}


//////////////////////////////////////////////////////////////////////////

BEGIN_MESSAGE_MAP(time_goto_dlg, CDialog)
END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////


void time_goto_dlg::set_center_base(CWnd* centerBase)
{
    G2RETURN_IF_ASSERT(centerBase != NULL &&
                       centerBase->GetSafeHwnd());

    _centerBase = centerBase;
}

BOOL time_goto_dlg::OnInitDialog()
{
    CDialog::OnInitDialog();

    CTime current(g2_time_to_time32_t(&_currentTime));

    _ctrlDate.SetFormat(_T("yyyy-MM-dd"));
    _ctrlTime.SetFormat(_T("HH:mm:ss"));
    _ctrlDate.SetTime(&current);
    _ctrlTime.SetTime(&current);

    _ctrlDate.EnableWindow(_supportDate ? TRUE : FALSE);

    CWnd* parent = (_centerBase != NULL && _centerBase->GetSafeHwnd()) ? _centerBase : _parent;
    CenterWindow(parent);

    return TRUE;
}

void time_goto_dlg::OnOK()
{
    CTime date, time;
    _ctrlDate.GetTime(date);
    _ctrlTime.GetTime(time);

    CTime current(date.GetYear(), date.GetMonth(), date.GetDay(),
                  time.GetHour(), time.GetMinute(), time.GetSecond());

    g2_time_from_time32_t(&_currentTime, (unsigned long)current.GetTime());

    CDialog::OnOK();
}

bool time_goto_dlg::set_current_time(const G2TIME& time)
{
    assert(g2_time_is_valid(&time));

    _currentTime = time;

    return true;
}

void time_goto_dlg::set_support_date(bool support)
{
    _supportDate = support;
}
