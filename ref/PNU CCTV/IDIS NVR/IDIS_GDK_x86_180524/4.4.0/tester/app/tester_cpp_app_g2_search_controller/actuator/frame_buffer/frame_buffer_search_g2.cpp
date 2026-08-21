// frame_buffer_search_g2.cpp : implementation file
//

#include "stdafx.h"
#include "frame_buffer_search_g2.h"

#include <mmsystem.h>

#pragma comment(lib, "winmm.lib")

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

frame_buffer_search_g2::frame_buffer_search_g2(TYPE type, int limit /*= SIZE::SIZE_PLAY*/)
    : frame_buffer(frame_buffer::MODE_PLAY, type, limit)
    , _buffer(limit)
{
    _speedcheck = false;
    _audioplay = false;
    _speed = 0;
    _prevSleep = 0;
    _prevImageTick = 0i64;
    _currImageTick = 0i64;
    _timerId = 0;
    _locked = false;
    _inited = false;
}

frame_buffer_search_g2::~frame_buffer_search_g2(void)
{
    cleanup();
}

//////////////////////////////////////////////////////////////////////////

void frame_buffer_search_g2::initialize(void)
{
    g2::scoped_criticalsection lock(_lock);

    _buffer.clear();
    _inited = true;
    _locked = false;
    _prevSleep = 0;
    _prevImageTick = 0i64;
    _perfcounter.reset();

    clear();
}

void frame_buffer_search_g2::initialize(int channelext)
{
    G2RETURN_IF_FAIL(valid_channelext(channelext));

    g2::scoped_criticalsection lock(_lock);

    _buffer.clear(channelext);
    clear(channelext);
}

void frame_buffer_search_g2::cleanup(void)
{
    g2::scoped_criticalsection lock(_lock);

    _inited = true;
    _locked = false;

    _buffer.clear();
}

//////////////////////////////////////////////////////////////////////////

void frame_buffer_search_g2::clear(int channelext)
{
    G2RETURN_IF_FAIL(valid_channelext(channelext));

    g2::scoped_criticalsection lock(_lock);

    _internal.clear_buffer(channelext, true);
}

void frame_buffer_search_g2::clear(const g2::channels& channelext)
{
    assert(!"not support");
}

void frame_buffer_search_g2::clear(const std::set<int>& channelexts)
{
    g2::scoped_criticalsection lock(_lock);

    int channelext = -1;
    for (std::set<int>::const_iterator itr(channelexts.begin());
         itr != channelexts.end();
         ++itr) {
        if (valid_channelext(channelext = *itr)) {
            _internal.clear_buffer(channelext, true);
        }
    }
}

void frame_buffer_search_g2::clear(void)
{
    g2::scoped_criticalsection lock(_lock);

    // reset map restricted registered item at clear buffer
    _internal.reset();
}

void frame_buffer_search_g2::remove_ref_camera(short scrcamera)
{
    g2::scoped_criticalsection lock(_lock);

    _buffer.clear_ref(scrcamera);
}

void frame_buffer_search_g2::append_ref_camera(int channelext, short scrcamera)
{
    g2::scoped_criticalsection lock(_lock);

    if (valid_channel_ext(channelext)) {
        _buffer.append_ref(channelext, scrcamera);
    }
}

//////////////////////////////////////////////////////////////////////////

void frame_buffer_search_g2::push(const G2FRAME& frame, short channel, short basecamera, unsigned __int64 refcameras)
{
    bool discard = false;

    if (basecamera >= 0 &&
        frame._index._type != G2FRAME::AUDIO) {
        const int channelext = frame._index._channel;
        if (_internal.is_cleared(channelext)) {
            if (frame._index._type == G2FRAME::I_FRAME) {
                _internal.clear_buffer(channelext, false);
            }
            else {
                return;
            }
        }

        discard = ((frame._index._flag & (G2FRAME::BROKEN_DATA_HEADER | G2FRAME::BROKEN_DATA_BODY)) != 0);
        if (_internal.is_discard(channelext)) {
            if (frame._index._type == G2FRAME::I_FRAME &&
                discard != true) {
                _internal.discard(channelext, false);
            }
            else {
                discard = true;
            }
        }
        else {
            if (discard) {
                _internal.discard(channelext, true);
            }
        }
    }

    if (_inited) {
        _inited = false;
    }

    bool pushed = false;
    while (!(pushed = _buffer.push(frame, channel, basecamera, refcameras, discard))) {
        // wait 100 msec if there is not efficient space.
        _buffer.lockFullMutex(100);

        // current this frame not use,
        // because buffer is cleared but status is init
        if (_inited) {
            _inited = false;
            break;
        }
    }
}

