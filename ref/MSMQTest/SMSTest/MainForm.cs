using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Messaging;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Net;

namespace MSMQTest
{
	public partial class MainForm : Form
	{
        private string m_szMsgEncoding = "ks_c_5601-1987";
        private string m_szMsgEncoding2 = "ks_c_5601-1987";
        private string m_szMysqlEncoding = "euckr";

		public MainForm()
		{
			InitializeComponent();
		}
        
        private void MainForm_Load(object sender, EventArgs e)
        {
        
            cmbMysqlEncoding.SelectedIndex = 0;

            string strSection = "Message Server Info";
            string  m_szServerIP = RegUtil.ReadRegValue(strSection, "MessageDBServerIP");
            if (m_szServerIP == null || m_szServerIP == "")
            {
                m_szServerIP = "10.131.5.6";
                RegUtil.WriteRegValue(strSection, "MessageDBServerIP", m_szServerIP);
            }
            this.editMysqlServer.Text = m_szServerIP;
        }

        private string GetIP4Address()
        {
            string IP4Address = String.Empty;
            foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (IPA.AddressFamily.ToString() == "InterNetwork")
                {
                    IP4Address = IPA.ToString();
                    break;
                }
            }
            return IP4Address;
        }

	    private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSaveRegistry_Click(object sender, EventArgs e)
        {
            SMSDBManager.Instance.SaveConnectionInfo();
        }
          

        private void cmbMysqlEncoding_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cmbMysqlEncoding.SelectedItem != null)
            {
                string szText = (string)cmbMysqlEncoding.SelectedItem;
                if( szText != null)
                {
                    if(szText == "KSC5601")
                    {
                        m_szMysqlEncoding = "euckr";
                    }
                    else if(szText == "UTF-8")
                    {
                        m_szMysqlEncoding = "utf8";
                    }
                    else
                    {
                        m_szMysqlEncoding = "latin1";
                    }
                }
            }
        }

        private void btnMysqlConnect_Click(object sender, EventArgs e)
        {
            if (!SMSDBManager.Instance.IsConnect)
            {
                
                SMSDBManager.Instance.CharSet = m_szMysqlEncoding;
                SMSDBManager.Instance.Port = "3306";
                SMSDBManager.Instance.ServerIP = editMysqlServer.Text;

                if(SMSDBManager.Instance.Connect())
                {
                    cmbMysqlEncoding.Enabled = false;
                    editMysqlServer.Enabled = false;
                    btnMysqlConnect.Enabled = false;
                    btnMysqlDisconnect.Enabled = true;
                    lbStatusMysql.Text = "접속이 되었습니다.";
                    lbStatusMysql.ForeColor = Color.Green;
                }
            }
        }

        private void btnMysqlDisconnect_Click(object sender, EventArgs e)
        {
           // if (!SMSDBManager.Instance.IsConnect)
            {

                cmbMysqlEncoding.Enabled = true;
                editMysqlServer.Enabled = true;

                btnMysqlConnect.Enabled = true;
                btnMysqlDisconnect.Enabled = false;
                lbStatusMysql.Text = "접속되지 않았습니다.";
                lbStatusMysql.ForeColor = Color.Red;

                SMSDBManager.Instance.Close();
            }
        }

       

        private void btnSaveMsgToDB_Click(object sender, EventArgs e)
        {
            if( SMSDBManager.Instance.IsConnect)
            {
                string szSendMsg = editMsg.Text;
                if (szSendMsg == null || szSendMsg == "")
                    return;

                string szText = editMsgIter.Text;
                int nCount;

                if (int.TryParse(szText, out nCount))
                {
                    for (int i = 1; i <= nCount; i++)
                    {
                        MessageContent sms = new MessageContent();
                        
                        //Encoding enc = Encoding.GetEncoding(m_szMsgEncoding);
                        //byte[] bytes1 = Encoding.UTF8.GetBytes(szSendMsg);
                        //byte[] bytes2 = Encoding.Convert(Encoding.UTF8, enc, bytes1);
                        //string szMsg = enc.GetString(bytes2);
                        sms.Message = szSendMsg + "_" + i.ToString();
                        
                        sms.Caller = editCallback.Text;
                        sms.Reciver = editReciver.Text;

                        SMSDBManager.Instance.InsertMessage(sms);
                    }
                }		
            }
            else
            {
                MessageBox.Show("DB에 접속이 되어 있지 않습니다.");
            }
        }
    

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SMSDBManager.Instance.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string szText = SMSDBManager.Instance.CheckCharset();

            string [] szLine = szText.Split(new char[]{'\n','\r'});
            szText = "";
            for (int i = 0; i < szLine.Length; i++)
            {
                szText += szLine[i];
                szText += Environment.NewLine;
            }
            
            MessageBox.Show(szText);
        }

	}

	
}
