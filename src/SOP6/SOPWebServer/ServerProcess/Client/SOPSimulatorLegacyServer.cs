using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Collections;
using System.ServiceModel;
using AgentFactory;

namespace ServerProcess.Client
{
    using ServerProcess.Data;

    public class SOPSimulatorLegacyServer : BaseClient, ISOPSimulatorServer
    {
        public class ClientData : ServerProcess.Client.ClientData
        {
            private int m_nSOPGenUserID = -1;
            private string m_strSOPGenUserID = "";
            private string m_strNickName = "";
            private int m_nUserLevel = 0;

            public int ID
            {
                get { return m_nSOPGenUserID; }
                set { m_nSOPGenUserID = value; }
            }

            public string UserID
            {
                get { return m_strSOPGenUserID; }
                set { m_strSOPGenUserID = value; }
            }

            public string NickName
            {
                get { return m_strNickName; }
                set { m_strNickName = value; }
            }

            // 계정등급
            // 값이 높을수록 상위 권한을 가진다.
            public int UserLevel
            {
                get { return m_nUserLevel; }
                set { m_nUserLevel = value; }
            }

            public ClientData()
                : base()
            {
            }

            public ClientData(string strSessionID, IPostMan postMan)
                : base(strSessionID, postMan)
            {
            }

            public ClientData(string strSessionID, IPostMan postMan, int nClientType, int nClientSubType)
                : base(strSessionID, postMan, nClientType, nClientSubType)
            {
            }
        }

        private static SOPSimulatorLegacyServer m_instance = null;

        // 현재 제어권 소유자
        private ClientData m_controlClient = null;
        // 제어권 소유자로부터의 마지막 제어권 소유확인 시간
        private DateTime m_dtLastControlCheck = new DateTime();
        // 마지막에 제어권을 소유하고 있었던 SOPGenUserID
        private int m_nLastControlUserID = -1;
        // DB로부터 m_nLastControlUserID를 읽어온 시간
        private DateTime m_dtReadLastControlUser = new DateTime();

        // 제어권 양도요청을 보낸 Client
        private ClientData m_requestControlClient = null;

        private bool m_initialized = true;

        public static SOPSimulatorLegacyServer Instance
        {
            get { return m_instance; }
        }

        public override int ClientType
        {
            get { return SOPWebServer.ClientType.SOP_SIMULATOR; }
        }

        public ClientData ControlClient
        {
            get { return m_controlClient; }
        }

        // 초기화가 완료되었는가?
        public bool Initialized
        {
            get { return m_initialized; }
        }

        public SOPSimulatorLegacyServer()
            : base()
        {
            m_instance = this;
        }

        public SOPSimulatorLegacyServer(Factory factory, IPostOffice postOffice)
            : base(factory, postOffice)
        {
            m_instance = this;
            m_agent = m_agentFactory.MakeAgent(Factory.AgentType.SOPSimulator);
        }

        protected override void OnLoadEvent()
        {
            // 서버가 재가동되면 그 이전에 제어권을 소유했던 클라이언트가 누군지 확인한다.
            // 만일 해당 클라이언트가 아직 접속중이라면 그 클라이언트에게 다시 제어권을 부여한다.
            CheckPrevControlClient();
            m_initialized = true;
        }

        // 서버가 재가동되면 그 이전에 제어권을 소유했던 클라이언트가 누군지 확인한다.
        // 만일 해당 클라이언트가 아직 접속중이라면 그 클라이언트에게 다시 제어권을 부여한다.
        private void CheckPrevControlClient()
        {
            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return;

            string strSQL = "Select UserID from ControlUser where SiteID = " + dbMgr.SiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count > 0)
            {
                VariousData<int> userID = WebDBManager.GetIntField(arrResult[0].ToString());

                if (userID != null)
                {
                    m_nLastControlUserID = userID.Data;
                    m_dtReadLastControlUser = DateTime.Now;
                }
            }

            dbMgr.Close();
        }

        protected override ServerProcess.Client.ClientData MakeClientData(int nClientType, int nClientSubType, OperationContext ctx, string strIP, int nPort)
        {
            if (m_postOffice != null)
            {
                IPostMan postMan = m_postOffice.GetPostMan(ctx);
                SOPSimulatorLegacyServer.ClientData data = new SOPSimulatorLegacyServer.ClientData(ctx.SessionId, postMan, nClientType, nClientSubType);
                data.IP = strIP;
                data.Port = nPort;
                postMan.ClientData = data;

                SendDataToSOPSimulator(SOPWebServer.Header.REQUEST_CLIENT_INFO, null);
                return data;
            }

            return null;
        }

