using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Text;

using GDK;

namespace GDK_tester
{
    public partial class form_live : g2live_listener
    {
        private g2live _adaptor;
        private g2admin _admin_adaptor;
        private g2user _user_adaptor;
        private List<liveinfo> _liveinfo_list;
        private int _screen_channel;
        private int _total_camera_count;
        private int _panel_number;

        public void init_connective_live()
        {
            _adaptor = new g2live();
            _adaptor.set_listener(this);
            _adaptor.startup(2, Handle);
            _screen_channel = -1;
        }

        public bool connect_live()
        {
            bool connect_res = false; 

            lock (_adaptor)
            {
                G2CONNECT_RES res;
                for (int i = 0; i < _liveinfo_list.Count; ++i)
                {
                    G2GUID service = _liveinfo_list[i].streaming_service;
                    int channel = _adaptor.connect(ref service, out res);
                    _liveinfo_list[i].adaptor_channel = channel;
                    if (channel >= 0) connect_res = true;
                }
            }

            if (connect_res)
            {
                _screen_channel = 0;
                _screen.buf_setup(_screen_channel, _total_camera_count * 4, frame_buf.TYPE.LIVE);
            }

            return connect_res;
        }

        public void on_post_receive_stream_channels(int channel, Dictionary<G2GUID, G2LIVE_CHANNEL_INFO> chs)
        {
            g2channel_set channels = new g2channel_set();

            for (int i = 0; i < _liveinfo_list.Count; ++i)
            {
                if (_liveinfo_list[i].adaptor_channel == channel)
                {
                    for (int j = 0; j < _liveinfo_list[i].camera_info_list.Count; ++j)
                    {
                        if (chs.ContainsKey(_liveinfo_list[i].camera_info_list[j]._guid))
                        {
                            int channelext = chs[_liveinfo_list[i].camera_info_list[j]._guid]._channelext;
                            if (!_liveinfo_list[i].channelext_list.Contains(channelext))
                            {
                                _liveinfo_list[i].channelext_list.Add(channelext);
                            }
                        }
                    }

                    channels.insert(_liveinfo_list[i].channelext_list.ToArray());

                    break;
                }
            }

            _adaptor.set_camera_list(channel, channels, true);
            _adaptor.set_camera_list_interest(channel, channels, true);
            _screen.resume();
        }

        public void on_post_receive_camera_status(int channel, G2LIVE_CAMERA_STATUS[] bunch)
        {
            bool init = _panel_number == 0;
            G2GUIDSET cameras = new G2GUIDSET();

            for (int i = 0; i < _liveinfo_list.Count; ++i)
            {
                if (_liveinfo_list[i].adaptor_channel == channel)
                {
                    for (int j = 0; j < bunch.Length; ++j)
                    {
                        if (_liveinfo_list[i].channelext_panel_dic.ContainsKey(bunch[j]._channelext) == false)
                        {
                            _liveinfo_list[i].channelext_panel_dic[bunch[j]._channelext] = _panel_number;
                            _liveinfo_list[i].camera_guid_panel_dic[bunch[j]._guid] = _panel_number++;

                            if (init)
                            {
                                cameras.Add(bunch[j]._guid);
                            }
                        }

                        int panel_num = _liveinfo_list[i].channelext_panel_dic[bunch[j]._channelext];

                        camera_pane cp = _screen.get_pane(panel_num);
                        cp._channelext = bunch[j]._channelext;
                        G2GUID camera_guid = bunch[j]._guid;
                        cp._stream_id = _adaptor.get_stream_id(channel, ref camera_guid);
                        camera_pane.STATUS status = camera_pane.STATUS.UNDEFINED;
                        if (bunch[j].camera == G2LIVE_CAMERA_STATUS.CAMERA.ACTIVE ||
                            bunch[j].camera == G2LIVE_CAMERA_STATUS.CAMERA.MULTISTREAM) status = camera_pane.STATUS.ENABLE;
                        else if (bunch[j].camera == G2LIVE_CAMERA_STATUS.CAMERA.INACTIVE) status = camera_pane.STATUS.INACTIVATE;
                        else if (bunch[j].camera == G2LIVE_CAMERA_STATUS.CAMERA.VIDEOLOSS) status = camera_pane.STATUS.NO_VIDEO;
                        else if (bunch[j].camera == G2LIVE_CAMERA_STATUS.CAMERA.NOTCONNECTED) status = camera_pane.STATUS.NOT_CONNECTED;
                        _screen.set_pane_status(panel_num, status, false);
                        string title = _liveinfo_list[i].get_camera_name(camera_guid)._string;
                        _screen.set_pane_title(panel_num, title, false);
                    }

                    break;
                }
            }

            _screen.update();

            _screen.fire_changed_pane(true);

            if (init)
            {
                G2GUIDSET roots = new G2GUIDSET();
                for (int i = 0; i < cameras.Count; ++i)
                {
                    G2GUID camera = cameras[i];
                    G2GUID root = _admin_adaptor.get_root_guid_from_leaf_guid(ref camera);
                    if (!roots.contains(root))
                    {
                        roots.Add(root);
                    }
                }
                _admin_adaptor.request_recording_device_status(roots, false);
                _admin_adaptor.request_recording_device_status(roots, true);
            }
        }

