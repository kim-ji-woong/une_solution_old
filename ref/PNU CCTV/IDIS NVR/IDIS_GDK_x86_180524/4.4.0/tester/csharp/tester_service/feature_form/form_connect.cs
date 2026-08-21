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
    public partial class form_connect_admin : Form
    {
        public bool FEN_query;
        public G2NETWORK_INFO ni;
        public Timer timer;

        public form_connect_admin()
        {
            InitializeComponent();

            //
            this.CONNECT_EDT_ADDRESS.Text = "127.0.0.1"; this.CONNECT_EDT_PASSWORD.Text = "12345678";
            //
            this.CONNECT_EDT_FEN_SERVER.Text = "fen.idisglobal.com";
            this.CONNECT_EDT_FEN_PORT.Text = Convert.ToString(10088);
            this.CONNECT_CHK_FEN_QUERY.Checked = false;
            this.CONNECT_EDT_ID.Text = "admin";
            this.CONNECT_EDT_PORT.Text = Convert.ToString(11001);

            CB_MODE.Items.Add("IP Address");
            CB_MODE.Items.Add("FEN");
            CB_MODE.SelectedIndex = 0;

            this.timer = new Timer();
            this.timer.Tick += new EventHandler(on_timer);
            this.timer.Interval = 100;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            timer.Start();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);

            timer.Dispose();
        }

        private void selected_connect_mode(object sender, EventArgs e)
        {
            int index = ((ComboBox)sender).SelectedIndex;
            if (index == 0)
            {
                GRP_IP.Visible = true;
                GRP_FEN.Visible = false;
            }
            else if (index == 1)
            {
                GRP_IP.Visible = false;
                GRP_FEN.Visible = true;
            }
        }

        private void on_btn_connect(object sender, EventArgs e)
        {
            try
            {
                ni._command_protocol_type = G2NETWORK_INFO.COMMAND_PROTOCOL_TYPE.IDIS;
                ni._user_id = CONNECT_EDT_ID.Text.Trim();
                ni._password = CONNECT_EDT_PASSWORD.Text.Trim();

                if (CB_MODE.SelectedIndex == 0)
                {
                    ni._address = CONNECT_EDT_ADDRESS.Text.Trim();
                    ni.set_port(G2NETWORK_INFO.PORT_TYPE.SERVICE_PORT, Convert.ToUInt16(CONNECT_EDT_PORT.Text));
                    ni._address_type = G2NETWORK_INFO.ADDRESS_TYPE.IPV4;
                }
                else if (CB_MODE.SelectedIndex == 1)
                {
                    ni._address = CONNECT_EDT_FEN_NAME.Text.Trim();
                    ni.set_port(G2NETWORK_INFO.PORT_TYPE.EXTRA_SERVER_PORT, Convert.ToUInt16(CONNECT_EDT_FEN_PORT.Text));
                    ni._address_type = G2NETWORK_INFO.ADDRESS_TYPE.DVRNS;
                    ni._extra_server_address = CONNECT_EDT_FEN_SERVER.Text.Trim();
                    FEN_query = CONNECT_CHK_FEN_QUERY.Checked;
                }
            }
            catch (Exception)
            {
                MessageBox.Show(this, "site information error.", "connect", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
                return;
            }
        }

        private void on_timer(object sender, EventArgs e)
        {
            bool connectable = true;

            try
            {
                if (CB_MODE.SelectedIndex == 0)
                {
                    if (CONNECT_EDT_ADDRESS.Text.Trim().Length == 0 ||
                        uint.Parse(CONNECT_EDT_PORT.Text) == 0)
                    {
                        connectable = false;
                    }
                }
                else if (CB_MODE.SelectedIndex == 1)
                {
                    if (CONNECT_EDT_FEN_NAME.Text.Trim().Length == 0 || CONNECT_EDT_FEN_SERVER.Text.Trim().Length == 0 ||
                       (uint.Parse(CONNECT_EDT_FEN_PORT.Text) == 0))
                    {
                        connectable = false;
                    }
                }

                if (CONNECT_EDT_ID.Text.Trim().Length == 0)
                {
                    connectable = false;
                }
            }
            catch (Exception)
            {
                connectable = false;
            }

            CONNECT_BTN_CONNECT.Enabled = connectable;
        }
    }
}