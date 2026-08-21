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

namespace ExternalFireSensorDBWatcher
{
    public partial class FormMain : Form, SensorWatcherOwner
    {
        private int m_nSiteID = 1;
        private WebDBManager m_dbMgr = null;
        private FireSensorDBWatcher m_watcher = null;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            m_nSiteID = NetworkWebManager.Instance.DBManager.SiteID;
            m_dbMgr = NetworkWebManager.Instance.DBManager;

            if (m_dbMgr != null)
            //if (ReadSiteID())
            {
                //m_dbMgr = new WebDBManager(m_nSiteID);
                string strConnectionInfo = ReadFireSensorDBConnectionInfo();

                string[] tokens = strConnectionInfo.Split(';');

                if (tokens.Count() >= 5)
                {
                    string strDBType = tokens[0].Trim();
                    string strServerURL = tokens[1].Trim();
                    string strDatabaseName = tokens[2].Trim();
                    string strUserName = tokens[3].Trim();
                    string strPassword = tokens[4].Trim();

                    if (strDBType.ToUpper() == "SQLSERVER")
                        m_watcher = new FireSensorDBWatcher_SQLServer(this, m_dbMgr, m_nSiteID);

                    if (m_watcher != null)
                    {
                        m_watcher.ServerURL = strServerURL;
                        m_watcher.DatabaseName = strDatabaseName;
                        m_watcher.UserName = strUserName;
                        m_watcher.Password = strPassword;

                        m_watcher.Run();
                    }
                }
            }
        }

        /*private bool ReadSiteID()
        {
            DBUtility.Utility util = new DBUtility.Utility();
            string szSiteID = util.getinivalue("Server Connection Info", "siteid");
            if (szSiteID == null || szSiteID == "")
            {
                System.Diagnostics.Trace.WriteLine("Site ID가 지정되지 않았습니다. ini파일을 확인하세요");
                return false;
            }

            int nSiteId = 1;

            if (int.TryParse(szSiteID, out nSiteId))
            {
                m_nSiteID = nSiteId;
            }
            else
            {
                System.Diagnostics.Trace.WriteLine("잘못된 Site ID입니다. ini파일을 확인하세요");
                return false;
            }

            return true;
        }*/

        private string ReadFireSensorDBConnectionInfo()
        {
            string strSQL = "Select PropertyValue from OptionSDMS where PropertyName = 'ExternalFireSensorDBConnection' and SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return null;

            string strConnectionInfo = WebDBManager.GetStringField(arrResult[0]);
            return strConnectionInfo;
        }

        public void AddAlarm(ExternalFireSensor sensor)
        {
            this.Invoke((MethodInvoker)delegate
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Tag == sensor)
                        return;
                }

                int nRowIndex = dataGridView1.Rows.Add();
                DataGridViewRow _row = dataGridView1.Rows[nRowIndex];

                _row.Cells[0].Value = sensor.ID;
                _row.Cells[1].Value = sensor.TagName;
                _row.Tag = sensor;

                NetworkWebManager.Instance.SendAlarm(sensor.SensorZoneID, sensor.SensorTagInfoID);

                // SaveTagHistory
                int nTagSensorType = -1;
                int nSensorTagID = GetAccessSensorTagID(sensor.SensorZoneID, out nTagSensorType);
                if (nSensorTagID > 0)
                {
                    SaveTagHistory(0x92, nSensorTagID, nTagSensorType, sensor.SensorZoneID);
                }

            });
        }

        public void RemoveAlarm(ExternalFireSensor sensor)
        {
            this.Invoke((MethodInvoker)delegate
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Tag == sensor)
                    {
                        dataGridView1.Rows.Remove(row);
                        NetworkWebManager.Instance.SendAlarmClear(sensor.SensorZoneID, sensor.SensorTagInfoID);

                        int nTagSensorType = -1;
                        int nSensorTagID = GetAccessSensorTagID(sensor.SensorZoneID, out nTagSensorType);
                        if (nSensorTagID > 0)
                        {
                            SaveTagHistory(0x93, nSensorTagID, nTagSensorType, sensor.SensorZoneID);
                        }

                        return;
                    }
                }
            });
        }

        private int GetAccessSensorTagID(int nSensorID, out int nSensorType)
        {
            nSensorType = -1;

            if (nSensorID < 0)
                return -1;

            string strSQL = "select ID, SensorType ";
            strSQL += "from SensorTagInfo ";
            strSQL += string.Format("where SensorZoneID = {0}", nSensorID);

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            VariousData<int> sensorTagInfo = WebDBManager.GetIntField(arrResult[0].ToString());
            VariousData<int> sensorType = WebDBManager.GetIntField(arrResult[1].ToString());

            nSensorType = sensorType == null ? -1 : sensorType.Data;

            return sensorTagInfo == null ? -1 : sensorTagInfo.Data;
        }

        private void SaveTagHistory(int nHeader, int nTagID, int nSensorType, int nSensorZoneID)
        {
            int nData = 0;
            int nTagType = 0;
            switch (nHeader)
            {
                case 0x87:
                case 0x88:
                case 0x89:
                    nData = 'N';
                    nTagType = 1;
                    break;

                case 0x91: // 전체복구
                    nData = 'R';
                    nTagType = 0;
                    break;
                case 0x92: // 신호발생
                    nData = 'N';
                    nTagType = 1;
                    break;
                case 0x93: // 신호복구
                    nData = 'F';
                    nTagType = 1;
                    break;
                case 0x94: // 장애발생
                    nData = 'E';
                    nTagType = 2;
                    break;
                case 0x95: // 장애복구
                    nData = 'C';
                    nTagType = 2;
                    break;
                case 0x96: // 감시발생
                    nData = 'N';
                    nTagType = 3;
                    break;
                case 0x97: // 감시복구
                    nData = 'F';
                    nTagType = 3;
                    break;
                case 0x98: // 예비경보발생
                    break;
                case 0x99: // 예비경보복구
                    break;
            }

            string szDate = WebDBManager.MakeDateTimeString(DateTime.Now);  
            string szSQL1 = "SELECT max(ID) FROM SensorTagHistory";
            ArrayList arResult = m_dbMgr.GetResultData(szSQL1);
            if (arResult != null && arResult.Count > 0)
            {
                int nMaxID = WebDBManager.GetIntField(arResult[0].ToString(), 0);
                int nID = nMaxID + 1;
                if (nTagID >= 0)
                {
                    string szSQL = "INSERT INTO SensorTagHistory (ID, SensorTagInfoID, TagType, TimeStamp, value, HistoryType, SiteID) VALUES " +
                                    " ( " + nID + "," + nTagID + "," + nTagType + ",'" + szDate + "'," + nData + "," + nSensorType + "," + m_nSiteID + ")";
      
                    string strSQL = string.Format(szSQL, m_nSiteID);
                    m_dbMgr.GetResultData(strSQL);
                }
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_watcher.Close();
            NetworkWebManager.Instance.ReleaseThread();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
