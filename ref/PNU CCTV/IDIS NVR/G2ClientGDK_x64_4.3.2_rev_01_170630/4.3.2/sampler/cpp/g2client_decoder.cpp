// g2client_decoder.cpp : implementation file
//

#include "stdafx.h"
#include "g2client_decoder.h"

#include <assert.h>

#if defined(_WIN32)
#if defined(_WIN64)
#pragma comment(lib, "G2ClientGDKx64.lib")
#else
#pragma comment(lib, "G2ClientGDK.lib")
#endif
#pragma warning(disable : 4244 4267 4312 4800)
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

g2decoder::g2decoder(void)
    : _handle(G2HNULL)
{
    _handle = g2_decoder_video_initialize((G2UPARAM)(this));
}

g2decoder::~g2decoder(void)
{
    g2_decoder_video_finalize(_handle);
}

//////////////////////////////////////////////////////////////////////////

void g2decoder::startup(int count)
{
    assert(_handle != G2HNULL && "g2client_decoder is not initialized");
    g2_decoder_video_startup(_handle, count);
}

void g2decoder::cleanup(void)
{
    assert(_handle != G2HNULL && "g2client_decoder is not initialized");
    g2_decoder_video_cleanup(_handle);
}

void g2decoder::close(int camera)
{
    g2_decoder_video_close(_handle, camera);
}

void g2decoder::close(void)
{
    g2_decoder_video_close_all(_handle);
}

void g2decoder::set_mega_pixel_limitation(int factor)
{
    g2_decoder_video_set_mega_pixel_limitation(factor);
}

int g2decoder::get_mega_pixel_limitation(void)
{
    return g2_decoder_video_get_mega_pixel_limitation();
}

//////////////////////////////////////////////////////////////////////////

bool g2decoder::decompress(G2DECODER_VIDEO_PARAM& param)
{
    return g2_decoder_video_decompress(_handle, &param);
}

bool g2decoder::decompress(G2DECODER_VIDEO_PARAM_V2& param, G2DECODER_VIDEO_RESULT* res)
{
    return g2_decoder_video_decompress_v2(_handle, &param, res);
}

void g2decoder::set_deinterlacing_filter(int camera, G2DECODER_VIDEO_DEINTERLACE::FILTER filter)
{
    g2_decoder_video_set_deinterlacing_filter(_handle, camera, filter);
}

void g2decoder::set_deinterlacing_filter(G2DECODER_VIDEO_DEINTERLACE::FILTER filter)
{
    g2_decoder_video_set_deinterlacing_filter_all(_handle, filter);
}

G2DECODER_VIDEO_DEINTERLACE::FILTER g2decoder::get_deinterlacing_filter(int camera) const
{
    return (G2DECODER_VIDEO_DEINTERLACE::FILTER)g2_decoder_video_get_deinterlacing_filter(_handle, camera);
}

//////////////////////////////////////////////////////////////////////////

void g2decoder::YV12_to_YUY2(unsigned char* dst, unsigned int linesize, const unsigned char* src, unsigned int cx, unsigned int cy)
{
    g2_color_convert_yv12_to_yuy2(dst, linesize, src, cx, cy);
}

void g2decoder::YV12_to_UYVY(unsigned char* dst, unsigned int linesize, const unsigned char* src, unsigned int cx, unsigned int cy)
{
    g2_color_convert_yv12_to_uyvy(dst, linesize, src, cx, cy);
}

void g2decoder::YV12_to_RGB32(unsigned char* dst, unsigned int linesize, const unsigned char* src, unsigned int cx, unsigned int cy)
{
    g2_color_convert_yv12_to_rgb32(dst, linesize, src, cx, cy);
}

void g2decoder::YV12_to_RGB24(unsigned char* dst, unsigned int linesize, const unsigned char* src, unsigned int cx, unsigned int cy)
{
    g2_color_convert_yv12_to_rgb24(dst, linesize, src, cx, cy);
}

void g2decoder::YV12_to_RGB565(unsigned char* dst, unsigned int linesize, const unsigned char* src, unsigned int cx, unsigned int cy)
{
    g2_color_convert_yv12_to_rgb565(dst, linesize, src, cx, cy);
}

void g2decoder::YV12_to_RGB555(unsigned char* dst, unsigned int linesize, const unsigned char* src, unsigned int cx, unsigned int cy)
{
    g2_color_convert_yv12_to_rgb555(dst, linesize, src, cx, cy);
}

void g2decoder::resize_YV12(unsigned char* dst, int dw, int dh, int dzx, int dzy, int dzw, int dzh, const unsigned char* src, int sw, int sh, int szx, int szy, int szw, int szh, G2DECODER_VIDEO_SCALER_PARAM::FILTER filter /*= G2DECODER_VIDEO_SCALER_PARAM::BILINEAR*/)
{
    g2_img_resize_yv12(dst, dw, dh, dzx, dzy, dzw, dzh, src, sw, sh, szx, szy, szw, szh, filter);
}

