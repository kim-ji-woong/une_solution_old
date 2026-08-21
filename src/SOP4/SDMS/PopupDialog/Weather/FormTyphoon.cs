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
    public partial class FormTyphoon : Form, IWeatherForm
    {
        private int m_nIndex = -1;
        private List<Typhoon> m_typhoons = new List<Typhoon>();
        // 최대 풍속을 5단계로 나눈다.
        // 마지막 5단계는 배열의 마지막 값 이후가 되므로 생략한다.
        private float[] m_arrMaxWindSpeedLevel = new float[4] { -1.0f, -1.0f, -1.0f, -1.0f };
        private FormWeatherDisplay m_frmOwner = null;

        private string m_strCurrentStatus = null;

        private WeatherSimulator.WeatherData m_weatherDataCurrView = null;
        private WeatherSimulator.WeatherData m_weatherDataView = null;


        public FormTyphoon(FormWeatherDisplay frm)
        {
            InitializeComponent();

            panelLocation.DisplayLength = 8;
            panelLocation.BufferLength = 11;
            panelLocation.NotMovingAlignment = ContentAlignment.TopCenter;
            panelLocation.MovingAlignment = ContentAlignment.TopLeft;
            panelLocation.DisplayFont = this.labelCenterPosition.Font;
            panelLocation.TextColor = this.labelCenterPosition.ForeColor;

            pictureBoxRight.EnabledImage = global::SDMS.Properties.Resources.right_arrow_clicked;
            pictureBoxRight.DisabledImage = global::SDMS.Properties.Resources.right_arrow_normal;
            pictureBoxLeft.EnabledImage = global::SDMS.Properties.Resources.left_arrow_clicked;
            pictureBoxLeft.DisabledImage = global::SDMS.Properties.Resources.left_arrow_normal;

            m_frmOwner = frm;
        }

        private void FormTyphoon_Load(object sender, EventArgs e)
        {
        }

        public void UpdateData(List<WeatherSimulator.WeatherData> weatherDatas)
        {
            m_weatherDataView = m_weatherDataCurrView;

            m_typhoons.Clear();

            foreach (WeatherData data in weatherDatas)
            {
                m_typhoons.Add((Typhoon)data);
            }

            if (m_typhoons.Count == 0)
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
                if (nIndex < m_typhoons.Count)
                {
                    Typhoon typhoon = m_typhoons[nIndex];
                    Reload(typhoon);
                }

                pictureBoxLeft.Enabled = m_nIndex > 0;
                pictureBoxRight.Enabled = m_nIndex < m_typhoons.Count - 1;
            }
        }

        private void Reload(Typhoon typhoon)
        {
            this.m_weatherDataCurrView = typhoon;

            if (typhoon == null)
            {
                pictureBoxWindDirection.Image = global::SDMS.Properties.Resources.dir_n;
                labelDate.Visible = labelTime.Visible = false;
                /* labelCenterPosition.Visible = */ labelCenterPressure.Visible = false;
                labelWindSpeed.Visible = labelWindRadius.Visible = labelMovingSpeed.Visible = false;

                panelLocation.Visible = false;

                SetStatus(null);
            }
            else
            {
                labelDate.Visible = labelTime.Visible = true;
                /* labelCenterPosition.Visible = */ labelCenterPressure.Visible = true;
                labelWindSpeed.Visible = labelWindRadius.Visible = labelMovingSpeed.Visible = true;

                //labelCenterPosition.Text = typhoon.CenterLocation;
                pictureBoxWindDirection.Image = GetDirImage(typhoon);
                SetTime(typhoon.Time);

                panelLocation.Visible = true;
                SetLocation(typhoon);

                SetMaxWindSpeed(typhoon);
                SetWindRadius(typhoon);
                SetMovingSpeed(typhoon);
                SetCenterPressure(typhoon);

                SetStatus(typhoon.Etc);
            }
        }

        public bool ApplyBeforeWeatherData()
        {
            if (this.m_weatherDataView == null)
                return false;

            Typhoon weatherDataView = this.m_weatherDataView as Typhoon;

            this.m_weatherDataCurrView = null;

            int nIndex = 0;

            foreach (Typhoon item in this.m_typhoons)
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

        private void SetMaxWindSpeed(Typhoon typhoon)
        {
            if (typhoon.MaxSpeed == null)
                labelWindSpeed.Visible = false;
            else
                FormRain.SetSpeedData(typhoon.MaxSpeed.Data, labelWindSpeed);
        }

        private void SetWindRadius(Typhoon typhoon)
        {
            if (typhoon.WindRadius == null)
                labelWindRadius.Visible = false;
            else
            {
                if (typhoon.WindRadius.Data >= 100.0f && typhoon.WindRadius.Data < 1000.0f)
                    labelWindRadius.Location = new Point(32, 36);
                else if (typhoon.WindRadius.Data >= 1000.0f)
                    labelWindRadius.Location = new Point(24, 36);
                else if (typhoon.WindRadius.Data < 100.0f && typhoon.WindRadius.Data >= 10.0f)
                    labelWindRadius.Location = new Point(40, 36);
                else
                    labelWindRadius.Location = new Point(48, 36);

                labelWindRadius.Text = string.Format("{0:F0}", typhoon.WindRadius.Data);
            }
        }

        private void SetMovingSpeed(Typhoon typhoon)
        {
            if (typhoon.MoveSpeed == null)
                labelMovingSpeed.Visible = false;
            else
            {
                if (typhoon.MoveSpeed.Data >= 10.0f && typhoon.MoveSpeed.Data < 100.0f)
                    labelMovingSpeed.Location = new Point(79, 19);
                else if (typhoon.MoveSpeed.Data >= 100.0f)
                    labelMovingSpeed.Location = new Point(65, 19);
                else
                    labelMovingSpeed.Location = new Point(95, 19);

                labelMovingSpeed.Text = string.Format("{0:F0}", typhoon.MoveSpeed.Data);
            }
        }

        private void SetCenterPressure(Typhoon typhoon)
        {
            if (typhoon.CenterPressure == null)
                labelCenterPressure.Visible = false;
            else
            {
                if (typhoon.CenterPressure.Data >= 1000.0f)
                    labelCenterPressure.Location = new Point(149, 16);
                else if (typhoon.CenterPressure.Data >= 100.0f)
                    labelCenterPressure.Location = new Point(162, 16);
                else if (typhoon.CenterPressure.Data >= 10.0f)
                    labelCenterPressure.Location = new Point(174, 16);
                else
                    labelCenterPressure.Location = new Point(186, 16);

                labelCenterPressure.Text = string.Format("{0:F1}", typhoon.CenterPressure.Data);
            }
        }

        private Image GetDirImage(Typhoon typhoon)
        {
            if (typhoon.WindDirection == null)
                return global::SDMS.Properties.Resources.dir_n;
            else if (typhoon.WindDirection.Data == Typhoon.Direction.East)
                return global::SDMS.Properties.Resources.dir_e;
            else if (typhoon.WindDirection.Data == Typhoon.Direction.ESouthE)
                return global::SDMS.Properties.Resources.dir_see;
            else if (typhoon.WindDirection.Data == Typhoon.Direction.SouthEast)
                return global::SDMS.Properties.Resources.dir_se;
            else if (typhoon.WindDirection.Data == Typhoon.Direction.SSEast)
                return global::SDMS.Properties.Resources.dir_sse;
            else if (typhoon.WindDirection.Data == Typhoon.Direction.South)
                return global::SDMS.Properties.Resources.dir_s;
            else if (typhoon.WindDirection.Data == Typhoon.Direction.SSWest)
                return global::SDMS.Properties.Resources.dir_ssw;
            else if (typhoon.WindDirection.Data == Typhoon.Direction.SouthWest)
                return global::SDMS.Properties.Resources.dir_sw;
            else if (typhoon.WindDirection.Data == Typhoon.Direction.WSouthW)
                return global::SDMS.Properties.Resources.dir_sww;
            else if (typhoon.WindDirection.Data == Typhoon.Direction.West)
                return global::SDMS.Properties.Resources.dir_w;
            else if (typhoon.WindDirection.Data == Typhoon.Direction.WNorthW)
                return global::SDMS.Properties.Resources.dir_nww;
            else if (typhoon.WindDirection.Data == Typhoon.Direction.NorthWest)
                return global::SDMS.Properties.Resources.dir_nw;
            else if (typhoon.WindDirection.Data == Typhoon.Direction.NNWest)
                return global::SDMS.Properties.Resources.dir_nnw;
            else if (typhoon.WindDirection.Data == Typhoon.Direction.North)
                return global::SDMS.Properties.Resources.dir_n;
            else if (typhoon.WindDirection.Data == Typhoon.Direction.NNEast)
                return global::SDMS.Properties.Resources.dir_nne;
            else if (typhoon.WindDirection.Data == Typhoon.Direction.NorthEast)
                return global::SDMS.Properties.Resources.dir_ne;
            //else if (typhoon.WindDirection.Data == Typhoon.Direction.ENorthE)
                return global::SDMS.Properties.Resources.dir_nee;
        }

        private void SetTime(DateTime dtRain)
        {
            labelDate.Text = string.Format("{0}년 {1:00}월 {2:00}일", dtRain.Year, dtRain.Month, dtRain.Day);
            labelTime.Text = string.Format("{0:00}:{1:00}", dtRain.Hour, dtRain.Minute);
        }

        private void SetLocation(Typhoon typhoon)
        {
            if (typhoon.CenterLocation == null)
            {
                panelLocation.RealTimeInfo = "";
                panelLocation.StopTimer();
            }
            else
            {
                panelLocation.StopTimer();
                //panelLocation.DisplayLength = typhoon.CenterLocation.Trim().Length;
                panelLocation.RealTimeInfo = typhoon.CenterLocation.Trim();
                panelLocation.DrawMovingText();
            }
        }

        public void ReadOptions(DBUtility.WebDBManager dbMgr, int nSiteID)
        {
            FormRain.ReadWindSpeedLevel(dbMgr, nSiteID, "TyphoonMaxWindSpeed", m_arrMaxWindSpeedLevel);
        }

        public void MoveNext()
        {
            if (m_nIndex < 0 || m_typhoons.Count == 0)
                return;

            m_nIndex++;

            if (m_nIndex >= m_typhoons.Count)
                m_nIndex = 0;

            Reload(m_nIndex);
            m_frmOwner.ResetTimer();
        }

        public void MovePrev()
        {
            if (m_nIndex < 0 || m_typhoons.Count == 0)
                return;

            m_nIndex--;

            if (m_nIndex < 0)
                m_nIndex = m_typhoons.Count - 1;

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
