using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using GDK;

namespace GDK_tester
{
    public partial class screen_pane : frame_buf_listener
    {
        public void run()
        {
            _looper.run(false);
        }
        public void suspend()
        {
            _looper.suspend();
        }
        public void suspend_search_audio()
        {
            //_search_audio.suspend();
        }
        public void resume()
        {
            _looper.resume();
        }
        public void resume_search_audio()
        {
            //_search_audio.resume();
        }
        public bool is_suspended()
        {
            return _looper.is_suspended;
        }
        public bool is_suspending()
        {
            return _looper.is_suspending;
        }
        public bool is_running()
        {
            return _looper.is_running;
        }
        public void looper_proc()
        {
            frame_element frame = null;
            int pane = _buf_manager.pop(out frame);

            if (valid_pane(pane))
            {
                if (frame != null &&
                    frame.valid)
                {
                    if (is_format1x1())
                    {
                        frame_buf buf = _buf_manager.get(frame.channel);
                        if (buf != null &&
                            buf.is_live() != true)
                        {
                            if (buf.speed() == 10)
                            {
                                if (_buf_manager.get_audio_count() > 0)
                                {
                                    search_audio_play(frame);
                                }
                            }
                        }
                    }

                    video_play(ref frame);
                }
            }
            else if (pane == (int)frame_element.LOADED.END_OF)
            {
                // in case that occur no-frame loaded as there is not inputed frame,
                // so is not called stop method, should stop audio play by compulsion.
                search_audio_stop();

                if (_listener != null)
                {
                    _listener.on_screen_play_end_loaded(frame.channel);
                }
            }
            else
            {
                Thread.Sleep(1);
            }

            // flush first element to prevent buffer full infinite loop
            // by input audio frames without video frame.
            //if (_search_audio.is_suspended())
            //{
            //    if (_buf_manager.get_audio_count() >= frame_buf.BUF_SIZE.AUDIO - 1)
            //    {
            //        int channel_audio = _buf_manager.get_audio_channel();
            //        if (channel_audio >= 0)
            //        {
            //            if (_buf_manager.get_frame_count(channel_audio) == 0)
            //            {
            //                _buf_manager.pop_audio();
            //            }
            //        }
            //    }
            //}
        }

        public void on_frame_buf_set_pane_audio_enable(bool enable)
        {
            set_pane_audio_enable(enable);
        }
        public void on_frame_buf_set_pane_audio_enable(int pane, bool enable)
        {
            set_pane_audio_enable(pane, enable);
        }
        public void on_frame_buf_sound_stream_get_current_pos(out G2SPOT spot, out int play_msec, out long pts)
        {
            spot = new G2SPOT();
            play_msec = 0;
            pts = 0;
            //_search_audio.get_current_pos(out spot, out play_msec, out pts);
        }

        protected g2looper _looper;
        protected sound_looper _search_audio;
    }
}
