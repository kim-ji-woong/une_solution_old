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
using SDMS_Building.Data;
using UnE.Sensor;

namespace SDMS_Building.PopupDialog.Config
{
    public enum SMSMessageType
    {
        UNKNOWN = -1,
        RESET_FIRE = 0,     // 화재복구(0)
        DETECT_FIRE,        // 화재탐지(1)
        REPORT_FIRE,        // 화재신고(2)
        DETECT_PSM,         // 누출탐지(3)
        REPORT_PSM,         // 누출신고(4)
        RESET_PSM,          // 누출복구(5)
        DETECT_SECURITY,    // 방범탐지(6)
        REPORT_SECURITY,    // 방범신고(7)
        RESET_SECURITY,     // 방범복구(8)
        DETECT_EARTHQUAKE,  // 지진탐지(9)
        DETECT_TH,          // 온도/습도 탐지(10)
        RESET_TH,           // 온도/습도 복구(11)
        RESET_ETC,          // ETC 복구
        DETECT_ETC,         // ETC 탐지
        REPORT_ETC          // ETC 신고
    }

    public partial class FormSMSConfig : Form
    {
        private UEWpfControl.WpfComboBox m_cbType = null;
        Dictionary<SMSMessageType, bool> m_dicSMSConfig = new Dictionary<SMSMessageType, bool>();

        public FormSMSConfig()
        {
            InitializeComponent();

            m_cbType = new UEWpfControl.WpfComboBox();
            eleType.Child = m_cbType;
            m_cbType.SetSize(eleType.Width, eleType.Height);
            m_cbType.customComboBox.SelectionChanged += cbType_SelectionChanged;
        }

        private void FormSMSConfig_Load(object sender, EventArgs e)
        {
            LoadSMSConfig();
            SetSMSConfig();

            InitComboBox();
        }

        private void InitComboBox()
        {
            m_cbType.customComboBox.DisplayMemberPath = "DisplayName";
            m_cbType.customComboBox.SelectedValuePath = "FacilityType";

            m_cbType.customComboBox.Items.Add(new FacilityTypeComboBoxItem(IFacility.FacilityType.NONE, "모두"));
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

        private void LoadSMSConfig()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Select MessageType, UseSMS ");
            sb.Append("  From SDMSSMSConfig ");
            sb.AppendFormat(" Where SiteID = {0} ", UnE.SOP.ProxySOP.Instance.SiteID);

            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(sb.ToString());
            if (arrResult != null && arrResult.Count > 0)
            {
                for (int i = 0; i < arrResult.Count; i += 2)
                {
                    int nMessageType = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    int nUseSMS = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);

                    SMSMessageType type;
                    if (Enum.TryParse(nMessageType.ToString(), out type))
                        m_dicSMSConfig[type] = (nUseSMS == 1) ? true : false;
                }
            }

            bool isRealMode = true;

            sb = new StringBuilder();
            sb.Append("SELECT PropertyValue ");
            sb.Append("  FROM OptionSDMS ");
            sb.AppendFormat(" where PropertyName ='TranningMode' AND SiteID = {0}", UnE.SOP.ProxySOP.Instance.SiteID);

            arrResult = FormMain.Instance.DBManager.GetResultData(sb.ToString());
            if (arrResult != null && arrResult.Count > 0)
            {
                int nDur = WebDBManager.GetIntField(arrResult[0].ToString(), 0);
                if (nDur == 1)
                    isRealMode = false;
                else
                    isRealMode = true;
            }

            PreferenceManager.Instance.RealMode = isRealMode;
            btnTrainingMode.IsChecked = !isRealMode;

            string strHeaderMsg = "훈련상황";

            sb = new StringBuilder();
            sb.Append("Select PropertyValue ");
            sb.Append("  From OptionSDMS ");
            sb.AppendFormat(" Where PropertyName ='HeaderMsg' And SiteID = {0} ", UnE.SOP.ProxySOP.Instance.SiteID);

