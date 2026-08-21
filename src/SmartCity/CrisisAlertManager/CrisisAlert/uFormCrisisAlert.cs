using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CrisisAlertManager.Data;
using CrisisAlertManager.Popup_Dialog.Message;
using UnE.GUI;
using CrisisAlertManager.Popup_Dialog.Alarm;
using libSensorProcess;
using CrisisAlertManager.Popup_Dialog;

namespace CrisisAlertManager.CrisisAlert
{
    public partial class uFormCrisisAlert : UserControl
    {
        private Image m_imgRisk_Normal = global::CrisisAlertManager.Properties.Resources.Normal_new;
        private Image m_imgRisk_Attention = global::CrisisAlertManager.Properties.Resources.Attention_new;
        private Image m_imgRisk_Caution = global::CrisisAlertManager.Properties.Resources.Caution_new;
        private Image m_imgRisk_Alert = global::CrisisAlertManager.Properties.Resources.Alert_new;
        private Image m_imgRisk_Serious = global::CrisisAlertManager.Properties.Resources.Serious_new;

        //private UEWpfControl.WpfRoundComboBox m_cbFireSensor = null;
        //private UEWpfControl.WpfRoundComboBox m_cbFloodSensor = null;
        //private UEWpfControl.WpfRoundComboBox m_cbHeatSensor = null;
        //private UEWpfControl.WpfRoundComboBox m_cbCollapseSensor = null;

        private uFormFireSensorInfo m_uFormFireSensorInfo = null;
        private uFormFloodSensorInfo m_uFormFloodSensorInfo = null;
        private uFormHeatSensorInfo m_uFormHeatSensorInfo = null;
        private uFormCollapseSensorInfo m_uformCollapseSensorInfo = null;

        private uFormSpread m_uformSpread = null;

        private bool m_bFireModifityCheck = false;
        private bool m_bPanelFireModifitySizeFull = false;
        private Timer m_timerFireModifity = null;

        private bool m_bFloodModifityCheck = false;
        private bool m_bPanelFloodModifitySizeFull = false;
        private Timer m_timerFloodModifity = null;

        private bool m_bHeatModifityCheck = false;
        private bool m_bPanelHeatModifitySizeFull = false;
        private Timer m_timerHeatModifity = null;

        private bool m_bCollapseModifityCheck = false;
        private bool m_bPanelCollapseModifitySizeFull = false;
        private Timer m_timerCollapseModifity = null;

        private FireSensor m_selectFireSensor;
        public FireSensor SelectFireSensor
        {
            set { m_selectFireSensor = value; }
        }

        private FloodSensor m_selectFloodSensor;
        public FloodSensor SelectFloodSensor
        {
            set { m_selectFloodSensor = value; }
        }

        private HeatSensor m_selectHeatSensor;
        public HeatSensor SelectHeatSensor
        {
            set { m_selectHeatSensor = value; }
        }

        private CollapseSensor m_selectCollapseSensor;
        public CollapseSensor SelectCollapseSensor 
        { 
            set { m_selectCollapseSensor = value; }
        }

        private FormAlertAlarm m_formAlertAlarm = null;

        // 사용자가 임의로 위기경보 레벨 수정 데이터
        private string m_strFireLevel = CommonString.RiskLevel_Normal;
        private string m_strFloodLevel = CommonString.RiskLevel_Normal;
        private string m_strHeatLevel = CommonString.RiskLevel_Normal;
        private string m_strCollapseLevel = CommonString.RiskLevel_Normal;

        private Timer m_timerSensorReload = null;

        private static uFormCrisisAlert m_instance = null;
        public static uFormCrisisAlert Instance
        {
            get { return m_instance; }
        }

        public uFormCrisisAlert()
        {
            InitializeComponent();

            this.DoubleBuffered = true;

            //InitSensorComboBax();
            InitLoadSensor();
            InitPosition();
            InitTimer();

            m_instance = this;
        }

        private void InitPosition()
        {
            plFireModifity.Size = new Size(plFireModifity.Width, 0);
            plFireModifity.Location = new Point(btnFireModifity.Location.X, btnFireModifity.Location.Y + btnFireModifity.Size.Height + 5);

            plFloodModifity.Size = new Size(plFloodModifity.Width, 0);
            plFloodModifity.Location = new Point(btnFloodModifity.Location.X, btnFloodModifity.Location.Y + btnFloodModifity.Size.Height + 5);

            plHeatModifity.Size = new Size(plHeatModifity.Width, 0);
            plHeatModifity.Location = new Point(btnHeatModifity.Location.X, btnHeatModifity.Location.Y + btnHeatModifity.Size.Height + 5);

            plCollapseModifitya.Size = new Size(plCollapseModifitya.Width, 0);
            plCollapseModifitya.Location = new Point(btnCollapseModifity.Location.X, btnCollapseModifity.Location.Y + btnCollapseModifity.Size.Height + 5);
        }

        private void InitTimer()
        {
            // 위기경보 단계 조정 창 조절 타이머
            m_timerFireModifity = new Timer();
            m_timerFireModifity.Interval = 10;
            m_timerFireModifity.Tick += M_timerFireModifity_Tick;

            m_timerFloodModifity = new Timer();
            m_timerFloodModifity.Interval = 10;
            m_timerFloodModifity.Tick += M_timerFloodModifity_Tick;

            m_timerHeatModifity = new Timer();
            m_timerHeatModifity.Interval = 10;
            m_timerHeatModifity.Tick += M_timerHeatModifity_Tick;

            m_timerCollapseModifity = new Timer();
            m_timerCollapseModifity.Interval = 10;
            m_timerCollapseModifity.Tick += M_timerCollapseModifity_Tick;


            // 센서 정보 주기적으로 읽어오는 타이머
            m_timerSensorReload = new Timer();
            m_timerSensorReload.Interval = 1000;
            m_timerSensorReload.Tick += M_timerSensorReload_Tick;
            m_timerSensorReload.Enabled = true;
        }

        private void InitSensorComboBax()
        {
            //m_cbFireSensor = new UEWpfControl.WpfRoundComboBox();
            //eleFireSensor.Child = m_cbFireSensor;
            //m_cbFireSensor.customComboBox.DropDownOpened += EleFireSensorComboBox_DropDownOpened;
            //m_cbFireSensor.customComboBox.SelectionChanged += EleFireSensorComboBox_SelectionChanged;
            //m_cbFireSensor.SetSize(eleFireSensor.Width, eleFireSensor.Height);
            //m_cbFireSensor.customComboBox.DisplayMemberPath = "Addr";

            //m_cbFloodSensor = new UEWpfControl.WpfRoundComboBox();
            //eleFloodSensor.Child = m_cbFloodSensor;
            //m_cbFloodSensor.customComboBox.DropDownOpened += EleFloodSensorComboBox_DropDownOpened;
            //m_cbFloodSensor.customComboBox.SelectionChanged += EleFloodSensorComboBox_SelectionChanged;
            //m_cbFloodSensor.SetSize(eleFloodSensor.Width, eleFloodSensor.Height);
            //m_cbFloodSensor.customComboBox.DisplayMemberPath = "Addr";

            //m_cbHeatSensor = new UEWpfControl.WpfRoundComboBox();
            //eleHeatSensor.Child = m_cbHeatSensor;
            //m_cbHeatSensor.customComboBox.DropDownOpened += EleHeatSensorComboBox_DropDownOpened;
            //m_cbHeatSensor.customComboBox.SelectionChanged += EleHeatSensorComboBox_SelectionChanged;
            //m_cbHeatSensor.SetSize(eleHeatSensor.Width, eleHeatSensor.Height);
            //m_cbHeatSensor.customComboBox.DisplayMemberPath = "Addr";

            //m_cbCollapseSensor = new UEWpfControl.WpfRoundComboBox();
            //eleCollapseSensor.Child = m_cbCollapseSensor;
            //m_cbCollapseSensor.customComboBox.DropDownOpened += EleCollapseSensorComboBox_DropDownOpened;
            //m_cbCollapseSensor.customComboBox.SelectionChanged += EleCollapseSensorComboBox_SelectionChanged;
            //m_cbCollapseSensor.SetSize(eleCollapseSensor.Width, eleCollapseSensor.Height);
            //m_cbCollapseSensor.customComboBox.DisplayMemberPath = "Addr";

            //ReloadFireSensor();
            //ReloadFloodSensor();
            //ReloadHeatSensor();
            //ReloadCollapseSensor();

            //if (m_cbFireSensor.customComboBox.Items.Count > 0)
            //    m_cbFireSensor.customComboBox.SelectedIndex = 0;

            //if (m_cbFloodSensor.customComboBox.Items.Count > 0)
            //    m_cbFloodSensor.customComboBox.SelectedIndex = 0;

            //if (m_cbHeatSensor.customComboBox.Items.Count > 0)
            //    m_cbHeatSensor.customComboBox.SelectedIndex = 0;

            //if (m_cbCollapseSensor.customComboBox.Items.Count > 0)
            //    m_cbCollapseSensor.customComboBox.SelectedIndex = 0;

        }

