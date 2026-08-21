#if _IDIS_NVR_
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using GDK;

namespace UnE.Control.CCTVControl.IDIS_NVR
{
    public partial class IdisNvrSet
    {
        private void init_control_alarm_out()
        {
            /*Rectangle rc = new Rectangle(this.GRP_CONTROL_ALARM_OUT.Location, this.GRP_CONTROL_ALARM_OUT.Size);
            rc.Inflate(-6, -6);
            rc.Y += 8;
            rc.Height -= 4;
            _alarm_out = new form_alarm_out(this, rc);
            _alarm_out.handler_alarm_out += new System.EventHandler(this.on_feature_control_alarm_out);
            _alarm_out.handler_beep += new System.EventHandler(this.on_feature_control_beep);
            this.Controls.Add(_alarm_out);
            _alarm_out.BringToFront();*/
        }

        public void feature_control_alarm_out_set_authority(ref G2RAS_AUTHORITY auth)
        {
            if (m_owner.InvokeRequired)
            {
                G2RAS_AUTHORITY param = auth;
                m_owner.BeginInvoke((MethodInvoker)delegate() { feature_control_alarm_out_set_authority(ref param); });
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
            if (m_owner.InvokeRequired)
            {
                m_owner.BeginInvoke((MethodInvoker)delegate() { feature_control_alarm_out_set_enable(enable); });
                return;
            }

            //_alarm_out.set_enable(enable);
        }
    }
}
#endif