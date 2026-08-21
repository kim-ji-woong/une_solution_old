using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using GDK;

namespace GDK_tester
{
    public partial class form_watch
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

        public void init_control_color()
        {
            this._feature_control_color = new feature_control_color();
            this._feature_control_color.timer_status.Tick += new EventHandler(on_feature_control_color_timer_update_status);
        }

        public void feature_control_color_set_product_info(ref G2_PRODUCT_INFO pi)
        {
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
        public void feature_control_color_set_authority(ref G2RAS_AUTHORITY auth)
        {
            if (this.InvokeRequired)
            {
                G2RAS_AUTHORITY param = auth;
                this.BeginInvoke((MethodInvoker)delegate() { feature_control_color_set_authority(ref param); });
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
        public void feature_control_color_set_enable(bool enable)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { feature_control_color_set_enable(enable); });
                return;
            }

            CONTROL_COLOR_BRIGHT_DEC.Enabled =
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
            CONTROL_COLOR_SLD_HUE.Enabled = enable;
        }
        public void feature_control_color_update_status(int channel, int camera, bool post_reserve)
        {
            if (_screen.selected_pane_channelext() != camera) return;
            if (_feature_control_color != null &&
                _feature_control_color.via_command != true)
            {
                this.BeginInvoke((MethodInvoker)delegate()
                {
                    imp_control_color_update(channel, camera);
                });
            }

            if (post_reserve)
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke((MethodInvoker)delegate()
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
                if (this.InvokeRequired)
                {
                    this.BeginInvoke((MethodInvoker)delegate() { feature_control_color_update_status(channel, camera, false); });
                    return;
                }

                imp_control_color_update_status(camera);
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
                    CONTROL_COLOR_SLD_BRIGHT.Visible =
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
                    CONTROL_COLOR_HUE_INC.Visible = false;
                }
                else
                {
                    CONTROL_COLOR_BRIGHT_DEC.Visible =
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
                    CONTROL_COLOR_SLD_HUE.Visible = false;
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
                    CONTROL_COLOR_SLD_BRIGHT.SetRange(range._min_brightness, range._max_brightness);
                    CONTROL_COLOR_SLD_CONTRAST.SetRange(range._min_contrast, range._max_contrast);
                    CONTROL_COLOR_SLD_SATURATION.SetRange(range._min_saturation, range._max_saturation);
                    CONTROL_COLOR_SLD_HUE.SetRange(range._min_hue, range._max_hue);
                    CONTROL_COLOR_SLD_BRIGHT.Value = control._brightness;
                    CONTROL_COLOR_SLD_CONTRAST.Value = control._contrast;
                    CONTROL_COLOR_SLD_SATURATION.Value = control._saturation;
                    CONTROL_COLOR_SLD_HUE.Value = control._hue;
                }
            }
        }

        private void on_feature_control_color(object sender, EventArgs e)
        {
            G2LIVE_CAMERA_COLOR.TYPE type = G2LIVE_CAMERA_COLOR.TYPE.REVERT;
            G2LIVE_CAMERA_COLOR.VALUE val = G2LIVE_CAMERA_COLOR.VALUE.DEFAULT;

            int channel = _channel;
            int camera = _screen.selected_pane_channelext();

            if (sender == CONTROL_COLOR_RESET)
            {
                if (_feature_control_color.via_command)
                {
                    G2LIVE_COMMAND_CONTROL_COLOR color = new G2LIVE_COMMAND_CONTROL_COLOR();
                    color._change = (byte)G2LIVE_COMMAND_CONTROL_COLOR.CHANGE.TO_DEFAULT;
                    if (_adaptor.send_command_control_color(channel, camera, ref color))
                    {
                        return;
                    }
                }
            }
            else
            {
                if (sender == CONTROL_COLOR_BRIGHT_DEC) type = G2LIVE_CAMERA_COLOR.TYPE.BRIGHTNESS;
                else if (sender == CONTROL_COLOR_BRIGHT_INC) type = G2LIVE_CAMERA_COLOR.TYPE.BRIGHTNESS;
                else if (sender == CONTROL_COLOR_CONTRAST_DEC) type = G2LIVE_CAMERA_COLOR.TYPE.CONTRAST;
                else if (sender == CONTROL_COLOR_CONTRAST_INC) type = G2LIVE_CAMERA_COLOR.TYPE.CONTRAST;
                else if (sender == CONTROL_COLOR_SATURATION_DEC) type = G2LIVE_CAMERA_COLOR.TYPE.SATURATION;
                else if (sender == CONTROL_COLOR_SATURATION_INC) type = G2LIVE_CAMERA_COLOR.TYPE.SATURATION;
                else if (sender == CONTROL_COLOR_HUE_DEC) type = G2LIVE_CAMERA_COLOR.TYPE.HUE;
                else if (sender == CONTROL_COLOR_HUE_INC) type = G2LIVE_CAMERA_COLOR.TYPE.HUE;

                if (sender == CONTROL_COLOR_BRIGHT_DEC ||
                    sender == CONTROL_COLOR_CONTRAST_DEC ||
                    sender == CONTROL_COLOR_SATURATION_DEC ||
                    sender == CONTROL_COLOR_HUE_DEC)
                {
                    val = G2LIVE_CAMERA_COLOR.VALUE.DOWN;
                }
                else
                {
                    val = G2LIVE_CAMERA_COLOR.VALUE.UP;
                }
            }

            _adaptor.set_camera_color(_channel, _screen.selected_pane_channelext(), type, val);
        }
        private void on_feature_control_color_slide(object sender, EventArgs e)
        {
            TrackBar control = sender as TrackBar;
            ushort val = (ushort)control.Value;

            G2LIVE_COMMAND_CONTROL_COLOR.CHANGE change =
                                (sender == CONTROL_COLOR_SLD_BRIGHT) ? G2LIVE_COMMAND_CONTROL_COLOR.CHANGE.BRIGHTNESS :
                                (sender == CONTROL_COLOR_SLD_CONTRAST) ? G2LIVE_COMMAND_CONTROL_COLOR.CHANGE.CONTRAST :
                                (sender == CONTROL_COLOR_SLD_SATURATION) ? G2LIVE_COMMAND_CONTROL_COLOR.CHANGE.SATURATION :
                                (sender == CONTROL_COLOR_SLD_HUE) ? G2LIVE_COMMAND_CONTROL_COLOR.CHANGE.HUE : G2LIVE_COMMAND_CONTROL_COLOR.CHANGE.TO_DEFAULT;
            G2LIVE_COMMAND_CONTROL_COLOR color = new G2LIVE_COMMAND_CONTROL_COLOR();
            color._change = (byte)(change);

            int camera = _screen.selected_pane_channelext();

            G2LIVE_COMMAND_CONTROL_COLOR pre;
            G2LIVE_COMMAND_CONTROL_COLOR_RANGE range;
            if (_adaptor.get_status_command_control_color(_channel, camera, out pre, out range))
            {
                color._brightness = pre._brightness;
                color._contrast = pre._contrast;
                color._saturation = pre._saturation;
                color._hue = pre._hue;

                if (change == G2LIVE_COMMAND_CONTROL_COLOR.CHANGE.BRIGHTNESS) color._change_value = (short)(val - pre._brightness);
                else if (change == G2LIVE_COMMAND_CONTROL_COLOR.CHANGE.CONTRAST) color._change_value = (short)(val - pre._contrast);
                else if (change == G2LIVE_COMMAND_CONTROL_COLOR.CHANGE.SATURATION) color._change_value = (short)(val - pre._saturation);
                else if (change == G2LIVE_COMMAND_CONTROL_COLOR.CHANGE.HUE) color._change_value = (short)(val - pre._hue);

                if (color._change_value == 0) return;   // no change
            }

            _adaptor.send_command_control_color(_channel, camera, ref color);
        }
        private void on_feature_control_color_timer_update_status(object sender, EventArgs e)
        {
            if (sender == _feature_control_color.timer_status)
            {
                _feature_control_color.timer_status.Stop();

                imp_control_color_update_status(_screen.selected_pane_channelext());
            }
        }

        private feature_control_color _feature_control_color;
    }
}
