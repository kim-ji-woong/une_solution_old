using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace GDK_tester
{
    public class screen_format
    {
        public enum FORMAT
        {
            UNDEFINED = -1,
            LAYOUT1X1 = 0,
            LAYOUT2X2,
            LAYOUT3X3,
            LAYOUT4X3,
            LAYOUT4X4,
            LAYOUT5X4,
            LAYOUT5X5,
            LAYOUT32P,
            LAYOUT6X6,
            LAYOUT7X7,
            LAYOUT8X8,
            COUNT
        };
        public enum CHANGED
        {
            LAYOUT = 0,
            PREV,
            NEXT
        }

        public screen_format()
        {
            _panes = 64;
            _start = 0;
            _rect = new Rectangle();
            _format = FORMAT.UNDEFINED;
        }
        public screen_format(int panes)
        {
            _panes = panes;
            _start = 0;
            _rect = new Rectangle();
            _format = FORMAT.UNDEFINED;
        }

        public FORMAT format_for_panes(int panes)
        {
            if (panes > _panes) panes = _panes;

            FORMAT format = FORMAT.LAYOUT6X6;
            for (int i = 0; i < (int)FORMAT.COUNT; i++)
            {
                FORMAT f = (FORMAT)i;
                if (panes_for_format(f) >= panes)
                {
                    format = f;
                    break;
                }
            }
            return format;
        }
        public FORMAT format_for_range(int first, int last)
        {
            FORMAT format = _format;
            for (int i = 0; i < (int)FORMAT.COUNT; ++i)
            {
                FORMAT f = (FORMAT)i;
                int count = panes_for_format(f);
                if ((first / count) == (last / count))
                {
                    format = f;
                    break;
                }
            }
            return format;
        }
        public static int panes_for_format(FORMAT format)
        {
            int panes = 0;
            switch (format)
            {
                case FORMAT.LAYOUT1X1: panes = 1; break;
                case FORMAT.LAYOUT2X2: panes = 4; break;
                case FORMAT.LAYOUT3X3: panes = 9; break;
                case FORMAT.LAYOUT4X3: panes = 12; break;
                case FORMAT.LAYOUT4X4: panes = 16; break;
                case FORMAT.LAYOUT5X4: panes = 20; break;
                case FORMAT.LAYOUT5X5: panes = 25; break;
                case FORMAT.LAYOUT32P: panes = 33; break;
                case FORMAT.LAYOUT6X6: panes = 36; break;
                case FORMAT.LAYOUT7X7: panes = 49; break;
                case FORMAT.LAYOUT8X8: panes = 64; break;
            }
            return panes;
        }
        public bool valid_format(FORMAT format)
        {
            return (format > FORMAT.UNDEFINED && format < FORMAT.COUNT);
        }
        public int get_row() { return get_row(_format); }
        public int get_col() { return get_col(_format); }

        public static int get_row(FORMAT format)
        {
            int row = 0;
            switch (format)
            {
                case FORMAT.LAYOUT1X1: row = 1; break;
                case FORMAT.LAYOUT2X2: row = 2; break;
                case FORMAT.LAYOUT3X3: row = 3; break;
                case FORMAT.LAYOUT4X3: row = 3; break;
                case FORMAT.LAYOUT4X4: row = 4; break;
                case FORMAT.LAYOUT5X4: row = 4; break;
                case FORMAT.LAYOUT5X5: row = 5; break;
                case FORMAT.LAYOUT32P: row = 6; break;
                case FORMAT.LAYOUT6X6: row = 6; break;
                case FORMAT.LAYOUT7X7: row = 7; break;
                case FORMAT.LAYOUT8X8: row = 8; break;
                default: break;
            }
            return row;
        }

        public static int get_col(FORMAT format)
        {
            int col = 0;
            switch (format)
            {
                case FORMAT.LAYOUT1X1: col = 1; break;
                case FORMAT.LAYOUT2X2: col = 2; break;
                case FORMAT.LAYOUT3X3: col = 3; break;
                case FORMAT.LAYOUT4X3: col = 4; break;
                case FORMAT.LAYOUT4X4: col = 4; break;
                case FORMAT.LAYOUT5X4: col = 5; break;
                case FORMAT.LAYOUT5X5: col = 5; break;
                case FORMAT.LAYOUT32P: col = 6; break;
                case FORMAT.LAYOUT6X6: col = 6; break;
                case FORMAT.LAYOUT7X7: col = 7; break;
                case FORMAT.LAYOUT8X8: col = 8; break;
                default: break;
            }
            return col;
        }
        public bool is_format1x1() { return (_format == FORMAT.LAYOUT1X1); }
        public bool is_enable_group() { return (get_row(_format) * get_col(_format) < _panes); }

        public FORMAT rects_for_format(FORMAT format, Rectangle rect, int start, out Rectangle[] rects)
        {
            format = valid_format(format) ? format : FORMAT.LAYOUT1X1;

            int ROW = get_row(format);
            int COL = get_col(format);
            int W = rect.Width / COL;
            int H = rect.Height / ROW;
            int right, bottom, pos, left, top;

            Rectangle[] buf = new Rectangle[ROW * COL];
            {
                for (int row = 0; row < ROW; row++)
                {
                    for (int col = 0; col < COL; col++)
                    {
                        right = (col < COL - 1) ? rect.Left + (col + 1) * W : rect.Right;
                        bottom = (row < ROW - 1) ? rect.Top + (row + 1) * H : rect.Bottom;
                        left = rect.Left + col * W;
                        top = rect.Top + row * H;
                        pos = row * COL + col;
                        buf[pos] = new Rectangle(left, top, right - left, bottom - top);
                    }
                }
            }

            rects = new Rectangle[_panes];

            if (format == FORMAT.LAYOUT32P)
            {
                Point[] guide = new Point[] {
                    new Point( 0,  0), new Point( 1,  1), new Point( 2,  2), new Point( 3,  3), new Point( 4,  4), new Point( 5,  5),
                    new Point( 6,  6), new Point( 7,  7), new Point( 8,  8), new Point( 9,  9), new Point(10, 10), new Point(11, 11),
                    new Point(12, 12), new Point(13, 13), new Point(14, 14), new Point(15, 15), new Point(16, 16), new Point(17, 17),
                    new Point(18, 18), new Point(19, 19), new Point(20, 20), new Point(21, 21), new Point(22, 22), new Point(23, 23),
                    new Point(24, 24), new Point(25, 25), new Point(26, 26), new Point(27, 27), new Point(30, 30), new Point(31, 31),
                    new Point(32, 32), new Point(33, 33), new Point(28, 35)
                };

                make_screen_format(format, buf, guide, rects);
            }
            else
            {
                for (int i = 0; i < _panes; ++i)
                {
                    if (i < buf.Length)
                    {
                        rects[i] = buf[i];
                    }
                }
            }

            FORMAT format_pre = _format;

            _start = start;
            _rect = rect;
            _format = format;

            return format_pre;
        }
        public int calc_start_camera(FORMAT format, CHANGED mode, int pane, out int select_pane)
        {
            int count = Math.Max(1, panes_for_format(format));
            int page_max = (_panes % count) != 0 ? _panes / count : _panes / count - 1;
            int page = pane / count;
            int start = 0;

            select_pane = pane;

            if (count * page >= _panes) page = 0;
            if (mode == CHANGED.LAYOUT)
            {
                start = page * count;
            }
            else if (mode == CHANGED.PREV)
            {
                if (--page < 0)
                {
                    page = page_max;
                }
                start = page * count;
                select_pane = (pane % count) + (page * count);
            }
            else if (mode == CHANGED.NEXT)
            {
                if (++page > page_max)
                {
                    page = 0;
                }
                start = page * count;
                select_pane = (pane % count) + (page * count);
            }

            if (select_pane >= start + count)
            {
                select_pane = start;
            }
            return start;
        }

        protected void make_screen_format(FORMAT format, Rectangle[] buf, Point[] guide, Rectangle[] rects)
        {
            int count = panes_for_format(format);
            int index = 0;

            for (int i = 0; index < _panes; ++i)
            {
                if (index >= count)
                {
                    index = index + 1;
                    continue;
                }

                Point reg = guide[index];
                Rectangle r = new Rectangle(buf[reg.X].X, buf[reg.X].Y, buf[reg.Y].Right - buf[reg.X].Left, buf[reg.Y].Bottom - buf[reg.X].Top);
                rects[i] = r;
                index = index + 1;
            }
        }

        public int _panes;
        public int _start;
        public Rectangle _rect;
        public FORMAT _format;
    }
}
