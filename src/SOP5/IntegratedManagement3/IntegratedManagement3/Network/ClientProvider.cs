using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using SDMS;
using System.Net.Sockets;
using System.Diagnostics;
using System.Collections;

namespace IntegratedManagement3
{
    public class ClientProvider : ClientServiceProvider
    {
        private NetworkManager m_mgr = null;
        
        // 현재 OnReceive()에서 받은 데이터를 처리중인가?
        private bool m_isReadingProcess = false;
        public bool IsReadingProcess
        {
            get { return m_isReadingProcess; }
        }

		private int m_nPingCount = 0;
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
			try
			{
                byte[] bytesRead = ReceivedData;

				if (bytesRead != null)
				{
					m_isReadingProcess = true;

                    int nBytesCount = bytesRead.Count();

					if (nBytesCount > 0)
					{
						m_nPingCount = 0;

                        if (!CheckValidation(bytesRead))
							goto RETURN;

                        m_mgr.RecvLog(bytesRead);

                        short nHeader;
                        ArrayList arrDatas = ReadBytes(bytesRead, out nHeader);

                        if (arrDatas == null)
                            return;

						//Debug.WriteLine(bytesRead[0].ToString());
                        if (nHeader == TCP_ID.ARE_YOU_THERE)
                        {
                            SendData(TCP_ID.I_AM_HERE);
                        }
                        else if (nHeader == TCP_ID.WHO_ARE_YOU)
                        {
                            SendWhoIam();
                        }
                        else if (nHeader == TCP_ID.ACCEPT_LOGIN)
                        {
                            int nReadData = 6;

                            // Read string
                            int nDataLength = BitConverter.ToInt32(bytesRead, ++nReadData);
                            nReadData += 4;
                            int nUserID = BitConverter.ToInt32(bytesRead, nReadData);
                            nReadData += 4;

                            // read string
                            nDataLength = BitConverter.ToInt32(bytesRead, ++nReadData);
                            nReadData += 4;
                            string szUserName = Encoding.UTF8.GetString(bytesRead, nReadData, nDataLength);
                            nReadData += nDataLength;

                            // read string
                            nDataLength = BitConverter.ToInt32(bytesRead, ++nReadData);
                            nReadData += 4;
                            string szNickName = Encoding.UTF8.GetString(bytesRead, nReadData, nDataLength);

                            LoginManager.Instance.OnAcceptLogin(nUserID, szUserName, szNickName);
                        }
                        else if (nHeader == TCP_ID.REJECT_LOGIN)
                        {
                            int nRejectType = BitConverter.ToInt32(bytesRead, 11);
                            LoginManager.Instance.OnRejectLogin(nRejectType);
                        }
                        else if (nHeader == TCP_ID.CHECK_LOGIN)
                        {
                            LoginManager.Instance.OnCheckLogin();
                        }
                        else if (nHeader == TCP_ID.LOGOUT_USER)
                        {
                            LoginManager.Instance.OnLogout();
                        }
                        else if (nHeader == TCP_ID.JOIN_USER)
                        {
                            int nGenUserID = BitConverter.ToInt32(bytesRead, 11);
                            LoginManager.Instance.OnJoinUser(nGenUserID);
                        }
                        else if(nHeader == TCP_ID.CHANGE_SOPGENUSER_COMMANDER)
                        {
                            int nResult = BitConverter.ToInt32(bytesRead, 11);
                            LoginManager.Instance.OnChangeSOPGenUserCommander(nResult);
                        }
                        else if (nHeader == TCP_ID.CHNAGE_PASSWORD || nHeader == TCP_ID.SET_PASSWORD)
                        {
                            int nSuccess = BitConverter.ToInt32(bytesRead, 11);
                            LoginManager.Instance.OnChangePassword(nSuccess);
                        }
                        else if (nHeader == TCP_ID.CHANGE_NICKNAME)
                        {
                            int nSuccess = BitConverter.ToInt32(bytesRead, 11);
                            int nReadData = 11 + 4;

                            // Read string
                            int nDataLength = BitConverter.ToInt32(bytesRead, ++nReadData);
                            nReadData += 4;
                            string szNickName = Encoding.UTF8.GetString(bytesRead, nReadData, nDataLength);

                            LoginManager.Instance.OnChangeNickName(nSuccess, szNickName);
                        }
                        else if (nHeader == TCP_ID.END_RESTORE)
                        {
                            LoginManager.Instance.OnEndRestore();

                        }
                        else if (nHeader == TCP_ID.SERVER_COMMAND)
                        {
                            ProcessServerCommand(arrDatas);
                        }
                        else if (nHeader == TCP_ID.INTERNAL_MESSAGE)
                        {
                            // SOP Server가 다른 곳에서 전송된 InternalMessage를 대신 전달해 주는 경우
                            ProcessInternalMessage(arrDatas, bytesRead);
                        }
					}
				}        
			}
			catch (System.Exception ex)
			{
				
			}
		RETURN:
			m_isReadingProcess = false;
        }

