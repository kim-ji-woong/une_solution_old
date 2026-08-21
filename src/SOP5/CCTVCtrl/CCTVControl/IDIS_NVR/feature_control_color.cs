#if _IDIS_NVR_
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GDK;

namespace UnE.Control.CCTVControl.IDIS_NVR
{
    public partial class IdisNvrSet
    {
        public class feature_control_color
        {
            public feature_control_color()
            {
                this.via_command = false;
                this.timer_status = new Timer();
                this.timer_status.Interval = 500;
            }
            public void start_timer_status()
            {
                timer_status.Stop();
                timer_status.Start();
            }

            public bool via_command;
            public Timer timer_status;
        }

        private feature_control_color _feature_control_color;

        private void init_control_color()
        {
            this._feature_control_color = new feature_control_color();
            this._feature_control_color.timer_status.Tick += new EventHandler(on_feature_control_color_timer_update_status);
        }

        private void on_feature_control_color_timer_update_status(object sender, EventArgs e)
        {
            if (sender == _feature_control_color.timer_status)
            {
                _feature_control_color.timer_status.Stop();

                imp_control_color_update_status(_screen.selected_pane_channelext());
            }
        }

        protected void imp_control_color_update(int channel, int camera)
        {
            bool pre = _feature_control_color.via_command;
            _feature_control_color.via_command = _adaptor.is_enable_command_control_color(channel, camera);

            if (pre != _feature_control_color.via_command)
            {
                if (_feature_control_color.via_command)
                {
                    /*CONTROL_COLOR_SLD_BRIGHT.Visible =
                    CONTROL_COLOR_SLD_CONTRAST.Visible =
                    CONTROL_COLOR_SLD_SATURATION.Visible =
                    CONTROL_COLOR_SLD_HUE.Visible = true;
                    CONTROL_COLOR_BRIGHT_DEC.Visible =
                    CONTROL_COLOR_BRIGHT_INC.Visible =
                    CONTROL_COLOR_CONTRAST_DEC.Visible =
                    CONTROL_COLOR_CONTRAST_INC.Visible =
                    CONTROL_COLOR_SATURATION_DEC.Visible =
                    CONTROL_COLOR_SATURATION_INC.Visible =
                    CONTROL_COLOR_HUE_DEC.Visible =
                    CONTROL_COLOR_HUE_INC.Visible = false;*/
                }
                else
                {
                    /*CONTROL_COLOR_BRIGHT_DEC.Visible =
                    CONTROL_COLOR_BRIGHT_INC.Visible =
                    CONTROL_COLOR_CONTRAST_DEC.Visible =
                    CONTROL_COLOR_CONTRAST_INC.Visible =
                    CONTROL_COLOR_SATURATION_DEC.Visible =
                    CONTROL_COLOR_SATURATION_INC.Visible =
                    CONTROL_COLOR_HUE_DEC.Visible =
                    CONTROL_COLOR_HUE_INC.Visible = true;
                    CONTROL_COLOR_SLD_BRIGHT.Visible =
                    CONTROL_COLOR_SLD_CONTRAST.Visible =
                    CONTROL_COLOR_SLD_SATURATION.Visible =
                    CONTROL_COLOR_SLD_HUE.Visible = false;*/
                }
            }
        }

        protected void imp_control_color_update_status(int camera)
        {
            int channel = _channel;
            if (valid_channel(channel) != true) return;
            if (_feature_control_color != null &&
                _feature_control_color.via_command)
            {
                G2LIVE_COMMAND_CONTROL_COLOR control;
                G2LIVE_COMMAND_CONTROL_COLOR_RANGE range;
                if (_adaptor.get_status_command_control_color(channel, camera, out control, out range))
                {
                    /*CONTROL_COLOR_SLD_BRIGHT.SetRange(range._min_brightness, range._max_brightness);
                    CONTROL_COLOR_SLD_CONTRAST.SetRange(range._min_contrast, range._max_contrast);
                    CONTROL_COLOR_SLD_SATURATION.SetRange(range._min_saturation, range._max_saturation);
                    CONTROL_COLOR_SLD_HUE.SetRange(range._min_hue, range._max_hue);
                    CONTROL_COLOR_SLD_BRIGHT.Value = control._brightness;
                    CONTROL_COLOR_SLD_CONTRAST.Value = control._contrast;
                    CONTROL_COLOR_SLD_SATURATION.Value = control._saturation;
                    CONTROL_COLOR_SLD_HUE.Value = control._hue;*/
                }
            }
        }

