using System;
using System.Collections.Generic;
using System.IO;
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

    public class g2guid
    {
        #region GDK DLL Import
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern G2GUID g2_guid_from_string(string right);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_guid_to_string(ref G2GUID guid, out G2STRING_46 right);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern uint g2_guid_get_hash_value(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_guid_make_invalid(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_less(ref G2GUID left, ref G2GUID right);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_equal(ref G2GUID left, ref G2GUID right);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_valid(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_user(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_user_group(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_user_group_administrator(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_service(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_service_aministration(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_service_federation(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_service_recording(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_service_recording_failover(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_service_recording_redundant(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_service_backup(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_service_streaming(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_service_analytics(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_service_monitoring(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_service_monitoring_failover(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_service_videowall(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_service_videowall_failover(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_device(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_device_root(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_device_parent(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_device_leaf(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_device_group(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_device_camera(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_device_stream(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_device_alarm_in(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_device_alarm_in_network(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_device_alarm_out(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_device_text_in(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_device_text_in_network(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_device_sdcard(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_device_audio_in(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_device_audio_out(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_device_hybrided(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_device_hybrid_parent(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_device_hybrid_leaf(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_device_hybrid_camera(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_device_violet(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_device_violet_agent(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_device_violet_monitor(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_device_violet_monitor_primary(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_device_network_keyboard(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_layout(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_sequence(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_sequence_camera(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_sequence_layout(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_map(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_map_device(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_map_shortcut(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_map_camera(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_map_alarm_in(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_map_alarm_in_network(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_map_alarm_out(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_map_audio_in(ref G2GUID guid);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_guid_is_map_path_sequence(ref G2GUID guid);
        #endregion
    }

    public partial struct G2GUID : ICloneable, IComparable, IComparable<G2GUID>, IEquatable<G2GUID>, IEqualityComparer<G2GUID>
    {
        public partial struct GUID_TYPE
        {
            public void serialize(BinaryWriter s)
            {
                s.Write(_data1);
                s.Write(_data2);
                s.Write(_data3);
                _data4.serialize(s);
            }
        }

        public static readonly G2GUID INVALID_GUID = G2GUID.create();
        public static G2GUID create()
        {
            G2GUID ret = new G2GUID();
            g2guid.g2_guid_make_invalid(ref ret);
            return ret;
        }
        public G2GUID(G2GUID right) { this._GUID = right._GUID; this._type = right._type; }
        public void from_string(string right) { this = g2guid.g2_guid_from_string(right); }
        public void serialize(BinaryWriter s)
        {
            _GUID.serialize(s);
            s.Write(_type);
        }
        public object Clone() { return new G2GUID(this); }
        public int CompareTo(object obj) { return CompareTo((G2GUID)obj); }
        public int CompareTo(G2GUID other)
        {
            return (this < other) ? -1 :
                   (this > other) ? +1 : 0;
        }
        public bool Equals(G2GUID other) { return (this == other); }
        public bool Equals(G2GUID left, G2GUID right) { return (left == right); }
        public int GetHashCode(G2GUID obj) { return (int)g2guid.g2_guid_get_hash_value(ref obj); }
        public override bool Equals(object obj) { return Equals((G2GUID)obj); }
        public override int GetHashCode() { return (int)g2guid.g2_guid_get_hash_value(ref this); }
        public override string ToString()
        {
            G2STRING_46 o;
            g2guid.g2_guid_to_string(ref this, out o);
            return o;
        }
        public static bool operator ==(G2GUID left, G2GUID right) { return g2guid.g2_guid_is_equal(ref left, ref right); }
        public static bool operator !=(G2GUID left, G2GUID right) { return !(left == right); }
        public static bool operator <(G2GUID left, G2GUID right) { return g2guid.g2_guid_is_less(ref left, ref right); }
        public static bool operator >(G2GUID left, G2GUID right) { return right < left; }
        public static bool operator <=(G2GUID left, G2GUID right) { return !(right < left); }
        public static bool operator >=(G2GUID left, G2GUID right) { return !(left < right); }

        public bool valid { get { return g2guid.g2_guid_is_valid(ref this); } }

        /// <summary>
        /// @ is_user
        /// <para>This method checks whether unique id type is user unique id.</para>
        /// </summary>
        /// <returns>If the type is user unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_user { get { return g2guid.g2_guid_is_user(ref this); } }
        /// <summary>
        /// @ is_user_group
        /// <para>This method checks whether unique id type is user group unique id.</para>
        /// </summary>
        /// <returns>If the type is user group unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_user_group { get { return g2guid.g2_guid_is_user_group(ref this); } }
        /// <summary>
        /// @ is_user_gorup_administrator
        /// <para>This method checks whether unique id type is user (administrator) group unique id.</para>
        /// </summary>
        /// <returns>If the type is user (administrator) group unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_user_gorup_administrator { get { return g2guid.g2_guid_is_user_group_administrator(ref this); } }
        /// <summary>
        /// @ is_service
        /// <para>This method checks whether unique id type is service unique id.</para>
        /// </summary>
        /// <returns>If the type is service unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_service { get { return g2guid.g2_guid_is_service(ref this); } }
        /// <summary>
        /// @ is_service_administration
        /// <para>This method checks whether unique id type is administration service unique id.</para>
        /// </summary>
        /// <returns>If the type is administration service unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_service_administration { get { return g2guid.g2_guid_is_service_aministration(ref this); } }
        /// <summary>
        /// @ is_service_federation
        /// <para>This method checks whether unique id type is federation service unique id.</para>
        /// </summary>
        /// <returns>If the type is administration service unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_service_federation { get { return g2guid.g2_guid_is_service_federation(ref this); } } 
        /// <summary>
        /// @ is_service_recording
        /// <para>This method checks whether unique id type is recording service unique id.</para>
        /// </summary>
        /// <returns>If the type is recording service unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_service_recording { get { return g2guid.g2_guid_is_service_recording(ref this); } }
        /// <summary>
        /// @ is_service_recording_failover
        /// <para>This method checks whether unique id type is recording failover service unique id.</para>
        /// </summary>
        /// <returns>If the type is recording failover service unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_service_recording_failover { get { return g2guid.g2_guid_is_service_recording_failover(ref this); } }
        /// <summary>
        /// @ is_service_recording_redeundant
        /// <para>This method checks whether unique id type is redundant recording service unique id.</para>
        /// </summary>
        /// <returns>If the type is redundant recording service unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_service_recording_redeundant { get { return g2guid.g2_guid_is_service_recording_redundant(ref this); } }
        /// <summary>
        /// @ is_service_backup
        /// <para>This method checks whether unique id type is backup service unique id.</para>
        /// </summary>
        /// <returns>If the type is backup service unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_service_backup { get { return g2guid.g2_guid_is_service_backup(ref this); } }
        /// <summary>
        /// @ is_service_streaming
        /// <para>This method checks whether unique id type is streaming service unique id.</para>
        /// </summary>
        /// <returns>If the type is streaming service unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_service_streaming { get { return g2guid.g2_guid_is_service_streaming(ref this); } }
        /// <summary>
        /// @ is_service_analytics
        /// <para>This method checks whether unique id type is video analytics service unique id.</para>
        /// </summary>
        /// <returns>If the type is video analytics service unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_service_analytics { get { return g2guid.g2_guid_is_service_analytics(ref this); } }
        /// <summary>
        /// @ is_service_monitoring
        /// <para>This method checks whether unique id type is monitoring service unique id.</para>
        /// </summary>
        /// <returns>If the type is monitoring service unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_service_monitoring { get { return g2guid.g2_guid_is_service_monitoring(ref this); } }
        /// <summary>
        /// @ is_service_monitoring_failover
        /// <para>This method checks whether unique id type is monitoring failover service unique id.</para>
        /// </summary>
        /// <returns>If the type is monitoring service unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_service_monitoring_failover { get { return g2guid.g2_guid_is_service_monitoring_failover(ref this); } } 
        /// <summary>
        /// @ is_service_videowall
        /// <para>This method checks whether unique id type is videowall service unique id.</para>
        /// </summary>
        /// <returns>If the type is videowall service unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_service_videowall { get { return g2guid.g2_guid_is_service_videowall(ref this); } }
        /// <summary>
        /// @ is_service_videowall_failover
        /// <para>This method checks whether unique id type is videowall failover service unique id.</para>
        /// </summary>
        /// <returns>If the type is videowall failover service unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_service_videowall_failover { get { return g2guid.g2_guid_is_service_videowall_failover(ref this); } }
        /// <summary>
        /// @ is_device
        /// <para>This method checks whether unique id type is device unique id.</para>
        /// </summary>
        /// <returns>If the type is device unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_device { get { return g2guid.g2_guid_is_device(ref this); } }
        /// <summary>
        /// @ is_device_root
        /// <para>This method checks whether unique id type is root device unique id.</para>
        /// </summary>
        /// <returns>If the type is root device unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_device_root { get { return g2guid.g2_guid_is_device_root(ref this); } }
        /// <summary>
        /// @ is_device_parent
        /// <para>This method checks whether unique id type is parent device unique id.</para>
        /// </summary>
        /// <returns>If the type is parent device unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_device_parent { get { return g2guid.g2_guid_is_device_parent(ref this); } }
        /// <summary>
        /// @ is_device_leaf
        /// <para>This method checks whether unique id type is leaf device unique id.</para>
        /// </summary>
        /// <returns>If the type is leaf device unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_device_leaf { get { return g2guid.g2_guid_is_device_leaf(ref this); } }
        /// <summary>
        /// @ is_device_group 
        /// <para>This method checks whether unique id type is device group unique id.</para>
        /// </summary>
        /// <returns>If the type is device group unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_device_group { get { return g2guid.g2_guid_is_device_group(ref this); } }
        /// <summary>
        /// @ is_device_camera
        /// <para>This method checks whether unique id type is camera unique id.</para>
        /// </summary>
        /// <returns>If the type is camera unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_device_camera { get { return g2guid.g2_guid_is_device_camera(ref this); } }
        /// <summary>
        /// @ is_device_stream
        /// <para>This method checks whether unique id type is streaming device unique id.</para>
        /// </summary>
        /// <returns>If the type is camera unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_device_stream { get { return g2guid.g2_guid_is_device_stream(ref this); } } 
        /// <summary>
        /// @ is_device_alarm_in
        /// <para>This method checks whether unique id type is alarm-in device unique id.</para>
        /// </summary>
        /// <returns>If the type is alarm-in device unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_device_alarm_in { get { return g2guid.g2_guid_is_device_alarm_in(ref this); } }
        /// <summary>
        /// @ is_device_alarm_in_network
        /// <para>This method checks whether unique id type is network alarm-in device unique id.</para>
        /// </summary>
        /// <returns>If the type is network alarm-in device unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_device_alarm_in_network { get { return g2guid.g2_guid_is_device_alarm_in_network(ref this); } }
        /// <summary>
        /// @ is_device_alarm_out
        /// <para>This method checks whether unique id type is alarm-out device unique id.</para>
        /// </summary>
        /// <returns>If the type is alarm-out device unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_device_alarm_out { get { return g2guid.g2_guid_is_device_alarm_out(ref this); } }
        /// <summary>
        /// @ is_device_text_in
        /// <para>This method checks whether unique id type is text-in device unique id.</para>
        /// </summary>
        /// <returns>If the type is text-in device unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_device_text_in { get { return g2guid.g2_guid_is_device_text_in(ref this); } }
        /// <summary>
        /// @ is_device_text_in_network
        /// <para>This method checks whether unique id type is network text-in device unique id.</para>
        /// </summary>
        /// <returns>If the type is network text-in device unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_device_text_in_network { get { return g2guid.g2_guid_is_device_text_in_network(ref this); } }
        /// <summary>
        /// @ is_device_sdcard
        /// <para>This method checks whether unique id type is SD card device unique id.</para>
        /// </summary>
        /// <returns>If the type is SD card device unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_device_sdcard { get { return g2guid.g2_guid_is_device_sdcard(ref this); } }
        /// <summary>
        /// @ is_device_audio_in
        /// <para>This method checks whether unique id type is audio-in device unique id.</para>
        /// </summary>
        /// <returns>If the type is audio-in device unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_device_audio_in { get { return g2guid.g2_guid_is_device_audio_in(ref this); } }
        /// <summary>
        /// @ is_device_audio_out
        /// <para>This method checks whether unique id type is audio-out device unique id.</para>
        /// </summary>
        /// <returns>If the type is audio-out device unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_device_audio_out { get { return g2guid.g2_guid_is_device_audio_out(ref this); } }
        /// <summary>
        /// @ is_device_hybrided
        /// <para>This method checks whether unique id type is hybrid DVR device unique id.</para>
        /// </summary>
        /// <returns>If the type is hybrid DVR device unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_device_hybrided { get { return g2guid.g2_guid_is_device_hybrided(ref this); } }
        /// <summary>
        /// @ is_device_hybrid_parent
        /// <para>This method checks whether unique id type is hybrid DVR network device unique id.</para>
        /// </summary>
        /// <returns>If the type is hybrid DVR network device unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_device_hybrid_parent { get { return g2guid.g2_guid_is_device_hybrid_parent(ref this); } }
        /// <summary>
        /// @ is_device_hybrid_leaf
        /// <para>This method checks whether unique id type is hybrid DVR leaf device unique id.</para>
        /// </summary>
        /// <returns>If the type is hybrid DVR leaf device unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_device_hybrid_leaf { get { return g2guid.g2_guid_is_device_hybrid_leaf(ref this); } }
        /// <summary>
        /// @ is_device_hybrid_camera
        /// <para>This method checks whether unique id type is hybrid DVR network camera unique id.</para>
        /// </summary>
        /// <returns>If the type is hybrid DVR network camera unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_device_hybrid_camera { get { return g2guid.g2_guid_is_device_hybrid_camera(ref this); } }
        /// <summary>
        /// @ is_device_violet
        /// <para>This method checks whether unique id type is videowall unique id.</para>
        /// </summary>
        /// <returns>If the type is videowall unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_device_violet { get { return g2guid.g2_guid_is_device_violet(ref this); } }
        /// <summary>
        /// @ is_device_violet_agent
        /// <para>This method checks whether unique id type is videowall agent unique id.</para>
        /// </summary>
        /// <returns>If the type is videowall agent unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_device_violet_agent { get { return g2guid.g2_guid_is_device_violet_agent(ref this); } }
        /// <summary>
        /// @ is_device_violet_monitor
        /// <para>This method checks whether unique id type is videowall monitor unique id.</para>
        /// </summary>
        /// <returns>If the type is videowall monitor unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_device_violet_monitor { get { return g2guid.g2_guid_is_device_violet_monitor(ref this); } }
        /// <summary>
        /// @ is_device_violet_monitor_primary
        /// <para>This method checks whether unique id type is primary videowall monitor unique id.</para>
        /// </summary>
        /// <returns>If the type is primary videowall monitor unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_device_violet_monitor_primary { get { return g2guid.g2_guid_is_device_violet_monitor_primary(ref this); } }
        /// <summary>
        /// @ is_device_network_keyboard
        /// <para>This method checks whether unique id type is network keyboard unique id.</para>
        /// </summary>
        /// <returns>If the type is network keyboard unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_device_network_keyboard { get { return g2guid.g2_guid_is_device_network_keyboard(ref this); } }
        /// <summary>
        /// @ is_layout
        /// <para>This method checks whether unique id type is layout unique id.</para>
        /// </summary>
        /// <returns>If the type is layout unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_layout { get { return g2guid.g2_guid_is_layout(ref this); } }
        /// <summary>
        /// @ is_sequence
        /// <para>This method checks whether unique id type is sequence monitoring unique id.</para>
        /// </summary>
        /// <returns>If the type is sequence monitoring unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_sequence { get { return g2guid.g2_guid_is_sequence(ref this); } }
        /// <summary>
        /// @ is_sequence_camera
        /// <para>This method checks whether unique id type is camera sequence monitoring unique id.</para>
        /// </summary>
        /// <returns>If the type is camera sequence monitoring unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_sequence_camera { get { return g2guid.g2_guid_is_sequence_camera(ref this); } }
        /// <summary>
        /// @ is_sequence_layout
        /// <para>This method checks whether unique id type is layout sequence monitoring unique id.</para>
        /// </summary>
        /// <returns>If the type is layout sequence monitoring unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_sequence_layout { get { return g2guid.g2_guid_is_sequence_layout(ref this); } }
        /// <summary>
        /// @ is_map
        /// <para>This method checks whether unique id type is map unique id.</para>
        /// </summary>
        /// <returns>If the type is map unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_map { get { return g2guid.g2_guid_is_map(ref this); } }
        /// <summary>
        /// @ is_map_device
        /// <para>This method checks whether unique id type is map device unique id.</para>
        /// </summary>
        /// <returns>If the type is map device unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_map_device { get { return g2guid.g2_guid_is_map_device(ref this); } }
        /// <summary>
        /// @ is_map_shortcut
        /// <para>This method checks whether unique id type is map shortcut unique id.</para>
        /// </summary>
        /// <returns>If the type is map shortcut unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_map_shortcut { get { return g2guid.g2_guid_is_map_shortcut(ref this); } }
        /// <summary>
        /// @ is_map_camera
        /// <para>This method checks whether unique id type is map camera unique id.</para>
        /// </summary>
        /// <returns>If the type is map camera unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_map_camera { get { return g2guid.g2_guid_is_map_camera(ref this); } }
        /// <summary>
        /// @ is_map_alarm_in
        /// <para>This method checks whether unique id type is map alarm-in device unique id.</para>
        /// </summary>
        /// <returns>If the type is map alarm-in device unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_map_alarm_in { get { return g2guid.g2_guid_is_map_alarm_in(ref this); } }
        /// <summary>
        /// @ is_map_alarm_in_network
        /// <para>This method checks whether unique id type is map network alarm-in device unique id.</para>
        /// </summary>
        /// <returns>If the type is map network alarm-in device unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_map_alarm_in_network { get { return g2guid.g2_guid_is_map_alarm_in_network(ref this); } }
        /// <summary>
        /// @ is_map_alarm_out
        /// <para>This method checks whether unique id type is map alarm-out device unique id.</para>
        /// </summary>
        /// <returns>If the type is map alarm-out device unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_map_alarm_out { get { return g2guid.g2_guid_is_map_alarm_out(ref this); } }
        /// <summary>
        /// @ is_map_audio_in
        /// <para>This method checks whether unique id type is map audio-in device unique id.</para>
        /// </summary>
        /// <returns>If the type is map audio-in device unique id, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_map_audio_in { get { return g2guid.g2_guid_is_map_audio_in(ref this); } }
        /// <summary>
        /// @ is_map_path_sequence
        /// <para>This method checks whether unique id type is map path unique id of sequence monitoring.</para>
        /// </summary>
        /// <returns>If the type is map path unique id of sequence monitoring, it returns TRUE; otherwise, it returns FALSE.</returns>
        public bool is_map_path_sequence { get { return g2guid.g2_guid_is_map_path_sequence(ref this); } }

        public static uint proc_enum_GUIDLIST(ref G2GUID guid, G2UPARAM param)
        {
            GCHandle gc = GCHandle.FromIntPtr(param);
            G2GUIDLIST GUIDs = (G2GUIDLIST)gc.Target;
            GUIDs.Add(guid);
            return G2BOOLEAN.G2TRUE;
        }
    }

    public partial struct G2PARAM_GUID_LIST
    {
        public G2GUID[] to()
        {
            G2GUID[] res = new G2GUID[_len];
            for (int i = 0; i < _len; ++i)
            {
                IntPtr ptr = new IntPtr(_guid.ToInt64() + i * Marshal.SizeOf(typeof(G2GUID)));
                res[i] = (G2GUID)Marshal.PtrToStructure(ptr, typeof(G2GUID));
            }
            return res;
        }
        public G2GUIDLIST to_list()
        {
            G2GUIDLIST res = new G2GUIDLIST();
            for (int i = 0; i < _len; ++i)
            {
                IntPtr ptr = new IntPtr(_guid.ToInt64() + i * Marshal.SizeOf(typeof(G2GUID)));
                res.Add((G2GUID)Marshal.PtrToStructure(ptr, typeof(G2GUID)));
            }
            return res;
        }
        public G2GUIDSET to_set()
        {
            G2GUIDSET res = new G2GUIDSET();
            for (int i = 0; i < _len; ++i)
            {
                IntPtr ptr = new IntPtr(_guid.ToInt64() + i * Marshal.SizeOf(typeof(G2GUID)));
                res.insert((G2GUID)Marshal.PtrToStructure(ptr, typeof(G2GUID)));
            }
            return res;
        }
        public void set(IntPtr ptr, int len) { _guid = ptr; _len = (uint)len; }
        public static long serialize(BinaryWriter s, G2GUIDSET GUIDs)
        {
            for (int i = 0; i < GUIDs.Count; ++i)
            {
                GUIDs[i].serialize(s);
            }
            return s.BaseStream.Position;
        }
        public static long serialize(BinaryWriter s, G2GUIDLIST GUIDs)
        {
            for (int i = 0; i < GUIDs.Count; ++i)
            {
                GUIDs[i].serialize(s);
            }
            return s.BaseStream.Position;
        }
    }

    public class G2GUIDLIST : List<G2GUID> { }
    public class G2GUIDSET : g2set<G2GUID> { }
}
