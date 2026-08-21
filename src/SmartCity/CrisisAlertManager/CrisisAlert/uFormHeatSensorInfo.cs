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
    public partial class uFormHeatSensorInfo : UserControl
    {
        private Image m_imgState_Normal = global::CrisisAlertManager.Properties.Resources.StateNormal;
        private Image m_imgState_Attention = global::CrisisAlertManager.Properties.Resources.StateAttention;
        private Image m_imgState_Caution = global::CrisisAlertManager.Properties.Resources.StateCaution;
        private Image m_imgState_Alert = global::CrisisAlertManager.Properties.Resources.StateAlert;
        private Image m_imgState_Serious = global::CrisisAlertManager.Properties.Resources.StateSerious;

        HeatSensor m_heatSensor = null;
        HeatSensor m_compareSensor = null;
        private string m_strModifiLevel = CommonString.RiskLevel_Normal;
        private Timer m_timerSensorReload = null;

        public uFormHeatSensorInfo(HeatSensor heatSensor)
        {
            InitializeComponent();
            m_heatSensor = heatSensor;
            InitCompareSensor(heatSensor);
            m_strModifiLevel = m_heatSensor.State;

            ShowSensorState(m_strModifiLevel);
            ShowDay();
            ShowTempByDay();

            m_timerSensorReload = new Timer();
            m_timerSensorReload.Interval = 5000;
            m_timerSensorReload.Tick += M_timerSensorReload_Tick;
            m_timerSensorReload.Enabled = true;
        }

        private void ShowDay()
        {
            DateTime dtToday = DateTime.Today;
            
            DateTime dtbeforeOne = dtToday.AddDays(-1);
            DateTime dtbeforeTwo = dtToday.AddDays(-2);
            DateTime dtbeforeThree = dtToday.AddDays(-3);
            DateTime dtbeforeFour = dtToday.AddDays(-4);

            DateTime dtAfterOne = dtToday.AddDays(1);
            DateTime dtAfterTwo = dtToday.AddDays(2);
            DateTime dtAfterThree = dtToday.AddDays(3);
            DateTime dtAfterFour = dtToday.AddDays(4);
            DateTime dtAfterFive = dtToday.AddDays(5);

            lbToday.Text = GetDay(dtToday);
            lbbeforeOne.Text = GetDay(dtbeforeOne);
            lbbeforeTwo.Text = GetDay(dtbeforeTwo);
            lbbeforeThree.Text = GetDay(dtbeforeThree);
            lbbeforeFour.Text = GetDay(dtbeforeFour);

            lbAfterOne.Text = GetDay(dtAfterOne);
            lbAfterTwo.Text = GetDay(dtAfterTwo);
            lbAfterThree.Text = GetDay(dtAfterThree);
            lbAfterFour.Text = GetDay(dtAfterFour);
            lbAfterFive.Text = GetDay(dtAfterFive);

            lbbeforeTwoDay.Text = dtbeforeTwo.ToString("M.dd");
            lbbeforeThreeDay.Text = dtbeforeThree.ToString("M.dd");
            lbbeforeFourDay.Text = dtbeforeFour.ToString("M.dd");

            lbAfterOneDay.Text = dtAfterOne.ToString("M.dd");
            lbAfterTwoDay.Text = dtAfterTwo.ToString("M.dd");
            lbAfterThreeDay.Text = dtAfterThree.ToString("M.dd");
            lbAfterFourDay.Text = dtAfterFour.ToString("M.dd");
            lbAfterFiveDay.Text = dtAfterFive.ToString("M.dd");
        }

        private void ShowTempByDay()
        {
            // 오늘 예상 최고 기온을 얻기 위해서 어제 날짜 데이터를 가져와 표시
            DateTime dtToday = DateTime.Today;
            dtToday = dtToday.AddDays(-1);

            string strToday = dtToday.ToString("yyyyMMdd");
            DataExpectTemp expectTemp = null;
            DataBeforMaxTemp beforMaxTemp = null;

            expectTemp = FormMain.Instance.DataManager.GetExpectTemp(strToday);

            if (expectTemp == null)
            {
                lbTodayTemp.Text = "-";
                lbAfterOneTemp.Text = "-";
                lbAfterTwoTemp.Text = "-";
                lbAfterThreeTemp.Text = "-";
                lbAfterFourTemp.Text = "-";
                lbAfterFiveTemp.Text = "-";
            }
            else
            {
                lbTodayTemp.Text = expectTemp.AfterOneDay + "℃";
                lbAfterOneTemp.Text = expectTemp.AfterTwoDay + "℃";
                lbAfterTwoTemp.Text = expectTemp.AfterThreeDay + "℃";
                lbAfterThreeTemp.Text = expectTemp.AfterFourDay + "℃";
                lbAfterFourTemp.Text = expectTemp.AfterFiveDay + "℃";
                lbAfterFiveTemp.Text = expectTemp.AfterSixDay + "℃";
            }

            beforMaxTemp = FormMain.Instance.DataManager.GetBeforMaxTemp(m_heatSensor.SensorID);

            if (beforMaxTemp == null)
            {
                lbbeforeOneTemp.Text = "-";
                lbbeforeTwoTemp.Text = "-";
                lbbeforeThreeTemp.Text = "-";
                lbbeforeFourTemp.Text = "-";
            }
            else
            {
                lbbeforeOneTemp.Text = beforMaxTemp.BeforeOneDay;
                lbbeforeTwoTemp.Text = beforMaxTemp.BeforeTwoDay;
                lbbeforeThreeTemp.Text = beforMaxTemp.BeforeThreeDay;
                lbbeforeFourTemp.Text = beforMaxTemp.BeforeFourDay;
            }

        }

        private string GetDay(DateTime dt)
        {
            string strDay = "";

            switch (dt.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    strDay = "월";
                    break;
                case DayOfWeek.Tuesday:
                    strDay = "화";
                    break;
                case DayOfWeek.Wednesday:
                    strDay = "수";
                    break;
                case DayOfWeek.Thursday:
                    strDay = "목";
                    break;
                case DayOfWeek.Friday:
                    strDay = "금";
                    break;
                case DayOfWeek.Saturday:
                    strDay = "토";
                    break;
                case DayOfWeek.Sunday:
                    strDay = "일";
                    break;
            }

            return strDay;
        }

        private void InitCompareSensor(HeatSensor heatSensor)
        {
            m_compareSensor = new HeatSensor();

            m_compareSensor.ID = heatSensor.ID;
            m_compareSensor.SensorID = heatSensor.SensorID;
            m_compareSensor.State = heatSensor.State;
            m_compareSensor.Addr = heatSensor.Addr;
            m_compareSensor.OccurTime = heatSensor.OccurTime;
            m_compareSensor.Temperature = heatSensor.Temperature;
            m_compareSensor.Humidity = heatSensor.Humidity;
            m_compareSensor.Speed = heatSensor.Speed;
            m_compareSensor.MeasPeriodStart = heatSensor.MeasPeriodStart;
            m_compareSensor.MeasPeriodEnd = heatSensor.MeasPeriodEnd;
            m_compareSensor.PreliminaryDate = heatSensor.PreliminaryDate;
            m_compareSensor.AdvisoryDate = heatSensor.AdvisoryDate;
            m_compareSensor.AlertDate = heatSensor.AlertDate;
            m_compareSensor.DeathToll = heatSensor.DeathToll;
            m_compareSensor.Message = heatSensor.Message;
            m_compareSensor.UserModifity = heatSensor.UserModifity;
        }

        private void M_timerSensorReload_Tick(object sender, EventArgs e)
        {
            HeatSensor heatSensor = FormMain.Instance.DataManager.DicHeatSensors[m_heatSensor.ID];

            if (!CheckSensorInfo(heatSensor))
                btnRefresh.Enabled = true;
        }

        private bool CheckSensorInfo(HeatSensor heatSensor)
        {
            bool bChk = true;

            if (heatSensor.Addr != m_heatSensor.Addr)
                bChk = false;
            else if (heatSensor.OccurTime != m_heatSensor.OccurTime)
                bChk = false;
            else if (heatSensor.Temperature != m_heatSensor.Temperature)
                bChk = false;
            else if (heatSensor.Humidity != m_heatSensor.Humidity)
                bChk = false;
            else if (heatSensor.Direction != m_heatSensor.Direction)
                bChk = false;
            else if (heatSensor.Speed != m_heatSensor.Speed)
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

        private void btnCancle_Click(object sender, EventArgs e)
        {
            FormMessageBox msg = new FormMessageBox("위기경보 판단 데이터 수정 취소", "[폭염] 위기경보 판단 데이터 수정을 모두 취소하고 나가시겠습니까?\n다시 한번 확인해주세요. ", MessageBoxButtons.YesNo);
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
            m_heatSensor.MeasPeriodStart = dateTimePicker1.Value;
            CheckSensorState();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            lblDateEnd.Text = dateTimePicker2.Value.ToString("yyyy-MM-dd");
            m_heatSensor.MeasPeriodEnd = dateTimePicker2.Value;
            CheckSensorState();
        }

        private void btnPreliminaryDate_Click(object sender, EventArgs e)
        {
            if (lbPreliminaryDate.Text == "-")
            {
                dateTimePicker3.Value = DateTime.Now;
            }
            else
                dateTimePicker3.Value = Convert.ToDateTime(lbPreliminaryDate.Text);

            int x = lbPreliminaryDate.Location.X;
            int y = lbPreliminaryDate.Location.Y + lbPreliminaryDate.Height - dateTimePicker3.Height;

            dateTimePicker3.SendToBack();
            dateTimePicker3.Location = new Point(x, y);
            dateTimePicker3.DropDownAlign = LeftRightAlignment.Left;
            dateTimePicker3.Show();
            dateTimePicker3.Select();
            SendKeys.Send("%{DOWN}");
        }

        private void dateTimePicker3_ValueChanged(object sender, EventArgs e)
        {
            lbPreliminaryDate.Text = dateTimePicker3.Value.ToString("yyyy-MM-dd");
            m_heatSensor.PreliminaryDate = dateTimePicker3.Value;
            CheckSensorState();
        }

        private void btnAdvisoryDate_Click(object sender, EventArgs e)
        {
            if (lbAdvisoryDate.Text == "-")
            {
                dateTimePicker4.Value = DateTime.Now;
            }
            else
                dateTimePicker4.Value = Convert.ToDateTime(lbAdvisoryDate.Text);


            int x = lbAdvisoryDate.Location.X;
            int y = lbAdvisoryDate.Location.Y + lbAdvisoryDate.Height - dateTimePicker4.Height;

            dateTimePicker4.SendToBack();
            dateTimePicker4.Location = new Point(x, y);
            dateTimePicker4.DropDownAlign = LeftRightAlignment.Left;
            dateTimePicker4.Show();
            dateTimePicker4.Select();
            SendKeys.Send("%{DOWN}");
        }

        private void dateTimePicker4_ValueChanged(object sender, EventArgs e)
        {
            lbAdvisoryDate.Text = dateTimePicker4.Value.ToString("yyyy-MM-dd");
            m_heatSensor.AdvisoryDate = dateTimePicker4.Value;
            CheckSensorState();
        }

        private void btnAlertDate_Click(object sender, EventArgs e)
        {
            if (lbAlertDate.Text == "-")
            {
                dateTimePicker5.Value = DateTime.Now;
            }
            else
                dateTimePicker5.Value = Convert.ToDateTime(lbAlertDate.Text);


            int x = lbAlertDate.Location.X;
            int y = lbAlertDate.Location.Y + lbAlertDate.Height - dateTimePicker5.Height;

            dateTimePicker5.SendToBack();
            dateTimePicker5.Location = new Point(x, y);
            dateTimePicker5.DropDownAlign = LeftRightAlignment.Left;
            dateTimePicker5.Show();
            dateTimePicker5.Select();
            SendKeys.Send("%{DOWN}");
        }

        private void dateTimePicker5_ValueChanged(object sender, EventArgs e)
        {
            lbAlertDate.Text = dateTimePicker5.Value.ToString("yyyy-MM-dd");
            m_heatSensor.AlertDate = dateTimePicker5.Value;
            CheckSensorState();
        }


        private void uFormHeatSensorInfo_Load(object sender, EventArgs e)
        {
            if (m_heatSensor == null)
                return;

            ShowSensorInfo(m_heatSensor);
        }

        private void ShowSensorInfo(HeatSensor heatSensor)
        {
            if (heatSensor == null)
                return;

            DateTime dtDefault = new DateTime();

            string strSensorID = heatSensor.SensorID;
            string strAddr = heatSensor.Addr;
            DateTime dtOccurTime = heatSensor.OccurTime;
            float fTemperature = heatSensor.Temperature;
            float fHumidity = heatSensor.Humidity;
            float fDirection = heatSensor.Direction;
            float fSpeed = heatSensor.Speed;
            DateTime dtMeasPeriodStart = heatSensor.MeasPeriodStart;
            DateTime dtMeasPeriodEnd = heatSensor.MeasPeriodEnd;
            DateTime dtPreliminaryDate = heatSensor.PreliminaryDate;
            DateTime dtAdvisoryDate = heatSensor.AdvisoryDate;
            DateTime dtAlertDate = heatSensor.AlertDate;
            int nDeathToll = heatSensor.DeathToll;

            lbSensorID.Text = strSensorID;
            lbAddress.Text = strAddr;

            if (dtOccurTime == dtDefault)
                lbOccurTime.Text = "-";
            else
                lbOccurTime.Text = dtOccurTime.ToString("yyyy.MM.dd hh:mm");

            lbTemp.Text = fTemperature.ToString();
            lbHum.Text = fHumidity.ToString();
            lbDirect.Text = fDirection.ToString();
            lbSpeed.Text = fSpeed.ToString();

            if (dtMeasPeriodStart == dtDefault)
                lblDateStart.Text = "-";
            else
                lblDateStart.Text = dtMeasPeriodStart.ToString("yyyy-MM-dd");

            if (dtMeasPeriodEnd == dtDefault)
                lblDateEnd.Text = "-";
            else
                lblDateEnd.Text = dtMeasPeriodEnd.ToString("yyyy-MM-dd");

            if (dtPreliminaryDate == dtDefault)
                lbPreliminaryDate.Text = "-";
            else
                lbPreliminaryDate.Text = dtPreliminaryDate.ToString("yyyy-MM-dd");

            if (dtAdvisoryDate == dtDefault)
                lbAdvisoryDate.Text = "-";
            else
                lbAdvisoryDate.Text = dtAdvisoryDate.ToString("yyyy-MM-dd");

            if (dtAlertDate == dtDefault)
                lbAlertDate.Text = "-";
            else
                lbAlertDate.Text = dtAlertDate.ToString("yyyy-MM-dd");

            textBoxDeathToll.Text = nDeathToll.ToString();
        }

        private void CheckSensorState()
        {
            bool bChek = false;
            int nResult = 0;
            DateTime dtDefault = new DateTime();

            nResult = DateTime.Compare(DateTime.Now, m_heatSensor.MeasPeriodStart);
            if (nResult > 0 && dtDefault != m_heatSensor.MeasPeriodStart && (m_strModifiLevel == CommonString.RiskLevel_Normal))
            {
                m_heatSensor.State = CommonString.RiskLevel_Attention;
                ShowSensorState(CommonString.RiskLevel_Attention);
                bChek = true;
            }

            nResult = DateTime.Compare(DateTime.Now, m_heatSensor.MeasPeriodEnd);
            if (nResult < 0 && dtDefault != m_heatSensor.MeasPeriodEnd && (m_strModifiLevel == CommonString.RiskLevel_Normal))
            {
                m_heatSensor.State = CommonString.RiskLevel_Attention;
                ShowSensorState(CommonString.RiskLevel_Attention);
                bChek = true;
            }

            nResult = DateTime.Compare(DateTime.Now, m_heatSensor.PreliminaryDate);
            if (nResult > 0 && dtDefault != m_heatSensor.PreliminaryDate && (m_strModifiLevel == CommonString.RiskLevel_Normal))
            {
                m_heatSensor.State = CommonString.RiskLevel_Attention;
                ShowSensorState(CommonString.RiskLevel_Attention);
                bChek = true;
            }

            nResult = DateTime.Compare(DateTime.Now, m_heatSensor.AdvisoryDate);
            if (nResult > 0 && dtDefault != m_heatSensor.AdvisoryDate && (m_strModifiLevel == CommonString.RiskLevel_Normal || m_strModifiLevel == CommonString.RiskLevel_Attention))
            {
                m_heatSensor.State = CommonString.RiskLevel_Caution;
                ShowSensorState(CommonString.RiskLevel_Caution);
                bChek = true;
            }

            // .TODO : 일 최고기온 33도 이상인 상태가 2일 이상 지속될 경우 Caution


            nResult = DateTime.Compare(DateTime.Now, m_heatSensor.AlertDate);
            if (nResult > 0 && dtDefault != m_heatSensor.AlertDate && (m_strModifiLevel == CommonString.RiskLevel_Normal || m_strModifiLevel == CommonString.RiskLevel_Attention || m_strModifiLevel == CommonString.RiskLevel_Caution))
            {
                m_heatSensor.State = CommonString.RiskLevel_Alert;
                ShowSensorState(CommonString.RiskLevel_Alert);
                bChek = true;
            }

            // .TODO : 일 최고기온 35도 이상인 상태가 2일 이상 지속될 경우 Alert

            //if (m_heatSensor.DeathToll > 0 && (m_strModifiLevel == CommonString.RiskLevel_Normal || m_strModifiLevel == CommonString.RiskLevel_Attention || m_strModifiLevel == CommonString.RiskLevel_Caution || m_strModifiLevel == CommonString.RiskLevel_Alert))
            //{
            //    m_heatSensor.State = CommonString.RiskLevel_Serious;
            //    ShowSensorState(CommonString.RiskLevel_Serious);
            //    bChek = true;
            //}

            if (bChek == false)
            {
                m_heatSensor.State = m_strModifiLevel;
                ShowSensorState(m_strModifiLevel);
            }

        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            FormMessageBox msg = new FormMessageBox("위기경보 판단 데이터 새로고침", "[폭염] 위기경보 판단 데이터를 새로고침 하시겠습니까?\n기존 수정된 데이터가 모두 초기화 되어 최신화 됩니다.\n다시 한번 확인해주세요. ", MessageBoxButtons.YesNo);
            msg.StartPosition = FormStartPosition.CenterParent;
            if (msg.ShowDialog() == DialogResult.Yes)
            {
                HeatSensor heatSensor = FormMain.Instance.DataManager.DicHeatSensors[m_heatSensor.ID];
                m_heatSensor = heatSensor;
                InitCompareSensor(heatSensor);
                m_strModifiLevel = m_heatSensor.State;

                ShowSensorInfo(m_heatSensor);
                ShowSensorState(m_heatSensor.State);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            FormMessageBox msg = new FormMessageBox("위기경보 판단 데이터 수정", "[폭염] 위기경보 판단 데이터를 변경하고 나가시겠습니까?\n다시 한번 확인해주세요. ", MessageBoxButtons.YesNo);
            msg.StartPosition = FormStartPosition.CenterParent;
            if (msg.ShowDialog() == DialogResult.Yes)
            {
                FormMain.Instance.DataManager.UpdateHeatSensorInfo(m_heatSensor);
                CheckModifityReport();
                CheckAlertAlarm();          // 알람 신호
                uFormCrisisAlert.Instance.M_timerSensorReload_Tick(null, null);

                this.Visible = false;
            }
        }

        private bool CheckAlertAlarm()
        {
            bool bRet = true;

            if (m_heatSensor.State != m_compareSensor.State)
            {
                int nID = m_heatSensor.ID;
                string strAddress = m_heatSensor.Addr;
                string strRiskLevel = TransRiskLevel(m_heatSensor.State);

                if (!FormMain.Instance.DataManager.InsertAlertAarm(FacilityType.HEAT_SENSOR, nID, strAddress, strRiskLevel))
                    bRet = false;
            }

            return bRet;
        }

        private void textBoxDeathToll_TextChanged(object sender, EventArgs e)
        {
            if (textBoxDeathToll.Text == "")
                textBoxDeathToll.Text = string.Format("0");

            m_heatSensor.DeathToll = Int32.Parse(textBoxDeathToll.Text);
            //CheckSensorState();
        }

        private void CheckModifityReport()
        {
            if (m_heatSensor.MeasPeriodStart != m_compareSensor.MeasPeriodStart || m_heatSensor.MeasPeriodEnd != m_compareSensor.MeasPeriodEnd)
            {
                DateTime dtDefault = new DateTime();

                string strDataName = CommonString.HeatSensor_MeasPeriod_Kor;
                string strOldData = "";
                string strNewData = "";
                string strMeasPeriodStart = "";
                string strMeasPeriodEnd = "";

                if (m_compareSensor.MeasPeriodStart == dtDefault)
                    strMeasPeriodStart = "";
                else
                    strMeasPeriodStart = m_compareSensor.MeasPeriodStart.ToString("yyyy-MM-dd");

                if (m_compareSensor.MeasPeriodEnd == dtDefault)
                    strMeasPeriodEnd = "";
                else
                    strMeasPeriodEnd = m_compareSensor.MeasPeriodEnd.ToString("yyyy-MM-dd");

                strOldData = strMeasPeriodStart + " ~ " + strMeasPeriodEnd;

                if (m_heatSensor.MeasPeriodStart == dtDefault)
                    strMeasPeriodStart = "";
                else
                    strMeasPeriodStart = m_heatSensor.MeasPeriodStart.ToString("yyyy-MM-dd");

                if (m_heatSensor.MeasPeriodEnd == dtDefault)
                    strMeasPeriodEnd = "";
                else
                    strMeasPeriodEnd = m_heatSensor.MeasPeriodEnd.ToString("yyyy-MM-dd");

                strNewData = strMeasPeriodStart + " ~ " + strMeasPeriodEnd;

                FormMain.Instance.DataManager.InsertDataReport(FacilityType.HEAT_SENSOR, m_heatSensor.ID, strDataName, strOldData, strNewData);
            }

            if (m_heatSensor.PreliminaryDate != m_compareSensor.PreliminaryDate)
            {
                DateTime dtDefault = new DateTime();

                string strDataName = CommonString.HeatSensor_PreliminaryDate_Kor;
                string strOldData = "";
                string strNewData = "";

                if (m_compareSensor.PreliminaryDate == dtDefault)
                    strOldData = "-";
                else
                    strOldData = m_compareSensor.PreliminaryDate.ToString("yyyy-MM-dd");

                if (m_heatSensor.PreliminaryDate == dtDefault)
                    strNewData = "-";
                else
                    strNewData = m_heatSensor.PreliminaryDate.ToString("yyyy-MM-dd");

                FormMain.Instance.DataManager.InsertDataReport(FacilityType.HEAT_SENSOR, m_heatSensor.ID, strDataName, strOldData, strNewData);
            }

            if (m_heatSensor.AdvisoryDate != m_compareSensor.AdvisoryDate)
            {
                DateTime dtDefault = new DateTime();

                string strDataName = CommonString.HeatSensor_AdvisoryDate_Kor;
                string strOldData = "";
                string strNewData = "";

                if (m_compareSensor.AdvisoryDate == dtDefault)
                    strOldData = "-";
                else
                    strOldData = m_compareSensor.AdvisoryDate.ToString("yyyy-MM-dd");

                if (m_heatSensor.AdvisoryDate == dtDefault)
                    strNewData = "-";
                else
                    strNewData = m_heatSensor.AdvisoryDate.ToString("yyyy-MM-dd");

                FormMain.Instance.DataManager.InsertDataReport(FacilityType.HEAT_SENSOR, m_heatSensor.ID, strDataName, strOldData, strNewData);
            }

            if (m_heatSensor.AlertDate != m_compareSensor.AlertDate)
            {
                DateTime dtDefault = new DateTime();

                string strDataName = CommonString.HeatSensor_AlertDate_Kor;
                string strOldData = "";
                string strNewData = "";

                if (m_compareSensor.AlertDate == dtDefault)
                    strOldData = "-";
                else
                    strOldData = m_compareSensor.AlertDate.ToString("yyyy-MM-dd");

                if (m_heatSensor.AlertDate == dtDefault)
                    strNewData = "-";
                else
                    strNewData = m_heatSensor.AlertDate.ToString("yyyy-MM-dd");

                FormMain.Instance.DataManager.InsertDataReport(FacilityType.HEAT_SENSOR, m_heatSensor.ID, strDataName, strOldData, strNewData);
            }

            if (m_heatSensor.DeathToll != m_compareSensor.DeathToll)
            {
                string strDataName = CommonString.DeathTollr_Kor;
                string strOldData = m_compareSensor.DeathToll.ToString();
                string strNewData = m_heatSensor.DeathToll.ToString();

                FormMain.Instance.DataManager.InsertDataReport(FacilityType.HEAT_SENSOR, m_heatSensor.ID, strDataName, strOldData, strNewData);
            }

            if (m_heatSensor.State != m_compareSensor.State)
            {
                string strOldData = TransRiskLevel(m_compareSensor.State);
                string strNewData = TransRiskLevel(m_heatSensor.State);
                string strDataName = CommonString.GetRiskDataName(strOldData, strNewData);

                FormMain.Instance.DataManager.InsertAlertReport(FacilityType.HEAT_SENSOR, m_heatSensor.ID, strDataName, strOldData, strNewData);
            }
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

        private void btnMeasPeriodRefresh_Click(object sender, EventArgs e)
        {
            DateTime dtDefault = new DateTime();

            m_heatSensor.MeasPeriodStart = m_compareSensor.MeasPeriodStart;
            m_heatSensor.MeasPeriodEnd = m_compareSensor.MeasPeriodEnd;

            DateTime dtMeasPeriodStart = m_compareSensor.MeasPeriodStart;
            DateTime dtMeasPeriodEnd = m_compareSensor.MeasPeriodEnd;

            if (dtMeasPeriodStart == dtDefault)
                lblDateStart.Text = "-";
            else
                lblDateStart.Text = dtMeasPeriodStart.ToString("yyyy-MM-dd");

            if (dtMeasPeriodEnd == dtDefault)
                lblDateEnd.Text = "-";
            else
                lblDateEnd.Text = dtMeasPeriodEnd.ToString("yyyy-MM-dd");

            CheckSensorState();
        }

        private void btnPreliminaryRefresh_Click(object sender, EventArgs e)
        {
            DateTime dtDefault = new DateTime();

            m_heatSensor.PreliminaryDate = m_compareSensor.PreliminaryDate;
            DateTime dtPreliminaryDate = m_compareSensor.PreliminaryDate;

            if (dtPreliminaryDate == dtDefault)
                lbPreliminaryDate.Text = "-";
            else
                lbPreliminaryDate.Text = dtPreliminaryDate.ToString("yyyy-MM-dd");

            CheckSensorState();
        }

        private void btnAdvisoryRefresh_Click(object sender, EventArgs e)
        {
            DateTime dtDefault = new DateTime();

            m_heatSensor.AdvisoryDate = m_compareSensor.AdvisoryDate;
            DateTime dtAdvisoryDate = m_compareSensor.AdvisoryDate;

            if (dtAdvisoryDate == dtDefault)
                lbAdvisoryDate.Text = "-";
            else
                lbAdvisoryDate.Text = dtAdvisoryDate.ToString("yyyy-MM-dd");

            CheckSensorState();
        }

        private void btnAlertRefresh_Click(object sender, EventArgs e)
        {
            DateTime dtDefault = new DateTime();

            m_heatSensor.AlertDate = m_compareSensor.AlertDate;
            DateTime dtAlertDate = m_compareSensor.AlertDate;

            if (dtAlertDate == dtDefault)
                lbAlertDate.Text = "-";
            else
                lbAlertDate.Text = dtAlertDate.ToString("yyyy-MM-dd");

            CheckSensorState();
        }

        private void btnDeathTollRefresh_Click(object sender, EventArgs e)
        {
            m_heatSensor.DeathToll = m_compareSensor.DeathToll;
            int nDeathToll = m_compareSensor.DeathToll;
            textBoxDeathToll.Text = nDeathToll.ToString();
        }

        private void textBoxDeathToll_KeyPress(object sender, KeyPressEventArgs e)
        {
            //숫자만 입력되도록 필터링
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))    //숫자와 백스페이스를 제외한 나머지를 바로 처리
            {
                e.Handled = true;
            }
        }

        private void panel8_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
