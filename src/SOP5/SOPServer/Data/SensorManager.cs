using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using DBUtility;
using System.IO;
using System.Reflection;
using UnE.Sensor;

namespace SDMSServer
{
	public class SensorManager
	{
		private static SensorManager m_Instance = null;
		public static SensorManager Instance
		{
			get
			{
				return m_Instance;
			}
		}

		private DBUtility.WebDBManager m_dbMgr = null;
        private ServiceProvider m_provider = null;

        private int m_nLastReadReactionHistoryID = -1;
        // Key : SensorHistoryID
        // Value : SensorZone ID

        private Dictionary<int, string> dicSensorTagDeactivation = new Dictionary<int, string>();       //TagID, Deactivation        

        private Dictionary<int, int> m_dicSensorHistory = new Dictionary<int, int>();
		public Dictionary<int, int> DicSensorHistory
		{
			get { return m_dicSensorHistory; }
			set { m_dicSensorHistory = value; }
		}

        private int m_nSiteID = 1;
		public SensorManager(DBUtility.WebDBManager dbMgr, ServiceProvider provider)
		{
            m_nSiteID = SDMSServer.NetworkServer.Instance.SiteID;

			m_Instance = this;
			m_dbMgr = dbMgr;
            m_provider = provider;
            ReadFacilityTypes();
            ReadLastLogID();
            LoadAllDeactivationTagsInfo();
		}
        private void LoadAllDeactivationTagsInfo()
        {
            string strSQL = "Select ID, DeActivate from SensorTagInfo";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            int lengthOfResult = arrResult.Count;
            
            if (arrResult == null || lengthOfResult == 0) return;

            for (int i = 0; i < lengthOfResult - 1; i += 2)
            {
                int tagID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string deActivationCode = WebDBManager.GetStringField(arrResult[i + 1]);
                dicSensorTagDeactivation.Add(tagID, deActivationCode);
            }
        }
        private void ReadFacilityTypes()
        {
            string strSQL = "select ID, LinkedTableName from FacilityType";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strTableName = WebDBManager.GetStringField(arrResult[i + 1]);

                if (strTableName == null)
                    continue;

                FacilityManager.SetFacilityTypeTable(nID, strTableName);
            }
        }

        private void ReadLastLogID()
        {
			string szPath = Assembly.GetEntryAssembly().Location;
			string szFullPath = Directory.GetParent(szPath).FullName;

			string strFilePath = szFullPath + "\\LastSensorHistory.log";
            
            if (File.Exists(strFilePath))
            {
                StreamReader reader = new StreamReader(strFilePath, Encoding.Default);
                string strLine = reader.ReadLine();
                reader.Close();

                string strReactionHistoryID = strLine.TrimStart(new char[] { ' ', '\t', '\r', '\n' });
                strReactionHistoryID = strReactionHistoryID.TrimEnd(new char[] { ' ', '\t', '\r', '\n' });

                int.TryParse(strReactionHistoryID, out m_nLastReadReactionHistoryID);
            }
        }

        public void SetLastReadSensorHistoryID(int nHistoryID)
        {
			string szPath = Assembly.GetEntryAssembly().Location;
			string szFullPath = Directory.GetParent(szPath).FullName;
			string strFilePath = szFullPath + "\\LastSensorHistory.log";

            StreamWriter writer = new StreamWriter(strFilePath, false, Encoding.Default);
            writer.Write(nHistoryID);
			
            writer.Close();

			m_nLastReadReactionHistoryID = nHistoryID;
        }

        // nSensorData가 같은 영역내에 존재하는 같은 Type의 Sensor들 가운데 가장 큰 값을 가지는 경우인지 확인한다.
        // Return 값 : true이면 nSensorData가 가장 큰 값이다.
        private bool CheckSensorZoneGroupData(IFacility.FacilityType sensorType, int nSensorZoneID, int nSensorData, SensorZoneGroup group, TimeHistory history, ref int data)
        {
            IOManager ioMgr = NetworkServer.Instance.IOManager;

            SensorZone sensor = ioMgr.GetSensorZone(nSensorZoneID);
            
            if (sensor == null)
                return false;

            if (sensor.IsConnected == false)
            {
                group.SensorDatas[sensor] = null;
                data = -2;
                return false;
            }
            else
            {
                if (nSensorData == (int)UnE.Alarm.AlarmType.NO_ALARM)
                //if (nSensorData == 0 || nSensorData == (int)PSMManager.HistoryDataType.CLEAR_PSM_ALARM)
                {
                    group.SensorDatas[sensor] = null;
                    data = 0;
                    return false;
                }
                else
                    group.SensorDatas[sensor] = nSensorData;
            }

            // nSensorData가 group내에 있는 모든 SensorData들 가운데 가장 큰 값인지 확인한다.
            foreach (KeyValuePair<SensorZone, object> pair in group.SensorDatas)
            {
                // Value가 null인 경우는 접속이 끊어졌거나, Sensor가 사용불능 상태이거나
                // 아니면 아직 데이터 초기화가 안되었음을 의미한다.
                if (pair.Value == null || pair.Key == sensor)
                    continue;

                if (pair.Value is int)
                {
                    int nValue = (int)pair.Value;

                    if (nValue > nSensorData)
                    {
                        int nLastSensorZoneID = GetHistorySensorZoneID(history);

                        // nSensorZoneID가 history와 관련된 Sensor ID라면
                        // history의 값이 nSensorZoneID의 값에서 다른값으로 바뀌어야 한다.
                        if (nSensorZoneID == nLastSensorZoneID)
                        {
                            data = GetMaxDatai(group);
                            return true;
                        }

                        return false;
                    }
                }
            }

            return true;
        }

