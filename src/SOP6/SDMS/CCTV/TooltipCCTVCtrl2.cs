using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.Sensor;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SDMS
{
    public partial class TooltipCCTVCtrl2 : Form, IPOIPopup
    {
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindowEx(IntPtr hWnd1, IntPtr hWnd2, string lpsz1, string lpsz2);
        [DllImport("user32.dll")]
        // private static extern int SendMessage(int hwnd, int wMsg, int wParam, int lParam);

        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        // Target과의 거리
        static private int m_nTargetSpaceX = 30;

        static private int m_nTargetSpaceY = 50;

        private int m_nOwnTargetSpaceX = -1;
        private int m_nOwnTargetSpaceY = -1;
        private int m_nTargetPOIX = 0;
        private int m_nTargetPOIY = 0;
        private Point m_ptOrigin = new Point();

        private ISensorTooltipOwner m_viewOwner = null;

        private CCTV m_cctv = null;
        private Process m_cctvProcess = null;
        private FormHandleInfo m_handleInfo = new FormHandleInfo();

        private static List<TooltipCCTVCtrl2> m_currentTooltips = new List<TooltipCCTVCtrl2>();

        public CCTV CCTV
        {
            get { return m_cctv; }
            set { m_cctv = value; }
        }

        public ISensor Sensor
        {
            get;
            set;
        }

        private bool m_bLayerVisible = true;

        public bool LayerVisible
        {
            get { return m_bLayerVisible; }
            set
            {
                m_bLayerVisible = value;
                if (m_bLayerVisible == false)
                {
                    Visible = false;
                }
                else
                {
                    if (this.Visible)
                    {
                        //base.Show();
                    }
                }
            }
        }

        public static TooltipCCTVCtrl2 MakeInstance(ISensorTooltipOwner view, CCTV cctv)
        {
            return new TooltipCCTVCtrl2(view, cctv);
        }

        public TooltipCCTVCtrl2(ISensorTooltipOwner view, CCTV cctv)
        {
            InitializeComponent();

            m_nOwnTargetSpaceX = m_nTargetSpaceX;
            m_nOwnTargetSpaceY = m_nTargetSpaceY;

            this.TopLevel = false;
            view.AddToolTipControl(this);
            this.BringToFront();

            m_viewOwner = view;
            m_cctv = cctv;

            base.Hide();
        }

        private void TooltipCCTVCtrl2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_cctv != null && m_cctv.POI != null)
            {
                UnE.View.Content.IFormContent content = FormMain.Instance.PageHome.ContentForm;

                if (content != null && content.OutdoorView != null)
                {
                    content.OutdoorView.ChangePOIIcon(m_cctv.POI, "CCTV");
                }
            }

            Disconnect();

            m_currentTooltips.Remove(this);
            e.Cancel = true;
            base.Hide();
        }

        private void TooltipCCTVCtrl2_Resize(object sender, EventArgs e)
        {
            if (m_cctvProcess != null)
            {
                Process p = m_cctvProcess;

                if (p != null && p.HasExited == false)
                {
                    IntPtr hWnd = FindWindowEx(cctvPanel.Handle, IntPtr.Zero, null, m_handleInfo.Name);

                    if (hWnd != IntPtr.Zero)
                    {
                        this.Invoke(new Action(() =>
                        {
                            MoveWindow(hWnd, 1, 1, cctvPanel.Width, cctvPanel.Height, true);
                            System.Windows.Forms.Control c = Form.FromHandle(hWnd);
                            if (c != null)
                                c.Refresh();
                        }));
                    }
                }
            }
        }

        // CCTVViewer가 실행된 이후 Window가 생성될때 CCTVViewer의 크기를 변경시키기 위한 타이머
        private void OnTimer(object sender, EventArgs e)
        {
            IntPtr hWnd = FindWindowEx(cctvPanel.Handle, IntPtr.Zero, null, m_handleInfo.Name);

            if (hWnd != IntPtr.Zero)
            {
                this.Invoke(new Action(() =>
                {
                    MoveWindow(hWnd, 1, 1, cctvPanel.Width, cctvPanel.Height, true);
                    System.Windows.Forms.Control c = Form.FromHandle(hWnd);
                    if (c != null)
                        c.Refresh();
                }));

                timer1.Stop();
                System.Diagnostics.Trace.WriteLine("OnTimer is Stopped");
            }

            System.Diagnostics.Trace.WriteLine("OnTimer");
        }

        public void Disconnect()
        {
            KillProcess();
            m_cctvProcess = null;
            m_handleInfo.FormProcess = null;
        }

        protected virtual void LoadCamera()
        {
            try
            {
                if (m_cctvProcess != null && m_cctvProcess.HasExited == false)
                    KillProcess();

                IntPtr handle = IntPtr.Zero;

                this.Invoke((MethodInvoker)delegate
                {
                    handle = cctvPanel.Handle;
                });

                Guid guid = Guid.NewGuid();
                string szName = string.Format("CCTVViewer{0}", guid.ToString());
                int EquipZoneID = -1;
                
                string args = string.Format("{0} {1} {2} {3} {4} {5} {6} 0", handle, szName, UnE.SOP.ProxySOP.Instance.SiteID, m_cctv.ID, 1
                    , false, EquipZoneID);

                string szDir = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
                string szFileName = szDir + "\\" + "CCTVViewer2.exe";

                if (System.IO.File.Exists(szFileName))
                {
                    m_cctvProcess = StartPocess(szFileName, szDir, args);

                    IntPtr ptr = FindWindowEx(handle, IntPtr.Zero, null, szName);

                    m_handleInfo.FormProcess = m_cctvProcess;
                    m_handleInfo.HWnd = ptr;
                    m_handleInfo.Name = szName;
                }
                else
                {
                    szDir = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
                    szFileName = szDir + "\\common\\" + "CCTVViewer2.exe";

                    m_cctvProcess = StartPocess(szFileName, szDir, args);

                    IntPtr ptr = FindWindowEx(handle, IntPtr.Zero, null, szName);
                    m_handleInfo.FormProcess = m_cctvProcess;
                    m_handleInfo.HWnd = ptr;
                    m_handleInfo.Name = szName;
                }

                timer1.Start();
            }
            catch (Exception)
            {
                m_cctvProcess = null;
            }
        }

        private Process StartPocess(string szFileName, string szWorkDir, string args)
        {
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = szFileName;
            startInfo.WorkingDirectory = szWorkDir;
            startInfo.ErrorDialog = true;
            startInfo.Arguments = args;

            System.Diagnostics.Process process;
            try
            {
                process = System.Diagnostics.Process.Start(startInfo);
                return process;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
            return null;
        }

        private void KillProcess()
        {
            Process p = m_cctvProcess;

            if (p != null && p.HasExited == false)
            {
                try
                {
                    p.Kill();
                }
                catch (Exception)
                {
                }
            }

            m_cctvProcess = null;
        }

        // xTarget, yTarget : Target POI의 좌표
        public void Show(int xTarget, int yTarget)
        {
            try
            {
                if (xTarget + this.Size.Width > this.Parent.Size.Width)
                    xTarget = this.Parent.Size.Width - this.Size.Width;

                if (yTarget + this.Size.Height > this.Parent.Size.Height)
                    yTarget = this.Parent.Size.Height - this.Size.Height;

                LoadCamera();

                if (m_cctv != null)
                    this.Text = String.Format("{0} - {1}", m_cctv.ID, m_cctv.AccessKey);

                m_nTargetPOIX = xTarget;
                m_nTargetPOIY = yTarget;
                m_ptOrigin = this.Location;

                int x = xTarget + m_nOwnTargetSpaceX;
                int y = yTarget - m_nOwnTargetSpaceY;

                this.Location = new Point(xTarget, yTarget);
                this.Show();

                this.BringToFront();

                if (this.IsDisposed == false)
                {
                    if (m_currentTooltips.Contains(this) == false)
                    {
                        m_currentTooltips.Add(this);
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("TooltipCCTVCtrl2.Show() Error : " + e.Message);
            }
        }

        public void Hide(bool absolutely)
        {
            if (IsDisposed == true)
                return;

            if (absolutely)
            {
                m_currentTooltips.Remove(this);
                Disconnect();
                base.Hide();
            }
        }

        public void MoveTarget(int xTarget, int yTarget)
        {
            m_nTargetPOIX = xTarget;
            m_nTargetPOIY = yTarget;
            m_ptOrigin = this.Location;

            int x = xTarget + m_nOwnTargetSpaceX;
            int y = yTarget - m_nOwnTargetSpaceY;

            this.Location = new Point(x, y);
        }

        public bool IsVisible()
        {
            if (m_bLayerVisible == true && this.Visible == true)
                return true;
            return this.Visible;
        }

        public static void CloseAll()
        {
            for (int i = m_currentTooltips.Count - 1; i >= 0; i--)
            {
                m_currentTooltips[i].Hide(true);
            }

            m_currentTooltips.Clear();
        }
    }
}
