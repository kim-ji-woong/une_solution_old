using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace SOPGen
{
    public partial class FormDockingMission : Form
    {
        private FormMain m_Main = null;
        private MemberofSection m_currentMission = null;

        //마우스 우클릭시 좌표로 받아오는 셀의 내용과 rowIndex
        private string m_strCellValue = "";
        private string m_strTaskValue = "";
        private int m_nCellRow = 0;
        private int m_nCheckCellRow = 0;

        public FormDockingMission(FormMain main)
        {
            InitializeComponent();

            m_Main = main;

            dataGridViewMission.Rows.Add();
        }

        // 임무
        private void AddGridRow_Mission(string strValue)
        {
            DataGridViewRow gridRow = new DataGridViewRow();
            DataGridViewCell cell = new DataGridViewTextBoxCell();

            cell.Value = strValue;
            gridRow.Cells.Add(cell);

            dataGridViewMission.Rows.Add(gridRow);
        }

        private void dataGridViewMission_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                foreach (DataGridViewCell cell in dataGridViewMission.SelectedCells)
                {
                    int nColumnCount = dataGridViewMission.ColumnCount;
                    if (cell.ColumnIndex + 1 < nColumnCount - 2)
                    {
                        e.Handled = true;
                        dataGridViewMission.Rows[cell.RowIndex].Cells[cell.ColumnIndex+1].Selected = true;
                    }
                    else if (cell.ColumnIndex + 1 > nColumnCount - 2)
                    {
                        e.Handled = true;
                        dataGridViewMission.Rows[cell.RowIndex].Cells[0].Selected = true;;
                    }
                    else if(cell.ColumnIndex + 1 == nColumnCount - 2)
                    {
                        if (cell.RowIndex< dataGridViewMission.RowCount-1)
                            dataGridViewMission.Rows[cell.RowIndex].Cells[0].Selected = true;
                        else
                        {
                            if (dataGridViewMission.Rows[cell.RowIndex].Cells[0].Value == null || dataGridViewMission.Rows[cell.RowIndex].Cells[1].Value == null) return;

                            string strDivision = dataGridViewMission.Rows[cell.RowIndex].Cells[0].Value.ToString();
                            string strValue = dataGridViewMission.Rows[cell.RowIndex].Cells[1].Value.ToString();
                            if (strDivision != "" && strValue != "")
                            {
                                AddGridRow_Mission(strDivision);
                                dataGridViewMission.Rows[cell.RowIndex + 1].Cells[0].Selected = true;
                            }
                        }
                    }
                    else
                    {
                        if (dataGridViewMission.Rows[cell.RowIndex].Cells[0].Value == null || dataGridViewMission.Rows[cell.RowIndex].Cells[1].Value == null) return;
                        
                        if( cell.RowIndex == dataGridViewMission.RowCount)
                        {
                            string strDivision = dataGridViewMission.Rows[cell.RowIndex].Cells[0].Value.ToString();
                            string strValue = dataGridViewMission.Rows[cell.RowIndex].Cells[1].Value.ToString();
                            if (strDivision != "" && strValue != "")
                            {
                                AddGridRow_Mission(strDivision);
                                dataGridViewMission.Rows[cell.RowIndex + 1].Cells[0].Selected = true;
                            }
                        }
                    }
                    return;
                }
            }
            else if (e.KeyCode == Keys.Delete)
            {
                foreach (DataGridViewCell cell in dataGridViewMission.SelectedCells)
                {
                    if (dataGridViewMission.RowCount > 1)
                        dataGridViewMission.Rows.Remove(dataGridViewMission.Rows[cell.RowIndex]);
                }
            }
        }

        private void dataGridViewMission_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMissionMenu.Show(dataGridViewMission, new Point(e.X, e.Y));

                DataGridView.HitTestInfo hInfo = dataGridViewMission.HitTest(e.X, e.Y);
                if (hInfo.ColumnIndex < 0 || hInfo.RowIndex < 0) return;
                if (dataGridViewMission.Rows[hInfo.RowIndex].Cells[0].Value == null || dataGridViewMission.Rows[hInfo.RowIndex].Cells[1].Value == null) return;
                m_strCellValue = dataGridViewMission.Rows[hInfo.RowIndex].Cells[0].Value.ToString();
                m_strTaskValue = dataGridViewMission.Rows[hInfo.RowIndex].Cells[1].Value.ToString();
                m_nCellRow = hInfo.RowIndex;
            }
        }

        private void dataGridViewMission_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            foreach (DataGridViewRow row in dataGridViewMission.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value == null) continue;
                    
                    if (cell.Selected)
                    {
                        if (cell.ColumnIndex == e.ColumnIndex)
                        {
                            int nColumnCount = dataGridViewMission.ColumnCount;
                            if (cell.ColumnIndex + 1 < nColumnCount - 2)
                            {
                                //row.Cells[cell.ColumnIndex + 1].Selected = true;
                            }
                            else if (cell.ColumnIndex + 1 == nColumnCount - 2)
                            {
                                if (cell.RowIndex < dataGridViewMission.RowCount - 1)
                                {
                                    foreach (DataGridViewRow rows in dataGridViewMission.SelectedRows)
                                    {
                                        if (rows.Index == cell.RowIndex)
                                            dataGridViewMission.Rows[cell.RowIndex].Cells[0].Selected = true;
                                    }
                                    
                                    //dataGridViewMission.Rows[cell.RowIndex].Cells[0].Selected = true;
                                }
                                else
                                {
                                    if (dataGridViewMission.Rows[cell.RowIndex].Cells[0].Value == null || dataGridViewMission.Rows[cell.RowIndex].Cells[1].Value == null) return;

                                    string strDivision = dataGridViewMission.Rows[cell.RowIndex].Cells[0].Value.ToString();
                                    string strValue = dataGridViewMission.Rows[cell.RowIndex].Cells[1].Value.ToString();
                                    if (strDivision != "" && strValue != "")
                                    {
                                        AddGridRow_Mission(strDivision);
                                        dataGridViewMission.Rows[cell.RowIndex + 1].Cells[0].Selected = true;
                                    }
                                }
                            }
                            else
                            {
                                string strDivision = dataGridViewMission.Rows[cell.RowIndex].Cells[0].Value.ToString();
                                string strValue = dataGridViewMission.Rows[cell.RowIndex].Cells[1].Value.ToString();
                                if (strDivision != "" && strValue != "")
                                {
                                    AddGridRow_Mission(strDivision);
                                    dataGridViewMission.Rows[cell.RowIndex + 1].Cells[0].Selected = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void dataGridViewMission_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("CellClick");
        }

        // 점검
        // rowMission : 임무
        private void AddGridRow_Check(string strFirst, string strSecond, DataGridViewRow rowMission)
        {
            DataGridViewRowEx<DataGridViewRow> gridRow = new DataGridViewRowEx<DataGridViewRow>();
            gridRow.Data = rowMission;

            DataGridViewCell cell = null;

            cell = new DataGridViewTextBoxCell();
            cell.Value = strFirst;
            gridRow.Cells.Add(cell);
            gridRow.Cells[0].ReadOnly = true;

            cell = new DataGridViewTextBoxCell();
            cell.Value = strSecond;
            gridRow.Cells.Add(cell);

            dataGridViewCheck.Rows.Add(gridRow);
        }

        private void dataGridViewCheck_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                foreach (DataGridViewCell cell in dataGridViewCheck.SelectedCells)
                {
                    int nColumnCount = dataGridViewCheck.ColumnCount;
                    
                    if (cell.ColumnIndex + 1 < nColumnCount)
                    {
                        e.Handled = true;
                        dataGridViewCheck.Rows[cell.RowIndex].Cells[cell.ColumnIndex + 1].Selected = true;
                    }
                    else if (cell.ColumnIndex + 1 == nColumnCount)
                    {
                        if (cell.RowIndex < dataGridViewCheck.RowCount - 1)
                            dataGridViewCheck.Rows[cell.RowIndex].Cells[0].Selected = true;
                        else //if(cell.RowIndex < dataGridViewMission.RowCount)
                        {
                            //e.Handled = true;
                            //dataGridViewCheck.Rows[cell.RowIndex].Cells[0].Selected = true;

                            if (dataGridViewCheck.Rows[cell.RowIndex].Cells[1].Value == null || dataGridViewCheck.Rows[cell.RowIndex].Cells[2].Value == null) return;

                            string strCategory = dataGridViewCheck.Rows[cell.RowIndex].Cells[0].Value.ToString();
                            string strSubCategory = dataGridViewCheck.Rows[cell.RowIndex].Cells[1].Value.ToString();
                            string strValue = dataGridViewCheck.Rows[cell.RowIndex].Cells[2].Value.ToString();
                            if (strCategory != "" && strSubCategory != "" && strValue != "")
                            {
                                DataGridViewRowEx<DataGridViewRow> row = (DataGridViewRowEx<DataGridViewRow>)dataGridViewCheck.Rows[cell.RowIndex];
                                AddGridRow_Check(strCategory, strSubCategory, row.Data);
                                dataGridViewCheck.Rows[cell.RowIndex + 1].Cells[0].Selected = true;
                            }
                        }
                    }
                    else
                    {
                        if (dataGridViewCheck.Rows[cell.RowIndex].Cells[1].Value == null || dataGridViewCheck.Rows[cell.RowIndex].Cells[2].Value == null) return;

                        string strCategory = dataGridViewCheck.Rows[cell.RowIndex].Cells[0].Value.ToString();
                        string strSubCategory = dataGridViewCheck.Rows[cell.RowIndex].Cells[1].Value.ToString();
                        if (strCategory != "" && strSubCategory != "")
                        {
                            DataGridViewRowEx<DataGridViewRow> row = (DataGridViewRowEx<DataGridViewRow>)dataGridViewCheck.Rows[cell.RowIndex];
                            AddGridRow_Check(strCategory, strSubCategory, row.Data);
                            dataGridViewCheck.Rows[cell.RowIndex + 1].Cells[0].Selected = true;
                        }
                    }
                    return;
                }
            }
            else if (e.KeyCode == Keys.Delete)
            {
                foreach (DataGridViewCell cell in dataGridViewCheck.SelectedCells)
                {
                    dataGridViewCheck.Rows.Remove(dataGridViewCheck.Rows[cell.RowIndex]);
                }
            }
        }

        private void CheckMenu_Click(object sender, EventArgs e)
        {
            if (m_strCellValue != "" && m_strTaskValue != "")
            {
                AddGridRow_Check(m_strCellValue, "", dataGridViewMission.Rows[m_nCellRow]);
            }
            else
            {
                MessageBox.Show("점검항목을 추가하려면 먼저 구분과 내용이 입력되어야 합니다.");
                return;
            }

//             foreach (DataGridViewCell cell in dataGridViewMission.SelectedCells)
//             {
//                 DataGridViewCellCollection cells = dataGridViewMission.Rows[cell.RowIndex].Cells;
// 
//                 if (cells[0].Value == null || cells[1].Value == null)
//                     continue;
// 
//                 string strCategory = Utility.TrimString(cells[0].Value.ToString());
//                 string strTaskName = Utility.TrimString(cells[1].Value.ToString());
// 
//                 if (strCategory != "" && strTaskName != "")
//                 {
//                     AddGridRow_Check(strCategory, "", dataGridViewMission.Rows[cell.RowIndex]);
//                 }
//             }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            string strMember = textMember.Text;
            string strCellPhone = textCellPhone1.Text + textCellPhone2.Text + textCellPhone3.Text;
            string strPhone = textPhone1.Text + textPhone2.Text + textPhone3.Text;
            string strMessenger = textMessenger.Text;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

        }

        private void textCellPhone1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))
            {
                e.Handled = true;
            }
        }

        private void textCellPhone1_Leave(object sender, EventArgs e)
        {
            TextBox strValue = (TextBox)sender;
            bool isCheck = m_Main.NumberCheck(strValue.Text);
            if (!isCheck)
            {
                MessageBox.Show("번호를 입력해 주세요.");
                textCellPhone1.Focus();
            }
        }

        private void textCellPhone2_Leave(object sender, EventArgs e)
        {
            TextBox strValue = (TextBox)sender;
            if (strValue.Text.Length < 3 && strValue.Text.Length != 0)
            {
                MessageBox.Show("번호를 입력해 주세요.");
                textCellPhone2.Focus();
            }
        }

        private void textCellPhone3_Leave(object sender, EventArgs e)
        {
            TextBox strValue = (TextBox)sender;
            if (strValue.Text.Length != 4 && strValue.Text.Length != 0)
            {
                MessageBox.Show("번호를 입력해 주세요.");
                textPhone3.Focus();
            }
        }

        private void textPhone1_Leave(object sender, EventArgs e)
        {
            TextBox text = (TextBox)sender;
            bool isCheck = m_Main.AreaCodeCheck(text.Text);
            if (!isCheck)
            {
                MessageBox.Show("지역번호를 정확하게 입력해 주세요.");
                textPhone1.Focus();
            }
        }

        private void textPhone2_Leave(object sender, EventArgs e)
        {
            TextBox strValue = (TextBox)sender;
            if (strValue.Text.Length < 3 && strValue.Text.Length != 0)
            {
                MessageBox.Show("번호를 입력해 주세요.");
                textPhone2.Focus();
            }
        }

        private void textPhone3_Leave(object sender, EventArgs e)
        {
            TextBox strValue = (TextBox)sender;
            if (strValue.Text.Length != 4 && strValue.Text.Length != 0)
            {
                MessageBox.Show("번호를 입력해 주세요.");
                textPhone3.Focus();
            }
        }

        public void SaveMission()
        {
            if (m_currentMission == null)
                return;

            m_currentMission.Member = textMember.Text;
            m_currentMission.CellPhone1 = textCellPhone1.Text;
            m_currentMission.CellPhone2 = textCellPhone2.Text;
            m_currentMission.CellPhone3 = textCellPhone3.Text;
            m_currentMission.Telephone1 = textPhone1.Text;
            m_currentMission.Telephone2 = textPhone2.Text;
            m_currentMission.Telephone3 = textPhone3.Text;
            m_currentMission.MessengerID = textMessenger.Text;

            m_currentMission.Missions.Clear();
            Dictionary<DataGridViewRow, MemberofSection.MissionofSection> dicMission = new Dictionary<DataGridViewRow, MemberofSection.MissionofSection>();

            foreach (DataGridViewRow gridRow in dataGridViewMission.Rows)
            {
                // 구분이 표기되어 있지 않으면 건너뛴다.
                string strCategory = (string)gridRow.Cells[0].Value;
                if (strCategory == null) continue;

                strCategory = strCategory.TrimStart(new char[] { ' ', '\t', '\r', '\n' });
                strCategory = strCategory.TrimEnd(new char[] { ' ', '\t', '\r', '\n' });
                if (strCategory.Length == 0) continue;

                if (strCategory == "" || Utility.TrimString((string)gridRow.Cells[1].Value) == "")
                    continue;

                MemberofSection.MissionofSection mission = new MemberofSection.MissionofSection();

                mission.Division = strCategory;
                mission.TaskName = (string)gridRow.Cells[1].Value;
                mission.Report = (string)gridRow.Cells[2].Value;

                m_currentMission.Missions.Add(mission);
                dicMission[gridRow] = mission;
            }

            foreach (DataGridViewRowEx<DataGridViewRow> row in dataGridViewCheck.Rows)
            {
                DataGridViewRow rowMission = row.Data;
                if (rowMission == null) continue;

                MemberofSection.CheckofMission checkItem = new MemberofSection.CheckofMission();

                checkItem.Category = (string)row.Cells[0].Value;
                checkItem.SubCategory = (string)row.Cells[1].Value;
                checkItem.TaskName = (string)row.Cells[2].Value;
                checkItem.Count = (string)row.Cells[3].Value;

                if (!dicMission.ContainsKey(rowMission))
                    continue;

                MemberofSection.MissionofSection mission = dicMission[rowMission];
                mission.CheckItems.Add(checkItem);
            }
        }

        public void SetMissionData(MemberofSection missionData)
        {
            if (m_currentMission == missionData)
                return;

            SaveMission();

            m_currentMission = missionData;

            if (m_currentMission == null)
            {
                this.textMember.Text = "";
                this.textCellPhone1.Text = "";
                this.textCellPhone2.Text = "";
                this.textCellPhone3.Text = "";
                this.textPhone1.Text = "";
                this.textPhone2.Text = "";
                this.textPhone3.Text = "";
                this.textMessenger.Text = "";

                dataGridViewMission.Rows.Clear();
                dataGridViewCheck.Rows.Clear();

                dataGridViewMission.Rows.Add();
            }
            else
            {
                this.textMember.Text = m_currentMission.Member;
                this.textCellPhone1.Text = m_currentMission.CellPhone1;
                this.textCellPhone2.Text = m_currentMission.CellPhone2;
                this.textCellPhone3.Text = m_currentMission.CellPhone3;
                this.textPhone1.Text = m_currentMission.Telephone1;
                this.textPhone2.Text = m_currentMission.Telephone2;
                this.textPhone3.Text = m_currentMission.Telephone3;
                this.textMessenger.Text = m_currentMission.MessengerID;

                dataGridViewMission.Rows.Clear();
                dataGridViewCheck.Rows.Clear();
                //dataGridViewMission.Rows.Add();

                //if(dataGridViewMission.Rows.Count >= 1)
                {
                    foreach (MemberofSection.MissionofSection mission in m_currentMission.Missions)
                    {
                        DataGridViewRow gridRow = new DataGridViewRow();
                        DataGridViewCell cell = new DataGridViewTextBoxCell();

                        cell.Value = mission.Division;
                        gridRow.Cells.Add(cell);

                        cell = new DataGridViewTextBoxCell();
                        cell.Value = mission.TaskName;
                        gridRow.Cells.Add(cell);

                        cell = new DataGridViewTextBoxCell();
                        cell.Value = mission.Report;
                        gridRow.Cells.Add(cell);
                        
                        cell = new DataGridViewTextBoxCell();
                        cell.Value = "";
                        gridRow.Cells.Add(cell);
                        
                        dataGridViewMission.Rows.Add(gridRow);

                        ArrayList arrCheckItems = mission.CheckItems;

                        foreach (MemberofSection.CheckofMission checkItem in arrCheckItems)
                        {
                            DataGridViewRowEx<DataGridViewRow> row = new DataGridViewRowEx<DataGridViewRow>();
                            row.Data = gridRow;

                            cell = new DataGridViewTextBoxCell();
                            cell.Value = checkItem.Category;
                            row.Cells.Add(cell);

                            cell = new DataGridViewTextBoxCell();
                            cell.Value = checkItem.SubCategory;
                            row.Cells.Add(cell);

                            cell = new DataGridViewTextBoxCell();
                            cell.Value = checkItem.TaskName;
                            row.Cells.Add(cell);

                            cell = new DataGridViewTextBoxCell();
                            cell.Value = checkItem.Count;
                            row.Cells.Add(cell);

                            cell = new DataGridViewTextBoxCell();
                            cell.Value = "";
                            row.Cells.Add(cell);

                            dataGridViewCheck.Rows.Add(row);
                        }
                    }
                }

                if (dataGridViewMission.Rows.Count == 0)
                    dataGridViewMission.Rows.Add();
            }
        }
        
        public void NewSOP()
        {
            textMember.Text = "";
            textCellPhone1.Text = "";
            textCellPhone2.Text = "";
            textCellPhone3.Text = "";
            textPhone1.Text = "";
            textPhone2.Text = "";
            textPhone3.Text = "";
            textMessenger.Text = "";

            if (m_currentMission != null)
                m_currentMission.Missions.Clear();
            dataGridViewMission.Rows.Clear();
            dataGridViewCheck.Rows.Clear();
            dataGridViewMission.Rows.Add();
        }

        private void tsMenuAddMission_Click(object sender, EventArgs e)
        {
            if (m_strCellValue == "")
            {
                MessageBox.Show("임무를 추가하려면 먼저 구분을 입력하여야 합니다.");
                return;
            }

            DataGridViewRow gridRow = new DataGridViewRow();
            DataGridViewCell cell = new DataGridViewImageCell();

            cell = new DataGridViewTextBoxCell();
            cell.Value = m_strCellValue;

            gridRow.Cells.Add(cell);
            dataGridViewMission.Rows.Insert(m_nCellRow + 1, gridRow);

            dataGridViewMission.ClearSelection();
            dataGridViewMission.Rows[m_nCellRow + 1].Cells[0].Selected = true;
        }

        private void tsMenuDeleteMission_Click(object sender, EventArgs e)
        {
            if (m_nCellRow > 0)
                dataGridViewMission.Rows.RemoveAt(m_nCellRow);
        }

        private void tsMenuAddCheck_Click(object sender, EventArgs e)
        {
            DataGridViewRowEx<DataGridViewRow> gridRow = new DataGridViewRowEx<DataGridViewRow>();
            DataGridViewRowEx<DataGridViewRow> selectedRow = (DataGridViewRowEx<DataGridViewRow>)dataGridViewCheck.Rows[m_nCheckCellRow];
            gridRow.Data = selectedRow.Data;

            DataGridViewCell cell = new DataGridViewImageCell();

            cell = new DataGridViewTextBoxCell();
            cell.Value = m_strCellValue;

            gridRow.Cells.Add(cell);
            dataGridViewCheck.Rows.Insert(m_nCheckCellRow + 1, gridRow);

            dataGridViewCheck.ClearSelection();
            dataGridViewCheck.Rows[m_nCheckCellRow + 1].Cells[1].Selected = true;
        }

        private void tsMenuDeleteCheck_Click(object sender, EventArgs e)
        {
            dataGridViewCheck.Rows.RemoveAt(m_nCheckCellRow);
        }

        private void dataGridViewCheck_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextCheckMenu.Show(dataGridViewCheck, new Point(e.X, e.Y));

                DataGridView.HitTestInfo hInfo = dataGridViewCheck.HitTest(e.X, e.Y);
                if (hInfo.ColumnIndex < 0 || hInfo.RowIndex < 0) return;

                if (dataGridViewCheck.Rows[hInfo.RowIndex].Cells[0].Value == null) return;
                m_nCheckCellRow = hInfo.RowIndex;
            }
        }
    }

    public class MemberofSection
    {
        public class MissionofSection
        {
            string m_strDivision;
            string m_strTaskName;
            string m_strReport;
            string m_strDescription;
            private ArrayList m_arrCheckItems = new ArrayList();

            public string Division
            {
                get { return m_strDivision; }
                set { m_strDivision = value; }
            }

            public string TaskName
            {
                get { return m_strTaskName; }
                set { m_strTaskName = value; }
            }

            public string Report
            {
                get { return m_strReport; }
                set { m_strReport = value; }
            }

            public string Description
            {
                get { return m_strDescription; }
                set { m_strDescription = value; }
            }

            public ArrayList CheckItems
            {
                get { return m_arrCheckItems; }
                set { m_arrCheckItems = value; }
            }
        }

        public class CheckofMission
        {
            string m_strCategory;
            string m_strSubCategory;
            string m_strTaskName;
            string m_strCount;
            string m_strDescription;

            public string Category
            {
                get { return m_strCategory; }
                set { m_strCategory = value; }
            }

            public string SubCategory
            {
                get { return m_strSubCategory; }
                set { m_strSubCategory = value; }
            }

            public string TaskName
            {
                get { return m_strTaskName; }
                set { m_strTaskName = value; }
            }

            public string Count
            {
                get { return m_strCount; }
                set { m_strCount = value; }
            }

            public string Description
            {
                get { return m_strDescription; }
                set { m_strDescription = value; }
            }
        }

        string m_strMember;
        string m_strCellphone1;
        string m_strCellphone2;
        string m_strCellphone3;
        string m_strTelephone1;
        string m_strTelephone2;
        string m_strTelephone3;
        string m_strMessengerID;
        SectionTimeText m_linkedSection = null;

        private ArrayList m_arrMissions = new ArrayList();

        public string Member
        {
            get { return m_strMember; }
            set { m_strMember = value; }
        }

        public string CellPhone1
        {
            get { return m_strCellphone1; }
            set { m_strCellphone1 = value; }
        }

        public string CellPhone2
        {
            get { return m_strCellphone2; }
            set { m_strCellphone2 = value; }
        }

        public string CellPhone3
        {
            get { return m_strCellphone3; }
            set { m_strCellphone3 = value; }
        }

        public string Telephone1
        {
            get { return m_strTelephone1; }
            set { m_strTelephone1 = value; }
        }

        public string Telephone2
        {
            get { return m_strTelephone2; }
            set { m_strTelephone2 = value; }
        }

        public string Telephone3
        {
            get { return m_strTelephone3; }
            set { m_strTelephone3 = value; }
        }

        public string MessengerID
        {
            get { return m_strMessengerID; }
            set { m_strMessengerID = value; }
        }

        public ArrayList Missions
        {
            get { return m_arrMissions; }
            set { m_arrMissions = value; }
        }

        public SectionTimeText LinkedSection
        {
            get { return m_linkedSection; }
            set
            {
                m_linkedSection = value;
                if (m_linkedSection != null)
                {
                    if (m_linkedSection.MissionData != this)
                        m_linkedSection.MissionData = this;
                }
            }
        }
    }
}
