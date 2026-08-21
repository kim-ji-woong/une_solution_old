// screen_actuator.cpp : implementation file
//

#include "stdafx.h"
#include "screen_actuator.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

#define member_function_ptr(func) &screen_actuator::##func

//////////////////////////////////////////////////////////////////////////

screen_actuator::screen_actuator(void)
    : _cleanup(true)
{
    initialize();
}

screen_actuator::~screen_actuator(void)
{
    cleanup();
}

//////////////////////////////////////////////////////////////////////////

bool screen_actuator::create(CWnd* parent, const CRect& rect, unsigned int id,
                             int countcamera /*= 36*/,
                             int countchannel /*= 16*/,
                             int layout /*= screen_formatter_::LAYOUT_6x6*/)
{
    _cleanup = false;
    _countcamera  = (countcamera < 1) ? 1 : countcamera;
    _countchannel = (countchannel < 1) ? 1 : countchannel;

    _screen.set_listener(this);

    bool success = _screen.create(parent,
                                  rect,
                                  id,
                                  _countcamera,
                                  screen_formatter_::LAYOUT(layout));

    if (success) {

    }
    else {
        assert(!"failed to create screen view");
        return false;
    }

    _bufferManager.create(_countchannel, _countcamera);
    _decoder.startup(countcamera);

    prepare_looper(THREAD_PRIORITY_NORMAL);

    return success;
}

bool screen_actuator::cleanup(void)
{
    _cleanup = true;

    release_looper();

    _bufferManager.cleanup();
    _screen.cleanup();
    _decoder.cleanup();

    return true;
}

void screen_actuator::set_listener(screen_actuator_listener* listener)
{
    _listener = listener;
}

void screen_actuator::set_priority(int priority /*= THREAD_PRIORITY_NORMAL*/)
{
    _looper.set_priority(priority);
}

void screen_actuator::set_drag_drop(CWnd* dropParent, unsigned int invokeMessageId, unsigned int secureMessageId)
{
    _screen.set_drag_drop(dropParent, invokeMessageId, secureMessageId);
}

void screen_actuator::set_camera_mode(short camera, screen::camera_::MODE cameraMode)
{
    _screen.set_camera_mode(camera, cameraMode);
}

void screen_actuator::set_camera_mode(unsigned __int64 rcameras, screen::camera_::MODE cameraMode)
{
    _screen.set_camera_mode(rcameras, cameraMode);
}

void screen_actuator::set_camera_status(short camera, screen::camera_status_::TYPE status, bool update /*= true*/)
{
    _screen.set_camera_status(camera, status, update);
}

void screen_actuator::set_camera_status(unsigned __int64 cameras, screen::camera_status_::TYPE status, bool update /*= true*/)
{
    _screen.set_camera_status(cameras, status, update);
}

void screen_actuator::set_use_camera_ptz(short camera, char use /*= G2LIVE_CAMERA_STATUS::PTZ_NONE*/)
{
    _screen.set_use_camera_ptz(camera, use);
}

void screen_actuator::set_camera_visible(short camera, bool visible /*= true*/)
{
    _screen.set_camera_visible(camera, visible);
}

void screen_actuator::set_camera_use_dual_audio(unsigned __int64& cameras, bool use, bool update /*= true*/)
{
    _screen.set_camera_use_dual_audio(cameras, use, update);
}

void screen_actuator::set_camera_use_sound_capturing(unsigned __int64& cameras, bool use, bool update /*= true*/)
{
    _screen.set_camera_use_sound_capturing(cameras, use, update);
}

void screen_actuator::set_camera_ratio(short camera, screen::img_ratio_::FACTOR factor)
{
    _screen.set_camera_ratio(camera, factor);
}

void screen_actuator::set_camera_ratio(screen::img_ratio_::FACTOR factor)
{
    _screen.set_camera_ratio(factor);
}

bool screen_actuator::is_camera_status(short camera, screen::camera_status_::TYPE status) const
{
    return _screen.is_camera_status(camera, status);
}

char screen_actuator::is_use_camera_ptz(short camera)const
{
    return _screen.is_use_camera_ptz(camera);
}

//////////////////////////////////////////////////////////////////////////

