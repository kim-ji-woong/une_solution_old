using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using TcpLib2;

namespace ClientTest
{
    public partial class Form1 : Form
    {
        private ClientProvider m_provider = new ClientProvider();

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
            m_provider.LengthAdd = false;

            if (!m_provider.Connect("127.0.0.1", 4380))
            {
                MessageBox.Show(m_provider.ErrorMessage);
            }
        }

        public void OnReceive()
        {
            byte[] bytes = m_provider.ReceivedData;

            if (bytes == null)
                return;

            string strLog = "";
            int nBytesCount = bytes.Count();

            for (int i=0;i<nBytesCount;i++)
            {
                if (strLog.Length == 0)
                    strLog = string.Format("{0:X2}", bytes[i]);
                else
                    strLog += ", " + string.Format("{0:X2}", bytes[i]);
            }

            this.Invoke((MethodInvoker)delegate
            {
                string strText = textBoxDialogue.Text.Trim();

                if (strText.Length == 0)
                    strText = "Receive Bytes : " + strLog;
                else
                    strText += "\r\nReceive Bytes : " + strLog;

                textBoxDialogue.Text = strText;
            });
            //System.Diagnostics.Trace.WriteLine(m_provider.ReceivedData);
            //string strReceived = Encoding.UTF8.GetString(m_provider.ReceivedData, 0, m_provider.ReceivedData.Length);

            //Invoke((MethodInvoker)delegate
            //{
            //    if (textBoxDialogue.Text.Length == 0)
            //        textBoxDialogue.Text = "Server : " + strReceived;
            //    else
            //        textBoxDialogue.Text += "\r\nServer : " + strReceived;
            //});
        }

        public void OnDropConnection()
        {
            Invoke((MethodInvoker)delegate
            {
                if (textBoxDialogue.Text.Length == 0)
                    textBoxDialogue.Text = "Server is disconnected";
                else
                    textBoxDialogue.Text += "\r\nServer is disconnected";
            });
        }

        // 헤더
        private byte STX = 0x02; // 고정값
        private byte Type = 0x02; // 명령 데이터 서버 => 측정소
        private ushort Sequence = 0x0001; // SEQUENCE = 오버플로우 시 1부터 다시 전송  // 임시값
        private ushort ServiceCode = 0x0001; // 악취 모니터링의 경우 0x0001 전송
        private ushort RegionCode = 0x0004; // 나이파 위치지역 : 0x0004 경상도
        private ushort GroupCode = 0x0001; // 측정소 설치가 광범위하거나 구분하여 관리가 필요한 경우
        private ushort NodeCode = 0x0001; // 악취노드 = 0x0001 ~ 0x063; 1~99 사용
        private ushort CountOfPacket = 0x0002; // 악취 센서 갯수 2개 , 미세먼지 센서 2개 => 16진수 0x0004
        private ulong MilliSeconds = 0x0000000000000000; // 서버시간 사용시 0

        // 패킷
        // 센서 클래스 코드 (악취)
        private ushort stink_class_code_basic = 0x0001;
        private ushort stink_class_code_normal = 0x0001;
        private ushort stink_class_code_vent = 0x0400;

        private ushort dust_class_code_basic = 0x0002;
        private ushort dust_class_code_normal = 0x0002;
        private ushort dust_class_code_vent = 0x0800;

        // 센서 코드 (악취)
        private ushort H2S = 0x2100;
        private ushort NH3 = 0x2101;
        private ushort TVOC = 0x2102;
        private ushort OU = 0x2103;

        // 센서 코드 (미세먼지)
        private ushort PM_10 = 0x2200;
        private ushort PM_25 = 0x2201;

        private uint SensorValue = 0x00000000; // 센서 측정값 서버 => 센서는 요청이라 0 전송
        private byte SensorStatus = 0x00;

        private ushort CRC = 0x0000; // 사용안함.
        private byte ETX = 0x03; // 고정값

