// time_table.h : header file
//

#ifndef _TIME_TABLE_H_
#define _TIME_TABLE_H_

#include "listener_time_table.h"

#include <search/search_common.h>
#include <vector>
#include <afxmt.h>

namespace client {

//////////////////////////////////////////////////////////////////////////

class time_table : public CWnd
{
    DECLARE_DYNAMIC(time_table)
public:
    time_table(void);
    virtual ~time_table(void);

public:
    struct time_table_data {
        std::wstring _title;
        std::vector<int> _data;

        time_table_data(void) {
            _title = L"";
            _data.resize(1440, 0);
        }
    };

private:
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

    HCURSOR _cursor[cursor_::COUNT];
    drag_::TYPE _dragMode;
    CPoint _beginPoint;

    int _beginOffset;
    int _selectPos;
    G2SPOT _spot;

    std::vector<time_table::time_table_data> _rtiList;

    bool _initialized;
    bool _enable;

    g2::critical_section _lock_data;
    g2::critical_section _lock_surface;

public:
    bool create(CWnd* parent, const CRect& rect, unsigned int id);
    void set_listenr(listener_time_table* listener);
    void update(const G2SPOT& spot, const std::vector<time_table::time_table_data>& list);
    void screen_image_drew(const G2TIME& time);
    void clear(void);

    bool is_initialized(void) const { return _initialized; }

protected:
    void initialize(void);
    void finalize(void);

    void initialize_surface(int cx, int cy);
    void finalize_surface(void);

    void draw_backgnd(CDC* pDC);
    void draw_time(CDC* pDC);
    void draw_time_bar(CDC* pDC);
    void draw_title(CDC* pDC);
    void draw_select_spot(CDC* pDC);

    void render(void);
    void present(CDC* pDC);

    COLORREF color_rec_time_info(int type);
    void prepare_bitmap_info(int cx, int cy, int bpp, BITMAPINFO& bi);
    void set_select_spot(int pos, bool request = true);

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

#endif // !_TIME_TABLE_H_
