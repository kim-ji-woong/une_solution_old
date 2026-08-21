// frame_buffer_search.h : header file
//

#ifndef _SCREEN_FRAME_BUFFER_SEARCH_H_
#define _SCREEN_FRAME_BUFFER_SEARCH_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "frame_buffer.h"
#include "buffer_queue_play.h"
#include <boost/unordered_map.hpp>

namespace client {

//////////////////////////////////////////////////////////////////////////

class frame_buffer_search : public frame_buffer
{
public:
    frame_buffer_search(frame_buffer::TYPE type, int limit, frame_buffer::PLAYTIME timeType);
    virtual ~frame_buffer_search(void);

public:
    virtual void initialize(void);
    virtual void initialize(int hostcamera);
    virtual void cleanup(void);

    virtual void clear(int hostcamera);
    virtual void clear(const g2::channels& hostcameras);
    virtual void clear(const std::set<int>& channelexts);
    virtual void clear(void);

    virtual void remove_ref_camera(short scrcamera);
    virtual void append_ref_camera(int hostcamera, short scrcamera);

    virtual void push(const G2FRAME& frame, short channel, short basecamera, unsigned __int64 refcameras);
    virtual bool pop(frame_element& element);
    virtual bool top(frame_element& element);

    virtual void set_play_speed(int speed);
    virtual void changed_layout(int layout, int count, int changed);

    virtual bool is_cleared(int channelext) const;
    virtual bool is_locked(void)  const { return _locked; }
    virtual int play_speed(void) const { return _speed; }

    virtual int count(void) const;
    virtual int count(int channelext) const;

public:
    unsigned int time_id(void) const { return _timerId; }
    void timer_func(void);

protected:
    void begin_timer(unsigned int interval);
    bool is_ignore_speed(void) const { return (_speedcheck == false); }

    static void CALLBACK timer_proc(UINT id,
                                    UINT msg,
                                    DWORD_PTR user,
                                    DWORD_PTR param1,
                                    DWORD_PTR param2);

protected:
    class internal_data
    {
    private:
        struct node {
            bool _cleared;
            bool _discard;

            node(void)
                : _cleared(true)
                , _discard(false) {
            }
            void reset(void) {
                _cleared = true;
                _discard = false;
            }
        };

        typedef boost::unordered_map<int, node> node_type;
        node_type _node;

    public:
        internal_data(void) {
        }
        void reset(void) {
            for (node_type::iterator itr(_node.begin());
                 itr != _node.end();
                 ++itr) {
                itr->second.reset();
            }
        }
        void reset(int channelext) {
            node_type::iterator itr(_node.find(channelext));
            if (itr != _node.end()) {
                itr->second.reset();
            }
        }
        void clear_buffer(int channelext, bool clear = true) {
            _node[channelext]._cleared = clear;
        }
        void discard(int channelext, bool skip = true) {
            _node[channelext]._discard = skip;
        }
        bool is_cleared(int channelext) const {
            node_type::const_iterator itr(_node.find(channelext));
            return (itr != _node.end()) ? itr->second._cleared : true;
        }
        bool is_discard(int channelext) const {
            node_type::const_iterator itr(_node.find(channelext));
            return (itr != _node.end()) ? itr->second._discard : false;
        }
    };

protected:
    internal_data _internal;
    buffer_queue_play _buffer;

    g2::critical_section _lock;
    performance_counter _perfcounter;

    bool _speedcheck;
    bool _audioplay;
    int _speed;
    int _prevSleep;

    __int64 _prevImageTick;
    __int64 _currImageTick;
    G2TIME  _prevImageTime;
    G2TIME  _currImageTime;

    unsigned int _timerId;

    frame_buffer::PLAYTIME _timeType;

    volatile bool _inited;
    volatile bool _locked;
};

//////////////////////////////////////////////////////////////////////////

} // namespace_client

#endif // !_SCREEN_FRAME_BUFFER_SEARCH_H_
