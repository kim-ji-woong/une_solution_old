using System;
using System.Collections.Generic;
using System.Text;
using GDK;

namespace GDK_tester
{
    public interface frame_buf_listener
    {
        void on_frame_buf_set_pane_audio_enable(bool enable);
        void on_frame_buf_set_pane_audio_enable(int pane, bool enable);
        void on_frame_buf_sound_stream_get_current_pos(out G2SPOT spot, out int play_msec, out long pts);
    }

    public abstract class frame_buf
    {
        public enum MODE
        {
            UNDEFINED = 0,
            LIVE = 1,
            PLAY = 2
        }
        public enum TYPE
        {
            UNDEFINED = -1,
            LIVE = 0,
            LIVE_SIMPLE,
            PLAY,
            SEARCH,
            SEARCH_SAMBA,
            SEARCH_IDR
        }

        public struct BUF_SIZE
        {
            public const int LIVE = 240;
            public const int PLAY = 480;
            public const int SEARCH_SAMBA = 120;
            public const int SEARCH_IDR = 120;
            public const int SEARCH = 32;
            public const int AUDIO = 64;
        }

        public frame_buf(MODE mode, TYPE type, int limit)
        {
            _mode = mode;
            _type = type;
            _limit = limit;
            _channel = -1;
        }

        public void set_channel(int channel) { _channel = channel; }
        public void set_listener(frame_buf_listener listener) { _listener = listener; }
        public virtual void set_speed(int speed) { }
        public virtual void set_channelset(g2channel_set channelset_interest, g2channel_set channelset) { }
        public virtual void set_audio_play(bool play) { }
        public virtual void stop_enter() { }
        public virtual void stop_leave() { }
        public abstract void push(ref G2FRAME frame, int channel, int pane);
        public abstract bool pop(out frame_element element);
        public abstract bool top(out frame_element element);
        public abstract void clear(int channelext, int stream_id);
        public abstract void clear(G2CHANNEL_STREAM cs);
        public abstract void clear(g2channel_stream_set chs);
        public abstract void remove(int channelext, int stream_id);
        public virtual void clear() { }
        public virtual void remove() { }
        public virtual int count() { return 0; }
        public virtual int speed() { return 10; }
        public virtual bool is_signaled() { return true; }

        public bool is_live() { return _mode == MODE.LIVE; }
        public bool is_play() { return _mode == MODE.PLAY; }
        public int channel() { return _channel; }

        protected readonly MODE _mode;
        protected readonly TYPE _type;
        protected readonly int _limit;
        protected int _channel;
        protected frame_buf_listener _listener;
    }
}