        private int GetMaxDatai(SensorZoneGroup group)
        {
            int max = -1;

            foreach (KeyValuePair<SensorZone, object> pair in group.SensorDatas)
            {
                if (pair.Value == null)
                    continue;

                if (pair.Value is int)
                {
                    if ((int)pair.Value > max)
                        max = (int)pair.Value;
                }
            }

            return max;
        }

        private int GetHistorySensorZoneID(TimeHistory history)
        {
            if (history == null)
                return -1;

            string strOriginSenzorZoneID = history.LastReactionLog == null ? "" : history.LastReactionLog.Param3;

            int nOriginSensorZoneID;

            if (int.TryParse(strOriginSenzorZoneID, out nOriginSensorZoneID))
                return nOriginSensorZoneID;

            return -1;
        }

        // Return 값 : SensorZoneHistory ID
        public int ProcessSensorData(IFacility.FacilityType sensorType, int nSensorTagInfoID, int nSensorZoneID, int nSensorData, out int outSensorID, out int data, out bool bconnected, ref int nPrevSensorHistoryID)
        {
            outSensorID = -1;
            bconnected = true;
            data = (int)UnE.Alarm.AlarmType.NO_ALARM;

            // 알람발생 신호에 대해서만 센서 비활성화를 검사한다.
            // 이미 알람이 발생한 센서의 경우 센서가 비활성화 상태이더라도 알람을 해제할 수 있어야 한다.
            if (nSensorData > 0)
            {
                if (!IsActiveSensor(nSensorTagInfoID))
                    return -1;
            }
            /*if (IsActiveSensor(nSensorZoneID) == false)
                return -1;*/

            SensorZone sensor = null;
            IOManager ioMgr = NetworkServer.Instance.IOManager;

            sensor = ioMgr.GetSensorZone(nSensorZoneID);

            if (sensor == null)
                return -1;

            int nEquipZoneID = sensor.EquipZone == null ? -1 : sensor.EquipZone.ID;
            int nSensorID = sensor.ID;

            /*if (!ioMgr.D_EquipZoneSensor.ContainsKey(equipZone))
                return -1;

            ArrayList arrSensorZone = ioMgr.D_EquipZoneSensor[equipZone];
            int nSensorID = -1;

            //선택된 센서아이디를 구함
            foreach (SensorZone sensorZone in arrSensorZone)
            {
                if (sensorZone.Type == nSensorType)
                {
                    sensor = sensorZone;
                    nSensorID = sensorZone.ID;
                    break;
                }
            }*/

            if (nSensorID < 0)
                return -1;

            outSensorID = nSensorID;

            int connected = 0;

            // 0 ~ 19 : 화재센서 대역
            // 20 ~ 39 : PSM센서 대역
            if (nSensorData == (int)UnE.Alarm.AlarmType.NO_ALARM)   // 이상 없음
            {
                connected = 1;
                data = (int)UnE.Alarm.AlarmType.NO_ALARM;
            }
            else if (nSensorData == (int)UnE.Alarm.AlarmType.ALARM)  // 상황 발생
            {
                connected = 1;
                data = (int)UnE.Alarm.AlarmType.ALARM;
            }
            else if (nSensorData == (int)UnE.Alarm.AlarmType.NOT_CONNECTED)  // 통신 끊김
            {
                connected = 0;
                data = (int)UnE.Alarm.AlarmType.NO_ALARM;

                if (sensor.IsConnected != (connected == 1))
                {
                    // 센서 접속 정보가 변경되었으므로 DB에 저장한다.
                    string strUpdate1 = "Update SensorZone Set Connected ='" + connected + "', Data = '" + data + "' Where ID ='" + nSensorZoneID + "' and EquipZoneID = '" + nEquipZoneID + "'";
                    m_dbMgr.GetResultData(strUpdate1, 0);

                    sensor.IsConnected = (connected == 1);
                    bconnected = (connected == 1);
                    return -2;
                }
            }
            else if (nSensorData == (int)UnE.Alarm.AlarmType.CONNECTED)  // 통신 연결
            {
                connected = 1;
                data = (int)UnE.Alarm.AlarmType.NO_ALARM;

                if (sensor.IsConnected != (connected == 1))
                {
                    // 센서 접속 정보가 변경되었으므로 DB에 저장한다.
                    string strUpdate1 = "Update SensorZone Set Connected ='" + connected + "', Data = '" + data + "' Where ID ='" + nSensorZoneID + "' and EquipZoneID = '" + nEquipZoneID + "'";
                    m_dbMgr.GetResultData(strUpdate1, 0);

                    sensor.IsConnected = (connected == 1);
                    bconnected = (connected == 1);
                    return -2;
                }
            }
            else if (nSensorData >= (int)UnE.Alarm.AlarmType.PSM_ALARM_1)
            //else if (nSensorData >= (int)PSMManager.HistoryDataType.CLEAR_PSM_ALARM) // PSM Data
            {
                connected = 1;
                data = nSensorData;
            }

            sensor.IsConnected = connected == 1;
            sensor.SensorData = data;
            //SensorZone
            string strUpdate = "Update SensorZone Set Connected ='" + connected + "', Data = '" + data + "' Where ID =" + nSensorZoneID.ToString();
            m_dbMgr.GetResultData(strUpdate, 0);

            // 데이터가 0인경우 해당 센서가 동작중인지 체크
            if (data == (int)UnE.Alarm.AlarmType.NO_ALARM)
            {
                if (!m_provider.CheckSituationForSensorID(nSensorID))
                {
                    return -1;
                }
            }



            SensorZoneGroup group = NetworkServer.Instance.IOManager.GetSensorZoneGroup(sensor.EquipZone, sensorType);

            // 이미 존재하는 값인지 확인해서 존재하는 값이면 더이상 진행하지 않는다.
            //TimeHistory history = GetSensorZoneGroupHistory(group, ref nPrevSensorHistoryID, ref outSensorID);
            
            
            //Equipzone을 그룹에서 체크하게 되면 같은 Equipzone에 있는 센서가 이미 history에 있는 경우 무시 되므로 
            //그룹비교를 생략하고 Sensorzone 아이디가 같은지만 비교하여 이미 존재하는 값이면 더 이상 진행하지 않는다. null리턴.

            TimeHistory history = GetSensorZoneHistory(group, ref nPrevSensorHistoryID, outSensorID);

            // nSensorData가 같은 영역내에 존재하는 같은 Type의 Sensor들 가운데 가장 큰 값을 가지는 경우인지 확인한다.            
            if (!CheckSensorZoneGroupData(sensorType, nSensorZoneID, nSensorData, group, history, ref data))
            {
                if (history != null && history.HistoryID > 0)
                {
                    return history.HistoryID;
                }
                else
                {
                    // data가 0일때 화재센서인경우 서버 재시작시 반드시 history가 null이므로 추가로 체크해본다.
                    if(sensorType == IFacility.FacilityType.FIRE_SENSOR)
                    {
                        int nHistoryID = GetSensorHistoryID(nSensorID, connected == 1, data, ref nPrevSensorHistoryID);
                        if (nHistoryID > 0)
                        {
                            return nHistoryID;
                        }
                    }
                    return -1;
                }
            }
            
            if (history != null && history.HistoryID > 0)
            {
                return history.HistoryID;
            }

            // 무시할 센서인지 확인
            if (AbnormalSensorManager.Instance.Exist(nSensorID) == true)
            {
                return -1;
            }

            //최대ID값 찾기
            //string sqlID = "select max(id) as id from SensorZoneHistory";

            //ArrayList arrResult = m_dbMgr.GetResultData(sqlID, 0);
            //int nResultCount = arrResult.Count;

            //int Max_ID = 0;
            //for (int i = 0; i < nResultCount; i += 1)
            //{
            //    //Data가 아예 안들어가 있을경우 0부터 시작
            //    int Find_Maxid = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), 0);
            //    Max_ID = Find_Maxid;
            //}
            //Max_ID++;

            string szText1 = "SELECT (IFNULL(MAX(ID),0) + 1) as ID FROM SensorZoneHistory";
            ArrayList arResult = m_dbMgr.GetResultData(szText1, 0);
            int Max_ID = DBUtility.WebDBManager.GetIntField(arResult[0].ToString(), 1);

            DateTime dtNow = DateTime.Now;
            string strDateTimeField = string.Format("{0} {1}:{2}:{3}", dtNow.ToShortDateString(), dtNow.Hour, dtNow.Minute, dtNow.Second);

            // 수동화재신고는 SensorZone이 없으므로 EquipZone ID를 Param1에 넣는다.
            string strParam1 = sensor.EquipZone == null ? "NULL" : "'" + sensor.EquipZone.ID.ToString() + "'";
            // Param2에는 센서타입을 넣는다.
            string strParam2 = ((int)sensor.Type).ToString();

            //History
            string sqlInsert = "insert into SensorZoneHistory(ID, SensorID,Connected,Data,Time, param1, param2, SiteID) Values('"
                + Max_ID + "','" + nSensorID + "','" + connected + "','" + data + "','" + strDateTimeField + "'," + strParam1 + ",'" + strParam2 + "','" + m_nSiteID + "')";

            m_dbMgr.GetResultData(sqlInsert, 0);

            m_dicSensorHistory[Max_ID] = nSensorID;

            return Max_ID;
        }

