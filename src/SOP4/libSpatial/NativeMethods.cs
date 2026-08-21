using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32;
using System.Security.AccessControl;
using System.Drawing;

namespace UnE.Win32
{
    public static class NativeMethods
    {
        #region Win32 Contant Value

        public const int WM_MOVE = 0x0003;
        public const int WM_LBUTTONDOWN = 0x201; //Left mousebutton down
        public const int WM_LBUTTONUP = 0x202; //Left mousebutton up
        public const int WM_LBUTTONDBLCLK = 0x203; //Left mousebutton doubleclick
        public const int WM_RBUTTONDOWN = 0x204; //Right mousebutton down
        public const int WM_RBUTTONUP = 0x205;  //Right mousebutton up
        public const int WM_RBUTTONDBLCLK = 0x206; //Right mousebutton doubleclick


        public const int GWL_ID = -12;
        public const int GWL_STYLE = -16;
        public const int GWL_EXSTYLE = -20;

        public const uint WS_OVERLAPPED = 0;
        public const uint WS_POPUP = 0x80000000;
        public const int WS_CHILD = 0x40000000;
        public const uint WS_MINIMIZE = 0x20000000;
        public const uint WS_VISIBLE = 0x10000000;
        public const uint WS_DISABLED = 0x8000000;
        public const uint WS_CLIPSIBLINGS = 0x4000000;
        public const uint WS_CLIPCHILDREN = 0x2000000;
        public const uint WS_MAXIMIZE = 0x1000000;
        public const uint WS_CAPTION = 0xC00000;      // WS_BORDER or WS_DLGFRAME  
        public const int WS_BORDER = 0x00800000;
        public const int WS_DLGFRAME = 0x400000;
        public const uint WS_VSCROLL = 0x200000;
        public const uint WS_HSCROLL = 0x100000;
        public const uint WS_SYSMENU = 0x80000;
        public const int WS_THICKFRAME = 0x0040000;
        public const uint WS_GROUP = 0x20000;
        public const uint WS_TABSTOP = 0x10000;
        public const uint WS_MINIMIZEBOX = 0x20000;
        public const uint WS_MAXIMIZEBOX = 0x10000;
        public const uint WS_TILED = WS_OVERLAPPED;
        public const uint WS_ICONIC = WS_MINIMIZE;
        public const uint WS_SIZEBOX = WS_THICKFRAME;

