using FireSensorServer.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FireSensorServer.Network
{
    public class SensorDetectorManager
    {
        private string m_confPath = "";
        private string szConfFileName = "ModbusConfig.ini";

        /// <summary>
        /// 종료 여부
        /// </summary>
        private bool m_bExitThread = false;

        /// <summary>
        /// Detector들을 순회하며 값을 가져오는 Thread
        /// </summary>
        private Thread m_CheckThread = null;

        /// <summary>
        /// 중계반별로 센서리스트를 만든다
        /// </summary>
        private Dictionary<int, SensorDetectorEntity> m_dicNMux = null;

        private SensorDetectorEntity m_sensorDetectorEntity = null;
        public SensorDetectorEntity SensorDetectorEntity
        {
            get { return m_sensorDetectorEntity; }
            set { m_sensorDetectorEntity = value; }
        }

        public SensorDetectorManager(SensorDetectorEntity entity)
        {
            m_sensorDetectorEntity = entity;
        }

        public bool Start()
        {            
            BeginNetworkManager(m_sensorDetectorEntity.NMuxNetworkMan);

            m_bExitThread = false;
            // 생성이 안된 경우 생성
            if (m_CheckThread == null || m_CheckThread.IsAlive == false)
            {
                m_CheckThread = new Thread(CheckEvent);
                m_CheckThread.Start();
            }

            return true;
        }

        private void BeginNetworkManager(NMuxNetworkManager mgr)
        {
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(BeginNetworkManagerThread));
            t.Start(mgr);
        }

        private void BeginNetworkManagerThread(object arg)
        {
            NMuxNetworkManager mgr = (NMuxNetworkManager)arg;
            mgr.BeginServer();
        }

        private void CheckEvent()
        {
            int nCount = 0;
            while (!m_bExitThread)
            {
                Console.WriteLine(this.m_sensorDetectorEntity.DicSensorDetector.Count);
                Dictionary<string, SensorDetector> detectors = new Dictionary<string, SensorDetector>(m_sensorDetectorEntity.DicSensorDetector);
                
                NMuxNetworkManager sm = m_sensorDetectorEntity.NMuxNetworkMan;
                Console.WriteLine(sm.ReceiverID + "]Begin : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                if (sm.IsConnected)
                {
                    foreach (KeyValuePair<string, SensorDetector> detector in detectors)
                    {
                        detector.Value.CheckValue(sm, 3000);

                        if (m_bExitThread == true)
                            break;

                        nCount++;

                        if (nCount == 1000)
                        {
                            try
                            {
                                nCount = 0;
                                if (m_bExitThread == false)
                                    GC.Collect();
                            }
                            catch (Exception)
                            { }
                        }
                    }
                }

                for (int i = 0; i < 100; i++)
                {
                    Thread.Sleep(5);
                    if (m_bExitThread == true)
                        break;
                }

                Console.WriteLine(sm.ReceiverID + "]End : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                //m_bExitThread = true;
            }
        }

        /// <summary>
        /// 특정 Comm에서 AlarmUnit의 bit 상태를 검사
        /// </summary>
        /// <param name="nComm">Comm Unit ID</param>
        /// <param name="nAlarmUnit">Alarm Unit ID</param>
        /// <param name="nChannel">Bit</param>
        /// <returns>Status (1:Active, 0:Normal -1:Offline)</returns>
        public int GetStatus(int nComm, int nAlarmUnit)
        {
            SensorDetector detector = FindDetector(nComm);
            if (detector != null)
            {

                return detector.GetStatus(nAlarmUnit);
            }
            return 0;
        }

        private SensorDetector FindDetector(int nID)
        {
            //foreach (KeyValuePair<string, SensorDetector> detector in m_sensorDetectorEntity.DicSensorDetector)
            //{
            //    if (detector.ID == nID)
            //    {
            //        return detector;
            //    }
            //}
            return null;
        }

        //public bool GetOnline(int nDetectID)
        //{
        //    SensorDetector dt = FindDetector(nDetectID);
        //    if (dt != null)
        //    {
        //        return dt.IsOnline();
        //    }
        //    return false;
        //}

        private void EndCollback(IAsyncResult ar)
        {
            try
            {
                ((Action)ar.AsyncState).EndInvoke(ar);
            }
            catch (Exception)
            {
                // 종료되었는지 검사
            }
        }

        public void End()
        {
            m_bExitThread = true;
            // 반드시 Check Thread를 먼저 종료한다.
            try
            {
                if (m_CheckThread != null && m_CheckThread.IsAlive)
                {
                    m_CheckThread.Join(3000);
                }
            }
            catch (Exception)
            {
            }

            Dictionary<string, SensorDetector> detectors = new Dictionary<string, SensorDetector>(m_sensorDetectorEntity.DicSensorDetector);
            m_sensorDetectorEntity.NMuxNetworkMan.StopServer();
            foreach (KeyValuePair<string, SensorDetector> detector in detectors)
            { 
                detector.Value.SetOff();
            }
        }
    }

    public class SensorDetectorEntity
    {
        private NMuxNetworkManager m_nmux = null;
        public NMuxNetworkManager NMuxNetworkMan
        {
            get { return m_nmux; }
            set { m_nmux = value; }
        }

        private Dictionary<string, SensorDetector> m_dicSensorDetector = null;
        public Dictionary<string, SensorDetector> DicSensorDetector
        {
            get { return m_dicSensorDetector; }
            set { m_dicSensorDetector = value; }
        }            
    }
}
