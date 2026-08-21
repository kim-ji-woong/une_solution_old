using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using HSMS;
using System.Threading;
using System.Collections;
using System.Data.SqlClient;

namespace HSMSServer2
{
    public class NetworkClient
    {
        public enum ObjectType
        {
            NONE = 0,
            VEHICLE,
            EQUIPMENT,
            ZONE,
            WORKER
        };

        private ClientProvider m_provider = null;
        private int m_nPort = -1;
        private string m_strServerAddr = "";
        private bool shutdownThread = false;

        private SafetyChecker m_safetyChecker = null;

        private object m_sendLock = new object();
        private object m_connectionLock = new object();

		private static NetworkClient m_manager = null;
		public static NetworkClient Instance
		{
			get 
			{
				if (m_manager == null)
					m_manager = new NetworkClient();
				return m_manager; 
			}
		}

        private void WriteLog(object str)
        {
            if (NetworkServer.Instance.ServiceProvider == null)
                return;

            if (NetworkServer.Instance.ServiceProvider.IsLogOpened)
                NetworkServer.Instance.ServiceProvider.WriteLog(str);
        }

		private void WriteLineLog(object str)
        {
            if (NetworkServer.Instance.ServiceProvider == null)
                return;

            if (NetworkServer.Instance.ServiceProvider.IsLogOpened)
                NetworkServer.Instance.ServiceProvider.WriteLineLog(str);
        }

        /*private void InitLog()
        {
            if (ConnectionLogEx.MakeInstance())
                m_bIsLogOpened = true;
            else
                m_bIsLogOpened = false;
        }

		private bool m_bIsLogOpened = false;
		public bool IsLogOpened
		{
			get { return m_bIsLogOpened; }
			set { m_bIsLogOpened = value; }
		}*/
	