        protected override int OnReceiveEvent(ServerProcess.Client.ClientData data, OperationContext ctx, int header, byte[] messages, ArrayList arrDatas)
        {
            if (header == SOPWebServer.Header.REPLY_CLIENT_INFO)
                return ProcessClientInfo((ClientData)data, arrDatas);
            else if (header == SOPWebServer.Header.CONFIRM_HAS_CONTROL)
                return ProcessConfirmHasControl((ClientData)data, arrDatas);
            else if (header == SOPWebServer.Header.REQUEST_CONTROL)
                return ProcessRequestControl((ClientData)data);
            else if (header == SOPWebServer.Header.CANCEL_REQUEST_CONTROL)
                return ProcessCancelRequestControl((ClientData)data);
            else if (header == SOPWebServer.Header.GIVE_CONTROL)
                return ProcessGiveControl((ClientData)data, arrDatas);
            else if (header == SOPWebServer.Header.REJECT_REQUEST_CONTROL)
                return ProcessRejectRequestControl((ClientData)data, arrDatas);
            else if (header == SOPWebServer.Header.STEAL_CONTROL)
                return ProcessStealControl((ClientData)data);
            else if (header == SOPWebServer.Header.RETURN_CONTROL)
                return ProcessReturnControl((ClientData)data);
            else if (header == SOPWebServer.Header.SOP_SELECT_MISSION)
                return ProcessRelay(header, messages);
            else if (header == SOPWebServer.Header.CHANGE_CONFIG)
                return ProcessRelay(header, messages, data);
            else if (header == SOPWebServer.Header.CHANGE_WORK_MEMBER)
                return ProcessRelay(header, messages, data);
            else if (header == SOPWebServer.Header.SOP_SIMULATOR_COMMAND)
                return ProcessSOPSimulatorCommand(data, header, messages, arrDatas);
            else if (header == SOPWebServer.Header.ALARM_SOP_RESULT)
                return ProcessAlarmSOPResult(arrDatas);

            /*else if (header == SOPWebServer.Header.RUN_SOP)
            {
                ProcessRunSOP(bytes);
            }
            else if (nHeader == TCP_ID.IGNORE_SOP)
            {
                ProcessIgnoreSOP(bytes);
            }
            else if (nHeader == TCP_ID.CONFIRM_GIVE_CONTROL)
            {
                ProcessConfirmGiveControl();
            }
            else if (nHeader == TCP_ID.CONFIRM_TAKE_CONTROL)
            {
                ProcessConfirmTakeControl();
            }
            else if (nHeader == TCP_ID.REQUEST_CONTROL)
            {
                ProcessRequestControl();
            }
            else if (nHeader == TCP_ID.CANCEL_REQUEST_CONTROL)
            {
                ProcessCancelRequestControl();
            }
            else if (nHeader == TCP_ID.GIVE_CONTROL)
            {
                // 제어권 전달
                ProcessGiveControl(bytes);
            }
            else if (nHeader == TCP_ID.REJECT_REQUEST_CONTROL)
            {
                // 제어권 거부
                ProcessRejectRequestControl(bytes);
            }
            else if (nHeader == TCP_ID.STEAL_CONTROL)
            {
                // 제어권 강제로 뺏기
                ProcessStealControl();
            }
            else if (nHeader == TCP_ID.RETURN_CONTROL)
            {
                // 제어권 반납
                ProcessReturnControl();
            }
            else if (nHeader == TCP_ID.SOP_SELECT_MISSION)
            {
                ProcessSelectMission(bytes);
            }
            else if (nHeader == TCP_ID.SOP_CURRENT_SELECT_MISSION)
            {
                ProcessCurrentSelectMission(bytes);
            }
            else if (nHeader == TCP_ID.CHANGE_CONFIG)
            {
                ProcessChangedConfig(arrDatas, bytes);
            }
            else if (nHeader == TCP_ID.CHAGNE_WORK_MEMBER)
            {
                ProcessUpdateToWorkingMemberData(bytes);
            }
            else if (nHeader == TCP_ID.SOP_SIMULATOR_COMMAND)
            {
                ProcessSOPSimulatorCommand(arrDatas, bytes);
            }*/

            return SOPWebServer.ErrorMessageType.UNKNOWN_HEADER;
        }

