using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility;
using System.Collections;

namespace EarthquakeSensorServer
{
    public partial class FormMain : Form
    {
        private bool m_closing = false;
        private bool m_safeClose = false;

        private int m_nSiteID = 1;
        private Network.NetworkManager m_netMgr = null;
        private WebDBManager m_dbMgr = null;
        private WebDBManager m_dbJubix = null;

        private string m_strPrevIgnoreMinute = "";
        private bool m_isSystemInput = false;

        private string m_strLastReadTime = "";
        // 마지막에 읽은 진도
        private int m_nLastReadIntensity = 0;

        private const int MAX_ROW_COUNT = 500;
        private static FormMain m_instance = null;

        public static FormMain Instance
        {
            get { return m_instance; }
        }

        public FormMain()
        {
            InitializeComponent();
            labelErrorMessage.Text = "";
            labelErrorMessage.Visible = false;

            m_instance = this;

            m_strPrevIgnoreMinute = textBoxIgnoreMinute.Text;

            ReadSiteID();

            m_dbMgr = new WebDBManager(m_nSiteID);
            SetJubixDB();

            m_netMgr = new Network.NetworkManager(m_nSiteID, m_dbMgr);

            RunThread();
        }

        private void SetJubixDB()
        {
            m_dbJubix = new WebDBManager(m_nSiteID);

            Utility ini = new Utility();
            string strSection = "Jubix Connection Info";
            string strServerIP = ini.getinivalue(strSection, "server_ip");
            string strServerPort = ini.getinivalue(strSection, "server_port");
            string strServerDB = ini.getinivalue(strSection, "server_db");

            m_dbJubix.DatabaseHost = strServerIP;
            m_dbJubix.WebServerURL = "http://127.0.0.1:8080/JUBIX";
            m_dbJubix.DatabaseType = WebDBManager.DBType.mysql;
            m_dbJubix.DatabaseName = strServerDB;
            m_dbJubix.DatabasePort = strServerPort;
        }

