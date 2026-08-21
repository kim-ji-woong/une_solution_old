// g2_decoder.h : header file
//

#ifndef _G2_CLIENT_DLL_DECODER_H_
#define _G2_CLIENT_DLL_DECODER_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "g2_define_decoder.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC G2HDECODER G2API g2_decoder_video_initialize(G2UPARAM param);
G2_DLLFUNC void G2API g2_decoder_video_finalize(G2HDECODER handle);
G2_DLLFUNC void G2API g2_decoder_video_startup(G2HDECODER handle, int count);
G2_DLLFUNC void G2API g2_decoder_video_cleanup(G2HDECODER handle);
G2_DLLFUNC void G2API g2_decoder_video_close(G2HDECODER handle, int camera);
G2_DLLFUNC void G2API g2_decoder_video_close_all(G2HDECODER handle);

G2_DLLFUNC void G2API g2_decoder_video_set_mega_pixel_limitation(int factor);
G2_DLLFUNC int  G2API g2_decoder_video_get_mega_pixel_limitation(void);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_decoder_video_decompress(G2HDECODER handle, G2DECODER_VIDEO_PARAM* param);
G2_DLLFUNC bool G2API g2_decoder_video_decompress_v2(G2HDECODER handle, G2DECODER_VIDEO_PARAM_V2* param, G2DECODER_VIDEO_RESULT* res);
G2_DLLFUNC bool G2API g2_decoder_video_decompress_v3(G2HDECODER handle, G2DECODER_VIDEO_PARAM_V3* param, G2DECODER_VIDEO_RESULT_V3* res);
G2_DLLFUNC void G2API g2_decoder_video_set_deinterlacing_filter(G2HDECODER handle, int camera, int filter);
G2_DLLFUNC void G2API g2_decoder_video_set_deinterlacing_filter_all(G2HDECODER handle, int filter);
G2_DLLFUNC int  G2API g2_decoder_video_get_deinterlacing_filter(G2HDECODER handle, int camera);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC int  G2API g2_decoder_picture_buf_len(int width, int height, int pix_format);
G2_DLLFUNC void G2API g2_decoder_picture_fill(G2PICTURE* pic, int pix_format, int width, int height, unsigned char* buf, int len);
G2_DLLFUNC bool G2API g2_decoder_picture_scale(G2PICTURE* dst, int dst_pix_format, int dst_cx, int dst_cy, unsigned char* buf, int len, int filter,
                                         const G2PICTURE* src, int src_pix_format, int src_cx, int src_cy, int color_space, int color_range);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_color_convert_yv12_to_yuy2  (unsigned char* dst, unsigned int linesize, const unsigned char* src, unsigned int cx, unsigned int cy);
G2_DLLFUNC void G2API g2_color_convert_yv12_to_uyvy  (unsigned char* dst, unsigned int linesize, const unsigned char* src, unsigned int cx, unsigned int cy);
G2_DLLFUNC void G2API g2_color_convert_yv12_to_rgb32 (unsigned char* dst, unsigned int linesize, const unsigned char* src, unsigned int cx, unsigned int cy);
G2_DLLFUNC void G2API g2_color_convert_yv12_to_rgb24 (unsigned char* dst, unsigned int linesize, const unsigned char* src, unsigned int cx, unsigned int cy);
G2_DLLFUNC void G2API g2_color_convert_yv12_to_rgb565(unsigned char* dst, unsigned int linesize, const unsigned char* src, unsigned int cx, unsigned int cy);
G2_DLLFUNC void G2API g2_color_convert_yv12_to_rgb555(unsigned char* dst, unsigned int linesize, const unsigned char* src, unsigned int cx, unsigned int cy);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_img_resize_yv12  (unsigned char* dst, int dw, int dh, int dzx, int dzy, int dzw, int dzh, const unsigned char* src, int sw, int sh, int szx, int szy, int szw, int szh, int filter);
G2_DLLFUNC void G2API g2_img_resize_rgb32 (unsigned char* dst, int dw, int dh, int dzx, int dzy, int dzw, int dzh, const unsigned char* src, int sw, int sh, int szx, int szy, int szw, int szh, int filter);
G2_DLLFUNC void G2API g2_img_resize_rgb24 (unsigned char* dst, int dw, int dh, int dzx, int dzy, int dzw, int dzh, const unsigned char* src, int sw, int sh, int szx, int szy, int szw, int szh, int filter);
G2_DLLFUNC void G2API g2_img_resize_rgb565(unsigned char* dst, int dw, int dh, int dzx, int dzy, int dzw, int dzh, const unsigned char* src, int sw, int sh, int szx, int szy, int szw, int szh, int filter);
G2_DLLFUNC void G2API g2_img_resize_rgb555(unsigned char* dst, int dw, int dh, int dzx, int dzy, int dzw, int dzh, const unsigned char* src, int sw, int sh, int szx, int szy, int szw, int szh, int filter);
G2_DLLFUNC bool G2API g2_img_resize_8u_v2 (unsigned char* dst_data, int dst_linesize, const unsigned char* src_data, int src_linesize, int bpp, int dst_roi_left, int dst_roi_top, int dst_roi_width, int dst_roi_height, int src_roi_left, int src_roi_top, int src_roi_width, int src_roi_height, int filter);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_img_filter_eqalize_l8    (unsigned char* dst, int dst_linesize, const unsigned char* src, int src_linesize, int cx, int cy);
G2_DLLFUNC void G2API g2_img_filter_high_boost_l8 (unsigned char* dst, int dst_linesize, const unsigned char* src, int src_linesize, int cx, int cy, float factor);
G2_DLLFUNC void G2API g2_img_filter_edge_detect_l8(unsigned char* dst, int dst_linesize, const unsigned char* src, int src_linesize, int cx, int cy);
G2_DLLFUNC void G2API g2_img_filter_median_rgb24  (unsigned char* dst, int dst_linesize, const unsigned char* src, int src_linesize, int cx, int cy);
G2_DLLFUNC void G2API g2_img_filter_sharpen_rgb24 (unsigned char* dst, int dst_linesize, const unsigned char* src, int src_linesize, int cx, int cy);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC G2HDECODER G2API g2_decoder_audio_initialize(G2UPARAM param);
G2_DLLFUNC void G2API g2_decoder_audio_finalize(G2HDECODER handle);
G2_DLLFUNC bool G2API g2_decoder_audio_startup(G2HDECODER handle, const G2DECODER_AUDIO_PARAM_V1* param);
G2_DLLFUNC void G2API g2_decoder_audio_cleanup(G2HDECODER handle);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_decoder_audio_decompress(G2HDECODER handle, G2DECODER_AUDIO_PARAM_V1* param);
G2_DLLFUNC bool G2API g2_decoder_audio_is_available(G2HDECODER handle);
G2_DLLFUNC bool G2API g2_decoder_audio_is_dif(G2HDECODER handle, const G2AUDIO_CODEC_INFO* ci);
G2_DLLFUNC bool G2API g2_decoder_audio_is_need_reset(G2HDECODER handle, const G2AUDIO_CODEC_INFO* ci);

//////////////////////////////////////////////////////////////////////////

#if defined(_WIN32)
G2_DLLFUNC bool G2API g2_decoder_get_diagnosis_codec_info_BSTR(const G2FRAME* frame, BSTR* res);
#endif

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_CLIENT_DLL_DECODER_H_
