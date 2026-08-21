using System;
using System.Collections.Generic;
using System.Drawing;
using GDK;

namespace GDK_tester
{
    public class screen_options
    {
        public class color
        {
            public class pane
            {
                static public Color back = Color.FromArgb(30, 30, 30);
                static public Color back_stub = Color.FromArgb(50, 50, 50);
                static public Color back_no_video = Color.Blue;
                static public Color back_stream_off = Color.FromArgb(50, 50, 50);
                static public Color border = Color.FromArgb(66, 66, 66);
                static public Color border_select = Color.FromArgb(5, 159, 175);
            }

            public class message
            {
                static public Color back = Color.FromArgb(53, 53, 53);
                static public Color border = Color.FromArgb(5, 159, 175);
                static public Color text = Color.White;
                static public Color spin = Color.FromArgb(5, 159, 175);
            }
        }

        public Font font_1x1 = new Font("Microsoft Sans Serif", 14, FontStyle.Regular, GraphicsUnit.Point);
        public Font font_big = new Font("Microsoft Sans Serif", 12, FontStyle.Regular, GraphicsUnit.Point);
        public Font font = new Font("Microsoft Sans Serif", 8, FontStyle.Regular, GraphicsUnit.Point);
        public Font font_small = new Font("Microsoft Sans Serif", 7, FontStyle.Regular, GraphicsUnit.Point);
        public bool probe_perf;
    }
}
