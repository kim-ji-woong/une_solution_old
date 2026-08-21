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
        public class feature_instant_record
        {
            public static string get_string_duration(ref G2INSTANT_RECORDING_CHANNEL_STATUS status)
            {
                string s = TimeSpan.FromMilliseconds((double)status._duration_post).ToString();

                if (status._duration_pre == (long)G2INSTANT_RECORD_SETTING.PRE_DURATION.USE_EXISTING_SETUP)
                {
                    s += " / Use Setup";
                }
                else
                {
                    s += " / ";
                    s += TimeSpan.FromMilliseconds((double)status._duration_pre).ToString();
                }
                return s;
            }
            public static string get_string_recording(ref G2INSTANT_RECORDING_CHANNEL_STATUS status)
            {
                return string.Format("{0} / {1}", status._on_recording ? "On" : "Off", status._on_recording_audio ? "On" : "Off");
            }
            public static string get_string_scope(ref G2INSTANT_RECORDING_CHANNEL_STATUS status)
            {
                string s;
                if (!status._scope._begin._time.valid ||
                    !status._scope._end._time.valid)
                {
                    s = "- - - -";
                }
                else
                {
                    s = string.Format("{0}~{1}", status._scope._begin._time.to_string_date_time(), status._scope._end._time.to_string_date_time()); 
                }
                return s;
            }
            public static string get_string_fail_reason(ref G2INSTANT_RECORDING_CHANNEL_STATUS status)
            {
                return (status.fail_reason != G2INSTANT_RECORDING_CHANNEL_STATUS.FAIL_REASON.SUCCESS) ? status.fail_reason.ToString() : "";
            }
        }

        public void init_instant_record()
        {
            CHK_INSTANT_RECORD_STATUS.Enabled =
            LSV_INSTANT_RECORD.Enabled = false;
        }
        public void feature_instant_record_reset()
        {
            LSV_INSTANT_RECORD.Items.Clear();
        }
        public void feature_instant_record_set_product_info(ref G2_PRODUCT_INFO pi)
        {

        }
        public void feature_instant_record_set_enable(bool enable)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { feature_instant_record_set_enable(enable); });
                return;
            }

            CHK_INSTANT_RECORD_STATUS.Enabled =
            LSV_INSTANT_RECORD.Enabled = enable;

            if (enable != true)
            {
                if (LSV_LOG_EVENT.Visible != true && LSV_LOG_SYSTEM.Visible != true)
                {
                    LSV_LOG_SYSTEM.Visible = true;
                }

                LSV_INSTANT_RECORD.Visible = false;
            }
        }

        public void feature_instant_record_set_status(int channel, G2INSTANT_RECORDING_CHANNEL_STATUS[] status)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { feature_instant_record_set_status(channel, status); });
                return;
            }

            for (int i = 0; i < status.Length; ++i) {
                G2INSTANT_RECORDING_CHANNEL_STATUS var = status[i];
                if (i >= LSV_INSTANT_RECORD.Items.Count)
                {
                    ListViewItem lvi = new ListViewItem(var._channel.ToString());
                    lvi.SubItems.Add(feature_instant_record.get_string_duration(ref var));
                    lvi.SubItems.Add(feature_instant_record.get_string_recording(ref var));
                    lvi.SubItems.Add(feature_instant_record.get_string_scope(ref var));
                    lvi.SubItems.Add(feature_instant_record.get_string_fail_reason(ref var));
                    lvi.Tag = var;
                    LSV_INSTANT_RECORD.Items.Add(lvi);
                }
                else
                {
                    ListViewItem item = LSV_INSTANT_RECORD.Items[i];
                    G2INSTANT_RECORDING_CHANNEL_STATUS pre = (G2INSTANT_RECORDING_CHANNEL_STATUS)item.Tag;
                    item.Tag = var;
                    if (pre._channel != var._channel) item.SubItems[0].Text = var._channel.ToString();
                    if (pre._duration_post != var._duration_post || pre._duration_pre != var._duration_pre) item.SubItems[1].Text = feature_instant_record.get_string_duration(ref var);
                    if (pre._on_recording != var._on_recording || pre._on_recording_audio != var._on_recording_audio) item.SubItems[2].Text = feature_instant_record.get_string_recording(ref var);
                    if (pre._scope != var._scope) item.SubItems[3].Text = feature_instant_record.get_string_scope(ref var);
                    if (pre._fail_reason != var._fail_reason) item.SubItems[4].Text = feature_instant_record.get_string_fail_reason(ref var);
                }
            }

            if (LSV_INSTANT_RECORD.Items.Count > status.Length)
            {
                for (int i = LSV_INSTANT_RECORD.Items.Count - status.Length; i >= 0; --i)
                {
                    LSV_INSTANT_RECORD.Items.RemoveAt(LSV_INSTANT_RECORD.Items.Count - 1);
                }
            }
        }

        private void on_chk_instant_record_status_checked_changed(object sender, EventArgs e)
        {
            if (CHK_INSTANT_RECORD_STATUS.Checked)
            {
                _timer_status_instant_record.Start();

                LSV_INSTANT_RECORD.Visible = true;
                LSV_LOG_EVENT.Visible = false;
                LSV_LOG_SYSTEM.Visible = false;
                feature_instant_record_reset();
            }
            else
            {
                _timer_status_instant_record.Stop();

                if (_feature_log.options._type == G2STATUS_LOG_SEARCH_OPTIONS.TYPE.EVENT)
                {
                    LSV_LOG_EVENT.Visible = true;
                    LSV_LOG_SYSTEM.Visible = false;
                }
                else
                {
                    LSV_LOG_SYSTEM.Visible = true;
                    LSV_LOG_EVENT.Visible = false;
                }
                LSV_INSTANT_RECORD.Visible = false;
                feature_instant_record_reset();
            }
        }
        private void on_feature_instant_record_selection_changed(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected) e.Item.Selected = false;
        }
    }
}
