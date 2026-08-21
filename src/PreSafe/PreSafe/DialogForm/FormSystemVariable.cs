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
    internal partial class FormSystemVariable : Form
    {
        private Variables<Variable> m_SystemVars = null;
        public Variables<Variable> SystemVariables
        {
            get { return m_SystemVars; }           
        }

        public FormSystemVariable()
        {
            InitializeComponent();
            this.TopLevel = false;

            InitDataGridView();
        }

        public void ClearSelection()
        {
            mSystemVarGrid.ClearSelection();
        }

        private void InitDataGridView()
        {
            m_SystemVars = SenarioManager.Instance.SystemVariables;
            
            mSystemVarGrid.ClearSelection();
            mSystemVarGrid.Rows.Clear();

            try
            {
                IEnumerable<Variable> varList = m_SystemVars.VarList;
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
                    cell3.Value = var.Unit;
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
    }     
}
