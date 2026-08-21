using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RoadMan
{
    public partial class FormColor : Form
    {
        private int m_nAlpha = 255;
        private Color m_color = Color.White;
        private Dictionary<TextBox, string> m_dicPrevText = new Dictionary<TextBox, string>();

        public int Alpha
        {
            get { return m_nAlpha; }
            set { m_nAlpha = value; }
        }

        public Color Color
        {
            get { return m_color; }
            set { m_color = value; }
        }

        public FormColor()
        {
            InitializeComponent();
        }

        private void FormColor_Load(object sender, EventArgs e)
        {
            trackBarAlpha.SetRange(0, 255);
            trackBarAlpha.Value = m_nAlpha;
            textBoxAlpha.Text = m_nAlpha.ToString();

            btnColor.BackColor = Color.FromArgb(m_nAlpha, m_color);
            SetRGB();

            // Key 입력을 받기 위하여 아무 Control이나 Focus
            textBoxR.Select();
            textBoxR.Select(0, 0);
        }

        private void SetRGB()
        {
            textBoxR.Text = btnColor.BackColor.R.ToString();
            textBoxG.Text = btnColor.BackColor.G.ToString();
            textBoxB.Text = btnColor.BackColor.B.ToString();
        }

        private void trackBarAlpha_ValueChanged(object sender, EventArgs e)
        {
            m_nAlpha = trackBarAlpha.Value;
            textBoxAlpha.Text = m_nAlpha.ToString();
            btnColor.BackColor = Color.FromArgb(m_nAlpha, m_color);
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = m_color;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (dlg.Color == m_color)
                    return;
                else
                {
                    m_color = dlg.Color;
                    btnColor.BackColor = Color.FromArgb(m_nAlpha, m_color);
                    SetRGB();
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (textBox.Text.Length == 0)
                return;

            int nColor;

            if (!int.TryParse(textBox.Text, out nColor))
            {
                UnE.Utility.UMessageBox.Show(this, "숫자만 입력 가능합니다.", "색상 입력", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //MessageBox.Show("숫자만 입력 가능합니다.");

                if (m_dicPrevText.ContainsKey(textBox))
                    textBox.Text = m_dicPrevText[textBox];
                else
                    textBox.Text = "";

                textBox.Focus();
            }
            else
            {
                if (nColor >= 0 && nColor <= 255)
                    m_dicPrevText[textBox] = textBox.Text;
                else
                {
                    UnE.Utility.UMessageBox.Show(this, "0~255 사이의 숫자만 입력 가능합니다.", "색상 입력", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //MessageBox.Show("0~255 사이의 숫자만 입력 가능합니다.");

                    if (m_dicPrevText.ContainsKey(textBox))
                        textBox.Text = m_dicPrevText[textBox];
                    else
                        textBox.Text = "";

                    textBox.Focus();
                }
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (textBoxR.Text.Length == 0)
                textBoxR.Text = "0";

            if (textBoxG.Text.Length == 0)
                textBoxG.Text = "0";

            if (textBoxB.Text.Length == 0)
                textBoxB.Text = "0";

            int r = int.Parse(textBoxR.Text);
            int g = int.Parse(textBoxG.Text);
            int b = int.Parse(textBoxB.Text);

            btnColor.BackColor = Color.FromArgb(r, g, b);
            m_color = btnColor.BackColor;
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnApply_Click(null, null);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (msg.Msg == WindowMessage.WM_KEYDOWN ||
                msg.Msg == WindowMessage.WM_CHAR ||
                msg.Msg == WindowMessage.WM_SYSKEYDOWN)
            {
                if (keyData == Keys.F1)
                {
                    FormMain.Instance.ShowHelp();
                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
