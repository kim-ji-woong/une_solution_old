// frame_buffer_manager.cpp : implementation file
//

#include "stdafx.h"
#include "frame_buffer_manager.h"
#include "frame_buffer/frame_buffer_search_g2.h"
#include "screen_actuator.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

frame_buffer_manager::frame_buffer_manager(void)
    : _countchannel(0)
    , _countcamera(0)
    , _nextchannel(0)
    , _spnRemove(NULL)
    , _selcamera(client::invalid_::CAMERA_NUMBER)
{

}

frame_buffer_manager::~frame_buffer_manager(void)
{
    cleanup();
}

//////////////////////////////////////////////////////////////////////////

void frame_buffer_manager::create(int countchannel, int countcamera)
{
    cleanup();  // neomind's excessive solicitude

    assert(countchannel > 0 && "count of channel argument abnormal");
    assert(countcamera  > 0 && "count of camera  argument abnormal");

    countchannel = (countchannel < 1) ? 1 : countchannel;
    countcamera  = (countcamera <  1) ? 1 : countcamera;

    // frame-buffer manager instance

    _countchannel = countchannel;
    _countcamera  = countcamera;

    _container.clear();
    _container.resize(countchannel, NULL);
    _spnRemove = new g2::critical_section[countchannel];

    _prepareDrive.resize(countchannel, false);
}

void frame_buffer_manager::cleanup(void)
{
    _nextchannel = 0;

    std::fill(_prepareDrive.begin(), _prepareDrive.end(), false);

    g2::scoped_criticalsection lock(_spnBuffer);

    frame_buffer* buff = NULL;
    for (int i = (int)_container.size() - 1; i >= 0; --i) {
        if (buff = _container[i]) {
            buff->cleanup();
            SAFE_DELETE(buff);
        }
    }

    std::fill(_container.begin(), _container.end(), (client::frame_buffer*)(NULL));
    SAFE_DELETE_ARR(_spnRemove);
}

//////////////////////////////////////////////////////////////////////////

void frame_buffer_manager::setup_buffer(short channel, client::frame_buffer::TYPE type, client::frame_buffer::PLAYTIME time)
{
    G2RETURN_IF_ASSERT(valid_channel(channel));

    if (type == frame_buffer::TYPE_UNDEFINED ||
        type == frame_buffer::TYPE_REMOVE) {
        remove(channel);
        return;
    }

    g2::scoped_criticalsection lock(_spnBuffer);
    frame_buffer*& buff = _container[channel];
    if (buff != NULL) {
        if (buff->is_type(type) != true) {
            remove(channel);
            assert(!"remained frame buffer");
        }
        return;
    }

    switch (type) {
        case frame_buffer::TYPE_SEARCH_G2:
            buff = new frame_buffer_search_g2(type, frame_buffer::SIZE_SEARCH_G2);
            break;
        default:
            assert(!"undefined frame buffer type");
            break;
    }

    if (buff) {
        buff->set_channel(channel);
    }
}

void frame_buffer_manager::remove(short channel)
{
    G2RETURN_IF_ASSERT(valid_channel(channel));

    g2::scoped_criticalsection lock1(_spnBuffer);
    g2::scoped_criticalsection lock2(_spnRemove[channel]);
    if (frame_buffer*& buff = _container[channel]) {
        SAFE_DELETE(buff);
    }
}

void frame_buffer_manager::remove(void)
{
    g2::scoped_criticalsection lock1(_spnBuffer);

    for (int i = (int)_container.size() - 1; i >= 0; --i) {
        g2::scoped_criticalsection lock2(_spnRemove[i]);
        if (frame_buffer*& buff = _container[i]) {
            SAFE_DELETE(buff);
        }
    }
}

//////////////////////////////////////////////////////////////////////////

void frame_buffer_manager::init(short channel)
{
    G2RETURN_IF_FAIL(valid_channel(channel));

    g2::scoped_criticalsection lock(_spnBuffer);
    if (frame_buffer* buff = _container[channel]) {
        buff->initialize();
    }
}

void frame_buffer_manager::init(short channel, int channelext)
{
    G2RETURN_IF_FAIL(valid_channel(channel));

    g2::scoped_criticalsection lock(_spnBuffer);
    if (frame_buffer* buff = _container[channel]) {
        buff->initialize(channelext);
    }
}

void frame_buffer_manager::clear_buffer(short channel, int hostcamera)
{
    G2RETURN_IF_FAIL(valid_channel(channel));

    g2::scoped_criticalsection lock(_spnBuffer);
    if (frame_buffer* buff = _container[channel]) {
        buff->clear(hostcamera);
    }
}

void frame_buffer_manager::clear_buffer(short channel, const g2::channels& channelext)
{
    G2RETURN_IF_FAIL(valid_channel(channel));

    g2::scoped_criticalsection lock(_spnBuffer);
    if (frame_buffer* buff = _container[channel]) {
        buff->clear(channelext);
    }
}