        private static byte[] StringToByteArray(string hexString)
        {
            hexString = hexString.Replace("-", "");
            byte[] byteArray = new byte[hexString.Length / 2];

            for (int i = 0; i < byteArray.Length; i++)
            {
                byteArray[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return byteArray;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {


            try
            {
                ////for(int i = 1 ; i < 24; i++)
                //{
                //    byte[] bytes = MsgHelper.MakeData((byte)0, 0, 0, (byte)15, (byte)3, (byte)1);
                //    m_provider.Client.Client.Send(bytes, 0, bytes.Length, SocketFlags.None);

                //    //byte[] bytes2 = MsgHelper.MakeData((byte)0, 0, 0, (byte)15, (byte)1, (byte)1);
                //    //m_provider.Client.Client.Send(bytes2, 0, bytes2.Length, SocketFlags.None);
                //}

                string message = textBoxSend.Text;

                string strMessage = message.Replace("-", "");

                byte[] data = new byte[message.Length / 2];

                byte[] strArray = StringToByteArray(message);

                byte[] temp = new byte[43];
                temp[0] = STX;
                temp[1] = Type;

                byte[] arrSequence = BitConverter.GetBytes(Sequence);
                Array.Reverse(arrSequence);
                Array.Copy(arrSequence, 0, temp, 2, arrSequence.Length);

                byte[] arrServiceCode = BitConverter.GetBytes(ServiceCode);
                Array.Reverse(arrServiceCode);
                Array.Copy(arrServiceCode, 0, temp, 4, arrServiceCode.Length);

                byte[] arrRegionCode = BitConverter.GetBytes(RegionCode);
                Array.Reverse(arrRegionCode);
                Array.Copy(arrRegionCode, 0, temp, 6, arrRegionCode.Length);

                byte[] arrGroupCode = BitConverter.GetBytes(GroupCode);
                Array.Reverse(arrGroupCode);
                Array.Copy(arrGroupCode, 0, temp, 8, arrGroupCode.Length);

                byte[] arrNodeCode = BitConverter.GetBytes(NodeCode);
                Array.Reverse(arrNodeCode);
                Array.Copy(arrNodeCode, 0, temp, 10, arrNodeCode.Length);

                byte[] arrCountOfPacket = BitConverter.GetBytes(CountOfPacket);
                Array.Reverse(arrCountOfPacket);
                Array.Copy(arrCountOfPacket, 0, temp, 12, arrCountOfPacket.Length);

                byte[] arrMilliSeconds = BitConverter.GetBytes(MilliSeconds);
                Array.Reverse(arrMilliSeconds);
                Array.Copy(arrMilliSeconds, 0, temp, 14, arrMilliSeconds.Length);

                byte[] arrStink_class_code_basic = BitConverter.GetBytes(stink_class_code_basic);
                Array.Reverse(arrStink_class_code_basic);
                Array.Copy(arrStink_class_code_basic, 0, temp, 22, arrStink_class_code_basic.Length);

                byte[] arrTVOC = BitConverter.GetBytes(TVOC);
                Array.Reverse(arrTVOC);
                Array.Copy(arrTVOC, 0, temp, 24, arrTVOC.Length);

                byte[] arrSensorValue = BitConverter.GetBytes(SensorValue);
                Array.Reverse(arrSensorValue);
                Array.Copy(arrSensorValue, 0, temp, 26, arrSensorValue.Length);

                temp[30] = SensorStatus;

                Array.Copy(arrStink_class_code_basic, 0, temp, 31, arrStink_class_code_basic.Length);

                byte[] arrOU = BitConverter.GetBytes(OU);
                Array.Reverse(arrOU);
                Array.Copy(arrOU, 0, temp, 33, arrOU.Length);

                Array.Copy(arrSensorValue, 0, temp, 35, arrSensorValue.Length);

                temp[39] = SensorStatus;

                byte[] arrCRC = BitConverter.GetBytes(CRC);

                Array.Reverse(arrCRC);
                Array.Copy(arrCRC, 0, temp, 40, arrCRC.Length);

                temp[42] = ETX;
                m_provider.LengthAdd = false;
                //m_provider.Client.Client.Send(temp, 0, temp.Length, SocketFlags.None);
                m_provider.Client.Client.Send(strArray, 0, strArray.Length, SocketFlags.None);


            }
            catch (Exception)
            {
                return;
            }

            textBoxDialogue.Text += "\r\nMe : " + textBoxSend.Text;
            textBoxSend.Text = "";
        }
    }
}
