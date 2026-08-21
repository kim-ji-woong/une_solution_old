using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility;
using System.Collections;
using UnE.Earthquake;

namespace SDMS.PopupDialog
{
    public partial class FormEarthquakeOption : Form
    {
        private class OptionGroup
        {
            private Label m_labelTitle = null;
            private Panel m_panelIntens = null;
            private Panel m_panelInitAction = null;
            private Panel m_panelSOP = null;
            private TextBox m_textBoxMin = null;
            private TextBox m_textBoxMax = null;
            private Label m_labelMin = null;
            private Label m_labelMax = null;
            private PictureBox m_pbPrev = null;
            private PictureBox m_pbNext = null;
            private TextBox m_textBoxSMS = null;
            private TextBox m_textBoxBroadcast = null;
            private TextBox m_textBoxSOP = null;
            private CheckBox m_checkBoxSMS = null;
            private CheckBox m_checkBoxBroadcast = null;
            private CheckBox m_checkBoxSOP = null;
            private Button m_btnSOP = null;

            private double m_minIntens = 0.0;
            private double m_maxIntens = 0.0;
            private string m_strLinkedSOP = "";

            private EarthquakeOption.IntensOption m_intensOption = EarthquakeOption.IntensOption.NONE;
            private bool m_isValidate = false;

            private FormEarthquakeOption m_frmParent = null;

            public Label Title
            {
                get { return m_labelTitle; }
                set { m_labelTitle = value; }
            }

            public Panel Intens
            {
                get { return m_panelIntens; }
                set { m_panelIntens = value; }
            }

            public Panel InitAction
            {
                get { return m_panelInitAction; }
                set { m_panelInitAction = value; }
            }

            public Panel SOP
            {
                get { return m_panelSOP; }
                set { m_panelSOP = value; }
            }

            public TextBox MinIntens
            {
                get { return m_textBoxMin; }
                set { m_textBoxMin = value; }
            }

            public TextBox MaxIntens
            {
                get { return m_textBoxMax; }
                set { m_textBoxMax = value; }
            }

            public Label MinIntensLabel
            {
                get { return m_labelMin; }
                set { m_labelMin = value; }
            }

            public Label MaxIntensLabel
            {
                get { return m_labelMax; }
                set { m_labelMax = value; }
            }

            public PictureBox PrevButton
            {
                get { return m_pbPrev; }
                set { m_pbPrev = value; }
            }

            public PictureBox NextButton
            {
                get { return m_pbNext; }
                set { m_pbNext = value; }
            }

            public TextBox SMSMessage
            {
                get { return m_textBoxSMS; }
                set { m_textBoxSMS = value; }
            }

            public TextBox BroadcastMessage
            {
                get { return m_textBoxBroadcast; }
                set { m_textBoxBroadcast = value; }
            }

            public TextBox LinkedSOP
            {
                get { return m_textBoxSOP; }
                set { m_textBoxSOP = value; }
            }

            public CheckBox UseSMS
            {
                get { return m_checkBoxSMS; }
                set { m_checkBoxSMS = value; }
            }

            public CheckBox UseBroadcast
            {
                get { return m_checkBoxBroadcast; }
                set { m_checkBoxBroadcast = value; }
            }

            public CheckBox RunSOP
            {
                get { return m_checkBoxSOP; }
                set { m_checkBoxSOP = value; }
            }

            public Button LoadSOP
            {
                get { return m_btnSOP; }
                set { m_btnSOP = value; }
            }

            public bool Visible
            {
                get { return m_labelTitle.Visible; }
                set
                {
                    m_labelTitle.Visible = m_panelIntens.Visible = m_panelInitAction.Visible = m_panelSOP.Visible = value;
                    SetRangeVisible(value);
                }
            }

