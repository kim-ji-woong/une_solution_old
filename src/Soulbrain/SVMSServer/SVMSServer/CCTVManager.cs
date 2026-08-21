using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Diagnostics;
using SDMS.DAL;
using SDMS.Model.CCTV;
using SDMS.Model.Sensor;
using dnsData.Sensor;
using dnsCommunicateSopServer;

namespace SVMSServer
{
    public class CCTVManager
    {
        private DataManager m_dataManager = null;
        private Common.DAL.DataManager m_commonDataManager = null;
        // Key : 고유키
        private Dictionary<string, CCTV> m_dicCCTVs = new Dictionary<string, CCTV>();
        // Key : CCTV ID
        private Dictionary<int, CCTV> m_dicCCTVIDs = new Dictionary<int, CCTV>();
        // Key : EquipZone ID
        private Dictionary<int, List<CCTV>> m_dicEquipZoneCCTVs = new Dictionary<int, List<CCTV>>();
        // Value : 하나의 CCTV가 연결되어 있는 EquipZone ID들
        private Dictionary<CCTV, List<int>> m_dicCCTVEquipZoneIDs = new Dictionary<CCTV, List<int>>();
        private string m_strAlarmURL = "";

        private const string Safety_I_Type = "Safety-I";
        private const string RTSP_Type = "RTSP";

        public CCTVManager(DataManager dataManager, Common.DAL.DataManager commonDataManager)
        {
            m_dataManager = dataManager;
            m_commonDataManager = commonDataManager;
            ReadCCTVs();

            m_strAlarmURL = ConfigurationManager.AppSettings.Get("Alarm_Security_URL");
        }

        private void ReadCCTVs()
        {
            string strErrorMessage;
            List<CCTV> cctvs = m_dataManager.GetSelectManager().SelectCCTVs(null, null, out strErrorMessage);

            if (cctvs != null)
            {
                m_dicCCTVIDs.Clear();
                m_dicCCTVs.Clear();

                foreach (CCTV cctv in cctvs)
                {
                    m_dicCCTVs[cctv.UniqueKey] = cctv;
                    m_dicCCTVIDs[cctv.ID] = cctv;
                }
            }

            List<EquipZoneCCTV> equipZoneCCTVs = m_dataManager.GetSelectManager().SelectEquipZoneCCTVs(null, null, out strErrorMessage);

            if (equipZoneCCTVs != null)
            {
                m_dicEquipZoneCCTVs.Clear();
                m_dicCCTVEquipZoneIDs.Clear();

                foreach (EquipZoneCCTV equipZoneCCTV in equipZoneCCTVs)
                {
                    CheckEquipZoneCCTV(equipZoneCCTV.EquipZoneID, equipZoneCCTV.CCTV1);
                    CheckEquipZoneCCTV(equipZoneCCTV.EquipZoneID, equipZoneCCTV.CCTV2);
                    CheckEquipZoneCCTV(equipZoneCCTV.EquipZoneID, equipZoneCCTV.CCTV3);
                    CheckEquipZoneCCTV(equipZoneCCTV.EquipZoneID, equipZoneCCTV.CCTV4);
                    CheckEquipZoneCCTV(equipZoneCCTV.EquipZoneID, equipZoneCCTV.CCTV5);
                    CheckEquipZoneCCTV(equipZoneCCTV.EquipZoneID, equipZoneCCTV.CCTV6);
                }
            }
        }

        private void CheckEquipZoneCCTV(int nEquipZoneID, int? cctvID)
        {
            if (cctvID == null)
                return;

            CCTV cctv;

            if (m_dicCCTVIDs.TryGetValue((int)cctvID, out cctv) == false)
                return;

            List<CCTV> cctvs;

            if (m_dicEquipZoneCCTVs.TryGetValue(nEquipZoneID, out cctvs) == false)
            {
                cctvs = new List<CCTV>();
                m_dicEquipZoneCCTVs[nEquipZoneID] = cctvs;
            }

            cctvs.Add(cctv);

            List<int> equipZoneIDs;

            if (m_dicCCTVEquipZoneIDs.TryGetValue(cctv, out equipZoneIDs) == false)
            {
                equipZoneIDs = new List<int>();
                m_dicCCTVEquipZoneIDs[cctv] = equipZoneIDs;
            }

            if (equipZoneIDs.Contains(nEquipZoneID) == false)
                equipZoneIDs.Add(nEquipZoneID);
        }

