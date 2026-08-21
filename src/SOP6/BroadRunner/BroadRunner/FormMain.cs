using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility2;
using System.Collections;

namespace BroadRunner
{
    public partial class FormMain : Form
    {
        private WebDBManager m_dbMgr = null;
        private const string OPTION_TAG = "BROAD_RUNNER_INIT";

        //private int m_nSiteID = 1;
        private bool m_bReadHeartBeat = false;
        private int m_nReadState = -1;

        public enum SpeechState
        {
            ERROR = -1,
            STANDBY = 1,
            PLAY = 2,
            STOP = 3,
            PAUSE = 4,
            REPEAT = 5,
        }

        public FormMain(int nSiteID, string strDBName)
        {
            InitializeComponent();

            //m_nSiteID = nSiteID;

            m_dbMgr = new WebDBManager(nSiteID);
            m_dbMgr.DatabaseName = strDBName;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            timer1.Start();
            ReadInitText();
        }

        private void ReadInitText()
        {
            string strSQL = "Select PropertyValue from OptionSOPSimulator where PropertyName = '" + OPTION_TAG + "' and SiteID = " + m_dbMgr.SiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return;

            string strValue = WebDBManager.GetStringField(arrResult[0]);

            if (strValue == null)
                return;

            textBoxMessage.Text = strValue;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int nState = ReadHeartBeat();

            switch (nState)
            {
                case (int)SpeechState.ERROR:
                    labelStatus.Text = "연결 대기(방송을 시작할 수 없습니다.)";
                    labelStatus.ForeColor = System.Drawing.Color.Red;
                    //btnRun.Enabled = false;
                    break;
                case (int)SpeechState.STANDBY:
                case (int)SpeechState.STOP:
                    if (UseBroadcast())
                    {
                        labelStatus.Text = "정상 (대기)";
                        labelStatus.ForeColor = System.Drawing.Color.Green;
                        btnRun.Enabled = true;
                    }
                    else
                    {
                        labelStatus.Text = "사용안함([실행설정]창에서 사용으로 변경 가능)";
                        labelStatus.ForeColor = System.Drawing.Color.DarkRed;
                        btnRun.Enabled = false;
                    }
                    break;

                case (int)SpeechState.PLAY:
                case (int)SpeechState.PAUSE:
                case (int)SpeechState.REPEAT:
                    labelStatus.Text = "정상 (방송중)";
                    labelStatus.ForeColor = System.Drawing.Color.Blue;
                    //btnRun.Enabled = false;
                    break;
                default:
                    labelStatus.Text = "접속 확인중";
                    labelStatus.ForeColor = System.Drawing.Color.Black;
                    //;;b//tnRun.Enabled = false;
                    break;
            }

            labelStatus.Visible = true;
        }

        private bool UseBroadcast()
        {
            string strSQL = "Select PropertyValue from OptionSOPSimulator where PropertyName = 'UseBroadcast' and SiteID = " + m_dbMgr.SiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return false;

            VariousData<int> useBroadcast = WebDBManager.GetIntField(arrResult[0].ToString());

            if (useBroadcast == null)
                return false;

            return useBroadcast.Data == 1;
        }

        private int ReadHeartBeat()
        {
            int nResult = m_nReadState;
            if (m_dbMgr == null)
                return -1;

            if (m_bReadHeartBeat == false)
            {
                nResult = -1;

                m_bReadHeartBeat = true;
                //string szSQL = "SELECT HOSTADDRESS, HEARTBEAT, BSTATE, BDescription from BroadcastState where id = 1";
                string szText = "SELECT HOSTADDRESS, HEARTBEAT, BSTATE, BDescription FROM BroadcastState " +
                                " WHERE id in (SELECT min(id) FROM BroadcastState WHERE SiteID = {0})";

                string szSQL = string.Format(szText, m_dbMgr.SiteID);
                ArrayList arResult = m_dbMgr.GetResultData(szSQL);

                if (arResult == null)
                {
                    m_bReadHeartBeat = false;
                    return -1;
                }

                DateTime nDate = DateTime.Now;

                int i = 0;
                if (arResult.Count == 4)
                {
                    DateTime nLast = WebDBManager.GetDateTimeField(arResult[i + 1], nDate);

                    m_nReadState = WebDBManager.GetIntField(arResult[i + 2].ToString(), -1);

                    TimeSpan nInt = nDate - nLast;

                    if (nInt.TotalSeconds > 60)
                    {
                        nResult = -1;
                    }
                    else
                    {
                        nResult = m_nReadState;
                    }
                }
                m_bReadHeartBeat = false;
            }
            return nResult;
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            string strMessage = textBoxMessage.Text.Trim();

            if (strMessage.Length == 0)
                return;

            string strFormat = "Insert into Broadcast (Text, UseSiren, PlayOption, RepeatCount, AddTime, SiteID) ";
            strFormat += "values ('{0}', {1}, {2}, {3}, '{4} {5:00}:{6:00}:{7:00}', {8})";

            DateTime dtNow = DateTime.Now;
            string strSQL = string.Format(strFormat, strMessage, checkBoxUseSiren.Checked ? 1 : 0, 1, 1, dtNow.ToShortDateString(), dtNow.Hour, dtNow.Minute, dtNow.Second, m_dbMgr.SiteID);

            if (m_dbMgr.GetResultData(strSQL) == null)
                return;

            strSQL = "Select PropertyValue from OptionSOPSimulator where PropertyName = '" + OPTION_TAG + "' and SiteID = " + m_dbMgr.SiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            if (arrResult.Count > 0)
            {
                strSQL = "Update OptionSOPSimulator set PropertyValue = '" + strMessage + "' where PropertyName = '" + OPTION_TAG + "' and SiteID = " + m_dbMgr.SiteID.ToString();
                m_dbMgr.GetResultData(strSQL);
            }
            else
            {
                strSQL = "Insert into OptionSOPSimulator (PropertyName, PropertyValue, SiteID, Description) values ";
                strSQL += string.Format("('{0}', '{1}', {2}, NULL)", OPTION_TAG, strMessage, m_dbMgr.SiteID);

                m_dbMgr.GetResultData(strSQL);
            }

            btnRun.Enabled = false;
        }
    }
}
