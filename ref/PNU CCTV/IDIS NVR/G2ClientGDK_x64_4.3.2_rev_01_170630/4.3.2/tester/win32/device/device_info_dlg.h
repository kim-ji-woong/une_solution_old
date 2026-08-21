// device_info_dlg.h : header file
//

#ifndef _DEVICE_INFO_DLG_H_
#define _DEVICE_INFO_DLG_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "device_info_repository.h"

#pragma once

//////////////////////////////////////////////////////////////////////////

class device_info_dlg : public CDialog
{
public:
    device_info_dlg(CWnd* parent = NULL);
    virtual ~device_info_dlg(void);

protected:
    client::device_info _info;

public:
    HWND safe_hwnd(void) const { return GetSafeHwnd(); }

    CString site(void) const                            { return _info._site; }
    CString address(void) const                         { return _info._address; }
    CString id(void) const                              { return _info._id; }
    CString password(void) const                        { return _info._password; }
    unsigned int adminPort(void) const                  { return _info._adminPort; }
    unsigned int watchPort(void) const                  { return _info._watchPort; }
    unsigned int searchPort(void) const                 { return _info._searchPort; }
    unsigned int audioPort(void) const                  { return _info._audioPort; }
    client::device_info deviceInfo(void) const          { return _info; }

    void setSite(const CString& site)                   { _info._site = site; }
    void setAddress(const CString& address)             { _info._address = address; }
    void setId(const CString& id)                       { _info._id = id; }
    void setPassword(const CString& password)           { _info._password = password; }
    void setAdminPort(unsigned int port)                { _info._adminPort = port; }
    void setWatchPort(unsigned int port)                { _info._watchPort = port; }
    void setSearchPort(unsigned int port)               { _info._searchPort = port; }
    void setAudioPort(unsigned int port)                { _info._audioPort = port; }
    void setDeviceInfo(const client::device_info& info) { _info = info; }

protected:
    virtual void DoDataExchange(CDataExchange* pDX);
    virtual BOOL PreTranslateMessage(MSG* pMsg);
    virtual BOOL OnInitDialog();
    virtual void OnOK();
    virtual void OnCancel();

    DECLARE_MESSAGE_MAP()

    afx_msg int  OnCreate(LPCREATESTRUCT lpCreateStruct);
    afx_msg void OnDestroy();
};

//////////////////////////////////////////////////////////////////////////

#endif // !_DEVICE_INFO_DLG_H_