using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SenarioServiceTester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            
        }

        private ServiceReference1.PreSafeSoapClient service = new ServiceReference1.PreSafeSoapClient();
        private string m_szSelectedSenario = "";
        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            if (m_szSelectedSenario == "")
                return;
            
            try
            {
                ServiceReference1.ArrayOfString szResult = service.RunSenario(m_szSelectedSenario, true, 10.0f, true, 120, true, 0.01f, false, 0.0f, false, 0.0f, false, 0, true, true);
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("ResponseCode : " + szResult[0]);
                sb.AppendLine(szResult[1]);
                sb.AppendLine("ResultValue : " + szResult[2]);
                sb.AppendLine("Script : " + szResult[3]);
                this.textBox1.Text = sb.ToString();
            }
            catch(Exception ex)
            {

            }          
         
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                ServiceReference1.ArrayOfString szResult = service.SenarioList();
                if (szResult == null)
                    return;

                comboBox1.Items.Clear();
                foreach(string szSenario in szResult)
                {
                    comboBox1.Items.Add(szSenario);
                }
            }
            catch (Exception ex)
            {

            }   
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex < 0)
                return;
            m_szSelectedSenario = comboBox1.SelectedItem.ToString();
        }
    }
}
