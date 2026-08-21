using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnE.SenarioMaker
{
    internal partial class FormScriptSetup : Form
    {
        private Variables<Variable> m_SystemVars = null;

        public FormScriptSetup()
        {
            InitializeComponent();
            this.TopLevel = false;

        }


        private ArrayList mUserVariableRows = new ArrayList();

        private void UpdateVarTable()
        {
            ArrayList arFindRow = new ArrayList();
            ArrayList arFindVar = new ArrayList();

            // Check Variable and UserVariableRow
            IEnumerable<UserVariable> userVarList = SenarioManager.Instance.UserVariables.VarList;
            foreach (DataGridViewRow row in mUserVariableRows)
            {
                UserVariable var = (UserVariable)row.Tag;
                bool bFind = false;
                foreach(UserVariable userVar in userVarList)
                {
                    if (var.Name == userVar.Name)
                    {
                        row.Tag = userVar;
                        row.Cells[2].Value = userVar.Value;
                        arFindVar.Add(userVar);
                        bFind = true;
                        break;
                    }
                }
                if (bFind == true)
                    continue;
                arFindRow.Add(row);
            }

            // Add Row for added Variable
            foreach (UserVariable var in userVarList)
            {
                if (!arFindVar.Contains(var))
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.Tag = var;

                    DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
                    cell1.Value = var.Name;
                    cell1.Tag = "User";
                    row.Cells.Add(cell1);


                    DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
                    cell2.Value = var.Type;
                    row.Cells.Add(cell2);

                    DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
                    cell3.Value = var.ToStringValue();

                    DataGridViewCellStyle style = cell3.Style.Clone();
                    style.BackColor = Color.RoyalBlue;
                    style.ForeColor = Color.WhiteSmoke;
                    cell3.Style = style;
                    row.Cells.Add(cell3);

                    DataGridViewTextBoxCell cell4 = new DataGridViewTextBoxCell();
                    cell4.Value = var.Description;
                    row.Cells.Add(cell4);

                    mSystemVarGrid.Rows.Add(row);

                    mUserVariableRows.Add(row);
                }
            }
            
            // Remove Row for Deleted variable
            foreach(DataGridViewRow row in arFindRow)
            {
                mSystemVarGrid.Rows.Remove(row);
                mUserVariableRows.Remove(row);
            }
        }

        private void CreateVarTable()
        {
            mSystemVarGrid.Rows.Clear();

            m_SystemVars = SenarioManager.Instance.SystemVariables;
            IEnumerable<UserVariable> userVarList = SenarioManager.Instance.UserVariables.VarList;
            try            
            {
               IEnumerable<Variable> varList =  m_SystemVars.VarList;
               foreach (Variable var in varList)
                { 
                    DataGridViewRow row = new DataGridViewRow();
                    row.Tag = var;

                    DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
                    cell1.Value = var.Name;
                    cell1.Tag = "System";
                    row.Cells.Add(cell1);

                    DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
                    cell2.Value = var.Type;
                    row.Cells.Add(cell2);
                    
                    DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
                    cell3.Value = "";

                    DataGridViewCellStyle style = cell3.Style.Clone();
                    style.BackColor = Color.RoyalBlue;
                    style.ForeColor = Color.WhiteSmoke;
                    cell3.Style = style; 
                    row.Cells.Add(cell3);                   

                    DataGridViewTextBoxCell cell4 = new DataGridViewTextBoxCell();
                    cell4.Value = var.Description;
                    row.Cells.Add(cell4);

                    mSystemVarGrid.Rows.Add(row);
                }

                foreach (UserVariable var in userVarList)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.Tag = var;

                    DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
                    cell1.Value = var.Name;
                    cell1.Tag = "User";
                    row.Cells.Add(cell1);


                    DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
                    cell2.Value = var.Type;
                    row.Cells.Add(cell2);

                    DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
                    cell3.Value = var.ToStringValue();

                    DataGridViewCellStyle style = cell3.Style.Clone();
                    style.BackColor = Color.RoyalBlue;
                    style.ForeColor = Color.WhiteSmoke;
                    cell3.Style = style;
                    row.Cells.Add(cell3);

                    DataGridViewTextBoxCell cell4 = new DataGridViewTextBoxCell();
                    cell4.Value = var.Description;
                    row.Cells.Add(cell4);

                    mSystemVarGrid.Rows.Add(row);

                    mUserVariableRows.Add(row);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message, ex);
            }
        }

        private bool CheckValues()
        {
            return true;
        }

        private void SetValue(Variable var, string szValue)
        {
            if( szValue != null && szValue != "")
            {
                try
                {
                    if (var.Type == "정수" || var.Type == "ENUM")
                    {
                        int nValue;
                        if (int.TryParse(szValue, out nValue))
                        {
                            var.Value = nValue;
                        }

                    }
                    else if (var.Type == "실수")
                    {
                        float fValue;
                        if (float.TryParse(szValue, out fValue))
                        {
                            var.Value = fValue;
                        }
                    }
                    else if (var.Type == "문자열")
                    {
                        var.Value = szValue;
                    }
                    else if (var.Type == "BOOLEAN")
                    {
                        bool bValue;
                        if (bool.TryParse(szValue, out bValue))
                        {
                            var.Value = bValue;
                        }                                    
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if( CheckValues())
            {
                this.Visible = false;
                
                DataGridViewRowCollection rows = mSystemVarGrid.Rows;
                foreach( DataGridViewRow row in rows)
                {
                    string szType = (string)row.Cells[0].Tag;
                    if( szType == "System")
                    {
                        Variable var = (Variable)row.Tag;
                        if(row.Cells[2].Value != null)
                        {
                            string szValue = row.Cells[2].Value.ToString();
                            SetValue(var, szValue);
                        }
                    }
                    else if( szType == "User")
                    {
                        UserVariable var = (UserVariable)row.Tag;
                        if (row.Cells[2].Value != null)
                        {
                            string szValue = row.Cells[2].Value.ToString();
                            row.Cells[2].Tag = var.Value;
                            SetValue(var, szValue);
                        }                        
                    }
                }             

                try
                {
                    SOPChecker sopCheker = new SOPChecker(false);

                    if (sopCheker.CheckSOP(SenarioManager.Instance, false))
                    {
                        ScriptValidator checker = new ScriptValidator(SenarioManager.Instance);
                        checker.CheckScript();
                        string szResult = checker.ScriptResult;
                        float fValue = 0.0f;
                        if(float.TryParse(szResult, out fValue))
                        {
                            string szMsg = string.Format("시나리오가 정상적으로 수행 되었습니다.\n\r입력에 대한 시나리오의 CR값은 {0}입니다.", szResult);
                            UnE.Utility.UMessageBox.Show(szMsg, "시나리오 검증");
                        }
                        else
                        {
                            string szMsg = string.Format("시나리오에 오류가 발생하였습니다.\n\r자세한 오류 메세지 : {0}", szResult);
                            UnE.Utility.UMessageBox.Show(szMsg, "시나리오 검증");
                        }
                        
                    }
                }
                catch(Exception ex)
                {
                    string szMsg = string.Format("시나리오에 오류가 발생하였습니다.\n\r자세한 오류 메세지 : {0}", ex.Message);
                    UnE.Utility.UMessageBox.Show(szMsg, "시나리오 검증");
                }

                
                foreach (DataGridViewRow row in rows)
                {
                    string szType = (string)row.Cells[0].Tag;
                    if (szType == "User")
                    {
                        UserVariable var = (UserVariable)row.Tag;
                        object value = (string)row.Cells[2].Tag;
                        var.Value = value;
                    }
                }

            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Visible = false;            
        }

        private void FormScriptSetup_Load(object sender, EventArgs e)
        {
            CreateVarTable();
        }

        private void FormScriptSetup_Shown(object sender, EventArgs e)
        {           
        }

        private void FormScriptSetup_VisibleChanged(object sender, EventArgs e)
        {
            if( this.Visible == true)
                UpdateVarTable();
        }
    }     
}
