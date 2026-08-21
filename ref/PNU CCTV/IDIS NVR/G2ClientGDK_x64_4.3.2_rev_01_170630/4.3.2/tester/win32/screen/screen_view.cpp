// screen_view.cpp : implementation file
//

#include "stdafx.h"
#include "screen_view.h"
#include "camera_view.h"
#include "screen_listener.h"
#include "painter/painter.h"
#include "text_in/text_in_queue.h"
#include <common/client_validate.h>
#include <utility/g2retention_bit.h>

#include <vector>
#include <functional>
#include <algorithm>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

#pragma warning(disable : 4800)

using namespace client;
using namespace screen;

//////////////////////////////////////////////////////////////////////////

enum user_message_ {
    UM_POST_SETFOCUS = WM_USER + 100,
    UM_POST_FIRE_CAMERA_CHANGED,
};

//////////////////////////////////////////////////////////////////////////

screen_view::screen_view(void)
    : _listener(NULL)
    , _painter(NULL)
    , _cameraView(NULL)
    , _parent(NULL)
    , _dropTarget(NULL)
    , _dropParent(NULL)
{
    _currLayout     = screen_formatter_::LAYOUT_UNDEFINED;
    _prevLayout     = _currLayout;
    _currCameraLT   = 0;
    _prevCameraLT   = 0;
    _selcameraPrev  = -1;
    _selcamera      = 0;
    _countcamera    = 0;

    _modeBaseLayout = screen::layout_base_::PAGE;
    _activate       = false;
    _lockUpdate     = false;

    _factImageRatio = screen::img_ratio_::CONSTANT;

    _invokeDropMessage = _UI32_MAX;
    _secureDropMessage = _UI32_MAX;

    _textIn._queue = NULL;
}

screen_view::~screen_view(void)
{
    cleanup();
}

//////////////////////////////////////////////////////////////////////////

bool screen_view::create(CWnd* parent,
                         const CRect& rect,
                         unsigned int id,
                         short countcamera,
                         screen_formatter_::LAYOUT layout /*= screen_formatter_::LAYOUT_6x6*/)
{
    G2RETURN_VAL_IF_ASSERT(parent != NULL && ::IsWindow(parent->GetSafeHwnd()), false);

    _countcamera = (countcamera < client::MAX_SCREEN_CAMERA_COUNT) ? countcamera : client::MAX_SCREEN_CAMERA_COUNT;

    _fmtScreen.create(_countcamera);

    _parent = parent;
    _currLayout = layout;
    _currCameraLT = 0;
    _prevCameraLT = 0;
    _rctScreen = CRect(CPoint(0, 0), rect.Size());

    /////////////////////////////////////////////

    BOOL created = CWnd::Create(NULL,
                                _T(""),
                                WS_CHILD | WS_CLIPCHILDREN | WS_CLIPSIBLINGS,
                                _rctScreen,
                                parent,
                                id);
    if (created) {

    }
    else {
        assert(!"failed to create screen view");
    }
    return (created == TRUE);
}

bool screen_view::cleanup(void)
{
    unregister_drag_drop();

    SAFE_DELETE_ARR(_cameraView);

    if (_painter) {
        _painter->finalize();
    }
    SAFE_DELETE(_painter);
    SAFE_DELETE(_dropTarget);
    SAFE_DELETE_ARR(_textIn._queue);

    if (GetSafeHwnd()) {
        DestroyWindow();
    }

    return true;
}

void screen_view::set_listener(screen_listener* listener)
{
    _listener = listener;
}

void screen_view::set_drag_drop(CWnd* dropParent, unsigned int invokeMessageId, unsigned int secureMessageId)
{
    assert(!_dropTarget);

    if (dropParent != NULL && dropParent->GetSafeHwnd()) {
        _dropParent = dropParent;
        _invokeDropMessage = invokeMessageId;
        _secureDropMessage = secureMessageId;
        register_drag_drop();
    }
    else {
        assert(false);
    }
}

bool screen_view::enable_window(bool enable /*= true*/)
{
    G2RETURN_VAL_IF_FAIL(this != NULL && GetSafeHwnd() != NULL, false);

    return (EnableWindow(enable ? TRUE : FALSE) == TRUE);
}

bool screen_view::show(int command /*= SW_SHOWNORMAL*/)
{
    G2RETURN_VAL_IF_FAIL(this != NULL && GetSafeHwnd() != NULL, false);

    return ShowWindow(command) ? true : false;
}

void screen_view::set_focus(void)
{
    G2RETURN_IF_FAIL(this != NULL && GetSafeHwnd() != NULL);

    if (GetFocus() != this) {
        SetFocus();
    }
}

void screen_view::set_focus_post(void)
{
    PostMessage(UM_POST_SETFOCUS);
}

bool screen_view::set_window_pos(const CWnd* insertafter, const CRect& rect, unsigned int flags)
{
    G2RETURN_VAL_IF_FAIL(this != NULL && GetSafeHwnd() != NULL, false);

    return (SetWindowPos(insertafter,
                         rect.left, rect.top, rect.Width(), rect.Height(),
                         flags) != FALSE);
}

void screen_view::move_window(const CRect& rect, bool repaint /*= true*/)
{
    G2RETURN_IF_FAIL(this != NULL && GetSafeHwnd() != NULL);

    MoveWindow(rect, repaint);
}

LRESULT screen_view::send_message(unsigned int message, WPARAM wParam /*= 0*/, LPARAM lParam /*= 0L*/)
{
    LRESULT result = 1L;
    if (GetSafeHwnd() != NULL && ::IsWindow(GetSafeHwnd())) {
        result = (SendMessage(message, wParam, lParam)) ? 0L : 1L;
    }
    return result;
}

bool screen_view::post_message(unsigned int message, WPARAM wParam /*= 0*/, LPARAM lParam /*= 0L*/)
{
    bool result = false;
    if (GetSafeHwnd() != NULL && ::IsWindow(GetSafeHwnd())) {
        result = (PostMessage(message, wParam, lParam)) ? true : false;
    }
    return result;
}

//////////////////////////////////////////////////////////////////////////

void screen_view::display_frame(const unsigned char* image,
                                short cx, short cy,
                                short scrcamera,
                                const G2FRAME& frame)
{
    if (client::valid_camera(scrcamera) != true) return;

    camera_view& camview = _cameraView[scrcamera];

    if (_activate != true) {
        if (camview.is_play() &&
            camview.is_visible() &&
            camview.is_enable()) {
            if (_listener) {
                _listener->on_screen_image_drew(scrcamera, frame._index._spot);
            }
        }
        return;
    }

    g2::scoped_criticalsection lock(_lock_exec);

    G2RETURN_IF_FAIL(_painter != NULL);

    const G2SPOT& spot = frame._index._spot;

    if (camview.is_visible() &&
        camview.is_enable()) {
    //  camview.set_title(frame._title);
        camview.set_spot(spot);
        camview.set_time(spot._time);
        camview.set_draw(false);
        camview.draw_image(_painter, image, cx, cy);
        camview.draw_osd(_painter);

        if (is_layout_1x1()) {
            display_text_in(scrcamera, spot);
        }
        camview.draw_border(_painter);

        ///////////////////////////////////////////////////

        camview.render(_painter);
        camview.present(_painter);
    }

    lock.free();

    if (camview.is_play() &&
        _listener) {
        _listener->on_screen_image_drew(scrcamera, spot);
    }
}

