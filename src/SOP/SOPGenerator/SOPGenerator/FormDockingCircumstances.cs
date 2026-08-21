using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace SOPGen
{
    public partial class FormDockingCircumstances : Form
    {
        private FormDocking m_Docking = null;

        public FormDockingCircumstances(FormDocking dock)
        {
            InitializeComponent();

            m_Docking = dock;
        }

        private void btnStandard_Click(object sender, EventArgs e)
        {
            m_Docking.GetStandard().ShowDialog();
        }

        public void SetString(string strValue)
        {
            textMessage.Text = strValue;

            int nLen = GetAnsiByte(textMessage.Text);
            labelMessageSize.Text = string.Format("메시지 크기 : ({0}Byte)", nLen);
        }

        private void textCellPhone1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))
            {
                e.Handled = true;
            }
        }

        private void textCellPhone1_Leave(object sender, EventArgs e)
        {
            TextBox strValue = (TextBox)sender;
            bool isCheck = m_Docking.GetMain().NumberCheck(strValue.Text);
            if (!isCheck)
            {
                MessageBox.Show("번호를 입력해 주세요");
                textCellPhone1.Focus();
            }
        }

        private void textCellPhone2_Leave(object sender, EventArgs e)
        {
            if (textCellPhone2.TextLength < 3)
            {
                MessageBox.Show("번호를 입력해 주세요");
                textCellPhone2.Focus();
            }
        }

        private void textCellPhone3_Leave(object sender, EventArgs e)
        {
            if (textCellPhone3.TextLength != 4)
            {
                MessageBox.Show("번호를 입력해 주세요");
                textCellPhone3.Focus();
            }
        }

        private void textFAX1_Leave(object sender, EventArgs e)
        {
            TextBox text = (TextBox)sender;
            bool isCheck = m_Docking.GetMain().AreaCodeCheck(text.Text);
            if (!isCheck)
            {
                MessageBox.Show("지역번호를 정확하게 입력해 주세요.");
                textFAX1.Focus();
            }
        }

        private void textFAX2_Leave(object sender, EventArgs e)
        {
            if (sender.ToString().Length < 3)
            {
                MessageBox.Show("번호를 입력해 주세요");
                textFAX2.Focus();
            }
        }

        private void textFAX3_Leave(object sender, EventArgs e)
        {
            TextBox strValue = (TextBox)sender;
            if (strValue.Text.Length != 4 && strValue.Text.Length != 0)
            {
                MessageBox.Show("번호를 입력해 주세요.");
                textFAX3.Focus();
            }
        }

        public void NewSOP()
        {
            textCellPhone1.Text = "";
            textCellPhone2.Text = "";
            textCellPhone3.Text = "";
            textFAX1.Text = "";
            textFAX2.Text = "";
            textFAX3.Text = "";
            textFAXFile.Text = "";
            textMessage.Text = "";
            textBroadcast.Text = "";
        }

        // 유니코드 문자열을 Ansi 문자열로 변환하였을때 Byte 크기를 알려준다.
        private int GetAnsiByte(string str)
        {
            char[] arrChar = str.ToCharArray();
            int nArrCount = arrChar.Length;
            int nByteCount = 0;

            for (int i = 0; i < nArrCount; i++)
            {
                if ((ushort)arrChar[i] < 256)
                    nByteCount++;
                else
                    nByteCount += 2;
            }

            return nByteCount;
        }

        private void textMessage_KeyDown(object sender, KeyEventArgs e)
        {
            int nLen = GetAnsiByte(textMessage.Text);
            labelMessageSize.Text = string.Format("메시지 크기 : ({0}Byte)", nLen);
        }
    }
}
