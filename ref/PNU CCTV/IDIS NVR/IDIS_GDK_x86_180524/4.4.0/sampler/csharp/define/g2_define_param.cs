using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace GDK
{
    public partial struct G2PARAM_LIST_int32
    {
        public int[] to()
        {
            int[] list = new int[_len];
            Marshal.Copy(_val, list, 0, (int)_len);
            return list;
        }
        public void set(IntPtr ptr, int len) { _val = ptr; _len = (uint)len; }
        public static long serialize(BinaryWriter s, g2set<int> vals)
        {
            for (int i = 0; i < vals.Count; ++i)
            {
                s.Write(vals[i]);
            }
            return s.BaseStream.Position;
        }
    }

    public partial class G2SERVICE_SEARCH_OPTION_EVENT_LOG
    {
        public G2SERVICE_SEARCH_OPTION_EVENT_LOG()
        {
            _source = new G2GUIDSET();
            _event = new g2set<int>();
            _action = new g2set<int>();
            _channels = new G2CHANNEL_SET();
            _scope = new G2SCOPE();
            _direction = DIRECTION.BACKWARD;
            _type = TYPE.RECORDING;
            _row_id = -1;
            _request_count = 100;
            _retrieve_removed_device = false;
            _union_result = false;
        }
        public void to_param(ref G2SERVICE_SEARCH_OPTION_EVENT_LOG.PARAM_TYPE options, MemoryStream stream, out GCHandle gch)
        {
            long[] pos = new long[3];
            using (BinaryWriter bin = new BinaryWriter(stream))
            {
                pos[0] = stream.Position;
                pos[1] = G2PARAM_GUID_LIST.serialize(bin, _source);
                pos[2] = G2PARAM_LIST_int32.serialize(bin, _event);
                G2PARAM_LIST_int32.serialize(bin, _action);
            }

            byte[] buf = stream.GetBuffer();
            gch = GCHandle.Alloc(buf, GCHandleType.Pinned);
            IntPtr ptr = gch.AddrOfPinnedObject();
            long begin = ptr.ToInt64();
            options._source._guid = new IntPtr(begin + pos[0]);
            options._source._len = (uint)_source.size();
            options._event.set(new IntPtr(begin + pos[1]), _event.size());
            options._action.set(new IntPtr(begin + pos[2]), _action.size());
            options._channels = _channels.to();
            options._scope = _scope;
            options._direction = (int)_direction;
            options._type = (int)_type;
            options._row_id = _row_id;
            options._request_count = _request_count;
            options._retrieve_removed_device = _retrieve_removed_device;
            options._uion_result = _union_result;
        }
    }

    public partial class G2SERVICE_SEARCH_OPTION_TEXT_IN_LOG
    {
        public G2SERVICE_SEARCH_OPTION_TEXT_IN_LOG()
        {
            _source = new G2GUIDSET();
        }
        public void to_param(ref G2SERVICE_SEARCH_OPTION_TEXT_IN_LOG.PARAM_TYPE options, MemoryStream stream, out GCHandle gch)
        {
            long[] pos = new long[1];
            using (BinaryWriter bin = new BinaryWriter(stream))
            {
                pos[0] = stream.Position;
                G2PARAM_GUID_LIST.serialize(bin, _source);
            }

            byte[] buf = stream.GetBuffer();
            gch = GCHandle.Alloc(buf, GCHandleType.Pinned);
            IntPtr ptr = gch.AddrOfPinnedObject();
            long begin = ptr.ToInt64();
            options._source._guid = new IntPtr(begin + pos[0]);
            options._source._len = (uint)_source.size();
            options._options = _options;
        }
    }

    public partial class G2SERVICE_SEARCH_OPTION_ACTION_ACK_LOG
    {
        public G2SERVICE_SEARCH_OPTION_ACTION_ACK_LOG()
        {
            _source = new G2GUIDSET();
            _event = new g2set<int>();
            _row_id = -1;
            _request_count = 100;
        }
        public void to_param(ref G2SERVICE_SEARCH_OPTION_ACTION_ACK_LOG.PARAM_TYPE options, MemoryStream stream, out GCHandle gch)
        {
            long[] pos = new long[2];
            using (BinaryWriter bin = new BinaryWriter(stream))
            {
                pos[0] = stream.Position;
                pos[1] = G2PARAM_GUID_LIST.serialize(bin, _source);
                G2PARAM_LIST_int32.serialize(bin, _event);
            }

            byte[] buf = stream.GetBuffer();
            gch = GCHandle.Alloc(buf, GCHandleType.Pinned);
            IntPtr ptr = gch.AddrOfPinnedObject();
            long begin = ptr.ToInt64();
            options._source._guid = new IntPtr(begin + pos[0]);
            options._source._len = (uint)_source.size();
            options._event.set(new IntPtr(begin + pos[1]), _event.size());
            options._scope = _scope;
            options._sender = _sender;
            options._receiver = _receiver;
            options._row_id = _row_id;
            options._request_count = _request_count;
            options._action = _action;
            options._ack = _ack;
        }
    }

    public partial class G2SERVICE_SEARCH_OPTION_SYSTEM_LOG
    {
        public G2SERVICE_SEARCH_OPTION_SYSTEM_LOG()
        {
            _GUIDs = new G2GUIDSET();
            _GUIDs_reserved = new G2GUIDSET();
            _IDs = new g2set<int>();
            _row_id = -1;
            _request_count = 100;
        }
        public void to_param(ref G2SERVICE_SEARCH_OPTION_SYSTEM_LOG.PARAM_TYPE options, MemoryStream stream, out GCHandle gch)
        {
            long[] pos = new long[3];
            using (BinaryWriter bin = new BinaryWriter(stream))
            {
                pos[0] = stream.Position;
                pos[1] = G2PARAM_GUID_LIST.serialize(bin, _GUIDs);
                pos[2] = G2PARAM_GUID_LIST.serialize(bin, _GUIDs_reserved);
                G2PARAM_LIST_int32.serialize(bin, _IDs);
            }

            byte[] buf = stream.GetBuffer();
            gch = GCHandle.Alloc(buf, GCHandleType.Pinned);
            IntPtr ptr = gch.AddrOfPinnedObject();
            long begin = ptr.ToInt64();
            options._GUIDs.set(new IntPtr(begin + pos[0]), _GUIDs.size());
            options._GUIDs_reserved._guid = new IntPtr(begin + pos[1]);
            options._GUIDs_reserved._len = (uint)_GUIDs_reserved.size();
            options._IDs.set(new IntPtr(begin + pos[2]), _IDs.size());
            options._row_id = _row_id;
            options._request_count = _request_count;
        }
    }

    public partial class G2SERVICE_SEARCH_OPTION_DEBUG_LOG
    {
        public G2SERVICE_SEARCH_OPTION_DEBUG_LOG()
        {
            _row_id = -1;
            _request_count = 100;
        }
        public void to_param(ref G2SERVICE_SEARCH_OPTION_DEBUG_LOG.PARAM_TYPE options, MemoryStream stream, out GCHandle gch)
        {
            byte[] buf = stream.GetBuffer();
            gch = GCHandle.Alloc(buf, GCHandleType.Pinned);
            IntPtr ptr = gch.AddrOfPinnedObject();
            long begin = ptr.ToInt64();
            options._row_id = _row_id;
            options._request_count = _request_count;
        }
    }
}
