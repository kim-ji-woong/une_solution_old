using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;


namespace GasDetector
{
    public delegate void AlarmNotifyDelegate(int nComm, int nAlarmUnit, float fValue, int nChannel, int nStatus);
    
    public class DetectorManager : IDisposable
    {
        public event AlarmNotifyDelegate OnNotifyAlarm;

        /// <summary>
        /// INI파일에 설정된 GasDetector의 갯수
        /// </summary>
        private int m_nDetectorCount = 0;
        public int DetectorCount
        {
            get { return m_nDetectorCount; }
        }
        
        /// <summary>
        /// INI에서 로딩된 GasDetector 리스트
        /// </summary>
        private List<GasDetector> m_DetectorList = new List<GasDetector>();
        internal List<GasDetector> DetectorList
        {
            get { return m_DetectorList; }
        }
       
        /// <summary>
        /// INI파일 Path
        /// </summary>
        private string m_confPath = "";

        /// <summary>
        /// Serial통신 관리자
        /// </summary>
        private SerialManager m_SerialManager = null;

        /// <summary>
        /// Detector들을 순회하며 값을 가져오는 Thread
        /// </summary>
        private Thread m_CheckThread = null;

        /// <summary>
        /// 종료 여부
        /// </summary>
        private bool m_bExitThread = false;
        
        public DetectorManager(string iniFilePath = "")
        {
            m_confPath = iniFilePath;
            ReadConfig();
        }

        public void Dispose()
        {
            End();
        }

        private void ReadConfig()
        {
            if( m_confPath == "")
            {
                string szPath = Assembly.GetEntryAssembly().Location;
                string szFullPath = Directory.GetParent(szPath).FullName;
                m_confPath = szFullPath + "\\GasDetectorConfig.ini";
                if(File.Exists(m_confPath))
                {
                    ConfigFile conf = new ConfigFile(m_confPath);
                    //m_SerialManager = new SerialManager(this, conf);                    
                    DetectorFactory factory = new DetectorFactory(this, conf);

                    List<GasDetector> list = factory.CreateDetectorList();
                    if (list != null)
                    {
                        m_DetectorList.AddRange(list);
                    }
                }
            }
        }

        public bool Start()
        {
            //bool bResult = false;
            // 통신 체크 전에 시리얼 서버를 먼저 연결한다.
            //if(m_SerialManager != null)
            {
                List<GasDetector> detectors = new List<GasDetector>(m_DetectorList);
                foreach (GasDetector detector in detectors)
                {
                    if(detector.TcpMode == false)
                    {
                        BeginSerialManager(detector.Serial);
                        //bResult = detector.Serial.BeginServer();
                        //if (bResult == false)
                        //{
                        //    break;
                        //}
                    }
                    else
                    {
                        BeginNetworkManager(detector.NetworkMan);
                        //bResult = detector.NetworkMan.BeginServer();
                        //if (bResult == false)
                        //{
                        //    break;
                        //}
                    }
                    
                }
            }

            // 통신이 연결되었을 경우만 Thread를 실행한다.
            //if (bResult == true)
            {
                m_bExitThread = false;

                // 생성이 안된 경우 생성
                if (m_CheckThread == null || m_CheckThread.IsAlive == false)
                {
                    m_CheckThread = new Thread(CheckDetector);
                    m_CheckThread.Name = "GasDetectorChecker";
                    m_CheckThread.Start();
                }                  
            }

            return true;
            //return bResult;
        }

        private void BeginNetworkManager(NetworkManager mgr)
        {
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(BeginNetworkManagerThread));
            t.Start(mgr);
        }

