using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDK
{
    using G2HANDLE = System.Int32;
    using G2HWATCH = System.Int32;
    using G2HWND = System.IntPtr;
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

    public class g2temple
    {
        #region GDK DLL Import
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern int g2_temple_pump_message(G2HWND handle, uint filter_min, uint filter_max, uint flags);
        [DllImport(G2PLATFORM.DLL_name, CallingConvention = CallingConvention.StdCall)]
        public static extern int g2_temple_pump_message_def();
        #endregion

        public static int pump_message(G2PUMP_MESSAGE_OPTIONS options)
        {
            return g2_temple_pump_message(options.handle, options.filter_min, options.filter_max, options.flags.val);
        }
        public static int pump_message()
        {
            return g2_temple_pump_message_def();
        }
    }
}
