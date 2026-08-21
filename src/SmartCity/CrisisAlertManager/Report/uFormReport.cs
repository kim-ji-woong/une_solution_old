using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.GUI;
using CrisisAlertManager.Data;
using CrisisAlertManager.Popup_Dialog.Message;

namespace CrisisAlertManager.Report
{
    public partial class uFormReport : UserControl
    {
        FacilityType m_facilityType = FacilityType.FIRE_SENSOR;

        Dictionary<int, DataReport> m_dicDataReport = null;
        Dictionary<int, AlertReport> m_dicAlertReport = null;
        Dictionary<int, SMSReport> m_dicSMSReport = null;

        public uFormReport()
        {
            InitializeComponent();

            this.DoubleBuffered = true;

            // 그리드 셀 줄바꿈 설정
            gridDataReport.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            gridAlertReport.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            gridSMSReport.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            // 메시지 이력 행 높이 고정
            gridSMSReport.RowTemplate.Height = 55;

            rbtnFire.IsChecked = true;
            initReportTab();
            LoadReporDatas();
        }

        private void initReportTab()
        {
            rbtnData_Click(null, null);
        }

        public void LoadReporDatas()
        {
            FormMain.Instance.DataManager.LoadReports();

            LoadDataReport();
            LoadAlertReport();
            LoadSMSReport();
        }

        private void rbtnFire_Click(object sender, EventArgs e)
        {
            if (rbtnFire.IsChecked)
                return;

            m_facilityType = FacilityType.FIRE_SENSOR;
            LoadReporDatas();

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
            LoadReporDatas();

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
            LoadReporDatas();

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
            LoadReporDatas();

            rbtnFire.IsChecked = false;
            rbtnFlood.IsChecked = false;
            rbtnHeat.IsChecked = false;
            rbtnCollapse.IsChecked = true;

            rbtnFire.Refresh();
            rbtnFlood.Refresh();
            rbtnHeat.Refresh();
            rbtnCollapse.Refresh();
        }

        private void rbtnData_Click(object sender, EventArgs e)
        {
            if (rbtnData.IsChecked)
                return;

            rbtnData.IsChecked = true;
            rbtnAlert.IsChecked = false;
            rbtnSMS.IsChecked = false;

            rbtnData.Refresh();
            rbtnAlert.Refresh();
            rbtnSMS.Refresh();

            gridDataReport.Visible = true;
            gridAlertReport.Visible = false;
            gridSMSReport.Visible = false;

            plReport.Visible = true;
            plSMSReport.Visible = false;
        }

        private void rbtnAlert_Click(object sender, EventArgs e)
        {
            if (rbtnAlert.IsChecked)
                return;

            rbtnData.IsChecked = false;
            rbtnAlert.IsChecked = true;
            rbtnSMS.IsChecked = false;

            rbtnData.Refresh();
            rbtnAlert.Refresh();
            rbtnSMS.Refresh();

            gridDataReport.Visible = false;
            gridAlertReport.Visible = true;
            gridSMSReport.Visible = false;

            plReport.Visible = true;
            plSMSReport.Visible = false;
        }

        private void rbtnSMS_Click(object sender, EventArgs e)
        {
            if (rbtnSMS.IsChecked)
                return;

            rbtnData.IsChecked = false;
            rbtnAlert.IsChecked = false;
            rbtnSMS.IsChecked = true;

            rbtnData.Refresh();
            rbtnAlert.Refresh();
            rbtnSMS.Refresh();

            gridDataReport.Visible = false;
            gridAlertReport.Visible = false;
            gridSMSReport.Visible = true;

            plReport.Visible = false;
            plSMSReport.Visible = true;
        }

        private void LoadDataReport()
        {
            gridDataReport.Rows.Clear();

            m_dicDataReport = new Dictionary<int, DataReport>();
            m_dicDataReport = FormMain.Instance.DataManager.LoadFacilityDataReports(m_facilityType);

            foreach (KeyValuePair<int, DataReport> item in m_dicDataReport)
            {
                DataReport data = item.Value;

                int nRowIndex = gridDataReport.Rows.Add();
                gridDataReport.Rows[nRowIndex].Cells[colNo.Index].Value = nRowIndex + 1;
                gridDataReport.Rows[nRowIndex].Cells[colType.Index].Value = TransFacilityType(data.FacilityType);
                gridDataReport.Rows[nRowIndex].Cells[colTime.Index].Value = data.OccurTime.ToString("yyyy년 MM월 dd일 \nhh시 mm분 ss초");
                gridDataReport.Rows[nRowIndex].Cells[colDataName.Index].Value = data.DataName;
                gridDataReport.Rows[nRowIndex].Cells[colOriginData.Index].Value = data.OriginData;
                gridDataReport.Rows[nRowIndex].Cells[colNewData.Index].Value = data.NewData;
            }
        }

