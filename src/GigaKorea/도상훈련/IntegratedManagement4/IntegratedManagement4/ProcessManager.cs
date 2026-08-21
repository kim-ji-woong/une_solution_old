using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;


namespace IntegratedManagement4
{	
	public class ProcessManager
	{
		private static ProcessManager m_instance = null;
		public static ProcessManager Instance
		{
			get 
			{
				if (m_instance == null)
					m_instance = new ProcessManager();
				return m_instance; 
			}
			set { m_instance = value; }
		}

		SortedList<string, SOPProcessInfo> m_arProcList = new SortedList<string, SOPProcessInfo>();
        public SortedList<string, SOPProcessInfo> ProcList
        {
            get { return m_arProcList; }
            set { m_arProcList = value; }
        }

        private List<int> m_processIDs = new List<int>();

		public ProcessManager()
		{
		}      

		public void InitProcess()
		{
            string[] szTarget = { "SOPSimulator", "ControlTeamEditor", "TeamEditor", "SOPManager", "CCTVViewer" ,
            "SensorTester", "HwpReport", "SDMS", "libCCTV", "SOPBulletin", "SMSSender", "BroadRunner"};

            System.Diagnostics.Process[] processList = System.Diagnostics.Process.GetProcesses();

            foreach (System.Diagnostics.Process process in processList)
            {
                if (IsStartWith(process.ProcessName, szTarget))
                {
                    process.Kill();
                }
            }

            /*for (int i = 0; i < szTarget.Length; i++)
			{
				Process process = GetProcess(szTarget[i]);
				if (process != null)
				{
					try
					{
						process.Kill();
					}
					catch (System.Exception)
					{						
					}
					
				}
			}*/
        }

        private bool IsStartWith(string strProcessName, string[] processList)
        {
            foreach (string strName in processList)
            {
                if (strProcessName.StartsWith(strName))
                    return true;
            }

            return false;
        }

		private string GetExecutablePath()
		{
			string strExePath = Application.ExecutablePath;
			int nIndex = strExePath.LastIndexOf('\\');
			string strTemp = strExePath.Substring(0, nIndex);

			return strTemp + "\\";
		}

