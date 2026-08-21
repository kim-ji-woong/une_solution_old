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
    public partial class FormPSMSensorAlarm : PopupFormBase
    {
        private PSMSensor m_sensor = null;
        private int m_nSensorNo = -1;

        public FormPSMSensorAlarm(PSMSensor sensor, int nSensorNo)
        {
            this.DoubleBuffered = true;
            InitializeComponent();

            InitCtrlSize(this);
            SetData(sensor, nSensorNo);
        }

        public void SetData(PSMSensor sensor, int nSensorNo)
        {
            lblSensorNo.Text = lblLocation.Text = lblMaterialName.Text = "";

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

            PSMMaterial material = linkedTank != null && linkedTank.Material != null ? linkedTank.Material : null;

            lblSensorNo.Text = nSensorNo.ToString();
            lblMaterialName.Text = material == null ? null : material.Name;
            lblLocation.Text = linkedTank == null ? null : linkedTank.LocationName;

            if (material != null)
            {
                lblDefUnit1.Text = lblDefUnit2.Text = lblDefUnit3.Text = lblCurrentUnit1.Text = lblCurrentUnit2.Text = lblCurrentUnit3.Text = material.UOM;
            }

            textBoxDefLevel1.Text = GetAlarmString(m_sensor.LimitLevel1);
            textBoxDefLevel2.Text = GetAlarmString(m_sensor.LimitLevel2);
            textBoxDefLevel3.Text = GetAlarmString(m_sensor.LimitLevel3);

            textBoxCurrentLevel1.Text = GetAlarmString(m_sensor.LimitLevel1);
            textBoxCurrentLevel2.Text = GetAlarmString(m_sensor.LimitLevel2);
            textBoxCurrentLevel3.Text = GetAlarmString(m_sensor.LimitLevel3);
        }

        private string GetAlarmString(float fLimitLevel)
        {
            return string.Format("{0:00}", fLimitLevel);
        }

        private bool GetAlarmLevel(TextBox textBox, ref float fLevel)
        {
            string strText = textBox.Text.Trim();

            if (strText.Length == 0)
            {
                textBox.Focus();
                MessageBox.Show("알람값을 입력하세요.");
                return false;
            }

            if (float.TryParse(strText, out fLevel))
            {
                if (fLevel <= 0.0f)
                {
                    textBox.Focus();
                    MessageBox.Show("알람값은 0보다 큰 값이어야 합니다.");
                    return false;
                }

                return true;
            }

            textBox.Focus();
            MessageBox.Show("알람값은 0보다 큰 숫자를 입력하여야 합니다.");
            return false;
        }

        private bool SaveSensorAlarm(PSMSensor sensor)
        {
            float fLevel1 = -1.0f, fLevel2 = -1.0f, fLevel3 = -1.0f;

            if (GetAlarmLevel(textBoxCurrentLevel1, ref fLevel1) == false)
                return false;
            if (GetAlarmLevel(textBoxCurrentLevel2, ref fLevel2) == false)
                return false;
            if (GetAlarmLevel(textBoxCurrentLevel3, ref fLevel3) == false)
                return false;

            if (FormPSMList.Instance != null && FormPSMList.Instance.IsDisposed == false)
            {
                NetworkWebManager.Instance.SendPSMSensorAlarmLevel(sensor, fLevel1, fLevel2, fLevel3);
            }

            return true;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (m_sensor != null)
            {
                if (!SaveSensorAlarm(m_sensor))
                    return;
            }

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
