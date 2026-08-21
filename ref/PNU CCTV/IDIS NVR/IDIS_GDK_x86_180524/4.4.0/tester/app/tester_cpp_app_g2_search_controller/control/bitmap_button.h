// bitmap_button.h : header file
//

#ifndef _BITMAP_BUTTON_H_
#define _BITMAP_BUTTON_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

namespace client {

//////////////////////////////////////////////////////////////////////////

class bitmap_button : public CButton
{
// Construction
public:
	bitmap_button(void);
	virtual ~bitmap_button(void);

public:
    struct image_ {
        enum TYPE {
	        UNDEFINED = -1,
	        NORMAL  = 0,
	        HOVER   = 1,
	        PRESSED = 2,
	        DISABLE = 3,

	        COUNT
        };
    };

    typedef struct button_info_ {
        UINT     _button_id;
        CString  _resource;
        CString  _tooltip;
        CRect    _rect;
        UINT     _flags;
        COLORREF _mask_color;
        COLORREF _back_color;

        button_info_ (UINT id, CString res, CRect rect, CString tooltip = CString(),
                  UINT flags = ILC_COLOR24, COLORREF mask = RGB(0, 0, 0), COLORREF back = -1)
            : _button_id(id)
            , _resource(res)
            , _rect(rect)
            , _tooltip(tooltip)
            , _flags(flags)
            , _mask_color(mask)
            , _back_color(back)
        {
        }
    } button_info;

protected:
    BOOL _is_tracking;
    BOOL _is_hover;
    BOOL _is_pressed;

    COLORREF _color_backgnd;
    COLORREF _color_text[image_::COUNT];
    COLORREF _color_text_outline;
    bool _use_outline_text;
    DWORD _string_format;

    CRect _rect_client;
    CRect _rect_margin;

    CImageList _image_list;
    CString _tooltip;
    CString _text;
    CToolTipCtrl _tooltip_ctrl;

public:
    bool create(CWnd* pParentWnd, const button_info& rbtnInfo);
    void enable(bool fenable = true);
    void show(bool fshow = true);
    void redraw(void);

    bool set_image_list(CImageList& rImgList);
    void draw_text_impl(CDC* pDc, int nImage);
    bool set_bitmap_button(const button_info& rbtnInfo);
    void set_tooltip_text(const CString& strToolTip);
    void set_text(const CString& strText);
    void set_text_format(DWORD format);
    void set_text_color(COLORREF color);
    void set_text_margin(const CRect& rect);
    void set_text_margin(int left, int top, int right, int bottom);
    void set_text_outline(bool useOutLine, COLORREF color = RGB(255, 255, 255));
    void set_font(CFont* pfont, bool update = true);

    CRect rect_margin(void) const	{ return _rect_margin; }

protected:
    virtual void PreSubclassWindow();
    virtual BOOL PreTranslateMessage(MSG* pMsg);
    virtual void DrawItem(LPDRAWITEMSTRUCT lpDrawItemStruct);
    virtual LRESULT WindowProc(UINT message, WPARAM wParam, LPARAM lParam);

    DECLARE_MESSAGE_MAP()

    afx_msg void OnSize(UINT nType, int cx, int cy);
    afx_msg void OnMouseMove(UINT nFlags, CPoint point);
    afx_msg LRESULT OnMouseLeave(WPARAM wparam, LPARAM lparam);
    afx_msg LRESULT OnMouseHover(WPARAM wparam, LPARAM lparam);
};

//////////////////////////////////////////////////////////////////////////

}; // !_namespace_client

#endif // !_BITMAP_BUTTON_H_
