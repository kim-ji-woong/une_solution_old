// frame_buffer_watch.cpp : implementation file
//

#include "stdafx.h"
#include "frame_buffer_watch.h"

#include <include/g2_define_decoder.h>
#include <boost/bind.hpp>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

namespace {
    template<class P>
    struct select1st : public std::unary_function<P, typename P::first_type> {
        const typename P::first_type& operator()(const P& pair) const {
            return pair.first;
        }
    };
}

//////////////////////////////////////////////////////////////////////////

frame_buffer_watch::frame_buffer_watch(TYPE type, int limit /*= frame_buffer::SIZE_LIVE*/)
    : frame_buffer(frame_buffer::MODE_LIVE, type, limit)
    , _next_stream(0)
{
    _buffer.resize(screen::count_::LIVE_STREAM);
    _internal.create(screen::count_::LIVE_STREAM);
}

frame_buffer_watch::~frame_buffer_watch(void)
{
    cleanup();
}

//////////////////////////////////////////////////////////////////////////

void frame_buffer_watch::initialize(void)
{
    g2::scoped_criticalsection lock(_lock);

    for (stream_map::const_iterator itr(_stream_map.begin());
         itr != _stream_map.end();
         ++itr) {
        clear_impl(itr->second);
    }
    _internal.reset();
    _stream_map.clear();
    _streams.clear();
    _buffer_count.clear();
}

void frame_buffer_watch::initialize(int channelext)
{
    g2::scoped_criticalsection lock(_lock);
    erase_impl(channelext);

    _buffer_count.insert(stdext::hash_map<int, int>::value_type(channelext, -1));
}

void frame_buffer_watch::cleanup(void)
{
    g2::scoped_criticalsection lock(_lock);

    clear();
}

void frame_buffer_watch::clear(int channelext)
{
    g2::scoped_criticalsection lock(_lock);

    erase_impl(channelext);
}

void frame_buffer_watch::clear(const g2::channels& channelext)
{
    g2::scoped_criticalsection lock(_lock);

    short stream = -1;
    for (int i = 0, limit = 32; i < limit; ++i) {
        if (channelext.contains(i) != true)
        if (valid_stream(stream = stream_of_camera(i))) {
            erase_impl(i);
        }
    }
}

void frame_buffer_watch::clear(const std::set<int>& channelexts)
{
    g2::scoped_criticalsection lock(_lock);

    short channelext = -1;
    std::vector<short> cameras;
    cameras.reserve(_stream_map.size());
    std::transform(_stream_map.begin(), _stream_map.end(), std::back_inserter(cameras),
                   select1st<std::iterator_traits<stream_map::iterator>::value_type>());

    for (int i = 0, count = (int)cameras.size(); i < count; ++i) {
        channelext = cameras[i];
        if (channelexts.find(channelext) == channelexts.end()) {
            erase_impl(channelext);
        }
    }
}

void frame_buffer_watch::clear(void)
{
    g2::scoped_criticalsection lock(_lock);

    for (stream_map::const_iterator itr = _stream_map.begin();
         itr != _stream_map.end();
         ++itr) {
        clear_impl(itr->second);
    }
    _stream_map.clear();
    _streams.clear();
    _internal.reset();
}

void frame_buffer_watch::remove_ref_camera(short scrcamera)
{
    G2RETURN_IF_FAIL(scrcamera >= 0);

    g2::scoped_criticalsection lock(_lock);

    short stream = -1;
    buffer_list* buffer = NULL;

    for (stream_map::const_iterator itr = _stream_map.begin();
         itr != _stream_map.end();
         ++itr) {
        if (valid_stream(stream = itr->second))
        if (buffer = _buffer[stream].get()) {
            std::for_each(buffer->begin(), buffer->end(),
                          boost::bind(&frame_element::remove_ref_camera, _1, scrcamera));
        }
    }
}

void frame_buffer_watch::append_ref_camera(int channelext, short scrcamera)
{
    g2::scoped_criticalsection lock(_lock);

    short stream = stream_of_camera(channelext);
    G2RETURN_IF_FAIL(scrcamera >= 0);
    G2RETURN_IF_FAIL(valid_stream(stream));

    if (buffer_list* buffer = _buffer[stream].get()) {
        std::for_each(buffer->begin(), buffer->end(),
        boost::bind(&frame_element::append_ref_camera, _1, scrcamera));
    }
}

//////////////////////////////////////////////////////////////////////////

