// frame_buffer_manager.h : header file
//

#ifndef _SCREEN_FRAME_BUFFER_MANAGER_H_
#define _SCREEN_FRAME_BUFFER_MANAGER_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "screen_common.h"
#include "frame_buffer/frame_buffer.h"

#include <utility/g2retention_bit.h>
#include <utility/g2channels.h>
#include <boost/noncopyable.hpp>
#include <vector>
#include <set>

namespace client {

class screen_actuator;

//////////////////////////////////////////////////////////////////////////

class frame_buffer_manager : private boost::noncopyable
{
public:
    frame_buffer_manager(void);
    ~frame_buffer_manager(void);

protected:
    typedef std::vector<frame_buffer*> container_type;

    container_type      _container;
    std::vector<bool>   _prepareDrive;     // for skip frame while prepare (ex. rollback)

protected:
    int _countchannel;
    int _countcamera;
    int _countlooper;
    int _nextchannel;
    int _selcamera;

    bool _audioIgnore;

protected:
    g2::critical_section  _spnBuffer;
    g2::critical_section* _spnRemove;

public:
    void create(int countchannel, int countcamera);
    void cleanup(void);

    void setup_buffer(short channel, client::frame_buffer::TYPE type, client::frame_buffer::PLAYTIME time);
    void remove(short channel);
    void remove(void);

    void init(short channel);
    void init(short channel, int channelext);

    void clear_buffer(short channel, int hostcamera);
    void clear_buffer(short channel, const g2::channels& channelext);
    void clear_buffer(short channel, const std::set<int>& channelexts);
    void clear_buffer(short channel);

    void exit_buffer(short channel);

    bool put_frame(const G2FRAME& frame, short channel, short basecamera, unsigned __int64 refcameras);
    int get_frame(frame_element& element);

    void set_prepare_drive(short channel, bool prepare = true);
    void set_ignore_audio(bool ignore);
    void set_play_speed(short channel, int speed);

    int play_speed(short channel);

    int count_frame(short channel) const;
    int count_frame(short channel, int channelext) const;
    int count_video(short channel) const { return count_frame(channel); }
    int count_video(short channel, int channelext) const { return count_frame(channel, channelext); }

    bool is_prepare_drive(short channel) const;
    void on_screen_changed_camera(short camera);

    bool valid_scrcamera(short camera) const;
    bool valid_channel(short channel) const;
    bool valid_buffers(short channel) const;
};

//////////////////////////////////////////////////////////////////////////

inline bool frame_buffer_manager::is_prepare_drive(short channel) const
{
    return (valid_channel(channel)) ?
            _prepareDrive[channel] : false;
}

inline bool frame_buffer_manager::valid_scrcamera(short camera) const
{
    return (camera < 0 ||
            camera >= _countcamera) ? false : true;
}

inline bool frame_buffer_manager::valid_channel(short channel) const
{
    return (channel < 0 ||
            channel >= _countchannel) ? false : true;
}

inline bool frame_buffer_manager::valid_buffers(short channel) const
{
    return (valid_channel(channel) &&
            _container[channel] != NULL);
}

//////////////////////////////////////////////////////////////////////////

} // !_namespace_idis

#endif // !_SCREEN_FRAME_BUFFER_MANAGER_H_
