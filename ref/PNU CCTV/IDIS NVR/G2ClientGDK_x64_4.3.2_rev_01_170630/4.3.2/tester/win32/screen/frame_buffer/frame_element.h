// frame_element.h : header file
//

#ifndef _SCREEN_FRAME_BUFFER_FRAME_ELEMENT_H_
#define _SCREEN_FRAME_BUFFER_FRAME_ELEMENT_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_define.h>
#include <boost/shared_ptr.hpp>
#include <vector>

namespace client {

//////////////////////////////////////////////////////////////////////////

class frame_element
{
public:
    frame_element(void);
    frame_element(const G2FRAME& frame, short channel, short camera, unsigned __int64 refcameas, bool discard = false, bool alloc = true);
    ~frame_element(void);

public:
    enum LOADED {
        INVALID_LOADED = -1,
        NOFRAME_LOADED = -2
    };

protected:
    G2FRAME _frame;

    short _channel;
    short _camera;
    unsigned __int64 _refcameras;

    bool _discard;       // ex. skip broken frame
    bool _valid;

    __int64 _difftick;      // tick difference between frames (play mode)
    __int64 _systemtick;    // tick system tick (play mode)
    __int64 _externtick;

protected:
    typedef frame_element this_type;
    typedef std::vector<unsigned char> memory_buffer;
    typedef g2::critical_section lock_type;

    boost::shared_ptr<memory_buffer> _frameBuffer;
    lock_type __spin__;

public:
    G2FRAME::FROM from(void) const { return G2FRAME::FROM(_frame._index._from); }
    short channel(void) const { return _channel; }
    int channel_ext(void) const { return _frame._index._channel; }
    int type(void) const { return _frame._index._type; }
    short camera(void) const { return _camera; }
    unsigned __int64 ref_cameras(void) const { return _refcameras; }

    bool discard(void) const { return _discard; }
    bool valid(void) const { return _valid; }
    bool invalid(void) const { return !valid(); }

    const G2FRAME& frame(void) const { return _frame; }
    const G2SPOT& spot(void) const { return _frame._index._spot; }
    const unsigned short height(void) const { return _frame._height; }
    const unsigned short width(void) const { return _frame._width; }

    const __int64& diff_tick(void) const { return _difftick; }
    const __int64& system_tick(void) const { return _systemtick; }
    const __int64& extern_tick(void) const { return _externtick; }

    ///////////////////////////////////////////////////////
    bool is_status(frame_element::LOADED type) const { return (_camera == type); }

    bool is_from(int froms) const { return (_frame._index._from & froms) != 0; }
    bool is_decoder(int type) const { return (_frame._extra._decoder == type); }
    bool is_frameType(char type) const { return (_frame._index._type == type); }
    bool is_xframe(void) const { return (_frame._index._type == G2FRAME::X_FRAME); }
    bool is_iframe(void) const { return (_frame._index._type == G2FRAME::I_FRAME); }
    bool is_audio(void) const { return (_frame._index._type == G2FRAME::AUDIO); }

    bool is_discard(void) const { return (_discard); }
    bool is_not_display(void) const { return (_frame._extra._display == false); }
    bool is_valid(void) const { return (_valid); }
    bool is_invalid(void) const { return (_valid == false); }
    bool is_display(void) const { return _frame._extra._display; }

    bool is_broken(void) const { return (_frame._index._flag & (G2FRAME::BROKEN_DATA_HEADER | G2FRAME::BROKEN_DATA_BODY)) != 0;  }
    bool is_brokenHead(void) const { return (_frame._index._flag & G2FRAME::BROKEN_DATA_HEADER) != 0; }
    bool is_brokenData(void) const { return (_frame._index._flag & G2FRAME::BROKEN_DATA_BODY) != 0;   }

public:
    const lock_type& locker(void) const { return __spin__; }
    bool trylock(void) const { return __spin__.trylock(); }
    void lock(void) const { __spin__.lock(); }
    void unlock(void) const { __spin__.unlock(); }
    void enter(void) const { __spin__.enter(); }
    void leave(void) const { __spin__.leave(); }

public:
    void set(const G2FRAME& frame, short channel, short camera, unsigned __int64 refcameras, bool discard = false, bool alloc = true);
    void set(const frame_element& element);
    void set_diff_tick(const __int64& tick) { _difftick = tick; }
    void set_system_tick(const __int64& tick) { _systemtick = tick; }
    void set_extern_tick(const __int64& tick) { _externtick = tick; }

    void attach(const frame_element& element);
    void reset(void);

    void append_ref_camera(int channelext, short scrcamera);
    void append_ref_camera(short scrcamera);
    void remove_ref_camera(short scrcamera);

protected:
    void allocate(const G2FRAME& frame);
};

//////////////////////////////////////////////////////////////////////////

const float FRAME_RATE_NTSC    = 16.66666667F;   // (1000. / 60.)
const float FRAME_RATE_PAL     = 20.00000000F;   // (1000. / 50.)
const float FRAME_RATE_SYSMSEC = 01.00000000F;   // for system msec

//////////////////////////////////////////////////////////////////////////

namespace screen {
    typedef boost::shared_ptr<frame_element> frame_element_ptr;
}


} // !_namespace_client

#endif // !_SCREEN_FRAME_BUFFER_FRAME_ELEMENT_H_
