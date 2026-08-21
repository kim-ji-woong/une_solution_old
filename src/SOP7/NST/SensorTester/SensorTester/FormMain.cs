using dnsCommunicateSopServer;
using dnsData.Sensor;
using dnsDBUtil;
using SoulbrainSensorTester.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using SDMS.DAL;

namespace SoulbrainSensorTester
{
    public partial class FormMain : Form, IFormMain
    {
        private SDMS.DAL.DataManager m_dataMgr = null;
        //private WebDBManager m_dbManager = null;
        private Data.DataManager m_dataManager = null;
        private SopQueryManager m_SopQueryMgr = null;

        private static FormMain m_instance = null;
        public static FormMain Instance
        {
            get { return m_instance; }
        }

        public FormMain()
        {
            InitializeComponent();
            m_instance = this;

            m_SopQueryMgr = new SopQueryManager();

            // WSOP DB 매니저
            InitDBSet();
            m_dataManager = new Data.DataManager(m_dataMgr);

            // 트리 작성
            TreeNode fireSensorTree = m_dataManager.MakeSensorTree(Facility.FacilityType.FIRE_SENSOR);
            if (fireSensorTree != null)
                sensorTreeView.Nodes.Add(fireSensorTree);

            TreeNode psmSensorTree = m_dataManager.MakeSensorTree(Facility.FacilityType.PSM_SENSOR);
            if (psmSensorTree != null)
                sensorTreeView.Nodes.Add(psmSensorTree);

            TreeNode etcSensorTree = m_dataManager.MakeSensorTree(Facility.FacilityType.ETC);
            if (etcSensorTree != null)
                sensorTreeView.Nodes.Add(etcSensorTree);
        }

        public void reloadGrid()
        {
            this.Invoke((MethodInvoker)delegate
            {
                List<AlarmData> listAlarms = m_dataManager.GetAlarmList();
                List<AlarmData> listOldAlarms = new List<AlarmData>();

                List<AlarmData> listAddAlarms = new List<AlarmData>();
                List<AlarmData> listRemoveAlarms = new List<AlarmData>();

                if (listAlarms == null)
                    return;

                // 기존 알람 리스트와 비교 후 추가 및 삭제
                // 기존 알람 리스트 작성
                DataGridViewRowCollection dataRows = gridCurrent.Rows;

                if (dataRows.Count != 0)
                {
                    foreach (DataGridViewRow row in dataRows)
                    {
                        AlarmData alarm = (AlarmData)row.Tag;
                        listOldAlarms.Add(alarm);
                    }
                }

                // 삭제 알람 리스트 작성
                foreach (AlarmData alarm in listOldAlarms)
                {
                    AlarmData chk = null;
                    chk = listAlarms.Find(x => x.SensorType == alarm.SensorType && x.SensorTagID == alarm.SensorTagID && x.SensorZoneID == alarm.SensorZoneID);

                    if (chk == null)
                        listRemoveAlarms.Add(alarm);
                }

                // 추가 알람 리스트 작성
                foreach (AlarmData alarm in listAlarms)
                {
                    AlarmData chk = null;
                    chk = listOldAlarms.Find(x => x.SensorType == alarm.SensorType && x.SensorTagID == alarm.SensorTagID && x.SensorZoneID == alarm.SensorZoneID);

                    if (chk == null)
                        listAddAlarms.Add(alarm);
                }

                // 추가된 알람 표시
                foreach (AlarmData alarmData in listAddAlarms)
                {
                    int rowIndex = gridCurrent.Rows.Add(alarmData.SensorName);
                    gridCurrent.Rows[rowIndex].Tag = alarmData;
                }


                // 해제된 알람 표시 제거
                if (dataRows.Count != 0)
                {
                    foreach (DataGridViewRow row in dataRows)
                    {
                        AlarmData alarm = (AlarmData)row.Tag;

                        if (alarm == null)
                            continue;

                        foreach (AlarmData alarmData in listRemoveAlarms)
                        {
                            if (alarmData.SensorTagID == alarm.SensorTagID && alarmData.SensorType == alarm.SensorType && alarmData.SensorZoneID == alarm.SensorZoneID)
                                dataRows.RemoveAt(row.Index);
                        }
                    }
                }
            });
        }

