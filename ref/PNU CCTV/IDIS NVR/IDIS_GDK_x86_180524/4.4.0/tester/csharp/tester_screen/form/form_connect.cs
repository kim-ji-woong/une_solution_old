using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using GDK;

namespace GDK_tester
{
    public partial class form_connect : Form
    {
        public class internal_data
        {
            public G2NETWORK_INFO ni;
            public bool FEN_query;
            public ushort port_check;
            public bool port_check_use;
            public bool port_unity;
            public bool search_G2;
            public bool IDR;
        }

        static form_connect()
        {
            if (serializer == null)
            {
                Type[] types = new Type[] { typeof(internal_data) };
                serializer = XmlSerializer.FromTypes(types)[0];
            }
        }
        public form_connect(G2NETWORK_INFO.PORT_TYPE port_type, ushort port)
        {
            InitializeComponent();

            this.CONNECT_EDT_FEN_SERVER.Text = "fen.idisglobal.com";
            this.CONNECT_EDT_FEN_PORT.Text = Convert.ToString(10088);
            this.CONNECT_CHK_FEN_QUERY.Checked = false;
            this.CONNECT_EDT_ID.Text = "admin";
            this.CONNECT_EDT_PORT.Text = Convert.ToString(port);
            this.internal_inf = new internal_data();
            this.internal_inf.ni.set_port(port_type, port);
            this.internal_inf.port_check_use = true;
            this.internal_inf.port_unity =
            this.internal_inf.search_G2 =
            this.internal_inf.IDR = false;
            this.port_type = port_type;
            this.need_port_check = (port_type == G2NETWORK_INFO.PORT_TYPE.SEARCH_PORT ||
                                    port_type == G2NETWORK_INFO.PORT_TYPE.ADMIN_PORT);
            this.timer = new Timer();
            this.timer.Tick += new EventHandler(on_timer);
            this.timer.Interval = 100;

            this.CONNECT_CHK_CHECK_PORT_USE.Checked = true;
            this.CONNECT_CHK_CHECK_PORT_UNITY.Enabled =
            this.CONNECT_CHK_CHECK_G2_SEARCH.Enabled =
            this.CONNECT_CHK_CHECK_IDR.Enabled = false;
            this.CONNECT_EDT_PORT.MaxLength =
            this.CONNECT_EDT_PORT_REMOTE_SETUP.MaxLength =
            this.CONNECT_EDT_PORT_CHECK.MaxLength =
            this.CONNECT_EDT_FEN_PORT.MaxLength = 5;
            this.CONNECT_EDT_PORT_CHECK.Text = "8016";

            if (this.need_port_check != true)
            {
                this.CONNECT_STC_PORT_CHECK.Enabled =
                this.CONNECT_EDT_PORT_CHECK.Enabled =
                this.CONNECT_CHK_CHECK_PORT_USE.Enabled = false;
            }

            if (port_type != G2NETWORK_INFO.PORT_TYPE.WATCH_PORT)
            {
                this.CONNECT_EDT_PORT_REMOTE_SETUP.Visible =
                this.CONNECT_STC_PORT_REMOTE_SETUP.Visible =
                this.CONNECT_STC_PORT_REMOTE_SETUP_HELP.Visible = false;
            }

            imp_load();
        }

