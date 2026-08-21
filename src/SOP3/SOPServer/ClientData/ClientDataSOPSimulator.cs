using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using System.Collections;
using SDMS;
using ControlMonitoring;
using SOP;

namespace SDMSServer
{
    public class ClientDataSOPSimulator : ClientData, ControlMonitoring.IControlClientOwner
    {
        private ControlClient m_clientControl = new ControlClient();

        public ControlMonitoring.ControlClient GetControlClient()
        {
            return m_clientControl;
        }

        public ClientDataSOPSimulator(ServiceProvider provider)
        {
            m_provider = provider;
            Type = ClientType.SOP_SIMULATOR;

            m_clientControl.Owner = this;
        }

        // OnAccept() 이후 WhoIAm을 받은 뒤 처리해야 할 로직
        protected override bool ProcessFirstConnection(ConnectionState state)
        {
            // 제어권 처리
            ProcessControl(ReceivedData, state);

            SendCurrentFireSensorSignal(state);
            return true;
        }

        private bool ReadClientInfo(byte[] bytes, ConnectionState state, out bool hasControl)
        {
            int nUserID, nUserLevel;
            string strUserName;
            hasControl = false;

            int nIndex = 0;

            if (!GetChunkDatai(bytes, ref nIndex, out nUserID))
                return false;

            if (!GetChunkDatai(bytes, ref nIndex, out nUserLevel))
                return false;

            if (!GetChunkDatas(bytes, ref nIndex, out strUserName))
                return false;

            if (!GetChunkDatab(bytes, ref nIndex, out hasControl))
                return false;

            string strIP = ((System.Net.IPEndPoint)state.RemoteEndPoint).Address.ToString();

            m_clientControl.UserID = nUserID;
            m_clientControl.UserLevel = nUserLevel;
            m_clientControl.UserName = strUserName;
            m_clientControl.IP = strIP;
			hasControl = true;
            return true;
        }

        private void RegisterControl(ControlManager conMgr)
        {
            IControlClientOwner client = conMgr.FindClient(m_clientControl.UserID);

            if (client == null)
            {
                conMgr.AddClient(this);
            }
            else
            {
                if (client != this)
                {
                    conMgr.RemoveClient(client, true);
                    conMgr.AddClient(client);
                }
            }
        }

        // 새로 접속한 SOP Simulator에 대한 제어권 처리
        private bool ProcessControl(byte[] bytes, ConnectionState state)
        {
            bool hasControl;
            if (!ReadClientInfo(bytes, state, out hasControl))
            {
                DateTime dtNow = DateTime.Now;
                string strTime = string.Format("{0}:{1}", dtNow.Minute, dtNow.Second);
                return false;
            }

            ControlManager conMgr = ControlManager.Instance;

            // conMgr에 신규 등록
            RegisterControl(conMgr);

            
            // 새로 접속한 Client가 이미 제어권이 있는 상태라면
            if (hasControl)
            {
                IControlClientOwner client2 = conMgr.FindClient(m_clientControl.UserID);
                
                if (client2.GetControlClient().UserLevel > 0)
                {
                    if (conMgr.ControlClient == this)
                    {
                        //return true;
                        conMgr.ControlClient = this;
                        return SendGiveControl(state);
                    }
                    else if (conMgr.ControlClient == null)
                    {
                        conMgr.ControlClient = this;
                        return SendGiveControl(state);
                    }
                    else
                    {
                        // 제어권 회수
                        return SendTakeControl(state);
                    }
                }
            }
            else
            {
                if (conMgr.ControlClient == null)
                {
                    IControlClientOwner client = conMgr.FindClient(m_clientControl.UserID);
                
                    if (client.GetControlClient().UserLevel > 0)
                    {
                        // 제어권 부여
                        conMgr.ControlClient = this;
                        return SendGiveControl(state);
                    }
                }
            }

            return true;
        }

        // 제어권 회수
        private bool SendTakeControl(ConnectionState state)
        {
			try
			{
				byte[] bytes = new byte[6];

				bytes[0] = TCP_ID.TAKE_CONTROL;

				for (int i = 1; i < 6; i++)
					bytes[i] = 0;

                return m_provider.Send(bytes, 0, 6, state);
               
				
			}
			catch (System.Exception)
			{				
			}
			return false;
        }