void screen_actuator::set_deinterlace_filter(short camera, G2DECODER_VIDEO_DEINTERLACE::FILTER filter)
{
    _decoder.set_deinterlacing_filter(camera, filter);
}

void screen_actuator::set_deinterlace_filter_all(G2DECODER_VIDEO_DEINTERLACE::FILTER filter)
{
    _decoder.set_deinterlacing_filter(filter);
}

//////////////////////////////////////////////////////////////////////////

void screen_actuator::initialize(void)
{
    _listener = NULL;
    _countcamera = client::MAX_SCREEN_CAMERA_COUNT;
    _countchannel = client::MAX_CONNECTIVE_CHANNEL;
    _ignoreFrame = false;
    _ignoreDisplay = false;
    _ignoreFrame = false;
    _ignoreDisplay = false;

    _speedPlay = 0;
    _refsleep = 0;

    _bufferExtra.resize(720 * 576 * 2);
}

bool screen_actuator::enable_window(bool enable /*= true*/)
{
    return _screen.enable_window(enable);
}

bool screen_actuator::show(int command /*= SW_SHOWNORMAL*/)
{
    return _screen.show(command);
}

bool screen_actuator::set_window_pos(const CWnd* insertafter, const CRect& rect, unsigned int flags)
{
    return _screen.set_window_pos(insertafter, rect, flags);
}

void screen_actuator::move_window(const CRect& rect, bool repaint /*= true*/)
{
    _screen.move_window(rect, repaint);
}

void screen_actuator::set_focus(void)
{
    if (_screen.GetSafeHwnd()) {
        _screen.set_focus();
    }
}

void screen_actuator::set_focus_post(void)
{
    if (_screen.GetSafeHwnd()) {
        _screen.set_focus_post();
    }
}

void screen_actuator::set_active_window(void)
{
    if (_screen.GetSafeHwnd()) {
        _screen.SetActiveWindow();
    }
}

void screen_actuator::close_decoder(unsigned __int64 cameras)
{
    for (int i = 0; i < client::MAX_SCREEN_CAMERA_COUNT; ++i) {
        if (g2::contains_bit(cameras, i)) {
            _decoder.close(i);
        }
    }
}

//////////////////////////////////////////////////////////////////////////

void screen_actuator::prepare_looper(int priority)
{
    _looper.create(this, &screen_actuator::looper_func, L"looper screen live actuator", priority);
}

void screen_actuator::release_looper(void)
{
    _looper.destroy();
}

void screen_actuator::run(void)
{
    _looper.run(false);
}

void screen_actuator::suspend(void)
{
    _looper.suspend();
}

void screen_actuator::resume(void)
{
    _looper.resume();
}

bool screen_actuator::is_suspended(void) const
{
    return _looper.is_suspended();
}

bool screen_actuator::is_suspending(void) const
{
    return _looper.is_suspending();
}

bool screen_actuator::is_running(void) const
{
    return _looper.is_running();
}

void screen_actuator::actuator_video(void)
{
    if (_videoFrame.valid() != true) {
        _bufferManager.get_frame(_videoFrame);
    }
    if (_videoFrame.valid()) {
        const G2FRAME::FROM from = _videoFrame.from();
        if (from == G2FRAME::FROM_LIVE ||
            from == G2FRAME::FROM_WATCH ||
            from == G2FRAME::FROM_RTP) {
            actuator_live(_videoFrame);
        }
        else if (from == G2FRAME::FROM_PLAY ||
            from == G2FRAME::FROM_BACKUP ||
            from == G2FRAME::FROM_SEARCH ||
            from == G2FRAME::FROM_SEARCH_G2) {
            actuator_play(_videoFrame);
        }
        _videoFrame.reset();
        _looper.yield();
    }
    else {
        if (!(++_refsleep & 0x01ui32)) {
            _looper.sleep(1);
        }
        else {
            _looper.yield();
        }
    }
}

void screen_actuator::actuator_live(const frame_element& element)
{
    video_play(screen::looper_::LIVE, element);
}

