using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpLib2;
using System.Collections;
using libUSS;
using SOPWebClient;
using System.Threading;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using DBUtility2;
using UnE.Sensor;

namespace USSFireSensorServer.Network
{
    public class USSServer
    {
        private TcpServer m_server = null;
        private USSServiceProvider m_provider = null;
        private int m_nPort = 0;
        private bool m_isOpened = false;
        private IUSSServiceOwner m_owner = null;
        private Logger m_logger = null;
        private bool m_closeEvent = false;
        private bool m_readEvent = true;
        private int m_nOfficeAID = -1;
        private int m_nOfficeBID = -1;
        private int m_nEarthquakeSensorZoneID = -1;
        // Key : Building ID
        // Value : 강풍 SensorZone ID
        private Dictionary<int, int> m_dicBuildingWindSensors = new Dictionary<int, int>();

        public USSServiceProvider Provider
        {
            get { return m_provider; }
        }

        private WebDBManager m_dbMgr = null;

        public USSServer(int nPort, IUSSServiceOwner owner, Logger logger, WebDBManager dbMgr)
        {
            m_nPort = nPort;
            m_owner = owner;
            m_logger = logger;
            m_dbMgr = dbMgr;

            ReadOfficeBuildingID();
            ReadEarthquakeSensorZone(dbMgr);
            ReadWindSensor(dbMgr);
        }

        private void ReadEarthquakeSensorZone(WebDBManager dbMgr)
        {
            string strSQL = "Select ID from SensorZone where Type = " + ((int)UnE.Sensor.IFacility.FacilityType.Earthquake).ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return;

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

            if (id == null)
                return;

            m_nEarthquakeSensorZoneID = id.Data;
        }

        private void ReadWindSensor(WebDBManager dbMgr)
        {
            string strSQL = "Select sz.ID, b.ID from SensorZone as sz, Zone as z, Building as b where sz.Zone = z.ID and z.BuildingID = b.ID and sz.Type = ";
            strSQL += ((int)IFacility.FacilityType.STRONG_WIND).ToString();

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                VariousData<int> sensorZoneID = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> buildingID = WebDBManager.GetIntField(arrResult[i + 1].ToString());

                if (sensorZoneID == null || buildingID == null)
                    continue;

                if (buildingID.Data == m_nOfficeAID)
                    m_dicBuildingWindSensors[m_nOfficeAID] = sensorZoneID.Data;
                else if (buildingID.Data == m_nOfficeBID)
                    m_dicBuildingWindSensors[m_nOfficeBID] = sensorZoneID.Data;
            }
        }

        public bool BeginServer()
        {
            if (m_nPort > 0)
            {
                if (m_provider != null)
                {
                    m_provider.ReleaseThread();
                }

                m_provider = new USSServiceProvider(m_owner, m_logger);

                m_server = new TcpServer(m_provider, m_nPort);
                m_isOpened = m_server.Start();

                // 지진, 강풍 이벤트 감시
                Thread t = new Thread(new ThreadStart(MonitoringThread));
                t.Start();
            }

            return m_isOpened;
        }

        public void StopServer()
        {
            m_closeEvent = true;

            if (m_provider != null)
            {
                if (m_isOpened)
                {
                    m_server.Stop();
                    m_isOpened = false;
                }

                m_provider.ReleaseThread();
                m_provider = null;
            }
        }

        private void ReadOfficeBuildingID()
        {
            string strOfficeA = System.Configuration.ConfigurationManager.AppSettings["officeA"].ToString().Trim();
            string strOfficeB = System.Configuration.ConfigurationManager.AppSettings["officeB"].ToString().Trim();

            int officeA, officeB;

            if (int.TryParse(strOfficeA, out officeA))
                m_nOfficeAID = officeA;

            if (int.TryParse(strOfficeB, out officeB))
                m_nOfficeBID = officeB;
        }

        public void SendPowerOffSignal(bool powerOff, int nBuildingID, int nSpaceID, DateTime timeStamp)
        {
            byte on = powerOff ? (byte)1 : (byte)0;
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(on);
            arrDatas.Add(nBuildingID);
            arrDatas.Add(nSpaceID);
            arrDatas.Add(timeStamp);
            byte[] bytes = BinaryHelper.MakeBytes(Header.POWER_OFF_DATA, arrDatas);

            if (bytes != null && m_provider != null)
            {
                m_provider.Send(bytes, 0, bytes.Length, EventType.PowerOff);
            }
        }

        // 이벤트(지진, 강풍) 감시
        private void MonitoringThread()
        {
            string strURL = System.Configuration.ConfigurationManager.AppSettings["earthWindURL"].ToString().Trim();

            if (strURL.Length == 0)
                return;

            strURL = strURL.Replace("abcdefghijk", "&");

            while (m_closeEvent == false)
            {
                if (m_readEvent)
                {
                    ReadEvent(strURL);
                    //SendEarthWind();
                }

                Thread.Sleep(1000);
            }
        }

