using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using System.Collections;

namespace UnE.CCTV
{    
    public partial class BigCCTVCtrlOwner : Form, UnE.Control.ICCTVCtrlOwner
    {
        #region DllImport Functions

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll")]
        static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, ref COPYDATASTRUCT lParam);

        #endregion

        #region Valiable

        private static BigCCTVCtrlOwner m_Instance = null;
        private static int m_nCCTVCount = 0;

        protected int m_nID = -1;

        private IntPtr m_hParent = IntPtr.Zero;

        private CCTV m_cctv = null;
        private CCTV m_cctvQueue = null;

        private bool m_isConnected = false;
        private bool m_isSelected = false;
        
        private CCTVLoader mLoader = null;
                
        // CCTV Viewer의 위치 Index(0 ~ 5)
        private int m_nPositionIndex = -1;
        
        private bool EquipZoneCCTVMode = false;
        private int EquipZoneID;
        private string DefaultPreset = "";

        private Size mSaveSize = new Size();
        private Point mSaveLoc = new Point();
        private bool m_bLargeMode = false;
        private int m_nLineThick = 5;
        
        #endregion

        #region Window Messages

        private static int WM_SYSKEYDOWN = 0x0104;
        private static int WM_CHAR = 0x0102;
        private static int WM_KEYDOWN = 0x0100;
        private static int WM_KEYUP = 0x101;  //Key up
        private static int WM_COPYDATA = 0x004A;

        private static int WM_LBUTTONDOWN = 0x201; //Left mousebutton down
        private static int WM_LBUTTONUP = 0x202; //Left mousebutton up
        private static int WM_LBUTTONDBLCLK = 0x203; //Left mousebutton doubleclick
        private static int WM_RBUTTONDOWN = 0x204; //Right mousebutton down
        private static int WM_RBUTTONUP = 0x205;  //Right mousebutton up
        private static int WM_RBUTTONDBLCLK = 0x206; //Right mousebutton doubleclick

        private static int WM_USER = 0x400;
        private static int ENABLE_DOUBLE_CLICK_EVENT = WM_USER + 1;

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

        #endregion

        #region Properties

        public bool IsConnected
        {
            get { return m_isConnected; }
        }

        public static BigCCTVCtrlOwner Instance
        {
            get { return BigCCTVCtrlOwner.m_Instance; }
        }

        public CCTVLoader CCTVLoader
        {
            get { return mLoader; }
            set { mLoader = value; }
        }

        public CCTV P_CCTV
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

        #endregion