        #region g2live_listener 멤버

        public void on_g2live_connected(int handle, int channel)                                                                // 1
        {
            _adaptor.request_stream_channels(channel);
            _screen.message().hide(STRING.NIS_CONNECTING);
            feature_control_alarm_out(channel);
        }

        public void on_g2live_receive_stream_channels(int handle, int channel, Dictionary<G2GUID, G2LIVE_CHANNEL_INFO> chs)     // 2
        {
            on_post_receive_stream_channels(channel, chs);
        }

        public void on_g2live_disconnected(int handle, int channel, G2DISCONNECT_REASON.TYPE reason)
        {
            for (int i = 0; i < _liveinfo_list.Count; ++i)
            {
                if (_liveinfo_list[i].adaptor_channel == channel)
                {
                    _liveinfo_list[i].adaptor_channel = -1;
                    break;
                }
            }

            bool every_disconnected = true;
            for (int i = 0; i < _liveinfo_list.Count; ++i)
            {
                if (_liveinfo_list[i].adaptor_channel > -1)
                {
                    every_disconnected = false;
                    break;
                }
            }

            if (every_disconnected)
            {
                if (reason != G2DISCONNECT_REASON.TYPE.LOGOUT)
                {
                    string error = g2foundation.get_string_disconnect_reason(reason);
                    _screen.message().disp(error, 60 * 1000, false);
                }
                else
                {
                    _screen.message().hide(STRING.NIS_CONNECTING);
                    _screen.message().hide(STRING.NIS_LOADING_RECORD_TIME);
                }
            }
        }

        public void on_g2live_probe_session_profile(int handle, int channel, ref G2PROBE_SESSION_PROFILE probe)
        {
            Debug.WriteLine("callback [on_g2live_probe_session_profile] is not implemented.");
        }

        public void on_g2live_receive_audio_out_not_available(int handle, int channel, ref G2GUID root)
        {
            Debug.WriteLine("callback [on_g2live_receive_audio_out_not_available] is not implemented.");
        }

        public void on_g2live_receive_camera_status(int handle, int channel, G2LIVE_CAMERA_STATUS[] bunch)                      // 3
        {
            on_post_receive_camera_status(channel, bunch);
        }

        public void on_g2live_receive_event(int handle, int channel, ref G2EVENT_INFO ei)
        {
            Debug.WriteLine("callback [on_g2live_receive_event] is not implemented.");
        }

        public void on_g2live_receive_frame_data(int handle, int channel, ref G2FRAME frame)                                    // 4
        {
            for (int i = 0; i < _liveinfo_list.Count; ++i)
            {
                if (_liveinfo_list[i].adaptor_channel == channel)
                {
                    if (_liveinfo_list[i].channelext_panel_dic.ContainsKey(frame.channel))
                    {
                        if (_transfer_all)
                        {
                            _screen.put_frame(ref frame, _screen_channel, frame.stream_id);
                        }
                        else
                        {
                            if (_liveinfo_list[i].channelext_panel_dic.ContainsKey(frame.channel))
                            {
                                int pane_num = _liveinfo_list[i].channelext_panel_dic[frame.channel];
                                _screen.put_frame(ref frame, _screen_channel, pane_num);
                            }
                        }
                    }
                    break;
                }
            }
        }