void frame_buffer_watch::push(const G2FRAME& frame, short channel, short basecamera, unsigned __int64 refcameras)
{
    const int channelext = frame._index._channel;

    // check frame data size
    G2RETURN_IF_ASSERT(frame._index._data_size < screen::frame_max_::SIZE);

    g2::scoped_criticalsection lock(_lock);

    short stream = -1;
    stream_map::const_iterator itr = _stream_map.find(channelext);
    if (itr == _stream_map.end()) {
        G2RETURN_IF_ASSERT(valid_stream(stream = _internal.find_free()));
        _stream_map[channelext] = stream;
        _buffer[stream] = buffer_list_ptr(new buffer_list);
        _internal.reset(stream);
        _internal.reserve(stream, true);
        _streams.push_back(stream);
        if (_streams.size() != _stream_map.size()) {
            _streams.clear();
            _streams.reserve(_buffer.size());

            std::transform(_stream_map.begin(), _stream_map.end(), std::back_inserter(_streams),
                           select1st<std::iterator_traits<stream_map::iterator>::value_type>());

            assert(!"abnormal status of stream map");
        }
    }
    else {
        stream = itr->second;
    }

    ///////////////////////////////////////////////////////

    if (_internal.is_cleared(stream)) {
        if (frame._index._type == G2FRAME::I_FRAME) {
            _internal.clear_buffer(stream, false);
        }
        else {
            return;
        }
    }

    bool discard = ((frame._index._flag & (G2FRAME::BROKEN_DATA_HEADER | G2FRAME::BROKEN_DATA_BODY)) != 0);
    if (_internal.is_discard(stream)) {
        if (frame._index._type == G2FRAME::I_FRAME &&
            discard != true) {
            _internal.discard(stream, false);
        }
        else {
            discard = true;
        }
    }
    else {
        if (discard) {
            _internal.discard(stream, true);
        }
    }

    frame_element* element = new frame_element(frame, channel, basecamera, refcameras, discard);

    /////////////////////////////////////////////

    if (frame._index._type == G2FRAME::I_FRAME) {
        if (frame._extra._decoder == G2DECODER_CODEC::MJPEG) {
            if (_internal.count_element(stream) > 1) {
                _internal.dec_element(stream);
                if (buffer_list* buffer = _buffer[stream].get()) {
                    buffer->pop_front();
                }
            }
        }
        else {
            if (_internal.count_element(stream) > capacity()) {
                clear_impl(stream);
            }

            // camera-buffer initialize for real-time live monitoring,
            // if inputed new i-frame at buffer that has i-frame
            if (_internal.count_iframes(stream) > 0) {
                _internal.inc_jump(stream);
                clear_impl(stream);
            }
        }
        _internal.inc_iframes(stream);
    }

    _buffer[stream]->push_back(screen::frame_element_ptr(element));
    _internal.inc_element(stream);

    if (_internal.count_element(stream) >= _limit) {
        _internal.reset_element(stream);
        _internal.clear_buffer(stream, true);
        clear_impl(stream);
        TRACE("live >> camera[%d] buffer cleared by limited capacity.\n", basecamera);
    }
}

bool frame_buffer_watch::pop(frame_element& element)
{
    g2::scoped_criticalsection lock(_lock);

    bool success = false;
    short stream = -1;
    screen::frame_element_ptr pelement;
    buffer_list* buffer = NULL;

    int count = (int)_streams.size();

    for (int i = count; i != 0; --i) {
        _next_stream = (_next_stream + 1) % count;
        stream = _streams[_next_stream];

        if ((buffer = _buffer[stream].get()) == NULL ||
            (buffer->empty())) {
            continue;
        }

        pelement = buffer->front(); buffer->pop_front();
        if (pelement) {
            element.attach(*pelement);
            assert(stream_of_camera(element.channel_ext()) == stream);

            // i-frame recount
            if (element.type() == G2FRAME::I_FRAME) {
                _internal.dec_iframes(stream);
            }
            _internal.dec_element(stream);

            success = true;
            break;
        }
    }

    return success;
}

bool frame_buffer_watch::top(frame_element& element)
{
    assert(!"not support");
    return false;
}

//////////////////////////////////////////////////////////////////////////

void frame_buffer_watch::changed_layout(int layout, int count, int changed)
{
    g2::scoped_criticalsection lock(_lock);

    _capacity = 32;
    _internal.reset_sync_time();
}

bool frame_buffer_watch::is_cleared(int channelext) const
{
    g2::scoped_criticalsection lock(_lock);
    const short stream = stream_of_camera(channelext);
    return (valid_stream(stream)) ? _internal.is_cleared(stream) : true;
}

int frame_buffer_watch::count(void) const
{
    return _internal.total_element();
}

int frame_buffer_watch::count(int channelext) const
{
    g2::scoped_criticalsection lock(_lock);
    const short stream = stream_of_camera(channelext);
    return (valid_stream(stream)) ? _internal.count_element(stream) : 0;
}

//////////////////////////////////////////////////////////////////////////

void frame_buffer_watch::clear_impl(short stream)
{
    G2RETURN_IF_FAIL(valid_stream(stream));

    if (buffer_list* buffer = _buffer[stream].get()) {
        buffer->clear();
    }

    _internal.reset_iframes(stream);
    _internal.reset_element(stream);
}

void frame_buffer_watch::erase_impl(int channelext)
{
    short stream = stream_of_camera(channelext);
    G2RETURN_IF_FAIL(valid_stream(stream));

    if (buffer_list* buffer = _buffer[stream].get()) {
        buffer->clear();
    }

    _stream_map.erase(channelext);
    _internal.reset(stream);
    _buffer[stream].reset();
    _streams.erase(std::remove(_streams.begin(), _streams.end(), stream));
}

short frame_buffer_watch::stream_of_camera(int channelext) const
{
    stream_map::const_iterator itr(_stream_map.find(channelext));
    return (itr != _stream_map.end()) ? itr->second : -1;
}
