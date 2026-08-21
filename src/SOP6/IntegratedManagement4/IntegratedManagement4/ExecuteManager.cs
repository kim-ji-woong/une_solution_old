using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Diagnostics;
using DBUtility2;

namespace IntegratedManagement4
{
    public class ExecuteManager
    {
        public enum APP_TYPE { SOP_MANAGER = 0, SOP_SIMULATOR, TEAM_MANAGER, SOP_MESSANGER, SDMS, ETC, SOP_WEATHER, BULLETIN };
   
        private FormMain m_frmMain = null;

        public ExecuteManager(FormMain frmMain)
        {
            m_frmMain = frmMain;
        }
	
        public void Run(string szProcName)
        {
            int nSiteID = FormMain.Instance.SiteID;
            string strUserID = LoginManager.Instance.LoginID;
            string strUserName = LoginManager.Instance.LoginUserName;
            int nUserID = LoginManager.Instance.LoginUserID;

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
                Process process = RunTeamManager(nUserID, strUserName, nSiteID);
				//Process process = RunTeamManager(nUserID);
				if (process != null)
				{
					ProcessManager.Instance.AddProcess(new SOPProcessInfo(process));
				}				
			}
            else if (szProcName == "MessageSend")
			{
                Process process = RunSOPMessanger(nUserID.ToString(), nSiteID.ToString());
				if (process != null)
				{
					ProcessManager.Instance.AddProcess(new SOPProcessInfo(process));
				}
			}
            else if (szProcName == "SDMS" || szProcName == "SOPSimulator")
			{
				Process process = RunSDMS(nUserID, strUserName);
				if (process != null)
				{
					ProcessManager.Instance.AddProcess(new SOPProcessInfo(process));
				}
			}
        }


        public void Run(APP_TYPE type)
        {
            int nSiteID = FormMain.Instance.SiteID;
            string strUserID = LoginManager.Instance.LoginID;
            string strUserName = LoginManager.Instance.LoginUserName;
            int nUserID = LoginManager.Instance.LoginUserID;

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
                Process process1 = RunSDMS(nUserID, strUserName);
                if (process1 != null)
                {
                    ProcessManager.Instance.AddProcess(new SOPProcessInfo(process1));
                }

                Process process2 = RunSOPSimulator(nUserID, strUserName);
                if (process2 != null)
                {
                    ProcessManager.Instance.AddProcess(new SOPProcessInfo(process2));
                }
			}
			else if (type == APP_TYPE.TEAM_MANAGER)
			{
                Process process = RunTeamManager(nUserID, strUserName, nSiteID);
				//Process process = RunTeamManager(nUserID);
				if (process != null)
				{
					ProcessManager.Instance.AddProcess(new SOPProcessInfo(process));
				}				
			}
			else if (type == APP_TYPE.SOP_MESSANGER)
			{
                Process process = RunSOPMessanger(nUserID.ToString(), nSiteID.ToString());
				if (process != null)
				{
					ProcessManager.Instance.AddProcess(new SOPProcessInfo(process));
				}

                FormMain.Instance.HideEtcButtons();
			}
			else if (type == APP_TYPE.SDMS)
			{
                Process process = RunSDMS(nUserID, strUserName);
                if (process != null)
                {
                    ProcessManager.Instance.AddProcess(new SOPProcessInfo(process));
                }
			}
            else if (type == APP_TYPE.ETC)
            {
                FormMain.Instance.ShowEtcButtons();
            }
        }

        // SOPSimulator 화면을 로딩시 바로 보이도록 할 것인가?
        private bool GetSOPSimulatorVisible()
        {
            string strSQL = "Select PropertyValue from OptionSOPSimulator where PropertyName = 'ShowSOPFromBeginning' and SiteID = " + m_frmMain.SiteID.ToString();
            ArrayList arrResult = m_frmMain.DBManager.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return false;

            string strValue = WebDBManager.GetStringField(arrResult[0]);

            if (strValue == null)
                return false;

            strValue = strValue.Trim().ToLower();

            if (strValue.Trim().ToLower() == "true" || strValue == "1")
                return true;

            return false;
        }

		private Process RunSDMS(int nUserID, string strUserName)
        {
            Process process = ProcessManager.Instance.RunCheckProcess("SDMS");

            if (process == null)
            {
                string strValue = nUserID.ToString() + " " + strUserName + " 1 0";
                return ProcessManager.Instance.RunStartProcess("SDMS.exe", strValue);
            }
            else
            {
                // SDMS가 이미 실행중이면 Toggle Hide 상태일 수도 있으니 일단 ShowSDMS를 보낸다.
                FormMain.Instance.NetworkServer.ServiceProvider.SendShowSDMS();
            }

			return process;
        }

		private Process RunSOPMessanger(string strUserID, string szSiteID)
        {
			if (ProcessManager.Instance.RunCheckProcess("MessageSend") == null)
            {
                string strValue = strUserID + " " + szSiteID;
                return ProcessManager.Instance.RunStartProcess("MessageSend.exe", strValue);
            }
			return null;
        }

        private Process RunTeamManager(int nUserID, string strUserName, int nSiteID)
        {
            if (ProcessManager.Instance.RunCheckProcess("TeamEditor") == null)
            {
                string strArgument = nUserID.ToString() + " " + nSiteID.ToString()/* + " " + strUserName*/;
                return ProcessManager.Instance.RunStartProcess("TeamEditor.exe", strArgument);
            }
            return null;
        }
        /*private Process RunTeamManager(int nUserID)
        {
            if (!ProcessManager.Instance.RunCheckProcess("TeamManagementSystem"))
            {
                return ProcessManager.Instance.RunStartProcess("TeamManagementSystem.exe", nUserID.ToString());
            }
            return null;
        }*/

		private Process RunSOPSimulator(int nUserID, string strUserName)
        {
            NetworkServer netServer = FormMain.Instance.NetworkServer;
            Process process = ProcessManager.Instance.RunCheckProcess("SOPSimulator2");

            if (process == null)
            {
                string cctvMode = "1";

                string strValue = string.Format("{0} {1} {2} {3} {4} {5}",
                    nUserID.ToString(), strUserName, "1", "1", "1", cctvMode);

                return ProcessManager.Instance.RunStartProcess("SOPSimulator2.exe", strValue);
            }
            else
            {
                // SDMS가 이미 실행중이면 Toggle Hide 상태일 수도 있으니 일단 ShowSOPSimulator를 보낸다.
                FormMain.Instance.NetworkServer.ServiceProvider.SendShowSOPSimulator();
            }

			return null;
        }

		private Process RunSOPManager(int nUserID, string strUserID, string strUserName)
        {
			if (ProcessManager.Instance.RunCheckProcess("SOPManager") == null)
            {
                string strValue = nUserID.ToString() + " " + strUserID;
                //string strValue = nUserID.ToString() + " " + strUserID + " " + strUserName;
                return ProcessManager.Instance.RunStartProcess("SOPManager2.exe", strValue);
            }
			return null;
        }

        
    }
}