        private void InitLoadSensor()
        {
            Dictionary<int, FireSensor> dicFireSensors = FormMain.Instance.DataManager.DicFireSensors;
            foreach (KeyValuePair<int, FireSensor> pair in dicFireSensors)
            {
                m_selectFireSensor = pair.Value;
                ShowFireSensorState();
                break;
            }

            Dictionary<int, FloodSensor> dicFloodSensors = FormMain.Instance.DataManager.DicFloodSensors;
            foreach (KeyValuePair<int, FloodSensor> pair in dicFloodSensors)
            {
                m_selectFloodSensor = pair.Value;
                ShowFloodSensorState();
                break;
            }

            Dictionary<int, HeatSensor> dicHeatSensors = FormMain.Instance.DataManager.DicHeatSensors;
            foreach (KeyValuePair<int, HeatSensor> pair in dicHeatSensors)
            {
                m_selectHeatSensor = pair.Value;
                ShowHeatSensorState();
                break;
            }

            Dictionary<int, CollapseSensor> dicCollapseSensors = FormMain.Instance.DataManager.DicCollapseSensors;
            foreach (KeyValuePair<int, CollapseSensor> pair in dicCollapseSensors)
            {
                m_selectCollapseSensor = pair.Value;
                ShowCollapseSensorState();
                break;
            }
        }

        //private void EleFireSensorComboBox_DropDownOpened(object sender, EventArgs e)
        //{
        //    ReloadFireSensor();

        //    if (m_selectFireSensor != null)
        //    {
        //        m_cbFireSensor.customComboBox.SelectedItem = m_selectFireSensor;

        //        //FireSensor fireSensor = FormMain.Instance.DataManager.DicFireSensors[m_selectFireSensor.ID];
        //        //m_selectFireSensor = fireSensor;
        //        //ShowFireSensorState();
        //        ReloadFireSensorState();
        //        m_cbFireSensor.customComboBox.SelectedItem = m_selectFireSensor;
        //    }
        //}

        public void ReloadFireSensorState()
        {
            if (m_selectFireSensor == null)
                return;

            FireSensor fireSensor = FormMain.Instance.DataManager.DicFireSensors[m_selectFireSensor.ID];
            m_selectFireSensor = fireSensor;
            ShowFireSensorState();
        }

        //private void EleFloodSensorComboBox_DropDownOpened(object sender, EventArgs e)
        //{
        //    // TODO: 너무 많은 센서 양으로 딜레이로 인한 임시 주석처리 
        //    //ReloadFloodSensor();

        //    if (m_selectFloodSensor != null)
        //    {
        //        m_cbFloodSensor.customComboBox.SelectedItem = m_selectFloodSensor;

        //        //FloodSensor floodSensor = FormMain.Instance.DataManager.DicFloodSensors[m_selectFloodSensor.ID];
        //        //m_selectFloodSensor = floodSensor;
        //        //ShowFloodSensorState();
        //        ReloadFloodSensorState();
        //        m_cbFloodSensor.customComboBox.SelectedItem = m_selectFloodSensor;
        //    }
        //}

        public void ReloadFloodSensorState()
        {
            if (m_selectFloodSensor == null)
                return;

            FloodSensor floodSensor = FormMain.Instance.DataManager.DicFloodSensors[m_selectFloodSensor.ID];
            m_selectFloodSensor = floodSensor;
            ShowFloodSensorState();
        }

        //private void EleHeatSensorComboBox_DropDownOpened(object sender, EventArgs e)
        //{
        //    ReloadHeatSensor();

        //    if (m_selectHeatSensor != null)
        //    {
        //        m_cbHeatSensor.customComboBox.SelectedItem = m_selectHeatSensor;

        //        //HeatSensor heatSensor = FormMain.Instance.DataManager.DicHeatSensors[m_selectHeatSensor.ID];
        //        //m_selectHeatSensor = heatSensor;
        //        //ShowHeatSensorState();
        //        ReloadHeatSensorState();
        //        m_cbHeatSensor.customComboBox.SelectedItem = m_selectHeatSensor;
        //    }
        //}

        public void ReloadHeatSensorState()
        {
            if (m_selectHeatSensor == null)
                return;

            HeatSensor heatSensor = FormMain.Instance.DataManager.DicHeatSensors[m_selectHeatSensor.ID];
            m_selectHeatSensor = heatSensor;
            ShowHeatSensorState();
        }

        //private void EleCollapseSensorComboBox_DropDownOpened(object sender, EventArgs e)
        //{
        //    ReloadCollapseSensor();

        //    if (m_selectCollapseSensor != null)
        //    {
        //        m_cbCollapseSensor.customComboBox.SelectedItem = m_selectCollapseSensor;

        //        //CollapseSensor collapseSensor = FormMain.Instance.DataManager.DicCollapseSensors[m_selectCollapseSensor.ID];
        //        //m_selectCollapseSensor = collapseSensor;
        //        //ShowCollapseSensorState();
        //        ReloadCollapseSensorState();
        //        m_cbCollapseSensor.customComboBox.SelectedItem = m_selectCollapseSensor;
        //    }
        //}

        public void ReloadCollapseSensorState()
        {
            if (m_selectCollapseSensor == null)
                return;

            CollapseSensor collapseSensor = FormMain.Instance.DataManager.DicCollapseSensors[m_selectCollapseSensor.ID];
            m_selectCollapseSensor = collapseSensor;
            ShowCollapseSensorState();
        }

        //private void EleFireSensorComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        //{
        //    if (m_cbFireSensor.customComboBox.SelectedItem == null)
        //        return;

        //    FireSensor fireSensor = (FireSensor)m_cbFireSensor.customComboBox.SelectedItem;
        //    m_selectFireSensor = fireSensor;
        //    ShowFireSensorState();
        //}

        private void ReloadFireRbtn()
        {
            if (m_selectFireSensor == null)
                return;

            ReflashFireRbtn();

            if (m_selectFireSensor == null)
                return;

            if (m_selectFireSensor.State == CommonString.RiskLevel_Attention)
            {
                m_strFireLevel = CommonString.RiskLevel_Attention;
                rbtnFireAttention.IsChecked = true;
                rbtnFireAttention.Refresh();
            }
            else if (m_selectFireSensor.State == CommonString.RiskLevel_Caution)
            {
                m_strFireLevel = CommonString.RiskLevel_Caution;
                rbtnFireCaution.IsChecked = true;
                rbtnFireCaution.Refresh();
            }
            else if (m_selectFireSensor.State == CommonString.RiskLevel_Alert)
            {
                m_strFireLevel = CommonString.RiskLevel_Alert;
                rbtnFireAlert.IsChecked = true;
                rbtnFireAlert.Refresh();
            }
            else if (m_selectFireSensor.State == CommonString.RiskLevel_Serious)
            {
                m_strFireLevel = CommonString.RiskLevel_Serious;
                rbtnFireSerious.IsChecked = true;
                rbtnFireSerious.Refresh();
            }
        }

        private void ReloadFloodRbtn()
        {
            if (m_selectFloodSensor == null)
                return;

            ReflashFloodRbtn();

            if (m_selectFloodSensor.State == CommonString.RiskLevel_Attention)
            {
                m_strFloodLevel = CommonString.RiskLevel_Attention;
                rbtnFloodAttention.IsChecked = true;
                rbtnFloodAttention.Refresh();
            }
            else if (m_selectFloodSensor.State == CommonString.RiskLevel_Caution)
            {
                m_strFloodLevel = CommonString.RiskLevel_Caution;
                rbtnFloodCaution.IsChecked = true;
                rbtnFloodCaution.Refresh();
            }
            else if (m_selectFloodSensor.State == CommonString.RiskLevel_Alert)
            {
                m_strFloodLevel = CommonString.RiskLevel_Alert;
                rbtnFloodAlert.IsChecked = true;
                rbtnFloodAlert.Refresh();
            }
            else if (m_selectFloodSensor.State == CommonString.RiskLevel_Serious)
            {
                m_strFloodLevel = CommonString.RiskLevel_Serious;
                rbtnFloodSerious.IsChecked = true;
                rbtnFloodSerious.Refresh();
            }
        }

        private void ReloadHeatRbtn()
        {
            if (m_selectHeatSensor == null)
                return;

            ReflashHeatRbtn();

            if (m_selectHeatSensor.State == CommonString.RiskLevel_Attention)
            {
                m_strHeatLevel = CommonString.RiskLevel_Attention;
                rbtnHeatAttention.IsChecked = true;
                rbtnHeatAttention.Refresh();
            }
            else if (m_selectHeatSensor.State == CommonString.RiskLevel_Caution)
            {
                m_strHeatLevel = CommonString.RiskLevel_Caution;
                rbtnHeatCaution.IsChecked = true;
                rbtnHeatCaution.Refresh();
            }
            else if (m_selectHeatSensor.State == CommonString.RiskLevel_Alert)
            {
                m_strHeatLevel = CommonString.RiskLevel_Alert;
                rbtnHeatAlert.IsChecked = true;
                rbtnHeatAlert.Refresh();
            }
            else if (m_selectHeatSensor.State == CommonString.RiskLevel_Serious)
            {
                m_strHeatLevel = CommonString.RiskLevel_Serious;
                rbtnHeatSerious.IsChecked = true;
                rbtnHeatSerious.Refresh();
            }
        }

        private void ReloadCollapseRbtn()
        {
            if (m_selectCollapseSensor == null)
                return;

            ReflashCollapseRbtn();

            if (m_selectCollapseSensor.State == CommonString.RiskLevel_Attention)
            {
                m_strCollapseLevel = CommonString.RiskLevel_Attention;
                rbtnCollapseAttention.IsChecked = true;
                rbtnCollapseAttention.Refresh();
            }
            else if (m_selectCollapseSensor.State == CommonString.RiskLevel_Caution)
            {
                m_strCollapseLevel = CommonString.RiskLevel_Caution;
                rbtnCollapseCaution.IsChecked = true;
                rbtnCollapseCaution.Refresh();
            }
            else if (m_selectCollapseSensor.State == CommonString.RiskLevel_Alert)
            {
                m_strCollapseLevel = CommonString.RiskLevel_Alert;
                rbtnCollapseAlert.IsChecked = true;
                rbtnCollapseAlert.Refresh();
            }
            else if (m_selectCollapseSensor.State == CommonString.RiskLevel_Serious)
            {
                m_strCollapseLevel = CommonString.RiskLevel_Serious;
                rbtnCollapseSerious.IsChecked = true;
                rbtnCollapseSerious.Refresh();
            }
        }


