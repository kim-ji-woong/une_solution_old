using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using TcpLib2;
using System.Collections;
using System.Windows.Forms;
using System.Data.SqlClient;
using HSMS;

namespace HSMSServer2
{
    public class NetworkServer
    {
        private LogManager m_logMgr = null;
		private System.Windows.Forms.DataGridView dataGridView1;

		private System.Windows.Forms.DataGridViewTextBoxColumn colIndex;
		private System.Windows.Forms.DataGridViewTextBoxColumn colIP;
		private System.Windows.Forms.DataGridViewTextBoxColumn colType;

        public System.Windows.Forms.DataGridView DataGridView1
        {
            get { return dataGridView1; }
        }

		private bool m_bCloseServer = false;
		public bool ClosingServer
		{
			get { return m_bCloseServer; }
		}
        private TcpServer m_server = null;
        private ServiceProvider m_provider = null;//new ServiceProvider();
        private int m_nPort = 0;
        private bool m_isOpened = false;
        private static NetworkServer m_instance = null;

        private bool m_finishProcess = false;

        // 몇일 이전의 로그는 무시할 것인가?
        private int m_nIgnorLogDay = 7;

        public int PortNo
        {
            get { return m_nPort; }
            set { m_nPort = value; }
        }

        // Delegate 호출을 위한 Form
        private Form m_frmDelegate = null;
        public Form FormDelegate
        {
            get { return m_frmDelegate; }
            set { m_frmDelegate = value; }
        }

        private DataManager m_dataMgr = null;
        public DataManager DataManager
        {
            get { return m_dataMgr; }
        }

        // DataGrid에 Client Type을 갱신하기 위한 변수
        // 동기화 문제를 피하기 위하여 Dictionary 사용
        private Dictionary<TcpLib2.ConnectionState, DataGridViewTextBoxCell> m_dicClientType = new Dictionary<TcpLib2.ConnectionState, DataGridViewTextBoxCell>();

        public bool FinishProcess
        {
            get { return m_finishProcess; }
        }

        public static NetworkServer Instance
        {
            get { return m_instance; }
        }

        public HSMSServer2.ServiceProvider ServiceProvider
        {
            get { return m_provider; }
        }

        private DBConn m_dbMgr = null;
        public DBConn DBManager
        {
            get { return m_dbMgr; }
        }

        private int m_nSiteID = 1;
        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        private AlarmManager m_alarmMgr = null;
        public AlarmManager AlarmManager
        {
            get { return m_alarmMgr; }
        }

        // Team이나 직원정보, 담당자 정보를 바꾸거나 조회하는 중인가?
        private object m_memberCriticalSection = new object();
        public object MemberCriticalSection
        {
            get { return m_memberCriticalSection; }
        }

        // Value : HSMS 클라이언트들의 로그인 계정 ID
        private Dictionary<TcpLib2.ConnectionState, string> m_dicLoginUserID = new Dictionary<TcpLib2.ConnectionState, string>();

        public void SetLoginUserID(TcpLib2.ConnectionState state, string strLoginUserID)
        {
            m_dicLoginUserID[state] = strLoginUserID;
        }

        public string GetLoginUserID(TcpLib2.ConnectionState state)
        {
            if (m_dicLoginUserID.ContainsKey(state))
                return m_dicLoginUserID[state];

            return null;
        }

		public void LoadBaseData()
		{
		}

