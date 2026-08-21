using CrisisAlertManager.Data;
using DBUtility2;
using libSensorProcess;
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

namespace CrisisAlertManager.Popup_Dialog.Alarm
{
    public partial class FormAlertAlarm : Form
    {
        private AlarmData m_alarm = null;
        private int m_nID = -1;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public FormAlertAlarm(AlarmData alarm)
        {
            InitializeComponent();

            m_alarm = alarm;
            m_nID = alarm.ID;

            ShowAlarmState(m_alarm);
        }

        public void ChangeAlertAlarm(AlarmData alarm)
        {
            m_alarm = alarm;
            m_nID = alarm.ID;

            ShowAlarmState(m_alarm);
        }

        private void ShowAlarmState(AlarmData alarm)
        {
            if (alarm == null)
                return;

            // 이미지 표시 (상태, 탑패널)
            ShowFacilityAlarm(alarm.FacilityType);
            ShowRiskLevel(alarm.RiskLevel);

            // 주소 표시
            lbAddress.Text = alarm.Address;

            // 알람 갯수 표시
            int nCount = FormMain.Instance.DataManager.GetAlarmCount();

            if (nCount > 0)
                lbAlarm.Text = nCount.ToString();
            else
                lbAlarm.Text = "0";

        }

        private void ShowFacilityAlarm(FacilityType type)
        {
            if (type == null || type == FacilityType.NONE)
                return;

            if (type == FacilityType.FIRE_SENSOR)
            {
                plTop.BackgroundImage = global::CrisisAlertManager.Properties.Resources.FireAlarmTop;
                plFireText.Visible = true;
                plFloodText.Visible = false;
                plHeatText.Visible = false;
                plCollapseText.Visible = false;
            }
            else if (type == FacilityType.FLOOD_SENSOR)
            {
                plTop.BackgroundImage = global::CrisisAlertManager.Properties.Resources.FloodAlarmTop;
                plFireText.Visible = false;
                plFloodText.Visible = true;
                plHeatText.Visible = false;
                plCollapseText.Visible = false;
            }  
            else if (type == FacilityType.HEAT_SENSOR)
            {
                plTop.BackgroundImage = global::CrisisAlertManager.Properties.Resources.HeatAlarmTop;
                plFireText.Visible = false;
                plFloodText.Visible = false;
                plHeatText.Visible = true;
                plCollapseText.Visible = false;
            }
            else if (type == FacilityType.COLLAPSE_SENSOR)
            {
                plTop.BackgroundImage = global::CrisisAlertManager.Properties.Resources.CollapseAlarmTop;
                plFireText.Visible = false;
                plFloodText.Visible = false;
                plHeatText.Visible = false;
                plCollapseText.Visible = true;
            }
                
        }

        private void ShowRiskLevel(string strRiskLevel)
        {
            if (strRiskLevel == null || strRiskLevel == "")
                return;

            if (strRiskLevel == CommonString.RiskLevel_Normal_Kor)
                pbState.BackgroundImage = global::CrisisAlertManager.Properties.Resources.Normal_new;
            else if (strRiskLevel == CommonString.RiskLevel_Attention_Kor)
                pbState.BackgroundImage = global::CrisisAlertManager.Properties.Resources.Attention_new;
            else if (strRiskLevel == CommonString.RiskLevel_Caution_Kor)
                pbState.BackgroundImage = global::CrisisAlertManager.Properties.Resources.Caution_new;
            else if (strRiskLevel == CommonString.RiskLevel_Alert_Kor)
                pbState.BackgroundImage = global::CrisisAlertManager.Properties.Resources.Alert_new;
            else if (strRiskLevel == CommonString.RiskLevel_Serious_Kor)
                pbState.BackgroundImage = global::CrisisAlertManager.Properties.Resources.Serious_new;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pbState_Click(object sender, EventArgs e)
        {
            // 알람소리 끄기
            FireDetectProcess.SoundPlayer.Stop();

            // 해당 알람 화면 띄우기
            FormMain.Instance.ShowAlertSensor(m_alarm.FacilityType, m_alarm.SersorID);

            // 클릭 시 알람 신호 체크
            FormMain.Instance.DataManager.ConfirmAlertAarm(m_alarm.SersorID, (int)m_alarm.FacilityType);

            this.Close();
        }

        private void plAlarm_Click(object sender, EventArgs e)
        {
            // 알람 관리 페이지로 이동
            FormMain.Instance.ShowAlarmBoard(m_alarm.FacilityType);
        }

        public void CheckCloseAlarm(int nID)
        {
            // 알람ID가 같을 경우 닫기
            if (nID == m_nID)
                this.Close();
        }
    }
}
