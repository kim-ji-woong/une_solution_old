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

bool screen_actuator::create(int countcamera /*= 36*/,
                             int countchannel /*= 16*/)
{
    _cleanup = false;
    _countcamera  = (countcamera < 1) ? 1 : countcamera;
    _countchannel = (countchannel < 1) ? 1 : countchannel;

    G2SPOT spot;
    g2_spot_make_invalid(&spot);
    _lastLoadedSpot.resize(_countcamera, spot);

    _bufferManager.create(_countchannel, _countcamera);

    prepare_looper(THREAD_PRIORITY_NORMAL);

    return true;
}

bool screen_actuator::cleanup(void)
{
    _cleanup = true;

    release_looper();

    _bufferManager.cleanup();
    
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

int screen_actuator::get_last_displayed_spot(unsigned __int64& cameras, G2SPOT& spot)
{
    g2_spot_make_invalid(&spot);
    int camera = client::invalid_::CAMERA_NUMBER;

    for (int i = 0; i < _countcamera; ++i) {
        if (g2::contains_bit(cameras, i) != true) continue;
        G2SPOT& temp = _lastLoadedSpot[i];
        if (g2_spot_is_valid(&temp) &&
           (g2_spot_is_valid(&spot) == false ||
            g2_spot_compare(&temp, &spot) >= 0)) {
            spot = temp;
            camera = i;
        }
    }
    return camera;
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
    int retcamera = -1;
    if (_videoFrame.valid() != true) {
        retcamera = _bufferManager.get_frame(_videoFrame);
    }
    if (_videoFrame.valid()) {
        const G2FRAME::FROM from = _videoFrame.from();
        if (from == G2FRAME::FROM_SEARCH_G2 && 
            valid_camera(retcamera)) {
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

void screen_actuator::actuator_play(const frame_element& element)
{
    if (element.is_status(frame_element::NOFRAME_LOADED)) {
        if (_listener) {
            _listener->on_screen_no_image_loaded(element.channel(), element.camera());
        }
        return;
    }

    if (element.camera() >= 0 &&
        _ignoreChannels.find(element.channel()) == _ignoreChannels.end()) {

        int camera = element.camera();
        if (camera < _countcamera)
            _lastLoadedSpot[camera] = element.frame()._index._spot;

        if (_listener) {
            _listener->on_screen_image_loaded(element.frame());
        }
    }
}

//////////////////////////////////////////////////////////////////////////

void screen_actuator::put_frame(G2FRAME::FROM from, const G2FRAME& frame, short channel, short basecamera, unsigned __int64 refcameras)
{
    if (is_ignore_frame()) {
        TRACE(L"@ screen_actuator::ignore host[%d] stream : %d\n", channel, basecamera);
        return;
    }

    if (basecamera >= 0 &&
        is_running() != true) {
        TRACE(L"@ screen_actuator::put frame failed::actuator is not working - %s\n", frame._title);
        return;
    }

    _bufferManager.put_frame(frame, channel, basecamera, refcameras);
}

void screen_actuator::clear_frame_buffer(short channel)
{
    _bufferManager.clear_buffer(channel);
}

void screen_actuator::clear_frame_buffer(short channel, int hostcamera)
{
    _bufferManager.clear_buffer(channel, hostcamera);
}

void screen_actuator::clear_frame_buffer(short channel, const g2::channels& channelext)
{
    _bufferManager.clear_buffer(channel, channelext);
}

void screen_actuator::clear_frame_buffer(short channel, const std::set<int>& channelexts)
{
    _bufferManager.clear_buffer(channel, channelexts);
}

void screen_actuator::clear_frame_buffer_play(short channel)
{
    _bufferManager.init(channel);
}

void screen_actuator::clear_frame_buffer_play(short channel, int channelext)
{
    _bufferManager.init(channel, channelext);
}

//////////////////////////////////////////////////////////////////////////

void screen_actuator::setup_buffer(short channel, frame_buffer::TYPE type, frame_buffer::PLAYTIME time /*= frame_buffer::TIME_MSEC*/)
{
    _bufferManager.setup_buffer(channel, type, time);

    switch (type) {
        case frame_buffer::TYPE_UNDEFINED:
        case frame_buffer::TYPE_REMOVE:
            /*if (channel == _bufferManager.get_audio_channel()) {
                stop_search_audio();
            }*/
            break;
    }
}

void screen_actuator::remove_buffer(short channel)
{
    _bufferManager.remove(channel);
}

void screen_actuator::remove_buffer(void)
{
    _bufferManager.remove();
}

void screen_actuator::set_prepare_drive(short channel, bool prepare)
{
    _bufferManager.set_prepare_drive(channel, prepare);
}

bool screen_actuator::is_prepare_drive(short channel) const
{
    return _bufferManager.is_prepare_drive(channel);
}

void screen_actuator::set_search_shutdown(short channel, unsigned __int64 cameras)
{
    _bufferManager.exit_buffer(channel);
}

void screen_actuator::set_search_play_speed(short channel, int speed, bool audioIgnore /*= false*/)
{
    if (audioIgnore ||
        speed != screen::play_speed_::NORMAL) {
        stop_search_audio();
    }

    _speedPlay = speed;
    _bufferManager.set_ignore_audio(audioIgnore);
    _bufferManager.set_play_speed(channel, speed);
}

void screen_actuator::set_search_end_play(G2FRAME::FROM from, int channel)
{
    G2FRAME frame = { 0 };
    frame._index._channel = frame_element::NOFRAME_LOADED;
    frame._index._from = from;

    put_frame(from, frame, channel, frame_element::NOFRAME_LOADED, 0);
}

void screen_actuator::stop_search_enter(short channel)
{
    _bufferManager.clear_buffer(channel);
    _ignoreChannels.insert(channel);
}

void screen_actuator::stop_search_leave(short channel)
{    
    boost::unordered_set<short>::iterator itr = _ignoreChannels.find(channel);
    if (itr != _ignoreChannels.end()) {
        _ignoreChannels.erase(itr);
    }
}

//////////////////////////////////////////////////////////////////////////

void __stdcall screen_actuator::looper_func(void)
{
    actuator_video();
}
