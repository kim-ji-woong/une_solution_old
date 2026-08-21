using System;
using System.Collections.Generic;
using System.Text;
using GDK;

namespace GDK_tester
{
    using container_type = System.Collections.Generic.Dictionary<int, frame_buf>;

    public class frame_buf_manager
    {
        public frame_buf_manager()
        {
            _connections = 0;
            _select = 0;
        }

        public void create(int connections, frame_buf_listener listener)
        {
            _connections = connections;
            _container = new container_type(connections);
            _buf_audio = new frame_buf_play(frame_buf.TYPE.PLAY, frame_buf.BUF_SIZE.AUDIO);
            _listener = listener;
            _select = 0;
        }

        public bool setup(int channel, int streams, frame_buf.TYPE type)
        {
            if (type == frame_buf.TYPE.UNDEFINED)
            {
                return remove(channel);
            }

            frame_buf buf = null;
            if (type == frame_buf.TYPE.LIVE)
            {
                buf = new frame_buf_live(type, streams, frame_buf.BUF_SIZE.LIVE);
            }
            else if (type == frame_buf.TYPE.LIVE_SIMPLE)
            {
                buf = new frame_buf_live(type, streams, int.MaxValue);
            }
            else if (type == frame_buf.TYPE.PLAY)
            {
                buf = new frame_buf_play(type, frame_buf.BUF_SIZE.PLAY);
            }
            else if (type == frame_buf.TYPE.SEARCH_SAMBA)
            {
                buf = new frame_buf_play(type, frame_buf.BUF_SIZE.SEARCH_SAMBA);
            }
            else if (type == frame_buf.TYPE.SEARCH_IDR)
            {
                buf = new frame_buf_play(type, frame_buf.BUF_SIZE.SEARCH_IDR);
            }
            else if (type == frame_buf.TYPE.SEARCH)
            {
                buf = new frame_buf_play(type, frame_buf.BUF_SIZE.SEARCH);
            }

            buf.set_channel(channel);
            buf.set_listener(_listener);

            lock (_container)
            {
                _container[channel] = buf;
            }
            return true;
        }
        public bool remove(int channel)
        {
            frame_buf buf = get(channel);
            if (buf != null)
            {
                buf.remove();
            }

            lock (_container)
            {
                return _container.Remove(channel);
            }
        }
        public void clear(int channel)
        {
            frame_buf buf = get(channel);
            if (buf != null)
            {
                buf.clear();
            }
        }
        public void reset_audio()
        {
            _buf_audio.remove();
            _buf_audio.set_channel(-1);
        }
        public bool push(ref G2FRAME frame, int channel, int pane)
        {
            G2FRAME.TYPE type = frame.type;
            bool ret = false;
            if (type == G2FRAME.TYPE.AUDIO)
            {
                if (_buf_audio.speed() == 10)
                {
                    _buf_audio.set_channel(channel);
                    _buf_audio.push(ref frame, channel, pane);
                    ret = true;
                }
            }
            else
            {
                frame_buf buf = get(channel);
                if (buf != null)
                {
                    buf.push(ref frame, channel, pane);
                    ret = true;
                }
            }
            return ret;
        }
        public int pop(out frame_element element)
        {
            int pane = -1;
            element = null;

            int count = _container.Count;
            for (int i = 0; i < count; ++i)
            {
                frame_buf buf = get(_select);
                if (buf != null)
                {
                    if (buf.is_signaled())
                    {
                        if (buf.pop(out element))
                        {
                            pane = element.pane;
                            if (element.frame._extra._display)
                            {
                                _select = (_select + 1) % count;
                            }
                            break;
                        }
                    }
                }

                _select = (_select + 1) % count;
            }
            return pane;
        }
        public int pop_audio(out frame_element element)
        {
            int pane = -1;
            if (_buf_audio.pop(out element))
            {
                pane = element.pane;
            }
            return pane;
        }
        public int pop_audio()
        {
            frame_element element;
            return pop_audio(out element);
        }
        public int top_audio(out frame_element element)
        {
            int pane = -1;
            if (_buf_audio.top(out element))
            {
                pane = element.pane;
            }
            return pane;
        }

        public void set_speed(int channel, int speed)
        {
            frame_buf buf = get(channel);
            if (buf != null)
            {
                buf.set_speed(speed);
            }

            _buf_audio.set_speed(speed);
        }
        public void set_audio_play(bool play)
        {
            lock (_container)
            {
                foreach (KeyValuePair<int, frame_buf> pair in _container)
                {
                    pair.Value.set_audio_play(play);
                }
            }
        }
        public void set_pane_audio_enable(bool enable)
        {
            if (_listener != null)
            {
                _listener.on_frame_buf_set_pane_audio_enable(enable);
            }
        }
        public void set_pane_audio_enable(int pane, bool enable)
        {
            if (_listener != null)
            {
                _listener.on_frame_buf_set_pane_audio_enable(pane, enable);
            }
        }

        public frame_buf get(int channel)
        {
            frame_buf buf = null;
            if (valid_channel(channel))
            {
                lock (_container)
                {
                    _container.TryGetValue(channel, out buf);
                }
            }
            return buf;
        }
        public int get_audio_channel()
        {
            return _buf_audio.channel();
        }
        public int get_audio_count()
        {
            return _buf_audio.count();
        }
        public int get_frame_count(int channel)
        {
            frame_buf buf = get(channel);
            return (buf != null) ? buf.count() : 0;
        }

        public bool valid_channel(int channel)
        {
            return (channel >= 0 && channel < _connections);
        }

        protected container_type _container;
        protected frame_buf_play _buf_audio;
        protected frame_buf_listener _listener;
        protected int _connections;
        protected int _select;
    }
}
