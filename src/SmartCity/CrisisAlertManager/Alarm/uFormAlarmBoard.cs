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
using libSensorProcess;

namespace CrisisAlertManager.Alarm
{
    public partial class uFormAlarmBoard : UserControl
    {
        FacilityType m_facilityType = FacilityType.FIRE_SENSOR;

        public uFormAlarmBoard()
        {
            InitializeComponent();

            rbtnFire_Click(null, null);
        }

        private void rbtnFire_Click(object sender, EventArgs e)
        {
            if (rbtnFire.IsChecked)
                return;

            m_facilityType = FacilityType.FIRE_SENSOR;
            LoadFacilityAlarms();

            rbtnFire.IsChecked = true;
            rbtnFlood.IsChecked = false;
            rbtnHeat.IsChecked = false;
            rbtnCollapse.IsChecked = false;

            rbtnFire.Refresh();
            rbtnFlood.Refresh();
            rbtnHeat.Refresh();
            rbtnCollapse.Refresh();
        }

        private void rbtnFlood_Click(object sender, EventArgs e)
        {
            if (rbtnFlood.IsChecked)
                return;

            m_facilityType = FacilityType.FLOOD_SENSOR;
            LoadFacilityAlarms();

            rbtnFire.IsChecked = false;
            rbtnFlood.IsChecked = true;
            rbtnHeat.IsChecked = false;
            rbtnCollapse.IsChecked = false;

            rbtnFire.Refresh();
            rbtnFlood.Refresh();
            rbtnHeat.Refresh();
            rbtnCollapse.Refresh();
        }

        private void rbtnHeat_Click(object sender, EventArgs e)
        {
            if (rbtnHeat.IsChecked)
                return;

            m_facilityType = FacilityType.HEAT_SENSOR;
            LoadFacilityAlarms();

            rbtnFire.IsChecked = false;
            rbtnFlood.IsChecked = false;
            rbtnHeat.IsChecked = true;
            rbtnCollapse.IsChecked = false;

            rbtnFire.Refresh();
            rbtnFlood.Refresh();
            rbtnHeat.Refresh();
            rbtnCollapse.Refresh();
        }

        private void rbtnCollapse_Click(object sender, EventArgs e)
        {
            if (rbtnCollapse.IsChecked)
                return;

            m_facilityType = FacilityType.COLLAPSE_SENSOR;
            LoadFacilityAlarms();

            rbtnFire.IsChecked = false;
            rbtnFlood.IsChecked = false;
            rbtnHeat.IsChecked = false;
            rbtnCollapse.IsChecked = true;

            rbtnFire.Refresh();
            rbtnFlood.Refresh();
            rbtnHeat.Refresh();
            rbtnCollapse.Refresh();
        }

        private void LoadFacilityAlarms()
        {
            gridAlarmList.Rows.Clear();

            Dictionary<int, AlarmData> dicFacilityAlarms = new Dictionary<int, AlarmData>();
            dicFacilityAlarms = FormMain.Instance.DataManager.LoadFacilityAlarms(m_facilityType);

            foreach (KeyValuePair<int, AlarmData> item in dicFacilityAlarms)
            {
                AlarmData data = item.Value;
                string strCheck = "";

                if (data.Check) strCheck = "YES";
                else strCheck = "NO";


                int nRowIndex = gridAlarmList.Rows.Add();
                gridAlarmList.Rows[nRowIndex].Cells[colNo.Index].Value = nRowIndex + 1;
                gridAlarmList.Rows[nRowIndex].Cells[colLevel.Index].Value = data.RiskLevel;
                gridAlarmList.Rows[nRowIndex].Cells[colLevel.Index].Style.ForeColor = GetAlertColor(data.RiskLevel);
                gridAlarmList.Rows[nRowIndex].Cells[colTime.Index].Value = data.CreateTime.ToString("yyyy년 MM월 dd일 \nhh시 mm분 ss초");
                gridAlarmList.Rows[nRowIndex].Cells[colAddress.Index].Value = data.Address;
                gridAlarmList.Rows[nRowIndex].Cells[colCheck.Index].Value = strCheck;
                gridAlarmList.Rows[nRowIndex].Tag = data;

            }
        }

        private Color GetAlertColor(string strAlert)
        {
            Color retColor = Color.Black;

            if (strAlert == CommonString.RiskLevel_Attention_Kor)
            {
                retColor = Color.FromArgb(0, 174, 228);
            }
            else if (strAlert == CommonString.RiskLevel_Caution_Kor)
            {
                retColor = Color.FromArgb(187, 167, 0);
            }
            else if (strAlert == CommonString.RiskLevel_Alert_Kor)
            {
                retColor = Color.FromArgb(249, 100, 35);
            }
            else if (strAlert == CommonString.RiskLevel_Serious_Kor)
            {
                retColor = Color.FromArgb(255, 23, 42);
            }

            return retColor;
        }

        public void ReloadAlarms()
        {
            FormMain.Instance.DataManager.LoadAlarm();
            LoadFacilityAlarms();
        }

        public void ShowAlarmTab(FacilityType type)
        {
            FormMain.Instance.DataManager.LoadAlarm();

            if (type == FacilityType.FIRE_SENSOR)
                rbtnFire_Click(null, null);
            else if (type == FacilityType.FLOOD_SENSOR)
                rbtnFlood_Click(null, null);
            else if (type == FacilityType.HEAT_SENSOR)
                rbtnHeat_Click(null, null);
            else if (type == FacilityType.COLLAPSE_SENSOR)
                rbtnCollapse_Click(null, null);
        }

        private void gridAlarmList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (gridAlarmList.Rows[e.RowIndex].Tag == null)
                return;

            if (e.ColumnIndex == colCheckBtn.Index)
            {
                AlarmData data = (AlarmData)gridAlarmList.Rows[e.RowIndex].Tag;

                // 알람소리 끄기
                FireDetectProcess.SoundPlayer.Stop();

                // 클릭 시 알람 신호 체크
                if (FormMain.Instance.DataManager.ConfirmAlertAarm(data.SersorID, (int)data.FacilityType))
                    gridAlarmList.Rows.Remove(gridAlarmList.Rows[e.RowIndex]);

                // 알람 팝업창이 떠 있다면 닫기
                FormMain.Instance.CheckCloseAlarm(data.ID);

                // 해당 알람 화면 띄우기
                FormMain.Instance.ShowAlertSensor(data.FacilityType, data.SersorID);
            }
        }
    }
}
