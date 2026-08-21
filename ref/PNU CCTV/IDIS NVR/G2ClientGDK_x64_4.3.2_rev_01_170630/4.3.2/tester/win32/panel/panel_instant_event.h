// panel_instant_event.h : header file
//

#ifndef _PANEL_INSTANT_EVENT_H_
#define _PANEL_INSTANT_EVENT_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <control/event/event_list_ctrl.h>

namespace client {

//////////////////////////////////////////////////////////////////////////

class panel_instant_event : public CDialog
{
public:
    panel_instant_event(void);
    ~panel_instant_event(void);

public:
    enum {
        UM_POST_APPEND_EVENT = WM_USER + 100,
    };

protected:
    enum COLUMN_TYPE {
        COLUMN_EVENT = 0,
        COLUMN_CAMERA,
        COLUMN_DEVICE,
        COLUMN_TIME
    };

    struct item_data_ : public G2EVENT_INFO {
        G2EVENT_INFO _info;
        COleDateTime _time;

        item_data_(const G2EVENT_INFO& info) {
            _info = info;
            _time = COleDateTime::GetCurrentTime();
        }
    };

protected:
    CWnd* _parent;
    event_list_ctrl _list;

    g2::critical_section _lock_exec;

protected:
    void initialize(void);
    void finalize(void);

    void remove_event_item(void);

public:
    void append_event(const G2EVENT_INFO& info, const G2DEVICE_STATUS& status);
    void clear(void);

public:
    virtual bool create(CWnd* parent, unsigned int id);
    virtual HWND safe_hwnd(void) const { return GetSafeHwnd(); }
    virtual void cleanup(void);

protected:
    virtual void DoDataExchange(CDataExchange* pDX);
    virtual BOOL OnInitDialog();
    virtual BOOL PreTranslateMessage(MSG* pMsg) ;

    DECLARE_MESSAGE_MAP();

    afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
    afx_msg void OnDestroy();
    afx_msg void OnSize(unsigned int nType, int cx, int cy);
    afx_msg BOOL OnEraseBkgnd(CDC* pDC);
    afx_msg void OnTimer(UINT_PTR nIDEvent);

    LRESULT on_post_append_event(WPARAM wParam, LPARAM lParam);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

namespace client {
    typedef panel_instant_event instant_eventer;
}

#endif // !_PANEL_INSTANT_EVENT_H_
