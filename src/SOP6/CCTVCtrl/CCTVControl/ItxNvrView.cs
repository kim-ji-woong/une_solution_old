#if _ITX_NVR_
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace UnE.Control.CCTVControl
{
    public class ItxNvrView : AxitxviewLib.Axitxview
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("user32")]
        static extern IntPtr FindWindow(String lpClassName, String lpWindowName);

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        private const int WM_CLOSE = 0x0010;
        private const int WM_LBUTTONUP = 0x202;
        private const int WM_LBUTTONDBLCLK = 0x0203;

        private CCTVCtrl m_owner = null;
        private Timer itxConnectionTimer = null;
        // 접속 제한시간(초)
        private int m_nConnectionTimeOut = 60;
        private DateTime m_dtConnect = new DateTime();

        public ItxNvrView(CCTVCtrl owner)
        {
            m_owner = owner;

            itxConnectionTimer = new Timer();
            itxConnectionTimer.Interval = 1000;
            itxConnectionTimer.Tick += OnTimer;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_LBUTTONUP)
            {
                if (m_owner != null)
                    m_owner.OnMouseDown(this, System.Windows.Forms.MouseButtons.Left, 0, 0);
            }
            else if (m.Msg == WM_LBUTTONDBLCLK)
            {
                if (m_owner != null)
                    m_owner.OnMouseDblClick(this, System.Windows.Forms.MouseButtons.Left, 0, 0);
            }
            base.WndProc(ref m);
        }

        private void OnTimer(object sender, EventArgs e)
        {
            uint pid = 0;
            uint currentProcessID = 0;

            System.Diagnostics.Process currentProcess = System.Diagnostics.Process.GetCurrentProcess();

            if (currentProcess != null)
                currentProcessID = (uint)currentProcess.Id;

            IntPtr handle = FindWindow(null, "WEBVIE~1");

            if (handle == IntPtr.Zero)
                handle = FindWindow(null, "WEBVIE~2");

            GetWindowThreadProcessId(handle, out pid);

            // 접속 실패하여 실패하였다는 메시지 창이 나타나면, 창을 닫고 타이머를 종료한다.
            if (handle != IntPtr.Zero && handle != this.Handle && pid == currentProcessID)
            {
                SendMessage(handle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                this.SessionClose();
                itxConnectionTimer.Stop();
                m_owner.OnDisconnected(this);
                return;
            }

            // 접속이 성공하면 타이머를 종료한다.
            if (this.IsConnected())
            {
                itxConnectionTimer.Stop();
                m_owner.OnConnected(this);
                return;
            }

            TimeSpan span = DateTime.Now - m_dtConnect;

            if (span.TotalSeconds >= m_nConnectionTimeOut)
            {
                itxConnectionTimer.Stop();
                m_owner.OnDisconnected(this);
            }
        }

        public bool Connect(string strIP, short nPort, int nChannel, string strUserName, string strPassword, string strMacAddr)
        {
            if (this.IsConnected())
            {
                itxConnectionTimer.Stop();
                this.SessionClose();
            }

            if (strMacAddr == null || strMacAddr.Length == 0)
                return false;

            if (m_owner.PositionIndex >= 1)
            {
                System.Threading.Thread.Sleep(100 * m_owner.PositionIndex);
            }

            // 접속 성공여부를 감시하기 위한 타이머
            itxConnectionTimer.Start();
            m_dtConnect = DateTime.Now;

            this.SetAccount(strUserName, strPassword);
            this.SetOEMCode("S1", "IPX_0412");
            this.SetMaxLayout(6);
            this.SetMacAddress(strMacAddr);
            this.SessionOpen(strIP + "/live", nPort, 1, 1, 1, 0, 0);
            this.SetSplitMode(0, (short)nChannel);
            //axitxview1.SetCovert((short)nChannel, false, true, 255, "", null);

            return true;
        }
    }
}
#endif