using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sections;

namespace SOPManager.Popup
{
    public partial class PopupNoteDecision2 : Form
    {
        private static PopupNoteDecision2 m_instance = null;

        private PropertiesDecision m_prop = null;
        private Section m_propSection = null;

        private bool m_isChanged;
        //private string m_strUserDefinedConfigName = "";
        private string m_strCategoryName = "";
        private string m_strSubCategoryName = "";

        public PropertiesDecision PropertiesDecision
        {
            get { return m_prop; }
            set
            {
                Section section = value == null ? null : value.GetSection();

                if (m_propSection != section)
                {
                    m_prop = value;
                    Init();
                }
            }
        }

        public bool IsChanged
        {
            get
            {
                return m_isChanged;
            }
        }

        private double WindowRateWidth;
        private double WindowRateHeight;

        public PopupNoteDecision2(string strCategoryName, string strSubCategoryName)
        {
            InitializeComponent();

            m_strCategoryName = strCategoryName;
            m_strSubCategoryName = strSubCategoryName;

            Double[] dWindowRate = FormMain.Instance.GetCurWindowRate();
            WindowRateWidth = dWindowRate[0];
            WindowRateHeight = dWindowRate[1];

            UpdateControlSize();
        }

        public void UpdateControlSize()
        {
            HaveControl(this, WindowRateWidth, WindowRateHeight);
        }

        private void HaveControl(Control pctl, double WindowRateWidth, double WindowRateHeight)
        {
            foreach (Control ctl in pctl.Controls)
            {
                if (ctl.Controls.Count > 0)
                    HaveControl(ctl, WindowRateWidth, WindowRateHeight);

                FormMain.Instance.UpdateWindowRate(ctl, WindowRateWidth, WindowRateHeight, Program.prgFont);
            }
        }

        private void Init()
        {
            if (m_prop == null)
                return;

            Section section = m_prop.GetSection();

            if (section == null)
                return;

            m_propSection = section;

            textBox.Text = section.Title;

            SectionDataDecision data = (SectionDataDecision)section.Data;
            textBoxExpression.Text = data.Expression;

            gridSystemType.Rows.Clear();

            List<SOPParameter> systemParameters = PopupSpecialMessage.GetSystemParameters(m_strCategoryName, m_strSubCategoryName);

            foreach (SOPParameter param in systemParameters)
            {
                AddVariable(gridSystemType, param);
            }

            gridUserType.Rows.Clear();

            ConfigData config = null;
            List<SOPParameter> userParameters = FormMain.Instance.GetPageLevel().GetBarConfig().GetCurrentVariables(out config);
            //List<SOPParameter> userParameters = FormMain.Instance.GetPageLevel().GetBarConfig().GetCurrentVariables(out m_strUserDefinedConfigName);

            if (userParameters != null)
            {
                foreach (SOPParameter param in userParameters)
                {
                    AddVariable(gridUserType, param);
                }
            }

            checkBoxExpression.Checked = data.Expression.Length > 0;
            checkBoxExpression_CheckedChanged(null, null);
            UpdateAutoRun();
        }

        private void UpdateAutoRun()
        {
            if (checkBoxExpression.Checked == true)
            {
                picAutoRun.BackgroundImage = global::SOPManager.Properties.Resources.__COMMON_ckb_enable;
            }
            else
            {
                picAutoRun.BackgroundImage = global::SOPManager.Properties.Resources.__COMMON_ckb_disable;
            }
        }

