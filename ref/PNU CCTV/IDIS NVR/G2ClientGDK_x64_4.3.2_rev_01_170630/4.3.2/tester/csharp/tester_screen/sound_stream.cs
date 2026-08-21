using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using GDK;

#if _CONFIG_DIRECT_SOUND

using Microsoft.DirectX.DirectSound;

namespace GDK_tester
{
    public interface sound_stream_listener
    {
        void on_sound_stream_play();
        void on_sound_stream_stop();
    }

    public class sound_stream
    {
        public sound_stream()
        {
            this._format = new WaveFormat();
            this._format_set = false;
        }

        public void create(Control focus)
        {
            _device = new Device();
            _device.SetCooperativeLevel(focus, CooperativeLevel.Priority);
        }
        public void cleanup()
        {
            if (_ds_buf != null) _ds_buf.Dispose();
            if (_device != null) _device.Dispose();

            _ds_buf = null;
            _device = null;
        }

        public void set_listener(sound_stream_listener listener)
        {
            _listener = listener;
        }
        public void set_sound_format(ref G2AUDIO_DEVICE_INFO di)
        {
            WaveFormat format = new WaveFormat();
            format.FormatTag = WaveFormatTag.Pcm;
            format.BitsPerSample = (short)di._internal.bits_per_sample;
            format.BlockAlign = (short)(di._internal.channel * (di._internal.bits_per_sample / 8));
            format.Channels = (short)di._internal.channel;
            format.SamplesPerSecond = di._internal.sampling;
            format.AverageBytesPerSecond = format.SamplesPerSecond * format.BlockAlign;
            set_sound_format(ref format);
        }
        public void set_sound_format(ref WaveFormat format)
        {
            _format = format;
            _format_set = true;

            if (_ds_buf != null)
            {
                _ds_buf.Format = format;
            }
        }
        public void set_frequency(int freq)
        {
            if (_ds_buf != null)
            {
                _ds_buf.Frequency = freq;
            }
        }

        public bool create_sound_buf(int buf_size, int buf_count)
        {
            if (_device == null) return false;
            if (_format_set != true) return false;

            _buf_size = buf_size;
            _buf_count = buf_count;
            _buf_size_ds = buf_size * buf_count;
            _offset_next_write = 0;
            _spot_bunch = new spot_element[buf_count];
            _msec_per_byte = 1000.0F / _format.AverageBytesPerSecond;

            BufferDescription desc = new BufferDescription();
            desc.BufferBytes = _buf_size_ds;
            desc.Format = _format;
            desc.CanGetCurrentPosition = true;
            desc.Control3D = false;
            desc.ControlEffects = false;
            desc.ControlFrequency = true;
            desc.ControlPan = false;
            desc.ControlPositionNotify = false;
            desc.ControlVolume = true;
            desc.GlobalFocus = true;

            _ds_buf = new SecondaryBuffer(desc, _device);

            return (_ds_buf != null);
        }

        public void write(byte[] data, int len, G2SPOT spot, long pts)
        {
            if (is_valid() != true) return;

            _sound_data_len = len;

            if (_sound_data == null)
            {
                _sound_data = new byte[len];
            }
            else
            {
                if (_sound_data.Length != len)
                {
                    Array.Resize(ref _sound_data, len);
                }
            }

            Array.Copy(data, _sound_data, len);

            bool restored = restore();
            if (restored)
            {
                fill_buf();
                return;
            }

            _ds_buf.Write(_offset_next_write, _sound_data, LockFlag.None);
            _sound_data_len = 0;

            int i = _offset_next_write / len;

            lock (_spot_bunch)
            {
                _spot_bunch[i].set(spot, pts);
            }

            _offset_next_write += len;
            _offset_next_write %= _buf_size_ds;
        }
        public void fill_buf()
        {
            if (is_valid() != true) return;

            bool restored = restore();
            bool next_fill_silence = _sound_data_len > 0 ? false : true;

            if (next_fill_silence)
            {
                _ds_buf.Write(0, new byte[_buf_size_ds], LockFlag.None);
            }
            else
            {
                _ds_buf.Write(0, _sound_data, LockFlag.None);
                _sound_data_len = 0;
            }
        }

        public void play()
        {
            if (is_valid() != true) return;
            if (is_play()) return;
            if (restore())
            {
                fill_buf();
            }

            _ds_buf.Play(0, BufferPlayFlags.Looping);

            if (_listener != null)
            {
                _listener.on_sound_stream_play();
            }
        }
        public void stop()
        {
            if (is_valid() != true) return;
            if (is_play() != true) return;

            _ds_buf.Stop();

            if (_listener != null)
            {
                _listener.on_sound_stream_stop();
            }
        }
        public bool restore()
        {
            if (is_valid() != true) return false;
            if (_ds_buf.Status.BufferLost)
            {
                do
                {
                    _ds_buf.Restore();
                }
                while (_ds_buf.Status.BufferLost);
                return true;
            }
            return false;
        }
        public void reset()
        {
            if (is_valid() != true) return;

            stop();
            restore();

            _ds_buf.SetCurrentPosition(0);
            _offset_next_write = 0;
            _ds_buf.Write(0, new byte[_buf_size_ds], LockFlag.EntireBuffer);
        }

        public bool is_valid()
        {
            return (_device != null && _ds_buf != null) &&
                   (_buf_size != 0);
        }
        public bool is_play()
        {
            return is_valid() ? _ds_buf.Status.Playing : false;
        }

        public int pos_current()
        {
            return is_valid() ? (_ds_buf.PlayPosition / _buf_size) % _buf_count : -1;
        }
        public int pos_next_write()
        {
            return (_offset_next_write / _buf_size) % _buf_count;
        }
        public bool spot_current(out spot_element spot, out int play_msec)
        {
            if (is_valid() != true)
            {
                spot = new spot_element();
                play_msec = -1;
                return false;
            }

            int pos = _ds_buf.PlayPosition;
            int i = (pos / _buf_size) % _buf_count;
            play_msec = (int)((pos % _buf_size) * _msec_per_byte);

            lock (_spot_bunch)
            {
                spot = _spot_bunch[i];
            }
            return true;
        }

        public struct spot_element
        {
            public G2SPOT spot;
            public long pts;
            public void set(G2SPOT spot, long pts)
            {
                this.spot = spot;
                this.pts = pts;
            }
        }

        protected sound_stream_listener _listener;
        protected Device _device;
        protected SecondaryBuffer _ds_buf;
        protected WaveFormat _format;
        protected bool _format_set;
        protected int _buf_size;
        protected int _buf_size_ds;
        protected int _buf_count;
        protected int _offset_next_write;
        protected float _msec_per_byte;
        protected byte[] _sound_data;
        protected int _sound_data_len;
        protected spot_element[] _spot_bunch;
    }
}

#endif // !_CONFIG_DIRECT_SOUND
