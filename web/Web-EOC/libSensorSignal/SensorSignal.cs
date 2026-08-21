using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBUtility;

namespace SensorTester
{
    public class SensorSignal
    {
        private NetworkClient m_netMgr = null;
 
        private int m_nSiteID = 1;
        private string m_szServerIP = "";

        private WebDBManager m_dbMgr = null;
        public SensorSignal(int nSiteID, string szServerIP)
        {

            try
            {
                log4net.Config.XmlConfigurator.Configure();
            }
            catch (System.Exception)
            {
            }

            m_nSiteID = nSiteID;
            m_szServerIP = szServerIP;
        }
       
        public void ConnectServer()
        {
            if (m_netMgr == null)
            {
                if (m_dbMgr == null)
                    m_dbMgr = new WebDBManager(m_nSiteID);
                m_netMgr = new NetworkClient(m_dbMgr, m_szServerIP, m_nSiteID);                
            }          
        }

        public void DisconnectServer()
        {
            if( m_netMgr != null)
            {
                m_netMgr.ReleaseThread();
                m_netMgr = null;
            }
        }

        /// <summary>
        /// SOP서버로 센서 동작을 전송하는 함수
        /// </summary>
        /// <param name="nSensorZoneID">SensorZone의 ID</param>
        /// <param name="nData">센서 data값 화재의 경우 1/0, 
        /// PSM의 경우
        /// CLEAR_PSM_ALARM = 20,
        ///    PSM_ALARM_1 = 21,
        ///    PSM_ALARM_2 = 22,
        ///    PSM_ALARM_3 = 23 </param>
        public void SendSensorActivate(int nSensorZoneID, int nData, bool bPSM)
        {
            if (m_netMgr != null)
            {
                int nType = bPSM == true ? 11 : 0;
                m_netMgr.SendSensorData(nSensorZoneID, nType, nData, bPSM);
            }
            else
            {
                throw new Exception("Network client not init");
            }
        }

    }
}
