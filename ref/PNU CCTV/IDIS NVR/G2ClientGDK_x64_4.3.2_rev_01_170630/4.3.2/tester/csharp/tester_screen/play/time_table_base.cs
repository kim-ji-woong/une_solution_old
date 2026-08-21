using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using GDK;

namespace GDK_tester
{
    public partial class time_table_base : Form
    {
        public class OPTIONS
        {
            public readonly int TABLE_WIDTH = 1440; // 1 day
            public readonly int TABLE_HEAD_HEIGHT = 30;
            public readonly int TABLE_TIME_BOUND_HEIGHT = 10;
            public readonly int TABLE_BAR_HEIGHT = 10;
            public readonly int TABLE_ROW_HEIGHT = 20;
        }

        public enum DRAG_MODE
        {
            OFF = 0,
            DATE_TIME = 1,
            TIME_BAR
        }

        public time_table_base(Control parent)
        {
            _options = new OPTIONS();
            _drag = DRAG_MODE.OFF;
            _drag_point = new Point();
            _listener = null;

            InitializeComponent();

            this.TopLevel = false;
            this.Parent = parent;
            this.DoubleBuffered = true;
        }
        static time_table_base()
        {
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("tester_screen.res.hand.cur"))
            {
                s_cur_hand = new Cursor(s);
            }
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("tester_screen.res.hand_finger.cur"))
            {
                s_cur_hand_finger = new Cursor(s);
            }
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("tester_screen.res.hand_grip.cur"))
            {
                s_cur_hand_grip = new Cursor(s);
            }
        }

        public void init(Rectangle rect, bool visible)
        {
            this.Location = rect.Location;
            this.Size = rect.Size;
            this.Visible = visible;
            this.BringToFront();
            reset();
        }
        protected void init_surface(int width, int height)
        {
            if (_surface == null ||
               (_surface.Width != width || _surface.Height != Height))
            {
                _surface = new Bitmap(Math.Max(_options.TABLE_WIDTH, width), height, PixelFormat.Format32bppArgb);
                _surface_OSD = new Bitmap(_surface.Width, _surface.Height, PixelFormat.Format32bppArgb);
                _surface_size = _surface.Size;
            }
        }
        protected void init_surface(Size res)
        {
            init_surface(res.Width, res.Height);
        }
        public virtual void reset()
        {
            Rectangle rect = new Rectangle();
            rect.Size = this.Size;

            _rect = rect;
            _rect_head = rect;
            _rect_head.Height = _options.TABLE_HEAD_HEIGHT + _options.TABLE_TIME_BOUND_HEIGHT;
            _rect_body = rect;
            _rect_body.Y = _rect_head.Bottom;
            _begin_offset = 0;
            _select_pos = -1;
            _enable = false;
            _spot.reset();
            _spot_standard.reset();
            _spot_selected.reset();

            init_surface(_rect.Size);
            render();
        }

        public void set_listener(time_table_listener listener)
        {
            _listener = listener;
        }
        public virtual void set_enable(bool enable)
        {
            _enable = enable;
        }
        public virtual void set_data(search_data data, int channelext) { }
        public virtual void set_spot_standard(G2SPOT spot)
        {
            _spot_standard = spot;
        }

        public void select_spot(G2SPOT spot)
        {
            select_spot(spot, true);
        }
        public void select_spot(G2SPOT spot, bool request)
        {
            select_pos(pos_from_spot(spot), request);
        }
        public virtual void select_pos(int pos)
        {
            select_pos(pos, true);
        }
        public virtual void select_pos(int pos, bool request)
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

        public virtual void update() { }
        public virtual bool is_contains_date(G2SPOT spot) { return false; }
        public virtual int pos_from_spot(G2SPOT spot) { return -1; }
        public virtual G2SPOT spot_from_pos(int pos) { return G2SPOT.INVALID_SPOT; }

        public virtual void render()
        {
            using (Graphics g = Graphics.FromImage(_surface))
            {
                render(g);
            }
        }
        public virtual void render_OSD()
        {
            using (Graphics g = Graphics.FromImage(_surface_OSD))
            {
                render_OSD(g);
            }
        }
        public virtual void render(Graphics g)
        {
            disp_back(g);
            disp_time(g);
            disp_time_bar(g);
        }
        public virtual void render_OSD(Graphics g)
        {
            g.Clear(Color.FromArgb(0, 0, 0, 0));
            disp_select_spot(g);
            disp_title(g);
        }
        public virtual void present()
        {
            Invalidate();
        }
        public virtual void present(Graphics g)
        {
            int width = Math.Max(_surface.Width, _rect.Width);
            g.DrawImageUnscaled(_surface, -_begin_offset, 0, width, _rect.Height);
            g.DrawImageUnscaled(_surface_OSD, -_begin_offset, 0, width, _rect.Height);
        }

        protected virtual void disp_back(Graphics g)
        {
            int width = Math.Max(_surface.Width, _rect.Width);

            SolidBrush br = new SolidBrush(Color.Black);
            br.Color = Color.FromArgb(230, 230, 230); g.FillRectangle(br, 0, 0, width, _rect.Height);
            br.Color = Color.FromArgb(210, 210, 210); g.FillRectangle(br, 0, 0, width, _options.TABLE_HEAD_HEIGHT);
            br.Color = Color.FromArgb(180, 180, 180); g.FillRectangle(br, 0, _options.TABLE_HEAD_HEIGHT, width, _options.TABLE_TIME_BOUND_HEIGHT);
        }

        protected virtual void disp_time(Graphics g)
        {
            Rectangle r = new Rectangle();
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Near;
            Font font = new Font("Microsoft Sans Serif", 7, FontStyle.Regular, GraphicsUnit.Point);
            SolidBrush br = new SolidBrush(Color.FromArgb(90, 90, 90));
            Pen pen = new Pen(Color.FromArgb(90, 90, 90), 1.0F);

            for (int i = 0; i < 24; ++i)
            {
                int pos = i * 60;
                string s = i.ToString("00");
                r.X = (i == 0) ? 0 : pos - 8;
                r.Y = _options.TABLE_HEAD_HEIGHT / 2;
                r.Width = 16;
                r.Height = r.Y;
                g.DrawString(s, font, br, r, format);
                g.DrawLine(pen, pos, _options.TABLE_HEAD_HEIGHT + (_options.TABLE_TIME_BOUND_HEIGHT / 2),
                                pos, _options.TABLE_HEAD_HEIGHT + (_options.TABLE_TIME_BOUND_HEIGHT / 2) + _options.TABLE_TIME_BOUND_HEIGHT / 2 - 1);
            }
        }

        protected virtual void disp_time_bar(Graphics g) { }
        protected virtual void disp_title(Graphics g) { }
        protected virtual void disp_select_spot(Graphics g)
        {
            if (_select_pos != -1)
            {
                Pen pen = new Pen(Color.FromArgb(255, 0, 0), 1.0F);
                g.DrawLine(pen, _select_pos, _rect_body.Y, _select_pos, _rect_body.Bottom);
            }
        }

        public int get_select_pos() { return _select_pos; }
        public G2SPOT get_spot() { return _spot; }
        public G2SPOT get_spot_standard() { return _spot_standard; }
        public G2SPOT get_spot_selected() { return _spot_selected; }

        public virtual void on_screen_image_disp(ref G2FRAME frame)
        {
            G2SPOT spot = frame.spot;
            if (spot._time.valid != true)
            {
                return;
            }

            _spot_selected = spot;

            if (spot._segment == _spot._segment &&
                spot._time.is_same_minute(_spot._time))
            {
                return;
            }

            _spot = spot;

            int pos = pos_from_spot(spot);
            select_pos(pos, false);
        }

        private void on_resize(object sender, EventArgs e)
        {
            Rectangle rect = new Rectangle();
            rect.Size = this.Size;

            if (_rect != rect)
            {
                _rect = rect;
                _rect_head = rect;
                _rect_head.Height = _options.TABLE_HEAD_HEIGHT + _options.TABLE_TIME_BOUND_HEIGHT;
                _rect_body = rect;
                _rect_body.Y = _rect_head.Bottom;

                if (_surface == null ||
                    _surface.Width < _rect.Width)
                {
                    _begin_offset = 0;
                }

                update();
            }
        }
        private void on_paint(object sender, PaintEventArgs e)
        {
            render_OSD();
            present(e.Graphics);
        }

        protected virtual void on_mouse_down(object sender, MouseEventArgs e)
        {
            if (_enable != true) return;
            if (_rect_head.Contains(e.Location))
            {
                _drag = DRAG_MODE.DATE_TIME;
                _drag_point = e.Location;
                this.Cursor = s_cur_hand_grip;
            }
            else if (_rect_body.Contains(e.Location))
            {
                _drag = DRAG_MODE.TIME_BAR;

                select_pos(e.X + _begin_offset);
            }
            else
            {
                return;
            }

            this.Capture = true;
        }
        protected virtual void on_mouse_up(object sender, MouseEventArgs e)
        {
            if (_enable != true) return;
            if (_drag == DRAG_MODE.TIME_BAR)
            {
                if (e.X >= _rect.Left &&
                    e.X < _rect.Right)
                {
                    select_pos(e.X + _begin_offset);

                }
            }

            _drag = DRAG_MODE.OFF;

            if (this.Capture)
            {
                this.Capture = false;
            }
        }
        protected virtual void on_mouse_move(object sender, MouseEventArgs e)
        {
            if (_enable != true) return;
            if (_rect.Contains(e.Location) != true)
            {
                if (this.Capture != true)
                {
                    return;
                }
            }

            Cursor cursor = null;
            if (_drag == DRAG_MODE.DATE_TIME)
            {
                cursor = s_cur_hand_grip;
            }
            else if (_drag == DRAG_MODE.TIME_BAR)
            {
                cursor = null;
            }
            else
            {
                if (_rect_head.Contains(e.Location))
                {
                    cursor = s_cur_hand;
                }
                else
                {
                    cursor = null;
                }
            }

            if (_drag == DRAG_MODE.DATE_TIME)
            {
                _begin_offset += (_drag_point.X - e.Location.X);
                _drag_point = e.Location;

                if (_begin_offset < 0 ||
                    _rect.Width > _surface.Width)
                {
                    _begin_offset = 0;
                }
                else if (_begin_offset + _rect.Width > _surface.Width)
                {
                    _begin_offset = _surface.Width - _rect.Width;
                }
            }
            else if (_drag == DRAG_MODE.TIME_BAR)
            {
                if (e.X >= _rect.Left &&
                    e.X < _rect.Right)
                {
                    select_pos(e.X + _begin_offset);
                }
                cursor = null;
            }

            if ((e.Button & MouseButtons.Left) != 0 &&
                (_drag == DRAG_MODE.DATE_TIME || _drag == DRAG_MODE.TIME_BAR))
            {
                present();
            }

            this.Cursor = cursor;
        }

        protected static Cursor s_cur_hand;
        protected static Cursor s_cur_hand_finger;
        protected static Cursor s_cur_hand_grip;
        protected OPTIONS _options;
        protected Rectangle _rect;
        protected Rectangle _rect_head;
        protected Rectangle _rect_body;
        protected Bitmap _surface;
        protected Bitmap _surface_OSD;
        protected Size _surface_size;
        protected DRAG_MODE _drag;
        protected Point _drag_point;
        protected int _begin_offset;
        protected int _select_pos;
        protected bool _enable;
        protected time_table_listener _listener;
        protected G2SPOT _spot;
        protected G2SPOT _spot_standard;
        protected G2SPOT _spot_selected;
    }
}
