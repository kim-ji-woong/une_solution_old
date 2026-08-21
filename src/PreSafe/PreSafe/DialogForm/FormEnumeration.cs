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
    internal partial class FormEnumeration : Form
    {
        private Variables<Enums> m_EnumList = null;
        public Variables<Enums> EnumList
        {
            get { return m_EnumList; }
        }

        
        public FormEnumeration()
        {
            InitializeComponent();
            this.TopLevel = false;

            dataGridView = mSystemEnumGrid;
            dataGridView.VirtualMode = true;

            dataGridView.CellValueNeeded += new DataGridViewCellValueEventHandler(DataGridView_CellValueNeeded);
            dataGridView.CellValuePushed += new DataGridViewCellValueEventHandler(DataGridView_CellValuePushed);
            
            dataGridView.NewRowNeeded += new DataGridViewRowEventHandler(DataGridView_NewRowNeeded);
            dataGridView.UserAddedRow += new DataGridViewRowEventHandler(DataGridView_UserAddedRow);
            dataGridView.UserDeletedRow += new DataGridViewRowEventHandler(DataGridView_UserDeletedRow);

            UpdateUserVariable();
        }

        public void ClearSelection()
        {
            mSystemEnumGrid.ClearSelection();
        }

        public void UpdateUserVariable()
        {
            m_EnumList = SenarioManager.Instance.EnumList;

            mSystemEnumGrid.ClearSelection();
            mSystemEnumGrid.Rows.Clear();


            try
            {
                // 이름, 타입, 기본값, 최대값, 최소값, 단위, 설명
                IEnumerable<Enums> varList = m_EnumList.VarList;
                foreach (Enums var in varList)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.Tag = var;

                    DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
                    cell1.Value = var.Name;
                    row.Cells.Add(cell1);

                    DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();                  
                    cell2.Value = var.Type;
                    row.Cells.Add(cell2);                   

                    DataGridViewTextBoxCell cell6 = new DataGridViewTextBoxCell();
                    cell6.Value = var.Value;
                    row.Cells.Add(cell6);

                    DataGridViewTextBoxCell cell7 = new DataGridViewTextBoxCell();
                    cell7.Value = var.Description;
                    row.Cells.Add(cell7);

                    mSystemEnumGrid.Rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message, ex);
            }
        }

        #region DataGridView Virtual Mode 처리 루틴
        private DataGridView dataGridView = null;
        private void DataGridView_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            DataGridViewRow row = e.Row;
            if (row != null)
            {
                Enums var = (Enums)row.Tag;
                if (var != null && var.Name != "")
                {
                    SenarioManager.Instance.EnumList.RemoveVariable(var.Name);
                    //FormMain.Instance.EnumVarExporter.RemoveXml("EnumVariable", var.Name);
                }
            }
        }

        private void DataGridView_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            DataGridViewRow row = e.Row;
            if (row != null)
            {
                if (row.Tag == null)
                {
                    Enums var = new Enums("", "정수", "", "");
                    row.Tag = var;
                }
            }
        }

        private void DataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            int nRow = e.RowIndex;
            int nCol = e.ColumnIndex;


            DataGridViewRow row = dataGridView.Rows[nRow];
            if (row == null)
                return;

            Enums var = (Enums)row.Tag;
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
                    e.Value = var.Description;
                    break;
            }
        }

        private void DataGridView_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            int nRow = e.RowIndex;
            int nCol = e.ColumnIndex;

            DataGridViewRow row = dataGridView.Rows[nRow];
            if (row == null)
                return;

            Enums var = (Enums)row.Tag;
            if (var == null)
                return;

            switch (nCol)
            {
                case 0:
                    string szName = (string)e.Value;
                    if (szName == null || szName == "")
                    {
                        UnE.Utility.UMessageBox.Show("사용자 변수의 이름은 빈 문자열일 수 없습니다.\n고유한 이름을 입력해 주세요.", "입력오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (m_EnumList.ContainsKey(szName))
                    {
                        UnE.Utility.UMessageBox.Show("사용자 변수의 이름이 중복 되었습니다.\n고유한 이름을 입력해 주세요.", "입력오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (SenarioManager.Instance.UserVariables.RemoveVariable(var.Name) != var)
                    {
                        var.Name = szName;
                        SenarioManager.Instance.EnumList.AddVariable(var);
                    } 
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
                    if (var.Type == "정수" || var.Type == "ENUM")
                    {
                        int nValue;
                        if (!ObjectUtil.GetValue(e.Value, out nValue))
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
                    var.Description = (string)e.Value;
                    break;
            }
        }

        private void DataGridView_NewRowNeeded(object sender, DataGridViewRowEventArgs e)
        {
            DataGridViewRow row = e.Row;
            if (row != null)
            {
                if (row.Tag == null)
                {
                    Enums var = new Enums("", "정수", "", "");
                    row.Tag = var;
                }
            }
        }
        #endregion   

    }
}
