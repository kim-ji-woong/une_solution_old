using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GDK;

namespace GDK_tester
{
    using G2HDEVICE_INFO = System.Int32;

    public partial class form_status : g2device_info_listener
    {
        public void init_connective_device_info()
        {
            _adaptor_di = new g2device_info();
            _adaptor_di.set_listener(this);
            _adaptor_di.startup(2);
            _channel_di = -1;
        }

        public void on_g2device_info_receive_product_info(G2HDEVICE_INFO handle, int channel, G2DISCONNECT_REASON.TYPE reason, ref G2_PRODUCT_INFO pi)
        {
            lock (_adaptor_di)
            {
                if (_channel_di == channel)
                {
                    _channel_di = -1;
                }
            }

            if (_finalize != true)
            {
                if (reason == G2DISCONNECT_REASON.TYPE.LOGIN_FAIL)
                {
                    this.BeginInvoke((MethodInvoker)delegate() { on_post_device_info_failed(channel, reason); });
                }
                else
                {
                    G2NETWORK_INFO ni = _ni;
                    bool idr = _adaptor_di.is_IDR(channel);
                    this.BeginInvoke((MethodInvoker)delegate() { on_post_connect_imp(ref ni, idr); });
                }
            }
        }
        public void on_g2device_info_failed(G2HDEVICE_INFO handle, int channel, G2DISCONNECT_REASON.TYPE reason)
        {
            lock (_adaptor_di)
            {
                if (_channel_di == channel)
                {
                    _channel_di = -1;
                }
            }

            if (_finalize != true)
            {
                this.BeginInvoke((MethodInvoker)delegate() { on_post_device_info_failed(channel, reason); });
            }
        }
        public void on_g2device_info_canceled(G2HDEVICE_INFO handle, int channel)
        {
            lock (_adaptor_di)
            {
                if (_channel_di == channel)
                {
                    _channel_di = -1;
                }
            }

            if (_finalize != true)
            {
                this.BeginInvoke((MethodInvoker)delegate() { on_post_device_info_canceled(channel); });
            }
        }

        private void on_post_connect_imp(ref G2NETWORK_INFO ni, bool idr)
        {
            G2CONNECT_RES res;
            lock (_adaptor)
            {
                int channel = _adaptor.connect_ras(ref ni, idr, out res);
                if (valid_channel(channel))
                {
                    BTN_DISCONNECT.Enabled = true;

                    _channel = channel;
                }
                else
                {
                    _channel = -1;

                    if (res._err_dvrns != 0)
                    {
                        string error = g2foundation.get_string_dvrns_error(res._err_dvrns);
                        _message.disp(error, 10 * 1000, false);
                    }

                    BTN_CONNECT.Enabled = true;
                }
            }
        }
        private void on_post_device_info_failed(int channel, G2DISCONNECT_REASON.TYPE reason)
        {
            if (reason != G2DISCONNECT_REASON.TYPE.LOGOUT)
            {
                string error = g2foundation.get_string_disconnect_reason(reason);
                 _message.disp(error, 5 * 1000, false);
            }
            else
            {
                _message.hide(STRING.NIS_CONNECTING);
            }

            BTN_CONNECT.Enabled = true;
            BTN_DISCONNECT.Enabled = false;
        }
        private void on_post_device_info_canceled(int channel)
        {
            _message.hide(STRING.NIS_CONNECTING);

            BTN_CONNECT.Enabled = true;
            BTN_DISCONNECT.Enabled = false;
        }

        private g2device_info _adaptor_di;
        private int _channel_di;
    }
}
