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
        public form_event_search(List<G2DEVICE_LEAF> camera_infos)
            : this(camera_infos, true) {}

        public form_event_search(List<G2DEVICE_LEAF> camera_infos, bool is_service_site)
        {
            InitializeComponent();

            _camera_infos = camera_infos;
            _option = new G2SERVICE_SEARCH_OPTION_EVENT_LOG();
            _is_service_site = is_service_site;

            if (_is_service_site)
            {
                PANEL_EVENT_SERVICE_SITE.Visible = true;
                this.Height = 496;
                BTN_OK.Location = new Point(246, 437);
                BTN_CANCEL.Location = new Point(327, 437);
            }
            else
            {
                PANEL_EVENT_DVR_SITE.Enabled = true;
                PANEL_EVENT_DVR_SITE.Visible = true;
            }
        }

        public void set_time_range(G2TIME from, G2TIME to)
        {
            _range_from = from;
            _range_to = to;
        }

        public void load()
        {
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

            imp_set_device();
        }

        protected bool imp_save()
        {
            G2SCOPE scope;
            if (imp_get_time_range(out scope) != true)
            {
                MessageBox.Show(this, "Invalid recording range", "Event Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            List<G2GUID> checked_camera = new List<G2GUID>();

            foreach (TreeNode camera in TRV_DEVICES.Nodes)
            {
                if (camera.Checked)
                {
                    checked_camera.Add((G2GUID)camera.Tag);
                }
            }

            bool selected_camera_event = false;

            if (_is_service_site)
            {
                if (CB_ALARM_IN.Checked) _option._event.Add((int)G2EVENT_INFO.TYPE_LEVEL1.L1_DEVICE | (int)G2EVENT_INFO.TYPE_LEVEL2.L2_ALARMIN_ON);
                if (CB_USER_ALARAM_IN.Checked) _option._event.Add((int)G2EVENT_INFO.TYPE_LEVEL1.L1_DEVICE | (int)G2EVENT_INFO.TYPE_LEVEL2.L2_USER_DEFINED_ALARMIN_ON);
                if (CB_MOTION_DETECTION.Checked) _option._event.Add((int)G2EVENT_INFO.TYPE_LEVEL1.L1_DEVICE | (int)G2EVENT_INFO.TYPE_LEVEL2.L2_CAMERA_MOTION_DETECTION_ON);
                if (CB_VIDEO_LOSS.Checked) _option._event.Add((int)G2EVENT_INFO.TYPE_LEVEL1.L1_DEVICE | (int)G2EVENT_INFO.TYPE_LEVEL2.L2_CAMERA_VIDEO_LOSS_ON);
                if (CB_VIDEO_BLIND.Checked) _option._event.Add((int)G2EVENT_INFO.TYPE_LEVEL1.L1_DEVICE | (int)G2EVENT_INFO.TYPE_LEVEL2.L2_CAMERA_VIDEO_BLIND_ON);
                if (CB_TRIPZONE.Checked) _option._event.Add((int)G2EVENT_INFO.TYPE_LEVEL1.L1_DEVICE | (int)G2EVENT_INFO.TYPE_LEVEL2.L2_CAMERA_TRIPZONE_ON);
                if (CB_TAMPERING.Checked) _option._event.Add((int)G2EVENT_INFO.TYPE_LEVEL1.L1_DEVICE | (int)G2EVENT_INFO.TYPE_LEVEL2.L2_CAMERA_TAMPER_ON);
                if (CB_VIDEO_ANALYTICS.Checked) _option._event.Add((int)G2EVENT_INFO.TYPE_LEVEL1.L1_DEVICE | (int)G2EVENT_INFO.TYPE_LEVEL2.L2_CAMERA_VIDEO_ANALYTICS_ON);
                if (CB_TEXT_IN.Checked) _option._event.Add((int)G2EVENT_INFO.TYPE_LEVEL1.L1_DEVICE | (int)G2EVENT_INFO.TYPE_LEVEL2.L2_TEXT_IN_ON);
                if (CB_AUDIO_DETECTION.Checked) _option._event.Add((int)G2EVENT_INFO.TYPE_LEVEL1.L1_DEVICE | (int)G2EVENT_INFO.TYPE_LEVEL2.L2_AUDIO_ON);
                if (CB_DEVICE_CONNECTION.Checked) _option._event.Add((int)G2EVENT_INFO.TYPE_LEVEL1.L1_DEVICE | (int)G2EVENT_INFO.TYPE_LEVEL2.L2_DEVICE_CONNECTED);
                if (CB_DEVICE_DISCONNECTION.Checked) _option._event.Add((int)G2EVENT_INFO.TYPE_LEVEL1.L1_DEVICE | (int)G2EVENT_INFO.TYPE_LEVEL2.L2_DEVICE_DISCONNECTED);
                if (CB_FACE_DETECTION.Checked) _option._event.Add((int)G2EVENT_INFO.TYPE_LEVEL1.L1_DEVICE | (int)G2EVENT_INFO.TYPE_LEVEL2.L2_CAMERA_FACE_DETECTION_ON);
            }
            else
            {
                if (CB_ALARM_IN_DVR.Checked) _option._event.Add((int)G2EVENT_INFO.TYPE_LEVEL1.L1_DEVICE | (int)G2EVENT_INFO.TYPE_LEVEL2.L2_ALARMIN_ON);
                if (CB_NETWORK_ALARM_IN_DVR.Checked) _option._event.Add((int)G2EVENT_INFO.TYPE_LEVEL1.L1_DEVICE | (int)G2EVENT_INFO.TYPE_LEVEL2.L2_NETWORK_ALARMIN_ON);
                if (CB_MOTION_DETECTION_DVR.Checked) _option._event.Add((int)G2EVENT_INFO.TYPE_LEVEL1.L1_DEVICE | (int)G2EVENT_INFO.TYPE_LEVEL2.L2_CAMERA_MOTION_DETECTION_ON);
                if (CB_VIDEO_LOSS_DVR.Checked) _option._event.Add((int)G2EVENT_INFO.TYPE_LEVEL1.L1_DEVICE | (int)G2EVENT_INFO.TYPE_LEVEL2.L2_CAMERA_VIDEO_LOSS_ON);
                if (CB_TRIPZONE_DVR.Checked) _option._event.Add((int)G2EVENT_INFO.TYPE_LEVEL1.L1_DEVICE | (int)G2EVENT_INFO.TYPE_LEVEL2.L2_CAMERA_TRIPZONE_ON);
                if (CB_TAMPERING_DVR.Checked) _option._event.Add((int)G2EVENT_INFO.TYPE_LEVEL1.L1_DEVICE | (int)G2EVENT_INFO.TYPE_LEVEL2.L2_CAMERA_TAMPER_ON);
                if (CB_FACE_DETECTION_DVR.Checked) _option._event.Add((int)G2EVENT_INFO.TYPE_LEVEL1.L1_DEVICE | (int)G2EVENT_INFO.TYPE_LEVEL2.L2_CAMERA_FACE_DETECTION_ON);
                if (CB_RECORDING_FAIL_DVR.Checked) _option._event.Add((int)G2EVENT_INFO.TYPE_LEVEL1.L1_DEVICE | (int)G2EVENT_INFO.TYPE_LEVEL2.L2_CAMERA_RECORD_BAD_ON);
                if (CB_FAN_ERROR.Checked) _option._event.Add((int)G2EVENT_INFO.TYPE_LEVEL1.L1_DEVICE | (int)G2EVENT_INFO.TYPE_LEVEL2.L2_CAMERA_FAN_ERROR_ON);
                if (CB_TEXT_IN_DVR.Checked) _option._event.Add((int)G2EVENT_INFO.TYPE_LEVEL1.L1_DEVICE | (int)G2EVENT_INFO.TYPE_LEVEL2.L2_TEXT_IN_ON);
                if (CB_AUDIO_DETECTION_DVR.Checked) _option._event.Add((int)G2EVENT_INFO.TYPE_LEVEL1.L1_DEVICE | (int)G2EVENT_INFO.TYPE_LEVEL2.L2_AUDIO_ON);

                if (CB_RECORD_BAD.Checked) _option._event.Add((int)G2EVENT_INFO.TYPE_LEVEL1.L1_DEVICE | (int)G2EVENT_INFO.TYPE_LEVEL2.L2_DVR_RECORDER_BAD_ON);
                if (CB_ALARM_IN_BAD.Checked) _option._event.Add((int)G2EVENT_INFO.TYPE_LEVEL1.L1_DEVICE | (int)G2EVENT_INFO.TYPE_LEVEL2.L2_ALARMIN_BAD);
                if (CB_DISK_BAD.Checked) _option._event.Add((int)G2EVENT_INFO.TYPE_LEVEL1.L1_DEVICE | (int)G2EVENT_INFO.TYPE_LEVEL2.L2_DVR_DISK_BAD_ON);
                if (CB_DISK_TEMPERATURE.Checked) _option._event.Add((int)G2EVENT_INFO.TYPE_LEVEL1.L1_DEVICE | (int)G2EVENT_INFO.TYPE_LEVEL2.L2_DVR_DISK_TEMPERATURE_ON);
                if (CB_DISK_SMART.Checked) _option._event.Add((int)G2EVENT_INFO.TYPE_LEVEL1.L1_DEVICE | (int)G2EVENT_INFO.TYPE_LEVEL2.L2_DVR_DISK_TEMPERATURE_ON);
                if (CB_DISK_ALMOST_FULL.Checked) _option._event.Add((int)G2EVENT_INFO.TYPE_LEVEL1.L1_DEVICE | (int)G2EVENT_INFO.TYPE_LEVEL2.L2_DVR_TANGO_ALMOST_FULL_ON);
                if (CB_DISK_DELETED.Checked) _option._event.Add((int)G2EVENT_INFO.TYPE_LEVEL1.L1_DEVICE | (int)G2EVENT_INFO.TYPE_LEVEL2.L2_DVR_DISK_OFF);
                if (CB_DISK_CONFIG_CHANGE.Checked) _option._event.Add((int)G2EVENT_INFO.TYPE_LEVEL1.L1_DEVICE | (int)G2EVENT_INFO.TYPE_LEVEL2.L2_DVR_DISK_CONFIG_CHANGE);
                if (CB_PANIC_RECORD.Checked) _option._event.Add((int)G2EVENT_INFO.TYPE_LEVEL1.L1_DEVICE | (int)G2EVENT_INFO.TYPE_LEVEL2.L2_DVR_PANIC_RECORD_ON);
                if (CB_FAN_ERROR_DVR.Checked) _option._event.Add((int)G2EVENT_INFO.TYPE_LEVEL1.L1_DEVICE | (int)G2EVENT_INFO.TYPE_LEVEL2.L2_DVR_FAN_ERROR_ON);
                if (CB_CONSECUTIVE_INVALID_PASSWORD.Checked) _option._event.Add((int)G2EVENT_INFO.TYPE_LEVEL1.L1_DEVICE | (int)G2EVENT_INFO.TYPE_LEVEL2.L2_DVR_LOGIN_FAILED_SEVERAL_TIMES);
            }
            

            if (_option._event.Count > 0) selected_camera_event = true;

            if (selected_camera_event)
            {
                if (checked_camera.Count == 0)
                {
                    MessageBox.Show(this, "Please select camera device for searching event.", "Event Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
            }

            if (selected_camera_event == false)
            {
                MessageBox.Show(this, "Please select event search items.", "Event Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            _option._scope = scope;
            _option._source.insert(checked_camera.ToArray());
            _option._request_count = 100;
            _option._type = G2SERVICE_SEARCH_OPTION_EVENT_LOG.TYPE.RECORDING;
            _option._direction = G2SERVICE_SEARCH_OPTION_EVENT_LOG.DIRECTION.BACKWARD;

            return true;
        }

        protected void imp_set_device()
        {
            TRV_DEVICES.Nodes.Clear();
            TRV_DEVICES.AfterCheck += new TreeViewEventHandler(on_tree_device_after_check);

            for (int i = 0; i < _camera_infos.Count; ++i)
            {
                TreeNode node = new TreeNode(_camera_infos[i]._name);
                node.Tag = _camera_infos[i]._guid;
                TRV_DEVICES.Nodes.Add(node);
            }

            TRV_DEVICES.ExpandAll();
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
        
        private void on_btn_OK(object sender, EventArgs e)
        {
            if (imp_save())
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
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

        private G2TIME _range_from;
        private G2TIME _range_to;
        public G2SERVICE_SEARCH_OPTION_EVENT_LOG _option;
        private List<G2DEVICE_LEAF> _camera_infos;
        private bool _is_service_site;
    }
}
