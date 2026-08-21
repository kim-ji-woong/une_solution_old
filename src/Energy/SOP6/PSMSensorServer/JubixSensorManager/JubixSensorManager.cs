using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;
using DBUtility2;
using DBUtility;

namespace JubixSensor
{
    public delegate void AlarmNotifyDelegate(int nComm, int nAlarmUnit, float fValue, int nChannel, int nStatus,int windDirection,int windSpeed);
    public delegate void FireSensorNotifyDelegate(int sensorType, int sensorTagID);       //화재 센서 알람용

    public class JubixSensorManager
    {
        public event AlarmNotifyDelegate OnNotifyAlarm;
        public event FireSensorNotifyDelegate OnNotifyFireAlarm;

        private int m_nSiteID = 3;
        private LocalDBManager m_dbJubix = null;
        private WebDBManager mSOPDB = null;
        private Thread m_ValueCheckThread = null;

        
        private Utility m_ini = new Utility();
        public JubixSensorManager(int nSiteID)
        {
            m_nSiteID = nSiteID;

            string strSection = "Jubix Connection Info";
            string strServerIP = m_ini.getinivalue(strSection, "server_ip");
            string strServerPort = m_ini.getinivalue(strSection, "server_port");
            string strServerDB = m_ini.getinivalue(strSection, "server_db");

            m_dbJubix = new LocalDBManager(strServerIP, strServerDB, "mysql", m_nSiteID);
            //mDBMgr.DatabaseHost = strServerIP;
            //mDBMgr.WebServerURL = "http://127.0.0.1:8080/JUBIX";
            //mDBMgr.DatabaseType = WebDBManager.DBType.mysql;
            //mDBMgr.DatabaseName = strServerDB;
            //mDBMgr.DatabasePort = strServerPort;

            mSOPDB = new WebDBManager(nSiteID);

            ReadJubixSensorInfo();
            
            alarmManager.ReadAlarmInfo(m_dbJubix);
        }

        public void End()
        {
            m_bExitThread = true;

            try
            {
                m_ValueCheckThread.Abort();
                m_ValueCheckThread.Join();                
            }
            catch(Exception)
            { }
        }

        public void Start()
        {
            m_bExitThread = false;

            m_ValueCheckThread = new Thread(CheckSensor);
            m_ValueCheckThread.Name = "JubixValueCheckThread";
            m_ValueCheckThread.Start();
        }

        public bool GetOnline(int p)
        {
            return true;
        }        

        //private SortedList<int, JubixSensor> mSensorList = new SortedList<int, JubixSensor>();
        private SortedList<int, SortedList<int, JubixSensor>> mSensorList = new SortedList<int, SortedList<int, JubixSensor>>();
        private ArrayList m_SensorList = new ArrayList();
        private void ReadJubixSensorInfo()
        {
            // ss_Stat가 '00'이면 센서상태 정상. '01'이면 비가동
            string szTemp = "SELECT ss_ID FROM c_ss_info where ss_Stat = '00' and (ss_Knd = '01' or ss_Knd = '02' or ss_Knd = '03' or ss_Knd = '15')";

            ArrayList arResult = m_dbJubix.GetResultData(szTemp, 0);
            if( arResult != null && arResult.Count > 0)
            {
                Dictionary<string, JubixSensor> dicJubixPSMSensors = new Dictionary<string, JubixSensor>();
                
                for(int i = 0 ; i <  arResult.Count; i++)
                {
                    string szSensorName = arResult[i].ToString();                   
                    JubixSensor sensor = new JubixSensor();
                    sensor.SensorName = szSensorName;
                    dicJubixPSMSensors[szSensorName] = sensor;
                }

                // Sensor 로딩 속도를 빠르게 하기 위하여 한번만 DB 호출을 하도록 변경                
                ReadSensorInfo(dicJubixPSMSensors);
            }
        }

