using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CrisisAlertManager.Data;
using CrisisAlertManager.Popup_Dialog.Message;

namespace CrisisAlertManager.CrisisAlert
{
    public partial class uFormCollapseSensorInfo : UserControl
    {
        CollapseSensor m_collapseSensor = null;
        private Timer m_timerSensorReload = null;

        private Image m_imgState_Normal = global::CrisisAlertManager.Properties.Resources.StateNormal;
        private Image m_imgState_Attention = global::CrisisAlertManager.Properties.Resources.StateAttention;
        private Image m_imgState_Caution = global::CrisisAlertManager.Properties.Resources.StateCaution;
        private Image m_imgState_Alert = global::CrisisAlertManager.Properties.Resources.StateAlert;
        private Image m_imgState_Serious = global::CrisisAlertManager.Properties.Resources.StateSerious;

        public uFormCollapseSensorInfo(CollapseSensor collapseSensor)
        {
            InitializeComponent();
            m_collapseSensor = collapseSensor;
            ShowSensorState(m_collapseSensor.State);

            m_timerSensorReload = new Timer();
            m_timerSensorReload.Interval = 5000;
            m_timerSensorReload.Tick += M_timerSensorReload_Tick;
            m_timerSensorReload.Enabled = true;
        }

        private void M_timerSensorReload_Tick(object sender, EventArgs e)
        {
            CollapseSensor collapseSensor = FormMain.Instance.DataManager.DicCollapseSensors[m_collapseSensor.ID];

            if (!CheckSensorInfo(collapseSensor))
                btnRefresh.Enabled = true;
        }

        private bool CheckSensorInfo(CollapseSensor collapseSensor)
        {
            bool bChk = true;

            if (collapseSensor.Addr != m_collapseSensor.Addr)
                bChk = false;
            else if (collapseSensor.MeasureTime != m_collapseSensor.MeasureTime)
                bChk = false;
            else if (collapseSensor.State != m_collapseSensor.State)
                bChk = false;

            return bChk;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            //FormMessageBox msg = new FormMessageBox("위기경보 판단 데이터 확인", "[경사지 붕괴] 데이터 조회 화면을 나가시겠습니까?\n다시 한번 확인해주세요. ", MessageBoxButtons.YesNo);
            //msg.StartPosition = FormStartPosition.CenterParent;
            //if (msg.ShowDialog() == DialogResult.Yes)
            //{
            //    this.Visible = false;
            //}

            this.Visible = false;
        }

        private void ShowSensorState(string strState)
        {

            InitSensorState();

            if (strState == CommonString.RiskLevel_Normal)
            {
                //pbState.Image = m_imgState_Normal;
                //lbState.Text = CommonString.RiskLevel_Normal_Kor;
            }
            else if (strState == CommonString.RiskLevel_Attention)
            {
                //pbState.Image = m_imgState_Attention;
                //lbState.Text = CommonString.RiskLevel_Attention_Kor;
                plAttentionInfo.BackgroundImage = global::CrisisAlertManager.Properties.Resources.AttentionInfoSign;
            }
            else if (strState == CommonString.RiskLevel_Caution)
            {
                //pbState.Image = m_imgState_Caution;
                //lbState.Text = CommonString.RiskLevel_Caution_Kor;
                plCautionInfo.BackgroundImage = global::CrisisAlertManager.Properties.Resources.CautionInfoSign;
            }
            else if (strState == CommonString.RiskLevel_Alert)
            {
                //pbState.Image = m_imgState_Alert;
                //lbState.Text = CommonString.RiskLevel_Alert_Kor;
                plAlertInfo.BackgroundImage = global::CrisisAlertManager.Properties.Resources.AlertInfoSign;
            }
            else if (strState == CommonString.RiskLevel_Serious)
            {
                //pbState.Image = m_imgState_Serious;
                //lbState.Text = CommonString.RiskLevel_Serious_Kor;
                plSeriousInfo.BackgroundImage = global::CrisisAlertManager.Properties.Resources.SeriousInfoSign;
            }
        }

        private void InitSensorState()
        {
            plAttentionInfo.BackgroundImage = global::CrisisAlertManager.Properties.Resources.AttentionInfo;
            plCautionInfo.BackgroundImage = global::CrisisAlertManager.Properties.Resources.CautionInfo;
            plAlertInfo.BackgroundImage = global::CrisisAlertManager.Properties.Resources.AlertInfo;
            plSeriousInfo.BackgroundImage = global::CrisisAlertManager.Properties.Resources.SeriousInfo;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            FormMessageBox msg = new FormMessageBox("위기경보 판단 데이터 새로고침", "[경사지 붕괴] 위기경보 판단 데이터를 새로고침 하시겠습니까?\n다시 한번 확인해주세요. ", MessageBoxButtons.YesNo);
            msg.StartPosition = FormStartPosition.CenterParent;
            if (msg.ShowDialog() == DialogResult.Yes)
            {
                CollapseSensor collapseSensor = FormMain.Instance.DataManager.DicCollapseSensors[m_collapseSensor.ID];
                m_collapseSensor = collapseSensor;

                ShowSensorInfo(m_collapseSensor);
                ShowSensorState(m_collapseSensor.State);
            }
        }

        private void ShowSensorInfo(CollapseSensor collapseSensor)
        {
            if (collapseSensor == null)
                return;

            DateTime dtDefault = new DateTime();

            string strSensorID = collapseSensor.SensorID;
            string strAddr = collapseSensor.Addr;
            DateTime dtMeasureTime = collapseSensor.MeasureTime;
            string strState = collapseSensor.State;

            lbSensorID.Text = strSensorID;
            lbAddress.Text = strAddr;

            if (dtMeasureTime == dtDefault)
                lbMeasureTime.Text = "-";
            else
                lbMeasureTime.Text = dtMeasureTime.ToString("yyyy.MM.dd hh:mm");

            if (strState == CommonString.RiskLevel_Normal)
                lbState.Text = CommonString.RiskLevel_Normal_Kor;
            else if (strState == CommonString.RiskLevel_Attention)
                lbState.Text = CommonString.RiskLevel_Attention_Kor;
            else if (strState == CommonString.RiskLevel_Caution)
                lbState.Text = CommonString.RiskLevel_Caution_Kor;
            else if (strState == CommonString.RiskLevel_Alert)
                lbState.Text = CommonString.RiskLevel_Alert_Kor;
            else if (strState == CommonString.RiskLevel_Serious)
                lbState.Text = CommonString.RiskLevel_Serious_Kor;

        }

        private void uFormCollapseSensorInfo_Load(object sender, EventArgs e)
        {
            if (m_collapseSensor == null)
                return;

            ShowSensorInfo(m_collapseSensor);
        }
    }
}
