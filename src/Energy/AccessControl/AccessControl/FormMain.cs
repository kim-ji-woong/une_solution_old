using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility2;

namespace AccessControl
{
    public partial class FormMain : Form
    {
        private NpgsqlManager m_npgsqlMgr = null;
        private WebDBManager m_dbMgr = null;
        private Timer m_timer = null;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            string npgSqlConnString = AccessControl.Properties.Settings.Default.npgSqlConnString;
            if (npgSqlConnString.Length == 0)
            {
                MessageBox.Show("ConnString 입력하기");
                this.Close();
            }

            m_npgsqlMgr = new NpgsqlManager();
            m_npgsqlMgr.ConnString = npgSqlConnString;
            m_npgsqlMgr.Open();

            // 처음 실행하면 00시부터
            DateTime dtNow = DateTime.Now;
            m_dtPrev = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, 0, 0, 0);

            m_dbMgr = new WebDBManager(3);

            m_timer = new Timer();
            m_timer.Interval = 1000;
            m_timer.Tick += M_timer_Tick;
            m_timer.Start();
        }

        private void M_timer_Tick(object sender, EventArgs e)
        {
            DateTime dtNow = DateTime.Now;
            if (dtNow.Hour == 23 && dtNow.Minute == 59 && dtNow.Second == 59)
            {
                DateTime temp = m_dtPrev.AddDays(1);
                m_dtPrev = new DateTime(temp.Year, temp.Month, temp.Day, 0, 0, 0);
            }

            DisplayUser();
        }

        private int m_nLastLogIndex = 0;
        private DateTime m_dtPrev = new DateTime();

        public void DisplayUser()
        {
            if (!m_npgsqlMgr.Connect)
                return;

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT u.employee_user_id, u.name, u.company, u.location, u.department, u.position, event_time, log.id as log_id, door_name ");
            sb.Append("  FROM public.tbl_user_info as u, public.tbl_access_log as log ");
            sb.Append(" WHERE u.employee_user_id=log.employee_user_id  ");
            sb.AppendFormat(" AND event_time > '{0}' ", GetTimeString(m_dtPrev));
            sb.AppendFormat(" AND log.id > " + m_nLastLogIndex);
            sb.Append(" ORDER BY log_id ");

            ArrayList arrResult = m_npgsqlMgr.GetResultData(sb.ToString());
            if (arrResult == null || arrResult.Count == 0)
                return;
            
            for (int i = 0; i < arrResult.Count; i += 9)
            {
                string strUserID = arrResult[i].ToString();
                string strName = arrResult[i + 1].ToString();
                string strCompany = arrResult[i + 2].ToString();
                string strLocation = arrResult[i + 3].ToString();
                string strDepartment = arrResult[i + 4].ToString();
                string strPosition = arrResult[i + 5].ToString();
                string strEventTime = arrResult[i + 6].ToString();
                string strLogIndex = arrResult[i + 7].ToString();
                string strDoorName = arrResult[i + 8].ToString();

                m_nLastLogIndex = Convert.ToInt32(strLogIndex);

                textBox1.Text += "[" + strEventTime + "] " + strName + " : " + strDoorName + "\r\n";

                string strSQL = string.Format("INSERT INTO access_log (ID, employee_user_id, Name, Company, Location, Department, Position, EventTime, Door_Name) VALUES ({0},'{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')"
                    , strLogIndex, strUserID, strName, strCompany, strLocation, strDepartment, strPosition, strEventTime, strDoorName);

                ArrayList arrResult2 = m_dbMgr.GetResultData("Select ID From access_log Where ID = " + strLogIndex);
                if (arrResult2 != null && arrResult2.Count == 0)
                    m_dbMgr.GetResultData(strSQL);
            }
        }

        private string GetTimeString(DateTime timeStamp)
        {
            string strTime = string.Format("{0:0000}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", timeStamp.Year, timeStamp.Month, timeStamp.Day, timeStamp.Hour, timeStamp.Minute, timeStamp.Second);
            return strTime;
        }
    }
}
