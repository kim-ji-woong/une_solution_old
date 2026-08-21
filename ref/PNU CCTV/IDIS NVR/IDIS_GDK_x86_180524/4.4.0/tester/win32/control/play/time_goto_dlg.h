// time_goto_dlg.h : header file
//

#ifndef _CONTROL_TIME_GOTO_DLG_H_
#define _CONTROL_TIME_GOTO_DLG_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

namespace client {

//////////////////////////////////////////////////////////////////////////

class time_goto_dlg : public CDialog
{
public:
	time_goto_dlg(CWnd* pParent = NULL);
	virtual ~time_goto_dlg(void);

protected:
    CWnd* _centerBase;
    CWnd* _parent;

    G2TIME _currentTime;
    bool _supportDate;

    CDateTimeCtrl _ctrlDate;
    CDateTimeCtrl _ctrlTime;

public:
    void set_center_base(CWnd* centerBase);
    bool set_current_time(const G2TIME& time);
    void set_support_date(bool support);

    const G2TIME& current_time(void) const { return _currentTime; }

protected:
	virtual void DoDataExchange(CDataExchange* pDX);
    virtual BOOL OnInitDialog();
    virtual void OnOK();

	DECLARE_MESSAGE_MAP()
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_CONTROL_TIME_GOTO_DLG_H_