        //private void EleFloodSensorComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        //{
        //    if (m_cbFloodSensor.customComboBox.SelectedItem == null)
        //        return;

        //    FloodSensor floodSensor = (FloodSensor)m_cbFloodSensor.customComboBox.SelectedItem;
        //    m_selectFloodSensor = floodSensor;
        //    ShowFloodSensorState();
        //}

        //private void EleHeatSensorComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        //{
        //    if (m_cbHeatSensor.customComboBox.SelectedItem == null)
        //        return;

        //    HeatSensor heatSensor = (HeatSensor)m_cbHeatSensor.customComboBox.SelectedItem;
        //    m_selectHeatSensor = heatSensor;
        //    ShowHeatSensorState();
        //}

        //private void EleCollapseSensorComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        //{
        //    if (m_cbCollapseSensor.customComboBox.SelectedItem == null)
        //        return;

        //    CollapseSensor collapseSensor = (CollapseSensor)m_cbCollapseSensor.customComboBox.SelectedItem;
        //    m_selectCollapseSensor = collapseSensor;
        //    ShowCollapseSensorState();
        //}

        //private void ReloadFireSensor()
        //{
        //    m_cbFireSensor.customComboBox.Items.Clear();

        //    foreach (KeyValuePair<int, FireSensor> item in FormMain.Instance.DataManager.DicFireSensors)
        //    {
        //        m_cbFireSensor.customComboBox.Items.Add(item.Value);
        //    }
        //}

        //private void ReloadFloodSensor()
        //{
        //    m_cbFloodSensor.customComboBox.Items.Clear();

        //    foreach (KeyValuePair<int, FloodSensor> item in FormMain.Instance.DataManager.DicFloodSensors)
        //    {
        //        m_cbFloodSensor.customComboBox.Items.Add(item.Value);
        //    }
        //}

        //private void ReloadHeatSensor()
        //{
        //    m_cbHeatSensor.customComboBox.Items.Clear();

        //    foreach (KeyValuePair<int, HeatSensor> item in FormMain.Instance.DataManager.DicHeatSensors)
        //    {
        //        m_cbHeatSensor.customComboBox.Items.Add(item.Value);
        //    }
        //}

        //private void ReloadCollapseSensor()
        //{
        //    m_cbCollapseSensor.customComboBox.Items.Clear();

        //    foreach (KeyValuePair<int, CollapseSensor> item in FormMain.Instance.DataManager.DicCollapseSensors)
        //    {
        //        m_cbCollapseSensor.customComboBox.Items.Add(item.Value);
        //    }
        //}

        private void ShowFireSensorState()
        {
            if (m_selectFireSensor == null)
                return;

            // 주소 표시
            lbFireAddress.Text = m_selectFireSensor.Addr;

            // 상황종료 버튼 상태
            btnFireEnd.Enabled = true;

            // 상태 이미지 표시
            if (m_selectFireSensor.State == CommonString.RiskLevel_Normal)
            {
                pbFireState.Image = m_imgRisk_Normal;
                btnFireEnd.Enabled = false;
            }
            else if (m_selectFireSensor.State == CommonString.RiskLevel_Attention)
                pbFireState.Image = m_imgRisk_Attention;
            else if (m_selectFireSensor.State == CommonString.RiskLevel_Caution)
                pbFireState.Image = m_imgRisk_Caution;
            else if (m_selectFireSensor.State == CommonString.RiskLevel_Alert)
                pbFireState.Image = m_imgRisk_Alert;
            else if (m_selectFireSensor.State == CommonString.RiskLevel_Serious)
                pbFireState.Image = m_imgRisk_Serious;
        }

        private void ShowFloodSensorState()
        {
            if (m_selectFloodSensor == null)
                return;

            // 주소 표시
            lbFloodAddress.Text = m_selectFloodSensor.Addr;

            // 상황종료 버튼 상태
            btnFloodEnd.Enabled = true;

            if (m_selectFloodSensor.State == CommonString.RiskLevel_Normal)
            {
                pbFloodState.Image = m_imgRisk_Normal;
                btnFloodEnd.Enabled = false;
            }
            else if (m_selectFloodSensor.State == CommonString.RiskLevel_Attention)
                pbFloodState.Image = m_imgRisk_Attention;
            else if (m_selectFloodSensor.State == CommonString.RiskLevel_Caution)
                pbFloodState.Image = m_imgRisk_Caution;
            else if (m_selectFloodSensor.State == CommonString.RiskLevel_Alert)
                pbFloodState.Image = m_imgRisk_Alert;
            else if (m_selectFloodSensor.State == CommonString.RiskLevel_Serious)
                pbFloodState.Image = m_imgRisk_Serious;
        }

        private void ShowHeatSensorState()
        {
            if (m_selectHeatSensor == null)
                return;

            // 주소 표시
            lbHeatAddress.Text = m_selectHeatSensor.Addr;

            // 상황종료 버튼 상태
            btnHeatEnd.Enabled = true;

            if (m_selectHeatSensor.State == CommonString.RiskLevel_Normal)
            {
                pbHeatState.Image = m_imgRisk_Normal;
                btnHeatEnd.Enabled = false;
            }
            else if (m_selectHeatSensor.State == CommonString.RiskLevel_Attention)
                pbHeatState.Image = m_imgRisk_Attention;
            else if (m_selectHeatSensor.State == CommonString.RiskLevel_Caution)
                pbHeatState.Image = m_imgRisk_Caution;
            else if (m_selectHeatSensor.State == CommonString.RiskLevel_Alert)
                pbHeatState.Image = m_imgRisk_Alert;
            else if (m_selectHeatSensor.State == CommonString.RiskLevel_Serious)
                pbHeatState.Image = m_imgRisk_Serious;
        }

        private void ShowCollapseSensorState()
        {
            if (m_selectCollapseSensor == null)
                return;

            // 주소 표시
            lbCollapseAddress.Text = m_selectCollapseSensor.Addr;

            // 상황종료 버튼 상태
            btnCollapseEnd.Enabled = true;

            if (m_selectCollapseSensor.State == CommonString.RiskLevel_Normal)
            {
                pbCollapseState.Image = m_imgRisk_Normal;
                btnCollapseEnd.Enabled = false;
            }
            else if (m_selectCollapseSensor.State == CommonString.RiskLevel_Attention)
                pbCollapseState.Image = m_imgRisk_Attention;
            else if (m_selectCollapseSensor.State == CommonString.RiskLevel_Caution)
                pbCollapseState.Image = m_imgRisk_Caution;
            else if (m_selectCollapseSensor.State == CommonString.RiskLevel_Alert)
                pbCollapseState.Image = m_imgRisk_Alert;
            else if (m_selectCollapseSensor.State == CommonString.RiskLevel_Serious)
                pbCollapseState.Image = m_imgRisk_Serious;
        }

        private void btnFireSensorInfo_Click(object sender, EventArgs e)
        {
            if (m_selectFireSensor == null)
                return;

            m_uFormFireSensorInfo = new uFormFireSensorInfo(m_selectFireSensor);
            m_uFormFireSensorInfo.Parent = this;
            m_uFormFireSensorInfo.Size = new Size(this.Width, this.Height);
            m_uFormFireSensorInfo.Dock = DockStyle.Fill;
            m_uFormFireSensorInfo.BringToFront();
        }

        private void btnFloodSensorInfo_Click(object sender, EventArgs e)
        {
            if (m_selectFloodSensor == null)
                return;

            m_uFormFloodSensorInfo = new uFormFloodSensorInfo(m_selectFloodSensor);
            m_uFormFloodSensorInfo.Parent = this;
            m_uFormFloodSensorInfo.Size = new Size(this.Width, this.Height);
            m_uFormFloodSensorInfo.Dock = DockStyle.Fill;
            m_uFormFloodSensorInfo.BringToFront();
        }

        private void btnHeatSensorInfo_Click(object sender, EventArgs e)
        {
            if (m_selectHeatSensor == null)
                return;

            m_uFormHeatSensorInfo = new uFormHeatSensorInfo(m_selectHeatSensor);
            m_uFormHeatSensorInfo.Parent = this;
            m_uFormHeatSensorInfo.Size = new Size(this.Width, this.Height);
            m_uFormHeatSensorInfo.Dock = DockStyle.Fill;
            m_uFormHeatSensorInfo.BringToFront();
        }

        private void btnCollapseSensorInfo_Click(object sender, EventArgs e)
        {
            if (m_selectCollapseSensor == null)
                return;

            m_uformCollapseSensorInfo = new uFormCollapseSensorInfo(m_selectCollapseSensor);
            m_uformCollapseSensorInfo.Parent = this;
            m_uformCollapseSensorInfo.Size = new Size(this.Width, this.Height);
            m_uformCollapseSensorInfo.Dock = DockStyle.Fill;
            m_uformCollapseSensorInfo.BringToFront();
        }

