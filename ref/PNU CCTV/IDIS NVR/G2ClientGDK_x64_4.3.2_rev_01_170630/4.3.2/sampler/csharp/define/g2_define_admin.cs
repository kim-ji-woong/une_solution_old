using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDK
{
#if _WIN64
    using G2WPARAM = System.UInt64;
    using G2LPARAM = System.Int64;
    using G2UPARAM = System.IntPtr;
    using G2RESULT = System.Int64;
#else
    using G2WPARAM = System.UInt32;
    using G2LPARAM = System.Int32;
    using G2UPARAM = System.IntPtr;
    using G2RESULT = System.Int32;
#endif

    public struct G2ADMIN_CALLBACK
    {
        public enum TYPE
        {
            on_connected = 0,
            on_disconnected = 1,
            on_login_failed_from_dvrns = 2,
            on_login_completed = 3,
            on_failover_prepare = 4,
            on_failover_connected = 5,
            on_failover_failed = 6,
            on_notify_connectable_service = 7,
            on_receive_device_empty = 8,
            on_receive_device_append = 9,
            on_receive_device_modify = 10,
            on_receive_device_remove = 11,
            on_receive_device_remove_list = 12,
            on_receive_device_group_list = 13,
            on_receive_device_group_append = 14,
            on_receive_device_group_modify = 15,
            on_receive_device_group_remove = 16,
            on_receive_device_to_group_map = 17,
            on_receive_device_append_to_group = 18,
            on_receive_device_remove_to_group = 19,
            on_receive_device_list = 20,
            on_receive_device_list_append_to_group = 21,
            on_receive_device_list_remove_to_group = 22,
            on_receive_layout_list = 23,
            on_receive_layout_append = 24,
            on_receive_layout_modify = 25,
            on_receive_layout_remove = 26,
            on_receive_sequence_list = 27,
            on_receive_sequence_append = 28,
            on_receive_sequence_modify = 29,
            on_receive_sequence_remove = 30,
            on_receive_recording_device_status = 31,
            on_receive_service_log_load = 32,
            on_receive_service_log_load_end = 33,
            on_receive_service_log_load_fail = 34,
            on_receive_service_log_load_stop = 35,
            on_receive_debug_log_load = 36,
            on_receive_debug_log_load_end = 37,
            on_receive_debug_log_load_fail = 38,
            on_receive_debug_log_load_stop = 39,
            on_modify_device_enable = 40,
            on_receive_test_failover_end = 41,

            CALLBACK_COUNT = 42
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2DEVICE_ROOT
    {
        public G2VERSION _version;
        public G2GUID _guid;
        [MarshalAs(UnmanagedType.U1)]
        public bool _enable;
        public int _parent_id;
        public G2STRING_128 _name;
        public G2STRING_256 _desc;
        public G2NETWORK_INFO _ni;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2DEVICE_LEAF
    {
        public G2VERSION _version;
        public G2GUID _guid;
        public G2GUID _guid_parent;
        public G2GUID _guid_root;
        public uint _number;
        public uint _number_root_based;
        [MarshalAs(UnmanagedType.U1)]
        public bool _enable;
        public G2STRING_128 _name;
        public G2STRING_256 _desc;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2DEVICE_GROUP
    {
        public G2GUID _guid;
        public G2GUID _guid_parent;
        public G2STRING_128 _name;
        public G2STRING_256 _desc;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2GROUP_MEMBER
    {
        public G2GUID _group;
        public G2GUID _member;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2GROUP_MEMBER_LIST
    {
        public IntPtr _members;
        public uint _len;
        public G2GROUP_MEMBER[] to()
        {
            G2GROUP_MEMBER[] list = new G2GROUP_MEMBER[_len];
            for (int i = 0; i < list.Length; ++i)
            {
                IntPtr ptr = new IntPtr(_members.ToInt64() + i * Marshal.SizeOf(typeof(G2GROUP_MEMBER)));
                list[i] = (G2GROUP_MEMBER)Marshal.PtrToStructure(ptr, typeof(G2GROUP_MEMBER));
            }
            return list;
        }
    }

    public struct G2DEVICE_GROUP_LIST
    {
        public IntPtr _groups;
        public uint _len;
        public G2DEVICE_GROUP[] to()
        {
            G2DEVICE_GROUP[] list = new G2DEVICE_GROUP[_len];
            for (int i = 0; i < list.Length; ++i)
            {
                IntPtr ptr = new IntPtr(_groups.ToInt64() + i * Marshal.SizeOf(typeof(G2DEVICE_GROUP)));
                list[i] = (G2DEVICE_GROUP)Marshal.PtrToStructure(ptr, typeof(G2DEVICE_GROUP));
            }
            return list;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2LAYOUT
    {
        public enum FORMAT
        {
            LAYOUT_UNDEFINED = -1,
            LAYOUT_1x1 = 0,
            LAYOUT_2x2 = 1,
            LAYOUT_3x3 = 2,
            LAYOUT_4x4 = 3,
            LAYOUT_5x5 = 4,
            LAYOUT_6x6 = 5,
            LAYOUT_7x7 = 6,
            LAYOUT_8x8 = 7,
            LAYOUT_OFBASE = 8,    // sentinel; this is not layout
            LAYOUT_5p1 = 9,
            LAYOUT_7p1 = 10,
            LAYOUT_9p1 = 11,
            LAYOUT_11p1 = 12,
            LAYOUT_12p1 = 13,
            LAYOUT_27p1 = 14,
            LAYOUT_8p2 = 15,
            LAYOUT_18p2 = 16,
            LAYOUT_4p3 = 17,
            LAYOUT_9p3 = 18,
            LAYOUT_3x2w = 19,
            LAYOUT_4x3w = 20,
            LAYOUT_5x4w = 21,
            LAYOUT_6x5w = 22,
            LAYOUT_2p1w_v2 = 57,
            LAYOUT_2p1w = 23,
            LAYOUT_3p1w = 24,
            LAYOUT_4p1w = 25,
            LAYOUT_5p1w = 26,
            LAYOUT_6p1w = 27,
            LAYOUT_8p1w = 28,
            LAYOUT_10p1w = 29,
            LAYOUT_4p2w = 30,
            LAYOUT_12p2w = 31,
            LAYOUT_12h1 = 32,
            LAYOUT_32h1 = 33,
            LAYOUT_28p4 = 34,
            LAYOUT_12p4w = 35,
            LAYOUT_16p12 = 36,
            LAYOUT_24p12w = 37,
            LAYOUT_7x6w = 38,
            LAYOUT_8x7w = 39,
            LAYOUT_8x1 = 40,
            LAYOUT_9x1 = 41,
            LAYOUT_10x1 = 42,
            LAYOUT_11x1 = 43,
            LAYOUT_12x1 = 44,
            LAYOUT_13x1 = 45,
            LAYOUT_14x1 = 46,
            LAYOUT_15x1 = 47,
            LAYOUT_16x1 = 48,
            LAYOUT_5x3w = 49,
            LAYOUT_6x3w = 50,
            LAYOUT_8x3w = 51,
            LAYOUT_8x5w = 52,
            LAYOUT_2x1 = 53,
            LAYOUT_3x1 = 54,
            LAYOUT_4x1 = 55,
            LAYOUT_32h1_v2 = 56,
            LAYOUT_COUNT = 58
        }
        public enum SHARING_MODE
        {
            PUBLIC = 0,
            GROUP,
            PRIVATE
        }

        public G2GUID _guid;
        public G2GUID _guid_admin_svr;
        public G2STRING_128 _name;
        public G2STRING_256 _desc;
        [MarshalAs(UnmanagedType.U1)]
        public bool _federated;
        public int _sharing;
        public int _format;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public G2GUID[] _cameras;
        public int _spot_hot;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] _spot_evt;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] _spot_evt_map;

        bool federated { get { return _federated; } }
        SHARING_MODE sharing { get { return (SHARING_MODE)_sharing; } }
        FORMAT format { get { return (FORMAT)_format; } }

        g2channel_set get_event_spot()
        {
            g2channel_set chs = new g2channel_set();
            int i = 0;
            foreach (byte f in _spot_evt)
            {
                if (f != 0)
                {
                    chs.insert(i);
                }
                i = i + 1;
            }
            return chs;
        }
        g2channel_set get_map_event_spot()
        {
            g2channel_set chs = new g2channel_set();
            int i = 0;
            foreach (byte f in _spot_evt_map)
            {
                if (f != 0)
                {
                    chs.insert(i);
                }
                i = i + 1;
            }
            return chs;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2SEQUENCE
    {
        public enum TYPE
        {
            UNKNOWN = -1,
            CAMERA = 0,
            LAYOUT
        }
        public enum SHARING_MODE
        {
            PUBLIC = 0,
            GROUP,
            PRIVATE
        }

        public struct ITEM
        {
            public G2GUID _item;
            public int _duration;
        }

        public G2GUID _guid;
        public G2GUID _guid_admin_svr;
        public G2STRING_128 _name;
        public G2STRING_256 _desc;
        public TYPE _type;
        [MarshalAs(UnmanagedType.U1)]
        public bool _federated;
        public int _sharing;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public ITEM[] _list;

        TYPE type { get { return (TYPE)_type; } }
        bool federated { get { return _federated; } }
        SHARING_MODE sharing { get { return (SHARING_MODE)_sharing; } }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2SERVICE_NETWORK_INFO
    {
        public struct element_t
        {
            public int _type;
            public G2STRING_128 _address;
            public G2MAC_ADDRESS _mac;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)G2NETWORK_INFO.PORT_TYPE.MAX_PORT_INDEX)]
            public ushort[] _port;
        }
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public element_t[] _element;
        public G2STRING_64 _user_id;
        public G2STRING_128 _password;
        public uint _count;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2SERVICE
    {
        public G2GUID _guid;
        public G2STRING_128 _name;
        public G2STRING_256 _desc;
        [MarshalAs(UnmanagedType.U1)]
        public bool _registered;
        public G2SERVICE_NETWORK_INFO _ni;
        public G2GUID _adminKey;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2SERVICE_ITEM
    {
        public G2GUID _service;
        public G2GUID _site;
        G2SERVICE_ITEM(ref G2GUID service, ref G2GUID site)
        {
            _service = service;
            _site = site;
        }
        public static uint proc_enum_ITEM_List(ref G2GUID service, ref G2GUID site, G2UPARAM param)
        {
            GCHandle gc = GCHandle.FromIntPtr(param);
            List<G2SERVICE_ITEM> ITEMs = (List<G2SERVICE_ITEM>)gc.Target;
            G2SERVICE_ITEM item = new G2SERVICE_ITEM(ref service, ref site);
            ITEMs.Add(item);
            return G2BOOLEAN.G2TRUE;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2RECORDING_CAMERA_STATUS
    {
        public enum REC_TYPE
        {
            TIME_LAPSE = 0,
            EVENT,
            INSTANT,
            IDLE
        }
        public enum REC_FAIL_REASON
        {
            VIDEO_LOSS = 0,
            DEACTIVATED,
            RECORD_FAIL,
            STORAGE_FULL,
            DISCONNECTED,
            NO_SCHEDULE,
            RECORD_SUCCESS
        }

        public G2GUID _camera;
        [MarshalAs(UnmanagedType.U1)]
        public bool _on_recording;
        [MarshalAs(UnmanagedType.U1)]
        public bool _on_recording_audio;
        public REC_TYPE _rec_type;
        public REC_FAIL_REASON _rec_fail_reason;
        public G2TIME _from;
        public G2TIME _to;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2RECORDING_DEVICE_STATUS
    {
        public G2GUID _root;
        public int _connection;
        public int _disconnect_reason;
        public G2PARAM_BUNCH _status;

        public G2RECORDING_CAMERA_STATUS[] status()
        {
            G2RECORDING_CAMERA_STATUS[] s = new G2RECORDING_CAMERA_STATUS[_status._len];
            for (int i = 0; i < s.Length; ++i)
            {
                IntPtr ptr = new IntPtr(_status._params.ToInt64() + i * Marshal.SizeOf(typeof(G2RECORDING_CAMERA_STATUS)));
                s[i] = (G2RECORDING_CAMERA_STATUS)Marshal.PtrToStructure(ptr, typeof(G2RECORDING_CAMERA_STATUS));
            }
            return s;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct G2DEVICE_NUMERIC_ID
    {
        public uint _service;
        public uint _device;
    }

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate uint G2PROC_ENUM_GUID(ref G2GUID guid, G2UPARAM param);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate uint G2PROC_ENUM_SERVICE_ITEM(ref G2GUID service, ref G2GUID site, G2UPARAM param);
}
