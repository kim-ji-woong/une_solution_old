using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WeatherSimulator
{
    public partial class FormMain : Form
    {
        public enum WeatherInfoType { UpdateData = 0 };

        private FormRainNWind m_frmRain = new FormRainNWind();
        private FormTyphoon m_frmTyphoon = new FormTyphoon();
        private FormEarthquake m_frmEarthquake = new FormEarthquake();
        private bool m_isSimulationMode = false;

        private NetworkManager m_netMgr = null;

        private static FormMain m_instance = null;

        public static FormMain Instance
        {
            get { return m_instance; }
        }

        public bool SimulationMode
        {
            get { return m_isSimulationMode; }
        }

        public FormMain(int nSOPGenUserID, int nSiteID, bool isSimulationMode)
        {
            m_instance = this;
            InitializeComponent();

            m_isSimulationMode = isSimulationMode;
            Init(nSOPGenUserID, nSiteID);
            m_netMgr = NetworkManager.Instance;
            m_netMgr.SiteID = nSiteID;
        }

        private void Init(int nSOPGenUserID, int nSiteID)
        {
            m_frmRain.TopLevel = false;
            m_frmTyphoon.TopLevel = false;
            m_frmEarthquake.TopLevel = false;

            panelMain.Controls.Add(m_frmRain);
            panelMain.Controls.Add(m_frmTyphoon);
            panelMain.Controls.Add(m_frmEarthquake);

            DateTime dtCreate;
            int nAvailablePeriodDay;

            DataManager.Instance.SOPGenUserID = nSOPGenUserID;
            DataManager.Instance.SiteID = nSiteID;

            if (DataManager.Instance.LoadData(out dtCreate, out nAvailablePeriodDay))
            {
                m_frmRain.LoadData(true);
                m_frmTyphoon.LoadData(true);
                m_frmEarthquake.LoadData(true);

                ShowLabelCreateTime(dtCreate);

                if (nAvailablePeriodDay <= 0 || nAvailablePeriodDay >= cboDuration.Items.Count - 1)
                    cboDuration.SelectedIndex = -1;
                else
                    cboDuration.SelectedIndex = nAvailablePeriodDay;
            }
            else
            {
                cboDuration.SelectedIndex = -1;
                HideLabelCreateTime();
            }

            m_frmRain.Show();
            m_frmTyphoon.Show();
            m_frmEarthquake.Show();
        }

        private void ShowLabelCreateTime(DateTime time)
        {
            labelDataCreatedTime.Visible = true;
            labelDataCreatedTime.Text = "작성시간 : " + string.Format("{0}년 {1}월 {2}일 {3}시 {4}분", time.Year, time.Month, time.Day, time.Hour, time.Minute);
        }

        private void HideLabelCreateTime()
        {
            labelDataCreatedTime.Visible = false;
        }

        private void cboWeatherType_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_frmRain.Visible = m_frmTyphoon.Visible = m_frmEarthquake.Visible = false;

            if (cboWeatherType.SelectedIndex == (int)WeatherData.DataType.RainNWind)
                m_frmRain.Visible = true;
            else if (cboWeatherType.SelectedIndex == (int)WeatherData.DataType.Typhoon)
                m_frmTyphoon.Visible = true;
            else if (cboWeatherType.SelectedIndex == (int)WeatherData.DataType.Earthquake)
                m_frmEarthquake.Visible = true;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            cboWeatherType.SelectedIndex = 0;
        }

        public DataGridViewRow MakeNewRow(DataGridView grid)
        {
            if (grid.AllowUserToAddRows)
            {
                DataGridViewRow row = (DataGridViewRow)grid.Rows[grid.Rows.Count - 1].Clone();
                grid.Rows.Add(row);

                return grid.Rows[grid.Rows.Count - 2];
            }
            else
            {
                grid.AllowUserToAddRows = true;

                DataGridViewRow row = (DataGridViewRow)grid.Rows[grid.Rows.Count - 1].Clone();
                grid.Rows.Add(row);

                grid.AllowUserToAddRows = false;
            }

            return grid.Rows[grid.Rows.Count - 1];
        }

        private void checkBoxNewData_CheckedChanged(object sender, EventArgs e)
        {
            m_frmRain.EditMode(checkBoxEditMode.Checked);
            m_frmTyphoon.EditMode(checkBoxEditMode.Checked);
            m_frmEarthquake.EditMode(checkBoxEditMode.Checked);

            cboDuration.Enabled = checkBoxEditMode.Checked;
            btnApply.Enabled = checkBoxEditMode.Checked;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            m_frmRain.ApplyCurrentData();
            m_frmTyphoon.ApplyCurrentData();
            m_frmEarthquake.ApplyCurrentData();

            int nAvailablePeriod = -1;

            if (cboDuration.SelectedIndex > 0)
                nAvailablePeriod = cboDuration.SelectedIndex;

            bool dbIsEmpty = false;

            if (DataManager.Instance.SaveDB(nAvailablePeriod, out dbIsEmpty) && !dbIsEmpty)
            {
                ShowLabelCreateTime(DateTime.Now);

                // SOP Server로 데이터 전달
                m_netMgr.SendMessage(SDMS.TCP_ID.WEATHER_INFO, (int)WeatherInfoType.UpdateData);
            }
            else
                HideLabelCreateTime();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_netMgr.ReleaseThread();
        }
    }
}
