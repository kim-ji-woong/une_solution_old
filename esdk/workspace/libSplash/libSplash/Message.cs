using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libSplash
{
    public class Message
    {
        public const int WM_COPYDATA = 0x4A;

        public const int ARE_YOU_THERE = 0x00;
        public const int I_AM_HERE = 0x01;
        public const int SPLASH_CLOSE = 0x02;
        public const int SPLASH_MESSAGE = 0x03;

        // {0} => CallerProcessID
        public const string SPLASH_HANLDE_FILE_NAME_FORMAT = "{0}_SplashHandle.ini";
    }

    public struct COPYDATASTRUCT
    {
        public IntPtr dwData;
        public int cbData;
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)]
        public string lpData;
    }
}
