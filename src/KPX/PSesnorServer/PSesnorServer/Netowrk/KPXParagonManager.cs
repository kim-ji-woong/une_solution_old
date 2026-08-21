using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using DBUtility;
using ParagonNetwork;
using JubixNetwork;

namespace PSensorServer.Netowrk
{
    public class KPXParagonManager
    {
        private int m_nSiteID = 500;
        private DBUtility.WebDBManager mDBMgr = null;
        private Thread m_LevelCheckThread = null;

        private ParagonManager pm = new ParagonManager();
   
        private static KPXParagonManager m_instance = null;
        public static KPXParagonManager Instance
        {
            get { return m_instance; }
        }

        public KPXParagonManager()
        {
            mDBMgr = new WebDBManager(500);
            m_instance = this;
        }


        private Action<int, int, float, int, int> mNotifyAction = null;
        public void BeginServer(Action<int, int, float, int, int> onNotify)
        {
            try
            {
                mNotifyAction = onNotify;
                pm.OnNotifyAlarm += ParagonPipe_OnNotifyAlarm;
                pm.Start();

            }
            catch (Exception)
            { }

            //ReadPipe();

            m_LevelCheckThread = new Thread(CheckValue);
            m_LevelCheckThread.Name = "ParagonPipe value check";
            m_LevelCheckThread.Start();
        }

        private void ParagonPipe_OnNotifyAlarm(int nComm, int nAlarmUnit, float fValue, int nChannel, int nStatus)
        {
            if (mNotifyAction != null)
            {
                mNotifyAction.Invoke(nComm, nAlarmUnit, fValue, nChannel, nStatus);
            }
        }

        private bool m_bSimulation = false;
        public bool Simulation
        {
            get { return m_bSimulation; }
        }
            
        internal void SimulationMode(bool bSet)
        {
            m_bSimulation = bSet;
        }
        
        public void StopServer()
        {
            try
            {
                m_bReleaseThread = true;
                if (m_LevelCheckThread != null)
                    m_LevelCheckThread.Join(2000);
                m_LevelCheckThread = null;
            }
            catch (Exception)
            { }

            pm.OnNotifyAlarm -= ParagonPipe_OnNotifyAlarm;
            pm.End();

            mNotifyAction = null;
        }
        
        //private List<PipeSensor> m_PipeList = new List<PipeSensor>();
        //public List<PipeSensor> PipeList
        //{
        //    get { return m_PipeList; }
        //}

        // Tank ID별 탱크정보
        //private SortedList<int, PipeSensor> m_dicPipeList = new SortedList<int, PipeSensor>();
        //public SortedList<int, PipeSensor> DicPipeList
        //{
        //    get { return m_dicPipeList; }
        //}
        
        public PipeSensor FindTank(int nPipeID)
        {
            PipeSensor info = null;
            //m_dicPipeList.TryGetValue(nPipeID, out info);
            return info;
        }

        //public void ReadPipe()
        //{
        //    // get pipe data
        //    string szSQL = "SELECT ID, Name, NormalBeginRange, NormalEndRange, Status FROM Pipe WHERE id in (10, 11)";
        //    ArrayList arResult = mDBMgr.GetResultData(szSQL, 0);
        //    if (arResult != null && arResult.Count > 0)
        //    {
        //        for (int i = 0; i < (arResult.Count - 4); i += 5)
        //        {
        //            int nID = WebDBManager.GetIntField(arResult[i].ToString(), -1);
        //            string szName = WebDBManager.GetStringField(arResult[i + 1], "");
        //            float fNormalRangeLower = WebDBManager.GetFloatField(arResult[i + 2].ToString(), 0.0f);
        //            float fNormalRangeUpper = WebDBManager.GetFloatField(arResult[i + 3].ToString(), 0.0f);
        //            int nStatus = WebDBManager.GetIntField(arResult[i + 4].ToString(), -1);

        //            PipeSensor pipe = new PipeSensor();

        //            // save pipe value
        //            pipe.PipeID = nID;
        //            pipe.PipeName = szName;
        //            pipe.NoramlValueUnder = fNormalRangeLower;
        //            pipe.NormalValueUpper = fNormalRangeUpper;
        //            pipe.Status = nStatus;

        //            JubixNetwork.JubixSensorManager.Instance.SensorList.Add(pipe);
        //            JubixNetwork.JubixSensorManager.Instance.DicPipeList.Add(nID, pipe);
        //            //m_PipeList.Add(pipe);
        //            //m_dicPipeList.Add(nID, pipe);
        //        }
        //    }
        //}

        private PipeSensor GetPipe(int nPipeID)
        {
            PipeSensor info = null;
            //m_dicPipeList.TryGetValue(nPipeID, out info);
            return info;
        }

        private bool m_bReleaseThread = false;
        
        // PLC모든 번지에 대해 데이터 입력이 1회이상 완료된 경우 true가 된다.
        // 정상적인 데이터가 입력되어 체크가 가능한 시점에 true가 된다.
        private bool m_bDataEnabled = false;
        public bool DataEnabled
        {
            get { return m_bDataEnabled; }
            set { m_bDataEnabled = value; }
        }

        private void CheckValue()
        {
            int nCount = 0;
           
            while (!m_bReleaseThread)
            {
                int nUnitID = 1;
                bool bOnline = pm.GetOnline(nUnitID);
                for (int i = 0; i < JubixNetwork.JubixSensorManager.Instance.SensorList.Count; i++)
                {
                    nUnitID = 1;
                    PipeSensor info = (PipeSensor)JubixNetwork.JubixSensorManager.Instance.SensorList[i];

                    if (info.PipeID != 10 && info.PipeID != 11)
                        continue;

                    int nDelta = 0;                    
                    if (info.PipeID == 11)
                        nDelta = 1;

                    float fPressure = pm.GetLevel(nUnitID, nDelta);

                    if (m_bSimulation == false)
                    {
                        if (fPressure != -9999.0)
                        {
                            // SetSensorValue 에서 나누기 100을 하기때문에 곱해서 넘겨줌
                            info.SetSensorValue(fPressure * 100);
                        }
                    }                    

                    if (m_bDataEnabled == true)
                    {
                        //SaveTankHistory(info);
                        //CheckAlarm(info);
                    }
                    
                    if (m_bReleaseThread == true)
                        break;
                }

                if (nCount == 20)
                    m_bDataEnabled = true;
                else
                    nCount++;

                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(100);
                    if (m_bReleaseThread == true)
                        break;
                }
            }
        }    
    }   
}
