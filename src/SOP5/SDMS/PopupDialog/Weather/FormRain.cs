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
    public partial class FormRain : Form, IWeatherForm
    {
        // 폭우의 기준
        private class HeavyRainDefinition
        {
            // 단위 : mm
            private float m_fRainPerHour = 30.0f;
            private float m_fRainPerDay = 100.0f;
            // true이면 시간당 강수량과 일 강수량을 동시에 만족해야 한다.
            private bool m_isAndOperation = false;

            // 시간당 강수량(mm)
            public float RainPerHour
            {
                get { return m_fRainPerHour; }
                set { m_fRainPerHour = value; }
            }

            // 일 강수량(mm)
            public float RainPerDay
            {
                get { return m_fRainPerDay; }
                set { m_fRainPerDay = value; }
            }

            // true이면 시간당 강수량과 일 강수량을 동시에 만족해야 한다.
            public bool AndOperation
            {
                get { return m_isAndOperation; }
                set { m_isAndOperation = value; }
            }

            public void ReadHeavyRainOption(string strCondition)
            {
                strCondition = strCondition.ToLower();

                m_fRainPerHour = ReadRain("hour", strCondition);
                m_fRainPerDay = ReadRain("day", strCondition);
                m_isAndOperation = strCondition.Contains("and");
            }

            private float ReadRain(string strOpt, string strCondition)
            {
                string strValue = FormRain.GetValue(strCondition, strOpt);

                if (strValue == null)
                    return -1.0f;
                
                if (strValue.Length == 0)
                    return -1.0f;

                double dRain;

                if (!double.TryParse(strValue, out dRain))
                    return -1.0f;

                return (float)dRain;
            }

            public bool IsHeavyRain(float fRainHour, float fRainDay)
            {
                bool hour = m_fRainPerHour < 0.0f ? false : fRainHour >= m_fRainPerHour;
                bool day = m_fRainPerDay < 0.0f ? false : fRainDay >= m_fRainPerDay;

                if (m_isAndOperation)
                    return hour && day;

                return hour || day;
            }
        }

        private int m_nIndex = -1;
        private List<RainNWind> m_rains = new List<RainNWind>();
        private FormWeatherDisplay m_frmOwner = null;

        // 평균 풍속을 5단계로 나눈다.
        // 마지막 5단계는 배열의 마지막 값 이후가 되므로 생략한다.
        private float[] m_arrAveWindSpeedLevel = new float[4] { -1.0f, -1.0f, -1.0f, -1.0f };
        // 최대 풍속을 5단계로 나눈다.
        // 마지막 5단계는 배열의 마지막 값 이후가 되므로 생략한다.
        private float[] m_arrMaxWindSpeedLevel = new float[4] { -1.0f, -1.0f, -1.0f, -1.0f };

        // 폭우의 기준
        private HeavyRainDefinition m_heavyRain = new HeavyRainDefinition();
        private string m_strCurrentStatus = null;

        private WeatherSimulator.WeatherData m_weatherDataCurrView = null;
        private WeatherSimulator.WeatherData m_weatherDataView = null;


        public FormRain(FormWeatherDisplay frm)
        {
            InitializeComponent();

            pictureBoxRight.EnabledImage = global::SDMS.Properties.Resources.right_arrow_clicked;
            pictureBoxRight.DisabledImage = global::SDMS.Properties.Resources.right_arrow_normal;
            pictureBoxLeft.EnabledImage = global::SDMS.Properties.Resources.left_arrow_clicked;
            pictureBoxLeft.DisabledImage = global::SDMS.Properties.Resources.left_arrow_normal;

            m_frmOwner = frm;
        }

        private void FormRain_Load(object sender, EventArgs e)
        {
        }

        public void UpdateData(List<WeatherData> weatherDatas)
        {
            m_weatherDataView = m_weatherDataCurrView;

            m_rains.Clear();

            foreach (WeatherData data in weatherDatas)
            {
                m_rains.Add((RainNWind)data);
            }

            if (m_rains.Count == 0)
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
                if (nIndex < m_rains.Count)
                {
                    RainNWind rain = m_rains[nIndex];
                    Reload(rain);
                }

                pictureBoxLeft.Enabled = m_nIndex > 0;
                pictureBoxRight.Enabled = m_nIndex < m_rains.Count - 1;
            }
        }

        private void Reload(RainNWind rain)
        {
            this.m_weatherDataCurrView = rain;

            if (rain == null)
            {
                panelLeft.BackgroundImage = global::SDMS.Properties.Resources.no_rain;
                labelDate.Visible = labelTime.Visible = false;
                labelRainHour.Visible = labelRainDay.Visible = false;
                labelAveWindSpeed.Visible = labelMaxWindSpeed.Visible = false;

                SetStatus(null);
            }
            else
            {
                labelDate.Visible = labelTime.Visible = true;
                labelRainHour.Visible = labelRainDay.Visible = true;
                labelAveWindSpeed.Visible = labelMaxWindSpeed.Visible = true;

                float fRainHour = -1.0f, fRainDay = -1.0f;

                if (rain.RainHour != null)
                    fRainHour = rain.RainHour.Data;

                if (rain.RainDay != null)
                    fRainDay = rain.RainDay.Data;

                if (fRainHour <= 0.0f && fRainDay <= 0.0f)
                    panelLeft.BackgroundImage = global::SDMS.Properties.Resources.no_rain;
                else
                {
                    if (m_heavyRain.IsHeavyRain(fRainHour, fRainDay))
                        panelLeft.BackgroundImage = global::SDMS.Properties.Resources.heavy_rain;
                    else
                        panelLeft.BackgroundImage = global::SDMS.Properties.Resources.small_rain;
                }

                if (fRainHour <= 0.0f)
                    fRainHour = 0.0f;

                if (fRainDay <= 0.0f)
                    fRainDay = 0.0f;

                float fAveSpeed = rain.WindSpeedAve == null || rain.WindSpeedAve.Data <= 0.0f ? 0.0f : rain.WindSpeedAve.Data;
                float fMaxSpeed = rain.WindSpeedMax == null || rain.WindSpeedMax.Data <= 0.0f ? 0.0f : rain.WindSpeedMax.Data;

                SetHour(fRainHour);
                SetDay(fRainDay);
                SetAveSpeed(fAveSpeed);
                SetMaxSpeed(fMaxSpeed);

                SetTime(rain.Time);

                SetStatus(rain.Region);
            }
        }

        public bool ApplyBeforeWeatherData()
        {
            if (this.m_weatherDataView == null)
                return false;

            RainNWind weatherDataView = this.m_weatherDataView as RainNWind;

            this.m_weatherDataCurrView = null;

            int nIndex = 0;
            
            foreach (RainNWind item in this.m_rains)
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

        private void SetTime(DateTime dtRain)
        {
            labelDate.Text = string.Format("{0}년 {1:00}월 {2:00}일", dtRain.Year, dtRain.Month, dtRain.Day);
            labelTime.Text = string.Format("{0:00}:{1:00}", dtRain.Hour, dtRain.Minute);
        }

        private void SetHour(float fRain)
        {
            SetRainData(fRain, labelRainHour);
        }

        private void SetDay(float fRain)
        {
            SetRainData(fRain, labelRainDay);
        }

        private void SetAveSpeed(float fSpeed)
        {
            int nWindSpeedLevel = GetWindSpeedLevel(fSpeed, m_arrAveWindSpeedLevel);
            panelAveWindData.BackgroundImage = GetWindSpeedLevelImage(nWindSpeedLevel);
            SetSpeedData(fSpeed, labelAveWindSpeed);
        }

        private void SetMaxSpeed(float fSpeed)
        {
            int nWindSpeedLevel = GetWindSpeedLevel(fSpeed, m_arrMaxWindSpeedLevel);
            panelMaxWindData.BackgroundImage = GetWindSpeedLevelImage(nWindSpeedLevel);
            SetSpeedData(fSpeed, labelMaxWindSpeed);
        }

        private int GetWindSpeedLevel(float fSpeed, float[] arrLevels)
        {
            for (int i=0;i<4;i++)
            {
                if (arrLevels[i] < 0.0f)
                    return i + 1;
                else if (arrLevels[i] >= fSpeed)
                    return i + 1;
            }

            return 5;
        }

        public static void SetSpeedData(float fSpeed, Label label)
        {
            if (fSpeed < 10.0f)
                label.Location = new Point(30, 68);
            else if (fSpeed < 100.0f)
                label.Location = new Point(11, 68);
            else
                label.Location = new Point(-6, 68);

            label.Text = string.Format("{0:F1}", fSpeed);
        }

        public static Image GetWindSpeedLevelImage(int nLevel)
        {
            if (nLevel == 1)
                return global::SDMS.Properties.Resources.wind_speed_1;
            else if (nLevel == 2)
                return global::SDMS.Properties.Resources.wind_speed_2;
            else if (nLevel == 3)
                return global::SDMS.Properties.Resources.wind_speed_3;
            else if (nLevel == 4)
                return global::SDMS.Properties.Resources.wind_speed_4;
            //else if (nLevel == 5)
                return global::SDMS.Properties.Resources.wind_speed_5;
        }

        private void SetRainData(float fRain, Label label)
        {
            if (fRain < 1000.0f && fRain >= 100.0f)
                label.Location = new Point(16, 53);
            else if (fRain >= 1000.0f)
                label.Location = new Point(4, 53);
            else if (fRain < 100.0f && fRain >= 10.0f)
                label.Location = new Point(34, 53);
            else
                label.Location = new Point(44, 53);

            label.Text = string.Format("{0:F1}", fRain);
        }

        public void ReadOptions(DBUtility.WebDBManager dbMgr, int nSiteID)
        {
            ReadHeavyRainOption(dbMgr, nSiteID);
            ReadWindSpeedLevel(dbMgr, nSiteID, "NormalAveWindSpeedLevel", m_arrAveWindSpeedLevel);
            ReadWindSpeedLevel(dbMgr, nSiteID, "NormalMaxWindSpeedLevel", m_arrMaxWindSpeedLevel);
        }

        public static void ReadWindSpeedLevel(DBUtility.WebDBManager dbMgr, int nSiteID, string strPropertyName, float[] arrLevels)
        {
            string strSQL = string.Format("Select PropertyValue from OptionSDMS where PropertyName = '{0}' and SiteID = {1}",
                strPropertyName, nSiteID);
            System.Collections.ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
            {
                for (int i=0;i<4;i++)
                {
                    arrLevels[i] = -1.0f;
                }
            }
            else
            {
                double dLevelData = 0.0;
                string strLevels = DBUtility.WebDBManager.GetStringField(arrResult[0], "");

                for (int i=1;i<=4;i++)
                {
                    string strValue = GetValue(strLevels, i.ToString());

                    if (strValue == null)
                        break;

                    if (!double.TryParse(strValue.Trim(), out dLevelData))
                        break;

                    if (i == 1)
                        arrLevels[i - 1] = (float)dLevelData;
                    else
                    {
                        // 이전 단계의 값이 더 커서는 안된다.
                        if (arrLevels[i - 2] >= (float)dLevelData)
                            break;
                        else
                            arrLevels[i - 1] = (float)dLevelData;
                    }
                }
            }
        }

        private void ReadHeavyRainOption(DBUtility.WebDBManager dbMgr, int nSiteID)
        {
            string strSQL = "Select PropertyValue from OptionSDMS where PropertyName = 'HeavyRainDefinition' and SiteID = " + nSiteID.ToString();
            System.Collections.ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return;

            string strCondition = DBUtility.WebDBManager.GetStringField(arrResult[0], "");
            m_heavyRain.ReadHeavyRainOption(strCondition);            
        }

        // 1(...), a(...),... 등으로 이루어진 문자열이 있을때
        // 1, a등을 TagName이라 하고 괄호안의 내용을 TagValue라 한다.
        // strTagName에 대한 TagValue를 찾아 리턴해준다.
        // 없을 경우 null을 리턴한다.
        public static string GetValue(string str, string strTagName)
        {
            int nIndex = str.IndexOf(strTagName);

            if (nIndex < 0)
                return null;

            int nIndex1 = str.IndexOf('(', nIndex + strTagName.Length);

            if (nIndex1 < 0)
                return null;

            int nIndex2 = str.IndexOf(')', nIndex1 + 1);

            if (nIndex2 < 0)
                return null;

            string strResult = str.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
            return strResult;
        }

        public void MoveNext()
        {
            if (m_nIndex < 0 || m_rains.Count == 0)
                return;

            m_nIndex++;

            if (m_nIndex >= m_rains.Count)
                m_nIndex = 0;

            Reload(m_nIndex);
            m_frmOwner.ResetTimer();
        }

        public void MovePrev()
        {
            if (m_nIndex < 0 || m_rains.Count == 0)
                return;

            m_nIndex--;

            if (m_nIndex < 0)
                m_nIndex = m_rains.Count - 1;

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