        #region Struct

        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public UInt32 cbData;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }

        #endregion

        #region Control

        private System.Windows.Forms.ContextMenuStrip contextMenu = new ContextMenuStrip();
        private System.Windows.Forms.ToolStripMenuItem contextMenu_CCTVCLOSE = new ToolStripMenuItem();
        //private System.Windows.Forms.ToolStripMenuItem contextMenu_PTZShow = new ToolStripMenuItem();

        #endregion

        /// <summary>생성자</summary>
        /// <param name="cctv"></param>
        /// <param name="frmParent"></param>
        /// <param name="nPositionIndex"></param>
        /// <param name="nEquipZoneCCTVMode"></param>
        /// <param name="nEquipZoneID"></param>
        /// <param name="nDefaultPreset"></param>
        public BigCCTVCtrlOwner(CCTV cctv, IntPtr frmParent, int nPositionIndex, bool nEquipZoneCCTVMode, int nEquipZoneID, String nDefaultPreset)
		{
            m_Instance = this;

			m_nID = ++m_nCCTVCount;
            m_nPositionIndex = nPositionIndex;
            EquipZoneCCTVMode = nEquipZoneCCTVMode;
            EquipZoneID = nEquipZoneID;
            DefaultPreset = nDefaultPreset.Trim();

            if (cctv != null)
                this.cctvCtrl1 = new UnE.Control.CCTVCtrl((UnE.Control.CCTVTypes)cctv.CCTVType, m_nPositionIndex);
            else
                this.cctvCtrl1 = new UnE.Control.CCTVCtrl(m_nPositionIndex);

			InitializeComponent();            

            lbTitle.Parent = this.cctvCtrl1;
            lbTitle.BackColor = Color.Black;
            lbTitle.BringToFront();

			P_CCTV = cctv;
            this.cctvCtrl1.CCTVID = P_CCTV.ID;
            this.cctvCtrl1.CCTVOwner = this;

            if (cctv == null || cctv.ReversePTZ < 0)
                this.btnPTZ.Visible = false;

            if (frmParent != IntPtr.Zero && cctv != null)
            {
                int param = cctv.EnableDoubleClickEvent() ? 1 : 0;
                IntPtr lParam = MakeLParam(param, 0);
                SendMessage(frmParent, ENABLE_DOUBLE_CLICK_EVENT, IntPtr.Zero, lParam);
            }

            btnPTZ.Parent = this.cctvCtrl1;                        
            btnPTZ.BringToFront();
            btnPTZ.Location = new Point(this.Size.Width - btnPTZ.Width - 5, 5);
            
            panelPTZ.Visible = false;
            panelPTZ.Location = new Point(this.Size.Width - panelPTZ.Width - 5, btnPTZ.Location.Y + btnPTZ.Height + 5);            

            btnPTZEdit.Visible = false;//(nEquipZoneCCTVMode == true) && (cctvCtrl1.EnablePTZ == true);
            btnPTZEdit.Location = new Point(this.Size.Width - (btnPTZEdit.Width + btnPTZ.Width + 10), 5);            
            
            panelPTZEdit.Visible = false;
            panelPTZEdit.Location = new Point(btnPTZEdit.Location.X, btnPTZEdit.Location.Y + btnPTZEdit.Height + 5);

            contextMenu.Name = "contextMenu";
            contextMenu.Size = new System.Drawing.Size(99, 26);
            contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { contextMenu_CCTVCLOSE });

            //contextMenu_PTZShow.Name = "contextMenu_PTZShow";
            //contextMenu_PTZShow.Size = new System.Drawing.Size(98, 22);
            //contextMenu_PTZShow.Text = "CCTV 제어하기";
            //contextMenu_PTZShow.Click += contextMenu_PTZShow_Click;

            contextMenu_CCTVCLOSE.Name = "contextMenu_CCTVCLOSE";
            contextMenu_CCTVCLOSE.Size = new System.Drawing.Size(98, 22);
            contextMenu_CCTVCLOSE.Text = "CCTV 없음";
            contextMenu_CCTVCLOSE.Click += contextMenu_CCTVCLOSE_Click;

            this.ContextMenuStrip = contextMenu;

            this.btnPresetMove.Click += btnPresetMove_Click;

            cboPreset_Control.KeyPress += cbo_KeyPress;
            cboPreset_PTZEdit.KeyPress += cbo_KeyPress;
		}

        protected virtual void BigCCTVCtrl_Load(object sender, EventArgs e){ Connect(); }

        public void Connect()
        {
            if (m_cctv != null)
            {
                LoadCamera();
                //System.Threading.Thread thread = new System.Threading.Thread(LoadCamera);
                //thread.Start();
            }

            //if (m_cctv != null && cctvCtrl1 != null && cctvCtrl1.EnablePTZ)
            //{
            //    // PTZ를 제어할 수 없다면 PTZ 제어버튼을 보이지 않도록 한다.
            //    // [2018-03-26] 김지웅
            //    this.btnPTZ.Visible = true;
            //    btnPTZ.BringToFront();

            //    if (EquipZoneCCTVMode == true)
            //    {
            //        //btnPTZEdit.BringToFront();
            //        //btnPTZEdit.Visible = EquipZoneCCTVMode;
            //        //ArrayList PresetList = RTSP_ONVIF.ONVIF_PTZ_Manager.Instance.GetPresetList(P_CCTV.ID);
            //        //cboPreset_PTZEdit.Items.Clear();
            //        //if (PresetList != null)
            //        //    this.cboPreset_PTZEdit.Items.AddRange(PresetList.ToArray());
            //    }

            //    EquipZoneCCTVMode = false;
            //}
            //else
            //{
            //    this.btnPTZ.Visible = false;
            //    //btnPTZ.BringToFront();
            //}
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
                if (cctvCtrl1.CCTVType == UnE.Control.CCTVTypes.None || (int)cctvCtrl1.CCTVType != m_cctv.CCTVType)
                {
                    this.Controls.Remove(cctvCtrl1);
                    this.cctvCtrl1 = new UnE.Control.CCTVCtrl((UnE.Control.CCTVTypes)m_cctv.CCTVType, m_nPositionIndex);
                    this.cctvCtrl1.CCTVOwner = this;
                    this.cctvCtrl1.CCTVID = m_cctv.ID;
                    this.cctvCtrl1.Visible = true;
                    this.cctvCtrl1.Location = new System.Drawing.Point(0, 0);
                    this.cctvCtrl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right)));
                    this.cctvCtrl1.Dock = DockStyle.Fill;

                    this.cctvCtrl1.AllowDrop = true;
                    this.cctvCtrl1.DragEnter += cctvCtrl1_DragEnter;
                    this.cctvCtrl1.DragDrop += cctvCtrl1_DragDrop;

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
            
            BigCCTVCtrlOwner.Instance.Invoke((MethodInvoker)delegate
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

            Update_CCTVType(m_cctv);
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

                if (m_cctv == null)
                {
                    this.Invoke(
                       new Action(() => SetTitle("CCTV정보 없음"))
                    );
                }

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

                    this.cctvCtrl1 = new UnE.Control.CCTVCtrl((UnE.Control.CCTVTypes)m_cctv.CCTVType, m_nPositionIndex);
                    cctvCtrl1.Visible = true;
                    this.cctvCtrl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
          | System.Windows.Forms.AnchorStyles.Left)
          | System.Windows.Forms.AnchorStyles.Right)));
                    this.cctvCtrl1.Location = new System.Drawing.Point(0, 0);
                    this.cctvCtrl1.Dock = DockStyle.Fill;

                    this.cctvCtrl1.AllowDrop = true;
                    this.cctvCtrl1.DragEnter += cctvCtrl1_DragEnter;
                    this.cctvCtrl1.DragDrop += cctvCtrl1_DragDrop;

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

            if (m_cctv != null && m_cctv.BigURL.Length > 0 && m_cctv.SmallURL.Length > 0)
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
                        //Connect();
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
            {
                cctvCtrl1.ZoomIn();                
            }                
            else if (sender == btnZoomOut)
            {
                cctvCtrl1.ZoomOut();                
            }                
            else if (sender == btnUp)
            {
                cctvCtrl1.MoveUp();                
            }
            else if (sender == btnDown)
            { 
                cctvCtrl1.MoveDown();
            }
            else if (sender == btnRight)
            {
                cctvCtrl1.MoveRight();                
            }
            else if (sender == btnLeft)
            {
                cctvCtrl1.MoveLeft();                
            }              
            else if (sender == btnStop)
            {
                cctvCtrl1.TestStop(9);                
            }
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
            return;
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
                IsSelected = !IsSelected;
                IntPtr pt = MakeLParam(1, 1);
                SendMessage(m_hParent, WM_LBUTTONDOWN, IntPtr.Zero, pt);
			}
			else if (e.Button == System.Windows.Forms.MouseButtons.Middle)
			{
			}
		}

        private void BigCCTVCtrl_DoubleClick(object sender, EventArgs e)
        {
            return;
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

        public void OnMouseLButtonDoubleClick()
        {
            //BigCCTVCtrl_DoubleClick(null, null);
            IntPtr pt = MakeLParam(1, 1);

            // Emulated mouse dbclk
            //SendMessage(m_hParent, WM_LBUTTONDOWN, IntPtr.Zero, pt);
            //SendMessage(m_hParent, WM_LBUTTONUP, IntPtr.Zero, pt);
            SendMessage(m_hParent, WM_LBUTTONDBLCLK, IntPtr.Zero, pt);
            SendMessage(m_hParent, WM_LBUTTONUP, IntPtr.Zero, pt);
        }

        public void OnMouseLButtonClick()
        {
            IsSelected = !IsSelected;

            IntPtr pt = MakeLParam(0, 0);
            SendMessage(m_hParent, WM_LBUTTONDOWN,IntPtr.Zero , pt);
            SendMessage(m_hParent, WM_LBUTTONUP, IntPtr.Zero, pt);                  
        }

        public void OnMouseRButtonClick(){ }

        public void OnMouseRButtonDoubleClick(){ }

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

            panelPTZEdit.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            btnPTZ.BringToFront();
            panelPTZ.Visible = false;
        }

        private void btnUp_Click(object sender, EventArgs e){ }
        
        public void SetParentHandle(IntPtr hParent)
        {
            m_hParent = hParent;
        }

        private void cctvCtrl1_SizeChanged(object sender, EventArgs e){ }

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

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_RBUTTONUP)//오른쪽 버튼 UP
            {
                if (lbTitle.Text == "CCTV 정보 없음") return;

                Point pt = GetLocation(m.LParam);
                if (pt.X == 0 && pt.Y == 0) return;

                contextMenu.Visible = true;
                ((ToolStrip)contextMenu).Location = pt;
            }
            else if (m.Msg == WM_KEYDOWN)
            {
                if ((int)m.WParam == (int)VKeys.VK_DELETE)
                {
                    if (lbTitle.ForeColor == Color.White) return;

                    SendMessage(m_hParent, WM_CHAR, (IntPtr)VKeys.VK_DELETE, IntPtr.Zero);
                }
            }

            base.WndProc(ref m);
        }

        public Point GetLocation(IntPtr LParam)
        {
            IntPtr xy = LParam;
            int x = unchecked((short)xy);
            int y = unchecked((short)((uint)xy >> 16));
            return new System.Drawing.Point(x, y);
        }

        private void Update_CCTVType(CCTV pCCTV)
        {
            btnPTZ.BringToFront();
            btnPTZEdit.BringToFront();

            UnE.Control.CCTVTypes CCTVType = (UnE.Control.CCTVTypes)pCCTV.CCTVType;
            Boolean isEditable = false;

            switch (CCTVType)
            {
                case Control.CCTVTypes.RTSP:
                case Control.CCTVTypes.RTSPONVIF:
                    isEditable = true;
                    break;
                default:
                    isEditable = false;
                    break;
            }

            if (isEditable == true)
                btnPTZ.Visible = true;
            else
                btnPTZ.Visible = false;

            if (isEditable == true)
            {
                ArrayList PresetList = RTSP_ONVIF.ONVIF_PTZ_Manager.Instance.GetPresetList(P_CCTV.ID);
                cboPreset_Control.Items.Clear();
                if (PresetList != null)
                    this.cboPreset_Control.Items.AddRange(PresetList.ToArray());
            }

            if (isEditable == true && EquipZoneCCTVMode == true)
            {
                btnPTZEdit.Visible = true;
                EquipZoneCCTVMode = false;

                if (DefaultPreset != "" && DefaultPreset.ToLower() != "null")
                    lblDefaultPreset.Text = DefaultPreset;
                else
                    lblDefaultPreset.Text = "None";

                ArrayList PresetList = RTSP_ONVIF.ONVIF_PTZ_Manager.Instance.GetPresetList(P_CCTV.ID);
                cboPreset_PTZEdit.Items.Clear();
                if (PresetList != null)
                    this.cboPreset_PTZEdit.Items.AddRange(PresetList.ToArray());
            }
            else
            {
                btnPTZEdit.Visible = false;
            }
        }

        void btnPresetMove_Click(object sender, EventArgs e)
        {
            if (cboPreset_Control.SelectedItem == null || cboPreset_Control.SelectedItem.ToString() == "") return;

            Int32 iReturn = RTSP_ONVIF.ONVIF_PTZ_Manager.Instance.GoPreset(P_CCTV.ID, cboPreset_Control.SelectedItem.ToString());
            if (iReturn != 220)
            {
                MessageBox.Show("Preset 이동에 실패하였습니다.", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void cctvCtrl1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text) == true)
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;
        }

        void cctvCtrl1_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                String strCCTVInfo = (String)e.Data.GetData(DataFormats.Text);

                UnE.CCTV.CCTVLoader loader = new UnE.CCTV.CCTVLoader(Int32.Parse(strCCTVInfo.Split(',')[0]));
                UnE.CCTV.CCTV _cctv = loader.LoadCCTV(Int32.Parse(strCCTVInfo.Split(',')[1]));

                //CloseCamera();
                P_CCTV = _cctv;
                //this.m_cctvQueue = _cctv;                                
                Update_CCTVType(P_CCTV);
                //Pipe 서버 통신을 위해서 CCTV 생성을 알림
                SendMessage(m_hParent, WM_CHAR, (IntPtr)VKeys.VK_INSERT, (IntPtr)int.Parse(strCCTVInfo.Split(',')[1]));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Drag and Drop Exception : " + ex.Message);
            }
        }

        void contextMenu_CCTVCLOSE_Click(object sender, EventArgs e)
        {
            SendMessage(m_hParent, WM_CHAR, (IntPtr)VKeys.VK_DELETE, IntPtr.Zero);
        }

        private void btnPTZEdit_Click(object sender, EventArgs e)
        {            
            panelPTZ.Visible = false;

            panelPTZEdit.BringToFront();
            panelPTZEdit.Visible = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            btnPTZEdit.BringToFront();
            panelPTZEdit.Visible = false;
        }

        private void btnPTZSave_Click(object sender, EventArgs e)
        {
            if (cboPreset_PTZEdit.SelectedItem == null || cboPreset_PTZEdit.SelectedItem.ToString() == "") return;

            Int32 iPosIndex = CCTVLoader.ChangeEquipZoneIndex(m_nPositionIndex);

            RTSP_ONVIF.ONVIF_PTZ_Manager.Instance.ChangePreset(
                EquipZoneID,
                iPosIndex,
                P_CCTV.ID, 
                cboPreset_PTZEdit.SelectedItem.ToString());
            
            byte[] buff = System.Text.Encoding.Default.GetBytes(cboPreset_PTZEdit.SelectedItem.ToString());

            COPYDATASTRUCT cds = new COPYDATASTRUCT();
            cds.dwData = IntPtr.Zero;
            cds.cbData = (uint)(buff.Length + 1); //buff size
            cds.lpData = cboPreset_PTZEdit.SelectedItem.ToString(); //msg string data

            SendMessage(m_hParent, WM_COPYDATA, 0, ref cds);

            lblDefaultPreset.Text = cboPreset_PTZEdit.SelectedItem.ToString(); //msg string data

            this.panelPTZEdit.Visible = false;
        }

        void cbo_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void cboPreset_PTZEdit_SelectedIndexChanged(object sender, EventArgs e)
        {
            Int32 iReturn = RTSP_ONVIF.ONVIF_PTZ_Manager.Instance.GoPreset(P_CCTV.ID, cboPreset_PTZEdit.SelectedItem.ToString());
            if (iReturn != 220)
            {
                MessageBox.Show("Preset 이동에 실패하였습니다.", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
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