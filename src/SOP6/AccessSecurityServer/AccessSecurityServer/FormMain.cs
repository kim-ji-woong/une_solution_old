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

namespace AccessSecurityServer
{
    public partial class FormMain : Form, AccessWatcherOwner, LocationManagerOwner
    {
        private int m_nSiteID = 1;
        private WebDBManager m_dbMgr = null;
        private AccessWatcher m_watcher = null;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            labelStatus.Text = "";
            m_nSiteID = NetworkWebManager.Instance.SiteID;

            //if (ReadSiteID())
            {
                m_dbMgr = NetworkWebManager.Instance.DBManager;
                //m_dbMgr = new WebDBManager(m_nSiteID);
                string strConnectionInfo = ReadAccessDBConnectionInfo();
                
                string[] tokens = strConnectionInfo.Split(';');

                if (tokens.Count() >= 5)
                {
                    string strDBType = tokens[0].Trim();
                    string strServerURL = tokens[1].Trim();
                    string strDatabaseName = tokens[2].Trim();
                    string strUserName = tokens[3].Trim();
                    string strPassword = tokens[4].Trim();

                    if (strDBType.ToUpper() == "SQLSERVER")
                        m_watcher = new AccessWatcher_SQLServer(this, m_dbMgr, m_nSiteID);

                    if (m_watcher != null)
                    {
                        m_watcher.ServerURL = strServerURL;
                        m_watcher.DatabaseName = strDatabaseName;
                        m_watcher.UserName = strUserName;
                        m_watcher.Password = strPassword;

                        NetworkWebManager.Instance.LocationManagerOwner = this;
                        m_watcher.Run();
                    }
                }
            }
        }

        private string ReadAccessDBConnectionInfo()
        {
            string strSQL = "Select PropertyValue from OptionSDMS where PropertyName = 'AccessSecurityDBConnection' and SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return null;

            string strConnectionInfo = WebDBManager.GetStringField(arrResult[0]);
            return strConnectionInfo;
        }

        /*private bool ReadSiteID()
        {
            DBUtility.Utility util = new DBUtility.Utility();
            string szSiteID = util.getinivalue("Server Connection Info", "siteid");
            if (szSiteID == null || szSiteID == "")
            {
                UnE.Utility.UMessageBox.Show("Site ID가 지정되지 않았습니다. ini파일을 확인하세요", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            int nSiteId = 1;

            if (int.TryParse(szSiteID, out nSiteId))
            {
                m_nSiteID = nSiteId;
            }
            else
            {
                UnE.Utility.UMessageBox.Show("잘못된 Site ID입니다.. ini파일을 확인하세요", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }*/

        private string GetTimeString(DateTime time)
        {
            return string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);
        }

        public void AddAlarm(Alarm alarm)
        {
            if (alarm.Device == null)
                return;

            // 기기의 알람상태를 기록한다.
            alarm.Device.AlarmState = alarm.AlarmState;

            this.Invoke((MethodInvoker)delegate
            {
                int nRowIndex = gridLog.Rows.Add();

                if (nRowIndex < 0)
                    return;

                DataGridViewRow row = gridLog.Rows[nRowIndex];
                row.Tag = alarm.Device;

                row.Cells[0].Value = gridLog.Rows.Count;
                row.Cells[1].Value = alarm.EventTime == null ? "" : GetTimeString(alarm.EventTime.Data);
                row.Cells[2].Value = alarm.Device.ID;
                row.Cells[3].Value = Alarm.ToAlarmString(alarm.AlarmState);                
                row.Cells[4].Value = alarm.GetLocationName();
                row.Cells[5].Value = alarm.Content1;
                row.Cells[6].Value = alarm.Content2;
                row.Cells[7].Value = alarm.Content3;
                row.Cells[8].Value = alarm.Content4;

                NetworkWebManager.Instance.SendAlarm(alarm);
            });
        }

        public void RemoveAlarm(Device device)
        {
            if (device == null)
                return;

            try
            {
                this.Invoke((MethodInvoker)delegate
                {
                    int nRowCount = gridLog.Rows.Count;

                    for (int i = 0; i < nRowCount; i++)
                    {
                        // 삭제한 행 이후는 번호를 하나씩 줄인다.
                        if (device == null)
                        {
                            DataGridViewRow row = gridLog.Rows[i - 1];
                            row.Cells[0].Value = (int)row.Cells[0].Value - 1;
                        }
                        else
                        {
                            DataGridViewRow row = gridLog.Rows[i];

                            if (row.Tag == device)
                            {
                                gridLog.Rows.RemoveAt(i);
                                NetworkWebManager.Instance.SendAlarmClear(device);
                                device = null;
                            }
                        }
                    }
                });
            }
            catch (Exception)
            {
            }
        }
        /*public void RemoveAlarm(Alarm alarm, Alarm.StateType prevAlarmState)
        {
            if (alarm == null)
                return;

            this.Invoke((MethodInvoker)delegate
            {
                int nRowCount = gridLog.Rows.Count;

                for (int i = 0; i < nRowCount; i++)
                {
                    // 삭제한 행 이후는 번호를 하나씩 줄인다.
                    if (alarm == null)
                    {
                        DataGridViewRow row = gridLog.Rows[i - 1];
                        row.Cells[0].Value = (int)row.Cells[0].Value - 1;
                    }
                    else
                    {
                        DataGridViewRow row = gridLog.Rows[i];

                        if (row.Tag == alarm)
                        {
                            gridLog.Rows.RemoveAt(i);
                            // 알람상태가 초기화되었으므로 이전 알람상태를 입력시킨다.
                            alarm.AlarmState = prevAlarmState;
                            NetworkManager.Instance.SendAlarm(alarm, 0);
                            alarm = null;
                        }
                    }
                }
            });
        }*/

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_watcher != null)
                m_watcher.Close();

            NetworkWebManager.Instance.ReleaseThread();
        }

        public void SetStatus(string strStatus, System.Drawing.Color fontColor)
        {
            this.Invoke((MethodInvoker)delegate
            {
                labelStatus.ForeColor = fontColor;
                labelStatus.Text = strStatus;
            });
        }

        private void btnUpdateLocation_Click(object sender, EventArgs e)
        {
            btnUpdateLocation.Enabled = false;
            LocationManager.Instance.CheckLocation(NetworkWebManager.Instance.AccessDBConnectionString, NetworkWebManager.Instance.DBManager, this);
            btnUpdateLocation.Enabled = true;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnTestAlarm_Click(object sender, EventArgs e)
        {
            m_watcher.MakeTestAlarm();
        }

        private void btnClearTestAlarm_Click(object sender, EventArgs e)
        {
            if (gridLog.SelectedCells.Count == 0)
                return;

            DataGridViewRow row = gridLog.SelectedCells[0].OwningRow;
            Device device = (Device)row.Tag;

            m_watcher.RemoveAlarm(device);
            //m_watcher.ClearTestAlarm();
        }
    }
}