void frame_buffer_manager::clear_buffer(short channel, const std::set<int>& channelexts)
{
    G2RETURN_IF_FAIL(valid_channel(channel));

    g2::scoped_criticalsection lock(_spnBuffer);
    if (frame_buffer* buff = _container[channel]) {
        buff->clear(channelexts);
    }
}

void frame_buffer_manager::clear_buffer(short channel)
{
    G2RETURN_IF_FAIL(valid_channel(channel));

    g2::scoped_criticalsection lock(_spnBuffer);
    if (frame_buffer* buff = _container[channel]) {
        buff->clear();
    }
}

void frame_buffer_manager::exit_buffer(short channel)
{
    G2RETURN_IF_FAIL(valid_channel(channel));

    g2::scoped_criticalsection lock(_spnBuffer);
    if (frame_buffer* buff = _container[channel]) {
        buff->cleanup();
    }
}

//////////////////////////////////////////////////////////////////////////

bool frame_buffer_manager::put_frame(const G2FRAME& frame, short channel, short basecamera, unsigned __int64 refcameras)
{
    G2RETURN_VAL_IF_FAIL(valid_buffers(channel), false);
    G2RETURN_VAL_IF_FAIL(is_prepare_drive(channel) != true, false);
    G2RETURN_VAL_IF_FAIL(frame._index._data_size < screen::frame_max_::SIZE, false);

    bool result = false;
    const unsigned char typeFrame = frame._index._type;

    if (typeFrame != G2FRAME::AUDIO) {
        // video frame
        const G2FRAME::FROM from = G2FRAME::FROM(frame._index._from);
        if (from == G2FRAME::FROM_SEARCH_G2) {
            // because search(play) buffer waiting (lock full mutex) when full state,
            // manager spin lock should not be used.
            if (frame_buffer* buff = _container[channel]) {
                buff->push(frame, channel, basecamera, refcameras);
                result = true;
            }
        }
    }
    else {
        // audio frame
        // not yet
    }

    return result;
}

int frame_buffer_manager::get_frame(frame_element& element)
{
    int retcamera = -1;

    g2::scoped_criticalsection lock(_spnBuffer);

    frame_buffer* buff = NULL;

    if (_nextchannel < 0 || _nextchannel >= _countchannel) {
        _nextchannel = 0;
    }

    for (int i = _countchannel; i != 0; --i) {
        if ((buff = _container[_nextchannel]) != NULL &&
            (buff->is_unlocked() && buff->pop(element))) {
            retcamera = element.camera();
            // next_channel don't update if input frame that is none-display
            // up to input display frame.
            if (element.is_display()) {
                _nextchannel = (_nextchannel + 1) % _countchannel;
            }
            break;
        }

        // check(increase) next_channel if buffer is locked or empty.
        _nextchannel = (_nextchannel + 1) % _countchannel;
    }

    return retcamera;
}

//////////////////////////////////////////////////////////////////////////

void frame_buffer_manager::set_prepare_drive(short channel, bool prepare /*= true*/)
{
    G2RETURN_IF_FAIL(valid_channel(channel));

    _prepareDrive[channel] = prepare;
}

void frame_buffer_manager::set_ignore_audio(bool ignore)
{
    _audioIgnore = ignore;
}

void frame_buffer_manager::set_play_speed(short channel, int speed)
{
    G2RETURN_IF_FAIL(client::valid_channel(channel));

    g2::scoped_criticalsection lock(_spnBuffer);
    if (frame_buffer* buff = _container[channel]) {
        buff->set_play_speed(speed);
    }
}

int frame_buffer_manager::play_speed(short channel)
{
    G2RETURN_VAL_IF_FAIL(client::valid_channel(channel), 0);

    int speed = screen::play_speed_::STOP;

    g2::scoped_criticalsection lock(_spnBuffer);
    if (frame_buffer* buff = _container[channel]) {
        speed = buff->play_speed();
    }
    return speed;
}

void frame_buffer_manager::on_screen_changed_camera(short camera)
{
    _selcamera = camera;
}

int frame_buffer_manager::count_frame(short channel) const
{
    G2RETURN_VAL_IF_FAIL(valid_channel(channel), 0);

    int count = 0;
    g2::scoped_criticalsection lock(_spnBuffer);
    if (const frame_buffer* buff = _container[channel]) {
        count += buff->count();
    }
    return count;
}

int frame_buffer_manager::count_frame(short channel, int hostcamera) const
{
    G2RETURN_VAL_IF_FAIL(valid_channel(channel), 0);

    int count = 0;
    g2::scoped_criticalsection lock(_spnBuffer);
    if (const frame_buffer* buff = _container[channel]) {
        count += buff->count(hostcamera);
    }
    return count;
}
