using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDK
{
    public struct G2FEN_CALLBACK
    {
        public enum TYPE
        {
            on_nat_type_discovered = 0,

            CALLBACK_COUNT = 1
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2NAT_INFO
    {
        public enum VERSION
        {
            G2NATINFO_1_0 = 0x00000100,
            G2NATINFO = G2NATINFO_1_0,
        }
        public enum TYPE
        {
            UNKNOWN = 0,
            OPEN_INTERNET,
            FULL_CONE,
            RESTRICTED_CONE,
            PORT_RESTRICTED_CONE,
            SYMMETRIC,
            SYMMETRIC_UDP_FIREWALL,
            UDP_BLOCKED,
            DISCOVERY_FAILED,
            MOBILE
        }
        public enum PORT_MAPPING
        {
            NONE = 0,
            PORT_FORWARDING = 1,
            UPNP = 2,
            NAT_PMP = 4
        }

        public uint _version;
        public byte _type;
        public byte _port_mapping;

        public VERSION version { get { return (VERSION)_version; } }
        public TYPE type { get { return (TYPE)_type; } }
        public PORT_MAPPING port_mapping { get { return (PORT_MAPPING)_port_mapping; } }

        public override string ToString()
        {
            return string.Format("TYPE: {0}, PORT MAPPING: {1}", type, port_mapping);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2FEN_CONNECTION
    {
        public enum TYPE
        {
            UNKNOWN = 0,
            TCP_DIRECT_EXTERNAL = 1,
            TCP_DIRECT_INTERNAL = 2,
            UDP_HOLE_PUNCHING = 3,
            VIA_RELAY_SERVICE = 4
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2FEN_RESULT
    {
        public enum TYPE
        {
            NOT_ERROR = 0,
            NETWORK_ERROR = NOT_ERROR + 1,
            NETWORK_INITIALIZE_ERROR = NOT_ERROR + 2,
            NETWORK_PROTOCOL_ERROR = NOT_ERROR + 3,
            INVALID_PACKET = NOT_ERROR + 4,
            INVALID_PACKET_VERSION = NOT_ERROR + 5,
            NETWORK_INITIALIZE_ERROR_HELPDESK = NOT_ERROR + 6,
            NETWORK_INITIALIZE_ERROR_NOTIFY = NOT_ERROR + 7,
            NETWORK_INITIALIZE_ERROR_MYIPADDRESS = NOT_ERROR + 8,
            QUERY_ERROR = NOT_ERROR + 200,
            NO_NAME_ERROR = QUERY_ERROR + 1,
            NAME_TABLE_RETRIEVE_ERROR = QUERY_ERROR + 2,
            NEW_DDNS_RESPONSE = QUERY_ERROR + 3,
            REMOVE_FAIL = QUERY_ERROR + 4
        }
    }
}
