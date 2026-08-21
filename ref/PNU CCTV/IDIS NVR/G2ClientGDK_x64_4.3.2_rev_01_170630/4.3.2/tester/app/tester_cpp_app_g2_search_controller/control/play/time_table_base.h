// time_table_base.h : header file
//

#ifndef _TIME_TABLE_BASE_H_
#define _TIME_TABLE_BASE_H_

#include "listener/listener_time_table.h"
#include "search/search_common.h"

#include <vector>
#include <afxmt.h>

namespace client {

//////////////////////////////////////////////////////////////////////////

class time_table_base : public CWnd
{
    DECLARE_DYNAMIC(time_table_base)
public:
    time_table_base(void);
    virtual ~time_table_base(void);

protected:
    struct cursor_ {
        enum TYPE {
            FINGER = 0,
            HAND,
            GRIP,
            COUNT
        };
    };

    struct drag_ {
        enum TYPE {
            OFF = 0,
            DATETIME,
            TIMEBAR,
        };
    };

    struct default_ {
        enum VAL {
            TIMETABLE_WIDTH = (60 * 24),
            TIMETABLE_HEAD_HEIGHT = 30,
            TIMETABLE_CAMERA_TITLE_WIDTH = 150,
            TIMETABLE_TIME_BOUNDARY_HEIGHT = 10,
            TIMETABLE_BAR_HEIGHT = 10,
            TIMETABLE_ROW_HEIGHT = TIMETABLE_BAR_HEIGHT + 10,
            TIMETABLE_BORDER = 3,
        };
    };

protected:
    CWnd* _parent;
    listener_time_table* _listener;

    CRect _rect;
    CRect _rectHead;
    CRect _rectBody;

    CFont _font;

    CDC _dc;
    CDC _surface;
    HBITMAP _surfaceDIB;
    BITMAPINFO _surfaceInfo;
    void* _surfaceBuffer;
    CSize _surface_size;

    HCURSOR _cursor[cursor_::COUNT];
    drag_::TYPE _dragMode;
    CPoint _beginPoint;

    int _beginOffset;
    int _selectPos;

    bool _initialized;
    bool _enable;
    bool _created;

    g2::critical_section _lock_data;
    g2::critical_section _lock_surface;

public:
    bool create(CWnd* parent, const CRect& rect, unsigned int id);
    void set_listenr(listener_time_table* listener);
    void set_enable(bool enable) { _enable = enable; }
    void clear(void);
    void show(bool show);
    void resize(const CRect& rect);
    
    bool is_initialized(void) const { return _initialized; }
    bool is_enable(void) const { return _enable; }
    bool is_created(void) const { return _created; }

    virtual void screen_image_drew(const G2SPOT& spot) = 0;

protected:
    virtual void initialize(void);
    virtual void finalize(void);

    void initialize_surface(int cx, int cy);
    void finalize_surface(void);

    void draw_backgnd(CDC* pDC);
    void draw_title(CDC* pDC);
    void draw_select_spot(CDC* pDC);

    void render(void);
    void present(CDC* pDC);

    void prepare_bitmap_info(int cx, int cy, int bpp, BITMAPINFO& bi);

    virtual void draw_time(CDC* pDC) = 0;
    virtual void draw_time_bar(CDC* pDC) = 0;
    virtual bool set_select_pos(int pos, bool request = true);
    virtual bool color_rec_time_info(int type, COLORREF& color) = 0;

protected:
    virtual void DoDataExchange(CDataExchange* pDX);

    DECLARE_MESSAGE_MAP()

    afx_msg int  OnCreate(LPCREATESTRUCT lpCreateStruct);
    afx_msg void OnDestroy();
    afx_msg void OnSize(UINT nType, int cx, int cy);
    afx_msg BOOL OnEraseBkgnd(CDC* pDC);
    afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
    afx_msg void OnLButtonUp(UINT nFlags, CPoint point);
    afx_msg void OnMouseMove(UINT nFlags, CPoint point);
    afx_msg BOOL OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message);
    afx_msg void OnVScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_TIME_TABLE_BASE_H_