        // 제어권 부여
        private bool SendGiveControl(ConnectionState state)
        {
			try
			{
				byte[] bytes = new byte[6];

				bytes[0] = TCP_ID.GIVE_CONTROL;

				for (int i = 1; i < 6; i++)
					bytes[i] = 0;

				return m_provider.Send(bytes, 0, 6, state);
			}
			catch (System.Exception)
			{
				
			}
			return false;
        }

        private void ProcessConfirmGiveControl()
        {
            ControlManager.Instance.ControlClient = this;
            ControlManager.Instance.ControlSOPGenUserID = -1;
            m_clientControl.Control_Type = ControlClient.ControlType.WANT_CONTROL;
        }

        private void ProcessConfirmTakeControl()
        {
            ControlManager conMgr = ControlManager.Instance;

            if (conMgr.ReservedNextControlClient != null)
            {
                conMgr.ControlClient = null;

                ClientDataSOPSimulator client = (ClientDataSOPSimulator)conMgr.ReservedNextControlClient;
                SendGiveControl(client.m_state);

                conMgr.ReservedNextControlClient = null;
            }
            else
            {
                if (conMgr.ControlClient == this)
                    conMgr.ControlClient = null;
            }            
        }

        private bool GetChunkDatai(byte[] bytes, ref int nIndex, out int nData)
        {
            nData = 0;

            if (bytes.Length < nIndex + 9)
                return false;

            if (bytes[nIndex] != TCP_TYPE.INTEGER)
                return false;

            int nDataLength = BitConverter.ToInt32(bytes, nIndex + 1);

            if (nDataLength != 4)
                return false;

            nData = BitConverter.ToInt32(bytes, nIndex + 5);
            nIndex += 9;

            return true;
        }

        private bool GetChunkDatab(byte[] bytes, ref int nIndex, out bool bData)
        {
            bData = false;

            if (bytes.Length < nIndex + 6)
                return false;

            if (bytes[nIndex] != TCP_TYPE.BOOLEAN)
                return false;

            int nDataLength = BitConverter.ToInt32(bytes, nIndex + 1);

            if (nDataLength != 1)
                return false;

            bData = BitConverter.ToBoolean(bytes, nIndex + 5);
            nIndex += 6;

            return true;
        }

        private bool GetChunkDatas(byte[] bytes, ref int nIndex, out string strData)
        {
            strData = "";

            if (bytes.Length < nIndex + 5)
                return false;

            if (bytes[nIndex] != TCP_TYPE.STRING)
                return false;

            int nDataLength = BitConverter.ToInt32(bytes, nIndex + 1);

            if (nDataLength < 0)
                return false;

            if (nDataLength == 0)
                return true;

            if (bytes.Length < nIndex + 5 + nDataLength)
                return false;

            byte[] bytesBlock = new byte[nDataLength];
            System.Buffer.BlockCopy(bytes, nIndex + 5, bytesBlock, 0, nDataLength);
            strData = Encoding.UTF8.GetString(bytesBlock, 0, nDataLength);

            nIndex += 5 + nDataLength;

            return true;
        }

        private void SendCurrentFireSensorSignal(ConnectionState state)
        {
            int nHistoryCount = m_provider.GetTimeHistoryCount();

            for (int i = 0; i < nHistoryCount;i++ )
            {
                TimeHistory history = m_provider.GetTimeHistory(i);

                if (history.LastReactionLog == null)
                    continue;

                if (history.LastReactionLog.Type != SensorReactionLog.ReactionType.NOTIFY_FIRE)
                    //&& history.LastReactionLog.Type != SensorReactionLog.ReactionType.NOTIFY_FIRE)
                    continue;

                m_provider.SendFireSensorSignal(history.LastReactionLog, state);
                break;
            }
        }

        // bytes는 length byte가 제거되었음
        protected override bool OnReceive(ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas)
        {
            if (nHeader == TCP_ID.RUN_SOP)
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
            else if (nHeader == TCP_ID.CHANGE_CONFIG)
            {
                ProcessChangedConfig(arrDatas, bytes);
            }

            return true;
        }

        private void ProcessChangedConfig(ArrayList arrDatas, byte[] bytes)
        {
            if (arrDatas == null)
                return;

            if (arrDatas.Count < 3)
                return;

            try
            {
                byte byteClientType = (byte)arrDatas[0];
                
                if (byteClientType != TCP_CLIENT.SOP_SIMULATOR)
                    return;

                m_provider.SendDataToOther(bytes, this, false, ClientType.SOP_SIMULATOR);
            }
            catch (Exception e)
            {
                ConnectionLogEx.Instance.WriteLine(e.StackTrace);
            }
        }

