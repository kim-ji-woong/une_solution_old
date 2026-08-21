using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDK
{
    public struct G2DECODER_CODEC
    {
        public enum TYPE
        {
            UNDEFINED = -1,
            MJPEG = 1,
            MLJPEG = 2,
            MPEG4 = 3,
            H264 = 4,
            BITMAP = 5,
            MXPEG = 6,
            HEVC,

            AUDIO_UNDEFINED = 64,
            AUDIO_IAUD = 66
        }
    }

    public struct G2DECODER_VIDEO_PIX_FORMAT
    {
        public enum TYPE
        {
            YV12 = 0,
            YUY2,
            UYVY,
            RGB32,
            RGB24,
            RGB565,
            RGB555
        }
    }

    public struct G2DECODER_VIDEO_DEINTERLACE
    {
        public enum FILTER
        {
            UNDEFINED = -1,
            NOT_USE = 0,
            SOFTEN = 1,
            SHARPEN = 5,
            EDGE_DETECT = 3,
            HIGH_SPEED = 7
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2DECODER_VIDEO_SCALER_PARAM
    {
        public enum FILTER
        {
            BILINEAR_FAST = 1,
            BILINEAR = 2,
            BICUBIC = 4,
            POINT = 16,
            AREA = 32,
            BICUBLIN = 64,
            GAUSS = 128,
            SINC = 256,
            LANCZOS = 512,
            SPLINE = 1024
        }

        public int _width;
        public int _height;
        public int _filter;

        public FILTER filter
        {
            get { return (FILTER)_filter; }
            set { _filter = (int)value; }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2DECODER_VIDEO_RESULT
    {
        public enum RESULT_TYPE
        {
            UNKNOWN = -1,
            DECODING_FAILED = 0,
            SUCCESS = 1,
            DECODER_NOT_FOUND,
            DECODER_NOT_READY,
            DECODER_RESETED,
            INVALID_ARGUMENT,
            INVALID_FRAME,
            DISCARD_FRAME,
            SKIPPED_FRAME,
            IS_NOT_IFRAME,
            BROKEN_FRAME,
            REPEAT_FRAME,
            NOT_ENOUGH_DATA,
            NOT_ENOUGH_BUF,
            OUT_OF_MEMORY,
            OUT_OF_DECODER,
            NEEDS_REINIT,
            ERR_DIVA_SESSION_POOL,
            ERR_MEMORY_ALLOC,
            ERR_UNSUPPORTED,
            ERR_UNSUPPORTED_RES,
            ERR_SCALER
        }

        public int _result;
        public int _width;
        public int _height;
        public int _elapse;
        public long _dts;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2DECODER_VIDEO_RESULT_V3
    {
        public enum RESULT_TYPE
        {
            UNKNOWN = -1,
            DECODING_FAILED = 0,
            SUCCESS = 1,
            DECODER_NOT_FOUND,
            DECODER_NOT_READY,
            DECODER_RESETED,
            INVALID_ARGUMENT,
            INVALID_FRAME,
            DISCARD_FRAME,
            SKIPPED_FRAME,
            IS_NOT_IFRAME,
            BROKEN_FRAME,
            REPEAT_FRAME,
            NOT_ENOUGH_DATA,
            NOT_ENOUGH_BUF,
            OUT_OF_MEMORY,
            OUT_OF_DECODER,
            NEEDS_REINIT,
            ERR_DIVA_SESSION_POOL,
            ERR_MEMORY_ALLOC,
            ERR_UNSUPPORTED,
            ERR_UNSUPPORTED_RES,
            ERR_SCALER
        };

        public G2PICTURE _pic;
        public G2SIZE _res_coded;
        public G2SIZE _res;
        public float _src_aspect_ratio;
        public int _result;
        public int _color_space;
        public int _color_range;
        public int _threads;
        public int _elapse;
        public long _dts;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct G2DECODER_VIDEO_PARAM_V1
    {
        public IntPtr _frame;
        public G2DECODER_VIDEO_PIX_FORMAT.TYPE _format;
        public IntPtr _buf_decompress;
        public IntPtr _buf_filter;
        public uint _buf_decompress_len;
        public uint _buf_filter_len;
        public int _decoder_id;
        public G2DECODER_VIDEO_SCALER_PARAM _resize;
        public G2DECODER_VIDEO_RESULT _result;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2DECODER_VIDEO_PARAM_V2
    {
        public G2FRAME _frame;
        public G2DECODER_VIDEO_PIX_FORMAT.TYPE _format;
        public IntPtr _buf_decompress;
        public IntPtr _buf_filter;
        public uint _buf_decompress_len;
        public uint _buf_filter_len;
        public int _decoder_id;
        public int _threads;
        public G2DECODER_VIDEO_SCALER_PARAM _resize;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2DECODER_VIDEO_PARAM_V3
    {
        public G2FRAME _frame;
        public IntPtr _buf;
        public uint _buf_len;
        public IntPtr _buf_filter;
        public uint _buf_filter_len;
        public int _decoder_id;
        public int _threads;
        public G2DECODER_VIDEO_SCALER_PARAM _resize;
    }

    public struct G2DECODER_VIDEO_PARAM
    {
        public G2FRAME _frame;
        public G2DECODER_VIDEO_PIX_FORMAT.TYPE _format;
        public byte[] _buf_data;
        public byte[] _buf_decompress;
        public byte[] _buf_filter;
        public int _decoder_id;
        public int _threads;
        public G2DECODER_VIDEO_SCALER_PARAM _resize;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2DECODER_AUDIO_PARAM_V1
    {
        public G2FRAME _frame;
        public IntPtr _buf_decompress;
        public uint _buf_decompress_len;
    }

    public struct G2DECODER_AUDIO_PARAM
    {
        public G2FRAME _frame;
        public byte[] _buf_data;
        public byte[] _buf_decompress;
    }
}
