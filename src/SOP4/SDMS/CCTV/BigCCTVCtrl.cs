using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using UnE.Sensor;
using UnE.Spatial;

namespace SDMS
{
    public partial class BigCCTVCtrl : Form
	{

        //[DllImport("user32.dll", CharSet=CharSet.Unicode)]
        //private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        //[DllImport("user32.dll", CharSet=CharSet.Unicode)]
        //private static extern IntPtr FindWindowEx(IntPtr hWnd1, IntPtr hWnd2, string lpsz1, string lpsz2);
        //[DllImport("user32.dll")]
        //private static extern int SendMessage(int hwnd, int wMsg, int wParam, int lParam);
        //[DllImport("user32.dll")]
        //private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
        //[DllImport("user32.dll")]
        //private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        //private const int SW_HIDE = 0;
        //private const int SW_SHOWNORMAL = 1;
        //private const int SW_SHOW = 5;

        //private const int SWP_NOSIZE = 0x0001;
        //private const int SWP_NOMOVE = 0x0002;
        //private const int SWP_NOZORDER = 0x0004;
        //private const int SWP_NOREDRAW = 0x0008;
        //private const int SWP_NOACTIVATE = 0x0010;
        //private const int SWP_FRAMECHANGED = 0x0020;  /* The frame changed: send WM_NCCALCSIZE */
        //private const int SWP_SHOWWINDOW = 0x0040;
        //private const int SWP_HIDEWINDOW = 0x0080;
        //private const int SWP_NOCOPYBITS = 0x0100;
        //private const int SWP_NOOWNERZORDER = 0x0200;  /* Don't do owner Z ordering */
        //private const int SWP_NOSENDCHANGING = 0x0400;  /* Don't send WM_WINDOWPOSCHANGING */

		private CCTV m_cctv = null;
		private bool m_isConnected = false;
		private int m_nBtnLeftPos = 0;
		private int m_nLeftSpace = 0;
		private bool m_isSelected = false;
		private Form4CCTV m_frmParent = null;
		private CCTV m_cctvQueue = null;

		private bool m_isValidCamera = false;
		private bool m_isClosing = false;
		//private System.IO.StreamWriter m_logger = null;

        private Process m_CCTVProcess = null;

		public bool IsConnected
		{
			get { return m_isConnected; }
		}

		public CCTV CCTV
		{
			get { return m_cctv; }
			set
			{
				if (m_cctv != value)
				{
					m_isValidCamera = false;

					if (m_cctv != null)
					{
						m_cctvQueue = value;
						CloseCamera();
					}


                    if (value != null)
					{
                        
						m_cctv = value;
                        
						LoadCamera();

                        if (this.IsDisposed == false && this.IsHandleCreated == true)
                        {

                            this.Invoke(
                             new Action(() => SetTitle(String.Format("{0} - {1}", m_cctv.ID, m_cctv.AccessKey)))
                             );
                        }
                        else
                        {
                            //MessageBox.Show("CCTVCrtl Disposed!, Check System");
                        }
                        BigCCTVCtrl_SizeChanged(null, null);
					}

					EnableControl(value != null);
				}
			}
		}

		public bool IsSelected
		{
			get { return m_isSelected; }
			set
			{
				m_isSelected = value;

				if (m_isSelected)
					this.BackColor = Color.FromArgb(109, 155, 206);
				else
					this.BackColor = Control.DefaultBackColor;
			}
		}

		private static int m_nCCTVCount = 0;
		protected int m_nID = -1;

		private static int m_isFakeMode = -1;
		protected static string m_strFakeCCTVFolderPath = "";

		private static int m_nInitWidth = 0, m_nInitHeight = 0;

    	public static BigCCTVCtrl MakeInstance(CCTV cctv, Form4CCTV frmParent)
		{

			return new BigCCTVCtrl(cctv, frmParent);
		}

		/*private void WriteLog(string strLog)
		{
			DateTime dtNow = DateTime.Now;
			string strNow = string.Format("{0}-{1}-{2} {3}:{4}:{5}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);
			m_logger.WriteLine(strNow + " : " + strLog);
			m_logger.Flush();
		}*/

