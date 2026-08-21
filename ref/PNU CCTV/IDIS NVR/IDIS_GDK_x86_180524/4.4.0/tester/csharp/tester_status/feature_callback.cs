using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using GDK;

namespace GDK_tester
{
    public partial class form_status : Form
    {
        public void init_callback()
        {
            CALLBACK_EDT_PORT.MaxLength = 5;
            CALLBACK_EDT_PORT.Text = Convert.ToString(8201);
        }

        public void feature_callback_receive_event(ref G2EVENT_INFO ei)
        {
            if (this.InvokeRequired)
            {
                G2EVENT_INFO param = ei;
                this.BeginInvoke((MethodInvoker)delegate() { feature_callback_receive_event(ref param); });
                return;
            }

            string type = ei.string_event_type();
            string data = ei._event_ras._label;
            string time = ei._spot._time.to_string_date_time();

            ListViewItem lvi = new ListViewItem(ei._event_ras._site._site);
            lvi.SubItems.Add(type);
            lvi.SubItems.Add(data);
            lvi.SubItems.Add(time);
            LSV_EVENT.Items.Insert(0, lvi);

            if (LSV_EVENT.Items.Count > 10000)
            {
                LSV_EVENT.Items.RemoveAt(LSV_EVENT.Items.Count - 1);
            }
        }

        private void on_feature_callback_btn_startup(object sender, EventArgs e)
        {
            if (_adaptor.callback_server_is_startup())
            {
                _adaptor.callback_server_shutdown();
            }
            else
            {
                ushort port = 0;
                try
                {
                    port = ushort.Parse(CALLBACK_EDT_PORT.Text);
                }
                catch (Exception) { }

                if (port == 0)
                {
                    MessageBox.Show(this, "Callback Port is not valid.", "Callback Startup", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    CALLBACK_EDT_PORT.Text = "";
                    return;
                }

                if (_adaptor.callback_server_restart(port) != true)
                {
                    MessageBox.Show(this, "Failed to startup callback server.", "Callback", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if (_adaptor.callback_server_is_startup())
            {
                CALLBACK_BTN_STARTUP.Text = "Shutdown";
                CALLBACK_EDT_PORT.Enabled = false;
                LSV_EVENT.ForeColor = SystemColors.WindowText;
            }
            else
            {
                CALLBACK_BTN_STARTUP.Text = "Startup";
                CALLBACK_EDT_PORT.Enabled = true;
                LSV_EVENT.ForeColor = Color.DimGray;
            }
        }
    }
}
