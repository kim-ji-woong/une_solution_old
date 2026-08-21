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
    public partial class form_violet : g2violet_listener
    {
        #region g2violet_listener

        public void on_g2violet_connected(int handle, int channel)
        {
            _channel = channel;

            on_post_connected();
        }
        public void on_g2violet_disconnected(int handle, int channel, G2DISCONNECT_REASON.TYPE reason)
        {
            _channel = -1;

            on_post_disconnected();
        }
        public void on_g2violet_receive_agent_audio_connected(int handle, int channel, int monitor)
        {
            on_post_agent_audio_connected();
        }
        public void on_g2violet_receive_agent_audio_disconnected(int handle, int channel, int monitor)
        {
            on_post_agent_audio_disconnected();
        }
        public void on_g2violet_receive_agent_layout_attached(int handle, int channel, ref G2GUID agent, int monitor, ref G2GUID layout)
        {
            on_post_agent_layout_attached(monitor, layout);
        }
        public void on_g2violet_receive_cameras_on_monitor(int handle, int channel, ref G2GUID agent, int monitor, G2GUID[] cameras)
        {
            on_post_cameras_on_monitor(monitor, cameras);
        }
        public void on_g2violet_receive_service_log_load(int handle, int channel, ref G2SYSTEM_LOG log) {}
        public void on_g2violet_receive_service_log_load_end(int handle, int channel) {}
        public void on_g2violet_receive_service_log_load_fail(int handle, int channel) {}
        public void on_g2violet_receive_service_log_load_stop(int handle, int channel) {}

        #endregion

        protected void on_post_connected()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate() { on_post_connected(); });
            }

            set_status_msg("Connected");
            enable_controls(true);
            BTN_VIOLET_CONNECT.Text = "Disconnect";
            BTN_VIOLET_CONNECT.Enabled = true;
        }

        protected void on_post_disconnected()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate() { on_post_disconnected(); });
            }

            set_status_msg("Disconnected");
            enable_controls(false);
            BTN_VIOLET_CONNECT.Text = "Connect";
            BTN_VIOLET_CONNECT.Enabled = true;
        }

        protected void on_post_agent_audio_connected()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate() { on_post_agent_audio_connected(); });
            }

            EDT_VIOLET_MONITOR_NUMBER_AUDIO.Enabled = false;
            EDT_VIOLET_DEVICE_ADDRESS.Enabled = false;
            EDT_VIOLET_DEVICE_PORT.Enabled = false;
            EDT_VIOLET_DEVICE_USER_ID.Enabled = false;
            EDT_VIOLET_DEVICE_USER_PW.Enabled = false;
            BTN_VIOLET_AUDIO_CONNECT.Enabled = false;
            BTN_VIOLET_AUDIO_DISCONNECT.Enabled = true;
        }

        protected void on_post_agent_audio_disconnected()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate() { on_post_agent_audio_disconnected(); });
            }

            EDT_VIOLET_MONITOR_NUMBER_AUDIO.Enabled = true;
            EDT_VIOLET_DEVICE_ADDRESS.Enabled = true;
            EDT_VIOLET_DEVICE_PORT.Enabled = true;
            EDT_VIOLET_DEVICE_USER_ID.Enabled = true;
            EDT_VIOLET_DEVICE_USER_PW.Enabled = true;
            BTN_VIOLET_AUDIO_CONNECT.Enabled = true;
            BTN_VIOLET_AUDIO_DISCONNECT.Enabled = false;
        }

        protected void on_post_agent_layout_attached(int monitor, G2GUID layout)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate() { on_post_agent_layout_attached(monitor, layout); });
            }

            EDT_VIOLET_NOTIFICATION.Items.Clear();

            G2LAYOUT device_layout;
            if (g2admin.get().get_layout_info(ref layout, out device_layout))
            {
                EDT_VIOLET_NOTIFICATION.Items.Add(string.Format("Monitor number : {0}, layout : {1}\n", monitor, device_layout._name._string));
            }
        }

        protected void on_post_cameras_on_monitor(int monitor, G2GUID[] cameras)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate() { on_post_cameras_on_monitor(monitor, cameras); });
            }

            if (cameras == null) return;

            EDT_VIOLET_NOTIFICATION.Items.Clear();
            EDT_VIOLET_NOTIFICATION.Items.Add(string.Format("Monitor number : {0}, Camera count : {1}", monitor, cameras.Length));

            foreach (G2GUID g in cameras)
            {
                G2GUID temp = g;
                G2DEVICE_LEAF device;
                if (g2admin.get().get_leaf_device_info(ref temp, out device))
                {
                    EDT_VIOLET_NOTIFICATION.Items.Add(device._name._string);
                }
            }
        }
    }
}
