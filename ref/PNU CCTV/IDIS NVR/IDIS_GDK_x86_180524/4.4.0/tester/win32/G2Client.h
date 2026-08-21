// G2Client.h : header file
//

#pragma once

#ifndef __AFXWIN_H__
	#error "include 'stdafx.h' before including this file for PCH"
#endif

#include "resource.h"       // main symbols

//////////////////////////////////////////////////////////////////////////

class CG2ClientApp : public CWinApp
{
public:
	CG2ClientApp(void);
    virtual ~CG2ClientApp(void);

    static CG2ClientApp& get(void);

protected:
    volatile bool _isinitialized;
    volatile bool _isfinalize;

public:
    void set_initialized(void);
    void set_finalize(void);

    bool is_initialized(void)  const { return _isinitialized;    }
    bool is_finalize(void)     const { return _isfinalize;       }

    volatile const bool& get_initialized_ref(void) const { return _isinitialized; }

public:
	virtual BOOL InitInstance(void);

    DECLARE_MESSAGE_MAP()

	afx_msg void OnAppAbout();
};

//////////////////////////////////////////////////////////////////////////

extern CG2ClientApp g_app;

namespace client {
    typedef CG2ClientApp app;
}

inline client::app& client::app::get(void)
{
    return g_app;
}
