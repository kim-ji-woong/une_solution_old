// status_dlg.h : header file
//
#ifndef _CONTROL_STATUS_DLG_H_
#define _CONTROL_STATUS_DLG_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "G2ClientView.h"
#include <vector>

namespace client {

//////////////////////////////////////////////////////////////////////////

class StatusViewer :public CDialog
{
public:
    StatusViewer(CWnd* parent = NULL);
    virtual ~StatusViewer(void);
private:
    CWnd* _parent;
    CString _data; // status

public:
    int _channel;
    G2_PRODUCT_INFO _productInfo;
    bool _reconnect;

public:
    CString data(void) const;
    void setData(const CString& data);

protected:

    virtual void DoDataExchange(CDataExchange* pDX);
    virtual BOOL OnInitDialog();

    DECLARE_MESSAGE_MAP()

    afx_msg void OnBtnUpdateStatus();


};
//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_CONTROL_STATUS_DLG_H_