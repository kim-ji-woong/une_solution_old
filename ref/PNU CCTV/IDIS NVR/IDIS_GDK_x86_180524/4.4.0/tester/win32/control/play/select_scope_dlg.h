// select_scope_dlg.h : header file
//

#ifndef _CONTROL_SELECT_SCOPE_DLG_H_
#define _CONTROL_SELECT_SCOPE_DLG_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

namespace client {

//////////////////////////////////////////////////////////////////////////

class select_scope_dlg : public CDialog
{
public:
    select_scope_dlg(CWnd* parent = NULL);
    virtual ~select_scope_dlg(void);

private:
    CWnd* _parent;
    int   _selected;

    CListCtrl _list;

    typedef std::vector<G2SCOPE> ScopeList;
    ScopeList _scopeList;

public:
    void set_scope_list(const std::vector<G2SCOPE>& list);
    int get_selected_index(void) const { return _selected; }

protected:
    virtual void DoDataExchange(CDataExchange* pDX);
    virtual BOOL OnInitDialog();

    DECLARE_MESSAGE_MAP()

    afx_msg void OnDestroy();
    afx_msg BOOL OnEraseBkgnd(CDC* pDC);
    afx_msg void OnClickList(NMHDR *pNMHDR, LRESULT *pResult);
    afx_msg void OnOK();
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_CONTROL_SELECT_SCOPE_DLG_H_
