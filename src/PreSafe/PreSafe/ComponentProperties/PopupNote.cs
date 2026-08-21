using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UnE.SenarioMaker
{
    internal partial class PopupNote : Form
    {
        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();

        public string Text
        {
            get { return textBox.Text; }
            set { textBox.Text = value; }
        }

        public PopupNote(bool bExpresssion)
        {
            InitializeComponent();

            if (bExpresssion == true)
            {              
                ExpressionMode();
            }
            else
            {
                TextMode();
            }
            
            textBox.Text = "";

            CloseMacroGrid();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {            
            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void PopupNote_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = this.PointToScreen(new Point(e.X, e.Y));
            }          
        }       

        private void PopupNote_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_bLeftMouseDown == true)
                {
                    Point pt = this.PointToScreen(new Point(e.X, e.Y));
                    int dx = pt.X - m_ptMove.X;
                    int dy = pt.Y - m_ptMove.Y;
                    if (!(dx == 0 && dy == 0))
                    {
                        Point ptCur = this.Location;
                        this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
                        m_ptMove.X += dx;
                        m_ptMove.Y += dy;
                    }
                }
            }
        }

        private void PopupNote_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;
        }

        public void ExpressionMode()
        {
            this.Text = "수식 작성";
            labelNote.Text = "수식";
            mBtnMacro.Visible = true;
        }

        public void TextMode()
        {
            this.Text = "내용 작성";
            labelNote.Text = "내용";
            mBtnMacro.Visible = false;
            this.Size = new Size(Size.Width, Size.Height - mBtnMacro.Size.Height);
        }

        private bool m_bOpenMacro = false;
        private void mBtnMacro_Click(object sender, EventArgs e)
        {
            if(m_bOpenMacro == false)
            {
                
                OpenMacroGrid();
                mBtnMacro.Text = "<< 매크로(&M)";
            }
            else
            {
                CloseMacroGrid();
                mBtnMacro.Text = "매크로(&M) >>";
            }
        }

        public void OpenMacroGrid()
        {
            m_bOpenMacro = true;
            mBtnInsertMacro.Visible = true;
            mVariableGrid.Visible = true;
            this.Size = new Size(this.Size.Width, this.Size.Height + 190);
        }
        public void CloseMacroGrid()
        {
            m_bOpenMacro = false;
            mBtnInsertMacro.Visible = false;
            mVariableGrid.Visible = false;
            this.Size = new Size(this.Size.Width, this.Size.Height - 190);
        }

        private void AddGridItem(IEnumerable<Variable> varList, string szType)
        {
            try
            {                
                foreach (Variable var in varList)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.Tag = var;

                    DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
                    cell1.Value = var.Name;
                    row.Cells.Add(cell1);

                   
                    DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
                    cell2.Value = var.Value;
                    row.Cells.Add(cell2);

                    if (szType == "System")
                    {
                        cell2.Value = "System 제공";
                    }

                    DataGridViewTextBoxCell cell4 = new DataGridViewTextBoxCell();
                    cell4.Value = var.Description;
                    row.Cells.Add(cell4);

                    row.Tag = szType;

                    mVariableGrid.Rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message, ex);
            }
        }

        public void InitMacro()
        {
            Variables<Variable> sysVar = SenarioManager.Instance.SystemVariables;
            AddGridItem(sysVar.VarList, "System");
            Variables<UserVariable> userVar = SenarioManager.Instance.UserVariables;
            AddGridItem(userVar.VarList, "User");
            Variables<Enums> enumVar = SenarioManager.Instance.EnumList;
            AddGridItem(enumVar.VarList, "Enums");
        }

        private void PopupNoteEx_Load(object sender, EventArgs e)
        {
            InitMacro();
        }

        private void mBtnInsertMacro_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection rows = mVariableGrid.SelectedRows;
            foreach(DataGridViewRow row in rows)
            {
                string szInsertText = string.Format("$({0})", row.Cells[0].Value.ToString());
                textBox.Paste(szInsertText);
            }
        }

        private void mVariableGrid_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //if (e.RowIndex < 0 || mVariableGrid.RowCount <= e.RowIndex)
            //    return;

            //DataGridViewRow row = mVariableGrid.Rows[e.RowIndex];
            //if(row != null)
            //{
            //    string szInsertText = string.Format("$({0})", row.Cells[0].Value.ToString());
            //    textBox.Paste(szInsertText);
            //}
        }

        private void mVariableGrid_DoubleClick(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection rows = mVariableGrid.SelectedRows;
            foreach (DataGridViewRow row in rows)
            {
                string szInsertText = string.Format("$({0})", row.Cells[0].Value.ToString());
                textBox.Paste(szInsertText);
            }
        }
    }
}
