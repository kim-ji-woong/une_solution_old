using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;

namespace UnE.CCTV
{
    public partial class BigCCTVCtrl : Form, ICCTVControl
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

        private const int SW_HIDE = 0;
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOW = 5;

        private const int SWP_NOSIZE = 0x0001;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOZORDER = 0x0004;
        private const int SWP_NOREDRAW = 0x0008;
        private const int SWP_NOACTIVATE = 0x0010;
        private const int SWP_FRAMECHANGED = 0x0020;  /* The frame changed: send WM_NCCALCSIZE */
        private const int SWP_SHOWWINDOW = 0x0040;
        private const int SWP_HIDEWINDOW = 0x0080;
        private const int SWP_NOCOPYBITS = 0x0100;
        private const int SWP_NOOWNERZORDER = 0x0200;  /* Don't do owner Z ordering */
        private const int SWP_NOSENDCHANGING = 0x0400;  /* Don't send WM_WINDOWPOSCHANGING */


        private static int WM_SYSKEYDOWN = 0x0104;
        private static int WM_CHAR = 0x0102;
        private static int WM_KEYDOWN = 0x0100;
        private static int WM_KEYUP = 0x101;  //Key up

        public const string NO_CCTV_INFO = "CCTV정보 없음";

        public enum VKeys : int
        {
            VK_LBUTTON = 0x01,  //Left mouse button
            VK_RBUTTON = 0x02,  //Right mouse button
            VK_CANCEL = 0x03,  //Control-break processing
            VK_MBUTTON = 0x04,  //Middle mouse button (three-button mouse)
            VK_BACK = 0x08,  //BACKSPACE key
            VK_TAB = 0x09,  //TAB key
            VK_CLEAR = 0x0C,  //CLEAR key
            VK_RETURN = 0x0D,  //ENTER key
            VK_SHIFT = 0x10,  //SHIFT key
            VK_CONTROL = 0x11,  //CTRL key
            VK_MENU = 0x12,  //ALT key
            VK_PAUSE = 0x13,  //PAUSE key
            VK_CAPITAL = 0x14,  //CAPS LOCK key
            VK_ESCAPE = 0x1B,  //ESC key
            VK_SPACE = 0x20,  //SPACEBAR
            VK_PRIOR = 0x21,  //PAGE UP key
            VK_NEXT = 0x22,  //PAGE DOWN key
            VK_END = 0x23,  //END key
            VK_HOME = 0x24,  //HOME key
            VK_LEFT = 0x25,  //LEFT ARROW key
            VK_UP = 0x26,  //UP ARROW key
            VK_RIGHT = 0x27,  //RIGHT ARROW key
            VK_DOWN = 0x28,  //DOWN ARROW key
            VK_SELECT = 0x29,  //SELECT key
            VK_PRINT = 0x2A,  //PRINT key
            VK_EXECUTE = 0x2B,  //EXECUTE key
            VK_SNAPSHOT = 0x2C,  //PRINT SCREEN key
            VK_INSERT = 0x2D,  //INS key
            VK_DELETE = 0x2E,  //DEL key
            VK_HELP = 0x2F,  //HELP key
            VK_0 = 0x30,  //0 key
            VK_1 = 0x31,  //1 key
            VK_2 = 0x32,  //2 key
            VK_3 = 0x33,  //3 key
            VK_4 = 0x34,  //4 key
            VK_5 = 0x35,  //5 key
            VK_6 = 0x36,  //6 key
            VK_7 = 0x37,  //7 key
            VK_8 = 0x38,  //8 key
            VK_9 = 0x39,  //9 key
            VK_A = 0x41,  //A key
            VK_B = 0x42,  //B key
            VK_C = 0x43,  //C key
            VK_D = 0x44,  //D key
            VK_E = 0x45,  //E key
            VK_F = 0x46,  //F key
            VK_G = 0x47,  //G key
            VK_H = 0x48,  //H key
            VK_I = 0x49,  //I key
            VK_J = 0x4A,  //J key
            VK_K = 0x4B,  //K key
            VK_L = 0x4C,  //L key
            VK_M = 0x4D,  //M key
            VK_N = 0x4E,  //N key
            VK_O = 0x4F,  //O key
            VK_P = 0x50,  //P key
            VK_Q = 0x51,  //Q key
            VK_R = 0x52,  //R key
            VK_S = 0x53,  //S key
            VK_T = 0x54,  //T key
            VK_U = 0x55,  //U key
            VK_V = 0x56,  //V key
            VK_W = 0x57,  //W key
            VK_X = 0x58,  //X key
            VK_Y = 0x59,  //Y key
            VK_Z = 0x5A,  //Z key
            VK_NUMPAD0 = 0x60,  //Numeric keypad 0 key
            VK_NUMPAD1 = 0x61,  //Numeric keypad 1 key
            VK_NUMPAD2 = 0x62,  //Numeric keypad 2 key
            VK_NUMPAD3 = 0x63,  //Numeric keypad 3 key
            VK_NUMPAD4 = 0x64,  //Numeric keypad 4 key
            VK_NUMPAD5 = 0x65,  //Numeric keypad 5 key
            VK_NUMPAD6 = 0x66,  //Numeric keypad 6 key
            VK_NUMPAD7 = 0x67,  //Numeric keypad 7 key
            VK_NUMPAD8 = 0x68,  //Numeric keypad 8 key
            VK_NUMPAD9 = 0x69,  //Numeric keypad 9 key
            VK_SEPARATOR = 0x6C,  //Separator key
            VK_SUBTRACT = 0x6D,  //Subtract key
            VK_DECIMAL = 0x6E,  //Decimal key
            VK_DIVIDE = 0x6F,  //Divide key
            VK_F1 = 0x70,  //F1 key
            VK_F2 = 0x71,  //F2 key
            VK_F3 = 0x72,  //F3 key
            VK_F4 = 0x73,  //F4 key
            VK_F5 = 0x74,  //F5 key
            VK_F6 = 0x75,  //F6 key
            VK_F7 = 0x76,  //F7 key
            VK_F8 = 0x77,  //F8 key
            VK_F9 = 0x78,  //F9 key
            VK_F10 = 0x79,  //F10 key
            VK_F11 = 0x7A,  //F11 key
            VK_F12 = 0x7B,  //F12 key            
            VK_CUSTOM = 0x88, // Unassigned key
            VK_SCROLL = 0x91,  //SCROLL LOCK key
            VK_LSHIFT = 0xA0,  //Left SHIFT key
            VK_RSHIFT = 0xA1,  //Right SHIFT key
            VK_LCONTROL = 0xA2,  //Left CONTROL key
            VK_RCONTROL = 0xA3,  //Right CONTROL key
            VK_LMENU = 0xA4,   //Left MENU key
            VK_RMENU = 0xA5,  //Right MENU key
            VK_PLAY = 0xFA,  //Play key
            VK_ZOOM = 0xFB, //Zoom key
        }

