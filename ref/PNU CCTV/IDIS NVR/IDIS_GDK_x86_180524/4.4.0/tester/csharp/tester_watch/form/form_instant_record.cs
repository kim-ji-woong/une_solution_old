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
    public partial class form_instant_record : Form
    {
        public form_instant_record()
        {
            InitializeComponent();

            CBX_ACTION.Items.Add("Start");
            CBX_ACTION.Items.Add("Stop");
            CBX_ACTION.SelectedIndex = 0;
            SETTING_NVR_CBX_PROFILE.Items.Add("Very High");
            SETTING_NVR_CBX_PROFILE.Items.Add("High");
            SETTING_NVR_CBX_PROFILE.Items.Add("Standard");
            SETTING_NVR_CBX_PROFILE.Items.Add("Basic");
            SETTING_NVR_CBX_PROFILE.SelectedIndex = 0;

            SETTING_HANA_CBX_PROFILE.Items.Add("Very High");
            SETTING_HANA_CBX_PROFILE.Items.Add("High");
            SETTING_HANA_CBX_PROFILE.Items.Add("Standard");
            SETTING_HANA_CBX_PROFILE.Items.Add("Basic");
            SETTING_HANA_CBX_PROFILE.SelectedIndex = 0;
            SETTING_HANA_CBX_IPS.Items.Add("30");
            SETTING_HANA_CBX_IPS.Items.Add("15");
            SETTING_HANA_CBX_IPS.Items.Add("10");
            SETTING_HANA_CBX_IPS.Items.Add("5");
            SETTING_HANA_CBX_IPS.Items.Add("4");
            SETTING_HANA_CBX_IPS.Items.Add("3");
            SETTING_HANA_CBX_IPS.Items.Add("2");
            SETTING_HANA_CBX_IPS.Items.Add("1");
            SETTING_HANA_CBX_IPS.SelectedIndex = 0;
            SETTING_HANA_CBX_PROFILE.Items.Add("Basic");
            SETTING_HANA_CBX_PROFILE.SelectedIndex = 0;
            SETTING_HANA_CBX_QUALITY.Items.Add("Low");
            SETTING_HANA_CBX_QUALITY.Items.Add("Normal");
            SETTING_HANA_CBX_QUALITY.Items.Add("High");
            SETTING_HANA_CBX_QUALITY.Items.Add("Very High");
            SETTING_HANA_CBX_QUALITY.SelectedIndex = 3;
            SETTING_HANA_CBX_RESOLUTION.Items.Add("Standard");
            SETTING_HANA_CBX_RESOLUTION.Items.Add("High");
            SETTING_HANA_CBX_RESOLUTION.Items.Add("Very High");
            SETTING_HANA_CBX_RESOLUTION.SelectedIndex = 2;

            CBX_DURATION_PRE.Items.Add("Use Existing Setup");
            CBX_DURATION_PRE.Items.Add("Stop PreEvent");
            CBX_DURATION_PRE.SelectedIndex = 0;
            DTP_DURATION_POST.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 1, 0);
        }

        public void set_product_info(ref G2_PRODUCT_INFO pi)
        {
            imp_build_cameras(pi.device.count_camera);
            imp_build_setting(ref pi);
        }

        protected void imp_build_cameras(int count)
        {
            TRV_CAMERAS.Nodes.Clear();
            TRV_CAMERAS.AfterCheck += new TreeViewEventHandler(on_trv_cameras_after_check);

            if (count > 0) TRV_CAMERAS.Nodes.Add(imp_build_cameras_node("Camera", count));

            foreach (TreeNode node in TRV_CAMERAS.Nodes)
            {
                node.ExpandAll();
            }
        }
        protected TreeNode imp_build_cameras_node(string name, int count)
        {
            TreeNode node = new TreeNode(name);
            for (int i = 0; i < count; ++i)
            {
                node.Nodes.Add(name + ((int)(i + 1)).ToString()).Tag = i;
            }
            return node;
        }
        protected void imp_build_setting(ref G2_PRODUCT_INFO pi)
        {
            if (pi.is_NVR)
            {
                setting_type = G2INSTANT_RECORD_SETTING.SUB_SETTING_TYPE.NVR;
                SETTING_NVR_GRP.Visible = true;
                SETTING_HANA_GRP.Visible = false;
                
            }
            else if (pi.is_series_HANA)
            {
                setting_type = G2INSTANT_RECORD_SETTING.SUB_SETTING_TYPE.HANA;
                SETTING_HANA_GRP.Visible = true;
                SETTING_NVR_GRP.Visible = false;
            }
            else if (pi.basic_info.analog_HD &&
                     pi.basic_info.analog_HD_type == (byte)G2_PRODUCT_INFO.BASIC_INFO.ANALOG_HD_TYPE.ANALOG_HD_TYPE_TVI)
            {
                setting_type = G2INSTANT_RECORD_SETTING.SUB_SETTING_TYPE.TVI;
                SETTING_HANA_GRP.Visible = true;
                SETTING_NVR_GRP.Visible = false;
                SETTING_HANA_STC_PROFILE.Visible = false;
                SETTING_HANA_CBX_PROFILE.Visible = false;
            }
            else
            {
                setting_type = G2INSTANT_RECORD_SETTING.SUB_SETTING_TYPE.UNKNOWN;
                SETTING_NVR_GRP.Visible = false;
            }
        }

        private void on_cbx_action_selected_index_changed(object sender, EventArgs e)
        {
            SETTING_NVR_GRP.Enabled = 
            STC_DURATION_PRE.Enabled =
            CBX_DURATION_PRE.Enabled = 
            STC_DURATION_POST.Enabled =
            DTP_DURATION_POST.Enabled = (CBX_ACTION.SelectedIndex == 0);
        }
        private void on_dtp_duration_post_mouse_wheel(object sender, MouseEventArgs e)
        {
            SendKeys.Send(e.Delta > 0 ? "{UP}" : "{DOWN}");
        }
        private void on_trv_cameras_after_check(object sender, TreeViewEventArgs e)
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
        private void on_btn_ok(object sender, EventArgs e)
        {
            g2channel_set chs = new g2channel_set();
            if (TRV_CAMERAS.Nodes.Count != 0)
            {
                foreach (TreeNode node in TRV_CAMERAS.Nodes[0].Nodes)
                {
                    if (node.Checked)
                    {
                        int ch = (int)node.Tag;
                        chs.insert(ch);
                    }
                }
            }

            if (chs.empty())
            {
                MessageBox.Show(this, "There is no selected device.", "Instant Recording", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.None;
                return;
            }

            cameras = chs;
            action = (ACTION_TYPE)CBX_ACTION.SelectedIndex;

            if (action == ACTION_TYPE.START)
            {
                G2INSTANT_RECORD_SETTING set = new G2INSTANT_RECORD_SETTING();
                set.sub_setting_type = setting_type;
                TimeSpan span = new TimeSpan(DTP_DURATION_POST.Value.Hour, DTP_DURATION_POST.Value.Minute, DTP_DURATION_POST.Value.Second);
                set._duration_post = Math.Max(10 * 1000, (uint)span.TotalMilliseconds);
                set._duration_pre = (CBX_DURATION_PRE.SelectedIndex == 1) ? (long)G2INSTANT_RECORD_SETTING.PRE_DURATION.STOP_PRE_EVENT : (long)G2INSTANT_RECORD_SETTING.PRE_DURATION.USE_EXISTING_SETUP;

                if (setting_type == G2INSTANT_RECORD_SETTING.SUB_SETTING_TYPE.NVR)
                {
                    set._sub_setting._nvr._record_profile = (uint)SETTING_NVR_CBX_PROFILE.SelectedIndex;
                }
                else if (setting_type == G2INSTANT_RECORD_SETTING.SUB_SETTING_TYPE.HANA)
                {
                    set._sub_setting._hana._record_profile = (uint)SETTING_HANA_CBX_PROFILE.SelectedIndex;
                    set._sub_setting._hana._ips = uint.Parse(SETTING_HANA_CBX_IPS.SelectedItem.ToString());
                    set._sub_setting._hana._quality = (byte)SETTING_HANA_CBX_QUALITY.SelectedIndex;
                    set._sub_setting._hana._resolution = (byte)SETTING_HANA_CBX_RESOLUTION.SelectedIndex;
                }
                else if (setting_type == G2INSTANT_RECORD_SETTING.SUB_SETTING_TYPE.TVI)
                {
                    set._sub_setting._tvi._ips = uint.Parse(SETTING_HANA_CBX_IPS.SelectedItem.ToString());
                    set._sub_setting._tvi._quality = (byte)SETTING_HANA_CBX_QUALITY.SelectedIndex;
                    set._sub_setting._tvi._resolution = (byte)SETTING_HANA_CBX_RESOLUTION.SelectedIndex;
                }

                setting = new G2INSTANT_RECORD_SETTING[chs.size()];
                int i = 0;
                foreach (int channel in chs)
                {
                    setting[i] = set;
                    setting[i]._channel = channel;
                    i = i + 1;
                }
            }
        }

        public enum ACTION_TYPE {
            START = 0,
            STOP
        }

        public ACTION_TYPE action;
        G2INSTANT_RECORD_SETTING.SUB_SETTING_TYPE setting_type;
        public g2channel_set cameras = new g2channel_set();
        public G2INSTANT_RECORD_SETTING[] setting;
    }
}
