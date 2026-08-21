using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

    public partial class CCTVCtrl : UserControl
    {
        private CCTVTypes mCurTypes = CCTVTypes.NotSet;
        public CCTVTypes CCTVType
        {
            get { return mCurTypes; }
        }

        private bool m_bIsConnected = false;
        private bool IsConnected
        {
            get { return m_bIsConnected; }
        }

        private string m_szPassword = "fa0e6a34fd25d96a";
        private string m_szIPAddress = "";

        private UnE.Control.MediaType m_AxisMediaType = UnE.Control.MediaType.mjpeg;

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

       
        //public string Password
        //{
        //    get { return m_szPassword; }
        //    set { m_szPassword = value; }
        //}

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

        public CCTVCtrl(CCTVTypes type)
        {
            mCurTypes = type;
            InitializeComponent();
            ChangeType(type);
        }

        public CCTVCtrl()
        {
            mCurTypes = CCTVTypes.NVS;
            InitializeComponent();
            ChangeType(mCurTypes);
        }

        private void SetDisable()
        {
            //if (m_bIsConnected == false)
            //    return;

            if (mCurTypes == CCTVTypes.XpressStrm)
            {
                try
                {
                    axxpressStrm1.LiveAudioOutput(0);
                    axxpressStrm1.LiveVideo(0);
                    axxpressStrm1.LiveAudio(0);
                    axxpressStrm1.Disconnect();
                    axxpressStrm1.RepositoryDisconnect();
                    axxpressStrm1.Visible = false;
                }
                catch (Exception)
                {
                }
            } 
            else if( mCurTypes == CCTVTypes.Axis)
            {
                try
                {
                    axAxisMediaControl1.Stop();
                    axAxisMediaControl1.Visible = false;
                }
                catch (Exception)
                {
                }
            }
            else if(mCurTypes == CCTVTypes.NVS || mCurTypes == CCTVTypes.NVT)
            {
                try
                {
                    axNVSViewerCtrl1.Stop();
                    axNVSViewerCtrl1.Visible = false;
                }
                catch (Exception)
                {
                }
            }
            else if(mCurTypes == CCTVTypes.UDP)
            {
                try
                {
                    axAxVCA1.Stop();
                    axAxVCA1.Visible = false;
                }
                catch (Exception)
                {
                }
            }
            else if (mCurTypes == CCTVTypes.Panasonic)
            {
                try
                {
                    axipropsapiCtrl1.Disconnect();
                    axipropsapiCtrl1.Visible = false;
                }
                catch (Exception)
                {
                }
            }
            else if (mCurTypes == CCTVTypes.TechWin)
            {

                try
                {
                    axTechWinLib1.Stop();
                    axTechWinLib1.Visible = false;
                }
                catch (Exception)
                {
                }
            }
            else if (mCurTypes == CCTVTypes.IPVideo)
            {
                try
                {
                    axTechWinLib1.Stop();
                    axTechWinLib1.Visible = false;
                }
                catch (Exception)
                {
                }
            }
           
            m_bIsConnected = false;
        }               
        
        public void Disconnect()
        {
            m_bIsConnected = false;
            SetDisable();
        }

        public void ChangeType(CCTVTypes type)
        {
            SetDisable();

            if (type == CCTVTypes.Axis)
            {
                axAxisMediaControl1.StretchToFit = true;
                axAxisMediaControl1.MaintainAspectRatio = false;
                axAxisMediaControl1.ShowStatusBar = false;
                axAxisMediaControl1.BackgroundColor = ColorToInt(Color.Red);
                axAxisMediaControl1.VideoRenderer = (int)AMC_VIDEO_RENDERER.AMC_VIDEO_RENDERER_VMR9;
                axAxisMediaControl1.EnableOverlays = true;
                axAxisMediaControl1.EnableContextMenu = false;
                axAxisMediaControl1.ToolbarConfiguration = "+play,+fullscreen,-settings"; //"-pixcount" to remove pixel counter

                axAxisMediaControl1.Visible = true;
                axAxisMediaControl1.Dock = DockStyle.Fill;
                axAxisMediaControl1.BringToFront();
            }
            else if (type == CCTVTypes.NVS || type == CCTVTypes.NVT)
            {
                axNVSViewerCtrl1.BkColor = ColorToInt(Color.Green);
                axNVSViewerCtrl1.Visible = true;
                axNVSViewerCtrl1.Dock = DockStyle.Fill;
                axNVSViewerCtrl1.BringToFront();
            }
            else if (type == CCTVTypes.XpressStrm)
            {
      
                axxpressStrm1.Visible = true;
                axxpressStrm1.Dock = DockStyle.Fill;
                axxpressStrm1.BringToFront();
            }
            else if (type == CCTVTypes.UDP)
            {
                axAxVCA1.StretchToFit = true;
                axAxVCA1.Visible = true;
                axAxVCA1.Dock = DockStyle.Fill;
                axAxVCA1.BringToFront();
            }
            else if (type == CCTVTypes.Panasonic)
            {              
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
            }
            else if( type == CCTVTypes.TechWin)
            {
                axTechWinLib1.Visible = true;
                axTechWinLib1.Dock = DockStyle.Fill;
                axTechWinLib1.BringToFront();
            }
            else if( type == CCTVTypes.IPVideo)
            {
                axTVSLiveControl1.Visible = true;
                axTVSLiveControl1.Dock = DockStyle.Fill;
                axTVSLiveControl1.BringToFront();
            }
            mCurTypes = type;
        }
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


        

        #endregion//PTZContorl

        private void axxpressStrm1_Notify(object sender, AxxpressStrmLib._DxpressStrmEvents_NotifyEvent e)
        {
            e.code = 2;

            if( e.code == 2)
            {
                axxpressStrm1.LiveVideo(1);
                this.Focus();
            }
            else if(e.code == 1)
            {
                XpressStrmConnect();
            }
            System.Diagnostics.Trace.WriteLine(e.code);
        }

        private void axxpressStrm1_StatusChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("DDDDDDD");
        }

        private void axxpressStrm1_EventSignal(object sender, AxxpressStrmLib._DxpressStrmEvents_EventSignalEvent e)
        {
            System.Diagnostics.Trace.WriteLine(e.code);
            System.Diagnostics.Trace.WriteLine(e.message);
        }


        DateTime dtClicked = DateTime.Now;
        private void axxpressStrm1_MouseCaptureChanged(object sender, EventArgs e)
        {
            DateTime dtNow = DateTime.Now;
            System.Diagnostics.Trace.WriteLine("dddddd");
            double nMili = (dtNow - dtClicked).TotalMilliseconds;
            if (nMili  < 400.0)
            {
                System.Diagnostics.Trace.WriteLine("Double click " + nMili);
            }
            else
            {
                System.Diagnostics.Trace.WriteLine("Click " + nMili);
            }
            dtClicked = dtNow;

            this.Parent.Focus();
            this.Focus();
        }


        void axxpressStrm1_LostFocus(object sender, System.EventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("Lost Focus");
        }

        private void axxpressStrm1_Leave(object sender, EventArgs e)
        {
            int i = 0;
            i++;
        }

        private void axxpressStrm1_Enter(object sender, EventArgs e)
        {
            this.Parent.Focus();
            this.Focus();
        }
    }
}