void screen_actuator::actuator_play(const frame_element& element)
{
    if (element.is_status(frame_element::NOFRAME_LOADED)) {
        // in case that occur no-frame loaded as there is not inputed frame,
        // so is not called stop method, should stop audio play by compulsion.
        if (_listener) {
            _listener->on_screen_no_image_loaded(element.channel());
        }
        return;
    }

    if (element.camera() >= 0 &&
        _ignoreChannels.find(element.channel()) == _ignoreChannels.end()) {
        video_play(screen::looper_::PLAY, element);
    }
}

void screen_actuator::video_play(screen::looper_::MODE mode, const frame_element& element)
{
    g2::scoped_criticalsection decoder_lock(_lock_decoder);

    const short channel = element.channel();
    unsigned __int64 refcameras = element.ref_cameras();

    if (element.valid() != true) return;
    if (is_prepare_drive(channel)) return;
    if (is_ignore_frame()) return;
    if (refcameras == 0x00ui64) return;

    // video_frame decoding ///////////////////////////////

    const short scrcamera = element.camera();
    const G2FRAME& frame = element.frame();
    CSize resimage(__max(720, element.width()), 
                   __max(576, element.height()));

    G2RETURN_IF_ASSERT(resimage.cx != 0 && resimage.cy != 0);

    // length secured-buffer consider as Mega Pixel,
    // if input frame's size is undefined.
    // ex) HITRON, etc IP-camera (3rd party)
    if (resimage.cx >= 0xFFFF ||
        resimage.cy >= 0xFFFF) {
        resimage.cx = 1920, resimage.cy = 1088;
    }

    size_t size = frame_buffer::calc_len_YV12(resimage.cx, resimage.cy);
    if (_bufferImage.size() < size) {
        _bufferImage.resize(size);
    }

    if (mode == screen::looper_::LIVE) {
        if (g2::contains_bitor(_screen.visible_camera(), refcameras) != true) {
            close_decoder();
            clear_frame_buffer(element.channel(), element.channel_ext());
            return;
        }
    }

    G2DECODER_VIDEO_PARAM param = { 0 };
    param._frame = &frame;
    param._format = G2DECODER_VIDEO_PIX_FORMAT::YV12;
    param._buf_decompress = &_bufferImage[0];
    param._buf_filter = &_bufferExtra[0];
    param._decoder_id = element.camera();

    bool success = _decoder.decompress(param);

    decoder_lock.free(); // important

    ///////////////////////////////////////////////////////

    if (element.is_display() != true ||
        is_ignore_display()) {
        return;
    }

    for (short i = 0; i < _countcamera; ++i) {
        if (g2::contains_bit(refcameras, i) &&
            _screen.is_camera_visible(i)) {
            if (param._result._result == G2DECODER_VIDEO_RESULT::SUCCESS) {
                _screen.display_frame(&_bufferImage[0], (short)param._result._width, (short)param._result._height, i, frame);
            }
        }
    }
}

//////////////////////////////////////////////////////////////////////////

void screen_actuator::put_frame(G2FRAME::FROM from, const G2FRAME& frame, short hostId, short basecamera, unsigned __int64 refcameras)
{
    if (is_ignore_frame()) {
        TRACE(L"@ screen_actuator::ignore host[%d] stream : %d\n", hostId, basecamera);
        return;
    }

    if (basecamera >= 0 &&
        is_running() != true) {
        TRACE(L"@ screen_actuator::put frame failed::actuator is not working - %s\n", frame._title);
        return;
    }

    _bufferManager.put_frame(frame, hostId, basecamera, refcameras);
}

void screen_actuator::clear_frame_buffer(short hostId)
{
    _bufferManager.clear_buffer(hostId);
}

void screen_actuator::clear_frame_buffer(short hostId, int hostcamera)
{
    _bufferManager.clear_buffer(hostId, hostcamera);
}

void screen_actuator::clear_frame_buffer(short hostId, const g2::channels& channelext)
{
    _bufferManager.clear_buffer(hostId, channelext);
}

void screen_actuator::clear_frame_buffer(short hostId, const std::set<int>& channelexts)
{
    _bufferManager.clear_buffer(hostId, channelexts);
}

void screen_actuator::clear_frame_buffer_play(short hostId)
{
    _bufferManager.init(hostId);
}

