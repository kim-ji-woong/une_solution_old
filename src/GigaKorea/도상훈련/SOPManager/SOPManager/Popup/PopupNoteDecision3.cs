using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sections;
using SOPManager.Popup.CreateFormulas;
//using static Sections.SectionDataDecision;

namespace SOPManager.Popup
{
    public partial class PopupNoteDecision3 : Form
    {
        private static PopupNoteDecision3 m_instance = null;
        public static PopupNoteDecision3 Instance { get { return m_instance; } }

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

        public bool IsChanged { get { return m_isChanged; } }

        private double WindowRateWidth;
        private double WindowRateHeight;

        #region CreateFormula

        private Dictionary<Sections.SectionDataDecision.VariableType, List<CustomComboBoxItem>> m_division = new Dictionary<Sections.SectionDataDecision.VariableType, List<CustomComboBoxItem>>();

        // 수식, 논리어(||, &&)의 배열 (수식은 key가 홀수, 논리어는 key가 짝수)
        // ex : (a > b && b < c) || a == c 라는 수식일때
        // 1, a > b
        // 2, &&
        // 3, b < c
        // 4, ||
        // 5, a == c
        private Dictionary<int, string> m_dicTemp = new Dictionary<int, string>();

        // 하나의 괄호단위가 m_dicTemp의 key로 입력
        // ex : (a > b && b < c) || a == c 라는 수식일때
        // m_temp[0] : 1,2,3 (a > b && b < c)
        // m_temp[1] : 4 ||
        // m_temp[2] : 5 a == c
        private List<string> m_temp = new List<string>();

        private int m_nPnSubIndex = 1;
        private OneFormula m_ClickFormula = null;
        private List<OneFormula> m_ClickFormulas = new List<OneFormula>();
        private Panel m_ClickSubPanel = null;
        private bool m_bClickSubPanelMove = false; // 선택된 SubPanel의 길이가 pnMain보다 길어서 이동했는지 여부

        #endregion

        #region 초기화
        public PopupNoteDecision3(string strCategoryName, string strSubCategoryName)
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            SetDoubleBuffer(pnMain, true);
            m_penClick.Width = 3;

            m_strCategoryName = strCategoryName;
            m_strSubCategoryName = strSubCategoryName;

            Double[] dWindowRate = FormMain.Instance.GetCurWindowRate();
            WindowRateWidth = dWindowRate[0];
            WindowRateHeight = dWindowRate[1];

            UpdateControlSize();
        }

        private void PopupNoteDecision3_Load(object sender, EventArgs e)
        {
            m_instance = this;

            InitDivision();
            InitPanel();            
        }