		public NetworkServer(DataGridView dataGridView)
        {
            m_instance = this;
            
            m_dbMgr = new DBConn("HSMS");
            m_alarmMgr = new AlarmManager();
            m_dataMgr = new DataManager(m_dbMgr);

            ERPDataChecker checker = ERPDataChecker.Instance;
            checker.BeginCheck();
            //m_dataMgr.ReadDBData();

            m_alarmMgr.LoadDB();

			dataGridView1 = dataGridView == null ? new System.Windows.Forms.DataGridView() : dataGridView;

            if (dataGridView == null)
            {
                colIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
                colIP = new System.Windows.Forms.DataGridViewTextBoxColumn();
                colType = new System.Windows.Forms.DataGridViewTextBoxColumn();

                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.AllowUserToDeleteRows = false;
                dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                    colIndex,
                    colIP,
                    colType});
                dataGridView1.Name = "dataGridView1";
                dataGridView1.RowHeadersVisible = false;
                dataGridView1.RowTemplate.Height = 23;
                dataGridView1.TabIndex = 3;
            }
			
			LoadBaseData();

            SqlConnection connection = m_dbMgr.Connect();
            SqlDataReader reader = m_dbMgr.ExecuteReader("Select Port from HSMSServerPort", connection);

            if (reader.Read())
            {
                m_nPort = (int)reader[0];
            }

            reader.Close();
            connection.Close();

			if (m_nPort == 0)
			{
                m_nPort = 20000;
			}
            
            m_logMgr = new LogManager(m_dbMgr);

