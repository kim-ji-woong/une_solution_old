using CrisisAlertManager.CrisisAlert;
using CrisisAlertManager.Data;
using CrisisAlertManager.Popup_Dialog.Message;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrisisAlertManager.Popup_Dialog
{
    public partial class FormSensorSearch : Form
    {
        private FacilityType m_facilityType = FacilityType.NONE;
        private int m_nID = -1;
        //private uFormCrisisAlert

        private Dictionary<int, SensorInfo> m_dicSensorAddress = null;

        public FormSensorSearch(FacilityType facilityType, int nID)
        {
            InitializeComponent();

            m_facilityType = facilityType;
            m_nID = nID;

            cmbLevel.SelectedIndex = 0;

            gridSensorAddress.RowTemplate.Height = 35;

            InitLoad();
        }

        private void InitLoad()
        {
            m_dicSensorAddress = new Dictionary<int, SensorInfo>();

            if (m_facilityType == FacilityType.NONE)
                return;
            else if (m_facilityType == FacilityType.FIRE_SENSOR)
            {
                foreach (KeyValuePair<int, FireSensor> item in FormMain.Instance.DataManager.DicFireSensors)
                {
                    FireSensor sensor = item.Value;
                    SensorInfo info = new SensorInfo();
                    info.ID = sensor.ID;
                    info.Address = sensor.Addr;
                    info.State = sensor.State;

                    m_dicSensorAddress[sensor.ID] = info;
                }
            }
            else if (m_facilityType == FacilityType.FLOOD_SENSOR)
            {
                foreach (KeyValuePair<int, FloodSensor> item in FormMain.Instance.DataManager.DicFloodSensors)
                {
                    FloodSensor sensor = item.Value;
                    SensorInfo info = new SensorInfo();
                    info.ID = sensor.ID;
                    info.Address = sensor.Addr;
                    info.State = sensor.State;

                    m_dicSensorAddress[sensor.ID] = info;
                }
            }
            else if (m_facilityType == FacilityType.HEAT_SENSOR)
            {
                foreach (KeyValuePair<int, HeatSensor> item in FormMain.Instance.DataManager.DicHeatSensors)
                {
                    HeatSensor sensor = item.Value;
                    SensorInfo info = new SensorInfo();
                    info.ID = sensor.ID;
                    info.Address = sensor.Addr;
                    info.State = sensor.State;

                    m_dicSensorAddress[sensor.ID] = info;
                }
            }
            else if (m_facilityType == FacilityType.COLLAPSE_SENSOR)
            {
                foreach (KeyValuePair<int, CollapseSensor> item in FormMain.Instance.DataManager.DicCollapseSensors)
                {
                    CollapseSensor sensor = item.Value;
                    SensorInfo info = new SensorInfo();
                    info.ID = sensor.ID;
                    info.Address = sensor.Addr;
                    info.State = sensor.State;

                    m_dicSensorAddress[sensor.ID] = info;
                }
            }

            ShowGrid(m_dicSensorAddress);
        }

        private void ShowGrid(Dictionary<int, SensorInfo> dicSensorAddress)
        {
            gridSensorAddress.Rows.Clear();

            if (dicSensorAddress == null)
                return;

            foreach (KeyValuePair<int, SensorInfo> pair in dicSensorAddress)
            {
                SensorInfo info = pair.Value;

                int nRowIndex = gridSensorAddress.Rows.Add();
                gridSensorAddress.Rows[nRowIndex].Cells[colAddress.Index].Value = info.Address;
                gridSensorAddress.Rows[nRowIndex].Tag = info;

                if (m_nID == pair.Key)
                    gridSensorAddress.Rows[nRowIndex].Cells[colCheck.Index].Value = true;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnModifityCancle_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void gridSensorAddress_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (gridSensorAddress.Rows[e.RowIndex].Tag == null)
                return;

            // 체크 해제
            foreach (DataGridViewRow row in gridSensorAddress.Rows)
            {
                gridSensorAddress.Rows[row.Index].Cells[colCheck.Index].Value = false;
            }

            // 체크
            gridSensorAddress.Rows[e.RowIndex].Cells[colCheck.Index].Value = true;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            Dictionary<int, SensorInfo> dicShowAddress = new Dictionary<int, SensorInfo>();
            string strSearchAddress = txtAddress.Text;
            string strSearchState = CommonString.GetRiskLevelKorToEng(cmbLevel.Text.Trim());

            foreach (KeyValuePair<int, SensorInfo> pair in m_dicSensorAddress)
            {
                SensorInfo info = pair.Value;

                if (info.Address.Contains(strSearchAddress))
                {
                    if (cmbLevel.SelectedIndex == 0)
                    {
                        dicShowAddress[pair.Key] = pair.Value;
                    }
                    else if (info.State == strSearchState)
                    {
                        dicShowAddress[pair.Key] = pair.Value;
                    }
                }
            }

            ShowGrid(dicShowAddress);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int nSelectID = -1;

            foreach (DataGridViewRow row in gridSensorAddress.Rows)
            {
                bool bChecked = Convert.ToBoolean(gridSensorAddress.Rows[row.Index].Cells[colCheck.Index].Value);

                if (bChecked)
                {
                    SensorInfo info = (SensorInfo)gridSensorAddress.Rows[row.Index].Tag;
                    nSelectID = info.ID;
                }
            }

            if (nSelectID == -1)
            {
                FormMessageBox msg = new FormMessageBox("확인", "센서를 선택해주세요.", MessageBoxButtons.OK);
                msg.StartPosition = FormStartPosition.CenterParent;
                msg.ShowDialog();
                return;
            }

            if (m_facilityType == FacilityType.FIRE_SENSOR)
            {
                uFormCrisisAlert.Instance.SelectFireSensor = FormMain.Instance.DataManager.DicFireSensors[nSelectID];
                uFormCrisisAlert.Instance.ReloadFireSensorState();
            }
            else if (m_facilityType == FacilityType.FLOOD_SENSOR)
            {
                uFormCrisisAlert.Instance.SelectFloodSensor = FormMain.Instance.DataManager.DicFloodSensors[nSelectID];
                uFormCrisisAlert.Instance.ReloadFloodSensorState();
            }
            else if (m_facilityType == FacilityType.HEAT_SENSOR)
            {
                uFormCrisisAlert.Instance.SelectHeatSensor = FormMain.Instance.DataManager.DicHeatSensors[nSelectID];
                uFormCrisisAlert.Instance.ReloadHeatSensorState();
            }
            else if (m_facilityType == FacilityType.COLLAPSE_SENSOR)
            {
                uFormCrisisAlert.Instance.SelectCollapseSensor = FormMain.Instance.DataManager.DicCollapseSensors[nSelectID];
                uFormCrisisAlert.Instance.ReloadCollapseSensorState();
            }

            this.Close();
        }
    }

    class SensorInfo
    {
        private int m_nID = -1;
        private string m_strAddress = "";
        private string m_strState = CommonString.RiskLevel_Normal;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string Address
        {
            get { return m_strAddress; }
            set { m_strAddress = value; }
        }

        public string State
        {
            get { return m_strState; }
            set { m_strState = value; }
        }
    }
}
