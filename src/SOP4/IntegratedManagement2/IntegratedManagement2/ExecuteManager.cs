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
        public enum APP_TYPE { SOP_MANAGER = 0, SOP_SIMULATOR, TEAM_MANAGER, SOP_MESSANGER, SDMS, TRAINING, SOP_WEATHER, BULLETIN };
   
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
				Process process = RunSOPSimulator(nUserID, strUserName, FormMain.Instance.SimulationMode);
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
				Process process = RunSDMS(nUserID, strUserName, isSimulationMode);
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
                if (FormMain.Instance.SimulationMode)
                    ProgressMessageBox.Show();

                Process process = RunSOPSimulator(nUserID, strUserName, FormMain.Instance.SimulationMode);
                if (process != null)
                {
                    ProcessManager.Instance.AddProcess(new SOPProcessInfo(process));
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
                if (FormMain.Instance.SimulationMode)
                    ProgressMessageBox.Show();

                Process process = RunSDMS(nUserID, strUserName, isSimulationMode);
                if (process != null)
                {
                    ProcessManager.Instance.AddProcess(new SOPProcessInfo(process));
                }
			}
            else if (type == APP_TYPE.SOP_WEATHER)
            {
                Process process = RunSOPWeather(nUserID.ToString(), nSiteID.ToString(), isSimulationMode);

                if (process != null)
                {
                    ProcessManager.Instance.AddProcess(new SOPProcessInfo(process));
                }

                FormMain.Instance.HideEtcButtons();
            }
            else if(type == APP_TYPE.TRAINING)  // 훈련평가
            {
                Process process = RunTrainingEvaluation(nUserID, strUserName);
            }
        }

        private Process RunSOPWeather(string strUserID, string szSiteID, bool isSimulationMode)
        {
            if (!ProcessManager.Instance.RunCheckProcess("WeatherSimulator"))
            {
                string strValue = strUserID + " " + szSiteID;

                if (isSimulationMode)
                    strValue += " 0";
                else
                    strValue += " 1";

                return ProcessManager.Instance.RunStartProcess("WeatherSimulator.exe", strValue);
            }
            return null;
        }

        // SOPSimulator 화면을 로딩시 바로 보이도록 할 것인가?
        private bool GetSOPSimulatorVisible()
        {
            string strSQL = "Select PropertyValue from OptionSOPSimulator where PropertyName = 'ShowSOPFromBeginning' and SiteID = " + m_frmMain.SiteID.ToString();
            ArrayList arrResult = m_frmMain.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return false;

            string strValue = DBUtility.WebDBManager.GetStringField(arrResult[0]);

            if (strValue == null)
                return false;

            strValue = strValue.Trim().ToLower();

            if (strValue.Trim().ToLower() == "true" || strValue == "1")
                return true;

            return false;
        }

		private Process RunSDMS(int nUserID, string strUserName, bool isSimulationMode)
        {
            string strSimulationMode = isSimulationMode ? "0" : "1";

            //string cctvMode = DBUtility.RegUtil.ReadRegValue("IntegratedManager", "cctv_mode", m_frmMain.SiteID);
            //if (cctvMode == null || cctvMode == "")
            //    cctvMode = "1"; 
            // CCTV모드는 기본적으로 상황실 모드를 사용함. skkim 2016-07-26
            string cctvMode = "1";

            if (!ProcessManager.Instance.RunCheckProcess("SOPSimulator"))
            {
                string strVisibleSOPSimulator = GetSOPSimulatorVisible() ? " 1 " : " 0 ";
                string strValue = nUserID.ToString() + " " + strUserName + " 1 " + strSimulationMode + strVisibleSOPSimulator + cctvMode;
                return ProcessManager.Instance.RunStartProcess("SOPSimulator.exe", strValue);
            }
            //else if (!ProcessManager.Instance.RunCheckProcess("SOPSimulator1"))
            //{
            //    string strValue = nUserID.ToString() + " " + strUserName + " 2 " + strSimulationMode + " 0 " + cctvMode;
            //    return ProcessManager.Instance.RunStartProcess("SOPSimulator1.exe", strValue);
            //}
			return null;
        }

		private Process RunSOPMessanger(string strUserID, string szSiteID)
        {
			if (!ProcessManager.Instance.RunCheckProcess("MessageSend"))
            {
                string strValue = strUserID + " " + szSiteID;
                return ProcessManager.Instance.RunStartProcess("MessageSend.exe", strValue);
            }
			return null;
        }

        private Process RunTeamManager(int nUserID, string strUserName, int nSiteID)
        {
            if (!ProcessManager.Instance.RunCheckProcess("TeamEditor"))
            {
                string strArgument = nUserID.ToString() + " " + nSiteID.ToString() + " " + strUserName;
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

		private Process RunSOPSimulator(int nUserID, string strUserName, bool isSimulationMode)
        {
            NetworkServer netServer = FormMain.Instance.NetworkServer;

            TcpLib2.ConnectionState sdms1, sdms2;

            netServer.ServiceProvider.GetSDMSClients(out sdms1, out sdms2);

			/*if (!ProcessManager.Instance.RunCheckProcess("SOPMonitoringSystem"))
            {
                string strValue = nUserID.ToString() + " " + strUserName;
				return ProcessManager.Instance.RunStartProcess("SOPMonitoringSystem.exe", strValue);
            }*/
            if (ProcessManager.Instance.RunCheckProcess("SOPSimulator"))
            {
                if (ProcessManager.Instance.RunCheckProcess("SOPSimulator1"))
                {
                    if (sdms2 != null)
                        netServer.ProcessCommand(sdms2, NetworkServer.Command.CHECK_SOP_SIMULATOR1_N_RUN_SOP_SIMULATOR0);
                    else
                        netServer.ProcessCommand(null, NetworkServer.Command.RESERVE_CHECK_SOP_SIMULATOR1_N_RUN_SOP_SIMULATOR0);
                }
                else
                {
                    if (sdms1 != null)
                        netServer.ProcessCommand(sdms1, NetworkServer.Command.RUN_SOP_SIMULATOR0);
                    else
                        netServer.ProcessCommand(null, NetworkServer.Command.RESERVE_RUN_SOP_SIMULATOR0);
                }
            }
            else if (ProcessManager.Instance.RunCheckProcess("SOPSimulator1"))
            {
                if (sdms2 != null)
                    netServer.ProcessCommand(sdms2, NetworkServer.Command.RUN_SOP_SIMULATOR1);
                else
                    netServer.ProcessCommand(null, NetworkServer.Command.RESERVE_RUN_SOP_SIMULATOR1);
            }
            else
            {

                //string cctvMode = DBUtility.RegUtil.ReadRegValue("IntegratedManager", "cctv_mode", m_frmMain.SiteID);
                // if (cctvMode == null || cctvMode == "")
                 
                string cctvMode = "1";

                string strValue = string.Format("{0} {1} {2} {3} {4} {5}",
                    nUserID.ToString(), strUserName, "1",
                    isSimulationMode ? "0" : "1",
                    "1", cctvMode);

                return ProcessManager.Instance.RunStartProcess("SOPSimulator.exe", strValue);
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

        private Process RunTrainingEvaluation(int nUserID, string strUserID)
        {
            if (!ProcessManager.Instance.RunCheckProcess("TrainingEvaluation"))
            {
                string strValue = nUserID.ToString() + " " + strUserID;
                return ProcessManager.Instance.RunStartProcess("TrainingEvaluation.exe", strValue);
            }
            return null;
        }
    }
}
