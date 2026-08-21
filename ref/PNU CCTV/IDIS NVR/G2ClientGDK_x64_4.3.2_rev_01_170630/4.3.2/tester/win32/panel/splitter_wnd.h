// splitter_wnd.h : header file
//

#ifndef _SPLITTER_WND_H_
#define _SPLITTER_WND_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

namespace client {

//////////////////////////////////////////////////////////////////////////

class splitter_wnd : public CSplitterWnd
{
    DECLARE_DYNCREATE(splitter_wnd)

public:
    splitter_wnd(void);
    virtual ~splitter_wnd(void);

protected:
    bool    _gripper;
    int     _ctr_cx, _ctn_cx, _cur_cx;   // col
    int     _ctr_cy, _ctn_cy, _cur_cy;   // row
    CSize   _size;

public:
    HWND safe_hwnd(void) const { return GetSafeHwnd(); }
    bool create(int row, int col, CWnd* control, const CSize& size);

    void set_content_cx(int cx) { _ctn_cx = cx; }
    void set_content_cy(int cy) { _ctn_cy = cy; }
    void set_control_cx(int cx) { _ctr_cx = cx; }
    void set_control_cy(int cy) { _ctr_cy = cy; }

protected:
    virtual void cleanup_context(void);

protected:
    virtual BOOL PreCreateWindow(CREATESTRUCT& cs);
    virtual void StartTracking(int ht);
    virtual void GetHitRect(int ht, CRect& rect);
    virtual void TrackRowSize(int y, int row);
    virtual void TrackColumnSize(int x, int col);
    virtual void OnDrawSplitter(CDC* dc, ESplitType type, const CRect& rect);

    DECLARE_MESSAGE_MAP()

    afx_msg int  OnCreate(LPCREATESTRUCT lpCreateStruct);
    afx_msg BOOL OnEraseBkgnd(CDC* dc);
    afx_msg void OnSize(unsigned int type, int cx, int cy);
    afx_msg void OnLButtonUp(unsigned int flags, CPoint point);
    afx_msg void OnMouseMove(unsigned int flags, CPoint point);
    afx_msg void OnShowWindow(BOOL show, unsigned int status);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_SPLITTER_WND_H_
