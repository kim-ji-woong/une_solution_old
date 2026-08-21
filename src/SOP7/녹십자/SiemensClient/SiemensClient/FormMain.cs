using System;
using System.Configuration;
using System.Windows.Forms;
using System.Text;

namespace SiemensClient
{
    public partial class FormMain : Form
    {
        private const byte STX = 0x02;
        private const byte ETX = 0x03;

        private const byte ClearAll = 0x91;
        private const byte DetectFire = 0x92;
        private const byte ClearSensor = 0x93;
        private const byte DetectError = 0x94;
        private const byte ClearError = 0x95;

        private ClientProvider m_provider = new ClientProvider();

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            string strIP;
            int nPort;

            if (ReadSettings(out strIP, out nPort))
                ConnectServer(strIP, nPort);
        }

        private void ConnectServer(string strIP, int nPort)
        {
            m_provider.LengthAdd = false;

            if (m_provider.Connect(strIP, nPort))
                btnSend.Enabled = true;
        }

        private bool ReadSettings(out string strIP, out int nPort)
        {
            nPort = 0;
            strIP = ConfigurationManager.AppSettings.Get("ip").Trim();

            if (strIP == null)
                return false;

            string strPort = ConfigurationManager.AppSettings.Get("port").Trim();

            if (strPort == null)
                return false;

            if (int.TryParse(strPort, out nPort) == false)
                return false;

            return true;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (m_provider.IsConnected)
            {
                byte[] bytes = GetBytes();

                if (bytes == null)
                    return;

                m_provider.Send(bytes, 0, bytes.Length);
            }
        }

        private byte[] GetBytes()
        {
            byte opCode = 0x00;

            if (radioAllClear.Checked)
                opCode = ClearAll;
            else if (radioFire.Checked)
                opCode = DetectFire;
            else if (radioClear.Checked)
                opCode = ClearSensor;
            else
            {
                MessageBox.Show("명령옵션을 선택하세요.");
                return null;
            }

            string strAddr = textBoxAddr.Text.Trim();

            if (strAddr.Length == 0)
            {
                textBoxAddr.Focus();
                MessageBox.Show("센서 Address를 입력하세요.");
                return null;
            }

            string strPosition = textBoxPosition.Text.Trim();
            string strMessage = textBoxMessage.Text.Trim();

            DateTime dtNow = DateTime.Now;
            string strData = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00},{6},{7},{8}",
                dtNow.Year, dtNow.Month, dtNow.Day,
                dtNow.Hour, dtNow.Minute, dtNow.Second,
                strAddr,
                strPosition,
                strMessage);

            Encoding enc = Encoding.GetEncoding("euc-kr");
            byte[] datas = enc.GetBytes(strData);
            int dataLength = datas.Length;
            byte[] bytes = new byte[dataLength + 8];

            bytes[0] = STX;
            bytes[1] = (byte)((dataLength + 4) / 128 + 0x80);
            bytes[2] = (byte)((dataLength + 4) % 128 + 0x80);
            bytes[3] = 0x80;
            bytes[4] = 0x80;
            bytes[5] = opCode;
            bytes[6] = 0x81;

            for (int i=0;i<dataLength;i++)
            {
                bytes[7 + i] = datas[i];
            }

            bytes[7 + dataLength] = ETX;
            return bytes;
        }
    }
}
