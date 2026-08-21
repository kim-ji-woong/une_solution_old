using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using static Sections.SectionDataDecision;

namespace SOPManager.Popup.CreateFormulas
{
    public class OneFormula : Panel
    {
        private Panel m_pnVariable = new Panel();
        public Panel PnVariable
        {
            get { return m_pnVariable; }
            set { m_pnVariable = value; }
        }

        private CustomComboBox m_cbCondition = new CustomComboBox();
        public CustomComboBox CbCondition
        {
            get { return m_cbCondition; }
            set { m_cbCondition = value; }
        }

        private CustomTextBox m_tbValue = new CustomTextBox();
        public CustomTextBox TbValue
        {
            get { return m_tbValue; }
            set { m_tbValue = value; }
        }
        
        public Dictionary<Sections.SectionDataDecision.VariableType, List<CustomComboBoxItem>> Condition = null;

        private Font m_Font = new Font("나눔스퀘어", 12.5f, FontStyle.Bold);
        private Color mOutLineColor = Color.FromArgb(95, 146, 201);

        private int m_nPanelWidth = 200;
        private int m_nPanelHeight = 40;
        private int m_nSpace = 2;

        public Sections.SectionDataDecision.VariableType CurrentVariableType = Sections.SectionDataDecision.VariableType.UNKNOWN;

        private bool m_bMouseEnter = false;
        public bool bMouseEnter
        {
            get { return m_bMouseEnter; }
            set { m_bMouseEnter = value; }
        }
        private bool m_bMouseDown = false;
        public bool bMouseDown
        {
            get { return m_bMouseDown; }
            set { m_bMouseDown = value; }
        }
        private Pen m_penClick = new Pen(Color.Red);
        private Pen m_backPen = new Pen(Color.FromArgb(250, 0xff, 0x00, 0x00));
        private Brush m_backBrush = new SolidBrush(Color.FromArgb(50, 0xff, 0x00, 0x00));

        private string m_strDisplay = "";
        private string m_strValue = "";

        public OneFormula()
        {
            this.DoubleBuffered = true;
            this.BackColor = Color.Transparent;// Color.FromArgb(100, 0xf8, 0xce, 0xf1);
            this.Size = new Size(m_nPanelWidth + (m_nSpace * 2), m_nPanelHeight);
            InitControl();

            m_penClick.Width = 3;
            PopupNoteDecision3.Instance.SetDoubleBuffer(PnVariable, true);

            m_cbCondition.Label.TextChanged += Label_TextChanged;

            this.Paint += OneFormula_Paint;
            this.MouseEnter += OneFormula_MouseEnter;
            this.MouseLeave += OneFormula_MouseLeave;            
            m_pnVariable.MouseEnter += OneFormula_MouseEnter;
            m_pnVariable.MouseLeave += OneFormula_MouseLeave;
        }

        private void Label_TextChanged(object sender, EventArgs e)
        {
            ResizeControl();
            MakeStrVariable();
        }

        private void OneFormula_Paint(object sender, PaintEventArgs e)
        {
            if (!m_bMouseEnter && !m_bMouseDown)
                return;

            Graphics g = e.Graphics;

            int width = this.Width;
            int height1 = CbCondition.Location.Y - 2;
            int height2 = CbCondition.Location.Y + CbCondition.Height + 2;

            if (m_bMouseDown)
            {   
                g.DrawLine(m_penClick, 0, height1, width, height1);
                g.DrawLine(m_penClick, width - 1, height1, width - 1, height2);
                g.DrawLine(m_penClick, width, height2 - 1, 0, height2 - 1);
                g.DrawLine(m_penClick, 0, height2, 0, height1); 
            }

            if (m_bMouseEnter)
            {
                g.FillRectangle(m_backBrush, 0, height1, this.Width, height2 - height1 + 1);
            }
        }

        private void OneFormula_MouseLeave(object sender, EventArgs e)
        {
            m_bMouseEnter = false;
            this.Refresh();
        }

        private void OneFormula_MouseEnter(object sender, EventArgs e)
        {
            m_bMouseEnter = true;
            this.Refresh();
        }

