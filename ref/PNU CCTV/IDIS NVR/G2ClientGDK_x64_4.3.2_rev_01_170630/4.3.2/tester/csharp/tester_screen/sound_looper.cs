using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using GDK;

namespace GDK_tester
{
#if !_CONFIG_SLIMDX_SOUND && !_CONFIG_DIRECT_SOUND
    public class sound_looper
    {
        public void create(frame_buf_manager buf_manager) { }
        public void cleanup() { }
        public void close() { }
        public void set_product(frame_element element) { }
        public void run() { }
        public void suspend() { }
        public void resume() { }
        public void play() { }
        public void stop() { }
        public void stop(bool wait) { }
        public bool get_current_pos(out G2SPOT spot, out int play_msec, out long pts)
        {
            spot = G2SPOT.INVALID_SPOT;
            play_msec = -1;
            pts = -1;
            return false;
        }
        public bool is_running() { return false; }
        public bool is_suspended() { return false; }
    }
#else
    public class sound_looper : sound_stream_listener
    {
        public sound_looper()
        {
            this._decoder = new g2decoder_audio();
            this._looper = new g2looper();
            this._looper.on_begin += new g2looper.looper_event(this.looper_begin);
            this._looper.on_end += new g2looper.looper_event(this.looper_end);
            this._stream = new sound_stream();
            this._product = G2DECODER_CODEC.TYPE.AUDIO_UNDEFINED;
            this._di = new G2AUDIO_DEVICE_INFO();
            this._pane = -1;
            this._stream_active = false;
            this._need_frame = true;
        }
        ~sound_looper()
        {
            cleanup();
        }

        public void create(frame_buf_manager buf_manager)
        {
            _buf_manager = buf_manager;
            _need_frame = true;
            _stream_active = true;
            _looper.create(looper_proc);
            _looper.name = "GDK_tester::sound_looper";
            _looper.run(true);
        }
        public void cleanup()
        {
            _looper.cleanup();

            close();
        }
        public void close()
        {
            stop();

            _stream.cleanup();
            _decoder.cleanup();
            _product = G2DECODER_CODEC.TYPE.AUDIO_UNDEFINED;
            _di = new G2AUDIO_DEVICE_INFO();
            _len_sample = 0;
            _len_output = 0;
            _pane = -1;
            _stream_active = true;
            _buf = null;
            _need_frame = true;
        }

        public void set_product(frame_element element)
        {
            bool need_reset = true;
            G2DECODER_CODEC.TYPE product = (G2DECODER_CODEC.TYPE)element.frame._extra._decoder;

            if (product == G2DECODER_CODEC.TYPE.AUDIO_IAUD)
            {
                G2AUDIO_CODEC_INFO ci = element.frame._extra._info._audio._device._internal;
                if (_decoder.is_dif(ref ci) != true)
                {
                    return;
                }
                if (_decoder.is_need_reset(ref ci) != true)
                {
                    need_reset = false;
                }
            }
            else if (_product == product)
            {
                return;
            }

            G2DECODER_AUDIO_PARAM param = new G2DECODER_AUDIO_PARAM();
            param._frame = element.frame;
            param._buf_data = element.data;

            _di = element.frame._extra._info._audio._device;
            _decoder.startup(ref param);
            _product = product;

            if (need_reset != true)
            {
                return;
            }

            bool play_pre = _stream.is_play();

            _buf = new byte[_di._len_output * 2];

            bool proceed = false;

            try
            {
                _stream.cleanup();
                _stream.create(_focus);
                _stream.set_sound_format(ref _di);
                _stream.create_sound_buf(_di._len_sample, COUNT_SOUND_BUF);
                _stream.reset();
                proceed = true;
            }
            catch { }

            _stream_active = proceed;

            if (play_pre)
            {
                _stream.play();
            }
        }

        public void run()
        {
            _looper.run(false);
        }
        public void suspend()
        {
            _looper.suspend();
        }
        public void resume()
        {
            _looper.resume();
        }

        public void play()
        {
            if (_looper.is_running)
            {
                return;
            }

            _looper.resume();
        }