        private void btnSpread_Click(object sender, EventArgs e)
        {
            ImageButton btn = sender as ImageButton;
            if (btn == null)
                return;

            if (btn == btnFireSpread)
                m_uformSpread = new uFormSpread(FacilityType.FIRE_SENSOR);
            else if (btn == btnFloodSpread)
                m_uformSpread = new uFormSpread(FacilityType.FLOOD_SENSOR);
            else if (btn == btnHeatSpread)
                m_uformSpread = new uFormSpread(FacilityType.HEAT_SENSOR);
            else if (btn == btnCollapseSpread)
                m_uformSpread = new uFormSpread(FacilityType.COLLAPSE_SENSOR);

            m_uformSpread.Parent = this;
            m_uformSpread.Size = new Size(this.Width, this.Height);
            m_uformSpread.Dock = DockStyle.Fill;
            m_uformSpread.BringToFront();
        }

        private void btnFireModifity_Click(object sender, EventArgs e)
        {
            if (m_selectFireSensor == null)
                return;

            m_bFireModifityCheck = !m_bFireModifityCheck;
            ChangeImgFireModifity(m_bFireModifityCheck);
        }

        private void ChangeImgFireModifity(bool bFireModifityCheck)
        {
            if (bFireModifityCheck == true)
            {
                ReloadFireRbtn();
                m_bPanelFireModifitySizeFull = true;
                m_timerFireModifity.Enabled = true;

                btnFireModifity.ImageNormal = global::CrisisAlertManager.Properties.Resources.ModifityClose_Normal;
                btnFireModifity.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.ModifityClose_Hover;
                btnFireModifity.ImageClicked = global::CrisisAlertManager.Properties.Resources.ModifityClose_Click;
            }
            else
            {
                m_bPanelFireModifitySizeFull = false;
                m_timerFireModifity.Enabled = true;

                btnFireModifity.ImageNormal = global::CrisisAlertManager.Properties.Resources.ModifityOpen_Normal;
                btnFireModifity.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.ModifityOpen_Hover;
                btnFireModifity.ImageClicked = global::CrisisAlertManager.Properties.Resources.ModifityOpen_Click;
            }

            btnFireModifity.Refresh();
        }

        private void M_timerFireModifity_Tick(object sender, EventArgs e)
        {
            int maxHight = 160;
            int minHight = 0;
            int gap = 20;

            if (m_bPanelFireModifitySizeFull)
            {
                if (plFireModifity.Height == maxHight)
                    m_timerFireModifity.Enabled = false;
                else
                {
                    if (plFireModifity.Height + gap > maxHight)
                        plFireModifity.Height += maxHight - plFireModifity.Height;
                    else
                        plFireModifity.Height += gap;
                }
            }
            else
            {
                if (plFireModifity.Height <= minHight)
                    m_timerFireModifity.Enabled = false;
                else
                {
                    if (plFireModifity.Height - gap < minHight)
                        plFireModifity.Height -= minHight - plFireModifity.Height;
                    else
                        plFireModifity.Height -= gap;
                }
            }
        }

        private void M_timerFloodModifity_Tick(object sender, EventArgs e)
        {
            int maxHight = 160;
            int minHight = 0;
            int gap = 20;

            if (m_bPanelFloodModifitySizeFull)
            {
                if (plFloodModifity.Height == maxHight)
                    m_timerFloodModifity.Enabled = false;
                else
                {
                    if (plFloodModifity.Height + gap > maxHight)
                        plFloodModifity.Height += maxHight - plFloodModifity.Height;
                    else
                        plFloodModifity.Height += gap;
                }
            }
            else
            {
                if (plFloodModifity.Height <= minHight)
                    m_timerFloodModifity.Enabled = false;
                else
                {
                    if (plFloodModifity.Height - gap < minHight)
                        plFloodModifity.Height -= minHight - plFloodModifity.Height;
                    else
                        plFloodModifity.Height -= gap;
                }
            }
        }

        private void M_timerHeatModifity_Tick(object sender, EventArgs e)
        {
            int maxHight = 160;
            int minHight = 0;
            int gap = 20;

            if (m_bPanelHeatModifitySizeFull)
            {
                if (plHeatModifity.Height == maxHight)
                    m_timerHeatModifity.Enabled = false;
                else
                {
                    if (plHeatModifity.Height + gap > maxHight)
                        plHeatModifity.Height += maxHight - plHeatModifity.Height;
                    else
                        plHeatModifity.Height += gap;
                }
            }
            else
            {
                if (plHeatModifity.Height <= minHight)
                    m_timerHeatModifity.Enabled = false;
                else
                {
                    if (plHeatModifity.Height - gap < minHight)
                        plHeatModifity.Height -= minHight - plHeatModifity.Height;
                    else
                        plHeatModifity.Height -= gap;
                }
            }
        }

        private void M_timerCollapseModifity_Tick(object sender, EventArgs e)
        {
            int maxHight = 160;
            int minHight = 0;
            int gap = 20;

            if (m_bPanelCollapseModifitySizeFull)
            {
                if (plCollapseModifitya.Height == maxHight)
                    m_timerCollapseModifity.Enabled = false;
                else
                {
                    if (plCollapseModifitya.Height + gap > maxHight)
                        plCollapseModifitya.Height += maxHight - plCollapseModifitya.Height;
                    else
                        plCollapseModifitya.Height += gap;
                }
            }
            else
            {
                if (plCollapseModifitya.Height <= minHight)
                    m_timerCollapseModifity.Enabled = false;
                else
                {
                    if (plCollapseModifitya.Height - gap < minHight)
                        plCollapseModifitya.Height -= minHight - plCollapseModifitya.Height;
                    else
                        plCollapseModifitya.Height -= gap;
                }
            }
        }


        public void M_timerSensorReload_Tick(object sender, EventArgs e)
        {
            FormMain.Instance.ReloadSensor();

            if (m_selectFireSensor != null && FormMain.Instance.DataManager.DicFireSensors.ContainsKey(m_selectFireSensor.ID))
            {
                ReloadFireSensorState();
            }

            if (m_selectFloodSensor != null && FormMain.Instance.DataManager.DicFloodSensors.ContainsKey(m_selectFloodSensor.ID))
            {
                ReloadFloodSensorState();
            }

            if (m_selectHeatSensor != null && FormMain.Instance.DataManager.DicFireSensors.ContainsKey(m_selectHeatSensor.ID))
            {
                ReloadHeatSensorState();
            }

            if (m_selectCollapseSensor != null && FormMain.Instance.DataManager.DicFireSensors.ContainsKey(m_selectCollapseSensor.ID))
            {
                ReloadCollapseSensorState();
            }

            // 알림 신호 읽기
            CheckAlarm();
        }

        private void CheckAlarm()
        {
            // 알림 신호 테이블 읽기(최근 순으로)
            AlarmData alarm = FormMain.Instance.DataManager.CheckAlarmData();

            if (alarm == null)
                return;

            // 신호가 있다면 현재 알림창이 떠 있는지 확인
            if (m_formAlertAlarm == null || (m_formAlertAlarm.Visible == false && m_formAlertAlarm.ID != alarm.ID))
            {
                // 알람소리
                FireDetectProcess.PlaySound();

                // 안떠있다면 띄우기
                m_formAlertAlarm = new FormAlertAlarm(alarm);
                m_formAlertAlarm.StartPosition = FormStartPosition.Manual;
                m_formAlertAlarm.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - m_formAlertAlarm.Size.Width, Screen.PrimaryScreen.WorkingArea.Height - m_formAlertAlarm.Size.Height);
                m_formAlertAlarm.Show();
                m_formAlertAlarm.Activate();
            }
            else if (m_formAlertAlarm.ID != alarm.ID)
            {
                m_formAlertAlarm.ChangeAlertAlarm(alarm);
                m_formAlertAlarm.Activate();
            }
        }

        private void rbtnFireAttention_Click(object sender, EventArgs e)
        {
            if (rbtnFireAttention.IsChecked == true)
                return;

            m_strFireLevel = CommonString.RiskLevel_Attention;
            ReflashFireRbtn(); 
            rbtnFireAttention.IsChecked = true;
            rbtnFireAttention.Refresh();
        }

        private void ReflashFireRbtn()
        {
            rbtnFireAttention.IsChecked = false;
            rbtnFireCaution.IsChecked = false;
            rbtnFireAlert.IsChecked = false;
            rbtnFireSerious.IsChecked = false;

            rbtnFireAttention.Refresh();
            rbtnFireCaution.Refresh();
            rbtnFireAlert.Refresh();
            rbtnFireSerious.Refresh();
        }

        private void ReflashFloodRbtn()
        {
            rbtnFloodAttention.IsChecked = false;
            rbtnFloodCaution.IsChecked = false;
            rbtnFloodAlert.IsChecked = false;
            rbtnFloodSerious.IsChecked = false;

            rbtnFloodAttention.Refresh();
            rbtnFloodCaution.Refresh();
            rbtnFloodAlert.Refresh();
            rbtnFloodSerious.Refresh();
        }

        private void ReflashHeatRbtn()
        {
            rbtnHeatAttention.IsChecked = false;
            rbtnHeatCaution.IsChecked = false;
            rbtnHeatAlert.IsChecked = false;
            rbtnHeatSerious.IsChecked = false;

            rbtnHeatAttention.Refresh();
            rbtnHeatCaution.Refresh();
            rbtnHeatAlert.Refresh();
            rbtnHeatSerious.Refresh();
        }