        public void Update(ICollection<CCTV> svmsCCTVs)
        {
            List<CCTV> changedCCTVs = new List<CCTV>();
            List<CCTV> deletedCCTVs = new List<CCTV>();
            List<CCTV> newCCTVs = new List<CCTV>();

            Logger.Instance.Write("CCTVManager.Update");
            CheckSvmsCCTVs(svmsCCTVs, changedCCTVs, deletedCCTVs, newCCTVs);

            if (changedCCTVs.Count > 0 || /*deletedCCTVs.Count > 0 || */newCCTVs.Count > 0)
            {
                List<string> safetyI_IP_List = ReadSafetyI_IPList();

                if (newCCTVs.Count > 0)
                    AddCCTVs(newCCTVs, safetyI_IP_List);

                if (changedCCTVs.Count > 0)
                    ChangeCCTVs(changedCCTVs, safetyI_IP_List);

                /*if (deletedCCTVs.Count > 0)
                    DeleteCCTVs(deletedCCTVs);*/

                // 삭제된 CCTV는 그냥 둔다.
                // 통신장애로 인하여 일시적으로 데이터를 못읽었을 가능성도 있다.

                string strStreamName = ConfigurationManager.AppSettings.Get("streamName");
                string strExeName = ConfigurationManager.AppSettings.Get("rtspServerName");
                string strRunModule = ConfigurationManager.AppSettings.Get("runRTSPServer");
                string strJsonFile = ConfigurationManager.AppSettings.Get("cctvJson");

                if (strExeName == null || strRunModule == null || strJsonFile == null)
                    return;

                if (KillProcess(strExeName))
                {
                    ICollection<CCTV> cctvs = m_dicCCTVs.Values;

                    UpdateCCTV(cctvs, strJsonFile, strStreamName);
                    RunProcess(strRunModule);
                }
            }
        }

        private CCTV FindCCTV(string strDeviceID, ICollection<CCTV> cctvList)
        {
            foreach (CCTV cctv in cctvList)
            {
                if (cctv.UniqueKey == strDeviceID)
                    return cctv;
            }

            return null;
        }

        private List<string> ReadSafetyI_IPList()
        {
            string strErrorMessage;
            List<Common.Model.Option.Options> options = m_commonDataManager.GetSelectManager().SelectOption(Common.Model.Option.Options.OptionTarget.SDMS, "Safety-I IP", out strErrorMessage);

            if (options == null || options.Count == 0)
                return null;

            string[] ips = options[0].PropertyValue.Trim().Split('/');

            List<string> ipList = new List<string>();
            
            foreach (string strIP in ips)
            {
                ipList.Add(strIP.Trim());
            }

            return ipList;
        }

        private void AddCCTVs(List<CCTV> newCCTVs, List<string> safetyI_IP_List)
        {
            foreach (CCTV cctv in newCCTVs)
            {
                string strCCTVType = IsSafetyI(cctv, safetyI_IP_List) ? Safety_I_Type : RTSP_Type;

                CCTV _cctv = m_dataManager.GetCreateManager().CreateCCTV(cctv.CameraName, "", cctv.UniqueKey, null, null, null, null, false, strCCTVType, null, cctv.UserID, cctv.Password, cctv.URL, cctv.BigURL, cctv.SmallURL, cctv.Enabled, cctv.CameraIP, cctv.CameraCompanyName, cctv.CameraModelName, null);

                if (_cctv != null)
                {
                    m_dicCCTVs[_cctv.UniqueKey] = _cctv;
                    m_dicCCTVIDs[_cctv.ID] = _cctv;

                    Logger.Instance.Write("CCTVManager.AddCCTVs, CCTV[" + _cctv.ID + "], " + _cctv.UniqueKey + ", Enabled : " + _cctv.Enabled);
                }
            }
        }

        private bool IsSafetyI(CCTV cctv, List<string> safetyI_IP_List)
        {
            if (safetyI_IP_List == null)
                return false;

            if (cctv.URL.ToLower().StartsWith("rtsp://") == false)
                return false;

            string strURL = cctv.URL.ToLower().Substring("rtsp://".Length);
            int nIndex = strURL.IndexOf('/');

            if (nIndex < 0)
                return false;

            string strIP1 = strURL.Substring(0, nIndex).Trim();

            foreach (string strIP in safetyI_IP_List)
            {
                if (strIP1 == strIP)
                    return true;
            }

            string strURL2 = strURL.Substring(nIndex + 1);
            int nIndex2 = strURL2.IndexOf('_');

            if (nIndex2 < 0)
                return false;

            string strIP2 = strURL2.Substring(0, nIndex2).Trim();

            foreach (string strIP in safetyI_IP_List)
            {
                if (strIP2 == strIP)
                    return true;
            }

            return false;
        }

