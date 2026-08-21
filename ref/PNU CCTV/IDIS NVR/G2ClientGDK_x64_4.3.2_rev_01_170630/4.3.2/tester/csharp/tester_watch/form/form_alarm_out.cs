using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GDK;

namespace GDK_tester
{
    public partial class form_alarm_out : Form
    {
        public class feature_alarm_out
        {
            public feature_alarm_out()
            {
                reset();
            }
            public void reset()
            {
                count = 4;
                page = 1;
                page_current = 0;
                enable = false;
                status = new sbyte[count];
            }
            public bool is_enable(int id)
            {
                return (enable != true) ? false :
                       (id >= 0 && id < status.Length) ? status[id] > (sbyte)G2DEVICE_STATUS.COMMON.INACTIVE : false;
            }
            public int count;
            public int page;
            public int page_current;
            public bool enable;
            public sbyte[] status;
            public Control[] CONTROL_LABEL;
            public Control[] CONTROL_ON;
            public Control[] CONTROL_OFF;
        }

        public class event_args : EventArgs
        {
            public event_args(int id, bool on)
            {
                this.id = id;
                this.on = on;
            }
            public int id;
            public bool on;
        }

        public form_alarm_out(Control parent, Rectangle rect)
        {
            InitializeComponent();

            this.TopLevel = false;
            this.Visible = true;
            this.Location = rect.Location;
            this.Size = rect.Size;
            this.Parent = parent;

            _feature_alarm_out = new feature_alarm_out();
            _feature_alarm_out.CONTROL_LABEL = new Control[] { STC_01, STC_02, STC_03, STC_04 };
            _feature_alarm_out.CONTROL_ON = new Control[] { BTN_01_ON, BTN_02_ON, BTN_03_ON, BTN_04_ON };
            _feature_alarm_out.CONTROL_OFF = new Control[] { BTN_01_OFF, BTN_02_OFF, BTN_03_OFF, BTN_04_OFF };

            foreach (Control c in this.Controls)
            {
                c.Enabled = false;
            }
        }
        public void reset()
        {
            _feature_alarm_out.reset();

            set_alarm_out_count(4);
        }
        public void set_enable(bool enable)
        {
            _feature_alarm_out.enable = enable;

            if (enable)
            {
                set_page(_feature_alarm_out.page_current);
            }
            else
            {
                foreach (Control c in this.Controls)
                {
                    c.Enabled = false;
                }
            }
        }
        public void set_alarm_out_count(int count)
        {
            _feature_alarm_out.count = count;
            _feature_alarm_out.page = (count % 4) != 0 ? count / 4 + 1 : count / 4;
            _feature_alarm_out.page_current = 0;

            set_page(0);
        }
        public void set_alarm_out_status(sbyte[] status)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { set_alarm_out_status(status); });
                return;
            }

            _feature_alarm_out.status = status;
            set_page(_feature_alarm_out.page_current);
        }
        public void set_beep_control_enable(bool enable)
        {
            STC_BEEP.Enabled =
            BTN_BEEP_ON.Enabled =
            BTN_BEEP_OFF.Enabled = enable && _feature_alarm_out.enable;
        }
        public void set_page(int page)
        {
            _feature_alarm_out.page_current = page;

            int left = page * 4;

            for (int i = 0; i < 4; ++i)
            {
                int id = left + i;

                _feature_alarm_out.CONTROL_LABEL[i].Visible =
                _feature_alarm_out.CONTROL_ON[i].Visible =
                _feature_alarm_out.CONTROL_OFF[i].Visible = id < _feature_alarm_out.count;

                if (_feature_alarm_out.count > id)
                {
                    _feature_alarm_out.CONTROL_LABEL[i].Text = "A" + (id + 1);
                    _feature_alarm_out.CONTROL_LABEL[i].Enabled =
                    _feature_alarm_out.CONTROL_ON[i].Enabled =
                    _feature_alarm_out.CONTROL_OFF[i].Enabled = _feature_alarm_out.is_enable(id);
                }
            }

            if (_feature_alarm_out.enable)
            {
                BTN_NEXT.Enabled = (_feature_alarm_out.page_current + 1 < _feature_alarm_out.page);
                BTN_PREV.Enabled = (_feature_alarm_out.page_current > 0);
            }
        }

        private void on_feature_control_alarm_out(object sender, EventArgs e)
        {
            Button b = sender as Button;
            bool on = b.Name.Contains("ON");
            int id = int.Parse(b.Name.Substring(4, 2)) - 1 + _feature_alarm_out.page_current * 4;

            if (handler_alarm_out != null)
            {
                handler_alarm_out(this, new event_args(id, on));
            }
        }
        private void on_feature_control_beep(object sender, EventArgs e)
        {
            Button b = sender as Button;
            if (handler_alarm_out != null)
            {
                handler_beep(this, new event_args(0, b.Name.Contains("ON")));
            }
        }

        public feature_alarm_out _feature_alarm_out;
        public event EventHandler handler_alarm_out;
        public event EventHandler handler_beep;

        private void on_btn_page_prev(object sender, EventArgs e)
        {
            int page = Math.Max(_feature_alarm_out.page_current - 1, 0);
            set_page(page);
        }
        private void on_btn_page_next(object sender, EventArgs e)
        {
            int page = Math.Min(_feature_alarm_out.page_current + 1, _feature_alarm_out.page - 1);
            set_page(page);
        }
    }
}
