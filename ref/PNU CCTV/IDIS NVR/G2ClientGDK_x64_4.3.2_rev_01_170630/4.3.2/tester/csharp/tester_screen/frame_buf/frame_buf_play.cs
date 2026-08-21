using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using GDK;

namespace GDK_tester
{
    public class frame_buf_play : frame_buf
    {
        public class internal_data
        {
            public class node
            {
                public node() { reset(); }
                public void reset()
                {
                    cleared = true;
                    discard = false;
                }
                public bool cleared;
                public bool discard;
            }

            public internal_data()
            {
                _container = new Dictionary<int, node>();
            }
            public void reset()
            {
                lock (_container)
                {
                    foreach (KeyValuePair<int, node> pair in _container)
                    {
                        pair.Value.reset();
                    }
                }
            }
            public void reset(int channelext)
            {
                node n = get_node(channelext);
                if (n != null)
                {
                    n.reset();
                }
            }
            public void clear(int channelext)
            {
                clear(channelext, true);
            }
            public void clear(int channelext, bool clear)
            {
                if (_container.ContainsKey(channelext) != true)
                {
                    _container[channelext] = new node();
                }
                _container[channelext].cleared = clear;
            }
            public void discard(int channelext)
            {
                discard(channelext, true);
            }
            public void discard(int channelext, bool discard)
            {
                if (_container.ContainsKey(channelext) != true)
                {
                    _container[channelext] = new node();
                }
                _container[channelext].discard = discard;
            }
            public bool is_cleared(int channelext)
            {
                node n = get_node(channelext);
                return (n != null) ? n.cleared : true;
            }
            public bool is_discard(int channelext)
            {
                node n = get_node(channelext);
                return (n != null) ? n.discard : false;
            }

            protected node get_node(int channelext)
            {
                node n = null;
                lock (_container)
                {
                    _container.TryGetValue(channelext, out n);
                }
                return n;
            }

            private Dictionary<int, node> _container;
        }

        public frame_buf_play(TYPE type, int limit)
            : base(frame_buf.MODE.PLAY, type, limit)
        {
            _buf = new frame_buf_queue(limit);
            _internal = new internal_data();
            _perf = new perf_counter();
            _remove_request = false;
            _discard_push = false;
            _locked = false;
            _speed_check = false;
            _audio_play = false;
            _speed = 0;
            _sleep_pre = 0;
            _image_tick_pre = 0;
            _image_tick_cur = 0;
            _timer = new Timer();
            _timer.Elapsed += new ElapsedEventHandler(on_timer);
        }

        public override void set_speed(int speed)
        {
            _speed = speed;
            _speed_check = (speed != 0 &&
                           (speed != int.MaxValue && speed != int.MinValue));
            _sleep_pre = 0;
            _image_tick_pre = 0;
            _perf.reset_begin();
        }
        public override void set_channelset(g2channel_set channelset_interest, g2channel_set channelset)
        {
            base.set_channelset(channelset_interest, channelset);
            _buf.set_channelset(channelset_interest, channelset);
        }
        public override void set_audio_play(bool play)
        {
            _audio_play = play;
        }

        public override void stop_enter()
        {
            _discard_push = true;
        }
        public override void stop_leave()
        {
            _discard_push = false;
        }