        private void InitControl()
        {
            m_pnVariable.Size = new Size(90, 30);
            m_pnVariable.Location = new Point(0, this.Height / 2 - m_pnVariable.Height / 2);
            m_pnVariable.BackColor = Color.Transparent; //mOutLineColor;// Color.FromArgb(247, 169, 43);
            m_pnVariable.AllowDrop = true;
            m_pnVariable.DragEnter += Variable_DragEnter;
            m_pnVariable.DragDrop += Variable_DragDrop;
            m_pnVariable.DragLeave += Variable_DragLeave;
            m_pnVariable.DragOver += Variable_DragOver;
            m_pnVariable.Paint += Variable_Paint;

            m_cbCondition.Font = m_Font;
            m_cbCondition.Size = new Size((this.Width - m_pnVariable.Width - (m_nSpace * 6)) / 2, 30);
            m_cbCondition.Location = new Point(this.Width / 2 - m_pnVariable.Width / 2, this.Height / 2 - m_cbCondition.Height / 2);
            m_cbCondition.MouseLeave += M_cbCondition_MouseLeave;


            m_tbValue.Font = m_Font;
            m_tbValue.Size = new Size((this.Width - m_cbCondition.Width - (m_nSpace * 6)) / 2, 30);
            m_tbValue.Location = new Point(m_cbCondition.Location.X + m_cbCondition.Width, this.Height / 2 - m_tbValue.Height / 2);
            
            m_cbCondition.Visible = m_tbValue.Visible = false;

            this.Controls.Add(m_pnVariable);
            this.Controls.Add(m_cbCondition);
            this.Controls.Add(m_tbValue);
        }

        private void M_cbCondition_MouseLeave(object sender, EventArgs e)
        {
            CustomComboBox customCb = sender as CustomComboBox;
            if (customCb != null)
            {
                customCb.ComboBox.Visible = false;
            }
        }

