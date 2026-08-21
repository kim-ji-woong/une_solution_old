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
                    m_SerialManager = new SerialManager(this, conf);                    
                    DetectorFactory factory = new DetectorFactory(this, conf);
                    m_DetectorList.AddRange(factory.CreateDetectorList());
                }
            }
        }

        public bool Start()
        {
            bool bResult = false;
            // 통신 체크 전에 시리얼 서버를 먼저 연결한다.
            if(m_SerialManager != null)
            {
                bResult = m_SerialManager.BeginServer();
            }

            // 통신이 연결되었을 경우만 Thread를 실행한다.
            if (bResult == true)
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
            return bResult;
        }

        private void CheckDetector()
        {
            int nCount = 0;
            while(!m_bExitThread)
            {
                List<GasDetector> detectors = new List<GasDetector>(m_DetectorList);
                foreach(GasDetector detector in detectors)
                {
                    
                    detector.CheckValue(m_SerialManager, 1000);

                    if (m_bExitThread == true)
                        break;

                    detector.UpdateValue(m_SerialManager, 1000);

                    if (m_bExitThread == true)
                        break;
                    detector.SendCommand(m_SerialManager, 1000);



                    nCount++;

                    if(nCount == 100)
                    {
                        try
                        {
                            nCount = 0;
                            GC.Collect();
                        }
                        catch(Exception)
                        { }
                    }
                }                
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
            catch(Exception)
            {
            }

            // 시리얼 통신을 해제
            if (m_SerialManager != null)
            {
                m_SerialManager.StopServer();
            }

            List<GasDetector> detectors = new List<GasDetector>(m_DetectorList);
            foreach (GasDetector detector in detectors)
            {
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
            }
            return null;
        }     
    }
}