        private void ReadSensorInfo(Dictionary<string, JubixSensor> dicJubixSensors)
        {
            string szSQL = "SELECT ID, SensorServerID, TagNo, SensorName, SensorType FROM sensortaginfo where SensorServerID = 1";
            ArrayList arResult = mSOPDB.GetResultData(szSQL);

            if (arResult == null)
                return;

            JubixSensor sensor = null;
            int nResultCount = arResult.Count;

            for (int i = 0; i < nResultCount - 4;i+=5 )
            {
                int nID = WebDBManager.GetIntField(arResult[i].ToString(), -1);
                int nServerID = WebDBManager.GetIntField(arResult[i + 1].ToString(), -1);
                int nTagNo = WebDBManager.GetIntField(arResult[i + 2].ToString(), -1);
                string strSensorName = WebDBManager.GetStringField(arResult[i + 3], "");                
                int sensorType = WebDBManager.GetIntField(arResult[i + 4].ToString(), -1);
                
                if (dicJubixSensors.TryGetValue(strSensorName, out sensor) == false)
                    continue;

                sensor.ID = nID;
                sensor.ServerID = nServerID;
                sensor.TagNo = nTagNo;                
                sensor.SensorType = sensorType;
                
                if (sensor.ServerID > 0)
                {
                    SortedList<int, JubixSensor> sensorList = null;
                    if (!mSensorList.TryGetValue(sensor.ServerID, out sensorList))
                    {
                        sensorList = new SortedList<int, JubixSensor>();
                        mSensorList.Add(sensor.ServerID, sensorList);
                    }
                    sensorList.Add(sensor.ID, sensor);
                    m_SensorList.Add(sensor);
                }

                //System.Diagnostics.Trace.WriteLine(szSQL);
            }
        }

        /*
        public void ReadSensorInfo(JubixSensor sensor)
        {
            string szTemp = "SELECT ID, SensorServerID, TagNo FROM sensortaginfo where SensorType = 11 and SensorName like '{0}'";
            string szSQL = string.Format(szTemp, sensor.SensorName);
            ArrayList arResult = mSOPDB.GetResultData(szSQL);
            if( arResult != null && arResult.Count > 0)
            {
                int nID = WebDBManager.GetIntField(arResult[0].ToString(), -1);
                int nServerID = WebDBManager.GetIntField(arResult[1].ToString(), -1);
                int nTagNo = WebDBManager.GetIntField(arResult[2].ToString(), -1);

                sensor.ID = nTagNo;
                sensor.ServerID = nServerID;
                sensor.TagNo = nTagNo;

                System.Diagnostics.Trace.WriteLine(szSQL);
            }
        }
        */

        private bool m_bExitThread = false;
        private int m_nSleepTime = 2000;

