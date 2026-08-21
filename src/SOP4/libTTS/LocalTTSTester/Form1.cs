using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LocalTTSTester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Test Server 위치
            // Server : 218.235.67.30
            // Port : 23456

            string szText = textBox4.Text;
            if( szText != null && szText != "")
            {
                try
                {
                    using (libTTS.Broadcast br = new libTTS.Broadcast(textBox1.Text, textBox2.Text))
                    {
                        int nCount = 1;
                        int.TryParse(textBox3.Text, out nCount);
                        if (nCount == 0)
                            nCount = 1;
                        br.AddSpeech(szText, nCount, m_bUseSiren);
                    }
                }
                catch(Exception ex)
                {

                }
                
            }
        }
        private bool m_bUseSiren = false;
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
           // if(checkBox1.Checked == true)
            {
                m_bUseSiren = checkBox1.Checked;
            }
        }
    }
}
