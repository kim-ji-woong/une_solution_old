using System;
using System.Collections.Generic;
using System.Text;
using GDK;

namespace GDK_tester
{
    using G2RECORD_TIME_INFO_LIST = List<G2RECORD_TIME_INFO>;

    public class search_minute_info
    {
        public class element_type
        {
            public element_type(G2SPOT spot, G2FRAME.FLAG record_type)
            {
                this.spot = spot;
                this.record_type = record_type;
            }
            public G2SPOT spot;
            public G2FRAME.FLAG record_type;
        }

        public search_minute_info()
        {
            _list = new G2RECORD_TIME_INFO_LIST();
        }

        public void clear()
        {
            lock (_list)
            {
                _list.Clear();
            }
        }
        public bool empty()
        {
            return (_list.Count == 0);
        }
        public void append(ref G2RECORD_TIME_INFO rti)
        {
            lock (_list)
            {
                _list.Add(rti);
            }
        }
        public void append(G2RECORD_TIME_INFO[] rtis, G2SCOPE scope)
        {
            lock (_list)
            {
                foreach (G2RECORD_TIME_INFO rti in rtis)
                {
                    if (scope.contains(rti._spot))
                    {
                        _list.Add(rti);
                    }
                }
            }
        }
        public void sort()
        {
            lock (_list)
            {
                _list.Sort(new G2RECORD_TIME_INFO.comparer_spot());
            }
        }

        public bool get(int channelext, List<search_minute_info.element_type> minutes)
        {
            if (empty()) return false;

            lock (_list)
            {
                foreach (G2RECORD_TIME_INFO rti in _list)
                {
                    for (int i = 0; i < rti._time_size; ++i)
                    {
                        G2FRAME.FLAG record_type = G2FRAME.FLAG.UNDEFINED;
                        for (int j = 0; j < rti._channels._len; ++j)
                        {
                            G2RECORD_TYPE_INFO.ELEMENT_TYPE e = rti._record_type[i]._elements[j];
                            if (channelext == -1 ||
                                channelext == e._channelext)
                            {
                                record_type = e.record_type;
                                break;
                            }
                        }

                        minutes.Add(new element_type(rti._spot, record_type));
                    }
                }
            }
            return (minutes.Count != 0);
        }
        public bool get(List<search_minute_info.element_type> minutes)
        {
            return get(-1, minutes);
        }

        public G2RECORD_TIME_INFO_LIST _list;
    }
}
