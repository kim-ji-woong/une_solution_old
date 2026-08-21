// panel_controller.h : header file
//

#ifndef _PANEL_CONTROLLER_H_
#define _PANEL_CONTROLLER_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

namespace client {
    class controller_base;
    class controller_watch;
    class controller_search;
    class controller_search_g2;

//////////////////////////////////////////////////////////////////////////

class panel_controller : public CDialog
{
public:
    panel_controller(void);
    ~panel_controller(void);

protected:
    enum CONTROLLER_TYPE {
        CONTROLLER_UNDEFINED = -1,
        CONTROLLER_LIVE = 0,
        CONTROLLER_SEARCH,
        CONTROLLER_SEARCH_G2,

        CONTROLLER_COUNT
    };

protected:
    CWnd* _parent;

    int _prevType;
    std::vector<controller_base*> _container;

protected:
    void initialize(void);
    void finalize(void);

public:
    virtual bool create(CWnd* parent, unsigned int id);
    virtual HWND safe_hwnd(void) const { return GetSafeHwnd(); }

public:
    controller_watch*        controller_live_ptr(void);
    controller_search*      controller_search_ptr(void);
    controller_search_g2*   controller_search_g2_ptr(void);

    std::vector<controller_base*> controller_bunch(void);

    void change_controller(int type);

protected:
    virtual void DoDataExchange(CDataExchange* pDX);
    virtual BOOL OnInitDialog();
    virtual BOOL PreTranslateMessage(MSG* pMsg) ;

    DECLARE_MESSAGE_MAP();

    afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
    afx_msg void OnDestroy();
    afx_msg void OnSize(unsigned int nType, int cx, int cy);
    afx_msg BOOL OnEraseBkgnd(CDC* pDC);

    LRESULT on_post_screen_camera_changed(WPARAM wParam, LPARAM lParam);
    LRESULT on_post_update_time_table(WPARAM wParam, LPARAM lParam);
};

//////////////////////////////////////////////////////////////////////////

typedef panel_controller controller;

} // !_namespace_client

#endif // !_PANEL_CONTROLLER_H_
