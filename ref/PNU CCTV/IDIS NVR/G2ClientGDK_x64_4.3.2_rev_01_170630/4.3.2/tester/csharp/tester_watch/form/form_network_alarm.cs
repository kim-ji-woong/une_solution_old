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
    public partial class form_network_alarm : Form
    {
        public form_network_alarm(g2watch adaptor, int channel)
        {
            InitializeComponent();

            this._adaptor = adaptor;
            this._channel = channel;

            G2_PRODUCT_INFO pi;
            int count = 16;
            if (adaptor.get_product_info(channel, out pi))
            {
                count = Math.Max(count, pi.device.count_alarm_in_network);
            }

            for (int i = 0; i < count; ++i)
            {
                CBX_ID.Items.Add((i + 1).ToString());
            }

            CBX_ID.SelectedIndex = 0;
            CBX_LEVEL.Items.Add("Normal");
            CBX_LEVEL.Items.Add("Emergency");
            CBX_LEVEL.Items.Add("Error");
            CBX_LEVEL.SelectedIndex = 0;
            EDT_DATA.Text = "network alarm";
            EDT_MSEC.Text = Environment.TickCount.ToString();
        }

        private void on_btn_control(object sender, EventArgs e)
        {
            G2LIVE_NETWORK_ALARM_INFO nai = new G2LIVE_NETWORK_ALARM_INFO();

            string s;
            s = EDT_TYPE.Text.Trim();
            nai._event = s.Length == 0 ? 0 : int.Parse(s);
            nai._id = (uint)CBX_ID.SelectedIndex;
            nai._level = CBX_LEVEL.SelectedIndex;
            nai._time = DTP_TIME.Value;
            s = EDT_MSEC.Text.Trim();
            nai._msec = s.Length == 0 ? 0 : uint.Parse(s);
            nai._data = EDT_DATA.Text.Trim();
            nai._on = (sender == BTN_ON);

            if (_adaptor.is_connected(_channel))
            {
                _adaptor.send_network_alarm_info(_channel, ref nai);
            }
            else
            {
                this.Close();
            }
        }

        private g2watch _adaptor;
        private int _channel;
    }
}
