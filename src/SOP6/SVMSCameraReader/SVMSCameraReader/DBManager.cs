using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using S1SVMSSDKv2.Model.Device;
using System.Collections;
using System.IO;

namespace SVMSCameraReader
{
    class DBManager : ISVMSClient
    {
        private const string CameraType = "SVMS";
        private const string ConfigFile = "svms.ini";

        private SVMSManager m_svmsMgr = null;
        private WebDBManager m_dbMgr = null;
        // Key : CCTV GUID
        private Dictionary<string, CCTV> m_dicCCTVs = new Dictionary<string, CCTV>();
        // SVMS 시스템에서 제거된 CCTV들
        // Key :CCTV ID
        private Dictionary<int, CCTV> m_dicRemoveCCTVs = new Dictionary<int, CCTV>();
        private int m_nUDPPort = 0;
        private bool m_closeApp = false;

        public bool CloseApp
        {
            get { return m_closeApp; }
        }

        public DBManager(int nSiteID, int nUDPPort)
        {
            m_svmsMgr = new SVMSManager(this);
            m_dbMgr = new WebDBManager(nSiteID);
            m_nUDPPort = nUDPPort;
        }

        public bool UpdateCCTVList()
        {
            if (ReadCCTVList() == false)
            {
                if (m_nUDPPort > 0)
                    Network.UDPClient.SendMessage(Network.Header.DBConnectionError, null, m_nUDPPort);
                return false;
            }

            string strSVMSServerIP, strUserID, strUserPW;
            int nPort;

            if (GetSVMSConnectionInfo(out strSVMSServerIP, out nPort, out strUserID, out strUserPW) == false)
            {
                if (m_nUDPPort > 0)
                    Network.UDPClient.SendMessage(Network.Header.NoSVMSInfo, null, m_nUDPPort);
                return false;
            }

            m_svmsMgr.Connect(strSVMSServerIP, nPort, strUserID, strUserPW);
            return true;
        }

        private bool GetSVMSConnectionInfo(out string strSVMSServerIP, out int nSVMSServerPort, out string strSVMSUserID, out string strSVMSUserPW)
        {
            Utility util = new Utility();
            string strSection = "SVMS Connection Info";

            strSVMSServerIP = util.getinivalue(strSection, "SVMSServer_IP");
            string strPort = util.getinivalue(strSection, "SVMSServer_Port");
            strSVMSUserID = util.getinivalue(strSection, "SVMSServer_User");
            strSVMSUserPW = util.getinivalue(strSection, "SVMSServer_Passwd");

            if (int.TryParse(strPort, out nSVMSServerPort) == false)
                return false;

            return strSVMSServerIP.Length > 0 && strSVMSUserID.Length > 0 && strSVMSUserPW.Length > 0;
        }

        private bool ReadCCTVList()
        {
            string strSQL = "Select ID, CameraName, IPAddr, Port, Description, URL from CCTV where Type = '" + CameraType + "'";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-5;i+=6)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strCameraName = WebDBManager.GetStringField(arrResult[i + 1]);
                string strIP = WebDBManager.GetStringField(arrResult[i + 2]);
                VariousData<int> port = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                string strGUID = WebDBManager.GetStringField(arrResult[i + 4]);
                string strURL = WebDBManager.GetStringField(arrResult[i + 5]);

                if (id == null || strIP == null || port == null || strGUID == null)
                    continue;

                CCTV cctv = new CCTV();

                cctv.ID = id.Data;
                cctv.Name = strCameraName;
                cctv.IP = strIP;
                cctv.Port = port.Data;
                cctv.GUID = strGUID;
                cctv.URL = strURL;

                m_dicCCTVs[cctv.GUID] = cctv;
                m_dicRemoveCCTVs[cctv.ID] = cctv;
            }

