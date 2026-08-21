using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace GDK
{
    using G2HANDLE = System.Int32;
    using G2HDECODER = System.Int32;
#if _WIN64
    using G2WPARAM = System.UInt64;
    using G2LPARAM = System.Int64;
    using G2UPARAM = System.IntPtr;
    using G2RESULT = System.Int64;
#else
    using G2WPARAM = System.UInt32;
    using G2LPARAM = System.Int32;
    using G2UPARAM = System.IntPtr;
    using G2RESULT = System.Int32;
#endif

    public class g2decoder
    {
        #region GDK DLL Import
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern G2HDECODER g2_decoder_video_initialize(G2UPARAM param);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_decoder_video_finalize(G2HDECODER handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_decoder_video_startup(G2HDECODER handle, int count);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_decoder_video_cleanup(G2HDECODER handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_decoder_video_close(G2HDECODER handle, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_decoder_video_close_all(G2HDECODER handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_decoder_video_set_mega_pixel_limitation(int factor);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_decoder_video_get_mega_pixel_limitation();
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_decoder_video_decompress(G2HDECODER handle, ref G2DECODER_VIDEO_PARAM_V1 param);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool g2_decoder_video_decompress_v2(G2HDECODER handle, ref G2DECODER_VIDEO_PARAM_V2 param, out G2DECODER_VIDEO_RESULT res);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_decoder_video_set_deinterlacing_filter(G2HDECODER handle, int camera, int filter);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_decoder_video_set_deinterlacing_filter_all(G2HDECODER handle, int filter);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern int g2_decoder_video_get_deinterlacing_filter(G2HDECODER handle, int camera);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_color_convert_yv12_to_yuy2(IntPtr dst, uint linesize, IntPtr src, uint cx, uint cy);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_color_convert_yv12_to_uyvy(IntPtr dst, uint linesize, IntPtr src, uint cx, uint cy);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_color_convert_yv12_to_rgb32(IntPtr dst, uint linesize, IntPtr src, uint cx, uint cy);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_color_convert_yv12_to_rgb24(IntPtr dst, uint linesize, IntPtr src, uint cx, uint cy);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_color_convert_yv12_to_rgb565(IntPtr dst, uint linesize, IntPtr src, uint cx, uint cy);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_color_convert_yv12_to_rgb555(IntPtr dst, uint linesize, IntPtr src, uint cx, uint cy);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_img_resize_yv12(IntPtr dst, int dw, int dh, int dzx, int dzy, int dzw, int dzh, IntPtr src, int sw, int sh, int szx, int szy, int szw, int szh, int filter);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_img_resize_rgb32(IntPtr dst, int dw, int dh, int dzx, int dzy, int dzw, int dzh, IntPtr src, int sw, int sh, int szx, int szy, int szw, int szh, int filter);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_img_resize_rgb24(IntPtr dst, int dw, int dh, int dzx, int dzy, int dzw, int dzh, IntPtr src, int sw, int sh, int szx, int szy, int szw, int szh, int filter);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_img_resize_rgb565(IntPtr dst, int dw, int dh, int dzx, int dzy, int dzw, int dzh, IntPtr src, int sw, int sh, int szx, int szy, int szw, int szh, int filter);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_img_resize_rgb555(IntPtr dst, int dw, int dh, int dzx, int dzy, int dzw, int dzh, IntPtr src, int sw, int sh, int szx, int szy, int szw, int szh, int filter);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_img_resize_8u_v2(IntPtr dst_data, int dst_linesize, IntPtr src_data, int src_linesize, int bpp, int dst_roi_left, int dst_roi_top, int dst_roi_width, int dst_roi_height, int src_roi_left, int src_roi_top, int src_roi_width, int src_roi_height, int filter);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_decoder_get_diagnosis_codec_info_BSTR(ref G2FRAME frame, out IntPtr res);
        #endregion

        public g2decoder()
        {
            _handle = 0;
            _param = new G2UPARAM(0);
        }
        ~g2decoder()
        {
            cleanup();
        }

        private G2HDECODER _handle;
        private G2UPARAM _param;

        public void startup(int count)
        {
            cleanup();

            _handle = g2_decoder_video_initialize(_param);

            g2_decoder_video_startup(_handle, count);
        }
        public void cleanup()
        {
            if (_handle != 0)
            {
                G2HDECODER handle = _handle; _handle = 0;
                g2_decoder_video_cleanup(handle);
                g2_decoder_video_finalize(handle);
            }
        }
        public void close(int camera)
        {
            g2_decoder_video_close(_handle, camera);
        }
        public void close()
        {
            g2_decoder_video_close_all(_handle);
        }

        public static void set_mega_pixel_limitation(int factor)
        {
            g2_decoder_video_set_mega_pixel_limitation(factor);
        }
        public static int get_mega_pixel_limitation()
        {
            return g2_decoder_video_get_mega_pixel_limitation();
        }

        public bool decompress(ref G2DECODER_VIDEO_PARAM_V1 param)
        {
            return g2_decoder_video_decompress(_handle, ref param);
        }
        public bool decompress(ref G2DECODER_VIDEO_PARAM_V2 param, out G2DECODER_VIDEO_RESULT res)
        {
            return g2_decoder_video_decompress_v2(_handle, ref param, out res);
        }
        public bool decompress(ref G2DECODER_VIDEO_PARAM param, out G2DECODER_VIDEO_RESULT res)
        {
            G2DECODER_VIDEO_PARAM_V2 p = new G2DECODER_VIDEO_PARAM_V2();
            GCHandle gch_data = GCHandle.Alloc(param._buf_data, GCHandleType.Pinned);
            GCHandle gch_buf_decompress = GCHandle.Alloc(param._buf_decompress, GCHandleType.Pinned);
            GCHandle gch_buf_filter = GCHandle.Alloc(param._buf_filter, GCHandleType.Pinned);

            p._frame = param._frame;
            p._frame._index._data_size = (uint)param._buf_data.Length;
            p._frame._data = gch_data.AddrOfPinnedObject();
            p._buf_decompress = gch_buf_decompress.AddrOfPinnedObject();
            p._buf_filter = gch_buf_filter.AddrOfPinnedObject();
            p._buf_decompress_len = (uint)param._buf_decompress.Length;
            p._buf_filter_len = (uint)param._buf_filter.Length;
            p._decoder_id = param._decoder_id;
            p._threads = param._threads;
            p._format = param._format;
            p._resize = param._resize;

            bool ret = g2_decoder_video_decompress_v2(_handle, ref p, out res);
            gch_data.Free();
            gch_buf_decompress.Free();
            gch_buf_filter.Free();
            return ret;
        }
        public void set_deinterlacing_filter(int camera, G2DECODER_VIDEO_DEINTERLACE.FILTER filter)
        {
            g2_decoder_video_set_deinterlacing_filter(_handle, camera, (int)filter);
        }
        public void set_deinterlacing_filter(G2DECODER_VIDEO_DEINTERLACE.FILTER filter)
        {
            g2_decoder_video_set_deinterlacing_filter_all(_handle, (int)filter);
        }
        public G2DECODER_VIDEO_DEINTERLACE.FILTER get_deinterlacing_filter(int camera)
        {
            return (G2DECODER_VIDEO_DEINTERLACE.FILTER)g2_decoder_video_get_deinterlacing_filter(_handle, camera);
        }

        public static void YV12_to_YUY2(byte[] dst, uint linesize, byte[] src, uint cx, uint cy)
        {
            GCHandle gch_dst = GCHandle.Alloc(dst, GCHandleType.Pinned);
            GCHandle gch_src = GCHandle.Alloc(src, GCHandleType.Pinned);
            g2_color_convert_yv12_to_yuy2(gch_dst.AddrOfPinnedObject(), linesize, gch_src.AddrOfPinnedObject(), cx, cy);
            gch_dst.Free();
            gch_src.Free();
        }
        public static void YV12_to_UYVY(byte[] dst, uint linesize, byte[] src, uint cx, uint cy)
        {
            GCHandle gch_dst = GCHandle.Alloc(dst, GCHandleType.Pinned);
            GCHandle gch_src = GCHandle.Alloc(src, GCHandleType.Pinned);
            g2_color_convert_yv12_to_uyvy(gch_dst.AddrOfPinnedObject(), linesize, gch_src.AddrOfPinnedObject(), cx, cy);
            gch_dst.Free();
            gch_src.Free();
        }
        public static void YV12_to_RGB32(byte[] dst, uint linesize, byte[] src, uint cx, uint cy)
        {
            GCHandle gch_dst = GCHandle.Alloc(dst, GCHandleType.Pinned);
            GCHandle gch_src = GCHandle.Alloc(src, GCHandleType.Pinned);
            g2_color_convert_yv12_to_rgb32(gch_dst.AddrOfPinnedObject(), linesize, gch_src.AddrOfPinnedObject(), cx, cy);
            gch_dst.Free();
            gch_src.Free();
        }
        public static void YV12_to_RGB24(byte[] dst, uint linesize, byte[] src, uint cx, uint cy)
        {
            GCHandle gch_dst = GCHandle.Alloc(dst, GCHandleType.Pinned);
            GCHandle gch_src = GCHandle.Alloc(src, GCHandleType.Pinned);
            g2_color_convert_yv12_to_rgb32(gch_dst.AddrOfPinnedObject(), linesize, gch_src.AddrOfPinnedObject(), cx, cy);
            gch_dst.Free();
            gch_src.Free();
        }
        public static void YV12_to_RGB565(byte[] dst, uint linesize, byte[] src, uint cx, uint cy)
        {
            GCHandle gch_dst = GCHandle.Alloc(dst, GCHandleType.Pinned);
            GCHandle gch_src = GCHandle.Alloc(src, GCHandleType.Pinned);
            g2_color_convert_yv12_to_rgb32(gch_dst.AddrOfPinnedObject(), linesize, gch_src.AddrOfPinnedObject(), cx, cy);
            gch_dst.Free();
            gch_src.Free();
        }
        public static void YV12_to_RGB555(byte[] dst, uint linesize, byte[] src, uint cx, uint cy)
        {
            GCHandle gch_dst = GCHandle.Alloc(dst, GCHandleType.Pinned);
            GCHandle gch_src = GCHandle.Alloc(src, GCHandleType.Pinned);
            g2_color_convert_yv12_to_rgb32(gch_dst.AddrOfPinnedObject(), linesize, gch_src.AddrOfPinnedObject(), cx, cy);
            gch_dst.Free();
            gch_src.Free();
        }
        public static void resize_YV12(byte[] dst, int dw, int dh, int dzx, int dzy, int dzw, int dzh, byte[] src, int sw, int sh, int szx, int szy, int szw, int szh, G2DECODER_VIDEO_SCALER_PARAM.FILTER filter)
        {
            GCHandle gch_dst = GCHandle.Alloc(dst, GCHandleType.Pinned);
            GCHandle gch_src = GCHandle.Alloc(src, GCHandleType.Pinned);
            g2_img_resize_yv12(gch_dst.AddrOfPinnedObject(), dw, dh, dzx, dzy, dzw, dzh, gch_src.AddrOfPinnedObject(), sw, sh, szx, szy, szw, szh, (int)filter);
            gch_dst.Free();
            gch_src.Free();
        }
        public static void resize_YV12(byte[] dst, int dw, int dh, byte[] src, int sw, int sh, int szx, int szy, int szw, int szh, G2DECODER_VIDEO_SCALER_PARAM.FILTER filter)
        {
            resize_YV12(dst, dw, dh, 0, 0, dw, dh, src, sw, sh, szx, szy, szw, szh, filter);
        }
        public static void resize_RGB32(byte[] dst, int dw, int dh, int dzx, int dzy, int dzw, int dzh, byte[] src, int sw, int sh, int szx, int szy, int szw, int szh, G2DECODER_VIDEO_SCALER_PARAM.FILTER filter)
        {
            GCHandle gch_dst = GCHandle.Alloc(dst, GCHandleType.Pinned);
            GCHandle gch_src = GCHandle.Alloc(src, GCHandleType.Pinned);
            g2_img_resize_rgb32(gch_dst.AddrOfPinnedObject(), dw, dh, dzx, dzy, dzw, dzh, gch_src.AddrOfPinnedObject(), sw, sh, szx, szy, szw, szh, (int)filter);
            gch_dst.Free();
            gch_src.Free();
        }
        public static void resize_RGB32(byte[] dst, int dw, int dh, byte[] src, int sw, int sh, int szx, int szy, int szw, int szh, G2DECODER_VIDEO_SCALER_PARAM.FILTER filter)
        {
            resize_RGB32(dst, dw, dh, 0, 0, dw, dh, src, sw, sh, szx, szy, szw, szh, filter);
        }
        public static void resize_RGB24(byte[] dst, int dw, int dh, int dzx, int dzy, int dzw, int dzh, byte[] src, int sw, int sh, int szx, int szy, int szw, int szh, G2DECODER_VIDEO_SCALER_PARAM.FILTER filter)
        {
            GCHandle gch_dst = GCHandle.Alloc(dst, GCHandleType.Pinned);
            GCHandle gch_src = GCHandle.Alloc(src, GCHandleType.Pinned);
            g2_img_resize_rgb24(gch_dst.AddrOfPinnedObject(), dw, dh, dzx, dzy, dzw, dzh, gch_src.AddrOfPinnedObject(), sw, sh, szx, szy, szw, szh, (int)filter);
            gch_dst.Free();
            gch_src.Free();
        }
        public static void resize_RGB24(byte[] dst, int dw, int dh, byte[] src, int sw, int sh, int szx, int szy, int szw, int szh, G2DECODER_VIDEO_SCALER_PARAM.FILTER filter)
        {
            resize_RGB24(dst, dw, dh, 0, 0, dw, dh, src, sw, sh, szx, szy, szw, szh, filter);
        }
        public static void resize_RGB565(byte[] dst, int dw, int dh, int dzx, int dzy, int dzw, int dzh, byte[] src, int sw, int sh, int szx, int szy, int szw, int szh, G2DECODER_VIDEO_SCALER_PARAM.FILTER filter)
        {
            GCHandle gch_dst = GCHandle.Alloc(dst, GCHandleType.Pinned);
            GCHandle gch_src = GCHandle.Alloc(src, GCHandleType.Pinned);
            g2_img_resize_rgb565(gch_dst.AddrOfPinnedObject(), dw, dh, dzx, dzy, dzw, dzh, gch_src.AddrOfPinnedObject(), sw, sh, szx, szy, szw, szh, (int)filter);
            gch_dst.Free();
            gch_src.Free();
        }
        public static void resize_RGB565(byte[] dst, int dw, int dh, byte[] src, int sw, int sh, int szx, int szy, int szw, int szh, G2DECODER_VIDEO_SCALER_PARAM.FILTER filter)
        {
            resize_RGB565(dst, dw, dh, 0, 0, dw, dh, src, sw, sh, szx, szy, szw, szh, filter);
        }
        public static void resize_RGB555(byte[] dst, int dw, int dh, int dzx, int dzy, int dzw, int dzh, byte[] src, int sw, int sh, int szx, int szy, int szw, int szh, G2DECODER_VIDEO_SCALER_PARAM.FILTER filter)
        {
            GCHandle gch_dst = GCHandle.Alloc(dst, GCHandleType.Pinned);
            GCHandle gch_src = GCHandle.Alloc(src, GCHandleType.Pinned);
            g2_img_resize_rgb555(gch_dst.AddrOfPinnedObject(), dw, dh, dzx, dzy, dzw, dzh, gch_src.AddrOfPinnedObject(), sw, sh, szx, szy, szw, szh, (int)filter);
            gch_dst.Free();
            gch_src.Free();
        }
        public static void resize_RGB555(byte[] dst, int dw, int dh, byte[] src, int sw, int sh, int szx, int szy, int szw, int szh, G2DECODER_VIDEO_SCALER_PARAM.FILTER filter)
        {
            resize_RGB555(dst, dw, dh, 0, 0, dw, dh, src, sw, sh, szx, szy, szw, szh, filter);
        }
        public static bool resize_8u_v2(byte[] dst_data, int dst_linesize, byte[] src_data, int src_linesize, int bpp, int dst_roi_left, int dst_roi_top, int dst_roi_width, int dst_roi_height, int src_roi_left, int src_roi_top, int src_roi_width, int src_roi_height, G2DECODER_VIDEO_SCALER_PARAM.FILTER filter)
        {
            GCHandle gch_dst = GCHandle.Alloc(dst_data, GCHandleType.Pinned);
            GCHandle gch_src = GCHandle.Alloc(src_data, GCHandleType.Pinned);
            bool res = g2_img_resize_8u_v2(gch_dst.AddrOfPinnedObject(), dst_linesize, gch_src.AddrOfPinnedObject(), src_linesize, bpp, dst_roi_left, dst_roi_top, dst_roi_width, dst_roi_height, src_roi_left, src_roi_top, src_roi_width, src_roi_height, (int)filter);
            gch_dst.Free();
            gch_src.Free();
            return res;
        }
        public static bool resize_RGB32_v2(byte[] dst_data, int dst_linesize, byte[] src_data, int src_linesize, Rectangle dst_roi, Rectangle src_roi, G2DECODER_VIDEO_SCALER_PARAM.FILTER filter)
        {
            return resize_8u_v2(dst_data, dst_linesize, src_data, src_linesize, 4, dst_roi.Left, dst_roi.Top, dst_roi.Width, dst_roi.Height, src_roi.Left, src_roi.Top, src_roi.Width, src_roi.Height, filter);
        }
        public static bool resize_RGB32_v2(byte[] dst_data, int dst_linesize, byte[] src_data, int src_linesize, Size dst_size, Rectangle src_roi, G2DECODER_VIDEO_SCALER_PARAM.FILTER filter)
        {
            return resize_8u_v2(dst_data, dst_linesize, src_data, src_linesize, 4, 0, 0, dst_size.Width, dst_size.Height, src_roi.Left, src_roi.Top, src_roi.Width, src_roi.Height, filter);
        }
        public static bool resize_RGB32_v2(IntPtr dst_data, int dst_linesize, IntPtr src_data, int src_linesize, Size dst_size, Rectangle src_roi, G2DECODER_VIDEO_SCALER_PARAM.FILTER filter)
        {
            return g2_img_resize_8u_v2(dst_data, dst_linesize, src_data, src_linesize, 4, 0, 0, dst_size.Width, dst_size.Height, src_roi.Left, src_roi.Top, src_roi.Width, src_roi.Height, (int)filter);
        }
        public static bool get_diagnosis_codec_info(ref G2FRAME frame, out string res)
        {
            IntPtr p = IntPtr.Zero;
            bool ret = g2_decoder_get_diagnosis_codec_info_BSTR(ref frame, out p);
            res = Marshal.PtrToStringBSTR(p);
            return ret;
        }
    }

    public class g2decoder_audio
    {
        #region GDK DLL Import
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern G2HDECODER g2_decoder_audio_initialize(G2UPARAM param);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_decoder_audio_finalize(G2HDECODER handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_decoder_audio_startup(G2HDECODER handle, ref G2DECODER_AUDIO_PARAM_V1 param);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_decoder_audio_cleanup(G2HDECODER handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_decoder_audio_decompress(G2HDECODER handle, ref G2DECODER_AUDIO_PARAM_V1 param);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_decoder_audio_is_available(G2HDECODER handle);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_decoder_audio_is_dif(G2HDECODER handle, ref G2AUDIO_CODEC_INFO ci);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_decoder_audio_is_need_reset(G2HDECODER handle, ref G2AUDIO_CODEC_INFO ci);
        #endregion

        public g2decoder_audio()
        {
            _handle = g2_decoder_audio_initialize(new G2UPARAM(0));
        }
        ~g2decoder_audio()
        {
            g2_decoder_audio_finalize(_handle);
        }

        private G2HDECODER _handle;

        public bool startup(ref G2DECODER_AUDIO_PARAM param)
        {
            G2DECODER_AUDIO_PARAM_V1 p = new G2DECODER_AUDIO_PARAM_V1();
            GCHandle gch_data = GCHandle.Alloc(param._buf_data, GCHandleType.Pinned);
            p._frame = param._frame;
            p._frame._index._data_size = (uint)param._buf_data.Length;
            p._frame._data = gch_data.AddrOfPinnedObject();
            bool ret = g2_decoder_audio_startup(_handle, ref p);
            gch_data.Free();
            return ret;
        }
        public void cleanup()
        {
            g2_decoder_audio_cleanup(_handle);
        }

        public bool decompress(ref G2DECODER_AUDIO_PARAM param)
        {
            G2DECODER_AUDIO_PARAM_V1 p = new G2DECODER_AUDIO_PARAM_V1();
            GCHandle gch_data = GCHandle.Alloc(param._buf_data, GCHandleType.Pinned);
            GCHandle gch_buf_decompress = GCHandle.Alloc(param._buf_decompress, GCHandleType.Pinned);
            p._frame = param._frame;
            p._frame._index._data_size = (uint)param._buf_data.Length;
            p._frame._data = gch_data.AddrOfPinnedObject();
            p._buf_decompress = gch_buf_decompress.AddrOfPinnedObject();
            p._buf_decompress_len = (uint)param._buf_decompress.Length;
            bool ret = g2_decoder_audio_decompress(_handle, ref p);
            gch_data.Free();
            gch_buf_decompress.Free();
            return ret;
        }

        public bool is_available()
        {
            return g2_decoder_audio_is_available(_handle);
        }
        public bool is_dif(ref G2AUDIO_CODEC_INFO ci)
        {
            return g2_decoder_audio_is_dif(_handle, ref ci);
        }
        public bool is_need_reset(ref G2AUDIO_CODEC_INFO ci)
        {
            return g2_decoder_audio_is_need_reset(_handle, ref ci);
        }
    }
}