        //private StreamWriter shsw = new StreamWriter(@"C:\UNE\Log\SH.txt", true);
        private void WriteLog(string strLog)
        {
            //shsw.WriteLine("1");
            //shsw.Flush();
        }
        //private void SendEarthWind()
        //{
        //    DateTime dtNow = DateTime.Now;

        //    string strSQL = "SELECT intensity, wSpeed_a02, wSpeed_c02, alarm_a02, alarm_c02 From earthWind Where EventTime = '" + dtNow.ToString("yyyy-MM-dd HH:mm:ss") + "'";

        //    ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
        //    if (arrResult == null || arrResult.Count != 5)
        //    {
        //        if (arrResult == null)
        //        {
        //            shsw.WriteLine("2 arrResult is null");
        //            shsw.Flush();
        //        }
        //        else
        //        {
        //            shsw.WriteLine("2 arrResult count " + arrResult.Count);
        //            shsw.Flush();
        //        }
        //        return;
        //    }

        //    VariousData<int> intensity = WebDBManager.GetIntField(arrResult[0].ToString());
        //    VariousData<double> wSpeedOfficeA = WebDBManager.GetDoubleField(arrResult[1].ToString());
        //    VariousData<double> wSpeedOfficeB = WebDBManager.GetDoubleField(arrResult[2].ToString());
        //    VariousData<int> alarm_a02 = WebDBManager.GetIntField(arrResult[3].ToString());
        //    VariousData<int> alarm_c02 = WebDBManager.GetIntField(arrResult[4].ToString());
            
        //    shsw.WriteLine("3 / " + intensity.Data + ", " + wSpeedOfficeA.Data + ", " + wSpeedOfficeB.Data + ", " + alarm_a02.Data + ", " + alarm_c02.Data);
        //    shsw.Flush();

        //    if (m_provider != null)
        //    {
        //        if (m_owner != null)
        //        {
        //            if (m_nEarthquakeSensorZoneID > 0 && intensity != null)
        //            {
        //                m_owner.OnEarthquakeSignal(intensity.Data, m_nEarthquakeSensorZoneID, dtNow);
        //            }
        //        }

        //        if (intensity != null)
        //            m_provider.SendEarthquakeSignal(intensity.Data, dtNow);

        //        SendWindData(m_nOfficeAID, wSpeedOfficeA, dtNow);
        //        SendWindData(m_nOfficeBID, wSpeedOfficeB, dtNow);

        //        shsw.WriteLine("send");
        //        shsw.Flush();
        //    }
        //    else
        //    {
        //        shsw.WriteLine("4 provider is null");
        //        shsw.Flush();
        //    }
        //}

        private void ReadEvent(string strURL)
        {
            string resResult = string.Empty;

            try
            {
                Uri uri = new Uri(strURL); // string 을 Uri 로 형변환
                HttpWebRequest wReq = (HttpWebRequest)WebRequest.Create(uri);
                
                wReq.Method = "GET";
                wReq.ServicePoint.Expect100Continue = false;

                using (HttpWebResponse wRes = (HttpWebResponse)wReq.GetResponse())
                {
                    Stream respPostStream = wRes.GetResponseStream();
                    //StreamReader readerPost = new StreamReader(respPostStream, Encoding.Default, true);
                    StreamReader readerPost = new StreamReader(respPostStream, Encoding.GetEncoding("EUC-KR"), true);

                    resResult = readerPost.ReadToEnd();
                }

                System.Diagnostics.Trace.WriteLine("Read Json : " + resResult);

                bool result;
                List<JObject> objList = ReadJson(resResult, out result);

                if (objList == null || objList.Count < 5)
                    return;

                JToken token;
                VariousData<int> intensity = null, wAlarmOfficeA = null, wAlarmOfficeB = null;
                VariousData<double> wSpeedOfficeA = null, wSpeedOfficeB = null;

                foreach (JObject obj in objList)
                {
                    if (obj.TryGetValue("mmi", out token))
                    {
                        GetJsonValue(token, out intensity);
                    }
                    else if (obj.TryGetValue("wspeed_a02", out token))
                    {
                        GetJsonValue(token, out wSpeedOfficeA);
                    }
                    else if (obj.TryGetValue("wspeed_c02", out token))
                    {
                        GetJsonValue(token, out wSpeedOfficeB);
                    }
                    else if (obj.TryGetValue("alarm_a02", out token))
                    {
                        GetJsonValue(token, out wAlarmOfficeA);
                    }
                    else if (obj.TryGetValue("alarm_c02", out token))
                    {
                        GetJsonValue(token, out wAlarmOfficeB);
                    }
                }

                DateTime dtNow = DateTime.Now;

                if (m_provider != null)
                {
                    if (m_owner != null)
                    {
                        if (m_nEarthquakeSensorZoneID > 0 && intensity != null)
                        {
                            m_owner.OnEarthquakeSignal(intensity.Data, m_nEarthquakeSensorZoneID, dtNow);
                        }
                    }

                    if (intensity != null)
                        m_provider.SendEarthquakeSignal(intensity.Data, dtNow);

                    SendWindData(m_nOfficeAID, wSpeedOfficeA, dtNow);
                    SendWindData(m_nOfficeBID, wSpeedOfficeB, dtNow);
                }

                /*System.Diagnostics.Trace.WriteLine("진도 : " + intensity.Data);
                System.Diagnostics.Trace.WriteLine("풍속 OfficeA  : " + wSpeedOfficeA.Data);
                System.Diagnostics.Trace.WriteLine("풍속 OfficeB : " + wSpeedOfficeB.Data);
                System.Diagnostics.Trace.WriteLine("강풍 위험단계 OfficeA : " + wAlarmOfficeA.Data);
                System.Diagnostics.Trace.WriteLine("강풍 위험단계 OfficeB : " + wAlarmOfficeB.Data);*/
            }
            catch (System.Net.WebException ex)
            {
                System.Diagnostics.Trace.WriteLine("Fail : " + ex.Message);
            }
        }

