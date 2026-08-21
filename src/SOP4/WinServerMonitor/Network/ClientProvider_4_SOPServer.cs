using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using System.Collections;
using SDMS;
using System.Threading;

namespace ServerMonitor
{
    public class ClientProvider_4_SOPServer : ClientServiceProvider
    {
        private NetworkManager_4_SOPServer m_mgr = null;

        public ClientProvider_4_SOPServer(NetworkManager_4_SOPServer netMgr)
        {
            m_mgr = netMgr;
        }

        public override void OnReceiveData()
        {
            short nHeader;
            ArrayList arrDatas = ClientProvider.ReadBytes(ReceivedData, out nHeader);

            if (arrDatas != null)
            {
                if (nHeader == TCP_ID.WHO_ARE_YOU)
                {
                    Thread t = new Thread(ProcessWhoAreYou);
                    t.Start();
                }
                else if (nHeader == TCP_ID.SERVER_COMMAND)
                    ProcessServerCommand(arrDatas);
            }
        }

        private void ProcessServerCommand(ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;

            if (nDataCount <= 0)
                return;

            int nCommand = (int)(byte)arrDatas[0];

            if (nCommand == (int)ServerCommandType.UPDATE_SYSTEM)
            {
                FormMain.Instance.EnableUpdateButton();
                Close();
                OnDropConnection();
            }
        }

        private void ProcessWhoAreYou()
        {
            SendWhoIAm();
            
            // Server가 ClientData를 생성할 시간을 준다.
            Thread.Sleep(1000);

            SendUpdateSystem();
        }

        private void SendUpdateSystem()
        {
            byte[] bytes = new byte[12];
            byte[] dataBytes = ClientProvider.MakeBytes(ServerCommandType.UPDATE_SYSTEM);

            byte[] bytesHeader = BitConverter.GetBytes((short)TCP_ID.SERVER_COMMAND);
            byte[] nCount = BitConverter.GetBytes(1);

            // SET MESSAGE Header
            bytes[0] = bytesHeader[0];
            bytes[1] = bytesHeader[1];

            // SET DATA COUNT
            bytes[2] = nCount[0];
            bytes[3] = nCount[1];
            bytes[4] = nCount[2];
            bytes[5] = nCount[3];

            System.Buffer.BlockCopy(dataBytes, 0, bytes, 6, dataBytes.Length);

            if (this.Client.Client.Connected == true)
                Send(bytes, 0, bytes.Length);
        }

        public void SendWhoIAm()
        {
            byte[] bytes = new byte[15];
            byte[] dataBytes = ClientProvider.MakeBytes((int)TCP_CLIENT.SERVER_COMMANDER);

            byte[] bytesHeader = BitConverter.GetBytes((short)TCP_ID.WHO_I_AM);
            byte[] nCount = BitConverter.GetBytes(1);

            // SET MESSAGE Header
            bytes[0] = bytesHeader[0];
            bytes[1] = bytesHeader[1];

            // SET DATA COUNT
            bytes[2] = nCount[0];
            bytes[3] = nCount[1];
            bytes[4] = nCount[2];
            bytes[5] = nCount[3];

            System.Buffer.BlockCopy(dataBytes, 0, bytes, 6, dataBytes.Length);

            if (this.Client.Client.Connected == true)
                Send(bytes, 0, bytes.Length);
        }

        public override void OnDropConnection()
        {
            m_mgr.OnDropConnection();
        }
    }
}
