using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TcpLib2;
using System.Collections;

namespace SDMSServer
{
    public partial class FormMain : Form
    {
        private TcpServer m_server = null;
        private ServiceProvider m_provider = null;//new ServiceProvider();
        private int m_nPort = 0;
        private bool m_isOpened = false;
        private DBUtility.WebDBManager m_dbMgr = new DBUtility.WebDBManager();
		public DBUtility.WebDBManager DBManager
		{
			get { return m_dbMgr; }
			set { m_dbMgr = value; }
		}
        private static FormMain m_instance = null;

        private SensorManager m_sensorMgr = null;
        private IOManager m_ioMgr = null;

        private bool m_finishProcess = false;

        // DataGrid에 Client Type을 갱신하기 위한 변수
        // 동기화 문제를 피하기 위하여 Dictionary 사용
        private Dictionary<TcpLib2.ConnectionState, DataGridViewTextBoxCell> m_dicClientType = new Dictionary<TcpLib2.ConnectionState, DataGridViewTextBoxCell>();

        public bool FinishProcess
        {
            get { return m_finishProcess; }
        }

        public static FormMain Instance
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

		public void LoadBaseData()
		{
			ZoneManager.Instance.LoadBuildingData();
			ZoneManager.Instance.LoadZones();
            ZoneManager.Instance.LoadEquipmentZones();

			DataManager.Instance.LoadFireEquipment();
			DataManager.Instance.LoadFacilityManager();
		}

        public FormMain()
        {
            m_instance = this;
            InitializeComponent();

			LoadBaseData();

            m_provider = new ServiceProvider();
			m_ioMgr = new IOManager(m_dbMgr);
            m_sensorMgr = new SensorManager(m_dbMgr, m_provider);			

            string strPort = m_dbMgr.LoadIni("sdms_port", "Server Connection Info");

            if (strPort.Length > 0)
            {
                if (int.TryParse(strPort, out m_nPort))
                    textBoxPort.Text = strPort;
            }
            else
                int.TryParse(textBoxPort.Text, out m_nPort);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
			AbnormalSensorManager.Instance.Progress = true;

            InitGrid();

            if (m_nPort > 0)
            {
                // Server가 꺼져있는 동안 발생했던 History 정보를 읽어온다.
                m_sensorMgr.ReadSensorHistory(m_provider);

                m_server = new TcpServer(m_provider, m_nPort);
                m_isOpened = m_server.Start();

                if (m_isOpened)
                    WritePortToDB();
            }
        }

        private void InitGrid()
        {
            colIndex.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colIndex.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            colIP.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colIP.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            colType.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colType.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        }

        private int IndexOfClient(TcpLib2.ConnectionState state)
        {
            int nRowCount = dataGridView1.Rows.Count;

            for (int i = 0; i < nRowCount; i++)
            {
                DataGridViewRow row = dataGridView1.Rows[i];
                if (row.Tag == state)
                    return i;
            }

            return -1;
        }

        public void AddClient(TcpLib2.ConnectionState state)
        {
            if (IndexOfClient(state) >= 0)
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

            dataGridView1.Rows.Add(row);
        }

        private string GetClientTypeString(ClientData client)
        {
            string strClientType = " 알수 없음";

            if (client.Type == ClientData.ClientType.SDMS_CLIENT)
                strClientType = " SDMS Client";
            else if (client.Type == ClientData.ClientType.SENSOR_SIMULATOR)
                strClientType = " Sensor Simulator";
            else if (client.Type == ClientData.ClientType.SOP_SIMULATOR)
                strClientType = " SOP Simulator";

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
            int nIndex = IndexOfClient(state);
            if (nIndex < 0)
                return;

            dataGridView1.Rows.RemoveAt(nIndex);
            int nRowCount = dataGridView1.Rows.Count;

            for (int i = nIndex; i < nRowCount; i++)
            {
                DataGridViewRow row = dataGridView1.Rows[i];
                DataGridViewTextBoxCell cell = (DataGridViewTextBoxCell)row.Cells[0];

                cell.Value = (i + 1).ToString();
            }

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
                strSQL = string.Format("Insert into SDMSServerPort (Port) values ({0})", m_nPort);
                m_dbMgr.GetResultData(strSQL, 0);
            }
            else
            {
                strSQL = string.Format("Update SDMSServerPort Set Port = {0}", m_nPort);
                m_dbMgr.GetResultData(strSQL, 0);
            }

        }

        private void btnChangePort_Click(object sender, EventArgs e)
        {
            int nPort;

            if (int.TryParse(textBoxPort.Text, out nPort))
            {
                if (m_nPort == nPort)
                    return;

                m_nPort = nPort;

                m_isOpened = false;
                m_server.Stop();

                m_server = new TcpServer(m_provider, m_nPort);
                m_isOpened = m_server.Start();

                if (m_isOpened)
                    WritePortToDB();
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
			AbnormalSensorManager.Instance.Dispose();

            m_provider.ReleaseThread();

            if (m_server != null && m_isOpened)
            {
                m_isOpened = false;
                m_server.Stop();
            }
        }
    }
}
