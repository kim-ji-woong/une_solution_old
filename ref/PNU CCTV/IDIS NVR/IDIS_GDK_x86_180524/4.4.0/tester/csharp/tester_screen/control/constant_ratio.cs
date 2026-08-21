using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using GDK;

namespace GDK_tester
{
    public class constant_ratio
    {
        public static Rectangle imp(ref Rectangle dest, int cx, int cy)
        {
            if ((cx == 0 || cy == 0) ||
                (dest.Width == 0 || dest.Height == 0))
            {
                return new Rectangle();
            }

            float w = (float)(dest.Width);
            float h = (float)(dest.Bottom);
            float th = (float)(cy) / ((float)(cx) / w);
            float tw = w;

            if (th > h)
            {
                th = h;
                tw = (float)(cx) / ((float)(cy) / h);
            }

            float l = ((float)(dest.X + dest.X + dest.Width) - tw) * 0.5F;
            float t = ((float)(dest.Y + dest.Y + dest.Height) - th) * 0.5F;
            return new Rectangle((int)l, (int)t, (int)(tw + 0.5F), (int)(th + 0.5F));
        }
        public static Rectangle imp(ref Rectangle dest, ref Size size)
        {
            return imp(ref dest, size.Width, size.Height);
        }
        public static Rectangle get(ref Rectangle dest, ref Size size, bool half)
        {
            return get(ref dest, size.Width, size.Height, half);
        }
        public static Rectangle get(ref Rectangle dest, int cx, int cy, bool half)
        {
            if (half && ((cy << 1) < cx))
            {
                cy <<= 1; // consider image half(cy) mode.
            }
            return imp(ref dest, cx, cy);
        }
                
        public static int center_horz(int left, int right)
        {
            return (left + right) >> 1;
        }
        public static float center_horz(float left, float right)
        {
            return (left + right) / 2.0f;
        }

        public static int center_vert(int top, int bottom)
        {
            return (top + bottom) >> 1;
        }
        public static float center_vert(float top, float bottom)
        {
            return (top + bottom) / 2.0f;
        }

        public static int center_horz(int left, int right, int cx)
        {
            return center_horz(left, right) - (cx >> 1);
        }
        public static float center_horz(float left, float right, float cx)
        {
            return center_horz(left, right) - (cx / 2.0f);
        }

        public static int center_vert(int top, int bottom, int cy)
        {
            return center_vert(top, bottom) - (cy >> 1);
        }
        public static float center_vert(float top, float bottom, float cy)
        {
            return center_vert(top, bottom) - (cy / 2.0f);
        }

        public static Rectangle center_rect(ref Rectangle dest, int cx, int cy)
        {
            return new Rectangle(center_horz(dest.X, dest.Right, cx), center_vert(dest.Y, dest.Bottom, cy), cx, cy);
        }
    }
}
