using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using TcpLib2;
using System.Collections;
using System.Windows.Forms;

namespace FireSignalSender
{
    public class NetworkServer
    {
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
        private ServiceProvider m_provider = null;
        private int m_nPort = 0;
        private bool m_isOpened = false;

        private static NetworkServer m_instance = null;

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
                
        public ServiceProvider ServiceProvider
        {
            get { return m_provider; }
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
            m_nSiteID = 1;
        }

        public NetworkServer()
        {
            LoadSiteID();
           
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
            m_nPort = 19500;
        }

        public void NetworkServerLoad()
        {
            if (m_nPort > 0)
            {
                // Read History SensorInfo
                m_server = new TcpServer(m_provider, m_nPort);
                m_server.ConnectionLog = ConnectionLogEx.Instance;
                m_isOpened = m_server.Start();
            }
        }

        public void NetworkServerClosing()
        {
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

            if (client.Type == ClientData.ClientType.UNKNOWN)
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
            string strClientType = " 알수 없음";
            //DdMonitor.Enter(m_provider.LockObject);
            {
                if (client.Type == ClientData.ClientType.SOP_MONITOR2)
                    strClientType = "Sensor Monitor";                
            }
            //DdMonitor.Exit(m_provider.LockObject);
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
                return;//_RemoveClient(state);
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

                for (int i = nTargetRowIndex; i < nRowCount; i++)
                {
                    DataGridViewRow row = dataGridView1.Rows[i];
                    row.Cells[0].Value = (i + 1).ToString();
                }
            }
            catch (System.Exception ex)
            {
                ConnectionLogEx.Instance.WriteLine("_RemoveClient", ex);
            }

            if (m_dicClientType.ContainsKey(state))
                m_dicClientType.Remove(state);
        }
    }
}
