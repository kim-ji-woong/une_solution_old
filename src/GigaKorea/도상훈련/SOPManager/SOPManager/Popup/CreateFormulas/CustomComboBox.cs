using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOPManager.Popup.CreateFormulas
{
    public class CustomComboBox : Panel
    {
        public ComboBox ComboBox
        {
            get { return m_ComboBox; }
            set { m_ComboBox = value; }
        }
        private ComboBox m_ComboBox = new ComboBox();

        public Label Label
        {
            get { return m_Label; }
            set { m_Label = value; }
        }
        private Label m_Label = new Label();
        private Font m_Font = new Font("나눔스퀘어", 12.5f, FontStyle.Bold);
        
        public CustomComboBox()
        {
            this.DoubleBuffered = true;

            m_ComboBox.DisplayMember = "StrDisplay";
            m_ComboBox.ValueMember = "StrValue";

            this.Size = new Size(100,40);
            m_ComboBox.Font = m_Font;
            m_ComboBox.Size = this.Size;
            m_ComboBox.DropDownWidth = 120;
            m_ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            m_ComboBox.Location = new Point(0, 0);
            m_ComboBox.SelectedIndexChanged += M_ComboBox_SelectedIndexChanged;
            m_ComboBox.DropDownClosed += M_ComboBox_DropDownClosed;
            m_ComboBox.MouseLeave += M_ComboBox_MouseLeave;
            m_ComboBox.Parent = this;

            m_Label.Font = m_Font;
            m_Label.AutoSize = true;
            m_Label.Size = this.Size;
            m_Label.Location = new Point(0, 0);
            m_Label.TextAlign = ContentAlignment.MiddleLeft;
            m_Label.BackColor = Color.Transparent;
            m_Label.MouseEnter += M_Label_MouseEnter;
            m_Label.TextChanged += M_label_TextChanged;
            m_Label.Parent = this;

            this.SizeChanged += CustomComboBox_SizeChanged;
        }

        private void CustomComboBox_SizeChanged(object sender, EventArgs e)
        {
            this.Size = new Size(this.Width, m_ComboBox.Height + 10);
            m_ComboBox.Size = new Size(this.Width, m_ComboBox.Height);
            m_Label.Size = m_ComboBox.Size;

            m_ComboBox.Location = new Point(this.Width / 2 - m_ComboBox.Width / 2, this.Height / 2 - m_ComboBox.Height / 2);
            m_Label.Location = new Point(this.Width / 2 - m_Label.Width / 2, this.Height / 2 - m_Label.Height / 2);
        }

        #region ComboBox 이벤트
        private void M_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CustomComboBoxItem combo = m_ComboBox.SelectedItem as CustomComboBoxItem;
            if (combo != null)
            {
                if (combo.StrValue == "true" || combo.StrValue == "false") 
                    m_Label.Text = "= " + combo.StrValue;
                else
                    m_Label.Text = combo.StrValue;

                m_Label.Visible = true;
                m_ComboBox.Visible = false;
            }
        }

        private void M_ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            m_ComboBox.Visible = false;
            m_Label.Visible = true;
        }

        private void M_ComboBox_MouseLeave(object sender, EventArgs e)
        {
            m_Label.Visible = true;
            m_ComboBox.Visible = false;
        }
        #endregion

        #region Label 이벤트
        private void M_Label_MouseEnter(object sender, EventArgs e)
        {
            if (m_Label.Visible && !m_ComboBox.Visible)
            {
                m_Label.Visible = false;
                m_ComboBox.Visible = true;
            }
        }

        private void M_label_TextChanged(object sender, EventArgs e)
        {
            m_Label.Location = new Point(this.Width / 2 - m_Label.PreferredSize.Width / 2, this.Height / 2 - m_Label.PreferredSize.Height / 2);

            m_Label.Size = m_Label.PreferredSize;      
        } 
        #endregion
    }

    public class CustomComboBoxItem
    {
        public string StrValue { get; set; }
        public string StrDisplay { get; set; }

        public CustomComboBoxItem(string value, string display)
        {
            this.StrValue = value;
            this.StrDisplay = display;
        }
    }
}
