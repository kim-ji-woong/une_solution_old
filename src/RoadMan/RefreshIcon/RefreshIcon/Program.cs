using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace RefreshIcon
{
    static class Program
    {
        [DllImport("Shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern void SHChangeNotify(int wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            SHChangeNotify(0x8000000, 0x1000, IntPtr.Zero, IntPtr.Zero);
            SHChangeNotify(0x08000000, 0x1000, IntPtr.Zero, IntPtr.Zero);
            SHChangeNotify(0x00008000, 0x1000, IntPtr.Zero, IntPtr.Zero);
            SHChangeNotify(0x00002000, 0x1000, IntPtr.Zero, IntPtr.Zero);
        }
    }
}
