// clip_copy_password_dlg.h : header file
//

#pragma once

namespace client {

//////////////////////////////////////////////////////////////////////////

class clip_copy_password_dlg : public CDialog
{
    DECLARE_DYNAMIC(clip_copy_password_dlg)

public:
    clip_copy_password_dlg(CWnd* parent = NULL);
    virtual ~clip_copy_password_dlg();

private:
    CWnd*   _parent;
    CString _password;

public:
    CString get_password() const { return _password; }

protected:
    virtual void DoDataExchange(CDataExchange* pDX);

    DECLARE_MESSAGE_MAP()

    virtual void OnOK();
    virtual BOOL OnInitDialog();
};

//////////////////////////////////////////////////////////////////////////

}; // !_namespace_client