        public void on_g2live_receive_network_alarm_result(int handle, int channel, ref G2GUID root, ref G2LIVE_NETWORK_ALARM_RESULT result)
        {
            Debug.WriteLine("callback [on_g2live_receive_network_alarm_result] is not implemented.");
        }

        public void on_g2live_receive_notify_append_device(int handle, int channel, ref G2GUID root)
        {
            Debug.WriteLine("callback [on_g2live_receive_notify_append_device] is not implemented.");
        }

        public void on_g2live_receive_notify_remove_device(int handle, int channel, ref G2GUID root)
        {
            Debug.WriteLine("callback [on_g2live_receive_notify_remove_device] is not implemented.");
        }

        public void on_g2live_receive_ptz_menu(int handle, int channel, ref G2LIVE_PTZ_MENU menu)
        {
            G2LIVE_PTZ_MENU info = menu;
            this.BeginInvoke((MethodInvoker)delegate() { on_post_live_receive_ptz_menu(channel, ref info); });
        }

        public void on_g2live_receive_ptz_preset(int handle, int channel, ref G2LIVE_PTZ_PRESET preset)
        {
            G2LIVE_PTZ_PRESET info = preset;
            this.BeginInvoke((MethodInvoker)delegate() { on_post_live_receive_ptz_preset(channel, ref info); });
        }

        public void on_g2live_receive_service_log_load(int handle, int channel, ref G2SYSTEM_LOG log)
        {
            Debug.WriteLine("callback [on_g2live_receive_service_log_load] is not implemented.");
        }

        public void on_g2live_receive_service_log_load_end(int handle, int channel)
        {
            Debug.WriteLine("callback [on_g2live_receive_service_log_load_end] is not implemented.");
        }

        public void on_g2live_receive_service_log_load_fail(int handle, int channel)
        {
            Debug.WriteLine("callback [on_g2live_receive_service_log_load_fail] is not implemented.");
        }

        public void on_g2live_receive_service_log_load_stop(int handle, int channel)
        {
            Debug.WriteLine("callback [on_g2live_receive_service_log_load_stop] is not implemented.");
        }

        public void on_g2live_receive_text_in(int handle, int channel, ref G2TEXT_IN data)
        {
            Debug.WriteLine("callback [on_g2live_receive_text_in] is not implemented.");
        }

        public void on_g2live_sound_capturing_started(int handle, int channel, ref G2GUID root, int camera)
        {
            Debug.WriteLine("callback [on_g2live_sound_capturing_started] is not implemented.");
        }

        public void on_g2live_sound_capturing_stopped(int handle, int channel, ref G2GUID root, int camera)
        {
            Debug.WriteLine("callback [on_g2live_sound_capturing_stopped] is not implemented.");
        }

        public void on_g2live_sound_streaming_started(int handle, int channel, int channelext)
        {
            Debug.WriteLine("callback [on_g2live_sound_streaming_started] is not implemented.");
        }

        public void on_g2live_sound_streaming_stopped(int handle, int channel, int channelext)
        {
            Debug.WriteLine("callback [on_g2live_sound_streaming_stopped] is not implemented.");
        }

        public void on_g2live_receive_debug_log_load(int handle, int channel, ref G2DEBUG_LOG log)
        {
            Debug.WriteLine("callback [on_g2live_receive_debug_log_load] is not implemented.");
        }

        public void on_g2live_receive_debug_log_load_end(int handle, int channel)
        {
            Debug.WriteLine("callback [on_g2live_receive_debug_log_load_end] is not implemented.");
        }

        public void on_g2live_receive_debug_log_load_fail(int handle, int channel)
        {
            Debug.WriteLine("callback [on_g2live_receive_debug_log_load_fail] is not implemented.");
        }

        public void on_g2live_receive_debug_log_load_stop(int handle, int channel)
        {
            Debug.WriteLine("callback [on_g2live_receive_debug_log_load_stop] is not implemented.");
        }

        #endregion
    }
}
