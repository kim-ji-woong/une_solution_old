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
    using G2HANDLE = System.Int32;

    public partial class form_server : g2app_frame_relay_server_listener
    {
        public void init_connective_server()
        {
            _server = new g2app_frame_relay_server();
            _server.set_listener(this);
            _option._connections = 1;
            _option._iocp = true;
            _option._port = 10000;
            _option._realloc = true;
            _option._send_queue_size = 64 * 1024;
            _option._threads = 1;
            _option._tick_out = Int32.MaxValue;
            _server.startup(ref _option);
        }

        public void on_g2app_frame_relay_server_connected(G2HANDLE handle, int channel)
        {

        }
        public void on_g2app_frame_relay_server_disconnected(G2HANDLE handle, int channel, G2DISCONNECT_REASON.TYPE reason)
        {
            _screen.buf_reset(channel);
            _screen.reset(channel, true);
            _screen.set_pane_status(channel, camera_pane.STATUS.NOT_CONNECTED, true);
        }

        public void on_g2app_frame_relay_server_receive_site_connected(G2HANDLE handle, int channel, ref G2STRING_64 site)
        {
            _screen.reset(channel, true);
            _screen.set_pane_mode(channel, camera_pane.MODE.PLAY);
            _screen.set_pane_status(channel, camera_pane.STATUS.ENABLE, true);
            _screen.buf_setup(channel, 1, frame_buf.TYPE.LIVE_SIMPLE);
        }
        public void on_g2app_frame_relay_server_receive_site_disconnected(G2HANDLE handle, int channel, ref G2STRING_64 site, int reason)
        {
            _screen.buf_reset(channel);
            _screen.reset(channel, true);
            _screen.set_pane_status(channel, camera_pane.STATUS.NOT_CONNECTED, true);
        }
        public void on_g2app_frame_relay_server_receive_site_product_info(G2HANDLE handle, int channel, ref G2STRING_64 site, ref G2_PRODUCT_INFO pi) { }
        public void on_g2app_frame_relay_server_receive_site_frame_data(G2HANDLE handle, int channel, ref G2STRING_64 site, ref G2FRAME frame)
        {
            _screen.put_frame(ref frame, channel, channel);
        }
    }
}
