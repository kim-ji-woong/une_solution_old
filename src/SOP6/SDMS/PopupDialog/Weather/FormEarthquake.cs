using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeatherSimulator;

namespace SDMS.WeatherDisplay
{
    public partial class FormEarthquake : Form, IWeatherForm
    {
        private int m_nIndex = -1;
        private List<Earthquake> m_earthquakes = new List<Earthquake>();
        private FormWeatherDisplay m_frmOwner = null;

        private string m_strCurrentStatus = null;

        private WeatherSimulator.WeatherData m_weatherDataCurrView = null;
        private WeatherSimulator.WeatherData m_weatherDataView = null;


        public FormEarthquake(FormWeatherDisplay frm)
        {
            InitializeComponent();

            panelLocation.DisplayLength = 4;
            panelLocation.BufferLength = 8;
            panelLocation.NotMovingAlignment = ContentAlignment.TopCenter;
            panelLocation.MovingAlignment = ContentAlignment.TopLeft;
            panelLocation.Font = this.labelLocation.Font;
            panelLocation.TextColor = labelLocation.ForeColor;

            pictureBoxRight.EnabledImage = global::SDMS.Properties.Resources.right_arrow_clicked;
            pictureBoxRight.DisabledImage = global::SDMS.Properties.Resources.right_arrow_normal;
            pictureBoxLeft.EnabledImage = global::SDMS.Properties.Resources.left_arrow_clicked;
            pictureBoxLeft.DisabledImage = global::SDMS.Properties.Resources.left_arrow_normal;

            m_frmOwner = frm;
        }

        private void FormEarthquake_Load(object sender, EventArgs e)
        {
            int nRightSpace = this.Size.Width - (pictureBoxRight.Location.X + pictureBoxRight.Size.Width);
            pictureBoxLeft.Location = new Point(nRightSpace, pictureBoxRight.Location.Y);
        }

        public void UpdateData(List<WeatherSimulator.WeatherData> weatherDatas)
        {
            m_weatherDataView = m_weatherDataCurrView;

            m_earthquakes.Clear();

            foreach (WeatherData data in weatherDatas)
            {
                m_earthquakes.Add((Earthquake)data);
            }

            if (m_earthquakes.Count == 0)
                Reload(null);
            else
            {
                m_nIndex = 0;
                Reload(m_nIndex);
            }
        }

        private void Reload(int nIndex)
        {
            if (nIndex < 0)
            {
                Reload(null);

                pictureBoxLeft.Enabled = pictureBoxRight.Enabled = false;
            }
            else
            {
                if (nIndex < m_earthquakes.Count)
                {
                    Earthquake earthquake = m_earthquakes[nIndex];
                    Reload(earthquake);
                }

                pictureBoxLeft.Enabled = m_nIndex > 0;
                pictureBoxRight.Enabled = m_nIndex < m_earthquakes.Count - 1;
            }
        }

        private void Reload(Earthquake earthquake)
        {
            this.m_weatherDataCurrView = earthquake;

            if (earthquake == null)
            {
                /*labelLocation.Visible = */labelStrength.Visible = labelHeight.Visible = false;

                panelLocation.Visible = false;

                SetStatus(null);
            }
            else
            {
                /*labelLocation.Visible = */labelStrength.Visible = labelHeight.Visible = true;

                panelLocation.Visible = true;

                SetLocation(earthquake);
                SetStrength(earthquake);
                SetHeight(earthquake);

                SetStatus(earthquake.Etc);
            }
        }

        public bool ApplyBeforeWeatherData()
        {
            if (this.m_weatherDataView == null)
                return false;

            Earthquake weatherDataView = this.m_weatherDataView as Earthquake;

            this.m_weatherDataCurrView = null;

            int nIndex = 0;

            foreach (Earthquake item in this.m_earthquakes)
            {
                if (item.ID == weatherDataView.ID)
                {
                    this.Reload(nIndex);
                    m_nIndex = nIndex;

                    break;
                }

                nIndex++;
            }

            if (this.m_weatherDataCurrView == null)
                return false;
            else
                return true;
        }

        private void SetStatus(string strStatus)
        {
            m_strCurrentStatus = strStatus;
            m_frmOwner.SetStatus(m_strCurrentStatus, this);
        }

        private void SetStrength(Earthquake earthquake)
        {
            if (earthquake.Strength == null)
                labelStrength.Visible = false;
            else
            {
                if (earthquake.Strength.Data < 10.0f)
                    labelStrength.Location = new Point(210, 101);
                else
                    labelStrength.Location = new Point(200, 101);

                labelStrength.Text = string.Format("{0:F1}", earthquake.Strength.Data);
            }
        }

        private void SetHeight(Earthquake earthquake)
        {
            if (earthquake.TsunamiHeight == null)
                labelHeight.Visible = false;
            else
            {
                if (earthquake.TsunamiHeight.Data < 10.0f)
                    labelHeight.Location = new Point(357, 99);
                else if (earthquake.TsunamiHeight.Data < 100.0f)
                    labelHeight.Location = new Point(347, 101);
                else
                    labelHeight.Location = new Point(337, 101);

                labelHeight.Text = string.Format("{0:F1}", earthquake.TsunamiHeight.Data);
            }
        }

        private void SetLocation(Earthquake earthquake)
        {
            if (earthquake.Location == null)
            {
                panelLocation.RealTimeInfo = "";
                panelLocation.StopTimer();
            }
            else
            {
                panelLocation.StopTimer();
                panelLocation.RealTimeInfo = earthquake.Location.Trim();
                panelLocation.DrawMovingText();
            }
        }

        public void MoveNext()
        {
            if (m_nIndex < 0 || m_earthquakes.Count == 0)
                return;

            m_nIndex++;

            if (m_nIndex >= m_earthquakes.Count)
                m_nIndex = 0;

            Reload(m_nIndex);
            m_frmOwner.ResetTimer();
        }

        public void MovePrev()
        {
            if (m_nIndex < 0 || m_earthquakes.Count == 0)
                return;

            m_nIndex--;

            if (m_nIndex < 0)
                m_nIndex = m_earthquakes.Count - 1;

            Reload(m_nIndex);
            m_frmOwner.ResetTimer();
        }

        private void pictureBoxRight_Click(object sender, EventArgs e)
        {
            MoveNext();
        }

        private void pictureBoxLeft_Click(object sender, EventArgs e)
        {
            MovePrev();
        }

        public void SendStatus()
        {
            m_frmOwner.SetStatus(m_strCurrentStatus, this);
        }

    }
}
