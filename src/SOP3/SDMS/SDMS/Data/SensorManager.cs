using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using DBUtility;


namespace SDMS
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

        public int FindZoneInSensor(int zoneID, Facility.FacilityType sensortype)
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

            if (sensorZone != null)
            {
                foreach (SensorZone sensor in sensorZone.SensorList)
                {
                    if (sensor.Type == sensortype)
                        return sensor.ID;
                }
            }

            return -1;
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
		
		public void ReadSensorZone()
		{			
			try
			{
				m_Mutex.WaitOne();
				string szSQP = "SELECT ID, Type, Connected, EquipZoneID, Data, Description, OrgSensorID FROM SensorZone";
				WebDBManager webDB = FormMain.Instance.DBManager;
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
						if( nID == 0)
							continue;

						SensorZone sensor = null;
						if (nSensorType == 1)
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
						else if (nSensorType == 2)
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
						else if (nSensorType == 3)
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
						else if (nSensorType == 4)
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
						else if (nSensorType == 10)
						{
							continue;
						}

                        sensor.OrgSensorID = nOrgSensroID;
						sensor.ID = nID;
						sensor.EquipZoneID = nZoneID;
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
							((TooltipSensor)(sensor.POI.Popup)).Sensor = sensor;
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
								Core.BaseView view = sensor.POI.ParentView;
								view.UpdateIcon(sensor.POI.ID, sensor.POI.Facility.DisconnectIconPath);
							}
                        }

						EquipmentZoneObjectList zoneSensor = FindZoneInSensor(nZoneID);
						if (zoneSensor == null)
						{
							zoneSensor = new EquipmentZoneObjectList();
							EquipmentZone zone = ZoneManager.Instance.GetEquipZone(nZoneID);
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

        public bool LoadAllSensor(BaseViewEx view, bool isIndoor)
        {
            if (!LoadFireSesnsor(view, isIndoor))
                return false;

            if (!LoadSpringCooler(view, isIndoor))
                return false;

            if (!LoadPumpPressuerSensor(view, isIndoor))
                return false;

			//if (!LoadAlarmStation(view, isIndoor))
			//	return false;

            return true;
        }

		private bool LoadAlarmStation(BaseViewEx view, bool isIndoor)
		{
			DBUtility.WebDBManager dbMgr = FormMain.Instance.DBManager;

			string strSQL = "Select ID, RFIDTag, EquipID, RFIDTagID, DxfObjID, EquipType, ZoneID, X, Y, Z, Description from FireEquipment where EquipType = 3";
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

				Facility.FacilityType type = Facility.FacilityType.NONE;

				
				type = Facility.FacilityType.FA;
				
				Zone zone = ZoneManager.Instance.GetZone(nZoneID);
				if( zone == null || zone.IsOutdoor != !isIndoor)
				{
					continue;
				}

				FireEquipment equip = new FireEquipment();

				equip.ID = nID;
				equip.GroupID = -1;

				equip.Description = strDescription;
				equip.EquipID = strEquipID;
				equip.RFIDTag = strRFIDTag;
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

        private bool LoadFireSesnsor(BaseViewEx view, bool isIndoor)
        {
            string szSQP = "SELECT ID, Name, PositionName, X, Y, Z, ZoneID, IsIndoor, Description FROM FireSensor where IsIndoor = " + (isIndoor ? "1" : "0");
            WebDBManager webDB = FormMain.Instance.DBManager;
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
				if (zone == null || zone.IsOutdoor != !isIndoor)
				{
					continue;
				}

				FireSensor senor = new FireSensor(); 
                if (!m_dicFireSenor.ContainsKey(nID))
                {
                    m_dicFireSenor.Add(nID, senor);
                }
				senor = (FireSensor)m_dicFireSenor[nID];

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

        public bool LoadPumpPressuerSensor(BaseViewEx view, bool isIndoor)
        {
            string szSQP = "SELECT ID, Name, PositionName, X, Y, Z, ZoneID, IsIndoor, Description FROM PumpPressureSensor where IsIndoor = " + (isIndoor ? "1" : "0");
            WebDBManager webDB = FormMain.Instance.DBManager;
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


        public bool LoadSpringCooler(BaseViewEx view, bool isIndoor)
        {
            string szSQP = "SELECT ID, Name, PositionName, X, Y, Z, ZoneID, IsIndoor, Description FROM SpringCooler where IsIndoor = " + (isIndoor ? "1" : "0");
            WebDBManager webDB = FormMain.Instance.DBManager;
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
	}
}
