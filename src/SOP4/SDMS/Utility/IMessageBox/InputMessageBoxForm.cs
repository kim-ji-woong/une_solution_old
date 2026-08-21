using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace SDMS.IMessageBox
{
    internal partial class InputMessageBoxForm : Form
    {
        private ArrayList mArLabel = new ArrayList();
        private string m_szText = "";

        private bool m_visibleTextBox = true;
        private string m_strDescriptionText = "";

        public bool VisibleTextBox
        {
            get { return m_visibleTextBox; }
            set { m_visibleTextBox = value; }
        }

        public string DescriptionInfo
        {
            get { return labelDescription.Text; }
            set
            {
                labelDescription.Text = value;
                m_visibleTextBox = value.Length > 0;
            }
        }

        public string DescriptionText
        {
            get { return m_strDescriptionText; }
        }

        public InputMessageBoxForm()
        {
            InitializeComponent();
            mIconBox.BackgroundImage = Bitmap.FromHicon(System.Drawing.SystemIcons.Information.Handle);
            label1.Visible = false;
            labelDescription.Text = "";

            DialogResult = DialogResult.Cancel;
        }

        // 자동완성 기능
        public void SetAutoCompleteSource(List<string> descriptionList)
        {
            foreach (string strDescription in descriptionList)
            {
                textBoxDescription.AutoCompleteCustomSource.Add(strDescription);
            }
        }
                
        internal void SetFont(Font font)
        {
            this.Font = font;
            

            UpdateText();
        }

        internal void SetForeColor(Color color)
        {
            this.ForeColor = color;
          
            UpdateText();
        }

        internal void SetBackColor(Color color)
        {
            this.BackColor = color;
            UpdateText();
        }

        internal void SetIcon(MessageBoxIcon icon)
        {
            if( icon == MessageBoxIcon.Asterisk)
            {
                mIconBox.BackgroundImage = Bitmap.FromHicon(System.Drawing.SystemIcons.Asterisk.Handle);
            }
            else if (icon == MessageBoxIcon.Exclamation)
            {
                mIconBox.BackgroundImage = Bitmap.FromHicon(System.Drawing.SystemIcons.Exclamation.Handle);
            }
            else if (icon == MessageBoxIcon.Error)
            {
                mIconBox.BackgroundImage = Bitmap.FromHicon(System.Drawing.SystemIcons.Error.Handle);
            }
            else if (icon == MessageBoxIcon.Hand)
            {
                mIconBox.BackgroundImage = Bitmap.FromHicon(System.Drawing.SystemIcons.Hand.Handle);
            }
            else if (icon == MessageBoxIcon.Information)
            {
                mIconBox.BackgroundImage = Bitmap.FromHicon(System.Drawing.SystemIcons.Information.Handle);
            }
            else if (icon == MessageBoxIcon.None)
            {
                mIconBox.BackgroundImage = null;
            }
            else if (icon== MessageBoxIcon.Question)
            {
                mIconBox.BackgroundImage = Bitmap.FromHicon(System.Drawing.SystemIcons.Question.Handle);
            }
            else if (icon == MessageBoxIcon.Warning)
            {
                mIconBox.BackgroundImage = Bitmap.FromHicon(System.Drawing.SystemIcons.Warning.Handle);
            }
            else if (icon == MessageBoxIcon.Stop)
            {
                mIconBox.BackgroundImage = Bitmap.FromHicon(System.Drawing.SystemIcons.Shield.Handle);
            }
        }
         
        private void UpdateText()
        {
            if (m_szText == null || m_szText == "")
                return;

            foreach (Label lb in mArLabel)
            {
                this.Controls.Remove(lb);
            }
            mArLabel.Clear();
			float maxWidth = float.MinValue;
            // label text
            using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(new Bitmap(1, 1)))
            {
				
                string[] sepList = { "\n\r", "\r", "\n" };
                string[] textList = m_szText.Split(sepList, StringSplitOptions.RemoveEmptyEntries);
                Point pt = label1.Location;
                float fHeight = 10.0f;
                foreach (string text in textList)
                {
                    SizeF size = graphics.MeasureString(text, this.Font);

                    Label label = new Label();
                    label.Text = text;
                    label.Font = this.Font;
                    label.ForeColor = this.ForeColor;
                    label.BackColor = this.BackColor;
                    label.AutoSize = true;
                    float lineSpace = size.Height / 3.0f;

                    fHeight += lineSpace;
                    label.Location = new Point(pt.X, (int)fHeight);
                    label.Visible = true;
                    fHeight += size.Height;

					if (maxWidth < size.Width)
					{
						maxWidth = size.Width;
					}

                    this.Controls.Add(label);
                    mArLabel.Add(label);
                }

                fHeight += 20.0f;

                float iconLocation = (fHeight + mIconBox.Height) / 2.0f;
                mIconBox.Location = new Point(mIconBox.Location.X, (int)iconLocation);

                button1.Font = this.Font;
                button2.Font = this.Font;
                button3.Font = this.Font;
                button4.Font = this.Font;

                float buttonLocation = fHeight;
                SizeF bSize = graphics.MeasureString("AAA", this.Font);
                int buttonHeight = (int)(bSize.Height + 10.0f);
                button1.Location = new Point(button1.Location.X, (int)buttonLocation);
                button1.Size = new Size(button1.Size.Width, buttonHeight);
                button2.Location = new Point(button2.Location.X, (int)buttonLocation);
                button2.Size = new Size(button2.Size.Width, buttonHeight);
                button3.Location = new Point(button3.Location.X, (int)buttonLocation);
                button3.Size = new Size(button3.Size.Width, buttonHeight);
                button4.Location = new Point(button4.Location.X, (int)buttonLocation);
                button4.Size = new Size(button4.Size.Width, buttonHeight);
                
                pictureBox1.Height = 40;
                pictureBox1.SendToBack();

                labelDescription.Location = new Point(labelDescription.Location.X, button1.Location.Y + button1.Size.Height + 20);
                textBoxDescription.Location = new Point(textBoxDescription.Location.X, labelDescription.Location.Y + labelDescription.Size.Height + 3);
                int textBoxHeight = (textBoxDescription.Location.Y + textBoxDescription.Size.Height) - (button1.Location.Y + button1.Size.Height);

                if (VisibleTextBox == false)
                {
                    textBoxHeight = 0;
                    labelDescription.Visible = textBoxDescription.Visible = false;
                }
                
				int dlgWidth = (int)(pt.X + maxWidth) + 20;

				int width = Math.Max(this.Size.Width, dlgWidth);
				this.Size = new Size(width, (int)fHeight + button1.Height + 40 + textBoxHeight);
                
            }
        }

        internal void DialogText(string szText)
        {
            m_szText = szText;
            UpdateText();      
        }

        internal void SetButtons(MessageBoxButtons buttons)
        {
            if( buttons == MessageBoxButtons.OK)
            {
                int locationX = this.Size.Width / 2 - button1.Width / 2;
                button1.Location = new Point(locationX, button1.Location.Y);
                button1.Visible = true;
                button2.Visible = false;
                button3.Visible = false;
                button4.Visible = false;
            }           

            else if( buttons == MessageBoxButtons.OKCancel)
            {
                int locationX = this.Size.Width / 2 - ( button1.Width + button2.Width + 10 ) / 2 ;
                button1.Location = new Point(locationX, button1.Location.Y);
                button1.Visible = true;
                locationX += button1.Width + 5;
                button2.Location = new Point(locationX, button2.Location.Y);
                button2.Visible = true;
                button3.Visible = false;
                button4.Visible = false;
                DialogResult = DialogResult.Cancel;
            }

            else if (buttons == MessageBoxButtons.YesNo)
            {
                int nWidth = (button3.Width + button4.Width + 20) / 2;
                int locationX = this.Size.Width / 2 - nWidth;
                button3.Location = new Point(locationX, button3.Location.Y);
                button3.Visible = true;

                locationX += button2.Width + 5;
                button4.Location = new Point(locationX, button4.Location.Y);
                button4.Visible = true;
  
                button2.Visible = false;
                button1.Visible = false;
                DialogResult = DialogResult.No;
            }
            else if (buttons == MessageBoxButtons.YesNoCancel)
            {
                int nWidth = (button2.Width + button3.Width + button4.Width + 20) / 2;
                int locationX = this.Size.Width / 2 - nWidth;
                button3.Location = new Point(locationX, button3.Location.Y);
                button3.Visible = true;

                locationX += button2.Width + 5;
                button4.Location = new Point(locationX, button4.Location.Y);
                button4.Visible = true;

                locationX += button3.Width + 5;
                button2.Location = new Point(locationX, button2.Location.Y);
                button2.Visible = true;
                button1.Visible = false;
                DialogResult = DialogResult.Cancel;
            }
            else
            {
                throw new NotImplementedException("구현되지 않았음. 새로 구현하세요.");
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            m_strDescriptionText = textBoxDescription.Text;
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            m_strDescriptionText = textBoxDescription.Text;
            DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            if(this.Owner != null)
            {
                this.Owner.DialogResult = DialogResult.Cancel;
            }           
            this.Close();
        }

        private void InputMessageBoxForm_Load(object sender, EventArgs e)
        {
           
        }

        private void InputMessageBoxForm_FormClosing(object sender, FormClosingEventArgs e)
        { 
                 
        }
    }
}
