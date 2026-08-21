// envent_list_ctrl.h : header file
//

#ifndef _CONTROL_EVENT_LIST_CTRL_H_
#define _CONTROL_EVENT_LIST_CTRL_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "control/drop_target.h"

namespace client {
    class CIDropTarget;

//////////////////////////////////////////////////////////////////////////

class event_list_ctrl : public CListCtrl
                      , protected client::drop_event
{
public:
    event_list_ctrl(void);
    virtual ~event_list_ctrl(void);

public:
    struct event_data {
        G2EVENT_INFO _info;
        COleDateTime _time;

        event_data(const G2EVENT_INFO& info) {
            _info = info;
            _time = COleDateTime::GetCurrentTime();
        }
    };

protected:
    CWnd* _parent;
    CIDropTarget* _dropTarget;

protected:
    void register_drag_drop(void);
    void unregister_drag_drop(void);
    void onLvnBeginDragImpl(LPNMLISTVIEW pNMLV, LRESULT* pResult);

protected:
    virtual void OnDrop(client::drop_info& di);

    DECLARE_MESSAGE_MAP()

    afx_msg int  OnCreate(LPCREATESTRUCT lpCreateStruct);
    afx_msg void OnNcCalcSize(BOOL bCalcValidRects, NCCALCSIZE_PARAMS* lpncsp);
    afx_msg void OnLvnBeginDrag(NMHDR *pNMHDR, LRESULT *pResult);
    afx_msg void OnLvnBeginRDrag(NMHDR *pNMHDR, LRESULT *pResult);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_CONTROL_EVENT_LIST_CTRL_H_