            arrResult = FormMain.Instance.DBManager.GetResultData(sb.ToString());
            if (arrResult == null || arrResult.Count == 0)
            {
                arrResult = FormMain.Instance.DBManager.GetResultData("SELECT MAX(ID) FROM OptionSDMS");
                if (arrResult != null && arrResult.Count > 0)
                {
                    int idx = WebDBManager.GetIntField(arrResult[0].ToString(), 0);
                    sb = new StringBuilder();
                    sb.Append("INSERT INTO OptionSDMS (ID, PropertyName, PropertyValue, Description, SiteID) ");
                    sb.AppendFormat("VALUES({2}, 'HeaderMsg', '{0}', '메시지 앞머리 문구', {1})", strHeaderMsg, UnE.SOP.ProxySOP.Instance.SiteID, idx + 1);
                    FormMain.Instance.DBManager.GetResultData(sb.ToString());
                }
            }
            else
                strHeaderMsg = WebDBManager.GetStringField(arrResult[0].ToString(), "");

            txtTraning.Text = strHeaderMsg;
            //txtTraning.ReadOnly = !btnTrainingMode.IsChecked;
            if (btnTrainingMode.IsChecked)
            {
                label9.Visible = true;
                txtTraning.Visible = true;
                panel7.Visible = true;
            }
            else
            {
                label9.Visible = false;
                txtTraning.Visible = false;
                panel7.Visible = false;
            }
        }

        private void SetSMSConfig()
        {
            if (m_dicSMSConfig.Count == 0)
                return;

            FacilityTypeComboBoxItem selectedItem = m_cbType.customComboBox.SelectedItem as FacilityTypeComboBoxItem;
            if (selectedItem == null)
                return;

            // 지진은 탐지밖에 없다
            if (selectedItem.FacilityType == IFacility.FacilityType.Earthquake)
            {
                btnMalfunction.Visible = false;
                btnReport.Visible = false;
                label2.Visible = false;
                label4.Visible = false;
            }
            else if (selectedItem.FacilityType == IFacility.FacilityType.TEMPERATURE_HUMIDITY)
            {
                btnMalfunction.Visible = true;
                btnReport.Visible = false;
                label2.Visible = false;
                label4.Visible = true;
            }
            else
            {
                btnMalfunction.Visible = true;
                btnReport.Visible = true;
                label2.Visible = true;
                label4.Visible = true;
            }

            if (selectedItem.FacilityType == IFacility.FacilityType.NONE)
            {
                int nDetectCount = 0;
                int nReportCount = 0;
                int nMalfunctionCount = 0;

                foreach (KeyValuePair<SMSMessageType, bool> item in m_dicSMSConfig)
                {
                    string strKey = item.Key.ToString();
                    bool bUse = item.Value;

                    if (strKey.Contains("DETECT") && !bUse)
                        nDetectCount++;
                    else if (strKey.Contains("REPORT") && !bUse)
                        nReportCount++;
                    else if (strKey.Contains("RESET") && !bUse)
                        nMalfunctionCount++;
                }

                if (nDetectCount == 0)
                    btnDetect.IsChecked = true;
                else
                    btnDetect.IsChecked = false;

                if (nReportCount == 0)
                    btnReport.IsChecked = true;
                else
                    btnReport.IsChecked = false;

                if (nMalfunctionCount == 0)
                    btnMalfunction.IsChecked = true;
                else
                    btnMalfunction.IsChecked = false;

                return;
            }
            else
            {
                foreach (KeyValuePair<SMSMessageType, bool> item in m_dicSMSConfig)
                {
                    SMSMessageType msgType = item.Key;
                    bool bChecked = item.Value;

                    if (selectedItem.FacilityType == IFacility.FacilityType.FIRE_SENSOR)
                    {
                        if (msgType == SMSMessageType.DETECT_FIRE)
                            btnDetect.IsChecked = bChecked;
                        else if (msgType == SMSMessageType.REPORT_FIRE)
                            btnReport.IsChecked = bChecked;
                        else if (msgType == SMSMessageType.RESET_FIRE)
                            btnMalfunction.IsChecked = bChecked;
                    }
                    else if (selectedItem.FacilityType == IFacility.FacilityType.PSM_SENSOR)
                    {
                        if (msgType == SMSMessageType.DETECT_PSM)
                            btnDetect.IsChecked = bChecked;
                        else if (msgType == SMSMessageType.REPORT_PSM)
                            btnReport.IsChecked = bChecked;
                        else if (msgType == SMSMessageType.RESET_PSM)
                            btnMalfunction.IsChecked = bChecked;
                    }
                    else if (selectedItem.FacilityType == IFacility.FacilityType.Security_Sensor)
                    {
                        if (msgType == SMSMessageType.DETECT_SECURITY)
                            btnDetect.IsChecked = bChecked;
                        else if (msgType == SMSMessageType.REPORT_SECURITY)
                            btnReport.IsChecked = bChecked;
                        else if (msgType == SMSMessageType.RESET_SECURITY)
                            btnMalfunction.IsChecked = bChecked;
                    }
                    else if (selectedItem.FacilityType == IFacility.FacilityType.Earthquake)
                    {
                        if (msgType == SMSMessageType.DETECT_EARTHQUAKE)
                            btnDetect.IsChecked = bChecked;
                    }
                    else if (selectedItem.FacilityType == IFacility.FacilityType.TEMPERATURE_HUMIDITY)
                    {
                        if (msgType == SMSMessageType.DETECT_TH)
                            btnDetect.IsChecked = bChecked;
                        else if (msgType == SMSMessageType.RESET_TH)
                            btnMalfunction.IsChecked = bChecked;
                    }
                    else if (selectedItem.FacilityType == IFacility.FacilityType.STRONG_WIND)
                    {
                        if (msgType == SMSMessageType.DETECT_ETC)
                            btnDetect.IsChecked = bChecked;
                        else if (msgType == SMSMessageType.REPORT_ETC)
                            btnReport.IsChecked = bChecked;
                        else if (msgType == SMSMessageType.RESET_ETC)
                            btnMalfunction.IsChecked = bChecked;
                    }
                }
            }

            btnDetect.Refresh();
            btnReport.Refresh();
            btnMalfunction.Refresh();
        }