bool frame_buffer_search_g2::pop(frame_element& element)
{
    g2::scoped_criticalsection lock(_lock);

    if (!_buffer.pop(element)) {
        return false;
    }

    if (element.is_status(frame_element::INVALID_LOADED)) {
        return false;
    }

    if (element.is_status(frame_element::NOFRAME_LOADED)) {
        return true;
    }

    if (element.is_audio()) {
        return true;
    }

    const G2FRAME& frame = element.frame();
    const G2SPOT&  rspot = frame._index._spot;
    const int  frameType = frame._index._type;
    const int channelext = frame._index._channel;
    if (!valid_channelext(channelext) ||
        is_cleared(channelext)) {
        return false;
    }

    if (is_ignore_speed()) {
        return true;
    }

    if (element.is_not_display()) {
        return true;
    }

    if (_perfcounter.empty_begin()) {
        _perfcounter.do_begin();
    }

    const __int64 tick = frame_buffer::is_mode(frame_buffer::MODE_PLAY) ?
                                               rspot._tick : element.extern_tick();

    if (_prevImageTick == 0) {
        _prevImageTick = tick;
    }

    _currImageTick = tick;
    _perfcounter.do_end();

    __int64 diffTimer = _perfcounter.result();
    __int64 diffImage = _currImageTick - _prevImageTick;

    element.set_diff_tick(diffImage);
    element.set_system_tick(tick);

    const int speed = _speed;

    if (speed != _I32_MAX) {
        diffImage = static_cast<__int64>(static_cast<double>(diffImage * 10.0) / speed);
        int predicted = static_cast<int>(diffImage - diffTimer + _prevSleep);

        if (predicted < -10000) {
            predicted = 0;
        }
        else if (predicted > 0) {
            if (predicted > 1000) {
                predicted = 1000;
            }
            begin_timer(predicted);
        }

        _prevSleep = predicted;
    }
    else {
        _prevSleep = 0;
    }

    _prevImageTick = _currImageTick;
    _perfcounter.equal_end();

    return true;
}

bool frame_buffer_search_g2::top(frame_element& element)
{
    g2::scoped_criticalsection lock(_lock);

    return _buffer.top(element);
}

//////////////////////////////////////////////////////////////////////////

void frame_buffer_search_g2::set_play_speed(int speed)
{
    g2::scoped_criticalsection lock(_lock);

    _speedcheck = (speed != 0 && speed != _I32_MAX) ? true : false;
    _speed = speed;
    _prevSleep  = 0;
    _prevImageTick = 0;
    _perfcounter.reset_begin();
}

void frame_buffer_search_g2::changed_layout(int layout, int count, int changed)
{
    _prevSleep = 0;
    _prevImageTick = 0;
    _perfcounter.reset();
}

bool frame_buffer_search_g2::is_cleared(int channelext) const
{
    return _internal.is_cleared(channelext);
}

int frame_buffer_search_g2::count(void) const
{
    return _buffer.count();
}

int  frame_buffer_search_g2::count(int channelext) const
{
    return _buffer.count(channelext);
}

//////////////////////////////////////////////////////////////////////////

void CALLBACK frame_buffer_search_g2::timer_proc(UINT id, UINT msg, DWORD_PTR user, DWORD_PTR param1, DWORD_PTR param2)
{
    frame_buffer_search_g2* buff = (frame_buffer_search_g2*)(user);
    G2RETURN_IF_FAIL(buff != NULL);

    if (buff->time_id() == id) {
        buff->timer_func();
    }
}

void frame_buffer_search_g2::begin_timer(unsigned int interval)
{
    _locked = true;

    TIMECAPS timecaps;
    timeGetDevCaps(&timecaps, sizeof(TIMECAPS));

    _timerId = ::timeSetEvent(interval,
                              timecaps.wPeriodMax,
                              timer_proc,
                              (DWORD_PTR)this,
                              TIME_ONESHOT | TIME_CALLBACK_FUNCTION);

    if (_timerId) {
        // success
    }
    else {
        _locked = false;
        assert(!"failed to begin one shot timer");
    }
}

void frame_buffer_search_g2::timer_func(void)
{
    _locked = false;
}