            public bool Editable
            {
                get { return !m_textBoxMin.ReadOnly; }
                set
                {
                    m_textBoxMin.ReadOnly = m_textBoxMax.ReadOnly = m_textBoxSMS.ReadOnly = m_textBoxBroadcast.ReadOnly = m_textBoxSOP.ReadOnly = !value;
                    //m_pbPrev.Visible = m_pbNext.Visible = value;
                }
            }

            public EarthquakeOption.IntensOption IntensOption
            {
                get { return m_intensOption; }
                set { m_intensOption = value; }
            }

            public bool IsValid
            {
                get { return m_isValidate; }
            }

            public OptionGroup(FormEarthquakeOption frmParent, Label title, Panel intens, Panel initAction, Panel sop, TextBox textBoxMin, TextBox textBoxMax, Label labelMin, Label labelMax, PictureBox pbPrev, PictureBox pbNext, TextBox textBoxSMS, TextBox textBoxBroadcast, TextBox textBoxSOP, CheckBox checkBoxSMS, CheckBox checkBoxBroadcast, CheckBox checkBoxSOP, Button btnSOP)
            {
                m_labelTitle = title;
                m_panelIntens = intens;
                m_panelInitAction = initAction;
                m_panelSOP = sop;
                m_textBoxMin = textBoxMin;
                m_textBoxMax = textBoxMax;
                m_labelMin = labelMin;
                m_labelMax = labelMax;
                m_pbPrev = pbPrev;
                m_pbNext = pbNext;
                m_textBoxSMS = textBoxSMS;
                m_textBoxBroadcast = textBoxBroadcast;
                m_textBoxSOP = textBoxSOP;
                m_checkBoxSMS = checkBoxSMS;
                m_checkBoxBroadcast = checkBoxBroadcast;
                m_checkBoxSOP = checkBoxSOP;
                m_btnSOP = btnSOP;
                m_frmParent = frmParent;

                SetIntensOption(m_frmParent.GetTypeText(), textBoxMin, textBoxMax, labelMin, labelMax);

                m_btnSOP.Click += new System.EventHandler(btnSOP_Click);
                m_pbPrev.Click += new System.EventHandler(GoPrev);
                m_pbNext.Click += new System.EventHandler(GoNext);
            }

            private void GoPrev(object sender, EventArgs e)
            {
                m_intensOption = (EarthquakeOption.IntensOption)((int)m_intensOption - 1);
                SetRangeOptions();
            }

            private void GoNext(object sender, EventArgs e)
            {
                m_intensOption = (EarthquakeOption.IntensOption)((int)m_intensOption + 1);
                SetRangeOptions();
            }

