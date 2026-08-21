using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace CCTVViewer
{
    static class Program
    {
        [DllImport("User32.dll")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            int nCCTVID = 0, nPort = 0;
            string strIP = "", strUserID = "", strPW = "", strCameraName = "";
            IntPtr parentWindowHandle = IntPtr.Zero;
            IntPtr messageWindowHandle = IntPtr.Zero;

            if (GetParams(args, ref nCCTVID, ref strIP, ref nPort, ref strUserID, ref strPW, ref strCameraName, ref parentWindowHandle, ref messageWindowHandle) == false)
            {
                System.Windows.Forms.MessageBox.Show("실행인자가 정확하지 않습니다.");
                return;
            }

            FormMain frm = new FormMain(nCCTVID, strIP, nPort, strUserID, strPW, strCameraName, messageWindowHandle);
            frm.TopLevel = false;

            SetParent(frm.Handle, parentWindowHandle);
            SetWindowPos(frm.Handle, IntPtr.Zero, 0, 0, frm.Size.Width, frm.Size.Height, 0);
            frm.Show();

            Application.Run(frm);
        }

        static bool GetParams(string[] args, ref int nCCTVID, ref string strIP, ref int nPort, ref string strUserID, ref string strPW, ref string strCameraName, ref IntPtr parentWindowHandle, ref IntPtr messageWindowHandle)
        {
            int nCount = args.Count();

            if (nCount < 8)
                return false;

            if (int.TryParse(args[0].Trim(), out nCCTVID) == false)
                return false;

            strIP = args[1].Trim();

            if (int.TryParse(args[2].Trim(), out nPort) == false)
                return false;

            strUserID = args[3].Trim();
            strPW = args[4].Trim();
            strCameraName = args[5].Trim();

            int parentHandle, messageHandle;
            if (int.TryParse(args[6].Trim(), out parentHandle) == false || int.TryParse(args[7].Trim(), out messageHandle) == false)
                return false;

            parentWindowHandle = new IntPtr(parentHandle);
            messageWindowHandle = new IntPtr(messageHandle);
            return true;
        }
    }
}
