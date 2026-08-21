// MainFrm.h : interface of the CMainFrame class
//

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <panel/splitter_wnd.h>
#include <device/device_info_manager.h>
#include <boost/shared_ptr.hpp>

class CG2ClientView;

namespace client {
    class device_tree;
    class panel_instant_event;
    class listener_client_user;
    class StatusViewer;
}

//////////////////////////////////////////////////////////////////////////

class CMainFrame : public CFrameWnd
{
protected:
	CMainFrame(void);
	DECLARE_DYNCREATE(CMainFrame)

public:
    virtual ~CMainFrame(void);

public:
    enum {
        UM_POST_INITIALIZE = WM_USER + 1000,
        UM_POST_EMPTY_DEVICE,
        UM_POST_SCREEN_SELECTED,
    };

    typedef client::device_info_manager::keyType       keyType;
    typedef client::device_info_manager::valueType     valueType;

protected:
    client::splitter_wnd            _splitter;
    client::panel_instant_event*    _instant_eventer;
    client::device_info_manager     _manager;
    client::StatusViewer*           _status_viewer;

protected:
    virtual BOOL PreCreateWindow(CREATESTRUCT& cs);
    virtual BOOL OnCreateClient(LPCREATESTRUCT lpcs, CCreateContext* pContext);

#ifdef _DEBUG
    virtual void AssertValid() const;
    virtual void Dump(CDumpContext& dc) const;
#endif

    DECLARE_MESSAGE_MAP()

	afx_msg int  OnCreate(LPCREATESTRUCT lpCreateStruct);
    afx_msg void OnClose();
    afx_msg BOOL OnQueryEndSession();
    afx_msg void OnGetMinMaxInfo(MINMAXINFO* lpMMI);

    afx_msg void on_main_system_add();
    afx_msg void on_main_system_remove();
    afx_msg void on_main_system_modify();
    afx_msg void on_main_system_status();

    LRESULT OnInvokeInitialize(WPARAM wParam, LPARAM lParam);
    LRESULT on_post_empty_device(WPARAM wParam, LPARAM lParam);
    LRESULT on_post_screen_selected(WPARAM wParam, LPARAM lParam);

protected:
    bool initialize(void);
    bool finalize(void);
    bool finalize_prev(void);
    void exit_imp(void);
    void construct_menu_bar(void);

public:
    CG2ClientView* get_runner(void);
    client::StatusViewer* get_status_viewer(void);

public: // instant event
    void set_instant_eventer(client::panel_instant_event* eventer)          { _instant_eventer = eventer; }
    void append_instant_event(const G2EVENT_INFO& info, const G2DEVICE_STATUS& status);

public: // site info manager
    void set_device_tree(boost::shared_ptr<client::device_tree> tree)       { _manager.set_device_tree(tree); }

    void add_site_child(const CString& site, const CString& child, const int channel, bool isCamera) { _manager.add_child(site, child, channel, isCamera); }
    void remvoe_site_children(const CString& site)                          { _manager.remove_children(site); }
    bool get_site_info(const keyType& key, valueType& info)                 { return _manager.get_info(key, info); }
    bool get_site_child(const keyType& key, int channel, CString& title)    { return _manager.get_child_info(key, channel, title); }
    keyType selected_site(void)                                             { return _manager.selected_site(); }
    bool site_has_children(const keyType& key)                              { return _manager.has_children(key); }
};

//////////////////////////////////////////////////////////////////////////

extern CMainFrame* g_mainframe;

namespace client {
    typedef CMainFrame foundation;

    inline client::foundation* foundation_ptr(void)
    {
        return g_mainframe;
    }
}
