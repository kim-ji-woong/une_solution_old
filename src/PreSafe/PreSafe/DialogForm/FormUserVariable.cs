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
    internal partial class FormUserVariable : Form
    {
        private Variables<UserVariable> m_UserVariables = null;
        public Variables<UserVariable> UserVariables
        {
            get { return m_UserVariables; }           
        }

        public FormUserVariable()
        {
            InitializeComponent();
            this.TopLevel = false;

            mUserVarGrid.VirtualMode = true;

            mUserVarGrid.CellValueNeeded += new DataGridViewCellValueEventHandler(DataGridView_CellValueNeeded);
            mUserVarGrid.CellValuePushed += new DataGridViewCellValueEventHandler(DataGridView_CellValuePushed);
            mUserVarGrid.DefaultValuesNeeded += new DataGridViewRowEventHandler(DataGridView_DefaultValuesNeeded);
            mUserVarGrid.NewRowNeeded += new DataGridViewRowEventHandler(DataGridView_NewRowNeeded);
            mUserVarGrid.UserAddedRow += new DataGridViewRowEventHandler(DataGridView_UserAddedRow);
            mUserVarGrid.UserDeletedRow += new DataGridViewRowEventHandler(DataGridView_UserDeletedRow);

            UpdateUserVariable();
        }

        public void ClearSelection()
        {
            mUserVarGrid.ClearSelection();
        }

        public void UpdateUserVariable()
        {
            m_UserVariables = SenarioManager.Instance.UserVariables;
            
            mUserVarGrid.ClearSelection();
            mUserVarGrid.Rows.Clear();

            try
            {
                // 이름, 타입, 기본값, 최대값, 최소값, 단위, 설명
                IEnumerable<UserVariable> varList = m_UserVariables.VarList;
                foreach (UserVariable var in varList)
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
                    cell3.Value = var.DefaultValue;
                    row.Cells.Add(cell3);

                    DataGridViewTextBoxCell cell4 = new DataGridViewTextBoxCell();
                    cell4.Value = var.MaxValue;
                    row.Cells.Add(cell4);

                    DataGridViewTextBoxCell cell5 = new DataGridViewTextBoxCell();
                    cell5.Value = var.MinValue;
                    row.Cells.Add(cell5);

                    DataGridViewTextBoxCell cell6 = new DataGridViewTextBoxCell();
                    cell6.Value = var.Unit;
                    row.Cells.Add(cell6);                    

                    DataGridViewTextBoxCell cell7 = new DataGridViewTextBoxCell();
                    cell7.Value = var.Description;
                    row.Cells.Add(cell7);

                    mUserVarGrid.Rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message, ex);
            }
        }

        #region DataGridView Virtual Mode 처리 루틴
        private void DataGridView_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            DataGridViewRow row = e.Row;
            if( row != null)
            {
                UserVariable var = (UserVariable)row.Tag;
                if (var != null && var.Name != "")
                {
                    SenarioManager.Instance.UserVariables.RemoveVariable(var.Name);
                    //FormMain.Instance.UserVarExporter.RemoveXml("UserVariable", var.Name);
                }
            }
        }

        private void DataGridView_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            DataGridViewRow row = e.Row;
            if (row != null)
            {
                if( row.Tag == null)
                {
                    UserVariable var = new UserVariable("", "정수", "");
                    row.Tag = var;
                }               
            }
        }

        private void DataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            int nRow = e.RowIndex;
            int nCol = e.ColumnIndex;

            DataGridViewRow row = mUserVarGrid.Rows[nRow];
            if (row == null)
                return;

            UserVariable var = (UserVariable)row.Tag;
            if (var == null)
                return;

            switch (nCol)
            {
                case 0:
                    e.Value = var.Name;                     
                    break;
                case 1:
                    e.Value = var.Type;
                    break;
                case 2:
                    e.Value = var.Value;
                    break;
                case 3:
                    e.Value = var.MaxValue;
                    break;
                case 4:
                    e.Value = var.MinValue;
                    break;
                case 5:
                    e.Value = var.Unit;
                    break;
                case 6:
                    e.Value = var.Description;
                    break;
            }
        }

        private void DataGridView_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            int nRow = e.RowIndex;
            int nCol = e.ColumnIndex;

            DataGridViewRow row = mUserVarGrid.Rows[nRow];
            if (row == null)
                return;

            UserVariable var = (UserVariable)row.Tag;
            if (var == null)
                return;

            switch(nCol)
            {
                case 0:

                    string szName = (string)e.Value;
                    if( szName == null || szName == "")
                    {
                        UnE.Utility.UMessageBox.Show("사용자 변수의 이름은 빈 문자열일 수 없습니다.\n고유한 이름을 입력해 주세요.","입력오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if( m_UserVariables.ContainsKey(szName))
                    {
                        UnE.Utility.UMessageBox.Show("사용자 변수의 이름이 중복 되었습니다.\n고유한 이름을 입력해 주세요.", "입력오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (var.Name != "")
                    {
                        SenarioManager.Instance.UserVariables.RemoveVariable(var.Name);
                    }
                    var.Name = szName;
                    SenarioManager.Instance.UserVariables.AddVariable(var);
                    break;
                case 1:
                    var.Type = (string)e.Value;
                    if (var.Type == "정수" || var.Type == "ENUM")
                    {                        
                        int nValue;
                        if (!ObjectUtil.GetValue(var.Value, out nValue))
                        {
                            UnE.Utility.UMessageBox.Show("Value의 입력 값이 정수가 아닙니다. \n초기값으로 변경됩니다.", "입력오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            var.Value = 0;
                        }                        
                    }
                    else if (var.Type == "실수")
                    {
                        float nValue;
                        if (!ObjectUtil.GetValue(e.Value, out nValue))
                        {
                            UnE.Utility.UMessageBox.Show("Value의 입력 값이 실수가 아닙니다. \n초기값으로 변경됩니다.", "입력오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            var.Value = 0.0f;
                        }                        
                    }
                    else if (var.Type == "BOOLEAN")
                    {
                        bool nValue;
                        if (!ObjectUtil.GetValue(e.Value, out nValue))
                        {
                            UnE.Utility.UMessageBox.Show("Value의 입력 값이 BOOLEAN이 아닙니다. \n초기값으로 변경됩니다.", "입력오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            var.Value = false;
                        }
                    }
                    break;
                case 2:
                    if(var.Type == "정수" || var.Type == "ENUM")
                    {
                        int nValue;
                        if( !ObjectUtil.GetValue(e.Value, out nValue))
                        {
                            UnE.Utility.UMessageBox.Show("입력값이 정수가 아닙니다. \n입력 값을 확인하세요.", "입력오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        var.Value = nValue;                        
                    }
                    else if (var.Type == "실수")
                    {
                        float nValue;
                        if (!ObjectUtil.GetValue(e.Value, out nValue))
                        {
                            UnE.Utility.UMessageBox.Show("입력값이 실수가 아닙니다. \n입력 값을 확인하세요.", "입력오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        var.Value = nValue;
                    }
                    else if (var.Type == "BOOLEAN")
                    {
                        bool nValue;
                        if (!ObjectUtil.GetValue(e.Value, out nValue))
                        {
                            UnE.Utility.UMessageBox.Show("입력값이 BOOLEAN이 아닙니다. \n입력 값을 확인하세요.", "입력오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        var.Value = nValue;
                    }                   
                    else
                        var.Value = e.Value;
                    break;
                case 3:
                    var.MaxValue = e.Value;
                    break;
                case 4:
                    var.MinValue = e.Value;
                    break;
                case 5:
                    var.Unit = (string)e.Value;
                    break;
                case 6:
                    var.Description = (string)e.Value;
                    break;
            }
        }

        private void DataGridView_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
        }

        private void DataGridView_NewRowNeeded(object sender, DataGridViewRowEventArgs e)
        {
            DataGridViewRow row = e.Row;
            if (row != null)
            {
                if (row.Tag == null)
                {
                    UserVariable var = new UserVariable("", "정수", "");
                    row.Tag = var;
                }
            }
        }
        #endregion
    }
}
