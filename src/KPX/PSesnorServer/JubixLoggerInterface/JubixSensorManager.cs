using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;
using DBUtility;

namespace JubixNetwork
{ 
    public delegate void AlarmNotifyDelegate(int nComm, int nAlarmUnit, float fValue, int nChannel, int nStatus);

    public class JubixSensorManager : IDisposable
    {
        public event AlarmNotifyDelegate OnNotifyAlarm;

        private static JubixSensorManager m_Instance = null;
        public static JubixSensorManager Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new JubixSensorManager();
                return JubixSensorManager.m_Instance;
            }
        }
      
        /// <summary>
        /// INI에서 로딩된 센서 리스트
        /// </summary>
        private List<PipeSensor> m_SensorList = new List<PipeSensor>();
        public List<PipeSensor> SensorList
        {
            get { return m_SensorList; }
        }

        private SortedList<int, PipeSensor> m_dicPipeList = new SortedList<int, PipeSensor>();
        public SortedList<int, PipeSensor> DicPipeList
        {
            get { return m_dicPipeList; }
        }
        
        private int m_nSiteID = 500;
        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }
        
        public JubixSensorManager()
        {
            ReadSiteID();

            m_dbMgr = new WebDBManager(m_nSiteID);

            ReadPipe();
        }

        public void ReadSiteID()
        {
            DBUtility.Utility iniFile = new DBUtility.Utility("KPXConfig.ini");
            string szSiteID = iniFile.getinivalue("Server Connection Info", "siteid");

            int nSiteID = 500;
            if (szSiteID.Length > 0)
            {
                int.TryParse(szSiteID, out nSiteID);

            }
            m_nSiteID = nSiteID;
        }

        private WebDBManager m_dbMgr = null;
        public WebDBManager DBManager
        {
            get { return m_dbMgr; }
        }

        public void Dispose()
        {
        }

        public void ReadPipe()
        {
            // get pipe data
            // 10, 11은 파라곤에서 가져옴
            string szSQL = "SELECT ID, Name, NormalBeginRange, NormalEndRange, Status FROM Pipe";
            ArrayList arResult = m_dbMgr.GetResultData(szSQL, 0);
            if (arResult != null && arResult.Count > 0)
            {
                for (int i = 0; i < (arResult.Count - 4); i += 5)
                {
                    int nID = WebDBManager.GetIntField(arResult[i].ToString(), -1);
                    string szName = WebDBManager.GetStringField(arResult[i + 1], "");
                    float fNormalRangeLower = WebDBManager.GetFloatField(arResult[i + 2].ToString(), 0.0f);
                    float fNormalRangeUpper = WebDBManager.GetFloatField(arResult[i + 3].ToString(), 0.0f);
                    int nStatus = WebDBManager.GetIntField(arResult[i + 4].ToString(), -1);

                    PipeSensor pipe = new PipeSensor();

                    // save pipe value
                    pipe.PipeID = nID;
                    pipe.PipeName = szName;
                    pipe.NoramlValueUnder = fNormalRangeLower;
                    pipe.NormalValueUpper = fNormalRangeUpper;
                    pipe.Status = nStatus;
                    
                    m_SensorList.Add(pipe);

                    m_dicPipeList.Add(nID, pipe);
                }
            }
        }

        // 함체 버튼 상태는 직접 DB를 업데이트 해준다.
        private void UpdatePushButtonStatus(int nState)
        {
            string szTemp = "UPDATE options SET PropertyValue = {0} WHERE PropertyName ='ButtonStatus'";
            string szSQL = string.Format(szTemp, nState);
            m_dbMgr.GetResultData(szSQL, 0);
        }

        // 경광등 상태는 직접 DB를 업데이트 해준다.
        private void UpdateSirenStatus(int nState)
        {
            string szTemp = "UPDATE options SET PropertyValue = {0} WHERE PropertyName ='SirenStatus'";
            string szSQL = string.Format(szTemp, nState);
            m_dbMgr.GetResultData(szSQL, 0);
        }

        private int m_nWarnLight = 0;
        public int WarnLight
        {
            get { return m_nWarnLight; }
        }

        private int m_nPushButton = 0;
        public int PushButton
        {
            get { return m_nPushButton; }
        }


        private bool m_bSimulation = false;
        public void SimulationMode(bool bSet)
        {
            if (bSet == true)
            {
                m_bSimulation = true;
            }
            else
            {
                m_bSimulation = false;
            }
        }

        // 네트워
        public void ProcessMessage(JubixMessage msg)
        {
            if (m_bSimulation == true)
                return;

            try
            {
                ArrayList arData = new ArrayList();
                arData.AddRange(msg.DataList);
                System.Diagnostics.Trace.WriteLine("SensorTime : " + msg.GetTimeString());

                for (int i = 0; i < m_dicPipeList.Values.Count; i++)
                {
                    PipeSensor sensor = m_dicPipeList.Values[0];
                    sensor.LastAlarm = null;
                }               

                for (int i = 0; i < m_SensorList.Count; i++)
                {
                    // 10, 11 배관은 Paragon Data 임
                    if (m_SensorList[i].PipeID == 10 || m_SensorList[i].PipeID == 11)
                        continue;

                    // 마지막 데이터 2개는 경광등 데이터므로 2개작게 계산
                    if (i < (arData.Count - 2))
                    {
                        short f = (short)arData[i];
                        if (f != -9999.0)
                        {                           
                            m_SensorList[i].SetSensorValue((float)f);
                        }
                        else
                        {
                            //m_SensorList[i].SetSensorValue(0);
                        }
                    }
                }

                if (m_SensorList.Count > 1)
                {
                    // 경광등 데이터 ( 1 켜짐 , 0 꺼짐 )
                    short s = (short)arData[9];
                    int nData = (int)(s > 0 ? 1 : 0);

                    if (m_nWarnLight != nData)
                    {
                        UpdateSirenStatus(nData);
                    }

                    // 스위치데이터 ( 0 눌러짐 , 1 안눌러짐)
                    short s2 = (short)arData[10];
                    int nData2 = (int)(s2 > 0 ? 1 : 0);

                    if (m_nPushButton != nData2)
                    {
                        UpdatePushButtonStatus(nData2);
                    }
                    m_nPushButton = nData2;
                    m_nWarnLight = nData;
                }

                //m_bChangedData = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
            }
        }

     
        /// <summary>
        /// 특정 Sensor의 Value를 가져오기
        /// </summary>
        /// <param name="nPipe">SensorID ID</param>
        /// <returns>정상인경우 0~100사이의 값, 오류인경우 -9999.0f</returns>
        public float GetValue(int nPipe)
        {
            if (nPipe < 0)
                return -9999;

            PipeSensor detector = FindSensor(nPipe);
            if (detector != null)
            {
                return detector.CurrentValue;
            }
            return -9999.0f;
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
            catch (Exception)
            {
                // 종료되었는지 검사
            }
        }

        private PipeSensor FindSensor(int nID)
        {
            foreach (PipeSensor detector in m_SensorList)
            {
                if (detector.PipeID == nID)
                {
                    return detector;
                }
            }
            return null;
        }

    }
}