using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TcpLib2;
using System.Collections;
using System.Threading;
using SDMS;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using SOP;
using UnE.Spatial;
using UnE.Sensor;

namespace SDMSServer
{

    public class ServiceProvider : TcpServiceProvider
    {
		[DllImport("kernel32.dll")]
		private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder refval, int size, string filepath);
				
		//private log4net.ILog logger = null;


        private ConcurrentDictionary<ConnectionState, ClientData> m_arrClients = new ConcurrentDictionary<ConnectionState, ClientData>();
        public object LockObject
        {
            get { return m_arrClients; }
        }

        //private bool m_isLock = false;
        private bool m_isAliveThread = true;

        // SOP 실행후 몇 일 이내에 종료되어야 하는가?
        private double m_dSOPTimeout = -1;
        // 화재 신고후 몇 시간 이내에 후속 작업이 진행되어야 하는가?
        private double m_dNotifyFireTimeout = -1;
        // 화재 탐지후 몇 시간 이내에 후속 작업이 진행되어야 하는가?
        private double m_dDetectFireTimeout = -1;

        private byte[] m_arrSelectMission = null;

        private string m_strSMSCaller = "07088983203";

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

		private bool m_bIsLogOpened = false;
		public bool IsLogOpened
		{
			get { return m_bIsLogOpened; }
			set { m_bIsLogOpened = value; }
		}

        // 임시로 무시할 센서들의 리스트
        private ArrayList m_arrTempIgnoreSensors = new ArrayList();
        public ArrayList TempIgnoreSensors
        {
            get { return m_arrTempIgnoreSensors; }
        }

        // Ping은 로그에 남기지 않는다.
        private bool m_exceptPingLog = true;

		private void WriteLog(object str)
        {
            if (ConnectionLogEx.Instance.IsOpened)
                ConnectionLogEx.Instance.Write(str);
        }

		private void WriteLineLog(object str)
        {
            if (ConnectionLogEx.Instance.IsOpened)
                ConnectionLogEx.Instance.WriteLine(str);
        }

        private void WriteLineLog(object str, Exception e)
        {
            if (ConnectionLogEx.Instance.IsOpened)
                ConnectionLogEx.Instance.WriteLine(str, e);
        }

        private void InitLog()
        {
			if (ConnectionLogEx.MakeInstance())
				m_bIsLogOpened = true;
			else
				m_bIsLogOpened = false;
        }

