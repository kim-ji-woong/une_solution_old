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
    public partial class uFormFireSensorInfo : UserControl
    {
        FireSensor m_fireSensor = null;
        FireSensor m_compareSensor = null;

        private Timer m_timerSensorReload = null;

        private Image m_imgState_Normal = global::CrisisAlertManager.Properties.Resources.StateNormal;
        private Image m_imgState_Attention = global::CrisisAlertManager.Properties.Resources.StateAttention;
        private Image m_imgState_Caution = global::CrisisAlertManager.Properties.Resources.StateCaution;
        private Image m_imgState_Alert = global::CrisisAlertManager.Properties.Resources.StateAlert;
        private Image m_imgState_Serious = global::CrisisAlertManager.Properties.Resources.StateSerious;

        private Image m_imgRadio_Normal = global::CrisisAlertManager.Properties.Resources.Radio_Normal;
        private Image m_imgRadio_Click = global::CrisisAlertManager.Properties.Resources.Radio_Click;

        private bool m_bAfterFire = false;
        private bool m_bInitReact = false; 

        private string m_strModifiLevel = CommonString.RiskLevel_Normal;

        public uFormFireSensorInfo(FireSensor fireSensor)
        {
            InitializeComponent();
            m_fireSensor = fireSensor;
            InitCompareSensor(fireSensor);
            
            if (fireSensor != null)
                m_strModifiLevel = m_fireSensor.State;

            ShowSensorState(m_strModifiLevel);

            m_timerSensorReload = new Timer();
            m_timerSensorReload.Interval = 5000;
            m_timerSensorReload.Tick += M_timerSensorReload_Tick;
            m_timerSensorReload.Enabled = true;
        }

        private void InitCompareSensor(FireSensor fireSensor)
        {
            if (fireSensor == null)
                return;

            m_compareSensor = new FireSensor();

            m_compareSensor.ID = fireSensor.ID;
            m_compareSensor.SensorID = fireSensor.SensorID;
            m_compareSensor.State = fireSensor.State;
            m_compareSensor.Addr = fireSensor.Addr;
            m_compareSensor.OccurTime = fireSensor.OccurTime;
            m_compareSensor.CloseTime = fireSensor.CloseTime;
            m_compareSensor.AfterFire = fireSensor.AfterFire;
            m_compareSensor.AlarmPeriodStart = fireSensor.AlarmPeriodStart;
            m_compareSensor.AlarmPeriodEnd = fireSensor.AlarmPeriodEnd;
            m_compareSensor.WeakStart = fireSensor.WeakStart;
            m_compareSensor.WeakEnd = fireSensor.WeakEnd;
            m_compareSensor.InitReact = fireSensor.InitReact;
            m_compareSensor.Demander = fireSensor.Demander;
            m_compareSensor.DeathToll = fireSensor.DeathToll;
            m_compareSensor.Message = fireSensor.Message;
            m_compareSensor.UserModifity = fireSensor.UserModifity;
        }

        private void M_timerSensorReload_Tick(object sender, EventArgs e)
        {
            if (m_fireSensor == null)
                return;

            FireSensor fireSensor = FormMain.Instance.DataManager.DicFireSensors[m_fireSensor.ID];

            if (!CheckSensorInfo(fireSensor))
                btnRefresh.Enabled = true;
        }

        private bool CheckSensorInfo(FireSensor fireSensor)
        {
            bool bChk = true;

            if (fireSensor.Addr != m_fireSensor.Addr)
                bChk = false;
            else if (fireSensor.OccurTime != m_fireSensor.OccurTime)
                bChk = false;
            else if (fireSensor.CloseTime != m_fireSensor.CloseTime)
                bChk = false;

            return bChk;
        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            FormMessageBox msg = new FormMessageBox("위기경보 판단 데이터 수정 취소", "[화재] 위기경보 판단 데이터 수정을 모두 취소하고 나가시겠습니까?\n다시 한번 확인해주세요. ", MessageBoxButtons.YesNo);
            msg.StartPosition = FormStartPosition.CenterParent;
            if (msg.ShowDialog() == DialogResult.Yes)
            {
                this.Visible = false;
            }
        }


        private void btnDateStart_Click(object sender, EventArgs e)
        {
            dateTimePicker2.Visible = false;

            if (lblDateStart.Text == "-")
            {
                dateTimePicker1.Value = DateTime.Now;
            }
            else
                dateTimePicker1.Value = Convert.ToDateTime(lblDateStart.Text);

            int x = lblDateStart.Location.X;
            int y = lblDateStart.Location.Y + lblDateStart.Height - dateTimePicker1.Height;

            dateTimePicker1.SendToBack();
            dateTimePicker1.Location = new Point(x, y);
            dateTimePicker1.DropDownAlign = LeftRightAlignment.Left;
            dateTimePicker1.Show();
            dateTimePicker1.Select();
            SendKeys.Send("%{DOWN}");
        }

        private void btnDateEnd_Click(object sender, EventArgs e)
        {
            dateTimePicker1.Visible = false;


            if (lblDateEnd.Text == "-")
            {
                dateTimePicker2.Value = DateTime.Now;
            }
            else
                dateTimePicker2.Value = Convert.ToDateTime(lblDateEnd.Text);


            int x = lblDateEnd.Location.X;
            int y = lblDateEnd.Location.Y + lblDateEnd.Height - dateTimePicker2.Height;

            dateTimePicker2.SendToBack();
            dateTimePicker2.Location = new Point(x, y);
            dateTimePicker2.DropDownAlign = LeftRightAlignment.Left;
            dateTimePicker2.Show();
            dateTimePicker2.Select();
            SendKeys.Send("%{DOWN}");
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            lblDateStart.Text = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            m_fireSensor.AlarmPeriodStart = dateTimePicker1.Value;
            CheckSensorState();
        }
        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            lblDateEnd.Text = dateTimePicker2.Value.ToString("yyyy-MM-dd");
            m_fireSensor.AlarmPeriodEnd = dateTimePicker2.Value;
            CheckSensorState();
        }

        private void dateTimePicker3_ValueChanged(object sender, EventArgs e)
        {
            lbWeakStart.Text = dateTimePicker3.Value.ToString("yyyy-MM-dd");
            m_fireSensor.WeakStart = dateTimePicker3.Value;
            CheckSensorState();
        }

        private void btnWeakStart_Click(object sender, EventArgs e)
        {
            dateTimePicker4.Visible = false;

            if (lbWeakStart.Text == "-")
            {
                dateTimePicker3.Value = DateTime.Now;
            }
            else
                dateTimePicker3.Value = Convert.ToDateTime(lbWeakStart.Text);



            int x = lbWeakStart.Location.X;
            int y = lbWeakStart.Location.Y + lbWeakStart.Height - dateTimePicker3.Height;

            dateTimePicker3.SendToBack();
            dateTimePicker3.Location = new Point(x, y);
            dateTimePicker3.DropDownAlign = LeftRightAlignment.Left;
            dateTimePicker3.Show();
            dateTimePicker3.Select();
            SendKeys.Send("%{DOWN}");
        }

        private void btnWeakEnd_Click(object sender, EventArgs e)
        {
            dateTimePicker3.Visible = false;

            if (lbWeakEnd.Text == "-")
            {
                dateTimePicker4.Value = DateTime.Now;
            }
            else
                dateTimePicker4.Value = Convert.ToDateTime(lbWeakEnd.Text);


            int x = lbWeakEnd.Location.X;
            int y = lbWeakEnd.Location.Y + lbWeakEnd.Height - dateTimePicker4.Height;

            dateTimePicker4.SendToBack();
            dateTimePicker4.Location = new Point(x, y);
            dateTimePicker4.DropDownAlign = LeftRightAlignment.Left;
            dateTimePicker4.Show();
            dateTimePicker4.Select();
            SendKeys.Send("%{DOWN}");
        }

        private void dateTimePicker4_ValueChanged(object sender, EventArgs e)
        {
            lbWeakEnd.Text = dateTimePicker4.Value.ToString("yyyy-MM-dd");
            m_fireSensor.WeakEnd = dateTimePicker4.Value;
            CheckSensorState();
        }

        private void uFormFireSensorInfo_Load(object sender, EventArgs e)
        {
            if (m_fireSensor == null)
                return;

            ShowSensorInfo(m_fireSensor);
        }

        private void ShowSensorInfo(FireSensor fireSensor)
        {
            if (fireSensor == null)
                return;

            DateTime dtDefault = new DateTime();

            string strSensorID = fireSensor.SensorID;
            string strAddr = fireSensor.Addr;
            DateTime dtOccurTime = fireSensor.OccurTime;
            DateTime dtCloseTime = fireSensor.CloseTime;
            bool bAfterFire = fireSensor.AfterFire;
            DateTime dtAlarmPeriodStart = fireSensor.AlarmPeriodStart;
            DateTime dtAlarmPeriodEnd = fireSensor.AlarmPeriodEnd;
            DateTime dtWeakStart = fireSensor.WeakStart;
            DateTime dtWeakEnd = fireSensor.WeakEnd;
            int nInitReact = fireSensor.InitReact;
            int nDemander = fireSensor.Demander;
            int nDeathToll = fireSensor.DeathToll;

            lbSensorID.Text = strSensorID;
            lbAddress.Text = strAddr;

            if (dtOccurTime == dtDefault)
                lbOccurTime.Text = "-";
            else
                lbOccurTime.Text = dtOccurTime.ToString("yyyy.MM.dd hh:mm");

            if (dtCloseTime == dtDefault)
                lbCloseTime.Text = "-";
            else
                lbCloseTime.Text = dtCloseTime.ToString("yyyy.MM.dd hh:mm");

            if (bAfterFire == true)
            {
                m_bAfterFire = true;
                SelectAfterFireYes();

            }
            else //(bAfterFire == false)
            {
                m_bAfterFire = false;
                SelectAfterFireNo();
            }
                

            if (dtAlarmPeriodStart == dtDefault)
                lblDateStart.Text = "-";
            else
                lblDateStart.Text = dtAlarmPeriodStart.ToString("yyyy-MM-dd");

            if (dtAlarmPeriodEnd == dtDefault)
                lblDateEnd.Text = "-";
            else
                lblDateEnd.Text = dtAlarmPeriodEnd.ToString("yyyy-MM-dd");

            if (dtWeakStart == dtDefault)
                lbWeakStart.Text = "-";
            else
                lbWeakStart.Text = dtWeakStart.ToString("yyyy-MM-dd");

            if (dtWeakEnd == dtDefault)
                lbWeakEnd.Text = "-";
            else
                lbWeakEnd.Text = dtWeakEnd.ToString("yyyy-MM-dd");

            if (nInitReact == 0)
            {
                m_bInitReact = false;
                SelectInitReactNo();

            }
            else if (nInitReact == 1)
            {
                m_bInitReact = true;

                SelectInitReactYes();
            }

            textBoxDemander.Text = nDemander.ToString();
            textBoxDeathToll.Text = nDeathToll.ToString();
        }


        private void btnRefresh_Click(object sender, EventArgs e)
        {
            FormMessageBox msg = new FormMessageBox("위기경보 판단 데이터 새로고침", "[화재] 위기경보 판단 데이터를 새로고침 하시겠습니까?\n기존 수정된 데이터가 모두 초기화 되어 최신화 됩니다.\n다시 한번 확인해주세요. ", MessageBoxButtons.YesNo);
            msg.StartPosition = FormStartPosition.CenterParent;
            if (msg.ShowDialog() == DialogResult.Yes)
            {
                FireSensor fireSensor = FormMain.Instance.DataManager.DicFireSensors[m_fireSensor.ID];
                m_fireSensor = fireSensor;
                InitCompareSensor(fireSensor);
                m_strModifiLevel = m_fireSensor.State;

                ShowSensorInfo(m_fireSensor);
                ShowSensorState(m_fireSensor.State);
            }

            
        }

        private void CheckSensorState()
        {
            bool bChek = false;
            int nResult = 0;
            DateTime dtDefault = new DateTime();

            nResult = DateTime.Compare(DateTime.Now, m_fireSensor.AlarmPeriodStart);
            if (nResult > 0 && dtDefault != m_fireSensor.AlarmPeriodStart && (m_strModifiLevel == CommonString.RiskLevel_Normal))
            {
                m_fireSensor.State = CommonString.RiskLevel_Attention;
                ShowSensorState(CommonString.RiskLevel_Attention);
                bChek = true;
            }

            nResult = DateTime.Compare(DateTime.Now, m_fireSensor.AlarmPeriodEnd);
            if (nResult < 0 && dtDefault != m_fireSensor.AlarmPeriodEnd && (m_strModifiLevel == CommonString.RiskLevel_Normal))
            {
                m_fireSensor.State = CommonString.RiskLevel_Attention;
                ShowSensorState(CommonString.RiskLevel_Attention);
                bChek = true;
            }

            nResult = DateTime.Compare(DateTime.Now, m_fireSensor.WeakStart);
            if (nResult > 0 && dtDefault != m_fireSensor.WeakStart && (m_strModifiLevel == CommonString.RiskLevel_Normal))
            {
                m_fireSensor.State = CommonString.RiskLevel_Attention;
                ShowSensorState(CommonString.RiskLevel_Attention);
                bChek = true;
            }

            nResult = DateTime.Compare(DateTime.Now, m_fireSensor.WeakEnd);
            if (nResult < 0 && dtDefault != m_fireSensor.WeakEnd && (m_strModifiLevel == CommonString.RiskLevel_Normal))
            {
                m_fireSensor.State = CommonString.RiskLevel_Attention;
                ShowSensorState(CommonString.RiskLevel_Attention);
                bChek = true;
            }

            if (m_fireSensor.AfterFire == true && (m_strModifiLevel == CommonString.RiskLevel_Normal || m_strModifiLevel == CommonString.RiskLevel_Attention))
            {
                m_fireSensor.State = CommonString.RiskLevel_Caution;
                ShowSensorState(CommonString.RiskLevel_Caution);
                bChek = true;
            }

            if (m_fireSensor.InitReact == 1 || m_fireSensor.Demander > 0 && (m_strModifiLevel == CommonString.RiskLevel_Normal || m_strModifiLevel == CommonString.RiskLevel_Attention || m_strModifiLevel == CommonString.RiskLevel_Caution))
            {
                m_fireSensor.State = CommonString.RiskLevel_Alert;
                ShowSensorState(CommonString.RiskLevel_Alert);
                bChek = true;
            }

            if (m_fireSensor.DeathToll > 0 && (m_strModifiLevel == CommonString.RiskLevel_Normal || m_strModifiLevel == CommonString.RiskLevel_Attention || m_strModifiLevel == CommonString.RiskLevel_Caution || m_strModifiLevel == CommonString.RiskLevel_Alert))
            {
                m_fireSensor.State = CommonString.RiskLevel_Serious;
                ShowSensorState(CommonString.RiskLevel_Serious);
                bChek = true;
            }

            if (bChek == false)
            {
                m_fireSensor.State = m_strModifiLevel;
                ShowSensorState(m_strModifiLevel);
            }

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

        private void textBoxDemander_TextChanged(object sender, EventArgs e)
        {
            if (textBoxDemander.Text == "")
                textBoxDemander.Text = string.Format("0");

            m_fireSensor.Demander = Int32.Parse(textBoxDemander.Text);
            CheckSensorState();
        }

        private void textBoxDeathToll_TextChanged(object sender, EventArgs e)
        {
            if (textBoxDeathToll.Text == "")
                textBoxDeathToll.Text = string.Format("0");

            m_fireSensor.DeathToll = Int32.Parse(textBoxDeathToll.Text);
            CheckSensorState();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            FormMessageBox msg = new FormMessageBox("위기경보 판단 데이터 수정", "[화재] 위기경보 판단 데이터를 변경하고 나가시겠습니까?\n다시 한번 확인해주세요. ", MessageBoxButtons.YesNo);
            msg.StartPosition = FormStartPosition.CenterParent;
            if (msg.ShowDialog() == DialogResult.Yes)
            {
                FormMain.Instance.DataManager.UpdateFireSensorInfo(m_fireSensor);
                CheckModifityReport();      // 리포트
                CheckAlertAlarm();          // 알람 신호
                uFormCrisisAlert.Instance.M_timerSensorReload_Tick(null, null);

                this.Visible = false;
            }
        }

        private void CheckModifityReport()
        {
            if (m_fireSensor.AfterFire != m_compareSensor.AfterFire)
            {
                string strDataName = CommonString.FireSensor_AfterFire_Kor;
                string strOldData = "";
                string strNewData = "";

                if (m_compareSensor.AfterFire)
                    strOldData = "YES";
                else
                    strOldData = "NO";

                if (m_fireSensor.AfterFire)
                    strNewData = "YES";
                else
                    strNewData = "NO";

                FormMain.Instance.DataManager.InsertDataReport(FacilityType.FIRE_SENSOR, m_fireSensor.ID, strDataName, strOldData, strNewData);

            }

            if (m_fireSensor.AlarmPeriodStart != m_compareSensor.AlarmPeriodStart || m_fireSensor.AlarmPeriodEnd != m_compareSensor.AlarmPeriodEnd)
            {
                DateTime dtDefault = new DateTime();

                string strDataName = CommonString.FireSensor_AlarmPeriod_Kor;
                string strOldData = "";
                string strNewData = "";
                string strAlarmPeriodStart = "";
                string strAlarmPeriodEnd = "";

                if (m_compareSensor.AlarmPeriodStart == dtDefault)
                    strAlarmPeriodStart = "";
                else
                    strAlarmPeriodStart = m_compareSensor.AlarmPeriodStart.ToString("yyyy-MM-dd");

                if (m_compareSensor.AlarmPeriodEnd == dtDefault)
                    strAlarmPeriodEnd = "";
                else
                    strAlarmPeriodEnd = m_compareSensor.AlarmPeriodEnd.ToString("yyyy-MM-dd");

                strOldData = strAlarmPeriodStart + " ~ " + strAlarmPeriodEnd;

                if (m_fireSensor.AlarmPeriodStart == dtDefault)
                    strAlarmPeriodStart = "";
                else
                    strAlarmPeriodStart = m_fireSensor.AlarmPeriodStart.ToString("yyyy-MM-dd");

                if (m_fireSensor.AlarmPeriodEnd == dtDefault)
                    strAlarmPeriodEnd = "";
                else
                    strAlarmPeriodEnd = m_fireSensor.AlarmPeriodEnd.ToString("yyyy-MM-dd");

                strNewData = strAlarmPeriodStart + " ~ " + strAlarmPeriodEnd;


                FormMain.Instance.DataManager.InsertDataReport(FacilityType.FIRE_SENSOR, m_fireSensor.ID, strDataName, strOldData, strNewData); 
            }

            if (m_fireSensor.WeakStart != m_compareSensor.WeakStart || m_fireSensor.WeakEnd != m_compareSensor.WeakEnd)
            {
                DateTime dtDefault = new DateTime();

                string strDataName = CommonString.FireSensor_Weak_Kor;
                string strOldData = "";
                string strNewData = "";
                string strWeakStart = "";
                string strWeakEnd = "";

                if (m_compareSensor.WeakStart == dtDefault)
                    strWeakStart = "";
                else
                    strWeakStart = m_compareSensor.WeakStart.ToString("yyyy-MM-dd");

                if (m_compareSensor.WeakEnd == dtDefault)
                    strWeakEnd = "";
                else
                    strWeakEnd = m_compareSensor.WeakEnd.ToString("yyyy-MM-dd");

                strOldData = strWeakStart + " ~ " + strWeakEnd;

                if (m_fireSensor.WeakStart == dtDefault)
                    strWeakStart = "";
                else
                    strWeakStart = m_fireSensor.WeakStart.ToString("yyyy-MM-dd");

                if (m_fireSensor.WeakEnd == dtDefault)
                    strWeakEnd = "";
                else
                    strWeakEnd = m_fireSensor.WeakEnd.ToString("yyyy-MM-dd");

                strNewData = strWeakStart + " ~ " + strWeakEnd;

                FormMain.Instance.DataManager.InsertDataReport(FacilityType.FIRE_SENSOR, m_fireSensor.ID, strDataName, strOldData, strNewData);
            }

            if (m_fireSensor.InitReact != m_compareSensor.InitReact)
            {
                string strDataName = CommonString.FireSensor_InitReact_Kor;
                string strOldData = "";
                string strNewData = "";

                if (m_compareSensor.InitReact == 0)
                    strOldData = "NO";
                else if (m_compareSensor.InitReact == 1)
                    strOldData = "YES";
                //else if (m_compareSensor.InitReact == 2)
                //    strOldData = "NO";

                if (m_fireSensor.InitReact == 0)
                    strNewData = "NO";
                else if (m_fireSensor.InitReact == 1)
                    strNewData = "YES";
                //else if (m_fireSensor.InitReact == 2)
                //    strNewData = "NO";

                FormMain.Instance.DataManager.InsertDataReport(FacilityType.FIRE_SENSOR, m_fireSensor.ID, strDataName, strOldData, strNewData);
            }

            if (m_fireSensor.Demander != m_compareSensor.Demander)
            {
                string strDataName = CommonString.Demander_Kor;
                string strOldData = m_compareSensor.Demander.ToString();
                string strNewData = m_fireSensor.Demander.ToString();

                FormMain.Instance.DataManager.InsertDataReport(FacilityType.FIRE_SENSOR, m_fireSensor.ID, strDataName, strOldData, strNewData);
            }

            if (m_fireSensor.DeathToll != m_compareSensor.DeathToll)
            {
                string strDataName = CommonString.DeathTollr_Kor;
                string strOldData = m_compareSensor.DeathToll.ToString();
                string strNewData = m_fireSensor.DeathToll.ToString();

                FormMain.Instance.DataManager.InsertDataReport(FacilityType.FIRE_SENSOR, m_fireSensor.ID, strDataName, strOldData, strNewData);
            }

            if (m_fireSensor.State != m_compareSensor.State)
            {
                string strOldData = TransRiskLevel(m_compareSensor.State);
                string strNewData = TransRiskLevel(m_fireSensor.State);
                string strDataName = CommonString.GetRiskDataName(strOldData, strNewData);

                FormMain.Instance.DataManager.InsertAlertReport(FacilityType.FIRE_SENSOR, m_fireSensor.ID, strDataName, strOldData, strNewData);
            }
        }

        private bool CheckAlertAlarm()
        {
            bool bRet = true;

            if (m_fireSensor.State != m_compareSensor.State)
            {
                int nID = m_fireSensor.ID;
                string strAddress = m_fireSensor.Addr;
                string strRiskLevel = TransRiskLevel(m_fireSensor.State);

                if (!FormMain.Instance.DataManager.InsertAlertAarm(FacilityType.FIRE_SENSOR, nID, strAddress, strRiskLevel))
                    bRet = false;
            }

            return bRet;
        }

        private string TransRiskLevel(string strRiskLevel)
        {
            string strRiskKor = "";

            if (strRiskLevel == CommonString.RiskLevel_Normal)
                strRiskKor = CommonString.RiskLevel_Normal_Kor;
            else if (strRiskLevel == CommonString.RiskLevel_Attention)
                strRiskKor = CommonString.RiskLevel_Attention_Kor;
            else if (strRiskLevel == CommonString.RiskLevel_Caution)
                strRiskKor = CommonString.RiskLevel_Caution_Kor;
            else if (strRiskLevel == CommonString.RiskLevel_Alert)
                strRiskKor = CommonString.RiskLevel_Alert_Kor;
            else if (strRiskLevel == CommonString.RiskLevel_Serious)
                strRiskKor = CommonString.RiskLevel_Serious_Kor;

            return strRiskKor;
        }

        private void btnAlarmPeriodRefresh_Click(object sender, EventArgs e)
        {
            DateTime dtDefault = new DateTime();

            m_fireSensor.AlarmPeriodStart = m_compareSensor.AlarmPeriodStart;
            m_fireSensor.AlarmPeriodEnd = m_compareSensor.AlarmPeriodEnd;

            DateTime dtAlarmPeriodStart = m_compareSensor.AlarmPeriodStart;
            DateTime dtAlarmPeriodEnd = m_compareSensor.AlarmPeriodEnd;

            if (dtAlarmPeriodStart == dtDefault)
                lblDateStart.Text = "-";
            else
                lblDateStart.Text = dtAlarmPeriodStart.ToString("yyyy-MM-dd");

            if (dtAlarmPeriodEnd == dtDefault)
                lblDateEnd.Text = "-";
            else
                lblDateEnd.Text = dtAlarmPeriodEnd.ToString("yyyy-MM-dd");

            CheckSensorState();
        }

        private void btnWeakRefresh_Click(object sender, EventArgs e)
        {
            DateTime dtDefault = new DateTime();

            m_fireSensor.WeakStart = m_compareSensor.WeakStart;
            m_fireSensor.WeakEnd = m_compareSensor.WeakEnd;

            DateTime dtWeakStart = m_compareSensor.WeakStart;
            DateTime dtWeakEnd = m_compareSensor.WeakEnd;

            if (dtWeakStart == dtDefault)
                lbWeakStart.Text = "-";
            else
                lbWeakStart.Text = dtWeakStart.ToString("yyyy-MM-dd");

            if (dtWeakEnd == dtDefault)
                lbWeakEnd.Text = "-";
            else
                lbWeakEnd.Text = dtWeakEnd.ToString("yyyy-MM-dd");

            CheckSensorState();
        }

        private void btnInitReactRefresh_Click(object sender, EventArgs e)
        {
            m_fireSensor.InitReact = m_compareSensor.InitReact;
            int nInitReact = m_compareSensor.InitReact;

            if (nInitReact == 0)
            {
                m_bInitReact = false;
                SelectInitReactNo();

            }
            else if (nInitReact == 1)
            {
                m_bInitReact = true;
                SelectInitReactYes();
            }

            CheckSensorState();
        }

        private void btnDemanderRefresh_Click(object sender, EventArgs e)
        {
            m_fireSensor.Demander = m_compareSensor.Demander;
            int nDemander = m_compareSensor.Demander;
            textBoxDemander.Text = nDemander.ToString();
        }

        private void btnDeathTollRefresh_Click(object sender, EventArgs e)
        {
            m_fireSensor.DeathToll = m_compareSensor.DeathToll;
            int nDeathToll = m_compareSensor.DeathToll;
            textBoxDeathToll.Text = nDeathToll.ToString();
        }

        private void textBoxDemander_KeyPress(object sender, KeyPressEventArgs e)
        {
            //숫자만 입력되도록 필터링
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))    //숫자와 백스페이스를 제외한 나머지를 바로 처리
            {
                e.Handled = true;
            }
        }

        private void textBoxDeathToll_KeyPress(object sender, KeyPressEventArgs e)
        {
            //숫자만 입력되도록 필터링
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))    //숫자와 백스페이스를 제외한 나머지를 바로 처리
            {
                e.Handled = true;
            }
        }


        private void btnAfterFireYes_Click(object sender, EventArgs e)
        {
            if (m_bAfterFire == false)
            {
                m_bAfterFire = true;
                m_fireSensor.AfterFire = true;

                SelectAfterFireYes();

                CheckSensorState();
            }
        }

        private void SelectAfterFireYes()
        {
            btnAfterFireYes.ImageNormal = m_imgRadio_Click;
            btnAfterFireYes.ImageMouseOver = m_imgRadio_Click;
            btnAfterFireNo.ImageNormal = m_imgRadio_Normal;
            btnAfterFireNo.ImageMouseOver = m_imgRadio_Normal;

            btnAfterFireYes.Refresh();
            btnAfterFireNo.Refresh();
        }

        private void btnAfterFireNo_Click(object sender, EventArgs e)
        {
            if (m_bAfterFire == true)
            {
                m_bAfterFire = false;
                m_fireSensor.AfterFire = false;

                SelectAfterFireNo();

                CheckSensorState();
            }
        }

        private void SelectAfterFireNo()
        {
            btnAfterFireYes.ImageNormal = m_imgRadio_Normal;
            btnAfterFireYes.ImageMouseOver = m_imgRadio_Normal;
            btnAfterFireNo.ImageNormal = m_imgRadio_Click;
            btnAfterFireNo.ImageMouseOver = m_imgRadio_Click;

            btnAfterFireYes.Refresh();
            btnAfterFireNo.Refresh();
        }

        private void btnInitReactYes_Click(object sender, EventArgs e)
        {
            if (m_bInitReact == false)
            {
                m_bInitReact = true;
                m_fireSensor.InitReact = 1;

                SelectInitReactYes();

                CheckSensorState();
            }
        }

        private void SelectInitReactYes()
        {
            btnInitReactYes.ImageNormal = m_imgRadio_Click;
            btnInitReactYes.ImageMouseOver = m_imgRadio_Click;
            btnInitReactNo.ImageNormal = m_imgRadio_Normal;
            btnInitReactNo.ImageMouseOver = m_imgRadio_Normal;

            btnInitReactYes.Refresh();
            btnInitReactNo.Refresh();
        }

        private void btnInitReactNo_Click(object sender, EventArgs e)
        {
            if (m_bInitReact == true)
            {
                m_bInitReact = false;
                m_fireSensor.InitReact = 0;

                SelectInitReactNo();

                CheckSensorState();
            }
        }

        private void SelectInitReactNo()
        {
            btnInitReactYes.ImageNormal = m_imgRadio_Normal;
            btnInitReactYes.ImageMouseOver = m_imgRadio_Normal;
            btnInitReactNo.ImageNormal = m_imgRadio_Click;
            btnInitReactNo.ImageMouseOver = m_imgRadio_Click;

            btnInitReactYes.Refresh();
            btnInitReactNo.Refresh();
        }
    }
}
