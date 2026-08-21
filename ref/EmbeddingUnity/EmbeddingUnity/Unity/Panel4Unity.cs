using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32;
using System.Security.AccessControl;


namespace UnE.Util
{
    public partial class Panel4Unity : Panel
    {
        #region Win32 Contant Value

        private const int GWL_ID = -12;
        private const int GWL_STYLE = -16;
        private const int GWL_EXSTYLE = -20;

        private const uint WS_OVERLAPPED = 0;
        private const uint WS_POPUP = 0x80000000;
        private const int WS_CHILD = 0x40000000;
        private const uint WS_MINIMIZE = 0x20000000;
        private const uint WS_VISIBLE = 0x10000000;
        private const uint WS_DISABLED = 0x8000000;
        private const uint WS_CLIPSIBLINGS = 0x4000000;
        private const uint WS_CLIPCHILDREN = 0x2000000;
        private const uint WS_MAXIMIZE = 0x1000000;
        private const uint WS_CAPTION = 0xC00000;      // WS_BORDER or WS_DLGFRAME  
        private const int WS_BORDER = 0x00800000;
        private const int WS_DLGFRAME = 0x400000;
        private const uint WS_VSCROLL = 0x200000;
        private const uint WS_HSCROLL = 0x100000;
        private const uint WS_SYSMENU = 0x80000;
        private const int WS_THICKFRAME = 0x0040000;
        private const uint WS_GROUP = 0x20000;
        private const uint WS_TABSTOP = 0x10000;
        private const uint WS_MINIMIZEBOX = 0x20000;
        private const uint WS_MAXIMIZEBOX = 0x10000;
        private const uint WS_TILED = WS_OVERLAPPED;
        private const uint WS_ICONIC = WS_MINIMIZE;
        private const uint WS_SIZEBOX = WS_THICKFRAME;