        private void ReflashCollapseRbtn()
        {
            rbtnCollapseAttention.IsChecked = false;
            rbtnCollapseCaution.IsChecked = false;
            rbtnCollapseAlert.IsChecked = false;
            rbtnCollapseSerious.IsChecked = false;

            rbtnCollapseAttention.Refresh();
            rbtnCollapseCaution.Refresh();
            rbtnCollapseAlert.Refresh();
            rbtnCollapseSerious.Refresh();
        }



        private void rbtnFireCaution_Click(object sender, EventArgs e)
        {
            if (rbtnFireCaution.IsChecked == true)
                return;

            m_strFireLevel = CommonString.RiskLevel_Caution;
            ReflashFireRbtn();
            rbtnFireCaution.IsChecked = true;
            rbtnFireCaution.Refresh();
        }

        private void rbtnFireAlert_Click(object sender, EventArgs e)
        {
            if (rbtnFireAlert.IsChecked == true)
                return;

            m_strFireLevel = CommonString.RiskLevel_Alert;
            ReflashFireRbtn();
            rbtnFireAlert.IsChecked = true;
            rbtnFireAlert.Refresh();
        }

        private void rbtnFireSerious_Click(object sender, EventArgs e)
        {
            if (rbtnFireSerious.IsChecked == true)
                return;

            m_strFireLevel = CommonString.RiskLevel_Serious;
            ReflashFireRbtn();
            rbtnFireSerious.IsChecked = true;
            rbtnFireSerious.Refresh();
        }

        private void btnFireSave_Click(object sender, EventArgs e)
        {
            if (m_selectFireSensor == null)
                return;

            FormMessageBox msg;
            bool bChk = false;

            if (m_strFireLevel == m_selectFireSensor.State)
            {
                msg = new FormMessageBox("위기경보 단계 수동 조정", "위기경보 단계가 변동되지 않았습니다. \n다시 한번 확인해주세요. ", MessageBoxButtons.OK);
                msg.StartPosition = FormStartPosition.CenterParent;
                msg.ShowDialog();
                return;
            }

            if (m_strFireLevel == CommonString.RiskLevel_Attention)
            {
                msg = new FormMessageBox("위기경보 단계 수동 조정", "[화재] 위기경보 단계[관심]로 변경하시겠습니까?\n다시 한번 확인해주세요. ", MessageBoxButtons.YesNo);
                bChk = true;
            }
            else if (m_strFireLevel == CommonString.RiskLevel_Caution)
            {
                msg = new FormMessageBox("위기경보 단계 수동 조정", "[화재] 위기경보 단계[주위]로 변경하시겠습니까?\n다시 한번 확인해주세요. ", MessageBoxButtons.YesNo);
                bChk = true;
            }
            else if (m_strFireLevel == CommonString.RiskLevel_Alert)
            {
                msg = new FormMessageBox("위기경보 단계 수동 조정", "[화재] 위기경보 단계[경계]로 변경하시겠습니까?\n다시 한번 확인해주세요. ", MessageBoxButtons.YesNo);
                bChk = true;
            }
            else if (m_strFireLevel == CommonString.RiskLevel_Serious)
            {
                msg = new FormMessageBox("위기경보 단계 수동 조정", "[화재] 위기경보 단계[심각]로 변경하시겠습니까?\n다시 한번 확인해주세요. ", MessageBoxButtons.YesNo);
                bChk = true;
            }
            else 
            {
                msg = new FormMessageBox("위기경보 단계 수동 조정", "위기경보 단계가 선택이 잘못 되었습니다.\n다시 한번 확인해주세요. ", MessageBoxButtons.OK);
            }

            msg.StartPosition = FormStartPosition.CenterParent;
            if (msg.ShowDialog() == DialogResult.Yes && bChk == true)
            {
                // 위기경보 단계 이력 저장
                AlertReport(FacilityType.FIRE_SENSOR, m_selectFireSensor.ID);
                // 알람신호 발생
                CheckAlertAlarm(FacilityType.FIRE_SENSOR);

                // 위기경보 단계 수동 조정
                FormMain.Instance.DataManager.UpdateFireSensorState(m_selectFireSensor, m_strFireLevel);
                FormMain.Instance.ReloadSensor();

                ReloadFireSensorState();
            }

            m_bFireModifityCheck = false;
            ChangeImgFireModifity(m_bFireModifityCheck);
        }

        private bool CheckAlertAlarm(FacilityType type)
        {
            bool bRet = false;
            bool bAlarm = false;
            int nID = -1;
            string strOldLevel = "";
            string strNewLevel = "";
            string strAddress = "";
            
            if (type == FacilityType.FIRE_SENSOR)
            {
                nID = m_selectFireSensor.ID;
                strAddress = m_selectFireSensor.Addr;
                strOldLevel = m_selectFireSensor.State;
                strNewLevel = m_strFireLevel;
            }
            else if (type == FacilityType.FLOOD_SENSOR)
            {
                nID = m_selectFloodSensor.ID;
                strAddress = m_selectFloodSensor.Addr;
                strOldLevel = m_selectFloodSensor.State;
                strNewLevel = m_strFloodLevel;
            }
            else if (type == FacilityType.HEAT_SENSOR)
            {
                nID = m_selectHeatSensor.ID;
                strAddress = m_selectHeatSensor.Addr;
                strOldLevel = m_selectHeatSensor.State;
                strNewLevel = m_strHeatLevel;

            }
            else if (type == FacilityType.COLLAPSE_SENSOR)
            {
                nID = m_selectCollapseSensor.ID;
                strAddress = m_selectCollapseSensor.Addr;
                strOldLevel = m_selectCollapseSensor.State;
                strNewLevel = m_strCollapseLevel;
            }

            if (strOldLevel == CommonString.RiskLevel_Normal || strOldLevel == CommonString.RiskLevel_Attention)
            {
                bAlarm = true;
            }
            if (strOldLevel == CommonString.RiskLevel_Caution && (strNewLevel == CommonString.RiskLevel_Alert || strNewLevel == CommonString.RiskLevel_Serious))
            {
                bAlarm = true;
            }
            else if (strOldLevel == CommonString.RiskLevel_Alert && (strNewLevel == CommonString.RiskLevel_Serious))
            {
                bAlarm = true;
            }

            if (bAlarm)
            {
                strNewLevel = TransRiskLevel(strNewLevel);
                if (FormMain.Instance.DataManager.InsertAlertAarm(type, nID, strAddress, strNewLevel))
                    bRet = true;
            }

            return bRet;
        }

        private string TransRiskLevel(string strRiskLevel)
        {
            string strRiskKor = "";

            if (strRiskLevel == CommonString.RiskLevel_Normal)
                strRiskKor = CommonString.RiskLevel_Normal_Kor;
            else if (strRiskLevel == CommonString.RiskLevel_Attention)
                strRiskKor = CommonString.RiskLevel_Attention_Kor;
            else if (strRiskLevel == CommonString.RiskLevel_Caution)
                strRiskKor = CommonString.RiskLevel_Caution_Kor;
            else if (strRiskLevel == CommonString.RiskLevel_Alert)
                strRiskKor = CommonString.RiskLevel_Alert_Kor;
            else if (strRiskLevel == CommonString.RiskLevel_Serious)
                strRiskKor = CommonString.RiskLevel_Serious_Kor;

            return strRiskKor;
        }

        private void btnFireCancle_Click(object sender, EventArgs e)
        {
            if (m_selectFireSensor == null)
                return;

            FormMessageBox msg = new FormMessageBox("위기경보 단계 수동 조정 취소", "[화재] 위기경보 단계 수동조정을 모두 취소하시겠습니까?\n다시 한번 확인해주세요. ", MessageBoxButtons.YesNo);
            msg.StartPosition = FormStartPosition.CenterParent;
            if (msg.ShowDialog() == DialogResult.Yes)
            {
                m_strFireLevel = m_selectFireSensor.State;

                m_bFireModifityCheck = false;
                ChangeImgFireModifity(m_bFireModifityCheck);
            }
        }

        private void btnFloodModifity_Click(object sender, EventArgs e)
        {
            if (m_selectFloodSensor == null)
                return;

            m_bFloodModifityCheck = !m_bFloodModifityCheck;
            ChangeImgFloodModifity(m_bFloodModifityCheck);
        }

        private void ChangeImgFloodModifity(bool bFloodModifityCheck)
        {
            if (bFloodModifityCheck == true)
            {
                ReloadFloodRbtn();
                m_bPanelFloodModifitySizeFull = true;
                m_timerFloodModifity.Enabled = true;

                btnFloodModifity.ImageNormal = global::CrisisAlertManager.Properties.Resources.ModifityClose_Normal;
                btnFloodModifity.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.ModifityClose_Hover;
                btnFloodModifity.ImageClicked = global::CrisisAlertManager.Properties.Resources.ModifityClose_Click;
            }
            else
            {
                m_bPanelFloodModifitySizeFull = false;
                m_timerFloodModifity.Enabled = true;

                btnFloodModifity.ImageNormal = global::CrisisAlertManager.Properties.Resources.ModifityOpen_Normal;
                btnFloodModifity.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.ModifityOpen_Hover;
                btnFloodModifity.ImageClicked = global::CrisisAlertManager.Properties.Resources.ModifityOpen_Click;
            }

            btnFloodModifity.Refresh();
        }

        private void btnHeatModifity_Click(object sender, EventArgs e)
        {
            if (m_selectHeatSensor == null)
                return;

            m_bHeatModifityCheck = !m_bHeatModifityCheck;
            ChangeImgHeatModifity(m_bHeatModifityCheck);
        }