        // 제어권을 넘겨받는다.
        public void SetControl()
        {
            SendGiveControl(m_state);
        }

        // 제어권 반납
        private void ProcessReturnControl()
        {
            IControlClientOwner nextOwner = ControlManager.Instance.GetNextControlOwner(this);

            if (nextOwner == null)
            {
                // 제어권을 건네받을 Client가 존재하지 않으므로 제어권 반납을 무시하고,
                // 제어권을 다시 넘겨준다.
                SendGiveControl(m_state);
            }
            else
            {
                ControlManager.Instance.ReservedNextControlClient = nextOwner;
                SendTakeControl(m_state);

                // 제어권을 반납했으니 제어권을 원치않는 것으로 설정한다.
                //m_clientControl.Control_Type = ControlClient.ControlType.NOT_WANT_CONTROL;
            }
        }

        // 제어권 강제로 뺏기
        // 현재 제어권을 가진 Client로부터 제어권을 빼앗아 this 객체에 전달한다.
        private void ProcessStealControl()
        {
            if (ControlManager.Instance.ControlClient == null)
                SendGiveControl(m_state);
            else if (ControlManager.Instance.ControlClient == this)
                return;
            else
            {
                ControlManager.Instance.ReservedNextControlClient = this;

                ClientDataSOPSimulator client = (ClientDataSOPSimulator)ControlManager.Instance.ControlClient;
                SendTakeControl(client.m_state);
            }
        }

        private void ProcessRejectRequestControl(byte[] bytes)
        {
            int nIndex = 6;
            if (bytes.Length < nIndex)
                return;

            int nChunkCount = BitConverter.ToInt32(bytes, 2);

            if (nChunkCount < 3)
                return;

            string strUserID;
            string strUserName, strIP;

            if (!GetChunkDatas(bytes, ref nIndex, out strUserID))
                return;

            if (!GetChunkDatas(bytes, ref nIndex, out strUserName))
                return;

            if (!GetChunkDatas(bytes, ref nIndex, out strIP))
                return;

            LoginInfo info = LoginManager.Instance.FindLoginUser(strUserID);
            ClientDataSOPSimulator client = null;

            if (info == null)
                client = (ClientDataSOPSimulator)ControlManager.Instance.FindClient(strUserName);
            else
                client = (ClientDataSOPSimulator)ControlManager.Instance.FindClient(info.ID);

            if (client == null)
                return;

            ControlManager.Instance.ControlClient = null;
            SendRejectRequestControl(client.ConnectionState);
        }

		private void ProcessSelectMission(byte[] bytes)
		{
			this.m_provider.SendSelectMission(bytes);
		}

        private void SendRejectRequestControl(ConnectionState state)
        {
            byte[] bytes = new byte[6];

            bytes[0] = TCP_ID.REJECT_REQUEST_CONTROL;

            for (int i = 1; i < 6; i++)
                bytes[i] = 0;

            m_provider.Send(bytes, 0, 6, state);
        }

        private void ProcessGiveControl(byte[] bytes)
        {
            int nIndex = 6;
            if (bytes.Length < nIndex)
                return;

            int nChunkCount = BitConverter.ToInt32(bytes, 2);

            if (nChunkCount < 3)
                return;

            string strUserID;
            string strUserName, strIP;

            if (!GetChunkDatas(bytes, ref nIndex, out strUserID))
                return;

            if (!GetChunkDatas(bytes, ref nIndex, out strUserName))
                return;

            if (!GetChunkDatas(bytes, ref nIndex, out strIP))
                return;

            LoginInfo info = LoginManager.Instance.FindLoginUser(strUserID);
            ClientDataSOPSimulator client = null;

            if (info == null)
                client = (ClientDataSOPSimulator)ControlManager.Instance.FindClient(strUserName);
            else
                client = (ClientDataSOPSimulator)ControlManager.Instance.FindClient(info.ID);

            if (client == null)
                return;

            ControlManager.Instance.ControlClient = null;
            SendGiveControl(client.ConnectionState);
        }

