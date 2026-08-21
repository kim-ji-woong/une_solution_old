using DBUtility2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TcpLib2;
using UnE.Sensor;

namespace BroadcastServer
{
    public partial class FormMain : Form
    {
        private TcpServer server;
        private ServiceProvider provider;

        private static FormMain m_instance = null;
        public static FormMain Instance
        {
            get { return m_instance; }
        }

        private bool m_runThread = false;
        public bool RunThread
        {
            get { return m_runThread; }
            set { m_runThread = value; }
        }

        private List<TcpLib2.ConnectionState> m_connectionStates = new List<TcpLib2.ConnectionState>();
        private WebDBManager m_dbManager = null;

        private int m_nRecentSec = 30;
        
        public FormMain()
        {
            m_instance = this;
            InitializeComponent();

            initComboBox();

            notifyIcon1.ContextMenuStrip = contextMenuStrip1;

            string strSiteID = System.Configuration.ConfigurationManager.AppSettings.Get("siteid");
            if (strSiteID != null && strSiteID.Length > 0)
            {
                int nSiteID;

                if (int.TryParse(strSiteID.Trim(), out nSiteID))
                {
                    m_dbManager = new WebDBManager(nSiteID);
                }
            }
            else
            {
                MessageBox.Show("site id를 확인하세요");
                this.Close();
            }
            string strPort = System.Configuration.ConfigurationManager.AppSettings.Get("port");
            if (strPort != null && strPort.Length > 0)
            {
                int nPort;

                if (int.TryParse(strPort.Trim(), out nPort))
                {
                    provider = new ServiceProvider();
                    server = new TcpServer(provider, nPort);
                    server.Start();
                }
            }
            else
            {
                MessageBox.Show("port를 확인하세요");
                this.Close();
            }

            string strRecentSec = System.Configuration.ConfigurationManager.AppSettings.Get("nRecentSec");
            if (strRecentSec != null && strRecentSec.Length > 0)
            {
                int nRecentSec;
                if (int.TryParse(strRecentSec.Trim(), out nRecentSec))
                {
                    m_nRecentSec = nRecentSec;
                }
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            Thread t = new Thread(new ThreadStart(DisplayCommand));
            t.Start();
        }

        private void DisplayCommand()
        {
            m_runThread = true;

            while (m_runThread)
            {
                ArrayList arrResult = m_dbManager.GetResultData("Select ID, TimeStamp, FacilityType, IsBegin From BroadcastCommand");
                if (arrResult != null && arrResult.Count > 0)
                {
                    for (int i = 0; i < arrResult.Count; i+=4)
                    {
                        VariousData<int> nID = WebDBManager.GetIntField(arrResult[i].ToString());
                        VariousData<DateTime> timeStamp = WebDBManager.GetDateTimeField(arrResult[i + 1]);
                        VariousData<int> nFacilityType = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                        VariousData<int> nIsBegin = WebDBManager.GetIntField(arrResult[i + 3].ToString());

                        if (nID == null || timeStamp == null || nFacilityType == null || nIsBegin == null)
                            continue;

                        bool bTimeover = false;

                        DateTime now = DateTime.Now.AddSeconds(-m_nRecentSec);
                        if (timeStamp.Data >= now)
                        {
                            string sendMessage = "U";
                            if (nFacilityType.Data == 0) //facilityType:2byte
                                sendMessage += "00";
                            else
                                sendMessage += nFacilityType.Data.ToString();

                            if (nIsBegin.Data == 1)
                                sendMessage += ":S";
                            else
                                sendMessage += ":E";

                            Send(sendMessage);
                        }
                        else
                        {
                            bTimeover = true;
                        }

                        string deleteQuery = string.Format("Delete from BroadcastCommand Where ID = {0} And FacilityType = {1}", nID.Data, nFacilityType.Data);
                        m_dbManager.GetResultData(deleteQuery);

                        IFacility.FacilityType facilityType = IFacility.ToFacilityType(nFacilityType.Data);
                        string strMsg = IFacility.GetFacilityTypeString(facilityType).Replace(" ", "").Replace("센서", "");
                        if (bTimeover)
                            strMsg += "시간 초과";
                        else
                            strMsg += (nIsBegin.Data == 0) ? "중지" : "실행";

                        StringBuilder historyQuery = new StringBuilder();
                        historyQuery.Append("Insert into BroadcastHistory(Text, UseSiren, PlayOption, RepeatCount, HostInfo, AddTime, SiteID) ");
                        historyQuery.AppendFormat("Values('{0}', 0,0,0,'',getDate(), {1})", strMsg, m_dbManager.SiteID);

                        m_dbManager.GetResultData(historyQuery.ToString());
                    }
                }

                Thread.Sleep(500);
            }
        }

        private void initComboBox()
        {
            comboBox1.Items.Add(new ComboBoxItem() { nFacilityType = 0, strFacilityType = "화재" });
            comboBox1.Items.Add(new ComboBoxItem() { nFacilityType = 11, strFacilityType = "가스" });
            comboBox1.Items.Add(new ComboBoxItem() { nFacilityType = 17, strFacilityType = "정전" });
            comboBox1.Items.Add(new ComboBoxItem() { nFacilityType = 19, strFacilityType = "침수" });
            comboBox1.Items.Add(new ComboBoxItem() { nFacilityType = 20, strFacilityType = "테러" });
            comboBox1.Items.Add(new ComboBoxItem() { nFacilityType = 22, strFacilityType = "코로나" });
            comboBox1.Items.Add(new ComboBoxItem() { nFacilityType = 50, strFacilityType = "지진" });
            comboBox1.SelectedIndex = 0;
            comboBox1.ValueMember = "nFacilityType";
            comboBox1.DisplayMember = "strFacilityType";
        }

        private void btnBegin_Click(object sender, EventArgs e)
        {
            int nFacilityType = ((ComboBoxItem)comboBox1.SelectedItem).nFacilityType;

            string strFacilityType;
            if (nFacilityType == 0)
                strFacilityType = "00";
            else
                strFacilityType = nFacilityType.ToString();

            string strMessage = "U" + strFacilityType + ":S";
            Send(strMessage);
        }

        private void btnEnd_Click(object sender, EventArgs e)
        {
            int nFacilityType = ((ComboBoxItem)comboBox1.SelectedItem).nFacilityType;

            string strFacilityType;
            if (nFacilityType == 0)
                strFacilityType = "00";
            else
                strFacilityType = nFacilityType.ToString();

            string strMessage = "U" + strFacilityType + ":E";
            Send(strMessage);
        }

        private void Send(string strMessage)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(strMessage);
            foreach (DataGridViewRow row in dataGridViewClients.Rows)
            {
                TcpLib2.ConnectionState state = (TcpLib2.ConnectionState)dataGridViewClients.Rows[row.Index].Tag;
                state.LengthAdd = false;
                state.Write(bytes, 0, bytes.Length);
            }
        }

