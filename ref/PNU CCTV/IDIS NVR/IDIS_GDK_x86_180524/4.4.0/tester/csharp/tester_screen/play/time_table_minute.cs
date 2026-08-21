using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using GDK;

namespace GDK_tester
{
    public class time_table_minute : time_table_base
    {
        public class internal_data
        {
            public struct hour_data
            {
                public hour_data(DateTime hour, int i, int segment)
                {
                    this.hour = hour;
                    this.i = i;
                    this.segment = segment;
                    this.padding = false;
                }
                public DateTime hour;
                public int i;
                public int segment;
                public bool padding;
            }

            public internal_data()
            {
                channelext = -1;
                hours = new hour_data[0];
                minutes = new search_minute_info.element_type[0];
            }
            public void reset()
            {
                channelext = -1;
                Array.Resize(ref hours, 0);
                Array.Resize(ref minutes, 0);
            }

            public int channelext;
            public hour_data[] hours;
            public search_minute_info.element_type[] minutes;
        }

        public time_table_minute(Control parent)
            : base(parent)
        {
            _internal = new internal_data();
        }

        public override void reset()
        {
            base.reset();

            lock (_internal)
            {
                _internal.reset();
            }
        }

        public override void set_data(search_data data, int channelext)
        {
            search_minute_info.element_type[] minutes;
            {
                List<search_minute_info.element_type> buf = new List<search_minute_info.element_type>();
                data._minute.sort();
                data._minute.get(channelext, buf);
                minutes = buf.ToArray();
            }

            internal_data.hour_data[] hours;
            {
                int segment = -1;
                int i = 0;
                DateTime time = new DateTime();
                List<internal_data.hour_data> buf = new List<internal_data.hour_data>();
                foreach (search_minute_info.element_type node in minutes)
                {
                    DateTime t = node.spot._time;
                    int s = (int)node.spot._segment;
                    if (s != segment ||
                        t != time)
                    {
                        buf.Add(new internal_data.hour_data(t, i++, s));
                        time = t;
                        segment = s;
                    }
                }

                if (buf.Count < 24)
                {
                    for (int j = time.Hour, t = 1; j < 24 - 1; ++j, ++t)
                    {
                        internal_data.hour_data temp = new internal_data.hour_data(time.AddHours(t), i++, segment);
                        temp.padding = true;
                        buf.Add(temp);
                    }
                }

                hours = buf.ToArray();
            }

            lock (_internal)
            {
                _internal.channelext = channelext;
                _internal.hours = hours;
                _internal.minutes = minutes;
            }
        }

        public override void update()
        {
            int width = Math.Max(Math.Max(_rect.Width, 60 * 24), _internal.hours.Length * 60);
            int height = Math.Max(_rect.Height, _options.TABLE_ROW_HEIGHT + _options.TABLE_HEAD_HEIGHT + _options.TABLE_TIME_BOUND_HEIGHT);
            init_surface(width, height);
            render();
            present();

            _enable = (_internal.hours.Length != 0);

            if (_enable)
            {
                if (_spot_selected.valid)
                {
                    int pos = pos_from_spot(_spot_selected);
                    if (pos != _select_pos)
                    {
                        _select_pos = -1;
                    }

                    select_pos(pos, false);
                }
            }
        }

        public override bool is_contains_date(G2SPOT spot)
        {
            bool ret = false;
            lock (_internal)
            {
                DateTime date = spot._time;
                foreach (internal_data.hour_data data in _internal.hours)
                {
                    if (data.segment == spot._segment &&
                        data.padding != true &&
                        data.hour.Date == date.Date)
                    {
                        ret = true;
                        break;
                    }
                }
            }
            return ret;
        }

        public override int pos_from_spot(G2SPOT spot)
        {
            int pos = -1;
            if (spot.valid)
            {
                DateTime time = spot._time;
                lock (_internal)
                {
                    int i = -1;
                    foreach (internal_data.hour_data h in _internal.hours)
                    {
                        if (spot._segment == h.segment &&
                            time.Hour == h.hour.Hour &&
                            time.Date == h.hour.Date)
                        {
                            i = h.i;
                            break;
                        }
                    }

                    if (i != -1)
                    {
                        pos = (i * 60) + time.Minute % 60;
                    }
                }
            }
            return pos;
        }
        public override G2SPOT spot_from_pos(int pos)
        {
            if (pos >= _internal.minutes.Length ||
                pos >= _surface.Width ||
                pos <  0)
            {
                return base.spot_from_pos(pos);
            }

            G2SPOT spot = _internal.minutes[pos].spot;
            DateTime time = spot._time;
            spot._time.set(time.Year, time.Month, time.Day, time.Hour, pos % 60, 0);
            return spot;
        }

