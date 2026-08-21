using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GDK;

namespace UnESample
{
    public partial class FormMain
    {
        public void feature_control_audio_screen_changed_pane(int pane)
        {
            int channel = _channel;
            bool enable = valid_channel(channel) && _adaptor.is_connected(channel);
            if (enable)
            {
                GDK_tester.camera_pane cp = _screen.get_pane(pane);
                if (cp != null)
                {
                    enable = (cp._mode == GDK_tester.camera_pane.MODE.LIVE);
                }

                if (enable)
                {
                    enable = _adaptor.is_support(channel, G2LIVE_SUPPORT.QUERY.AUDIO_STREAM_IN_WATCH_PORT);
                }
            }
            feature_control_audio_set_enable(enable);
        }

        public void feature_control_audio_set_enable(bool enable)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { feature_control_audio_set_enable(enable); });
                return;
            }

            if (enable)
            {
                bool sounds = false;
                bool active = false;
                int camera = _screen.selected_pane_channelext();

                sounds = feature_control_audio_is_enable_audio_in_on_PI();
                active = feature_control_audio_is_active_audio_in(camera);
                if (active != true &&
                    sounds)
                {
                    sounds = feature_control_audio_is_enable_audio_in(camera);
                }
                //CONTROL_AUDIO_IN.Enabled = sounds;
                //CONTROL_AUDIO_IN.Checked = active;

                sounds = feature_control_audio_is_enable_audio_out_on_PI();
                active = feature_control_audio_is_active_audio_out(camera);
                if (active != true &&
                    sounds)
                {
                    sounds = feature_control_audio_is_enable_audio_out(camera);
                }
                //CONTROL_AUDIO_OUT.Enabled = sounds;
                //CONTROL_AUDIO_OUT.Checked = active;
            }
            else
            {
                /*CONTROL_AUDIO_IN.Checked =
                CONTROL_AUDIO_IN.Enabled =
                CONTROL_AUDIO_OUT.Checked =
                CONTROL_AUDIO_OUT.Enabled = enable;*/
            }
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

        public bool feature_control_audio_is_enable_audio_in(int camera)
        {
            return _adaptor.is_enable_audio_in(_channel, camera);
        }
        public bool feature_control_audio_is_active_audio_in(int camera)
        {
            return _adaptor.is_contains_audio_streaming(_channel, camera);
        }
        public bool feature_control_audio_is_enable_audio_out_on_PI()
        {
            G2_PRODUCT_INFO pi;
            if (_adaptor.get_product_info(_channel, out pi))
            {
                return (pi.remote_watch.audio_sync && (pi.remote_access.count_audio_out > 0));
            }
            return false;
        }
        public bool feature_control_audio_is_enable_audio_out(int camera)
        {
            return _adaptor.is_enable_audio_out(_channel, camera);
        }
        public bool feature_control_audio_is_active_audio_out(int camera)
        {
            return _adaptor.is_contains_audio_capturing(_channel, camera);
        }

        private void on_post_watch_receive_audio_out_not_available()
        {
            feature_control_audio_set_enable(true);
            _screen.message().disp("all sessions are being used.", 5 * 1000, false);
        }
    }
}