		public BigCCTVCtrl(CCTV cctv, Form4CCTV frmParent)
		{
			m_nID = ++m_nCCTVCount;

			//string strLogFilePath = string.Format("BigCCTV_{0}.log", m_nID);
			//m_logger = new System.IO.StreamWriter(strLogFilePath, true, Encoding.UTF8);
			//WriteLog("LogFile Create");

          

			InitializeComponent();

            lbTitle.Parent =  this;
            lbTitle.BackColor = Color.Black;
            lbTitle.BringToFront();

			CCTV = cctv;

			this.TopLevel = false;
			frmParent.Controls.Add(this);
			m_frmParent = frmParent;
		}

		protected virtual void BigCCTVCtrl_Load(object sender, EventArgs e)
		{
			if (m_cctv != null)
				LoadCamera();
		}


        private void SetTitle(string szName)
        {
            lbTitle.Text = szName;
        }

        /*private UnE.Control.CCTVTypes GetCCTVType(int nType)
        {
            switch (nType)
            {
                case 1:
                    return UnE.Control.CCTVTypes.Axis;
                case 2:
                    return UnE.Control.CCTVTypes.NVS;
                case 3:
                    return UnE.Control.CCTVTypes.XpressStrm;
                case 4:
                    return UnE.Control.CCTVTypes.UDP;
                case 5:
                    return UnE.Control.CCTVTypes.Panasonic;
                case 6:
                    return UnE.Control.CCTVTypes.TechWin;
                case 7:
                    return UnE.Control.CCTVTypes.IPVideo;
                case 8:
                    return UnE.Control.CCTVTypes.HIK;
                case 9:
                    return UnE.Control.CCTVTypes.NVT;
            }
            return UnE.Control.CCTVTypes.NotSet;
        }*/

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
        
        private void CreateProcess()
        {

            try
            {
                if (m_CCTVProcess != null && m_CCTVProcess.HasExited == false)
                    KillProcess();

                Guid guid = Guid.NewGuid();
                string szName = string.Format("CCTVViewer{0}", guid.ToString());
                string args = string.Format("{0} {1} {2} {3}", this.Handle, szName, UnE.SOP.ProxySOP.Instance.SiteID, m_cctv.ID);


                string szDir = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
                string szFileName = szDir + "\\" + "CCTVViewer.exe";

                if (File.Exists(szFileName))
                {
                    m_CCTVProcess = StartPocess(szFileName, szDir, args);

                    IntPtr ptr = UnE.Win32.NativeMethods.FindWindowEx(this.Handle, IntPtr.Zero, null, szName);

                    info.FormProcess = m_CCTVProcess;
                    info.HWnd = ptr;
                    info.Name = szName;
                }
                else
                {
                    szDir = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
                    szFileName = szDir + "\\common\\" + "CCTVViewer.exe";

                    m_CCTVProcess = StartPocess(szFileName, szDir, args);

                    IntPtr ptr = UnE.Win32.NativeMethods.FindWindowEx(this.Handle, IntPtr.Zero, null, szName);
                    info.FormProcess = m_CCTVProcess;
                    info.HWnd = ptr;
                    info.Name = szName;
                }            
            }catch(Exception)
            {
                m_CCTVProcess = null;
            }

           
        }

        private void KillProcess()
        {
            Process p = m_CCTVProcess;
            
            if (p!= null && p.HasExited == false)
            {
                try
                {
                    p.Kill();
                }
                catch (Exception)
                {
                }
            }
            m_CCTVProcess = null;
        }

        private FormHandleInfo info = new FormHandleInfo();

		protected virtual void LoadCamera()
		{
			//lock (m_cctv)
			{
				if (m_cctv == null)
					return;
                CreateProcess();   
            
                if( m_CCTVProcess != null && m_CCTVProcess.HasExited == false)
                {
                    m_isConnected = true;
                }

                mCheckTimer.Interval = 1000;
                mCheckTimer.Tick += CCTVCheckTimer_Tick;
                mCheckTimer.Enabled = true;
                mCheckTimer.Start();

                mCheckTimer2.Interval = 3000;
                mCheckTimer2.Tick += CCTVCheckTimer2_Tick;
                mCheckTimer2.Enabled = true;
                mCheckTimer2.Start();

			}
		}

