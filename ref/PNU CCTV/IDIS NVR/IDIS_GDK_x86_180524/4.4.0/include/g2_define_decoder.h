// g2_define_decoder_h : header file
//

#ifndef _G2_DEFINE_DECODER_H_
#define _G2_DEFINE_DECODER_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define.h"

#ifdef __cplusplus
extern "C" {
#endif

///////////////////////////////////////////////////////////////////////////////

struct G2DECODER_CODEC {
    enum CODEC {
        UNDEFINED = -1,
        MJPEG  = 1,
        MLJPEG = 2,
        MPEG4  = 3,
        H264   = 4,
        BITMAP = 5,
        MXPEG  = 6,
        HEVC,

        AUDIO_UNDEFINED = 64,
        AUDIO_IAUD = 66
    };
};

struct G2DECODER_VIDEO_PIX_FORMAT {
    enum TYPE {
        YV12 = 0,
        YUY2,
        UYVY,
        RGB32,
        RGB24,
        RGB565,
        RGB555
    };
};

struct G2DECODER_VIDEO_COLOR_SPACE {
    enum TYPE {
        RGB         = 0,
        BT709       = 1,
        UNSPECIFIED = 2,
        RESERVED    = 3,
        FCC         = 4,
        BT470BG     = 5,
        SMPTE170M   = 6,
        SMPTE240M   = 7,
        YCOCG       = 8,
        BT2020_NCL  = 9,
        BT2020_CL   = 10
    };
};

struct G2DECODER_VIDEO_COLOR_RANGE {
    enum TYPE {
        UNSPECIFIED = 0,
        MPEG        = 1,
        FULL        = 2
    };
};

struct G2DECODER_VIDEO_DEINTERLACE {
    enum FILTER {
        UNDEFINED   = -1,
        NOT_USE     = 0,
        SOFTEN      = 1,
        SHARPEN     = 5,
        EDGE_DETECT = 3,
        HIGH_SPEED  = 7
    };
};

struct G2DECODER_VIDEO_SCALER_PARAM {
    enum FILTER {
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
    };

    int _width;
    int _height;
    int _filter;
};

struct G2DECODER_VIDEO_RESULT {
    enum RESULT_TYPE {
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

    int _result;
    int _width;
    int _height;
    int _elapse;
    __int64 _dts;
};

struct G2DECODER_VIDEO_RESULT_V3 {
    enum RESULT_TYPE {
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

    G2PICTURE _pic;
    G2SIZE _res_coded;
    G2SIZE _res;
    float  _src_aspect_ratio;
    int _result;
    int _color_space;
    int _color_range;
    int _threads;
    int _elapse;
    __int64 _dts;
};

struct G2DECODER_VIDEO_PARAM {
    const G2FRAME* _frame;
    G2DECODER_VIDEO_PIX_FORMAT::TYPE _format;
    unsigned char* _buf_decompress;
    unsigned char* _buf_filter;
    unsigned int   _buf_decompress_len;
    unsigned int   _buf_filter_len;
    int _decoder_id;
    int _threads;
    G2DECODER_VIDEO_SCALER_PARAM _resize;
    G2DECODER_VIDEO_RESULT _result;
};

struct G2DECODER_VIDEO_PARAM_V2 {
    G2FRAME _frame;
    G2DECODER_VIDEO_PIX_FORMAT::TYPE _format;
    unsigned char* _buf_decompress;
    unsigned char* _buf_filter;
    unsigned int   _buf_decompress_len;
    unsigned int   _buf_filter_len;
    int _decoder_id;
    int _threads;
    G2DECODER_VIDEO_SCALER_PARAM _resize;
};

struct G2DECODER_VIDEO_PARAM_V3 {
    G2FRAME _frame;
    unsigned char* _buf;
    unsigned int   _buf_len;
    unsigned char* _buf_filter;
    unsigned int   _buf_filter_len;
    int _decoder_id;
    int _threads;
    G2DECODER_VIDEO_SCALER_PARAM _resize;
};

struct G2DECODER_AUDIO_PARAM_V1
{
    G2FRAME _frame;
    unsigned char* _buf_decompress;
    unsigned int   _buf_decompress_len;
};

///////////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_DEFINE_DECODER_H_
