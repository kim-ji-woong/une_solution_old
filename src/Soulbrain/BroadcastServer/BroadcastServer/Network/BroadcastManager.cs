using System;
using System.Threading;
using System.Collections;
using TcpLib2;

namespace BroadcastServer.Network
{
    public class BroadcastManager
    {
        public enum CommandType { CMD_PSM = 1, CMD_FIRE };
        public enum MaterialType { HF = 1, HCl, Co, Co2, Tvoc, O2 };
        public enum FireType { Fire = 1 };
        public enum AlarmLevel { ClearAlarm = 0, Level1 = 1, Level2, Level3, Level4 };

        private int m_nPort = 0;
        private TcpServer m_server = null;
        private ServiceProvider m_provider = null;
        //private bool m_shutdownThread = false;

        public BroadcastManager(IServiceOwner owner, int nPort)
        {
            m_nPort = nPort;
            m_provider = new ServiceProvider();
            m_provider.ServiceOwner = owner;
            
            try
            {
                m_server = new TcpServer(m_provider, m_nPort);
                m_server.Start();
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("Listen Error : " + e.Message);
            }
            //Thread t = new Thread(ConnectionThread);
            //t.Start();
        }

        public bool SendMessage(CommandType cmd, int param, AlarmLevel level)
        {
            byte command = (byte)cmd;
            byte[] bytes = new byte[2];

            bytes[0] = (byte)param;
            bytes[1] = (byte)level;

            if (m_provider.SendMessage(command, bytes))
            {
                string strLog = string.Format("SendMessage : {0:X2}, {1:X2}, {2:X2}", (int)command, (int)bytes[0], (int)bytes[1]);
                Logger.Instance.Write(strLog);
                return true;
            }

            return false;
            //return m_provider.SendMessage(command, bytes);
        }

        /*public void SendMessage(CommandType cmd, MaterialType material, AlarmLevel level)
        {
            ArrayList arrDatas = new ArrayList();
            arrDatas.Add((byte)cmd);
            arrDatas.Add((byte)material);
            arrDatas.Add((byte)level);

            Thread t = new Thread(new ParameterizedThreadStart(SendMessageThread));
            t.Start(arrDatas);
        }

        private void SendMessageThread(object arg)
        {
            ArrayList arrDatas = (ArrayList)arg;
            byte cmd = (byte)arrDatas[0];
            byte data1 = (byte)arrDatas[1];
            byte data2 = (byte)arrDatas[2];

            ServiceProvider provider = new ServiceProvider();

            if (provider.Connect(m_strIP, m_nPort) == false)
                return;

            byte[] bytes = new byte[2];
            bytes[0] = data1;
            bytes[1] = data2;

            provider.SendMessage(cmd, bytes);
        }

        public void Close()
        {
            m_shutdownThread = true;
        }*/
    }
}
