using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using System.Net.Sockets;
using SOPMonitoringSystem;
using System.Windows.Forms;
using FireSimulator;
using UnE.SOP;
using DBUtility;

namespace SOPMonitoringSystem
{
    public class ClientProvider : ClientServiceProvider
    {
        private NetworkManager m_mgr = null;
        private int m_nPingCount = 0;
        private byte[] m_arrReceived = null;
        // OnReceive()에서 전달받는 데이터(ReceivedData)가 아직 완결되지 않은 Packet일 경우 다음 OnReceive() 호출시 데이터를
        // 합치기 위한 임시 버퍼
        private byte[] m_arrTemp = null;

        // 현재 OnReceive()에서 받은 데이터를 처리중인가?
        private bool m_isReadingProcess = false;

        private List<Alarm> m_alarms = new List<Alarm>();

        public bool IsReadingProcess
        {
            get { return m_isReadingProcess; }
        }

        public int PingCount
        {
            get { return m_nPingCount; }
            set { m_nPingCount = value; }
        }

        public ClientProvider(NetworkManager mgr)
        {
            m_mgr = mgr;
            this.Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);
        }
        
        public override void OnReceiveData()
        {
            OnReceive(ReceivedData);
        }

        private bool OnReceive(byte[] bytes)
        {
            if (bytes != null)
            {
                m_isReadingProcess = true;

                m_arrReceived = bytes;

                if (m_arrTemp != null)
                {
                    int nReceivedCount = m_arrReceived.Length;
                    int nTempCount = m_arrTemp.Length;

                    byte[] arrBuffer = new byte[nReceivedCount + nTempCount];
                    Array.Copy(m_arrTemp, arrBuffer, nTempCount);
                    Array.Copy(m_arrReceived, 0, arrBuffer, nTempCount, nReceivedCount);

                    m_arrReceived = arrBuffer;
                    m_arrTemp = null;
                }

                int nBytesCount = m_arrReceived.Count();

                if (nBytesCount > 0)
                {
                    m_nPingCount = 0;

                    if (!CheckValidation(m_arrReceived))
                    {
                        m_arrTemp = m_arrReceived;
                        m_isReadingProcess = false;
                        return false;
                    }

                    m_mgr.RecvLog(m_arrReceived);

                    short nHeader;
                    ArrayList arrDatas = TcpHelper.ReadBytes(m_arrReceived, out nHeader);

                    if (arrDatas == null)
                        return false;

					if (nHeader == TCP_ID.ARE_YOU_THERE)
                    {
                        // 이미 종료되었어야 할 접속이 유지되고 있는 경우는 해당 접속을 강제로 종료시킨다.
                        if (m_mgr.ClientProvier != this)
                        {
                            this.Close();
                            m_isReadingProcess = false;
                            return true;
                        }

                        ProcessAreYouThere(arrDatas);
                    }
                    else if (nHeader == FireSimulator.TCP_ID.REPORT_FIRE)
                    {
                        ProcessReportFire(arrDatas);
                    }
                    else if (nHeader == FireSimulator.TCP_ID.CLEAR_FIRE)
                    {
                        ProcessClearFire(arrDatas);
                    }
                }
            }

            m_isReadingProcess = false;
            return true;
        }

        private void ProcessReportFire(ArrayList arrDatas)
        {
            if (arrDatas.Count == 4 && arrDatas[0] is string && arrDatas[1] is string && arrDatas[2] is string && arrDatas[3] is long)
            {
                string strProjectName = (string)arrDatas[0];
                string strLevelID = (string)arrDatas[1];
                string strSpaceID = (string)arrDatas[2];
                DateTime timeStamp = DateTime.FromBinary((long)arrDatas[3]);

                Alarm alarm = FindAlarm(strProjectName, strLevelID, strSpaceID);

                if (alarm == null)
                {
                    int nZoneID, nSensorZoneID, nSensorZoneHistoryID;
                    string strZoneName;

                    if (FindZones(strProjectName, strLevelID, strSpaceID, timeStamp, out nZoneID, out nSensorZoneID, out nSensorZoneHistoryID, out strZoneName))
                    {
                        alarm = new Alarm(strProjectName, strLevelID, strSpaceID, timeStamp);
                        alarm.SensorZoneHistoryID = nSensorZoneHistoryID;
                        alarm.SensorZoneID = nSensorZoneID;
                        m_alarms.Add(alarm);

                        // RunSOP
                        StubWorker.Instance.OpenSOP_Fire(nZoneID, timeStamp, nSensorZoneID, nSensorZoneHistoryID);
                    }
                }
            }
        }

