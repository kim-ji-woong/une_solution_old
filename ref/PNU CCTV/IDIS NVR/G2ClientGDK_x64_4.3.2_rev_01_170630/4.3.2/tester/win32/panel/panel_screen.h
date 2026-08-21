// panel_screen.h : header file
//

#ifndef _PANEL_SCREEN_H_
#define _PANEL_SCREEN_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <screen/screen_actuator.h>

namespace client {

//////////////////////////////////////////////////////////////////////////

class panel_screen : public CDialog
{
public:
    panel_screen(void);
    ~panel_screen(void);

protected:
    CWnd* _parent;

    screen_actuator _actuator;

protected:
    void initialize(void);
    void finalize(void);

public:
    virtual bool create(CWnd* parent, unsigned int id);
    virtual HWND safe_hwnd(void) const { return GetSafeHwnd(); }

public:
    client::screen_actuator& actuator_ref(void) { return _actuator; }

    void change_view(int type);

protected:
    virtual void DoDataExchange(CDataExchange* pDX);
    virtual BOOL OnInitDialog();
    virtual BOOL PreTranslateMessage(MSG* pMsg) ;

    DECLARE_MESSAGE_MAP();

    afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
    afx_msg void OnDestroy();
    afx_msg void OnSize(unsigned int nType, int cx, int cy);
    afx_msg BOOL OnEraseBkgnd(CDC* pDC);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_PANEL_SCREEN_H_