        private void InitDBSet()
        {
            string strSiteID = ConfigurationManager.AppSettings.Get("SITE_ID");
            if (strSiteID == null || strSiteID.Length == 0)
                strSiteID = "11";

            string strDBName = ConfigurationManager.AppSettings.Get("DB_NAME");
            if (strDBName == null || strDBName.Length == 0)
                strDBName = "WSOP_11";

            string strDBType = ConfigurationManager.AppSettings.Get("DB_TYPE");
            if (strDBType == null || strDBType.Length == 0)
                strDBType = "0";

            string strWebServerURL = ConfigurationManager.AppSettings.Get("WebServerURL");
            if (strWebServerURL == null || strWebServerURL.Length == 0)
                strWebServerURL = "http://127.0.0.1:808";

            int nSiteID, nDBType;
            int.TryParse(strSiteID.Trim(), out nSiteID);
            int.TryParse(strDBType.Trim(), out nDBType);

            //m_dbManager = new WebDBManager(strDBName, nDBType, nSiteID, strWebServerURL);
            m_dataMgr = new SDMS.DAL.DataManager(strDBName, nDBType, nSiteID, strWebServerURL);
        }

        private void sensorTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null || e.Node.Tag == null || !(e.Node.Tag is FireSensorData || e.Node.Tag is PSMSensorData || e.Node.Tag is ETCSensorData)) 
            {
                btnSend.Enabled = false;
                btnReset.Enabled = false;
                lbSensorType.Text = "";
                lbSensorName.Text = "";

                return;
            }
                
