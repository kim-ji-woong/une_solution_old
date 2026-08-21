using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using GDK;

namespace GDK_tester
{
    public interface adaptor_base
    {
        int connect_ras(ref G2NETWORK_INFO ni, bool port_unity, out G2CONNECT_RES res);
        void disconnect(int channel);
        bool is_connecting(int channel);
        bool is_connected(int channel);
        bool is_disconnecting(int channel);
        bool is_disconnected(int channel);
        bool is_disconnectable(int channel);
        bool is_support(int channel, G2SEARCH_SUPPORT.QUERY query);
        bool is_authority(int channel, G2RAS_AUTHORITY.TYPE authority);
    }

    public interface adaptor_playable : adaptor_base
    {
        bool is_stopped(int channel);
    }
}
