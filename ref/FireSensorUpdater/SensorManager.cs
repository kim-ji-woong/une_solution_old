using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using DBUtility;


namespace ConsoleApplication2
{
    class SensorManager
    {
        private Mutex m_Mutex = new Mutex(false);
        private static SensorManager m_Instance = null;
        public static SensorManager Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new SensorManager();
                return m_Instance;
            }
        }

        private Dictionary<int, EquipmentZoneObjectList> m_dicSensorZone = new Dictionary<int, EquipmentZoneObjectList>();
        public Dictionary<int, EquipmentZoneObjectList> DicSensorZone
        {
            get
            {
                return m_dicSensorZone;
            }
        }

        // Key : SensorZone ID
        // Value : Sensor
        private SortedList<int, SensorZone> m_dicAllSenor = new SortedList<int, SensorZone>();
        public SortedList<int, SensorZone> DicAllSenor
        {
            get { return m_dicAllSenor; }
        }

        private Hashtable m_dicFireSenor = new Hashtable();
        public Hashtable DicFireSensor
        {
            get { return m_dicFireSenor; }
        }


        public SensorManager()
        {
        }

        public SensorZone FindSensor(int nID)
        {
            SensorZone sensor = null;
            try
            {
                m_Mutex.WaitOne();
                if (m_dicAllSenor.ContainsKey(nID))
                {
                    m_dicAllSenor.TryGetValue(nID, out sensor);
                }
            }
            finally
            {
                m_Mutex.ReleaseMutex();
            }
            return sensor;
        }

        public EquipmentZoneObjectList FindZoneInSensor(int zoneID)
        {
            EquipmentZoneObjectList sensorZone = null;
            try
            {
                m_Mutex.WaitOne();
                if (m_dicSensorZone.ContainsKey(zoneID))
                {
                    m_dicSensorZone.TryGetValue(zoneID, out sensorZone);
                }
            }
            finally
            {
                m_Mutex.ReleaseMutex();
            }
            return sensorZone;
        }

        public void ReadAllSensorData()
        {

            LoadFireSesnsor();
            
            ReadSensorZone();

        }

        private void ReadSensorZone()
        {
            try
            {
                m_Mutex.WaitOne();
                string szSQP = "SELECT ID, Type, Connected, EquipZoneID, Data, Description, OrgSensorID FROM SensorZone";
                WebDBManager webDB = ZoneManager.Instance.DBManager;
                ArrayList arrResult = webDB.GetResultData(szSQP, 0);
                int nResultCount = 0;
                if (arrResult != null)
                    nResultCount = arrResult.Count;
                else
                    nResultCount = 0;
                for (int i = 0; i < nResultCount - 6; i += 7)
                {
                    try
                    {
                        int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                        int nSensorType = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                        string bConntected = WebDBManager.GetStringField(arrResult[i + 2], "null");
                        int nZoneID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                        string nSensorData = WebDBManager.GetStringField(arrResult[i + 4], "null");
                        string szDesc = WebDBManager.GetStringField(arrResult[i + 5], "");

                        int nOrgSensroID = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                        if (nID == 0)
                            continue;
                        EquipmentZone zone = ZoneManager.Instance.GetEquipZone(nZoneID);   
                        if( zone == null)
                            continue;

                        SensorZone sensor = new SensorZone();
                        if (nSensorType == 1)
                        {
                            FireSensor fs = null;
                            if (m_dicFireSenor.ContainsKey(nOrgSensroID))
                            {
                                fs = (FireSensor)m_dicFireSenor[nOrgSensroID];
                                sensor.LinkedSensor = fs;

                                fs.EquipZone = zone;
                            }   
                        }     
                        else
                        {
                            continue;
                        }
                   
                        sensor.LinkedSensorID = nOrgSensroID;
                        sensor.ID = nID;
                        sensor.EquipZone = zone;

                        if (szDesc.Equals("null"))
                            szDesc = "";
                        sensor.Description = szDesc;

                        EquipmentZoneObjectList zoneSensor = FindZoneInSensor(nZoneID);
                        if (zoneSensor == null)
                        {
                            zoneSensor = new EquipmentZoneObjectList();                           
                            zoneSensor.Zone = zone;
                            m_dicSensorZone.Add(nZoneID, zoneSensor);
                        }

                        if (!m_dicAllSenor.ContainsKey(nID))
                            m_dicAllSenor.Add(nID, sensor);

                        if (!zoneSensor.SensorList.Contains(sensor))
                        {
                            zoneSensor.SensorList.Add(sensor);
                        }                      
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            finally
            {
                m_Mutex.ReleaseMutex();
            }
        } 
        

        private bool LoadFireSesnsor()
        {
            string szSQP = "SELECT ID, Name, PositionName, X, Y, Z, ZoneID, IsIndoor, Description FROM FireSensor where IsIndoor = " + (true ? "1" : "0");
            WebDBManager webDB = ZoneManager.Instance.DBManager;
            ArrayList arrResult = webDB.GetResultData(szSQP, 0);
            int nResultCount = 0;
            if (arrResult == null || arrResult.Count == 0)
            {
                return true;
            }
            nResultCount = arrResult.Count;
            for (int i = 0; i < nResultCount - 8; i += 9)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string szName = WebDBManager.GetStringField(arrResult[i + 1].ToString(), "null");
                string szPosName = WebDBManager.GetStringField(arrResult[i + 2], "null");
                float fx = WebDBManager.GetFloatField(arrResult[i + 3].ToString(), 0.0f);
                float fy = WebDBManager.GetFloatField(arrResult[i + 4].ToString(), 0.0f);
                float fz = WebDBManager.GetFloatField(arrResult[i + 5].ToString(), 0.0f);
                int nZoneID = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);

                string szIndoor = WebDBManager.GetStringField(arrResult[i + 7], "null");
                string szDesc = WebDBManager.GetStringField(arrResult[i + 8], "");

                Zone zone = ZoneManager.Instance.GetZone(nZoneID);
                if (zone == null || zone.IsOutdoor != !true)
                {
                    continue;
                }

                FireSensor sensor = new FireSensor();
                sensor.ID = nID;
                sensor.X = fx;
                sensor.Y = fz;
                sensor.Z = fy;
                sensor.Zone = zone;
                sensor.Description = szDesc;

                m_dicFireSenor.Add(nID, sensor);
                
            }
            return true;
        }             

    }
}
