// event_condition_dlg.h : header file
//

#ifndef _CONTROL_EVENT_CONDITION_DLG_H_
#define _CONTROL_EVENT_CONDITION_DLG_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_define_live.h>
#include <common/event_functional.h>
#include <vector>

namespace client {

//////////////////////////////////////////////////////////////////////////

class event_condition_dlg : public CDialog
{
public:
    event_condition_dlg(CWnd* parent = NULL);
    virtual ~event_condition_dlg(void);

private:
    CWnd* _parent;

    std::vector<int> _eventList;

    CButton _chkEvent[event_condition_::END_EVENT];

public:
    void set_event(const std::vector<int>& eventList);
    std::vector<int> get_event(void) const { return _eventList; }

protected:
    virtual void DoDataExchange(CDataExchange* pDX);
    virtual BOOL OnInitDialog();

    DECLARE_MESSAGE_MAP()

    afx_msg void OnOK();
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_CONTROL_EVENT_CONDITION_DLG_H_