        private void ChangeCCTVs(List<CCTV> changedCCTVs, List<string> safetyI_IP_List)
        {
            string strErrorMessage;

            foreach (CCTV cctv in changedCCTVs)
            {
                if (IsSafetyI(cctv, safetyI_IP_List))
                    cctv.Type = Safety_I_Type;
                else
                    cctv.Type = RTSP_Type;

                if (m_dataManager.GetUpdateManager().UpdateCCTV(cctv, out strErrorMessage) == false)
                {
                    Logger.Instance.Write("CCTVManager.ChangeCCTVs, CCTV[" + cctv.ID + "], " + cctv.UniqueKey + ", Enabled : " + cctv.Enabled);
                }
            }
        }

        private void DeleteCCTVs(List<CCTV> deletedCCTVs)
        {
            string strErrorMessage;
            string strIDs = "";

            foreach (CCTV cctv in deletedCCTVs)
            {
                if (strIDs.Length == 0)
                    strIDs = cctv.ID.ToString();
                else
                    strIDs += ", " + cctv.ID.ToString();
            }

            if (strIDs.Length == 0)
                return;

            string strConditions = string.Format("ID in ({0})", strIDs);
            m_dataManager.GetDeleteManager().DeleteCCTV(null, strConditions, out strErrorMessage);
        }

        private void CheckSvmsCCTVs(ICollection<CCTV> svmsCCTVs, List<CCTV> changedCCTVs, List<CCTV> deletedCCTVs, List<CCTV> newCCTVs)
        {
            // Key : Unique Key
            Dictionary<string, CCTV> dicCopiedCCTVs = new Dictionary<string, CCTV>();

            foreach (KeyValuePair<string, CCTV> pair in m_dicCCTVs)
            {
                dicCopiedCCTVs[pair.Key] = pair.Value;
            }

            CCTV _cctv;

            foreach (CCTV cctv in svmsCCTVs)
            {
                if (dicCopiedCCTVs.TryGetValue(cctv.UniqueKey, out _cctv) == false)
                {
                    newCCTVs.Add(cctv);
                    continue;
                }

                if (IsSame(cctv, _cctv) == false)
                {
                    if (UpdateCCTV(_cctv.ID, cctv.CameraName, cctv.URL, cctv.BigURL, cctv.SmallURL, cctv.Enabled))
                    {
                        _cctv.CameraName = cctv.CameraName;
                        _cctv.URL = cctv.URL;
                        _cctv.BigURL = cctv.BigURL;
                        _cctv.SmallURL = cctv.SmallURL;
                        _cctv.Enabled = cctv.Enabled;
                        _cctv.CameraIP = cctv.CameraIP;
                        _cctv.CameraCompanyName = cctv.CameraCompanyName;
                        _cctv.CameraModelName = cctv.CameraModelName;
                        changedCCTVs.Add(_cctv);
                        Logger.Instance.Write("CCTVManager.UpdateCCTV, CCTV[" + _cctv.ID + "], " + _cctv.UniqueKey + ", Enabled : " + _cctv.Enabled);
                    }
                }

                dicCopiedCCTVs.Remove(cctv.UniqueKey);
            }

            foreach (KeyValuePair<string, CCTV> pair in dicCopiedCCTVs)
            {
                deletedCCTVs.Add(pair.Value);
            }
        }

        private bool UpdateCCTV(int nID, string strCameraName, string strURL, string strBigURL, string strSmallURL, bool? enabled)
        {
            Dictionary<CCTV.Fields, object> dicSets = new Dictionary<CCTV.Fields, object>();
            Dictionary<CCTV.Fields, object> dicConditions = new Dictionary<CCTV.Fields, object>();

            dicSets[CCTV.Fields.CameraName] = strCameraName;
            dicSets[CCTV.Fields.URL] = strURL;
            dicSets[CCTV.Fields.BigURL] = strBigURL;
            dicSets[CCTV.Fields.SmallURL] = strSmallURL;
            dicSets[CCTV.Fields.Enabled] = enabled;
            dicConditions[CCTV.Fields.ID] = nID;

            string strErrorMessage;
            
            if (m_dataManager.GetUpdateManager().UpdateCCTV(dicSets, dicConditions, "", out strErrorMessage) == false)
            {
                System.Diagnostics.Trace.WriteLine("UpdateCCTV Error : " + strErrorMessage);
                return false;
            }

            return true;
        }