        private void ProcessClearFire(ArrayList arrDatas)
        {
            if (arrDatas.Count == 4 && arrDatas[0] is string && arrDatas[1] is string && arrDatas[2] is string && arrDatas[3] is long)
            {
                string strProjectName = (string)arrDatas[0];
                string strLevelID = (string)arrDatas[1];
                string strSpaceID = (string)arrDatas[2];
                DateTime time = DateTime.FromBinary((long)arrDatas[3]);

                Alarm alarm = FindAlarm(strProjectName, strLevelID, strSpaceID);

                if (alarm != null)
                {
                    m_alarms.Remove(alarm);
                    // CloseSOP
                    StubWorker.Instance.SensorClose(alarm.SensorZoneID, alarm.SensorZoneHistoryID);
                }
            }
        }

        private bool FindZones(string strProjectName, string strLevelId, string strSpaceID, DateTime timeStamp, out int nZoneID, out int nSensorZoneID, out int nSensorZoneHistoryID, out string strZoneName)
        {
            nZoneID = nSensorZoneID = nSensorZoneHistoryID = -1;
            strZoneName = "";

            WebDBManager dbMgr = FormSOP.Instance.DBManager;

            string strSQL = string.Format("Select Zone.ID from Building, Zone where Zone.BuildingID = Building.ID and Building.BuildingName = '{0}' and Zone.ZoneName = '{1}'", strProjectName, strSpaceID);
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return false;

            VariousData<int> zoneID = WebDBManager.GetIntField(arrResult[0].ToString());

            if (zoneID == null)
                return false;

            nZoneID = zoneID.Data;

            strSQL = "Select ZoneName from EquipmentZone where ID = " + nZoneID.ToString();
            arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return false;

            strZoneName = WebDBManager.GetStringField(arrResult[0]);

            if (strZoneName == null)
                return false;

            nSensorZoneID = nZoneID;
            nSensorZoneHistoryID = MakeSensorZoneHistory(dbMgr, nSensorZoneID, timeStamp, strZoneName);

            if (nSensorZoneHistoryID < 0)
                return false;

            return true;
        }

        private int MakeSensorZoneHistory(WebDBManager dbMgr, int nSensorZoneID, DateTime timeStamp, string strZoneName)
        {
            string strSQL = "Select max(ID) from SensorZoneHistory";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return -1;

            int nID = 1;

            if (arrResult.Count > 0)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

                if (id != null)
                    nID = id.Data + 1;
            }

            string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", timeStamp.Year, timeStamp.Month, timeStamp.Day, timeStamp.Hour, timeStamp.Minute, timeStamp.Second);

            strSQL = "Insert into SensorZoneHistory (ID, SensorID, Connected, Data, Time, Description, param1, param2, param3, SiteID) values ";
            strSQL += string.Format("({0}, {1}, 1, 1, '{2}', NULL, NULL, NULL, NULL, 1)", nID, nSensorZoneID, strTime);

            if (dbMgr.GetResultData(strSQL, 0) == null)
                return -1;

            string strMessage = string.Format("{0}에서 화재가 발생하였습니다.", strZoneName);
            strSQL = "Insert into SensorReactionHistory (ID, SensorHistoryID, ReactionType, Time, Message, Param1, Param2, Param3, Param4, Param5, DetectionStatus) values ";
            strSQL += string.Format("({0}, {0}, 0, '{1}', '{2}', NULL, NULL, NULL, NULL, NULL, 3)", nID, strTime, strMessage);

            if (dbMgr.GetResultData(strSQL, 0) == null)
                return -1;

