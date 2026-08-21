// controller_base.h : haeder file
//

#ifndef _CONTROLLER_BASE_H_
#define _CONTROLLER_BASE_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

namespace client {

//////////////////////////////////////////////////////////////////////////

class controller_base : public CDialog {
public:
    controller_base(void)
        : _parent(NULL) {
    }
    virtual ~controller_base(void) {}

protected:
    CWnd* _parent;

protected:
    virtual void initialize(void) = 0;
    virtual void finalize(void) = 0;

public:
    virtual bool create(CWnd* parent) { return false; }
    virtual HWND safe_hwnd(void) const { return GetSafeHwnd(); }
    virtual void on_screen_layout_changed(int layout, int changed) {}
    virtual void on_screen_image_drew(short camera, const G2SPOT& spot) {}
};


//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_CONTROLLER_BASE_H_