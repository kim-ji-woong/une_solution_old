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
using System.Collections.Concurrent;
using System.IO;
using UnE.Util.Unity;

namespace SDMS_Building.Content
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

        private const int CCTVMaxCount = 4;
        // Key : CCTV Index
        private static Dictionary<int, TooltipCCTVCtrl2> m_dicCCTVTooltip = null;
        private static List<TooltipCCTVCtrl2> m_currentTooltips = new List<TooltipCCTVCtrl2>();
        private int m_nCCTVIndex = 0;

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

        public static Size CctvPopupSize = new Size();

        public static TooltipCCTVCtrl2 MakeInstance(ISensorTooltipOwner view, CCTV cctv)
        {
            return new TooltipCCTVCtrl2(view, cctv);
        }

        public TooltipCCTVCtrl2(ISensorTooltipOwner view, CCTV cctv)
        {
            InitializeComponent();

            InitCCTVIndex();

            cctvPanel.Size = new Size(this.Width - (m_nLine * 2), this.Height - pnTop.Height - m_nLine);
            cctvPanel.Location = new Point(m_nLine, pnTop.Height);

            m_nOwnTargetSpaceX = m_nTargetSpaceX;
            m_nOwnTargetSpaceY = m_nTargetSpaceY;

            TooltipCCTVCtrl2.CctvPopupSize = this.Size;

            this.TopLevel = false;
            view.AddToolTipControl(this);
            this.BringToFront();

            m_viewOwner = view;
            m_cctv = cctv;

            base.Hide();
        }

        private void InitCCTVIndex()
        {
            if (m_dicCCTVTooltip == null)
            {
                m_dicCCTVTooltip = new Dictionary<int, TooltipCCTVCtrl2>();

                for (int i = 1; i <= CCTVMaxCount; i++)
                {
                    m_dicCCTVTooltip[i] = null;
                }
            }
        }

        private void SetCCTVIndex()
        {
            for (int i = 1; i <= CCTVMaxCount; i++)
            {
                if (m_dicCCTVTooltip.ContainsKey(i))
                {
                    //if (m_dicCCTVTooltip[i] == null)
                    {
                        TooltipCCTVCtrl2 item = m_dicCCTVTooltip[i] as TooltipCCTVCtrl2;
                        if (item == null)
                        {
                            m_dicCCTVTooltip[i] = this;
                            m_nCCTVIndex = i;
                            SetPOIIndex(m_nCCTVIndex);
                            return;  
                        }
                    }
                }
            }

            // 모든 CCTV가 사용중이면 가장 먼저 생성된 CCTV 창을 닫는다.
            if (m_currentTooltips.Count <= 1)
                return;

            TooltipCCTVCtrl2 tooltip = m_currentTooltips[0];

            int nCCTVIndex = tooltip.m_nCCTVIndex;
            tooltip.Close();

            m_dicCCTVTooltip[nCCTVIndex] = this;
            m_nCCTVIndex = nCCTVIndex;
            SetPOIIndex(m_nCCTVIndex);
        }

        private void SetPOIIndex(int nIndex)
        {
            Panel4Unity panel = (Panel4Unity)FormMain.Instance.ContentManager.ContentForm.OutdoorView;

            if (nIndex > 0)
            {
                if (nIndex == 1)
                    pictureBoxIndex.BackgroundImage = global::SDMS_Building.Properties.Resources._1;
                else if (nIndex == 2)
                    pictureBoxIndex.BackgroundImage = global::SDMS_Building.Properties.Resources._2;
                else if (nIndex == 3)
                    pictureBoxIndex.BackgroundImage = global::SDMS_Building.Properties.Resources._3;
                else if (nIndex == 4)
                    pictureBoxIndex.BackgroundImage = global::SDMS_Building.Properties.Resources._4;

                pictureBoxIndex.Visible = true;
                panel.ChangePOIIcon(m_cctv.POI, "CCTV_" + nIndex.ToString());
            }
            else
            {
                pictureBoxIndex.Visible = false;
                panel.ChangePOIIcon(m_cctv.POI, "CCTV");
            }
        }

        private void TooltipCCTVCtrl2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Disconnect();

            //e.Cancel = true;

            SetPOIIndex(0);
            m_currentTooltips.Remove(this);
            m_dicCCTVTooltip[m_nCCTVIndex] = null;

            //base.Hide();
        }

        private void TooltipCCTVCtrl2_Resize(object sender, EventArgs e)
        {
            cctvPanel.Size = new Size(this.Width - (m_nLine * 2), this.Height - pnTop.Height - m_nLine);

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
        private void cctvPanel_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // 두배로 커짐
            if (this.Size.Width <= TooltipCCTVCtrl2.CctvPopupSize.Width && this.Size.Height <= TooltipCCTVCtrl2.CctvPopupSize.Height)
            {
                int width = TooltipCCTVCtrl2.CctvPopupSize.Width * 2;
                int height = TooltipCCTVCtrl2.CctvPopupSize.Height * 2;

                this.Size = new Size(width, height);
            }
            else
            {
                int width = TooltipCCTVCtrl2.CctvPopupSize.Width;
                int height = TooltipCCTVCtrl2.CctvPopupSize.Height;

                this.Size = new Size(width, height);
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

        private string MakeFile()
        {
            DateTime dtNow = DateTime.Now;
            string strPath = string.Format("CCTV_{0}_{1:00}{2:00}{3:000}.dat", m_cctv.ID, dtNow.Minute, dtNow.Second, dtNow.Millisecond);

            StreamWriter writer = new StreamWriter(strPath, false, System.Text.Encoding.UTF8);

            writer.WriteLine("!" + m_cctv.ID);
            writer.WriteLine("!" + m_cctv.AccessKey);
            writer.WriteLine("!" + m_cctv.IPAddress);
            writer.WriteLine("!" + m_cctv.PortNo);
            writer.WriteLine("");
            writer.WriteLine("!0");
            writer.WriteLine("!0");
            writer.WriteLine("!0");
            writer.WriteLine("!1");
            writer.WriteLine("!1");
            writer.WriteLine("!1");
            writer.WriteLine("");
            writer.WriteLine("");
            writer.WriteLine("!" + UnE.Control.CCTVCtrl.GetCCTVTypeString(m_cctv.CCTVType));
            writer.WriteLine("!" + m_cctv.Stream);
            writer.WriteLine("!" + m_cctv.Channel);
            writer.WriteLine("!" + m_cctv.UserName);
            writer.WriteLine("!" + m_cctv.Password);
            writer.WriteLine("!" + m_cctv.URL);
            writer.WriteLine("!" + m_cctv.ReversePTZ);
            writer.WriteLine("");
            writer.WriteLine("");

            writer.Close();
            return strPath;
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

                string strFilePath = MakeFile();
                string args = string.Format("{0} {1} {2} {3} {4} {5} {6} 0 {7}", handle, szName, UnE.SOP.ProxySOP.Instance.SiteID, m_cctv.ID, 1
                    , false, EquipZoneID, strFilePath);

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
            // 같은 CCTV 띄우지 않기
            for (int i = 0; i < m_currentTooltips.Count; i++)
            {
                if (m_currentTooltips[i].CCTV.ID == m_cctv.ID)
                {
                    m_currentTooltips[i].MoveTarget(xTarget, yTarget);
                    return;
                }
            }

            TooltipCCTVCtrl2 tooltip = MakeInstance(m_viewOwner, m_cctv);
            ShowChild(xTarget, yTarget, tooltip);
            /*try
            {
                if (this.Parent == null)
                    return;

                if (xTarget + this.Size.Width > this.Parent.Size.Width)
                    xTarget = this.Parent.Size.Width - this.Size.Width;

                if (yTarget + this.Size.Height > this.Parent.Size.Height)
                    yTarget = this.Parent.Size.Height - this.Size.Height;

                LoadCamera();

                if (m_cctv != null)
                    lblTitle.Text = String.Format("{0} - {1}", m_cctv.ID, m_cctv.AccessKey);

                m_nTargetPOIX = xTarget;
                m_nTargetPOIY = yTarget;
                m_ptOrigin = this.Location;

                int x = xTarget + m_nOwnTargetSpaceX;
                int y = yTarget - m_nOwnTargetSpaceY;

                this.Location = new Point(xTarget, yTarget);
                this.Show();

                this.BringToFront();

                FormMain.Instance.SelectedCCTV(m_cctv.ID);

                if (this.IsDisposed == false)
                {
                    if (m_currentTooltips.Contains(this) == false)
                    {
                        m_currentTooltips.Add(this);
                        SetCCTVIndex();
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("TooltipCCTVCtrl2.Show() Error : " + e.Message);
            }*/

            FormMain.Instance.SelectedPOI(IFacility.FacilityType.CCTV, m_cctv.ID);
        }

        private static void ShowChild(int xTarget, int yTarget, TooltipCCTVCtrl2 tooltip)
        {
            try
            {
                if (tooltip.Parent == null)
                    return;

                if (xTarget + tooltip.Size.Width > tooltip.Parent.Size.Width)
                    xTarget = tooltip.Parent.Size.Width - tooltip.Size.Width;

                if (yTarget + tooltip.Size.Height > tooltip.Parent.Size.Height)
                    yTarget = tooltip.Parent.Size.Height - tooltip.Size.Height;

                tooltip.LoadCamera();

                if (tooltip.m_cctv != null)
                    tooltip.lblTitle.Text = String.Format("{0} - {1}", tooltip.m_cctv.ID, tooltip.m_cctv.AccessKey);

                tooltip.m_nTargetPOIX = xTarget;
                tooltip.m_nTargetPOIY = yTarget;
                tooltip.m_ptOrigin = tooltip.Location;

                int x = xTarget + tooltip.m_nOwnTargetSpaceX;
                int y = yTarget - tooltip.m_nOwnTargetSpaceY;

                tooltip.Location = new Point(xTarget, yTarget);
                tooltip.Show();

                tooltip.BringToFront();

                FormMain.Instance.SelectedCCTV(tooltip.m_cctv.ID);

                if (tooltip.IsDisposed == false)
                {
                    if (m_currentTooltips.Contains(tooltip) == false)
                    {
                        m_currentTooltips.Add(tooltip);
                        tooltip.SetCCTVIndex();
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
                Disconnect();
                base.Hide();

                m_currentTooltips.Remove(this);
                m_dicCCTVTooltip[m_nCCTVIndex] = null;
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
                m_currentTooltips[i].Close();
                //m_currentTooltips[i].Hide(true);
            }

            m_currentTooltips.Clear();

            if (m_dicCCTVTooltip != null)
            {
                for (int i = 1; i <= CCTVMaxCount; i++)
                {
                    m_dicCCTVTooltip[i] = null;
                }
            }
        }

        #region 폼 이동
        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();
        private bool m_isClicked = false;
        private Point m_ptFrmOrigin = new Point();

        private void pnTop_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = Control.MousePosition;
                m_ptFrmOrigin = this.Location;
            }

            m_isClicked = true;
        }

        private void pnTop_MouseMove(object sender, MouseEventArgs e)
        {
            if (!m_isClicked)
                return;

            if (!m_bLeftMouseDown)
                return;

            Point ptScreen = Control.MousePosition;

            int dx = ptScreen.X - m_ptMove.X;
            int dy = ptScreen.Y - m_ptMove.Y;

            if (dx == 0 && dy == 0)
                return;

            Point ptCur = this.Location;
            this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
            m_ptMove.X += dx;
            m_ptMove.Y += dy;
        }

        private void pnTop_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;

            m_isClicked = false;
        }
        #endregion

        private void btnClose_Click(object sender, EventArgs e)
        {
            Disconnect();
            this.Close();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.FillRectangle(Brushes.Silver, Top);
            e.Graphics.FillRectangle(Brushes.Silver, Left);
            e.Graphics.FillRectangle(Brushes.Silver, Right);
            e.Graphics.FillRectangle(Brushes.Silver, Bottom);
        }

        private const int
            HTLEFT = 10,
            HTRIGHT = 11,
            HTTOP = 12,
            HTTOPLEFT = 13,
            HTTOPRIGHT = 14,
            HTBOTTOM = 15,
            HTBOTTOMLEFT = 16,
            HTBOTTOMRIGHT = 17;

        const int m_nLine = 3;

        Rectangle Top { get { return new Rectangle(0, 0, this.ClientSize.Width, m_nLine); } }
        Rectangle Left { get { return new Rectangle(0, 0, m_nLine, this.ClientSize.Height); } }

        Rectangle Bottom { get { return new Rectangle(0, this.ClientSize.Height - m_nLine, this.ClientSize.Width, m_nLine); } }
        Rectangle Right { get { return new Rectangle(this.ClientSize.Width - m_nLine, 0, m_nLine, this.ClientSize.Height); } }

        Rectangle TopLeft { get { return new Rectangle(0, 0, m_nLine, m_nLine); } }
        Rectangle TopRight { get { return new Rectangle(this.ClientSize.Width - m_nLine, 0, m_nLine, m_nLine); } }
        Rectangle BottomLeft { get { return new Rectangle(0, this.ClientSize.Height - m_nLine, m_nLine, m_nLine); } }
        Rectangle BottomRight { get { return new Rectangle(this.ClientSize.Width - m_nLine, this.ClientSize.Height - m_nLine, m_nLine, m_nLine); } }

        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);

            if (message.Msg == 0x84) // WM_NCHITTEST
            {
                var cursor = this.PointToClient(Cursor.Position);

                if (TopLeft.Contains(cursor)) message.Result = (IntPtr)HTTOPLEFT;
                else if (TopRight.Contains(cursor)) message.Result = (IntPtr)HTTOPRIGHT;
                else if (BottomLeft.Contains(cursor)) message.Result = (IntPtr)HTBOTTOMLEFT;
                else if (BottomRight.Contains(cursor)) message.Result = (IntPtr)HTBOTTOMRIGHT;

                else if (Top.Contains(cursor)) message.Result = (IntPtr)HTTOP;
                else if (Left.Contains(cursor)) message.Result = (IntPtr)HTLEFT;
                else if (Right.Contains(cursor)) message.Result = (IntPtr)HTRIGHT;
                else if (Bottom.Contains(cursor)) message.Result = (IntPtr)HTBOTTOM;
            }
        }
    }

    public class FormHandleInfo
    {
        private string m_szName = "";

        public string Name
        {
            get { return m_szName; }
            set { m_szName = value; }
        }

        private IntPtr m_hWnd;
        public IntPtr HWnd
        {
            get { return m_hWnd; }
            set { m_hWnd = value; }
        }

        private Process m_Process = null;
        public Process FormProcess
        {
            get { return m_Process; }
            set { m_Process = value; }
        }
    }
}
