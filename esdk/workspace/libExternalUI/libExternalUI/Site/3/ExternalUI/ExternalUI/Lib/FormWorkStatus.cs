using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace libExternalUI.Lib
{
    public partial class FormWorkStatus : Form
    {
        private int m_nLastLogIndex = 0;
        private DateTime m_dtPrev = new DateTime();

        private int m_nHiCount = 0;
        public int HiCount
        {
            get { return m_nHiCount; }
        }
        private int m_nByeCount = 0;
        public int ByeCount
        {
            get { return m_nByeCount; }
        }
        private int m_nStayCount = 0;
        public int StayCount
        {
            get { return m_nStayCount; }
        }

        public FormWorkStatus()
        {
            InitializeComponent();

            this.TopLevel = false;

            btnClose.ImageNormal = global::libExternalUI.Properties.Resources.close_Normal;
            btnClose.ImageClicked = global::libExternalUI.Properties.Resources.close_Click;
            btnClose.ImageMouseOver = global::libExternalUI.Properties.Resources.close_MouseOver;

            // 처음 실행하면 00시부터
            DateTime dtNow = DateTime.Now;
            m_dtPrev = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, 0, 0, 0);
        }

        private Dictionary<string, Worker> m_dicWorkers = new Dictionary<string, Worker>();
        public void DisplayWorkStatus()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT employee_user_id, name, eventtime, ID, door_name ");
            sb.Append("  FROM access_log ");
            sb.AppendFormat(" WHERE eventtime > '{0}' ", GetTimeString(m_dtPrev));
            sb.AppendFormat(" AND ID > " + m_nLastLogIndex);
            sb.Append(" ORDER BY ID");

            ArrayList arrResult = UIManager.Instance.DBMgr.GetResultData(sb.ToString());

            if (arrResult == null || arrResult.Count == 0)
                return;

            for (int i = 0; i < arrResult.Count; i += 5)
            {
                string strUserID = DBUtility2.WebDBManager.GetStringField(arrResult[i]);
                string strName = DBUtility2.WebDBManager.GetStringField(arrResult[i + 1]);                
                string strEventTime = DBUtility2.WebDBManager.GetStringField(arrResult[i + 2]);
                string strLogIndex = DBUtility2.WebDBManager.GetStringField(arrResult[i + 3]);
                string strDoorName = DBUtility2.WebDBManager.GetStringField(arrResult[i + 4]);

                m_nLastLogIndex = Convert.ToInt32(strLogIndex);

                bool isBye = false;
                bool err = false;

                long nEventTime = Convert.ToDateTime(strEventTime).ToFileTime();
                if (strDoorName == "출근기")
                {
                    if (m_dicWorkers.ContainsKey(strUserID))
                    {
                        if (m_dicWorkers[strUserID].HiTime < nEventTime) // 이미 출근 기록이 있음
                            err = true;
                    }
                    else
                    {
                        Worker worker = new Worker();
                        worker.Name = strName;
                        worker.UserID = strUserID;
                        worker.InWork = true;
                        worker.HiTime = nEventTime;
                        m_dicWorkers[strUserID] = worker;
                    }
                }
                else if (strDoorName == "퇴근기")
                {
                    if (m_dicWorkers.ContainsKey(strUserID))
                    {
                        m_dicWorkers[strUserID].InWork = false;
                        if (m_dicWorkers[strUserID].ByeTime > 0)
                        {
                            if (m_dicWorkers[strUserID].ByeTime < nEventTime)
                                m_dicWorkers[strUserID].ByeTime = nEventTime;

                            err = true;
                        }
                        else
                        {
                            m_dicWorkers[strUserID].ByeTime = nEventTime;
                            isBye = true;
                        }
                    }
                    else // 출근기록이 없는데 퇴근을 찍으면 에러임
                    {
                        err = true;
                    }
                }

                //long nEventTime = Convert.ToDateTime(strEventTime).ToFileTime();

                //if (m_dicWorkers.ContainsKey(strUserID))
                //{
                //    if (m_dicWorkers[strUserID].HiTime < nEventTime)
                //    {
                //        if (m_dicWorkers[strUserID].HiTime > 0 && m_dicWorkers[strUserID].ByeTime > 0)
                //        {
                //            // 출퇴근 기록이 이미 있는데 또 찍은경우 제외
                //            err = true;
                //        }

                //        m_dicWorkers[strUserID].InWork = false;
                //        m_dicWorkers[strUserID].ByeTime = nEventTime;
                //        isBye = true;
                //    }
                //}
                //else
                //{
                //    Worker worker = new Worker();
                //    worker.Name = strName;
                //    worker.UserID = strUserID;

                //    worker.InWork = true;
                //    worker.HiTime = nEventTime;

                //    m_dicWorkers[strUserID] = worker;
                //}

                // 출근 로그가 찍힌 데이터만 들어오므로 무조건 +
                if (!err)
                {
                    if (!isBye)
                        m_nHiCount++;
                    else
                        m_nByeCount++; 
                }

                m_nStayCount = m_nHiCount - m_nByeCount;
            }

            lblHi.Text = m_nHiCount.ToString();
            lblBye.Text = m_nByeCount.ToString();
            lblStay.Text = m_nStayCount.ToString();
        }

        private string GetTimeString(DateTime timeStamp)
        {
            string strTime = string.Format("{0:0000}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", timeStamp.Year, timeStamp.Month, timeStamp.Day, timeStamp.Hour, timeStamp.Minute, timeStamp.Second);
            return strTime;
        }

        public void Init()
        {
            DateTime temp = m_dtPrev.AddDays(1);
            m_dtPrev = new DateTime(temp.Year, temp.Month, temp.Day, 0, 0, 0);

            m_nHiCount = m_nByeCount = m_nStayCount = 0;
            lblHi.Text = "0";
            lblBye.Text = "0";
            lblStay.Text = "0";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            //SDMS.FormMain.TransferExternalForm((int)2);
            libExternalUI.Lib.UIManager.TransferExternalForm((int)2);
            this.Hide();
        }
    }

    public class Worker
    {
        // DB ID
        private int m_nID = 0;
        // 사번
        private string m_strUserID = "";
        public string UserID
        {
            get { return m_strUserID; }
            set { m_strUserID = value; }
        }

        private string m_strName = "";
        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }
        // 작업장에서 작업중인가?
        private bool m_inWork = false;
        public bool InWork
        {
            get { return m_inWork; }
            set { m_inWork = value; }
        }
        private long m_nHiTime = -1;
        private long m_nByeTime = -1;

        public long HiTime
        {
            get { return m_nHiTime; }
            set { m_nHiTime = value; }
        }

        public long ByeTime
        {
            get { return m_nByeTime; }
            set { m_nByeTime = value; }
        }
    }
    }