        // Extended Window Styles 
        private const long WS_EX_DLGMODALFRAME = 0x0001;
        private const long WS_EX_NOPARENTNOTIFY = 0x0004;
        private const long WS_EX_TOPMOST = 0x0008;
        private const long WS_EX_ACCEPTFILES = 0x0010;
        private const long WS_EX_TRANSPARENT = 0x0020;
        private const int WS_EX_MDICHILD = 0x0040;
        private const long WS_EX_TOOLWINDOW = 0x0080;
        private const long WS_EX_WINDOWEDGE = 0x0100;
        private const int WS_EX_CLIENTEDGE = 0x0200;
        private const long WS_EX_CONTEXTHELP = 0x0400;
        private const long WS_EX_RIGHT = 0x1000;
        private const long WS_EX_LEFT = 0x0000;
        private const long WS_EX_RTLREADING = 0x2000;
        private const long WS_EX_LTRREADING = 0x0000;
        private long WS_EX_LEFTSCROLLBAR = 0x4000;
        private const long WS_EX_RIGHTSCROLLBAR = 0x0000;
        private long WS_EX_CONTROLPARENT = 0x10000;
        private const long WS_EX_STATICEDGE = 0x20000;
        private const long WS_EX_APPWINDOW = 0x40000;
        private const long WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE);
        private const long WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST);
        private const long WS_EX_LAYERED = 0x00080000;
        private long WS_EX_NOINHERITLAYOUT = 0x00100000; // Disable inheritence of mirroring by children
        private const long WS_EX_LAYOUTRTL = 0x00400000; // Right to left mirroring
        private const long WS_EX_COMPOSITED = 0x02000000;
        private const long WS_EX_NOACTIVATE = 0x08000000;

        private const int HWND_TOPMOST = -1;
        private const int HWND_NOTOPMOST = -2;
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
        private const int SWP_NOOWNERZORDER = 0x0200;
        private IContainer components;  /* Don't do owner Z ordering */
        private const int SWP_NOSENDCHANGING = 0x0400;  /* Don't send WM_WINDOWPOSCHANGING */
        
        private const int WM_MOVE = 0x0003;

        #endregion

        #region Win Function
        [DllImport("user32.dll")]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);            

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindowEx(IntPtr hWnd1, IntPtr hWnd2, string lpsz1, string lpsz2);

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        private static extern bool SetWindowPos(
                IntPtr hWnd, // window handle
                IntPtr hWndInsertAfter, // placement-order handle
                int X, // horizontal position
                int Y, // vertical position
                int cx, // width
                int cy, // height
                uint uFlags); // window positioning flags

        [DllImport("user32.dll")]
        private static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr SetFocus(IntPtr hWnd);

        private int MakeLParam(int LoWord, int HiWord)
        {
            int i = (HiWord << 16) | (LoWord & 0xffff);
            return i;
        }  

        #endregion
        
        private Pipelib.PassivePipeServer m_PipeServer;
        private IntPtr m_hWndUnity = IntPtr.Zero;
        private Process m_ProcessUnity = null;
        
        private System.Windows.Forms.ContextMenuStrip m_PopupMenu = null;
        public System.Windows.Forms.ContextMenuStrip PopupMenu
        {
            get { return m_PopupMenu; }
            set { m_PopupMenu = value; }
        }
                
        private string m_szPipeName = "TestPipe";
        public string NamedPipeName
        {
            get { return m_szPipeName; }
            set { m_szPipeName = value; }
        }


        private string m_szUnityFileName = "UnitySam";
        private string m_szUnityExePath = @"C:\UNE\bin\common12\UnitySam.exe";
        public string UnityExePath
        {
            get { return m_szUnityExePath; }
            set
            {
                if (value == null || value == "")
                    return;
                string szFileName = Path.GetFileName(value);
                string ext = Path.GetExtension(value);
                m_szUnityFileName = szFileName.Replace(ext, "").Replace(".", "");
                m_szUnityExePath = value;
            }
        }



        private string szUnityName = "AA_Unity";
        public string UnityWndName
        {
            get { return szUnityName; }
            set { szUnityName = value; }
        }
        
        private Action<int, float, float, float> m_IconPOIAddCallback = null;
        private Action<int, float, float, float> m_TextPOIAddCallback = null;


        private float m_fLastPickX = 0.0f;
        private float m_fLastPickY = 0.0f;
        private float m_fLastPickZ = 0.0f;

        private bool m_bAddTextMode = false;
        private bool m_bAddIconMode = false;

        private string szOverObjName = "";
        public string MouseOverObject
        {
            get { return szOverObjName; }
            set { szOverObjName = value; }
        }

        private string m_szIconName = "";
        public string IconName
        {
            get { return m_szIconName; }
            set { m_szIconName = value; }
        }

        private string m_szPoiText = "";
        public string PoiText
        {
            get { return m_szPoiText; }
            set { m_szPoiText = value; }
        }

        private System.Windows.Forms.Timer m_TimerSize;
       

        public Panel4Unity()
        {

            this.SizeChanged += Panel4Unity_SizeChanged;

            m_TimerSize = new System.Windows.Forms.Timer();
            m_TimerSize.Interval = 2000;
            m_TimerSize.Tick += OnTimerSizeChanged;
        }

        protected override void Dispose(bool disposing)
        {
            if (m_PipeServer != null)
            {
                StopUnity();
            }
            base.Dispose(disposing);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if (this.Handle != null)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    base.OnSizeChanged(e);
                });
            }
        }

       
        public void Update3D()
        {
            //Point pt = Location;
            //if (this.Parent != null)
            //{
               
            //    MoveWindow(m_hWndUnity, -11, -31, this.Parent.Width + 22, this.Height + 40, true);
            //    SetActiveWindow(m_hWndUnity);
            //}
            
        }

        public void SetPickMode(bool bPick)
        {
            if(m_PipeServer != null)
            {
                string szCmd = string.Format("CMD:SetMode(2, {0})", bPick.ToString());
                m_PipeServer.Send(szCmd);
            }            
        }       


        // Indoor Only
        public void OpenModel(string szName)
        {
            if(m_PipeServer != null)
            {
                string szCmd = string.Format("CMD:OpenModel('{0}')", szName);
                m_PipeServer.Send(szCmd);
            }
        }
       
        private void Panel4Unity_SizeChanged(object sender, EventArgs e)
        {
            m_TimerSize.Stop();
            m_TimerSize.Interval = 100;
            m_TimerSize.Start();
        }

        void OnTimerSizeChanged(object sender, EventArgs e)
        {
            Point pt = Location;
            if( this.Parent != null)
            {
                ///Update3D();
                MoveWindow(m_hWndUnity, -11, -31, this.Parent.Width + 22, this.Height + 40, false);            
            }
            
            m_TimerSize.Stop();
        }
     
        internal void OnDataCmd(string cmd)
        {
            this.Invoke((MethodInvoker)delegate
            {
                System.Diagnostics.Trace.WriteLine(cmd);
                UnE.Util.CommandProcessor cm = UnE.Util.CommandProcessor.Instance;
                cm.ProcessCommand(cmd, this);
            });
        }

        private Action m_CallbackReady = null;
        public bool BeginUnity(Action callbackReady)
        {
            KillProcess(m_szUnityFileName);
            BeginServer();
            SetUnity(m_szUnityFileName);

            m_CallbackReady = callbackReady;
            return true;
        }

        private void BeginServer()
        {
            m_PipeServer = new Pipelib.PassivePipeServer(true, m_szPipeName);
            m_PipeServer.OnReciveMessage += OnDataCmd;
            m_PipeServer.BeginPipe();
        }

        private void SetUnity(string szName)
        {
            m_hWndUnity = FindWindowEx(IntPtr.Zero, IntPtr.Zero, null, szUnityName);
            if (m_hWndUnity == IntPtr.Zero)
            {
                m_ProcessUnity = StartUnityPocess(m_szUnityExePath, m_szUnityExePath, "");
            }

            while (m_hWndUnity == IntPtr.Zero)
            {
                m_hWndUnity = FindWindowEx(IntPtr.Zero, IntPtr.Zero, null, szUnityName);
            }

            SetParent(m_hWndUnity, Handle);
            ShowWindow(m_hWndUnity, SW_SHOW);

            int style = GetWindowLong(m_hWndUnity, GWL_STYLE);
            int exStyle = GetWindowLong(m_hWndUnity, GWL_EXSTYLE);
            style &= ~(WS_BORDER | WS_THICKFRAME);
            exStyle &= ~WS_EX_CLIENTEDGE;
            exStyle |= (WS_EX_MDICHILD | WS_CHILD);
            SetWindowLong(m_hWndUnity, GWL_STYLE, (int)style);
            SetWindowLong(m_hWndUnity, GWL_EXSTYLE, (int)exStyle);

            SetWindowPos(m_hWndUnity, IntPtr.Zero, 0, 0, 0, 0, SWP_FRAMECHANGED | SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_NOOWNERZORDER);
            Point pt = Location;
            MoveWindow(m_hWndUnity, -11, -31, this.Width + 22, this.Height + 40, false);
        }

        public void StopUnity()
        {
            if (m_PipeServer != null)
                m_PipeServer.StopPipe();
            m_PipeServer = null;
            KillProcess(m_szUnityFileName);
        }    
        
        internal void OnReadyToSend()
        {
            if (m_CallbackReady != null)
                m_CallbackReady.Invoke();
        }

        private void OnUnityProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            //System.Diagnostics.Trace.WriteLine(e.Data);
        }

        private void OnUnityProcess_Exited(object sender, EventArgs e)
        {
            //int nExit = m_ProcessUnity.ExitCode;
        }

        private Process StartUnityPocess(string szFileName, string szWorkDir, string args)
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

                process.Exited += OnUnityProcess_Exited;
                process.ErrorDataReceived += OnUnityProcess_ErrorDataReceived;
                process.OutputDataReceived += OnUnityProcess_ErrorDataReceived;
                return process;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
            return null;
        }     

        internal void KillProcess(string strProcessName)
        {
            System.Diagnostics.Process[] processList = System.Diagnostics.Process.GetProcesses();

            foreach (System.Diagnostics.Process process in processList)
            {
                if (process.ProcessName == strProcessName)
                {
                    try
                    {
                        process.Kill();
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }


        private int m_nLastID =1;
        internal void OnReciveLastID(int nID)
        {

            if (this.Parent != null)
                this.Parent.Focus();


            m_nLastID = nID;

            if (m_bAddIconMode == true)
            {
                this.BeginInvoke(new Action(() =>
                {                    
                    if (m_IconPOIAddCallback != null)
                    {
                        m_IconPOIAddCallback.Invoke(m_nLastID, m_fLastPickX, m_fLastPickY, m_fLastPickZ);
                    }
                }));
            }
            if (m_bAddTextMode == true)
            {                
                this.BeginInvoke(new Action(() =>
                {
                        
                    if (m_TextPOIAddCallback != null)
                    {
                        m_TextPOIAddCallback.Invoke(m_nLastID, m_fLastPickX, m_fLastPickY, m_fLastPickZ);
                    }
                }));                
            }
        }

        internal void OnPoistionPick(float x, float y, float z)
        {
            if (this.Parent != null)
                this.Parent.Focus();

            m_fLastPickX = x;
            m_fLastPickY = y;
            m_fLastPickZ = z;

            if(m_bAddIconMode == true)
            {
                string szCmd = string.Format("CMD:AddIconPOI('{0}',{1},{2},{3})", m_szIconName, x, y, z);
                m_PipeServer.Send(szCmd);

                string szCmd2 = "CMD:GetLastID()";
                this.BeginInvoke(new Action(() =>
                {
                    m_PipeServer.Send(szCmd2);                    
                })); 
            }
            if(m_bAddTextMode == true)
            {
                string szText = this.m_szPoiText;
                if (szText != null && szText != "")
                {
                    string szCmd = string.Format("CMD:AddTextPOI('{0}',{1},{2},{3})", szText, x, y, z);
                    m_PipeServer.Send(szCmd);

                    string szCmd2 = "CMD:GetLastID()";
                    
                    this.BeginInvoke(new Action(() =>
                    {
                        m_PipeServer.Send(szCmd2);                        
                    })); 
                }                
            }
        }

        public int AddIconPOI(string szIconName , float x, float y, float z)
        {
            if( m_PipeServer != null)
            {
                string szCmd = string.Format("CMD:AddIconPOI('{0}',{1},{2},{3})", m_szIconName, x, y, z);
                m_PipeServer.Send(szCmd);

                string szCmd2 = "CMD:GetLastID()";
                m_PipeServer.Send(szCmd2);

                Thread.Sleep(50);

                return m_nLastID;
            }
            return -1;
        }

        public int AddIconPOI(string szIconName, int x, int y)
        {
            if (m_PipeServer != null)
            {
                string szCmd = string.Format("CMD:AddIconPOI2D('{0}',{1},{2})", m_szIconName, x - m_dX, y - m_dY);
                m_PipeServer.Send(szCmd);
                System.Diagnostics.Trace.WriteLine(szCmd);
                string szCmd2 = "CMD:GetLastID()";
                m_PipeServer.Send(szCmd2);

                Thread.Sleep(50);

                return m_nLastID;
            }
            return -1;
        }



        public int AddTextPOI(string szText, float x, float y, float z)
        {
            if (szText != null && szText != "")
            {
                string szCmd = string.Format("CMD:AddTextPOI('{0}',{1},{2},{3})", szText, x, y, z);
                m_PipeServer.Send(szCmd);

                string szCmd2 = "CMD:GetLastID()";
                m_PipeServer.Send(szCmd2);

                Thread.Sleep(50);

                return m_nLastID;
            }
            return -1;
        }

        public int AddTextPOI(string szText, int x, int y)
        {
            if (szText != null && szText != "")
            {
                string szCmd = string.Format("CMD:AddTextPOI2D('{0}',{1},{2})", szText, x - m_dX, y - m_dY);

                System.Diagnostics.Trace.WriteLine(szCmd);
                m_PipeServer.Send(szCmd);

                string szCmd2 = "CMD:GetLastID()";
                m_PipeServer.Send(szCmd2);

                Thread.Sleep(50);

                return m_nLastID;
            }
            return -1;
        }
            

        internal void ClosePopup()
        {
            if(m_PopupMenu != null)
            {
                if(m_PopupMenu.Visible == true)
                {
                    m_PopupMenu.Close();
                }
            }
        }


        private int m_dX = -8;
        private int m_dY = -29;

        internal void ShowPopup(int x, int y)
        {
            if (m_PopupMenu != null)
            {
                m_PopupMenu.AutoClose = true;
                m_PopupMenu.Tag = new Point(x + m_dX, y + m_dY);
                m_PopupMenu.Show(this, x + m_dX, y + m_dY);
                if (m_PopupMenu.Visible == false)
                    m_PopupMenu.Show(this, x + m_dX, y + m_dY);
                else
                    m_PopupMenu.BringToFront();
            }           
        }

        internal void OnPostRightMouseUp(int x, int y)
        {
            if (this.Parent != null)
                this.Parent.Focus();

            ShowPopup(x, y);
        }

        internal void OnPostRightMouseDown(int x, int y)
        {
            if (this.Parent != null)
                this.Parent.Focus();

            ClosePopup();           
        }

        internal void OnPostLeftMouseDown(int x, int y)
        {
            if (this.Parent != null)
                this.Parent.Focus();

            ClosePopup();
        }

        internal void OnPostLeftMouseUp(int x, int y)
        {
            if (this.Parent != null)
                this.Parent.Focus();

        }

        internal void OnPostMiddleMouseDown(int x, int y)
        {
            if (this.Parent != null)
                this.Parent.Focus();

            ClosePopup();
        }

        internal void OnPoseMiddleMouseUp(int x, int y)
        {
            if (this.Parent != null)
                this.Parent.Focus();

        }
        
        public void SendCommand(string szCmd)
        {
            if (m_PipeServer != null)
                m_PipeServer.Send(szCmd);
        }     
  
        internal void SetEnterObject(string szName)
        {
            szOverObjName = szName;
        }

        internal void SetLeaveObject(string szName)
        {
            if( szOverObjName == szName)
            {
                szOverObjName = "";
            }
        }

        public void SetIconPickAdd(bool bValue, Action<int, float, float, float> callback)
        {
            if (bValue == true)
            {
                m_IconPOIAddCallback = callback;
                m_bAddTextMode = false;
                m_PipeServer.Send("CMD:SetMode(2, True)");
                m_bAddIconMode = true;
            }
            else
            {
                m_IconPOIAddCallback = null;
                m_bAddIconMode = false;
                m_PipeServer.Send("CMD:SetMode(2, False)");
            }
        }

        public void SetTextPickAdd(bool bValue, Action<int, float, float, float> callback)
        {
            if (bValue == true)
            {
                m_TextPOIAddCallback = callback;
                m_bAddIconMode = false;
                m_PipeServer.Send("CMD:SetMode(2, True)");
                m_bAddTextMode = true;
            }
            else
            {
                m_TextPOIAddCallback = null;
                m_bAddTextMode = false;
                m_PipeServer.Send("CMD:SetMode(2, False)");
            }
        }

        public void SetTextColor(Color color)
        {
            int nColor = color.ToArgb();
            string szCmd = string.Format("CMD:SetTextColor({0})", nColor);
            if (m_PipeServer != null)
                m_PipeServer.Send(szCmd);
        }

        public void SetAliasTextColor(Color color)
        {
            int nColor = color.ToArgb();
            string szCmd = string.Format("CMD:SetAliasTextColor({0})", nColor);
            if (m_PipeServer != null)
                m_PipeServer.Send(szCmd);
        }

        public void AddAliasName(string szMeshName, string szAliasName)
        {
            string szCmd = string.Format("CMD:AddAliasName('{0}','{1}')", szMeshName, szAliasName);
            if (m_PipeServer != null)
                m_PipeServer.Send(szCmd);
        }

        public void UpdateAliasNames()
        {
            if (m_PipeServer != null)
                m_PipeServer.Send("CMD:UpdateAliasNames()");
        }

        public void SetZoomObject(string szMeshName)
        {
            string szCmd = string.Format("CMD:SetZoomObject('{0}')", szMeshName);
            if (m_PipeServer != null)
                m_PipeServer.Send(szCmd);

        }

        public void SetHomeView()
        {
            //string szCmd = string.Format("CMD:CameraView('{0}')", "fit");
            //if (m_PipeServer != null)
            //    m_PipeServer.Send(szCmd);
        }

        public void SetFrontView()
        {
            string szCmd = string.Format("CMD:CameraView('{0}')", "fit");
            if (m_PipeServer != null)
                m_PipeServer.Send(szCmd);
        }

         public void SetTopView()
        {
            string szCmd = string.Format("CMD:CameraView('{0}')", "top");
            if (m_PipeServer != null)
                m_PipeServer.Send(szCmd);
        }

        public void SetLeftView()
        {
            string szCmd = string.Format("CMD:CameraView('{0}')", "left");
            if (m_PipeServer != null)
                m_PipeServer.Send(szCmd);
        }

        public void SetRightView()
        {
            string szCmd = string.Format("CMD:CameraView('{0}')", "right");
            if (m_PipeServer != null)
                m_PipeServer.Send(szCmd);
        }

        public void SetRearView()
        {
            string szCmd = string.Format("CMD:CameraView('{0}')", "rear");
            if (m_PipeServer != null)
                m_PipeServer.Send(szCmd);
        }


       
        internal class Vector3
        {
            public Vector3(float fx, float fy, float fz)
            {
                x = fx;
                y = fy;
                z = fz;
            }

            private float x;
            public float X
            {
                get { return x; }
                set { x = value; }
            }

            private float y;
            public float Y
            {
                get { return y; }
                set { y = value; }
            }

            private float z;
            public float Z
            {
                get { return z; }
                set { z = value; }
            }
        }

        private bool m_bSaveHomeView = false;
        private string m_szHomeKey = @"SDMS\Unity\Homview";
        private Vector3 m_CamPos = null;
        private Vector3 m_Quater = null;
        private Vector3 m_CamDir = null;


        internal void OnReciveCameraPosition(float x, float y, float z)
        {
            m_CamPos = new Vector3(x, y, z);
        }

        internal void OnReciveCameraOrientaion(float x, float y, float z)
        {
            m_Quater = new Vector3(x, y, z);
        }

        internal void OnReciveCameraDirection(float x, float y, float z)
        {
            m_CamDir = new Vector3(x, y, z);

            WriteHomeView(m_szCurrentHomeViewName);
            m_bSaveHomeView = true;
        }

        private void GetCameraPosition()
        {
            string szCmd = "CMD:CameraPosition()";
            if (m_PipeServer != null)
                m_PipeServer.Send(szCmd);
        }

        private void GetCameraOrientaion()
        {
            string szCmd = "CMD:CameraAngles()";
            if (m_PipeServer != null)
                m_PipeServer.Send(szCmd);
        }

        private void GetCameraDirection()
        {
            string szCmd = "CMD:CameraDirection()";
            if (m_PipeServer != null)
                m_PipeServer.Send(szCmd);
        }


        private void SetCameraPosition()
        {
            string szCmd = string.Format("CMD:SetCameraPosition({0},{1},{2})", m_CamPos.X, m_CamPos.Y, m_CamPos.Z);
            if (m_PipeServer != null)
                m_PipeServer.Send(szCmd);
        }

        private void SetCameraOrientaion()
        {
            string szCmd = string.Format("CMD:SetCameraAngles({0},{1},{2})", m_Quater.X, m_Quater.Y, m_Quater.Z);
            if (m_PipeServer != null)
                m_PipeServer.Send(szCmd);
        }

        private void SetCameraDirection()
        {
            string szCmd = string.Format("CMD:SetCameraDirection({0},{1},{2})", m_CamDir.X, m_CamDir.Y, m_CamDir.Z);
            if (m_PipeServer != null)
                m_PipeServer.Send(szCmd);
        }

        private string m_szCurrentHomeViewName = "";
        public void SaveHomeView(string szName)
        {
            m_szCurrentHomeViewName = szName;

            GetCameraPosition();
            Thread.Sleep(50);
            GetCameraOrientaion();
            Thread.Sleep(50);
            GetCameraDirection();
        }

        public void LoadHomeView(string szName)
        {
            ReadHomeView(szName);   
            
            if(m_bSaveHomeView == true)
            {
                SetCameraPosition();
                SetCameraOrientaion();
                SetCameraDirection();
            }
        }

        private void ReadHomeView(string szName)
        {
            string szKeyName = m_szHomeKey + "\\" + szName;

            RegistryKey rkey = Registry.CurrentUser.OpenSubKey(szKeyName);
            if (rkey == null)
            {
                m_bSaveHomeView = false;
            }
            else
            {
                float x, y, z;
                string pX = (string)rkey.GetValue("POSITIONX");
                string pY = (string)rkey.GetValue("POSITIONY");
                string pZ = (string)rkey.GetValue("POSITIONZ");

                if (pX == null || pY == null || pZ == null)
                    return;
                if (float.TryParse(pX, out x))
                {
                    if (float.TryParse(pY, out y))
                    {
                        if (float.TryParse(pZ, out z))
                        {
                            m_CamPos = new Vector3(x, y, z);
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }

                pX = (string)rkey.GetValue("QUATERNIONX");
                pY = (string)rkey.GetValue("QUATERNIONY");
                pZ = (string)rkey.GetValue("QUATERNIONZ");

                if (pX == null || pY == null || pZ == null)
                    return;

                if (float.TryParse(pX, out x))
                {
                    if (float.TryParse(pY, out y))
                    {
                        if (float.TryParse(pZ, out z))
                        {
                            m_Quater = new Vector3(x, y, z);
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }

                pX = (string)rkey.GetValue("DIRECTIONX");
                pY = (string)rkey.GetValue("DIRECTIONY");
                pZ = (string)rkey.GetValue("DIRECTIONZ");

                if (pX == null || pY == null || pZ == null)
                    return;

                if (float.TryParse(pX, out x))
                {
                    if (float.TryParse(pY, out y))
                    {
                        if (float.TryParse(pZ, out z))
                        {
                            m_CamDir = new Vector3(x, y, z);
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }

                m_bSaveHomeView = true;
            }

            if (rkey != null)
                rkey.Close();
        }

        private void WriteHomeView(string szName)
        {
            string szUserName = Environment.UserDomainName + "\\" + Environment.UserName;

            RegistrySecurity rs = new RegistrySecurity();

            rs.AddAccessRule(new RegistryAccessRule(szUserName,
                RegistryRights.ReadKey | RegistryRights.Delete | RegistryRights.WriteKey,
                InheritanceFlags.None,
                PropagationFlags.None,
                AccessControlType.Allow));

            rs.AddAccessRule(new RegistryAccessRule(szUserName,
                RegistryRights.ChangePermissions,
                InheritanceFlags.None,
                PropagationFlags.None,
                AccessControlType.Deny));
            
            string szKeyName = m_szHomeKey + "\\" + szName;
            RegistryKey rkey = Registry.CurrentUser.OpenSubKey(szKeyName, true);
            if (rkey == null)
            {
                try
                {
                    rkey = Registry.CurrentUser.CreateSubKey(szKeyName, RegistryKeyPermissionCheck.ReadWriteSubTree, rs);
                }
                catch (Exception)
                {
                }
            }

            if (rkey != null)
            {
                if (m_CamPos != null)
                {
                    rkey.SetValue("POSITIONX", m_CamPos.X);
                    rkey.SetValue("POSITIONY", m_CamPos.Y);
                    rkey.SetValue("POSITIONZ", m_CamPos.Z);
                }
                if (m_Quater!= null)
                {
                    rkey.SetValue("QUATERNIONX", m_Quater.X);
                    rkey.SetValue("QUATERNIONY", m_Quater.Y);
                    rkey.SetValue("QUATERNIONZ", m_Quater.Z);
                }
                if (m_CamDir != null)
                {
                    rkey.SetValue("DIRECTIONX", m_CamDir.X);
                    rkey.SetValue("DIRECTIONY", m_CamDir.Y);
                    rkey.SetValue("DIRECTIONZ", m_CamDir.Z);
                }                
                rkey.Close();
            }
        }
    }
}
