using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using GDK;

namespace GDK_tester
{
    using G2HWATCH = System.Int32;
    public partial class form_watch : g2watch_listener
    {
        public void init_connective_watch()
        {
            _adaptor = new g2watch();
            _adaptor.set_listener(this);
            _adaptor.startup(2, Handle);
            _buf = new frame_buf();
            _channel = -1;
            _count_camera = 0;
        }

        public bool valid_channel(int channel)
        {
            return channel >= 0;
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
                G2_PRODUCT_INFO pi;
                if (_adaptor.get_product_info(channel, out pi))
                {
                    _count_camera = pi.device.count_camera;
                }

                g2channel_set cameras = new g2channel_set(0, _count_camera);
                _adaptor.set_camera_list(channel, cameras, false);
                g2main.options_live_audio_in_load_audio_data(true);
                feature_control_audio_set_enable();
                on_post_watch_connected();

            }
        }
        public void on_post_watch_connected()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { on_post_watch_connected(); });
                return;
            }
            BTN_SAVE.Enabled = true;
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
            _count_camera = 0;
            _channel = -1;
        }
        public void on_g2watch_receive_frame_data(G2HWATCH handle, int channel, ref G2FRAME frame) { }
        public void on_g2watch_receive_audio_data(G2HWATCH handle, int channel, ref G2FRAME frame)
        {
            int pane = frame.channel;
            _buf.on_receive_audio_frame(ref frame, channel, pane);
        }
        public void on_g2watch_receive_event(G2HWATCH handle, int channel, ref G2EVENT_INFO ei) { }
        public void on_g2watch_receive_device_status(G2HWATCH handle, int channel, ref G2DEVICE_STATUS status) { }
        public void on_g2watch_receive_ptz_preset(G2HWATCH handle, int channel, int camera, ref G2LIVE_PTZ_PRESET preset) { }
        public void on_g2watch_receive_ptz_menu(G2HWATCH handle, int channel, int camera, ref G2LIVE_PTZ_MENU menu) { }
        public void on_g2watch_receive_camera_title_idr(G2HWATCH handle, int channel, int camera, string title) { }
        public void on_g2watch_receive_text_in(G2HWATCH handle, int channel, ref G2TEXT_IN data) { }
        public void on_g2watch_receive_network_camera_information(G2HWATCH handle, int channel) { }
        public void on_g2watch_receive_audio_out_not_available(G2HWATCH handle, int channel) { }
        public void on_g2watch_receive_command_result_control_color_status(G2HWATCH handle, int channel, int camera, ref G2LIVE_COMMAND_CONTROL_COLOR control, ref G2LIVE_COMMAND_CONTROL_COLOR_RANGE range) { }
        public void on_g2watch_receive_command_result_control_color(G2HWATCH handle, int channel, int camera, ref G2LIVE_COMMAND_CONTROL_COLOR control, G2LIVE_COMMAND_RESULT.TYPE result) { }
        public void on_g2watch_receive_command_result_control_ptz_status(G2HWATCH handle, int channel, int camera, ref G2LIVE_COMMAND_CONTROL_PTZ control, ref G2LIVE_COMMAND_CONTROL_PTZ_RANGE range) { }
        public void on_g2watch_receive_command_result_control_ptz(G2HWATCH handle, int channel, int camera, G2LIVE_COMMAND_RESULT.TYPE result) { }
        public void on_g2watch_receive_network_alarm_result(G2HWATCH handle, int channel, ref G2LIVE_NETWORK_ALARM_RESULT result) { }
        public void on_g2watch_receive_elevator_status_info_response(G2HWATCH handle, int channel, uint seq_number) { }
        public void on_g2watch_receive_instant_recording_start(G2HWATCH handle, int channel, G2INSTANT_RECORDING_RESULT.TYPE result, G2INSTANT_RECORDING_CHANNEL_STATUS[] status) { }
        public void on_g2watch_receive_instant_recording_stop(G2HWATCH handle, int channel, G2INSTANT_RECORDING_RESULT.TYPE result) { }
        public void on_g2watch_receive_instant_recording_status(G2HWATCH handle, int channel, G2INSTANT_RECORDING_RESULT.TYPE result, G2INSTANT_RECORDING_CHANNEL_STATUS[] status) { }
        public void on_g2watch_audio_streaming_started(G2HWATCH handle, int channel, int camera) { }
        public void on_g2watch_audio_streaming_stopped(G2HWATCH handle, int channel, int camera) { }
        public void on_g2watch_audio_capturing_started(G2HWATCH handle, int channel, int camera) { }
        public void on_g2watch_audio_capturing_stopped(G2HWATCH handle, int channel, int camera) { }
        public void on_g2watch_probe_session_profile(G2HWATCH handle, int channel, ref G2PROBE_SESSION_PROFILE probe) { }

        private g2watch _adaptor;
        private int _channel;
        private int _count_camera;
        private frame_buf _buf;
    }
}