void screen_actuator::clear_frame_buffer_play(short hostId, int channelext)
{
    _bufferManager.init(hostId, channelext);
}

//////////////////////////////////////////////////////////////////////////

void screen_actuator::setup_buffer(short hostId, frame_buffer::TYPE type, frame_buffer::PLAYTIME time /*= frame_buffer::TIME_MSEC*/)
{
    _bufferManager.setup_buffer(hostId, type, time);

    switch (type) {
        case frame_buffer::TYPE_UNDEFINED:
        case frame_buffer::TYPE_REMOVE:
            /*if (hostId == _bufferManager.get_audio_channel()) {
                stop_search_audio();
            }*/
            break;
    }
}

void screen_actuator::remove_buffer(short hostId)
{
    _bufferManager.remove(hostId);
}

void screen_actuator::remove_buffer(void)
{
    _bufferManager.remove();
}

void screen_actuator::set_prepare_drive(short hostId, bool prepare)
{
    _bufferManager.set_prepare_drive(hostId, prepare);
}

bool screen_actuator::is_prepare_drive(short hostId) const
{
    return _bufferManager.is_prepare_drive(hostId);
}

void screen_actuator::set_search_shutdown(short hostId, unsigned __int64 cameras)
{
    _bufferManager.exit_buffer(hostId);
}

void screen_actuator::set_search_play_speed(short hostId, int speed, bool audioIgnore /*= false*/)
{
    if (audioIgnore ||
        speed != screen::play_speed_::NORMAL) {
        stop_search_audio();
    }

    _speedPlay = speed;
    _bufferManager.set_ignore_audio(audioIgnore);
    _bufferManager.set_play_speed(hostId, speed);
}

void screen_actuator::set_search_end_play(G2FRAME::FROM from, int hostId)
{
    G2FRAME frame = { 0 };
    frame._index._channel = frame_element::NOFRAME_LOADED;
    frame._index._from = from;

    put_frame(from, frame, hostId, frame_element::NOFRAME_LOADED, 0);
}

void screen_actuator::stop_search_enter(short hostId)
{
    _ignoreChannels.insert(hostId);
}

void screen_actuator::stop_search_leave(short hostId)
{
    _bufferManager.clear_buffer(hostId);

    boost::unordered_set<short>::iterator itr = _ignoreChannels.find(hostId);
    if (itr != _ignoreChannels.end()) {
        _ignoreChannels.erase(itr);
    }
}

void screen_actuator::put_text_in(short hostId, short camera, const G2TEXT_IN_ELEMENT& element, bool tohead /*= false*/)
{
    _screen.put_text_in(hostId, camera, element, tohead);
}

void screen_actuator::delete_text_in(const unsigned __int64& cameras)
{
    _screen.delete_text_in(cameras);
}

void screen_actuator::set_camera_title(short camera, const std::wstring& title, bool update /*= true*/)
{
    _screen.set_camera_title(camera, title, update);
}

void screen_actuator::set_camera_title(unsigned __int64& cameras, const std::wstring& title, bool update /*= true*/)
{
    _screen.set_camera_title(cameras, title, update);
}

void screen_actuator::set_camera_title_ext(short camera, const std::wstring& title, bool update /*= true*/)
{
    _screen.set_camera_title_ext(camera, title, update);
}

void screen_actuator::set_camera_title_ext(unsigned __int64& cameras, const std::wstring& title, bool update /*= true*/)
{
    _screen.set_camera_title_ext(cameras, title, update);
}

void screen_actuator::reset_screen(bool mode, bool title /*= true*/, bool update /*= true*/)
{
    _screen.reset_screen(mode, title, update);
}

void screen_actuator::reset_camera(short camera, bool mode, bool title /*= true*/, bool update /*= true*/)
{
    _screen.reset_camera(camera, mode, title, update);
}

void screen_actuator::reset_camera(unsigned __int64 cameras, bool mode, bool title /*= true*/, bool update /*= true*/)
{
    _screen.reset_camera(cameras, mode, title, update);
}

void screen_actuator::set_layout(screen_formatter_::LAYOUT layout)
{
    _screen.set_layout(layout);
}

