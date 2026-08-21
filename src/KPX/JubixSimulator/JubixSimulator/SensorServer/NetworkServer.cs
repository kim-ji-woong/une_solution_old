using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using TcpLib2;
using System.Collections;
using System.Windows.Forms;


namespace S1SensorServer
{
    public class NetworkServer
    {
        private LogFileManager m_logMgr = null;
		
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
        private NetworkServiceProvider m_provider = null;//new ServiceProvider();
        private int m_nPort = 0;
        private bool m_isOpened = false;
        private DBUtility.WebDBManager m_dbMgr = null;//new DBUtility.WebDBManager();
		public DBUtility.WebDBManager DBManager
		{
			get { return m_dbMgr; }
			set { m_dbMgr = value; }
		}
        

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
        private static NetworkServer m_instance = null;
        public static NetworkServer Instance
        {
            get { return m_instance; }
        }
        	
        public IOManager IOManager
        {
            get { return m_ioMgr; }
        }

        public NetworkServiceProvider ServiceProvider
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
        }

        private int m_nSiteID = 1;
        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        public void LoadSiteID()
        {
            DBUtility.Utility ini = new DBUtility.Utility();
            string strSiteID = ini.getinivalue("Server Connection Info", "siteid");

            if (strSiteID.Length > 0)
            {
                int.TryParse(strSiteID, out m_nSiteID);
            }
            else
            {
                m_nSiteID = 500;
            }
        }

        public NetworkServer(DataGridView dtGrid)
        {

            //m_dbMgr = dbMgr;
            LoadSiteID();

            if (m_dbMgr == null)
                m_dbMgr = new DBUtility.WebDBManager(m_nSiteID);

            m_instance = this;

            if (dtGrid != null)
            {
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

                dtGrid = dataGridView1;
            }
			
						
			LoadBaseData();

            m_provider = new NetworkServiceProvider();
            m_ioMgr = new IOManager(m_nSiteID);          

            string strPort = m_dbMgr.LoadIni("jubix_port", "Server Connection Info");

			if (strPort.Length > 0)
			{
				int.TryParse(strPort, out m_nPort);
			}
			else
			{
				m_nPort = 33333;
			}
            //m_nPort = 5000;
            //PSMSensorServer.WriteLine("PORT : " + m_nPort.ToString());
            			
            m_logMgr = new LogFileManager();
        }

		public void NetworkServerLoad()
        {
            if (m_nPort > 0)
            {
                m_server = new TcpServer(m_provider, m_nPort);
                m_server.ConnectionLog = ConnectionLogClient.Instance;
                m_isOpened = m_server.Start();
            }
        }

		public void NetworkServerClosing()
		{
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
			catch (System.Exception ex)
			{
                ConnectionLogClient.Instance.WriteLine("IndexOfClient", ex);
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

            if (client.Type == ClientData.ClientType.UNKNOWN)
                m_dicClientType[state] = cell;

            DdMonitor.Enter(dataGridView1);
			try
			{
				dataGridView1.Rows.Add(row);
			}
			catch (System.Exception ex)
			{
                ConnectionLogClient.Instance.WriteLine("_AddClient", ex);
			}			
			
            DdMonitor.Exit(dataGridView1);
        }


        private string GetClientTypeString(ClientData client)
        {            
            string strClientType = " 알수 없음";
            //DdMonitor.Enter(m_provider.LockObject);
            {

                if (client.Type == ClientData.ClientType.KPXSensorServer)
                    strClientType = " KPX";

            }
            //DdMonitor.Exit(m_provider.LockObject);
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
                DataGridViewRow row = IndexOfClient(state);

                if (row != null)
                    row.Cells[2].Value = strClientType;

                if (client.Type != ClientData.ClientType.UNKNOWN)
                    m_dicClientType.Remove(state);
            }
        }

        public void RemoveClient(TcpLib2.ConnectionState state)
        {
            ClientData data = (ClientData)state.Tag;

            // 정상적인 종료인가?
            bool normalClose = data != null && data.PingCount <= 5;
            			
            if (m_frmDelegate == null)
            {
                if (m_dicClientType.ContainsKey(state))
                    m_dicClientType.Remove(state);
                return;
            }
                
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
                ConnectionLogClient.Instance.WriteLine("_RemoveClient", ex);
            }
            
            if (m_dicClientType.ContainsKey(state))
                m_dicClientType.Remove(state);
            
        }

      
    }
}