            public bool SetIntensOption(string strType, TextBox textBoxMin, TextBox textBoxMax, Label labelMin, Label labelMax)
            {
                // 0 : 이하 또는 이상, 1 : 미만 또는 초과
                int minOption = 0, maxOption = 0;
                bool isIntensity = true;

                m_isValidate = false;
                m_intensOption = EarthquakeOption.IntensOption.NONE;

                if (strType == "진도")
                    isIntensity = true;
                else if (strType == "규모")
                    isIntensity = false;
                else
                    return false;

                if (textBoxMin.Visible == true && labelMin.Visible == true)
                {
                    if (GetMinMaxOption(true, labelMin.Text, ref minOption) == false)
                        return false;

                    if (textBoxMax.Visible == true && labelMax.Visible == true)
                    {
                        if (GetMinMaxOption(false, labelMax.Text, ref maxOption) == false)
                            return false;

                        if (minOption == 0 && maxOption == 1)
                        {
                            if (isIntensity)
                                m_intensOption = EarthquakeOption.IntensOption.I_MIN_GE_MAX_LT;
                            else
                                m_intensOption = EarthquakeOption.IntensOption.M_MIN_GE_MAX_LT;
                        }
                        else if (minOption == 1 && maxOption == 0)
                        {
                            if (isIntensity)
                                m_intensOption = EarthquakeOption.IntensOption.I_MIN_GT_MAX_LE;
                            else
                                m_intensOption = EarthquakeOption.IntensOption.M_MIN_GT_MAX_LE;
                        }
                        else
                            return false;
                    }
                    else if (textBoxMax.Visible == false && labelMax.Visible == false)
                    {
                        if (minOption == 0)
                        {
                            if (isIntensity)
                                m_intensOption = EarthquakeOption.IntensOption.I_MIN_LE;
                            else
                                m_intensOption = EarthquakeOption.IntensOption.M_MIN_LE;
                        }
                        else if (minOption == 1)
                        {
                            if (isIntensity)
                                m_intensOption = EarthquakeOption.IntensOption.I_MIN_LT;
                            else
                                m_intensOption = EarthquakeOption.IntensOption.M_MIN_LT;
                        }
                    }
                    else
                        return false;
                }
                else if (textBoxMin.Visible == false && labelMin.Visible == false)
                {
                    if (textBoxMax.Visible == true && labelMax.Visible == true)
                    {
                        if (GetMinMaxOption(false, labelMax.Text, ref maxOption) == false)
                            return false;

                        if (maxOption == 0)
                        {
                            if (isIntensity)
                                m_intensOption = EarthquakeOption.IntensOption.I_MAX_GE;
                            else
                                m_intensOption = EarthquakeOption.IntensOption.M_MAX_GE;
                        }
                        else if (maxOption == 1)
                        {
                            if (isIntensity)
                                m_intensOption = EarthquakeOption.IntensOption.I_MAX_GT;
                            else
                                m_intensOption = EarthquakeOption.IntensOption.M_MAX_GT;
                        }
                    }
                    else
                        return false;
                }
                else
                    return false;

                m_isValidate = true;
                return true;
            }

            // nResult : 0(이하 또는 이상), 1(미만 또는 초과)
            private bool GetMinMaxOption(bool isMinimum, string strText, ref int nResult)
            {
                if (isMinimum)
                {
                    if (strText == "이상")
                        nResult = 0;
                    else if (strText == "초과")
                        nResult = 1;
                    else
                        return false;
                }
                else
                {
                    if (strText == "이하")
                        nResult = 0;
                    else if (strText == "미만")
                        nResult = 1;
                    else
                        return false;
                }

                return true;
            }

            private void btnSOP_Click(object sender, EventArgs e)
            {
                PopupDialog.SOP.PopupSelectSOP selectSOP = new SOP.PopupSelectSOP();

                if (PageBackstageHome.ShowTranslucentSubForm(selectSOP) == System.Windows.Forms.DialogResult.OK)
                    m_textBoxSOP.Text = selectSOP.TargetSOP;
            }

            public void Init()
            {
                m_textBoxMin.Text = m_textBoxMax.Text = m_textBoxSMS.Text = m_textBoxBroadcast.Text = m_textBoxSOP.Text = "";
                m_checkBoxSMS.Checked = m_checkBoxBroadcast.Checked = m_checkBoxSOP.Checked = false;
            }

            public void SetData(double min, double max, string linkedSOP)
            {
                m_minIntens = min;
                m_maxIntens = max;
                m_strLinkedSOP = linkedSOP;
            }

            public string UpdateQuery(int nID)
            {
                string strFormat = "Update OptionEarthquake set MinIntens = {0:F1}, MaxIntens = {1:F1}, IntensOption = {2}, UseSMS = {3},";
                strFormat += "SMSMessage = '{4}', UseBroadcast = {5}, BroadcastMessage = '{6}', RunSOP = {7},";
                strFormat += "LinkedSOP = '{8}', SiteID = {9} where ID = {10}";

                string strSQL = string.Format(strFormat, m_minIntens, m_maxIntens, (int)m_intensOption,
                    m_checkBoxSMS.Checked ? 1 : 0, m_textBoxSMS.Text.Trim(),
                    m_checkBoxBroadcast.Checked ? 1 : 0, m_textBoxBroadcast.Text.Trim(),
                    m_checkBoxSOP.Checked ? 1 : 0, m_textBoxSOP.Text.Trim(),
                    UnE.SOP.ProxySOP.Instance.SiteID, nID);

                return strSQL;
            }