        public void StartReadEvent()
        {
            m_readEvent = true;
        }

        public void StopReadEvent()
        {
            m_readEvent = false;
        }

        public void SendSimulationWindSpeed(int nSensorID, float fWindSpeed)
        {
            if (m_provider != null)
            {
                SendWindData(nSensorID, new VariousData<double>(fWindSpeed), DateTime.Now);
            }
        }

        public void SendSimulationEarthquake(int nIntensity)
        {
            if (m_provider != null)
            {
                DateTime dtNow = DateTime.Now;

                if (m_owner != null)
                {
                    if (m_nEarthquakeSensorZoneID > 0)
                    {
                        m_owner.OnEarthquakeSignal(nIntensity, m_nEarthquakeSensorZoneID, dtNow);
                    }
                }

                m_provider.SendEarthquakeSignal(nIntensity, dtNow);
            }
        }

        private void SendWindData(int nBuildingID, VariousData<double> windSpeed, DateTime timeStamp)
        {
            if (nBuildingID > 0 && windSpeed != null)
            {
                if (m_owner != null)
                {
                    int nSensorZoneID;

                    if (m_dicBuildingWindSensors.TryGetValue(nBuildingID, out nSensorZoneID))
                        m_owner.OnStrongWindSignal((float)windSpeed.Data, nSensorZoneID, timeStamp);
                }

                m_provider.SendWindSignal(nBuildingID, (float)windSpeed.Data, timeStamp);
            }
        }

        private bool GetJsonValue(JToken token, out VariousData<int> value)
        {
            string str = token.ToString();
            int data;

            if (int.TryParse(str, out data))
            {
                value = new VariousData<int>(data);
                return true;
            }
        
            value = null;
            return false;
        }

        private bool GetJsonValue(JToken token, out VariousData<double> value)
        {
            string str = token.ToString();
            double data;

            if (double.TryParse(str, out data))
            {
                value = new VariousData<double>(data);
                return true;
            }
        
            value = null;
            return false;
        }

        private List<JObject> ReadJson(string strJson, out bool result)
        {
            List<JObject> objectList = new List<JObject>();
            result = false;

            int index = strJson.IndexOf("{");
            if (index > 0)
                strJson = strJson.Remove(0, index);

            JObject obj = JObject.Parse(strJson);
            JToken token;

            if (obj.TryGetValue("bid_data", out token))
            {
                if (token.Count() > 0)
                {
                    JToken token2 = token.ElementAt(0);

                    string strToken = token2.ToString();
                    string str = GetJson(strToken);

                    JObject data = JObject.Parse(str);

                    if (data.TryGetValue("genmax", out token))
                    {
                        try
                        {
                            int nChildCount = token.Count();

                            for (int i = 0; i < nChildCount; i++)
                            {
                                JToken _token = token.ElementAt(i);

                                strToken = _token.ToString();
                                str = GetJson(strToken);

                                data = JObject.Parse(str);
                                objectList.Add(data);
                            }
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Trace.WriteLine(e.Message);
                        }

                        return objectList;
                    }
                }
            }

            return null;
        }

        private string GetJson(string str)
        {
            if (str.StartsWith("["))
            {
                str = str.Substring(1, str.Length - 2).Trim();
            }
            else if (str.StartsWith("{"))
                return str;
            else
            {
                str = "{" + str + "}";
            }

            return str;
        }
    }
}