        private int ProcessAlarmSOPResult(ArrayList arrDatas)
        {
            if (arrDatas == null)
                return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;

            int nDataCount = arrDatas.Count;

            for (int i = 0; i < nDataCount - 1; i += 2)
            {
                if (arrDatas[i] is int && arrDatas[i + 1] is int)
                {
                    int nSensorZoneHistoryID = (int)arrDatas[i];
                    int nResultType = (int)arrDatas[i + 1];

                    AlarmData alarm = AlarmManager.Instance.GetAlarm(nSensorZoneHistoryID);

                    if (alarm != null)
                        alarm.SOPProcess = (AlarmData.SOPProcessType)nResultType;
                }
                else
                    return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
            }

            return SOPWebServer.ErrorMessageType.SUCCESS;
        }

        private int ProcessSOPSimulatorCommand(Client.ClientData data, int header, byte[] messages, ArrayList arrDatas)
        {
            if (arrDatas != null && arrDatas.Count > 0 && arrDatas[0] is byte)
            {
                byte command = (byte)arrDatas[0];

                if (command == SOPWebServer.SOPSimulatorCommandType.RESET_USER_DEFINED_TEAM_NAMES)
                {
                    return ProcessRelay(header, messages, data);
                }

                return SOPWebServer.ErrorMessageType.UNKNOWN_COMMAND;
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        // Client들에게 그대로 전달
        private int ProcessRelay(int header, byte[] messages, Client.ClientData exceptClient = null)
        {
            // Debuggin을 위한 Break Point를 사용하면 아래의 로직으로 인하여 서버가 Block된다.
            // Timer를 사용하도록 한다.
            SendClientData(header, messages, SOPWebServer.ClientType.SOP_SIMULATOR, -1, exceptClient);
            /*List<Client.ClientData> clients = GetClientDatas();
            List<Client.ClientData> removeClients = new List<Client.ClientData>();

            foreach (Client.ClientData client in clients)
            {
                if (client == exceptClient)
                    continue;

                if (client.PostMan.ClientChannel.State == CommunicationState.Opened)
                    client.PostMan.OnRing(header, messages);
                else
                    removeClients.Add(client);
            }

            foreach (Client.ClientData client in removeClients)
            {
                RemoveClient(client);
            }

            removeClients.Clear();
            clients.Clear();*/

            return SOPWebServer.ErrorMessageType.SUCCESS;
        }

        // 제어권 반납
        // 제어권은 반납한 Client를 제외한 다른 Client 가운데 하나에게 전달된다.
        // 만일, 다른 Client가 없다면 반납되지 않는다.
        private int ProcessReturnControl(ClientData data)
        {
            if (m_controlClient != data)
                return SOPWebServer.ErrorMessageType.NO_PERMISSION;

            List<Client.ClientData> clients = GetClientDatas();
            ClientData target = null;

            foreach (ClientData client in clients)
            {
                if (client != data)
                {
                    target = client;
                    break;
                }
            }

            if (target != null)
            {
                SetControlClient(target);
                return SOPWebServer.ErrorMessageType.SUCCESS;
            }

            return SOPWebServer.ErrorMessageType.NO_OTHER_CLIENTS;
        }

        // 제어권 강제로 뺏기
        // 현재 제어권을 가진 Client로부터 제어권을 빼앗아 data에게 전달한다.
        private int ProcessStealControl(ClientData data)
        {
            if (m_controlClient != data)
                SetControlClient(data);

            return SOPWebServer.ErrorMessageType.SUCCESS;
        }

        private int ProcessRejectRequestControl(ClientData data, ArrayList arrDatas)
        {
            if (m_controlClient == data)
            {
                if (arrDatas != null && arrDatas.Count >= 2 && arrDatas[0] is string && arrDatas[1] is string)
                {
                    string strUserID = (string)arrDatas[0];
                    string strIP = (string)arrDatas[1];

                    List<Client.ClientData> clients = GetClientDatas();

                    foreach (ClientData client in clients)
                    {
                        if (client.UserID == strUserID && client.IP == strIP)
                        {
                            // Debuggin을 위한 Break Point를 사용하면 아래의 로직으로 인하여 서버가 Block된다.
                            // Timer를 사용하도록 한다.
                            SendClientData(SOPWebServer.Header.REJECT_REQUEST_CONTROL, null, client);
                            /*if (client.PostMan.ClientChannel.State == CommunicationState.Opened)
                            {
                                client.PostMan.OnRing(SOPWebServer.Header.REJECT_REQUEST_CONTROL, null);
                            }*/

                            break;
                        }
                    }

                    return SOPWebServer.ErrorMessageType.SUCCESS;
                }

                return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
            }

            return SOPWebServer.ErrorMessageType.NO_PERMISSION;
        }

        private int ProcessGiveControl(ClientData data, ArrayList arrDatas)
        {
            if (m_controlClient == data)
            {
                if (arrDatas != null && arrDatas.Count >= 3 && arrDatas[0] is int)
                {
                    int nUserIDCount = (int)arrDatas[0];

                    if (nUserIDCount <= 0)
                        return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;

                    string strUserID = (string)arrDatas[1];
                    string strIP = (string)arrDatas[nUserIDCount + 1];

                    List<Client.ClientData> clients = GetClientDatas();
                    //List<Client.ClientData> removeClients = new List<Client.ClientData>();

                    foreach (ClientData client in clients)
                    {
                        if (client.UserID == strUserID && client.IP == strIP)
                        {
                            SetControlClient(client);
                            break;
                        }
                    }

                    // 제어권을 취득할 클라이언트와 그렇지 않은 클라이언트에 각각 통지해준다.
                    foreach (ClientData client in clients)
                    {
                        if (client == m_controlClient)
                        {
                            // Debuggin을 위한 Break Point를 사용하면 아래의 로직으로 인하여 서버가 Block된다.
                            // Timer를 사용하도록 한다.
                            SendClientData(SOPWebServer.Header.GIVE_CONTROL, null, client);
                            /*if (client.PostMan.ClientChannel.State == CommunicationState.Opened)
                                client.PostMan.OnRing(SOPWebServer.Header.GIVE_CONTROL, null);
                            else
                                removeClients.Add(client);*/
                        }
                        else
                        {
                            for (int i = 2; i <= nUserIDCount; i++)
                            {
                                string strID = (string)arrDatas[i];

                                if (client.UserID == strID)
                                {
                                    // Debuggin을 위한 Break Point를 사용하면 아래의 로직으로 인하여 서버가 Block된다.
                                    // Timer를 사용하도록 한다.
                                    SendClientData(SOPWebServer.Header.REJECT_REQUEST_CONTROL, null, client);
                                    /*if (client.PostMan.ClientChannel.State == CommunicationState.Opened)
                                        client.PostMan.OnRing(SOPWebServer.Header.REJECT_REQUEST_CONTROL, null);
                                    else
                                        removeClients.Add(client);*/
                                }
                            }
                        }
                    }

                    /*foreach (Client.ClientData client in removeClients)
                    {
                        RemoveClient(client);
                    }*/

                    return SOPWebServer.ErrorMessageType.SUCCESS;
                }

                return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
            }

            return SOPWebServer.ErrorMessageType.NO_PERMISSION;
        }

        private int ProcessCancelRequestControl(ClientData data)
        {
            if (m_requestControlClient != null)
            {
                if (m_requestControlClient.PostMan.ClientChannel.State == CommunicationState.Opened)
                {
                    ArrayList arrDatas = new ArrayList();

                    arrDatas.Add(data.UserID);
                    arrDatas.Add(data.NickName);
                    arrDatas.Add(data.IP);

                    byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);

                    if (m_requestControlClient.PostMan.ClientChannel.State == CommunicationState.Opened)
                    {
                        // Debuggin을 위한 Break Point를 사용하면 아래의 로직으로 인하여 서버가 Block된다.
                        // Timer를 사용하도록 한다.
                        SendClientData(SOPWebServer.Header.CANCEL_REQUEST_CONTROL, bytes, m_requestControlClient);
                        //m_requestControlClient.PostMan.OnRing(SOPWebServer.Header.CANCEL_REQUEST_CONTROL, bytes);
                        m_requestControlClient = m_controlClient;
                    }
                }
            }

            return SOPWebServer.ErrorMessageType.SUCCESS;
        }

        private int ProcessRequestControl(ClientData data)
        {
            if (m_controlClient == data)
            {
            }
            else if (m_controlClient == null)
            {
                SetControlClient(data);
            }
            else
            {
                ArrayList arrDatas = new ArrayList();

                arrDatas.Add(data.UserID);
                arrDatas.Add(data.NickName);
                arrDatas.Add(data.IP);

                byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);

                if (m_controlClient.PostMan.ClientChannel.State == CommunicationState.Opened)
                {
                    // Debuggin을 위한 Break Point를 사용하면 아래의 로직으로 인하여 서버가 Block된다.
                    // Timer를 사용하도록 한다.
                    SendClientData(SOPWebServer.Header.REQUEST_CONTROL, bytes, m_controlClient);
                    //m_controlClient.PostMan.OnRing(SOPWebServer.Header.REQUEST_CONTROL, bytes);
                    m_requestControlClient = m_controlClient;
                }
                else
                {
                    RemoveClient(m_controlClient);
                    SetControlClient(data);
                }
            }

            return SOPWebServer.ErrorMessageType.SUCCESS;
        }

