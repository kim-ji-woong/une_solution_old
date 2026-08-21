using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDK
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void G2VERBOSE_CALLBACK(G2MAIN_VERBOSE.LEVEL level, [MarshalAs(UnmanagedType.LPWStr)] string verbose);

    public class g2main
    {
        #region GDK DLL Import
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_main_app_initialize(int language);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_main_app_finalize();
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_main_set_language(int language);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_main_verbose_set_level(int level);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_main_verbose_callback_invoke(G2VERBOSE_CALLBACK func);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_main_verbose_callback_revoke();
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        protected static extern void g2_main_options_live_audio_in_load_audio_data([MarshalAs(UnmanagedType.U1)] bool flag);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern int g2_main_get_language();
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern int g2_main_verbose_get_level();
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool g2_main_verbose_is_activate();
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern void g2_main_dvrns_setup(string address, ushort port);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_main_date_time_formatter_set_format(int date, int time);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern void g2_main_date_time_formatter_set_string(string am, string pm);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_main_date_time_formatter_get_string_date(ref G2TIME time, out G2STRING_32 res);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_main_date_time_formatter_get_string_time(ref G2TIME time, out G2STRING_32 res);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_main_date_time_formatter_get_string_date_time(ref G2TIME time, out G2STRING_64 res);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern void g2_main_get_string(int id, out G2STRING_256 str);
        #endregion

        public static void app_initialize(G2LANGUAGE.ID language)
        {
            g2_main_app_initialize((int)language);
        }
        public static void app_finalize()
        {
            g2_main_app_finalize();
        }
        public static void set_language(G2LANGUAGE.ID language)
        {
            g2_main_set_language((int)language);
        }
        public static void verbose_set_level(G2MAIN_VERBOSE.LEVEL level)
        {
            g2_main_verbose_set_level((int)level);
        }
        public static void verbose_callback_invoke(G2VERBOSE_CALLBACK fn)
        {
            g2_main_verbose_callback_invoke(fn);
        }
        public static void verbose_callback_revoke()
        {
            g2_main_verbose_callback_revoke();
        }
        public static void options_live_audio_in_load_audio_data(bool flag)
        {
            g2_main_options_live_audio_in_load_audio_data(flag);
        }
        public static G2LANGUAGE.ID get_language()
        {
            return (G2LANGUAGE.ID)g2_main_get_language();
        }
        public static G2MAIN_VERBOSE.LEVEL verbose_get_level()
        {
            return (G2MAIN_VERBOSE.LEVEL)g2_main_verbose_get_level();
        }
        public static bool verbose_is_activate()
        {
            return g2_main_verbose_is_activate();
        }

        public static void dvrns_setup(string address, ushort port)
        {
            g2_main_dvrns_setup(address, port);
        }

        public static void time_formatter_set_format(G2DATE_TIME_FORMAT.DATE_TYPE date, G2DATE_TIME_FORMAT.TIME_TYPE time)
        {
            g2_main_date_time_formatter_set_format((int)date, (int)time);
        }
        public static void time_formatter_set_string(string am, string pm)
        {
            g2_main_date_time_formatter_set_string(am, pm);
        }
        public static string time_to_string_date(G2TIME time)
        {
            G2STRING_32 res;
            g2_main_date_time_formatter_get_string_date(ref time, out res);
            return res;
        }
        public static string time_to_string_time(G2TIME time)
        {
            G2STRING_32 res;
            g2_main_date_time_formatter_get_string_time(ref time, out res);
            return res;
        }
        public static string time_to_string_date_time(G2TIME time)
        {
            G2STRING_64 res;
            g2_main_date_time_formatter_get_string_date_time(ref time, out res);
            return res;
        }

        public static string get_string(int id)
        {
            G2STRING_256 res;
            g2_main_get_string(id, out res);
            return res;
        }
    }
}
