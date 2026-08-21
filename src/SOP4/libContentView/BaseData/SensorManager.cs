using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using DBUtility;
using UnE.Spatial;
using UnE.Sensor;


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

		private Hashtable m_dicFireSenor = new Hashtable();
        public Hashtable DicFireSensor
        {
            get { return m_dicFireSenor; }
        }

        private Hashtable m_dicSecuritySensor = new Hashtable();
        public Hashtable DicSecuritySensor
        {
            get { return m_dicSecuritySensor; }
        }

        private Hashtable m_dicSecuritySecomExSensor = new Hashtable();
        public Hashtable DicSecuritySecomExSensor
        {
            get { return m_dicSecuritySecomExSensor; }
        }

        private Hashtable m_dicSecuritySecomWomanSensor = new Hashtable();
        public Hashtable DicSecuritySecomWomanSensor
        {
            get { return m_dicSecuritySecomWomanSensor; }
        }

        private Hashtable m_dicSmokeSenor = new Hashtable();

        public Hashtable DicSmokeSensor
        {
            get { return m_dicSmokeSenor; }
        }

		private Hashtable m_dicSpringCooler = new Hashtable();

		public Hashtable DicSpringCooler
		{
			get { return m_dicSpringCooler; }
		}

		private Hashtable m_dicPressureSensor = new Hashtable();

		public Hashtable DicPressureSensor
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
            LoadFireSesnsor();
            
            //LoadS1Access();

            //LoadS1SVMS();

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
                
                ArrayList arrResult = webDB.GetResultData(szSQL, 0);
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

                        ISensor sensor = null;
                        if (nSensorType == (int)IFacility.FacilityType.FIRE_SENSOR || nSensorType == (int)IFacility.FacilityType.FireSensor_TypeA || nSensorType == (int)IFacility.FacilityType.FireSensor_SiemensType)
                        {
                            FireSensor fs = null;
                            if (m_dicFireSenor.ContainsKey(nOrgSensroID))
                            {
                                fs = (FireSensor)m_dicFireSenor[nOrgSensroID];
                            }
                            else
                                fs = new FireSensor();
                            sensor = fs;
                        }
                        else if (nSensorType == (int)IFacility.FacilityType.FireSensor_AnalogSmokeType)
                        {
                            SmokeSensor fs = null;
                            if (m_dicSmokeSenor.ContainsKey(nOrgSensroID))
                            {
                                fs = (SmokeSensor)m_dicSmokeSenor[nOrgSensroID];
                            }
                            else
                                fs = new SmokeSensor();
                            sensor = fs;
                        }
                        else if (nSensorType == (int)IFacility.FacilityType.COOLER_SENSOR)
                        {
                            SpringCooler fs = null;
                            if (m_dicSpringCooler.ContainsKey(nOrgSensroID))
                            {
                                fs = (SpringCooler)m_dicSpringCooler[nOrgSensroID];
                            }
                            else
                                fs = new SpringCooler();
                            sensor = fs;
                        }
                        else if (nSensorType == (int)IFacility.FacilityType.PRESSURE_SENSOR)
                        {
                            PumpPressureSensor fs = null;
                            if (m_dicPressureSensor.ContainsKey(nOrgSensroID))
                            {
                                fs = (PumpPressureSensor)m_dicPressureSensor[nOrgSensroID];
                            }
                            else
                                fs = new PumpPressureSensor();
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
                        else
                        {
                            continue;
                        }

                        sensor.OrgSensorID = nOrgSensroID;
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
                ArrayList arrResult = webDB.GetResultData(szSQL, 0);
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

                        ISensor sensor = null;
                        if (nSensorType == (int)IFacility.FacilityType.FIRE_SENSOR || nSensorType == (int)IFacility.FacilityType.FireSensor_TypeA || nSensorType == (int)IFacility.FacilityType.FireSensor_SiemensType)
                        {
                            FireSensor fs = null;
                            if (m_dicFireSenor.ContainsKey(nOrgSensroID))
                            {
                                fs = (FireSensor)m_dicFireSenor[nOrgSensroID];
                            }
                            else
                                fs = new FireSensor();
                            sensor = fs;
                        }
                        else if (nSensorType == (int)IFacility.FacilityType.FireSensor_AnalogSmokeType)
                        {
                            SmokeSensor fs = null;
                            if (m_dicSmokeSenor.ContainsKey(nOrgSensroID))
                            {
                                fs = (SmokeSensor)m_dicSmokeSenor[nOrgSensroID];
                            }
                            else
                                fs = new SmokeSensor();
                            sensor = fs;
                        }
                        else if (nSensorType == (int)IFacility.FacilityType.COOLER_SENSOR)
                        {
                            SpringCooler fs = null;
                            if (m_dicSpringCooler.ContainsKey(nOrgSensroID))
                            {
                                fs = (SpringCooler)m_dicSpringCooler[nOrgSensroID];
                            }
                            else
                                fs = new SpringCooler();
                            sensor = fs;
                        }
                        else if (nSensorType == (int)IFacility.FacilityType.PRESSURE_SENSOR)
                        {
                            PumpPressureSensor fs = null;
                            if (m_dicPressureSensor.ContainsKey(nOrgSensroID))
                            {
                                fs = (PumpPressureSensor)m_dicPressureSensor[nOrgSensroID];
                            }
                            else
                                fs = new PumpPressureSensor();
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
                        else
                        {
                            continue;
                        }

                        sensor.OrgSensorID = nOrgSensroID;
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

		public void ReadSensorZone()
		{
			try
			{
				m_Mutex.WaitOne();
				
                //string szSQP = "SELECT ID, Type, Connected, EquipZoneID, Data, Description, OrgSensorID FROM SensorZone";
                string szText = "SELECT sz.ID, sz.Type, sz.Connected, sz.EquipZoneID, sz.Data, sz.Description, sz.OrgSensorID FROM SensorZone sz " +
                                "  INNER JOIN EquipmentZone as ez ON ez.ID = sz.EquipZoneID and ez.SiteID = {0}";

                string szSQL = string.Format(szText, m_nSiteID);

                UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
                WebDBManager webDB = owner.DBManager;
				ArrayList arrResult = webDB.GetResultData(szSQL, 0);
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

						ISensor sensor = null;
                        if (nSensorType == (int)IFacility.FacilityType.FIRE_SENSOR || nSensorType == (int)IFacility.FacilityType.FireSensor_TypeA || nSensorType == (int)IFacility.FacilityType.FireSensor_SiemensType)
						{
							FireSensor fs = null;
                            if (m_dicFireSenor.ContainsKey(nOrgSensroID))
                            {
                                fs = (FireSensor)m_dicFireSenor[nOrgSensroID];
                            }
                            else
                            {
                                fs = new FireSensor();
                                
                                fs.OrgSensorID = nOrgSensroID;
                                m_dicFireSenor[nOrgSensroID] = fs;
                                //LoadFireSensorName(fs);
                            }

                            EquipmentZone eq = ZoneManager.Instance.GetEquipZone(nZoneID);

                            if (eq != null)
                            {
                                fs.PositionName = eq.DisplayText;
                            }


							sensor = fs;
						}
                        else if (nSensorType == (int)IFacility.FacilityType.FireSensor_AnalogSmokeType)
                        {
                            SmokeSensor fs = null;
                            if (m_dicSmokeSenor.ContainsKey(nOrgSensroID))
                            {
                                fs = (SmokeSensor)m_dicSmokeSenor[nOrgSensroID];
                            }
                            else
                            {
                                fs = new SmokeSensor();
                                EquipmentZone eq = ZoneManager.Instance.GetEquipZone(nZoneID);

                                if (eq != null)
                                {
                                    fs.PositionName = eq.DisplayText;
                                    fs.OrgSensorID = nOrgSensroID;
                                    LoadFireSensorName(fs);
                                }

                                m_dicSmokeSenor[nOrgSensroID] = fs;
                            }
                            sensor = fs;
                        }
                        else if (nSensorType == (int)IFacility.FacilityType.COOLER_SENSOR)
                        {
                            SpringCooler fs = null;
                            if (m_dicSpringCooler.ContainsKey(nOrgSensroID))
                            {
                                fs = (SpringCooler)m_dicSpringCooler[nOrgSensroID];
                            }
                            else
                            {
                                fs = new SpringCooler();
                                m_dicSpringCooler[nOrgSensroID] = fs;
                            }
                            sensor = fs;
                        }
                        else if (nSensorType == (int)IFacility.FacilityType.PRESSURE_SENSOR)
                        {
                            PumpPressureSensor fs = null;
                            if (m_dicPressureSensor.ContainsKey(nOrgSensroID))
                            {
                                fs = (PumpPressureSensor)m_dicPressureSensor[nOrgSensroID];
                            }
                            else
                            {
                                fs = new PumpPressureSensor();
                                m_dicPressureSensor[nOrgSensroID] = fs;
                            }
                            sensor = fs;
                        }
                        else if (nSensorType == (int)IFacility.FacilityType.FireSensor_ManualControl)
                        {
                            continue;
                            //FireAlarm fs = null;
                            //if (m_dicAlarmStation.ContainsKey(nOrgSensroID))
                            //{
                            //    fs = (FireAlarm)m_dicAlarmStation[nOrgSensroID];
                            //}
                            //else
                            //    fs = new FireAlarm();
                            //sensor = fs;
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
                             
                                psz.OrgSensor = owner.GetPSMSensor(nOrgSensroID);
                                if(psz.OrgSensor != null)
                                {
                                    if( psz.OrgSensor.LinkedTankList != null && psz.OrgSensor.LinkedTankList.Count > 0)
                                    {
                                        psz.PositionName = psz.OrgSensor.LinkedTankList[0].LocationName;
                                    }
                                }

                              //  m_dicPSMSensorZone[nID] = psz;
                            }

                            sensor = psz;

                            m_dicPSMSensorZone.Add(nID, psz);
                        }
                        else if( nSensorType == (int)IFacility.FacilityType.FireF1_S1 || 
                                 nSensorType == (int)IFacility.FacilityType.Fire_S1 ||
                                 nSensorType == (int)IFacility.FacilityType.SecomFire)
                        {
                            FireSensor fs = null;
                            if (m_dicFireSenor.ContainsKey(nOrgSensroID))
                            {
                                fs = (FireSensor)m_dicFireSenor[nOrgSensroID];
                            }
                            else
                            {
                                fs = new FireSensor();

                                fs.OrgSensorID = nOrgSensroID;
                                //LoadFireSensorName(fs);
                                m_dicFireSenor[nOrgSensroID] = fs;
                            }

                            EquipmentZone eq = ZoneManager.Instance.GetEquipZone(nZoneID);
                            if (eq != null)
                            {
                                fs.PositionName = eq.DisplayText;
                            }
                            sensor = fs;
                        }
                        else if (nSensorType >= (int)IFacility.FacilityType.Intrusion_S1 && nSensorType <= (int)IFacility.FacilityType.ExternalAlarmBell)
                        {
                            SecuritySensor fs = null;
                            if (m_dicSecuritySensor.ContainsKey(nOrgSensroID))
                            {
                                fs = (SecuritySensor)m_dicSecuritySensor[nOrgSensroID];
                            }
                            else
                            {
                                fs = new SecuritySensor();
                                fs.SubType = IFacility.ToFacilityType(nSensorType);
                                fs.OrgSensorID = nOrgSensroID;
                                //LoadFireSensorName(fs);
                                m_dicSecuritySensor[nOrgSensroID] = fs;
                            }

                            EquipmentZone eq = ZoneManager.Instance.GetEquipZone(nZoneID);
                            if (eq != null)
                            {
                                fs.PositionName = eq.DisplayText;
                            }
                            sensor = fs;
                        }
                        /**
                        ** orgSensorID 가 다르므로 추가. by hypark 2018.06.19
                        */
                        else if (nSensorType >= (int)IFacility.FacilityType.SecomWomenAlarmBell)
                        {
                            SecuritySensor fs = null;
                            if (m_dicSecuritySecomWomanSensor.ContainsKey(nOrgSensroID))
                            {
                                fs = (SecuritySensor)m_dicSecuritySecomWomanSensor[nOrgSensroID];
                            }
                            else
                            {
                                fs = new SecuritySensor();
                                fs.SubType = IFacility.ToFacilityType(nSensorType);
                                fs.OrgSensorID = nOrgSensroID;
                                //LoadFireSensorName(fs);
                                m_dicSecuritySecomWomanSensor[nOrgSensroID] = fs;
                            }

                            EquipmentZone eq = ZoneManager.Instance.GetEquipZone(nZoneID);
                            if (eq != null)
                            {
                                fs.PositionName = eq.DisplayText;
                            }
                            sensor = fs;
                        }
                        else if (nSensorType >= (int)IFacility.FacilityType.SecomExternalAlarmBell)
                        {
                            SecuritySensor fs = null;
                            if (m_dicSecuritySecomExSensor.ContainsKey(nOrgSensroID))
                            {
                                fs = (SecuritySensor)m_dicSecuritySecomExSensor[nOrgSensroID];
                            }
                            else
                            {
                                fs = new SecuritySensor();
                                fs.SubType = IFacility.ToFacilityType(nSensorType);
                                fs.OrgSensorID = nOrgSensroID;
                                //LoadFireSensorName(fs);
                                m_dicSecuritySecomExSensor[nOrgSensroID] = fs;
                            }

                            EquipmentZone eq = ZoneManager.Instance.GetEquipZone(nZoneID);
                            if (eq != null)
                            {
                                fs.PositionName = eq.DisplayText;
                            }
                            sensor = fs;
                        }
                        else
                        {
                            continue;
                        }

						sensor.OrgSensorID = nOrgSensroID;
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

                        EquipmentZoneObjectList zoneSensor = GetZoneInSensor(nZoneID);						

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
			if (!LoadFireSesnsor(view, isIndoor))
				return false;

			if (!LoadSpringCooler(view, isIndoor))
				return false;

			if (!LoadPumpPressuerSensor(view, isIndoor))
				return false;

            if (!LoadSmokeSensor(view, isIndoor))
                return false;
			//if (!LoadAlarmStation(view, isIndoor))
			//	return false;

			return true;
		}

        private bool LoadAlarmStation(ISensorTooltipOwner view, bool isIndoor)
		{
            UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
            DBUtility.WebDBManager dbMgr = owner.DBManager;

			//string strSQL = "Select ID, RFIDTag, EquipID, RFIDTagID, DxfObjID, EquipType, ZoneID, X, Y, Z, Description from FireEquipment where EquipType = 3";
            string szText = "Select fe.ID, fe.RFIDTag, fe.EquipID, fe.RFIDTagID, fe.DxfObjID, fe.EquipType, fe.ZoneID, fe.X, fe.Y, fe.Z, fe.Description from FireEquipment as fe "+
                            " INNER JOIN Zone as z on z.ID = fe.ZoneID and z.SiteID = {0} and fe.EquipType = " + ((int)IFacility.FacilityType.FA).ToString();

            string strSQL = string.Format(szText, m_nSiteID);

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

			if (arrResult == null)
				return false;

			int nResultCount = arrResult.Count;

			for (int i = 0; i < nResultCount - 10; i += 11)
			{
				int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
				string strRFIDTag = DBUtility.WebDBManager.GetStringField(arrResult[i + 1], "");
				string strEquipID = DBUtility.WebDBManager.GetStringField(arrResult[i + 2], "");
				string strRFIDTagID = DBUtility.WebDBManager.GetStringField(arrResult[i + 3], "");
				string strDxfObjID = DBUtility.WebDBManager.GetStringField(arrResult[i + 4], "");
				int nEquipType = DBUtility.WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
				int nZoneID = DBUtility.WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
				float x = DBUtility.WebDBManager.GetFloatField(arrResult[i + 7].ToString(), 0.0f);
				float y = DBUtility.WebDBManager.GetFloatField(arrResult[i + 8].ToString(), 0.0f);
				float z = DBUtility.WebDBManager.GetFloatField(arrResult[i + 9].ToString(), 0.0f);
				string strDescription = DBUtility.WebDBManager.GetStringField(arrResult[i + 10], "");

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

        private void LoadFireSensorName(FireSensor sensor)
        {

            string szText = "SELECT fs.Name FROM FireSensor as fs " +
                           " WHERE fs.ID = {0}";

            string szSQL = string.Format(szText, sensor.OrgSensorID);

            WebDBManager webDB = UnE.View.Content.ViewUtils.GetContentViewOwner().DBManager;
            ArrayList arrResult = webDB.GetResultData(szSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
            {
                return;
            }

            sensor.SensorName = arrResult[0].ToString();
        }

        private bool LoadFireSesnsor()
        {
            string szText = "SELECT fs.ID, fs.Name, fs.PositionName, fs.X, fs.Y, fs.Z, fs.ZoneID, fs.IsIndoor, fs.Description FROM FireSensor as fs " +
                            " INNER JOIN Zone as z on z.ID = fs.ZoneID and z.SiteID = {0}";

            string szSQL = string.Format(szText, m_nSiteID);

            UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
            WebDBManager webDB = owner.DBManager;
            ArrayList arrResult = webDB.GetResultData(szSQL, 0);
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
                string szPosName = WebDBManager.GetStringField(arrResult[i + 2], "");
                float fx = WebDBManager.GetFloatField(arrResult[i + 3].ToString(), 0.0f);
                float fy = WebDBManager.GetFloatField(arrResult[i + 4].ToString(), 0.0f);
                float fz = WebDBManager.GetFloatField(arrResult[i + 5].ToString(), 0.0f);
                int nZoneID = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);

                int nIndoor = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0);
                //string szIndoor = WebDBManager.GetStringField(arrResult[i + 7], "null");
                string szDesc = WebDBManager.GetStringField(arrResult[i + 8], "");

                if (m_dicFireSenor.ContainsKey(nID))
                    continue;

                if (nZoneID == -1)
                    continue;
                Zone zone = ZoneManager.Instance.GetZone(nZoneID);
                if (zone == null)
                {
                    continue;
                }

                FireSensor senor = new FireSensor();
                //if (!m_dicFireSenor.ContainsKey(nID))
                {
                    m_dicFireSenor.Add(nID, senor);
                }
                senor = (FireSensor)m_dicFireSenor[nID];
                senor.SensorName = szName;
                senor.PositionName = szPosName;
                senor.OrgSensorID = nID;
                senor.ID = nID;
                senor.POI = new POI();
                senor.POI.X = fx;
                senor.POI.Y = fy;
                senor.POI.Z = fz;
                senor.POI.Zone = zone;
                senor.POI.IsIndoor = nIndoor == 0 ? false : true;
                senor.Description = szDesc;
                senor.POI.Facility = senor;
            }
            return true;
        }

        private bool LoadFireSesnsor(ISensorTooltipOwner view, bool isIndoor)
		{			
            
            //string szSQP = "SELECT ID, Name, PositionName, X, Y, Z, ZoneID, IsIndoor, Description FROM FireSensor where IsIndoor = " + (isIndoor ? "1" : "0");

            string szText = "SELECT fs.ID, fs.Name, fs.PositionName, fs.X, fs.Y, fs.Z, fs.ZoneID, fs.IsIndoor, fs.Description FROM FireSensor as fs " +
                            " INNER JOIN Zone as z on z.ID = fs.ZoneID and z.SiteID = {0} and fs.IsIndoor = {1}";

            string szSQL = string.Format(szText, m_nSiteID, (isIndoor ? "1" : "0"));

            UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
            WebDBManager webDB = owner.DBManager;
			ArrayList arrResult = webDB.GetResultData(szSQL, 0);
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
				string szPosName = WebDBManager.GetStringField(arrResult[i + 2], "");
				float fx = WebDBManager.GetFloatField(arrResult[i + 3].ToString(), 0.0f);
				float fy = WebDBManager.GetFloatField(arrResult[i + 4].ToString(), 0.0f);
				float fz = WebDBManager.GetFloatField(arrResult[i + 5].ToString(), 0.0f);
				int nZoneID = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);

				string szIndoor = WebDBManager.GetStringField(arrResult[i + 7], "null");
				string szDesc = WebDBManager.GetStringField(arrResult[i + 8], "");

                if (m_dicFireSenor.ContainsKey(nID))
                    continue;

                //if (nZoneID == -1)
                //    continue;
				Zone zone = ZoneManager.Instance.GetZone(nZoneID);
				//if (zone == null || zone.IsOutdoor != !isIndoor)
				//{
				//	continue;
				//}

				FireSensor senor = new FireSensor();
				//if (!m_dicFireSenor.ContainsKey(nID))
				{
					m_dicFireSenor.Add(nID, senor);
				}
				senor = (FireSensor)m_dicFireSenor[nID];
                senor.SensorName = szName;
                senor.PositionName = szPosName;
				senor.OrgSensorID = nID;
				senor.ID = nID;
				senor.POI = new POI();
				senor.POI.X = fx;
				senor.POI.Y = fy;
				senor.POI.Z = fz;
				senor.POI.Zone = zone;
				senor.POI.IsIndoor = isIndoor;
				senor.Description = szDesc;
				senor.POI.Facility = senor;

                
				//view.AddPOI(senor.POI);
			}
			return true;
		}

        private bool LoadSmokeSensor(ISensorTooltipOwner view, bool isIndoor)
        {

            //string szSQP = "SELECT ID, Name, PositionName, X, Y, Z, ZoneID, IsIndoor, Description FROM FireSensor where IsIndoor = " + (isIndoor ? "1" : "0");

            string szText = "SELECT fs.ID, fs.Name, fs.PositionName, fs.X, fs.Y, fs.Z, fs.ZoneID, fs.IsIndoor, fs.Description FROM AnalogSmokeTypeSensor as fs " +
                            " INNER JOIN Zone as z on z.ID = fs.ZoneID and z.SiteID = {0} and fs.IsIndoor = {1}";

            string szSQL = string.Format(szText, m_nSiteID, (isIndoor ? "1" : "0"));

            UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
            WebDBManager webDB = owner.DBManager;
            ArrayList arrResult = webDB.GetResultData(szSQL, 0);
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
                string szPosName = WebDBManager.GetStringField(arrResult[i + 2], "");
                float fx = WebDBManager.GetFloatField(arrResult[i + 3].ToString(), 0.0f);
                float fy = WebDBManager.GetFloatField(arrResult[i + 4].ToString(), 0.0f);
                float fz = WebDBManager.GetFloatField(arrResult[i + 5].ToString(), 0.0f);
                int nZoneID = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);

                string szIndoor = WebDBManager.GetStringField(arrResult[i + 7], "null");
                string szDesc = WebDBManager.GetStringField(arrResult[i + 8], "");

                Zone zone = ZoneManager.Instance.GetZone(nZoneID);
                //if (zone == null || zone.IsOutdoor != !isIndoor)
                //{
                //    continue;
                //}

                SmokeSensor senor = new SmokeSensor();
                if (!m_dicSmokeSenor.ContainsKey(nID))
                {
                    m_dicSmokeSenor.Add(nID, senor);
                }
                senor = (SmokeSensor)m_dicSmokeSenor[nID];
                senor.SensorName = szName;
                senor.PositionName = szPosName;
                senor.OrgSensorID = nID;
                senor.ID = nID;
                senor.POI = new POI();
                senor.POI.X = fx;
                senor.POI.Y = fy;
                senor.POI.Z = fz;
                senor.POI.Zone = zone;
                senor.POI.IsIndoor = isIndoor;
                senor.Description = szDesc;
                senor.POI.Facility = senor;
                //view.AddPOI(senor.POI);
            }
            return true;
        }

        public bool LoadPumpPressuerSensor(ISensorTooltipOwner view, bool isIndoor)
		{
			//string szSQP = "SELECT ID, Name, PositionName, X, Y, Z, ZoneID, IsIndoor, Description FROM PumpPressureSensor where IsIndoor = " + (isIndoor ? "1" : "0");

            string szText = "SELECT pps.ID, pps.Name, pps.PositionName, pps.X, pps.Y, pps.Z, pps.ZoneID, pps.IsIndoor, pps.Description FROM PumpPressureSensor AS pps " +
                            " INNER JOIN Zone AS z ON z.ID = pps.ZoneID AND z.SiteID = {0} AND pps.IsIndoor = {1}";

            string szSQL = string.Format(szText, m_nSiteID, (isIndoor ? "1" : "0"));

            UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
            WebDBManager webDB = owner.DBManager;
			ArrayList arrResult = webDB.GetResultData(szSQL, 0);
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
				if (zone == null || zone.IsOutdoor != !isIndoor)
				{
					continue;
				}

				PumpPressureSensor senor = new PumpPressureSensor();
				if (!m_dicPressureSensor.ContainsKey(nID))
				{
					m_dicPressureSensor.Add(nID, senor);
				}
				senor = (PumpPressureSensor)m_dicPressureSensor[nID];
                //senor.SensorName = szName;
				senor.OrgSensorID = nID;
				senor.ID = nID;
				senor.POI = new POI();
				senor.POI.X = fx;
				senor.POI.Y = fy;
				senor.POI.Z = fz;
				senor.POI.Zone = zone;
				senor.POI.IsIndoor = isIndoor;
				senor.Description = szDesc;
				senor.POI.Facility = senor;

				view.AddPOI(senor.POI);
			}
			return true;
		}

        public bool LoadSpringCooler(ISensorTooltipOwner view, bool isIndoor)
		{
			//string szSQP = "SELECT ID, Name, PositionName, X, Y, Z, ZoneID, IsIndoor, Description FROM SpringCooler where IsIndoor = " + (isIndoor ? "1" : "0");
            string szText = "SELECT sc.ID, sc.Name, sc.PositionName, sc.X, sc.Y, sc.Z, sc.ZoneID, sc.IsIndoor, sc.Description FROM SpringCooler as sc " +
                            " INNER JOIN Zone AS z ON z.ID = sc.ZoneID AND z.SiteID = {0} AND sc.IsIndoor = {1}";

            string szSQL = string.Format(szText, m_nSiteID, (isIndoor ? "1" : "0"));

            UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
			WebDBManager webDB = owner.DBManager;
            ArrayList arrResult = webDB.GetResultData(szSQL, 0);
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
				if (zone == null || zone.IsOutdoor != !isIndoor)
				{
					continue;
				}

				SpringCooler senor = new SpringCooler();
				if (!m_dicSpringCooler.ContainsKey(nID))
				{
					m_dicSpringCooler.Add(nID, senor);
				}

				senor = (SpringCooler)m_dicSpringCooler[nID];
				senor.OrgSensorID = nID;
				senor.ID = nID;
				senor.POI = new POI();
				senor.POI.X = fx;
				senor.POI.Y = fy;
				senor.POI.Z = fz;
				senor.POI.Zone = zone;
				senor.POI.IsIndoor = isIndoor;
				senor.Description = szDesc;
				senor.POI.Facility = senor;

				if (!m_dicSpringCooler.ContainsKey(nID))
				{
					m_dicSpringCooler.Add(nID, senor);
				}
				view.AddPOI(senor.POI);
			}
			return true;
		}
                
        /*private bool LoadS1SVMS()
        {
            string szText = "SELECT fs.ID, fs.Name, fs.PositionName, fs.X, fs.Y, fs.Z, fs.ZoneID, fs.IsIndoor, fs.Description FROM S1Access as fs " +
                            " INNER JOIN Zone as z on z.ID = fs.ZoneID and z.SiteID = {0}";

            string szSQL = string.Format(szText, m_nSiteID);

            UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
            WebDBManager webDB = owner.DBManager;
            ArrayList arrResult = webDB.GetResultData(szSQL, 0);
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
                string szPosName = WebDBManager.GetStringField(arrResult[i + 2], "");
                float fx = WebDBManager.GetFloatField(arrResult[i + 3].ToString(), 0.0f);
                float fy = WebDBManager.GetFloatField(arrResult[i + 4].ToString(), 0.0f);
                float fz = WebDBManager.GetFloatField(arrResult[i + 5].ToString(), 0.0f);
                int nZoneID = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);

                int nIndoor = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0);
                //string szIndoor = WebDBManager.GetStringField(arrResult[i + 7], "null");
                string szDesc = WebDBManager.GetStringField(arrResult[i + 8], "");

                if (m_dicFireSenor.ContainsKey(nID))
                    continue;

                if (nZoneID == -1)
                    continue;
                Zone zone = ZoneManager.Instance.GetZone(nZoneID);
                if (zone == null)
                {
                    continue;
                }

                SecuritySensor senor = new SecuritySensor();
                //if (!m_dicFireSenor.ContainsKey(nID))
                {
                    m_dicFireSenor.Add(nID, senor);
                }
                senor = (SecuritySensor)m_dicFireSenor[nID];
                senor.SensorName = szName;
                senor.PositionName = szPosName;
                senor.OrgSensorID = nID;
                senor.ID = nID;
                senor.POI = new POI();
                senor.POI.X = fx;
                senor.POI.Y = fy;
                senor.POI.Z = fz;
                senor.POI.Zone = zone;
                senor.POI.IsIndoor = nIndoor == 0 ? false : true;
                senor.Description = szDesc;
                senor.POI.Facility = senor;
            }
            return true;
        }

        private bool LoadS1Access()
        {
            string szText = "SELECT fs.ID, fs.Name, fs.PositionName, fs.X, fs.Y, fs.Z, fs.ZoneID, fs.IsIndoor, fs.Description FROM S1Access as fs " +
                            " INNER JOIN Zone as z on z.ID = fs.ZoneID and z.SiteID = {0}";

            string szSQL = string.Format(szText, m_nSiteID);

            UnE.View.Content.IFormContentOwner owner = UnE.View.Content.ViewUtils.GetContentViewOwner();
            WebDBManager webDB = owner.DBManager;
            ArrayList arrResult = webDB.GetResultData(szSQL, 0);
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
                string szPosName = WebDBManager.GetStringField(arrResult[i + 2], "");
                float fx = WebDBManager.GetFloatField(arrResult[i + 3].ToString(), 0.0f);
                float fy = WebDBManager.GetFloatField(arrResult[i + 4].ToString(), 0.0f);
                float fz = WebDBManager.GetFloatField(arrResult[i + 5].ToString(), 0.0f);
                int nZoneID = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);

                int nIndoor = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0);
                //string szIndoor = WebDBManager.GetStringField(arrResult[i + 7], "null");
                string szDesc = WebDBManager.GetStringField(arrResult[i + 8], "");

                if (m_dicFireSenor.ContainsKey(nID))
                    continue;

                if (nZoneID == -1)
                    continue;
                Zone zone = ZoneManager.Instance.GetZone(nZoneID);
                if (zone == null)
                {
                    continue;
                }

                SecuritySensor senor = new SecuritySensor();
                //if (!m_dicFireSenor.ContainsKey(nID))
                {
                    m_dicFireSenor.Add(nID, senor);
                }
                senor = (SecuritySensor)m_dicFireSenor[nID];
                senor.SensorName = szName;
                senor.PositionName = szPosName;
                senor.OrgSensorID = nID;
                senor.ID = nID;
                senor.POI = new POI();
                senor.POI.X = fx;
                senor.POI.Y = fy;
                senor.POI.Z = fz;
                senor.POI.Zone = zone;
                senor.POI.IsIndoor = nIndoor == 0 ? false : true;
                senor.Description = szDesc;
                senor.POI.Facility = senor;
            }
            return true;
        }*/
        
        public FireSensor GetFireSensor(int nSensorID)
        {
            if (m_dicFireSenor == null)
                return null;

            FireSensor sensor = null;

            if (m_dicFireSenor.ContainsKey(nSensorID) == true)
            {
                sensor = m_dicFireSenor[nSensorID] as FireSensor;
            }

            return sensor;
        }

        public SmokeSensor GetSmokeSensor(int nSensorID)
        {
            if (m_dicSmokeSenor == null)
                return null;

            SmokeSensor sensor = null;

            if (m_dicSmokeSenor.ContainsKey(nSensorID) == true)
            {
                sensor = m_dicSmokeSenor[nSensorID] as SmokeSensor;
            }

            return sensor;
        }
        
        public PumpPressureSensor GetPumpPressureSensor(int nSensorID)
        {
            if (m_dicPressureSensor == null)
                return null;

            PumpPressureSensor sensor = null;

            if (m_dicPressureSensor.ContainsKey(nSensorID) == true)
            {
                sensor = m_dicPressureSensor[nSensorID] as PumpPressureSensor;
            }

            return sensor;
        }
        
        public SpringCooler GetSpringCoolerSensor(int nSensorID)
        {
            if (m_dicSpringCooler == null)
                return null;

            SpringCooler sensor = null;

            if (m_dicSpringCooler.ContainsKey(nSensorID) == true)
            {
                sensor = m_dicSpringCooler[nSensorID] as SpringCooler;
            }

            return sensor;
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