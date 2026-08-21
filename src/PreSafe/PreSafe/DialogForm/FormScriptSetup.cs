using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PreSafe
{
    internal partial class FormScriptSetup : Form
    {
        private Variables<Variable> m_SystemVars = null;
        public Variables<Variable> SystemVariables
        {
            get { return m_SystemVars; }           
        }

        public FormScriptSetup()
        {
            InitializeComponent();
            this.TopLevel = false;

            CreateVarTable();

        }

        private void CreateVarTable()
        {
            m_SystemVars = SenarioManager.Instance.SystemVariables;            
            try            
            {
               IEnumerable<Variable> varList =  m_SystemVars.VarList;
               foreach (Variable var in varList)
                { 
                    DataGridViewRow row = new DataGridViewRow();
                    row.Tag = var;

                    DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
                    cell1.Value = var.Name;
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
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message, ex);
            }
        }

        private bool CheckValues()
        {
            return false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if( CheckValues())
            {
                //this.Visible = false;
                

                // set system var -> python var;
                // set user var -> pyhotn var;
                // set enum -> python var;

                // get start section

                // do loop

                // get next sections , befor decision, end section

                // make section expression -> python expression

                // run script -> get result;

                // decision, or end point

                // if enpoint then exit

                // while enpoint


            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

            this.Visible = false;
            
        }
    }     
}
