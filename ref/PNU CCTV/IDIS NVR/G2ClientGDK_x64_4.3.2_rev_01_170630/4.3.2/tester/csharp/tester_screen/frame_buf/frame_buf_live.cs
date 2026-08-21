using System;
using System.Collections.Generic;
using System.Text;
using GDK;

namespace GDK_tester
{
    using buf_list = System.Collections.Generic.LinkedList<frame_element>;
    using buf_bunch = System.Collections.Generic.List<System.Collections.Generic.LinkedList<frame_element>>;
    using stream_map = System.Collections.Generic.Dictionary<G2CHANNEL_STREAM, int>;
    using stream_bunch = System.Collections.Generic.List<int>;

    public class frame_buf_live : frame_buf
    {
        public class internal_data
        {
            public class node
            {
                public node() { reset(); }
                public void reset()
                {
                    clear();
                    reserved = false;
                }
                public void clear()
                {
                    count_iframe = count_element = 0;
                    cleared = true;
                }

                public int count_iframe;
                public int count_element;
                public bool reserved;
                public bool cleared;
            }

            public internal_data() { _count = 0; }
            public void create(int count)
            {
                _node_list = new node[count];
                _count = count;
                for (int i = 0; i < _node_list.Length; ++i)
                {
                    _node_list[i] = new node();
                }
            }
            public void reset()
            {
                foreach (node n in _node_list)
                {
                    n.reset();
                }
            }
            public int find_free()
            {
                int slot = -1;
                for (int i = 0; i < _count; ++i)
                {
                    if (_node_list[i].reserved != true)
                    {
                        slot = i;
                        break;
                    }
                }
                return slot;
            }

            public node get_node(int stream) { return _node_list[stream]; }

            private node[] _node_list;
            private int _count;
        }

        public frame_buf_live(TYPE type, int streams, int limit)
            : base(frame_buf.MODE.LIVE, type, limit)
        {
            _internal = new internal_data();
            _internal.create(streams);
            _buf = new buf_bunch(streams);
            _stream_map = new stream_map();
            _stream_next = 0;
            _streams = new stream_bunch();
        }

        public override void clear(int channelext, int stream_id)
        {
            G2CHANNEL_STREAM cs = new G2CHANNEL_STREAM(channelext, stream_id);
            clear(cs);
        }
        public override void clear(G2CHANNEL_STREAM cs)
        {
            lock (_buf)
            {
                if (_stream_map.ContainsKey(cs))
                {
                    int stream = _stream_map[cs];
                    _buf[stream].Clear();
                    _internal.get_node(stream).clear();
                }
            }
        }
        public override void clear(g2channel_stream_set chs)
        {
            lock (_buf)
            {
                foreach (G2CHANNEL_STREAM cs in chs)
                {
                    if (_stream_map.ContainsKey(cs))
                    {
                        int stream = _stream_map[cs];
                        _buf[stream].Clear();
                        _internal.get_node(stream).clear();
                    }
                }
            }
        }
        public override void remove(int channelext, int stream_id)
        {
            lock (_buf)
            {
                G2CHANNEL_STREAM cs = new G2CHANNEL_STREAM(channelext, stream_id);
                if (_stream_map.ContainsKey(cs))
                {
                    int stream = _stream_map[cs];
                    _buf[stream].Clear();
                    _internal.get_node(stream).reset();
                    _stream_map.Remove(cs);
                    _streams.RemoveAt(stream);
                }
            }
        }
        public override void clear()
        {
            lock (_buf)
            {
                _stream_map.Clear();
                _buf.Clear();
                _internal.reset();
                _stream_map.Clear();
                _streams.Clear();
            }
        }
        public override void push(ref G2FRAME frame, int channel, int pane)
        {
            lock (_buf)
            {
                int channelext = frame.channel;
                int stream_id = frame.stream_id;
                int stream = -1;
                internal_data.node node = null;

                G2CHANNEL_STREAM cs = new G2CHANNEL_STREAM(channelext, stream_id);

                if (_stream_map.TryGetValue(cs, out stream) != true)
                {
                    buf_list list = new buf_list();
                    stream = _internal.find_free();
                    node = _internal.get_node(stream);
                    node.reset();
                    node.reserved = true;

                    _stream_map[cs] = stream;
                    _streams.Add(stream);
                    _buf.Insert(stream, list);
                }

                if (node == null)
                {
                    node = _internal.get_node(stream);
                }

                if (frame.type == G2FRAME.TYPE.I_FRAME)
                {
                    if (node.count_iframe > 1)
                    {
                        clear_stream(stream);
                    }
                }

                if (node.cleared)
                {
                    if (frame.type == G2FRAME.TYPE.I_FRAME)
                    {
                        node.cleared = false;
                    }
                    else
                    {
                        return;
                    }
                }

                frame_element element = new frame_element(ref frame, channel, pane);
                node.count_element++;

                _buf[stream].AddLast(element);

                if (element.type == G2FRAME.TYPE.I_FRAME)
                {
                    node.count_iframe++;
                }

                if (node.count_element >= _limit)
                {
                    clear_stream(stream);
                }
            }
        }
        public override bool pop(out frame_element element)
        {
            lock (_buf)
            {
                element = null;
                if (_streams == null || _streams.Count == 0)
                {
                    return false;
                }

                bool ret = false;
                int count = _streams.Count;

                for (int i = count; i != 0; --i)
                {
                    _stream_next = (_stream_next + 1) % count;

                    int stream = _streams[_stream_next];
                    buf_list list = _buf[stream];
                    internal_data.node node = _internal.get_node(stream);

                    if (list.Count > 0)
                    {
                        element = list.First.Value;
                        list.RemoveFirst();
                        node.count_element--;
                        if (element.type == G2FRAME.TYPE.I_FRAME)
                        {
                            node.count_iframe--;
                        }

                        ret = true;
                        break;
                    }
                }
                return ret;
            }
        }
        public override bool top(out frame_element element)
        {
            element = null;
            return false;
        }

        protected void clear_stream(int stream)
        {
            lock (_buf)
            {
                if (stream >= 0 && stream < _buf.Count)
                {
                    _buf[stream].Clear();
                    _internal.get_node(stream).clear();
                }
            }
        }

        protected internal_data _internal;
        protected buf_bunch _buf;
        protected stream_map _stream_map;
        protected stream_bunch _streams;
        protected int _stream_next;
    }
}