        private void ReadSiteID()
        {
            DBUtility.Utility util = new DBUtility.Utility();
            string szSiteID = util.getinivalue("Server Connection Info", "siteid");
            if (szSiteID == null || szSiteID == "")
            {
                MessageBox.Show("Site ID가 지정되지 않았습니다. ini파일을 확인하세요", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }

            if (!int.TryParse(szSiteID, out m_nSiteID))
            {
                MessageBox.Show("잘못된 Site ID입니다.. ini파일을 확인하세요", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }
        }

        private void RunThread()
        {
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(SendThread));
            t.Start();
        }

        private void SendThread()
        {
            while (m_closing == false)
            {
                EarthquakeData data = ReadData();

                if (data != null && data.Intensity > 1)
                {
                    m_netMgr.SendEarthquakeSignal(1, data.Magnitude, data.Intensity, data.Location, data.TimeStamp);
                    System.Diagnostics.Trace.WriteLine("SendAlarm : " + data.Intensity.ToString() + ", " + data.Location + ", " + data.TimeStamp.ToString());
                }

                System.Threading.Thread.Sleep(1000);
            }

            m_safeClose = true;
        }

        private EarthquakeData ReadData()
        {
            string strSQL = "Select Dev_Date, Dev_SS_ID, Dev_Shindo, Dev_TPGA from r_ss_eq_dat where Dev_Date = (Select max(Dev_Date) from r_ss_eq_dat) and Dev_Date > '" + m_strLastReadTime + "'";
            ArrayList arrResult = m_dbJubix.GetResultData(strSQL, 0);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;

            if (nResultCount < 4)
                return null;

            string strTime = WebDBManager.GetStringField(arrResult[0]);
            string strSensorID = WebDBManager.GetStringField(arrResult[1]);
            VariousData<int> intensity = WebDBManager.GetIntField(arrResult[2].ToString());
            string strTPGA = WebDBManager.GetStringField(arrResult[3]);

            if (strTime == null || strSensorID == null || intensity == null || strTPGA == null)
                return null;

            if (m_strLastReadTime.Length == 0)
                return AddGrid(strTime, intensity.Data, strTPGA);
            else if (intensity.Data > 0 || m_nLastReadIntensity > 0)
            {
                if (intensity.Data <= m_nLastReadIntensity)
                {
                    if (m_strLastReadTime.Length != 14 || strTime.Length != 14)
                        return AddGrid(strTime, intensity.Data, strTPGA);
                    else
                    {
                        double ignoreTime = GetIgnoreTime();
                        double diff = GetDiff(m_strLastReadTime, strTime);

                        if (diff >= ignoreTime)
                            return AddGrid(strTime, intensity.Data, strTPGA);
                    }
                }
                else
                    return AddGrid(strTime, intensity.Data, strTPGA);
            }

            m_nLastReadIntensity = intensity.Data;
            m_strLastReadTime = strTime;
            return null;
        }

        private EarthquakeData AddGrid(string strTime, int nIntensity, string strTPGA, string strLocation = "")
        {
            EarthquakeData data = new EarthquakeData();

            data.Intensity = nIntensity;
            data.Location = strLocation;

            if (strTime.Length == 14)
                data.TimeStamp = ToDateTime(strTime);

            AddGridRow(strTime, nIntensity, strTPGA, strLocation);

            m_nLastReadIntensity = nIntensity;
            m_strLastReadTime = strTime;

            return data;
        }

        private void AddGridRow(string strTime, int nIntensity, string strTPGA, string strLocation)
        {
            this.Invoke((MethodInvoker)delegate
            {
                // MAX_ROW_COUNT 이상은 늘어나지 않도록 한다.
                if (gridLog.Rows.Count >= MAX_ROW_COUNT)
                {
                    gridLog.Rows.RemoveAt(0);

                    for (int i = 0; i < gridLog.Rows.Count; i++)
                    {
                        gridLog.Rows[i].Cells[0].Value = i + 1;
                    }
                }

                int nRowIndex = gridLog.Rows.Add();

                if (nRowIndex >= 0)
                {
                    DataGridViewRow row = gridLog.Rows[nRowIndex];
                    int nNo = gridLog.Rows.Count;

                    row.Cells[0].Value = nNo;
                    row.Cells[1].Value = strLocation;
                    row.Cells[2].Value = strTime;
                    row.Cells[4].Value = nIntensity;
                    row.Cells[5].Value = strTPGA;
                }
            });
        }

        // 두 시간의 차이(분)
        private double GetDiff(string strPrevTime, string strCurrentTime)
        {
            DateTime dtPrev = ToDateTime(strPrevTime);
            DateTime dtCurrent = ToDateTime(strCurrentTime);
            return (dtCurrent - dtPrev).TotalMinutes;
        }

        private DateTime ToDateTime(string strTime)
        {
            string strYear = strTime.Substring(0, 4);
            string strMonth = strTime.Substring(4, 2);
            string strDay = strTime.Substring(6, 2);
            string strHour = strTime.Substring(8, 2);
            string strMin = strTime.Substring(10, 2);
            string strSec = strTime.Substring(12, 2);

            int year, month, day, hour, min, sec;

            if (int.TryParse(strYear, out year) && int.TryParse(strMonth, out month) && int.TryParse(strDay, out day) &&
                int.TryParse(strHour, out hour) && int.TryParse(strMin, out min) && int.TryParse(strSec, out sec))
                return new DateTime(year, month, day, hour, min, sec);

            return new DateTime();
        }

        private double GetIgnoreTime()
        {
            double ignoreTime = 0.0;
            double.TryParse(textBoxIgnoreMinute.Text.Trim(), out ignoreTime);

            if (ignoreTime < 0.0)
                return 0.0;

            return ignoreTime;
        }

        private void textBoxIgnoreMinute_TextChanged(object sender, EventArgs e)
        {
            if (m_isSystemInput)
                return;

            double minute = 0.0;

            if (double.TryParse(textBoxIgnoreMinute.Text.Trim(), out minute) == false || minute < 0.0)
            {
                MessageBox.Show("0 또는 그 보다 큰 숫자를 입력해야 합니다.");

                m_isSystemInput = true;
                textBoxIgnoreMinute.Text = m_strPrevIgnoreMinute;
                m_isSystemInput = false;

                return;
            }

            m_strPrevIgnoreMinute = textBoxIgnoreMinute.Text;
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_closing = true;
            m_netMgr.ReleaseThread();

            while (m_safeClose == false)
            {
                System.Threading.Thread.Sleep(1000);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSimpleInput_Click(object sender, EventArgs e)
        {
            FormEarthquakeSimulation frm = new FormEarthquakeSimulation();
            
            if (frm.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                if (frm.Intensity > 0)
                {
                    DateTime dtNow = DateTime.Now;
                    string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);
                    AddGridRow(strTime, frm.Intensity, "", frm.EarthLocation);

                    m_netMgr.SendEarthquakeSignal(1, 0, frm.Intensity, frm.EarthLocation, dtNow);
                    System.Diagnostics.Trace.WriteLine("SendAlarmManual : " + frm.Intensity.ToString() + ", " + frm.EarthLocation + ", " + strTime);
                }
            }
        }
    }
}