int screen_actuator::get_last_displayed_spot(unsigned __int64& cameras, G2SPOT& spot)
{
    return _screen.last_display_spot(cameras, spot);
}

//////////////////////////////////////////////////////////////////////////

void screen_actuator::on_screen_camera_changed(short camera)
{
    _bufferManager.on_screen_changed_camera(camera);

    if (_listener) {
        _listener->on_screen_camera_changed(camera);
    }
}

void screen_actuator::on_screen_layout_changed(int layout, int changed /*= client::screen::layout_change_::CHANGE*/)
{
    if (_listener) {
        _listener->on_screen_layout_changed(layout, changed);
    }
}

void screen_actuator::on_screen_image_drew(short camera, const G2SPOT& spot)
{
    if (_listener) {
        _listener->on_screen_image_drew(camera, spot);
    }
}

void screen_actuator::on_screen_lbutton_down(unsigned int flags, const CPoint& point)
{
    if (_listener) {
        _listener->on_screen_lbutton_down(flags, point);
    }
}

void screen_actuator::on_screen_lbutton_up(unsigned int flags, const CPoint& point)
{
    if (_listener) {
        _listener->on_screen_lbutton_up(flags, point);
    }
}

void screen_actuator::on_screen_lbutton_dblclk(unsigned int flags, const CPoint& point)
{
    if (_listener) {
        _listener->on_screen_lbutton_dblclk(flags, point);
    }
}

void screen_actuator::on_screen_rbutton_down(unsigned int flags, const CPoint& point)
{
    if (_listener) {
        _listener->on_screen_rbutton_down(flags, point);
    }
}

void screen_actuator::on_screen_rbutton_up(unsigned int flags, const CPoint& point)
{
    if (_listener) {
        _listener->on_screen_rbutton_up(flags, point);
    }
}

void screen_actuator::on_screen_mbutton_down(unsigned int flags, const CPoint& point)
{
    if (_listener) {
        _listener->on_screen_mbutton_down(flags, point);
    }
}

void screen_actuator::on_screen_mbutton_up(unsigned int flags, const CPoint& point)
{
    if (_listener) {
        _listener->on_screen_mbutton_up(flags, point);
    }
}

void screen_actuator::on_screen_mouse_move(unsigned int flags, const CPoint& point)
{
    if (_listener) {
        _listener->on_screen_mouse_move(flags, point);
    }
}

void screen_actuator::on_screen_mouse_wheel(unsigned int flags, short zdelta, const CPoint& point)
{
    if (_listener) {
        _listener->on_screen_mouse_wheel(flags, zdelta, point);
    }
}

bool screen_actuator::on_screen_set_cursor(CWnd* pWnd, unsigned int hittest, unsigned int message, int camera, const CRect& rect)
{
    return (_listener) ?
            _listener->on_screen_set_cursor(pWnd, hittest, message, camera, rect) : false;
}

void screen_actuator::on_screen_key_down(unsigned int nChar, unsigned int nRepCnt, unsigned int nFlags)
{
    if (_listener) {
        _listener->on_screen_key_down(nChar, nRepCnt, nFlags);
    }
}

void screen_actuator::on_screen_key_up(unsigned int nChar, unsigned int nRepCnt, unsigned int nFlags)
{
    if (_listener) {
        _listener->on_screen_key_up(nChar, nRepCnt, nFlags);
    }
}

void screen_actuator::on_screen_set_foucs(CWnd* pOldWnd)
{
    if (_listener) {
        _listener->on_screen_set_foucs(pOldWnd);
    }
}

void screen_actuator::on_screen_resized(unsigned int type, int cx, int cy)
{
    if (_listener) {
        _listener->on_screen_resized(type, cx, cy);
    }
}

void screen_actuator::fire_connected(short camera /*= -1*/)
{
    _screen.fire_connected(camera);
}

void screen_actuator::fire_disconnected(short camera /*= -1*/)
{
    _screen.fire_disconnected(camera);
}

void screen_actuator::fire_camera_changed(short camera /*= -1*/, bool post /*= true*/)
{
    _screen.fire_camera_changed(camera, post);
}

//////////////////////////////////////////////////////////////////////////

void __stdcall screen_actuator::looper_func(void)
{
    actuator_video();
}
