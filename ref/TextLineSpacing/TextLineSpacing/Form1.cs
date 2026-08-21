using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextLineSpacing
{
    public partial class Form1 : Form
    {

        float m_fLineSpacing = 10.0f;
        Font m_SelectedFont;
        SolidBrush m_TextBrush = new SolidBrush(Color.Black);
        Pen m_Pen = new Pen(Color.Black);

        public Form1()
        {
            InitializeComponent();

            m_SelectedFont = textBox1.Font;
        }

        private TextLineSpaceRenderer textRenderer = new TextLineSpaceRenderer();
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            RectangleF rect = new RectangleF(0.0f, 0.0f, (float)panel1.Size.Width - 1, (float)panel1.Size.Height - 1);
            
            Graphics g = e.Graphics;

            string szText = textBox1.Text;

            textRenderer.DrawText(g, szText, m_SelectedFont, m_TextBrush, rect, m_fLineSpacing);
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string szValue = textBox2.Text;

            if (szValue == null || szValue == "")
                return;

            if(!float.TryParse(szValue, out m_fLineSpacing))
            {
                m_fLineSpacing = 10.0f;
            }

            panel1.Invalidate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            fontDialog1.ShowColor = true;
            fontDialog1.ShowEffects = true;
            fontDialog1.FontMustExist = true;

            if(fontDialog1.ShowDialog() == DialogResult.OK)
            {
                m_SelectedFont = fontDialog1.Font;
                Color color = fontDialog1.Color;

                m_TextBrush.Color = color;
            }

            panel1.Invalidate();
        }
    }
}
