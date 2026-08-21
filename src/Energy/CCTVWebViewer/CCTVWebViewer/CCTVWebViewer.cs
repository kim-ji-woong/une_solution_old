using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using SHDocVw;

namespace CCTVWeb
{
    public partial class CCTVWebViewer : Form
    {
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", CharSet=CharSet.Unicode)]
        private static extern IntPtr FindWindowEx(IntPtr hWnd1, IntPtr hWnd2, string lpsz1, string lpsz2);
        [DllImport("user32.dll")]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        [DllImport("user32.dll")]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        private static extern bool SetWindowPos(
                IntPtr hWnd, // window handle
                IntPtr hWndInsertAfter, // placement-order handle
                int X, // horizontal position
                int Y, // vertical position
                int cx, // width
                int cy, // height
                uint uFlags); // window positioning flags

        [DllImport("user32")]
        public static extern int GetWindowRect(IntPtr hwnd, ref RECT lpRect);

        [DllImport("user32.dll")]
        private static extern bool DestroyWindow(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32")]
        public static extern Boolean ShowWindow(IntPtr hWnd, Int32 nCmdShow);

        internal const int GWL_STYLE = -16;
        internal const int GWL_EXSTYLE = -20;

        internal const int WS_BORDER = 0x00800000;
        internal const int WS_CAPTION = 0x00C00000;
        internal const int WS_CHILD = 0x40000000;
        internal const int WS_SYSMENU = 0x00080000;
        internal const int WS_THICKFRAME = 0x0040000;

        internal const int WS_EX_CLIENTEDGE = 0x0200;
        internal const int WS_EX_MDICHILD = 0x0040;
        const int WS_EX_TOOLWINDOW = 0x00000080;

        internal const int SWP_FRAMECHANGED = 0x0020;
        internal const int SWP_NOMOVE = 0x0002;
        internal const int SWP_NOOWNERZORDER = 0x0200;
        internal const int SWP_NOSIZE = 0x0001;
        internal const int SWP_NOZORDER = 0x0004;

        const uint WM_CLOSE = 0x0010;
        const uint WM_KEYDOWN = 0x0100;
        const uint WM_MOUSEDOWN = 0x201;
        const uint WM_MOUSEUP = 0x202;

        const uint MK_LBUTTON = 0x201;

        Process m_processWeb = null;
        IntPtr m_hWndWeb = IntPtr.Zero;
        int m_yPos = 0;//-77;
        int m_nHeight = 0;

        public CCTVWebViewer()
        {
            InitializeComponent();

            Init();
        }

        private void Init()
        {
            string tabName = "<?TITLE?> - Internet Explorer";
            m_hWndWeb = FindWindowEx(IntPtr.Zero, IntPtr.Zero, "IEFrame", tabName);
            if (m_hWndWeb != IntPtr.Zero)
                DestroyWindow(m_hWndWeb);

            string szName = Application.StartupPath + @"\..\..\..\..\Web\screen.html";

            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = "IExplore.exe";
            startInfo.ErrorDialog = true;
            startInfo.Arguments = szName;
            startInfo.WindowStyle = ProcessWindowStyle.Normal;

            m_processWeb = Process.Start(startInfo);
            if (m_processWeb == null)
                return;

            m_processWeb.WaitForInputIdle();

            m_hWndWeb = FindWindowEx(IntPtr.Zero, IntPtr.Zero, "IEFrame", tabName);
            while (m_hWndWeb == IntPtr.Zero)
            {
                m_hWndWeb = FindWindowEx(IntPtr.Zero, IntPtr.Zero, "IEFrame", tabName);
            }

            SetParent(m_hWndWeb, panel1.Handle);

            int style = GetWindowLong(m_hWndWeb, GWL_STYLE);
            int exStyle = GetWindowLong(m_hWndWeb, GWL_EXSTYLE);
            style &= ~(WS_BORDER | WS_THICKFRAME | WS_CAPTION | WS_SYSMENU);
            exStyle &= ~WS_EX_CLIENTEDGE | ~WS_EX_TOOLWINDOW;
            exStyle |= WS_CHILD;
            SetWindowLong(m_hWndWeb, GWL_STYLE, (int)style);
            SetWindowLong(m_hWndWeb, GWL_EXSTYLE, (int)exStyle);

            SetWindowPos(m_hWndWeb, IntPtr.Zero, 0, 0, 0, 0,
                SWP_FRAMECHANGED | SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_NOOWNERZORDER);

            if (m_hWndWeb != IntPtr.Zero)
            {
                int width = panel1.Width;
                int height = panel1.Height;

                m_nHeight = (int)((height - m_yPos) * 1.02);
                MoveWindow(m_hWndWeb, 0, m_yPos, width, m_nHeight, true);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FullScreenWeb();
        }

        private void FullScreenWeb()
        {
            string tabName = "<?TITLE?> - Internet Explorer";
            IntPtr h1 = FindWindowEx(panel1.Handle, IntPtr.Zero, "IEFrame", tabName);
            if (h1 == IntPtr.Zero)
                return;

            IntPtr h2 = FindWindowEx(h1, IntPtr.Zero, "Frame Tab", "");
            if (h2 == IntPtr.Zero)
                return;

            IntPtr h3 = FindWindowEx(h2, IntPtr.Zero, "TabWindowClass", tabName);
            if (h3 == IntPtr.Zero)
                return;

            IntPtr h4 = FindWindowEx(h3, IntPtr.Zero, "Shell DocObject View", "");
            if (h4 == IntPtr.Zero)
                return;

            IntPtr ie = FindWindowEx(h4, IntPtr.Zero, "Internet Explorer_Server", "");
            if (ie == IntPtr.Zero)
                return;

            RECT rt = default(RECT);
            GetWindowRect(ie, ref rt);

            int y = rt.Bottom - rt.Top - 1;
            Point pt = new Point(20, y);

            SendMessage(ie, WM_MOUSEDOWN, (IntPtr)MK_LBUTTON, MakeLParam(pt.X, pt.Y));
            SendMessage(ie, WM_MOUSEUP, (IntPtr)MK_LBUTTON, MakeLParam(pt.X, pt.Y));
            SendMessage(ie, WM_MOUSEDOWN, (IntPtr)MK_LBUTTON, MakeLParam(pt.X, pt.Y));
            SendMessage(ie, WM_MOUSEUP, (IntPtr)MK_LBUTTON, MakeLParam(pt.X, pt.Y));

            if (m_hWndWeb != IntPtr.Zero)
            {
                int width = panel1.Width;
                int height = panel1.Height;

                m_nHeight = (int)((height - m_yPos) * 1.02);
                MoveWindow(m_hWndWeb, 0, m_yPos, width, m_nHeight, true);
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (m_hWndWeb != IntPtr.Zero)
            {
                int width = panel1.Width;
                int height = panel1.Height;
                m_nHeight = (int)((height - m_yPos) * 1.02);
                
                MoveWindow(m_hWndWeb, 0, m_yPos, width, m_nHeight, true);
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            string tabName = "<?TITLE?> - Internet Explorer";
            IntPtr h = FindWindowEx(panel1.Handle, IntPtr.Zero, "IEFrame", tabName);
            if (h != IntPtr.Zero)
            {
                //SendMessage(h, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                try
                {
                    //m_processWeb.CloseMainWindow();
                    m_processWeb.Kill();
                }
                catch (Exception)
                {

                }
            }
        }

        public IntPtr MakeLParam(int LoWord, int HiWord)
        {
            return (IntPtr)((HiWord << 16) | (LoWord & 0xffff));
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string tabName = "<?TITLE?> - Internet Explorer";
            IntPtr h1 = FindWindowEx(panel1.Handle, IntPtr.Zero, "IEFrame", tabName);
            if (h1 == IntPtr.Zero)
                return;

            IntPtr h2 = FindWindowEx(h1, IntPtr.Zero, "Frame Tab", "");
            if (h2 == IntPtr.Zero)
                return;

            IntPtr h3 = FindWindowEx(h2, IntPtr.Zero, "TabWindowClass", tabName);
            if (h3 == IntPtr.Zero)
                return;

            IntPtr h4 = FindWindowEx(h3, IntPtr.Zero, "Shell DocObject View", "");
            if (h4 == IntPtr.Zero)
                return;

            IntPtr ie = FindWindowEx(h4, IntPtr.Zero, "Internet Explorer_Server", "");
            if (ie == IntPtr.Zero)
                return;

            RECT rt = default(RECT);
            GetWindowRect(ie, ref rt);

            int idx = comboBox1.SelectedIndex;
            int y = rt.Bottom - rt.Top - 1;
            Point pt = new Point(idx + 1, y);
            //Point pt = new Point(idx+1, panel1.Height-1);

            SendMessage(ie, WM_MOUSEDOWN, (IntPtr)MK_LBUTTON, MakeLParam(pt.X, pt.Y));
            SendMessage(ie, WM_MOUSEUP, (IntPtr)MK_LBUTTON, MakeLParam(pt.X, pt.Y));
            SendMessage(ie, WM_MOUSEDOWN, (IntPtr)MK_LBUTTON, MakeLParam(pt.X, pt.Y));
            SendMessage(ie, WM_MOUSEUP, (IntPtr)MK_LBUTTON, MakeLParam(pt.X, pt.Y));
        }

        private void OnBtnClickPtz(object sender, EventArgs e)
        {
            string tabName = "<?TITLE?> - Internet Explorer";
            IntPtr h1 = FindWindowEx(panel1.Handle, IntPtr.Zero, "IEFrame", tabName);
            if (h1 == IntPtr.Zero)
                return;

            //int h = (panel1.Height - m_yPos) * 2;
            //MoveWindow(h1, 0, m_yPos, panel1.Width, h, true);

            IntPtr h2 = FindWindowEx(h1, IntPtr.Zero, "Frame Tab", "");
            if (h2 == IntPtr.Zero)
                return;

            IntPtr h3 = FindWindowEx(h2, IntPtr.Zero, "TabWindowClass", tabName);
            if (h3 == IntPtr.Zero)
                return;

            IntPtr h4 = FindWindowEx(h3, IntPtr.Zero, "Shell DocObject View", "");
            if (h4 == IntPtr.Zero)
                return;

            IntPtr ie = FindWindowEx(h4, IntPtr.Zero, "Internet Explorer_Server", "");
            if (ie == IntPtr.Zero)
                return;

            RECT rt = default(RECT);
            GetWindowRect(ie, ref rt);

            int cmd = 0;
            Button btn = (Button)sender;
            if (btn == btnUp)
                cmd = 33;
            else if (btn == btnDown)
                cmd = 34;
            else if (btn == btnRight)
                cmd = 35;
            else if (btn == btnLeft)
                cmd = 36;
            else if (btn == btnZoomIn)
                cmd = 38;
            else if (btn == btnZoomOut)
                cmd = 37;
            else if (btn == btnFocusIn)
                cmd = 39;
            else if (btn == btnFocusOut)
                cmd = 40;
            else if (btn == btnUpLeft)
                cmd = 41;
            else if (btn == btnUpRight)
                cmd = 42;
            else if (btn == btnDownLeft)
                cmd = 43;
            else if (btn == btnDownRight)
                cmd = 44;
            else if (btn == btnAuto)
                cmd = 51;
            else if (btn == btnIrisOut)
                cmd = 65;
            else if (btn == btnIrisIn)
                cmd = 66;

            //int y = panel1.Height - 1;
            int y = rt.Bottom - rt.Top - 1;
            Point pt = new Point(cmd, y);
            SendMessage(ie, WM_MOUSEDOWN, (IntPtr)MK_LBUTTON, MakeLParam(pt.X, pt.Y));
            SendMessage(ie, WM_MOUSEUP, (IntPtr)MK_LBUTTON, MakeLParam(pt.X, pt.Y));
            SendMessage(ie, WM_MOUSEDOWN, (IntPtr)MK_LBUTTON, MakeLParam(pt.X, pt.Y));
            SendMessage(ie, WM_MOUSEUP, (IntPtr)MK_LBUTTON, MakeLParam(pt.X, pt.Y));
            Console.WriteLine("Button Down");
        }

        private void btnFullScreen_Click(object sender, EventArgs e)
        {
            FullScreenWeb();
        }
    }
}