        private void cbType_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            SetSMSConfig();
        }

        private void btnDetect_Click(object sender, EventArgs e)
        {
            btnDetect.IsChecked = !btnDetect.IsChecked;

            FacilityTypeComboBoxItem selectedItem = m_cbType.customComboBox.SelectedItem as FacilityTypeComboBoxItem;
            if (selectedItem == null)
                return;

            if (selectedItem.FacilityType == IFacility.FacilityType.NONE)
            {
                List<SMSMessageType> items = new List<SMSMessageType>();
                foreach (KeyValuePair<SMSMessageType, bool> item in m_dicSMSConfig)
                {
                    if (item.Key.ToString().Contains("DETECT"))
                        items.Add(item.Key);
                }
                foreach (SMSMessageType item in items)
                {
                    m_dicSMSConfig[item] = btnDetect.IsChecked;
                }
            }
            else if (selectedItem.FacilityType == IFacility.FacilityType.FIRE_SENSOR)
                m_dicSMSConfig[SMSMessageType.DETECT_FIRE] = btnDetect.IsChecked;
            else if (selectedItem.FacilityType == IFacility.FacilityType.PSM_SENSOR)
                m_dicSMSConfig[SMSMessageType.DETECT_PSM] = btnDetect.IsChecked;
            else if (selectedItem.FacilityType == IFacility.FacilityType.Security_Sensor)
                m_dicSMSConfig[SMSMessageType.DETECT_SECURITY] = btnDetect.IsChecked;
            else if (selectedItem.FacilityType == IFacility.FacilityType.Earthquake)
                m_dicSMSConfig[SMSMessageType.DETECT_EARTHQUAKE] = btnDetect.IsChecked;
            else if (selectedItem.FacilityType == IFacility.FacilityType.STRONG_WIND)
                m_dicSMSConfig[SMSMessageType.DETECT_ETC] = btnDetect.IsChecked;
            else if (selectedItem.FacilityType == IFacility.FacilityType.TEMPERATURE_HUMIDITY)
                m_dicSMSConfig[SMSMessageType.DETECT_TH] = btnDetect.IsChecked;
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            btnReport.IsChecked = !btnReport.IsChecked;

            FacilityTypeComboBoxItem selectedItem = m_cbType.customComboBox.SelectedItem as FacilityTypeComboBoxItem;
            if (selectedItem == null)
                return;

            if (selectedItem.FacilityType == IFacility.FacilityType.NONE)
            {
                List<SMSMessageType> items = new List<SMSMessageType>();
                foreach (KeyValuePair<SMSMessageType, bool> item in m_dicSMSConfig)
                {
                    if (item.Key.ToString().Contains("REPORT"))
                        items.Add(item.Key);
                }
                foreach (SMSMessageType item in items)
                {
                    m_dicSMSConfig[item] = btnReport.IsChecked;
                }
            }
            else if (selectedItem.FacilityType == IFacility.FacilityType.FIRE_SENSOR)
                m_dicSMSConfig[SMSMessageType.REPORT_FIRE] = btnReport.IsChecked;
            else if (selectedItem.FacilityType == IFacility.FacilityType.PSM_SENSOR)
                m_dicSMSConfig[SMSMessageType.REPORT_PSM] = btnReport.IsChecked;
            else if (selectedItem.FacilityType == IFacility.FacilityType.Security_Sensor)
                m_dicSMSConfig[SMSMessageType.REPORT_SECURITY] = btnReport.IsChecked;
            else if (selectedItem.FacilityType == IFacility.FacilityType.STRONG_WIND)
                m_dicSMSConfig[SMSMessageType.REPORT_ETC] = btnReport.IsChecked;
        }

        private void btnMalfunction_Click(object sender, EventArgs e)
        {
            btnMalfunction.IsChecked = !btnMalfunction.IsChecked;

            FacilityTypeComboBoxItem selectedItem = m_cbType.customComboBox.SelectedItem as FacilityTypeComboBoxItem;
            if (selectedItem == null)
                return;

            if (selectedItem.FacilityType == IFacility.FacilityType.NONE)
            {
                List<SMSMessageType> items = new List<SMSMessageType>();
                foreach (KeyValuePair<SMSMessageType, bool> item in m_dicSMSConfig)
                {
                    if (item.Key.ToString().Contains("RESET"))
                        items.Add(item.Key);
                }
                foreach (SMSMessageType item in items)
                {
                    m_dicSMSConfig[item] = btnMalfunction.IsChecked;
                }
            }
            else if (selectedItem.FacilityType == IFacility.FacilityType.FIRE_SENSOR)
                m_dicSMSConfig[SMSMessageType.RESET_FIRE] = btnMalfunction.IsChecked;
            else if (selectedItem.FacilityType == IFacility.FacilityType.PSM_SENSOR)
                m_dicSMSConfig[SMSMessageType.RESET_PSM] = btnMalfunction.IsChecked;
            else if (selectedItem.FacilityType == IFacility.FacilityType.Security_Sensor)
                m_dicSMSConfig[SMSMessageType.RESET_SECURITY] = btnMalfunction.IsChecked;
            else if (selectedItem.FacilityType == IFacility.FacilityType.STRONG_WIND)
                m_dicSMSConfig[SMSMessageType.RESET_ETC] = btnMalfunction.IsChecked;
            else if (selectedItem.FacilityType == IFacility.FacilityType.TEMPERATURE_HUMIDITY)
                m_dicSMSConfig[SMSMessageType.RESET_TH] = btnMalfunction.IsChecked;
        }

        private void btnTrainingMode_Click(object sender, EventArgs e)
        {
            btnTrainingMode.IsChecked = !btnTrainingMode.IsChecked;
            if (btnTrainingMode.IsChecked)
            {
                label9.Visible = true;
                txtTraning.Visible = true;
                panel7.Visible = true;
            }
            else
            {
                label9.Visible = false;
                txtTraning.Visible = false;
                panel7.Visible = false;
            }
            //txtTraning.ReadOnly = !btnTrainingMode.IsChecked;
        }

        public void Save()
        {
            string strSQL = "";
            foreach (KeyValuePair<SMSMessageType, bool> item in m_dicSMSConfig)
            {
                int nKey = (int)item.Key;
                int nUse = (item.Value) ? 1 : 0;

                strSQL = string.Format("Update SDMSSMSConfig Set UseSMS={0} Where MessageType={1}", nUse, nKey);

                FormMain.Instance.DBManager.GetResultData(strSQL);
            }

            strSQL = string.Format("UPDATE OptionSDMS SET PropertyValue={0} WHERE PropertyName='TranningMode' and SiteID = {1}", btnTrainingMode.IsChecked ? 1 : 0, UnE.SOP.ProxySOP.Instance.SiteID);
            FormMain.Instance.DBManager.GetResultData(strSQL);

            strSQL = string.Format("UPDATE OptionSDMS SET PropertyValue='{0}' WHERE PropertyName='HeaderMsg' and SiteID = {1}", txtTraning.Text.Trim(), UnE.SOP.ProxySOP.Instance.SiteID);
            FormMain.Instance.DBManager.GetResultData(strSQL);
        }
    }
}