            if (e.Node.Tag is FireSensorData)
            {
                // 화재 센서 선택
                FireSensorData fireSensor = (FireSensorData)e.Node.Tag;

                // 버튼 활성화
                btnSend.Enabled = true;
                btnReset.Enabled = true;

                // 센서 타입 및 이름 표시
                lbSensorType.Text = Facility.GetFacilityTypeString(Facility.FacilityType.FIRE_SENSOR);
                lbSensorName.Text = fireSensor.Name;
            }
            else if (e.Node.Tag is PSMSensorData)
            {
                // PSM 센서 선택
                PSMSensorData psmSensor = (PSMSensorData)e.Node.Tag;

                // 버튼 활성화
                btnSend.Enabled = true;
                btnReset.Enabled = true;

                // 센서 타입 및 이름 표시
                lbSensorType.Text = Facility.GetFacilityTypeString(Facility.FacilityType.PSM_SENSOR);
                lbSensorName.Text = psmSensor.Name;
            }
            else if (e.Node.Tag is ETCSensorData)
            {
                // ETC 센서 선택
                ETCSensorData etcSensor = (ETCSensorData)e.Node.Tag;

                // 버튼 활성화
                btnSend.Enabled = true;
                btnReset.Enabled = true;

                // 센서 타입 및 이름 표시
                lbSensorType.Text = Facility.GetFacilityTypeString(Facility.FacilityType.ETC);
                lbSensorName.Text = etcSensor.Name;
            }

        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            // 선택된 센서 값 가져오기
            TreeNode node = sensorTreeView.SelectedNode;
            SendAlarm(node);            
        }

        private void SendAlarm(TreeNode node)
        {
            if (node == null || node.Tag == null || !(node.Tag is FireSensorData || node.Tag is PSMSensorData || node.Tag is ETCSensorData))
                return;

            AlarmData alarm = m_dataManager.GetAlarmData(node.Tag);

            if (alarm != null)
            {
                ArrayList arrData = new ArrayList();
                arrData.Add(alarm.SensorType);
                arrData.Add(alarm.SensorTagID);
                arrData.Add(alarm.SensorZoneID);
                arrData.Add(true);

                m_SopQueryMgr.SendAlarmQuery_TEST(arrData, CommonString.ALARM_METHOD, alarm.URL);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            // 선택된 센서 값 가져오기
            TreeNode node = sensorTreeView.SelectedNode;

            if (node == null || node.Tag == null || !(node.Tag is FireSensorData || node.Tag is PSMSensorData || node.Tag is ETCSensorData))
                return;

            AlarmData alarm = m_dataManager.GetAlarmData(node.Tag);

            if (alarm != null)
            {
                ArrayList arrData = new ArrayList();
                arrData.Add(alarm.SensorType);
                arrData.Add(alarm.SensorTagID);
                arrData.Add(alarm.SensorZoneID);
                arrData.Add(false);

                m_SopQueryMgr.SendAlarmQuery_TEST(arrData, CommonString.ALARM_METHOD, alarm.URL);
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            m_dataManager.StartThread();
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_dataManager.Shutdown();
        }

        private void btnSelectReset_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewCell cell in gridCurrent.SelectedCells)
            {
                DataGridViewRow row = gridCurrent.Rows[cell.RowIndex];

                if (row.IsNewRow)
                    continue;

                if (row.Tag == null || !(row.Tag is AlarmData))
                    continue;

                AlarmData alarm = (AlarmData)row.Tag;

                // SVMS 관련 복구 금지
                if (alarm.SensorType >= (int)Facility.FacilityType.Intrusion_S1 && alarm.SensorType <= (int)Facility.FacilityType.EmergencyBell_S1)
                    return;


                if (alarm != null)
                {
                    ArrayList arrData = new ArrayList();
                    arrData.Add(alarm.SensorType);
                    arrData.Add(alarm.SensorTagID);
                    arrData.Add(alarm.SensorZoneID);
                    arrData.Add(false);

                    m_SopQueryMgr.SendAlarmQuery_TEST(arrData, CommonString.ALARM_METHOD, alarm.URL);
                }
            }
        }

        private void btnAllReset_Click(object sender, EventArgs e)
        {
            DataGridViewRowCollection dataRows = gridCurrent.Rows;

            if (dataRows.Count != 0)
            {
                foreach (DataGridViewRow row in dataRows)
                {
                    AlarmData alarm = (AlarmData)row.Tag;

                    // SVMS 관련 복구 금지
                    if (alarm.SensorType >= (int)Facility.FacilityType.Intrusion_S1 && alarm.SensorType <= (int)Facility.FacilityType.EmergencyBell_S1)
                        continue;

                    if (alarm != null)
                    {
                        ArrayList arrData = new ArrayList();
                        arrData.Add(alarm.SensorType);
                        arrData.Add(alarm.SensorTagID);
                        arrData.Add(alarm.SensorZoneID);
                        arrData.Add(false);

                        m_SopQueryMgr.SendAlarmQuery(arrData, CommonString.ALARM_METHOD, alarm.URL);
                    }
                }
            }
        }

        private void btnProcessAllClear_Click(object sender, EventArgs e)
        {
            m_SopQueryMgr.SendAllClearQuery(CommonString.ALARM_METHOD, m_dataManager.StrAlarm_Fire_RUL);
        }

        private void btnMultipleAlarms_Click(object sender, EventArgs e)
        {
            string strAlarmCount = textBoxMultipleAlarmCount.Text.Trim();

            if (strAlarmCount.Length == 0)
            {
                textBoxMultipleAlarmCount.Focus();
                MessageBox.Show("알람 개수를 입력하세요");
                return;
            }

            int nAlarmCount;

            if (int.TryParse(strAlarmCount, out nAlarmCount) == false || nAlarmCount <= 0)
            {
                textBoxMultipleAlarmCount.Focus();
                MessageBox.Show("알람 개수는 0보다 큰 정수 형태의 값이어야 합니다.");
                return;
            }

            SendMultipleAlarms(nAlarmCount);
        }

        private void SendMultipleAlarms(int nAlarmCount)
        {
            List<TreeNode> sensorNodes = new List<TreeNode>();
            GetSensorNode(sensorTreeView.Nodes, sensorNodes, nAlarmCount);

            

            foreach (TreeNode node in sensorNodes)
            {
                SendAlarm(node);
            }
        }

        private bool GetSensorNode(TreeNodeCollection nodes, List<TreeNode> sensorNodes, int nTargetCount)
        {
            List<int> listZoneID = new List<int>();

            foreach (TreeNode node in nodes)
            {
                if (node.Tag != null && (node.Tag is FireSensorData || node.Tag is PSMSensorData || node.Tag is ETCSensorData))
                {
                    int nZoneID = 0;

                    if (node.Tag is FireSensorData)
                    {
                        FireSensorData data = (FireSensorData)node.Tag;
                        nZoneID = data.EquipZoneID;
                    }
                    else if (node.Tag is PSMSensorData)
                    {
                        PSMSensorData data = (PSMSensorData)node.Tag;
                        nZoneID = data.EquipZoneID;
                    }
                    else if (node.Tag is ETCSensorData)
                    {
                        ETCSensorData data = (ETCSensorData)node.Tag;
                        nZoneID = data.EquipZoneID;
                    }

                    if (!listZoneID.Contains(nZoneID))
                    {
                        sensorNodes.Add(node);
                    }

                    listZoneID.Add(nZoneID);

                    if (sensorNodes.Count >= nTargetCount)
                        return true;
                }
                else
                {
                    if (GetSensorNode(node.Nodes, sensorNodes, nTargetCount))
                        return true;
                }
            }

            return false;
        }
    }
}
