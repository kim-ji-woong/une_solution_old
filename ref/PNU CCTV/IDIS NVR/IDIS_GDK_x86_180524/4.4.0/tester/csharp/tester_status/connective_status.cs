using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using GDK;

namespace GDK_tester
{
    using G2HSTATUS = System.Int32;

    public partial class form_status : g2status_listener
    {
        public void init_connective_status()
        {
            _adaptor = new g2status();
            _adaptor.set_listener(this);
            _adaptor.startup(2, 4);
            _channel = -1;
            _count_camera = 0;
            _timer_status = new Timer();
            _timer_status.Interval = 1000;
            _timer_status.Tick += new EventHandler(on_timer_status);
            _timer_status_instant_record = new Timer();
            _timer_status_instant_record.Interval = 1000;
            _timer_status_instant_record.Tick += new EventHandler(on_timer_status_instant_record);
        }
        public bool valid_channel(int channel)
        {
            return channel >= 0;
        }

        public void on_g2status_connected(G2HSTATUS handle, int channel)
        {
            lock (_adaptor)
            {
                if (_channel != channel)
                {
                    _adaptor.disconnect(channel);
                    return;
                }
            }

            G2_PRODUCT_INFO pi;
            if (_adaptor.get_product_info(channel, out pi))
            {
                _count_camera = pi.device.count_camera;
            }

            if (_finalize != true)
            {
                this.BeginInvoke((MethodInvoker)delegate() { on_post_status_connected(channel); });
            }
        }
        public void on_g2status_disconnected(G2HSTATUS handle, int channel, G2DISCONNECT_REASON.TYPE reason)
        {
            lock (_adaptor)
            {
                if (_channel != channel)
                {
                    return;
                }
            }
            _count_camera = 0;
            _channel = -1;

            feature_log_set_enable(false);
            feature_instant_record_set_enable(false);
            feature_health_set_enable(false);
            feature_status_set_enable(false);

            if (_finalize != true)
            {
                this.BeginInvoke((MethodInvoker)delegate() { on_post_status_disconnected(channel, reason); });
            }
        }
        public void on_g2status_receive_instant_recording_start(G2HSTATUS handle, int channel, G2INSTANT_RECORDING_RESULT.TYPE result, G2INSTANT_RECORDING_CHANNEL_STATUS[] status) { }
        public void on_g2status_receive_instant_recording_stop(G2HSTATUS handle, int channel, G2INSTANT_RECORDING_RESULT.TYPE result) { }
        public void on_g2status_receive_instant_recording_status(G2HSTATUS handle, int channel, G2INSTANT_RECORDING_RESULT.TYPE result, G2INSTANT_RECORDING_CHANNEL_STATUS[] status)
        {
            if (_finalize != true)
            {
                feature_instant_record_set_status(channel, status);
            }
        }
        public void on_g2status_receive_status(G2HSTATUS handle, int channel, ref G2MONITORING_DEVICE_STATUS status)
        {
            if (_finalize != true)
            {
                feature_health_set_status(channel, ref status);
                feature_record_panic_set_status(channel, ref status, _enable_panic);
                feature_record_status_set(channel, ref status);
                feature_status_set_status(channel, ref status);
            }
        }
        public void on_g2status_receive_callback_event(G2HSTATUS handle, ref G2EVENT_INFO info)
        {
            if (_finalize != true)
            {
                feature_callback_receive_event(ref info);
            }
        }
        public void on_g2status_receive_log_system(G2HSTATUS handle, int channel, G2STATUS_LOG_SYSTEM[] logs)
        {
            this.BeginInvoke((MethodInvoker)delegate() { on_post_feature_log_receive_system(channel, logs); });
        }
        public void on_g2status_receive_log_event(G2HSTATUS handle, int channel, G2STATUS_LOG_EVENT[] logs)
        {
            this.BeginInvoke((MethodInvoker)delegate() { on_post_feature_log_receive_event(channel, logs); });
        }
        public void on_g2status_receive_log_debug(G2HSTATUS handle, int channel, G2STATUS_LOG_SYSTEM[] logs)
        {
            this.BeginInvoke((MethodInvoker)delegate() { on_post_feature_log_receive_debug(channel, logs); });
        }

        private void on_post_status_connected(int channel)
        {
            BTN_DISCONNECT.Enabled = true;
            BTN_REMOTE_SETUP.Enabled = (File.Exists(@".\remote_setup\G2RemoteConf.exe") &&
                                        _adaptor.is_authority(channel, G2RAS_AUTHORITY.TYPE.AUTHORITY_SETUP));

            G2_PRODUCT_INFO pi;
            _adaptor.get_product_info(channel, out pi);
            _message.hide(STRING.NIS_CONNECTING);
            _timer_status.Start();

            feature_status_set_product_info(ref pi);
            feature_status_set_enable(true);
            feature_health_set_product_info(ref pi);
            feature_health_set_enable(true);
            feature_instant_record_set_product_info(ref pi);
            feature_instant_record_set_enable(_adaptor.is_support(channel, G2STATUS_SUPPORT.QUERY.SUPPORT_INSTANT_RECORDING));
            feature_log_set_product_info(ref pi);
            feature_log_set_enable(true);
            feature_record_panic_set_product_info(ref pi);
            feature_record_status_set_product_info(ref pi);
        }
        private void on_post_status_disconnected(int channel, G2DISCONNECT_REASON.TYPE reason)
        {
            _timer_status.Stop();
            _timer_status_instant_record.Stop();

            if (reason != G2DISCONNECT_REASON.TYPE.LOGOUT)
            {
                string error = g2foundation.get_string_disconnect_reason(reason);
                _message.disp(error, 5 * 1000, false);
            }
            else
            {
                _message.hide(STRING.NIS_CONNECTING);
                _message.hide(STRING.NIS_SEARCHING);
            }

            BTN_CONNECT.Enabled = true;
            BTN_DISCONNECT.Enabled = false;
            BTN_REMOTE_SETUP.Enabled = false;

            feature_log_reset();
            feature_health_reset();
            feature_instant_record_reset();
            feature_record_panic_reset();
            feature_record_status_reset();
            feature_status_reset();
        }

        private void on_timer_status(object sender, EventArgs e)
        {
            if (_finalize)
            {
                _timer_status.Stop();
            }
            else
            {
                _adaptor.request_status(_channel);
            }
        }
        private void on_timer_status_instant_record(object sender, EventArgs e)
        {
            if (_finalize)
            {
                _timer_status_instant_record.Stop();
            }
            else
            {
                _adaptor.request_instant_recording_status(_channel);
            }
        }

        private g2status _adaptor;
        public int _channel;
        public int _count_camera;
        public Timer _timer_status;
        public Timer _timer_status_instant_record;
    }
}
