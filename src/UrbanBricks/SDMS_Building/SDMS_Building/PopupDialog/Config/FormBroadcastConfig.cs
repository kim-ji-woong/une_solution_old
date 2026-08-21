using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility2;
using UnE.GUI;
using UnE.Sensor;

namespace SDMS_Building.PopupDialog.Config
{
    public enum SituationType
    {
        Unknown = -1,
        DETECT_FIRE = 0,        // 화재 탐지
        REPORT_FIRE = 1,        // 화재 신고
        DETECT_PSM = 2,         // 누출 탐지
        REPORT_PSM = 3,         // 누출 신고
        DETECT_EARTHQUAKE = 4,  // 지진 탐지
        DETECT_SECURITY = 5,
        REPORT_SECURITY = 6,
        DETECT_TH = 7,
        REPORT_TH = 8,
        DETECT_ETC = 9,
        REPORT_ETC = 10
    }

    public partial class FormBroadcastConfig : Form
    {
        private UEWpfControl.WpfComboBox m_cbType = null;
        
        private Dictionary<SituationType, BroadcastConfig> m_dicBroadcastConfig = new Dictionary<SituationType, BroadcastConfig>();

        public FormBroadcastConfig()
        {
            InitializeComponent();

            m_cbType = new UEWpfControl.WpfComboBox();
            eleType.Child = m_cbType;
            m_cbType.SetSize(eleType.Width, eleType.Height);
            m_cbType.customComboBox.SelectionChanged += cbType_SelectionChanged;
        }

        private void FormBroadcastConfig_Load(object sender, EventArgs e)
        {
            LoadBroadcastConfig();
            SetBroadcastConfig();

            InitComboBox();            
        }

        private void InitComboBox()
        {
            m_cbType.customComboBox.DisplayMemberPath = "DisplayName";
            m_cbType.customComboBox.SelectedValuePath = "FacilityType";
            
            m_cbType.customComboBox.Items.Add(new FacilityTypeComboBoxItem(IFacility.FacilityType.FIRE_SENSOR, Data.CommonString.POI_Fire_Kor));

            if (UnE.SOP.ProxySOP.Instance.UsePSM)
                m_cbType.customComboBox.Items.Add(new FacilityTypeComboBoxItem(IFacility.FacilityType.PSM_SENSOR, Data.CommonString.POI_Gas_Kor));

            if (UnE.SOP.ProxySOP.Instance.UseIntrusion)
                m_cbType.customComboBox.Items.Add(new FacilityTypeComboBoxItem(IFacility.FacilityType.Security_Sensor, "방범"));

            if (UnE.SOP.ProxySOP.Instance.UseEarthquake)
                m_cbType.customComboBox.Items.Add(new FacilityTypeComboBoxItem(IFacility.FacilityType.Earthquake, Data.CommonString.POI_Earthquake_Kor));

            if (UnE.SOP.ProxySOP.Instance.UseTH)
                m_cbType.customComboBox.Items.Add(new FacilityTypeComboBoxItem(IFacility.FacilityType.TEMPERATURE_HUMIDITY, "온/습도"));

            // 그 외의 신호를 포함한다. FacilityType에 ETC가 없으므로 그냥 STRONG_WIND(강풍) 으로 함
            if (UnE.SOP.ProxySOP.Instance.UseStrongWind || UnE.SOP.ProxySOP.Instance.UseBlackout || UnE.SOP.ProxySOP.Instance.UseTerror)
                m_cbType.customComboBox.Items.Add(new FacilityTypeComboBoxItem(IFacility.FacilityType.STRONG_WIND, "기타"));

            m_cbType.customComboBox.SelectedIndex = 0;
        }

        private void LoadBroadcastConfig()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Select SituationType, UseBroadcast, Message, UseSiren, RepeatCount ");
            sb.Append("  From SDMSBroadcastConfig ");
            sb.AppendFormat(" Where SiteID = {0} ", UnE.SOP.ProxySOP.Instance.SiteID);

            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(sb.ToString());
            if (arrResult != null && arrResult.Count > 0)
            {
                for (int i = 0; i < arrResult.Count; i += 5)
                {
                    int nSituationType = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    bool bUseBroadcast = WebDBManager.GetIntField(arrResult[i + 1].ToString(), 0) == 0 ? false : true;
                    string strMessage = WebDBManager.GetStringField(arrResult[i + 2], "");
                    bool bUseSiren = WebDBManager.GetIntField(arrResult[i + 3].ToString(), 0) == 0 ? false : true;
                    int nRepeatCount = WebDBManager.GetIntField(arrResult[i + 4].ToString(), 0);

                    SituationType type;
                    if (Enum.TryParse(nSituationType.ToString(), out type))
                    {
                        BroadcastConfig cfg = new BroadcastConfig();
                        cfg.NewLine = false;
                        cfg.SituationType = type;
                        cfg.UseBroadcast = bUseBroadcast;
                        cfg.Message = strMessage;
                        cfg.UseSiren = bUseSiren;
                        cfg.RepeatCount = nRepeatCount;
                        
                        m_dicBroadcastConfig[type] = cfg;
                    }
                }
            }
        }