            public string InsertQuery(int nID)
            {
                string strFormat = "Insert into OptionEarthquake (ID, MinIntens,  MaxIntens, IntensOption, UseSMS, SMSMessage, ";
                strFormat += "UseBroadcast, BroadcastMessage, RunSOP, LinkedSOP, SiteID) values ({10}, {0:F1}, {1:F1}, {2}, ";
                strFormat += "{3}, '{4}', {5}, '{6}', {7}, '{8}', {9})";

                string strSQL = string.Format(strFormat, m_minIntens, m_maxIntens, (int)m_intensOption,
                    m_checkBoxSMS.Checked ? 1 : 0, m_textBoxSMS.Text.Trim(),
                    m_checkBoxBroadcast.Checked ? 1 : 0, m_textBoxBroadcast.Text.Trim(),
                    m_checkBoxSOP.Checked ? 1 : 0, m_textBoxSOP.Text.Trim(),
                    UnE.SOP.ProxySOP.Instance.SiteID, nID);

                return strSQL;
            }

            private void SetRangeVisible(bool isVisible)
            {
                if (isVisible == true)
                {
                    if (m_frmParent.IsEditable())
                    {
                        SetRangeOptions();
                    }
                    else
                    {
                        m_pbNext.Visible = m_pbPrev.Visible = false;
                    }
                }
            }

            public void SetRangeOptions()
            {
                if (m_intensOption == EarthquakeOption.IntensOption.I_MIN_GE_MAX_LT || m_intensOption == EarthquakeOption.IntensOption.M_MIN_GE_MAX_LT)
                {
                    m_labelMin.Text = "이상";
                    m_labelMax.Text = "미만";
                    m_textBoxMin.Visible = m_textBoxMax.Visible = m_labelMin.Visible = m_labelMax.Visible = true;

                    m_pbPrev.Visible = m_intensOption == EarthquakeOption.IntensOption.M_MIN_GE_MAX_LT;
                    m_pbNext.Visible = true;
                }
                else if (m_intensOption == EarthquakeOption.IntensOption.I_MIN_GT_MAX_LE || m_intensOption == EarthquakeOption.IntensOption.M_MIN_GT_MAX_LE)
                {
                    m_labelMin.Text = "초과";
                    m_labelMax.Text = "이하";
                    m_textBoxMin.Visible = m_textBoxMax.Visible = m_labelMin.Visible = m_labelMax.Visible = true;
                    m_pbPrev.Visible = m_pbNext.Visible = true;
                }
                else if (m_intensOption == EarthquakeOption.IntensOption.I_MIN_LT || m_intensOption == EarthquakeOption.IntensOption.M_MIN_LT)
                {
                    m_labelMin.Text = "미만";
                    m_textBoxMin.Visible = m_labelMin.Visible = true;
                    m_textBoxMax.Visible = m_labelMax.Visible = false;
                    m_pbPrev.Visible = m_pbNext.Visible = true;
                }
                else if (m_intensOption == EarthquakeOption.IntensOption.I_MIN_LE || m_intensOption == EarthquakeOption.IntensOption.M_MIN_LE)
                {
                    m_labelMin.Text = "이하";
                    m_textBoxMin.Visible = m_labelMin.Visible = true;
                    m_textBoxMax.Visible = m_labelMax.Visible = false;
                    m_pbPrev.Visible = m_pbNext.Visible = true;
                }
                else if (m_intensOption == EarthquakeOption.IntensOption.I_MAX_GT || m_intensOption == EarthquakeOption.IntensOption.M_MAX_GT)
                {
                    m_labelMax.Text = "초과";
                    m_textBoxMin.Visible = m_labelMin.Visible = false;
                    m_textBoxMax.Visible = m_labelMax.Visible = true;
                    m_pbPrev.Visible = m_pbNext.Visible = true;
                }
                else if (m_intensOption == EarthquakeOption.IntensOption.I_MAX_GE || m_intensOption == EarthquakeOption.IntensOption.M_MAX_GE)
                {
                    m_labelMax.Text = "이상";
                    m_textBoxMin.Visible = m_labelMin.Visible = false;
                    m_textBoxMax.Visible = m_labelMax.Visible = true;

                    m_pbPrev.Visible = true;
                    m_pbNext.Visible = m_intensOption == EarthquakeOption.IntensOption.I_MAX_GE;
                }
            }
        }

