using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using GDK;

namespace GDK_tester
{
    public class adaptor_g1 : g2search, adaptor_playable
    {
        public int connect_ras(ref G2NETWORK_INFO ni, bool port_unity, out G2CONNECT_RES res)
        {
            return base.connect_ras(ref ni, out res);
        }
        public new void disconnect(int channel)
        {
            base.disconnect(channel);
        }
        new public bool is_connecting(int channel) { return base.is_connecting(channel); }
        new public bool is_connected(int channel) { return base.is_connected(channel); }
        new public bool is_disconnecting(int channel) { return base.is_disconnecting(channel); }
        new public bool is_disconnected(int channel) { return base.is_disconnected(channel); }
        new public bool is_disconnectable(int channel) { return base.is_disconnectable(channel); }
        new public bool is_stopped(int channel) { return base.is_stopped(channel); }
    }

    public class adaptor_g2 : g2search_g2, adaptor_playable
    {
        new public int connect_ras(ref G2NETWORK_INFO ni, bool port_unity, out G2CONNECT_RES res)
        {
            return base.connect_ras(ref ni, port_unity, out res);
        }
        public new void disconnect(int channel)
        {
            base.disconnect(channel);
        }

        new public bool is_connecting(int channel) { return base.is_connecting(channel); }
        new public bool is_connected(int channel) { return base.is_connected(channel); }
        new public bool is_disconnecting(int channel) { return base.is_disconnecting(channel); }
        new public bool is_disconnected(int channel) { return base.is_disconnected(channel); }
        new public bool is_disconnectable(int channel) { return base.is_disconnectable(channel); }
        new public bool is_stopped(int channel) { return base.is_stopped(channel); }
    }
}
