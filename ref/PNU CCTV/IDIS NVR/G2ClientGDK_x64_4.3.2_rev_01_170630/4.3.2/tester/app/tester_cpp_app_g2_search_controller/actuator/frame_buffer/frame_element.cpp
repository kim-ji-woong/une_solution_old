// frame_element.cpp : implementation file
//

#include "stdafx.h"
#include "frame_element.h"
#include <utility/g2retention_bit.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

frame_element::frame_element(void)
    : _channel(-1)
    , _camera(-1)
    , _refcameras(0x00ui64)
    , _discard(false)
    , _valid(false)
    , _frameBuffer(new memory_buffer)
    , _difftick(_I64_MAX)
    , _systemtick(_I64_MAX)
    , _externtick(_I64_MAX)
{
    memset(&_frame, 0, sizeof(_frame));

    _frame._index._from = G2FRAME::FROM_UNDEFINED;
}

frame_element::frame_element(const G2FRAME& frame, short channel, short camera, unsigned __int64 refcameras, bool discard /*= false*/, bool alloc /*= true*/)
    : _frame(frame)
    , _channel(channel)
    , _camera(camera)
    , _refcameras(refcameras)
    , _discard(discard)
    , _valid(true)
    , _frameBuffer(new memory_buffer)
    , _difftick(_I64_MAX)
    , _systemtick(_I64_MAX)
    , _externtick(_I64_MAX)
{
    if (alloc) {
        allocate(_frame);

        // because step frame don't manage memory,
        // should manage buffer manually.
        memcpy(&(_frameBuffer->at(0)), _frame._data, _frame._index._data_size);
        _frame._data = &(_frameBuffer->at(0));
    }
}

frame_element::~frame_element(void)
{

}

//////////////////////////////////////////////////////////////////////////

void frame_element::set(const G2FRAME& frame, short channel, short camera, unsigned __int64 refcameras, bool discard /*= false*/, bool alloc /*= true*/)
{
    g2::scoped_criticalsection lock(__spin__);

    _frame = frame;
    _channel = channel;
    _camera = camera;
    _refcameras = refcameras;
    _discard = discard;
    _valid = true;

    if (alloc) {
        allocate(_frame);

        // because step frame don't manage memory,
        // should manage buffer manually.
        memcpy(&(_frameBuffer->at(0)), _frame._data, _frame._index._data_size);
        _frame._data = &(_frameBuffer->at(0));
    }
    else {
        _frameBuffer->clear();
    }
}

void frame_element::set(const frame_element& element)
{
    if (this == &element) return;

    g2::scoped_criticalsection lock1(__spin__);
    g2::scoped_criticalsection lock2(element.__spin__);

    _frame = element.frame();
    _channel = element.channel();
    _camera = element.camera();
    _refcameras = element.ref_cameras();
    _discard = element.discard();
    _valid = element.valid();
    _difftick = element.diff_tick();
    _systemtick = element.system_tick();
    _externtick = element.extern_tick();

    allocate(_frame);

    memcpy(&(_frameBuffer->at(0)), _frame._data, _frame._index._data_size);
    _frame._data = &(_frameBuffer->at(0));
}

void frame_element::attach(const frame_element& element)
{
    if (this == &element) return;

    g2::scoped_criticalsection lock1(__spin__);
    g2::scoped_criticalsection lock2(element.__spin__);

    _frame = element.frame();
    _channel = element.channel();
    _camera = element.camera();
    _refcameras = element.ref_cameras();
    _discard = element.discard();
    _valid = element.valid();
    _difftick = element.diff_tick();
    _systemtick = element.system_tick();
    _externtick = element.extern_tick();

    // sharing memory buffer
    _frameBuffer = element._frameBuffer;
}

void frame_element::reset(void)
{
    g2::scoped_criticalsection lock(__spin__);

    memset(&_frame, 0, sizeof(_frame));

    _channel = -1;
    _camera = -1;
    _refcameras = 0x00ui64;
    _discard = false;
    _valid = false;
    _difftick = _I64_MAX;
    _systemtick = _I64_MAX;
    _externtick = _I64_MAX;

    if (_frameBuffer.use_count() < 2) {
        _frameBuffer->clear();
    }
}

void frame_element::append_ref_camera(int channelext, short scrcamera)
{
    if (frame_element::channel_ext() == channelext) {
        g2::set_bit(_refcameras, scrcamera);
    }
}

void frame_element::append_ref_camera(short scrcamera)
{
    g2::set_bit(_refcameras, scrcamera);
}

void frame_element::remove_ref_camera(short scrcamera)
{
    if (scrcamera >= 0) {
        g2::reset_bit(_refcameras, scrcamera);
    }
}

void frame_element::allocate(const G2FRAME& frame)
{
    const size_t length = frame._index._data_size;

    _frameBuffer->resize(length + 1024);
}
