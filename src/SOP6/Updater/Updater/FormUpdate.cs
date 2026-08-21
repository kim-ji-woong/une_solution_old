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
using DBUtility2;

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
                updater.BeginUpdate();
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
                Utility ini = new Utility();
                string strProcName = ini.getinivalue("Server Connection Info", "process");
                string strArguments = "";

                if (strProcName == null || strProcName.Length == 0)
                    strProcName = "IntegratedManagement4";
                else
                {
                    int nIndex = GetEmptyIndex(strProcName);

                    if (nIndex >= 0)
                    {
                        strArguments = strProcName.Substring(nIndex + 1).Trim();
                        strProcName = strProcName.Substring(0, nIndex).Trim();
                    }
                }

                /*string strSiteID = ini.getinivalue("Server Connection Info", "siteid");

                string strProcName = "IntegratedManagement4";

                int nSiteID = 1;                
                int.TryParse(strSiteID, out nSiteID);
                if (nSiteID == 202)
                {
                    strProcName = "SOPSimulator_SKT";
                }
                else if (nSiteID == 203)
                {
                    strProcName = "SOPSimulator_LG";
                }*/

                Process proc = ProcessManager.Instance.GetProcess(strProcName);
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
                ProcessManager.Instance.RunStartProcess(strProcName + ".exe", strArguments);
            }
            Application.Exit();
        }

        private int GetEmptyIndex(string str)
        {
            int len = str.Length;

            for (int i=0;i<len;i++)
            {
                char ch = str.ElementAt(i);

                if (ch == ' ' || ch == '\t' || ch == '\r' || ch == '\n')
                    return i;
            }

            return -1;
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
