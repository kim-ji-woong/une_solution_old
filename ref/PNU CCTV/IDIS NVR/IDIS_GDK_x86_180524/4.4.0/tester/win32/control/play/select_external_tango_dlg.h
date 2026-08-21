// select_external_tango_dlg.h : header file
//

#ifndef _CONTROL_EXTERNAL_TANGO_DLG_H_
#define _CONTROL_EXTERNAL_TANGO_DLG_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_define_search.h>

namespace client {

//////////////////////////////////////////////////////////////////////////

class select_external_tango_dlg : public CDialog
{
public:
    select_external_tango_dlg(CWnd* parent = NULL);
    virtual ~select_external_tango_dlg(void);

private:
    CWnd* _parent;
    int   _selected;

    CListCtrl _list;
    
    std::vector<G2SEARCH_EXTERNAL_DISK> _tangoList;
    
public:
    void set_external_tango_info(const std::vector<G2SEARCH_EXTERNAL_DISK>& info);
    int selected(void) const { return _selected; }

protected:
    CString string_tango_disk_type(int type, int number);
    CString string_tango_disk_capacity(unsigned __int64 capacity);
    CString string_tango_disk_format(int format);

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

#endif // !_CONTROL_EXTERNAL_TANGO_DLG_H_