        private bool IsSame(CCTV cctv1, CCTV cctv2)
        {
            if (cctv1.CameraName != cctv2.CameraName)
                return false;

            if (cctv1.URL != cctv2.URL)
                return false;

            if (cctv1.BigURL != cctv2.BigURL || cctv1.SmallURL != cctv2.SmallURL ||
                cctv1.CameraIP != cctv2.CameraIP || cctv1.CameraModelName != cctv2.CameraModelName ||
                cctv1.CameraCompanyName != cctv2.CameraCompanyName)
                return false;

            bool isEnabled1 = cctv1.Enabled == null || (bool)cctv1.Enabled;
            bool isEnabled2 = cctv2.Enabled == null || (bool)cctv2.Enabled;

            if (isEnabled1 != isEnabled2)
                return false;

            // Camera 이름과 URL이 같으면 같은 CCTV로 간주한다.
            // UniqueKey는 이미 같다는 전제하에서다.
            return true;
        }

        public bool UpdateCCTV(CCTV cctv)
        {
            string strErrorMessage;
            CCTV _cctv;

            if (m_dicCCTVs.TryGetValue(cctv.UniqueKey, out _cctv))
            {
                bool success = m_dataManager.GetUpdateManager().UpdateCCTV(cctv, out strErrorMessage);

                if (success == false)
                {
                    System.Diagnostics.Trace.WriteLine("UpdateCCTV Fail : " + strErrorMessage);
                    return false;
                }

                _cctv.UserID = cctv.UserID;
                _cctv.Password = cctv.Password;
                _cctv.CameraName = cctv.CameraName;
                _cctv.URL = cctv.URL;
                _cctv.Enabled = cctv.Enabled;
            }
            else
            {
                _cctv = m_dataManager.GetCreateManager().CreateCCTV(cctv.CameraName, "", cctv.UniqueKey, null, null, null, null, false, "RTSP", null, cctv.UserID, cctv.Password, cctv.URL, null, null, cctv.Enabled, cctv.CameraIP, cctv.CameraCompanyName, cctv.CameraModelName, null);

                if (_cctv == null)
                {
                    System.Diagnostics.Trace.WriteLine("UpdateCCTV Fail : " + m_dataManager.GetCreateManager().GetErrorMessage());
                    return false;
                }

                m_dicCCTVs[_cctv.UniqueKey] = _cctv;
                m_dicCCTVIDs[_cctv.ID] = _cctv;
            }

            return true;
        }

        public void RestartProcess()
        {
            string strStreamName = ConfigurationManager.AppSettings.Get("streamName");
            string strExeName = ConfigurationManager.AppSettings.Get("rtspServerName");
            string strRunModule = ConfigurationManager.AppSettings.Get("runRTSPServer");
            string strJsonFile = ConfigurationManager.AppSettings.Get("cctvJson");

            if (strExeName == null || strRunModule == null || strJsonFile == null)
                return;

            if (KillProcess(strExeName))
            {
                ICollection<CCTV> cctvs = m_dicCCTVs.Values;

                UpdateCCTV(cctvs, strJsonFile, strStreamName);
                RunProcess(strRunModule);
            }
        }

