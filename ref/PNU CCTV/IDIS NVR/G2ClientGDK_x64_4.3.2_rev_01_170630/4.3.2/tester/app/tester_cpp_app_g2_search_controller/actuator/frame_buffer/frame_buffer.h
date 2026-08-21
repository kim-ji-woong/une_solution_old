// frame_buffer.h : header file
//

#ifndef _SCREEN_FRAME_BUFFER_H_
#define _SCREEN_FRAME_BUFFER_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "frame_element.h"
#include <actuator/screen_common.h>
#include <utility/g2channels.h>
#include <boost/noncopyable.hpp>
#include <vector>
#include <set>
#include <mmsystem.h>
namespace client {

//////////////////////////////////////////////////////////////////////////

class performance_counter
{
public:
    performance_counter(void)
        : _begin(0)
        , _end(0) {
    }

private:
    unsigned long _begin, _end;

public:
    unsigned long begin(void) const { return _begin; }
    unsigned long end(void)   const { return _end;   }

    void do_begin(void) {
        _begin = ::timeGetTime();
    }

    void do_end(void) {
        _end = ::timeGetTime();
    }

    int result(void) const {
        return (_end - _begin);
    }

    double result_sec(void) const {
        return (_end - _begin) / 1000.0;
    }

    __int64 result_micro(void) const {
        return (_end - _begin) * 1000;
    }

    bool empty(void) const {
        return (_begin == 0 && _end == 0);
    }

    bool empty_begin(void) const {
        return (_begin == 0);
    }

    bool empty_end(void) const {
        return (_end == 0);
    }

    void reset(void) {
        _begin = _end = 0;
    }

    void reset_begin(void) {
        _begin = 0;
    }

    void reset_end(void) {
        _end = 0;
    }

    void equal_begin(void) {
        _end = _begin;
    }

    void equal_end(void) {
        _begin = _end;
    }
};

//////////////////////////////////////////////////////////////////////////

class frame_buffer : private boost::noncopyable
{
public:
    enum MODE {
        MODE_UNDEFINED = 0x0,
        MODE_LIVE   = 0x0001,
        MODE_PLAY   = 0x0002,
        MODE_WATCH  = 0x0004,
        MODE_SEARCH = 0x0008
    };

    enum TYPE {
        TYPE_UNDEFINED = -2,
        TYPE_REMOVE = -1,
        TYPE_WATCH = 0,
        TYPE_SEARCH,
        TYPE_SEARCH_SAMBA,
        TYPE_SEARCH_IDR,
        TYPE_SEARCH_G2,
    };

    enum SIZE {
        SIZE_WATCH  = 240,
        SIZE_SEARCH = 32,
        SIZE_SEARCH_SAMBA = 120,
        SIZE_SEARCH_IDR   = 120,
        SIZE_SEARCH_G2    = 240,
        SIZE_AUDIO  = 64
    };

    enum PLAYTIME {
        TIME_NOT_ASSIGN = -1,
        TIME_MSEC       = 0,
        TIME_TICK,
        TIME_SYSMSEC        // system msec
    };

public:
    frame_buffer(MODE mode, TYPE type, int limit);
    virtual ~frame_buffer(void);

public:
    virtual void initialize(void) = 0;
    virtual void initialize(int channelext) = 0;
    virtual void cleanup(void) = 0;

    virtual void clear(int channelext) = 0;
    virtual void clear(const g2::channels& channelext) = 0;
    virtual void clear(const std::set<int>& channelexts) = 0;
    virtual void clear(void) = 0;

    virtual void remove_ref_camera(short scrcamera) = 0;
    virtual void append_ref_camera(int channelext, short scrcamera) = 0;

    virtual void push(const G2FRAME& frame, short channel, short basecamera, unsigned __int64 refcameras) = 0;
    virtual bool pop(frame_element& element) = 0;
    virtual bool top(frame_element& element) = 0;

    virtual void set_play_speed(int speed) { }
    virtual void changed_layout(int layout, int count, int changed) = 0;

    virtual bool is_cleared(int hstcamera) const = 0;
    virtual bool is_locked(void) const { return false; }
    virtual bool is_unlocked(void) { return !is_locked(); }
    virtual int play_speed(void) const { return screen::play_speed_::NORMAL; }

    virtual int count(void) const = 0;
    virtual int count(int channelext) const = 0;
    virtual int capacity(void) const { return _capacity; }
    virtual short channel(void) const { return _channel; }

public:
    void set_channel(short channel) { _channel = channel; }
    MODE mode(void) const { return _mode; }
    bool is_mode(MODE mode) const { return (_mode == mode); }
    bool is_mode(int modes) const { return (_mode & modes) != 0; }
    bool is_type(TYPE type) const { return (_type == type); }

protected:
    bool valid_host_camera(short hostcamera) const;
    bool valid_channelext(int channelext) const;
    bool valid_stream(short stream) const;

public:
    static size_t calc_len_YV12(size_t cx, size_t cy);
    static size_t calc_len_RGB(size_t cx, size_t cy, int bpp = 32);

protected:
    const MODE _mode;
    const TYPE _type;
    const int  _limit;

    int _capacity;
    short _channel;
};

//////////////////////////////////////////////////////////////////////////

inline bool frame_buffer::valid_host_camera(short hstcamera) const
{
    return (hstcamera >= 0 && hstcamera < screen::count_::HOST_CAMERA);
}

inline bool frame_buffer::valid_channelext(int channelext) const
{
    return (channelext >= 0);
}

inline bool frame_buffer::valid_stream(short stream) const
{
    return (stream >= 0 && stream < screen::count_::LIVE_STREAM);
}

//////////////////////////////////////////////////////////////////////////

inline size_t frame_buffer::calc_len_YV12(size_t cx, size_t cy)
{
    if (cx & 0x01) ++cx;
    if (cy & 0x01) ++cy;
    return (cx * (cy + (cy >> 1)));
}

inline size_t frame_buffer::calc_len_RGB(size_t cx, size_t cy, int bpp /*= 32*/)
{
    return ((((cx * bpp) + 31) & ~31) >> 3) * cy;
}

//////////////////////////////////////////////////////////////////////////

} // namespace_client

#endif // !_SCREEN_FRAME_BUFFER_H_