        private int ProcessConfirmHasControl(ClientData data, ArrayList arrDatas)
        {
            if (m_controlClient == data)
            {
                m_dtLastControlCheck = DateTime.Now;
                return SOPWebServer.ErrorMessageType.SUCCESS;
            }

            return SOPWebServer.ErrorMessageType.NO_PERMISSION;
        }

        private int ProcessClientInfo(ClientData data, ArrayList arrDatas)
        {
            if (data != null && arrDatas.Count >= 4 && arrDatas[0] is int && arrDatas[1] is string && arrDatas[2] is string && arrDatas[3] is int)
            {
                int nSOPGenUserID = (int)arrDatas[0];
                string strUserID = (string)arrDatas[1];
                string strNickName = (string)arrDatas[2];
                int nUserLevel = (int)arrDatas[3];

                data.ID = nSOPGenUserID;
                data.UserID = strUserID;
                data.NickName = strNickName;
                data.UserLevel = nUserLevel;

                if (m_controlClient == null)
                {
                    if (m_nLastControlUserID == nSOPGenUserID)
                    {
                        SetControlClient(data);
                    }
                }

                return SOPWebServer.ErrorMessageType.SUCCESS;
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        private void SetControlClient(ClientData data)
        {
            if (data == null)
            {
                if (m_nLastControlUserID > 0)
                {
                    DirectDBManager dbMgr = m_dbMgr.Clone();

                    if (dbMgr.Connect() == false)
                        return;

                    string strSQL = "Update ControlUser set UserID = NULL where SiteID = " + dbMgr.SiteID.ToString();
                    dbMgr.GetResultData(strSQL);
                    dbMgr.Close();
                }

                m_nLastControlUserID = -1;
                m_controlClient = data;
            }
            else
            {
                if (m_nLastControlUserID != data.ID)
                {
                    DirectDBManager dbMgr = m_dbMgr.Clone();

                    if (dbMgr.Connect() == false)
                        return;

                    string strSQL = string.Format("Update ControlUser set UserID = {0} where SiteID = {1}", data.ID, dbMgr.SiteID);

                    if (dbMgr.GetResultData(strSQL) == null)
                    {
                        // 데이터가 존재하지 않는 경우
                        strSQL = string.Format("Insert into ControlUser(ID, UserID, SiteID) values((select isnull(max(id), 0) + 1 from ControlUser), {0}, {1})", data.ID, dbMgr.SiteID);

                        if (dbMgr.GetResultData(strSQL) == null)
                        {
                            m_nLastControlUserID = -1;
                            m_controlClient = null;
                        }
                        else
                        {
                            m_nLastControlUserID = data.ID;
                            m_controlClient = data;
                            m_dtLastControlCheck = DateTime.Now;
                        }
                    }
                    else
                    {
                        m_nLastControlUserID = data.ID;
                        m_controlClient = data;
                        m_dtLastControlCheck = DateTime.Now;
                    }

                    dbMgr.Close();
                }
                else
                {
                    m_controlClient = data;
                    m_dtLastControlCheck = DateTime.Now;
                }
            }
        }

        private int SendDataToSOPSimulator(int header, ArrayList arrDatas)
        {
            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            SendClientData(header, bytes, SOPWebServer.ClientType.SOP_SIMULATOR, SOPWebServer.ClientSubType.SOP_SIMULATOR);
            return SOPWebServer.ErrorMessageType.SUCCESS;
        }

        protected override void OnTimerEvent()
        {
            base.OnTimerEvent();

            List<Client.ClientData> clients = GetClientDatas();

            if (clients.Count == 0)
            {
                SetControlClient(null);
                return;
            }

            List<AlarmData> alarms = AlarmManager.Instance.CurrentAlarms;
            // SOP 처리가 되지 않은 알람들에 대한 byte Array를 생성한다.
            byte[] sopBytes = MakeAlarmDataList(alarms);

            int nControlUserID = CheckControlClient(clients);

            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(nControlUserID);
            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);

            List<Client.ClientData> removeClients = new List<Client.ClientData>();

            foreach (Client.ClientData client in clients)
            {
                IClientChannel channel = client.PostMan.ClientChannel;

                if (channel.State == CommunicationState.Opened)
                {
                    client.PostMan.OnRing(SOPWebServer.Header.CONTROL_CLIENT, bytes);

                    // 아직 SOP 처리가 되지 않은 알람들에 대한 정보를 SOP Simulator에게 알려준다.
                    if (sopBytes != null)
                        client.PostMan.OnRing(SOPWebServer.Header.ALARM_DATA_LIST, sopBytes);
                }
                else
                {
                    removeClients.Add(client);

                    if (m_controlClient == client)
                        SetControlClient(null);
                }
            }

            foreach (Client.ClientData client in removeClients)
            {
                RemoveClient(client);
            }

            removeClients.Clear();
            clients.Clear();
        }

        // SOP 처리가 되지 않은 알람들에 대한 byte Array를 생성한다.
        private byte[] MakeAlarmDataList(List<AlarmData> alarms)
        {
            ArrayList arrDatas = null;

            foreach (AlarmData alarm in alarms)
            {
                if (alarm.SOPProcess != AlarmData.SOPProcessType.None)
                    continue;

                if (arrDatas == null)
                    arrDatas = new ArrayList();

                int nZoneID, nEquipZoneID;
                GetZoneNEquipZoneIDFromAlarm(alarm, out nZoneID, out nEquipZoneID);

                arrDatas.Add((int)alarm.SensorType);
                arrDatas.Add(nEquipZoneID);
                arrDatas.Add(nZoneID);
                arrDatas.Add(alarm.TimeStamp.ToBinary());
                arrDatas.Add(alarm.SensorZoneID);
                arrDatas.Add(alarm.SensorZoneHistoryID);
            }

            if (arrDatas == null)
                return null;

            return SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
        }

        private void GetZoneNEquipZoneIDFromAlarm(AlarmData alarm, out int nZoneID, out int nEquipZoneID)
        {
            nZoneID = nEquipZoneID = -1;

            BaseSMSManager.SMSMessageType messageType = ServerProcess.Data.SMSManager.ReactionTypeToMessageType(alarm.Status, alarm.SensorType);

            if (messageType == BaseSMSManager.SMSMessageType.REPORT_FIRE && alarm.SensorZoneID <= 0)
            {
                // 수동화재신고
                nZoneID = -1;
                int.TryParse(alarm.ReactionHistoryParam1, out nZoneID);
            }
            else
            {
                SensorZoneGroup group = SensorZoneManager.Instance.GetSensorZoneGroup(alarm.SensorZoneID);

                if (group != null && group.EquipmentZone != null)
                {
                    nEquipZoneID = group.EquipmentZone.ID;

                    if (group.EquipmentZone.LinkedZoneList.Count > 0)
                    {
                        UnE.Spatial.Zone zone = (UnE.Spatial.Zone)group.EquipmentZone.LinkedZoneList[0];
                        nZoneID = zone.ID;
                    }
                }
            }
        }

        protected override void RemoveNotConnectedClients()
        {
            // 다른 서버들과 달리 SOPSimulatorServer는 OnTimerEvent에서 직접 Client들의 접속여부를 확인한다.
        }

        private int CheckControlClient(List<Client.ClientData> clients)
        {
            int nControlUserID = -1;
            bool newControlUser = true;
            Client.ClientData exceptClient = null;

            if (m_controlClient != null)
            {
                TimeSpan span = DateTime.Now - m_dtLastControlCheck;

                if (span.TotalSeconds <= 3.0)
                {
                    nControlUserID = m_controlClient.ID;
                    newControlUser = false;
                }
                else
                {
                    // 제어권 소유자로부터 3초 이내에 제어권 확인을 받지 못하면 제어권을 재할당한다.
                    exceptClient = m_controlClient;
                }
            }
            else
            {
                if (m_nLastControlUserID > 0)
                {
                    TimeSpan span = DateTime.Now - m_dtReadLastControlUser;

                    // 서버가 가동된지 3초가 지나지 않았으면 제어권 소유자가 접속할 때까지 기다린다.
                    if (span.TotalSeconds <= 3.0)
                    {
                        newControlUser = false;
                    }
                }
            }

            if (newControlUser)
            {
                foreach (Client.ClientData client in clients)
                {
                    if (client != exceptClient)
                    {
                        SetControlClient((ClientData)client);
                        break;
                    }
                }

                if (m_controlClient != null && m_controlClient != exceptClient)
                    nControlUserID = m_controlClient.ID;
                else
                    SetControlClient(null);
            }

            return nControlUserID;
        }

        public void SendSensorSignal(AlarmData alarm, int nZoneID, int nOriginSensorID, float x = 0.0f, float y = 0.0f, float z = 0.0f)
        {
            DirectDBManager dbMgr = m_dbMgr.Clone();

            if (dbMgr.Connect() == false)
                return;

            bool isRealMode = GetTrainingMode(dbMgr);
            dbMgr.Close();

            if (nZoneID < 0)
            {
                SensorZone sensorZone = SensorZoneManager.Instance.GetSensorZone(alarm.SensorZoneID);

                if (sensorZone != null && sensorZone.EquipZone != null)
                    nZoneID = sensorZone.EquipZone.ID;
            }

            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(nOriginSensorID);
            arrDatas.Add(alarm.SensorZoneHistoryID);
            arrDatas.Add(nZoneID);
            arrDatas.Add(alarm.TimeStamp.ToBinary());
            arrDatas.Add(x);
            arrDatas.Add(y);
            arrDatas.Add(z);
            arrDatas.Add(isRealMode ? 0 : 1);
            arrDatas.Add(alarm.SensorZoneID);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);

            // Debuggin을 위한 Break Point를 사용하면 아래의 로직으로 인하여 서버가 Block된다.
            // Timer를 사용하도록 한다.
            SendClientData(SOPWebServer.Header.SENSOR_SIGNAL_FOR_SOP, bytes, SOPWebServer.ClientType.SOP_SIMULATOR, -1);
            //SendMessageToClient(-1, -1, SOPWebServer.Header.SENSOR_SIGNAL_FOR_SOP, bytes, null);
        }