        protected override void disp_time(Graphics g)
        {
            if (_internal.hours.Length == 0)
            {
                base.disp_time(g);
                return;
            }

            lock (_internal)
            {
                Rectangle r = new Rectangle();
                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Near;
                Font font_7 = new Font("Microsoft Sans Serif", 7, FontStyle.Regular, GraphicsUnit.Point);
                SolidBrush br = new SolidBrush(Color.FromArgb(80, 80, 80));
                SolidBrush br_date = new SolidBrush(Color.FromArgb(100, 100, 100));
                Pen pen = new Pen(Color.FromArgb(90, 90, 90), 1.0F);
                DateTime time_pre = new DateTime();

                foreach (internal_data.hour_data data in _internal.hours)
                {
                    int pos = data.i * 60;
                    string s = data.hour.Hour.ToString("00");
                    r.X = (data.i == 0) ? 0 : pos - 8;
                    r.Y = _options.TABLE_HEAD_HEIGHT / 2;
                    r.Width = 16;
                    r.Height = r.Y;
                    g.DrawString(s, font_7, br, r, format);
                    g.DrawLine(pen, pos, _options.TABLE_HEAD_HEIGHT + (_options.TABLE_TIME_BOUND_HEIGHT / 2),
                                    pos, _options.TABLE_HEAD_HEIGHT + (_options.TABLE_TIME_BOUND_HEIGHT / 2) + _options.TABLE_TIME_BOUND_HEIGHT / 2 - 1);

                    if (time_pre != data.hour.Date)
                    {
                        time_pre = data.hour.Date;
                        G2TIME t = data.hour.Date;
                        Point pt = new Point(data.i * 60, 2);
                        g.DrawString(t.to_string_date(), font_7, br_date, pt);
                    }
                }
            }
        }
        protected override void disp_time_bar(Graphics g)
        {
            if (_internal.hours.Length == 0) return;

            lock (_internal)
            {
                if (_internal.hours.Length == 0) return;

                int y = _options.TABLE_HEAD_HEIGHT + _options.TABLE_TIME_BOUND_HEIGHT;
                int w = Math.Max(_rect.Width, _surface.Width);

                SolidBrush br = new SolidBrush(Color.Black);
                br.Color = Color.FromArgb(200, 200, 200);
                g.FillRectangle(br, 0, y, w, _options.TABLE_ROW_HEIGHT);

                int x = 0, cx = 0;
                G2FRAME.FLAG type = G2FRAME.FLAG.UNDEFINED;
                G2FRAME.FLAG type_pre = G2FRAME.FLAG.UNDEFINED;

                foreach (search_minute_info.element_type data in _internal.minutes)
                {
                    type = data.record_type;
                    if (cx > 0 && type != type_pre)
                    {
                        br.Color = color_rec_type(type_pre);
                        g.FillRectangle(br, x, y + 5, cx, _options.TABLE_BAR_HEIGHT);
                        x += cx;
                        cx = 0;
                    }

                    if (type > 0)
                    {
                        ++cx;
                    }
                    else
                    {
                        ++x;
                    }
                    type_pre = type;
                }

                if (cx > 0)
                {
                    br.Color = color_rec_type(type_pre);
                    g.FillRectangle(br, x, y + 5, cx, _options.TABLE_BAR_HEIGHT);

                    x += cx;
                    cx = 0;
                }

                br.Color = Color.FromArgb(100, 255, 255, 0);
                int segment = -1;
                foreach (internal_data.hour_data data in _internal.hours)
                {
                    if (segment == -1) segment = data.segment;
                    if (segment != data.segment)
                    {
                        segment = data.segment;

                        int pos = data.i * 60;
                        g.FillRectangle(br, pos, y, 3, _surface.Height - y);
                    }
                }
            }
        }

        Color color_rec_type(G2FRAME.FLAG type)
        {
            if ((type & G2FRAME.FLAG.PANIC) != 0) return Color.FromArgb(218, 26, 75);
            else if ((type & G2FRAME.FLAG.PRE_EVENT) != 0) return Color.FromArgb(255, 174, 1);
            else if ((type & G2FRAME.FLAG.EVENT) != 0) return Color.FromArgb(91, 41, 229);
            else if ((type & G2FRAME.FLAG.TIME_LAPSE) != 0) return Color.FromArgb(41, 144, 229);
            else if ((type & G2FRAME.FLAG.IRREGULAR) != 0) return Color.FromArgb(5, 184, 13);
            return Color.Black;
        }

        protected internal_data _internal;
    }
}
