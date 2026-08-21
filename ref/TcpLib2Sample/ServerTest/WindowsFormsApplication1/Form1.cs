using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TcpLib2;

namespace ServerTest
{
    public partial class Form1 : Form
    {
        private TcpServer Server;
        private DefaultServiceProvider Provider;

        private static Form1 m_instance = null;
        public static Form1 Instance
        {
            get { return m_instance; }
        }

        public Form1()
        {
            m_instance = this;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //TcpLib2.ConnectionLog.Instance.Create();
            Provider = new DefaultServiceProvider();
            Server = new TcpServer(Provider, 502);
            Server.Start();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Server.Stop();
        }

        private void SendReturn(byte[] received)
        {
            string strBytes = "8F 01 04 8C 20 00 40 96 20 00 20 00 20 00 20 00 20 00 20 00 20 00 20 00 20 0A 20 0A 40 D2 20 00 20 00 20 00 28 01 20 00 20 00 20 00 20 00 20 0F 41 04 40 FA 20 00 20 00 20 00 20 00 20 00 20 00 20 00 20 00 20 00 20 00 20 00 20 14 20 00 20 00 20 00 20 00 20 00 20 05 20 00 40 BE 20 00 40 BE 20 19 20 00 20 1E 40 C8 20 00 40 C8 20 00 40 C8 20 00 20 0A 40 C8 20 00 40 C8 40 C8 20 1E 40 C8 20 14 40 C8 20 00 40 DC 20 14 20 00 40 C8 20 00";
            byte[] _bytes = ToBinary(strBytes);

            byte[] bytes = new byte[_bytes.Length + 5];

            for (int i=0;i<5;i++)
            {
                bytes[i] = received[i];
            }

            for (int i=5;i<bytes.Length;i++)
            {
                bytes[i] = _bytes[i - 5];
            }

            int nIndex = dataGridViewClients.SelectedCells[0].RowIndex;
            TcpLib2.ConnectionState state = (TcpLib2.ConnectionState)dataGridViewClients.Rows[nIndex].Tag;
            state.Write(bytes, 0, bytes.Length);
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (dataGridViewClients.SelectedCells.Count == 0)
            {
                MessageBox.Show("Client를 선택해 주세요.");
            }
            else
            {
                string strBytes = "07 9B 00 00 00 83 01 04 80 20 00 20 05 20 00 40 C8 40 BE 40 C8 40 C8 20 00 20 00 20 00 40 BE 40 BE 20 00 20 00 20 00 20 00 20 00 40 BE 40 BE 40 BE 40 BE 40 BE 40 BE 40 BE 40 BE 40 BE 40 BE 40 00 40 AA 40 BE 40 B4 40 B4 40 BE 40 B4 48 B1 20 00 40 B4 40 B4 40 B4 40 B4 40 AA 40 B4 40 B4 40 B4 40 BE 40 BE 40 BE 40 BE 40 C8 40 BE 40 BE 20 00 40 BE 40 BE 40 C8 40 BE 40 BE 40 C8 40 BE 40 BE 40 B4 40 BE 40 BE 40 C8";
                byte[] bytes = ToBinary(strBytes);

                int nIndex = dataGridViewClients.SelectedCells[0].RowIndex;
                TcpLib2.ConnectionState state = (TcpLib2.ConnectionState)dataGridViewClients.Rows[nIndex].Tag;
                state.LengthAdd = false;
                state.Write(bytes, 0, bytes.Length);
                /*string strMessage = textBoxSend.Text.Trim();

                if (strMessage.Length == 0)
                    return;

                int nIndex = dataGridViewClients.SelectedCells[0].RowIndex;
                TcpLib2.ConnectionState state = (TcpLib2.ConnectionState)dataGridViewClients.Rows[nIndex].Tag;

                if (strMessage.StartsWith("0x"))
                {
                    state.LengthAdd = false;
                    byte[] bytes = ToBinary(strMessage);

                    if (bytes != null)
                        state.Write(bytes, 0, bytes.Length);
                }
                else
                {
                    // 1.
                    string hexString = textBoxSend.Text.Replace(" ", "");
                    byte[] xbytes = new byte[hexString.Length / 2];
                    for (int i = 0; i < xbytes.Length; i++)
                    {
                        xbytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
                    }

                    if (xbytes != null)
                        state.Write(xbytes, 0, xbytes.Length);

                    // 2.
                    //byte[] bytes = Encoding.UTF8.GetBytes(textBoxSend.Text);

                    //if (state.Write(bytes, 0, bytes.Length))
                    //{
                    //    if (textBoxDialogue.Text.Length == 0)
                    //        textBoxDialogue.Text += "Me : " + textBoxSend.Text;
                    //    else
                    //        textBoxDialogue.Text += "\r\nMe : " + textBoxSend.Text;

                    //    textBoxSend.Text = "";
                    //}
                }*/
            }
        }

        private byte[] ToBinary(string strMessage)
        {
            if (strMessage.StartsWith("0x"))
                strMessage = strMessage.Substring(2);

            char separator = ' ';

            if (strMessage.Contains(','))
                separator = ',';

            string[] tokens = strMessage.Split(separator);

            if (tokens.Count() < 2)
                return null;

            int nIndex = 0;
            byte[] bytes = new byte[tokens.Count()];

            foreach (string token in tokens)
            {
                string strToken = token.Trim();

                if (StringToByte(strToken, out bytes[nIndex++]) == false)
                    return null;
            }

            return bytes;
        }

        private bool StringToByte(string str, out byte data)
        {
            data = 0x00;

            if (str.Length >= 3 || str.Length < 1)
                return false;

            int nData = 0;

            for (int i=0;i<str.Length;i++)
            {
                char ch = str.ElementAt(i);

                if (ch >= '0' && ch <= '9')
                    nData = nData * 16 + ch - '0';
                else if (ch >= 'a' && ch <= 'f')
                    nData = nData * 16 + ch - 'a' + 10;
                else if (ch >= 'A' && ch <= 'F')
                    nData = nData * 16 + ch - 'A' + 10;
                else
                    return false;
            }

            data = (byte)nData;
            return true;
        }

        public void OnReceive(TcpLib2.ConnectionState state, byte[] receivedData)
        {
            if (receivedData == null)
                return;

            /*if (receivedData.Length == 12)
            {
                if (receivedData[8] == 0x02 && receivedData[9] == 0x4E && receivedData[10] == 0x00 && receivedData[11] == 0x46)
                {
                    SendReturn(receivedData);
                    return;
                }
            }*/

            System.Net.IPEndPoint endPoint = (System.Net.IPEndPoint)state.RemoteEndPoint;
            string strIP = endPoint.Address.ToString();

            string strReceived = Encoding.UTF8.GetString(receivedData, 0, receivedData.Length);

            byte[] sendBytes = new byte[12];
            sendBytes[0] = receivedData[0];
            sendBytes[1] = receivedData[1];
            sendBytes[2] = receivedData[2];
            sendBytes[3] = receivedData[3];
            sendBytes[4] = receivedData[4];
            sendBytes[5] = receivedData[5];
            sendBytes[6] = receivedData[6];
            sendBytes[7] = 0x01;
            sendBytes[8] = 0x8D;

            state.LengthAdd = false;
            //state.Write(sendBytes, 0, sendBytes.Length);

            //Invoke((MethodInvoker)delegate
            //{
            //    if (textBoxDialogue.Text.Length == 0)
            //        textBoxDialogue.Text += strIP + " : " + strReceived;
            //    else
            //        textBoxDialogue.Text += "\r\n" + strIP + " : " + strReceived;
            //});
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
    }
}