		private CCTV m_cctv = null;
		private bool m_isConnected = false;
		private int m_nBtnLeftPos = 0;
		private int m_nLeftSpace = 0;
		private bool m_isSelected = false;
		private Form4CCTV m_frmParent = null;
		private CCTV m_cctvQueue = null;

		private bool m_isValidCamera = false;
		private bool m_isClosing = false;
        private bool m_isLoading = false;
		//private System.IO.StreamWriter m_logger = null;

        private Process m_CCTVProcess = null;

        private int m_nPositionIndex = -1;
        private VariousData<DateTime> m_dtConnection = null;
        private bool m_pause = false;

        // 접속시 시작되었을 당시의 시간
        // 접속중이지 않으면 null을 리턴한다.
        public VariousData<DateTime> ConnectionTime
        {
            get { return m_dtConnection; }
        }

        public int PositionIndex
        {
            get { return m_nPositionIndex; }
            set { m_nPositionIndex = value; }
        }

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

                        if (m_isLoading)
                        {
                            LoadCamera();

                            if (this.IsDisposed == false && this.IsHandleCreated == true)
                            {

                                this.Invoke(
                                 new Action(() => SetTitle(String.Format("{0} - {1}", m_cctv.ID, m_cctv.AccessKey)))
                                 );
                            }
                            else
                            {
                                MessageBox.Show("CCTVCrtl Disposed!, Check System");
                            }
                            BigCCTVCtrl_SizeChanged(null, null);
                        }
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
                {
                    this.BackColor = Color.FromArgb(109, 155, 206);
                    this.lbTitle.ForeColor = Color.Orange;

                    if (info != null && info.HWnd != IntPtr.Zero)
                        SendMessage(info.HWnd, WM_KEYDOWN, (IntPtr)130, IntPtr.Zero);
                }
                else
                {
                    this.BackColor = System.Windows.Forms.Control.DefaultBackColor;
                    this.lbTitle.ForeColor = Color.White;
                    if (info != null && info.HWnd != IntPtr.Zero)
                        SendMessage(info.HWnd, WM_KEYDOWN, (IntPtr)129, IntPtr.Zero);
                }
            }
        }

        public void SetPreset(int nType)
        {
            if (nType == 1)
            {   
                if (info != null && info.HWnd != IntPtr.Zero) // F14 - Fire
                    SendMessage(info.HWnd, WM_KEYDOWN, (IntPtr)125, IntPtr.Zero);
            }
            else
            {                
                if (info != null && info.HWnd != IntPtr.Zero) // F15 - PSM
                    SendMessage(info.HWnd, WM_KEYDOWN, (IntPtr)126, IntPtr.Zero);
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
            m_isLoading = true;

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
            // Process 생성 실패를 대비하여 저장해 둔다.
            VariousData<DateTime> dtConnection = m_dtConnection == null ? null : new VariousData<DateTime>(m_dtConnection.Data);

            try
            {
                if (m_CCTVProcess != null && m_CCTVProcess.HasExited == false)
                    KillProcess();

                IntPtr handle = IntPtr.Zero;

                this.Invoke((MethodInvoker)delegate
                {
                    handle = this.Handle;
                });

                Guid guid = Guid.NewGuid();
                string szName = string.Format("CCTVViewer{0}", guid.ToString());
                int EquipZoneID = -1;
                if(ProxyCCTV.Instance.CurrentEquipZone != null)
                    EquipZoneID = ProxyCCTV.Instance.CurrentEquipZone.ID;

                string args = string.Format("{0} {1} {2} {3} {4} {5} {6}", handle, szName, UnE.SOP.ProxySOP.Instance.SiteID, m_cctv.ID, m_nPositionIndex
                    , ProxyCCTV.Instance.EquipZoneCCTVMode, EquipZoneID);

                string szDir = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
                string szFileName = szDir + "\\" + "CCTVViewer.exe";

                if (File.Exists(szFileName))
                {
                    m_CCTVProcess = StartPocess(szFileName, szDir, args);

                    IntPtr ptr = FindWindowEx(handle, IntPtr.Zero, null, szName);

                    info.FormProcess = m_CCTVProcess;
                    info.HWnd = ptr;
                    info.Name = szName;
                }
                else
                {
                    szDir = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
                    szFileName = szDir + "\\common\\" + "CCTVViewer.exe";

                    m_CCTVProcess = StartPocess(szFileName, szDir, args);

                    IntPtr ptr = FindWindowEx(handle, IntPtr.Zero, null, szName);
                    info.FormProcess = m_CCTVProcess;
                    info.HWnd = ptr;
                    info.Name = szName;
                }

                m_dtConnection = new VariousData<DateTime>(DateTime.Now);                
            }
            catch(Exception)
            {
                m_CCTVProcess = null;
                m_dtConnection = dtConnection;
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

            if (m_pause == false)
                m_dtConnection = null;
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

                this.Invoke((MethodInvoker)delegate
                {
                    mCheckTimer.Interval = 1000;
                    mCheckTimer.Tick += CCTVCheckTimer_Tick;
                    mCheckTimer.Enabled = true;
                    mCheckTimer.Start();
                });

                /*mCheckTimer2.Interval = 3000;
                mCheckTimer2.Tick += CCTVCheckTimer2_Tick;
                mCheckTimer2.Enabled = true;
                mCheckTimer2.Start();*/
			}
		}

        public void Reload()
        {
            LoadCamera();
        }

        void CCTVCheckTimer_Tick(object sender, EventArgs e)
        {
            Process p = m_CCTVProcess;
            if (p != null && p.HasExited == false)
            {
                try
                {
                    IntPtr hWnd = FindWindowEx(this.Handle, IntPtr.Zero, null, info.Name);
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

        /*private int checkCount = 0;
        void CCTVCheckTimer2_Tick(object sender, EventArgs e)
        {
            Process p = m_CCTVProcess;
            if (p != null && p.HasExited == false)
            {
                try
                {
                    IntPtr hWnd = FindWindowEx(this.Handle, IntPtr.Zero, null, info.Name);
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
        }*/

        private Timer mCheckTimer = new Timer();

        //private Timer mCheckTimer2 = new Timer();

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
                       new Action(() => SetTitle(NO_CCTV_INFO))
                    );
                }
			}
			else
			{
                KillProcess();

                this.Invoke(
                   new Action(() => SetTitle(NO_CCTV_INFO))
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
            {
                CloseCamera();
            }

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
                    IntPtr hWnd = FindWindowEx(this.Handle, IntPtr.Zero, null, info.Name);
                    if (hWnd != IntPtr.Zero)
                    {
                        this.Invoke(new Action(() =>
                        {
                            //System.Diagnostics.Trace.WriteLine("resize cctv :  " + lbTitle.Text);
                            MoveWindow(hWnd, 1, 1, width, height, true);
                            System.Windows.Forms.Control c = Form.FromHandle(hWnd);
                            if (c != null)
                                c.Refresh();
                            if (this.Parent != null)
                                this.Parent.Location = this.Parent.Location;
                        }));
                    }
                }
            }
            catch(Exception ex)
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

        private bool m_enableLButtonDoubleClickEvent = true;

        private static int WM_LBUTTONDOWN = 0x201; //Left mousebutton down
        private static int WM_LBUTTONDBLCLK = 0x203; //Left mousebutton doubleclick

        private static int WM_USER = 0x400;
        private static int ENABLE_DOUBLE_CLICK_EVENT = WM_USER + 1;

        //int count = 0;

        public bool LargeMode
        {
            get { return m_bLargeMode; }
            set { m_bLargeMode = value; }
        }

        public int LineThick
        {
            get { return m_nLineThick; }
            set { m_nLineThick = value; }
        }

        public Size SaveSize
        {
            get { return mSaveSize; }
            set { mSaveSize = value; }
        }

        public Point SaveLoc
        {
            get { return mSaveLoc; }
            set { mSaveLoc = value; }
        }

        public System.Windows.Forms.Control ParentControl
        {
            get { return mParentContorl; }
            set
            { 
                mParentContorl = value; 
            }
        }

        public System.Windows.Forms.Control ParentForm
        {
            get { return mParentForm; }
            set 
            {
                mParentForm = value; 
            }
        }

        public System.Windows.Forms.Control ThisControl
        {
            get { return this; }
        }

        public bool EnableDoubleClickEvent
        {
            get { return m_enableLButtonDoubleClickEvent; }
        }
       
        private void SetDoubleClickEvent(int nOption)
        {
            m_enableLButtonDoubleClickEvent = nOption == 1;
        }

        private System.Windows.Forms.Control mParentContorl = null;
        private System.Windows.Forms.Control mParentForm = null;
        private void BigCCTVCtrl_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (m_frmParent == null)
                return;

            m_frmParent.OnMouseDoubleClick(this);
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
            //System.Diagnostics.Trace.WriteLine("LLLL");
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

        private void lbTitle_Click(object sender, EventArgs e)
        {
            m_frmParent.OnSelectCCTV(this);
        }

        private void lbTitle_DoubleClick(object sender, EventArgs e)
        {
            BigCCTVCtrl_MouseDoubleClick(this, null);
        }

        public void Pause()
        {
            m_pause = true;
            KillProcess();
        }

        public void Resume()
        {
            m_pause = false;

            if (m_dtConnection != null)
                Reload();
        }

        public bool IsAlive()
        {
            if (m_CCTVProcess == null)
                return false;

            return !m_CCTVProcess.HasExited;
        }

        private void BigCCTVCtrl_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text) == true)
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;
        }

        private void BigCCTVCtrl_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                String strCCTVInfo = (String)e.Data.GetData(DataFormats.Text);
                CCTV _cctv = CCTVManager.Instance.GetCCTV(Int32.Parse(strCCTVInfo.Split(',')[1]));
                if (_cctv == null) return;
                
                //m_cctv = _cctv;
                //LoadCamera();
                //CCTV = _cctv;

                FormMain.Instance.SetCCTV(_cctv, m_nPositionIndex);
                FormMain.Instance.PipeServer_SendCCTV(m_cctv);
            }
            catch
            {
            }
        }

        private static int WM_COPYDATA = 0x004A;
        private static int WM_RBUTTONUP = 0x205;  //Right mousebutton up

        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public UInt32 cbData;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_LBUTTONDBLCLK)
            {
                BigCCTVCtrl_MouseDoubleClick(null, null);
            }
            else if (m.Msg == WM_LBUTTONDOWN)
            {
                //System.Diagnostics.Trace.WriteLine("LButtonClick" + (count++).ToString());                    
            }
            else if (m.Msg == WM_RBUTTONUP)
            {
            }
            else if (m.Msg == ENABLE_DOUBLE_CLICK_EVENT)
            {
                SetDoubleClickEvent(m.LParam.ToInt32());
            }
            else if (m.Msg == WM_COPYDATA)
            {
                if (ProxyCCTV.Instance.EquipZoneCCTVMode == true)
                {
                    COPYDATASTRUCT cds = new COPYDATASTRUCT();
                    cds = (COPYDATASTRUCT)m.GetLParam(cds.GetType());

                    int index = ChangeEquipZoneIndex(m_nPositionIndex);
                    ProxyCCTV.Instance.CurrentEquipZone.Preset[index] = cds.lpData;
                }
            }
            else if (m.Msg == WM_CHAR)
            {
                //CCTVViewer.exe 에서 받은 정보를 통해 내부 CCTV 정보 갱신 및 Pipe 통신 진행
                if ((int)m.WParam == (int)VKeys.VK_INSERT)
                {
                    CCTV _cctv = CCTVManager.Instance.GetCCTV(m.LParam.ToInt32());
                    if (_cctv != null)
                    {
                        m_cctv = _cctv;
                        FormMain.Instance.SetCCTV(m_cctv, m_nPositionIndex);
                        //Pipe
                        FormMain.Instance.PipeServer_SendCCTV(m_cctv); 
                    }
                }
                else if ((int)m.WParam == (int)VKeys.VK_DELETE)
                {
                    if (ProxyCCTV.Instance.EquipZoneCCTVMode == true)
                    {
                        int index = ChangeEquipZoneIndex(m_nPositionIndex);
                        ProxyCCTV.Instance.CurrentEquipZone.Preset[index] = "";
                    }

                    FormMain.Instance.SelectCCTV(m_nPositionIndex);
                    CloseCamera();
                    FormMain.Instance.SetCCTV(null, m_nPositionIndex);
                }
            }

            base.WndProc(ref m);
        }

        /// <summary>CCTV Index를 EquipZoneCCTV 기준 Index로 변환</summary>
        /// <param name="pPositionIndex">TL : 0 , TM : 1, TR : 2, BL : 3, BM : 4, BR : 5</param>
        /// <returns>TM : 0, BM : 1, BR : 2, TR : 3, TL : 4, BL : 5</returns>
        private int ChangeEquipZoneIndex(int pPositionIndex)
        {
            if (pPositionIndex == 0) return 4;
            else if (pPositionIndex == 1) return 0;
            else if (pPositionIndex == 2) return 3;
            else if (pPositionIndex == 3) return 5;
            else if (pPositionIndex == 4) return 1;
            else if (pPositionIndex == 5) return 2;
            else return -1;
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