using dnsCommunicateSopServer;
using dnsDBUtil;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MuxFireSensorServer.Network
{
    /// <summary>
    /// SOPWebServer로 송신
    /// </summary>
    public class NetworkWebClient
    {
        private static NetworkWebClient m_instance = null;
        private SopQueryManager m_sopQueryManager = null;

        public static NetworkWebClient Instance
        {
            get { return m_instance; }
        }

        public NetworkWebClient()
        {
            m_instance = this;

            string strURL = System.Configuration.ConfigurationManager.AppSettings.Get("sopApiUrl");
            m_sopQueryManager = new SopQueryManager(strURL);
        }

        public bool SendSensorData(SensorTag sensor, int nSensorData)
        {
            try
            {
                ArrayList arrDatas = new ArrayList();
                //arrDatas.Add(dnsSopID.Header.SENSOR_DATA);
                arrDatas.Add(0); // 0: 화재
                arrDatas.Add(sensor.TagNo);
                arrDatas.Add(sensor.SensorZoneID);
                arrDatas.Add((nSensorData == 1) ? true : false);

                bool result = m_sopQueryManager.SendAlarmQuery(arrDatas, "POST");
                return result;
            }
            catch (Exception ex)
            {
                Logger.Instance.Write("[ERROR] NetworkWebClient.cs > bool SendSensorData(SensorTag, int) :" + ex.Message);
                return false;
            }
        }

        public bool SendAllClear()
        {
            return m_sopQueryManager.SendAllClearQuery("POST");
        }
    }
}