        public void RecvLog(byte[] bytes, ConnectionState state)
        {
			if (!IsLogOpened)
                return;

            if (bytes[0] != TCP_ID.I_AM_HERE || !m_exceptPingLog)
            {
                string strClient = "Unknown";

                ClientData data = (ClientData)state.Tag;

                if (data != null)
                {
                    strClient = TCP_CLIENT.GetClientTypeString(data.ClientType);
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

        // arrDropList가 null이 아닐 경우, 예외가 발생하면 바로 OnDropConnection()을 호출하지 않고 해당 state를 일단 arrDropList에 담아둔다.
        // m_arrClient Loop 실행 도중 OnDropConnection() 호출로 인하여 m_arrClient가 변경되는 것을 막기 위함이다.
        private bool _Send(byte[] bytes, int nOffset, int nLength, ConnectionState state, ArrayList arrDropList)
        {
            try
            {
                if (state.Connected == false)
                {
                    if (arrDropList == null)
                        OnDropConnection(state);
                    else
                        arrDropList.Add(state);

                    return false;
                }


                if (state.WriteAsync(bytes, nOffset, nLength))
                {
                    try
                    {
                        if (!IsLogOpened)
                            return true;

                        if (bytes[nOffset] != TCP_ID.ARE_YOU_THERE || !m_exceptPingLog)
                        {
                            StringBuilder sb = new StringBuilder();
                            string strClient = "Unknown";

                            ClientData data = (ClientData)state.Tag;
                            if (data != null)
                            {
                                strClient = TCP_CLIENT.GetClientTypeString(data.ClientType);
                            }

                            string szRemote = state.RemoteEndPoint.ToString();

                            sb.AppendFormat("SendMessage : Header({0}), Length({1}) to {2}({3})", (int)bytes[nOffset], nLength, strClient, szRemote);
 
                            bool bFirst = true;

                            foreach (byte b in bytes)
                            {
                                if (bFirst == true)
                                {
                                    bFirst = false;
                                    sb.AppendFormat("\r\n\t\t{0:X2}", (int)b);
                                }
                                else
                                    sb.AppendFormat(" {0:X2}", (int)b);
                            }

                            WriteLineLog(sb.ToString());
                        }
                    }
                    catch (System.Exception exx)
                    {
                        WriteLineLog("Write Send log", exx);
                    }                    
                    return true;
                }
                else
                {
                    if (arrDropList == null)
                        OnDropConnection(state);
                    else
                        arrDropList.Add(state);
                }
            }
            catch (Exception ex)
            {
                ConnectionLogEx.Instance.WriteLine("_Send", ex);

                if (arrDropList == null)
                    OnDropConnection(state);
                else
                    arrDropList.Add(state);

                return false;
            }           
            return false;
        }

        public bool Send(byte[] bytes, int nOffset, int nLength, ConnectionState state, bool noLock = false, ArrayList arrDropList = null)
        {
            if (!noLock)
            {

                    return _Send(bytes, nOffset, nLength, state, arrDropList);

            }

            return _Send(bytes, nOffset, nLength, state, arrDropList);
			/*lock(this)
			{	
				if (state.Write(bytes, nOffset, nLength))
				{
					if (!IsLogOpened)
						return true;

					if (bytes[nOffset] != TCP_ID.ARE_YOU_THERE || !m_exceptPingLog)
					{
						string strClient = "Unknown";

						ClientData data = (ClientData)state.Tag;

						if (data != null)
						{
							if (data.Type == TCP_CLIENT.SDMS_CLIENT)
								strClient = "SDMS Client";
							else if (data.Type == TCP_CLIENT.SENSOR_SIMULATOR)
								strClient = "Sensor Simulator";
							else if (data.Type == TCP_CLIENT.SOP_SIMULATOR)
								strClient = "SOP Simulator";
							else if (data.Type == TCP_CLIENT.SOP_MONITOR)
								strClient = "Sensor Monitor";
							else if (data.Type == TCP_CLIENT.SOP_RESOTRE)
								strClient = "Restore Manager";
							else if (data.Type == TCP_CLIENT.INTEGRATE_MANAGER)
								strClient = "Integrate Manager";
							else if (data.Type == TCP_CLIENT.SDMS_CLIENT_SECOND)
								strClient = "SDMS Client Sub Line";
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
			}
			return false;*/
        }

        private static ServiceProvider m_Instance = null;
        public static ServiceProvider Instance
        {
            get { return ServiceProvider.m_Instance; }
        }
        

        private Thread m_PingThread = null;
        public ServiceProvider()
        {
            m_Instance = this;

            InitLog();
            ReadOption();
            m_PingThread = new Thread(new ThreadStart(PingThread));
            m_PingThread.Start();
            
        }

        public string getinivalue(string section, string key, string filepath)
        {
            StringBuilder temp = new StringBuilder(255);
            int nLen = GetPrivateProfileString(section, key, "", temp, 255, filepath);

            return temp.ToString();
        }

        private void ReadOption()
        {
            DBUtility.WebDBManager dbMgr = NetworkServer.Instance.DBManager;
            string strSOPTimeout = "", strDetectTimeout = "", strNotifyTimeout = "";

            if (GetDBOption(dbMgr, "OptionSOPSimulator", "SopTimeout", ref strSOPTimeout))
                double.TryParse(strSOPTimeout, out m_dSOPTimeout);

            if (GetDBOption(dbMgr, "OptionSDMS", "DetectFireTimeout", ref strDetectTimeout))
                double.TryParse(strDetectTimeout, out m_dDetectFireTimeout);

            if (GetDBOption(dbMgr, "OptionSDMS", "NotifyFireTimeout", ref strNotifyTimeout))
                double.TryParse(strNotifyTimeout, out m_dNotifyFireTimeout);

            string szSMSCaller = m_strSMSCaller;
            if (GetDBOption(dbMgr, "OptionSDMS", "SMSCaller", ref szSMSCaller))
            {
                m_strSMSCaller = szSMSCaller;
            }
        }

        private bool GetDBOption(DBUtility.WebDBManager dbMgr, string strTableName, string strPropertyName, ref string strValue)
        {
            string strSQL = "Select PropertyValue from " + strTableName + " where PropertyName = '" + strPropertyName + "'";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return false;

            strValue = arrResult[0].ToString();
            return true;
        }

		public override object Clone()
		{
            return this;           
		}


        private List<TimeHistory> m_arTimeHistory = new List<TimeHistory>();
        public List<TimeHistory> TimeHistoryList
        {
            get { return m_arTimeHistory; }
        }

		public override void OnAcceptConnection(ConnectionState state)
		{
            if (m_isAliveThread == false)
                return;

            //lock (m_arrClients)
            {
                //state.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);
                ClientData data = new ClientDataUnknown(this);
                state.Tag = data;
                if (m_arrClients.TryAdd(state, data))
                {
                    SendMessage(TCP_ID.WHO_ARE_YOU, state);
                    NetworkServer.Instance.AddClient(state);	
                }                		
            }
		}

        // Header만 있는 메시지 보내기
        private void SendMessage(byte header, ConnectionState state)
        {
            byte[] bytes = new byte[6] { header, 0, 0, 0, 0, 0 };
            try
            {
                Send(bytes, 0, bytes.Length, state);
            }
            catch (System.Exception ex)
            {
                ConnectionLogEx.Instance.WriteLine("SendMessage : " + header, ex);
            }            
        }

		public void ResetSensorData(int nID)
		{
			string szSQP = string.Format("UPDATE SensorZone set Data=0 , Connected=1 where ID={0}", nID);
			NetworkServer.Instance.DBManager.GetResultData(szSQP, 0);
		}

		public SensorReactionLog ReadFailReport()
		{
			SensorReactionLog log = new SensorReactionLog();
			return log;
		}

        //private string m_strTranning = "[훈련상황]";
		private object m_bLockObj = new object();
		public void AddReactionLog(SensorReactionLog log)
		{
			if (DataManager.GetTranningMode())
			{
                // 171114 KYJ
                log.Message = GetTranningMessage() + log.Message;
                //
                //log.Message = m_strTranning + log.Message;
			}

            DdMonitor.Enter(m_bLockObj, true);			
			{
				string strSQL = "Select max(ID) from SensorReactionHistory";
				ArrayList arrResult = NetworkServer.Instance.DBManager.GetResultData(strSQL, 0);

				int nReactionHistoryID = -1;
				if (arrResult == null)
					return;
				if (arrResult.Count == 0)
					nReactionHistoryID = 1;
				else
					nReactionHistoryID = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;

				log.LogTime = DateTime.Now;
				string strDateTimeField = string.Format("{0} {1}:{2}:{3}", log.LogTime.ToShortDateString(), log.LogTime.Hour, log.LogTime.Minute, log.LogTime.Second);
				strSQL = string.Format("Insert into SensorReactionHistory (ID, SensorHistoryID, ReactionType, Time, Message, Param1, Param2, Param3, Param4, Param5, DetectionStatus) values ({0}, {1}, {2}, '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', {10})",
				   nReactionHistoryID, log.SensorHistoryID, (int)log.Type, strDateTimeField, log.Message, log.Param1, log.Param2, log.Param3, log.Param4, log.Param5, (int)log.Status);
				log.ID = nReactionHistoryID;
				NetworkServer.Instance.DBManager.GetResultData(strSQL, 0);

                // 171115 KYJ
                // 오작동일 경우 ReactionType = 0에 해당하는 DetectionStatus를 2로 바꾸어 줘야 리스트에도 오작동으로 표시된다.
                if ((int)log.Status == 2)
                {
                    string query = string.Format("Update SensorReactionHistory set DetectionStatus=2 where SensorHistoryID={0} and ReactionType=0", log.SensorHistoryID);
                    NetworkServer.Instance.DBManager.GetResultData(query, 0);
                }
                //

                ServiceProvider.WriteSensorReactionHistoryDescription(log, NetworkServer.Instance.DBManager);
			}
            DdMonitor.Exit(m_bLockObj, true);

			// SMS 전송로그는 사용자에게 전송할 필요 없다.
            if (log.Type == libSensorProcess.ReactionType.SEND_SMS)
				return;
			// 방송메시지는 사용자에게 전송할 필요없다.
            if (log.Type == libSensorProcess.ReactionType.RUN_BROADCAST)
				return;

            if (log.SensorHistoryID > 0)
            {
                foreach (TimeHistory history in m_arTimeHistory)
                {
                    if (history.LastReactionLog == null && history.HistoryID == log.SensorHistoryID)
                    {
                        history.LastReactionLog = log;
                        break;
                    }
                    else if (history.LastReactionLog != null && history.LastReactionLog.SensorHistoryID == log.SensorHistoryID)
                    {
                        history.LastReactionLog = log;
                        break;
                    }
                }
            }
            SensorManager.Instance.SetLastReadSensorHistoryID(log.SensorHistoryID - 1);
		}

        // 171114 KYJ
        private string GetTranningMessage()
        {
            string strTranning = "";
            string query = "SELECT PropertyValue FROM OptionSDMS where PropertyName ='HeaderMsg'";
            ArrayList arrResult = NetworkServer.Instance.DBManager.GetResultData(query, 0);
            if (arrResult != null && arrResult.Count != 0)
                strTranning = DBUtility.WebDBManager.GetStringField(arrResult[0].ToString(), "");

            if (!strTranning.Equals(""))
                strTranning = "[" + strTranning + "]";

            return strTranning;
        }
		
		public override bool OnReceiveData(ConnectionState state)
		{			
			
            if (!base.OnReceiveData(state))
                return false;
            
            ClientData client = (ClientData)state.Tag;
            if (client == null)
                return false;
                
            //WriteByteArray(state.RecivedBuffer);
                
            bool bResult = client.OnReceiveData(state, state.RecivedBuffer);
            state.RecivedBuffer = null;
            return bResult;
		}

        private void WriteByteArray(byte[] bytes)
        {
            Debug.Write("{");
            for (int i = 0; i < bytes.Length; i++)
            {
                Debug.Write(string.Format("{0:X}", bytes[i]));
                Debug.Write(" ");
            }
            Debug.WriteLine("}");
        }

		private string MakeSMSMessage(SensorReactionLog log)
		{
			return log.Message;
		}

		public string GetSendPhoneNumber()
		{
            return m_strSMSCaller;// "07088982203";
		}
		
		private ArrayList GetOperatorPhoneNumber(SensorReactionLog log, bool isDetectTime)
        {
			ArrayList arrPhoneNumbers = new ArrayList();

            using(DdMonitor.Lock (NetworkServer.Instance.MemberCriticalSection))
            {
                // FacilityManager table
                //시설물 Type : 0(화재탐지센서), 1(스프링쿨러), 2(펌프압력센서), 3(CCTV), 4(소화기), 5(소화전), 6(발신기)
                //0(CompanyMember), 1(RegularTeam), 2(ExternalCompanyMember), 3(ExternalCompanyTeam)
                // MemberType이 1(RegularTeam)일 경우에만 사용. 몇 급이상만 담당자로 지정할 것인지 설정. NULL이면 팀원 모두. ex)4 => 4급 이상
                //return "01043632290";

                // 당직자에 전송이 설정된경우, 설정시간이 30시간 이네인 경우, 메시지 전송한다.
                // 당직자 전화번호를 가져온다.
                /*if (GetSendDutyConfig())
                {
                    string szDutyPhoneNumber = GetNightDutyPhoneNumber();
                    if (szDutyPhoneNumber != null && szDutyPhoneNumber != "")
                    {
                        arrPhoneNumbers.Add(szDutyPhoneNumber);
                    }
                }*/
                
                int nSensorZoneID = SensorManager.Instance.GetSensorID(log.SensorHistoryID);

                // 신호가 종료되어 HistoryID로부터 SensorZoneID를 가져오지 못하는경우
                if( nSensorZoneID == -1)
                {
                    try
                    {
                        string szSensorID = log.Param2;
                        //nSensorZoneID = Convert.ToInt32(szSensorID);
                        int.TryParse(szSensorID, out nSensorZoneID);
                    }catch(Exception)
                    {}
                }

                SensorZone sensor = NetworkServer.Instance.IOManager.GetSensorZone(nSensorZoneID);

                // 수동 신고의 경우
                if (log.Param2 == "0")
                {
                    int nZoneID = -1;
                    if (int.TryParse(log.Param1, out nZoneID))
                    {
                        Zone zone = ZoneManager.Instance.GetZone(nZoneID);
                        ArrayList arEquipZone = ZoneManager.Instance.GetEquipmentZoneList(zone);
                        if (arEquipZone != null && arEquipZone.Count > 0)
                        {
                            EquipmentZone equipZone = (EquipmentZone)arEquipZone[0];
                            IFacility.FacilityType type = IFacility.FacilityType.FIRE_SENSOR;
                            if (!AddPhoneNumbers(type, equipZone, arrPhoneNumbers, isDetectTime))
                            {
                                AddPhoneNumbers(type, zone, arrPhoneNumbers, isDetectTime);

                            }
                        }
                    }
                }
                else
                {
                    if (nSensorZoneID < 0)
                        return arrPhoneNumbers;

                    if (sensor == null || sensor.EquipZone == null)
                        return arrPhoneNumbers;

                    IFacility.FacilityType type = sensor.Type;

                    // 가스 방출신호를 제외한 모든 자탐 신호는 화재 신호로 간주한다.
                    // 서울대 프로젝트에서 방범센터 카테고리가 추가됨 2017-04-14
                    if (type >= IFacility.FacilityType.FireSensor_TypeA && type <= IFacility.FacilityType.FireSensor_AnalogSmokeType && type != IFacility.FacilityType.FireSensor_GasEmission)
                        type = IFacility.FacilityType.FIRE_SENSOR;
                    
                    // 서울대 프로젝트에서 카테고리가 추가됨 2017-04-14
                    if (type == IFacility.FacilityType.Fire_S1 || type == IFacility.FacilityType.FireF1_S1 ||
                        type == IFacility.FacilityType.SecomFire)
                        type = IFacility.FacilityType.FIRE_SENSOR;
                    
                    switch(type)
                    {
                        case IFacility.FacilityType.Intrusion_S1:
                        case IFacility.FacilityType.Loiter_S1:
                        case IFacility.FacilityType.Collapse_S1:
                        case IFacility.FacilityType.Theft_S1:
                        case IFacility.FacilityType.Neglect_S1:
                        case IFacility.FacilityType.VirtualFence_S1:
                        case IFacility.FacilityType.EmergencyBell_S1:
                        case IFacility.FacilityType.GeneralIntrusionT1_S1:
                        case IFacility.FacilityType.GeneralIntrusionT2_S1:
                        case IFacility.FacilityType.InternalIntrusionT3_S1:
                        case IFacility.FacilityType.VaultIntrusionT4_S1:
                        case IFacility.FacilityType.CustomerEmergencyC1_S1:
                        case IFacility.FacilityType.CustomerEmergencyC2_S1:
                        case IFacility.FacilityType.RescueQQ_S1:
                        case IFacility.FacilityType.GasG1_S1:
                        case IFacility.FacilityType.BlackoutAbnormalityU1_S1:
                        case IFacility.FacilityType.LeakAbnormalityU4_S1:
                        case IFacility.FacilityType.SynthesisAlertAbnormalityU8_S1:
                        case IFacility.FacilityType.ExternalAlarmBell:
                        case IFacility.FacilityType.SecomExternalAlarmBell:
                        case IFacility.FacilityType.SecomWomenAlarmBell:
                            type = IFacility.FacilityType.Security_Sensor;
                            break;
                    }

                    if (!AddPhoneNumbers(type, sensor.EquipZone, arrPhoneNumbers, isDetectTime))
                    {
                        foreach (Zone zone in sensor.EquipZone.LinkedZoneList)
                        {
                            AddPhoneNumbers(type, zone, arrPhoneNumbers, isDetectTime);
                        }
                    }
                }
            }

			return arrPhoneNumbers;
		}

		private bool AddPhoneNumbers(IFacility.FacilityType type, EquipmentZone zone, ArrayList arrPhoneNumbers, bool isDetectTime)
		{
			if (zone != null)
			{
				//Facility.FacilityType type = (Facility.FacilityType)sensor.Type;
				FacilityManagerGroup group = null;
				group = DataManager.Instance.GetEquipZoneFacilityManagerGroup(type, zone, isDetectTime);

				ArrayList arNewNum = new ArrayList();
				AddPhoneNumberFromGroup(arNewNum, group);
				//if (arNewNum.Count == 0)
				//	return false;

                // EquipZone FacilityManager 뿐만 아니라 건물 Manager와 전체 Manager까지 모두 포함한다.
                if (zone.LinkedZoneList != null && zone.LinkedZoneList.Count > 0)
					AddPhoneNumbers(type, (Zone)zone.LinkedZoneList[0], arNewNum, isDetectTime);
                else
                    AddPhoneNumberFromGroup(arNewNum, DataManager.Instance.GetEntireFacilityManagerGroup(type, isDetectTime));

				arrPhoneNumbers.AddRange(arNewNum);
				return true;
			}
			return false;
		}

        /*private string GetNightDutyPhoneNumber()
        {
            DBUtility.WebDBManager dbMgr = NetworkServer.Instance.DBManager;

            string szSQL = "select ID, MemberID, TeamID, InsertTime, Description from duty";
            ArrayList arResult = dbMgr.GetResultData(szSQL, 0);
            if( arResult == null || arResult.Count == 0)
                return null;

            int nCount = arResult.Count;
            for (int i = 0; i < nCount - 3; i += 4)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arResult[i].ToString(), -1);
                int nMemberID = DBUtility.WebDBManager.GetIntField(arResult[i + 1].ToString(), -1);
                int nTeamID = DBUtility.WebDBManager.GetIntField(arResult[i + 2].ToString(), -1);
                DateTime insertTime = DBUtility.WebDBManager.GetDateTimeField(arResult[i + 3], DateTime.Now);
                string szDesc = DBUtility.WebDBManager.GetStringField(arResult[i + 4].ToString(),"");

                DateTime dtNow = DateTime.Now;
                TimeSpan span = dtNow - insertTime;
                double nTime = span.TotalHours;
                if (nTime < 30.0)
                {
                    DataCompanyMember member = DataManager.Instance.GetReqularTeamMembers(nTeamID, nID);
                    if (member == null)
                        return null;

                    return member.PhoneNumber;
                }
            }
            return null;
        }*/

		private void AddPhoneNumbers(IFacility.FacilityType type, Zone zone, ArrayList arrPhoneNumbers, bool isDetectTime)
        {
            Building building = zone.Building;

            //Facility.FacilityType type = (Facility.FacilityType)sensor.Type;
            FacilityManagerGroup group = null;

            if (building == null)
                group = DataManager.Instance.GetOutdoorFacilityManagerGroup(type, zone, isDetectTime);
            else
                group = DataManager.Instance.GetBuildingFacilityManagerGroup(type, building, isDetectTime);

            AddPhoneNumberFromGroup(arrPhoneNumbers, group);
            
            // 건물별 담당자 뿐만 아니라 전체 담당자에게도 문자메시지를 보낸다.
            AddPhoneNumberFromGroup(arrPhoneNumbers, DataManager.Instance.GetEntireFacilityManagerGroup(type, isDetectTime));
            
        }

        private void AddPhoneNumberFromGroup(ArrayList arrPhoneNumbers, FacilityManagerGroup group)
        {
            if (group == null)
                return;

            foreach (FacilityManager mgr in group.CompanyMembers)
            {
                AddPhoneNumber(arrPhoneNumbers, mgr);
            }

            // 171114 KYJ TEST
            //
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

            foreach (FacilityManager mgr in group.ControlRoomMembers)
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

                if (arrPhoneNumbers.Contains(member))
                    return;

                arrPhoneNumbers.Add(member);
            }
            else if (mgr.MemberType == 1 || mgr.MemberType == 4)
            {
                DataTeam team = (DataTeam)mgr.Tag;
                AddRegularTeamPhoneNumber(arrPhoneNumbers, team, mgr);
             
            }
            else if (mgr.MemberType == 2)
            {
                DataExternalMember member = (DataExternalMember)mgr.Tag;

                if (member == null)
                    return;

                if (arrPhoneNumbers.Contains(member))
                    return;

                arrPhoneNumbers.Add(member);
            }
            else if (mgr.MemberType == 3 || mgr.MemberType == 5)
            {
                DataTeam team = (DataTeam)mgr.Tag;
                AddExternalTeamPhoneNumber(arrPhoneNumbers, team);

            }
            else if (mgr.MemberType == 6)
            {
                //AddDutyMemberTeamPhoneNumber(arrPhoneNumbers);
            }
            else if (mgr.MemberType == 7)
            {
                DataTeamControlRoom team = (DataTeamControlRoom)mgr.Tag;
                AddControlRoomPhoneNumbers(arrPhoneNumbers, team);
            }
        }

        private void AddControlRoomPhoneNumbers(ArrayList arrPhoneNumbers, DataTeamControlRoom team)
        {
            int nRoomID = team.ControlRoomID;
            int nPositionID = team.ControlTeamJobPositionID;
            string strSQL = "";

            if (nRoomID == 0)
            {
                strSQL = "select ctm.MemberType, ctm.MemberID ";
                strSQL += "from ControlRoom as cr, ControlWorkingTeam as cwt, ControlTeamMembers as ctm ";
                strSQL += "where cr.ID = cwt.RoomID and ctm.RoomID = cr.ID and ctm.TeamID = cwt.TeamID and ctm.MemberID is not NULL";
            }
            else if (nPositionID == 0)
            {
                strSQL = "select ctm.MemberType, ctm.MemberID ";
                strSQL += "from ControlRoom as cr, ControlWorkingTeam as cwt, ControlTeamMembers as ctm ";
                strSQL += string.Format("where cr.ID = cwt.RoomID and ctm.RoomID = cr.ID and ctm.TeamID = cwt.TeamID and cr.ID = {0} and ctm.MemberID is not NULL", nRoomID);
            }
            else
            {
                strSQL = "select ctm.MemberType, ctm.MemberID ";
                strSQL += "from ControlRoom as cr, ControlWorkingTeam as cwt, ControlTeamMembers as ctm ";
                strSQL += "where cr.ID = cwt.RoomID and ctm.RoomID = cr.ID and ctm.TeamID = cwt.TeamID and ";
                strSQL += string.Format("ctm.JobPosition = {0} and cr.ID = {1} and ctm.MemberID is not NULL", nPositionID, nRoomID);
            }

            DBUtility.WebDBManager dbMgr = NetworkServer.Instance.DBManager;
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nMemberType = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nMemberID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);

                object member = null;

                if (nMemberType == 1)
                    member = DataManager.Instance.GetRegularMember(nMemberID);
                else if (nMemberType == 4)
                    member = DataManager.Instance.GetExternalMember(nMemberID);

                if (member != null)
                {
                    if (!arrPhoneNumbers.Contains(member))
                        arrPhoneNumbers.Add(member);
                }
            }
        }

