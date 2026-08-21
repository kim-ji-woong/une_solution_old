using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOPManager.Popup.CreateFormulas
{
    public class CustomTextBox : Panel
    {
        public TextBox TextBox
        {
            get { return m_TextBox; }
            set { m_TextBox = value; }
        }
        private TextBox m_TextBox = new TextBox();

        public Label Label
        {
            get { return m_Label; }
            set { m_Label = value; }
        }
        private Label m_Label = new Label();
        private Font m_Font = new Font("나눔스퀘어", 12.5f, FontStyle.Bold);
        
        public CustomTextBox()
        {
            this.DoubleBuffered = true;

            this.Size = new Size(100,40);
            m_TextBox.Font = m_Font;
            m_TextBox.Size = this.Size;
            m_TextBox.Location = new Point(0, 0);
            m_TextBox.KeyDown += M_TextBox_KeyDown;
            m_TextBox.TextChanged += M_TextBox_TextChanged;
            m_TextBox.Parent = this;

            m_Label.Font = m_Font;
            m_Label.AutoSize = true;
            m_Label.Size = this.Size;
            m_Label.Location = new Point(0, 0);
            m_Label.TextAlign = ContentAlignment.MiddleLeft;
            m_Label.BackColor = Color.Transparent;
            m_Label.TextChanged += M_label_TextChanged;
            m_Label.MouseDown += M_Label_MouseDown;
            m_Label.Parent = this;

            this.SizeChanged += CustomComboBox_SizeChanged;            
            this.MouseLeave += CustomTextBox_MouseLeave;
        }
        
        private void M_Label_MouseDown(object sender, MouseEventArgs e)
        {
            m_TextBox.Visible = true;
            m_TextBox.Focus();
        }

        private void M_TextBox_TextChanged(object sender, EventArgs e)
        {
            m_Label.Text = m_TextBox.Text;
        }

        private void M_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (m_Label.Text.Length > 0)
                    m_TextBox.Visible = false;
            }
        }

        private void CustomTextBox_MouseLeave(object sender, EventArgs e)
        {
            if (m_Label.Text.Length > 0)
            {
                m_TextBox.Visible = false;
                CustomComboBox_SizeChanged(null, null);
            }
        }

        private void CustomComboBox_SizeChanged(object sender, EventArgs e)
        {
            m_TextBox.Size = new Size(this.Width, m_TextBox.Height);
            m_Label.Size = new Size(this.Width, m_Label.Height);
            m_TextBox.Location = new Point(this.Width / 2 - m_TextBox.Width / 2, this.Height / 2 - m_TextBox.Height / 2);
            m_Label.Location = new Point(this.Width / 2 - m_Label.Width / 2, this.Height / 2 - m_Label.Height / 2);
        }
        #region Label 이벤트

        private void M_label_TextChanged(object sender, EventArgs e)
        {
            if (m_Label.PreferredSize.Width > 0)
                this.Size = new Size(m_Label.PreferredSize.Width + 15, m_TextBox.Height + 10);
            else
                this.Size = new Size(this.Width, m_TextBox.Height + 10);

            this.Size = new Size(this.Width, m_TextBox.Height + 10);
            m_TextBox.Size = new Size(this.Width, m_TextBox.Height);

            if (m_Label.PreferredSize.Width > 0)
                m_Label.Size = m_TextBox.Size = new Size(m_Label.PreferredSize.Width + 10, m_Label.PreferredSize.Height);
            else
                m_Label.Size = m_TextBox.Size = new Size(this.Width, m_Label.PreferredSize.Height);

            m_TextBox.Location = new Point(this.Width / 2 - m_TextBox.Width / 2, this.Height / 2 - m_TextBox.Height / 2);
            m_Label.Location = new Point(this.Width / 2 - m_Label.Width / 2, this.Height / 2 - m_Label.Height / 2);
            
            OneFormula parent = this.Parent as OneFormula;
            if (parent != null)
            {
                parent.ResizeControl();
                parent.MakeStrVariable();
            }
        } 
        #endregion
    }
}
