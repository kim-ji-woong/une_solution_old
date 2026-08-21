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
    public class time_table_hour : time_table_base
    {
        public class internal_data
        {
            public internal_data() { reset(); }
            public void reset()
            {
                lock (this)
                {
                    _hours = new int[1][];
                    _hours[0] = new int[24];
                    _selected_segment = 0;
                }
            }
            public bool contains_hour(int segment_id, int hour)
            {
                lock (this)
                {
                    return (contains_segment_id(segment_id) && hour < 24) ? (_hours[segment_id][hour] == hour_type.EXIST) : false;
                }
            }
            public bool contains_hour(int hour)
            {
                lock (this)
                {
                    return contains_hour(_selected_segment, hour);
                }
            }
            public bool contains_segment_id(int id)
            {
                return (id < _hours.Length);
            }
            public int[][] _hours;
            public int _selected_segment;
        }

        public struct hour_type
        {
            public const int NONE = 0;
            public const int EXIST = 1;
            public const int EXIST_SELECTED = 2;
            public const int SEGMENT = 3;
        }

        public time_table_hour(Control parent)
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
            int[][] buf = null;
            lock (data._record_hour)
            {
                buf = new int[data._record_hour.Count][];
                for (int i = 0; i < data._record_hour.Count; ++i)
                {
                    bool[] hs = data._record_hour[i];
                    buf[i] = new int[24];

                    for (int j = 0; j < 24; ++j)
                    {
                        buf[i][j] = hs[j] ? hour_type.EXIST : hour_type.NONE;
                    }
                }
            }

            lock (_internal)
            {
                _internal._hours = buf;
                _internal._selected_segment = 0;
            }
        }

        public override void update()
        {
            int width = Math.Max(_rect.Width, 60 * 24);
            int height = Math.Max(_rect.Height, _options.TABLE_ROW_HEIGHT + _options.TABLE_HEAD_HEIGHT + _options.TABLE_TIME_BOUND_HEIGHT);
            init_surface(width, height);
            render();
            present();

            _enable = _internal._hours.Length != 0;
            _select_pos = -1;

            if (_enable)
            {
                if (_spot_selected.valid)
                {
                    select_pos(pos_from_spot(_spot_selected), false);
                }
            }
        }
        public override bool is_contains_date(G2SPOT spot)
        {
            return (_spot_standard._time.valid) ? _spot_standard._time.is_same_date(spot._time) : false;
        }

        public override void select_pos(int pos, bool request)
        {
            int hour_pos = pos / 60;
            int hour_select = _select_pos / 60;

            if (hour_pos == hour_select) return;
            if (pos < 0 || pos > _surface_size.Width) return;
            if (_internal.contains_hour(hour_pos) != true) return;
            if (pos < _begin_offset - 1 || pos >= _begin_offset + _rect.Width) _select_pos = -1;
            if (_select_pos == -1)
            {
                _begin_offset = (hour_pos * 60) - _rect.Width / 2;

                if (_begin_offset < 0)
                {
                    _begin_offset = 0;
                }
                else if (_begin_offset + _rect.Width > _surface_size.Width)
                {
                    _begin_offset = _surface_size.Width - _rect.Width;
                }
            }

            _select_pos = hour_pos * 60;

            if (request)
            {
                if (_listener != null)
                {
                    G2SPOT spot = spot_from_pos(_select_pos);
                    if (spot.valid)
                    {
                        _listener.on_time_table_move_to_spot(spot);
                    }
                }
            }

            present();
            return;
        }
        public void select_segment_id(int id)
        {
            if (_internal._selected_segment == id) return;
            if (_internal.contains_segment_id(id) != true) return;

            _internal._selected_segment = id;
            _select_pos = -1;

            update();

            int hour = 0;

            lock (_internal)
            {
                for (int i = 24 - 1; i >= 0; --i)
                {
                    if (_internal._hours[id][i] == hour_type.EXIST)
                    {
                        hour = i;
                        break;
                    }
                }
            }

            G2TIME time = _spot_selected._time;
            time -= new G2TIME_SPAN(0, time.hour, 0, 0);
            time += new G2TIME_SPAN(0, hour, 0, 0);
            G2SPOT spot = new G2SPOT(0, time);

            _listener.on_time_table_move_to_spot(spot);
        }

        public override int pos_from_spot(G2SPOT spot)
        {
            DateTime time = spot._time;
            return time.Hour * 60;
        }
        public override G2SPOT spot_from_pos(int pos)
        {
            DateTime time = _spot_standard._time;
            return new G2SPOT(0, G2TIME.create(time.Year, time.Month, time.Day, pos / 60, 0, 0));
        }

        protected override void disp_time(Graphics g)
        {
            base.disp_time(g);

            G2TIME time = _spot_standard._time;
            if (time.valid)
            {
                SolidBrush br_date = new SolidBrush(Color.FromArgb(100, 100, 100));
                Font font_7 = new Font("Microsoft Sans Serif", 7, FontStyle.Regular, GraphicsUnit.Point);
                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Near;
                g.DrawString(time.to_string_date(), font_7, br_date, new Rectangle(0, 2, _surface.Width, _options.TABLE_HEAD_HEIGHT), format);
            }
        }
        protected override void disp_time_bar(Graphics g)
        {
            if (_internal._hours.Length == 0) return;

            int[] buf;

            lock (_internal)
            {
                if (_internal._hours.Length == 0) return;

                int i = 0;
                int id = _internal._selected_segment;
                int id_pre = 0;
                int selected_hour = _spot_selected._time.hour;

                buf = new int[24];

                foreach (int[] hs in _internal._hours)
                {
                    if (id != id_pre)
                    {
                        for (i = 0; i < 24; ++i)
                        {
                            buf[i] = hs[i] == hour_type.EXIST ? hour_type.SEGMENT : hour_type.NONE;
                        }
                    }
                    id_pre = id_pre + 1;
                }

                i = 0;
                foreach (int h in _internal._hours[id])
                {
                    if (h == hour_type.EXIST)
                    {
                        buf[i] = (i == selected_hour) ? hour_type.EXIST_SELECTED : hour_type.EXIST;
                    }
                    ++i;
                }
            }

            int x = 0;
            int y = _options.TABLE_HEAD_HEIGHT + _options.TABLE_TIME_BOUND_HEIGHT;
            SolidBrush br = new SolidBrush(Color.Black);

            foreach (int val in buf)
            {
                if (val != hour_type.NONE)
                {
                    br.Color = color_rec_type(val);
                    g.FillRectangle(br, x, y + 5, 60, _options.TABLE_BAR_HEIGHT);
                }

                x += 60;
            }
        }

        protected override void disp_select_spot(Graphics g)
        {
            if (_select_pos != -1)
            {
                SolidBrush br = new SolidBrush(Color.FromArgb(255, 255, 0));
                g.FillRectangle(br, _select_pos, _options.TABLE_HEAD_HEIGHT + _options.TABLE_TIME_BOUND_HEIGHT + 8, 60, _options.TABLE_BAR_HEIGHT - 6);
            }
        }

        public override void on_screen_image_disp(ref G2FRAME frame)
        {
            G2SPOT spot = frame.spot;
            if (spot._time.valid != true)
            {
                return;
            }

            _spot_selected = spot;

            if (spot._segment == _spot._segment &&
                spot._time.is_same_hour(_spot._time))
            {
                return;
            }

            _spot = spot;

            select_pos(pos_from_spot(spot), false);
        }

        Color color_rec_type(int type)
        {
            return (type == hour_type.EXIST ||
                    type == hour_type.EXIST_SELECTED) ? Color.FromArgb(50, 67, 89) :
                   (type == hour_type.SEGMENT) ? Color.FromArgb(128, 128, 128) : Color.Black;
        }

        protected internal_data _internal;
    }
}
