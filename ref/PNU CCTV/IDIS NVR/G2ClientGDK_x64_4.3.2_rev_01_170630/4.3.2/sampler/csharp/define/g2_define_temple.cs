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

    public struct G2PUMP_MESSAGE_OPTIONS
    {
        public struct FLAGS
        {
            public uint val;
            public bool REMOVE
            {
                get { return (val & 1) != 0; }
                set { if (value) val |= 1; }
            }
            public bool NOYIELD
            {
                get { return (val & 2) != 0; }
                set { if (value) val |= 2; }
            }
            public bool QS_INPUT
            {
                get { return (val & 67567616) != 0; }
                set { if (value) val |= 67567616; }
            }
            public bool QS_POSTMESSAGE
            {
                get { return (val & 9961472) != 0; }
                set { if (value) val |= 9961472; }
            }
            public bool QS_PAINT
            {
                get { return (val & 2097152) != 0; }
                set { if (value) val |= 2097152; }
            }
            public bool QS_SENDMESSAGE
            {
                get { return (val & 4194304) != 0; }
                set { if (value) val |= 4194304; }
            }
        }

        public G2HWND handle;
        public uint filter_min;
        public uint filter_max;
        public FLAGS flags;
    }
}
