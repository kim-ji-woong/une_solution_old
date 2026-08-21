#if _IDIS_NVR_
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GDK;
using GDK_tester;
using System.Windows.Forms;
using System.Drawing;

namespace UnE.Control.CCTVControl.IDIS_NVR
{
    public partial class IdisNvrSet
    {
        private class ConnectionData
        {
            private string m_strIP = "";
            private ushort m_nPort = 0;
            private int m_nChannel = 0;
            private string m_strID = "";
            private string m_strPW = "";

            public string IP
            {
                get { return m_strIP; }
            }

            public ushort Port
            {
                get { return m_nPort; }
            }

            public int Channel
            {
                get { return m_nChannel; }
            }

            public string ID
            {
                get { return m_strID; }
            }

            public string Password
            {
                get { return m_strPW; }
            }

            public ConnectionData()
            {
            }

            public ConnectionData(string strIP, ushort nPort, int nChannel, string strID, string strPW)
            {
                m_strIP = strIP;
                m_nPort = nPort;
                m_nChannel = nChannel;
                m_strID = strID;
                m_strPW = strPW;
            }

            public bool IsSame(string strIP, ushort nPort, int nChannel, string strID, string strPW)
            {
                if (m_strIP != strIP ||
                    m_nPort != nPort ||
                    m_nChannel != nChannel ||
                    m_strID != strID ||
                    m_strPW != strPW)
                    return false;

                return true;
            }
        }

        private class STRING
        {
            public const int NIS_CONNECTING = 1;
            public static string get(int id)
            {
                if (id == NIS_CONNECTING) return "connecting...";
                return "";
            }
        }

        private GDK_tester.screen_pane _screen = null;

        //private bool _probe_perf;
        private bool _finalize = false;

        private int m_nChannel = -1;
        private ConnectionData m_prevConnection = null;

        private CCTVCtrl m_owner = null;

        public UserControl Control
        {
            get { return _screen; }
        }

        public IdisNvrSet(CCTVCtrl owner)
        {
            m_owner = owner;
        }

        public void InitializeComponent(System.Windows.Forms.Control parent, int x, int y)
        {
            _screen = new GDK_tester.screen_pane();
            _screen.Name = "SCREEN_PANE";
            _screen.TabIndex = 0;

            _screen.Location = new System.Drawing.Point(x, y);
            _screen.Size = parent.Size;
            _screen.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            parent.Controls.Add(_screen);

            init_connective_watch();
            init_connective_screen();
            //init_connective_fen();
            init_control_ptz();
            init_control_color();
            //init_control_alarm_out();

            _screen.set_pane_select(0);
            _screen.set_format2(GDK_tester.screen_format.FORMAT.LAYOUT1X1, true, 0);
        }

        public void OnClosing()
        {
            _finalize = true;

            if (_adaptor.is_disconnectable(_channel))
            {
                _adaptor.disconnect(_channel);

                while (valid_channel(_channel))
                {
                    System.Threading.Thread.Sleep(10);
                }
            }

            _adaptor.cleanup();
            _screen.cleanup();
        }

        public void Disconnect()
        {
            m_prevConnection = null;

            int channel_pre = _channel;
            if (_adaptor.is_disconnectable(channel_pre))
            {
                _adaptor.disconnect(channel_pre);

                while (_adaptor.is_disconnected(channel_pre) != true)
                {
                    System.Threading.Thread.Sleep(10);
                }
            }
        }