		public System.Diagnostics.Process RunStartProcess(string strFileName, string args)
		{
			System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
			startInfo.FileName = strFileName;
			startInfo.WorkingDirectory = GetExecutablePath();
			startInfo.ErrorDialog = true;
			startInfo.Arguments = args;

			System.Diagnostics.Process process;
			try
			{
				process = System.Diagnostics.Process.Start(startInfo);

				return process;
			}
			catch (Exception ex)
			{
				//System.Windows.Forms.MessageBox.Show(ex.Message);
                UnE.Utility.UMessageBoxRibbon.Show(ex.Message, "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			return null;
		}

		//strProcessName을 가진 프로그램이 실행중인지 체크
		public Process RunCheckProcess(string strProcessName)
		{
			System.Diagnostics.Process[] processList = System.Diagnostics.Process.GetProcesses();

			foreach (System.Diagnostics.Process process in processList)
			{
				if (process.ProcessName == strProcessName)
					return process;
			}

			return null;
		}

        public void KillAllProcess(string strProcessName)
        {
            System.Diagnostics.Process[] processList = System.Diagnostics.Process.GetProcesses();
            foreach (System.Diagnostics.Process process in processList)
            {
                if (process.ProcessName == strProcessName)
                {
                    try
                    {
                        process.Kill();
                    }
                    catch(Exception)
                    {
                        
                    }
                }

            }
        }

		public Process GetProcess(string strProcessName)
		{
			System.Diagnostics.Process[] processList = System.Diagnostics.Process.GetProcesses();

			foreach (System.Diagnostics.Process process in processList)
			{
				if (process.ProcessName == strProcessName)
					return process;
			}
			return null;
		}


		public void AbortAllProcess()
		{
			foreach (KeyValuePair<string, SOPProcessInfo> pair in m_arProcList)
			{
				SOPProcessInfo proc = (SOPProcessInfo)pair.Value;
				proc.AboartProcess();
			}

            KillAllProcess("CCTVViewer");
            KillAllProcess("CCTVViewer2");
            KillAllProcess("libCCTV");
            KillAllProcess("libCCTV2");

			// Parc1 Unity 관련
			if (FormMain.Instance.DBManager.SiteID == 201)
			{
				KillAllProcess("Parc1Unity");
				KillAllProcess("UnityCrashHandler64");
			}
			

			foreach (int nProcessID in m_processIDs)
            {
                try
                {
                    Process process = Process.GetProcessById(nProcessID);

                    if (process != null)
                        process.Kill();
                }
                catch (Exception)
                {
                }
            }

            m_processIDs.Clear();
		}

		public void RestartAllProcess()
		{
			Thread t = new Thread(RestartThread);
			t.Start();
		}

        public void ClearNAddProcessIDs(List<int> ids)
        {
            m_processIDs.Clear();
            m_processIDs.AddRange(ids);
        }

        public void AddProcessIDs(List<int> ids)
        {
            m_processIDs.AddRange(ids);
        }

        public void RemoveProcessIDs(List<int> ids)
        {
            foreach (int nProcessID in ids)
            {
                m_processIDs.Remove(nProcessID);
            }
        }

        public void ClearProcessIDs()
        {
            m_processIDs.Clear();
        }

        public void RestartThread()
		{
			
			foreach (KeyValuePair<string, SOPProcessInfo> pair in m_arProcList)
			{
				SOPProcessInfo proc = (SOPProcessInfo)pair.Value;
				proc.RestartProcess();
			}
		}

		public void AddProcess(SOPProcessInfo process)
		{
			if (!m_arProcList.ContainsKey(process.ProcessName))
			{
				m_arProcList.Add(process.ProcessName, process);
			}
			else
			{
				m_arProcList.Remove(process.ProcessName);
				m_arProcList.Add(process.ProcessName, process);
			}	
		}
	}

	public class SOPProcessInfo
	{
		private System.Diagnostics.Process m_Process = null;
		public System.Diagnostics.Process Process
		{
			get { return m_Process; }
			set { m_Process = value; }
		}

		private DateTime m_StartTime;
		public System.DateTime StartTime
		{
			get { return m_StartTime; }
			set { m_StartTime = value; }
		}

		private string m_szProcessName = "";
		public string ProcessName
		{
			get { return m_szProcessName; }
			set { m_szProcessName = value; }
		}
		
		private bool m_bExitProcess = false;
		public bool Exited
		{
			get { return m_bExitProcess; }
			set { m_bExitProcess = value; }
		}

		public SOPProcessInfo(System.Diagnostics.Process process)
		{
			m_Process = process;		
			m_StartTime = m_Process.StartTime;
			m_szProcessName = m_Process.ProcessName;
			process.EnableRaisingEvents = true;
			process.Exited += new EventHandler(OnExitProcess);
		}

		private void OnExitProcess(object sender, EventArgs e)
		{
			m_bExitProcess = true;
		}

		public bool AboartProcess()
		{
			if (m_Process == null)
				return false;

			if (m_bExitProcess == true)
				return false;
			try
			{
				m_Process.Kill();
			}
			catch (System.Exception e)
			{
				Debug.WriteLine(e);
			}
			return true;
		}

		public bool RestartProcess()
		{
			if (m_Process == null)
				return false;

			if (m_bExitProcess == true)
				return false;
			try
			{
				m_Process.Kill();
			}
			catch (System.Exception e)
			{
				Debug.WriteLine(e);
			}

			try
			{
				m_Process.Start();
			}
			catch (System.Exception e)
			{
				Debug.WriteLine(e);
			}		

			m_bExitProcess = false;
			m_StartTime = m_Process.StartTime;

			return true;
		}

		private bool CheckProcess()
		{
			if (m_bExitProcess == true)
				return false;

			if (m_Process != null)
			{
				bool bExit = m_Process.HasExited;
				return !bExit;
			}
			return false;
		}
	}
}
