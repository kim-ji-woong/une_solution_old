using dnsCommunicateSopServer;
using dnsData.Sensor;
using System.Collections;
using System.Threading;

namespace FireSensorServer.Network
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

        public bool SendSensorData(int nTagID, int nSensorZoneID, int nSensorData)
        {
            ArrayList arrDatas = new ArrayList();
            arrDatas.Add((int)Facility.FacilityType.FIRE_SENSOR);
            arrDatas.Add(nTagID);
            arrDatas.Add(nSensorZoneID);
            arrDatas.Add((nSensorData == 1) ? true : false);

            bool result = m_sopQueryManager.SendAlarmQuery(arrDatas, "POST");
            return result;
        }

        public void SendSensorDataAsync(int nTagID, int nSensorZoneID, int nSensorData)
        {
            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(nTagID);
            arrDatas.Add(nSensorZoneID);
            arrDatas.Add(nSensorData);

            Thread t = new Thread(new ParameterizedThreadStart(SendSensorDataThread));
            t.Start(arrDatas);
        }

        private void SendSensorDataThread(object param)
        {
            ArrayList arrDatas = (ArrayList)param;

            int nTagID = (int)arrDatas[0];
            int nSensorZoneID = (int)arrDatas[1];
            int nSensorData = (int)arrDatas[2];

            SendSensorData(nTagID, nSensorZoneID, nSensorData);
        }

        public bool SendAllClear()
        {
            return m_sopQueryManager.SendAllClearQuery("POST");
        }

        public void SendAllClearAsync()
        {
            Thread t = new Thread(new ThreadStart(SendAllClearThread));
            t.Start();
        }

        private void SendAllClearThread()
        {
            SendAllClear();
        }
    }
}
