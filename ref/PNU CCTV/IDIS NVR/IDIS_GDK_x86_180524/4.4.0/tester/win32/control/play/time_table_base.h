// time_table_base.h : header file
//

#ifndef _TIME_TABLE_BASE_H_
#define _TIME_TABLE_BASE_H_

#include "listener/listener_time_table.h"
#include "search/search_common.h"
#include "search/search_data.h"
#include "search/search_minute_info.h"

#include <set>
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
        };
    };

    struct table_data {
        struct time_data {
            int _pos;
            CTime _time;
            int _segment;
            int _tick;
            int _rec_type;

            time_data(int pos = -1, const CTime& time = CTime(), int segment = 0, int tick = 0, int rec_type = 0)
                : _pos(pos)
                , _time(time)
                , _segment(segment) 
                , _tick(tick)
                , _rec_type(rec_type)
            {
            }
        };

        void clear(void) {
            _title = L"";
            _data.clear();
        }

        void resize(size_t size, int channelext) {
            clear();
            _channelext = channelext;
            std::vector<time_data>(size).swap(_data);
        }

        int _channelext;
        std::wstring _title;
        std::vector<time_data> _data;
    };

    typedef std::vector<table_data>     table_data_list;
    typedef table_data::time_data       time_data;
    typedef std::vector<time_data>      time_data_list;

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
    CSize _surfaceSize;

    HCURSOR _cursor[cursor_::COUNT];
    drag_::TYPE _dragMode;
    CPoint _beginPoint;
    int _beginOffset;
    int _selectPos;

    bool _initialized;
    bool _enable;

    table_data_list _data_list;

    g2::critical_section _lock_data;
    g2::critical_section _lock_surface;

protected:
    table_data_list& data_list(void) { return _data_list; }
    void reset_data_list(size_t size = 1) { table_data_list(size > 1 ? size : 1).swap(_data_list); }
    time_data_list& time_list(void) { return _data_list.front()._data; }

    virtual void set_data(search_data_ptr data, const std::set<int>& cameras) = 0;

public:
    bool create(CWnd* parent, const CRect& rect, unsigned int id);
    void set_listenr(listener_time_table* listener);
    void set_enable(bool enable) { _enable = enable; }
    void clear(void);
    void show(bool show);
    virtual void update(search_data_ptr data, const std::set<int>& cameras) = 0;

    bool is_initialized(void) const { return _initialized; }
    bool is_enable(void) const { return _enable; }

protected:
    virtual void initialize(void);
    virtual void finalize(void);
    void initialize_surface(int cx, int cy);
    void finalize_surface(void);
    void prepare_bitmap_info(int cx, int cy, int bpp, BITMAPINFO& bi);

    void draw_backgnd(CDC* pDC);
    void draw_time(CDC* pDC);
    void draw_time_bar(CDC* pDC);
    void draw_title(CDC* pDC);
    void draw_select_spot(CDC* pDC);
    void render(void);
    void present(CDC* pDC);

protected:
    int find_pos_by_spot(const G2SPOT& spot);

public:
    void screen_image_drew(const G2SPOT& spot);
    void set_select_pos(int pos, bool request = true);

    virtual bool is_valid_rec_type(int type) = 0;
    virtual bool get_rec_type_color(int type, COLORREF& color) = 0;
    
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