//////////////////////////////////////////////////////////////////////////

void screen_view::reset_screen(bool mode, bool title, bool update)
{
    g2::scoped_criticalsection lock(_lock_exec);

    for (int i = 0; i < _countcamera; ++i) {
        _cameraView[i].reset(mode, title);
        _textIn._queue[i].clear();
    }

    if (update) {
        execute_command(command_::UPDATE_ALL);
    }
}

void screen_view::reset_camera(short camera, bool mode, bool title, bool update)
{
    G2RETURN_IF_FAIL(client::valid_camera(camera));
    G2RETURN_IF_FAIL(_cameraView != NULL);

    g2::scoped_criticalsection lock(_lock_exec);

    _cameraView[camera].reset(mode, title);
    _textIn._queue[camera].clear();

    if (update) {
        update_camera(camera);
    }
}

void screen_view::reset_camera(unsigned __int64 cameras, bool mode, bool title, bool update)
{
    g2::scoped_criticalsection lock(_lock_exec);

    for (int i = 0; i < _countcamera; ++i) {
        if (g2::contains_bit(cameras, i)) {
            _cameraView[i].reset(mode, title);
            _textIn._queue[i].clear();
        }
    }

    if (update) {
        update_camera(cameras);
    }
}

void screen_view::fire_connected(short camera)
{
    int selcamera = _selcamera;

    if (!client::valid_camera(camera)) {
        camera = selcamera;
    }
    else {
        if (camera != selcamera) {
            return;
        }
    }

    if (_listener) {
        _listener->on_screen_camera_changed(camera);
    }
}

void screen_view::fire_disconnected(short camera)
{
    int selcamera = _selcamera;

    if (!client::valid_camera(camera)) {
        camera = selcamera;
    }
    else {
        if (camera != selcamera) {
            return;
        }
    }

    if (_listener) {
        _listener->on_screen_camera_changed(camera);
    }
}

void screen_view::fire_camera_changed(short camera, bool post /*= true*/)
{
    const int selcamera = _selcamera;

    if (!client::valid_camera(camera)) {
        camera = selcamera;
    }
    else {
        if (camera != selcamera) {
            return;
        }
    }

    if (post) {
        post_message(UM_POST_FIRE_CAMERA_CHANGED, static_cast<WPARAM>(camera));
    }
    else {
        if (_listener) {
            _listener->on_screen_camera_changed(camera);
        }
    }
}

short screen_view::find_camera_by_point(const CPoint& point) const
{
    short camera = -1;
    for (int i = 0; i < _countcamera; ++i) {
        const camera_view& camview = _cameraView[i];
        if (camview.is_visible() &&
            camview.pt_in_camera(point)) {
            camera = camview.index();
            break;
        }
    }
    return camera;
}

void screen_view::set_activate(bool activate /*= true*/)
{
    G2RETURN_IF_FAIL(GetSafeHwnd() != NULL && ::IsWindow(GetSafeHwnd()));

    _activate = activate;

    if (activate) {
        SetFocus();
    }
}

void screen_view::set_camera(short camera, bool redraw /*= true*/)
{
    if (camera == -1) {
        camera_view& camview = _cameraView[_selcamera];
        if (camview.is_selected() &&
            camview.is_visible()) {
            g2::scoped_criticalsection lock(_lock_exec);
            camview.set_selected(false);
            camview.draw_border(_painter);
            camview.render(_painter);
            camview.present(_painter);
        }
        return;
    }

    if (valid_camera(camera) != true) return;

    _selcameraPrev = _selcamera;

    execute_command(command_::SET_CAMERA, WPARAM(camera), LPARAM(redraw));
}

void screen_view::set_layout_base(int base)
{
    _modeBaseLayout = screen::layout_base_::MODE(base);
}

void screen_view::set_layout(int layout)
{
    G2RETURN_IF_ASSERT(_fmtScreen.valid_layout(layout));

    if (layout == _currLayout) {
        return;
    }

    execute_command(command_::SET_LAYOUT, WPARAM(layout));
}

void screen_view::set_camera_layout(short camera, int layout)
{
    G2RETURN_IF_ASSERT(_fmtScreen.valid_layout(layout));

    if (client::valid_camera(camera) != true) {
        camera = 0;
    }

    set_camera(camera);
    set_layout(layout);
}

void screen_view::set_camera_count_layout(short camera, int countcamera)
{
    if (!valid_camera(camera)) {
        camera = 0;
    }

    int layout = _fmtScreen.secure_layout_for_camera_count(countcamera);

    set_camera(camera);
    set_layout(layout);
}

void screen_view::set_layout_page_prev(void)
{
    execute_command(command_::PREV_LAYOUT);
}

void screen_view::set_layout_page_next(void)
{
    execute_command(command_::NEXT_LAYOUT);
}

//////////////////////////////////////////////////////////////////////////

void screen_view::set_camera_title(short camera, const std::wstring& title, bool update /*= true*/)
{
    G2RETURN_IF_FAIL(client::valid_camera(camera));

    if (camera_title(camera) != title) {
        execute_command(command_::SET_CAMERA_TITLE, MAKEWPARAM(camera, update), (LPARAM)(static_cast<LPCTSTR>(title.c_str())));
    }
}

void screen_view::set_camera_title(unsigned __int64& cameras, const std::wstring& title, bool update /*= true*/)
{
    for (short i = 0; i < _countcamera; ++i) {
        if (g2::contains_bit(cameras, i)) {
            set_camera_title(i, title, update);
        }
    }
}

void screen_view::set_camera_title_ext(short camera, const std::wstring& title, bool update /*= true*/)
{
    G2RETURN_IF_FAIL(client::valid_camera(camera));

    if (camera_title_ext(camera) != title) {
        execute_command(command_::SET_CAMERA_TITLE_EXT, MAKEWPARAM(camera, update), (LPARAM)(static_cast<LPCTSTR>(title.c_str())));
    }
}

void screen_view::set_camera_title_ext(unsigned __int64& cameras, const std::wstring& title, bool update /*= true*/)
{
    for (short i = 0; i < _countcamera; ++i) {
        if (g2::contains_bit(cameras, i)) {
            set_camera_title_ext(i, title, update);
        }
    }
}

void screen_view::set_camera_status(short camera, short status, bool update /*= true*/)
{
    G2RETURN_IF_FAIL(client::valid_camera(camera));

    execute_command(command_::SET_CAMERA_STATUS, MAKEWPARAM(camera, status), static_cast<LPARAM>(update));
}

void screen_view::set_camera_status(unsigned __int64 cameras, short status, bool update /*= true*/)
{
    for (short i = 0; i < _countcamera; ++i) {
        if (g2::contains_bit(cameras, i)) {
            set_camera_status(i, status, update);
        }
    }
}

