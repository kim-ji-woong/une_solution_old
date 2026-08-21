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
    public partial class FormMain2 : Form, IFormMain
    {
        private SDMS.DAL.DataManager m_dataMgr = null;
        //private WebDBManager m_dbManager = null;
        private Data.DataManager m_dataManager = null;
        private SopQueryManager m_SopQueryMgr = null;

        private double m_dCoLimit = 100;
        private double m_dThermalLimit = 100;
        private bool m_alarmCondition = true;
        private string m_strTargetSensorName = "";

        private bool[] m_probability = new bool[100];

        private static FormMain2 m_instance = null;
        public static FormMain2 Instance
        {
            get { return m_instance; }
        }

        public FormMain2()
        {
            InitializeComponent();
            m_instance = this;

            m_SopQueryMgr = new SopQueryManager();

            // WSOP DB 매니저
            InitDBSet();
            m_dataManager = new Data.DataManager(m_dataMgr, false);
            m_dataManager.SetTargetSensor(m_strTargetSensorName);
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

            string strCOLimit = ConfigurationManager.AppSettings.Get("CO_Limit");

            if (strCOLimit != null)
            {
                strCOLimit = strCOLimit.Trim();
                double.TryParse(strCOLimit, out m_dCoLimit);
            }

            string strThermalLimit = ConfigurationManager.AppSettings.Get("Thermal_Limit");

            if (strThermalLimit != null)
            {
                strThermalLimit = strThermalLimit.Trim();
                double.TryParse(strThermalLimit, out m_dThermalLimit);
            }

            string strAlarmCondition = ConfigurationManager.AppSettings.Get("AlarmCondition");

            if (strAlarmCondition != null)
            {
                if (strAlarmCondition.Trim().ToLower() == "and")
                    m_alarmCondition = true;
                else if (strAlarmCondition.Trim().ToLower() == "or")
                    m_alarmCondition = false;
            }

            m_strTargetSensorName = ConfigurationManager.AppSettings.Get("TargetSensorName");

            string strProbability = ConfigurationManager.AppSettings.Get("Probability");

            int nProbability;

            if (int.TryParse(strProbability, out nProbability))
            {
                if (nProbability >= 0 && nProbability <= 100)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        if (i < nProbability)
                            m_probability[i] = true;
                        else
                            m_probability[i] = false;
                    }
                }
            }
            else
            {
                for (int i = 0; i < 100; i++)
                {
                    m_probability[i] = true;
                }
            }

            //m_dbManager = new WebDBManager(strDBName, nDBType, nSiteID, strWebServerURL);
            m_dataMgr = new SDMS.DAL.DataManager(strDBName, nDBType, nSiteID, strWebServerURL);
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string strCo = textBoxCO.Text.Trim();
            string strThermal = textBoxThermal.Text.Trim();

            double dCo, dThermal;

            if (strCo.Length == 0)
            {
                textBoxCO.Focus();
                MessageBox.Show("CO 농도를 입력하세요.");
                return;
            }

            if (double.TryParse(strCo, out dCo) == false)
            {
                textBoxCO.Focus();
                MessageBox.Show("CO 농도는 숫자를 입력해야 합니다.");
                return;
            }

            if (strThermal.Length == 0)
            {
                textBoxThermal.Focus();
                MessageBox.Show("온도를 입력하세요.");
                return;
            }

            if (double.TryParse(strThermal, out dThermal) == false)
            {
                textBoxThermal.Focus();
                MessageBox.Show("온도는 숫자를 입력해야 합니다.");
                return;
            }

            if (m_alarmCondition)
            {
                if (dCo >= m_dCoLimit && dThermal >= m_dThermalLimit)
                    SendAlarm(true);
                else
                    SendAlarm(false);
            }
            else
            {
                if (dCo >= m_dCoLimit || dThermal >= m_dThermalLimit)
                    SendAlarm(true);
                else
                    SendAlarm(false);
            }         
        }

        private void SendAlarm(bool isAlarm)
        {
            if (m_dataManager.TargetSensor == null)
                return;

            AlarmData alarm = new AlarmData();
            //AlarmData alarm = m_dataManager.GetAlarmData(m_dataManager.TargetSensor);

            alarm.SensorTagID = 2008;
            alarm.SensorType = 0;
            alarm.SensorZoneID = 2008;
            alarm.URL = m_dataManager.StrAlarm_Fire_RUL;

            if (alarm != null)
            {
                if (GetRandom())
                {
                    ArrayList arrData = new ArrayList();
                    arrData.Add(alarm.SensorType);
                    arrData.Add(alarm.SensorTagID);
                    arrData.Add(alarm.SensorZoneID);
                    arrData.Add(isAlarm);

                    if (m_SopQueryMgr.SendAlarmQuery_TEST(arrData, CommonString.ALARM_METHOD, alarm.URL))
                    {
                        lblResult.Text = GetTimeString() + "데이터 전송에 성공하였습니다.";
                        lblResult.ForeColor = Color.Green;
                    }
                    else
                    {
                        lblResult.Text = GetTimeString() + "데이터 전송에 실패하였습니다.";
                        lblResult.ForeColor = Color.Red;
                    }
                }
                else
                {
                    lblResult.Text = GetTimeString() + "데이터 전송에 실패하였습니다.";
                    lblResult.ForeColor = Color.Red;
                }

                lblResult.Visible = true;
            }
        }

        private string GetTimeString()
        {
            DateTime dtNow = DateTime.Now;
            return string.Format("[{0:00}:{1:00}:{2:00}] : ", dtNow.Hour, dtNow.Minute, dtNow.Second);
        }

        private bool GetRandom()
        {
            Random rand = new Random((int)DateTime.Now.Ticks);
            int nIndex = rand.Next(0, 100);
            return m_probability[nIndex];
        }

        private void FormMain2_Load(object sender, EventArgs e)
        {
            m_dataManager.StartThread();
        }

        private void FormMain2_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_dataManager.Shutdown();
        }
    }
}
