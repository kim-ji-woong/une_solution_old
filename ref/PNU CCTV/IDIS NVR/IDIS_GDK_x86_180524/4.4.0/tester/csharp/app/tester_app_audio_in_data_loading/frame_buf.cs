using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using GDK;

namespace GDK_tester
{
    class frame_buf
    {
        public frame_buf()
        {
            this._decoder = new g2decoder_audio();
            this._di = new G2AUDIO_DEVICE_INFO();
            this._product = G2DECODER_CODEC.TYPE.AUDIO_UNDEFINED;
            _file_path = "";
        }

        ~frame_buf()
        {
            _decoder.cleanup();
        }
        public void on_receive_audio_frame(ref G2FRAME frame, int channel, int pane)
        {
            if (_wg == null)
            {
                G2AUDIO_CODEC_INFO aci = frame._extra._info._audio._device._internal;
                _wg = new wave_generator(aci);
            }

            if (_wg._is_data_saving == true)
            {
                startup(ref frame);
                G2DECODER_AUDIO_PARAM param = new G2DECODER_AUDIO_PARAM();
                param._frame = frame;
                byte[] data = new byte[frame.data_size];
                Marshal.Copy(frame._data, data, 0, (int)frame.data_size);
                param._buf_data = data;
                param._buf_decompress = _buf;

                if (_decoder.decompress(ref param))
                {
                    _wg.SaveDecompressedData(param._buf_decompress);
                }
            }            
        }

        public void startup(ref G2FRAME frame)
        {
            bool need_reset = true;
            G2DECODER_CODEC.TYPE product = (G2DECODER_CODEC.TYPE)frame._extra._decoder;

            if (product == G2DECODER_CODEC.TYPE.AUDIO_IAUD)
            {
                G2AUDIO_CODEC_INFO ci = frame._extra._info._audio._device._internal;
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
            byte[] data = new byte[frame.data_size];
            Marshal.Copy(frame._data, data, 0, (int)frame.data_size);
            param._frame = frame;
            param._buf_data = data;

            _di = frame._extra._info._audio._device;
            _decoder.startup(ref param);
            _product = product;

            if (need_reset != true)
            {
                return;
            }

            _buf = new byte[_di._len_output];
        }

        public void save_file()
        {
            if (_file_path != "")
            {
                _wg.Save(_file_path);
            }
        }

        public void set_file_path(string file_path)
        {
            _file_path = file_path;
        }
       


        protected g2decoder_audio       _decoder;
        protected G2AUDIO_DEVICE_INFO   _di;
        protected G2DECODER_CODEC.TYPE  _product;
        protected byte[]                _buf;
        protected string                _file_path;
        public wave_generator           _wg;

    }
}
