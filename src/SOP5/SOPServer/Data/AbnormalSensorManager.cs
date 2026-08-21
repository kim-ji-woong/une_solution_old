
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using DBUtility;


namespace SDMSServer
{
    public class AbnormalSensorManager : IDisposable
    {
        protected static AbnormalSensorManager instance = null;
        public static AbnormalSensorManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AbnormalSensorManager();
                    instance.Start();
                }
                return instance;
            }
        }

		private int m_nDetectPolicy = -1;
		public int DetectPolicy
		{
			get { return m_nDetectPolicy; }
		}

		private int m_nDuration = 0;
		public int Duration
		{
			get { return m_nDuration; }
		}

        private ArrayList mAbnormalQueue = new ArrayList();
		public System.Collections.ArrayList AbnormalSensorList
		{
			get 
			{
				ArrayList arList = null;

                DdMonitor.Enter(mAbnormalQueue);
				arList = (ArrayList)mAbnormalQueue.Clone();
                //DdMonitor.Enter(mAbnormalQueue);
                DdMonitor.Exit(mAbnormalQueue);
				return arList;
			}
		}

        private Thread mProcessThread = null;
		private int mSleepTime = 2000;
        public int SleepTime
        {
            get { return mSleepTime; }
            set { mSleepTime = value; }
        }
        private bool mbProcess = true;

		private WebDBManager dbManager = null;

        private int m_nSiteID = 1;
		private AbnormalSensorManager()
		{
            m_nSiteID = NetworkServer.Instance.SiteID;

			dbManager = NetworkServer.Instance.DBManager;
		}
        public void Dispose()
        {
            Stop();
            Clear();
        }

        public bool Progress
        {
            get { return mbProcess; }
            set
            {
                mbProcess = value;
                if (mbProcess == false)
                {
                    mSleepTime = -1;
                }
                else
                {
                    mSleepTime = 2000;
                }
            }
        }           

        private void Start()
        {
            mProcessThread = new Thread(CheckSensor);
            mProcessThread.Name = "AbnormalSensorThread";
            mProcessThread.Start();
        }

        private void Stop()
        {
            mbProcess = false;
            mProcessThread.Join();
        }
		
		private ArrayList arDeleteList = new ArrayList();
        
		private void CheckSensor()
        {
            while (mbProcess == true)
            {            
                try
                {
                    // DB설정 값을 검사 - 대기시간을 가져온다.
				LoadData();

                }catch(Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    System.Diagnostics.Trace.WriteLine(ex.StackTrace);

                }
				
				// 대기 큐를 확인
				if (mAbnormalQueue.Count > 0)
				{
					if (m_nDetectPolicy != 0 && m_nDetectPolicy != 4)
					{
						arDeleteList.Clear();
						DateTime dtNow = DateTime.Now;

                        DdMonitor.Enter(mAbnormalQueue); 
                       
						// id와 대기 시작 시간을 읽는다.						
						foreach (AbnormalSensor sensor in mAbnormalQueue)
						{
							// 대기시간을 넘었으면 무시센서에서 없앤다.
							TimeSpan span = dtNow - sensor.Time;
							if (span.TotalSeconds > m_nDuration)
							{
								arDeleteList.Add(sensor);
							}
						}

						foreach (AbnormalSensor sensor in arDeleteList)
						{
							mAbnormalQueue.Remove(sensor);
						}
						arDeleteList.Clear();

						DdMonitor.Exit(mAbnormalQueue);
					}
					if (m_nDetectPolicy == 0)
					{
                        DdMonitor.Enter(mAbnormalQueue, true); 
					    {
							mAbnormalQueue.Clear();
						}
                        DdMonitor.Exit(mAbnormalQueue);
					}
                }                    
                // 선점 방지 재우기
                Thread.Sleep(mSleepTime);
            }
        }


		private void LoadData()
		{
            //System.Diagnostics.Trace.WriteLine("LoadData");
            string szSQL = "SELECT PropertyValue FROM OptionSDMS where PropertyName='AbnormalSensorDetectPolicy' and SiteID = " + m_nSiteID.ToString();
			ArrayList arResult = dbManager.GetResultData(szSQL, 0);
			if (arResult == null || arResult.Count == 0)
			{
				m_nDetectPolicy = 0;
			}
			else
			{
				m_nDetectPolicy = WebDBManager.GetIntField(arResult[0].ToString(), -1);
			}

            string szSQL2 = "SELECT PropertyValue FROM OptionSDMS where PropertyName='IgnoreDurate' and SiteID = " + m_nSiteID.ToString();
			ArrayList arResult2 = dbManager.GetResultData(szSQL2, 0);
			if (arResult2 == null || arResult2.Count == 0)
			{
				m_nDuration = 0;
			}
			else
			{
				m_nDuration = WebDBManager.GetIntField(arResult2[0].ToString(), -1);
			}
		}

		
	
		public bool Exist(int nSensorID)
		{
			// 모두 무시하지 않는 정책인 경우 무조건 없는걸로
			if (m_nDetectPolicy == 0)
				return false;

            bool bResult = false;
            DdMonitor.Enter(mAbnormalQueue, true);

            foreach (AbnormalSensor sensor in mAbnormalQueue)
            {
                if (nSensorID == sensor.SensorID)
                {
                    bResult = true;
                    break;
                }
            }
            DdMonitor.Exit(mAbnormalQueue, true);

            return bResult;
		}

		public void AddFirst(int nSensorID)
        {
			if (m_nDetectPolicy == 0)
				return;
			
			AbnormalSensor sensor = new AbnormalSensor(nSensorID);

            DdMonitor.Enter(mAbnormalQueue, true);

            mAbnormalQueue.Insert(0, sensor);

            DdMonitor.Exit(mAbnormalQueue, true);
        }

		public bool Remove(int nSensorID)
		{
			AbnormalSensor target = null;
			foreach (AbnormalSensor sensor in mAbnormalQueue)
			{
				if (nSensorID == sensor.SensorID)
				{
					target = sensor;
					break;
				}
			}

			if (target != null)
			{
				DdMonitor.Enter(mAbnormalQueue, true);
                
                mAbnormalQueue.Remove(target);

                DdMonitor.Exit(mAbnormalQueue, true);

				return true;
			}
			return false;
		}
				
		public void Add(int nSensorID)
        {
			if (m_nDetectPolicy == 0)
				return;

			AbnormalSensor sensor = new AbnormalSensor(nSensorID);

            DdMonitor.Enter(mAbnormalQueue, true);
			mAbnormalQueue.Add(sensor);
            DdMonitor.Exit(mAbnormalQueue, true);                
        }

        public void Clear()
        {
			mAbnormalQueue.Clear();
        }
    }	
}