        /*private void AddDutyMemberTeamPhoneNumber(ArrayList arrPhoneNumbers)
        {
            DBUtility.WebDBManager dbMgr = NetworkServer.Instance.DBManager;

            string strSQL = "select ctm.MemberType, ctm.MemberID ";
            strSQL += "from ControlRoomType as crt, ControlRoom as cr, ControlTeamMembers as ctm ";
            strSQL += "where crt.TypeName = '당직실' and cr.RoomType = crt.ID and ctm.RoomID = cr.ID and crt.SiteID = " + NetworkServer.Instance.SiteID.ToString();

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;
            
            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                DBUtility.VariousData<int> memberType = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString());
                DBUtility.VariousData<int> memberID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString());

                if (memberType == null || memberID == null)
                    continue;

                if (memberType.Data == 1)
                {
                    DataCompanyMember member = DataManager.Instance.GetRegularMember(memberID.Data);

                    if (member == null)
                        continue;

                    if (!arrPhoneNumbers.Contains(member))
                        arrPhoneNumbers.Add(member);
                }
                else if (memberType.Data == 4)
                {
                    DataExternalMember member = DataManager.Instance.GetExternalMember(memberID.Data);

                    if (member == null)
                        continue;

                    if (!arrPhoneNumbers.Contains(member))
                        arrPhoneNumbers.Add(member);
                }
            }
        }*/
        /*private void AddDutyMemberTeamPhoneNumber(ArrayList arrPhoneNumbers)
        {
            DBUtility.WebDBManager dbMgr = NetworkServer.Instance.DBManager;

            // 30시간 이내에 지정된 당직자만 검색
            string strSQL = "select memberID from Duty where InsertTime between DATEADD(hour, -30, getdate()) and getdate()";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount; i++)
            {
                int nCompanyMemberID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);

                DataCompanyMember member = DataManager.Instance.GetRegularMember(nCompanyMemberID);

                if (member == null)
                    continue;

                if (!arrPhoneNumbers.Contains(member))
                    arrPhoneNumbers.Add(member);
            }
        }*/

		public ArrayList GetAllMemberPhoneNumber()
		{
			ArrayList arrPhoneNumber = new ArrayList();

			ArrayList arrCompanyMember = DataManager.Instance.GetAllCompanyMember();
			foreach (DataCompanyMember data in arrCompanyMember)
			{
				arrPhoneNumber.Add(data);
			}
			return arrPhoneNumber;
		}

        private void AddExternalTeamPhoneNumber(ArrayList arrPhoneNumbers, DataTeam team)
        {
            if (team == null)
                return;

            ArrayList arrMembers = DataManager.Instance.GetTeamMembers(team);

            if (arrMembers != null)
            {
                foreach (DataExternalMember member in arrMembers)
                {
                    if (arrPhoneNumbers.Contains(member))
                        continue;

                    arrPhoneNumbers.Add(member);
                }
            }

            foreach (DataTeam childTeam in team.ChildTeams)
            {
                AddExternalTeamPhoneNumber(arrPhoneNumbers, childTeam);
            }
        }

        private void AddRegularTeamPhoneNumber(ArrayList arrPhoneNumbers, DataTeam team, FacilityManager mgr)
        {
            if (team == null)
                return;

            ArrayList arrMembers = DataManager.Instance.GetTeamMembers(team);

            if (arrMembers != null)
            {
                foreach (DataCompanyMember member in arrMembers)
                {
                    if (arrPhoneNumbers.Contains(member))
                        continue;

					if (mgr.LevelLimit > 0)
					{
						if (mgr.UpperLimit > 0)
						{
							// member.LevelID 또는 그 상위 직급에게 문자메시지를 보낸다.
							if (member.LevelID > 0 && member.LevelID <= mgr.LevelLimit)
							{
								arrPhoneNumbers.Add(member);
							}
						}
						else if (mgr.UpperLimit < 0)
						{
							// member.LevelID 또는 그 하위 직급에게 문자메시지를 보낸다.
							if ((member.LevelID > 0 && member.LevelID >= mgr.LevelLimit) ||
								member.LevelID == 0)
							{
								arrPhoneNumbers.Add(member);
							}
						}
						else
						{
							if (member.LevelID == mgr.LevelLimit)
							{
								arrPhoneNumbers.Add(member);
							}
						}
					}
					else
					{
						arrPhoneNumbers.Add(member);
					}
                }
            }

            foreach (DataTeam childTeam in team.ChildTeams)
            {
                AddRegularTeamPhoneNumber(arrPhoneNumbers, childTeam, mgr);
            }
        }


        // 서울대에서는 전체 문자가 없으므로 사용하지 않도록 한다. 2017-04-14
        public void SendSMSToAllCompanyMember(SensorReactionLog log, SMSManager.SMSMessageType smsType)
		{            
            if (log == null)
                return;

            bool bUseSMS = GetSMSConfig(smsType);
            //bool bUseSMS = GetSMSConfig(nSmsType);
            if (bUseSMS == false)
                return;

            // 센서와 연결된 담당자 전화번호 가져오기
            ArrayList arrOperatorPhoneNumbers = GetOperatorPhoneNumber(log, false);

            // 전체 인원에 대한 DataMember를 가져온다.
            ArrayList arrPhoneNumbers = GetAllMemberPhoneNumber();

            // 전체인원과 운영자들이 없는 경우
            if (arrPhoneNumbers == null && arrOperatorPhoneNumbers == null)
                return;

            // 전체 인원이 없는 경우, 운영자 사용
            if (arrPhoneNumbers == null)
                arrPhoneNumbers = arrOperatorPhoneNumbers;
            else
            {
                // 운영자 목록이 있는경우 전체 목록에 중복되지 않도록 추가해준다.
                if (arrOperatorPhoneNumbers != null && arrOperatorPhoneNumbers.Count > 0)
                {
                    foreach (DataMember member in arrOperatorPhoneNumbers)
                    {
                        if (!arrPhoneNumbers.Contains(member))
                        {
                            arrPhoneNumbers.Add(member);
                        }
                    }
                }
                
            }

            if (arrPhoneNumbers == null || arrPhoneNumbers.Count == 0)
                return;

            // 사전 정의된 메시지 가져오기
            string szMsg = MakeSMSMessage(log);
            // 발신자 번호 가져오기
            string szSendNum = GetSendPhoneNumber();
            // 문자 메시지 보내기
            //if (szPhone != "" && szMsg != "")
            if (szMsg != "")
            {
                SensorReactionLog smsLog = new SensorReactionLog();
                smsLog.Message = "전체인원에게 메시지가 전송되었습니다. 내용 : " + szMsg;
                smsLog.Param1 = log.Param1;
                smsLog.Param2 = log.Param2;
                smsLog.SensorHistoryID = log.SensorHistoryID;
                smsLog.Type = libSensorProcess.ReactionType.SEND_SMS;
                AddReactionLog(smsLog);
                
                // Send SMS
                // 메시지 전송 부분만 쓰레드로 처리해야 함. (중요), skkim 2014.12.16
                new Thread(() =>
                {
                    SMSManager.Instance.SendSMS(arrPhoneNumbers, szSendNum, szMsg, true);

                    SaveSMSHistory(arrPhoneNumbers, smsLog.SensorHistoryID, smsLog.ID, szMsg, 1, true);

                }).Start();
            }
		}

        public void SendSMSToAllCompanyMember(string strMessage)
        {
            if (strMessage.Length == 0)
                return;

            // 전체 인원에 대한 DataMember를 가져온다.
            ArrayList arrPhoneNumbers = GetAllMemberPhoneNumber();

            // 전체인원이 없는 경우
            if (arrPhoneNumbers == null || arrPhoneNumbers.Count == 0)
                return;

            // 발신자 번호 가져오기
            string szSendNum = GetSendPhoneNumber();

            // 문자 메시지 보내기
            if (strMessage != "")
            {
                /*SensorReactionLog smsLog = new SensorReactionLog();
                smsLog.Message = "전체인원에게 메시지가 전송되었습니다. 내용 : " + strMessage;
                smsLog.Type = SensorReactionLog.ReactionType.SEND_SMS;
                AddReactionLog(smsLog);*/

                // Send SMS
                // 메시지 전송 부분만 쓰레드로 처리해야 함. (중요), skkim 2014.12.16
                new Thread(() =>
                {
                    SMSManager.Instance.SendSMS(arrPhoneNumbers, szSendNum, strMessage, true);

                    SaveSMSHistory(arrPhoneNumbers, -1, -1, strMessage, 1, true);

                }).Start();
            }
        }

        private void SaveSMSHistory(ArrayList arMembers,int SensorHistoryID, int nReactionHistoryID, string szMessage, int nSendType, bool bAll)
        {
            DBUtility.WebDBManager dbMgr = NetworkServer.Instance.DBManager;
            
            string strSQL = "select max(id) from SDMSSMSHistory";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null)
            {
                return;
            }
                       
            string szCompanyMemberList = "";
            string szExternalMemberList = "";
            //if(bAll == false)
            {
                StringBuilder sb1 = new StringBuilder();
                StringBuilder sb2 = new StringBuilder();
                foreach (DataMember member in arMembers)
                {
                    if (member.ObjectType == 1)
                    {
                        if (sb1.Length > 0)
                            sb1.Append(',');
                        sb1.Append(member.ID);
                    }
                    else if (member.ObjectType == 2)
                    {
                        if (sb2.Length > 0)
                            sb2.Append(',');
                        sb2.Append(member.ID);
                    }
                }
                szCompanyMemberList = sb1.ToString();
                szExternalMemberList = sb2.ToString();
            }

            int nID = arrResult.Count == 0 ? 1 : DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;

            strSQL = string.Format("Insert into SDMSSMSHistory (ID,SensorHistoryID, ReactionHistoryID, CompanyMemberIDList, ExternalCompanyMemberIDList, SMSMessage, SendType) values ({0}, {1}, {2}, '{3}', '{4}', '{5}', {6})",
                nID, SensorHistoryID, nReactionHistoryID, szCompanyMemberList, szExternalMemberList, szMessage, nSendType);

