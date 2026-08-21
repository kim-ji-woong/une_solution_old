using dnsDBUtil;
using FireSensorServer.Data;
using FireSensorServer.Network;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FireSensorServer
{
    public partial class FormMain : Form
    {
        private WebDBManager m_dbManager = null;
        private DataManager m_dataManager = null;
        private NetworkWebManager m_networkManager = null;

        public FormMain()
        {
            InitializeComponent();

            //((16 * 2 * 127) * (수신기번호 - 1)) +(2 * 127 * (중계반번호 - 1))+(127 * loop 번호) +(중계기 - 1);
            float a = ((16 * 2 * 127) * (1 - 1)) + (2 * 127 * (6 - 1)) + (127 * 0) + (50 - 1);
            Console.WriteLine(a);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            string strIP = System.Configuration.ConfigurationManager.AppSettings["ip"].ToString();
            string strPort = System.Configuration.ConfigurationManager.AppSettings["Port"].ToString();
            string strSiteID = System.Configuration.ConfigurationManager.AppSettings["site"].ToString();
            string strWebServerURL = System.Configuration.ConfigurationManager.AppSettings["webServerURL"].ToString();
            string strDbName = System.Configuration.ConfigurationManager.AppSettings["dbName"].ToString();
            string strDbType = System.Configuration.ConfigurationManager.AppSettings["dbType"].ToString();

            int nSiteID = int.Parse(strSiteID);
            int nDBType = int.Parse(strDbType);

            m_dbManager = new WebDBManager(strDbName, nDBType, nSiteID, strWebServerURL);            
            m_dataManager = new DataManager(m_dbManager);
            //m_networkManager = new NetworkWebManager(m_dbManager);

            Dictionary<int, SensorDetectorEntity> detetors = MakeDetector();
            foreach (KeyValuePair<int, SensorDetectorEntity> item in detetors)
            {
                SensorDetectorManager sensorDetectorManager = new SensorDetectorManager(item.Value);
                sensorDetectorManager.Start();

                break;
            }
        }

        private Dictionary<int, SensorDetectorEntity> MakeDetector()
        {
            Dictionary<int, SensorDetectorEntity> m_dicNMux = new Dictionary<int, SensorDetectorEntity>();
            foreach (SensorInfo sensor in DataManager.DicSensorTagInfo)
            {
                int tagNo = sensor.TagNo;
                // 100121
                int receiverID = int.Parse(tagNo.ToString().Substring(0, 1)); // 중계반: 1
                int loop = int.Parse(tagNo.ToString().Substring(1, 1));       //  LOOP: 0
                string relayID = tagNo.ToString().Substring(2, 3);    // 중계기: 012

                string detectorKey = receiverID.ToString() + loop.ToString() + relayID.ToString();

                NMuxNetworkManager nm = null;
                SensorDetectorEntity entity = null;
                if (!m_dicNMux.ContainsKey(receiverID))
                {
                    nm = new NMuxNetworkManager();
                    nm.ReceiverID = receiverID;

                    entity = new SensorDetectorEntity();
                    entity.NMuxNetworkMan = nm;
                    entity.DicSensorDetector = new Dictionary<string, SensorDetector>();

                    m_dicNMux.Add(receiverID, entity);
                }
                else
                {
                    entity = m_dicNMux[receiverID];
                    nm = m_dicNMux[receiverID].NMuxNetworkMan;
                }

                SensorDetector detector = null;

                int address = GetFormular(receiverID, loop, relayID);
                if (!entity.DicSensorDetector.ContainsKey(detectorKey))
                {
                    detector = new SensorDetector(address);
                    entity.DicSensorDetector.Add(detectorKey, detector);
                }
                else
                {
                    detector = entity.DicSensorDetector[detectorKey];
                }
                address = 40000 + address;

                DetectRegister dr = new DetectRegister(address);
                dr.SensorTagInfo = sensor;

                detector.Detectors.Add(dr);
            }

            return m_dicNMux;
        }

        private int GetFormular(int nReceiverID, int nLoop, string strRelayID)
        {
            int temp = 1; // 수신기 번호
            int nRelayID = int.Parse(strRelayID);
            int value = ((16 * 2 * 127) * (temp - 1)) + (2 * 127 * (nReceiverID - 1)) + (127 * nLoop) + (nRelayID - 1);

            return value;
        }
    }
}