        private void BeginSerialManager(SerialManager mgr)
        {
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(BeginSerialManagerThread));
            t.Start(mgr);
        }

        private void BeginNetworkManagerThread(object arg)
        {
            NetworkManager mgr = (NetworkManager)arg;
            mgr.BeginServer();
        }

        private void BeginSerialManagerThread(object arg)
        {
            SerialManager mgr = (SerialManager)arg;
            mgr.BeginServer();
        }


        private int m_nCount = 0;
        private void CheckDetector()
        {
            int nCount = 0;
            while(!m_bExitThread)
            {
                
                List<GasDetector> detectors = new List<GasDetector>(m_DetectorList);

                if (detectors == null || detectors.Count == 0)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        Thread.Sleep(10);
                        if (m_bExitThread == true)
                            break;
                    }

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

                foreach(GasDetector detector in detectors)
                {
                    if(detector.TcpMode == false)
                    {
                        SerialManager sm = detector.Serial;
                        if (sm.IsConnected == true)
                        {
                            detector.CheckValue(sm, 1000);

                            if (m_bExitThread == true)
                                break;

                            detector.UpdateValue(sm, 1000);

                            if (m_bExitThread == true)
                                break;
                            detector.SendCommand(sm, 1000);
                        }  
                    }
                    else
                    {
                        NetworkManager sm = detector.NetworkMan;
                        if (sm.IsConnected == true)
                        {
                            detector.CheckValue(sm, 1000);

                            if (m_bExitThread == true)
                                break;

                            detector.UpdateValue(sm, 1000);

                            if (m_bExitThread == true)
                                break;
                            detector.SendCommand(sm, 1000);
                            
                            // 통신이 연결되어 있더라도 status가 읽혀지지 않는상태
                            // 랜선빠짐, 중계기 off 등의 변수에 대한 처리
                            if(detector.IsOnline() == false)
                            {
                                sm.ConnectionCheckCount();
                            }

                            // sh
                            if (detector.OtherDetector != null)
                            {
                                detector.OtherDetector.CheckValue(sm, 1000);

                                if (m_bExitThread == true)
                                    break;

                                detector.OtherDetector.UpdateValue(sm, 1000);

                                if (m_bExitThread == true)
                                    break;
                                detector.OtherDetector.SendCommand(sm, 1000);

                                // 통신이 연결되어 있더라도 status가 읽혀지지 않는상태
                                // 랜선빠짐, 중계기 off 등의 변수에 대한 처리
                                if (detector.OtherDetector.IsOnline() == false)
                                {
                                    sm.ConnectionCheckCount();
                                }
                            }
                        }  
#if VALUE_DEBUG
                        if( detector == detectors[0])
                        {
                            if (m_nCount == 10)
                            {
                                detector.SetValue(1, 10.0f);
                                detector.SetDebugUnitStatus(1);
                                detector.UpdateValue(sm, 1000);

                            }

                            if (m_nCount == 20)
                            {
                                detector.SetValue(1, 0.0f);
                                detector.SetDebugUnitStatus(1);
                                detector.UpdateValue(sm, 1000);
                                m_nCount = 0;
                            }

                            m_nCount++;
                        }
#endif

                       
                    }


                    for (int i = 0; i < 10; i++ )
                    {
                        Thread.Sleep(10);
                        if (m_bExitThread == true)
                            break;
                    }
                        
                    nCount++;

                    if(nCount == 1000)
                    {
                        try
                        {
                            nCount = 0;
                            if (m_bExitThread == false)
                                GC.Collect();
                        }
                        catch(Exception)
                        { }
                    }
                }                
            }
        }
#if VALUE_DEBUG
        public void SetStatusClear(int nComm, int nAlram)
        {
            GasDetector detector = FindDetector(nComm);
            if (detector != null)
            {
                detector.SetDebugUnitStatus(0);
            }           
        }
