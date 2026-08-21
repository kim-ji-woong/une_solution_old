using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using DBUtility2;
using UnE.Spatial;
using UnE.Sensor;
using System.Text;

namespace SDMS
{
	public class SensorManager
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

        // Key : EquipmentZone ID
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
		private SortedList<int, ISensor> m_dicAllSenor = new SortedList<int, ISensor>();

		public SortedList<int, ISensor> DicAllSenor
		{
			get { return m_dicAllSenor; }
		}

        private Dictionary<int, List<ISensor>> m_dicFireSensorByZoneID = new Dictionary<int, List<ISensor>>();
        public Dictionary<int, List<ISensor>> DicFireSensorByZoneID
        {
            get { return m_dicFireSensorByZoneID; }
        }

        private Dictionary<int, List<ISensor>> m_dicDoorSensorByZoneID = new Dictionary<int, List<ISensor>>();
        public Dictionary<int, List<ISensor>> DicDoorSensorByZoneID
        {
            get { return m_dicDoorSensorByZoneID; }
        }

        // Key : Origin Sensor
        private Dictionary<int, List<ISensor>> m_dicFireSensor = new Dictionary<int, List<ISensor>>();
        public Dictionary<int, List<ISensor>> DicFireSensor
        {
            get { return m_dicFireSensor; }
        }

        // Key : Origin Sensor
        private Dictionary<int, List<ISensor>> m_dicSecuritySensor = new Dictionary<int, List<ISensor>>();
        public Dictionary<int, List<ISensor>> DicSecuritySensor
        {
            get { return m_dicSecuritySensor; }
        }

        // Key : Origin Sensor
        private Dictionary<int, List<ISensor>> m_dicSecuritySecomExSensor = new Dictionary<int, List<ISensor>>();
        public Dictionary<int, List<ISensor>> DicSecuritySecomExSensor
        {
            get { return m_dicSecuritySecomExSensor; }
        }

        // Key : Origin Sensor
        private Dictionary<int, List<ISensor>> m_dicSecuritySecomWomanSensor = new Dictionary<int, List<ISensor>>();
        public Dictionary<int, List<ISensor>> DicSecuritySecomWomanSensor
        {
            get { return m_dicSecuritySecomWomanSensor; }
        }

        // Key : Origin Sensor
        private Dictionary<int, List<ISensor>> m_dicSmokeSensor = new Dictionary<int, List<ISensor>>();
        public Dictionary<int, List<ISensor>> DicSmokeSensor
        {
            get { return m_dicSmokeSensor; }
        }

        // Key : Origin Sensor
        private Dictionary<int, List<ISensor>> m_dicSpringCooler = new Dictionary<int, List<ISensor>>();
        public Dictionary<int, List<ISensor>> DicSpringCooler
        {
            get { return m_dicSpringCooler; }
        }

        // Key : Origin Sensor
        private Dictionary<int, List<ISensor>> m_dicPressureSensor = new Dictionary<int, List<ISensor>>();
        public Dictionary<int, List<ISensor>> DicPressureSensor
        {
            get { return m_dicPressureSensor; }
        }

		private Hashtable m_dicAlarmStation = new Hashtable();

		public System.Collections.Hashtable DicAlarmStation
		{
			get { return m_dicAlarmStation; }
			set { m_dicAlarmStation = value; }
		}

        private Hashtable m_dicPSMSensorZone = new Hashtable();
        public Hashtable DicPSMSensorZone
        {
            get { return m_dicPSMSensorZone; }
        }

        private int m_nSiteID = 1;
		public SensorManager()
		{
            m_nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;
		}

