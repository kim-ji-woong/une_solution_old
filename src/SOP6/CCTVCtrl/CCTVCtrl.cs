


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RTSP_ONVIF;
using System.Threading;
using System.Diagnostics;
namespace UnE.Control
{
    public enum CCTVTypes 
    {
        NotSet = 0,
        Axis,       
        NVS,
        XpressStrm,
        UDP,
        Panasonic,
        TechWin,
        IPVideo,
        HIK,
        NVT,
        MediaPlayer,
        IDIS,
        RTSP,       
        IDIS_NVR,
        ITX_NVR,
        RTSPONVIF,
        SVMS,
        Divisys,
        WESP,
        None
    }

    enum MediaType
    {
        mjpeg,
        h264,
        mpeg4
    }

    enum AMC_VIDEO_RENDERER
    {
        AMC_VIDEO_RENDERER_VMR7 = 0,
        AMC_VIDEO_RENDERER_VMR9 = 4096,
        AMC_VIDEO_RENDERER_EVR = 65536,
    }

    public partial class CCTVCtrl : UserControl, CCTVControl.IDISCameraOwner, IReconnectManagerOwner
    {
        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        private static int WM_CLOSE = 0x0010;

        private CCTVTypes mCurTypes = CCTVTypes.NotSet;
        public CCTVTypes CCTVType
        {
            get { return mCurTypes; }
        }

        private bool m_bIsConnected = false;
        public bool IsConnected
        {
            get { return m_bIsConnected; }
        }

        private bool m_enablePTZ = true;
        public bool EnablePTZ
        {
            get { return m_enablePTZ; }
        }

        private string m_szPassword = "fa0e6a34fd25d96a";
        private string m_szIPAddress = "";
        private Panel panel1;

        private ICCTVCtrlOwner m_owner = null;

        private UnE.Control.MediaType m_AxisMediaType = UnE.Control.MediaType.mjpeg;

        public ICCTVCtrlOwner CCTVOwner
        {
            get { return m_owner; }
            set { m_owner = value; }
        }

        private CCTVMessageFilter m_msgFilter = new CCTVMessageFilter();

        // 예외적인 상황이 발생하여 화면이 정지되거나 접속이 끊어졌을때 재접속을 하도록 도와준다.
        private ReconnectManager m_reconnectMgr = null;

        //private short m_nPlayBackMode = 0;
        //public short PlayBackMode
        //{
        //    get { return m_nPlayBackMode; }
        //    set { m_nPlayBackMode = value; }
        //}

        //private short m_nUseRepository = 0;
        //public short UseRepository
        //{
        //    get { return m_nUseRepository; }
        //    set { m_nUseRepository = value; }
        //}

        //private string m_szAccessKey = "";
        //public string AccessKey
        //{
        //    get { return m_szAccessKey; }
        //    set { m_szAccessKey = value; }
        //}

        
        //public string IPAddress
        //{
        //    get { return m_szIPAddress; }
        //    set { m_szIPAddress = value; }
        //}

        //private short m_nPort = 0;
        //public short Port
        //{
        //    get { return m_nPort; }
        //    set { m_nPort = value; }
        //}
             

        //private int m_nStream = 0;
        //public int Stream
        //{
        //    get { return m_nStream; }
        //    set { m_nStream = value; }
        //}

        //private int m_nChannel = 0;
        //public int Channel
        //{
        //    get { return m_nChannel; }
        //    set { m_nChannel = value; }
        //}

        //private string m_szUserName = "root";
        //public string UserName
        //{
        //    get { return m_szUserName; }
        //    set { m_szUserName = value; }
        //}

        private string m_szUserName = "root";
        // 재생버튼 및 재생 Text 보이는 부분
        private const int MEDIA_PLAYER_HIDDEN_SIZE = 60;
        //private AxAXVLC.AxVLCPlugin2 axVLCPlugin21;
        private Vlc.DotNet.Forms.VlcControl vlcControl1;

        private int m_nPositionIndex = -1;

        // 마우스 이벤트가 노출되어 있지 않은 CCTVControl들을 위한 Hooking Object
        //private Microsoft.Win32.MouseHookComponent mouseHookComponent = null;

        public int PositionIndex
        {
            get { return m_nPositionIndex; }
            set { m_nPositionIndex = value; }
        }

        private int m_CctvID;
        public int CCTVID
        {
            get { return m_CctvID; }
            set { m_CctvID = value; }
        }

        //private string m_szMediaType = "rtp-tcp";
        //public string MediaType
        //{
        //    get { return m_szMediaType; }
        //    set { m_szMediaType = value; }
        //}

       
        //internal UnE.Control.MediaType AxisMediaType
        //{
        //    get { return m_AxisMediaType; }
        //    set { m_AxisMediaType = value; }
        //}

        public CCTVCtrl(int nPositionIndex = -1)
        {
            mCurTypes = CCTVTypes.None;
            m_nPositionIndex = nPositionIndex;

            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "CCTVCtrl";
            this.Size = new System.Drawing.Size(801, 408);

            if (panel1 == null)
            {
                this.panel1 = new System.Windows.Forms.Panel();
                this.SuspendLayout();
                // 
                // panel1
                // 
                this.panel1.BackColor = this.BackColor;
                this.panel1.Location = new System.Drawing.Point(0, 0);
                this.panel1.Name = "panel1";
                this.panel1.Size = new System.Drawing.Size(801, 408);
                this.panel1.Dock = DockStyle.Fill;
                this.panel1.TabIndex = 0;
                this.Controls.Add(panel1);
            }
            this.Resize += CCTVCtrl_Resize;

            ptzTimer.Interval = 1000;
            ptzTimer.Tick += ptzTimer_Tick;

            m_reconnectMgr = new ReconnectManager(this);                        
        }

        public CCTVCtrl(CCTVTypes type, int nPositionIndex = -1)
        {
            mCurTypes = type;
            m_nPositionIndex = nPositionIndex;

            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "CCTVCtrl";
            this.Size = new System.Drawing.Size(801, 408);
         
            if (panel1 == null)
            {
                this.panel1 = new System.Windows.Forms.Panel();
                this.SuspendLayout();
                // 
                // panel1
                // 
                this.panel1.BackColor = this.BackColor;
                this.panel1.Location = new System.Drawing.Point(0, 0);
                this.panel1.Name = "panel1";
                this.panel1.Size = new System.Drawing.Size(801, 408);
                this.panel1.Dock = DockStyle.Fill;
                this.panel1.TabIndex = 0;
                panel1.MouseDown += panel1_MouseClick;
                panel1.MouseDoubleClick += panel1_MouseDoubleClick;
                this.Controls.Add(panel1);
            }
            this.Resize += CCTVCtrl_Resize;

            ptzTimer.Interval = 1000;
            ptzTimer.Tick += ptzTimer_Tick;

            m_reconnectMgr = new ReconnectManager(this);
        }

        private void AsyncResizeMediaPlayer()
        {
            System.Threading.Thread.Sleep(1000);

            try
            {
                this.Invoke((MethodInvoker)delegate
                {
                    if (axWindowsMediaPlayer1.IsHandleCreated == true)
                    {
                        axWindowsMediaPlayer1.Size = new Size(this.Width, this.Height + MEDIA_PLAYER_HIDDEN_SIZE);
                        axWindowsMediaPlayer1.SetBounds(0, 0, this.Width, this.Height + MEDIA_PLAYER_HIDDEN_SIZE);
                        //axWindowsMediaPlayer1.Size = new System.Drawing.Size(642, 510 + MEDIA_PLAYER_HIDDEN_SIZE);
                    }
                    
                });
            }
            catch(Exception)
            {
            }
        }