        // nSensorZoneID가 현재 신호처리가 가능한 센서인지 확인한다.
        /*private bool IsActiveSensor(int nSensorZoneID)
        {
            string strSQL = "Select DeActivate from SensorZone where ID = " + nSensorZoneID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return false;

            string str = WebDBManager.GetStringField(arrResult[0]);

            if (str == "Y" || str == "y")
                return false;

            return true;
        }*/

        // nSensorTagID가 현재 신호처리가 가능한 센서인지 확인한다.
        private bool IsActiveSensor(int nSensorTagID)
        {
            if(dicSensorTagDeactivation.ContainsKey(nSensorTagID))
            {
                string code = dicSensorTagDeactivation[nSensorTagID];

                if (code == "Y" || code == "y")
                {
                    return false;
                }
                return true;
            }
            else
            {
                return true;
            }
        }

        public bool updateSensorTagDeactivation(int tagID, string code)
        {
            string strSQL = "Update SensorTagInfo set DeActivate = '" + code + "' where ID = " + tagID;
            ArrayList arResult = m_dbMgr.GetResultData(strSQL, 0);
            if (arResult == null)
                return false;

            if (dicSensorTagDeactivation.ContainsKey(tagID))
            {
                dicSensorTagDeactivation[tagID] = code;
            }
            else
            {
                return false;
            }
            return true;
        }

