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

namespace SOPManager
{
    public partial class PopupNoteDecision : Form
    {
        private bool m_systemCall = false;

        private PropertiesDecision m_prop = null;
        private bool m_isChanged;

        public PropertiesDecision PropertiesDecision
        {
            get { return m_prop; }
            set
            {
                m_prop = value;
                Init();
            }
        }

        public bool IsChanged
        {
            get
            {
                return m_isChanged;
            }
        }

        public PopupNoteDecision()
        {
            InitializeComponent();
            //m_prop = prop;
            //Init();
        }

        private void Init()
        {
            if (m_prop == null)
                return;

            Section section = m_prop.GetSection();

            if (section == null)
                return;

            textBox.Text = section.Title;

            SectionDataDecision data = (SectionDataDecision)section.Data;

            m_systemCall = true;
            textBoxExpression.Text = data.Expression;
            m_systemCall = false;

            gridType.Rows.Clear();
                
            foreach (KeyValuePair<string, SectionDataDecision.VariableType> pair in data.VariableTypes)
            {
                AddVariable(pair.Key, pair.Value);
            }

            checkBoxExpression.Checked = data.Expression.Length > 0;
            checkBoxExpression_CheckedChanged(null, null);
        }

        private void AddVariable(string strVariable, SectionDataDecision.VariableType type = SectionDataDecision.VariableType.UNKNOWN)
        {
            /*DataGridViewRow row = new DataGridViewRow();

            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            cell.Value = gridType.Rows.Count + 1;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strVariable == null ? "" : strVariable;
            row.Cells.Add(cell);

            DataGridViewComboBoxCell cell2 = new DataGridViewComboBoxCell();
            cell2.Value = this.colType.Items[(int)type].ToString();
            row.Cells.Add(cell2);

            gridType.Rows.Add(row);*/
            int nRowIndex = gridType.Rows.Add();
            DataGridViewRow row = gridType.Rows[nRowIndex];

            row.Cells[0].Value = gridType.Rows.Count;
            row.Cells[1].Value = strVariable;
            row.Cells[2].Value = this.colType.Items[(int)type];
        }

        private void checkBoxExpression_CheckedChanged(object sender, EventArgs e)
        {
            labelExpression.Visible = labelType.Visible = textBoxExpression.Visible = gridType.Visible = checkBoxExpression.Checked;

            if (checkBoxExpression.Checked == false)
            {
                this.Size = new Size(this.Size.Width, 274);
                textBox.Size = new Size(this.Size.Width, 229);
            }
            else
            {
                this.Size = new Size(this.Size.Width, 464);
                textBox.Size = new Size(this.Size.Width, 111);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SaveData();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public void SaveData()
        {
            if (m_prop != null)
            {
                Section section = m_prop.GetSection();

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

                    if (m_isChanged == false)
                    {
                        int nRowCount = gridType.Rows.Count;

                        if (nRowCount != data.VariableTypes.Count)
                            m_isChanged = true;
                        else
                        {
                            SectionDataDecision.VariableType type;

                            for (int i = 0; i < nRowCount; i++)
                            {
                                DataGridViewRow row = gridType.Rows[i];
                                string strVariable = row.Cells[1].Value.ToString().Trim();
                                string strType = row.Cells[2].Value.ToString().Trim();

                                if (data.VariableTypes.TryGetValue(strVariable, out type))
                                {
                                    if (type != SectionDataDecision.ToVariableType(strType))
                                    {
                                        m_isChanged = true;
                                        break;
                                    }
                                }
                                else
                                {
                                    m_isChanged = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (m_isChanged)
                    {
                        m_prop.Text = textBox.Text;

                        if (checkBoxExpression.Checked)
                            m_prop.Expression = textBoxExpression.Text;
                        else
                            m_prop.Expression = "";

                        data.VariableTypes.Clear();

                        foreach (DataGridViewRow row in gridType.Rows)
                        {
                            string strVariable = row.Cells[1].Value.ToString().Trim();
                            string strType = row.Cells[2].Value.ToString().Trim();
                            data.VariableTypes[strVariable] = SectionDataDecision.ToVariableType(strType);
                        }
                    }
                }
            }
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

        private void textBoxExpression_TextChanged(object sender, EventArgs e)
        {
            if (m_systemCall)
                return;

            List<string> variablesSmall;
            List<string> variables = GetVariables(textBoxExpression.Text, out variablesSmall);

            // 새로운 변수 추가
            foreach (string strVariable in variables)
            {
                if (ContainsVariableGrid(strVariable) == false)
                    AddVariable(strVariable);
            }

            int nRowCount = gridType.Rows.Count;

            // 삭제된 변수 제거
            for (int i=nRowCount-1;i>=0;i--)
            {
                DataGridViewRow row = gridType.Rows[i];
                string strVariable = row.Cells[1].Value.ToString().ToLower();

                if (variablesSmall.Contains(strVariable) == false)
                    gridType.Rows.RemoveAt(i);
            }
        }

        private bool ContainsVariableGrid(string strVariable)
        {
            string strLower = strVariable.ToLower();

            foreach (DataGridViewRow row in gridType.Rows)
            {
                if (row.IsNewRow)
                    continue;

                string str = row.Cells[1].Value.ToString().ToLower();

                if (str == strLower)
                    return true;
            }

            return false;
        }

        private List<string> GetVariables(string strText, out List<string> variablesSmall)
        {
            variablesSmall = new List<string>();
            List<string> variables = new List<string>();

            int nBeginIndex = -1;
            int nLength = strText.Length;

            for (int i=0;i<nLength;i++)
            {
                char ch = strText[i];

                if (nBeginIndex < 0)
                {
                    if (ch == '{')
                        nBeginIndex = i;
                }
                else
                {
                    if (ch == '}')
                    {
                        string strVariable = strText.Substring(nBeginIndex, i - nBeginIndex + 1);
                        string strVariableSmall = strVariable.ToLower();
                        nBeginIndex = -1;

                        // 같은 변수를 다시 저장하지 않는다.
                        if (variablesSmall.Contains(strVariableSmall) == false)
                        {
                            variablesSmall.Add(strVariableSmall);
                            variables.Add(strVariable);
                        }
                    }
                }
            }

            return variables;
        }
    }
}
