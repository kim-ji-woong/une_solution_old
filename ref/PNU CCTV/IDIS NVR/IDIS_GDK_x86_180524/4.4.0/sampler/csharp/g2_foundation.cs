using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace GDK
{
    using G2HANDLE = System.Int32;
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

    public class g2foundation
    {
        #region GDK DLL Import
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_time_make_invalid(ref G2TIME time);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_time_get_current(ref G2TIME time);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_time_is_valid(ref G2TIME time);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_time_is_same_date(ref G2TIME left, ref G2TIME right);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_time_is_same_hour(ref G2TIME left, ref G2TIME right);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_time_is_same_minute(ref G2TIME left, ref G2TIME right);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern int g2_time_compare(ref G2TIME left, ref G2TIME right);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern uint g2_time_to_utc(ref G2TIME time);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern uint g2_time_to_time32_t(ref G2TIME time);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern int g2_time_get_year(ref G2TIME time);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern int g2_time_get_month(ref G2TIME time);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern int g2_time_get_day(ref G2TIME time);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern int g2_time_get_hour(ref G2TIME time);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern int g2_time_get_minute(ref G2TIME time);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern int g2_time_get_second(ref G2TIME time);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern uint g2_time_to_uint32(ref G2TIME time);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_time_to_elements(ref G2TIME time, out int year, out int month, out int day, out int hour, out int minute, out int second);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_time_from_utc(ref G2TIME time, uint utc);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_time_from_uint32(ref G2TIME time, uint val);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_time_from_time32_t(ref G2TIME time, uint val);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_time_from_elements(ref G2TIME time, int year, int month, int day, int hour, int minute, int second);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern int g2_time_span_total_hours(ref G2TIME_SPAN ts);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern int g2_time_span_total_minutes(ref G2TIME_SPAN ts);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern int g2_time_span_total_seconds(ref G2TIME_SPAN ts);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_time_span_set_total_seconds(ref G2TIME_SPAN ts, int seconds);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern G2TIME_SPAN g2_time_subtract_time(ref G2TIME left, ref G2TIME right);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern G2TIME g2_time_plus_span(ref G2TIME left, ref G2TIME_SPAN ts);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern G2TIME g2_time_subtract_span(ref G2TIME left, ref G2TIME_SPAN ts);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern int g2_time_span_compare(ref G2TIME_SPAN left, ref G2TIME_SPAN right);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_spot_make_invalid(ref G2SPOT spot);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_spot_is_valid(ref G2SPOT spot);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern int g2_spot_compare(ref G2SPOT left, ref G2SPOT right);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_scope_make_invalid(ref G2SCOPE scope);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_scope_is_overlap(ref G2SCOPE left, ref G2SCOPE right);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_scope_contains_spot(ref G2SCOPE scope, ref G2SPOT spot);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_scope_contains_day(ref G2SCOPE scope, ref G2TIME time);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_scope_contains_hour(ref G2SCOPE scope, ref G2TIME time);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_scope_contains_min(ref G2SCOPE scope, ref G2TIME time);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern G2SCOPE g2_scope_get_overlapped_scope(ref G2SCOPE left, ref G2SCOPE right);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern G2SCOPE g2_scope_get_merged_scope(ref G2SCOPE left, ref G2SCOPE right);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_channelset_from_array(ref G2CHANNEL_SET channelset, int[] channels, uint size);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_get_string_disconnect_reason(int reason, out G2STRING_128 str);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_get_string_disconnect_reason_service(int reason, int reason_user, out G2STRING_128 str);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_get_string_dvrns_error(int error, out G2STRING_128 str);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_get_string_service_log(ref G2GUID service, ref G2SYSTEM_LOG log, out G2STRING_128 str);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_get_string_event_type(int level1, int level2, out G2STRING_64 str);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_get_string_event_type_network(int type, out G2STRING_64 str);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_get_string_event_type_g2(int type, out G2STRING_64 str);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_get_event_info_type_from_network_event(int evt, out int level1, out int level2);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_get_event_info_type_from_g2event(int evt, out int level1, out int level2);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_event_info_is_camera(int level2);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_event_info_is_alarm(int level2);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_event_info_is_alarm_network(int level2);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_event_info_is_audio(int level2);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_event_info_is_text_in(int level2);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_event_info_is_dvr_system(int level2);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_event_info_is_ignored(int level2);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_event_info_is_off(int level2);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_event_is_disk(int type);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_event_is_gps(int type);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_event_is_text_in(int type);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_event_is_network_alarm(int type);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_event_is_user_defined(int type);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_event_is_secom(int type);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern void g2_string_encrypt_by_sha256(string text, out G2STRING_256 str);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_disk_get_free_space(string dir, out ulong free_bytes_available, out ulong total_number_of_bytes, out ulong total_number_of_free_bytes);
        #endregion

        public static string get_string_disconnect_reason(G2DISCONNECT_REASON.TYPE reason)
        {
            G2STRING_128 s;
            g2_get_string_disconnect_reason((int)reason, out s);
            return s._string;
        }
        public static string get_string_disconnect_reason(G2DISCONNECT_REASON.TYPE reason, G2SERVICE_LOGIN_FAIL_REASON.TYPE reason_user)
        {
            G2STRING_128 s;
            g2_get_string_disconnect_reason_service((int)reason, (int)reason_user, out s);
            return s._string;
        }
        public static string get_string_dvrns_error(int error)
        {
            G2STRING_128 s;
            g2_get_string_dvrns_error(error, out s);
            return s._string;
        }
        public static string get_string_service_log(ref G2GUID service, ref G2SYSTEM_LOG log)
        {
            G2STRING_128 s;
            g2_get_string_service_log(ref service, ref log, out s);
            return s._string;
        }
        public static string get_string_event_type(int level1, int level2)
        {
            G2STRING_64 s;
            g2_get_string_event_type(level1, level2, out s);
            return s._string;
        }
        public static string get_string_event_type_network(int type)
        {
            G2STRING_64 s;
            g2_get_string_event_type_network(type, out s);
            return s._string;
        }
        public static string get_string_event_type_g2(G2EVENT.TYPE type)
        {
            G2STRING_64 s;
            g2_get_string_event_type_g2((int)type, out s);
            return s._string;
        }
        public static ulong get_disk_free_space(string dir)
        {
            ulong p0 = 0UL, p1, p2;
            g2_disk_get_free_space(dir, out p0, out p1, out p2);
            return p0;
        }
    }

    public partial struct G2STRING_16
    {
        public static implicit operator G2STRING_16(string s)
        {
            G2STRING_16 ret = new G2STRING_16();
            ret._string = s;
            ret._len = (uint)Math.Min(16, s.Length);
            return ret;
        }
        public static implicit operator string(G2STRING_16 s) { return s._string; }
    }
    public partial struct G2STRING_32
    {
        public static implicit operator G2STRING_32(string s)
        {
            G2STRING_32 ret = new G2STRING_32();
            ret._string = s;
            ret._len = (uint)Math.Min(32, s.Length);
            return ret;
        }
        public static implicit operator string(G2STRING_32 s) { return s._string; }
    }
    public partial struct G2STRING_46
    {
        public static implicit operator G2STRING_46(string s)
        {
            G2STRING_46 ret = new G2STRING_46();
            ret._string = s;
            ret._len = (uint)Math.Min(46, s.Length);
            return ret;
        }
        public static implicit operator string(G2STRING_46 s) { return s._string; }
    }
    public partial struct G2STRING_64
    {
        public static implicit operator G2STRING_64(string s)
        {
            G2STRING_64 ret = new G2STRING_64();
            ret._string = s;
            ret._len = (uint)Math.Min(64, s.Length);
            return ret;
        }
        public static implicit operator string(G2STRING_64 s) { return s._string; }
    }
    public partial struct G2STRING_128
    {
        public static implicit operator G2STRING_128(string s)
        {
            G2STRING_128 ret = new G2STRING_128();
            ret._string = s;
            ret._len = (uint)Math.Min(128, s.Length);
            return ret;
        }
        public static implicit operator string(G2STRING_128 s) { return s._string; }
    }
    public partial struct G2STRING_256
    {
        public static implicit operator G2STRING_256(string s)
        {
            G2STRING_256 ret = new G2STRING_256();
            ret._string = s;
            ret._len = (uint)Math.Min(256, s.Length);
            return ret;
        }
        public static implicit operator string(G2STRING_256 s) { return s._string; }
    }
    public partial struct G2STRING_512
    {
        public static implicit operator G2STRING_512(string s)
        {
            G2STRING_512 ret = new G2STRING_512();
            ret._string = s;
            ret._len = (uint)Math.Min(512, s.Length);
            return ret;
        }
        public static implicit operator string(G2STRING_512 s) { return s._string; }
    }

    public partial struct G2TIME : ICloneable, IComparable, IComparable<G2TIME>, IEquatable<G2TIME>
    {
        public static readonly G2TIME INVALID_TIME = G2TIME.create();
        public static G2TIME create()
        {
            G2TIME ret = new G2TIME();
            g2foundation.g2_time_make_invalid(ref ret);
            return ret;
        }
        public static G2TIME create(int year, int month, int day, int hour, int minute, int second)
        {
            G2TIME ret = new G2TIME();
            return g2foundation.g2_time_from_elements(ref ret, year, month, day, hour, minute, second) ? ret : INVALID_TIME;
        }
        public static G2TIME current()
        {
            G2TIME ret = new G2TIME();
            g2foundation.g2_time_get_current(ref ret);
            return ret;
        }

        public G2TIME(int year, int month, int day, int hour, int minute, int second) { this._time = G2TIME.create(year, month, day, hour, minute, second)._time; }
        public G2TIME(int year, int month, int day, int hour, int minute) { this._time = G2TIME.create(year, month, day, hour, minute, 0)._time; }
        public G2TIME(int year, int month, int day, int hour) { this._time = G2TIME.create(year, month, day, hour, 0, 0)._time; }
        public G2TIME(int year, int month, int day) { this._time = G2TIME.create(year, month, day, 0, 0, 0)._time; }
        public G2TIME(G2TIME time) { this._time = time._time; }

        public void set(int year, int month, int day, int hour, int minute, int second) { _time = G2TIME.create(year, month, day, hour, minute, second)._time; }
        public void set(int year, int month, int day, int hour, int minute) { _time = G2TIME.create(year, month, day, hour, minute, 0)._time; }
        public void set(int year, int month, int day, int hour) { _time = G2TIME.create(year, month, day, hour, 0, 0)._time; }
        public void set(int year, int month, int day) { _time = G2TIME.create(year, month, day, 0, 0, 0)._time; }
        public void reset() { this = INVALID_TIME; }
        public bool is_same_date(G2TIME right) { return g2foundation.g2_time_is_same_date(ref this, ref right); }
        public bool is_same_hour(G2TIME right) { return g2foundation.g2_time_is_same_hour(ref this, ref right); }
        public bool is_same_minute(G2TIME right) { return g2foundation.g2_time_is_same_minute(ref this, ref right); }
        public static G2TIME_SPAN operator -(G2TIME left, G2TIME right) { return g2foundation.g2_time_subtract_time(ref left, ref right); }
        public static G2TIME operator +(G2TIME time, G2TIME_SPAN ts) { return g2foundation.g2_time_plus_span(ref time, ref ts); }
        public static G2TIME operator -(G2TIME time, G2TIME_SPAN ts) { return g2foundation.g2_time_subtract_span(ref time, ref ts); }
        public static bool operator ==(G2TIME left, G2TIME right) { return (left._time == right._time); }
        public static bool operator !=(G2TIME left, G2TIME right) { return !(left == right); }
        public static bool operator <(G2TIME left, G2TIME right) { return left._time < right._time; }
        public static bool operator >(G2TIME left, G2TIME right) { return right < left; }
        public static bool operator <=(G2TIME left, G2TIME right) { return !(right < left); }
        public static bool operator >=(G2TIME left, G2TIME right) { return !(left < right); }

        public object Clone() { return new G2TIME(this); }
        public int CompareTo(object obj) { return CompareTo((G2TIME)obj); }
        public int CompareTo(G2TIME other)
        {
            return (this._time < other._time) ? -1 :
                   (this._time > other._time) ? +1 : 0;
        }
        public bool Equals(G2TIME other) { return (this == other); }
        public override bool Equals(object obj) { return Equals((G2TIME)obj); }
        public override int GetHashCode() { return (int)_time; }
        public override string ToString()
        {
            if (valid)
            {
                return to_string_date_time();
            }
            else
            {
                int year, month, day, hour, minute, second;
                g2foundation.g2_time_to_elements(ref this, out year, out month, out day, out hour, out minute, out second);
                return string.Format("{0}-{1}-{2} {3}:{4}:{5}", year.ToString("D4"), month.ToString("D2"), day.ToString("D2"), hour.ToString("D2"), minute.ToString("D2"), second.ToString("D2"));
            }
        }
        public static implicit operator G2TIME(DateTime time) { return create(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second); }
        public static implicit operator DateTime(G2TIME time)
        {
            if (time.valid)
            {
                int year, month, day, hour, minute, second;
                g2foundation.g2_time_to_elements(ref time, out year, out month, out day, out hour, out minute, out second);
                return new DateTime(year, month, day, hour, minute, second);
            }
            else
            {
                return new DateTime();
            }
        }
        public string to_string_date() { return g2main.time_to_string_date(this); }
        public string to_string_time() { return g2main.time_to_string_time(this); }
        public string to_string_date_time() { return g2main.time_to_string_date_time(this); }
    }

    public partial struct G2TIME_SPAN : ICloneable, IComparable, IComparable<G2TIME_SPAN>, IEquatable<G2TIME_SPAN>
    {
        public G2TIME_SPAN(int hours, int minutes, int seconds)
        {
            this._days = 0;
            this._hours = hours;
            this._minutes = minutes;
            this._seconds = seconds;
        }
        public G2TIME_SPAN(int days, int hours, int minutes, int seconds)
        {
            this._days = days;
            this._hours = hours;
            this._minutes = minutes;
            this._seconds = seconds;
        }
        public G2TIME_SPAN(G2TIME_SPAN right)
        {
            this._days = right._days;
            this._hours = right._hours;
            this._minutes = right._minutes;
            this._seconds = right._seconds;
        }
        public G2TIME_SPAN(int total_seconds)
        {
            this._days = total_seconds / (60 * 60 * 24);
            this._hours = (total_seconds / (60 * 60)) % 24;
            this._minutes = (total_seconds / 60) % 60;
            this._seconds = total_seconds % 60;
        }

        public void set(int days, int hours, int minutes, int seconds)
        {
            _days = days;
            _hours = hours;
            _minutes = minutes;
            _seconds = seconds;
        }
        public void set(int total_seconds)
        {
            _days = total_seconds / (60 * 60 * 24);
            _hours = (total_seconds / (60 * 60)) % 24;
            _minutes = (total_seconds / 60) % 60;
            _seconds = total_seconds % 60;
        }
        public void set(G2TIME_SPAN ts)
        {
            _days = ts._days;
            _hours = ts._hours;
            _minutes = ts._minutes;
            _seconds = ts._seconds;
        }

        public static G2TIME_SPAN operator +(G2TIME_SPAN left, G2TIME_SPAN right) { return new G2TIME_SPAN(left.total_seconds + right.total_seconds); }
        public static G2TIME_SPAN operator -(G2TIME_SPAN left, G2TIME_SPAN right) { return new G2TIME_SPAN(left.total_seconds - right.total_seconds); }
        public static bool operator ==(G2TIME_SPAN left, G2TIME_SPAN right)
        {
            return (left._days == right._days &&
                    left._hours == right._hours &&
                    left._minutes == right._minutes &&
                    left._seconds == right._seconds);
        }
        public static bool operator !=(G2TIME_SPAN left, G2TIME_SPAN right) { return !(left == right); }
        public static bool operator <(G2TIME_SPAN left, G2TIME_SPAN right) { return left.CompareTo(right) < 0; }
        public static bool operator >(G2TIME_SPAN left, G2TIME_SPAN right) { return right < left; }
        public static bool operator <=(G2TIME_SPAN left, G2TIME_SPAN right) { return !(right < left); }
        public static bool operator >=(G2TIME_SPAN left, G2TIME_SPAN right) { return !(left < right); }
        public object Clone() { return new G2TIME_SPAN(this); }
        public int CompareTo(object obj) { return CompareTo((G2TIME_SPAN)obj); }
        public int CompareTo(G2TIME_SPAN other) { return g2foundation.g2_time_span_compare(ref this, ref other); }
        public bool Equals(G2TIME_SPAN other) { return (this == other); }
        public override bool Equals(object obj) { return Equals((G2TIME_SPAN)obj); }
        public override int GetHashCode() { return total_seconds; }
        public override string ToString() { return string.Format("{0}-{1}:{2}:{3} (total {4} seconds)", _days, _hours, _minutes, _seconds, total_seconds); }
        public static implicit operator G2TIME_SPAN(TimeSpan ts) { return new G2TIME_SPAN(ts.Days, ts.Hours, ts.Minutes, ts.Seconds); }
        public static implicit operator TimeSpan(G2TIME_SPAN ts) { return new TimeSpan(ts._days, ts._hours, ts._minutes, ts._seconds); }
    }

    public partial struct G2SPOT : ICloneable, IComparable, IComparable<G2SPOT>, IEquatable<G2SPOT>
    {
        public const uint INVALID_SEGMENT_ID = 0xFFFFFFFF;
        public const uint INVALID_SPOT_TICK = 0xFFFFFFFF;
        public static readonly G2SPOT INVALID_SPOT = G2SPOT.create();
        public static G2SPOT create()
        {
            G2SPOT ret = new G2SPOT();
            ret._segment = INVALID_SEGMENT_ID;
            ret._tick = 0;
            ret._time = G2TIME.INVALID_TIME;
            return ret;
        }

        public G2SPOT(uint segment, G2TIME time, uint tick)
        {
            this._segment = segment;
            this._tick = tick;
            this._time = time;
        }
        public G2SPOT(uint segment, G2TIME time)
        {
            this._segment = segment;
            this._tick = 0;
            this._time = time;
        }
        public G2SPOT(G2SPOT other)
        {
            this._segment = other._segment;
            this._time = other._time;
            this._tick = other._tick;
        }

        public void set(uint segment, G2TIME time, uint tick)
        {
            this._segment = segment;
            this._tick = tick;
            this._time = time;
        }
        public void set(uint segment, G2TIME time)
        {
            this._segment = segment;
            this._tick = 0;
            this._time = time;
        }
        public void reset() { this = INVALID_SPOT; }
        public static bool operator ==(G2SPOT left, G2SPOT right)
        {
            return (left._segment == right._segment &&
                    left._tick == right._tick &&
                    left._time == right._time);
        }
        public static bool operator !=(G2SPOT left, G2SPOT right) { return !(left == right); }
        public static bool operator <(G2SPOT left, G2SPOT right) { return g2foundation.g2_spot_compare(ref left, ref right) < 0; }
        public static bool operator >(G2SPOT left, G2SPOT right) { return right < left; }
        public static bool operator <=(G2SPOT left, G2SPOT right) { return !(right < left); }
        public static bool operator >=(G2SPOT left, G2SPOT right) { return !(left < right); }
        public object Clone() { return new G2SPOT(this); }
        public int CompareTo(object obj) { return CompareTo((G2SPOT)obj); }
        public int CompareTo(G2SPOT other) { return g2foundation.g2_spot_compare(ref this, ref other); }
        public bool Equals(G2SPOT other) { return (this == other); }
        public override bool Equals(object other) { return (this == (G2SPOT)other); }
        public override int GetHashCode() { return base.GetHashCode(); }
        public override string ToString() { return string.Format("({0}) {1} {2}", _segment, _time.ToString(), _tick); }
    }

    public partial struct G2SCOPE : ICloneable, IEquatable<G2SCOPE>
    {
        public static readonly G2SCOPE INVALID_SCOPE = G2SCOPE.create();
        public static G2SCOPE create()
        {
            G2SCOPE ret = new G2SCOPE();
            g2foundation.g2_scope_make_invalid(ref ret);
            return ret;
        }

        public G2SCOPE(G2SPOT begin, G2SPOT end)
        {
            this._begin = begin;
            this._end = end;
        }
        public G2SCOPE(G2SPOT begin)
        {
            this._begin = begin;
            this._end = G2SPOT.INVALID_SPOT;
        }
        public G2SCOPE(G2SCOPE other)
        {
            this._begin = other._begin;
            this._end = other._end;
        }

        public bool contains(G2SPOT spot) { return g2foundation.g2_scope_contains_spot(ref this, ref spot); }
        public bool contains_day(G2TIME time) { return g2foundation.g2_scope_contains_day(ref this, ref time); }
        public bool contains_hour(G2TIME time) { return g2foundation.g2_scope_contains_hour(ref this, ref time); }
        public bool contains_min(G2TIME time) { return g2foundation.g2_scope_contains_min(ref this, ref time); }
        public bool is_overlap(G2SCOPE scope) { return g2foundation.g2_scope_is_overlap(ref this, ref scope); }
        public G2SCOPE overlapped_scope(G2SCOPE scope) { return g2foundation.g2_scope_get_overlapped_scope(ref this, ref scope); }
        public G2SCOPE merged_scope(G2SCOPE scope) { return g2foundation.g2_scope_get_merged_scope(ref this, ref scope); }

        public static bool operator ==(G2SCOPE left, G2SCOPE right)
        {
            return (left._begin == right._begin &&
                    left._end == right._end);
        }
        public static bool operator !=(G2SCOPE left, G2SCOPE right) { return !(left == right); }
        public object Clone() { return new G2SCOPE(this); }
        public bool Equals(G2SCOPE other) { return (this == other); }
        public override bool Equals(object other) { return (this == (G2SCOPE)other); }
        public override int GetHashCode() { return base.GetHashCode(); }
        public override string ToString() { return string.Format("{0} ~ {1}", _begin.ToString(), _end.ToString()); }
    }

    public partial struct G2CODEC_INFO_CONTENT_ANALYTICS_FACE_DETECTION
    {
        public void reset()
        {
            _has = false;
            _rects = null;
        }
        public bool from(ref PARAM_ARCHIVED param)
        {
            if (param._len > 0)
            {
                G2SIZE res = (G2SIZE)Marshal.PtrToStructure(param._ptr, typeof(G2SIZE));
                IntPtr ptr = new IntPtr(param._ptr.ToInt64() + Marshal.SizeOf(typeof(G2SIZE)));
                int len = Marshal.ReadInt32(ptr);
                ptr = new IntPtr(ptr.ToInt64() + 4);

                _resolution = new Size(res.cx, res.cy);

                if (len > 0)
                {
                    _rects = new RectangleF[len];
                    _has = true;

                    for (int i = 0; i < len; ++i)
                    {
                        ptr = new IntPtr(ptr.ToInt64() + i * Marshal.SizeOf(typeof(G2CODEC_INFO_CONTENT_ANALYTICS_FACE_DETECTION.COORDINATE_TYPE)));
                        G2CODEC_INFO_CONTENT_ANALYTICS_FACE_DETECTION.COORDINATE_TYPE r = (G2CODEC_INFO_CONTENT_ANALYTICS_FACE_DETECTION.COORDINATE_TYPE)Marshal.PtrToStructure(ptr, typeof(G2CODEC_INFO_CONTENT_ANALYTICS_FACE_DETECTION.COORDINATE_TYPE));
                        _rects[i] = r.to();
                    }
                    return true;
                }
                else
                {
                    _has = false;
                }
            }
            else
            {
                _has = false;
            }
            return false;
        }
    }

    public partial struct G2CODEC_INFO_SI_ELEVATOR_STATUS
    {
        public void reset()
        {
            _has = false;
        }
        public bool from(ref PARAM_ARCHIVED param)
        {
            if (param._len > 0)
            {
                this = (G2CODEC_INFO_SI_ELEVATOR_STATUS)Marshal.PtrToStructure(param._ptr, typeof(G2CODEC_INFO_SI_ELEVATOR_STATUS));
                _has = true;
                return true;
            }
            else
            {
                _has = false;
            }
            return false;
        }
    }

    public partial struct G2FRAME
    {
        public override string ToString() { return string.Format("{0} ch {1}({2}) {3} flag 0x{4:X02} {5}", spot, channel, stream_id, type, _index._flag, record_type); }
    }

    public partial struct G2CHANNEL_SET
    {
        public static readonly G2CHANNEL_SET G2CHANNEL_SET_ALL = new G2CHANNEL_SET(G2CHANNEL_SET.G2_CHANNEL_ALL);

        public G2CHANNEL_SET(params int[] chs)
        {
            _channels = new int[(int)CONST.MAX_CHANNEL_COUNT];
            _len = (uint)Math.Min(chs.Length, _channels.Length);
            Array.Copy(chs, _channels, _len);
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            for (uint i = 0; i != _len; )
            {
                sb.Append(_channels[i]);

                if (++i != _len)
                {
                    sb.Append(", ");
                }
            }
            sb.Append("}");
            return sb.ToString();
        }
    }

    public partial struct G2CHANNEL_STREAM : ICloneable, IComparable, IComparable<G2CHANNEL_STREAM>, IEquatable<G2CHANNEL_STREAM>
    {
        public static readonly G2CHANNEL_STREAM INVALID_CHANNEL_STREAM = G2CHANNEL_STREAM.create();
        public static G2CHANNEL_STREAM create()
        {
            G2CHANNEL_STREAM ret = new G2CHANNEL_STREAM(-1, -1);
            return ret;
        }

        public G2CHANNEL_STREAM(int channel) { _channel = channel; _stream = 0; }
        public G2CHANNEL_STREAM(int channel, int stream) { _channel = channel; _stream = stream; }
        public G2CHANNEL_STREAM(G2CHANNEL_STREAM right) { _channel = right._channel; _stream = right._stream; }
        public static bool operator ==(G2CHANNEL_STREAM left, G2CHANNEL_STREAM right) { return (left._channel == right._channel) && (left._stream == right._stream); }
        public static bool operator !=(G2CHANNEL_STREAM left, G2CHANNEL_STREAM right) { return !(left == right); }
        public static bool operator <(G2CHANNEL_STREAM left, G2CHANNEL_STREAM right) { return (left._channel == right._channel) ? (left._stream < right._stream) : (left._channel < right._channel); }
        public static bool operator >(G2CHANNEL_STREAM left, G2CHANNEL_STREAM right) { return right < left; }
        public static bool operator <=(G2CHANNEL_STREAM left, G2CHANNEL_STREAM right) { return !(right < left); }
        public static bool operator >=(G2CHANNEL_STREAM left, G2CHANNEL_STREAM right) { return !(left < right); }
        public object Clone() { return new G2CHANNEL_STREAM(this); }
        public int CompareTo(object obj) { return CompareTo((G2CHANNEL_STREAM)obj); }
        public int CompareTo(G2CHANNEL_STREAM other)
        {
            return (this < other) ? -1 :
                   (this > other) ? +1 : 0;
        }
        public bool Equals(G2CHANNEL_STREAM other) { return (this == other); }
        public override bool Equals(object obj) { return Equals((G2CHANNEL_STREAM)obj); }
        public override int GetHashCode() { return _channel * (int)CONST.STREAM_LIMIT + _stream; }
        public override string ToString()
        {
            return string.Format("{0}({1})", _channel, _stream);
        }
    }

    public partial struct G2CHANNEL_STREAM_SET
    {
        public G2CHANNEL_STREAM_SET(g2channel_stream_set chs)
        {
            _streams = new G2CHANNEL_STREAM[(int)CONST.MAX_STREAM_COUNT];
            _len = (uint)Math.Min(chs.size(), _streams.Length);

            uint i = 0;
            foreach (G2CHANNEL_STREAM ch in chs)
            {
                _streams[i] = ch;
                if (++i >= _len)
                {
                    break;
                }
            }
        }
    }

    public partial struct G2MAC_ADDRESS
    {
        public override string ToString()
        {
            return string.Format("{0:X2}-{1:X2}-{2:X2}-{3:X2}-{4:X2}-{5:X2}", _mac[0], _mac[1], _mac[2], _mac[3], _mac[4], _mac[5]);
        }
    }

    public partial struct G2DEVICE_STATUS
    {
        public CAMERA camera(int channel) { return (channel >= 0 && channel < _camera.Length) ? (CAMERA)_camera[channel] : (CAMERA)COMMON.NOT_VALUE; }
        public PTZ ptz(int channel) { return (channel >= 0 && channel < _ptz.Length) ? (PTZ)_ptz[channel] : (PTZ)COMMON.NOT_VALUE; }
        public COVERT convert(int channel) { return (channel >= 0 && channel < _covert.Length) ? (COVERT)_covert[channel] : (COVERT)COMMON.NOT_VALUE; }
        public AUDIO_IN audio_in(int channel) { return (channel >= 0 && channel < _audio_in.Length) ? (AUDIO_IN)_audio_in[channel] : (AUDIO_IN)COMMON.NOT_VALUE; }
        public COMMON audio_out(int channel) { return (channel >= 0 && channel < _audio_out.Length) ? (COMMON)_audio_out[channel] : COMMON.NOT_VALUE; }
        public ALARM_IN alarm_in(int channel) { return (channel >= 0 && channel < _alarm_in.Length) ? (ALARM_IN)_alarm_in[channel] : (ALARM_IN)COMMON.NOT_VALUE; }
        public COMMON alarm_out(int channel) { return (channel >= 0 && channel < _alarm_out.Length) ? (COMMON)_alarm_out[channel] : COMMON.NOT_VALUE; }
        public ALARM_IN alarm_in_network(int channel) { return (channel >= 0 && channel < _alarm_in_network.Length) ? (ALARM_IN)_alarm_in_network[channel] : (ALARM_IN)COMMON.NOT_VALUE; }
        public COMMON text_in(int channel) { return (channel >= 0 && channel < _text_in.Length) ? (COMMON)_text_in[channel] : COMMON.NOT_VALUE; }
        public RECORDING recording(int channel) { return (channel >= 0 && channel < _recording.Length) ? (RECORDING)_recording[channel] : (RECORDING)COMMON.NOT_VALUE; }
        public uint stream_remote(int channel) { return (channel >= 0 && channel < _stream_remote.Length) ? _stream_remote[channel] : 0; }
        public PTZ_FUNCTION ptz_function(int channel) { return (channel >= 0 && channel < _ptz_function.Length) ? (PTZ_FUNCTION)_ptz_function[channel] : 0; }
    }

    public partial struct G2EVENT_INFO
    {
        public static string string_event_type(TYPE_LEVEL1 level1, TYPE_LEVEL2 level2) { return g2foundation.get_string_event_type((int)level1, (int)level2); }
        public static bool is_event_camera(TYPE_LEVEL2 level2) { return g2foundation.g2_event_info_is_camera((int)level2); }
        public static bool is_event_alarm(TYPE_LEVEL2 level2) { return g2foundation.g2_event_info_is_alarm((int)level2); }
        public static bool is_event_alarm_network(TYPE_LEVEL2 level2) { return g2foundation.g2_event_info_is_alarm_network((int)level2); }
        public static bool is_event_audio(TYPE_LEVEL2 level2) { return g2foundation.g2_event_info_is_audio((int)level2); }
        public static bool is_event_text_in(TYPE_LEVEL2 level2) { return g2foundation.g2_event_info_is_text_in((int)level2); }
        public static bool is_event_dvr_system(TYPE_LEVEL2 level2) { return g2foundation.g2_event_info_is_dvr_system((int)level2); }
        public static bool is_event_ignored(TYPE_LEVEL2 level2) { return g2foundation.g2_event_info_is_ignored((int)level2); }
        public static bool is_event_off(TYPE_LEVEL2 level2) { return g2foundation.g2_event_info_is_off((int)level2); }

        public string string_event_type() { return g2foundation.get_string_event_type(_level1, _level2); }
        public bool is_event_camera() { return g2foundation.g2_event_info_is_camera(_level2); }
        public bool is_event_alarm() { return g2foundation.g2_event_info_is_alarm(_level2); }
        public bool is_event_alarm_network() { return g2foundation.g2_event_info_is_alarm_network(_level2); }
        public bool is_event_audio() { return g2foundation.g2_event_info_is_audio(_level2); }
        public bool is_event_text_in() { return g2foundation.g2_event_info_is_text_in(_level2); }
        public bool is_event_dvr_system() { return g2foundation.g2_event_info_is_dvr_system(_level2); }
        public bool is_event_ignored() { return g2foundation.g2_event_info_is_ignored(_level2); }
        public bool is_event_off() { return g2foundation.g2_event_info_is_off(_level2); }

        public partial struct G2EVENT_RAS
        {
            public g2channel_set associated_channel()
            {
                g2channel_set res = new g2channel_set();
                if (_g2)
                    res.from(ref _g2event._associated_channels);
                else
                    res.from(_cameras);
                return res;
            }
        }
    }

    public partial struct G2EVENT
    {
        public static string string_event_type(G2EVENT.TYPE type) { return g2foundation.get_string_event_type_g2(type); }
        public static G2EVENT_INFO.TYPE_LEVEL2 to_G2EVENT_INFO_LEVEL2(G2EVENT.TYPE type)
        {
            int lv1 = 0, lv2 = 0;
            g2foundation.g2_get_event_info_type_from_g2event((int)type, out lv1, out lv2);
            return (G2EVENT_INFO.TYPE_LEVEL2)lv2;
        }

        public string string_event_type() { return g2foundation.get_string_event_type_g2(type); }
        public G2EVENT_INFO.TYPE_LEVEL2 to_G2EVENT_INFO_LEVEL2() { return G2EVENT.to_G2EVENT_INFO_LEVEL2(type); }
        public bool is_event_disk() { return g2foundation.g2_event_is_disk(_type); }
        public bool is_event_gps() { return g2foundation.g2_event_is_gps(_type); }
        public bool is_event_text_in() { return g2foundation.g2_event_is_text_in(_type); }
        public bool is_event_secom() { return g2foundation.g2_event_is_secom(_type); }
        public bool is_event_network_alarm() { return g2foundation.g2_event_is_network_alarm(_type); }
        public bool is_event_user_defined() { return g2foundation.g2_event_is_user_defined(_type); }
    }

    public partial struct G2PLAYER
    {
        public static bool is_command_play(G2PLAYER.COMMAND_AND_SPEED command) { return (COMMAND_AND_SPEED.NONE != command && command < COMMAND_AND_SPEED.COMMAND_MOVE); }
        public static bool is_command_play_forward(G2PLAYER.COMMAND_AND_SPEED command)
        {
            return (command == COMMAND_AND_SPEED.PLAY_SLOW ||
                    command == COMMAND_AND_SPEED.PLAY_NORMAL ||
                    command == COMMAND_AND_SPEED.PLAY_FAST ||
                    command == COMMAND_AND_SPEED.PLAY_FASTER ||
                    command == COMMAND_AND_SPEED.PLAY_FASTEST);
        }
        public static bool is_command_play_backward(G2PLAYER.COMMAND_AND_SPEED command)
        {
            return (command == COMMAND_AND_SPEED.BACK_SLOW ||
                    command == COMMAND_AND_SPEED.BACK_NORMAL ||
                    command == COMMAND_AND_SPEED.BACK_FAST ||
                    command == COMMAND_AND_SPEED.BACK_FASTER ||
                    command == COMMAND_AND_SPEED.BACK_FASTEST);
        }
        public static bool is_command_play_key(G2PLAYER.COMMAND_AND_SPEED command)
        {
            return (command == COMMAND_AND_SPEED.PLAY_FAST ||
                    command == COMMAND_AND_SPEED.PLAY_FASTER ||
                    command == COMMAND_AND_SPEED.PLAY_FASTEST ||
                    command == COMMAND_AND_SPEED.BACK_FAST ||
                    command == COMMAND_AND_SPEED.BACK_FASTER ||
                    command == COMMAND_AND_SPEED.BACK_FASTEST);
        }
        public static bool is_command_move(G2PLAYER.COMMAND_AND_SPEED command) { return (COMMAND_AND_SPEED.COMMAND_MOVE < command && command < COMMAND_AND_SPEED.COMMAND_SEARCH); }
        public static bool is_command_search(G2PLAYER.COMMAND_AND_SPEED command) { return (COMMAND_AND_SPEED.COMMAND_SEARCH < command && command < COMMAND_AND_SPEED.COMMAND_SPECIAL); }

    }

    public partial struct G2RECORD_TIME_INFO
    {
        public class comparer_spot : IComparer<G2RECORD_TIME_INFO>
        {
            public comparer_spot() { this.ascending = true; }
            public comparer_spot(bool ascending) { this.ascending = ascending; }
            public int Compare(G2RECORD_TIME_INFO left, G2RECORD_TIME_INFO right)
            {
                int ret = left._spot.CompareTo(right._spot);
                return (ascending) ? ret : -ret;
            }
            public bool ascending;
        }
    }

    public partial struct G2PROBE_SESSION_PROFILE
    {
        public override string ToString()
        {
            string s = "";
            if (_SSL_state >= 0) s += string.Format("SSL: {0}", SSL_state);
            if (_FEN_connection > 0) s += (s.Length == 0) ? string.Format("FEN: {0}", FEN_connection) : string.Format(" / FEN: {0}", FEN_connection);
            if (s.Length != 0) s.Replace('_', ' ');

            string bps = (_bps > 1000) ? string.Format("bps: {0:F2} Mbps", (float)_bps / 1000.0F) : string.Format("bps: {0:D3} Kbps", _bps);
            if (_bps_audio > 0)
            {
                bps += string.Format(" (audio {0:D2} Kbps)", _bps_audio);
            }
            string p = s.Length != 0 ? s + " / " : "";
            return p + string.Format("channel: {0} / ips: {1:00.0} / {2}", _channel, _ips, bps);
        }
    }
}