        protected bool imp_load()
        {
            try
            {
                using (StreamReader file = new StreamReader(file_device))
                {
                    internal_data load = (internal_data)serializer.Deserialize(file);

                    if (load.ni._address != null)
                    {
                        CONNECT_EDT_ADDRESS.Text = load.ni._address;
                    }
                    ushort load_port = load.ni.get_port(port_type);
                    if (load_port != 0)
                    {
                        CONNECT_EDT_PORT.Text = load_port.ToString();
                    }

                    load_port = load.ni.get_port(G2NETWORK_INFO.PORT_TYPE.ADMIN_PORT);
                    if (load_port != 0)
                    {
                        CONNECT_EDT_PORT_REMOTE_SETUP.Text = load_port.ToString();
                    }

                    CONNECT_EDT_PORT_CHECK.Text = (load.port_check == 0) ? "" : load.port_check.ToString();
                    CONNECT_CHK_CHECK_PORT_USE.Checked = load.port_check_use;
                    CONNECT_CHK_CHECK_PORT_UNITY.Checked = load.port_unity;
                    CONNECT_CHK_CHECK_G2_SEARCH.Checked = load.search_G2;
                    CONNECT_CHK_CHECK_IDR.Checked = load.IDR;

                    if (need_port_check)
                    {
                        CONNECT_STC_PORT_CHECK.Enabled =
                        CONNECT_EDT_PORT_CHECK.Enabled = load.port_check_use;
                        CONNECT_CHK_CHECK_PORT_UNITY.Enabled =
                        CONNECT_CHK_CHECK_G2_SEARCH.Enabled =
                        CONNECT_CHK_CHECK_IDR.Enabled = (load.port_check_use != true);
                    }

                    if (load.ni._address_type == G2NETWORK_INFO.ADDRESS_TYPE.DVRNS)
                    {
                        CONNECT_CHK_FEN_USE.Checked = true;
                        CONNECT_STC_PORT.Enabled =
                        CONNECT_EDT_PORT.Enabled =
                        CONNECT_EDT_PORT_REMOTE_SETUP.Enabled =
                        CONNECT_STC_PORT_CHECK.Enabled =
                        CONNECT_EDT_PORT_CHECK.Enabled = false;
                        CONNECT_STC_FEN_SERVER.Enabled =
                        CONNECT_EDT_FEN_SERVER.Enabled =
                        CONNECT_STC_FEN_PORT.Enabled =
                        CONNECT_EDT_FEN_PORT.Enabled = 
                        CONNECT_CHK_FEN_QUERY.Enabled = true;
                        CONNECT_STC_ADDRESS.Text = "Name :";

                        if (load.ni._extra_server_address != null)
                        {
                            CONNECT_EDT_FEN_SERVER.Text = load.ni._extra_server_address;
                        }
                        load_port = load.ni.get_port(G2NETWORK_INFO.PORT_TYPE.EXTRA_SERVER_PORT);
                        if (load_port != 0)
                        {
                            CONNECT_EDT_FEN_PORT.Text = load_port.ToString();
                        }

                        CONNECT_CHK_FEN_QUERY.Checked = load.FEN_query;
                    }

                    if (load.ni._user_id != null)
                    {
                        CONNECT_EDT_ID.Text = load.ni._user_id;
                    }
                }
                return true;
            }
            catch (Exception) {}
            return false;
        }
        protected bool imp_save()
        {
            try
            {
                using (StreamWriter file = new StreamWriter(file_device))
                {
                    serializer.Serialize(file, internal_inf);
                }
                return true;
            }
            catch (Exception) { }
            return false;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);

            timer.Dispose();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            Control focus = CONNECT_EDT_ADDRESS;

            if (CONNECT_EDT_ADDRESS.Text.Length > 0)
            {
                focus = (CONNECT_EDT_ID.Text.Length == 0) ? CONNECT_EDT_ID : CONNECT_EDT_PASSWORD;
            }

            focus.Focus();

            timer.Start();
        }

