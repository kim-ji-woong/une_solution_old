using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOPWebServer
{
    public partial class FormClientList : Form
    {
        public class ClientData
        {
            private int m_nClientType = 0;
            private int m_nClientSubType = 0;
            private string m_strIP = "";
            private int m_nPort = 0;

            public int ClientType
            {
                get { return m_nClientType; }
                set { m_nClientType = value; }
            }

            public int ClientSubType
            {
                get { return m_nClientSubType; }
                set { m_nClientSubType = value; }
            }

            public string IP
            {
                get { return m_strIP; }
                set { m_strIP = value; }
            }

            public int Port
            {
                get { return m_nPort; }
                set { m_nPort = value; }
            }
        }

        public FormClientList()
        {
            InitializeComponent();
        }

        public void SetClient(List<ClientData> clientDatas)
        {
            try
            {
                gridClients.Rows.Clear();

                foreach (ClientData data in clientDatas)
                {
                    int nRowIndex = gridClients.Rows.Add();

                    if (nRowIndex < 0)
                        return;

                    DataGridViewRow row = gridClients.Rows[nRowIndex];

                    row.Cells[0].Value = nRowIndex + 1;
                    row.Cells[0].Tag = data.Port;
                    row.Cells[1].Value = SOPWebServer.ClientType.ToString(data.ClientType);
                    row.Cells[1].Tag = data.ClientType;
                    row.Cells[2].Value = SOPWebServer.ClientSubType.ToString(data.ClientSubType);
                    row.Cells[2].Tag = data.ClientSubType;
                    row.Cells[3].Value = string.Format("{0}:{1}", data.IP, data.Port);
                    row.Cells[3].Tag = data.IP;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("FormClientList.SetClient Error : " + e.Message);
            }
        }

        public void AddClient(int nClientType, int nClientSubType, string strIP, int nPort)
        {
            this.Invoke((MethodInvoker)delegate
            {
                try
                {
                    int nRowIndex = gridClients.Rows.Add();

                    if (nRowIndex < 0)
                        return;

                    DataGridViewRow row = gridClients.Rows[nRowIndex];

                    row.Cells[0].Value = nRowIndex + 1;
                    row.Cells[0].Tag = nPort;
                    row.Cells[1].Value = SOPWebServer.ClientType.ToString(nClientType);
                    row.Cells[1].Tag = nClientType;
                    row.Cells[2].Value = SOPWebServer.ClientSubType.ToString(nClientSubType);
                    row.Cells[2].Tag = nClientSubType;
                    row.Cells[3].Value = string.Format("{0}:{1}", strIP, nPort);
                    row.Cells[3].Tag = strIP;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine("FormClientList.SetClient Error : " + e.Message);
                }
            });
        }

        public void RemoveClient(string strIP, int nPort)
        {
            string strConnection = string.Format("{0}:{1}", strIP, nPort);

            this.Invoke((MethodInvoker)delegate
            {
                int nRowCount = gridClients.Rows.Count;
                int nIndex = -1;

                try
                {
                    for (int i = 0; i < nRowCount; i++)
                    {
                        DataGridViewRow row = gridClients.Rows[i];

                        if (row.Cells[3].Value.ToString() == strConnection)
                        {
                            gridClients.Rows.RemoveAt(i);
                            nIndex = i;
                            nRowCount--;
                        }
                    }

                    for (int i = nIndex; i < nRowCount; i++)
                    {
                        DataGridViewRow row = gridClients.Rows[i];
                        row.Cells[0].Value = i + 1;
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine("FormMain.RemoveClient Error : " + e.Message);
                }
            });
        }
    }
}