            return nID;
        }

        private Alarm FindAlarm(string strProjectName, string strLevelID, string strSpaceID)
        {
            foreach (Alarm alarm in m_alarms)
            {
                if (alarm.ProjectName == strProjectName && alarm.LevelID == strLevelID && alarm.SpaceID == strSpaceID)
                    return alarm;
            }

            return null;
        }

        private void ProcessAreYouThere(ArrayList arrDatas)
        {
            arrDatas.Clear();
            arrDatas.Add(FormSOP.Instance.HasControl);

            byte[] bytes = TcpHelper.MakeBytes(TCP_ID.I_AM_HERE, arrDatas);

            m_mgr.Send(bytes, this);
        }

		private bool CheckValidation(byte[] bytes)
		{
			int length = bytes.Length;
			if (length < 6)
				return false;

			int nChunkCount = (int)BitConverter.ToInt16(bytes, 2);
			int nIndex = 6;

			for (int i = 0; i < nChunkCount; i++)
			{
				if (length < nIndex + 5)
					return false;

				int nDataLength = BitConverter.ToInt32(bytes, nIndex + 1);

				if (length < nIndex + 5 + nDataLength)
					return false;

				nIndex += 5 + nDataLength;
			}

			if (length > nIndex)
			{
				byte[] bytes1 = new byte[nIndex];
				byte[] bytes2 = new byte[length - nIndex];

				Array.Copy(bytes, bytes1, nIndex);
				Array.Copy(bytes, nIndex, bytes2, 0, length - nIndex);

				OnReceive(bytes1);

				if (!OnReceive(bytes2))
					return false;

				m_arrReceived = null;
				return false;
			}

			return true;
		}

        // header 1 Byte로만 이루어진 데이터
		public void SendData(short header)
		{
			byte[] bytes = new byte[6];

			byte[] nHader = BitConverter.GetBytes(header);
			byte[] nCount = BitConverter.GetBytes(0);

			bytes[0] = nHader[0];
			bytes[1] = nHader[1];

			bytes[2] = nCount[0];
			bytes[3] = nCount[1];
			bytes[4] = nCount[2];
			bytes[5] = nCount[3];

			if (Client != null && this.Client.Client != null)
			{
				if (this.Client.Client.Connected == true)
					m_mgr.Send(bytes, this);
			}			
		}

		public void SendData(short header, byte dataHeader, byte[] datas)
		{
			if (header < 0)
				return;

			if (datas.Length >= 10000)
				return;
			if (datas == null || datas.Length == 0)
				return;

			byte[] sndData = new byte[datas.Length + 11];

			byte[] nHader = BitConverter.GetBytes(header);
			byte[] nCount = BitConverter.GetBytes(1);

			// SET MESSAGE HeADER
			sndData[0] = nHader[0];
			sndData[1] = nHader[1];

			// SET DATA COUNT
			sndData[2] = nCount[0];
			sndData[3] = nCount[1];
			sndData[4] = nCount[2];
			sndData[5] = nCount[3];

			// SET DATA TYPE
			sndData[6] = dataHeader;

			// SET DATA LENGTH
			byte[] lengthData = BitConverter.GetBytes(datas.Length);
			for (int i = 0; i < 4; i++)
			{
				if (lengthData.Length > i)
				{
					sndData[7 + i] = lengthData[i];
				}
			}

			// SET DATA
			for (int i = 0; i < datas.Length; i++)
			{
				sndData[i + 11] = datas[i];
			}

			if (this.IsClientDisposed == false)
				m_mgr.Send(sndData, this);
		}

        public override void OnDropConnection()
        {
            m_mgr.OnDropConnection();
            m_arrTemp = null;
        }

        public new void Close()
        {
            base.Close();
            m_arrTemp = null;
        }

        public void SendResetUserDefinedTeamNames(int nActionStepHistoryID)
        {
        }
    }

    public class Alarm
    {
        private string m_strProjectName = "";
        private string m_strLevelID = "";
        private string m_strSpaceID = "";
        private DateTime m_timeStamp = new DateTime();
        private int m_nSensorZoneID = -1;
        private int m_nSensorZoneHistoryID = -1;

        public string ProjectName
        {
            get { return m_strProjectName; }
            set { m_strProjectName = value; }
        }

        public string LevelID
        {
            get { return m_strLevelID; }
            set { m_strLevelID = value; }
        }

        public string SpaceID
        {
            get { return m_strSpaceID; }
            set { m_strSpaceID = value; }
        }

        public DateTime TimeStamp
        {
            get { return m_timeStamp; }
            set { m_timeStamp = value; }
        }

        public int SensorZoneID
        {
            get { return m_nSensorZoneID; }
            set { m_nSensorZoneID = value; }
        }

        public int SensorZoneHistoryID
        {
            get { return m_nSensorZoneHistoryID; }
            set { m_nSensorZoneHistoryID = value; }
        }

        public Alarm()
        {
        }

        public Alarm(string strProjectName, string strLevelID, string strSpaceID, DateTime timeStamp)
        {
            m_strProjectName = strProjectName;
            m_strLevelID = strLevelID;
            m_strSpaceID = strSpaceID;
            m_timeStamp = timeStamp;
        }
    }
}
