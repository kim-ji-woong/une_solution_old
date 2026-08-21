// g2client_decoder.h : header file
//

#ifndef _G2_CLIENT_DLL_SAMPLER_DECODER_H_
#define _G2_CLIENT_DLL_SAMPLER_DECODER_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_decoder.h>
#include <map>

namespace client {

//////////////////////////////////////////////////////////////////////////

class g2decoder
{
public:
    g2decoder(void);
    virtual ~g2decoder(void);

private:
    G2HDECODER _handle;

public:
    void startup(int count);
    void cleanup(void);
    void close(int camera);
    void close(void);

    G2HDECODER safe_handle(void) const { return this != NULL ? _handle : G2HNULL; }

    static void set_mega_pixel_limitation(int factor);
    static int  get_mega_pixel_limitation(void);

public:
    bool decompress(G2DECODER_VIDEO_PARAM& param);
    bool decompress(G2DECODER_VIDEO_PARAM_V2& param, G2DECODER_VIDEO_RESULT* res);

    void set_deinterlacing_filter(int camera, G2DECODER_VIDEO_DEINTERLACE::FILTER filter);
    void set_deinterlacing_filter(G2DECODER_VIDEO_DEINTERLACE::FILTER filter);
    G2DECODER_VIDEO_DEINTERLACE::FILTER get_deinterlacing_filter(int camera) const;

public:
    static void YV12_to_YUY2(unsigned char* dst, unsigned int linesize, const unsigned char* src, unsigned int cx, unsigned int cy);
    static void YV12_to_UYVY(unsigned char* dst, unsigned int linesize, const unsigned char* src, unsigned int cx, unsigned int cy);
    static void YV12_to_RGB32(unsigned char* dst, unsigned int linesize, const unsigned char* src, unsigned int cx, unsigned int cy);
    static void YV12_to_RGB24(unsigned char* dst, unsigned int linesize, const unsigned char* src, unsigned int cx, unsigned int cy);
    static void YV12_to_RGB565(unsigned char* dst, unsigned int linesize, const unsigned char* src, unsigned int cx, unsigned int cy);
    static void YV12_to_RGB555(unsigned char* dst, unsigned int linesize, const unsigned char* src, unsigned int cx, unsigned int cy);

    static void resize_YV12(unsigned char* dst, int dw, int dh, int dzx, int dzy, int dzw, int dzh, const unsigned char* src, int sw, int sh, int szx, int szy, int szw, int szh, G2DECODER_VIDEO_SCALER_PARAM::FILTER filter = G2DECODER_VIDEO_SCALER_PARAM::BILINEAR);
    static void resize_YV12(unsigned char* dst, int dw, int dh, const unsigned char* src, int sw, int sh, int szx, int szy, int szw, int szh, G2DECODER_VIDEO_SCALER_PARAM::FILTER filter = G2DECODER_VIDEO_SCALER_PARAM::BILINEAR);
    static void resize_RGB32(unsigned char* dst, int dw, int dh, int dzx, int dzy, int dzw, int dzh, const unsigned char* src, int sw, int sh, int szx, int szy, int szw, int szh, G2DECODER_VIDEO_SCALER_PARAM::FILTER filter = G2DECODER_VIDEO_SCALER_PARAM::BILINEAR);
    static void resize_RGB32(unsigned char* dst, int dw, int dh, const unsigned char* src, int sw, int sh, int szx, int szy, int szw, int szh, G2DECODER_VIDEO_SCALER_PARAM::FILTER filter = G2DECODER_VIDEO_SCALER_PARAM::BILINEAR);
    static void resize_RGB24(unsigned char* dst, int dw, int dh, int dzx, int dzy, int dzw, int dzh, const unsigned char* src, int sw, int sh, int szx, int szy, int szw, int szh, G2DECODER_VIDEO_SCALER_PARAM::FILTER filter = G2DECODER_VIDEO_SCALER_PARAM::BILINEAR);
    static void resize_RGB24(unsigned char* dst, int dw, int dh, const unsigned char* src, int sw, int sh, int szx, int szy, int szw, int szh, G2DECODER_VIDEO_SCALER_PARAM::FILTER filter = G2DECODER_VIDEO_SCALER_PARAM::BILINEAR);
    static void resize_RGB565(unsigned char* dst, int dw, int dh, int dzx, int dzy, int dzw, int dzh, const unsigned char* src, int sw, int sh, int szx, int szy, int szw, int szh, G2DECODER_VIDEO_SCALER_PARAM::FILTER filter = G2DECODER_VIDEO_SCALER_PARAM::BILINEAR);
    static void resize_RGB565(unsigned char* dst, int dw, int dh, const unsigned char* src, int sw, int sh, int szx, int szy, int szw, int szh, G2DECODER_VIDEO_SCALER_PARAM::FILTER filter = G2DECODER_VIDEO_SCALER_PARAM::BILINEAR);
    static void resize_RGB555(unsigned char* dst, int dw, int dh, int dzx, int dzy, int dzw, int dzh, const unsigned char* src, int sw, int sh, int szx, int szy, int szw, int szh, G2DECODER_VIDEO_SCALER_PARAM::FILTER filter = G2DECODER_VIDEO_SCALER_PARAM::BILINEAR);
    static void resize_RGB555(unsigned char* dst, int dw, int dh, const unsigned char* src, int sw, int sh, int szx, int szy, int szw, int szh, G2DECODER_VIDEO_SCALER_PARAM::FILTER filter = G2DECODER_VIDEO_SCALER_PARAM::BILINEAR);

    static void resize_8u_v2(unsigned char* dst_data, int dst_linesize, const unsigned char* src_data, int src_linesize, int bpp, int dst_roi_left, int dst_roi_top, int dst_roi_width, int dst_roi_height, int src_roi_left, int src_roi_top, int src_roi_width, int src_roi_height, int filter = G2DECODER_VIDEO_SCALER_PARAM::BILINEAR);
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_G2_CLIENT_DLL_SAMPLER_DECODER_H_
