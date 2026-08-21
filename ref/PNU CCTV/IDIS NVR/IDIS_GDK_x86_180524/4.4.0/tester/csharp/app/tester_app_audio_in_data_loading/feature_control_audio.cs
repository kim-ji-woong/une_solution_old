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
    public partial class form_watch
    {
        public void feature_control_audio_set_enable()
        {
            int channel = _channel;
            bool enable = valid_channel(channel) && _adaptor.is_connected(channel);
            if (enable)
            {
                enable = _adaptor.is_support(channel, G2LIVE_SUPPORT.QUERY.AUDIO_STREAM_IN_WATCH_PORT);
            }
            if (enable)
            {
                if (feature_control_audio_is_enable_audio_in_on_PI() == true)
                {
                    feature_control_audio_in();
                }
            }
        }
        public bool feature_control_audio_is_enable_audio_in(int camera)
        {
            return _adaptor.is_enable_audio_in(_channel, camera);
        }
        public bool feature_control_audio_is_activate_audio_in(int camera)
        {
            return _adaptor.is_contains_audio_streaming(_channel, camera);
        }
        public bool feature_control_audio_is_enable_audio_in_on_PI()
        {
            G2_PRODUCT_INFO pi;
            if (_adaptor.get_product_info(_channel, out pi))
            {
                return (pi.remote_watch.audio_sync && (pi.remote_access.count_audio_in > 0));
            }
            return false;
        }
        public void feature_control_audio_in()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { feature_control_audio_in(); });
                return;
            }
            bool active = feature_control_audio_is_activate_audio_in(0) ? false : true;
            _adaptor.set_enable_audio_streaming(_channel, 0, active);
        }


    }
}