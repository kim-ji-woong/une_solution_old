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
        public class feature_PTZ
        {
            public enum PARAM
            {
                RELEASE = 0,
                STEP,
                HOLD
            }

            public feature_PTZ()
            {
                this.pane = -1;
                this.on_hold = false;
                this.on_OSD = false;
            }
            public void reset()
            {
                pane = -1;
                on_hold = false;
                on_OSD = false;
            }

            public int pane;
            public bool on_hold;
            public bool on_OSD;
            /*public form_ptz_preset form_preset;
            public form_ptz_menu form_menu;
            public form_ptz_command form_command;*/
        }

        private feature_PTZ _feature_PTZ = null;

        private void init_control_ptz()
        {
            this._feature_PTZ = new feature_PTZ();
            /*this.CONTROL_PTZ_MOVE_NW.Tag = G2LIVE_PTZ_COMMAND.COMMAND.PTZ_MOVE_NW;
            this.CONTROL_PTZ_MOVE_N.Tag = G2LIVE_PTZ_COMMAND.COMMAND.PTZ_MOVE_N;
            this.CONTROL_PTZ_MOVE_NE.Tag = G2LIVE_PTZ_COMMAND.COMMAND.PTZ_MOVE_NE;
            this.CONTROL_PTZ_MOVE_W.Tag = G2LIVE_PTZ_COMMAND.COMMAND.PTZ_MOVE_W;
            this.CONTROL_PTZ_MOVE_E.Tag = G2LIVE_PTZ_COMMAND.COMMAND.PTZ_MOVE_E;
            this.CONTROL_PTZ_MOVE_SW.Tag = G2LIVE_PTZ_COMMAND.COMMAND.PTZ_MOVE_SW;
            this.CONTROL_PTZ_MOVE_S.Tag = G2LIVE_PTZ_COMMAND.COMMAND.PTZ_MOVE_S;
            this.CONTROL_PTZ_MOVE_SE.Tag = G2LIVE_PTZ_COMMAND.COMMAND.PTZ_MOVE_SE;
            this.CONTROL_PTZ_ZOOM_INC.Tag = G2LIVE_PTZ_COMMAND.COMMAND.PTZ_ZOOM_IN;
            this.CONTROL_PTZ_ZOOM_DEC.Tag = G2LIVE_PTZ_COMMAND.COMMAND.PTZ_ZOOM_OUT;
            this.CONTROL_PTZ_IRIS_INC.Tag = G2LIVE_PTZ_COMMAND.COMMAND.PTZ_IRIS_OPEN;
            this.CONTROL_PTZ_IRIS_DEC.Tag = G2LIVE_PTZ_COMMAND.COMMAND.PTZ_IRIS_CLOSE;
            this.CONTROL_PTZ_FOCUS_INC.Tag = G2LIVE_PTZ_COMMAND.COMMAND.PTZ_FOCUS_NEAR;
            this.CONTROL_PTZ_FOCUS_DEC.Tag = G2LIVE_PTZ_COMMAND.COMMAND.PTZ_FOCUS_FAR;*/
        }

        public void feature_control_PTZ_set_authority(ref G2RAS_AUTHORITY auth)
        {
            if (auth.is_authority(G2RAS_AUTHORITY.TYPE.AUTHORITY_PTZ_CONTROL))
            {
                feature_control_PTZ_screen_changed_pane(_screen.selected_pane());
            }
            else
            {
                feature_control_PTZ_set_enable(false);
            }
        }

        public void feature_control_PTZ_set_enable(bool enable)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { feature_control_PTZ_set_enable(enable); });
                return;
            }

            if (enable)
            {
                G2DEVICE_STATUS.PTZ_FUNCTION func;
                int pane = _screen.selected_pane();
                int channel = _channel;
                int camera = _screen.get_pane_channelext(pane);
                int stream_id = _screen.get_pane_stream_id(pane);

                G2DEVICE_STATUS_STREAM_INFO si;
                bool roi_stream = false;
                if (_adaptor.get_status_stream_info(channel, camera, stream_id, out si))
                {
                    roi_stream = si._roi;
                }

                if (roi_stream)
                {
                    func = _adaptor.get_status_stream_eptz_function(channel, camera, stream_id);
                }
                else
                {
                    func = _adaptor.get_status_ptz_function(channel, camera);
                }

                /*CONTROL_PTZ_MOVE_NW.Enabled =
                CONTROL_PTZ_MOVE_N.Enabled =
                CONTROL_PTZ_MOVE_NE.Enabled =
                CONTROL_PTZ_MOVE_W.Enabled =
                CONTROL_PTZ_MOVE_E.Enabled =
                CONTROL_PTZ_MOVE_SW.Enabled =
                CONTROL_PTZ_MOVE_S.Enabled =
                CONTROL_PTZ_MOVE_SE.Enabled = (func & G2DEVICE_STATUS.PTZ_FUNCTION.PAN_TILT) != 0;
                CONTROL_PTZ_ZOOM_DEC.Enabled =
                CONTROL_PTZ_ZOOM_INC.Enabled = (func & G2DEVICE_STATUS.PTZ_FUNCTION.ZOOM) != 0;
                CONTROL_PTZ_FOCUS_DEC.Enabled =
                CONTROL_PTZ_FOCUS_INC.Enabled = (func & G2DEVICE_STATUS.PTZ_FUNCTION.FOCUS) != 0;
                CONTROL_PTZ_IRIS_DEC.Enabled =
                CONTROL_PTZ_IRIS_INC.Enabled = (func & G2DEVICE_STATUS.PTZ_FUNCTION.IRIS) != 0;
                CONTROL_PTZ_PRESET_MOVE.Enabled = (func & G2DEVICE_STATUS.PTZ_FUNCTION.PRESET_MOVE) != 0;
                CONTROL_PTZ_PRESET_SAVE.Enabled = (func & G2DEVICE_STATUS.PTZ_FUNCTION.PRESET_SETUP) != 0;
                CONTROL_PTZ_ONE_PUSH.Enabled = (func & G2DEVICE_STATUS.PTZ_FUNCTION.FOCUS_ONEPUSH) != 0;
                CONTROL_PTZ_MENU.Enabled = (func & G2DEVICE_STATUS.PTZ_FUNCTION.ADVANCED) != 0;
                CONTROL_PTZ_OSD.Enabled = (func & G2DEVICE_STATUS.PTZ_FUNCTION.OSD_MENU) != 0;
                CONTROL_PTZ_COMMAND_CONTROL.Enabled = _adaptor.is_enable_command_control_ptz(channel, camera);*/
            }
            else
            {
                /*CONTROL_PTZ_MOVE_NW.Enabled =
                CONTROL_PTZ_MOVE_N.Enabled =
                CONTROL_PTZ_MOVE_NE.Enabled =
                CONTROL_PTZ_MOVE_W.Enabled =
                CONTROL_PTZ_MOVE_E.Enabled =
                CONTROL_PTZ_MOVE_SW.Enabled =
                CONTROL_PTZ_MOVE_S.Enabled =
                CONTROL_PTZ_MOVE_SE.Enabled =
                CONTROL_PTZ_ZOOM_DEC.Enabled =
                CONTROL_PTZ_ZOOM_INC.Enabled =
                CONTROL_PTZ_FOCUS_DEC.Enabled =
                CONTROL_PTZ_FOCUS_INC.Enabled =
                CONTROL_PTZ_IRIS_DEC.Enabled =
                CONTROL_PTZ_IRIS_INC.Enabled =
                CONTROL_PTZ_PRESET_MOVE.Enabled =
                CONTROL_PTZ_PRESET_SAVE.Enabled =
                CONTROL_PTZ_ONE_PUSH.Enabled =
                CONTROL_PTZ_MENU.Enabled =
                CONTROL_PTZ_OSD.Enabled =
                CONTROL_PTZ_COMMAND_CONTROL.Enabled = enable;*/
            }
        }

        public void feature_control_PTZ_screen_changed_pane(int pane)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { feature_control_PTZ_screen_changed_pane(pane); });
                return;
            }

            bool enable = _screen.is_enable(pane);
            int camera = _screen.get_pane_channelext(pane);
            int stream_id = _screen.get_pane_stream_id(pane);
            int channel = _channel;
            if (enable) enable = _adaptor.is_authority(channel, G2RAS_AUTHORITY.TYPE.AUTHORITY_PTZ_CONTROL);
            if (enable)
            {
                G2DEVICE_STATUS_STREAM_INFO si;
                bool roi_stream = false;
                if (_adaptor.get_status_stream_info(channel, camera, stream_id, out si))
                {
                    roi_stream = si._roi;
                }

                if (roi_stream)
                {
                    enable = _adaptor.get_status_stream_eptz(channel, camera, stream_id) > G2DEVICE_STATUS.PTZ.INACTIVE;
                }
                else
                {
                    enable = _adaptor.get_status_ptz(channel, camera) > G2DEVICE_STATUS.PTZ.INACTIVE;
                }
            }

            if (enable)
            {
                if (_feature_PTZ.pane != pane)
                {
                    _feature_PTZ.reset();
                    _feature_PTZ.pane = pane;
                }
            }

            feature_control_PTZ_set_enable(enable);
        }

        public void feature_control_ptz_set_product_info(ref G2_PRODUCT_INFO pi)
        {
            _feature_PTZ.reset();
            /*_feature_PTZ.form_preset = null;
            _feature_PTZ.form_menu = null;
            _feature_PTZ.form_command = null;*/
        }

        private void on_post_watch_receive_ptz_preset(int channel, ref G2LIVE_PTZ_PRESET preset)
        {
            /*form_ptz_preset form = _feature_PTZ.form_preset;
            if (form != null)
            {
                form.set_preset(ref preset);
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    if (_adaptor.is_connected(_channel) != true)
                    {
                        return;
                    }

                    G2LIVE_PTZ_COMMAND pc = new G2LIVE_PTZ_COMMAND();
                    pc._command = (int)form.command;
                    pc._argument = form.selected;

                    if (form.command == G2LIVE_PTZ_COMMAND.COMMAND.PTZ_SET_PRESET)
                    {
                        G2LIVE_PTZ_PRESET data = form.data;
                        _adaptor.set_ptz_preset(channel, form.camera, ref data);
                        _adaptor.set_ptz_command(_channel, form.camera, ref pc);
                    }
                    else
                    {
                        _adaptor.set_ptz_command(_channel, form.camera, ref pc);
                    }
                }

                form.Dispose();

                _feature_PTZ.form_preset = null;
            }*/
        }

        private void on_post_watch_receive_ptz_menu(int channel, ref G2LIVE_PTZ_MENU menu)
        {
            /*form_ptz_menu form = _feature_PTZ.form_menu;
            if (form != null)
            {
                form.set_features(ref menu);
                form.ShowDialog(this);
                form.Dispose();

                _feature_PTZ.form_menu = null;
            }*/
        }

        private void on_post_watch_receive_ptz_command_control_status(int channel, int camera, ref G2LIVE_COMMAND_CONTROL_PTZ control, ref G2LIVE_COMMAND_CONTROL_PTZ_RANGE range)
        {
            /*form_ptz_command form = _feature_PTZ.form_command;
            if (form != null)
            {
                form.set_status(ref control, ref range);
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    G2LIVE_COMMAND_CONTROL_PTZ data = form.data;
                    if (_adaptor.is_connected(channel))
                    {
                        _adaptor.send_command_control_ptz(channel, camera, ref data);
                    }
                }
                form.Dispose();

                _feature_PTZ.form_command = null;
            }*/
        }
    }
}
