// buffer_queue_play.h : header file
//

#ifndef _SCREEN_FRAME_BUFFER_QUEUE_PLAY_H_
#define _SCREEN_FRAME_BUFFER_QUEUE_PLAY_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <boost/noncopyable.hpp>
#include <boost/shared_ptr.hpp>
#include <hash_map>

namespace client {
    class frame_element;

//////////////////////////////////////////////////////////////////////////

class buffer_queue_play sealed : private boost::noncopyable
{
public:
    buffer_queue_play(int limit);
    ~buffer_queue_play(void);

private:
    typedef std::list<boost::shared_ptr<frame_element>> buffer_list;
    typedef stdext::hash_map<int, int> CounterMap;  // [ channelext : counotfFrame ]

protected:
    HANDLE _evtFullBuff;
    int _count;
    int _limit;
    const int _limit_c;
    volatile bool _blocked;

    buffer_list _list;
    CounterMap _counter;
    g2::critical_section _lock;
    g2::critical_section _lock_counter;

public:
    void lockFullMutex(unsigned int msec = 1000);

    int count(void) const;
    int count(int channelext) const;

    void clear(void);
    void clear(int channelext);
    void clear_ref(short scrcamera);
    void append_ref(int channelext, short scrcamera);
    void changed_layout(int layout, int count, int changed);

public:
    bool __stdcall pop(frame_element& element);
    bool __stdcall top(frame_element& element);
    bool __stdcall push(const G2FRAME& frame, short channel, short camera, unsigned __int64 refcamera, bool discard = false);

protected:
    bool is_empty(void) const { return _list.empty(); }
    bool is_full(void) const { return (_count >= _limit); }
    bool is_block(void) const;

    void check_full_release(void);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_SCREEN_FRAME_BUFFER_QUEUE_PLAY_H_
