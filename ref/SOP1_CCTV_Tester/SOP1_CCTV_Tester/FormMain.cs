using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOP1_CCTV_Tester
{
    public partial class FormMain : Form
    {
        private UnE.Control.CCTVCtrl m_cctvCtrl = null;

        public FormMain()
        {
            InitializeComponent();

            m_cctvCtrl = new UnE.Control.CCTVCtrl(UnE.Control.CCTVTypes.XpressStrm);
            panelCCTV.Controls.Add(m_cctvCtrl);

            InitCCTVList();
        }

        private void InitCCTVList()
        {
            cboCCTV.Items.Add(new CCTV(1, "1호기_COAL_FEEDER-A", "172.18.131.111", "9400"));
            cboCCTV.Items.Add(new CCTV(1, "1호기_COAL_FEEDER-C", "172.18.131.111", "9400"));
            cboCCTV.Items.Add(new CCTV(1, "1호기_COAL_FEEDER-D", "172.18.131.111", "9400"));
            cboCCTV.Items.Add(new CCTV(1, "1호기_COAL_FEEDER-B", "172.18.131.111", "9400"));
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            cboCCTV.SelectedIndex = 0;
        }

        private void cboCCTV_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboCCTV.SelectedIndex < 0)
                m_cctvCtrl.Disconnect();
            else
            {
                CCTV cctv = (CCTV)cboCCTV.Items[cboCCTV.SelectedIndex];

                m_cctvCtrl.AddProperty("MediaType", "rtp-tcp");
                m_cctvCtrl.AddProperty("Channel", "0");
                m_cctvCtrl.AddProperty("Stream", "0");
                m_cctvCtrl.AddProperty("HttpPort", "0");
                m_cctvCtrl.AddProperty("IPAddress", cctv.IP);
                m_cctvCtrl.AddProperty("Port", cctv.Port);
                m_cctvCtrl.AddProperty("UserName", "guest");
                m_cctvCtrl.AddProperty("Password", "");
                m_cctvCtrl.AddProperty("ReversePTZ", "0");
                m_cctvCtrl.AddProperty("AccessKey", cctv.CameraName);
                m_cctvCtrl.AddProperty("URL", "");

                m_cctvCtrl.Connect();
            }
        }
    }
}