        public void feature_control_color_set_authority(ref G2RAS_AUTHORITY auth)
        {
            if (m_owner.InvokeRequired)
            {
                G2RAS_AUTHORITY param = auth;
                m_owner.BeginInvoke((MethodInvoker)delegate() { feature_control_color_set_authority(ref param); });
                return;
            }

            if (auth.is_authority(G2RAS_AUTHORITY.TYPE.AUTHORITY_COLOR_CONTROL))
            {
                feature_control_color_screen_changed_pane(_screen.selected_pane_channelext());
            }
            else
            {
                feature_control_color_set_enable(false);
            }
        }

        public void feature_control_color_screen_changed_pane(int pane)
        {
            bool enable = _screen.is_enable(pane);
            int camera = _screen.get_pane_channelext(pane);
            int channel = _channel;
            if (enable)
            {
                enable = _adaptor.is_authority(channel, G2RAS_AUTHORITY.TYPE.AUTHORITY_COLOR_CONTROL);
            }

            if (enable)
            {
                imp_control_color_update(channel, camera);
            }

            feature_control_color_set_enable(enable);
            feature_control_color_update_status(channel, camera, false);
        }

        public void feature_control_color_set_enable(bool enable)
        {
            if (m_owner.InvokeRequired)
            {
                m_owner.BeginInvoke((MethodInvoker)delegate() { feature_control_color_set_enable(enable); });
                return;
            }

            /*CONTROL_COLOR_BRIGHT_DEC.Enabled =
            CONTROL_COLOR_BRIGHT_INC.Enabled =
            CONTROL_COLOR_CONTRAST_DEC.Enabled =
            CONTROL_COLOR_CONTRAST_INC.Enabled =
            CONTROL_COLOR_SATURATION_DEC.Enabled =
            CONTROL_COLOR_SATURATION_INC.Enabled =
            CONTROL_COLOR_HUE_DEC.Enabled =
            CONTROL_COLOR_HUE_INC.Enabled =
            CONTROL_COLOR_RESET.Enabled = enable;
            CONTROL_COLOR_SLD_BRIGHT.Enabled =
            CONTROL_COLOR_SLD_CONTRAST.Enabled =
            CONTROL_COLOR_SLD_SATURATION.Enabled =
            CONTROL_COLOR_SLD_HUE.Enabled = enable;*/
        }

        public void feature_control_color_update_status(int channel, int camera, bool post_reserve)
        {
            if (_screen.selected_pane_channelext() != camera) return;
            if (_feature_control_color != null &&
                _feature_control_color.via_command != true)
            {
                m_owner.BeginInvoke((MethodInvoker)delegate()
                {
                    imp_control_color_update(channel, camera);
                });
            }

            if (post_reserve)
            {
                if (m_owner.InvokeRequired)
                {
                    m_owner.BeginInvoke((MethodInvoker)delegate()
                    {
                        _feature_control_color.start_timer_status();
                    });
                    return;
                }
                else
                {
                    _feature_control_color.start_timer_status();
                }
            }
            else
            {
                if (m_owner.InvokeRequired)
                {
                    m_owner.BeginInvoke((MethodInvoker)delegate() { feature_control_color_update_status(channel, camera, false); });
                    return;
                }

                imp_control_color_update_status(camera);
            }
        }

        public void feature_control_color_set_product_info(ref G2_PRODUCT_INFO pi)
        {
        }
    }
}
#endif