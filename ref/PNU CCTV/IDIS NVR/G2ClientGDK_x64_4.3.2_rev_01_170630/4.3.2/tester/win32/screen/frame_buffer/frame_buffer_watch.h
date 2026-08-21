// frame_buffer_watch.h : header file
//

#ifndef _SCREEN_FRAME_BUFFER_WATCH_H_
#define _SCREEN_FRAME_BUFFER_WATCH_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "frame_buffer.h"
#include <list>
#include <map>
#include <hash_map>
#include <boost/shared_ptr.hpp>
#include <boost/unordered_map.hpp>

namespace client {

//////////////////////////////////////////////////////////////////////////

class frame_buffer_watch : public frame_buffer
{
public:
    frame_buffer_watch(TYPE type, int limit = frame_buffer::SIZE_WATCH);
    virtual ~frame_buffer_watch(void);

public:
    virtual void initialize(void);
    virtual void initialize(int channelext);
    virtual void cleanup(void);

    virtual void clear(int channelext);
    virtual void clear(const g2::channels& channelext);
    virtual void clear(const std::set<int>& channelexts);
    virtual void clear(void);

    virtual void remove_ref_camera(short scrcamera);
    virtual void append_ref_camera(int channelext, short scrcamera);

    virtual void push(const G2FRAME& frame, short channel, short basecamera, unsigned __int64 refcameras);
    virtual bool pop(frame_element& element);
    virtual bool top(frame_element& element);

    virtual void changed_layout(int layout, int count, int changed);
    virtual bool is_cleared(int channelext) const;

    virtual int count(void) const;
    virtual int count(int channelext) const;

protected:
    void clear_impl(short stream);
    void erase_impl(int channelext);
    short stream_of_camera(int channelext) const;

private:
    class internal_data
    {
    public:
        struct node {
            short _ciframes;
            short _celement;
            int   _cjump;
            int   _streaming;
            bool  _cleared;
            bool  _discard;
            bool  _reserved;

            struct element_sync {
                volatile bool _locked;
                bool _enable;
                int  _count; // cout of buffering frames
                int  _upperBound;
                int  _lowerBound;
                int  _prevSleep;
                __int64 _prevImageTick;
                __int64 _currImageTick;

                element_sync(void)
                    : _locked(false)
                    , _enable(true)
                    , _count(0)
                    , _upperBound(0)
                    , _lowerBound(0)
                    , _prevSleep(0)
                    , _prevImageTick(0i64)
                    , _currImageTick(0i64) {
                }
                void reset(void) {
                    _locked = false;
                    _enable = true;
                    _count = 0;
                    _upperBound = 0;
                    _lowerBound = 0;
                    reset_time();
                }
                void reset_time(void) {
                    _prevSleep = 0;
                    _prevImageTick =
                    _currImageTick = 0i64;
                }
            }
            _sync;

            node(void)
                : _ciframes(0)
                , _celement(0)
                , _cjump(0)
                , _streaming(screen::stream_buf_::OFF)
                , _cleared(true)
                , _discard(false)
                , _reserved(false) {

            }
            void reset(void) {
                _ciframes  = 0;
                _celement  = 0;
                _cjump     = 0;
                _streaming = screen::stream_buf_::OFF;
                _cleared   = true;
                _discard   = false;
                _reserved  = false;
                _sync.reset();
            }
            void reset_sync(void) {
                _sync.reset();
            }
            void reset_sync_time(void) {
                _sync.reset_time();
            }
        };

    private:
        std::vector<node> _node;
        size_t            _count;

    public:
        internal_data(void)
            : _count(0) {
        }

    public:
        void create(size_t countelement) {
            _node.resize(countelement);
            _count = _node.size();
        }
        void reset(void) {
            for (size_t i = 0; i < _count; ++i) {
                _node[i].reset();
            }
        }
        void reset(short stream) {
            _node[stream].reset();
        }
        short find_free(void) const {
            short slot = -1;
            for (size_t i = 0; i < _count; ++i) {
                if (_node[i]._reserved != true) {
                    slot = (short)i;
                    break;
                }
            }
            return slot;
        }
        void reserve(short stream, bool use) {
            _node[stream]._reserved = use;
        }

