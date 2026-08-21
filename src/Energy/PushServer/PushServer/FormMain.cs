using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using Newtonsoft.Json;
using System.IO;
using System.Collections;
using DBUtility;
using System.Threading;

namespace PushServer
{
    public partial class FormMain : Form
    {
        private static string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

        private DBUtility.WebDBManager m_dbMgr = null;
        private System.Windows.Forms.Timer timer = null;

        //ini
        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, int def, StringBuilder retVal, int size, string filePath); 

        /// PUSH 알람 받을 Device Id List
        /// </summary>
        private List<string> deviceIDs = new List<string>();
        private int m_nSiteID = 3;
        // 마지막에 읽은 SensorZoneHistoryID
        private int m_nLastSensorZoneHistoryID = -1;

        private const string WELCOME_TAG = "Welcome";

        private bool m_isAlive = true;

        private Dictionary<int, BuildingGroup> m_dicBuildingGroup = new Dictionary<int, BuildingGroup>();
        private Dictionary<int, Building> m_dicBuildings = new Dictionary<int, Building>();
        private Dictionary<int, ArrayList> m_dicBuildingZones = new Dictionary<int, ArrayList>();
        private Dictionary<int, Zone> m_dicOutdoorZones = new Dictionary<int, Zone>();
        private Dictionary<int, Zone> m_dicZones = new Dictionary<int, Zone>();
        private Dictionary<Zone, List<EquipmentZone>> m_dicZoneEquipZones = new Dictionary<Zone, List<EquipmentZone>>();
        private Dictionary<int, EquipmentZone> m_dicEquipZones = new Dictionary<int, EquipmentZone>();

        public FormMain()
        {
            InitializeComponent();

            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Visible = false;
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.ContextMenuStrip = contextMenuStrip1;

            m_dbMgr = new DBUtility.WebDBManager(m_nSiteID);
            m_dbMgr.DatabaseHost = "127.0.0.1";

            LoadBuildingData();
            LoadZones();
            LoadEquipZones();

            ReadLastSensorZoneHistoryID();

            Thread t = new Thread(new ThreadStart(WelcomeThread));
            t.Start();

            this.timer = new System.Windows.Forms.Timer();
            this.timer.Interval = 1000;
            this.timer.Tick += timer_Tick;
            this.timer.Start();  
        }

        // 새로 App을 설치한 기기에 Welcome 메시지를 보낸다.
        // Background push 메시지를 받을수 있도록 하기 위해서다.
        private void WelcomeThread()
        {
            while (m_isAlive)
            {
                string strSQL = "Select ID, DeviceID from MobileAppUser where Param < 0";
                ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

                if (arrResult != null)
                {
                    int nResultCount = arrResult.Count;

                    for (int i=0;i<nResultCount-1;i+=2)
                    {
                        VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                        string strDeviceID = WebDBManager.GetStringField(arrResult[i + 1]);

                        if (id == null || strDeviceID == null)
                            continue;

                        string strSQL2 = "Update MobileAppUser set Param = NULL where ID = " + id.Data.ToString();
                        m_dbMgr.GetResultData(strSQL2, 0);

                        SendNotification(strDeviceID, WELCOME_TAG, ".");
                    }
                }

                Thread.Sleep(1000);
            }
        }
 
