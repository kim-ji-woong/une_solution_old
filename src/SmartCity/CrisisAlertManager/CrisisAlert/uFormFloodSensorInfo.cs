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
    public partial class uFormFloodSensorInfo : UserControl
    {
        private Image m_imgState_Normal = global::CrisisAlertManager.Properties.Resources.StateNormal;
        private Image m_imgState_Attention = global::CrisisAlertManager.Properties.Resources.StateAttention;
        private Image m_imgState_Caution = global::CrisisAlertManager.Properties.Resources.StateCaution;
        private Image m_imgState_Alert = global::CrisisAlertManager.Properties.Resources.StateAlert;
        private Image m_imgState_Serious = global::CrisisAlertManager.Properties.Resources.StateSerious;

        private FloodSensor m_floodSensor = null;

        private Timer m_timerSensorReload = null;

        public uFormFloodSensorInfo(FloodSensor floodSensor)
        {
            InitializeComponent();
            m_floodSensor = floodSensor;

            ShowSensorState(m_floodSensor.State);

            m_timerSensorReload = new Timer();
            m_timerSensorReload.Interval = 5000;
            m_timerSensorReload.Tick += M_timerSensorReload_Tick;
            m_timerSensorReload.Enabled = true;
        }

        private void M_timerSensorReload_Tick(object sender, EventArgs e)
        {
            FloodSensor floodSensor = FormMain.Instance.DataManager.DicFloodSensors[m_floodSensor.ID];

            if (!CheckSensorInfo(floodSensor))
                btnRefresh.Enabled = true;
        }

        private bool CheckSensorInfo(FloodSensor floodSensor)
        {
            bool bChk = true;

            if (floodSensor.Addr != m_floodSensor.Addr)
                bChk = false;
            else if (floodSensor.MeasureTime != m_floodSensor.MeasureTime)
                bChk = false;
            else if (floodSensor.Depth != m_floodSensor.Depth)
                bChk = false;
            else if (floodSensor.Flow != m_floodSensor.Flow)
                bChk = false;

            return bChk;
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

        private void uFormFloodSensorInfo_Load(object sender, EventArgs e)
        {
            if (m_floodSensor == null)
                return;

            ShowSensorInfo(m_floodSensor);
        }

        private void ShowSensorInfo(FloodSensor floodSensor)
        {
            if (floodSensor == null)
                return;

            DateTime dtDefault = new DateTime();

            string strSensorID = floodSensor.SensorID;
            string strAddr = floodSensor.Addr;
            DateTime dtMeasureTime = floodSensor.MeasureTime;
            float fDepth = floodSensor.Depth;
            float fFlow = floodSensor.Flow;

            lbSensorID.Text = strSensorID;
            lbAddress.Text = strAddr;

            if (dtMeasureTime == dtDefault)
                lbMeasureTime.Text = "-";
            else
                lbMeasureTime.Text = dtMeasureTime.ToString("yyyy.MM.dd hh:mm");

            lbDepth.Text = fDepth.ToString();
            lbFlow.Text = fFlow.ToString();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            //FormMessageBox msg = new FormMessageBox("위기경보 판단 데이터 확인", "[홍수] 데이터 조회 화면을 나가시겠습니까?\n다시 한번 확인해주세요. ", MessageBoxButtons.YesNo);
            //msg.StartPosition = FormStartPosition.CenterParent;
            //if (msg.ShowDialog() == DialogResult.Yes)
            //{
            //    this.Visible = false;
            //}

            this.Visible = false;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            FormMessageBox msg = new FormMessageBox("위기경보 판단 데이터 새로고침", "[홍수] 위기경보 판단 데이터를 새로고침 하시겠습니까?\n기존 수정된 데이터가 모두 초기화 되어 최신화 됩니다.\n다시 한번 확인해주세요. ", MessageBoxButtons.YesNo);
            msg.StartPosition = FormStartPosition.CenterParent;
            if (msg.ShowDialog() == DialogResult.Yes)
            {
                FloodSensor floodSensor = FormMain.Instance.DataManager.DicFloodSensors[m_floodSensor.ID];
                m_floodSensor = floodSensor;

                ShowSensorInfo(m_floodSensor);
                ShowSensorState(m_floodSensor.State);
            }
        }
    }
}
