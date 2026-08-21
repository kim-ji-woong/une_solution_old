using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using GDK;

namespace GDK_tester
{
    using buf_list = System.Collections.Generic.LinkedList<frame_element>;

    public class frame_buf_queue
    {
        public frame_buf_queue(int capacity)
        {
            this._event_full = new EventWaitHandle(false, EventResetMode.AutoReset);
            this._list = new buf_list();
            this._count = 0;
            this._limit = capacity;
            this._capacity = capacity;
        }

        public void set_channelset(g2channel_set channelset_interest, g2channel_set channelset)
        {
            lock (_list)
            {
                _limit = Math.Max(30, Math.Min(_capacity, channelset.size() * 8));
            }
        }

        public bool push(frame_element element, bool discard)
        {
            if (is_full()) return false;

            lock (_list)
            {
                if (is_full() != true)
                {
                    _list.AddLast(element);
                    _count = _list.Count;
                    return true;
                }
            }
            return false;
        }
        public bool pop(out frame_element element)
        {
            check_full_release();
            element = null;

            if (is_empty()) return false;

            lock (_list)
            {
                if (is_empty() != true)
                {
                    element = _list.First.Value;
                    _list.RemoveFirst();
                    _count = _list.Count;
                }
            }
            return (element != null);
        }
        public bool top(out frame_element element)
        {
            element = null;
            if (is_empty()) return false;

            lock (_list)
            {
                if (is_empty() != true)
                {
                    element = _list.First.Value;
                }
            }
            return (element != null);
        }
        public void clear()
        {
            lock (_list)
            {
                _list.Clear();
                _count = 0;
                _event_full.Set();
            }
        }
        public void clear(int channelext)
        {
            lock (_list)
            {
                LinkedListNode<frame_element> node = _list.First;
                while (node != null)
                {
                    LinkedListNode<frame_element> node_next = node.Next;
                    if (node.Value.channelext == channelext)
                    {
                        _list.Remove(node);
                    }
                    node = node_next;
                }
            }
        }

        public void lock_full(int msec)
        {
            _event_full.WaitOne(msec);
        }
        public void check_full_release()
        {
            if (_count < (_limit >> 1))
            {
                _event_full.Set();
            }
        }

        public int count() { return _list.Count; }
        public bool is_empty() { return _list.Count == 0; }
        public bool is_full() { return _count >= _limit; }

        EventWaitHandle _event_full;
        buf_list _list;
        int _count;
        int _limit;
        int _capacity;
    }
}