        private void ChangeImgHeatModifity(bool bHeatModifityCheck)
        {
            if (bHeatModifityCheck == true)
            {
                ReloadHeatRbtn();
                m_bPanelHeatModifitySizeFull = true;
                m_timerHeatModifity.Enabled = true;

                btnHeatModifity.ImageNormal = global::CrisisAlertManager.Properties.Resources.ModifityClose_Normal;
                btnHeatModifity.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.ModifityClose_Hover;
                btnHeatModifity.ImageClicked = global::CrisisAlertManager.Properties.Resources.ModifityClose_Click;
            }
            else
            {
                m_bPanelHeatModifitySizeFull = false;
                m_timerHeatModifity.Enabled = true;

                btnHeatModifity.ImageNormal = global::CrisisAlertManager.Properties.Resources.ModifityOpen_Normal;
                btnHeatModifity.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.ModifityOpen_Hover;
                btnHeatModifity.ImageClicked = global::CrisisAlertManager.Properties.Resources.ModifityOpen_Click;
            }

            btnHeatModifity.Refresh();
        }

        private void btnCollapseModifity_Click(object sender, EventArgs e)
        {
            if (m_selectCollapseSensor == null)
                return;

            m_bCollapseModifityCheck = !m_bCollapseModifityCheck;
            ChangeImgCollapseModifity(m_bCollapseModifityCheck);
        }

        private void ChangeImgCollapseModifity(bool bCollapseModifityCheck)
        {
            if (bCollapseModifityCheck == true)
            {
                ReloadCollapseRbtn();
                m_bPanelCollapseModifitySizeFull = true;
                m_timerCollapseModifity.Enabled = true;

                btnCollapseModifity.ImageNormal = global::CrisisAlertManager.Properties.Resources.ModifityClose_Normal;
                btnCollapseModifity.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.ModifityClose_Hover;
                btnCollapseModifity.ImageClicked = global::CrisisAlertManager.Properties.Resources.ModifityClose_Click;
            }
            else
            {
                m_bPanelCollapseModifitySizeFull = false;
                m_timerCollapseModifity.Enabled = true;

                btnCollapseModifity.ImageNormal = global::CrisisAlertManager.Properties.Resources.ModifityOpen_Normal;
                btnCollapseModifity.ImageMouseOver = global::CrisisAlertManager.Properties.Resources.ModifityOpen_Hover;
                btnCollapseModifity.ImageClicked = global::CrisisAlertManager.Properties.Resources.ModifityOpen_Click;
            }

            btnCollapseModifity.Refresh();
        }

        private void rbtnFloodAttention_Click(object sender, EventArgs e)
        {
            if (rbtnFloodAttention.IsChecked == true)
                return;

            m_strFloodLevel = CommonString.RiskLevel_Attention;
            ReflashFloodRbtn();
            rbtnFloodAttention.IsChecked = true;
            rbtnFloodAttention.Refresh();
        }

        private void rbtnFloodCaution_Click(object sender, EventArgs e)
        {
            if (rbtnFloodCaution.IsChecked == true)
                return;

            m_strFloodLevel = CommonString.RiskLevel_Caution;
            ReflashFloodRbtn();
            rbtnFloodCaution.IsChecked = true;
            rbtnFloodCaution.Refresh();
        }

        private void rbtnFloodAlert_Click(object sender, EventArgs e)
        {
            if (rbtnFloodAlert.IsChecked == true)
                return;

            m_strFloodLevel = CommonString.RiskLevel_Alert;
            ReflashFloodRbtn();
            rbtnFloodAlert.IsChecked = true;
            rbtnFloodAlert.Refresh();
        }

        private void rbtnFloodSerious_Click(object sender, EventArgs e)
        {
            if (rbtnFloodSerious.IsChecked == true)
                return;

            m_strFloodLevel = CommonString.RiskLevel_Serious;
            ReflashFloodRbtn();
            rbtnFloodSerious.IsChecked = true;
            rbtnFloodSerious.Refresh();
        }

        private void btnFloodSave_Click(object sender, EventArgs e)
        {
            if (m_selectFloodSensor == null)
                return;

            FormMessageBox msg;
            bool bChk = false;

            if (m_strFloodLevel == m_selectFloodSensor.State)
            {
                msg = new FormMessageBox("위기경보 단계 수동 조정", "위기경보 단계가 변동되지 않았습니다. \n다시 한번 확인해주세요. ", MessageBoxButtons.OK);
                msg.StartPosition = FormStartPosition.CenterParent;
                msg.ShowDialog();
                return;
            }

            if (m_strFloodLevel == CommonString.RiskLevel_Attention)
            {
                msg = new FormMessageBox("위기경보 단계 수동 조정", "[홍수] 위기경보 단계[관심]로 변경하시겠습니까?\n다시 한번 확인해주세요. ", MessageBoxButtons.YesNo);
                bChk = true;
            }
            else if (m_strFloodLevel == CommonString.RiskLevel_Caution)
            {
                msg = new FormMessageBox("위기경보 단계 수동 조정", "[홍수] 위기경보 단계[주위]로 변경하시겠습니까?\n다시 한번 확인해주세요. ", MessageBoxButtons.YesNo);
                bChk = true;
            }
            else if (m_strFloodLevel == CommonString.RiskLevel_Alert)
            {
                msg = new FormMessageBox("위기경보 단계 수동 조정", "[홍수] 위기경보 단계[경계]로 변경하시겠습니까?\n다시 한번 확인해주세요. ", MessageBoxButtons.YesNo);
                bChk = true;
            }
            else if (m_strFloodLevel == CommonString.RiskLevel_Serious)
            {
                msg = new FormMessageBox("위기경보 단계 수동 조정", "[홍수] 위기경보 단계[심각]로 변경하시겠습니까?\n다시 한번 확인해주세요. ", MessageBoxButtons.YesNo);
                bChk = true;
            }
            else
            {
                msg = new FormMessageBox("위기경보 단계 수동 조정", "위기경보 단계가 선택이 잘못 되었습니다.\n다시 한번 확인해주세요. ", MessageBoxButtons.OK);
            }

            msg.StartPosition = FormStartPosition.CenterParent;
            if (msg.ShowDialog() == DialogResult.Yes && bChk == true)
            {
                // 위기경보 단계 이력 저장
                AlertReport(FacilityType.FLOOD_SENSOR, m_selectFloodSensor.ID);
                // 알람신호 발생
                CheckAlertAlarm(FacilityType.FLOOD_SENSOR);

                // 위기경보 단계 수동 조정
                FormMain.Instance.DataManager.UpdateFloodSensorState(m_selectFloodSensor, m_strFloodLevel);
                FormMain.Instance.ReloadSensor();

                ReloadFloodSensorState();
            }

            m_bFloodModifityCheck = false;
            ChangeImgFloodModifity(m_bFloodModifityCheck);
        }

        private void btnFloodCancle_Click(object sender, EventArgs e)
        {
            if (m_selectFloodSensor == null)
                return;

            FormMessageBox msg = new FormMessageBox("위기경보 단계 수동 조정 취소", "[홍수] 위기경보 단계 수동조정을 모두 취소하시겠습니까?\n다시 한번 확인해주세요. ", MessageBoxButtons.YesNo);
            msg.StartPosition = FormStartPosition.CenterParent;

            if (msg.ShowDialog() == DialogResult.Yes)
            {
                m_strFloodLevel = m_selectFloodSensor.State;

                m_bFloodModifityCheck = false;
                ChangeImgFloodModifity(m_bFloodModifityCheck);
            }
        }

        private void rbtnHeatAttention_Click(object sender, EventArgs e)
        {
            if (rbtnHeatAttention.IsChecked == true)
                return;

            m_strHeatLevel = CommonString.RiskLevel_Attention;
            ReflashHeatRbtn();
            rbtnHeatAttention.IsChecked = true;
            rbtnHeatAttention.Refresh();
        }

        private void rbtnHeatCaution_Click(object sender, EventArgs e)
        {
            if (rbtnHeatCaution.IsChecked == true)
                return;

            m_strHeatLevel = CommonString.RiskLevel_Caution;
            ReflashHeatRbtn();
            rbtnHeatCaution.IsChecked = true;
            rbtnHeatCaution.Refresh();
        }

        private void rbtnHeatAlert_Click(object sender, EventArgs e)
        {
            if (rbtnHeatAlert.IsChecked == true)
                return;

            m_strHeatLevel = CommonString.RiskLevel_Alert;
            ReflashHeatRbtn();
            rbtnHeatAlert.IsChecked = true;
            rbtnHeatAlert.Refresh();
        }

        private void rbtnHeatSerious_Click(object sender, EventArgs e)
        {
            if (rbtnHeatSerious.IsChecked == true)
                return;

            m_strHeatLevel = CommonString.RiskLevel_Serious;
            ReflashHeatRbtn();
            rbtnHeatSerious.IsChecked = true;
            rbtnHeatSerious.Refresh();
        }

