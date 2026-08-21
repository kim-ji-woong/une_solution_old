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
        public void init_record_status()
        {
            RECORD_BTN_RECORDING.Enabled =
           RECORD_BTN_PLAYBACK.Enabled =
           RECORD_BTN_ARCHIVE.Enabled =
           RECORD_BTN_CLIP_EXPORT.Enabled = false;
        }
        public void feature_record_status_set_product_info(ref G2_PRODUCT_INFO pi)
        {

        }
        public void feature_record_status_reset()
        {
            RECORD_BTN_RECORDING.BackColor =
            RECORD_BTN_PLAYBACK.BackColor =
            RECORD_BTN_ARCHIVE.BackColor =
            RECORD_BTN_CLIP_EXPORT.BackColor = SystemColors.Control;
        }
        public void feature_record_status_set(int channel, ref G2MONITORING_DEVICE_STATUS status)
        {
            bool record = status._record.on_record;
            bool play = status._record.on_play;
            bool archive = status._record.on_archive;
            bool clip = status._record.on_clip;

            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate() { on_post_feature_record_status_update(record, play, archive, clip); });
            }
            else
            {
                on_post_feature_record_status_update(record, play, archive, clip);
            }
        }

        protected void on_post_feature_record_status_update(bool record, bool play, bool archive, bool clip)
        {
            RECORD_BTN_RECORDING.BackColor = record ? Color.LightGray : SystemColors.Control;
            RECORD_BTN_PLAYBACK.BackColor = play ? Color.LightGray : SystemColors.Control;
            RECORD_BTN_ARCHIVE.BackColor = archive ? Color.LightGray : SystemColors.Control;
            RECORD_BTN_CLIP_EXPORT.BackColor = clip ? Color.LightGray : SystemColors.Control;
        }
    }
}
