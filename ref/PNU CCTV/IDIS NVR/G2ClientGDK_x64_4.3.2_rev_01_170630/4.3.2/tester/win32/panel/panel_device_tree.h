// panel_device_tree.h : header file
//

#ifndef _PANEL_DEVICE_TREE_H_
#define _PANEL_DEVICE_TREE_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <boost/shared_ptr.hpp>

namespace client {
    class device_tree;

//////////////////////////////////////////////////////////////////////////

class panel_device_tree : public CDialog
{
public:
    panel_device_tree(void);
    ~panel_device_tree(void);

protected:
    CWnd* _parent;
    boost::shared_ptr<device_tree> _tree;

public:
    virtual bool create(CWnd* parent, unsigned int id);
    virtual HWND safe_hwnd(void) const { return GetSafeHwnd(); }
    virtual void cleanup(void);

protected:
    void initialize(void);
    void finalize(void);

protected:
    virtual void DoDataExchange(CDataExchange* pDX);
    virtual BOOL OnInitDialog();
    virtual BOOL PreTranslateMessage(MSG* pMsg) ;

    DECLARE_MESSAGE_MAP();

    afx_msg int  OnCreate(LPCREATESTRUCT lpCreateStruct);
    afx_msg void OnDestroy();
    afx_msg BOOL OnEraseBkgnd(CDC* pDC);
    afx_msg void OnSize(unsigned int nType, int cx, int cy);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_PANEL_DEVICE_TREE_H_
