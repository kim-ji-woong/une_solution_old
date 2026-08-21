using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using GDK;

namespace GDK_tester
{
    public partial class form_watch
    {
        public void on_post_watch_receive_instant_recording_start(int channel, G2INSTANT_RECORDING_RESULT.TYPE result, G2INSTANT_RECORDING_CHANNEL_STATUS[] status)
        {
            string info = string.Format("instant recording start: {0}", result.ToString());
            _screen.message().disp(info, 5 * 1000, false);
        }
        public void on_post_watch_receive_instant_recording_stop(int channel, G2INSTANT_RECORDING_RESULT.TYPE result)
        {
            string info = string.Format("instant recording stop: {0}", result.ToString());
            _screen.message().disp(info, 5 * 1000, false);
        }
        public void on_post_watch_receive_instant_recording_status(int channel, G2INSTANT_RECORDING_RESULT.TYPE result, G2INSTANT_RECORDING_CHANNEL_STATUS[] status)
        {

        }

        private void on_btn_instant_recording(object sender, EventArgs e)
        {
            form_instant_record form = new form_instant_record();
            G2_PRODUCT_INFO pi;
            int channel = _channel;
            if (_adaptor.is_connected(channel) != true) return;
            if (_adaptor.get_product_info(channel, out pi))
            {
                form.set_product_info(ref pi);
            }
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                if (form.action == form_instant_record.ACTION_TYPE.START)
                {
                    _adaptor.request_instant_recording_start(channel, form.setting);
                }
                else
                {
                    _adaptor.request_instant_recording_stop(channel, form.cameras);
                }
            }
        }
    }
}