        public static bool KillProcess(string strProcessName)
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
                    catch (Exception e)
                    {
                        System.Diagnostics.Trace.WriteLine(e.Message);
                        return false;
                    }
                }

            }

            return true;
        }

        public static void UpdateCCTV(ICollection<CCTV> cctvs, string strJsonFile, string strStreamName)
        {
            string strJson = "";
            StreamReader reader = new StreamReader(strJsonFile, Encoding.UTF8);

            if (reader.EndOfStream == false)
            {
                strJson = reader.ReadToEnd();
            }

            reader.Close();

            if (strJson.Length > 0)
            {
                JObject json = JObject.Parse(strJson);

                /*JObject streams = (JObject)json["streams"];
                JObject streamData = (JObject)streams[strStreamName];

                if (streamData.ContainsKey("channels"))
                {
                    streamData.Remove("channels");
                }

                JObject channels = new JObject();
                int index = 0;

                foreach (CCTV cctv in cctvs)
                {
                    JObject url = new JObject();
                    url["debug"] = false;
                    url["url"] = cctv.URL;

                    channels[string.Format("{0}", index++)] = url;
                    //channels[cctv.UniqueKey] = url;
                }

                streamData["channels"] = channels;*/

                if (json.ContainsKey("streams"))
                {
                    json.Remove("streams");
                }

                JObject streams = new JObject();
                //int index = 0;

                foreach (CCTV cctv in cctvs)
                {
                    string strURL = cctv.SmallURL != null ? cctv.SmallURL : cctv.URL;

                    JObject url = new JObject();
                    url["on_demand"] = true;
                    url["disable_audio"] = true;
                    url["url"] = strURL;

                    streams[string.Format("{0}", cctv.ID)] = url;
                    //streams[string.Format("{0}", index++)] = url;
                    //streams[cctv.UniqueKey] = url;
                }

                json["streams"] = streams;

                UTF8Encoding utf8WithoutBOM = new UTF8Encoding(false);

                StreamWriter writer = new StreamWriter(strJsonFile, false, utf8WithoutBOM);
                writer.Write(json.ToString());
                writer.Close();
            }
        }

        public static Process RunProcess(string strFilePath)
        {
            int nIndex = strFilePath.LastIndexOf('\\');

            string strWorkingDirectory = nIndex >= 0 ? strFilePath.Substring(0, nIndex) : ".";
            string strFileName = nIndex >= 0 ? strFilePath.Substring(nIndex + 1) : strFilePath;

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";
            startInfo.WorkingDirectory = strWorkingDirectory;
            startInfo.ErrorDialog = true;
            startInfo.Arguments = "/C " + strFileName;

            Process process;
            try
            {
                process = Process.Start(startInfo);
                return process;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }

            return null;
        }

        public void SendEvent(DateTime eventTime, string uniqueKey, Facility.FacilityType sensorType)
        {
            CCTV cctv;

            if (m_dicCCTVs.TryGetValue(uniqueKey, out cctv))
            {
                Dictionary<SensorZone.Fields, object> dicConditions = new Dictionary<SensorZone.Fields, object>();
                dicConditions[SensorZone.Fields.SensorType] = (int)sensorType;
                dicConditions[SensorZone.Fields.OrgSensorID] = cctv.ID;

                string strErrorMessage;
                ArrayList arrDatas = m_dataManager.GetSelectManager().JoinSensorZoneTagInfo(dicConditions, null, null, out strErrorMessage);

                if (arrDatas == null || arrDatas.Count < 2)
                    return;

                if (arrDatas[0] is SensorZone && arrDatas[1] is TagInfo)
                {
                    SensorZone sensorZone = (SensorZone)arrDatas[0];
                    TagInfo tagInfo = (TagInfo)arrDatas[1];

                    ArrayList arr = new ArrayList();
                    arr.Add((int)sensorType);
                    arr.Add(tagInfo.ID);
                    arr.Add(sensorZone.ID);
                    arr.Add(true);

                    SopQueryManager mgr = new SopQueryManager();
                    mgr.SendAlarmQuery(arr, "POST", m_strAlarmURL);
                }
            }
        }

        public void SendTestEvent(DateTime eventTime, int cctvID, Facility.FacilityType sensorType)
        {
            CCTV cctv = null;

            foreach (KeyValuePair<string, CCTV> pair in m_dicCCTVs)
            {
                if (pair.Value.ID == cctvID)
                {
                    cctv = pair.Value;
                    break;
                }
            }

            if (cctv == null)
                return;

            Dictionary<SensorZone.Fields, object> dicConditions = new Dictionary<SensorZone.Fields, object>();
            dicConditions[SensorZone.Fields.SensorType] = (int)sensorType;
            dicConditions[SensorZone.Fields.OrgSensorID] = cctv.ID;

            string strErrorMessage;
            ArrayList arrDatas = m_dataManager.GetSelectManager().JoinSensorZoneTagInfo(dicConditions, null, null, out strErrorMessage);

            if (arrDatas == null || arrDatas.Count < 2)
                return;

            if (arrDatas[0] is SensorZone && arrDatas[1] is TagInfo)
            {
                SensorZone sensorZone = (SensorZone)arrDatas[0];
                TagInfo tagInfo = (TagInfo)arrDatas[1];

                ArrayList arr = new ArrayList();
                arr.Add((int)sensorType);
                arr.Add(tagInfo.ID);
                arr.Add(sensorZone.ID);
                arr.Add(true);

                SopQueryManager mgr = new SopQueryManager();
                mgr.SendAlarmQuery(arr, "POST", m_strAlarmURL);
            }
        }
    }
}
