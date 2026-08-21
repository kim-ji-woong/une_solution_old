using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility2;

namespace UnityTester
{
    public partial class FormSensorList : Form
    {
        private List<SensorTag> m_sensors = null;
        private FormMain m_frmMain = null;
        private WebDBManager m_dbMgr = null;

        public FormSensorList(FormMain frmMain, WebDBManager dbMgr)
        {
            InitializeComponent();
            m_frmMain = frmMain;
            m_dbMgr = dbMgr;
        }

        public void SetSensors(Zone zone, List<SensorTag> sensors)
        {
            m_sensors = sensors;
            this.Text = zone.ZoneName;

            SetDataGrid();
        }

        private void SetDataGrid()
        {
            gridSensors.Rows.Clear();

            if (m_sensors == null)
                return;

            foreach (SensorTag sensor in m_sensors)
            {
                int nRowIndex = gridSensors.Rows.Add();

                if (nRowIndex < 0)
                    continue;

                DataGridViewRow row = gridSensors.Rows[nRowIndex];
                row.Tag = sensor;

                row.Cells[0].Value = string.Format("{0}-{1}", sensor.TabHighIndex, sensor.TabLowIndex);
                row.Cells[1].Value = string.Format("{0}-{1}", sensor.RelayFirstIndex, sensor.RelaySecondIndex);
                row.Cells[2].Value = sensor.SensorName;
            }
        }

        private void FormSensorList_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_frmMain.OnCloseSensorList();
        }

        private void gridSensors_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0)
                return;

            DataGridViewRow row = gridSensors.Rows[e.RowIndex];
            SensorTag sensor = (SensorTag)row.Tag;

            textBoxAlarmZones.Text = sensor.VolumeName;
            textBoxAlarmZones.Tag = sensor;

            string strVolumeName = GetVolumeName();
            m_frmMain.ShowAlarmZone(strVolumeName);
        }

        private string GetVolumeName()
        {
            string strVolumeName = textBoxAlarmZones.Text.Trim();
            string[] tokens = strVolumeName.Split(',');

            strVolumeName = "";

            foreach (string strToken in tokens)
            {
                if (strVolumeName.Length == 0)
                    strVolumeName = strToken;
                else
                    strVolumeName += "\t" + strToken;
            }

            return strVolumeName;
        }

        private void btnSaveDB_Click(object sender, EventArgs e)
        {
            if (textBoxAlarmZones.Tag == null)
                return;

            SensorTag sensor = (SensorTag)textBoxAlarmZones.Tag;
            string strVolumeName = GetVolumeName();

            string strSQL = string.Format("Update EquipZoneVolume set VolumeName = '{0}' where EquipZoneID = {1}", strVolumeName, sensor.EquipZoneID);

            if (m_dbMgr.GetResultData(strSQL) != null)
            {
                sensor.VolumeName = strVolumeName;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (textBoxAlarmZones.Tag == null)
                return;

            SensorTag sensor = (SensorTag)textBoxAlarmZones.Tag;
            string strVolumeName = GetVolumeName();
            m_frmMain.ShowAlarmZone(strVolumeName);
        }
    }
}
