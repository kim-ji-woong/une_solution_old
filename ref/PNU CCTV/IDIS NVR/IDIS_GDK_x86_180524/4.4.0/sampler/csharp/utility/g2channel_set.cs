using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GDK
{
    public class g2channel_set : IEnumerable
    {
        public g2channel_set() { _set = new g2set<int>(); }
        public g2channel_set(g2channel_set right) { _set = new g2set<int>(right._set); }
        public g2channel_set(g2set<int> right) { _set = new g2set<int>(right); }
        public g2channel_set(G2CHANNEL_SET right)
        {
            _set = new g2set<int>();
            from(ref right);
        }
        public g2channel_set(int ch) { _set = new g2set<int>(ch); }
        public g2channel_set(int begin, int end)
        {
            _set = new g2set<int>();

            for (int i = begin; i < end; ++i)
            {
                _set.insert(i);
            }
        }

        public IEnumerator GetEnumerator() { return _set.GetEnumerator(); }

        public void insert(int ch) { _set.insert(ch); }
        public void insert(params int[] chs) { _set.insert(chs); }
        public void insert(g2channel_set right) { _set.insert(right._set); }
        public void assign(g2channel_set right) { _set.assign(right._set); }
        public void clear() { _set.clear(); }
        public bool erase(int ch) { return _set.erase(ch); }

        public int this[int i] { get { return _set[i]; } }

        public bool empty() { return _set.empty(); }
        public bool contains(int ch) { return _set.contains(ch); }
        public bool equal(g2channel_set right) { return _set.equal(right); }
        public int size() { return _set.size(); }

        public g2channel_set clone() { return new g2channel_set(this); }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            for (int i = 0; i != _set.Count; )
            {
                sb.Append(_set[i]);

                if (++i != _set.Count)
                {
                    sb.Append(", ");
                }
            }
            sb.Append("}");
            return sb.ToString();
        }

        public static g2channel_set operator +(g2channel_set left, g2channel_set right) { return new g2channel_set(left._set + right._set); }
        public static g2channel_set operator -(g2channel_set left, g2channel_set right) { return new g2channel_set(left._set - right._set); }
        public static g2channel_set operator &(g2channel_set left, g2channel_set right) { return new g2channel_set(left._set & right._set); }
        public static implicit operator G2CHANNEL_SET(g2channel_set right) { return right.to(); }
        public static implicit operator g2channel_set(G2CHANNEL_SET right) { return new g2channel_set(right); }
        public static implicit operator g2set<int>(g2channel_set chs) { return chs._set; }

        public G2CHANNEL_SET to() { return new G2CHANNEL_SET(_set.ToArray()); }
        public int[] to_array() { return _set.to_array(); }
        public void from(ref G2CHANNEL_SET right)
        {
            for (int i = 0; i < right._len; ++i)
            {
                _set.insert(right._channels[i]);
            }
        }
        public uint to_uint32()
        {
            uint chs = 0;
            for (int i = 0; i < 32; ++i)
            {
                if (contains(i))
                {
                    chs |= (1U << i);
                }
            }
            return chs;
        }
        public ulong to_uint64()
        {
            ulong chs = 0;
            for (int i = 0; i < 64; ++i)
            {
                if (contains(i))
                {
                    chs |= (1UL << 1);
                }
            }
            return chs;
        }
        public void from(uint chs)
        {
            for (int i = 0; i < 32; ++i)
            {
                if ((chs & (1U << i)) != 0)
                {
                    insert(i);
                }
            }
        }
        public void from(ulong chs)
        {
            for (int i = 0; i < 64; ++i)
            {
                if ((chs & (1UL << i)) != 0)
                {
                    insert(i);
                }
            }
        }

        public g2set<int> _set;
    }

    public class g2channel_stream_set : IEnumerable
    {
        public g2channel_stream_set() { _set = new g2set<G2CHANNEL_STREAM>(); }
        public g2channel_stream_set(g2channel_stream_set right) { _set = new g2set<G2CHANNEL_STREAM>(right._set); }
        public g2channel_stream_set(g2set<G2CHANNEL_STREAM> right) { _set = new g2set<G2CHANNEL_STREAM>(right); }
        public g2channel_stream_set(G2CHANNEL_STREAM_SET right) { _set = new g2set<G2CHANNEL_STREAM>(); from(ref right); }
        public g2channel_stream_set(int begin, int end)
        {
            _set = new g2set<G2CHANNEL_STREAM>();

            for (int i = begin; i < end; ++i)
            {
                _set.insert(new G2CHANNEL_STREAM(i, 0));
            }
        }

        public IEnumerator GetEnumerator() { return _set.GetEnumerator(); }

        public void insert(int ch) { _set.insert(new G2CHANNEL_STREAM(ch)); }
        public void insert(int ch, int stream_id) { _set.insert(new G2CHANNEL_STREAM(ch, stream_id)); }
        public void insert(G2CHANNEL_STREAM s) { _set.insert(s); }
        public void insert(g2channel_stream_set right) { _set.insert(right._set); }
        public void assign(g2channel_stream_set right) { _set.assign(right._set); }
        public void clear() { _set.clear(); }
        public bool erase(int ch, int stream_id) { return _set.erase(new G2CHANNEL_STREAM(ch, stream_id)); }
        public bool erase(int ch)
        {
            List<G2CHANNEL_STREAM> list = new List<G2CHANNEL_STREAM>();
            foreach (G2CHANNEL_STREAM s in _set)
            {
                if (s._channel == ch)
                {
                    list.Add(s);
                }
            }

            foreach (G2CHANNEL_STREAM s in list)
            {
                _set.erase(s);
            }
            return (list.Count != 0);
        }

        public G2CHANNEL_STREAM this[int i] { get { return _set[i]; } }

        public bool empty() { return _set.empty(); }
        public bool contains(int ch)
        {
            foreach (G2CHANNEL_STREAM s in _set)
            {
                if (s._channel == ch)
                {
                    return true;
                }
            }
            return false;
        }
        public bool contains(int ch, int stream_id) { return _set.contains(new G2CHANNEL_STREAM(ch, stream_id)); }
        public bool contains(G2CHANNEL_STREAM s) { return _set.contains(s); }
        public bool equal(g2channel_stream_set right) { return _set.equal(right); }
        public int size() { return _set.size(); }

        public g2channel_stream_set clone() { return new g2channel_stream_set(this); }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            for (int i = 0; i != _set.Count; )
            {
                sb.Append(_set[i]);

                if (++i != _set.Count)
                {
                    sb.Append(", ");
                }
            }
            sb.Append("}");
            return sb.ToString();
        }

        public static g2channel_stream_set operator +(g2channel_stream_set left, g2channel_stream_set right) { return new g2channel_stream_set(left._set + right._set); }
        public static g2channel_stream_set operator -(g2channel_stream_set left, g2channel_stream_set right) { return new g2channel_stream_set(left._set - right._set); }
        public static g2channel_stream_set operator &(g2channel_stream_set left, g2channel_stream_set right) { return new g2channel_stream_set(left._set & right._set); }
        public static implicit operator G2CHANNEL_STREAM_SET(g2channel_stream_set right) { return right.to(); }
        public static implicit operator g2channel_stream_set(G2CHANNEL_STREAM_SET right) { return new g2channel_stream_set(right); }
        public static implicit operator g2set<G2CHANNEL_STREAM>(g2channel_stream_set chs) { return chs._set; }

        public G2CHANNEL_STREAM_SET to() { return new G2CHANNEL_STREAM_SET(this); }
        public G2CHANNEL_STREAM[] to_array() { return _set.to_array(); }
        public g2channel_set to_channel_set()
        {
            g2channel_set chs = new g2channel_set();
            foreach (G2CHANNEL_STREAM s in _set)
            {
                chs.insert(s._channel);
            }
            return chs;
        }
        public void from(ref G2CHANNEL_STREAM_SET right)
        {
            for (int i = 0; i < right._len; ++i)
            {
                _set.insert(right._streams[i]);
            }
        }
        public void from(g2channel_set chs)
        {
            foreach (int ch in chs)
            {
                insert(ch);
            }
        }

        public g2set<G2CHANNEL_STREAM> _set;
    }
}