        public void Connect(string strIP, ushort nPort, int nChannel, string strID, string strPW)
        {
            if (nPort == 0 || nChannel <= 0 ||
                (strIP.Length == 0 || strID.Length == 0))
            {
                System.Diagnostics.Trace.WriteLine("device information error");
                return;
            }

            _screen.message().hide();
            m_nChannel = nChannel;

            G2NETWORK_INFO ni = new G2NETWORK_INFO();
            ni._address = strIP;
            ni.set_port(G2NETWORK_INFO.PORT_TYPE.WATCH_PORT, nPort);
            ni._user_id = strID;
            ni._password = strPW;

            Disconnect();
            m_prevConnection = new ConnectionData(strIP, nPort, nChannel, strID, strPW);

            if (ni._address_type == G2NETWORK_INFO.ADDRESS_TYPE.DVRNS)
            {
                {
                    if (g2fen.get().is_startup() != true)
                    {
                        g2fen.get().startup(2, 2, 2, 2, "tester_fen_history_watch.dat");
                    }

                    if (g2fen.get().is_startup())
                    {
                        using (GDK_tester.scoped_screen_message message = new GDK_tester.scoped_screen_message(_screen.message(), 0, "FEN preparing...", 0, true))
                        {
                            m_owner.Enabled = false;
                            g2fen.get().set_gateway(ni._extra_server_address, ni.get_port(G2NETWORK_INFO.PORT_TYPE.EXTRA_SERVER_PORT));
                            g2fen.get().join_service((uint)TimeSpan.FromMinutes(2).TotalMilliseconds, true);
                            m_owner.Enabled = true;
                        }
                    }

                    if (g2fen.get().is_activate() != true)
                    {
                        g2main.dvrns_setup(ni._extra_server_address, ni.get_port(G2NETWORK_INFO.PORT_TYPE.EXTRA_SERVER_PORT));
                    }
                }

                if (_finalize) return;
            }

            lock (_adaptor)
            {
                _screen.message().disp(STRING.NIS_CONNECTING, ni._address + " : connecting...", 0, true, 1000);

                G2CONNECT_RES res;

                int channel = _adaptor.connect_ras(ref ni, out res);
                if (valid_channel(channel))
                {
                    _channel = channel;
                    _screen.buf_setup(_channel, 64, GDK_tester.frame_buf.TYPE.LIVE);
                }
                else
                {
                    _channel = -1;

                    if (res._err_dvrns != 0)
                    {
                        string error = g2foundation.get_string_dvrns_error(res._err_dvrns);
                        _screen.message().disp(error, 10 * 1000, false);
                    }
                }
            }
        }

        private Timer m_reconnectTimer = null;

        public void Resize(int nWidth, int nHeight)
        {
            // 사용하지 않음
            /*if (_screen.Size.Width != nWidth && _screen.Size.Height != nHeight)
            {
                if (m_reconnectTimer == null)
                {
                    m_reconnectTimer = new Timer();
                    m_reconnectTimer.Interval = 3000;
                    m_reconnectTimer.Tick += m_reconnectTimer_Tick;
                }

                m_reconnectTimer.Start();
            }*/
        }

        private void Reconnct()
        {
            Point pt = _screen.Location;
            System.Windows.Forms.Control parent = _screen.Parent;

            OnClosing();

            _finalize = false;
            parent.Controls.Remove(_screen);

            _screen = new GDK_tester.screen_pane();
            _screen.Name = "SCREEN_PANE";
            _screen.TabIndex = 0;

            _screen.Location = pt;
            _screen.Size = new Size(parent.Width, parent.Height);
            _screen.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            parent.Controls.Add(_screen);

            init_connective_watch();
            init_connective_screen();
            //init_connective_fen();
            init_control_ptz();
            init_control_color();
            //init_control_alarm_out();

            _screen.set_pane_select(0);
            _screen.set_format2(GDK_tester.screen_format.FORMAT.LAYOUT1X1, true, 0);

            if (m_prevConnection != null)
            {
                Connect(m_prevConnection.IP, m_prevConnection.Port, m_prevConnection.Channel, m_prevConnection.ID, m_prevConnection.Password);
            }
        }

        void m_reconnectTimer_Tick(object sender, EventArgs e)
        {
            m_reconnectTimer.Stop();
            Reconnct();
        }
    }
}
#endif