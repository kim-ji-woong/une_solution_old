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
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CCTVAlarmWatcher
{
    public partial class FormMain : Form
    {
        private int m_nSiteID = 1;
        private List<Process> m_childProcessList = new List<Process>();
        private WebDBManager m_dbMgr = null;

        // Simulation을 위한 임시 데이터
        private List<int> m_cctvIDList = new List<int>();

        public FormMain()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 기존에 실행되고 있던 CCTVViewer가 있으면 중지시킨다. 
            KillProcess("AlarmCCTVViewer");

            cboCCTVIndex.SelectedIndex = 0;

            m_nSiteID = NetworkManager.Instance.SiteID;
            ReadCCTVList();
        }

        public static void KillProcess(string strProcessName)
        {
            System.Diagnostics.Process[] processList = System.Diagnostics.Process.GetProcesses();

            foreach (System.Diagnostics.Process process in processList)
            {
                if (process.ProcessName == strProcessName)
                {
                    process.Kill();
                }
            }
        }

        private void ReadCCTVList()
        {
            WebDBManager dbMgr = new WebDBManager(m_nSiteID);
            m_dbMgr = dbMgr;
            string strSQL = "select CCTV_ID, IPAddr, CameraName, Port, UserID, Password from CCTVAlarm, CCTV where CCTV_ID = CCTV.ID and SiteID = " + m_nSiteID.ToString() + " order by CCTV_ID";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            int nCCTVCount = nResultCount / 6;
            //int nColumnCount = GetColumnCount(nCCTVCount);
            //int nRowIndex = 0, nColumnIndex = 0;
            int nCCTVIndex = 0;

            PanelCCTV[] arrPanels = new PanelCCTV[8] { panelCCTV1, panelCCTV2, panelCCTV3, panelCCTV4, panelCCTV5, panelCCTV6, panelCCTV7, panelCCTV8 };

            for (int i=0;i<nResultCount-5;i+=6)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strIP = WebDBManager.GetStringField(arrResult[i + 1]);
                string strName = WebDBManager.GetStringField(arrResult[i + 2]);
                VariousData<int> port = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                string strUserID = WebDBManager.GetStringField(arrResult[i + 4]);
                string strPassword = WebDBManager.GetStringField(arrResult[i + 5]);

                if (id == null || strIP == null || strName == null || port == null || strUserID == null || strPassword == null)
                    continue;

                /*nRowIndex = (nCCTVIndex + 1) % nColumnCount == 0 ? (nCCTVIndex + 1) / nColumnCount - 1 : (nCCTVIndex + 1) / nColumnCount;
                nColumnIndex = nCCTVIndex++ % nColumnCount;

                string strParam = string.Format("{0} {1} {2} {3} {4} {5} {6} \"{7}\" {8}",
                    nRowIndex, nColumnIndex, id.Data, strIP, port.Data, strUserID, strPassword, strName, this.Handle.ToInt32());*/
                string strParam = string.Format("{0} {1} {2} {3} {4} \"{5}\" {6} {7}",
                    id.Data, strIP, port.Data, strUserID, strPassword, strName, arrPanels[nCCTVIndex++].Handle.ToInt32(), this.Handle.ToInt32());

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "AlarmCCTVViewer.exe";
                startInfo.Arguments = strParam;
                Process process = Process.Start(startInfo);

                m_cctvIDList.Add(id.Data);

                if (process != null)
                {
                    m_childProcessList.Add(process);
                }
            }
        }

        // CCTV 창들을 몇개의 행으로 구분하여 정렬할 것인가?
        private int GetColumnCount(int nCCTVCount)
        {
            if (nCCTVCount < 5)
                return nCCTVCount;
            else if (nCCTVCount == 6)
                return 3;
            else if (nCCTVCount < 9)
                return 4;
            else if (nCCTVCount == 9)
                return 3;
            
            return 5;
        }
        
        public void OnSelectCCTV(IDISCameraControl cctv)
        {
            //labelSelectedCCTV.Text = string.Format("({0}){1}, {2}", cctv.ID, cctv.CameraName, cctv.IP);
        }

        /*private bool ReadSiteID()
        {
            DBUtility.Utility util = new DBUtility.Utility();
            string szSiteID = util.getinivalue("Server Connection Info", "siteid");
            if (szSiteID == null || szSiteID == "")
            {
                MessageBox.Show("Site ID가 지정되지 않았습니다. ini파일을 확인하세요", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            int nSiteId = 1;

            if (int.TryParse(szSiteID, out nSiteId))
            {
                m_nSiteID = nSiteId;
            }
            else
            {
                MessageBox.Show("잘못된 Site ID입니다.. ini파일을 확인하세요", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }*/

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            NetworkManager.Instance.ReleaseThread();

            foreach (Process process in m_childProcessList)
            {
                try
                {
                    process.Kill();
                }
                catch (Exception)
                {
                }
            }
        }

        // nAlarmType : 0이면 알람해제, 1이면 알람
        private void OnReceiveAlarm(int nCCTVID, int nAlarmType)
        {
            System.Diagnostics.Trace.WriteLine("OnReceiveAlarm : " + nCCTVID.ToString() + ", " + nAlarmType.ToString());
            foreach (DataGridViewRow row in gridAlarmLog.Rows)
            {
                CCTVAlarm cctv = (CCTVAlarm)row.Tag;

                if (cctv == null)
                    continue;

                if (cctv.CCTVID == nCCTVID)
                {
                    if (nAlarmType == 0)
                    {
                        gridAlarmLog.Rows.Remove(row);
                        // EMPoll이 보내오는 Alarm Off는 처리하지 않는다.
                        /*NetworkManager.Instance.SendAlarmClear(cctv.SensorZoneID);

                        try
                        {
                            int nTagSensorType = -1;
                            int nSensorTagID = GetAccessSensorTagID(cctv.SensorZoneID, out nTagSensorType);
                            if (nSensorTagID > 0)
                            {
                                SaveTagHistory(0x93, nSensorTagID, nTagSensorType, cctv.SensorZoneID);
                            }
                        }
                        catch(Exception ex)
                        {

                        }*/
                    }
                    return;
                }
            }

            if (nAlarmType == 1)
                AddAlarm(nCCTVID);
        }

        private void AddAlarm(int nCCTVID)
        {
            CCTVAlarm cctv = AlarmManager.Instance.GetCCTV(nCCTVID);

            if (cctv != null)
            {
                int nRowIndex = gridAlarmLog.Rows.Add();

                if (nRowIndex < 0)
                    return;

                DateTime now = DateTime.Now;
                string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);

                DataGridViewRow row = gridAlarmLog.Rows[nRowIndex];
                row.Cells[0].Value = nRowIndex + 1;
                row.Cells[1].Value = strTime;
                row.Cells[2].Value = nCCTVID;
                row.Cells[3].Value = cctv.CameraName;

                row.Tag = cctv;

                NetworkManager.Instance.SendAlarm(cctv.SensorZoneID, cctv.SensorTagInfoID);

                // SaveTagHistory
                try
                {
                    int nTagSensorType = -1;
                    int nSensorTagID = GetAccessSensorTagID(cctv.SensorZoneID, out nTagSensorType);
                    if (nSensorTagID > 0)
                    {
                        SaveTagHistory(0x92, nSensorTagID, nTagSensorType, cctv.SensorZoneID);
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        private int GetAccessSensorTagID(int nSensorID, out int nSensorType)
        {
            nSensorType = -1;
            if (m_dbMgr == null)
                return -1;
            if (nSensorID < 0)
                return -1;

            string strSQL = "select ID, SensorType ";
            strSQL += "from SensorTagInfo ";
            strSQL += string.Format("where SensorZoneID = {0}", nSensorID);

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            VariousData<int> sensorTagInfo = WebDBManager.GetIntField(arrResult[0].ToString());
            VariousData<int> sensorType = WebDBManager.GetIntField(arrResult[1].ToString());

            nSensorType = sensorType == null ? -1 : sensorType.Data;

            return sensorTagInfo == null ? -1 : sensorTagInfo.Data;
        }

        private void SaveTagHistory(int nHeader, int nTagID, int nSensorType, int nSensorZoneID)
        {
            if (m_dbMgr == null)
                return;

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
            ArrayList arResult = m_dbMgr.GetResultData(szSQL1, 0);
            if (arResult != null && arResult.Count > 0)
            {
                int nMaxID = DBUtility.WebDBManager.GetIntField(arResult[0].ToString(), 0);
                int nID = nMaxID + 1;
                if (nTagID >= 0)
                {
                    string szSQL = "INSERT INTO SensorTagHistory (ID, SensorTagInfoID, TagType, TimeStamp, value, HistoryType, SiteID) VALUES " +
                                    " ( " + nID + "," + nTagID + "," + nTagType + ",'" + szDate + "'," + nData + "," + nSensorType + "," + m_nSiteID + ")";

                    string strSQL = string.Format(szSQL, m_nSiteID);
                    m_dbMgr.GetResultData(strSQL, 0);
                }
            }
        }

        const int WM_COPYDATA = 0x4A;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_COPYDATA)
            {
                COPYDATASTRUCT cds = (COPYDATASTRUCT)m.GetLParam(typeof(COPYDATASTRUCT));
                OnReceiveAlarm(cds.cbData, cds.dwData.ToInt32());
            }

            base.WndProc(ref m);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAlarm_Click(object sender, EventArgs e)
        {
            if (cboCCTVIndex.SelectedIndex < 0)
                return;

            int nCCTVIndex = 0;

            string strSelectedText = cboCCTVIndex.Items[cboCCTVIndex.SelectedIndex].ToString();

            if (int.TryParse(strSelectedText, out nCCTVIndex) == false)
                return;

            nCCTVIndex--;

            if (m_cctvIDList.Count <= nCCTVIndex)
                return;

            int nCCTVID = m_cctvIDList[nCCTVIndex];
            AddAlarm(nCCTVID);
        }

        private void btnAlarmClear_Click(object sender, EventArgs e)
        {
            if (cboCCTVIndex.SelectedIndex < 0)
                return;

            int nCCTVIndex = 0;

            string strSelectedText = cboCCTVIndex.Items[cboCCTVIndex.SelectedIndex].ToString();

            if (int.TryParse(strSelectedText, out nCCTVIndex) == false)
                return;

            nCCTVIndex--;

            if (m_cctvIDList.Count <= nCCTVIndex)
                return;

            int nCCTVID = m_cctvIDList[nCCTVIndex];

            foreach (DataGridViewRow row in gridAlarmLog.Rows)
            {
                CCTVAlarm cctv = (CCTVAlarm)row.Tag;

                if (cctv == null)
                    continue;

                if (cctv.CCTVID == nCCTVID)
                {
                    gridAlarmLog.Rows.Remove(row);
                    NetworkManager.Instance.SendAlarmClear(cctv.SensorZoneID, cctv.SensorTagInfoID);

                    try
                    {
                        int nTagSensorType = -1;
                        int nSensorTagID = GetAccessSensorTagID(cctv.SensorZoneID, out nTagSensorType);
                        if (nSensorTagID > 0)
                        {
                            SaveTagHistory(0x93, nSensorTagID, nTagSensorType, cctv.SensorZoneID);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }
    }

    public class IDISCameraControl : AxRASplus_WatSearLib.AxRASplus_WatSear
    {
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONDBLCLK = 0x0203;
        private const int WM_RBUTTONDOWN = 0x0204;

        private FormMain m_owner = null;

        private int m_nID = 0;
        private string m_strIP = "";
        private string m_strCameraName = "";

        public FormMain Owner
        {
            set { m_owner = value; }
        }

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string IP
        {
            get { return m_strIP; }
            set { m_strIP = value; }
        }

        public string CameraName
        {
            get { return m_strCameraName; }
            set { m_strCameraName = value; }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_LBUTTONDOWN)
            {
                if (m_owner != null)
                    m_owner.OnSelectCCTV(this);
                //System.Diagnostics.Trace.WriteLine("LButtonDown");
            }
            else if (m.Msg == WM_RBUTTONDOWN)
            {
                /*int y = ((int)m.LParam >> 16);
                int x = ((int)m.LParam & 0xffff);
                System.Diagnostics.Trace.WriteLine("RButtonDown(" + x.ToString() + ", " + y.ToString() + ")");*/

                if (m_owner != null)
                    m_owner.OnSelectCCTV(this);
            }
            else if (m.Msg == WM_LBUTTONDBLCLK)
            {
                /*int y = ((int)m.LParam >> 16);
                int x = ((int)m.LParam & 0xffff);
                System.Diagnostics.Trace.WriteLine("LButtonDoubleClick(" + x.ToString() + ", " + y.ToString() + ")");*/
            }

            base.WndProc(ref m);
        }
    }

    public struct COPYDATASTRUCT
    {
        public IntPtr dwData;
        public int cbData;
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)]
        public string lpData;
    }
}
