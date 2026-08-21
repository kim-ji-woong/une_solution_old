// G2ClientSide.h : header file
//

#pragma once

#include <panel/splitter_wnd.h>
#include <panel/panel_device_tree.h>
#include <panel/panel_instant_event.h>

class CG2ClientSide : public CView
{
protected:
    CG2ClientSide(void);
    DECLARE_DYNCREATE(CG2ClientSide);

public:
    virtual ~CG2ClientSide(void);

protected:
    client::splitter_wnd _splitter;
    client::panel_device_tree   _tree_panel;
    client::panel_instant_event _event_panel;

protected:
    void initialize(void);
    void finalize(void);

protected:
    virtual BOOL PreCreateWindow(CREATESTRUCT& cs);
    virtual void OnDraw(CDC* /*pDC*/);
    virtual void OnInitialUpdate();

    DECLARE_MESSAGE_MAP()

    afx_msg int  OnCreate(LPCREATESTRUCT lpcs);
    afx_msg BOOL OnEraseBkgnd(CDC* pDC);
    afx_msg void OnStyleChanged(int nStyleType, LPSTYLESTRUCT lpStyleStruct);
    afx_msg void OnSize(unsigned int nType, int cx, int cy);
};