void screen_view::set_use_camera_ptz(short camera, char use /*= G2LIVE_CAMERA_STATUS::PTZ_NONE*/)
{
    G2RETURN_IF_FAIL(client::valid_camera(camera));

    _cameraView[camera].set_use_ptz(use);
}

void screen_view::set_camera_mode(short camera, screen::camera_::MODE cameraMode)
{
    G2RETURN_IF_FAIL(client::valid_camera(camera));
    G2RETURN_IF_FAIL(_cameraView != NULL);

    _cameraView[camera].set_camera_mode(cameraMode);
}

void screen_view::set_camera_mode(unsigned __int64 cameras, screen::camera_::MODE cameraMode)
{
    for (short i = 0; i < _countcamera; ++i) {
        if (g2::contains_bit(cameras, i)) {
            set_camera_mode(i, cameraMode);
        }
    }
}

void screen_view::set_camera_visible(short camera, bool visible /*= true*/)
{
    G2RETURN_IF_FAIL(client::valid_camera(camera));

    _cameraView[camera].set_visible(visible);
}

void screen_view::set_camera_use_dual_audio(short camera, bool use, bool update /*= true*/)
{
    G2RETURN_IF_FAIL(client::valid_camera(camera));

    camera_view& camview = _cameraView[camera];
    if (camview.is_use_dual_audio() == use) return;

    camview.set_use_dual_audio(use);

    if (update) {
        update_camera(camera, false);
    }
}

void screen_view::set_camera_use_dual_audio(unsigned __int64& cameras, bool use, bool update /*= true*/)
{
    if (cameras == 0x00ui64) return;
    for (short i = 0; i < _countcamera; ++i) {
        if (g2::contains_bit(cameras, i)) {
            set_camera_use_dual_audio(i, use, false);
        }
    }
    if (update) {
        update_camera(cameras, false);
    }
}

void screen_view::set_camera_use_sound_capturing(short camera, bool use, bool update /*= true*/)
{
    G2RETURN_IF_FAIL(client::valid_camera(camera));

    camera_view& camview = _cameraView[camera];
    if (camview.is_use_sound_capturing() == use) return;

    camview.set_use_sound_capturing(use);

    if (update) {
        update_camera(camera, false);
    }
}

void screen_view::set_camera_use_sound_capturing(unsigned __int64& cameras, bool use, bool update /*= true*/)
{
    if (cameras == 0x00ui64) return;
    for (short i = 0; i < _countcamera; ++i) {
        if (g2::contains_bit(cameras, i)) {
            set_camera_use_sound_capturing(i, use, false);
        }
    }
    if (update) {
        update_camera(cameras, false);
    }
}

const std::wstring screen_view::camera_title(short camera) const
{
    return (client::valid_camera(camera)) ?
            _cameraView[camera].title() : std::wstring();
}

const std::wstring screen_view::camera_title_ext(short camera) const
{
    return (client::valid_camera(camera)) ?
            _cameraView[camera].title_ext() : std::wstring();
}

bool screen_view::is_camera_mode_none(short camera) const
{
    G2RETURN_VAL_IF_FAIL(valid_camera(camera), false);
    G2RETURN_VAL_IF_FAIL(_cameraView != NULL, false);

    return _cameraView[camera].is_mode_none();
}

bool screen_view::is_camra_mode(short camera, int mode) const
{
    G2RETURN_VAL_IF_FAIL(valid_camera(camera), false);
    G2RETURN_VAL_IF_FAIL(_cameraView != NULL, false);

    return _cameraView[camera].is_mode(mode);
}

bool screen_view::is_camera_status(short camera, screen::camera_status_::TYPE status) const
{
    G2RETURN_VAL_IF_FAIL(valid_camera(camera), false);
    G2RETURN_VAL_IF_FAIL(_cameraView != NULL, false);

    return (_cameraView[camera].status() == status);
}

char screen_view::is_use_camera_ptz(short camera) const
{
    G2RETURN_VAL_IF_FAIL(valid_camera(camera), false);
    G2RETURN_VAL_IF_FAIL(_cameraView != NULL, false);

    return _cameraView[camera].is_use_ptz();
}

bool screen_view::is_camera_visible(short camera) const
{
    G2RETURN_VAL_IF_FAIL(valid_camera(camera), false);
    G2RETURN_VAL_IF_FAIL(_cameraView != NULL, false);

    return _cameraView[camera].is_visible();
}

bool screen_view::is_camera_enable(short camera) const
{
    G2RETURN_VAL_IF_FAIL(valid_camera(camera), false);
    G2RETURN_VAL_IF_FAIL(_cameraView != NULL, false);

    return _cameraView[camera].is_enable();
}

bool screen_view::is_camera_no_video(short camera) const
{
    G2RETURN_VAL_IF_FAIL(valid_camera(camera), false);
    G2RETURN_VAL_IF_FAIL(_cameraView != NULL, false);

    return _cameraView[camera].is_no_video();
}

bool screen_view::is_camera_contains(short camera) const
{
    G2RETURN_VAL_IF_FAIL(valid_camera(camera), false);
    G2RETURN_VAL_IF_FAIL(_cameraView != NULL, false);

    return _cameraView[camera].is_visible();
}

bool screen_view::is_camera_stub(short camera) const
{
    G2RETURN_VAL_IF_FAIL(valid_camera(camera), false);
    G2RETURN_VAL_IF_FAIL(_cameraView != NULL, false);

    return _cameraView[camera].is_stub();
}

short screen_view::camera_mode(short camera) const
{
    return (valid_camera(camera)) ?
            _cameraView[camera].camera_mode() : screen::camera_::UNDEFINED;
}

short screen_view::camera_status(short camera) const
{
    return (valid_camera(camera)) ?
            _cameraView[camera].status() : screen::camera_status_::DISABLE;
}

bool screen_view::camera_rect(short camera, CRect& rect) const
{
    G2RETURN_VAL_IF_FAIL(valid_camera(camera), false);
    G2RETURN_VAL_IF_FAIL(_cameraView != NULL, false);

    rect = _cameraView[camera].rect();
    return true;
}

CRect screen_view::camera_rect(short camera) const
{
    return (valid_camera(camera)) ?
            _cameraView[camera].rect() : CRect();
}

CSize screen_view::camera_image_size(short camera) const
{
    return (valid_camera(camera)) ?
            _cameraView[camera].image_size() : CSize();
}

int screen_view::count_camera_running(void) const
{
    int count = 0;
    for (int i = 0; i < _countcamera; ++i) {
        if (_cameraView[i].is_running()) {
            ++count;
        }
    }
    return count;
}

int screen_view::count_camera_mode(int mode) const
{
    int count = 0;
    for (int i = 0; i < _countcamera; ++i) {
        if (_cameraView[i].is_mode(mode)) {
            ++count;
        }
    }
    return count;
}

camera_view& screen_view::camera_view_ref(short camera) const
{
    return _cameraView[camera];
}

G2SPOT screen_view::camera_spot(short camera) const
{
    return (valid_camera(camera)) ?
            _cameraView[camera].spot() : G2SPOT();
}