        private void ReadLastSensorZoneHistoryID()
        {
            string strSQL = "Select max(ID), max(SensorZoneHistoryID) from MobileAppLastNotification";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count < 2)
            {
                NewLastSensorZoneHistoryID();
            }
            else
            {
                VariousData<int> sensorZoneHistoryID = WebDBManager.GetIntField(arrResult[1].ToString());

                if (sensorZoneHistoryID == null)
                    NewLastSensorZoneHistoryID();
                else
                    m_nLastSensorZoneHistoryID = sensorZoneHistoryID.Data;
            }
        }

        private void NewLastSensorZoneHistoryID()
        {
            m_nLastSensorZoneHistoryID = 0;
            string strSQL = "Insert into MobileAppLastNotification (ID, Title, Message, SensorZoneHistoryID) values (1, NULL, NULL, 0)";
            m_dbMgr.GetResultData(strSQL, 0);
        }

        private void WriteLastSensorZoneHistoryID(string strTitle, string strMessage, int nSensorZoneHistoryID)
        {
            string strSQL = string.Format("Update MobileAppLastNotification set Title = '{0}', Message = '{1}', SensorZoneHistoryID = {2} where ID = 1",
                strTitle, strMessage, nSensorZoneHistoryID);
            m_dbMgr.GetResultData(strSQL, 0);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            CheckAlarm();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string strDeviceID = "eRYP8lcbEMI:APA91bEBbe6KjFdERCm5fddJyJgzZWA-5DwJRA9SC0HrKRbxdy3fe3MSWpb_Esj-DgMtKMwksGlKTWUQ-ywyfNQ_wHykqydZCi_sQjoK_S9FaJjqO2kSVGJ1bixGViJzhhqSg2KmD5wy";
            SendNotification(strDeviceID, "PTMS 메시지", textBoxMessage.Text);
            //SendNotification("app registration token key string 152 bytes here", textBoxMessage.Text);
        }

        #region PUSH
        private bool SendNotify(string strTitle, string strMessage, string strSOPName, int nSensorZoneHistoryID)
        {
            int nActionStepID = GetActionStepID(strSOPName);

            List<string> deviceInfos = GetDeviceInfoList();

            int nInfoCount = deviceInfos.Count;

            string strBody = string.Format("{0}, {1}", nActionStepID, strMessage);
            WriteLastSensorZoneHistoryID(strTitle, strBody, nSensorZoneHistoryID);

            for (int i = 0; i < nInfoCount - 2;i+=3 )
            {
                string strDeviceID = deviceInfos[i];
                string strNormalID = deviceInfos[i + 1];
                string strEmergencyID = deviceInfos[i + 2];

                //UpdateSensorZoneHistoryID(strDeviceID, strTitle, strBody, nSensorZoneHistoryID);
                SendNotification(strDeviceID, strTitle, strBody);
            }

            return true;
        }

        /*private void UpdateSensorZoneHistoryID(string strDeviceID, string strTitle, string strMessage, int nSensorZoneHistoryID)
        {
            string strSQL = "Select ID from MobileAppLastNotification where DeviceID = '" + strDeviceID + "'";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
            {
                strSQL = string.Format("Insert into MobileAppLastNotification (DeviceID, Title, Message, SensorZoneHistoryID) values ('{0}', '{1}', '{2}', {3})",
                    strDeviceID, strTitle, strMessage, nSensorZoneHistoryID);
            }
            else
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

                if (id == null)
                    strSQL = string.Format("Insert into MobileAppLastNotification (DeviceID, Title, Message, SensorZoneHistoryID) values ('{0}', '{1}', '{2}', {3})",
                        strDeviceID, strTitle, strMessage, nSensorZoneHistoryID);
                else
                    strSQL = string.Format("Update MobileAppLastNotification set Title = '{0}', Message = '{1}', SensorZoneHistoryID = {2} where ID = {3}",
                        strTitle, strMessage, nSensorZoneHistoryID, id.Data);
            }

            m_dbMgr.GetResultData(strSQL, 0);
        }*/

        private int GetActionStepID(string strSOPName)
        {
            string[] tokens = strSOPName.Split('/');

            if (tokens.Count() < 3)
                return -1;

            string strCategoryName = tokens[0].Trim();
            string strSubCategoryName = tokens[1].Trim();
            string strDisasterName = tokens[2].Trim();

            string strSQL = "Select Disaster.ID, Disaster.VersionID from Disaster, subdisastercategory, DisasterCategory ";
            strSQL += "where Disaster.SubDisasterID = SubDisasterCategory.ID and subdisastercategory.DisasterID = DisasterCategory.ID ";
            strSQL += string.Format("and disastercategory.CategoryName = '{0}' and subdisastercategory.SubCategoryName = '{1}' and Disaster.DisasterName = '{2}'", strCategoryName, strSubCategoryName, strDisasterName);

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return -1;

            int nResultCount = arrResult.Count;
            int nDisasterID = -1, nVersionID = -1;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> versionID = WebDBManager.GetIntField(arrResult[i + 1].ToString());

                if (id == null || versionID == null)
                    continue;

                if (versionID.Data > nVersionID)
                {
                    nDisasterID = id.Data;
                    nVersionID = versionID.Data;
                }
            }

            if (nDisasterID < 0)
                return -1;

            strSQL = "Select ID from ActionStep where DisasterID = " + nDisasterID.ToString();
            arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            VariousData<int> actionStepID = WebDBManager.GetIntField(arrResult[0].ToString());

            if (actionStepID == null)
                return -1;

            return actionStepID.Data;
        }

        private List<string> GetDeviceInfoList()
        {
            string strSQL = "Select DeviceID, TemporaryNormalTeamID, TemporaryEmergencyTeamID from MobileAppUser";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            List<string> results = new List<string>();

            if (arrResult == null)
                return results;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-2;i+=3)
            {
                string deviceID = WebDBManager.GetStringField(arrResult[i]);
                VariousData<int> normalID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> emergencyID = WebDBManager.GetIntField(arrResult[i + 2].ToString());

                if (deviceID == null)
                    continue;

                results.Add(deviceID);

                if (normalID == null)
                    results.Add("null");
                else
                    results.Add(normalID.Data.ToString());

                if (emergencyID == null)
                    results.Add("null");
                else
                    results.Add(emergencyID.Data.ToString());
            }

            return results;
        }

        public string SendNotification(string deviceId, string strTitle, string message)
        {
            string SERVER_API_KEY = "AAAAANztO24:APA91bHa9dYBPy-TggReKQXx5Aj75F9KZOIFC_cgUX6v9dfSMAs48MSHXT5dlmYPaQ05HZ-I5ws62neeL13p8g6Hs0yr6oDbbeTsen2pFnw0DLdlbRjO9UNY5h-7mgzuOciPesetQxqE";

            var value = message;
            string resultStr = "";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://fcm.googleapis.com/fcm/send");
            request.Method = "POST";
            request.ContentType = "application/json;charset=utf-8;";
            request.Headers.Add(string.Format("Authorization: key={0}", SERVER_API_KEY));

            var postData =
            new
            {
                /*data = new
                {
                    title = strTitle,
                    body = message,
                    vibrate = true,
                    sound = true
                },*/

                notification = new
                {
                    body = message,
                    title = strTitle,
                    vibrate = true,
                    sound = true
                },

                // FCM allows 1000 connections in parallel.
                to = deviceId
            };

            //Linq to json
            string contentMsg = JsonConvert.SerializeObject(postData);
            System.Diagnostics.Trace.WriteLine("contentMsg = " + contentMsg);

            Byte[] byteArray = Encoding.UTF8.GetBytes(contentMsg);
            request.ContentLength = byteArray.Length;

            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            try
            {
                WebResponse response = request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                resultStr = reader.ReadToEnd();
                System.Diagnostics.Trace.WriteLine("response: " + resultStr);
                reader.Close();
                responseStream.Close();
                response.Close();
            }
            catch (Exception e)
            {
                resultStr = "";
                System.Diagnostics.Trace.WriteLine(e.Message);
            }

            return resultStr;
        } 
        #endregion

        #region 알람 감시
        private int CheckAlarm()
        {
            int FIRE_TYPE = 0, PSM_TYPE = 60;

            string szText = "SELECT srh.SensorHistoryID, srh.ReactionType, srh.Time, srh.Message, szh.SensorID, sz.EquipZoneID, sz.Zone FROM SensorReactionHistory as srh ";
            szText += "INNER JOIN  SensorZoneHistory as szh on srh.SensorHistoryID = szh.ID ";
            szText += "INNER JOIN SensorZone as sz on szh.SensorID = sz.ID ";
            szText += "WHERE SensorHistoryID in (  SELECT srh2.SensorHistoryID FROM SensorReactionHistory as srh2 WHERE srh2.ReactionType in ( 0, 60, 62) ) ";
            szText += " AND SensorHistoryID not in (  SELECT srh3.SensorHistoryID FROM SensorReactionHistory as srh3 WHERE srh3.ReactionType in (21, 23, 33, 50, 70)) ";
            szText += string.Format(" AND szh.SiteID = {0} and ReactionType in ({1}, {2}) and SensorHistoryID > {3}", m_nSiteID, FIRE_TYPE, PSM_TYPE, m_nLastSensorZoneHistoryID);
            szText += " ORDER BY srh.Time, szh.SensorID";

            string strSQL = string.Format(szText, m_nSiteID);

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return 0;

            int nPrevSensorZoneHistoryID = m_nLastSensorZoneHistoryID;

            string strLocation = "", strMaterial = "";
            int nResultCount = arrResult.Count;
            int nNotifyCount = 0;

            for (int i = 0; i < nResultCount - 6; i += 7)
            {
                VariousData<int> historyID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> reactionType = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<DateTime> time = DBUtility.WebDBManager.GetDateTimeField(arrResult[i + 2]);
                string strMessage = DBUtility.WebDBManager.GetStringField(arrResult[i + 3]);
                VariousData<int> sensorZoneID = DBUtility.WebDBManager.GetIntField(arrResult[i + 4].ToString());
                VariousData<int> equipZoneID = DBUtility.WebDBManager.GetIntField(arrResult[i + 5].ToString());
                VariousData<int> zoneID = DBUtility.WebDBManager.GetIntField(arrResult[i + 6].ToString());

                if (historyID == null || reactionType == null || time == null || strMessage == null)
                    continue;

                if (m_nLastSensorZoneHistoryID < historyID.Data)
                    m_nLastSensorZoneHistoryID = historyID.Data;

                if (reactionType.Data == FIRE_TYPE)
                {
                    if (ReadFireLocationInfo(strMessage, out strLocation) == false)
                        continue;

                    string strSOPName = GetLinkedSOPName_Fire(m_dbMgr, zoneID);
                    SendNotify("화재탐지", GetTimeString(time) + ", " + strLocation, strSOPName, historyID.Data);
                    nNotifyCount++;
                }
                else if (reactionType.Data == PSM_TYPE)
                {
                    if (ReadPSMLocationInfo(strMessage, out strLocation, out strMaterial) == false)
                        continue;

                    string strSOPName = GetLinkedSOPName_PSM(m_dbMgr, equipZoneID);
                    SendNotify(strMaterial + "누출", strMaterial + ", " + GetTimeString(time) + ", " + strLocation, strSOPName, historyID.Data);
                    nNotifyCount++;
                }
            }

            return nNotifyCount;
        }

        public string GetLinkedSOPName_PSM(WebDBManager dbMgr, VariousData<int> equipZoneID)
        {
            if (equipZoneID == null)
                return "null";

            EquipmentZone equipZone = GetEquipZone(equipZoneID.Data);

            if (equipZone == null)
                return "null";

            string strSQL = "select ID from PSMTank where EquipZoneID = " + equipZone.ID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return "null";

            int nResultCount = arrResult.Count;
            Dictionary<int, int> dicTankIDs = new Dictionary<int, int>();

            for (int i = 0; i < nResultCount; i++)
            {
                int nTankID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                dicTankIDs[nTankID] = nTankID;
            }

            if (dicTankIDs.Count == 0)
                return "null";

            strSQL = "Select SOPName, LinkedTankID from PSMSensorSOPLink where SiteID = " + m_nSiteID;
            arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return "null";

            nResultCount = arrResult.Count;

            string strAllSOP = "null";

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                string strSOPName = WebDBManager.GetStringField(arrResult[i], "");
                string strLinkedTankID = WebDBManager.GetStringField(arrResult[i + 1]);

                if (strLinkedTankID == null)
                {
                    // 첫번째 SOP를 사용한다.
                    if (strAllSOP == null)
                        strAllSOP = strSOPName;
                    continue;
                }

                List<int> tankIDs = GetIndeces(strLinkedTankID);

                foreach (int nTankID in tankIDs)
                {
                    if (dicTankIDs.ContainsKey(nTankID))
                        return strSOPName;
                }
            }

            return strAllSOP;
        }

        public string GetLinkedSOPName_Fire(WebDBManager dbMgr, VariousData<int> zoneID)
        {
            if (zoneID == null)
                return "null";

            Zone zone = GetZone(zoneID.Data);

            if (zone == null)
                return "null";

            string strSQL = "Select SOPName, LinkedBuildingID, LinkedZoneID from FireSensorSOPLink where SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return "null";

            int nResultCount = arrResult.Count;

            string strAllSOP = "null";

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                string strSOPName = WebDBManager.GetStringField(arrResult[i], "");
                string strLinkedBuildingID = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strLinkedZoneID = WebDBManager.GetStringField(arrResult[i + 2], "");

                if (strLinkedBuildingID == "null" && strLinkedZoneID == "null")
                    strAllSOP = strSOPName;

                if (strLinkedBuildingID != "null" && zone.Building != null)
                {
                    List<Building> buildings = GetBuildings(strLinkedBuildingID);

                    if (buildings != null)
                    {
                        if (buildings.Contains(zone.Building))
                            return strSOPName;
                    }
                }

                if (strLinkedZoneID != "null")
                {
                    List<Zone> zones = GetZones(strLinkedZoneID);

                    if (zones != null)
                    {
                        if (zones.Contains(zone))
                            return strSOPName;
                    }
                }
            }

            return strAllSOP;
        }

        private List<Zone> GetZones(string strZoneIDs)
        {
            List<int> zoneIndeces = GetIndeces(strZoneIDs);

            if (zoneIndeces == null)
                return null;

            List<Zone> zones = new List<Zone>();

            foreach (int nZoneID in zoneIndeces)
            {
                Zone zone = GetZone(nZoneID);

                if (zone != null)
                    zones.Add(zone);
            }

            return zones;
        }

        private List<Building> GetBuildings(string strBuilldingIDs)
        {
            List<int> buildingIndeces = GetIndeces(strBuilldingIDs);

            if (buildingIndeces == null)
                return null;

            List<Building> buildings = new List<Building>();

            foreach (int nBuildingID in buildingIndeces)
            {
                Building building = GetBuilding(nBuildingID);

                if (building != null)
                    buildings.Add(building);
            }

            return buildings;
        }

        private List<int> GetIndeces(string strIDs)
        {
            int nIndex1, nIndex2;
            string[] arrTokens = strIDs.Split(',');

            List<int> indeces = new List<int>();

            foreach (string strToken in arrTokens)
            {
                string[] arrTokens2 = strToken.Split('-');

                if (arrTokens2.Count() == 2)
                {
                    if (!Get2Indeces(arrTokens2[0].Trim(), arrTokens2[1].Trim(), out nIndex1, out nIndex2))
                        return null;

                    if (nIndex1 < nIndex2)
                    {
                        for (int i = nIndex1; i <= nIndex2; i++)
                        {
                            if (!indeces.Contains(i))
                                indeces.Add(i);
                        }
                    }
                    else
                    {
                        for (int i = nIndex2; i <= nIndex1; i++)
                        {
                            if (!indeces.Contains(i))
                                indeces.Add(i);
                        }
                    }
                }
                else if (arrTokens2.Count() == 1)
                {
                    if (!int.TryParse(arrTokens2[0].Trim(), out nIndex1))
                        return null;

                    if (!indeces.Contains(nIndex1))
                        indeces.Add(nIndex1);
                }
            }

            return indeces;
        }

        private bool Get2Indeces(string str1, string str2, out int nIndex1, out int nIndex2)
        {
            nIndex1 = nIndex2 = -1;

            if (!int.TryParse(str1, out nIndex1))
                return false;

            if (!int.TryParse(str2, out nIndex2))
                return false;

            return true;
        }

        private string GetTimeString(VariousData<DateTime> time)
        {
            return string.Format("{0:00}:{1:00}:{2:00}", time.Data.Hour, time.Data.Minute, time.Data.Second);
        }

        private bool ReadFireLocationInfo(string strMessage, out string strLocation)
        {
            strLocation = null;

            int nIndex = strMessage.IndexOf("에서");

            if (nIndex < 0)
                return false;

            strLocation = strMessage.Substring(0, nIndex).Trim();
            strLocation = strLocation.Replace("[테스트]", "");

            if (strLocation.StartsWith("[") && strLocation.EndsWith("]"))
            {
                strLocation = strLocation.Substring(1, strLocation.Length - 2);
            }

            return true;
        }

        private bool ReadPSMLocationInfo(string strMessage, out string strLocation, out string strMaterial)
        {
            strLocation = strMaterial = null;

            if (ReadFireLocationInfo(strMessage, out strLocation) == false)
                return false;

            int nIndex = strMessage.IndexOf("누출");

            if (nIndex < 0)
                return false;

            strMaterial = strMessage.Substring(0, nIndex).Trim();
            nIndex = strMaterial.LastIndexOf(' ');

            if (nIndex < 0)
                return false;

            strMaterial = strMaterial.Substring(nIndex + 1);
            return true;
        }

        #endregion

        private void 종료ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public void LoadBuildingData()
        {
            string szText = "SELECT bd.id, bd.BuildingID,  bd.BuildingCode, bd.BuildingName, bd.BuildingGroupID, bd.MaxFloor, " +
                            " bd.MinFloor, bg.GroupName, bg.DisplayText, bg.TextCenter, bd.BroadCastingText, bd.DisplayText FROM Building as bd " +
                            " INNER JOIN BuildingGroup as bg ON bd.BuildingGroupID = bg.ID AND bg.SiteID = {0}";

            string strSQL = string.Format(szText, m_nSiteID);

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 11; i += 12)
            {
                try
                {
                    int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    string strBuildingID = WebDBManager.GetStringField(arrResult[i + 1], "");
                    string strBuildingCode = WebDBManager.GetStringField(arrResult[i + 2], "");
                    string strBuildingName = WebDBManager.GetStringField(arrResult[i + 3], "");
                    int nBuildingGroupID = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                    int nMaxFloorID = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                    int nMinFloorID = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                    string strBuildingGroupName = WebDBManager.GetStringField(arrResult[i + 7], "");
                    string strBuildingGroupDisplayName = WebDBManager.GetStringField(arrResult[i + 8], "");
                    string strGroupNamePos = WebDBManager.GetStringField(arrResult[i + 9], "");
                    string strBroadcastName = WebDBManager.GetStringField(arrResult[i + 10], "");
                    string strDisplayText = WebDBManager.GetStringField(arrResult[i + 11], "");

                    if (strBroadcastName == null || strBroadcastName.Equals("null"))
                    {
                        strBroadcastName = strBuildingName;
                    }
                    else
                    {
                        int nIdx = strBroadcastName.IndexOf('*');
                        if (nIdx != -1)
                        {
                            strBroadcastName = strBroadcastName.Substring(0, nIdx);
                        }
                    }

                    if (strBuildingGroupDisplayName == null || strBuildingGroupDisplayName.Equals("null"))
                        strBuildingGroupDisplayName = "";

                    Building building = new Building();

                    if (m_dicBuildingGroup.ContainsKey(nBuildingGroupID))
                    {
                        building.BuildingGroup = m_dicBuildingGroup[nBuildingGroupID];
                    }
                    else
                    {
                        BuildingGroup group = new BuildingGroup();
                        group.BuildingGroupName = strBuildingGroupName;
                        group.DisplayName = strBuildingGroupDisplayName;
                        group.GroupID = nBuildingGroupID;
                        //group.BuildingList.Add(building);

                        string[] xy = strGroupNamePos.Split(',');
                        float x = 0.0f, y = 0.0f;
                        if (xy.Length == 2)
                        {
                            float.TryParse(xy[0], out x);
                            float.TryParse(xy[1], out y);
                        }
                        group.TextCenterX = x;
                        group.TextCenterY = y;
                        m_dicBuildingGroup[nBuildingGroupID] = group;
                        building.BuildingGroup = group;
                    }

                    building.ID = nID;
                    building.BuildingName = strBuildingName;
                    building.MaxFloorIndex = nMaxFloorID;
                    building.MinFloorIndex = nMinFloorID;
                    building.BuildingCode = strBuildingCode;
                    building.BuildingID = strBuildingID;
                    building.BroadcastName = strBroadcastName;
                    building.DisplayText = strDisplayText;

                    building.BuildingGroup.BuildingList.Add(building);

                    m_dicBuildings[nID] = building;

                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.StackTrace);
                }
            }
        }

        public void LoadZones()
        {
            string strSQL = "select id, ZoneName, BuildingID, FloorIndex, Boundary, DXFFileName, DXFAccessedTime, _3DFileName, _3DAccessedTime, BroadcastName, AddFloor,DisplayText from Zone WHERE SiteID = " + m_nSiteID.ToString();

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            DateTime dtDefault = new DateTime();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 11; i += 12)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strZoneName = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nBuildingID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nFloorIndex = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                string strBoundary = WebDBManager.GetStringField(arrResult[i + 4], "");
                string strDXFFileName = WebDBManager.GetStringField(arrResult[i + 5], "");
                DateTime dtDXF = WebDBManager.GetDateTimeField(arrResult[i + 6], dtDefault);
                string str3DFileName = WebDBManager.GetStringField(arrResult[i + 7], "");
                DateTime dt3D = WebDBManager.GetDateTimeField(arrResult[i + 8], dtDefault);
                string strBroadcastName = WebDBManager.GetStringField(arrResult[i + 9], "");
                string strAddFloor = WebDBManager.GetStringField(arrResult[i + 10], "0.0");
                string szDisplayText = WebDBManager.GetStringField(arrResult[i + 11], "");
                Zone zone = new Zone();

                zone.ID = nID;
                zone.ZoneName = strZoneName;
                zone.DisplayName = szDisplayText;
                //zone.FloorIndex = nFloorIndex;

                if (strBroadcastName == "null" || strBroadcastName == "")
                    zone.BroadcastName = strZoneName;
                else
                    zone.BroadcastName = strBroadcastName;


                if (m_dicBuildings.ContainsKey(nBuildingID))
                {
                    zone.Building = m_dicBuildings[nBuildingID];
                    zone.Building.FloorList.Add(zone);
                }

                //지하나 .2.5인 층들 
                try
                {
                    //strAddFloor가 비었다면 0.0f
                    if (strAddFloor.Length == 0 || strAddFloor == "null")
                        zone.AddFloor = 0.0f;
                    else
                        zone.AddFloor = float.Parse(strAddFloor);
                }
                catch (Exception)
                {
                    zone.AddFloor = 0.0f;
                }

                zone.Floor.FloorIndex = (nFloorIndex + zone.AddFloor);

                m_dicZones[nID] = zone;
                if (nBuildingID < 0)
                    m_dicOutdoorZones[nID] = zone;

                if (zone.Building != null)
                {
                    if (m_dicBuildingZones.ContainsKey(zone.Building.ID))
                    {
                        ArrayList arrZones = m_dicBuildingZones[zone.Building.ID];
                        arrZones.Add(zone);
                    }
                    else
                    {
                        ArrayList arrZone = new ArrayList();
                        m_dicBuildingZones[zone.Building.ID] = arrZone;
                        arrZone.Add(zone);
                    }
                }
            }
        }

        public void LoadEquipZones()
        {
            string strSQL = "select ID, ZoneName, LinkedZoneIDList, Type, BroadcastName from EquipmentZone where SiteID = " + m_nSiteID.ToString();

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 4; i += 5)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strEquipZoneName = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strLinkedZoneIDList = WebDBManager.GetStringField(arrResult[i + 2], "");
                int nType = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                string strBroadcastName = WebDBManager.GetStringField(arrResult[i + 4], "");

                if (nID < 0)
                    continue;

                ArrayList arrLinkedZones = ParseZoneList(strLinkedZoneIDList);
                if (arrLinkedZones == null)
                    continue;

                if (nType < (int)EquipmentZone.EquipZoneType.SENSOR_TYPE ||
                     nType > (int)EquipmentZone.EquipZoneType.OTHER_TYPE)
                    continue;

                EquipmentZone equipZone = new EquipmentZone();

                equipZone.ID = nID;
                equipZone.EquipZoneName = strEquipZoneName;
                equipZone.BroadcastName = strBroadcastName;
                equipZone.Type = (EquipmentZone.EquipZoneType)nType;

                foreach (Zone zone in arrLinkedZones)
                {
                    equipZone.LinkedZoneList.Add(zone);

                    if (m_dicZoneEquipZones.ContainsKey(zone))
                    {
                        List<EquipmentZone> arrEquipZones = m_dicZoneEquipZones[zone];
                        arrEquipZones.Add(equipZone);
                    }
                    else
                    {
                        List<EquipmentZone> arrEquipZones = new List<EquipmentZone>();
                        m_dicZoneEquipZones[zone] = arrEquipZones;
                        arrEquipZones.Add(equipZone);
                    }
                }

                m_dicEquipZones[nID] = equipZone;
            }
        }

        private ArrayList ParseZoneList(string strZoneIDList)
        {
            strZoneIDList = strZoneIDList.Trim();

            string[] arrZoneIDs = strZoneIDList.Split(',');

            ArrayList arrResult = new ArrayList();
            int nZoneID;

            foreach (string strZoneID in arrZoneIDs)
            {
                if (int.TryParse(strZoneID, out nZoneID))
                {
                    Zone zone = GetZone(nZoneID);

                    if (zone != null && !arrResult.Contains(zone))
                        arrResult.Add(zone);
                }
            }

            return arrResult;
        }

        public Building GetBuilding(int nID)
        {
            if (!m_dicBuildings.ContainsKey(nID))
                return null;
            Building b = m_dicBuildings[nID];
            return b;
        }

        public Zone GetZone(int nZoneID)
        {
            if (m_dicZones.ContainsKey(nZoneID))
                return m_dicZones[nZoneID];

            return null;
        }

        public EquipmentZone GetEquipZone(int nEquipZoneID)
        {
            if (m_dicEquipZones.ContainsKey(nEquipZoneID))
                return m_dicEquipZones[nEquipZoneID];

            return null;
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_isAlive = false;
        }
    }
}
