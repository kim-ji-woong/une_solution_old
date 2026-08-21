// screen_actuator.h : header file
//

#ifndef _SCREEN_ACTUATOR_H_
#define _SCREEN_ACTUATOR_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "screen_actuator_listener.h"
#include "frame_buffer/frame_buffer.h"
#include "frame_buffer_manager.h"

#include <utility/g2looper.h>
#include <boost/unordered_set.hpp>

namespace client {

//////////////////////////////////////////////////////////////////////////

class screen_actuator
{
public:
    screen_actuator(void);
    ~screen_actuator(void);

protected:
    int _countcamera;
    int _countchannel;
    int _speedPlay;

    bool _cleanup;

    volatile bool _ignoreFrame;
    volatile bool _ignoreDisplay;
    volatile unsigned int _refsleep;

    G2SPOT _spotVideo;
    boost::unordered_set<short> _ignoreChannels;
    std::vector<G2SPOT> _lastLoadedSpot;

    frame_element _videoFrame;
    frame_buffer_manager _bufferManager;

    typedef g2::looper_<screen_actuator> looper_type;
    looper_type _looper;

    screen_actuator_listener* _listener;

protected:
    void initialize(void);
    bool is_cleanup(void) const { _cleanup; }

public:
    bool create(int countcamera = 36, int countchannel = 16);

    bool cleanup(void);

    void set_listener(screen_actuator_listener* listener);
    void set_priority(int priority = THREAD_PRIORITY_NORMAL);
    int get_last_displayed_spot(unsigned __int64& cameras, G2SPOT& spot);

public:
    void prepare_looper(int priority);
    void release_looper(void);

    void run(void);
    void suspend(void);
    void resume(void);

    bool is_suspended(void) const;
    bool is_suspending(void) const;
    bool is_running(void) const;

    void __stdcall looper_func(void);

protected:
    void actuator_video(void);
    void actuator_play(const frame_element& element);
    void video_play(screen::looper_::MODE mode, const frame_element& element);

    void play_search_audio(void) {}
    void stop_search_audio(void) {}
    void set_search_audio_spot(const G2SPOT& spot) {}

public:
    void put_frame(G2FRAME::FROM from, const G2FRAME& frame, short channel, short basecamera, unsigned __int64 refcameras);

    void setup_buffer(short channel, frame_buffer::TYPE type, frame_buffer::PLAYTIME time = frame_buffer::TIME_MSEC);

    void clear_frame_buffer(short channel);
    void clear_frame_buffer(short channel, int hostcamera);
    void clear_frame_buffer(short channel, const g2::channels& channelext);
    void clear_frame_buffer(short channel, const std::set<int>& channelexts);
    void clear_frame_buffer_play(short channel);
    void clear_frame_buffer_play(short channel, int channelext);

    void remove_buffer(short channel);
    void remove_buffer(void);

    void set_prepare_drive(short channel, bool prepare);
    bool is_prepare_drive(short channel) const;

    bool is_ignore_frame(void) const { return _ignoreFrame; }
    void set_ignore_frame(bool ignore) { _ignoreFrame = ignore; }

    bool is_ignore_display(void) const { return _ignoreDisplay; }
    void set_ignore_display(bool ignore) { _ignoreDisplay = ignore; }

    void set_search_shutdown(short channel, unsigned __int64 cameras);
    void set_search_play_speed(short channel, int speed, bool audioIgnore = false);
    void set_search_end_play(G2FRAME::FROM from, int channel);

    void stop_search_enter(short channel);
    void stop_search_leave(short channel);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_SCREEN_ACTUATOR_H_