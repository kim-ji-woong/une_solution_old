// message_box.h : header file
//

#ifndef _CONTROL_MESSAGE_BOX_H_
#define _CONTROL_MESSAGE_BOX_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

namespace client {

//////////////////////////////////////////////////////////////////////////

class message_box : public CDialog
{
public:
    message_box(void);
    ~message_box(void);

protected:
    CWnd* _parent;

    CSize _size;

protected:
    CFont   _font;
    CBrush  _brush;
    CStatic _stcString;
    CStatic _stcMessage;

    std::wstring _string;
    std::wstring _message;

protected:
    void initialize(void);
    void finalize(void);

public:
    virtual bool create(CWnd* parent, const CSize& size = CSize(600, 60));
    virtual HWND safe_hwnd(void) const { return GetSafeHwnd(); }

    virtual void show(bool message = false, bool autohide = true, int elapse = 5000);
    virtual void hide(void);

public:
    void set_string(const std::wstring& string);
    void set_message(const std::wstring& message);

    void set_size(const CSize& size);
    bool is_visible(void) const { return (IsWindowVisible() == TRUE); }

protected:
    virtual void DoDataExchange(CDataExchange* pDX);
    virtual BOOL OnInitDialog();
    virtual BOOL PreTranslateMessage(MSG* pMsg) ;

    DECLARE_MESSAGE_MAP();

    afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
    afx_msg void OnDestroy();
    afx_msg void OnSize(unsigned int nType, int cx, int cy);
    afx_msg BOOL OnEraseBkgnd(CDC* pDC);
    afx_msg HBRUSH OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor);
    afx_msg void OnTimer(UINT_PTR nIDEvent);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_CONTROL_MESSAGE_BOX_H_
