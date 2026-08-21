using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Text;
using System.Windows.Forms;
using GDK;

namespace GDK_tester
{
    public partial class screen_message : Form
    {
        public screen_message(Control parent)
        {
            InitializeComponent();

            Rectangle rect = this.ClientRectangle;
            rect.Width = rect.Height;
            rect.Inflate(-1, -1);
            this.spin = new spin_circle();
            this.parent = parent;
            this.spin.create(screen_options.color.message.spin, new PointF((float)rect.Width / 2.0F, (float)rect.Height / 2.0F));
            this.spin_buf = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);
            this.BackColor = screen_options.color.message.back;
            this.DoubleBuffered = true;

            timer_hide = new Timer();
            timer_hide.Tick += new EventHandler(on_timer);
            timer_posistion = new Timer();
            timer_posistion.Tick += new EventHandler(on_timer);
            timer_posistion.Interval = 100;
            timer_spin = new Timer();
            timer_spin.Tick += new EventHandler(on_timer);
            timer_spin.Interval = 100;
            timer_due = new screen_message_due_timer();
            timer_due.handler += new screen_message_due_timer.handler_proc(disp);
        }

        public void disp(string message, int elapse, bool spin)
        {
            disp(0, message, elapse, spin);
        }
        public void disp(string message, int elapse, bool spin, int due_time)
        {
            disp(0, message, elapse, spin, due_time);
        }
        public void disp(int id, string message, int elapse, bool spin)
        {
            disp(id, message, elapse, spin, 0);
        }
        public void disp(int id, string message, int elapse, bool spin, int due_time)
        {
            if (parent.InvokeRequired)
            {
                on_post_disp post = new on_post_disp(disp);
                parent.BeginInvoke(post, id, message, elapse, spin, due_time);
                return;
            }

            timer_hide.Stop();
            timer_due.Stop();

            if (due_time > 0)
            {
                timer_due.set_disp(id, message, elapse, spin);
                timer_due.Interval = due_time;
                timer_due.Stop();
                timer_due.Start();
                return;
            }

            if (is_need_disp() != true)
            {
                return;
            }

            bool update = (message != this.message);
            this.id = id;
            this.message = message;
            this.spinning = spin;

            if (update)
            {
                Invalidate();
            }

            if (elapse > 0)
            {
                timer_hide.Interval = elapse;
                timer_hide.Stop();
                timer_hide.Start();
            }

            timer_posistion.Start();
            imp_position();
            this.Show();

            if (spin)
            {
                timer_spin.Start();
            }
            else
            {
                timer_spin.Stop();
            }
        }
        public void hide()
        {
            if (parent.InvokeRequired)
            {
                on_post_hide post = new on_post_hide(hide);
                parent.BeginInvoke(post);
                return;
            }

            timer_hide.Stop();
            timer_posistion.Stop();
            timer_spin.Stop();
            timer_due.Stop();
            this.Hide();
        }
        public void hide(int id)
        {
            if (id < 0 ||
                id == this.id)
            {
                hide();
            }
            else
            {
                if (timer_due.id == id)
                {
                    timer_due.Stop();
                }
            }
        }
        public void imp_position()
        {
            Rectangle r = parent.RectangleToScreen(parent.DisplayRectangle);
            Point point = new Point(r.X + r.Width / 2 - this.Size.Width / 2, r.Y + r.Height / 2 - this.Size.Height / 2);
            this.Location = point;
        }
        public bool is_need_disp()
        {
            if (parent.Visible != true)
            {
                return false;
            }
            return true;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            hide();
            base.OnFormClosed(e);
        }
        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        private void on_paint(object sender, PaintEventArgs e)
        {
            Rectangle rect = new Rectangle(0, 0, this.Size.Width - 1, this.Size.Height - 1);
            Graphics g = e.Graphics;
            g.SetClip(e.ClipRectangle);
            StringFormat fmt = new StringFormat();
            g.Clear(screen_options.color.message.back);
            g.DrawRectangle(new Pen(screen_options.color.message.border), rect);

            if (spinning)
            {
                using (Graphics g_spin = Graphics.FromImage(spin_buf))
                {
                    g_spin.Clear(screen_options.color.message.back);
                    spin.render(g_spin);
                }

                g.DrawImage(spin_buf, 1, 1);
                rect.X = this.Size.Height + 4;
                rect.Width -= rect.X;
                fmt.Alignment = StringAlignment.Near;
            }
            else
            {
                fmt.Alignment = StringAlignment.Center;
            }

            fmt.LineAlignment = StringAlignment.Center;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.DrawString(message, this.Font, new SolidBrush(screen_options.color.message.text), rect, fmt);
            g.ResetClip();
        }
        private void on_click(object sender, EventArgs e)
        {
            if (spinning != true) hide();
        }
        private void on_timer(object sender, EventArgs e)
        {
            if (sender == timer_hide)
            {
                hide();
            }
            else if (sender == timer_posistion)
            {
                if (is_need_disp())
                {
                    imp_position();
                }
            }
            else if (sender == timer_spin)
            {
                spin.update();
                Rectangle rect = this.ClientRectangle;
                rect.Width = rect.Height;
                rect.Inflate(-1, -1);
                Invalidate(rect);
            }
        }

        private spin_circle spin;
        private string message;
        private int id;
        private bool spinning;
        private Control parent;
        private Bitmap spin_buf;
        private Timer timer_hide;
        private Timer timer_posistion;
        private Timer timer_spin;
        private screen_message_due_timer timer_due;
        private delegate void on_post_disp(int id, string message, int elapse, bool spin, int due_time);
        private delegate void on_post_hide();
    }

    public class screen_message_due_timer : Timer
    {
        public void set_disp(int id, string message, int elapse, bool spin)
        {
            this.message = message;
            this.id = id;
            this.elapse = elapse;
            this.spin = spin;
        }

        protected override void OnTick(EventArgs e)
        {
            if (handler != null)
            {
                handler(id, message, elapse, spin);
            }
            else
            {
                base.OnTick(e);
            }
            Stop();
        }

        public string message;
        public int id;
        public int elapse;
        public bool spin;
        public delegate void handler_proc(int id, string message, int elapse, bool spin);
        public event handler_proc handler;
    }

    public class scoped_screen_message : IDisposable
    {
        public scoped_screen_message(screen_message control, int id, string message, int elapse, bool spin)
        {
            this.control = control;
            this.control.disp(id, message, elapse, spin);
        }
        public void Dispose()
        {
            this.control.hide();
        }

        screen_message control;
    }
}
