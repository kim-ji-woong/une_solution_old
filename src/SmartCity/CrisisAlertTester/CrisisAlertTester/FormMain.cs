using CrisisAlertTester.Data;
using DBUtility2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrisisAlertTester
{
    public partial class FormMain : Form
    {
        private WebDBManager m_dbMgr = null;

        private DataManager m_dataMgr = null;
        public DataManager DataManager
        {
            get { return m_dataMgr; }
        }

        public FormMain()
        {
            InitializeComponent();
            // 테스트 주석
            string strWebDBServerURL = ConfigurationManager.AppSettings.Get("WebDBServerURL");
            if (strWebDBServerURL == null || strWebDBServerURL.Length == 0)
                strWebDBServerURL = "http://localhost";

            m_dbMgr = new WebDBManager(1);
            m_dbMgr.WebServerURL = strWebDBServerURL;
            m_dbMgr.DatabaseName = "SmartCity";
            m_dbMgr.DatabaseType = WebDBManager.DBType.mysql;

            m_dataMgr = new DataManager(m_dbMgr);

            InitComboBox();
        }

        public void InitComboBox()
        {
            cmbFacilityType.Items.Clear();
            cmbSensor.Items.Clear();
            cmbRiskLevel.Items.Clear();

            cmbSensor.DisplayMember = "Addr";

            // 신호타입
            cmbFacilityType.Items.Add(CommonString.FacilityType_Fire_Kor);
            cmbFacilityType.Items.Add(CommonString.FacilityType_Flood_Kor);
            cmbFacilityType.Items.Add(CommonString.FacilityType_Heat_Kor);
            cmbFacilityType.Items.Add(CommonString.FacilityType_Collapse_Kor);

            if (cmbFacilityType.Items.Count > 0)
                cmbFacilityType.SelectedIndex = 0;

            // 위기경보 단계
            cmbRiskLevel.Items.Add(CommonString.RiskLevel_Attention_Kor);
            cmbRiskLevel.Items.Add(CommonString.RiskLevel_Caution_Kor);
            cmbRiskLevel.Items.Add(CommonString.RiskLevel_Alert_Kor);
            cmbRiskLevel.Items.Add(CommonString.RiskLevel_Serious_Kor);

            if (cmbRiskLevel.Items.Count > 0)
                cmbRiskLevel.SelectedIndex = 0;
        }

        private void cmbFacilityType_SelectedValueChanged(object sender, EventArgs e)
        {
            cmbSensor.Items.Clear();

            if (cmbFacilityType.SelectedItem == null)
                return;

            // 센서 다시 불러오기
            m_dataMgr.LoadSensors();

            if (cmbFacilityType.SelectedItem == CommonString.FacilityType_Fire_Kor)
            {
                // 화재센서 불러오기
                Dictionary<int, FireSensor> dicFireSensors = new Dictionary<int, FireSensor>();
                dicFireSensors = m_dataMgr.DicFireSensors;

                foreach (KeyValuePair<int, FireSensor> pair in dicFireSensors)
                {
                    FireSensor sensor = pair.Value;

                    cmbSensor.Items.Add(sensor);
                }

                if (cmbSensor.Items.Count > 0)
                    cmbSensor.SelectedIndex = 0;
            }
            else if (cmbFacilityType.SelectedItem == CommonString.FacilityType_Flood_Kor)
            {
                // 홍수센서 불러오기
                Dictionary<int, FloodSensor> dicFloodSensors = new Dictionary<int, FloodSensor>();
                dicFloodSensors = m_dataMgr.DicFloodSensors;

                foreach (KeyValuePair<int, FloodSensor> pair in dicFloodSensors)
                {
                    FloodSensor sensor = pair.Value;

                    cmbSensor.Items.Add(sensor);
                }

                if (cmbSensor.Items.Count > 0)
                    cmbSensor.SelectedIndex = 0;
            }
            else if (cmbFacilityType.SelectedItem == CommonString.FacilityType_Heat_Kor)
            {
                // 폭염센서 불러오기
                Dictionary<int, HeatSensor> dicHeatSensors = new Dictionary<int, HeatSensor>();
                dicHeatSensors = m_dataMgr.DicHeatSensors;

                foreach (KeyValuePair<int, HeatSensor> pair in dicHeatSensors)
                {
                    HeatSensor sensor = pair.Value;

                    cmbSensor.Items.Add(sensor);
                }

                if (cmbSensor.Items.Count > 0)
                    cmbSensor.SelectedIndex = 0;
            }
            else if (cmbFacilityType.SelectedItem == CommonString.FacilityType_Collapse_Kor)
            {
                // 경사지 붕괴센서 불러오기
                Dictionary<int, CollapseSensor> dicCollapseSensors = new Dictionary<int, CollapseSensor>();
                dicCollapseSensors = m_dataMgr.DicCollapseSensors;

                foreach (KeyValuePair<int, CollapseSensor> pair in dicCollapseSensors)
                {
                    CollapseSensor sensor = pair.Value;

                    cmbSensor.Items.Add(sensor);
                }

                if (cmbSensor.Items.Count > 0)
                    cmbSensor.SelectedIndex = 0;
            }
        }

        private void cmbSensor_SelectedValueChanged(object sender, EventArgs e)
        {
            lbRiskLevel.Text = "";

            if (cmbSensor.SelectedItem == null)
                return;

            // 해당 센서의 현재 상태 표시하기
            if (cmbFacilityType.SelectedItem == CommonString.FacilityType_Fire_Kor)
            {
                // 화재센서 불러오기
                FireSensor fire = (FireSensor)cmbSensor.SelectedItem;
                lbRiskLevel.Text = ChangeStateKor(fire.State);
            }
            else if (cmbFacilityType.SelectedItem == CommonString.FacilityType_Flood_Kor)
            {
                // 홍수센서 불러오기
                FloodSensor flood = (FloodSensor)cmbSensor.SelectedItem;
                lbRiskLevel.Text = ChangeStateKor(flood.State);
            }
            else if (cmbFacilityType.SelectedItem == CommonString.FacilityType_Heat_Kor)
            {
                // 폭염센서 불러오기
                HeatSensor heat = (HeatSensor)cmbSensor.SelectedItem;
                lbRiskLevel.Text = ChangeStateKor(heat.State);
            }
            else if (cmbFacilityType.SelectedItem == CommonString.FacilityType_Collapse_Kor)
            {
                // 경사지 붕괴센서 불러오기
                CollapseSensor collapse = (CollapseSensor)cmbSensor.SelectedItem;
                lbRiskLevel.Text = ChangeStateKor(collapse.State);
            }
        }

        private string ChangeStateKor(string strRiskLevel)
        {
            string strRet = "";

            if (strRiskLevel == CommonString.RiskLevel_Normal)
                strRet = CommonString.RiskLevel_Normal_Kor;
            else if (strRiskLevel == CommonString.RiskLevel_Attention)
                strRet = CommonString.RiskLevel_Attention_Kor;
            else if (strRiskLevel == CommonString.RiskLevel_Caution)
                strRet = CommonString.RiskLevel_Caution_Kor;
            else if (strRiskLevel == CommonString.RiskLevel_Alert)
                strRet = CommonString.RiskLevel_Alert_Kor;
            else if (strRiskLevel == CommonString.RiskLevel_Serious)
                strRet = CommonString.RiskLevel_Serious_Kor;

            return strRet;
        }

        private string ChangeState(string strRiskLevel)
        {
            string strRet = "";

            if (strRiskLevel == CommonString.RiskLevel_Normal_Kor)
                strRet = CommonString.RiskLevel_Normal;
            else if (strRiskLevel == CommonString.RiskLevel_Attention_Kor)
                strRet = CommonString.RiskLevel_Attention;
            else if (strRiskLevel == CommonString.RiskLevel_Caution_Kor)
                strRet = CommonString.RiskLevel_Caution;
            else if (strRiskLevel == CommonString.RiskLevel_Alert_Kor)
                strRet = CommonString.RiskLevel_Alert;
            else if (strRiskLevel == CommonString.RiskLevel_Serious_Kor)
                strRet = CommonString.RiskLevel_Serious;

            return strRet;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (cmbSensor.SelectedItem == null)
                return;

            // 위기경보 단계 이력 저장
            AlertReport();

            // 알람신호 발생
            CheckAlertAlarm();

            // 위기경보 단계 수동 조정
            UpdateSensorState();
        }

        private void AlertReport()
        {
            string strFacility = (string)cmbFacilityType.SelectedItem;
            FacilityType facilityType = FacilityType.NONE;

            string strDataName = CommonString.RiskLevel_Kor;
            string strOldData = "";
            string strNewData = "";
            int nID = 0;

            if (strFacility == CommonString.FacilityType_Fire_Kor)
            {
                facilityType = FacilityType.FIRE_SENSOR;

                FireSensor fire = (FireSensor)cmbSensor.SelectedItem;
                nID = fire.ID;
                strOldData = ChangeStateKor(fire.State);

                strNewData = (string)cmbRiskLevel.SelectedItem;
            }
            else if (strFacility == CommonString.FacilityType_Flood_Kor)
            {
                facilityType = FacilityType.FLOOD_SENSOR;

                FloodSensor flood = (FloodSensor)cmbSensor.SelectedItem;
                nID = flood.ID;
                strOldData = ChangeStateKor(flood.State);

                strNewData = (string)cmbRiskLevel.SelectedItem;
            }
            else if (strFacility == CommonString.FacilityType_Heat_Kor)
            {
                facilityType = FacilityType.HEAT_SENSOR;

                HeatSensor heat = (HeatSensor)cmbSensor.SelectedItem;
                nID = heat.ID;
                strOldData = ChangeStateKor(heat.State);

                strNewData = (string)cmbRiskLevel.SelectedItem;
            }
            else if (strFacility == CommonString.FacilityType_Collapse_Kor)
            {
                facilityType = FacilityType.COLLAPSE_SENSOR;

                CollapseSensor collapse = (CollapseSensor)cmbSensor.SelectedItem;
                nID = collapse.ID;
                strOldData = ChangeStateKor(collapse.State);

                strNewData = (string)cmbRiskLevel.SelectedItem;
            }

            m_dataMgr.InsertAlertReport(facilityType, nID, strDataName, strOldData, strNewData);
        }

        private bool CheckAlertAlarm()
        {
            bool bRet = false;
            bool bAlarm = false;

            string strFacility = (string)cmbFacilityType.SelectedItem;
            FacilityType facilityType = FacilityType.NONE;

            string strOldData = "";
            string strNewData = "";
            string strAddress = "";
            int nID = 0;

            if (strFacility == CommonString.FacilityType_Fire_Kor)
            {
                facilityType = FacilityType.FIRE_SENSOR;

                FireSensor fire = (FireSensor)cmbSensor.SelectedItem;
                nID = fire.ID;
                strAddress = fire.Addr;
                strOldData = ChangeStateKor(fire.State);

                strNewData = (string)cmbRiskLevel.SelectedItem;
            }
            else if (strFacility == CommonString.FacilityType_Flood_Kor)
            {
                facilityType = FacilityType.FLOOD_SENSOR;

                FloodSensor flood = (FloodSensor)cmbSensor.SelectedItem;
                nID = flood.ID;
                strAddress = flood.Addr;
                strOldData = ChangeStateKor(flood.State);

                strNewData = (string)cmbRiskLevel.SelectedItem;
            }
            else if (strFacility == CommonString.FacilityType_Heat_Kor)
            {
                facilityType = FacilityType.HEAT_SENSOR;

                HeatSensor heat = (HeatSensor)cmbSensor.SelectedItem;
                nID = heat.ID;
                strAddress = heat.Addr;
                strOldData = ChangeStateKor(heat.State);

                strNewData = (string)cmbRiskLevel.SelectedItem;
            }
            else if (strFacility == CommonString.FacilityType_Collapse_Kor)
            {
                facilityType = FacilityType.COLLAPSE_SENSOR;

                CollapseSensor collapse = (CollapseSensor)cmbSensor.SelectedItem;
                nID = collapse.ID;
                strAddress = collapse.Addr;
                strOldData = ChangeStateKor(collapse.State);

                strNewData = (string)cmbRiskLevel.SelectedItem;
            }

            if (strOldData == CommonString.RiskLevel_Normal_Kor)
            {
                bAlarm = true;
            }
            else if (strOldData == CommonString.RiskLevel_Attention_Kor && (strNewData == CommonString.RiskLevel_Caution_Kor || strNewData == CommonString.RiskLevel_Alert_Kor || strNewData == CommonString.RiskLevel_Serious_Kor))
            {
                bAlarm = true;
            }
            else if (strOldData == CommonString.RiskLevel_Caution_Kor && (strNewData == CommonString.RiskLevel_Alert_Kor || strNewData == CommonString.RiskLevel_Serious_Kor))
            {
                bAlarm = true;
            }
            else if (strOldData == CommonString.RiskLevel_Alert_Kor && (strNewData == CommonString.RiskLevel_Serious_Kor))
            {
                bAlarm = true;
            }

            if (bAlarm)
            {
                if (m_dataMgr.InsertAlertAarm(facilityType, nID, strAddress, strNewData))
                    bRet = true;
            }

            return bRet;
        }

        private bool UpdateSensorState()
        {
            bool bRet = false;
            bool bAlarm = false;

            int nID = 0;
            string strFacility = (string)cmbFacilityType.SelectedItem;
            string strRiskLevel = (string)cmbRiskLevel.SelectedItem;

            strRiskLevel = ChangeState(strRiskLevel);

            if (strFacility == CommonString.FacilityType_Fire_Kor)
            {
                FireSensor fire = (FireSensor)cmbSensor.SelectedItem;
                nID = fire.ID;

                if (m_dataMgr.UpdateFireSensorState(nID, strRiskLevel))
                    bRet = true;
            }
            else if (strFacility == CommonString.FacilityType_Flood_Kor)
            {
                FloodSensor flood = (FloodSensor)cmbSensor.SelectedItem;
                nID = flood.ID;

                if (m_dataMgr.UpdateFloodSensorState(nID, strRiskLevel))
                    bRet = true;
            }
            else if (strFacility == CommonString.FacilityType_Heat_Kor)
            {
                HeatSensor heat = (HeatSensor)cmbSensor.SelectedItem;
                nID = heat.ID;

                if (m_dataMgr.UpdateHeatSensorState(nID, strRiskLevel))
                    bRet = true;
            }
            else if (strFacility == CommonString.FacilityType_Collapse_Kor)
            {
                CollapseSensor collapse = (CollapseSensor)cmbSensor.SelectedItem;
                nID = collapse.ID;

                if (m_dataMgr.UpdateCollapseSensorState(nID, strRiskLevel))
                    bRet = true;
            }

            

            return bRet;
        }
    }
}
