using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace SDMS.WeatherDisplay
{
    public partial class FormWeatherDisplay : Form
    {
        private class TabButton : System.Windows.Forms.Button
        {
            private Image m_imgNormal = null;
            private Image m_imgChecked = null;
            private IWeatherForm m_frm = null;

            private List<WeatherSimulator.WeatherData> m_weatherDatas = new List<WeatherSimulator.WeatherData>();

            public Image NormalImage
            {
                get { return m_imgNormal; }
                set { m_imgNormal = value; }
            }

            public Image CheckedImage
            {
                get { return m_imgChecked; }
                set { m_imgChecked = value; }
            }

            public IWeatherForm Form
            {
                get { return m_frm; }
                set { m_frm = value; }
            }

            public bool Selected
            {
                get { return this.Image == m_imgChecked; }
                set
                {
                    if (value)
                    {
                        this.Image = m_imgChecked;

                        if (this.m_frm != null && !this.m_frm.Visible)
                        {
                            this.m_frm.Show();
                            this.m_frm.SendStatus();
                        }
                    }
                    else
                    {
                        this.Image = m_imgNormal;

                        if (this.m_frm != null && this.m_frm.Visible)
                            this.m_frm.Hide();
                    }
                }
            }

            public List<WeatherSimulator.WeatherData> WeatherDatas
            {
                get { return m_weatherDatas; }
            }

            public TabButton()
            {
            }

            public void UpdateData()
            {
                m_frm.UpdateData(m_weatherDatas);
            }
        }

        private FormRain m_frmRain = null;
        private FormTyphoon m_frmTyphoon = null;
        private FormEarthquake m_frmEarthquake = null;

        private TabButton m_btnSelected = null;
        private List<TabButton> m_tabButtons = new List<TabButton>();

        // 기후정보의 유효기간
        private DateTime m_dtCreate = new DateTime();
        private int m_nAvailablePeriodDay = -1;

        private bool m_isLoading = false;
        private int m_nUpdateInterval = 30000;

        // 사용자가 보고있는 기후정보
        private WeatherSimulator.WeatherData m_weatherDataUserFocused = null;


        public FormWeatherDisplay()
        {
            InitializeComponent();

            InitControls();
        }


        private void InitControls()
        {
            m_frmRain = new FormRain(this);
            m_frmTyphoon = new FormTyphoon(this);
            m_frmEarthquake = new FormEarthquake(this);

            m_frmRain.TopLevel = m_frmTyphoon.TopLevel = m_frmEarthquake.TopLevel = false;

            panelBody.Controls.Add(m_frmRain);
            panelBody.Controls.Add(m_frmTyphoon);
            panelBody.Controls.Add(m_frmEarthquake);

            tabRain.NormalImage = tabRain.Image = global::SDMS.Properties.Resources.rain_normal;
            tabRain.CheckedImage = global::SDMS.Properties.Resources.rain_clicked;
            tabRain.Form = m_frmRain;
            tabTyphoon.NormalImage = tabTyphoon.Image = global::SDMS.Properties.Resources.typhoon_normal;
            tabTyphoon.CheckedImage = global::SDMS.Properties.Resources.typhoon_clicked;
            tabTyphoon.Form = m_frmTyphoon;
            tabEarthquake.NormalImage = tabEarthquake.Image = global::SDMS.Properties.Resources.earthquake_normal;
            tabEarthquake.CheckedImage = global::SDMS.Properties.Resources.earthquake_clicked;
            tabEarthquake.Form = m_frmEarthquake;

            panelBottom.DisplayLength = 29;
            panelBottom.BufferLength = 32;
            panelBottom.DisplayFont = this.labelStatus.Font;
            panelBottom.TextColor = labelStatus.ForeColor;
            panelBottom.NotMovingAlignment = ContentAlignment.MiddleRight;
            panelBottom.MovingAlignment = ContentAlignment.MiddleRight;
        }

        private void FormWeatherDisplay_Load(object sender, EventArgs e)
        {
            m_isLoading = true;
            timerDisplay.Interval = m_nUpdateInterval;
            UpdateData(FormMain.Instance.DBManager, UnE.SOP.ProxySOP.Instance.SiteID);
        }

        // 재난정보 업데이트(외부에서 호출하는 유일한 업데이트 메소드)
        public void UpdateData(DBUtility.WebDBManager dbMgr, int nSiteID)
        {
            // 재난 정보 초기화
            tabRain.WeatherDatas.Clear();
            tabTyphoon.WeatherDatas.Clear();
            tabEarthquake.WeatherDatas.Clear();

            m_frmRain.ReadOptions(dbMgr, nSiteID);
            m_frmTyphoon.ReadOptions(dbMgr, nSiteID);

            // 기본 버튼은 호우 경보 버튼 활성화
            TabButton selectedButton = tabRain;

            int nLogID = ReadLogID(dbMgr, nSiteID);

            if (nLogID > 0)
            {
                // 각각의 재난정보 데이터를 DB에서 로드
                LoadWeatherList(dbMgr, nLogID);

                if (!LoadWeatherLog(dbMgr, nLogID, ref m_dtCreate, ref m_nAvailablePeriodDay))
                    m_nAvailablePeriodDay = -1;

                // tab 컨트롤의 데이터에 재난정보 데이터를 적용
                tabRain.UpdateData();
                tabTyphoon.UpdateData();
                tabEarthquake.UpdateData();


                // 갱신전에 사용자가 조회중이던 재난 정보를 보여주도록 함
                bool bApplyData = false;
                foreach(Control ctl in this.Controls)
                {
                    if(ctl is TabButton)
                    {
                        if(ctl == m_btnSelected)
                        {
                            bApplyData = (ctl as TabButton).Form.ApplyBeforeWeatherData();

                            if (bApplyData)
                            {
                                selectedButton = (ctl as TabButton);
                            }
                        }
                    }
                }

                // 기존에 선택하던 데이터가 없는경우,,
                // 삭제하였거나, 보고있던 재난 정보가 없던 경우에는 제일 최근에 보고된 재난 정보를 보여줌
                if (bApplyData == false)
                {
                    long nRainTime = -1;
                    long nTyphoonTime = -1;
                    long nEarthquakeTime = -1;

                    if (tabRain.WeatherDatas.Count != 0)
                    {
                        nRainTime = tabRain.WeatherDatas[0].Time.Ticks;
                    }
                    if (tabTyphoon.WeatherDatas.Count != 0)
                    {
                        nTyphoonTime = tabTyphoon.WeatherDatas[0].Time.Ticks;
                    }
                    if (tabEarthquake.WeatherDatas.Count != 0)
                    {
                        nEarthquakeTime = tabEarthquake.WeatherDatas[0].Time.Ticks;
                    }

                    if (nRainTime > nTyphoonTime)
                    {
                        if (nRainTime > nEarthquakeTime)
                        {
                            selectedButton = tabRain;
                        }
                        else
                        {
                            selectedButton = tabEarthquake;
                        }
                    }
                    else if (nTyphoonTime > nEarthquakeTime)
                    {
                        selectedButton = tabTyphoon;
                    }
                    else
                    {
                        selectedButton = tabEarthquake;
                    }

                }

                timerDisplay.Start();
            }
            else
            {
                tabRain.UpdateData();
                tabTyphoon.UpdateData();
                tabEarthquake.UpdateData();

                timerDisplay.Stop();
            }

            SetMode(selectedButton);
        }

        private bool LoadWeatherLog(DBUtility.WebDBManager dbMgr, int nID, ref DateTime dtCreate, ref int nAvailablePeriodDay)
        {
            string strSQL = "Select CreatedTime, AvailablePeriod FROM Weather_Log where ID = " + nID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count != 2)
                return false;

            DateTime dtNull = new DateTime();
            dtCreate = DBUtility.WebDBManager.GetDateTimeField(arrResult[0], dtNull);
            nAvailablePeriodDay = DBUtility.WebDBManager.GetIntField(arrResult[1].ToString(), -1);

            return true;
        }

        private void LoadWeatherList(DBUtility.WebDBManager dbMgr, int nLogID)
        {
            if (nLogID < 0)
                return;

            string strRainIDs = "", strTyphoonIDs = "", strEarthquakeIDs = "";

            if (!LoadWeatherDatas(dbMgr, nLogID.ToString(), ref strRainIDs, ref strTyphoonIDs, ref strEarthquakeIDs))
                return;

            LoadRain(dbMgr, strRainIDs);
            LoadTyphoon(dbMgr, strTyphoonIDs);
            LoadEarthquake(dbMgr, strEarthquakeIDs);
        }

        // DB 에서 가져온 호우경보 데이터 조합
        private void LoadRain(DBUtility.WebDBManager dbMgr, string strRainIDs)
        {
            string strSQL = "Select ID, Time, RainHour, RainDay, WindSpeedAve, WindSpeedMax, Region FROM Weather_RainNWind where ID in (" + strRainIDs + ") Order by Time DESC";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            DateTime dtNull = new DateTime();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 6; i += 7)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                DateTime dtTime = DBUtility.WebDBManager.GetDateTimeField(arrResult[i + 1], dtNull);
                float fRainHour = DBUtility.WebDBManager.GetFloatField(arrResult[i + 2].ToString(), -1.0f);
                float fRainDay = DBUtility.WebDBManager.GetFloatField(arrResult[i + 3].ToString(), -1.0f);
                float fSpeedAve = DBUtility.WebDBManager.GetFloatField(arrResult[i + 4].ToString(), -1.0f);
                float fSpeedMax = DBUtility.WebDBManager.GetFloatField(arrResult[i + 5].ToString(), -1.0f);
                string strRegion = DBUtility.WebDBManager.GetStringField(arrResult[i + 6], null);

                if (nID < 0)
                    continue;

                WeatherSimulator.RainNWind rain = new WeatherSimulator.RainNWind();

                rain.ID = nID;
                rain.Time = dtTime;

                if (fRainHour >= 0.0f)
                    rain.RainHour = new WeatherSimulator.VariousData<float>(fRainHour);

                if (fRainDay >= 0.0f)
                    rain.RainDay = new WeatherSimulator.VariousData<float>(fRainDay);

                if (fSpeedAve >= 0.0f)
                    rain.WindSpeedAve = new WeatherSimulator.VariousData<float>(fSpeedAve);

                if (fSpeedMax >= 0.0f)
                    rain.WindSpeedMax = new WeatherSimulator.VariousData<float>(fSpeedMax);

                if (strRegion != null && strRegion != "null")
                    rain.Region = strRegion;

                tabRain.WeatherDatas.Add(rain);
            }
        }

        // DB 에서 가져온 태풍경보 데이터 조합
        private void LoadTyphoon(DBUtility.WebDBManager dbMgr, string strTyphoonIDs)
        {
            string strSQL = "Select ID, Time, CenterLocation, CenterPressure, MaxSpeed, WindRadius, WindDirection, MoveSpeed, Etc FROM Weather_Typhoon where ID in (" + strTyphoonIDs + ") Order by Time DESC";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            DateTime dtNull = new DateTime();
            WeatherSimulator.Typhoon.Direction dir;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 8; i += 9)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                DateTime dtTime = DBUtility.WebDBManager.GetDateTimeField(arrResult[i + 1], dtNull);
                string strCenterLocation = DBUtility.WebDBManager.GetStringField(arrResult[i + 2].ToString(), null);
                float fCenterPressure = DBUtility.WebDBManager.GetFloatField(arrResult[i + 3].ToString(), -1.0f);
                float fMaxSpeed = DBUtility.WebDBManager.GetFloatField(arrResult[i + 4].ToString(), -1.0f);
                float fRadius = DBUtility.WebDBManager.GetFloatField(arrResult[i + 5].ToString(), -1.0f);
                int nDirection = DBUtility.WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                float fMoveSpeed = DBUtility.WebDBManager.GetFloatField(arrResult[i + 7].ToString(), -1.0f);
                string strEtc = DBUtility.WebDBManager.GetStringField(arrResult[i + 8], null);

                if (nID < 0)
                    continue;

                WeatherSimulator.Typhoon typhoon = new WeatherSimulator.Typhoon();

                typhoon.ID = nID;
                typhoon.Time = dtTime;

                if (strCenterLocation != null && strCenterLocation != "null")
                    typhoon.CenterLocation = strCenterLocation;

                if (fCenterPressure >= 0.0f)
                    typhoon.CenterPressure = new WeatherSimulator.VariousData<float>(fCenterPressure);

                if (fMaxSpeed >= 0.0f)
                    typhoon.MaxSpeed = new WeatherSimulator.VariousData<float>(fMaxSpeed);

                if (fRadius >= 0.0f)
                    typhoon.WindRadius = new WeatherSimulator.VariousData<float>(fRadius);

                if (WeatherSimulator.Typhoon.ToDirection(nDirection, out dir))
                    typhoon.WindDirection = new WeatherSimulator.VariousData<WeatherSimulator.Typhoon.Direction>(dir);

                if (fMoveSpeed >= 0.0f)
                    typhoon.MoveSpeed = new WeatherSimulator.VariousData<float>(fMoveSpeed);

                if (strEtc != null && strEtc != "null")
                    typhoon.Etc = strEtc;

                tabTyphoon.WeatherDatas.Add(typhoon);
            }
        }

        // DB 에서 가져온 지진경보 데이터 조합
        private void LoadEarthquake(DBUtility.WebDBManager dbMgr, string strEarthquakeIDs)
        {
            string strSQL = "Select ID, Time, Location, Strength, TsunamiHeight, Etc FROM Weather_Earthquake where ID in (" + strEarthquakeIDs + ") Order by Time DESC";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            DateTime dtNull = new DateTime();

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 5; i += 6)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                DateTime dtTime = DBUtility.WebDBManager.GetDateTimeField(arrResult[i + 1], dtNull);
                string strLocation = DBUtility.WebDBManager.GetStringField(arrResult[i + 2].ToString(), null);
                float fStrength = DBUtility.WebDBManager.GetFloatField(arrResult[i + 3].ToString(), -1.0f);
                float fTsunamiHeight = DBUtility.WebDBManager.GetFloatField(arrResult[i + 4].ToString(), -1.0f);
                string strEtc = DBUtility.WebDBManager.GetStringField(arrResult[i + 5].ToString(), null);

                if (nID < 0)
                    continue;

                WeatherSimulator.Earthquake earthquake = new WeatherSimulator.Earthquake();

                earthquake.ID = nID;
                earthquake.Time = dtTime;

                if (strLocation != null && strLocation != "null")
                    earthquake.Location = strLocation;

                if (fStrength >= 0.0f)
                    earthquake.Strength = new WeatherSimulator.VariousData<float>(fStrength);

                if (fTsunamiHeight >= 0.0f)
                    earthquake.TsunamiHeight = new WeatherSimulator.VariousData<float>(fTsunamiHeight);

                if (strEtc != null && strEtc != "null")
                    earthquake.Etc = strEtc;

                tabEarthquake.WeatherDatas.Add(earthquake);
            }
        }

        // 각 재난정보의 ID 값 조회 및 불러들인 재난정보의 유무 판단
        private bool LoadWeatherDatas(DBUtility.WebDBManager dbMgr, string strWeatherIDs, ref string strRainIDs, ref string strTyphoonIDs, ref string strEarthquakeIDs)
        {
            string strSQL = "Select DataID, DataType from Weather_List where WeatherID in (" + strWeatherIDs + ")";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            // 결과가 없을시,,,
            if (arrResult == null)
            {
                return false;
            }

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nDataID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nDataType = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);

                if (nDataType == (int)WeatherSimulator.WeatherData.DataType.RainNWind)
                {
                    if (strRainIDs.Length == 0)
                        strRainIDs = nDataID.ToString();
                    else
                        strRainIDs += ", " + nDataID.ToString();
                }
                else if (nDataType == (int)WeatherSimulator.WeatherData.DataType.Typhoon)
                {
                    if (strTyphoonIDs.Length == 0)
                        strTyphoonIDs = nDataID.ToString();
                    else
                        strTyphoonIDs += ", " + nDataID.ToString();
                }
                else if (nDataType == (int)WeatherSimulator.WeatherData.DataType.Earthquake)
                {
                    if (strEarthquakeIDs.Length == 0)
                        strEarthquakeIDs = nDataID.ToString();
                    else
                        strEarthquakeIDs += ", " + nDataID.ToString();
                }
            }

            return true;
        }

        private void btnTab_Click(object sender, EventArgs e)
        {
            if (sender == null)
                return;

            SetMode((TabButton)sender);
        }

        private void SetMode(TabButton btn)
        {
            if (btn == m_btnSelected)
                return;
            
            if (m_btnSelected != null)
                m_btnSelected.Selected = false;

            m_btnSelected = btn;
            btn.Selected = true;


        }

        // Return 값 : 유효기간이 경과한 Log Id들
        private int ReadLogID(DBUtility.WebDBManager dbMgr, int nSiteID)
        {
            int nLogID = -1;

            // 가장 나중의 것부터 읽기 위하여 시간 반대순서대로 Query를 작성한다.
            string strSQL = "Select ID, CreatedTime, AvailablePeriod from Weather_Log where SiteID = " + nSiteID.ToString() + " order by CreatedTime desc";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return nLogID;

            DateTime dtNull = new DateTime();
            DateTime dtNow = DateTime.Now;

            //string strRemoveIDs = "";

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                DateTime dtCreate = DBUtility.WebDBManager.GetDateTimeField(arrResult[i + 1], dtNull);
                int nAvailablePeriod = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);

                if (CheckPeriod(ref dtNow, dtCreate, nAvailablePeriod))
                {
                    if (nLogID < 0)
                        nLogID = nID;
                }
            }

            return nLogID;
        }

        // Return 값 : true이면 유효기간이 경과하지 않았다.
        //             false이면 유효기간이 지났다.
        private bool CheckPeriod(ref DateTime dtNow, DateTime dtCreate, int nAvailablePeriod)
        {
            if (nAvailablePeriod < 0)
                return true;

            TimeSpan span = dtNow - dtCreate;

            if (span.TotalDays >= nAvailablePeriod)
                return false;

            return true;
        }

        private void timerDisplay_Tick(object sender, EventArgs e)
        {
            m_frmRain.MoveNext();
            m_frmTyphoon.MoveNext();
            m_frmEarthquake.MoveNext();
        }
        
        // Timer 동작 시간을 초기화한다.
        public void ResetTimer()
        {
            timerDisplay.Stop();
            timerDisplay.Start();
        }

        public void SetStatus(string strStatus, Form frmSend)
        {
            if (m_btnSelected == null)
                return;

            if (m_btnSelected.Form != frmSend)
                return;

            if (strStatus == null || strStatus.Length == 0)
            {
                panelBottom.RealTimeInfo = "";
                panelBottom.StopTimer();
            }
            else
            {
                panelBottom.RealTimeInfo = strStatus;
                panelBottom.DrawMovingText();
            }
        }
    }

    public class PictureBoxArrow : System.Windows.Forms.PictureBox
    {
        private Image m_imgDisabled = null;
        private Image m_imgEnabled = null;

        public Image DisabledImage
        {
            get { return m_imgDisabled; }
            set { m_imgDisabled = value; }
        }

        public Image EnabledImage
        {
            get { return m_imgEnabled; }
            set { m_imgEnabled = value; }
        }

        public new bool Enabled
        {
            get { return base.Enabled; }
            set
            {
                base.Enabled = value;

                if (value)
                    this.Visible = true;
                else if (!value)
                {
                    this.Visible = false;

                    if (m_imgDisabled != null)
                        this.Image = m_imgDisabled;
                }
            }
        }

        public PictureBoxArrow()
        {
            this.MouseEnter += new System.EventHandler(this.OnMouseEnter);
            this.MouseLeave += new System.EventHandler(this.OnMouseLeave);
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            if (Enabled)
            {
                if (m_imgDisabled != null)
                    this.Image = m_imgDisabled;
            }
        }

        private void OnMouseEnter(object sender, EventArgs e)
        {
            if (Enabled)
            {
                if (m_imgEnabled != null)
                    this.Image = m_imgEnabled;
            }
        }
    }
}
