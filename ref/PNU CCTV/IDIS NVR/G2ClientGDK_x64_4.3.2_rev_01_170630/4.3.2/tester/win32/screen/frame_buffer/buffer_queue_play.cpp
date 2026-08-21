// buffer_queue_play.cpp : implementation file
//

#include "stdafx.h"
#include "buffer_queue_play.h"
#include "frame_element.h"

#include <boost/bind.hpp>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

buffer_queue_play::buffer_queue_play(int limit)
    : _count(0)
    , _limit(limit + 1)
    , _limit_c(limit + 1)
    , _blocked(false)
{
    _evtFullBuff = ::CreateEvent(NULL, FALSE, FALSE, NULL);
}

buffer_queue_play::~buffer_queue_play(void)
{
    g2::scoped_criticalsection lock(_lock);

    if (_evtFullBuff) {
        ::CloseHandle(_evtFullBuff);
        _evtFullBuff = NULL;
    }

    buffer_list().swap(_list);
    _counter.clear();
}

//////////////////////////////////////////////////////////////////////////

void buffer_queue_play::lockFullMutex(unsigned int msec /*= 1000*/)
{
    _blocked = true;

    if (_evtFullBuff) {
        ::WaitForSingleObject(_evtFullBuff, msec);
    }
}

int buffer_queue_play::count(void) const
{
    g2::scoped_criticalsection lock(_lock_counter);

    return _count;
}

int buffer_queue_play::count(int channelext) const
{
    int count = 0;
    CounterMap::const_iterator itr = _counter.find(channelext);
    if (itr != _counter.end()) {
        count = itr->second;
    }
    return count;
}

void buffer_queue_play::clear(void)
{
    g2::scoped_criticalsection lock(_lock);

    buffer_list().swap(_list);

    g2::scoped_criticalsection lockcounter(_lock_counter);
    _count = 0;
    _counter.clear();
    lockcounter.free();

    check_full_release();
}

void buffer_queue_play::clear(int channelext)
{
    g2::scoped_criticalsection lock(_lock);

    _list.erase(std::remove_if(_list.begin(), _list.end(),
                boost::bind(std::equal_to<int>(),
                boost::bind(&frame_element::channel_ext, _1), channelext)), _list.end());

    g2::scoped_criticalsection lockcounter(_lock_counter);

    _count = static_cast<int>(_list.size());

    CounterMap::iterator itr = _counter.find(channelext);
    if (itr != _counter.end()) {
        _counter[channelext] = 0;
    }

    lockcounter.free();
}

void buffer_queue_play::clear_ref(short scrcamera)
{
    g2::scoped_criticalsection lock(_lock);

    std::for_each(_list.begin(), _list.end(),
                  boost::bind(&frame_element::remove_ref_camera, _1, scrcamera));
}

void buffer_queue_play::append_ref(int channelext, short scrcamera)
{
    g2::scoped_criticalsection lock(_lock);

    std::for_each(_list.begin(), _list.end(),
                  boost::bind(&frame_element::append_ref_camera, _1, channelext, scrcamera));
}

void buffer_queue_play::changed_layout(int layout, int count, int changed)
{
    g2::scoped_criticalsection lock(_lock);

    if (layout == client::screen_formatter_::LAYOUT_1x1) {
        _limit = (_limit_c , 30) ? _limit_c : 30;
    }
    else {
        _limit = _limit_c;
    }
}

//////////////////////////////////////////////////////////////////////////

bool buffer_queue_play::pop(frame_element& element)
{
    check_full_release(); // check buffer full lock

    g2::scoped_criticalsection lock(_lock);

    if (is_empty()) return false;

    screen::frame_element_ptr pelement = _list.front(); _list.pop_front();
    if (pelement.get() == NULL) return false;

    ///////////////////////////////////

    element.attach(*pelement);

    g2::scoped_criticalsection lockcounter(_lock_counter);

    if ((--_count) < 0) {
        assert(!"occur abnormal count");
        _count = 0;
    }

    const int channelext = element.channel_ext();
    if (channelext >= 0) {
        CounterMap::iterator itr(_counter.find(channelext));
        if (itr != _counter.end())
        if (--(itr->second) < 0) {
            assert(!"occur abnormal count");
            itr->second = 0;
        }
    }

    return true;
}

bool buffer_queue_play::top(frame_element& element)
{
    g2::scoped_criticalsection lock(_lock);

    if (is_empty()) return false;

    const screen::frame_element_ptr& pelement = _list.front();
    if (pelement.get() == NULL) return false;

    ///////////////////////////////////

    element.attach(*pelement);

    return true;
}

bool buffer_queue_play::push(const G2FRAME& frame, short channel, short camera, unsigned __int64 refcamera, bool discard /*= false*/)
{
    g2::scoped_criticalsection lock(_lock);

    if (is_full()) return false;

    frame_element* element = new frame_element(frame, channel, camera, refcamera, discard);
    if (element == NULL) {
        assert(!"failed to allocate memory");
        return false;
    }

    _list.push_back(screen::frame_element_ptr(element));

    g2::scoped_criticalsection lockcounter(_lock_counter);

    _count++;

    const int channelext = element->channel_ext();
    if (channelext >= 0) {
        CounterMap::iterator itr(_counter.find(channelext));
        if (itr == _counter.end()) {
            _counter[channelext] = 1;
        }
        else {
            ++(itr->second);
        }
    }

    return true;
}

//////////////////////////////////////////////////////////////////////////

bool buffer_queue_play::is_block(void) const
{
    return _blocked;
}

void buffer_queue_play::check_full_release(void)
{
    if (is_block() &&
        count() < (_limit >> 1)) {
        _blocked = false;
        ::SetEvent(_evtFullBuff);
    }
}
