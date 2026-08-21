using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace UnE.CCTV
{    
    public partial class BigCCTVCtrl : Form, UnE.Control.ICCTVCtrlOwner
	{
        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

		private CCTV m_cctv = null;
		private bool m_isConnected = false;
		private bool m_isSelected = false;
		private CCTV m_cctvQueue = null;

        public bool IsConnected
		{
			get { return m_isConnected; }
		}

        private static BigCCTVCtrl m_Instance = null;
        public static BigCCTVCtrl Instance
        {
            get { return BigCCTVCtrl.m_Instance; }
        }


        CCTVLoader mLoader = null;

        public CCTVLoader CCTVLoader
        {
            get { return mLoader; }
            set { mLoader = value; }
        }

        private IntPtr m_hParent = IntPtr.Zero;

		public CCTV CCTV
		{
			get { return m_cctv; }
			set
			{
				if (m_cctv != value)
				{
					if (m_cctv != null)
					{
						m_cctvQueue = value;
						CloseCamera();
					}
					else
					{
                        
						m_cctv = value;
                        
						
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
                    //this.BackColor = Color.FromArgb(109, 155, 206);
                    this.lbTitle.ForeColor = Color.Orange;
                }
                else
                {
                    //this.BackColor = System.Windows.Forms.Control.DefaultBackColor;
                    this.lbTitle.ForeColor = Color.White;
                }
			}
		}

		private static int m_nCCTVCount = 0;
		protected int m_nID = -1;

		public UnE.Control.CCTVCtrl CCTVCtrl
		{
            get { return cctvCtrl1; }
		}

		protected Button LeftButton
		{
			get { return btnLeft; }
		}

		protected Button RightButton
		{
			get { return btnRight; }
		}
        
		public BigCCTVCtrl(CCTV cctv, IntPtr frmParent)
		{
            m_Instance = this;

			m_nID = ++m_nCCTVCount;

            if (cctv != null)
                this.cctvCtrl1 = new UnE.Control.CCTVCtrl((UnE.Control.CCTVTypes)cctv.CCTVType);
            else
                this.cctvCtrl1 = new UnE.Control.CCTVCtrl();

			InitializeComponent();

            lbTitle.Parent = this.cctvCtrl1;
            lbTitle.BackColor = Color.Black;
            lbTitle.BringToFront();

			CCTV = cctv;

            btnPTZ.Parent = this.cctvCtrl1;
            btnPTZ.BringToFront();

            panelPTZ.Visible = false;

            
            if (cctv.ReversePTZ < 0)
                this.btnPTZ.Visible = false;
		}

		protected virtual void BigCCTVCtrl_Load(object sender, EventArgs e)
		{
            Connect();
		}

        public void Connect()
        {
            if (m_cctv != null)
            {
                LoadCamera();
                //System.Threading.Thread thread = new System.Threading.Thread(LoadCamera);
                //thread.Start();
            }

            if (m_cctv != null && cctvCtrl1 != null && cctvCtrl1.EnablePTZ)
            {
                // PTZ를 제어할 수 없다면 PTZ 제어버튼을 보이지 않도록 한다.
                // [2018-03-26] 김지웅
                this.btnPTZ.Visible = true;
                btnPTZ.BringToFront();
            }
            else
            {
                this.btnPTZ.Visible = false;
                //btnPTZ.BringToFront();
            }
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
                case 10:
                    return UnE.Control.CCTVTypes.MediaPlayer;
                case 11:
                    return Control.CCTVTypes.IDIS;
                case 12:
                    return Control.CCTVTypes.RTSP;
            }
            return UnE.Control.CCTVTypes.NotSet;
        }*/

		protected virtual void LoadCamera()
		{
			if (m_cctv == null)
				return;

            try
            {
                cctvCtrl1.CCTVOwner = this;

                if (cctvCtrl1.CCTVType == UnE.Control.CCTVTypes.None || (int)cctvCtrl1.CCTVType != m_cctv.CCTVType)
                {
                    this.Controls.Remove(cctvCtrl1);
                    this.cctvCtrl1 = new UnE.Control.CCTVCtrl((UnE.Control.CCTVTypes)m_cctv.CCTVType);
                    cctvCtrl1.Visible = true;
                    this.cctvCtrl1.Location = new System.Drawing.Point(0, 0);
                    this.cctvCtrl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
                    this.cctvCtrl1.Dock = DockStyle.Fill;
                    this.Controls.Add(cctvCtrl1);
                }

                cctvCtrl1.AddProperty("MediaType", "rtp-tcp");
                cctvCtrl1.AddProperty("Channel", m_cctv.Channel.ToString());
                cctvCtrl1.AddProperty("Stream", m_cctv.Stream.ToString());
                cctvCtrl1.AddProperty("HttpPort", m_cctv.HttpPort.ToString());
                cctvCtrl1.AddProperty("IPAddress", m_cctv.IPAddress);
                cctvCtrl1.AddProperty("Port", m_cctv.PortNo.ToString());
                cctvCtrl1.AddProperty("UserName", m_cctv.UserName);
                cctvCtrl1.AddProperty("Password", m_cctv.Password);
                cctvCtrl1.AddProperty("ReversePTZ", m_cctv.ReversePTZ.ToString());
                cctvCtrl1.AddProperty("AccessKey", m_cctv.AccessKey.ToString());
                cctvCtrl1.AddProperty("URL", m_cctv.URL);
               
            }
            catch(Exception ex)
            {
                //BigCCTVCtrl.Instance.Invoke((MethodInvoker)delegate
                //{
                //    MessageBox.Show("" + ex.Message + " " + ex.StackTrace);

                //}); 
            }

            try
            {
                if (cctvCtrl1.IsConnected == false)
                    cctvCtrl1.Connect();
            }
            catch (Exception ex)
            {
                //BigCCTVCtrl.Instance.Invoke((MethodInvoker)delegate
                //{
                //    MessageBox.Show("" + ex.Message + " " + ex.StackTrace);

                //}); 
            }

            if (cctvCtrl1.EnablePTZ)
                btnPTZ.Visible = true;
            else
                btnPTZ.Visible = false;
            
            BigCCTVCtrl.Instance.Invoke((MethodInvoker)delegate
            {
                try
                {
                
                    cctvCtrl1.Visible = true;
                    lbTitle.Text = String.Format("{0} - {1}", m_cctv.ID, m_cctv.AccessKey);
                    lbTitle.Parent = this.cctvCtrl1;
                    lbTitle.BringToFront();

                    btnPTZ.Parent = this.cctvCtrl1;
                    btnPTZ.BringToFront();
                }
                catch(Exception ex)
                {
                    //BigCCTVCtrl.Instance.Invoke((MethodInvoker)delegate
                    //{
                    //    MessageBox.Show("" + ex.Message + " " + ex.StackTrace);

                    //}); 
                }
                
            }); 
		}

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
                    this.Invoke(
                       new Action(() => SetTitle(String.Format("{0} - {1}", m_cctv.ID, m_cctv.AccessKey)))
                    );
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
                cctvCtrl1.Disconnect();

                this.Invoke(
                   new Action(() => SetTitle("CCTV정보 없음"))
                );

				m_isConnected = false;
			}
		}

		protected virtual void CloseCameraThread()
		{
            cctvCtrl1.Disconnect();
			m_isConnected = false;

			if (m_cctvQueue != null)
			{
				m_cctv = m_cctvQueue;
                if (cctvCtrl1.CCTVType == UnE.Control.CCTVTypes.None ||(int)cctvCtrl1.CCTVType != m_cctv.CCTVType)
                {
                    this.Controls.Remove(cctvCtrl1);
                    //cctvCtrl1.Dispose();
                    //cctvCtrl1 = null;

                    this.cctvCtrl1 = new UnE.Control.CCTVCtrl((UnE.Control.CCTVTypes)m_cctv.CCTVType);
                    cctvCtrl1.Visible = true;
                    this.cctvCtrl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
          | System.Windows.Forms.AnchorStyles.Left)
          | System.Windows.Forms.AnchorStyles.Right)));
                    this.cctvCtrl1.Location = new System.Drawing.Point(0, 0);
                    this.cctvCtrl1.Dock = DockStyle.Fill;
                    //ChangeCCTVSize(nLeftPos - m_nLeftSpace, Size.Height);
                    this.Controls.Add(cctvCtrl1);

                    lbTitle.Text = m_cctv.AccessKey;
                    lbTitle.Parent = this.cctvCtrl1;
                    lbTitle.BringToFront();

                    btnPTZ.Parent = this.cctvCtrl1;
                    btnPTZ.BringToFront();
                }
				m_cctvQueue = null;

				LoadCamera();
			}
			else
				m_cctv = m_cctvQueue;
		}

		private void BigCCTVCtrl_FormClosing(object sender, FormClosingEventArgs e)
		{
            if (m_bLargeMode == true)
            {
                BigCCTVCtrl_DoubleClick(null, null);
            }

			if (m_cctv != null)
				CloseCamera();
		}

		private void BigCCTVCtrl_Resize(object sender, EventArgs e)
		{
            Size size = this.Size;
            btnPTZ.Location = new Point(size.Width - btnPTZ.Width - 5, 5);
            panelPTZ.Location = new Point(size.Width - panelPTZ.Width - 5, 5);
            if (cctvCtrl1 != null)
                cctvCtrl1.Refresh();

            if (m_cctv.BigURL.Length > 0 && m_cctv.SmallURL.Length > 0)
            {
                if (m_cctv.BigURL != m_cctv.URL || m_cctv.SmallURL != m_cctv.URL)
                {
                    bool currentIsBig = m_cctv.URL == m_cctv.BigURL;
                    bool targetIsBig = IsBigSizeMode();

                    if (currentIsBig != targetIsBig)
                    {
                        if (targetIsBig)
                            m_cctv.URL = m_cctv.BigURL;
                        else
                            m_cctv.URL = m_cctv.SmallURL;

                        CloseCamera(false);
                        Connect();
                    }
                }
            }
		}

        private bool IsBigSizeMode()
        {
            Point ptWorld = this.PointToScreen(this.Location);

            for (int i = 0; i < Screen.AllScreens.Length; i++)
            {
                Screen screen = Screen.AllScreens[i];

                if (ptWorld.X >= screen.Bounds.Left && ptWorld.X <= screen.Bounds.Right &&
                    ptWorld.Y >= screen.Bounds.Top && ptWorld.Y <= screen.Bounds.Bottom)
                {
                    double screenWidth = screen.Bounds.Width;
                    double ratio = this.Size.Width / screenWidth;

                    if (ratio >= 0.9)
                        return true;
                    else
                        return false;
                }
            }

            return false;
        }

		protected virtual void ChangeCCTVSize(int nWidth, int nHeight)
		{
            cctvCtrl1.Size = new Size(nWidth, nHeight);
		}


        protected virtual void OnCommandButtonDown(object sender, EventArgs e)
		{
            if (sender == btnZoomIn)
                cctvCtrl1.ZoomIn();
            else if (sender == btnZoomOut)
                cctvCtrl1.ZoomOut();
            else if (sender == btnUp)
                cctvCtrl1.MoveUp();
            else if (sender == btnDown)
                cctvCtrl1.MoveDown();
            else if (sender == btnRight)
                cctvCtrl1.MoveRight();
            else if (sender == btnLeft)
                cctvCtrl1.MoveLeft();
            else if (sender == btnStop)
                cctvCtrl1.TestStop(9);
		}

		protected virtual void OnCommandButtonUp(object sender, MouseEventArgs e)
		{
            if( cctvCtrl1 != null && cctvCtrl1.CCTVType == Control.CCTVTypes.XpressStrm)
            {
                cctvCtrl1.TestStop(9);
            }
		}

		private void BigCCTVCtrl_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
                IntPtr pt = MakeLParam(1, 1);
                SendMessage(m_hParent, WM_LBUTTONDOWN, IntPtr.Zero, pt);
			}
			else if (e.Button == System.Windows.Forms.MouseButtons.Middle)
			{
			}
		}

        private Size mSaveSize = new Size();
        private Point mSaveLoc = new Point();
        private bool m_bLargeMode = false;
        private int m_nLineThick = 5;
       
        private void BigCCTVCtrl_DoubleClick(object sender, EventArgs e)
        {
            IntPtr pt = MakeLParam(1, 1);
            
            // Emulated mouse dbclk
            //SendMessage(m_hParent, WM_LBUTTONDOWN, IntPtr.Zero, pt);
            //SendMessage(m_hParent, WM_LBUTTONUP, IntPtr.Zero, pt);
            SendMessage(m_hParent, WM_LBUTTONDBLCLK, IntPtr.Zero, pt);
            SendMessage(m_hParent, WM_LBUTTONUP, IntPtr.Zero, pt);
        }


		private void EnableControl(bool enabled)
		{
			btnLeft.Enabled = btnRight.Enabled = btnUp.Enabled = btnDown.Enabled = enabled;
			btnZoomIn.Enabled = btnZoomOut.Enabled = enabled;
		}

        private static int WM_SYSKEYDOWN = 0x0104;
        private static int WM_CHAR = 0x0102;
        private static int WM_KEYDOWN = 0x0100;
        private static int WM_KEYUP = 0x101;  //Key up

        private static int WM_LBUTTONDOWN = 0x201; //Left mousebutton down
        private static int WM_LBUTTONUP = 0x202; //Left mousebutton up
        private static int WM_LBUTTONDBLCLK = 0x203; //Left mousebutton doubleclick
        private static int WM_RBUTTONDOWN = 0x204; //Right mousebutton down
        private static int WM_RBUTTONUP = 0x205;  //Right mousebutton up
        private static int WM_RBUTTONDBLCLK = 0x206; //Right mousebutton doubleclick
        


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

		private void BigCCTVCtrl_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete)
			{
                SendMessage(m_hParent, WM_CHAR, (IntPtr)VKeys.VK_DELETE, IntPtr.Zero);
			}
            else if( e.KeyCode == Keys.F19)
            {
                IsSelected = true;
            }
            else if( e.KeyCode == Keys.F18)
            {
                IsSelected = false;
            }
            else if (e.KeyCode == Keys.F14)
            {
                //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\UNE\cctvviewer_log_" + this.GetHashCode() + ".log", true))
                //{
                //    file.WriteLine(DateTime.Now.ToLongTimeString() + " : Fire " + lbTitle.Text);
                //}

                if (cctvCtrl1 != null)
                    cctvCtrl1.Preset(1, "Fire");
            }
            else if (e.KeyCode == Keys.F15)
            {
                if (cctvCtrl1 != null)
                    cctvCtrl1.Preset(2, "PSM");

                //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\UNE\cctvviewer_log_" + this.GetHashCode() + ".log", true))
                //{
                 //   file.WriteLine(DateTime.Now.ToLongTimeString() + " : PSM " + lbTitle.Text);
                //}
            }
		}

		protected virtual void BigCCTVCtrl_Shown(object sender, EventArgs e)
		{
		}

        public void OnMouseLButtonDoubleClick()
        {
            BigCCTVCtrl_DoubleClick(null, null);
        }

        public void OnMouseLButtonClick()
        {
            IntPtr pt = MakeLParam(0, 0);
            SendMessage(m_hParent, WM_LBUTTONDOWN,IntPtr.Zero , pt);
            SendMessage(m_hParent, WM_LBUTTONUP, IntPtr.Zero, pt);

            //IsSelected = !IsSelected;
        }

        public IntPtr MakeLParam(int LoWord, int HiWord)
        {
            return (IntPtr)((HiWord << 16) | (LoWord & 0xffff));
        }



        private void cctvCtrl1_Load(object sender, EventArgs e)
        {
            panelPTZ.Visible = false;
        }

        private void btnPTZ_Click(object sender, EventArgs e)
        {
            panelPTZ.BringToFront();
            panelPTZ.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            btnPTZ.BringToFront();
            panelPTZ.Visible = false;
        }

        private void btnUp_Click(object sender, EventArgs e)
        {

        }
        
        public void SetParentHandle(IntPtr hParent)
        {
            m_hParent = hParent;
        }

        private void cctvCtrl1_SizeChanged(object sender, EventArgs e)
        {

        }

        private void BigCCTVCtrl_SizeChanged(object sender, EventArgs e)
        {
            if( cctvCtrl1 != null)
            {
                cctvCtrl1.PerformLayout();
            }

            lbTitle.Refresh();
        }

        private void lbTitle_Click(object sender, EventArgs e)
        {
            OnMouseLButtonClick();
        }

        private void lbTitle_DoubleClick(object sender, EventArgs e)
        {
            OnMouseLButtonDoubleClick();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (cctvCtrl1 != null)
            {
                //cctvCtrl1.Preset("P1");
            }
        }
	}

    public class TitleLable : Label
    {
        public TitleLable()
            : base()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.Transparent;
        }
    }	
}