        private void SetBroadcastConfig()
        {
            FacilityTypeComboBoxItem selectedItem = m_cbType.customComboBox.SelectedItem as FacilityTypeComboBoxItem;
            if (selectedItem == null)
                return;

            SituationType detectType = SituationType.Unknown;
            SituationType reportType = SituationType.Unknown;

            if (selectedItem.FacilityType == IFacility.FacilityType.FIRE_SENSOR)
            {
                if (!m_dicBroadcastConfig.ContainsKey(SituationType.DETECT_FIRE))
                    m_dicBroadcastConfig[SituationType.DETECT_FIRE] = AddConfig(SituationType.DETECT_FIRE, true);
                if (!m_dicBroadcastConfig.ContainsKey(SituationType.REPORT_FIRE))
                    m_dicBroadcastConfig[SituationType.REPORT_FIRE] = AddConfig(SituationType.REPORT_FIRE, false);

                detectType = SituationType.DETECT_FIRE;
                reportType = SituationType.REPORT_FIRE;
            }
            else if (selectedItem.FacilityType == IFacility.FacilityType.PSM_SENSOR)
            {
                if (!m_dicBroadcastConfig.ContainsKey(SituationType.DETECT_PSM))
                    m_dicBroadcastConfig[SituationType.DETECT_PSM] = AddConfig(SituationType.DETECT_PSM, true);
                if (!m_dicBroadcastConfig.ContainsKey(SituationType.REPORT_PSM))
                    m_dicBroadcastConfig[SituationType.REPORT_PSM] = AddConfig(SituationType.REPORT_PSM, false);

                detectType = SituationType.DETECT_PSM;
                reportType = SituationType.REPORT_PSM;
            }
            else if (selectedItem.FacilityType == IFacility.FacilityType.Security_Sensor)
            {
                if (!m_dicBroadcastConfig.ContainsKey(SituationType.DETECT_SECURITY))
                    m_dicBroadcastConfig[SituationType.DETECT_SECURITY] = AddConfig(SituationType.DETECT_SECURITY, true);
                if (!m_dicBroadcastConfig.ContainsKey(SituationType.REPORT_SECURITY))
                    m_dicBroadcastConfig[SituationType.REPORT_SECURITY] = AddConfig(SituationType.REPORT_SECURITY, false);

                detectType = SituationType.DETECT_SECURITY;
                reportType = SituationType.REPORT_SECURITY;
            }
            else if (selectedItem.FacilityType == IFacility.FacilityType.Earthquake)
            {
                if (!m_dicBroadcastConfig.ContainsKey(SituationType.DETECT_EARTHQUAKE))
                    m_dicBroadcastConfig[SituationType.DETECT_EARTHQUAKE] = AddConfig(SituationType.DETECT_EARTHQUAKE, true);

                detectType = SituationType.DETECT_EARTHQUAKE;
                reportType = SituationType.Unknown;
            }
            else if (selectedItem.FacilityType == IFacility.FacilityType.TEMPERATURE_HUMIDITY)
            {
                if (!m_dicBroadcastConfig.ContainsKey(SituationType.DETECT_TH))
                    m_dicBroadcastConfig[SituationType.DETECT_TH] = AddConfig(SituationType.DETECT_TH, true);
                if (!m_dicBroadcastConfig.ContainsKey(SituationType.REPORT_TH))
                    m_dicBroadcastConfig[SituationType.REPORT_TH] = AddConfig(SituationType.REPORT_TH, false);

                detectType = SituationType.DETECT_TH;
                reportType = SituationType.REPORT_TH;
            }
            else if (selectedItem.FacilityType == IFacility.FacilityType.STRONG_WIND)
            {
                if (!m_dicBroadcastConfig.ContainsKey(SituationType.DETECT_ETC))
                    m_dicBroadcastConfig[SituationType.DETECT_ETC] = AddConfig(SituationType.DETECT_ETC, true);
                if (!m_dicBroadcastConfig.ContainsKey(SituationType.REPORT_ETC))
                    m_dicBroadcastConfig[SituationType.REPORT_ETC] = AddConfig(SituationType.REPORT_ETC, false);

                detectType = SituationType.DETECT_ETC;
                reportType = SituationType.REPORT_ETC;
            }

            if (m_dicBroadcastConfig.ContainsKey(detectType))
            {
                BroadcastConfig detectCfg = m_dicBroadcastConfig[detectType];
                if (detectCfg != null)
                {
                    chkUseDetect.IsChecked = detectCfg.UseBroadcast;
                    txtDetect.Text = detectCfg.Message;
                    chkUseSiren.IsChecked = detectCfg.UseSiren;
                    if (detectCfg.RepeatCount == 1)
                    {
                        radioNoRepeat.IsChecked = false;
                        radioRepeatOnce.IsChecked = true;
                        radioRepeatTwice.IsChecked = false;
                    }
                    else if (detectCfg.RepeatCount == 2)
                    {
                        radioNoRepeat.IsChecked = false;
                        radioRepeatOnce.IsChecked = false;
                        radioRepeatTwice.IsChecked = true;
                    }
                    else
                    {
                        radioNoRepeat.IsChecked = true;
                        radioRepeatOnce.IsChecked = false;
                        radioRepeatTwice.IsChecked = false;
                    }
                } 
            }

            if (m_dicBroadcastConfig.ContainsKey(reportType))
            {
                BroadcastConfig reportCfg = m_dicBroadcastConfig[reportType];
                if (reportCfg != null)
                {
                    chkUseReport.IsChecked = reportCfg.UseBroadcast;
                    txtReport.Text = reportCfg.Message;
                    chkUseSiren.IsChecked = reportCfg.UseSiren;
                    if (reportCfg.RepeatCount == 1)
                    {
                        radioNoRepeat.IsChecked = false;
                        radioRepeatOnce.IsChecked = true;
                        radioRepeatTwice.IsChecked = false;
                    }
                    else if (reportCfg.RepeatCount == 2)
                    {
                        radioNoRepeat.IsChecked = false;
                        radioRepeatOnce.IsChecked = false;
                        radioRepeatTwice.IsChecked = true;
                    }
                    else
                    {
                        radioNoRepeat.IsChecked = true;
                        radioRepeatOnce.IsChecked = false;
                        radioRepeatTwice.IsChecked = false;
                    }
                } 
            }

            radioNoRepeat.Refresh();
            radioRepeatOnce.Refresh();
            radioRepeatTwice.Refresh();
        }