        public static bool GetTrainingMode(DirectDBManager dbMgr)
        {
            string szSQL = "SELECT PropertyValue FROM OptionSDMS where PropertyName ='TranningMode' and SiteID = " + dbMgr.SiteID;
            ArrayList arrResult = dbMgr.GetResultData(szSQL);

            if (arrResult == null || arrResult.Count == 0)
            {
                return false;
            }
            else
            {
                int value = WebDBManager.GetIntField(arrResult[0].ToString(), 0);

                if (value == 1)
                    return true;
            }

            return false;
        }

        public void SendClearAlarm(AlarmData alarm)
        {
            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(alarm.SensorZoneHistoryID);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);

            // Debuggin을 위한 Break Point를 사용하면 아래의 로직으로 인하여 서버가 Block된다.
            // Timer를 사용하도록 한다.
            SendClientData(SOPWebServer.Header.CLEAR_DETECT_REPORT, bytes, SOPWebServer.ClientType.SOP_SIMULATOR, -1);
            //SendMessageToClient(-1, -1, SOPWebServer.Header.CLEAR_DETECT_REPORT, bytes, null);
        }

        public void SendChangedConfig(int nConfigData)
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(SOPWebServer.ClientType.SDMS);
            arrDatas.Add(SOP.SDMSConfig.PropertyName);
            arrDatas.Add(nConfigData.ToString());

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            SendClientData(SOPWebServer.Header.CHANGE_CONFIG, bytes, SOPWebServer.ClientType.SOP_SIMULATOR, -1);
        }

        public void DeleteActionStepHistory(List<int> actionStepHistoryIDs, List<int> actionStepIDs)
        {
        }
    }
}
