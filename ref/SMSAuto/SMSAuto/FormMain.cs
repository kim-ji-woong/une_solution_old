using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using libSMSReceiver;
using libSMS;
using System.IO;

namespace SMSAuto
{
    public partial class FormMain : Form, IEventReceiver
    {
        private SMSManager m_mgr = new SMSManager();
        // Key : 전화번호
        // Value : 이름
        private Dictionary<string, string> m_dicPhoneNumbers = new Dictionary<string, string>();

        public FormMain()
        {
            InitializeComponent();

            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Visible = false;
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.Icon = this.Icon;
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;

            ReadPhoneNumbers("PhoneNumberList.txt");

            m_mgr.Start(this, "192.168.0.250", false);
        }

        private void ReadPhoneNumbers(string strPath)
        {
            m_dicPhoneNumbers.Clear();

            StreamReader reader = new StreamReader(strPath, Encoding.UTF8);

            while (reader.EndOfStream == false)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    continue;

                int nIndex1 = strLine.IndexOf(' ');
                int nIndex2 = strLine.IndexOf('\t');
                int nIndex = 0;

                if (nIndex1 > 0 && nIndex2 > 0)
                {
                    nIndex = nIndex1 < nIndex2 ? nIndex1 : nIndex2;
                }
                else if (nIndex1 > 0)
                    nIndex = nIndex1;
                else if (nIndex2 > 0)
                    nIndex = nIndex2;
                else
                    continue;

                string strPhoneNumber = strLine.Substring(0, nIndex);
                string strName = strLine.Substring(nIndex + 1).Trim();
                m_dicPhoneNumbers[strPhoneNumber] = strName;
            }

            reader.Close();
        }

        private void tsMenuClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public bool OnReceive(List<libSMSReceiver.Message> messages)
        {
            IMessageClient messageClient = MessageClientFactory.CreateMessageClient(3, "");

            if (messageClient == null)
                return true;

            List<MessageContent> contentsList = new List<MessageContent>();

            foreach (libSMSReceiver.Message message in messages)
            {
                string strPhoneNumber = message.PhoneNumber;
                string strMessage = message.MessageText;

                string strName = "", str = "";

                if (m_dicPhoneNumbers.TryGetValue(strPhoneNumber, out strName))
                    str = strName + "님 안녕하세요. 주식회사 유엔이입니다.\r\n보내주신 격려의 메시지 감사드립니다.\r\n[수신메시지]\r\n" + strMessage;
                else
                    str = string.Format("주식회사 유엔이입니다.\r\n보내주신 격려의 메시지 감사드립니다.\r\n[수신메시지]\r\n" + strMessage);

                MessageContent contents = new MessageContent();
                contents.Caller = "027144133";
                contents.Reciver = strPhoneNumber;
                contents.Message = str;

                contentsList.Add(contents);
                //messageClient.SendSMS("027144133", strPhoneNumber, )
            }

            messageClient.SendSMS(contentsList);
            return true;
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_mgr.Stop();
        }

        private void tsMenuReloadPhoneNumberList_Click(object sender, EventArgs e)
        {
            ReadPhoneNumbers("PhoneNumberList.txt");
        }
    }
}
