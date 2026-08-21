using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace GDK_tester
{
    public class button_dwell : Button
    {
        public class event_args : EventArgs
        {
            public event_args(int repeat_count)
            {
                this.repeat_count = repeat_count;
            }
            public int repeat_count;
        }

        public button_dwell()
        {
            timer = new Timer();
            timer.Tick += new EventHandler(on_timer);
            timer_repeat_start = new Timer();
            timer_repeat_start.Tick += new EventHandler(on_timer);
            handler = null;
            repeat_interval = 200;
            repeat_count = 0;
            run = false;
            pressed = false;
        }

        public void job_start()
        {
            if (run) return;

            pressed = true;

            if (repeat_interval > 0)
            {
                repeat_count = 0;

                int delay = (200 > repeat_interval) ? 200 - repeat_interval : 0;
                if (delay == 0)
                {
                    timer.Interval = repeat_interval;
                    timer.Stop();
                    timer.Start();
                }
                else
                {
                    timer_repeat_start.Interval = delay;
                    timer_repeat_start.Stop();
                    timer_repeat_start.Start();
                }
            }

            run = true;
        }

        public void job_finish()
        {
            if (run != true) return;
            if (repeat_interval > 0)
            {
                timer_repeat_start.Stop();
                timer.Stop();
                repeat_count = 0;
            }

            if (pressed)
            {
                pressed = false;
                if (handler != null)
                {
                    event_args ea = new event_args(repeat_count);
                    handler(this, ea);
                }
            }

            run = false;
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);

            timer.Dispose();
            timer_repeat_start.Dispose();
        }
        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            job_start();

            base.OnMouseDown(mevent);
        }
        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            job_finish();

            base.OnMouseUp(mevent);
        }

        private void on_timer(object sender, EventArgs e)
        {
            if (sender == timer_repeat_start)
            {
                timer_repeat_start.Stop();
                timer.Interval = repeat_interval;
                timer.Stop();
                timer.Start();
            }
            else if (sender == timer)
            {
                repeat_count++;

                if (handler != null)
                {
                    event_args ea = new event_args(repeat_count);
                    handler(this, ea);
                }
            }
        }

        public Timer timer;
        public Timer timer_repeat_start;
        public event EventHandler handler;
        public int repeat_interval;
        public int repeat_count;
        public bool run;
        public bool pressed;
    }
}
