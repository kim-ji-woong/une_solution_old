// select_segment_dlg.h : header file
//

#ifndef _CONTROL_SELECT_SEGMENT_DLG_H_
#define _CONTROL_SELECT_SEGMENT_DLG_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <vector>

namespace client {

//////////////////////////////////////////////////////////////////////////

class select_segment_dlg : public CDialog
{
public:
    select_segment_dlg(CWnd* parent = NULL);
    virtual ~select_segment_dlg(void);

public:
    enum {
        SELECT_MODE_SCOPE = 0,
        SELECT_MODE_SPOT,
        SELECT_MODE_ID
    };

private:
    CWnd* _parent;
    int   _mode;
    int   _selected;
    int   _count;

    CListCtrl _list;

    typedef std::vector<G2SCOPE> ScopeList;
    typedef std::vector<G2SPOT> SpotList;

    SpotList _spotList;

public:
    void set_mode(int mode) { _mode = mode; }
    void set_scope_info(const std::vector<G2SCOPE>& list);
    void set_spot_info(const std::vector<G2SPOT>& list);
    void set_segment_count(int count) { _count = count; }
    void set_segment_id(int id) { _selected = id; }

    int selected(void) const { return _selected; }
    G2SPOT selected_spot(void) const;

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

#endif // !_CONTROL_SELECT_SEGMENT_DLG_H_