		public int ProcessSensorData(ArrayList arrDatas, out int outSensorID, out int data, out bool bconnected, ref int nPrevSensorHistoryID, out IFacility.FacilityType sensorType)
		{
            int nSensorType = -1, nSensorTagID = -1, nSensorZoneID = -1, nSensorData = -1;

            outSensorID = data = -1;
            bconnected = false;
            sensorType = IFacility.FacilityType.NONE;

            if (arrDatas.Count >= 4 && arrDatas[0] is int && arrDatas[1] is int && arrDatas[2] is int && arrDatas[3] is int)
            {
                nSensorType = (int)arrDatas[0];
                nSensorTagID = (int)arrDatas[1];
                nSensorZoneID = (int)arrDatas[2];
                nSensorData = (int)arrDatas[3];

                sensorType = IFacility.ToFacilityType(nSensorType);
            }
            else
                return -1;

            /*int nSensorType = BitConverter.ToInt32(bytesSensorData, 11);
            int nSensorTagInfoID = BitConverter.ToInt32(bytesSensorData, 20);
            int nSensorZoneID = BitConverter.ToInt32(bytesSensorData, 29);
            int nSensorData = BitConverter.ToInt32(bytesSensorData, 38);
            sensorType = IFacility.ToFacilityType(nSensorType);*/

            return ProcessSensorData(sensorType, nSensorTagID, nSensorZoneID, nSensorData, out outSensorID, out data, out bconnected, ref nPrevSensorHistoryID);
		}

        public void RemoveSensorHistory(int nSensorHistoryID)
        {
            m_dicSensorHistory.Remove(nSensorHistoryID);
        }


		public bool CheckSituationForManual(SensorReactionLog log)
		{
			
			return false;
		}

		public int GetSensorHistoryIDForManual(int nZoneID, ref int nPrevSensorHistoryID)
		{
			int nHistoryCount = m_provider.GetTimeHistoryCount();
			nPrevSensorHistoryID = -1;
			for (int i = 0; i < nHistoryCount; i++)
			{
				TimeHistory history = m_provider.GetTimeHistory(i);

                if(history.LastReactionLog != null)
                {
                    // 수동신고 ?
                    if (history.LastReactionLog.Param2 == "0")
                    {
                        int nTargetZone = -1;
                        if (int.TryParse(history.LastReactionLog.Param1, out nTargetZone))
                        {

                            if (nZoneID == nTargetZone)
                            {
                                nPrevSensorHistoryID = history.HistoryID;
                                return history.HistoryID;
                            }
                        }
                    }	
                }              			
			}
			return -1;
		}

        // group내에 존재하는 Sensor들에 대한 SensorZoneHistory가 이미 존재하면 해당 History를 리턴하고,
        // 존재하지 않으면 null을 리턴한다.
        private TimeHistory GetSensorZoneGroupHistory(SensorZoneGroup group, ref int nPrevSensorHistoryID, ref int nSensorZoneID)
        {
            int nHistoryCount = m_provider.GetTimeHistoryCount();

            for (int i = 0; i < nHistoryCount; i++)
            {
                TimeHistory history = m_provider.GetTimeHistory(i);

                if (!m_dicSensorHistory.ContainsKey(history.HistoryID))
                    continue;

                int sensorZoneID = m_dicSensorHistory[history.HistoryID];

                SensorZone sensor = NetworkServer.Instance.IOManager.GetSensorZone(sensorZoneID);
                if (sensor == null)
                    continue;

                if (group.SensorDatas.ContainsKey(sensor))
                {
                    nSensorZoneID = sensorZoneID;
                    nPrevSensorHistoryID = history.HistoryID;
                    return history;
                }
            }

            return null;
        }

        private TimeHistory GetSensorZoneHistory(SensorZoneGroup group, ref int nPrevSensorHistoryID, int nSensorZoneID)
        {
            int nHistoryCount = m_provider.GetTimeHistoryCount();

            for (int i = 0; i < nHistoryCount; i++)
            {
                TimeHistory history = m_provider.GetTimeHistory(i);

                if (!m_dicSensorHistory.ContainsKey(history.HistoryID))
                    continue;

                int sensorZoneID = m_dicSensorHistory[history.HistoryID];

                SensorZone sensor = NetworkServer.Instance.IOManager.GetSensorZone(sensorZoneID);
                if (sensor == null)
                    continue;
                if(sensorZoneID == nSensorZoneID)
                {
                    nSensorZoneID = sensorZoneID;
                    nPrevSensorHistoryID = history.HistoryID;
                    return history;
                }
                
            }

            return null;
        }



        // 이미 같은 값이 존재하면 해당 HistoryID를 리턴하고,
        // 존재하지 않으면 -1을 리턴한다.
		public int GetSensorHistoryID(int nSensorID, bool isConnected, int nData, ref int nPrevSensorHistoryID)
		{
			int nHistoryCount = m_provider.GetTimeHistoryCount();

			for (int i = 0; i < nHistoryCount; i++)
			{

				TimeHistory history = m_provider.GetTimeHistory(i);

				if (!m_dicSensorHistory.ContainsKey(history.HistoryID))
					continue;

				int sensorID = m_dicSensorHistory[history.HistoryID];

				if (nSensorID != sensorID)
					continue;

				SensorZone sensor = NetworkServer.Instance.IOManager.GetSensorZone(nSensorID);
				if (sensor == null)
					continue;

				nPrevSensorHistoryID = history.HistoryID;
                //if (sensor.IsConnected == isConnected && sensor.SensorData == nData)
				if (sensor.IsConnected == isConnected)
					return history.HistoryID;
			}

			return -1;
		}

