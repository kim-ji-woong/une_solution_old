using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Concurrent;

namespace IntegratedManagement4
{
    public partial class FormClients : Form
    {
        private ConcurrentDictionary<TcpLib2.ConnectionState, ClientData> m_arrClients = null;

        public FormClients(object arrClients)
        {
            InitializeComponent();

            m_arrClients = (ConcurrentDictionary<TcpLib2.ConnectionState, ClientData>)arrClients;
        }

        private void FormClients_Load(object sender, EventArgs e)
        {
            ICollection<TcpLib2.ConnectionState> arClient = null;
            SDMSServer.DdMonitor.Enter(FormMain.Instance.NetworkServer.ServiceProvider.LockObject, true);
            {
                arClient = m_arrClients.Keys;
            }
            SDMSServer.DdMonitor.Exit(FormMain.Instance.NetworkServer.ServiceProvider.LockObject, true);

            foreach (TcpLib2.ConnectionState state in arClient)
            {
                ClientData data = (ClientData)state.Tag;

                if (data == null)
                    continue;

                int nRowIndex = gridClient.Rows.Add();

                if (nRowIndex < 0)
                    continue;

                DataGridViewRow row = gridClient.Rows[nRowIndex];

                row.Cells[0].Value = nRowIndex + 1;
                row.Cells[1].Value = state.IPAddress + ":" + state.PortNo.ToString();
                row.Cells[2].Value = data.Type.ToString();
            }
        }
    }
}