        private WebDBManager m_dbMgr = null;
        private Point m_ptTextBoxCenter = new Point();
        private Point m_ptTextBoxLeft = new Point();
        private Point m_ptTextBoxRight = new Point();
        private Point m_ptTextBoxLabel = new Point();
        private int m_nStepIndex = 0;

        private List<OptionGroup> m_optionGroups = new List<OptionGroup>();
        private UnE.Spatial.Shelter m_shelter = null;

        public FormEarthquakeOption(WebDBManager dbMgr)
        {
            InitializeComponent();
            m_dbMgr = dbMgr;

            m_ptTextBoxCenter = textBoxStep3Max.Location;
            m_ptTextBoxLeft = textBoxStep2Min.Location;
            m_ptTextBoxRight = textBoxStep2Max.Location;
            m_ptTextBoxLabel = new Point(labelStep3Max.Location.X - textBoxStep3Max.Location.X, labelStep3Max.Location.Y - textBoxStep3Max.Location.Y);
        }

        private void FormEarthquakeOption_Load(object sender, EventArgs e)
        {
            m_optionGroups.Add(new OptionGroup(this, labelStep1, panelStep1, panelStep1InitAction, panelStep1SOP, textBoxStep1Min, textBoxStep1Max, labelStep1Min, labelStep1Max, pbStep1Prev, pbStep1Next, textBoxStep1SMS, textBoxStep1Broadcast, textBoxStep1SOP, checkBoxStep1SMS, checkBoxStep1Broadcast, checkBoxStep1SOP, btnStep1SOP));
            m_optionGroups.Add(new OptionGroup(this, labelStep2, panelStep2, panelStep2InitAction, panelStep2SOP, textBoxStep2Min, textBoxStep2Max, labelStep2Min, labelStep2Max, pbStep2Prev, pbStep2Next, textBoxStep2SMS, textBoxStep2Broadcast, textBoxStep2SOP, checkBoxStep2SMS, checkBoxStep2Broadcast, checkBoxStep2SOP, btnStep2SOP));
            m_optionGroups.Add(new OptionGroup(this, labelStep3, panelStep3, panelStep3InitAction, panelStep3SOP, textBoxStep3Min, textBoxStep3Max, labelStep3Min, labelStep3Max, pbStep3Prev, pbStep3Next, textBoxStep3SMS, textBoxStep3Broadcast, textBoxStep3SOP, checkBoxStep3SMS, checkBoxStep3Broadcast, checkBoxStep3SOP, btnStep3SOP));

            LoadOptions();
            LoadShelter();

            CheckSteps();
        }

        private void CheckSteps()
        {
            int nOptionCount = m_optionGroups.Count;
            int nVisibleCount = 0;

            for (int i=0;i<nOptionCount;i++)
            {
                if (m_optionGroups[i].Visible)
                    nVisibleCount++;
            }

            btnDecreaseStep.Visible = nVisibleCount > 1;
            btnIncreaseStep.Visible = nVisibleCount < 3;
        }

        private void LoadShelter()
        {
            Dictionary<int, UnE.Spatial.Shelter> dicShelters = UnE.Spatial.ZoneManager.Instance.GetShelters(UnE.Spatial.Shelter.ShelterTypes.Earthquake);

            if (dicShelters == null)
                return;

            foreach (KeyValuePair<int, UnE.Spatial.Shelter> pair in dicShelters)
            {
                m_shelter = pair.Value;
                textBoxShelter.Text = m_shelter.ShelterName;
                break;
            }
        }