		public ISensor FindSensor(int nID)
		{
			ISensor sensor = null;
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

        public List<ISensor> FindSensorZoneFromType(IFacility.FacilityType type)
        {
            List<ISensor> sensors = new List<ISensor>();

            foreach (KeyValuePair<int, ISensor> pair in m_dicAllSenor)
            {
                if (pair.Value.Type == type)
                    sensors.Add(pair.Value);
            }

            return sensors;
        }

        public List<ISensor> FindZoneInSensor(int zoneID, IFacility.FacilityType sensortype)
		{
			EquipmentZoneObjectList sensorZone = null;
            EquipmentZoneObjectList sensorZone2 = null;
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

			if (sensorZone != null)
			{
                sensorZone2 = new EquipmentZoneObjectList();
                for (int i = 0; i < sensorZone.SensorList.Count; i++)
                {
                    ISensor sensor = (ISensor)sensorZone.SensorList[i];
                    if (sensor.Type == sensortype)
                        sensorZone2.SensorList.Add(sensorZone.SensorList[i]);
                }

                return sensorZone2.SensorList;
			}

			return null;
		}
        public List<ISensor> FindZoneInSensorIntrusion(int zoneID)
        {

            EquipmentZoneObjectList sensorZone = null;
            EquipmentZoneObjectList sensorZone2 = null;
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

            if (sensorZone != null)
            {
                sensorZone2 = new EquipmentZoneObjectList();
                for (int i = 0; i < sensorZone.SensorList.Count; i++)
                {
                    ISensor sensor = (ISensor)sensorZone.SensorList[i];
                    if (sensor.Type == IFacility.FacilityType.Intrusion_S1 ||
                        sensor.Type == IFacility.FacilityType.Loiter_S1 ||
                        sensor.Type == IFacility.FacilityType.Collapse_S1 ||
                        sensor.Type == IFacility.FacilityType.Theft_S1 ||
                        sensor.Type == IFacility.FacilityType.Neglect_S1 ||
                        sensor.Type == IFacility.FacilityType.VirtualFence_S1 ||
                        sensor.Type == IFacility.FacilityType.EmergencyBell_S1 ||
                        sensor.Type == IFacility.FacilityType.GeneralIntrusionT1_S1 ||
                        sensor.Type == IFacility.FacilityType.GeneralIntrusionT2_S1 ||
                        sensor.Type == IFacility.FacilityType.InternalIntrusionT3_S1 ||
                        sensor.Type == IFacility.FacilityType.VaultIntrusionT4_S1 ||
                        sensor.Type == IFacility.FacilityType.CustomerEmergencyC1_S1 ||
                        sensor.Type == IFacility.FacilityType.CustomerEmergencyC2_S1 ||
                        sensor.Type == IFacility.FacilityType.RescueQQ_S1 ||
                        sensor.Type == IFacility.FacilityType.GasG1_S1 ||
                        sensor.Type == IFacility.FacilityType.BlackoutAbnormalityU1_S1 ||
                        sensor.Type == IFacility.FacilityType.LeakAbnormalityU4_S1 ||
                        sensor.Type == IFacility.FacilityType.SynthesisAlertAbnormalityU8_S1 ||
                        sensor.Type == IFacility.FacilityType.ExternalAlarmBell ||
                        sensor.Type == IFacility.FacilityType.SecomExternalAlarmBell ||
                        sensor.Type == IFacility.FacilityType.SecomWomenAlarmBell)
                    {
                        sensorZone2.SensorList.Add(sensorZone.SensorList[i]);
                    }
                }

                return sensorZone2.SensorList;
                ////


                ////foreach (SensorZone sensor in sensorZone.SensorList)
                ////{
                ////    if (sensor.Type == sensorZone.SensorList)
                //return sensorZone.SensorList;
                ////}
            }

            return null;
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
            ReadSensorZone();            
        }

        public ArrayList GetExtranSensorList()
        {
            ArrayList arResult = new ArrayList();
            try
            {              

                m_Mutex.WaitOne();

                string szText = "SELECT sz.ID, sz.Type, sz.Connected, sz.EquipZoneID, sz.Data, sz.Description, sz.OrgSensorID FROM SensorZone sz " +
                                "  INNER JOIN EquipmentZone as ez ON ez.ID = sz.EquipZoneID and ez.ID = 0 and ez.SiteID = {0}";

                string szSQL = string.Format(szText, m_nSiteID);

                WebDBManager webDB = UnE.View.Content.ViewUtils.GetContentViewOwner().DBManager;
                
                ArrayList arrResult = webDB.GetResultData(szSQL);
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
                        int nEquipZoneID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                        string nSensorData = WebDBManager.GetStringField(arrResult[i + 4], "null");
                        string szDesc = WebDBManager.GetStringField(arrResult[i + 5], "");

                        int nOrgSensorID = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                        if (nID == 0)
                            continue;

                        ISensor sensor = null;
                        List<ISensor> sensors = null;

                        if (nSensorType == (int)IFacility.FacilityType.FIRE_SENSOR || nSensorType == (int)IFacility.FacilityType.FireSensor_TypeA || nSensorType == (int)IFacility.FacilityType.FireSensor_SiemensType)
                        {
                            FireSensor fs = null;

                            if (m_dicFireSensor.TryGetValue(nOrgSensorID, out sensors) == false)
                            {
                                sensors = new List<ISensor>();
                                m_dicFireSensor[nOrgSensorID] = sensors;
                                fs = new FireSensor();
                                sensors.Add(fs);
                            }
                            else
                            {
                                fs = (FireSensor)GetSensor(sensors, nID);

                                if (fs == null)
                                {
                                    fs = new FireSensor();
                                    sensors.Add(fs);
                                }
                            }
                            
                            sensor = fs;
                            /*FireSensor fs = null;
                            if (m_dicFireSensor.ContainsKey(nOrgSensroID))
                            {
                                fs = (FireSensor)m_dicFireSensor[nOrgSensroID];
                            }
                            else
                                fs = new FireSensor();
                            sensor = fs;*/
                        }
                        else if (nSensorType == (int)IFacility.FacilityType.FireSensor_AnalogSmokeType)
                        {
                            SmokeSensor fs = null;

                            if (m_dicSmokeSensor.TryGetValue(nOrgSensorID, out sensors) == false)
                            {
                                sensors = new List<ISensor>();
                                m_dicSmokeSensor[nOrgSensorID] = sensors;
                                fs = new SmokeSensor();
                                sensors.Add(fs);
                            }
                            else
                            {
                                fs = (SmokeSensor)GetSensor(sensors, nID);

                                if (fs == null)
                                {
                                    fs = new SmokeSensor();
                                    sensors.Add(fs);
                                }
                            }

                            sensor = fs;
                            /*SmokeSensor fs = null;
                            if (m_dicSmokeSenor.ContainsKey(nOrgSensorID))
                            {
                                fs = (SmokeSensor)m_dicSmokeSenor[nOrgSensorID];
                            }
                            else
                                fs = new SmokeSensor();
                            sensor = fs;*/
                        }
                        else if (nSensorType == (int)IFacility.FacilityType.COOLER_SENSOR)
                        {
                            SpringCooler fs = null;

                            if (m_dicSpringCooler.TryGetValue(nOrgSensorID, out sensors) == false)
                            {
                                sensors = new List<ISensor>();
                                m_dicSpringCooler[nOrgSensorID] = sensors;
                                fs = new SpringCooler();
                                sensors.Add(fs);
                            }
                            else
                            {
                                fs = (SpringCooler)GetSensor(sensors, nID);

                                if (fs == null)
                                {
                                    fs = new SpringCooler();
                                    sensors.Add(fs);
                                }
                            }

                            sensor = fs;
                            /*SpringCooler fs = null;
                            if (m_dicSpringCooler.ContainsKey(nOrgSensorID))
                            {
                                fs = (SpringCooler)m_dicSpringCooler[nOrgSensorID];
                            }
                            else
                                fs = new SpringCooler();
                            sensor = fs;*/
                        }
                        else if (nSensorType == (int)IFacility.FacilityType.PRESSURE_SENSOR)
                        {
                            PumpPressureSensor fs = null;

                            if (m_dicPressureSensor.TryGetValue(nOrgSensorID, out sensors) == false)
                            {
                                sensors = new List<ISensor>();
                                m_dicPressureSensor[nOrgSensorID] = sensors;
                                fs = new PumpPressureSensor();
                                sensors.Add(fs);
                            }
                            else
                            {
                                fs = (PumpPressureSensor)GetSensor(sensors, nID);

                                if (fs == null)
                                {
                                    fs = new PumpPressureSensor();
                                    sensors.Add(fs);
                                }
                            }

                            sensor = fs;
                            /*PumpPressureSensor fs = null;
                            if (m_dicPressureSensor.ContainsKey(nOrgSensorID))
                            {
                                fs = (PumpPressureSensor)m_dicPressureSensor[nOrgSensorID];
                            }
                            else
                                fs = new PumpPressureSensor();
                            sensor = fs;*/
                        }
                        else if (nSensorType == (int)IFacility.FacilityType.FireSensor_ManualControl)
                        {
                            continue;
                        }
                        else if (nSensorType == (int)IFacility.FacilityType.FireSensor_MonitoringType)
                        {
                            continue;
                        }
                        else
                        {
                            continue;
                        }

                        sensor.OrgSensorID = nOrgSensorID;
                        sensor.ID = nID;
                        sensor.EquipZoneID = nEquipZoneID;
                        sensor.EquipZoneDB = nEquipZoneID;
                        if (szDesc.Equals("null"))
                            szDesc = "";
                        sensor.Description = szDesc;
                        if (bConntected.Equals("1"))
                        {
                            sensor.Connected = true;
                        }
                        else
                        {
                            sensor.Connected = false;
                        }

                        if (nSensorData.Equals("1"))
                        {
                            sensor.SensorData = 1;
                            sensor.InitSensor = true;
                        }
                        else if (nSensorData.Equals("0"))
                        {
                            sensor.SensorData = 0;
                            sensor.InitSensor = true;
                        }
                        else
                        {
                            sensor.SensorData = -1;
                            sensor.InitSensor = false;
                        }

                        if (sensor.POI != null && sensor.POI.Popup != null)
                        {
                            sensor.POI.Popup.Sensor = sensor;
                        }

                        arResult.Add(sensor);
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

            return arResult;
        }

        private ISensor GetSensor(List<ISensor> sensors, int nID)
        {
            foreach (ISensor sensor in sensors)
            {
                if (sensor.ID == nID)
                    return sensor;
            }

            return null;
        }

        public ISensor MakeNewFireSensor(int nSensorZoneID)
        {
            ArrayList arResult = new ArrayList();
            try
            {

                m_Mutex.WaitOne();

                string szText = "SELECT sz.ID, sz.Type, sz.Connected, sz.EquipZoneID, sz.Data, sz.Description, sz.OrgSensorID FROM SensorZone sz " +
                                "  INNER JOIN EquipmentZone as ez ON ez.ID = sz.EquipZoneID and ez.ID = 0 and ez.SiteID = {0} and sz.ID = {1}";

                string szSQL = string.Format(szText, m_nSiteID, nSensorZoneID);



                UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
                WebDBManager webDB = owner.DBManager;
                ArrayList arrResult = webDB.GetResultData(szSQL);
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

                        int nOrgSensorID = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                        if (nID == 0)
                            continue;

                        ISensor sensor = null;
                        List<ISensor> sensors = null;

                        if (nSensorType == (int)IFacility.FacilityType.FIRE_SENSOR || nSensorType == (int)IFacility.FacilityType.FireSensor_TypeA || nSensorType == (int)IFacility.FacilityType.FireSensor_SiemensType)
                        {
                            FireSensor fs = null;

                            if (m_dicFireSensor.TryGetValue(nOrgSensorID, out sensors) == false)
                            {
                                sensors = new List<ISensor>();
                                m_dicFireSensor[nOrgSensorID] = sensors;
                                fs = new FireSensor();
                                sensors.Add(fs);
                            }
                            else
                            {
                                fs = (FireSensor)GetSensor(sensors, nID);

                                if (fs == null)
                                {
                                    fs = new FireSensor();
                                    sensors.Add(fs);
                                }
                            }

                            sensor = fs;
                            /*FireSensor fs = null;
                            if (m_dicFireSensor.ContainsKey(nOrgSensroID))
                            {
                                fs = (FireSensor)m_dicFireSensor[nOrgSensroID];
                            }
                            else
                                fs = new FireSensor();
                            sensor = fs;*/
                        }
                        else if (nSensorType == (int)IFacility.FacilityType.FireSensor_AnalogSmokeType)
                        {
                            SmokeSensor fs = null;

                            if (m_dicSmokeSensor.TryGetValue(nOrgSensorID, out sensors) == false)
                            {
                                sensors = new List<ISensor>();
                                m_dicSmokeSensor[nOrgSensorID] = sensors;
                                fs = new SmokeSensor();
                                sensors.Add(fs);
                            }
                            else
                            {
                                fs = (SmokeSensor)GetSensor(sensors, nID);

                                if (fs == null)
                                {
                                    fs = new SmokeSensor();
                                    sensors.Add(fs);
                                }
                            }

                            sensor = fs;
                            /*SmokeSensor fs = null;
                            if (m_dicSmokeSensor.ContainsKey(nOrgSensorID))
                            {
                                fs = (SmokeSensor)m_dicSmokeSensor[nOrgSensorID];
                            }
                            else
                                fs = new SmokeSensor();
                            sensor = fs;*/
                        }
                        else if (nSensorType == (int)IFacility.FacilityType.COOLER_SENSOR)
                        {
                            SpringCooler fs = null;

                            if (m_dicSpringCooler.TryGetValue(nOrgSensorID, out sensors) == false)
                            {
                                sensors = new List<ISensor>();
                                m_dicSpringCooler[nOrgSensorID] = sensors;
                                fs = new SpringCooler();
                                sensors.Add(fs);
                            }
                            else
                            {
                                fs = (SpringCooler)GetSensor(sensors, nID);

                                if (fs == null)
                                {
                                    fs = new SpringCooler();
                                    sensors.Add(fs);
                                }
                            }

                            sensor = fs;
                            /*SpringCooler fs = null;
                            if (m_dicSpringCooler.ContainsKey(nOrgSensorID))
                            {
                                fs = (SpringCooler)m_dicSpringCooler[nOrgSensorID];
                            }
                            else
                                fs = new SpringCooler();
                            sensor = fs;*/
                        }
                        else if (nSensorType == (int)IFacility.FacilityType.PRESSURE_SENSOR)
                        {
                            PumpPressureSensor fs = null;

                            if (m_dicPressureSensor.TryGetValue(nOrgSensorID, out sensors) == false)
                            {
                                sensors = new List<ISensor>();
                                m_dicPressureSensor[nOrgSensorID] = sensors;
                                fs = new PumpPressureSensor();
                                sensors.Add(fs);
                            }
                            else
                            {
                                fs = (PumpPressureSensor)GetSensor(sensors, nID);

                                if (fs == null)
                                {
                                    fs = new PumpPressureSensor();
                                    sensors.Add(fs);
                                }
                            }

                            sensor = fs;
                            /*PumpPressureSensor fs = null;
                            if (m_dicPressureSensor.ContainsKey(nOrgSensorID))
                            {
                                fs = (PumpPressureSensor)m_dicPressureSensor[nOrgSensorID];
                            }
                            else
                                fs = new PumpPressureSensor();
                            sensor = fs;*/
                        }
                        else if (nSensorType == (int)IFacility.FacilityType.FireSensor_ManualControl)
                        {
                            continue;
                        }
                        else if (nSensorType == (int)IFacility.FacilityType.FireSensor_MonitoringType)
                        {
                            continue;
                        }
                        else
                        {
                            continue;
                        }

                        sensor.OrgSensorID = nOrgSensorID;
                        sensor.ID = nID;
                        sensor.EquipZoneID = nZoneID;
                        sensor.EquipZoneDB = nZoneID;
                        if (szDesc.Equals("null"))
                            szDesc = "";
                        sensor.Description = szDesc;
                        if (bConntected.Equals("1"))
                        {
                            sensor.Connected = true;
                        }
                        else
                        {
                            sensor.Connected = false;
                        }

                        if (nSensorData.Equals("1"))
                        {
                            sensor.SensorData = 1;
                            sensor.InitSensor = true;
                        }
                        else if (nSensorData.Equals("0"))
                        {
                            sensor.SensorData = 0;
                            sensor.InitSensor = true;
                        }
                        else
                        {
                            sensor.SensorData = -1;
                            sensor.InitSensor = false;
                        }

                        if (sensor.POI != null && sensor.POI.Popup != null)
                        {
                            sensor.POI.Popup.Sensor = sensor;
                        }

                        return sensor;
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

            return null; ;
        }

        private Dictionary<int, POI> m_dicDoorPOI = new Dictionary<int, POI>();
        public Dictionary<int, POI> DicDoorPOI
        {
            get { return m_dicDoorPOI; }
            set { m_dicDoorPOI = value; }
        }

        public void ReadDoorSensor()
        {
            m_Mutex.WaitOne();

            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT ID, Name, ZoneID, X, Y, Z From DoorSensor ");

                UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
                WebDBManager webDB = owner.DBManager;

                ArrayList arrResult = webDB.GetResultData(sb.ToString());
                if (arrResult == null)
                    return;

                int nResultCount = arrResult.Count;
                for (int i = 0; i < nResultCount; i += 6)
                {
                    VariousData<int> nID = WebDBManager.GetIntField(arrResult[i].ToString());
                    string strName = WebDBManager.GetStringField(arrResult[i + 1]);
                    VariousData<int> nZoneID = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                    VariousData<float> x = WebDBManager.GetFloatField(arrResult[i + 3].ToString());
                    VariousData<float> y = WebDBManager.GetFloatField(arrResult[i + 4].ToString());
                    VariousData<float> z = WebDBManager.GetFloatField(arrResult[i + 5].ToString());

                    if (nID == null || nZoneID == null || x == null || y == null || z == null)
                        continue;

                    Zone zone = ZoneManager.Instance.GetZone(nZoneID.Data);

                    EtcSensor etc = new EtcSensor();
                    etc.SetSensorType(IFacility.FacilityType.DOOR);
                    etc.ID = nID.Data;
                    etc.OrgSensorID = nID.Data;
                    etc.SensorName = strName;
                    etc.ZoneID = nZoneID.Data;

                    etc.POI = new POI();
                    etc.POI.Popup = etc.CreatePopup(null, null);
                    etc.POI.Facility = etc;
                    etc.POI.ID = nID.Data;
                    etc.POI.X = x.Data;
                    etc.POI.Y = y.Data;
                    etc.POI.Z = z.Data;
                    etc.POI.Zone = zone;
                    etc.POI.IsIndoor = true;
                    
                    if (!m_dicDoorSensorByZoneID.ContainsKey(etc.ZoneID))
                        m_dicDoorSensorByZoneID.Add(etc.ZoneID, new List<ISensor>());

                    m_dicDoorSensorByZoneID[etc.ZoneID].Add(etc);
                    m_dicDoorPOI[etc.ID] = etc.POI;
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                m_Mutex.ReleaseMutex();
            }
        }

		public void ReadSensorZone()
		{
			try
			{
				m_Mutex.WaitOne();

                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT sz.ID, sz.Type, sz.Connected, sz.EquipZoneID, sz.Data, sz.Description, sz.OrgSensorID, SensorName, DeActivate, sz.Zone ");
                sb.Append("  FROM SensorZone sz ");
                sb.Append(" INNER JOIN SensorTagInfo as sti on sti.SensorZoneID = sz.ID");
                sb.Append(" INNER JOIN EquipmentZone as ez ON ez.ID = sz.EquipZoneID");
                sb.AppendFormat("  And ez.SiteID = {0}", m_nSiteID);

                UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
                WebDBManager webDB = owner.DBManager;

				ArrayList arrResult = webDB.GetResultData(sb.ToString());
                if (arrResult == null)
                    return;

				int nResultCount = arrResult.Count;
				for (int i = 0; i < nResultCount - 9; i += 10)
				{
					try
					{
						int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
						int nSensorType = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
						string bConntected = WebDBManager.GetStringField(arrResult[i + 2], "null");
						int nEquipZoneID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
						string strSensorData = WebDBManager.GetStringField(arrResult[i + 4]);
						string szDesc = WebDBManager.GetStringField(arrResult[i + 5], "");
						int nOrgSensorID = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                        string strSensorName = WebDBManager.GetStringField(arrResult[i + 7], "");
                        string strDeActivate = WebDBManager.GetStringField(arrResult[i + 8], "");
                        int nZoneID = WebDBManager.GetIntField(arrResult[i + 9].ToString(), -1);

                        if (nID == 0)
							continue;

						ISensor sensor = null;
                        List<ISensor> sensors = null;

                        if (nSensorType == (int)IFacility.FacilityType.FIRE_SENSOR || nSensorType == (int)IFacility.FacilityType.FireSensor_TypeA || nSensorType == (int)IFacility.FacilityType.FireSensor_SiemensType)
						{
                            sensor = SetFireSensor(nOrgSensorID, nID, nZoneID, nEquipZoneID, out sensors);                            
						}
                        else if (nSensorType == (int)IFacility.FacilityType.FireSensor_AnalogSmokeType)
                        {
                            SmokeSensor fs = null;

                            if (m_dicSmokeSensor.TryGetValue(nOrgSensorID, out sensors) == false)
                            {
                                sensors = new List<ISensor>();
                                m_dicSmokeSensor[nOrgSensorID] = sensors;
                                fs = new SmokeSensor();
                                sensors.Add(fs);
                            }
                            else
                            {
                                fs = (SmokeSensor)GetSensor(sensors, nID);

                                if (fs == null)
                                {
                                    fs = new SmokeSensor();
                                    sensors.Add(fs);
                                }
                            }

                            fs.OrgSensorID = nOrgSensorID;

                            EquipmentZone eq = ZoneManager.Instance.GetEquipZone(nEquipZoneID);

                            if (eq != null)
                            {
                                fs.PositionName = eq.DisplayText;
                            }

                            sensor = fs;
                        }
                        else if (nSensorType == (int)IFacility.FacilityType.COOLER_SENSOR)
                        {
                            SpringCooler fs = null;

                            if (m_dicSpringCooler.TryGetValue(nOrgSensorID, out sensors) == false)
                            {
                                sensors = new List<ISensor>();
                                m_dicSpringCooler[nOrgSensorID] = sensors;
                                fs = new SpringCooler();
                                sensors.Add(fs);
                            }
                            else
                            {
                                fs = (SpringCooler)GetSensor(sensors, nID);

                                if (fs == null)
                                {
                                    fs = new SpringCooler();
                                    sensors.Add(fs);
                                }
                            }

                            fs.OrgSensorID = nOrgSensorID;
                            sensor = fs;
                        }
                        else if (nSensorType == (int)IFacility.FacilityType.PRESSURE_SENSOR)
                        {
                            PumpPressureSensor fs = null;

                            if (m_dicPressureSensor.TryGetValue(nOrgSensorID, out sensors) == false)
                            {
                                sensors = new List<ISensor>();
                                m_dicPressureSensor[nOrgSensorID] = sensors;
                                fs = new PumpPressureSensor();
                                sensors.Add(fs);
                            }
                            else
                            {
                                fs = (PumpPressureSensor)GetSensor(sensors, nID);

                                if (fs == null)
                                {
                                    fs = new PumpPressureSensor();
                                    sensors.Add(fs);
                                }
                            }

                            fs.OrgSensorID = nOrgSensorID;
                            sensor = fs;
                        }
                        else if (nSensorType == (int)IFacility.FacilityType.FireSensor_ManualControl)
                        {
                            continue;
                        }
                        else if (nSensorType == (int)IFacility.FacilityType.FireSensor_MonitoringType)
                        {
                            continue;
                        }
                        else if (nSensorType == (int)IFacility.FacilityType.PSM_SENSOR)
                        {
                            UnE.PSM.PSMSensorZone psz = null;

                            if (m_dicPSMSensorZone.Contains(nID))
                            {
                                psz = (UnE.PSM.PSMSensorZone)m_dicPSMSensorZone[nID];
                            }
                            else
                            {
                                psz = new UnE.PSM.PSMSensorZone();
                             
                                psz.OrgSensor = owner.GetPSMSensor(nOrgSensorID);
                                if(psz.OrgSensor != null)
                                {
                                    if( psz.OrgSensor.LinkedTankList != null && psz.OrgSensor.LinkedTankList.Count > 0)
                                    {
                                        psz.PositionName = psz.OrgSensor.LinkedTankList[0].LocationName;
                                    }
                                }
                            }

                            sensor = psz;

                            m_dicPSMSensorZone.Add(nID, psz);
                        }
                        else if( nSensorType == (int)IFacility.FacilityType.FireF1_S1 || 
                                 nSensorType == (int)IFacility.FacilityType.Fire_S1 ||
                                 nSensorType == (int)IFacility.FacilityType.SecomFire)
                        {
                            sensor = SetFireSensor(nOrgSensorID, nID, nZoneID, nEquipZoneID, out sensors);
                        }
                        else if (nSensorType >= (int)IFacility.FacilityType.Intrusion_S1 && nSensorType <= (int)IFacility.FacilityType.ExternalAlarmBell)
                        {
                            sensor = SetSecuritySensor(nOrgSensorID, nID, nEquipZoneID, nSensorType, m_dicSecuritySensor, out sensors);
                            
                        }
                        /**
                        ** orgSensorID 가 다르므로 추가. by hypark 2018.06.19
                        */
                        else if (nSensorType >= (int)IFacility.FacilityType.SecomWomenAlarmBell)
                        {
                            sensor = SetSecuritySensor(nOrgSensorID, nID, nEquipZoneID, nSensorType, m_dicSecuritySecomWomanSensor, out sensors);                            
                        }
                        else if (nSensorType >= (int)IFacility.FacilityType.SecomExternalAlarmBell)
                        {
                            sensor = SetSecuritySensor(nOrgSensorID, nID, nEquipZoneID, nSensorType, m_dicSecuritySecomExSensor, out sensors);                            
                        }
                        else if (nSensorType == (int)IFacility.FacilityType.Earthquake)
                        {
                            sensor = SetEarthquakeSensor(nID);
                        }
                        else
                        {
                            sensor = SetEtcSensor(nID, nSensorType);
                        }

						sensor.OrgSensorID = nOrgSensorID;
						sensor.ID = nID;
						sensor.EquipZoneID = nEquipZoneID;
                        sensor.EquipZoneDB = nEquipZoneID;
                        sensor.SensorName = strSensorName;
                        sensor.DeActivate = strDeActivate.StartsWith("N") ? false : true;

                        if (szDesc.Equals("null"))
							szDesc = "";

						sensor.Description = szDesc;

						if (bConntected.Equals("1"))
						{
							sensor.Connected = true;
						}
						else
						{
							sensor.Connected = false;
						}

                        int sensorData;
                        sensor.InitSensor = true;

                        if (strSensorData != null && int.TryParse(strSensorData, out sensorData))
                        {
                            sensor.SensorData = sensorData;
                        }
                        else
                            sensor.SensorData = -1;
                        
						if (sensor.POI != null && sensor.POI.Popup != null)
						{
							sensor.POI.Popup.Sensor = sensor;
						}

						if (sensor.InitSensor == true)
						{
							if (sensor.SensorData == 1 || sensor.Connected == false)
							{
								/*if (!m_arAbnormalSensor.Contains(sensor))
									m_arAbnormalSensor.Add(sensor);*/
							}
							else
							{
								//m_arAbnormalSensor.Remove(sensor);
								// DB 모니터링을 통한 Process 변경을 하지 않는다.
								//ProcessManager.Instance.EndProcess(sensor.ID);
							}

							if (sensor.Connected == false)
							{
                                if( sensor.POI != null)
                                {
                                    if( sensor.POI.ViewType == 1)
                                    {
                                        //Core.BaseView view = (Core.BaseView)sensor.POI.ParentView;
                                        //if (view != null)
                                        //    view.UpdateIcon(sensor.POI.ID, sensor.POI.Facility.DisconnectIconPath);
                                    }
                                   
                                }
								
							}
						}

                        EquipmentZoneObjectList zoneSensor = GetZoneInSensor(nEquipZoneID);						

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

        private FireSensor SetFireSensor(int nOrgSensorID, int nID, int nZoneID, int nEquipZoneID, out List<ISensor> sensors)
        {
            FireSensor fs = null;

            if (m_dicFireSensor.TryGetValue(nOrgSensorID, out sensors) == false)
            {
                sensors = new List<ISensor>();
                m_dicFireSensor[nOrgSensorID] = sensors;
                fs = new FireSensor();                
                sensors.Add(fs);
            }
            else
            {
                fs = (FireSensor)GetSensor(sensors, nID);

                if (fs == null)
                {
                    fs = new FireSensor();
                    sensors.Add(fs);
                }
            }

            fs.OrgSensorID = nOrgSensorID;
            fs.ZoneID = nZoneID;

            EquipmentZone eq = ZoneManager.Instance.GetEquipZone(nEquipZoneID);

            if (eq != null)
            {
                fs.PositionName = eq.DisplayText;
            }

            if (!m_dicFireSensorByZoneID.ContainsKey(nZoneID))
                m_dicFireSensorByZoneID.Add(nZoneID, new List<ISensor>());

            m_dicFireSensorByZoneID[nZoneID].Add(fs);

            return fs;
        }

        private SecuritySensor SetSecuritySensor(int nOrgSensorID, int nID, int nZoneID, int nSensorType, Dictionary<int, List<ISensor>> dicSensors, out List<ISensor> sensors)
        {
            SecuritySensor fs = null;

            if (dicSensors.TryGetValue(nOrgSensorID, out sensors) == false)
            {
                sensors = new List<ISensor>();
                dicSensors[nOrgSensorID] = sensors;
                fs = new SecuritySensor();
                sensors.Add(fs);
            }
            else
            {
                fs = (SecuritySensor)GetSensor(sensors, nID);

                if (fs == null)
                {
                    fs = new SecuritySensor();
                    sensors.Add(fs);
                }
            }

            fs.SubType = IFacility.ToFacilityType(nSensorType);
            fs.OrgSensorID = nOrgSensorID;

            EquipmentZone eq = ZoneManager.Instance.GetEquipZone(nZoneID);
            if (eq != null)
            {
                fs.PositionName = eq.DisplayText;
            }

            return fs;
        }

        private EarthquakeSensor SetEarthquakeSensor(int nID)
        {
            EarthquakeSensor sensor = new EarthquakeSensor();
            sensor.ID = nID;
            return sensor;
        }

        private EtcSensor SetEtcSensor(int nID, int nSensorType)
        {
            EtcSensor sensor = new EtcSensor(IFacility.ToFacilityType(nSensorType));
            sensor.ID = nID;
            return sensor;
        }

        public void AddSensor(FireSensor sensor)
        {
            int nID = sensor.ID;
            int nZoneID = sensor.EquipZoneID;
            EquipmentZoneObjectList zoneSensor = GetZoneInSensor(nZoneID);

            if (!m_dicAllSenor.ContainsKey(nID))
                m_dicAllSenor.Add(nID, sensor);

            if (!zoneSensor.SensorList.Contains(sensor))
            {
                zoneSensor.SensorList.Add(sensor);
            }
        }

        public EquipmentZoneObjectList GetZoneInSensor(int nZoneID)
        {
            EquipmentZoneObjectList zoneSensor = FindZoneInSensor(nZoneID);
            if (zoneSensor == null)
            {
                zoneSensor = new EquipmentZoneObjectList();
                EquipmentZone zone = ZoneManager.Instance.GetEquipZone(nZoneID);
                zoneSensor.Zone = zone;
                m_dicSensorZone.Add(nZoneID, zoneSensor);
            }
            return zoneSensor;
        }

        public bool LoadAllSensor(ISensorTooltipOwner view, bool isIndoor)
		{
			/*if (!LoadFireSesnsor(view, isIndoor))
				return false;

			if (!LoadSpringCooler(view, isIndoor))
				return false;

			if (!LoadPumpPressuerSensor(view, isIndoor))
				return false;

            if (!LoadSmokeSensor(view, isIndoor))
                return false;*/
			//if (!LoadAlarmStation(view, isIndoor))
			//	return false;

			return true;
		}

        private bool LoadAlarmStation(ISensorTooltipOwner view, bool isIndoor)
		{
            UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
            WebDBManager dbMgr = owner.DBManager;

			//string strSQL = "Select ID, RFIDTag, EquipID, RFIDTagID, DxfObjID, EquipType, ZoneID, X, Y, Z, Description from FireEquipment where EquipType = 3";
            string szText = "Select fe.ID, fe.RFIDTag, fe.EquipID, fe.RFIDTagID, fe.DxfObjID, fe.EquipType, fe.ZoneID, fe.X, fe.Y, fe.Z, fe.Description from FireEquipment as fe "+
                            " INNER JOIN Zone as z on z.ID = fe.ZoneID and z.SiteID = {0} and fe.EquipType = " + ((int)IFacility.FacilityType.FA).ToString();

            string strSQL = string.Format(szText, m_nSiteID);

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

			if (arrResult == null)
				return false;

			int nResultCount = arrResult.Count;

			for (int i = 0; i < nResultCount - 10; i += 11)
			{
				int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
				string strRFIDTag = WebDBManager.GetStringField(arrResult[i + 1], "");
				string strEquipID = WebDBManager.GetStringField(arrResult[i + 2], "");
				string strRFIDTagID = WebDBManager.GetStringField(arrResult[i + 3], "");
				string strDxfObjID = WebDBManager.GetStringField(arrResult[i + 4], "");
				int nEquipType = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
				int nZoneID = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
				float x = WebDBManager.GetFloatField(arrResult[i + 7].ToString(), 0.0f);
				float y = WebDBManager.GetFloatField(arrResult[i + 8].ToString(), 0.0f);
				float z = WebDBManager.GetFloatField(arrResult[i + 9].ToString(), 0.0f);
				string strDescription = WebDBManager.GetStringField(arrResult[i + 10], "");

				if (nID < 0)
					continue;

                IFacility.FacilityType type = IFacility.FacilityType.NONE;

                type = IFacility.FacilityType.FA;

				Zone zone = ZoneManager.Instance.GetZone(nZoneID);
				if (zone == null || zone.IsOutdoor != !isIndoor)
				{
					continue;
				}

				FireEquipment equip = new FireEquipment();

				equip.ID = nID;
				equip.GroupID = -1;

				equip.Description = strDescription;
				equip.EquipID = strEquipID;
				equip.RFIDTag = strRFIDTag;
                equip.TagID = strRFIDTagID;
				equip.SetType(type);

				equip.Zone = zone;
				equip.X = x;
				equip.Y = 0.1f;
				equip.Z = y;

				float dx = 0;
				float dz = 0;
				if (zone.IsOutdoor == false)
				{
					UnE.Geometry.Vertex2D posMin = zone.Polygon.GetMin();
					UnE.Geometry.Vertex2D posMax = zone.Polygon.GetMax();
					dx = (float)(posMax.x + posMin.x) / 2.0f;
					dz = (float)(posMax.y + posMin.y) / 2.0f;
					float pos3DX = x - dx;
					float pos3DZ = dz - y;
					equip.X = pos3DX;
					equip.Y = 0.5f;
					equip.Z = pos3DZ;
				}
				FireAlarm senor = new FireAlarm();
				if (!m_dicAlarmStation.ContainsKey(nID))
				{
					m_dicAlarmStation.Add(nID, senor);
				}
				senor = (FireAlarm)m_dicAlarmStation[nID];
                //senor.SensorName = szName;
				senor.OrgSensorID = nID;
				senor.ID = nID;
				senor.POI = new POI();
				senor.POI.X = equip.X;
				senor.POI.Y = equip.Y;
				senor.POI.Z = equip.Z;
				senor.POI.Zone = zone;
				senor.POI.IsIndoor = isIndoor;
				senor.Description = strDescription;
				senor.AlarmStation = equip;
				senor.POI.Facility = senor;
				view.AddPOI(senor.POI);
			}
			return true;
		}

        public ISensor GetSensorZone(int nSensorZoneID)
        {
            ISensor sensor;

            if (m_dicAllSenor.TryGetValue(nSensorZoneID, out sensor))
                return sensor;

            return null;
        }

        public List<ISensor> GetPSMSensorZone(int nPSMSensorID)
        {
            //UnE.PSM.PSMSensor sensor = PSMManager.Instance.GetSensor(nPSMSensorID);

            //if (sensor != null)
            //    return sensor.SensorZone;

            //return null;


            List<ISensor> arResult = new List<ISensor>();

            if (m_dicPSMSensorZone == null)
                return null;

            foreach (UnE.PSM.PSMSensorZone sensor in m_dicPSMSensorZone.Values)
            {
                if(sensor.OrgSensorID == nPSMSensorID )
                {
                    arResult.Add(sensor);
                }
            }
            return arResult;
        }


        private List<ISensor> mEditSensorList = new List<ISensor>();
        public List<ISensor> EditSensorList
        {
            get { return mEditSensorList; }
        }

        public void BeginEditSensor(ISensor sensor)
        {
            if(!mEditSensorList.Contains(sensor))
            {
                mEditSensorList.Add(sensor);
            }
        }

        public void EndEditSensor(ISensor sensor)
        {
            if (mEditSensorList.Contains(sensor))
            {
                mEditSensorList.Remove(sensor);
            }
        }
	}
}