        private void AddVariable(DataGridView grid, SOPParameter param)
        {
            int nRowIndex = grid.Rows.Add();
            DataGridViewRow row = grid.Rows[nRowIndex];

            row.Cells[0].Value = "{" + param.VariableName + "}";
            row.Cells[1].Value = Sections.SectionDataDecision.GetVariableTypeName(param.Type);
            row.Cells[2].Value = param.Description;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void checkBoxExpression_CheckedChanged(object sender, EventArgs e)
        {
            labelExpression.Visible = labelType.Visible = textBoxExpression.Visible = gridSystemType.Visible = gridUserType.Visible = checkBoxExpression.Checked;

            //if (checkBoxExpression.Checked == false)
            //{
            //    this.Size = new Size(this.Size.Width, 301);
            //    textBox.Size = new Size(this.Size.Width, 229);
            //}
            //else
            //{
            //    this.Size = new Size(this.Size.Width, 635);
            //    textBox.Size = new Size(this.Size.Width, 111);
            //}

            if (checkBoxExpression.Checked == false)
            {
                this.Size = new Size((int)(433 * WindowRateWidth), (int)(301 * WindowRateHeight));
                textBox.Size = new Size((int)(433 * WindowRateWidth), (int)(252 * WindowRateHeight));

                btnShowSpecialMessage.Location = new Point(0, (int)(264 * WindowRateHeight));
                picAutoRun.Location = new Point((int)(116 * WindowRateWidth), (int)(273 * WindowRateHeight));
                lblAutoRun.Location = new Point((int)(136 * WindowRateWidth), (int)(275 * WindowRateHeight));
                btnOK.Location = new Point((int)(301 * WindowRateWidth), (int)(264 * WindowRateHeight));
                btnCancel.Location = new Point((int)(368 * WindowRateWidth), (int)(264 * WindowRateHeight));
            }
            else
            {
                this.Size = new Size((int)(433 * WindowRateWidth), (int)(625 * WindowRateHeight));
                textBox.Size = new Size((int)(433 * WindowRateWidth), (int)(111 * WindowRateHeight));

                btnShowSpecialMessage.Location = new Point(0, (int)(585 * WindowRateHeight));
                picAutoRun.Location = new Point((int)(116 * WindowRateWidth), (int)(594 * WindowRateHeight));
                lblAutoRun.Location = new Point((int)(136 * WindowRateWidth), (int)(595 * WindowRateHeight));
                btnOK.Location = new Point((int)(301 * WindowRateWidth), (int)(585 * WindowRateHeight));
                btnCancel.Location = new Point((int)(368 * WindowRateWidth), (int)(585 * WindowRateHeight));
            }

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!SaveData())
                return;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public bool SaveData()
        {
            if (m_prop != null)
            {
                Section section = m_prop.GetSection();

                if (section == null && m_propSection != null && m_prop.ID == m_propSection.Data.ComponentID)
                {
                    m_prop.SetData(m_propSection);
                    section = m_propSection;
                }

                if (section != null && section is SectionDecision)
                {
                    if (m_prop.Text != textBox.Text)
                        m_isChanged = true;

                    if (m_isChanged == false)
                    {
                        if (m_prop.Expression.Length == 0 && checkBoxExpression.Checked && textBoxExpression.Text.Length > 0)
                            m_isChanged = true;
                        else if (m_prop.Expression.Length > 0 && (checkBoxExpression.Checked == false || textBoxExpression.Text != m_prop.Expression))
                            m_isChanged = true;
                    }

                    SectionDataDecision data = (SectionDataDecision)section.Data;

                    if (m_isChanged)
                    {
                        m_prop.Text = textBox.Text;

                        if (checkBoxExpression.Checked)
                        {
                            if (CheckExpression(textBoxExpression.Text))
                                m_prop.Expression = textBoxExpression.Text;
                            else
                                return false;
                        }
                        else
                            m_prop.Expression = "";

                        data.VariableTypes.Clear();

                        Dictionary<string, Sections.SectionDataDecision.VariableType> dicVariableTypes = GetUsingVariableTypes(m_prop.Expression);
                        
                        foreach (KeyValuePair<string, Sections.SectionDataDecision.VariableType> pair in dicVariableTypes)
                        {
                            data.VariableTypes["{" + pair.Key + "}"] = pair.Value;
                        }
                    }
                }
            }

            return true;
        }

        private bool CheckExpression(string strExpression, bool popupErrorMessage = true)
        {
            Dictionary<string, Sections.SectionDataDecision.VariableType> dicVariables = new Dictionary<string, SectionDataDecision.VariableType>();
            int nIndex = strExpression.IndexOf('{');

            while (nIndex >= 0)
            {
                int nIndex2 = strExpression.IndexOf('}', nIndex + 1);

                if (nIndex2 < 0)
                {
                    if (popupErrorMessage)
                    {
                        FocusExpression(nIndex + 1);
                        MessageBox.Show("변수의 닫는 중괄호('}')가 빠졌습니다.");
                    }
                    else
                        System.Diagnostics.Trace.WriteLine("변수의 닫는 중괄호('}')가 빠졌습니다.");

                    return false;
                }

                string strVariableName = strExpression.Substring(nIndex + 1, nIndex2 - nIndex - 1).Trim();
                string strVariableNameLow = "{" + strVariableName.ToLower() + "}";

                Sections.SectionDataDecision.VariableType type = SectionDataDecision.VariableType.UNKNOWN;

                if (dicVariables.TryGetValue(strVariableNameLow, out type) == false)
                {
                    type = GetVariableType(strVariableName);

                    if (type == SectionDataDecision.VariableType.UNKNOWN)
                    {
                        if (popupErrorMessage)
                        {
                            FocusExpression(nIndex + 1, nIndex2 - nIndex - 1);
                            MessageBox.Show(strVariableName + "는 존재하지 않는 변수입니다.");
                        }
                        else
                            System.Diagnostics.Trace.WriteLine(strVariableName + "는 존재하지 않는 변수입니다.");

                        return false;
                    }

                    dicVariables[strVariableNameLow] = type;
                }

                nIndex = strExpression.IndexOf('{', nIndex2 + 1);
            }

            string strError;

            SOPMonitoringSystem.DecisionDataHelper.IsValidExpression(strExpression, dicVariables, out strError);
            if (strError.Length > 0)
            { 
                if (popupErrorMessage)
                {
                    textBoxExpression.Focus();
                    MessageBox.Show(strError);
                }
                else
                    System.Diagnostics.Trace.WriteLine(strError);

                return false;
            }

            return true;
        }

        private void FocusExpression(int nBeginIndex, int nLength = -1)
        {
            textBoxExpression.SelectionStart = nBeginIndex;

            if (nLength > 0)
            {
                textBoxExpression.SelectionLength = nLength;
            }

            textBoxExpression.Focus();
        }

        private Dictionary<string, Sections.SectionDataDecision.VariableType> GetUsingVariableTypes(string strExpression)
        {
            Dictionary<string, Sections.SectionDataDecision.VariableType> dicVariableTypes = new Dictionary<string,SectionDataDecision.VariableType>();
            int nIndex = strExpression.IndexOf('{');

            Sections.SectionDataDecision.VariableType type;

            while (nIndex >= 0)
            {
                int nIndex2 = strExpression.IndexOf('}', nIndex + 1);

                if (nIndex2 < 0)
                    break;

                string strVariable = strExpression.Substring(nIndex + 1, nIndex2 - nIndex - 1).Trim();

                if (dicVariableTypes.TryGetValue(strVariable, out type) == false)
                {
                    type = GetVariableType(strVariable);

                    if (type != SectionDataDecision.VariableType.UNKNOWN)
                        dicVariableTypes[strVariable] = type;
                }

                nIndex = strExpression.IndexOf('{', nIndex2 + 1);
            }

            return dicVariableTypes;
        }

        private Sections.SectionDataDecision.VariableType GetVariableType(string strVariableName)
        {
            // 변수명이 시스템과 사용자 정의타입에 같은 이름으로 존재할 경우, 사용자 정의타입에 있는 것을 사용하도록 한다.
            Sections.SectionDataDecision.VariableType type = GetVariableType(strVariableName, gridUserType);

            if (type == SectionDataDecision.VariableType.UNKNOWN)
                type = GetVariableType(strVariableName, gridSystemType);

            return type;
        }

        private Sections.SectionDataDecision.VariableType GetVariableType(string strVariableName, DataGridView grid)
        {
            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.IsNewRow)
                    continue;

                string strValue = row.Cells[0].Value.ToString();
                strValue = strValue.Substring(1);
                strValue = strValue.Substring(0, strValue.Length - 1);

                if (string.Compare(strValue, strVariableName, true) == 0)
                {
                    return Sections.SectionDataDecision.ToVariableType(row.Cells[1].Value.ToString());
                }
            }