        private void LoadOptions()
        {
            List<EarthquakeOption> options = EarthquakeOption.LoadOptions(m_dbMgr);

            if (options == null)
                return;

            SetDatas(options);

            //foreach (OptionGroup optionGroup in m_optionGroups)
            //{
            //    optionGroup.Editable = checkBoxEdit.Checked;
            //}
        }

        private void SetDatas(List<EarthquakeOption> options)
        {
            foreach (OptionGroup optionGroup in m_optionGroups)
            {
                optionGroup.Init();
            }

            if (options.Count == 0)
            {
                for (int i = 1; i < m_optionGroups.Count;i++ )
                {
                    OptionGroup optionGroup = m_optionGroups[i];
                    optionGroup.Visible = false;
                }

                return;
            }
            else
            {
                for (int i = 0; i < m_optionGroups.Count; i++)
                {
                    OptionGroup optionGroup = m_optionGroups[i];
                    optionGroup.Visible = false;
                }
            }

            if (options[0].IsIntensity == false)
                labelIntensity.Text = "규모";
            else
                labelIntensity.Text = "진도";

            for (int i=m_nStepIndex;i<options.Count && i < m_nStepIndex + 3;i++)
            {
                EarthquakeOption opt = options[i];
                OptionGroup ui = m_optionGroups[i - m_nStepIndex];
                ui.IntensOption = opt.MinMaxOption;
                ui.Visible = true;
                ui.SetRangeOptions();

                if (opt.BothMinMax)
                {
                    ui.MinIntens.Location = m_ptTextBoxLeft;
                    ui.MinIntensLabel.Location = new Point(ui.MinIntens.Location.X + m_ptTextBoxLabel.X, ui.MinIntens.Location.Y + m_ptTextBoxLabel.Y);
                    ui.MaxIntens.Location = m_ptTextBoxRight;
                    ui.MaxIntensLabel.Location = new Point(ui.MaxIntens.Location.X + m_ptTextBoxLabel.X, ui.MaxIntens.Location.Y + m_ptTextBoxLabel.Y);
                    ui.MinIntens.Visible = ui.MaxIntens.Visible = ui.MinIntensLabel.Visible = ui.MaxIntensLabel.Visible = true;

                    ui.MinIntens.Text = string.Format("{0:F1}", opt.Minimum);
                    ui.MaxIntens.Text = string.Format("{0:F1}", opt.Maximum);
                }
                else if (opt.OnlyMin)
                {
                    ui.MinIntens.Location = m_ptTextBoxCenter;
                    ui.MinIntensLabel.Location = new Point(ui.MinIntens.Location.X + m_ptTextBoxLabel.X, ui.MinIntens.Location.Y + m_ptTextBoxLabel.Y);
                    ui.MinIntens.Visible = ui.MinIntensLabel.Visible = true;
                    ui.MaxIntens.Visible = ui.MaxIntensLabel.Visible = false;

                    ui.MinIntens.Text = string.Format("{0:F1}", opt.Minimum);
                }
                else if (opt.OnlyMax)
                {
                    ui.MaxIntens.Location = m_ptTextBoxCenter;
                    ui.MaxIntensLabel.Location = new Point(ui.MaxIntens.Location.X + m_ptTextBoxLabel.X, ui.MaxIntens.Location.Y + m_ptTextBoxLabel.Y);
                    ui.MinIntens.Visible = ui.MinIntensLabel.Visible = false;
                    ui.MaxIntens.Visible = ui.MaxIntensLabel.Visible = true;

                    ui.MaxIntens.Text = string.Format("{0:F1}", opt.Maximum);
                }

                ui.UseSMS.Checked = opt.UseSMS;
                ui.UseBroadcast.Checked = opt.UseBroadcast;
                ui.RunSOP.Checked = opt.RunSOP;

                ui.SMSMessage.Text = opt.SMSMessage;
                ui.BroadcastMessage.Text = opt.BroadcastMessage;
                ui.LinkedSOP.Text = opt.LinkedSOP;
            }
        }

