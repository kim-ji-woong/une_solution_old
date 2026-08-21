using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using GDK;

namespace GDK_tester
{
    public partial class form_status : Form
    {
        public void init_record_panic()
        {
            CHK_RECORD_PANIC.Enabled = false;
        }
        public void feature_record_panic_set_product_info(ref G2_PRODUCT_INFO pi)
        {
            _enable_panic = pi.remote_status.remote_panic_recording;
        }
        public void feature_record_panic_reset()
        {
            _enable_panic = false;
            _loaded_status_panic = false;

            CHK_RECORD_PANIC.Checked = false;
            CHK_RECORD_PANIC.Enabled = false;
        }
        public void feature_record_panic_set_status(int channel, ref G2MONITORING_DEVICE_STATUS status, bool panic)
        {
            if (panic != true)
            {
                panic = status._support_remote_panic_recording;
            }

            panic = (panic && status.is_panic_record_not_use() != true);
            bool panic_on = (panic && status.is_panic_recording());

            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { on_post_feature_record_panic_update(panic, panic_on); });
            }
            else
            {
                on_post_feature_record_panic_update(panic, panic_on);
            }
        }

        protected void on_post_feature_record_panic_update(bool enable, bool panic_on)
        {
            if (CHK_RECORD_PANIC.Checked != panic_on ||
                _loaded_status_panic != true)
            {
                _loaded_status_panic = true;
                CHK_RECORD_PANIC.Checked = panic_on;
            }

            CHK_RECORD_PANIC.Enabled = enable;
        }

        private void on_chk_panic_record(object sender, EventArgs e)
        {
            int channel = _channel;
            if (_adaptor.is_connected(channel))
            {
                _adaptor.request_panic_record(channel, CHK_RECORD_PANIC.Checked);
            }
        }

        public bool _loaded_status_panic;
        public bool _enable_panic;
    }
}
