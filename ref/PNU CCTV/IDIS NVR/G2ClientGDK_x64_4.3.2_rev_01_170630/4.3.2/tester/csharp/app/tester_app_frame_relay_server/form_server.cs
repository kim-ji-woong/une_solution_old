using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using GDK;

namespace GDK_tester
{
    public partial class form_server : Form
    {
        public form_server()
        {
            this.SCREEN_PANE = new GDK_tester.screen_pane();
            this.SCREEN_PANE.Name = "SCREEN_PANE";
            this.SCREEN_PANE.TabIndex = 0;
            this.Controls.Add(this.SCREEN_PANE);

            InitializeComponent();

            this.SCREEN_PANE.Location = STC_SCREEN.Location;
            this.SCREEN_PANE.Size = STC_SCREEN.Size;

            init_connective_server();
            init_connective_screen();

            for (int i = 1; i < 32 + 1; ++i)
            {
                DEVICE1_CBX_CHANNEL.Items.Add(i.ToString());
            }

            DEVICE1_CBX_CHANNEL.IntegralHeight = false;
            DEVICE1_CBX_CHANNEL.MaxDropDownItems = 8;
            DEVICE1_EDT_PORT.Text = "8016";
            DEVICE1_EDT_ID.Text = "admin";
            DEVICE1_CHK_UNITY_PORT.Checked = true;
            DEVICE1_CBX_CHANNEL.SelectedIndex = 0;
        }

        private void on_form_closing(object sender, FormClosingEventArgs e)
        {
            _server.cleanup();
            _screen.cleanup();
        }

        private void on_btn_device1_run(object sender, EventArgs e)
        {
            string device_address = DEVICE1_EDT_SERVER.Text.Trim();
            string device_id = DEVICE1_EDT_ID.Text.Trim();
            string device_pass = DEVICE1_EDT_PASSWORD.Text.Trim();
            ushort device_port = 0;
            int camera = -1;

            try
            {
                device_port = Convert.ToUInt16(DEVICE1_EDT_PORT.Text);
                camera = Convert.ToInt32((string)DEVICE1_CBX_CHANNEL.SelectedItem) - 1;
            }
            catch (Exception) { }

            if (device_port == 0 || camera < 0 ||
               (device_address.Length == 0 || device_id.Length == 0))
            {
                MessageBox.Show(this, "device information error", "site", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (_server.is_disconnectable(0))
            {
                _server.disconnect(0);

                while (_server.is_disconnected(0) != true)
                {
                    Thread.Sleep(10);
                }
            }

            G2APP_FRAME_RELAY_SEARCH_CONTROLLER_OPTION option = new G2APP_FRAME_RELAY_SEARCH_CONTROLLER_OPTION();
            option._server_ni._address = "127.0.0.1";
            option._server_ni.set_port(G2NETWORK_INFO.PORT_TYPE.SERVICE_PORT, _option._port);
            option._device_ni._address = device_address;
            option._device_ni._user_id = device_id;
            option._device_ni._password = device_pass;
            option._device_ni.set_port(G2NETWORK_INFO.PORT_TYPE.SEARCH_PORT, device_port);
            option._channels = new g2channel_set(camera);
            option._site = "SITE_NVR_TEST";
            option._title = "G2Search Controller";
            option._no_port_unity = DEVICE1_CHK_UNITY_PORT.Checked != true;
            option._message_duration = 10 * 1000;

            if (g2app_frame_relay_server.search_controller_option_make_file("device1_sample_option.dat", ref option))
            {
                g2app_frame_relay_server.search_controller_run("tester_cpp_app_g2_search_controller.exe", "device1_sample_option.dat");
            }
        }

        private g2app_frame_relay_server _server;
        private screen_pane _screen;
        private G2APP_FRAME_RELAY_SERVER_OPTION _option;
    }
}