        void CCTVCheckTimer_Tick(object sender, EventArgs e)
        {
            Process p = m_CCTVProcess;
            if (p != null && p.HasExited == false)
            {
                try
                {
                    IntPtr hWnd = UnE.Win32.NativeMethods.FindWindowEx(this.Handle, IntPtr.Zero, null, info.Name);
                    if (hWnd != IntPtr.Zero)
                    {
                        mCheckTimer.Enabled = false;
                        mCheckTimer.Stop();

                        info.HWnd = hWnd;
                        BigCCTVCtrl_SizeChanged(null, null);
                    }
                    else
                    {
                        BigCCTVCtrl_SizeChanged(null, null);
                    }
                }
                catch(Exception)
                {
                    mCheckTimer.Enabled = false;
                    mCheckTimer.Stop();
                }                
            }
        }

        private int checkCount = 0;
        void CCTVCheckTimer2_Tick(object sender, EventArgs e)
        {
            Process p = m_CCTVProcess;
            if (p != null && p.HasExited == false)
            {
                try
                {
                    IntPtr hWnd = UnE.Win32.NativeMethods.FindWindowEx(this.Handle, IntPtr.Zero, null, info.Name);
                    if (hWnd != IntPtr.Zero)
                    {
                        if (checkCount == 200)
                        {
                            mCheckTimer2.Enabled = false;
                            mCheckTimer2.Stop();
                            checkCount = 0;
                        }
                       

                        info.HWnd = hWnd;
                        BigCCTVCtrl_SizeChanged(null, null);

                        checkCount++;
                    }
                    else
                    {
                        BigCCTVCtrl_SizeChanged(null, null);
                    }
                }
                catch (Exception ex)
                {

                    mCheckTimer2.Stop();
                    mCheckTimer2.Enabled = false;
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                }

            }
        }

        private Timer mCheckTimer = new Timer();

        private Timer mCheckTimer2 = new Timer();

		protected virtual void LoadCameraThread()
		{
			LoadCamera();
		}

		protected virtual void CloseCamera(bool useThread = true)
		{
			if (useThread)
			{
                CloseCameraThread();
                if (m_cctv != null)
                {
                    //this.Invoke(
                    //   new Action(() => SetTitle(String.Format("{0} - {1}", m_cctv.ID, m_cctv.AccessKey)))
                    //);
                }
                else
                {
                    this.Invoke(
                       new Action(() => SetTitle("CCTV정보 없음"))
                    );
                }
			}
			else
			{
                KillProcess();

                this.Invoke(
                   new Action(() => SetTitle("CCTV정보 없음"))
                );

				m_isConnected = false;
			}
		}

		protected virtual void CloseCameraThread()
		{
            KillProcess();

			m_isConnected = false;

			if (m_cctvQueue != null)
			{
				m_cctv = m_cctvQueue;

				m_cctvQueue = null;

				LoadCamera();

			}
			else
				m_cctv = m_cctvQueue;
		}


		private void BigCCTVCtrl_FormClosing(object sender, FormClosingEventArgs e)
		{
			m_isClosing = true;

            if (m_bLargeMode == true)
            {
                BigCCTVCtrl_MouseDoubleClick(null, null);
            }

			if (m_cctv != null)
				CloseCamera();

            KillProcess();
		}

		private void BigCCTVCtrl_Resize(object sender, EventArgs e)
		{

		}