            //StartServer(m_nPort);
            /*m_provider = new ServiceProvider();
            m_server = new TcpServer(m_provider, m_nPort);
            m_isOpened = m_server.Start();*/
        }

        // dicSensorIDs : SensorID, SensorOwner
        private Dictionary<int, DangerState> ReadLastLog(out Dictionary<string, object> dicSensorIDs)
        {
            dicSensorIDs = new Dictionary<string, object>();

            // Server가 꺼져있는 동안 발생했던 이벤트를 읽어온다.
            // Alarm이 발생한 후(1) 종료되지(3) 않은 것들만 얻어온다.
            // m_nIgnoreLogDay 이전의 로그는 무시한다.
            string strSQL = "Select aph.ID, ah.ID, aph.Time, ah.AlarmType, ah.WorkerMemberID, ah.TargetSensorID, ah.TargetZoneID, ";
            strSQL += "aph.ProcessType, aph.Distance, aph.Status, aph.Message, aph.IsCritical from AlarmProcessHistory as aph, AlarmHistory as ah ";
            strSQL += "where aph.AlarmHistoryID = ah.ID and AlarmHistoryID not in (Select ID from AlarmHistory where Done = 1) and ";
            strSQL += "AlarmHistoryID in (Select AlarmHistoryID from AlarmProcessHistory where ProcessType = 1) and ";
            strSQL += "aph.Time Between DATEADD(day, -" + m_nIgnorLogDay.ToString() + ", getdate()) and getdate() and ";
            strSQL += "ah.SiteID = " + NetworkServer.Instance.SiteID.ToString();

            SqlConnection connection = m_dbMgr.Connect();
            SqlDataReader reader = m_dbMgr.ExecuteReader(strSQL, connection);

            // 하나의 AlarmHistory에 대하여 여러개의 AlarmProcessHistory가 존재할 수 있다.
            // 그 가운데 가장 나중의 것만 사용한다.
            Dictionary<int, DangerState> dicAlarms = new Dictionary<int, DangerState>();

            while (reader.Read())
            {
                int nProcessHistoryID = (int)reader[0];
                int nAlarmHistoryID = (int)reader[1];
                DateTime time = (DateTime)reader[2];
                int nAlarmType = (int)reader[3];
                string strWorkerMemberID = (string)reader[4];
                string strTargetSensorID = reader.IsDBNull(5) ? "" : (string)reader[5];
                string strTargetZoneID = reader.IsDBNull(6) ? "" : reader[6].ToString();
                int nProcessType = (int)reader[7];
                double distance = (double)reader[8];
                string strStatus = (string)reader[9];
                string strMessage = (string)reader[10];
                bool isCritical = (bool)reader[11];

                if (nAlarmType <= (int)SafetyChecker.DangerType.NONE || nAlarmType >= (int)SafetyChecker.DangerType.TYPE_COUNT)
                {
                    //string strError = string.Format("DB Error, AlarmHistoryID : {0}, AlarmType : {1}인 데이터가 존재합니다.",
                    //    nAlarmHistoryID, nAlarmType);
                    //NetworkServer.Instance.ServiceProvider.ConnectionLog.WriteLine(strError);

                    continue;
                }

                if (nProcessType <= (int)AlarmManager.AlarmStatus.NONE || nProcessType >= (int)AlarmManager.AlarmStatus.TYPE_COUNT)
                {
                    //string strError = string.Format("DB Error, AlarmProcessHistoryID : {0}, Status : {1}인 데이터가 존재합니다.",
                    //    nProcessHistoryID, nProcessType);
                    //NetworkServer.Instance.ServiceProvider.ConnectionLog.WriteLine(strError);

                    continue;
                }

                if (strWorkerMemberID.Length == 0)
                {
                    m_alarmMgr.AddGasAlarm(strTargetSensorID, distance, (SafetyChecker.DangerType)nAlarmType, nAlarmHistoryID);
                    continue;
                }

                DataCar car = null;
                DataEquip equip = null;
                DataZone zone = null;

                if (strTargetSensorID.Length == 0 && strTargetZoneID.Length == 0)
                {
                    //string strError = string.Format("DB Error, AlarmHistoryID : {0}, TargetSensorID와 TargetZoneID 둘 다 NULL인 데이터가 존재합니다.",
                    //    nAlarmHistoryID);
                    //NetworkServer.Instance.ServiceProvider.ConnectionLog.WriteLine(strError);

                    continue;
                }
                else if (strTargetSensorID.Length > 0)
                {
                    car = m_dataMgr.FindCar2(strTargetSensorID);

                    if (car == null)
                    {
                        equip = m_dataMgr.FindEquip2(strTargetSensorID);

                        if (equip != null)
                            dicSensorIDs[strTargetSensorID] = equip;
                    }
                    else
                        dicSensorIDs[strTargetSensorID] = car;
                }
                else// if (strTargetZoneID.Length > 0)
                {
                    int nZoneID;

                    if (!int.TryParse(strTargetZoneID, out nZoneID))
                        continue;

                    zone = m_dataMgr.FindZone(nZoneID);

                    /*foreach (DataZone _zone in m_dataMgr.DataZones)
                    {
                        if (_zone.ID == nZoneID)
                        {
                            zone = _zone;
                            break;
                        }
                    }*/
                }

                if (car == null && equip == null && zone == null)
                    continue;

                DataWorker worker = m_dataMgr.FindWorker(strWorkerMemberID);
                //DataWorker worker = m_dataMgr.GetWorkerFromID(nWorkerID);

                if (worker == null)
                    continue;

                dicSensorIDs[worker.Sensor] = worker;

                DangerState state = null;

                if (dicAlarms.ContainsKey(nAlarmHistoryID))
                {
                    state = dicAlarms[nAlarmHistoryID];

                    if (state.AlarmProcessHistoryID > nProcessHistoryID)
                        continue;
                }
                else
                {
                    state = new DangerState();
                    dicAlarms[nAlarmHistoryID] = state;
                }

                state.AlarmHistoryID = nAlarmHistoryID;
                state.AlarmProcessHistoryID = nProcessHistoryID;
                state.EventTime = time;
                state.Type = (SafetyChecker.DangerType)nAlarmType;
                state.AlarmMessage = strMessage;
                state.AlarmStatus = (HSMSServer2.AlarmManager.AlarmStatus)nProcessType;
                state.AlarmStatusMessage = strStatus;
                state.ShortAlarmMessage = AlarmManager.MakeShortAlarmMessage(worker, car, equip, zone, state.Type, distance, isCritical);
                state.Distance = distance;
                state.IsCritical = isCritical;
                state.TargetCar = car;
                state.TargetEquipment = equip;
                state.TargetZone = zone;
                state.Worker = worker;
            }

            reader.Close();
            connection.Close();

            return dicAlarms;
        }

		public void NetworkServerLoad()
        {
		    // Server가 꺼져있는 동안 발생했던 이벤트를 읽어온다.
            Dictionary<string, object> dicSensorIDs;
            Dictionary<int, DangerState> dicAlarms = ReadLastLog(out dicSensorIDs);

            if (dicAlarms == null)
                return;

            SqlConnection connection = m_dbMgr.Connect();

            foreach (KeyValuePair<string, object> pair in dicSensorIDs)
            {
                string strSQL = string.Format("Select Time, X, Y from SensorHistory where SensorID = '{0}' and Time = " +
                    "(select max(Time) from SensorHistory where SensorID = '{0}')", pair.Key);

                SqlDataReader reader = m_dbMgr.ExecuteReader(strSQL, connection);

                if (reader.Read())
                {
                    DateTime dtEvent = (DateTime)reader[0];
                    double x = (double)reader[1];
                    double y = (double)reader[2];

                    SafetyChecker.Instance.AddSensorHistory(pair.Key, new EventSensorData(pair.Key, dtEvent, x, y));
                }

                reader.Close();
            }

            connection.Close();

            foreach (KeyValuePair<int, DangerState> pair in dicAlarms)
            {
                if (SafetyChecker.Instance.CheckAlarmValidation(pair.Value))
                    m_alarmMgr.AddAlarm(pair.Value.Worker, pair.Value, null, true);
                else
                {
                    // 유효하지 않은 알람은 DB에서 종료처리한다.
                    FinishAlarmHistory(pair.Value.AlarmHistoryID);
                }
            }
        }

        // 유효하지 않은 알람을 종료시킨다.
        private void FinishAlarmHistory(int nAlarmHistoryID)
        {
            int nID = AlarmManager.GetMaxID("AlarmProcessHistory") + 1;

            DateTime dtTime = DateTime.Now;
            string strTime = string.Format("{0}-{1}-{2} {3}:{4}:{5}", dtTime.Year, dtTime.Month, dtTime.Day, dtTime.Hour, dtTime.Minute, dtTime.Second);

            string strSQL = string.Format("Insert into AlarmProcessHistory (ID, AlarmHistoryID, Time, ProcessType, Distance, Status, Message, isCritical, Description) values ({0}, {1}, '{2}', 3, 0.0, '', '', 0, NULL)",
                nID, nAlarmHistoryID, strTime);

            SqlConnection connection = m_dbMgr.Connect();
            m_dbMgr.ExecuteSQL(strSQL, connection);

            strSQL = string.Format("Update AlarmHistory set Done = 1 where id = {0}", nAlarmHistoryID);
            m_dbMgr.ExecuteSQL(strSQL, connection);

            connection.Close();
        }

        public void StartServer(int nPort)
        {
            m_nPort = nPort;

            m_provider = new ServiceProvider();
            m_server = new TcpServer(m_provider, m_nPort);
            m_isOpened = m_server.Start();
        }

		public void NetworkServerClosing()
		{
            ERPDataChecker checker = ERPDataChecker.Instance;
            checker.ReleaseThread = true;

            m_logMgr.Stop();

			m_bCloseServer = true;
			m_finishProcess = true;

			m_provider.ReleaseThread();

			if (m_server != null && m_isOpened)
			{
				m_isOpened = false;
				m_server.Stop();
			}
		}

		private DataGridViewRow IndexOfClient(TcpLib2.ConnectionState state)
        {
            int nRowCount = dataGridView1.Rows.Count;
			try
			{
				for (int i = 0; i < nRowCount; i++)
				{
					DataGridViewRow row = dataGridView1.Rows[i];
					if (row.Tag == state)
						return row;
				}
			}
			catch (System.Exception)
			{
			
			}
            

            return null;
        }

        public void AddClient(TcpLib2.ConnectionState state)
        {
            if (m_frmDelegate == null)
                _AddClient(state);
            else
            {
                m_frmDelegate.Invoke((MethodInvoker)delegate
                {
                    _AddClient(state);
                });
            }
        }

        private void _AddClient(TcpLib2.ConnectionState state)
        {
            if (IndexOfClient(state) != null)
                return;

            ClientData client = (ClientData)state.Tag;
            if (client == null)
                return;

            string strClientType = GetClientTypeString(client);

            int nIndex = dataGridView1.Rows.Count + 1;

            DataGridViewRow row = new DataGridViewRow();
            row.Tag = state;

            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            cell.Value = nIndex.ToString();
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = state.RemoteEndPoint.ToString();
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strClientType;
            row.Cells.Add(cell);

            if (client.Type == ClientData.ClientType.UNKNOWN)
                m_dicClientType[state] = cell;

			lock (dataGridView1)
			{
				try
				{
					dataGridView1.Rows.Add(row);
				}
				catch (System.Exception)
				{					
				}				
			}
            
        }

        private static object lockThis = new object();
        private string GetClientTypeString(ClientData client)
        {            
            string strClientType = " 알수 없음";
            lock (lockThis)
            {

                if (client.Type == ClientData.ClientType.HSMS_CLIENT)
                    strClientType = " HSMS Client";
            }
            return strClientType;
        }

        public void UpdateClientType(TcpLib2.ConnectionState state)
        {
            if (m_dicClientType.ContainsKey(state))
            {
                ClientData client = (ClientData)state.Tag;
                if (client == null)
                    return;

                string strClientType = GetClientTypeString(client);

                DataGridViewTextBoxCell cell = m_dicClientType[state];
                cell.Value = strClientType;

                if (client.Type != ClientData.ClientType.UNKNOWN)
                    m_dicClientType.Remove(state);
            }
        }

        public void RemoveClient(TcpLib2.ConnectionState state)
        {
            ClientData data = (ClientData)state.Tag;

            // 정상적인 종료인가?
            bool normalClose = data != null && data.PingCount < 3;

            if (m_frmDelegate == null || m_frmDelegate.IsDisposed)
                _RemoveClient(state);
            else
            {              
                m_frmDelegate.Invoke((MethodInvoker)delegate
                {
                    _RemoveClient(state);
                });
            }
        }

        private void _RemoveClient(TcpLib2.ConnectionState state)
        {
            try
            {
                DataGridViewRow targetRow = IndexOfClient(state);
                if (targetRow == null)
                    return;
                dataGridView1.Rows.Remove(targetRow);
            }
            catch (System.Exception)
            {
            }

            m_dicLoginUserID.Remove(state);

            if (m_dicClientType.ContainsKey(state))
                m_dicClientType.Remove(state);
        }

        public void WritePortToDB(int nPort)
        {
            SqlConnection connection = m_dbMgr.Connect();
            string strSQL = string.Format("Select Max(Port) from HSMSServerPort");

            SqlDataReader reader = m_dbMgr.ExecuteReader(strSQL, connection);
            bool isExist = false;

            if (reader.Read())
            {
                if (!reader.IsDBNull(0))
                {
                    if ((int)reader[0] > 0)
                        isExist = true;
                }
            }

            reader.Close();
            
            if (!isExist)
            {
                strSQL = string.Format("Insert into HSMSServerPort (Port) values ({0})", nPort);
                m_dbMgr.ExecuteSQL(strSQL, connection);
            }
            else
            {
                strSQL = string.Format("Update HSMSServerPort Set Port = {0}", nPort);
                m_dbMgr.ExecuteSQL(strSQL, connection);
            }

            connection.Close();
        }
    }
}

namespace HSMS
{
    public class SensorWorker
    {}

    public class SensorVehicle
    {}
}