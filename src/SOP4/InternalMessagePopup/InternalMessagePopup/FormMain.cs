using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using DBUtility;

namespace InternalMessagePopup
{
    public partial class FormMain : Form
    {
        private int m_nBroadcastPos = 206;
        private int m_nSMSPos = 364;
        private int m_nX = 0, m_nY = 0;
        private int m_nProcessID = 0;

        private int m_nBroadcastHistoryID = -1;
        private int m_nSiteID = 2;
        private WebDBManager m_dbMgr = null;
        private int m_nLatestSeconds = 3;
        private string m_strBroadcastMessage = "", m_strSMSMessage = "";
        private double m_dBroadcastSpan = -1.0, m_dSMSSpan = -1.0;
        private DateTime m_dtLastShow = new DateTime();
        private int m_nShowTimeSeconds = 3;
        private bool m_initLocation = false;

        string m_strConfig = "InternalConfig.txt";

        public FormMain(int x, int y, int nProcessID)
        {
            InitializeComponent();
            ReadXY(ref x, ref y);

            m_nX = x + 45;
            m_nY = y + 189;
            m_nProcessID = nProcessID;
            m_dbMgr = new WebDBManager(m_nSiteID);
        }

        private void ReadXY(ref int x, ref int y)
        {
            if (System.IO.File.Exists(m_strConfig))
            {
                try
                {
                    System.IO.StreamReader reader = new System.IO.StreamReader(m_strConfig);
                    string strLine = reader.ReadLine().Trim();
                    string[] tokens = strLine.Split(' ');

                    x = int.Parse(tokens[0].Trim());
                    y = int.Parse(tokens[1].Trim());
                    reader.Close();
                }
                catch (Exception)
                {

                }
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            this.Location = new Point(-10000, -10000);
            //this.Location = new Point(m_nX, m_nY);
            //this.Size = new Size(1852, 839);

            this.BringToFront();

            timer1.Start();
        }

        private bool GetLatestBroadcastMessage(string strSQL)
        {
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            int nDataCount = 4;

            if (arrResult == null || arrResult.Count < nDataCount)
                return false;

            int nIndex = (arrResult.Count / nDataCount - 1) * nDataCount;

            VariousData<DateTime> currentTime = WebDBManager.GetDateTimeField(arrResult[nIndex]);
            VariousData<int> id = WebDBManager.GetIntField(arrResult[nIndex + 1].ToString());
            string strText = WebDBManager.GetStringField(arrResult[nIndex + 2]);
            VariousData<DateTime> addTime = WebDBManager.GetDateTimeField(arrResult[nIndex + 3]);

            if (currentTime == null || id == null || strText == null || addTime == null)
                return false;

            m_nBroadcastHistoryID = id.Data;
            TimeSpan span = currentTime.Data - addTime.Data;

            /*if (span.TotalSeconds > (double)m_nLatestSeconds)
                return false;*/

            m_strBroadcastMessage = strText;
            m_dBroadcastSpan = span.TotalSeconds;
            return true;
        }

        private bool GetLatestBroadcastMessage()
        {
            m_dBroadcastSpan = -1.0;
            string strSQL = "";

            if (m_nBroadcastHistoryID < 0)
            {
                strSQL = "Select max(ID) from broadcasthistory where SiteID = " + m_nSiteID.ToString();
                ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

                if (arrResult == null || arrResult.Count == 0)
                    return false;

                int nMaxID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);

                if (nMaxID < 0)
                    return false;

                strSQL = "Select getdate(), ID, Text, AddTime from broadcasthistory where ID = " + nMaxID.ToString();
            }
            else
                strSQL = "Select getdate(), ID, Text, AddTime from broadcasthistory where SiteID = 2 and ID >= " + m_nBroadcastHistoryID.ToString();

            return GetLatestBroadcastMessage(strSQL);
        }

