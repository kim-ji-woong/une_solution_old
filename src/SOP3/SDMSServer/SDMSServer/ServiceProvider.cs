using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using System.Collections;
using System.Threading;
using SDMS;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace SDMSServer
{
    public class ServiceProvider : TcpServiceProvider
    {
        private ArrayList m_arrClients = new ArrayList();
        //private bool m_isLock = false;
        private bool m_isAliveThread = true;

        // SOP 실행후 몇 일 이내에 종료되어야 하는가?
        private double m_dSOPTimeout = -1;
        // 화재 신고후 몇 시간 이내에 후속 작업이 진행되어야 하는가?
        private double m_dNotifyFireTimeout = -1;
        // 화재 탐지후 몇 시간 이내에 후속 작업이 진행되어야 하는가?
        private double m_dDetectFireTimeout = -1;

        public double SOPTimeout
        {
            get { return m_dSOPTimeout; }
        }

        public double NotifyFireTimeout
        {
            get { return m_dNotifyFireTimeout; }
        }

        public double DetectFireTimeout
        {
            get { return m_dDetectFireTimeout; }
        }

        // 임시로 무시할 센서들의 리스트
        private ArrayList m_arrTempIgnoreSensors = new ArrayList();

        // Ping은 로그에 남기지 않는다.
        private bool m_exceptPingLog = true;

        private void WriteLog(string str)
        {
            if (ConnectionLog.Instance.IsOpened)
                ConnectionLog.Instance.Write(str);
        }

        private void WriteLineLog(string str)
        {
            if (ConnectionLog.Instance.IsOpened)
                ConnectionLog.Instance.WriteLine(str);
        }

        private void InitLog()
        {
            ConnectionLog.Instance.Create("sdmsserver.log");
        }

        private void RecvLog(byte[] bytes, ConnectionState state)
        {
            if (!ConnectionLog.Instance.IsOpened)
                return;

            if (bytes[0] != TCP_ID.I_AM_HERE || !m_exceptPingLog)
            {
                string strClient = "Unknown";

                ClientData data = (ClientData)state.Tag;

                if (data != null)
                {
                    if (data.Type == ClientData.ClientType.SDMS_CLIENT)
                        strClient = "SDMS Client";
                    else if (data.Type == ClientData.ClientType.SENSOR_SIMULATOR)
                        strClient = "Sensor Simulator";
                    else if (data.Type == ClientData.ClientType.SOP_SIMULATOR)
                        strClient = "SOP Simulator";
                }

                strClient += "(" + state.RemoteEndPoint.ToString() + ")";

                string strLog = string.Format("RecvMessage : Header({0}), Length({1}) from {2}", (int)bytes[0], (int)bytes.Length, strClient);
                string strBytes = "";

                foreach (byte b in bytes)
                {
                    if (strBytes.Length == 0)
                        strBytes = string.Format("\r\n\t\t{0:X2}", (int)b);
                    else
                        strBytes += string.Format(" {0:X2}", (int)b);
                }

                WriteLineLog(strLog + strBytes);
            }
        }

        private bool Send(byte[] bytes, int nOffset, int nLength, ConnectionState state)
        {
            if (state.Write(bytes, nOffset, nLength))
            {
                if (!ConnectionLog.Instance.IsOpened)
                    return true;

                if (bytes[nOffset] != TCP_ID.ARE_YOU_THERE || !m_exceptPingLog)
                {
                    string strClient = "Unknown";

                    ClientData data = (ClientData)state.Tag;

                    if (data != null)
                    {
                        if (data.Type == ClientData.ClientType.SDMS_CLIENT)
                            strClient = "SDMS Client";
                        else if (data.Type == ClientData.ClientType.SENSOR_SIMULATOR)
                            strClient = "Sensor Simulator";
                        else if (data.Type == ClientData.ClientType.SOP_SIMULATOR)
                            strClient = "SOP Simulator";
                    }

                    strClient += "(" + state.RemoteEndPoint.ToString() + ")";

                    string strLog = string.Format("SendMessage : Header({0}), Length({1}) to {2}", (int)bytes[nOffset], nLength, strClient);
                    string strBytes = "";

                    foreach (byte b in bytes)
                    {
                        if (strBytes.Length == 0)
                            strBytes = string.Format("\r\n\t\t{0:X2}", (int)b);
                        else
                            strBytes += string.Format(" {0:X2}", (int)b);
                    }

                    WriteLineLog(strLog + strBytes);
                }

                return true;
            }

            return false;
        }

        public ServiceProvider()
        {
            InitLog();
            ReadOption();
            Thread t = new Thread(new ThreadStart(PingThread));
            t.Start();
        }

        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder refval, int size, string filepath);

        public string getinivalue(string section, string key, string filepath)
        {
            StringBuilder temp = new StringBuilder(255);
            int nLen = GetPrivateProfileString(section, key, "", temp, 255, filepath);

            return temp.ToString();

        }

        private void ReadOption()
        {
            string strFilePath = System.Windows.Forms.Application.StartupPath + "\\sdmsserver.ini";
            string strSOPTimeout = getinivalue("Timeout Option", "SOP_TIMEOUT", strFilePath);
            string strDetectTimeout = getinivalue("Timeout Option", "DETECT_FIRE_TIMEOUT", strFilePath);
            string strNotifyTimeout = getinivalue("Timeout Option", "NOTIFY_FIRE_TIMEOUT", strFilePath);

            double.TryParse(strSOPTimeout, out m_dSOPTimeout);
            double.TryParse(strDetectTimeout, out m_dDetectFireTimeout);
            double.TryParse(strNotifyTimeout, out m_dNotifyFireTimeout);
        }

		public override object Clone()
		{
            return this;           
		}       	

		private ArrayList m_arTimeHistory = new ArrayList();
		public override void OnAcceptConnection(ConnectionState state)
		{
            lock (this)
            {
                state.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);
                state.Tag = new ClientData();
                m_arrClients.Add(state);

                SendMessage(TCP_ID.WHO_ARE_YOU, state);
			
                FormMain.Instance.Invoke((System.Windows.Forms.MethodInvoker)delegate
                {
                    FormMain.Instance.AddClient(state);
                });				
            }
		}

        // Header만 있는 메시지 보내기
        private void SendMessage(byte header, ConnectionState state)
        {
            byte[] bytes = new byte[2] { header, 0 };
            Send(bytes, 0, bytes.Length, state);
        }
		
		public SensorReactionLog ReadFireReport(byte[] bytes)
		{
			SensorReactionLog log = new SensorReactionLog();
	
			int nReadDataCount = 1;
			int chunkSize = (int)bytes[nReadDataCount++];

			int nSensorHistoryID = -1;
			byte dataHeader = bytes[nReadDataCount++];
			int nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
            nReadDataCount += 4;

			if (dataHeader == TCP_TYPE.INTEGER)
			{
				nSensorHistoryID = BitConverter.ToInt32(bytes, nReadDataCount);
				nReadDataCount += nDataLength;				
			}
			
            chunkSize -= 1;
			log.SensorHistoryID = nSensorHistoryID;

			int nEquipZoneID = -1;
			dataHeader = bytes[nReadDataCount++];
			nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
			nReadDataCount += 4;

            if (dataHeader == TCP_TYPE.INTEGER)
			{
				nEquipZoneID = BitConverter.ToInt32(bytes, nReadDataCount);
				nReadDataCount += nDataLength;
			}
			chunkSize -= 1;

			log.Param1 = nEquipZoneID.ToString();
			
			int nSensorID = -1;
			if (chunkSize > 0)
			{				
				dataHeader = bytes[nReadDataCount++];
				nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
				nReadDataCount += 4;
				if (dataHeader == TCP_TYPE.INTEGER)
				{
					nSensorID = BitConverter.ToInt32(bytes, nReadDataCount);
					nReadDataCount += nDataLength;
				}
			}
			log.Param2 = nSensorID.ToString();
			if (nEquipZoneID == -1)
			{
				log.Message = "화재발생이 신고되었습니다";
			}
			else
			{
                EquipmentZone equipZone = ZoneManager.Instance.GetEquipmentZone(nEquipZoneID);

				if (equipZone != null)
				{
                    string szZoneName = equipZone.EquipZoneName;
					log.Message = string.Format("[{0}]에서 화재발생이 신고되었습니다", szZoneName);
				}
				log.Param1 = nEquipZoneID.ToString();
			}
			log.Type = SensorReactionLog.ReactionType.NOTIFY_FIRE;

			return log;
		}
		        
		public SensorReactionLog ReadMalfunctionReport(byte[] bytes)
		{
			SensorReactionLog log = new SensorReactionLog();

			int nReadDataCount = 1;
            int chunkSize = (int)bytes[nReadDataCount++];

			int nSensorHistoryID = -1;
            byte dataHeader = bytes[nReadDataCount++];
            int nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
			nReadDataCount += 4;
			if (dataHeader == TCP_TYPE.INTEGER)
			{
                nSensorHistoryID = BitConverter.ToInt32(bytes, nReadDataCount);
				nReadDataCount += nDataLength;
			}
			chunkSize -= 1;
			log.SensorHistoryID = nSensorHistoryID;

			int nSensorID = -1;
			if (chunkSize > 0)
			{
                dataHeader = bytes[nReadDataCount++];
                nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
				nReadDataCount += 4;
				if (dataHeader == TCP_TYPE.INTEGER)
				{
                    nSensorID = BitConverter.ToInt32(bytes, nReadDataCount);
					nReadDataCount += nDataLength;
				}
			}
			//ResetSensorData(nSensorID);
			log.Param2 = nSensorID.ToString();
			int nEquipZoneID = SensorManager.Instance.GetSensorZone(nSensorID);
		    
			if (nEquipZoneID == -1)
			{
				log.Message = "탐지된 화재가 오작동으로 신고되었습니다";
			}
			else
			{
                EquipmentZone equipZone = ZoneManager.Instance.GetEquipmentZone(nEquipZoneID);
				if (equipZone != null)
				{
                    string szZoneName = equipZone.EquipZoneName;
					log.Message = string.Format("[{0}]에서 탐지된 화재가 오작동으로 신고되었습니다", szZoneName);
				}
                log.Param1 = nEquipZoneID.ToString();
			}

            log.Type = SensorReactionLog.ReactionType.MALFUNCTION;
            log.SensorHistoryID = nSensorHistoryID;

			return log;
		}

		public SensorReactionLog CreateFireDetect(int nHistoryID, int nSensorID)
		{
			SensorReactionLog log = new SensorReactionLog();

			log.LogTime = DateTime.Now;
			log.SensorHistoryID = nHistoryID;
            log.Type = SensorReactionLog.ReactionType.BEGIN_STATUS;
			int nEquipZoneID = SensorManager.Instance.GetSensorZone(nSensorID);
			if (nEquipZoneID == -1)
			{
				log.Message = "화재가 탐지 되었습니다";
			}
			else
			{
                EquipmentZone equipZone = ZoneManager.Instance.GetEquipmentZone(nEquipZoneID);
				if (equipZone != null)
				{
                    string szZoneName = equipZone.EquipZoneName;
					log.Message = string.Format("[{0}]에서 화재가 탐지 되었습니다", szZoneName);
				}
				log.Param1 = nEquipZoneID.ToString();
			}

            log.Param2 = nSensorID.ToString();

			return log;
		}


		public void ResetSensorData(int nID)
		{
			string szSQP = string.Format("UPDATE SensorZone set Data=0 , Connected=1 where ID={0}", nID);
			FormMain.Instance.DBManager.GetResultData(szSQP, 0);
		}

		public SensorReactionLog ReadFailReport()
		{
			SensorReactionLog log = new SensorReactionLog();
			return log;
		}

		public void AddReactionLog(SensorReactionLog log)
		{
            string strSQL = "Select max(ID) from SensorReactionHistory";
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);
                       
            int nReactionHistoryID = -1;
            if (arrResult == null)
                return;
            if (arrResult.Count == 0)
                nReactionHistoryID = 1;
            else
                nReactionHistoryID = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;

            log.LogTime = DateTime.Now;
			string strDateTimeField = string.Format("{0} {1}:{2}:{3}", log.LogTime.ToShortDateString(), log.LogTime.Hour, log.LogTime.Minute, log.LogTime.Second);
            strSQL = string.Format("Insert into SensorReactionHistory (ID, SensorHistoryID, ReactionType, Time, Message, Param1, Param2) values ({0}, {1}, {2}, '{3}', '{4}', '{5}', '{6}')",
			   nReactionHistoryID, log.SensorHistoryID, (int)log.Type, strDateTimeField, log.Message, log.Param1, log.Param2);
            log.ID = nReactionHistoryID;
			FormMain.Instance.DBManager.GetResultData(strSQL, 0);

			// SMS 전송로그는 사용자에게 전송할 필요 없다.
			if (log.Type == SensorReactionLog.ReactionType.SEND_SMS)
				return;
			// 방송메세지는 사용자에게 전송할 필요없다.
			if (log.Type == SensorReactionLog.ReactionType.RUN_BROADCAST)
				return;

            if (log.SensorHistoryID > 0)
            {
                foreach (TimeHistory history in m_arTimeHistory)
                {
                    if (history.LastReactionLog != null && history.LastReactionLog.SensorHistoryID == log.SensorHistoryID)
                    {
                        System.Diagnostics.Trace.WriteLine(string.Format("LastReactionLog status is changed({0})", log.Type));
                        history.LastReactionLog = log;
                        break;
                    }
                }
            }
            SensorManager.Instance.SetLastReadSensorHistoryID(log.SensorHistoryID - 1);
		}

		public override bool OnReceiveData(ConnectionState state)
		{
            lock (this)
            {
                if (!base.OnReceiveData(state))
                    return false;

                if (ReceivedData != null)
                {
                    int nBytesCount = ReceivedData.Length;

                    if (nBytesCount > 0)
                    {
                        if (!CheckValidation(ReceivedData))
                            return false;

                        RecvLog(ReceivedData, state);

                        if (ReceivedData[0] == TCP_ID.I_AM_HERE)
                        {
                            ClientData client = (ClientData)state.Tag;
                            client.PingCount = 0;
                        }
                        else if (ReceivedData[0] == TCP_ID.WHO_I_AM)
                        {
                            if (SetClientType(ReceivedData, state))
                            {
                                // 현재 진행중인 화재들에 대한 마지막 Log List를 전송한다.
                                SendSensorReactionLogList(state, ClientData.ClientType.SDMS_CLIENT);
                                SendCurrentFireSensorSignal(state, ClientData.ClientType.SOP_SIMULATOR);
                            }
                        }
                        else if (ReceivedData[0] == TCP_ID.SENSOR_DATA)
                        {

                            int nSensorID, data, nPrevSensorHistoryID = -1;
                            bool connected = false;
                            int nHistoryID = FormMain.Instance.SensorManager.ProcessSensorData(ReceivedData, out nSensorID, out data, out connected, ref nPrevSensorHistoryID);
                            PostProcessSensorData(nHistoryID, nPrevSensorHistoryID, nSensorID, data, connected);

                        }
                        else if (ReceivedData[0] == TCP_ID.FIRE_DETECT_REPORT)
                        {
                            // SensorZone이 아닌 개별 Sensor ID
                            SensorReactionLog log = ReadFireReport(ReceivedData);
                            if (CheckSituation(log.SensorHistoryID))
                            {
                                AddReactionLog(log);

                                // 사내방송 실시
                                RunBroadcast(log);
                                // SMS전송
                                SendSMS(log);

                                MonitorNotifyFireProcess(log);
                                // Send Reaction Log
                                SendSensorReactionLog(log, ClientData.ClientType.SDMS_CLIENT);
                                SendFireSensorSignal(log);


                            }
                        }
                        else if (ReceivedData[0] == TCP_ID.MALFUNCTION_REPORT)
                        {
                            ProcessMalfunction(ReceivedData);
                        }
                        else if (ReceivedData[0] == TCP_ID.RUN_SOP)
                            ProcessRunSOP(ReceivedData);
                        else if (ReceivedData[0] == TCP_ID.IGNORE_SOP)
                            ProcessIgnoreSOP(ReceivedData);
                        else if (ReceivedData[0] == TCP_ID.CHANGE_FACILITY_MANAGER)
                            ProcessChangeFacilityManager(ReceivedData);
                    }
                }
            }
            return true;
		}

        private void ProcessChangeFacilityManager(byte[] bytes)
        {
            SendData(bytes, true, ClientData.ClientType.SDMS_CLIENT);

            DataManager.Instance.LoadFacilityManager();
        }

		private void RunBroadcast(SensorReactionLog log)
		{
			// 화재 발생 방송
			string szBroadcastMsg = "";
			int nRepeat = 1;
			bool bUseSiren = false;
			bool bResult = GetBroadcastMessage(log, out szBroadcastMsg, out nRepeat, out bUseSiren);
			if (bResult)
			{
				SensorReactionLog smsLog = new SensorReactionLog();
				smsLog.Message = "사내 방송 실시";			
				smsLog.Param1 = log.Param1;
				smsLog.Param2 = log.Param2;
				smsLog.SensorHistoryID = log.SensorHistoryID;
				smsLog.Type = SensorReactionLog.ReactionType.RUN_BROADCAST;
				AddReactionLog(smsLog);

				BroadcastManager.Instance.AddSpeech(szBroadcastMsg, nRepeat, bUseSiren);
			}
		}

		private string MakeSMSMessage(SensorReactionLog log)
		{
			return log.Message;
		}

		private string GetSendPhoneNumber()
		{
			return "";
		}

		private ArrayList GetOperatorPhoneNumber(SensorReactionLog log)
		{
			// FacilityManager table
			//시설물 Type : 0(화재탐지센서), 1(스프링쿨러), 2(펌프압력센서), 3(CCTV), 4(소화기), 5(소화전), 6(발신기)
			//0(CompanyMember), 1(RegularTeam), 2(ExternalCompanyMember), 3(ExternalCompanyTeam)
			// MemberType이 1(RegularTeam)일 경우에만 사용. 몇 급이상만 담당자로 지정할 것인지 설정. NULL이면 팀원 모두. ex)4 => 4급 이상
			//return "01043632290";

            int nSensorZoneID = SensorManager.Instance.GetSensorID(log.SensorHistoryID);
            if (nSensorZoneID < 0)
                return null;

            SensorZone sensor = FormMain.Instance.IOManager.GetSensorZone(nSensorZoneID);
            if (sensor == null || sensor.EquipZone == null)
                return null;

            ArrayList arrPhoneNumbers = new ArrayList();

            foreach (Zone zone in sensor.EquipZone.LinkedZoneList)
            {
                AddPhoneNumbers(sensor, zone, arrPhoneNumbers);
            }

            /*Building building = sensor.Zone.Building;

            Facility.FacilityType type = (Facility.FacilityType)sensor.Type;
            FacilityManagerGroup group = null;

            if (building == null)
                group = DataManager.Instance.GetOutdoorFacilityManagerGroup(type, sensor.Zone);
            else
                group = DataManager.Instance.GetBuildingFacilityManagerGroup(type, building);

            if (group == null)
                group = DataManager.Instance.GetEntireFacilityManagerGroup(type);

            if (group == null)
                return null;

            ArrayList arrPhoneNumbers = new ArrayList();

            foreach (FacilityManager mgr in group.CompanyMembers)
            {
                AddPhoneNumber(arrPhoneNumbers, mgr);
            }

            foreach (FacilityManager mgr in group.ExternalCompanyMembers)
            {
                AddPhoneNumber(arrPhoneNumbers, mgr);
            }

            foreach (FacilityManager mgr in group.RegularTeams)
            {
                AddPhoneNumber(arrPhoneNumbers, mgr);
            }

            foreach (FacilityManager mgr in group.ExternalTeams)
            {
                AddPhoneNumber(arrPhoneNumbers, mgr);
            }*/

            return arrPhoneNumbers;
		}

        private void AddPhoneNumbers(SensorZone sensor, Zone zone, ArrayList arrPhoneNumbers)
        {
            Building building = zone.Building;

            Facility.FacilityType type = (Facility.FacilityType)sensor.Type;
            FacilityManagerGroup group = null;

            if (building == null)
                group = DataManager.Instance.GetOutdoorFacilityManagerGroup(type, zone);
            else
                group = DataManager.Instance.GetBuildingFacilityManagerGroup(type, building);

            if (group == null)
                group = DataManager.Instance.GetEntireFacilityManagerGroup(type);

            if (group == null)
                return;

            foreach (FacilityManager mgr in group.CompanyMembers)
            {
                AddPhoneNumber(arrPhoneNumbers, mgr);
            }

            foreach (FacilityManager mgr in group.ExternalCompanyMembers)
            {
                AddPhoneNumber(arrPhoneNumbers, mgr);
            }

            foreach (FacilityManager mgr in group.RegularTeams)
            {
                AddPhoneNumber(arrPhoneNumbers, mgr);
            }

            foreach (FacilityManager mgr in group.ExternalTeams)
            {
                AddPhoneNumber(arrPhoneNumbers, mgr);
            }
        }

        private void AddPhoneNumber(ArrayList arrPhoneNumbers, FacilityManager mgr)
        {
            if (mgr.MemberType == 0)
            {
                DataCompanyMember member = (DataCompanyMember)mgr.Tag;

                if (member == null)
                    return;

                if (arrPhoneNumbers.Contains(member.PhoneNumber))
                    return;

                arrPhoneNumbers.Add(member.PhoneNumber);
            }
            else if (mgr.MemberType == 1)
            {
                DataTeam team = (DataTeam)mgr.Tag;

                if (team == null)
                    return;

                ArrayList arrMembers = DataManager.Instance.GetTeamMembers(team);

                foreach (DataCompanyMember member in arrMembers)
                {
                    if (arrPhoneNumbers.Contains(member.PhoneNumber))
                        continue;

                    if (mgr.LevelLimit > 0)
                    {
                        //if (member.LevelID >= mgr.LevelLimit)
                        if (member.LevelID > 0 && member.LevelID <= mgr.LevelLimit)
                            arrPhoneNumbers.Add(member.PhoneNumber);
                    }
                    else
                        arrPhoneNumbers.Add(member.PhoneNumber);
                }
            }
            else if (mgr.MemberType == 2)
            {
                DataExternalMember member = (DataExternalMember)mgr.Tag;

                if (member == null)
                    return;

                if (arrPhoneNumbers.Contains(member.PhoneNumber))
                    return;

                arrPhoneNumbers.Add(member.PhoneNumber);
            }
            else if (mgr.MemberType == 3)
            {
                DataTeam team = (DataTeam)mgr.Tag;

                if (team == null)
                    return;

                ArrayList arrMembers = DataManager.Instance.GetTeamMembers(team);

                foreach (DataExternalMember member in arrMembers)
                {
                    if (arrPhoneNumbers.Contains(member.PhoneNumber))
                        continue;

                    arrPhoneNumbers.Add(member.PhoneNumber);
                }
            }
        }

		private void SendSMS(SensorReactionLog log)
		{			
			// 센서와 연결된 담당자 전화번호 가져오기
			//string szPhone = GetOperatorPhoneNumber(log);
            ArrayList arrPhoneNumbers = GetOperatorPhoneNumber(log);
            if (arrPhoneNumbers == null)
                return;

			// 사전 정의된 메세지 가져오기
			string szMsg = MakeSMSMessage(log);
			// 발신자 번호 가져오기
			string szSendNum = GetSendPhoneNumber();
			// 문자 메세지 보내기
			//if (szPhone != "" && szMsg != "")
            if (szMsg != "")
			{	
				// Send SMS
				//SMSManager.Instance.SendSMS(szPhone, szSendNum, szMsg);
                SMSManager.Instance.SendSMS(arrPhoneNumbers, szSendNum, szMsg);
								
				SensorReactionLog smsLog = new SensorReactionLog();
				//smsLog.Message = szPhone + "으로 메세지가 전송되었습니다. 내용 : " +szMsg;
                smsLog.Message = "담당자에게 메세지가 전송되었습니다. 내용 : " + szMsg;
				smsLog.Param1 = log.Param1;
				smsLog.Param2 = log.Param2;
				smsLog.SensorHistoryID = log.SensorHistoryID;
				smsLog.Type = SensorReactionLog.ReactionType.SEND_SMS;
				AddReactionLog(smsLog);
			}	
		}

		private bool GetBroadcastMessage(SensorReactionLog log, out string szBroadcastMessage, out int nRepeat, out bool bSiren)
		{	
			szBroadcastMessage = "";
			bSiren = false;
			nRepeat = 1;
			DBUtility.WebDBManager dbMgr = FormMain.Instance.DBManager;
			string strSQL = "select id, UseBroadcast, Message, UseSiren, RepeatCount from SDMSBroadcastConfig where SituationType = 0";

			ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);
			if (arrResult == null)
				return false;

			int nResultCount = arrResult.Count;

			for (int i = 0; i < nResultCount - 4; i++)
			{
				int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
				bool useBroadcast = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), 0) == 0 ? false : true;
				string strMessage = DBUtility.WebDBManager.GetStringField(arrResult[i + 2], "");
				bool useSiren = DBUtility.WebDBManager.GetIntField(arrResult[i + 3].ToString(), 0) == 0 ? false : true;
				int nRepeatCount = DBUtility.WebDBManager.GetIntField(arrResult[i + 4].ToString(), 0);

				if (useBroadcast == false)
				{
					return false;
				}

				bSiren = useSiren;
				nRepeat = 1;
				
				int nEquipZoneID = -1;
				int.TryParse(log.Param1, out nEquipZoneID);

				string szOnce = "";
				int nIdx = strMessage.IndexOf("<<");
				int nIdx2 = strMessage.IndexOf(">>");
				if( nIdx != -1 && nIdx2 != -1)
                    szOnce = strMessage.Substring(nIdx + 2, (nIdx2 - nIdx) - 2);

				string szMsg = strMessage.Substring(nIdx2 + 2);
				string szMsg1 = "";
                if (nEquipZoneID != -1)
				{
                    EquipmentZone equipZone = ZoneManager.Instance.GetEquipmentZone(nEquipZoneID);

                    if (equipZone != null)
                        szMsg1 = szMsg.Replace("●", equipZone.BroadcastName);
				}
				szBroadcastMessage = szOnce +szMsg1;
				for (int j = 0; j < nRepeatCount; j++)
				{
					szBroadcastMessage += "...";
					szBroadcastMessage += szMsg1;					
				}
			}
			return true;
		}

        public void MonitorDetectFireProcess(SensorReactionLog log)
        {
            Thread t = new Thread(new ParameterizedThreadStart(MonitorDetectFireThread));
            t.Start(log);
        }

        // 화재 감지후 일정시간동안 진행사항이 있는지 감시
        private void MonitorDetectFireThread(object arg)
        {
            SensorReactionLog log = (SensorReactionLog)arg;

            while (!FormMain.Instance.FinishProcess)
            {
                TimeHistory _history = null;

                foreach (TimeHistory history in m_arTimeHistory)
                {
                    if (history.LastReactionLog == null)
                        continue;

                    if (history.LastReactionLog.SensorHistoryID == log.SensorHistoryID)
                    {
                        _history = history;
                        break;
                    }
                }

                if (_history == null)
                    break;

                // 추가 진행사항이 있으므로 Thread를 종료시킨다.
                if (_history.LastReactionLog.Type != SensorReactionLog.ReactionType.BEGIN_STATUS)
                    break;

                Thread.Sleep(2000);

                DateTime dtNow = DateTime.Now;
                TimeSpan span = dtNow - log.LogTime;

                if (span.TotalHours >= DetectFireTimeout)
                {
                    WriteIgnoreDetect(log, dtNow);
                    SendIgnoreDetect(log, ClientData.ClientType.SDMS_CLIENT);

                    break;
                }
            }
        }

        private void SendIgnoreDetect(SensorReactionLog log, ClientData.ClientType type)
        {
            if (log.SensorHistoryID < 0)
                return;

            byte[] bytes = new byte[11];

            bytes[0] = TCP_ID.IGNORE_DETECT_REPORT;
            bytes[1] = 1;

            byte[] sensorHistoryIDBytes = MakeBytes(log.SensorHistoryID);
            System.Buffer.BlockCopy(sensorHistoryIDBytes, 0, bytes, 2, sensorHistoryIDBytes.Length);

            foreach (ConnectionState state in m_arrClients)
            {
                ClientData client = (ClientData)state.Tag;
                if (client == null || client.Type == ClientData.ClientType.UNKNOWN)
                    continue;

                if (type == ClientData.ClientType.ALL || type == client.Type)
                {
                    Send(bytes, 0, bytes.Length, state);
                }
            }
        }

        private void WriteIgnoreDetect(SensorReactionLog log, DateTime dtNow)
        {
            string strMsg = string.Format("화재감지후 {0}시간동안 아무런 진행사항이 없어서 시스템이 상황을 종료시킵니다.",
                DetectFireTimeout);

            DBUtility.WebDBManager dbMgr = FormMain.Instance.DBManager;

            ArrayList arrResult = dbMgr.GetResultData("Select max(id) from SensorReactionHistory", 0);
            if (arrResult == null)
                return;

            int nID = arrResult.Count == 0 ? 1 : DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;

            string strSQL = string.Format("Insert into SensorReactionHistory (ID, SensorHistoryID, ReactionType, Time, Message, Param1, Param2) values ({0}, {1}, {2}, '{3}', '{4}', '', '')",
                nID, log.SensorHistoryID, (int)SensorReactionLog.ReactionType.IGNORE_FIRE,
                string.Format("{0} {1:00}:{2:00}:{3:00}", dtNow.ToShortDateString(), dtNow.Hour, dtNow.Minute, dtNow.Second),
                strMsg);

            dbMgr.GetResultData(strSQL, 0);
        }

        public void MonitorNotifyFireProcess(SensorReactionLog log)
        {
            Thread t = new Thread(new ParameterizedThreadStart(MonitorNotifyFireThread));
            t.Start(log);
        }

        // 화재 신고후 일정시간동안 진행사항이 있는지 감시
        private void MonitorNotifyFireThread(object arg)
        {
            SensorReactionLog log = (SensorReactionLog)arg;

            while (!FormMain.Instance.FinishProcess)
            {
                TimeHistory _history = null;

                foreach (TimeHistory history in m_arTimeHistory)
                {
                    if (history.LastReactionLog == null)
                        continue;

                    if (history.LastReactionLog.SensorHistoryID == log.SensorHistoryID)
                    {
                        _history = history;
                        break;
                    }
                }

                if (_history == null)
                    break;

                // 추가 진행사항이 있으므로 Thread를 종료시킨다.
                if (_history.LastReactionLog.Type != SensorReactionLog.ReactionType.NOTIFY_FIRE)
                    break;

                Thread.Sleep(2000);

                TimeSpan span = DateTime.Now - log.LogTime;

                if (span.TotalHours >= NotifyFireTimeout)
                {
                    SensorReactionLog log2 = log.Clone();
                    log2.Type = SensorReactionLog.ReactionType.IGNORE_SOP;
                    log2.Message = string.Format("화재신고후 {0}시간동안 아무런 진행사항이 없어서 시스템이 상황을 종료시킵니다.", (int)NotifyFireTimeout);
                    ProcessIgnoreSOP(log2, log2.SensorHistoryID);

                    break;
                }
            }
        }

        private void SendFireSensorSignal(SensorReactionLog log, ConnectionState state = null)
        {
            int nSensorZoneID = SensorManager.Instance.GetSensorID(log.SensorHistoryID);
            if (nSensorZoneID < 0)
                return;

            SensorZone sensor = FormMain.Instance.IOManager.GetSensorZone(nSensorZoneID);
            if (sensor == null)
                return;

            string strOriginSensorTableName = "";

            if (sensor.Type == 1)
                strOriginSensorTableName = "FireSensor";
            else if (sensor.Type == 2)
                strOriginSensorTableName = "SpringCooler";
            else if (sensor.Type == 3)
                strOriginSensorTableName = "PumpPressuerSensor";
            else
                return;

            string strSQL = string.Format("select sz.OrgSensorID, os.X, os.Y, os.Z from SensorZoneHistory as szh, SensorZone as sz, {0} as os where szh.ID = {1} and szh.SensorID = sz.ID and sz.OrgSensorID = os.ID",
                strOriginSensorTableName, log.SensorHistoryID);
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count < 4)
                return;

            int nSensorID = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            float x = DBUtility.WebDBManager.GetFloatField(arrResult[1].ToString(), 0.0f);
            float y = DBUtility.WebDBManager.GetFloatField(arrResult[2].ToString(), 0.0f);
            float z = DBUtility.WebDBManager.GetFloatField(arrResult[3].ToString(), 0.0f);

            if (nSensorID < 0)
                return;

            int nEquipZoneID = sensor.EquipZone == null ? -1 : sensor.EquipZone.ID;

            byte[] sensorIDBytes = MakeBytes(nSensorID);
            byte[] sensorHistoryIDBytes = MakeBytes(log.SensorHistoryID);
            byte[] zoneIDBytes = MakeBytes(nEquipZoneID);
            byte[] timeBytes = MakeBytes(log.LogTime.ToBinary());
            byte[] xBytes = MakeBytes(x);
            byte[] yBytes = MakeBytes(y);
            byte[] zBytes = MakeBytes(z);

            int nBlockLength = sensorIDBytes.Length + sensorHistoryIDBytes.Length + zoneIDBytes.Length + timeBytes.Length + xBytes.Length + yBytes.Length + zBytes.Length;
            byte[] bytes = new byte[2 + nBlockLength];

            bytes[0] = TCP_ID.FIRE_SENSOR_SIGNAL;
            bytes[1] = 7;

            int nIndex = 2;
            SensorReactionLog.CopyBytes(bytes, ref nIndex, sensorIDBytes);
            SensorReactionLog.CopyBytes(bytes, ref nIndex, sensorHistoryIDBytes);
            SensorReactionLog.CopyBytes(bytes, ref nIndex, zoneIDBytes);
            SensorReactionLog.CopyBytes(bytes, ref nIndex, timeBytes);
            SensorReactionLog.CopyBytes(bytes, ref nIndex, xBytes);
            SensorReactionLog.CopyBytes(bytes, ref nIndex, yBytes);
            SensorReactionLog.CopyBytes(bytes, ref nIndex, zBytes);

            if (state == null)
                SendData(bytes, true, ClientData.ClientType.SOP_SIMULATOR);
            else
                Send(bytes, 0, bytes.Length, state);
        }

		private void AddTempIgnoreSensor(SensorZone sensor)
		{
			if (!m_arrTempIgnoreSensors.Contains(sensor))
				m_arrTempIgnoreSensors.Add(sensor);

			//FormMain.Instance.SensorManager.AddIgnoreSensor(sensor);
		}

		private void RemoveTempIgnoreSensor(SensorZone sensor)
		{
			m_arrTempIgnoreSensors.Remove(sensor);
			//FormMain.Instance.SensorManager.RemoveIgnoreSensor(sensor);
		}

        private void ProcessMalfunction(byte[] bytes)
        {
            SensorReactionLog log = ReadMalfunctionReport(bytes);
            if (CheckSituation(log.SensorHistoryID))
            {
                FormMain.Instance.SensorManager.SetLastReadSensorHistoryID(log.SensorHistoryID);
                int nSensorID = FormMain.Instance.SensorManager.GetSensorID(log.SensorHistoryID);

                AddReactionLog(log);

                if (nSensorID > 0)
                {
                    SensorZone sensor = FormMain.Instance.IOManager.GetSensorZone(nSensorID);

                    if (sensor != null && sensor.SensorData == 1)
                    {
                        // 무시할 센서 리스트에 포함
                        AddTempIgnoreSensor(sensor);

						AbnormalSensorManager.Instance.Add(sensor.ID);

						SendSMS(log);
                    }
                }

                // Send Reaction Log
                SendSensorReactionLog(log, ClientData.ClientType.SDMS_CLIENT);

                RemoveSituation(log.SensorHistoryID);
            }
        }

        private bool SetClientType(byte[] bytes, ConnectionState state)
        {
            ClientData client = (ClientData)state.Tag;
            if (client == null)
                return false;

            int nClientType = BitConverter.ToInt32(bytes, 7);

            if (nClientType <= (int)ClientData.ClientType.ALL || nClientType >= (int)ClientData.ClientType.UNKNOWN)
                return false;

            client.Type = (ClientData.ClientType)nClientType;
            client.PingCount = 0;
            FormMain.Instance.UpdateClientType(state);
            return true;
        }

        private void PostProcessSensorData(int nHistoryID, int nPrevSensorHistoryID, int nSensorID, int nData, bool bConnected)
        {
			// comment by skkim : AbnormalSensorManager에서 대행
            // 임시로 무시된 Sensor List에서 해제할 것이 있는지 검사
			if (nSensorID > 0 && nData == 0)
			{
			    foreach (SensorZone sensor in m_arrTempIgnoreSensors)
			    {
			        if (sensor.ID == nSensorID)
			        {
			            RemoveTempIgnoreSensor(sensor);
			            break;
			        }
			    }
			}
			
			// Connection만 변경되는 경우 리턴값이 -2임
			if (nHistoryID == -2)
			{
				SendSensorZoneData(nData, nSensorID, ClientData.ClientType.SDMS_CLIENT);
			}

            if (nData == 1 && nHistoryID != -1)
            {
                if (!CheckSituation(nHistoryID))
                {
                    TimeHistory hs = new TimeHistory(nHistoryID, DateTime.Now);
                    m_arTimeHistory.Add(hs);

                    SendSensorZoneData(nData, nSensorID, ClientData.ClientType.SDMS_CLIENT);

                    SensorReactionLog log = CreateFireDetect(nHistoryID, nSensorID);
                    AddReactionLog(log);

					// 사내방송 실시
					RunBroadcast(log);
					SendSMS(log);

                    // Send Reaction Log
                    SendSensorReactionLog(log, ClientData.ClientType.SDMS_CLIENT);

                    hs.LastReactionLog = log;
                    // Send History ID
                    //SendSensorHistoryID(nHistoryID);

                    MonitorDetectFireProcess(log);
                }
            }
            else if (nData == 0 && nHistoryID != -1)
            {
                //int nPrevSensorHistoryID = SensorManager.Instance.GetSensorHistoryID(nSensorID, true, 1);

                if (nPrevSensorHistoryID > 0)
                {
                    TimeHistory history = FindTimeHistory(nPrevSensorHistoryID);

                    if (history != null && history.LastReactionLog != null/* && history.LastReactionLog.Type == SensorReactionLog.ReactionType.BEGIN_STATUS*/)
                    {
                        SendSensorZoneData(nData, nSensorID, ClientData.ClientType.SDMS_CLIENT);

                        // 화재 상황 종료
                        SendClearDetectReport(nPrevSensorHistoryID);
                        m_arTimeHistory.Remove(history);

                        SensorManager.Instance.RemoveSensorHistory(nPrevSensorHistoryID);
                        SensorManager.Instance.RemoveSensorHistory(nHistoryID);

                        if (history.LastReactionLog.Type == SensorReactionLog.ReactionType.BEGIN_STATUS)
                        {
                            SensorReactionLog log = new SensorReactionLog();

                            log.LogTime = DateTime.Now;
                            log.Message = "화재 신호가 무시되었습니다.";
                            log.SensorHistoryID = nPrevSensorHistoryID;
                            log.Type = SensorReactionLog.ReactionType.IGNORE_FIRE;

                            AddReactionLog(log);
                        }
                    }
                }
            }
        }

        private void SendSensorZoneData(int nData, int nSensorID, ClientData.ClientType type)
        {
            SensorZone sensor = FormMain.Instance.IOManager.GetSensorZone(nSensorID);
            if (sensor == null || sensor.EquipZone == null)
                return;

            byte[] sensorZoneIDBytes = MakeBytes(nSensorID);
            byte[] sensorTypeBytes = MakeBytes(sensor.Type);
            byte[] connectedBytes = MakeBytes(sensor.IsConnected ? 1 : 0);
            byte[] zoneIDBytes = MakeBytes(sensor.EquipZone.ID);
            byte[] dataBytes = MakeBytes(nData);
            byte[] linkedSensorIDBytes = MakeBytes(sensor.LinkedSensorID);

            byte[] bytes = new byte[2 + 9 * 6];

            bytes[0] = TCP_ID.SENSOR_ZONE_DATA;
            bytes[1] = 6;

            int nIndex = 2;
            SensorReactionLog.CopyBytes(bytes, ref nIndex, sensorZoneIDBytes);
            SensorReactionLog.CopyBytes(bytes, ref nIndex, sensorTypeBytes);
            SensorReactionLog.CopyBytes(bytes, ref nIndex, connectedBytes);
            SensorReactionLog.CopyBytes(bytes, ref nIndex, zoneIDBytes);
            SensorReactionLog.CopyBytes(bytes, ref nIndex, dataBytes);
            SensorReactionLog.CopyBytes(bytes, ref nIndex, linkedSensorIDBytes);

            foreach (ConnectionState state in m_arrClients)
            {
                ClientData client = (ClientData)state.Tag;
                if (client == null || client.Type == ClientData.ClientType.UNKNOWN)
                    continue;

                if (type == ClientData.ClientType.ALL || type == client.Type)
                {
                    Send(bytes, 0, bytes.Length, state);
                }
            }
        }

        private void SendClearDetectReport(int nSensorHistoryID)
        {
            byte[] historyBytes = MakeBytes(nSensorHistoryID);

            byte[] bytes = new byte[2 + historyBytes.Length];

            bytes[0] = TCP_ID.CLEAR_DETECT_REPORT;
            bytes[1] = 1;

            System.Buffer.BlockCopy(historyBytes, 0, bytes, 2, historyBytes.Length);

            foreach (ConnectionState state in m_arrClients)
            {
                ClientData data = (ClientData)state.Tag;

                if (data != null && (data.Type == ClientData.ClientType.SDMS_CLIENT || data.Type == ClientData.ClientType.SOP_SIMULATOR))
                {
                    //state.Write(bytes, 0, bytes.Length);
                    Send(bytes, 0, bytes.Length, state);
                }
            }
        }

        /*private void SendLastReactionLogList(ConnectionState state)
        {
            foreach (TimeHistory history in m_arTimeHistory)
            {
                if (history.LastReactionLog == null)
                    continue;

                byte[] bytes = history.LastReactionLog.MakeBytes();
                //state.Write(bytes, 0, bytes.Length);
                Send(bytes, 0, bytes.Length, state);
            }
        }*/

        private void RemoveSituation(int nHistoryID)
        {
            lock (this)
            {
                TimeHistory target = null;
                foreach (TimeHistory th in m_arTimeHistory)
                {
                    if (th.HistoryID == nHistoryID)
                    {
                        target = th;
                        break;
                    }
                }
                if (target != null)
                {
                    m_arTimeHistory.Remove(target);
                    SensorManager.Instance.RemoveSensorHistory(nHistoryID);
                }

                SendClearDetectReport(nHistoryID);
            }
        }
        private bool CheckSituation(int nHistoryID)
        {
            lock (this)
            {
                foreach( TimeHistory th in m_arTimeHistory)
                {
                    if (th.HistoryID == nHistoryID)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool CheckValidation(byte[] bytes)
        {
            int length = bytes.Length;
            if (length < 2)
                return false;

            int nChunkCount = (int)bytes[1];
            int nIndex = 2;

            for (int i = 0; i < nChunkCount; i++)
            {
                if (length < nIndex + 5)
                    return false;

                int nDataLength = BitConverter.ToInt32(bytes, nIndex + 1);

                if (length < nIndex + 5 + nDataLength)
                    return false;

                nIndex += 5 + nDataLength;
            }

            return true;
        }

        private void ProcessIgnoreSOP(byte[] bytes)
        {
            /*if (!CheckValidation(bytes))
                return;*/

            int nSensorHistoryID = BitConverter.ToInt32(bytes, 7);

            SensorReactionLog log = WriteIgnoreSOP(nSensorHistoryID);
            ProcessIgnoreSOP(log, nSensorHistoryID);
        }

        private void ProcessIgnoreSOP(SensorReactionLog log, int nSensorHistoryID)
        {
            if (log != null)
            {
                foreach (TimeHistory history in m_arTimeHistory)
                {
                    if (history.HistoryID == nSensorHistoryID)
                    {
                        history.LastReactionLog = log;
                        break;
                    }
                }

                FormMain.Instance.SensorManager.SetLastReadSensorHistoryID(nSensorHistoryID);
                SendData(log.MakeBytes(), true);
            }

            int nSensorID = SensorManager.Instance.GetSensorID(nSensorHistoryID);
            SensorZone sensor = FormMain.Instance.IOManager.GetSensorZone(nSensorID);

            System.Diagnostics.Trace.WriteLine(string.Format("IgnoreSOP, sensor Data : {0}", sensor.SensorData));

            if (sensor != null && sensor.SensorData == 0)
                RemoveSituation(nSensorHistoryID);
        }

        private void ProcessRunSOP(byte[] bytes)
        {
            int nSensorHistoryID = BitConverter.ToInt32(bytes, 7);

            int nDataLength = BitConverter.ToInt32(bytes, 12);
            string strActionStepHistoryID = Encoding.UTF8.GetString(bytes, 16, nDataLength);

            SensorReactionLog log = new SensorReactionLog();

            log.SensorHistoryID = nSensorHistoryID;
            log.Param1 = strActionStepHistoryID;

            ProcessRunSOP(log);
        }

        public void ProcessRunSOP(SensorReactionLog log)
        {
            Thread t = new Thread(new ParameterizedThreadStart(MonitorActionStepThread));
            t.Start(log);
        }

        private SensorReactionLog WriteRunSOPLog(int nSensorHistoryID, int nActionStepHistoryID)
        {
            string strSQL = "select ActionStepHistory.BeginTime, RealMode, SubCategoryName, DisasterName, StepName from ActionStepHistory, ActionStep, Disaster, SubDisasterCategory";
            strSQL += " where ActionStepHistory.ID = " + nActionStepHistoryID.ToString() + " and ActionStepHistory.ActionStepID = ActionStep.ID and ";
            strSQL += "ActionStep.DisasterID = Disaster.ID and Disaster.SubDisasterID = SubDisasterCategory.ID";

            DBUtility.WebDBManager dbMgr = FormMain.Instance.DBManager;

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;
            if (nResultCount < 5)
                return null;

            DateTime dtBegin = DBUtility.WebDBManager.GetDateTimeField(arrResult[0], new DateTime());
            bool isRealMode = DBUtility.WebDBManager.GetIntField(arrResult[1].ToString(), 0) == 0 ? false : true;
            string strSubCategoryName = DBUtility.WebDBManager.GetStringField(arrResult[2], "");
            string strDisasterName = DBUtility.WebDBManager.GetStringField(arrResult[3], "");
            string strStepName = DBUtility.WebDBManager.GetStringField(arrResult[4], "");

            string strMsg = string.Format("{0}/{1} {2} 단계의 SOP가 발동되었습니다.",
                isRealMode ? strSubCategoryName : "(훈련모드)" + strSubCategoryName,
                strDisasterName, strStepName);

            arrResult = dbMgr.GetResultData("Select max(id) from SensorReactionHistory", 0);
            if (arrResult == null)
                return null;

            int nID = arrResult.Count == 0 ? 1 : DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;

            strSQL = string.Format("Insert into SensorReactionHistory (ID, SensorHistoryID, ReactionType, Time, Message, Param1, Param2) values ({0}, {1}, {2}, '{3}', '{4}', '{5}', '')",
                nID, nSensorHistoryID, (int)SensorReactionLog.ReactionType.RUN_SOP,
                string.Format("{0} {1:00}:{2:00}:{3:00}", dtBegin.ToShortDateString(), dtBegin.Hour, dtBegin.Minute, dtBegin.Second),
                strMsg, nActionStepHistoryID.ToString());

            SensorReactionLog log = new SensorReactionLog();

            log.ID = nID;
            log.SensorHistoryID = nSensorHistoryID;
            log.Type = SensorReactionLog.ReactionType.RUN_SOP;
            log.LogTime = dtBegin;
            log.Message = strMsg;
            log.Param1 = nActionStepHistoryID.ToString();

            if (dbMgr.GetResultData(strSQL, 0) != null)
                return log;

            return null;
        }

        private SensorReactionLog WriteIgnoreSOP(int nSensorHistoryID)
        {
            DBUtility.WebDBManager dbMgr = FormMain.Instance.DBManager;

            ArrayList arrResult = dbMgr.GetResultData("Select max(id) from SensorReactionHistory", 0);
            if (arrResult == null)
                return null;

            int nID = arrResult.Count == 0 ? 1 : DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;
            DateTime dtNow = DateTime.Now;
            string strMessage = "상황종료";

            string strSQL = string.Format("Insert into SensorReactionHistory (ID, SensorHistoryID, ReactionType, Time, Message, Param1, Param2) values ({0}, {1}, {2}, '{3}', '{4}', '', '')",
                nID, nSensorHistoryID, (int)SensorReactionLog.ReactionType.IGNORE_SOP,
                string.Format("{0} {1:00}:{2:00}:{3:00}", dtNow.ToShortDateString(), dtNow.Hour, dtNow.Minute, dtNow.Second),
                strMessage);

            SensorReactionLog log = new SensorReactionLog();

            log.ID = nID;
            log.SensorHistoryID = nSensorHistoryID;
            log.Type = SensorReactionLog.ReactionType.IGNORE_SOP;
            log.LogTime = dtNow;
            log.Message = strMessage;

            if (dbMgr.GetResultData(strSQL, 0) != null)
                return log;

            return null;
        }

        private SensorReactionLog WriteRunNCancelSOPLog(int nSensorHistoryID, int nActionStepHistoryID, string strFormatMessage = null, bool selectCancelTime = true)
        {
            string strSQL = "select ActionStepHistory.CancelTime, RealMode, SubCategoryName, DisasterName, StepName from ActionStepHistory, ActionStep, Disaster, SubDisasterCategory";
            strSQL += " where ActionStepHistory.ID = " + nActionStepHistoryID.ToString() + " and ActionStepHistory.ActionStepID = ActionStep.ID and ";
            strSQL += "ActionStep.DisasterID = Disaster.ID and Disaster.SubDisasterID = SubDisasterCategory.ID";

            DBUtility.WebDBManager dbMgr = FormMain.Instance.DBManager;

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;
            if (nResultCount < 5)
                return null;

            DateTime dtCancel = selectCancelTime ? DBUtility.WebDBManager.GetDateTimeField(arrResult[0], new DateTime()) : DateTime.Now;
            bool isRealMode = DBUtility.WebDBManager.GetIntField(arrResult[1].ToString(), 0) == 0 ? false : true;
            string strSubCategoryName = DBUtility.WebDBManager.GetStringField(arrResult[2], "");
            string strDisasterName = DBUtility.WebDBManager.GetStringField(arrResult[3], "");
            string strStepName = DBUtility.WebDBManager.GetStringField(arrResult[4], "");

            if (strFormatMessage == null)
                strFormatMessage = "상황종료... {0}/{1} {2} 단계의 SOP가 실행 도중 취소되었습니다.";

            string strMsg = string.Format(strFormatMessage,
                isRealMode ? strSubCategoryName : "(훈련모드)" + strSubCategoryName,
                strDisasterName, strStepName);

            arrResult = dbMgr.GetResultData("Select max(id) from SensorReactionHistory", 0);
            if (arrResult == null)
                return null;

            int nID = arrResult.Count == 0 ? 1 : DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;

            strSQL = string.Format("Insert into SensorReactionHistory (ID, SensorHistoryID, ReactionType, Time, Message, Param1, Param2) values ({0}, {1}, {2}, '{3}', '{4}', '{5}', '')",
                nID, nSensorHistoryID, (int)SensorReactionLog.ReactionType.RUN_N_CANCEL_SOP,
                string.Format("{0} {1:00}:{2:00}:{3:00}", dtCancel.ToShortDateString(), dtCancel.Hour, dtCancel.Minute, dtCancel.Second),
                strMsg, nActionStepHistoryID.ToString());

            SensorReactionLog log = new SensorReactionLog();

            log.ID = nID;
            log.SensorHistoryID = nSensorHistoryID;
            log.Type = SensorReactionLog.ReactionType.RUN_N_CANCEL_SOP;
            log.LogTime = dtCancel;
            log.Message = strMsg;
            log.Param1 = nActionStepHistoryID.ToString();

            if (dbMgr.GetResultData(strSQL, 0) != null)
                return log;

            return null;
        }

        private SensorReactionLog WriteFinishSOPLog(int nSensorHistoryID, int nActionStepHistoryID)
        {
            string strSQL = "select ActionStepHistory.EndTime, RealMode, SubCategoryName, DisasterName, StepName from ActionStepHistory, ActionStep, Disaster, SubDisasterCategory";
            strSQL += " where ActionStepHistory.ID = " + nActionStepHistoryID.ToString() + " and ActionStepHistory.ActionStepID = ActionStep.ID and ";
            strSQL += "ActionStep.DisasterID = Disaster.ID and Disaster.SubDisasterID = SubDisasterCategory.ID";

            DBUtility.WebDBManager dbMgr = FormMain.Instance.DBManager;

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;
            if (nResultCount < 5)
                return null;

            DateTime dtEnd = DBUtility.WebDBManager.GetDateTimeField(arrResult[0], new DateTime());
            bool isRealMode = DBUtility.WebDBManager.GetIntField(arrResult[1].ToString(), 0) == 0 ? false : true;
            string strSubCategoryName = DBUtility.WebDBManager.GetStringField(arrResult[2], "");
            string strDisasterName = DBUtility.WebDBManager.GetStringField(arrResult[3], "");
            string strStepName = DBUtility.WebDBManager.GetStringField(arrResult[4], "");

            string strMsg = string.Format("상황종료... {0}/{1} {2} 단계의 SOP가 실행후 정상 종료되었습니다.",
                isRealMode ? strSubCategoryName : "(훈련모드)" + strSubCategoryName,
                strDisasterName, strStepName);

            arrResult = dbMgr.GetResultData("Select max(id) from SensorReactionHistory", 0);
            if (arrResult == null)
                return null;

            int nID = arrResult.Count == 0 ? 1 : DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;

            strSQL = string.Format("Insert into SensorReactionHistory (ID, SensorHistoryID, ReactionType, Time, Message, Param1, Param2) values ({0}, {1}, {2}, '{3}', '{4}', '{5}', '')",
                nID, nSensorHistoryID, (int)SensorReactionLog.ReactionType.FINISH_SOP,
                string.Format("{0} {1:00}:{2:00}:{3:00}", dtEnd.ToShortDateString(), dtEnd.Hour, dtEnd.Minute, dtEnd.Second),
                strMsg, nActionStepHistoryID.ToString());

            SensorReactionLog log = new SensorReactionLog();

            log.ID = nID;
            log.SensorHistoryID = nSensorHistoryID;
            log.Type = SensorReactionLog.ReactionType.FINISH_SOP;
            log.LogTime = dtEnd;
            log.Message = strMsg;
            log.Param1 = nActionStepHistoryID.ToString();

            if (dbMgr.GetResultData(strSQL, 0) != null)
                return log;

            return null;
        }

        private void MonitorActionStepThread(object arg)
        {
            SensorReactionLog log = (SensorReactionLog)arg;

            int nSensorHistoryID = log.SensorHistoryID;

            int nActionStepHistoryID;
            if (!int.TryParse(log.Param1, out nActionStepHistoryID))
                return;

            if (log.ID < 0)
                log = WriteRunSOPLog(nSensorHistoryID, nActionStepHistoryID);

            if (log == null)
                return;

            foreach (TimeHistory history in m_arTimeHistory)
            {
                if (history.HistoryID == log.SensorHistoryID)
                {
                    history.LastReactionLog = log;
                    break;
                }
            }

            SendData(log.MakeBytes());

            DBUtility.WebDBManager dbMgr = FormMain.Instance.DBManager;
            string strSQL = "select EndTime, CancelTime from ActionStepHistory where ID = " + log.Param1;

            DateTime dtBegin = log.LogTime;

            while (!FormMain.Instance.FinishProcess)
            {
                ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);
                if (arrResult == null)
                    break;

                if (arrResult.Count != 2)
                    break;

                if (string.Compare(arrResult[0].ToString(), "null", true) != 0)
                {
                    SensorReactionLog reactionLog = WriteFinishSOPLog(nSensorHistoryID, nActionStepHistoryID);

                    if (reactionLog != null)
                    {
                        FormMain.Instance.SensorManager.SetLastReadSensorHistoryID(nSensorHistoryID);
                        //SendFinishSOP(nSensorHistoryID, nActionStepHistoryID);
                        SendData(reactionLog.MakeBytes());
                        CloseSituation(nSensorHistoryID);
                    }
                    break;
                }

                if (string.Compare(arrResult[1].ToString(), "null", true) != 0)
                {
                    SensorReactionLog reactionLog = WriteRunNCancelSOPLog(nSensorHistoryID, nActionStepHistoryID);

                    if (reactionLog != null)
                    {
                        FormMain.Instance.SensorManager.SetLastReadSensorHistoryID(nSensorHistoryID);
                        //SendRunNCancelSOP(nSensorHistoryID, nActionStepHistoryID);
                        SendData(reactionLog.MakeBytes());

                        CloseSituation(nSensorHistoryID);
                    }
                    break;
                }

                Thread.Sleep(2000);

                if (m_dSOPTimeout > 0.0)
                {
                    TimeSpan span = DateTime.Now - dtBegin;
                    
                    if (span.TotalDays >= m_dSOPTimeout)
                    {
                        string strFormatMessage = "상황종료... {0}/{1} {2} 단계의 SOP가 실행후 " + ((int)m_dSOPTimeout).ToString() + "일이 경과할때까지 종료되지 않아 시스템에 의하여 실행 취소처리 되었습니다.";
                        SensorReactionLog reactionLog = WriteRunNCancelSOPLog(nSensorHistoryID, nActionStepHistoryID, strFormatMessage, false);

                        if (reactionLog != null)
                        {
                            FormMain.Instance.SensorManager.SetLastReadSensorHistoryID(nSensorHistoryID);
                            SendData(reactionLog.MakeBytes());
                            CloseSituation(nSensorHistoryID);
                        }

                        break;
                    }
                }
            }
        }

        private void CloseSituation(int nSensorHistoryID)
        {
            int nSensorID = SensorManager.Instance.GetSensorID(nSensorHistoryID);
            SensorZone sensor = FormMain.Instance.IOManager.GetSensorZone(nSensorID);

            if (sensor != null && sensor.SensorData == 0)
                RemoveSituation(nSensorHistoryID);
        }

        /*public void SendRunNCancelSOP(int nSensorHistoryID, int nActionStepHistoryID)
        {
            byte[] actionStepHistoryBytes = MakeBytes(nActionStepHistoryID.ToString());
            byte[] sensorHistoryBytes = MakeBytes(nSensorHistoryID);

            byte[] bytes = new byte[2 + actionStepHistoryBytes.Length + sensorHistoryBytes.Length];
            bytes[0] = TCP_ID.RUN_N_CANCEL_SOP;
            bytes[1] = 2;

            System.Buffer.BlockCopy(sensorHistoryBytes, 0, bytes, 2, sensorHistoryBytes.Length);
            System.Buffer.BlockCopy(actionStepHistoryBytes, 0, bytes, 2 + sensorHistoryBytes.Length, actionStepHistoryBytes.Length);

            SendData(bytes);
        }

        public void SendFinishSOP(int nSensorHistoryID, int nActionStepHistoryID)
        {
            byte[] actionStepHistoryBytes = MakeBytes(nActionStepHistoryID.ToString());
            byte[] sensorHistoryBytes = MakeBytes(nSensorHistoryID);

            byte[] bytes = new byte[2 + actionStepHistoryBytes.Length + sensorHistoryBytes.Length];
            bytes[0] = TCP_ID.FINISH_SOP;
            bytes[1] = 2;

            System.Buffer.BlockCopy(sensorHistoryBytes, 0, bytes, 2, sensorHistoryBytes.Length);
            System.Buffer.BlockCopy(actionStepHistoryBytes, 0, bytes, 2 + sensorHistoryBytes.Length, actionStepHistoryBytes.Length);

            SendData(bytes);
        }*/

		public override void OnDropConnection(ConnectionState state)
		{
            m_arrClients.Remove(state);

            FormMain.Instance.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                FormMain.Instance.RemoveClient(state);
            });
		}

        private void SendData(byte[] bytes, bool noLock = false, ClientData.ClientType type = ClientData.ClientType.ALL)
        {
            if (noLock)
            {
                lock (this)
                {
                    foreach (ConnectionState state in m_arrClients)
                    {
                        ClientData client = (ClientData)state.Tag;
                        if (client == null || client.Type == ClientData.ClientType.UNKNOWN)
                            continue;

                        if (client.Type == type || type == ClientData.ClientType.ALL)
                        {
                            //state.Write(bytes, 0, bytes.Length);
                            Send(bytes, 0, bytes.Length, state);
                        }
                    }
                }
            }
            else
            {
                foreach (ConnectionState state in m_arrClients)
                {
                    ClientData client = (ClientData)state.Tag;
                    if (client == null || client.Type == ClientData.ClientType.UNKNOWN)
                        continue;

                    if (client.Type == type || type == ClientData.ClientType.ALL)
                    {
                        //state.Write(bytes, 0, bytes.Length);
                        Send(bytes, 0, bytes.Length, state);
                    }
                }
            }
        }

        // 연결이 지속되고 있는지 여부를 확인하는 Thread
        private void PingThread()
        {
            byte[] data = new byte[2] { TCP_ID.ARE_YOU_THERE, 0 };
            byte[] data2 = new byte[2] { TCP_ID.WHO_ARE_YOU, 0 };

            while (m_isAliveThread)
            {
                //EnterLock();
                lock (this)
                {
                    int nClientCount = m_arrClients.Count;

                    for (int i = nClientCount - 1; i >= 0; i--)
                    {
                        ConnectionState state = (ConnectionState)m_arrClients[i];
                        ClientData client = (ClientData)state.Tag;

                        if (!state.Connected || client.PingCount >= 3)
                        {
                            state.EndConnection();
                            m_arrClients.RemoveAt(i);

                            FormMain.Instance.Invoke((System.Windows.Forms.MethodInvoker)delegate
                            {
                                FormMain.Instance.RemoveClient(state);
                            });
                        }
                        else
                        {
                            if (client.Type == ClientData.ClientType.UNKNOWN)
                            {
                                if (Send(data2, 0, data2.Length, state))
                                    client.PingCount++;
                            }
                            else if (Send(data, 0, data.Length, state))
                                client.PingCount++;
                        }
                    }

                    //ReleaseLock();
                }
                Thread.Sleep(1000);
            }
        }

        public void ReleaseThread()
        {
            m_isAliveThread = false;
        }

        public static byte[] MakeBytes(int data)
        {
            int nDataLength = sizeof(int);
            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.INTEGER;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

			int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = BitConverter.GetBytes(data);

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(long data)
        {
            int nDataLength = sizeof(long);
            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.LONG;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

			int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = BitConverter.GetBytes(data);

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(float data)
        {
            int nDataLength = sizeof(float);
            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.FLOAT;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = BitConverter.GetBytes(data);

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(double data)
        {
            int nDataLength = sizeof(double);
            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.DOUBLE;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = BitConverter.GetBytes(data);

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(string data)
        {
			UTF8Encoding enc = new UTF8Encoding();
			byte[] datas = enc.GetBytes(data);

			int nDataLength = datas.Length;

            byte[] bytes = new byte[5 + nDataLength];

            bytes[0] = TCP_TYPE.STRING;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            for (int i = 0; i < nDataLength; i++)
            {
				bytes[i + 1 + nCount] = datas[i];
            }

            return bytes;
        }

        /*public void SendSensorHistoryID(int nHistoryID)
        {
            lock (this)
            {
                if (m_arrClients.Count > 0)
                {
                    byte[] bytes = new byte[11];
                    bytes[0] = TCP_ID.SENSOR_HISTORY_ID;
                    bytes[1] = 1;

                    byte[] chunckBytes = MakeBytes(nHistoryID);
                    System.Buffer.BlockCopy(chunckBytes, 0, bytes, 2, chunckBytes.Length);

                    foreach (ConnectionState state in m_arrClients)
                    {
                        state.Write(bytes, 0, bytes.Length);
                    }
                }
            }
        }*/

		/*public void SendSensorHistoryIDList(ConnectionState state, ArrayList arrHistoryIDs)
		{
			lock (this)
			{
				int nHistoryCount = arrHistoryIDs.Count;

				if (m_arrClients.Count > 0 && nHistoryCount > 0)
				{
					byte[] bytes = new byte[2 + 9 * nHistoryCount];
					bytes[0] = TCP_ID.SENSOR_HISTORY_ID_LIST;
					bytes[1] = (byte)nHistoryCount;

					for (int i = 0; i < nHistoryCount; i++)
					{
						byte[] chunckBytes = MakeBytes((int)arrHistoryIDs[i]);
						System.Buffer.BlockCopy(chunckBytes, 0, bytes, 2 + 9 * i, chunckBytes.Length);
					}

                    //state.Write(bytes, 0, bytes.Length);					
                    Send(bytes, 0, bytes.Length, state);
				}
			}
		}

        public void SendSensorHistoryIDList(ArrayList arrHistoryIDs)
        {
            lock (this)
            {
                int nHistoryCount = arrHistoryIDs.Count;

                if (m_arrClients.Count > 0 && nHistoryCount > 0)
                {
                    byte[] bytes = new byte[2 + 9 * nHistoryCount];
                    bytes[0] = TCP_ID.SENSOR_HISTORY_ID_LIST;
                    bytes[1] = (byte)nHistoryCount;

                    for (int i = 0; i < nHistoryCount; i++)
                    {
                        byte[] chunckBytes = MakeBytes((int)arrHistoryIDs[i]);
                        System.Buffer.BlockCopy(chunckBytes, 0, bytes, 2 + 9 * i, chunckBytes.Length);
                    }

                    foreach (ConnectionState state in m_arrClients)
                    {
                        //state.Write(bytes, 0, bytes.Length);
                        Send(bytes, 0, bytes.Length, state);
                    }
                }
            }
        }*/

        public void SendSensorReactionLog(SensorReactionLog log, ClientData.ClientType type)
        {
            lock (this)
            {
                if (m_arrClients.Count > 0)
                {
                    byte[] bytes = log.MakeBytes();

                    foreach (ConnectionState state in m_arrClients)
                    {
                        ClientData client = (ClientData)state.Tag;
                        if (client == null || client.Type == ClientData.ClientType.UNKNOWN)
                            continue;

                        if (type == ClientData.ClientType.ALL || type == client.Type)
                        {
                            //state.Write(bytes, 0, bytes.Length);
                            Send(bytes, 0, bytes.Length, state);
                        }
                    }
                }
            }
        }

        private void SendCurrentFireSensorSignal(ConnectionState state, ClientData.ClientType type)
        {
            ClientData client = (ClientData)state.Tag;
            if (client == null)
                return;

            if (client.Type == ClientData.ClientType.UNKNOWN)
                return;

            if (type != ClientData.ClientType.ALL && type != client.Type)
                return;

            foreach (TimeHistory history in m_arTimeHistory)
            {
                if (history.LastReactionLog == null || history.LastReactionLog.Type != SensorReactionLog.ReactionType.NOTIFY_FIRE)
                    continue;

                SendFireSensorSignal(history.LastReactionLog, state);
                break;
            }
        }

        // SensorReactionLog가 하나도 없으면 2바이트만 전송된다.
        // 이를 받은 Client는 모든 화재 상황이 해제된다.
        public void SendSensorReactionLogList(ConnectionState state, ClientData.ClientType type)
        {
            ClientData client = (ClientData)state.Tag;
            if (client == null)
                return;

            if (client.Type == ClientData.ClientType.UNKNOWN)
                return;

            if (type != ClientData.ClientType.ALL && client.Type != type)
                return;

            ArrayList arrLogBytes = new ArrayList();
            int nByteCount = 0;

            foreach (TimeHistory history in m_arTimeHistory)
            {
                if (history.LastReactionLog == null)
                    continue;				
				
                byte[] dataBytes = history.LastReactionLog.MakeBytes();
                arrLogBytes.Add(dataBytes);

                nByteCount += dataBytes.Length - 2;
            }

            byte[] bytes = new byte[nByteCount + 2];

            bytes[0] = TCP_ID.SENSOR_REACTION_HISTORY_DATA_LIST;

            // 한꺼번에 보낼수 있는 Log의 개수는 36개가 한계다.
            // 그 이상의 Log는 byte 범위를 넘어서는 값을 가진다.
            int nLogCount = (int)arrLogBytes.Count;
            if (nLogCount > 36)
            {
                for (int i = 0; i < nLogCount - 36; i++)
                {
                    // 뒤에 있는 Log가 최근의 것이므로 앞의 Log를 삭제한다.
                    arrLogBytes.RemoveAt(0);
                }

                nLogCount = 36;
            }

            bytes[1] = (byte)(nLogCount * 7);
            int nIndex = 2;

            for (int i = 0; i < nLogCount;i++ )
            {
                byte[] dataBytes = (byte[])arrLogBytes[i];
                int nDataLength = dataBytes.Length - 2;
                System.Buffer.BlockCopy(dataBytes, 2, bytes, nIndex, nDataLength);
                nIndex += nDataLength;
            }

            Send(bytes, 0, bytes.Length, state);
        }

        public void AddTimeHistoryList(ArrayList arrTimeHistory)
        {
            foreach (TimeHistory history in arrTimeHistory)
            {
                m_arTimeHistory.Add(history);
            }
        }

        public int GetTimeHistoryCount()
        {
            return m_arTimeHistory.Count;
        }

        public TimeHistory GetTimeHistory(int nIndex)
        {
            if (nIndex >= m_arTimeHistory.Count || nIndex < 0)
                return null;

            return (TimeHistory)m_arTimeHistory[nIndex];
        }

        public TimeHistory FindTimeHistory(int nSensorHistoryID)
        {
            foreach (TimeHistory history in m_arTimeHistory)
            {
                if (history.HistoryID == nSensorHistoryID)
                    return history;
            }

            return null;
        }
    }
	public class TimeHistory
	{
		private int m_nSensorHistoryID = -1;
		public int HistoryID
		{
			get { return m_nSensorHistoryID; }
			set { m_nSensorHistoryID = value; }
		}
		
        private DateTime dtTime;
		public System.DateTime Time
		{
			get { return dtTime; }
			set { dtTime = value; }
		}

        private SensorReactionLog m_lastLog = null;
        public SensorReactionLog LastReactionLog
        {
            get { return m_lastLog; }
            set { m_lastLog = value; }
        }

		public TimeHistory(int nID, DateTime t)
		{
			dtTime = t;
			m_nSensorHistoryID = nID;
		}
	}

    public class ClientData
    {
        public enum ClientType { ALL = 0, SDMS_CLIENT, SOP_SIMULATOR, SENSOR_SIMULATOR, UNKNOWN };

        private int m_nPingCount = 0;
        private ClientType m_type = ClientType.UNKNOWN;

        public int PingCount
        {
            get { return m_nPingCount; }
            set { m_nPingCount = value; }
        }

        public ClientType Type
        {
            get { return m_type; }
            set { m_type = value; }
        }
    }
}
