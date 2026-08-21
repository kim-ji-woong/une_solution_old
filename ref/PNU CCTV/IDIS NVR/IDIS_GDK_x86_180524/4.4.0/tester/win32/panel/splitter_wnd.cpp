// splitter_wnd.cpp : implementation file
//

#include "stdafx.h"
#include "splitter_wnd.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

splitter_wnd::splitter_wnd(void)
    : _gripper(true)
{
    _ctr_cx =
    _ctr_cy =
    _ctn_cx =
    _ctn_cy =
    _cur_cx =
    _cur_cx = 0;

    _size.SetSize(0, 0);

    m_cxSplitter = m_cxSplitterGap = 6;
    m_cySplitter = m_cySplitterGap = 6;
    m_cxBorder = m_cyBorder = 0;
    m_cxBorderShare = m_cyBorderShare = 0;
}

splitter_wnd::~splitter_wnd(void)
{

}

//////////////////////////////////////////////////////////////////////////

IMPLEMENT_DYNCREATE(splitter_wnd, CSplitterWnd)

BEGIN_MESSAGE_MAP(splitter_wnd, CSplitterWnd)
    ON_WM_CREATE()
    ON_WM_ERASEBKGND()
    ON_WM_SIZE()
    ON_WM_LBUTTONUP()
    ON_WM_MOUSEMOVE()
    ON_WM_SHOWWINDOW()
END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////

bool splitter_wnd::create(int row, int col, CWnd* control, const CSize& size)
{
#ifdef _DEBUG
    ASSERT_VALID(this);
    assert(row >= 0 && row < m_nRows);
    assert(col >= 0 && col < m_nCols);
    assert(control != NULL);
    assert(control->m_hWnd != NULL);
    assert(control->GetDlgCtrlID() == IdFromRowCol(row, col));
    ASSERT_KINDOF(CWnd, control);

    if (GetDlgItem(IdFromRowCol(row, col)) != NULL) {

    }
    else {
        assert(!"control is created in advance");
        return false;
    }
#endif

    // set the initial size for that pane
    m_pColInfo[col].nIdealSize = size.cx;
    m_pRowInfo[row].nIdealSize = size.cy;

    if (row == 0) {
        _ctn_cy = size.cy;
    }
    else if (row == 1) {
        _ctr_cy = size.cy;
        _cur_cx = size.cy;
    }

    if (col == 0) {
        _ctr_cx = size.cx;
        _cur_cx = size.cx;
    }
    else if (col == 1) {
        _ctn_cx = size.cx;
    }

    return true;
}

void splitter_wnd::cleanup_context(void)
{
    if (::IsWindow(GetSafeHwnd())) {
        DestroyWindow();
    }
}

BOOL splitter_wnd::PreCreateWindow(CREATESTRUCT& cs)
{
    return CSplitterWnd::PreCreateWindow(cs);
}

void splitter_wnd::StartTracking(int ht)
{
    // hit_test return values (values and spacing between values is important)
    enum HitTestValue
    {
        noHit = 0,
        vSplitterBox = 1,
        hSplitterBox = 2,
        bothSplitterBox = 3, // just for keyboard
        vSplitterBar1 = 101,
        vSplitterBar15 = 115,
        hSplitterBar1 = 201,
        hSplitterBar15 = 215,
        splitterIntersection1 = 301,
        splitterIntersection225 = 525
    };

    CSplitterWnd::StartTracking(ht);

    GetInsideRect(m_rectLimit);

    if (ht >= vSplitterBar1 &&
        ht <= vSplitterBar15) {
        m_rectLimit.top = _ctn_cy + 3;
        m_rectLimit.bottom -= _ctr_cy;
    }
    else if (
        ht >= hSplitterBar1 &&
        ht <= hSplitterBar15) {
        m_rectLimit.left = _ctr_cx + 3;
        m_rectLimit.right -= _ctn_cx;
    }
}

void splitter_wnd::GetHitRect(int ht, CRect& rect)
{
    CSplitterWnd::GetHitRect(ht, rect);
}

