using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using TcpLib2;
using System.Collections;
using System.Windows.Forms;
using SOPServer;

namespace SDMSServer
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
        private DBUtility.WebDBManager m_dbMgr = null;//new DBUtility.WebDBManager();
		public DBUtility.WebDBManager DBManager
		{
			get { return m_dbMgr; }
			set { m_dbMgr = value; }
		}
        private static NetworkServer m_instance = null;

        private SensorManager m_sensorMgr = null;
        private IOManager m_ioMgr = null;

        private bool m_finishProcess = false;

        // Delegate 호출을 위한 Form
        private Form m_frmDelegate = null;

        public Form FormDelegate
        {
            get { return m_frmDelegate; }
            set { m_frmDelegate = value; }
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

		public SDMSServer.SensorManager SensorManager
        {
            get { return m_sensorMgr; }
        }

        public SDMSServer.IOManager IOManager
        {
            get { return m_ioMgr; }
        }

        public SDMSServer.ServiceProvider ServiceProvider
        {
            get { return m_provider; }
        }

        // Team이나 직원정보, 담당자 정보를 바꾸거나 조회하는 중인가?
        private object m_memberCriticalSection = new object();
        public object MemberCriticalSection
        {
            get { return m_memberCriticalSection; }
        }

        private bool m_isSimulationMode = false;
        public bool SimulationMode
        {
            get { return m_isSimulationMode; }
        }

		public void LoadBaseData()
		{
            m_usePSM = ReadPSMInfo();
			ZoneManager.Instance.LoadBuildingData();
			ZoneManager.Instance.LoadZones();
            ZoneManager.Instance.LoadEquipmentZones();

			DataManager.Instance.LoadFireEquipment();
			DataManager.Instance.LoadFacilityManager();

            ReciverManager.Instance.InitReciverState();
		}

        private bool m_usePSM = true;

        public bool UsePSM
        {
            get { return m_usePSM; }
            set { m_usePSM = value; }
        }

        private int m_nSiteID = 1;
        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        private bool ReadPSMInfo()
        {
            string strSQL = "Select PropertyName, PropertyValue from OptionSDMS where PropertyName = 'UsePSM' and SiteID = " + m_nSiteID.ToString();

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;
            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                string strPropertyName = DBUtility.WebDBManager.GetStringField(arrResult[i]);
                string strPropertyValue = DBUtility.WebDBManager.GetStringField(arrResult[i + 1]);

                if (strPropertyName == null || strPropertyValue == null)
                    continue;

                if (strPropertyValue.ToLower() == "false" || strPropertyValue == "0")
                {
                    return false;
                }
            }
            return true;
        }

        public void LoadSiteID()
        {
            DBUtility.Utility ini = new DBUtility.Utility();
            string strSiteID = ini.getinivalue("Server Connection Info", "siteid");
            //string strSiteID = m_dbMgr.LoadIni("siteid", "Server Connection Info");

            if (strSiteID.Length > 0)
            {
                int.TryParse(strSiteID, out m_nSiteID);
            }
            else
            {
                m_nSiteID = 1;
            }
        }

        public NetworkServer(DBUtility.WebDBManager dbMgr = null, bool isSimulationMode = false)
        {

            m_dbMgr = dbMgr;

            LoadSiteID();
            m_isSimulationMode = isSimulationMode;

            if (m_dbMgr == null)
            { 
                m_dbMgr = new DBUtility.WebDBManager(m_nSiteID);
                LoadDBOption();
            }

            m_instance = this;

			dataGridView1 = new System.Windows.Forms.DataGridView();
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
			
			
			LoadBaseData();

            m_provider = new ServiceProvider();
			m_ioMgr = new IOManager(m_dbMgr);
            m_sensorMgr = new SensorManager(m_dbMgr, m_provider);

            if (m_usePSM)
                PSMManager.Instance.Load(m_dbMgr);

            string strPort = m_dbMgr.LoadIni("sdms_port", "Server Connection Info");

			if (strPort.Length > 0)
			{
				int.TryParse(strPort, out m_nPort);
			}
			else
			{
				m_nPort = 20700;
			}


              
            SOPService.WriteLine("PORT : " + m_nPort.ToString());

			string bRunTeamReader = m_dbMgr.LoadIni("run_team_reader", "Server Connection Info");
			if (bRunTeamReader.Length > 0)
			{
				int nTeamReader = -1;
				int.TryParse(bRunTeamReader, out nTeamReader);
				if (nTeamReader == 1)
				{
					m_bRunTeamReader = true;
				}
			}
			else
			{
				m_bRunTeamReader = false;
			}
            SOPService.WriteLine("TeamReader : " + m_bRunTeamReader.ToString());

            m_logMgr = new LogManager(m_dbMgr);
        }

        private bool LoadDBOption()
        {
            string strWebServerURL = m_dbMgr.LoadIni("webserver_url", "Server Connection Info").Trim();

            if (strWebServerURL.Length == 0)
                return false;

            string strServerIP = m_dbMgr.LoadIni("server_ip", "Server Connection Info").Trim();

            if (strServerIP.Length == 0)
                return false;

            string strServerPort = m_dbMgr.LoadIni("server_port", "Server Connection Info").Trim();

            if (strServerPort.Length == 0)
                return false;

            string strDBName = m_dbMgr.LoadIni("server_db", "Server Connection Info").Trim();

            if (strDBName.Length == 0)
                return false;

            if (strServerPort == "1433")
                m_dbMgr.DatabaseType = DBUtility.WebDBManager.DBType.sqlserver;
            else if (strServerPort == "3306")
                m_dbMgr.DatabaseType = DBUtility.WebDBManager.DBType.mysql;
            else
                return false;

            m_dbMgr.WebServerURL = strWebServerURL;
            m_dbMgr.DatabaseHost = strServerIP;
            m_dbMgr.DatabaseName = strDBName;
            return true;
        }

		private bool m_bRunTeamReader = false;

        // TeamReader는 남동발전 인사DB에 접근하여 팀및 팀원 정보를 갱신하는 기능
        // 현재는 유지 되지 않으므로 Comment처리함. 추후 사용할 가능성이 있음 
        // comment by skkim 2017-04-28
        //private TeamReader.TeamReader m_TeamReader = null;

		public void NetworkServerLoad()
        {
			AbnormalSensorManager.Instance.Progress = true;

            if (m_nPort > 0)
            {
                // Server가 꺼져있는 동안 발생했던 History 정보를 읽어온다.
                m_sensorMgr.ReadSensorHistory(m_provider);

                m_server = new TcpServer(m_provider, m_nPort);
                m_server.ConnectionLog = ConnectionLogEx.Instance;
                m_isOpened = m_server.Start();

                if (m_isOpened)
                    WritePortToDB();
            }

			if (m_bRunTeamReader == true)
			{
                // TeamReader는 남동발전 인사DB에 접근하여 팀및 팀원 정보를 갱신하는 기능
                // 현재는 유지 되지 않으므로 Comment처리함. 추후 사용할 가능성이 있음 
                // comment by skkim 2017-04-28
                //m_TeamReader = new TeamReader.TeamReader(m_dbMgr);
			}
        }

		public void NetworkServerClosing()
		{
			if (m_bRunTeamReader == true)
			{
                // TeamReader는 남동발전 인사DB에 접근하여 팀및 팀원 정보를 갱신하는 기능
                // 현재는 유지 되지 않으므로 Comment처리함. 추후 사용할 가능성이 있음 
                // comment by skkim 2017-04-28
				//m_TeamReader.StopDB();
			}

            m_logMgr.Stop();

			m_bCloseServer = true;
			m_finishProcess = true;

			AbnormalSensorManager.Instance.Dispose();

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
			catch (System.Exception ex)
			{
                ConnectionLogEx.Instance.WriteLine("IndexOfClient", ex);
			}
            

            return null;
        }

        public void AddClient(TcpLib2.ConnectionState state)
        {
            if (m_frmDelegate == null)
                return;
            //_AddClient(state);
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

            if (client.ClientType == SDMS.TCP_CLIENT.UNKNOWN)
                m_dicClientType[state] = cell;

            DdMonitor.Enter(dataGridView1);
			try
			{
				dataGridView1.Rows.Add(row);
			}
			catch (System.Exception ex)
			{
                ConnectionLogEx.Instance.WriteLine("_AddClient", ex);
			}			
			
            DdMonitor.Exit(dataGridView1);
        }


        private string GetClientTypeString(ClientData client)
        {            
            string strClientType = " " + SDMS.TCP_CLIENT.GetClientTypeString(client.ClientType);
            return strClientType;
        }

        public void UpdateClientType(TcpLib2.ConnectionState state, ClientData newData)
        {
            if (m_dicClientType.ContainsKey(state))
            {
                ClientData client = (ClientData)state.Tag;
                if (client == null)
                    return;

                string strClientType = GetClientTypeString(newData);
                DataGridViewRow row = IndexOfClient(state);

                if (row != null)
                    row.Cells[2].Value = strClientType;

                //DataGridViewTextBoxCell cell = m_dicClientType[state];
                //cell.Value = strClientType;

                if (client.ClientType != SDMS.TCP_CLIENT.UNKNOWN)
                    m_dicClientType.Remove(state);
            }
        }

        public void RemoveClient(TcpLib2.ConnectionState state)
        {
            ClientData data = (ClientData)state.Tag;

            // 정상적인 종료인가?
            bool normalClose = data != null && data.PingCount <= 5;

			LoginManager.Instance.RemoveClient(state);
            ControlMonitoring.ControlManager.Instance.RemoveClient(state.Tag, normalClose);

            if (m_frmDelegate == null)
            {
                if (m_dicClientType.ContainsKey(state))
                    m_dicClientType.Remove(state);
                return;//_RemoveClient(state);
            }
                
            else
            {
                m_frmDelegate.Invoke((MethodInvoker)delegate
                {
                    _RemoveClient(state);
                });
            }

            try
            {
                if( data != null)
                    data.CloseClient();
            }
            catch(Exception)
            { }
            
        }

        private void _RemoveClient(TcpLib2.ConnectionState state)
        {
            
            try
            {
                DataGridViewRow targetRow = IndexOfClient(state);
                if (targetRow == null)
                    return;

                int nTargetRowIndex = targetRow.Index;
                dataGridView1.Rows.Remove(targetRow);
                int nRowCount = dataGridView1.RowCount;

                for (int i=nTargetRowIndex;i<nRowCount;i++)
                {
                    DataGridViewRow row = dataGridView1.Rows[i];
                    row.Cells[0].Value = (i + 1).ToString();
                }
            }
            catch (System.Exception ex)
            {
                ConnectionLogEx.Instance.WriteLine("_RemoveClient", ex);
            }
            
            //int nRowCount = dataGridView1.Rows.Count;
            //for (int i = nIndex; i < nRowCount; i++)
            //{
            //   DataGridViewRow row = dataGridView1.Rows[i];
            //    DataGridViewTextBoxCell cell = (DataGridViewTextBoxCell)row.Cells[0];

            //    cell.Value = (i + 1).ToString();
            //}

            if (m_dicClientType.ContainsKey(state))
                m_dicClientType.Remove(state);

            
        }

        private void WritePortToDB()
        {
            string strSQL = string.Format("Select Max(Port) from SDMSServerPort");
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            if (arrResult.Count == 0 || arrResult[0].ToString() == "null")
            {
                strSQL = string.Format("Insert into SDMSServerPort (Port, SiteID) values ({0}, {1})", m_nPort, m_nSiteID);
                m_dbMgr.GetResultData(strSQL, 0);
            }
            else
            {
                strSQL = string.Format("Update SDMSServerPort Set Port = {0} WHERE SiteID = {1}", m_nPort, m_nSiteID);
                m_dbMgr.GetResultData(strSQL, 0);
            }

        }

		
    }
}
