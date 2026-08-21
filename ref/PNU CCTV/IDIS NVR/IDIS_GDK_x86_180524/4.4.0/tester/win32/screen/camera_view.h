// camera_view.h : header file
//

#ifndef _SCREEN_CAMERA_VIEW_H_
#define _SCREEN_CAMERA_VIEW_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "screen_common.h"

namespace client {
    class screen_view;
    class painter;
    class text_in_queue;

//////////////////////////////////////////////////////////////////////////

class camera_view
{
public:
    camera_view(void);
    camera_view(short index, screen_view* screen);
    ~camera_view(void);

private:
    short   _index;
    short   _status;

    bool    _selected;
    bool    _visible;
    bool    _needDraw;
    bool    _stub;

    char    _modePTZ;
    int     _modeCamera;

    G2SPOT  _spot;
    G2TIME  _time;

    bool    _timeUpdate;
    CRect   _rect;
    CSize   _size;
    CRect   _rectCaption;
    CRect   _rectOSD;
    CSize   _resImageCrop;
    CSize   _resImage;
    CSize   _resPrior;

    bool _useDualAudio;
    bool _usePlayAudio;
    bool _useSoundCapture;

    std::wstring _title;
    std::wstring _title_ext;
    std::wstring _strdate;
    std::wstring _strtime;

    screen::img_ratio_::FACTOR _ratioFact;

protected:
    screen_view* _screen;

public:
    void initialize(short index, screen_view* screen);
    void reset(bool mode =  true, bool title = true);

public:
    void set_stub_camera(bool stub) { _stub = stub; }
    void set_visible(bool visible) { _visible = visible; }
    void set_selected(bool selected);
    void set_title(const wchar_t* title) { _title = title; }
    void set_title_ext(const wchar_t* title) { _title_ext = title; }
    void set_draw(bool needDraw = true) { _needDraw = needDraw; }
    void set_spot(const G2SPOT& spot) { _spot = spot; }
    void set_time(const G2TIME& time) { _timeUpdate = (_time._time != time._time); _time = time; }
    void set_time_update(bool update) { _timeUpdate = update; }
    void set_rect(const CRect& rect);
    void set_status(short status);
    void set_use_ptz(char modePTZ) { _modePTZ = modePTZ; }
    void set_camera_mode(int mode) { _modeCamera = mode; }
    void set_use_dual_audio(bool use) { _useDualAudio = use; }
    void set_use_play_audio(bool use) { _usePlayAudio = use; }
    void set_use_sound_capturing(bool use) { _useSoundCapture = use; }

public:
    bool is_stub(void) const { return _stub; }
    bool is_visible(void) const { return _visible; }
    bool is_selected(void) const { return _selected; }
    bool is_draw(bool reset = true);
    bool is_status(short status) const { return (_status == status); }
    char is_use_ptz(void) const { return _modePTZ; }
    bool is_enable(void) const { return (_status == screen::camera_status_::ENABLE); }
    bool is_no_video(void) const { return (_status == screen::camera_status_::NO_VIDEO); }
    bool is_inactivate(void) const { return (_status == screen::camera_status_::INACTIVATE); }
    bool is_not_connected(void) const { return (_status == screen::camera_status_::NOT_CONNECTED); }
    bool is_covert_level1(void) const { return (_status == screen::camera_status_::COVERT_L1); }
    bool is_covert_level2(void) const { return (_status == screen::camera_status_::COVERT_L2); }
    bool is_covert(void) const;
    bool is_mode(int mode) const { return (_modeCamera == mode); }
    bool is_live(void) const { return (_modeCamera == screen::camera_::WATCH); }
    bool is_play(void) const { return (_modeCamera == screen::camera_::SEARCH); }
    bool is_mode_none(void) const { return (_modeCamera == screen::camera_::UNDEFINED); }
    bool is_running(void) const { return (is_visible() && is_enable()); }
    bool is_use_dual_audio(void) const { return _useDualAudio; }
    bool is_use_play_audio(void) const { return _usePlayAudio; }
    bool is_use_sound_capturing(void) const { return _useSoundCapture; }

public:
    const CRect& rect(void) const { return _rect; }
    const CSize& size(void) const { return _size; }

    int width(void) const { return _size.cx; }
    int height(void) const { return _size.cy; }

    std::wstring title(void) const { return _title; }
    std::wstring title_ext(void) const { return _title_ext; }

    short status(void) const { return _status; }
    short index(void) const { return _index; }

    int camera_mode(void) const { return _modeCamera; }

    const CSize& image_size(void) const { return _resImage; }
    G2SPOT spot(void) const { return _spot; }

public:
    void __stdcall draw_image(painter* painter,
                              const unsigned char* image,
                              int cx, int cy);
    bool draw_status(painter* painter);
    void draw_border(painter* painter);
    void draw_osd(painter* painter);
    void draw_text_in_live(painter* painter, text_in_queue* queue);
    void draw_text_in_play(painter* painter, text_in_queue* queue, const G2SPOT& spot);
    void fill_rect(painter* painter);
    void clear_osd(painter* painter);

    void render(painter* painter);
    void present(painter* painter);

    /////////////////////////////////////////////

    bool pt_in_camera(const CPoint& point) const;

    typedef screen::img_ratio_::FACTOR ImageRatioFactor;
    void set_zoom_ratio(ImageRatioFactor factor) { _ratioFact = factor; }
    screen::img_ratio_::FACTOR image_ratio(void) const { return _ratioFact; }
    bool is_image_ratio(ImageRatioFactor factor) const { return _ratioFact == factor; }

protected:
    int center_horz(int left, int right);
    int center_vert(int top, int bottom);
    int center_horz(int left, int right, int cx);
    int center_vert(int top, int bottom, int cy);
    const RECT& center_rect(RECT& result, const RECT& dest, int cx, int cy);
    void constant_ratio(RECT& out, const RECT& in, int cx, int cy);
    void zoom_ratio(CRect& out, const CRect& in, int cx, int cy, screen::img_ratio_::FACTOR factor);
};

//////////////////////////////////////////////////////////////////////////

inline bool camera_view::pt_in_camera(const CPoint& point) const
{
    return (_rect.PtInRect(point) == TRUE);
}

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_SCREEN_CAMERA_VIEW_H_