        private bool GetLatestSMSMessage()
        {
            m_dSMSSpan = -1.0;

            string strSQL = "Select getdate(), PropertyValue from OptionSOPSimulator where PropertyName = 'LastSMSMessage' and SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            int nDataCount = 2;

            if (arrResult == null || arrResult.Count < nDataCount)
                return false;

            int nDataIndex = (arrResult.Count / nDataCount - 1) * nDataCount;

            VariousData<DateTime> currentTime = WebDBManager.GetDateTimeField(arrResult[nDataIndex]);
            string strResult = WebDBManager.GetStringField(arrResult[nDataIndex + 1]);

            if (strResult == null || currentTime == null)
                return false;

            int nIndex = strResult.IndexOf(',');

            if (nIndex < 0)
                return false;

            string strTime = strResult.Substring(0, nIndex);
            string strMessage = strResult.Substring(nIndex + 1);
            VariousData<DateTime> addTime = WebDBManager.GetDateTimeField(strTime);

            TimeSpan span = currentTime.Data - addTime.Data;

            /*if (span.TotalSeconds > (double)m_nLatestSeconds)
                return false;*/

            m_strSMSMessage = strMessage;
            m_dSMSSpan = span.TotalSeconds;
            return true;
        }

        // 호출한 process가 비정상적으로 종료되지 않았는지 검사하기 위한 Timer
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_nProcessID == 0)
                return;

            System.Diagnostics.Process process = null;

            try
            {
                process = System.Diagnostics.Process.GetProcessById(m_nProcessID);
            }
            catch (Exception)
            {
                process = null;
            }

            if (process == null)
            {
                timer1.Stop();
                this.Close();
                return;
            }

            bool smsVisible = !IsClosingTime(true);
            bool broadcastVisible = !IsClosingTime(false);

            if (smsVisible == false && broadcastVisible == false)
            {
                HideAll();
                return;
            }

            bool broadcast = broadcastVisible ? GetLatestBroadcastMessage() : false;
            bool sms = smsVisible ? GetLatestSMSMessage() : false;
            
            if (broadcast && sms)
            {
                if (m_dBroadcastSpan < m_dSMSSpan)
                    ShowBroadcast();
                else
                    ShowSMS();
            }
            else if (broadcast)
                ShowBroadcast();
            else if (sms)
                ShowSMS();
            //else if (IsClosingTime())
            //    HideAll();

            if (m_initLocation == false)
            {
                this.Location = new Point(m_nX, m_nY);
                m_initLocation = true;
            }
        }

        private void HideAll()
        {
            if (this.Visible == true)
                this.Hide();
        }

        private bool IsClosingTime(bool sms)
        {
            string strOption = sms ? "InternalMessagePopupSMSOnOff" : "InternalMessagePopupBroadcastOnOff";
            string strSQL = "Select PropertyValue from OptionSOPSimulator where PropertyName = '" + strOption + "' and SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return true;

            string strValue = WebDBManager.GetStringField(arrResult[0]);

            if (strValue == null)
                return true;

            if (strValue == "1")
                return false;
            /*TimeSpan span = DateTime.Now - m_dtLastShow;

            if (span.TotalSeconds < (double)m_nShowTimeSeconds)
                return false;*/

            return true;
        }

        private void ShowBroadcast()
        {
            pictureBoxWorkType.BackgroundImage = global::InternalMessagePopup.Properties.Resources.broadcast;
            labelWorkType.Text = "방송실행";

            labelReceiver.Hide();
            labelReceiverName.Hide();

            textBoxMessage.Text = m_strBroadcastMessage;
            this.Show();

            m_dtLastShow = DateTime.Now;
        }

        private void ShowSMS()
        {
            pictureBoxWorkType.BackgroundImage = global::InternalMessagePopup.Properties.Resources.broadcast;
            labelWorkType.Text = "문자발송";

            labelReceiver.Show();
            labelReceiverName.Text = "영흥화력본부";
            labelReceiverName.Show();

            textBoxMessage.Text = m_strSMSMessage;
            this.Show();

            m_dtLastShow = DateTime.Now;
        }

        private void textBoxMessage_EnabledChanged(object sender, EventArgs e)
        {
            textBoxMessage.BackColor = Color.White;
        }
    }
}