        private void BigCCTVCtrl_SizeChanged(object sender, EventArgs e)
        {
            int width = this.Width - 2;
            int height = this.Height - 2;
            if (width <= 0 || height <= 0)
                return;

            try
            {
                Process p = m_CCTVProcess;
                if (p != null && p.HasExited == false)
                {
                    IntPtr hWnd = UnE.Win32.NativeMethods.FindWindowEx(this.Handle, IntPtr.Zero, null, info.Name);
                    if (hWnd != IntPtr.Zero)
                    {
                        this.Invoke(new Action(() =>
                        {
                            System.Diagnostics.Trace.WriteLine("resize cctv :  " + lbTitle.Text);
                            UnE.Win32.NativeMethods.MoveWindow(hWnd, 1, 1, width, height, true);
                            Control c = Form.FromHandle(hWnd);
                            if (c != null)
                                c.Refresh();
                            if (this.Parent != null)
                                this.Parent.Location = this.Parent.Location;
                        }));
                    }
                }
            }
            catch(Exception)
            {

            }
            
        }        

		private void BigCCTVCtrl_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				m_frmParent.OnSelectCCTV(this);
			}
			else if (e.Button == System.Windows.Forms.MouseButtons.Middle)
			{
				if (m_cctv == null)
					MessageBox.Show("m_cctv is null");
				else if (m_isConnected)
					MessageBox.Show("CCTV is connected");
				else
					MessageBox.Show("CCTV is disconnected");
			}
		}

        private Size mSaveSize = new Size();
        private Point mSaveLoc = new Point();
        private bool m_bLargeMode = false;
        private int m_nLineThick = 5;

        private static int WM_LBUTTONDOWN = 0x201; //Left mousebutton down
        private static int WM_LBUTTONDBLCLK = 0x203; //Left mousebutton doubleclick
       
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_LBUTTONDBLCLK)
            {
                BigCCTVCtrl_MouseDoubleClick(null, null);
            }
            else if (m.Msg == WM_LBUTTONDOWN)
            {
                System.Diagnostics.Trace.WriteLine("LButtonClick");
            }

            base.WndProc(ref m);
        }

        private System.Windows.Forms.Control mParentContorl = null;
        private System.Windows.Forms.Control mParentForm = null;
        private void BigCCTVCtrl_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (Parent == null)
                return;

            if (m_bLargeMode == false)
            {
                int d = m_nLineThick * 2;
                m_bLargeMode = true;
                mSaveSize = this.Size;
                mSaveLoc = this.Location;
                mParentContorl = this.Parent;
                mParentForm = this.Parent.Parent;

                if(UnE.SOP.ProxySOP.Instance.ShowCCTVForm == true)
                {
                    mParentContorl.Controls.Remove(this);
                    mParentForm.Controls.Add(this);
                    this.Location = new Point(m_nLineThick, m_nLineThick);
                    this.Size = new Size(Parent.Parent.Width - d, Parent.Parent.Height - d);

                }
                else
                {
                    this.Location = new Point(m_nLineThick, m_nLineThick);
                    this.Size = new Size(Parent.Width - d, Parent.Height - d);
                }
               
                this.BringToFront();
            }
            else
            {
                if (UnE.SOP.ProxySOP.Instance.ShowCCTVForm == true)
                {
                    mParentForm.Controls.Remove(this);
                    mParentContorl.Controls.Add(this);
                }
                m_bLargeMode = false;
                this.Size = mSaveSize;
                this.Location = mSaveLoc;
            }

            BigCCTVCtrl_SizeChanged(null, null);
        }

		private void EnableControl(bool enabled)
		{
		}

		private void BigCCTVCtrl_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete)
			{
				m_frmParent.RemoveCCTV();
			}
		}

		protected virtual void BigCCTVCtrl_Shown(object sender, EventArgs e)
		{
		}

		public void SetCCTVMode(CCTVMode mode)
		{

		}

        public void OnMouseLButtonDoubleClick()
        {
            BigCCTVCtrl_MouseDoubleClick(null, null);
        }

        public void OnMouseLButtonClick()
        {
            m_frmParent.OnSelectCCTV(this);
        }

        protected virtual void ChangeCCTVSize(int nWidth, int nHeight)
        {
        }

        protected virtual void OnCommandButtonDown(object sender, EventArgs e)
        {
        }

        protected virtual void OnCommandButtonUp(object sender, MouseEventArgs e)
        {
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