void splitter_wnd::TrackRowSize(int y, int row)
{
    CSplitterWnd::TrackRowSize(y, row);

    if (row == 0) {
        CRect rect;
        GetClientRect(&rect);

        _cur_cx = rect.Height() - y;
    }
}

void splitter_wnd::TrackColumnSize(int x, int col)
{
    CSplitterWnd::TrackColumnSize(x, col);

    if (col == 0) {
        _cur_cx = x;
    }
}

void splitter_wnd::OnDrawSplitter(CDC* dc, ESplitType type, const CRect& rect)
{
    if (type == splitBorder) return;
    if (type != splitBar ||
       (dc == NULL)) {
        CSplitterWnd::OnDrawSplitter(dc, type, rect);
        return;
    }

    dc->FillSolidRect(rect, ::GetSysColor(COLOR_3DSHADOW));

    if (GetRowCount() > 1) {
        int l = rect.CenterPoint().x - __min(16, rect.Width() / 2 - 10);
        int r = rect.CenterPoint().x + __min(16, rect.Width() / 2 - 10);
        int y = rect.CenterPoint().y - 1;

        for (int x = l; x < r; x += 6) {
            dc->FillSolidRect(CRect(x, y, x + 2, y + 2), GetSysColor(COLOR_BTNFACE));
        }
    }
    if (GetColumnCount() > 1) {
        int t = rect.CenterPoint().y - __min(16, rect.Height() / 2 - 10);
        int b = rect.CenterPoint().y + __min(16, rect.Height() / 2 - 10);
        int x = rect.CenterPoint().x - 1;

        for (int y = t; y < b; y += 6) {
            dc->FillSolidRect(CRect(x, y, x + 2, y + 2), GetSysColor(COLOR_BTNFACE));
        }
    }
}

//////////////////////////////////////////////////////////////////////////

int splitter_wnd::OnCreate(LPCREATESTRUCT lpCreateStruct)
{
    return CSplitterWnd::OnCreate(lpCreateStruct);
}

BOOL splitter_wnd::OnEraseBkgnd(CDC* pDC)
{
    return CSplitterWnd::OnEraseBkgnd(pDC);
}

void splitter_wnd::OnSize(unsigned int nType, int cx, int cy)
{
    int cx_ctr, cx_ctn, cy_ctr, cy_ctn;

    // horizon
    if ((_size.cx != cx && m_pColInfo != NULL) &&
        (GetColumnCount() == 2)) {
        cx_ctr = (_ctr_cx < _cur_cx) ? _cur_cx + 11 : _ctr_cx;
        cx_ctn = (cx - cx_ctr);

        if (cx_ctn < _ctn_cx) {
            cx_ctn = _ctn_cx;
            cx_ctr = (cx - cx_ctn < 0) ? 0 : cx - cx_ctn;
        }
        if (cx_ctn >= 0) {
            SetColumnInfo(0, cx_ctr, 0);
            SetColumnInfo(1, cx_ctn, 0);
        }
    }

    // vertical
    if ((_size.cy != cy && m_pRowInfo != NULL) &&
        (GetRowCount() == 2)) {
        cy_ctr = (_ctr_cy < _cur_cx) ? _cur_cx + 11 : _ctr_cy;
        cy_ctn = (cy - cy_ctr);

        if (cy_ctn < _ctn_cy) {
            cy_ctn = _ctn_cy;
            cy_ctr = (cy - cy_ctn < 0) ? 0 : cy - cy_ctn;
        }
        if (cy_ctn >= 0) {
            SetRowInfo(0, cy_ctn, 0);
            SetRowInfo(1, cy_ctr, 0);
        }
    }

    _size.SetSize(cx, cy);

    CSplitterWnd::OnSize(nType, cx, cy);
}

void splitter_wnd::OnLButtonUp(unsigned int nFlags, CPoint point)
{
    CSplitterWnd::OnLButtonUp(nFlags, point);
}

void splitter_wnd::OnMouseMove(unsigned int nFlags, CPoint point)
{
    CSplitterWnd::OnMouseMove(nFlags, point);
}

void splitter_wnd::OnShowWindow(BOOL bShow, unsigned int nStatus)
{
    CSplitterWnd::OnShowWindow(bShow, nStatus);
}