            if (dbMgr.GetResultData(strSQL, 0) == null)
            {              
                return;
            }        
        }

        public static SMSManager.SMSMessageType GetSMSMessageTypeFromLog(SensorReactionLog log)
        {
            SMSManager.SMSMessageType smsType = SMSManager.SMSMessageType.UNKNOWN;

            if (log.Type == libSensorProcess.ReactionType.MALFUNCTION)
            {

                int nIdx = log.Message.IndexOf("화재");
                if (nIdx != -1)
                    smsType = SMSManager.SMSMessageType.RESET_FIRE;
                else
                    smsType = SMSManager.SMSMessageType.RESET_SECURITY;
            }

            else if (log.Type == libSensorProcess.ReactionType.BEGIN_STATUS)
                smsType = SMSManager.SMSMessageType.DETECT_FIRE;

            // 알람단계변경에 대해서는 문자전송을 하지 않는다. 20160503 skkim (kjw요청)
            else if (log.Type == libSensorProcess.ReactionType.BEGIN_PSM_STATUS)
            {
                smsType = SMSManager.SMSMessageType.DETECT_PSM;
            }
            else if (log.Type == libSensorProcess.ReactionType.PSM_USER_RESET)
            {
                smsType = SMSManager.SMSMessageType.RESET_PSM;
            }
            else if (log.Type == libSensorProcess.ReactionType.IGNORE_FIRE)
            {
                smsType = SMSManager.SMSMessageType.RESET_FIRE;
            }
            else if (log.Type == libSensorProcess.ReactionType.IGNORE_PSM_DETECT)
            {
                smsType = SMSManager.SMSMessageType.RESET_PSM;
            }
            else if (log.Type == libSensorProcess.ReactionType.IGNORE_S1SVMS_STATUS || log.Type == libSensorProcess.ReactionType.IGNORE_S1ACCESS_STATUS ||
                log.Type == libSensorProcess.ReactionType.IGNORE_SECOM_STATUS)
            {
                smsType = SMSManager.SMSMessageType.RESET_SECURITY;
            }
            else if (log.Type == libSensorProcess.ReactionType.BEGIN_S1SVMS_STATUS || log.Type == libSensorProcess.ReactionType.BEGIN_S1ACCESS_STATUS ||
                log.Type == libSensorProcess.ReactionType.BEGIN_SECOM_STATUS)
            {
                smsType = SMSManager.SMSMessageType.DETECT_SECURITY;
            }
            else if (log.Type == libSensorProcess.ReactionType.NOTIFY_SECURITY)
            {
                smsType = SMSManager.SMSMessageType.REPORT_SECURITY;
            }

            return smsType;
        }

		public void SendSMS(SensorReactionLog log, SMSManager.SMSMessageType smsType)
		{
             //public enum MessageType { FACILITY_FAULT = 0, DETECT_FIRE, REPORT_FIRE, DETECT_SPILL, REPORT_SPILL, RESET_SPILL };
            if (log == null)
                return;

            /*int nSmsType = -1;
            if (log.Type == SensorReactionLog.ReactionType.MALFUNCTION)
                nSmsType = 0;
            else if (log.Type == SensorReactionLog.ReactionType.BEGIN_STATUS)
                nSmsType = 1;

            // 알람단계변경에 대해서는 문자전송을 하지 않는다. 20160503 skkim (kjw요청)
            else if (log.Type == SensorReactionLog.ReactionType.BEGIN_PSM_STATUS)
            {
                nSmsType = 3;
            }
            else if (log.Type == SensorReactionLog.ReactionType.PSM_USER_RESET)
            {
                nSmsType = 5;
            }
            else if (log.Type == SensorReactionLog.ReactionType.IGNORE_FIRE)
            {
                nSmsType = 0;
            }
            else if (log.Type == SensorReactionLog.ReactionType.IGNORE_PSM_DETECT)
            {
                nSmsType = 5;
            }*/
            //else if (log.Type == SensorReactionLog.ReactionType.NOTIFY_FIRE)
            //{
            //    nSmsType = 2;
            //}
            //else if (log.Type == SensorReactionLog.ReactionType.NOTIFY_PSM)
            //{
            //    nSmsType = 4;
            //}

            // ToDo : Security상황에 대한 문자 전송 추가
            bool bUseSMS = GetSMSConfig(smsType);    
            //bool bUseSMS = GetSMSConfig(nSmsType);
            if (bUseSMS == false)
                return;

            // 센서와 연결된 담당자 전화번호 가져오기
            ArrayList arrPhoneNumbers = GetOperatorPhoneNumber(log, !SMSManager.IsReportType(smsType));
            if (arrPhoneNumbers == null || arrPhoneNumbers.Count == 0)
                return;

            // 사전 정의된 메시지 가져오기
            string szMsg = MakeSMSMessage(log);
            // 발신자 번호 가져오기
            string szSendNum = GetSendPhoneNumber();
            // 문자 메시지 보내기			
            if (szMsg != "")
            {
                SensorReactionLog smsLog = new SensorReactionLog();
                smsLog.Message = "담당자에게 메시지가 전송되었습니다. 내용 : " + szMsg;
                smsLog.Param1 = log.Param1;
                smsLog.Param2 = log.Param2;
                smsLog.SensorHistoryID = log.SensorHistoryID;
                smsLog.Type = libSensorProcess.ReactionType.SEND_SMS;
                AddReactionLog(smsLog);

                // Send SMS thread
                // 메시지 전송 부분만 쓰레드로 처리해야 함. (중요), skkim 2014.12.16
                new Thread(() =>
                {
                    SMSManager.Instance.SendSMS(arrPhoneNumbers, szSendNum, szMsg, false);

                    SaveSMSHistory(arrPhoneNumbers, smsLog.SensorHistoryID, smsLog.ID, szMsg, 1, false);

                }).Start();
            }
		}

        private bool GetSMSConfig(SMSManager.SMSMessageType type)
		//private bool GetSMSConfig(int nType = 1)
		{
            if (type < SMSManager.SMSMessageType.RESET_FIRE)
                return false;

			DBUtility.WebDBManager dbMgr = NetworkServer.Instance.DBManager;
			string strSQL = string.Format("Select id, MessageType, UseSMS from SDMSSMSConfig Where MessageType={0}", (int)type);
			ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);
			if (arrResult == null)
				return false;

			int nResultCount = arrResult.Count;
			if (nResultCount > 2)
			{
				int nID = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), -1);
				int nMessageType = DBUtility.WebDBManager.GetIntField(arrResult[1].ToString(), -1);
				bool useSMS = DBUtility.WebDBManager.GetIntField(arrResult[2].ToString(), 0) == 0 ? false : true;
				return useSMS;
			}			
			return false;
		}

        /*private bool GetSendDutyConfig()
        {
            bool m_bSendSmsNightDuty = false;
            DBUtility.WebDBManager dbMgr = NetworkServer.Instance.DBManager;
            string szSQL4 = "SELECT PropertyValue FROM OptionSDMS where PropertyName='SendSMSonNightDuty'";
            ArrayList arResult4 = dbMgr.GetResultData(szSQL4, 0);
            if (arResult4 == null || arResult4.Count == 0)
            {
                m_bSendSmsNightDuty = false;
            }
            else
            {
                int nTemp = DBUtility.WebDBManager.GetIntField(arResult4[0].ToString(), -1);

                if (nTemp == 1)
                    m_bSendSmsNightDuty = true;
                else
                    m_bSendSmsNightDuty = false;
            }
            return m_bSendSmsNightDuty;
        }*/

        public void MonitorDetectFireProcess(SensorReactionLog log)
        {
            //Thread t = new Thread(new ParameterizedThreadStart(MonitorDetectFireThread));
            //t.Start(log);
        }

        // 화재 감지후 일정시간동안 진행사항이 있는지 감시
        private void MonitorDetectFireThread(object arg)
        {
            SensorReactionLog log = (SensorReactionLog)arg;

			while (!NetworkServer.Instance.FinishProcess)
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
                if (_history.LastReactionLog.Type != libSensorProcess.ReactionType.BEGIN_STATUS)
                    break;

                Thread.Sleep(2000);

                DateTime dtNow = DateTime.Now;
                TimeSpan span = dtNow - log.LogTime;

                if (span.TotalHours >= DetectFireTimeout)
                {
                    WriteIgnoreDetect(log, dtNow);
                    SendIgnoreDetect(log, TCP_CLIENT.SDMS_CLIENT);
                    break;
                }
            }
        }

        private void SendIgnoreDetect(SensorReactionLog log, byte clientType)
        {
            if (log.SensorHistoryID < 0)
                return;

            byte[] bytes = new byte[11];

            bytes[0] = TCP_ID.IGNORE_DETECT_REPORT;
            bytes[1] = 1;

            byte[] sensorHistoryIDBytes = TcpHelper.MakeBytes(log.SensorHistoryID);
            System.Buffer.BlockCopy(sensorHistoryIDBytes, 0, bytes, 2, sensorHistoryIDBytes.Length);

            ICollection<ConnectionState> arClients = null;
            DdMonitor.Enter(m_arrClients, true);		
            {
                arClients = m_arrClients.Keys;
            }
            DdMonitor.Exit(m_arrClients, true);		

            ArrayList arrDropStates = new ArrayList();
            foreach (ConnectionState state in arClients)
            {
                ClientData client = (ClientData)state.Tag;
                if (client == null || client.ClientType == TCP_CLIENT.UNKNOWN)
                    continue;

                if (clientType == TCP_CLIENT.ALL || clientType == client.ClientType)
                {
                    try
                    {
                        Send(bytes, 0, bytes.Length, state, false, arrDropStates);
                    }
                    catch (System.Exception ex)
                    {
                        ConnectionLogEx.Instance.WriteLine("SendIgnoreDectect", ex);
                    }                   
                }
            }
            ProcessDropList(arrDropStates);
            
        }

        private void WriteIgnoreDetect(SensorReactionLog log, DateTime dtNow)
        {
            string strMsg = string.Format("화재감지후 {0}시간동안 아무런 진행사항이 없어서 시스템이 상황을 종료시킵니다.",
                DetectFireTimeout);

			DBUtility.WebDBManager dbMgr = NetworkServer.Instance.DBManager;

            ArrayList arrResult = dbMgr.GetResultData("Select max(id) from SensorReactionHistory", 0);
            if (arrResult == null)
                return;

            int nID = arrResult.Count == 0 ? 1 : DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;

            string strSQL = string.Format("Insert into SensorReactionHistory (ID, SensorHistoryID, ReactionType, Time, Message, Param1, Param2, DetectionStatus) values ({0}, {1}, {2}, '{3}', '{4}', '', '', {5})",
                nID, log.SensorHistoryID, (int)libSensorProcess.ReactionType.IGNORE_FIRE,
                string.Format("{0} {1:00}:{2:00}:{3:00}", dtNow.ToShortDateString(), dtNow.Hour, dtNow.Minute, dtNow.Second),
                strMsg, (int)log.Status);

            dbMgr.GetResultData(strSQL, 0);
            ServiceProvider.WriteSensorReactionHistoryDescription(log, dbMgr);
        }

        public void MonitorNotifyFireProcess(SensorReactionLog log)
        {
            Thread t = new Thread(new ParameterizedThreadStart(MonitorNotifyStatusThread));
            t.Start(log);
        }

        public void MonitorNotifySecurityProcess(SensorReactionLog log)
        {
            Thread t = new Thread(new ParameterizedThreadStart(MonitorNotifyStatusThread));
            t.Start(log);
        }


        // 화재 신고후 일정시간동안 진행사항이 있는지 감시
        private void MonitorNotifyStatusThread(object arg)
        {
            SensorReactionLog log = (SensorReactionLog)arg;

			while (!NetworkServer.Instance.FinishProcess)
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
                if (_history.LastReactionLog.Type != libSensorProcess.ReactionType.NOTIFY_FIRE ||
                    _history.LastReactionLog.Type != libSensorProcess.ReactionType.NOTIFY_SECURITY
                    )
				   //&& _history.LastReactionLog.Type != SensorReactionLog.ReactionType.TRAINNING_FIRE)
                    break;

                Thread.Sleep(2000);

                TimeSpan span = DateTime.Now - log.LogTime;

                if (span.TotalHours >= NotifyFireTimeout)
                {
                    SensorReactionLog log2 = log.Clone();
                    log2.Type = libSensorProcess.ReactionType.IGNORE_SOP;
                    log2.Message = string.Format("신고후 {0}시간동안 아무런 진행사항이 없어서 시스템이 상황을 종료시킵니다.", (int)NotifyFireTimeout);
                    ProcessIgnoreSOP(log2, log2.SensorHistoryID, null);

                    break;
                }
            }
        }

        public void SendStatusSensorSignal(SensorReactionLog log,ConnectionState state = null)
        {
            int nSensorZoneID = SensorManager.Instance.GetSensorID(log.SensorHistoryID);
            if (nSensorZoneID < 0)
                return;
						
			SensorZone sensor = null;
			string strOriginSensorTableName = "";
			float x = 0.0f;
			float y = 0.0f;
			float z = 0.0f;
			int nSensorID = 0;
			int nEquipZoneID = 0;
			if (nSensorZoneID != 0)
			{
				sensor = NetworkServer.Instance.IOManager.GetSensorZone(nSensorZoneID);
				if (sensor == null)
					return;

                strOriginSensorTableName = FacilityManager.GetFacilityTypeTable(sensor.Type);

                if (strOriginSensorTableName.Length == 0)
                    return;

                /*if (sensor.Type == 1 || sensor.Type == 6)
                    strOriginSensorTableName = "FireSensor";
                else if (sensor.Type == 9)
                    strOriginSensorTableName = "AnalogSmokeTypeSensor";
                else if (sensor.Type == 2)
                    strOriginSensorTableName = "SpringCooler";
                else if (sensor.Type == 3)
                    strOriginSensorTableName = "PumpPressureSensor";
                else
                    return;*/


                if( strOriginSensorTableName == "PSMSensor" )
                {
                    string strSQL = string.Format("select sz.OrgSensorID, os.X, os.Y,os.Y from SensorZoneHistory as szh, SensorZone as sz, {0} as os where szh.ID = {1} and szh.SensorID = sz.ID and sz.OrgSensorID = os.ID",
                                       strOriginSensorTableName, log.SensorHistoryID);
                    ArrayList arrResult = NetworkServer.Instance.DBManager.GetResultData(strSQL, 0);

                    if (arrResult == null || arrResult.Count < 4)
                        return;

                    nSensorID = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), -1);
                    x = DBUtility.WebDBManager.GetFloatField(arrResult[1].ToString(), 0.0f);
                    y = DBUtility.WebDBManager.GetFloatField(arrResult[2].ToString(), 0.0f);
                    z = 0.5f;

                    if (nSensorID < 0)
                        return;

                    nEquipZoneID = sensor.EquipZone == null ? -1 : sensor.EquipZone.ID;

                    return; // 일단 PSM인경우 리턴한다. 별도 전파 처리가 필요한경우 추가로 구성할것. 2016-05-20 . comment by skkim
                    
                }
                else 
                {
                    string strSQL = string.Format("select sz.OrgSensorID, os.X, os.Y, os.Z from SensorZoneHistory as szh, SensorZone as sz, {0} as os where szh.ID = {1} and szh.SensorID = sz.ID and sz.OrgSensorID = os.ID",
                                        strOriginSensorTableName, log.SensorHistoryID);
                    ArrayList arrResult = NetworkServer.Instance.DBManager.GetResultData(strSQL, 0);

                    if (arrResult == null || arrResult.Count < 4)
                        return;

                    nSensorID = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), -1);
                    x = DBUtility.WebDBManager.GetFloatField(arrResult[1].ToString(), 0.0f);
                    y = DBUtility.WebDBManager.GetFloatField(arrResult[2].ToString(), 0.0f);
                    z = DBUtility.WebDBManager.GetFloatField(arrResult[3].ToString(), 0.0f);
                    if (nSensorID < 0)
                        return;

                    nEquipZoneID = sensor.EquipZone == null ? -1 : sensor.EquipZone.ID;

                }    
			}
			else
			{
                // 수동신고의 경우
				strOriginSensorTableName = "FireSensor";
				int.TryParse(log.Param1, out nEquipZoneID);
			}

            bool bReal = false;
            if (DataManager.GetTranningMode())
            {
                bReal = false;
            }
            else
            {
                bReal = true;
            }

            ArrayList arDatas = new ArrayList();

            arDatas.Add(nSensorID);
            arDatas.Add(log.SensorHistoryID);
            arDatas.Add(nEquipZoneID);
            arDatas.Add(log.LogTime.ToBinary());
            arDatas.Add(x);
            arDatas.Add(y);
            arDatas.Add(z);
            arDatas.Add((bReal == true ? 0 : 1));

            byte[] bytes = TcpHelper.MakeBytes(TCP_ID.FIRE_SENSOR_SIGNAL, arDatas);

            //byte[] sensorIDBytes = MakeBytes(nSensorID);
            //byte[] sensorHistoryIDBytes = MakeBytes(log.SensorHistoryID);
            //byte[] zoneIDBytes = MakeBytes(nEquipZoneID);
            //byte[] timeBytes = MakeBytes(log.LogTime.ToBinary());
            //byte[] xBytes = MakeBytes(x);
            //byte[] yBytes = MakeBytes(y);
            //byte[] zBytes = MakeBytes(z);
			
            //byte[] real = MakeBytes((bReal == true ? 0 : 1));

            //int nBlockLength = sensorIDBytes.Length + sensorHistoryIDBytes.Length + zoneIDBytes.Length + timeBytes.Length + xBytes.Length + yBytes.Length + zBytes.Length + real.Length;
            //byte[] bytes = new byte[6 + nBlockLength];

            //bytes[0] = TCP_ID.FIRE_SENSOR_SIGNAL;
            //bytes[1] = 0;

            //int nChunkCount = 8;
            //byte[] chunkBytes = BitConverter.GetBytes(nChunkCount);
            //System.Buffer.BlockCopy(chunkBytes, 0, bytes, 2, 4);

            //int nIndex = 6;
            //SensorReactionLog.CopyBytes(bytes, ref nIndex, sensorIDBytes);
            //SensorReactionLog.CopyBytes(bytes, ref nIndex, sensorHistoryIDBytes);
            //SensorReactionLog.CopyBytes(bytes, ref nIndex, zoneIDBytes);
            //SensorReactionLog.CopyBytes(bytes, ref nIndex, timeBytes);
            //SensorReactionLog.CopyBytes(bytes, ref nIndex, xBytes);
            //SensorReactionLog.CopyBytes(bytes, ref nIndex, yBytes);
            //SensorReactionLog.CopyBytes(bytes, ref nIndex, zBytes);
            //SensorReactionLog.CopyBytes(bytes, ref nIndex, real);

            if (state == null)
                SendData(bytes, true, TCP_CLIENT.SOP_SIMULATOR);
            else
            {
                try
                {
                    Send(bytes, 0, bytes.Length, state);
                }
                catch (System.Exception ex)
                {
                    ConnectionLogEx.Instance.WriteLine("SendFireSensorSignal", ex);
                }
            }
                
        }

		public void AddTempIgnoreSensor(SensorZone sensor)
		{
			if (!m_arrTempIgnoreSensors.Contains(sensor))
				m_arrTempIgnoreSensors.Add(sensor);
		}

		private void RemoveTempIgnoreSensor(SensorZone sensor)
		{
			m_arrTempIgnoreSensors.Remove(sensor);
		}

        public void RemoveTempIgnoreSensor(int nSensorID)
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

        public void SendClientData(byte[] bytes, byte clientType, bool nolock)
        {
            ICollection<ConnectionState> arClient = null;
            DdMonitor.Enter(m_arrClients, true);
            {
                arClient = m_arrClients.Keys;
            }
            DdMonitor.Exit(m_arrClients, true);		         

            foreach (ConnectionState state in arClient)
            {

                ClientData client = (ClientData)state.Tag;
                if (client == null || client.ClientType == TCP_CLIENT.UNKNOWN)
                    continue;

                if (clientType == TCP_CLIENT.ALL || clientType == client.ClientType)
                {
                    try
                    {
                        Send(bytes, 0, bytes.Length, state, nolock, null);
                    }
                    catch (System.Exception ex)
                    {
                        ConnectionLogEx.Instance.WriteLine("SendClientData", ex);
                    }                   
                }
            }
           
        }
       
        public void SendReciverState(int nReciver, bool bConnected, byte clientType, bool noLock = false, bool noOnDropConnection = false)
        {


            ArrayList arDatas = new ArrayList();
            arDatas.Add(nReciver);
            arDatas.Add((bConnected == true ? 1 : 0));
            short nHeader = bConnected == true ? TCP_ID.RECIVER_CONNECT : TCP_ID.RECIVER_DISCONNECT;

            byte[] bytes = TcpHelper.MakeBytes(nHeader, arDatas);

			//byte[] bytes = new byte[24];

            //byte[] nReciverIDBytes = MakeBytes(nReciver);
            //byte[] nConnectedBytes = MakeBytes(bConnected == true ? 1 : 0);

			
            //byte[] byteHeader = BitConverter.GetBytes(nHeader);
            //bytes[0] = byteHeader[0];
            //bytes[1] = byteHeader[1];

            //// SET DATA COUNT
            //byte[] nCount = BitConverter.GetBytes(2);
            //bytes[2] = nCount[0];
            //bytes[3] = nCount[1];
            //bytes[4] = nCount[2];
            //bytes[5] = nCount[3];

            //int nIndex = 6;

            //SensorReactionLog.CopyBytes(bytes, ref nIndex, nReciverIDBytes);
            //SensorReactionLog.CopyBytes(bytes, ref nIndex, nConnectedBytes);


            if (noLock)
                SendReceiverState(bytes, clientType, noLock, noOnDropConnection);
            else
            {
                ICollection<ConnectionState> arClients = null;
                DdMonitor.Enter(m_arrClients, true);
                {
                    arClients = m_arrClients.Keys;
                }
                DdMonitor.Exit(m_arrClients, true);		
                    
                ArrayList arrDropStates = new ArrayList();
                foreach (ConnectionState state in arClients)
                {
                    ClientData client = (ClientData)state.Tag;
                    if (client == null || client.ClientType == TCP_CLIENT.UNKNOWN)
                        continue;

                    if (clientType == TCP_CLIENT.ALL || clientType == client.ClientType)
                    {
                        try
                        {
                            Send(bytes, 0, bytes.Length, state, false, arrDropStates);
                        }
                        catch (System.Exception ex)
                        {
                            ConnectionLogEx.Instance.WriteLine("SendReciverState", ex);
                        }                        
                    }
                }

                ProcessDropList(arrDropStates);                
            }
        }
        
        private void SendReceiverState(byte[] bytes, byte clientType, bool noLock, bool noOnDropConnection)
        {
            ArrayList arrDropStates = noOnDropConnection ? null : new ArrayList();

            ICollection<ConnectionState> arClinets = null;
            DdMonitor.Enter(m_arrClients, true);
            {
                arClinets = m_arrClients.Keys;
            }
            DdMonitor.Exit(m_arrClients, true);		

            foreach (ConnectionState state in arClinets)
            {
                ClientData client = (ClientData)state.Tag;
                if (client == null || client.ClientType == TCP_CLIENT.UNKNOWN)
                    continue;

                if (clientType == TCP_CLIENT.ALL || clientType == client.ClientType)
                {
                    try
                    {
                        Send(bytes, 0, bytes.Length, state, noLock, arrDropStates);
                    }
                    catch (System.Exception ex)
                    {
                        ConnectionLogEx.Instance.WriteLine("SendReciverState", ex);
                    }                   
                }
            }

            if (!noOnDropConnection)
                ProcessDropList(arrDropStates);

        }

        private void ProcessDropList(ArrayList arrDropStates)
        {
            if (arrDropStates == null)
                return;

            foreach (ConnectionState state in arrDropStates)
            {
                //OnDropConnection(state);
                _OnDropConnection(state, true);
            }
        }
        //광교 psm(암모니아)전용. - 바람 데이터를 보내기 위하여 작성.
        public void SendSensorZoneDataWithWind(int nData, int nSensorID,byte clientType, int wDir, int wSpeed, bool noLock = false)
        {
            SensorZone sensor = NetworkServer.Instance.IOManager.GetSensorZone(nSensorID);
            if (sensor == null || sensor.EquipZone == null)
                return;

            ArrayList arDatas = new ArrayList();
            arDatas.Add(nSensorID);
            arDatas.Add((int)sensor.Type);
            arDatas.Add(sensor.IsConnected ? 1 : 0);
            arDatas.Add(sensor.EquipZone.ID);
            arDatas.Add(nData);
            arDatas.Add(sensor.LinkedSensorID);
            arDatas.Add(wDir);
            arDatas.Add(wSpeed);
            byte[] bytes = TcpHelper.MakeBytes(TCP_ID.SENSOR_ZONE_DATA, arDatas);

            SendClientData(bytes, clientType, true);

        }

        public void SendSensorZoneData(int nData, int nSensorID, byte clientType, bool noLock = false)
        {
            SensorZone sensor = NetworkServer.Instance.IOManager.GetSensorZone(nSensorID);
            if (sensor == null || sensor.EquipZone == null)
                return;

            ArrayList arDatas = new ArrayList();
            arDatas.Add(nSensorID);
            arDatas.Add((int)sensor.Type);
            arDatas.Add(sensor.IsConnected ? 1 : 0);
            arDatas.Add(sensor.EquipZone.ID);
            arDatas.Add(nData);
            arDatas.Add(sensor.LinkedSensorID);

            byte[] bytes = TcpHelper.MakeBytes(TCP_ID.SENSOR_ZONE_DATA, arDatas);

            //byte[] sensorZoneIDBytes = MakeBytes(nSensorID);
            //byte[] sensorTypeBytes = MakeBytes((int)sensor.Type);
            //byte[] connectedBytes = MakeBytes(sensor.IsConnected ? 1 : 0);
            //byte[] zoneIDBytes = MakeBytes(sensor.EquipZone.ID);
            //byte[] dataBytes = MakeBytes(nData);
            //byte[] linkedSensorIDBytes = MakeBytes(sensor.LinkedSensorID);

            //byte[] bytes = new byte[6 + 9 * 6];

            //bytes[0] = TCP_ID.SENSOR_ZONE_DATA;
            //bytes[1] = 0;

            //int nChunkCount = 6;
            //byte[] chunkBytes = BitConverter.GetBytes(nChunkCount);
            //System.Buffer.BlockCopy(chunkBytes, 0, bytes, 2, 4);

            //int nIndex = 6;
            //SensorReactionLog.CopyBytes(bytes, ref nIndex, sensorZoneIDBytes);
            //SensorReactionLog.CopyBytes(bytes, ref nIndex, sensorTypeBytes);
            //SensorReactionLog.CopyBytes(bytes, ref nIndex, connectedBytes);
            //SensorReactionLog.CopyBytes(bytes, ref nIndex, zoneIDBytes);
            //SensorReactionLog.CopyBytes(bytes, ref nIndex, dataBytes);
            //SensorReactionLog.CopyBytes(bytes, ref nIndex, linkedSensorIDBytes);

            SendClientData(bytes, clientType, true);

        }
       
        private void SendSensorZoneData(byte[] bytes, byte clientType, bool noLock)
        {
            SendClientData(bytes, clientType, true);
        }

        private void AddClearStatusLog(int nSensorHistoryID)
        {
            DBUtility.WebDBManager dbMgr = NetworkServer.Instance.DBManager;

            // MaxID 가져오는 Query 변경 - skkim 2017.03.16
            string strSQL = "SELECT (IFNULL(MAX(ID), 0) + 1) as ID FROM SensorReactionHistory";
            //string strSQL = "select max(id) from SensorReactionHistory";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            SensorReactionLog.DetectionStatus eStatus = SensorReactionLog.DetectionStatus.TEST;
            TimeHistory history = FindTimeHistory(nSensorHistoryID);
            if( history != null)
            {
                eStatus = history.DetectStatus;
            }                
            
            SensorZone sensorZone = NetworkServer.Instance.IOManager.GetSensorZone(SensorManager.Instance.GetSensorID(nSensorHistoryID));

            if (sensorZone != null)
            {
                if (sensorZone.Type == IFacility.FacilityType.PSM_SENSOR)
                {
                    eStatus = SensorReactionLog.DetectionStatus.REAL;
                }
            }

            int nID = arrResult.Count == 0 ? 1 : DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;
            DateTime dtNow = DateTime.Now;

            strSQL = string.Format("Insert into SensorReactionHistory (id, SensorHistoryID, ReactionType, Time, Message, Param1, Param2, DetectionStatus)" +
                " values ({0}, {1}, {2}, '{3}', '상황해제', '', '', {4})",
                nID, nSensorHistoryID, (int)libSensorProcess.ReactionType.END_STATUS,
                string.Format("{0} {1:00}:{2:00}:{3:00}", dtNow.ToShortDateString(), dtNow.Hour, dtNow.Minute, dtNow.Second),
                (int)eStatus);

            dbMgr.GetResultData(strSQL, 0);

            // 상황종료시 현재상황으로 인한 방송이 실행중이면 중지시킨다.
            OffBroadcast(nSensorHistoryID, dbMgr);
        }

        // 상황종료시 현재상황으로 인한 방송이 실행중이면 중지시킨다.
        private void OffBroadcast(int nSensorZoneHistoryID, DBUtility.WebDBManager dbMgr)
        {
            string strSQL = "Select ID from SensorReactionHistory where SensorHistoryID = " + nSensorZoneHistoryID.ToString();
            strSQL += " and ReactionType = " + (int)libSensorProcess.ReactionType.RUN_BROADCAST;

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult != null && arrResult.Count > 0)
            {
                BroadcastManager.Instance.StopSpeech();
            }
        }

        public void SendClearDetectReport(int nSensorHistoryID, byte clientType, bool writeClearLog = true, bool noLock = false)
        {
            if (writeClearLog)
                AddClearStatusLog(nSensorHistoryID);

            
            foreach (TimeHistory th in m_arTimeHistory)
            {
                if (th.HistoryID == nSensorHistoryID)
                {
                    m_arTimeHistory.Remove(th);
                    SensorManager.Instance.RemoveSensorHistory(nSensorHistoryID);
                    break;
                }
            }

            ArrayList arDatas = new ArrayList();
            arDatas.Add(nSensorHistoryID);


            byte[] bytes = TcpHelper.MakeBytes(TCP_ID.CLEAR_DETECT_REPORT, arDatas);
            
            //byte[] historyBytes = TcpHelper.MakeBytes(nSensorHistoryID);

            //byte[] bytes = new byte[6 + historyBytes.Length];

            //bytes[0] = TCP_ID.CLEAR_DETECT_REPORT;
            //bytes[1] = 0;

            //int nChunkCount = 1;
            //byte[] chunkBytes = BitConverter.GetBytes(nChunkCount);
            //System.Buffer.BlockCopy(chunkBytes, 0, bytes, 2, 4);
            //System.Buffer.BlockCopy(historyBytes, 0, bytes, 6, historyBytes.Length);


            SendClientData(bytes, clientType, true);
        }

        private void SendClearDetectReport(byte[] bytes, byte clientType, bool noLock)
        {
            SendClientData(bytes, clientType, true);
        }

		public void RemoveSituation(int nHistoryID, bool writeClearLog = true)
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

            SendClearDetectReport(nHistoryID, TCP_CLIENT.SDMS_CLIENT, writeClearLog);
            SendClearDetectReport(nHistoryID, TCP_CLIENT.SOP_SIMULATOR, writeClearLog);            
        }

        // nHistoryID에 해당하는 로그가 이미 존재하는지 여부를 알려준다.
        // Return값 : true이면 해당 로그가 이미 존재한다.
        //            false이면 로그가 존재하지 않는다.
        public bool CheckSituation(int nHistoryID)
        {
            TimeHistory history;
            return CheckSituation(nHistoryID, out history);
        }

        // nHistoryID에 해당하는 로그가 이미 존재하는지 여부를 알려준다.
        // Return값 : true이면 해당 로그가 이미 존재한다.
        //            false이면 로그가 존재하지 않는다.
        public bool CheckSituation(int nHistoryID, out TimeHistory history)
        {
            foreach (TimeHistory th in m_arTimeHistory)
            {
                if (th.HistoryID == nHistoryID)
                {
                    history = th;
                    return true;
                }
            }

            history = null;
            return false;
        }

        public bool CheckSituationForSensorID(int nSensorID)
        {
            foreach (TimeHistory th in m_arTimeHistory)
            {
                int nSensorID2 = SensorManager.Instance.GetSensorID(th.HistoryID);
                if (nSensorID2 == nSensorID)
                {
                    // nSensorID2와 nSensorID가 같은 Sensor인 경우
                    //return th;
                    return true;
                }
                else
                {
                    // nSensorID2와 nSensorID가 다른 Sensor이지만
                    // 같은 SensorZoneGroup에 속해있는지 검사한다.
                    SensorZoneGroup group = NetworkServer.Instance.IOManager.GetSensorZoneGroup(nSensorID2);

                    if (group != null)
                    {
                        foreach (KeyValuePair<SensorZone, object> pair in group.SensorDatas)
                        {
                            if (pair.Key.ID == nSensorID)
                                //return th;
                                return true;
                        }
                    }
                }
            }

            //return null;
            return false;
        }
        
        public void ProcessIgnoreSOP(SensorReactionLog log, int nSensorHistoryID, byte[] bytes)
        {
            if (log != null)
            {
				bool bAddedLastLog = false;
                foreach (TimeHistory history in m_arTimeHistory)
                {
                    if (history.HistoryID == nSensorHistoryID)
                    {
                        history.LastReactionLog = log;
						bAddedLastLog = true;
                        break;
                    }
                }

				NetworkServer.Instance.SensorManager.SetLastReadSensorHistoryID(nSensorHistoryID);

                //SendData(bytes, false, TCP_CLIENT.SDMS_CLIENT);
                SendData(bytes, false, TCP_CLIENT.SOP_SIMULATOR);

				if (bAddedLastLog == true)
					SendData(log.MakeBytes(), false, TCP_CLIENT.SDMS_CLIENT);
            }

            int nSensorID = SensorManager.Instance.GetSensorID(nSensorHistoryID);
			SensorZone sensor = NetworkServer.Instance.IOManager.GetSensorZone(nSensorID);
			
			// comment by skkim 2013-12-10
			// 상황종료는 ProcessSensorData에서 신호가 0인경우로만 한정
			// SOP가 무시되어도 상황이 유지 되어야 함. 아니면 동일한 신호에 대해 계속 처리하게 됨
			//RemoveSituation(nSensorHistoryID);
		}


		public void CheckTranningMode(SensorReactionLog log)
		{
			if (DataManager.GetTranningMode())
			{
                // 171114 KYJ
                log.Message = GetTranningMessage() + log.Message;
                //
                //log.Message = m_strTranning + log.Message;
			}
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

			DBUtility.WebDBManager dbMgr = NetworkServer.Instance.DBManager;

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
                isRealMode ? strSubCategoryName : GetTranningMessage() + strSubCategoryName,
                strDisasterName, strStepName);

            arrResult = dbMgr.GetResultData("Select max(id) from SensorReactionHistory", 0);
            if (arrResult == null)
                return null;

            SensorReactionLog.DetectionStatus eStatus = SensorReactionLog.DetectionStatus.TEST;
            SensorZone sensorZone = NetworkServer.Instance.IOManager.GetSensorZone(SensorManager.Instance.GetSensorID(nSensorHistoryID));

            if (sensorZone != null)
            {
                if (sensorZone.Type == IFacility.FacilityType.PSM_SENSOR)
                {
                    eStatus = SensorReactionLog.DetectionStatus.REAL;
                }
            }

            int nID = arrResult.Count == 0 ? 1 : DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;
			dtBegin = DateTime.Now;

            strSQL = string.Format("Insert into SensorReactionHistory (ID, SensorHistoryID, ReactionType, Time, Message, Param1, Param2, DetectionStatus) values ({0}, {1}, {2}, '{3}', '{4}', '{5}', '', {6})",
                nID, nSensorHistoryID, (int)libSensorProcess.ReactionType.RUN_SOP,
                string.Format("{0} {1:00}:{2:00}:{3:00}", dtBegin.ToShortDateString(), dtBegin.Hour, dtBegin.Minute, dtBegin.Second),
                strMsg, nActionStepHistoryID.ToString(), (int)eStatus);

            SensorReactionLog log = new SensorReactionLog();

            log.ID = nID;
            log.SensorHistoryID = nSensorHistoryID;
            log.Type = libSensorProcess.ReactionType.RUN_SOP;
            log.LogTime = dtBegin;
            log.Message = strMsg;
            log.Param1 = nActionStepHistoryID.ToString();
            log.Status = eStatus;

			CheckTranningMode(log);

            if (dbMgr.GetResultData(strSQL, 0) != null)
            {
                ServiceProvider.WriteSensorReactionHistoryDescription(log, dbMgr);
                return log;
            }

            return null;
        }

        private SensorReactionLog WriteRunNCancelSOPLog(int nSensorHistoryID, int nActionStepHistoryID, string strFormatMessage = null, bool selectCancelTime = true)
        {
            string strSQL = "select ActionStepHistory.CancelTime, RealMode, SubCategoryName, DisasterName, StepName from ActionStepHistory, ActionStep, Disaster, SubDisasterCategory";
            strSQL += " where ActionStepHistory.ID = " + nActionStepHistoryID.ToString() + " and ActionStepHistory.ActionStepID = ActionStep.ID and ";
            strSQL += "ActionStep.DisasterID = Disaster.ID and Disaster.SubDisasterID = SubDisasterCategory.ID";

			DBUtility.WebDBManager dbMgr = NetworkServer.Instance.DBManager;

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
                isRealMode ? strSubCategoryName : GetTranningMessage() + strSubCategoryName,
                strDisasterName, strStepName);

            arrResult = dbMgr.GetResultData("Select max(id) from SensorReactionHistory", 0);
            if (arrResult == null)
                return null;

            int nID = arrResult.Count == 0 ? 1 : DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;

            SensorReactionLog.DetectionStatus eStatus = SensorReactionLog.DetectionStatus.TEST;
            SensorZone sensorZone = NetworkServer.Instance.IOManager.GetSensorZone(SensorManager.Instance.GetSensorID(nSensorHistoryID));

            if (sensorZone != null)
            {
                if (sensorZone.Type == IFacility.FacilityType.PSM_SENSOR)
                {
                    eStatus = SensorReactionLog.DetectionStatus.REAL;
                }
            }

			// Reaction로그의 시간은 현재 시간으로 변경
			dtCancel = DateTime.Now;

            strSQL = string.Format("Insert into SensorReactionHistory (ID, SensorHistoryID, ReactionType, Time, Message, Param1, Param2, DetectionStatus) values ({0}, {1}, {2}, '{3}', '{4}', '{5}', '', {6})",
                nID, nSensorHistoryID, (int)libSensorProcess.ReactionType.RUN_N_CANCEL_SOP,
                string.Format("{0} {1:00}:{2:00}:{3:00}", dtCancel.ToShortDateString(), dtCancel.Hour, dtCancel.Minute, dtCancel.Second),
                strMsg, nActionStepHistoryID.ToString(), (int)eStatus);

            SensorReactionLog log = new SensorReactionLog();

            log.ID = nID;
            log.SensorHistoryID = nSensorHistoryID;
            log.Type = libSensorProcess.ReactionType.RUN_N_CANCEL_SOP;
            log.LogTime = dtCancel;
            log.Message = strMsg;
            log.Param1 = nActionStepHistoryID.ToString();
            log.Status = eStatus;

			CheckTranningMode(log);

            if (dbMgr.GetResultData(strSQL, 0) != null)
            {
                ServiceProvider.WriteSensorReactionHistoryDescription(log, dbMgr);
                return log;
            }

            return null;
        }

        private SensorReactionLog WriteFinishSOPLog(int nSensorHistoryID, int nActionStepHistoryID)
        {
            string strSQL = "select ActionStepHistory.EndTime, RealMode, SubCategoryName, DisasterName, StepName from ActionStepHistory, ActionStep, Disaster, SubDisasterCategory";
            strSQL += " where ActionStepHistory.ID = " + nActionStepHistoryID.ToString() + " and ActionStepHistory.ActionStepID = ActionStep.ID and ";
            strSQL += "ActionStep.DisasterID = Disaster.ID and Disaster.SubDisasterID = SubDisasterCategory.ID";

			DBUtility.WebDBManager dbMgr = NetworkServer.Instance.DBManager;

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
                isRealMode ? strSubCategoryName : GetTranningMessage() + strSubCategoryName,
                strDisasterName, strStepName);

            arrResult = dbMgr.GetResultData("Select max(id) from SensorReactionHistory", 0);
            if (arrResult == null)
                return null;

            int nID = arrResult.Count == 0 ? 1 : DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;

            SensorReactionLog.DetectionStatus eStatus = SensorReactionLog.DetectionStatus.TEST;
            SensorZone sensorZone = NetworkServer.Instance.IOManager.GetSensorZone(SensorManager.Instance.GetSensorID(nSensorHistoryID));

            if (sensorZone != null)
            {
                if (sensorZone.Type == IFacility.FacilityType.PSM_SENSOR)
                {
                    eStatus = SensorReactionLog.DetectionStatus.REAL;
                }
            }

			// Reaction로그의 시간은 현재 시간으로 변경
			dtEnd = DateTime.Now;

            strSQL = string.Format("Insert into SensorReactionHistory (ID, SensorHistoryID, ReactionType, Time, Message, Param1, Param2, DetectionStatus) values ({0}, {1}, {2}, '{3}', '{4}', '{5}', '', {6})",
                nID, nSensorHistoryID, (int)libSensorProcess.ReactionType.FINISH_SOP,
                string.Format("{0} {1:00}:{2:00}:{3:00}", dtEnd.ToShortDateString(), dtEnd.Hour, dtEnd.Minute, dtEnd.Second),
                strMsg, nActionStepHistoryID.ToString(), (int)eStatus);

            SensorReactionLog log = new SensorReactionLog();

            log.ID = nID;
            log.SensorHistoryID = nSensorHistoryID;
            log.Type = libSensorProcess.ReactionType.FINISH_SOP;
            log.LogTime = dtEnd;
            log.Message = strMsg;
            log.Param1 = nActionStepHistoryID.ToString();
            log.Status = eStatus;

			CheckTranningMode(log);
            if (dbMgr.GetResultData(strSQL, 0) != null)
            {
                WriteSensorReactionHistoryDescription(log, dbMgr);
                return log;
            }

            return null;
        }

        public static bool WriteSensorReactionHistoryDescription(SensorReactionLog log, DBUtility.WebDBManager dbMgr)
        {
            if (log.DescriptionText.Length == 0)
                return true;

            string strSQL = "Select ID, RefCount from SensorReactionHistoryDescriptionText where Description = '" + log.DescriptionText + "'";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nTextID = -1;
            int nResultCount = arrResult.Count;

            if (nResultCount < 2)
            {
                int nID = GetMaxTableID("SensorReactionHistoryDescriptionText", dbMgr) + 1;

                strSQL = string.Format("Insert into SensorReactionHistoryDescriptionText (ID, RefCount, Description) values ({0}, {1}, '{2}')",
                    nID, 1, log.DescriptionText);

                if (dbMgr.GetResultData(strSQL, 0) == null)
                    return false;

                nTextID = nID;
            }
            else
            {
                nTextID = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), -1);
                int nRefCount = DBUtility.WebDBManager.GetIntField(arrResult[1].ToString(), -1);

                if (nTextID < 0)
                    return false;

                strSQL = string.Format("Update SensorReactionHistoryDescriptionText set RefCount = {0} where ID = {1}", nRefCount + 1, nTextID);

                if (dbMgr.GetResultData(strSQL, 0) == null)
                    return false;
            }

            int nDescriptionID = GetMaxTableID("SensorReactionHistoryDescription", dbMgr) + 1;

            strSQL = string.Format("Insert into SensorReactionHistoryDescription (ID, SensorReactionHistoryID, DescriptionID) values ({0}, {1}, {2})",
                nDescriptionID, log.ID, nTextID);

            return dbMgr.GetResultData(strSQL, 0) != null;
        }

        public static int GetMaxTableID(string strTableName, DBUtility.WebDBManager dbMgr)
        {
            string strSQL = "Select max(ID) from " + strTableName;
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return 0;

            DBUtility.VariousData<int> maxID = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString());

            if (maxID == null)
                return 0;

            return maxID.Data;
        }

		private void CheckManualReport(int nHistoryID)
		{
			int nSensorID = SensorManager.Instance.GetSensorID(nHistoryID);
			if (nSensorID == 0) // 수동 신고
			{
				Thread t = new Thread(ClearThread);
				t.Start(nHistoryID);
			}
		}

		private void ClearThread(object param1)
		{
			int nHistoryID = (int)param1;
			for( int i = 0 ; i < 60 ; i++)
			{
				if (NetworkServer.Instance.ClosingServer == true)
					break;
				Thread.Sleep(1000);
			}

			if (NetworkServer.Instance.ClosingServer == false)
			{
				SendClearDetectReport(nHistoryID, TCP_CLIENT.SDMS_CLIENT);
			}				
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


			bool bAddedLog = false;
            foreach (TimeHistory history in m_arTimeHistory)
            {
                if (history.HistoryID == log.SensorHistoryID)
                {
                    history.LastReactionLog = log;
					bAddedLog = true;
                    break;
                }
            }

			// Edit by skkim 2013-12-10
			// 현재 진행중인 화재인지 검사, 종료된 화재이면 전송하지 않는다.
            if (bAddedLog == true)
            {
                byte[] bytes = log.MakeBytes();
                SendData(bytes, false, TCP_CLIENT.SDMS_CLIENT);
                SendData(bytes, false, TCP_CLIENT.SOP_SIMULATOR);
            }

			DBUtility.WebDBManager dbMgr = NetworkServer.Instance.DBManager;
            string strSQL = "select EndTime, CancelTime from ActionStepHistory where ID = " + log.Param1;
						
            DateTime dtBegin = log.LogTime;

			while (!NetworkServer.Instance.FinishProcess)
            {
                ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);
                if (arrResult == null)
                    break;

                if (arrResult.Count != 2)
                    break;

                string strEndTime = arrResult[0].ToString();
                string strCancelTime = arrResult[1].ToString();

                if (strEndTime.Length > 0 && string.Compare(strEndTime, "null", true) != 0)
                {
                    SensorReactionLog reactionLog = WriteFinishSOPLog(nSensorHistoryID, nActionStepHistoryID);
					
                    if (reactionLog != null)
                    {
						NetworkServer.Instance.SensorManager.SetLastReadSensorHistoryID(nSensorHistoryID);

						// Edit by skkim 2013-12-10
						// 현재 진행중인 화재인지 검사, 종료된 화재이면 전송하지 않는다.
                        if (CheckSituation(nSensorHistoryID))
                        {
                            //SendData(reactionLog.MakeBytes());
                            byte[] bytes = reactionLog.MakeBytes();                            
                            SendData(bytes, false, TCP_CLIENT.SDMS_CLIENT);
                            SendData(bytes, false, TCP_CLIENT.SOP_SIMULATOR);
                        }

						CloseSituation(nSensorHistoryID, reactionLog);
						CheckManualReport(nSensorHistoryID);
                    }
                    break;
                }

                if (strCancelTime.Length > 0 && string.Compare(strCancelTime, "null", true) != 0)
                {
                    SensorReactionLog reactionLog = WriteRunNCancelSOPLog(nSensorHistoryID, nActionStepHistoryID);

                    if (reactionLog != null)
                    {
						NetworkServer.Instance.SensorManager.SetLastReadSensorHistoryID(nSensorHistoryID);

						// Edit by skkim 2013-12-10
						// 현재 진행중인 화재인지 검사, 종료된 화재이면 전송하지 않는다.
                        if (CheckSituation(nSensorHistoryID))
                        {
                            //SendData(reactionLog.MakeBytes());
                            byte[] bytes = reactionLog.MakeBytes();
                            SendData(bytes, false, TCP_CLIENT.SDMS_CLIENT);
                            SendData(bytes, false, TCP_CLIENT.SOP_SIMULATOR);
                        }

						CloseSituation(nSensorHistoryID, reactionLog);

						CheckManualReport(nSensorHistoryID);
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
							NetworkServer.Instance.SensorManager.SetLastReadSensorHistoryID(nSensorHistoryID);

							// Edit by skkim 2013-12-10
							// 현재 진행중인 화재인지 검사, 종료된 화재이면 전송하지 않는다.
                            if (CheckSituation(nSensorHistoryID))
                            {
                                //SendData(reactionLog.MakeBytes());
                                byte[] bytes = reactionLog.MakeBytes();
                                SendData(bytes, false, TCP_CLIENT.SDMS_CLIENT);
                                SendData(bytes, false, TCP_CLIENT.SOP_SIMULATOR);
                            }
							CloseSituation(nSensorHistoryID, reactionLog);

							CheckManualReport(nSensorHistoryID);
                        }

                        break;
                    }
                }
            }
        }

        private bool CloseSituation(int nSensorHistoryID, SensorReactionLog log)
        {
            int nSensorID = SensorManager.Instance.GetSensorID(nSensorHistoryID);
			SensorZone sensor = NetworkServer.Instance.IOManager.GetSensorZone(nSensorID);

			bool bFind = false;
			if (nSensorHistoryID > 0)
			{
				foreach (TimeHistory history in m_arTimeHistory)
				{
					if (history.LastReactionLog != null && history.LastReactionLog.SensorHistoryID == log.SensorHistoryID)
					{
						//System.Diagnostics.Trace.WriteLine(string.Format("LastReactionLog status is changed({0})", log.Type));
						history.LastReactionLog = log;
						bFind = true;
						break;
					}
				}
			}
			SensorManager.Instance.SetLastReadSensorHistoryID(log.SensorHistoryID - 1);
			return bFind;
        }

		public override void OnDropConnection(ConnectionState state)
		{
            _OnDropConnection(state, false);
		}

        private void _OnDropConnection(ConnectionState state, bool noLock)
        {
            // 서버가 종료상태면 다른 처리를 하지 않는다.
            if (m_isAliveThread == false)
                return;

            if (noLock)
            {
                ClientData data = null;
                if(m_arrClients.TryRemove(state, out data))
                {
                    NetworkServer.Instance.RemoveClient(state);
                }                
            }
            else
            {
                DdMonitor.Enter(m_arrClients, true);		
                ClientData data = null;
                if (m_arrClients.TryRemove(state, out data))
                {
                    NetworkServer.Instance.RemoveClient(state);
                } 
                DdMonitor.Exit(m_arrClients, true);		
                
            }

            ClientData client = (ClientData)state.Tag;
            if (client.ClientType == TCP_CLIENT.SENSOR_MONITOR2)
            {
                SendDisconnectAllReciverState(noLock, true);
            }
            else if (client.ClientType == TCP_CLIENT.PSM_SENSOR_SERVER)
            {

                WriteLineLog("Drop PSM Sensor");
                SendDisconnectAllPSMReciverState(noLock, true);
            }

            client.TempData = null;

            try
            {
                GC.Collect();
            }
            catch (System.Exception ex)
            {
                ConnectionLogEx.Instance.WriteLine("CG.Collect", ex);
            }
        }

        private void SendDisconnectAllPSMReciverState(bool noLock, bool noOnDropConnection)
        {
            ArrayList arRecivers = ReciverManager.Instance.GetPSMReciverList();
            WriteLineLog("Drop PSM Sensor Count : " + arRecivers.Count);
            foreach (Reciver reciver in arRecivers)
            {
                WriteLineLog("Drop PSM Sensor : " + reciver.ID);
                ReciverManager.Instance.UpdateState(reciver.ID, false, false);
                //SendReciverState(reciver.ID, false, TCP_CLIENT.SDMS_CLIENT, noLock, noOnDropConnection);
            }
        }

		private void SendDisconnectAllReciverState(bool noLock, bool noOnDropConnection)
		{
			ArrayList arRecivers = (ArrayList)ReciverManager.Instance.GetReciverList().Clone();
			foreach(Reciver reciver in arRecivers)
			{
				ReciverManager.Instance.UpdateState(reciver.ID, false, false);
                //SendReciverState(reciver.ID, false, TCP_CLIENT.SDMS_CLIENT, noLock, noOnDropConnection);
			}		
		}

        // 자기 자신을 제외한 다른 클라이언트에 전송
        public void SendDataToOther(byte[] bytes, ClientData sender, bool nolock = false, byte clientType = TCP_CLIENT.ALL)
        {
            ICollection<ConnectionState> arClient = null;
            DdMonitor.Enter (m_arrClients, true);
            {
                arClient = m_arrClients.Keys;
            }
            DdMonitor.Exit(m_arrClients, true);

            ArrayList arrDropStates = null;
            foreach (ConnectionState state in arClient)
            {
                ClientData client = (ClientData)state.Tag;
                if (client == null || client.ClientType == TCP_CLIENT.UNKNOWN)
                    continue;

                if (clientType == TCP_CLIENT.ALL || clientType == client.ClientType)
                {
                    if (sender != client)
                    {
                        try
                        {
                            Send(bytes, 0, bytes.Length, state, nolock, arrDropStates);
                        }
                        catch (System.Exception ex)
                        {
                            ConnectionLogEx.Instance.WriteLine("SendDataToOther", ex);
                        }
                    }
                }
            }
            ProcessDropList(arrDropStates);
        }
       
        // nClientCount가 0보다 크면 nCount만큼의 Client에게만 데이터를 보낸다.
        public void SendData(byte[] bytes, bool noLock = false, byte clientType = TCP_CLIENT.ALL, int nClientCount = -1)
        {

            if (!noLock)
            {
                SendClientData(bytes, clientType, noLock);                
            }
            else
            {
                SendClientData(bytes, clientType, noLock);                
            }
        }
        private static int nCountThread = 0;
        // 연결이 지속되고 있는지 여부를 확인하는 Thread
        private void PingThread()
        {
            byte[] data = new byte[6] { TCP_ID.ARE_YOU_THERE, 0, 0, 0, 0, 0 };
            byte[] data2 = new byte[6] { TCP_ID.WHO_ARE_YOU, 0, 0, 0, 0, 0 };

            while (m_isAliveThread)
            {
                ICollection<ConnectionState> arClientList = null;
                DdMonitor.Enter(m_arrClients, false);
                {
                    arClientList = m_arrClients.Keys;
                }
                DdMonitor.Exit(m_arrClients, false);

                int nClientCount = arClientList.Count;

                foreach(ConnectionState state in arClientList)
                {
                    ClientData client = (ClientData)state.Tag;
                    if (!state.Connected || client.PingCount > 5)
                    {        
                        try
                        {
                            state.EndConnection();    
                            NetworkServer.Instance.RemoveClient(state);
                            client.TempData = null;  
                        }
                        catch (System.Exception ex)
                        {
                            ConnectionLogEx.Instance.WriteLine("PingThread", ex);
                        }                  
                    }
                    else
                    {                        
                        try
                        {

                            if (Send(data, 0, data.Length, state, true))
                                client.PingCount++;
                        }
                        catch (System.Exception ex)
                        {
                            ConnectionLogEx.Instance.WriteLine("PingThread Send", ex);
                        }
                       
                    }
                }
                //}
                Thread.Sleep(1000);

                nCountThread++;

                if (nCountThread == 3600)
                {
                    nCountThread = 0;
                    try
                    {
                        GC.Collect();
                    }
                    catch (Exception ex)
                    {
                        ConnectionLogEx.Instance.WriteLine("PingThread GCCollect", ex);
                    }

                }
            }
        }

        public void ReleaseThread()
        {
            m_isAliveThread = false;

            // 쓰레드 종료를 2초간 기다린다.
            Thread.Sleep(2000);

            try
            {
                if (m_PingThread.IsAlive)
                {
                    m_PingThread.Abort();
                    m_PingThread.Join();
                }
            }
            catch (System.Exception ex)
            {
                ConnectionLogEx.Instance.WriteLine("ReleaseThread", ex);
            }
        }      

        public void SendSensorReactionLog(SensorReactionLog log, byte clientType)
        {
            ICollection<ConnectionState> arClient = null;
            DdMonitor.Enter(m_arrClients, true);
            {
                arClient = m_arrClients.Keys;
            }
            DdMonitor.Exit(m_arrClients, true);

            if (arClient.Count > 0)
            {
                ArrayList arrDropStates = new ArrayList();
                byte[] bytes = log.MakeBytes();

                foreach (ConnectionState state in arClient)
                {
                    ClientData client = (ClientData)state.Tag;
                    if (client == null || client.ClientType == TCP_CLIENT.UNKNOWN)
                        continue;

                    if (clientType == TCP_CLIENT.ALL || clientType == client.ClientType)
                    {
                        try
                        {
                            Send(bytes, 0, bytes.Length, state, true, arrDropStates);
                        }
                        catch (System.Exception ex)
                        {
                            ConnectionLogEx.Instance.WriteLine("SendSensorReactionLog", ex);
                        }
                    }
                }

                ProcessDropList(arrDropStates);
            }
            
        }

        private void SendCurrentFireSensorSignal(ConnectionState state, byte clientType)
        {
            ClientData client = (ClientData)state.Tag;
            if (client == null)
                return;

            if (client.ClientType == TCP_CLIENT.UNKNOWN)
                return;

            if (clientType != TCP_CLIENT.ALL && clientType != client.ClientType)
                return;

            foreach (TimeHistory history in m_arTimeHistory)
            {
                if (history.LastReactionLog == null)
 					continue;

                if (history.LastReactionLog.Type != libSensorProcess.ReactionType.NOTIFY_FIRE)
					//&& history.LastReactionLog.Type != SensorReactionLog.ReactionType.NOTIFY_FIRE)
                    continue;

                SendStatusSensorSignal(history.LastReactionLog, state);
                break;
            }
        }

        // SensorReactionLog가 하나도 없으면 2바이트만 전송된다.
        // 이를 받은 Client는 모든 화재 상황이 해제된다.
        public void SendSensorReactionLogList(ConnectionState state, byte clientType)
        {
            ClientData client = (ClientData)state.Tag;
            if (client == null)
                return;

            if (client.ClientType == TCP_CLIENT.UNKNOWN)
                return;

            if (clientType != TCP_CLIENT.ALL && client.ClientType != clientType)
                return;

            ArrayList arrLogBytes = new ArrayList();
            int nByteCount = 0;

            int nHistoryCount = m_arTimeHistory.Count;

            for (int i = 0; i < nHistoryCount;i++ )
            {               

                TimeHistory history = (TimeHistory)m_arTimeHistory[i];

                if (history.LastReactionLog == null)
                    continue;

                byte[] dataBytes = history.LastReactionLog.MakeBytes();
                arrLogBytes.Add(dataBytes);

                nByteCount += dataBytes.Length - 6;
            }

            byte[] bytes = new byte[nByteCount + 6];

            bytes[0] = TCP_ID.SENSOR_REACTION_HISTORY_DATA_LIST;
            bytes[1] = 0;            

            int nLogCount = (int)arrLogBytes.Count;
            byte[] chunkBytes = BitConverter.GetBytes(nLogCount * 7);
            int nIndex = 6;

            System.Buffer.BlockCopy(chunkBytes, 0, bytes, 2, 4);

            for (int i = 0; i < nLogCount;i++ )
            {
                byte[] dataBytes = (byte[])arrLogBytes[i];
                int nDataLength = dataBytes.Length - 6;

				// dataBytes가 헤더 정보를 포함하여 있어 이를 제외 하기 위해 시작을 6번째 부터 한다.
                System.Buffer.BlockCopy(dataBytes, 6, bytes, nIndex, nDataLength);
                nIndex += nDataLength;
            }

            try
            {
                Send(bytes, 0, bytes.Length, state);
            }
            catch (System.Exception ex)
            {
                ConnectionLogEx.Instance.WriteLine("SendSensorReactionLogList", ex);
            }
            
        }

        public void AddTimeHistoryList(ArrayList arrTimeHistory)
        {
            DdMonitor.Enter(arrTimeHistory, false);
            {
                {
                    foreach (TimeHistory history in arrTimeHistory)
                    {
                        m_arTimeHistory.Add(history);
                    }
                }
            }
            DdMonitor.Exit(arrTimeHistory, false);
            
        }

        public void AddTimeHistory(TimeHistory history)
        {
            DdMonitor.Enter(m_arTimeHistory, false);
            {
                m_arTimeHistory.Add(history);
            }
            DdMonitor.Exit(m_arTimeHistory, false);
        }

        public void RemoveTimeHistory(TimeHistory history)
        {
            DdMonitor.Enter(m_arTimeHistory, false);
            {
                m_arTimeHistory.Remove(history);
            }
            DdMonitor.Exit(m_arTimeHistory, false);
        }

        public int GetTimeHistoryCount()
        {
            return m_arTimeHistory.Count;
        }

		public bool ExistFireDetectSituation()
		{
            DdMonitor.Enter(m_arTimeHistory, false);
			{
				foreach (TimeHistory history in m_arTimeHistory)
				{
					if (history.LastReactionLog == null)
						continue;

                    if (history.LastReactionLog.Type == libSensorProcess.ReactionType.NOTIFY_FIRE)
						return true;						
				}
			}
            DdMonitor.Exit(m_arTimeHistory, false);
			return false;
		}

        public TimeHistory GetTimeHistory(int nIndex)
        {
            TimeHistory history = null;

            DdMonitor.Enter(m_arTimeHistory, false);
            {
                if (nIndex >= m_arTimeHistory.Count || nIndex < 0)
                    history = null;
                else
                    history = (TimeHistory)m_arTimeHistory[nIndex];
            }
            DdMonitor.Exit(m_arTimeHistory, false);

            return history;
        }

        public TimeHistory FindTimeHistory(int nSensorHistoryID)
        {
            lock (m_arTimeHistory)
            {
                foreach (TimeHistory history in m_arTimeHistory)
                {
                    if (history.HistoryID == nSensorHistoryID)
                        return history;
                }
            }

            return null;
        }

		public void SendBeginRestore()
		{			
			byte[] bytes = new byte[6] { TCP_ID.BEGEIN_RESTORE, 0, 0, 0, 0, 0 };
            SendData(bytes, false, TCP_CLIENT.SOP_RESTORE);		
		}

		public void SendAllRestart()
		{
			byte[] bytes = new byte[6] { TCP_ID.END_RESTORE, 0, 0, 0, 0, 0 };
			SendData(bytes, false, TCP_CLIENT.INTEGRATE_MANAGE);	
		}

        // nChangedConfig : Config.ConfigType의 비트 조합
        public void SendChangedConfig(int nChangedConfig, byte clientType)
        {

            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(TCP_CLIENT.SDMS_CLIENT);
            arrDatas.Add(SDMSConfig.PropertyName);
            arrDatas.Add(nChangedConfig.ToString());

            byte[] bytes = TcpHelper.MakeBytes(TCP_ID.CHANGE_CONFIG, arrDatas);

            SendClientData(bytes, clientType, true);
        }

		public void SendSelectMission(byte[] bytes)
		{
            SendClientData(bytes, TCP_CLIENT.SOP_SIMULATOR, true);
		}

        public void RememberSelectMission(byte[] bytes)
        {
            m_arrSelectMission = bytes;
        }

        public void SendCurrentSelectMission()
        {
            if (m_arrSelectMission != null)
            {
                SendClientData(m_arrSelectMission, TCP_CLIENT.SOP_SIMULATOR, true);
            }
        }

        // dtTime 이전에 생성된 SensorTagHistory는 모두 지우도록 한다.
        public void SendRemoveSensorTagHistory(DateTime dtTime, byte clientType = TCP_CLIENT.SDMS_CLIENT)
        {
            ICollection<ConnectionState> arClient = null;
            DdMonitor.Enter(m_arrClients, true);
            {
                arClient = m_arrClients.Keys;
            }
            DdMonitor.Exit(m_arrClients, true);

            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(ServerCommandType.DELETE_SENSOR_TAG_HISTORY);
            arrDatas.Add(dtTime.Ticks);

            byte[] bytes = TcpHelper.MakeBytes(TCP_ID.SERVER_COMMAND, arrDatas);

            ArrayList arrDropStates = null;
            foreach (ConnectionState state in arClient)
            {
                ClientData client = (ClientData)state.Tag;
                if (client == null || client.ClientType == TCP_CLIENT.UNKNOWN)
                    continue;

                if (clientType == TCP_CLIENT.ALL || clientType == client.ClientType)
                {
                    try
                    {
                        Send(bytes, 0, bytes.Length, state, false, arrDropStates);
                    }
                    catch (System.Exception ex)
                    {
                        ConnectionLogEx.Instance.WriteLine("SendRemoveSensorTagHistory", ex);
                    }
                }
            }
            ProcessDropList(arrDropStates);
        }
    }

	public class ArrayListEx : ArrayList
	{
		public ArrayListEx()
		{
		}
		
		public override int Add(object value)
		{
			return base.Add(value);
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

        public TimeHistory(int nID, DateTime t, SensorReactionLog.DetectionStatus status)
		{
			dtTime = t;
			m_nSensorHistoryID = nID;
            m_Status = status;
		}

        private SensorReactionLog.DetectionStatus m_Status = SensorReactionLog.DetectionStatus.REAL;
        public SensorReactionLog.DetectionStatus DetectStatus
        {
            get { return m_Status; }
            set { m_Status = value; }
        }

	}

	public class ConnectionLogEx : ConnectionLog
	{
		private log4net.ILog logger = null;
        private static ConnectionLogEx m_instance2 = new ConnectionLogEx();

        public static ConnectionLogEx Instance
        {
            get
            {
                return m_instance2;
            }
        }

		public static bool MakeInstance()
		{
			m_instance2.logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

			m_instance2.m_isOpened = true;
			return m_instance2.m_isOpened;
		}

		public override bool Write(object str, bool writeTime = true)
		{
			if (logger != null)
				logger.DebugFormat("{0}", str);

			return true;
		}

        public override bool WriteLine(object str, Exception e)
        {
            if (logger != null)
            {
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(e, true);
                logger.Debug("프로그램 오류 : " + str, e);
                logger.Debug("Line: " + trace.GetFrame(0).GetFileLineNumber());
            }
            return true;
        }

		public override bool WriteLine(object str, bool writeTime = true)
		{
			if (logger != null)
				logger.Debug(str);

			return true;
		}
	}
}
