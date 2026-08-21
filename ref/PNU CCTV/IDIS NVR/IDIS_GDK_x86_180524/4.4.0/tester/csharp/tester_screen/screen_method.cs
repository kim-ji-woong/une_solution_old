using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using GDK;

namespace GDK_tester
{
    public class screen_method
    {
        public static void imp_disp_image_G2(Bitmap buf, Bitmap surf_image, ref Rectangle dst_rect, ref Rectangle src_rect, byte[] image, int width, int height, bool interpolation)
        {
            BitmapData data = surf_image.LockBits(dst_rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
            GCHandle gch = GCHandle.Alloc(image, GCHandleType.Pinned);
            // because position already been determined at LockBits, (0, 0) instead of (dst_rect.X, Y)
            g2decoder.g2_img_resize_rgb32(data.Scan0, data.Stride / 4, dst_rect.Height,
                                          0, 0, dst_rect.Width, dst_rect.Height,
                                          gch.AddrOfPinnedObject(), width, height,
                                          src_rect.X, src_rect.Y, src_rect.Width, src_rect.Height, (int)G2DECODER_VIDEO_SCALER_PARAM.FILTER.BILINEAR);
            gch.Free();
            surf_image.UnlockBits(data);
        }

        public static void imp_disp_image_GDI(Bitmap buf, Bitmap surf_image, ref Rectangle dst_rect, ref Rectangle src_rect, byte[] image, int width, int height, bool interpolation)
        {
            lock (buf)
            {
                BitmapData data = buf.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
                IntPtr ptr = data.Scan0;
                for (int i = 0, len = width * 4; i < height; ++i)
                {
                    Marshal.Copy(image, i * len, ptr, len);
                    ptr = new IntPtr(data.Scan0.ToInt32() + data.Stride * i);
                }
                buf.UnlockBits(data);

                using (Graphics g = Graphics.FromImage(surf_image))
                {
                    InterpolationMode pre = g.InterpolationMode;
                    g.InterpolationMode = interpolation ? InterpolationMode.Bilinear : InterpolationMode.NearestNeighbor;
                    g.DrawImage(buf, dst_rect, src_rect, GraphicsUnit.Pixel);
                    g.InterpolationMode = pre;
                }
            }
        }

        public delegate void fun_disp_image(Bitmap buf, Bitmap surf_image, ref Rectangle dst_rect, ref Rectangle src_rect, byte[] image, int width, int height, bool interpolation);
    }
}