        void CCTVCtrl_Resize(object sender, EventArgs e)
        {
            if (mCurTypes == CCTVTypes.MediaPlayer)
            {
                if (axWindowsMediaPlayer1 != null && (this.Width > 0 && this.Height > 0))
                {
                    System.Threading.Thread t = new System.Threading.Thread(AsyncResizeMediaPlayer);
                    t.Start();
                }
            }
            else if( mCurTypes == CCTVTypes.IPVideo)
            {
#if _IPVideo_
                if (axTVSLiveControl1 != null && (this.Width > 0 && this.Height > 0))                
                {
                    if (nIpvideoCh > 0)
                    {
                        axTVSLiveControl1.Size = new Size(this.Width, this.Height);
                    }
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.Axis)
            {
#if _AXIS_
                if (axAxisMediaControl1 != null && (this.Width > 0 && this.Height > 0))
                {
                    axAxisMediaControl1.Size = new Size(this.Width, this.Height);
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.NVS || mCurTypes == CCTVTypes.NVT)
            {
#if _NVS_
                if (axNVSViewerCtrl1 != null && (this.Width > 0 && this.Height > 0))
                {
                    //axNVSViewerCtrl1.Size = new Size(this.Width, this.Height);
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.XpressStrm)
            {
#if _XpressStrm_
                if (axxpressStrm1 != null && (this.Width > 0 && this.Height > 0))
                {
                    axxpressStrm1.Size = new Size(this.Width, this.Height);
                } 
#endif
            }
            else if (mCurTypes == CCTVTypes.UDP)
            {
#if _UDP_
                if (axAxVCA1 != null && (this.Width > 0 && this.Height > 0))
                {
                    axAxVCA1.Size = new Size(this.Width, this.Height);
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.Panasonic)
            {
#if _Panasonic_
                if (axipropsapiCtrl1 != null && (this.Width > 0 && this.Height > 0))
                {
                    axipropsapiCtrl1.Size = new Size(this.Width, this.Height);
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.TechWin)
            {
#if _TechWin_
                if (axTechWinLib1 != null && (this.Width > 0 && this.Height > 0))
                {
                    axTechWinLib1.Size = new Size(this.Width, this.Height);
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.IDIS)
            {
#if _IDIS_
                if (axRASplus_WatSear1 != null && (this.Width > 0 && this.Height > 0))
                {
                    axRASplus_WatSear1.Size = new Size(this.Width, this.Height);
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.RTSP)
            {
                if (vlcControl2 != null && (this.Width > 0 && this.Height > 0))
                {
                    vlcControl2.Size = new Size(this.Width, this.Height);
                    m_panelRTSPNoConnect.Location = vlcControl2.Location;
                    m_panelRTSPNoConnect.Size = vlcControl2.Size;
                }
                /*if (streamPlayerControl != null && (this.Width > 0 && this.Height > 0))
                {
                    streamPlayerControl.Size = new Size(this.Width, this.Height);
                    m_panelRTSPNoConnect.Location = streamPlayerControl.Location;
                    m_panelRTSPNoConnect.Size = streamPlayerControl.Size;
                }*/
            }
            else if (mCurTypes == CCTVTypes.RTSPONVIF)
            {
                //if (axvlcplugin21 != null && (this.width > 0 && this.height > 0))
                //{
                //    axvlcplugin21.width = this.width;
                //    axvlcplugin21.height = this.height;
                //}
                if (vlcControl1 != null && (this.Width > 0 && this.Height > 0))
                {
                    vlcControl1.Width = this.Width;
                    vlcControl1.Height = this.Height;
                }
            }
            else if (mCurTypes == CCTVTypes.IDIS_NVR)
            {
#if _IDIS_NVR_
                if (idisNVRSet != null && (this.Width > 0 && this.Height > 0))
                    idisNVRSet.Resize(this.Width, this.Height);
#endif
            }
            else if (mCurTypes == CCTVTypes.ITX_NVR)
            {
#if _ITX_NVR_
                if (axitxview1 != null && (this.Width > 0 && this.Height > 0))
                    axitxview1.Size = new Size(this.Width, this.Height);
#endif
            }
            else if (mCurTypes == CCTVTypes.SVMS)
            {
#if _SVMS_
                if (svmsCamera != null && (this.Width > 0 && this.Height > 0))
                    svmsCamera.Size = new Size(this.Width, this.Height);
#endif
            }
            else if (mCurTypes == CCTVTypes.Divisys)
            {
#if _Divisys_
                if (divisysCamera != null && (this.Width > 0 && this.Height > 0))
                    divisysCamera.Size = new Size(this.Width, this.Height);
#endif
            }
            else if (mCurTypes == CCTVTypes.WESP)
            {
#if _WESP_
                if (wespCamera != null && (this.Width > 0 && this.Height > 0))
                    wespCamera.Size = new Size(this.Width, this.Height);
#endif
            }

            if (this.Width > 0 && this.Height > 0)
                panel1.Size = new Size(this.Width, this.Height);

        }

        private void SetDisable()
        {
            m_reconnectMgr.Close();

            if (m_bIsConnected == false)
            { 
                return;
            }


            if (mCurTypes == CCTVTypes.XpressStrm)
            {
#if _XpressStrm_
                try
                {
                    axxpressStrm1.LiveAudioOutput(0);
                    axxpressStrm1.LiveVideo(0);
                    axxpressStrm1.LiveAudio(0);
                    axxpressStrm1.Disconnect();
                    axxpressStrm1.RepositoryDisconnect();
                    axxpressStrm1.Dispose();

                    Controls.Remove(axxpressStrm1);
                    axxpressStrm1 = null;
                }
                catch (Exception)
                {
                }
#endif
            } 
            else if( mCurTypes == CCTVTypes.Axis)
            {
#if _AXIS_
                try
                {
                    axAxisMediaControl1.Stop();
                    axAxisMediaControl1.Dispose();
                    Controls.Remove(axAxisMediaControl1);
                    axAxisMediaControl1 = null;
                }
                catch (Exception)
                {
                }
#endif
            }
            else if(mCurTypes == CCTVTypes.NVS || mCurTypes == CCTVTypes.NVT)
            {
#if _NVS_
                try
                {
                    axNVSViewerCtrl1.Stop();
                    axNVSViewerCtrl1.Dispose();
                    Controls.Remove(axNVSViewerCtrl1);
                    axNVSViewerCtrl1 = null;
                }
                catch (Exception)
                {
                }
#endif
            }
            else if(mCurTypes == CCTVTypes.UDP)
            {
#if _UDP_
                try
                {
                    axAxVCA1.Stop();
                    axAxVCA1.Dispose();
                    Controls.Remove(axAxVCA1);
                    axAxVCA1 = null;
                }
                catch (Exception)
                {
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.Panasonic)
            {
#if _Panasonic_
                try
                {
                    axipropsapiCtrl1.Disconnect();
                    axipropsapiCtrl1.Dispose();

                    Controls.Remove(axipropsapiCtrl1);
                    axipropsapiCtrl1 = null;
                }
                catch (Exception)
                {
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.TechWin)
            {
#if _TechWin_
                try
                {
                    axTechWinLib1.Stop();
                    axTechWinLib1.Dispose();
                    Controls.Remove(axTechWinLib1);
                    axTechWinLib1 = null;
                }
                catch (Exception)
                {
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.IPVideo)
            {
#if _IPVideo_
                try
                {
                    Controls.Remove(axTVSLiveControl1);
                    //axTVSLiveControl1.SetLocalConfig(7, 0);

                    if (nIpvideoCh > 0)
                    {
                        //axTVSLiveControl1.Pause(nIpvideoCh);
                        axTVSLiveControl1.SetMute(nIpvideoCh, true);
                        axTVSLiveControl1.Dispose();
                        nIpvideoCh = -1;
                    }     
                    //axTVSLiveControl1.Dispose();
                    axTVSLiveControl1 = null;

                    GC.Collect();
                }
                catch (Exception)
                {
                    if (axTVSLiveControl1 != null)
                    {
                        axTVSLiveControl1.Dispose();
                        axTVSLiveControl1 = null;
                    }
                    
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.MediaPlayer)
            {
                try
                {
                    Controls.Remove(axWindowsMediaPlayer1);
                    axWindowsMediaPlayer1.close();
                    axWindowsMediaPlayer1.Dispose();
                    axWindowsMediaPlayer1 = null;
                    GC.Collect();
                }
                catch (Exception)
                {
                    if (axWindowsMediaPlayer1 != null)
                    {
                        axWindowsMediaPlayer1.close();
                        axWindowsMediaPlayer1.Dispose();
                        axWindowsMediaPlayer1 = null;
                    }
                }
            }
            else if (mCurTypes == CCTVTypes.IDIS)
            {
#if _IDIS_
                try
                {
                    axRASplus_WatSear1.disconnectAll();
                    axRASplus_WatSear1.Dispose();
                    Controls.Remove(axRASplus_WatSear1);
                    axRASplus_WatSear1 = null;
                    GC.Collect();
                }
                catch (Exception)
                {
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.RTSP)
            {
                try
                {
                    vlcControl2.Stop();
                    vlcControl2.Dispose();
                    Controls.Remove(vlcControl2);
                    vlcControl2 = null;
                    /*streamPlayerControl.Stop();
                    streamPlayerControl.Dispose();
                    Controls.Remove(streamPlayerControl);
                    streamPlayerControl = null;*/
                    GC.Collect();
                }
                catch (Exception)
                {
                }
            }

            else if (mCurTypes == CCTVTypes.RTSPONVIF)
            {
                try
                {
                    Controls.Remove(vlcControl1);
                    vlcControl1 = null;                    
                    GC.Collect();                    
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception Dispose : " + ex.Message);
                }
            }                
            else if (mCurTypes == CCTVTypes.IDIS_NVR)
            {
#if _IDIS_NVR_
                if (idisNVRSet != null)
                {
                    idisNVRSet.Disconnect();
                    idisNVRSet.OnClosing();
                    Controls.Remove(idisNVRSet.Control);
                    idisNVRSet = null;
                    GC.Collect();
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.ITX_NVR)
            {
#if _ITX_NVR_
                if (axitxview1 != null)
                {
                    if (axitxview1.IsConnected())
                        axitxview1.SessionClose();

                    axitxview1.Dispose();
                    Controls.Remove(axitxview1);
                    axitxview1 = null;
                    GC.Collect();
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.SVMS)
            {
#if _SVMS_
                if (svmsCamera != null)
                {
                    svmsCamera.Close();
                    svmsCamera = null;
                    GC.Collect();
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.Divisys)
            {
#if _Divisys_
                if (divisysCamera != null)
                {
                    divisysCamera.Close();
                    divisysCamera = null;
                    GC.Collect();
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.WESP)
            {
#if _WESP_
                if (wespCamera != null)
                {
                    wespCamera.Close();
                    wespCamera = null;
                    GC.Collect();
                }
#endif
            }

            m_bIsConnected = false;
        }               
        
        public void Disconnect()
        {
            SetDisable();
            m_bIsConnected = false;
        }

        public void ChangeType(CCTVTypes type)
        {
            SetDisable();

            if (type == CCTVTypes.Axis)
            {
#if _AXIS_
                axAxisMediaControl1.StretchToFit = true;
                //axAxisMediaControl1.MaintainAspectRatio = false;
                //axAxisMediaControl1.ShowStatusBar = false;
                //axAxisMediaControl1.BackgroundColor = ColorToInt(Color.Red);
                //axAxisMediaControl1.VideoRenderer = (int)AMC_VIDEO_RENDERER.AMC_VIDEO_RENDERER_VMR9;
                //axAxisMediaControl1.EnableOverlays = true;
                ///axAxisMediaControl1.EnableContextMenu = false;
               // axAxisMediaControl1.ToolbarConfiguration = "+play,+fullscreen,-settings"; //"-pixcount" to remove pixel counter
                if (axAxisMediaControl1 != null)
                {
                    axAxisMediaControl1.StretchToFit = true;
                    axAxisMediaControl1.MaintainAspectRatio = false;
                    axAxisMediaControl1.VideoRenderer = (int)AMC_VIDEO_RENDERER.AMC_VIDEO_RENDERER_VMR7;
                    axAxisMediaControl1.MJPEGVideoRenderer = (int)AMC_VIDEO_RENDERER.AMC_VIDEO_RENDERER_VMR7;
                    axAxisMediaControl1.OnMouseDown += new AxAXISMEDIACONTROLLib._IAxisMediaControlEvents_OnMouseDownEventHandler(axAxisMediaControl1_OnMouseDown);
                    axAxisMediaControl1.OnDoubleClick += new AxAXISMEDIACONTROLLib._IAxisMediaControlEvents_OnDoubleClickEventHandler(axAxisMediaControl1_OnDoubleClick);

                    axAxisMediaControl1.Visible = true;
                    axAxisMediaControl1.Dock = DockStyle.Fill;
                    axAxisMediaControl1.BringToFront();
                    axAxisMediaControl1.Refresh();
                }      
#endif
            }
            else if (type == CCTVTypes.NVS || type == CCTVTypes.NVT)
            {
#if _NVS_
                if (axNVSViewerCtrl1 != null)
                {
                    axNVSViewerCtrl1.MouseDownEvent += new AxNVSVIEWERCTRLLib._DNVSViewerCtrlEvents_MouseDownEventHandler(axNVSViewerCtrl1_MouseDownEvent);

                    axNVSViewerCtrl1.BkColor = ColorToInt(Color.Black);
                    axNVSViewerCtrl1.Visible = true;
                    axNVSViewerCtrl1.Dock = DockStyle.Fill;
                    axNVSViewerCtrl1.BringToFront();

                    axNVSViewerCtrl1.Refresh();
                }
#endif
            }
            else if (type == CCTVTypes.XpressStrm)
            {                
                //axxpressStrm1.Visible = true;
                //axxpressStrm1.Dock = DockStyle.Fill;
                //axxpressStrm1.BringToFront();
            }
            else if (type == CCTVTypes.UDP)
            {
#if _UDP_
                if( axAxVCA1 != null)
                {
                    axAxVCA1.OnEvent += new AxAXVCALib._DAxVCAEvents_OnEventEventHandler(axAxVCA1_OnEvent);

                    axAxVCA1.StretchToFit = true;
                    axAxVCA1.Visible = true;
                    axAxVCA1.Dock = DockStyle.Fill;
                    axAxVCA1.BringToFront();
                    axAxVCA1.Refresh();
                }
#endif
            }
            else if (type == CCTVTypes.Panasonic)
            {
#if _Panasonic_
                if (axipropsapiCtrl1 != null)
                {
                    axipropsapiCtrl1.DblClickEnable = 1;
                    axipropsapiCtrl1.MouseDownEnable = 1;
                    axipropsapiCtrl1.DblClick += new AxIPROPSAPILib._IipropsapiCtrlEvents_DblClickEventHandler(axipropsapiCtrl1_DblClick);
                    axipropsapiCtrl1.MouseDownEvent += new AxIPROPSAPILib._IipropsapiCtrlEvents_MouseDownEventHandler(axipropsapiCtrl1_MouseDownEvent);

                    axipropsapiCtrl1.DeviceType = 2;
                    axipropsapiCtrl1.HttpPort = 80;
                    //Set properties for display area
                    axipropsapiCtrl1.StreamFormat = 0;
                    axipropsapiCtrl1.JPEGResolution = 640;
                    axipropsapiCtrl1.MPEG4Resolution = 640;
                    axipropsapiCtrl1.H264Resolution = 640;

                    //Set properties for event
                    axipropsapiCtrl1.OnErrorEnable = 0;
                    axipropsapiCtrl1.OnDevStatusEnable = 0;
                    axipropsapiCtrl1.OnRecStatusEnable = 0;
                    axipropsapiCtrl1.OnPlayStatusEnable = 0;
                    axipropsapiCtrl1.OnImageRefreshEnable = 0;
                    axipropsapiCtrl1.OnRecordStatusEnable = 0;
                    axipropsapiCtrl1.OnOpStatusEnable = 0;
                    axipropsapiCtrl1.OnAlmStatusEnable = 0;

                    axipropsapiCtrl1.OnRecStatusCBEnable = 0;
                    axipropsapiCtrl1.OnSearchCBEnable = 0;
                    axipropsapiCtrl1.OnSearchExCBEnable = 0;
                    axipropsapiCtrl1.OnPlayStatusCBEnable = 0;
                    axipropsapiCtrl1.OnOpStatusCBEnable = 0;
                    axipropsapiCtrl1.OnAlmStatusCBEnable = 0;
                    axipropsapiCtrl1.OnFtpStatusCBEnable = 0;
                    axipropsapiCtrl1.PictureFitMode = 1;

                    axipropsapiCtrl1.Visible = true;
                    axipropsapiCtrl1.Dock = DockStyle.Fill;
                    axipropsapiCtrl1.BringToFront();
                    axipropsapiCtrl1.Refresh();
                }
#endif
            }
            else if( type == CCTVTypes.TechWin)
            {
#if _TechWin_
                if (axTechWinLib1 != null)
                {
                    //axTechWinLib1.
                    axTechWinLib1.Visible = true;
                    axTechWinLib1.Dock = DockStyle.Fill;
                    axTechWinLib1.BringToFront();
                }
#endif
            }
            else if( type == CCTVTypes.IPVideo)
            {
#if _IPVideo_
                if (axTVSLiveControl1 != null)
                {
                    axTVSLiveControl1.OnMouseEvent += new AxTVSLib._ITVSLiveControlEvents_OnMouseEventEventHandler(axTVSLiveControl1_OnMouseEvent);

                    axTVSLiveControl1.Visible = true;
                    axTVSLiveControl1.Dock = DockStyle.Fill;
                    axTVSLiveControl1.BringToFront();
                    axTVSLiveControl1.Refresh();
                }
#endif
            }
            else if (type == CCTVTypes.MediaPlayer)
            {
                if (axWindowsMediaPlayer1 != null)
                {
                    axWindowsMediaPlayer1.Visible = true;
                    axWindowsMediaPlayer1.Size = new Size(this.Width, this.Height + MEDIA_PLAYER_HIDDEN_SIZE);
                    //axWindowsMediaPlayer1.Dock = DockStyle.Fill;
                    axWindowsMediaPlayer1.BringToFront();
                    axWindowsMediaPlayer1.Refresh();
                }
            }
            else if (type == CCTVTypes.IDIS)
            {
#if _IDIS_
                if (axRASplus_WatSear1 != null)
                {
                    axRASplus_WatSear1.Visible = true;
                    axRASplus_WatSear1.Dock = DockStyle.Fill;
                    axRASplus_WatSear1.BringToFront();

                    axRASplus_WatSear1.Refresh();
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.RTSP)
            {
                if (vlcControl2 != null)
                {
                    m_panelRTSPNoConnect.Visible = false;
                    vlcControl2.Visible = true;
                    vlcControl2.Dock = DockStyle.Fill;
                    vlcControl2.BringToFront();
                    vlcControl2.Refresh();
                }
                /*if (streamPlayerControl != null)
                {
                    m_panelRTSPNoConnect.Visible = false;
                    streamPlayerControl.Visible = true;
                    streamPlayerControl.Dock = DockStyle.Fill;
                    streamPlayerControl.BringToFront();
                    streamPlayerControl.Refresh();
                }*/
            }

            else if (mCurTypes == CCTVTypes.RTSPONVIF)
            {
                if (vlcControl1 != null)
                {
                    vlcControl1.Visible = true;
                    vlcControl1.Dock = DockStyle.Fill;
                    vlcControl1.BringToFront();
                    vlcControl1.Refresh();
                }
            }
            else if (mCurTypes == CCTVTypes.IDIS_NVR)
            {
#if _IDIS_NVR_
                if (idisNVRSet != null && idisNVRSet.Control != null)
                {
                    idisNVRSet.Control.Size = new Size(this.Width, this.Height);
                    idisNVRSet.Control.Visible = true;
                    idisNVRSet.Control.BringToFront();
                    idisNVRSet.Control.Refresh();
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.ITX_NVR)
            {
#if _ITX_NVR_
                if (axitxview1 != null)
                {
                    axitxview1.Dock = DockStyle.Fill;
                    axitxview1.Visible = true;
                    axitxview1.BringToFront();
                    axitxview1.Refresh();
                }
#endif
            }

            mCurTypes = type;
        }

        void axWindowsMediaPlayer1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (e.newState == (int)WMPLib.WMPPlayState.wmppsPlaying)
                m_bIsConnected = true;
            else if (e.newState == (int)WMPLib.WMPPlayState.wmppsReady)
                m_bIsConnected = false;
        }

        void axWindowsMediaPlayer1_ClickEvent(object sender, AxWMPLib._WMPOCXEvents_ClickEvent e)
        {
            if (e.nButton == 1) // 1(Left), 2(Right), 4(Middle)
            {
                OnMouseDown(sender, System.Windows.Forms.MouseButtons.Left, e.fX, e.fY);
                /*if (m_owner != null)
                    m_owner.OnMouseLButtonClick();*/
            }
        }

        void axWindowsMediaPlayer1_DoubleClickEvent(object sender, AxWMPLib._WMPOCXEvents_DoubleClickEvent e)
        {
            if (e.nButton == 1) // 1(Left), 2(Right), 4(Middle)
            {
                OnMouseDblClick(sender, MouseButtons.Left, e.fX, e.fY);
                /*if (m_owner != null)
                    m_owner.OnMouseLButtonDoubleClick();*/
            }
        }
#if _AXIS_
        void axAxisMediaControl1_OnDoubleClick(object sender, AxAXISMEDIACONTROLLib._IAxisMediaControlEvents_OnDoubleClickEvent e)
        {
            if (e.nButton == 1)
            {
                OnMouseDblClick(sender, MouseButtons.Left, e.fX, e.fY);
                /*if (m_owner != null)
                    m_owner.OnMouseLButtonDoubleClick();*/
                //System.Diagnostics.Trace.WriteLine("Axis MouseDblClick : " + e.nButton.ToString());
            }
        }

        void axAxisMediaControl1_OnMouseDown(object sender, AxAXISMEDIACONTROLLib._IAxisMediaControlEvents_OnMouseDownEvent e)
        {
            if (e.nButton == 1)
            {
                OnMouseDown(sender, System.Windows.Forms.MouseButtons.Left, e.fX, e.fY);
                /*if (m_owner != null)
                    m_owner.OnMouseLButtonClick();*/
                //System.Diagnostics.Trace.WriteLine("Axis MouseClick : " + e.nButton.ToString());
            }
        }
#endif

#if _Panasonic_
        void axipropsapiCtrl1_DblClick(object sender, AxIPROPSAPILib._IipropsapiCtrlEvents_DblClickEvent e)
        {
            if (e.button == 1)
            {
                OnMouseDblClick(sender, MouseButtons.Left, e.x, e.y);
                /*if (m_owner != null)
                    m_owner.OnMouseLButtonDoubleClick();*/
                //System.Diagnostics.Trace.WriteLine("Panasonic MouseDblClick : " + e.button.ToString());
            }
        }

        void axipropsapiCtrl1_MouseDownEvent(object sender, AxIPROPSAPILib._IipropsapiCtrlEvents_MouseDownEvent e)
        {
            if (e.button == 1)
            {
                OnMouseDown(sender, System.Windows.Forms.MouseButtons.Left, e.x, e.y);
                /*if (m_owner != null)
                    m_owner.OnMouseLButtonClick();*/
                //System.Diagnostics.Trace.WriteLine("Panasonic MouseClick : " + e.button.ToString());
            }
        }
#endif

#if _UDP_
        void axAxVCA1_OnEvent(object sender, AxAXVCALib._DAxVCAEvents_OnEventEvent e)
        {
            if (e.szEvent == "mouse" && e.szSubEvent == "dblclick" && e.szParam.Contains("button=1"))
            {
                OnMouseDblClick(sender, MouseButtons.Left, 0, 0);
                /*if (m_owner != null)
                    m_owner.OnMouseLButtonDoubleClick();*/
                //System.Diagnostics.Trace.WriteLine("UDP MouseDblClick");
            }
            else if (e.szEvent == "mouse" && e.szSubEvent == "click" && e.szParam.Contains("button=1"))
            {
                OnMouseDown(sender, System.Windows.Forms.MouseButtons.Left, 0, 0);
                /*if (m_owner != null)
                    m_owner.OnMouseLButtonClick();*/
                //System.Diagnostics.Trace.WriteLine("UDP MouseClick");
            }
        }
#endif

#if _IPVideo_
        void axTVSLiveControl1_OnMouseEvent(object sender, AxTVSLib._ITVSLiveControlEvents_OnMouseEventEvent e)
        {
            if (e.lMsg == WM_LBUTTONDBLCLK)
            {
                OnMouseDblClick(sender, MouseButtons.Left, 0, 0);
                /*if (m_owner != null)
                    m_owner.OnMouseLButtonDoubleClick();*/
                //System.Diagnostics.Trace.WriteLine("IPVideo MouseDblClick");
            }
            else if (e.lMsg == WM_LBUTTONDOWN)
            {
                OnMouseDown(sender, System.Windows.Forms.MouseButtons.Left, 0, 0);
                /*if (m_owner != null)
                    m_owner.OnMouseLButtonClick();*/
                //System.Diagnostics.Trace.WriteLine("IPVideo MouseClick");
            }
        }
#endif
        // Stream Started
        private void vlcControl_Playing(object sender, Vlc.DotNet.Core.VlcMediaPlayerPlayingEventArgs e)
        {
            m_bIsConnected = true;

            this.Invoke((MethodInvoker)delegate
            {
                vlcControl2.BringToFront();
                m_panelRTSPNoConnect.Hide();
            });
        }

        // Stream Stopped
        private void vlcControl_Stopped(object sender, Vlc.DotNet.Core.VlcMediaPlayerStoppedEventArgs e)
        {
            m_bIsConnected = false;
            m_reconnectMgr.OnStop();
        }

        // Failed
        private void vlcControl_EncounteredError(object sender, Vlc.DotNet.Core.VlcMediaPlayerEncounteredErrorEventArgs e)
        {
            m_bIsConnected = false;
            System.Diagnostics.Trace.WriteLine("vlcControl Stream Failed");
            m_reconnectMgr.OnFail();

            this.Invoke((MethodInvoker)delegate
            {
                m_panelRTSPNoConnect.BringToFront();
                m_panelRTSPNoConnect.Show();
            });
        }

        private void vlcControl_MouseClick(object sender, MouseEventArgs e)
        {
            OnMouseDown(sender, e.Button, e.X, e.Y);
        }

        private void vlcControl_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            OnMouseDblClick(sender, e.Button, e.X, e.Y);
        }

        /*private void streamPlayerControl_MouseClick(object sender, MouseEventArgs e)
        {
            OnMouseDown(sender, e.Button, e.X, e.Y);
        }

        private void streamPlayerControl_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            OnMouseDblClick(sender, e.Button, e.X, e.Y);
        }*/
        private void axVLCPlugin21_MouseClick(object sender, MouseEventArgs e)
        {
            OnMouseDown(sender, e.Button, e.X, e.Y);
        }

        private void axVLCPlugin21_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            OnMouseDblClick(sender, e.Button, e.X, e.Y);
        }

        private void axVLCPlugin21_DragEventHandler(object sender, EventArgs e)
        {

        }

        //private void RTSPStreamStartedEvent(object sender, EventArgs e)
        //{
        //    m_bIsConnected = true;

        //    /*m_labelRTSPStatus.Invoke((MethodInvoker)delegate
        //    {
        //        m_panelRTSPNoConnect.Visible = false;
        //    });*/

        //    streamPlayerControl.BringToFront();
        //    m_panelRTSPNoConnect.Hide();
        //}

        //private void RTSPStreamFailedEvent(object sender, WebEye.Controls.WinForms.StreamPlayerControl.StreamFailedEventArgs e)
        //{
        //    m_bIsConnected = false;
        //    System.Diagnostics.Trace.WriteLine(e.Error);
        //    m_reconnectMgr.OnFail();

        //    /*m_labelRTSPStatus.Invoke((MethodInvoker)delegate
        //    {
        //        m_panelRTSPNoConnect.Visible = true;
        //    });*/

        //    m_panelRTSPNoConnect.BringToFront();
        //    m_panelRTSPNoConnect.Show();
        //}

        //private void RTSPStreamStoppedEvent(object sender, EventArgs e)
        //{
        //    m_bIsConnected = false;
        //    m_reconnectMgr.OnStop();
        //}

        private DateTime m_dtPrevMouseLButtonDown = new DateTime();

        private bool IsDoubleClick()
        {
            DateTime dtNow = DateTime.Now;
            TimeSpan span = dtNow - m_dtPrevMouseLButtonDown;

            m_dtPrevMouseLButtonDown = dtNow;

            if (span.TotalMilliseconds <= 300)
                return true;

            return false;
        }
#if _NVS_
        void axNVSViewerCtrl1_MouseDownEvent(object sender, AxNVSVIEWERCTRLLib._DNVSViewerCtrlEvents_MouseDownEvent e)
        {
            if (e.button == 1)
            {
                if (IsDoubleClick())
                {
                    OnMouseDblClick(sender, MouseButtons.Left, e.x, e.y);
                    /*if (m_owner != null)
                        m_owner.OnMouseLButtonDoubleClick();*/
                    //System.Diagnostics.Trace.WriteLine("NVS MouseDblClick");
                }
                else
                {
                    OnMouseDown(sender, System.Windows.Forms.MouseButtons.Left, e.x, e.y);
                    /*if (m_owner != null)
                        m_owner.OnMouseLButtonClick();*/

                    //System.Diagnostics.Trace.WriteLine("NVS MouseClick");
                }
            }
        }
#endif
        private int ColorToInt(Color color)
        {
            return (int)((color.R << 0) | (color.G << 8) | (color.B << 16));
        }

        
        // for axis
        private string CompleteURL(string theMediaURL, UnE.Control.MediaType theMediaType)
        {
            string anURL = theMediaURL;
            if (!anURL.EndsWith("/")) anURL += "/";

            switch (theMediaType)
            {
                case UnE.Control.MediaType.mjpeg:
                    anURL += "axis-cgi/mjpg/video.cgi";
                    break;
                case UnE.Control.MediaType.mpeg4:
                    anURL += "mpeg4/media.amp";
                    break;
                case UnE.Control.MediaType.h264:
                    anURL += "axis-media/media.amp?videocodec=h264";
                    break;
            }

            anURL = CompleteProtocol(anURL, theMediaType);
            return anURL;
        }

        // for axis
        private string CompleteProtocol(string theMediaURL, MediaType theMediaType)
        {
            if (theMediaURL.IndexOf("://") >= 0) return theMediaURL;

            string anURL = theMediaURL;

            switch (theMediaType)
            {
                case UnE.Control.MediaType.mjpeg:
                    // This example streams Motion JPEG over HTTP multipart (only video)
                    // See docs on how to receive unsynchronized audio with Motion JPEG
                    anURL = "http://" + anURL;
                    break;
                case UnE.Control.MediaType.mpeg4:
                case UnE.Control.MediaType.h264:
                    // Use RTP over RTSP over HTTP (for other transport mechanisms see docs)
                    anURL = "axrtsphttp://" + anURL;
                    break;
            }

            return anURL;
        }

      
        



        #region PTZControl
        private int m_nPtzPresetNum = 1;
        private int m_nPtzSpeed = 50;

        public void Preset(int nNum, string szName)
        {
            // Camera Preset이동
            if (mCurTypes == CCTVTypes.TechWin)
            {
#if _TechWin_
                if (axTechWinLib1 != null)
                {
                    axTechWinLib1.MovePreset(nNum, szName);
                }
#endif
            }           
        }

        public void ZoomIn()
        {
            if (mCurTypes == CCTVTypes.Axis)
            {
#if _AXIS_
                if (axAxisMediaControl1 != null)
                {
                    ptzTimer.Interval = 1000;
                    //ptzTimer.Tick += ptzTimer_Tick;
                    ptzTimer.Start();

                    SetAxisPtzCmd("zoom", "in");
                    System.Threading.Thread.Sleep(100);
                    SetAxisPtzCmd("zoom", "stop");

                    bProcessPtz = true;

                }
#endif
            }
            else if (mCurTypes == CCTVTypes.NVS || mCurTypes == CCTVTypes.NVT)
            {
#if _NVS_
                if (axNVSViewerCtrl1!= null)
                {
                    axNVSViewerCtrl1.Scale(new SizeF(1.1f, 1.1f));
                    //System.Diagnostics.Trace.WriteLine("ZoomIn, NVS, NVT Set scale");
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.XpressStrm)
            {
#if _XpressStrm_
                if (axxpressStrm1.PTZLockStatus == 0)
                {
                    axxpressStrm1.PresetNumber = 1;
                    axxpressStrm1.PTZSpeed = 50;
                    axxpressStrm1.CustomPTZControl(4);
                } 

                //axxpressStrm1.CustomPTZControl(4);
                //System.Diagnostics.Trace.WriteLine("ZoomIn, XpressStrm, CustomPTZControl(4)");
#endif
            }
            else if (mCurTypes == CCTVTypes.UDP)
            {
#if _UDP_
                if( axAxVCA1 != null)
                {
                    axAxVCA1.Scale(new SizeF(1.1f, 1.1f));
                    //System.Diagnostics.Trace.WriteLine("ZoomIn, UDP Set scale");
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.Panasonic)
            {
#if _Panasonic_
                if (axipropsapiCtrl1 != null)
                {
                    axipropsapiCtrl1.DigitalZoom++;
                    //System.Diagnostics.Trace.WriteLine("ZoomIn, UDP Set scale");
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.TechWin)
            {
#if _TechWin_
                if (axTechWinLib1 != null)
                {
                    axTechWinLib1.ZoomIn(1);

                    System.Threading.Thread.Sleep(500);
                    axTechWinLib1.PtzStop(1);
                    //System.Diagnostics.Trace.WriteLine("ZoomIn, TechWin ZoomIn(1)");
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.IPVideo)
            {
#if _IPVideo_
                if (axTVSLiveControl1 != null)
                {
                    //axTVSLiveControl1.SendPTZCommand(nIpvideoCh, 6);
                    //System.Diagnostics.Trace.WriteLine("ZoomIn, IPVideo Channel : " + nIpvideoCh.ToString() + ", ZoomIn(6)");
                    if (axTVSLiveControl1 != null)
                    {
                        //axTVSLiveControl1.SendPTZCommand(nIpvideoCh, 7);

                        int nChannel = 1;
                        if (mProperties.ContainsKey("Channel"))
                        {
                            nChannel = GetInt(mProperties["Channel"]) + 1;

                            SetTruenPtzCmd(nChannel, "zoom", "in");
                            System.Threading.Thread.Sleep(480);
                            SetTruenPtzCmd(nChannel, "move", "stop");

                        }
                        //System.Diagnostics.Trace.WriteLine("ZoomIn, IPVideo Channel : " + nIpvideoCh.ToString() + ", SendPtZCommand(6)");
                    }
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.IDIS)
            {
#if _IDIS_
                if (axRASplus_WatSear1 != null)
                {
                    axRASplus_WatSear1.setPtz(0, 9, 0);
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.RTSP)
            {                
                RTSP_ONVIF.ONVIF_PTZ_Manager.Instance.Move(CCTVID, 200);
            }
            else if (mCurTypes == CCTVTypes.IDIS_NVR)
            {
                // 지원하지 않음
            }
            else if (mCurTypes == CCTVTypes.ITX_NVR)
            {
                // 지원하지 않음
            }
            else if (mCurTypes == CCTVTypes.RTSPONVIF)
            {
                RTSP_ONVIF.ONVIF_PTZ_Manager.Instance.Move(CCTVID, 200);
            }
            else if (mCurTypes == CCTVTypes.SVMS)
            {
#if _SVMS_
                if (svmsCamera != null)
                {
                    svmsCamera.ZoomIn();
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.Divisys)
            {
                // 지원하지 않음
            }
        }

        public void ZoomOut()
        {
            if (mCurTypes == CCTVTypes.Axis)
            {
#if _AXIS_
                if (axAxisMediaControl1 != null)
                {
                    ptzTimer.Interval = 1000;
                   // ptzTimer.Tick += ptzTimer_Tick;
                    ptzTimer.Start();

                    SetAxisPtzCmd("zoom", "out");
                    System.Threading.Thread.Sleep(100);
                    SetAxisPtzCmd("zoom", "stop");

                    bProcessPtz = true;
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.NVS || mCurTypes == CCTVTypes.NVT)
            {
#if _NVS_
                if (axNVSViewerCtrl1 != null)
                {
                    axNVSViewerCtrl1.Scale(new SizeF(0.9f, 0.9f));
                    //System.Diagnostics.Trace.WriteLine("ZoomOut, NVS, NVT Set scale");
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.XpressStrm)
            {
#if _XpressStrm_
                if (axxpressStrm1.PTZLockStatus == 0)
                {
                    axxpressStrm1.PresetNumber = 1;
                    axxpressStrm1.PTZSpeed = 50;
                    axxpressStrm1.CustomPTZControl(6);
                } 

               // axxpressStrm1.CustomPTZControl(6);
                //System.Diagnostics.Trace.WriteLine("ZoomOut, XpressStrm, CustomPTZControl(6)");
#endif
            }
            else if (mCurTypes == CCTVTypes.UDP)
            {
#if _UDP_
                if (axAxVCA1 != null)
                {
                    axAxVCA1.Scale(new SizeF(0.9f, 0.9f));
                    //System.Diagnostics.Trace.WriteLine("ZoomOut, UDP Set scale");
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.Panasonic)
            {
#if _Panasonic_
                if (axipropsapiCtrl1 != null)
                {
                    axipropsapiCtrl1.DigitalZoom--;
                    //System.Diagnostics.Trace.WriteLine("ZoomOut, Panasonic DigitalZoom--");
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.TechWin)
            {
#if _TechWin_
                if (axTechWinLib1 != null)
                {
                    axTechWinLib1.ZoomOut(1);
                    System.Threading.Thread.Sleep(500);
                    //System.Diagnostics.Trace.WriteLine("ZoomOut, TechWin ZoomOut(1)");
                    axTechWinLib1.PtzStop(1);
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.IPVideo)
            {
#if _IPVideo_
                if (axTVSLiveControl1 != null)
                {
                    //axTVSLiveControl1.SendPTZCommand(nIpvideoCh, 7);
                    
                    int nChannel = 1;
                    if (mProperties.ContainsKey("Channel"))
                    {
                        nChannel = GetInt(mProperties["Channel"]) + 1;

                        SetTruenPtzCmd(nChannel, "zoom", "out");
                        System.Threading.Thread.Sleep(480);
                        SetTruenPtzCmd(nChannel, "move", "stop");

                    }                    
                    //System.Diagnostics.Trace.WriteLine("ZoomOut, IPVideo Channel : " + nIpvideoCh.ToString() + ", SendPtZCommand(7)");
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.IDIS)
            {
#if _IDIS_
                if (axRASplus_WatSear1 != null)
                {
                    axRASplus_WatSear1.setPtz(0, 8, 0);
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.RTSP)
            {
                RTSP_ONVIF.ONVIF_PTZ_Manager.Instance.Move(CCTVID, 210);
            }
            else if (mCurTypes == CCTVTypes.IDIS_NVR)
            {
                // 지원하지 않음
            }
            else if (mCurTypes == CCTVTypes.ITX_NVR)
            {
                // 지원하지 않음
            }
            else if( mCurTypes == CCTVTypes.RTSPONVIF)
            {
                RTSP_ONVIF.ONVIF_PTZ_Manager.Instance.Move(CCTVID, 210);
            }
            else if (mCurTypes == CCTVTypes.SVMS)
            {
#if _SVMS_
                if (svmsCamera != null)
                    svmsCamera.ZoomOut();
#endif
            }
            else if (mCurTypes == CCTVTypes.Divisys)
            {
                // 지원하지 않음
            }
        }

        public void MoveLeft()
        {
            if (mCurTypes == CCTVTypes.Axis)
            {
#if _AXIS_
                if (axAxisMediaControl1 != null)
                {
                    ptzTimer.Interval = 1000;
                    //ptzTimer.Tick += ptzTimer_Tick;
                    ptzTimer.Start();

                    if (m_nReversPTZ == 0)
                        SetAxisPtzCmd("move", "left");
                    else
                        SetAxisPtzCmd("move", "right");

                    //SetAxisPtzCmd("move", "left");
                    System.Threading.Thread.Sleep(100);
                    SetAxisPtzCmd("move", "stop");

                    bProcessPtz = true;
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.NVS || mCurTypes == CCTVTypes.NVT)
            {
#if _NVS_
                if (axNVSViewerCtrl1 != null)
                {
                    axNVSViewerCtrl1.PutCamCtlEx(1, 0x2002, 0x000000C4, 3);
                    System.Threading.Thread.Sleep(100);
                    axNVSViewerCtrl1.PutCamCtlEx(1, 0x2002, 0, 3);
                    //System.Diagnostics.Trace.WriteLine("Left, NVS, NVT PutCanCtl(1,1,4)");
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.XpressStrm)
            {
#if _XpressStrm_
                if (axxpressStrm1.PTZLockStatus == 0)
                {
                    axxpressStrm1.PresetNumber = 1;
                    axxpressStrm1.PTZSpeed = 50;
                    axxpressStrm1.CustomPTZControl(0);
                }                 
                //System.Diagnostics.Trace.WriteLine("Left, XpressStrm, CustomPTZControl(0)");
#endif
            }
            else if (mCurTypes == CCTVTypes.UDP)
            {
#if _UDP_
                if (axAxVCA1 != null)
                {
                    //axAxVCA1.SetParamVCA
                    //System.Diagnostics.Trace.WriteLine("Left, UDP No code");
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.Panasonic)
            {
#if _Panasonic_
                if (axipropsapiCtrl1 != null)
                {
                    axipropsapiCtrl1.CameraControl(1, -100, 0, 0, 0, 3);
                    System.Threading.Thread.Sleep(300);
                    axipropsapiCtrl1.CameraControl(1, 0, 0, 0, 0, 0);
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.TechWin)
            {
#if _TechWin_
                if (axTechWinLib1 != null)
                {                    
                    axTechWinLib1.PtzLeft(6);
                    System.Threading.Thread.Sleep(100);
                    axTechWinLib1.PtzStop(0);
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.IPVideo)
            {
#if _IPVideo_
                if (axTVSLiveControl1 != null)
                {

                    int nChannel = 1;
                    if (mProperties.ContainsKey("Channel"))
                    {
                        nChannel = GetInt(mProperties["Channel"]) + 1;

                        SetTruenPtzCmd(nChannel, "move", "left");
                        System.Threading.Thread.Sleep(480);
                        SetTruenPtzCmd(nChannel, "move", "stop");

                    }

                    //axTVSLiveControl1.SendPTZCommand(nIpvideoCh, 2);
                    //System.Diagnostics.Trace.WriteLine("Left, IPVideo Channel : " + nIpvideoCh.ToString() + ", SendPtZCommand(2)");
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.IDIS)
            {
#if _IDIS_
                if (axRASplus_WatSear1 != null)
                {
                    axRASplus_WatSear1.setPtz(0, 6, 0);
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.RTSP)
            {
                RTSP_ONVIF.ONVIF_PTZ_Manager.Instance.Move(CCTVID, 140);
            }
            else if (mCurTypes == CCTVTypes.IDIS_NVR)
            {
                // 지원하지 않음
            }
            else if (mCurTypes == CCTVTypes.ITX_NVR)
            {
                // 지원하지 않음
            }
            else if (mCurTypes == CCTVTypes.RTSPONVIF)
            {
                RTSP_ONVIF.ONVIF_PTZ_Manager.Instance.Move(CCTVID, 140);
            }
            else if (mCurTypes == CCTVTypes.SVMS)
            {
#if _SVMS_
                if (svmsCamera != null)
                    svmsCamera.Move(CCTVControl.SVMSCamera.Direction.LEFT);
#endif
            }
            else if (mCurTypes == CCTVTypes.Divisys)
            {
                // 지원하지 않음
            }
        }

        public void MoveRight()
        {
            if (mCurTypes == CCTVTypes.Axis)
            {
#if _AXIS_
                if (axAxisMediaControl1 != null)
                {
                    ptzTimer.Interval = 1000;
                    ptzTimer.Tick += ptzTimer_Tick;
                    ptzTimer.Start();


                    if (m_nReversPTZ == 0)
                        SetAxisPtzCmd("move", "right");
                    else
                        SetAxisPtzCmd("move", "left");

                    //SetAxisPtzCmd("move", "right");
                    System.Threading.Thread.Sleep(100);
                    SetAxisPtzCmd("move", "stop");
                    bProcessPtz = true;

                }
#endif
            }
            else if (mCurTypes == CCTVTypes.NVS || mCurTypes == CCTVTypes.NVT)
            {
#if _NVS_
                if (axNVSViewerCtrl1 != null)
                {
                    axNVSViewerCtrl1.PutCamCtlEx(1, 0x2002, 0x0000003C, 3);
                    System.Threading.Thread.Sleep(100);
                    axNVSViewerCtrl1.PutCamCtlEx(1, 0x2002, 0, 3);
                    //System.Diagnostics.Trace.WriteLine("Right, NVS, NVT PutCanCtl(1,1,4)");
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.XpressStrm)
            {
#if _XpressStrm_
                if (axxpressStrm1.PTZLockStatus == 0)
                {
                    axxpressStrm1.PresetNumber = 1;
                    axxpressStrm1.PTZSpeed = 50;
                    axxpressStrm1.CustomPTZControl(1);
                } 
                  
                //System.Diagnostics.Trace.WriteLine("Right, XpressStrm, CustomPTZControl(1)");
#endif
            }
            else if (mCurTypes == CCTVTypes.UDP)
            {
#if _UDP_
                if (axAxVCA1 != null)
                {
                    //axAxVCA1.SetParamVCA
                    //System.Diagnostics.Trace.WriteLine("Right, UDP No code");
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.Panasonic)
            {
#if _Panasonic_
                if (axipropsapiCtrl1 != null)
                {
                    axipropsapiCtrl1.CameraControl(1, 100, 0, 0, 0, 3);
                    System.Threading.Thread.Sleep(100);
                    axipropsapiCtrl1.CameraControl(1, 0, 0, 0, 0, 0);
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.TechWin)
            {
#if _TechWin_
                if (axTechWinLib1 != null)
                {
                    axTechWinLib1.PtzRight(6);
                    System.Threading.Thread.Sleep(100);
                    axTechWinLib1.PtzStop(0);
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.IPVideo)
            {
#if _IPVideo_
                if (axTVSLiveControl1 != null)
                {
                    int nChannel = 1;
                    if (mProperties.ContainsKey("Channel"))
                    {
                        nChannel = GetInt(mProperties["Channel"]) + 1;

                        SetTruenPtzCmd(nChannel, "move", "right");
                        System.Threading.Thread.Sleep(480);
                        SetTruenPtzCmd(nChannel, "move", "stop");

                    }

                    //axTVSLiveControl1.SendPTZCommand(nIpvideoCh, 3);
                    //System.Diagnostics.Trace.WriteLine("Right, IPVideo Channel : " + nIpvideoCh.ToString() + ", SendPtZCommand(3)");
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.IDIS)
            {
#if _IDIS_
                if (axRASplus_WatSear1 != null)
                {
                    axRASplus_WatSear1.setPtz(0, 2, 0);
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.RTSP)
            {
                RTSP_ONVIF.ONVIF_PTZ_Manager.Instance.Move(CCTVID, 130);
            }
            else if (mCurTypes == CCTVTypes.IDIS_NVR)
            {
                // 지원하지 않음
            }
            else if (mCurTypes == CCTVTypes.ITX_NVR)
            {
                // 지원하지 않음
            }
            else if (mCurTypes == CCTVTypes.RTSPONVIF)
            {
                RTSP_ONVIF.ONVIF_PTZ_Manager.Instance.Move(CCTVID, 130);
            }
            else if (mCurTypes == CCTVTypes.SVMS)
            {
#if _SVMS_
                if (svmsCamera != null)
                    svmsCamera.Move(CCTVControl.SVMSCamera.Direction.RIGHT);
#endif
            }
            else if (mCurTypes == CCTVTypes.Divisys)
            {
                // 지원하지 않음
            }
        }

        bool bProcessPtz = false;
        System.Windows.Forms.Timer ptzTimer = new System.Windows.Forms.Timer();

        private void SetAxisPtzCmd(string cmd, string dir)
        {
#if _AXIS_
            if (axAxisMediaControl1 != null && bProcessPtz == false)
            {                
                string szPtzURL = "http://" + m_szIPAddress + "/axis-cgi/com/ptz.cgi";

                float nPtzMoveValue = 70.0f;
                float nPtzZoomValue = 70.0f;
                string addUrl = "";
                
                if( cmd == "move")
                {
                    if( dir == "left")
                    {
                        addUrl = string.Format("continuouspantiltmove=-{0},0&imagerotation=180&mirror=no", nPtzMoveValue);
                    }
                    else if(dir == "right")
                    {
                        addUrl = string.Format("continuouspantiltmove={0},0&imagerotation=180&mirror=no", nPtzMoveValue);
                    }
                    else if(dir == "up")
                    {
                        addUrl = string.Format("continuouspantiltmove=0,{0},0&imagerotation=180&mirror=no", nPtzMoveValue);
                    }
                    else if(dir == "down")
                    {
                        addUrl = string.Format("continuouspantiltmove=0,-{0},0&imagerotation=180&mirror=no", nPtzMoveValue);
                    }
                    if( dir == "stop")
                    {
                       
                        addUrl = "continuouspantiltmove=0,0&imagerotation=180&mirror=no";
                       

                    }
                }
                else if( cmd == "zoom")
                {
                    if( dir == "in")
                        addUrl = string.Format("continuouszoommove={0}&imagerotation=180&mirror=no", nPtzMoveValue);
                    else if(dir == "out")
                        addUrl = string.Format("continuouszoommove=-{0}&imagerotation=180&mirror=no", nPtzMoveValue);
                    else if(dir == "stop")
                        addUrl = "continuouszoommove=0&imagerotation=180&mirror=no";
                }

                addUrl += string.Format("&timestamp={0}", DateTime.Now.Ticks);
             
                string postData = "camera=" + 1 + "&" + addUrl;
                szPtzURL += "?" + postData;

                //if( cmd == "stop")
                {
                    WebBrowser browserTemp = new WebBrowser();
                    string authHdr = "Authorization: Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(m_szUserName + ":" + m_szPassword)) + "\r\n";
                    //browser.Navigate("http://" + m_szIPAddress,null, null, authHdr);
                    //browser.Visible = true;
                    browserTemp.Navigate(szPtzURL, null, null, authHdr);

                }
                //else
                //{
                //    HttpWebRequest wReq = (HttpWebRequest)WebRequest.Create(szPtzURL);
                //    wReq.Method = "GET";
                //    wReq.Credentials = new NetworkCredential(axAxisMediaControl1.MediaUsername, axAxisMediaControl1.MediaPassword);
                //    wReq.PreAuthenticate = true;
                //    wReq.Timeout = 1000;
                //    try
                //    {
                //        HttpWebResponse wRes = (HttpWebResponse)wReq.GetResponse();
                //    }
                //    catch (System.Net.WebException e)
                //    {
                //        System.Diagnostics.Trace.WriteLine(e.Message);
                //        System.Diagnostics.Trace.WriteLine(e.StackTrace);
                //    }  
                //}
                             
            }
#endif
        }

        private CookieContainer cookie = new CookieContainer();
        private WebBrowser browser = new WebBrowser();
        private void SetTruenPtzCmd(int nChannel, string cmd, string dir)
        {
#if _IPVideo_
            if (axTVSLiveControl1 != null && bProcessPtz == false)
            {
                string szPtzURL = "http://" + m_szIPAddress + "/httpapx/SendPTZ";

                float nPtzMoveValue = 2.0f;
                float nPtzZoomValue = 5.0f;

                string addUrl = "";

                addUrl = string.Format("action=sendptz&PTZ_CHANNEL={0}&", nChannel);
                if (cmd == "move")
                {
                    if (dir == "stop")
                    {
                        addUrl += string.Format("PTZ_MOVE=stop", nChannel);
                    }
                    else
                    {
                        addUrl += string.Format("PTZ_MOVE={0},{1}&PTZ_TIMEOUT=5000", dir, nPtzMoveValue);
                    }
                }
                else if (cmd == "zoom")
                {                  
                    if (dir == "in")
                        addUrl = "PTZ_MOVE=zoomin,-1";
                    else if (dir == "out")
                        addUrl = "PTZ_MOVE=zoomin,1";
                }
                                
                szPtzURL += "?" + addUrl;
                System.Diagnostics.Trace.WriteLine(szPtzURL);
                
                if( dir == "stop")
                {
                   
                    string authHdr = "Authorization: Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(m_szUserName + ":" + m_szPassword)) + "\r\n";
                    //browser.Navigate("http://" + m_szIPAddress,null, null, authHdr);
                    browser.Visible = true;
                    browser.Navigate(szPtzURL, null, null, authHdr);
                    
                }
                else
                {
                    HttpWebRequest wReq = (HttpWebRequest)WebRequest.Create(szPtzURL);
                    wReq.Method = "GET";
                    wReq.UserAgent = "Mozilla/4.0";
                    wReq.CookieContainer = cookie;
                    wReq.ContentType = "text/plain";
                    //if(dir != "stop")
                    {
                        wReq.Credentials = new NetworkCredential(m_szUserName, m_szPassword);
                        wReq.PreAuthenticate = true;
                    }

                    wReq.Timeout = 100;
                    try
                    {
                        wReq.BeginGetResponse(null, null);
                    }
                    catch (System.Net.WebException e)
                    {
                        System.Diagnostics.Trace.WriteLine(e.Message);
                        System.Diagnostics.Trace.WriteLine(e.StackTrace);
                    }
                }
               
            }
#endif
        }

        private void FinishRequestStrem(IAsyncResult result)
        {

        }

        void ptzTimer_Tick(object sender, EventArgs e)
        {
            bProcessPtz = false;
            ptzTimer.Stop();
        }
//        <map name="btns2">
//    <area shape="rect" coords="21,6, 40,24" href="#" alt="Zoom In" onMouseDown="ZoonIn(0);" onMouseUp="ZoonIn(1);" OnMouseOut="ZoonIn(1);">
//    <area shape="rect" coords="21,45, 40,63" href="#" alt="Zoom Out" onMouseDown="ZoonOut(0)" onMouseUp="ZoonOut(1);" OnMouseOut="ZoonOut(1);">
//    <area shape="rect" coords="73,6, 94,24" href="#" alt="Tilt Up" onMouseDown="NVSControl.PutCamCtlEx(1, 0x2002,    0x00003C00, 3);" onMouseUp="NVSControl.PutCamCtlEx(1, 0x2002, '000000', 3);" OnMouseOut="NVSControl.PutCamCtlEx(1, 0x2002, '000000', 3);">
//    <area shape="rect" coords="73,45, 94,63" href="#" alt="Tilt Down" onMouseDown="NVSControl.PutCamCtlEx(1, 0x2002, 0x0000C400, 3);" onMouseUp="NVSControl.PutCamCtlEx(1, 0x2002, '000000', 3);" OnMouseOut="NVSControl.PutCamCtlEx(1, 0x2002, '000000', 3);">
//    <area shape="rect" coords="54,23, 75,44" href="#" alt="Pan Left" onMouseDown="NVSControl.PutCamCtlEx(1, 0x2002, 0x000000C4, 3);" onMouseUp="NVSControl.PutCamCtlEx(1, 0x2002, '000000', 3);" OnMouseOut="NVSControl.PutCamCtlEx(1, 0x2002, '000000', 3);">
//    <area shape="rect" coords="92,23, 114,44" href="#" alt="Pan Right" onMouseDown="NVSControl.PutCamCtlEx(1, 0x2002,0x0000003C, 3);" onMouseUp="NVSControl.PutCamCtlEx(1, 0x2002, '000000', 3);" OnMouseOut="NVSControl.PutCamCtlEx(1, 0x2002, '000000', 3);">
    
//    <area shape="rect" coords="127,6, 147,24" href="#" alt="Focus Up" onMouseDown="NVSControl.PutCamCtlEx(1, 0x2001, 0x3C, 1);" onMouseUp="NVSControl.PutCamCtlEx(1, 0x2001, '00', 1);" OnMouseOut="NVSControl.PutCamCtlEx(1, 0x2001, '00', 1);">
//    <area shape="rect" coords="127,45, 147,63" href="#" alt="Focus Down" onMouseDown="NVSControl.PutCamCtlEx(1, 0x2001, 0xC4, 1);" onMouseUp="NVSControl.PutCamCtlEx(1, 0x2001, '00', 1);" OnMouseOut="NVSControl.PutCamCtlEx(1, 0x2001, '00', 1);">
//</map>zx

        public void TestStop(int nCmd)
        {            
            
            if (mCurTypes == CCTVTypes.XpressStrm)
            {
#if _XpressStrm_
                if (axxpressStrm1!= null)
                {
                    if (axxpressStrm1.PTZLockStatus == 0)
                    {
                        axxpressStrm1.PresetNumber = 1;
                        axxpressStrm1.PTZSpeed = 50;
                        axxpressStrm1.CustomPTZControl((short)nCmd);
                    }
                }
#endif
            }
            else if(mCurTypes == CCTVTypes.TechWin)
            {
#if _TechWin_
                if( axTechWinLib1 != null)
                {
                    axTechWinLib1.PtzStop(1);
                }
#endif
            }
        }

        public void MoveStop()
        {
            if (mCurTypes == CCTVTypes.RTSP)
            {                              
            }
            else if (mCurTypes == CCTVTypes.RTSPONVIF)
            {
            }
        }

        public void MoveUp()
        {
            if (mCurTypes == CCTVTypes.Axis)
            {
#if _AXIS_
                ptzTimer.Start();
                if(m_nReversPTZ == 0)
                    SetAxisPtzCmd("move", "up");
                else
                    SetAxisPtzCmd("move", "down");
                System.Threading.Thread.Sleep(100);
                
                SetAxisPtzCmd("move", "stop");

                bProcessPtz = true;
#endif
            }
            else if (mCurTypes == CCTVTypes.NVS || mCurTypes == CCTVTypes.NVT)
            {
#if _NVS_
                if (axNVSViewerCtrl1 != null)
                {
                    axNVSViewerCtrl1.PutCamCtlEx(1, 0x2002, 0x00003C00, 3);
                    System.Threading.Thread.Sleep(100);
                    axNVSViewerCtrl1.PutCamCtlEx(1, 0x2002, 0, 3);
                    //System.Diagnostics.Trace.WriteLine("Up, NVS, NVT PutCanCtl(1,1,4)");
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.XpressStrm)
            {
#if _XpressStrm_
                if (axxpressStrm1.PTZLockStatus == 0)
                {
                    axxpressStrm1.PresetNumber = 1;
                    axxpressStrm1.PTZSpeed = 50;
                    axxpressStrm1.CustomPTZControl(2);

                    
                } 

                //axxpressStrm1.CustomPTZControl(2);
                //System.Diagnostics.Trace.WriteLine("Up, XpressStrm, CustomPTZControl(2)");
#endif
            }
            else if (mCurTypes == CCTVTypes.UDP)
            {
#if _UDP_
                if (axAxVCA1 != null)
                {
                    //axAxVCA1.SetParamVCA
                    //System.Diagnostics.Trace.WriteLine("Up, UDP No code");
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.Panasonic)
            {
#if _Panasonic_
                if (axipropsapiCtrl1 != null)
                {
                    axipropsapiCtrl1.CameraControl(1, 0, -100, 0, 0, 0);
                    System.Threading.Thread.Sleep(100);
                    axipropsapiCtrl1.CameraControl(1, 0, 0, 0, 0, 0);
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.TechWin)
            {
#if _TechWin_
                if (axTechWinLib1 != null)
                {
                    axTechWinLib1.PtzUp(6);
                    System.Threading.Thread.Sleep(100);
                    axTechWinLib1.PtzStop(0);
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.IPVideo)
            {
#if _IPVideo_
                if (axTVSLiveControl1 != null)
                {

                    int nChannel = 1;
                    if (mProperties.ContainsKey("Channel"))
                    {
                        nChannel = GetInt(mProperties["Channel"]) + 1;

                        SetTruenPtzCmd(nChannel, "move", "up");
                        System.Threading.Thread.Sleep(100);
                        SetTruenPtzCmd(nChannel, "move", "stop");

                    }
                  
                    //axTVSLiveControl1.SendPTZCommand(nIpvideoCh, 4);
                    //System.Diagnostics.Trace.WriteLine("Up, IPVideo Channel : " + nIpvideoCh.ToString() + ", SendPtZCommand(4)");
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.IDIS)
            {
#if _IDIS_
                if (axRASplus_WatSear1 != null)
                {
                    axRASplus_WatSear1.setPtz(0, 0, 0);
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.RTSP)
            {              
                RTSP_ONVIF.ONVIF_PTZ_Manager.Instance.Move(CCTVID, 110);
            }
            else if (mCurTypes == CCTVTypes.IDIS_NVR)
            {
                // 지원하지 않음
            }
            else if (mCurTypes == CCTVTypes.ITX_NVR)
            {
                // 지원하지 않음
            }
            else if (mCurTypes == CCTVTypes.RTSPONVIF)
            {
                RTSP_ONVIF.ONVIF_PTZ_Manager.Instance.Move(CCTVID, 110);
            }
            else if (mCurTypes == CCTVTypes.SVMS)
            {
#if _SVMS_
                if (svmsCamera != null)
                    svmsCamera.Move(CCTVControl.SVMSCamera.Direction.UP);
#endif
            }
            else if (mCurTypes == CCTVTypes.Divisys)
            {
                // 지원하지 않음
            }
        }

        public void MoveDown()
        {
            if (mCurTypes == CCTVTypes.Axis)
            {
#if _AXIS_
                if (axAxisMediaControl1 != null)
                {
                    ptzTimer.Interval = 1000;
                    ptzTimer.Tick += ptzTimer_Tick;
                    ptzTimer.Start();

                    if (m_nReversPTZ == 0)
                        SetAxisPtzCmd("move", "down");
                    else
                        SetAxisPtzCmd("move", "up");


                    System.Threading.Thread.Sleep(100);
                    SetAxisPtzCmd("move", "stop");

                    bProcessPtz = true;
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.NVS || mCurTypes == CCTVTypes.NVT)
            {
#if _NVS_
                if (axNVSViewerCtrl1 != null)
                {
                    axNVSViewerCtrl1.PutCamCtlEx(1, 0x2002, 0x0000C400, 3);
                    System.Threading.Thread.Sleep(100);
                    axNVSViewerCtrl1.PutCamCtlEx(1, 0x2002, 0, 3);
                    //System.Diagnostics.Trace.WriteLine("Down, NVS, NVT PutCanCtl(1,1,4)");
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.XpressStrm)
            {
#if _XpressStrm_
                if (axxpressStrm1.PTZLockStatus == 0)
                {
                    axxpressStrm1.PresetNumber = 1;
                    axxpressStrm1.PTZSpeed = 50;
                    axxpressStrm1.CustomPTZControl(3);
                }               
                //System.Diagnostics.Trace.WriteLine("Down, XpressStrm, CustomPTZControl(3)");
#endif
            }
            else if (mCurTypes == CCTVTypes.UDP)
            {
#if _UDP_
                if (axAxVCA1 != null)
                {
                    //axAxVCA1.SetParamVCA
                    //System.Diagnostics.Trace.WriteLine("Down, UDP No code");
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.Panasonic)
            {
#if _Panasonic_
                if (axipropsapiCtrl1 != null)
                {
                    axipropsapiCtrl1.CameraControl(1, 0, 100, 0, 0, 3);
                    System.Threading.Thread.Sleep(100);
                    axipropsapiCtrl1.CameraControl(1, 0, 0, 0, 0, 0);
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.TechWin)
            {
#if _TechWin_
                if (axTechWinLib1 != null)
                {
                    axTechWinLib1.PtzDown(6);
                    System.Threading.Thread.Sleep(100);
                    axTechWinLib1.PtzStop(0);
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.IPVideo)
            {
#if _IPVideo_
                if (axTVSLiveControl1 != null)
                {
                    int nChannel = 1;
                    if (mProperties.ContainsKey("Channel"))
                    {
                        nChannel = GetInt(mProperties["Channel"]) + 1;

                        SetTruenPtzCmd(nChannel, "move", "down");
                        System.Threading.Thread.Sleep(480);
                        SetTruenPtzCmd(nChannel, "move", "stop");
                        
                    }
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.IDIS)
            {
#if _IDIS_
                if (axRASplus_WatSear1 != null)
                {
                    axRASplus_WatSear1.setPtz(0, 4, 0);
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.RTSP)
            {
                RTSP_ONVIF.ONVIF_PTZ_Manager.Instance.Move(CCTVID, 120);
            }
            else if (mCurTypes == CCTVTypes.IDIS_NVR)
            {
                // 지원하지 않음
            }
            else if (mCurTypes == CCTVTypes.ITX_NVR)
            {
                // 지원하지 않음
            }
            else if (mCurTypes == CCTVTypes.RTSPONVIF)
            {
                RTSP_ONVIF.ONVIF_PTZ_Manager.Instance.Move(CCTVID, 120);
            }
            else if (mCurTypes == CCTVTypes.SVMS)
            {
#if _SVMS_
                if (svmsCamera != null)
                    svmsCamera.Move(CCTVControl.SVMSCamera.Direction.DOWN);
#endif
            }
            else if (mCurTypes == CCTVTypes.Divisys)
            {
                // 지원하지 않음
            }
        }
        

        #endregion//PTZContorl


        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (m_reconnectMgr != null)
                m_reconnectMgr.ReleaseThread();

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitActiveXControl(CCTVTypes type)
        {
            if( type == CCTVTypes.None || type == CCTVTypes.NotSet || type == CCTVTypes.HIK)
            {
                panel1.BringToFront();
            }
            else
            {
                SetActiveX(type);
                panel1.SendToBack();
            } 
        }

        public void SetActiveX(CCTVTypes type)        
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CCTVCtrl));
            components = new System.ComponentModel.Container();

            if (type == CCTVTypes.Axis)
            {
#if _AXIS_
                this.axAxisMediaControl1 = new AxAXISMEDIACONTROLLib.AxAxisMediaControl();
                ((System.ComponentModel.ISupportInitialize)(this.axAxisMediaControl1)).BeginInit();
                this.SuspendLayout();
            
                // 
                // axAxisMediaControl1
                // 
                this.axAxisMediaControl1.Enabled = true;
                this.axAxisMediaControl1.Location = new System.Drawing.Point(0, 0);
                this.axAxisMediaControl1.Name = "axAxisMediaControl1";
                this.axAxisMediaControl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axAxisMediaControl1.OcxState")));
                this.axAxisMediaControl1.Size = new System.Drawing.Size(Width, Height);
                this.axAxisMediaControl1.TabIndex = 7;
                axAxisMediaControl1.Dock = DockStyle.Fill;
                Controls.Add(this.axAxisMediaControl1);
                ((System.ComponentModel.ISupportInitialize)(this.axAxisMediaControl1)).EndInit();                
#endif
            }
            else if (type == CCTVTypes.NVS || type == CCTVTypes.NVT)
            {
#if _NVS_
                this.axNVSViewerCtrl1 = new AxNVSVIEWERCTRLLib.AxNVSViewerCtrl();
                ((System.ComponentModel.ISupportInitialize)(this.axNVSViewerCtrl1)).BeginInit();
                
                this.axNVSViewerCtrl1.Enabled = true;
                this.axNVSViewerCtrl1.Location = new System.Drawing.Point(0, 0);
                this.axNVSViewerCtrl1.Name = "axNVSViewerCtrl1";
                this.axNVSViewerCtrl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axNVSViewerCtrl1.OcxState")));
                this.axNVSViewerCtrl1.Size = new System.Drawing.Size(Width, Height);
                this.axNVSViewerCtrl1.TabIndex = 1;
                axNVSViewerCtrl1.Dock = DockStyle.Fill;
                Controls.Add(this.axNVSViewerCtrl1);
                ((System.ComponentModel.ISupportInitialize)(this.axNVSViewerCtrl1)).EndInit();
#endif
            }
            else if (type == CCTVTypes.XpressStrm)
            {
#if _XpressStrm_
                this.axxpressStrm1 = new AxxpressStrmLib.AxxpressStrm();
                ((System.ComponentModel.ISupportInitialize)(this.axxpressStrm1)).BeginInit();
                this.SuspendLayout();
                // 
                // axxpressStrm1
                // 
                this.axxpressStrm1.Enabled = true;
                this.axxpressStrm1.Location = new System.Drawing.Point(0, 0);
                this.axxpressStrm1.Name = "axxpressStrm1";
                this.axxpressStrm1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axxpressStrm1.OcxState")));
                this.axxpressStrm1.Size = new System.Drawing.Size(Width, Height);
                this.axxpressStrm1.TabIndex = 0;

                axxpressStrm1.Notify += axxpressStrm1_Notify;
                axxpressStrm1.MouseCaptureChanged += axxpressStrm1_MouseCaptureChanged;
                axxpressStrm1.GotFocus += axxpressStrm1_GotFocus;

                axxpressStrm1.Visible = true;
                axxpressStrm1.Dock = DockStyle.Fill;
                Controls.Add(this.axxpressStrm1);
                ((System.ComponentModel.ISupportInitialize)(this.axxpressStrm1)).EndInit();

                panel1.Visible = false;

                m_msgFilter.Owner = m_owner;
                Application.AddMessageFilter(m_msgFilter);
#endif
            }
            else if (type == CCTVTypes.UDP)
            {
#if _UDP_
                this.axAxVCA1 = new AxAXVCALib.AxAxVCA();            
                ((System.ComponentModel.ISupportInitialize)(this.axAxVCA1)).BeginInit();
                this.SuspendLayout();           
                // 
                // axAxVCA1
                // 
                this.axAxVCA1.Enabled = true;
                this.axAxVCA1.Location = new System.Drawing.Point(0, 0);
                this.axAxVCA1.Name = "axAxVCA1";
                this.axAxVCA1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axAxVCA1.OcxState")));
                this.axAxVCA1.Size = new System.Drawing.Size(Width, Height);
                this.axAxVCA1.TabIndex = 0;
                axAxVCA1.Dock = DockStyle.Fill;
                Controls.Add(this.axAxVCA1);
                ((System.ComponentModel.ISupportInitialize)(this.axAxVCA1)).EndInit();
#endif
            }
            else if (type == CCTVTypes.Panasonic)
            {
#if _Panasonic_
                this.axipropsapiCtrl1 = new AxIPROPSAPILib.AxipropsapiCtrl();
                ((System.ComponentModel.ISupportInitialize)(this.axipropsapiCtrl1)).BeginInit();
                this.SuspendLayout();
                // 
                // axipropsapiCtrl1
                // 
                this.axipropsapiCtrl1.Enabled = true;
                this.axipropsapiCtrl1.Location = new System.Drawing.Point(0, 0);
                this.axipropsapiCtrl1.Name = "axipropsapiCtrl1";
                this.axipropsapiCtrl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axipropsapiCtrl1.OcxState")));
                this.axipropsapiCtrl1.Size = new System.Drawing.Size(Width, Height);
                this.axipropsapiCtrl1.TabIndex = 4;
                axipropsapiCtrl1.Dock = DockStyle.Fill;
                Controls.Add(this.axipropsapiCtrl1);
                ((System.ComponentModel.ISupportInitialize)(this.axipropsapiCtrl1)).EndInit();
#endif
            }
            else if( type == CCTVTypes.TechWin)
            {
#if _TechWin_
                this.axTechWinLib1 = new Axwebviewer_activexplugin_libLib.Axwebviewer_activexplugin_lib();
                ((System.ComponentModel.ISupportInitialize)(this.axTechWinLib1)).BeginInit();
                this.SuspendLayout();            
                // 
                // axTechWinLib1
                // 
                this.axTechWinLib1.Enabled = true;
                this.axTechWinLib1.Location = new System.Drawing.Point(0, 0);
                this.axTechWinLib1.Name = "axTechWinLib1";
                this.axTechWinLib1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axTechWinLib1.OcxState")));
                this.axTechWinLib1.Size = new System.Drawing.Size(Width, Height);
                this.axTechWinLib1.TabIndex = 0;
                axTechWinLib1.Dock = DockStyle.Fill;
                Controls.Add(this.axTechWinLib1);
                ((System.ComponentModel.ISupportInitialize)(this.axTechWinLib1)).EndInit();
#endif
            }
            else if( type == CCTVTypes.IPVideo)
            {
#if _IPVideo_
                this.axTVSLiveControl1 = new AxTVSLib.AxTVSLiveControl();
                ((System.ComponentModel.ISupportInitialize)(this.axTVSLiveControl1)).BeginInit();
                  this.SuspendLayout();
                // 
                // axTVSLiveControl1
                // 
                this.axTVSLiveControl1.Enabled = true;
                this.axTVSLiveControl1.Location = new System.Drawing.Point(0, 0);
                this.axTVSLiveControl1.Name = "axTVSLiveControl1";
                this.axTVSLiveControl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axTVSLiveControl1.OcxState")));
                this.axTVSLiveControl1.Size = new System.Drawing.Size(Width, Height);
                this.axTVSLiveControl1.TabIndex = 0;
                axTVSLiveControl1.Dock = DockStyle.Fill;
                Controls.Add(this.axTVSLiveControl1);
                ((System.ComponentModel.ISupportInitialize)(this.axTVSLiveControl1)).EndInit();
#endif
            }
            else if (type == CCTVTypes.MediaPlayer)
            {
                this.axWindowsMediaPlayer1 = new AxWMPLib.AxWindowsMediaPlayer();
                //((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).BeginInit();
                this.SuspendLayout();

                // 
                // windowsMediaPlayer
                // 
                this.axWindowsMediaPlayer1.Enabled = true;
                this.axWindowsMediaPlayer1.Location = new System.Drawing.Point(0, 0);
                this.axWindowsMediaPlayer1.Name = "axWindowsMediaPlayer1";
                this.axWindowsMediaPlayer1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axWindowsMediaPlayer1.OcxState")));
                this.axWindowsMediaPlayer1.Size = new System.Drawing.Size(642, 510 + MEDIA_PLAYER_HIDDEN_SIZE);
                this.axWindowsMediaPlayer1.TabIndex = 0;

                //this.axWindowsMediaPlayer1.Dock = System.Windows.Forms.DockStyle.Fill;
                
                axWindowsMediaPlayer1.ClickEvent += new AxWMPLib._WMPOCXEvents_ClickEventHandler(axWindowsMediaPlayer1_ClickEvent);
                axWindowsMediaPlayer1.DoubleClickEvent += new AxWMPLib._WMPOCXEvents_DoubleClickEventHandler(axWindowsMediaPlayer1_DoubleClickEvent);
                axWindowsMediaPlayer1.PlayStateChange += axWindowsMediaPlayer1_PlayStateChange;

                this.Controls.Add(this.axWindowsMediaPlayer1);
                //((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).EndInit();

                m_enablePTZ = false;
            }
            else if (type == CCTVTypes.IDIS)
            {
#if _IDIS_
                this.axRASplus_WatSear1 = new CCTVControl.IDISCameraControl();
                this.axRASplus_WatSear1.Owner = this;
                ((System.ComponentModel.ISupportInitialize)(this.axRASplus_WatSear1)).BeginInit();
                this.SuspendLayout();
                // 
                // axRASplus_WatSear1
                // 
                this.axRASplus_WatSear1.Enabled = true;
                this.axRASplus_WatSear1.Location = new System.Drawing.Point(0, 0);
                this.axRASplus_WatSear1.Name = "axRASplus_WatSear1";
                this.axRASplus_WatSear1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axRASplus_WatSear1.OcxState")));
                this.axRASplus_WatSear1.Size = new System.Drawing.Size(Width, Height);
                this.axRASplus_WatSear1.TabIndex = 0;
                this.axRASplus_WatSear1.Dock = DockStyle.Fill;

                Controls.Add(this.axRASplus_WatSear1);
                ((System.ComponentModel.ISupportInitialize)(this.axRASplus_WatSear1)).EndInit();

                axRASplus_WatSear1.initialize();
                axRASplus_WatSear1.setLayout(0);
                axRASplus_WatSear1.setupOSD(false, false, false, false, false, false);
                // 접속 시도시 메시지박스가 나타나지 않도록 한다.
                axRASplus_WatSear1.setHiddenMessageBox(true);
                // Mouse 오른쪽 버튼 Click시 팝업메튜가 나타나지 않도록 한다.
                axRASplus_WatSear1.setProperty(0, 0, 0, 0, "", "");
#endif
            }
            else if (type == CCTVTypes.RTSP)
            {
                vlcControl2 = new Vlc.DotNet.Forms.VlcControl();
                //streamPlayerControl = new WebEye.Controls.WinForms.StreamPlayerControl.StreamPlayerControl();
                m_labelRTSPStatus = new Label();
                m_panelRTSPNoConnect = new Panel();
                m_panelRTSPNoConnect.SuspendLayout();
                ((System.ComponentModel.ISupportInitialize)(this.vlcControl2)).BeginInit();
                this.SuspendLayout();
                // 
                // vlcControl
                // 
                this.vlcControl2.BackColor = System.Drawing.Color.Black;
                this.vlcControl2.Dock = System.Windows.Forms.DockStyle.Fill;
                this.vlcControl2.Location = new System.Drawing.Point(0, 0);
                this.vlcControl2.Name = "vlcControl";
                this.vlcControl2.Size = new System.Drawing.Size(Width, Height);
                this.vlcControl2.Spu = -1;
                this.vlcControl2.TabIndex = 0;
                this.vlcControl2.Text = "vlcControl";
                this.vlcControl2.VlcLibDirectory = GetVlcDirectory();
                this.vlcControl2.VlcMediaplayerOptions = GetVlcOptions();

                this.vlcControl2.Playing += new System.EventHandler<Vlc.DotNet.Core.VlcMediaPlayerPlayingEventArgs>(this.vlcControl_Playing);
                this.vlcControl2.Stopped += new System.EventHandler<Vlc.DotNet.Core.VlcMediaPlayerStoppedEventArgs>(this.vlcControl_Stopped);
                this.vlcControl2.EncounteredError += new System.EventHandler<Vlc.DotNet.Core.VlcMediaPlayerEncounteredErrorEventArgs>(this.vlcControl_EncounteredError);
                this.vlcControl2.MouseClick += new System.Windows.Forms.MouseEventHandler(this.vlcControl_MouseClick);
                this.vlcControl2.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.vlcControl_MouseDoubleClick);
                /*// 
                // streamPlayerControl
                // 
                this.streamPlayerControl.Location = new System.Drawing.Point(0, 0);
                this.streamPlayerControl.Name = "streamPlayerControl";
                this.streamPlayerControl.Size = new System.Drawing.Size(Width, Height);
                this.streamPlayerControl.TabIndex = 0;
                this.streamPlayerControl.Dock = DockStyle.Fill;

                this.streamPlayerControl.StreamStarted += new System.EventHandler(this.RTSPStreamStartedEvent);
                this.streamPlayerControl.StreamStopped += new System.EventHandler(this.RTSPStreamStoppedEvent);
                this.streamPlayerControl.StreamFailed += new System.EventHandler<WebEye.Controls.WinForms.StreamPlayerControl.StreamFailedEventArgs>(this.RTSPStreamFailedEvent);
                this.streamPlayerControl.MouseClick += new System.Windows.Forms.MouseEventHandler(this.streamPlayerControl_MouseClick);
                this.streamPlayerControl.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.streamPlayerControl_MouseDoubleClick);*/
                // 
                // m_labelRTSPStatus
                // 
                this.m_labelRTSPStatus.AutoSize = true;
                this.m_labelRTSPStatus.Font = new System.Drawing.Font("맑은 고딕", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
                this.m_labelRTSPStatus.Name = "m_labelRTSPStatus";
                this.m_labelRTSPStatus.Size = new System.Drawing.Size(229, 32);
                this.m_labelRTSPStatus.TabIndex = 0;
                this.m_labelRTSPStatus.Text = "연결할 수 없습니다.";
                int x = (Width - m_labelRTSPStatus.Size.Width) / 2;
                int y = (Height - m_labelRTSPStatus.Size.Height) / 2;
                this.m_labelRTSPStatus.Location = new System.Drawing.Point(x, y);
                this.m_labelRTSPStatus.Anchor = (AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right));
                // 
                // m_panelRTSPNoConnect
                // 
                this.m_panelRTSPNoConnect.Controls.Add(this.m_labelRTSPStatus);
                this.m_panelRTSPNoConnect.Location = new System.Drawing.Point(0, 0);
                this.m_panelRTSPNoConnect.Name = "m_panelRTSPNoConnect";
                this.m_panelRTSPNoConnect.Size = new System.Drawing.Size(Width, Height);
                this.m_panelRTSPNoConnect.TabIndex = 6;
                this.m_panelRTSPNoConnect.Visible = false;

                this.Controls.Add(this.m_panelRTSPNoConnect);
                this.Controls.Add(this.vlcControl2);
                //this.Controls.Add(this.streamPlayerControl);

                m_enablePTZ = false;

                ((System.ComponentModel.ISupportInitialize)(this.vlcControl2)).EndInit();
            }
            else if (type == CCTVTypes.RTSPONVIF)
            {
                this.vlcControl1 = new Vlc.DotNet.Forms.VlcControl();
                ((System.ComponentModel.ISupportInitialize)(this.vlcControl1)).BeginInit();
                this.SuspendLayout();
                // 
                // vlcControl1
                // 
                this.vlcControl1.BackColor = System.Drawing.Color.Black;
                this.vlcControl1.Location = new System.Drawing.Point(0, 0);
                this.vlcControl1.Name = "vlcControl1";
                this.vlcControl1.Size = this.Size;
                this.vlcControl1.Spu = -1;
                this.vlcControl1.TabIndex = 0;
                this.vlcControl1.Text = "vlcControl1";
                this.vlcControl1.VlcLibDirectory = new DirectoryInfo(Application.StartupPath); 
                this.vlcControl1.VlcMediaplayerOptions = GetVlcOptions();
                // 
                // CCTVCtrl
                // 
                this.Controls.Add(this.vlcControl1);
                this.Name = "CCTVCtrl";
                ((System.ComponentModel.ISupportInitialize)(this.vlcControl1)).EndInit();
                this.ResumeLayout(false);
                
                m_enablePTZ = true;
            }

            else if (mCurTypes == CCTVTypes.IDIS_NVR)
            {
#if _IDIS_NVR_
                idisNVRSet = new CCTVControl.IDIS_NVR.IdisNvrSet(this);
                idisNVRSet.InitializeComponent(this, 0, 0);
                m_enablePTZ = false;
#endif
            }
            else if (mCurTypes == CCTVTypes.ITX_NVR)
            {
#if _ITX_NVR_
                axitxview1 = new CCTVControl.ItxNvrView(this);
                //axitxview1.CreateControl();
                ((System.ComponentModel.ISupportInitialize)(this.axitxview1)).BeginInit();
                this.SuspendLayout();

                this.Controls.Add(this.axitxview1);

                // 
                // axitxview1
                // 
                this.axitxview1.Enabled = true;
                this.axitxview1.Location = new System.Drawing.Point(0, 0);
                this.axitxview1.Name = "axitxview1";
                this.axitxview1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axitxview1.OcxState")));
                this.axitxview1.Size = new System.Drawing.Size(this.Width, this.Height);
                this.axitxview1.Dock = DockStyle.Fill;
                this.axitxview1.TabIndex = 0;

                //this.Controls.Add(this.axitxview1);

                m_enablePTZ = false;
                ((System.ComponentModel.ISupportInitialize)(this.axitxview1)).EndInit();
#endif
            }
            else if (mCurTypes == CCTVTypes.SVMS)
            {
#if _SVMS_
                svmsCamera = new CCTVControl.SVMSCamera(resources, this);
                MakeHookingComponent();
#endif
            }
            else if (mCurTypes == CCTVTypes.Divisys)
            {
#if _Divisys_
                divisysCamera = new CCTVControl.DivisysCamera(resources, this);
                //MakeHookingComponent();
#endif
            }
            else if (mCurTypes == CCTVTypes.WESP)
            {
#if _WESP_
                wespCamera = new CCTVControl.WESPCamera(resources, this);
#endif
            }

            this.ResumeLayout(false);      
        }

        private static string[] GetVlcOptions()
        {
            var options = new[]
            {
                "--rtsp-tcp"
            };

            return options;
        }

        private DirectoryInfo GetVlcDirectory()
        {
            int index = Application.ExecutablePath.LastIndexOf('\\');
            string strFolderPath = index > 0 ? Application.ExecutablePath.Substring(0, index) : Application.ExecutablePath;

            var vlcLibDirectory = new DirectoryInfo(Path.Combine(strFolderPath, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));
            return vlcLibDirectory;
        }

        /*private void MakeHookingComponent()
        {
            mouseHookComponent = new Microsoft.Win32.MouseHookComponent(components);
            mouseHookComponent.MouseDown += new Microsoft.Win32.MouseHookEventHandler(this.mouseHookComponent_MouseDown);
            mouseHookComponent.MouseDoubleClick += new Microsoft.Win32.MouseHookEventHandler(this.mouseHookComponent_MouseDoubleClick);
        }

        private void mouseHookComponent_MouseDoubleClick(object sender, Microsoft.Win32.MouseHookEventArgs e)
        {
#if _SVMS_
            if (svmsCamera != null && e.Control == svmsCamera.Control)
                OnMouseDblClick(sender, e.Button, e.X, e.Y);
#elif _Divisys_
            if (divisysCamera != null && e.Control == divisysCamera.Control)
                OnMouseDblClick(sender, e.Button, e.X, e.Y);
#endif
        }

        private void mouseHookComponent_MouseDown(object sender, Microsoft.Win32.MouseHookEventArgs e)
        {
#if _SVMS_
            if (svmsCamera != null && e.Control == svmsCamera.Control)
                OnMouseDown(sender, e.Button, e.X, e.Y);
#elif _Divisys_
            if (divisysCamera != null && e.Control == divisysCamera.Control)
                OnMouseDown(sender, e.Button, e.X, e.Y);
#endif
        }*/

#if _XpressStrm_
        void axxpressStrm1_GotFocus(object sender, EventArgs e)
        {
            //this.Parent.Focus();
            //this.Focus();
        }



        private DateTime dtClicked = DateTime.Now;

        void axxpressStrm1_MouseCaptureChanged(object sender, EventArgs e)
        {
            DateTime dtNow = DateTime.Now;
            //System.Diagnostics.Trace.WriteLine("dddddd");
            double nMili = (dtNow - dtClicked).TotalMilliseconds;
            if (nMili < 400.0)
            {
                //System.Diagnostics.Trace.WriteLine("Double click " + nMili);
            }
            else
            {
                OnMouseDown(sender, System.Windows.Forms.MouseButtons.Left, 0, 0);
                /*if (m_owner != null)
                    m_owner.OnMouseLButtonClick();*/
            }
            dtClicked = dtNow;

            this.Parent.Focus();
            this.Focus();
        }



        private int m_nXpressStrmConnect = 0;

        void axxpressStrm1_Notify(object sender, AxxpressStrmLib._DxpressStrmEvents_NotifyEvent e)
        {
            if (e.code == 1)
            {
                if (m_nXpressStrmConnect < 7)
                {
                    XpressStrmConnect();                    
                }
                else
                {
                    m_nXpressStrmConnect = -1;
                }
                m_nXpressStrmConnect++;                
            }
            else if (e.code == 2)
            {
                m_nXpressStrmConnect = 0;
                axxpressStrm1.LiveVideo(1);
            }
        }
#endif

#if _XpressStrm_
        private AxxpressStrmLib.AxxpressStrm axxpressStrm1 = null;
#endif
#if _NVS_
        private AxNVSVIEWERCTRLLib.AxNVSViewerCtrl axNVSViewerCtrl1 = null;
#endif
#if _UDP_
        private AxAXVCALib.AxAxVCA axAxVCA1 = null;
#endif
#if _Panasonic_
        private AxIPROPSAPILib.AxipropsapiCtrl axipropsapiCtrl1 = null;
#endif
#if _IPVideo_
        private AxTVSLib.AxTVSLiveControl axTVSLiveControl1 = null;
#endif
#if _TechWin_
        private Axwebviewer_activexplugin_libLib.Axwebviewer_activexplugin_lib axTechWinLib1 = null;
#endif
#if _AXIS_
        private AxAXISMEDIACONTROLLib.AxAxisMediaControl axAxisMediaControl1 = null;
#endif
        private AxWMPLib.AxWindowsMediaPlayer axWindowsMediaPlayer1 = null;
#if _IDIS_
        private CCTVControl.IDISCameraControl axRASplus_WatSear1 = null;
#endif

#if _IDIS_NVR_
        private CCTVControl.IDIS_NVR.IdisNvrSet idisNVRSet = null;
#endif

#if _ITX_NVR_
        private CCTVControl.ItxNvrView axitxview1 = null;
#endif

#if _SVMS_
        private CCTVControl.SVMSCamera svmsCamera = null;
#endif

#if _Divisys_
        private CCTVControl.DivisysCamera divisysCamera = null;
#endif

#if _WESP_
        private CCTVControl.WESPCamera wespCamera = null;
#endif

        //private WebEye.Controls.WinForms.StreamPlayerControl.StreamPlayerControl streamPlayerControl = null;
        private System.Windows.Forms.Label m_labelRTSPStatus = null;
        private System.Windows.Forms.Panel m_panelRTSPNoConnect = null;
        private Vlc.DotNet.Forms.VlcControl vlcControl2 = null;


        private Uri m_urlRTSP = null;

        //private WMPCtrl windowsMediaPlayer = null;
#if _IPVideo_
        private void OnIPVideoSize(int currnetRes)
        {
			if (currnetRes == 0 || currnetRes == 1 || currnetRes == 2 || currnetRes == 3)
			{
                axTVSLiveControl1.Width = 352;
                axTVSLiveControl1.Height = 240;
			}
			else if (currnetRes == 4 || currnetRes == 5 || currnetRes == 6 || currnetRes == 7)			
			{
                axTVSLiveControl1.Width = 352;
                axTVSLiveControl1.Height = 288;
			}		
			else if (currnetRes == 8)			
			{
                axTVSLiveControl1.Width = 320;
                axTVSLiveControl1.Height = 240;
			}
			else if (currnetRes == 9)
			{
                axTVSLiveControl1.Width = 400;
                axTVSLiveControl1.Height = 300;
			}
			else if (currnetRes == 10)			
			{
                axTVSLiveControl1.Width = 512;
                axTVSLiveControl1.Height = 384;
			}
			else if (currnetRes == 11)			
			{
                axTVSLiveControl1.Width = 640;
                axTVSLiveControl1.Height = 480;
			}
			else if (currnetRes == 12)			
			{
                axTVSLiveControl1.Width = 640;
                axTVSLiveControl1.Height = 512;
			}
			else if (currnetRes == 13)			
			{
                axTVSLiveControl1.Width = 720;
                axTVSLiveControl1.Height = 450;
			}
			else if (currnetRes == 14)			
			{
                axTVSLiveControl1.Width = 800;
                axTVSLiveControl1.Height = 450;
			}
			else if (currnetRes == 15)			
			{
                axTVSLiveControl1.Width = 840;
                axTVSLiveControl1.Height = 525;
			}
			else if (currnetRes == 16)			
			{
                axTVSLiveControl1.Width = 640;
                axTVSLiveControl1.Height = 360;
			}
			else if (currnetRes == 17)			
			{
                axTVSLiveControl1.Width = 960;
                axTVSLiveControl1.Height = 540;
			}
			else if (currnetRes == 23)			
			{
                axTVSLiveControl1.Width = 1024;
                axTVSLiveControl1.Height = 768;
			}
			else if (currnetRes == 127 || currnetRes == 27)
			{
                axTVSLiveControl1.Width = 320;
                axTVSLiveControl1.Height = 180;
			}
			else if (currnetRes == 28)
			{
                axTVSLiveControl1.Width = 720;
                axTVSLiveControl1.Height = 270;
			}
        }
#endif

        void panel1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            OnMouseDblClick(sender, e.Button, e.X, e.Y);
            /*if (m_owner != null)
                    m_owner.OnMouseLButtonDoubleClick();*/
        }

        void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            OnMouseDown(sender, System.Windows.Forms.MouseButtons.Left, 0, 0);
             /*if (m_owner != null)
                    m_owner.OnMouseLButtonClick();*/
        }

        public void Pause()
        {
            if (mCurTypes == CCTVTypes.XpressStrm)
            {
#if _XpressStrm_
                if (axxpressStrm1 != null)
                {
                    axxpressStrm1.LiveAudio(0);
                    axxpressStrm1.LiveAudioOutput(0);
                    //MessageBox.Show("Codec : " + axxpressStrm1.VideoCodecString);
                    axxpressStrm1.LiveVideo(0);
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.RTSP)
            {
                if (vlcControl2 != null)
                {
                    vlcControl2.Stop();
                }
                /*if (streamPlayerControl != null)
                {
                    streamPlayerControl.Stop();
                }*/
            }
            else if (mCurTypes == CCTVTypes.SVMS)
            {
#if _SVMS_
                if (svmsCamera != null)
                {
                    svmsCamera.Pause();
                }
#endif
            }
        }

        public void Resume()
        {
            if (mCurTypes == CCTVTypes.XpressStrm)
            {
#if _XpressStrm_
                if (axxpressStrm1 != null)
                {
                    axxpressStrm1.LiveAudio(0);
                    axxpressStrm1.LiveAudioOutput(0);
                    //MessageBox.Show("Codec : " + axxpressStrm1.VideoCodecString);
                    axxpressStrm1.Resume();
                    axxpressStrm1.LiveVideo(1);
                }
#endif
            }
            else if (mCurTypes == CCTVTypes.RTSP)
            {
                if (vlcControl2 != null && m_urlRTSP != null)
                {
                    vlcControl2.Play(m_urlRTSP);
                }
                /*if (streamPlayerControl != null && m_urlRTSP != null)
                {
                    streamPlayerControl.StartPlay(m_urlRTSP);
                }*/
            }
            else if (mCurTypes == CCTVTypes.SVMS)
            {
#if _SVMS_
                if (svmsCamera != null)
                    svmsCamera.Resume();
#endif
            }
        }

        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONDBLCLK = 0x0203;

        //protected override void WndProc(ref Message m)
        //{
        //    if (m.Msg == WM_LBUTTONDBLCLK)
        //    {
        //        if (m_owner != null)
        //            m_owner.OnMouseLButtonDoubleClick();
        //        //MessageBox.Show("LButtonDblClick");
        //    }
        //    else if (m.Msg == WM_LBUTTONDOWN)
        //    {
        //        if (m_owner != null)
        //            m_owner.OnMouseLButtonClick();
        //        System.Diagnostics.Trace.WriteLine("LButtonClick");
        //    }

        //    base.WndProc(ref m);
        //}

       

        public void OnMouseDown(object sender, MouseButtons btn, int x, int y)
        {            
            if (btn == System.Windows.Forms.MouseButtons.Left)
            {
                if (m_owner != null)
                    m_owner.OnMouseLButtonClick();
            }
        }

        public void OnMouseDblClick(object sender, MouseButtons btn, int x, int y)
        {
            if (btn == System.Windows.Forms.MouseButtons.Left)
            {
                if (m_owner != null)
                    m_owner.OnMouseLButtonDoubleClick();
            }
        }
        
        public void OnConnected(object sender)
        {
            m_bIsConnected = true;
        }

        public void OnDisconnected(object sender)
        {
            m_bIsConnected = false;

#if _ITX_NVR_
            if (sender != null && sender is CCTVControl.ItxNvrView)
            {
                this.Controls.Remove(axitxview1);
                axitxview1.Dispose();
                axitxview1 = null;

                this.panel1.BackgroundImage = UnE.Control.Resources.itx_nvr;
                this.panel1.BackgroundImageLayout = ImageLayout.Stretch;
            }
#endif
        }

        public static int GetCCTVType(string szType)
        {
            if (szType == "Axis")
                return (int)CCTVTypes.Axis;
            else if (szType == "NVS")
                return (int)CCTVTypes.NVS;
            else if (szType == "XpressStrm")
                return (int)CCTVTypes.XpressStrm;
            else if (szType == "UDP")
                return (int)CCTVTypes.UDP;
            else if (szType == "Panasonic")
                return (int)CCTVTypes.Panasonic;
            else if (szType == "iPolis")
                return (int)CCTVTypes.TechWin;
            else if (szType == "IPVideo")
                return (int)CCTVTypes.IPVideo;
            else if (szType == "HIK")
                return (int)CCTVTypes.HIK;
            else if (szType == "NVT")
                return (int)CCTVTypes.NVT;
            else if (szType == "MediaPlayer")
                return (int)CCTVTypes.MediaPlayer;
            else if (szType == "IDIS")
                return (int)CCTVTypes.IDIS;
            else if (szType == "RTSP")
                return (int)CCTVTypes.RTSP;           
            else if (szType == "IDIS_NVR")
                return (int)CCTVTypes.IDIS_NVR;
            else if (szType == "ITX_NVR")
                return (int)CCTVTypes.ITX_NVR;
            else if (szType == "RTSPONVIF")
                return (int)CCTVTypes.RTSPONVIF;
            else if (szType == "SVMS")
                return (int)CCTVTypes.SVMS;
            else if (szType == "Divisys")
                return (int)CCTVTypes.Divisys;
            else if (szType == "WESP")
                return (int)CCTVTypes.WESP;

            return 0;
        }

        public static string GetCCTVTypeString(int nType)
        {
            if (nType == (int)CCTVTypes.Axis)
                return "Axis";
            else if (nType == (int)CCTVTypes.NVS)
                return "NVS";
            else if (nType == (int)CCTVTypes.XpressStrm)
                return "XpressStrm";
            else if (nType == (int)CCTVTypes.UDP)
                return "UDP";
            else if (nType == (int)CCTVTypes.Panasonic)
                return "Panasonic";
            else if (nType == (int)CCTVTypes.TechWin)
                return "iPolis";
            else if (nType == (int)CCTVTypes.IPVideo)
                return "IPVideo";
            else if (nType == (int)CCTVTypes.HIK)
                return "HIK";
            else if (nType == (int)CCTVTypes.NVT)
                return "NVT";
            else if (nType == (int)CCTVTypes.MediaPlayer)
                return "MediaPlayer";
            else if (nType == (int)CCTVTypes.IDIS)
                return "IDIS";
            else if (nType == (int)CCTVTypes.RTSP)
                return "RTSP";
            else if (nType == (int)CCTVTypes.IDIS_NVR)
                return "IDIS_NVR";
            else if (nType == (int)CCTVTypes.ITX_NVR)
                return "ITX_NVR";
            else if (nType == (int)CCTVTypes.RTSPONVIF)
                return "RTSPONVIF";
            else if (nType == (int)CCTVTypes.SVMS)
                return "SVMS";
            else if (nType == (int)CCTVTypes.Divisys)
                return "Divisys";
            else if (nType == (int)CCTVTypes.WESP)
                return "WESP";

            return "";
        }

        public static void InitializeApp()
        {
#if _IDIS_NVR_
            GDK.g2main.app_initialize(GDK.G2LANGUAGE.ID.KOREAN);
#endif
        }

        public static void FinalizeApp()
        {
#if _IDIS_NVR_
            GDK.g2main.app_finalize();
#endif
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // CCTVCtrl
            // 
            this.Name = "CCTVCtrl";
            this.ResumeLayout(false);

        }
    }

    public interface ICCTVCtrlOwner
    {
        void OnMouseLButtonDoubleClick();
        void OnMouseLButtonClick();
    }

    public class CCTVMessageFilter : IMessageFilter
    {
        private ICCTVCtrlOwner m_owner = null;

        public ICCTVCtrlOwner Owner
        {
            get { return m_owner; }
            set { m_owner = value; }
        }

        public bool PreFilterMessage(ref Message msg)
        {
            // 왼쪽 마우스 버튼 이벤트를 가로챈다.
            if (513 == msg.Msg)     // WM_LBUTTONDOWN = 513
            {
                //MessageBox.Show("WM_LBUTTONDOWN is : " + msg.Msg.ToString());
                //return true;
            }
            else if (msg.Msg == 0x203)
            {
                if (m_owner != null)
                {
                    m_owner.OnMouseLButtonDoubleClick();

                    //MessageBox.Show("WM_LBUTTONDBLCLICK");
                    return true;
                }
            }
            return false;
        } // end of method PreFilterMessage
    } // end of class MyMessageFilter
}



