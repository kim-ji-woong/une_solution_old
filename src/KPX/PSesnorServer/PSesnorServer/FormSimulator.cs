using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JubixNetwork;
using DBUtility;


namespace PSensorServer
{
    public partial class FormSimulator : Form
    {
        public FormSimulator()
        {
            InitializeComponent();
        }

        private List<PipeSensor> m_PipeList = new List<PipeSensor>();
        private List<TankInfo> m_TankList = new List<TankInfo>();

        private void FormSimulator_Load(object sender, EventArgs e)
        {
          

            m_PipeList.Clear();
            m_PipeList.AddRange(KPXAlarmChecker.Instance.PipeList);

            m_TankList.Clear();
            m_TankList.AddRange(KPXAlarmChecker.Instance.TankList);


            comboBox1.Items.Clear();
            foreach(PipeSensor sensor in m_PipeList)
            {
                comboBox1.Items.Add(sensor.PipeName);
            }

            comboBox2.Items.Clear();
            foreach (TankInfo sensor in m_TankList)
            {
                comboBox2.Items.Add(sensor.Name);
            }

            //timer1.Interval = 3000;
            //timer1.Enabled = true;
            //timer1.Start();

            if (comboBox1.Items.Count > 0)
                comboBox1.SelectedIndex = 0;
            if (comboBox2.Items.Count > 0)
                comboBox2.SelectedIndex = 0;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if( checkBoxAllPipe.Checked == true)
            {
                comboBox1.Enabled = false;
            }
            else
            {
                comboBox1.Enabled = true;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAllTank.Checked == true)
            {
                comboBox2.Enabled = false;
            }
            else
            {
                comboBox2.Enabled = true;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void FormSimulator_FormClosing(object sender, FormClosingEventArgs e)
        {
            //timer1.Interval = 3000;
            //timer1.Enabled = false;
            //timer1.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {                
            if (checkBoxAllPipe.Checked == true)
            {
                string szText = textBoxPressure.Text;
                float fPressure;
                if (float.TryParse(szText, out fPressure))
                {
                    foreach (PipeSensor pipe in m_PipeList)
                    {
                        KPXAlarmChecker.Instance.SetPressure(pipe.PipeID, fPressure * 100.0f);
                    }
                }
            }
            else
            {
                object item = comboBox1.SelectedItem;
                string szPipeName = item.ToString();
                if (item != null)
                {
                    PipeSensor findPipe = null;
                    foreach (PipeSensor pipe in m_PipeList)
                    {
                        if (szPipeName == pipe.PipeName)
                        {
                            findPipe = pipe;
                            break;
                        }
                    }

                    if (findPipe != null)
                    {
                        string szText = textBoxPressure.Text;
                        float fPressure;
                        if (float.TryParse(szText, out fPressure))
                        {
                            KPXAlarmChecker.Instance.SetPressure(findPipe.PipeID, fPressure * 100.0f);
                        }
                    }
                }                    
            } 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (checkBoxAllTank.Checked == true)
            {
                string szText1 = textBoxFlow.Text;
                string szText2 = textBoxTemp.Text;
                string szText3 = textBoxLevel.Text;

                float fFlow;
                if (float.TryParse(szText1, out fFlow))
                {
                    foreach (TankInfo tank in m_TankList)
                    {
                        tank.Flow = fFlow;
                    }
                }

                float fTemp;
                if (float.TryParse(szText2, out fTemp))
                {
                    foreach (TankInfo tank in m_TankList)
                    {
                        tank.Temperature = fTemp;
                    }
                }

                float fLevel;
                if (float.TryParse(szText3, out fLevel))
                {
                    foreach (TankInfo tank in m_TankList)
                    {
                        tank.Level = fLevel;
                    }
                }
            }
            else
            {
                object item = comboBox2.SelectedItem;

                if (item != null)
                {
                    string szPipeName = item.ToString();
                    TankInfo findTank = null;
                    foreach (TankInfo tank in m_TankList)
                    {
                        if (szPipeName == tank.Name)
                        {
                            findTank = tank;
                            break;
                        }
                    }

                    string szText1 = textBoxFlow.Text;
                    string szText2 = textBoxTemp.Text;
                    string szText3 = textBoxLevel.Text;

                    if (findTank != null)
                    {
                        float fFlow;
                        if (float.TryParse(szText1, out fFlow))
                        {
                            findTank.Flow = fFlow;
                        }

                        float fTemp;
                        if (float.TryParse(szText2, out fTemp))
                        {
                            findTank.Temperature = fTemp;
                        }

                        float fLevel;
                        if (float.TryParse(szText3, out fLevel))
                        {
                            findTank.Level = fLevel;
                        }
                    }
                }                
            }            
        }


        private KPXSimulator sim = new KPXSimulator();
        private bool m_bFlow = true;
        private bool m_bOptions = true;
        private bool m_bWorks = true;
        private bool m_bPressure = true;
        private int nSpeed = 1;

        private void button7_Click(object sender, EventArgs e)
        {
            DateTime dtStart = dateTimePicker1.Value;

            sim.StartDate = dtStart;
            sim.Speed = nSpeed;
            sim.UseFlow = m_bFlow;
            sim.UsePressure = m_bPressure;
            sim.UseOptions = m_bOptions;
            sim.UseWork = m_bWorks;
            sim.Start();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            sim.Stop();
        }
    
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                nSpeed = 12;
            }           
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked == true)
            {
                nSpeed = 6;
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked == true)
            {
                nSpeed = 1;
            }
        }
        
        // 유량
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox3.Checked == true)
            {
                m_bFlow = true;
            }
        }
    
        // 압력
        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked == true)
            {
                m_bPressure = true;
            }
        }
       
        // 옵션
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked == true)
            {
                m_bOptions = true;
            }
        }
     

        // 배관/탱크 작업
        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked == true)
            {
                m_bWorks = true;
            }
        }       
    }
}