        public void RecvLog(byte[] bytes, string strReceived)
        {
			/*if (!IsLogOpened)
                return;*/

            string strLog = string.Format("RecvMessage({0} {1}) : {2} from Sensor Server",
                bytes.Length, bytes.Length > 1 ? "bytes" : "byte", strReceived);

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

        public int Send(byte[] bytes, string strSend, ClientProvider provider)
        {
            int nResult = 0;

            lock (m_sendLock)
            {
                nResult = provider.Send(bytes, 0, bytes.Length);
            }

            if (nResult > 0)
			{
				/*if (!IsLogOpened)
					return nResult;*/

                string strLog = string.Format("SendMessage({0} {1}) : {2} to Sensor Server",
                    bytes.Length, bytes.Length > 1 ? "bytes" : "byte", strSend);

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

			return nResult;
        }

        protected NetworkClient()
        {
            m_safetyChecker = new SafetyChecker(NetworkServer.Instance.DataManager);

            m_strServerAddr = DBConn.GetInValue("SensorServer", "ip_addr");
            string strServerPort = DBConn.GetInValue("SensorServer", "port");

            int.TryParse(strServerPort, out m_nPort);
			
			m_provider = new ClientProvider(this);

            // 접속이 계속 유지될 수 있도록 한다.
            Thread t = new Thread(ConnectionThread);
            t.Start();
        }

        // dtTarget이 dtNow보다 1달 이전의 시간인가?
        private bool IsPassedTime(DateTime dtNow, int nYear, int nMonth, int nDay)
        {
            if (dtNow.Year - nYear > 1)
                return true;
            else if (dtNow.Year - nYear == 1)
            {
                if (dtNow.Month < 12)
                    return true;
                else if (nMonth > 1)
                    return true;
                else if (dtNow.Day < nDay)
                    return true;
                else
                    return false;
            }
            else if (dtNow.Year > nYear)
                return false;

            if (dtNow.Month - nMonth > 1)
                return true;
            else if (dtNow.Month >= nMonth)
                return false;

            return dtNow.Day < nDay;
        }

        // 서버와의 접속이 끊어지면 다시 연결시킨다.
        private void ConnectionThread()
        {
            while (!shutdownThread)
            {
                bool closeProvider = false;

                lock (m_connectionLock)
                {
                    if (!m_provider.IsConnected)
                    {
                        try
                        {
                            m_provider.Connect(m_strServerAddr, m_nPort);
                        }
                        catch (Exception)
                        {
                        }
                    }
                    else if (m_provider.PingCount > 0 && m_provider.PingCount % 2 == 1)
                        m_provider.SendPing();

                    if (m_provider.IsConnected)
                    {
                        FormMain.Instance.SetSensorServer(m_strServerAddr + ":" + m_nPort.ToString(), true);

                        if (m_provider.PingCount++ > 10)
                        {
                            m_provider.PingCount = 0;
                            //m_provider.Close();
                            closeProvider = true;
                        }
                    }
                    else
                        FormMain.Instance.SetSensorServer(m_strServerAddr + ":" + m_nPort.ToString(), false);
                }

                try
                {
                    if (closeProvider)
                        m_provider.Close();
                }
                catch (Exception)
                {
                }

                Thread.Sleep(500);
            }
        }

        public void ReleaseThread()
        {
            shutdownThread = true;
            m_safetyChecker.ReleaseThread();
        }

        public void OnDropConnection()
        {
            lock (m_connectionLock)
            {
                m_provider = new ClientProvider(this);			
            }
        }

        public void OnReceiveSensorData(string strSensorID, double x, double y, double methaneGas, double coGas)
        {
            OnReceiveSensorLocation(strSensorID, x, y);
            OnReceiveSensorGas(strSensorID, methaneGas, coGas);
        }

        public void OnReceiveSensorGas(string strSensorID, double methaneGas, double coGas)
        {
            NetworkServer.Instance.AlarmManager.CheckGasData(strSensorID, coGas, methaneGas);
        }

        public void OnReceiveSensorLocation(string strSensorID, double x, double y)
        {
            lock (m_safetyChecker)
            {
                m_safetyChecker.AddSensorHistory(strSensorID, new EventSensorData(strSensorID, DateTime.Now, x, y));
            }

            DataManager dataMgr = NetworkServer.Instance.DataManager;
            DataWorker worker = dataMgr.FindWorker2(strSensorID);

            if (worker != null)
            {
                ProcessSensorData(ObjectType.WORKER, worker.ID, strSensorID, x, y, worker.SensorDetect);
            }
            else
            {
                DataCar car = dataMgr.FindCar2(strSensorID);

                if (car != null)
                {
                    ProcessSensorData(ObjectType.VEHICLE, car.ID, strSensorID, x, y, car.SensorDetect);
                }
                else
                {
                    DataEquip equip = dataMgr.FindEquip2(strSensorID);

                    if (equip != null)
                    {
                        equip.Moved.x = x - (equip.SensorPosition.x + equip.OriginPosition.x);
                        equip.Moved.y = y - (equip.SensorPosition.y + equip.OriginPosition.y);

                        ProcessSensorData(ObjectType.EQUIPMENT, equip.ID, strSensorID, x, y, equip.SensorDetect);
                    }
                }
            }
        }

        private void ProcessSensorData(ObjectType type, int nObjectID, string strSensorID, double x, double y, bool sensorEnabled)
        {
            // SensorHistory 기록
            DBConn dbMgr = NetworkServer.Instance.DBManager;
            SqlConnection connection = dbMgr.Connect();

            DateTime dtNow = DateTime.Now;

            string strTime = string.Format("{0}-{1}-{2} {3}:{4}:{5}",
                dtNow.Year, dtNow.Month, dtNow.Day,
                dtNow.Hour, dtNow.Minute, dtNow.Second);

            string strSQL = string.Format("Insert into SensorHistory (SensorID, Time, X, Y, Description) values ('{0}', '{1}', {2}, {3}, NULL)",
                strSensorID, strTime, x, y);

            dbMgr.ExecuteSQL(strSQL, connection);
            connection.Close();
            ///////////////////////////////////////////////////

            if (sensorEnabled)
                SendSensorData(type, nObjectID, strSensorID, x, y);
        }

        private void SendSensorData(ObjectType type, int nObjectID, string strSensorID, double x, double y)
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add((int)type);
            arrDatas.Add(nObjectID);
            arrDatas.Add(strSensorID);
            arrDatas.Add(x);
            arrDatas.Add(y);

            byte[] bytes = ServiceProvider.MakeBytes(TCP_ID.SENSOR_DATA, arrDatas);

            NetworkServer.Instance.ServiceProvider.SendClientDataOnLoginUser(bytes, ClientData.ClientType.HSMS_CLIENT, false);
        }
    }
}