        private void on_load(object sender, EventArgs e)
        {
        }
        private void on_chk_fen_use(object sender, EventArgs e)
        {
            bool use = CONNECT_CHK_FEN_USE.Checked;
            CONNECT_STC_PORT.Enabled =
            CONNECT_EDT_PORT.Enabled =
            CONNECT_STC_PORT_REMOTE_SETUP.Enabled =
            CONNECT_EDT_PORT_REMOTE_SETUP.Enabled = (use != true);
            CONNECT_STC_PORT_CHECK.Enabled =
            CONNECT_EDT_PORT_CHECK.Enabled = need_port_check && (use != true) && CONNECT_CHK_CHECK_PORT_USE.Checked;
            CONNECT_STC_FEN_SERVER.Enabled =
            CONNECT_EDT_FEN_SERVER.Enabled =
            CONNECT_STC_FEN_PORT.Enabled =
            CONNECT_EDT_FEN_PORT.Enabled =
            CONNECT_CHK_FEN_QUERY.Enabled = use;
            CONNECT_STC_ADDRESS.Text = use ? "Name :" : "Address :";
        }
        private void on_chk_check_port_use(object sender, EventArgs e)
        {
            bool use = CONNECT_CHK_CHECK_PORT_USE.Checked;
            CONNECT_STC_PORT_CHECK.Enabled =
            CONNECT_EDT_PORT_CHECK.Enabled = use && (CONNECT_CHK_FEN_USE.Checked != true);
            CONNECT_CHK_CHECK_PORT_UNITY.Enabled =
            CONNECT_CHK_CHECK_G2_SEARCH.Enabled =
            CONNECT_CHK_CHECK_IDR.Enabled = use != true;
        }
        private void on_btn_connect(object sender, EventArgs e)
        {
            internal_inf.ni._command_protocol_type = G2NETWORK_INFO.COMMAND_PROTOCOL_TYPE.IDIS;
            internal_inf.ni._address = CONNECT_EDT_ADDRESS.Text.Trim();
            internal_inf.ni._user_id = CONNECT_EDT_ID.Text.Trim();
            internal_inf.ni._password = CONNECT_EDT_PASSWORD.Text.Trim();
            internal_inf.ni.set_port(port_type, Convert.ToUInt16(CONNECT_EDT_PORT.Text));

            if (port_type == G2NETWORK_INFO.PORT_TYPE.WATCH_PORT)
            {
                if (CONNECT_EDT_PORT_REMOTE_SETUP.Enabled &&
                    CONNECT_STC_PORT_REMOTE_SETUP.Visible)
                {
                    ushort port = CONNECT_EDT_PORT_REMOTE_SETUP.Text.Trim().Length > 0 ? Convert.ToUInt16(CONNECT_EDT_PORT_REMOTE_SETUP.Text) : (ushort)0;
                    internal_inf.ni.set_port(G2NETWORK_INFO.PORT_TYPE.ADMIN_PORT, port);
                }
            }

            try
            {
                if (CONNECT_CHK_CHECK_PORT_USE.Enabled &&
                    CONNECT_CHK_CHECK_PORT_USE.Checked)
                {
                    if (CONNECT_EDT_PORT_CHECK.Text.Trim().Length == 0 ||
                        uint.Parse(CONNECT_EDT_PORT_CHECK.Text) == 0)
                    {
                        MessageBox.Show(this, "check port has not been entered.", "connect", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.DialogResult = DialogResult.None;
                        return;
                    }
                }

                if (CONNECT_CHK_FEN_USE.Checked)
                {
                    internal_inf.ni._address_type = G2NETWORK_INFO.ADDRESS_TYPE.DVRNS;
                    internal_inf.ni._extra_server_address = CONNECT_EDT_FEN_SERVER.Text.Trim();
                    internal_inf.ni.set_port(G2NETWORK_INFO.PORT_TYPE.EXTRA_SERVER_PORT, Convert.ToUInt16(CONNECT_EDT_FEN_PORT.Text));
                    internal_inf.FEN_query = CONNECT_CHK_FEN_QUERY.Checked;
                }
                else
                {
                    internal_inf.ni._address_type = G2NETWORK_INFO.ADDRESS_TYPE.IPV4;
                }

                internal_inf.port_check_use = CONNECT_CHK_CHECK_PORT_USE.Checked;
                internal_inf.port_check = CONNECT_EDT_PORT_CHECK.Text.Length == 0 ? (ushort)0 : Convert.ToUInt16(CONNECT_EDT_PORT_CHECK.Text);
                internal_inf.port_unity = CONNECT_CHK_CHECK_PORT_UNITY.Checked;
                internal_inf.search_G2 = CONNECT_CHK_CHECK_G2_SEARCH.Checked;
                internal_inf.IDR = CONNECT_CHK_CHECK_IDR.Checked;
            }
            catch (Exception)
            {
                MessageBox.Show(this, "site information error.", "connect", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
                return;
            }

            G2NETWORK_INFO ni = internal_inf.ni;
            internal_inf.ni._password = "";
            imp_save();
            internal_inf.ni = ni;

            this.Close();
        }
        private void on_btn_cancel(object sender, EventArgs e)
        {
            this.Close();
        }
        private void on_timer(object sender, EventArgs e)
        {
            bool connectable = true;

            try
            {
                if (CONNECT_CHK_FEN_USE.Checked)
                {
                    if (CONNECT_EDT_ADDRESS.Text.Trim().Length == 0 || CONNECT_EDT_FEN_SERVER.Text.Trim().Length == 0 ||
                       (uint.Parse(CONNECT_EDT_FEN_PORT.Text) == 0))
                    {
                        connectable = false;
                    }
                }
                else
                {
                    if (CONNECT_EDT_ADDRESS.Text.Trim().Length == 0 ||
                        uint.Parse(CONNECT_EDT_PORT.Text) == 0)
                    {
                        connectable = false;
                    }
                }
            }
            catch (Exception)
            {
                connectable = false;
            }

            CONNECT_BTN_CONNECT.Enabled = connectable;
        }

        public internal_data internal_inf;
        public G2NETWORK_INFO.PORT_TYPE port_type;
        public Timer timer;
        public bool need_port_check;
        public static string file_device = "tester_ras_device_v1_0_0.xml";
        public static XmlSerializer serializer = null;
    }
}