        private void ProcessInternalMessage(ArrayList arrDatas, byte[] bytes)
        {
            if (arrDatas.Count >= 1 && arrDatas[0] is byte)
            {
                byte msg = (byte)arrDatas[0];

                if (msg == InternalMessage.SDMS_2_SOP_SIMULATOR)
                    FormMain.Instance.NetworkServer.ServiceProvider.SendDataToOther(bytes, arrDatas, null, false, ClientData.ClientType.SOP_SIMULATOR);
                else if (msg == InternalMessage.SOP_SIMULATOR_2_SDMS)
                    FormMain.Instance.NetworkServer.ServiceProvider.SendDataToOther(bytes, arrDatas, null, false, ClientData.ClientType.SDMS_CLIENT);
            }
        }

        public static ArrayList ReadBytes(byte[] bytes, out short nHeader)
        {
            nHeader = 0;

            int nLength = bytes.Length;

            if (nLength < 6)
                return null;

            nHeader = BitConverter.ToInt16(bytes, 0);
            int nChunkCount = BitConverter.ToInt32(bytes, 2);

            ArrayList arrResult = new ArrayList();
            int nIndex = 6;
            bool isNullData;

            for (int i = 0; i < nChunkCount; i++)
            {
                if (nLength <= nIndex)
                    return null;

                byte type = bytes[nIndex];

                if (type == TCP_TYPE.INTEGER)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 9, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        int nData = BitConverter.ToInt32(bytes, nIndex - 4);
                        arrResult.Add(nData);
                    }
                }
                else if (type == TCP_TYPE.FLOAT)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 9, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        float fData = BitConverter.ToSingle(bytes, nIndex - 4);
                        arrResult.Add(fData);
                    }
                }
                else if (type == TCP_TYPE.DOUBLE)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 13, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        double dData = BitConverter.ToDouble(bytes, nIndex - 8);
                        arrResult.Add(dData);
                    }
                }
                else if (type == TCP_TYPE.LONG)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 13, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        long lData = BitConverter.ToInt64(bytes, nIndex - 8);
                        arrResult.Add(lData);
                    }
                }
                else if (type == TCP_TYPE.BOOLEAN)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 6, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        bool bData = BitConverter.ToBoolean(bytes, nIndex - 1);
                        arrResult.Add(bData);
                    }
                }
                else if (type == TCP_TYPE.SHORT)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 7, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        short sData = BitConverter.ToInt16(bytes, nIndex - 2);
                        arrResult.Add(sData);
                    }
                }
                else if (type == TCP_TYPE.BYTE)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 6, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        byte data = bytes[nIndex - 1];
                        arrResult.Add(data);
                    }
                }
                else if (type == TCP_TYPE.STRING)
                {
                    if (nLength < nIndex + 5)
                        return null;

                    int nDataLength = BitConverter.ToInt32(bytes, nIndex + 1);

                    if (nDataLength < 0)
                        return null;
                    else if (nDataLength > 0)
                    {
                        if (nLength < nIndex + 5 + nDataLength)
                            return null;

                        string strData = Encoding.UTF8.GetString(bytes, nIndex + 5, nDataLength);
                        arrResult.Add(strData);

                        nIndex += 5 + nDataLength;
                    }
                    else
                    {
                        arrResult.Add("");
                        nIndex += 5;
                    }
                }
                else
                    return null;
            }

            return arrResult;
        }

        private static bool ReadType(byte[] bytes, int nBytesLength, ref int nIndex, int nTotalLength, out bool isNullData)
        {
            isNullData = false;

            if (nBytesLength < nIndex + 5)
                return false;

            int nDataLength = BitConverter.ToInt32(bytes, nIndex + 1);

            if (nDataLength < 0)
                return false;
            else if (nDataLength > 0)
            {
                if (nBytesLength < nIndex + nTotalLength)
                    return false;

                nIndex += nTotalLength;
            }
            else
            {
                isNullData = true;
                nIndex += 5;
            }

            return true;
        }

        private void ProcessServerCommand(ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;

            if (nDataCount == 0)
                return;

            int nCommand = (int)(byte)arrDatas[0];

            if (nCommand == ServerCommandType.RUN_SDMS)
                FormMain.Instance.ExecuteManager.Run(ExecuteManager.APP_TYPE.SDMS);
            else if (nCommand == ServerCommandType.UPDATE_SYSTEM)
                FormMain.Instance.CheckNUpdateSystem(null, true);
        }

		public bool SendLogout(string szID)
		{
			if (this.IsConnected == false)
			{
				return false;
			}

			byte[] dataBytes = MakeBytes(szID);
			byte[] bytes = new byte[dataBytes.Length + 6];

			byte[] nHader = BitConverter.GetBytes((short)TCP_ID.LOGOUT_USER);
			byte[] nCount = BitConverter.GetBytes(1);

			// SET MESSAGE HeADER
			bytes[0] = nHader[0];
			bytes[1] = nHader[1];

			// SET DATA COUNT
			bytes[2] = nCount[0];
			bytes[3] = nCount[1];
			bytes[4] = nCount[2];
			bytes[5] = nCount[3];

			System.Buffer.BlockCopy(dataBytes, 0, bytes, 6, dataBytes.Length);

			m_mgr.Send(bytes, this);

			return true;
		}

		public bool SendCheckUser(string szID)
		{
			if (this.IsConnected == false)
			{
				return false;
			}

			byte[] dataBytes = MakeBytes(szID);
			byte[] bytes = new byte[dataBytes.Length + 6];

			byte[] nHader = BitConverter.GetBytes((short)TCP_ID.CHECK_LOGIN);
			byte[] nCount = BitConverter.GetBytes(1);

			// SET MESSAGE HeADER
			bytes[0] = nHader[0];
			bytes[1] = nHader[1];

			// SET DATA COUNT
			bytes[2] = nCount[0];
			bytes[3] = nCount[1];
			bytes[4] = nCount[2];
			bytes[5] = nCount[3];

			System.Buffer.BlockCopy(dataBytes, 0, bytes, 6, dataBytes.Length);

			m_mgr.Send(bytes, this);

			return true;
		}

		public bool SendLoginUser(string szID, string szPass)
		{
            System.Diagnostics.Trace.WriteLine("SendLoginUser");
			if (this.IsConnected == false)
			{
                System.Diagnostics.Trace.WriteLine("not connected");
				return false;
			}

			byte[] dataBytes = MakeBytes(szID);
			byte[] dataBytes2 = MakeBytes(szPass);
			
			byte[] bytes = new byte[dataBytes.Length + dataBytes2.Length + 6];

			byte[] nHader = BitConverter.GetBytes((short)TCP_ID.LOGIN_USER);
			byte[] nCount = BitConverter.GetBytes(2);

			// SET MESSAGE HeADER
			bytes[0] = nHader[0];
			bytes[1] = nHader[1];

			// SET DATA COUNT
			bytes[2] = nCount[0];
			bytes[3] = nCount[1];
			bytes[4] = nCount[2];
			bytes[5] = nCount[3];

			System.Buffer.BlockCopy(dataBytes, 0, bytes, 6, dataBytes.Length);
			System.Buffer.BlockCopy(dataBytes2, 0, bytes, 6 + dataBytes.Length, dataBytes2.Length);

            if (m_mgr.Send(bytes, this) < 0)
            {
                System.Diagnostics.Trace.WriteLine("SendLoginUser fail");
                return false;
            }

            System.Diagnostics.Trace.WriteLine("SendLoginUser success");
			return true;
		}

        public bool SendRegisterUser(int nMemberID, string szID, string szPass, string szNickName, IntegratedManagement3.PopupDialog.Chief chief)
		{
			if (this.IsConnected == false)
			{
				return false;
			}

			byte[] dataBytes = MakeBytes(nMemberID);
			byte[] dataBytes1 = MakeBytes(szID);
			byte[] dataBytes2 = MakeBytes(szPass);
            byte[] dataBytes3 = MakeBytes(szNickName);

            // SopGenUserCommander
            chief.CallerPhoneNumber = chief.CallerPhoneNumber.Replace("-","");
            byte[] dataBytes4 = MakeBytes(chief.DisplayText);
            byte[] dataBytes5 = MakeBytes(chief.CallerPhoneNumber);
            //byte[] dataBytes6 = MakeBytes((chief.DataTeam == null || !chief.DataTeam.External) ? 0 : 1);
            byte[] dataBytes6 = MakeBytes((int)chief.SOPTYPE);
            byte[] dataBytes7 = MakeBytes(chief.ID);

            Int32 iDayLight = 0;
            if (chief.DayLight_Day == true)
                iDayLight += 1;
            if (chief.DayLight_Night == true)
                iDayLight += 2;

            byte[] dataBytes8 = MakeBytes(iDayLight);   // 1 : 주간 , 2 : 야간 , 3: 주간&야간

            byte[] bytes = new byte[dataBytes.Length + dataBytes1.Length + dataBytes2.Length + dataBytes3.Length + dataBytes4.Length + dataBytes5.Length + dataBytes6.Length + dataBytes7.Length + dataBytes8.Length + 6];
 
			byte[] nHader = BitConverter.GetBytes((short)TCP_ID.JOIN_USER);
			byte[] nCount = BitConverter.GetBytes(9);

			// SET MESSAGE HeADER
			bytes[0] = nHader[0];
			bytes[1] = nHader[1];

			// SET DATA COUNT
			bytes[2] = nCount[0];
			bytes[3] = nCount[1];
			bytes[4] = nCount[2];
			bytes[5] = nCount[3];

			System.Buffer.BlockCopy(dataBytes, 0, bytes, 6, dataBytes.Length);
			System.Buffer.BlockCopy(dataBytes1, 0, bytes, 6 + dataBytes.Length, dataBytes1.Length);
			System.Buffer.BlockCopy(dataBytes2, 0, bytes, 6 + dataBytes.Length + dataBytes1.Length, dataBytes2.Length);
            System.Buffer.BlockCopy(dataBytes3, 0, bytes, 6 + dataBytes.Length + dataBytes1.Length + dataBytes2.Length, dataBytes3.Length);
            System.Buffer.BlockCopy(dataBytes4, 0, bytes, 6 + dataBytes.Length + dataBytes1.Length + dataBytes2.Length + dataBytes3.Length, dataBytes4.Length);
            System.Buffer.BlockCopy(dataBytes5, 0, bytes, 6 + dataBytes.Length + dataBytes1.Length + dataBytes2.Length + dataBytes3.Length + dataBytes4.Length, dataBytes5.Length);
            System.Buffer.BlockCopy(dataBytes6, 0, bytes, 6 + dataBytes.Length + dataBytes1.Length + dataBytes2.Length + dataBytes3.Length + dataBytes4.Length + dataBytes5.Length, dataBytes6.Length);
            System.Buffer.BlockCopy(dataBytes7, 0, bytes, 6 + dataBytes.Length + dataBytes1.Length + dataBytes2.Length + dataBytes3.Length + dataBytes4.Length + dataBytes5.Length + dataBytes6.Length, dataBytes7.Length);
            System.Buffer.BlockCopy(dataBytes8, 0, bytes, 6 + dataBytes.Length + dataBytes1.Length + dataBytes2.Length + dataBytes3.Length + dataBytes4.Length + dataBytes5.Length + dataBytes6.Length + dataBytes7.Length, dataBytes8.Length);

			m_mgr.Send(bytes, this);
			return true;
		}

		public bool SendChangePassword(int nGenUserID, string szPass, string szNewPass)
		{
			if (this.IsConnected == false)
			{
				return false;
			}
			byte[] dataBytes = MakeBytes(nGenUserID);
			byte[] dataBytes1 = MakeBytes(szPass);
			byte[] dataBytes2 = MakeBytes(szNewPass);

			byte[] bytes = new byte[dataBytes.Length + dataBytes1.Length + dataBytes2.Length + 6];

			byte[] nHader = BitConverter.GetBytes((short)TCP_ID.CHNAGE_PASSWORD);
			byte[] nCount = BitConverter.GetBytes(3);

			// SET MESSAGE HeADER
			bytes[0] = nHader[0];
			bytes[1] = nHader[1];

			// SET DATA COUNT
			bytes[2] = nCount[0];
			bytes[3] = nCount[1];
			bytes[4] = nCount[2];
			bytes[5] = nCount[3];

			System.Buffer.BlockCopy(dataBytes, 0, bytes, 6, dataBytes.Length);
			System.Buffer.BlockCopy(dataBytes1, 0, bytes, 6 + dataBytes.Length, dataBytes1.Length);
			System.Buffer.BlockCopy(dataBytes2, 0, bytes, 6 + dataBytes.Length + dataBytes1.Length, dataBytes2.Length);

			m_mgr.Send(bytes, this);
			return true;
		}

        public bool SendChangeNickName(int nGenUserID, string szNickName)
        {
            if (this.IsConnected == false)
            {
                return false;
            }
            byte[] dataBytes = MakeBytes(nGenUserID);
            byte[] dataBytes1 = MakeBytes(szNickName);

            byte[] bytes = new byte[dataBytes.Length + dataBytes1.Length + 6];

            byte[] nHader = BitConverter.GetBytes((short)TCP_ID.CHANGE_NICKNAME);
            byte[] nCount = BitConverter.GetBytes(2);

            // SET MESSAGE HeADER
            bytes[0] = nHader[0];
            bytes[1] = nHader[1];

            // SET DATA COUNT
            bytes[2] = nCount[0];
            bytes[3] = nCount[1];
            bytes[4] = nCount[2];
            bytes[5] = nCount[3];

            System.Buffer.BlockCopy(dataBytes, 0, bytes, 6, dataBytes.Length);
            System.Buffer.BlockCopy(dataBytes1, 0, bytes, 6 + dataBytes.Length, dataBytes1.Length);
            
            m_mgr.Send(bytes, this);
            return true;
        }

        public bool SendChangeSOPGenCommander(int szID,IntegratedManagement3.PopupDialog.Chief pchief)
        {
            if (this.IsConnected == false)
            {
                return false;
            }

            Int32 iDayLight = 0;
            if (pchief.DayLight_Day == true)
                iDayLight += 1;
            if (pchief.DayLight_Night == true)
                iDayLight += 2;
           
            pchief.CallerPhoneNumber = pchief.CallerPhoneNumber.Replace("-","");

            byte[] dataBytes = MakeBytes(szID);//SOPGenUser ID
            byte[] dataBytes1 = MakeBytes(pchief.DisplayText);
            byte[] dataBytes2 = MakeBytes(pchief.CallerPhoneNumber);
            byte[] dataBytes3 = MakeBytes((int)pchief.SOPTYPE);
            byte[] dataBytes4 = MakeBytes(pchief.ID);
            byte[] dataBytes5 = MakeBytes(iDayLight);   // 1 : 주간 , 2 : 야간 , 3: 주간&야간

            byte[] bytes = new byte[dataBytes.Length + dataBytes1.Length + dataBytes2.Length + dataBytes3.Length + dataBytes4.Length + dataBytes5.Length + 6];

            byte[] nHader = BitConverter.GetBytes((short)TCP_ID.CHANGE_SOPGENUSER_COMMANDER);
            byte[] nCount = BitConverter.GetBytes(6);

            // SET MESSAGE HeADER
            bytes[0] = nHader[0];
            bytes[1] = nHader[1];

            // SET DATA COUNT
            bytes[2] = nCount[0];
            bytes[3] = nCount[1];
            bytes[4] = nCount[2];
            bytes[5] = nCount[3];

            System.Buffer.BlockCopy(dataBytes, 0, bytes, 6, dataBytes.Length);
            System.Buffer.BlockCopy(dataBytes1, 0, bytes, 6 + dataBytes.Length, dataBytes1.Length);
            System.Buffer.BlockCopy(dataBytes2, 0, bytes, 6 + dataBytes.Length + dataBytes1.Length, dataBytes2.Length);
            System.Buffer.BlockCopy(dataBytes3, 0, bytes, 6 + dataBytes.Length + dataBytes1.Length + dataBytes2.Length, dataBytes3.Length);
            System.Buffer.BlockCopy(dataBytes4, 0, bytes, 6 + dataBytes.Length + dataBytes1.Length + dataBytes2.Length + dataBytes3.Length, dataBytes4.Length);
            System.Buffer.BlockCopy(dataBytes5, 0, bytes, 6 + dataBytes.Length + dataBytes1.Length + dataBytes2.Length + dataBytes3.Length + dataBytes4.Length, dataBytes5.Length);
            
            m_mgr.Send(bytes, this);
            return true;
        }

		public bool SendSetPassword(string szGenUserID , string szNewPass)
		{
			if (this.IsConnected == false)
			{
				return false;
			}
			byte[] dataBytes = MakeBytes(szGenUserID);
			byte[] dataBytes2 = MakeBytes(szNewPass);

			byte[] bytes = new byte[dataBytes.Length + dataBytes2.Length + 6];

			byte[] nHader = BitConverter.GetBytes((short)TCP_ID.SET_PASSWORD);
			byte[] nCount = BitConverter.GetBytes(2);            

			// SET MESSAGE HeADER
			bytes[0] = nHader[0];
			bytes[1] = nHader[1];

			// SET DATA COUNT
			bytes[2] = nCount[0];
			bytes[3] = nCount[1];
			bytes[4] = nCount[2];
			bytes[5] = nCount[3];

			System.Buffer.BlockCopy(dataBytes, 0, bytes, 6, dataBytes.Length);
			System.Buffer.BlockCopy(dataBytes2, 0, bytes, 6 + dataBytes.Length, dataBytes2.Length);

			m_mgr.Send(bytes, this);
			return true;
		}

        private void SendWhoIam()
        {
            byte[] bytes = new byte[15];
            byte[] dataBytes = MakeBytes((int)TCP_CLIENT.INTEGRATE_MANAGE);

			byte[] nHader = BitConverter.GetBytes((short)TCP_ID.WHO_I_AM);
			byte[] nCount = BitConverter.GetBytes(1);

			// SET MESSAGE HeADER
			bytes[0] = nHader[0];
			bytes[1] = nHader[1];

			// SET DATA COUNT
			bytes[2] = nCount[0];
			bytes[3] = nCount[1];
			bytes[4] = nCount[2];
			bytes[5] = nCount[3];

            System.Buffer.BlockCopy(dataBytes, 0, bytes, 6, dataBytes.Length);

            m_mgr.Send(bytes, this);
            FormMain.Instance.OnMakeItself();
        }

        private bool CheckValidation(byte[] bytes)
        {
            int length = bytes.Length;
            if (length < 6)
                return false;

            int nChunkCount = (int)bytes[1];
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

			if (this.Client.Client.Connected == true)
				m_mgr.Send(bytes, this);
		}

        public void SendSDMSInternalMessage(byte[] bytes)
        {
            if (this.Client.Client.Connected == true)
                m_mgr.Send(bytes, this);
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

        public override void OnDropConnection()
        {
            this.m_nPingCount = 0;
            
            m_mgr.OnDropConnection();
        }
    }
}
