using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using System.Diagnostics;

namespace Updater
{
    public partial class FormUpdate : Form
    {
        private static int nCount = 0;
        private AutoUpdater updater = new AutoUpdater();

        public FormUpdate()
        {
            InitializeComponent();
        }

        private void FormUpdate_Load(object sender, EventArgs e)
        {
            ProcessManager.Instance.InitProcess();

            this.BringToFront();

            if (updater.CheckUpdateXML())
            {
                timer1.Interval = 200;
                timer1.Enabled = true;
            }
            else
            {
                ExitUpdate();
            }
        }          
        
        private void FormUpdate_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void ExitUpdate()
        {
            //if (!ProcessManager.Instance.RunCheckProcess("IntegratedManagement2"))
            {
                Process proc = ProcessManager.Instance.GetProcess("IntegratedManagement2");
                if (proc != null)
                {
                    try
                    {
                        proc.Kill();
                       
                    }
                    catch (System.Exception)
                    {                    	
                    }

                    try
                    {
                        proc.WaitForExit(5000);
                    }
                    catch (System.Exception)
                    {
                    }
                }
                ProcessManager.Instance.RunStartProcess("IntegratedManagement2.exe", "");
            }
            Application.Exit();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            nCount++;
            this.progressBar1.Value = nCount;
            if (nCount == 100)
            {
                nCount = -1;
            }
            if (updater.IsExitUpdate)
            {
                ExitUpdate();
            }           
        }
       
        private void FormUpdate_Shown(object sender, EventArgs e)
        {
            updater.AutoUpdate();
        }

        private void FormUpdate_Activated(object sender, EventArgs e)
        {  
        }
    } 
}