        private void ProcessRequestControl()
        {
            ControlManager conMgr = ControlManager.Instance;

            if (conMgr.ControlClient == null || conMgr.ControlClient == this)
            {
                SendGiveControl(m_state);
            }
            else if (conMgr.ControlClient != this)
            {
                // this가 현재 제어권을 가진 Client보다 계정 등급이 높으면 그냥 뺏어온다.
                if (this.m_clientControl.UserLevel > conMgr.ControlClient.GetControlClient().UserLevel)
                {
                    conMgr.ReservedNextControlClient = this;

                    ClientDataSOPSimulator client = (ClientDataSOPSimulator)conMgr.ControlClient;
                    SendTakeControl(client.m_state);
                }
                else
                {
                    ClientDataSOPSimulator client = (ClientDataSOPSimulator)conMgr.ControlClient;
                    SendRequestControl(client.m_state, this.GetControlClient());
                }
            }
        }

        private void SendRequestControl(ConnectionState state, ControlClient data)
        {
            string strUserID = "", strUserNickName = "";
            LoginInfo info = LoginManager.Instance.FindLoginUser(data.UserID, data.UserName);

            if (info != null)
            {
                strUserID = info.SOPGenUserID;
                strUserNickName = info.NickName;
            }
            else
            {
                strUserID = "등록되지 않은 ID";
                strUserNickName = "등록되지 않은 사용자";
            }

            byte[] userIDBytes = ServiceProvider.MakeBytes(strUserID);
            byte[] userNameBytes = ServiceProvider.MakeBytes(data.UserName);
            byte[] userNickNameBytes = ServiceProvider.MakeBytes(strUserNickName);

            string strIP = ((System.Net.IPEndPoint)state.RemoteEndPoint).Address.ToString();
            byte[] ipBytes = ServiceProvider.MakeBytes(strIP);

            int nChunkCount = 4;
            byte[] chunkCountBytes = BitConverter.GetBytes(nChunkCount);

            int nLen = chunkCountBytes.Length + userIDBytes.Length + userNameBytes.Length + userNickNameBytes.Length + ipBytes.Length + 2;

            byte[] bytes = new byte[nLen];

            bytes[0] = TCP_ID.REQUEST_CONTROL;
            bytes[1] = 0;

            int nIndex = 2;

            SensorReactionLog.CopyBytes(bytes, ref nIndex, chunkCountBytes);
            SensorReactionLog.CopyBytes(bytes, ref nIndex, userIDBytes);
            SensorReactionLog.CopyBytes(bytes, ref nIndex, userNameBytes);
            SensorReactionLog.CopyBytes(bytes, ref nIndex, userNickNameBytes);
            SensorReactionLog.CopyBytes(bytes, ref nIndex, ipBytes);

 
            m_provider.Send(bytes, 0, nLen, state);
        }

        private void ProcessIgnoreSOP(byte[] bytes)
        {
            int nSensorHistoryID = BitConverter.ToInt32(bytes, 11);

            SensorReactionLog log = WriteIgnoreSOP(nSensorHistoryID);
            m_provider.ProcessIgnoreSOP(log, nSensorHistoryID, bytes);
        }

        private SensorReactionLog WriteIgnoreSOP(int nSensorHistoryID)
        {
            DBUtility.WebDBManager dbMgr = NetworkServer.Instance.DBManager;

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

            m_provider.CheckTranningMode(log);

            if (dbMgr.GetResultData(strSQL, 0) != null)
                return log;

            return null;
        }

        private void ProcessRunSOP(byte[] bytes)
        {
            int nSensorHistoryID = BitConverter.ToInt32(bytes, 11);

            int nDataLength = BitConverter.ToInt32(bytes, 16);
            string strActionStepHistoryID = Encoding.UTF8.GetString(bytes, 20, nDataLength);

            SensorReactionLog log = new SensorReactionLog();

            log.SensorHistoryID = nSensorHistoryID;
            log.Param1 = strActionStepHistoryID;

            m_provider.ProcessRunSOP(log);
        }

        protected override bool ProcessIAmHere(ArrayList arrDatas)
        {
            if (arrDatas.Count < 1)
                return false;

            bool hasControl = (bool)arrDatas[0];

            if (hasControl)
            {
                if (ControlManager.Instance.ControlClient != this)
                {
                    SendTakeControl(this.ConnectionState);
                }
            }
            else
            {
                if (ControlManager.Instance.ControlClient == this)
                {
                    SendGiveControl(this.ConnectionState);
                }
            }

            return true;
        }
    }
}
