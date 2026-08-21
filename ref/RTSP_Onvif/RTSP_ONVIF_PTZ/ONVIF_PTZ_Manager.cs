
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Net;
using System.Collections;
using DBUtility;


namespace RTSP_ONVIF
{
    public class ONVIF_PTZ_Manager
    {       
        private static ONVIF_PTZ_Manager m_Instance = null;
        private Object lockingObject = new Object();
        public static ONVIF_PTZ_Manager Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new ONVIF_PTZ_Manager();
                return m_Instance;
            }           
        }
        
        private WebDBManager webDBManager = null;
        public WebDBManager DBManager
        {
            get { return webDBManager; }
        }
        
        private int siteID = 0;
        
        private int ReadSiteID()
        {
            DBUtility.Utility util = new DBUtility.Utility();
            string szSiteID = util.getinivalue("Server Connection Info", "siteid");

            if (int.TryParse(szSiteID, out siteID))
            {
                return siteID;
            }
                        
            return -1;
        }

        private Dictionary<int, Dictionary<int, string>> equipzoneCCTVPresetDic = new Dictionary<int, Dictionary<int, string>>();
        private Dictionary<int, OnvifDevice> cctvDeviceInfoDic = new Dictionary<int, OnvifDevice>();
        private Dictionary<int, ArrayList> allCCTVPresetList = new Dictionary<int, ArrayList>();        //string in ArrayList
        private Dictionary<int, OnvifSessionConnector> connectorsDic = new Dictionary<int, OnvifSessionConnector>();
        
        private ONVIF_PTZ_Manager()
        {
            System.Net.ServicePointManager.Expect100Continue = false;
            ReadSiteID();
            if(siteID > 0)
                webDBManager = new WebDBManager(siteID);
            else 
            {
                MessageBox.Show("SITE ID가 레지스트리에 잘못 구성되었거나 없습니다. 관리자에게 문의하세요");                
            }
            //LoadEquipzonePresets();
            LoadCCTVData();
            LoadAllCCTVSessions(); //위 로드 순서를 지켜야 함            
        }

        /**
         * Equipzone 당 cctv 세팅되어 있는 preset들을 모두 저장. by hypark.
         */
        private void LoadEquipzonePresets()
        {
            string strSQL = "SELECT EquipzoneID, CCTV1, CCTV2, CCTV3, CCTV4, CCTV5, CCTV6, PRESET1, PRESET2, PRESET3, PRESET4, PRESET5, PRESET6 FROM EquipZoneCCTV";

            ArrayList arrResult = webDBManager.GetResultData(strSQL, 0);
            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 12; i += 13)
            {
                try
                {
                    int EquipzoneID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);

                    Dictionary<int, string> presetDic = new Dictionary<int, string>();
                    for (int j = 1; j < 7; j++)
                    {
                        int cctvID = WebDBManager.GetIntField(arrResult[i + j].ToString(), 0);
                        if (cctvID > 0)
                        {
                            string strPreset = WebDBManager.GetStringField(arrResult[i + j + 6], "");
                            if (strPreset != null && !strPreset.Equals("null") && strPreset.Length > 0)
                            {
                                if (!presetDic.ContainsKey(cctvID))
                                    presetDic.Add(cctvID, strPreset);
                            }
                            else
                                continue;
                        }
                        else
                            continue; 
                    }  
                  
                    if(presetDic.Count >0)
                        equipzoneCCTVPresetDic.Add(EquipzoneID, presetDic);
               
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.StackTrace);
                }
            }
        }

        private void LoadCCTVData()
        {
            string szText = "SELECT ID, IPAddr, Type, UserID, Password FROM CCTV WHERE Type = 'RTSP'";
            string strSQL = string.Format(szText, siteID);
            ArrayList arrResult = webDBManager.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 4; i += 5)
            {
                try
                {
                    int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    string ipAddress = WebDBManager.GetStringField(arrResult[i + 1], "");                    
                    string cctvType = WebDBManager.GetStringField(arrResult[i + 2], "");
                    string userID = WebDBManager.GetStringField(arrResult[i + 3], "");
                    string password = WebDBManager.GetStringField(arrResult[i + 4], "");

                    OnvifDevice onvifDevice = new OnvifDevice(ipAddress, userID, password, cctvType);

                    cctvDeviceInfoDic.Add(nID, onvifDevice);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.StackTrace);
                }
            }
        }



        /***
         * EquipzoneCCTV Table에 정의되어 있는 기본 CCTV Preset으로 이동
         * 해당 Table의 CCTV Preset 정보 읽기.
         */
        public void GoCCTVPreset(int equipzoneCCTVID, int cctvID)
        {
            lock (lockingObject)
            {
                if(cctvDeviceInfoDic.ContainsKey(cctvID))
                {
                    OnvifDevice lookingForDevice = cctvDeviceInfoDic[cctvID];
                    if(lookingForDevice != null) 
                    {
                        Dictionary<int, string> presetsDic = equipzoneCCTVPresetDic[equipzoneCCTVID];
                        if(presetsDic != null && presetsDic.Count > 0 ) 
                        {
                            if(presetsDic.ContainsKey(cctvID)) 
                            {
                                string presetName = presetsDic[cctvID];
                                if (connectorsDic.ContainsKey(cctvID))
                                {
                                    OnvifSessionConnector connector = connectorsDic[cctvID];
                                    connector.GoTargetPreset(presetName);
                                }
                            }
                        }
                            
                    }
                }  
            }
        }

        public void GoCCTVPreset(int equipzoneCCTVID)
        {
            lock (lockingObject)
            {              
                Dictionary<int, string> presetsDic = equipzoneCCTVPresetDic[equipzoneCCTVID];
                if (presetsDic != null && presetsDic.Count > 0)
                {
                    foreach (KeyValuePair<int, string> keyValue in presetsDic)
                    {
                        string presetName = keyValue.Value;
                        if (connectorsDic.ContainsKey(keyValue.Key))
                        {
                            OnvifSessionConnector connector = connectorsDic[keyValue.Key];
                            connector.GoTargetPreset(presetName);
                        }
                       
                    }                    
                }              
            }
        }

        /***
         *  CCTV 설정 Preset 변경시 적용할 메소드
         *  두번째 파라미터는 뷰어의 컨트롤 위치 = viewerOrder
         **/
        public bool ChangePreset(int equipzoneID, int viewerOrder, int cctvID, string presetName)
        {
            lock (lockingObject)
            {
                if(!(viewerOrder >=1 && viewerOrder <7)) 
                    return false;
                string strSQL = "Update EquipZoneCCTV set CCTV" + viewerOrder + " = " + cctvID + ", PRESET" + viewerOrder + " = '" + presetName + "' where EquipZoneID = " + equipzoneID;

                if (webDBManager.GetResultData(strSQL, 0) == null)
                    return false;

                return true;
            }
        }
        public void Move(int cctvID, int direction)
        {

            if (connectorsDic.ContainsKey(cctvID))
            {
                OnvifSessionConnector controller = connectorsDic[cctvID];
                controller.RelativeMove(direction);
            }

        }
        public void Stop(int cctvID)
        {
            if (connectorsDic.ContainsKey(cctvID))
            {
                OnvifSessionConnector controller = connectorsDic[cctvID];
             
            }
        }
        /**
         * CCTV의 특정 Preset으로 이동
         * 특정 preset name을 알기 위해 해당 cctv의 목록을 얻으려면 GetPresetList를 호출.
         */
        public int GoPreset(int cctvID, string presetName)
        {
            lock (lockingObject)
            {
                if (connectorsDic.ContainsKey(cctvID))
                {
                    OnvifSessionConnector connector = connectorsDic[cctvID];
                    return connector.GoTargetPreset(presetName);
                }
                return RTSP_ONVIF_RESPONSE.FAILED_GOTO_PRESET;
            }            
        }

        public ArrayList GetPresetList(int cctvID)
        {
            if (allCCTVPresetList.ContainsKey(cctvID))
                return allCCTVPresetList[cctvID];
            return null;
        }

        
        
        public void LoadAllCCTVSessions()
        {
            foreach (KeyValuePair<int, OnvifDevice> pair in cctvDeviceInfoDic)
            {
                OnvifSessionConnector onvifSeesionConnector = new OnvifSessionConnector(pair.Value.IPAddress, pair.Value.UserID, pair.Value.Password);
                onvifSeesionConnector.MakeSession();
                ArrayList list = onvifSeesionConnector.GetPresetList();
                connectorsDic.Add(pair.Key, onvifSeesionConnector);
                allCCTVPresetList.Add(pair.Key, list);
            }
        }


        //public void LoadAllCCTVPresetList()
        //{
        //    foreach (KeyValuePair<int, OnvifDevice> pair in cctvDeviceInfoDic)
        //    {
        //        OnvifConnector onvifConnector = new OnvifConnector();
        //        OnvifDevice device = pair.Value;
        //        ArrayList list = onvifConnector.GetPresetList(device.IPAddress, device.UserID, device.Password);

        //        allCCTVPresetList.Add(pair.Key, list);          //CCTVID, presetlist
        //    }
        //}

        /**
         * CCTV ID, Preset string list
         **/
        public Dictionary<int, ArrayList> GetAllCCTVPresetList()
        {
            return allCCTVPresetList;
        }


    }

    class OnvifDevice
    {
        private string ipAddress = null;
        public string IPAddress
        {
            get { return ipAddress; }
            set { ipAddress = value; }
        }

        private string userID = null;
        public string UserID
        {
            get { return userID; }
            set { userID = value; }
        }
        
        private string password = null;
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        private string type = null;
        public string CCTVType
        {
            get { return type; }
            set { type = value; }
        }
        
        private string profileName = null;
        public string ProfileName
        {
            get { return profileName; }
            set { profileName = value; }
        }

        public OnvifDevice( string ipAddr, string userid, string userpass, string type)
        {
            IPAddress = ipAddr;
            UserID = userid;
            Password = userpass;
            CCTVType = type;
        }
    }
       
}
