using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.PSM;
using System.Collections;

namespace SDMS.PopupDialog
{
    public partial class FormPSMSensorLifeTime : Form
    {
        private PSMSensor m_sensor = null;
        private int m_nSensorNo = -1;

        public FormPSMSensorLifeTime(PSMSensor sensor, int nSensorNo)
        {
            this.DoubleBuffered = true;
            InitializeComponent();
            SetData(sensor, nSensorNo);
        }

        public void SetData(PSMSensor sensor, int nSensorNo)
        {
            lblSensorNo.Text = lblLocation.Text = lblMaterialName.Text = lblDeadLine.Text = "";

            m_sensor = sensor;
            m_nSensorNo = nSensorNo;

            if (m_sensor == null)
                return;

            PSMTank linkedTank = null;

            foreach (PSMTank tank in sensor.LinkedTankList)
            {
                linkedTank = tank;
                break;
            }

            if (sensor.InstallDate == null)
                dtpIntallDate.Value = dtpIntallDate.MinDate;
            else
                dtpIntallDate.Value = sensor.InstallDate.Data;

            lblSensorNo.Text = nSensorNo.ToString();
            lblMaterialName.Text = linkedTank == null || linkedTank.Material == null ? null : linkedTank.Material.Name;
            lblLocation.Text = linkedTank == null ? null : linkedTank.LocationName;

            SetDeadLine(sensor);

            PSMManager.Instance.ReadPSMSensorTypes();
            Dictionary<string, PSMSensorType> dicPSMSensorTypes = PSMManager.Instance.GetPSMSensorTypes();

            cboSenosrType.Items.Clear();

            if (dicPSMSensorTypes != null)
            {
                foreach (KeyValuePair<string, PSMSensorType> pair in dicPSMSensorTypes)
                {
                    cboSenosrType.Items.Add(pair.Value);
                }
            }

            if (sensor.SensorType != null)
            {
                int nIndex = cboSenosrType.Items.IndexOf(sensor.SensorType);
                cboSenosrType.SelectedIndex = nIndex;
            }
        }

        private void SetDeadLine(PSMSensor sensor)
        {
            if (sensor.InstallDate != null && sensor.SensorType != null)
                SetDeadLine(sensor.InstallDate.Data, sensor.SensorType.LifeTimeMonth);
            else
                lblDeadLine.Text = "";
        }

        private void SetDeadLine()
        {
            if (cboSenosrType.SelectedIndex >= 0)
            {
                PSMSensorType sensorType = (PSMSensorType)cboSenosrType.Items[cboSenosrType.SelectedIndex];
                SetDeadLine(dtpIntallDate.Value, sensorType.LifeTimeMonth);
            }
            else
                lblDeadLine.Text = "";
        }

        private void SetDeadLine(DateTime dtInstall, int nLifeTimeMonth)
        {
            DateTime dtDeadLine = dtInstall.AddMonths(nLifeTimeMonth);
            lblDeadLine.Text = string.Format("{0}년 {1}월 {2}일", dtDeadLine.Year, dtDeadLine.Month, dtDeadLine.Day);
        }

        private bool SaveSensorLifeTime(PSMSensor sensor)
        {
            string strInstallDate = "NULL", strTypeName = "NULL";

            if (sensor.InstallDate != null)
            {
                strInstallDate = string.Format("'{0}-{1}-{2} 0:0:0'", sensor.InstallDate.Data.Year, sensor.InstallDate.Data.Month, sensor.InstallDate.Data.Day);
            }

            if (sensor.SensorType != null)
                strTypeName = "'" + sensor.SensorType.TypeName + "'";

            string strSQL = string.Format("Update PSMSensor set InstallDate = {0}, SensorTypeName = {1} where ID = {2}",
                strInstallDate, strTypeName, sensor.ID);

            //System.Diagnostics.Trace.WriteLine(strSQL);

            return FormMain.Instance.DBManager.GetResultData(strSQL, 0) != null;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (m_sensor != null)
            {
                m_sensor.SensorType = (PSMSensorType)cboSenosrType.SelectedItem;
                m_sensor.InstallDate = new DBUtility.VariousData<DateTime>(dtpIntallDate.Value);

                if (SaveSensorLifeTime(m_sensor))
                {
                    if (FormPSMList.Instance != null && FormPSMList.Instance.IsDisposed == false)
                    {
                        NetworkManager.Instance.SendRefreshSensorLifeTime();
                        //FormPSMList.Instance.CheckPSMSensorLifeTime();
                    }
                }
            }

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }

        private void btnNewType_Click(object sender, EventArgs e)
        {
            FormPSMSensorType frm = new FormPSMSensorType(this);
            DialogResult result = frm.ShowDialog(this);

            // 새 타입 입력을 취소했더라도 그 사이 다른 시스템을 통하여 새로운 타입이 추가되었을 수 있다.
            Dictionary<string, PSMSensorType> dicPSMSenosrTypes = PSMManager.Instance.GetPSMSensorTypes();

            if (dicPSMSenosrTypes != null)
            {
                foreach (KeyValuePair<string, PSMSensorType> pair in dicPSMSenosrTypes)
                {
                    if (cboSenosrType.Items.Contains(pair.Value) == false)
                        cboSenosrType.Items.Add(pair.Value);
                }
            }

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                PSMSensorType sensorType = frm.Result;

                if (sensorType != null)
                {
                    int nIndex = cboSenosrType.Items.IndexOf(sensorType);
                    cboSenosrType.SelectedIndex = nIndex;
                }
            }
        }

        private void cboSenosrType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDeadLine();
        }

        private void dtpIntallDate_ValueChanged(object sender, EventArgs e)
        {
            SetDeadLine();
        }

        public void RemoveSensorType(PSMSensorType sensorType)
        {
            cboSenosrType.Items.Remove(sensorType);
        }
    }
}
