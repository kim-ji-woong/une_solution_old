// calendar_dlg.h : header file
//

#ifndef _CONTROL_CALENDAR_DLG_H_
#define _CONTROL_CALENDAR_DLG_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "search/search_common.h"

namespace client {

//////////////////////////////////////////////////////////////////////////

class calendar_dlg : public CDialog
{
public:
    calendar_dlg(CWnd* pParent = NULL, int mode = search::SEARCH_LOCAL);
	virtual ~calendar_dlg(void);

protected:
    CWnd* _parent;
    short _channel;

    CTime _selectDate;

    int _mode;
    CPoint _pos;

    bool _init;

protected:
    void update_calendar(LPNMDAYSTATE pDayState = NULL);
    void set_range(void);

public:
    void set_channel(short channel) { _channel = channel; }
    void set_pos(CPoint& pos) { _pos = pos; }
    CTime get_selected_date(void) const { return _selectDate; }

protected:
	virtual void DoDataExchange(CDataExchange* pDX);
    virtual BOOL OnInitDialog();

	DECLARE_MESSAGE_MAP()

    afx_msg void on_mcn_get_day_state_calendar_control(NMHDR *pNMHDR, LRESULT *pResult);
    afx_msg void on_mcn_select_calendar_control(NMHDR *pNMHDR, LRESULT *pResult);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_CONTROL_CALENDAR_DLG_H_