            return true;
        }

        private bool CheckCamera(DeviceCamera camera)
        {
            CCTV cctv;

            if (m_dicCCTVs.TryGetValue(camera.Guid, out cctv))
            {
                // 확인된 CCTV는 SVMS에 여전히 남아있으므로 제거목록에서 제외한다.
                m_dicRemoveCCTVs.Remove(cctv.ID);

                if (CheckUpdate(camera, cctv))
                    return true;
            }
            else
            {
                cctv = InsertCCTV(camera);

                if (cctv != null)
                {
                    m_dicCCTVs[cctv.GUID] = cctv;
                    return true;
                }
            }

            return false;
        }

        private CCTV InsertCCTV(DeviceCamera camera)
        {
            WebDBManager dbMgr = m_dbMgr.Clone();

            CCTV cctv = new CCTV();

            cctv.URL = GetCameraConnection(camera);

            if (cctv.URL.Length == 0)
                return null;

            cctv.GUID = camera.Guid;
            cctv.IP = camera.CameraIPAddress;
            cctv.Port = camera.CameraRTSPPort;
            cctv.Name = camera.CameraName;

            if (dbMgr.BeginBatch() == false)
                return null;

            string strSQL = "Select max(ID) from CCTV";
            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
            {
                dbMgr.BatchRollback();
                return null;
            }

            int nID = 1;

            if (arrResult.Count > 0)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

                if (id != null)
                    nID = id.Data + 1;
            }

            cctv.ID = nID;

            // Zone이 아직 지정되지 않았는데 Zone과 IsIndoor는 Not Null 속성이기 때문에 그냥 1로 넣는다.
            // 대신 ReversePTZ를 -1로 두어 Zone이 아직 제대로 지정되지 않았음을 표시한다.
            // ReversePTZ가 NULL이 거나 0 이상의 값이면 Zone이 제대로 지정된 것이다.
            string strFormat = "Insert into CCTV (ID, CameraName, IPAddr, Port, PositionName, X, Y, Z, ZoneID, IsIndoor, LOD, Description, HTTPPort, Type, Stream, Channel, UserID, Password, URL, ReversePTZ, BigURL, SmallURL) ";
            strFormat += "values ({0}, '{1}', '{2}', {3}, NULL, 0, 0, 0, 1, 1, 1, '{4}', NULL, '{5}', NULL, NULL, NULL, NULL, '{6}', -1, NULL, NULL)";
            strSQL = string.Format(strFormat, cctv.ID, cctv.Name, cctv.IP, cctv.Port, cctv.GUID, CameraType, cctv.URL);

            if (dbMgr.GetBatchData(strSQL) == null)
            {
                dbMgr.BatchRollback();
                return null;
            }

            if (dbMgr.BatchCommit() == false)
            {
                dbMgr.BatchRollback();
                return null;
            }

            return cctv;
        }

        private bool RemoveCCTV()
        {
            if (m_dicRemoveCCTVs.Count == 0)
                return false;

            string strIDs = "";

            foreach (KeyValuePair<int, CCTV> pair in m_dicRemoveCCTVs)
            {
                if (strIDs.Length == 0)
                    strIDs = pair.Key.ToString();
                else
                    strIDs += ", " + pair.Key.ToString();
            }

            string strSQL = string.Format("Delete from CCTV where ID in ({0})", strIDs);
            return m_dbMgr.GetResultData(strSQL) != null;
        }

        // Return 값 : DB 데이터가 바뀌었는가?
        private bool CheckUpdate(DeviceCamera camera, CCTV cctv)
        {
            string strConnection = GetCameraConnection(camera);

            if (strConnection.Length == 0)
                return false;

            bool needUpdate = false;

            if (cctv.IP != camera.CameraIPAddress)
            {
                cctv.IP = camera.CameraIPAddress;
                needUpdate = true;
            }

            if (cctv.Name != camera.CameraName)
            {
                cctv.Name = camera.CameraName;
                needUpdate = true;
            }

            if (cctv.Port != camera.CameraRTSPPort)
            {
                cctv.Port = camera.CameraRTSPPort;
                needUpdate = true;
            }

            if (cctv.URL != strConnection)
            {
                cctv.URL = strConnection;
                needUpdate = true;
            }

            if (needUpdate)
            {
                string strSQL = string.Format("Update CCTV set IPAddr = '{0}', CameraName = '{1}', Port = {2}, URL = '{3}' where ID = {4}", cctv.IP, cctv.Name, cctv.Port, cctv.URL, cctv.ID);
                return m_dbMgr.GetResultData(strSQL) != null;
            }

            return false;
        }

        private string GetCameraConnection(DeviceCamera camera)
        {
            string strCCTVIP = camera.CameraIPAddress;
            string strRTSP = "rtsp://";
            string strLower = camera.ConnectURL.ToLower();

            int nIndex1 = strLower.IndexOf(strRTSP);
            int nIndex2 = strLower.IndexOf(strCCTVIP);
            string strConnection = camera.ConnectURL;

            if (nIndex1 >= 0 && nIndex2 > nIndex1)
            {
                string strServer = strConnection.Substring(strRTSP.Length, nIndex2 - strRTSP.Length);

                if (strServer.Contains(':') == false)
                {
                    if (strServer.EndsWith("/"))
                        strConnection = strRTSP + strServer.Substring(0, strServer.Length - 1) + ":" + camera.CameraRTSPPort.ToString() + "/" + strCCTVIP;
                    else
                        strConnection = strRTSP + strServer + ":" + camera.CameraRTSPPort.ToString() + "/" + strCCTVIP;
                }
            }
            else
                return "";

            return strConnection;
        }

        #region ISVMSClient
        public void OnConnection(bool isSuccess)
        {
            if (m_nUDPPort > 0)
            {
                ArrayList arrDatas = new ArrayList();
                arrDatas.Add(isSuccess);
                Network.UDPClient.SendMessage(Network.Header.ConnectionComplete, arrDatas, m_nUDPPort);
            }

            if (isSuccess == false)
                m_closeApp = true;
        }

        public void OnClientType(string strClientGUID)
        {
        }

        public void OnLogin(bool isSuccess)
        {
            if (isSuccess)
            {
                m_svmsMgr.RequestCameraList();
            }

            if (m_nUDPPort > 0)
            {
                ArrayList arrDatas = new ArrayList();
                arrDatas.Add(isSuccess);
                Network.UDPClient.SendMessage(Network.Header.LoginComplete, arrDatas, m_nUDPPort);
            }

            if (isSuccess == false)
                m_closeApp = true;
        }

        public void OnDisconnect()
        {
            m_closeApp = true;
        }

        public void OnReconnect()
        {
        }

        public void OnCameraList(bool isSuccess, bool isFinished, List<DeviceCamera> deviceCameras)
        {
            if (isSuccess == true)
            {
                if (isFinished != true)
                {
                    bool updateDB = false;

                    foreach (var deviceCameraItem in deviceCameras)
                    {
                        string deviceCameraGUID = deviceCameraItem.CameraGUID;
                        if (deviceCameraGUID != null)
                        {
                            if (CheckCamera(deviceCameraItem))
                                updateDB = true;
                        }
                    }

                    if (m_nUDPPort > 0)
                    {
                        // SVMS 시스템에서 제거된 CCTV들을 DB에서 삭제한다.
                        if (RemoveCCTV())
                            updateDB = true;

                        ArrayList arrDatas = new ArrayList();
                        arrDatas.Add(updateDB);
                        arrDatas.Add(m_dicCCTVs.Count - m_dicRemoveCCTVs.Count);
                        Network.UDPClient.SendMessage(Network.Header.FinishUpdate, arrDatas, m_nUDPPort);
                    }
                }
                else
                    m_closeApp = true;
            }
            else
                m_closeApp = true;
        }

        public void OnAddCamera(DeviceCamera deviceCamera)
        {
        }

        public void OnModifyCamera(DeviceCamera deviceCamera)
        {
        }

        public void OnRemoveCamera(DeviceCamera deviceCamera)
        {
        }
        #endregion
    }

    class CCTV
    {
        private int m_nID = -1;
        private string m_strCameraName = null;
        private string m_strGUID = null;
        private string m_strIP = "";
        private int m_nPort = 0;
        private string m_strURL = null;
        private string m_strBigURL = null;
        private string m_strSmallURL = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string Name
        {
            get { return m_strCameraName; }
            set { m_strCameraName = value; }
        }

        public string GUID
        {
            get { return m_strGUID; }
            set { m_strGUID = value; }
        }

        public string IP
        {
            get { return m_strIP; }
            set { m_strIP = value; }
        }

        public int Port
        {
            get { return m_nPort; }
            set { m_nPort = value; }
        }

        public string URL
        {
            get { return m_strURL; }
            set { m_strURL = value; }
        }

        public string BigURL
        {
            get { return m_strBigURL; }
            set { m_strBigURL = value; }
        }

        public string SmallURL
        {
            get { return m_strSmallURL; }
            set { m_strSmallURL = value; }
        }
    }
}
