using System;
using System.Collections.Generic;
using System.Text;
using GDK;

namespace GDK_tester
{
    public class search_data
    {
        public enum MODE
        {
            INVALID = -1,
            LOCAL_G1 = 0,   // dvr local search
            LOCAL_G2,       // dvr or nvr local search based on g2search
            PLAY,           // recording service
            BACKUP          // backup service
        }

        public search_data(MODE mode, int channel)
        {
            this._mode = mode;
            this._channel = channel;
            this._request_record_time = false;
            this._date = new search_date_info();
            this._minute = new search_minute_info();
            this._spot_standard = G2SPOT.INVALID_SPOT;
            this._spot_disp = G2SPOT.INVALID_SPOT;
            this._spot_disp_channelext = -1;
            this._record_scope = G2SCOPE.INVALID_SCOPE;
            this._record_date = new List<G2TIME>();
            this._record_hour = new List<bool[]>();
            this._record_minute_IDR = new List<byte[]>();
        }

        public void set_scope(G2SCOPE scope)
        {
            _record_scope = scope;
        }
        public void set_date(ref G2RECORD_TIME_INFO rti)
        {
            _date.append(ref rti);
        }
        public void set_minute(ref G2RECORD_TIME_INFO rti)
        {
            _minute.append(ref rti);
        }

        public void set_record_scope(G2SCOPE scope)
        {
            DateTime b_time = scope._begin._time;
            DateTime e_time = scope._end._time;
            G2SPOT b = new G2SPOT(scope._begin._segment, G2TIME.create(b_time.Year, b_time.Month, b_time.Day, 0, 0, 0), 0);
            G2SPOT e = new G2SPOT(scope._end._segment, G2TIME.create(e_time.Year, e_time.Month, e_time.Day, 23, 59, 59), G2SPOT.INVALID_SPOT_TICK);

            _record_scope = new G2SCOPE(b, e);
        }
        public void set_record_date(G2TIME[] dates)
        {
            lock (_record_date)
            {
                _record_date.Clear();
                _record_date.AddRange(dates);
                _record_date.Sort();

                for (int i = 0; i < _record_date.Count - 1; )
                {
                    if (_record_date[i] == _record_date[i + 1])
                    {
                        _record_date.RemoveAt(i);
                    }
                    else
                    {
                        ++i;
                    }
                }
            }
        }
        public void set_record_hour(bool[][] hours)
        {
            lock (_record_hour)
            {
                _record_hour.Clear();

                for (int i = 0; i < hours.Length; ++i)
                {
                    bool[] h = new bool[24];
                    Array.Copy(hours[i], h, 24);
                    _record_hour.Add(h);
                }
            }
        }
        public void set_record_minute_IDR(List<byte[]> minutes)
        {
            lock (_record_minute_IDR)
            {
                _record_minute_IDR.Clear();

                foreach (byte[] m in minutes)
                {
                    byte[] buf = new byte[1440];
                    Array.Copy(m, buf, 1440);
                    _record_minute_IDR.Add(buf);
                }
            }
        }
        public void set_record_minute(G2RECORD_TIME_INFO[] rtis)
        {
            _minute.append(rtis, _record_scope);
        }

        public void clear_record_time_info()
        {
            _minute.clear();

            if (_mode == MODE.LOCAL_G1)
            {
                lock (_record_hour)
                {
                    _record_hour.Clear();
                }

                lock (_record_minute_IDR)
                {
                    _record_minute_IDR.Clear();
                }
            }
        }

        public bool get_record_date(List<DateTime> dates)
        {
            lock (_record_date)
            {
                foreach (G2TIME time in _record_date)
                {
                    dates.Add(time);
                }
            }
            return (dates.Count != 0);
        }
        public bool get_record_date_last(out G2TIME date)
        {
            date = G2TIME.INVALID_TIME;
            List<DateTime> dates = new List<DateTime>();
            if (get_record_date(dates))
            {
                date = dates[dates.Count - 1];
            }
            return date.valid;
        }
        public bool get_record_minute_IDR(int channelext, byte[] minutes)
        {
            lock (_record_minute_IDR)
            {
                if (channelext >= 0 &&
                    channelext < _record_minute_IDR.Count)
                {
                    Array.Copy(_record_minute_IDR[channelext], minutes, 1440);
                }
            }
            return (minutes.Length > 0);
        }

        public bool contains_record_date(G2TIME time)
        {
            lock (_record_date)
            {
                foreach (G2TIME val in _record_date)
                {
                    if (val.is_same_date(time))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool is_empty_record_time_info()
        {
            if (_mode == MODE.LOCAL_G1)
            {
                return (_record_hour.Count == 0 &&
                        _record_minute_IDR.Count == 0 &&
                        _minute.empty());
            }
            else
            {
                return _minute.empty();
            }
        }

        private MODE _mode;
        private int _channel;
        public bool _request_record_time;
        public search_date_info _date;
        public search_minute_info _minute;
        public G2SPOT _spot_standard;
        public G2SPOT _spot_disp;
        public int _spot_disp_channelext;
        public G2SCOPE _record_scope;
        public List<G2TIME> _record_date;
        public List<bool[]> _record_hour;
        public List<byte[]> _record_minute_IDR;
    }
}
