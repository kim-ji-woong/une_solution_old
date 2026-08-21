using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireSimulator
{
    public class ExecuteManager
    {
        public enum APP_TYPE { SAMPLE_PROJECT = 0 };

        private string m_strID = null;
        private string m_strPW = null;

        private FormMain m_frmMain = null;

        public ExecuteManager(FormMain frmMain)
        {
            m_frmMain = frmMain;

            Init();
        }

        private void Init()
        {
            string strID = ConfigurationManager.AppSettings.Get("SampleProject_ID");
            if (strID == null || strID.Length == 0)
                strID = "user_spatial";

            string strPW = ConfigurationManager.AppSettings.Get("SampleProject_PW");
            if (strPW == null || strPW.Length == 0)
                strPW = "spatial1234";

            m_strID = strID;
            m_strPW = strPW;
        }

		public void Run(string szProcName)
		{
			if (szProcName == "SampleProject")
			{
                Process process = RunSampleProject(m_strID, m_strPW);
                if (process != null)
                {
                    ProcessManager.Instance.AddProcess(new SOPProcessInfo(process));
                }
            }
			
		}

        public void Run(APP_TYPE type)
        {
            if (type == APP_TYPE.SAMPLE_PROJECT)
            {
                Process process = RunSampleProject(m_strID, m_strPW);
                if (process != null)
                {
                    ProcessManager.Instance.AddProcess(new SOPProcessInfo(process));
                }
            }
            
        }

        private Process RunSampleProject(string strID, string strPW)
        {
            if (ProcessManager.Instance.RunCheckProcess("SampleProject") == null)
            {
                string strValue = strID + " " + strPW;
                
                return ProcessManager.Instance.RunStartProcess("SampleProject.exe", strValue);
            }
            return null;
        }
    }
}
