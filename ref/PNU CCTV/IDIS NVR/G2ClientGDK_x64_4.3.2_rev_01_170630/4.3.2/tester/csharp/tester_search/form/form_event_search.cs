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
    public partial class form_event_search : Form
    {
        public enum DEVICE_TYPE
        {
            CAMERA = 0,
            ALARM_IN,
            ALARM_IN_NETWORK,
            AUDIO_IN,
            TEXT_IN
        }

        public form_event_search(object options, g2channel_set cameras)
        {
            InitializeComponent();

            if (options is G2SEARCH_G2_EVENT_SEARCH_OPTIONS)
            {
                this._options = (G2SEARCH_G2_EVENT_SEARCH_OPTIONS)options;
            }
            else if (options is G2EVENT_QUERY_CONDITION)
            {
                this._options_G1 = (G2EVENT_QUERY_CONDITION)options;
            }

            this._options_pre = options;
            this._cameras = cameras;
            this._col_camera = new List<Control>();
            this._col_system = new List<Control>();
        }

        public void set_product_info(ref G2_PRODUCT_INFO pi)
        {
            _pi = pi;
            _caps_event = _pi.remote_search.get_event_set();
            _count_camera = (_pi.remote_search.base_channel > 0 &&
                             _pi.remote_search.stream_count > 0) ? pi.remote_search.base_channel * _pi.remote_search.stream_count :
                             _pi.device.count_camera;
            _count_alarm_in = _pi.device.count_alarm_in;
            _count_alarn_in_network = _pi.device.count_alarm_in_network;
            _count_audio_in = _caps_event.contains((int)G2EVENT.TYPE.AUDIO_ON) ? (int)_pi.device.count_audio_in : 0;
            _count_text_in = _pi.device.count_text_in;

            Point point = new Point(TRV_DEVICES.Left, TRV_DEVICES.Bottom + 10);
            int bottom = 0;

            imp_set_device();

            if (imp_set_event_camera(point.X, point.Y, out bottom))
            {
                foreach (Control c in _col_camera)
                {
                    this.Controls.Add(c);
                }
                this.Controls.Add(imp_create_seperator(new Rectangle(TRV_DEVICES.Left, bottom + 20, TRV_DEVICES.Width, 2)));
            }
            if (imp_set_event_system(point.X, bottom + 32, out bottom))
            {
                foreach (Control c in _col_system)
                {
                    this.Controls.Add(c);
                }
            }

            BTN_CANCEL.Top = BTN_OK.Top = bottom + 30;
            this.ClientSize = new Size(this.ClientSize.Width, BTN_CANCEL.Bottom + 12);
        }
        public void set_time_range(G2TIME from, G2TIME to)
        {
            _range_from = from;
            _range_to = to;
        }
        public void load()
        {
            imp_load();

            if (CHK_FIRST.Checked &&
                CHK_LAST.Checked)
            {
                if (_range_from.valid &&
                    _range_to.valid)
                {
                    DTP_FROM.Value = _range_from;
                    DTP_TO.Value = _range_to;
                }
            }
        }

        protected void imp_load()
        {
            G2SCOPE scope = new G2SCOPE();
            g2channel_set channels_camera = new g2channel_set(_cameras);
            g2channel_set channels_alarm_in = new g2channel_set();
            g2channel_set channels_alarm_in_network = new g2channel_set();
            g2channel_set channels_audio_in = new g2channel_set();
            g2channel_set channels_text_in = new g2channel_set();

            if (_options_pre is G2SEARCH_G2_EVENT_SEARCH_OPTIONS)
            {
                scope = _options._scope;
                channels_alarm_in = _options._alarm_in;
                channels_alarm_in_network = _options._alarm_in_network;
                channels_audio_in = _options._audio_in;
                channels_text_in = _options._text_in;

                foreach (CheckBox c in _col_camera)
                {
                    switch ((G2EVENT.TYPE)c.Tag)
                    {
                        case G2EVENT.TYPE.MOTION_ON: c.Checked = _options._motion._len > 0; break;
                        case G2EVENT.TYPE.OBJECT_TRACK_ON: c.Checked = _options._object._len > 0; break;
                        case G2EVENT.TYPE.VIDEO_LOSS_ON: c.Checked = _options._video_loss._len > 0; break;
                        case G2EVENT.TYPE.VIDEO_BLIND_ON: c.Checked = _options._video_blind._len > 0; break;
                        case G2EVENT.TYPE.VIDEO_ANALYTICS_ON: c.Checked = _options._video_analytics._len > 0; break;
                        case G2EVENT.TYPE.TRIPZONE_ON: c.Checked = _options._trip_zone._len > 0; break;
                        case G2EVENT.TYPE.TAMPER_ON: c.Checked = _options._tamper._len > 0; break;
                        case G2EVENT.TYPE.FACE_DETECTION_ON: c.Checked = _options._face_detect._len > 0; break;
                        case G2EVENT.TYPE.INSTANT_RECORDING_ON: c.Checked = _options._instant_record._len > 0; break;
                        case G2EVENT.TYPE.CAMERA_RECORD_BAD_ON: c.Checked = _options._camera_record_bad._len > 0; break;
                        case G2EVENT.TYPE.CAMERA_FAN_ERROR_ON: c.Checked = _options._camera_fan_error._len > 0; break;
                    }
                }

                g2channel_set temp = _options._system_events;
                foreach (CheckBox c in _col_system)
                {
                    c.Checked = temp.contains((int)c.Tag);
                }
            }
            else if (_options_pre is G2EVENT_QUERY_CONDITION)
            {
                scope._begin._time = _options_G1._begin;
                scope._end._time = _options_G1._end;
                channels_alarm_in.from(_options_G1._alarm_in);
                channels_audio_in.from(_options_G1._audio_in);
                channels_text_in.from(_options_G1._text_in);

                foreach (CheckBox c in _col_camera)
                {
                    switch ((G2EVENT.TYPE)c.Tag)
                    {
                        case G2EVENT.TYPE.MOTION_ON: c.Checked = _options_G1._motion > 0; break;
                        case G2EVENT.TYPE.OBJECT_TRACK_ON: c.Checked = _options_G1._object > 0; break;
                        case G2EVENT.TYPE.VIDEO_LOSS_ON: c.Checked = _options_G1._video_loss > 0; break;
                        case G2EVENT.TYPE.VIDEO_BLIND_ON: c.Checked = _options_G1._video_blind > 0; break;
                        case G2EVENT.TYPE.VIDEO_ANALYTICS_ON: c.Checked = _options_G1._video_analytics > 0; break;
                        case G2EVENT.TYPE.TRIPZONE_ON: c.Checked = _options_G1._trip_zone > 0; break;
                        case G2EVENT.TYPE.TAMPER_ON: c.Checked = _options_G1._tamper > 0; break;
                        case G2EVENT.TYPE.FACE_DETECTION_ON: c.Checked = _options_G1._face_detect > 0; break;
                        case G2EVENT.TYPE.INSTANT_RECORDING_ON: c.Checked = _options_G1._instant_record > 0; break;
                    }
                }

                g2channel_set temp = new g2channel_set();
                temp.from(_options_G1._system_events);

                foreach (CheckBox c in _col_system)
                {
                    switch ((G2EVENT.TYPE)c.Tag)
                    {
                        case G2EVENT.TYPE.RECORDER_BAD_ON: c.Checked = temp.contains((int)G2EVENT_QUERY_CONDITION.SYSTEM_EVENT_TYPE.SYSTEM_EVENT_RECORDER_BAD); break;
                        case G2EVENT.TYPE.ALARM_IN_BAD_ON: c.Checked = temp.contains((int)G2EVENT_QUERY_CONDITION.SYSTEM_EVENT_TYPE.SYSTEM_EVENT_ALARM_IN_BAD); break;
                        case G2EVENT.TYPE.TEXT_IN_BAD_ON: c.Checked = temp.contains((int)G2EVENT_QUERY_CONDITION.SYSTEM_EVENT_TYPE.SYSTEM_EVENT_TEXT_IN_BAD); break;
                        case G2EVENT.TYPE.TANGO_FULL_ON: c.Checked = temp.contains((int)G2EVENT_QUERY_CONDITION.SYSTEM_EVENT_TYPE.SYSTEM_EVENT_TANGO_FULL); break;
                        case G2EVENT.TYPE.DISK_BAD: c.Checked = temp.contains((int)G2EVENT_QUERY_CONDITION.SYSTEM_EVENT_TYPE.SYSTEM_EVENT_DISK_BAD); break;
                        case G2EVENT.TYPE.DISK_TEMPERATURE_ON: c.Checked = temp.contains((int)G2EVENT_QUERY_CONDITION.SYSTEM_EVENT_TYPE.SYSTEM_EVENT_DISK_TEMPERATURE); break;
                        case G2EVENT.TYPE.DISK_SMART_ON: c.Checked = temp.contains((int)G2EVENT_QUERY_CONDITION.SYSTEM_EVENT_TYPE.SYSTEM_EVENT_DISK_SMART); break;
                        case G2EVENT.TYPE.TANGO_ALMOST_FULL_ON: c.Checked = temp.contains((int)G2EVENT_QUERY_CONDITION.SYSTEM_EVENT_TYPE.SYSTEM_EVENT_TANGO_ALMOST_FULL); break;
                        case G2EVENT.TYPE.DISK_ON: c.Checked = temp.contains((int)G2EVENT_QUERY_CONDITION.SYSTEM_EVENT_TYPE.SYSTEM_EVENT_DISK_ON); break;
                        case G2EVENT.TYPE.DISK_OFF: c.Checked = temp.contains((int)G2EVENT_QUERY_CONDITION.SYSTEM_EVENT_TYPE.SYSTEM_EVENT_DISK_OFF); break;
                        case G2EVENT.TYPE.DISK_CONFIG_CHANGE: c.Checked = temp.contains((int)G2EVENT_QUERY_CONDITION.SYSTEM_EVENT_TYPE.SYSTEM_EVENT_DISK_CONFIG_CHANGE); break;
                        case G2EVENT.TYPE.PANIC_ON: c.Checked = temp.contains((int)G2EVENT_QUERY_CONDITION.SYSTEM_EVENT_TYPE.SYSTEM_EVENT_PANIC); break;
                        case G2EVENT.TYPE.FAN_ERROR_ON: c.Checked = temp.contains((int)G2EVENT_QUERY_CONDITION.SYSTEM_EVENT_TYPE.SYSTEM_EVENT_DC_FAN_ERROR); break;
                        case G2EVENT.TYPE.GPS_RECEIVE_ERROR_ON: c.Checked = temp.contains((int)G2EVENT_QUERY_CONDITION.SYSTEM_EVENT_TYPE.SYSTEM_EVENT_GPS_RECEIVE_ERROR); break;
                        case G2EVENT.TYPE.LOGIN_FAILED_SEVERAL_TIMES: c.Checked = temp.contains((int)G2EVENT_QUERY_CONDITION.SYSTEM_EVENT_TYPE.SYSTEM_EVENT_LOGIN_FAILED_SEVERAL_TIMES); break;
                    }
                }
            }

            if (scope._begin._time.valid) DTP_FROM.Value = scope._begin._time;
            if (scope._end._time.valid) DTP_TO.Value = scope._end._time;

            CHK_FIRST.Checked = (scope._begin._time.valid != true);
            DTP_FROM.Enabled = CHK_FIRST.Checked != true;
            CHK_LAST.Checked = (scope._end._time.valid != true);
            DTP_TO.Enabled = CHK_LAST.Checked != true;

            foreach (TreeNode root in TRV_DEVICES.Nodes)
            {
                DEVICE_TYPE type = (DEVICE_TYPE)root.Tag;
                bool check_all = true;

                switch (type)
                {
                    case DEVICE_TYPE.CAMERA:
                        foreach (TreeNode node in root.Nodes)
                        {
                            int i = (int)node.Tag;
                            node.Checked = channels_camera.contains(i);
                            if (node.Checked != true)
                            {
                                check_all = false;
                            }
                        }
                        root.Checked = check_all;
                        break;
                    case DEVICE_TYPE.ALARM_IN:
                        foreach (TreeNode node in root.Nodes)
                        {
                            int i = (int)node.Tag;
                            node.Checked = channels_alarm_in.contains(i);
                            if (node.Checked != true)
                            {
                                check_all = false;
                            }
                        }
                        root.Checked = check_all;
                        break;
                    case DEVICE_TYPE.ALARM_IN_NETWORK:
                        foreach (TreeNode node in root.Nodes)
                        {
                            int i = (int)node.Tag;
                            node.Checked = channels_alarm_in_network.contains(i);
                            if (node.Checked != true)
                            {
                                check_all = false;
                            }
                        }
                        root.Checked = check_all;
                        break;
                    case DEVICE_TYPE.AUDIO_IN:
                        foreach (TreeNode node in root.Nodes)
                        {
                            int i = (int)node.Tag;
                            node.Checked = channels_audio_in.contains(i);
                            if (node.Checked != true)
                            {
                                check_all = false;
                            }
                        }
                        root.Checked = check_all;
                        break;
                    case DEVICE_TYPE.TEXT_IN:
                        foreach (TreeNode node in root.Nodes)
                        {
                            int i = (int)node.Tag;
                            node.Checked = channels_text_in.contains(i);
                            if (node.Checked != true)
                            {
                                check_all = false;
                            }
                        }
                        root.Checked = check_all;
                        break;
                }
            }
        }
        protected bool imp_save()
        {
            G2SCOPE scope;
            if (imp_get_time_range(out scope) != true)
            {
                MessageBox.Show(this, "Invalid recording range", "Event Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            g2channel_set channels_camera = new g2channel_set();
            g2channel_set channels_alarm_in = new g2channel_set();
            g2channel_set channels_alarm_in_network = new g2channel_set();
            g2channel_set channels_audio_in = new g2channel_set();
            g2channel_set channels_text_in = new g2channel_set();
            g2channel_set channels_system = new g2channel_set();

            foreach (TreeNode root in TRV_DEVICES.Nodes)
            {
                switch ((DEVICE_TYPE)root.Tag)
                {
                    case DEVICE_TYPE.CAMERA: foreach (TreeNode node in root.Nodes) if (node.Checked) channels_camera.insert((int)node.Tag); break;
                    case DEVICE_TYPE.ALARM_IN: foreach (TreeNode node in root.Nodes) if (node.Checked) channels_alarm_in.insert((int)node.Tag); break;
                    case DEVICE_TYPE.ALARM_IN_NETWORK: foreach (TreeNode node in root.Nodes) if (node.Checked) channels_alarm_in_network.insert((int)node.Tag); break;
                    case DEVICE_TYPE.AUDIO_IN: foreach (TreeNode node in root.Nodes) if (node.Checked) channels_audio_in.insert((int)node.Tag); break;
                    case DEVICE_TYPE.TEXT_IN: foreach (TreeNode node in root.Nodes) if (node.Checked) channels_text_in.insert((int)node.Tag); break;
                }
            }

            bool selected_camera_event = false;
            foreach (CheckBox c in _col_camera)
            {
                if (c.Checked)
                {
                    selected_camera_event = true;
                    break;
                }
            }

            foreach (CheckBox c in _col_system)
            {
                if (c.Checked) channels_system.insert((int)c.Tag);
            }

            if (selected_camera_event)
            {
                if (channels_camera.empty())
                {
                    MessageBox.Show(this, "Please select camera device for searching camera-related event.", "Event Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
            }

            if (channels_camera.empty() &&
                channels_alarm_in.empty() &&
                channels_alarm_in_network.empty() &&
                channels_audio_in.empty() &&
                channels_text_in.empty() &&
                channels_system.empty())
            {
                MessageBox.Show(this, "Please select event search items.", "Event Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            _cameras.assign(channels_camera);

            if (_options_pre is G2SEARCH_G2_EVENT_SEARCH_OPTIONS)
            {
                _options._scope = scope;
                _options._alarm_in = channels_alarm_in;
                _options._alarm_in_network = channels_alarm_in_network;
                _options._audio_in = channels_audio_in;
                _options._text_in = channels_text_in;
                _options.clear_channels();

                foreach (CheckBox c in _col_camera)
                {
                    if (c.Checked)
                    {
                        switch ((G2EVENT.TYPE)c.Tag)
                        {
                            case G2EVENT.TYPE.MOTION_ON: _options._motion = channels_camera; break;
                            case G2EVENT.TYPE.OBJECT_TRACK_ON: _options._object = channels_camera; break;
                            case G2EVENT.TYPE.VIDEO_LOSS_ON: _options._video_loss = channels_camera; break;
                            case G2EVENT.TYPE.VIDEO_BLIND_ON: _options._video_blind = channels_camera; break;
                            case G2EVENT.TYPE.TRIPZONE_ON: _options._trip_zone = channels_camera; break;
                            case G2EVENT.TYPE.TAMPER_ON: _options._tamper = channels_camera; break;
                            case G2EVENT.TYPE.VIDEO_ANALYTICS_ON: _options._video_analytics = channels_camera; break;
                            case G2EVENT.TYPE.FACE_DETECTION_ON: _options._face_detect = channels_camera; break;
                            case G2EVENT.TYPE.INSTANT_RECORDING_ON: _options._instant_record = channels_camera; break;
                            case G2EVENT.TYPE.CAMERA_RECORD_BAD_ON: _options._camera_record_bad = channels_camera; break;
                            case G2EVENT.TYPE.CAMERA_FAN_ERROR_ON: _options._camera_fan_error = channels_camera; break;
                        }
                    }
                }

                if (channels_system.contains((int)G2EVENT.TYPE.PANIC_ON)) channels_system.insert((int)G2EVENT.TYPE.PANIC_OFF);
                if (channels_system.contains((int)G2EVENT.TYPE.DISK_SMART_ON)) channels_system.insert((int)G2EVENT.TYPE.DISK_SMART_OFF);
                if (channels_system.contains((int)G2EVENT.TYPE.DISK_TEMPERATURE_ON)) channels_system.insert((int)G2EVENT.TYPE.DISK_TEMPERATURE_OFF);

                _options._system_events = channels_system;
            }
            else if (_options_pre is G2EVENT_QUERY_CONDITION)
            {
                _options_G1.clear_channels();
                _options_G1._begin = scope._begin._time;
                _options_G1._end = scope._end._time;
                _options_G1._alarm_in = channels_alarm_in.to_uint32();
                _options_G1._alarm_in_network = channels_alarm_in_network.to_uint32();
                _options_G1._audio_in = channels_audio_in.to_uint32();
                _options_G1._text_in = channels_text_in.to_uint32();

                uint cameras = channels_camera.to_uint32();
                foreach (CheckBox c in _col_camera)
                {
                    if (c.Checked)
                    {
                        switch ((G2EVENT.TYPE)c.Tag)
                        {
                            case G2EVENT.TYPE.MOTION_ON: _options_G1._motion = cameras; break;
                            case G2EVENT.TYPE.OBJECT_TRACK_ON: _options_G1._object = cameras; break;
                            case G2EVENT.TYPE.VIDEO_LOSS_ON: _options_G1._video_loss = cameras; break;
                            case G2EVENT.TYPE.VIDEO_BLIND_ON: _options_G1._video_blind = cameras; break;
                            case G2EVENT.TYPE.TRIPZONE_ON: _options_G1._trip_zone = cameras; break;
                            case G2EVENT.TYPE.TAMPER_ON: _options_G1._tamper = cameras; break;
                            case G2EVENT.TYPE.VIDEO_ANALYTICS_ON: _options_G1._video_analytics = cameras; break;
                            case G2EVENT.TYPE.FACE_DETECTION_ON: _options_G1._face_detect = cameras; break;
                            case G2EVENT.TYPE.INSTANT_RECORDING_ON: _options_G1._instant_record = cameras; break;
                        }
                    }
                }

                g2channel_set temp = new g2channel_set();
                foreach (G2EVENT.TYPE evt in channels_system)
                {
                    if (evt == G2EVENT.TYPE.RECORDER_BAD_ON) temp.insert((int)G2EVENT_QUERY_CONDITION.SYSTEM_EVENT_TYPE.SYSTEM_EVENT_RECORDER_BAD);
                    else if (evt == G2EVENT.TYPE.ALARM_IN_BAD_ON) temp.insert((int)G2EVENT_QUERY_CONDITION.SYSTEM_EVENT_TYPE.SYSTEM_EVENT_ALARM_IN_BAD);
                    else if (evt == G2EVENT.TYPE.TEXT_IN_BAD_ON) temp.insert((int)G2EVENT_QUERY_CONDITION.SYSTEM_EVENT_TYPE.SYSTEM_EVENT_TEXT_IN_BAD);
                    else if (evt == G2EVENT.TYPE.TANGO_FULL_ON) temp.insert((int)G2EVENT_QUERY_CONDITION.SYSTEM_EVENT_TYPE.SYSTEM_EVENT_TANGO_FULL);
                    else if (evt == G2EVENT.TYPE.DISK_BAD) temp.insert((int)G2EVENT_QUERY_CONDITION.SYSTEM_EVENT_TYPE.SYSTEM_EVENT_DISK_BAD);
                    else if (evt == G2EVENT.TYPE.DISK_TEMPERATURE_ON) temp.insert((int)G2EVENT_QUERY_CONDITION.SYSTEM_EVENT_TYPE.SYSTEM_EVENT_DISK_TEMPERATURE);
                    else if (evt == G2EVENT.TYPE.DISK_SMART_ON) temp.insert((int)G2EVENT_QUERY_CONDITION.SYSTEM_EVENT_TYPE.SYSTEM_EVENT_DISK_SMART);
                    else if (evt == G2EVENT.TYPE.TANGO_ALMOST_FULL_ON) temp.insert((int)G2EVENT_QUERY_CONDITION.SYSTEM_EVENT_TYPE.SYSTEM_EVENT_TANGO_ALMOST_FULL);
                    else if (evt == G2EVENT.TYPE.DISK_ON) temp.insert((int)G2EVENT_QUERY_CONDITION.SYSTEM_EVENT_TYPE.SYSTEM_EVENT_DISK_ON);
                    else if (evt == G2EVENT.TYPE.DISK_OFF) temp.insert((int)G2EVENT_QUERY_CONDITION.SYSTEM_EVENT_TYPE.SYSTEM_EVENT_DISK_OFF);
                    else if (evt == G2EVENT.TYPE.DISK_CONFIG_CHANGE) temp.insert((int)G2EVENT_QUERY_CONDITION.SYSTEM_EVENT_TYPE.SYSTEM_EVENT_DISK_CONFIG_CHANGE);
                    else if (evt == G2EVENT.TYPE.PANIC_ON) temp.insert((int)G2EVENT_QUERY_CONDITION.SYSTEM_EVENT_TYPE.SYSTEM_EVENT_PANIC);
                    else if (evt == G2EVENT.TYPE.FAN_ERROR_ON) temp.insert((int)G2EVENT_QUERY_CONDITION.SYSTEM_EVENT_TYPE.SYSTEM_EVENT_DC_FAN_ERROR);
                    else if (evt == G2EVENT.TYPE.GPS_RECEIVE_ERROR_ON) temp.insert((int)G2EVENT_QUERY_CONDITION.SYSTEM_EVENT_TYPE.SYSTEM_EVENT_GPS_RECEIVE_ERROR);
                    else if (evt == G2EVENT.TYPE.LOGIN_FAILED_SEVERAL_TIMES) temp.insert((int)G2EVENT_QUERY_CONDITION.SYSTEM_EVENT_TYPE.SYSTEM_EVENT_LOGIN_FAILED_SEVERAL_TIMES);
                }

                _options_G1._system_events = temp.to_uint32();
            }
            return true;
        }

        protected void imp_set_device()
        {
            TRV_DEVICES.Nodes.Clear();
            TRV_DEVICES.AfterCheck += new TreeViewEventHandler(on_tree_device_after_check);

            if (_count_camera > 0) TRV_DEVICES.Nodes.Add(imp_build_node("Camera", DEVICE_TYPE.CAMERA, _count_camera));
            if (_count_alarm_in > 0) TRV_DEVICES.Nodes.Add(imp_build_node("Alarm-In", DEVICE_TYPE.ALARM_IN, _count_alarm_in));
            if (_count_alarn_in_network > 0) TRV_DEVICES.Nodes.Add(imp_build_node("Network Alarm-In", DEVICE_TYPE.ALARM_IN_NETWORK, _count_alarn_in_network));
            if (_count_audio_in > 0) TRV_DEVICES.Nodes.Add(imp_build_node("Audio", DEVICE_TYPE.AUDIO_IN, _count_audio_in));
            if (_count_text_in > 0) TRV_DEVICES.Nodes.Add(imp_build_node("Text-In", DEVICE_TYPE.TEXT_IN, _count_text_in));

            foreach (TreeNode node in TRV_DEVICES.Nodes)
            {
                node.ExpandAll();
            }
        }
        protected bool imp_set_event_camera(int x, int y, out int bottom)
        {
            G2EVENT.TYPE[] events = new G2EVENT.TYPE[] {
                G2EVENT.TYPE.MOTION_ON,
                G2EVENT.TYPE.OBJECT_TRACK_ON,
                G2EVENT.TYPE.VIDEO_LOSS_ON,
                G2EVENT.TYPE.VIDEO_BLIND_ON,
                G2EVENT.TYPE.VIDEO_ANALYTICS_ON,
                G2EVENT.TYPE.TRIPZONE_ON,
                G2EVENT.TYPE.TAMPER_ON,
                G2EVENT.TYPE.FACE_DETECTION_ON,
                G2EVENT.TYPE.INSTANT_RECORDING_ON,
                G2EVENT.TYPE.CAMERA_RECORD_BAD_ON,
                G2EVENT.TYPE.CAMERA_FAN_ERROR_ON
            };
            return imp_set_event(_col_camera, events, x, y, out bottom);
        }
        protected bool imp_set_event_system(int x, int y, out int bottom)
        {
            G2EVENT.TYPE[] events = new G2EVENT.TYPE[] {
                G2EVENT.TYPE.RECORDER_BAD_ON,
                G2EVENT.TYPE.ALARM_IN_BAD_ON,
                G2EVENT.TYPE.TEXT_IN_BAD_ON,
                G2EVENT.TYPE.TANGO_FULL_ON,
                G2EVENT.TYPE.DISK_BAD,
                G2EVENT.TYPE.DISK_TEMPERATURE_ON,
                G2EVENT.TYPE.DISK_SMART_ON,
                G2EVENT.TYPE.TANGO_ALMOST_FULL_ON,
                G2EVENT.TYPE.DISK_ON, G2EVENT.TYPE.DISK_OFF,
                G2EVENT.TYPE.DISK_CONFIG_CHANGE,
                G2EVENT.TYPE.PANIC_ON,
                G2EVENT.TYPE.FAN_ERROR_ON,
                G2EVENT.TYPE.GPS_RECEIVE_ERROR_ON,
                G2EVENT.TYPE.LOGIN_FAILED_SEVERAL_TIMES,
            };
            return imp_set_event(_col_system, events, x, y, out bottom);
        }
        protected bool imp_set_event(List<Control> res, G2EVENT.TYPE[] events, int x, int y, out int bottom)
        {
            List<G2EVENT.TYPE> choice = new List<G2EVENT.TYPE>();
            foreach (G2EVENT.TYPE e in events)
            {
                if (_caps_event.contains((int)e))
                {
                    choice.Add(e);
                }
            }

            if (choice.Count == 0)
            {
                bottom = y;
                return false;
            }

            int count = choice.Count;
            int col = 2;
            int row = (count / col) + (count % col);
            int len = TRV_DEVICES.Size.Width / col;

            for (int i = 0; i < col; ++i)
            {
                for (int j = 0; j < row; ++j)
                {
                    int pos = j * col + i;
                    if (pos < choice.Count)
                    {
                        CheckBox c = new CheckBox();
                        c.Tag = choice[pos];
                        c.AutoSize = true;
                        c.Location = new Point(i * len + x, j * 18 + y);
                        c.Text = G2EVENT.string_event_type(choice[pos]);
                        c.UseVisualStyleBackColor = true;
                        res.Add(c);
                    }
                }
            }

            bottom = res[res.Count - 1].Bottom;
            return true;
        }
        protected TreeNode imp_build_node(string name, DEVICE_TYPE tag, int count)
        {
            TreeNode node = new TreeNode(name);
            node.Tag = tag;
            for (int i = 0; i < count; ++i)
            {
                node.Nodes.Add(name + ((int)(i + 1)).ToString()).Tag = i;
            }
            return node;
        }
        protected Label imp_create_seperator(Rectangle rect)
        {
            Label c = new Label();
            c.AutoSize = false;
            c.Location = rect.Location;
            c.Size = rect.Size;
            c.BorderStyle = BorderStyle.None;
            c.BackColor = SystemColors.ActiveBorder;
            return c;
        }
        protected bool imp_get_time_range(out G2SCOPE scope)
        {
            scope = new G2SCOPE();

            if (CHK_FIRST.Checked) scope._begin._time.reset();
            else scope._begin._time = DTP_FROM.Value;
            if (CHK_LAST.Checked) scope._end._time.reset();
            else scope._end._time = DTP_TO.Value;

            return (scope._begin._time.valid && scope._end._time.valid &&
                   (scope._begin._time >= scope._end._time)) ? false : true;
        }

        private void on_chk_first(object sender, EventArgs e)
        {
            bool enable = (CHK_FIRST.Checked != true);
            DTP_FROM.Enabled = enable;
        }
        private void on_chk_last(object sender, EventArgs e)
        {
            bool enable = (CHK_LAST.Checked != true);
            DTP_TO.Enabled = enable;
        }
        private void on_dtp_mouse_wheel(object sender, MouseEventArgs e)
        {
            SendKeys.Send(e.Delta > 0 ? "{UP}" : "{DOWN}");
        }
        private void on_tree_device_after_check(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
                if (e.Node.Nodes.Count > 0)
                {
                    foreach (TreeNode node in e.Node.Nodes)
                    {
                        node.Checked = e.Node.Checked;
                    }
                }
            }
        }
        private void on_btn_OK(object sender, EventArgs e)
        {
            if (imp_save())
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private G2_PRODUCT_INFO _pi;
        private G2TIME _range_from;
        private G2TIME _range_to;
        private g2channel_set _cameras;
        private g2set<int> _caps_event;
        private object _options_pre;
        private int _count_camera;
        private int _count_alarm_in;
        private int _count_alarn_in_network;
        private int _count_audio_in;
        private int _count_text_in;
        private List<Control> _col_camera;
        private List<Control> _col_system;
        public G2SEARCH_G2_EVENT_SEARCH_OPTIONS _options;
        public G2EVENT_QUERY_CONDITION _options_G1;
    }
}
