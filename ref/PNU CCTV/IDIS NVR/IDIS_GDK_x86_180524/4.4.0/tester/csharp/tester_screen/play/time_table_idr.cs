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
    public class time_table_IDR : time_table_base
    {
        public class internal_data
        {
            public struct element_type
            {
                public element_type(int data, Rectangle rect)
                {
                    this.data = data;
                    this.rect = rect;
                }
                public element_type(int data)
                {
                    this.data = data;
                    this.rect = new Rectangle();
                }
                public void set(int data, Rectangle rect)
                {
                    this.data = data;
                    this.rect = rect;
                }
                public void clear() { this.data = -1; }
                public bool is_cleared() { return this.data == -1; }
                public bool is_valid() { return this.data > 0; }
                public int data;
                public Rectangle rect;
            }

            public internal_data()
            {
                channelext = -1;
                elements = new List<element_type>();
            }
            public void reset()
            {
                lock (this)
                {
                    channelext = -1;
                    elements.Clear();
                }
            }

            public int channelext;
            public List<element_type> elements;
        }

        public struct bar_
        {
            public enum TYPE
            {
                NORMAL = 1,
                SENSOR,
                MOTION,
                PRE_EVENT,
                AUDIO,
                TEXT_IN,
                OBJECT_TRACKER,
                VLOSS,
                VDETECT,
                EVENT_ALL,
                VAEVENT
            };
        }

        public time_table_IDR(Control parent)
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
            byte[] minutes = new byte[1440];
            data.get_record_minute_IDR(channelext, minutes);

            int y = _options.TABLE_HEAD_HEIGHT + _options.TABLE_TIME_BOUND_HEIGHT + 5;

            internal_data.element_type element = new internal_data.element_type(-1);
            List<internal_data.element_type> buf = new List<internal_data.element_type>();

            for (int i = 0, size = 1440; i < size; ++i)
            {
                byte info = minutes[i];
                if (info > 0)
                {
                    if (element.is_cleared())
                    {
                        element.set(info, new Rectangle(i, y, 1, _options.TABLE_BAR_HEIGHT));
                        if (i != size - 1)
                        {
                            continue;
                        }
                    }

                    if (i != size - 1 &&
                        element.data == info)
                    {
                        element.rect.Width += 1;
                    }
                    else
                    {
                        buf.Add(element);
                        element.clear();
                    }
                }
                else
                {
                    if (element.is_valid())
                    {
                        buf.Add(element);
                        element.clear();
                    }
                }
            }

            lock (_internal)
            {
                _internal.channelext = channelext;
                _internal.elements = buf;
            }
        }

        public override void select_pos(int pos, bool request)
        {
            if (pos == _select_pos) return;
            if (pos < 0 || pos > _surface_size.Width) return;
            if (pos < _begin_offset - 1 || pos >= _begin_offset + _rect.Width) _select_pos = -1;
            if (_select_pos == -1)
            {
                _begin_offset = pos - _rect.Width / 2;

                if (_begin_offset < 0)
                {
                    _begin_offset = 0;
                }
                else if (_begin_offset + _rect.Width > _surface_size.Width)
                {
                    _begin_offset = _surface_size.Width - _rect.Width;
                }
            }

            _select_pos = pos;

            if (request)
            {
                if (_listener != null)
                {
                    G2SPOT spot = spot_from_pos(pos);
                    if (spot.valid)
                    {
                        _listener.on_time_table_move_to_spot(spot);
                    }
                }
            }

            present();
            return;
        }

        public override void update()
        {
            int width = Math.Max(_rect.Width, 24 * 60);
            int height = Math.Max(_rect.Height, _options.TABLE_ROW_HEIGHT + _options.TABLE_HEAD_HEIGHT + _options.TABLE_TIME_BOUND_HEIGHT);
            init_surface(width, height);
            render();
            present();

            _enable = (_internal.elements.Count != 0);

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
            return (_spot_standard._time.valid) ? _spot_standard._time.is_same_date(spot._time) : false;
        }

        public override int pos_from_spot(G2SPOT spot)
        {
            DateTime time = spot._time;
            return time.Hour * 60 + time.Minute;
        }
        public override G2SPOT spot_from_pos(int pos)
        {
            DateTime time = _spot_standard._time;
            return new G2SPOT(0, G2TIME.create(time.Year, time.Month, time.Day, pos / 60, pos % 60, 0));
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
            if (_internal.elements.Count == 0) return;

            lock (_internal)
            {
                if (_internal.elements.Count == 0) return;

                SolidBrush br = new SolidBrush(Color.Black);
                foreach (internal_data.element_type element in _internal.elements)
                {
                    Color cr = color_rec_type(element.data);
                    br.Color = cr;

                    if (element.rect.Width > 0)
                    {
                        g.FillRectangle(br, element.rect);
                    }
                }
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

            if (spot._time.is_same_minute(_spot._time))
            {
                return;
            }

            _spot = spot;

            int pos = pos_from_spot(spot);
            select_pos(pos, false);
        }

        Color color_rec_type(int type)
        {
            Color cr = Color.FromArgb(42, 56, 103);
            switch ((bar_.TYPE)type)
            {
                case bar_.TYPE.NORMAL: cr = Color.FromArgb(42, 56, 103); break;
                case bar_.TYPE.SENSOR: cr = Color.FromArgb(135, 2, 7); break;
                case bar_.TYPE.MOTION: cr = Color.FromArgb(20, 120, 20); break;
                case bar_.TYPE.PRE_EVENT: cr = Color.FromArgb(130, 134, 5); break;
                case bar_.TYPE.TEXT_IN: cr = Color.FromArgb(82, 82, 82); break;
                case bar_.TYPE.OBJECT_TRACKER: cr = Color.FromArgb(23, 138, 79); break;
                case bar_.TYPE.VLOSS: cr = Color.FromArgb(88, 0, 138); break;
                case bar_.TYPE.VDETECT: cr = Color.FromArgb(133, 44, 5); break;
                case bar_.TYPE.EVENT_ALL: cr = Color.FromArgb(108, 2, 63); break;
                case bar_.TYPE.VAEVENT: cr = Color.FromArgb(29, 29, 29); break;
            }
            return cr;
        }

        protected internal_data _internal;
    }
}