        private void LoadAlertReport()
        {
            gridAlertReport.Rows.Clear();

            m_dicAlertReport = new Dictionary<int, AlertReport>();
            m_dicAlertReport = FormMain.Instance.DataManager.LoadFacilityAlertReports(m_facilityType);

            foreach (KeyValuePair<int, AlertReport> item in m_dicAlertReport)
            {
                

                

                AlertReport data = item.Value;

                string strAddr = "";
                
                if (data.FacilityType == FacilityType.FIRE_SENSOR)
                {
                    if (FormMain.Instance.DataManager.DicFireSensors.ContainsKey(data.SensorID))
                    {
                        FireSensor sensor = FormMain.Instance.DataManager.DicFireSensors[data.SensorID];
                        strAddr = sensor.Addr;
                    }
                }
                else if (data.FacilityType == FacilityType.FLOOD_SENSOR)
                {
                    if (FormMain.Instance.DataManager.DicFloodSensors.ContainsKey(data.SensorID))
                    {
                        FloodSensor sensor = FormMain.Instance.DataManager.DicFloodSensors[data.SensorID];
                        strAddr = sensor.Addr;
                    }
                }
                else if (data.FacilityType == FacilityType.HEAT_SENSOR)
                {
                    if (FormMain.Instance.DataManager.DicHeatSensors.ContainsKey(data.SensorID))
                    {
                        HeatSensor sensor = FormMain.Instance.DataManager.DicHeatSensors[data.SensorID];
                        strAddr = sensor.Addr;
                    }
                }
                else if (data.FacilityType == FacilityType.COLLAPSE_SENSOR)
                {
                    if (FormMain.Instance.DataManager.DicCollapseSensors.ContainsKey(data.SensorID))
                    {
                        CollapseSensor sensor = FormMain.Instance.DataManager.DicCollapseSensors[data.SensorID];
                        strAddr = sensor.Addr;
                    }
                }

                
                    

                int nRowIndex = gridAlertReport.Rows.Add();
                gridAlertReport.Rows[nRowIndex].Cells[colNoAlert.Index].Value = nRowIndex + 1;
                gridAlertReport.Rows[nRowIndex].Cells[colTypeAlert.Index].Value = TransFacilityType(data.FacilityType);
                gridAlertReport.Rows[nRowIndex].Cells[colTimeAlert.Index].Value = data.OccurTime.ToString("yyyy년 MM월 dd일 \nhh시 mm분 ss초");
                gridAlertReport.Rows[nRowIndex].Cells[colDataNameAlert.Index].Value = data.DataName;
                gridAlertReport.Rows[nRowIndex].Cells[colOriginDataAlert.Index].Value = data.OriginData;
                gridAlertReport.Rows[nRowIndex].Cells[colOriginDataAlert.Index].Style.ForeColor = GetAlertColor(data.OriginData);
                gridAlertReport.Rows[nRowIndex].Cells[colNewDataAlert.Index].Value = data.NewData;
                gridAlertReport.Rows[nRowIndex].Cells[colNewDataAlert.Index].Style.ForeColor = GetAlertColor(data.NewData);
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

        private void LoadSMSReport()
        {
            gridSMSReport.Rows.Clear();

            m_dicSMSReport = new Dictionary<int, SMSReport>();
            m_dicSMSReport = FormMain.Instance.DataManager.LoadFacilitySMSReports(m_facilityType);

            foreach (KeyValuePair<int, SMSReport> item in m_dicSMSReport)
            {
                SMSReport data = item.Value;

                int nRowIndex = gridSMSReport.Rows.Add();
                gridSMSReport.Rows[nRowIndex].Cells[colNoSMS.Index].Value = nRowIndex + 1;
                gridSMSReport.Rows[nRowIndex].Cells[colTypeSMS.Index].Value = TransFacilityType(data.FacilityType);
                gridSMSReport.Rows[nRowIndex].Cells[colTimeSMS.Index].Value = data.OccurTime.ToString("yyyy년 MM월 dd일 \nhh시 mm분 ss초");
                gridSMSReport.Rows[nRowIndex].Cells[colMessage.Index].Value = data.Message;
                gridSMSReport.Rows[nRowIndex].Cells[colManager.Index].Value = data.Managers;
            }
        }

        private string TransFacilityType(FacilityType facilityType)
        {
            string strFacilityType = "";

            if (facilityType == FacilityType.FIRE_SENSOR)
                strFacilityType = CommonString.FacilityType_Fire_Kor;
            else if (facilityType == FacilityType.FLOOD_SENSOR)
                strFacilityType = CommonString.FacilityType_Flood_Kor;
            else if (facilityType == FacilityType.HEAT_SENSOR)
                strFacilityType = CommonString.FacilityType_Heat_Kor;
            else if (facilityType == FacilityType.COLLAPSE_SENSOR)
                strFacilityType = CommonString.FacilityType_Collapse_Kor;

            return strFacilityType;
        }

        private void rbtnExport_Click(object sender, EventArgs e)
        {
            FormMessageBox msg = new FormMessageBox("데이터 엑셀파일 저장", "선택된 데이터를 저장하시겠습니까?\n다시 한번 확인해주세요. ", MessageBoxButtons.YesNo);
            msg.StartPosition = FormStartPosition.CenterParent;
            if (msg.ShowDialog() == DialogResult.Yes)
            {
                ExcelManager excelManager = new ExcelManager();
                string strPath = "";

                if (rbtnData.IsChecked)
                    strPath = excelManager.SaveDataReportExcel(m_dicDataReport);
                else if (rbtnAlert.IsChecked)
                    strPath = excelManager.SaveAlertReportExcel(m_dicAlertReport);
                else if (rbtnSMS.IsChecked)
                    strPath = excelManager.SaveSMSReportExcel(m_dicSMSReport);

                msg = new FormMessageBox("데이터 엑셀파일 저장", "저장이 완료되었습니다.\n저장경로: " + strPath, MessageBoxButtons.OK);
                msg.StartPosition = FormStartPosition.CenterParent;
                msg.ShowDialog();
            }
            


        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }
    }
}