        public void OnReceive(TcpLib2.ConnectionState state, byte[] receivedData)
        {
            if (receivedData == null)
                return;

            System.Net.IPEndPoint endPoint = (System.Net.IPEndPoint)state.RemoteEndPoint;
            string strIP = endPoint.Address.ToString();

            string strReceived = Encoding.UTF8.GetString(receivedData, 0, receivedData.Length);

            Invoke((MethodInvoker)delegate
            {
                if (textBoxDialogue.Text.Length == 0)
                    textBoxDialogue.Text += strIP + " : " + strReceived;
                else
                    textBoxDialogue.Text += "\r\n" + strIP + " : " + strReceived;
            });
        }

        private int FindClientIndex(string strIP, int nPort)
        {
            int nRowCount = dataGridViewClients.Rows.Count;

            for (int i = 0; i < nRowCount; i++)
            {
                DataGridViewRow row = dataGridViewClients.Rows[i];
                string _strIP = (string)row.Cells[1].Value;
                int _nPort = (int)row.Cells[2].Value;

                if (strIP == _strIP && nPort == _nPort)
                    return i;
            }

            return -1;
        }

        private void RemoveClient(int nIndex)
        {
            dataGridViewClients.Rows.RemoveAt(nIndex);
        }

        private void RemoveClient(TcpLib2.ConnectionState state)
        {
            foreach (DataGridViewRow row in dataGridViewClients.Rows)
            {
                if (row.Tag == state)
                {
                    dataGridViewClients.Rows.Remove(row);
                    return;
                }
            }
        }

        private void AddClient(string strIP, int nPort, TcpLib2.ConnectionState state)
        {
            int nRowCount = dataGridViewClients.Rows.Count;

            DataGridViewRow row = new DataGridViewRow();
            row.Tag = state;

            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            cell.Value = nRowCount + 1;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strIP;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = nPort;
            row.Cells.Add(cell);

            dataGridViewClients.Rows.Add(row);
        }

        public void OnAccept(TcpLib2.ConnectionState state)
        {
            System.Net.IPEndPoint endPoint = (System.Net.IPEndPoint)state.RemoteEndPoint;
            string strIP = endPoint.Address.ToString();
            int nPort = endPoint.Port;

            int nIndex = FindClientIndex(strIP, nPort);

            if (nIndex >= 0)
            {
                Invoke((MethodInvoker)delegate
                {
                    RemoveClient(nIndex);
                });
            }

            Invoke((MethodInvoker)delegate
            {
                AddClient(strIP, nPort, state);
            });
        }

        public void OnDropConnection(TcpLib2.ConnectionState state)
        {
            Invoke((MethodInvoker)delegate
            {
                RemoveClient(state);
            });
        }

        public class ComboBoxItem
        {
            private int m_nFacilityType = -1;
            private string m_strFacilityType = "";

            public int nFacilityType
            {
                get { return m_nFacilityType; }
                set { m_nFacilityType = value; }
            }

            public string strFacilityType
            {
                get { return m_strFacilityType; }
                set { m_strFacilityType = value; }
            }
        }

        private bool m_bClose = false;
        private void 종료ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_bClose = true;
            RunThread = false;
            this.Close();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!m_bClose)
            {
                e.Cancel = true;
                notifyIcon1.Visible = true;
                this.Hide();
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!this.Visible)
                this.Show();
        }
    }
}