        public void stop()
        {
            stop(false);
        }
        public void stop(bool wait)
        {
            if (_looper.is_suspended ||
                _looper.is_suspending)
            {
                return;
            }

            _looper.suspend();

            if (wait)
            {
                while (_looper.is_suspending)
                {
                    g2looper.sleep(1);
                }
            }

            _stream.reset();
            _product = G2DECODER_CODEC.TYPE.UNDEFINED;
            _decoder.cleanup();
            _buf_manager.reset_audio();
            _buf_manager.set_audio_play(false);
            _need_frame = true;
        }

        public bool get_current_pos(out G2SPOT spot, out int play_msec, out long pts)
        {
            sound_stream.spot_element element;
            bool ret = false;
            if (_stream.spot_current(out element, out play_msec))
            {
                spot = element.spot;
                pts = element.pts;
                ret = true;
            }
            else
            {
                spot = G2SPOT.INVALID_SPOT;
                play_msec = -1;
                pts = -1;
            }
            return ret;
        }

        private void audio_proc(frame_element element)
        {
            set_product(element);

            if (_stream_active != true)
            {
                return;
            }

            G2DECODER_AUDIO_PARAM param = new G2DECODER_AUDIO_PARAM();
            param._frame = element.frame;
            param._buf_data = element.data;
            param._buf_decompress = _buf;

            if (_decoder.decompress(ref param))
            {
                _stream.write(_buf, _di._len_sample, element.frame.spot, element.frame._extra._pts);
            }
        }

        public void looper_begin(g2looper looper)
        {
            _focus = new Form();
        }
        public void looper_end(g2looper looper)
        {
            if (_focus != null)
            {
                _focus.Dispose();
            }
        }

        public void looper_proc()
        {
            if (_looper.is_continue != true) return;
            if (_need_frame)
            {
                frame_element frame = null;
                _pane = _buf_manager.pop_audio(out frame);

                if (_pane >= 0)
                {
                    audio_proc(frame);
                }
            }

            if (_stream_active != true)
            {
                _buf_manager.set_pane_audio_enable(false);
                stop();
                return;
            }

            if (_stream.is_valid() != true)
            {
                g2looper.sleep(1);
                return;
            }

            int pos_curr = _stream.pos_current();
            int pos_next = _stream.pos_next_write();

            int dif = pos_next - pos_curr;
            if (dif < 0)
            {
                dif = COUNT_SOUND_BUF - pos_curr + pos_next;
            }

            if (_pane < 0 && dif != 0)
            {
                // in case inputed audio-frame appended at sound buffer and don't play total frame,
                // but there is not frame that append, sound stream play to process frame at sound buffer.
                // if sound stream is not played, video play may be stopped.
                _stream.play(); // for check play status internally
            }
            else if (dif == 0)
            {
                // if sound stream played total frame at buffer, audio play will be stopped.
                // to play video normally, notify at actuator fact that audio spot is invalid.
                stop();
            }
            else if (dif >= COUNT_SOUND_BUF - COUNT_SOUND_BUF_SENTINEL)
            {
                _stream.play();
                _need_frame = false;
            }
            else if (dif < COUNT_SOUND_BUF_SENTINEL)
            {
                _need_frame = true;
            }

            // sound play if runtime of input-frames is bigger than 1 sec.
            if (dif >= Math.Max(_di._count_per_sec, 2))
            {
                _stream.play();
            }

            if (_looper.is_running)
            {
                g2looper.sleep(2);
            }
        }

        public bool is_running()
        {
            return _looper.is_running;
        }
        public bool is_suspended()
        {
            return _looper.is_suspended;
        }

        public void on_sound_stream_play()
        {
            _buf_manager.set_audio_play(true);
        }

        public void on_sound_stream_stop()
        {
            _buf_manager.set_audio_play(false);
        }

        protected g2decoder_audio _decoder;
        protected g2looper _looper;
        protected sound_stream _stream;
        protected frame_buf_manager _buf_manager;
        protected G2DECODER_CODEC.TYPE _product;
        protected G2AUDIO_DEVICE_INFO _di;
        protected Form _focus;
        protected int _len_sample;
        protected int _len_output;
        protected int _pane;
        protected byte[] _buf;
        protected bool _stream_active;
        protected bool _need_frame;

        public readonly int COUNT_SOUND_BUF = 64;
        public readonly int COUNT_SOUND_BUF_SENTINEL = 8;
    }
#endif
}
