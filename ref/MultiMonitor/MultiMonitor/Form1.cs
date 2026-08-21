using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace MultiMonitor
{
    public partial class Form1 : Form
    {
        private Form2 form2 = new Form2();
        private Form3 form3 = new Form3();
        private Form4 form4 = new Form4();

        public Form1()
        {
            InitializeComponent();
        }

        private bool SetMonitorForm(Form form, int nDisplay)
        {
            Screen[] sc;
            sc = Screen.AllScreens;
            
            if (sc.Length == 0)
            {
                return false;
            }

            string szNum = nDisplay.ToString();
            int nIdx = -1;
            for (int i = 0; i < sc.Length; i++)
            {
                if (sc[i].DeviceName.IndexOf(szNum) != -1)
                {
                    nIdx = i;
                    break;
                }
            }

            if (nIdx == -1)
                nIdx = 0;

            if (sc.Length >= nDisplay)
            {
                form.StartPosition = FormStartPosition.Manual;
                form.Location = sc[nIdx].Bounds.Location;
                form.Size = new Size(1920, 1080);
                form.WindowState = FormWindowState.Maximized;
            }            
            return true;
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            SetMonitorForm(this, 2);
            SetMonitorForm(form2, 3);
            SetMonitorForm(form3, 4);

            form2.Show();
            form3.Show();
        }
    }
}
