// search_event_progress.h : header file
//

#ifndef _CONTROL_SEARCH_PROGRESS_H_
#define _CONTROL_SEARCH_PROGRESS_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

namespace client {
    class g2search;

//////////////////////////////////////////////////////////////////////////

class search_event_progress : public CDialog
{
public:
    search_event_progress(CWnd* parent = NULL);
    virtual ~search_event_progress(void);

    struct message_ {
        enum UM {
            POST_DESTROY = WM_USER + 100
        };
    };

    struct timer_ {
        struct id_ {
            enum ID {
                CHECK_POSITION = 100
            };
        };
    };

private:
    CWnd* _parent;
    bool _progressing;
    short _channel;
    unsigned int _previous;

protected:
    unsigned int _begin;
    unsigned int _end;

    g2search* _client;

public:
    bool create(CWnd* parent);
    void destroy(bool post = false);
    void range(const G2TIME& begin, const G2TIME& end);
    void channel(g2search* adaptor, short channel);
    void progress(const G2TIME& time);
    void show(bool show);

    HWND safe_handle(void) const { return GetSafeHwnd(); }

protected:
    virtual void DoDataExchange(CDataExchange* pDX);
    virtual BOOL PreTranslateMessage(MSG* pMsg);
    virtual BOOL OnInitDialog();

    DECLARE_MESSAGE_MAP()

    afx_msg void OnTimer(UINT_PTR nIDEvent);
    afx_msg void OnShowWindow(BOOL bShow, UINT nStatus);
    afx_msg void OnBtnStop();

    LRESULT OnPostDestroy(WPARAM wParam, LPARAM lParam);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_CONTROL_SEARCH_PROGRESS_H_
