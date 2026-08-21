using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace UnE.Control
{
    partial class CCTVCtrl
    {
        // Axis Properties string
        public const string AxisUserName = "UserName";
        public const string AxisPassword = "Password";
        public const string AxisIPAddress = "IPAddress";
        public const string AxisMediaType = "MediaType";

        // NVS Properties string 
        public const string NvsCameraName = "CameraName";
        public const string NvsIPAddress = "IPAddress";
        public const string NvsChannel = "Channel";
        public const string NvsViewNum = "ViewNum";

        private int nIpvideoCh = -1;
       

        private Dictionary<string, string> mProperties = new Dictionary<string, string>();

        public Dictionary<string, string> Properties
        {
            get { return mProperties; }
        }

        public void ClearProperties()
        {
            mProperties.Clear();
        }

        public void AddProperty(string key, string value)
        {
            RemoveProperty(key);
            mProperties.Add(key, value);
        }

        public bool RemoveProperty(string key)
        {
            if (mProperties.ContainsKey(key))
            {
                mProperties.Remove(key);
                return true;
            }
            return false;
        }

        private int GetInt(string value)
        {
            int nResult = 0;
            // ToInt32 can throw FormatException or OverflowException.
            try
            {
                nResult = Convert.ToInt32(value);
            }
            catch (FormatException e)
            {
                Console.WriteLine("Input string is not a sequence of digits.");
            }
            catch (OverflowException e)
            {
                Console.WriteLine("The number cannot fit in an Int32.");
            }
            finally
            {
                if (nResult < Int32.MaxValue)
                {
                    Console.WriteLine("Get Value {0}", nResult);
                }
                else
                {
                    Console.WriteLine("numVal cannot be incremented beyond its current value");
                }
            }
            return nResult;
        }
        
        public bool AxisConnect()
        {
#if _AXIS_
            System.Threading.Thread thread = new System.Threading.Thread(AxisConnectThread);
            thread.Start();
            return true;
#else
            return false;
#endif
        }

        private void AxisConnectThread()
        {
#if _AXIS_
            if (axAxisMediaControl1 == null)
                return;


            axAxisMediaControl1.Stop();

            if (mProperties.ContainsKey("ReversePTZ"))
            {
                m_nReversPTZ = GetInt(mProperties["ReversePTZ"]);
            }
            else
            {
                m_nReversPTZ = 0;
            }

            if (mProperties.ContainsKey("UserName"))
            {
                axAxisMediaControl1.MediaUsername = mProperties["UserName"];
                m_szUserName = mProperties["UserName"];
            }
            else
            {
                axAxisMediaControl1.MediaUsername = "guest";
                m_szUserName = "guest";
            }

            if (mProperties.ContainsKey("Password"))
            {
                axAxisMediaControl1.MediaPassword = mProperties["Password"];
                m_szPassword = mProperties["Password"];
            }
            else
            {
                axAxisMediaControl1.MediaPassword = "";
                m_szPassword = "";
            }

            if (mProperties.ContainsKey("IPAddress"))
            {
                m_szIPAddress = mProperties["IPAddress"];
            }
            else
            {
                m_szIPAddress = "";
            }

            if (mProperties.ContainsKey("MediaType"))
            {
                string szVvalue = mProperties["MediaType"].ToUpper();
                if (szVvalue == "JPEG")
                {
                    m_AxisMediaType = Control.MediaType.mjpeg;
                }
                else if (szVvalue == "MPEG")
                {
                    m_AxisMediaType = Control.MediaType.mpeg4;
                }
                else if (szVvalue == "H264")
                {
                    m_AxisMediaType = Control.MediaType.h264;
                }
            }
            else
            {
                m_AxisMediaType = Control.MediaType.h264;
            }

            if (m_szIPAddress != "")
            {
                axAxisMediaControl1.MediaURL = CompleteURL(m_szIPAddress, (MediaType)m_AxisMediaType);
                axAxisMediaControl1.Play();
            }
#endif
        }

        public bool NVSConnect()
        {
#if _NVS_
            System.Threading.Thread thread = new System.Threading.Thread(NVSConnectThread);
            thread.Start();
            return true;
#else
            return false;
#endif
        }    

        private void NVSConnectThread()
        {
#if _NVS_
            try
            {
                if (axNVSViewerCtrl1 == null)
                    return;

                axNVSViewerCtrl1.Stop();


                if (mProperties.ContainsKey("ReversePTZ"))
                {
                    m_nReversPTZ = GetInt(mProperties["ReversePTZ"]);
                }
                else
                {
                    m_nReversPTZ = 0;
                }
                //axNVSViewerCtrl1.FrameHeight = 340;
                //axNVSViewerCtrl1.FrameWidth = 480;
                axNVSViewerCtrl1.EnableAudio = false;
                axNVSViewerCtrl1.EnableInternalCodec = true;

                if (mProperties.ContainsKey("CameraName"))
                {
                    axNVSViewerCtrl1.CameraName = mProperties["CameraName"];
                }
                else
                {
                    axNVSViewerCtrl1.CameraName = "";
                }

                string szAddress = "";
                if (mProperties.ContainsKey("IPAddress"))
                {
                    szAddress = mProperties["IPAddress"];

                }
                else
                {
                    szAddress = "";

                }

                bool selectChannel = false;
                short nChannel = -1;

                if (mProperties.ContainsKey("Channel"))
                {
                    nChannel = (short)(GetInt(mProperties["Channel"]) + 1);
                    selectChannel = axNVSViewerCtrl1.SelectChannel((short)(nChannel));
                }
                else
                {
                    selectChannel = axNVSViewerCtrl1.SelectChannel(0);
                }

                string szUser = "";
                if (mProperties.ContainsKey("UserName"))
                {
                    szUser = mProperties["UserName"];
                }
                else
                {
                    szUser = "guest";
                }

                string szPass = "";
                if (mProperties.ContainsKey("Password"))
                {
                    szPass = mProperties["Password"];
                }
                else
                {
                    szPass = "guest";
                }
                //if( szUser == "guest")
                //{
                //    szUser = "root";
                //    szPass = "yhtp3000";
                //}
                axNVSViewerCtrl1.PutAccount(szUser, szPass);

                int nPort = -1;
                //if (nChannel == 0)
                //{
                nPort = 1852;
                //} 
                //else if (nChannel == 1)
                //{
                //    nPort = 1853;
                //} 
                //else if (nChannel == 2)
                //{
                //    nPort = 1854;
                //}

                axNVSViewerCtrl1.PutFrameResolution(640, 480);
                if (szAddress != "")
                {
                    axNVSViewerCtrl1.PutAddress(szAddress, nPort, "");

                    if (selectChannel == true)
                    {
                        bool success = axNVSViewerCtrl1.Preview();
                        if (success == true)
                        {
                            if (mProperties.ContainsKey("Stream"))
                            {
                                axNVSViewerCtrl1.ViewNum = (short)GetInt(mProperties["Stream"]);
                            }
                            else
                            {
                                axNVSViewerCtrl1.ViewNum = 0;
                            }
                        }
                    }
                }
            }
            catch(Exception)
            {
            }
#endif
        }

        private void XpressStrmConnectThread()
        {
#if _XpressStrm_
            if (axxpressStrm1 == null || axxpressStrm1.IsDisposed == true)
                return;

            try
            {

                if (mProperties.ContainsKey("ReversePTZ"))
                {
                    m_nReversPTZ = GetInt(mProperties["ReversePTZ"]);
                }
                else
                {
                    m_nReversPTZ = 0;
                }
                if (mProperties.ContainsKey("PlayBackMode"))
                {
                    if (axxpressStrm1 == null || axxpressStrm1.IsDisposed == true)
                        return;
                    axxpressStrm1.PlaybackMode = (short)GetInt(mProperties["PlayBackMode"]);
                }
                else
                {
                    if (axxpressStrm1 == null || axxpressStrm1.IsDisposed == true)
                        return;
                    axxpressStrm1.PlaybackMode = (short)0;
                }

                if (mProperties.ContainsKey("UseRepository"))
                {
                    if (axxpressStrm1 == null || axxpressStrm1.IsDisposed == true)
                        return;
                    axxpressStrm1.UseRepository = (short)GetInt(mProperties["UseRepository"]);
                }
                else
                {
                    if (axxpressStrm1 == null || axxpressStrm1.IsDisposed == true)
                        return;
                    axxpressStrm1.UseRepository = (short)0;
                }

                if (mProperties.ContainsKey("AccessKey"))
                {
                    if (axxpressStrm1 == null || axxpressStrm1.IsDisposed == true)
                        return;
                    axxpressStrm1.AccessKey = mProperties["AccessKey"];
                }
                else
                {
                    if (axxpressStrm1 == null || axxpressStrm1.IsDisposed == true)
                        return;
                    axxpressStrm1.AccessKey = "";
                }

                if (mProperties.ContainsKey("IPAddress"))
                {
                    if (axxpressStrm1 == null || axxpressStrm1.IsDisposed == true)
                        return;
                    axxpressStrm1.IP = mProperties["IPAddress"];
                }
                else
                {
                    if (axxpressStrm1 == null || axxpressStrm1.IsDisposed == true)
                        return;
                    axxpressStrm1.IP = "";
                }
                if (mProperties.ContainsKey("Port"))
                {
                    if (axxpressStrm1 == null || axxpressStrm1.IsDisposed == true)
                        return;
                    axxpressStrm1.Port = (short)GetInt(mProperties["Port"]);
                }


                if (axxpressStrm1 == null || axxpressStrm1.IsDisposed == true)
                    return;
                if (axxpressStrm1.Connect() >= 0)
                {
                    //axxpressStrm1.Dock = System.Windows.Forms.DockStyle.Fill;
                    //axxpressStrm1.Visible = true;
                    //axxpressStrm1.BringToFront();
                    //System.Windows.Forms.MessageBox.Show("Connect : " + axxpressStrm1.AccessKey + "," + axxpressStrm1.IP);
                    return;
                }
            }
            catch(Exception)
            {

            }


            return;
#endif
        }
           
        public bool XpressStrmConnect()
        {
#if _XpressStrm_
            System.Threading.Thread thread = new System.Threading.Thread(XpressStrmConnectThread);
            thread.Start();
            return true;
#else
            return false;
#endif
        }

        

        private int m_nReversPTZ = 0;
        public bool UDPConnect()
        {
#if _UDP_
            int nChannel = 0;
            int nStream = 0;

            if (mProperties.ContainsKey("ReversePTZ"))
            {
                m_nReversPTZ = GetInt(mProperties["ReversePTZ"]);
            }
            else
            {
                m_nReversPTZ = 0;
            }
            if (mProperties.ContainsKey("Channel"))
            {
                nChannel = GetInt(mProperties["Channel"]);
            }
            else
            {
                nChannel = 0;
            }
            if (mProperties.ContainsKey("Stream"))
            {
                nStream = GetInt(mProperties["Stream"]);
            }
            else
            {
                nStream = 0;
            }

            axAxVCA1.MediaStream = string.Format("channel={0},stream={1}", nChannel, nStream);
            if (mProperties.ContainsKey("MediaType"))
            {
                axAxVCA1.MediaType = mProperties["MediaType"];
            }
            else
            {
                axAxVCA1.MediaType = "";
            }

            if (mProperties.ContainsKey("UserName"))
            {
                axAxVCA1.MediaUsername = mProperties["UserName"];
            }
            else
            {
                axAxVCA1.MediaUsername = "guest";
            }

            if (mProperties.ContainsKey("Password"))
            {
                axAxVCA1.MediaPassword = mProperties["Password"];
            }
            else
            {
                axAxVCA1.MediaPassword = "";
            }

            if (mProperties.ContainsKey("IPAddress"))
            {
                axAxVCA1.MediaURL = "http://" + mProperties["IPAddress"];
            }
            else
            {
                axAxVCA1.MediaURL = "";
            }
            if (axAxVCA1.Play() >= 0)
            {
                return true;
            }
#endif
            return false;
        }

        public void Connect()
        {
            this.panel1.BackgroundImage = null;

            InitActiveXControl(this.mCurTypes);
                       
            //ChangeType(mCurTypes);

            //SetDisable();

            //ChangeType(mCurTypes);

            if (mCurTypes == CCTVTypes.Axis)
            {
                m_bIsConnected = AxisConnect();
            }
            else if (mCurTypes == CCTVTypes.NVS || mCurTypes == CCTVTypes.NVT)
            {
                m_bIsConnected = NVSConnect();
            }
            else if (mCurTypes == CCTVTypes.XpressStrm)
            {
                m_bIsConnected = XpressStrmConnect();
            }
            else if (mCurTypes == CCTVTypes.UDP)
            {
                m_bIsConnected = UDPConnect();               
            }
            else if (mCurTypes == CCTVTypes.Panasonic)
            {
                m_bIsConnected = PanasonicConnect();   
            }
            else if (mCurTypes == CCTVTypes.TechWin)
            {
                m_bIsConnected = TechWinConnect();   
            }
            else if (mCurTypes == CCTVTypes.IPVideo)
            {
                m_bIsConnected = IPVideoConnect();   
            }
            else if (mCurTypes == CCTVTypes.MediaPlayer)
            {
                m_bIsConnected = MediaPlayerConnect();
            }
            else if (mCurTypes == CCTVTypes.IDIS)
            {
                m_bIsConnected = IDISConnect();
            }
            else if (mCurTypes == CCTVTypes.RTSP)
            {
                m_bIsConnected = RTSPConnect();
            }
            else if (mCurTypes == CCTVTypes.RTSPONVIF)
            {
                m_bIsConnected = RTSPONVIFConnect();
            }
            else if (mCurTypes == CCTVTypes.IDIS_NVR)
            {
                m_bIsConnected = IdisNVRConnect();
            }
            else if (mCurTypes == CCTVTypes.ITX_NVR)
            {
                m_bIsConnected = ItxNVRConnect();
            }
            else if (mCurTypes == CCTVTypes.SVMS)
            {
                m_bIsConnected = SVMSConnect();
            }
            else if (mCurTypes == CCTVTypes.Divisys)
            {
                m_bIsConnected = DivisysConnect();
            }
            else if (mCurTypes == CCTVTypes.WESP)
            {
                m_bIsConnected = WESPConnect();
            }
        }

        public void Connect(bool needInvoke)
        {
            if (needInvoke)
            {
                this.Invoke((System.Windows.Forms.MethodInvoker)delegate
                {
                    Connect();
                });
            }
            else
                Connect();
        }

        private bool WESPConnect()
        {
#if _WESP_
            if (wespCamera == null)
                return false;

            string strURL = "", strID = "", strPW = "";
            short nPort = 0, nChannel = 0;

            if (mProperties.ContainsKey("IPAddress"))
            {
                strURL = mProperties["IPAddress"];
            }

            if (mProperties.ContainsKey("UserName"))
            {
                strID = mProperties["UserName"];
            }

            if (mProperties.ContainsKey("Password"))
            {
                strPW = mProperties["Password"];
            }

            if (mProperties.ContainsKey("Port"))
            {
                nPort = (short)GetInt(mProperties["Port"]);
            }

            if (mProperties.ContainsKey("Channel"))
            {
                nChannel = (short)GetInt(mProperties["Channel"]);
            }

            wespCamera.Connect(strURL, nPort, nChannel, strID, strPW);
            return true;
#else
            return false;
#endif
        }

        private bool DivisysConnect()
        {
#if _Divisys_
            if (divisysCamera == null)
                return false;

            string strURL = "", strID = "", strPW = "";
            int nPort = 0, nChannel = 0;

            if (mProperties.ContainsKey("IPAddress"))
            {
                strURL = mProperties["IPAddress"];
            }

            if (mProperties.ContainsKey("UserName"))
            {
                strID = mProperties["UserName"];
            }

            if (mProperties.ContainsKey("Password"))
            {
                strPW = mProperties["Password"];
            }

            if (mProperties.ContainsKey("Port"))
            {
                nPort = (int)GetInt(mProperties["Port"]);
            }

            if (mProperties.ContainsKey("Channel"))
            {
                nChannel = (int)GetInt(mProperties["Channel"]);
            }

            return divisysCamera.Connect(strURL, nPort, nChannel, strID, strPW) > 0;
#else
            return false;
#endif
        }

        private bool SVMSConnect()
        {
#if _SVMS_
            if (svmsCamera == null)
                return false;

            string strURL = "";
            int nPort = 0;

            if (mProperties.ContainsKey("URL"))
            {
                strURL = mProperties["URL"];
            }

            if (mProperties.ContainsKey("Port"))
            {
                nPort = (int)GetInt(mProperties["Port"]);
            }

            if (mProperties.ContainsKey("IPAddress"))
            {
                svmsCamera.GUID = mProperties["IPAddress"];
            }

            return svmsCamera.Connect(strURL, nPort) > 0;
#else
            return false;
#endif
        }

        private bool ItxNVRConnect()
        {
#if _ITX_NVR_
            if (axitxview1 == null)
                return false;

            string strIP = "", strUserName = "", strPassword = "", strMacAddr = "";
            short nPort = 0;
            int nChannel = 0;

            if (mProperties.ContainsKey("IPAddress"))
            {
                strIP = mProperties["IPAddress"];
            }

            if (mProperties.ContainsKey("Port"))
            {
                nPort = (short)GetInt(mProperties["Port"]);
            }

            if (mProperties.ContainsKey("Channel"))
            {
                nChannel = GetInt(mProperties["Channel"]);
            }

            if (mProperties.ContainsKey("UserName"))
            {
                strUserName = mProperties["UserName"];
            }

            if (mProperties.ContainsKey("Password"))
            {
                strPassword = mProperties["Password"];
            }

            if (mProperties.ContainsKey("URL"))
            {
                strMacAddr = mProperties["URL"];
            }

            return axitxview1.Connect(strIP, nPort, nChannel, strUserName, strPassword, strMacAddr);
#else
            return false;
#endif
        }

        private bool IdisNVRConnect()
        {
#if _IDIS_NVR_
            if (idisNVRSet == null)
                return false;

            string strIP = "", strUserName = "", strPassword = "";
            short nPort = 0;
            int nChannel = 0;

            if (mProperties.ContainsKey("IPAddress"))
            {
                strIP = mProperties["IPAddress"];
            }

            if (mProperties.ContainsKey("Port"))
            {
                nPort = (short)GetInt(mProperties["Port"]);
            }

            if (mProperties.ContainsKey("Channel"))
            {
                nChannel = GetInt(mProperties["Channel"]);
            }

            if (mProperties.ContainsKey("UserName"))
            {
                strUserName = mProperties["UserName"];
            }

            if (mProperties.ContainsKey("Password"))
            {
                strPassword = mProperties["Password"];
            }

            idisNVRSet.Connect(strIP, (ushort)nPort, nChannel, strUserName, strPassword);
            return true;
#else
            return false;
#endif
        }

        private bool RTSPConnect()
        {
            if (vlcControl2 == null)
                return false;
            //if (streamPlayerControl == null)
            //    return false;

            if (mProperties.ContainsKey("URL"))
            {
                string url = mProperties["URL"];
                string urlLow = url.ToLower();

                if (urlLow.StartsWith("rtsp://"))
                    mProperties["FullURL"] = url;
            }

            if (mProperties.ContainsKey("FullURL"))
            {
                m_urlRTSP = new Uri(mProperties["FullURL"]);
            }
            else
            {
                string strIP = "", strURL = "", strUserName = "", strPassword = "";
                short nPort = 0;

                if (mProperties.ContainsKey("IPAddress"))
                {
                    strIP = mProperties["IPAddress"];
                }

                if (mProperties.ContainsKey("Port"))
                {
                    nPort = (short)GetInt(mProperties["Port"]);
                }

                if (mProperties.ContainsKey("URL"))
                {
                    strURL = mProperties["URL"];
                }

                if (mProperties.ContainsKey("UserName"))
                {
                    strUserName = mProperties["UserName"];
                }

                if (mProperties.ContainsKey("Password"))
                {
                    strPassword = mProperties["Password"];
                }

                if (strIP.Length == 0 || nPort == 0)
                {
                    if (strURL.Length > 0)
                        m_urlRTSP = new Uri(strURL);
                    else
                        return false;
                }
                else
                {
                    string strPath = "rtsp://";

                    if (strUserName.Length > 0)
                        strPath += strUserName + ":" + strPassword + "@";

                    strPath += strIP + ":" + nPort.ToString();

                    if (strURL.Length > 0)
                        strPath += "/" + strURL;

                    m_urlRTSP = new Uri(strPath);
                }
            }

            vlcControl2.Play(m_urlRTSP);
            //streamPlayerControl.StartPlay(m_urlRTSP);
            return true;
        }


        private bool RTSPONVIFConnect()
        {
            if (vlcControl1 == null)
                return false;

            if (mProperties.ContainsKey("URL"))
            {
                string url = mProperties["URL"];
                string urlLow = url.ToLower();

                if (urlLow.StartsWith("rtsp://"))
                    mProperties["FullURL"] = url;
            }

            if (mProperties.ContainsKey("FullURL"))
            {
                m_urlRTSP = new Uri(mProperties["FullURL"]);
            }
            else
            {
                string strIP = "", strURL = "", strUserName = "", strPassword = "";
                short nPort = 0;

                if (mProperties.ContainsKey("IPAddress"))
                {
                    strIP = mProperties["IPAddress"];
                }

                if (mProperties.ContainsKey("Port"))
                {
                    nPort = (short)GetInt(mProperties["Port"]);
                }

                if (mProperties.ContainsKey("URL"))
                {
                    strURL = mProperties["URL"];
                }

                if (mProperties.ContainsKey("UserName"))
                {
                    strUserName = mProperties["UserName"];
                }

                if (mProperties.ContainsKey("Password"))
                {
                    strPassword = mProperties["Password"];
                }

                if (strIP.Length == 0 || nPort == 0)
                    return false;

                string strPath = "rtsp://";

                if (strUserName.Length > 0)
                    strPath += strUserName + ":" + strPassword + "@";

                strPath += strIP + ":" + nPort.ToString();

                if (strURL.Length > 0)
                    strPath += "/" + strURL;

                m_urlRTSP = new Uri(strPath);
            }

            string[] options = { ":network-caching=500" };
            vlcControl1.Video.AspectRatio = this.Width + ":" + this.Height;

            vlcControl1.Play(m_urlRTSP, options);

            return true;
        }
        private bool IDISConnect()
        {
#if _IDIS_
            if (axRASplus_WatSear1 == null)
                return false;

            string strUserName = "admin", strPassword = "";
            string strIP = "";
            short nPort = 0;

            if (mProperties.ContainsKey("UserName"))
            {
                strUserName = mProperties["UserName"];
            }
            
            if (mProperties.ContainsKey("Password"))
            {
                strPassword = mProperties["Password"];
            }
            
            if (mProperties.ContainsKey("IPAddress"))
            {
                strIP = mProperties["IPAddress"];
            }
            
            if (mProperties.ContainsKey("Port"))
            {
                nPort = (short)GetInt(mProperties["Port"]);
            }

            axRASplus_WatSear1.disconnectAll();
            axRASplus_WatSear1.setCameraMap(0, 0, "", strIP, 0, strUserName, strPassword, nPort, false, false, false, "", 0, 0);
            axRASplus_WatSear1.connect();
            return true;
#else
            return false;
#endif
        }

        private bool MediaPlayerConnect()
        {
            if (axWindowsMediaPlayer1 == null)
                return false;

            axWindowsMediaPlayer1.Size = this.Size;

            string strURL = mProperties.ContainsKey("URL") ? mProperties["URL"] : "";

            try
            {
                // 무음 모드로 만든다.
                axWindowsMediaPlayer1.settings.mute = true;
                axWindowsMediaPlayer1.settings.volume = 0;
                // 무한 반복
                axWindowsMediaPlayer1.settings.setMode("loop", true);
                // 재생관련 제어 컨트롤들을 안보이게 한다.
                axWindowsMediaPlayer1.uiMode = "none";
                axWindowsMediaPlayer1.stretchToFit = false;
                // 마우스 오른쪽 버튼을 클릭했을때 팝업메뉴가 안나타나도록 한다.
                axWindowsMediaPlayer1.enableContextMenu = false;
                axWindowsMediaPlayer1.Ctlenabled = false;

                if (strURL.Length > 0)
                {
                    axWindowsMediaPlayer1.BringToFront();

                    axWindowsMediaPlayer1.Ctlcontrols.stop();
                    axWindowsMediaPlayer1.URL = strURL;
                    axWindowsMediaPlayer1.Ctlcontrols.play();
                    return true;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }

            return false;
        }



        private bool PanasonicConnect()
        {
#if _Panasonic_
            int nHttpPort = -1;

            if (mProperties.ContainsKey("ReversePTZ"))
            {
                m_nReversPTZ = GetInt(mProperties["ReversePTZ"]);
            }
            else
            {
                m_nReversPTZ = 0;
            }

            if (mProperties.ContainsKey("HttpPort"))
            {
                nHttpPort = GetInt(mProperties["HttpPort"]);
            }
            else
            {
                nHttpPort = 0;
            }

            if (mProperties.ContainsKey("UserName"))
            {
                axipropsapiCtrl1.UserName = mProperties["UserName"];
            }
            else
            {
                axipropsapiCtrl1.UserName = "guest";
            }

            if (mProperties.ContainsKey("Password"))
            {
                axipropsapiCtrl1.Password = mProperties["Password"];
            }
            else
            {
                axipropsapiCtrl1.Password = "";
            }

            if (mProperties.ContainsKey("IPAddress"))
            {
                axipropsapiCtrl1.IPAddr = mProperties["IPAddress"];
            }
            else
            {
                axipropsapiCtrl1.IPAddr = "";
            }


            int nChannel = 1;
            if (mProperties.ContainsKey("Channel"))
            {
                nChannel = GetInt(mProperties["Channel"]);
            }
            else
            {
                nChannel = 1;
            }
           
            int iRet = -1;
            axipropsapiCtrl1.DeviceType = 2;
            iRet = axipropsapiCtrl1.Open();           
            if (iRet > -1)
            {
                iRet = axipropsapiCtrl1.PlayLive(nChannel, 0);
                if (iRet == 0)
                {
                    return true;
                }
            }
#endif
            return false;
        }
        
        private bool TechWinConnect()
        {
#if _TechWin_
            System.Threading.Thread t = new System.Threading.Thread(TechWinConnectThread);
            t.Start();
            return true;
#else
            return false;
#endif
        }

        private void TechWinConnectThread()
        {
#if _TechWin_
            string szAddress = "";

            if (mProperties.ContainsKey("ReversePTZ"))
            {
                m_nReversPTZ = GetInt(mProperties["ReversePTZ"]);
            }
            else
            {
                m_nReversPTZ = 0;
            }

            if (mProperties.ContainsKey("IPAddress"))
            {
                szAddress = mProperties["IPAddress"];
            }
            else
            {
                szAddress = "";
            }

            short nPort = 554;
            if (mProperties.ContainsKey("Port"))
            {
                nPort = (short)GetInt(mProperties["Port"]);
            }

            int nChannel = 0;
            if (mProperties.ContainsKey("Channel"))
            {
                nChannel = GetInt(mProperties["Channel"]);
            }

            string szUserName = "guest";
            if (mProperties.ContainsKey("UserName"))
            {
                szUserName = mProperties["UserName"];
            }
          
            string szPassword = "";
            if (mProperties.ContainsKey("Password"))
            {
                szPassword = mProperties["Password"];
            }

            //axTechWinLib1.SetResolution(320, 240);
            //axTechWinLib1.SetResolution(320, 240);
            axTechWinLib1.LivePlay(szAddress, (int)nPort, nChannel, szUserName, szPassword);
#endif            
        }
                
        private bool IPVideoConnect()
        {
#if _IPVideo_
            if (axTVSLiveControl1 == null)
                return false;
            if (mProperties.ContainsKey("ReversePTZ"))
            {
                m_nReversPTZ = GetInt(mProperties["ReversePTZ"]);
            }
            else
            {
                m_nReversPTZ = 0;
            }

            string szAddress = "";
            if (mProperties.ContainsKey("IPAddress"))
            {
                szAddress = mProperties["IPAddress"];
            }
            else
            {
                szAddress = "";
            }
            m_szIPAddress = szAddress;
            short nPort = 0;
            if (mProperties.ContainsKey("Port"))
            {
               // nPort = (short)GetInt(mProperties["Port"]);
            }

            int nChannel = 1;
            if (mProperties.ContainsKey("Channel"))
            {
                nChannel = GetInt(mProperties["Channel"]);
            }
            string liveAddres = "";
            int nSub = 0;
            if (nChannel == 0)
            {
                if (nSub == 0)
                {
                    liveAddres = "vsnm://" + szAddress + ":" + nPort + "//video1+audio1";
                }
                else
                {
                    liveAddres = "vsnm://" + szAddress + ":" + nPort + "//video1s+audio1";
                }
            }
            else if (nChannel == 1)
            {
                if (nSub == 0)
                {
                    liveAddres = "vsnm://" + szAddress + ":" + nPort + "//video2";
                }
                else
                {
                    liveAddres = "vsnm://" + szAddress + ":" + nPort + "//video2s";
                }
            }
            else if (nChannel == 2)
            {
                if (nSub == 0)
                {
                    liveAddres = "vsnm://" + szAddress + ":" + nPort + "//video3";
                }
                else
                {
                    liveAddres = "vsnm://" + szAddress + ":" + nPort + "//video3s";
                }
            }
            else if (nChannel == 3)
            {
                if (nSub == 0)
                {
                    liveAddres = "vsnm://" + szAddress + ":" + nPort + "//video4";
                }
                else
                {
                    liveAddres = "vsnm://" + szAddress + ":" + nPort + "//video4s";
                }
            }

            string szUserName = "guest";
            if (mProperties.ContainsKey("UserName"))
            {
                szUserName = mProperties["UserName"];
            }
            m_szUserName = szUserName;
            string szPassword = "";
            if (mProperties.ContainsKey("Password"))
            {
                szPassword = mProperties["Password"];
            }
            m_szPassword = szPassword;
            axTVSLiveControl1.SetLocalConfig(3,  2);
            axTVSLiveControl1.SetLocalConfig(9, -1); // Autoplay on
            axTVSLiveControl1.SetLocalConfig(42, 0); // SetSnapshotAspectRatio

            //axTVSLiveControl1.SetDecodingOption(2, 0);
            nIpvideoCh = axTVSLiveControl1.Connect(liveAddres, szUserName, szPassword);
            if( nIpvideoCh > 0)
            {               
                axTVSLiveControl1.SetAutoReconnect(nIpvideoCh, false);

                //System.IO.StreamWriter writer = new System.IO.StreamWriter("c:\\temp\\dddd.txt");
                //for(int i = 0; i < 100 ; i++)
                //{
                //    writer.Write(i + ":");
                //    writer.WriteLine(axTVSLiveControl1.GetLocalConfig(i));
                //}
                //writer.Close();
                return true;
            }
#endif
            return false;
        }

      
        
    }
}