#endif

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
            catch(Exception)
            {
            }

            List<GasDetector> detectors = new List<GasDetector>(m_DetectorList);
            foreach (GasDetector detector in detectors)
            {
                if (detector.TcpMode == false)
                {
                    detector.Serial.StopServer();
                }
                else
                {
                    detector.NetworkMan.StopServer();
                }
                detector.SetOff();
            }
        }

        /// <summary>
        /// 특정 Comm에서 AlarmUnit의 bit 상태를 검사
        /// </summary>
        /// <param name="nComm">Comm Unit ID</param>
        /// <param name="nAlarmUnit">Alarm Unit ID</param>
        /// <param name="nChannel">Bit</param>
        /// <returns>Status (1:Active, 0:Normal -1:Offline)</returns>
        public int GetStatus(int nComm, int nAlarmUnit, int nChannel)
        {
            GasDetector detector = FindDetector(nComm);
            if (detector != null)
            {
                return detector.GetStatus(nAlarmUnit, nChannel);
            }
            return 0;
        }

        /// <summary>
        /// 특정 COMM에서 AlarmUnit의 Value를 가져오기
        /// </summary>
        /// <param name="nComm">COMM ID</param>
        /// <param name="nAlarmUnit">Alarm Unit ID</param>
        /// <returns>정상인경우 0~100사이의 값, 오류인경우 -999.0f</returns>
        public float GetDensity(int nComm, int nAlarmUnit)
        {
            GasDetector detector = FindDetector(nComm);
            if (detector != null)
            {
                return detector.GetValue(nAlarmUnit);
            }
            return -999.0f;
        }

#if VALUE_DEBUG
        public void SetDensity(int nComm, int nAlarmUnit, float fValue)
        {
            GasDetector detector = FindDetector(nComm);
            if (detector != null)
            {
                detector.SetValue(nAlarmUnit, fValue);
            }
            
        }
#endif

        public void SetReset(int nComm)
        {
            GasDetector detector = FindDetector(nComm);
            if (detector != null)
            {
                detector.Reset();
            }
        }

        public bool GetNotify(int nComm, int nAlarmUnit, int nChannel)
        {
            GasDetector detector = FindDetector(nComm);
            if (detector != null)
            {
                return detector.GetChannelNotify(nAlarmUnit, nChannel);
            }
            return false;
        }

        public void SetNotify(int nComm, int nAlarmUnit, int nChannel, bool bEnable)
        {
            GasDetector detector = FindDetector(nComm);
            if (detector != null)
            {
                detector.SetChannelNotify(nAlarmUnit, nChannel, bEnable);
            }
        }

        public void RequestNotify(int nComm)
        {
            GasDetector detector = FindDetector(nComm);
            if (detector != null)
            {
                detector.RequestAlarm();
            }
        }

        /// <summary>
        /// 제어용 Register의 값을 설정한다.
        /// </summary>
        /// <param name="nComm">COMM ID</param>
        /// <param name="nFunction">Function Code</param>
        /// <param name="nChannel">Address</param>
        /// <param name="nValue">Value</param>
        public void SetControlRegister(int nComm, int nFunction, int nChannel, int nValue)
        {
            GasDetector detector = FindDetector(nComm);
            if (detector != null)
            {
                m_nCmdFunc = nFunction;
                detector.AddCommand(nFunction, nChannel, nValue);
            }
        }

        private int m_nCmdFunc = 5;
        internal int CommandFunction            
        {
            get { return m_nCmdFunc; }
            set { m_nCmdFunc = value; }
        }


        internal void OnAlarmNotify(int nComm, int nAlarmUnit, float fValue, int nChannel, int nStatus)
        {
            if (OnNotifyAlarm != null)
            {
                // 이벤트를 비동기로 호출한다.
                Action fire = new Action(() => OnNotifyAlarm(nComm, nAlarmUnit, fValue, nChannel, nStatus));
                fire.BeginInvoke(EndCollback, fire);
            }
        }

        private void EndCollback(IAsyncResult ar)
        {
            try
            {
                ((Action)ar.AsyncState).EndInvoke(ar);
            }
            catch(Exception)
            {
                // 종료되었는지 검사
            }   
        }
        
        private GasDetector FindDetector(int nID)
        {
            foreach(GasDetector detector in m_DetectorList)
            {
                if(detector.ID == nID)
                {
                    return detector;
                }

                if (detector.OtherDetector != null)
                {
                    if (detector.OtherDetector.ID == nID)
                    {
                        return detector.OtherDetector;
                    }
                }
            }
            return null;
        }   
  
        public bool GetOnline(int nDetectID)
        {
            GasDetector dt = FindDetector(nDetectID);
            if( dt != null)
            {
                return dt.IsOnline();
            }
            return false;
        }
    }
}
