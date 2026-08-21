// calendar_dlg.cpp : implementation file
//

#include "stdafx.h"
#include "calendar_dlg.h"
#include <search/search_data_manager.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

calendar_dlg::calendar_dlg(CWnd* parent /*= NULL*/, int mode /*= search::SEARCH_PLAY*/)
    : CDialog(IDD_PLAY_CALENDAR, parent)
    , _parent(parent)
    , _centerBase(parent)
    , _mode(mode)
{

}

calendar_dlg::~calendar_dlg(void)
{

}

//////////////////////////////////////////////////////////////////////////

void calendar_dlg::DoDataExchange(CDataExchange* pDX)
{
    CDialog::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(calendar_dlg, CDialog)
    ON_NOTIFY(MCN_GETDAYSTATE, IDC_PLAY_CALENDAR_CONTROL, &calendar_dlg::on_mcn_get_day_state_calendar_control)
    ON_NOTIFY(MCN_SELECT, IDC_PLAY_CALENDAR_CONTROL, &calendar_dlg::on_mcn_select_calendar_control)
END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////

void calendar_dlg::set_center_base(CWnd* centerBase)
{
    G2RETURN_IF_ASSERT(centerBase != NULL &&
                       centerBase->GetSafeHwnd());

    _centerBase = centerBase;
}

BOOL calendar_dlg::OnInitDialog()
{
    CDialog::OnInitDialog();

    CWnd* parent = (_centerBase != NULL && _centerBase->GetSafeHwnd()) ? _centerBase : _parent;
    CenterWindow(parent);

    return TRUE;
}

void calendar_dlg::on_mcn_get_day_state_calendar_control(NMHDR *pNMHDR, LRESULT *pResult)
{
    LPNMDAYSTATE pDayState = reinterpret_cast<LPNMDAYSTATE>(pNMHDR);

    update_calendar(pDayState);

    *pResult = 0;
}

void calendar_dlg::on_mcn_select_calendar_control(NMHDR *pNMHDR, LRESULT *pResult)
{
    LPNMSELCHANGE pSelChange = reinterpret_cast<LPNMSELCHANGE>(pNMHDR);

    _selectDate = CTime(pSelChange->stSelStart.wYear, pSelChange->stSelStart.wMonth, pSelChange->stSelStart.wDay, 0, 0, 0);

    CDialog::EndDialog(IDOK);

    *pResult = 0;
}

//////////////////////////////////////////////////////////////////////////

void calendar_dlg::update_calendar(LPNMDAYSTATE pDayState /*= NULL*/)
{
    const int channel = _channel;
    search_data_ptr data = search_data_manager::get().find_search_data(_mode, channel);
    G2RETURN_IF_FAIL(data);

    search::SEARCH_DATE_INFO_LIST dateList;
    if (_mode == search::SEARCH_LOCAL_G2) {
        data->get_date_info(client::invalid_::CHANNEL_EXT, dateList);
    }
    else {
        data->get_date_info_dvr(dateList);
    }

    int i, j, count, monthCount, index;
    int year, month;

    if (pDayState != NULL)  {
        index = (pDayState->stStart.wDay == 1) ? 0 : 1;
        monthCount = pDayState->cDayState;

        for (i = index; i < monthCount; ++i) {
            pDayState->prgDayState[i] = (MONTHDAYSTATE)0;

            if (dateList.empty() != true) {
                year = pDayState->stStart.wYear;
                month = pDayState->stStart.wMonth + i;
                if (month > 12) {
                    year += (month / 12);
                    month -= 12;
                }

                for (j = 0, count = (int)dateList.size(); j < count; ++j) {
                    const CTime& time = dateList[j];
                    if (year == time.GetYear() &&
                        month == time.GetMonth()) {
                        pDayState->prgDayState[i] |= 0x01 << (time.GetDay() - 1);
                    }
                }
            }
        }
    }
    else {
        CMonthCalCtrl* pCalendar = (CMonthCalCtrl*)GetDlgItem(IDC_PLAY_CALENDAR_CONTROL);
        if (pCalendar == NULL) return;

        SYSTEMTIME timeBegin;
        SYSTEMTIME timeEnd;
        monthCount = pCalendar->GetMonthRange(&timeBegin, &timeEnd, GMR_DAYSTATE);

        LPMONTHDAYSTATE pDayState;
        pDayState = new MONTHDAYSTATE[monthCount];
        memset(pDayState, 0, sizeof(MONTHDAYSTATE) * monthCount);

        if (dateList.empty()) {
            for(int i = 0; i < monthCount; ++i) {
                pDayState[i] = (MONTHDAYSTATE)0;
            }
        }
        else {
            index = (timeBegin.wDay == 1) ? 0 : 1;

            for (i = index; i < monthCount; ++i)  {
                pDayState[i] = (MONTHDAYSTATE)0;

                year = timeBegin.wYear;
                month = timeBegin.wMonth + i;
                if (month > 12) {
                    year += (month / 12);
                    month -= 12;
                }
                for (j = 0, count = (int)dateList.size(); j < count; ++j) {
                    const CTime& time = dateList[j];
                    if (year == time.GetYear() &&
                        month == time.GetMonth()) {
                        pDayState[i] |= 0x01 << (time.GetDay() - 1);
                    }
                }
            }
        }

        VERIFY(pCalendar->SetDayState(monthCount, pDayState));
        if (pDayState) {
            delete[] pDayState;
            pDayState = NULL;
        }
    }
}