        private void Variable_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DataGridView)))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        public void Variable_DragDrop(object sender, DragEventArgs e)
        {
            DataGridView gridView = (DataGridView)e.Data.GetData(typeof(DataGridView));

            Panel variable = sender as Panel;
            if (gridView == null)
            {
                if (variable != null)
                    variable.Visible = false;

                return;
            }

            variable.BackColor = Color.Transparent;
            DataGridViewRow row = gridView.CurrentRow;
            SetVariable(row);
        }

        public void SetVariable(DataGridViewRow row)
        {   
            m_pnVariable.Tag = row;

            m_strValue = row.Cells[0].Value.ToString();
            m_strDisplay = row.Cells[2].Value.ToString();

            Graphics g = m_pnVariable.CreateGraphics();
            SizeF valueSize = g.MeasureString(m_strValue, m_Font);
            pnSize = new Size((int)valueSize.Width + m_nSpace, 30);

            bool txtVisible = true;
            Sections.SectionDataDecision.VariableType type = Sections.SectionDataDecision.ToVariableType(row.Cells[1].Value.ToString());
            this.CurrentVariableType = type;

            if (type == Sections.SectionDataDecision.VariableType.BOOLEAN)
                txtVisible = false;

            foreach (Control child in this.Controls)
            {
                CustomComboBox customCb = child as CustomComboBox;
                if (customCb != null)
                {
                    if (customCb.ComboBox != null)
                    {
                        customCb.Visible = true;
                        customCb.ComboBox.Items.Clear();

                        foreach (var item in Condition[type])
                        {
                            customCb.ComboBox.Items.Add(item);
                        }
                        customCb.ComboBox.SelectedIndex = 0;
                    }
                }

                CustomTextBox txt = child as CustomTextBox;
                if (txt != null)
                {
                    txt.TextBox.Text = "";
                    if (txtVisible)
                    {
                        txt.Visible = true;
                        txt.TextBox.Visible = true;
                    }

                    else
                        txt.Visible = false;
                }
            }
            ResizeControl();
            //MakeStrVariable();
        }

        private void Variable_DragLeave(object sender, EventArgs e)
        {
            Panel pn = sender as Panel;
            if (pn == null)
                return;

            pn.BackColor = Color.Transparent;
        }

        private void Variable_DragOver(object sender, DragEventArgs e)
        {
            Panel pn = sender as Panel;
            if (pn == null)
                return;

            pn.BackColor = mOutLineColor;
        }
                
        private void Variable_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            if (panel == null)
                return;

            DataGridViewRow row = panel.Tag as DataGridViewRow;
            if (row == null)
                return;

            Graphics g = e.Graphics;
            string value = row.Cells[0].Value.ToString();
            SizeF valueSize = g.MeasureString(value, m_Font);
            pnSize = new Size((int)valueSize.Width + m_nSpace, 30);
            g.DrawString(value, m_Font, Brushes.Black, new PointF(panel.Width / 2 - valueSize.Width / 2, panel.Height / 2 - valueSize.Height / 2));
        }
        private Size pnSize = new Size(90, 30);
        public void ResizeControl()
        {
            //DataGridViewRow tag = m_pnVariable.Tag as DataGridViewRow;
            //if (tag == null)
            //    m_pnVariable.Size = new Size(90, 30);
            //else
            //{
            //    string value = tag.Cells[0].Value.ToString();
            //    SizeF valueSize = g.MeasureString(value, m_Font);
            //    m_pnVariable.Size = new Size((int)valueSize.Width + m_nSpace, 30);
            //}
            m_pnVariable.Size = pnSize;
            m_pnVariable.Location = new Point(0, this.Height / 2 - m_pnVariable.Height / 2);

            m_cbCondition.Size = m_cbCondition.Label.Size;

            m_cbCondition.Location = new Point(m_pnVariable.Location.X + m_pnVariable.Width + m_nSpace, this.Height / 2 - m_cbCondition.Height / 2);
            m_tbValue.Location = new Point(m_cbCondition.Location.X + m_cbCondition.Width + m_nSpace, this.Height / 2 - m_tbValue.Height / 2);

            if (m_tbValue.Visible)
                this.Size = new Size(m_nSpace * 3 + m_pnVariable.Width + m_cbCondition.Width + m_tbValue.Width, this.Height);
            else
                this.Size = new Size(m_nSpace * 2 + m_pnVariable.Width + m_cbCondition.Width, this.Height);

            PopupNoteDecision3.Instance.ResizeControl();
        }

        public void GetStrVariable(ref string strDisplay, ref string strValue)
        {
            if (m_strValue.Length == 0 || m_strDisplay.Length == 0)
                return;

            strValue = m_strValue;
            string temp = m_strDisplay + "{은는}";
            strDisplay += UnE.Utility.SOPSimulatorScript.Parse(temp, new DateTime(), "");

            CustomComboBoxItem combo = m_cbCondition.ComboBox.SelectedItem as CustomComboBoxItem;
            string temp3 = (combo != null) ? " " + combo.StrDisplay : "";

            string temp2 = "";
            if (CurrentVariableType == Sections.SectionDataDecision.VariableType.BOOLEAN)
            {
                strValue += " = " + combo.StrValue;
            }
            else
            {
                strValue += " " + combo.StrValue;
                temp2 = m_tbValue.TextBox.Text;

                if (CurrentVariableType == Sections.SectionDataDecision.VariableType.STRING)
                {
                    temp2 = "'" + temp2 + "'";
                    if (temp3.Contains("포함"))
                    {
                        temp2 += "{이가}";
                        if (temp3.Trim() == "포함")
                            temp3 = " 포함됨";
                    }
                    else
                        temp2 += "{과와}";

                    strValue += " '" + m_tbValue.TextBox.Text + "'";
                }
                else if (CurrentVariableType == Sections.SectionDataDecision.VariableType.DOUBLE || CurrentVariableType == Sections.SectionDataDecision.VariableType.INTEGER)
                {
                    if (temp3.Contains("같다") || temp3.Contains("다르다"))
                        temp2 += "{과와}";

                    strValue += " " + m_tbValue.TextBox.Text;
                }

                strDisplay += " " + UnE.Utility.SOPSimulatorScript.Parse(temp2, new DateTime(), "");                
            }            

            strDisplay += temp3;
            
        }

        private bool IsHangul(int ch)
        {
            return ch >= 0xac00 && ch <= 0xd7a3;
        }

        public void MakeStrVariable()
        {
            PopupNoteDecision3.Instance.MakeStrVariable();
        }        
        public void NullStr()
        {
            m_strDisplay = "";
            m_strValue = "";
        }
    }
}