        // Return : EquipmentZone ID
        public int GetEquipmentZoneID(int nSensorID)
		{
            string szSQP = string.Format("SELECT EquipZoneID FROM SensorZone WHERE ID = {0}", nSensorID);

			ArrayList arrResult = m_dbMgr.GetResultData(szSQP, 0);

			if (arrResult == null || arrResult.Count == 0)
				return -1;

			int nID = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), -1);
			return nID;
		}

		public void GetSensorInfo(int nSensorID, out int nType, out int nOrgID)
		{
			nType = 0;
			nOrgID = -1;
			string szSQP = string.Format("SELECT Type, OrgSensorID FROM SensorZone WHERE ID = {0}", nSensorID);
			//1(화재탐지 센서), 2(소화 센서), 3(압력 센서)
			ArrayList arrResult = m_dbMgr.GetResultData(szSQP, 0);

			if (arrResult == null || arrResult.Count < 2)
				return;

			nType = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), -1);
			nOrgID = DBUtility.WebDBManager.GetIntField(arrResult[1].ToString(), -1);

		}

        /// <summary>
        /// 사용하지 않음 skkim 2017-03-20
        /// </summary>
        /// <param name="nOrgID"></param>
        /// <param name="nType"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
		public bool GetSensorLocation(int nOrgID, int nType, out float x, out float y, out float z)
		{
			x = 0.0f;
			y = 0.0f;
			z = 0.0f;
			//1(화재탐지 센서), 2(소화 센서), 3(압력 센서)
			string szSQL = "";

			if (nType == 1)
			{
				szSQL = string.Format("SELECT X, Y, Z FROM FireSensor WHERE ID= {0}", nOrgID);
			}
			else if (nType == 2)
			{
				szSQL = string.Format("SELECT X, Y, Z FROM SpringCooler WHERE ID= {0}", nOrgID);
			}
			else if (nType == 3)
			{
				szSQL = string.Format("SELECT X, Y, Z FROM PumpPressureSensor WHERE ID= {0}", nOrgID);
			}
			else
				return false;

			WebDBManager webDB = NetworkServer.Instance.DBManager;
			ArrayList arrResult = webDB.GetResultData(szSQL, 0);
			int nResultCount = 0;
			if (arrResult == null || arrResult.Count == 0)
			{
				return true;
			}
			nResultCount = arrResult.Count;
			for (int i = 0; i < nResultCount -2; i += 3)
			{
				x = WebDBManager.GetFloatField(arrResult[i + 0].ToString(), 0.0f);
				y = WebDBManager.GetFloatField(arrResult[i + 1].ToString(), 0.0f);
				z = WebDBManager.GetFloatField(arrResult[i + 2].ToString(), 0.0f);
			}
			return true;
		}

        // 현재 Alarm이 발생중인 SensorZone에 대한 Query 조건문
        private string GetAlarmSensorZoneQueryString()
        {
            string strCondition = ((int)UnE.Alarm.AlarmType.ALARM).ToString();

            strCondition += ", " + ((int)UnE.Alarm.AlarmType.PSM_ALARM_1).ToString();
            strCondition += ", " + ((int)UnE.Alarm.AlarmType.PSM_ALARM_2).ToString();
            strCondition += ", " + ((int)UnE.Alarm.AlarmType.PSM_ALARM_3).ToString();
            /*strCondition += ", " + ((int)PSMManager.HistoryDataType.PSM_ALARM_1).ToString();
            strCondition += ", " + ((int)PSMManager.HistoryDataType.PSM_ALARM_2).ToString();
            strCondition += ", " + ((int)PSMManager.HistoryDataType.PSM_ALARM_3).ToString();*/

            return "(" + strCondition + ")";
        }

        // 현재 Alarm이 발생중인 SensorReactionLog에 대한 Query 조건문
        private string GetAlarmReactionHistoryQueryString()
        {
            string strCondition = ((int)libSensorProcess.ReactionType.BEGIN_STATUS).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.BEGIN_PSM_STATUS).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.NOTIFY_FIRE).ToString();

            strCondition += ", " + ((int)libSensorProcess.ReactionType.BEGIN_S1SVMS_STATUS).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.BEGIN_S1ACCESS_STATUS).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.BEGIN_SECOM_STATUS).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.NOTIFY_SECURITY).ToString();

            return "(" + strCondition + ")";
        }

        //현재 Alarm이 꺼진 SensorReactionLog에 대한 Query조건문
        private string GetAlarmOffReactionHistoryQueryString()
        {
            string strCondition = ((int)libSensorProcess.ReactionType.MALFUNCTION).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.IGNORE_FIRE).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.IGNORE_SOP).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.END_STATUS).ToString();
            //strCondition += ", " + ((int)SensorReactionLog.ReactionType.END_PSM_STATUS).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.IGNORE_S1SVMS_STATUS).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.IGNORE_S1ACCESS_STATUS).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.IGNORE_SECOM_STATUS).ToString();

            // strCondition += ", " + ((int)SensorReactionLog.ReactionType.END_S1SVMS_STATUS).ToString();

            return "(" + strCondition + ")";
        }

        // Server가 꺼져있는 동안 발생했던 History 정보를 읽어온다.
        public void ReadSensorHistory(ServiceProvider provider)
        {
            //string strSQL = "SELECT srh.id, srh.SensorHistoryID, srh.ReactionType, srh.Time, srh.Message, srh.Param1, srh.Param2, srh.Param3, srh.Param4, srh.Param5, szh.SensorID " +
            //                            "FROM SensorReactionHistory as srh, SensorZoneHistory as szh, SensorZone as sz " +
            //                            "where  SensorHistoryID in (" +
            //                            "SELECT srh2.SensorHistoryID " +
            //                            "FROM SensorReactionHistory as srh2, SensorZoneHistory as szh2 " +
            //                            "where szh2.Id = srh2.SensorHistoryID and srh2.ReactionType = 0) " +
            //                            "and SensorHistoryID not in (" +
            //                            "SELECT srh3.SensorHistoryID " +
            //                            "FROM SensorReactionHistory as srh3, SensorZoneHistory as szh3 " +
            //                            "where szh3.Id = srh3.SensorHistoryID and srh3.ReactionType in (21, 23, 33, 50)) " +
            //                            "and srh.SensorHistoryID = szh.ID " +
            //                            "and szh.SensorID = sz.ID " +
            //                            "and sz.Data = 1 " +
            //                            "and srh.Time between DATEADD(hour,-24,getdate()) and GETDATE() " +
            //                            "order by srh.Time, szh.SensorID";

            string strQueryField = "SELECT srh.id, srh.SensorHistoryID, srh.ReactionType, srh.Time, srh.Message, srh.Param1, srh.Param2, srh.Param3, srh.Param4, srh.Param5, szh.SensorID, srh.DetectionStatus ";



            // SensorZone ID가 존재하는 SensorZoneHistory(센서로부터 발생한 신호) 검색
            string szText = strQueryField;
            szText += "FROM SensorReactionHistory as srh, SensorZoneHistory as szh, SensorZone as sz, EquipmentZone as ez ";
            szText += "WHERE SensorHistoryID in (";
            szText += "         SELECT srh2.SensorHistoryID ";
            szText += "         FROM SensorReactionHistory as srh2, SensorZoneHistory as szh2 ";
            szText += "         WHERE szh2.Id = srh2.SensorHistoryID and srh2.ReactionType in " + GetAlarmReactionHistoryQueryString() + ") ";
            szText += "     AND SensorHistoryID not in (";
            szText += "         SELECT srh3.SensorHistoryID ";
            szText += "         FROM SensorReactionHistory as srh3, SensorZoneHistory as szh3 ";
            szText += "         WHERE szh3.Id = srh3.SensorHistoryID and srh3.ReactionType in " + GetAlarmOffReactionHistoryQueryString() + ") ";
            szText += "     AND srh.SensorHistoryID = szh.ID ";
            szText += "     AND szh.SensorID = sz.ID ";
            szText += "     AND sz.EquipZoneID = ez.ID ";
            szText += "     AND ez.SiteID = {0} ";
            szText += "     AND sz.Data in " + GetAlarmSensorZoneQueryString();
            szText += "     AND ( srh.Time between '{1}' and '{2}') ";
            szText += "     ORDER BY srh.Time, szh.SensorID";

            // SensorZone ID가 0인 SensorZoneHistory(수동화재신고) 검색
            string szText2 = strQueryField;
            szText2 += "FROM SensorReactionHistory as srh, SensorZoneHistory as szh ";
            szText2 += "WHERE SensorHistoryID in (";
            szText2 += "         SELECT srh2.SensorHistoryID ";
            szText2 += "         FROM SensorReactionHistory as srh2, SensorZoneHistory as szh2 ";
            szText2 += "         WHERE szh2.Id = srh2.SensorHistoryID and srh2.ReactionType in " + GetAlarmReactionHistoryQueryString() + ") ";
            szText2 += "     AND SensorHistoryID not in (";
            szText2 += "         SELECT srh3.SensorHistoryID ";
            szText2 += "         FROM SensorReactionHistory as srh3, SensorZoneHistory as szh3 ";
            szText2 += "         WHERE szh3.Id = srh3.SensorHistoryID and srh3.ReactionType in " + GetAlarmOffReactionHistoryQueryString() + ") ";
            szText2 += "     AND srh.SensorHistoryID = szh.ID ";
            szText2 += "     AND szh.SensorID = 0 ";
            szText2 += "     AND szh.SiteID = {0} ";
            szText2 += "     AND ( srh.Time between '{1}' and '{2}') ";
            szText2 += "     ORDER BY srh.Time, szh.SensorID";



            DateTime dtNow = DateTime.Now;
            string szNowTime = WebDBManager.MakeDateTimeString(DateTime.Now);
            DateTime dtPrev = dtNow.AddDays(-1.0);
            string szPrevTime = WebDBManager.MakeDateTimeString(dtPrev);
            // SensorZone ID가 존재하는 SensorZoneHistory(센서로부터 발생한 신호) 검색
            string strSQL = string.Format(szText, m_nSiteID, szPrevTime, szNowTime);
            
			ArrayList arrResult = NetworkServer.Instance.DBManager.GetResultData(strSQL, 0);
            if (arrResult == null)
                return;

            // SensorZone ID가 0인 SensorZoneHistory(수동화재신고) 검색
            string strSQL2 = string.Format(szText2, m_nSiteID, szPrevTime, szNowTime);

            ArrayList arrResult2 = NetworkServer.Instance.DBManager.GetResultData(strSQL2, 0);
            if (arrResult2 == null)
                return;

            // 두 Query 결과를 하나로 통합
            arrResult.AddRange(arrResult2);

            int nResultCount = arrResult.Count;

            DateTime dtDefault = new DateTime();

            SensorReactionLog log = new SensorReactionLog();
            bool isSuccess;
            int nMaxID = -1, nPrevSensorID = -1, nSensorID = -1;
            int nPrevCount = 0;

            ArrayList arrTimeHistory = new ArrayList();

			SortedList<int, int> keyExistList = new SortedList<int, int>();


            for (int i = 0; i < nResultCount - 11; i += 12)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nHistoryID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nReactionType = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                DateTime time = DBUtility.WebDBManager.GetDateTimeField(arrResult[i + 3], dtDefault);
                string strMessage = DBUtility.WebDBManager.GetStringField(arrResult[i + 4], "");
                string strParam1 = DBUtility.WebDBManager.GetStringField(arrResult[i + 5], "");
                string strParam2 = DBUtility.WebDBManager.GetStringField(arrResult[i + 6], "");
                string strParam3 = DBUtility.WebDBManager.GetStringField(arrResult[i + 7], "");
                string strParam4 = DBUtility.WebDBManager.GetStringField(arrResult[i + 8], "");
                string strParam5 = DBUtility.WebDBManager.GetStringField(arrResult[i + 9], "");
                nSensorID = DBUtility.WebDBManager.GetIntField(arrResult[i + 10].ToString(), -1);

                int nStatus = DBUtility.WebDBManager.GetIntField(arrResult[i + 11].ToString(), 3);

                if (nID < 0 || nHistoryID < 0)
                    continue;

				string szHashKey = nHistoryID.ToString() + "_-_" + nReactionType.ToString() + "_-_" + strMessage;
				int nHash = szHashKey.GetHashCode();
				if( keyExistList.ContainsKey(nHash))
					continue;

				keyExistList.Add(nHash, nHash);


                libSensorProcess.ReactionType type = SensorReactionLog.ToReactionType(nReactionType, out isSuccess);
				
				// 방송정보와 sms송신 로그는 보내지 않는다.
                if (type == libSensorProcess.ReactionType.SEND_SMS || type == libSensorProcess.ReactionType.RUN_BROADCAST)
					continue;

                if (!isSuccess)
                    continue;

                if (log.SensorHistoryID > 0 && log.SensorHistoryID != nHistoryID)
                {
                    if (nSensorID != nPrevSensorID && nPrevSensorID > 0)
                        CheckHistory(ref nMaxID, nPrevSensorID, log, arrTimeHistory);

                    int nHistoryCount = arrTimeHistory.Count;

                    if (nHistoryCount > nPrevCount)
                    {
                        log = new SensorReactionLog();
                        nPrevCount = nHistoryCount;
                    }
                }
                else if (log.SensorHistoryID > 0 && log.LogTime > time)
                    continue;

                log.ID = nID;
                log.SensorHistoryID = nHistoryID;
                log.Type = type;
                log.LogTime = time;
                log.Message = string.Compare(strMessage, "null", true) == 0 ? "" : strMessage;
                log.Param1 = string.Compare(strParam1, "null", true) == 0 ? "" : strParam1;
                log.Param2 = string.Compare(strParam2, "null", true) == 0 ? "" : strParam2;
                log.Param3 = string.Compare(strParam3, "null", true) == 0 ? "" : strParam3;
                log.Param4 = string.Compare(strParam4, "null", true) == 0 ? "" : strParam4;
                log.Param5 = string.Compare(strParam5, "null", true) == 0 ? "" : strParam5;
                log.Status = (SensorReactionLog.DetectionStatus)nStatus;

                nPrevSensorID = nSensorID;
            }

            CheckHistory(ref nMaxID, nPrevSensorID, log, arrTimeHistory);
            provider.AddTimeHistoryList(arrTimeHistory);

            foreach (TimeHistory history in arrTimeHistory)
            {
                if (history.LastReactionLog == null)
                    continue;

                if (history.LastReactionLog.Type == libSensorProcess.ReactionType.RUN_SOP)
                    provider.ProcessRunSOP(history.LastReactionLog);
                else if (history.LastReactionLog.Type == libSensorProcess.ReactionType.NOTIFY_FIRE)
                    provider.MonitorNotifyFireProcess(history.LastReactionLog);
                //else if (history.LastReactionLog.Type == SensorReactionLog.ReactionType.TRAINNING_FIRE)
                //	provider.MonitorNotifyFireProcess(history.LastReactionLog);
                else if (history.LastReactionLog.Type == libSensorProcess.ReactionType.BEGIN_STATUS)
                    provider.MonitorDetectFireProcess(history.LastReactionLog);

                // 방범신호에 대해 초기 로드감시 추가
                // skkim 2017-03-26
                else if (history.LastReactionLog.Type == libSensorProcess.ReactionType.NOTIFY_SECURITY)
                    provider.MonitorNotifySecurityProcess(history.LastReactionLog);
            }
        }

        private void CheckHistory(ref int nMaxReactionID, int nSensorID, SensorReactionLog log, ArrayList arrTimeHistory)
        {
            if (log.SensorHistoryID < 0)
                return;

            if (log.Type == libSensorProcess.ReactionType.IGNORE_S1ACCESS_STATUS ||
                log.Type == libSensorProcess.ReactionType.IGNORE_S1SVMS_STATUS ||
                log.Type == libSensorProcess.ReactionType.IGNORE_SECOM_STATUS)
                return;

            if (log.Type == libSensorProcess.ReactionType.IGNORE_FIRE ||
                log.Type == libSensorProcess.ReactionType.IGNORE_SOP ||
                log.Type == libSensorProcess.ReactionType.MALFUNCTION)
                return;
            else if (log.Type == libSensorProcess.ReactionType.FINISH_SOP ||
                log.Type == libSensorProcess.ReactionType.RUN_N_CANCEL_SOP)
            {
				SensorZone sensor = NetworkServer.Instance.IOManager.GetSensorZone(nSensorID);

                if (sensor != null && sensor.SensorData == 0)
                    return;
            }
            else if (log.Type == libSensorProcess.ReactionType.END_STATUS)
            {
                foreach (TimeHistory th in arrTimeHistory)
                {
                    if (th.HistoryID == log.SensorHistoryID)
                    {
                        arrTimeHistory.Remove(th);
                        return;
                    }
                }
            }

            TimeSpan span = DateTime.Now - log.LogTime;

            if (log.Type == libSensorProcess.ReactionType.RUN_SOP)
            {
                // 화재 SOP 발동후 Timeout(일)이 지나도록 종료되지 않은 것은 취소된 것으로 간주한다.
				if (span.TotalDays > NetworkServer.Instance.ServiceProvider.SOPTimeout)
                {
                    UpdateReactionHistory(ref nMaxReactionID, log, libSensorProcess.ReactionType.RUN_N_CANCEL_SOP, arrTimeHistory);
                    return;
                }
            }
            else if (log.Type == libSensorProcess.ReactionType.NOTIFY_FIRE ||
                log.Type == libSensorProcess.ReactionType.NOTIFY_SECURITY
               
                )
				//|| log.Type == SensorReactionLog.ReactionType.TRAINNING_FIRE)
            {
				if (span.TotalHours > NetworkServer.Instance.ServiceProvider.NotifyFireTimeout)
                {
                    // 화재 신고후 Timeout(시간)이 지나도록 발동하지 않은 SOP는 무시된 것으로 간주한다.
                    UpdateReactionHistory(ref nMaxReactionID, log, libSensorProcess.ReactionType.IGNORE_SOP, arrTimeHistory);
                    return;
                }
            }

            // nSensorID에 대한 SensorHistory가 존재하면, 그 이후 History가 생성되었으므로 기존것은 없앤다.
            int nOldSensorHistoryID = GetSensorHistoryID(nSensorID);
            if (nOldSensorHistoryID > 0)
            {
                m_dicSensorHistory.Remove(nOldSensorHistoryID);
            }
            ///////////////////////////////////////////////////////////////////////////////////////////////

            m_dicSensorHistory[log.SensorHistoryID] = nSensorID;
            
			TimeHistory history = NetworkServer.Instance.ServiceProvider.FindTimeHistory(log.SensorHistoryID);

            if (history != null)
            {
                history.LastReactionLog = log;
                history.Time = log.LogTime;
                history.DetectStatus = log.Status;
            }
            else
            {
                history = new TimeHistory(log.SensorHistoryID, log.LogTime, log.Status);
                history.LastReactionLog = log;
                history.DetectStatus = log.Status;
                arrTimeHistory.Add(history);
            }
        }

        public int GetSensorHistoryID(int nSensorID)
        {
            foreach (KeyValuePair<int, int> pair in m_dicSensorHistory)
            {
                if (pair.Value == nSensorID)
                    return pair.Key;
            }
            return -1;
        }

        private void UpdateReactionHistory(ref int nMaxReactionID, SensorReactionLog log, libSensorProcess.ReactionType type, ArrayList arrTimeHistory)
        {
            if (nMaxReactionID < 0)
            {
				ArrayList arrResult = NetworkServer.Instance.DBManager.GetResultData("Select max(ID) from SensorReactionHistory", 0);
                if (arrResult == null)
                    return;

                if (arrResult.Count == 0)
                    nMaxReactionID = 0;
                else
                    nMaxReactionID = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0);
            }

            DateTime dtNow = DateTime.Now;

            string strSQL = string.Format("Insert into SensorReactionHistory (ID, SensorHistoryID, ReactionType, Time, Message, Param1, Param2, Param3,  DetectionStatus) values ({0}, {1}, {2}, '{3}', '{4}', '{5}', '{6}', '{7}', {8})",
                ++nMaxReactionID, log.SensorHistoryID, (int)type, string.Format("{0} {1:00}:{2:00}:{3:00}", dtNow.ToShortDateString(), dtNow.Hour, dtNow.Minute, dtNow.Second),
                log.Message, log.Param1, log.Param2, log.Param3, (int)log.Status);

			if (NetworkServer.Instance.DBManager.GetResultData(strSQL, 0) != null)
            {
                ServiceProvider.WriteSensorReactionHistoryDescription(log, NetworkServer.Instance.DBManager);

                foreach (TimeHistory history in arrTimeHistory)
                {
                    if (history.HistoryID > log.SensorHistoryID)
                        return;
                }
                SetLastReadSensorHistoryID(log.SensorHistoryID);
            }
        }

        public int GetSensorID(int nSensorHistoryID)
        {
            if (m_dicSensorHistory.ContainsKey(nSensorHistoryID))
                return m_dicSensorHistory[nSensorHistoryID];

            return -1;
        }	
	}
}
