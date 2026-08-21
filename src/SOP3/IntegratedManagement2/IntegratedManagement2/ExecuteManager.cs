using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Diagnostics;

namespace IntegratedManagement2
{
    public class ExecuteManager
    {
        public enum APP_TYPE { SOP_MANAGER = 0, SOP_SIMULATOR, TEAM_MANAGER, SOP_MESSANGER, SDMS };
   
        private FormMain m_frmMain = null;

        public ExecuteManager(FormMain frmMain)
        {
            m_frmMain = frmMain;
        }
	
        public void Run(string szProcName)
        {
            string strUserID = LoginManager.Instance.LoginID;
            string strUserName = LoginManager.Instance.LoginUserName;
            int nUserID = LoginManager.Instance.LoginUserID;
            bool isSimulationMode = FormMain.Instance.SimulationMode;

            if (szProcName == "SOPManager")
			{
				Process process = RunSOPManager(nUserID, strUserID, strUserName);
				if (process != null)
				{
					ProcessManager.Instance.AddProcess(new SOPProcessInfo(process));
				}
			}
            else if (szProcName == "SOPMonitoringSystem")
			{
				Process process = RunSOPSimulator(nUserID, strUserName);
				if (process != null)
				{
					ProcessManager.Instance.AddProcess(new SOPProcessInfo(process));
				}
			}
            else if (szProcName == "TeamManagementSystem")
			{
				Process process = RunTeamManager(nUserID);
				if (process != null)
				{
					ProcessManager.Instance.AddProcess(new SOPProcessInfo(process));
				}				
			}
            else if (szProcName == "MessageSend")
			{
				Process process = RunSOPMessanger(strUserID);
				if (process != null)
				{
					ProcessManager.Instance.AddProcess(new SOPProcessInfo(process));
				}
			}
            else if (szProcName == "SDMS" || szProcName == "SDMS1")
			{
				Process process = RunSDMS(nUserID, strUserName, isSimulationMode);
				if (process != null)
				{
					ProcessManager.Instance.AddProcess(new SOPProcessInfo(process));
				}
			}
        }


        public void Run(APP_TYPE type)
        {
            string strUserID = LoginManager.Instance.LoginID;
            string strUserName = LoginManager.Instance.LoginUserName;
            int nUserID = LoginManager.Instance.LoginUserID;
            bool isSimulationMode = FormMain.Instance.SimulationMode;

			if (type == APP_TYPE.SOP_MANAGER)
			{
				Process process = RunSOPManager(nUserID, strUserID, strUserName);
				if (process != null)
				{
					ProcessManager.Instance.AddProcess(new SOPProcessInfo(process));
				}
			}
			else if (type == APP_TYPE.SOP_SIMULATOR)
			{
				Process process = RunSOPSimulator(nUserID, strUserName);
				if (process != null)
				{
					ProcessManager.Instance.AddProcess(new SOPProcessInfo(process));
				}
			}
			else if (type == APP_TYPE.TEAM_MANAGER)
			{
				Process process = RunTeamManager(nUserID);
				if (process != null)
				{
					ProcessManager.Instance.AddProcess(new SOPProcessInfo(process));
				}				
			}
			else if (type == APP_TYPE.SOP_MESSANGER)
			{
				Process process = RunSOPMessanger(strUserID);
				if (process != null)
				{
					ProcessManager.Instance.AddProcess(new SOPProcessInfo(process));
				}
			}
			else if (type == APP_TYPE.SDMS)
			{
				Process process = RunSDMS(nUserID, strUserName, isSimulationMode);
				if (process != null)
				{
					ProcessManager.Instance.AddProcess(new SOPProcessInfo(process));
				}
			}
        }

		private Process RunSDMS(int nUserID, string strUserName, bool isSimulationMode)
        {
            int nMonitor1, nMonitor2;
            string szSDMS = DBUtility.RegUtil.ReadRegValue("Monitor Info", "SDMS");
            int.TryParse(szSDMS, out nMonitor1);

            if (nMonitor1 > 1)
                nMonitor2 = nMonitor1 - 1;
            else
                nMonitor2 = nMonitor1 + 1;

            string strSimulationMode = isSimulationMode ? "1" : "0";

			if (!ProcessManager.Instance.RunCheckProcess("SDMS"))
            {
                string strValue = nUserID.ToString() + " " + strUserName + " " + nMonitor1.ToString() + " " + strSimulationMode;
				return ProcessManager.Instance.RunStartProcess("SDMS.exe", strValue);
            }
			else if (!ProcessManager.Instance.RunCheckProcess("SDMS1"))
            {
                string strValue = nUserID.ToString() + " " + strUserName + " " + nMonitor2.ToString() + " " + strSimulationMode;
				return ProcessManager.Instance.RunStartProcess("SDMS1.exe", strValue);
            }
			return null;
        }

		private Process RunSOPMessanger(string strUserID)
        {
			if (!ProcessManager.Instance.RunCheckProcess("MessageSend"))
            {
				return ProcessManager.Instance.RunStartProcess("MessageSend.exe", strUserID);
            }
			return null;
        }

		private Process RunTeamManager(int nUserID)
        {
			if (!ProcessManager.Instance.RunCheckProcess("TeamManagementSystem"))
            {
				return ProcessManager.Instance.RunStartProcess("TeamManagementSystem.exe", nUserID.ToString());
            }
			return null;
        }

		private Process RunSOPSimulator(int nUserID, string strUserName)
        {
			if (!ProcessManager.Instance.RunCheckProcess("SOPMonitoringSystem"))
            {
                string strValue = nUserID.ToString() + " " + strUserName;
				return ProcessManager.Instance.RunStartProcess("SOPMonitoringSystem.exe", strValue);
            }
			return null;
        }

		private Process RunSOPManager(int nUserID, string strUserID, string strUserName)
        {
			if (!ProcessManager.Instance.RunCheckProcess("SOPManager"))
            {
                string strValue = nUserID.ToString() + " " + strUserID + " " + strUserName;
				return ProcessManager.Instance.RunStartProcess("SOPManager.exe", strValue);
            }
			return null;
        }

        
    }
}