        private void PopupNoteDecision3_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_instance = null;
        }

        public void SetDoubleBuffer(Panel panel, bool bEnabled)
        {
            Type dgvType1 = panel.GetType();
            PropertyInfo pi1 = dgvType1.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi1.SetValue(panel, bEnabled, null);
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
            m_strExpression = data.Expression;

            gridSystemType.Rows.Clear();

            List<SOPParameter> systemParameters = PopupSpecialMessage.GetSystemParameters(m_strCategoryName, m_strSubCategoryName);

            foreach (SOPParameter param in systemParameters)
            {
                AddVariable(gridSystemType, param);
            }

            DisplayUserType();

            checkBoxExpression.Checked = data.Expression.Length > 0;
            checkBoxExpression_CheckedChanged(null, null);
            UpdateAutoRun();
        }

        public void DisplayUserType()
        {
            ConfigData config = null;
            List<SOPParameter> userParameters = FormMain.Instance.GetPageLevel().GetBarConfig().GetCurrentVariables(out config);
            //List<SOPParameter> userParameters = FormMain.Instance.GetPageLevel().GetBarConfig().GetCurrentVariables(out m_strUserDefinedConfigName);

            if (userParameters != null)
            {
                gridUserType.Rows.Clear();
                foreach (SOPParameter param in userParameters)
                {
                    AddVariable(gridUserType, param);
                }
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
        #endregion

        #region 4K, FullHD Resize
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

                FormMain.Instance.UpdateWindowRate(ctl, WindowRateWidth, WindowRateHeight, "나눔스퀘어");
            }
        }
        #endregion
        private bool m_bDownCtrlKey = false;
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            else if (keyData == Keys.Right || keyData == Keys.Left)
            {
                MoveChild(keyData);
            }
            else if (keyData == Keys.Control)
            {
                m_bDownCtrlKey = !m_bDownCtrlKey;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        
        #region 수식사용 체크박스 이벤트
        private void checkBoxExpression_CheckedChanged(object sender, EventArgs e)
        {
            labelType.Visible = gridSystemType.Visible = gridUserType.Visible = checkBoxExpression.Checked;

            if (checkBoxExpression.Checked == false)
            {
                this.Size = new Size((int)(432 * WindowRateWidth), (int)(179 * WindowRateHeight));

                btnShowSpecialMessage.Location = new Point(0, (int)(143 * WindowRateHeight)); //특수문자 옵션
                picAutoRun.Location = new Point((int)(120 * WindowRateWidth), (int)(149 * WindowRateHeight)); // 수식사용 체크박스
                lblAutoRun.Location = new Point((int)(140 * WindowRateWidth), (int)(150 * WindowRateHeight)); // 수식사용
                btnOK.Location = new Point((int)(300 * WindowRateWidth), (int)(143 * WindowRateHeight)); // 확인
                btnCancel.Location = new Point((int)(367 * WindowRateWidth), (int)(143 * WindowRateHeight)); // 취소
            }
            else
            {
                this.Size = new Size((int)(1186 * WindowRateWidth), (int)(488 * WindowRateHeight));

                btnShowSpecialMessage.Location = new Point(0, (int)(449 * WindowRateHeight)); //특수문자 옵션
                picAutoRun.Location = new Point((int)(120 * WindowRateWidth), (int)(455 * WindowRateHeight)); // 수식사용 체크박스
                lblAutoRun.Location = new Point((int)(140 * WindowRateWidth), (int)(456 * WindowRateHeight)); // 수식사용
                btnOK.Location = new Point((int)(300 * WindowRateWidth), (int)(449 * WindowRateHeight)); // 확인
                btnCancel.Location = new Point((int)(367 * WindowRateWidth), (int)(449 * WindowRateHeight)); // 취소
            }

            UpdateAutoRun();
        }

        private void UpdateAutoRun()
        {
            if (checkBoxExpression.Checked == true)
                picAutoRun.BackgroundImage = global::SOPManager.Properties.Resources.__COMMON_ckb_enable;
            else
                picAutoRun.BackgroundImage = global::SOPManager.Properties.Resources.__COMMON_ckb_disable;
        }

        private void Expression_Click(object sender, EventArgs e)
        {
            checkBoxExpression.Checked = !checkBoxExpression.Checked;
            checkBoxExpression_CheckedChanged(null, null);
        }
        #endregion

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
                        if (m_prop.Expression.Length == 0 && checkBoxExpression.Checked && m_strExpression.Length > 0)
                            m_isChanged = true;
                        else if (m_prop.Expression.Length > 0 && (checkBoxExpression.Checked == false || m_strExpression != m_prop.Expression))
                            m_isChanged = true;
                    }

                    SectionDataDecision data = (SectionDataDecision)section.Data;

                    if (m_isChanged)
                    {
                        m_prop.Text = textBox.Text;

                        if (checkBoxExpression.Checked && m_strExpression.Length > 0)
                        {
                            if (CheckExpression(m_strExpression))
                                m_prop.Expression = m_strExpression;
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

        public bool CheckExpression(string strExpression, bool popupErrorMessage = true)
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
                    MessageBox.Show(strError);
                }
                else
                    System.Diagnostics.Trace.WriteLine(strError);

                return false;
            }

            return true;
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

        private DataGridViewRow GetVariableType(string strVariableName, ref SectionDataDecision.VariableType type)
        {
            // 변수명이 시스템과 사용자 정의타입에 같은 이름으로 존재할 경우, 사용자 정의타입에 있는 것을 사용하도록 한다.
            DataGridViewRow row = GetVariableType(strVariableName, gridUserType, ref type);

            if (type == SectionDataDecision.VariableType.UNKNOWN || row == null)
                row = GetVariableType(strVariableName, gridSystemType, ref type);

            return row;
        }

        private DataGridViewRow GetVariableType(string strVariableName, DataGridView grid, ref SectionDataDecision.VariableType type)
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
                    type = Sections.SectionDataDecision.ToVariableType(row.Cells[1].Value.ToString());
                    return row;
                }
            }

            return null;
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

        #region 버튼 이벤트
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!SaveData())
                return;

            this.DialogResult = DialogResult.OK;
            this.Close();
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

        private void btnHighRank_Click(object sender, EventArgs e)
        {
            string orgExpression = m_strExpression;
            PopupNoteDecision3_HighRank pop = new PopupNoteDecision3_HighRank(orgExpression, m_strCategoryName, m_strSubCategoryName);
            UnE.GUI.DialogFormFrameRibbon popup = new UnE.GUI.DialogFormFrameRibbon(pop);
            popup.StartPosition = FormStartPosition.CenterParent;
            popup.TopMost = true;
            if (popup.ShowDialog() == DialogResult.Yes)
            {
                if (pop.Value != orgExpression)
                {
                    m_strExpression = pop.Value;
                    
                    InitPanel();
                }
            }
        }
        #endregion

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

        private string m_strExpression = "";
        public void InitPanel()
        {
            if (checkBoxExpression.Checked)
            {
                pnMain.Controls.Clear();                
                m_nPnSubIndex = 1;
                
                SetDicExpression();

                int beginX = 0;
                int beginY = 0;

                for (int i = 0; i < m_temp.Count; i++)
                {
                    if (i % 2 == 0)
                    {
                        Panel pnSub = AddSubPanel(beginX, beginY);
                        
                        int beginSubX = m_nSpace;
                        string expressionNumber = m_temp[i];
                        string[] expressionNumber2 = expressionNumber.Split(',');
                        for (int j = 0; j < expressionNumber2.Length; j++)
                        {
                            if (j % 2 == 0)
                            {
                                OneFormula formula = AddOneFormula();
                                formula.Location = new Point(beginSubX, pnSub.Height / 2 - formula.Height / 2);
                                formula.Parent = pnSub;

                                if (!m_dicTemp.ContainsKey(Convert.ToInt32(expressionNumber2[j])))
                                    continue;

                                string expression = m_dicTemp[Convert.ToInt32(expressionNumber2[j])];

                                SetFormula(formula, expression);

                                beginSubX += formula.Width;
                            }
                            else
                            {
                                LogicalTerm logical = AddLogicalTerm();
                                logical.Location = new Point(beginSubX, pnSub.Height / 2 - logical.Height / 2);
                                logical.Parent = pnSub;
                                beginSubX += logical.Width;

                                if (!m_dicTemp.ContainsKey(Convert.ToInt32(expressionNumber2[j])))
                                    continue;

                                string strCbValue = m_dicTemp[Convert.ToInt32(expressionNumber2[j])];

                                foreach (CustomComboBoxItem item in logical.ComboBox.Items)
                                {
                                    if (strCbValue == item.StrValue)
                                    {
                                        logical.ComboBox.SelectedItem = item;
                                        break;
                                    }
                                }
                            }
                        }

                        beginX += pnSub.Size.Width;
                    }
                    else
                    {
                        LogicalTerm logical = AddLogicalTerm();
                        logical.Location = new Point(beginX, (m_nPanelHeight + 20) / 2 - logical.Height / 2);
                        logical.Parent = pnMain;
                        logical.Label.ForeColor = Color.White;

                        beginX += logical.Width;

                        if (!m_dicTemp.ContainsKey(Convert.ToInt32(m_temp[i])))
                            continue;

                        string strCbValue = m_dicTemp[Convert.ToInt32(m_temp[i])];

                        foreach (CustomComboBoxItem item in logical.ComboBox.Items)
                        {
                            if (strCbValue.Trim() == item.StrValue)
                            {
                                logical.ComboBox.SelectedItem = item;
                                break;
                            }
                        }
                    }
                }
            }

            if (m_temp.Count == 0 || !checkBoxExpression.Checked)
            {
                Panel pnSub = AddSubPanel(0, 0);
                OneFormula formula = AddOneFormula();
                formula.Parent = pnSub;
                formula.Location = new Point(m_nSpace, pnSub.Height / 2 - formula.Height / 2);
            }
        }

        /// <summary>
        /// 괄호 있는 수식
        /// </summary>
        private void MakeArr(int beginIdx, int endIdx, ref int dicIndex)
        {
            string[] logicalArr = new string[] { "and", "or" };

            string temp = m_strExpression.Substring(beginIdx, endIdx - beginIdx);
            string[] dd = temp.Split(logicalArr, StringSplitOptions.RemoveEmptyEntries);
            
            string bracketIndex = "";

            int lastLogicalIndex = 0; // 마지막으로 검색한 논리어 위치 다음부터 검색하기 위한 변수

            for (int j = 0; j < dd.Length; j++)
            {
                int tempIndex = temp.IndexOf(dd[j], lastLogicalIndex);
                int tempLength = tempIndex + dd[j].Length;
                string logical = "";
                if (dd.Length - 1 >= j + 1 && dd[j + 1].Trim().Length > 0)
                {
                    int tempIndex2 = temp.IndexOf(dd[j + 1], lastLogicalIndex);
                    logical = temp.Substring(tempLength, tempIndex2 - tempLength);

                    lastLogicalIndex = tempIndex2 + 1;
                }
                else
                {
                    if (tempIndex > 0)
                        logical = temp.Substring(tempLength);
                }

                m_dicTemp.Add(dicIndex, dd[j]);
                if (bracketIndex.Length == 0)
                    bracketIndex = dicIndex.ToString();
                else
                    bracketIndex += "," + dicIndex.ToString();
                dicIndex++;
                if (logical.Length > 0)
                {
                    m_dicTemp.Add(dicIndex, logical);
                    bracketIndex += "," + dicIndex.ToString();
                    dicIndex++;
                }
            }

            m_temp.Add(bracketIndex);
        }

        /// <summary>
        /// 괄호 없는 수식
        /// </summary>
        private void MakeArr2(int beginIdx, int endIdx, ref int dicIndex)
        {
            string[] logicalArr = new string[] { "and", "or" };

            string temp = m_strExpression.Substring(beginIdx, endIdx - beginIdx);
            
            for (int i = 0; i < logicalArr.Length; i++)
            {
                if (temp.Contains(logicalArr[i]))
                {
                    temp = temp.Replace(logicalArr[i], "@" + logicalArr[i] + "@");
                }
            }
            
            string[] dd = temp.Split('@');
            
            for (int j = 0; j < dd.Length; j++)
            {
                if (dd[j].Trim().Length == 0)
                    continue;

                m_dicTemp.Add(dicIndex, dd[j]);
                m_temp.Add(dicIndex.ToString());
                dicIndex++;
            }
        }

        private void SetFormula(OneFormula formula, string expression)
        {
            int nIndex = expression.IndexOf('{');
            int nIndex2 = expression.IndexOf('}', nIndex + 1);
            string strVariableName = expression.Substring(nIndex + 1, nIndex2 - nIndex - 1).Trim();
            string strVariableNameLow = "{" + strVariableName.ToLower() + "}";

            SectionDataDecision.VariableType type = Sections.SectionDataDecision.VariableType.UNKNOWN;
            DataGridViewRow row = GetVariableType(strVariableName, ref type);
            if (type == SectionDataDecision.VariableType.UNKNOWN || row == null)
                return;

            formula.CurrentVariableType = type;
            formula.SetVariable(row);

            string expressionExcept = expression.ToLower().Replace(strVariableNameLow, "").Trim();

            string strCbValue = "";
            if (type == Sections.SectionDataDecision.VariableType.BOOLEAN)
            {
                string strValue = "";
                string firstLetter = expressionExcept.Substring(0, 1);
                if (firstLetter == "=")
                    firstLetter = expressionExcept.Replace("=", "").Trim().Substring(0, 1);

                if (firstLetter == "t" || firstLetter == "1")
                    strValue = "true";
                else if (firstLetter == "f" || firstLetter == "0")
                    strValue = "false";
                
                strCbValue = strValue;
            }
            else if (type == Sections.SectionDataDecision.VariableType.DOUBLE || type == Sections.SectionDataDecision.VariableType.INTEGER)
            {
                string regEx = "";
                if (type == Sections.SectionDataDecision.VariableType.DOUBLE)
                    regEx = @"^[0-9\.]";                
                else
                    regEx = "^[0-9]";
                
                string strValue = "";
                int nLengthIndex = 0;

                while (nLengthIndex < expressionExcept.Length)
                {
                    string temp = expressionExcept.Substring(nLengthIndex, 1);
                    bool chk = System.Text.RegularExpressions.Regex.IsMatch(temp, regEx);
                    if (chk)
                        strValue += temp;

                    nLengthIndex++;
                }

                if (type == Sections.SectionDataDecision.VariableType.DOUBLE)
                {
                    double dValue = 0;
                    if (double.TryParse(strValue, out dValue))
                    {
                        formula.TbValue.TextBox.Text = dValue.ToString();
                        formula.TbValue.TextBox.Visible = false;
                    }
                }
                else if (type == Sections.SectionDataDecision.VariableType.INTEGER)
                {
                    int nValue = 0;
                    if (int.TryParse(strValue, out nValue))
                    {
                        formula.TbValue.TextBox.Text = nValue.ToString();
                        formula.TbValue.TextBox.Visible = false;
                    }
                }

                if (strValue.Length > 0)
                    strCbValue = expressionExcept.Replace(strValue, "").Trim(); // 값 제거하면 부등호만 남음
            }
            else if (type == Sections.SectionDataDecision.VariableType.STRING)
            {
                string[] stringArr = new string[] { "=", "<>", "not like", "like", "not contains", "contains" };
                string value = "";

                for (int i = 0; i < stringArr.Length; i++)
                {
                    if (expressionExcept.Contains(stringArr[i]))
                    {
                        strCbValue = stringArr[i];
                        break;
                    }
                }

                value = expressionExcept.Replace(strCbValue, "");
                strCbValue = strCbValue.Replace("like", "contains");

                if (strCbValue.Contains("contains"))
                    value = value.Replace("%", "");
                value = value.Replace("'", "").Trim();

                formula.TbValue.TextBox.Text = value;
                formula.TbValue.TextBox.Visible = false;
            }

            foreach (CustomComboBoxItem item in formula.CbCondition.ComboBox.Items)
            {
                if (strCbValue == item.StrValue)
                {
                    formula.CbCondition.ComboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void InitDivision()
        {
            List<CustomComboBoxItem> item1 = new List<CustomComboBoxItem>();
            item1.Add(new CustomComboBoxItem("true", "참(그렇다)"));
            item1.Add(new CustomComboBoxItem("false", "거짓(아니다)"));
            m_division.Add(Sections.SectionDataDecision.VariableType.BOOLEAN, item1);

            List<CustomComboBoxItem> item2 = new List<CustomComboBoxItem>();
            item2.Add(new CustomComboBoxItem(">=", "이상"));
            item2.Add(new CustomComboBoxItem("<=", "이하"));
            item2.Add(new CustomComboBoxItem(">", "초과"));
            item2.Add(new CustomComboBoxItem("<", "미만"));
            item2.Add(new CustomComboBoxItem("=", "같다"));
            item2.Add(new CustomComboBoxItem("<>", "다르다"));
            m_division.Add(Sections.SectionDataDecision.VariableType.INTEGER, item2);
            m_division.Add(Sections.SectionDataDecision.VariableType.DOUBLE, item2);

            List<CustomComboBoxItem> item3 = new List<CustomComboBoxItem>();
            item3.Add(new CustomComboBoxItem("=", "같다"));
            item3.Add(new CustomComboBoxItem("<>", "다르다"));
            item3.Add(new CustomComboBoxItem("contains", "포함"));
            item3.Add(new CustomComboBoxItem("not contains", "포함되지 않음"));
            m_division.Add(Sections.SectionDataDecision.VariableType.STRING, item3);
        }

        /// <summary>
        /// pnMain에 DragDrop을 통해서 pnSub를 한개 더 추가함
        /// </summary>
        /// <param name="e"></param>
        private void AddSubPanel(DragEventArgs e)
        {
            Point location = new Point(-1, -1);
            Size size = new Size(-1, -1);

            int index = 0;
            foreach (Control pnSub in pnMain.Controls)
            {
                if (pnSub.Name.Contains("pnSub_"))
                {
                    string nameTemp = pnSub.Name.Replace("pnSub_", "");

                    if (index < Convert.ToInt32(nameTemp))
                    {
                        location = pnSub.Location;
                        size = pnSub.Size;
                        index = Convert.ToInt32(nameTemp);
                    }
                }
            }

            // 삭제로 인해 마지막 index가 변경됐을수도 있으니 업데이트해줌
            m_nPnSubIndex = index + 1;

            if (location.X == -1 && location.Y == -1)
                return;

            LogicalTerm term = new LogicalTerm();
            term.Label.ForeColor = Color.White;
            term.Location = new Point(location.X + size.Width, location.Y + size.Height / 2 - term.Height / 2);
            term.Parent = pnMain;
            term.Label.TextChanged += Label_TextChanged;

            Panel pn = AddSubPanel(location.X + size.Width + term.Width, location.Y);
            OneFormula formula = AddOneFormula();
            formula.Parent = pn;
            formula.Location = new Point(m_nSpace, pn.Height / 2 - formula.Height / 2);

            formula.Variable_DragDrop(formula.PnVariable, e);
        }

        private void Label_TextChanged(object sender, EventArgs e)
        {
            MakeStrVariable();
        }

        private Panel AddSubPanel(int pnSubX, int pnSubY)
        {
            Panel pnSub = new Panel();
            pnSub.Parent = pnMain;
            pnSub.Name = "pnSub_" + m_nPnSubIndex++;

            pnSub.Size = new Size(100 + m_nSpace * 2/*앞 뒤 공백 width 더해줌*/, m_nPanelHeight + 20);
            Size pnSubSize = pnSub.Size;
            if (pnSubX + pnSubSize.Width >= pnMain.Width)
            {
                if (pnMain.Width > pnSubSize.Width)
                {
                    pnSubX = 0;
                    pnSubY += pnSubSize.Height;
                }
            }
            pnSub.Location = new Point(pnSubX, pnSubY);
            pnSub.Paint += PnSub_Paint;
            pnSub.MouseDown += PnSub_MouseDown;
            pnSub.PreviewKeyDown += PnSub_PreviewKeyDown;

            SetDoubleBuffer(pnSub, true);

            return pnSub;
        }

        private OneFormula AddOneFormula()
        {
            OneFormula formula = new OneFormula();
            formula.AllowDrop = true;
            formula.Condition = m_division;
            formula.MouseDown += Formula_MouseDown;
            formula.PnVariable.MouseDown += Formula_MouseDown;
            formula.PreviewKeyDown += Formula_PreviewKeyDown;

            return formula;
        }

        private void PnSub_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (m_ClickSubPanel == null)
                return;

            if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
            {
                Panel pnSub = sender as Panel;
                if (pnSub == null)
                    return;

                RemoveSubPanel(pnSub);
                RemoveClickSubPanel(m_ClickSubPanel);
                RemoveClickFormula();

                MakeStrVariable();
            }
        }

        private void PnSub_MouseDown(object sender, MouseEventArgs e)
        {
            Panel pnSub = sender as Panel;
            //pnSub.DoDragDrop(sender, DragDropEffects.Copy);

            if (m_ClickSubPanel == pnSub)
            {
                RemoveClickSubPanel(m_ClickSubPanel);
                return;
            }

            RemoveClickSubPanel(m_ClickSubPanel);
            RemoveClickFormula();

            if (m_ClickSubPanel != null && pnSub != m_ClickSubPanel)
            {
                m_ClickSubPanel.Refresh();
            }

            m_ClickSubPanel = pnSub;
            pnSub.Refresh();
            m_ClickSubPanel.Focus();
        }

        private void Formula_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (m_ClickFormula == null)
                return;

            if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
            {
                RemoveFormula(m_ClickFormula);
                RemoveClickFormula();

                MakeStrVariable();
            }
        }

        private void Formula_MouseDown(object sender, MouseEventArgs e)
        {
            Panel pn = sender as Panel;
            //pn.DoDragDrop(sender, DragDropEffects.All);

            if (pn is Panel)
            {
                OneFormula formula = null;
                if (pn is OneFormula)
                    formula = pn as OneFormula;
                else if (pn is Panel)
                    formula = pn.Parent as OneFormula;

                if (formula == null)
                    return;

                //if (!m_bDownCtrlKey)
                //{
                //    // 기존 
                //    foreach (OneFormula item in m_ClickFormulas)
                //    {
                //        item.bMouseEnter = false;
                //        item.Refresh();
                //    }

                //    m_ClickFormulas.Clear();


                //}

                if (m_ClickFormula != null)
                {
                    m_ClickFormula.bMouseEnter = false;
                    m_ClickFormula.Refresh();
                }

                if (m_ClickFormula == formula)
                {
                    RemoveClickFormula();
                    return;
                }

                if (m_ClickFormula != null && formula != m_ClickFormula)
                {
                    m_ClickFormula.bMouseDown = !m_ClickFormula.bMouseDown;
                    m_ClickFormula.Refresh();
                }

                m_ClickFormula = formula;
                formula.bMouseDown = !formula.bMouseDown;
                formula.Refresh();
                m_ClickFormula.Focus();
            }

            RemoveClickSubPanel(m_ClickSubPanel);
        } 

        /// <summary>
        /// 수식 사이에 논리어를 추가한다
        /// </summary>
        /// <param name="pnParnet"></param>
        /// <param name="formula"></param>
        private void AddLogicalTerm(Panel pnParnet, OneFormula formula)
        {
            var controls = pnParnet.Controls.Cast<Control>();
            IEnumerable<Control> gg = controls.OrderBy(p => p.Location.X);

            List<Control> tempCtrls = new List<Control>();
            foreach (Control item in gg)
            {
                tempCtrls.Add(item);
            }

            int trgIndex = -1;
            for (int i = 0; i < tempCtrls.Count; i++)
            {
                Control ctrl = tempCtrls[i] as Control;
                if (ctrl is OneFormula)
                {
                    if (ctrl == formula)
                    {
                        trgIndex = i;
                        break;
                    }
                }
            }

            Control ctrl2 = null;
            if (trgIndex > 0)
            {
                int nearIndex = trgIndex - 1;
                if (tempCtrls[nearIndex] is OneFormula)
                    ctrl2 = tempCtrls[trgIndex];
                else if (tempCtrls[nearIndex] is LogicalTerm)
                     ctrl2 = tempCtrls[trgIndex + 1];
            }
            else
            {
                int nearIndex = trgIndex + 1;
                if (tempCtrls[nearIndex] is OneFormula)
                     ctrl2 = tempCtrls[nearIndex];
            }

            if (ctrl2 != null)
            {
                LogicalTerm logical = AddLogicalTerm();
                logical.Location = new Point(ctrl2.Location.X, ctrl2.Parent.Height / 2 - logical.Height / 2);
                logical.Parent = ctrl2.Parent;

                // x를 +1 을 해야 Resize할때 x로 정렬할수 있다.
                ctrl2.Location = new Point(ctrl2.Location.X + 1, ctrl2.Location.Y);
            }
        }

        private LogicalTerm AddLogicalTerm()
        {
            LogicalTerm logical = new LogicalTerm();
            logical.Label.ForeColor = Color.Black;
            logical.BackColor = Color.Transparent;
            logical.Size = new Size(50, 30);
            logical.Label.TextChanged += Label_TextChanged;
            
            return logical;
        }

        /// <summary>
        /// 수식에 변수가 입력되었는지 확인
        /// 변수가 입력되지 않은 상태에선 앞 뒤에 새로운 수식을 추가할 수 없다.
        /// </summary>
        /// <param name="formula"></param>
        private bool CheckVariable(Panel pn)
        {
            foreach (Control ctrl in pn.Parent.Controls)
            {
                if (ctrl is OneFormula)
                {
                    OneFormula formula = ctrl as OneFormula;
                    if (formula.PnVariable.Tag == null)
                        return false;

                    if (formula.CurrentVariableType == Sections.SectionDataDecision.VariableType.UNKNOWN)
                        return false;

                    if (formula.CurrentVariableType != Sections.SectionDataDecision.VariableType.BOOLEAN)
                    {
                        if (formula.TbValue.TextBox.Text.Length == 0)
                            return false;
                    }
                }
            }

            return true;
        }

        #region Paint
        private float radius = 30.0f;
        private float mOutLineThick = 8.0f;
        private Color FILL_BRUSH = Color.FromArgb(210, 210, 210);
        private Color mOutLineColor = Color.FromArgb(95, 146, 201);
        private Pen m_penClick = new Pen(Color.Red);

        private int m_nPanelHeight = 50;
        private int m_nSpace = 20;
        private void PnSub_Paint(object sender, PaintEventArgs e)
        {
            Panel pnSub = sender as Panel;
            if (pnSub == null)
                return;

            Graphics g = e.Graphics;

            // DRAW SELECT LINE
            int nWidth = pnSub.Width - m_nSpace;
            int nHeight = m_nPanelHeight;

            float fx = m_nSpace / 2;
            float fy = m_nSpace / 2;

            int x = (int)fx;
            int y = (int)fy;

            Pen pen = new Pen(mOutLineColor);
            pen.Width = mOutLineThick;

            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.AddLine(x + radius, y, x + nWidth - radius, y);
            path.AddArc(x + nWidth - radius, y, radius, radius, 270, 90);
            path.AddLine(x + nWidth, y + radius, x + nWidth, y + nHeight - radius);
            path.AddArc(x + nWidth - radius, y + nHeight - radius, radius, radius, 0, 90);
            path.AddLine(x + nWidth - radius, y + nHeight, x + radius, y + nHeight);
            path.AddArc(x, y + nHeight - radius, radius, radius, 90, 90);
            path.AddLine(x, y + nHeight - radius, x, y + radius);
            path.AddArc(x, y, radius, radius, 180, 90);
            path.CloseFigure();

            g.SmoothingMode = SmoothingMode.AntiAlias;

            LinearGradientBrush brush = new LinearGradientBrush(new Point(x + nWidth / 2, y), new Point(x + nWidth / 2, y + nHeight), Color.White, FILL_BRUSH);
            g.FillPath(brush, path);
            g.DrawPath(pen, path);

            if (m_ClickSubPanel == pnSub)
            {
                //if (m_bMouseDown)
                {
                    g.DrawLine(m_penClick, 3, 3, pnSub.Width - 3, 3);
                    g.DrawLine(m_penClick, pnSub.Width - 3, 3, pnSub.Width - 3, pnSub.Height - 3);
                    g.DrawLine(m_penClick, pnSub.Width - 3, pnSub.Height - 3, 3, pnSub.Height - 3);
                    g.DrawLine(m_penClick, 3, pnSub.Height - 3, 3, 3);
                }
            }
        }
        #endregion

        /// <summary>
        /// 앞 뒤로 수식을 추가할 수 있도록 빈 패널 삽입, 수식이 추가되면 OneFormula 클래스로 대체한다.
        /// </summary>
        /// <param name="pnParnet">수식을 추가할 부모 panel</param>
        /// <param name="x">부모 panel의 x좌표</param>
        /// <returns></returns>
        private Panel FillBlanks(Panel pnParnet, int x)
        {
            Panel pnEmpty = new Panel();
            pnEmpty.AllowDrop = true;
            pnEmpty.Size = new Size(4, 30);
            pnEmpty.Location = new Point(x, pnParnet.Height / 2 - pnEmpty.Height / 2);
            pnEmpty.BackColor = Color.Transparent;//Color.Red;//Color.FromArgb(100, 0xf8, 0xce, 0xf1);
            pnEmpty.Name = "pnEmpty";
            pnEmpty.Parent = pnParnet;
            pnEmpty.DragEnter += TrgDragEnter;
            pnEmpty.DragDrop += TrgDragDrop;
            pnEmpty.DragLeave += TrgDragLeave;
            pnEmpty.DragOver += TrgDragOver;

            SetDoubleBuffer(pnEmpty, true);

            return pnEmpty;
        }

        public void ResizeControl()
        {
            int beginX = 0;
            int beginY = 0;
            
            List<Control> deleteCtrl = new List<Control>();

            foreach (Control pnSub in pnMain.Controls)
            {
                if (pnSub.Name.Contains("pnSub_"))
                {
                    int count = 0;
                    foreach (Control ctrl in pnSub.Controls)
                    {
                        if (ctrl is OneFormula || ctrl is LogicalTerm)
                            count++;
                    }

                    var controls = pnSub.Controls.Cast<Control>();
                    IEnumerable<Control> gg = controls.OrderBy(p => p.Location.X);

                    int blanksCnt = count + 1; // 공백은 수식 또는 논리어 갯수 보다 하나씩 더 많다. ex:[blank]<수식>[blank]<논리어>[blank]<수식>[blank]
                    int blanksTempCnt = 0;
                    int tempIndex = 0;

                    int width = 0;
                    int subBeginX = m_nSpace;
                    
                    // 선택된 패널의 길이가 길어서 뒤쪽 수식을 못볼때 화살표키로 수식을 이동할수 있다.
                    // 수식을 이동시킨 후 drag&drop으로 수식을 수정할때 위치를 원상복구하는것을 막는다.
                    if (pnSub == m_ClickSubPanel && m_bClickSubPanelMove)
                    {
                        // 첫번째 Control 위치
                        foreach (Control item in gg)
                        {
                            subBeginX = item.Location.X;
                            break;
                        }
                    }

                    foreach (Control ctrl in gg)
                    {
                        if (tempIndex % 2 == 0) // 공백 차례인데 공백이 아니면 공백 추가함
                        {
                            if (ctrl is CustomComboBox || ctrl is OneFormula)
                            {
                                Panel blank = FillBlanks(pnSub as Panel, subBeginX);
                                subBeginX += blank.Width;
                                width += blank.Width;
                                tempIndex++;
                            }
                            blanksTempCnt++;
                        }
                        else // 공백 차례가 아닌데 공백이면 제거
                        {
                            if (ctrl.Name == "pnEmpty")
                            {
                                deleteCtrl.Add(ctrl);
                                continue;
                            }
                        }

                        ctrl.Location = new Point(subBeginX, ctrl.Location.Y);

                        subBeginX += ctrl.Width;
                        width += ctrl.Width;

                        tempIndex++;
                    }

                    if (blanksCnt != blanksTempCnt && blanksCnt > blanksTempCnt)
                    {
                        Panel blank = FillBlanks(pnSub as Panel, subBeginX);
                        width += blank.Width;
                    }

                    width += (m_nSpace * 2); // 맨앞, 맨뒤 공백 width

                    pnSub.Size = new Size(width, pnSub.Size.Height);

                    if (pnSub.Width >= pnMain.Width)
                    {
                        if (beginX > 0)
                        {
                            beginX = 0;
                            beginY += pnSub.Height;
                        }
                    }
                    else
                    {
                        if (beginX + pnSub.Width >= pnMain.Width)
                        {
                            beginX = 0;
                            beginY += pnSub.Height;
                        }
                    }

                    pnSub.Location = new Point(beginX, beginY);

                    beginX += pnSub.Size.Width;
                    pnSub.Refresh();
                }
                else
                {
                    LogicalTerm term = pnSub as LogicalTerm;
                    if (term != null)
                    {
                        if (beginX + term.Width >= pnMain.Width)
                        {
                            beginX = 0;
                            beginY += 70;
                        }

                        term.Location = new Point(beginX, beginY + m_nPanelHeight / 2 - term.ComboBox.Height / 2);
                        beginX += term.Width;
                    }
                }
            }

            foreach (Control ctrl in deleteCtrl)
            {
                foreach (Control pnSub in pnMain.Controls)
                {
                    pnSub.Controls.Remove(ctrl);
                    break;
                }
            }
        }

        private void DeleteLogicalTerm()
        {
            List<Control> deleteInsideLogical = new List<Control>();
            List<Control> deleteOutsideLogical = new List<Control>();

            int index2 = 0;
            Control lastLogicalTerm2 = null;
            foreach (Control pnSub in pnMain.Controls)
            {
                if (pnSub.Name.Contains("pnSub_"))
                {
                    List<Control> ctrls = new List<Control>();
                    foreach (Control ctrl in pnSub.Controls)
                    {
                        if (ctrl is OneFormula || ctrl is LogicalTerm)
                            ctrls.Add(ctrl);
                    }

                    var controls = ctrls.Cast<Control>();
                    IEnumerable<Control> gg = controls.OrderBy(p => p.Location.X); // x 순으로 정렬

                    int index = 0;
                    Control lastLogicalTerm = null;
                    foreach (Control ctrl in gg)
                    {
                        // 짝수는 OneFormula 차례
                        if (ctrl is LogicalTerm)
                        {
                            if (index % 2 == 0)
                            {
                                deleteInsideLogical.Add(ctrl);
                                index++;
                            }
                        }

                        index++;

                        if (ctrl is LogicalTerm)
                            lastLogicalTerm = ctrl;
                        else
                            lastLogicalTerm = null;
                    }

                    if (lastLogicalTerm != null && !deleteInsideLogical.Contains(lastLogicalTerm))
                        deleteInsideLogical.Add(lastLogicalTerm);
                }
                else
                {
                    if (pnSub is LogicalTerm && index2 % 2 == 0) // 짝수 차례에 LogicalTerm이 있으면 안됨
                    {
                        deleteOutsideLogical.Add(pnSub);
                        index2++;
                    }
                }

                if (pnSub is LogicalTerm)
                    lastLogicalTerm2 = pnSub;
                else
                    lastLogicalTerm2 = null;

                index2++;
            }

            if (lastLogicalTerm2 != null)
                deleteOutsideLogical.Add(lastLogicalTerm2);

            foreach (Control ctrl in deleteInsideLogical)
            {
                foreach (Control pnSub in pnMain.Controls)
                {
                    if (pnSub.Controls.Contains(ctrl))
                    {
                        pnSub.Controls.Remove(ctrl);
                        break;
                    }
                }
            }

            foreach (Control item in deleteOutsideLogical)
            {
                pnMain.Controls.Remove(item);
            }
        }

        private void grid_MouseDown(object sender, MouseEventArgs e)
        {
            RemoveClickFormula();

            if (m_ClickSubPanel != null && !m_bClickSubPanelMove)
                RemoveClickSubPanel(m_ClickSubPanel);

            DataGridView grid = (DataGridView)sender;
            grid.DoDragDrop(sender, DragDropEffects.Copy);
        }

        #region Drag&Drop
        private void TrgDragEnter(object sender, DragEventArgs e)
        {
            Panel trgPanel = sender as Panel;
            if (trgPanel == null)
            {
                return;
            }

            if (trgPanel.Name == "pnDelete") // 휴지통
            {
                if (e.Data.GetDataPresent(typeof(OneFormula)) || e.Data.GetDataPresent(typeof(Panel)))
                {
                    e.Effect = DragDropEffects.Copy;
                }
            }
            else if (trgPanel.Name == "pnEmpty") // 공백
            {
                Panel pn = sender as Panel;
                if (pn == null)
                    return;

                if (!CheckVariable(pn))
                    return;

                pn.Size = new Size(30, pn.Height);
                ResizeControl();

                if (e.Data.GetDataPresent(typeof(DataGridView)))
                {
                    e.Effect = DragDropEffects.Copy;
                }
            }
            else if (trgPanel.Name == "pnMain") // Main
            {
                if (e.Data.GetDataPresent(typeof(DataGridView)))
                {
                    e.Effect = DragDropEffects.Copy;
                }
            }
        }

        private void TrgDragOver(object sender, DragEventArgs e)
        {
            Panel trgPanel = sender as Panel;
            if (trgPanel == null)
            {
                return;
            }

            if (trgPanel.Name == "pnDelete") // 휴지통
            {
                //pnDelete.BackgroundImage = imgDeleteEnable;
                //pnDelete.BackColor = Color.FromArgb(0xf7, 0xa9, 0x2b);
            }
            else if (trgPanel.Name == "pnEmpty") // 공백
            {
                if (!CheckVariable(trgPanel))
                    return;

                trgPanel.BackColor = mOutLineColor;
            }
        }

        private void TrgDragLeave(object sender, EventArgs e)
        {
            Panel trgPanel = sender as Panel;
            if (trgPanel == null)
            {
                return;
            }

            if (trgPanel.Name == "pnDelete") // 휴지통
            {
                //pnDelete.BackgroundImage = imgDeleteDisable;
                //pnDelete.BackColor = Color.Transparent;

                //this.Cursor = Cursors.Default;
            }
            else if (trgPanel.Name == "pnEmpty")
            {
                Panel pn = sender as Panel;
                if (pn == null)
                    return;

                pn.Size = new Size(4, pn.Height);
                ResizeControl();
                
                pn.BackColor = Color.Transparent;
            }
        }

        private void TrgDragDrop(object sender, DragEventArgs e)
        {
            Panel trgPanel = sender as Panel;
            if (trgPanel == null)
            {
                return;
            }

            if (trgPanel.Name == "pnDelete") // 휴지통
            {
                //OneFormula formula = (OneFormula)e.Data.GetData(typeof(OneFormula));
                //RemoveFormula(formula);

                //pnDelete.BackgroundImage = imgDeleteDisable;
                //pnDelete.BackColor = Color.Transparent;

                //this.Cursor = Cursors.Default;
            }
            else if (trgPanel.Name == "pnEmpty") // 공백
            {
                // OneFormula 패널로 변경하기               
                int x = trgPanel.Location.X;
                int y = trgPanel.Location.Y;

                Panel pnParent = trgPanel.Parent as Panel;
                pnParent.Controls.Remove(trgPanel);

                OneFormula formula = AddOneFormula();
                formula.Location = new Point(x, pnParent.Height / 2 - formula.Height / 2);
                formula.Parent = pnParent;

                AddLogicalTerm(pnParent, formula);
                formula.Variable_DragDrop(formula.PnVariable, e);
            }
            else if (trgPanel.Name == "pnMain") // Main
            {
                AddSubPanel(e);
            }
        } 
        #endregion

        #region 선택한 수식 삭제, 선택한 수식 취소
        private void RemoveFormula(OneFormula formula)
        {
            if (formula == null)
                return;

            Panel pnParent = formula.Parent as Panel;

            int formulaCnt = 0;
            foreach (Control ctrl in pnParent.Controls)
            {
                if (ctrl is OneFormula)
                {
                    formulaCnt++;
                }
            }
                        
            if (formulaCnt == 1) // 하나밖에 없으면 삭제하지 않고 비운다
            {
                formula.PnVariable.Tag = null;
                formula.CbCondition.Visible = false;
                formula.TbValue.Visible = false;
                formula.NullStr();
                formula.ResizeControl();
            }
            else
            {
                pnParent.Controls.Remove(formula);
            }

            DeleteLogicalTerm();
            ResizeControl();
        }

        private void RemoveClickFormula()
        {
            if (m_ClickFormula == null)
                return;

            m_ClickFormula.bMouseDown = false;
            m_ClickFormula.Refresh();
            m_ClickFormula = null;
        }

        private void RemoveSubPanel(Panel subPanel)
        {
            if (subPanel == null)
                return;

            Panel pnParent = subPanel.Parent as Panel;

            int formulaCnt = 0;
            foreach (Control ctrl in pnParent.Controls)
            {
                if (ctrl is Panel && ctrl.Name.Contains("pnSub_"))
                {
                    formulaCnt++;
                }
            }

            if (formulaCnt == 1)
            {
                for (int i = subPanel.Controls.Count - 1; i >= 0; i--)
                {
                    subPanel.Controls.RemoveAt(i);
                }

                OneFormula formula = AddOneFormula();
                formula.Location = new Point(m_nSpace, subPanel.Height / 2 - formula.Height / 2);
                formula.Parent = subPanel;
            }
            else
            {
                pnParent.Controls.Remove(subPanel);
            }

            DeleteLogicalTerm();
            ResizeControl();
        }

        private void RemoveClickSubPanel(Panel pnSub)
        {
            if (m_ClickSubPanel == null)
                return;

            if (m_bClickSubPanelMove)
            {
                m_bClickSubPanelMove = false;
                ResizeControl();
            }
            
            m_ClickSubPanel = null;
            pnSub.Refresh();
        }

        private void RemoveClickFormula_MouseDown(object sender, MouseEventArgs e)
        {
            RemoveClickFormula();
            RemoveClickSubPanel(m_ClickSubPanel);
        }
        #endregion

        /// <summary>
        /// 해설과 그려진 그림을 풀어서 수식을 만든다
        /// </summary>
        public void MakeStrVariable()
        {
            string strDisplay = "";
            string strValue = "";

            foreach (Control mainCtrl in pnMain.Controls)
            {
                if (strDisplay.Length > 1 && strDisplay.Substring(0, strDisplay.Length) != " ")
                    strDisplay += " ";

                if (strValue.Length > 1 && strValue.Substring(0, strValue.Length) != " ")
                    strValue += " ";

                if (mainCtrl.Name.Contains("pnSub_"))
                {
                    strDisplay += "(";

                    string strValueSub = "";
                    var controls = mainCtrl.Controls.Cast<Control>();
                    IEnumerable<Control> gg = controls.OrderBy(p => p.Location.X); // x 순으로 정렬
                    foreach (Control pnSub in gg)
                    {
                        if (pnSub.Name == "pnEmpty")
                            continue;

                        if (strDisplay.Length > 1 && strDisplay.Substring(strDisplay.Length - 1, 1) != " " && strDisplay.Substring(strDisplay.Length - 1, 1) != "(")
                            strDisplay += " ";
                        if (strValue.Length > 1 && strValue.Substring(strValue.Length - 1, 1) != " " && strValue.Substring(strValue.Length - 1, 1) != "(")
                        {
                            //strValue += " ";
                            strValueSub += " ";
                        }

                        if (pnSub is OneFormula)
                        {
                            string display = "", value = "";
                            OneFormula formula = pnSub as OneFormula;
                            formula.GetStrVariable(ref display, ref value);
                            strDisplay += display;
                            //strValue += value;
                            strValueSub += value;
                        }
                        else if (pnSub is LogicalTerm)
                        {
                            LogicalTerm logical = pnSub as LogicalTerm;
                            if (logical == null)
                                continue;

                            CustomComboBoxItem combo = logical.ComboBox.SelectedItem as CustomComboBoxItem;
                            strDisplay += (combo == null) ? "" : combo.StrDisplay;
                            //strValue += (combo == null) ? "" : combo.StrValue;
                            strValueSub += (combo == null) ? "" : combo.StrValue;
                        }
                    }

                    if (strValueSub.Length > 0)
                    {                        
                        strValue += "(";
                        strValue += strValueSub;
                        strValue += ")";
                    }
                    strDisplay += ")";                    
                }
                else
                {
                    LogicalTerm logical = mainCtrl as LogicalTerm;
                    if (logical == null)
                        continue;

                    CustomComboBoxItem combo = logical.ComboBox.SelectedItem as CustomComboBoxItem;
                    strDisplay += (combo == null) ? "" : combo.StrDisplay;
                    strValue += (combo == null) ? "" : combo.StrValue;
                }
            }

            lblDesc.Text = strDisplay;
            m_strExpression = strValue;

            new ToolTip().SetToolTip(lblDesc, strDisplay);
        }

        private void MoveChild(Keys keyData)
        {
            if (m_ClickSubPanel == null)
                return;

            if (m_ClickSubPanel.Width <= pnMain.Width)
                return;
            
            int minX = 0;
            int endX = 0;
            foreach (Control ctrl in m_ClickSubPanel.Controls)
            {
                if (minX == 0)
                    minX = ctrl.Location.X;
                else
                    minX = Math.Min(minX, ctrl.Location.X);
                endX = Math.Max(endX, ctrl.Location.X + ctrl.Width);
            }

            if (keyData == Keys.Right && pnMain.Width - 30 >= endX)
                return;
            else if (keyData == Keys.Left && minX == m_nSpace)
                return;

            int moveX = 0;
            if (keyData == Keys.Right)
                moveX = -5;
            else
                moveX = 5;

            foreach (Control ctrl in m_ClickSubPanel.Controls)
            {
                ctrl.Location = new Point(ctrl.Location.X + moveX, ctrl.Location.Y);
            }

            if (!m_bClickSubPanelMove)
                m_bClickSubPanelMove = true;
        }

        public void ttt(int changeColumnIndex, SOPParameter param, object rollbackData)
        {
            if (m_strExpression.Length == 0)
                return;

            if (changeColumnIndex == 0)
            {
                if (m_strExpression.Contains("{" + (string)rollbackData + "}"))
                {
                    m_strExpression = m_strExpression.Replace("{" + (string)rollbackData + "}", "{" + param.VariableName + "}");
                } 
            }
            else if (changeColumnIndex == 1)
            {
                Sections.SectionDataDecision.VariableType oldType = (Sections.SectionDataDecision.VariableType)rollbackData;

                if ((oldType == SectionDataDecision.VariableType.INTEGER && param.Type == SectionDataDecision.VariableType.DOUBLE) ||
                    (oldType == SectionDataDecision.VariableType.DOUBLE && param.Type == SectionDataDecision.VariableType.INTEGER))
                {
                    return;
                }

                string expression = "{" + param.VariableName + "}";

                string[] logicalArr = new string[] { "and", "or" };

                //string temp = m_strExpression.Substring(beginIdx, endIdx - beginIdx);
                string[] dd = expression.Split(logicalArr, StringSplitOptions.RemoveEmptyEntries);

                SetDicExpression();

                string value = "";
                if (param.Type == SectionDataDecision.VariableType.STRING)
                    value = "='내용'";
                else if (param.Type == SectionDataDecision.VariableType.INTEGER || param.Type == SectionDataDecision.VariableType.DOUBLE)
                    value = "=0";
                else if (param.Type == SectionDataDecision.VariableType.BOOLEAN)
                    value = "=true";
                                
                foreach (KeyValuePair<int, string> item in m_dicTemp)
                {
                    if (item.Value.Contains(expression))
                    {
                        m_strExpression = m_strExpression.Replace(item.Value, expression + value);
                    }
                }             
            }
            else if (changeColumnIndex == 2)
            {
                MakeStrVariable();
            }

            InitPanel();
        }

        private void SetDicExpression()
        {
            m_dicTemp.Clear();
            m_temp.Clear();
            m_strExpression = m_strExpression.Trim();

            int dicIndex = 1;
            if (m_strExpression.IndexOf("(") >= 0)
            {
                ArrayList bracketSets = new ArrayList();
                List<int> openIndexs = new List<int>();
                for (int i = 0; i < m_strExpression.Length; i++)
                {
                    if (m_strExpression.Substring(i, 1) == "(")
                    {
                        openIndexs.Add(i);
                    }
                    else if (m_strExpression.Substring(i, 1) == ")")
                    {
                        bracketSets.AddRange(new int[] { openIndexs[openIndexs.Count - 1], i });
                        openIndexs.Remove(openIndexs[openIndexs.Count - 1]);
                    }
                }

                if (bracketSets.Count > 0 && (int)bracketSets[0] > 0)
                {
                    int beginIdx = 0;
                    int endIdx = (int)bracketSets[0];

                    MakeArr2(beginIdx, endIdx, ref dicIndex);
                }

                // 하나의 괄호단위
                for (int i = 0; i < bracketSets.Count; i += 2)
                {
                    int beginIdx = (int)bracketSets[i] + 1;
                    int endIdx = (int)bracketSets[i + 1];

                    MakeArr(beginIdx, endIdx, ref dicIndex);

                    // 마지막 괄호가 아니라면 괄호 사이 논리어 구하기
                    if (bracketSets.Count - 1 >= i + 3)
                    {
                        int nextBarcketBeginIndex = (int)bracketSets[i + 2]; // 다음 괄호 index
                        string logical = m_strExpression.Substring(endIdx + 1, nextBarcketBeginIndex - 1 - endIdx);
                        m_dicTemp.Add(dicIndex, logical);
                        m_temp.Add(dicIndex.ToString());
                        dicIndex++;
                    }
                }

                if (bracketSets.Count > 0 && (int)bracketSets[bracketSets.Count - 1] + 1 < m_strExpression.Length)
                {
                    int beginIdx = (int)bracketSets[bracketSets.Count - 1] + 1;
                    int endIdx = m_strExpression.Length;

                    MakeArr2(beginIdx, endIdx, ref dicIndex);
                }
            }
            else
            {
                MakeArr(0, m_strExpression.Length, ref dicIndex);
            }

            Section section = m_prop.GetSection();

            if (section == null)
                return;

            m_propSection = section;

            textBox.Text = section.Title;

            SectionDataDecision data = (SectionDataDecision)section.Data;
            data.Expression = m_strExpression;
        }
    }
}