        public override void push(ref G2FRAME frame, int channel, int pane)
        {
            int channelext = frame.channel;
            bool discard = false;

            if (frame.type != G2FRAME.TYPE.AUDIO)
            {
                if (_internal.is_cleared(channelext))
                {
                    if (frame.type == G2FRAME.TYPE.I_FRAME)
                    {
                        _internal.clear(channelext, false);
                    }
                    else
                    {
                        return;
                    }
                }

                discard = frame.is_broken;
                if (_internal.is_discard(channelext))
                {
                    if (frame.type == G2FRAME.TYPE.I_FRAME &&
                        discard != true)
                    {
                        _internal.discard(channelext, false);
                    }
                    else
                    {
                        discard = true;
                    }
                }
                else
                {
                    if (discard)
                    {
                        _internal.discard(channelext, true);
                    }
                }
            }

            if (_remove_request)
            {
                _remove_request = false;
            }

            frame_element element = new frame_element(ref frame, channel, pane);

            while (_buf.push(element, discard) != true)
            {
                // wait 100 msec if there is not efficient buf space
                _buf.lock_full(100);

                // current this frame not use,
                // because buf is cleared but status is init
                if (_remove_request)
                {
                    _remove_request = false;
                    break;
                }
                if (_discard_push)
                {
                    break;
                }
            }
        }
        public override bool pop(out frame_element element)
        {
            element = null;

            if (_buf.pop(out element) != true)
            {
                return false;
            }
            if (element.pane == (int)frame_element.LOADED.INVALID)
            {
                return false;
            }
            if (element.pane == (int)frame_element.LOADED.END_OF)
            {
                return true;
            }
            if (element.type == G2FRAME.TYPE.AUDIO)
            {
                return true;
            }

            int channelext = element.channelext;
            if (channelext < 0 ||
                _internal.is_cleared(channelext))
            {
                return false;
            }

            if (_speed_check != true)
            {
                return true;
            }
            if (element.frame._extra._display != true)
            {
                if (_sleep_pre < 0)
                {
                    _sleep_pre = 0;
                }
                return true;
            }

            if (_perf.begin == 0)
            {
                _perf.do_begin();
            }

            if (_image_tick_pre == 0)
            {
                _image_tick_pre = element.frame._extra._pts;
            }

            _image_tick_cur = element.frame._extra._pts;
            _perf.do_end();

            long dif_timer = _perf.result();
            long dif_image = _image_tick_cur - _image_tick_pre;

            if (_audio_play)
            {
                _image_tick_pre = _image_tick_cur;

                long audio_tick = _image_tick_cur;

                if (_listener != null)
                {
                    G2SPOT spot;
                    int msec = 0;
                    long pts = 0;
                    _listener.on_frame_buf_sound_stream_get_current_pos(out spot, out msec, out pts);

                    audio_tick = pts + msec;
                }

                int pred = (int)(_image_tick_cur - audio_tick);
                if (pred > 1000)
                {
                    pred = 1000;
                }
                else if (pred < 0)
                {
                    pred = 0;
                }

                element.sleep = pred;

                return true;
            }

            int speed = _speed;

            if (speed != int.MinValue &&
                speed != int.MaxValue)
            {
                dif_image = (long)(((double)dif_image * 10.0) / speed);

                int pred = (int)((dif_image - dif_timer) + _sleep_pre);

                if (pred < -1000)
                {
                    pred = 0;
                }
                else if (pred > 0)
                {
                    if (pred > 1000)
                    {
                        pred = 1000;
                    }
                    set_timer(pred);
                }

                _sleep_pre = pred;
            }
            else
            {
                _sleep_pre = 0;
            }

            _image_tick_pre = _image_tick_cur;
            _perf.equal_end();

            return true;
        }
        public override bool top(out frame_element element)
        {
            return _buf.top(out element);
        }
        public override void clear(int channelext, int stream_id)
        {
            _internal.clear(channelext, true);
        }
        public override void clear(G2CHANNEL_STREAM cs)
        {
            _internal.clear(cs._channel, true);
        }
        public override void clear(g2channel_stream_set chs)
        {
            foreach (G2CHANNEL_STREAM cs in chs)
            {
                _internal.clear(cs._channel, true);
            }
        }
        public override void clear()
        {
            _internal.reset();
        }
        public override void remove()
        {
            if (_remove_request != true)
            {
                _remove_request = true;
                _buf.clear();
                _perf.reset();
                _locked = false;
                _sleep_pre = 0;
                _image_tick_pre = 0;
                _timer.Stop();

                clear();
            }
        }
        public override void remove(int channelext, int stream_id)
        {
            _buf.clear(channelext);

            clear(channelext, stream_id);
        }
        public override int count()
        {
            return _buf.count();
        }
        public override int speed()
        {
            return _speed;
        }
        public override bool is_signaled()
        {
            return (_locked != true);
        }

        public void set_timer(int interval)
        {
            _locked = true;
            _timer.Stop();
            _timer.Interval = interval;
            _timer.Start();
        }

        protected void on_timer(object sender, ElapsedEventArgs e)
        {
            _timer.Stop();
            _locked = false;
        }

        protected frame_buf_queue _buf;
        protected internal_data _internal;
        protected perf_counter _perf;
        protected bool _remove_request;
        protected bool _discard_push;
        protected bool _locked;
        protected bool _speed_check;
        protected bool _audio_play;
        protected int _speed;
        protected int _sleep_pre;
        protected long _image_tick_pre;
        protected long _image_tick_cur;
        protected Timer _timer;
    }
}
