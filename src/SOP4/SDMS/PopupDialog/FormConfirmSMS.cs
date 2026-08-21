using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SDMS.PopupDialog
{
    public partial class FormConfirmSMS : Form
    {
        public enum MessageType { SMS = 1, BROADCAST = 2, BOTH = 3 };
        public enum OwnerType { SDMS = 0, SOP_SIMULATOR };
        public enum EventType { DETECT_FIRE = 0, MALFUNCTION, REPORT_FIRE };
        public enum ResultType { NONE = 0, SEND_SMS = 1, RUN_BROADCAST = 2, RUN_BOTH };

        private ResultType m_resultType = ResultType.NONE;

        public ResultType Result
        {
            get { return m_resultType; }
        }

        public FormConfirmSMS()
        {
            InitializeComponent();
        }

        public void ShowConfirmMessage(MessageType msgType, OwnerType ownerType, EventType eventType, string strReceiver)
        {
            string strMessageType = "";

            if (msgType == MessageType.SMS)
            {
                strMessageType = "[문자메시지 전송]";
                btnSendSMS.Visible = true;
                btnRunBoth.Visible = btnRunBroadcast.Visible = false;
            }
            else if (msgType == MessageType.BROADCAST)
            {
                strMessageType = "[방송]";
                btnRunBroadcast.Location = btnSendSMS.Location;
                btnRunBroadcast.Visible = true;
                btnSendSMS.Visible = btnRunBoth.Visible = false;
            }
            else if (msgType == MessageType.BOTH)
            {
                strMessageType = "[문자메시지 전송 및 방송]";
                btnRunBroadcast.Location = new Point(btnCancel.Location.X - btnSendSMS.Location.X + btnRunBoth.Location.X, btnRunBroadcast.Location.Y);
                btnRunBoth.Visible = btnRunBroadcast.Visible = btnSendSMS.Visible = true;
            }
            else
                return;

            string strOwner = GetOwnerString(ownerType);

            if (strOwner == null)
                return;

            string strTitle = strMessageType + " - " + strOwner;

            if (eventType == EventType.DETECT_FIRE)
            {
                textBoxMsg.Text = "[화재탐지신호]가 발생하였습니다.\r\n" + strMessageType + "을 실행하시겠습니까?";
            }
            else if (eventType == EventType.MALFUNCTION)
            {
                textBoxMsg.Text = "[오작동/복구]를 선택하였습니다.\r\n" + strMessageType + "을 실행하시겠습니까?";
            }
            else if (eventType == EventType.REPORT_FIRE)
            {
                textBoxMsg.Text = "[화재전파]를 선택하였습니다.\r\n" + strMessageType + "을 실행하시겠습니까?";
            }
            else
                return;

            base.Show();
        }

        private string GetOwnerString(OwnerType type)
        {
            if (type == OwnerType.SDMS)
                return "재난탐지시스템";
            else if (type == OwnerType.SOP_SIMULATOR)
                return "SOP 시스템";

            return null;
        }

        private void btnRunBoth_Click(object sender, EventArgs e)
        {
            m_resultType = ResultType.RUN_BOTH;
            this.Close();
        }

        private void btnRunBroadcast_Click(object sender, EventArgs e)
        {
            m_resultType = ResultType.RUN_BROADCAST;
            this.Close();
        }

        private void btnSendSMS_Click(object sender, EventArgs e)
        {
            m_resultType = ResultType.SEND_SMS;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            m_resultType = ResultType.NONE;
            this.Close();
        }
    }
}