        private BroadcastConfig AddConfig(SituationType type, bool isDetect)
        {
            BroadcastConfig cfg = new BroadcastConfig();
            cfg.NewLine = true;
            cfg.SituationType = type;
            if (isDetect)
                cfg.UseBroadcast = chkUseDetect.IsChecked;
            else
                cfg.UseBroadcast = chkUseReport.IsChecked;
            cfg.Message = "";
            cfg.UseSiren = chkUseSiren.IsChecked;
            cfg.RepeatCount = 0;

            return cfg;
        }

        private void cbType_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            SetBroadcastConfig();
        }

        private void chkUseSiren_Click(object sender, EventArgs e)
        {
            chkUseSiren.IsChecked = !chkUseSiren.IsChecked;

            FacilityTypeComboBoxItem selectedItem = m_cbType.customComboBox.SelectedItem as FacilityTypeComboBoxItem;
            if (selectedItem == null)
                return;

            if (selectedItem.FacilityType == IFacility.FacilityType.FIRE_SENSOR)
            {
                m_dicBroadcastConfig[SituationType.DETECT_FIRE].UseSiren = chkUseSiren.IsChecked;
                m_dicBroadcastConfig[SituationType.REPORT_FIRE].UseSiren = chkUseSiren.IsChecked;
            }
            else if (selectedItem.FacilityType == IFacility.FacilityType.PSM_SENSOR)
            {
                m_dicBroadcastConfig[SituationType.DETECT_PSM].UseSiren = chkUseSiren.IsChecked;
                m_dicBroadcastConfig[SituationType.REPORT_PSM].UseSiren = chkUseSiren.IsChecked;
            }
            else if (selectedItem.FacilityType == IFacility.FacilityType.Security_Sensor)
            {
                m_dicBroadcastConfig[SituationType.DETECT_SECURITY].UseSiren = chkUseSiren.IsChecked;
                m_dicBroadcastConfig[SituationType.REPORT_SECURITY].UseSiren = chkUseSiren.IsChecked;
            }
            else if (selectedItem.FacilityType == IFacility.FacilityType.Earthquake)
            {
                m_dicBroadcastConfig[SituationType.DETECT_EARTHQUAKE].UseSiren = chkUseSiren.IsChecked;
            }
            else if (selectedItem.FacilityType == IFacility.FacilityType.TEMPERATURE_HUMIDITY)
            {
                m_dicBroadcastConfig[SituationType.DETECT_TH].UseSiren = chkUseSiren.IsChecked;
                m_dicBroadcastConfig[SituationType.REPORT_TH].UseSiren = chkUseSiren.IsChecked;
            }
            else if (selectedItem.FacilityType == IFacility.FacilityType.STRONG_WIND)
            {
                m_dicBroadcastConfig[SituationType.DETECT_ETC].UseSiren = chkUseSiren.IsChecked;
                m_dicBroadcastConfig[SituationType.REPORT_ETC].UseSiren = chkUseSiren.IsChecked;
            }
        }

        private void radioRepeat_Click(object sender, EventArgs e)
        {
            RibbonButton rbtn = sender as RibbonButton;
            if (rbtn == null)
                return;

            if (rbtn == radioNoRepeat && radioNoRepeat.IsChecked) 
                return;
            if (rbtn == radioRepeatOnce && radioRepeatOnce.IsChecked)
                return;
            if (rbtn == radioRepeatTwice && radioRepeatTwice.IsChecked)
                return;

            int nReportCount = 0;
            if (rbtn == radioRepeatOnce)
            {
                nReportCount = 1;

                radioNoRepeat.IsChecked = false;
                radioRepeatOnce.IsChecked = true;
                radioRepeatTwice.IsChecked = false;
            }
            else if (rbtn == radioRepeatTwice)
            {
                nReportCount = 2;

                radioNoRepeat.IsChecked = false;
                radioRepeatOnce.IsChecked = false;
                radioRepeatTwice.IsChecked = true;
            }
            else
            {
                nReportCount = 0;

                radioNoRepeat.IsChecked = true;
                radioRepeatOnce.IsChecked = false;
                radioRepeatTwice.IsChecked = false;
            }

            radioNoRepeat.Refresh();
            radioRepeatOnce.Refresh();
            radioRepeatTwice.Refresh();

            FacilityTypeComboBoxItem selectedItem = m_cbType.customComboBox.SelectedItem as FacilityTypeComboBoxItem;
            if (selectedItem == null)
                return;

            if (selectedItem.FacilityType == IFacility.FacilityType.FIRE_SENSOR)
            {
                m_dicBroadcastConfig[SituationType.DETECT_FIRE].RepeatCount = nReportCount;
                m_dicBroadcastConfig[SituationType.REPORT_FIRE].RepeatCount = nReportCount;
            }
            else if (selectedItem.FacilityType == IFacility.FacilityType.PSM_SENSOR)
            {
                m_dicBroadcastConfig[SituationType.DETECT_PSM].RepeatCount = nReportCount;
                m_dicBroadcastConfig[SituationType.REPORT_PSM].RepeatCount = nReportCount;
            }
            else if (selectedItem.FacilityType == IFacility.FacilityType.Security_Sensor)
            {
                m_dicBroadcastConfig[SituationType.DETECT_SECURITY].RepeatCount = nReportCount;
                m_dicBroadcastConfig[SituationType.REPORT_SECURITY].RepeatCount = nReportCount;
            }
            else if (selectedItem.FacilityType == IFacility.FacilityType.Earthquake)
            {
                m_dicBroadcastConfig[SituationType.DETECT_EARTHQUAKE].RepeatCount = nReportCount;
            }
            else if (selectedItem.FacilityType == IFacility.FacilityType.TEMPERATURE_HUMIDITY)
            {
                m_dicBroadcastConfig[SituationType.DETECT_TH].RepeatCount = nReportCount;
                m_dicBroadcastConfig[SituationType.REPORT_TH].RepeatCount = nReportCount;
            }
            else if (selectedItem.FacilityType == IFacility.FacilityType.STRONG_WIND)
            {
                m_dicBroadcastConfig[SituationType.DETECT_ETC].RepeatCount = nReportCount;
                m_dicBroadcastConfig[SituationType.REPORT_ETC].RepeatCount = nReportCount;
            }
        }
        
        private void chkUseDetect_Click(object sender, EventArgs e)
        {
            chkUseDetect.IsChecked = !chkUseDetect.IsChecked;

            FacilityTypeComboBoxItem selectedItem = m_cbType.customComboBox.SelectedItem as FacilityTypeComboBoxItem;
            if (selectedItem == null)
                return;

            if (selectedItem.FacilityType == IFacility.FacilityType.FIRE_SENSOR)
            {
                m_dicBroadcastConfig[SituationType.DETECT_FIRE].UseBroadcast = chkUseDetect.IsChecked;
            }
            else if (selectedItem.FacilityType == IFacility.FacilityType.PSM_SENSOR)
            {
                m_dicBroadcastConfig[SituationType.DETECT_PSM].UseBroadcast = chkUseDetect.IsChecked;
            }
            else if (selectedItem.FacilityType == IFacility.FacilityType.Security_Sensor)
            {
                m_dicBroadcastConfig[SituationType.DETECT_SECURITY].UseBroadcast = chkUseDetect.IsChecked;
            }
            else if (selectedItem.FacilityType == IFacility.FacilityType.Earthquake)
            {
                m_dicBroadcastConfig[SituationType.DETECT_EARTHQUAKE].UseBroadcast = chkUseDetect.IsChecked;
            }
            else if (selectedItem.FacilityType == IFacility.FacilityType.TEMPERATURE_HUMIDITY)
            {
                m_dicBroadcastConfig[SituationType.DETECT_TH].UseBroadcast = chkUseDetect.IsChecked;
            }
            else if (selectedItem.FacilityType == IFacility.FacilityType.STRONG_WIND)
            {
                m_dicBroadcastConfig[SituationType.DETECT_ETC].UseBroadcast = chkUseDetect.IsChecked;
            }
        }

        private void chkUseReport_Click(object sender, EventArgs e)
        {
            chkUseReport.IsChecked = !chkUseReport.IsChecked;

            FacilityTypeComboBoxItem selectedItem = m_cbType.customComboBox.SelectedItem as FacilityTypeComboBoxItem;
            if (selectedItem == null)
                return;

            if (selectedItem.FacilityType == IFacility.FacilityType.FIRE_SENSOR)
            {
                m_dicBroadcastConfig[SituationType.REPORT_FIRE].UseBroadcast = chkUseReport.IsChecked;
            }
            else if (selectedItem.FacilityType == IFacility.FacilityType.PSM_SENSOR)
            {
                m_dicBroadcastConfig[SituationType.REPORT_PSM].UseBroadcast = chkUseReport.IsChecked;
            }
            else if (selectedItem.FacilityType == IFacility.FacilityType.Security_Sensor)
            {
                m_dicBroadcastConfig[SituationType.REPORT_SECURITY].UseBroadcast = chkUseReport.IsChecked;
            }
            else if (selectedItem.FacilityType == IFacility.FacilityType.TEMPERATURE_HUMIDITY)
            {
                m_dicBroadcastConfig[SituationType.REPORT_TH].UseBroadcast = chkUseReport.IsChecked;
            }
            else if (selectedItem.FacilityType == IFacility.FacilityType.STRONG_WIND)
            {
                m_dicBroadcastConfig[SituationType.REPORT_ETC].UseBroadcast = chkUseReport.IsChecked;
            }
        }

        private void txtDetect_TextChanged(object sender, EventArgs e)
        {
            FacilityTypeComboBoxItem selectedItem = m_cbType.customComboBox.SelectedItem as FacilityTypeComboBoxItem;
            if (selectedItem == null)
                return;

            string msg = txtDetect.Text;

            if (selectedItem.FacilityType == IFacility.FacilityType.FIRE_SENSOR)
            {
                m_dicBroadcastConfig[SituationType.DETECT_FIRE].Message = msg;
            }
            else if (selectedItem.FacilityType == IFacility.FacilityType.PSM_SENSOR)
            {
                m_dicBroadcastConfig[SituationType.DETECT_PSM].Message = msg;
            }
            else if (selectedItem.FacilityType == IFacility.FacilityType.Security_Sensor)
            {
                m_dicBroadcastConfig[SituationType.DETECT_SECURITY].Message = msg;
            }
            else if (selectedItem.FacilityType == IFacility.FacilityType.Earthquake)
            {
                m_dicBroadcastConfig[SituationType.DETECT_EARTHQUAKE].Message = msg;
            }
            else if (selectedItem.FacilityType == IFacility.FacilityType.TEMPERATURE_HUMIDITY)
            {
                m_dicBroadcastConfig[SituationType.DETECT_TH].Message = msg;
            }
            else if (selectedItem.FacilityType == IFacility.FacilityType.STRONG_WIND)
            {
                m_dicBroadcastConfig[SituationType.DETECT_ETC].Message = msg;
            }
        }

        private void txtReport_TextChanged(object sender, EventArgs e)
        {
            FacilityTypeComboBoxItem selectedItem = m_cbType.customComboBox.SelectedItem as FacilityTypeComboBoxItem;
            if (selectedItem == null)
                return;

            string msg = txtReport.Text;

            if (selectedItem.FacilityType == IFacility.FacilityType.FIRE_SENSOR)
            {
                m_dicBroadcastConfig[SituationType.REPORT_FIRE].Message = msg;
            }
            else if (selectedItem.FacilityType == IFacility.FacilityType.PSM_SENSOR)
            {
                m_dicBroadcastConfig[SituationType.REPORT_PSM].Message = msg;
            }
            else if (selectedItem.FacilityType == IFacility.FacilityType.Security_Sensor)
            {
                m_dicBroadcastConfig[SituationType.REPORT_SECURITY].Message = msg;
            }
            else if (selectedItem.FacilityType == IFacility.FacilityType.TEMPERATURE_HUMIDITY)
            {
                m_dicBroadcastConfig[SituationType.REPORT_TH].Message = msg;
            }
            else if (selectedItem.FacilityType == IFacility.FacilityType.STRONG_WIND)
            {
                m_dicBroadcastConfig[SituationType.REPORT_ETC].Message = msg;
            }
        }

        public void Save()
        {
            foreach (KeyValuePair<SituationType, BroadcastConfig> item in m_dicBroadcastConfig)
            {
                BroadcastConfig cfg = item.Value;

                StringBuilder sb = new StringBuilder();

                if (cfg.NewLine)
                {
                    sb.Append("INSERT INTO SDMSBroadcastConfig (ID, SituationType, UseBroadcast, Message, UseSiren, RepeatCount, SiteID) ");
                    sb.AppendFormat("VALUES ((Select ISNULL(Max(ID) + 1, 1) FROM SDMSBroadcastConfig),{0},{1},'{2}',{3},{4},{5}) "
                        , (int)cfg.SituationType, (cfg.UseBroadcast) ? 1 : 0, cfg.Message, (cfg.UseSiren) ? 1 : 0, cfg.RepeatCount, UnE.SOP.ProxySOP.Instance.SiteID);
                }
                else
                {
                    sb.AppendFormat("UPDATE SDMSBroadcastConfig SET UseBroadcast={0}, Message='{1}', UseSiren={2}, RepeatCount={3} WHERE SituationType={4}"
                        , (cfg.UseBroadcast) ? 1 : 0, cfg.Message, (cfg.UseSiren) ? 1 : 0, cfg.RepeatCount, (int)cfg.SituationType);
                }

                FormMain.Instance.DBManager.GetResultData(sb.ToString());
            }
        }
    }

    public class BroadcastConfig
    {
        private SituationType m_situationType = SituationType.Unknown;
        public SituationType SituationType
        {
            get { return m_situationType; }
            set { m_situationType = value; }
        }

        private bool m_bUseBroadcast = false;
        public bool UseBroadcast
        {
            get { return m_bUseBroadcast; }
            set { m_bUseBroadcast = value; }
        }

        private string m_strMessage = "";
        public string Message
        {
            get { return m_strMessage; }
            set { m_strMessage = value; }
        }

        private bool m_bUseSiren = false;
        public bool UseSiren
        {
            get { return m_bUseSiren; }
            set { m_bUseSiren = value; }
        }

        private int m_nRepeatCount = 0;
        public int RepeatCount
        {
            get { return m_nRepeatCount; }
            set { m_nRepeatCount = value; }
        }

        private bool m_bNewLine = false;
        /// <summary>
        /// DB에 데이터가 없어서 새로 만든 항목인가?
        /// 새로 만든 항목은 저장할때 Insert문을 할거야
        /// </summary>
        public bool NewLine
        {
            get { return m_bNewLine; }
            set { m_bNewLine = value; }
        }
    }
}
