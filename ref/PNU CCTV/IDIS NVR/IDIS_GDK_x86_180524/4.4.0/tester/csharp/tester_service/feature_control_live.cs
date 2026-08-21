using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using GDK;

namespace GDK_tester
{
    public partial class form_live
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
            public form_ptz_preset form_preset;
            public form_ptz_menu form_menu;
            public form_ptz_command form_command;
        }

        public class alarm_out_item
        {
            public string name;
            public string root_name;
            public G2GUID guid;
            public bool enable;
            public int owner_channel;

            public override string ToString()
            {
                return "[" + root_name + "] " + name;
            }

            public alarm_out_item(G2DEVICE_LEAF leaf_info, int channel, string root_name)
            {
                this.name = leaf_info._name;
                this.guid = leaf_info._guid;
                this.enable = leaf_info._enable;
                this.owner_channel = channel;
                this.root_name = root_name;
            }
        }

        public class beep_item
        {
            public string name;
            public G2GUID guid;
            public int owner_channel;

            public override string ToString()
            {
                return name;
            }

            public beep_item(G2DEVICE_ROOT root_info, int channel)
            {
                this.name = root_info._name;
                this.guid = root_info._guid;
                this.owner_channel = channel;
            }
        }

        private feature_PTZ _feature_PTZ;

        public void feature_control_multi_stream(bool multi, bool transfer)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { feature_control_multi_stream(multi, transfer); });
                return;
            }

            CONTROL_MULTI_STREAM.Enabled = multi;
            CONTROL_MULTI_STREAM_CHK_TRANSFER.Enabled = transfer;
        }

        public void on_cotrol_multi_stream(object sender, EventArgs e)
        {
            int channelext = _screen.selected_pane_channelext();
            int panel_num = _screen.selected_pane();
            for (int i = 0; i < _liveinfo_list.Count; ++i)
            {
                if (_liveinfo_list[i].channelext_list.Contains(channelext) && _liveinfo_list[i].channelext_panel_dic[channelext] == panel_num)
                {
                    int channel = _liveinfo_list[i].adaptor_channel;
                    int index = _liveinfo_list[i].channelext_list.IndexOf(channelext);
                    G2GUID camera_guid = _liveinfo_list[i].camera_info_list[index]._guid;
                    ContextMenu pop = new ContextMenu();
                    int stream_count = _adaptor.get_stream_count(channel, ref camera_guid);
                    MenuItem[] menus = new MenuItem[stream_count];

                    for (int j = 0; j < stream_count; ++j)
                    {
                        string stream_str;
                        G2DEVICE_STATUS_STREAM_INFO status;
                        if (_adaptor.get_camera_status_stream_info(channel, ref camera_guid, j, out status))
                        {
                            stream_str = string.Format("stream {0} ({1}) {2}x{3}@{4}ips {5}", j + 1, status.codec,
                                status._width, status._height, status._ips.ToString("0.00"), status.bitrate_control_type);
                            string title = status._title;
                            if (title.Length != 0)
                            {
                                stream_str += " / ";
                                stream_str += title;
                            }
                        }
                        else
                        {
                            stream_str = string.Format("stream {0}", j + 1);
                        }

                        MenuItem menu = new MenuItem(stream_str, new EventHandler(on_control_multi_stream_item_select));
                        menu.Tag = j;
                        menus[j] = menu;
                    }

                    int current_stream_id = _adaptor.get_stream_id(channel, ref camera_guid);
                    menus[current_stream_id].Checked = true;

                    pop.MenuItems.AddRange(menus);
                    Point position = new Point(GRP_CONTROL_STREAM.Location.X + CONTROL_MULTI_STREAM.Location.X, GRP_CONTROL_STREAM.Location.Y + CONTROL_MULTI_STREAM.Location.Y + CONTROL_MULTI_STREAM.Height);
                    pop.Show(this, position);

                    break;
                }
            }
        }

        public void on_control_multi_stream_item_select(object sender, EventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            int stream_id = (int)mi.Tag;
            int channelext = _screen.selected_pane_channelext();
            int panel = _screen.selected_pane();
            int channel = -1;
            G2GUID camera_guid = new G2GUID();
            g2channel_set channels = new g2channel_set();

            for (int i = 0; i < _liveinfo_list.Count; ++i)
            {
                if (_liveinfo_list[i].channelext_list.Contains(channelext) && _liveinfo_list[i].channelext_panel_dic[channelext] == panel)
                {
                    channel = _liveinfo_list[i].adaptor_channel;
                    int index = _liveinfo_list[i].channelext_list.IndexOf(channelext);
                    camera_guid = _liveinfo_list[i].camera_info_list[index]._guid;
                    channels.insert(_liveinfo_list[i].channelext_list.ToArray());
                    break;
                }
            }

            G2CHANNEL_STREAM stream_pre = _screen.get_pane_channel_stream(panel);
            _screen.get_pane(panel)._stream_id = stream_id;
            _adaptor.set_stream_id(channel, ref camera_guid, stream_id);
            _adaptor.set_camera_list(channel, channels, true);

            GDK_tester.frame_buf buf = _screen.buf_manager().get(channel);
            if (buf != null)
            {
                buf.clear(stream_pre._channel, stream_pre._stream);
            }

            g2decoder decoder = _screen.decoder();
            lock (decoder)
            {
                decoder.close(channelext);
            }
        }

        public void on_control_multi_stream_transfer(object sender, EventArgs e)
        {
            CheckBox control = sender as CheckBox;
            CONTROL_MULTI_STREAM.Enabled = (control.Checked != true);
            _transfer_all = control.Checked;

            if (_transfer_all)
            {
                GDK_tester.camera_pane.STATUS status = _screen.get_pane_status(0);

                int channelext = _screen.selected_pane_channelext();
                int panel_num = _screen.selected_pane();
                for (int i = 0; i < _liveinfo_list.Count; ++i)
                {
                    if (_liveinfo_list[i].channelext_list.Contains(channelext) && _liveinfo_list[i].channelext_panel_dic[channelext] == panel_num)
                    {
                        int channel = _liveinfo_list[i].adaptor_channel;
                        int index = _liveinfo_list[i].channelext_list.IndexOf(channelext);
                        G2GUID camera_guid = _liveinfo_list[i].camera_info_list[index]._guid;

                        int stream_count;
                        if (control.Checked)
                        {
                            stream_count = _adaptor.get_stream_count(channel, ref camera_guid);
                        }
                        else
                        {
                            stream_count = 1;
                        }

                        int[] panes = new int[stream_count];
                        g2channel_stream_set stream_set = new g2channel_stream_set();
                        for (int j = 0; j < stream_count; ++j)
                        {
                            panes[j] = j;
                            stream_set.insert(channelext, j);
                        }

                        _screen.buf_manager().clear(channel);
                        _adaptor.set_camera_stream_set(channel, stream_set, true);
                        _screen.reset(false);

                        for (int k = 0; k < stream_count; ++k)
                        {
                            GDK_tester.camera_pane cp = _screen.get_pane(k);
                            if (cp != null)
                            {
                                cp._channelext = channelext;
                                cp._stream_id = k;
                            }
                        }

                        _screen.set_pane_mode(panes, GDK_tester.camera_pane.MODE.LIVE);
                        _screen.set_pane_select(0, false);
                        _screen.set_format(stream_count, false);
                        _screen.buf_manager().clear(channel);

                        for (int n = 0; n < stream_count; ++n)
                        {
                            G2DEVICE_STATUS_STREAM_INFO stream_info;
                            _adaptor.get_camera_status_stream_info(channel, ref camera_guid, n, out stream_info);
                            string title = stream_info._title;
                            if (title.Length == 0) title = string.Format("Stream {0}", n + 1);
                            if (stream_info.is_on != true)
                            {
                                _screen.set_pane_status(n, GDK_tester.camera_pane.STATUS.STREAM_OFF, false);
                            }
                            else
                            {
                                _screen.set_pane_status(n, status, false);
                            }
                            _screen.set_pane_title(n, title, false);
                        }

                        _screen.update();
                        break;
                    }
                }
            }
            else
            {
                _screen.reset(false);
                int[] panes = new int[1];
                panes[0] = 0;
                _screen.set_pane_mode(panes, GDK_tester.camera_pane.MODE.LIVE);
                _screen.set_pane_select(0, false);
                _screen.set_format(GDK_tester.screen_format.FORMAT.LAYOUT1X1, false);
                _screen.update();
            }
        }

        public void on_feature_control_color(object sender, EventArgs e)
        {
            G2LIVE_CAMERA_COLOR.TYPE type;
            G2LIVE_CAMERA_COLOR.VALUE value;

            if (sender == CONTROL_COLOR_RESET)
            {
                type = G2LIVE_CAMERA_COLOR.TYPE.REVERT;
                value = G2LIVE_CAMERA_COLOR.VALUE.DEFAULT;
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
                else type = G2LIVE_CAMERA_COLOR.TYPE.HUE;

                if (sender == CONTROL_COLOR_BRIGHT_DEC ||
                    sender == CONTROL_COLOR_CONTRAST_DEC ||
                    sender == CONTROL_COLOR_SATURATION_DEC ||
                    sender == CONTROL_COLOR_HUE_DEC)
                {
                    value = G2LIVE_CAMERA_COLOR.VALUE.DOWN;
                }
                else
                {
                    value = G2LIVE_CAMERA_COLOR.VALUE.UP;
                }
            }

            int channelext = _screen.selected_pane_channelext();
            int panel_num = _screen.selected_pane();
            for (int i = 0; i < _liveinfo_list.Count; ++i)
            {
                if (_liveinfo_list[i].channelext_list.Contains(channelext) && _liveinfo_list[i].channelext_panel_dic[channelext] == panel_num)
                {
                    int channel = _liveinfo_list[i].adaptor_channel;
                    int index = _liveinfo_list[i].channelext_list.IndexOf(channelext);
                    G2GUID camera_guid = _liveinfo_list[i].camera_info_list[index]._guid;
                    _adaptor.set_camera_color(channel, ref camera_guid, type, value);
                    break;
                }
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
        }

        public void feature_control_PTZ_set_enable(bool enable, G2LIVE_CAMERA_STATUS.PTZ_FUNCTION function)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { feature_control_PTZ_set_enable(enable, function); });
                return;
            }

            if (enable)
            {
                CONTROL_PTZ_MOVE_NW.Enabled =
                CONTROL_PTZ_MOVE_N.Enabled =
                CONTROL_PTZ_MOVE_NE.Enabled =
                CONTROL_PTZ_MOVE_W.Enabled =
                CONTROL_PTZ_MOVE_E.Enabled =
                CONTROL_PTZ_MOVE_SW.Enabled =
                CONTROL_PTZ_MOVE_S.Enabled =
                CONTROL_PTZ_MOVE_SE.Enabled = (function & G2LIVE_CAMERA_STATUS.PTZ_FUNCTION.PAN_TILT) != 0;
                CONTROL_PTZ_ZOOM_DEC.Enabled =
                CONTROL_PTZ_ZOOM_INC.Enabled = (function & G2LIVE_CAMERA_STATUS.PTZ_FUNCTION.ZOOM) != 0;
                CONTROL_PTZ_FOCUS_DEC.Enabled =
                CONTROL_PTZ_FOCUS_INC.Enabled = (function & G2LIVE_CAMERA_STATUS.PTZ_FUNCTION.FOCUS) != 0;
                CONTROL_PTZ_IRIS_DEC.Enabled =
                CONTROL_PTZ_IRIS_INC.Enabled = (function & G2LIVE_CAMERA_STATUS.PTZ_FUNCTION.IRIS) != 0;
                CONTROL_PTZ_PRESET_MOVE.Enabled = (function & G2LIVE_CAMERA_STATUS.PTZ_FUNCTION.PRESET_MOVE) != 0;
                CONTROL_PTZ_PRESET_SAVE.Enabled = (function & G2LIVE_CAMERA_STATUS.PTZ_FUNCTION.PRESET_SETUP) != 0;
                CONTROL_PTZ_ONE_PUSH.Enabled = (function & G2LIVE_CAMERA_STATUS.PTZ_FUNCTION.FOCUS_ONEPUSH) != 0;
                CONTROL_PTZ_MENU.Enabled = (function & G2LIVE_CAMERA_STATUS.PTZ_FUNCTION.ADVANCED) != 0;
                CONTROL_PTZ_OSD.Enabled = (function & G2LIVE_CAMERA_STATUS.PTZ_FUNCTION.OSD_MENU) != 0;
            }
            else
            {
                CONTROL_PTZ_MOVE_NW.Enabled =
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
                CONTROL_PTZ_OSD.Enabled = enable;
            }
        }

        public void init_control_ptz()
        {
            this._feature_PTZ = new feature_PTZ();
            this.CONTROL_PTZ_MOVE_NW.Tag = G2LIVE_PTZ_COMMAND.COMMAND.PTZ_MOVE_NW;
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
            this.CONTROL_PTZ_FOCUS_DEC.Tag = G2LIVE_PTZ_COMMAND.COMMAND.PTZ_FOCUS_FAR;
        }

        public void on_feature_control_PTZ_move(object sender, EventArgs e)
        {
            feature_PTZ.PARAM param = feature_PTZ.PARAM.RELEASE;
            button_dwell.event_args args = e as button_dwell.event_args;
            if (args.repeat_count == 0)
            {
                if (_feature_PTZ.on_hold)
                {
                    _feature_PTZ.on_hold = false;
                    param = feature_PTZ.PARAM.RELEASE;
                }
                else
                {
                    param = feature_PTZ.PARAM.STEP;
                }
            }
            else if (args.repeat_count == 1)
            {
                _feature_PTZ.on_hold = true;
                param = feature_PTZ.PARAM.HOLD;
            }
            else
            {
                return;
            }

            G2LIVE_PTZ_COMMAND.COMMAND cmd = G2LIVE_PTZ_COMMAND.COMMAND.PTZ_STOP_MOVE;

            Control control = sender as Control;
            if (control.Tag != null)
            {
                cmd = (G2LIVE_PTZ_COMMAND.COMMAND)control.Tag;
            }

            G2LIVE_PTZ_COMMAND pc = new G2LIVE_PTZ_COMMAND();
            pc._command = (int)cmd;
            pc._argument = param == feature_PTZ.PARAM.HOLD ? 1 : 0;
            pc._reserved = 0;

            int channelext = _screen.selected_pane_channelext();
            int panel_num = _screen.selected_pane();
            for (int i = 0; i < _liveinfo_list.Count; ++i)
            {
                if (_liveinfo_list[i].channelext_list.Contains(channelext) && _liveinfo_list[i].channelext_panel_dic[channelext] == panel_num)
                {
                    int channel = _liveinfo_list[i].adaptor_channel;
                    int index = _liveinfo_list[i].channelext_list.IndexOf(channelext);
                    G2GUID camera_guid = _liveinfo_list[i].camera_info_list[index]._guid;
                    pc._camera = camera_guid;
                    pc._stream_id = _screen.selected_pane_stream_id();
                    _adaptor.set_ptz_command(channel, ref camera_guid, ref pc);
                    break;
                }
            }
        }

        public void on_feature_control_PTZ_function(object sender, EventArgs e)
        {
            if (sender == CONTROL_PTZ_ONE_PUSH)
            {
                G2LIVE_PTZ_COMMAND pc = new G2LIVE_PTZ_COMMAND();
                pc._command = (int)G2LIVE_PTZ_COMMAND.COMMAND.PTZ_FOCUS_ONEPUSH;
                pc._argument = 0;
                pc._reserved = 0;

                int channelext = _screen.selected_pane_channelext();
                int panel_num = _screen.selected_pane();
                for (int i = 0; i < _liveinfo_list.Count; ++i)
                {
                    if (_liveinfo_list[i].channelext_list.Contains(channelext) && _liveinfo_list[i].channelext_panel_dic[channelext] == panel_num)
                    {
                        int channel = _liveinfo_list[i].adaptor_channel;
                        int channelext_index = _liveinfo_list[i].channelext_list.IndexOf(channelext);
                        G2GUID camera_guid = _liveinfo_list[i].camera_info_list[channelext_index]._guid;
                        pc._stream_id = _adaptor.get_stream_id(channel, ref camera_guid);
                        _adaptor.set_ptz_command(channel, ref camera_guid, ref pc);
                        break;
                    }
                }
                return;
            }
            else if (sender == CONTROL_PTZ_MENU)
            {
                if (_feature_PTZ.form_menu == null)
                {
                    int channelext = _screen.selected_pane_channelext();
                    int panel_num = _screen.selected_pane();
                    for (int i = 0; i < _liveinfo_list.Count; ++i)
                    {
                        if (_liveinfo_list[i].channelext_list.Contains(channelext) && _liveinfo_list[i].channelext_panel_dic[channelext] == panel_num)
                        {
                            int channel = _liveinfo_list[i].adaptor_channel;
                            int index = _liveinfo_list[i].channelext_list.IndexOf(channelext);
                            G2GUID camera_guid = _liveinfo_list[i].camera_info_list[index]._guid;
                            _feature_PTZ.form_menu = new form_ptz_menu();
                            _feature_PTZ.form_menu.camera = camera_guid;
                            _feature_PTZ.form_menu.handler += new System.EventHandler(on_feature_control_PTZ_menu);
                            _adaptor.request_ptz_menu(channel, ref camera_guid);
                            break;
                        }
                    }
                    
                }
                return;
            }
            else if (sender == CONTROL_PTZ_OSD)
            {
                int panel_num = _screen.selected_pane();
                int channelext = _screen.selected_pane_channelext();
                for (int i = 0; i < _liveinfo_list.Count; ++i)
                {
                    if (_liveinfo_list[i].channelext_list.Contains(channelext) && _liveinfo_list[i].channelext_panel_dic[channelext] == panel_num)
                    {
                        int index = _liveinfo_list[i].channelext_list.IndexOf(channelext);
                        G2GUID camera_guid = _liveinfo_list[i].camera_info_list[index]._guid;
                        G2LIVE_CAMERA_STATUS status;
                        _adaptor.get_camera_status(_liveinfo_list[i].adaptor_channel, ref camera_guid, out status);
                        if ((status.ptz_function & G2LIVE_CAMERA_STATUS.PTZ_FUNCTION.OSD_MENU_OFF_NONE) != 0)
                        {
                            _feature_PTZ.on_OSD = true;
                        }
                        else
                        {
                            _feature_PTZ.on_OSD = !_feature_PTZ.on_OSD;
                        }
                        G2LIVE_PTZ_COMMAND pc = new G2LIVE_PTZ_COMMAND();
                        pc._command = (int)(_feature_PTZ.on_OSD ? G2LIVE_PTZ_COMMAND.COMMAND.PTZ_MENU_ON : G2LIVE_PTZ_COMMAND.COMMAND.PTZ_MENU_OFF);
                        pc._argument = 0;
                        pc._reserved = 0;
                        _adaptor.set_ptz_command(_liveinfo_list[i].adaptor_channel, ref camera_guid, ref pc);

                        break;
                    }
                }
                
                return;
            }
            else
            {
                Control control = sender as Control;
                if (control == null) return;
                if (control.Tag == null) return;

                G2LIVE_PTZ_COMMAND pc = new G2LIVE_PTZ_COMMAND();
                pc._command = (int)(G2LIVE_PTZ_COMMAND.COMMAND)control.Tag;

                button_dwell.event_args args = e as button_dwell.event_args;
                if (args.repeat_count == 0)
                {
                    if (_feature_PTZ.on_hold)
                    {
                        _feature_PTZ.on_hold = false;
                        if (pc._command == (int)G2LIVE_PTZ_COMMAND.COMMAND.PTZ_ZOOM_IN ||
                            pc._command == (int)G2LIVE_PTZ_COMMAND.COMMAND.PTZ_ZOOM_OUT)
                        {
                            pc._command = (int)G2LIVE_PTZ_COMMAND.COMMAND.PTZ_STOP_ZOOM;
                        }
                        else if (pc._command == (int)G2LIVE_PTZ_COMMAND.COMMAND.PTZ_FOCUS_FAR ||
                                    pc._command == (int)G2LIVE_PTZ_COMMAND.COMMAND.PTZ_FOCUS_NEAR)
                        {
                            pc._command = (int)G2LIVE_PTZ_COMMAND.COMMAND.PTZ_STOP_FOCUS;
                        }
                        else if (pc._command == (int)G2LIVE_PTZ_COMMAND.COMMAND.PTZ_IRIS_CLOSE ||
                                    pc._command == (int)G2LIVE_PTZ_COMMAND.COMMAND.PTZ_IRIS_OPEN)
                        {
                            pc._command = (int)G2LIVE_PTZ_COMMAND.COMMAND.PTZ_STOP_IRIS;
                        }
                    }
                    pc._argument = 0;
                }
                else if (args.repeat_count == 1)
                {
                    _feature_PTZ.on_hold = true;
                    pc._argument = 1;
                }
                else
                {
                    return;
                }

                int channelext = _screen.selected_pane_channelext();
                int panel_num = _screen.selected_pane();
                for (int i = 0; i < _liveinfo_list.Count; ++i)
                {
                    if (_liveinfo_list[i].channelext_list.Contains(channelext) && _liveinfo_list[i].channelext_panel_dic[channelext] == panel_num)
                    {
                        int channel = _liveinfo_list[i].adaptor_channel;
                        int channelext_index = _liveinfo_list[i].channelext_list.IndexOf(channelext);
                        G2GUID camera_guid = _liveinfo_list[i].camera_info_list[channelext_index]._guid;
                        pc._stream_id = _adaptor.get_stream_id(channel, ref camera_guid);
                        _adaptor.set_ptz_command(channel, ref camera_guid, ref pc);
                        break;
                    }
                }
            }
        }

        public void on_feature_control_PTZ_menu(object sender, EventArgs e)
        {
            form_ptz_menu.event_args args = e as form_ptz_menu.event_args;
            feature_control_ptz_menu(args.camera, args.command, args.i);
        }

        public void feature_control_ptz_menu(G2GUID camera, G2LIVE_PTZ_COMMAND.COMMAND cmd, int index)
        {
            G2LIVE_PTZ_COMMAND pc = new G2LIVE_PTZ_COMMAND();
            pc._command = (int)cmd;
            pc._argument = index;
            pc._reserved = 0;

            int channelext = _screen.selected_pane_channelext();
            int panel_num = _screen.selected_pane();
            for (int i = 0; i < _liveinfo_list.Count; ++i)
            {
                if (_liveinfo_list[i].channelext_list.Contains(channelext) && _liveinfo_list[i].channelext_panel_dic[channelext] == panel_num)
                {
                    int channel = _liveinfo_list[i].adaptor_channel;
                    int channelext_index = _liveinfo_list[i].channelext_list.IndexOf(channelext);
                    G2GUID camera_guid = _liveinfo_list[i].camera_info_list[channelext_index]._guid;
                    _adaptor.set_ptz_command(channel, ref camera_guid, ref pc);
                    break;
                }
            }
        }

        public void on_post_live_receive_ptz_menu(int channel, ref G2LIVE_PTZ_MENU menu)
        {
            form_ptz_menu form = _feature_PTZ.form_menu;
            if (form != null)
            {
                form.set_features(ref menu);
                form.ShowDialog(this);
                form.Dispose();

                _feature_PTZ.form_menu = null;
            }
        }

        public void on_feature_control_PTZ_preset(object sender, EventArgs e)
        {
            int channelext = _screen.selected_pane_channelext();
            int panel_num = _screen.selected_pane();
            for (int i = 0; i < _liveinfo_list.Count; ++i)
            {
                if (_liveinfo_list[i].channelext_list.Contains(channelext) && _liveinfo_list[i].channelext_panel_dic[channelext] == panel_num)
                {
                    int channel = _liveinfo_list[i].adaptor_channel;
                    int channelext_index = _liveinfo_list[i].channelext_list.IndexOf(channelext);
                    G2GUID camera_guid = _liveinfo_list[i].camera_info_list[channelext_index]._guid;
                    if (_feature_PTZ.form_preset == null)
                    {
                        _feature_PTZ.form_preset = new form_ptz_preset();
                        _feature_PTZ.form_preset.command = (sender == CONTROL_PTZ_PRESET_SAVE) ? G2LIVE_PTZ_COMMAND.COMMAND.PTZ_SET_PRESET : G2LIVE_PTZ_COMMAND.COMMAND.PTZ_MOVE_PRESET;
                        _feature_PTZ.form_preset.camera = camera_guid;
                        _feature_PTZ.form_preset.handler_move += new System.EventHandler(on_feature_control_PTZ_preset_move);
                        _adaptor.request_ptz_preset(channel, ref camera_guid);
                    }
                    break;
                }
            }
        }

        public void on_post_live_receive_ptz_preset(int channel, ref G2LIVE_PTZ_PRESET preset)
        {
            form_ptz_preset form = _feature_PTZ.form_preset;
            if (form != null)
            {
                form.set_preset(ref preset);
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    if (_adaptor.is_connected(channel) != true)
                    {
                        return;
                    }

                    G2LIVE_PTZ_COMMAND pc = new G2LIVE_PTZ_COMMAND();
                    pc._command = (int)form.command;
                    pc._argument = form.selected;

                    G2GUID camera = form.camera;
                    if (form.command == G2LIVE_PTZ_COMMAND.COMMAND.PTZ_SET_PRESET)
                    {
                        G2LIVE_PTZ_PRESET data = form.data;
                        _adaptor.set_ptz_preset(channel, ref camera, ref data);
                        _adaptor.set_ptz_command(channel, ref camera, ref pc);
                    }
                    else
                    {
                        _adaptor.set_ptz_command(channel, ref camera, ref pc);
                    }
                }

                form.Dispose();

                _feature_PTZ.form_preset = null;
            }
        }

        public void on_feature_control_PTZ_preset_move(object sender, EventArgs e)
        {
            form_ptz_preset form = _feature_PTZ.form_preset;
            if (form.command == G2LIVE_PTZ_COMMAND.COMMAND.PTZ_MOVE_PRESET)
            {
                G2LIVE_PTZ_COMMAND pc = new G2LIVE_PTZ_COMMAND();
                pc._command = (int)form.command;
                pc._argument = form.selected;

                int channelext = _screen.selected_pane_channelext();
                int panel_num = _screen.selected_pane();
                for (int i = 0; i < _liveinfo_list.Count; ++i)
                {
                    if (_liveinfo_list[i].channelext_list.Contains(channelext) && _liveinfo_list[i].channelext_panel_dic[channelext] == panel_num)
                    {
                        int channel = _liveinfo_list[i].adaptor_channel;
                        int channelext_index = _liveinfo_list[i].channelext_list.IndexOf(channelext);
                        G2GUID camera_guid = _liveinfo_list[i].camera_info_list[channelext_index]._guid;
                        _adaptor.set_ptz_command(channel, ref camera_guid, ref pc);
                        break;
                    }
                }
            }
        }

        public void feature_control_audio_set_enable(bool enable)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { feature_control_audio_set_enable(enable); });
                return;
            }

            bool auth_in = _user_adaptor.is_authority(G2USER_AUTHORITY.TYPE.AUTHORITY_AUDIO_IN);
            bool auth_out = _user_adaptor.is_authority(G2USER_AUTHORITY.TYPE.AUTHORITY_AUDIO_OUT);

            if (enable)
            {
                bool in_sounds = false;
                bool in_active = false;
                bool out_sounds = false;
                bool out_active = false;

                int channelext = _screen.selected_pane_channelext();
                int panel_num = _screen.selected_pane();
                for (int i = 0; i < _liveinfo_list.Count; ++i)
                {
                    if (_liveinfo_list[i].channelext_list.Contains(channelext) && _liveinfo_list[i].channelext_panel_dic.ContainsKey(channelext) &&
                        _liveinfo_list[i].channelext_panel_dic[channelext] == panel_num)
                    {
                        int channel = _liveinfo_list[i].adaptor_channel;
                        int channelext_index = _liveinfo_list[i].channelext_list.IndexOf(channelext);
                        G2GUID camera_guid = _liveinfo_list[i].camera_info_list[channelext_index]._guid;
                        G2GUID root_guid = _liveinfo_list[i].camera_info_list[channelext_index]._guid_root;
                        G2LIVE_CAMERA_STATUS status;
                        _adaptor.get_camera_status(channel, ref camera_guid, out status);
                        if (auth_in)
                        {
                            if (status.audio_in > 0 && _adaptor.is_enable_audio_in(channel, ref camera_guid))
                            {
                                in_sounds = true;
                            }
                            if (_adaptor.is_contains_audio_streaming(channel, ref camera_guid))
                            {
                                in_active = true;
                            }
                        }
                        if (auth_out)
                        {
                            if (_adaptor.is_enable_audio_out(channel, ref root_guid, (int)_liveinfo_list[i].camera_info_list[channelext_index]._number_root_based))
                            {
                                out_sounds = true;
                            }
                            if (_adaptor.is_contains_audio_capturing(channel, ref root_guid, (int)_liveinfo_list[i].camera_info_list[channelext_index]._number_root_based))
                            {
                                out_active = true;
                            }
                        }
                        break;
                    }
                }

                CONTROL_AUDIO_IN.Enabled = in_sounds;
                CONTROL_AUDIO_IN.Checked = in_active;
                CONTROL_AUDIO_OUT.Enabled = out_sounds;
                CONTROL_AUDIO_OUT.Checked = out_active;
            }
            else
            {
                CONTROL_AUDIO_IN.Checked =
                CONTROL_AUDIO_IN.Enabled =
                CONTROL_AUDIO_OUT.Checked =
                CONTROL_AUDIO_OUT.Enabled = enable;
            }
        }

        public void on_feature_control_audio_in(object sender, EventArgs e)
        {
            int channelext = _screen.selected_pane_channelext();
            int panel_num = _screen.selected_pane();
            for (int i = 0; i < _liveinfo_list.Count; ++i)
            {
                if (_liveinfo_list[i].channelext_list.Contains(channelext) && _liveinfo_list[i].channelext_panel_dic.ContainsKey(channelext) &&
                    _liveinfo_list[i].channelext_panel_dic[channelext] == panel_num)
                {
                    int channel = _liveinfo_list[i].adaptor_channel;
                    int channelext_index = _liveinfo_list[i].channelext_list.IndexOf(channelext);
                    G2GUID camera_guid = _liveinfo_list[i].camera_info_list[channelext_index]._guid;

                    bool active = _adaptor.is_contains_audio_streaming(channel, ref camera_guid) ? false : true;
                    _adaptor.set_enable_audio_streaming(channel, ref camera_guid, active);
                    bool contains = _adaptor.is_contains_audio_streaming(channel, ref camera_guid);
                    if (active)
                    {
                        if (contains != true)
                        {
                            feature_control_audio_set_enable(true);
                        }
                    }

                    break;
                }
            }
        }

        public void on_feature_control_audio_out(object sender, EventArgs e)
        {
            int channelext = _screen.selected_pane_channelext();
            int panel_num = _screen.selected_pane();
            for (int i = 0; i < _liveinfo_list.Count; ++i)
            {
                if (_liveinfo_list[i].channelext_list.Contains(channelext) && _liveinfo_list[i].channelext_panel_dic.ContainsKey(channelext) &&
                    _liveinfo_list[i].channelext_panel_dic[channelext] == panel_num)
                {
                    int channel = _liveinfo_list[i].adaptor_channel;
                    int channelext_index = _liveinfo_list[i].channelext_list.IndexOf(channelext);
                    G2GUID root_guid = _liveinfo_list[i].camera_info_list[channelext_index]._guid_root;

                    bool active = _adaptor.is_contains_audio_capturing(channel, ref root_guid, 
                        (int)_liveinfo_list[i].camera_info_list[channelext_index]._number_root_based) ? false : true;
                    bool res = _adaptor.set_enable_audio_capturing(channel, ref root_guid,
                        (int)_liveinfo_list[i].camera_info_list[channelext_index]._number_root_based, active);
                    
                    if (active)
                    {
                        if (res != true)
                        {
                            feature_control_audio_set_enable(true);
                        }
                    }

                    break;
                }
            }
        }

        public void check_alarm_auth()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { check_alarm_auth(); });
                return;
            }

            CONTROL_BEEP_CHECK_LIST.Items.Clear();
            CONTROL_ALARM_CHECK_LIST.Items.Clear();
            for (int i = 0; i < _liveinfo_list.Count; ++i)
            {
                feature_control_alarm_out(_liveinfo_list[i].adaptor_channel);
            }
        }

        public void feature_control_alarm_out(int channel)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { feature_control_alarm_out(channel); });
                return;
            }

            for (int i = 0; i < _liveinfo_list.Count; ++i)
            {
                if (_liveinfo_list[i].adaptor_channel == channel)
                {
                    if (_user_adaptor.is_authority(G2USER_AUTHORITY.TYPE.AUTHORITY_ALARM_OUT_CONTROL))
                    {
                        for (int j = 0; j < _liveinfo_list[i].alarm_out_list.Count; ++j)
                        {
                            G2DEVICE_ROOT root_info = _liveinfo_list[i].get_root_info(_liveinfo_list[i].alarm_out_list[j]._guid_root);
                            alarm_out_item item = new alarm_out_item(_liveinfo_list[i].alarm_out_list[j], _liveinfo_list[i].adaptor_channel, root_info._name);

                            CONTROL_ALARM_CHECK_LIST.Items.Add(item);
                        }
                    }

                    List<G2GUID> root_guids = _liveinfo_list[i].get_root_guids();
                    for (int k = 0; k < root_guids.Count; ++k)
                    {
                        G2GUID root = root_guids[k];
                        beep_item item = new beep_item(_liveinfo_list[i].get_root_info(root), _liveinfo_list[i].adaptor_channel);
                        CONTROL_BEEP_CHECK_LIST.Items.Add(item);
                    }

                    break;
                }
            }

            if (CONTROL_ALARM_CHECK_LIST.Items.Count > 0)
            {
                CONTROL_ALARM_CHECK_LIST.Enabled = true;
                CONTROL_ALARM_ON.Enabled = true;
                CONTROL_ALARM_OFF.Enabled = true;
            }
            else
            {
                CONTROL_ALARM_CHECK_LIST.Enabled = false;
                CONTROL_ALARM_ON.Enabled = false;
                CONTROL_ALARM_OFF.Enabled = false;
            }
            if (CONTROL_BEEP_CHECK_LIST.Items.Count > 0)
            {
                CONTROL_BEEP_CHECK_LIST.Enabled = true;
                CONTROL_BEEP_ON.Enabled = true;
                CONTROL_BEEP_OFF.Enabled = true;
            }
            else
            {
                CONTROL_BEEP_CHECK_LIST.Enabled = false;
                CONTROL_BEEP_ON.Enabled = false;
                CONTROL_BEEP_OFF.Enabled = false;
            }
        }

        public void on_feature_control_alarm_out(object sender, EventArgs e)
        {
            Button b = sender as Button;
            bool on = b.Text.Contains("on");

            CheckedListBox.CheckedItemCollection checked_alarms = CONTROL_ALARM_CHECK_LIST.CheckedItems;

            for (int i = 0; i < checked_alarms.Count; ++i)
            {
                alarm_out_item alarm = (alarm_out_item)checked_alarms[i];
                G2GUID guid = alarm.guid;
                _adaptor.set_alarm_out(alarm.owner_channel, ref guid, on);
            }
        }

        public void on_feature_control_beep(object sender, EventArgs e)
        {
            Button b = sender as Button;
            bool on = b.Text.Contains("on");

            CheckedListBox.CheckedItemCollection checked_beeps = CONTROL_BEEP_CHECK_LIST.CheckedItems;

            for (int i = 0; i < checked_beeps.Count; ++i)
            {
                beep_item beep = (beep_item)checked_beeps[i];
                G2GUID guid = beep.guid;
                _adaptor.set_beep_control(beep.owner_channel, ref guid, on);
            }
        }

        public void feature_control_instant_rec(bool enable, G2GUID camera)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { feature_control_instant_rec(enable, camera); });
                return;
            }

            if (enable && _admin_adaptor.is_device_ipcamera(ref camera) &&
                _user_adaptor.is_authority(G2USER_AUTHORITY.TYPE.AUTHORITY_INSTANT_RECORDING))
            {
                if (_admin_adaptor.is_activate_instant_record(ref camera, true))
                {
                    BTN_INST_REC_END.Enabled = true;
                    BTN_INST_REC_START.Enabled = false;
                }
                else
                {
                    BTN_INST_REC_END.Enabled = false;
                    BTN_INST_REC_START.Enabled = true;
                }
            }
            else
            {
                BTN_INST_REC_END.Enabled = false;
                BTN_INST_REC_START.Enabled = false;
            }
        }

        public bool is_current_panel(G2GUID camera)
        {
            int selected_pane = _screen.selected_pane();

            for (int i = 0; i < _liveinfo_list.Count; ++i)
            {
                if (_liveinfo_list[i].camera_guid_panel_dic.ContainsKey(camera))
                {
                    int panel = _liveinfo_list[i].camera_guid_panel_dic[camera];
                    if (panel == selected_pane)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return false;
        }

        private void on_btn_instant_record_start(object sender, EventArgs e)
        {
            int panel = _screen.selected_pane();
            camera_pane cp = _screen.get_pane(panel);

            for (int i = 0; i < _liveinfo_list.Count; ++i)
            {
                if (_liveinfo_list[i].channelext_list.Contains(cp._channelext) && _liveinfo_list[i].channelext_panel_dic.ContainsKey(cp._channelext) &&
                    _liveinfo_list[i].channelext_panel_dic[cp._channelext] == panel)
                {
                    int index = _liveinfo_list[i].channelext_list.IndexOf(cp._channelext);
                    G2GUID camera_guid = _liveinfo_list[i].camera_info_list[index]._guid;
                    _admin_adaptor.request_recording_instant(ref camera_guid, true);
                    break;
                }
            }
        }

        private void on_btn_instant_record_end(object sender, EventArgs e)
        {
            int panel = _screen.selected_pane();
            camera_pane cp = _screen.get_pane(panel);

            for (int i = 0; i < _liveinfo_list.Count; ++i)
            {
                if (_liveinfo_list[i].channelext_list.Contains(cp._channelext) && _liveinfo_list[i].channelext_panel_dic.ContainsKey(cp._channelext) &&
                    _liveinfo_list[i].channelext_panel_dic[cp._channelext] == panel)
                {
                    int index = _liveinfo_list[i].channelext_list.IndexOf(cp._channelext);
                    G2GUID camera_guid = _liveinfo_list[i].camera_info_list[index]._guid;
                    _admin_adaptor.request_recording_instant(ref camera_guid, false);
                    break;
                }
            }
        }
    }
}