    public:
        const node& data(short stream) const { return _node[stream]; }
              node& data(short stream)       { return _node[stream]; }

        node::element_sync& synchronize(short stream) {
            return _node[stream]._sync;
        }
        void clear_buffer(short stream, bool clear = true) {
            if (_node[stream]._cleared = clear) {
                _node[stream]._cjump = 0;
            }
        }
        void discard(short stream, bool skip = true) {
            _node[stream]._discard = skip;
        }
        void streaming(short stream, int mode) {
            _node[stream]._streaming = mode;
        }
        int streaming(short stream) const {
            return _node[stream]._streaming;
        }
        bool is_streaming(short stream) const {
            return _node[stream]._streaming != screen::stream_buf_::OFF;
        }
        bool is_streaming(short stream, int mode) const {
            return _node[stream]._streaming == mode;
        }
        bool is_cleared(short stream) const {
            return _node[stream]._cleared;
        }
        bool is_discard(short stream) const {
            return _node[stream]._discard;
        }
        short count_iframes(short stream) const {
            return _node[stream]._ciframes;
        }
        short count_element(short stream) const {
            return _node[stream]._celement;
        }
        void count_iframes(short stream, short count) {
            _node[stream]._ciframes = count;
        }
        void count_element(short stream, short count) {
            _node[stream]._celement = count;
        }
        short count_jump(short stream) const {
            return _node[stream]._cjump;
        }
        void reset_iframes(short stream) {
            _node[stream]._ciframes = 0;
        }
        void reset_element(short stream) {
            _node[stream]._celement = 0;
        }
        void reset_sync_time(short stream) {
            _node[stream].reset_sync_time();
        }
        void reset_sync_time(void) {
            std::for_each(_node.begin(), _node.end(),
                          std::mem_fun_ref(&node::reset_sync_time));
        }
        void reset_sychronize(short stream) {
            _node[stream].reset_sync();
        }
        void reset_sychronize(void) {
            std::for_each(_node.begin(), _node.end(),
                          std::mem_fun_ref(&node::reset_sync));
        }
        void reset_clear(short stream) {
            _node[stream]._cleared = true;
        }
        void inc_iframes(short stream) {
            ++(_node[stream]._ciframes);
        }
        void dec_iframes(short stream) {
            --(_node[stream]._ciframes);
        }
        void inc_element(short stream) {
            ++(_node[stream]._celement);
        }
        void dec_element(short stream) {
            --(_node[stream]._celement);
        }
        void inc_jump(short stream) {
            ++(_node[stream]._cjump);
        }
        void dec_jump(short stream) {
            --(_node[stream]._cjump);
        }
        short total_element(void) const {
            short total = 0;
            for (size_t i = 0; i < _count; ++i) {
                total += _node[i]._celement;
            }
            return total;
        }
    };

    ///////////////////////////////////////////////////////

    typedef internal_data::node               internal_node;
    typedef internal_data::node::element_sync internal_sync;

    typedef std::list<screen::frame_element_ptr> buffer_list;
    typedef boost::shared_ptr<buffer_list>    buffer_list_ptr;
    typedef std::vector<buffer_list_ptr>      buffer_bunch;
    typedef stdext::hash_map<int, int>        buffer_count;
    typedef std::map<int, short>              stream_map;
    typedef std::vector<short>                stream_bunch;

    internal_data _internal;
    buffer_bunch  _buffer;
    buffer_count  _buffer_count;
    stream_map    _stream_map;
    stream_bunch  _streams;
    int           _next_stream;

protected:
    g2::critical_section _lock;
    g2::critical_section _lock_buffer_count;
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_SCREEN_FRAME_BUFFER_WATCH_H_
