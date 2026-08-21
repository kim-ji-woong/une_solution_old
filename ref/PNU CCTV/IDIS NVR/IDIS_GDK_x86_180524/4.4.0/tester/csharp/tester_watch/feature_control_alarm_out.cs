using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GDK;

namespace GDK_tester
{
    public partial class form_watch
    {
        public void init_control_alarm_out()
        {
            Rectangle rc = new Rectangle(this.GRP_CONTROL_ALARM_OUT.Location, this.GRP_CONTROL_ALARM_OUT.Size);
            rc.Inflate(-6, -6);
            rc.Y += 8;
            rc.Height -= 4;
            _alarm_out = new form_alarm_out(this, rc);
            _alarm_out.handler_alarm_out += new System.EventHandler(this.on_feature_control_alarm_out);
            _alarm_out.handler_beep += new System.EventHandler(this.on_feature_control_beep);
            this.Controls.Add(_alarm_out);
            _alarm_out.BringToFront();
        }
        public void feature_control_alarm_out_set_authority(ref G2RAS_AUTHORITY auth)
        {
            if (this.InvokeRequired)
            {
                G2RAS_AUTHORITY param = auth;
                this.BeginInvoke((MethodInvoker)delegate() { feature_control_alarm_out_set_authority(ref param); });
                return;
            }

            if (auth.is_authority(G2RAS_AUTHORITY.TYPE.AUTHORITY_ALARM_OUT_CONTROL))
            {
                feature_control_alarm_out_set_enable(true);
            }
            else
            {
                feature_control_alarm_out_set_enable(false);
            }
        }
        public void feature_control_alarm_out_set_enable(bool enable)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { feature_control_alarm_out_set_enable(enable); });
                return;
            }

            _alarm_out.set_enable(enable);
        }

        private void on_feature_control_alarm_out(object sender, EventArgs e)
        {
            form_alarm_out.event_args args = e as form_alarm_out.event_args;
            if (args != null)
            {
                _adaptor.set_alarm_out(_channel, args.id, args.on);
            }
        }
        private void on_feature_control_beep(object sender, EventArgs e)
        {
            form_alarm_out.event_args args = e as form_alarm_out.event_args;
            if (args != null)
            {
                _adaptor.set_beep_control(_channel, args.on);
            }
        }

        private form_alarm_out _alarm_out;
    }
}