            return SectionDataDecision.VariableType.UNKNOWN;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnShowSpecialMessage_Click(object sender, EventArgs e)
        {
            FormMain.Instance.ShowSpecialMessage();
        }

        private void Expression_Click(object sender, EventArgs e)
        {
            checkBoxExpression.Checked = !checkBoxExpression.Checked;
            UpdateAutoRun();
            checkBoxExpression_CheckedChanged(null, null);
        }

        private void gridType_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != 0)
                return;

            DataGridView grid = (DataGridView)sender;
            DataGridViewRow row = grid.Rows[e.RowIndex];

            if (row.IsNewRow)
                return;

            textBoxExpression.Paste(row.Cells[e.ColumnIndex].Value.ToString());
            textBoxExpression.Focus();
        }

        private void PopupNoteDecision2_Load(object sender, EventArgs e)
        {
            m_instance = this;
        }

        private void PopupNoteDecision2_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_instance = null;
        }

        public static void ChangeUserDefinedVariables(List<SOPParameter> parameters)
        {
            if (m_instance == null)
                return;

            m_instance.gridUserType.Rows.Clear();

            if (parameters != null)
            {
                foreach (SOPParameter param in parameters)
                {
                    m_instance.AddVariable(m_instance.gridUserType, param);
                }
            }
        }
    }
}