unsigned __int64 screen_view::visible_camera(void) const
{
    g2::scoped_criticalsection lock(_lock_exec);

    unsigned __int64 cameras = 0x00ui64;
    for (int i = 0; i < _countcamera; ++i) {
        if (_cameraView[i].is_visible()) {
            g2::set_bit(cameras, i);
        }
    }
    return cameras;
}

unsigned __int64 screen_view::visible_camera_mode(int mode) const
{
    g2::scoped_criticalsection lock(_lock_exec);

    unsigned __int64 cameras = 0x00ui64;
    for (int i = 0; i < _countcamera; ++i) {
        if (_cameraView[i].is_visible() &&
            _cameraView[i].is_enable() &&
            _cameraView[i].is_mode(mode)) {
            g2::set_bit(cameras, i);
        }
    }
    return cameras;
}

unsigned __int64 screen_view::visible_camera_live(void) const
{
    return visible_camera_mode(screen::camera_::WATCH);
}

unsigned __int64 screen_view::visible_camera_play(void) const
{
    return visible_camera_mode(screen::camera_::SEARCH);
}

unsigned __int64 screen_view::running_camera(void) const
{
    g2::scoped_criticalsection lock(_lock_exec);

    unsigned __int64 cameras = 0x00ui64;
    for (int i = 0; i < _countcamera; ++i) {
        if (_cameraView[i].is_running()) {
            g2::set_bit(cameras, i);
        }
    }
    return cameras;
}

void screen_view::erase_image(unsigned __int64& scrcameras)
{
    g2::scoped_criticalsection lock(_lock_exec);

    G2RETURN_IF_FAIL(_painter != NULL);

    for (int i = 0; i < _countcamera; ++i) {
        camera_view& camview = _cameraView[i];
        if (g2::contains_bit(scrcameras, i)) {
            if (camview.is_visible()) {
                camview.set_time(G2TIME());
                camview.fill_rect(_painter);
            }
        }
    }

    execute_command(command_::UPDATE_ALL);
}

void screen_view::update_camera(int camera, bool fill /*= true*/)
{
    execute_command(command_::UPDATE_BY_ONE, static_cast<WPARAM>(camera), static_cast<LPARAM>(fill));
}

void screen_view::update_camera(unsigned __int64 cameras, bool fill /*= true*/)
{
    if (cameras == 0x00ui64) return;

    g2::scoped_criticalsection lock(_lock_exec);

    for (int i = 0; i < _countcamera; ++i) {
        if (g2::contains_bit(cameras, i)) {
            update_camera(i, fill);
        }
    }
}

void screen_view::update_camera_all(void)
{
    execute_command(command_::UPDATE_ALL);
}

void screen_view::update_screen(void)
{
    execute_command(command_::UPDATE_SCREEN);
}

void screen_view::present(unsigned int elpase /*= 0*/)
{
    if (elpase > 0) {

    }
    else {
        g2::scoped_criticalsection lock(_lock_exec);

        if (_painter != NULL) {
            CRect rect;
            _parent->GetClientRect(rect);
            _painter->present(rect);
        }
    }
}

void screen_view::set_camera_ratio(short camera, screen::img_ratio_::FACTOR ratiofact)
{
    if (client::valid_camera(camera) &&
        camera_ratio(camera) != ratiofact) {
        execute_command(command_::SET_RATIO_FACT, static_cast<WPARAM>(camera), static_cast<LPARAM>(ratiofact));
    }
}

void screen_view::set_camera_ratio(screen::img_ratio_::FACTOR ratiofact)
{
    if (_factImageRatio == ratiofact) return;

    _factImageRatio = ratiofact;

    if (_cameraView) {
        for (int i = 0; i < _countcamera; ++i) {
            set_camera_ratio(i, ratiofact);
        }
    }
}

screen::img_ratio_::FACTOR screen_view::camera_ratio(short camera) const
{
    return (valid_camera(camera)) ?
            _cameraView[camera].image_ratio() : screen::img_ratio_::CONSTANT;
}

//////////////////////////////////////////////////////////////////////////

IMPLEMENT_DYNAMIC(screen_view, CWnd)

