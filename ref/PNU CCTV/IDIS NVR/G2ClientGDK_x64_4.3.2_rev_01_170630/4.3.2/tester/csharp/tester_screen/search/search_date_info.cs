using System;
using System.Collections.Generic;
using System.Text;
using GDK;

namespace GDK_tester
{
    using G2RECORD_TIME_INFO_LIST = LinkedList<G2RECORD_TIME_INFO>;

    public class search_date_info
    {
        public search_date_info()
        {
            _list = new G2RECORD_TIME_INFO_LIST();
        }

        public void clear()
        {
            _list = new G2RECORD_TIME_INFO_LIST();
        }
        public bool empty()
        {
            return (_list.Count == 0);
        }
        public void append(ref G2RECORD_TIME_INFO rti)
        {
            lock (_list)
            {
                _list.AddLast(rti);
            }
        }

        public bool get(int channelext, List<DateTime> dates)
        {
            if (empty()) return false;

            TimeSpan span = new TimeSpan(1, 0, 0, 0);

            lock (_list)
            {
                foreach (G2RECORD_TIME_INFO rti in _list)
                {
                    if (rti._spot._time.valid)
                    {
                        DateTime time = rti._spot._time;
                        for (int i = 1; i < rti._time_size; ++i)
                        {
                            for (int j = 0; j < rti._channels._len; ++j)
                            {
                                G2RECORD_TYPE_INFO.ELEMENT_TYPE e = rti._record_type[i]._elements[j];
                                if (e._rec_type != 0)
                                {
                                    if (channelext == -1 ||
                                        channelext == e._channelext)
                                    {
                                        dates.Add(time);
                                    }
                                }
                            }
                            time += span;
                        }
                    }
                }

                dates.Sort();

                for (int i = 0; i < dates.Count - 1; )
                {
                    if (dates[i] == dates[i + 1])
                    {
                        dates.RemoveAt(i);
                    }
                    else
                    {
                        ++i;
                    }
                }
                return (dates.Count != 0);
            }
        }
        public bool get(List<DateTime> dates)
        {
            return get(-1, dates);
        }

        public G2RECORD_TIME_INFO_LIST _list;
    }
}
