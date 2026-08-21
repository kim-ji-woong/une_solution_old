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
        public class feature_health
        {
            public enum STATUS
            {
                HEALTHY = 0,
                PROBLEM,
                UNREACHABLE
            }

            public feature_health()
            {
                this.status = "";
                this.problem = "";
                this.mac = "";
                this.version = "";
                this.cameras = "";
                this.cameras_record = "";
                this.sensors = "";
                this.record = "";
                this.record_check = "";
                this.record_period = "";
            }

            public string to_string_record_check() { return bad_camera_record ? cameras_record : record_check; }

            public STATUS cur;
            public string status;
            public string problem;
            public string mac;
            public string version;
            public string cameras;
            public string cameras_record;
            public string sensors;
            public string fans;
            public string record;
            public string record_check;
            public string record_period;
            public bool bad_camera, bad_camera_record, bad_sensor, bad_fan, bad_record;
        }

        public void init_health()
        {
            feature_health_set_enable(false);
        }
        public void feature_health_reset()
        {
            LSV_HEALTH.Items.Clear();
        }
        public void feature_health_set_product_info(ref G2_PRODUCT_INFO pi)
        {

        }
        public void feature_health_set_enable(bool enable)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { feature_health_set_enable(enable); });
                return;
            }
        }
        public void feature_health_set_status(int channel, ref G2MONITORING_DEVICE_STATUS status)
        {
            feature_health element = new feature_health();
            element.cur = imp_health_for_status(ref status, element);

            if (status.is_authority(G2RAS_AUTHORITY.TYPE.AUTHORITY_SYSTEM_CHECK) != true)
            {
                element.status = "No Authority";
                this.BeginInvoke((MethodInvoker)delegate() { on_post_feature_health_update(element); });
                return;
            }

            element.version = status._version;
            element.mac = status._mac.ToString();
            element.status = (element.cur == feature_health.STATUS.HEALTHY) ? "Healthy" :
                             (element.cur == feature_health.STATUS.PROBLEM) ? "Problem" : "Unknown";

            string str;
            if (G2MONITORING_DEVICE_STATUS.is_empty_status(status._health._cameras))
            {
                element.cameras = "";
            }
            else if (imp_health_for_string(status._health._cameras, G2MONITORING_DEVICE_STATUS.CHECK_SYSTEM.ABNORMAL, out str))
            {
                element.cameras = "Video Loss " + str;
            }
            else if (imp_health_for_string(status._health._cameras, G2MONITORING_DEVICE_STATUS.CHECK_SYSTEM.NORMAL, out str))
            {
                element.cameras = str;
            }
            else
            {
                element.cameras = "Not Use";
            }

            if (G2MONITORING_DEVICE_STATUS.is_empty_status(status._health._cameras_record))
            {
                element.cameras_record = "";
            }
            else if (imp_health_for_string(status._health._cameras_record, G2MONITORING_DEVICE_STATUS.CHECK_SYSTEM.ABNORMAL, out str))
            {
                element.cameras_record = "Record Bad " + str;
            }
            else if (imp_health_for_string(status._health._cameras_record, G2MONITORING_DEVICE_STATUS.CHECK_SYSTEM.NORMAL, out str))
            {
                element.cameras_record = str;
            }
            else
            {
                element.cameras_record = "Not Use";
            }

            if (G2MONITORING_DEVICE_STATUS.is_empty_status(status._health._sensors))
            {
                element.sensors = "";
            }
            else if (imp_health_for_string(status._health._sensors, G2MONITORING_DEVICE_STATUS.CHECK_SYSTEM.ABNORMAL, out str))
            {
                element.sensors = "Bad " + str;
            }
            else if (imp_health_for_string(status._health._sensors, G2MONITORING_DEVICE_STATUS.CHECK_SYSTEM.NORMAL, out str))
            {
                element.sensors = str;
            }
            else
            {
                element.sensors = "Not Use";
            }

            if (G2MONITORING_DEVICE_STATUS.is_empty_status(status._health._fans))
            {
                element.fans = "";
            }
            else if (imp_health_for_string(status._health._fans, G2MONITORING_DEVICE_STATUS.CHECK_SYSTEM.ABNORMAL, out str))
            {
                element.fans = "Error " + str;
            }
            else if (imp_health_for_string(status._health._fans, G2MONITORING_DEVICE_STATUS.CHECK_SYSTEM.NORMAL, out str))
            {
                element.fans = str;
            }
            else
            {
                element.fans = "Not Use";
            }

            switch ((G2MONITORING_DEVICE_STATUS.CHECK_SYSTEM)status._health._record)
            {
                case G2MONITORING_DEVICE_STATUS.CHECK_SYSTEM.ABNORMAL:
                    element.record_check = "Bad";
                    break;
                case G2MONITORING_DEVICE_STATUS.CHECK_SYSTEM.NORMAL:
                    element.record_check = "Good";
                    break;
                case G2MONITORING_DEVICE_STATUS.CHECK_SYSTEM.NOT_SUPPORT:
                    element.record_check = "- - - -";
                    break;
                default:
                    element.record_check = "Off";
                    break;
            }

            if (status._record._begin == status._record._end ||
                status._record._begin.valid != true || status._record._end.valid != true)
            {
                element.record_period = "- - - -";
            }
            else
            {
                element.record_period = string.Format("{0}~{1}", status._record._begin.to_string_date_time(), status._record._end.to_string_date_time());
            }

            element.record = (status._record._on._record == G2MONITORING_DEVICE_STATUS.NOT_SUPPORT) ? "- - -" :
                             (status._record.on_record) ? "On" : "Off";

            if (element.cur == feature_health.STATUS.PROBLEM)
            {
                if (element.bad_record)
                {
                    element.problem += "Record Bad";
                }
                if (element.bad_camera)
                {
                    if (element.problem.Length != 0)
                    {
                        element.problem += ',';
                    }
                    element.problem += "Video Loss";
                }
                if (element.bad_sensor)
                {
                    if (element.problem.Length != 0)
                    {
                        element.problem += ',';
                    }
                    element.problem += "Sensor Bad";
                }
                if (element.bad_fan)
                {
                    if (element.problem.Length != 0)
                    {
                        element.problem += ',';
                    }
                    element.problem += "Fan Error";
                }
            }

            this.BeginInvoke((MethodInvoker)delegate() { on_post_feature_health_update(element); });
        }

        private void on_post_feature_health_update(feature_health health)
        {
            string record_check = health.to_string_record_check();

            if (LSV_HEALTH.Items.Count == 0)
            {
                ListViewItem lvi = new ListViewItem(health.status);
                lvi.SubItems.Add(health.problem);
                lvi.SubItems.Add(health.mac);
                lvi.SubItems.Add(health.version);
                lvi.SubItems.Add(health.cameras);
                lvi.SubItems.Add(health.sensors);
                lvi.SubItems.Add(health.fans);
                lvi.SubItems.Add(record_check);
                lvi.SubItems.Add(health.record);
                lvi.SubItems.Add(health.record_period);
                LSV_HEALTH.Items.Add(lvi);

                LSV_HEALTH.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.ColumnContent);
                LSV_HEALTH.AutoResizeColumn(2, ColumnHeaderAutoResizeStyle.ColumnContent);
                LSV_HEALTH.AutoResizeColumn(3, ColumnHeaderAutoResizeStyle.ColumnContent);
                LSV_HEALTH.AutoResizeColumn(6, ColumnHeaderAutoResizeStyle.ColumnContent);
                LSV_HEALTH.AutoResizeColumn(7, ColumnHeaderAutoResizeStyle.ColumnContent);
                LSV_HEALTH.AutoResizeColumn(8, ColumnHeaderAutoResizeStyle.ColumnContent);
            }
            else
            {
                ListViewItem lvi = LSV_HEALTH.Items[0];
                if (lvi.SubItems[0].Text != health.status) lvi.SubItems[0].Text = health.status;
                if (lvi.SubItems[1].Text != health.problem) lvi.SubItems[1].Text = health.problem;
                if (lvi.SubItems[2].Text != health.mac) lvi.SubItems[2].Text = health.mac;
                if (lvi.SubItems[3].Text != health.version) lvi.SubItems[3].Text = health.version;
                if (lvi.SubItems[4].Text != health.cameras) lvi.SubItems[4].Text = health.cameras;
                if (lvi.SubItems[5].Text != health.sensors) lvi.SubItems[5].Text = health.sensors;
                if (lvi.SubItems[6].Text != health.fans) lvi.SubItems[6].Text = health.fans;
                if (lvi.SubItems[7].Text != record_check) lvi.SubItems[7].Text = record_check;
                if (lvi.SubItems[8].Text != health.record) lvi.SubItems[8].Text = health.record;
                if (lvi.SubItems[9].Text != health.record_period) lvi.SubItems[9].Text = health.record_period;
            }

            if (health.cur == feature_health.STATUS.PROBLEM)
            {
                LSV_HEALTH.ForeColor = (health.bad_record || health.bad_camera_record) ? Color.Red : Color.FromArgb(240, 60, 0);
            }
            else
            {
                LSV_HEALTH.ForeColor = Color.DimGray;
            }
        }

        protected feature_health.STATUS imp_health_for_status(ref G2MONITORING_DEVICE_STATUS status, feature_health element)
        {
            feature_health.STATUS s = feature_health.STATUS.HEALTHY;

            if (status.is_authority(G2RAS_AUTHORITY.TYPE.AUTHORITY_SYSTEM_CHECK) != true)
            {
                s = feature_health.STATUS.PROBLEM;
            }
            else
            {
                element.bad_camera = false;
                element.bad_camera_record = false;
                element.bad_sensor = false;
                element.bad_record = false;

                foreach (byte val in status._health._cameras)
                {
                    if (val == (byte)G2MONITORING_DEVICE_STATUS.CHECK_SYSTEM.ABNORMAL)
                    {
                        element.bad_camera = true;
                        break;
                    }
                }
                foreach (byte val in status._health._cameras_record)
                {
                    if (val == (byte)G2MONITORING_DEVICE_STATUS.CHECK_SYSTEM.ABNORMAL)
                    {
                        element.bad_camera_record = true;
                        break;
                    }
                }
                foreach (byte val in status._health._sensors)
                {
                    if (val == (byte)G2MONITORING_DEVICE_STATUS.CHECK_SYSTEM.ABNORMAL)
                    {
                        element.bad_sensor = true;
                        break;
                    }
                }
                foreach (byte val in status._health._fans)
                {
                    if (val == (byte)G2MONITORING_DEVICE_STATUS.CHECK_SYSTEM.ABNORMAL)
                    {
                        element.bad_fan= true;
                        break;
                    }
                }

                element.bad_record = (status._health._record == (byte)G2MONITORING_DEVICE_STATUS.CHECK_SYSTEM.ABNORMAL);

                if (element.bad_camera ||
                    element.bad_sensor ||
                    element.bad_fan ||
                    element.bad_record)
                {
                    s = feature_health.STATUS.PROBLEM;
                }
            }
            return s;
        }

        protected bool imp_health_for_string(byte[] state, G2MONITORING_DEVICE_STATUS.CHECK_SYSTEM std, out string str)
        {
            str = "";
            if (G2MONITORING_DEVICE_STATUS.is_empty_status(state))
            {
                return false;
            }

            int count = 0;
            for (int i = 0; i < state.Length; ++i)
            {
                if (state[i] == (byte)std)
                {
                    if (count == 0)
                    {
                        str += string.Format(str.Length == 0 ? "{0}" : ", {0}", i + 1);
                    }
                    count++;
                }
                else
                {
                    if (count >= 2)
                    {
                        str += string.Format("~{0}", i);
                    }
                    count = 0;
                }
            }

            if (count >= 2 &&
                state[state.Length - 1] == (byte)std)
            {
                str += string.Format("~{0}", state.Length);
            }
            return (str.Length != 0);
        }

        private void on_feature_health_selection_changed(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected) e.Item.Selected = false;
        }
    }
}
