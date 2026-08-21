using System;
using System.Threading;
using System.Collections;
using TcpLib2;
using System.Configuration;
using System.Collections.Generic;
using SDMS.Model.Alarm;

namespace BroadcastServer.Network
{
    public class BuildingConfigData
    {
        private List<int> m_IDs = new List<int>();
        private int m_nPort = 0;
        private string m_strName = "";
        private bool m_bRunBroadcast = false;
        private Dictionary<int, CurrentAlarm> m_dicAlarms = new Dictionary<int, CurrentAlarm>();

        /// <summary>
        /// 건물 ID 리스트
        /// </summary>
        public List<int> IDs
        {
            get { return m_IDs; }
            set { m_IDs = value; }
        }

        /// <summary>
        /// 건물에 Port
        /// </summary>
        public int Port
        {
            get { return m_nPort; }
            set { m_nPort = value; }
        }

        /// <summary>
        /// 건물 이름
        /// </summary>
        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        /// <summary>
        /// 해당 건물 방송 실행 여부
        /// </summary>
        public bool RunBroadcast
        {
            get { return m_bRunBroadcast; }
            set { m_bRunBroadcast = value; }
        }

        /// <summary>
        /// 해당 건물 알람 리스트
        /// </summary>
        public Dictionary<int, CurrentAlarm> Alarms
        {
            get { return m_dicAlarms; }
        }
    }

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

        private Dictionary<string, TcpServer> m_dicServers = null;
        private Dictionary<string, ServiceProvider> m_dicProviders = null;

        public Dictionary<string, ServiceProvider> DicProviders 
        {
            get { return m_dicProviders; }
        }

        public BroadcastManager(IServiceOwner owner, Dictionary<string, BuildingConfigData> dicBuilding)
        {
            if (dicBuilding == null || dicBuilding.Count == 0)
                return;

            try
            {
                m_dicServers = new Dictionary<string, TcpServer>();
                m_dicProviders = new Dictionary<string, ServiceProvider>();

                foreach (KeyValuePair<string, BuildingConfigData> pair in dicBuilding)
                {
                    BuildingConfigData data = pair.Value;

                    ServiceProvider provider = new ServiceProvider();
                    provider.ServiceOwner = owner;
                    provider.BuildingConfigData = data;

                    TcpServer server = new TcpServer(provider, data.Port);

                    m_dicProviders[data.Name] = provider;
                    m_dicServers[data.Name] = server;

                    server.Start();
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Write("Listen Error : " + e.Message);
                System.Diagnostics.Trace.WriteLine("Listen Error : " + e.Message);
            }
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

        public ServiceProvider GetBroadcastProvider(int nBuildingID)
        {
            foreach (KeyValuePair<string, ServiceProvider> pair in m_dicProviders)
            {
                ServiceProvider data = pair.Value;

                if (data.BuildingConfigData.IDs.Contains(nBuildingID))
                {
                    return data;
                }
            }

            return null;
        }

        public bool CheckAlarmBroadcast(CurrentAlarm alarm)
        {
            foreach (KeyValuePair<string, ServiceProvider> pair in m_dicProviders)
            {
                ServiceProvider data = pair.Value;

                if (data.BuildingConfigData.Alarms.ContainsKey(alarm.SensorZoneHistoryID))
                    return true;
            }

            return false;
        }

        public bool InitBroadcast()
        {
            if (m_dicProviders == null)
                return false;

            foreach (KeyValuePair<string, ServiceProvider> pair in m_dicProviders)
            {
                ServiceProvider provider = pair.Value;

                if (provider.SendBroadcast(false) == false)
                    return false;
            }

            return true;
        }
    }
}
