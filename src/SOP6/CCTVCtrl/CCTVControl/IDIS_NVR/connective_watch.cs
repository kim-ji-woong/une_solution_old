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
    using G2HWATCH = System.Int32;

    public partial class IdisNvrSet : g2watch_listener
    {
        private g2watch _adaptor = null;
        private string _sitename;
        private int _channel;
        private int _count_camera;
        private int _count_stream;
        private bool _transfer_multi_stream;

        private void init_connective_watch()
        {
            _adaptor = new g2watch();
            _adaptor.set_listener(this);
            _adaptor.startup(1, m_owner.Handle);
            _sitename = " ";
            _channel = -1;
            _count_camera = 0;
            _count_stream = 0;
            _transfer_multi_stream = false;
        }

        public bool valid_channel(int channel)
        {
            return channel >= 0;
        }

        private void on_post_watch_connected(int channel)
        {
            /*BTN_DISCONNECT.Enabled = true;
            BTN_REQ_DEVICE_STATUS.Enabled = true;
            BTN_REMOTE_SETUP.Enabled = (File.Exists(@".\remote_setup\G2RemoteConf.exe") &&
                                        _adaptor.is_authority(channel, G2RAS_AUTHORITY.TYPE.AUTHORITY_SETUP));
            BTN_NETWORK_ALARM.Enabled = _adaptor.is_support(channel, G2LIVE_SUPPORT.QUERY.NETWORK_ALARM_G2);
            BTN_INSTANT_RECORDING.Enabled = _adaptor.is_support(channel, G2LIVE_SUPPORT.QUERY.INSTANT_RECORDING) &&
                                            _adaptor.is_authority(channel, G2RAS_AUTHORITY.TYPE.AUTHORITY_RECORD_SETUP);
            LSV_EVENT.Items.Clear();
            LSV_EVENT.ForeColor = SystemColors.WindowText;*/

            G2_PRODUCT_INFO pi;
            _adaptor.get_product_info(channel, out pi);
            //_alarm_out.set_alarm_out_count(pi.device.count_alarm_out);
            //_alarm_out.set_beep_control_enable(_adaptor.is_support(channel, G2LIVE_SUPPORT.QUERY.BEEP_CONTROL));
            _screen.message().hide(STRING.NIS_CONNECTING);

            feature_control_ptz_set_product_info(ref pi);
            feature_control_color_set_product_info(ref pi);

            _screen.set_pane_select(m_nChannel - 1);
            _screen.set_format2(GDK_tester.screen_format.FORMAT.LAYOUT1X1, true, m_nChannel - 1);

            SetMultiStream();

            if (m_owner != null)
                m_owner.OnConnected(this);
        }
        private void on_post_watch_disconnected(int channel, G2DISCONNECT_REASON.TYPE reason)
        {
            if (reason != G2DISCONNECT_REASON.TYPE.LOGOUT)
            {
                string error = g2foundation.get_string_disconnect_reason(reason);
                _screen.message().disp(error, 5 * 1000, false);
            }
            else
            {
                _screen.message().hide(STRING.NIS_CONNECTING);
            }

            /*BTN_CONNECT.Enabled = true;
            BTN_DISCONNECT.Enabled = false;
            BTN_REQ_DEVICE_STATUS.Enabled = false;
            BTN_REMOTE_SETUP.Enabled = false;
            BTN_NETWORK_ALARM.Enabled = false;
            BTN_INSTANT_RECORDING.Enabled = false;
            LSV_EVENT.ForeColor = Color.DimGray;
            STC_PROBE_PERF.Text = "";*/

            _sitename = " ";
            //_alarm_out.reset();

            m_prevConnection = null;

            if (m_owner != null)
                m_owner.OnDisconnected(this);

            m_owner.BeginInvoke((MethodInvoker)delegate() { on_post_watch_update_sitename(); });
        }
        private void on_post_watch_receive_event(int channel, ref G2EVENT_INFO ei)
        {
            if (ei.is_event_ignored()) return;
            if (ei.is_event_off()) return;

            string source = (ei.is_event_camera()) ? string.Format("Camera {0}", ei._event_ras._channel + 1) :
                            (ei.is_event_alarm()) ? string.Format("Alarm-In {0}", ei._event_ras._channel + 1) :
                            (ei.is_event_audio()) ? string.Format("Audio-In {0}", ei._event_ras._channel + 1) :
                            (ei.is_event_text_in()) ? string.Format("Text-In {0}", ei._event_ras._channel + 1) :
                            (ei.is_event_alarm_network()) ? string.Format("Network Alarm-In {0}", ei._event_ras._channel + 1) :
                            (ei.is_event_dvr_system()) ? _sitename : "";
            string type = ei.string_event_type();
            string data = ei._event_ras._label;
            string time = ei._spot._time.to_string_date_time();

            /*ListViewItem lvi = new ListViewItem(type);
            lvi.SubItems.Add(source);
            lvi.SubItems.Add(data);
            lvi.SubItems.Add(time);
            LSV_EVENT.Items.Insert(0, lvi);

            if (LSV_EVENT.Items.Count > 10000)
            {
                LSV_EVENT.Items.RemoveAt(LSV_EVENT.Items.Count - 1);
            }*/
        }
        private void on_post_watch_receive_network_alarm_result(int channel, ref G2LIVE_NETWORK_ALARM_RESULT result)
        {
            string info = string.Format("{0}({1}):{2}", result._event.string_event_type(), result._event._channel, result.result.ToString());
            _screen.message().disp(info, 5 * 1000, false);
        }
        private void on_post_watch_update_sitename()
        {
            //string title = "GDK W/a/t/c/h ";
            //this.Text = title + _sitename;
        }

        public void on_g2watch_connected(G2HWATCH handle, int channel)
        {
            lock (_adaptor)
            {
                if (_channel != channel)
                {
                    _adaptor.disconnect(channel);
                    return;
                }
            }

            G2RAS_AUTHORITY auth;
            if (_adaptor.get_authority(channel, out auth))
            {
                feature_control_color_set_authority(ref auth);
                feature_control_PTZ_set_authority(ref auth);
                feature_control_alarm_out_set_authority(ref auth);
            }
            else
            {
                feature_control_color_set_enable(false);
                feature_control_PTZ_set_enable(false);
                feature_control_alarm_out_set_enable(false);
                feature_control_audio_set_enable(false);
                feature_control_multi_stream_set_enable(false);
            }

            G2_PRODUCT_INFO pi;
            if (_adaptor.get_product_info(channel, out pi))
            {
                _count_camera = pi.device.count_camera;
                _count_stream = pi.remote_watch.stream_count;

                if (pi.ip_camera.dewarping != (byte)G2_PRODUCT_INFO_CAPS.IP_CAMERA.DEWARPING_TYPE.NOT_SUPPORT)
                {

                }
            }

            g2channel_set cameras = new g2channel_set(0, _count_camera);

            int[] panes = new int[_count_camera];
            for (int i = 0; i < _count_camera; ++i)
            {
                panes[i] = i;
            }

            _screen.reset(false);

            screen_channel_set_update(channel);

            _screen.set_pane_select(0, false);
            _screen.set_format(_count_camera, true);
            _screen.set_pane_mode(panes, GDK_tester.camera_pane.MODE.LIVE);
            _screen.resume();
            _adaptor.set_camera_list(channel, cameras, false);

            if (_finalize != true)
            {
                m_owner.BeginInvoke((MethodInvoker)delegate() { on_post_watch_connected(channel); });
            }
        }

        public void on_g2watch_disconnected(G2HWATCH handle, int channel, G2DISCONNECT_REASON.TYPE reason)
        {
            lock (_adaptor)
            {
                if (_channel != channel)
                {
                    return;
                }
            }

            _screen.buf_reset(channel);
            _screen.set_pane_select(0);
            _screen.set_format(1, false);
            _screen.reset(true);
            _screen.suspend();
            _screen.decoder().close();
            _count_camera = 0;
            _count_stream = 0;
            _transfer_multi_stream = false;
            _channel = -1;

            feature_control_color_set_enable(false);
            feature_control_PTZ_set_enable(false);
            feature_control_alarm_out_set_enable(false);
            feature_control_audio_set_enable(false);
            feature_control_multi_stream_set_enable(false);

            if (_finalize != true)
            {
                m_owner.BeginInvoke((MethodInvoker)delegate() { on_post_watch_disconnected(channel, reason); });
            }
        }

        public void on_g2watch_receive_frame_data(G2HWATCH handle, int channel, ref G2FRAME frame)
        {
            int pane = (_transfer_multi_stream) ? frame.stream_id : frame.channel;

            _screen.put_frame(ref frame, channel, pane);
        }

        public void on_g2watch_receive_audio_data(G2HWATCH handle, int channel, ref G2FRAME frame){ }
        
        public void on_g2watch_receive_event(G2HWATCH handle, int channel, ref G2EVENT_INFO ei)
        {
            G2EVENT_INFO info = ei;
            m_owner.BeginInvoke((MethodInvoker)delegate() { on_post_watch_receive_event(channel, ref info); });
        }

        public void on_g2watch_receive_device_status(G2HWATCH handle, int channel, ref G2DEVICE_STATUS status)
        {
            screen_set_device_status(channel, ref status);

            if (_adaptor.is_support(channel, G2LIVE_SUPPORT.QUERY.MULTI_STREAM))
            {
                feature_control_multi_stream_status_update();
            }

            _screen.fire_changed_pane(true);
            //_alarm_out.set_alarm_out_status(status._alarm_out);

            if (_sitename != status._sitename)
            {
                _sitename = status._sitename;

                if (_sitename.Length == 0)
                {
                    _sitename = "<no name>";
                }

                m_owner.BeginInvoke((MethodInvoker)delegate() { on_post_watch_update_sitename(); });
            }
        }

        public void on_g2watch_receive_ptz_preset(G2HWATCH handle, int channel, int camera, ref G2LIVE_PTZ_PRESET preset)
        {
            G2LIVE_PTZ_PRESET info = preset;
            m_owner.BeginInvoke((MethodInvoker)delegate() { on_post_watch_receive_ptz_preset(channel, ref info); });
        }
        public void on_g2watch_receive_ptz_menu(G2HWATCH handle, int channel, int camera, ref G2LIVE_PTZ_MENU menu)
        {
            G2LIVE_PTZ_MENU info = menu;
            m_owner.BeginInvoke((MethodInvoker)delegate() { on_post_watch_receive_ptz_menu(channel, ref info); });
        }
        public void on_g2watch_receive_camera_title_idr(G2HWATCH handle, int channel, int camera, string title) { }
        public void on_g2watch_receive_text_in(G2HWATCH handle, int channel, ref G2TEXT_IN data) { }
        public void on_g2watch_receive_network_camera_information(G2HWATCH handle, int channel) { }
        public void on_g2watch_receive_audio_out_not_available(G2HWATCH handle, int channel)
        {
            m_owner.BeginInvoke((MethodInvoker)delegate() { on_post_watch_receive_audio_out_not_available(); });
        }
        public void on_g2watch_receive_command_result_control_color_status(G2HWATCH handle, int channel, int camera, ref G2LIVE_COMMAND_CONTROL_COLOR control, ref G2LIVE_COMMAND_CONTROL_COLOR_RANGE range)
        {
            feature_control_color_update_status(channel, camera, true);
        }
        public void on_g2watch_receive_command_result_control_color(G2HWATCH handle, int channel, int camera, ref G2LIVE_COMMAND_CONTROL_COLOR control, G2LIVE_COMMAND_RESULT.TYPE result)
        {
            if (result == G2LIVE_COMMAND_RESULT.TYPE.SUCCESS)
            {
                feature_control_color_update_status(channel, camera, true);
            }
        }
        public void on_g2watch_receive_command_result_control_ptz_status(G2HWATCH handle, int channel, int camera, ref G2LIVE_COMMAND_CONTROL_PTZ control, ref G2LIVE_COMMAND_CONTROL_PTZ_RANGE range)
        {
            G2LIVE_COMMAND_CONTROL_PTZ c = control;
            G2LIVE_COMMAND_CONTROL_PTZ_RANGE r = range;
            m_owner.BeginInvoke((MethodInvoker)delegate() { on_post_watch_receive_ptz_command_control_status(channel, camera, ref c, ref r); });
        }
        public void on_g2watch_receive_command_result_control_ptz(G2HWATCH handle, int channel, int camera, G2LIVE_COMMAND_RESULT.TYPE result) { }
        public void on_g2watch_receive_network_alarm_result(G2HWATCH handle, int channel, ref G2LIVE_NETWORK_ALARM_RESULT result)
        {
            G2LIVE_NETWORK_ALARM_RESULT info = result;
            m_owner.BeginInvoke((MethodInvoker)delegate() { on_post_watch_receive_network_alarm_result(channel, ref info); });
        }
        public void on_g2watch_receive_elevator_status_info_response(G2HWATCH handle, int channel, uint seq_number) { }
        public void on_g2watch_receive_instant_recording_start(G2HWATCH handle, int channel, G2INSTANT_RECORDING_RESULT.TYPE result, G2INSTANT_RECORDING_CHANNEL_STATUS[] status)
        {
            m_owner.BeginInvoke((MethodInvoker)delegate() { on_post_watch_receive_instant_recording_start(channel, result, status); });
        }
        public void on_g2watch_receive_instant_recording_stop(G2HWATCH handle, int channel, G2INSTANT_RECORDING_RESULT.TYPE result)
        {
            m_owner.BeginInvoke((MethodInvoker)delegate() { on_post_watch_receive_instant_recording_stop(channel, result); });
        }
        public void on_g2watch_receive_instant_recording_status(G2HWATCH handle, int channel, G2INSTANT_RECORDING_RESULT.TYPE result, G2INSTANT_RECORDING_CHANNEL_STATUS[] status)
        {
            m_owner.BeginInvoke((MethodInvoker)delegate() { on_post_watch_receive_instant_recording_status(channel, result, status); });
        }
        public void on_g2watch_audio_streaming_started(G2HWATCH handle, int channel, int camera) { }
        public void on_g2watch_audio_streaming_stopped(G2HWATCH handle, int channel, int camera) { }
        public void on_g2watch_audio_capturing_started(G2HWATCH handle, int channel, int camera) { }
        public void on_g2watch_audio_capturing_stopped(G2HWATCH handle, int channel, int camera) { }
        public void on_g2watch_probe_session_profile(G2HWATCH handle, int channel, ref G2PROBE_SESSION_PROFILE probe)
        {
            /*if (_probe_perf)
            {
                string p = probe.ToString();
                this.BeginInvoke((MethodInvoker)delegate() { STC_PROBE_PERF.Text = p; });
            }*/
        }
    }
}
#endif