        public void CheckSensor()
        {
            while(!m_bExitThread)
            {
                foreach (JubixSensor sensor in m_SensorList)
                {
                    // Get sensor value
                    float fValue = ReadDensity(sensor.SensorName);

                    sensor.Density = fValue;

                    int nStatus = ReadStatus(sensor.SensorName);

                    sensor.SetStatus(nStatus);
                   
                    if( sensor.FireNotify == true)
                    {
                        int nNotifyStatus = (sensor.Status == 0 ? 0 : 1);       
                        //여기서 ss_info의 ss_knd (센서 종류 : 암모니아)를 읽어야 한다. 
                        //direction 0 : N, 1 : NE, E : 2, 3 : SE, 4 : S, 5 : SW, 6 : W, 7 : NW
                        int windDir = -1;
                        int windSpd = -1;

                        if (IsNH3Sensor(sensor.SensorName))
                        {
                            float[] windFactors = ReadWeatherInfo();
                            if (windFactors != null && windFactors.Length >= 2) 
                            {
                                windDir = 0;
                                windSpd = 0;
                                if ((windFactors[0] >= 0.0 && windFactors[0] < 22.5) || (windFactors[0] >= 337.5 && windFactors[0] <= 360.0))
                                {
                                    windDir = 0;
                                }
                                else if (windFactors[0] >= 22.5 && windFactors[0] < 67.5)
                                {
                                    windDir = 1;
                                }
                                else if (windFactors[0] >= 67.5 && windFactors[0] < 112.5)
                                {
                                    windDir = 2;
                                }
                                else if (windFactors[0] >= 112.5 && windFactors[0] < 157.5)
                                {
                                    windDir = 3;
                                }
                                else if (windFactors[0] >= 157.5 && windFactors[0] < 202.5)
                                {
                                    windDir = 4;
                                }
                                else if (windFactors[0] >= 202.5 && windFactors[0] < 247.5)
                                {
                                    windDir = 5;
                                }
                                else if (windFactors[0] >= 247.5 && windFactors[0] < 292.5)
                                {
                                    windDir = 6;
                                }
                                else if (windFactors[0] >= 292.5 && windFactors[0] < 337.5)
                                {
                                    windDir = 7;
                                }

                                /*
                                   direction 0 : N, 1 : NE, E : 2, 3 : SE, 4 : S, 5 : SW, 6 : W, 7 : NW
                                   windStrength 2 : 오염도 약함(M:바람강함),  0: 오염도 강함(X:바람약함)        
                                   */
                                if (windFactors[1] >= 2.7) 
                                    windSpd = 2;
                                else
                                    windSpd = 0;        //바람 약함
                                windDir = 2;        //시연용 하드코드
                                windSpd = 0;        //시연용 하드코드
                            }
                            if (OnNotifyAlarm != null)
                                OnNotifyAlarm(sensor.ServerID, sensor.ID, fValue, 0, nNotifyStatus, windDir, windSpd);
                        }
                        else if (IsFireSensor(sensor.SensorType))
                        {
                            if (OnNotifyFireAlarm != null)
                                OnNotifyFireAlarm(sensor.SensorType, sensor.ID);
                        }
                        else
                        {
                            if (OnNotifyAlarm != null)
                                OnNotifyAlarm(sensor.ServerID, sensor.ID, fValue, 0, nNotifyStatus, windDir, windSpd);
                        }
                        
                    }

                    sensor.FireNotify = false;
                }

                int nSleepTime = m_nSleepTime / 100;
                for( int i  = 0 ; i < nSleepTime; i++)
                {
                    if (m_bExitThread == true)
                        break;
                    System.Threading.Thread.Sleep(100);
                }
            }
        }
        //암모니아 센서 종류 파악
        public bool IsNH3Sensor(string sensorName)
        {
            string szTemp = "SELECT ss_Knd from c_ss_info WHERE ss_ID = '{0}'";
            string szSQL = string.Format(szTemp, sensorName);
            ArrayList arResult = m_dbJubix.GetResultData(szSQL, 0);
            if (arResult != null && arResult.Count > 0)
            {                
                string kindOf = LocalDBManager.GetStringField(arResult[0].ToString(),"00");
                if(kindOf != null && kindOf.Equals("01")) return true;               
            }
            return false;
        }

        public bool IsFireSensor(int sensorType)
        {
            if (sensorType > 100 && sensorType <= 110) return true;     //화재센서 101~110
            return false;
        }

        //wind info 읽기
        private string szAwsID = "A1000";
        private float[] ReadWeatherInfo()
        {
            string szTemp = "SELECT Aws_wind, Aws_wspd FROM r_aws_dat where Aws_ID = '{0}' order by Aws_date DESC limit 1";
            string szSQL = string.Format(szTemp, szAwsID);
            ArrayList arResult = m_dbJubix.GetResultData(szSQL, 0);          
           
            if (arResult != null && arResult.Count > 0)
            {
                float[] result = new float[2];
                result[0] = LocalDBManager.GetFloatField(arResult[0].ToString(), -9999);
                result[1] = LocalDBManager.GetFloatField(arResult[1].ToString(), -9999);

                return result;
            }
            return null;
        }
      