void screen_view::DoDataExchange(CDataExchange* pDX)
{
    CWnd::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(screen_view, CWnd)
    ON_WM_CREATE()
    ON_WM_DESTROY()
    ON_WM_LBUTTONDOWN()
    ON_WM_LBUTTONUP()
    ON_WM_LBUTTONDBLCLK()
    ON_WM_RBUTTONDOWN()
    ON_WM_RBUTTONUP()
    ON_WM_MBUTTONDOWN()
    ON_WM_MBUTTONUP()
    ON_WM_MOUSEMOVE()
    ON_WM_MOUSEWHEEL()
    ON_WM_SETCURSOR()
    ON_WM_ERASEBKGND()
    ON_WM_MOVE()
    ON_WM_MOVING()
    ON_WM_SIZE()
    ON_WM_SIZING()
    ON_WM_SHOWWINDOW()
    ON_WM_KEYDOWN()
    ON_WM_KEYUP()
    ON_WM_SETFOCUS()
    ON_WM_WINDOWPOSCHANGED()

    ON_MESSAGE(UM_POST_SETFOCUS, on_post_set_focus)
    ON_MESSAGE(UM_POST_FIRE_CAMERA_CHANGED, on_post_fire_camera_changed)
END_MESSAGE_MAP()

//////////////////////////////////////////////////////////////////////////

int screen_view::OnCreate(LPCREATESTRUCT lpCreateStruct)
{
    if (CWnd::OnCreate(lpCreateStruct) == -1) {
        return -1;
    }

    /////////////////////////////////////////////

    _textIn._queue = new text_in_queue[_countcamera + 1];

    create_painter(_rctScreen.Size());

    _totalcamera = _countcamera + _countcamera; /* dummy for base page layout */
    _rangeStub._begin = _countcamera;
    _rangeStub._end   = _totalcamera;

    if (_cameraView = new client::camera_view[_totalcamera]) {
        int i;
        for (i = 0; i < _countcamera; ++i) {
            camera_view& camview = _cameraView[i];
            camview.initialize(i, this);
            camview.set_zoom_ratio(_factImageRatio);
        }

        for (i = _rangeStub._begin; i < _rangeStub._end; ++i) {
            _cameraView[i].initialize(i, this);
            _cameraView[i].set_stub_camera(true);
        }
    }

    update_screen_rect(_currLayout);

    return 0;
}

void screen_view::OnDestroy()
{
    CWnd::OnDestroy();

    unregister_drag_drop();
}

void screen_view::OnLButtonDown(UINT nFlags, CPoint point)
{
    CWnd::OnLButtonDown(nFlags, point);

    on_mouse_button_down_impl(nFlags, point);

    if (_listener) {
        _listener->on_screen_lbutton_down(nFlags, point);
    }
}

void screen_view::OnLButtonUp(UINT nFlags, CPoint point)
{
    CWnd::OnLButtonUp(nFlags, point);

    if (_listener) {
        _listener->on_screen_lbutton_up(nFlags, point);
    }
}

void screen_view::OnLButtonDblClk(UINT nFlags, CPoint point)
{
    CWnd::OnLButtonDblClk(nFlags, point);

    short camera = find_camera_by_point(point);

    if (valid_camera(camera)) {
        execute_command(command_::DBL_CLICK);
    }

    if (_listener) {
        _listener->on_screen_lbutton_dblclk(nFlags, point);
    }
}

void screen_view::OnRButtonDown(UINT nFlags, CPoint point)
{
    CWnd::OnRButtonDown(nFlags, point);

    on_mouse_button_down_impl(nFlags, point);

    if (_listener) {
        _listener->on_screen_rbutton_down(nFlags, point);
    }
}

void screen_view::OnRButtonUp(UINT nFlags, CPoint point)
{
    CWnd::OnRButtonUp(nFlags, point);

    int camera = find_camera_by_point(point);
    if (valid_camera(camera)) {
        if (_listener) {
            _listener->on_screen_rbutton_up(nFlags, point);
        }
    }
}

void screen_view::OnMButtonDown(UINT nFlags, CPoint point)
{
    CWnd::OnMButtonDown(nFlags, point);

    on_mouse_button_down_impl(nFlags, point);

    if (_listener) {
        _listener->on_screen_mbutton_down(nFlags, point);
    }
}

void screen_view::OnMButtonUp(UINT nFlags, CPoint point)
{
    CWnd::OnMButtonUp(nFlags, point);

    if (_listener) {
        _listener->on_screen_mbutton_up(nFlags, point);
    }
}

void screen_view::OnMouseMove(UINT nFlags, CPoint point)
{
    CWnd::OnMouseMove(nFlags, point);

    if (_listener) {
        _listener->on_screen_mouse_move(nFlags, point);
    }
}

BOOL screen_view::OnMouseWheel(UINT nFlags, short zDelta, CPoint pt)
{
    if (_listener) {
        _listener->on_screen_mouse_wheel(nFlags, zDelta, pt);
    }

    return CWnd::OnMouseWheel(nFlags, zDelta, pt);
}

BOOL screen_view::OnSetCursor(CWnd* pWnd, UINT nHitTest, UINT message)
{
    bool processed = false;
    if (_listener) {
        const int selcamera = _selcamera;
        processed = _listener->on_screen_set_cursor(
                                        pWnd, nHitTest, message,
                                        selcamera,
                                        camera_rect(selcamera));
    }
    return (processed) ? TRUE : CWnd::OnSetCursor(pWnd, nHitTest, message);
}

BOOL screen_view::OnEraseBkgnd(CDC* pDC)
{
    CRect rcclip, rect;
    pDC->GetClipBox(&rcclip);
    GetClientRect(&rect);

    g2::scoped_criticalsection lock(_lock_exec);

    bool succeeded = false;
    if (_painter != NULL) {
        succeeded = _painter->present(rcclip.IsRectEmpty() ? rect : rcclip);
    }

    lock.free();

    if (succeeded != true) {
        pDC->FillSolidRect(rcclip, RGB(0, 0, 0));
    }

    return TRUE;
}

void screen_view::OnMoving(UINT nSide, LPRECT lpRect)
{
    CWnd::OnMoving(nSide, lpRect);
}

void screen_view::OnMove(int x, int y)
{
    CWnd::OnMove(x, y);
}

void screen_view::OnSize(UINT nType, int cx, int cy)
{
    CWnd::OnSize(nType, cx, cy);

    CRect rect;
    GetClientRect(rect);

    g2::scoped_criticalsection lock(_lock_exec);

    if (_painter != NULL) {
        if (_painter->size_port() != rect.Size()) {
            create_painter(rect.Size());
        }
    }

    update_screen_rect(rect);

    lock.free();

    _rctScreen = rect;
}

void screen_view::OnSizing(UINT nSide, LPRECT lpRect)
{
    CWnd::OnSizing(nSide, lpRect);
}

void screen_view::OnShowWindow(BOOL bShow, UINT nStatus)
{
    CWnd::OnShowWindow(bShow, nStatus);
}

void screen_view::OnKeyDown(UINT nChar, UINT nRepCnt, UINT nFlags)
{
    if (_listener) {
        _listener->on_screen_key_down(nChar, nRepCnt, nFlags);
    }
}

void screen_view::OnKeyUp(UINT nChar, UINT nRepCnt, UINT nFlags)
{
    if (_listener) {
        _listener->on_screen_key_up(nChar, nRepCnt, nFlags);
    }
}

void screen_view::OnSetFocus(CWnd* pOldWnd)
{
    CWnd::OnSetFocus(pOldWnd);

    if (_listener) {
        _listener->on_screen_set_foucs(pOldWnd);
    }
}

void screen_view::OnWindowPosChanged(WINDOWPOS* lpwndpos)
{
    CWnd::OnWindowPosChanged(lpwndpos);
}

//////////////////////////////////////////////////////////////////////////

LRESULT screen_view::on_post_set_focus(WPARAM wParam, LPARAM lParam)
{
    SetFocus();
    return 0L;
}

LRESULT screen_view::on_post_fire_camera_changed(WPARAM wParam, LPARAM lParam)
{
    int camera = static_cast<int>(wParam);

    if (_listener) {
        _listener->on_screen_camera_changed(camera);
    }

    return 0L;
}

//////////////////////////////////////////////////////////////////////////

bool screen_view::OnDrop(FORMATETC* format,
                         STGMEDIUM* medium,
                         DWORD* effect,
                         POINTL point,
                         LONG_PTR userData)
{
    bool success = false;

    if (_dropParent) {
        drop_info_param di;
        di._from = drop_info_param::from_screen;
        di._format = format;
        di._medium = medium;
        di._effect = effect;
        di._point = point;
        di._userData = userData;

        if (_invokeDropMessage != _UI32_MAX) {
            _dropParent->SendMessage(_invokeDropMessage, NULL, LPARAM(&di));
        }

        set_focus_post();

        success = true;
    }
    else {
        assert(!"is not initiation DragDrop object");
    }

    return success;
}

bool screen_view::OnDragEnter(FORMATETC* format,
                              STGMEDIUM* medium,
                              DWORD* effect,
                              POINTL point,
                              LONG_PTR userData)
{
    bool enable = false;

    if (_dropParent) {
        drop_info_param di;
        di._from = drop_info_param::from_screen;
        di._format = format;
        di._medium = medium;
        di._effect = effect;
        di._point = point;
        di._userData = userData;

        if (_secureDropMessage != _UI32_MAX) {
            _dropParent->SendMessage(_secureDropMessage, NULL, LPARAM(&di));
        }

        const drop_info_param::secure_info& si = di._secured;
        _dropTarget->set_secure_info(si);

        enable = si._enableDrop;
    }
    else {
        assert(!"is not initiation DragDrop object");
    }

    return enable;
}

bool screen_view::OnDragOver(const POINTL& point)
{
    return true;
}

bool screen_view::OnDragLeave(void)
{
    return true;
}

//////////////////////////////////////////////////////////////////////////

bool screen_view::create_painter(const CSize& sizePort)
{
    G2RETURN_VAL_IF_FAIL(GetSafeHwnd() != NULL, false);

    bool success = false;

    if (_painter) {
        _painter->finalize();
        SAFE_DELETE(_painter);
    }

    _painter = new painter(GetSafeHwnd(), sizePort);
    _painter->initialize();

    return success;
}

void screen_view::set_camera_exe(short camera)
{
    G2RETURN_IF_FAIL(client::valid_camera(camera));

    for (int i = 0; i < _countcamera; ++i) {
        camera_view& camview = _cameraView[i];
        if (camview.is_selected()) {
            camview.set_selected(false);
            break;
        }
    }

    _cameraView[camera].set_selected(true);
    _selcamera = camera;
}

void screen_view::set_layout_exe(int layout)
{
    update_screen_rect(layout);
    _currCameraLT = _selcamera;
}

void screen_view::update_screen_rect(int layout)
{
    short temp, cameraLT = _selcamera;
    if (is_layout_base_page()) {
        reckon_camera_layout(screen::layout_reckon_::CHANGE, layout, cameraLT, cameraLT, temp);
    }

    _currLayout = _fmtScreen.reckon_layout(layout, cameraLT, _rctScreen);

    std::for_each(&_cameraView[_rangeStub._begin], &_cameraView[_rangeStub._end],
                  std::bind2nd(std::mem_fun_ref(&client::camera_view::set_visible), false));

    const screen::formatter::container_t& rctvector = _fmtScreen.container();

    if (_rctScreen.IsRectEmpty()) {
        for (int i = 0, count = static_cast<int>(rctvector.size()); i < count; ++i) {
            const screen::formatter::element_t& element = rctvector[i];
            camera_view& camview = _cameraView[i];
            if (element.is_visible()) {
                camview.set_visible(true);
            }
            else {
                camview.set_visible(false);
            }
        }
        return;
    }

    for (int i = 0, count = static_cast<int>(rctvector.size()); i < count; ++i) {
        const screen::formatter::element_t& element = rctvector[i];
        camera_view& camview = _cameraView[i];
        if (element.is_visible()) {
            camview.set_visible(true);
            camview.set_rect(element.rect());
            camview.fill_rect(_painter);
            camview.set_draw(true);
        }
        else {
            camview.set_visible(false);
        }

        if (is_layout_base_page()) {
            if (i <cameraLT &&
                element.is_visible()) {
                camview.set_visible(false);
                camera_view& camstub = _cameraView[i + _rangeStub._begin];
                camstub.set_visible(true);
                camstub.set_rect(element.rect());
                camstub.fill_rect(_painter);
                camstub.set_draw(true);
            }
        }
    }
}

void screen_view::update_screen_rect(const CRect& rect)
{
    _rctScreen = rect;

    execute_command(command_::UPDATE_RECT, true);
}

void screen_view::update_camera_osd(void)
{
    g2::scoped_criticalsection critical(_lock_exec);

    for (short i = 0; i < _countcamera + 1; ++i) {
        camera_view& camview = _cameraView[i];
        if (camview.is_visible()) {
            camview.clear_osd(_painter);
            camview.set_draw(true);
        }
    }
}

void screen_view::update_camera_by_one(int camera, bool fill /*= true*/)
{
    if (valid_camera(camera) != true) return;

    camera_view& camview = _cameraView[camera];

    if (camview.is_visible()) {
        if (fill) {
            camview.fill_rect(_painter);
        }
        camview.set_draw(true);
    }
}

void screen_view::reckon_camera_layout(screen::layout_reckon_::MODE mode, int layout, short selcamera, short& rpreselect, short& rcamera)
{
    const int counts = _fmtScreen.count_per_layout(layout);
    short leftTop = 0;

    if (counts == 0) {
        assert("!failed to calculate count of camera per layout");
        return;
    }

    if (is_layout_base_page()) {
        const int pageMax = (_countcamera % counts) != 0 ?
                             _countcamera / counts :
        _countcamera / counts - 1;
        short page = (selcamera / counts);
        short posTemp = 0;

        if ((counts * page) >= _countcamera) {
            page = 0;
        }

        switch (mode) {
        case screen::layout_reckon_::CHANGE:
            leftTop = (page * counts);
            break;

        case screen::layout_reckon_::PREV:
            if (--page < 0) page = pageMax;
            leftTop   = (page * counts);
            selcamera = (selcamera % counts) + (page * counts);
            break;

        case screen::layout_reckon_::NEXT:
            if (++page > pageMax) page = 0;
            leftTop   = (page * counts);
            selcamera = (selcamera % counts) + (page * counts);
            break;
        }

        if (selcamera >= _countcamera) {
            selcamera = page * counts;
        }
    }
    else {
        switch (mode) {
        case screen::layout_reckon_::CHANGE:
            break;

        case screen::layout_reckon_::PREV:
            if ((selcamera -= counts) < 0) {
                selcamera += _countcamera;
            }
            if ((leftTop = _currCameraLT - counts) < 0) {
                leftTop += _countcamera;
            }
            break;

        case screen::layout_reckon_::NEXT:
            if ((selcamera += counts) >= _countcamera) {
                selcamera -= _countcamera;
            }
            if ((leftTop = _currCameraLT + counts) >= _countcamera) {
                leftTop -= _countcamera;
            }
            break;
        }
    }

    rpreselect = leftTop;
    rcamera = selcamera;
}

int screen_view::last_display_spot(unsigned __int64& cameras, G2SPOT& spot)
{
    g2_spot_make_invalid(&spot);
    int camera = client::invalid_::CAMERA_NUMBER;

    for (int i = 0; i < _countcamera; ++i) {
        if (g2::contains_bit(cameras, i) != true) continue;

        client::camera_view& camview = _cameraView[i];

        if (g2_spot_is_valid(&camview.spot()) &&
           (g2_spot_is_valid(&spot) != true ||
            g2_spot_compare(&camview.spot(), &spot) < 0)) {
            spot = camview.spot();
            camera = i;
        }
    }
    return camera;
}

G2SPOT screen_view::last_display_spot(short camera)
{
    G2SPOT spot = { 0 };
    g2_spot_make_invalid(&spot);
    if (valid_camera(camera)) {
        spot = _cameraView[camera].spot();
    }
    
    return spot;
}

void screen_view::put_text_in(short hostId, short camera, const G2TEXT_IN_ELEMENT& element, bool tohead /*= false*/)
{
    _textIn._queue[camera].put(element, tohead);
}

void screen_view::delete_text_in(const unsigned __int64& cameras)
{
    for (int i = 0; i < _countcamera; ++i) {
        if (g2::contains_bit(cameras, i)) {
            _textIn._queue[i].clear();
        }
    }
}

void screen_view::display_text_in(short camera, const G2SPOT& spot, bool rollback /*= false*/)
{
    G2RETURN_IF_FAIL(valid_camera(camera));
    G2RETURN_IF_FAIL(is_camera_stub(camera) != true);

    text_in_queue& queue = _textIn._queue[camera];

    if (queue.empty()) return;

    bool enable = (_currLayout == screen_formatter_::LAYOUT_1x1 && _selcamera == camera) ? true : false;

    int from = queue.from();
    if (from == G2TEXT_IN_ELEMENT::FROM_LIVE) {
        if (enable) {
            CTimeSpan gab = CTime::GetCurrentTime() - CTime(g2_time_to_time32_t(&queue.scope_last()));
            int seconds = static_cast<int>(gab.GetTotalSeconds());
            if (seconds < 10 &&
                seconds >= 0) {
                _cameraView[camera].draw_text_in_live(_painter, &queue);
            }
            else {
                // because over 10 seconds, delete text-in data.
                queue.clear();
            }
        }
        else {
            queue.clear();
        }
    }
    else if (from == G2TEXT_IN_ELEMENT::FROM_PLAY) {
        if (enable) {
            const G2SPOT& rspot = (rollback) ? last_display_spot(camera) : spot;
            if (g2_spot_is_valid(&rspot)) {
                _cameraView[camera].draw_text_in_play(_painter, &queue, rspot);
                _textIn._spotLast = rspot;
            }
        }
    }
}

//////////////////////////////////////////////////////////////////////////

void screen_view::on_mouse_button_down_impl(UINT nFlags, const CPoint& point)
{
    const int selcamera = find_camera_by_point(point);

    if (valid_camera(selcamera)) {
        set_camera(selcamera);
    }

    SetFocus();
}

//////////////////////////////////////////////////////////////////////////

void screen_view::register_drag_drop(void)
{
    assert(!_dropTarget);

    _dropTarget = new screen_drop_target(GetSafeHwnd(), this);

    if (SUCCEEDED(::RegisterDragDrop(GetSafeHwnd(), _dropTarget))) {
        FORMATETC fmtETC = { 0 };
        fmtETC.cfFormat = CF_TEXT;
        fmtETC.dwAspect = DVASPECT_CONTENT;
        fmtETC.lindex   = -1;
        fmtETC.tymed    = TYMED_HGLOBAL;
        _dropTarget->append_support_format(fmtETC);
        fmtETC.cfFormat = CF_HDROP;
        _dropTarget->append_support_format(fmtETC);
    }
    else {
        SAFE_DELETE(_dropTarget);
        assert(!"failed to create DragDrop object");
    }
}

void screen_view::unregister_drag_drop(void)
{
    HRESULT result = (_dropTarget) ? ::RevokeDragDrop(GetSafeHwnd()) : DRAGDROP_E_NOTREGISTERED;
    switch (result) {
        case S_OK:
            // registration as a target window was revoked successfully
            _dropTarget = NULL;
            break;
        case DRAGDROP_E_INVALIDHWND:
            // invalid handle returned in the hwnd parameter
        case DRAGDROP_E_NOTREGISTERED:
            // an attempt was made to revoke a drop target that has not been registered
        default:
            SAFE_DELETE(_dropTarget);
            break;
    }
}

//////////////////////////////////////////////////////////////////////////

LRESULT screen_view::execute_command(command_::ID command, WPARAM wParam /*= 0*/, LPARAM lParam /*= 0L*/)
{
    LRESULT   lreturn = 0L;
    BOOL      proceed = TRUE;
    bool screenUpdate = false;

    switch (command) {
        case command_::SET_CAMERA:
            lreturn = exe_set_camera(wParam, lParam, proceed);
            break;
        case command_::SET_LAYOUT:
            lreturn = exe_set_layout(wParam, lParam, proceed);
            screenUpdate = true;
            break;
        case command_::PREV_LAYOUT:
            lreturn = exe_prev_layout(wParam, lParam, proceed);
            screenUpdate = true;
            break;
        case command_::NEXT_LAYOUT:
            lreturn = exe_next_layout(wParam, lParam, proceed);
            screenUpdate = true;
            break;
        case command_::DBL_CLICK:
            lreturn = exe_dbl_click(wParam, lParam, proceed);
            screenUpdate = true;
            break;
        case command_::UPDATE_RECT:
            lreturn = exe_update_rect(wParam, lParam, proceed);
            break;
        case command_::UPDATE_ALL:
            lreturn = exe_update_all(wParam, lParam, proceed);
            break;
        case command_::UPDATE_BY_ONE:
            lreturn = exe_update_camera_by_one(wParam, lParam, proceed);
            break;
        case command_::UPDATE_SCREEN:
            lreturn = exe_update_screen(wParam, lParam, proceed);
            break;
        case command_::SET_CAMERA_TITLE:
            lreturn = exe_set_camera_title(wParam, lParam, proceed);
            break;
        case command_::SET_CAMERA_TITLE_EXT:
            lreturn = exe_set_camera_title_ext(wParam, lParam, proceed);
            break;
        case command_::SET_CAMERA_STATUS:
            lreturn = exe_set_camera_status(wParam, lParam, proceed);
            break;
        case command_::SET_RATIO_FACT:
            lreturn = exe_set_ratio_factor(wParam, lParam, proceed);
            break;
        case command_::DUMMY:
            break;
        default:
            assert(!"undefined execute command");
            break;
    }

    if (_lockUpdate) return lreturn;

    g2::scoped_criticalsection lock(_lock_exec);

    if (proceed) {
        int i = 0, count = 0;
        if (screenUpdate) {
            for (i = 0; i < _totalcamera; ++i) {
                camera_view& camview = _cameraView[i];
                if (camview.is_visible() &&
                    camview.is_draw()) {
                    camview.draw_border(_painter);
                    camview.draw_osd(_painter);
                    display_text_in(i, _textIn._spotLast, true);
                    camview.render(_painter);
                    ++count;
                }
            }
            if (_painter != NULL &&
                count > 0) {
                _painter->present();
            }
        }
        else {
            for (i = 0; i < _totalcamera; ++i) {
                camera_view& camview = _cameraView[i];
                if (camview.is_visible() &&
                    camview.is_draw()) {
                    camview.draw_border(_painter);
                    camview.draw_osd(_painter);
                    display_text_in(i, _textIn._spotLast, true);
                    camview.render(_painter);
                    camview.present(_painter);
                }
            }
        }
    }

    return lreturn;
}

LRESULT screen_view::exe_set_camera(WPARAM wParam, LPARAM lParam, BOOL& proceed)
{
    g2::scoped_criticalsection lock(_lock_exec);

    proceed = TRUE;

    short camera = (short)(wParam);
    bool  redraw = (bool)(lParam);
    set_camera_exe(camera);

    lock.free();

    if (!redraw) proceed = FALSE;

    if (_listener) {
        _listener->on_screen_camera_changed(_selcamera);
    }

    return 0L;
}

LRESULT screen_view::exe_set_layout(WPARAM wParam, LPARAM lParam, BOOL& proceed)
{
    g2::scoped_criticalsection lock(_lock_exec);

    proceed = TRUE;

    const int layout = static_cast<int>(wParam);
    if (layout != _currLayout) {
        _prevLayout = _currLayout;
    }

    set_layout_exe(layout);
    set_camera_exe(_selcamera);

    lock.free();

    if (_listener) {
        _listener->on_screen_layout_changed(layout);
    }

    return 0L;
}

LRESULT screen_view::exe_prev_layout(WPARAM wParam, LPARAM lParam, BOOL& proceed)
{
    g2::scoped_criticalsection lock(_lock_exec);

    proceed = TRUE;

    short camera    = _selcamera;
    short preselect = 0;
    reckon_camera_layout(screen::layout_reckon_::PREV, _currLayout, camera, preselect, camera);

    set_camera_exe(preselect);
    set_layout_exe(_currLayout);
    set_camera_exe(camera);

    lock.free();

    if (_listener) {
        _listener->on_screen_camera_changed(camera);
        _listener->on_screen_layout_changed(_currLayout, screen::layout_change_::PREV);
    }

    return 0L;
}

LRESULT screen_view::exe_next_layout(WPARAM wParam, LPARAM lParam, BOOL& proceed)
{
    g2::scoped_criticalsection lock(_lock_exec);

    proceed = TRUE;

    short camera = _selcamera;
    short preselect = 0;
    reckon_camera_layout(screen::layout_reckon_::NEXT, _currLayout, camera, preselect, camera);

    set_camera_exe(preselect);
    set_layout_exe(_currLayout);
    set_camera_exe(camera);

    lock.free();

    if (_listener) {
        _listener->on_screen_camera_changed(camera);
        _listener->on_screen_layout_changed(_currLayout, screen::layout_change_::NEXT);
    }

    return 0L;
}

LRESULT screen_view::exe_dbl_click(WPARAM wParam, LPARAM lParam, BOOL& proceed)
{
    g2::scoped_criticalsection lock(_lock_exec);

    proceed = TRUE;

    int layout;
    short camera;

    if (_currLayout != screen_formatter_::LAYOUT_1x1) {
        _prevLayout = _currLayout;
        _prevCameraLT = _currCameraLT;
        set_layout_exe(screen_formatter_::LAYOUT_1x1);
        layout = screen_formatter_::LAYOUT_1x1;
    }
    else {
        camera = _selcamera;
        _prevLayout = _countcamera < _fmtScreen.count_per_layout(_prevLayout) ?
                      _fmtScreen.secure_max_layout(_prevLayout) : _prevLayout;
        set_camera_exe(_prevCameraLT);
        set_layout_exe(_prevLayout);
        set_camera_exe(camera);
        layout = _prevLayout;
    }

    lock.free();

    if (_listener) {
        _listener->on_screen_layout_changed(layout);
    }

    return 0L;
}

LRESULT screen_view::exe_update_rect(WPARAM wParam, LPARAM lParam, BOOL& proceed)
{
    g2::scoped_criticalsection lock(_lock_exec);

    short camera = _selcamera;

    set_camera_exe(_currCameraLT);
    update_screen_rect(_currLayout);
    set_camera_exe(camera);

    exe_update_all(0, 0L, proceed);

    return 0L;
}

LRESULT screen_view::exe_update_all(WPARAM wParam, LPARAM lParam, BOOL& proceed)
{
    g2::scoped_criticalsection lock(_lock_exec);

    // after each cameras work to rendering,
    // screen update at once.
    proceed = FALSE;
    for (int i = 0; i < _totalcamera; ++i) {
        camera_view& camview = _cameraView[i];
        if (camview.is_visible()) {
            camview.set_draw(false);
            camview.draw_border(_painter);
            camview.draw_osd(_painter);
            display_text_in(i, _textIn._spotLast, true);
            camview.render(_painter);
        }
    }

    if (_painter != NULL) {
        _painter->present();
    }

    return 0L;
}

LRESULT screen_view::exe_update_camera_by_one(WPARAM wParam, LPARAM lParam, BOOL& proceed)
{
    g2::scoped_criticalsection lock(_lock_exec);

    proceed = TRUE;

    const short camera = static_cast<int>(wParam);
    const bool filled = static_cast<bool>(lParam);

    update_camera_by_one(camera, filled);

    return 0L;
}

LRESULT screen_view::exe_update_screen(WPARAM wParam, LPARAM lParam, BOOL& proceed)
{
    g2::scoped_criticalsection lock(_lock_exec);

    exe_update_all(0, 0, proceed);

    return 0L;
}

LRESULT screen_view::exe_set_camera_title(WPARAM wParam, LPARAM lParam, BOOL& proceed)
{
    g2::scoped_criticalsection lock(_lock_exec);

    proceed = TRUE;

    const short camera = LOWORD(wParam);
    const bool redraw = HIWORD(wParam);
    LPCTSTR title = reinterpret_cast<LPCTSTR>(lParam);

    ///////////////////////////////////

    G2RETURN_VAL_IF_ASSERT(valid_camera(camera), 1L);

    _cameraView[camera].set_title(title);

    if (redraw) {
        update_camera_by_one(camera);
    }
    else {
        proceed = FALSE;
    }

    return 0L;
}

LRESULT screen_view::exe_set_camera_title_ext(WPARAM wParam, LPARAM lParam, BOOL& proceed)
{
    g2::scoped_criticalsection lock(_lock_exec);

    proceed = TRUE;

    const short camera = LOWORD(wParam);
    const bool redraw = HIWORD(wParam);
    LPCTSTR title = reinterpret_cast<LPCTSTR>(lParam);

    ///////////////////////////////////

    G2RETURN_VAL_IF_ASSERT(valid_camera(camera), 1L);

    _cameraView[camera].set_title_ext(title);

    if (redraw) {
        update_camera_by_one(camera);
    }
    else {
        proceed = FALSE;
    }

    return 0L;
}

LRESULT screen_view::exe_set_camera_status(WPARAM wParam, LPARAM lParam, BOOL& proceed)
{
    g2::scoped_criticalsection lock(_lock_exec);

    proceed = TRUE;

    short camera = LOWORD(wParam);
    short status = HIWORD(wParam);
    bool  redraw = static_cast<bool>(lParam);

    ///////////////////////////////////

    G2RETURN_VAL_IF_ASSERT(valid_camera(camera), 1L);

    camera_view& camview = _cameraView[camera];

    short prevStatus = camview.status();

    camview.set_status(status);

    if (redraw) {
        if (prevStatus != status) {
            if (screen::camera_status_::RECONNECTING == status) {
                update_camera_by_one(camera, false);
            }
            else {
                update_camera_by_one(camera);
            }
        }
    }
    else {
        proceed = FALSE;
    }

    return 0L;
}

LRESULT screen_view::exe_set_ratio_factor(WPARAM wParam, LPARAM lParam, BOOL& proceed)
{
    g2::scoped_criticalsection lock(_lock_exec);

    proceed = TRUE;

    const short camera = static_cast<short>(wParam);
    screen::img_ratio_::FACTOR ratiofact = static_cast<screen::img_ratio_::FACTOR>(lParam);

    camera_view& camview = _cameraView[camera];

    camview.set_zoom_ratio(ratiofact);

    if (camview.is_visible()) {
        camview.fill_rect(_painter);
        camview.set_draw(true);
    }

    return 0L;
}
