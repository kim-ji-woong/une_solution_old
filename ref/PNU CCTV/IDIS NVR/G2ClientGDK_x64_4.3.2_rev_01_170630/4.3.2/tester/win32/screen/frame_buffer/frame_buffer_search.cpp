// frame_buffer_search.cpp : implementation file
//

#include "stdafx.h"
#include "frame_buffer_search.h"

#include <mmsystem.h>

#pragma comment(lib, "winmm.lib")

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

frame_buffer_search::frame_buffer_search(TYPE type, int limit, frame_buffer::PLAYTIME timeType)
    : frame_buffer(frame_buffer::MODE_SEARCH, type, limit)
    , _buffer(limit)
    , _timeType(timeType)
{
    _speedcheck = false;
    _audioplay = false;
    _speed = 0;
    _prevSleep = 0;
    _prevImageTick = 0i64;
    _currImageTick = 0i64;
    g2_time_make_invalid(&_prevImageTime);
    g2_time_make_invalid(&_currImageTime);
    _timerId = 0;
    _locked = false;
    _inited = false;
}

frame_buffer_search::~frame_buffer_search(void)
{
    cleanup();
}

//////////////////////////////////////////////////////////////////////////

void frame_buffer_search::initialize(void)
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

void frame_buffer_search::initialize(int hostcamera)
{
    G2RETURN_IF_FAIL(valid_host_camera(hostcamera));

    g2::scoped_criticalsection lock(_lock);

    _buffer.clear(hostcamera);
    clear(hostcamera);
}

void frame_buffer_search::cleanup(void)
{
    g2::scoped_criticalsection lock(_lock);

    _inited = true;
    _locked = false;

    _buffer.clear();
}

//////////////////////////////////////////////////////////////////////////

void frame_buffer_search::clear(int hostcamera)
{
    G2RETURN_IF_FAIL(valid_host_camera(hostcamera));

    g2::scoped_criticalsection lock(_lock);

    _internal.clear_buffer(hostcamera, true);
}

void frame_buffer_search::clear(const g2::channels& hostcameras)
{
    assert(!"not support");
}

void frame_buffer_search::clear(const std::set<int>& channelexts)
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

void frame_buffer_search::clear(void)
{
    g2::scoped_criticalsection lock(_lock);

    // reset map restricted registered item at clear buffer
    _internal.reset();
}

void frame_buffer_search::remove_ref_camera(short scrcamera)
{
    g2::scoped_criticalsection lock(_lock);

    _buffer.clear_ref(scrcamera);
}

void frame_buffer_search::append_ref_camera(int hostcamera, short scrcamera)
{
    g2::scoped_criticalsection lock(_lock);

    if (valid_host_camera(hostcamera)) {
        _buffer.append_ref(hostcamera, scrcamera);
    }
}

//////////////////////////////////////////////////////////////////////////

void frame_buffer_search::push(const G2FRAME& frame, short channel, short basecamera, unsigned __int64 refcameras)
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

bool frame_buffer_search::pop(frame_element& element)
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
    const int hostcamera = frame._index._channel;
    if (!valid_host_camera(hostcamera) ||
        is_cleared(hostcamera)) {
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

    if (_prevImageTick == 0i64) {
        _prevImageTick = rspot._tick;
        _prevImageTime = rspot._time;
    }

    if (g2_time_is_valid(&_prevImageTime) != true) {
        _prevImageTime = rspot._time;
    }

    _currImageTick = rspot._tick;
    _currImageTime = rspot._time;
    _perfcounter.do_end();

    // reckon time-diff for between previous image and current image
    __int64 diffTimer = _perfcounter.result();
    __int64 diffImage = _currImageTick - _prevImageTick;

    double tickFactor = FRAME_RATE_SYSMSEC;

    if (_timeType == frame_buffer::TIME_TICK) {
        switch (frame._height) {
            case 144:
            case 288:
            case 576:
            case 256:
            case 272:
            case 544: tickFactor = FRAME_RATE_PAL; break;
            default : tickFactor = FRAME_RATE_NTSC; break;
        }

        diffImage = (__int64)((_currImageTick - _prevImageTick) * tickFactor);
        element.set_diff_tick(diffImage);
    }
    else if (_timeType == frame_buffer::TIME_SYSMSEC) {
        diffImage = _currImageTick - _prevImageTick;
        element.set_diff_tick(diffImage);
    }
    else {
        unsigned long ts = g2_time_to_utc32(&_currImageTime) - g2_time_to_utc32(&_prevImageTime);
        diffImage = ts * 1000 + (_currImageTick - _prevImageTick);
        element.set_diff_tick(diffImage);
    }

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
    _prevImageTime = _currImageTime;
    _perfcounter.equal_end();

    return true;
}

bool frame_buffer_search::top(frame_element& element)
{
    g2::scoped_criticalsection lock(_lock);

    return _buffer.top(element);
}

//////////////////////////////////////////////////////////////////////////

void frame_buffer_search::set_play_speed(int speed)
{
    g2::scoped_criticalsection lock(_lock);

    _speedcheck = (speed != 0 && speed != _I32_MAX) ? true : false;
    _speed = speed;
    _prevSleep  = 0;
    _prevImageTick = 0;
    g2_time_make_invalid(&_prevImageTime);
    _perfcounter.reset_begin();
}

void frame_buffer_search::changed_layout(int layout, int count, int changed)
{
    _prevSleep = 0;
    _prevImageTick = 0;
    g2_time_make_invalid(&_prevImageTime);
    _perfcounter.reset();
}

bool frame_buffer_search::is_cleared(int channelext) const
{
    return _internal.is_cleared(channelext);
}

int frame_buffer_search::count(void) const
{
    return _buffer.count();
}

int  frame_buffer_search::count(int channelext) const
{
    return _buffer.count(channelext);
}

//////////////////////////////////////////////////////////////////////////

void CALLBACK frame_buffer_search::timer_proc(UINT id, UINT msg, DWORD_PTR user, DWORD_PTR param1, DWORD_PTR param2)
{
    frame_buffer_search* buff = (frame_buffer_search*)(user);
    G2RETURN_IF_FAIL(buff != NULL);

    if (buff->time_id() == id) {
        buff->timer_func();
    }
}

void frame_buffer_search::begin_timer(unsigned int interval)
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

void frame_buffer_search::timer_func(void)
{
    _locked = false;
}