        public float ReadDensity(string szSensorName)
        {
            string szTemp = "SELECT ss_Cur_Value, ss_Cur_Date FROM c_ss_info WHERE ss_ID = '{0}'";
            string szSQL = string.Format(szTemp, szSensorName);
            ArrayList arResult = m_dbJubix.GetResultData(szSQL, 0);
            if (arResult != null && arResult.Count > 0)
            {
                for (int i = 0; i < arResult.Count - 1; i += 2)
                {
                    float value = LocalDBManager.GetFloatField(arResult[0].ToString(), -999.0f);

                    string strDate = arResult[1].ToString();
                    try
                    {
                        //DateTime dtDate = DateTime.ParseExact(strDate, "yyyyMMddHHmmss", null);
                        //if (CheckTime(dtDate))
                        {
                            return value;
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            return -9999.0f;
        }

        public float GetDensity(int nUnitID, int nSensorID)
        {           
            SortedList<int, JubixSensor> sensorList = null;               
            if (mSensorList.TryGetValue(nUnitID, out sensorList))
            {
                JubixSensor sensor = null;
                if (sensorList.TryGetValue(nSensorID, out sensor))
                {
                    return sensor.Density;
                }
            }                        
            return 0.0f;
        }

        private int mValidateTime = 12000000;//second
        private bool CheckTime(DateTime dt)
        {
            DateTime dtNow = DateTime.Now;
            if (dt != null)
            {
                TimeSpan span = dtNow - dt;
                if (span.TotalSeconds < mValidateTime)
                {
                    return true;
                }
            }
            return false;
        }

        private int ReadStatus(string szSensorName)
        {
            string szTemp = "SELECT ss_Cur_Stat, ss_Cur_Date FROM c_ss_info WHERE ss_ID = '{0}'";
            string szSQL = string.Format(szTemp, szSensorName);
            ArrayList arResult = m_dbJubix.GetResultData(szSQL, 0);
            if (arResult != null && arResult.Count > 0)
            {
                for (int i = 0; i < arResult.Count - 1; i += 2)
                {
                    int value = LocalDBManager.GetIntField(arResult[0].ToString(), 0);
                    string strDate = arResult[1].ToString();
                    try
                    {
                        //DateTime dtDate = DateTime.ParseExact(strDate, "yyyyMMddHHmmss", null);
                        //if (CheckTime(dtDate))
                        {
                            return value;
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            return -1;
        }
        
        public int GetStatus(int nUnitID, int nSensorID, int nLevel)
        {
            SortedList<int, JubixSensor> sensorList = null;
            if (mSensorList.TryGetValue(nUnitID, out sensorList))
            {
                JubixSensor sensor = null;
                if (sensorList.TryGetValue(nSensorID, out sensor))
                {
                    int nStatus = sensor.Status;
                    if (nStatus == 1 || nStatus == 2 || nStatus == 3)
                    {
                        if ((nLevel+1) <= nStatus)
                            return 1;
                    }
                }
            } 
            return 0;
        }

        public void ResetSensor(int nSensorID)
        {
            foreach (JubixSensor sensor in m_SensorList)
            {
                if (sensor.ID == nSensorID)
                {
                    SetResetJubixDB(sensor.SensorName);
                    break;  
                }
            }
        }

        private AlarmManager alarmManager = new AlarmManager();
        public void ReadAlarmStatus()
        {
            alarmManager.ReadAlarmInfo(m_dbJubix);
        }
        public bool ReadAlarm(int n)
        {
            if (n == 1)
            {
                return (alarmManager.AlarmStatus1 != "정상");                 
            }
            else if (n == 2)
            {
                return (alarmManager.AlarmStatus2 != "정상");  
            }
            return false;
        }
        public void SetAlarm(int n, bool bAlarm)
        {
            if(n == 1)
            {
                // 중앙제어실
                alarmManager.SetAlarmContorlRoom(bAlarm);                
            }
            else if(n == 2)
            {
                // 수처리실
                alarmManager.SetAlarmWaterRoom(bAlarm);
            }
        }
        public void SetAlarm( bool bAlarm)
        {
            alarmManager.SetAlarmContorlRoom(bAlarm);
            alarmManager.SetAlarmWaterRoom(bAlarm);
        }

        public void SetReset(int nUnit)
        {
            foreach (JubixSensor sensor in m_SensorList)
            {
                SetResetJubixDB(sensor.SensorName);

                //sensor.FireNotify = false;
                //sensor.Status = 0;               
            }


            SetAlarm(false);

        }
        private string MakeSimpleDateTimeString(DateTime time)
        {
            return string.Format("{0}{1:00}{2:00}{3:00}{4:00}{5:00}", time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);
        }

        private string MakeSimpleYearDateTimeString(DateTime time)
        {
            return string.Format("{0}{1:00}{2:00}", time.Year, time.Month, time.Day);
        }

        public void SetResetJubixDB(string szSensorName)
        {
            DateTime dt = DateTime.Now;
            string szDate = MakeSimpleDateTimeString(dt);
            string szDate2 = WebDBManager.MakeDateTimeString(dt);
            string szTemp = "INSERT INTO r_ss_dat (ss_ID, ss_date, ss_Stat, ss_Value, ss_Bigo, Crte_User, Crte_Dttm, Mdfy_User, Mdfy_Dttm) " +
                "VALUES ('{0}','{1}', '00', '0', '훈련시스템입력', 'ETMGR', '{2}', 'ETMGR', '{3}')";

            string szSQL = string.Format(szTemp, szSensorName, szDate, szDate2, szDate2);
            m_dbJubix.GetResultData(szSQL, 0);
        }

        private void SetTestAlarmJubixDB(string szSensorName)
        {
            float fValue = 30;
            if(szSensorName.StartsWith("H"))
            {
                fValue = 3;
            }

            DateTime dt = DateTime.Now;
            string szDate = MakeSimpleDateTimeString(dt);
            string szDate2 = WebDBManager.MakeDateTimeString(dt);
            string szTemp = "INSERT INTO r_ss_dat (ss_ID, ss_date, ss_Stat, ss_Value, ss_Bigo, Crte_User, Crte_Dttm, Mdfy_User, Mdfy_Dttm) " +
                "VALUES ('{0}','{1}', '01', '{4}', '훈련시스템입력', 'ETMGR', '{2}', 'ETMGR', '{3}')";

            string szSQL = string.Format(szTemp, szSensorName, szDate, szDate2, szDate2, fValue);
            m_dbJubix.GetResultData(szSQL, 0);
        }

        public void SetTestAlarm(int nUnitID, int nSensorID)
        {
            JubixSensor sensor = null;
            SortedList<int, JubixSensor> sensorList = null;
            if (mSensorList.TryGetValue(nUnitID, out sensorList))
            {               
                if (sensorList.TryGetValue(nSensorID, out sensor))
                {
                    SetTestAlarmJubixDB(sensor.SensorName);
                }
            } 
        }
    }

    public class JubixSensor
    {
        private int m_nID;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        private string szSensorName;
        public string SensorName
        {
            get { return szSensorName; }
            set { szSensorName = value; }
        }
        private int m_nStatus;
        public int Status
        {
            get { return m_nStatus; }
            set { m_nStatus = value; }
        }

        private bool m_bFireNotify = false;
        public bool FireNotify
        {
            get { return m_bFireNotify; }
            set { m_bFireNotify = value; }
        }

        private float m_fDensity = -9999;
        public float Density
        {
            get { return m_fDensity; }
            set { m_fDensity = value; }
        }
        
        private int sensor_type = -1;       //NONE,, FacilityType Check
        public int SensorType
        {
            get { return sensor_type; }
            set { sensor_type = value; }
        }
        
        
        public void SetStatus(int nStatus)
        {
            // 센서 알람 상태
            if( m_nStatus < nStatus)
            {
                if (nStatus == 1 || nStatus == 2 || nStatus == 3)
                {
                    if(  m_nStatus < nStatus)
                        // 센서 동작 알림
                        m_bFireNotify = true;
                }
            }

            // 알람중 센서 정상 정환
            if( m_nStatus > 0 && nStatus == 0)
            {
                // 센서 정상 알림
                m_bFireNotify = true;
            }
            m_nStatus = nStatus;
        }

        private int m_nServerID = -1;
        public int ServerID 
        {
            get
            {
                return m_nServerID;
            }
            set
            {
                m_nServerID = value;
            }
        }

        private int m_nTagID = -1;
        public int TagNo 
        {
            get { return m_nTagID; }
            set { m_nTagID = value; }
        }
    }
}