        private void btnHeatSave_Click(object sender, EventArgs e)
        {
            if (m_selectHeatSensor == null)
                return;

            FormMessageBox msg;
            bool bChk = false;

            if (m_strHeatLevel == m_selectHeatSensor.State)
            {
                msg = new FormMessageBox("위기경보 단계 수동 조정", "위기경보 단계가 변동되지 않았습니다. \n다시 한번 확인해주세요. ", MessageBoxButtons.OK);
                msg.StartPosition = FormStartPosition.CenterParent;
                msg.ShowDialog();
                return;
            }

            if (m_strHeatLevel == CommonString.RiskLevel_Attention)
            {
                msg = new FormMessageBox("위기경보 단계 수동 조정", "[폭염] 위기경보 단계[관심]로 변경하시겠습니까?\n다시 한번 확인해주세요. ", MessageBoxButtons.YesNo);
                bChk = true;
            }
            else if (m_strHeatLevel == CommonString.RiskLevel_Caution)
            {
                msg = new FormMessageBox("위기경보 단계 수동 조정", "[폭염] 위기경보 단계[주위]로 변경하시겠습니까?\n다시 한번 확인해주세요. ", MessageBoxButtons.YesNo);
                bChk = true;
            }
            else if (m_strHeatLevel == CommonString.RiskLevel_Alert)
            {
                msg = new FormMessageBox("위기경보 단계 수동 조정", "[폭염] 위기경보 단계[경계]로 변경하시겠습니까?\n다시 한번 확인해주세요. ", MessageBoxButtons.YesNo);
                bChk = true;
            }
            else if (m_strHeatLevel == CommonString.RiskLevel_Serious)
            {
                msg = new FormMessageBox("위기경보 단계 수동 조정", "[폭염] 위기경보 단계[심각]로 변경하시겠습니까?\n다시 한번 확인해주세요. ", MessageBoxButtons.YesNo);
                bChk = true;
            }
            else
            {
                msg = new FormMessageBox("위기경보 단계 수동 조정", "위기경보 단계가 선택이 잘못 되었습니다.\n다시 한번 확인해주세요. ", MessageBoxButtons.OK);
            }

            msg.StartPosition = FormStartPosition.CenterParent;
            if (msg.ShowDialog() == DialogResult.Yes && bChk == true)
            {
                // 위기경보 단계 이력 저장
                AlertReport(FacilityType.HEAT_SENSOR, m_selectHeatSensor.ID);
                // 알람신호 발생
                CheckAlertAlarm(FacilityType.HEAT_SENSOR);

                // 위기경보 단계 수동 조정
                FormMain.Instance.DataManager.UpdateHeatSensorState(m_selectHeatSensor, m_strHeatLevel);
                FormMain.Instance.ReloadSensor();

                ReloadHeatSensorState();
            }

            m_bHeatModifityCheck = false;
            ChangeImgHeatModifity(m_bHeatModifityCheck);
        }

        private void btnHeatCancle_Click(object sender, EventArgs e)
        {
            if (m_selectHeatSensor == null)
                return;

            FormMessageBox msg = new FormMessageBox("위기경보 단계 수동 조정 취소", "[폭염] 위기경보 단계 수동조정을 모두 취소하시겠습니까?\n다시 한번 확인해주세요. ", MessageBoxButtons.YesNo);
            msg.StartPosition = FormStartPosition.CenterParent;
            if (msg.ShowDialog() == DialogResult.Yes)
            {
                m_strHeatLevel = m_selectHeatSensor.State;

                m_bHeatModifityCheck = false;
                ChangeImgHeatModifity(m_bHeatModifityCheck);
            }
        }

        private void rbtnCollapseAttention_Click(object sender, EventArgs e)
        {
            if (rbtnCollapseAttention.IsChecked == true)
                return;

            m_strCollapseLevel = CommonString.RiskLevel_Attention;
            ReflashCollapseRbtn();
            rbtnCollapseAttention.IsChecked = true;
            rbtnCollapseAttention.Refresh();
          
        }

        private void rbtnCollapseCaution_Click(object sender, EventArgs e)
        {
            if (rbtnCollapseCaution.IsChecked == true)
                return;

            m_strCollapseLevel = CommonString.RiskLevel_Caution;
            ReflashCollapseRbtn();
            rbtnCollapseCaution.IsChecked = true;
            rbtnCollapseCaution.Refresh();
        }

        private void rbtnCollapseAlert_Click(object sender, EventArgs e)
        {
            if (rbtnCollapseAlert.IsChecked == true)
                return;

            m_strCollapseLevel = CommonString.RiskLevel_Alert;
            ReflashCollapseRbtn();
            rbtnCollapseAlert.IsChecked = true;
            rbtnCollapseAlert.Refresh();
        }

        private void rbtnCollapseSerious_Click(object sender, EventArgs e)
        {
            if (rbtnCollapseSerious.IsChecked == true)
                return;

            m_strCollapseLevel = CommonString.RiskLevel_Serious;
            ReflashCollapseRbtn();
            rbtnCollapseSerious.IsChecked = true;
            rbtnCollapseSerious.Refresh();
        }

        private void btnCollapseSave_Click(object sender, EventArgs e)
        {
            if (m_selectCollapseSensor == null)
                return;

            FormMessageBox msg;
            bool bChk = false;

            if (m_strCollapseLevel == m_selectCollapseSensor.State)
            {
                msg = new FormMessageBox("위기경보 단계 수동 조정", "위기경보 단계가 변동되지 않았습니다. \n다시 한번 확인해주세요. ", MessageBoxButtons.OK);
                msg.StartPosition = FormStartPosition.CenterParent;
                msg.ShowDialog();
                return;
            }

            if (m_strCollapseLevel == CommonString.RiskLevel_Attention)
            {
                msg = new FormMessageBox("위기경보 단계 수동 조정", "[경사지붕괴] 위기경보 단계[관심]로 변경하시겠습니까?\n다시 한번 확인해주세요. ", MessageBoxButtons.YesNo);
                bChk = true;
            }
            else if (m_strCollapseLevel == CommonString.RiskLevel_Caution)
            {
                msg = new FormMessageBox("위기경보 단계 수동 조정", "[경사지붕괴] 위기경보 단계[주위]로 변경하시겠습니까?\n다시 한번 확인해주세요. ", MessageBoxButtons.YesNo);
                bChk = true;
            }
            else if (m_strCollapseLevel == CommonString.RiskLevel_Alert)
            {
                msg = new FormMessageBox("위기경보 단계 수동 조정", "[경사지붕괴] 위기경보 단계[경계]로 변경하시겠습니까?\n다시 한번 확인해주세요. ", MessageBoxButtons.YesNo);
                bChk = true;
            }
            else if (m_strCollapseLevel == CommonString.RiskLevel_Serious)
            {
                msg = new FormMessageBox("위기경보 단계 수동 조정", "[경사지붕괴] 위기경보 단계[심각]로 변경하시겠습니까?\n다시 한번 확인해주세요. ", MessageBoxButtons.YesNo);
                bChk = true;
            }
            else
            {
                msg = new FormMessageBox("위기경보 단계 수동 조정", "위기경보 단계가 선택이 잘못 되었습니다.\n다시 한번 확인해주세요. ", MessageBoxButtons.OK);
            }

            msg.StartPosition = FormStartPosition.CenterParent;
            if (msg.ShowDialog() == DialogResult.Yes && bChk == true)
            {
                // 위기경보 단계 이력 저장
                AlertReport(FacilityType.COLLAPSE_SENSOR, m_selectCollapseSensor.ID);
                // 알람신호 발생
                CheckAlertAlarm(FacilityType.COLLAPSE_SENSOR);

                // 위기경보 단계 수동 조정
                FormMain.Instance.DataManager.UpdateCollapseSensorState(m_selectCollapseSensor, m_strCollapseLevel);
                FormMain.Instance.ReloadSensor();

                ReloadCollapseSensorState();
            }

            m_bCollapseModifityCheck = false;
            ChangeImgCollapseModifity(m_bCollapseModifityCheck);
        }

        private void btnCollapseCancle_Click(object sender, EventArgs e)
        {
            if (m_selectCollapseSensor == null)
                return;

            FormMessageBox msg = new FormMessageBox("위기경보 단계 수동 조정 취소", "[경사지붕괴] 위기경보 단계 수동조정을 모두 취소하시겠습니까?\n다시 한번 확인해주세요. ", MessageBoxButtons.YesNo);
            msg.StartPosition = FormStartPosition.CenterParent;
            if (msg.ShowDialog() == DialogResult.Yes)
            {
                m_strCollapseLevel = m_selectCollapseSensor.State;

                m_bCollapseModifityCheck = false;
                ChangeImgCollapseModifity(m_bCollapseModifityCheck);
            }
        }

        private void btnFireEnd_Click(object sender, EventArgs e)
        {
            if (m_selectFireSensor == null)
                return;

            FormMessageBox msg = new FormMessageBox("상황종료", "[화재] 위기경보 단계를 상황종료 하시겠습니까?\n다시 한번 확인해주세요. ", MessageBoxButtons.YesNo);
            msg.StartPosition = FormStartPosition.CenterParent;
            if (msg.ShowDialog() == DialogResult.Yes)
            {
                // 위기경보 단계 이력 저장
                EndAlertReport(FacilityType.FIRE_SENSOR, m_selectFireSensor.ID);

                // 상황종료
                FormMain.Instance.DataManager.ResetFireSensor(m_selectFireSensor);
                FormMain.Instance.ReloadSensor();

                ReloadFireSensorState();
            }
        }

        private void btnFloodEnd_Click(object sender, EventArgs e)
        {
            if (m_selectFloodSensor == null)
                return;

            FormMessageBox msg = new FormMessageBox("상황종료", "[홍수] 위기경보 단계를 상황종료 하시겠습니까?\n다시 한번 확인해주세요. ", MessageBoxButtons.YesNo);
            msg.StartPosition = FormStartPosition.CenterParent;
            if (msg.ShowDialog() == DialogResult.Yes)
            {
                // 위기경보 단계 이력 저장
                EndAlertReport(FacilityType.FLOOD_SENSOR, m_selectFloodSensor.ID);

                // 상황종료
                FormMain.Instance.DataManager.ResetFloodSensor(m_selectFloodSensor);
                FormMain.Instance.ReloadSensor();

                ReloadFloodSensorState();
            }
        }

