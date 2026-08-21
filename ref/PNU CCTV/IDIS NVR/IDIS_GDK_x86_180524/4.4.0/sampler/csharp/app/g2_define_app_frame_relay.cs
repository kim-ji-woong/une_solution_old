using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDK
{
    public struct G2APP_FRAME_RELAY_CALLBACK
    {
        public enum TYPE
        {
            on_connected = 0,
            on_disconnected,
            on_receive_request_site_product_info,

            CALLBACK_COUNT
        }
    }

    public struct G2APP_FRAME_RELAY_SERVER_CALLBACK
    {
        public enum TYPE
        {
            on_connected = 0,
            on_disconnected,
            on_receive_site_connected,
            on_receive_site_disconnected,
            on_receive_site_product_info,
            on_receive_site_frame_data,

            CALLBACK_COUNT
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2APP_FRAME_RELAY_SERVER_OPTION
    {
        public int _connections;
        public int _threads;
        public int _send_queue_size;
        public uint _tick_out;
        public ushort _port;
        [MarshalAs(UnmanagedType.U1)]
        public bool _iocp;
        [MarshalAs(UnmanagedType.U1)]
        public bool _realloc;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2APP_FRAME_RELAY_SEARCH_CONTROLLER_OPTION
    {
        public G2NETWORK_INFO _server_ni;
        public G2NETWORK_INFO _device_ni;
        public G2CHANNEL_SET _channels;
        public G2STRING_64 _site;
        public G2STRING_128 _title;
        public G2POINT _startup_pos;
        [MarshalAs(UnmanagedType.U1)]
        public bool _startup_pos_use;
        [MarshalAs(UnmanagedType.U1)]
        public bool _no_top_most;
        [MarshalAs(UnmanagedType.U1)]
        public bool _no_delete_file;
        [MarshalAs(UnmanagedType.U1)]
        public bool _no_port_unity;
        public int _message_duration;
    }
}
