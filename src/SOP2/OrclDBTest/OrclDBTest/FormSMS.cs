using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace OrclDBTest
{
    public partial class FormSMS : Form
    {
        private SMSManager m_smsMgr = null;

        public FormSMS(SMSManager smsMgr)
        {
            m_smsMgr = smsMgr;
            InitializeComponent();
        }

        private void FormSMS_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int nByteLength = 0;
            string strMessage = textBox1.Text;
            int nLen = strMessage.Length;

            for (int i = 0; i < nLen; i++)
            {
                if (strMessage.ElementAt(i) < 256)
                    nByteLength++;
                else
                    nByteLength += 2;
            }

            labelMsgSize.Text = string.Format("{0} Byte", nByteLength);
        }

        private bool CheckPhoneNumber(TextBox textBox)
        {
            string strPhoneNumber = textBox.Text;
            int nLen = strPhoneNumber.Length;

            for (int i = 0; i < nLen; i++)
            {
                char ch = strPhoneNumber.ElementAt(i);

                if (ch < '0' || ch > '9')
                    return false;
            }

            return true;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (textBoxPhoneNumber1.Text == "")
            {
                MessageBox.Show("첫번째 수신번호는 반드시 입력해야 합니다.");
                return;
            }

            if (textBoxSendPhoneNumber.Text == "")
            {
                MessageBox.Show("발신번호는 반드시 입력해야 합니다.");
                return;
            }

            ArrayList arrPhoneNumber = new ArrayList();

            if (!CheckPhoneNumber(textBoxPhoneNumber1))
            {
                MessageBox.Show("첫번째 수신번호에 잘못된 값이 들어있습니다.");
                return;
            }
            else
                arrPhoneNumber.Add(textBoxPhoneNumber1.Text);

            if (textBoxPhoneNumber2.Text != "")
            {
                if (!CheckPhoneNumber(textBoxPhoneNumber2))
                {
                    MessageBox.Show("두번째 수신번호에 잘못된 값이 들어있습니다.");
                    return;
                }
                else
                    arrPhoneNumber.Add(textBoxPhoneNumber2.Text);
            }

            if (!CheckPhoneNumber(textBoxSendPhoneNumber))
            {
                MessageBox.Show("발신번호에 잘못된 값이 들어있습니다.");
                return;
            }

            m_smsMgr.SendSMS(arrPhoneNumber, textBoxSendPhoneNumber.Text, textBox1.Text);
        }
    }
}