        private void btnHeatEnd_Click(object sender, EventArgs e)
        {
            if (m_selectHeatSensor == null)
                return;

            FormMessageBox msg = new FormMessageBox("상황종료", "[폭염] 위기경보 단계를 상황종료 하시겠습니까?\n다시 한번 확인해주세요. ", MessageBoxButtons.YesNo);
            msg.StartPosition = FormStartPosition.CenterParent;
            if (msg.ShowDialog() == DialogResult.Yes)
            {
                // 위기경보 단계 이력 저장
                EndAlertReport(FacilityType.HEAT_SENSOR, m_selectHeatSensor.ID);

                // 상황종료
                FormMain.Instance.DataManager.ResetHeatSensor(m_selectHeatSensor);
                FormMain.Instance.ReloadSensor();

                ReloadHeatSensorState();
            }
        }

        private void btnCollapseEnd_Click(object sender, EventArgs e)
        {
            if (m_selectCollapseSensor == null)
                return;

            FormMessageBox msg = new FormMessageBox("상황종료", "[경사지붕괴] 위기경보 단계를 상황종료 하시겠습니까?\n다시 한번 확인해주세요. ", MessageBoxButtons.YesNo);
            msg.StartPosition = FormStartPosition.CenterParent;
            if (msg.ShowDialog() == DialogResult.Yes)
            {
                // 위기경보 단계 이력 저장
                EndAlertReport(FacilityType.COLLAPSE_SENSOR, m_selectCollapseSensor.ID);

                // 상황종료
                FormMain.Instance.DataManager.ResetCollapseSensor(m_selectCollapseSensor);
                FormMain.Instance.ReloadSensor();

                ReloadCollapseSensorState();
            }
        }

        private void EndAlertReport(FacilityType facilityType, int nID)
        {
            string strOldData = "";
            string strNewData = CommonString.RiskLevel_Normal_Kor;

            if (facilityType == FacilityType.FIRE_SENSOR)
                strOldData = TransRiskLevel(m_selectFireSensor.State);
            else if (facilityType == FacilityType.FLOOD_SENSOR)
                strOldData = TransRiskLevel(m_selectFloodSensor.State);
            else if (facilityType == FacilityType.HEAT_SENSOR)
                strOldData = TransRiskLevel(m_selectHeatSensor.State);
            else if (facilityType == FacilityType.COLLAPSE_SENSOR)
                strOldData = TransRiskLevel(m_selectCollapseSensor.State);

            string strDataName = CommonString.GetRiskDataName(strOldData, strNewData);

            FormMain.Instance.DataManager.InsertAlertReport(facilityType, nID, strDataName, strOldData, strNewData);
        }

        private void AlertReport(FacilityType facilityType, int nID)
        {
            string strOldData = "";
            string strNewData = "";

            if (facilityType == FacilityType.FIRE_SENSOR)
            {
                strOldData = TransRiskLevel(m_selectFireSensor.State);
                strNewData = TransRiskLevel(m_strFireLevel);
            }
            else if (facilityType == FacilityType.FLOOD_SENSOR)
            {
                strOldData = TransRiskLevel(m_selectFloodSensor.State);
                strNewData = TransRiskLevel(m_strFloodLevel);
            }
            else if (facilityType == FacilityType.HEAT_SENSOR)
            {
                strOldData = TransRiskLevel(m_selectHeatSensor.State);
                strNewData = TransRiskLevel(m_strHeatLevel);
            }
            else if (facilityType == FacilityType.COLLAPSE_SENSOR)
            {
                strOldData = TransRiskLevel(m_selectCollapseSensor.State);
                strNewData = TransRiskLevel(m_strCollapseLevel);
            }

            string strDataName = CommonString.GetRiskDataName(strOldData, strNewData);

            FormMain.Instance.DataManager.InsertAlertReport(facilityType, nID, strDataName, strOldData, strNewData);
        }

        private void btnFireRefresh_Click(object sender, EventArgs e)
        {
            FormMain.Instance.ReloadSensor();

            ReloadFireSensorState();
        }

        private void btnFloodRefresh_Click(object sender, EventArgs e)
        {
            FormMain.Instance.ReloadSensor();

            ReloadFloodSensorState();
        }

        private void btnHeatRefresh_Click(object sender, EventArgs e)
        {
            FormMain.Instance.ReloadSensor();

            ReloadHeatSensorState();
        }

        private void btnCollapseRefresh_Click(object sender, EventArgs e)
        {
            FormMain.Instance.ReloadSensor();

            ReloadCollapseSensorState();
        }

        public void ShowAlarmSensor(FacilityType type, int nSensorID)
        {
            if (type == FacilityType.FIRE_SENSOR)
            {
                if (!FormMain.Instance.DataManager.DicFireSensors.ContainsKey(nSensorID))
                    return;

                m_selectFireSensor = FormMain.Instance.DataManager.DicFireSensors[nSensorID];
                ShowFireSensorState();

                //for (int i = 0; m_cbFireSensor.customComboBox.Items.Count > i; i++)
                //{
                //    FireSensor fireSensor = (FireSensor)m_cbFireSensor.customComboBox.Items[i];

                //    if (fireSensor.ID == nSensorID)
                //    {
                //        m_cbFireSensor.customComboBox.SelectedIndex = i;
                //        break;
                //    }
                //}
            }
            else if (type == FacilityType.FLOOD_SENSOR)
            {
                if (!FormMain.Instance.DataManager.DicFloodSensors.ContainsKey(nSensorID))
                    return;

                m_selectFloodSensor = FormMain.Instance.DataManager.DicFloodSensors[nSensorID];
                ShowFloodSensorState();

                //for (int i = 0; m_cbFloodSensor.customComboBox.Items.Count > i; i++)
                //{
                //    FloodSensor floodSensor = (FloodSensor)m_cbFloodSensor.customComboBox.Items[i];

                //    if (floodSensor.ID == nSensorID)
                //    {
                //        m_cbFloodSensor.customComboBox.SelectedIndex = i;
                //        break;
                //    }
                //}
            }
            else if (type == FacilityType.HEAT_SENSOR)
            {
                if (!FormMain.Instance.DataManager.DicHeatSensors.ContainsKey(nSensorID))
                    return;

                m_selectHeatSensor = FormMain.Instance.DataManager.DicHeatSensors[nSensorID];
                ShowHeatSensorState();

                //for (int i = 0; m_cbHeatSensor.customComboBox.Items.Count > i; i++)
                //{
                //    HeatSensor heatSensor = (HeatSensor)m_cbHeatSensor.customComboBox.Items[i];

                //    if (heatSensor.ID == nSensorID)
                //    {
                //        m_cbHeatSensor.customComboBox.SelectedIndex = i;
                //        break;
                //    }
                //}
            }
            else if (type == FacilityType.COLLAPSE_SENSOR)
            {
                if (!FormMain.Instance.DataManager.DicCollapseSensors.ContainsKey(nSensorID))
                    return;

                m_selectCollapseSensor = FormMain.Instance.DataManager.DicCollapseSensors[nSensorID];
                ShowCollapseSensorState();

                //for (int i = 0; m_cbCollapseSensor.customComboBox.Items.Count > i; i++)
                //{
                //    CollapseSensor collapseSensor = (CollapseSensor)m_cbCollapseSensor.customComboBox.Items[i];

                //    if (collapseSensor.ID == nSensorID)
                //    {
                //        m_cbCollapseSensor.customComboBox.SelectedIndex = i;
                //        break;
                //    }
                //}
            }
        }

        public void CheckCloseAlarm(int nID)
        {
            if (m_formAlertAlarm == null || m_formAlertAlarm.Visible == false)
                return;

            m_formAlertAlarm.CheckCloseAlarm(nID);
        }

        private void btnFireSensor_Click(object sender, EventArgs e)
        {
            if (m_selectFireSensor == null)
                return;

            FormSensorSearch sensorSearch = new FormSensorSearch(FacilityType.FIRE_SENSOR, m_selectFireSensor.ID);
            sensorSearch.StartPosition = FormStartPosition.CenterParent;
            sensorSearch.ShowDialog();
        }

        private void btnFloodSensor_Click(object sender, EventArgs e)
        {
            if (m_selectFloodSensor == null)
                return;

            FormSensorSearch sensorSearch = new FormSensorSearch(FacilityType.FLOOD_SENSOR, m_selectFloodSensor.ID);
            sensorSearch.StartPosition = FormStartPosition.CenterParent;
            sensorSearch.ShowDialog();
        }

        private void btnHeatSensor_Click(object sender, EventArgs e)
        {
            if (m_selectHeatSensor == null)
                return;

            FormSensorSearch sensorSearch = new FormSensorSearch(FacilityType.HEAT_SENSOR, m_selectHeatSensor.ID);
            sensorSearch.StartPosition = FormStartPosition.CenterParent;
            sensorSearch.ShowDialog();
        }

        private void btnCollapseSensor_Click(object sender, EventArgs e)
        {
            if (m_selectCollapseSensor == null)
                return;

            FormSensorSearch sensorSearch = new FormSensorSearch(FacilityType.COLLAPSE_SENSOR, m_selectCollapseSensor.ID);
            sensorSearch.StartPosition = FormStartPosition.CenterParent;
            sensorSearch.ShowDialog();
        }
    }
}
