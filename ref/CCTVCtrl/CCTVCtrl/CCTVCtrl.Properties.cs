using System;
using System.Collections.Generic;
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
        
        private bool AxisConnect()
        {
            if (mProperties.ContainsKey("UserName"))
            {
                axAxisMediaControl1.MediaUsername = mProperties["UserName"];
            }
            else
            {
                axAxisMediaControl1.MediaUsername = "guest";
            }

            if (mProperties.ContainsKey("Password"))
            {
                axAxisMediaControl1.MediaPassword = mProperties["Password"];
            }
            else
            {
                axAxisMediaControl1.MediaPassword = "";
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
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool NVSConnect()
        {
            if (mProperties.ContainsKey("CameraName"))
            {
                axNVSViewerCtrl1.CameraName = mProperties["CameraName"];
            }
            else
            {
                axNVSViewerCtrl1.CameraName = "";
            }

            if (mProperties.ContainsKey("IPAddress"))
            {
                axNVSViewerCtrl1.IPAddress = mProperties["IPAddress"];
            }
            else
            {
                axNVSViewerCtrl1.IPAddress = "";
            }

            bool selectChannel = false;
            if (mProperties.ContainsKey("Channel"))
            {
                selectChannel = axNVSViewerCtrl1.SelectChannel((short)GetInt(mProperties["Channel"]));
            }
            else
            {
                selectChannel = axNVSViewerCtrl1.SelectChannel(0);
            }

            if (selectChannel == true)
            {
                bool success = axNVSViewerCtrl1.Preview();
                if (success == true)
                {
                    if (mProperties.ContainsKey("ViewNum"))
                    {
                        axNVSViewerCtrl1.ViewNum = (short)GetInt(mProperties["ViewNum"]);
                    }
                    else
                    {
                        axNVSViewerCtrl1.ViewNum = 0;
                    }
                    return true;
                }
                
            }
            return false;
        }

        public bool XpressStrmConnect()
        {
            if (mProperties.ContainsKey("PlayBackMode"))
            {
                axxpressStrm1.PlaybackMode = (short)GetInt(mProperties["PlayBackMode"]);
            }
            else
            {
                axxpressStrm1.PlaybackMode = (short)0;
            }

            if (mProperties.ContainsKey("UseRepository"))
            {
                axxpressStrm1.UseRepository = (short)GetInt(mProperties["UseRepository"]);
            }
            else
            {
                axxpressStrm1.UseRepository = (short)0;
            }

            if (mProperties.ContainsKey("UserName"))
            {
                axxpressStrm1.AccessKey = mProperties["UserName"];
            }
            else
            {
                axxpressStrm1.AccessKey = "";
            }

            if (mProperties.ContainsKey("IPAddress"))
            {
                axxpressStrm1.IP = mProperties["IPAddress"];
            }
            else
            {
                axxpressStrm1.IP = "";
            }
            if (mProperties.ContainsKey("Password"))
            {
                axxpressStrm1.Port = (short)GetInt(mProperties["Password"]);
            }
            if (axxpressStrm1.Connect() >= 0)
            {
                return true;
            }
            return false;
        }

        public bool UDPConnect()
        {
            int nChannel = 0;
            int nStream = 0;


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
            return false;
        }

        public void Connect()
        {
            SetDisable();

            ChangeType(mCurTypes);

            if (mCurTypes == CCTVTypes.Axis)
            {
                m_bIsConnected = AxisConnect();
            }
            else if (mCurTypes == CCTVTypes.NVS)
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
        } 

        private bool PanasonicConnect()
        {
            int nHttpPort = -1;
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
            return false;
        }
        
        private bool TechWinConnect()
        {
            string szAddress = "";
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

            axTechWinLib1.LivePlay(szAddress, (int)nPort, nChannel, szUserName, szPassword);
            return false;
        }
                
        private bool IPVideoConnect()
        {
            string szAddress = "";
            if (mProperties.ContainsKey("IPAddress"))
            {
                szAddress = mProperties["IPAddress"];
            }
            else
            {
                szAddress = "";
            }

            short nPort = 0;
            if (mProperties.ContainsKey("Port"))
            {
                nPort = (short)GetInt(mProperties["Port"]);
            }

            int nChannel = 1;
            if (mProperties.ContainsKey("Channel"))
            {
                nChannel = GetInt(mProperties["Channel"]);
            }
            string liveAddres = "";
            int nSub = 0;
            if(nChannel == 0)
            {
                if(nSub == 0)
                {
                    liveAddres = "vsnm://" + szAddress + ":" + nPort + "//video1+audio1";
	            }
	            else{
	                liveAddres = "vsnm://" + szAddress + ":" + nPort + "//video1s+audio1";
                }
            }
            else if(nChannel == 1){
                if(nSub == 0){
	                liveAddres = "vsnm://" + szAddress + ":" + nPort + "//video2";
	            }
	            else{
	                liveAddres = "vsnm://" + szAddress + ":" + nPort + "//video2s";
                }
            }
            else if(nChannel == 2){
                if(nSub == 0){
	                liveAddres = "vsnm://" + szAddress + ":" + nPort + "//video3";
	            }
	            else{
	                liveAddres = "vsnm://" + szAddress + ":" + nPort + "//video3s";
                }
            }
            else if(nChannel == 3)
            {
                if(nSub == 0){
	                liveAddres = "vsnm://" + szAddress + ":" + nPort + "//video4";
	            }
	            else{
	                liveAddres = "vsnm://" + szAddress + ":" + nPort + "//video4s";
                }
            }
            
            //if(nChannel == 0)
            //{
            //    liveAddres = "vsnm://" + szAddress + ":" + nPort + "//video1+audio1";
            //}
            //else
            //{
            //    liveAddres = "vsnm://" + szAddress + ":" + nPort + "//video1s+audio1";
            //}

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
              
            axTVSLiveControl1.SetLocalConfig(3, 2);
            axTVSLiveControl1.SetLocalConfig(9, -1); // Autoplay on
            axTVSLiveControl1.SetLocalConfig(42, 0); // SetSnapshotAspectRatio

            int g_lChID = axTVSLiveControl1.Connect(liveAddres, szUserName, szPassword);

            axTVSLiveControl1.SetAutoReconnect(g_lChID, false);
            
            //axTVSLiveControl1.SetMute(g_lChID, false);
            //axTVSLiveControl1.SetPTZControlSpeed(g_lChID, 0, -1);
            //axTVSLiveControl1.SetPTZControlSpeed(g_lChID, 1, -1);
            //axTVSLiveControl1.SetPTZControlSpeed(g_lChID, 2, -1);
            return false;
        }       
    }
}