void g2decoder::resize_YV12(unsigned char* dst, int dw, int dh, const unsigned char* src, int sw, int sh, int szx, int szy, int szw, int szh, G2DECODER_VIDEO_SCALER_PARAM::FILTER filter /*= G2DECODER_VIDEO_SCALER_PARAM::BILINEAR*/)
{
    g2_img_resize_yv12(dst, dw, dh, 0, 0, dw, dh, src, sw, sh, szx, szy, szw, szh, filter);
}

void g2decoder::resize_RGB32(unsigned char* dst, int dw, int dh, int dzx, int dzy, int dzw, int dzh, const unsigned char* src, int sw, int sh, int szx, int szy, int szw, int szh, G2DECODER_VIDEO_SCALER_PARAM::FILTER filter /*= G2DECODER_VIDEO_SCALER_PARAM::BILINEAR*/)
{
    g2_img_resize_rgb32(dst, dw, dh, dzx, dzy, dzw, dzh, src, sw, sh, szx, szy, szw, szh, filter);
}

void g2decoder::resize_RGB32(unsigned char* dst, int dw, int dh, const unsigned char* src, int sw, int sh, int szx, int szy, int szw, int szh, G2DECODER_VIDEO_SCALER_PARAM::FILTER filter /*= G2DECODER_VIDEO_SCALER_PARAM::BILINEAR*/)
{
    g2_img_resize_rgb32(dst, dw, dh, 0, 0, dw, dh, src, sw, sh, szx, szy, szw, szh, filter);
}

void g2decoder::resize_RGB24(unsigned char* dst, int dw, int dh, int dzx, int dzy, int dzw, int dzh, const unsigned char* src, int sw, int sh, int szx, int szy, int szw, int szh, G2DECODER_VIDEO_SCALER_PARAM::FILTER filter /*= G2DECODER_VIDEO_SCALER_PARAM::BILINEAR*/)
{
    g2_img_resize_rgb24(dst, dw, dh, dzx, dzy, dzw, dzh, src, sw, sh, szx, szy, szw, szh, filter);
}

void g2decoder::resize_RGB24(unsigned char* dst, int dw, int dh, const unsigned char* src, int sw, int sh, int szx, int szy, int szw, int szh, G2DECODER_VIDEO_SCALER_PARAM::FILTER filter /*= G2DECODER_VIDEO_SCALER_PARAM::BILINEAR*/)
{
    g2_img_resize_rgb24(dst, dw, dh, 0, 0, dw, dh, src, sw, sh, szx, szy, szw, szh, filter);
}

void g2decoder::resize_RGB565(unsigned char* dst, int dw, int dh, int dzx, int dzy, int dzw, int dzh, const unsigned char* src, int sw, int sh, int szx, int szy, int szw, int szh, G2DECODER_VIDEO_SCALER_PARAM::FILTER filter /*= G2DECODER_VIDEO_SCALER_PARAM::BILINEAR*/)
{
    g2_img_resize_rgb565(dst, dw, dh, dzx, dzy, dzw, dzh, src, sw, sh, szx, szy, szw, szh, filter);
}

void g2decoder::resize_RGB565(unsigned char* dst, int dw, int dh, const unsigned char* src, int sw, int sh, int szx, int szy, int szw, int szh, G2DECODER_VIDEO_SCALER_PARAM::FILTER filter /*= G2DECODER_VIDEO_SCALER_PARAM::BILINEAR*/)
{
    g2_img_resize_rgb565(dst, dw, dh, 0, 0, dw, dh, src, sw, sh, szx, szy, szw, szh, filter);
}

void g2decoder::resize_RGB555(unsigned char* dst, int dw, int dh, int dzx, int dzy, int dzw, int dzh, const unsigned char* src, int sw, int sh, int szx, int szy, int szw, int szh, G2DECODER_VIDEO_SCALER_PARAM::FILTER filter /*= G2DECODER_VIDEO_SCALER_PARAM::BILINEAR*/)
{
    g2_img_resize_rgb555(dst, dw, dh, dzx, dzy, dzw, dzh, src, sw, sh, szx, szy, szw, szh, filter);
}

void g2decoder::resize_RGB555(unsigned char* dst, int dw, int dh, const unsigned char* src, int sw, int sh, int szx, int szy, int szw, int szh, G2DECODER_VIDEO_SCALER_PARAM::FILTER filter /*= G2DECODER_VIDEO_SCALER_PARAM::BILINEAR*/)
{
    g2_img_resize_rgb555(dst, dw, dh, 0, 0, dw, dh, src, sw, sh, szx, szy, szw, szh, filter);
}

void g2decoder::resize_8u_v2(unsigned char* dst_data, int dst_linesize, const unsigned char* src_data, int src_linesize, int bpp, int dst_roi_left, int dst_roi_top, int dst_roi_width, int dst_roi_height, int src_roi_left, int src_roi_top, int src_roi_width, int src_roi_height, int filter /*= G2DECODER_VIDEO_SCALER_PARAM::BILINEAR*/)
{
    g2_img_resize_8u_v2(dst_data, dst_linesize, src_data, src_linesize, bpp, dst_roi_left, dst_roi_top, dst_roi_width, dst_roi_height, src_roi_left, src_roi_top, src_roi_width, src_roi_height, filter);
}