        private void btnIncreaseStep_Click(object sender, EventArgs e)
        {
            int nOptionCount = m_optionGroups.Count;

            for (int i=0;i<nOptionCount;i++)
            {
                if (m_optionGroups[i].Visible == false)
                {
                    string strMessage = m_optionGroups[i].Title.Text + "를 추가하시겠습니까?";

                    if (MessageBox.Show(strMessage, "확인", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {
                        m_optionGroups[i].Visible = true;
                        CheckSteps();
                    }

                    break;
                }
            }
        }

        private void btnDecreaseStep_Click(object sender, EventArgs e)
        {
            int nOptionCount = m_optionGroups.Count;

            for (int i = nOptionCount - 1; i >= 0; i--)
            {
                if (m_optionGroups[i].Visible == true)
                {
                    string strMessage = m_optionGroups[i].Title.Text + "를 삭제하시겠습니까?";

                    if (MessageBox.Show(strMessage, "확인", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {
                        m_optionGroups[i].Visible = false;
                        CheckSteps();
                    }

                    break;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (CheckValid() == false)
                return;

            string strSQL = "Select ID from OptionEarthquake where SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nIndex = 0;
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount;i++)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());

                if (id == null)
                    continue;

                if (m_optionGroups.Count > nIndex)
                {
                    OptionGroup option = m_optionGroups[nIndex];

                    strSQL = option.UpdateQuery(id.Data);

                    if (m_dbMgr.GetResultData(strSQL, 0) != null)
                        nIndex++;
                }
            }

            for (int i=nIndex;i<m_optionGroups.Count;i++)
            {
                OptionGroup option = m_optionGroups[i];

                if (option.Visible)
                {
                    strSQL = option.InsertQuery(i + 1);
                    m_dbMgr.GetResultData(strSQL, 0);
                }
            }

            if (SaveShelter())
            {
                // 대피소 정보가 바뀌었으니 새로 읽어들이도록 한다.
                UnE.Spatial.ZoneManager.Instance.LoadShelters();
            }

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private bool SaveShelter()
        {
            string strSQL = "";
            string strShelterName = textBoxShelter.Text.Trim();

            if (m_shelter == null)
            {
                if (strShelterName.Length == 0)
                    return false;

                int nID = GetMaxID("Shelter") + 1;

                string strFormat = "Insert into Shelter (ID, ShelterName, ShelterType, ShelterIDType, ShelterID, Boundary, SiteID, Description) ";
                strFormat += "values ({0}, '{1}', {2}, {3}, NULL, NULL, {4}, NULL)";

                strSQL = string.Format(strFormat, nID, strShelterName, (int)UnE.Spatial.Shelter.ShelterTypes.Earthquake, 
                    (int)UnE.Spatial.Shelter.ShelterIDTypes.None, UnE.SOP.ProxySOP.Instance.SiteID);
            }
            else
            {
                if (strShelterName.Length == 0)
                    strSQL = "Delete from Shelter where ID = " + m_shelter.ID.ToString();
                else
                    strSQL = string.Format("Update Shelter set ShelterName = '{0}' where ID = {1}", m_shelter.ShelterName, m_shelter.ID); 
            }

            return m_dbMgr.GetResultData(strSQL, 0) != null;
        }

        private int GetMaxID(string strTableName)
        {
            string strSQL = "Select max(ID) from " + strTableName;
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return 0;

            DBUtility.VariousData<int> maxID = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString());
            return maxID == null ? 0 : maxID.Data;
        }

        private bool CheckValid()
        {
            if (m_optionGroups.Count > 0)
            {
                OptionGroup lastOption = m_optionGroups[m_optionGroups.Count - 1];

                foreach (OptionGroup optionGroup in m_optionGroups)
                {
                    if (optionGroup.Visible == false)
                        continue;

                    double min, max;
                    bool minResult = GetDouble(optionGroup.MinIntens, out min);
                    bool maxResult = GetDouble(optionGroup.MaxIntens, out max);

                    if (maxResult == false || max <= 0.0)
                    {
                        optionGroup.MaxIntens.Focus();
                        MessageBox.Show("진도값은 0보다 큰 숫자이어야만 합니다.");
                        return false;
                    }

                    if (optionGroup != lastOption && (minResult == false || min <= 0.0))
                    {
                        optionGroup.MinIntens.Focus();
                        MessageBox.Show("진도값은 0보다 큰 숫자이어야만 합니다.");
                        return false;
                    }

                    string strSOP;
                    
                    if (CheckValidSOP(optionGroup.LinkedSOP, out strSOP) == false)
                        return false;

                    optionGroup.SetData(min, max, strSOP);
                }
            }

            return true;
        }

        private bool CheckValidSOP(TextBox text, out string strSOP)
        {
            strSOP = "NULL";
            string strPath = text.Text.Trim();

            if (strPath.Length == 0)
                return true;

            // SOP가 ''로 감싸여 있을경우 이를 제거한다.
            if (strPath.StartsWith("'"))
                strPath = strPath.Substring(1);

            if (strPath.EndsWith("'"))
                strPath = strPath.Substring(0, strPath.Length - 1);

            string[] tokens = strPath.Split('/');

            if (tokens.Count() < 3)
            {
                text.Focus();
                MessageBox.Show("'SOP는 카테고리/하부카테고리/SOP이름'의 형식으로 표기되어야 합니다.");
                return false;
            }

            string strSQL = "Select ID from DisasterCategory where SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString() + " and CategoryName = '" + tokens[0].Trim() + "'";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
            {
                MessageBox.Show("DB에 접속할 수 없습니다.");
                return false;
            }

            if (arrResult.Count == 0)
            {
                text.Focus();
                MessageBox.Show("'" + tokens[0].Trim() + "'는 존재하지 않는 카테고리입니다.");
                return false;
            }

            int nDisasterCategoryID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);

            strSQL = "Select ID from SubDisasterCategory where DisasterID = " + nDisasterCategoryID.ToString() + " and SubCategoryName = '" + tokens[1].Trim() + "'";
            arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
            {
                MessageBox.Show("DB에 접속할 수 없습니다.");
                return false;
            }

            if (arrResult.Count == 0)
            {
                text.Focus();
                MessageBox.Show("'" + tokens[0] + "/" + tokens[0] + "'는 유효하지 않은 경로입니다.");
                return false;
            }

            int nSubCategoryID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);

            strSQL = "Select ID from Disaster where SubDisasterID = " + nSubCategoryID.ToString() + " and DisasterName = '" + tokens[2].Trim() + "'";
            arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
            {
                MessageBox.Show("DB에 접속할 수 없습니다.");
                return false;
            }

            if (arrResult.Count == 0)
            {
                text.Focus();
                MessageBox.Show("'" + strPath + "'는 유효하지 않은 경로입니다.");
                return false;
            }

            strSOP = "'" + tokens[0].Trim() + "/" + tokens[1].Trim() + "/" + tokens[2].Trim() + "'";
            return true;
        }

        private bool GetDouble(TextBox text, out double data)
        {
            string str = text.Text.Trim();

            if (double.TryParse(str, out data) == false)
                return false;

            return true;
        }

        private void checkBoxEdit_CheckedChanged(object sender, EventArgs e)
        {
            foreach (OptionGroup option in m_optionGroups)
            {
                if (option.Visible)
                    option.Visible = true;
            }

            btnDecreaseStep.Enabled = btnIncreaseStep.Enabled = checkBoxEdit.Checked;
        }

        public string GetTypeText()
        {
            return labelIntensity.Text;
        }

        public bool IsEditable()
        {
            return checkBoxEdit.Checked;
        }
    }
}