        // Extended Window Styles 
        public const long WS_EX_DLGMODALFRAME = 0x0001;
        public const long WS_EX_NOPARENTNOTIFY = 0x0004;
        public const long WS_EX_TOPMOST = 0x0008;
        public const long WS_EX_ACCEPTFILES = 0x0010;
        public const long WS_EX_TRANSPARENT = 0x0020;
        public const int WS_EX_MDICHILD = 0x0040;
        public const long WS_EX_TOOLWINDOW = 0x0080;
        public const long WS_EX_WINDOWEDGE = 0x0100;
        public const int WS_EX_CLIENTEDGE = 0x0200;
        public const long WS_EX_CONTEXTHELP = 0x0400;
        public const long WS_EX_RIGHT = 0x1000;
        public const long WS_EX_LEFT = 0x0000;
        public const long WS_EX_RTLREADING = 0x2000;
        public const long WS_EX_LTRREADING = 0x0000;
        public const long WS_EX_LEFTSCROLLBAR = 0x4000;
        public const long WS_EX_RIGHTSCROLLBAR = 0x0000;
        public const long WS_EX_CONTROLPARENT = 0x10000;
        public const long WS_EX_STATICEDGE = 0x20000;
        public const long WS_EX_APPWINDOW = 0x40000;
        public const long WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE);
        public const long WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST);
        public const long WS_EX_LAYERED = 0x00080000;
        public const long WS_EX_NOINHERITLAYOUT = 0x00100000; // Disable inheritence of mirroring by children
        public const long WS_EX_LAYOUTRTL = 0x00400000; // Right to left mirroring
        public const long WS_EX_COMPOSITED = 0x02000000;
        public const long WS_EX_NOACTIVATE = 0x08000000;

        public const int HWND_TOPMOST = -1;
        public const int HWND_NOTOPMOST = -2;
        public const int SW_HIDE = 0;
        public const int SW_SHOWNORMAL = 1;
        public const int SW_SHOW = 5;
        public const int SW_MINIMIZE = 6;
        public const int SW_RESTORE = 9;

        public const int SWP_NOSIZE = 0x0001;
        public const int SWP_NOMOVE = 0x0002;
        public const int SWP_NOZORDER = 0x0004;
        public const int SWP_NOREDRAW = 0x0008;
        public const int SWP_NOACTIVATE = 0x0010;
        public const int SWP_FRAMECHANGED = 0x0020;  /* The frame changed: send WM_NCCALCSIZE */
        public const int SWP_SHOWWINDOW = 0x0040;
        public const int SWP_HIDEWINDOW = 0x0080;
        public const int SWP_NOCOPYBITS = 0x0100;
        public const int SWP_NOOWNERZORDER = 0x0200;
        public const int SWP_NOSENDCHANGING = 0x0400;  /* Don't send WM_WINDOWPOSCHANGING */
                


        public const int PFM_SPACEBEFORE = 0x00000040;
        public const int PFM_SPACEAFTER = 0x00000080;
        public const int PFM_LINESPACING = 0x00000100;
        public const int SCF_SELECTION = 1;
        public const int EM_SETPARAFORMAT = 1095;

        #endregion

        #region Win Function
        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindowEx(IntPtr hWnd1, IntPtr hWnd2, string lpsz1, string lpsz2);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        [DllImport("user32", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, ref PARAFORMAT lParam);
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("User32")]
        public static extern int SetForegroundWindow(IntPtr hwnd);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern bool SetWindowPos(
                IntPtr hWnd, // window handle
                IntPtr hWndInsertAfter, // placement-order handle
                int X, // horizontal position
                int Y, // vertical position
                int cx, // width
                int cy, // height
                uint uFlags); // window positioning flags

        [DllImport("user32.dll")]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr SetFocus(IntPtr hWnd);
               
        [DllImport("user32.dll")]
        public static extern int GetWindowRect(IntPtr hwnd, out Rectangle rect);
        
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern bool SetWindowPos(
                IntPtr hWnd, // window handle
                int hWndInsertAfter, // placement-order handle
                int X, // horizontal position
                int Y, // vertical position
                int cx, // width
                int cy, // height
                uint uFlags); // window positioning flags
        
        [DllImport("kernel32")]
        public static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        public static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);


        [DllImport("Kernel32.dll")]
        public static extern bool Beep(UInt32 frequency, UInt32 duration);

        #endregion


        public static void BringFront(IntPtr ptr)
        {
            SetWindowPos(ptr, HWND_TOPMOST, 0, 0, 0, 0,
                                          SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
            // 최상위 윈도우 속성을 제거한다. 하지만 윈도우는 다른 윈도우보다 앞에 존재한다. 
            SetWindowPos(ptr, HWND_NOTOPMOST, 0, 0, 0, 0,
                                          SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
        }

        public static IntPtr MakeLParam(int LoWord, int HiWord)
        {
            return (IntPtr)((HiWord << 16) | (LoWord & 0xffff));
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PARAFORMAT
    {
        public int cbSize;
        public uint dwMask;
        public short wNumbering;
        public short wReserved;
        public int dxStartIndent;
        public int dxRightIndent;
        public int dxOffset;
        public short wAlignment;
        public short cTabCount;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public int[] rgxTabs;

        // PARAFORMAT2 from here onwards
        public int dySpaceBefore;

        public int dySpaceAfter;
        public int dyLineSpacing;
        public short sStyle;
        public byte bLineSpacingRule;
        public byte bOutlineLevel;
        public short wShadingWeight;
        public short wShadingStyle;
        public short wNumberingStart;
        public short wNumberingStyle;
        public short wNumberingTab;
        public short wBorderSpace;
        public short wBorderWidth;
        public short wBorders;
    }
}
